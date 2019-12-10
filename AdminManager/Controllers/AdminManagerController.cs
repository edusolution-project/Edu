using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AdminManager.Controllers
{
    public class AdminManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}