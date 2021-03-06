using BaseCustomerEntity.Database;
using BaseCustomerMVC.Models;
using EasyZoom.Interfaces;
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
        private readonly SkillService _skillService;
        private readonly ChapterService _chapterService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly TeacherService _teacherService;
        //private readonly LessonScheduleService _lessonScheduleService;
        private readonly StudentService _studentService;

        private readonly IZoomHelpers _zoomHelpers;

        public CalendarHelper(
            CalendarService calendarService,
            //LessonScheduleService lessonScheduleService,
            LessonService lessonService,
            ClassService classService,
            SkillService skillService,
            ClassSubjectService classSubjectService,
            ChapterService chapterService,
            TeacherService teacherService,
            StudentService studentService,
            IZoomHelpers zoomHelpers
            )
        {
            _calendarService = calendarService;
            _lessonService = lessonService;
            _classService = classService;
            _skillService = skillService;
            _classSubjectService = classSubjectService;
            _chapterService = chapterService;
            _teacherService = teacherService;
            //_lessonScheduleService = lessonScheduleService;
            _studentService = studentService;
            _zoomHelpers = zoomHelpers;
        }
        public Task<CalendarEntity> CreateEvent(CalendarEntity item)
        {
            // kiem tra co phai sua event hay ko ?
            if (!string.IsNullOrEmpty(item.ID))
            {
                // update event
                if (string.IsNullOrEmpty(item.UrlRoom) && item.Status == 5)
                {
                    var zoomScheduled = _zoomHelpers.CreateScheduled(item.Title, item.StartDate, 60);
                    item.UrlRoom = zoomScheduled.Id;
                }

                _calendarService.CreateOrUpdate(item);
                return Task.FromResult(item);
            }
            // bỏ check trùng
            // check event da ton tai hay chua
            //if (existEvent(item.EndDate, item.StartDate, item.GroupID))
            //{
            //    //trùng
            //}
            item.Created = DateTime.UtcNow;
            if (item.Status == 5)
            {
                var zoomScheduled = _zoomHelpers.CreateScheduled(item.Title, item.StartDate, 60);
                item.UrlRoom = zoomScheduled.Id;
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

        public Task<CalendarEntity> CreateEventClass(CalendarEntity item)
        {
            // kiem tra co phai sua event hay ko ?
            if (!string.IsNullOrEmpty(item.ID))
            {
                // update event
                _calendarService.CreateOrUpdate(item);
                return Task.FromResult(item);
            }
            //bỏ check trùng
            // check event da ton tai hay chua
            if (existEvent(item.EndDate, item.StartDate, item.GroupID))
            {
                //..trùng
            }

            item.Status = 5;

            var zoomScheduled = _zoomHelpers.CreateScheduled(item.Title, item.StartDate, 60);
            item.UrlRoom = zoomScheduled.Id;

            _calendarService.CreateOrUpdate(item);

            return Task.FromResult(item);
        }

        public Task<bool> RemoveEvent(string id, string user)
        {
            var delItem = _calendarService.GetItemByID(id);

            if (delItem != null)
            {
                // neu event ko phai cua user hoac da het thoi gian thi ko the huy
                if (delItem.StartDate <= DateTime.UtcNow || delItem.CreateUser != user) return Task.FromResult(false);
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
            if (classList != null && classList.Count > 0)
            {
                filter.Add(Builders<CalendarEntity>.Filter.Where(o => classList.Contains(o.GroupID)));
            }
            if (startDate > DateTime.MinValue && endDate > DateTime.MinValue)
            {
                var _startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
                var _endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);
                filter.Add(Builders<CalendarEntity>.Filter.Where(o => o.StartDate >= _startDate || o.EndDate <= _endDate));
            }
            filter.Add(Builders<CalendarEntity>.Filter.Where(o => o.IsDel == false));
            var data = filter.Count > 0 ? _calendarService.Collection.Find(Builders<CalendarEntity>.Filter.And(filter)) : _calendarService.GetAll();
            var DataResponse = data == null || data.Count() <= 0 ? null : data.ToEnumerable().Select(o => new CalendarEventModel()
            {
                end = o.EndDate,
                start = o.StartDate,
                groupid = o.GroupID,
                id = o.ID,
                title = o.Title,
                url = "",
                Status = o.Status,
                Color = o.Status == 5 ? "#ccc" : ""
            }).ToList();
            return DataResponse;
        }
        [Obsolete]
        public List<CalendarEventModel> GetListEvent(DateTime startDate, DateTime endDate, List<string> classList, string userid)
        {
            bool isTeacher = _teacherService.GetItemByID(userid) != null;
            if (classList == null) classList = new List<string>();
            var filter = new List<FilterDefinition<CalendarEntity>>();
            filter.Add(Builders<CalendarEntity>.Filter.Where(o => classList.Contains(o.GroupID)
            //&& (o.TeacherID == userid)
            ));
            if (startDate > DateTime.MinValue && endDate > DateTime.MinValue)
            {
                var _startDate = startDate.AddDays(-1);
                //new DateTime(startDate.Year, startDate.Month, 1, 0, 0, 0);

                var _endDate = endDate.AddDays(1);
                    //new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);
                filter.Add(Builders<CalendarEntity>.Filter.Where(o => (o.StartDate >= _startDate && o.EndDate <= _endDate)));
            }
            else
            {
                startDate = DateTime.UtcNow.AddDays(-1);
                endDate = startDate.AddMonths(1);
                //var _startDate = new DateTime(startDate.Year, startDate.Month, 1, 0, 0, 0);
                //var _endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);
                filter.Add(Builders<CalendarEntity>.Filter.Where(o => (o.StartDate >= startDate && o.EndDate <= endDate)));
            }
            filter.Add(Builders<CalendarEntity>.Filter.Where(o => o.IsDel == false));

            var data = filter.Count > 0 ? _calendarService.Collection.Find(Builders<CalendarEntity>.Filter.And(filter)) : _calendarService.GetAll();

            var DataResponse = new List<CalendarEventModel>();

            if (data == null || data.Count() <= 0)
                DataResponse = null;
            else
                DataResponse =
                    (from r in data.ToEnumerable() //.ToList() //TODO: Too heavy => add info to schedule & add to Event later
                                                   //let schedule = _lessonScheduleService.GetItemByID(r.ScheduleID)
                                                   //let classSbj = schedule != null ? _classSubjectService.GetItemByID(schedule.ClassSubjectID) : null
                                                   //let skill = classSbj != null ? _skillService.GetItemByID(classSbj.SkillID) : null
                     select new CalendarEventModel()
                     {
                         start = r.StartDate,
                         groupid = r.GroupID,
                         id = r.ID,
                         title = r.Title,// + (skill == null ? "" : (" (" + skill.Name + ")")),
                         url = "",
                         skype = "",//isTeacher && o.Status != 5 ? _studentService.GetItemByID(o.StudentID)?.Skype  : _teacherService.GetItemByID(o.TeacherID)?.Skype,
                         Status = r.Status,
                         Color = r.Status == 5 ? "#ccc" : ""
                     }).ToList();
            return DataResponse;
        }

        public CalendarEntity GetByScheduleId(string scheduleID)
        {
            return _calendarService.CreateQuery().Find(t => t.ScheduleID == scheduleID).SortByDescending(t => t.ID).FirstOrDefault();
        }

        public CalendarEntity GetByEventID(string eventID)
        {
            return _calendarService.CreateQuery().Find(t => t.ID == eventID).SingleOrDefault();
        }

        public long Remove(string ID)
        {
            return _calendarService.CreateQuery().DeleteMany(t => t.ID == ID).DeletedCount;
        }

        public long RemoveLessonSchedule(string LessonID)
        {
            return _calendarService.CreateQuery().DeleteMany(t => t.ScheduleID == LessonID).DeletedCount;
        }

        public long RemoveManySchedules(List<string> lessonIds)
        {
            return _calendarService.CreateQuery().DeleteMany(t => lessonIds.Contains(t.ScheduleID)).DeletedCount;
        }

        public void UpdateChapterCalendar(ChapterEntity entity, string UserID)
        {
            var lessons = _lessonService.CreateQuery().Find(t => t.ChapterID == entity.ID && t.ClassSubjectID == entity.ClassSubjectID).ToList();
            foreach (var lesson in lessons)
            {
                lesson.StartDate = entity.StartDate;
                lesson.EndDate = entity.EndDate;
                UpdateCalendar(lesson, UserID);
                _lessonService.Save(lesson);
            }
            var subchaps = _chapterService.GetSubChapters(entity.ClassSubjectID, entity.ID);
            foreach (var subchap in subchaps)
            {
                subchap.StartDate = entity.StartDate;
                subchap.EndDate = entity.EndDate;
                _chapterService.Save(subchap);
                UpdateChapterCalendar(subchap, UserID);
            }
        }

        public void UpdateCalendar(LessonEntity entity, string userid)
        {
            //RemoveLessonSchedule(entity.ID);
            //if (entity.IsActive)
            ConvertCalendarFromLesson(entity, userid);
        }


        [Obsolete]
        public async Task ScheduleAutoConvertEvent()
        {
            await _calendarService.RemoveAllAsync();
            var data = _lessonService.GetAll()?.ToList();
            for (int i = 0; data != null && i < data.Count(); i++)
            {
                var item = data[i];
                //if (item.IsActive)
                ConvertCalendarFromLesson(item, "");
            }
            await Task.CompletedTask;
        }

        public bool ConvertCalendarFromLesson(LessonEntity item, string userCreate)
        {
            if (item.StartDate == DateTime.MinValue) return false;
            //var lesson = _lessonService.GetItemByID(item.LessonID);
            //if (lesson == null)
            //    return false;
            var ourClass = _classService.GetItemByID(item.ClassID);
            if (ourClass == null)
                return false;
            var classSbj = _classSubjectService.GetItemByID(item.ClassSubjectID);
            if (classSbj == null)
                return false;
            var teacher = _teacherService.GetItemByID(classSbj.TeacherID);
            if (teacher == null)
                return false;
            CalendarEntity oldItem = _calendarService.CreateQuery().Find(o => o.ScheduleID == item.ID
            //&& o.GroupID == item.ClassID
            )?.FirstOrDefault();
            CalendarEntity calendar;
            if (oldItem == null)
            {
                calendar = new CalendarEntity()
                {
                    Created = DateTime.UtcNow,
                    CreateUser = userCreate,
                    EndDate = item.EndDate,
                    StartDate = item.StartDate,
                    GroupID = item.ClassID,
                    Title = item.Title,
                    TeacherID = teacher.ID,
                    TeacherName = teacher.FullName,
                    Skype = teacher.Skype,//TODO: kiểm tra tại thời điểm call giáo viên thay skype ?
                    Status = item.IsOnline ? 5 : 0,
                    LimitNumberUser = 0,
                    UrlRoom = item.IsOnline ? (string.IsNullOrEmpty(teacher.ZoomID) ? _zoomHelpers.CreateScheduled(item.Title, item.StartDate, 60).Id : teacher.ZoomID.Replace("-", "")) : "",
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
                    EndDate = item.EndDate,
                    StartDate = item.StartDate,
                    GroupID = item.ClassID,
                    Title = item.Title,
                    TeacherID = teacher.ID,
                    TeacherName = teacher.FullName,
                    Skype = teacher.Skype,//TODO: kiểm tra tại thời điểm call giáo viên thay skype ?
                    Status = item.IsOnline ? 5 : 0,
                    LimitNumberUser = oldItem.LimitNumberUser,
                    UrlRoom = (item.IsOnline && oldItem.Status == 5) ? oldItem.UrlRoom : //not change => keep event
                        item.IsOnline ? (string.IsNullOrEmpty(teacher.ZoomID) ? _zoomHelpers.CreateScheduled(item.Title, item.StartDate, 60).Id : teacher.ZoomID.Replace("-", "")) : "",
                    UserBook = oldItem.UserBook,
                    ScheduleID = item.ID
                };
            }
            _calendarService.CreateOrUpdate(calendar);
            //oldItem.CreateUser = userCreate;
            //oldItem.EndDate = item.EndDate;
            //oldItem.StartDate = item.StartDate;
            //oldItem.GroupID = item.ClassID;
            //oldItem.Title = item.Title;
            //oldItem.TeacherID = teacher.ID;
            //oldItem.TeacherName = teacher.FullName;
            //oldItem.Skype = teacher.Skype;
            //oldItem.Status = item.IsOnline ? 5 : 0;
            return true;
        }
    }
}
