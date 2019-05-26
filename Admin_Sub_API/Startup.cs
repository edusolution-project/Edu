using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
//using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using NLog.Extensions.Logging;
using NLog.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using SMERest.Common;
using BaseMongoDB.Factory;

namespace SMERest
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
           
            var timout = Configuration["Session:TimeOut"];
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllHeaders",
                builder =>
                {
                    builder.AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod();
                });
            });

            //services.AddCors();

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(Convert.ToInt32(timout));
                options.Cookie.HttpOnly = true;
            });
            services.AddServiceBase();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddDbContext<SMEEntities>(
            //    options => options.UseMySql(Configuration.GetConnectionString("SMEConnectionStrings"),
            //        mysqlOptions =>
            //        {
            //            mysqlOptions.ServerVersion(new Version(5, 7, 21), ServerType.MySql);
            //        }
            //));


            services.AddMvc();//.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //services.Configure<ForwardedHeadersOptions>(options =>
            //{
            //    options.KnownProxies.Add(IPAddress.Parse("127.0.0.1"));
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseCors("AllowAllHeaders");
            app.Use(async (ctx, next) =>
            {
                await next();
                if (ctx.Response.StatusCode == 204)
                {
                    ctx.Response.ContentLength = 0;
                }
            });

            app.UseCors("AllowAllHeaders");

            //app.UseMiddleware(typeof(CorsMiddleware));
           // RoleManagementService.Init(env);
            //app.UseCors(builder =>
            //builder.WithOrigins("http://localhost:58434"));
            //.AllowAnyHeader()
            //.AllowAnyMethod());

            //app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials());
    //        app.UseCors(builder => builder
    //.AllowAnyOrigin()
    //.AllowAnyMethod()
    //.AllowAnyHeader());
//            app.UseCors(corsPolicyBuilder =>
//   corsPolicyBuilder.WithOrigins("http://10.60.157.110:8989")
//.AllowAnyMethod()
//    .AllowCredentials()
//    .AllowAnyHeader().AllowCredentials()
//  );
            //app.UseAuthentication();
            env.ConfigureNLog("nlog.config");
            app.UseDefaultFiles();
            app.UseStaticFiles();
            //app.UseHttpsRedirection();
            app.UseSession();
            loggerFactory.AddLog4Net();
            loggerFactory.AddNLog();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "api/{controller=CPHome}/{action=Index}/{id?}");
            });
            //app.UseForwardedHeaders(new ForwardedHeadersOptions
            //{
            //    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            //});

            //app.UseAuthentication();

            //app.UseMvc(routes =>
            //{                           
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Authentication}/{action=Login}/{id?}");
            //});
            // app.AddNLogWeb();
            //loggerFactory.AddLog4Net();
        }
    }
}
