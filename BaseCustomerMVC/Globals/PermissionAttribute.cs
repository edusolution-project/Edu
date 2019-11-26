using BaseCustomerEntity.Database;
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
        private readonly IConfiguration _configuration;
        public PermissionAttribute()
        {
            _configuration = StartUp.Configuration;
        }
        private readonly string _roles;
        public PermissionAttribute(string Roles)
        {
            _roles = Roles;
            _configuration = StartUp.Configuration;
        }
        /// <summary>
        /// -1 chưa login
        /// 0 đã login - ko có quyền
        /// 1 đã login và có quyền
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="ctrlName"></param>
        /// <param name="actName"></param>
        /// <returns></returns>
        private int CheckCtrlAndAct(Controller controller,string type, string ctrlName, string actName)
        {

            if (controller == null)
            {
                string _accessList = StartUp.Configuration.GetSection("AccessList:" + ctrlName.ToLower()).Value;
                if (string.IsNullOrEmpty(_accessList))
                {
                    return -1;
                }
                else
                {
                    if (_accessList.Split(',').Contains(actName.ToLower()))
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            else
            {
                ClaimsPrincipal user = controller.User;
                if (user == null || !user.Identity.IsAuthenticated)
                {
                    return -1;
                }
                
                if (user.IsInRole("superadmin"))
                {
                    return 1;
                }
                string _accessList = _configuration.GetSection("AccessListUser:" + ctrlName.ToLower()).Value;
                if (!string.IsNullOrEmpty(_accessList))
                {
                    if (_accessList.Split(',').Contains(actName.ToLower()))
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                //lấy roleid
                var claimRole = user.FindAll(o => o.Type == "Permission");
                if (claimRole != null && claimRole.Count() > 0)
                {
                    var access = claimRole.Where(o=>o.Value==$"{type}{ctrlName}{actName}")?.FirstOrDefault();
                    if (access == null) return 0;
                    return  1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string ctrlName = context.RouteData.Values["Controller"].ToString();
            string actName = context.RouteData.Values["Action"].ToString();
            string _area = context.RouteData.Values["Area"]?.ToString();
            if (CheckCtrlAndAct(null,_area, ctrlName, actName) > 0)
            {
                base.OnActionExecuting(context);
            }
            else
            {
                Controller ctrl = (Controller)context.Controller;
                int _number = CheckCtrlAndAct(ctrl,_area, ctrlName, actName);
                if (_number > 0)
                {
                    base.OnActionExecuting(context);
                }
                else
                {
                    if (_number == -1)
                    {
                        string _returnUrl = System.Net.WebUtility.UrlEncode(ctrlName + "/" + actName);
                        context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "home", action = "login", returnurl = _returnUrl }));
                    }
                    else
                    {
                        var type = ctrl.User.Claims.GetClaimByType("Type").Value;
                        //context.Result = new LocalRedirectResult("/" + type + "/home/deny");
                        context.Result = new JsonResult("Access deny");
                        //new RedirectToRouteResult(new RouteValueDictionary(new { controller = "home", action = "login" }));
                    }
                }
            }
        }
    }
}
