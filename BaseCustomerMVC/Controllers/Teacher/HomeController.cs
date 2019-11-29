using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Core_v2.Globals;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace BaseCustomerMVC.Controllers.Teacher
{
    [IndefindCtrlAttribulte("Trang chủ", "Home", "teacher")]
    public class HomeController : TeacherController
    {
        private readonly FileProcess _fileProcess;
        private readonly TeacherService _teacherService;
        private readonly AccountService _accountService;
        private readonly ISession _session;

        public DefaultConfigs _default { get; }

        public HomeController(FileProcess fileProcess, TeacherService teacherService, AccountService accountService,
            IHttpContextAccessor httpContextAccessor,
            IOptions<DefaultConfigs> defaultvalue)
        {
            _teacherService = teacherService;
            _accountService = accountService;
            _fileProcess = fileProcess;
            _session = httpContextAccessor.HttpContext.Session;
            _default = defaultvalue.Value;
        }

        public IActionResult Index()
        {
            ViewBag.RoleCode = User.Claims.GetClaimByType(ClaimTypes.Role).Value;
            string _teacherid = User.Claims.GetClaimByType("UserID").Value;
            var teacher = _teacherService.GetItemByID(_teacherid);
            _session.SetString("userAvatar", teacher.Avatar ?? _default.defaultAvatar);
            return View();
        }

        public IActionResult Profile()
        {
            string _teacherid = User.Claims.GetClaimByType("UserID") != null ? User.Claims.GetClaimByType("UserID").Value.ToString() : "0";
            var account = _teacherService.GetItemByID(_teacherid);
            ViewBag.avatar = account.Avatar ?? "/img/defaultAvatar.png";
            _session.SetString("userAvatar", account.Avatar ?? _default.defaultAvatar);
            return View(account);
        }

        [HttpPost]
        public IActionResult Profile(TeacherEntity entity)
        {
            string _teacherid = User.Claims.GetClaimByType("UserID").Value;
            var acc = _teacherService.GetItemByID(_teacherid);
            acc.FullName = entity.FullName;
            acc.Phone = entity.Phone;
            _teacherService.CreateOrUpdate(acc);
            ViewBag.avatar = acc.Avatar ?? _default.defaultAvatar;
            ViewBag.Description = "Lưu thành công!";
            return View(acc);
        }

        [HttpPost]
        public JsonResult UploadPhoto(IFormFile fileUpload)
        {
            var pathImage = _fileProcess.SaveMediaAsync(fileUpload, fileUpload.FileName).Result;
            // Cap nhat vao truong avartar
            string _teacherid = User.Claims.GetClaimByType("UserID").Value;
            TeacherEntity teOld = _teacherService.GetItemByID(_teacherid);
            teOld.Avatar = pathImage;
            _teacherService.CreateOrUpdate(teOld);
            _session.SetString("userAvatar", teOld.Avatar);
            //return Content(pathImage);

            return Json(new ReturnJsonModel
            {
                StatusCode = ReturnStatus.SUCCESS,
                StatusDesc = pathImage,
            });
        }

        [HttpPost]
        public JsonResult GetListTeachers(string SubjectID)
        {
            var filter = new List<FilterDefinition<TeacherEntity>>();
            if (!string.IsNullOrEmpty(SubjectID))
            {
                filter.Add(Builders<TeacherEntity>.Filter.Where(o => o.Subjects.Contains(SubjectID)));
            }

            var data = filter.Count > 0 ? _teacherService.CreateQuery().Find(Builders<TeacherEntity>.Filter.And(filter)) : _teacherService.GetAll();

            var response = new Dictionary<string, object>
            {
                { "Data", data.ToList()}
            };
            return new JsonResult(response);
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

            string _teacherid = User.Claims.GetClaimByType("UserID") != null ? User.Claims.GetClaimByType("UserID").Value.ToString() : "0";
            var te = _teacherService.GetItemByID(_teacherid);

            AccountEntity user = _accountService.GetAccountByEmail(te.Email);

            if (Security.Encrypt(newpass).Equals(user.PassWord))
            {
                return new JsonResult(
                new Dictionary<string, object>
                 {
                        { "Error", "New password can not be the same as old password" }
                 });
            }

            user.PassWord = Security.Encrypt(newpass);
            _accountService.CreateOrUpdate(user);
            return new JsonResult(
            new Dictionary<string, object>
                    {
                        { "Message", "Password Updated Successfully" }
                    });
        }
    }


}
