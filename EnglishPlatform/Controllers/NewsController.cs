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

        public NewsController(
            NewsService newsService,
            NewsCategoryService newsCategoryService
            )
        {
            _newsService = newsService;
            _newsCategoryService = newsCategoryService;
        }
        //[Route("tin-tuc")]
        public IActionResult Index()
        {
            ViewBag.newsTop = _newsService.Collection.Find(tbl => tbl.IsTop == true && tbl.IsActive == true).SortByDescending(tbl => tbl.PublishDate).Limit(5).ToList();
            ViewBag.newsHot = _newsService.Collection.Find(tbl => tbl.IsHot == true && tbl.IsActive == true).SortByDescending(tbl => tbl.PublishDate).Limit(2).ToList();
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
    }

}