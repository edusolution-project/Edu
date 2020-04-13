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
using Microsoft.AspNetCore.Http;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class ClassController : TeacherController
    {
        private readonly GradeService _gradeService;
        private readonly AccountService _accountService;
        private readonly SubjectService _subjectService;
        private readonly TeacherService _teacherService;
        private readonly ClassService _service;
        private readonly ClassStudentService _classStudentService;
        private readonly SkillService _skillService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly CourseService _courseService;
        private readonly CourseHelper _courseHelper;
        private readonly ClassProgressService _classProgressService;
        private readonly ClassSubjectProgressService _classSubjectProgressService;

        private readonly ChapterService _chapterService;
        private readonly ChapterExtendService _chapterExtendService;
        private readonly LessonService _lessonService;
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly StudentService _studentService;
        private readonly ScoreStudentService _scoreStudentService;
        private readonly LessonProgressService _lessonProgressService;
        private readonly LearningHistoryService _learningHistoryService;

        private readonly MappingEntity<StudentEntity, ClassStudentViewModel> _mapping;
        private readonly MappingEntity<ClassEntity, ClassActiveViewModel> _activeMapping;
        private readonly IHostingEnvironment _env;

        private readonly ChapterProgressService _chapterProgressService;

        //private readonly LessonPartService _lessonPartService;
        //private readonly LessonPartAnswerService _lessonPartAnswerService;
        //private readonly LessonPartQuestionService _lessonPartQuestionService;
        private readonly ExamService _examService;
        private readonly ExamDetailService _examDetailService;

        private readonly FileProcess _fileProcess;
        private readonly StudentHelper _studentHelper;
        private readonly LessonHelper _lessonHelper;
        private readonly MappingEntity<LessonEntity, StudentModuleViewModel> _moduleViewMapping;
        private readonly MappingEntity<LessonEntity, StudentAssignmentViewModel> _assignmentViewMapping;


        public ClassController(
            AccountService accountService,
            GradeService gradeservice,
            SubjectService subjectService,
            ClassSubjectService classSubjectService,
            ClassStudentService classStudentService,
            TeacherService teacherService,
            ClassService service,
            SkillService skillService,
            CourseService courseService,
            CourseHelper courseHelper,
            ClassProgressService classProgressService,
            ClassSubjectProgressService classSubjectProgressService,

            ChapterService chapterService,
            ChapterExtendService chapterExtendService,
            LessonService lessonService,
            LessonScheduleService lessonScheduleService,
            ExamService examService,
            ExamDetailService examDetailService,
            LearningHistoryService learningHistoryService,

            ScoreStudentService scoreStudentService,
            LessonProgressService lessonProgressService,

            StudentService studentService, IHostingEnvironment evn,

            FileProcess fileProcess,
            LessonHelper lessonHelper,
            StudentHelper studentHelper,

            ChapterProgressService chapterProgressService

            )
        {
            _accountService = accountService;
            _gradeService = gradeservice;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _courseService = courseService;
            _courseHelper = courseHelper;
            _service = service;
            _skillService = skillService;
            _classSubjectService = classSubjectService;
            _classStudentService = classStudentService;
            _classProgressService = classProgressService;
            _classSubjectProgressService = classSubjectProgressService;

            _chapterService = chapterService;
            _chapterExtendService = chapterExtendService;
            _lessonService = lessonService;
            _lessonScheduleService = lessonScheduleService;
            _lessonProgressService = lessonProgressService;

            _examService = examService;
            _examDetailService = examDetailService;
            _learningHistoryService = learningHistoryService;
            _scoreStudentService = scoreStudentService;

            _studentService = studentService;
            _mapping = new MappingEntity<StudentEntity, ClassStudentViewModel>();
            _activeMapping = new MappingEntity<ClassEntity, ClassActiveViewModel>();
            _env = evn;
            _fileProcess = fileProcess;

            _studentHelper = studentHelper;

            _lessonHelper = lessonHelper;

            _moduleViewMapping = new MappingEntity<LessonEntity, StudentModuleViewModel>();
            _assignmentViewMapping = new MappingEntity<LessonEntity, StudentAssignmentViewModel>();


            _chapterProgressService = chapterProgressService;
        }

        public IActionResult Index(DefaultModel model, int old = 0)
        {
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            var teacher = _teacherService.CreateQuery().Find(t => t.ID == UserID).SingleOrDefault();

            if (teacher != null && teacher.Subjects != null)
            {
                var subject = _subjectService.CreateQuery().Find(t => teacher.Subjects.Contains(t.ID)).ToList();
                var grade = _gradeService.CreateQuery().Find(t => teacher.Subjects.Contains(t.SubjectID)).ToList();
                ViewBag.Grades = grade;
                ViewBag.Subjects = subject;
                ViewBag.Skills = _skillService.GetList();
            }

            ViewBag.User = UserID;
            ViewBag.Model = model;
            ViewBag.Managable = CheckPermission(PERMISSION.COURSE_EDIT);
            if (old == 1)
                return View("Index_o");
            return View();
        }

        public IActionResult Detail(DefaultModel model)
        {
            if (model == null) return null;
            var currentClass = _service.GetItemByID(model.ID);
            if (currentClass == null)
                return RedirectToAction("Index");
            var vm = new ClassViewModel(currentClass);
            var subjects = _classSubjectService.GetByClassID(currentClass.ID);
            var skillIDs = subjects.Select(t => t.SkillID).Distinct();
            var subjectIDs = subjects.Select(t => t.SubjectID).Distinct();
            vm.SkillName = string.Join(", ", _skillService.GetList().Where(t => skillIDs.Contains(t.ID)).Select(t => t.Name).ToList());
            vm.SubjectName = string.Join(", ", _subjectService.Collection.Find(t => subjectIDs.Contains(t.ID)).Project(t => t.Name).ToList());
            ViewBag.Class = vm;
            ViewBag.Subject = _subjectService.GetItemByID(currentClass.SubjectID);
            ViewBag.Grade = _gradeService.GetItemByID(currentClass.GradeID);
            return View();
        }

        public IActionResult References(DefaultModel model)
        {
            if (model == null) return null;
            var currentClass = _service.GetItemByID(model.ID);
            if (currentClass == null)
                return RedirectToAction("Index");
            ViewBag.Class = currentClass;
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            var myClasses = _service.CreateQuery()
                .Find(t => t.TeacherID == UserID)
                //.Find(t=> true)
                //.Project(Builders<ClassEntity>.Projection.Include(t => t.ID).Include(t => t.Name))
                .ToList();
            ViewBag.AllClass = myClasses;
            ViewBag.User = UserID;
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
            ViewBag.Managable = CheckPermission(PERMISSION.MEMBER_COURSE_EDIT);
            return View();
        }

        public IActionResult StudentDetail(string ID, string ClassID)
        {
            if (string.IsNullOrEmpty(ClassID))
                return RedirectToAction("Index");
            var currentClass = _service.GetItemByID(ClassID);
            if (currentClass == null)
                return RedirectToAction("Index");
            ViewBag.Class = currentClass;

            if (string.IsNullOrEmpty(ID))
                return RedirectToAction("Member", "Class", new { ID = ClassID });

            var student = _studentService.GetItemByID(ID);
            if (student == null)
                return RedirectToAction("Member", "Class", new { ID = ClassID });

            ViewBag.Student = student;

            return View();
        }

        public IActionResult StudentModules(string ID, string ClassID)
        {
            var currentClass = _service.GetItemByID(ClassID);
            //var UserID = User.Claims.GetClaimByType("UserID").Value;
            if (currentClass == null)
                return null;
            if (currentClass.Students.IndexOf(ID) < 0)
                return null;
            var student = _studentService.GetItemByID(ID);
            if (student == null)
                return null;

            ViewBag.Class = currentClass;
            string courseid = currentClass.CourseID;
            var course = _courseService.GetItemByID(courseid);

            var lessons = (from r in _lessonService.CreateQuery().Find(o => o.CourseID == course.ID
                           //&& o.TemplateType == LESSON_TEMPLATE.LECTURE
                           ).ToList()
                           let schedule = _lessonScheduleService.CreateQuery().Find(o => o.LessonID == r.ID && o.ClassID == currentClass.ID).FirstOrDefault()
                           where schedule != null
                           let lessonProgress = new LessonProgressEntity()
                           //_lessonProgressService.GetByClassID_StudentID_LessonID(currentClass.ID, student.ID, r.ID)
                           //where lessonProgress != null
                           select _moduleViewMapping.AutoOrtherType(r, new StudentModuleViewModel()
                           {
                               ScheduleID = schedule.ID,
                               ScheduleStart = schedule.StartDate,
                               ScheduleEnd = schedule.EndDate,
                               IsActive = schedule.IsActive,
                               LearnCount = lessonProgress.TotalLearnt,
                               TemplateType = r.TemplateType,
                               LearnLast = lessonProgress.LastDate
                           })).OrderBy(r => r.LearnStart).ThenBy(r => r.ChapterID).ThenBy(r => r.ID).ToList();
            ViewBag.Lessons = lessons;
            return View();
        }

        public IActionResult StudentAssignment(string ID, string ClassID)
        {
            var currentClass = _service.GetItemByID(ClassID);
            //var UserID = User.Claims.GetClaimByType("UserID").Value;
            if (currentClass == null)
                return null;
            if (currentClass.Students.IndexOf(ID) < 0)
                return null;
            var student = _studentService.GetItemByID(ID);
            if (student == null)
                return null;

            ViewBag.Class = currentClass;
            string courseid = currentClass.CourseID;
            var course = _courseService.GetItemByID(courseid);

            var lessons = (from r in _lessonService.CreateQuery().Find(o => o.CourseID == course.ID && o.TemplateType == LESSON_TEMPLATE.EXAM).ToList()
                           let schedule = _lessonScheduleService.CreateQuery().Find(o => o.LessonID == r.ID && o.ClassID == currentClass.ID).FirstOrDefault()
                           where schedule != null
                           let exam = _examService.CreateQuery().Find(x => x.StudentID == student.ID && x.LessonID == r.ID && x.ClassID == currentClass.ID).SortByDescending(x => x.Created).FirstOrDefault()
                           //where exam != null
                           //let lastjoin = lessonProgress != null ? lessonProgress.LastDate : DateTime.MinValue
                           select _assignmentViewMapping.AutoOrtherType(r, new StudentAssignmentViewModel()
                           {
                               ScheduleID = schedule.ID,
                               ScheduleStart = schedule.StartDate,
                               ScheduleEnd = schedule.EndDate,
                               IsActive = schedule.IsActive,
                               LearnCount = exam == null ? 0 : exam.Number,
                               LearnLast = exam == null ? DateTime.MinValue : exam.Updated,
                               Point = exam == null ? 0 : exam.Point
                           })).OrderBy(r => r.ScheduleStart).ThenBy(r => r.ChapterID).ThenBy(r => r.LessonId).ToList();
            ViewBag.Lessons = lessons;
            return View();
        }

        #region ClassDetail
        [HttpPost]
        public JsonResult GetDetail(string ID)
        {
            try
            {
                var currentClass = _service.GetItemByID(ID);
                if (currentClass == null)
                    return new JsonResult(new Dictionary<string, object>
                    {
                        {"Error", "Không tìm thấy lớp học" }
                    });
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", currentClass }
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    {"Error", ex.Message }
                });
            }
        }

        //TODO: Check Usage
        //[HttpPost]
        //public JsonResult GetMainChapters(string ID)
        //{
        //    try
        //    {
        //        var currentClass = _service.GetItemByID(ID);
        //        if (currentClass == null)
        //            return new JsonResult(new Dictionary<string, object>
        //            {
        //                {"Error", "Không tìm thấy lớp học" }
        //            });

        //        var chapters = _chapterService.CreateQuery().Find(c => c.CourseID == currentClass.CourseID && c.ParentID == "0").ToList();
        //        var chapterExtends = _chapterExtendService.Search(currentClass.ID);

        //        foreach (var chapter in chapters)
        //        {
        //            var extend = chapterExtends.SingleOrDefault(t => t.ChapterID == chapter.ID);
        //            if (extend != null) chapter.Description = extend.Description;
        //        }
        //        var response = new Dictionary<string, object>
        //        {
        //            { "Data", chapters }
        //        };

        //        return new JsonResult(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new JsonResult(new Dictionary<string, object>
        //        {
        //            { "Data", null },
        //            {"Error", ex.Message }
        //        });
        //    }
        //}

        //[HttpPost]
        //public JsonResult UpdateChapterInfo(string ClassID, ChapterEntity chapter)
        //{
        //    try
        //    {
        //        var currentClass = _service.GetItemByID(ClassID);
        //        if (currentClass == null)
        //            return new JsonResult(new Dictionary<string, object>
        //            {
        //                {"Error", "Không tìm thấy lớp học" }
        //            });

        //        var currentChapter = _chapterService.CreateQuery().Find(c => c.ID == chapter.ID).FirstOrDefault();
        //        if (currentChapter == null)
        //            return new JsonResult(new Dictionary<string, object>
        //            {
        //                {"Error", "Không tìm thấy chương" }
        //            });

        //        var chapterExtend = _chapterExtendService.Search(ClassID, chapter.ID).SingleOrDefault();
        //        if (chapterExtend == null)
        //        {
        //            chapterExtend = new ChapterExtendEntity
        //            {
        //                ChapterID = chapter.ID,
        //                ClassID = ClassID
        //            };
        //        }
        //        chapterExtend.Description = chapter.Description;
        //        _chapterExtendService.Save(chapterExtend);

        //        currentChapter.Description = chapter.Description;

        //        var response = new Dictionary<string, object>
        //        {
        //            { "Data", currentChapter }
        //        };

        //        return new JsonResult(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new JsonResult(new Dictionary<string, object>
        //        {
        //            { "Data", null },
        //            {"Error", ex.Message }
        //        });
        //    }
        //}

        //TODO: CHECK USAGE
        //[Obsolete]
        //[HttpPost]
        //public JsonResult GetDetailsLesson(DefaultModel model, string SubjectID = "", string GradeID = "", string UserID = "")
        //{
        //    var filter = new List<FilterDefinition<ClassEntity>>();
        //    TeacherEntity teacher = null;
        //    if (string.IsNullOrEmpty(UserID))
        //        UserID = User.Claims.GetClaimByType("UserID").Value;
        //    if (!string.IsNullOrEmpty(UserID) && UserID != "0")
        //    {
        //        teacher = UserID == "0" ? null : _teacherService.GetItemByID(UserID);
        //        if (teacher == null)
        //        {
        //            return new JsonResult(new Dictionary<string, object> {
        //                {"Data",null },
        //                {"Error",model },
        //                {"Msg","Không có thông tin giảng viên" }
        //            });
        //        }
        //    }
        //    if (teacher != null)
        //        filter.Add(Builders<ClassEntity>.Filter.Where(o => o.TeacherID == UserID));

        //    if (!string.IsNullOrEmpty(model.SearchText))
        //    {
        //        filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Name.ToLower().Contains(model.SearchText.ToLower())));
        //    }
        //    if (!string.IsNullOrEmpty(SubjectID))
        //    {
        //        filter.Add(Builders<ClassEntity>.Filter.Where(o => o.SubjectID == SubjectID));
        //    }
        //    if (!string.IsNullOrEmpty(GradeID))
        //    {
        //        filter.Add(Builders<ClassEntity>.Filter.Where(o => o.GradeID == GradeID));
        //    }

        //    var data = filter.Count > 0 ? _service.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _service.GetAll();
        //    model.TotalRecord = data.Count();
        //    var DataResponse = data == null || data.Count() <= 0 || data.Count() < model.PageSize
        //        ? data
        //        : data.Skip((model.PageIndex - 1) * model.PageSize).Limit(model.PageSize);

        //    var response = new Dictionary<string, object>
        //    {
        //        { "Data", DataResponse.ToList().Select(o=> new ClassViewModel(o){
        //                CourseName = _courseService.GetItemByID(o.CourseID)?.Name,
        //                GradeName = _gradeService.GetItemByID(o.GradeID)?.Name,
        //                SubjectName = _subjectService.GetItemByID(o.SubjectID).Name,
        //                TeacherName = _teacherService.GetItemByID(o.TeacherID).FullName
        //            })
        //        },
        //        { "Model", model }
        //    };
        //    return new JsonResult(response);
        //}

        public JsonResult GetListTeacher(string SubjectID = "")
        {
            var filter = new List<FilterDefinition<TeacherEntity>>();

            var UserID = User.Claims.GetClaimByType("UserID").Value;

            if (string.IsNullOrEmpty(SubjectID))
                return new JsonResult(new Dictionary<string, object> { });

            filter.Add(Builders<TeacherEntity>.Filter.Where(o => o.Subjects.Contains(SubjectID)));
            var teachers = _teacherService.Collection.Find(Builders<TeacherEntity>.Filter.And(filter));
            var response = new Dictionary<string, object>
            {
                { "Data", teachers.ToList().Select(o => new
                    {
                       id = o.ID,
                       fullname = o.FullName
                    }
                    ).ToList()
                }
            };
            return new JsonResult(response);
        }

        public JsonResult GetMarks(DefaultModel model)
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
            var studentsView = students.ToList().Select(t => _mapping.AutoOrtherType(t, new ClassStudentViewModel()
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

        public JsonResult GetClassResult(DefaultModel model, string ClassID)
        {
            var @class = _service.GetItemByID(ClassID);
            if (@class == null)
                return null;
            var data = new List<StudentSummaryViewModel>();

            var total_students = _studentService.CountByClass(@class.ID);
            var examCount = _lessonScheduleService.CountClassExam(@class.ID, null);
            var total_lessons = _lessonService.CountClassLesson(@class.ID);
            var results = _classProgressService.GetClassResults(@class.ID).OrderByDescending(t => t.TotalPoint).ToList();
            foreach (var student in _studentService.GetClassStudents(@class.ID))
            {
                var summary = new MappingEntity<ClassProgressEntity, StudentSummaryViewModel>()
                    .AutoOrtherType(_classProgressService.GetStudentResult(@class.ID, student.ID) ?? new ClassProgressEntity
                    {
                        ClassID = @class.ID,
                        StudentID = student.ID,
                        TotalLessons = total_lessons,
                    }, new StudentSummaryViewModel()
                    {
                        ClassName = @class.Name,
                        Rank = -1,
                        TotalStudents = (int)total_students
                    });

                summary.FullName = student.FullName;
                if (results != null && (results.FindIndex(t => t.StudentID == student.ID) >= 0))
                    summary.Rank = results.FindIndex(t => t.TotalPoint == summary.TotalPoint) + 1;
                summary.AvgPoint = examCount > 0 ? summary.TotalPoint / examCount : 0;

                data.Add(summary);
            }
            var response = new Dictionary<string, object>
            {
                { "Data", data},
                { "Model", model }
            };
            return new JsonResult(response);
        }

        #endregion

        #region Old
        //Class Management
        //[HttpPost]
        //public JsonResult GetList(DefaultModel model, string SubjectID = "", string GradeID = "", string UserID = "")
        //{
        //    var filter = new List<FilterDefinition<ClassEntity>>();
        //    filter.Add(Builders<ClassEntity>.Filter.Where(o => o.IsActive));
        //    TeacherEntity teacher = null;
        //    if (string.IsNullOrEmpty(UserID))
        //        UserID = User.Claims.GetClaimByType("UserID").Value;
        //    if (!string.IsNullOrEmpty(UserID) && UserID != "0")
        //    {
        //        teacher = UserID == "0" ? null : _teacherService.GetItemByID(UserID);
        //        if (teacher == null)
        //        {
        //            return new JsonResult(new Dictionary<string, object> {
        //                {"Data",null },
        //                {"Error",model },
        //                {"Msg","Teacher not found" }
        //            });
        //        }
        //    }
        //    if (teacher != null)
        //        filter.Add(Builders<ClassEntity>.Filter.Where(o => o.TeacherID == UserID));

        //    if (!string.IsNullOrEmpty(model.SearchText))
        //    {
        //        filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Name.ToLower().Contains(model.SearchText.ToLower())));
        //    }
        //    if (!string.IsNullOrEmpty(SubjectID))
        //    {
        //        filter.Add(Builders<ClassEntity>.Filter.Where(o => o.SubjectID == SubjectID));
        //    }
        //    //if (!string.IsNullOrEmpty(GradeID))
        //    //{
        //    //    filter.Add(Builders<ClassEntity>.Filter.Where(o => o.GradeID == GradeID));
        //    //}

        //    var data = filter.Count > 0 ? _service.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _service.GetAll();
        //    model.TotalRecord = data.CountDocuments();
        //    var DataResponse = data == null || model.TotalRecord <= 0 // || model.TotalRecord < model.PageSize
        //        ? data
        //        : data.Skip((model.PageIndex) * model.PageSize).Limit(model.PageSize);

        //    var response = new Dictionary<string, object>
        //    {
        //        { "Data", DataResponse.ToList().Select(o=> new ClassViewModel(o){
        //                //CourseName = _courseService.GetItemByID(o.CourseID)?.Name,
        //                //GradeName = _gradeService.GetItemByID(o.GradeID)?.Name,
        //                //SubjectName = _subjectService.GetItemByID(o.SubjectID).Name,
        //                //TeacherName = _teacherService.GetItemByID(o.TeacherID).FullName
        //            })
        //        },
        //        { "Model", model }
        //    };
        //    return new JsonResult(response);
        //}
        #endregion

        #region Homepage
        public JsonResult GetActiveList(DateTime today)
        {
            today = today.ToUniversalTime();
            var filter = new List<FilterDefinition<ClassEntity>>();
            filter.Add(Builders<ClassEntity>.Filter.Where(o => o.IsActive));
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new ReturnJsonModel
                {
                    StatusCode = ReturnStatus.ERROR,
                    StatusDesc = "Authentication Error"
                });
            }
            filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Members.Any(t => t.TeacherID == userId)));
            filter.Add(Builders<ClassEntity>.Filter.Where(o => (o.StartDate <= today) && (o.EndDate >= today)));

            var data = (filter.Count > 0 ? _service.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _service.GetAll()).SortByDescending(t => t.ID);

            var std = (from o in data.ToList()
                       let totalweek = (o.EndDate.Date - o.StartDate.Date).TotalDays / 7
                       let subject = _subjectService.GetItemByID(o.SubjectID)
                       let studentCount = //_classStudentService.GetClassStudents(o.ID).Count
                       _studentService.CountByClass(o.ID)
                       select new
                       {
                           id = o.ID,
                           courseID = o.CourseID,
                           courseName = o.Name,
                           subjectName = subject == null ? "" : subject.Name,
                           thumb = o.Image ?? "",
                           endDate = o.EndDate,
                           //week = totalweek > 0 ? (DateTime.Now.Date - o.StartDate.Date).TotalDays / 7 / totalweek : 0,
                           students = studentCount
                       }).ToList();
            return Json(new { Data = std });
        }

        public JsonResult GetFinishList(DefaultModel model, DateTime today)
        {
            today = today.ToUniversalTime();
            var filter = new List<FilterDefinition<ClassEntity>>();
            filter.Add(Builders<ClassEntity>.Filter.Where(o => o.IsActive));
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Members.Any(t => t.TeacherID == userId)));
            filter.Add(Builders<ClassEntity>.Filter.Where(o => o.EndDate < today));

            var data = filter.Count > 0 ? _service.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _service.GetAll();
            model.TotalRecord = data.CountDocuments();
            var DataResponse = data == null || model.TotalRecord <= 0 || model.TotalRecord < model.PageSize
                ? data.SortByDescending(t => t.ID).ToList()
                : data.SortByDescending(t => t.ID).Skip(model.PageIndex * model.PageSize).Limit(model.PageSize).ToList();

            var std = (from o in DataResponse
                       select new
                       {
                           id = o.ID,
                           courseID = o.CourseID,
                           title = o.Name,
                           endDate = o.EndDate,
                       }).ToList();
            return Json(new { Data = std });
        }

        public JsonResult GetThisWeekLesson(DateTime today)
        {
            today = today.ToUniversalTime();
            var startWeek = today.AddDays(DayOfWeek.Sunday - today.DayOfWeek);
            var endWeek = startWeek.AddDays(7);

            var filter = new List<FilterDefinition<LessonScheduleEntity>>();
            filter.Add(Builders<LessonScheduleEntity>.Filter.Where(o => o.IsActive));
            filter.Add(Builders<LessonScheduleEntity>.Filter.Where(o => o.StartDate <= endWeek && o.EndDate >= startWeek));
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new ReturnJsonModel
                {
                    StatusCode = ReturnStatus.ERROR,
                    StatusDesc = "Authentication Error"
                });
            }

            var classFilter = new List<FilterDefinition<ClassEntity>>();
            classFilter.Add(Builders<ClassEntity>.Filter.Where(o => o.Members.Any(t => t.TeacherID == userId)));
            var classIds = _service.Collection.Find(Builders<ClassEntity>.Filter.And(classFilter)).Project(t => t.ID).ToList();

            filter.Add(Builders<LessonScheduleEntity>.Filter.Where(t => classIds.Contains(t.ClassID)));

            //var csIds = _lessonScheduleService.Collection.Distinct(t => t.ClassSubjectID, Builders<LessonScheduleEntity>.Filter.And(filter)).ToList();

            var data = _lessonScheduleService.Collection.Find(Builders<LessonScheduleEntity>.Filter.And(filter)).ToList();

            //var data = _classSubjectService.Collection.Find(t => csIds.Contains(t.ID));
            var std = (from o in data.ToList()
                       let _lesson = _lessonService.Collection.Find(t => t.ID == o.LessonID).SingleOrDefault()
                       where _lesson != null
                       let _class = _service.Collection.Find(t => t.ID == o.ClassID).SingleOrDefault()
                       where _class != null
                       let _sbj = _classSubjectService.GetItemByID(o.ClassSubjectID)
                       let skill = _skillService.GetItemByID(_sbj.SkillID)
                       let studentCount = //_classStudentService.GetClassStudents(_class.ID).Count
                       _studentService.CountByClass(_class.ID)
                       select new
                       {
                           id = o.ID,
                           classID = _class.ID,
                           classsubjectID = _sbj.ID,
                           className = _class.Name,
                           title = _lesson.Title,
                           lessonID = _lesson.ID,
                           startDate = o.StartDate,
                           endDate = o.EndDate,
                           students = studentCount,
                           skill = skill
                           //isLearnt = isLearnt
                       }).OrderBy(t => t.startDate).ToList();
            //var std = (from o in data.ToList()
            //           let _class = _service.Collection.Find(t => t.ID == o.ClassID).SingleOrDefault()
            //           where _class != null
            //           let skill = _skillService.GetItemByID(o.SkillID)
            //           //let isLearnt = _learningHistoryService.GetLastLearnt(userId, o.LessonID) != null
            //           select new
            //           {
            //               id = o.ID,
            //               classID = _class.ID,
            //               className = _class.Name,
            //               endDate = o.EndDate,
            //               students = _class.Students.Count,
            //               skill = skill
            //               //isLearnt = isLearnt
            //           }).ToList();
            return Json(new { Data = std });
        }
        #endregion

        #region Manage
        public JsonResult GetManageList(DefaultModel model, string SubjectID = "", string GradeID = "", string TeacherID = "", bool skipActive = true)
        {
            var returndata = FilterClass(model, SubjectID, GradeID, TeacherID, skipActive);
            //model.TotalRecord = totalrec;

            var response = new Dictionary<string, object>
                {
                    { "Data", returndata},
                    { "Model", model }
                };
            return new JsonResult(response);
        }

        public JsonResult GetClassList(DefaultModel model, string SubjectID = "", string GradeID = "")
        {
            var returndata = FilterClass(model, SubjectID, GradeID, User.Claims.GetClaimByType("UserID").Value, true);
            //model.TotalRecord = totalrec;

            var response = new Dictionary<string, object>
                {
                    { "Data", returndata},
                    { "Model", model }
                };
            return new JsonResult(response);
        }

        private List<Dictionary<string, object>> FilterClass(DefaultModel model, string SubjectID = "", string GradeID = "", string TeacherID = "", bool skipActive = true)
        {
            model.TotalRecord = 0;
            var filter = new List<FilterDefinition<ClassSubjectEntity>>();
            var classfilter = new List<FilterDefinition<ClassEntity>>();
            var deep_filter = false;
            FilterDefinition<ClassEntity> ownerfilter = null;
            var UserID = User.Claims.GetClaimByType("UserID").Value;

            if (!string.IsNullOrEmpty(SubjectID))
            {
                deep_filter = true;
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.SubjectID == SubjectID));
            }
            else
            {
                var teacher = _teacherService.GetItemByID(UserID);
                if (teacher == null)
                    return null;
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => teacher.Subjects.Contains(o.SubjectID)));
            }
            if (!string.IsNullOrEmpty(GradeID))
            {
                deep_filter = true;
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.GradeID == GradeID));
            }
            if (!string.IsNullOrEmpty(TeacherID))
            {
                deep_filter = true;
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.TeacherID == TeacherID));
            }

            if (!deep_filter)
                ownerfilter = new FilterDefinitionBuilder<ClassEntity>().Where(o => o.TeacherID == UserID);

            if (model.StartDate > new DateTime(1900, 1, 1))
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.EndDate >= model.StartDate));

            if (model.StartDate > new DateTime(1900, 1, 1))
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.StartDate <= model.EndDate));

            var data = _classSubjectService.Collection
                .Distinct(t => t.ClassID, filter.Count > 0 ? Builders<ClassSubjectEntity>.Filter.And(filter) : Builders<ClassSubjectEntity>.Filter.Empty).ToList();
            //filter by classsubject
            if (ownerfilter != null)
            {
                if (data.Count > 0)
                    classfilter.Add(
                        Builders<ClassEntity>.Filter.Or(ownerfilter,
                        Builders<ClassEntity>.Filter.Where(t => data.Contains(t.ID) && (t.IsActive || skipActive))));
                else
                    classfilter.Add(ownerfilter);
            }
            else
                if (data.Count > 0)
                classfilter.Add(Builders<ClassEntity>.Filter.Where(t => data.Contains(t.ID) && (t.IsActive || skipActive)));

            //if (data.Count > 0)
            //{
            //    if (ownerfilter != null)

            //    else
            //        classfilter.Add(Builders<ClassEntity>.Filter.Where(t => data.Contains(t.ID) && (t.IsActive || skipActive)));
            //}

            //if (ownerfilter != null && filter.Count <= 1)//no filter
            //    classfilter.Add(ownerfilter);

            if (!string.IsNullOrEmpty(model.SearchText))
                classfilter.Add(Builders<ClassEntity>.Filter.Text("\"" + model.SearchText + "\""));

            if (classfilter.Count == 0)
                return null;

            var classResult = _service.Collection.Find(Builders<ClassEntity>.Filter.And(classfilter));

            model.TotalRecord = classResult.CountDocuments();

            var classData = classResult.SortByDescending(t => t.IsActive).ThenByDescending(t => t.StartDate).Skip(model.PageIndex * model.PageSize).Limit(model.PageSize).ToList();
            var returndata = from o in classData
                             let skillIDs = _classSubjectService.GetByClassID(o.ID).Select(t => t.SkillID).Distinct()
                             let creator = _teacherService.GetItemByID(o.TeacherID) //Todo: Fix
                             let sname = skillIDs == null ? "" : string.Join(", ", _skillService.GetList().Where(t => skillIDs.Contains(t.ID)).Select(t => t.Name).ToList())
                             select new Dictionary<string, object>
                                {
                                 { "ID", o.ID },
                                 { "Name", o.Name },
                                 { "Students", 
                                     //_classStudentService.GetClassStudents(o.ID).Count 
                                     _studentService.CountByClass(o.ID)
                                 },
                                 { "Created", o.Created },
                                 { "IsActive", o.IsActive },
                                 { "Image", o.Image },
                                 { "StartDate", o.StartDate },
                                 { "EndDate", o.EndDate },
                                 { "Order", o.Order },
                                 { "Skills", o.Skills },
                                 { "Members", o.Members },
                                 { "Description", o.Description },
                                 { "SkillName", sname },
                                 { "Creator", o.TeacherID },
                                 { "CreatorName", creator.FullName }
                             };
            return returndata.ToList();
        }


        [HttpPost]
        [Obsolete]
        public JsonResult Create(ClassEntity item, List<ClassSubjectEntity> classSubjects, IFormFile fileUpload)
        {
            if (string.IsNullOrEmpty(item.ID) || item.ID == "0")
            {
                item.ID = null;
                item.Created = DateTime.Now;
                var userId = User.Claims.GetClaimByType("UserID").Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return new JsonResult(new Dictionary<string, object>()
                        {
                            {"Error", "Vui lòng đăng nhập lại" }
                        });
                }

                if (classSubjects == null || classSubjects.Count == 0)
                {
                    return new JsonResult(new Dictionary<string, object>()
                        {
                            {"Error", "Cần chọn ít nhất một môn học" }
                        });
                }

                item.TeacherID = userId; // creator
                item.Skills = new List<string>();
                item.Subjects = new List<string>();
                item.Members = new List<ClassMemberEntity>();
                item.TotalLessons = 0;
                item.IsActive = true;
                item.StartDate = item.StartDate.ToUniversalTime();
                item.EndDate = item.EndDate.ToUniversalTime();

                if (fileUpload != null)
                {
                    var pathImage = _fileProcess.SaveMediaAsync(fileUpload, "", "CLASSIMG").Result;
                    item.Image = pathImage;
                }

                _service.Save(item);

                //Create class subjects
                if (classSubjects != null && classSubjects.Count > 0)
                {
                    foreach (var csubject in classSubjects)
                    {

                        var newMember = new ClassMemberEntity();
                        long lessoncount = 0;
                        var nID = CreateNewClassSubject(csubject, item, out newMember, out lessoncount);
                        if (!item.Skills.Contains(csubject.SkillID))
                            item.Skills.Add(csubject.SkillID);
                        if (!item.Subjects.Contains(csubject.SubjectID))
                            item.Subjects.Add(csubject.SubjectID);
                        if (!item.Members.Any(t => t.TeacherID == newMember.TeacherID && t.Type == ClassMemberType.TEACHER))
                            item.Members.Add(newMember);
                    }
                    _service.Save(item);
                }
                Dictionary<string, object> response = new Dictionary<string, object>()
                {
                    {"Data",item },
                    {"Error",null },
                    {"Msg","Success" }
                };
                return new JsonResult(response);
            }
            else
            {
                var processCS = new List<string>();
                var oldData = _service.GetItemByID(item.ID);
                if (oldData == null) return new JsonResult(new Dictionary<string, object>()
                {
                    {"Error", "Class not found" }
                });

                oldData.Updated = DateTime.Now;
                oldData.Name = item.Name;
                oldData.Code = item.Code;
                oldData.StartDate = item.StartDate.ToUniversalTime();
                oldData.EndDate = item.EndDate.ToUniversalTime();
                oldData.Description = item.Description;
                //_service.Save(oldData);

                oldData.Skills = new List<string>();
                oldData.Subjects = new List<string>();
                oldData.Members = new List<ClassMemberEntity>();
                oldData.TotalLessons = 0;

                var oldSubjects = _classSubjectService.GetByClassID(item.ID);

                if (oldSubjects != null)
                {
                    foreach (var oSbj in oldSubjects)
                    {
                        var nSbj = classSubjects.Find(t => t.ID == oSbj.ID);
                        if (nSbj == null || (nSbj.CourseID != oSbj.CourseID))
                        //delete oldSubject
                        {
                            _ = RemoveClassSubject(oSbj);
                            if (nSbj != null)
                                nSbj.ID = null;//remove ID to create new
                        }

                        if (nSbj != null)
                        {
                            var newMember = new ClassMemberEntity();
                            long lessoncount = 0;
                            if (nSbj.CourseID != oSbj.CourseID)//SkillID ~ CourseID
                            {
                                nSbj.ID = CreateNewClassSubject(nSbj, item, out newMember, out lessoncount);
                                if (string.IsNullOrEmpty(nSbj.ID))//Error
                                    continue;
                            }
                            else //Not change
                            {
                                //update period
                                oSbj.StartDate = item.StartDate.ToUniversalTime();
                                oSbj.EndDate = item.EndDate.ToUniversalTime();
                                oSbj.TeacherID = nSbj.TeacherID;
                                _classSubjectService.Save(oSbj);

                                var teacher = _teacherService.GetItemByID(nSbj.TeacherID);
                                if (teacher == null) continue;
                                newMember = new ClassMemberEntity
                                {
                                    TeacherID = teacher.ID,
                                    Name = teacher.FullName,
                                    Type = ClassMemberType.TEACHER
                                };
                            }
                            processCS.Add(nSbj.ID);
                            if (!oldData.Skills.Contains(nSbj.SkillID))
                                oldData.Skills.Add(nSbj.SkillID);
                            if (!oldData.Subjects.Contains(nSbj.SubjectID))
                                oldData.Subjects.Add(nSbj.SubjectID);
                            if (!oldData.Members.Any(t => t.TeacherID == newMember.TeacherID && t.Type == ClassMemberType.TEACHER))
                                oldData.Members.Add(newMember);
                            oldData.TotalLessons += lessoncount;
                        }
                    }
                }

                if (classSubjects != null && classSubjects.Count > 0)
                {
                    foreach (var nSbj in classSubjects)
                    {
                        if (processCS.IndexOf(nSbj.ID) >= 0)
                            continue;
                        //create new subject
                        var newMember = new ClassMemberEntity();
                        long lessoncount = 0;
                        var nID = CreateNewClassSubject(nSbj, item, out newMember, out lessoncount);
                        if (string.IsNullOrEmpty(nSbj.ID))//Error
                            continue;

                        if (!oldData.Skills.Contains(nSbj.SkillID))
                            oldData.Skills.Add(nSbj.SkillID);
                        if (!oldData.Subjects.Contains(nSbj.SubjectID))
                            oldData.Subjects.Add(nSbj.SubjectID);
                        if (!oldData.Members.Any(t => t.TeacherID == newMember.TeacherID && t.Type == ClassMemberType.TEACHER))
                            oldData.Members.Add(newMember);
                        oldData.TotalLessons += lessoncount;
                    }
                }

                if (fileUpload != null)
                {
                    var pathImage = _fileProcess.SaveMediaAsync(fileUpload, "", "CLASSIMG").Result;
                    oldData.Image = pathImage;
                }

                //update data
                _service.Save(oldData);
                //refresh class total lesson => no need
                _ = _classProgressService.RefreshTotalLessonForClass(oldData.ID);

                Dictionary<string, object> response = new Dictionary<string, object>()
                {
                    {"Data",item },
                    {"Error",null },
                    {"Msg","Success" }
                };
                return new JsonResult(response);
            }
        }

        private async Task RemoveClassSubject(ClassSubjectEntity cs)
        {
            ////remove old schedule
            await _lessonScheduleService.RemoveClassSubject(cs.ID);
            //remove chapter
            await _chapterService.RemoveClassSubjectChapter(cs.ID);
            //remove clone lesson
            await _lessonHelper.RemoveClassSubjectLesson(cs.ID);
            //remove progress: learning history => class progress, chapter progress, lesson progress
            await _learningHistoryService.RemoveClassSubjectHistory(cs.ID);
            //remove exam
            await _examService.RemoveClassSubjectExam(cs.ID);
            //remove classSubject
            _classSubjectService.Remove(cs.ID);
        }


        private string CreateNewClassSubject(ClassSubjectEntity nSbj, ClassEntity @class, out ClassMemberEntity member, out long lessoncount)
        {
            member = new ClassMemberEntity();
            lessoncount = 0;
            try
            {
                var subject = _subjectService.GetItemByID(nSbj.SubjectID);
                if (subject == null)
                {
                    throw new Exception("Subject " + nSbj.SubjectID + " is not avaiable");
                }
                var course = _courseService.GetItemByID(nSbj.CourseID);
                if (course == null || !course.IsActive)
                {
                    throw new Exception("Course " + nSbj.CourseID + " is not avaiable");
                }

                lessoncount = course.TotalLessons;

                var teacher = _teacherService.GetItemByID(nSbj.TeacherID);
                if (teacher == null || !teacher.IsActive || !teacher.Subjects.Contains(nSbj.SubjectID))
                {
                    throw new Exception("Teacher " + nSbj.TeacherID + " is not avaiable");
                }

                nSbj.ClassID = @class.ID;
                nSbj.StartDate = @class.StartDate;
                nSbj.EndDate = @class.EndDate;

                nSbj.SkillID = course.SkillID;
                nSbj.Description = course.Description;
                nSbj.LearningOutcomes = course.LearningOutcomes;
                nSbj.TotalLessons = course.TotalLessons;

                _classSubjectService.Save(nSbj);

                //Clone Course
                _courseHelper.CloneForClassSubject(nSbj);

                member = new ClassMemberEntity
                {
                    Name = teacher.FullName,
                    TeacherID = teacher.ID,
                    Type = ClassMemberType.TEACHER
                };
                return nSbj.ID;
            }
            catch (Exception e)
            {
                return "";
            }
        }

        [HttpPost]
        [Obsolete]
        public JsonResult Remove(DefaultModel model)
        {
            if (model.ArrID.Length <= 0)
            {
                return new JsonResult(
                    new Dictionary<string, object>
                    {
                        { "Error", "Nothing to Delete" }
                    }
                );
            }
            else
            {

                var ids = model.ArrID.Split(',');
                if (ids.Length > 0)
                {
                    //Remove Class Student
                    _ = _studentService.LeaveClassAll(ids.ToList());
                    //remove ClassSubject
                    _classSubjectService.RemoveClassSubjects(ids);
                    //remove Lesson, Part, Question, Answer
                    _lessonHelper.RemoveClone(ids);
                    //remove Schedule
                    _ = _lessonScheduleService.RemoveManyClass(ids);
                    //remove History
                    _ = _learningHistoryService.RemoveClassHistory(ids);
                    //remove Exam
                    _examService.Collection.DeleteMany(o => ids.Contains(o.ClassID));
                    //remove Exam Detail
                    _examDetailService.Collection.DeleteMany(o => ids.Contains(o.ClassID));
                    var delete = _service.Collection.DeleteMany(o => ids.Contains(o.ID));
                    return new JsonResult(delete);
                }
                else
                {
                    return new JsonResult(
                       new Dictionary<string, object>
                       {
                            { "Error", "Nothing to Delete" }
                       }
                    );
                }
            }
        }

        [HttpPost]
        [Obsolete]
        public JsonResult Publish(DefaultModel model)
        {
            if (model.ArrID.Length <= 0)
            {
                return new JsonResult(null);
            }
            else
            {
                if (model.ArrID.Contains(","))
                {
                    var filter = Builders<ClassEntity>.Filter.Where(o => model.ArrID.Split(',').Contains(o.ID) && o.IsActive == false);
                    var update = Builders<ClassEntity>.Update.Set("IsActive", true);
                    var publish = _service.Collection.UpdateMany(filter, update);
                    return new JsonResult(publish);
                }
                else
                {
                    var filter = Builders<ClassEntity>.Filter.Where(o => model.ArrID == o.ID && o.IsActive == false);
                    var update = Builders<ClassEntity>.Update.Set("IsActive", true);
                    var publish = _service.Collection.UpdateMany(filter, update);
                    return new JsonResult(publish);
                }


            }
        }

        [HttpPost]
        [Obsolete]
        public JsonResult UnPublish(DefaultModel model)
        {
            if (model.ArrID.Length <= 0)
            {
                return new JsonResult(null);
            }
            else
            {
                if (model.ArrID.Contains(","))
                {
                    var filter = Builders<ClassEntity>.Filter.Where(o => model.ArrID.Split(',').Contains(o.ID) && o.IsActive == true);
                    var update = Builders<ClassEntity>.Update.Set("IsActive", false);
                    var publish = _service.Collection.UpdateMany(filter, update);
                    return new JsonResult(publish);
                }
                else
                {
                    var filter = Builders<ClassEntity>.Filter.Where(o => model.ArrID == o.ID && o.IsActive == true);
                    var update = Builders<ClassEntity>.Update.Set("IsActive", false);
                    var publish = _service.Collection.UpdateMany(filter, update);
                    return new JsonResult(publish);
                }


            }
        }
        #endregion

        #region Student Detail
        public JsonResult GetLearningProgress(DefaultModel model, string ClassID, string ClassSubjectID)
        {
            var StudentID = model.ID;
            if (!_studentService.IsStudentInClass(ClassID, StudentID))
                return null;

            var currentClass = _service.GetItemByID(ClassID);
            if (currentClass == null)
                return null;

            List<LessonProgressEntity> data;
            if (string.IsNullOrEmpty(ClassSubjectID))
                data = _lessonProgressService.GetByClassID_StudentID(ClassID, StudentID);
            else
                data = _lessonProgressService.GetByClassSubjectID_StudentID(ClassSubjectID, StudentID);

            var subjects = _classSubjectService.GetByClassID(ClassID);

            var lessons = (from progress in data
                           let schedule = _lessonScheduleService.CreateQuery().Find(o => o.LessonID == progress.LessonID && o.ClassID == currentClass.ID).FirstOrDefault()
                           where schedule != null
                           let classsubject = subjects.Single(t => t.ID == schedule.ClassSubjectID)
                           where classsubject != null
                           let lesson = _lessonService.GetItemByID(progress.LessonID)
                           select _moduleViewMapping.AutoOrtherType(lesson, new StudentModuleViewModel()
                           {
                               ScheduleID = schedule.ID,
                               ScheduleStart = schedule.StartDate,
                               Skill = _skillService.GetItemByID(classsubject.SkillID).Name,
                               ScheduleEnd = schedule.EndDate,
                               IsActive = schedule.IsActive,
                               LearnCount = progress.TotalLearnt,
                               TemplateType = lesson.TemplateType,
                               LearnStart = progress.FirstDate,
                               LearnLast = progress.LastDate
                           })).OrderBy(r => r.LearnStart).ThenBy(r => r.ChapterID).ThenBy(r => r.ID).ToList();

            var response = new Dictionary<string, object>
            {
                { "Data", lessons},
                { "Model", model }
            };
            return new JsonResult(response);
        }

        public JsonResult GetLearningResult(DefaultModel model, string ClassID, string ClassSubjectID)
        {
            var StudentID = model.ID;
            if (!_studentService.IsStudentInClass(ClassID, StudentID))
                return null;

            var currentClass = _service.GetItemByID(ClassID);
            if (currentClass == null)
                return null;

            List<LessonProgressEntity> data;
            List<LessonScheduleEntity> passExams;
            if (string.IsNullOrEmpty(ClassSubjectID))
            {
                passExams = _lessonScheduleService.GetClassExam(ClassID, model.StartDate, model.EndDate);
                data = _lessonProgressService.GetByClassID_StudentID(ClassID, StudentID);
            }
            else
            {
                passExams = _lessonScheduleService.GetClassSubjectExam(ClassSubjectID, model.StartDate, model.EndDate);
                data = _lessonProgressService.GetByClassSubjectID_StudentID(ClassSubjectID, StudentID);
            }

            var subjects = _classSubjectService.GetByClassID(ClassID);

            var lessons = (
                            from schedule in passExams
                            let progress = data.FirstOrDefault(t => t.StudentID == model.ID && t.ClassSubjectID == schedule.ClassSubjectID && t.LessonID == schedule.LessonID) ?? new LessonProgressEntity()
                            //from progress in data
                            //where progress.Tried > 0
                            //let schedule = _lessonScheduleService.CreateQuery().Find(o => o.LessonID == progress.LessonID && o.ClassID == currentClass.ID).FirstOrDefault()
                            //where schedule != null
                            //let classsubject = subjects.Single(t => t.ID == schedule.ClassSubjectID)
                            //where classsubject != null
                            let lesson = _lessonService.GetItemByID(schedule.LessonID)
                            select _assignmentViewMapping.AutoOrtherType(lesson, new StudentAssignmentViewModel()
                            {
                                ScheduleID = schedule.ID,
                                ScheduleStart = schedule.StartDate,
                                ScheduleEnd = schedule.EndDate,
                                IsActive = schedule.IsActive,
                                LearnCount = progress.Tried,
                                LearnLast = progress.LastTry,
                                Result = progress.LastPoint,
                            })).OrderByDescending(r => r.ScheduleStart).ThenBy(r => r.ChapterID).ThenBy(r => r.LessonId).ToList();

            var response = new Dictionary<string, object>
            {
                { "Data", lessons},
                { "Model", model }
            };
            return new JsonResult(response);
        }

        public JsonResult GetLearningSummary(DefaultModel model, string ClassID)
        {
            var StudentID = model.ID;
            if (!_studentService.IsStudentInClass(ClassID, StudentID))
                return null;

            var currentClass = _service.GetItemByID(ClassID);
            if (currentClass == null)
                return null;

            var total_students = _studentService.CountByClass(currentClass.ID);
            var rank = -1;
            var results = _classProgressService.GetClassResults(ClassID).OrderByDescending(t => t.TotalPoint).ToList();
            var avgpoint = 0.0;
            var studentresult = _classProgressService.GetStudentResult(ClassID, StudentID);

            if (studentresult != null) {
                var examCount = _lessonScheduleService.CountClassExam(ClassID, null);
                if (examCount > 0)
                    avgpoint = studentresult.TotalPoint / examCount;
            }
            if (studentresult != null && results != null && (results.FindIndex(t => t.StudentID == StudentID) >= 0))
                rank = results.FindIndex(t => t.TotalPoint == studentresult.TotalPoint) + 1;
            var response = new Dictionary<string, object>
            {
                { "Result", new { pos =  1, total = total_students, avg = avgpoint } },
                { "Data", GetClassSubjectSummary(currentClass, StudentID, total_students)},
                { "Model", model }
            };

            return new JsonResult(response);
        }

        public JsonResult GetStudentSummary(DefaultModel model)
        {
            var StudentID = model.ID;
            var student = _studentService.GetItemByID(StudentID);
            if (student == null)
                return null;

            var data = new List<StudentSummaryViewModel>();
            if (student.JoinedClasses != null && student.JoinedClasses.Count > 0)
            {

                foreach (var ClassID in student.JoinedClasses)
                {
                    var @class = _service.GetItemByID(ClassID);
                    var total_students = _studentService.CountByClass(@class.ID);
                    var examCount = _lessonScheduleService.CountClassExam(@class.ID, null);
                    var summary = new MappingEntity<ClassProgressEntity, StudentSummaryViewModel>()
                        .AutoOrtherType(_classProgressService.GetStudentResult(ClassID, StudentID) ?? new ClassProgressEntity
                        {
                            ClassID = ClassID,
                            StudentID = StudentID,
                            TotalLessons = _lessonService.CountClassLesson(ClassID),
                        }, new StudentSummaryViewModel()
                        {
                            ClassName = @class.Name,
                            Rank = -1,
                            TotalStudents = (int)total_students
                        });

                    var results = _classProgressService.GetClassResults(ClassID).OrderByDescending(t => t.TotalPoint).ToList();
                    if (results != null && (results.FindIndex(t => t.StudentID == StudentID) >= 0))
                        summary.Rank = results.FindIndex(t => t.TotalPoint == summary.TotalPoint) + 1;
                    summary.AvgPoint = examCount > 0 ? summary.TotalPoint / examCount : 0;

                    data.Add(summary);
                    data.AddRange(GetClassSubjectSummary(@class, StudentID, total_students));
                }
            }
            var response = new Dictionary<string, object>
            {
                { "Data", data},
                { "Model", model }
            };
            return new JsonResult(response);
        }


        private List<StudentSummaryViewModel> GetClassSubjectSummary(ClassEntity @class, string StudentID, long total_students)
        {
            var data = new List<StudentSummaryViewModel>();

            var subjects = _classSubjectService.GetByClassID(@class.ID);

            foreach (var sbj in subjects)
            {
                var examCount = _lessonScheduleService.CountClassSubjectExam(new List<string> { sbj.ID }, null);
                var prg = new MappingEntity<ClassSubjectProgressEntity, StudentSummaryViewModel>()
                    .AutoOrtherType(_classSubjectProgressService.GetItemByClassSubjectID(sbj.ID, StudentID) ?? new ClassSubjectProgressEntity
                    {
                        ClassID = @class.ID,
                        StudentID = StudentID,
                        ClassSubjectID = sbj.ID,
                        TotalLessons = _lessonService.CountClassSubjectLesson(sbj.ID)
                    }, new StudentSummaryViewModel()
                    {
                        SkillName = _skillService.GetItemByID(sbj.SkillID).Name,
                        Rank = -1,
                        TotalStudents = (int)total_students
                    });

                var results = _classSubjectProgressService.GetStudentResults(sbj.ID).OrderByDescending(t => t.TotalPoint).ToList();
                if (results != null && (results.FindIndex(t => t.StudentID == StudentID) >= 0))
                    prg.Rank = results.FindIndex(t => t.TotalPoint == prg.TotalPoint) + 1;
                prg.AvgPoint = examCount > 0 ? prg.TotalPoint / examCount : 0;

                data.Add(prg);
            }
            return data;
        }

        #endregion


        #region Fix Data
        #endregion
    }
}
