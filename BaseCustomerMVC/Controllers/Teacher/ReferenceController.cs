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

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class ReferenceController : TeacherController
    {
        private readonly TeacherService _teacherService;
        private readonly ClassService _classService;
        private readonly FileProcess _fileProcess;
        private readonly ReferenceService _referenceService;

        public ReferenceController(
            TeacherService teacherService,
            ClassService classService,
            FileProcess fileProcess,
            ReferenceService referenceService
            )
        {
            _teacherService = teacherService;
            _classService = classService;
            _referenceService = referenceService;
            _fileProcess = fileProcess;
        }

        public IActionResult Index(DefaultModel model)
        {
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            var myClasses = _referenceService.CreateQuery()
                .Find(t => t.OwnerID == UserID)
                //.Find(t=> true)
                //.Project(Builders<ClassEntity>.Projection.Include(t => t.ID).Include(t => t.Name))
                .ToList();
            ViewBag.AllClass = myClasses;
            ViewBag.User = UserID;
            return View();
        }

        public JsonResult GetList(ReferenceEntity entity, DefaultModel defaultModel)
        {
            if (entity != null)
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var result = new List<ReferenceEntity>();
                switch (entity.Range)
                {
                    case REF_RANGE.TEACHER:
                        result = _referenceService.CreateQuery().Find(t => t.Range == REF_RANGE.TEACHER && t.OwnerID == UserID).Skip(defaultModel.PageSize * defaultModel.PageIndex).Limit(defaultModel.PageSize).ToList();
                        break;
                    case REF_RANGE.CLASS:
                        result = _referenceService.CreateQuery().Find(t => t.OwnerID == UserID && (t.Range == REF_RANGE.TEACHER) || (t.Range == REF_RANGE.CLASS && t.Target == entity.Target)).Skip(defaultModel.PageSize * defaultModel.PageIndex).Limit(defaultModel.PageSize).ToList();
                        break;
                    //case REF_RANGE.ALL:
                    default:
                        result = _referenceService.CreateQuery().Find(t => t.Range == REF_RANGE.ALL).Skip(defaultModel.PageSize * defaultModel.PageIndex).Limit(defaultModel.PageSize).ToList();
                        break;
                }
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", result },
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
                            {"Error", "Item not found" }
                        });
                    if (oldObj.OwnerID != UserID)
                        return new JsonResult(new Dictionary<string, object>
                        {
                            {"Error", "Permission fail" }
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
                if (!string.IsNullOrEmpty(entity.Link))
                    if (!(entity.Link.ToLower().StartsWith("http://") || entity.Link.ToLower().StartsWith("https://")))
                        entity.Link = "http://" + entity.Link;
                entity.UpdateTime = DateTime.Now;
                _referenceService.CreateOrUpdate(entity);
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
                        {"Error", "Permission fail" }
                    });
                await _referenceService.RemoveAsync(ID);
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
    }
}
