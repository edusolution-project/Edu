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

namespace BaseCustomerMVC.Controllers.Student
{
    public class CalendarController : StudentController
    {
        private readonly CalendarService _calendarService;
        private readonly CalendarLogService _calendarLogService;
        private readonly CalendarReportService _calendarReportService;
        private readonly CalendarHelper _calendarHelper;
        private readonly ClassService _classService;
        private readonly ClassStudentService _classStudentService;
        private readonly TeacherService _teacherService;
        private readonly StudentService _studentService;
        private readonly LessonScheduleService _scheduleService;
        public CalendarController(
            CalendarService calendarService,
            CalendarLogService calendarLogService,
            CalendarReportService calendarReportService,
            CalendarHelper calendarHelper,
             ClassStudentService classStudentService,
            ClassService classService,
            TeacherService teacherService,
            StudentService studentService,
            LessonScheduleService scheduleService
            )
        {
            this._calendarService = calendarService;
            this._calendarLogService = calendarLogService;
            this._calendarReportService = calendarReportService;
            _calendarHelper = calendarHelper;
            _classService = classService;
            _teacherService = teacherService;
            _studentService = studentService;
            _scheduleService = scheduleService;
            _classStudentService = classStudentService;
        }

        public IActionResult Index(DefaultModel model)
        {
            ViewBag.Model = model;
            return View();
        }
        [Obsolete]
        public Task<JsonResult> GetList(DefaultModel model)
        {
            if (!User.Identity.IsAuthenticated) return Task.FromResult(new JsonResult(new { code = 540}));
            var userId = User?.FindFirst("UserID").Value;
            var currentStudent = _studentService.GetItemByID(userId);
            if (currentStudent == null || currentStudent.JoinedClasses == null) return Task.FromResult(new JsonResult(new { }));
            var data = _calendarHelper.GetListEvent(model.Start, model.End, currentStudent.JoinedClasses.ToList(), userId);
            if(data == null) return Task.FromResult(new JsonResult(new { }));
            return Task.FromResult(new JsonResult(data));
        }
        [HttpPost]
        [Obsolete]
        public Task<JsonResult> GetDetail(string id)
        {
            var map = new MappingEntity<CalendarEntity, CalendarViewModel>();
            var DataResponse = _calendarService.GetItemByID(id);
            var data = new CalendarViewModel();
            data = map.AutoOrtherType(DataResponse, data);
            // scheduleId => classID + lesson ID => student/lesson/detail/lessonid/classsubject;
            var schedule = string.IsNullOrEmpty(DataResponse.ScheduleID) ? null : _scheduleService.GetItemByID(DataResponse.ScheduleID);
            string url = schedule != null ? Url.Action("Detail", "Lesson", new { id = schedule.LessonID, ClassID = schedule.ClassSubjectID }) : null;
            data.LinkLesson = url;
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
                item.Created = DateTime.Now;
                // check validate
            }
            var data = _calendarHelper.CreateEvent(item).Result;
            if(data == null)
            {
                return new JsonResult(new
                {
                    code = 400,
                    msg = "đã có event tồn tại",
                    data = data
                });
            }

            return new JsonResult(new {
                code =201,
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
