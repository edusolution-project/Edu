using BaseCustomerMVC.Globals;
using Microsoft.AspNetCore.Mvc;
using BaseCustomerEntity.Database;
using Core_v2.Globals;
using MongoDB.Driver;
using System.Linq;
using System;
using BaseCustomerMVC.Controllers.Student;

namespace BaseCustomerMVC.Controllers.Teacher
{
    [BaseAccess.Attribule.AccessCtrl("Thảo luận", "teacher")]
    public class DiscussController : TeacherController
    {
        private readonly TeacherService _teacherService;
        private readonly StudentService _studentService;
        private readonly ClassService _classService;

        public DiscussController(ClassService classService, StudentService studentService, TeacherService teacherService)
        {
            _classService = classService;
            _studentService = studentService;
            _teacherService = teacherService;
        }

        public IActionResult Index(string id, string searchText)
        {
            return View();
        }

        public IActionResult Detail(string id)
        {
            ViewBag.ClassID = id;
            return View();
        }
    }
}
