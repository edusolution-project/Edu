using BaseCustomerEntity.Database;
using Core_v2.Globals;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace BaseCustomerMVC.Globals
{
    public static class StartUp
    {
        public static Claim GetClaimByType(this IEnumerable<Claim> claims , string type)
        {
            try
            {
                if (claims != null) return claims.SingleOrDefault(o => o.Type == type);
                return null;
            }
            catch
            {
                return null;
            }
        }
        public static IConfiguration Configuration { get; private set; }
        public static void GetConfiguration(this IApplicationBuilder app,IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public static IApplicationBuilder UseAuthention(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.Use(next => context =>
            {
                if(!context.User.Identity.IsAuthenticated) context.User = context.GetCurrentUser(configuration);
                return next(context);
            });

            return app;
        }
        private static ClaimsPrincipal GetCurrentUser(this HttpContext context, IConfiguration configuration)
        {
            string token = context.GetValue(Cookies.DefaultLogin, false);
            if (string.IsNullOrEmpty(token)) return null;
            else
            {
                // neeus co cache
                var cache = CacheExtends.GetDataFromCache<ClaimsPrincipal>(token);
                if (cache != null) return cache;
                // ko co cache
                var logs = new AccountLogService(configuration);
                var _currentLog = logs.GetItemByToken(token);
                if (_currentLog == null)
                {
                    return null;
                }
                else
                {
                    var account = new AccountService(configuration);
                    var user = account.GetItemByID(_currentLog.AccountID);
                    if (user == null) return null;
                    else
                    {
                        var role = new RoleService(configuration);
                        var irole = role.GetItemByID(user.RoleID);
                        if (role == null) return null;
                        string FullName,id;
                        switch (user.Type)
                        {
                            case "teacher": var tc = new TeacherService(configuration).GetItemByID(user.UserID);
                                FullName = tc.FullName;
                                id = tc.ID;
                                break;
                            case "student": var st = new StudentService(configuration).GetItemByID(user.UserID);
                                FullName = st.FullName;
                                id = st.ID;
                                break;
                            default:
                                FullName = "admin"; id = "0";
                                break;
                        }

                        var claims = new List<Claim>
                        {
                            new Claim("UserID", id),
                            new Claim(ClaimTypes.Email, user.UserName),
                            new Claim(ClaimTypes.Name, FullName),
                            new Claim(ClaimTypes.Role, irole.Code),
                            new Claim("Type", user.Type),
                            new Claim("Code", irole.Code),
                            new Claim("RoleID", irole.ID.ToString())
                        };
                        var claimsIdentity = new ClaimsIdentity(claims, Cookies.DefaultLogin);
                        _ = new AuthenticationProperties
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
    }
}
