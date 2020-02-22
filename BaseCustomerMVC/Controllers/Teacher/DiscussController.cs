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
        private readonly ClassStudentService _classStudentService;
        public DiscussController(ClassService classService, StudentService studentService, TeacherService teacherService, ClassStudentService classStudentService)
        {
            _classService = classService;
            _studentService = studentService;
            _teacherService = teacherService;
            _classStudentService = classStudentService;
        }

        public IActionResult Index(string id, string searchText)
        {
            var teacher = _teacherService.Collection.Find(o => o.Email == User.FindFirst(System.Security.Claims.ClaimTypes.Email).Value)?.SingleOrDefault();
            var listClass = _classService.Collection.Find(o => o.TeacherID == User.Claims.GetClaimByType("UserID").Value).ToList();
            var listActive = listClass.Select(o => new ClassInfo()
            {
                ID = o.ID,
                IsAllow = o.EndDate >= DateTime.Now && o.StartDate <= DateTime.Now,
                Name = o.Name
            })?.OrderByDescending(o => o.IsAllow)?.ToList();
            ViewBag.Data = listActive;
            ViewBag.SearchText = searchText;
            ViewBag.ID = id;
            if (!string.IsNullOrEmpty(id))
            {
                var currentClass = _classService.GetItemByID(id);
                if (currentClass != null)
                {
                    ViewBag.Class = new ClassInfo()
                    {
                        ID = currentClass.ID,
                        Name = currentClass.Name,
                        IsAllow = currentClass.StartDate <= DateTime.Now && currentClass.EndDate >= DateTime.Now
                    };
                    var listAccount = _classStudentService.GetClassStudents(currentClass.ID)?.Select(o => o.ID)?.ToList();
                    var teacherID = currentClass.TeacherID;
                    ViewBag.Students = _studentService.Collection.Find(o => listAccount.Contains(o.StudentId))?.ToList()?
                        .Select(x => new MemberInfo() { ID = x.ID,Name = x.FullName, Email = x.Email }).ToList();
                    ViewBag.Teacher = new MemberInfo() { ID = teacher.ID, Name = teacher.FullName, Email = teacher.Email };
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
}
