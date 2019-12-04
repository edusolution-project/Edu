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
using BaseAccess.Interfaces;
using Microsoft.Extensions.Options;

namespace EnglishPlatform.Controllers
{
    public class HomeController : Controller
    {
        private readonly AccessesService _accessesService;
        private readonly IAuthenService _authenService;
        private readonly AccountService _accountService;
        private readonly RoleService _roleService;
        private readonly AccountLogService _logService;
        private readonly TeacherService _teacherService;
        private readonly StudentService _studentService;
        private readonly ILog _log;
        private readonly ClassService _classService;
        private readonly StudentHelper _studentHelper;
        public DefaultConfigs _default { get; }

        public HomeController(AccountService accountService, RoleService roleService, AccountLogService logService
            , TeacherService teacherService
            , StudentService studentService
            , IAuthenService authenService
            , AccessesService accessesService
            , ClassService classService
            , IOptions<DefaultConfigs> defaultvalue
            , ILog log)
        {
            _accessesService = accessesService;
            _authenService = authenService;
            _accountService = accountService;
            _roleService = roleService;
            _logService = logService;
            _teacherService = teacherService;
            _studentService = studentService;
            _classService = classService;
            _studentHelper = new StudentHelper(studentService, accountService);
            _log = log;
            _default = defaultvalue.Value;
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
                _authenService.SignOut(HttpContext, Cookies.DefaultLogin);
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
                        case ACCOUNT_TYPE.TEACHER:
                            user.RoleID = _roleService.GetItemByCode("teacher").ID;
                            break;
                        default:
                            user.IsActive = true;// active student
                            user.RoleID = _roleService.GetItemByCode("student").ID;
                            break;
                    }
                    _accountService.CreateQuery().InsertOne(user);
                    switch (Type)
                    {
                        case ACCOUNT_TYPE.TEACHER:
                            //create teacher
                            var teacher = new TeacherEntity()
                            {
                                FullName = user.Name,
                                Email = user.UserName,
                                IsActive = false,
                                CreateDate = DateTime.Now
                            };
                            _teacherService.CreateQuery().InsertOne(teacher);
                            user.UserID = teacher.ID;
                            //send email for teacher
                            break;
                        default:
                            //create student
                            var student = new StudentEntity()
                            {
                                FullName = user.Name,
                                Email = user.UserName,
                                IsActive = true,// active student
                                CreateDate = DateTime.Now
                            };

                            _studentService.CreateQuery().InsertOne(student);
                            user.UserID = student.ID;
                            //send email for student
                            //add default class
                            var testClassIDs = _default.defaultClassID;
                            if (!string.IsNullOrEmpty(testClassIDs))
                            {
                                foreach (var id in testClassIDs.Split(';'))
                                    if (!string.IsNullOrEmpty(id))
                                        _classService.AddStudentToClass(id, student.ID);
                            }
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
        public IActionResult Login()
        {
            long limit = 0;
            long count = _accountService.CreateQuery().CountDocuments(_ => true);
            if (count <= limit)
            {
                startPage();
            }
            return View();
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
                                case ACCOUNT_TYPE.TEACHER:
                                    var tc = _teacherService.GetItemByID(user.UserID);
                                    FullName = tc.FullName;
                                    id = tc.ID;
                                    break;
                                case ACCOUNT_TYPE.STUDENT:
                                    var st = _studentService.GetItemByID(user.UserID);
                                    FullName = st.FullName;
                                    id = st.ID;
                                    break;
                                default:
                                    FullName = "admin"; id = "0";
                                    break;
                            }
                            var listAccess = _accessesService.GetAccessByRole(role.ID);
                            
                            var claims = new List<Claim>
                            {
                                new Claim("UserID",id),
                                new Claim(ClaimTypes.Email, user.UserName),
                                new Claim(ClaimTypes.Name, FullName),
                                new Claim(ClaimTypes.Role, role.Code),
                                new Claim("Type", user.Type),
                                new Claim("RoleID", role.ID)
                            };
                            if (listAccess != null && listAccess.Count() > 0)
                            {
                                for (int i = 0; i < listAccess.Count(); i++)
                                {
                                    var accItem = listAccess.ElementAt(i);
                                    if(accItem.Type == "admin") claims.Add(new BaseAccess.Permission($"{accItem.Type}*{accItem.CtrlName}*{accItem.ActName}"));
                                    else claims.Add(new BaseAccess.Permission($"{accItem.Type}*{accItem.CtrlName}"));
                                    
                                }
                            }
                            var claimsIdentity = new ClaimsIdentity(claims, Cookies.DefaultLogin);
                            _ = new AuthenticationProperties
                            {
                                IsPersistent = true,
                                ExpiresUtc = DateTime.UtcNow.AddMinutes(Cookies.ExpiresLogin)
                            };
                            ClaimsPrincipal claim = new ClaimsPrincipal(claimsIdentity);
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
                            await _authenService.SignIn(HttpContext, claim, Cookies.DefaultLogin);
                            //await HttpContext.SignInAsync(Cookies.DefaultLogin, claim, new AuthenticationProperties()
                            //{
                            //    ExpiresUtc = !IsRemmember ? DateTime.Now : DateTime.Now.AddMinutes(Cookies.ExpiresLogin),
                            //    AllowRefresh = true,
                            //    RedirectUri = user.Type
                            //});
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

        private void startPage()
        {
            var superadminRole = new RoleEntity()
            {
                Name = "Super Admin",
                Code = "superadmin",
                Type = ACCOUNT_TYPE.ADMIN,
                CreateDate = DateTime.Now,
                IsActive = true,
                UserCreate = "longht"
            };
            var adminRole = new RoleEntity()
            {
                Name = "Admin",
                Code = "admin",
                Type = ACCOUNT_TYPE.ADMIN,
                CreateDate = DateTime.Now,
                IsActive = true,
                UserCreate = "longht"
            };
            var headteacherRole = new RoleEntity()
            {
                Name = "Trưởng bộ môn",
                Code = "head-teacher",
                Type = ACCOUNT_TYPE.TEACHER,
                CreateDate = DateTime.Now,
                IsActive = true,
                UserCreate = "longht"
            };
            var teacherrole = new RoleEntity()
            {
                Name = "Giáo viên",
                Code = "teacher",
                Type = ACCOUNT_TYPE.TEACHER,
                CreateDate = DateTime.Now,
                IsActive = true,
                UserCreate = "longht"
            };
            var studentRole = new RoleEntity()
            {
                Name = "Học viên",
                Code = "student",
                Type = ACCOUNT_TYPE.STUDENT,
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
                Type = ACCOUNT_TYPE.ADMIN,
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