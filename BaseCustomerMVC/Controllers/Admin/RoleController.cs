using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using MongoDB.Driver;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OfficeOpenXml;
using Core_v2.Globals;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using MongoDB.Bson;
using BaseAccess.Interfaces;
using System.Reflection;
using Core_v2.Interfaces;
using BaseCustomerEntity.Globals;
using System.Security.Claims;

namespace BaseCustomerMVC.Controllers.Admin
{
    [BaseAccess.Attribule.AccessCtrl("Quản lý quyền", "admin", 4)]
    public class RoleController : AdminController
    {
        private readonly ILog _log;
        private readonly IAccess _access;
        private readonly AccessesService _accessesService;
        private readonly RoleService _service;
        private readonly IHostingEnvironment _env;
        public RoleController(ILog log)
        {
            _log = log;
        }
        // GET: Home

        public ActionResult Index(DefaultModel model)
        {
            ViewBag.Model = model;
            return View();
        }
        [HttpGet]
        public JsonResult Get(DefaultModel model)
        {
            try
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    string parent = User.FindFirst(ClaimTypes.Role)?.Value;
                    if (parent != null && parent != "supperadmin")
                    {
                        var data = _service.CreateQuery().Find(o =>
                        (string.IsNullOrEmpty(model.SearchText) || (o.Name.Contains(model.SearchText) || o.Code.Contains(model.SearchText)) &&
                        (string.IsNullOrEmpty(model.ID) || o.ID == model.ID) &&
                        (parent != null && o.ParentID == parent) // chỉ lấy data từ thằng cha
                        ))?.ToList();
                        if (data != null)
                        {
                            return new JsonResult(new { code = 200, msg = "success", data = data });
                        }
                    }
                    if(parent == "supperadmin")
                    {
                        var data = _service.CreateQuery().Find(o =>
                        (string.IsNullOrEmpty(model.SearchText) || (o.Name.Contains(model.SearchText) || o.Code.Contains(model.SearchText)) &&
                        (string.IsNullOrEmpty(model.ID) || o.ID == model.ID)))?.ToList();

                        if (data != null)
                        {
                            return new JsonResult(new { code = 200, msg = "success", data = data });
                        }
                    }
                }
                return new JsonResult(new { code = 404, msg = "Data not found" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { code = 500, msg = ex.Message, data = ex });
            }
        }
        [HttpPost]
        public JsonResult Create(RoleEntity item)
        {
            try
            {
                if(User != null && User.Identity.IsAuthenticated)
                {
                    if (!string.IsNullOrEmpty(item.Name))
                    {
                        item.UserCreate = User.FindFirst("UserID")?.Value;
                        item.CreateDate = DateTime.Now;
                        string code = item.Name.ConvertUnicodeToCode("", true);
                        item.Code = code;
                        if (_service.GetItemByCode(code) == null)
                        {
                            _service.CreateOrUpdate(item);
                            return new JsonResult(new { code = 200, msg = "create success", data = item });
                        }
                        else
                        {
                            return new JsonResult(new { code = 301, msg = "exist item", data = item });
                        }
                    }
                    return new JsonResult(new { code = 305, msg = "name empty", data = item });
                }
                return new JsonResult(new { code = 405, msg = "User not found", data = item });
            }
            catch(Exception ex)
            {
                return new JsonResult(new { code = 500, msg = ex.Message, data = ex });
            }
        }
        [HttpDelete]
        public JsonResult Delete(string id)
        {
            try
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        var item = _service.GetItemByID(id);
                        if(item == null)
                        {
                            return new JsonResult(new { code = 404, msg = "Data not found" });
                        }
                        if(item.Code == User.FindFirst(ClaimTypes.Role)?.Value || item.Code == "superadmin")
                        {
                            return new JsonResult(new { code = 405, msg = "Insufficient authority" });
                        }
                        _service.Remove(id);
                        return new JsonResult(new { code = 200, msg = "delete success", data = item });
                    }
                    return new JsonResult(new { code = 305, msg = "id empty"});
                }
                return new JsonResult(new { code = 404, msg = "Data not found" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { code = 500, msg = ex.Message, data = ex });
            }
        }
        [HttpPut]
        public JsonResult Update(RoleEntity item)
        {
            try
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    if (item != null && !string.IsNullOrEmpty(item.ID))
                    {
                        var oldItem = _service.GetItemByID(item.ID);
                        if (oldItem != null)
                        {
                            if (item.Code == User.FindFirst(ClaimTypes.Role)?.Value || item.Code == "superadmin")
                            {
                                return new JsonResult(new { code = 405, msg = "Insufficient authority" });
                            }
                            item.Code = oldItem.Code;
                            item.UserCreate = oldItem.UserCreate;
                            item.CreateDate = oldItem.CreateDate;
                            if (string.IsNullOrEmpty(item.ParentID)) item.ParentID = oldItem.ParentID;
                            _service.CreateOrUpdate(item);
                            return new JsonResult(new { code = 200, msg = "update success", data = item });
                        }
                    }
                    return new JsonResult(new { code = 404, msg = "Data not found" });
                }
                return new JsonResult(new { code = 404, msg = "Data not found" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { code = 500, msg = ex.Message, data = ex });
            }
        }

        [HttpPut]
        public JsonResult Approved(string id)
        {
            try
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        var item = _service.GetItemByID(id);
                        if (item == null)
                        {
                            return new JsonResult(new { code = 404, msg = "Data not found" });
                        }
                        if (item.Code == User.FindFirst(ClaimTypes.Role)?.Value || item.Code == "superadmin")
                        {
                            return new JsonResult(new { code = 405, msg = "Insufficient authority" });
                        }
                        var filter = Builders<RoleEntity>.Filter.Eq(o=>o.ID, item.ID);
                        var update = Builders<RoleEntity>.Update.Set(o => o.IsActive, !item.IsActive);
                        _service.CreateQuery().UpdateOne(filter, update);
                        return new JsonResult(new { code = 200, msg = "Approved success", data = item });
                    }
                    return new JsonResult(new { code = 305, msg = "id empty" });
                }
                return new JsonResult(new { code = 404, msg = "Data not found" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { code = 500, msg = ex.Message, data = ex });
            }
        }


        public IActionResult Detail(string id)
        {
            if (string.IsNullOrEmpty(id)) return View("Index");

            var assembly = GetAssembly();

            ViewBag.AdminCtrl = _access.GetAccessByAttribue<Globals.AdminController>(assembly, "admin");
            ViewBag.TeacherCtrl = _access.GetAccessByAttribue<Globals.TeacherController>(assembly, "teacher");
            ViewBag.StudentCtrl = _access.GetAccessByAttribue<Globals.StudentController>(assembly, "student");

            ViewBag.Data = _accessesService.Collection.Find(o => o.RoleID == id && o.IsActive == true)?.ToList();
            ViewBag.RoleID = id;
            return View();
        }
        [HttpPost]
        public IActionResult Detail(string id, List<AccessEntity> data)
        {
            if (string.IsNullOrEmpty(id)) return View("Index");

            var assembly = GetAssembly();

            ViewBag.AdminCtrl = _access.GetAccessByAttribue<Globals.AdminController>(assembly, "admin");
            ViewBag.TeacherCtrl = _access.GetAccessByAttribue<Globals.TeacherController>(assembly, "teacher");
            ViewBag.StudentCtrl = _access.GetAccessByAttribue<Globals.StudentController>(assembly, "student");

            for (int i = 0; data != null && i < data.Count; i++)
            {
                var item = data[i];
                var oldItem = _accessesService.Collection.Find(o => o.CtrlName == item.CtrlName && o.ActName == item.ActName && o.RoleID == id && o.Type == item.Type)?.SingleOrDefault();
                if (oldItem != null)
                {
                    oldItem.IsActive = item.IsActive;
                    _accessesService.Collection.ReplaceOne(x => x.ID == oldItem.ID, oldItem);
                }
                else
                {
                    item.RoleID = id;
                    item.CreateDate = DateTime.Now;
                    item.UserCreate = User.FindFirst("UserID")?.Value;
                    _accessesService.CreateOrUpdate(item);
                }
            }
            ViewBag.Data = _accessesService.Collection.Find(o => o.RoleID == id && o.IsActive == true)?.ToList();
            ViewBag.RoleID = id;
            return View();
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetList(DefaultModel model)
        {
            var filter = new List<FilterDefinition<RoleEntity>>();

            if (!string.IsNullOrEmpty(model.SearchText))
            {
                filter.Add(Builders<RoleEntity>.Filter.Where(o => o.Name.ToLower().Contains(model.SearchText.ToLower()) || o.Code.ToLower().Contains(model.SearchText.ToLower())));
            }
            if (model.StartDate > DateTime.MinValue)
            {
                filter.Add(Builders<RoleEntity>.Filter.Where(o => o.CreateDate >= new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0)));
            }
            if (model.EndDate > DateTime.MinValue)
            {
                filter.Add(Builders<RoleEntity>.Filter.Where(o => o.CreateDate <= new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59)));
            }
            var data = filter.Count > 0 ? _service.Collection.Find(Builders<RoleEntity>.Filter.And(filter)) : _service.GetAll();
            model.TotalRecord = data.Count();
            var DataResponse = data == null || data.Count() <= 0 || data.Count() < model.PageSize
                ? data.ToList()
                : data.Skip((model.PageIndex - 1) * model.PageSize).Limit(model.PageSize).ToList();
            var response = new Dictionary<string, object>
            {
                { "Data", DataResponse },
                { "Model", model }
            };
            return new JsonResult(response);

        }


        private Assembly GetAssembly()
        {
            return Assembly.GetExecutingAssembly();
        }
    }
}
