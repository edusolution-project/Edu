﻿using BaseCustomerMVC.Globals;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class DiscussionController : TeacherController
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Detail()
        {
            return View();
        }
    }
}
