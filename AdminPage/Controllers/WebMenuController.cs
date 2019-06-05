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
    [CusMenu(CModule = "WebMenu", Icon = "nav",Name = "Quản lý chuyên mục",Type = "Sys")]
    public class WebMenuController : Controller
    {
        private readonly WebMenuServices _webMenu;
        public WebMenuController()
        {
            _webMenu = new WebMenuServices();
        }
        // GET: User
        public ActionResult Index(WebMenuModel model)
        {
            ViewBag.CurrentLang = CurrentLang;

            DateTime startDate = model.StartDate > DateTime.MinValue ? new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0) : DateTime.MinValue;
            DateTime endDate = model.EndDate > DateTime.MinValue ? new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59) : DateTime.MinValue;

            var data = _webMenu.DbQuery.Where(!string.IsNullOrEmpty(model.SearchText), o => o.Name.Contains(model.SearchText) || o.Code.Contains(model.SearchText))
                .Where(model.ID > 0, o => o.ID == model.ID)
                .Where(startDate > DateTime.MinValue, o => o.Created >= startDate)
                .Where(endDate > DateTime.MinValue, o => o.Created <= endDate)
                .OrderByDescending(o => o.ID)
                .ToList();
            ViewBag.Data = new List<WebMenu>()
                {
                    new WebMenu()
                    {
                        ID = 1,
                        Name = "Test",
                        Code = "test",
                        Type = "menu",
                        Created =DateTime.Now,
                        Content = "chưa có",
                        LangID = 1,
                        Activity = true,
                        Files = "chưa có",
                        ParentID = 0,
                        Summary = "đây là thằng cha"
                    },
                    new WebMenu()
                    {
                        ID = 2,
                        Name = "Test2",
                        Code = "test2",
                        Type = "menu",
                        Created =DateTime.Now,
                        Content = "chưa có",
                        LangID = 1,
                        Activity = false,
                        Files = "chưa có",
                        ParentID = 0,
                        Summary = "đây là thằng cha"
                    }
                };//;data.Skip(model.PageSize * model.PageIndex).Take(model.PageSize).ToList();
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
            await _webMenu.SaveChangesAsync();

            return View();
        }
        public IActionResult Edit()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(AccountModel model, SysAccount account)
        {
            await _webMenu.SaveChangesAsync();

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
                    var item = id < 0? null : _webMenu.GetItemByID(id);
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
                var item = _webMenu.GetItemByID(id);
                if(item != null && !item.Activity)
                {
                    item.Activity = true;
                   await _webMenu.InsertItemAsync(item);
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
                var item = _webMenu.GetItemByID(id);
                if (item != null && item.Activity)
                {
                    item.Activity = false;
                    await _webMenu.InsertItemAsync(item);
                }
            }
            return RedirectToAction("Index");
        }
    }
}