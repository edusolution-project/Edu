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
using FileManagerCore.Interfaces;

namespace BaseCustomerMVC.Controllers.Admin
{
    [BaseAccess.Attribule.AccessCtrl("Quản lý Tin tức", "admin", 4)]
    public class NewsController : AdminController
    {
        private readonly NewsCategoryService _serviceNewCate;
        private readonly NewsService _serviceNews;
        private readonly CenterService _serviceCenter;
        private readonly ClassService _serviceClass;
        private readonly IHostingEnvironment _env;
        private readonly MappingEntity<NewsCategoryEntity, NewsCategoryViewModel> _mapping;
        private readonly MappingEntity<NewsEntity, NewsViewModel> _mappings;
        private readonly TeacherService _teacherService;
        private static MailHelper _mailHelper;
        private readonly IRoxyFilemanHandler _roxyFilemanHandler;
        private readonly FileProcess _fileProcess;
        public NewsController(
            NewsCategoryService newsCategoryService, 
            NewsService newsService,
            CenterService centerService, 
            ClassService classService,
            IHostingEnvironment evn,
            TeacherService teacherService,
            MailHelper mailHelper,
            IRoxyFilemanHandler roxyFilemanHandler,
            FileProcess fileProcess
            )
        {
            _serviceNewCate = newsCategoryService;
            _serviceNews = newsService;
            _serviceCenter = centerService;
            _serviceClass = classService;
            _mapping = new MappingEntity<NewsCategoryEntity, NewsCategoryViewModel>();
            _mappings = new MappingEntity<NewsEntity, NewsViewModel>();
            _env = evn;
            _teacherService = teacherService;
            _mailHelper = mailHelper;
            _roxyFilemanHandler = roxyFilemanHandler;
            _fileProcess = fileProcess;
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
                var oldItem = _serviceNewCate.GetItemByID(item.ID);
                if (oldItem == null)
                {
                    Dictionary<string, object> _response = new Dictionary<string, object>()
                    {
                        {"Data",null },
                        {"Error",null },
                        {"Msg","Không tìm thấy danh mục tương ứng." }
                    };
                    return new JsonResult(_response);
                }
                if (String.IsNullOrEmpty(oldItem.Code) || !oldItem.Name.Equals(item.Name))
                {
                    item.Code = item.Name.ConvertUnicodeToCode("-", true);
                    int pos = 0;
                    while (_serviceNewCate.GetItemByCode(item.Code) != null)
                    {
                        pos++;
                        item.Code += ("-" + pos);
                    }
                }
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
            ViewBag.ListCenter = _serviceCenter.GetAll().ToList();
            return View("News");
        }

        public JsonResult CreateNews(NewsEntity item, IFormFile Thumbnail,string CategoryCode)
        {
            if (item.CategoryID == null)
            {
                Dictionary<string, object> response = new Dictionary<string, object>()
                    {
                        {"Data",item },
                        {"Error",null },
                        {"Msg","Cập nhật thành công" }
                    };
                return new JsonResult(response);
            }    

            if (string.IsNullOrEmpty(item.ID) || item.ID == "0")
            {
                item.CreateDate = DateTime.UtcNow;
                if (item.PublishDate < new DateTime(1900, 1, 1))
                    item.PublishDate = item.CreateDate;//publish now
                item.Code = item.Title.ConvertUnicodeToCode("-", true);
                item.IsActive = true;
                item.Type = "news";
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
                item.LastEdit = DateTime.UtcNow;
                item.IsActive = olditem.IsActive;

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
                    //removeThumbnail(olditem.Thumbnail);
                    item.Thumbnail = urlThumbnail(Thumbnail);
                }
                else
                    item.Thumbnail = olditem.Thumbnail;
                item.Type = "news";

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

            if (!string.IsNullOrEmpty(CategoryID))
            {
                filter.Add(Builders<NewsEntity>.Filter.Where(o => o.CategoryID == CategoryID));
            }
            filter.Add(Builders<NewsEntity>.Filter.Where(o => o.Type.ToLower() == "news" || o.Type==null));

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
            else
            {
                data = data.SortByDescending(x => x.CreateDate);
            }

            var dataResult = (from d in data.ToList()
                             let category = _serviceNewCate.GetItemByID(d.CategoryID)
                             where category != null
                             select new NewsViewModel
                             {
                                 CategoryID = category.ID,
                                 CenterID = d.CenterID,
                                 Code = d.Code,
                                 Content = d.Content,
                                 CreateDate = d.CreateDate,
                                 CreateUser = d.CreateUser,
                                 ID=d.ID,
                                 IsActive = d.IsActive,
                                 IsHot = d.IsHot,
                                 IsPublic = d.IsPublic,
                                 IsTop = d.IsTop,
                                 PublishDate = d.PublishDate,
                                 Summary = d.Summary,
                                 Thumbnail = d.Thumbnail,
                                 Type = d.Type,
                                 Title = d.Title,
                                 Url = String.IsNullOrEmpty(category.Code) && String.IsNullOrEmpty(d.Code) ? "" : $"{category.Code}/{d.Code}"
                             }).ToList();
            
            model.TotalRecord = dataResult.Count();
            var DataResponse = dataResult == null || model.TotalRecord <= 0 || model.TotalRecord <= model.PageSize
                ? dataResult.ToList()
                : dataResult.Skip((model.PageIndex) * model.PageSize).Take(model.PageSize).ToList();

            var response = new Dictionary<string, object>
            {
                { "Data", DataResponse},
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
                //removeThumbnail(thumbnail);
                var delete = _serviceNews.Collection.DeleteMany(o => model.ArrID.Split(',').Contains(o.ID));
                return new JsonResult(delete);
            }
        }

        [System.Obsolete]
        [HttpPost]
        public JsonResult GetDetailsNews(string id)
        {
            var _data = _serviceNews.CreateQuery().Find(o=>o.ID==id && o.Type.ToLower()=="news").FirstOrDefault();
            var data=_data!=null?_data: _serviceNews.CreateQuery().Find(o => o.ID == id).FirstOrDefault();
            var CategoryCode = _serviceNewCate.GetItemByID(data.CategoryID);

            //var categoryname = _serviceNewCate.getNameCategoryByID(id).Name;

            var response = new Dictionary<string, object>
            {
                { "Data", data},
                {"CategoryCode",CategoryCode }
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

        public String urlThumbnail(IFormFile formFile)
        {
            string _fileName = formFile.FileName;
            var timestamp = DateTime.UtcNow.ToFileTime();
            _fileName = timestamp + "_" + _fileName;
            String urlImg =_fileProcess.SaveMediaAsync(formFile, _fileName, "News", "Eduso").Result;
            return urlImg;
        }

        public void removeThumbnail(String urlImg)
        {
            _fileProcess.DeleteFile(urlImg);
        }
        #endregion

        #region
        [HttpPost]
        public JsonResult getAllCourse(string CenterID)
        {
            var data = _serviceClass.Collection.Find(tbl => tbl.IsActive == true && tbl.Center.Equals(CenterID)).SortByDescending(tbl => tbl.ID);
            return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", data.ToList() },
                                {"Error", null }
                            });
        }
        #endregion

        #region New functions
        public IActionResult NewFunctions()
        {
            var centers = _serviceCenter.GetAll();
            ViewBag.Centers = centers.ToList();
            return View();
        } //chức năng mới

        public async Task<JsonResult> SendMail(NewsEntity item)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var category = _serviceNewCate.GetItemByCode("gioi-thieu");
                item.CategoryID = category == null ? "" : category.ID;
                item.IsActive = false;
                item.IsHot = false;
                item.IsPublic = false;
                item.IsTop = false;
                item.CreateDate = DateTime.UtcNow;
                item.CreateUser = UserID;

                _serviceNews.CreateOrUpdate(item);
                if (item.Targets != null)
                    foreach (var centerID in item.Targets)
                    {
                        var center = _serviceCenter.CreateQuery().Find(x => x.ID == centerID && x.ExpireDate >= DateTime.Now).FirstOrDefault();
                        if (center != null)
                        {
                            //danh sach giao vien trong co so
                            var listTeacher = _teacherService.GetByCenterID(center.ID);
                            List<String> emails = listTeacher.Select(x => x.Email).ToList();
                            var toAddress = new List<String> { "nguyenvanhoa2017602593@gmail.com" };
                            _ = await _mailHelper.SendBaseEmail(toAddress, item.Title, item.Content, MailPhase.NEW_FUNCTION);
                        }
                    }
                return Json("Gửi thành công");
            }
            catch(Exception ex)
            {
                return Json(ex.Message);
            }
        }
        #endregion

        #region uploadimg
        public JsonResult GetPathIMG(IFormFile Thumbnail,IFormFile upload,String ckCsrfToken)
        {
            try
            {
                if (upload != null)
                {
                    var filepath = urlThumbnail(upload);
                    var data = new Dictionary<String, Object>
                        {
                            { "FilePath", filepath }
                        };
                    return Json(data);
                }
                else { return Json("loi"); }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
        #endregion
    }
}
