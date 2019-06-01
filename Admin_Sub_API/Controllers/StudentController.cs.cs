
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
    public class StudentController : ControllerBase
    {



        CPUserSubService _userService;
        AccessTokenService _accessTokenService;
        StudentService studentSerVice;

        public StudentController(
        CPUserSubService userService, 
        AccessTokenService accessTokenService,
        StudentService studentSerVice
       )
        {
            _userService = userService;
            _accessTokenService = accessTokenService;
            this.studentSerVice = studentSerVice;
        }

        [HttpPost]
        public Task<BaseResponse<CPUserSubEntity>> getSubUser([FromBody]SeachForm seachForm)
        {
           

            return _userService.getListUserSub(seachForm);
        }

        [HttpPost]
        public List<StudentEntity> getAll([FromBody]SeachForm seachForm)
        {


            return studentSerVice.getListALL();
        }

        [HttpPost]
        public Task<BaseResponse<StudentEntity>> getList([FromBody]SeachForm seachForm)
        {
            var userItem = _userService.GetItemByUserName(seachForm.UserName);
            seachForm.UserName = userItem.UserNameManager;

            return studentSerVice.getList(seachForm);
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
                _userService.Add(item);

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
