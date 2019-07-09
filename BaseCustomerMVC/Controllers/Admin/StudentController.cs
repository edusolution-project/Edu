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
    [IndefindCtrlAttribulte("Quản lý học viên", "StudentManager", "admin")]
    public class StudentController : AdminController
    {
        private readonly StudentService _service;
        private readonly RoleService _roleService;
        private readonly AccountService _accountService;
        private readonly IHostingEnvironment _env;
        public StudentController(StudentService service
            , RoleService roleService
            , AccountService accountService
            , IHostingEnvironment evn)
        {
            _env = evn;
            _service = service;
            _roleService = roleService;
            _accountService = accountService;
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
            var filter = new List<FilterDefinition<StudentEntity>>();

            if (!string.IsNullOrEmpty(model.SearchText))
            {
                filter.Add(Builders<StudentEntity>.Filter.Where(o => o.FullName.ToLower().Contains(model.SearchText.ToLower()) || o.Email.ToLower().Contains(model.SearchText.ToLower()) || o.Class.Contains(model.SearchText) || o.StudentId.ToLower().Contains(model.SearchText.ToLower())));
            }
            if (model.StartDate > DateTime.MinValue)
            {
                filter.Add(Builders<StudentEntity>.Filter.Where(o => o.CreateDate >= new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0)));
            }
            if (model.EndDate > DateTime.MinValue)
            {
                filter.Add(Builders<StudentEntity>.Filter.Where(o => o.CreateDate <= new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59)));
            }
            var data = filter.Count > 0 ? _service.Collection.Find(Builders<StudentEntity>.Filter.And(filter)) : _service.GetAll();
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
            var filter = Builders<StudentEntity>.Filter.Where(o => o.ID == id);
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
        public JsonResult Create(StudentEntity item)
        {
            if (string.IsNullOrEmpty(item.ID) || item.ID == "0")
            {
                if (!ExistEmail(item.Email) && !ExistStudentId(item.StudentId))
                {
                    _service.CreateQuery().InsertOne(item);
                    Dictionary<string, object> response = new Dictionary<string, object>()
                    {
                        {"Data",item },
                        {"Error",null },
                        {"Msg","Thêm thành công" }
                    };
                    var account = new AccountEntity()
                    {
                        CreateDate = DateTime.Now,
                        IsActive = true,
                        PassTemp = Security.Encrypt(string.Format("{0:ddMMyyyy}", item.DateBorn)),
                        PassWord = Security.Encrypt(string.Format("{0:ddMMyyyy}", item.DateBorn)),
                        UserCreate = item.UserCreate,
                        Type = "student",
                        UserID = item.ID,
                        UserName = item.Email.ToLower().Trim(),
                        RoleID = _roleService.GetItemByCode("student").ID
                    };
                    _accountService.CreateQuery().InsertOne(account);
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
                    _accountService.Collection.DeleteMany(o => model.ArrID.Split(',').Contains(o.UserID));
                    var delete = _service.Collection.DeleteMany(o => model.ArrID.Split(',').Contains(o.ID));
                    return new JsonResult(delete);
                }
                else
                {
                    _accountService.Collection.DeleteMany(o => model.ArrID == o.UserID);
                    var delete = _service.Collection.DeleteMany(o => model.ArrID == o.ID);
                    return new JsonResult(delete);
                }


            }
        }

        [HttpPost]
        [Obsolete]
        public async Task<JsonResult> Import()
        {
            var form = HttpContext.Request.Form;
            if (form == null) return new JsonResult(null);
            if (form.Files == null || form.Files.Count <= 0) return new JsonResult(null);
            var file = form.Files[0];
            var filePath = Path.Combine(_env.WebRootPath, file.FileName);
            List<StudentEntity> studentList = null;
            List<StudentEntity> Error = null;
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
                stream.Close();
                try
                {
                    var readStream = new FileStream(filePath, FileMode.Open);
                    using (ExcelPackage package = new ExcelPackage(readStream))
                    {
                        ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                        int totalRows = workSheet.Dimension.Rows;
                        studentList = new List<StudentEntity>();
                        Error = new List<StudentEntity>();
                        for (int i = 1; i <= totalRows; i++)
                        {
                            if (workSheet.Cells[i, 1].Value == null || workSheet.Cells[i, 1].Value.ToString() == "STT") continue;
                            string code = workSheet.Cells[i, 2].Value == null ? "" : workSheet.Cells[i, 2].Value.ToString();
                            string name = workSheet.Cells[i, 3].Value == null ? "" : workSheet.Cells[i, 3].Value.ToString();
                            var item = new StudentEntity
                            {
                                StudentId = code,
                                FullName = name,
                                DateBorn = workSheet.Cells[i, 4].Value == null ? DateTime.MinValue : (DateTime.Parse(workSheet.Cells[i, 4].Value.ToString())),
                                Email = workSheet.Cells[i, 5].Value == null ? "" : workSheet.Cells[i, 5].Value.ToString(),
                                Class = new List<string>() { workSheet.Cells[i, 6].Value == null ? "" : workSheet.Cells[i, 6].Value.ToString() },
                                //Phone = workSheet.Cells[i, 8].Value == null ? "" : workSheet.Cells[i, 8].Value.ToString(),
                                //Address = workSheet.Cells[i, 9].Value == null ? "" : workSheet.Cells[i, 9].Value.ToString(),
                                CreateDate = DateTime.Now,
                                UserCreate = User.Claims.GetClaimByType("UserID") != null ? User.Claims.GetClaimByType("UserID").Value.ToString() : "0",
                                IsActive = true
                            };
                            if (!ExistEmail(item.Email))
                            {
                                await _service.CreateQuery().InsertOneAsync(item);
                                studentList.Add(item);
                                var account = new AccountEntity()
                                {
                                    CreateDate = DateTime.Now,
                                    IsActive = true,
                                    PassTemp = Security.Encrypt(string.Format("{0:ddMMyyyy}", item.DateBorn)),
                                    PassWord = Security.Encrypt(string.Format("{0:ddMMyyyy}", item.DateBorn)),
                                    UserCreate = item.UserCreate,
                                    Type = "student",
                                    UserID = item.ID,
                                    UserName = item.Email.ToLower().Trim(),
                                    RoleID = _roleService.GetItemByCode("student").ID
                                };
                                _accountService.CreateQuery().InsertOne(account);
                            }
                            else
                            {
                                Error.Add(item);
                            }
                        }

                    }
                    System.IO.File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    return new JsonResult(ex);
                }
            }
            Dictionary<string, object> response = new Dictionary<string, object>()
            {
                {"Data",studentList},
                {"Error",Error }
            };
            return new JsonResult(response);
        }

        [HttpGet]
        [Obsolete]
        public async Task<IActionResult> Export(DefaultModel model)
        {
            var filter = new List<FilterDefinition<StudentEntity>>();

            if (!string.IsNullOrEmpty(model.SearchText))
            {
                filter.Add(Builders<StudentEntity>.Filter.Where(o => o.FullName.Contains(model.SearchText) || o.Email.Contains(model.SearchText) || o.Class.Contains(model.SearchText) || o.StudentId.Contains(model.SearchText)));
            }
            if (model.StartDate > DateTime.MinValue)
            {
                filter.Add(Builders<StudentEntity>.Filter.Where(o => o.CreateDate >= new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0)));
            }
            if (model.EndDate > DateTime.MinValue)
            {
                filter.Add(Builders<StudentEntity>.Filter.Where(o => o.CreateDate <= new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59)));
            }
            var filterData = filter.Count > 0 ? _service.Collection.Find(Builders<StudentEntity>.Filter.And(filter)) : _service.GetAll();
            var list = await filterData.ToListAsync();
            var index = 1;
            var data = list.Select(o => new
            {
                STT = index++,
                Ma_HV = o.StudentId,
                Ho_ten = o.FullName,
                Ngay_sinh = o.DateBorn.ToLocalTime().ToString("MM/dd/yyyy"),
                Email = o.Email,
                Lop = o.Class,
                Trang_thai = o.IsActive ? "Hoạt động" : "Đang khóa"
            });
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("DS_HV");
                workSheet.Cells.LoadFromCollection(data, true);
                package.Save();
            }
            stream.Position = 0;
            string excelName = $"StudentList-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

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
                    var filter = Builders<StudentEntity>.Filter.Where(o => model.ArrID.Split(',').Contains(o.ID) && o.IsActive == false);
                    var update = Builders<StudentEntity>.Update.Set("IsActive", true);
                    var publish = _service.Collection.UpdateMany(filter, update);
                    return new JsonResult(publish);
                }
                else
                {
                    var filter = Builders<StudentEntity>.Filter.Where(o => model.ArrID == o.ID && o.IsActive == false);
                    var update = Builders<StudentEntity>.Update.Set("IsActive", true);
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
                    var filter = Builders<StudentEntity>.Filter.Where(o => model.ArrID.Split(',').Contains(o.ID) && o.IsActive == true);
                    var update = Builders<StudentEntity>.Update.Set("IsActive", false);
                    var publish = _service.Collection.UpdateMany(filter, update);
                    return new JsonResult(publish);
                }
                else
                {
                    var filter = Builders<StudentEntity>.Filter.Where(o => model.ArrID == o.ID && o.IsActive == true);
                    var update = Builders<StudentEntity>.Update.Set("IsActive", false);
                    var publish = _service.Collection.UpdateMany(filter, update);
                    return new JsonResult(publish);
                }


            }
        }

        [Obsolete]
        private bool ExistEmail(string email)
        {
            var _currentData = _service.CreateQuery().Find(o => o.Email == email);
            if (_currentData.Count() > 0)
            {
                return true;
            }
            return false;
        }
        [Obsolete]
        private bool ExistStudentId(string studentId)
        {
            var _currentData = _service.CreateQuery().Find(o => o.StudentId == studentId);
            if (_currentData.Count() > 0)
            {
                return true;
            }
            return false;
        }


    }
}
