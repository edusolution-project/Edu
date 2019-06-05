using BaseModels;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVCBase.Globals;
using MVCBase.Models;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MVCBase.AdminControllers
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
        public CPMenuController()
        {
            _service = new CPMenuService();
        }
        public ActionResult Index(DefaultModel model)
        {
            DateTime startDate = model.StartDate > DateTime.MinValue ? new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0) : DateTime.MinValue;
            DateTime endDate = model.EndDate > DateTime.MinValue ? new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59) : DateTime.MinValue;

            var data = _service.CreateQuery().Find(!string.IsNullOrEmpty(model.SearchText), o => o.Name.Contains(model.SearchText) || o.Type.Contains(model.SearchText))
                .Where(model.ID > 0, o => o.ID == model.ID)
                .Where(o => o.ParentID == model.Record)
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
            if (model.ID > 0)
            {
                return RedirectToAction("Edit", new { model.ID });
            }
            ViewBag.Root = _service.CreateQuery().Find(o => o.ParentID == 0 && o.Activity == true)
                .Where(_currentLang != null, o => o.LangID == _currentLang.ID).ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DefaultModel model, CPMenuEntity item)
        {
            ViewBag.Title = "Thêm mới";
            if (model.ID > 0 || item.ID > 0)
            {
                return RedirectToAction("Edit", new { model.ID });
            }
            else
            {
                if(item.ParentID > 0)
                {
                    var root = _service.GetItemByID(item.ParentID);
                    if (root != null)
                    {
                        item.Type = root.Type;
                    }
                }
                item.LangID = _currentLang.ID;
                item.Code = UnicodeName.ConvertUnicodeToCode(item.Name, "-", true);
                await _service.SaveAsync(item);
            }
            ViewBag.Root = _service.CreateQuery().Find(o => o.ParentID == 0 && o.Activity == true)
                .Where(_currentLang != null, o => o.LangID == _currentLang.ID).ToList();
            return View();
        }
        public IActionResult Edit(int ID)
        {
            DefaultModel model = new DefaultModel
            {
                ID = ID
            };
            ViewBag.Title = "Chỉnh sửa";
            if (ID == 0)
            {
                return RedirectToAction("Create");
            }
            else
            {
                var item = _service.GetItemByID(ID);
                if (item == null)
                {
                    ViewBag.Message = "Not Found Data";
                }
                ViewBag.Data = item;
            }
            ViewBag.Root = _service.CreateQuery().Find(o => o.ParentID == 0 && o.Activity == true)
                .Where(_currentLang != null, o => o.LangID == _currentLang.ID).ToList();
            ViewBag.Model = model;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DefaultModel model, CPMenuEntity item)
        {
            ViewBag.Title = "Chỉnh sửa";
            if (model.ID <= 0 && item.ID <= 0)
            {
                ViewBag.Message = "Chưa chọn đối tượng đê sửa";
            }
            else
            {
                int ID = model.ID;
                var _item = _service.GetItemByID(ID);
                if(_item != null)
                {
                    item.ID = _item.ID;
                    item.Code = UnicodeName.ConvertUnicodeToCode(item.Name, "-", true);
                    item.LangID = _currentLang.ID;
                    await _service.SaveAsync(item);
                }

                ViewBag.Data = _service.GetItemByID(ID);
            }
            ViewBag.Root = _service.CreateQuery().Find(o => o.ParentID == 0 && o.Activity == true)
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
                var arr = model.ArrID.Split(',').Select(int.Parse).ToList();
                int arrCount = arr != null ? arr.Count : 0;
                for (int i = 0; i < arrCount; i++)
                {
                    int id = arr[i];
                    var item = _service.GetItemByID(id);
                    if(item != null)
                    {
                        var listChild = _service.CreateQuery().Find(o => o.ParentID == item.ID).ToList();
                        _service.CreateQuery().Remove(item);
                        await _service.CreateQuery().CompleteAsync();
                        if(listChild != null)
                        {
                            _service.CreateQuery().RemoveRange(listChild);
                            await _service.CreateQuery().CompleteAsync();
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

            var data = _service.CreateQuery().Find(!string.IsNullOrEmpty(model.SearchText), o => o.Name.Contains(model.SearchText) || o.Type.Contains(model.SearchText))
                .Where(model.ID > 0, o => o.ID == model.ID)
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
            
            var data = _service.CreateQuery().GetAll()
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
            var arr = model.ArrID.Split(',').Select(int.Parse).ToList();
            int arrCount = arr != null ? arr.Count : 0;
            for (int i = 0; i < arrCount; i++)
            {
                int id = arr[i];
                var item = _service.GetItemByID(id);
                if (item != null && !item.Activity)
                {
                    item.Activity = true;
                    await _service.SaveAsync(item);
                }
            }
            ViewBag.Model = model;
            return RedirectToAction("Index", new { Record = model.Record });
        }
        [HttpPost]
        public async Task<IActionResult> NonActive(DefaultModel model)
        {
            var arr = model.ArrID.Split(',').Select(int.Parse).ToList();
            int arrCount = arr != null ? arr.Count : 0;
            for (int i = 0; i < arrCount; i++)
            {
                int id = arr[i];
                var item = _service.GetItemByID(id);
                if (item != null && item.Activity)
                {
                    item.Activity = false;
                    await _service.SaveAsync(item);
                }
            }
            ViewBag.Model = model;
            return RedirectToAction("Index", new { Record = model.Record });
        }

        public IActionResult Clear()
        {
             _service.CreateQuery().ClearCache();
            return RedirectToAction("Index");
        }
    }
}
