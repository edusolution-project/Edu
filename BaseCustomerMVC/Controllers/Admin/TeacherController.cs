using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Core_v2.Globals;
using Core_v2.Interfaces;
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
using Core_v2.Globals;

namespace BaseCustomerMVC.Controllers.Admin
{
    [BaseAccess.Attribule.AccessCtrl("Quản lý giáo viên", "admin", 2)]
    public class TeacherController : AdminController
    {
        private readonly SubjectService _subjectService;
        private readonly TeacherService _service;
        private readonly RoleService _roleService;
        private readonly AccountService _accountService;
        private readonly IHostingEnvironment _env;
        private readonly MappingEntity<TeacherEntity, TeacherViewModel> _mapping;
        private readonly UserAndRoleService _userAndRoleService;
        private readonly TeacherHelper _teacherHelper;

        public TeacherController(TeacherService service
            , RoleService roleService
            , AccountService accountService
            , IHostingEnvironment evn
            , SubjectService subjectService,
            UserAndRoleService userAndRoleService)
        {
            _env = evn;
            _service = service;
            _roleService = roleService;
            _accountService = accountService;
            _subjectService = subjectService;
            _mapping = new MappingEntity<TeacherEntity, TeacherViewModel>();

            _teacherHelper = new TeacherHelper(service, accountService);
            _userAndRoleService = userAndRoleService;
        }

        public ActionResult Index(DefaultModel model)
        {
            ViewBag.Subject = _subjectService.GetAll().ToList();
            var roleList = new List<string> { "teacher", "head-teacher" };
            ViewBag.Roles = _roleService.CreateQuery().Find(o => roleList.Contains(o.Code)).ToList();
            ViewBag.Model = model;
            return View();
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetList(DefaultModel model)
        {
            var filter = new List<FilterDefinition<TeacherEntity>>();

            if (!string.IsNullOrEmpty(model.SearchText))
            {
                filter.Add(Builders<TeacherEntity>.Filter.Where(o => o.FullName.ToLower().Contains(model.SearchText.ToLower()) || o.Email.ToLower().Contains(model.SearchText.ToLower()) || o.Subjects.Contains(model.SearchText) || o.TeacherId.ToLower().Contains(model.SearchText.ToLower())));
            }
            if (model.StartDate > DateTime.MinValue)
            {
                filter.Add(Builders<TeacherEntity>.Filter.Where(o => o.CreateDate >= new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0)));
            }
            if (model.EndDate > DateTime.MinValue)
            {
                filter.Add(Builders<TeacherEntity>.Filter.Where(o => o.CreateDate <= new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59)));
            }
            var data = filter.Count > 0 ? _service.Collection.Find(Builders<TeacherEntity>.Filter.And(filter)) : _service.GetAll();
            model.TotalRecord = data.Count();
            var teachers = data == null || data.Count() <= 0 || data.Count() < model.PageSize
                ? data
                : data.Skip((model.PageIndex - 1) * model.PageSize).Limit(model.PageSize);

            var roleList = new List<string> { "teacher", "head-teacher" };
            var roles = _roleService.CreateQuery().Find(o => roleList.Contains(o.Code)).ToList();

            var DataResponse =

                from t in teachers.ToList()
                let account = _accountService.CreateQuery().Find(o => o.UserID == t.ID && o.Type == ACCOUNT_TYPE.TEACHER).FirstOrDefault()
                where account != null
                let role = roles.Find(r => r.ID == account.RoleID)
                where role != null
                select _mapping.AutoOrtherType(t, new TeacherViewModel()
                {
                    SubjectList = t.Subjects == null ? null : _subjectService.CreateQuery().Find(o => t.Subjects.Contains(o.ID)).ToList(),
                    RoleID = role.ID,
                    RoleName = role.Name,
                    AccountID = account.ID
                });

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
            var filter = Builders<TeacherEntity>.Filter.Where(o => o.ID == id);
            var data = _service.Collection.Find(filter).First();

            var roleList = new List<string> { "teacher", "head-teacher" };
            var roles = _roleService.CreateQuery().Find(o => roleList.Contains(o.Code)).ToList();
            TeacherViewModel teacher = null;

            if (data != null)
            {
                var account = _accountService.CreateQuery().Find(o => o.UserID == data.ID && o.Type == ACCOUNT_TYPE.TEACHER).First();
                var role = roles.Find(r => r.ID == account.RoleID);
                teacher = _mapping.AutoOrtherType(data, new TeacherViewModel()
                {
                    SubjectList = data.Subjects == null ? null : _subjectService.CreateQuery().Find(o => data.Subjects.Contains(o.ID)).ToList(),
                    RoleID = role?.ID,
                    RoleName = role?.Name,
                    AccountID = account?.ID
                });
            }

            var response = new Dictionary<string, object>
            {
                { "Data", teacher }
            };
            return new JsonResult(response);

        }

        [HttpPost]
        [Obsolete]
        public JsonResult Create(TeacherEntity item, string RoleID, string Basis)
        {
            if (string.IsNullOrEmpty(item.ID) || item.ID == "0")
            {
                if (string.IsNullOrEmpty(item.Email))
                {
                    Dictionary<string, object> response = new Dictionary<string, object>()
                    {
                        {"Data",null },
                        {"Error",item },
                        {"Msg","Email không được để trống" }
                    };
                    return new JsonResult(response);
                }
                if (!ExistEmail(item.Email) //&& !ExistTeacherId(item.TeacherId)
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
                        PassTemp = Security.Encrypt(string.Format("{0:ddMMyyyy}", item.DateBorn)),
                        PassWord = Security.Encrypt(string.Format("{0:ddMMyyyy}", item.DateBorn)),
                        UserCreate = item.UserCreate,
                        Type = ACCOUNT_TYPE.TEACHER,
                        UserID = item.ID,
                        UserName = item.Email.ToLower().Trim(),
                        RoleID = RoleID
                    };
                    _accountService.CreateOrUpdate(account);
                    var role = _roleService.GetItemByID(RoleID);
                    if(role != null)
                    {
                        var userAndRole = new UserAndRoleEntity()
                        {
                            Role = role.Code,
                            UserID = item.ID,
                            Basis = Basis
                        };
                        _userAndRoleService.SaveRole(userAndRole);
                    }

                    
                    return new JsonResult(response);
                }
                else
                {
                    Dictionary<string, object> response = new Dictionary<string, object>()
                    {
                        {"Data",null },
                        {"Error",item },
                        {"Msg","Trùng email" }
                    };
                    return new JsonResult(response);
                }
            }
            else
            {
                var oldData = _service.GetItemByID(item.ID);
                if (oldData == null) return new JsonResult(null);
                item.ID = oldData.ID;
                item.UserCreate = oldData.UserCreate;
                item.CreateDate = oldData.CreateDate;
                item.Address = oldData.Address;
                _service.CreateOrUpdate(item);

                var oldAccount = _accountService.CreateQuery().Find(t => t.UserID == item.ID && t.Type == ACCOUNT_TYPE.TEACHER).SingleOrDefault();
                if (oldAccount == null) return new JsonResult(null);
                oldAccount.RoleID = RoleID;
                oldAccount.IsActive = item.IsActive;
                _accountService.CreateOrUpdate(oldAccount);

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
            var filePath = Path.Combine(_env.WebRootPath, file.FileName + DateTime.Now.ToString("ddMMyyyyhhmmss"));
            List<TeacherEntity> importlist = null;
            List<TeacherEntity> Error = null;
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
                        importlist = new List<TeacherEntity>();
                        Error = new List<TeacherEntity>();
                        for (int i = 1; i <= totalRows; i++)
                        {
                            if (workSheet.Cells[i, 1].Value == null || workSheet.Cells[i, 1].Value.ToString() == "STT") continue;
                            //string ho = workSheet.Cells[i, 3].Value == null ? "" : workSheet.Cells[i, 3].Value.ToString();
                            //string name = workSheet.Cells[i, 4].Value == null ? "" : workSheet.Cells[i, 4].Value.ToString();
                            string code = workSheet.Cells[i, 2].Value == null ? "" : workSheet.Cells[i, 2].Value.ToString();
                            string name = workSheet.Cells[i, 3].Value == null ? "" : workSheet.Cells[i, 3].Value.ToString();
                            string subjectname = workSheet.Cells[i, 8].Value == null ? "" : workSheet.Cells[i, 8].Value.ToString().Trim();
                            var subject = _subjectService.CreateQuery().Find(t => subjectname.Split(',').Contains(t.Name.ToLower().Trim())).ToList();
                            var item = new TeacherEntity
                            {
                                FullName = name,
                                TeacherId = code,
                                Subjects = (subject != null && subject.Count > 0 ? subject.Select(s => s.ID).ToList() : null),
                                DateBorn = workSheet.Cells[i, 4].Value == null ? DateTime.MinValue : (DateTime.Parse(workSheet.Cells[i, 4].Value.ToString())),
                                Email = workSheet.Cells[i, 5].Value == null ? "" : workSheet.Cells[i, 5].Value.ToString(),
                                Phone = workSheet.Cells[i, 6].Value == null ? "" : workSheet.Cells[i, 6].Value.ToString(),
                                Address = workSheet.Cells[i, 7].Value == null ? "" : workSheet.Cells[i, 7].Value.ToString(),
                                CreateDate = DateTime.Now,
                                UserCreate = User.Claims.GetClaimByType("UserID") != null ? User.Claims.GetClaimByType("UserID").Value.ToString() : "0",
                                IsActive = true
                            };

                            if (!ExistEmail(item.Email))
                            {
                                await _service.CreateQuery().InsertOneAsync(item);
                                importlist.Add(item);
                                var account = new AccountEntity()
                                {
                                    CreateDate = DateTime.Now,
                                    IsActive = true,
                                    PassTemp = Security.Encrypt(string.Format("{0:ddMMyyyy}", item.DateBorn)),
                                    PassWord = Security.Encrypt(string.Format("{0:ddMMyyyy}", item.DateBorn)),
                                    UserCreate = item.UserCreate,
                                    Type = ACCOUNT_TYPE.TEACHER,
                                    UserID = item.ID,
                                    UserName = item.Email.ToLower().Trim(),
                                    RoleID = _roleService.GetItemByCode("teacher").ID
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
                {"Data",importlist},
                {"Error",Error }
            };
            return new JsonResult(response);
        }

        [HttpGet]
        [Obsolete]
        public async Task<IActionResult> Export(DefaultModel model)
        {
            var filter = new List<FilterDefinition<TeacherEntity>>();

            if (!string.IsNullOrEmpty(model.SearchText))
            {
                filter.Add(Builders<TeacherEntity>.Filter.Where(o => o.FullName.Contains(model.SearchText) || o.Email.Contains(model.SearchText) || o.Subjects.Contains(model.SearchText) || o.TeacherId.Contains(model.SearchText)));
            }
            if (model.StartDate > DateTime.MinValue)
            {
                filter.Add(Builders<TeacherEntity>.Filter.Where(o => o.CreateDate >= new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0)));
            }
            if (model.EndDate > DateTime.MinValue)
            {
                filter.Add(Builders<TeacherEntity>.Filter.Where(o => o.CreateDate <= new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59)));
            }
            var filterData = filter.Count > 0 ? _service.Collection.Find(Builders<TeacherEntity>.Filter.And(filter)) : _service.GetAll();
            var list = await filterData.ToListAsync();
            //var data = list.Select(o => new { o.TeacherId, o.FullName, o.DateBorn, o.Email, o.Subjects, o.IsActive });
            var stream = new MemoryStream();
            var dataview = list.Select(t => _mapping.AutoOrtherType(t, new TeacherViewModel()
            {
                SubjectList = t.Subjects != null ? _subjectService.CreateQuery().Find(o => t.Subjects.Contains(o.ID)).ToList() : new List<SubjectEntity>()
            })).ToList();
            var index = 0;
            var dataResponse = dataview.Select(o => new
            {
                STT = index++,
                Ma_GV = o.TeacherId,
                Ho_ten = o.FullName,
                Ngay_sinh = o.DateBorn.ToLocalTime().ToString("MM/dd/yyyy"),
                o.Email,
                Dien_thoai = o.Phone,
                Dia_chi = o.Address,
                Chuyen_mon = ((o.SubjectList != null && o.SubjectList.Count > 0) ? o.SubjectList.Select(t => t.Name).Aggregate(func: (result, item) => result + "," + item) : ""),
                Trang_thai = o.IsActive ? "Hoạt động" : "Đang khóa"
            });


            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("DS_Giaovien");
                workSheet.Cells.LoadFromCollection(dataResponse, true);
                workSheet.Column(3).Style.Numberformat.Format = "MM/dd/yyyy";
                package.Save();
            }
            stream.Position = 0;
            string excelName = $"TeacherList-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

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
                _teacherHelper.ChangeStatus(model.ArrID.Split(','), status);
            else
                _teacherHelper.ChangeStatus(model.ArrID, status);
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
        private bool ExistTeacherId(string TeacherId)
        {
            var _currentData = _service.CreateQuery().Find(o => o.TeacherId == TeacherId);
            if (_currentData.Count() > 0)
            {
                return true;
            }
            return false;
        }

    }
}
