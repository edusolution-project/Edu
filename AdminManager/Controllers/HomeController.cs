using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AdminManager.Models;
using MaketingExtends;

namespace AdminManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmail _email;
        public HomeController(IEmail email)
        {
            _email = email;
        }
        public IActionResult Index()
        {
            _email.SendEmailAsync("longthaihoang94@gmail.com", "Email tesst", "how do you do !!");
            ViewBag.Data = HttpContext.Request.Headers["SigoutNow"].ToString();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
