using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntityBaseFW
{
    public class SignalR : Hub
    {
        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();
        #region 
        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }
        #endregion
        public ConnectionMapping<string> SendAllListOnline()
        {
            return _connections;
        }
        public Task SendMessageForAll(string message)
        {
            return Clients.All.ReciverMessageAll(message);
        }
        //booking (student - teacher - admin)
        public Task SendMessageForOther(string message)
        {
            return Clients.Others.ReciverMessageOther(message);
        }
        //chat (student + teacher)
        public Task SendMessageForUser(string user, string message)
        {
            return Clients.User(user).ReciverMessageForUser(message);
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        
    }
    public class Response
    {
        public string Status { get; set; }
        public MessageInfo Data { get; set; }
        public string Message { get; set; }
    }
    public class MessageInfo
    {
        public string Name { get; set; }
        public string Message { get; set; }
    }
    public class ConnectionMapping<T>
    {
        public int Count
        {
            get
            {
                return ListOnline.Count;
            }
        }
        public Dictionary<T, HashSet<string>> ListOnline { get; } = new Dictionary<T, HashSet<string>>();
        public void Add(T key, string connectionId)
        {
            lock (ListOnline)
            {
                if (!ListOnline.TryGetValue(key, out HashSet<string> connections))
                {
                    connections = new HashSet<string>();
                    ListOnline.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }

        public IEnumerable<string> GetConnections(T key)
        {
            if (ListOnline.TryGetValue(key, out HashSet<string> connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }

        public void Remove(T key, string connectionId)
        {
            lock (ListOnline)
            {
                if (!ListOnline.TryGetValue(key, out HashSet<string> connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        ListOnline.Remove(key);
                    }
                }
            }
        }
        public void Remove(T key)
        {
            lock (ListOnline)
            {
                ListOnline.Remove(key);
            }
        }
    }

}
