using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Core_v2.Globals;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseCustomerMVC.Controllers.Admin
{
    [IndefindCtrlAttribulte("Trang Quản trị viên", "AccountManagement", "admin")]
    public class AccountController : AdminController
    {
        private readonly AccountService _service;
        private readonly TeacherService _teacherService;
        private readonly StudentService _studentService;
        private readonly RoleService _roleService;
        private readonly IHostingEnvironment _env;

        public AccountController(AccountService service
            , RoleService roleService
            , TeacherService teacherService
            , StudentService studentService
            , IHostingEnvironment evn)
        {
            _env = evn;
            _service = service;
            _teacherService = teacherService;
            _studentService = studentService;
            _roleService = roleService;
        }

        public ActionResult Index(DefaultModel model)
        {
            ViewBag.Roles = _roleService.CreateQuery().Find(_=>true).SortBy(o => o.Name).ToList();
            ViewBag.Model = model;
            return View();
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetList(DefaultModel model)
        {
            var filter = new List<FilterDefinition<AccountEntity>>();

            if (!string.IsNullOrEmpty(model.SearchText))
            {
                filter.Add(Builders<AccountEntity>.Filter.Where(o => o.UserName.ToLower().Contains(model.SearchText.ToLower())));
            }
            if (model.StartDate > DateTime.MinValue)
            {
                filter.Add(Builders<AccountEntity>.Filter.Where(o => o.CreateDate >= new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0)));
            }
            if (model.EndDate > DateTime.MinValue)
            {
                filter.Add(Builders<AccountEntity>.Filter.Where(o => o.CreateDate <= new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59)));
            }
            var data = filter.Count > 0 ? _service.Collection.Find(Builders<AccountEntity>.Filter.And(filter)) : _service.GetAll();
            model.TotalRecord = data.Count();
            var DataResponse = data == null || data.Count() <= 0 || data.Count() < model.PageSize
                ? data.ToList()
                : data.Skip((model.PageIndex - 1) * model.PageSize).Limit(model.PageSize).ToList();
            var response = new Dictionary<string, object>
            {
                { "Data", DataResponse.Select(o=> new AccountViewModel(){
                    ID = o.ID,
                    UserName = o.UserName,
                    RoleName = _roleService.GetItemByID(o.RoleID).Name ,
                    RoleID = o.RoleID,
                    Type = o.Type,
                    Name = Name(o.Type,o.UserID),
                    UserID = o.UserID,
                    UserCreate = o.UserCreate,
                    IsActive = o.IsActive
                } ) },
                { "Model", model }
            };
            return new JsonResult(response);

        }
        private string Name(string type,string id)
        {
            if(type == "teacher")
            {
                return _teacherService.GetItemByID(id).FullName;
            }
            if (type == "student")
            {
                return _studentService.GetItemByID(id).FullName;
            }
            return "admin";
        }
        [System.Obsolete]
        [HttpPost]
        public JsonResult GetDetails(string id)
        {
            var filter = Builders<AccountEntity>.Filter.Where(o => o.ID == id);
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
        public JsonResult Create(AccountEntity item)
        {
            if (string.IsNullOrEmpty(item.ID) || item.ID == "0")
            {
                if (!ExistUserName(item.UserName))
                {
                    item.Type = _roleService.GetItemByID(item.RoleID).Type;
                    item.CreateDate = DateTime.Now;
                    item.UserCreate = User.Claims.GetClaimByType("UserID").Value;
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
                        {"Error",item },
                        {"Msg","Trùng email hoặc mã sinh viên" }
                    };
                    return new JsonResult(response);
                }
            }
            else
            {
                var oldData = _service.GetItemByID(item.ID);
                if (oldData == null) return new JsonResult(null);
                item.UserID = oldData.UserID;
                item.UserCreate = User.Claims.GetClaimByType("UserID").Value;
                item.Type = _roleService.GetItemByID(item.RoleID).Type;
                
                if (string.IsNullOrEmpty(item.PassWord) || Security.Encrypt(item.PassWord) == oldData.PassWord) item.PassWord = oldData.PassWord;
                else item.PassWord = Security.Encrypt(item.PassWord);
                if (string.IsNullOrEmpty(item.PassTemp) || Security.Encrypt(item.PassTemp) == oldData.PassTemp) item.PassTemp = oldData.PassTemp;
                else item.PassTemp = Security.Encrypt(item.PassTemp);

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
                    var delete = _service.Collection.DeleteMany(o => model.ArrID == o.ID);
                    return new JsonResult(delete);
                }


            }
        }
        [HttpGet]
        [Obsolete]
        public async Task<IActionResult> Export(DefaultModel model)
        {
            var filter = new List<FilterDefinition<AccountEntity>>();

            if (!string.IsNullOrEmpty(model.SearchText))
            {
                filter.Add(Builders<AccountEntity>.Filter.Where(o => o.UserName.Contains(model.SearchText)));
            }
            if (model.StartDate > DateTime.MinValue)
            {
                filter.Add(Builders<AccountEntity>.Filter.Where(o => o.CreateDate >= new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0)));
            }
            if (model.EndDate > DateTime.MinValue)
            {
                filter.Add(Builders<AccountEntity>.Filter.Where(o => o.CreateDate <= new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59)));
            }
            var filterData = filter.Count > 0 ? _service.Collection.Find(Builders<AccountEntity>.Filter.And(filter)) : _service.GetAll();
            var list = await filterData.ToListAsync();
            var data = list.Select(o => new { o.UserName, o.Type, o.IsActive, _roleService.GetItemByID(o.RoleID).Name});
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells.LoadFromCollection(data, true);
                package.Save();
            }
            stream.Position = 0;
            string excelName = $"Account_List-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

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
                    var filter = Builders<AccountEntity>.Filter.Where(o => model.ArrID.Split(',').Contains(o.ID) && o.IsActive == false);
                    var update = Builders<AccountEntity>.Update.Set("IsActive", true);
                    var publish = _service.Collection.UpdateMany(filter, update);
                    return new JsonResult(publish);
                }
                else
                {
                    var filter = Builders<AccountEntity>.Filter.Where(o => model.ArrID == o.ID && o.IsActive == false);
                    var update = Builders<AccountEntity>.Update.Set("IsActive", true);
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
                    var filter = Builders<AccountEntity>.Filter.Where(o => model.ArrID.Split(',').Contains(o.ID) && o.IsActive == true);
                    var update = Builders<AccountEntity>.Update.Set("IsActive", false);
                    var publish = _service.Collection.UpdateMany(filter, update);
                    return new JsonResult(publish);
                }
                else
                {
                    var filter = Builders<AccountEntity>.Filter.Where(o => model.ArrID == o.ID && o.IsActive == true);
                    var update = Builders<AccountEntity>.Update.Set("IsActive", false);
                    var publish = _service.Collection.UpdateMany(filter, update);
                    return new JsonResult(publish);
                }


            }
        }

        [Obsolete]
        private bool ExistUserName(string UserName)
        {
            var _currentData = _service.CreateQuery().Find(o => o.UserName == UserName);
            if (_currentData.Count() > 0)
            {
                return true;
            }
            return false;
        }
    }
}
