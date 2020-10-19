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
using BaseCustomerEntity.Globals;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace BaseCustomerMVC.Controllers.Admin
{
    [BaseAccess.Attribule.AccessCtrl("Quản lý cơ sở", "admin", 2)]
    public class CenterController : AdminController
    {
        private readonly CenterService _service;
        private readonly RoleService _roleService;
        private readonly AccountService _accountService;
        private readonly IHostingEnvironment _env;
        private readonly MappingEntity<StudentEntity, StudentViewModel> _mapping;
        private readonly StudentService _studentService;
        private readonly StudentHelper _studentHelper;
        private readonly TeacherService _teacherService;
        private readonly FileProcess _fileProcess;

        public CenterController(
            CenterService service,
            RoleService roleService,
            AccountService accountService,
            TeacherService teacherService,
            StudentHelper studentHelper,
            FileProcess fileProcess,
            IHostingEnvironment evn,
            StudentService studentService
            )
        {
            _env = evn;
            _service = service;
            _roleService = roleService;
            _accountService = accountService;
            _teacherService = teacherService;
            _fileProcess = fileProcess;
            _studentHelper = studentHelper;
            _mapping = new MappingEntity<StudentEntity, StudentViewModel>();
            _studentService = studentService;

        }
        // GET: Home

        public ActionResult Index(DefaultModel model)
        {

            ViewBag.Model = model;
            return View();
        }

        [HttpPost]
        public JsonResult GetList(DefaultModel model)
        {
            var filter = new List<FilterDefinition<CenterEntity>>();

            if (!string.IsNullOrEmpty(model.SearchText))
            {
                filter.Add(Builders<CenterEntity>.Filter.Text(model.SearchText));
            }
            //if (model.StartDate > DateTime.MinValue)
            //{
            //    filter.Add(Builders<StudentEntity>.Filter.Where(o => o.CreateDate >= new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0)));
            //}
            //if (model.EndDate > DateTime.MinValue)
            //{
            //    filter.Add(Builders<StudentEntity>.Filter.Where(o => o.CreateDate <= new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59)));
            //}
            var data = (filter.Count > 0 ? _service.Collection.Find(Builders<CenterEntity>.Filter.And(filter)) : _service.GetAll())
                .SortByDescending(t => t.ID);
            model.TotalRecord = data.CountDocuments();
            var DataResponse = data == null || model.TotalRecord <= 0 || model.TotalRecord <= model.PageSize
                ? data.ToList()
                : data.Skip((model.PageIndex) * model.PageSize).Limit(model.PageSize).ToList();

            //var students = from r in DataResponse
            //               let account = _accountService.CreateQuery().Find(o => o.UserID == r.ID && o.Type == ACCOUNT_TYPE.STUDENT).FirstOrDefault()
            //               where account != null
            //               select _mapping.AutoOrtherType(r, new StudentViewModel()
            //               {
            //                   AccountID = account.ID
            //               });
            var _DataResponse = from d in DataResponse
                    let totalStudens = _studentService.CountByCenter(d.ID)
                    select new CenterVM
                    {
                        Name = d.Name,
                        Code = d.Code,
                        Description = d.Description,
                        Abbr=d.Abbr,
                        Image=d.Image,
                        Status=d.Status,
                        Limit=d.Limit,
                        Created=d.Created,
                        StartDate=d.StartDate,
                        ExpireDate=d.ExpireDate,
                        IsDefault=d.IsDefault,
                        ID=d.ID,
                        TotalStudent=totalStudens.ToString()
                    };
                  
            //var a = new CenterVM();
            //a = DataResponse;
            //a.TotalStudent = _studentService.CountByCenter(CenterID);

            var response = new Dictionary<string, object>
            {
                { "Data", _DataResponse },
                { "Model", model }
            };
            return new JsonResult(response);

        }

        [System.Obsolete]
        [HttpPost]
        public JsonResult GetDetails(string id)
        {
            var data = _service.GetItemByID(id);
            var response = new Dictionary<string, object>
            {
                { "Data", data}
            };
            return new JsonResult(response);
        }

        [HttpPost]
        [Obsolete]
        public async Task<JsonResult> Create(CenterEntity item, IFormFile upload)
        {
            var checkAbbr = true;
            var newabbr = item.Abbr.ToLower().Trim();
            if (string.IsNullOrEmpty(item.ID) || item.ID == "0")
            {
                item.Code = item.Name.ConvertUnicodeToCode("-", true);//.Replace(@" ","-");
            }
            else
            {
                var oldItem = _service.GetItemByID(item.ID);
                if (oldItem == null)
                {
                    return new JsonResult(new Dictionary<string, object>()
                    {
                        {"Error", "Không tìm thấy cơ sở" },
                    });
                }

                if (newabbr == oldItem.Abbr)
                    checkAbbr = false;

                item.Code = oldItem.Code;
                item.IsDefault = oldItem.IsDefault;
                item.Image = oldItem.Image;
            }

            if (checkAbbr && _service.CreateQuery().Count(t => t.Abbr == newabbr) > 0)
                return new JsonResult(new Dictionary<string, object>()
                    {
                        {"Error", "Tên viết tắt đã tồn tại" }
                    });

            if (upload != null && upload.Length > 0)
            {
                var fileName = item.Code + "_" + Guid.NewGuid() + Path.GetExtension(upload.FileName).ToLower();
                item.Image = await _fileProcess.SaveMediaAsync(upload, fileName, "center", item.Code, true, 200, 300);
            }

            _service.Save(item);

            //Check Default Teacher: huonghl@utc.edu.vn
            _teacherService.CreateQuery().UpdateMany(t => t.Email == "huonghl@utc.edu.vn",
                Builders<TeacherEntity>.Update.AddToSet(t => t.Centers, new CenterMemberEntity { CenterID = item.ID, Code = item.Code, Name = item.Name, RoleID = _roleService.GetItemByCode("head-teacher").ID })
                );

            Dictionary<string, object> response = new Dictionary<string, object>()
                    {
                        {"Data", item },
                        {"Error",null },
                        {"Msg","Cập nhật thành công" }
                    };
            return new JsonResult(response);
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
                var delete = _service.Collection.DeleteMany(o => model.ArrID.Split(',').Contains(o.ID));
                return new JsonResult(delete);
            }
        }

        [HttpGet]
        [Obsolete]
        public async Task<IActionResult> Export(DefaultModel model)
        {
            var filter = new List<FilterDefinition<StudentEntity>>();

            //if (!string.IsNullOrEmpty(model.SearchText))
            //{
            //    filter.Add(Builders<StudentEntity>.Filter.Where(o => o.FullName.Contains(model.SearchText) || o.Email.Contains(model.SearchText) || o.Class.Contains(model.SearchText) || o.StudentId.Contains(model.SearchText)));
            //}
            //if (model.StartDate > DateTime.MinValue)
            //{
            //    filter.Add(Builders<CenterEntity>.Filter.Where(o => o.CreateDate >= new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0)));
            //}
            //if (model.EndDate > DateTime.MinValue)
            //{
            //    filter.Add(Builders<StudentEntity>.Filter.Where(o => o.CreateDate <= new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59)));
            //}
            //var filterData = filter.Count > 0 ? _service.Collection.Find(Builders<CenterEntity>.Filter.And(filter)) : _service.GetAll();
            //var list = await filterData.ToListAsync();
            //var index = 1;
            //var data = list.Select(o => new
            //{
            //    STT = index++,
            //    Ma_HV = o.StudentId,
            //    Ho_ten = o.FullName,
            //    Ngay_sinh = o.DateBorn.ToLocalTime().ToString("MM/dd/yyyy"),
            //    Email = o.Email,
            //    Lop = o.Class,
            //    Trang_thai = o.IsActive ? "Hoạt động" : "Đang khóa"
            //});
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("DS_HV");
                //workSheet.Cells.LoadFromCollection(data, true);
                package.Save();
            }
            stream.Position = 0;
            string excelName = $"CenterList-{DateTime.Now:yyyyMMddHHmmssfff}.xlsx";

            //return File(stream, "application/octet-stream", excelName);  
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        [HttpPost]
        [Obsolete]
        public JsonResult Publish(DefaultModel model)
        {
            if (string.IsNullOrEmpty(model.ArrID) || model.ArrID.Length <= 0)
                return new JsonResult(null);

            _service.ChangeStatus(model.ArrID.Split(',').ToList(), true);
            return new JsonResult("Publish OK");
        }

        [HttpPost]
        [Obsolete]
        public JsonResult UnPublish(DefaultModel model)
        {
            if (string.IsNullOrEmpty(model.ArrID) || model.ArrID.Length <= 0)
                return new JsonResult(null);

            _service.ChangeStatus(model.ArrID.Split(',').ToList(), false);
            return new JsonResult("UnPublish OK");
        }

        private class CenterVM : CenterEntity
        {
            public string TotalStudent { get; set; }
        }
    }
}
