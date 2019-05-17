
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BaseMVC.Globals;
using BaseMVC.Models;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BaseMongoDB.Database;
using CoreMongoDB.Repositories;

namespace BaseMVC.AdminControllers
{
    [MenuControl(
        CModule = "CPRole",
        Name ="Sys : Nhóm người dùng",
        Order = 40,
        IShow = true
    )]
    public class CPRoleController : AdminController
    {
        private readonly CPRoleService _service;
        private readonly CPAccessService _accessService;
        public CPRoleController(CPRoleService roleService, CPAccessService accessService)
        {
            _service = roleService;
            _accessService = accessService;
        }
        public ActionResult Index(DefaultModel model)
        {
            var data = _service.CreateQuery().FindList(!string.IsNullOrEmpty(model.SearchText), o => o.Name.Contains(model.SearchText) || o.Code.Contains(model.SearchText))
                .Where(!string.IsNullOrEmpty(model.ID), o => o.ID == model.ID)
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
        public async Task<IActionResult> Create(DefaultModel model, CPRoleEntity item)
        {
            ViewBag.Title = "Thêm mới";
            if (!string.IsNullOrEmpty(model.ID) || !string.IsNullOrEmpty(item.ID))
            {
                return RedirectToAction("Edit", new { model.ID });
            }
            else
            {
                if (string.IsNullOrEmpty(item.Name))
                {
                    ViewBag.Message = "Bạn chưa điện tên của nhóm";
                    return View();
                }
                else
                {
                    item.Code = UnicodeName.ConvertUnicodeToCode(item.Name, "-", true);

                    if (_service.GetItemByCode(item.Code) == null)
                    {
                        await _service.AddAsync(item);
                        ViewBag.Message = "Thêm thành công";
                    }
                    else
                    {
                        ViewBag.Message = "Nhóm người đã tồn tại";
                        return View();
                    }
                    
                }
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
                ViewBag.Data = item;
            }

            ViewBag.Model = model;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DefaultModel model, CPRoleEntity item)
        {
            ViewBag.Title = "Chỉnh sửa";
            if (string.IsNullOrEmpty(model.ID) && string.IsNullOrEmpty(item.ID))
            {
                ViewBag.Message = "Chưa chọn đối tượng đê sửa";
            }
            else
            {
                string ID = !string.IsNullOrEmpty(model.ID) ? model.ID : item.ID;
                var _item = _service.GetByID(ID);
                if (!_item.Lock)
                {
                    item.ID = _item.ID;
                    if (string.IsNullOrEmpty(item.Name))
                    {
                        item.Name = _item.Name;
                    }
                    if (string.IsNullOrEmpty(item.Code))
                    {
                        item.Code = UnicodeName.ConvertUnicodeToCode(item.Name,"-",true);
                    }
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
                    if (!item.Lock)
                    {
                        var listAccess = _accessService.GetItemByRoleID(item.ID);
                        await _service.RemoveAsync(item.ID);
                        await _accessService.RemoveRangeAsync(listAccess.Select(o=>o.ID).ToList());
                        
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
            //DateTime startDate = model.StartDate > DateTime.MinValue ? new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0) : DateTime.MinValue;
            //DateTime endDate = model.EndDate > DateTime.MinValue ? new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59) : DateTime.MinValue;

            var data = _service.CreateQuery().FindList(!string.IsNullOrEmpty(model.SearchText), o => o.Name.Contains(model.SearchText) || o.Code.Contains(model.SearchText))
                .Where(!string.IsNullOrEmpty(model.ID), o => o.ID == model.ID)
                //.Where(startDate > DateTime.MinValue, o => o.Created >= startDate)
                //.Where(endDate > DateTime.MinValue, o => o.Created <= endDate)
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

        //[HttpPost]
        //public async Task<IActionResult> Active(DefaultModel model)
        //{
        //    var arr = model.ArrID.Split(',').ToList();
        //    int arrCount = arr != null ? arr.Count : 0;
        //    for (int i = 0; i < arrCount; i++)
        //    {
        //        string ID = arr[i];
        //        var item = _service.GetByID(ID);
        //        if (item != null && !item.IsActive)
        //        {
        //            item.IsActive = true;
        //            await _service.AddAsync(item);
        //        }
        //    }
        //    return RedirectToAction("Index");
        //}
        //[HttpPost]
        //public async Task<IActionResult> NonActive(DefaultModel model)
        //{
        //    var arr = model.ArrID.Split(',').ToList();
        //    int arrCount = arr != null ? arr.Count : 0;
        //    for (int i = 0; i < arrCount; i++)
        //    {
        //        string ID = arr[i];
        //        var item = _service.GetByID(ID);
        //        if (item != null && item.IsActive)
        //        {
        //            item.IsActive = false;
        //            await _service.AddAsync(item);
        //        }
        //    }
        //    return RedirectToAction("Index");
        //}

        public IActionResult Clear()
        {
             _service.ClearCache();
            return RedirectToAction("Index");
        }
    }
}
