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
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Microsoft.AspNetCore.Razor.Language;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace BaseCustomerMVC.Controllers.Admin
{
    [BaseAccess.Attribule.AccessCtrl("Quản lý Quảng cáo", "admin", 2)]
    public class AdsController : AdminController
    {
        private readonly AdsService _servicesAds;
        private readonly IHostingEnvironment _evn;
        private string RootPath { get; }
        public AdsController(AdsService servicesAds, IHostingEnvironment evn)
        {
            _servicesAds = servicesAds;
            _evn = evn;
            RootPath = _evn.WebRootPath + "/Files";
        }

        public ActionResult Index()
        {
            //ViewBag.Camp = _servicesAds.GetAll().ToList();
            return View();
        }

        [HttpPost]
        [Obsolete]
        [Route("/admin/ads/CreateCampaign")]
        public JsonResult CreateCampaign(AdsEntity camp, IFormFile Banner)
        {
            if (camp.PublishDate < new DateTime(1900, 1, 1))
                camp.PublishDate = camp.CreateDate;//publish now

            if (string.IsNullOrEmpty(camp.ID) || camp.ID == "0")
            {
                camp.CreateDate = DateTime.UtcNow;
                if (Banner != null)
                {
                    camp.Banner=urlBanner(Banner);
                }
                _servicesAds.CreateQuery().InsertOne(camp);
                Dictionary<string, object> response = new Dictionary<string, object>()
                {
                    {"Mess","Thêm chiến dịch thành công" },
                    {"Error","Thêm thất bại" }
                };
                return new JsonResult(response);
            }
            else
            {
                var oldCamp = _servicesAds.GetItemByID(camp.ID);
                camp.CreateDate= oldCamp.CreateDate;
                if (Banner != null)
                {
                 removeBanner(oldCamp.Banner);
                    camp.Banner = urlBanner(Banner);
                }
                else
                {
                    camp.Banner = oldCamp.Banner;
                }
                _servicesAds.Save(camp);
                Dictionary<string, object> response = new Dictionary<string, object>()
                {
                    {"Mess","Sửa chiến dịch thành công" },
                    {"Error","Sửa thất bại" }
                };
                return new JsonResult(response);
            }
        }

        [HttpPost]
        [Obsolete]
        [Route("/admin/ads/DeleteCampaign")]
        public JsonResult DeleteCampaign(DefaultModel model)
        {
            if (model.ArrID.Length <= 0)
            {
                return new JsonResult(null);
            }
            else
            {
                var banner = _servicesAds.GetItemByID(model.ArrID).Banner;
                removeBanner(banner);
                var delete = _servicesAds.Collection.DeleteMany(o => model.ArrID.Split(',').Contains(o.ID));
                return new JsonResult(delete);
            }
        }

        [System.Obsolete]
        [HttpPost]
        [Route("/admin/ads/getAllCampaign")]
        public JsonResult getAllCampaign(DefaultModel model)
        {
            var filter = new List<FilterDefinition<AdsEntity>>();
            //var sort = new List<SortDefinition<NewsEntity>>();

            if (!string.IsNullOrEmpty(model.SearchText))
            {
                filter.Add(Builders<AdsEntity>.Filter.Text(model.SearchText));
            }
            var data = (filter.Count > 0 ? _servicesAds.Collection.Find(Builders<AdsEntity>.Filter.And(filter)) : _servicesAds.GetAll())
                .SortByDescending(t => t.ID);
            model.TotalRecord = data.CountDocuments();
            var DataResponse = data == null || model.TotalRecord <= 0 || model.TotalRecord <= model.PageSize
                ? data.ToList()
                : data.Skip((model.PageIndex) * model.PageSize).Limit(model.PageSize).ToList();
            var response = new Dictionary<string, object>
            {
                { "Data", DataResponse},
                { "Model", model }
            };
            return new JsonResult(response);
        }

        [System.Obsolete]
        [HttpPost]
        [Route("/admin/ads/getDetailAds")]
        public JsonResult getDetailAds(string ID)
        {
            var data = _servicesAds.GetItemByID(ID);
            var response = new Dictionary<string, object>
            {
                { "Data", data}
            };
            return new JsonResult(response);
        }

        [HttpPost]
        [Obsolete]
        public JsonResult Publish(DefaultModel model)
        {
            if (string.IsNullOrEmpty(model.ArrID) || model.ArrID.Length <= 0)
                return new JsonResult(null);

            _servicesAds.ChangeStatus(model.ArrID.Split(',').ToList(), true);
            return new JsonResult("Publish OK");
        }

        [HttpPost]
        [Obsolete]
        public JsonResult UnPublish(DefaultModel model)
        {
            if (string.IsNullOrEmpty(model.ArrID) || model.ArrID.Length <= 0)
                return new JsonResult(null);

            _servicesAds.ChangeStatus(model.ArrID.Split(',').ToList(), false);
            return new JsonResult("UnPublish OK");
        }

        public string urlBanner(IFormFile formFile)
        {
            string _fileName = formFile.FileName;
            var timestamp = DateTime.Now.ToFileTime();
            _fileName = timestamp + "_" + _fileName;

            var _dirPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Image_Ads");
            //var folder = ("Image_Ads/");
            //string uploads = Path.Combine(RootPath, folder);
            if (!Directory.Exists(_dirPath))
            {
                Directory.CreateDirectory(_dirPath);
            }

            //string _path = Path.Combine(Path.Combine(_dirPath, _fileName));
            //using (var stream = new FileStream(_path, FileMode.Create))
            //{
            //    Banner.CopyTo(stream);
            //}
            //return _fileName;
            var standardSize = new SixLabors.Primitives.Size(1920,1080);

            using (Stream inputStream = formFile.OpenReadStream())
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
                    using (var fileStream = new FileStream(Path.Combine(_dirPath, _fileName), FileMode.Create))
                    {
                        image.Save(fileStream, imageEncoder);
                        return $"{_fileName}";
                    }
                }
            }
        }

        public void removeBanner(string Banner)
        {
            if (Banner != null && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Image_Ads", Banner)))
            {
                System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Image_Ads", Banner));
            }
        }
    }
}
