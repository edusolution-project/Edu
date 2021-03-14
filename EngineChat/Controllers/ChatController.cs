using BaseEngineEntity;
using Core_v2.Interfaces;
using FileManagerCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineChat.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        const string __SECRUITY = "security";
        private readonly ILog _log;
        private readonly ChatService _chatService;
        private readonly MessageService _messageService;
        private readonly IRoxyFilemanHandler _roxyFilemanHandler;
        public ChatController(ChatService chatService, MessageService messageService, IRoxyFilemanHandler roxyFilemanHandler, ILog log)
        {
            _chatService = chatService;
            _messageService = messageService;
            _roxyFilemanHandler = roxyFilemanHandler;
            _log = log;
        }
        [HttpDelete]
        public async Task<JsonResult> RemoveMessage([FromBody] string messageId)
        {
            JsonResult result;
            try
            {
                string security = HttpContext.Request.Headers[__SECRUITY];
                if (string.IsNullOrEmpty(security))
                {
                    result = CreateResponeNoAccount();
                }
                else
                {
                    bool isDel = await _messageService.Remove(messageId, security);
                    result = CreateResponeOK(isDel);
                }
            }
            catch (Exception ex)
            {
                await _log.Error("Exception", ex);
                result = CreateResponeError(ex.Message);
            }
            return result;
        }
        [HttpPost]
        public async Task<JsonResult> SendMessage([FromForm] MessageEntity data)
        {
            JsonResult result;
            try
            {
                string security = HttpContext.Request.Headers[__SECRUITY];
                if (string.IsNullOrEmpty(security))
                {
                    result = CreateResponeNoAccount();
                }
                else
                {
                    var files = HttpContext.Request.Form?.Files;
                    List<Attachment> attachs = new List<Attachment>();
                    if (files != null && files.Count > 0)
                    {
                        var listData = _roxyFilemanHandler.UploadFileWithGoogleDrive("ChatEngine", security, HttpContext);
                        if(listData != null && listData.Count > 0)
                        {
                            attachs = listData.Select(o => new Attachment() { Name = o.Name, Path = o.Path, UserID = security, Exts = o.Extends })?.ToList();
                        }
                    }
                    var msg = await _messageService.Create(new MessageEntity() {
                        Receiver = data.Receiver,
                        Sender = data.Sender,
                        Text = data.Text,
                        Attachments = attachs,
                        Type = data.Type
                    });

                   await _chatService.UpdateAttachMements(security, data.Receiver, attachs);

                    result = CreateResponeOK(msg);
                }
            }
            catch (Exception ex)
            {
                await _log.Error("Exception", ex);
                result = CreateResponeError(ex.Message);
            }
            return result;
        }
        [HttpPost]
        public async Task<JsonResult> GetContact([FromForm]List<ChatEntity> chat)
        {
            JsonResult result;
            try
            {
                string security = HttpContext.Request.Headers[__SECRUITY];
                if (string.IsNullOrEmpty(security))
                {
                    result = CreateResponeNoAccount();
                }
                else {
                    List<ChatEntity> data = await _chatService.GetList(security, chat);
                    result = CreateResponeOK(data);
                }
            }
            catch (Exception ex)
            {
                await _log.Error("Exception", ex);
                result = CreateResponeError(ex.Message);
            }
            return result;
        }

        public async Task<JsonResult> GetContactDetail([FromQuery] string contactId)
        {
            JsonResult result;
            try
            {
                string security = HttpContext.Request.Headers[__SECRUITY];
                if (string.IsNullOrEmpty(security))
                {
                    result = CreateResponeNoAccount();
                }
                else
                {
                    ChatDetailEntity data = await _chatService.GetDetail(contactId);
                    result = CreateResponeOK(data);
                }
            }
            catch (Exception ex)
            {
                await _log.Error("Exception", ex);
                result = CreateResponeError(ex.Message);
            }
            return result;
        }


        private JsonResult CreateRespone(int code, string message, object data)
        {
            return new JsonResult(new Response() { Code = code, Message = message, Data = data });
        }
        private JsonResult CreateResponeOK(object data)
        {
            return CreateRespone(200, "SUCCESS", data);
        }
        private JsonResult CreateResponeError(string message)
        {
            return CreateRespone(500, message, null);
        }
        private JsonResult CreateResponeNoAccount()
        {
            return CreateRespone(101, "access denied", null);
        }
    }
    public class Response
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
