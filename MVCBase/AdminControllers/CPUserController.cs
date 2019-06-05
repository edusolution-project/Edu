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
        CModule = "CPUser",
        Name ="Sys : Người dùng",
        Order = 40,
        IShow = true
    )]
    public class CPUserController : AdminController
    {
        private readonly CPUserService _service;
        private readonly CPRoleService _roleService;
        private readonly List<CPRoleEntity> _listRoles;
        public CPUserController()
        {
            _service = new CPUserService();
            _roleService = new CPRoleService();
            _listRoles = _roleService.GetAllItem();
        }
        public ActionResult Index(DefaultModel model)
        {
            DateTime startDate = model.StartDate > DateTime.MinValue ? new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0) : DateTime.MinValue;
            DateTime endDate = model.EndDate > DateTime.MinValue ? new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59) : DateTime.MinValue;

            var data = _service.CreateQuery().Find(!string.IsNullOrEmpty(model.SearchText), o => o.Name.Contains(model.SearchText) || o.Email.Contains(model.SearchText))
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
        public IActionResult Create(DefaultModel model)
        {
            ViewBag.Title = "Thêm mới";
            if (model.ID > 0)
            {
                return RedirectToAction("Edit", new { model.ID });
            }
            ViewBag.RoleData = _listRoles;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DefaultModel model, CPUserEntity item)
        {
            if(_service.IsExistByEmail(item.Email))
            {
                ViewBag.Message = "Email đã tồn tại";
                return View();
            }
            ViewBag.Title = "Thêm mới";
            if (model.ID > 0 || item.ID > 0)
            {
                return RedirectToAction("Edit", new { model.ID });
            }
            else
            {
                item.Pass = Security.Encrypt(item.Pass);
                await _service.SaveAsync(item);
            }
            ViewBag.RoleData = _listRoles;
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
            ViewBag.RoleData = _listRoles;
            ViewBag.Model = model;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DefaultModel model, CPUserEntity item)
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
                item.ID = _item.ID;
                item.Email = _item.Email;
                item.Created = _item.Created > DateTime.MinValue ? _item.Created : DateTime.Now;
                if(item.Pass != _item.Pass)
                {
                    item.Pass = Security.Encrypt(item.Pass);
                }
                await _service.SaveAsync(item);

                ViewBag.Data = _service.GetItemByID(ID);
            }
            ViewBag.Model = model;
            ViewBag.RoleData = _listRoles;
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
                    _service.CreateQuery().Remove(item);
                    await _service.CreateQuery().CompleteAsync();
                    delete++;
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

            var data = _service.CreateQuery().Find(!string.IsNullOrEmpty(model.SearchText), o => o.Name.Contains(model.SearchText) || o.Email.Contains(model.SearchText))
                .Where(model.ID > 0, o => o.ID == model.ID)
                .Where(startDate > DateTime.MinValue, o => o.Created >= startDate)
                .Where(endDate > DateTime.MinValue, o => o.Created <= endDate)
                .OrderByDescending(o => o.ID)
                .Select(o => new { o.Name,o.BirthDay,o.Email,o.Phone,o.Skype,o.Created,o.CurrentRoleName,o.ID })
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
            return RedirectToAction("Index");
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
            return RedirectToAction("Index");
        }

        public IActionResult Clear()
        {
             _service.CreateQuery().ClearCache();
            return RedirectToAction("Index");
        }
    }
}
