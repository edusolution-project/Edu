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

namespace BaseCustomerMVC.Controllers.Student
{
    public class LessonTodayController : StudentController
    {
        // bài học hôm nay.
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly ClassService _classService;
        private readonly LessonService _lessonService;
        private readonly LessonPartService _lessonPartService;
        public LessonTodayController(
            LessonScheduleService lessonScheduleService,
            ClassService classService,
            LessonService lessonService,
            LessonPartService lessonPartService
            )
        {
            _lessonScheduleService = lessonScheduleService;
            _classService = classService;
            _lessonService = lessonService;
            _lessonPartService = lessonPartService;
        }
        public IActionResult Index()
        {
            return View();
        }


        public JsonResult GetList(DefaultModel model)
        {
            //student id
            var userid = User.Claims.GetClaimByType("UserID").Value;
            // lấy class theo student id
            var data = _classService.Collection.Find(o => o.IsActive == true && o.Students.Contains(userid)).ToList();

            if(data != null && data.Count > 0)
            {
                var mapping = new MappingEntity<ClassEntity, LessonTodayViewModel>();
                var map2 = new MappingEntity<LessonEntity, LessonScheduleTodayViewModel>() { };
                //id class
                var listID = data.Select(o => o.ID).ToList();
                // lịch học hôm nay
                var schedule = _lessonScheduleService.Collection.Find(o => listID.Contains(o.ClassID)).ToList();
                // có list lessonid
                var listIDSchedule = schedule.Select(x => x.LessonID).ToList();

                var resData = data.Select(o => mapping.AutoOrtherType(o, new LessonTodayViewModel()
                {
                    Lessons = schedule != null ? _lessonService.Collection.Find(y => listIDSchedule.Contains(y.ID)).ToList()
                        .Select(y => map2.AutoOrtherType(y, new LessonScheduleTodayViewModel() {
                            ClassID = schedule.SingleOrDefault(x => x.LessonID == y.ID)?.ClassID
                        })).ToList() : null
                })) ;

                var response = new Dictionary<string, object>()
                    {
                        {"Data", resData }
                    };
                return new JsonResult(response);
            }
            var response2 = new Dictionary<string, object>()
            {
                {"Data", data }
            };
            return new JsonResult(response2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id">LessonID</param>
        /// <param name="ClassID">ClassID</param>
        /// <returns></returns>
        public IActionResult LessonStudent(DefaultModel model, string id, string ClassID)
        {
            ViewBag.LessonID = id;
            ViewBag.ClassID = ClassID;
            return View();
        }

        [HttpPost]
        public JsonResult GetDetailsLesson(string ID)
        {
            try
            {
                var lesson = _lessonService.CreateQuery().Find(o => o.ID == ID).FirstOrDefault();

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

        public IActionResult Detail(DefaultModel model, string ClassID)
        {
            if (model == null) return null;

            if (ClassID == null)
                return RedirectToAction("Index", "Class");
            var currentClass = _classService.GetItemByID(ClassID);
            if (currentClass == null)
                return RedirectToAction("Index", "Class");
            var Data = _lessonService.GetItemByID(model.ID);
            if (Data == null)
                return RedirectToAction("Index", "Class");
            ViewBag.Class = currentClass;
            ViewBag.Data = Data;
            if (Data.TemplateType == LESSON_TEMPLATE.LECTURE)
                return View();
            else
                return View("Exam");
        }

    }
}
