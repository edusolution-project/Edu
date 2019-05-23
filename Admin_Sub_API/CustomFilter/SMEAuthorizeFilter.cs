using Data.Access.Object.Entities.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SME.Bussiness.Lib.Service;
using SME.Utils.Common;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace SME.API.CustomFilter
{
    public class SMEAuthorizeFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                var headerToken = filterContext.HttpContext.Request.Headers.SingleOrDefault
                                      (x => x.Key.ToUpper() == GlobalConstants.HEADER_AUTHORIZATION.ToUpper());
                if (!string.IsNullOrEmpty(filterContext.HttpContext.Request.Headers["Authorization"].ToString()))
                {
                    string username = filterContext.HttpContext.Request.Headers.SingleOrDefault(x => x.Key.ToUpper() == GlobalConstants.HEADER_USERNAME.ToUpper()).Value.SingleOrDefault();
                    string userAgent = Convert.ToString(filterContext.HttpContext.Request.Headers["User-Agent"].ToString());
                    string authorization = Convert.ToString(filterContext.HttpContext.Request.Headers["Authorization"]);
                    SMEEntities dbContext = new SMEEntities();
                    AccessTokenService service = new AccessTokenService(dbContext);
                    ACCESS_TOKEN at;
                    if (!service.IsValidToken(authorization, username, userAgent, out at))
                    {
                        //filterContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                        filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        filterContext.Result = new UnauthorizedResult();
                        //filterContext.Result = ;
                    }
                    if (at == null)
                    {
                        throw new Exception("Hàm xác thực token (AccessTokenService.IsValidToken) chưa trả về bản ghi ACCESS_TOKEN");
                    }
                    NGUOI_DUNG user = new NguoiDungService(dbContext).Find(at.NGUOI_DUNG_ID);
                    if (user == null)
                    {
                        throw new Exception(string.Format(
                            "Thông tin NGUOI_DUNG_ID ({0}) lưu trong bảng ACCESS_TOKEN không tồn tại trong bảng NGUOI_DUNG",
                            at.NGUOI_DUNG_ID));
                    }
                    //Kiem tra vai tro nguoi dung
                    string action = filterContext.RouteData.Values["action"].ToString();
                    string controller = filterContext.RouteData.Values["controller"].ToString();
                    if (!RoleManagementService.CanAccess(user.VAI_TRO, controller, action))
                    {
                        //var result = new HttpResponseMessage(HttpStatusCode.MethodNotAllowed);
                        filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                        filterContext.Result = new StatusCodeResult((int)HttpStatusCode.MethodNotAllowed);
                        //return;
                    }
                }
                else
                {
                    //filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;//new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    filterContext.Result = new StatusCodeResult((int)HttpStatusCode.Unauthorized);//new UnauthorizedResult();
                                                                                                  //return;
                }
            }
            catch (Exception ex)
            {
                string mes = ex.Message;
                filterContext.HttpContext.Response.StatusCode = (int)(HttpStatusCode.Unauthorized);
                filterContext.Result = new UnauthorizedResult();
            }
            base.OnActionExecuting(filterContext);
        }
    }


    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    //public class SMEAuthorizeFilter : AuthorizeAttribute, IAuthorizationFilter
    //{

    //    public void OnAuthorization(AuthorizationFilterContext filterContext)
    //    {
    //        try
    //        {
    //            var headerToken = filterContext.HttpContext.Request.Headers.SingleOrDefault
    //                                  (x => x.Key.ToUpper() == GlobalConstants.HEADER_AUTHORIZATION.ToUpper());
    //            if (!string.IsNullOrEmpty(filterContext.HttpContext.Request.Headers["Authorization"].ToString()))
    //            {
    //                string username = filterContext.HttpContext.Request.Headers.SingleOrDefault(x => x.Key.ToUpper() == GlobalConstants.HEADER_USERNAME.ToUpper()).Value.SingleOrDefault();
    //                string userAgent = Convert.ToString(filterContext.HttpContext.Request.Headers["User-Agent"].ToString());
    //                string authorization = Convert.ToString(filterContext.HttpContext.Request.Headers["Authorization"]);
    //                SMEEntities dbContext = new SMEEntities();
    //                AccessTokenService service = new AccessTokenService(dbContext);
    //                ACCESS_TOKEN at;
    //                if (!service.IsValidToken(authorization, username, userAgent, out at))
    //                {
    //                    //filterContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
    //                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
    //                    filterContext.Result = new UnauthorizedResult();
    //                    //filterContext.Result = ;
    //                }
    //                if (at == null)
    //                {
    //                    throw new Exception("Hàm xác thực token (AccessTokenService.IsValidToken) chưa trả về bản ghi ACCESS_TOKEN");
    //                }
    //                NGUOI_DUNG user = new NguoiDungService(dbContext).Find(at.NGUOI_DUNG_ID);
    //                if (user == null)
    //                {
    //                    throw new Exception(string.Format(
    //                        "Thông tin NGUOI_DUNG_ID ({0}) lưu trong bảng ACCESS_TOKEN không tồn tại trong bảng NGUOI_DUNG",
    //                        at.NGUOI_DUNG_ID));
    //                }
    //                //Kiem tra vai tro nguoi dung
    //                string action = filterContext.RouteData.Values["action"].ToString();
    //                string controller = filterContext.RouteData.Values["controller"].ToString();
    //                if (!RoleManagementService.CanAccess(user.VAI_TRO, controller, action))
    //                {
    //                    //var result = new HttpResponseMessage(HttpStatusCode.MethodNotAllowed);
    //                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
    //                    filterContext.Result = new StatusCodeResult((int)HttpStatusCode.MethodNotAllowed);
    //                    //return;
    //                }
    //            }
    //            else
    //            {
    //                //filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;//new HttpResponseMessage(HttpStatusCode.Unauthorized);
    //                filterContext.Result = new StatusCodeResult((int)HttpStatusCode.Unauthorized);//new UnauthorizedResult();
    //                //return;
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            string mes = ex.Message;
    //            filterContext.HttpContext.Response.StatusCode = (int)(HttpStatusCode.Unauthorized);
    //            filterContext.Result = new UnauthorizedResult();
    //        }
    //        //base.IsDefaultAttribute()
    //    }
    //}
}