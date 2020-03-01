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

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class ReferenceController : TeacherController
    {
        private readonly TeacherService _teacherService;
        private readonly ClassService _classService;
        private readonly FileProcess _fileProcess;
        private readonly ReferenceService _referenceService;
        private readonly IHostingEnvironment _env;

        public ReferenceController(
            TeacherService teacherService,
            ClassService classService,
            FileProcess fileProcess,
            IHostingEnvironment env,
            ReferenceService referenceService
            )
        {
            _teacherService = teacherService;
            _classService = classService;
            _referenceService = referenceService;
            _fileProcess = fileProcess;
            _env = env;
        }

        public IActionResult Index(DefaultModel model, int old = 0)
        {
            var UserID = User.Claims.GetClaimByType("UserID").Value;
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
                if (!string.IsNullOrEmpty(defaultModel.SearchText))
                {
                    filter.Add(Builders<ReferenceEntity>.Filter.Text("\"" + defaultModel.SearchText + "\""));
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

        public async Task<JsonResult> Save(ReferenceEntity entity)
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
                        var file = files
                            //.Where(f => f.Name == entity.Media.Name).SingleOrDefault();
                            .FirstOrDefault();
                        if (file != null)
                        {
                            entity.Media = new Media();
                            entity.Media.Name = entity.Media.OriginalName = file.FileName;
                            entity.Media.Created = DateTime.Now;
                            entity.Media.Size = file.Length;
                            entity.Media.Path = await _fileProcess.SaveMediaAsync(file, entity.Media.OriginalName);
                        }
                    }
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
                        var file = files
                            //.Where(f => f.Name == entity.Media.Name)
                            .FirstOrDefault();
                        if (file != null)
                        {
                            entity.Media.Name = entity.Media.OriginalName = file.FileName;
                            entity.Media.Created = DateTime.Now;
                            entity.Media.Size = file.Length;
                            entity.Media.Path = await _fileProcess.SaveMediaAsync(file, entity.Media.OriginalName);
                        }
                    }
                }
                if (entity.Link == null)
                    entity.Link = "";
                if (!string.IsNullOrEmpty(entity.Link))
                    if (!(entity.Link.ToLower().StartsWith("http://") || entity.Link.ToLower().StartsWith("https://")))
                        entity.Link = "http://" + entity.Link;
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
                        var filePath = Path.Combine(_env.WebRootPath, item.Media.Path.TrimStart('/').Replace("/", "\\"));
                        var stream = System.IO.File.OpenRead(filePath);
                        return File(stream, "application/octet-stream", item.Media.Name);
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
                _ = _referenceService.IncView(ID, 1);
                //TODO: create view log
                var url = item.Link;
                if (!string.IsNullOrEmpty(url))
                {
                    if (!url.ToLower().StartsWith("http://") && !url.ToLower().StartsWith("https://"))
                        url = "http://" + url;
                    return Redirect(url);
                }
            }
            return BadRequest();
        }
    }
}
