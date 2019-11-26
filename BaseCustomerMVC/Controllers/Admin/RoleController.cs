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

namespace BaseCustomerMVC.Controllers.Admin
{
    [BaseAccess.Attribule.AccessCtrl("Quản lý quyền", "Role", "admin")]
    public class RoleController : AdminController
    {
        private readonly IAccess _access;
        private readonly AccessesService _accessesService;
        private readonly RoleService _service;
        private readonly IHostingEnvironment _env;
        public RoleController(RoleService service
            , AccessesService accessesService
            , IAccess access
            , IHostingEnvironment evn)
        {
            _env = evn;
            _service = service;
            _access = access;
            _accessesService = accessesService;
        }
        // GET: Home

        public ActionResult Index(DefaultModel model)
        {
            ViewBag.Model = model;
            return View();
        }


        public IActionResult Access(string id)
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
        public IActionResult Access(string id, List<AccessEntity> data)
        {
            if (string.IsNullOrEmpty(id)) return View("Index");

            var assembly = GetAssembly();

            ViewBag.AdminCtrl = _access.GetAccessByAttribue<Globals.AdminController>(assembly, "admin");
            ViewBag.TeacherCtrl = _access.GetAccessByAttribue<Globals.TeacherController>(assembly, "teacher");
            ViewBag.StudentCtrl = _access.GetAccessByAttribue<Globals.StudentController>(assembly, "student");

            for(int i = 0; data != null && i < data.Count; i++)
            {
                var item = data[i];
                var oldItem = _accessesService.Collection.Find(o => o.CtrlName == item.CtrlName && o.ActName == item.ActName && o.RoleID == id)?.SingleOrDefault();
                if(oldItem != null)
                {
                    oldItem.IsActive = item.IsActive;
                    _accessesService.Collection.ReplaceOne(x=>x.ID == oldItem.ID, oldItem);
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
