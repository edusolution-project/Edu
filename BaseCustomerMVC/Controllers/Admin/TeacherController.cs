﻿using BaseCustomerEntity.Database;
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
    [IndefindCtrlAttribulte("Quản lý giáo viên", "TeacherManagement", "admin")]
    public class TeacherController : AdminController
    {
        private readonly SubjectService _subjectService;
        private readonly TeacherService _service;
        private readonly RoleService _roleService;
        private readonly AccountService _accountService;
        private readonly IHostingEnvironment _env;
        private readonly MappingEntity<TeacherEntity, TeacherViewModel> _mapping;

        public TeacherController(TeacherService service
            , RoleService roleService
            , AccountService accountService
            , IHostingEnvironment evn
            , SubjectService subjectService)
        {
            _env = evn;
            _service = service;
            _roleService = roleService;
            _accountService = accountService;
            _subjectService = subjectService;
            _mapping = new MappingEntity<TeacherEntity, TeacherViewModel>();
        }

        public ActionResult Index(DefaultModel model)
        {
            ViewBag.Subject = _subjectService.GetAll().ToList();
            ViewBag.Roles = _roleService.CreateQuery().Find(o => o.Type == "teacher").SortBy(o => o.Name).ToList();
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
            var teacher = data == null || data.Count() <= 0 || data.Count() < model.PageSize
                ? data
                : data.Skip((model.PageIndex - 1) * model.PageSize).Limit(model.PageSize);
            var DataResponse = teacher.ToList().Select(t => _mapping.AutoOrtherType(t, new TeacherViewModel()
            {
                SubjectList = _subjectService.CreateQuery().Find(o => t.Subjects.Contains(o.ID)).ToList()
            })).ToList();
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
        public JsonResult Create(TeacherEntity item)
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
                        Type = "teacher",
                        UserID = item.ID,
                        UserName = item.Email.ToLower().Trim(),
                        RoleID = _roleService.GetItemByCode("teacher").ID
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
                        {"Msg","Trùng email" }
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
                            string subjectname = workSheet.Cells[i, 6].Value == null ? "" : workSheet.Cells[i, 6].Value.ToString().Trim();
                            var subject = _subjectService.CreateQuery().Find(t => t.Name == subjectname).SingleOrDefault();
                            var item = new TeacherEntity
                            {
                                //TeacherId = workSheet.Cells[i, 2].Value == null ? "" : workSheet.Cells[i, 2].Value.ToString(),

                                FullName = name,
                                TeacherId = code,
                                Subjects = subject != null ? new List<string> { subject.ID } : null,
                                DateBorn = workSheet.Cells[i, 4].Value == null ? DateTime.MinValue : (DateTime)workSheet.Cells[i, 4].Value,
                                Email = workSheet.Cells[i, 5].Value == null ? "" : workSheet.Cells[i, 5].Value.ToString(),
                                Phone = workSheet.Cells[i, 7].Value == null ? "" : workSheet.Cells[i, 7].Value.ToString(),
                                Address = workSheet.Cells[i, 8].Value == null ? "" : workSheet.Cells[i, 8].Value.ToString(),
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
                                    Type = "teacher",
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
                SubjectList = _subjectService.CreateQuery().Find(o => t.Subjects.Contains(o.ID)).ToList()
            })).ToList();
            var index = 1;
            var dataResponse = dataview.Select(o => new { STT = index++, Ma_GV = o.TeacherId, Ho_ten = o.FullName, Ngay_sinh = o.DateBorn.ToLocalTime(), o.Email, Mon_hoc = o.SubjectList.Select(t => t.Name), Trang_thai = o.IsActive ? "Hoạt động" : "Đang khóa" });


            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("DS_Giaovien");
                workSheet.Cells.LoadFromCollection(dataResponse, true);
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
            if (model.ArrID.Length <= 0)
            {
                return new JsonResult(null);
            }
            else
            {
                if (model.ArrID.Contains(","))
                {
                    var filter = Builders<TeacherEntity>.Filter.Where(o => model.ArrID.Split(',').Contains(o.ID) && o.IsActive != true);
                    var update = Builders<TeacherEntity>.Update.Set("IsActive", true);
                    var publish = _service.Collection.UpdateMany(filter, update);
                    return new JsonResult(publish);
                }
                else
                {
                    var filter = Builders<TeacherEntity>.Filter.Where(o => model.ArrID == o.ID && o.IsActive != true);
                    var update = Builders<TeacherEntity>.Update.Set("IsActive", true);
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
                    var filter = Builders<TeacherEntity>.Filter.Where(o => model.ArrID.Split(',').Contains(o.ID) && o.IsActive == true);
                    var update = Builders<TeacherEntity>.Update.Set("IsActive", false);
                    var publish = _service.Collection.UpdateMany(filter, update);
                    return new JsonResult(publish);
                }
                else
                {
                    var filter = Builders<TeacherEntity>.Filter.Where(o => model.ArrID == o.ID && o.IsActive == true);
                    var update = Builders<TeacherEntity>.Update.Set("IsActive", false);
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
