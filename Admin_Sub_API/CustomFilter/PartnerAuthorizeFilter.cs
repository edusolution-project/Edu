using Data.Access.Object.Entities.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SME.API.Controllers;
using SME.Bussiness.Lib.Dto;
using SME.Bussiness.Lib.Service;
using SME.Utils.Common;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace SME.API.CustomFilter
{
    public class PartnerAuthorizeFilter : AuthorizeAttribute, IAuthorizationFilter
    {

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            ((BaseController)(filterContext.HttpContext.RequestServices.GetService(typeof(IActionContextAccessor)) as IActionContextAccessor)).BeginRequestTime = DateTime.Now;

            if (filterContext.HttpContext.Request.Headers.All(x => x.Key.ToUpper() != GlobalConstants.HEADER_REQ_MESSAGE_ID.ToUpper()))
            {
                throw new UnauthorizedAccessException("Thiếu header ReqMessageId");
            }

            if (filterContext.HttpContext.Request.Headers.All(x => x.Key.ToUpper() != GlobalConstants.HEADER_USERNAME.ToUpper()))
            {
                throw new UnauthorizedAccessException("Thiếu header Username");
            }

            if (filterContext.HttpContext.Request.Headers.All(x => x.Key.ToUpper() != GlobalConstants.HEADER_PASSWORD.ToUpper()))
            {
                throw new UnauthorizedAccessException("Thiếu header Password");
            }

            string username = filterContext.HttpContext.Request.Headers.SingleOrDefault(x => x.Key.ToUpper() == GlobalConstants.HEADER_USERNAME.ToUpper()).Value.SingleOrDefault();
            string password = filterContext.HttpContext.Request.Headers.SingleOrDefault(x => x.Key.ToUpper() == GlobalConstants.HEADER_PASSWORD.ToUpper()).Value.SingleOrDefault();

            using (SMEEntities dbContext = new SMEEntities())
            {
                DoiTacService service = new DoiTacService(dbContext);
                HE_THONG_DOI_TAC dt = service.FindPartner(username, password);
                if (dt == null)
                {
                    throw new UnauthorizedAccessException("Tên đăng nhập hoặc password không đúng");
                }
            }
        }
    }
}