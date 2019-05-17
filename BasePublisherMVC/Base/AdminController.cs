using Microsoft.AspNetCore.Mvc;
using BasePublisherMVC.Globals;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using BasePublisherModels.Database;
using BasePublisherModels.Factory;

namespace BasePublisherMVC
{
    public class AdminController : Controller
    {
        protected readonly WebMenu _menu;
        protected readonly CPLangEntity _currentLang;
        protected readonly CPUserEntity _currentUser;
        public AdminController()
        {
            _menu = new WebMenu();
            _currentLang = StartUp.CurrentLang;
            _currentUser = StartUp.CurrentUser;
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
                        var access = Instance.CreateInstanceCPAccess("CPAccess");
                        int count = data.Count;
                        var cl = User.Claims.FirstOrDefault(o => o.Type == "RoleID");
                        if (cl == null) return null;
                        string RoleID = cl != null ? cl.Value : string.Empty;
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
            this.TempData["Message"] = message;
        }
        protected void SetMessageSuccess(string content)
        {
            Dictionary<string, string> message = new Dictionary<string, string>
            {
                { "status", "success" },{ "msg" , content }
            };
            this.TempData["Message"] = message;
        }
        protected void SetMessageWarning(string content)
        {
            Dictionary<string, string> message = new Dictionary<string, string>
            {
                { "status", "warning" },{ "msg" , content }
            };
            this.TempData["Message"] = message;
        }
    }
}
