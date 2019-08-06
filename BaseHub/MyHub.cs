using BaseHub.Database;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BaseHub
{
    public class MyHub : Hub
    {
        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();
        private readonly static GroupMapping<string> _groups = new GroupMapping<string>();
        private readonly ChatService _chatService;
        public MyHub(ChatService chatService)
        {
            _chatService = chatService;
        }
        public Task GoToClass(string className)
        {
            _groups.Add(Context.ConnectionId, className);
            Groups.AddToGroupAsync(className, Context.ConnectionId);
            string message = UserName + " đã vào lớp";
            return Clients.Group(className).SendAsync("JoinGroup", new { UserSend = UserName, Message = message, Time = DateTime.Now, Type = UserType });
        }
        public Task OutOfTheClassroom(string className)
        {
            _groups.Remove(Context.ConnectionId, className);
            Groups.RemoveFromGroupAsync(className, Context.ConnectionId);
            string message = UserName + " đã ra khỏi lớp";
            return Clients.Group(className).SendAsync("LeaveGroup", new { UserSend = UserName, Message = message, Time = DateTime.Now, Type = UserType });
        }
        public Task SendToGroup(string groupName, string message)
        {
            return Clients.Group(groupName).SendAsync("ReceiveGroup", new { UserSend = UserName, Message = message, Time = DateTime.Now, Type = UserType });
        }
        public Task SendToUser(string userId, string message)
        {
            return Clients.User(userId).SendAsync("ReceiveUser", new { UserSend = UserName,Message = message,Time = DateTime.Now, Type = UserType });
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            // offline
            RemoveAllGroup();
            _connections.Remove(KeyUser, Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
        public override Task OnConnectedAsync()
        {
            //online
            _connections.Add(KeyUser, Context.ConnectionId);
            return base.OnConnectedAsync();
        }
        protected string KeyUser
        {
            get
            {
                return Context.User?.FindFirst(ClaimTypes.Email)?.Value;
            }
        }
        protected string UserType
        {
            get
            {
                return Context.User?.FindFirst("Type")?.Value;
            }
        }
        protected string UserName
        {
            get
            {
                return Context.User.Identity.Name;
            }
        }
        protected Task RemoveAllGroup()
        {
            var userid = Context.ConnectionId;
            var listGroup = _groups.GetGroupConnections(userid);
            if (listGroup != null && listGroup.Count() > 0)
            {
                foreach(string item in listGroup)
                {
                    _groups.Remove(userid, item);
                    Groups.RemoveFromGroupAsync(userid, item);
                }
            }
            return Task.CompletedTask;
        }
    }
    public class ConnectionMapping<T>
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

        public IEnumerable<string> GetConnections(T key)
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
}
