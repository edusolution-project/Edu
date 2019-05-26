
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BasePublisherMVC.Globals;
using BasePublisherMVC.Models;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BasePublisherModels.Database;
using CoreMongoDB.Repositories;
using MongoDB.Driver;

namespace BasePublisherMVC.AdminControllers
{
    [MenuControl(
        CModule = "CPMenu",
        Name ="Sys : Chuyên mục",
        Order = 40,
        IShow = true
    )]
    public class CPMenuController : AdminController
    {
        private readonly CPMenuService _service;
        public CPMenuController(CPMenuService menuService)
        {
            _service = menuService;
        }
        public ActionResult Index(DefaultModel model)
        {
            DateTime startDate = model.StartDate > DateTime.MinValue ? new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0) : DateTime.MinValue;
            DateTime endDate = model.EndDate > DateTime.MinValue ? new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59) : DateTime.MinValue;

            var data = _service.Find(!string.IsNullOrEmpty(model.SearchText), o => o.Name.Contains(model.SearchText) || o.Type.Contains(model.SearchText))
                .Where(!string.IsNullOrEmpty(model.ID), o => o.ID == model.ID)
                .Where(string.IsNullOrEmpty(model.Record), o => o.ParentID.Equals("0"))
                .Where(!string.IsNullOrEmpty(model.Record),o => o.ParentID == model.Record)
                .Where(_currentLang != null,o=>o.LangID == _currentLang.ID)
                .Where(startDate > DateTime.MinValue, o => o.Created >= startDate)
                .Where(endDate > DateTime.MinValue, o => o.Created <= endDate)
                .OrderByDescending(o => o.ID)
                .ToList();

            ViewBag.Data = data.Skip(model.PageSize * model.PageIndex).Take(model.PageSize).ToList();
            model.TotalRecord = data.Count;
            ViewBag.Model = model;
            return View();
        }
        public IActionResult Create(DefaultModel model)
        {
            ViewBag.Title = "Thêm mới";
            if (!string.IsNullOrEmpty(model.ID))
            {
                return RedirectToAction("Edit", new { model.ID });
            }
            ViewBag.Root = _service.Find(true,o=>o.IsActive == true && o.ParentID.Equals("0"))
                .Where(_currentLang != null, o => o.LangID == _currentLang.ID).ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DefaultModel model, CPMenuEntity item)
        {
            ViewBag.Title = "Thêm mới";
            if (!string.IsNullOrEmpty(model.ID) || !string.IsNullOrEmpty(item.ID))
            {
                return RedirectToAction("Edit", new { model.ID });
            }
            else
            {
                if(!item.ParentID.Equals("0"))
                {
                    var root = _service.GetByID(item.ParentID);
                    if (root != null)
                    {
                        item.Type = root.Type;
                    }
                }
                item.LangID = _currentLang.ID;
                item.Code = UnicodeName.ConvertUnicodeToCode(item.Name, "-", true);
                await _service.AddAsync(item);
            }
            ViewBag.Root = _service.Find(true,o => o.ParentID.Equals("0") && o.IsActive == true)
                .Where(_currentLang != null, o => o.LangID == _currentLang.ID).ToList();
            return View();
        }
        public IActionResult Edit(string ID)
        {
            DefaultModel model = new DefaultModel
            {
                ID = ID
            };
            ViewBag.Title = "Chỉnh sửa";
            if (string.IsNullOrEmpty(ID))
            {
                return RedirectToAction("Create");
            }
            else
            {
                var item = _service.GetByID(ID);
                if (item == null)
                {
                    ViewBag.Message = "Not Found Data";
                }
                ViewBag.Data = item;
            }
            ViewBag.Root = _service.CreateQuery().FindList(true,o => string.IsNullOrEmpty(o.ParentID) && o.IsActive == true)
                .Where(_currentLang != null, o => o.LangID == _currentLang.ID).ToList();
            ViewBag.Model = model;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DefaultModel model, CPMenuEntity item)
        {
            ViewBag.Title = "Chỉnh sửa";
            if (string.IsNullOrEmpty(model.ID) && string.IsNullOrEmpty(item.ID))
            {
                ViewBag.Message = "Chưa chọn đối tượng đê sửa";
            }
            else
            {
                string ID = model.ID;
                var _item = _service.GetByID(ID);
                if(_item != null)
                {
                    item.ID = _item.ID;
                    item.Code = UnicodeName.ConvertUnicodeToCode(item.Name, "-", true);
                    item.LangID = _currentLang.ID;
                    await _service.AddAsync(item);
                }

                ViewBag.Data = _service.GetByID(ID);
            }
            ViewBag.Root = _service.CreateQuery().FindList(true,o => string.IsNullOrEmpty(o.ParentID) && o.IsActive == true)
                .Where(_currentLang != null, o => o.LangID == _currentLang.ID).ToList();
            ViewBag.Model = model;
            return RedirectToAction("index");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(DefaultModel model)
        {
            int delete = 0;
            if (string.IsNullOrEmpty(model.ArrID))
            {
                SetMessageError("Dữ liệu trống");
                return RedirectToAction("index");

            }
            else
            {
                var arr = model.ArrID.Split(',').ToList();
                int arrCount = arr != null ? arr.Count : 0;
                for (int i = 0; i < arrCount; i++)
                {
                    string ID = arr[i];
                    var item = _service.GetByID(ID);
                    if(item != null)
                    {
                        var listChild = _service.CreateQuery().Find(o => o.ParentID == item.ID).ToList();
                        _service.Remove(item.ID);
                        if(listChild != null)
                        {
                            await _service.RemoveRangeAsync(listChild.Select(o=>o.ID).ToList());
                        }
                        delete++;
                    }
                   
                }
                if(delete > 0)
                {
                    SetMessageSuccess("Đã xóa "+delete+" đối tượng");
                    return RedirectToAction("Index");
                }
                else
                {
                    SetMessageWarning("Không có đổi tượng nào bị xóa");
                    return RedirectToAction("Index");
                }
            }
            
        }

        [HttpPost]
        public void Export(DefaultModel model)
        {
            DateTime startDate = model.StartDate > DateTime.MinValue ? new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0) : DateTime.MinValue;
            DateTime endDate = model.EndDate > DateTime.MinValue ? new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59) : DateTime.MinValue;

            var data = _service.CreateQuery().FindList(!string.IsNullOrEmpty(model.SearchText), o => o.Name.Contains(model.SearchText) || o.Type.Contains(model.SearchText))
                .Where(!string.IsNullOrEmpty(model.ID), o => o.ID == model.ID)
                .Where(startDate > DateTime.MinValue, o => o.Created >= startDate)
                .Where(endDate > DateTime.MinValue, o => o.Created <= endDate)
                .OrderByDescending(o => o.ID)
                .ToList();

            DataTable dt = data.ToDataTable();

            Response.Clear();
            Response.Headers["content-disposition"] = "attachment;filename=Catalog.xls";
            Response.ContentType = "application/excel";
            
            string html = Query.ConvertDataTableToHTML(dt);
            HtmlString htmlTextWriter = new HtmlString(html);
            
            Response.WriteAsync(html);
        }
        [HttpGet]
        public void Export()
        {
            
            var data = _service.GetAll()
                .OrderByDescending(o => o.ID)
                .ToList();

            DataTable dt = data.ToDataTable();

            Response.Clear();
            Response.Headers["content-disposition"] = "attachment;filename=Catalog.xls";
            Response.ContentType = "application/excel";

            string html = Query.ConvertDataTableToHTML(dt);
            HtmlString htmlTextWriter = new HtmlString(html);

            Response.WriteAsync(html);
        }

        [HttpPost]
        public async Task<IActionResult> Active(DefaultModel model)
        {
            var arr = model.ArrID.Split(',').ToList();
            int arrCount = arr != null ? arr.Count : 0;
            for (int i = 0; i < arrCount; i++)
            {
                string ID = arr[i];
                var item = _service.GetByID(ID);
                if (item != null && !item.IsActive)
                {
                    item.IsActive = true;
                    await _service.AddAsync(item);
                }
            }
            ViewBag.Model = model;
            return RedirectToAction("Index", new { Record = model.Record });
        }
        [HttpPost]
        public async Task<IActionResult> NonActive(DefaultModel model)
        {
            var arr = model.ArrID.Split(',').ToList();
            int arrCount = arr != null ? arr.Count : 0;
            for (int i = 0; i < arrCount; i++)
            {
                string ID = arr[i];
                var item = _service.GetByID(ID);
                if (item != null && item.IsActive)
                {
                    item.IsActive = false;
                    await _service.AddAsync(item);
                }
            }
            ViewBag.Model = model;
            return RedirectToAction("Index", new { Record = model.Record });
        }

        public IActionResult Clear()
        {
             _service.ClearCache();
            return RedirectToAction("Index");
        }
    }
}
