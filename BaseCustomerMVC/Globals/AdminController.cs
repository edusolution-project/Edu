using Core_v2.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Globals
{
    [Area("admin")]
    [Permission("superadmin")]
    public class AdminController : BaseController
    {
        public AdminController()
        {
        }
    }
}
