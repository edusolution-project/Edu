
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

namespace BasePublisherMVC.AdminControllers
{
    [MenuControl(
        CModule = "CPResource",
        Name ="Sys : Tài nguyên",
        Order = 40,
        IShow = true
    )]
    public class CPResourceController : AdminController
    {
        private readonly CPResourceService _service;
        public CPResourceController(CPResourceService resourceService)
        {
            _service = resourceService;
        }
        public ActionResult Index(DefaultModel model)
        {
            var data = _service.CreateQuery().FindList(!string.IsNullOrEmpty(model.SearchText), o => o.Code.Contains(model.SearchText) || o.Value.Contains(model.SearchText))
                .Where(!string.IsNullOrEmpty(model.ID), o => o.ID == model.ID)
                .Where(_currentLang != null,o=>o.LangID == _currentLang.ID)
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
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DefaultModel model, CPResourceEntity item)
        {
            ViewBag.Title = "Thêm mới";
            if (!string.IsNullOrEmpty(model.ID) || !string.IsNullOrEmpty(item.ID))
            {
                return RedirectToAction("Edit", new { model.ID });
            }
            else
            {
                item.LangID = _currentLang.ID;
                await _service.AddAsync(item);
            }
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
                else
                {
                    ViewBag.Data = item;
                }
            }
            
            ViewBag.Model = model;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DefaultModel model, CPResourceEntity item)
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
                    item.LangID =string.IsNullOrEmpty(item.LangID) ? _item.LangID : item.LangID;
                    await _service.AddAsync(item);
                }

                ViewBag.Data = _service.GetByID(ID);
            }
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
                        
                        await _service.RemoveAsync(item.ID);
                        
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
            var data = _service.CreateQuery().FindList(!string.IsNullOrEmpty(model.SearchText), o => o.Code.Contains(model.SearchText) || o.Value.Contains(model.SearchText))
                .Where(!string.IsNullOrEmpty(model.ID), o => o.ID == model.ID)
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
        public IActionResult Clear()
        {
             _service.ClearCache();
            return RedirectToAction("Index");
        }
    }
}
