using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using MongoDB.Driver;
using System.Text;
using System.Linq;
using Core_v2.Globals;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using BaseCustomerMVC.Controllers.Student;
using BaseCustomerEntity.Globals;
using EasyZoom.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Transactions;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class ProductController: TeacherController
    {
        private readonly FileProcess _fileProcess;
        private readonly TeacherService _teacherService;
        private readonly TeacherHelper _teacherHelper;
        private readonly AccountService _accountService;
        private readonly CenterService _centerService;
        private readonly NewsService _newsService;
        private readonly StudentService _studentService;
        private readonly ClassService _classService;
        private readonly HistoryTransactionService _historyTransactionService;
        private readonly MappingEntity<NewsEntity, NewsViewModel> _mapping;
        private readonly MappingEntity<HistoryTransactionEntity, HistoryTransactionViewModel> _mapping_H;
        private readonly ISession _session;

        public DefaultConfigs _default { get; }

        public ProductController(
            FileProcess fileProcess,
            TeacherService teacherService,
            AccountService accountService,
            CenterService centerService,
            HistoryTransactionService historyTransactionService,
            NewsService newsService,
            StudentService studentService,
            ClassService classService,
            IHttpContextAccessor httpContextAccessor,
            TeacherHelper teacherHelper,
            IOptions<DefaultConfigs> defaultvalue)
        {
            _teacherService = teacherService;
            _teacherHelper = teacherHelper;
            _accountService = accountService;
            _fileProcess = fileProcess;
            _centerService = centerService;
            _session = httpContextAccessor.HttpContext.Session;
            _default = defaultvalue.Value;
            _historyTransactionService = historyTransactionService;
            _newsService = newsService;
            _studentService = studentService;
            _mapping = new MappingEntity<NewsEntity, NewsViewModel>();
            _mapping_H = new MappingEntity<HistoryTransactionEntity, HistoryTransactionViewModel>();
            _classService = classService;
        }

        public IActionResult Index(string basis)
        {
            string _teacherid = User.Claims.GetClaimByType("UserID").Value;
            var teacher = _teacherService.GetItemByID(_teacherid);
            //if(teacher.Centers)
            var center = _centerService.GetItemByCode(basis);
            //var list_products = _newsService.CreateQuery().Find(o => o.CenterID == center.ID || o.Targets!=null && o.Targets.Where(x => x == center.ID) != null).ToList();

            var data = _newsService.CreateQuery().Find(o => o.Type == "san-pham" && o.CenterID==center.ID).Limit(10);
            var list_products = data.ToList();

            var DataResponse =
                from t in list_products
                select _mapping.AutoOrtherType(t, new NewsViewModel()
                {
                    ClassName = t.ClassID == null || t.ClassID == "0" || t.ClassID == "" ? null : _classService.GetItemByID(t.ClassID).Name,
                    CenterName = t.CenterID == null || t.CenterID == "0" || t.CenterID == "" ? null : _centerService.GetItemByID(t.CenterID).Name,
                    TotalPrice= _historyTransactionService.CreateQuery().Find(o=>o.NewsID==t.ID).Project(o=>o.Price).ToList().Sum(o=>o),
                    Students= _historyTransactionService.CreateQuery().Find(o => o.NewsID == t.ID).CountDocuments()
                });
            ViewBag.ListHistory = DataResponse.ToList();
            //var a=DataResponse.ToList();
            return View();
        }

        [HttpPost]
        public JsonResult GetListProduct(DefaultModel model, string Center, string SubjectID = "", string GradeID = "")
        {
            string _teacherid = User.Claims.GetClaimByType("UserID").Value;
            var teacher = _teacherService.GetItemByID(_teacherid);
            var center = _centerService.GetItemByCode(Center);

            var list_products = _newsService.CreateQuery().Find(o => o.Type == "san-pham" && o.CenterID == center.ID).Skip(model.PageIndex * model.PageSize).Limit(model.PageSize).ToList();
            var DataResponse =
               from t in list_products
               select _mapping.AutoOrtherType(t, new NewsViewModel()
               {
                   ClassName = t.ClassID == null || t.ClassID == "0" || t.ClassID == "" ? null : _classService.GetItemByID(t.ClassID).Name,
                   CenterName = t.CenterID == null || t.CenterID == "0" || t.CenterID == "" ? null : _centerService.GetItemByID(t.CenterID).Name,
                   TotalPrice = _historyTransactionService.CreateQuery().Find(o => o.NewsID == t.ID).Project(o => o.Price).ToList().Sum(o => o),
                   Students = _historyTransactionService.CreateQuery().Find(o => o.NewsID == t.ID).CountDocuments()
               });
            ViewBag.ListHistory = DataResponse.ToList();
            return null;
        }

        [HttpPost]
        public JsonResult GetDetail(string ID,string Center)
        {
            var center = _centerService.GetItemByCode(Center);
            var data = _historyTransactionService.CreateQuery().Find(o => o.NewsID == ID && o.CenterID == center.ID).ToList();
            var ViewDetail =
                from t in data
                select _mapping_H.AutoOrtherType(t, new HistoryTransactionViewModel()
                {
                    StudentName=t.StudentID==null?null: _studentService.CreateQuery().Find(o=>o.ID==t.StudentID).FirstOrDefault().FullName,
                    ProductName = t.NewsID == null ? null : _newsService.GetItemByID(t.NewsID).Title,
                    ClassName=t.NewsID==null?null: _classService.CreateQuery().Find(o=>o.ID==_newsService.GetItemByID(t.NewsID).ClassID).FirstOrDefault().Name,
                }
                );
            var ViewProduct = _newsService.CreateQuery().Find(o => o.ID == ID).FirstOrDefault();

            var DataRespone = new Dictionary<string, object>()
            {
                {"ViewDetail",ViewDetail },
                {"Error","" },
                {"ViewProduct", ViewProduct}
            };
            return Json(DataRespone);
        }
    }
}
