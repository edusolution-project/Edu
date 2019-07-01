using BaseCustomerMVC.Globals;
using Core_v2.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Controllers.Student
{
    public class CheckStudentController : StudentController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
