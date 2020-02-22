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
        private readonly ClassStudentService _classStudentService;
        public DiscussController(ClassService classService, StudentService studentService, TeacherService teacherService, ClassStudentService classStudentService)
        {
            _classService = classService;
            _studentService = studentService;
            _teacherService = teacherService;
            _classStudentService = classStudentService;
        }

        public IActionResult Index(string id,string searchText)
        {
            var listClassID = _classStudentService.GetStudentClasses(User.Claims.GetClaimByType("UserID")?.Value);
            var listClass = listClassID != null ? _classService.CreateQuery().Find(o=> listClassID.Contains(o.ID))?.ToList() : null;
            var listActive = listClass?.Select(o => new ClassInfo()
            {
                ID = o.ID,
                IsAllow = o.EndDate >= DateTime.Now && o.StartDate <= DateTime.Now,
                Name = o.Name
            })?.OrderByDescending(o=>o.IsAllow)?.ToList();
            ViewBag.Data = listActive;
            ViewBag.SearchText = searchText;
            ViewBag.ID = id;
            if (!string.IsNullOrEmpty(id))
            {
                var currentClass = _classService.GetItemByID(id);
                if(currentClass != null)
                {
                    ViewBag.Class = new ClassInfo() {
                        ID = currentClass.ID,
                        Name = currentClass.Name,
                        IsAllow = currentClass.StartDate <= DateTime.Now && currentClass.EndDate >= DateTime.Now
                    };
                    var listAccount = currentClass.Students;
                    var teacherID = currentClass.TeacherID;
                    ViewBag.Students = _studentService.Collection.Find(o => listAccount.Contains(o.ID))?.ToList()?
                        .Select(x => new MemberInfo (){ ID= x.ID, Name = x.FullName, Email = x.Email , Skype = x.Skype}).ToList();
                    var teacher = _teacherService.GetItemByID(teacherID);
                    ViewBag.Teacher = new MemberInfo() { ID = teacher.ID, Name = teacher.FullName, Email = teacher.Email, Skype = teacher.Skype };
                }
            }
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
