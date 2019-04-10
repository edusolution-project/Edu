using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceBaseNet.Services;
using EntityBaseNet;
using GlobalNet.Utils;
using Controller = ServiceBaseNet.ControllerBase;

namespace AdminPage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CAjaxController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        private readonly SysAccountServices _sysAccount;
        public CAjaxController()
        {
            _sysAccount = new SysAccountServices();
        }
        [Route("/CreateUser")]
        public async Task<bool> StartWeb()
        {
            SysAccount account = new SysAccount()
            {
                Name = "Hoàng Thái Long",
                Avatar = "NoImage",
                Email = "longthaihoang94@gmail.com",
                RoleID = 1,
                Pass = Security.Encrypt("123"),
                UserCreated = 1,
                Activity = true,
            };
            await _sysAccount.InsertItemAsync(account);
            return true;
        }
    }
}