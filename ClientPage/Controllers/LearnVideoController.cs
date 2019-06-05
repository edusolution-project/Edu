using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ClientPage.Controllers
{
    public class LearnVideoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}