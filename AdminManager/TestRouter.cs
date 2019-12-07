using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AdminManager
{
    public class TestRouter : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            throw new NotImplementedException();
        }
    }
}
