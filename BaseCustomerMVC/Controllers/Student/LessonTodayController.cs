using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Core_v2.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Controllers.Student
{
    public class LessonTodayController : StudentController
    {
        // bài học hôm nay.

        private readonly ClassService _classService;
        private readonly LessonService _lessonService;
        private readonly LessonPartService _lessonPartService;
        public LessonTodayController(
            ClassService classService,
            LessonService lessonService,
            LessonPartService lessonPartService
            )
        {
            _classService = classService;
            _lessonService = lessonService;
            _lessonPartService = lessonPartService;
        }
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id">LessonID</param>
        /// <param name="ClassID">ClassID</param>
        /// <returns></returns>
        public IActionResult LessonStudent(DefaultModel model, string ClassID)
        {
            ViewBag.LessonID = model.ID;
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
