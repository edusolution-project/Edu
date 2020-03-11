using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BaseHub
{
    public class MyHub : Hub
    {
        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();
        private readonly static GroupMapping<string> _groups = new GroupMapping<string>();
        public MyHub()
        {

        }

        public Task RemoveMessage(string user, string messageId)
        {
            var listUser = _connections.GetConnections(user);
            if (listUser != null && user.Length > 0)
            {
                IReadOnlyList<string> listUSerReadOnly = listUser?.ToList()?.AsReadOnly();
                Clients.Users(listUSerReadOnly).SendAsync("RemoveMessage", new { Message = messageId, Time = DateTime.Now, Type = UserType });
                Clients.Caller.SendAsync("RemoveMessage", new { Message = messageId, Time = DateTime.Now, Type = UserType });
            }
            return Task.CompletedTask;
        }

        public async Task SendToUser(string user, object message)
        {
            var listUser = _connections.GetConnections(user);
            if(listUser != null && user.Length > 0)
            {
                IReadOnlyList<string> listUSerReadOnly = listUser?.ToList()?.AsReadOnly();
                await Clients.Clients(listUSerReadOnly).SendAsync("ChatToUser", new {UserReciver = user , UserSend = UserName, Message = message, Time = DateTime.Now, Type = UserType, Receiver = user, Sender = UserID });
                await Clients.Caller.SendAsync("ChatToUser", new { UserSend = UserName, Message = message, Time = DateTime.Now, Type = UserType, Receiver = user, Sender = UserID });
            }
            await Task.CompletedTask;
        }

        public Task GoToClass(string className)
        {
            try
            {
                if (!_groups.GetGroupConnections(Context.ConnectionId).Contains(className))
                {
                    _groups.Add(Context.ConnectionId, className);
                    Groups.AddToGroupAsync(Context.ConnectionId, className);
                    string message = UserName + " đã vào lớp có tên là : "+className;
                    return Clients.Group(className).SendAsync("JoinGroup", new { UserSend = UserName, Message = message, Time = DateTime.Now, Type = UserType, Sender = UserID });
                }
                else
                {
                    return Task.CompletedTask;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task SendToGroup(object content, string groupName)
        {
            return Clients.Group(groupName).SendAsync("ReceiveGroup", new { UserSend = UserName, Message = content, Time = DateTime.Now, Type = UserType , Sender = UserID });
        }

        public async Task OutOfTheClassroom(string className)
        {
            _groups.Remove(Context.ConnectionId, className);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, className);
            string message = UserName + " đã ra khỏi lớp";
            await Clients.Group(className).SendAsync("LeaveGroup", new { UserSend = UserName, Message = message, Time = DateTime.Now, Type = UserType });
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            // offline
            RemoveAllGroup();
            _connections.Remove(UserID, Context.ConnectionId);
            Clients.All.SendAsync("Offline", UserID);
            return base.OnDisconnectedAsync(exception);
        }
        public override Task OnConnectedAsync()
        {
            //online
            _connections.Add(UserID, Context.ConnectionId);
            Clients.All.SendAsync("Online", UserID);
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
        protected string UserID
        {
            get
            {
                return Context.User?.FindFirst("UserID")?.Value;
            }
        }
        protected Task RemoveAllGroup()
        {
            var userid = Context.ConnectionId;
            var listGroup = _groups.GetGroupConnections(userid);
            if (listGroup != null || listGroup.Count() > 0)
            {
                foreach (string item in listGroup.ToList())
                {
                    _groups.Remove(userid, item);
                    Groups.RemoveFromGroupAsync(userid, item);
                }
            }
            Clients.Groups(listGroup.ToList()).SendAsync("Offline", UserName + "Offline");
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
