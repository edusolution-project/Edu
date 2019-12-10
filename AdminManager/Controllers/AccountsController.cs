using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminManager.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace AdminManager.Controllers
{
    public class AccountsController : Controller
    {
        public AccountsController()
        {

        }

        [Route("/thong-tin-tai-khoan")]
        public IActionResult Index()
        {
            return View();
        }
        [Route("/dang-nhap")]
        public IActionResult Login()
        {
            return View();
        }
        [Route("/dang-ky")]
        public IActionResult Register(CustomerEntity item)
        {
            ViewBag.Data = item;
            return View();
        }
    }
}