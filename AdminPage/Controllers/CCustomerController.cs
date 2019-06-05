using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityBaseNet;
using EntityBaseNet.Models;
using GlobalNet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceBaseNet;
using ServiceBaseNet.Services;
using Controller = ServiceBaseNet.ControllerBase;

namespace AdminPage.Controllers
{
    [CusMenu(CModule = "CCustomer", Icon = "people",Name = "Quản lý khách hàng",Type = "Mod")]
    public class CCustomerController : Controller
    {
        private readonly SysAccountServices _sysUser;
        private readonly SysRoleServices _sysRoles;
        public CCustomerController(IHttpContextAccessor httpContext)
        {
            _sysUser = new SysAccountServices();
            _sysRoles = new SysRoleServices();
        }
        // GET: User
        public ActionResult Index(UserModel model)
        {
            DateTime startDate = model.StartDate > DateTime.MinValue ? new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0) : DateTime.MinValue;
            DateTime endDate = model.EndDate > DateTime.MinValue ? new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59) : DateTime.MinValue;

            var data = _sysUser.DbQuery.Where(!string.IsNullOrEmpty(model.SearchText), o => o.Name.Contains(model.SearchText) || o.Email.Contains(model.SearchText))
                .Where(model.ID > 0, o => o.ID == model.ID)
                .Where(startDate > DateTime.MinValue, o => o.Created >= startDate)
                .Where(endDate > DateTime.MinValue, o => o.Created <= endDate)
                .OrderByDescending(o => o.ID)
                .ToList();

            ViewBag.Data = data.Skip(model.PageSize * model.PageIndex).Take(model.PageSize).ToList();
            model.TotalRecord = data.Count;
            ViewBag.Model = model;
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(AccountModel model, SysAccount account)
        {
            await _sysUser.SaveChangesAsync();

            return View();
        }
        public IActionResult Edit()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(AccountModel model, SysAccount account)
        {
            await _sysUser.SaveChangesAsync();

            return View();
        }
        [HttpPost]
        public async Task Delete(int[] ArrID)
        {
            if (ArrID.Count() <= 0) await Task.CompletedTask;
            else
            {
                foreach(int id in ArrID)
                {
                    var item = id < 0? null : _sysUser.GetItemByID(id);
                    if(item != null)
                    {
                        var role = _sysRoles.GetItemByID(item.RoleID);
                        if (!role.Lock)
                        {
                            await _sysUser.RemoveItemAsync(item);
                        }
                    }
                }
            }
        }

        [HttpGet]
        public void Import()
        {
            return;
        }
        [HttpPost]
        public void Import(RolesModel model)
        {
            return;
        }

        [HttpPost]
        public async Task<IActionResult> Active(RolesModel model)
        {
            var arr = model.ArrID.Split(',').Select(int.Parse).ToList();
            int arrCount = arr != null ? arr.Count : 0;
            for (int i = 0; i < arrCount; i++)
            {
                int id = arr[i];
                var item = _sysUser.GetItemByID(id);
                if(item != null && !item.Activity)
                {
                    item.Activity = true;
                   await _sysUser.InsertItemAsync(item);
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> NonActive(RolesModel model)
        {
            var arr = model.ArrID.Split(',').Select(int.Parse).ToList();
            int arrCount = arr != null ? arr.Count : 0;
            for (int i = 0; i < arrCount; i++)
            {
                int id = arr[i];
                var item = _sysUser.GetItemByID(id);
                if (item != null && item.Activity)
                {
                    item.Activity = false;
                    await _sysUser.InsertItemAsync(item);
                }
            }
            return RedirectToAction("Index");
        }
    }
}