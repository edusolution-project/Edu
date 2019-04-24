using BaseMongoDB.Factory;
using BaseMVC.Globals;
using CoreLogs;
using EntityBaseFW;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace AdminPage_MongoDB
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            // thay doi theo request
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ILogs,Logs>();
            services.AddServiceBase();
            // 1 nguoi - ko doi

            services.AddMvc(options=> {
                options.Filters.Add<PermissionAttribute>();
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(errorApp => {

                    errorApp.Run(async context =>
                    {
                        ILogs logs = new Logs(env);
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "text/html";
                        await logs.WriteLogsInfo(context.Request.Protocol);
                        await context.Response.WriteAsync("<html lang=\"en\"><body>\r\n");

                        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                        await context.Response.WriteAsync("ERROR!<br><br>\r\n" + exceptionHandlerPathFeature?.Error.ToString());

                        // Use exceptionHandlerPathFeature to process the exception (for example, 
                        // logging), but do NOT expose sensitive error information directly to 
                        // the client.

                        if (exceptionHandlerPathFeature?.Error is FileNotFoundException)
                        {
                            await context.Response.WriteAsync("File error thrown!<br><br>\r\n");
                        }
                        await logs.WriteLogsError(exceptionHandlerPathFeature?.Error.ToString());

                        await context.Response.WriteAsync("<a href=\"/\">Home</a><br>\r\n");
                        await context.Response.WriteAsync("</body></html>\r\n");
                        await context.Response.WriteAsync(new string(' ', 512)); // IE padding
                    });

                });
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.GetConfiguration(Configuration);
            // get sqlconfig cho core entity framework =)))
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            // lấy user authentication (tự customer) =)))
            app.UseAuthention(Configuration);
            // ngôn ngữ mặc định 
            app.UseResource(Configuration);
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=CPHome}/{action=Index}/{id?}");
            });
            //*******      CKFinder    *********** ///
            var CKFinderLic = Configuration.GetSection("CKFinder");
            string key = CKFinderLic["Key"].ToString();
            string value = CKFinderLic["Value"].ToString();
            // lấy host vì phải dùng trang quản trị riêng và trang client riêng.
            string host = CKFinderLic["Host"].ToString();
            
            app.UseOwinAppBuilder(env.WebRootPath, key, value, host);
            //*******      End    *********** ///
        }
    }
}
