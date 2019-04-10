using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityBaseNet;
using EntityBaseNet.Models;
using GlobalNet;
using GlobalNet.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceBaseNet;
using ServiceBaseNet.Services;
using Controller = ServiceBaseNet.ControllerBase;

namespace AdminPage.Controllers
{
    [CusMenu(CModule = "CLang", Icon = "language",Name = "Quản lý ngôn ngữ",Type = "Sys")]
    public class CLangController : Controller
    {
        private readonly SysLangServices _service;
        public CLangController(IHttpContextAccessor httpContext)
        {
            _service = new SysLangServices();
        }
        // GET: User
        public ActionResult Index(UserModel model)
        {
            var data = _service.DbQuery.Where(!string.IsNullOrEmpty(model.SearchText), o => o.Name.Contains(model.SearchText) || o.Code.Contains(model.SearchText))
                .Where(model.ID > 0, o => o.ID == model.ID)
                .OrderByDescending(o => o.ID)
                .ToList();

            ViewBag.Data = data.Skip(model.PageSize * model.PageIndex).Take(model.PageSize).ToList();
            model.TotalRecord = data.Count;
            ViewBag.Model = model;
            return View();
        }
        public IActionResult Create(RolesModel model)
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
        public async Task<IActionResult> Create(DefaultModel model, SysLang item)
        {
            ViewBag.Title = "Thêm mới";
            if (model.ID > 0 || item.ID > 0)
            {
                return RedirectToAction("Edit", new { model.ID });
            }
            else
            {
                if (string.IsNullOrEmpty(item.Name) || string.IsNullOrEmpty(item.Code))
                {

                }
                await _service.InsertItemAsync(item);
            }
            return View();
        }
        public IActionResult Edit(int ID)
        {
            RolesModel model = new RolesModel
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

            ViewBag.Model = model;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DefaultModel model, SysLang item)
        {
            ViewBag.Title = "Chỉnh sửa";
            if (model.ID <= 0 && item.ID <= 0)
            {
                ViewBag.Message = "Chưa chọn đối tượng đê sửa";
            }
            else
            {
                int ID = model.ID > 0 ? model.ID : item.ID;
                var _item = _service.GetItemByID(ID);
                item.ID = _item.ID;
                if (string.IsNullOrEmpty(item.Name))
                {
                    item.Name = _item.Name;
                }
                if (string.IsNullOrEmpty(item.Code))
                {
                    item.Code = _item.Code;
                }
                await _service.InsertItemAsync(item);

                ViewBag.Data = _service.GetItemByID(ID);
            }
            ViewBag.Model = model;
            return RedirectToAction("index");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(RolesModel model)
        {
            Message messageSuccess = new Message();
            Message messageError = new Message();
            Message messageWarning = new Message();
            string msg = "";
            string type = "success";
            int delete = 0;
            if (string.IsNullOrEmpty(model.ArrID))
            {
                msg = "Dữ liệu trống";
                type = "danger";
            }
            else
            {
                var arr = model.ArrID.Split(',').Select(int.Parse).ToList();
                int arrCount = arr != null ? arr.Count : 0;
                for (int i = 0; i < arrCount; i++)
                {
                    int id = arr[i];
                    var item = _service.GetItemByID(id);
                    
                    await _service.RemoveItemAsync(item);
                    delete++;
                    msg = "đã xóa";
                   

                }
            }
            Message message = new Message()
            {
                Content = msg,
                Number = delete,
                Type = type
            };
            //CacheExtends.SetObjectFromCache(_keyCache + "delete", 240, message);
            return RedirectToAction("Index");
        }
    }
}