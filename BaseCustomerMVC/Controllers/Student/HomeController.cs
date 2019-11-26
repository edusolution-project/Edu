using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using Core_v2.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Controllers.Student
{
    [BaseAccess.Attribule.AccessCtrl("Dashboard")]
    public class HomeController : StudentController
    {
        private StudentService _studentService;
        
        public HomeController(StudentService studentService)
        {
            _studentService = studentService;
        }

        public IActionResult Index()
        {
            var student = _studentService.GetItemByID(User.Claims.GetClaimByType("UserID").Value);
            ViewBag.Student = student;
            return View();
        }
    }
}
