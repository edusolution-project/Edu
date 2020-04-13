using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseEasyRealTime.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Linq;

namespace BaseCustomerMVC.Controllers.Student
{
    [BaseAccess.Attribule.AccessCtrl("Trao đổi", "student")]
    public class DiscussController : StudentController
    {
        private readonly TeacherService _teacherService;
        private readonly StudentService _studentService;
        private readonly ClassService _classService;

        public DiscussController(ClassService classService, StudentService studentService, TeacherService teacherService, ClassStudentService classStudentService)
        {
            _classService = classService;
            _studentService = studentService;
            _teacherService = teacherService;
        }

        public IActionResult Index(string id,string searchText)
        {
            return View();
        }

        public IActionResult Detail(string id)
        {
            ViewBag.ClassID = id;
            return View();
        }
    }

    public class MemberInfo
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Skype { get; set; }
    }
    public class ClassInfo
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public bool IsAllow { get; set; }
    }
}
