using BasePublisherModels.Database;
using BasePublisherMVC.Globals;
using BasePublisherMVC.Models;
using CoreMongoDB.Repositories;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BasePublisherMVC.ViewModel;

namespace BasePublisherMVC.AdminControllers
{
    [MenuControl(
        CModule = "ModGrades",
        Name = "MO : Quản lý cấp độ",
        Order = 2,
        Icon = "grade",
        IShow = true,
        Type = MenuType.Mod
    )]
    public class ModGradesController : AdminController
    {
        private readonly ModGradeService _service;
        private readonly CPUserService _userService;
        private readonly ModProgramService _programService;

        public ModGradesController(ModGradeService service, CPUserService userService, ModProgramService programService)
        {
            _service = service;
            _userService = userService;
            _programService = programService;
        }

        public ActionResult Index(DefaultModel model)
        {
            var results = new List<ModGradeViewModel>();

            var data = _service
                .Find(!string.IsNullOrEmpty(model.SearchText), o => (o.Name.Contains(model.SearchText) || o.Code.Contains(model.SearchText)))
                .Where(o => o.CreateUser == _currentUser.ID)
                //.Where(!string.IsNullOrEmpty(model.ID), o => o.ID == model.ID)
                //.Where(string.IsNullOrEmpty(model.Record), o => (o.ParentID.Equals("0") || string.IsNullOrEmpty(o.ParentID)))
                .Where(!string.IsNullOrEmpty(model.Record), o => o.ParentID == model.Record)
                .OrderBy(o => o.Name)
                .ToList();

            results.AddRange(data.Select(t => new ModGradeViewModel(t)
            {
                Parent = (string.IsNullOrEmpty(t.ParentID) || t.ParentID == "0") ? null : _service.GetByID(t.ParentID),
                SubGradeCount = _service.CountSubGradeByID(t.ID)
            }));


            ViewBag.Data = results.ToList();
            model.TotalRecord = results.Count;
            ViewBag.Model = model;
            return View();
        }

        public IActionResult Create(DefaultModel model)
        {
            ViewBag.Title = "Thêm mới";
            string userID = _currentUser.ID;
            ViewBag.Root = _service.Find(true, o => o.IsActive == true && (o.ParentID.Equals("0") || string.IsNullOrEmpty(o.ParentID))).ToList();
            if (!string.IsNullOrEmpty(model.ID))
            {
                return RedirectToAction("Edit", new { model.ID });
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DefaultModel model, ModGradeEntity item)
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
                    SetMessageWarning("Bạn chưa điền tên của cấp độ");
                    return View();
                }
                else
                {
                    item.Code = UnicodeName.ConvertUnicodeToCode(item.Name, "-", true);
                    item.Created = DateTime.Now;
                    item.Updated = DateTime.Now;
                    item.IsAdmin = true;
                    item.CreateUser = _currentUser.ID;
                    if (_service.GetItemByCode(item.Code) == null)
                    {
                        await _service.AddAsync(item);
                        SetMessageSuccess("Thêm mới thành công");
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        SetMessageWarning("Cấp độ đã tồn tại!");
                        ViewBag.Root = _service.GetRootItems();
                        return View();
                    }

                }
            }
            ViewBag.Root = _service.GetRootItems();
            return View();
        }

        public IActionResult Edit(string ID)
        {
            ViewBag.Root = _service.Find(true, o => o.IsActive == true && (o.ParentID.Equals("0") || string.IsNullOrEmpty(o.ParentID))).ToList();
            DefaultModel model = new DefaultModel
            {
                ID = ID
            };
            ViewBag.Title = "Cập nhật thông tin";
            if (string.IsNullOrEmpty(ID))
            {
                return RedirectToAction("Create");
            }
            else
            {
                var item = _service.GetByID(ID);
                if (item == null)
                {
                    SetMessageError("Không tìm thấy đối tượng");
                }
                ViewBag.Data = item;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DefaultModel model, ModGradeEntity item)
        {
            ViewBag.Title = "Cập nhật thông tin";
            if (string.IsNullOrEmpty(model.ID) && string.IsNullOrEmpty(item.ID))
            {
                SetMessageWarning("Chưa chọn đối tượng chỉnh sửa");
            }
            else
            {
                string ID = !string.IsNullOrEmpty(model.ID) ? model.ID : item.ID;
                var _item = _service.GetByID(ID);
                if (string.IsNullOrEmpty(item.Name))
                {
                    _item.Name = item.Name;
                    _item.Updated = DateTime.Now;
                }
                //TODO: tính toán xem có cần cập nhật lại code ko
                _item.ParentID = item.ParentID;
                _item.IsActive = item.IsActive;
                await _service.AddAsync(_item);
                ViewBag.Root = _service.GetRootItems();
                ViewBag.Data = _service.GetByID(ID);
                SetMessageSuccess("Cập nhật thành công");
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
                    if (item != null)
                    {
                        var listChild = _service.GetItemsByParentID(ID).ToList();
                        if (listChild.Count > 0)
                        {
                            SetMessageWarning("Đang có các cấp độ phụ thuộc vào cấp độ này, không được xóa!");
                            return RedirectToAction("Index");
                        }

                        //Kiểm tra giáo trình có môn học này, nếu có thì không cho xóa
                        if (_programService.FindByGrade(item.ID) != null)
                        {
                            SetMessageWarning("Đang có giáo trình sử dụng cấp độ này, không được xóa!");
                            return RedirectToAction("Index");
                        }
                        await _service.RemoveAsync(item.ID); delete++;
                        delete++;
                    }
                }
                if (delete > 0)
                {
                    SetMessageSuccess("Đã xóa " + delete + " đối tượng");
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

            var data = _service.CreateQuery().FindList(!string.IsNullOrEmpty(model.SearchText), o => o.Name.Contains(model.SearchText) || o.Code.Contains(model.SearchText))
                .Where(!string.IsNullOrEmpty(model.ID), o => o.ID == model.ID)
                .Where(startDate > DateTime.MinValue, o => o.Created >= startDate)
                .Where(endDate > DateTime.MinValue, o => o.Created <= endDate)
                .OrderByDescending(o => o.ID)
                .ToList();

            DataTable dt = data.ToDataTable();

            Response.Clear();
            Response.Headers["content-disposition"] = "attachment;filename=DanhMucCapDo.xls";
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
            SetMessageSuccess("Cập nhật thành công");
            return RedirectToAction("Index");
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
            SetMessageSuccess("Cập nhật thành công");
            return RedirectToAction("Index");
        }

        public IActionResult Clear()
        {
            _service.ClearCache();
            return RedirectToAction("Index");
        }
    }
}
