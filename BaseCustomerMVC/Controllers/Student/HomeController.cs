using BaseCustomerMVC.Globals;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Controllers.Student
{
    public class HomeController : StudentController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
