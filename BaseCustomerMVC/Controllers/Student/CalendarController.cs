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
using BaseCustomerEntity.Globals;
using System.Drawing.Printing;

namespace BaseCustomerMVC.Controllers.Student
{
    public class CalendarController : StudentController
    {
        private readonly CalendarService _calendarService;
        private readonly CalendarLogService _calendarLogService;
        private readonly CalendarReportService _calendarReportService;
        private readonly CalendarHelper _calendarHelper;
        private readonly ClassService _classService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly ClassGroupService _classGroupService;
        private readonly LessonService _lessonService;
        private readonly CenterService _centerService;
        //private readonly ClassStudentService _classStudentService;
        private readonly TeacherService _teacherService;
        private readonly StudentService _studentService;
        //private readonly LessonScheduleService _scheduleService;
        public CalendarController(
            CalendarService calendarService,
            CalendarLogService calendarLogService,
            CalendarReportService calendarReportService,
            CalendarHelper calendarHelper,
            ClassGroupService classGroupService,
            //ClassStudentService classStudentService,
            ClassService classService,
            ClassSubjectService classSubjectService,
            LessonService lessonService,
            TeacherService teacherService,
            StudentService studentService,
            //LessonScheduleService scheduleService,
            CenterService centerService
            )
        {
            this._calendarService = calendarService;
            this._calendarLogService = calendarLogService;
            this._calendarReportService = calendarReportService;
            _calendarHelper = calendarHelper;
            _classService = classService;
            _classSubjectService = classSubjectService;
            _teacherService = teacherService;
            _classGroupService = classGroupService;
            _studentService = studentService;
            _lessonService = lessonService;
            //_scheduleService = scheduleService;
            _centerService = centerService;
            //_classStudentService = classStudentService;
        }

        public IActionResult Index(DefaultModel model)
        {
            ViewBag.Model = model;
            return View();
        }

        //EDIT: get directly from lesson
        public Task<JsonResult> GetList(DefaultModel model, string basis)
        {
            if (!User.Identity.IsAuthenticated) return Task.FromResult(new JsonResult(new { code = 540 }));
            if (!string.IsNullOrEmpty(basis))
            {
                var center = _centerService.GetItemByCode(basis);
                var userId = User?.FindFirst("UserID").Value;
                var currentStudent = _studentService.GetItemByID(userId);
                if (currentStudent == null || currentStudent.JoinedClasses == null) return Task.FromResult(new JsonResult(new { }));
                var classIds = _classService.GetItemsByIDs(currentStudent.JoinedClasses).Where(t => t.Center == center.ID).Select(t => t.ID).ToList();


                var listClassID = _classService.Collection.Find(o => o.Center == center.ID && currentStudent.JoinedClasses.Contains(o.ID))?.Project(t => t.ID).ToList();
                if (listClassID == null || listClassID.Count <= 0) return Task.FromResult(new JsonResult(new { }));

                var activeLessons = _lessonService.GetClassActiveLesson(model.Start, model.End, listClassID);

                if (activeLessons.Any())
                {
                    List<StudentLessonScheduleViewModel> listSchedule = new List<StudentLessonScheduleViewModel>();

                    var studentGroups = _classGroupService.GetByClassIDs(listClassID).Where(t => t.Members != null && t.Members.Any(m => m.MemberID == userId)).Select(t => t.ID).ToList();

                    var data = new List<CalendarEventModel>();
                    if (studentGroups == null || studentGroups.Count == 0)
                        data = activeLessons.Where(t => t.GroupIDs == null).Select(t => _calendarHelper.ConvertEventFromLesson(t)).ToList();
                    else
                        data = activeLessons.Where(t => t.GroupIDs == null || studentGroups.Intersect(t.GroupIDs).Any()).Select(t => _calendarHelper.ConvertEventFromLesson(t)).ToList();
                    return Task.FromResult(new JsonResult(data));
                }
                ////var data = _calendarHelper.GetListEvent(model.Start, model.End, classIds);

                //if (data == null) return Task.FromResult(new JsonResult(new { }));
                //return Task.FromResult(new JsonResult(data));
                return Task.FromResult(new JsonResult(new { }));
            }
            return Task.FromResult(new JsonResult(new { code = 403 }));
        }

        [HttpPost]//get directly from lesson
        [Obsolete]
        public Task<JsonResult> GetDetail(string id)
        {

            var lesson = _lessonService.GetItemByID(id);
            var data = new CalendarViewModel();
            if (lesson != null)
            {
                string url = $"{Url.Action("Detail", "Lesson")}/{lesson.ID}/{lesson.ClassSubjectID}";
                var content = "";
                var course = _classSubjectService.GetItemByID(lesson.ClassSubjectID);
                //if (course != null)
                //    content += course.CourseName;
                //if (lesson.ChapterID != "0")
                //{
                //    var chap = _chapterService.GetItemByID(lesson.ChapterID);
                //    if (chap != null)
                //        content += " - " + chap.Name;
                //}
                var teacher = _teacherService.GetItemByID(course.TeacherID);
                data.ID = lesson.ID;
                data.TeacherID = course.TeacherID;
                data.TeacherName = teacher?.FullName;
                data.StartDate = lesson.StartDate;
                data.EndDate = lesson.EndDate;
                data.Title = lesson.Title;
                data.GroupID = lesson.ClassID;
                data.Status = lesson.IsOnline ? 5 : 0;
                data.UrlRoom = teacher?.ZoomID;
                data.Content = content;
                data.LinkLesson = url;
            }

            //var map = new MappingEntity<CalendarEntity, CalendarViewModel>();
            //var DataResponse = _calendarService.GetItemByID(id);
            //var data = new CalendarViewModel();
            //data = map.AutoOrtherType(DataResponse, data);
            //// scheduleId => classID + lesson ID => student/lesson/detail/lessonid/classsubject;
            //var schedule = string.IsNullOrEmpty(DataResponse.ScheduleID) ? null : _lessonService.GetItemByID(DataResponse.ScheduleID);
            //string url = schedule != null ? Url.Action("Detail", "Lesson", new { id = schedule.ID, ClassID = schedule.ClassSubjectID }) : null;
            //data.LinkLesson = url;
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
