using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Globals
{
    [Permission]
    public class BaseController : Controller
    {
        public BaseController()
        {
            var data = new IndefindCtrlService();
        }
    }
}
