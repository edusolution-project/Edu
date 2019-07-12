using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using MongoDB.Driver;
using System.Text;
using System.Linq;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class LessonController : TeacherController
    {
        private readonly GradeService _gradeService;
        private readonly SubjectService _subjectService;
        private readonly TeacherService _teacherService;
        private readonly ClassService _classService;
        private readonly CourseService _courseService;
        private readonly ChapterService _chapterService;
        private readonly LessonService _service;
        private readonly LessonScheduleService _lessonScheduleService;

        public LessonController(GradeService gradeservice
           , SubjectService subjectService, TeacherService teacherService, ClassService classService,
            CourseService courseService, ChapterService chapterService, LessonService service, LessonScheduleService lessonScheduleService)
        {
            _gradeService = gradeservice;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _courseService = courseService;
            _classService = classService;
            _chapterService = chapterService;
            _service = service;
            _lessonScheduleService = lessonScheduleService;
        }

        public IActionResult Detail(DefaultModel model, string ClassID)
        {
            if (model == null) return null;

            if (ClassID == null)
                return RedirectToAction("Index", "Class");
            var currentClass = _classService.GetItemByID(ClassID);
            if (currentClass == null)
                return RedirectToAction("Index", "Class");
            var Data = _service.GetItemByID(model.ID);
            if (Data == null)
                return RedirectToAction("Index", "Class");
            ViewBag.Class = currentClass;
            ViewBag.Data = Data;
            if (Data.TemplateType == LESSON_TEMPLATE.LECTURE)
                return View();
            else
                return View("Exam");
        }


        [HttpPost]
        public JsonResult GetDetailsLesson(string ID)
        {
            try
            {
                var lesson = _service.CreateQuery().Find(o => o.ID == ID).FirstOrDefault();

                var response = new Dictionary<string, object>
                {
                    { "Data", lesson }
                };
                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", null },
                    {"Error", ex.Message }
                });
            }
        }


        [HttpPost]
        public JsonResult CreateOrUpdateLesson(LessonEntity item)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var data = _service.CreateQuery().Find(o => o.ID == item.ID).SingleOrDefault();
                if (data == null)
                {
                    item.Created = DateTime.Now;
                    item.CreateUser = UserID;
                    item.IsAdmin = true;
                    item.IsActive = false;
                    item.IsParentCourse = item.ChapterID.Equals("0");
                    item.Updated = DateTime.Now;
                    item.Order = 0;
                    var maxItem = new LessonEntity();
                    if (item.IsParentCourse)
                        maxItem = _service.CreateQuery().Find(o => o.CourseID == item.CourseID && o.IsParentCourse).SortByDescending(o => o.Order).FirstOrDefault();
                    else
                        maxItem = _service.CreateQuery().Find(o => o.ChapterID == item.ChapterID).SortByDescending(o => o.Order).FirstOrDefault();
                    if (maxItem != null)
                    {
                        item.Order = maxItem.Order + 1;
                    }
                    _service.CreateQuery().InsertOne(item);
                }
                else
                {
                    item.Updated = DateTime.Now;
                    item.Order = data.Order;
                    _service.CreateQuery().ReplaceOne(o => o.ID == item.ID, item);
                }

                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", item },
                    {"Error",null }
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", null },
                    {"Error",ex.Message }
                });
            }

        }

        public IActionResult Exam()
        {
            return View();
        }



    }
}
