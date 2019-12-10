using Core_v2.Interfaces;
using Core_v2.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core_v2.Globals
{
    public static class Startup
    {
        public static string keyOver = "";
        public static void UseCores(this IApplicationBuilder app)
        {
            app.Use(next => context =>
            {
                // get cookie domain
                if (context.Request.Cookies.TryGetValue("keyOver", out string key))
                {
                    KeyModel item = Newtonsoft.Json.JsonConvert.DeserializeObject<KeyModel>(Security.Decrypt(key));
                    if (item == null)
                    {
                        context.Remove("keyOver");
                        context.Request.Headers.Add("SigoutNow", "yes");
                    }
                    else
                    {
                        System.DateTime now = new System.DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, System.DateTime.Now.Hour, System.DateTime.Now.Minute, System.DateTime.Now.Second); ;
                        System.DateTime limit = new System.DateTime(item.EndDate.Year, item.EndDate.Month, item.EndDate.Day, 23, 59, 59);
                        if ((limit - now).TotalSeconds > 0)
                        {
                            keyOver = item.Domain;
                        }
                        else
                        {
                            context.Remove("keyOver");
                            context.Request.Headers.Add("SigoutNow", "yes");
                        }
                    }
                }
                else
                {
                    //chayj link get key
                    context.Request.Headers.Add("SigoutNow", "yes");
                }
                return next(context);
            });
        }
        public static void AddLogs(this IServiceCollection services)
        {
            services.AddTransient<ILog, Log>();
        }
    }
}
