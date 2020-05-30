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
using System.Runtime.Caching;
using BaseAccess.Services;

namespace BaseCustomerMVC.Controllers.Admin
{
    [BaseAccess.Attribule.AccessCtrl("Quản lý quyền", "admin", 4)]
    public class RoleController : AdminController
    {
        private readonly ILog _log;
        private readonly RoleService _service;
        private readonly AuthorityService _authorityService;
        private readonly AccessesService _accessesService;
        public RoleController(ILog log, RoleService service, AuthorityService authorityService, AccessesService accessesService)
        {
            _log = log;
            _service = service;
            _authorityService = authorityService;
            _accessesService = accessesService;
        }
        // GET: Home

        public ActionResult Index(DefaultModel model)
        {
            string code = User.FindFirst(ClaimTypes.Role)?.Value;
            if (!string.IsNullOrEmpty(code))
            {
                ViewBag.Data = _service.GetItemByCode(code);
            }
            ViewBag.Model = model;
            return View();
        }

        public ActionResult Detail(string id)
        {
            var _access = new AccessService();
            var assembly = Assembly.GetExecutingAssembly();
            ViewBag.AdminCtrl = _access.GetAccessByAttribue<Globals.AdminController>(assembly, "admin");
            ViewBag.TeacherCtrl = _access.GetAccessByAttribue<Globals.TeacherController>(assembly, "teacher");
            ViewBag.StudentCtrl = _access.GetAccessByAttribue<Globals.StudentController>(assembly, "student");
            var access = _accessesService.GetAccessByRole(id);
            if (User != null && User.Identity.IsAuthenticated)
            {
                string parent = User.FindFirst(ClaimTypes.Role)?.Value;
                var parentAccess = _accessesService.GetAccessByRole(parent);
                List<AuthorityEntity> data = _authorityService.GetAll()?.ToList();
                ViewBag.Authority = data?.ToList();
                ViewBag.ParentAccess = parentAccess?.ToList();
                ViewBag.Access = access?.ToList();
                ViewBag.Data = id;
            }
            return View();
        }

        [HttpGet]
        [Obsolete]
        public JsonResult Get([FromQuery] DefaultModel model, [FromQuery] string code)
        {
            try
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    string parent = string.IsNullOrEmpty(code) ? User.FindFirst(ClaimTypes.Role)?.Value : code;
                    if (string.IsNullOrEmpty(model.SearchText)) model.SearchText = string.Empty;
                    long count = _service.CreateQuery().Count(_ => true);
                    if (count > 0)
                    {
                        if (parent != null && parent != "supperadmin" && parent != "superadmin")
                        {
                            var data = _service.CreateQuery().Find(o =>
                            (string.IsNullOrEmpty(model.SearchText) || (o.Name.Contains(model.SearchText) || o.Code.Contains(model.SearchText))) &&
                            (string.IsNullOrEmpty(model.ID) || o.ID == model.ID) && (parent != null && o.ParentID == parent) // chỉ lấy data từ thằng cha
                            )?.ToList();
                            if (data != null)
                            {
                                return new JsonResult(new { code = 200, msg = "success", data = data });
                            }
                        }
                        if (parent == "supperadmin" || parent == "superadmin")
                        {
                            var data = _service.CreateQuery().Find(o => o.Code != "supperadmin" && o.Code != "superadmin" &&
                            (string.IsNullOrEmpty(model.SearchText) || (o.Name.Contains(model.SearchText) || o.Code.Contains(model.SearchText))) &&
                            (string.IsNullOrEmpty(o.ParentID)) &&
                            (string.IsNullOrEmpty(model.ID) || o.ID == model.ID))?.ToList();
                            if (data != null)
                            {
                                return new JsonResult(new { code = 200, msg = "success", data = data });
                            }
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
        public JsonResult Create([FromBody]RoleEntity item)
        {
            try
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    if (!string.IsNullOrEmpty(item.Name))
                    {
                        item.UserCreate = User.FindFirst("UserID")?.Value;
                        item.CreateDate = DateTime.Now;
                        string code = item.Name.ConvertUnicodeToCode("_", true);
                        item.Code = code.Replace(@" ", "_");
                        if (_service.GetItemByCode(code) == null)
                        {
                            _service.CreateNewRole(item);
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
            catch (Exception ex)
            {
                return new JsonResult(new { code = 500, msg = ex.Message, data = ex });
            }
        }

        [HttpPost]
        public JsonResult Creates([FromBody]List<RoleEntity> listItem)
        {
            try
            {
                List<RoleEntity> success = new List<RoleEntity>();
                if (User != null && User.Identity.IsAuthenticated)
                {
                    for (int i = 0; i < listItem.Count; i++)
                    {
                        var item = listItem[i];
                        if (!string.IsNullOrEmpty(item.Name))
                        {
                            item.UserCreate = User.FindFirst("UserID")?.Value;
                            item.CreateDate = DateTime.Now;
                            string code = item.Name.ConvertUnicodeToCode("_", true);
                            item.Code = code.Replace(@" ", "_");
                            if (_service.GetItemByCode(code) == null)
                            {
                                _service.CreateNewRole(item);
                                success.Add(item);
                            }
                        }
                    }
                    return new JsonResult(new { code = 200, msg = "success", data = success });
                }
                return new JsonResult(new { code = 405, msg = "User not found" });
            }
            catch (Exception ex)
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
                        if (item == null)
                        {
                            return new JsonResult(new { code = 404, msg = "Data not found" });
                        }
                        if (item.Code == User.FindFirst(ClaimTypes.Role)?.Value || item.Code == "superadmin")
                        {
                            return new JsonResult(new { code = 405, msg = "Insufficient authority" });
                        }
                        _service.Remove(id);
                        return new JsonResult(new { code = 200, msg = "delete success", data = item });
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
                            _service.UpdateRole(item);
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

        [HttpPost]
        public JsonResult Approved(string ArrID)
        {
            try
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    if (!string.IsNullOrEmpty(ArrID))
                    {
                        var item = _service.GetItemByID(ArrID);
                        if (item == null)
                        {
                            return new JsonResult(new { code = 404, msg = "Data not found" });
                        }
                        if (item.Code == User.FindFirst(ClaimTypes.Role)?.Value || item.Code == "superadmin")
                        {
                            return new JsonResult(new { code = 405, msg = "Insufficient authority" });
                        }
                        var filter = Builders<RoleEntity>.Filter.Eq(o => o.ID, item.ID);
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
        [HttpPost]
        public JsonResult CreateAccess([FromBody] List<AccessEntity> listItem)
        {
            try
            {
                Dictionary<string, string> permission = new Dictionary<string, string>();
                List<AccessEntity> success = new List<AccessEntity>();
                string role = "";
                if (User != null && User.Identity.IsAuthenticated)
                {
                    for (int i = 0; i < listItem.Count; i++)
                    {
                        var item = listItem[i];
                        role = item.RoleID;
                        var oldItem = _accessesService.CreateQuery().Find(o => o.Authority == item.Authority && o.RoleID == item.RoleID)?.ToList();
                        if (oldItem == null || oldItem.Count == 0)
                        {
                            item.UserCreate = User.FindFirst("UserID")?.Value;
                            item.CreateDate = DateTime.Now;
                            _accessesService.CreateOrUpdate(item);
                            success.Add(item);
                            if (item.IsActive)
                            {
                                permission.Add(item.Authority, "");
                            }
                        }
                        else
                        {
                            var realItem = oldItem.FirstOrDefault();
                            realItem.IsActive = item.IsActive;
                            _accessesService.Save(realItem);
                            success.Add(realItem);
                            if (item.IsActive)
                            {
                                permission.Add(item.Authority, "");
                            }
                        }
                    }
                    var currentRole = _service.GetItemByCode(role);
                    if(currentRole != null)
                    {
                        CacheExtends.SetObjectFromCache(currentRole.ID, 3600 * 24 * 360, permission.Select(o=>o.Key)?.ToList());
                    }

                    return new JsonResult(new { code = 200, msg = "success", data = success });
                }
                return new JsonResult(new { code = 405, msg = "User not found" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { code = 500, msg = ex.Message, data = ex });
            }
        }

        private void UpdateAuthority(string key, List<string> permission)
        {
            string keys = key;
            ObjectCache cache = MemoryCache.Default;
            var data = cache.Get(keys);
            if (data != null)
            {
                CacheExtends.SetObjectFromCache(keys, 3600 * 24 * 360, permission);
            }
        }
    }
}
