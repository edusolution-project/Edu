using BaseCustomerMVC.Globals;
using Microsoft.AspNetCore.Mvc;
using BaseCustomerEntity.Database;

namespace BaseCustomerMVC.Controllers.Teacher
{
    [BaseAccess.Attribule.AccessCtrl("Thảo luận")]
    public class DiscussController : TeacherController
    {
        private readonly ClassService _classService;

        public DiscussController(ClassService classService)
        {
            _classService = classService;
            
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
