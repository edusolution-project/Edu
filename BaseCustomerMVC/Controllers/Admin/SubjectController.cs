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

namespace BaseCustomerMVC.Controllers.Admin
{
    [BaseAccess.Attribule.AccessCtrl("Quản lý chương trình", "admin", 6)]
    public class SubjectController : AdminController
    {
        private readonly SubjectService _service;
        private readonly IHostingEnvironment _env;
        public SubjectController(SubjectService service
            , RoleService roleService
            , AccountService accountService
            , IHostingEnvironment evn)
        {
            _env = evn;
            _service = service;
        }
        // GET: Home

        public ActionResult Index(DefaultModel model)
        {
            ViewBag.Model = model;
            return View();
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetList(DefaultModel model)
        {
            var filter = new List<FilterDefinition<SubjectEntity>>();

            if (!string.IsNullOrEmpty(model.SearchText))
            {
                filter.Add(Builders<SubjectEntity>.Filter.Where(o => o.Name.ToLower().Contains(model.SearchText.ToLower()) || o.Code.ToLower().Contains(model.SearchText.ToLower())));
            }
            if (model.StartDate > DateTime.MinValue)
            {
                filter.Add(Builders<SubjectEntity>.Filter.Where(o => o.Created >= new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0)));
            }
            if (model.EndDate > DateTime.MinValue)
            {
                filter.Add(Builders<SubjectEntity>.Filter.Where(o => o.Created <= new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59)));
            }
            var data = filter.Count > 0 ? _service.Collection.Find(Builders<SubjectEntity>.Filter.And(filter)) : _service.GetAll();
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

        [System.Obsolete]
        [HttpPost]
        public JsonResult GetDetails(string id)
        {
            var filter = Builders<SubjectEntity>.Filter.Where(o => o.ID == id);
            var data = _service.Collection.Find(filter);
            var DataResponse = data == null || data.Count() <= 0 ? null : data.First();
            var response = new Dictionary<string, object>
            {
                { "Data", DataResponse }
            };
            return new JsonResult(response);

        }

        [HttpPost]
        [Obsolete]
        public JsonResult Create(SubjectEntity item)
        {
            if (string.IsNullOrEmpty(item.ID) || item.ID == "0")
            {
                //if(string.IsNullOrEmpty(item.Code))
                //{
                //     Dictionary<string, object> response = new Dictionary<string, object>()
                //    {
                //        {"Data",null },
                //        {"Error",item },
                //        {"Msg","Chưa nhập mã môn học" }
                //    };
                //    return new JsonResult(response);
                //}
                
                if (!Exist(item.Name))
                {
                    item.ID = null;
                    item.IsAdmin = false;
                    item.Updated = DateTime.Now;
                    item.Created = DateTime.Now;
                    _service.CreateQuery().InsertOne(item);
                    Dictionary<string, object> response = new Dictionary<string, object>()
                    {
                        {"Data",item },
                        {"Error",null },
                        {"Msg","Thêm thành công" }
                    };
                    return new JsonResult(response);
                }
                else
                {
                    Dictionary<string, object> response = new Dictionary<string, object>()
                    {
                        {"Data",null },
                        {"Error", "Môn học đã tồn tại" },
                        {"Msg","" }
                    };
                    return new JsonResult(response);
                }
            }
            else
            {
                var oldData = _service.GetItemByID(item.ID);
                if (oldData == null) return new JsonResult(null);
                item.Created = oldData.Created;
                item.IsAdmin = false;
                item.Updated = DateTime.Now;
                _service.CreateQuery().ReplaceOne(o => o.ID == item.ID, item);

                Dictionary<string, object> response = new Dictionary<string, object>()
                {
                    {"Data",item },
                    {"Error",null },
                    {"Msg","Cập nhập thành công" }
                };
                return new JsonResult(response);
            }
        }

        [HttpPost]
        [Obsolete]
        public JsonResult Delete(DefaultModel model)
        {
            if (model.ArrID.Length <= 0)
            {
                return new JsonResult(null);
            }
            else
            {
                if (model.ArrID.Contains(","))
                {
                    var delete = _service.Collection.DeleteMany(o => model.ArrID.Split(',').Contains(o.ID));
                    return new JsonResult(delete);
                }
                else
                {
                    var delete = _service.Collection.DeleteMany(o => model.ArrID==o.ID);
                    return new JsonResult(delete);
                }
                    
                
            }
        }
        
        [HttpGet]
        [Obsolete]
        public async Task<IActionResult> Export(DefaultModel model)
        {
            var filter = new List<FilterDefinition<SubjectEntity>>();

            if (!string.IsNullOrEmpty(model.SearchText))
            {
                filter.Add(Builders<SubjectEntity>.Filter.Where(o => o.Name.Contains(model.SearchText) || o.Code.Contains(model.SearchText)));
            }
            if (model.StartDate > DateTime.MinValue)
            {
                filter.Add(Builders<SubjectEntity>.Filter.Where(o => o.Created >= new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0)));
            }
            if (model.EndDate > DateTime.MinValue)
            {
                filter.Add(Builders<SubjectEntity>.Filter.Where(o => o.Created <= new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59)));
            }
            var filterData = filter.Count > 0 ? _service.Collection.Find(Builders<SubjectEntity>.Filter.And(filter)) : _service.GetAll();
            var list = await filterData.ToListAsync();
            var data = list.Select(o => new { o.Name,o.Code,o.Created,o.IsActive });
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells.LoadFromCollection(data, true);
                package.Save();
            }
            stream.Position = 0;
            string excelName = $"SubjectList-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

            //return File(stream, "application/octet-stream", excelName);  
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
       
        [HttpPost]
        [Obsolete]
        public JsonResult Publish(DefaultModel model)
        {
            if (model.ArrID.Length <= 0)
            {
                return new JsonResult(null);
            }
            else
            {
                if (model.ArrID.Contains(","))
                {
                    var filter = Builders<SubjectEntity>.Filter.Where(o => model.ArrID.Split(',').Contains(o.ID) && o.IsActive == false);
                    var update = Builders<SubjectEntity>.Update.Set("IsActive",true);
                    var publish = _service.Collection.UpdateMany(filter,update);
                    return new JsonResult(publish);
                }
                else
                {
                    var filter = Builders<SubjectEntity>.Filter.Where(o => model.ArrID == o.ID && o.IsActive == false);
                    var update = Builders<SubjectEntity>.Update.Set("IsActive", true);
                    var publish = _service.Collection.UpdateMany(filter, update);
                    return new JsonResult(publish);
                }


            }
        }
        
        [HttpPost]
        [Obsolete]
        public JsonResult UnPublish(DefaultModel model)
        {
            if (model.ArrID.Length <= 0)
            {
                return new JsonResult(null);
            }
            else
            {
                if (model.ArrID.Contains(","))
                {
                    var filter = Builders<SubjectEntity>.Filter.Where(o => model.ArrID.Split(',').Contains(o.ID) && o.IsActive == true);
                    var update = Builders<SubjectEntity>.Update.Set("IsActive", false);
                    var publish = _service.Collection.UpdateMany(filter, update);
                    return new JsonResult(publish);
                }
                else
                {
                    var filter = Builders<SubjectEntity>.Filter.Where(o => model.ArrID == o.ID && o.IsActive == true);
                    var update = Builders<SubjectEntity>.Update.Set("IsActive", false);
                    var publish = _service.Collection.UpdateMany(filter, update);
                    return new JsonResult(publish);
                }


            }
        }

        [Obsolete]
        private bool Exist(string name)
        {
            var _currentData = _service.CreateQuery().Find(o => o.Name == name);
            if(_currentData.Count() > 0)
            {
                return true;
            }
            return false;
        }

    }
}
