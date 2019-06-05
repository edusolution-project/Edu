
using Microsoft.AspNetCore.Mvc;
using BasePublisherMVC.Globals;
using System;
using System.Collections.Generic;
using BasePublisherModels.Database;

namespace BasePublisherMVC.AdminControllers
{
    public class CPHomeController : AdminController
    {
        private readonly CPLangService _langService;
        public CPHomeController(CPLangService langService)
        {
            _langService = langService;
        }
        public ActionResult Index()
        {
            ViewBag.Data = GetMenuForUser(User, _menu.GetAdminMenu);
            ViewBag.Message = TempData["error"];
            return View();
        }

        public ActionResult ok(string data)
        {
            return PartialView("_NavBar",data);
        }

        [HttpGet]
        public ActionResult<List<MenuControlAttribute>> GetMenu()
        {
            try
            {
                var data = GetMenuForUser(User, _menu.GetAdminMenu);
                return data;
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
                return null;
            }
        }
        [HttpGet]
        [Route("/setlang")]
        public bool SetLang(string langCode)
        {
            var lang = _langService.GetItemByCode(langCode);
            if(lang != null && lang != _currentLang)
            {
                HttpContext.SetCurrentLang(langCode);
            }
            //CacheExtends.ClearCacheAll();
            return true;
        }
    }
}
