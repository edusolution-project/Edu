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

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class CalendarController : TeacherController
    {
        private readonly CalendarService _calendarService;
        private readonly CalendarLogService _calendarLogService;
        private readonly CalendarReportService _calendarReportService;
        private readonly CalendarHelper _calendarHelper;
        private readonly ClassService _classService;
        public CalendarController(
            CalendarService calendarService,
            CalendarLogService calendarLogService,
            CalendarReportService calendarReportService,
            CalendarHelper calendarHelper,
            ClassService classService
            )
        {
            this._calendarService = calendarService;
            this._calendarLogService = calendarLogService;
            this._calendarReportService = calendarReportService;
            _calendarHelper = calendarHelper;
            _classService = classService;
        }

        public IActionResult Index(DefaultModel model)
        {
            var userId = User?.FindFirst("UserID").Value;
            var listClass = _classService.Collection.Find(o => o.Members.Any(t=> t.TeacherID == userId) && o.IsActive == true)?
                .SortBy(o=>o.EndDate)
                .ToList();

            ViewBag.ClassList = listClass;
            ViewBag.Model = model;
            return View();
        }
        [Obsolete]
        public Task<JsonResult> GetList(DefaultModel model,DateTime start,DateTime end)
        {
            var userId = User?.FindFirst("UserID").Value;
            var listClass = _classService.Collection.Find(o => o.TeacherID == userId)?.ToList();
            if (listClass == null) return Task.FromResult(new JsonResult(null));
            var data = _calendarHelper.GetListEvent(start, end, listClass.Select(o=>o.ID).ToList());
            if(data == null) return Task.FromResult(new JsonResult(new {}));
            return Task.FromResult(new JsonResult(data));
        }

        [HttpPost]
        [Obsolete]
        public Task<JsonResult> GetDetail(string id)
        {
            var DataResponse = _calendarService.GetItemByID(id);
            return Task.FromResult(new JsonResult(DataResponse));
        }
        [HttpPost]
        [Obsolete]
        public JsonResult Create(CalendarEntity item)
        {
            if (string.IsNullOrEmpty(item.ID))
            {
                // nguoi tao
                item.CreateUser = User?.FindFirst("UserID").Value;
                // ngay tao
                item.Created = DateTime.Now;
                // check validate
            }
            // check validate
            var data = _calendarHelper.CreateEvent(item).Result;
            if (data == null)
            {
                return new JsonResult(new
                {
                    code = 400,
                    msg = "đã có event tồn tại",
                    data = data
                });
            }

            return new JsonResult(new
            {
                code = 201,
                msg = "tạo thành công",
                data = data
            });
        }
        [HttpPost]
        [Obsolete]
        public bool Delete(string id)
        {
            return _calendarHelper.RemoveEvent(id, User.FindFirst("UserID").Value).Result;
        }
    }
}
