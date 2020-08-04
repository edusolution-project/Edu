using BaseCustomerMVC.Globals;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using BaseCustomerEntity.Database;
using BaseCustomerMVC.Models;
using MongoDB.Driver;
using System.Linq;
using Core_v2.Globals;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using BaseCustomerEntity.Globals;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.Threading.Tasks;

namespace BaseCustomerMVC.Controllers.Admin
{
    [BaseAccess.Attribule.AccessCtrl("Quản lý Gói học", "admin", 2)]
    public class ProductController : AdminController
    {
        private readonly NewsCategoryService _serviceNewCate;
        private readonly NewsService _serviceNews;
        private readonly CenterService _serviceCenter;
        private readonly ClassService _serviceClass;
        private readonly FileProcess _fileProcess;
        private readonly IHostingEnvironment _env;
        private readonly MappingEntity<NewsEntity, NewsViewModel> _mapping;

        public ProductController(
            NewsCategoryService newsCategoryService,
            NewsService newsService,
            CenterService centerService,
            ClassService classService,
            IHostingEnvironment evn,
            FileProcess fileProcess
            )
        {
            _serviceNewCate = newsCategoryService;
            _serviceNews = newsService;
            _serviceCenter = centerService;
            _serviceClass = classService;
            _env = evn;
            _fileProcess = fileProcess;
            _mapping = new MappingEntity<NewsEntity, NewsViewModel>();
        }

        public ActionResult Index()
        {
            var center = _serviceCenter.GetAll();
            var listClass = _serviceClass.GetAll();
            ViewBag.listcenter = center.ToList();
            ViewBag.listclass = listClass.ToList();
            return View();
        }

        public async Task<JsonResult> CreateOrUpdate(NewsEntity item, IFormFile Thumbnail, string CategoryCode="san-pham")
        {
            if (item.CategoryID == null)
                item.CategoryID = _serviceNewCate.GetItemByCode(CategoryCode).ID;

            //if (item.Sale != 0)
            //{
            //    item.PriceSale = item.Price - item.Price * item.Sale / 100;
            //}

            if (string.IsNullOrEmpty(item.ID) || item.ID == "0")
            {
                item.CreateDate = DateTime.UtcNow;
                if (item.PublishDate < new DateTime(1900, 1, 1))
                    item.PublishDate = item.CreateDate;//publish now
                item.Code = item.Title.ConvertUnicodeToCode("-", true);
                item.IsActive = true;
                item.IsPublic = false; //mac dinh se khong public
                item.Type = "san-pham";
                if (item.CenterID == "0") item.IsPublic = true;
                else item.IsPublic = false;

                var pos = 0;
                while (_serviceNews.GetItemByCode(item.Code) != null)
                {
                    pos++;
                    item.Code += ("-" + pos);
                }

                if (Thumbnail != null)
                {
                    item.Thumbnail = await _fileProcess.SaveMediaAsync(Thumbnail,"","Image_Product");
                }
                _serviceNews.CreateQuery().InsertOne(item);
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
                var olditem = _serviceNews.GetItemByID(item.ID);

                item.Code = item.Title.ConvertUnicodeToCode("-", true);
                item.CreateDate = olditem.CreateDate;
                item.LastEdit = DateTime.Now;

                var pos = 0;
                var sameUrl = _serviceNews.GetItemByCode(item.Code);
                while (sameUrl != null && sameUrl.ID != item.ID)
                {
                    pos++;
                    item.Code += ("-" + pos);
                    sameUrl = _serviceNews.GetItemByCode(item.Code);
                }

                if (Thumbnail != null)
                {
                    _fileProcess.DeleteFile(olditem.Thumbnail);
                    item.Thumbnail = await _fileProcess.SaveMediaAsync(Thumbnail, "", "Image_Product");
                }
                else
                    item.Thumbnail = olditem.Thumbnail;

                if (item.CenterID == "0") item.IsPublic = true;
                else item.IsPublic = false;

                _serviceNews.Save(item);
                Dictionary<string, object> response = new Dictionary<string, object>()
                    {
                        {"Data",item },
                        {"Error",null },
                        {"Msg","Cập nhật thành công" }
                    };
                return new JsonResult(response);
            }
        }

        public JsonResult GetClass(string centerID)
        {
            if (centerID != "0")
            {
                //var listclass = _serviceClass.CreateQuery().Find(o => o.Center == centerID && o.IsActive==true);
                var listclass = _serviceClass.CreateQuery().Find(o => o.Center == centerID);
                var Dataresponse = new Dictionary<string, object>{
                { "Data",listclass.ToList()}
            };
                return Json(Dataresponse);
            }
            else
            {
                //var listclass = _serviceClass.CreateQuery().Find(o=>o.IsActive==true);
                var listclass = _serviceClass.GetAll();
                var Dataresponse = new Dictionary<string, object>{
                { "Data",listclass.ToList()}
            };
                return Json(Dataresponse);
            }
        }

        public JsonResult getListProduct(DefaultModel model)
        {
            var data = _serviceNews.CreateQuery().Find(o => o.Type.Equals("san-pham"));

            model.TotalRecord = data.CountDocuments();
            var list = data == null || model.TotalRecord <= 0 || model.TotalRecord <= model.PageSize
                ? data.ToList()
                : data.Skip((model.PageIndex) * model.PageSize).Limit(model.PageSize).ToList();

            //var list_product = _serviceNews.GetAll();
            //var Dataresponse = new Dictionary<string, object>{
            //    { "Data",DataResponse},
            //    { "Model", model }
            //};

            var DataResponse =

                from t in list
                    //let account = _accountService.CreateQuery().Find(o => o.UserID == t.ID && o.Type == ACCOUNT_TYPE.TEACHER).FirstOrDefault()
                    //where account != null
                    //let role = roles.Find(r => r.ID == account.RoleID)
                    //where role != null
                select _mapping.AutoOrtherType(t, new NewsViewModel()
                {
                    //ParentName = t.Name == null ? null : _serviceNewCate.CreateQuery().Find(x => x.ID == t.ParentID).ToList()
                    ClassName = t.ClassID == null || t.ClassID == "0" || t.ClassID == "" ? null : _serviceClass.GetItemByID(t.ClassID).Name,
                    CenterName = t.CenterID == null || t.CenterID == "0" || t.CenterID == "" ? null : _serviceCenter.GetItemByID(t.CenterID).Name

                });
            var response = new Dictionary<string, object>
            {
                { "Data", DataResponse.ToList() },
                { "Model", model }
            };
            return new JsonResult(response);
        }

        [System.Obsolete]
        [HttpPost]
        public JsonResult GetDetail(string id)
        {
            var data = _serviceNews.GetItemByID(id);

            var response = new Dictionary<string, object>
            {
                { "Data", data}
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
                var delete = _serviceNews.Collection.DeleteMany(o => o.ID==model.ArrID);
                _fileProcess.DeleteFile(_serviceNews.GetItemByID(model.ArrID).Thumbnail);
                return new JsonResult(delete);
            }
        }

        [HttpPost]
        [Obsolete]
        public JsonResult Publish(DefaultModel model, string Check)
        {
            if (string.IsNullOrEmpty(model.ArrID) || model.ArrID.Length <= 0)
                return new JsonResult(null);

            _serviceNews.ChangeStatus(model.ArrID.Split(',').ToList(), true, Check);
            return new JsonResult("Publish OK");
        }

        [HttpPost]
        [Obsolete]
        public JsonResult UnPublish(DefaultModel model, string Check)
        {
            if (string.IsNullOrEmpty(model.ArrID) || model.ArrID.Length <= 0)
                return new JsonResult(null);

            _serviceNews.ChangeStatus(model.ArrID.Split(',').ToList(), false, Check);
            return new JsonResult("UnPublish OK");
        }
    }
}
