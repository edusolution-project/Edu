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
        public static void UseCores(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.Use(next => context =>
            {
                // get cookie domain
                if (context.Request.Cookies.TryGetValue("domain",out string domain))
                {
                    // có domain //get 
                    if (context.Request.Cookies.TryGetValue(domain, out string key))
                    {
                        KeyModel item = Newtonsoft.Json.JsonConvert.DeserializeObject<KeyModel>(Security.Decrypt(key));
                        if (item == null)
                        {
                            context.Remove("key");
                        }
                        else
                        {
                            System.DateTime now = new System.DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, System.DateTime.Now.Hour, System.DateTime.Now.Minute, System.DateTime.Now.Second); ;
                            System.DateTime limit = new System.DateTime(item.EndDate.Year, item.EndDate.Month, item.EndDate.Day, 23, 59, 59);
                            if ((limit - now).TotalSeconds > 0)
                            {
                                keyOver = domain;
                            }
                        }
                    }
                    else
                    {
                        
                    }
                }
                else
                {

                }
                return next(context);
            });
        }
        public static void AddCores(this IServiceCollection services)
        {

        }
        public static void AddLogs(this IServiceCollection services)
        {
            services.AddTransient<ILog, Log>();
        }
    }
}
