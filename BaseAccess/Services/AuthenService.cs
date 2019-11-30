using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BaseAccess.Interfaces;

namespace BaseAccess.Services
{
    public class AuthenService : IAuthenService
    {
        public AuthenService()
        {

        }
        public IEnumerable<Permission> CreateListPermissions(string[] names)
        {
            if (names.Length == 0) return null;
            List<Permission> listItem = new List<Permission>();
            foreach(string name in names)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    listItem.Add(new Permission(name));
                }
            }
            return listItem;
        }

        public Permission CreatePermission(string name)
        {
            return string.IsNullOrEmpty(name) ? null : new Permission(name);
        }

        public Task SignIn(HttpContext httpContext, ClaimsPrincipal user, string cookiesName)
        {
            return httpContext.SignInAsync(cookiesName,user,new AuthenticationProperties() {
                ExpiresUtc = DateTime.Now.AddMinutes(60*24*365),
                AllowRefresh = true,
                IsPersistent = true
            });
        }

        public Task SignOut(HttpContext httpContext, string cookiesName)
        {
            return httpContext.SignOutAsync(cookiesName);
        }
    }
}
