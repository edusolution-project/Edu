using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using FileManagerCore.Interfaces;
using FileManagerCore.Services;
using GoogleLib.Interfaces;
using GoogleLib.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmailTemplate
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
            services.AddSingleton<MailLogService>();
            services.AddSingleton<MailHelper>();
            services.AddSingleton<ClassService>();
            services.AddSingleton<ClassSubjectService>();
            services.AddSingleton<CenterService>();
            services.AddSingleton<LessonService>();
            //services.AddSingleton<LessonScheduleService>();
            services.AddSingleton<AccountService>();
            services.AddSingleton<StudentService>();
            services.AddSingleton<TeacherService>();
            services.AddSingleton<SkillService>();
            //services.AddSingleton<LessonScheduleService>();
            services.AddSingleton<RoleService>();
            services.AddSingleton<CourseService>();
            services.AddSingleton<LearningHistoryService>();
            services.AddSingleton<LessonProgressService>();
            services.AddSingleton<IRoxyFilemanHandler, RoxyFilemanHandler>();
            services.AddSingleton<ReportService>();
            services.AddSingleton<IndexService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
