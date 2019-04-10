using BaseModels;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVCBase.Globals;
using MVCBase.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MVCBase.AdminControllers
{
    [MenuControl(
        CModule = "ModExamples",
        Name = "Mod : Quản lý Bài Tập",
        Order = 40,
        IShow = false
    )]
    public class ModExamplesController : AdminController
    {
        private readonly ModExampleService _service;
        public ModExamplesController()
        {
            _service = new ModExampleService();
        }
        public ActionResult Create(DefaultModel model)
        {
            if(model.ID > 0)
            {
                var data = _service.GetItemByRoleID(model.ID);
                if (data != null)
                {
                    ViewBag.Data = data;
                    ViewBag.Control = _menu.GetAdminMenu;
                }
            }
            ViewBag.Model = model;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create(DefaultModel model, string ArrID)
        {
            if (model.ID > 0 && !string.IsNullOrEmpty(ArrID))
            {
                var arr = model.ArrID.Split(',').ToList();
                int count = arr.Count;
                for (int i = 0; i < count; i++)
                {
                    var iview = arr[i].Split('|');

                    string cname = iview[0];
                    string aname = iview[1];
                    bool isTrue = iview[2] == "true";

                    var oldItem = _service.GetItem(model.ID, cname, aname);
                    if (oldItem != null)
                    {
                        oldItem.Activity = isTrue;
                        await _service.SaveAsync(oldItem);
                    }
                    else
                    {
                        ModExampleEntity item = new ModExampleEntity()
                        {
                            CModule = cname,
                            CMethod = aname,
                            Activity = isTrue,
                            RoleID = model.ID
                        };
                        await _service.SaveAsync(item);
                    }
                    
                }
                ViewBag.Data = _service.GetItemByRoleID(model.ID);
            }
            ViewBag.Control = _menu.GetAdminMenu;
            ViewBag.Model = model;
            return View();
        }
        public IActionResult Clear()
        {
             _service.CreateQuery().ClearCache();
            return RedirectToAction("Index");
        }
    }
}
