using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace RouterCustomer
{
    public class RouterContrain : IRouteConstraint
    {
        const string _keyKey = "key";
        const string _keyDomain = "domain";
        const string _keyController = "controller";
        const string _keyAction = "action";
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var dataKey = httpContext.Session.GetString(_keyKey);
            string domain = values[_keyDomain].ToString();
            string controller = values[_keyController].ToString();
            string action = values[_keyAction].ToString();
            if (string.IsNullOrEmpty(domain)) return false;
            if (string.IsNullOrEmpty(controller)) return false;
            if (string.IsNullOrEmpty(action)) return false;
            return true;
        }
    }
}
