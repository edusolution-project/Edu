using BaseCustomerEntity.Database;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyChatApp
{
    public class EasyChatHub : Hub
    {
        private readonly static GroupMapping<string> _userMapping = new GroupMapping<string>();
        private readonly static ConnectIdToUser _connectIdToUser = new ConnectIdToUser();
        private readonly static GroupMapping<string> _groupToUsers = new GroupMapping<string>();
        private readonly GroupAndUserService _groupAndUserService;
        public EasyChatHub(GroupAndUserService groupAndUserService)
        {
            _groupAndUserService = groupAndUserService;
        }

        [Obsolete]
        public async Task Online(string user, List<string> groupNames)
        {
            var connectionId = Context.ConnectionId;
            for(int  i =  0; groupNames != null && i < groupNames.Count; i++)
            {
                var groupName = groupNames[i];
                // neu chua ton tai thi add join group
                if (!_groupToUsers.GetGroupConnections(user).Contains(groupName))
                {
                    //add to group
                    await Groups.AddToGroupAsync(connectionId, groupName);
                    // tao khi join vao group lan dau
                    await _groupAndUserService.CreateTimeJoin(groupName, user);
                }
                _groupToUsers.Add(user, groupName);
                await Clients.OthersInGroup(groupName).SendAsync("OnlineToGroup",groupName, user);
            }
            _userMapping.Add(user, connectionId);
            _connectIdToUser.Add(connectionId, user);
            await Clients.Others.SendAsync("Online", user, connectionId);
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string connectionId = Context.ConnectionId;
            string user = _connectIdToUser.Get(connectionId);
            if (!string.IsNullOrEmpty(user))
            {
                //bo ra khoi mapconnection
                _connectIdToUser.Remove(connectionId);
                //bo ra khoi dong connection dang ton tai
                _userMapping.Remove(user,connectionId);
                // check cho connection con ton tai
                var connects = _userMapping.GetGroupConnections(user);
                if (connects == null || connects.Count() == 0)
                {
                    // update time life cho user
                    _ = _groupAndUserService.UpdateTimeLife(user);
                }
            }
            return base.OnDisconnectedAsync(exception);
        }
    }

    public class GroupMapping<T>
    {
        private readonly Dictionary<T, HashSet<string>> _connections = new Dictionary<T, HashSet<string>>();

        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public void Add(T key, string connectionId)
        {
            lock (_connections)
            {
                if (!_connections.TryGetValue(key, out HashSet<string> connections))
                {
                    connections = new HashSet<string>();
                    _connections.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }

        public IEnumerable<string> GetGroupConnections(T key)
        {
            if (_connections.TryGetValue(key, out HashSet<string> connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }

        public void Remove(T key, string connectionId)
        {
            lock (_connections)
            {
                if (!_connections.TryGetValue(key, out HashSet<string> connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        _connections.Remove(key);
                    }
                }
            }
        }
    }
    public class ConnectIdToUser
    {
        private readonly Dictionary<string,string> _connections = new Dictionary<string,string>();

        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public string Get(string connectiondId)
        {
            if(_connections.TryGetValue(connectiondId,out string user))
            {
                return user;
            }
            return string.Empty;
        }

        public void Add(string connectionId, string userId)
        {
            lock (_connections)
            {
                if (!_connections.TryGetValue(connectionId, out string user))
                {
                    _connections.Add(connectionId, user);
                }
                else
                {
                    _connections.Remove(connectionId);
                    _connections.Add(connectionId, userId);
                }
            }
        }

        public void Remove(string connectionId)
        {
            lock (_connections)
            {
                if (_connections.TryGetValue(connectionId, out string user))
                {
                    _connections.Remove(connectionId);
                }
                
            }
        }
    }
}
