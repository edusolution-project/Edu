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
        private readonly string _roles;
        public PermissionAttribute(string Roles)
        {
            _roles = Roles;
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
                var claimRole = user.Claims.GetClaimByType("RoleID");
                if (user.IsInRole("superadmin"))
                {
                    return 1;
                }
                if (string.IsNullOrEmpty(_roles))
                {
                    string _accessList = StartUp.Configuration.GetSection("AccessListUser:" + ctrlName.ToLower()).Value;
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

                    if (claimRole != null)
                    {
                        var access = _permissionService.GetPermission(claimRole.Value, ctrlName, actName);
                        if (access == null) return 0;
                        return access.IsActive ? 1 : 0;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    var role = _roleService.GetItemByID(claimRole.Value);
                    if (role == null) return 0;
                    if (_roles.Contains(','))
                    {
                        if (_roles.Split(',').Contains(role.Code))
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        if (_roles.Equals(role.Code))
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }
            }
        }
        
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string ctrlName = context.RouteData.Values["Controller"].ToString();
            string actName = context.RouteData.Values["Action"].ToString();
            string _area = context.RouteData.Values["Area"]?.ToString();
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
                        context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "home", action = "login", returnurl = _returnUrl }));
                    }
                    else
                    {
                        context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "home", action = "deny", Areas = _area }));
                        return;
                    }
                }
            }
        }
    }
}
