using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Core_v2.Globals;
using Core_v2.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace BaseCustomerMVC.Controllers.Student
{
    public class HomeController : StudentController
    {
        private StudentService _studentService;
        private readonly FileProcess _fileProcess;
        private readonly AccountService _accountService;
        private readonly CenterService _centerService;
        private readonly ISession _session;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DefaultConfigs _default { get; }

        public HomeController(FileProcess fileProcess, AccountService accountService,
            IHttpContextAccessor httpContextAccessor,
            IOptions<DefaultConfigs> defaultvalue,
            CenterService centerService,
            StudentService studentService)
        {
            _studentService = studentService;
            _accountService = accountService;
            _centerService = centerService;
            _fileProcess = fileProcess;
            _httpContextAccessor = httpContextAccessor;
            _session = _httpContextAccessor.HttpContext.Session;
            _default = defaultvalue.Value;
        }

        public IActionResult Index(string basis)
        {
            string _studentid = User.Claims.GetClaimByType("UserID").Value;
            var student = _studentService.GetItemByID(_studentid);
            ViewBag.Student = student;
            if (student != null)
                ViewBag.AllCenters = student.Centers.Where(t => _centerService.GetItemByID(t).ExpireDate >= DateTime.Now).Select(t => _centerService.GetItemByID(t));

            if (!string.IsNullOrEmpty(basis))
            {
                var center = _centerService.GetItemByCode(basis);
                if (center != null)
                    ViewBag.Center = center;
            }
            //var avatar = student != null && !string.IsNullOrEmpty(student.Avatar) ? student.Avatar : _default.defaultAvatar;
            //HttpContext.Session.SetString("userAvatar", avatar);
            return View();
        }

        public IActionResult Profile()
        {
            string _studentid = User.Claims.GetClaimByType("UserID") != null ? User.Claims.GetClaimByType("UserID").Value.ToString() : "0";
            var account = _studentService.GetItemByID(_studentid);
            ViewBag.avatar = account.Avatar ?? _default.defaultAvatar;
            _session.SetString("userAvatar", account.Avatar ?? _default.defaultAvatar);
            return View(account);
        }

        public JsonResult GetProfile()
        {
            try
            {
                string _studentid = User.Claims.GetClaimByType("UserID") != null ? User.Claims.GetClaimByType("UserID").Value.ToString() : "0";
                var account = _studentService.GetItemByID(_studentid);
                var avatar = account.Avatar ?? _default.defaultAvatar;
                _session.SetString("userAvatar", avatar);
                ViewBag.avatar = avatar;
                account.Avatar = avatar;
                return Json(new ReturnJsonModel
                {
                    StatusCode = ReturnStatus.SUCCESS,
                    StatusDesc = "OK",
                    Data = account
                });
            }
            catch (Exception e)
            {
                return Json(new ReturnJsonModel
                {
                    StatusCode = ReturnStatus.ERROR,
                    StatusDesc = e.Message,
                });
            }
        }

        [HttpPost]
        public IActionResult Profile(StudentEntity entity)
        {
            string _studentid = User.Claims.GetClaimByType("UserID").Value;
            var acc = _studentService.GetItemByID(_studentid);
            acc.FullName = entity.FullName;
            acc.Phone = entity.Phone;
            acc.Skype = entity.Skype;
            _studentService.CreateOrUpdate(acc);
            ViewBag.avatar = acc.Avatar ?? _default.defaultAvatar;
            ViewBag.Description = "Lưu thành công!";
            return View(acc);
        }

        public JsonResult SaveProfile(StudentEntity entity, IFormFile fileUpload)
        {
            try
            {
                string _studentid = User.Claims.GetClaimByType("UserID") != null ? User.Claims.GetClaimByType("UserID").Value.ToString() : "0";
                var account = _studentService.GetItemByID(_studentid);
                account.FullName = entity.FullName;
                account.Phone = entity.Phone;
                account.Skype = entity.Skype;
                if (fileUpload != null)
                {
                    var pathImage = _fileProcess.SaveMediaAsync(fileUpload, fileUpload.FileName).Result;
                    account.Avatar = pathImage;
                    _session.SetString("userAvatar", account.Avatar);
                }

                _studentService.CreateOrUpdate(account);
                return Json(new ReturnJsonModel
                {
                    StatusCode = ReturnStatus.SUCCESS,
                    StatusDesc = "SaveOk",
                    Data = account
                });
            }
            catch (Exception e)
            {
                return Json(new ReturnJsonModel
                {
                    StatusCode = ReturnStatus.ERROR,
                    StatusDesc = e.Message,
                });
            }
        }


        [HttpPost]
        public JsonResult UploadPhoto(IFormFile fileUpload)
        {
            var pathImage = _fileProcess.SaveMediaAsync(fileUpload, fileUpload.FileName).Result;
            // Cap nhat vao truong avartar
            string _studentId = User.Claims.GetClaimByType("UserID").Value;
            StudentEntity oldAcc = _studentService.GetItemByID(_studentId);
            oldAcc.Avatar = pathImage;
            _studentService.CreateOrUpdate(oldAcc);
            _session.SetString("userAvatar", oldAcc.Avatar);
            //return Content(pathImage);

            return Json(new ReturnJsonModel
            {
                StatusCode = ReturnStatus.SUCCESS,
                StatusDesc = pathImage,
            });
        }

        [HttpPost]
        public JsonResult ChangePassword(string oldpass, string newpass, string retypepass)
        {
            /*
             * 1. Kiem tra mat khau cu co khop hay khong
             * 2. kiem tra mat khau moi va xac nhan mat khau co khop nhau hay khong
             * 2.1 Kiem tra mat khau phai co it nhat 6 ki tu
             * 3. Luu mat khat moi vao CSDL
             * 4. Thong bao thanh cong cho don vi
             */

            if (string.IsNullOrEmpty(oldpass))
            {
                return new JsonResult(
                new Dictionary<string, object>
                    {
                        { "Error", "Password is not correct" }
                    });
            }

            if (string.IsNullOrEmpty(newpass) || newpass.Length < 6)
            {
                return new JsonResult(
                new Dictionary<string, object>
                    {
                        { "Error", "Password's length must be over 6 " }
                    });
            }

            if (!newpass.Equals(retypepass))
            {
                return new JsonResult(
                new Dictionary<string, object>
                    {
                        { "Error", "Retype password is not matched" }
                    });
            }

            string _studentid = User.Claims.GetClaimByType("UserID") != null ? User.Claims.GetClaimByType("UserID").Value.ToString() : "0";
            var acc = _studentService.GetItemByID(_studentid);

            AccountEntity user = _accountService.GetAccountByEmail(acc.Email);

            if (!Core_v2.Globals.Security.Encrypt(oldpass).Equals(user.PassWord))
            {
                return new JsonResult(
                new Dictionary<string, object>
                 {
                        { "Error", "Old password not correct" }
                 });
            }


            if (Core_v2.Globals.Security.Encrypt(newpass).Equals(user.PassWord))
            {
                return new JsonResult(
                new Dictionary<string, object>
                 {
                        { "Error", "New password can not be the same as old password" }
                 });
            }

            user.PassWord = Core_v2.Globals.Security.Encrypt(newpass);
            _accountService.CreateOrUpdate(user);
            return new JsonResult(
            new Dictionary<string, object>
                    {
                        { "Message", "Password Updated Successfully" }
                    });
        }
    }
}
