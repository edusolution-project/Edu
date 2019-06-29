using BaseCustomerMVC.Globals;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class CalendarController : TeacherController
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Detail()
        {
            return View();
        }
        public IActionResult CheckTeacher()
        {
            return View();
        }
        public IActionResult CheckTeacherStudent()
        {
            return View();
        }
        public IActionResult CheckList()
        {
            return View();
        }
        public IActionResult Lesson()
        {
            return View();
        }
    }
}
