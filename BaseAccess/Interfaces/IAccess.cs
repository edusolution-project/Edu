using BaseAccess.Attribule;
using BaseAccess.Models;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;

namespace BaseAccess.Interfaces
{
    public interface IAccess
    {
        string ClaimType { get; }
        List<AccessCtrlAttribute> GetAccessByAttribue<T>(Assembly assembly, string group);
        List<AccessModel> GetAccessWithoutAttribue<T>(Assembly assembly, string group);

        bool IsAccess(ClaimsPrincipal user , string ctrl, string action, string type);

    }
}
