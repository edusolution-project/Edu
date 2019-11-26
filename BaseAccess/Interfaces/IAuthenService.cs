using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BaseAccess.Interfaces
{
    public interface IAuthenService
    {
        IEnumerable<Permission> CreateListPermissions(string[] names);
        Permission CreatePermission(string name);
        Task SignIn(HttpContext httpContext, ClaimsPrincipal user, string cookiesName);

        Task SignOut(HttpContext httpContext, string cookiesName);
    }
}
