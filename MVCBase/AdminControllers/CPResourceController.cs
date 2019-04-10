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
        CModule = "CPResource",
        Name ="Sys : Tài nguyên",
        Order = 40,
        IShow = true
    )]
    public class CPResourceController : AdminController
    {
        private readonly CPResourceService _service;
        public CPResourceController()
        {
            _service = new CPResourceService();
        }
        public ActionResult Index(DefaultModel model)
        {
            var data = _service.CreateQuery().Find(!string.IsNullOrEmpty(model.SearchText), o => o.Code.Contains(model.SearchText) || o.Value.Contains(model.SearchText))
                .Where(model.ID > 0, o => o.ID == model.ID)
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
            if (model.ID > 0)
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
            if (model.ID > 0 || item.ID > 0)
            {
                return RedirectToAction("Edit", new { model.ID });
            }
            else
            {
                item.LangID = _currentLang.ID;
                await _service.SaveAsync(item);
            }
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
                    item.LangID = item.LangID <= 0 ? _item.LangID : item.LangID;
                    await _service.SaveAsync(item);
                }

                ViewBag.Data = _service.GetItemByID(ID);
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
                var arr = model.ArrID.Split(',').Select(int.Parse).ToList();
                int arrCount = arr != null ? arr.Count : 0;
                for (int i = 0; i < arrCount; i++)
                {
                    int id = arr[i];
                    var item = _service.GetItemByID(id);
                    if(item != null)
                    {
                        
                        _service.CreateQuery().Remove(item);
                        await _service.CreateQuery().CompleteAsync();
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
            var data = _service.CreateQuery().Find(!string.IsNullOrEmpty(model.SearchText), o => o.Code.Contains(model.SearchText) || o.Value.Contains(model.SearchText))
                .Where(model.ID > 0, o => o.ID == model.ID)
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
        public IActionResult Clear()
        {
             _service.CreateQuery().ClearCache();
            return RedirectToAction("Index");
        }
    }
}
