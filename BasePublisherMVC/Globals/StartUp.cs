
using BasePublisherModels.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace BasePublisherMVC.Globals
{
    public static class StartUp
    {
        private static CPLangEntity _currentLang;
        private static CPUserEntity _currentUser;
        private static List<CPResourceEntity> _currentResource;
        public static CPUserEntity CurrentUser => _currentUser;
        public static CPLangEntity CurrentLang => _currentLang;
        public static List<CPResourceEntity> CurrentResource => _currentResource;
        public static IApplicationBuilder UseAuthention(this IApplicationBuilder app,IConfiguration configuration)
        {
            app.Use(next => context =>
            {
                context.User = context.GetCurrentUser(configuration,ref _currentUser);
                
                return next(context);
            });

            return app;
        }
        public static IApplicationBuilder UseResource(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.Use(next => context =>
            {
                context.GetCurrentResource(configuration,ref _currentResource,ref _currentLang);
                return next(context);
            });

            return app;
        }
        private static void GetCurrentResource(this HttpContext context,IConfiguration configuration,ref List<CPResourceEntity> currentResource, ref CPLangEntity currentLang)
        {
            string cookie = context.GetValue(Cookies.DefaultLang,false);
            if (string.IsNullOrEmpty(cookie)) { cookie = "VN"; context.SetCurrentLang("VN"); }
            var lang = new CPLangService(configuration);
            currentLang = lang.CreateQuery().Find(o => o.IsActive == true && o.Code == cookie)?.SingleOrDefault();
            if (currentLang != null)
            {
                // cache
                var cache = CacheExtends.GetDataFromCache<List<CPResourceEntity>>(cookie + "-" + CacheExtends.DefaultLang);
                //
                if (cache != null) { currentResource = cache; return; }

                var res = new CPResourceService(configuration);
                currentResource = res.GetByLangID(currentLang.ID);
            }
        }
        private static ClaimsPrincipal GetCurrentUser(this HttpContext context, IConfiguration configuration, ref CPUserEntity user)
        {
            string token = context.GetValue(Cookies.DefaultLogin, false);
            if (string.IsNullOrEmpty(token)) return null;
            else
            {
                // neeus co cache
                var cache = CacheExtends.GetDataFromCache<ClaimsPrincipal>(token);
                if (cache != null) return cache;
                // ko co cache
                var logs = new CPLoginLogService(configuration);
                string email = logs.GetEmailFromDb(token);
                if (string.IsNullOrEmpty(email))
                {
                    return null;
                }
                else
                {
                    var account = new CPUserService(configuration);
                        user = account.GetItemByEmail(email);
                    if (user == null) return null;
                    else
                    {
                        var role = new CPRoleService(configuration);
                        var irole = role.GetByID(user.RoleID);
                        if (role == null) return null;
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(ClaimTypes.Name, user.Name),
                            new Claim(ClaimTypes.Role, irole.Code),
                            new Claim("RoleID", irole.ID.ToString())
                        };
                        var claimsIdentity = new ClaimsIdentity(claims, Cookies.DefaultLogin);

                        var authenProperties = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTime.UtcNow.AddMinutes(Cookies.ExpiresLogin)
                        };
                        ClaimsPrincipal claim = new ClaimsPrincipal();
                        claim.AddIdentity(claimsIdentity);

                        CacheExtends.SetObjectFromCache(token, Cookies.ExpiresLogin, claim);

                        return claim;
                    }
                }
            }
        }

        public static IApplicationBuilder UseCustomerMvc(this IApplicationBuilder app)
        {
            app.UseMvc(routes=> {
                routes.MapRoute(
                    name: "test",
                    template: "{controller=CPHome}/{action=Index}/{id?}"
                );
                //routes.MapRoute(
                //    name:"default",
                //    template: "{AdminControllers.controller=CPHome}/{action=Index}/{id?}"
                //);
            });

            return app;
        }
    }
}
