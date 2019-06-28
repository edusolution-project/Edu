using Core_v2.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Globals
{
    [Area("student")]
    [Permission("student,superadmin")]
    public class StudentController : BaseController
    {
        public StudentController()
        {
        }
    }
}
