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
        private readonly StudentService _studentService;
        public CalendarHelper(
            CalendarService calendarService,
            LessonScheduleService lessonScheduleService,
            LessonService lessonService,
            ClassService classService,
            TeacherService teacherService,
            StudentService studentService
            )
        {
            _calendarService = calendarService;
            _lessonService = lessonService;
            _classService = classService;
            _teacherService = teacherService;
            _lessonScheduleService = lessonScheduleService;
            _studentService = studentService;
        }
        public Task<CalendarEntity> CreateEvent(CalendarEntity item)
        {
            // kiem tra co phai sua event hay ko ?
            if (!string.IsNullOrEmpty(item.ID))
            {
                // update event
                _calendarService.CreateOrUpdate(item);
                return Task.FromResult(item);
            }
            // check event da ton tai hay chua
            if (existEvent(item.EndDate, item.StartDate, item.GroupID))
            {
                item.Created = DateTime.Now;
                if (item.Status == 5)
                {
                    var teacher = _teacherService.GetItemByID(item.CreateUser);
                    if (teacher != null)
                    {
                        item.TeacherID = teacher.ID;
                        item.TeacherName = teacher.FullName;
                    }
                }
                _calendarService.CreateOrUpdate(item);

                return Task.FromResult(item);
            }
            return Task.FromResult<CalendarEntity>(null);
        }

        public Task<CalendarEntity> CreateEventClass(CalendarEntity item)
        {
            // kiem tra co phai sua event hay ko ?
            if (!string.IsNullOrEmpty(item.ID))
            {
                // update event
                _calendarService.CreateOrUpdate(item);
                return Task.FromResult(item);
            }
            // check event da ton tai hay chua
            if (existEvent(item.EndDate, item.StartDate, item.GroupID))
            {
                item.Status = 5;
                _calendarService.CreateOrUpdate(item);

                return Task.FromResult(item);
            }
            return Task.FromResult<CalendarEntity>(null);
        }

        public Task<bool> RemoveEvent(string id, string user)
        {
            var delItem = _calendarService.GetItemByID(id);

            if (delItem != null)
            {
                // neu event ko phai cua user hoac da het thoi gian thi ko the huy
                if (delItem.StartDate <= DateTime.Now || delItem.CreateUser != user) return Task.FromResult(false);
                delItem.IsDel = true;
                _calendarService.CreateOrUpdate(delItem);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        private bool existEvent(DateTime startDate, DateTime endDate, string classID)
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
        public List<CalendarEventModel> GetListEvent(DateTime startDate, DateTime endDate, List<string> classList)
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
            var DataResponse = data == null || data.Count() <= 0 ? null : data.ToList().Select(o => new CalendarEventModel()
            {
                end = o.EndDate,
                start = o.StartDate,
                groupid = o.GroupID,
                id = o.ID,
                title = o.Title,
                url = "",
                Status = o.Status
            }).ToList();
            return DataResponse;
        }
        [Obsolete]
        public List<CalendarEventModel> GetListEvent(DateTime startDate, DateTime endDate, List<string> classList, string userid)
        {
            bool isTeacher = _teacherService.GetItemByID(userid) != null;
            if (classList == null) classList = new List<string>();
            var filter = new List<FilterDefinition<CalendarEntity>>();
            if (startDate > DateTime.MinValue && endDate > DateTime.MinValue)
            {
                var _startDate = new DateTime(startDate.Year, startDate.Month, 1, 0, 0, 0);
                var _endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);
                filter.Add(Builders<CalendarEntity>.Filter.Where(o => (o.StartDate >= _startDate && o.EndDate <= _endDate)));
            }
            else
            {
                startDate = DateTime.Now;
                endDate = startDate.AddMonths(1);
                var _startDate = new DateTime(startDate.Year, startDate.Month, 1, 0, 0, 0);
                var _endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);
                filter.Add(Builders<CalendarEntity>.Filter.Where(o => (o.StartDate >= _startDate && o.EndDate <= _endDate)));
            }
            filter.Add(Builders<CalendarEntity>.Filter.Where(o => (o.IsDel == false) && (o.CreateUser == userid || classList.Contains(o.GroupID))));
            var data = filter.Count > 0 ? _calendarService.Collection.Find(Builders<CalendarEntity>.Filter.And(filter)) : _calendarService.GetAll();
            var DataResponse = data == null || data.Count() <= 0 ? null : data.ToList().Select(o => new CalendarEventModel()
            {
                start = o.StartDate,
                groupid = o.GroupID,
                id = o.ID,
                title = o.Title,
                url = "",
                skype = "",//isTeacher && o.Status != 5 ? _studentService.GetItemByID(o.StudentID)?.Skype  : _teacherService.GetItemByID(o.TeacherID)?.Skype,
                Status = o.Status,
                Color = o.Status == 5 ? "#ccc" : ""
            }).ToList();
            return DataResponse;
        }

        public CalendarEntity GetByScheduleId(string scheduleID)
        {
            return _calendarService.CreateQuery().Find(t => t.ScheduleID == scheduleID).SingleOrDefault();
        }

        public long Remove(string ID)
        {
            return _calendarService.CreateQuery().DeleteMany(t => t.ID == ID).DeletedCount;
        }

        [Obsolete]
        public async Task ScheduleAutoConvertEvent()
        {
            await _calendarService.RemoveAllAsync();
            var data = _lessonScheduleService.GetAll()?.ToList();
            for (int i = 0; data != null && i < data.Count(); i++)
            {
                var item = data[i];
                if (item.IsActive)
                    ConvertCalendarFromSchedule(item, "");
            }
            await Task.CompletedTask;
        }

        public bool ConvertCalendarFromSchedule(LessonScheduleEntity item, string userCreate)
        {
            if (item.StartDate == DateTime.MinValue) return false;
            var lesson = _lessonService.GetItemByID(item.LessonID);
            if (lesson == null)
                return false;
            var ourClass = _classService.GetItemByID(item.ClassID);
            if (ourClass == null)
                return false;
            var teacher = _teacherService.GetItemByID(ourClass.TeacherID);
            if (teacher == null)
                return false;
            CalendarEntity oldItem = _calendarService.CreateQuery().Find(o => o.ScheduleID == item.ID && o.GroupID == item.ClassID)?.FirstOrDefault();
            CalendarEntity calendar;
            if (oldItem == null)
            {
                calendar = new CalendarEntity()
                {
                    Created = DateTime.Now,
                    CreateUser = userCreate,
                    EndDate = item.StartDate,
                    StartDate = item.StartDate,
                    GroupID = item.ClassID,
                    Title = lesson.Title,
                    TeacherID = teacher.ID,
                    TeacherName = teacher.FullName,
                    Skype = teacher.Skype,//TODO: kiểm tra tại thời điểm call giáo viên thay skype ?
                    Status = 0,
                    LimitNumberUser = 0,
                    UrlRoom = string.Empty,
                    UserBook = new List<string>(),
                    ScheduleID = item.ID
                };
            }
            else
            {
                calendar = new CalendarEntity()
                {
                    ID = oldItem.ID,
                    Created = oldItem.Created,
                    CreateUser = userCreate,
                    EndDate = item.StartDate,
                    StartDate = item.StartDate,
                    GroupID = item.ClassID,
                    Title = lesson.Title,
                    TeacherID = teacher.ID,
                    TeacherName = teacher.FullName,
                    Skype = teacher.Skype,//TODO: kiểm tra tại thời điểm call giáo viên thay skype ?
                    Status = oldItem.Status,
                    LimitNumberUser = oldItem.LimitNumberUser,
                    UrlRoom = oldItem.UrlRoom,
                    UserBook = oldItem.UserBook,
                    ScheduleID = item.ID
                };
            }
            _calendarService.CreateOrUpdate(calendar);
            return true;
        }
    }
}
