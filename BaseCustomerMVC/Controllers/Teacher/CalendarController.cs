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

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class CalendarController : TeacherController
    {
        private readonly CalendarService _calendarService;
        private readonly CalendarLogService _calendarLogService;
        private readonly CalendarReportService _calendarReportService;
        private readonly CalendarHelper _calendarHelper;
        private readonly ClassService _classService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly ChapterService _chapterService;
        private readonly LessonService _lessonService;
        private readonly TeacherService _teacherService;
        private readonly CenterService _centerService;
        private readonly TeacherHelper _teacherHelper;
        public CalendarController(
            CalendarService calendarService,
            CalendarLogService calendarLogService,
            CalendarReportService calendarReportService,
            CalendarHelper calendarHelper,
            ClassService classService,
            ChapterService chapterService,
            ClassSubjectService classSubjectService,
            LessonService lessonService,
            TeacherService teacherService,
            CenterService centerService,
            TeacherHelper teacherHelper
            )
        {
            this._calendarService = calendarService;
            this._calendarLogService = calendarLogService;
            this._calendarReportService = calendarReportService;
            _calendarHelper = calendarHelper;
            _classService = classService;
            _classSubjectService = classSubjectService;
            _chapterService = chapterService;
            //_scheduleService = scheduleService;
            _lessonService = lessonService;
            _teacherService = teacherService;
            _centerService = centerService;
            _teacherHelper = teacherHelper;
        }

        public IActionResult Index(DefaultModel model, string basis)
        {
            string _teacherid = User.Claims.GetClaimByType("UserID").Value;
            var teacher = _teacherService.GetItemByID(_teacherid);
            //if (teacher != null)
            //    ViewBag.AllCenters = teacher.Centers;

            //if (!string.IsNullOrEmpty(basis))
            //{
            //    var center = _centerService.GetItemByCode(basis);
            //    if (center != null)
            //        ViewBag.Center = center;
            //    ViewBag.IsHeadTeacher = _teacherHelper.HasRole(_teacherid, center.ID, "head-teacher");
            //}
            var listClass = _classService.Collection.Find(o => o.Members.Any(t => t.TeacherID == _teacherid) && o.IsActive == true)?
                .SortBy(o => o.EndDate)
                .ToList();
            ViewBag.CurrentTeacher = teacher;
            ViewBag.ClassList = listClass;
            ViewBag.Model = model;
            return View();
        }
        [Obsolete]
        public Task<JsonResult> GetList(DefaultModel model, DateTime start, DateTime end, string basis)
        {
            try
            {
                if (!string.IsNullOrEmpty(basis))
                {
                    var center = _centerService.GetItemByCode(basis);
                    var userId = User?.FindFirst("UserID").Value;
                    if (center != null && userId != null)
                    {
                        var listClass = _classService.Collection.Find(o => o.Members.Any(t => t.TeacherID == userId && t.Type == ClassMemberType.TEACHER) && o.Center == center.ID)?.ToList();
                        if (listClass == null || listClass.Count <= 0) return Task.FromResult(new JsonResult(new { }));
                        var data = _calendarHelper.GetListEvent(start, end, listClass.Select(o => o.ID).ToList(), userId);
                        if (data == null) return Task.FromResult(new JsonResult(new { }));
                        return Task.FromResult(new JsonResult(data));
                    }
                }
                return Task.FromResult(new JsonResult(new { }));
            }
            catch (Exception e)
            {
                return Task.FromResult(new JsonResult(new { msg = e.Message }));
            }
        }

        [HttpPost]
        [Obsolete]
        public Task<JsonResult> GetDetail(string id)
        {
            var map = new MappingEntity<CalendarEntity, CalendarViewModel>();
            var calendar = _calendarService.GetItemByID(id);
            var data = new CalendarViewModel();
            data = map.AutoOrtherType(calendar, data);
            if (!string.IsNullOrEmpty(data.ScheduleID))
            {
                if (string.IsNullOrEmpty(data.Path))
                {
                    var lesson = string.IsNullOrEmpty(data.ScheduleID) ? null : _lessonService.GetItemByID(data.ScheduleID);
                    //schedule -> LessonID + class -> classubject -> skill
                    if (lesson != null && string.IsNullOrEmpty(calendar.Content))
                    {
                        string url = $"{Url.Action("Detail", "Lesson")}/{lesson.ID}/{lesson.ClassSubjectID}";
                        var content = "";
                        var course = _classSubjectService.GetItemByID(lesson.ClassSubjectID);
                        if (course != null)
                            content += course.CourseName;
                        if (lesson.ChapterID != "0")
                        {
                            var chap = _chapterService.GetItemByID(lesson.ChapterID);
                            if (chap != null)
                                content += " - " + chap.Name;
                        }
                        calendar.Content = content;
                        calendar.Path = url;
                        _calendarService.Save(calendar);
                        data.Content = calendar.Content;
                        data.LinkLesson = url;
                    }
                }
                else
                    data.LinkLesson = data.Path;
            }
            if (data.UserBook == null || data.UserBook.Count == 0)
            {

            }
            // scheduleId => classID + lesson ID => student/lesson/detail/lessonid/classsubject;

            return Task.FromResult(new JsonResult(data));
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
                item.Created = DateTime.UtcNow;
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

        [Obsolete]
        public JsonResult FixCalendar()
        {
            _ = _calendarHelper.ScheduleAutoConvertEvent();
            return Json("Fixed");
        }

    }
}
