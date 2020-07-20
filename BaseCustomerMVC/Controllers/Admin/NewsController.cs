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

namespace BaseCustomerMVC.Controllers.Admin
{
    [BaseAccess.Attribule.AccessCtrl("Quản lý Tin tức", "admin", 2)]
    public class NewsController : AdminController
    {
        private readonly NewsCategoryService _serviceNewCate;
        private readonly NewsService _serviceNews;
        private readonly IHostingEnvironment _env;
        private readonly MappingEntity<NewsCategoryEntity, NewsCategoryViewModel> _mapping;
        private readonly MappingEntity<NewsEntity, NewsViewModel> _mappings;
        public NewsController(NewsCategoryService newsCategoryService, NewsService newsService, IHostingEnvironment evn)
        {
            _serviceNewCate = newsCategoryService;
            _serviceNews = newsService;
            _mapping = new MappingEntity<NewsCategoryEntity, NewsCategoryViewModel>();
            _mappings = new MappingEntity<NewsEntity, NewsViewModel>();
            _env = evn;
        }

        public ActionResult Index()
        {
            return View();
        }

        #region NewsCategory
        public IActionResult NewsCategory()
        {
            ViewBag.newscategory = _serviceNewCate.GetAll().ToList();
            return View("NewsCategory");
        }

        [HttpPost]
        [Obsolete]
        public JsonResult Create(NewsCategoryEntity item)
        {
            if (string.IsNullOrEmpty(item.ID) || item.ID == "0")
            {
                item.Code = item.Name.ConvertUnicodeToCode("-", true);
                int pos = 0;
                while (_serviceNewCate.GetItemByCode(item.Code) != null)
                {
                    pos++;
                    item.Code += ("-" + pos);
                }
                _serviceNewCate.CreateQuery().InsertOne(item);
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
                _serviceNewCate.Save(item);
                Dictionary<string, object> response = new Dictionary<string, object>()
                    {
                        {"Data",item },
                        {"Error",null },
                        {"Msg","Cập nhật thành công" }
                    };
                return new JsonResult(response);
            }
        }

        [HttpPost]
        [Route("/admin/news/GetList")]
        public JsonResult GetList(DefaultModel model)
        {
            var filter = new List<FilterDefinition<NewsCategoryEntity>>();

            if (!string.IsNullOrEmpty(model.SearchText))
            {
                filter.Add(Builders<NewsCategoryEntity>.Filter.Text(model.SearchText));
            }
            //if (model.StartDate > DateTime.MinValue)
            //{
            //    filter.Add(Builders<StudentEntity>.Filter.Where(o => o.CreateDate >= new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0)));
            //}
            //if (model.EndDate > DateTime.MinValue)
            //{
            //    filter.Add(Builders<StudentEntity>.Filter.Where(o => o.CreateDate <= new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59)));
            //}
            //var data = (filter.Count > 0 ? _serviceNewCate.Collection.Find(Builders<CenterEntity>.Filter.And(filter)) : _serviceNewCate.GetAll())
            var data = _serviceNewCate.GetAll()
                .SortByDescending(t => t.ID);
            model.TotalRecord = data.CountDocuments();
            var newsCate = data == null || model.TotalRecord <= 0 || model.TotalRecord <= model.PageSize
                ? data.ToList()
                : data.Skip((model.PageIndex) * model.PageSize).Limit(model.PageSize).ToList();

            var DataResponse =

                from t in newsCate.ToList()
                    //let account = _accountService.CreateQuery().Find(o => o.UserID == t.ID && o.Type == ACCOUNT_TYPE.TEACHER).FirstOrDefault()
                    //where account != null
                    //let role = roles.Find(r => r.ID == account.RoleID)
                    //where role != null
                select _mapping.AutoOrtherType(t, new NewsCategoryViewModel()
                {
                    ParentName = t.Name == null ? null : _serviceNewCate.CreateQuery().Find(x => x.ID == t.ParentID).ToList()
                });

            var response = new Dictionary<string, object>
            {
                { "Data", DataResponse },
                { "Model", model }
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
                var delete = _serviceNewCate.Collection.DeleteMany(o => model.ArrID.Split(',').Contains(o.ID));
                return new JsonResult(delete);
            }
        }

        [System.Obsolete]
        [HttpPost]
        public JsonResult GetDetails(string id)
        {
            var data = _serviceNewCate.GetItemByID(id);
            var response = new Dictionary<string, object>
            {
                { "Data", data}
            };
            return new JsonResult(response);
        }
        #endregion

        #region News
        public IActionResult News()
        {
            //ViewBag.news = _serviceNews.GetAll().ToList();
            ViewBag.newscategory = _serviceNewCate.GetAll().ToList();
            return View("News");
        }

        public IActionResult NewsForm()
        {
            ViewBag.newscategory = _serviceNewCate.GetAll().ToList();
            return View("NewsForm");
        }

        public JsonResult CreateNews(NewsEntity item, IFormFile Thumbnail)
        {
            if (string.IsNullOrEmpty(item.ID) || item.ID == "0")
            {
                item.CreateDate = DateTime.UtcNow;
                if (item.PublishDate < new DateTime(1900, 1, 1))
                    item.PublishDate = item.CreateDate;//publish now
                item.Code = item.Title.ConvertUnicodeToCode("-", true);
                item.IsActive = true;
                var pos = 0;
                while (_serviceNews.GetItemByCode(item.Code) != null)
                {
                    pos++;
                    item.Code += ("-" + pos);
                }

                if (Thumbnail != null)
                {
                    item.Thumbnail = urlThumbnail(Thumbnail);
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
                    removeThumbnail(olditem.Thumbnail);
                    item.Thumbnail = urlThumbnail(Thumbnail);
                }
                else
                    item.Thumbnail = olditem.Thumbnail;

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

        [HttpPost]
        [Route("/admin/news/GetListNews")]
        public JsonResult GetListNews(DefaultModel model, string CategoryID)
        {
            var filter = new List<FilterDefinition<NewsEntity>>();
            //var sort = new List<SortDefinition<NewsEntity>>();

            if (!string.IsNullOrEmpty(model.SearchText))
            {
                filter.Add(Builders<NewsEntity>.Filter.Text(model.SearchText));
            }
            //if (model.StartDate > DateTime.MinValue)
            //{
            //    filter.Add(Builders<StudentEntity>.Filter.Where(o => o.CreateDate >= new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0)));
            //}
            //if (model.EndDate > DateTime.MinValue)
            //{
            //    filter.Add(Builders<StudentEntity>.Filter.Where(o => o.CreateDate <= new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59)));
            //}
            if (!string.IsNullOrEmpty(CategoryID))
            {
                filter.Add(Builders<NewsEntity>.Filter.Where(o => o.CategoryID == CategoryID));
            }

            //===========
            var hasfilter = _serviceNews.Collection.Find(Builders<NewsEntity>.Filter.And(filter));/*.Sort(Builders<NewsEntity>.Sort.Descending(tbl=>tbl.CreateDate));*/
            var nofilter = _serviceNews.GetAll().SortByDescending(tbl => tbl.ID);

            //============

            var data = (filter.Count > 0 ? hasfilter : nofilter);


            if (!string.IsNullOrEmpty(model.Sort))
            {
                if (model.Sort.Equals("NewNews"))
                    data = data.SortByDescending(tbl => tbl.CreateDate);
                else
                    data = data.SortBy(tbl => tbl.CreateDate);
            }

            //    && 
            //{
            //    sort.SortByDescending(tbl => tbl.CreateDate);
            //    _sort.SortByDescending(tbl => tbl.CreateDate);
            //}
            //if (!string.IsNullOrEmpty(model.Sort) && model.Sort.Equals("OldNews"))
            //{
            //    sort.SortBy(tbl => tbl.CreateDate);
            //    _sort.SortBy(tbl => tbl.CreateDate);
            //}

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

        [HttpPost]
        [Obsolete]
        public JsonResult DeleteNews(DefaultModel model)
        {
            if (model.ArrID.Length <= 0)
            {
                return new JsonResult(null);
            }
            else
            {
                var thumbnail = _serviceNews.GetItemByID(model.ArrID).Thumbnail;
                removeThumbnail(thumbnail);
                var delete = _serviceNews.Collection.DeleteMany(o => model.ArrID.Split(',').Contains(o.ID));
                return new JsonResult(delete);
            }
        }

        [System.Obsolete]
        [HttpPost]
        public JsonResult GetDetailsNews(string id)
        {
            var data = _serviceNews.GetItemByID(id);

            //var categoryname = _serviceNewCate.getNameCategoryByID(id).Name;

            var response = new Dictionary<string, object>
            {
                { "Data", data}
                //,{"CategoryName",categoryname }
            };
            return new JsonResult(response);
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

        public string urlThumbnail(IFormFile Thumbnail)
        {
            string _fileName = Thumbnail.FileName;
            var timestamp = DateTime.Now.ToFileTime();
            _fileName = timestamp + "_" + _fileName;

            var _dirPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/News");
            if (!Directory.Exists(_dirPath))
                Directory.CreateDirectory(_dirPath);

            string _path = Path.Combine(Path.Combine(_dirPath, _fileName));
            using (var stream = new FileStream(_path, FileMode.Create))
            {
                Thumbnail.CopyTo(stream);
            }
            return _fileName;
        }

        public void removeThumbnail(string Thumbnail)
        {
            if (Thumbnail != null && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "News", Thumbnail)))
            {
                System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "News", Thumbnail));
            }
        }
        #endregion
    }
}
