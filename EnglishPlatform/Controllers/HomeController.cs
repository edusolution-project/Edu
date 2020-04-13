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
using BaseCustomerMVC.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using Newtonsoft.Json;

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
        private readonly CalendarHelper _calendarHelper;

        public DefaultConfigs _default { get; }

        public HomeController(AccountService accountService, RoleService roleService, AccountLogService logService
            , TeacherService teacherService
            , StudentService studentService
            , IAuthenService authenService
            , AccessesService accessesService
            , ClassService classService
            , IOptions<DefaultConfigs> defaultvalue
            , CalendarHelper calendarHelper
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
            _calendarHelper = calendarHelper;
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
        public async Task<JsonResult> LoginAPI(string UserName, string PassWord, string Type, bool IsRemmember)
        {
            //var x = HttpContext.Request;
            //_log.Debug("login", new { UserName, PassWord, Type, IsRemmember });
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(PassWord))
            {
                return Json(new ReturnJsonModel
                {
                    StatusCode = ReturnStatus.ERROR,
                    StatusDesc = "Vui lòng điền đầy đủ thông tin",
                });
            }
            else
            {
                string _sPass = Core_v2.Globals.Security.Encrypt(PassWord);
                var user = _accountService.GetAccount(UserName.ToLower(), _sPass);
                if (user == null)
                {
                    return Json(new ReturnJsonModel
                    {
                        StatusCode = ReturnStatus.ERROR,
                        StatusDesc = "Thông tin tài khoản không đúng"
                    });
                }
                else
                {
                    if (user.IsActive)
                    {
                        TempData["success"] = "Hi " + user.UserName;
                        string _token = Guid.NewGuid().ToString();
                        string FullName, id;
                        HttpContext.SetValue(Cookies.DefaultLogin, _token, Cookies.ExpiresLogin, false);
                        var role = _roleService.GetItemByID(user.RoleID);
                        if (role == null)
                        {
                            return Json(new ReturnJsonModel
                            {
                                StatusCode = ReturnStatus.ERROR,
                                StatusDesc = "Có lỗi, không đăng nhập được. Vui lòng liên hệ Admin để được trợ giúp"
                            });
                        }
                        switch (Type)
                        {
                            case ACCOUNT_TYPE.TEACHER:
                                var tc = _teacherService.GetItemByID(user.UserID);
                                if (tc == null)
                                    return Json(new ReturnJsonModel
                                    {
                                        StatusCode = ReturnStatus.ERROR,
                                        StatusDesc = "Thông tin tài khoản không đúng",
                                    });
                                FullName = tc.FullName;
                                id = tc.ID;
                                break;
                            case ACCOUNT_TYPE.STUDENT:
                                var st = _studentService.GetItemByID(user.UserID);
                                if (st == null)
                                {
                                    if (role.Type == "teacher")
                                    {
                                        //create student account
                                        st = new StudentEntity()
                                        {
                                            FullName = user.Name,
                                            Email = user.UserName,
                                            Phone = user.Phone,
                                            IsActive = true,// active student
                                            CreateDate = DateTime.Now,
                                            ID = user.UserID
                                        };
                                        _studentService.CreateQuery().InsertOne(st);
                                    }
                                    else
                                        return Json(new ReturnJsonModel
                                        {
                                            StatusCode = ReturnStatus.ERROR,
                                            StatusDesc = "Thông tin tài khoản không đúng",
                                        });
                                }
                                role = _roleService.GetItemByCode("student");
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
                                new Claim("Type", role.Type),
                                new Claim("RoleID", role.ID)
                            };

                        if (listAccess != null && listAccess.Count() > 0)
                        {
                            for (int i = 0; i < listAccess.Count(); i++)
                            {
                                var accItem = listAccess.ElementAt(i);
                                if (accItem.Type == "admin") claims.Add(new BaseAccess.Permission($"{accItem.Type}*{accItem.CtrlName}*{accItem.ActName}"));
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
                        return Json(new ReturnJsonModel
                        {
                            StatusCode = ReturnStatus.SUCCESS,
                            StatusDesc = "OK",
                            Location = "/" + Type
                        });
                    }
                    else
                    {
                        return Json(new ReturnJsonModel
                        {
                            StatusCode = ReturnStatus.ERROR,
                            StatusDesc = "Tài khoản đang bị khóa. Vui lòng liên hệ với quản trị viên để được hỗ trợ"
                        });
                    }
                }
            }
        }

        public async Task<JsonResult> RegisterAPI(string UserName, string Name, string Phone, string PassWord, string Type)
        {
            var _username = UserName.Trim().ToLower();
            if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(PassWord))
            {
                return Json(new ReturnJsonModel
                {
                    StatusCode = ReturnStatus.ERROR,
                    StatusDesc = "Please fill your information",
                });
            }
            else
            {
                string _sPass = Core_v2.Globals.Security.Encrypt(PassWord);
                if (_accountService.IsAvailable(_username))
                {
                    return Json(new ReturnJsonModel
                    {
                        StatusCode = ReturnStatus.ERROR,
                        StatusDesc = "Account exist",
                    });
                }
                else
                {
                    var user = new AccountEntity()
                    {
                        PassWord = _sPass,
                        UserName = _username,
                        Name = Name,
                        Phone = Phone,
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
                                Phone = user.Phone,
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
                                Phone = user.Phone,
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
                    return Json(new ReturnJsonModel
                    {
                        StatusCode = ReturnStatus.SUCCESS,
                        StatusDesc = "Tài khoản đã được tạo, mời bạn đăng nhập để trải nghiệm ngay!",
                        Location = "/Login"
                    });
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
                PassTemp = Core_v2.Globals.Security.Encrypt("123"),
                PassWord = Core_v2.Globals.Security.Encrypt("123"),
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


        [HttpPost]
        public IActionResult UploadImage(IFormFile upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            if (upload.Length <= 0) return null;
            //if (!upload.IsImage())
            //{
            //    var NotImageMessage = "please choose a picture";
            //    dynamic NotImage = JsonConvert.DeserializeObject("{ 'uploaded': 0, 'error': { 'message': \"" + NotImageMessage + "\"}}");
            //    return Json(NotImage);
            //}

            var fileName = Guid.NewGuid() + Path.GetExtension(upload.FileName).ToLower();

            //Image image = .FromStream(upload.OpenReadStream());
            //int width = image.Width;
            //int height = image.Height;
            //if ((width > 750) || (height > 500))
            //{
            //    var DimensionErrorMessage = "Custom Message for error"
            //    dynamic stuff = JsonConvert.DeserializeObject("{ 'uploaded': 0, 'error': { 'message': \"" + DimensionErrorMessage + "\"}}");
            //    return Json(stuff);
            //}

            //if (upload.Length > 500 * 1024)
            //{
            //    var LengthErrorMessage = "Custom Message for error";
            //    dynamic stuff = JsonConvert.DeserializeObject("{ 'uploaded': 0, 'error': { 'message': \"" + LengthErrorMessage + "\"}}");
            //    return Json(stuff);
            //}

            var path = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot/images/CKEditorImages",
                fileName);

            //using (var stream = new FileStream(path, FileMode.Create))
            //{
            //    upload.CopyTo(stream);
            //}

            var standardSize = new SixLabors.Primitives.Size(1024, 768);

            using (Stream inputStream = upload.OpenReadStream())
            {
                using (var image = Image.Load<Rgba32>(inputStream))
                {
                    var imageEncoder = new JpegEncoder()
                    {
                        Quality = 90,
                        Subsample = JpegSubsample.Ratio444
                    };

                    int width = image.Width;
                    int height = image.Height;
                    if ((width > standardSize.Width) || (height > standardSize.Height))
                    {
                        ResizeOptions options = new ResizeOptions
                        {
                            Mode = ResizeMode.Max,
                            Size = standardSize,
                        };
                        image.Mutate(x => x
                         .Resize(options));

                        //.Grayscale());
                    }
                    image.Save(path, imageEncoder); // Automatic encoder selected based on extension.
                }
            }

            var url = $"{"/images/CKEditorImages/"}{fileName}";
            var successMessage = "image is uploaded successfully";
            dynamic success = JsonConvert.DeserializeObject("{ 'uploaded': 1,'fileName': \"" + fileName + "\",'url': \"" + url + "\", 'error': { 'message': \"" + successMessage + "\"}}");
            return Json(success);
        }

        public IActionResult OnlineClass(string ID)
        {
            var @event = _calendarHelper.GetByEventID(ID);
            if (@event != null)
            {
                //@event.UrlRoom = "6725744943";

                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var type = User.Claims.GetClaimByType("Type").Value;
                if (type == "teacher")
                {
                    //ViewBag.Role = "1";
                    var teacher = _teacherService.GetItemByID(UserID);
                    //if (!string.IsNullOrEmpty(teacher.ZoomID))
                    if (teacher.ZoomID == @event.UrlRoom)
                    {
                        //var roomID = "6725744943";//test
                        ViewBag.URL = "https://zoom.us/wc/" + teacher.ZoomID.Replace("-", "") + "/join";
                        //ViewBag.URL = Url.Action("ZoomClass", "Home", new { roomID });
                    }
                    else
                        ViewBag.URL = @event.UrlRoom;
                }
                else
                {
                    //ViewBag.Role = "0";
                    ViewBag.Url = Url.Action("ZoomClass", "Home", new { roomID = @event.UrlRoom.Replace("-", "") });
                }
            }
            return View();
        }

        public IActionResult ZoomClass(string roomID)
        {
            ViewBag.RoomID = roomID;
            return View();
        }
    }
}