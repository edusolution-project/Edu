﻿
using BasePublisherModels.Database;
using BasePublisherModels.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Linq;

namespace BasePublisherMVC.Globals
{
    public class PermissionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string ctrlName = context.RouteData.Values["Controller"].ToString();
            string actName = context.RouteData.Values["Action"].ToString();

            if (ctrlName.ToLower() == "cpapi" && actName.ToLower() == "startpage")
            {
                base.OnActionExecuting(context);
            }
            else
            {
                Controller controller = (Controller)context.Controller;
                var user = controller.User;
                if ((ctrlName.ToLower() == "cpaccounts" && (actName.ToLower() == "signin" || actName == "signout" || actName == "register"))
                    || (ctrlName.ToLower() == "cphome" && (user != null && user.Identity.IsAuthenticated))
                    || (user != null && user.Identity.IsAuthenticated && user.IsInRole("administrator")))
                {
                    base.OnActionExecuting(context);
                }
                else
                {
                    if (user == null || !user.Identity.IsAuthenticated)
                    {
                        string _returnUrl = System.Net.WebUtility.UrlEncode(ctrlName + "/" + actName);
                        context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "cpaccounts", action = "signin", returnurl = _returnUrl }));
                    }
                    else
                    {
                        base.OnActionExecuting(context);
                    }
                }
            }
        }
    }
}