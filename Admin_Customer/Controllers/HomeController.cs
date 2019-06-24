using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BaseCustomerEntity.Database;
using Core_v2.Globals;
using Core_v2.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Admin_Customer.Controllers
{
    public class HomeController : Controller
    {
        private readonly AccountService _accountService;
        private readonly RoleService _roleService;
        private readonly AccountLogService _logService;
        private readonly TeacherService _teacherService;
        private readonly StudentService _studentService;
        private readonly ILog _log;
        public HomeController(AccountService accountService , RoleService roleService, AccountLogService logService
            , TeacherService teacherService
            , StudentService studentService
            ,ILog log)
        {
            _accountService = accountService;
            _roleService = roleService;
            _logService = logService;
            _teacherService = teacherService;
            _studentService = studentService;
            _log = log;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Route("/login")]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [Route("/login")]
        public IActionResult Login(string UserName,string PassWord,string Type, bool IsRemmember)
        {
            _log.Debug("login", new { UserName, PassWord, Type, IsRemmember });
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(PassWord))
            {
                ViewBag.MessageError = "Không thể bỏ trống các trường bắt buộc";
                return View();
            }
            else
            {
                string _sPass = Security.Encrypt(PassWord);
                var user = _accountService.GetAccount(Type, UserName.ToLower(), _sPass);
                if (user == null)
                {
                    ViewBag.MessageError = "Email không tồn tại";
                    return View();
                }
                else
                {
                    if (user.IsActive)
                    {
                        TempData["success"] = "Xin chào " + user.UserName;
                        string _token = Guid.NewGuid().ToString();
                        var role = _roleService.GetItemByID(user.RoleID);
                        if (role != null)
                        {
                            HttpContext.SetValue(Cookies.DefaultLogin, _token, Cookies.ExpiresLogin, false);
                            //_ilogs.WriteLogsInfo(_token);
                            string FullName;
                            switch (user.Type)
                            {
                                case "teacher":
                                    var tc = _teacherService.GetItemByID(user.UserID);
                                    FullName = tc.FullName;
                                    break;
                                case "student":
                                    var st = _studentService.GetItemByID(user.UserID);
                                    FullName = st.FullName;
                                    break;
                                default:
                                    FullName = "admin";
                                    break;
                            }

                            var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Email, user.UserName),
                            new Claim(ClaimTypes.Name, FullName),
                            new Claim(ClaimTypes.Role, role.Code),
                            new Claim("Type", user.Type),
                            new Claim("RoleID", role.ID)
                        };
                            var claimsIdentity = new ClaimsIdentity(claims, Cookies.DefaultLogin);
                            _ = new AuthenticationProperties
                            {
                                IsPersistent = true,
                                ExpiresUtc = DateTime.UtcNow.AddMinutes(Cookies.ExpiresLogin)
                            };
                            ClaimsPrincipal claim = new ClaimsPrincipal();
                            claim.AddIdentity(claimsIdentity);

                            AccountLogEntity login = new AccountLogEntity()
                            {
                                IP = HttpContext.Connection.RemoteIpAddress.ToString(),
                                AccountID = user.ID,
                                Token = _token,
                                IsRemember = IsRemmember,
                                CreateDate = DateTime.Now
                            };
                            _logService.CreateQuery().InsertOne(login);
                            return Redirect(user.Type+"/Home/");
                        }
                        else
                        {
                            ViewBag.MessageError = "Bạn không có quyền hạn vào quản trị viện";
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.MessageError = "Tài khoản của bạn đã bị khóa";
                        return View();
                    }
                }
            }
        }
        [Route("/forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [Route("/forgot-password")]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(string Email)
        {
            return View();
        }
    }
}