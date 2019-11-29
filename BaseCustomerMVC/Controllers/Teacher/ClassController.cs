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
        private readonly AccountService _accountService;
        private readonly SubjectService _subjectService;
        private readonly TeacherService _teacherService;
        private readonly ClassService _service;
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

        private readonly MappingEntity<StudentEntity, ClassMemberViewModel> _mapping;
        private readonly MappingEntity<ClassEntity, ClassActiveViewModel> _activeMapping;
        private readonly IHostingEnvironment _env;


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
            TeacherService teacherService,
            ClassService service,
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

            FileProcess fileProcess)
        {
            _accountService = accountService;
            _gradeService = gradeservice;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _courseService = courseService;
            _service = service;
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
            _mapping = new MappingEntity<StudentEntity, ClassMemberViewModel>();
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
            ViewBag.Managable = CheckPermission(PERMISSION.COURSE_EDIT);
            return View();
        }

        public IActionResult Detail(DefaultModel model)
        {
            if (model == null) return null;
            var currentClass = _service.GetItemByID(model.ID);
            if (currentClass == null)
                return RedirectToAction("Index");
            ViewBag.Class = currentClass;
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
                    { "Data", null },
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

        //Class Management
        [HttpPost]
        public JsonResult GetList(DefaultModel model, string SubjectID = "", string GradeID = "", string UserID = "")
        {
            var filter = new List<FilterDefinition<ClassEntity>>();
            filter.Add(Builders<ClassEntity>.Filter.Where(o => o.IsActive));
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
                        {"Msg","Teacher not found" }
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
            model.TotalRecord = data.CountDocuments();
            var DataResponse = data == null || model.TotalRecord <= 0 || model.TotalRecord < model.PageSize
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

        public JsonResult GetManageList(DefaultModel model, string SubjectID = "", string GradeID = "", string TeacherID = "", string UserID = "")
        {
            var filter = new List<FilterDefinition<ClassEntity>>();
            if (!string.IsNullOrEmpty(SubjectID))
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.SubjectID == SubjectID));
            }
            if (!string.IsNullOrEmpty(GradeID))
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.GradeID == GradeID));
            }
            if (!string.IsNullOrEmpty(TeacherID))
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.TeacherID == TeacherID));
            }

            var data = filter.Count > 0 ? _service.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _service.GetAll();

            var response = new Dictionary<string, object>
            {
                { "Data", data.SortByDescending(t=> t.IsActive).ThenByDescending(t=> t.ID).ToList().Select(o=> new ClassViewModel(o){
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
        [Obsolete]
        public JsonResult Create(ClassEntity item)
        {
            if (string.IsNullOrEmpty(item.ID) || item.ID == "0")
            {
                item.ID = null;
                item.Created = DateTime.Now;

                if (!String.IsNullOrEmpty(item.Code))
                {
                    if (_service.CreateQuery().Find(t => t.Code == item.Code).FirstOrDefault() != null)
                        return new JsonResult(new Dictionary<string, object>()
                        {
                            {"Error", "Class code used" }
                        });
                }
                var course = _courseService.GetItemByID(item.CourseID);
                if (course == null || !course.IsActive)
                    return new JsonResult(new Dictionary<string, object>()
                        {
                            {"Error", "Curriculum is not available" }
                        });
                item.Description = course.Description;
                item.LearningOutcomes = course.LearningOutcomes;
                item.Image = course.Image;
                _service.CreateOrUpdate(item);

                //Create Class => Create Lesson Schedule & Clone all lesson
                var lessons = _lessonService.CreateQuery().Find(o => o.CourseID == item.CourseID).ToList();

                var schedules = new List<LessonScheduleEntity>();
                if (lessons != null)
                    foreach (LessonEntity lesson in lessons)
                    {
                        _lessonScheduleService.CreateQuery().InsertOne(new LessonScheduleEntity
                        {
                            ClassID = item.ID,
                            LessonID = lesson.ID,
                            IsActive = true
                        });
                        _lessonHelper.CloneLessonForClass(lesson, item);
                    }

                _courseService.Collection.UpdateOneAsync(t => t.ID == item.CourseID, new UpdateDefinitionBuilder<CourseEntity>().Set(t => t.IsUsed, true));

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
                var oldData = _service.GetItemByID(item.ID);
                if (oldData == null) return new JsonResult(new Dictionary<string, object>()
                {
                    {"Error", "Data not correct" }
                });
                if (item.Code != oldData.Code)
                {
                    if (_service.CreateQuery().Find(t => t.Code == item.Code).FirstOrDefault() != null)
                        return new JsonResult(new Dictionary<string, object>()
                        {
                            {"Error", "Class code used" }
                        });
                }

                oldData.Updated = DateTime.Now;


                if (item.CourseID != oldData.CourseID)
                {
                    //remove old schedule
                    _lessonScheduleService.CreateQuery().DeleteMany(o => o.ClassID == item.ID);
                    //remove clone lesson part
                    _lessonHelper.RemoveClone(item.ID);
                    //remove progress: learning history => class progress, chapter progress, lesson progress
                    _learningHistoryService.RemoveClassHistory(item.ID);
                    //resest exam
                    _examService.RemoveClassExam(item.ID);

                    //Create Class => Create Lesson Schedule & Clone all lesson
                    var lessons = _lessonService.CreateQuery().Find(o => o.CourseID == item.CourseID).ToList();

                    if (lessons != null)
                        foreach (LessonEntity lesson in lessons)
                        {
                            var schedule = new LessonScheduleEntity
                            {
                                ClassID = item.ID,
                                LessonID = lesson.ID,
                                IsActive = true
                            };
                            _lessonScheduleService.CreateOrUpdate(schedule);
                            //_calendarHelper.ConvertCalendarFromSchedule(schedule, "");

                            _lessonHelper.CloneLessonForClass(lesson, item);
                        }
                }

                oldData.Name = item.Name;
                oldData.Code = item.Code;
                oldData.StartDate = item.StartDate;
                oldData.EndDate = item.EndDate;
                oldData.CourseID = item.CourseID;
                oldData.GradeID = item.GradeID;
                oldData.TeacherID = item.TeacherID;

                _service.CreateOrUpdate(oldData);

                _courseService.Collection.UpdateOneAsync(t => t.ID == item.CourseID, new UpdateDefinitionBuilder<CourseEntity>().Set(t => t.IsUsed, true));

                Dictionary<string, object> response = new Dictionary<string, object>()
                {
                    {"Data",item },
                    {"Error",null },
                    {"Msg","Success" }
                };
                return new JsonResult(response);
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
    }
}
