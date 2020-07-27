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
using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace BaseCustomerMVC.Controllers.Admin
{
    [BaseAccess.Attribule.AccessCtrl("Quản lý học viên", "admin", 3)]
    public class StudentController : AdminController
    {
        private readonly StudentService _service;
        private readonly RoleService _roleService;
        private readonly AccountService _accountService;
        private readonly CenterService _centerService;
        private readonly IHostingEnvironment _env;
        private readonly MappingEntity<StudentEntity, StudentViewModel> _mapping;

        private readonly StudentHelper _studentHelper;
        private IConfiguration _configuration;
        private readonly string _defaultPass;

        public StudentController(StudentService service
            , RoleService roleService
            , AccountService accountService
            , StudentService studentService
            , CenterService centerService
            , IHostingEnvironment evn
            , IConfiguration iConfig
            )
        {
            _env = evn;
            _service = service;
            _roleService = roleService;
            _accountService = accountService;

            _studentHelper = new StudentHelper(studentService, accountService);
            _mapping = new MappingEntity<StudentEntity, StudentViewModel>();
            _configuration = iConfig;
            _defaultPass = _configuration.GetValue<string>("SysConfig:DP");
            _centerService = centerService;
        }
        // GET: Home

        public ActionResult Index(DefaultModel model)
        {
            ViewBag.Model = model;
            ViewBag.Centers = _centerService.GetAll().ToList();
            ViewBag.Role = _roleService.CreateQuery().Find(o => o.Code == "student").SingleOrDefault();
            return View();
        }

        [HttpPost]
        public JsonResult GetList(DefaultModel model, string Center)
        {
            var filter = new List<FilterDefinition<StudentEntity>>();

            if (!string.IsNullOrEmpty(model.SearchText))
            {
                filter.Add(Builders<StudentEntity>.Filter.Where(o => o.FullName.ToLower().Contains(model.SearchText.ToLower()) ||
                o.Email.ToLower().Contains(model.SearchText.ToLower()) ||
                o.Class.Contains(model.SearchText) ||
                o.StudentId.ToLower().Contains(model.SearchText.ToLower())));
            }
            if (model.StartDate > DateTime.MinValue)
            {
                filter.Add(Builders<StudentEntity>.Filter.Where(o => o.CreateDate >= new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0)));
            }
            if (model.EndDate > DateTime.MinValue)
            {
                filter.Add(Builders<StudentEntity>.Filter.Where(o => o.CreateDate <= new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59)));
            }
            if (!String.IsNullOrEmpty(Center))
            {
                filter.Add(Builders<StudentEntity>.Filter.Where(o => o.Centers.Any(t => t == Center)));
            }

            var data = (filter.Count > 0 ? _service.Collection.Find(Builders<StudentEntity>.Filter.And(filter)) : _service.GetAll())
                .SortByDescending(t => t.ID);
            model.TotalRecord = data.CountDocuments();
            var DataResponse = data == null || model.TotalRecord <= 0 || model.TotalRecord <= model.PageSize
                ? data.ToList()
                : data.Skip((model.PageIndex) * model.PageSize).Limit(model.PageSize).ToList();

            var students = from r in DataResponse
                           let account = _accountService.CreateQuery().Find(o => o.UserName == r.Email
                           //&& o.Type == ACCOUNT_TYPE.STUDENT
                           ).FirstOrDefault()
                           //where account != null
                           select _mapping.AutoOrtherType(r, new StudentViewModel()
                           {
                               AccountID = account?.ID
                           });

            var response = new Dictionary<string, object>
            {
                { "Data", students },
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
            var student = data == null || data.Count() <= 0 ? null : data.First();
            var account = _accountService.CreateQuery().Find(o => o.UserID == student.ID
            //&& o.Type == ACCOUNT_TYPE.STUDENT
            ).First();
            var response = new Dictionary<string, object>
            {
                { "Data", _mapping.AutoOrtherType(student, new StudentViewModel()
                    {
                    AccountID = account.ID
                    })
                }
            };
            return new JsonResult(response);
        }

        [HttpPost]
        [Obsolete]
        public JsonResult Create(StudentEntity item)
        {

            if (string.IsNullOrEmpty(item.ID) || item.ID == "0")
            {
                if (!ExistEmail(item.Email)
                    //&& !ExistStudentId(item.StudentId)
                    )
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
                        PassTemp = Core_v2.Globals.Security.Encrypt(_defaultPass),
                        PassWord = Core_v2.Globals.Security.Encrypt(_defaultPass),
                        UserCreate = item.UserCreate,
                        Type = ACCOUNT_TYPE.STUDENT,
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
                        {"Msg","Email đã được sử dụng" }
                    };
                    return new JsonResult(response);
                }
            }
            else
            {
                var oldData = _service.GetItemByID(item.ID);
                if (oldData == null) return new JsonResult(null);
                oldData.Centers = item.Centers;
                oldData.FullName = item.FullName;
                oldData.DateBorn = item.DateBorn;
                oldData.Phone = item.Phone;
                oldData.IsActive = item.IsActive;
                _service.CreateQuery().ReplaceOne(o => o.ID == item.ID, oldData);

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
        public async Task<JsonResult> Import(string Center)
        {
            var form = HttpContext.Request.Form;
            if (form == null) return new JsonResult(null);
            if (form.Files == null || form.Files.Count <= 0) return new JsonResult(null);
            var file = form.Files[0];
            var dirPath = _env.WebRootPath + "\\Temp";
            if (!System.IO.Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            var filePath = Path.Combine(dirPath, file.FileName);

            List<StudentEntity> studentList = null;
            List<StudentEntity> Error = null;

            if (!string.IsNullOrEmpty(Center))
            {
                var _center = _centerService.GetItemByID(Center);
                if (_center == null) return new JsonResult("Cơ sở không đúng");
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
                stream.Close();
                try
                {
                    using (var readStream = new FileStream(filePath, FileMode.Open))
                    {
                        using (ExcelPackage package = new ExcelPackage(readStream))
                        {
                            ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                            int totalRows = workSheet.Dimension.Rows;
                            studentList = new List<StudentEntity>();
                            Error = new List<StudentEntity>();
                            for (int i = 1; i <= totalRows; i++)
                            {
                                if (workSheet.Cells[i, 1].Value == null || workSheet.Cells[i, 1].Value.ToString() == "STT") continue;
                                if (workSheet.Cells[i, 4].Value == null) continue; // Email null;
                                                                                   //string code = workSheet.Cells[i, 2].Value == null ? "" : workSheet.Cells[i, 2].Value.ToString();
                                string name = workSheet.Cells[i, 2].Value == null ? "" : workSheet.Cells[i, 2].Value.ToString();
                                string dateStr = workSheet.Cells[i, 3].Value == null ? "" : workSheet.Cells[i, 3].Value.ToString();
                                string email = (workSheet.Cells[i, 4].Value == null ? "" : workSheet.Cells[i, 4].Value.ToString()).ToLower().Trim();
                                var birthdate = new DateTime();
                                DateTime.TryParseExact(dateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                                                   DateTimeStyles.None,
                                                   out birthdate);
                                var phone = workSheet.Cells[i, 5].Value == null ? "" : workSheet.Cells[i, 5].Value.ToString();
                                var skype = workSheet.Cells[i, 6].Value == null ? "" : workSheet.Cells[i, 6].Value.ToString();


                                var acc = _accountService.GetAccountByEmail(email);
                                var item = new StudentEntity
                                {
                                    //StudentId = code,
                                    FullName = name,
                                    DateBorn = birthdate,
                                    Email = email,
                                    Phone = phone,
                                    Skype = skype,
                                    CreateDate = DateTime.Now,
                                    UserCreate = User.Claims.GetClaimByType("UserID") != null ? User.Claims.GetClaimByType("UserID").Value.ToString() : "0",
                                    IsActive = true,
                                    Centers = String.IsNullOrEmpty(Center) ? null : new List<string> { Center }
                                };
                                if (acc == null)
                                {
                                    await _service.CreateQuery().InsertOneAsync(item);
                                    studentList.Add(item);
                                    var account = new AccountEntity()
                                    {
                                        CreateDate = DateTime.Now,
                                        IsActive = true,
                                        PassTemp = Core_v2.Globals.Security.Encrypt(_defaultPass),
                                        PassWord = Core_v2.Globals.Security.Encrypt(_defaultPass),
                                        UserCreate = item.UserCreate,
                                        Type = ACCOUNT_TYPE.STUDENT,
                                        UserID = item.ID,
                                        UserName = item.Email.ToLower().Trim(),
                                        RoleID = _roleService.GetItemByCode("student").ID
                                    };
                                    _accountService.CreateQuery().InsertOne(account);
                                }
                                else
                                {
                                    if (acc.Type != ACCOUNT_TYPE.STUDENT && !ExistEmail(email))
                                    {
                                        await _service.CreateQuery().InsertOneAsync(item);
                                        studentList.Add(item);
                                    }
                                    else
                                    {
                                        var oldStudent = _service.GetStudentByEmail(item.Email);
                                        if (oldStudent != null)
                                        {
                                            if (oldStudent.Centers == null)
                                                oldStudent.Centers = item.Centers;
                                            else if (!oldStudent.Centers.Contains(Center))
                                                oldStudent.Centers.Add(Center);
                                            _service.Save(oldStudent);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    System.IO.File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    return new JsonResult(ex.Message);
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
        public async Task<IActionResult> Export(DefaultModel model, string Center)
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
            if (!String.IsNullOrEmpty(Center))
            {
                filter.Add(Builders<StudentEntity>.Filter.Where(o => o.Centers.Contains(Center)));
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

        [HttpGet]
        [Obsolete]
        public IActionResult ExportTemplate(DefaultModel model)
        {
            var list = new List<StudentEntity>() { new StudentEntity() {
                ID = "undefined"
                } };
            var data = list.Select(o => new
            {
                STT = 1,
                Ho_ten = "Nguyễn Văn A",
                Ngay_sinh = "dd/mm/yyyy",
                Email = "email@gmail.com",
                SDT = "0123456789",
                SkypeId = "skypeid"
            });
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("DS_HV");
                workSheet.Cells.LoadFromCollection(data, true);
                package.Save();
            }
            stream.Position = 0;
            string excelName = $"StudentTemplate.xlsx";

            //return File(stream, "application/octet-stream", excelName);  
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        [HttpPost]
        [Obsolete]
        public JsonResult Publish(DefaultModel model)
        {
            if (string.IsNullOrEmpty(model.ArrID) || model.ArrID.Length <= 0)
                return new JsonResult(null);

            ChangeStatus(model, true);
            return new JsonResult("Publish OK");
        }

        [HttpPost]
        [Obsolete]
        public JsonResult UnPublish(DefaultModel model)
        {
            if (string.IsNullOrEmpty(model.ArrID) || model.ArrID.Length <= 0)
                return new JsonResult(null);

            ChangeStatus(model, false);
            return new JsonResult("UnPublish OK");
        }

        private void ChangeStatus(DefaultModel model, bool status)
        {
            if (model.ArrID.Contains(","))
                _studentHelper.ChangeStatus(model.ArrID.Split(','), status);
            else
                _studentHelper.ChangeStatus(model.ArrID, status);
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
