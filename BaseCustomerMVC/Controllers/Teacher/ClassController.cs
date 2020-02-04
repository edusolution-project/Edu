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
        private readonly SkillService _skillService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly CourseService _courseService;
        private readonly ClassProgressService _progressService;

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

        //private readonly CloneLessonPartService _cloneLessonPartService;
        //private readonly CloneLessonPartAnswerService _cloneAnswerService;
        //private readonly CloneLessonPartQuestionService _cloneQuestionService;

        //private readonly MappingEntity<LessonPartEntity, CloneLessonPartEntity> _lessonPartMapping;
        //private readonly MappingEntity<LessonPartQuestionEntity, CloneLessonPartQuestionEntity> _lessonPartQuestionMapping;
        //private readonly MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity> _lessonPartAnswerMapping;

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
            TeacherService teacherService,
            ClassService service,
            SkillService skillService,
            CourseService courseService,
            ClassProgressService progressService,

            ChapterService chapterService,
            ChapterExtendService chapterExtendService,
            LessonService lessonService,
            LessonScheduleService lessonScheduleService,
            ExamService examService,
            ExamDetailService examDetailService,
            LearningHistoryService learningHistoryService,

            ScoreStudentService scoreStudentService,
            LessonPartService lessonPartService,
            LessonPartQuestionService lessonPartQuestionService,
            LessonPartAnswerService lessonPartAnswerService,
            LessonProgressService lessonProgressService,

            CloneLessonPartService cloneLessonPartService,
            CloneLessonPartAnswerService cloneLessonPartAnswerService,
            CloneLessonPartQuestionService cloneLessonPartQuestionService,

            StudentService studentService, IHostingEnvironment evn,

            FileProcess fileProcess,

            ChapterProgressService chapterProgressService

            )
        {
            _accountService = accountService;
            _gradeService = gradeservice;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _courseService = courseService;
            _service = service;
            _skillService = skillService;
            _classSubjectService = classSubjectService;
            _progressService = progressService;

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

            _studentHelper = new StudentHelper(studentService, accountService);
            _lessonHelper = new LessonHelper(
                lessonService,
                lessonPartService,
                lessonPartQuestionService,
                lessonPartAnswerService,
                cloneLessonPartService,
                cloneLessonPartAnswerService,
                cloneLessonPartQuestionService
                );
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
                           let lessonProgress = _lessonProgressService.GetByClassID_StudentID_LessonID(currentClass.ID, student.ID, r.ID)
                           ?? new LessonProgressEntity()
                           //where lessonProgress != null
                           select _moduleViewMapping.AutoOrtherType(r, new StudentModuleViewModel()
                           {
                               ScheduleID = schedule.ID,
                               LessonStartDate = schedule.StartDate,
                               LessonEndDate = schedule.EndDate,
                               IsActive = schedule.IsActive,
                               LearningNumber = lessonProgress.TotalLearnt,
                               LessonName = r.Title,
                               TemplateType = r.TemplateType,
                               LearningEndDate = lessonProgress.LastDate
                           })).OrderBy(r => r.LessonStartDate).ThenBy(r => r.ChapterID).ThenBy(r => r.LessonId).ToList();
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
                               LessonStartDate = schedule.StartDate,
                               LessonEndDate = schedule.EndDate,
                               IsActive = schedule.IsActive,
                               LearningNumber = exam == null ? 0 : exam.Number,
                               LearningEndDate = exam == null ? DateTime.MinValue : exam.Updated,
                               Result = exam == null ? 0 : exam.Point
                           })).OrderBy(r => r.LessonStartDate).ThenBy(r => r.ChapterID).ThenBy(r => r.LessonId).ToList();
            ViewBag.Lessons = lessons;
            return View();
        }


        //Class Detail Management
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
                _chapterExtendService.Save(chapterExtend);

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
            var studentsView = students.ToList().Select(t => _mapping.AutoOrtherType(t, new ClassStudentViewModel()
            {
                ClassName = currentClass.Name,
                ClassStatus = "Đang học",
                LastJoinDate = DateTime.Now,
                Progress = _progressService.GetItemByClassID(currentClass.ID, t.ID),
                Score = _scoreStudentService.GetScoreStudentByStudentIdAndClassId(currentClass.ID, t.ID)
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


        public JsonResult GetActiveList(DateTime today)
        {
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
                       let progress = _progressService.GetItemByClassID(o.ID, userId)
                       let percent = progress == null || progress.TotalLessons == 0 ? 0 : progress.CompletedLessons.Count * 100 / progress.TotalLessons
                       let totalweek = (o.EndDate.Date - o.StartDate.Date).TotalDays / 7
                       let subject = _subjectService.GetItemByID(o.SubjectID)
                       select new
                       {
                           id = o.ID,
                           courseID = o.CourseID,
                           courseName = o.Name,
                           subjectName = subject == null ? "" : subject.Name,
                           thumb = o.Image ?? "",
                           endDate = o.EndDate,
                           //week = totalweek > 0 ? (DateTime.Now.Date - o.StartDate.Date).TotalDays / 7 / totalweek : 0,
                           students = o.Students.Count
                       }).ToList();
            return Json(new { Data = std });
        }

        public JsonResult GetFinishList(DefaultModel model, DateTime today)
        {
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
                       let progress = _progressService.GetItemByClassID(o.ID, userId)
                       let per = progress == null || progress.TotalLessons == 0 ? 0 : progress.CompletedLessons.Count * 100 / progress.TotalLessons
                       select new
                       {
                           id = o.ID,
                           courseID = o.CourseID,
                           title = o.Name,
                           endDate = o.EndDate,
                           per,
                           //score = progress != null ? progress.AvgPoint : 0
                       }).ToList();
            return Json(new { Data = std });
        }

        public JsonResult GetManageList(DefaultModel model, string SubjectID = "", string GradeID = "", string TeacherID = "", bool skipActive = true)
        {
            var filter = new List<FilterDefinition<ClassSubjectEntity>>();
            if (!string.IsNullOrEmpty(SubjectID))
            {
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.SubjectID == SubjectID));
            }
            if (!string.IsNullOrEmpty(GradeID))
            {
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.GradeID == GradeID));
            }
            if (!string.IsNullOrEmpty(TeacherID))
            {
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.TeacherID == TeacherID));
            }
            if (model.StartDate > new DateTime(1900, 1, 1))
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.EndDate >= model.StartDate));
            if (model.StartDate > new DateTime(1900, 1, 1))
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.StartDate <= model.EndDate));

            var data = _classSubjectService.Collection
                //.AsQueryable().
                //GroupBy(t => t.ClassID).Select(t => new ClassViewModel(t) {
                //    CourseName = 
                //})
                .Distinct(t => t.ClassID, filter.Count > 0 ? Builders<ClassSubjectEntity>.Filter.And(filter): Builders<ClassSubjectEntity>.Filter.Empty).ToList();
            model.TotalRecord = data.Count();
            var classData = _service.Collection.AsQueryable().Where(t => data.Contains(t.ID) && (t.IsActive || skipActive)).OrderByDescending(t => t.IsActive).ThenByDescending(t => t.ID).Skip(model.PageIndex * model.PageSize).Take(model.PageSize).ToList();
            var returndata = from o in classData
                                 //where o.Skills != null
                             let skillIDs = _classSubjectService.GetByClassID(o.ID).Select(t => t.SkillID).Distinct()
                             let sname = skillIDs == null ? "" : string.Join(", ", _skillService.GetList().Where(t => skillIDs.Contains(t.ID)).Select(t => t.Name).ToList())
                             select new Dictionary<string, object>
                             {
                                 { "ID", o.ID },
                                 { "Name", o.Name },
                                 { "Students", o.Students },
                                 { "Created", o.Created },
                                 { "IsActive", o.IsActive },
                                 { "Image", o.Image },
                                 { "StartDate", o.StartDate },
                                 { "EndDate", o.EndDate },
                                 { "Order", o.Order },
                                 { "Skills", o.Skills },
                                 { "Members", o.Members },
                                 { "Description", o.Description },
                                 { "SkillName", sname }
                             };

            var response = new Dictionary<string, object>
                {
                    { "Data", returndata.ToList()},
                    { "Model", model }
                };

            return new JsonResult(response);
        }

        public JsonResult GetThisWeekLesson(DateTime today)
        {
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
            classFilter.Add(Builders<ClassEntity>.Filter.Where(o => o.Members.Any(t=> t.TeacherID == userId)));
            var classIds = _service.Collection.Find(Builders<ClassEntity>.Filter.And(classFilter)).Project(t => t.ID).ToList();

            filter.Add(Builders<LessonScheduleEntity>.Filter.Where(t => classIds.Contains(t.ClassID)));

            var csIds = _lessonScheduleService.Collection.Distinct(t => t.ClassSubjectID, Builders<LessonScheduleEntity>.Filter.And(filter)).ToList();

            var data = _classSubjectService.Collection.Find(t => csIds.Contains(t.ID));
            //var std = (from o in data.ToList()
            //           let _lesson = _lessonService.Collection.Find(t => t.ID == o.LessonID).SingleOrDefault()
            //           where _lesson != null
            //           let _class = _service.Collection.Find(t => t.ID == o.ClassID).SingleOrDefault()
            //           where _class != null
            //           //let isLearnt = _learningHistoryService.GetLastLearnt(userId, o.LessonID) != null
            //           select new
            //           {
            //               id = o.ID,
            //               classID = _class.ID,
            //               className = _class.Name,
            //               title = _lesson.Title,
            //               lessonID = _lesson.ID,
            //               startDate = o.StartDate,
            //               endDate = o.EndDate,
            //               students = _class.Students.Count
            //               //isLearnt = isLearnt
            //           }).ToList();
            var std = (from o in data.ToList()
                       let _class = _service.Collection.Find(t => t.ID == o.ClassID).SingleOrDefault()
                       where _class != null
                       let skill = _skillService.GetItemByID(o.SkillID)
                       //let isLearnt = _learningHistoryService.GetLastLearnt(userId, o.LessonID) != null
                       select new
                       {
                           id = o.ID,
                           classID = _class.ID,
                           className = _class.Name,
                           endDate = o.EndDate,
                           students = _class.Students.Count,
                           skill = skill
                           //isLearnt = isLearnt
                       }).ToList();
            return Json(new { Data = std });
        }

        //public JsonResult GetActiveList()
        //{
        //    var filter = new List<FilterDefinition<ClassEntity>>();
        //    TeacherEntity teacher = null;
        //    var UserID = User.Claims.GetClaimByType("UserID").Value;
        //    if (!string.IsNullOrEmpty(UserID) && UserID != "0")
        //    {
        //        teacher = UserID == "0" ? null : _teacherService.GetItemByID(UserID);
        //        if (teacher == null)
        //        {
        //            return new JsonResult(new Dictionary<string, object> {
        //                {"Data",null },
        //                {"Error",UserID },
        //                {"Msg","Không có thông tin giảng viên" }
        //            });
        //        }
        //    }
        //    if (teacher != null)
        //        filter.Add(Builders<ClassEntity>.Filter.Where(o => o.TeacherID == UserID && o.EndDate >= DateTime.Now.ToLocalTime().Date));

        //    var data = filter.Count > 0 ? _service.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _service.GetAll();
        //    var DataResponse = data;
        //    if (data != null)
        //        DataResponse = data.Limit(3);

        //    var response = new Dictionary<string, object>
        //    {
        //        { "Data", DataResponse.ToList().Select(t=> _activeMapping.AutoOrtherType(t, new ClassActiveViewModel(){
        //            Progress = (int)((DateTime.Now.ToLocalTime().Date - t.StartDate.ToLocalTime().Date).TotalDays  * 100 / (t.EndDate.ToLocalTime().Date - t.StartDate.ToLocalTime().Date).TotalDays),
        //            SubjectName = _subjectService.GetItemByID(t.SubjectID).Name
        //            }))
        //            }
        //    };
        //    return new JsonResult(response);
        //}

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
                            {"Error", "Permission Error" }
                        });
                }
                item.TeacherID = userId; // creator
                item.Skills = new List<string>();
                item.Subjects = new List<string>();
                item.Members = new List<ClassMemberEntity>();
                item.IsActive = true;

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
                        //var subject = _subjectService.GetItemByID(csubject.SubjectID);
                        //if (subject == null)
                        //{
                        //    //throw new Exception("Subject " + csubject.SubjectID + " is not avaiable");
                        //    continue;
                        //}
                        //var course = _courseService.GetItemByID(csubject.CourseID);
                        //if (course == null || !course.IsActive)
                        //{
                        //    //throw new Exception("Course " + csubject.CourseID + " is not avaiable");
                        //    continue;
                        //}

                        //var teacher = _teacherService.GetItemByID(csubject.TeacherID);
                        //if (teacher == null || !teacher.IsActive || !teacher.Skills.Contains(csubject.SubjectID))
                        //{
                        //    //throw new Exception("Teacher " + csubject.TeacherID + " is not avaiable");
                        //    continue;
                        //}

                        //csubject.ClassID = item.ID;
                        //csubject.Description = course.Description;
                        //csubject.LearningOutcomes = course.LearningOutcomes;
                        //csubject.StartDate = item.StartDate;
                        //csubject.EndDate = item.EndDate;
                        ////subject.Image = course.Image;
                        //_classSubjectService.CreateOrUpdate(csubject);
                        //item.Skills.Add(subject.ID);
                        //item.Members.Add(new ClassMemberEntity
                        //{
                        //    Name = teacher.FullName,
                        //    TeacherID = teacher.ID,
                        //    Type = ClassMemberType.TEACHER
                        //});
                        ////Create Class => Create Lesson Schedule & Clone all lesson
                        //var lessons = _lessonService.CreateQuery().Find(o => o.CourseID == csubject.CourseID).ToList();

                        //var schedules = new List<LessonScheduleEntity>();
                        //if (lessons != null)
                        //    foreach (LessonEntity lesson in lessons)
                        //    {
                        //        _lessonScheduleService.CreateQuery().InsertOne(new LessonScheduleEntity
                        //        {
                        //            ClassID = item.ID,
                        //            ClassSubjectID = csubject.ID,
                        //            LessonID = lesson.ID,
                        //            IsActive = true
                        //        });
                        //        _lessonHelper.CloneLessonForClass(lesson, csubject);
                        //    }
                        //_courseService.Collection.UpdateOneAsync(t => t.ID == csubject.CourseID, new UpdateDefinitionBuilder<CourseEntity>().Set(t => t.IsUsed, true));
                        var newMember = new ClassMemberEntity();
                        var nID = CreateNewClassSubject(csubject, item, out newMember);
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
                oldData.StartDate = item.StartDate;
                oldData.EndDate = item.EndDate;
                oldData.Description = item.Description;
                _service.Save(oldData);

                oldData.Skills = new List<string>();
                oldData.Subjects = new List<string>();
                oldData.Members = new List<ClassMemberEntity>();
                var oldSubjects = _classSubjectService.GetByClassID(item.ID);
                if (oldSubjects != null)
                {
                    foreach (var oSbj in oldSubjects)
                    {
                        var nSbj = classSubjects.Find(t => t.ID == oSbj.ID);
                        if (nSbj == null || (nSbj.CourseID != oSbj.CourseID))
                        //delete oldSubject
                        {
                            ////remove old schedule
                            _lessonScheduleService.CreateQuery().DeleteMany(o => o.ClassSubjectID == oSbj.ID);
                            //remove clone lesson part
                            _lessonHelper.RemoveCloneClassSubject(oSbj.ID);
                            //remove progress: learning history => class progress, chapter progress, lesson progress
                            _learningHistoryService.RemoveClassSubjectHistory(oSbj.ID);
                            //resest exam
                            _examService.RemoveClassSubjectExam(oSbj.ID);
                            if (nSbj == null)
                                _classSubjectService.Remove(oSbj.ID);
                        }

                        if (nSbj != null)
                        {
                            var newMember = new ClassMemberEntity();
                            if (nSbj.CourseID != oSbj.CourseID)
                            {
                                nSbj.ID = CreateNewClassSubject(nSbj, item, out newMember);
                                if (string.IsNullOrEmpty(nSbj.ID))//Error
                                    continue;
                            }
                            else //Not change
                            {
                                //update period
                                oSbj.StartDate = item.StartDate;
                                oSbj.EndDate = item.EndDate;
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
                        var nID = CreateNewClassSubject(nSbj, item, out newMember);
                        if (string.IsNullOrEmpty(nSbj.ID))//Error
                            continue;

                        if (!oldData.Skills.Contains(nSbj.SkillID))
                            oldData.Skills.Add(nSbj.SkillID);
                        if (!oldData.Subjects.Contains(nSbj.SubjectID))
                            oldData.Subjects.Add(nSbj.SubjectID);
                        if (!oldData.Members.Any(t => t.TeacherID == newMember.TeacherID && t.Type == ClassMemberType.TEACHER))
                            oldData.Members.Add(newMember);
                    }
                }

                //update data
                _service.Save(oldData);

                //_courseService.Collection.UpdateOneAsync(t => t.ID == item.CourseID, new UpdateDefinitionBuilder<CourseEntity>().Set(t => t.IsUsed, true));

                Dictionary<string, object> response = new Dictionary<string, object>()
                {
                    {"Data",item },
                    {"Error",null },
                    {"Msg","Success" }
                };
                return new JsonResult(response);
            }
        }

        private string CreateNewClassSubject(ClassSubjectEntity nSbj, ClassEntity @class, out ClassMemberEntity member)
        {
            member = new ClassMemberEntity();
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

                _classSubjectService.Save(nSbj);

                //Clone Lesson
                var lessons = _lessonService.CreateQuery().Find(o => o.CourseID == nSbj.CourseID).ToList();

                if (lessons != null)
                    foreach (LessonEntity lesson in lessons)
                    {
                        var schedule = new LessonScheduleEntity
                        {
                            ClassID = @class.ID,
                            ClassSubjectID = nSbj.ID,
                            LessonID = lesson.ID,
                            IsActive = true
                        };
                        _lessonScheduleService.Save(schedule);
                        //_calendarHelper.ConvertCalendarFromSchedule(schedule, "");

                        _lessonHelper.CloneLessonForClass(lesson, nSbj);
                    }

                _courseService.Collection.UpdateOneAsync(t => t.ID == nSbj.CourseID, new UpdateDefinitionBuilder<CourseEntity>().Set(t => t.IsUsed, true));

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
                    //remove Schedule, Part, Question, Answer
                    _lessonScheduleService.CreateQuery().DeleteMany(o => ids.Contains(o.ClassID));
                    _lessonHelper.RemoveClone(ids);
                    _examService.Collection.DeleteMany(o => ids.Contains(o.ClassID));
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

        public IActionResult ConvertMultiSubject()
        {
            var allClass = _service.GetAll().ToList();
            foreach (var @class in allClass)
            {
                if (@class.Skills == null || @class.Skills.Count == 0)
                {
                    //create class subject:
                    var teacher = _teacherService.GetItemByID(@class.TeacherID);
                    if (teacher == null)
                    {
                        //Delete Class
                        _lessonScheduleService.CreateQuery().DeleteMany(o => o.ClassID == @class.ID);
                        _lessonHelper.RemoveClone(@class.ID);
                        _examService.Collection.DeleteMany(o => o.ClassID == @class.ID);
                        _examDetailService.Collection.DeleteMany(o => o.ClassID == @class.ID);
                        var delete = _service.Collection.DeleteMany(o => o.ID == @class.ID);
                    }
                    @class.Members = new List<ClassMemberEntity>
                    {
                        new ClassMemberEntity
                        {
                            TeacherID = teacher.ID,
                            Name = teacher.FullName,
                            Type = ClassMemberType.TEACHER
                        }
                    };
                    var subject = _subjectService.GetItemByID(@class.SubjectID);
                    if (subject == null)
                    {
                        //Delete Class
                        _lessonScheduleService.CreateQuery().DeleteMany(o => o.ClassID == @class.ID);
                        _lessonHelper.RemoveClone(@class.ID);
                        _examService.Collection.DeleteMany(o => o.ClassID == @class.ID);
                        _examDetailService.Collection.DeleteMany(o => o.ClassID == @class.ID);
                        var delete = _service.Collection.DeleteMany(o => o.ID == @class.ID);
                    }
                    var course = _courseService.GetItemByID(@class.CourseID);
                    if (course == null)
                    {
                        //Delete Class
                        _lessonScheduleService.CreateQuery().DeleteMany(o => o.ClassID == @class.ID);
                        _lessonHelper.RemoveClone(@class.ID);
                        _examService.Collection.DeleteMany(o => o.ClassID == @class.ID);
                        _examDetailService.Collection.DeleteMany(o => o.ClassID == @class.ID);
                        var delete = _service.Collection.DeleteMany(o => o.ID == @class.ID);
                    }
                    //create classSubject
                    var classSubject = new ClassSubjectEntity
                    {
                        ClassID = @class.ID,
                        CourseID = course.ID,
                        GradeID = @class.GradeID,
                        TeacherID = @class.TeacherID,
                        EndDate = @class.EndDate,
                        StartDate = @class.StartDate,
                        Image = course.Image,
                        LearningOutcomes = course.LearningOutcomes,
                        Description = course.Description,
                        SubjectID = subject.ID
                    };
                    _classSubjectService.Save(classSubject);
                    //Save Class
                    _service.Save(@class);
                    //Convert Progress
                    _ = _chapterProgressService.UpdateClassSubject(classSubject);
                    _ = _learningHistoryService.UpdateClassSubject(classSubject);
                    _ = _lessonProgressService.UpdateClassSubject(classSubject);
                    //Convert Schedule
                    _ = _lessonScheduleService.UpdateClassSubject(classSubject);
                    //Convert Clone Part
                    _ = _lessonHelper.ConvertClassSubject(classSubject);
                    //Convert Exam
                    _ = _examService.ConvertClassSubject(classSubject);
                    _ = _examDetailService.ConvertClassSubject(classSubject);
                    //Convert Score
                    _ = _scoreStudentService.UpdateClassSubject(classSubject);
                }
            }
            return null;
        }

        public IActionResult ConvertSkills()
        {
            var courses = _courseService.GetAll().ToList();
            foreach (var course in courses)
            {
                if (course.SkillID == null)
                {
                    var name = course.Name.ToLower();
                    if (name.IndexOf("listen") >= 0 || name.IndexOf("nghe") >= 0)
                    {
                        course.SkillID = "1";
                    }
                    else if (name.IndexOf("speak") >= 0)
                    {
                        course.SkillID = "2";
                    }
                    else if (name.IndexOf("read") >= 0)
                    {
                        course.SkillID = "3";
                    }
                    else if (name.IndexOf("writ") >= 0)
                    {
                        course.SkillID = "4";
                    }
                    else if (name.IndexOf("voca") >= 0)
                    {
                        course.SkillID = "5";
                    }
                    else if (name.IndexOf("gramma") >= 0)
                    {
                        course.SkillID = "6";
                    }
                    else
                        course.SkillID = "7";
                }
                _courseService.Save(course);
                _classSubjectService.UpdateCourseSkill(course.ID, course.SkillID);
            }
            return null;
        }
    }
}
