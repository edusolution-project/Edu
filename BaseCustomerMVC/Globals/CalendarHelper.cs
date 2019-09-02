using BaseCustomerEntity.Database;
using BaseCustomerMVC.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseCustomerMVC.Globals
{
    public class CalendarHelper
    {
        private readonly CalendarService _calendarService;
        private readonly LessonService _lessonService;
        private readonly ClassService _classService;
        private readonly TeacherService _teacherService;
        private readonly LessonScheduleService _lessonScheduleService;

        [Obsolete]
        public CalendarHelper(
            CalendarService calendarService, 
            LessonScheduleService lessonScheduleService,
            LessonService lessonService,
            ClassService classService,
            TeacherService teacherService
            )
        {
            _calendarService = calendarService;
            _lessonService = lessonService;
            _classService = classService;
            _teacherService = teacherService;
            _lessonScheduleService = lessonScheduleService;
            ScheduleAutoConvertEvent();
        }
        public Task<CalendarEntity> CreateEvent(CalendarEntity item)
        {
            if (existEvent(item.EndDate, item.StartDate, item.GroupID))
            {
                _calendarService.CreateOrUpdate(item);

                return Task.FromResult(item);
            }
            return Task.FromResult<CalendarEntity>(null);
        }
        public Task<bool> RemoveEvent(string id)
        {
            var delItem = _calendarService.GetItemByID(id);
            if (delItem != null)
            {
                delItem.IsDel = true;
                _calendarService.CreateOrUpdate(delItem);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        private bool existEvent(DateTime startDate, DateTime endDate,string classID)
        {
            var filter = new List<FilterDefinition<CalendarEntity>>
            {
                //o.StartDate >= new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0)
                Builders<CalendarEntity>.Filter.Where(o => o.StartDate >= startDate),
                //new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59)
                Builders<CalendarEntity>.Filter.Where(o => o.EndDate <= endDate),
                Builders<CalendarEntity>.Filter.Where(o => o.GroupID == classID)
            };
            var data = _calendarService.Collection.Find(Builders<CalendarEntity>.Filter.And(filter))?.ToList();
            return data == null || data.Count() == 0;
        }

        [Obsolete]
        public List<CalendarEventModel> GetListEvent(DateTime startDate,DateTime endDate,List<string> classList)
        {
            var filter = new List<FilterDefinition<CalendarEntity>>();
            //if (classList != null && classList.Count > 0)
            //{
            //    filter.Add(Builders<CalendarEntity>.Filter.Where(o => classList.Contains(o.GroupID)));
            //}
            if (startDate > DateTime.MinValue && endDate > DateTime.MinValue)
            {
                var _startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
                var _endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);
                filter.Add(Builders<CalendarEntity>.Filter.Where(o => o.StartDate >= _startDate || o.EndDate <= _endDate));
            }
            filter.Add(Builders<CalendarEntity>.Filter.Where(o => o.IsDel == false));
            var data = filter.Count > 0 ? _calendarService.Collection.Find(Builders<CalendarEntity>.Filter.And(filter)) : _calendarService.GetAll();
            var DataResponse = data == null || data.Count() <= 0 ? null : data.ToList().Select(o=> new CalendarEventModel() {
                end = o.EndDate,
                start = o.StartDate,
                groupid = o.GroupID,
                id = o.ID,
                title = o.Title,
                url = o.UrlRoom == null ? "" : o.UrlRoom
            }).ToList();
            return DataResponse;
        }

        [Obsolete]
        public Task ScheduleAutoConvertEvent()
        {
           var data = _lessonScheduleService.GetAll()?.ToList();
            for (int i = 0; data != null && i < data.Count() ; i++)
            {
                var item = data[i];
                ConvertCalendarFromSchedule(item, "");
            }
            return Task.CompletedTask;
        }
        public bool ConvertCalendarFromSchedule(LessonScheduleEntity item, string userCreate)
        {
            var lesson = _lessonService.GetItemByID(item.LessonID);
            if (lesson == null) return false;
            var ourClass = _classService.GetItemByID(item.ClassID);
            if (ourClass == null) return false;
            var teacher = _teacherService.GetItemByID(ourClass.TeacherID);
            if (teacher == null) return false;
            var calendar = new CalendarEntity()
            {
                Created = DateTime.Now,
                CreateUser = userCreate,
                EndDate = item.EndDate,
                StartDate = item.StartDate,
                GroupID = item.ClassID,
                Title = lesson.Title,
                TeacherID = teacher.ID,
                TeacherName = teacher.FullName,
                Status = 0,
                LimitNumberUser = 0,
                UrlRoom = string.Empty,
                UserBook = new List<string>()
            };
            if (existEvent(calendar.StartDate, calendar.EndDate, calendar.GroupID))
            {
                _calendarService.CreateOrUpdate(calendar);
                return true;
            }
            return false;
        }
    }
}
