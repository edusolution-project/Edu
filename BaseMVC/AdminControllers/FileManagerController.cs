﻿using Microsoft.AspNetCore.Mvc;
using BaseMVC.Globals;

namespace BaseMVC.AdminControllers
{
    [MenuControl(CModule = "FileManager", Icon = "document", Name = "Sys : Quản lý file")]
    public class FileManagerController : AdminController
    {
        public FileManagerController()
        {
            
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
    }
}
