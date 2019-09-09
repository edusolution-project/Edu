using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using MongoDB.Driver;
using System.Text;
using System.Linq;
using Core_v2.Globals;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class ClassController : TeacherController
    {
        private readonly GradeService _gradeService;
        private readonly SubjectService _subjectService;
        private readonly TeacherService _teacherService;
        private readonly ClassService _service;
        private readonly CourseService _courseService;
        private readonly ChapterService _chapterService;
        private readonly ChapterExtendService _chapterExtendService;
        private readonly LessonService _lessonService;
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly StudentService _studentService;
        private readonly MappingEntity<StudentEntity, ClassMemberViewModel> _mapping;
        private readonly MappingEntity<ClassEntity, ClassActiveViewModel> _activeMapping;
        private readonly IHostingEnvironment _env;

        private readonly FileProcess _fileProcess;

        public ClassController(GradeService gradeservice
           , SubjectService subjectService, TeacherService teacherService, ClassService service,
            CourseService courseService, ChapterService chapterService, ChapterExtendService chapterExtendService, LessonService lessonService, LessonScheduleService lessonScheduleService,
            StudentService studentService, IHostingEnvironment evn,
            FileProcess fileProcess)
        {
            _gradeService = gradeservice;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _courseService = courseService;
            _service = service;
            _chapterService = chapterService;
            _chapterExtendService = chapterExtendService;
            _lessonService = lessonService;
            _lessonScheduleService = lessonScheduleService;
            _studentService = studentService;
            _mapping = new MappingEntity<StudentEntity, ClassMemberViewModel>();
            _activeMapping = new MappingEntity<ClassEntity, ClassActiveViewModel>();
            _env = evn;
            _fileProcess = fileProcess;
        }

        public IActionResult Index(DefaultModel model)
        {
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            var teacher = _teacherService.CreateQuery().Find(t => t.ID == UserID).SingleOrDefault();

            var subject = new List<SubjectEntity>();
            var grade = new List<GradeEntity>();

            if (teacher != null && teacher.Subjects != null)
            {
                subject = _subjectService.CreateQuery().Find(t => teacher.Subjects.Contains(t.ID)).ToList();
                grade = _gradeService.CreateQuery().Find(t => teacher.Subjects.Contains(t.SubjectID)).ToList();
            }
            ViewBag.Grade = grade;
            ViewBag.Subject = subject;

            ViewBag.User = UserID;
            ViewBag.Model = model;
            return View();
        }

        public IActionResult Edit(DefaultModel model)
        {
            if (model == null) return null;
            var currentClass = _service.GetItemByID(model.ID);
            if (currentClass == null)
                return RedirectToAction("Index");
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            if (currentClass.TeacherID != UserID)
                return RedirectToAction("Index");
            var subject = _subjectService.GetItemByID(currentClass.SubjectID);
            ViewBag.Class = currentClass;
            ViewBag.Subject = subject;
            return View();
        }


        public IActionResult Detail(DefaultModel model)
        {
            if (model == null) return null;
            var currentClass = _service.GetItemByID(model.ID);
            if (currentClass == null)
                return RedirectToAction("Index");
            ViewBag.Class = currentClass;
            return View();
        }

        public IActionResult Modules(DefaultModel model)
        {
            if (model == null) return null;
            var currentClass = _service.GetItemByID(model.ID);
            if (currentClass == null)
                return RedirectToAction("Index");
            ViewBag.Class = currentClass;
            return View();
        }

        public IActionResult Assignments(DefaultModel model)
        {
            if (model == null) return null;
            var currentClass = _service.GetItemByID(model.ID);
            if (currentClass == null)
                return RedirectToAction("Index");
            ViewBag.Class = currentClass;
            return View();
        }

        public IActionResult Discussions(DefaultModel model)
        {
            if (model == null) return null;
            var currentClass = _service.GetItemByID(model.ID);
            if (currentClass == null)
                return RedirectToAction("Index");
            ViewBag.Class = currentClass;
            return View();
        }

        public IActionResult Announcements(DefaultModel model)
        {
            if (model == null) return null;
            var currentClass = _service.GetItemByID(model.ID);
            if (currentClass == null)
                return RedirectToAction("Index");
            ViewBag.Class = currentClass;
            return View();
        }

        public IActionResult Members(DefaultModel model)
        {
            if (model == null) return null;
            var currentClass = _service.GetItemByID(model.ID);
            if (currentClass == null)
                return RedirectToAction("Index");
            ViewBag.Class = currentClass;
            return View();
        }

        [HttpPost]
        public JsonResult GetMainChapters(string ID)
        {
            try
            {
                var currentClass = _service.GetItemByID(ID);
                if (currentClass == null)
                    return new JsonResult(new Dictionary<string, object>
                    {
                        {"Error", "Không tìm thấy lớp học" }
                    });

                var chapters = _chapterService.CreateQuery().Find(c => c.CourseID == currentClass.CourseID && c.ParentID == "0").ToList();
                var chapterExtends = _chapterExtendService.Search(currentClass.ID);

                foreach (var chapter in chapters)
                {
                    var extend = chapterExtends.SingleOrDefault(t => t.ChapterID == chapter.ID);
                    if (extend != null) chapter.Description = extend.Description;
                }
                var response = new Dictionary<string, object>
                {
                    { "Data", chapters }
                };

                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", null },
                    {"Error", ex.Message }
                });
            }
        }

        [HttpPost]
        public JsonResult UpdateChapterInfo(string ClassID, ChapterEntity chapter)
        {
            try
            {
                var currentClass = _service.GetItemByID(ClassID);
                if (currentClass == null)
                    return new JsonResult(new Dictionary<string, object>
                    {
                        {"Error", "Không tìm thấy lớp học" }
                    });

                var currentChapter = _chapterService.CreateQuery().Find(c => c.ID == chapter.ID).FirstOrDefault();
                if (currentChapter == null)
                    return new JsonResult(new Dictionary<string, object>
                    {
                        {"Error", "Không tìm thấy chương" }
                    });

                var chapterExtend = _chapterExtendService.Search(ClassID, chapter.ID).SingleOrDefault();
                if (chapterExtend == null)
                {
                    chapterExtend = new ChapterExtendEntity
                    {
                        ChapterID = chapter.ID,
                        ClassID = ClassID
                    };
                }
                chapterExtend.Description = chapter.Description;
                _chapterExtendService.CreateOrUpdate(chapterExtend);

                currentChapter.Description = chapter.Description;

                var response = new Dictionary<string, object>
                {
                    { "Data", currentChapter }
                };

                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", null },
                    {"Error", ex.Message }
                });
            }
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetSchedules(DefaultModel model)
        {
            TeacherEntity teacher = null;
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            if (!string.IsNullOrEmpty(UserID) && UserID != "0")
            {
                teacher = UserID == "0" ? null : _teacherService.GetItemByID(UserID);
                if (teacher == null)
                {
                    return new JsonResult(new Dictionary<string, object> {
                        {"Error", "Không có thông tin giảng viên" }
                    });
                }
            }
            if (string.IsNullOrEmpty(model.ID))
            {
                return new JsonResult(new Dictionary<string, object> {
                        {"Error","Không có thông tin lớp học" },
                });
            }

            var currentClass = _service.GetItemByID(model.ID);

            if (currentClass == null)
            {
                return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Không có thông tin lớp học" }
                    });
            }

            var course = _courseService.GetItemByID(currentClass.CourseID);

            if (course == null)
            {
                return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Không có thông tin giáo trình" }
                    });
            }
            var _scheduleMapping = new MappingEntity<LessonEntity, LessonScheduleViewModel>();
            var classSchedule = new ClassScheduleViewModel(course)
            {
                Chapters = _chapterService.CreateQuery().Find(o => o.CourseID == course.ID).SortBy(o => o.ParentID).ThenBy(o => o.Order).ThenBy(o => o.ID).ToList(),
                Lessons = (from r in _lessonService.CreateQuery().Find(o => o.CourseID == course.ID).SortBy(o => o.ChapterID).ThenBy(o => o.Order).ThenBy(o => o.ID).ToList()
                           let schedule = _lessonScheduleService.CreateQuery().Find(o => o.LessonID == r.ID && o.ClassID == model.ID).FirstOrDefault()
                           where schedule != null
                           select _scheduleMapping.AutoOrtherType(r, new LessonScheduleViewModel()
                           {
                               ScheduleID = schedule.ID,
                               StartDate = schedule.StartDate,
                               EndDate = schedule.EndDate,
                               IsActive = schedule.IsActive
                           })).ToList()
            };

            var response = new Dictionary<string, object>
            {
                { "Data", classSchedule },
                { "Model", model }
            };
            return new JsonResult(response);
        }
        [Obsolete]
        [HttpPost]
        public JsonResult GetList(DefaultModel model, string SubjectID = "", string GradeID = "", string UserID = "")
        {
            var filter = new List<FilterDefinition<ClassEntity>>();
            TeacherEntity teacher = null;
            if (string.IsNullOrEmpty(UserID))
                UserID = User.Claims.GetClaimByType("UserID").Value;
            if (!string.IsNullOrEmpty(UserID) && UserID != "0")
            {
                teacher = UserID == "0" ? null : _teacherService.GetItemByID(UserID);
                if (teacher == null)
                {
                    return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Không có thông tin giảng viên" }
                    });
                }
            }
            if (teacher != null)
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.TeacherID == UserID));

            if (!string.IsNullOrEmpty(model.SearchText))
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Name.ToLower().Contains(model.SearchText.ToLower())));
            }
            if (!string.IsNullOrEmpty(SubjectID))
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.SubjectID == SubjectID));
            }
            if (!string.IsNullOrEmpty(GradeID))
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.GradeID == GradeID));
            }

            var data = filter.Count > 0 ? _service.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _service.GetAll();
            model.TotalRecord = data.Count();
            var DataResponse = data == null || data.Count() <= 0 || data.Count() < model.PageSize
                ? data
                : data.Skip((model.PageIndex - 1) * model.PageSize).Limit(model.PageSize);

            var response = new Dictionary<string, object>
            {
                { "Data", DataResponse.ToList().Select(o=> new ClassViewModel(o){
                        CourseName = _courseService.GetItemByID(o.CourseID)?.Name,
                        GradeName = _gradeService.GetItemByID(o.GradeID)?.Name,
                        SubjectName = _subjectService.GetItemByID(o.SubjectID).Name,
                        TeacherName = _teacherService.GetItemByID(o.TeacherID).FullName
                    })
                },
                { "Model", model }
            };
            return new JsonResult(response);
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetDetailsLesson(DefaultModel model, string SubjectID = "", string GradeID = "", string UserID = "")
        {
            var filter = new List<FilterDefinition<ClassEntity>>();
            TeacherEntity teacher = null;
            if (string.IsNullOrEmpty(UserID))
                UserID = User.Claims.GetClaimByType("UserID").Value;
            if (!string.IsNullOrEmpty(UserID) && UserID != "0")
            {
                teacher = UserID == "0" ? null : _teacherService.GetItemByID(UserID);
                if (teacher == null)
                {
                    return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Không có thông tin giảng viên" }
                    });
                }
            }
            if (teacher != null)
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.TeacherID == UserID));

            if (!string.IsNullOrEmpty(model.SearchText))
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Name.ToLower().Contains(model.SearchText.ToLower())));
            }
            if (!string.IsNullOrEmpty(SubjectID))
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.SubjectID == SubjectID));
            }
            if (!string.IsNullOrEmpty(GradeID))
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.GradeID == GradeID));
            }

            var data = filter.Count > 0 ? _service.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _service.GetAll();
            model.TotalRecord = data.Count();
            var DataResponse = data == null || data.Count() <= 0 || data.Count() < model.PageSize
                ? data
                : data.Skip((model.PageIndex - 1) * model.PageSize).Limit(model.PageSize);

            var response = new Dictionary<string, object>
            {
                { "Data", DataResponse.ToList().Select(o=> new ClassViewModel(o){
                        CourseName = _courseService.GetItemByID(o.CourseID)?.Name,
                        GradeName = _gradeService.GetItemByID(o.GradeID)?.Name,
                        SubjectName = _subjectService.GetItemByID(o.SubjectID).Name,
                        TeacherName = _teacherService.GetItemByID(o.TeacherID).FullName
                    })
                },
                { "Model", model }
            };
            return new JsonResult(response);
        }

        public JsonResult GetListMember(DefaultModel model)
        {
            if (string.IsNullOrEmpty(model.ID))
                return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Không tìm thấy thông tin lớp" }
                    });
            var currentClass = _service.GetItemByID(model.ID);
            if (currentClass == null)
                return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Không tìm thấy thông tin lớp" }
                    });
            var teacher = _teacherService.GetItemByID(currentClass.TeacherID);

            var filter = new List<FilterDefinition<StudentEntity>>();
            filter.Add(Builders<StudentEntity>.Filter.Where(o => currentClass.Students.Contains(o.ID)));
            var students = filter.Count > 0 ? _studentService.Collection.Find(Builders<StudentEntity>.Filter.And(filter)) : _studentService.GetAll();
            var studentsView = students.ToList().Select(t => _mapping.AutoOrtherType(t, new ClassMemberViewModel()
            {
                ClassName = currentClass.Name,
                ClassStatus = "Đang học",
                LastJoinDate = DateTime.Now
            })).ToList();

            var response = new Dictionary<string, object>
            {
                { "Data",new Dictionary<string, object> {
                        {"Teacher",teacher },
                        {"Students",studentsView }
                    }
                },
                { "Model", model }
            };
            return new JsonResult(response);
        }

        public JsonResult GetActiveList()
        {
            var filter = new List<FilterDefinition<ClassEntity>>();
            TeacherEntity teacher = null;
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            if (!string.IsNullOrEmpty(UserID) && UserID != "0")
            {
                teacher = UserID == "0" ? null : _teacherService.GetItemByID(UserID);
                if (teacher == null)
                {
                    return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",UserID },
                        {"Msg","Không có thông tin giảng viên" }
                    });
                }
            }
            if (teacher != null)
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.TeacherID == UserID && o.EndDate >= DateTime.Now.ToLocalTime().Date));

            var data = filter.Count > 0 ? _service.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _service.GetAll();
            var DataResponse = data;
            if (data != null)
                DataResponse = data.Limit(3);

            var response = new Dictionary<string, object>
            {
                { "Data", DataResponse.ToList().Select(t=> _activeMapping.AutoOrtherType(t, new ClassActiveViewModel(){
                    Progress = (int)((DateTime.Now.ToLocalTime().Date - t.StartDate.ToLocalTime().Date).TotalDays  * 100 / (t.EndDate.ToLocalTime().Date - t.StartDate.ToLocalTime().Date).TotalDays),
                    SubjectName = _subjectService.GetItemByID(t.SubjectID).Name
                    }))
                    }
            };
            return new JsonResult(response);
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<JsonResult> SaveInfo(ClassEntity entity)
        {

            if (String.IsNullOrEmpty(entity.ID))
            {
                new JsonResult(
                    new Dictionary<string, object>
                    {
                        { "Error", "Không có thông tin lớp"}
                    });
            }

            var currentClass = _service.GetItemByID(entity.ID);
            if (currentClass == null)
                if (String.IsNullOrEmpty(entity.ID))
                {
                    new JsonResult(
                        new Dictionary<string, object>
                        {
                        { "Error", "Không có thông tin lớp"}
                        });
                }

            try
            {
                var files = HttpContext.Request.Form != null && HttpContext.Request.Form.Files.Count > 0 ? HttpContext.Request.Form.Files : null;
                if (files != null && files.Count > 0)
                {
                    var file = files[0];

                    var filename = currentClass.ID + "_" + DateTime.Now.ToUniversalTime().ToString("yyyyMMddhhmmss") + Path.GetExtension(file.FileName);
                    currentClass.Image = await _fileProcess.SaveMediaAsync(file, filename);
                }
                currentClass.Description = entity.Description ?? "";
                currentClass.Syllabus = entity.Syllabus ?? "";
                currentClass.Modules = entity.Modules ?? "";
                currentClass.LearningOutcomes = entity.LearningOutcomes ?? "";
                currentClass.References = entity.References ?? "";
                _service.CreateOrUpdate(currentClass);

                return new JsonResult(
                    new Dictionary<string, object>
                    {
                        { "Data", currentClass }
                    }
                );
            }
            catch (Exception e)
            {
                return new JsonResult(
                        new Dictionary<string, object>
                        {
                            { "Error", e.Message}
                        });
            }
        }

        [HttpPost]
        [Obsolete]
        public async Task<JsonResult> ImportStudent(string ID)
        {
            var form = HttpContext.Request.Form;
            if (string.IsNullOrEmpty(ID)) return new JsonResult("Fail");
            var itemCourse = _service.GetItemByID(ID);
            if (itemCourse == null) return new JsonResult("Fail");
            if (itemCourse.Students == null) itemCourse.Students = new List<string>();
            if (form == null) return new JsonResult(null);
            if (form.Files == null || form.Files.Count <= 0) return new JsonResult(null);
            var file = form.Files[0];
            var filePath = Path.Combine(_env.WebRootPath, itemCourse.ID + "_" + file.FileName + DateTime.Now.ToString("ddMMyyyyhhmmss"));
            List<StudentEntity> studentList = null;
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
                stream.Close();
                try
                {
                    var readStream = new FileStream(filePath, FileMode.Open);
                    using (ExcelPackage package = new ExcelPackage(readStream))
                    {
                        ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                        int totalRows = workSheet.Dimension.Rows;
                        studentList = new List<StudentEntity>();
                        for (int i = 1; i <= totalRows; i++)
                        {
                            if (workSheet.Cells[i, 1].Value == null || workSheet.Cells[i, 1].Value.ToString() == "STT") continue;
                            var studentEmail = workSheet.Cells[i, 5].Value == null ? "" : workSheet.Cells[i, 5].Value.ToString();
                            var student = _studentService.CreateQuery().Find(o => o.Email == studentEmail).SingleOrDefault();
                            if (student != null)
                                studentList.Add(student);
                        }

                        var listID = studentList.Select(o => o.ID);
                        itemCourse.Students.AddRange(listID);
                        itemCourse.Students = itemCourse.Students.Distinct().ToList();
                        _service.CreateQuery().ReplaceOne(o => o.ID == itemCourse.ID, itemCourse);
                    }
                    System.IO.File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    return new JsonResult(ex);
                }
            }
            Dictionary<string, object> response = new Dictionary<string, object>()
            {
                {"Data",studentList}
            };
            return new JsonResult(response);
        }
    }
}
