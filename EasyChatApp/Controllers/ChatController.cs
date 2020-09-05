using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Core_v2.Interfaces;
using FileManagerCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace EasyChatApp.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<EasyChatHub> _hubContext;
        private readonly ILog _log;
        private readonly IRoxyFilemanHandler _roxyFilemanHandler;
        public ChatController(ILog log, IRoxyFilemanHandler roxyFilemanHandler, IHubContext<EasyChatHub> hubContext)
        {
            this._log = log;
            _roxyFilemanHandler = roxyFilemanHandler;
            _hubContext = hubContext;
        }
        [HttpPost]
        public async Task<Response> SendMessage(string center, string user, string receiver, string message)
        {
            Response response = new Response();
            await Task.Delay(100);
            try
            {
                var medias = _roxyFilemanHandler.UploadFileWithGoogleDrive(center, user, HttpContext);


            }
            catch(Exception ex)
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

        public string GetUser()
        {
            return User.Identity.IsAuthenticated ? User.FindFirst("UserID").Value : "no login";
        }
    }

    public class Response
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}