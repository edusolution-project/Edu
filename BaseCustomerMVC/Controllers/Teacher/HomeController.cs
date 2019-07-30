using BaseCustomerMVC.Globals;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace BaseCustomerMVC.Controllers.Teacher
{
    [IndefindCtrlAttribulte("Trang chủ", "Home", "teacher")]
    public class HomeController : TeacherController
    {
        public IActionResult Index()
        {
            ViewBag.RoleCode = User.Claims.GetClaimByType(ClaimTypes.Role).Value;
            return View();
        }
    }
}
