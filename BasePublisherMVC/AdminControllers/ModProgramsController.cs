using BasePublisherMVC.Globals;
using BasePublisherMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BasePublisherModels.Database;
using CoreMongoDB.Repositories;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;

namespace BasePublisherMVC.AdminControllers
{
    [MenuControl(
        CModule = "ModPrograms",
        Name = "MO : Quản lý giáo trình",
        Order = 3,
        Icon = "card_travel",
        IShow = true,
        Type = MenuType.Mod
    )]
    public class ModProgramsController : AdminController
    {
        private readonly ModProgramService _service;
        private readonly ModSubjectService _subjectService;
        private readonly ModGradeService _gradeService;
        private readonly ModCourseService _courseService;

        public ModProgramsController(ModProgramService service, ModSubjectService subjectService, ModGradeService gradeService, ModCourseService courseService)
        {
            _service = service;
            _subjectService = subjectService;
            _gradeService = gradeService;
            _courseService = courseService;
        }

        public ActionResult Index(ModProgramModel model)
        {
            var data = _service
                .Find(!string.IsNullOrEmpty(model.SearchText), o => (o.Name.Contains(model.SearchText) || o.Code.Contains(model.SearchText)))
                .Where(o => o.CreateUser == _currentUser.ID)
                .Where(!String.IsNullOrEmpty(model.Subject), o => o.Subjects.IndexOf(model.Subject) >= 0)
                .Where(!String.IsNullOrEmpty(model.Grade), o => o.Grades.IndexOf(model.Grade) >= 0)
                //.Where(string.IsNullOrEmpty(model.Record), o => (o.ParentID.Equals("0") || string.IsNullOrEmpty(o.ParentID)))
                .OrderBy(o => o.Name)
                .ToList();
            ViewBag.Data = data.ToList();

            var subjectdata = _subjectService.Find(true, o => o.IsActive).OrderBy(o => o.Name).ToList();
            ViewBag.SubjectData = subjectdata;

            var gradedata = _gradeService.Find(true, o => o.IsActive).OrderBy(o => o.Name).ToList();
            ViewBag.GradeData = gradedata;

            model.TotalRecord = data.Count;
            ViewBag.Model = model;
            return View();
        }

        public IActionResult Create(DefaultModel model)
        {
            ViewBag.Title = "Thêm mới";
            string userID = _currentUser.ID;
            var subjectdata = _subjectService.Find(true, o => o.IsActive).OrderBy(o => o.Name).ToList();
            ViewBag.SubjectData = subjectdata;

            var gradedata = _gradeService.Find(true, o => o.IsActive).OrderBy(o => o.Name).ToList();
            ViewBag.GradeData = gradedata;

            if (!string.IsNullOrEmpty(model.ID))
            {
                return RedirectToAction("Edit", new { model.ID });
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DefaultModel model, ModProgramEntity item)
        {
            ViewBag.Title = "Thêm mới";

            var subjectdata = _subjectService.Find(true, o => o.IsActive).OrderBy(o => o.Name).ToList();
            ViewBag.SubjectData = subjectdata;

            var gradedata = _gradeService.Find(true, o => o.IsActive).OrderBy(o => o.Name).ToList();
            ViewBag.GradeData = gradedata;

            if (!string.IsNullOrEmpty(model.ID) || !string.IsNullOrEmpty(item.ID))
            {
                return RedirectToAction("Edit", new { model.ID });
            }
            else
            {
                if (string.IsNullOrEmpty(item.Name))
                {
                    SetMessageWarning("Bạn chưa điền tên giáo trình");
                    return View();
                }
                else
                {
                    item.Code = UnicodeName.ConvertUnicodeToCode(item.Name, "-", true);
                    item.Created = DateTime.Now;
                    item.Updated = DateTime.Now;
                    item.IsAdmin = true;
                    item.CreateUser = _currentUser.ID;
                    item.Grades.RemoveAll(o => o == null);
                    item.Subjects.RemoveAll(o => o == null);
                    if (_service.GetItemByCode(item.Code) == null)
                    {
                        await _service.AddAsync(item);
                        SetMessageSuccess("Thêm mới thành công");
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        SetMessageWarning("Giáo trình đã tồn tại");
                        return View();
                    }

                }
            }
        }

        public IActionResult Edit(string ID)
        {
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
                var subjectdata = _subjectService.Find(true, o => o.IsActive).OrderBy(o => o.Name).ToList();
                ViewBag.SubjectData = subjectdata;

                var gradedata = _gradeService.Find(true, o => o.IsActive).OrderBy(o => o.Name).ToList();
                ViewBag.GradeData = gradedata;

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
        public async Task<IActionResult> Edit(DefaultModel model, ModProgramEntity item)
        {
            ViewBag.Title = "Cập nhật thông tin";
            var subjectdata = _subjectService.Find(true, o => o.IsActive).OrderBy(o => o.Name).ToList();
            ViewBag.SubjectData = subjectdata;

            var gradedata = _gradeService.Find(true, o => o.IsActive).OrderBy(o => o.Name).ToList();
            ViewBag.GradeData = gradedata;
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
                }

                _item.Description = item.Description;
                _item.Updated = DateTime.Now;
                item.Grades.RemoveAll(o => o == null);
                item.Subjects.RemoveAll(o => o == null);

                _item.Grades = item.Grades;
                _item.Subjects = item.Subjects;
                //TODO: tính toán xem có cần cập nhật lại code ko
                _item.IsActive = item.IsActive;
                await _service.AddAsync(_item);
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
                        if (_courseService.Find(true, o => o.ProgramID == item.ID).Any())
                        {
                            SetMessageWarning("Đang có khóa học thuộc giáo trình này, không xóa được");
                            return RedirectToAction("Index");
                        }
                        await _service.RemoveAsync(item.ID);
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
            Response.Headers["content-disposition"] = "attachment;filename=DanhMucGiaoTrinh.xls";
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
