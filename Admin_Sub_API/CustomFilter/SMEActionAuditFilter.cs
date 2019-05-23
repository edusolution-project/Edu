using Data.Access.Object.Entities.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Filters;
using SME.Bussiness.Lib.Service;
using SME.Utils.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using Viettel.Business.BusinessServices;

namespace SME.API.CustomFilter
{
    public class SMEActionAuditFilter : ActionFilterAttribute
    {
        ACTION_AUDIT actionAudit;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            actionAudit = new ACTION_AUDIT();
            actionAudit.CONTROLLER = filterContext.RouteData.Values["controller"].ToString();
            actionAudit.ACTION = filterContext.RouteData.Values["action"].ToString();
            actionAudit.USER_AGENT = Convert.ToString(filterContext.HttpContext.Request.Headers["User-Agent"].ToString());
            actionAudit.BEGIN_AUDIT_TIME = DateTime.Now;
            actionAudit.IP = GetClientIpAddress(filterContext.HttpContext);
            actionAudit.DESCRIPTION = Dns.GetHostName();
            var tg = filterContext.HttpContext.Request.Headers.Where(x => x.Key.ToUpper() == GlobalConstants.HEADER_USERNAME.ToUpper()).SingleOrDefault();
            string username = null;
            if (!string.IsNullOrEmpty(tg.Key))
            {
                username = tg.Value.FirstOrDefault();
            }
            if (username != null)
            {
                actionAudit.USER_NAME = username;

            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var tg = filterContext.HttpContext.Request.Headers.Where(x => x.Key.ToUpper() == GlobalConstants.HEADER_USERNAME.ToUpper()).SingleOrDefault();
            string username = null;
            if (!string.IsNullOrEmpty(tg.Key))
            {
                username = tg.Value.FirstOrDefault();
            }
            if (actionAudit == null) { return; }
            if (username != null && string.IsNullOrWhiteSpace(actionAudit.USER_NAME) && !string.IsNullOrWhiteSpace(username))

            {
                actionAudit.USER_NAME = username;
            }

            if (actionAudit.USER_NAME == null)

            {
                if (actionAudit.ACTION != "ExecuteChangePass")

                {
                    return;
                }
                else
                {
                    actionAudit.USER_NAME = "Error";
                }
            }
            actionAudit.END_AUDIT_TIME = DateTime.Now;
            SMEEntities dbContext = new SMEEntities();
            ActionAuditService service = new ActionAuditService(dbContext);
            service.Insert(actionAudit);
            dbContext.SaveChanges();

        }
        private string GetClientIpAddress(HttpContext request)
        {
            return request.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
        }
    }
}