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
        private readonly NewFeedService _newFeedService;
        private readonly CommentService _commentService;
        private readonly ChatService _chatService;
        private readonly GroupService _groupService;
        private readonly ChatPrivateService _chatPrivateService;
        public MyHub(ChatPrivateService chatPrivateService,NewFeedService newFeedService, CommentService commentService, ChatService chatService, GroupService groupService)
        {
            _newFeedService = newFeedService;
            _commentService = commentService;
            _chatService = chatService;
            _groupService = groupService;
            _chatPrivateService = chatPrivateService;
        }
        public  Task GoToClass(string className)
        {
            try
            {
                if (!_groups.GetGroupConnections(Context.ConnectionId).Contains(className))
                {
                     _groupService.AddMember(className, UserID);
                    _groups.Add(Context.ConnectionId, className);
                     Groups.AddToGroupAsync(Context.ConnectionId, className);
                    string message = UserName + " đã vào lớp";
                    return Clients.Group(className).SendAsync("JoinGroup", new { UserSend = UserName, Message = message, Time = DateTime.Now, Type = UserType });
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
        public async Task OutOfTheClassroom(string className)
        {
             _groups.Remove(Context.ConnectionId, className);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, className);
            string message = UserName + " đã ra khỏi lớp";
            await Clients.Group(className).SendAsync("LeaveGroup", new { UserSend = UserName, Message = message, Time = DateTime.Now, Type = UserType });
        }
        /// <summary>
        /// Chat trong group
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private Task ChatToGroup(string groupName, ChatEntity item)
        {
            var group = _groupService.GetItemByName(groupName);
            if (group == null)
                return Task.CompletedTask;
            item.GroupID = group.ID;
            item.Sender = UserID;
            item.Created = DateTime.Now;
            _chatService.CreateOrUpdate(item);
            return Clients.Group(groupName).SendAsync("ChatGroup", new { UserSend = UserName, Message = item, Time = DateTime.Now, Type = UserType });
        }
        /// <summary>
        /// Chat trong new feed
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task CommentToNewFeed(string groupName, CommentEntity message)
        {
            var group = _groupService.GetItemByName(groupName);
            if (group == null)
                return Task.CompletedTask;
            message.Poster = UserID;
            message.PosterName = UserName;
            message.TimePost = DateTime.Now;
            _commentService.CreateOrUpdate(message);
            return Clients.Group(groupName).SendAsync("CommentNewFeed", new { UserSend = UserName, Message = message, Time = DateTime.Now, Type = UserType });
        }
        /// <summary>
        /// chat riêng
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task ChatToUser(string userId, ChatPrivateEntity message)
        {
            message.Created = DateTime.Now;
            message.Sender = UserID;
            message.Receiver = userId;
            _chatPrivateService.CreateOrUpdate(message);
            var receiver = _connections.GetConnections(userId);
            return Clients.Users(receiver.ToList()).SendAsync("Receive", new { UserSend = UserName,Message = message,Time = DateTime.Now, Type = UserType });
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            // offline
            RemoveAllGroup();
            _connections.Remove(UserID, Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
        public override Task OnConnectedAsync()
        {
            //online
            _connections.Add(UserID, Context.ConnectionId);
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
                foreach(string item in listGroup.ToList())
                {
                    _groups.Remove(userid, item);
                    Groups.RemoveFromGroupAsync(userid, item);
                }
            }
            Clients.Groups(listGroup.ToList()).SendAsync("Offline",UserName + "Offline");
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
