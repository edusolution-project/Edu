using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AdminPage.Models;
using Controller = ServiceBaseNet.ControllerBase;
using ServiceBaseNet;
using GlobalNet.Utils;
using System;
using Microsoft.AspNetCore.Http;

namespace AdminPage.Controllers
{
    public class CHomeController : Controller
    {
        private readonly IHttpContextAccessor _httpContext;
        public CHomeController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor;
        }
        public IActionResult Index()
        {
            ViewBag.Data = MenuExtends.GetMenu();
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
        [HttpGet]
        [Route("/setlang")]
        public bool SetLanguage(string langCode)
        {
            try
            {
                langCode = string.IsNullOrEmpty(langCode) ? "vn" : langCode;
                Cookies.SetCurrentLang(_httpContext.HttpContext, langCode);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
