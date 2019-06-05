using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace MVCBase.ClientControllers
{
    public class AccountController : ClientController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
