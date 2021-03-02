using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnglishPlatform.Controllers
{
    public class ZoomController : ControllerBase
    {
        //https://zoom.us/oauth/authorize?response_type=code&client_id=WyJcArKQSUW_FoXdaC1lQ&redirect_uri=https%3A%2F%2Feduso.vn%2Fzoom%2Foauth
        public JsonResult OAuth()
        {
            var http = HttpContext;
            return new JsonResult("");
        }
        public JsonResult Deauthorization()
        {
            var http = HttpContext;
            return new JsonResult("");
        }
    }
}
