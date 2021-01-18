using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BaseCustomerEntity.Database;
using Core_v2.Globals;
using Core_v2.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using BaseCustomerMVC.Globals;
using BaseAccess.Interfaces;
using Microsoft.Extensions.Options;
using BaseCustomerMVC.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using Newtonsoft.Json;
using System.Diagnostics;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using BaseCustomerEntity.Globals;

namespace EnglishPlatform.Controllers
{
    public class NewsController : Controller
    {
        private readonly NewsService _newsService;
        private readonly NewsCategoryService _newsCategoryService;
        private readonly CenterService _centerService;
        private readonly StudentService _studentService;
        private readonly TeacherService _teacherService;

        public NewsController(
            NewsService newsService,
            NewsCategoryService newsCategoryService,
            CenterService centerService,
            StudentService studentService,
            TeacherService teacherService
            )
        {
            _newsService = newsService;
            _newsCategoryService = newsCategoryService;
            _centerService = centerService;
            _studentService = studentService;
            _teacherService = teacherService;
        }
        //[Route("tin-tuc")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Category(DefaultModel model, String catcode)
        {
            model.PageSize = 12;
            var cat = _newsCategoryService.GetItemByCode(catcode);
            ViewBag.Category = cat;
            if (cat == null)
                return RedirectToRoute("news-default");

            var filter = new List<FilterDefinition<NewsEntity>>();
            filter.Add(Builders<NewsEntity>.Filter.Where(t => t.CategoryID == cat.ID));
            filter.Add(Builders<NewsEntity>.Filter.Lte(t => t.PublishDate, DateTime.Now));
            filter.Add(Builders<NewsEntity>.Filter.Where(t => t.IsActive == true));
            filter.Add(Builders<NewsEntity>.Filter.Where(t => t.Type.Equals("news")));

            var data = _newsService.Collection.Find(Builders<NewsEntity>.Filter.And(filter))
                .SortByDescending(t => t.PublishDate);
            ViewBag.TotalRec = data.CountDocuments();
            model.TotalRecord = ViewBag.TotalRec;
            ViewBag.FirstPage = data.Any() == true ? data.Limit(model.PageSize).ToList() : null;
            //ViewBag.Data = _newsService.Collection.Find(tbl => tbl.CategoryID.Equals(cat.ID)).ToList();
            if (catcode.Equals("ve-eduso"))
            {
                return View("AboutEduso");
            }
            else
            {
                return View();
            }
        }

        //lay chi tiet tin
        public IActionResult Detail(string catcode, string newscode)
        {
            var cat = _newsCategoryService.GetItemByCode(catcode);
            ViewBag.Category = cat;
            var filter = new List<FilterDefinition<NewsEntity>>();

            if (!string.IsNullOrEmpty(newscode))
            {
                filter.Add(Builders<NewsEntity>.Filter.Where(t => t.Code == newscode));
            }
            filter.Add(Builders<NewsEntity>.Filter.Where(t => t.Type == "news" || t.Type == null));

            var listnews = _newsService.CreateQuery().Find(x => x.Type == "news" || x.Type == null).ToList();
            foreach (var item in listnews)
            {
                if (item.Code == null)
                {
                    item.Code = item.Title.ConvertUnicodeToCode("-", true);
                }
                if (item.Code[item.Code.Length - 1].ToString() == "?")
                {
                    item.Code = item.Code.Remove(item.Code.Length - 1);
                }
            }

            //var news = _newsService.CreateQuery().Find(Builders<NewsEntity>.Filter.And(filter)).FirstOrDefault();
            var news = listnews.Find(x => x.Code == newscode);

            ViewBag.News = news;
            if (news == null || !news.IsActive)
            {
                if (cat == null)
                    return RedirectToRoute("news-default");
                else
                    return RedirectToRoute("news-category", new { catcode = cat.Code });
            }
            return View();
        }

        [HttpPost]
        [Route("/news/getlist")]
        public JsonResult GetList(DefaultModel model, string catID, string CurrentNewsCode, bool isHot = false, bool isTop = false)
        {
            var filter = new List<FilterDefinition<NewsEntity>>();

            if (!string.IsNullOrEmpty(catID))
            {
                filter.Add(Builders<NewsEntity>.Filter.Where(t => t.CategoryID == catID));
            }
            if (isHot)
            {
                filter.Add(Builders<NewsEntity>.Filter.Where(t => t.IsHot));
            }
            if (isTop)
            {
                filter.Add(Builders<NewsEntity>.Filter.Where(t => t.IsTop));
            }
            if (!string.IsNullOrEmpty(CurrentNewsCode))
            {
                filter.Add(Builders<NewsEntity>.Filter.Where(t => !t.Code.Equals(CurrentNewsCode)));
            }
            filter.Add(Builders<NewsEntity>.Filter.Lte(t => t.PublishDate, DateTime.Now));
            filter.Add(Builders<NewsEntity>.Filter.Where(t => t.IsActive == true));
            filter.Add(Builders<NewsEntity>.Filter.Where(t => t.Type == "news" || t.Type == null));

            var categories = _newsCategoryService.GetAll().ToList();

            var _data = (filter.Count > 0 ? _newsService.Collection.Find(Builders<NewsEntity>.Filter.And(filter)) : _newsService.GetAll())
                .SortByDescending(t => t.PublishDate);
            model.TotalRecord = _data.CountDocuments();


            var categoryCode = categories.Find(t => t.ID == catID)?.Code;

            var data = from d in _data.ToList()
                           //let categoryCode = !string.IsNullOrEmpty(d.CategoryID) ? categorys.Where(x => x.ID == d.CategoryID).FirstOrDefault().Code : ""
                       select new NewsViewModel
                       {
                           CategoryID = d.CategoryID,
                           CategoryCode = categoryCode,
                           Code = string.IsNullOrEmpty(d.Code) ? d.Title.ConvertUnicodeToCode("-", true) : d.Code,
                           Content = d.Content,
                           CreateDate = d.CreateDate,
                           ID = d.ID,
                           PublishDate = d.PublishDate,
                           Summary = d.Summary,
                           Thumbnail = d.Thumbnail,
                           Title = d.Title,
                           Type = d.Type,
                           IsHot = d.IsHot
                       };
            var DataResponse = data == null || model.TotalRecord <= 0 || model.TotalRecord <= model.PageSize
                ? data.ToList()
                : data.Skip((model.PageIndex) * model.PageSize).Take(model.PageSize).ToList();

            var listCurrentNewsID = DataResponse.Select(x => x.ID).ToList();

            var _dataHot = _newsService.CreateQuery().Find(x =>
            !listCurrentNewsID.Contains(x.ID) && 
            (x.IsActive || x.PublishDate >= DateTime.Now) && 
            x.IsHot == true && x.Type == "news")
                .Limit(6)
                .ToList()
                .OrderByDescending(x=>x.PublishDate);

            var dataResult = new List<NewsViewModel>();
            var datahot = (from d in _dataHot
                           where categories.Where(x => x.ID == d.CategoryID).FirstOrDefault() != null
                           let categoryCode1 =
                            (categories.Where(x => x.ID == d.CategoryID).FirstOrDefault() ?? new NewsCategoryEntity { Code = d.CategoryID }).Code ?? "Khong co code"
                           //select d.ID).ToList();
                           select new NewsViewModel
                           {
                               CategoryID = d.CategoryID,
                               CategoryCode = categoryCode1,
                               Code = string.IsNullOrEmpty(d.Code) ? d.Title.ConvertUnicodeToCode("-", true) : d.Code,
                               Content = "",
                               CreateDate = d.CreateDate,
                               ID = d.ID,
                               PublishDate = d.PublishDate,
                               Summary = d.Summary,
                               Thumbnail = d.Thumbnail,
                               Title = d.Title,
                               Type = d.Type,
                               IsHot = d.IsHot
                           }).ToList();

            var response = new Dictionary<string, object>
            {
                { "Data", DataResponse },
                { "DataNewsH", datahot },
                { "Model", model }
            };
            return new JsonResult(response);
        }

        public IActionResult DetailProduct(string code)
        {
            var detail = _newsService.CreateQuery().Find(o => o.Code.Equals(code) && o.Type == "san-pham").FirstOrDefault();
            ViewBag.detail = detail;
            return View();
        }

        public JsonResult GetCategory(String ParentCategoryCode)
        {
            if (String.IsNullOrEmpty(ParentCategoryCode))
            {
                return Json(new Dictionary<String, Object> { { "Data", "" } });
            }
            else
            {
                var parentCategory = _newsCategoryService.GetItemByCode(ParentCategoryCode);
                if (parentCategory == null)
                {
                    return Json(new Dictionary<String, Object> { { "Data", "" } });
                }
                var data = _newsCategoryService.GetByParentCategoryID(parentCategory.ID);
                var dataResponse = new Dictionary<String, Object>
                {
                    {"Data",data }
                };
                return Json(dataResponse);
            }
        }

        #region get all news
        public JsonResult GetAllNews(List<String> oldNewsID)
        {
            try
            {
                var categories = _newsCategoryService.CreateQuery().Find(x=>x.IsShow).ToList();
                if(categories.Count() == 0 || categories == null)
                {
                    return Json("");
                }

                List<NewsEntity> news = new List<NewsEntity>();
                if(oldNewsID.Count() == 0)
                {
                    news = _newsService.GetAllNews().ToList();
                }
                else
                {
                    news = _newsService.CreateQuery().Find(x =>
                    x.Type == "news" &&
                    oldNewsID.Contains(x.ID) &&
                    (x.IsPublic || x.PublishDate <= DateTime.Now))
                        .ToList()
                        .OrderByDescending(x => x.PublishDate)
                        .ToList();
                }

                var listNews = new List<NewsEntity>();
                foreach(var item  in news.GroupBy(x => x.CategoryID))
                {
                    listNews.AddRange(item.ToList().Take(6));
                }

                return Json(
                        new Dictionary<String, Object>
                        {
                            {"News",listNews },
                            {"Categories",categories.ToList() }
                        }
                    );
            }
            catch(Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public JsonResult GetNewsList()
        {
            try
            {
                var NewsTop = _newsService.Collection.Find(tbl => tbl.IsTop == true && (tbl.PublishDate <= DateTime.Now || tbl.IsActive == true)).ToList().OrderByDescending(x => x.PublishDate).Take(3);
                var NewsHot = _newsService.Collection.Find(tbl => tbl.IsHot == true && (tbl.PublishDate <= DateTime.Now || tbl.IsActive == true)).ToList().OrderByDescending(x=>x.PublishDate).Take(1);
                var response = new Dictionary<string, object>
                    {
                        {"NewsTop",NewsTop.ToList() },
                        {"NewsHot",NewsHot.ToList() }
                    };

                return Json(response);
            }
            catch(Exception ex)
            {
                return Json(ex.Message);
            }
        }
        #endregion

        #region getCurrentInfor
        public JsonResult GetCurrentInfor()
        {
            try
            {
                var type = User.Claims.GetClaimByType("Type");
                var center = _centerService.GetDefault();
                string centerCode = center.Code;
                //string roleCode = "";
                var user = User.FindFirst("UserID");
                if(user == null)
                {
                    return Json(new Dictionary<String, object>
                            {
                                {"AllCenters",new List<CenterEntity>() },
                                {"Mtype","" }
                            });
                }

                var userID = user.Value;
                var tc = _teacherService.GetItemByID(userID);
                var st = _studentService.GetItemByID(userID);
                var listCenters = new List<CenterEntity>();
                switch (type.Value)
                {
                    case ACCOUNT_TYPE.ADMIN:
                        centerCode = center.Code;
                        listCenters.Add(center);
                        break;
                    case ACCOUNT_TYPE.TEACHER:
                        if (tc != null)
                        {
                            var centers = tc.Centers
                                .Where(ct => _centerService.GetItemByID(ct.CenterID)?.ExpireDate > DateTime.Now)
                                .Select(t => new CenterEntity { Code = t.Code, Name = t.Name }).ToList();
                            if (centers != null)
                            {
                                centerCode = centers.FirstOrDefault().Code;
                                //ViewBag.AllCenters = centers; //????
                            }
                            else
                            {
                                centerCode = center.Code;
                                centers.Add(center);
                                //ViewBag.AllCenters = listCenters.Add(center); //????
                            }
                            listCenters.AddRange(centers);
                        }
                        break;
                    default:
                        if (st != null)
                        {
                            if (st.Centers != null && st.Centers.Count > 0)
                            {
                                var allcenters = st.Centers
                                    .Where(ct => _centerService.GetItemByID(ct)?.ExpireDate > DateTime.Now)
                                    .Select(t => _centerService.GetItemByID(t)).ToList();
                                centerCode = allcenters.Count > 0 ? allcenters.FirstOrDefault().Code : center.Code;
                                listCenters.AddRange(allcenters);
                            }
                            else
                            {
                                centerCode = center.Code;
                                //var a = new List<CenterEntity>();
                                //a.Add(center);
                                listCenters.Add(center);
                            }
                        }
                        break;
                }

                return Json(new Dictionary<String, object> 
                {
                    {"AllCenters",listCenters },
                    {"Mtype",type.Value }
                });
            }
            catch(Exception ex)
            {
                return Json(ex.Message);
            }
        }
        #endregion
    }

}