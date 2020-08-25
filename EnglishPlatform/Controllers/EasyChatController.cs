using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseCustomerEntity.Database;
using BaseCustomerMVC.Controllers.Student;
using BaseCustomerMVC.Globals;
using BaseEasyRealTime.Entities;
using FileManagerCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

namespace EnglishPlatform.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EasyChatController : ControllerBase
    {
        protected readonly Dictionary<string, List<string>> _mapUserOffline = new Dictionary<string, List<string>>();
        protected readonly Dictionary<string, string> _mapConnectId = new Dictionary<string,string>();
        protected readonly Dictionary<string, List<string>> _mapUsersConnectionId = new Dictionary<string, List<string>>();
        private readonly IRoxyFilemanHandler _roxyFilemanHandler;
        private readonly IHubContext<ChatHub> _hubContent;

        public EasyChatController(IHubContext<ChatHub> hubContent,IRoxyFilemanHandler roxyFilemanHandler)
        {
            
            _hubContent = hubContent;
            _roxyFilemanHandler = roxyFilemanHandler;
        }

        // danh sách group 
        public List<object> GetClassList()
        {
            if(User != null && User.Identity.IsAuthenticated)
            {

            }

            return null;
        }

        // add key connectid và key userId
        private bool AddMapUserWithConnectId(string userId,string connectId)
        {
            lock(_mapUsersConnectionId)
            {
                if (!_mapUsersConnectionId.TryGetValue(userId, out List<string> connectionIds))
                {
                    connectionIds = new List<string>() { };
                    _mapUsersConnectionId.Add(userId, connectionIds);
                }
                lock(connectionIds)
                {
                    connectionIds.Add(connectId);
                }
            }
            return false;
        }
        private bool AddMapConnectIdWithUser(string userId,string connectId)
        {
            lock (_mapConnectId)
            {
                if (!_mapConnectId.TryGetValue(connectId, out string connection))
                {
                    return _mapConnectId.TryAdd(connectId, userId);
                }
            }
            return false;
        }

        private void RemoveMapConnectionId(string connectionId)
        {
            lock(_mapConnectId)
            {
                if(_mapConnectId.TryGetValue(connectionId,out string userId))
                {
                    _mapConnectId.Remove(connectionId);
                }
            }
        }
        private void RemoveMapConnectionIdWithUser(string userId, string connectionId)
        {
            lock (_mapUsersConnectionId)
            {
                if (_mapUsersConnectionId.TryGetValue(userId, out List<string> connections))
                {
                    if(connections == null)
                    {
                        _mapUsersConnectionId.Remove(userId);
                    }
                    else
                    {
                        if (connections.Count > 0)
                        {
                            lock(connections)
                            {
                                connections.Remove(connectionId);
                            }
                            if (connections == null)
                            {
                                _mapUsersConnectionId.Remove(userId);
                            }
                        }
                    }
                }
            }
        }

        private void FillUserToGroup(List<string> groupsName, string connectId)
        {

        }
    }
}