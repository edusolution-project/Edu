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
        CModule = "ModChapters",
        Name = "MO : Quản lý chương/mục",
        Order = 2,
        Icon = "chaptr",
        IShow = false,
        Type = MenuType.Mod
    )]
    public class ModChaptersController : AdminController
    {
        private readonly ModChapterService _service;
        private readonly CPUserService _userService;
        private readonly ModCourseService _courseService;

        public ModChaptersController(ModChapterService service, CPUserService userService, ModCourseService courseService)
        {
            _service = service;
            _userService = userService;
            _courseService = courseService;
        }

        //public IActionResult Create(ModChapterModel model)
        //{
        //    ViewBag.Title = "Thêm mới";
        //    string userID = _currentUser.ID;
        //    if (string.IsNullOrEmpty(model.CourseID))
        //    {
        //        return RedirectToAction("Index", "ModCourses");
        //    }

        //    var course = _courseService.GetByID(model.CourseID);
        //    if (course == null)
        //    {
        //        return RedirectToAction("Index", "ModCourses");
        //    }

        //    ViewBag.Course = course;

        //    var chapters = _service.Find(true, o => o.CourseID == model.CourseID);
        //    ViewBag.Chapters = chapters.Select(o => new ModChapterViewModel(o)
        //    {
        //        Parent = (!string.IsNullOrEmpty(o.ParentID) && o.ParentID != "0")
        //            ? chapters.FirstOrDefault(t => t.ID == o.ParentID)
        //            : new ModChapterEntity { Order = o.Order }
        //    }).OrderBy(o => o.Parent.Order).ThenBy(o => o.Order).ToList();

        //    if (!string.IsNullOrEmpty(model.ID))
        //    {
        //        return RedirectToAction("Edit", new { model.ID, model.CourseID });
        //    }

        //    ViewBag.Model = model;
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(ModChapterModel model, ModChapterEntity item)
        //{
        //    ViewBag.Title = "Thêm mới";
        //    ViewData.Model = model;
        //    if (string.IsNullOrEmpty(model.CourseID))
        //    {
        //        return RedirectToAction("Index", "ModCourses");
        //    }
        //    var course = _courseService.GetByID(model.CourseID);
        //    if (course == null)
        //    {
        //        return RedirectToAction("Index", "ModCourses");
        //    }
        //    ViewBag.Course = course;

        //    var chapters = _service.Find(true, o => o.CourseID == model.CourseID);
        //    ViewBag.Chapters = chapters.Select(o => new ModChapterViewModel(o)
        //    {
        //        Parent = (!string.IsNullOrEmpty(o.ParentID) && o.ParentID != "0")
        //            ? chapters.FirstOrDefault(t => t.ID == o.ParentID)
        //            : new ModChapterEntity { Order = o.Order }
        //    }).OrderBy(o => o.Parent.Order).ThenBy(o => o.Order).ToList();

        //    if (!string.IsNullOrEmpty(model.ID))
        //    {
        //        return RedirectToAction("Edit", new { model.ID, model.CourseID });
        //    }

        //    if (!string.IsNullOrEmpty(model.ID) || !string.IsNullOrEmpty(item.ID))
        //    {
        //        return RedirectToAction("Edit", new { model.ID, model.CourseID });
        //    }
        //    else
        //    {
        //        if (string.IsNullOrEmpty(item.Name))
        //        {
        //            SetMessageWarning("Bạn chưa điền tên của mục");
        //            return View();
        //        }
        //        else
        //        {
        //            item.Code = UnicodeName.ConvertUnicodeToCode(item.Name, "-", true);
        //            item.Created = DateTime.Now;
        //            item.Updated = DateTime.Now;
        //            item.CreateUser = _currentUser.ID;
        //            item.Order = chapters.Count() + 1;
        //            if (!string.IsNullOrEmpty(item.ParentID) && item.ParentID != "0")
        //                item.ParentType = PARENT_TYPE_CODE.CHAPTER;
        //            else
        //                item.ParentType = PARENT_TYPE_CODE.COURSE;
        //            await _service.AddAsync(item);
        //            SetMessageSuccess("Thêm mới thành công");
        //            return RedirectToAction("Detail", "ModCourses", new { ID = model.CourseID });
        //        }
        //    }

        //    return View();
        //}

        //public IActionResult Edit(ModChapterModel model)
        //{
        //    ViewBag.Title = "Cập nhật";
        //    string userID = _currentUser.ID;

        //    var item = new ModChapterEntity();
        //    if (string.IsNullOrEmpty(model.ID))
        //    {
        //        if (!string.IsNullOrEmpty(model.CourseID) && model.CourseID != "0")
        //            return RedirectToAction("Detail", "ModCourses", new { ID = model.CourseID });
        //        else
        //            return RedirectToAction("Index", "ModCourses");
        //    }
        //    else
        //    {
        //        item = _service.GetByID(model.ID);
        //        model.CourseID = item.CourseID;
        //        if (item == null)
        //        {
        //            SetMessageError("Không tìm thấy chương");
        //            return RedirectToAction("Detail", "ModCourses", new { ID = item.CourseID });
        //        }
        //        ViewBag.Data = item;
        //    }

        //    var course = _courseService.GetByID(model.CourseID);
        //    if (course == null)
        //    {
        //        return RedirectToAction("Index", "ModCourses");
        //    }

        //    ViewBag.Course = course;

        //    var chapters = _service.Find(true, o => o.CourseID == model.CourseID);
        //    ViewBag.Chapters = chapters.Select(o => new ModChapterViewModel(o)
        //    {
        //        Parent = (!string.IsNullOrEmpty(o.ParentID) && o.ParentID != "0")
        //            ? chapters.FirstOrDefault(t => t.ID == o.ParentID)
        //            : new ModChapterEntity { Order = o.Order }
        //    }).OrderBy(o => o.Parent.Order).ThenBy(o => o.Order).ToList();

        //    ViewBag.Model = model;
        //    ViewBag.CurrentChapter = item.ID;
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(ModChapterModel model, ModChapterEntity item)
        //{
        //    ViewBag.Title = "Cập nhật";
        //    if (string.IsNullOrEmpty(model.ID))
        //    {
        //        if (!string.IsNullOrEmpty(model.CourseID) && model.CourseID != "0")
        //            return RedirectToAction("Detail", "ModCourses", new { ID = model.CourseID });
        //        else
        //            return RedirectToAction("Index", "ModCourses");
        //    }
        //    else
        //    {
        //        var _item = _service.GetByID(model.ID);
        //        if (_item == null)
        //        {
        //            SetMessageError("Không tìm thấy chương");
        //            return RedirectToAction("Detail", "ModCourses", new { ID = item.CourseID });
        //        }
        //        if (!string.IsNullOrEmpty(item.Name))
        //        {
        //            _item.Name = item.Name;
        //        }
        //        _item.Updated = DateTime.Now;
        //        _item.ParentID = item.ParentID;
        //        await _service.AddAsync(_item);
        //        SetMessageSuccess("Cập nhật thành công");
        //        return RedirectToAction("Detail", "ModChapters", new { ID = item.ID });
        //    }
        //}

        //public IActionResult Detail(ModChapterModel model)
        //{
        //    ViewBag.Title = "Nội dung chương";

        //    ViewData.Model = model;
        //    var item = new ModChapterEntity();

        //    if (string.IsNullOrEmpty(model.ID))
        //    {
        //        if (!string.IsNullOrEmpty(model.CourseID) && model.CourseID != "0")
        //            return RedirectToAction("Detail", "ModCourses", new { ID = model.CourseID });
        //        else
        //            return RedirectToAction("Index", "ModCourses");
        //    }
        //    else
        //    {
        //        item = _service.GetByID(model.ID);
        //        model.CourseID = item.CourseID;
        //        if (item == null)
        //        {
        //            SetMessageError("Không tìm thấy chương");
        //            return RedirectToAction("Detail", "ModCourses", new { ID = item.CourseID });
        //        }
        //        ViewBag.Data = item;
        //    }

        //    var course = _courseService.GetByID(item.CourseID);
        //    if (course == null)
        //    {
        //        return RedirectToAction("Index", "ModCourses");
        //    }
        //    ViewBag.Course = course;

        //    var chapters = _service.Find(true, o => o.CourseID == item.CourseID);
        //    ViewBag.Chapters = chapters.Select(o => new ModChapterViewModel(o)
        //    {
        //        Parent = (!string.IsNullOrEmpty(o.ParentID) && o.ParentID != "0")
        //            ? chapters.FirstOrDefault(t => t.ID == o.ParentID)
        //            : new ModChapterEntity { Order = o.Order }
        //    }).OrderBy(o => o.Parent.Order).ThenBy(o => o.Order).ToList();

        //    ViewBag.CurrentChapter = item.ID;

        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> Delete(DefaultModel model)
        //{
        //    int delete = 0;
        //    if (string.IsNullOrEmpty(model.ArrID))
        //    {
        //        SetMessageError("Dữ liệu trống");
        //    }
        //    else
        //    {
        //        return null;
        //    }

        //    return null;
        //}

        //[HttpPost]
        //public void Export(DefaultModel model)
        //{
        //    DateTime startDate = model.StartDate > DateTime.MinValue ? new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0) : DateTime.MinValue;
        //    DateTime endDate = model.EndDate > DateTime.MinValue ? new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59) : DateTime.MinValue;

        //    var data = _service.CreateQuery().FindList(!string.IsNullOrEmpty(model.SearchText), o => o.Name.Contains(model.SearchText) || o.Code.Contains(model.SearchText))
        //        .Where(!string.IsNullOrEmpty(model.ID), o => o.ID == model.ID)
        //        .Where(startDate > DateTime.MinValue, o => o.Created >= startDate)
        //        .Where(endDate > DateTime.MinValue, o => o.Created <= endDate)
        //        .OrderByDescending(o => o.ID)
        //        .ToList();

        //    DataTable dt = data.ToDataTable();

        //    Response.Clear();
        //    Response.Headers["content-disposition"] = "attachment;filename=DanhMucCapDo.xls";
        //    Response.ContentType = "application/excel";

        //    string html = Query.ConvertDataTableToHTML(dt);
        //    HtmlString htmlTextWriter = new HtmlString(html);

        //    Response.WriteAsync(html);
        //}

        //[HttpGet]
        //public void Export()
        //{

        //    var data = _service.GetAll()
        //         .OrderByDescending(o => o.ID)
        //         .ToList();

        //    DataTable dt = data.ToDataTable();

        //    Response.Clear();
        //    Response.Headers["content-disposition"] = "attachment;filename=Catalog.xls";
        //    Response.ContentType = "application/excel";

        //    string html = Query.ConvertDataTableToHTML(dt);
        //    HtmlString htmlTextWriter = new HtmlString(html);

        //    Response.WriteAsync(html);
        //}

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
        //    SetMessageSuccess("Cập nhật thành công");
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
        //    SetMessageSuccess("Cập nhật thành công");
        //    return RedirectToAction("Index");
        //}

        //public IActionResult Clear()
        //{
        //    _service.ClearCache();
        //    return RedirectToAction("Index");
        //}
    }
}
