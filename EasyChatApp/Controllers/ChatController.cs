using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EasyChatApp.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        public ChatController()
        {

        }

        public string GetUser()
        {
            return User.Identity.IsAuthenticated ? User.FindFirst("UserID").Value : "no login";
        }
    }
}