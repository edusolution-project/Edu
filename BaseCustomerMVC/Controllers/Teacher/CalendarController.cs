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

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class CalendarController : TeacherController
    {
        private readonly CalendarService _calendarService;
        private readonly CalendarLogService _calendarLogService;
        private readonly CalendarReportService _calendarReportService;

        public CalendarController(
            CalendarService calendarService,
            CalendarLogService calendarLogService,
            CalendarReportService calendarReportService
            )
        {
            this._calendarService = calendarService;
            this._calendarLogService = calendarLogService;
            this._calendarReportService = calendarReportService;
        }

        public IActionResult Index(DefaultModel model)
        {
            ViewBag.Model = model;
            return View();
        }
        [Obsolete]
        public Task<JsonResult> GetList(DefaultModel model)
        {
            var filter = new List<FilterDefinition<CalendarEntity>>();
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            else
            {
                filter.Add(Builders<CalendarEntity>.Filter.Where(o => o.TeacherID == userId));
            }
            if (model.StartDate > DateTime.MinValue)
            {
                filter.Add(Builders<CalendarEntity>.Filter.Where(o => o.StartDate >= new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0)));
            }
            if (model.EndDate > DateTime.MinValue)
            {
                filter.Add(Builders<CalendarEntity>.Filter.Where(o => o.EndDate <= new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59)));
            }
            var data = filter.Count > 0 ? _calendarService.Collection.Find(Builders<CalendarEntity>.Filter.And(filter)) : _calendarService.GetAll();
            model.TotalRecord = data.Count();
            var DataResponse = data == null || data.Count() <= 0 ? null : data.ToList();
            DataResponse.Select(o => new CalendarEventModel()
            {
                id = o.ID,
                start = o.StartDate,
                end = o.EndDate,
                title = o.Title,
                groupid = o.GroupID,
                url = (o.StartDate >= DateTime.Now && o.EndDate <= DateTime.Now) ? o.UrlRoom : ""
            });
            //var DataResponse = data == null || data.Count() <= 0 || data.Count() < model.PageSize
            //    ? data.ToList()
            //    : data.Skip((model.PageIndex - 1) * model.PageSize).Limit(model.PageSize).ToList();
            //Task.Factory.StartNew(() => {
            //    Task inner = Task.Factory.StartNew(() => { });
            //    return Task.FromResult(new JsonResult(model));
            //});
            return Task.FromResult(new JsonResult(DataResponse));
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
        public CalendarEntity Create(CalendarEntity item)
        {
            // check validate
            if (validate(item.StartDate, item.EndDate)) return _calendarService.CreateOrUpdate(item);
            else return null;
        }
        [HttpPost]
        [Obsolete]
        public bool Delete(string id)
        {
            //validate
            var item = _calendarService.GetItemByID(id);
            if (item == null || item.Status == 1) return false;
            return _calendarService.Remove(id) != null;
        }

        [Obsolete]
        private bool validate(DateTime startDate, DateTime endDate)
        {
            if (startDate == DateTime.MinValue || endDate == DateTime.MinValue) return false;
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (startDate <= DateTime.Now || endDate <= DateTime.Now) return false;
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }
            var filter = new List<FilterDefinition<CalendarEntity>>();
            filter.Add(Builders<CalendarEntity>.Filter.Where(o => o.TeacherID == userId));
            filter.Add(Builders<CalendarEntity>.Filter.Where(o => o.StartDate >= startDate));
            filter.Add(Builders<CalendarEntity>.Filter.Where(o => o.EndDate <= endDate));
            var data = _calendarService.Collection.Find(Builders<CalendarEntity>.Filter.And(filter));
            if(data.Count() <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
