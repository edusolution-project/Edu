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

        private readonly StudentHelper _studentHelper;

        public CenterController(CenterService service
            , RoleService roleService
            , AccountService accountService
            , StudentService studentService
            , IHostingEnvironment evn
            )
        {
            _env = evn;
            _service = service;
            _roleService = roleService;
            _accountService = accountService;

            _studentHelper = new StudentHelper(studentService, accountService);
            _mapping = new MappingEntity<StudentEntity, StudentViewModel>();
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
            var data = _service.GetItemByID(id);
            var response = new Dictionary<string, object>
            {
                { "Data", data}
            };
            return new JsonResult(response);
        }

        [HttpPost]
        [Obsolete]
        public JsonResult Create(CenterEntity item, IFormFile upload)
        {

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
                item.Code = oldItem.Code;
                item.IsDefault = oldItem.IsDefault;
                item.Image = oldItem.Image;
            }

            if (upload != null && upload.Length > 0)
            {
                var fileName = item.Code + "_" + Guid.NewGuid() + Path.GetExtension(upload.FileName).ToLower();
                var dirPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/upload/center");
                var path = Path.Combine(dirPath, fileName);

                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                var standardSize = new SixLabors.Primitives.Size(512, 384);

                using (Stream inputStream = upload.OpenReadStream())
                {
                    using (var image = Image.Load<Rgba32>(inputStream))
                    {
                        var imageEncoder = new JpegEncoder()
                        {
                            Quality = 90,
                            Subsample = JpegSubsample.Ratio444
                        };

                        int width = image.Width;
                        int height = image.Height;
                        if ((width > standardSize.Width) || (height > standardSize.Height))
                        {
                            ResizeOptions options = new ResizeOptions
                            {
                                Mode = ResizeMode.Max,
                                Size = standardSize,
                            };
                            image.Mutate(x => x
                             .Resize(options));

                            //.Grayscale());
                        }
                        image.Save(path, imageEncoder); // Automatic encoder selected based on extension.
                    }
                }
                var url = $"{"/upload/center/"}{fileName}";
                item.Image = url;
            }

            _service.Save(item);
            Dictionary<string, object> response = new Dictionary<string, object>()
                    {
                        {"Data",item },
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
    }
}
