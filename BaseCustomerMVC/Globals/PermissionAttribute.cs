using BaseCustomerEntity.Database;
using Core_v2.Globals;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace BaseCustomerMVC.Globals
{
    public class PermissionAttribute : ActionFilterAttribute
    {
        public PermissionAttribute()
        {

        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string ctrlName = context.RouteData.Values["Controller"].ToString().ToLower();
            string actName = context.RouteData.Values["Action"].ToString().ToLower();
            string area = context.RouteData.Values["Area"]?.ToString().ToLower();
            string basis = context.RouteData.Values["basis"]?.ToString().ToLower();
            var currentUser = context.HttpContext.User;
            if (currentUser != null && currentUser.Identity.IsAuthenticated)
            {
                string userId = currentUser.FindFirst("UserID").Value;
                string userType = currentUser.FindFirst("Type")?.Value;
                string type = currentUser.FindFirst(ClaimTypes.Role)?.Value;
                try
                {
                    var ctrl = (Controller)context.Controller;
                    ctrl.TempData.Add("center_router", basis);
                    if (ctrl != null)
                    {
                        if (!ctrl.TempData.ContainsKey(userId))
                        {
                            ctrl.TempData.Add(userId, basis);
                        }
                        else
                        {
                            ctrl.TempData[userId] = basis;
                        }
                    }
                }
                catch { }
                // kieerm ta nguon tu cache
                string keys = $"{userId}_{basis}";
                if (string.IsNullOrEmpty(area) || ctrlName == "home" || ctrlName == "error" || type == "superadmin" || ctrlName == "news")
                {
                    base.OnActionExecuting(context);
                }
                else
                {
                    if (IsValidate(keys, area, ctrlName, actName, userType))
                    {
                        base.OnActionExecuting(context);
                    }
                    else
                    {
                        if (actName == "index" || actName == "details")
                        {
                            var result = new ViewResult
                            {
                                StatusCode = 500
                            };
                            context.Result = result;
                        }
                        else
                        {
                            context.Result = new JsonResult(new { code = 405, message = "not accept" });
                        }
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(area))
                {
                    base.OnActionExecuting(context);
                }
                else
                {
                    if (actName == "index" || actName == "details")
                    {
                        context.Result = new ViewResult();
                    }
                    else
                    {
                        context.Result = new JsonResult(new { code = 405, message = "not accept" });
                    }
                }
            }
        }

        
        private bool IsValidate(string keys, string area, string ctrl, string act, string userType)
        {
            try
            {
                List<AuthorityEntity> data = CacheExtends.GetDataFromCache<List<AuthorityEntity>>(CacheExtends.DefaultPermission);
                string keypermission = CacheExtends.GetDataFromCache<string>(keys);
                List<string> permission = CacheExtends.GetDataFromCache<List<string>>(keypermission);

                if (userType == "student")
                {
                    return area == "student";
                }

                if (permission == null || permission.Count == 0)
                {
                    return false;
                }
                else
                {
                    var item = data?.Where(o => o.CtrlName == ctrl && o.ActName == act && o.Area == area && permission.Contains(o.ID))?.ToList();
                    return item != null && item.Count > 0;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
