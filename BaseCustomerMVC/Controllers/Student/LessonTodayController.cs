using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
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
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult LessonStudent()
        {
            return View();
        }
    }
}
