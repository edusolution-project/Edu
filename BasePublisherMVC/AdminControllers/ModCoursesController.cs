using BasePublisherMVC.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasePublisherModels.Database;
using BasePublisherMVC.Models;
using BasePublisherMVC.ViewModel;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace BasePublisherMVC.AdminControllers
{
    [MenuControl(
        CModule = "ModCourses",
        Name = "MO : Quản lý khóa học",
        Order = 4,
        IShow = true,
        Type = MenuType.Mod
    )]
    public class ModCoursesController : AdminController
    {
        private readonly ModCourseService _service;
        private readonly ModSubjectService _subjectService;
        private readonly ModGradeService _gradeService;
        private readonly ModProgramService _programService;
        private readonly ModChapterService _chapterService;

        public ModCoursesController(ModCourseService service, ModProgramService programService, ModSubjectService subjectService, ModGradeService gradeService, ModChapterService chapterService)
        {
            _service = service;
            _subjectService = subjectService;
            _gradeService = gradeService;
            _programService = programService;
            _chapterService = chapterService;
        }

        public ActionResult Index(ModCourseModel model)
        {
            if (string.IsNullOrEmpty(model.ID)||model.ID.Equals("0")) {
                var programdata = _programService.Find(true, o => o.CreateUser == _currentUser.ID)
                .Where(!string.IsNullOrEmpty(model.Subject), o => o.Subjects.IndexOf(model.Subject) >= 0)
                .Where(!string.IsNullOrEmpty(model.Grade), o => o.Grades.IndexOf(model.Grade) >= 0)
                .OrderBy(o => o.Name).ToList();
                ViewBag.ProgramData = programdata;

                if (!string.IsNullOrEmpty(model.Program))
                {
                    if (programdata.Count(o => o.ID == model.Program) == 0)
                        model.Program = null;
                }

                var result = new List<ModCourseViewModel>();
                var data = _service
                    .Find(!string.IsNullOrEmpty(model.SearchText), o => (o.Name.Contains(model.SearchText) || o.Code.Contains(model.SearchText)))
                    .Where(o => o.CreateUser == _currentUser.ID)
                    .Where(!string.IsNullOrEmpty(model.Program), o => o.ProgramID == model.Program)
                    .Where(!string.IsNullOrEmpty(model.Subject), o => o.SubjectID == model.Subject)
                    .Where(!string.IsNullOrEmpty(model.Grade), o => o.GradeID == model.Grade)
                    .OrderBy(o => o.Name)
                    .ToList();

                var subjectdata = _subjectService.Find(true, o => o.IsActive).OrderBy(o => o.Name).ToList();
                ViewBag.SubjectData = subjectdata;

                var gradedata = _gradeService.Find(true, o => o.IsActive).OrderBy(o => o.Name).ToList();
                ViewBag.GradeData = gradedata;

                result.AddRange(data.Select(t => new ModCourseViewModel(t)
                {
                    Program = programdata.Find(o => o.ID == t.ProgramID),
                    Grade = gradedata.Find(o => o.ID == t.GradeID),
                    Subject = subjectdata.Find(o => o.ID == t.SubjectID),
                    ChildNodeCount = 0//TODO: add chapter count here
                }).ToList()
                );

                ViewBag.Data = result.ToList();

                model.TotalRecord = result.Count;
            }
            else
            {
                var data = _service.CreateQuery().Find(o => o.ID == model.ID && o.CreateUser == _currentUser.ID).SingleOrDefault();
                ViewBag.Data = data;
            }
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

            var programdata = _programService.Find(true, o => o.CreateUser == _currentUser.ID).OrderBy(o => o.Name).ToList();
            ViewBag.ProgramData = programdata;

            if (!string.IsNullOrEmpty(model.ID))
            {
                return RedirectToAction("Edit", new { model.ID });
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DefaultModel model, ModCourseEntity item)
        {
            ViewBag.Title = "Thêm mới";

            var subjectdata = _subjectService.Find(true, o => o.IsActive).OrderBy(o => o.Name).ToList();
            ViewBag.SubjectData = subjectdata;

            var gradedata = _gradeService.Find(true, o => o.IsActive).OrderBy(o => o.Name).ToList();
            ViewBag.GradeData = gradedata;

            var programdata = _programService.Find(true, o => o.CreateUser == _currentUser.ID).OrderBy(o => o.Name).ToList();
            ViewBag.ProgramData = programdata;

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
                    if (_service.GetItemByCode(item.Code) == null)
                    {
                        await _service.AddAsync(item);
                        SetMessageSuccess("Thêm mới thành công");
                        return RedirectToAction("index");
                    }
                    else
                    {
                        SetMessageWarning("Khóa học đã tồn tại");
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

                var programdata = _programService.Find(true, o => o.CreateUser == _currentUser.ID).OrderBy(o => o.Name).ToList();
                ViewBag.ProgramData = programdata;

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
        public async Task<IActionResult> Edit(DefaultModel model, ModCourseEntity item)
        {
            ViewBag.Title = "Cập nhật thông tin";
            var subjectdata = _subjectService.Find(true, o => o.IsActive).OrderBy(o => o.Name).ToList();
            ViewBag.SubjectData = subjectdata;

            var gradedata = _gradeService.Find(true, o => o.IsActive).OrderBy(o => o.Name).ToList();
            ViewBag.GradeData = gradedata;

            var programdata = _programService.Find(true, o => o.CreateUser == _currentUser.ID).OrderBy(o => o.Name).ToList();
            ViewBag.ProgramData = programdata;

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
                _item.GradeID = item.GradeID;
                _item.SubjectID = item.SubjectID;
                _item.ProgramID = item.ProgramID;

                //TODO: tính toán xem có cần cập nhật lại code ko
                _item.IsActive = item.IsActive;
                await _service.AddAsync(_item);
                ViewBag.Data = _service.GetByID(ID);
                SetMessageSuccess("Cập nhật thành công");
            }
            ViewBag.Model = model;
            return RedirectToAction("index");
        }

        public IActionResult Detail(string ID)
        {
            DefaultModel model = new DefaultModel
            {
                ID = ID
            };
            ViewBag.Title = "";
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
                ViewBag.Title = item.Name;
                ViewBag.Course = item;
                ViewBag.Data = new ModCourseViewModel(item)
                {
                    Program = _programService.GetByID(item.ProgramID),
                    Grade = _gradeService.GetByID(item.GradeID),
                    Subject = _subjectService.GetByID(item.SubjectID)
                };
                var chapters = _chapterService.Find(true, o => o.CourseID == item.ID);
                ViewBag.Chapters = chapters.Select(o => new ModChapterViewModel(o)
                {
                    Parent = (!string.IsNullOrEmpty(o.ParentID) && o.ParentID != "0")
                        ? chapters.FirstOrDefault(t => t.ID == o.ParentID)
                        : new ModChapterEntity {Order = o.Order}
                }).OrderBy(o => o.Parent.Order).ThenBy(o => o.Order).ToList();
            }
            return View();
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
            return RedirectToAction("Index");
        }

    }
}
