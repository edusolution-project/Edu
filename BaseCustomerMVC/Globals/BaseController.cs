﻿using Core_v2.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Globals
{

    [Authorize]
    public class BaseController : Controller
    {
        public BaseController()
        {
            var data = new IndefindCtrlService().getByAdmin();

        }

    }
}
