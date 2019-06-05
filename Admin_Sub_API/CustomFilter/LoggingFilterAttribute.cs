//using System;
//using System.Web.Http;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Newtonsoft.Json.Serialization;
//using Microsoft.Extensions.Logging;

//namespace SME.API.CustomFilter
//{
//    public class LoggingFilterAttribute : ActionFilterAttribute
//    {
//        public override void OnActionExecuting(ActionExecutingContext filterContext)
//        {
//            GlobalConfiguration.Configuration.Services.Replace(typeof(ITraceWriter), new ILogger());
//            var trace = GlobalConfiguration.Configuration.Services.GetTraceWriter();
//            trace.Info(filterContext.HttpContext.Request, "Controller : " + filterContext.ControllerContext.ControllerDescriptor.ControllerType.FullName + Environment.NewLine + "Action : " + filterContext.ActionDescriptor.ActionName, "JSON", filterContext.ActionArguments);
//        }
//        public override void OnActionExecuted(ActionExecutedContext filterContext)
//        {
//            //GlobalConfiguration.Configuration.Services.Replace(typeof(ITraceWriter), new NLogger());
//            //var trace = GlobalConfiguration.Configuration.Services.GetTraceWriter();
//            if (filterContext.Exception != null)
//            {
//                var reponseObject = new
//                {
//                    Response = new
//                    {
//                        Content = filterContext.Exception
//                    }
//                };
//                trace.Error(null, "Error: " + JObject.FromObject(reponseObject).ToString(), "JSON", "");
//            }
//            else if (filterContext.HttpContext.Response != null)
//            {
//                if (filterContext.HttpContext.Response.ContentLength != null)
//                {
//                    var reponseObject = new
//                    {
//                        Response = new
//                        {
//                            StatusCode = filterContext.HttpContext.Response.StatusCode,
//                            Content = filterContext.HttpContext.Response.ContentLength
//                        }
//                    };
//                    try
//                    {
//                        trace.Info(null, "Response : " + JObject.FromObject(reponseObject).ToString(), "JSON", "");
//                    }
//                    catch (Exception)
//                    {
//                        trace.Info(null, "Response : Parsing response Error", "JSON", "");
//                    }
//                }
//                else
//                {
//                    var reponseObject = new
//                    {
//                        Response = new
//                        {
//                            StatusCode = filterContext.HttpContext.Response.StatusCode
//                        }
//                    };
//                    try
//                    {
//                        trace.Info(null, "Response : " + JObject.FromObject(reponseObject).ToString(), "JSON", "");
//                    }
//                    catch (Exception)
//                    {
//                        trace.Info(null, "Response : Parsing response Error", "JSON", "");
//                    }
//                }
//            }

//        }
//    }
//}