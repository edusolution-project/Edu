using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Core_v2.Globals;
using Core_v2.Interfaces;
using EasyChatApp.DataBase;
using FileManagerCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace EasyChatApp.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ChatController : Controller
    {
        private readonly IHubContext<EasyChatHub> _hubContext;
        private readonly ILog _log;
        private readonly IRoxyFilemanHandler _roxyFilemanHandler;
        private readonly GroupUserService _groupUserService;
        private readonly MessagerService _messagerService;
        private readonly IConfiguration _configuration;
        public ChatController(ILog log, IRoxyFilemanHandler roxyFilemanHandler, IHubContext<EasyChatHub> hubContext , IConfiguration configuration
            , GroupUserService  groupUserService
            , MessagerService messagerService)
        {
            this._log = log;
            _roxyFilemanHandler = roxyFilemanHandler;
            _hubContext = hubContext;
            _groupUserService = groupUserService;
            _messagerService = messagerService;
            _configuration = configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="center"></param>
        /// <param name="user"></param>
        /// <param name="groupId"></param>
        /// <param name="receiver"></param>
        /// <param name="message"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Response> SendMessage([FromQuery]ChatModel model)
        {
            Response response = new Response();
            //await Task.Delay(100);
            try
            {
                MessagerEntity item = string.IsNullOrEmpty(model.messageId) ? new MessagerEntity {Sender = model.user } : _messagerService.GetItemByID(model.messageId);
                #region group or user
                if (!string.IsNullOrEmpty(model.groupId))
                {
                    item.GroupId = model.groupId;
                }
                if (!string.IsNullOrEmpty(model.receiver))
                {
                    var group = _groupUserService.GetGroupPrivate(model.user, model.receiver);
                    item.GroupId = group.ID;
                }
                #endregion
                var medias = _roxyFilemanHandler.UploadFileWithGoogleDrive(model.center, model.user, HttpContext);
                if (medias == null || medias.Count == 0)
                {
                    item.Content = model.message;
                }
                else
                {
                    List<MetaData> metaDatas = new List<MetaData>();
                    for (int i = 0; i < medias.Count; i++)
                    {
                        var media = medias[i];
                        metaDatas.Add(new MetaData()
                        {
                            Id = media.FileId,
                            Type = media.Extends,
                            Url = Program.GoogleDriveApiService.CreateLinkViewFile(media.FileId)
                        });
                    }
                    item.Data = metaDatas;
                }
                _messagerService.CreateOrUpdate(item);
                if (!string.IsNullOrEmpty(model.groupId))
                {
                    await _hubContext.Clients.Group(model.groupId).SendAsync("ReceiverMessage", item,null,model.groupId);
                }
                else
                {
                    var usersConnections = EasyChatHub.UserMap;
                    var connestionIds = usersConnections.GetGroupConnections(model.receiver);
                    if(connestionIds != null && connestionIds.Count() > 0)
                    {
                        await _hubContext.Clients.Clients(connestionIds.ToList()).SendAsync("ReceiverMessage", item,model.user,null);
                    }
                }
                response.Code = 200;
                response.Data = item;
                response.Message = "SUCCESS";

            }
            catch(Exception ex)
            {
                response.Code = 500;
                response.Message = ex.Message;
                response.Data = null;
            }
            return response;
        } 
        [HttpPost]
        public async Task<Response> RemoveMessage(string user, string messageId, string connectionId)
        {
            Response response = new Response();
            try
            {
                var message = _messagerService.GetItemByID(messageId);
                if(message != null)
                {
                    if(message.Sender == user)
                    {
                        message.IsDel = true;
                        _messagerService.CreateOrUpdate(message);
                        response.Code = 200;
                        response.Message = "SUCCESS";
                        response.Data = message;
                        if (!string.IsNullOrEmpty(connectionId))
                        {
                            await _hubContext.Clients.Client(connectionId).SendAsync("RemoveMessage", message.ID);
                        }
                        else
                        {
                            await _hubContext.Clients.Group(message.GroupId).SendAsync("RemoveMessage", message.ID);
                        }
                        return response;
                    }
                    else
                    {
                        response.Code = 405;
                        response.Message = "bạn không đủ quyền";
                        return response;
                    }
                }

                response.Code = 404;
                response.Message = "Data not found";
            }
            catch (Exception ex)
            {
                StackTrace stackTrace = new StackTrace();
                MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();
                await _log.Error(methodBase.Name, ex);
                response.Code = 500;
                response.Message = ex.Message;
                response.Data = null;
            }
            return response;
        }

        [HttpPost]
        [Obsolete]
        public async Task<Response> GetMessages(string user, string receiver, string groupId, string messageId, double startDate, int pageIndex, int pageSize)
        {
            Response response = new Response();
            try
            {
                string groupName = groupId;
                if (!string.IsNullOrEmpty(receiver))
                {
                    groupName = _groupUserService.GetGroupPrivate(user, receiver).ID;
                }
                if (string.IsNullOrEmpty(messageId))
                {
                    response.Code = 200;
                    response.Message = "SUCCESS";
                    //thoi diem hien tai ve sau
                    var listData = _messagerService.CreateQuery().Find(o => o.GroupId == groupName)?.SortByDescending(o=>o.Time)?.Skip(pageSize*pageIndex)?
                        .Limit(pageSize)?.ToList()?.OrderBy(o=>o.Time)?.ToList();

                    response.Data = new { Data = listData, PageIndex = (listData == null || listData.Count < 0) ? pageIndex : pageIndex + 1 };
                }
                else
                {
                    var data = _messagerService.GetItemByID(messageId);
                    response.Code = 200;
                    response.Data = data;
                    response.Message = "SUCCESS";
                }

                return response;

            }
            catch (Exception ex)
            {
                StackTrace stackTrace = new StackTrace();
                MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();
                await _log.Error(methodBase.Name, ex);
                response.Code = 500;
                response.Message = ex.Message;
                response.Data = null;
            }
            return response;
        }

        public GroupMapping<string> GroupMapping()
        {
            return EasyChatHub.GroupMapping;
        }
        public string ScriptEasyChat()
        {
            string host = _configuration.GetSection("EasyChat:Host").Value;
            string urlSend = host + "/Chat/SendMessage";
            string urlRemove = host + "/Chat/RemoveMessage?user={user}&messageId={messageId}&connectionId={connectionId}";
            string urlGet = host + "/Chat/GetMessages?user={user}&receiver={receiver}&groupId={groupId}&messageId={messageId}&startDate={startDate}&pageIndex={pageIndex}&pageSize={pageSize}";
            string value = "var g_EasyChatURL={'SendMessage':'"+urlSend+ "','RemoveMessage':'" + urlRemove + "','GetMessage':'" + urlGet + "'}";
            return value;
        }
    }
    public class ChatModel
    {
        public string messageId { get; set; }
        public string center { get; set; }
        public string user { get; set; }
        public string groupId { get; set; }
        public string receiver { get; set; }
        public string message { get; set; }
    }
    public class Response
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}