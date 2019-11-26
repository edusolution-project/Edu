using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace BaseCustomerMVC.Controllers.Student
{
    [BaseAccess.Attribule.AccessCtrl("Trao đổi")]
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

        public IActionResult Detail(string id)
        {
            ViewBag.ClassID = id;
            return View();
        }
    }
}
