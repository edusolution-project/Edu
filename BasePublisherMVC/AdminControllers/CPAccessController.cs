using BasePublisherMVC.Globals;
using System.Linq;
using System.Threading.Tasks;
using BasePublisherMVC.Models;
using BasePublisherModels.Database;
using Microsoft.AspNetCore.Mvc;

namespace BasePublisherMVC.AdminControllers
{
    [MenuControl(
        CModule = "CPAccess",
        Name = "Sys : Quản lý quyền",
        Order = 40,
        IShow = false
    )]
    public class CPAccessController : AdminController
    {
        private readonly CPAccessService _service;
        public CPAccessController(CPAccessService accessService)
        {
            _service = accessService;
        }
        public ActionResult Create(DefaultModel model)
        {
            if(!string.IsNullOrEmpty(model.ID))
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
            if (!string.IsNullOrEmpty(model.ID) && !string.IsNullOrEmpty(ArrID))
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
                        await _service.AddAsync(oldItem);
                    }
                    else
                    {
                        CPAccessEntity item = new CPAccessEntity()
                        {
                            CModule = cname,
                            CMethod = aname,
                            Activity = isTrue,
                            RoleID = model.ID
                        };
                        await _service.AddAsync(item);
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
             _service.ClearCache();
            return RedirectToAction("Index");
        }
    }
}
