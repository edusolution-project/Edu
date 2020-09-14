using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using FileManagerCore.Interfaces;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class ReferenceController : TeacherController
    {
        private readonly TeacherService _teacherService;
        private readonly TeacherHelper _teacherHelper;
        private readonly ClassService _classService;
        private readonly FileProcess _fileProcess;
        private readonly ReferenceService _referenceService;
        private readonly CenterService _centerService;
        private readonly SubjectService _subjectService;
        private readonly GradeService _gradeService;
        private readonly IHostingEnvironment _env;
        private readonly IRoxyFilemanHandler _roxyFilemanHandler;

        private readonly HashSet<string> _imageType = new HashSet<string>() { "JPG", "JPEG", "GIF", "PNG", "ICO", "SVG" };
        private readonly HashSet<string> _fileType = new HashSet<string>() { "DOC", "DOCX", "XLS", "XLSX", "PPTX", "PPTX", "PDF" };
        private string host;
        private string staticPath;

        public ReferenceController(
            TeacherService teacherService,
            TeacherHelper teacherHelper,
            ClassService classService,
            FileProcess fileProcess,
            IHostingEnvironment env,
            ReferenceService referenceService,
            CenterService centerService,
            SubjectService subjectService,
            GradeService gradeService,
            IConfiguration iConfig,
            IRoxyFilemanHandler roxyFilemanHandler
            )
        {
            _teacherService = teacherService;
            _classService = classService;
            _referenceService = referenceService;
            _fileProcess = fileProcess;
            _centerService = centerService;
            _teacherHelper = teacherHelper;
            _subjectService = subjectService;
            _gradeService = gradeService;
            _env = env;
            _roxyFilemanHandler = roxyFilemanHandler;
            host = iConfig.GetValue<string>("SysConfig:Domain");
            staticPath = iConfig.GetValue<string>("SysConfig:StaticPath");
        }

        public IActionResult Index(DefaultModel model, string basis, int old = 0)
        {
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            if (!string.IsNullOrEmpty(basis))
            {
                var center = _centerService.GetItemByCode(basis);
                if (center != null)
                    ViewBag.Center = center;
                ViewBag.IsHeadTeacher = _teacherHelper.HasRole(UserID, center.ID, "head-teacher");
            }

            var teacher = _teacherService.CreateQuery().Find(t => t.ID == UserID).SingleOrDefault();
            if (teacher != null && teacher.Subjects != null)
            {
                var subjects = _subjectService.CreateQuery().Find(t => teacher.Subjects.Contains(t.ID)).ToList();
                var grades = _gradeService.CreateQuery().Find(t => teacher.Subjects.Contains(t.SubjectID)).ToList();
                ViewBag.Grades = grades;
                ViewBag.Subjects = subjects;
            }

            var myClasses = _classService.CreateQuery()
                .Find(t => t.Members.Any(o => o.TeacherID == UserID)
                //&& t.IsActive
                ).SortByDescending(t => t.IsActive).ThenByDescending(t => t.StartDate)
                //.Find(t=> true)
                //.Project(Builders<ClassEntity>.Projection.Include(t => t.ID).Include(t => t.Name))
                .ToList();
            ViewBag.AllClass = myClasses;
            ViewBag.User = UserID;
            if (old == 1)
                return View("index_o");
            return View();
        }

        public JsonResult GetList(ReferenceEntity entity, DefaultModel defaultModel)
        {
            if (entity != null)
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var filter = new List<FilterDefinition<ReferenceEntity>>();

                var tc = (string.IsNullOrEmpty(entity.Target) || entity.Range != REF_RANGE.CLASS) ? UserID : entity.Target;
                var filterTeacher = Builders<ReferenceEntity>.Filter.Where(o => o.Range == REF_RANGE.CLASS && o.OwnerID == tc);
                var filterAll = Builders<ReferenceEntity>.Filter.Eq(o => o.Range, REF_RANGE.ALL);

                switch (entity.Range)
                {
                    case REF_RANGE.TEACHER:
                        filter.Add(filterTeacher);
                        break;
                    case REF_RANGE.CLASS:
                        filter.Add(Builders<ReferenceEntity>.Filter.Where(o => o.Range == entity.Range && o.Target == entity.Target));
                        break;
                    case REF_RANGE.ALL:
                        filter.Add(filterAll);
                        break;
                    default:
                        filter.Add(Builders<ReferenceEntity>.Filter.Or(filterTeacher, filterAll));
                        break;
                }
                if (!string.IsNullOrEmpty(entity.SubjectID))
                {
                    filter.Add(Builders<ReferenceEntity>.Filter.Eq(t => t.SubjectID, entity.SubjectID));
                }
                if (!string.IsNullOrEmpty(entity.GradeID))
                {
                    filter.Add(Builders<ReferenceEntity>.Filter.Eq(t => t.GradeID, entity.GradeID));
                }

                if (!string.IsNullOrEmpty(defaultModel.SearchText))
                {
                    filter.Add(Builders<ReferenceEntity>.Filter.Text("\"" + defaultModel.SearchText + "\""));
                }
                var result = _referenceService.CreateQuery().Find(Builders<ReferenceEntity>.Filter.And(filter));
                defaultModel.TotalRecord = result.CountDocuments();

                //var result = _referenceService.GetAll();
                //defaultModel.TotalRecord = result.CountDocuments();
                var returnData = result.Skip(defaultModel.PageSize * defaultModel.PageIndex).Limit(defaultModel.PageSize).ToList();
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", returnData },
                    { "Model", defaultModel }
                });
            }
            return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", null},
                });
        }

        public JsonResult GetClassList(ReferenceEntity entity, DefaultModel defaultModel)
        {
            if (entity != null)
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var filter = new List<FilterDefinition<ReferenceEntity>>();
                filter.Add(Builders<ReferenceEntity>.Filter.Where(o =>
                    (o.Range == REF_RANGE.ALL) ||
                    (o.Range == REF_RANGE.TEACHER && o.Target == UserID) ||
                    (o.Range == REF_RANGE.CLASS && o.Target == entity.Target))
                );
                if (!string.IsNullOrEmpty(defaultModel.SearchText))
                {
                    filter.Add(Builders<ReferenceEntity>.Filter.Text("\"" + defaultModel.SearchText + "\""));
                }
                if (!string.IsNullOrEmpty(entity.SubjectID))
                {
                    filter.Add(Builders<ReferenceEntity>.Filter.Eq(o => o.SubjectID, entity.SubjectID));
                }
                if (!string.IsNullOrEmpty(entity.GradeID))
                {
                    filter.Add(Builders<ReferenceEntity>.Filter.Eq(o => o.GradeID, entity.GradeID));
                }
                var result = _referenceService.CreateQuery().Find(Builders<ReferenceEntity>.Filter.And(filter));
                defaultModel.TotalRecord = result.CountDocuments();
                var returnData = result.Skip(defaultModel.PageSize * defaultModel.PageIndex).Limit(defaultModel.PageSize).ToList();
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", returnData },
                    { "Model", defaultModel }
                });
            }
            return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", null},
                });
        }

        public async Task<JsonResult> Save(ReferenceEntity entity, string basis, IFormFile formFile)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                if (entity.ID == null)
                {
                    entity.OwnerID = UserID;
                    entity.OwnerName = User.Identity.Name;
                    entity.CreateTime = DateTime.Now;
                    //insert

                    //if (entity.Media != null && entity.Media.Name == null) entity.Media = null;//valid Media
                    var files = HttpContext.Request.Form != null && HttpContext.Request.Form.Files.Count > 0 ? HttpContext.Request.Form.Files : null;

                    if (files != null)
                    {
                        foreach (var file in files)
                        {
                            string extension = Path.GetExtension(file.FileName);
                            string type = extension.Replace(".", string.Empty).ToUpper();

                            var mediarsp = _roxyFilemanHandler.UploadSingleFileWithGoogleDrive(basis, UserID, file);

                            if (_imageType.Contains(type))//anh bia
                            {
                                entity.Image = mediarsp.Path;
                            }
                            else
                            {
                                entity.Media = new Media();
                                entity.Media.Name = entity.Media.OriginalName = file.FileName;
                                entity.Media.Created = DateTime.Now;
                                entity.Media.Size = file.Length;
                                entity.Media.Extension = extension;
                                entity.Media.Path = mediarsp.Path;
                            }
                        }
                    }

                    entity.UpdateTime = DateTime.Now;
                    _referenceService.Save(entity);
                }
                else
                {
                    var oldObj = _referenceService.GetItemByID(entity.ID);
                    if (oldObj == null)
                        return new JsonResult(new Dictionary<string, object>
                        {
                            {"Error", "Dữ liệu không đúng" }
                        });
                    if (oldObj.OwnerID != UserID)
                        return new JsonResult(new Dictionary<string, object>
                        {
                            {"Error", "Bạn không được quyền thực hiện thao tác này" }
                        });
                    entity.Media = oldObj.Media ?? new Media();
                    entity.OwnerID = UserID;
                    entity.OwnerName = oldObj.OwnerName;
                    entity.CreateTime = oldObj.CreateTime;

                    var files = HttpContext.Request.Form != null && HttpContext.Request.Form.Files.Count > 0 ? HttpContext.Request.Form.Files : null;
                    if (files != null)
                    {
                        //var file = files[0];//file dinh kem
                        ////.Where(f => f.Name == entity.Media.Name).SingleOrDefault();
                        ////.FirstOrDefault();
                        //if (file != null)
                        //{
                        //    entity.Media = new Media();
                        //    entity.Media.Name = entity.Media.OriginalName = file.FileName;
                        //    entity.Media.Created = DateTime.Now;
                        //    entity.Media.Size = file.Length;
                        //    entity.Media.Path = await _fileProcess.SaveMediaAsync(file, entity.Media.OriginalName, "", basis);
                        //}

                        //var cover = files[1]; //anh bia
                        //if (cover != null)
                        //{
                        //    entity.Image = await _fileProcess.SaveMediaAsync(cover, entity.Media.OriginalName, "", basis);
                        //}
                        foreach (var file in files)
                        {
                            string extension = Path.GetExtension(file.FileName);
                            string type = extension.Replace(".", string.Empty).ToUpper();

                            var mediarsp = _roxyFilemanHandler.UploadSingleFileWithGoogleDrive(basis, UserID, file);

                            if (_imageType.Contains(type))//anh bia
                            {
                                entity.Image = mediarsp.Path;
                            }
                            else
                            {
                                entity.Media = new Media();
                                entity.Media.Name = entity.Media.OriginalName = file.FileName;
                                entity.Media.Created = DateTime.Now;
                                entity.Media.Size = file.Length;
                                entity.Media.Extension = extension;
                                entity.Media.Path = mediarsp.Path;
                            }
                        }
                    }
                    else
                    {
                        entity.Image = oldObj.Image;
                    }
                }
                if (entity.Link == null)
                    entity.Link = "";
                if (!string.IsNullOrEmpty(entity.Link))
                    if (!(entity.Link.ToLower().StartsWith("http://") || entity.Link.ToLower().StartsWith("https://")))
                        entity.Link = "http://" + host + "/" + entity.Link;
                entity.UpdateTime = DateTime.Now;
                _referenceService.Save(entity);
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", entity},
                    { "Error", null }
                });
            }
            catch (Exception e)
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    {"Error", e.Message }
                });
            }
        }

        public async Task<JsonResult> Remove(string ID)
        {
            if (!string.IsNullOrEmpty(ID))
            {
                var item = _referenceService.GetItemByID(ID);
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                if (item.OwnerID != UserID)
                    return new JsonResult(new Dictionary<string, object>
                    {
                        {"Error", "Bạn không có quyền xóa tài liệu này" }
                    });
                _ = _referenceService.RemoveAsync(ID);
            }
            return new JsonResult(new Dictionary<string, object>
            {
                { "Message", "Remove OK"}
            });
        }

        [HttpPost]
        public JsonResult GetDetail(string ID)
        {
            if (!string.IsNullOrEmpty(ID))
            {
                var item = _referenceService.GetItemByID(ID);
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", item}
                });
            }
            return null;
        }

        public IActionResult Download(string ID)
        {
            if (!string.IsNullOrEmpty(ID))
            {
                var item = _referenceService.GetItemByID(ID);
                if (item == null)
                    return BadRequest();
                _ = _referenceService.IncDownload(ID, 1);

                //TODO: create download log
                if (item.Media != null)
                {
                    try
                    {
                        if (!item.Media.Path.StartsWith("http"))
                        {
                            //Hard code:
                            var filePath = Path.Combine(string.IsNullOrEmpty(staticPath) ? _env.WebRootPath : staticPath, item.Media.Path.TrimStart('/').Replace("/", "\\"));
                            var stream = System.IO.File.OpenRead(filePath);
                            return File(stream, "application/octet-stream", item.Media.Name);
                        }
                        return Redirect(item.Media.Path);
                    }
                    catch
                    {
                        return NotFound();
                    }
                }
            }
            return BadRequest();
        }

        public IActionResult OpenLink(string ID)
        {
            if (!string.IsNullOrEmpty(ID))
            {
                var item = _referenceService.GetItemByID(ID);
                if (item == null)
                    return BadRequest();
                _ = _referenceService.IncLink(ID, 1);
                //TODO: create view log
                var url = item.Link;
                if (!string.IsNullOrEmpty(url))
                {
                    if (!url.ToLower().StartsWith("http://") && !url.ToLower().StartsWith("https://"))
                        url = "http://" + host + "/" + url;
                    return Redirect(url);
                }
            }
            return BadRequest();
        }

        public JsonResult View(string ID)
        {
            if (!string.IsNullOrEmpty(ID))
                _ = _referenceService.IncView(ID, 1);
            return Json("OK");
        }
    }
}
