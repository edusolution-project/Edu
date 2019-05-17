using CoreLogs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using BasePublisherMVC.Globals;
using BasePublisherMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BasePublisherModels.Database;
using CoreMongoDB.Repositories;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace BasePublisherMVC.AdminControllers
{
    public class CPApiController : Controller
    {
        protected const string regexElement = @"<\s*static-layout[^>]*>(.*?)\<\s*\/\s*static-layout>|<\s*dynamic-layout[^>]*>(.*?)\<\s*\/\s*dynamic-layout>";
        protected const string regexID = @"id\s*=\s*\""(.*?)""";
        protected const string regexName = @"name\s*=\s*\""(.*?)""";
        protected const string regexCode = @"code\s*=\""(.*?)""";
        private readonly ILogs _logs;
        private readonly WebMenu _menu;
        private readonly CPMenuService _menuService;
        private readonly CPUserService _userService;
        private readonly CPRoleService _roleService;
        private readonly CPLangService _langService;
        private readonly Security _security;
        private readonly IConfiguration _configuration;
        private readonly ModLessonService _lessionService;
        private readonly ModLessonPartService _lessionPartService;
        private readonly ModLessonExtendService _lessionExtendService;
        private readonly ModLessonPartAnswerService _answerService;
        private readonly CPLoginLogService _loginLogService;
        private readonly CPLangEntity _currentLang;
        private readonly CPUserEntity _currentUser;
        private readonly FileProcess _fileProcess;
        public CPApiController(ILogs logs
            , IConfiguration configuration
            , CPMenuService menuService
            , CPUserService userService
            , CPRoleService roleService
            , CPLangService langService
            , Security security
            , ModLessonService lessonService
            , ModLessonPartService lessonPartService
            , ModLessonExtendService lessonExtendService
            , ModLessonPartAnswerService answerService
            , CPLoginLogService loginLogService
            , FileProcess fileProcess)
        {
            _configuration = configuration;
            _logs = logs;
            _menu = new WebMenu();
            _security = security;
            _menuService = menuService;
            _userService = userService;
            _roleService = roleService;
            _langService = langService;
            _lessionService = lessonService;
            _lessionPartService = lessonPartService;
            _lessionExtendService = lessonExtendService;
            _loginLogService = loginLogService;
            _answerService = answerService;
            _fileProcess = fileProcess;
            _currentLang = StartUp.CurrentLang;
            _currentUser = StartUp.CurrentUser;
        }
        [HttpGet]
        public Response GetCurrentUser()
        {
            IDictionary<string, string> valuePairs = new Dictionary<string, string>()
            {
                { "UserID",_currentUser.ID},
                { "ClientID",HttpContext.GetValue(Cookies.DefaultLogin,false) }
            };
            return new Response(200, "Success", valuePairs);
        }
        private bool CheckLogin(string UserID, string ClientID)
        {
            try
            {
                if (_currentUser == null) return false;
                return _currentUser.ID == UserID && _loginLogService.GetItemByCode(ClientID) != null;
            }
            catch (Exception ex)
            {
                _logs.WriteLogsError(ex);
                return false;
            }
        }
        private async Task<IEnumerable<ModLessonExtendEntity>> CreateLessonExtends(IFormCollection form, string LessonPartID)
        {
            try
            {
                List<ModLessonExtendEntity> extendEntities = new List<ModLessonExtendEntity>();
                if (form != null)
                {
                    var listFiles = form.Files;
                    if (listFiles != null && listFiles.Count > 0)
                    {
                        int i = 0;
                        foreach (var file in listFiles)
                        {
                            i++;
                            string link = await _fileProcess.SaveMediaAsync(file);
                            var extends = new ModLessonExtendEntity()
                            {
                                IsActive = true,
                                NameOriginal = file.FileName,
                                Created = DateTime.Now,
                                LessonPartID = LessonPartID,
                                Order = i,
                                Updated = DateTime.Now,
                                File = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + link,
                                OriginalFile = link

                            };
                            await _lessionExtendService.AddAsync(extends);
                            extendEntities.Add(extends);
                        }
                    }
                }
                return extendEntities;
            }
            catch (Exception ex)
            {
                await _logs.WriteLogsError("CreateOrUpdateLesson", ex);
                return null;
            }
        }
        #region Lession
        //get Lesson
        [HttpGet]
        public async Task<Response> GetListLesson(string courseID, string UserID, string ClientID)
        {
            try
            {
                if (CheckLogin(UserID, ClientID))
                {
                    var data = await _lessionService.CreateQuery().Find(o => o.CourseID == courseID && o.CreateUser == _currentUser.ID).ToListAsync();
                    if (data == null) return new Response(404, "Data not Found", null);
                    return new Response(200, "Success get all", data);
                }
                else
                {
                    return new Response(301, "Lỗi xác thực", null);
                }
            }
            catch (Exception ex)
            {
                await _logs.WriteLogsError("GetListLesson", ex);
                return new Response(500, ex.Message, null);
            }
        }
        //get LessonDetails
        [HttpGet]
        public async Task<Response> GetDetailsLesson(string ID, string UserID, string ClientID)
        {
            try
            {
                if (CheckLogin(UserID, ClientID))
                {
                    var data = await _lessionService.CreateQuery().Find(o => o.ID == ID && o.CreateUser == _currentUser.ID).SingleOrDefaultAsync();
                    if (data == null) return new Response(404, "Data not Found", null);
                    return new Response(200, "Success get all", data);
                }
                else
                {
                    return new Response(301, "Lỗi xác thực", null);
                }
            }
            catch (Exception ex)
            {
                await _logs.WriteLogsError("GetListLesson", ex);
                return new Response(500, ex.Message, null);
            }
        }
        //add Lesson
        [HttpPost]
        public async Task<Response> CreateOrUpdateLesson(ModLessonEntity item, string UserID, string ClientID)
        {
            try
            {
                if (CheckLogin(UserID, ClientID))
                {
                    var data = await _lessionService.CreateQuery().Find(o => o.ID == item.ID && o.CreateUser == _currentUser.ID).SingleOrDefaultAsync();
                    if (data == null)
                    {
                        item.Code = _currentUser.ID + "_" + item.CourseID + "_" + item.ChapterID + "_" + UnicodeName.ConvertUnicodeToCode(item.Title, "-", true);
                        item.Created = DateTime.Now;
                        item.CreateUser = _currentUser.ID;
                        item.IsAdmin = true;
                        item.IsActive = false;
                        item.Updated = DateTime.Now;
                        var list = _lessionService.CreateQuery().Find(o => o.Code == item.Code).ToList();
                        if (list != null && list.Count > 0)
                        {
                            item.Code = item.Code + "_" + list.Count.ToString();
                        }
                    }
                    else
                    {
                        item.Updated = DateTime.Now;
                    }
                    await _lessionService.AddAsync(item);
                    return new Response(200, "Success get all", item);
                }
                else
                {
                    return new Response(301, "Lỗi xác thực", null);
                }
            }
            catch (Exception ex)
            {
                await _logs.WriteLogsError("CreateOrUpdateLesson", ex);
                return new Response(500, ex.Message, null);
            }
        }
        //Remove Lesson
        [HttpPost]
        public async Task<Response> RemoveLesson(string ID, string UserID, string ClientID)
        {
            try
            {
                if (CheckLogin(UserID, ClientID))
                {
                    var ourItem = _lessionService.CreateQuery().Find(o => o.ID == ID && o.CreateUser == UserID);
                    if (ourItem != null)
                    {
                        var childNote = _lessionPartService.CreateQuery().Find(o => o.ParentID == ID).ToList();
                        for (int i = 0; childNote != null && i < childNote.Count; i++)
                        {
                            var item = childNote[0];
                            var media = _lessionExtendService.CreateQuery().Find(o => o.LessonPartID == item.ID).ToList();
                            if (media != null && media.Count > 0)
                            {
                                _fileProcess.DeleteFiles(media.Select(o => o.OriginalFile).ToList());
                                await _lessionExtendService.RemoveRangeAsync(media.Select(o => o.ID));
                            }
                            await _lessionPartService.RemoveAsync(item.ID);
                        }
                        await _lessionService.RemoveAsync(ID);
                        return new Response(200, "Success get all", ID);
                    }
                    else
                    {
                        return new Response(301, "Permission fails", ID);
                    }
                }
                else
                {
                    return new Response(301, "Lỗi xác thực", null);
                }

            }
            catch (Exception ex)
            {
                await _logs.WriteLogsError("CreateOrUpdateLesson", ex);
                return new Response(500, ex.Message, null);
            }
        }
        #endregion
        #region LessonPart
        [HttpGet]
        public async Task<Response> GetListLessonPart(string LessonID, string UserID, string ClientID)
        {
            try
            {
                if (CheckLogin(UserID, ClientID))
                {
                    var root = _lessionService.CreateQuery().Find(o => o.ID == LessonID && o.CreateUser == UserID).SingleOrDefault();
                    if (root != null)
                    {
                        var listLessonPart = _lessionPartService.CreateQuery().Find(o => o.ParentID == LessonID).ToList();
                        if (listLessonPart == null || listLessonPart.Count <= 0) return new Response(404, "data not found", null);
                        return new Response(200, "Success", listLessonPart);
                    }
                    else
                    {
                        return new Response(301, "Permission fails", null);
                    }
                }
                else
                {
                    return new Response(301, "Lỗi xác thực", null);
                }

            }
            catch (Exception ex)
            {
                await _logs.WriteLogsError("GetListLessonPart", ex);
                return new Response(500, ex.Message, null);
            }
        }
        [HttpGet]
        public async Task<Response> GetDetailsLessonPart(string ID, string UserID, string ClientID)
        {
            try
            {
                if (CheckLogin(UserID, ClientID))
                {
                    var item = _lessionPartService.CreateQuery().Find(o => o.ID == ID).SingleOrDefault();
                    if (item == null) return new Response(404, "data not found", null);
                    var root = _lessionService.CreateQuery().Find(o => o.ID == item.ParentID && o.CreateUser == UserID).SingleOrDefault();
                    if (root != null)
                    {
                        var listMedia = _lessionExtendService.CreateQuery().Find(o => o.LessonPartID == item.ID);
                        var listChild = _answerService.CreateQuery().Find(o => o.ParentID == item.ID);
                        IDictionary<string, object> keyValues = new Dictionary<string, object>()
                        {
                            {"LessonPart",item },
                            {"Answer",listChild},
                            {"Media",listMedia }
                        };
                        return new Response(200, "Success", keyValues);
                    }
                    else
                    {
                        return new Response(301, "Permission fails", null);
                    }
                }
                else
                {
                    return new Response(301, "Lỗi xác thực", null);
                }

            }
            catch (Exception ex)
            {
                await _logs.WriteLogsError("GetListLessonPart", ex);
                return new Response(500, ex.Message, null);
            }
        }
        [HttpPost]
        public async Task<Response> CreateOrUpdateLessonPart(ModLessonPartEntity item, string UserID, string ClientID)
        {
            try
            {
                if (CheckLogin(UserID, ClientID))
                {
                    if(item.ID == "0")
                    {
                        var listItem = _lessionPartService.CreateQuery().Find(o => o.ParentID == item.ParentID).ToList();
                        item.Created = DateTime.Now;
                        item.Order = listItem != null ? listItem.Count : 0;
                        item.Updated = DateTime.Now;
                    }
                    else
                    {
                        item.Updated = DateTime.Now;
                    }
                    var ourItem = _lessionService.CreateQuery().Find(o => o.ID == item.ParentID && o.CreateUser == UserID).SingleOrDefault();
                    if (ourItem != null)
                    {
                        await _lessionPartService.AddAsync(item);
                        IEnumerable<ModLessonExtendEntity> files = new List<ModLessonExtendEntity>();
                        if (HttpContext.Request.Form != null)
                        {
                            if (HttpContext.Request.Form.Files != null && HttpContext.Request.Form.Files.Count > 0)
                            {
                                files = await CreateLessonExtends(HttpContext.Request.Form, item.ID);
                            }
                            else
                            {
                                files = _lessionExtendService.CreateQuery().Find(o => o.LessonPartID == item.ID).ToEnumerable();
                            }
                        }
                        IDictionary<string, object> valuePairs = new Dictionary<string, object>
                        {
                            { "LessonPart", item },
                            { "LessonPartExtends", files }
                        };
                        return new Response(200, "Success get all", valuePairs);
                    }
                    else
                    {
                        return new Response(301, "Permission fails", null);
                    }
                }
                else
                {
                    return new Response(301, "Lỗi xác thực", null);
                }

            }
            catch (Exception ex)
            {
                await _logs.WriteLogsError("CreateOrUpdateLesson", ex);
                return new Response(500, ex.Message, null);
            }
        }

        [HttpPost]
        public async Task<Response> RemoveLessonPart(string ID, string UserID, string ClientID)
        {
            try
            {
                if (CheckLogin(UserID, ClientID))
                {
                    var Item = _lessionPartService.CreateQuery().Find(o => o.ID == ID).SingleOrDefault();
                    if (Item == null) return new Response(404, "data not found", ID);
                    var ourItem = _lessionService.CreateQuery().Find(o => o.ID == Item.ParentID && o.CreateUser == UserID).SingleOrDefault();
                    if (ourItem != null)
                    {
                        var item = Item;
                        var media = _lessionExtendService.CreateQuery().Find(o => o.LessonPartID == item.ID).ToList();
                        if (media != null && media.Count > 0)
                        {
                            _fileProcess.DeleteFiles(media.Select(o => o.OriginalFile).ToList());
                            await _lessionExtendService.RemoveRangeAsync(media.Select(o => o.ID));
                        }
                        await _lessionPartService.RemoveAsync(item.ID);

                        return new Response(200, "Success get all", ID);
                    }
                    else
                    {
                        return new Response(301, "Permission fails", ID);
                    }
                }
                else
                {
                    return new Response(301, "Lỗi xác thực", null);
                }

            }
            catch (Exception ex)
            {
                await _logs.WriteLogsError("CreateOrUpdateLesson", ex);
                return new Response(500, ex.Message, null);
            }
        }
        #endregion
        #region Lesson Answer
        [HttpGet]
        public async Task<Response> GetListAnswer(string LessonPartID, string UserID, string ClientID)
        {
            try
            {
                if (CheckLogin(UserID, ClientID))
                {
                    var lessonPart = _lessionPartService.CreateQuery().Find(o => o.ID == LessonPartID).SingleOrDefault();
                    if (lessonPart == null) return new Response(404, "data not found", null);
                    var root = _lessionService.CreateQuery().Find(o => o.ID == lessonPart.ParentID && o.CreateUser == UserID).SingleOrDefault();
                    if (root != null)
                    {
                        var listAnswer = _answerService.CreateQuery().Find(o => o.ParentID == LessonPartID).ToList();
                        if (listAnswer == null || listAnswer.Count <= 0) return new Response(404, "data not found", null);
                        return new Response(200, "Success", listAnswer);
                    }
                    else
                    {
                        return new Response(301, "Permission fails", null);
                    }
                }
                else
                {
                    return new Response(301, "Lỗi xác thực", null);
                }

            }
            catch (Exception ex)
            {
                await _logs.WriteLogsError("GetListLessonPart", ex);
                return new Response(500, ex.Message, null);
            }
        }
        [HttpGet]
        public async Task<Response> GetDetailsAnswer(string ID, string UserID, string ClientID)
        {
            try
            {
                if (CheckLogin(UserID, ClientID))
                {
                    var listAnswer = _answerService.CreateQuery().Find(o => o.ID == ID).SingleOrDefault();
                    if (listAnswer == null) return new Response(404, "data not found", null);
                    return new Response(200, "Success", listAnswer);
                }
                else
                {
                    return new Response(301, "Lỗi xác thực", null);
                }

            }
            catch (Exception ex)
            {
                await _logs.WriteLogsError("GetListLessonPart", ex);
                return new Response(500, ex.Message, null);
            }
        }
        [HttpPost]
        public async Task<Response> CreateOrUpdateLessonAnswer(ModLessonPartAnswerEntity item, string UserID, string ClientID)
        {
            try
            {
                if (CheckLogin(UserID, ClientID))
                {
                    if (item.ID == "0")
                    {
                        var listItem = _answerService.CreateQuery().Find(o => o.ParentID == item.ParentID).ToList();
                        item.Created = DateTime.Now;
                        item.Order = listItem != null ? listItem.Count : 0;
                        item.Updated = DateTime.Now;
                    }
                    else
                    {
                        item.Updated = DateTime.Now;
                    }
                    await _answerService.AddAsync(item);
                    return new Response(200, "Success get all", item);
                }
                else
                {
                    return new Response(301, "Lỗi xác thực", null);
                }

            }
            catch (Exception ex)
            {
                await _logs.WriteLogsError("CreateOrUpdateLesson", ex);
                return new Response(500, ex.Message, null);
            }
        }

        [HttpPost]
        public async Task<Response> RemoveLessonAnswer(string ID, string UserID, string ClientID)
        {
            try
            {
                if (CheckLogin(UserID, ClientID))
                {
                    var item = _answerService.CreateQuery().Find(o => o.ID == ID).SingleOrDefault();
                    if (item == null) return new Response(404, "data not found", ID);
                    await _answerService.RemoveAsync(item.ID);

                    return new Response(200, "Success get all", ID);
                }
                else
                {
                    return new Response(301, "Lỗi xác thực", null);
                }

            }
            catch (Exception ex)
            {
                await _logs.WriteLogsError("CreateOrUpdateLesson", ex);
                return new Response(500, ex.Message, null);
            }
        }
        #endregion
        #region LessonExtends
        [HttpGet]
        public async Task<Response> GetDetailsLessonExtends(string ID, string UserID, string ClientID)
        {
            try
            {
                if (CheckLogin(UserID, ClientID))
                {
                    var item = _answerService.CreateQuery().Find(o => o.ID == ID).SingleOrDefault();
                    if (item == null) return new Response(404, "data not found", ID);
                    return new Response(200, "Success get all", ID);
                }
                else
                {
                    return new Response(301, "Lỗi xác thực", null);
                }

            }
            catch (Exception ex)
            {
                await _logs.WriteLogsError("CreateOrUpdateLesson", ex);
                return new Response(500, ex.Message, null);
            }
        }
        [HttpPost]
        public async Task<Response> UpdateLessonExtends(ModLessonExtendEntity item, string UserID, string ClientID)
        {
            try
            {
                if (CheckLogin(UserID, ClientID))
                {
                    var OldItem = _lessionExtendService.GetByID(item.ID);
                    if (OldItem == null) return new Response(404, "no data found", null);

                    var file = HttpContext.Request.Form != null && HttpContext.Request.Form.Files.Count > 0 ? HttpContext.Request.Form.Files[0] : null;
                    if(file != null)
                    {
                       await _fileProcess.Update(OldItem.OriginalFile, file);
                    }
                    await _lessionExtendService.AddAsync(item);
                    return new Response(200, "Success get all", item);
                }
                else
                {
                    return new Response(301, "Lỗi xác thực", null);
                }

            }
            catch (Exception ex)
            {
                await _logs.WriteLogsError("CreateOrUpdateLesson", ex);
                return new Response(500, ex.Message, null);
            }
        }
        #endregion
        public Response GetMenuForWeb(string type)
        {
            try
            {
                var data = _menuService.GetItemByType(type, _currentLang.ID);
                if (data == null) return new Response()
                {
                    Code = 404,
                    Data = null,
                    Message = "No Data Found"
                };
                return new Response()
                {
                    Code = 200,
                    Data = data,
                    Message = ""
                };
            }
            catch (Exception ex)
            {
                _logs.WriteLogsError("GETMenuForWeb", ex);
                return new Response()
                {
                    Code = 500,
                    Data = null,
                    Message = ex.Message
                };
            }
        }
        public Response GetCModuleAll()
        {
            try
            {
                var data = _menu.GetControl.Select(o => new { o.Name, o.Code, o.FullName });
                if (data == null) return new Response() { Code = 404, Data = null, Message = "Data Not Found" };
                return new Response() { Code = 200, Data = data, Message = "Success" };
            }
            catch (Exception ex)
            {
                _logs.WriteLogsError("GetTemplateInfo", ex);
                return new Response()
                {
                    Code = 500,
                    Data = null,
                    Message = ex.Message
                };
            }

        }
        public CModule GetCModule(string name)
        {
            return _menu.GetControl.SingleOrDefault(o => o.FullName == name);
        }
        #region Create Begin
        public Response StartPage()
        {
            var listRole = new List<CPRoleEntity>()
            {
                new CPRoleEntity()
                {
                    Name = "Administrator",
                    Code = "administrator",
                    Lock = true
                },
                new CPRoleEntity()
                {
                    Name = "Member",
                    Code = "member",
                    Lock = true
                }
            };
            _roleService.AddRange(listRole);
            List<CPUserEntity> listUser = new List<CPUserEntity>();
            for (int i = 0; i < listRole.Count; i++)
            {
                var user = new CPUserEntity()
                {
                    IsActive = true,
                    BirthDay = DateTime.Now,
                    Created = DateTime.Now,
                    Email = listRole[i].Code + "@gmail.com",
                    Name = listRole[i].Name,
                    RoleID = listRole[i].ID,
                    Phone = "0976497928",
                    Skype = "breakingdawn1235",
                    Pass = Security.Encrypt("123")
                };
                _userService.Add(user);
                listUser.Add(user);
            }
            List<CPLangEntity> listLang = new List<CPLangEntity>()
            {
                new CPLangEntity()
                {
                    IsActive =true,
                    Code = "VN",
                    Name = "Việt Nam"
                },
                new CPLangEntity()
                {
                    IsActive =true,
                    Code = "EN",
                    Name = "English"
                }
            };
            _langService.AddRange(listLang);
            IDictionary<string, dynamic> data = new Dictionary<string, dynamic>();
            data.Add("Role", listRole);
            data.Add("User", listUser);
            data.Add("Lang", listLang);
            return new Response(201, "Create Success", data);
        }
        #endregion
    }
    public class Response
    {
        public Response()
        {
        }

        public Response(long code, string message, object data)
        {
            Code = code;
            Message = message;
            Data = data;
        }

        public long Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
