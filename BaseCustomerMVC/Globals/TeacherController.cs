using Core_v2.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Globals
{
    [Area("teacher")]
    [Permission("superadmin,teacher,head-teacher")]
    public class TeacherController: BaseController
    {

        public TeacherController()
        {
        }
    }
}
