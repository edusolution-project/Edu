using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BaseAccess;
using BaseCustomerEntity.Database;
using BaseCustomerEntity.Globals;
using BaseCustomerMVC.Globals;
using BaseEasyRealTime.Globals;
using BaseHub;
using com.wiris.plugin.api;
using Core_v2.Globals;
using EasyZoom;
using EnglishPlatform.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
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
            services.AddSingleton<MailHelper>();
            services.AddSingleton<CourseHelper>();
            services.AddSingleton<StudentHelper>();
            services.AddSingleton<LessonHelper>();
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
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
            });
            services.AddDistributedMemoryCache();
            services.AddSignalR();
            services.AddEasyZoom(Configuration);
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
            //app.UseAuthention(Configuration);
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.Use(async (context, next) =>
            {
                string center = context.Request.Path.Value != "" && context.Request.Path.Value != "/" ? context.Request.Path.Value.Split('/')[1] : string.Empty;
                if (!string.IsNullOrEmpty(center) &&
                !center.Contains("hub") &&
                !context.Request.Path.Value.Contains("EasyRealTime") &&
                !context.Request.Path.Value.Contains("home") &&
                !context.Request.Path.Value.Contains("login") &&
                !context.Request.Path.Value.Contains("logout") &&
                !(context.Request.Path.Value == "/"))
                {
                    string userID = context.User.FindFirst("UserID")?.Value;
                    string roleCode = context.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                    string type = context.User.FindFirst("Type")?.Value;
                    if (roleCode != "superadmin")
                    {
                        string key = $"{center}_{roleCode}";
                        string defaultKey = $"{userID}_{center}";
                        string currentKey = CacheExtends.GetDataFromCache<string>(defaultKey);
                        if (!string.IsNullOrEmpty(currentKey) && currentKey == key)
                        {
                            // some things
                        }
                        else
                        {
                            CenterService _centerService = new CenterService(Configuration);
                            AccountService _accountService = new AccountService(Configuration);
                            TeacherService _teacherService = new TeacherService(Configuration);
                            StudentService _studentService = new StudentService(Configuration);
                            RoleService _roleService = new RoleService(Configuration);
                            AccessesService _accessesService = new AccessesService(Configuration);

                            string centerCode = center;
                            var tc = _teacherService.GetItemByID(userID);
                            var st = _studentService.GetItemByID(userID);
                            var user = _accountService.GetItemByID(userID);
                            var defaultUser = new UserModel() { };
                            bool isRealCenter = false;

                            switch (type)
                            {
                                case ACCOUNT_TYPE.ADMIN:
                                    defaultUser = new UserModel(user.ID, "admin");
                                    centerCode = center;
                                    roleCode = user.UserName == "supperadmin@gmail.com" ? "superadmin" : "admin";
                                    break;
                                case ACCOUNT_TYPE.TEACHER:
                                    if (tc != null)
                                    {
                                        defaultUser = new UserModel(tc.ID, tc.FullName);
                                        centerCode = tc.Centers != null && tc.Centers.Count > 0 ? tc.Centers.FirstOrDefault().Code : center;
                                        roleCode = tc.Centers != null && tc.Centers.Count > 0 ? tc.Centers.FirstOrDefault().RoleID : "";
                                        isRealCenter = tc.Centers.Any(o => o.Code == centerCode);
                                    }
                                    break;
                                default:
                                    if (st != null)
                                    {
                                        defaultUser = new UserModel(st.ID, st.FullName);
                                        centerCode = (st.Centers != null && st.Centers.Count > 0) ? _centerService.GetItemByID(st.Centers.FirstOrDefault()).Code : center;
                                        roleCode = "student";
                                        isRealCenter = st.Centers != null && st.Centers.Any(o => o == centerCode);
                                    }
                                    break;
                            }

                            if (type != ACCOUNT_TYPE.ADMIN)
                            {
                                if (isRealCenter)
                                {
                                    var role = roleCode != "student" ? _roleService.GetItemByID(roleCode) : _roleService.GetItemByCode(roleCode);
                                    var listAccess = _accessesService.GetAccessByRole(role.Code);
                                    CacheExtends.SetObjectFromCache($"{defaultUser.ID}_{centerCode}", 3600 * 24 * 360, key);
                                    CacheExtends.SetObjectFromCache(key, 3600 * 24 * 360, listAccess.Select(o => o.Authority)?.ToList());
                                }
                                else
                                {
                                    context.Response.ContentType = "text/html;charset=utf-8";
                                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                                    await context.Response.WriteAsync("<p>Không tìm thấy thông tin</p>");
                                    await context.Response.WriteAsync("<a href='/'>quay về</a>");
                                }
                            }
                        }
                    }
                }
                // Do work that doesn't write to the Response.
                await next.Invoke();
                // Do logging or other work that doesn't write to the Response.
            });

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
                 name: "default",
                 template: "{controller=home}/{action=index}/{id?}"
               );
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
                   template: "{basis:basis}/{area:exists}/{controller=Home}/{action=Index}"
                 );
                routes.MapRoute(
                   name: "areas",
                   template: "{basis:basis}/{area:exists}/{controller=Home}/{action=Index}/{id?}"
                 );
                routes.MapRoute(
                   name: "areas",
                   template: "{basis:basis}/{area:exists}/{controller=Home}/{action=Index}/{id?}/{ClassID?}"
                 );
            });


        }
    }
    public class MyCustomerRoute : IRouteConstraint
    {
        public MyCustomerRoute()
        {

        }
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            string key = values[routeKey]?.ToString();
            string controller = values["controller"]?.ToString();
            string action = values["action"]?.ToString();
            string area = values["area"]?.ToString();
            // kieerm tra co so 

            return routeKey == "basis";
        }
    }
}
