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
