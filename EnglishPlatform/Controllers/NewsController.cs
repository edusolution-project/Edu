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
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using Newtonsoft.Json;
using System.Diagnostics;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace EnglishPlatform.Controllers
{
    public class NewsController : Controller
    {
        private readonly NewsService _newsService;
        private readonly NewsCategoryService _newsCategoryService;

        public NewsController(
            NewsService newsService,
            NewsCategoryService newsCategoryService
            )
        {
            _newsService = newsService;
            _newsCategoryService = newsCategoryService;
        }
        //[Route("tin-tuc")]
        public IActionResult Index()
        {
            ViewBag.newsTop = _newsService.Collection.Find(tbl => tbl.IsTop == true && tbl.IsActive == true).SortByDescending(tbl => tbl.PublishDate).Limit(5).ToList();
            ViewBag.newsHot = _newsService.Collection.Find(tbl => tbl.IsHot == true && tbl.IsActive == true).SortByDescending(tbl => tbl.PublishDate).Limit(2).ToList();
            return View();
        }

        public IActionResult Category(DefaultModel model, string catcode)
        {
            model.PageSize = 12;
            var cat = _newsCategoryService.GetItemByCode(catcode);
            ViewBag.Category = cat;
            if (cat == null)
                return RedirectToRoute("news-default");

            var filter = new List<FilterDefinition<NewsEntity>>();
            filter.Add(Builders<NewsEntity>.Filter.Where(t => t.CategoryID == cat.ID));
            filter.Add(Builders<NewsEntity>.Filter.Lte(t => t.PublishDate, DateTime.Now));
            filter.Add(Builders<NewsEntity>.Filter.Where(t => t.IsActive == true));

            var data = _newsService.Collection.Find(Builders<NewsEntity>.Filter.And(filter))
                .SortByDescending(t => t.PublishDate);
            ViewBag.TotalRec = data.CountDocuments();
            model.TotalRecord = ViewBag.TotalRec;
            ViewBag.FirstPage = data.Any() == true ? data.Limit(model.PageSize).ToList() : _newsService.GetAll().ToList();
            //ViewBag.Data = _newsService.Collection.Find(tbl => tbl.CategoryID.Equals(cat.ID)).ToList();
            if (catcode.Equals("ve-eduso"))
            {
                return View("AboutEduso");
            }
            else
            {
                return View();
            }
        }

        public IActionResult Detail(string catcode, string newscode)
        {
            var cat = _newsCategoryService.GetItemByCode(catcode);
            ViewBag.Category = cat;
            var filter = new List<FilterDefinition<NewsEntity>>();

            if (!string.IsNullOrEmpty(newscode))
            {
                filter.Add(Builders<NewsEntity>.Filter.Where(t => t.ID == newscode));
            }
            filter.Add(Builders<NewsEntity>.Filter.Where(t => t.Type == "news" || t.Type == null));

            var news = _newsService.CreateQuery().Find(Builders<NewsEntity>.Filter.And(filter)).FirstOrDefault();
            ViewBag.News = news;
            if (news == null || !news.IsActive)
            {
                if (cat == null)
                    return RedirectToRoute("news-default");
                else
                    return RedirectToRoute("news-category", new { catcode = cat.Code });
            }
            return View();
        }

        [HttpPost]
        [Route("/news/getlist")]
        public JsonResult GetList(DefaultModel model, string catID, bool isHot = false, bool isTop = false)
        {
            var filter = new List<FilterDefinition<NewsEntity>>();

            if (!string.IsNullOrEmpty(catID))
            {
                filter.Add(Builders<NewsEntity>.Filter.Where(t => t.CategoryID == catID));
            }
            if (isHot)
            {
                filter.Add(Builders<NewsEntity>.Filter.Where(t => t.IsHot));
            }
            if (isTop)
            {
                filter.Add(Builders<NewsEntity>.Filter.Where(t => t.IsTop));
            }
            filter.Add(Builders<NewsEntity>.Filter.Lte(t => t.PublishDate, DateTime.Now));
            filter.Add(Builders<NewsEntity>.Filter.Where(t => t.IsActive == true));
            filter.Add(Builders<NewsEntity>.Filter.Where(t => t.Type == "news" || t.Type==null));

            var data = (filter.Count > 0 ? _newsService.Collection.Find(Builders<NewsEntity>.Filter.And(filter)) : _newsService.GetAll())
                .SortByDescending(t => t.PublishDate);
            model.TotalRecord = data.CountDocuments();
            var DataResponse = data == null || model.TotalRecord <= 0 || model.TotalRecord <= model.PageSize
                ? data.ToList()
                : data.Skip((model.PageIndex) * model.PageSize).Limit(model.PageSize).ToList();

            var response = new Dictionary<string, object>
            {
                { "Data", DataResponse },
                { "Model", model }
            };
            return new JsonResult(response);
        }

        public IActionResult DetailProduct(string code)
        {
            var detail = _newsService.CreateQuery().Find(o => o.Code.Equals(code) && o.Type == "san-pham").FirstOrDefault();
            ViewBag.detail = detail;
            return View();
        }

        //[Route("/login")]
        //public IActionResult Login()
        //{
        //    long limit = 0;
        //    long count = _accountService.CreateQuery().CountDocuments(_ => true);
        //    if (count <= limit)
        //    {
        //        startPage();
        //    }
        //    return View();
        //}

        //[HttpPost]
        //public async Task<JsonResult> LoginAPI(string UserName, string PassWord, string Type, bool IsRemmember)
        //{
        //    //var x = HttpContext.Request;
        //    //_log.Debug("login", new { UserName, PassWord, Type, IsRemmember });
        //    try
        //    {
        //        var _username = UserName.Trim().ToLower();
        //        if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(PassWord))
        //        {
        //            return Json(new ReturnJsonModel
        //            {
        //                StatusCode = ReturnStatus.ERROR,
        //                StatusDesc = "Vui lòng điền đầy đủ thông tin",
        //            });
        //        }
        //        else
        //        {
        //            string _sPass = Core_v2.Globals.Security.Encrypt(PassWord);
        //            var user = _accountService.GetAccount(_username, _sPass);
        //            if (user == null)
        //            {
        //                return Json(new ReturnJsonModel
        //                {
        //                    StatusCode = ReturnStatus.ERROR,
        //                    StatusDesc = "Thông tin tài khoản không đúng"
        //                });
        //            }
        //            else
        //            {
        //                if (user.IsActive)
        //                {
        //                    TempData["success"] = "Hi " + _username;
        //                    string _token = Guid.NewGuid().ToString();
        //                    HttpContext.SetValue(Cookies.DefaultLogin, _token, Cookies.ExpiresLogin, false);

        //                    var center = _centerService.GetDefault();
        //                    string centerCode = center.Code;
        //                    string roleCode = "";
        //                    var tc = _teacherService.GetItemByID(user.UserID);
        //                    var st = _studentService.GetItemByID(user.UserID);

        //                    var defaultUser = new UserModel() { };
        //                    switch (Type)
        //                    {
        //                        case ACCOUNT_TYPE.TEACHER:
        //                            if (tc != null)
        //                            {
        //                                defaultUser = new UserModel(tc.ID, tc.FullName);
        //                                centerCode = tc.Centers != null && tc.Centers.Count > 0 ? tc.Centers.FirstOrDefault().Code : center.Code;
        //                                roleCode = tc.Centers != null && tc.Centers.Count > 0 ? tc.Centers.FirstOrDefault().RoleID : "";
        //                            }
        //                            break;
        //                        case ACCOUNT_TYPE.ADMIN:
        //                            defaultUser = new UserModel(user.ID, "admin");
        //                            centerCode = center.Code;
        //                            roleCode = user.UserName == "supperadmin@gmail.com" ? "superadmin" : "admin";
        //                            break;
        //                        default:
        //                            if (st != null)
        //                            {
        //                                defaultUser = new UserModel(st.ID, st.FullName);
        //                                centerCode = (st.Centers != null && st.Centers.Count > 0) ? _centerService.GetItemByID(st.Centers.FirstOrDefault()).Code : center.Code;
        //                                roleCode = "student";
        //                            }
        //                            break;
        //                    }
        //                    if (Type != ACCOUNT_TYPE.ADMIN)
        //                    {
        //                        var role = roleCode != "student" ? _roleService.GetItemByID(roleCode) : _roleService.GetItemByCode(roleCode);
        //                        if (role != null)
        //                        {
        //                            var listAccess = _accessesService.GetAccessByRole(role.Code);
        //                            string key = $"{roleCode}";
        //                            CacheExtends.SetObjectFromCache($"{defaultUser.ID}_{centerCode}", 3600 * 24 * 360, key);
        //                            if (CacheExtends.GetDataFromCache<List<string>>(key) == null)
        //                            {
        //                                var access = listAccess.Select(o => o.Authority)?.ToList();
        //                                CacheExtends.SetObjectFromCache(key, 3600 * 24 * 360, access);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            return Json(new ReturnJsonModel
        //                            {
        //                                StatusCode = ReturnStatus.ERROR,
        //                                StatusDesc = "Tài khoản đang bị khóa. Vui lòng liên hệ với quản trị viên để được hỗ trợ"
        //                            });
        //                        }
        //                    }

        //                    //cache


        //                    var claims = new List<Claim>{
        //                        new Claim("UserID",defaultUser.ID),
        //                        new Claim(ClaimTypes.Email, _username),
        //                        new Claim(ClaimTypes.Name, defaultUser.Name),
        //                        new Claim(ClaimTypes.Role,roleCode),
        //                        new Claim("Type", Type)};


        //                    var claimsIdentity = new ClaimsIdentity(claims, Cookies.DefaultLogin);
        //                    _ = new AuthenticationProperties
        //                    {
        //                        IsPersistent = true,
        //                        ExpiresUtc = DateTime.UtcNow.AddMinutes(Cookies.ExpiresLogin)
        //                    };
        //                    ClaimsPrincipal claim = new ClaimsPrincipal(claimsIdentity);

        //                    AccountLogEntity login = new AccountLogEntity()
        //                    {
        //                        IP = HttpContext.Connection.RemoteIpAddress.ToString(),
        //                        AccountID = user.ID,
        //                        Token = _token,
        //                        IsRemember = IsRemmember,
        //                        CreateDate = DateTime.Now
        //                    };
        //                    _logService.CreateQuery().InsertOne(login);
        //                    await _authenService.SignIn(HttpContext, claim, Cookies.DefaultLogin);
        //                    return Json(new ReturnJsonModel
        //                    {
        //                        StatusCode = ReturnStatus.SUCCESS,
        //                        StatusDesc = "OK",
        //                        Location = $"{centerCode}/{Type}"
        //                    });
        //                }
        //                else
        //                {
        //                    return Json(new ReturnJsonModel
        //                    {
        //                        StatusCode = ReturnStatus.ERROR,
        //                        StatusDesc = "Tài khoản đang bị khóa. Vui lòng liên hệ với quản trị viên để được hỗ trợ"
        //                    });
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public JsonResult RegisterAPI(string UserName, string Name, string Phone, string PassWord, string Type)
        //{
        //    var _username = UserName.Trim().ToLower();
        //    if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(PassWord))
        //    {
        //        return Json(new ReturnJsonModel
        //        {
        //            StatusCode = ReturnStatus.ERROR,
        //            StatusDesc = "Please fill your information",
        //        });
        //    }
        //    else
        //    {
        //        string _sPass = Core_v2.Globals.Security.Encrypt(PassWord);
        //        if (_accountService.IsAvailable(_username))
        //        {
        //            return Json(new ReturnJsonModel
        //            {
        //                StatusCode = ReturnStatus.ERROR,
        //                StatusDesc = "Account exist",
        //            });
        //        }
        //        else
        //        {
        //            var user = new AccountEntity()
        //            {
        //                PassWord = _sPass,
        //                UserName = _username,
        //                Name = Name ?? _username,
        //                Phone = Phone,
        //                Type = Type,
        //                IsActive = false,
        //                CreateDate = DateTime.Now,
        //                UserCreate = null,
        //            };
        //            switch (Type)
        //            {
        //                case ACCOUNT_TYPE.TEACHER:
        //                    user.RoleID = _roleService.GetItemByCode("teacher").ID;
        //                    break;
        //                default:
        //                    user.IsActive = true;// active student
        //                    user.RoleID = _roleService.GetItemByCode("student").ID;
        //                    break;
        //            }
        //            _accountService.CreateQuery().InsertOne(user);
        //            switch (Type)
        //            {
        //                case ACCOUNT_TYPE.TEACHER:
        //                    //create teacher
        //                    var teacher = new TeacherEntity()
        //                    {
        //                        FullName = user.Name,
        //                        Email = _username,
        //                        Phone = user.Phone,
        //                        IsActive = false,
        //                        CreateDate = DateTime.Now
        //                    };
        //                    _teacherService.CreateQuery().InsertOne(teacher);
        //                    user.UserID = teacher.ID;
        //                    //send email for teacher
        //                    break;
        //                default:
        //                    //create student
        //                    var student = new StudentEntity()
        //                    {
        //                        FullName = user.Name,
        //                        Email = _username,
        //                        Phone = user.Phone,
        //                        IsActive = true,// active student
        //                        CreateDate = DateTime.Now
        //                    };

        //                    _studentService.CreateQuery().InsertOne(student);
        //                    user.UserID = student.ID;
        //                    //send email for student
        //                    //add default class
        //                    var testClassIDs = _default.defaultClassID;
        //                    if (!string.IsNullOrEmpty(testClassIDs))
        //                    {
        //                        foreach (var id in testClassIDs.Split(';'))
        //                            if (!string.IsNullOrEmpty(id))
        //                                _classService.AddStudentToClass(id, student.ID);
        //                    }
        //                    break;
        //            }
        //            var filter = Builders<AccountEntity>.Filter.Where(o => o.ID == user.ID);
        //            _accountService.CreateQuery().ReplaceOne(filter, user);
        //            ViewBag.Data = user;
        //            _ = _mailHelper.SendRegisterEmail(user, PassWord);
        //            return Json(new ReturnJsonModel
        //            {
        //                StatusCode = ReturnStatus.SUCCESS,
        //                StatusDesc = "Tài khoản đã được tạo, mời bạn đăng nhập để trải nghiệm ngay!",
        //                Location = "/Login"
        //            });
        //        }
        //    }
        //}


        //private void StartAuthority()
        //{
        //    if (CacheExtends.GetDataFromCache<List<AuthorityEntity>>(CacheExtends.DefaultPermission) == null)
        //    {
        //        List<AuthorityEntity> data = _authorityService.GetAll()?.ToList();
        //        CacheExtends.SetObjectFromCache(CacheExtends.DefaultPermission, 3600 * 24 * 360, data);
        //    }
        //}

        //private void startPage()
        //{
        //    var superadminRole = new RoleEntity()
        //    {
        //        Name = "Super Admin",
        //        Code = "superadmin",
        //        Type = ACCOUNT_TYPE.ADMIN,
        //        CreateDate = DateTime.Now,
        //        IsActive = true,
        //        UserCreate = "longht"
        //    };
        //    var adminRole = new RoleEntity()
        //    {
        //        Name = "Admin",
        //        Code = "admin",
        //        Type = ACCOUNT_TYPE.ADMIN,
        //        CreateDate = DateTime.Now,
        //        IsActive = true,
        //        UserCreate = "longht"
        //    };
        //    var headteacherRole = new RoleEntity()
        //    {
        //        Name = "Trưởng bộ môn",
        //        Code = "head-teacher",
        //        Type = ACCOUNT_TYPE.TEACHER,
        //        CreateDate = DateTime.Now,
        //        IsActive = true,
        //        UserCreate = "longht"
        //    };
        //    var teacherrole = new RoleEntity()
        //    {
        //        Name = "Giáo viên",
        //        Code = "teacher",
        //        Type = ACCOUNT_TYPE.TEACHER,
        //        CreateDate = DateTime.Now,
        //        IsActive = true,
        //        UserCreate = "longht"
        //    };
        //    var studentRole = new RoleEntity()
        //    {
        //        Name = "Học viên",
        //        Code = "student",
        //        Type = ACCOUNT_TYPE.STUDENT,
        //        CreateDate = DateTime.Now,
        //        IsActive = true,
        //        UserCreate = "longht"
        //    };
        //    _roleService.CreateQuery().InsertOne(headteacherRole);
        //    _roleService.CreateQuery().InsertOne(studentRole);
        //    _roleService.CreateQuery().InsertOne(teacherrole);
        //    _roleService.CreateQuery().InsertOne(superadminRole);
        //    _roleService.CreateQuery().InsertOne(adminRole);
        //    var superadmin = new AccountEntity()
        //    {
        //        CreateDate = DateTime.Now,
        //        IsActive = true,
        //        Type = ACCOUNT_TYPE.ADMIN,
        //        UserName = "supperadmin@gmail.com",
        //        PassTemp = Core_v2.Globals.Security.Encrypt("123"),
        //        PassWord = Core_v2.Globals.Security.Encrypt("123"),
        //        UserID = "0", // admin
        //        RoleID = superadminRole.ID
        //    };
        //    _accountService.CreateQuery().InsertOne(superadmin);
        //}

        //[Route("/logout")]
        //public async Task<IActionResult> LogOut()
        //{
        //    var basis = HttpContext.Request;
        //    string keys = User.FindFirst("UserID")?.Value;
        //    if (!string.IsNullOrEmpty(keys))
        //    {
        //        CacheExtends.ClearCache(keys);
        //    }
        //    await HttpContext.SignOutAsync(Cookies.DefaultLogin);
        //    HttpContext.Remove(Cookies.DefaultLogin);

        //    return RedirectToAction("Login");
        //}

        //[Route("/forgot-password")]
        //public IActionResult ForgotPassword()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[Route("/forgot-password")]
        //[ValidateAntiForgeryToken]
        //public IActionResult ForgotPassword(string Email)
        //{
        //    return View();
        //}


        //[HttpPost]
        //public IActionResult UploadImage(IFormFile upload, string CKEditorFuncNum, string CKEditor, string langCode)
        //{
        //    if (upload.Length <= 0) return null;
        //    //if (!upload.IsImage())
        //    //{
        //    //    var NotImageMessage = "please choose a picture";
        //    //    dynamic NotImage = JsonConvert.DeserializeObject("{ 'uploaded': 0, 'error': { 'message': \"" + NotImageMessage + "\"}}");
        //    //    return Json(NotImage);
        //    //}

        //    var fileName = Guid.NewGuid() + Path.GetExtension(upload.FileName).ToLower();

        //    //Image image = .FromStream(upload.OpenReadStream());
        //    //int width = image.Width;
        //    //int height = image.Height;
        //    //if ((width > 750) || (height > 500))
        //    //{
        //    //    var DimensionErrorMessage = "Custom Message for error"
        //    //    dynamic stuff = JsonConvert.DeserializeObject("{ 'uploaded': 0, 'error': { 'message': \"" + DimensionErrorMessage + "\"}}");
        //    //    return Json(stuff);
        //    //}

        //    //if (upload.Length > 500 * 1024)
        //    //{
        //    //    var LengthErrorMessage = "Custom Message for error";
        //    //    dynamic stuff = JsonConvert.DeserializeObject("{ 'uploaded': 0, 'error': { 'message': \"" + LengthErrorMessage + "\"}}");
        //    //    return Json(stuff);
        //    //}

        //    var path = Path.Combine(
        //        Directory.GetCurrentDirectory(), "wwwroot/images/CKEditorImages",
        //        fileName);

        //    //using (var stream = new FileStream(path, FileMode.Create))
        //    //{
        //    //    upload.CopyTo(stream);
        //    //}

        //    var standardSize = new SixLabors.Primitives.Size(1024, 768);

        //    using (Stream inputStream = upload.OpenReadStream())
        //    {
        //        using (var image = Image.Load<Rgba32>(inputStream))
        //        {
        //            var imageEncoder = new JpegEncoder()
        //            {
        //                Quality = 90,
        //                Subsample = JpegSubsample.Ratio444
        //            };

        //            int width = image.Width;
        //            int height = image.Height;
        //            if ((width > standardSize.Width) || (height > standardSize.Height))
        //            {
        //                ResizeOptions options = new ResizeOptions
        //                {
        //                    Mode = ResizeMode.Max,
        //                    Size = standardSize,
        //                };
        //                image.Mutate(x => x
        //                 .Resize(options));

        //                //.Grayscale());
        //            }
        //            image.Save(path, imageEncoder); // Automatic encoder selected based on extension.
        //        }
        //    }

        //    var url = $"{"/images/CKEditorImages/"}{fileName}";
        //    var successMessage = "image is uploaded successfully";
        //    dynamic success = JsonConvert.DeserializeObject("{ 'uploaded': 1,'fileName': \"" + fileName + "\",'url': \"" + url + "\", 'error': { 'message': \"" + successMessage + "\"}}");
        //    return Json(success);
        //}

        //public IActionResult OnlineClass(string ID)
        //{
        //    var @event = _calendarHelper.GetByEventID(ID);
        //    if (@event != null)
        //    {
        //        //@event.UrlRoom = "6725744943";

        //        var UserID = User.Claims.GetClaimByType("UserID").Value;
        //        var type = User.Claims.GetClaimByType("Type").Value;
        //        if (type == "teacher")
        //        {
        //            //ViewBag.Role = "1";
        //            var teacher = _teacherService.GetItemByID(UserID);
        //            //if (!string.IsNullOrEmpty(teacher.ZoomID))
        //            if (teacher.ZoomID == @event.UrlRoom)
        //            {
        //                //var roomID = "6725744943";//test
        //                ViewBag.URL = "https://zoom.us/wc/" + teacher.ZoomID.Replace("-", "") + "/join";
        //                //ViewBag.URL = Url.Action("ZoomClass", "Home", new { roomID });
        //            }
        //            else
        //                ViewBag.URL = @event.UrlRoom;
        //        }
        //        else
        //        {
        //            //ViewBag.Role = "0";
        //            ViewBag.Url = Url.Action("ZoomClass", "Home", new { roomID = @event.UrlRoom.Replace("-", "") });
        //        }
        //    }
        //    return View();
        //}

        //public IActionResult ZoomClass(string roomID)
        //{
        //    ViewBag.RoomID = roomID;
        //    return View();
        //}
    }

}