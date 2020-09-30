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
using SixLabors.ImageSharp.Formats.Jpeg;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Routing;

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
        private readonly MailHelper _mailHelper;
        private readonly ClassService _classService;
        private readonly StudentHelper _studentHelper;
        private readonly LessonScheduleService _scheduleService;
        private readonly CalendarHelper _calendarHelper;
        private readonly UserAndRoleService _userAndRoleService;
        private readonly CenterService _centerService;
        private readonly AuthorityService _authorityService;
        private readonly NewsService _newsService;
        private readonly NewsCategoryService _newsCategoryService;
        private readonly QCService _QCService;
        public DefaultConfigs _default { get; }

        private string host;

        private readonly ISession _session;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(AccountService accountService, RoleService roleService, AccountLogService logService
            , TeacherService teacherService
            , StudentService studentService
            , IAuthenService authenService
            , AccessesService accessesService
            , ClassService classService
            , IOptions<DefaultConfigs> defaultvalue
            , CalendarHelper calendarHelper
            , MailHelper mailHelper
            , UserAndRoleService userAndRoleService
            , CenterService centerService
            , LessonScheduleService scheduleService
            , AuthorityService authorityService
            , ILog log
            , NewsService newsService
            , NewsCategoryService newsCategoryService
            , QCService QCService
            , IConfiguration iConfig
            , IHttpContextAccessor httpContextAccessor
            )
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
            _scheduleService = scheduleService;
            _mailHelper = mailHelper;
            _default = defaultvalue.Value;
            _userAndRoleService = userAndRoleService;
            _centerService = centerService;
            _authorityService = authorityService;
            _newsService = newsService;
            _newsCategoryService = newsCategoryService;
            _QCService = QCService;
            host = iConfig.GetValue<string>("SysConfig:Domain");
            _httpContextAccessor = httpContextAccessor;
            _session = _httpContextAccessor.HttpContext.Session;
        }

        public IActionResult Index()
        {
            StartAuthority();
            var type = User.Claims.GetClaimByType("Type");
            if (type != null)
            {
                var center = _centerService.GetDefault();
                string centerCode = center.Code;
                //string roleCode = "";
                string userID = User.FindFirst("UserID").Value;
                var tc = _teacherService.GetItemByID(userID);
                var st = _studentService.GetItemByID(userID);
                //var user = _accountService.GetItemByID(userID);
                //var defaultUser = new UserModel() { };

                switch (type.Value)
                {
                    case ACCOUNT_TYPE.ADMIN:
                        centerCode = center.Code;
                        break;
                    case ACCOUNT_TYPE.TEACHER:
                        if (tc != null)
                        {
                            var centers = tc.Centers
                                .Where(ct => _centerService.GetItemByID(ct.CenterID)?.ExpireDate > DateTime.Now)
                                .Select(t => new CenterEntity { Code = t.Code, Name = t.Name }).ToList();
                            if (centers != null)
                                centerCode = centers.FirstOrDefault().Code;
                            else
                                centerCode = center.Code;

                            _session.SetString("userAvatar", tc.Avatar==null? "/images/no-avatar.png":tc.Avatar);
                        }
                        break;
                    default:
                        if (st != null)
                        {
                            if (st.Centers != null && st.Centers.Count > 0)
                            {
                                var allcenters = st.Centers
                                    .Where(ct => _centerService.GetItemByID(ct)?.ExpireDate > DateTime.Now)
                                    .Select(t => _centerService.GetItemByID(t)).ToList();
                                centerCode = allcenters.Count > 0 ? allcenters.FirstOrDefault().Code : center.Code;
                                ViewBag.AllCenters = allcenters;
                            }
                            else
                            {
                                centerCode = center.Code;
                            }

                            _session.SetString("userAvatar", st.Avatar == null ? "/images/no-avatar.png" : st.Avatar);
                        }
                        break;
                }
                ViewBag.Type = type.Value;
                //cache
                return Redirect($"{centerCode}/{type.Value}");
            }
            else
            {
                _authenService.SignOut(HttpContext, Cookies.DefaultLogin);
                HttpContext.SignOutAsync(Cookies.DefaultLogin);
                return RedirectToAction("Login");
            }
            return View();
        }


        [Route("/login")]
        public IActionResult Login()
        {
            //long limit = 0;
            //long count = _accountService.CreateQuery().CountDocuments(_ => true);
            //if (count <= limit)
            //{
            //    startPage();
            //}
            return View();
        }

        [Route("/logincp")]
        public IActionResult LoginCP()
        {
            ViewBag.Adm = true;
            return View("Login");
        }

        [HttpPost]
        public async Task<JsonResult> LoginAPI(string UserName, string PassWord, string Type, bool IsRemmember)
        {
            //var x = HttpContext.Request;
            //_log.Debug("login", new { UserName, PassWord, Type, IsRemmember });
            try
            {
                var _username = UserName.Trim().ToLower();
                if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(PassWord))
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
                    var user = _accountService.GetAccount(_username, _sPass);
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
                            TempData["success"] = "Hi " + _username;
                            string _token = Guid.NewGuid().ToString();
                            HttpContext.SetValue(Cookies.DefaultLogin, _token, Cookies.ExpiresLogin, false);

                            var center = _centerService.GetDefault();
                            string centerCode = center.Code;
                            string roleCode = "";
                            var tc = _teacherService.GetItemByID(user.UserID) ?? _teacherService.GetItemByEmail(user.UserName);
                            var st =
                                user.Type == "teacher" ?
                                _studentService.GetStudentByEmail(user.UserName) :
                                _studentService.GetItemByID(user.UserID);

                            var defaultUser = new UserModel() { };
                            switch (Type)
                            {
                                case ACCOUNT_TYPE.TEACHER:
                                    if (tc != null)
                                    {
                                        defaultUser = new UserModel(tc.ID, tc.FullName);

                                        if (tc.Centers != null && tc.Centers.Count > 0)//return to first valid center
                                        {
                                            foreach (var ct in tc.Centers)
                                            {
                                                var _ct = _centerService.GetItemByID(ct.CenterID);
                                                if (_ct == null || _ct.ExpireDate <= DateTime.Now)
                                                {
                                                    continue;
                                                    //return Json(new ReturnJsonModel
                                                    //{
                                                    //    StatusCode = ReturnStatus.ERROR,
                                                    //    StatusDesc = "Cơ sở không hoạt động hoặc đã hết hạn, vui lòng liên hệ quản trị để được hỗ trợ"
                                                    //});
                                                }
                                                else
                                                {
                                                    centerCode = _ct.Code;
                                                    roleCode = _roleService.GetItemByID(ct.RoleID).Code;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            centerCode = center.Code;
                                            roleCode = "";
                                        }
                                    }
                                    break;
                                case ACCOUNT_TYPE.ADMIN:
                                    defaultUser = new UserModel(user.ID, "admin");
                                    centerCode = center.Code;
                                    roleCode = user.UserName == "supperadmin@gmail.com" ? "superadmin" : "admin";
                                    break;
                                default:
                                    if (st != null)
                                    {
                                        defaultUser = new UserModel(st.ID, st.FullName);
                                        centerCode = (st.Centers != null && st.Centers.Count > 0) ? _centerService.GetItemByID(st.Centers.FirstOrDefault()).Code : center.Code;
                                        roleCode = "student";
                                    }
                                    break;
                            }
                            if (Type != ACCOUNT_TYPE.ADMIN)
                            {
                                var role =
                                    //roleCode != "student" ? _roleService.GetItemByID(roleCode) : 
                                    _roleService.GetItemByCode(roleCode);
                                if (role != null)
                                {
                                    var listAccess = _accessesService.GetAccessByRole(role.Code);
                                    string key = $"{roleCode}";
                                    CacheExtends.SetObjectFromCache($"{defaultUser.ID}_{centerCode}", 3600 * 24 * 360, key);
                                    if (CacheExtends.GetDataFromCache<List<string>>(key) == null)
                                    {
                                        var access = listAccess.Select(o => o.Authority)?.ToList();
                                        CacheExtends.SetObjectFromCache(key, 3600 * 24 * 360, access);
                                    }
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

                            //cache
                            var claims = new List<Claim>{
                                new Claim("UserID",defaultUser.ID),
                                new Claim(ClaimTypes.Email, _username),
                                new Claim(ClaimTypes.Name, defaultUser.Name),
                                new Claim(ClaimTypes.Role,roleCode),
                                new Claim("Type", Type)};


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
                            return Json(new ReturnJsonModel
                            {
                                StatusCode = ReturnStatus.SUCCESS,
                                StatusDesc = "OK",
                                Location = $"{centerCode}/{Type}"
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult RegisterAPI(string UserName, string Name, string Phone, string PassWord, string Type)
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
                    var defCenter = _centerService.GetItemByCode("eduso");

                    var user = new AccountEntity()
                    {
                        PassWord = _sPass,
                        UserName = _username,
                        Name = Name ?? _username,
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
                                Email = _username,
                                Phone = user.Phone,
                                IsActive = false,
                                CreateDate = DateTime.Now,
                                Centers = new List<CenterMemberEntity> { new CenterMemberEntity { CenterID = defCenter.ID, Code = defCenter.Code, Name = defCenter.Name, RoleID = user.RoleID } }
                            };
                            _teacherService.CreateQuery().InsertOne(teacher);
                            user.UserID = teacher.ID;
                            //send email for teacher
                            _ = _mailHelper.SendTeacherJoinCenterNotify(teacher.FullName, teacher.Email, PassWord, defCenter.Name);
                            break;
                        default: //temporary block
                            //create student
                            var student = new StudentEntity()
                            {
                                FullName = user.Name,
                                Email = _username,
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
                    _ = _mailHelper.SendRegisterEmail(user, PassWord);
                    return Json(new ReturnJsonModel
                    {
                        StatusCode = ReturnStatus.SUCCESS,
                        StatusDesc = "Tài khoản đã được tạo, mời bạn đăng nhập để trải nghiệm ngay!",
                        Location = "/Login"
                    });
                }
            }
        }

        //Quên mật khẩu
        public async Task<JsonResult> ForgotAPI(string UserName)
        {
            var Error = "";
            var Status = false;
            var Url = "";
            var StatusDesc = "";
            try
            {
                var user = _accountService.GetAccountByEmail(UserName);
                if (user == null)
                {
                    Error += "Tài khoản không tồn tại! Vui lòng nhập lại tài khoản";
                }
                else
                {
                    var OTP = new Random().Next(100000, 999999).ToString();
                    var VerificationCodes = Core_v2.Globals.Security.Encrypt(OTP);
                    if (_accountService.CreateQuery().Find(x => x.VerificationCodes == VerificationCodes).Any())
                    {
                        OTP = new Random().Next(100000, 999999).ToString();
                    }
                    user.VerificationCodes = Core_v2.Globals.Security.Encrypt(OTP);
                    user.TimeOut = DateTime.Now; //Thời gian tồn tại mã xác nhận, max 300s
                    _accountService.CreateOrUpdate(user);

                    var resetLink = $"https://{host}/forgot-password?code={OTP}";
                    _ = _mailHelper.SendResetPassConfirm(user, resetLink, OTP);
                    Status = true;
                    Url = $"https://{host}/forgot-password";
                }
            }
            catch (Exception ex)
            {
                StatusDesc = ex.Message;
            }

            var DataRespone = new Dictionary<string, object>()
            {
                {"Status",Status },
                {"Error",Error },
                {"Url",Url },
                {"StatusDesc",StatusDesc }
            };
            return Json(DataRespone);
        }

        [Route("/forgot-password")]
        public IActionResult ForgotPassword(string code = "0")
        {
            if (code != "0")
                ViewBag.VerificationCodes = code;
            return View();
        }

        //[HttpPost]
        //[Route("/forgot-password")]
        //[ValidateAntiForgeryToken]
        //public JsonResult ResetPassword(string Email)
        //{
        //    return View("ForgotPassword");
        //}

        ////[Route("/forgot-password")]
        ////[HttpPost]
        //public IActionResult ForgotPassword(string VerificationCodes = "")
        //{
        //    ViewBag.VerificationCodes = VerificationCodes;
        //    return View();
        //}

        //dat lai mat khau
        [HttpPost]
        //[Route("/forgot-password")]
        //[ValidateAntiForgeryToken]
        public JsonResult ResetPassword(string NewPassword, string Email = "", string VerificationCodes = "")
        {
            var Error = "";
            var Message = "";
            var Url = "";
            var Status = false;
            if (Email != "" && VerificationCodes != "")
            {
                var user = _accountService.GetAccountByEmail(Email);
                if (user == null)
                {
                    Error += $"Email: {Email} không tồn tại!";
                    Status = false;
                }
                else
                {
                    var checkTimeOut = DateTime.Now;
                    var TotalSeconds = (checkTimeOut - user.TimeOut).TotalSeconds;

                    if (TotalSeconds < 0 && TotalSeconds > 300)
                    {
                        Error = "Mã xác thực đã hết hạn, vui lòng thực hiện lại từ đầu!";
                        Status = false;
                    }
                    else if (user.VerificationCodes != Core_v2.Globals.Security.Encrypt(VerificationCodes))
                    {
                        Error = "Mã xác thực không đúng";
                        Status = false;
                    }
                    else
                    {
                        user.PassTemp = Core_v2.Globals.Security.Encrypt(NewPassword);
                        user.PassWord = Core_v2.Globals.Security.Encrypt(NewPassword);
                        user.TimeOut = new DateTime(1990, 01, 01, 00, 00, 00);
                        user.VerificationCodes = "";
                        _accountService.CreateOrUpdate(user);
                        _ = _mailHelper.SendPasswordChangeNotify(user, NewPassword);
                        Message = "Thay đổi mật khẩu thành công! Đang điều hướng về trang đăng nhập...";
                        Url = $"https://{host}/login";
                        Status = true;
                    }
                }
            }
            var DataRespone = new Dictionary<string, object>()
            {
                {"Message",Message },
                {"Error",Error },
                {"Url",Url },
                {"Status",Status }
            };
            return Json(DataRespone);
        }

        private void StartAuthority()
        {
            if (CacheExtends.GetDataFromCache<List<AuthorityEntity>>(CacheExtends.DefaultPermission) == null)
            {
                List<AuthorityEntity> data = _authorityService.GetAll()?.ToList();
                CacheExtends.SetObjectFromCache(CacheExtends.DefaultPermission, 3600 * 24 * 360, data);
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
                Name = "GV Quản lý",
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
            var basis = HttpContext.Request;
            string keys = User.FindFirst("UserID")?.Value;
            if (!string.IsNullOrEmpty(keys))
            {
                CacheExtends.ClearCache(keys);
            }
            await HttpContext.SignOutAsync(Cookies.DefaultLogin);
            HttpContext.Remove(Cookies.DefaultLogin);

            return RedirectToAction("Login");
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
            var teacher = _teacherService.GetItemByID(@event.TeacherID);//check teacher of class

            if (teacher == null)
                throw (new Exception("Teacher not found"));
            if (@event != null)
            {
                //@event.UrlRoom = "6725744943";

                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var type = User.Claims.GetClaimByType("Type").Value;
                if (type == "teacher")
                {
                    //ViewBag.Role = "1";
                    //var teacher = _teacherService.GetItemByID(UserID);
                    //if (!string.IsNullOrEmpty(teacher.ZoomID))
                    if (teacher.ZoomID == @event.UrlRoom)
                    {
                        //var roomID = "6725744943";//test
                        ViewBag.URL = "https://zoom.us/wc/" + teacher.ZoomID.Replace("-", "").Replace(" ", "") + "/join";
                        //ViewBag.URL = Url.Action("ZoomClass", "Home", new { roomID });
                    }
                    else
                        ViewBag.URL = @event.UrlRoom;
                }
                else
                {
                    string zoomId = (teacher != null && !string.IsNullOrEmpty(teacher.ZoomID) ? teacher.ZoomID : @event.UrlRoom).Replace("-", "").Replace(" ", "");
                    //ViewBag.Role = "0";
                    ViewBag.Url = Url.Action("ZoomClass", "Home", new { roomID = zoomId });
                }
            }

            var schedule = _scheduleService.GetItemByID(@event.ScheduleID);
            if (schedule != null)
            {
                try
                {
                    var center = _centerService.GetItemByID(_classService.GetItemByID(schedule.ClassID).Center).Code;
                    ViewBag.LessonUrl = $"/{center}/student/Lesson/Detail/{schedule.LessonID}/{schedule.ClassSubjectID}";
                }
                catch (Exception e)
                {

                }
            }

            return View();
        }

        public IActionResult ZoomClass(string roomID)
        {
            ViewBag.RoomID = roomID;
            return View();
        }

        #region LoadNews
        [HttpPost]
        [Route("/home/getnewslist")]
        public JsonResult getNewsList()
        {
            var NewsTop = _newsService.Collection.Find(tbl => tbl.IsTop == true && tbl.PublishDate < DateTime.Now && tbl.IsActive == true).Limit(5);
            var NewsHot = _newsService.Collection.Find(tbl => tbl.IsHot == true && tbl.PublishDate < DateTime.Now && tbl.IsActive == true).Limit(2);
            var response = new Dictionary<string, object>
            {
                {"NewsTop",NewsTop.ToList() },
                {"NewsHot",NewsHot.ToList() }
            };

            return Json(response);
        }

        public JsonResult getDataForPartner(string CatCode)
        {
            var category = _newsCategoryService.Collection.Find(tbl => tbl.Code.Equals(CatCode)).FirstOrDefault();
            List<NewsEntity> data = null;
            if (category != null)
                data = _newsService.Collection.Find(tbl => tbl.CategoryID.Equals(category.ID)).Limit(10).SortByDescending(tbl => tbl.PublishDate).ToList();
            return Json(data);
        }
        #endregion

        #region load Banner
        public JsonResult getDataForBanner()
        {
            var filter = new List<FilterDefinition<QCEntity>>();
            filter.Add(Builders<QCEntity>.Filter.Where(tbl => tbl.PublishDate <= DateTime.UtcNow));
            filter.Add(Builders<QCEntity>.Filter.Where(tbl => tbl.EndDate >= DateTime.UtcNow));
            filter.Add(Builders<QCEntity>.Filter.Where(tbl => tbl.IsActive == true));
            var data = _QCService.Collection.Find(Builders<QCEntity>.Filter.And(filter)).Project(tbl => tbl.Banner).ToList();
            Dictionary<string, object> Response = new Dictionary<string, object>
            {
                {"Data",data }
            };
            return new JsonResult(Response);
        }
        #endregion

        #region Fix Data
        public JsonResult FixAcc()
        {
            var students = _studentService.GetAll().ToEnumerable();
            var count = 0;
            var countdelete = 0;
            var str = "";
            foreach (var student in students)
            {
                if (student == null)
                    continue;

                var alias = _studentService.CreateQuery().Find(t => t.Email == student.Email).ToList();
                var accs = _accountService.CreateQuery().Find(t => t.UserName == student.Email).ToList();
                if (alias != null && alias.Count == 1 && accs != null && accs.Count == 1) continue;

                str += (student.Email + "<br/>");
                //if (alias == null) continue;
                //var JoinClasses = new List<string>();
                //var JoinCenters = new List<string>();

                //if (alias.Count >= 1)
                //{
                //    foreach(var st in alias)
                //    {
                //        JoinClasses.AddRange(st.JoinedClasses);
                //        JoinCenters.AddRange(st.Centers);
                //    }    
                //}    

                //if(accs.Count() == 1)
                //{
                //    var acc = accs[0];
                //    if(acc.Type == "student")
                //    {
                //        if(acc.UserID == student.ID)
                //        {
                //            student.JoinedClasses = JoinClasses;
                //            student.Centers = JoinCenters;
                //        }
                //        _studentService.CreateQuery().ReplaceOne(t => t.ID == student.ID, student);
                //        _studentService.CreateQuery().DeleteMany(t => t.Email == student.Email && t.ID != student.ID);
                //    }   
                //    else
                //    {
                //        acc.UserId = student.ID;
                //    }    
                //}    

                //if (student.Email != student.Email.ToLower().Trim())
                //{
                //    student.Email = student.Email.ToLower().Trim();
                //    _studentService.Save(student);
                //}
                //try
                //{
                //    var acc = _accountService.GetAccountByEmail(student.Email);
                //    if (acc == null)
                //    {
                //        var _defaultPass = "Eduso123";
                //        acc = new AccountEntity()
                //        {
                //            CreateDate = DateTime.Now,
                //            IsActive = true,
                //            PassTemp = Core_v2.Globals.Security.Encrypt(_defaultPass),
                //            PassWord = Core_v2.Globals.Security.Encrypt(_defaultPass),
                //            UserCreate = student.UserCreate,
                //            Type = ACCOUNT_TYPE.STUDENT,
                //            UserID = student.ID,
                //            UserName = student.Email.ToLower().Trim(),
                //            RoleID = _roleService.GetItemByCode("student").ID
                //        };
                //        _accountService.CreateQuery().InsertOne(acc);
                //        count++;
                //    }
                //    else
                //    {
                //        if (acc.Type == "student" && acc.UserID != student.ID)
                //        {
                //            var exactStudent = _studentService.GetItemByID(acc.UserID);
                //            if (exactStudent == null)
                //            {
                //                acc.UserID = student.ID;
                //                _accountService.CreateQuery().ReplaceOne(t => t.ID == acc.ID, acc);
                //            }
                //            else
                //            {
                //                _studentService.CreateQuery().DeleteMany(t => t.Email.ToLower() == student.Email.ToLower() && t.ID != acc.UserID);
                //            }
                //        }
                //    }
                //}
                //catch (Exception e)
                //{
                //    var accs = _accountService.CreateQuery().Find(t => t.UserName == student.Email).SortBy(t => t.ID).ToList();
                //    if (accs.Count > 1)
                //    {
                //        for (int i = 1; i < accs.Count; i++)
                //        {
                //            _accountService.Remove(accs[i].ID);
                //        }
                //        countdelete++;
                //    }
                //}
            }
            return Json("OK " + count + " - " + countdelete + " _ " + str);
        }
       
        #endregion
        [HttpGet]
        public string CurrentUser()
        {
            try
            {
                string strScript = "var g_UserOnline={id:'',name:'',email:''}";
                if(User.Identity.IsAuthenticated)
                {
                    string userId = User.FindFirst("UserID").Value;
                    string name = User.Identity.Name;
                    string email = User.FindFirst(System.Security.Claims.ClaimTypes.Email).Value;
                    strScript = "var g_UserOnline={id:'" + userId + "',name:'"+name+"',email:'"+email+"'}";
                }

                return strScript;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }

    public class UserModel
    {
        public UserModel()
        {
        }

        public UserModel(string iD, string name)
        {
            ID = iD;
            Name = name;
        }

        public string ID { get; set; }
        public string Name { get; set; }
    }
}
