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

namespace EnglishPlatform.Controllers
{
    public class HomeController : Controller
    {
        private readonly AccountService _accountService;
        private readonly RoleService _roleService;
        private readonly AccountLogService _logService;
        private readonly TeacherService _teacherService;
        private readonly StudentService _studentService;
        private readonly ILog _log;
        public HomeController(AccountService accountService, RoleService roleService, AccountLogService logService
            , TeacherService teacherService
            , StudentService studentService
            , ILog log)
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

        public IActionResult Register()
        {
            HttpContext.Remove(Cookies.DefaultLogin);
            return View();
        }
        [HttpPost]
        [Obsolete]
        public IActionResult Register(string UserName, string Name, string PassWord, string Type)
        {
            var _username = UserName.Trim().ToLower();
            if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(PassWord))
            {
                ViewBag.MessageError = "Please fill your information";
                return View();
            }
            else
            {
                string _sPass = Security.Encrypt(PassWord);
                if (_accountService.IsAvailable(_username))
                {
                    ViewBag.MessageError = "Account exist";
                    return View();
                }
                else
                {
                    var user = new AccountEntity()
                    {
                        PassWord = _sPass,
                        UserName = _username,
                        Name = Name,
                        Type = Type,
                        IsActive = false,
                        CreateDate = DateTime.Now,
                        UserCreate = null,
                    };
                    switch (Type)
                    {
                        case "teacher":
                            user.RoleID = _roleService.GetItemByCode("teacher").ID;
                            break;
                        default:
                            user.RoleID = _roleService.GetItemByCode("student").ID;
                            break;
                    }
                    _accountService.CreateQuery().InsertOne(user);
                    switch (Type)
                    {
                        case "teacher":
                            var teacher = new TeacherEntity()
                            {
                                FullName = user.Name,
                                Email = user.UserName,
                                IsActive = false,
                                CreateDate = DateTime.Now
                            };
                            _teacherService.CreateQuery().InsertOne(teacher);
                            user.UserID = teacher.ID;
                            //create teacher
                            break;
                        default:
                            var student = new StudentEntity()
                            {
                                FullName = user.Name,
                                Email = user.UserName,
                                IsActive = false,
                                CreateDate = DateTime.Now
                            };

                            _studentService.CreateQuery().InsertOne(student);
                            user.UserID = student.ID;
                            //create student
                            break;
                    }
                    var filter = Builders<AccountEntity>.Filter.Where(o => o.ID == user.ID);
                    _accountService.CreateQuery().ReplaceOne(filter, user);
                    ViewBag.Data = user;
                    return View();
                }
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
                Name = "Super Admin",
                Code = "superadmin",
                Type = "admin",
                CreateDate = DateTime.Now,
                IsActive = true,
                UserCreate = "longht"
            };
            var adminRole = new RoleEntity()
            {
                Name = "Admin",
                Code = "admin",
                Type = "admin",
                CreateDate = DateTime.Now,
                IsActive = true,
                UserCreate = "longht"
            };
            var headteacherRole = new RoleEntity()
            {
                Name = "Trưởng bộ môn",
                Code = "head-teacher",
                Type = "teacher",
                CreateDate = DateTime.Now,
                IsActive = true,
                UserCreate = "longht"
            };
            var teacherrole = new RoleEntity()
            {
                Name = "Giáo viên",
                Code = "teacher",
                Type = "teacher",
                CreateDate = DateTime.Now,
                IsActive = true,
                UserCreate = "longht"
            };
            var studentRole = new RoleEntity()
            {
                Name = "Học viên",
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
                ViewBag.MessageError = "Please fill your information";
                return View();
            }
            else
            {
                string _sPass = Security.Encrypt(PassWord);
                var user = _accountService.GetAccount(Type, UserName.ToLower(), _sPass);
                if (user == null)
                {
                    ViewBag.MessageError = "Account's infomation is not correct";
                    return View();
                }
                else
                {
                    if (user.IsActive)
                    {
                        TempData["success"] = "Hi " + user.UserName;
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
                            ViewBag.MessageError = "Can't signin now.";
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.MessageError = "Account is locked.";
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