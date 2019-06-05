using Data.Access.Object.Entities.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;
using SME.Bussiness.Lib.Service;
using SME.Utils.Common;
using SME.Utils.Common.SMEException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace SME.API.CustomFilter
{
    public class SMEExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is APIException)
            {
                Logger.Error(context.Exception, "Co loi xay ra!");
                APIException apiException = (APIException)context.Exception;
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Result = new JsonResult(apiException.ResponseObject);
            }
            else if (context.Exception is InvalidUsernameOrPasswordException)
            {
                context.Result = new ObjectResult(GlobalConstants.COMMON_INVALID_USER) { StatusCode = (int)HttpStatusCode.NotAcceptable };
            }
            else if (context.Exception is InvalidSSOUserException)
            {
                try
                {
                    using (SMEEntities ct = new SMEEntities())
                    {
                        LogDoiTacService logService = new LogDoiTacService(ct);

                        LOG_DOI_TAC log = new LOG_DOI_TAC
                        {
                            MA_DOI_TAC = "CSDL",
                            LOG_MESSAGE = context.Exception.ToString(),
                            NGAY_CAP_NHAT = DateTime.Now
                        };
                        logService.Insert(log);
                        logService.Save();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Co loi khi ghi log!");
                }
                context.Result = new ObjectResult(GlobalConstants.COMMON_SSO_INVALID_USER) { StatusCode = (int)GlobalEnum.InvalidSSOUserException }; 
            }
            else if (context.Exception is InvalidSSOAuthorizeException)
            {
                try
                {
                    using (SMEEntities ct = new SMEEntities())
                    {
                        LogDoiTacService logService = new LogDoiTacService(ct);

                        LOG_DOI_TAC log = new LOG_DOI_TAC
                        {
                            MA_DOI_TAC = "CSDL",
                            LOG_MESSAGE = context.Exception.ToString(),
                            NGAY_CAP_NHAT = DateTime.Now
                        };

                        logService.Insert(log);
                        logService.Save();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Co loi khi ghi log!");
                }                
                context.Result = new ObjectResult(GlobalConstants.COMMON_SSO_INVALID_AUTHORIZE) { StatusCode = (int)GlobalEnum.InvalidSSOAuthorizeException }; 
            }
            else if (context.Exception is InvalidAndEnableCaptchaException)
            {
                //throw new HttpResponseException(new HttpResponseMessage((HttpStatusCode)GlobalEnum.InvalidAndEnableCaptchaException)
                //{
                //    ReasonPhrase = "Login Exception",
                //});                
                context.Result = new ObjectResult("Login Exception") { StatusCode = (int)GlobalEnum.InvalidAndEnableCaptchaException };
            }
            else if (context.Exception is InvalidCaptchaException)
            {
                //context.Response = context.Request.CreateResponse((HttpStatusCode)GlobalEnum.InvalidCaptchaException, GlobalConstants.COMMON_INVALID_CAPTCHA);
                context.HttpContext.Response.StatusCode = (int)(HttpStatusCode)GlobalEnum.InvalidCaptchaException;
                context.Result = new JsonResult(GlobalConstants.COMMON_INVALID_CAPTCHA);
            }
            else if (context.Exception is ImportErrorException)
            {
                //context.Response = context.Request.CreateResponse((HttpStatusCode)GlobalEnum.ImportErrorException, ((ImportErrorException)context.Exception).FileError);
                context.HttpContext.Response.StatusCode = (int)(HttpStatusCode)GlobalEnum.ImportErrorException;
                context.Result = new JsonResult(((ImportErrorException)context.Exception).FileError);
            }
            else if (context.Exception is InvalidDataInDatabaseException
                || context.Exception is InvalidDataInRequestException
                || context.Exception is NotPermissionException
                || context.Exception is BusinessException)
            {
                //context.Response = context.Request.CreateResponse((HttpStatusCode)GlobalEnum.ErrorException, context.Exception.Message);
                context.HttpContext.Response.StatusCode = (int)(HttpStatusCode)GlobalEnum.ErrorException;
                context.Result = new JsonResult(context.Exception.Message);
            }
            else
            {
                try
                {
                    using (SMEEntities ct = new SMEEntities())
                    {
                        LogDoiTacService logService = new LogDoiTacService(ct);

                        LOG_DOI_TAC log = new LOG_DOI_TAC
                        {
                            MA_DOI_TAC = "CSDL",
                            LOG_MESSAGE = context.Exception.ToString(),
                            NGAY_CAP_NHAT = DateTime.Now
                        };
                        ct.Entry(log).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        logService.Insert(log);
                        logService.Save();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Co loi khi ghi log!");
                }

                Logger.Error(context.Exception, "Co loi xay ra!");
                //context.Response = context.Request.CreateResponse(HttpStatusCode.InternalServerError, "Co loi xay ra!");
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Result = new JsonResult("Co loi xay ra!");
            }
        }
    }
}