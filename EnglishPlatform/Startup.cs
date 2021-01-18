using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Threading.Tasks;
using BaseAccess;
using BaseCustomerEntity.Database;
using BaseCustomerEntity.Globals;
using BaseCustomerMVC.Globals;
using BaseEasyRealTime.Globals;
using BaseHub;
using Core_v2.Globals;
using EasyZoom;
using EnglishPlatform.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using RestSharp;
//using WebPush.Services;

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
            //services.AddPushSubscriptionStore(Configuration)
            //    .AddPushNotificationService(Configuration)
            //    .AddPushNotificationsQueue();
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = int.MaxValue;
            });
            services.AddAuthentication(Cookies.DefaultLogin) // Sets the default scheme to cookies
                .AddCookie(Cookies.DefaultLogin, options =>
                {
                    options.AccessDeniedPath = "/denied";
                    options.LoginPath = "/login";
                });
            services.AddAccess();
            services.Configure<DefaultConfigs>(Configuration.GetSection("DefaultConfigs"));
            services.AddLogs();
            services.AddTransient<IndefindCtrlService>();
            services.AddServiceBase();
            services.AddScoped<FileProcess>();
            services.AddSingleton<CalendarHelper>();
            services.AddSingleton<MailHelper>();
            services.AddSingleton<CourseHelper>();
            services.AddSingleton<ClassHelper>();
            services.AddSingleton<ProgressHelper>();
            services.AddSingleton<StudentHelper>();
            services.AddSingleton<LessonHelper>();
            services.AddSingleton<TeacherHelper>();
            services.AddDistributedMemoryCache();
            services.AddMemoryCache();
            services.AddSingleton<CacheHelper>();

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
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
            });
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

            app.UseEndpointRouting();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseAuthentication();
            app.GetConfiguration(Configuration);
            //app.UseAuthention(Configuration);
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            //app.UsePushSubscriptionStore();

            app.Use(async (content, next) =>
            {
                if (content.Request.Query.ContainsKey("googletest"))
                {
                    string str = Program.GoogleDriveApiService != null ? $"ok {Program.GoogleDriveApiService.URL_VIEW_FILE}" : "null";
                    await content.Response.WriteAsync(str);
                }
                await next.Invoke();
            });
            app.Use(async (context, next) =>
            {
                if (CacheExtends.GetDataFromCache<List<AuthorityEntity>>(CacheExtends.DefaultPermission) == null)
                {
                    AuthorityService authorityService = new AuthorityService(Configuration);
                    List<AuthorityEntity> data = authorityService.GetAll()?.ToList();
                    CacheExtends.SetObjectFromCache(CacheExtends.DefaultPermission, 3600 * 24 * 360, data);
                }
                var routingFeature = context.Features[typeof(IRoutingFeature)] as IRoutingFeature;
                //context.Features[typeof(IEndpointFeature)];
                RouteData routeValues = routingFeature?.RouteData;
                if (routeValues != null)
                {
                    if (routeValues.Values.Keys.Contains("basis"))
                    {
                        string center = routeValues.Values.GetValueOrDefault("basis")?.ToString();
                        if (!string.IsNullOrEmpty(center))
                        {
                            string userID = context.User.FindFirst("UserID")?.Value;
                            string roleCode = context.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                            string type = context.User.FindFirst("Type")?.Value;
                            if (roleCode != "superadmin")
                            {
                                string key = $"{roleCode}";
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
                                                centerCode = string.IsNullOrEmpty(center) && tc.Centers != null && tc.Centers.Count > 0 ? tc.Centers.FirstOrDefault().Code : center;
                                                roleCode = tc.Centers != null && tc.Centers.Count > 0 ? tc.Centers.FirstOrDefault().RoleID : "";
                                                isRealCenter = tc.Centers.Any(o => o.Code == centerCode);
                                            }
                                            break;
                                        default:
                                            if (st != null)
                                            {
                                                defaultUser = new UserModel(st.ID, st.FullName);
                                                centerCode = (string.IsNullOrEmpty(center) && st.Centers != null && st.Centers.Count > 0) ? _centerService.GetItemByID(st.Centers.FirstOrDefault()).Code : center;
                                                roleCode = "student";
                                                isRealCenter = st.Centers != null && st.Centers.Any(o => o == _centerService.GetItemByCode(centerCode).ID);
                                                //var avatar = st != null && !string.IsNullOrEmpty(st.Avatar) ? st.Avatar : null;
                                                //HttpContext.Session.SetString("userAvatar", avatar);
                                            }
                                            break;
                                    }

                                    if (type != ACCOUNT_TYPE.ADMIN)
                                    {
                                        if (isRealCenter)
                                        {
                                            var role = roleCode != "student" ? _roleService.GetItemByID(roleCode) : _roleService.GetItemByCode(roleCode);
                                            CacheExtends.SetObjectFromCache($"{defaultUser.ID}_{centerCode}", 3600 * 24 * 360, key);
                                            if (CacheExtends.GetDataFromCache<List<string>>(key) == null)
                                            {
                                                var listAccess = _accessesService.GetAccessByRole(role.Code);
                                                var access = listAccess.Select(o => o.Authority)?.ToList();
                                                CacheExtends.SetObjectFromCache(key, 3600 * 24 * 360, access);
                                            }
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
                    }
                }
                await next.Invoke();
                // Do logging or other work that doesn't write to the Response.
            });
            app.UseCors(builder =>
            {
                builder
                .SetIsOriginAllowed(IsOriginAllowed)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
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
                    name: "news-default",
                    template: "tin-tuc",
                    defaults: new { controller = "News", action = "Index" }
                );
                routes.MapRoute(
                    name: "news-recruitment",
                    template: "tin-tuc/tuyen-dung",
                    defaults: new { controller = "News", action = "Category", catcode = "tuyen-dung" }
                );
                routes.MapRoute(
                   name: "news-event",
                   template: "tin-tuc/su-kien",
                   defaults: new { controller = "News", action = "Category", catcode = "su-kien" }
               );
                routes.MapRoute(
                    name: "news-product",
                    template: "tin-tuc/san-pham",
                    defaults: new { controller = "News", action = "Category", catcode = "san-pham" }
                );
                routes.MapRoute(
                    name: "news-about-us",
                    template: "tin-tuc/ve-eduso",
                    defaults: new { controller = "News", action = "Detail", catcode = "gioi-thieu", newscode = "ve-eduso" }
                );
                routes.MapRoute(
                    name: "news-category",
                    template: "tin-tuc/{catcode}",
                    defaults: new { controller = "News", action = "Category" }
                );
                routes.MapRoute(
                    name: "news-detail",
                    template: "tin-tuc/{catcode}/{newscode}",
                    defaults: new { controller = "News", action = "Detail" }
                );
                routes.MapRoute(
                    name: "product-detail",
                    template: "chi-tiet-khoa-hoc/{code}",
                    defaults: new { controller = "News", action = "DetailProduct" }
                );
                routes.MapRoute(
                 name: "default",
                 template: "{controller=home}/{action=index}/{id?}"
                );
                routes.MapRoute(
                   name: "areas0",
                   template: "{area:exists}/{controller=Home}/{action=Index}"
                );
                routes.MapRoute(
                   name: "areas1",
                   template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                 );
                routes.MapRoute(
                   name: "areas2",
                   template: "{area:exists}/{controller=Home}/{action=Index}/{id?}/{ClassID?}"
                 );
                routes.MapRoute(
                   name: "areas3",
                   template: "{basis:basis}/{area:exists}/{controller=Home}/{action=Index}"
                 );
                routes.MapRoute(
                   name: "areas4",
                   template: "{basis:basis}/{area:exists}/{controller=Home}/{action=Index}/{id?}"
                 );
                routes.MapRoute(
                   name: "areas5",
                   template: "{basis:basis}/{area:exists}/{controller=Home}/{action=Index}/{id?}/{ClassID?}"
                 );
            });
        }
        private bool IsOriginAllowed(string host)
        {
            if (host.Contains("localhost")) return true;
            if (host.Contains("eduso.vn")) return true;
            return false;
            //var originConfig = Configuration.GetSection("AllowOrigin").Value;
            //var corsOriginAllowed = new[] { originConfig };
            //return corsOriginAllowed.Any(origin =>Regex.IsMatch(host, $@"^http(s)?://.*{origin}(:[0-9]+)?$", RegexOptions.IgnoreCase));
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
