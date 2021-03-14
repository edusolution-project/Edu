using Core_v2.Globals;
using Core_v2.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineChat
{
    public class ChatHub : Hub
    {
        private readonly ILog _log;
        public ChatHub(ILog log)
        {
            _log = log;
        }

        public Task SendMessageToUser(string senderId, string reveiverId, string context)
        {
            List<string> connectionIds = CacheExtends.GetDataFromCache<List<string>>(reveiverId);
            int count = connectionIds != null ? connectionIds.Count : 0;
            if(count == 0)
            {
              return  Clients.Caller.SendAsync("ReveiverUser", "OFFLINE", reveiverId);
            }
            else
            {
                Clients.Clients(connectionIds).SendAsync("ReceiveUser", senderId, reveiverId, context);
            }
            return Clients.Caller.SendAsync("done", true);
        }
        public Task SendMessageToGroup(string groupName, string context)
        {
             Clients.Group(groupName).SendAsync("ReceiveGroup", context);
            return Clients.Caller.SendAsync("done", true);
        }

        public Task Online(string userId)
        {
            string connectionId = Context.ConnectionId;
            CacheExtends.SetObjectFromCache(connectionId,24*60*365, userId);
            SetCache(userId, connectionId);
            return Clients.All.SendAsync("Status", userId ,true);
        }

        private void SetCache(string user, string value)
        {
            try
            {
                int expires = 24 * 60 * 365;
                List<string> data = CacheExtends.GetDataFromCache<List<string>>(user);
                if (data == null){data = new List<string>(){value};}
                else{if (!data.Contains(value)) data.Add(value);}
                CacheExtends.SetObjectFromCache(user, expires, data);
            }
            catch(Exception ex)
            {
                _log.Error("SetCache", ex);
            }
        }
        private void RemoveCache(string connectionId)
        {
            try
            {
                int expires = 24 * 60 * 365;
                string user = CacheExtends.GetDataFromCache<string>(connectionId);
                if (!string.IsNullOrEmpty(user))
                {
                    CacheExtends.ClearCache(connectionId);
                    List<string> connectionIds = CacheExtends.GetDataFromCache<List<string>>(user);
                    connectionIds.Remove(user);
                    if (connectionIds.Count > 0)
                    {
                        CacheExtends.SetObjectFromCache(user, expires, connectionIds);
                    }
                    else
                    {
                        // offline
                        CacheExtends.ClearCache(user);
                        Clients.Others.SendAsync("Status", user, false);
                    }
                }
            }
            catch(Exception ex)
            {
                _log.Error("RemoveCache", ex);
            }
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            RemoveCache(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
