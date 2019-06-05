using BaseModels;
using Microsoft.AspNetCore.Mvc;
using MVCBase.Globals;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace MVCBase
{
    [Permission()]
    public class AdminController : Controller
    {
        protected readonly WebMenu _menu;
        protected readonly CPLangEntity _currentLang;
        public AdminController()
        {
            _menu = new WebMenu();
            _currentLang = StartUp.CurrentLang;
        }
        
        protected List<MenuControlAttribute> GetMenuForUser(ClaimsPrincipal user , List<MenuControlAttribute> data)
        {
            try
            {
                var current = user;
                if (current == null || !current.Identity.IsAuthenticated)
                {
                    return null;
                }
                else
                {
                    List<MenuControlAttribute> cache = CacheExtends.GetDataFromCache<List<MenuControlAttribute>>(User.Claims.FirstOrDefault(o => o.Type == ClaimTypes.Email).Value + "menmu");
                    if (cache != null) return cache;
                    if (current.IsInRole("administrator"))
                    {
                        CacheExtends.SetObjectFromCache(User.Claims.FirstOrDefault(o => o.Type == ClaimTypes.Email).Value + "menmu", Cookies.ExpiresLogin, data);
                        return data;
                    }
                    else
                    {
                        List<MenuControlAttribute> menuControls = new List<MenuControlAttribute>();
                        var access = new CPAccessService();
                        int count = data.Count;
                        var cl = User.Claims.FirstOrDefault(o => o.Type == "RoleID");
                        if (cl == null) return null;
                        int RoleID = cl != null ? int.Parse(cl.Value) : 0;
                        for (int i = 0; i < count; i++)
                        {
                            var item = data[i];
                            if (access.GetPermission(RoleID, item.CModule, "index"))
                            {
                                menuControls.Add(item);
                            }
                        }
                        if (menuControls != null)
                        {
                            CacheExtends.SetObjectFromCache(User.Claims.FirstOrDefault(o => o.Type == ClaimTypes.Email).Value + "menmu", Cookies.ExpiresLogin, menuControls);
                            return menuControls;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        
        protected void SetMessageError(string content)
        {
            Dictionary<string, string> message = new Dictionary<string, string>
            {
                { "status", "error" },
                { "msg" , content }
            };
            this.TempData["error"] = message;
        }
        protected void SetMessageSuccess(string content)
        {
            Dictionary<string, string> message = new Dictionary<string, string>
            {
                { "status", "success" },{ "msg" , content }
            };
            this.TempData["success"] = message;
        }
        protected void SetMessageWarning(string content)
        {
            Dictionary<string, string> message = new Dictionary<string, string>
            {
                { "status", "warning" },{ "msg" , content }
            };
            this.TempData["warning"] = message;
        }
    }
}
