using System;
using Newtonsoft.Json;
using System.IO;
using System.Web;
using System.Text;
using System.Net.Http;
using System.Linq;
using SME.Utils.Common;
using SME.Bussiness.Lib.Validator;
using Data.Access.Object.Entities.Model;
using SME.Bussiness.Lib.Dto.System;
using SME.Bussiness.Lib.Service;
using System.Net;
using SME.API.Controllers;
using System.Net.Http.Formatting;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace SME.API.CustomFilter
{
    public class LoggingDatabaseFilterAttribute : ActionFilterAttribute
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            using (var dbContext = new SMEEntities())
            {
                string username = context.HttpContext.Request.Headers.SingleOrDefault(x => x.Key.ToUpper() == GlobalConstants.HEADER_USERNAME.ToUpper()).Value.SingleOrDefault();
                string requestId = context.HttpContext.Request.Headers.SingleOrDefault(x => x.Key.ToUpper() == GlobalConstants.HEADER_REQ_MESSAGE_ID.ToUpper()).Value.SingleOrDefault();
                ValidateHelper.NotNull("ReqMessageId", requestId);
                string action = context.RouteData.Values["action"].ToString();
                string controller = context.RouteData.Values["controller"].ToString();
                string url = VtFunction.ToFullPath(controller, action);
                GiaoDichService giaoDichService = new GiaoDichService(dbContext);
                if (username != null)
                {
                    var giaoDich = giaoDichService.GetRequestId(requestId, username.ToUpper(), url);

                    if (giaoDich != null)
                    {
                        if (giaoDich.TRANGTHAI == (long)HttpStatusCode.OK)
                        {
                            var json = JsonConvert.DeserializeObject(CompressionUtils.DecompressToString(giaoDich.RESPONSEMESSAGE));
                            context.HttpContext.Response.StatusCode = (int)(HttpStatusCode)GlobalConstants.StatusCode.DuplicateSuccessRequest;
                            context.Result = new JsonResult(json);
                            //context.Response = context.Request.CreateResponse((HttpStatusCode)GlobalConstants.StatusCode.DuplicateSuccessRequest,
                            //    json,
                            //    JsonMediaTypeFormatter.DefaultMediaType);
                        }
                        else
                        {
                            context.HttpContext.Response.StatusCode = (int)(HttpStatusCode)GlobalConstants.StatusCode.DuplicateErrorRequest;
                            context.Result = new JsonResult(giaoDich.THONGBAOLOI);
                            //context.Response = context.Request.CreateResponse((HttpStatusCode)GlobalConstants.StatusCode.DuplicateErrorRequest, giaoDich.THONGBAOLOI);
                        }
                    }
                }
            }

        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            try
            {
                if (context.Exception == null)
                {
                    string jsonContent = context.HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
                  
                    //Ghi log giao dich
                    using (var dbContext = new SMEEntities())
                    {

                        string username = context.HttpContext.Request.Headers.SingleOrDefault(x => x.Key.ToUpper() == GlobalConstants.HEADER_USERNAME.ToUpper()).Value.SingleOrDefault();
                        string requestId = context.HttpContext.Request.Headers.SingleOrDefault(x => x.Key.ToUpper() == GlobalConstants.HEADER_REQ_MESSAGE_ID.ToUpper()).Value.SingleOrDefault();
                        string action = context.RouteData.Values["action"].ToString();
                        string controllerS = context.RouteData.Values["controller"].ToString();
                        string url = VtFunction.ToFullPath(controllerS, action);
                        var controller = (BaseController)(context.HttpContext.RequestServices.GetService(typeof(IActionContextAccessor)) as IActionContextAccessor);
                        var giaoDichService = new GiaoDichService(dbContext);
                        HE_THONG_DOI_TAC doiTac = (new DoiTacService(dbContext)).FindPartner(username);
                        if (username != null)
                        {
                            var objGiaoDich = new GIAO_DICH()
                            {
                                GIAO_DICH_ID = 0,
                                IDDOITAC = doiTac.HE_THONG_DOI_TAC_ID,
                                MESSAGEID = requestId,
                                //REQUESTMESSAGE = CompressionUtils.CompressToByte(JsonConvert.SerializeObject(jsonContent)),
                                // Fix request string have too many slash after decompression 20180518
                                REQUESTMESSAGE = CompressionUtils.CompressToByte(jsonContent),
                                RESPONSEMESSAGE = context.Result == null ? null : CompressionUtils.CompressToByte(JsonConvert.SerializeObject(context.Result)),
                                THOIGIANNHAN = controller.BeginRequestTime,
                                THOIGIANTRAKETQUA = DateTime.Now,
                                TRANGTHAI = (long)context.HttpContext.Response.StatusCode,
                                SERVICE_URL = url
                            };

                            // Add ycDdlTruongHocId to GIAO_DICH
                            object ycDdlTruongHocId = 0;
                            var ycID = context.HttpContext.Request.Query["ycDdlTruongHocId"].ToString();
                            if (!string.IsNullOrEmpty(ycID))
                            {
                                ycDdlTruongHocId = ycID;
                            }
                            giaoDichService.Insert(objGiaoDich);
                        }
                        giaoDichService.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Co loi khi ghi log!");
            }
        }
    }
}