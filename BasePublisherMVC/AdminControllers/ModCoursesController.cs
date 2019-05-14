using BasePublisherMVC.Globals;
using System;
using System.Collections.Generic;
using System.Text;
using BasePublisherMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace BasePublisherMVC.AdminControllers
{
    [MenuControl(
        CModule = "ModCourses",
        Name = "MO : Quản lý khóa học",
        Order = 4,
        IShow = true,
        Type = MenuType.Mod
    )]
    public class ModCoursesController : AdminController
    {
        public ActionResult Index(DefaultModel model)
        {
            return View();
        }
        public ActionResult Details(DefaultModel model)
        {
            return View();
        }
    }
}
