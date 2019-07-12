using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Core_v2.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Controllers.Student
{
    public class LessonTodayController : StudentController
    {
        // bài học hôm nay.

        private readonly ClassService _service;
        private readonly CourseService _courseService;
        private readonly LessonService _lessonService;
        private readonly LessonPartService _lessonPartService;
        public LessonTodayController()
        {

        }
        public IActionResult Index(DefaultModel model, string id, string ClassID)
        {
            ViewBag.LessonID = id;
            ViewBag.ClassID = ClassID;
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id">LessonID</param>
        /// <param name="ClassID">ClassID</param>
        /// <returns></returns>
        public IActionResult LessonStudent(DefaultModel model,string id,string ClassID)
        {
            ViewBag.LessonID = id;
            ViewBag.ClassID = ClassID;
            return View();
        }
    }
}
