using BaseCustomerMVC.Globals;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Controllers.Admin
{
    [BaseAccess.Attribule.AccessCtrl("Chăm sóc khách hàng", "admin", 13)]
    public class CustomerCareController: AdminController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
