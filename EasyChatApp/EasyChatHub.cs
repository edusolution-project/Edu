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
        #region local memory
        private readonly static ConnectIdToCurrentUser _connectIdToCurrentUser = new ConnectIdToCurrentUser();
        private static readonly GroupMapping<string> _userMapping = new GroupMapping<string>();
        private readonly static ConnectIdToUser _connectIdToUser = new ConnectIdToUser();
        private readonly static GroupMapping<string> _groupToUsers = new GroupMapping<string>();

        internal static GroupMapping<string> UserMap
        {
            get
            {
                return _userMapping;
            }
        }
        internal static GroupMapping<string> GroupMapping
        {
            get
            {
                return _groupToUsers;
            }
        }
        #endregion
        private readonly GroupAndUserService _groupAndUserService;
        public EasyChatHub(GroupAndUserService groupAndUserService)
        {
            _groupAndUserService = groupAndUserService;
        }
        [Obsolete]
        public async Task Online(string user, string groups)
        {
            var connectionId = Context.ConnectionId;
            List<string> groupNames = groups.Split(',')?.ToList();
            for (int  i =  0; groupNames != null && i < groupNames.Count; i++)
            {
                var groupName = groupNames[i];
                await Groups.AddToGroupAsync(connectionId, groupName);
                // neu chua ton tai thi add join group
                if (!_groupToUsers.GetGroupConnections(user).Contains(groupName))
                {
                    // tao khi join vao group lan dau
                    await _groupAndUserService.CreateTimeJoin(groupName, user);
                }
                _groupToUsers.Add(user, groupName);
                await Clients.Group(groupName).SendAsync("OnlineToGroup",groupName, user);
            }
            _userMapping.Add(user, connectionId);
            _connectIdToUser.Add(connectionId, user);
            await Clients.Others.SendAsync("Online", user);
            await Clients.Caller.SendAsync("UsersOnline", _userMapping.GetKeys());
        }
        public async Task UpdateInfoUser(string userId,string name,string email)
        {
            var current = new CurrentUser() { ID = userId, Name = name, Email = email };
            _connectIdToCurrentUser.Add(userId, current);
            await Clients.Caller.SendAsync("UpdateInfoUser", current);
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
                    Clients.All.SendAsync("Offline", user);
                }
            }
            return base.OnDisconnectedAsync(exception);
        }
    }

    public class GroupMapping<T>
    {
        private readonly Dictionary<T, HashSet<string>> _connections = new Dictionary<T, HashSet<string>>();

        public List<T> GetKeys()
        {
            try{
                return _connections.Keys?.ToList();
            }
            catch
            {
                return null;
            }
        }
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
                    _connections.Add(connectionId, userId);
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

    public class ConnectIdToCurrentUser
    {
        private readonly Dictionary<string, CurrentUser> _connections = new Dictionary<string, CurrentUser>();

        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public CurrentUser Get(string connectiondId)
        {
            if (_connections.TryGetValue(connectiondId, out CurrentUser user))
            {
                return user;
            }
            return null;
        }

        public void Add(string connectionId, CurrentUser current)
        {
            lock (_connections)
            {
                if (!_connections.TryGetValue(connectionId, out CurrentUser user))
                {
                    _connections.Add(connectionId, user);
                }
                else
                {
                    _connections.Remove(connectionId);
                    _connections.Add(connectionId, current);
                }
            }
        }

        public void Remove(string connectionId)
        {
            lock (_connections)
            {
                if (_connections.TryGetValue(connectionId, out CurrentUser user))
                {
                    _connections.Remove(connectionId);
                }

            }
        }
    }

    public class CurrentUser
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
