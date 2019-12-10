using BaseCustomerMVC.Globals;
using Microsoft.AspNetCore.Mvc;
using BaseCustomerEntity.Database;
using Core_v2.Globals;
using MongoDB.Driver;
using System.Linq;

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
            var teacher = _teacherService.Collection.Find(o => o.Email == User.FindFirst(System.Security.Claims.ClaimTypes.Email).Value)?.SingleOrDefault();
            ViewBag.Data = _classService.Collection.Find(o => o.TeacherID == teacher.ID).ToList();
            ViewBag.SearchText = searchText;
            ViewBag.ID = id;
            if (!string.IsNullOrEmpty(id))
            {
                var currentClass = _classService.GetItemByID(id);
                if (currentClass != null)
                {
                    ViewBag.Class = currentClass;
                    var listAccount = currentClass.Students;
                    var teacherID = currentClass.TeacherID;
                    ViewBag.Students = _studentService.Collection.Find(o => listAccount.Contains(o.ID))?.ToList()?
                        .Select(x => new MemberInfo() { Name = x.FullName, Email = x.Email }).ToList();
                    ViewBag.Teacher = new MemberInfo() { Name = teacher.FullName, Email = teacher.Email };
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
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
