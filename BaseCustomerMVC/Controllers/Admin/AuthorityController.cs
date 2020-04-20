using BaseAccess.Attribule;
using BaseAccess.Interfaces;
using BaseAccess.Services;
using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Core_v2.Globals;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BaseCustomerMVC.Controllers.Admin
{
    [BaseAccess.Attribule.AccessCtrl("Phân quyền (root)", "admin", 1)]
    public class AuthorityController : AdminController
    {
        private readonly AuthorityService _authorityService;
        private readonly IAccess _access;
        public AuthorityController(AuthorityService authorityService)
        {
            _authorityService = authorityService;
            _access = new AccessService();
        }

        public ActionResult Index(DefaultModel model)
        {
            var assembly = GetAssembly();
            ViewBag.AdminCtrl = _access.GetAccessByAttribue<Globals.AdminController>(assembly, "admin");
            ViewBag.TeacherCtrl = _access.GetAccessByAttribue<Globals.TeacherController>(assembly, "teacher");
            ViewBag.StudentCtrl = _access.GetAccessByAttribue<Globals.StudentController>(assembly, "student");
            return View();
        }
        public ActionResult Detail()
        {
            var assembly = GetAssembly();
            ViewBag.AdminCtrl = _access.GetAccessByAttribue<Globals.AdminController>(assembly, "admin");
            ViewBag.TeacherCtrl = _access.GetAccessByAttribue<Globals.TeacherController>(assembly, "teacher");
            ViewBag.StudentCtrl = _access.GetAccessByAttribue<Globals.StudentController>(assembly, "student");
            return View();
        }
        private Assembly GetAssembly()
        {
            return Assembly.GetExecutingAssembly();
        }
    }
}
