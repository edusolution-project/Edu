
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
        private readonly PermissionService _permissionService;
        private readonly RoleService _roleService;
        private readonly AccountService _accountService;
        private readonly IConfiguration _configuration;
        public PermissionAttribute()
        {
            _configuration = StartUp.Configuration;
            _permissionService = new PermissionService(_configuration);
            _roleService = new RoleService(_configuration);
            _accountService = new AccountService(_configuration);
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
        private int CheckCtrlAndAct(Controller controller, string ctrlName, string actName)
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
                string _accessList = StartUp.Configuration.GetSection("AccessListUser:" + ctrlName.ToLower()).Value;
                if (!string.IsNullOrEmpty(_accessList))
                {
                    if(_accessList.Split(',').Contains(actName.ToLower()))
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                //lấy roleid
                var claimRole = user.Claims.SingleOrDefault(o => o.Type == "RoleID");
                if (claimRole != null)
                {
                    var access = _permissionService.GetPermission(claimRole.Value, ctrlName, actName);
                    if (access == null) return 0;
                    return access.IsActive?1:0;
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
            string area = context.RouteData.Values["Area"]?.ToString();
            if (CheckCtrlAndAct(null, ctrlName, actName) > 0)
            {
                base.OnActionExecuting(context);
            }
            else
            {
                Controller controller = (Controller)context.Controller;
                int _number = CheckCtrlAndAct(controller, ctrlName, actName);
                if (_number > 0)
                {
                    base.OnActionExecuting(context);
                }
                else
                {
                    if (_number == -1)
                    {
                        string _returnUrl = System.Net.WebUtility.UrlEncode(ctrlName + "/" + actName);
                        context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { area, controller = "home", action = "login", returnurl = _returnUrl }));
                    }
                    else
                    {
                        context.RouteData.Values.Add("Error", "Not accept deny !!!");
                        //var router = new RouteValueDictionary(new { controller = ctrlName, action = "index" });
                        //context.Result = new RedirectToRouteResult(router);
                        return;
                    }
                }
            }
        }
    }
}
