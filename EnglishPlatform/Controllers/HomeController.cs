﻿using System;
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
using System.Text.RegularExpressions;
using FileManagerCore.Interfaces;

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

        private readonly ClassProgressService _classProgressService;
        private readonly ClassSubjectService _classSubjectService;
        //private readonly LessonScheduleService _lessonScheduleService;
        private readonly LessonService _lessonService;
        private readonly SkillService _skillService;
        private readonly SubjectService _subjectService;
        private readonly LearningHistoryService _learningHistoryService;
        private readonly ClassSubjectProgressService _classSubjectProgressService;
        private readonly CourseService _courseService;
        private readonly GradeService _gradeService;
        private readonly ReferenceService _referenceService;
        private readonly ProgressHelper _progressHelper;
        private readonly CloneLessonPartService _cloneLessonPartService;
        private readonly CloneLessonPartAnswerService _cloneLessonPartAnswerService;
        private readonly CloneLessonPartQuestionService _cloneLessonPartQuestionService;
        private readonly ExamService _examService;
        private readonly VocabularyService _vocabularyService;

        private readonly IRoxyFilemanHandler _roxyFilemanHandler;
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly LessonProgressService _lessonProgressService;

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

            , ClassProgressService classProgressService
            , ClassSubjectService classSubjectService
            , LessonService lessonService
            , SkillService skillService
            , SubjectService subjectService
            , LearningHistoryService learningHistoryService
            , ClassSubjectProgressService classSubjectProgressService
            , CourseService courseService
            , GradeService gradeService
            , ReferenceService referenceService
            , ProgressHelper progressHelper
            , CloneLessonPartService cloneLessonPartService
            , CloneLessonPartAnswerService cloneLessonPartAnswerService
            , CloneLessonPartQuestionService cloneLessonPartQuestionService
            , ExamService examService
            , VocabularyService vocabularyService
            , IRoxyFilemanHandler roxyFilemanHandler
            , LessonScheduleService lessonScheduleService
            , LessonProgressService lessonProgressService
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

            _classProgressService = classProgressService;
            _classSubjectService = classSubjectService;
            _lessonService = lessonService;
            _skillService = skillService;
            _subjectService = subjectService;
            _learningHistoryService = learningHistoryService;
            _classSubjectProgressService = classSubjectProgressService;
            _courseService = courseService;
            _gradeService = gradeService;
            _referenceService = referenceService;
            _progressHelper = progressHelper;
            _cloneLessonPartService = cloneLessonPartService;
            _cloneLessonPartAnswerService = cloneLessonPartAnswerService;
            _cloneLessonPartQuestionService = cloneLessonPartQuestionService;
            _examService = examService;
            _vocabularyService = vocabularyService;
            _roxyFilemanHandler = roxyFilemanHandler;

            _lessonScheduleService = lessonScheduleService;
            _lessonProgressService = lessonProgressService;
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

                            _session.SetString("userAvatar", tc.Avatar == null ? "/images/no-avatar.png" : tc.Avatar);
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
                //_authenService.SignOut(HttpContext, Cookies.DefaultLogin);
                //HttpContext.SignOutAsync(Cookies.DefaultLogin);
                //return RedirectToAction("Login");
                return View();
            }
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
                if (User.Identity.IsAuthenticated)
                {
                    string userId = User.FindFirst("UserID").Value;
                    string name = User.Identity.Name;
                    string email = User.FindFirst(System.Security.Claims.ClaimTypes.Email).Value;
                    strScript = "var g_UserOnline={id:'" + userId + "',name:'" + name + "',email:'" + email + "'}";
                }

                return strScript;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #region Test
        //[HttpPost]
        //[Route("/home/test/{id}")]
        public IActionResult Test(string ID)
        {
            StudentEntity userST = null;
            userST = _studentService.GetItemByID(ID);
            if (userST == null)
            {
                return View("Test/Home");
            }
            var center = _centerService.GetItemByID(userST.Centers[0]);
            if (userST.JoinedClasses == null || userST.JoinedClasses.Count() == 0)
            {
                return View("Test/Home");
            }
            var ClassID = userST.JoinedClasses[0];
            var classsb = _classSubjectService.GetByClassID(ClassID);
            if (classsb == null)
            {
                return View("Test/Home");
            }
            //var classsb = _class[0];
            //ViewBag.Center = center.Code;
            if (center == null)
                return Json(new { Err = "Không có dữ liệu" });

            var a = getbeatstudent(center);
            var b = GetThisWeekLesson(userST, center, DateTime.Now);
            var c = GetActiveListV2(userST, center, DateTime.Now);
            var d = GetFinishList(userST, center, DateTime.Now);
            var e = GetClassSubjects(userST, center);
            var f = GetList(new ReferenceEntity(), new DefaultModel(), userST, center);
            var g = Modules(classsb[0].ID, userST, center);
            var lessons = _lessonService.CreateQuery().Find(x => x.ClassSubjectID == classsb[0].ID).ToEnumerable();
            var h = "";
            if (lessons.Count() > 0)
            {
                foreach (var lesson in lessons)
                {
                    //lesson = _lessonService.CreateQuery().Find(x => x.ClassSubjectID == "5f62d9987cd5490b70a7061d").FirstOrDefault();
                    h += GetLesson(lesson.ID, ClassID, classsb[0].ID, userST.ID);
                }
            }

            ViewBag.Msg = $"{a} - {b} - {c} - {d} - {e} - {f} - {g} - {h}";

            return View("Test/Home");
            //}
            //return null;

        }

        //function
        private string getbeatstudent(CenterEntity center)
        {
            try
            {
                var list = new List<StudentRankingViewModel>();

                var classIDs = _classService.CreateQuery().Find(t => t.Center == center.ID).Project(t => t.ID).ToEnumerable();
                var results = _classProgressService.CreateQuery().Aggregate().Match(t => classIDs.Contains(t.ClassID)).Group(t => t.StudentID, g => new StudentRankingViewModel
                {
                    StudentID = g.Key,
                    TotalPoint = g.Sum(t => t.TotalPoint),
                    PracticePoint = g.Sum(t => t.PracticePoint),
                }).SortByDescending(s => s.TotalPoint).ThenByDescending(s => s.PracticePoint).Limit(20).ToEnumerable();

                var rtn = new List<StudentRankingViewModel>();
                foreach (var result in results)
                {
                    var st = _studentService.GetItemByID(result.StudentID);
                    if (st != null)
                    {
                        result.StudentName = st.FullName;
                        rtn.Add(result);
                    }

                }
                return "OK";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        private string GetThisWeekLesson(StudentEntity currentStudent, CenterEntity center, DateTime today)
        {
            try
            {
                var startWeek = today.AddDays(DayOfWeek.Sunday - today.DayOfWeek);
                var endWeek = startWeek.AddDays(7);

                var classids = _classService.GetItemsByIDs(currentStudent.JoinedClasses, center.ID).Select(t => t.ID).ToList();

                var filter = new List<FilterDefinition<LessonScheduleEntity>>();
                //filter.Add(Builders<LessonScheduleEntity>.Filter.Where(o => o.IsActive));
                filter.Add(Builders<LessonScheduleEntity>.Filter.Where(o => o.StartDate <= endWeek && o.EndDate >= startWeek));
                filter.Add(Builders<LessonScheduleEntity>.Filter.Where(o => classids.Contains(o.ClassID)));

                //var csIds = _lessonScheduleService.Collection.Distinct(t => t.ClassSubjectID, Builders<LessonScheduleEntity>.Filter.And(filter)).ToList();

                //var data = _classSubjectService.Collection.Find(t => csIds.Contains(t.ID));

                var data = _scheduleService.Collection.Find(Builders<LessonScheduleEntity>.Filter.And(filter)).ToList();

                var std = (from o in data
                           let _lesson = _lessonService.Collection.Find(t => t.ID == o.LessonID).SingleOrDefault()
                           where _lesson != null
                           let _class = _classService.Collection.Find(t => t.ID == o.ClassID).SingleOrDefault()
                           where _class != null
                           let _cs = _classSubjectService.Collection.Find(t => t.ID == o.ClassSubjectID).SingleOrDefault()
                           where _cs != null
                           let skill = _skillService.GetItemByID(_cs.SkillID)
                           let _subject = _subjectService.Collection.Find(t => t.ID == _cs.SubjectID).SingleOrDefault()
                           where _subject != null
                           let isLearnt = _learningHistoryService.GetLastLearnt(currentStudent.ID, o.LessonID, o.ClassSubjectID) != null
                           let lessonCalendar = _calendarHelper.GetByScheduleId(o.ID)
                           let onlineUrl = (o.IsOnline && lessonCalendar != null) ? lessonCalendar.UrlRoom : ""
                           select new
                           {
                               id = o.ID,
                               classID = _class.ID,
                               className = _class.Name,
                               classSubjectID = _cs.ID,
                               subjectName = _subject.Name,
                               title = _lesson.Title,
                               lessonID = _lesson.ID,
                               startDate = o.StartDate,
                               endDate = o.EndDate,
                               skill = skill,
                               isLearnt = isLearnt,
                               type = _lesson.TemplateType,
                               onlineUrl = o.IsOnline ? onlineUrl : ""
                           }).OrderBy(t => t.startDate).ToList();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private string GetActiveListV2(StudentEntity currentStudent, CenterEntity center, DateTime today)
        {
            try
            {
                var filter = new List<FilterDefinition<ClassEntity>>();
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.IsActive && o.Center == center.ID));
                filter.Add(Builders<ClassEntity>.Filter.Where(o => currentStudent.JoinedClasses.Contains(o.ID)));
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.ClassMechanism != CLASS_MECHANISM.PERSONAL));
                filter.Add(Builders<ClassEntity>.Filter.Where(o => (o.StartDate <= today) && (o.EndDate >= today)));

                var clIDs = (filter.Count > 0 ? _classService.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _classService.GetAll()).Project(t => t.ID).ToList();


                var lstSbj = new List<ClassSubjectEntity>();
                var lstClass = new List<ClassEntity>();
                foreach (var clID in clIDs)
                {
                    lstSbj.AddRange(_classSubjectService.GetByClassID(clID));
                    lstClass.Add(_classService.GetItemByID(clID));
                }

                var std = (from o in lstSbj.ToList()
                           let _class = lstClass.SingleOrDefault(t => t.ID == o.ClassID)
                           let progress = _classSubjectProgressService.GetItemByClassSubjectID(o.ID, currentStudent.ID)
                           let examCount = _scheduleService.CountClassExam(o.ID, end: DateTime.Now)
                           //let skill = _skillService.GetItemByID(o.SkillID)
                           let course = _courseService.GetItemByID(o.CourseID)
                           select new
                           {
                               id = o.ID,
                               //courseID = o.CourseID,
                               courseName = course?.Name,
                               endDate = _class.EndDate,
                               percent = (progress == null || o.TotalLessons == 0) ? 0 : progress.Completed * 100 / o.TotalLessons,
                               max = o.TotalLessons,
                               min = progress != null ? progress.Completed : 0,
                               score = (progress != null && examCount > 0) ? progress.TotalPoint / examCount : 0,
                               thumb = string.IsNullOrEmpty(o.Image) ? course?.Image : o.Image,
                           }).ToList();
                return "OK";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        private string GetFinishList(StudentEntity currentStudent, CenterEntity center, DateTime today)
        {
            try {
                var filter = new List<FilterDefinition<ClassEntity>>();
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.IsActive && o.Center == center.ID));
                filter.Add(Builders<ClassEntity>.Filter.Where(o => currentStudent.JoinedClasses.Contains(o.ID)));

                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.EndDate < today));

                var data = filter.Count > 0 ? _classService.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _classService.GetAll();
                //model.TotalRecord = data.CountDocuments();
                var DataResponse = data == null
                    ? data.ToList()
                    : data.Skip(1 * 30).Limit(30).ToList();

                //var std = (from o in DataResponse
                //           let progress = _classProgressService.GetStudentResult(o.ID, currentStudent.ID)
                //           let per = (progress == null || o.TotalLessons == 0) ? 0 : progress.Completed * 100 / o.TotalLessons
                //           let examCount = _scheduleService.CountClassExam(o.ID)
                //           select new
                //           {
                //               id = o.ID,
                //               //courseID = o.CourseID,
                //               title = o.Name,
                //               endDate = o.EndDate,
                //               per,
                //               max = o.TotalLessons,
                //               min = progress != null ? progress.Completed : 0,
                //               score = (progress != null && examCount > 0) ? progress.TotalPoint / examCount : 0,
                //           }).ToList();
                return "OK";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            }

        private string GetClassSubjects(StudentEntity student, CenterEntity center, string SubjectID = "", string GradeID = "")
        {
            try
            {
                var retClass = new List<ClassEntity>();
                var retClassSbj = new List<ClassSubjectViewModel>();

                var lclass = _classService.GetItemsByIDs(student.JoinedClasses).Where(t => (t.Center == center.ID && t.EndDate >= DateTime.Now) || (t.ClassMechanism == CLASS_MECHANISM.PERSONAL)).OrderBy(t => t.ClassMechanism).ThenByDescending(t => t.StartDate).AsEnumerable();


                foreach (var _class in lclass.ToList())
                {
                    var csbjs = _classSubjectService.GetByClassID(_class.ID).AsEnumerable();
                    if (!string.IsNullOrEmpty(SubjectID))
                        csbjs = csbjs.Where(t => t.SubjectID == SubjectID).AsEnumerable();
                    if (!string.IsNullOrEmpty(GradeID))
                        csbjs = csbjs.Where(t => t.GradeID == GradeID).AsEnumerable();

                    if (csbjs.Count() > 0)
                    {
                        var data = (from r in csbjs
                                    let subject = _subjectService.GetItemByID(r.SubjectID)
                                    let grade = _gradeService.GetItemByID(r.GradeID)
                                    let course = _courseService.GetItemByID(r.CourseID) ?? new CourseEntity()
                                    let skill = r.SkillID == null ? null : _skillService.GetItemByID(r.SkillID)
                                    let teacher = _teacherService.GetItemByID(r.TeacherID)
                                    select new ClassSubjectViewModel
                                    {
                                        ID = r.ID,
                                        SubjectID = r.SubjectID,
                                        SkillID = r.SkillID,
                                        SkillName = skill != null ? skill.Name : "",
                                        SkillImage = string.IsNullOrEmpty(r.Image) ? (!string.IsNullOrEmpty(course.Image) ? course.Image : (skill != null ? skill.Image : "")) : r.Image,
                                        Color = skill != null ? skill.Color : "",
                                        //SubjectName = subject.Name,
                                        SubjectName = r.CourseName,
                                        GradeID = r.GradeID,
                                        GradeName = grade.Name,
                                        CourseID = r.CourseID,
                                        CourseName = string.IsNullOrEmpty(r.CourseName) ? course.Name : r.CourseName,
                                        TeacherID = r.TeacherID,
                                        TeacherName = teacher == null ? "" : teacher.FullName,
                                        TypeClass = r.TypeClass == null ? CLASS_TYPE.STANDARD : r.TypeClass,
                                        ClassName = _class.Name,
                                        ClassID = r.ClassID,
                                        StartDate = _class.StartDate,
                                        EndDate = _class.EndDate
                                    }).ToList();
                        retClassSbj.AddRange(data);
                        retClass.Add(_class);
                    }
                }
                return "OK";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        private string GetList(ReferenceEntity entity, DefaultModel defaultModel, StudentEntity student,CenterEntity center)
        {
            try
            {
                var _mapping = new MappingEntity<CourseEntity, CourseViewModel>();
                var filter = new List<FilterDefinition<ReferenceEntity>>();
                entity.Range = "all";
                filter.Add(Builders<ReferenceEntity>.Filter.Where(o => (o.Range == REF_RANGE.ALL)));
                var result = _referenceService.CreateQuery().Find(Builders<ReferenceEntity>.Filter.And(filter));
                defaultModel.TotalRecord = result.CountDocuments();
                var returnData = result.Skip(defaultModel.PageSize * defaultModel.PageIndex).Limit(defaultModel.PageSize).ToList();

                var _filter = new List<FilterDefinition<CourseEntity>> { Builders<CourseEntity>.Filter.Where(o => o.IsActive == true) };
                _filter = new List<FilterDefinition<CourseEntity>> { Builders<CourseEntity>.Filter.Where(o => o.IsPublic) };
                _filter = new List<FilterDefinition<CourseEntity>> { Builders<CourseEntity>.Filter.Where(o => o.TargetCenters.Contains(center.ID)) };
                _filter = new List<FilterDefinition<CourseEntity>> { Builders<CourseEntity>.Filter.Where(o => o.PublicWStudent == true) };
                var result1 = _courseService.CreateQuery().Find(Builders<CourseEntity>.Filter.And(_filter)).ToList();
                return "OK";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        private string Modules(string id, StudentEntity student, CenterEntity center)
        {
            try
            {
                var currentCs = _classSubjectService.GetItemByID(id);
                //if (currentCs == null)
                //    return Redirect($"/{basis}{Url.Action("Index", "Course")}");
                //var userId = User.Claims.GetClaimByType("UserID").Value;
                if (currentCs == null)
                {
                    currentCs = _classSubjectService.GetItemByID("5f62d9987cd5490b70a7061d");
                }
                var currentClass = _classService.GetItemByID(currentCs.ClassID);
                //if (currentClass == null)
                //    return Redirect($"/{basis}{Url.Action("Index", "Course")}");
                //var classStudent = _classStudentService.GetClassStudent(currentClass.ID, userId);
                //if (classStudent == null)
                //if (!_studentService.IsStudentInClass(currentClass.ID, student.ID))
                //    return Redirect($"/{basis}{Url.Action("Index", "Course")}");
                var progress = _classSubjectProgressService.GetItemByClassSubjectID(id, student.ID);
                //long completed = 0;
                //if (progress != null && progress.TotalLessons > 0)
                //    completed = progress.Completed;
                //var subject = _subjectService.GetItemByID(currentCs.SubjectID);
                //if (subject == null)
                //    return Redirect($"/{basis}{Url.Action("Index", "Course")}");
                //ViewBag.Completed = completed;
                ViewBag.ClassSubject = new ClassSubjectViewModel()
                {
                    ID = currentCs.ID,
                    //Name = subject.Name,
                    CourseID = currentCs.CourseID,
                    ClassID = currentClass.ID,
                    ClassName = currentClass.Name,
                    CourseName = string.IsNullOrEmpty(currentCs.CourseName) ? _courseService.GetItemByID(currentCs.CourseID)?.Name : currentCs.CourseName,
                    //SkillName = _skillService.GetItemByID(currentCs.SkillID).Name,
                    CompletedLesssons = progress == null ? 0 : progress.Completed,
                    TotalLessons = currentCs.TotalLessons,
                };
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private string GetLesson(string LessonID, string ClassID, string ClassSubjectID,string userId)
        {
            try
            {
                //var userId = User.Claims.GetClaimByType("UserID").Value;
                //if (string.IsNullOrEmpty(userId))
                //    return new JsonResult(
                //    new Dictionary<string, object> { { "Error", "Student not found" } });

                var lesson = _lessonService.GetItemByID(LessonID);
                //if (lesson == null)
                //    return new JsonResult(
                //    new Dictionary<string, object> { { "Error", "Lesson not found" } });

                if (string.IsNullOrEmpty(ClassSubjectID))
                    ClassSubjectID = ClassID;

                var currentcs = _classSubjectService.GetItemByID(ClassSubjectID);
                //if (currentcs == null)
                //    return new JsonResult(
                //    new Dictionary<string, object> { { "Error", "Subject not found" } });

                //if (string.IsNullOrEmpty(ClassID))
                //    ClassID = currentcs.ClassID;

                var currentClass = _classService.GetItemByID(currentcs.ClassID);
                //if (currentClass == null)
                //    return new JsonResult(
                //    new Dictionary<string, object> { { "Error", "Class not found" } });


                //Create learning history
                _ = _progressHelper.CreateHist(new LearningHistoryEntity()
                {
                    ClassID = ClassID,
                    ClassSubjectID = ClassSubjectID,
                    LessonID = LessonID,
                    ChapterID = lesson.ChapterID,
                    Time = DateTime.Now,
                    StudentID = userId
                });

                var listParts = _cloneLessonPartService.CreateQuery().Find(o => o.ParentID == lesson.ID && o.ClassSubjectID == currentcs.ID).ToList();

                var mapping = new MappingEntity<LessonEntity, StudentLessonViewModel>();
                var mapPart = new MappingEntity<CloneLessonPartEntity, PartViewModel>();
                var mapQuestion = new MappingEntity<CloneLessonPartQuestionEntity, QuestionViewModel>();



                var result = new List<PartViewModel>();
                foreach (var part in listParts)
                {
                    var convertedPart = mapPart.AutoOrtherType(part, new PartViewModel());
                    switch (part.Type)
                    {
                        case "QUIZ1":
                        case "QUIZ3":
                        case "QUIZ4":
                        case "ESSAY":
                            convertedPart.Questions = _cloneLessonPartQuestionService.CreateQuery()
                                .Find(q => q.ParentID == part.ID).SortBy(q => q.Order).ThenBy(q => q.ID).ToList()
                                .Select(q => new QuestionViewModel(q)
                                {
                                    CloneAnswers = _cloneLessonPartAnswerService.CreateQuery().Find(x => x.ParentID == q.ID).ToList(),
                                    Description = q.Description
                                }).ToList();
                            break;
                        case "QUIZ2":
                            convertedPart.Questions = _cloneLessonPartQuestionService.CreateQuery().Find(q => q.ParentID == part.ID)
                                //.SortBy(q => q.Order).ThenBy(q => q.ID)
                                .ToList()
                                .Select(q => new QuestionViewModel(q)
                                {
                                    CloneAnswers = null,
                                    Description = null
                                }).ToList();
                            break;
                        case "VOCAB":
                            //convertedPart.Description = RenderVocab(part.Description);
                            RenderVocab(part.Description);
                            break;
                        default:
                            break;
                    }
                    result.Add(convertedPart);
                }

                var dataResponse = mapping.AutoOrtherType(lesson, new StudentLessonViewModel()
                {
                    Part = result
                    //listParts.Select(o => mapPart.AutoOrtherType(o, new PartViewModel()
                    //{
                    //    Questions = _cloneLessonPartQuestionService.CreateQuery().Find(x => x.ParentID == o.ID).ToList()
                    //        .Select(z => mapQuestion.AutoOrtherType(z, new QuestionViewModel()
                    //        {
                    //            CloneAnswers = o.Type == "QUIZ2" ? null : _cloneLessonPartAnswerService.CreateQuery().Find(x => x.ParentID == z.ID).ToList(),
                    //            Description = o.Type == "QUIZ2" ? null : z.Description
                    //        }))?.ToList()
                    //})).ToList()
                });

                var lastexam = _examService.CreateQuery().Find(o => o.LessonID == LessonID && o.ClassSubjectID == ClassSubjectID
                    //&& o.ClassID == ClassID 
                    && o.StudentID == userId).SortByDescending(o => o.Created).FirstOrDefault();

                if (lastexam == null)
                {
                    var response = new Dictionary<string, object> { { "Data", dataResponse } };
                    //return new JsonResult(response);
                }
                else //TODO: Double check here
                {
                    var currentTimespan = new TimeSpan(0, 0, lesson.Timer, 0);

                    if (!lastexam.Status && lesson.Timer > 0) //bài kt cũ chưa xong => check thời gian làm bài
                    {
                        var endtime = (lastexam.Created.AddMinutes(lastexam.Timer));
                        if (endtime < DateTime.UtcNow) // hết thời gian 
                        {
                            //lastexam = _examService.CompleteNoEssay(lastexam, lesson, out _);
                        }
                    }

                    var timeSpan = lastexam.Status ? new TimeSpan(0, 0, lesson.Timer, 0) : (lastexam.Created.AddMinutes(lastexam.Timer) - DateTime.UtcNow);
                }
                return "OK";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        private string RenderVocab(string description)
        {
            try
            {
                string result = "";
                var vocabs = description.Split('|');
                //if (vocabs == null || vocabs.Count() == 0)
                //    return description;
                foreach (var vocab in vocabs)
                {
                    var vocabularies = _vocabularyService.GetItemByCode(vocab.Trim().Replace("-", ""));
                }
                return "OK";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            //eturn result;
        }
        #endregion

        #region Mail Report Monthly
        private string GetBase64FromJavaScriptImage(string javaScriptBase64String)
        {
            return Regex.Match(javaScriptBase64String, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
        }
        [HttpPost]
        public async Task<JsonResult> SendMonthlyReport(string[] data,string image)
        {
            try
            {
                //var dateTime = DateTime.Now;
                var dateTime = new DateTime(2020, 11, 01);
                var day = dateTime.Day;
                var month = dateTime.Month;
                var year = dateTime.Year;

                var currentTime = new DateTime(year, month, day, 0, 0, 0);
                var startMonth = currentTime.AddMonths(-1);
                var endMonth = currentTime.AddDays(-1).AddHours(23).AddMinutes(59);

                //var centersActive = _centerService.GetActiveCenter(currentTime);//lay co so dang hoat dong trong thang
                //foreach(var center in centersActive)
                //{
                //    if (center.Abbr == "c3vyvp")//test truong Vinh Yen
                //    {
                //        var listTeacherHeader = _teacherService.CreateQuery().Find(x => x.IsActive == true && x.Centers.Any(y => y.CenterID == center.ID)).ToList().FindAll(y => HasRole(y.ID, center.ID, "head-teacher")).Select(x => x.Email).ToList();
                //        if (listTeacherHeader.Contains("huonghl@utc.edu.vn"))
                //        {
                //            listTeacherHeader.Remove("huonghl@utc.edu.vn");
                //        }
                string base64 = GetBase64FromJavaScriptImage(image);
                var bytes = Convert.FromBase64String(base64);
                string link = "";
                using (var memory = new MemoryStream(bytes))
                {

                    link = Program.GoogleDriveApiService.CreateLinkViewFile(_roxyFilemanHandler.UploadFileWithGoogleDrive("eduso", "admin", memory));
                }

                var body = $"<div><img src='{link}' /></div>";
                var subject = "div tesst";
                //var isTest = true;
                //var toAddress = isTest == true ? new List<string> { "nguyenvanhoa2017602593@gmail.com", "vietphung.it@gmail.com" } : listTeacherHeader;
                //var bccAddress = isTest == true ? null : new List<string> { "nguyenhoa.dev@gmail.com", "vietphung.it@gmail.com", "huonghl@utc.edu.vn", "buihong9885@gmail.com" };
                //_ = await _mailHelper.SendBaseEmail(toAddress, subject, body, MailPhase.WEEKLY_SCHEDULE, null, bccAddress);
                var isTest = true;
                var toAddress = isTest == true ? new List<string> { "nguyenvanhoa2017602593@gmail.com", "vietphung.it@gmail.com" } : new List<string> { "shin.l0v3.ly@gmail.com" };
                try
                {
                    _ = await _mailHelper.SendBaseEmail(toAddress, subject, body, MailPhase.WEEKLY_SCHEDULE, null, toAddress);
                    return Json("ok");
                }
                catch (Exception ex)
                {
                    return Json(ex.Message);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public IActionResult EmailTemp()
        {
            var currentTime = DateTime.Now;
            if (currentTime == null || currentTime <= DateTime.MinValue)
            {
                currentTime = DateTime.Now;
            }
            IEnumerable<CenterEntity> centersActive = _centerService.GetActiveCenter(currentTime);//lay co so dang hoat dong
            Dictionary<string, Dictionary<int, string[]>> dataClass = new Dictionary<string, Dictionary<int, string[]>>();
            Dictionary<string, string> classCenter = new Dictionary<string, string>();
            for (int i = 0; centersActive != null && i < centersActive.Count(); i++)
            {
                var center = centersActive.ElementAt(i);
                var classesActive = _classService.GetActiveClass(currentTime, center.ID);
                if (classesActive != null)
                {
                    foreach (var _class in classesActive)
                    {
                        classCenter.Add(_class.ID, center.ID);
                        var data = GetDataForReprot(_class, currentTime);
                        dataClass.Add(_class.ID, data);
                    }
                }
            }

            ViewBag.Centers = classCenter;
            ViewBag.Data = dataClass;
            return View();
        }

        private Dictionary<int, string[]> GetDataForReprot(ClassEntity @class, DateTime dateTime)
        {
            //[] => {}
            var dataResponse = new Dictionary<int, string[]>();

            //var dateTime = DateTime.Now;
            var day = dateTime.Day;
            var month = dateTime.Month;
            var year = dateTime.Year;

            var currentTime = new DateTime(year, month, day, 0, 0, 0).AddMonths(-1);//Lùi 1 tháng
            var sw1 = currentTime;//Tuan 1
            var ew1 = currentTime.AddDays(6).AddHours(23).AddMinutes(59);

            var sw2 = ew1.AddMinutes(1);//Tuan 2
            var ew2 = sw2.AddDays(6).AddHours(23).AddMinutes(59);

            var sw3 = ew2.AddMinutes(1);//Tuan 3
            var ew3 = sw3.AddDays(6).AddHours(23).AddMinutes(59);

            var sw4 = ew3.AddMinutes(1);//Tuan 4
            var ew4 = new DateTime(year, month, day, 0, 0, 0).AddDays(-1);//Lui 1 ngay

            var listDateTime = new List<dateTime>
            {
                new dateTime(sw1, ew1),
                new dateTime(sw2, ew2),
                new dateTime(sw3, ew3),
                new dateTime(sw4, ew4)
            };

            var key = 0;
            foreach (var item in listDateTime)
            {
                var data = GetDataInWeek(item.StartWeek, item.EndWeek, @class);
                dataResponse.Add(key, data);
                key++;
            };

            return dataResponse;
        }
        /// <summary>
        /// //classStudent.ToString(),stChuaVaoLop.ToString(),min8.ToString(),min5.ToString(),min2.ToString(),min0.ToString(),chualam.ToString()
        /// </summary>
        /// <param name="startWeek"></param>
        /// <param name="endWeek"></param>
        /// <param name="class"></param>
        /// <returns></returns>
        private string[] GetDataInWeek(DateTime startWeek, DateTime endWeek, ClassEntity @class)
        {
            //data can lay
            var min8 = 9;
            var min5 = 0;
            var min2 = 0;
            var min0 = 0;
            var chualam = 0;

            //Lay danh sach ID hoc sinh trong lop
            var studentIds = _studentService.GetStudentsByClassId(@class.ID).Select(t => t.ID).ToList();
            var classStudent = studentIds.Count();

            var activeSchedules = _lessonScheduleService.CreateQuery().Find(o => o.ClassID == @class.ID && o.StartDate <= endWeek && o.EndDate >= startWeek)?.Project(t => new LessonScheduleEntity
            {
                LessonID = t.LessonID,
                ClassSubjectID = t.ClassSubjectID,
                StartDate = t.StartDate,
                EndDate = t.EndDate
            })?.ToList();
            var activeLessonIds = activeSchedules?.Select(t => t.LessonID)?.ToList();

            //Lay danh sach hoc sinh da hoc cac bai tren trong tuan
            var activeProgress = _lessonProgressService.CreateQuery().Find(
                x => studentIds.Contains(x.StudentID) && activeLessonIds.Contains(x.LessonID)
                && x.LastDate <= endWeek && x.LastDate >= startWeek).ToEnumerable();
            var activeStudents = activeProgress.Select(t => t.StudentID).Distinct();
            var stChuaVaoLop = classStudent - activeStudents.Count();

            var examIds = _lessonService.CreateQuery().Find(x => (x.TemplateType == 2 || x.IsPractice == true) && activeLessonIds.Contains(x.ID)).Project(x => x.ID).ToList();
            if (examIds.Count > 0) //co bai kiem tra
            {
                //ket qua lam bai cua hoc sinh trong lop
                var classResult = (from r in activeProgress.Where(t => examIds.Contains(t.LessonID) && t.Tried > 0)
                                   group r by r.StudentID
                                   into g
                                   select new StudentResult
                                   {
                                       StudentID = g.Key,
                                       ExamCount = g.Count(),
                                       AvgPoint = g.Sum(t => t.LastPoint) / examIds.Count
                                   }).ToList();
                //render ket qua hoc tap
                min8 = classResult.Count(t => t.AvgPoint >= 80);
                min5 = classResult.Count(t => t.AvgPoint >= 50 && t.AvgPoint < 80);
                min2 = classResult.Count(t => t.AvgPoint >= 20 && t.AvgPoint < 50);
                min0 = classResult.Count(t => t.AvgPoint >= 0 && t.AvgPoint < 20);
                chualam = classStudent - (min0 + min2 + min5 + min8);
            }

            string[] data = { classStudent.ToString(), stChuaVaoLop.ToString(), min8.ToString(), min5.ToString(), min2.ToString(), min0.ToString(), chualam.ToString() };
            return data;
        }
        #endregion
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

    public class StudentResult
    {
        public string StudentID { get; set; }
        public long ExamCount { get; set; }
        public double AvgPoint { get; set; }
    }
    public class dateTime
    {
        public DateTime StartWeek { get; set; }
        public DateTime EndWeek { get; set; }

        public dateTime(DateTime sw, DateTime ew)
        {
            this.StartWeek = sw;
            this.EndWeek = ew;
        }
    }
}
