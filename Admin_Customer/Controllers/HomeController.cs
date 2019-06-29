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
using MongoDB.Driver;
using BaseCustomerMVC.Globals;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using System.Configuration;

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
        private readonly IConfiguration _config;

        public HomeController(AccountService accountService, RoleService roleService, AccountLogService logService
            , TeacherService teacherService
            , StudentService studentService
            , ILog log, IConfiguration config)
        {
            _accountService = accountService;
            _roleService = roleService;
            _logService = logService;
            _teacherService = teacherService;
            _studentService = studentService;
            _log = log;
            _config = config;
        }
        public IActionResult Index()
        {
            var type = User.Claims.GetClaimByType("Type");
            if (type != null)
            {
                return Redirect(type.Value);
            }
            else
            {
                HttpContext.SignOutAsync(Cookies.DefaultLogin);
                return RedirectToAction("Login");
            }
        }
        [Route("/login")]
        [System.Obsolete]
        public IActionResult Login()
        {
            long limit = 0;
            long count = _accountService.CreateQuery().Count(_ => true);
            if (count <= limit)
            {
                startPage();
            }
            return View();
        }

        private void startPage()
        {
            var superadminRole = new RoleEntity()
            {
                Name = _config.GetSection("Role:Admin:superadmin").Value,
                Code = "superadmin",
                Type = "admin",
                CreateDate = DateTime.Now,
                IsActive = true,
                UserCreate = "longht"
            };
            var adminRole = new RoleEntity()
            {
                Name = _config.GetSection("Role:Admin:admin").Value,
                Code = "admin",
                Type = "admin",
                CreateDate = DateTime.Now,
                IsActive = true,
                UserCreate = "longht"
            };
            var headteacherRole = new RoleEntity()
            {
                Name = _config.GetSection("Role:Teacher:head-teacher").Value,
                Code = "head-teacher",
                Type = "teacher",
                CreateDate = DateTime.Now,
                IsActive = true,
                UserCreate = "longht"
            };
            var teacherrole = new RoleEntity()
            {
                Name = _config.GetSection("Role:Teacher:teacher").Value,
                Code = "teacher",
                Type = "teacher",
                CreateDate = DateTime.Now,
                IsActive = true,
                UserCreate = "longht"
            };
            var studentRole = new RoleEntity()
            {
                Name = _config.GetSection("Role:Student:student").Value,
                Code = "student",
                Type = "student",
                CreateDate = DateTime.Now,
                IsActive = true,
                UserCreate = "longht"
            };
            _roleService.CreateQuery().InsertOne(headteacherRole);
            _roleService.CreateQuery().InsertOne(studentRole);
            _roleService.CreateQuery().InsertOne(teacherrole);
            _roleService.CreateQuery().InsertOne(superadminRole);
            _roleService.CreateQuery().InsertOne(adminRole);
            var superadmin = new AccountEntity()
            {
                CreateDate = DateTime.Now,
                IsActive = true,
                Type = "admin",
                UserName = "supperadmin@gmail.com",
                PassTemp = Security.Encrypt("123"),
                PassWord = Security.Encrypt("123"),
                UserID = "0", // admin
                RoleID = superadminRole.ID
            };
            _accountService.CreateQuery().InsertOne(superadmin);
        }
        [Route("/logout")]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(Cookies.DefaultLogin);
            HttpContext.Remove(Cookies.DefaultLogin);
            return RedirectToAction("Login");
        }

        [HttpPost]
        [Route("/login")]
        public async Task<IActionResult> Login(string UserName, string PassWord, string Type, bool IsRemmember)
        {
            //_log.Debug("login", new { UserName, PassWord, Type, IsRemmember });
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
                            string FullName, id;
                            switch (user.Type)
                            {
                                case "teacher":
                                    var tc = _teacherService.GetItemByID(user.UserID);
                                    FullName = tc.FullName;
                                    id = tc.ID;
                                    break;
                                case "student":
                                    var st = _studentService.GetItemByID(user.UserID);
                                    FullName = st.FullName;
                                    id = st.ID;
                                    break;
                                default:
                                    FullName = "admin"; id = "0";
                                    break;
                            }

                            var claims = new List<Claim>
                            {
                                new Claim("UserID",id),
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
                            await HttpContext.SignInAsync(Cookies.DefaultLogin, claim, new AuthenticationProperties()
                            {
                                ExpiresUtc = !IsRemmember ? DateTime.Now : DateTime.Now.AddMinutes(Cookies.ExpiresLogin),
                                AllowRefresh = true,
                                RedirectUri = user.Type
                            });
                            return Redirect(user.Type);
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