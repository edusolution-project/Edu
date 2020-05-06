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
            if (currentUser != null)
            {
                // kieerm ta nguon tu cache

                
            }


            base.OnActionExecuting(context);
        }
    }
}
