using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using Core_v2.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Controllers.Student
{
    public class DiscussController : StudentController
    {
        private readonly ClassService _classService;
        public DiscussController(ClassService classService)
        {
            _classService = classService;
        }
        public IActionResult Index()
        {
            ViewBag.Data = _classService.Collection.Find(o => o.Students.Contains(User.Claims.GetClaimByType("UserID").Value)).ToList();
            return View();
        }

        public IActionResult Exam()
        {
            return View();
        }
    }
}
