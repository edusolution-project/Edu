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
    public class CalendarController : StudentController
    {
        // bài học hôm nay.
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
        [HttpPost]
        [Obsolete]
        public Task<JsonResult>  GetList(DefaultModel model)
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
            var DataResponse = data == null || data.Count() <= 0 ? null :data.ToList();
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
        public CalendarEntity Create(CalendarEntity item)
        {
            // check validate
            
            return _calendarService.CreateOrUpdate(item);
        }
        [HttpPost]
        public bool Delete(string id)
        {
            //validate
            return _calendarService.Remove(id) != null;
        }


        //private bool validate()
        //{
        //    var filter = new List<FilterDefinition<CalendarEntity>>();
        //    var userId = User.Claims.GetClaimByType("UserID").Value;
        //    if (string.IsNullOrEmpty(userId))
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        filter.Add(Builders<CalendarEntity>.Filter.Where(o => o.TeacherID == userId));
        //    }
        //    if (model.StartDate > DateTime.MinValue)
        //    {
        //        filter.Add(Builders<CalendarEntity>.Filter.Where(o => o.StartDate >= new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0)));
        //    }
        //    if (model.EndDate > DateTime.MinValue)
        //    {
        //        filter.Add(Builders<CalendarEntity>.Filter.Where(o => o.EndDate <= new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59)));
        //    }
        //}
    }
}
