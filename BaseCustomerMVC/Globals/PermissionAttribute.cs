﻿using BaseCustomerEntity.Database;
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
                // kieerm ta nguon tu cache
                string keys = $"{currentUser.FindFirst("UserID").Value}_{basis}";
                if (string.IsNullOrEmpty(area) || ctrlName == "home" || ctrlName == "error")
                {
                    base.OnActionExecuting(context);
                }
                else
                {
                    if (IsValidate(keys, area, ctrlName, actName))
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

        private bool IsValidate(string keys, string area, string ctrl, string act)
        {
            try
            {
                List<AuthorityEntity> data = CacheExtends.GetDataFromCache<List<AuthorityEntity>>(CacheExtends.DefaultPermission);
                string keypermission = CacheExtends.GetDataFromCache<string>(keys);
                List<string> permission = CacheExtends.GetDataFromCache<List<string>>(keypermission);

                if(permission == null || permission.Count == 0)
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
