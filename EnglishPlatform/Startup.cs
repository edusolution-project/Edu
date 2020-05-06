﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseAccess;
using BaseCustomerEntity.Globals;
using BaseCustomerMVC.Globals;
using BaseEasyRealTime.Globals;
using BaseHub;
using Core_v2.Globals;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnglishPlatform
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
            services.AddAuthentication(Cookies.DefaultLogin) // Sets the default scheme to cookies
                .AddCookie(Cookies.DefaultLogin, options =>
                {
                    options.AccessDeniedPath = "/denied";
                    options.LoginPath = "/login";
                });
            services.AddEasyRealTime();
            services.AddAccess();
            services.Configure<DefaultConfigs>(Configuration.GetSection("DefaultConfigs"));
            services.AddLogs();
            services.AddTransient<IndefindCtrlService>();
            services.AddServiceBase();
            services.AddScoped<FileProcess>();
            services.AddSingleton<CalendarHelper>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMvc(options =>
            {
                options.Filters.Add<PermissionAttribute>();
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.Configure<RouteOptions>(routeOptions =>
            {
                routeOptions.ConstraintMap.Add("basis", typeof(MyCustomerRoute));
            });

            services.AddSession();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddDistributedMemoryCache();
            services.AddSignalR();
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
                app.UseExceptionHandler("/Error/");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseCookiePolicy();
            app.UseSession();
            app.UseAuthentication();
            app.GetConfiguration(Configuration);
            app.UseAuthention(Configuration);
            //app.UseHttpsRedirection();
            app.UseStaticFiles();
           
            
            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/chatHub");
            });
            app.UseSignalR(routes =>
            {
                routes.MapHub<MyHub>("/hub");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                   name: "areas",
                   template: "{area:exists}/{controller=Home}/{action=Index}"
                 );
                routes.MapRoute(
                   name: "areas",
                   template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                 );
                routes.MapRoute(
                   name: "areas",
                   template: "{area:exists}/{controller=Home}/{action=Index}/{id?}/{ClassID?}"
                 );
                routes.MapRoute(
                   name: "areas",
                   template: "{basis:basis}-{area:exists}/{controller=Home}/{action=Index}"
                 );
                routes.MapRoute(
                   name: "areas",
                   template: "{basis:basis}_{area:exists}/{controller=Home}/{action=Index}/{id?}"
                 );
                routes.MapRoute(
                   name: "areas",
                   template: "{basis:basis}_{area:exists}/{controller=Home}/{action=Index}/{id?}/{ClassID?}"
                 );
                routes.MapRoute(
                   name: "default",
                   template: "{controller=home}/{action=index}/{id?}"
                 );

            });
        }
    }
    public class MyCustomerRoute : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            string key = values[routeKey]?.ToString();
            string controller = values["controller"]?.ToString();
            string action = values["action"]?.ToString();
            string area = values["area"]?.ToString();

            // kieerm tra co so 
            
            return true;
        }
    }
}
