
using Newtonsoft.Json;
using System;
using NLog;
using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using BaseMongoDB.Database;

using Business.Dto.Form;
using BaseMVC.Globals;
using System.Net;
using BaseMVC.Models;
using System.Threading.Tasks;

namespace SME.API.Controllers
{

    //[ApiController]
    // [SMEExceptionFilter]
    public class AccountController : ControllerBase
    {



        CPUserSubService _userService;
        AccessTokenService _accessTokenService;
        public AccountController(
        CPUserSubService userService, AccessTokenService accessTokenService
       )
        {
            _userService = userService;
            _accessTokenService = accessTokenService;
        }

        [HttpPost]
        [Obsolete]
        public Task<BaseResponse<CPUserSubEntity>> getSubUser([FromBody]SeachForm seachForm)
        {
           

            return _userService.getListUserSub(seachForm);
        }

        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CPUserSubEntity item)
        {
            //if (_userService.GetItemByEmail(item.Email) != null)
            //{
            //  //  ViewBag.Message = "Email đã tồn tại";
            //   // return View();
            //}
            // ViewBag.Title = "Thêm mới";
            if(_userService.GetItemByUserName(item.UserName)!=null && string.IsNullOrEmpty(item.ID))
                return BadRequest("Tên tài khoản đã có trong hệ thống");

            // return Ok(appUser.Configuration);

            item.Pass = "123";
                item.Pass = Security.Encrypt(item.Pass);
                await _userService.AddAsync(item);

            return NoContent() ;

        }

        [HttpPost]
        public CPUserSubEntity Delete([FromBody]CPUserSubEntity item)
        {
            item.Activity = false;
            _userService.Add(item);

            return new CPUserSubEntity();

        }
    }
}
