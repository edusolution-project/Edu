using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Core_v2.Interfaces;
using Core_v2.Globals;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseCustomerMVC.Controllers.Student
{
    [BaseAccess.Attribule.AccessCtrl("Lich học","student")]
    public class CalendarController : StudentController
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
            ViewBag.Model = model;
            return View();
        }
        [Obsolete]
        public Task<JsonResult> GetList(DefaultModel model)
        {
            var userId = User?.FindFirst("UserID").Value;
            var listClass = _classService.Collection.Find(o => o.Students.Contains(userId))?.ToList();
            if (listClass == null) return Task.FromResult(new JsonResult(null));
            var data = _calendarHelper.GetListEvent(model.StartDate, model.EndDate, listClass.Select(o => o.ID).ToList());
            return Task.FromResult(new JsonResult(data));
        }
        [HttpPost]
        [Obsolete]
        public Task<JsonResult> GetDetail(string id)
        {
            var DataResponse = _calendarService.GetItemByID(id);
            return Task.FromResult(new JsonResult(DataResponse));
        }
        //[HttpPost]
        //[Obsolete]
        //public bool Create(CalendarEntity item)
        //{
        //    // check validate
        //    return _calendarHelper.CreateEvent(item).Result;
        //}
        //[HttpPost]
        //[Obsolete]
        //public bool Delete(string id)
        //{
        //    return _calendarHelper.RemoveEvent(id).Result;
        //}
    }
}
