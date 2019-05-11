using BasePublisherMVC.Globals;
using BasePublisherMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasePublisherMVC.AdminControllers
{
    [MenuControl(
        CModule = "ModPrograms",
        Name = "MO : Quản lý chương trình",
        Order = 40,
        Icon = "card_travel",
        IShow = true,
        Type = MenuType.Mod
    )]
    public class ModProgramsController : AdminController
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
