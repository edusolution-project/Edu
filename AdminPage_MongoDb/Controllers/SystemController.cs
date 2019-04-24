using System.Collections.Generic;
using System.Linq;
using EntityBaseNet;
using GlobalNet.Utils;
using Microsoft.AspNetCore.Mvc;
using ServiceBaseNet;
using ServiceBaseNet.Services;
using Controller = ServiceBaseNet.ControllerBase;

namespace AdminPage.Controllers
{
    public class SystemController : Controller
    {
        private readonly SysAccessServices _sysAccess;
        private const string _keyCache = "CusMenuSystem";
        public SystemController()
        {
            _sysAccess = new SysAccessServices();
        }
        // GET: System
        public ActionResult Index()
        {
            var cache = CacheExtends.GetDataFromCache<List<CusMenuAttribute>>(_keyCache);
            var data = cache?? GetMenuFromList();
            if(cache == null) CacheExtends.SetObjectFromCache(_keyCache, 400, data);
            ViewBag.Data = data;
            return View();
        }
        private List<CusMenuAttribute> GetMenuFromList()
        {
            List<CusMenuAttribute> cusMenus = new List<CusMenuAttribute>();
            var user = AuthenticationExtends.CurrentUser;
            if (user != null && user.Role != null)
            {
                var data = MenuExtends.GetMenu().Where(o => o.Type == "Sys" && o.IsShow == true).ToList();
                if (HttpContext.User.IsInRole("administrator")) return data;
                if(data != null)
                {
                    int _count = data.Count;
                    for(int i = 0; i<_count; i++)
                    {
                        var item = data[i];
                        var role = _sysAccess.GetPermisstion(item.CModule, user.RoleID);
                        if (role != null)
                        {
                            if(role.Where(o=>o.Activity == true) != null)
                            {
                                cusMenus.Add(item);
                            }
                           
                        }
                    }
                }
            }
            return cusMenus;
        }
    }
}