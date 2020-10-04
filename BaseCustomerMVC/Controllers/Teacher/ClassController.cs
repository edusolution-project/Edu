﻿using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using MongoDB.Driver;
using System.Linq;
using Core_v2.Globals;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Serializers;
using BaseEasyRealTime.Entities;
using System.Drawing;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class ClassController : TeacherController
    {
        private readonly GradeService _gradeService;
        private readonly AccountService _accountService;
        private readonly SubjectService _subjectService;
        private readonly TeacherService _teacherService;
        private readonly ClassService _service;
        //private readonly ClassStudentService _classStudentService;
        private readonly SkillService _skillService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly CourseService _courseService;
        private readonly CourseHelper _courseHelper;
        private readonly ClassHelper _classHelper;
        private readonly LessonHelper _lessonHelper;

        private readonly ClassProgressService _classProgressService;
        private readonly ClassSubjectProgressService _classSubjectProgressService;
        private readonly ProgressHelper _progressHelper;

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

        private readonly ExamService _examService;
        private readonly ExamDetailService _examDetailService;

        private readonly FileProcess _fileProcess;
        private readonly StudentHelper _studentHelper;
        private readonly TeacherHelper _teacherHelper;
        private readonly MailHelper _mailHelper;
        private readonly MappingEntity<LessonEntity, StudentModuleViewModel> _moduleViewMapping;
        private readonly MappingEntity<LessonEntity, StudentAssignmentViewModel> _assignmentViewMapping;

        private readonly CenterService _centerService;
        private readonly RoleService _roleService;
        private readonly CloneLessonPartService _cloneLessonPartService;
        private readonly CloneLessonPartAnswerService _cloneLessonPartAnswerService;
        private readonly CloneLessonPartQuestionService _cloneLessonPartQuestionService;
        private readonly LessonPartService _lessonPartService;
        private readonly LessonPartQuestionService _lessonPartQuestionService;

        private readonly GroupService _groupService;

        private readonly List<string> quizType = new List<string> { "QUIZ1", "QUIZ2", "QUIZ3", "QUIZ4", "ESSAY" };
        private readonly CourseChapterService _courseChapterService;
        private readonly CourseLessonService _courseLessonService;
        private readonly MappingEntity<CourseChapterEntity, CourseChapterEntity> _cloneCourseChapterMapping = new MappingEntity<CourseChapterEntity, CourseChapterEntity>();
        private readonly MappingEntity<CourseLessonEntity, CourseLessonEntity> _cloneCourseLessonMapping = new MappingEntity<CourseLessonEntity, CourseLessonEntity>();
        private readonly LessonPartAnswerService _lessonPartAnswerService;
        private readonly MappingEntity<CourseEntity, CourseEntity> _cloneCourseMapping = new MappingEntity<CourseEntity, CourseEntity>();
        private readonly ClassService _classService;

        public ClassController(
            AccountService accountService,
            GradeService gradeservice,
            SubjectService subjectService,
            ClassSubjectService classSubjectService,
            //ClassStudentService classStudentService,
            TeacherService teacherService,
            ClassService service,
            SkillService skillService,
            CourseService courseService,
            CourseHelper courseHelper,
            ClassHelper classHelper,
            ClassProgressService classProgressService,
            ClassSubjectProgressService classSubjectProgressService,
            ProgressHelper progressHelper,

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
            TeacherHelper teacherHelper,
            MailHelper mailHelper,

            ChapterProgressService chapterProgressService,
            CenterService centerService
            , CloneLessonPartService cloneLessonPartService
            , CloneLessonPartAnswerService cloneLessonPartAnswerService
            , CloneLessonPartQuestionService cloneLessonPartQuestionService
            , LessonPartService lessonPartService
            , LessonPartQuestionService lessonPartQuestionService,
            RoleService roleService,
            GroupService groupService,
            CourseChapterService courseChapterService,
            CourseLessonService courseLessonService,
            LessonPartAnswerService lessonPartAnswerService,
            ClassService classService
            )
        {
            _accountService = accountService;
            _gradeService = gradeservice;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _courseService = courseService;
            _courseHelper = courseHelper;
            _classHelper = classHelper;
            _service = service;
            _skillService = skillService;
            _classSubjectService = classSubjectService;
            //_classStudentService = classStudentService;
            _classProgressService = classProgressService;
            _classSubjectProgressService = classSubjectProgressService;
            _progressHelper = progressHelper;

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
            _teacherHelper = teacherHelper;
            _lessonHelper = lessonHelper;
            _mailHelper = mailHelper;

            _moduleViewMapping = new MappingEntity<LessonEntity, StudentModuleViewModel>();
            _assignmentViewMapping = new MappingEntity<LessonEntity, StudentAssignmentViewModel>();

            _chapterProgressService = chapterProgressService;
            _centerService = centerService;
            _roleService = roleService;
            _cloneLessonPartService = cloneLessonPartService;
            _cloneLessonPartQuestionService = cloneLessonPartQuestionService;
            _lessonPartService = lessonPartService;
            _lessonPartQuestionService = lessonPartQuestionService;
            _cloneLessonPartAnswerService = cloneLessonPartAnswerService;
            _groupService = groupService;
            _courseChapterService = courseChapterService;
            _courseLessonService = courseLessonService;
            _lessonPartAnswerService = lessonPartAnswerService;
            _classService = classService;
        }

        public IActionResult Index(DefaultModel model, string basis, int old = 0)
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
            var center = new CenterEntity();
            if (!string.IsNullOrEmpty(basis))
            {
                center = _centerService.GetItemByCode(basis);
                if (center != null)
                    ViewBag.Center = center;
            }
            ViewBag.IsHeadTeacher = _teacherHelper.HasRole(UserID, center.ID, "head-teacher");

            ViewBag.User = UserID;
            ViewBag.Model = model;
            ViewBag.Managable = CheckPermission(PERMISSION.COURSE_EDIT);
            if (old == 1)
                return View("Index_o");
            return View();
        }

        public IActionResult Detail(DefaultModel model, string basis)
        {
            //if (!string.IsNullOrEmpty(basis))
            //{
            //    var center = _centerService.GetItemByCode(basis);
            //    if (center != null)
            //        ViewBag.Center = center;
            //    var UserID = User.Claims.GetClaimByType("UserID").Value;
            //    ViewBag.IsHeadTeacher = _teacherHelper.HasRole(UserID, center.ID, "head-teacher");
            //}

            if (model == null) return null;
            var currentClass = _service.GetItemByID(model.ID);
            if (currentClass == null)
                return Redirect($"/{basis}{Url.Action("Index")}");
            var vm = new ClassViewModel(currentClass);
            var subjects = _classSubjectService.GetByClassID(currentClass.ID);
            var skillIDs = subjects.Select(t => t.SkillID).Distinct();
            var subjectIDs = subjects.Select(t => t.SubjectID).Distinct();
            vm.SkillName = string.Join(", ", _skillService.GetList().Where(t => skillIDs.Contains(t.ID)).Select(t => t.Name).ToList());
            vm.SubjectName = string.Join(", ", _subjectService.Collection.Find(t => subjectIDs.Contains(t.ID)).Project(t => t.Name).ToList());
            vm.TotalStudents = _studentService.CountByClass(currentClass.ID);
            ViewBag.Class = vm;
            //ViewBag.Subject = _subjectService.GetItemByID(currentClass.SubjectID);
            //ViewBag.Grade = _gradeService.GetItemByID(currentClass.GradeID);

            var UserID = User.Claims.GetClaimByType("UserID").Value;
            var teacher = _teacherService.CreateQuery().Find(t => t.ID == UserID).SingleOrDefault();
            if (teacher != null && teacher.Subjects != null)
            {
                var listsubjects = _subjectService.CreateQuery().Find(t => teacher.Subjects.Contains(t.ID)).ToList();
                var listgrades = _gradeService.CreateQuery().Find(t => teacher.Subjects.Contains(t.SubjectID)).ToList();
                ViewBag.Grades = listgrades;
                ViewBag.Subjects = listsubjects;
            }
            return View();
        }

        //public IActionResult References(DefaultModel model)
        //{
        //    if (model == null) return null;
        //    var currentClass = _service.GetItemByID(model.ID);
        //    if (currentClass == null)
        //        return Redirect($"/{basis}{Url.Action("Index");
        //    ViewBag.Class = currentClass;
        //    var UserID = User.Claims.GetClaimByType("UserID").Value;
        //    var myClasses = _service.CreateQuery()
        //        .Find(t => t.TeacherID == UserID)
        //        //.Find(t=> true)
        //        //.Project(Builders<ClassEntity>.Projection.Include(t => t.ID).Include(t => t.Name))
        //        .ToList();
        //    ViewBag.AllClass = myClasses;
        //    ViewBag.User = UserID;
        //    return View();
        //}

        public IActionResult Modules(DefaultModel model, string basis)
        {
            if (!string.IsNullOrEmpty(basis))
            {
                var center = _centerService.GetItemByCode(basis);
                if (center != null)
                    ViewBag.Center = center;
            }
            if (model == null) return null;
            var currentClass = _service.GetItemByID(model.ID);
            if (currentClass == null)
                return Redirect($"/{basis}{Url.Action("Index")}");
            ViewBag.Class = currentClass;
            return View();
        }

        public IActionResult StudentDetail(string basis, string ID, string ClassID)
        {
            if (!string.IsNullOrEmpty(basis))
            {
                var center = _centerService.GetItemByCode(basis);
                if (center != null)
                    ViewBag.Center = center;
            }
            if (string.IsNullOrEmpty(ClassID))
                return Redirect($"/{basis}{Url.Action("Index")}");
            var currentClass = _service.GetItemByID(ClassID);
            if (currentClass == null)
                return Redirect($"/{basis}{Url.Action("Index")}");
            ViewBag.Class = currentClass;

            if (string.IsNullOrEmpty(ID))
                return Redirect($"/{basis}{Url.Action("Member", "Class", new { ID = ClassID })}");

            var student = _studentService.GetItemByID(ID);
            if (student == null)
                return Redirect($"/{basis}{Url.Action("Member", "Class", new { ID = ClassID })}");

            ViewBag.Student = student;

            return View();
        }

        #region ClassDetail

        public JsonResult GetListTeacher(string basis, string SubjectID = "")
        {
            var filter = new List<FilterDefinition<TeacherEntity>>();

            var UserID = User.Claims.GetClaimByType("UserID").Value;

            if (string.IsNullOrEmpty(SubjectID))
                return new JsonResult(new Dictionary<string, object> { });
            filter.Add(Builders<TeacherEntity>.Filter.Where(o => o.Centers.Any(c => c.Code == basis)));
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

            var results = _progressHelper.GetClassResults(@class.ID)
                .OrderByDescending(t => t.RankPoint).ToList();

            foreach (var student in _studentService.GetStudentsByClassId(@class.ID))
            {
                var summary = new MappingEntity<ClassProgressEntity, StudentSummaryViewModel>()
                    .AutoOrtherType(_classProgressService.GetStudentResult(@class.ID, student.ID) ?? new ClassProgressEntity()
                    {
                        ClassID = @class.ID,
                        StudentID = student.ID
                    }, new StudentSummaryViewModel()
                    {
                        Rank = -1,
                        TotalStudents = (int)total_students
                    });

                summary.ClassName = @class.Name;
                summary.FullName = student.FullName;
                summary.RankPoint = _progressHelper.CalculateRankPoint(summary.TotalPoint, summary.PracticePoint, summary.Completed);

                summary.Rank = results.FindIndex(t => t.RankPoint == summary.RankPoint) + 1;
                summary.ExamResult = @class.TotalExams > 0 ? summary.TotalPoint / @class.TotalExams : 0;
                summary.PracticeResult = @class.TotalPractices > 0 ? summary.PracticePoint / @class.TotalPractices : 0;

                summary.TotalExams = @class.TotalExams;
                summary.TotalLessons = @class.TotalLessons;
                summary.TotalPractices = @class.TotalPractices;

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

        #region Homepage
        public JsonResult GetActiveList(DateTime today, string Center)
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
                    StatusDesc = "Phiên làm việc đã kết thúc, yêu cầu đăng nhập lại"
                });
            }
            if (!string.IsNullOrEmpty(Center))
            {
                var @center = _centerService.GetItemByCode(Center);
                if (@center == null) return new JsonResult(new Dictionary<string, object>
                {
                    { "Error", "Cơ sở không đúng"}
                });
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Center == @center.ID));
            }
            filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Members.Any(t => t.TeacherID == userId && t.Type == ClassMemberType.TEACHER)));
            filter.Add(Builders<ClassEntity>.Filter.Where(o => (o.StartDate <= today) && (o.EndDate >= today)));

            var data = (filter.Count > 0 ? _service.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _service.GetAll()).SortByDescending(t => t.ID);

            var std = (from o in data.ToList()
                       let totalweek = (o.EndDate.Date - o.StartDate.Date).TotalDays / 7
                       //let subject = _subjectService.GetItemByID(o.SubjectID)
                       let studentCount = //_classStudentService.GetClassStudents(o.ID).Count
                       _studentService.CountByClass(o.ID)
                       select new
                       {
                           id = o.ID,
                           //courseID = o.CourseID,
                           courseName = o.Name,
                           //subjectName = subject == null ? "" : subject.Name,
                           thumb = o.Image ?? "",
                           endDate = o.EndDate,
                           //week = totalweek > 0 ? (DateTime.Now.Date - o.StartDate.Date).TotalDays / 7 / totalweek : 0,
                           students = studentCount
                       }).ToList();
            return Json(new { Data = std });
        }

        public JsonResult GetFinishList(DefaultModel model, DateTime today, string Center)
        {
            today = today.ToUniversalTime();
            var filter = new List<FilterDefinition<ClassEntity>>();
            filter.Add(Builders<ClassEntity>.Filter.Where(o => o.IsActive));
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            if (!string.IsNullOrEmpty(Center))
            {
                var @center = _centerService.GetItemByCode(Center);
                if (@center == null) return new JsonResult(new Dictionary<string, object>
                {
                    { "Error", "Cơ sở không đúng"}
                });
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Center == @center.ID));
            }
            filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Members.Any(t => t.TeacherID == userId && t.Type == ClassMemberType.TEACHER)));
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
                           //courseID = o.CourseID,
                           title = o.Name,
                           endDate = o.EndDate,
                       }).ToList();
            return Json(new { Data = std });
        }

        public JsonResult GetThisWeekLesson(DateTime today, string Center)
        {
            today = today.ToUniversalTime();
            if (today < new DateTime(1900, 1, 1))
                return Json(new { });
            var startWeek = today.AddDays(DayOfWeek.Sunday - today.DayOfWeek);
            var endWeek = startWeek.AddDays(7);

            var filter = new List<FilterDefinition<LessonScheduleEntity>>();
            //filter.Add(Builders<LessonScheduleEntity>.Filter.Where(o => o.IsActive));
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
            if (!string.IsNullOrEmpty(Center))
            {
                var @center = _centerService.GetItemByCode(Center);
                if (@center == null) return new JsonResult(new Dictionary<string, object>
                {
                    { "Error", "Cơ sở không đúng"}
                });
                classFilter.Add(Builders<ClassEntity>.Filter.Where(o => o.Members.Any(t => t.TeacherID == userId) && o.Center == @center.ID));
            }

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

            return Json(new { Data = std });
        }
        #endregion

        #region Manage
        public JsonResult GetManageList(DefaultModel model, string Center, string SubjectID = "", string GradeID = "", string TeacherID = "", bool skipActive = true)
        {
            var center = new CenterEntity();
            if (!string.IsNullOrEmpty(Center))
            {
                center = _centerService.GetItemByCode(Center);
                if (@center == null) return new JsonResult(new Dictionary<string, object>
                {
                    { "Error", "Cơ sở không đúng"}
                });
            }

            var returndata = FilterClass(model, center.ID, SubjectID, GradeID, "", skipActive, true);
            //model.TotalRecord = totalrec;

            var response = new Dictionary<string, object>
                {
                    { "Data", returndata},
                    { "Model", model }
                };
            return new JsonResult(response);
        }

        public JsonResult GetClassList(DefaultModel model, string Center, string SubjectID = "", string GradeID = "")
        {
            var center = new CenterEntity();


            if (!string.IsNullOrEmpty(Center))
            {
                center = _centerService.GetItemByCode(Center);
                if (@center == null) return new JsonResult(new Dictionary<string, object>
                {
                    { "Error", "Cơ sở không đúng"}
                });

            }

            var returndata = FilterClass(model, center.ID, SubjectID, GradeID, User.Claims.GetClaimByType("UserID").Value, true);
            //model.TotalRecord = totalrec;

            var response = new Dictionary<string, object>
                {
                    { "Data", returndata},
                    { "Model", model }
                };
            return new JsonResult(response);
        }

        private List<Dictionary<string, object>> FilterClass(DefaultModel model, string Center, string SubjectID = "", string GradeID = "", string TeacherID = "", bool skipActive = true, bool isManager = false)
        {
            model.TotalRecord = 0;
            var filter = new List<FilterDefinition<ClassSubjectEntity>>();
            var classfilter = new List<FilterDefinition<ClassEntity>>();

            var skip_owned = false;
            FilterDefinition<ClassEntity> ownerfilter = null;
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            var teacher = _teacherService.GetItemByID(UserID);

            if (!string.IsNullOrEmpty(SubjectID))
            {
                skip_owned = true;
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.SubjectID == SubjectID));
            }
            else
            {
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => teacher.Subjects.Contains(o.SubjectID)));
            }
            if (!string.IsNullOrEmpty(GradeID))
            {
                skip_owned = true;
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.GradeID == GradeID));
            }

            if (model.StartDate > new DateTime(1900, 1, 1))
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.EndDate >= model.StartDate));

            if (model.StartDate > new DateTime(1900, 1, 1))
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.StartDate <= model.EndDate));


            var data = new List<string>();
            if (filter.Count > 0)
            {
                var dCursor = _classSubjectService.Collection
                .Distinct(t => t.ClassID, Builders<ClassSubjectEntity>.Filter.And(filter));
                data = dCursor.ToList();
            }

            if (!string.IsNullOrEmpty(TeacherID))
            {
                if (!skip_owned)
                    classfilter.Add(Builders<ClassEntity>.Filter.Where(o => o.Members.Any(t => t.TeacherID == TeacherID && t.Type == ClassMemberType.TEACHER)));
                else
                    classfilter.Add(Builders<ClassEntity>.Filter.Where(o => o.Members.Any(t => t.TeacherID == TeacherID)));
            }

            if (!skipActive)
                classfilter.Add(Builders<ClassEntity>.Filter.Where(o => o.IsActive));

            if (data.Count > 0)
                classfilter.Add(Builders<ClassEntity>.Filter.Where(t => data.Contains(t.ID)));

            if (!string.IsNullOrEmpty(model.SearchText))
                classfilter.Add(Builders<ClassEntity>.Filter.Text(
                    //"\"" + 
                    model.SearchText
                    //+ "\""
                    ));

            if (classfilter.Count == 0)
                return null;

            var classResult = _service.Collection.Find(
                Builders<ClassEntity>.Filter.And(
                    Builders<ClassEntity>.Filter.Where(o => o.Center == Center && o.ClassMechanism != CLASS_MECHANISM.PERSONAL),
                    Builders<ClassEntity>.Filter.And(classfilter)
                ));

            model.TotalRecord = classResult.CountDocuments();

            var classData = classResult.SortByDescending(t => t.IsActive).ThenByDescending(t => t.StartDate).Skip(model.PageIndex * model.PageSize).Limit(model.PageSize).ToList();
            var returndata = from o in classData
                             let skillIDs = _classSubjectService.GetByClassID(o.ID).Select(t => t.SkillID).Distinct()
                             let creator = _teacherService.GetItemByID(o.TeacherID) //Todo: Fix
                             let sname = skillIDs == null ? "" : string.Join(", ", _skillService.GetList().Where(t => skillIDs.Contains(t.ID)).Select(t => t.Name).ToList())
                             let teachers = (o.Members == null || o.Members.Count == 0) ? "" : string.Join(", ", o.Members.Where(t => t.Type != ClassMemberType.OWNER).Select(t => t.TeacherID).Distinct().Select(m => _teacherService.GetItemByID(m)?.FullName))
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
                                 { "Teachers", teachers },
                                 { "Creator", o.TeacherID },
                                 { "CreatorName", creator==null?"":creator.FullName },
                                 { "ClassMechanism", o.ClassMechanism }
                             };
            return returndata.ToList();
        }

        [HttpPost]
        [Obsolete]
        public JsonResult Create(ClassEntity item, string CenterCode, List<ClassSubjectEntity> classSubjects, IFormFile fileUpload)
        {
            var userId = User.Claims.GetClaimByType("UserID").Value;

            if (string.IsNullOrEmpty(userId))
            {
                return new JsonResult(new Dictionary<string, object>()
                        {
                            {"Error", "Vui lòng đăng nhập lại" }
                        });
            }
            var cm = _teacherService.GetItemByID(userId);
            if (cm == null)
            {
                return new JsonResult(new Dictionary<string, object>()
                        {
                            {"Error", "Vui lòng đăng nhập lại" }
                        });
            }
            var center = _centerService.GetItemByCode(CenterCode);
            if (center == null || cm.Centers.Count(t => t.Code == CenterCode) == 0)
            {
                return new JsonResult(new Dictionary<string, object>()
                        {
                            {"Error", "Cơ sở không đúng" }
                        });
            }
            var tc_sj = new List<TeacherSubjectsViewModel>();

            if (string.IsNullOrEmpty(item.ID) || item.ID == "0")
            {
                item.ID = null;
                item.Created = DateTime.Now;


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
                item.Members = new List<ClassMemberEntity> { new ClassMemberEntity { TeacherID = userId, Type = ClassMemberType.OWNER, Name = cm.FullName } };
                item.TotalLessons = 0;
                item.TotalPractices = 0;
                item.TotalExams = 0;
                item.IsActive = true;
                item.StartDate = item.StartDate.ToUniversalTime();
                item.EndDate = item.EndDate.ToUniversalTime();
                item.Center = center.ID;

                if (fileUpload != null)
                {
                    var pathImage = _fileProcess.SaveMediaAsync(fileUpload, "", "CLASSIMG", center.Code).Result;
                    item.Image = pathImage;
                }

                _service.Save(item);

                //Create class subjects
                if (classSubjects != null && classSubjects.Count > 0)
                {
                    foreach (var csubject in classSubjects)
                    {
                        var teacher = _teacherService.GetItemByID(csubject.TeacherID);
                        var newMember = new ClassMemberEntity();
                        long lessoncount = 0;
                        long examcount = 0;
                        long practicecount = 0;
                        var nID = CreateNewClassSubject(csubject, item, out newMember, out lessoncount, out examcount, out practicecount);
                        if (!item.Skills.Contains(csubject.SkillID))
                            item.Skills.Add(csubject.SkillID);
                        if (!item.Subjects.Contains(csubject.SubjectID))
                            item.Subjects.Add(csubject.SubjectID);
                        if (!item.Members.Any(t => t.TeacherID == newMember.TeacherID && t.Type == ClassMemberType.TEACHER))
                            item.Members.Add(newMember);
                        item.TotalLessons += lessoncount;
                        item.TotalExams += examcount;
                        item.TotalPractices += practicecount;
                        var skill = _skillService.GetItemByID(csubject.SkillID);
                        if (skill == null) continue;
                        var course = _courseService.GetItemByID(csubject.CourseID);
                        var tc = tc_sj.SingleOrDefault(t => t.TeacherId == teacher.ID);
                        if (tc == null)
                            tc_sj.Add(new TeacherSubjectsViewModel
                            {
                                TeacherId = teacher.ID,
                                FullName = teacher.FullName,
                                Email = teacher.Email,
                                SubjectList = new List<SubjectModel> { new SubjectModel { SkillName = skill.Name, BookName = course != null ? course.Name : "" } }
                            });
                        else
                            tc.SubjectList.Add(new SubjectModel { SkillName = skill.Name, BookName = course != null ? course.Name : "" });
                    }

                    //Send email for each teacher
                    if (tc_sj.Count > 0)
                        foreach (var tc in tc_sj)
                            _ = _mailHelper.SendTeacherJoinClassNotify(tc, item, center.Name);

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
                    {"Error", "Không tìm thấy lớp" }
                });

                oldData.Updated = DateTime.Now;
                var mustUpdateName = false;
                if (oldData.Name != item.Name)
                {
                    oldData.Name = item.Name;
                    mustUpdateName = true;
                }
                oldData.Code = item.Code;
                oldData.StartDate = item.StartDate.ToUniversalTime();
                oldData.EndDate = item.EndDate.ToUniversalTime();
                oldData.Description = item.Description;
                //_service.Save(oldData);

                oldData.Skills = new List<string>();
                oldData.Subjects = new List<string>();
                var creator = _teacherService.GetItemByID(oldData.TeacherID);
                oldData.Members = new List<ClassMemberEntity> { };
                if (creator != null)
                    oldData.Members.Add(new ClassMemberEntity { TeacherID = creator.ID, Type = ClassMemberType.OWNER, Name = creator.FullName });
                oldData.TotalLessons = 0;
                oldData.TotalExams = 0;
                oldData.TotalPractices = 0;
                oldData.ClassMechanism = item.ClassMechanism;

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
                            long examcount = 0;
                            long practicecount = 0;

                            if (nSbj.CourseID != oSbj.CourseID)//SkillID ~ CourseID
                            {
                                nSbj.ID = CreateNewClassSubject(nSbj, oldData, out newMember, out lessoncount, out examcount, out practicecount);
                                if (string.IsNullOrEmpty(nSbj.ID))//Error
                                    continue;
                            }
                            else //Not change
                            {
                                //update period
                                oSbj.StartDate = item.StartDate.ToUniversalTime();
                                oSbj.EndDate = item.EndDate.ToUniversalTime();
                                oSbj.TypeClass = nSbj.TypeClass;
                                var teacher = _teacherService.GetItemByID(nSbj.TeacherID);
                                if (teacher == null) continue;

                                if (oSbj.TeacherID != nSbj.TeacherID) //change teacher
                                {
                                    oSbj.TeacherID = nSbj.TeacherID;
                                    var skill = _skillService.GetItemByID(oSbj.SkillID);
                                    if (skill == null) continue;
                                    var course = _courseService.GetItemByID(nSbj.CourseID);
                                    var tc = tc_sj.SingleOrDefault(t => t.TeacherId == teacher.ID);
                                    if (tc == null)
                                        tc_sj.Add(new TeacherSubjectsViewModel
                                        {
                                            TeacherId = teacher.ID,
                                            FullName = teacher.FullName,
                                            Email = teacher.Email,
                                            SubjectList = new List<SubjectModel> { new SubjectModel { SkillName = skill.Name, BookName = course != null ? course.Name : "" } }
                                        });
                                    else
                                        tc.SubjectList.Add(new SubjectModel { SkillName = skill.Name, BookName = course != null ? course.Name : "" });
                                }
                                //_ = _mailHelper.SendTeacherJoinClassNotify(teacher.FullName, teacher.Email, item.Name, skill.Name, item.StartDate, item.EndDate, center.Name);

                                _classSubjectService.Save(oSbj);
                                examcount = oSbj.TotalExams;
                                lessoncount = oSbj.TotalLessons;
                                practicecount = oSbj.TotalPractices;
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
                            //add counter

                            oldData.TotalLessons += lessoncount;
                            oldData.TotalExams += examcount;
                            oldData.TotalPractices += practicecount;
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
                        long examcount = 0;
                        long practicecount = 0;
                        var nID = CreateNewClassSubject(nSbj, oldData, out newMember, out lessoncount, out examcount, out practicecount);
                        if (string.IsNullOrEmpty(nSbj.ID))//Error
                            continue;

                        if (!oldData.Skills.Contains(nSbj.SkillID))
                            oldData.Skills.Add(nSbj.SkillID);
                        if (!oldData.Subjects.Contains(nSbj.SubjectID))
                            oldData.Subjects.Add(nSbj.SubjectID);
                        if (!oldData.Members.Any(t => t.TeacherID == newMember.TeacherID && t.Type == ClassMemberType.TEACHER))
                            oldData.Members.Add(newMember);
                        oldData.TotalLessons += lessoncount;
                        oldData.TotalExams += examcount;
                        oldData.TotalPractices += practicecount;
                    }
                }

                if (fileUpload != null)
                {
                    var pathImage = _fileProcess.SaveMediaAsync(fileUpload, "", "CLASSIMG", center.Code).Result;
                    oldData.Image = pathImage;
                }

                //update data
                _service.Save(oldData);

                if (tc_sj.Count > 0)
                    foreach (var tc in tc_sj)
                        _ = _mailHelper.SendTeacherJoinClassNotify(tc, item, center.Name);

                if (mustUpdateName)
                {
                    var change = _groupService.UpdateGroupDisplayName(oldData.ID, oldData.Name);
                }

                //refresh class total lesson => no need
                //_ = _classProgressService.RefreshTotalLessonForClass(oldData.ID);

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
        public JsonResult CloneClass(string ID, string CenterCode, string Name, bool updateCourse = false)
        {
            var oldData = _service.GetItemByID(ID);
            var center = _centerService.GetItemByCode(CenterCode);
            var userId = User.Claims.GetClaimByType("UserID").Value;
            var teacher = _teacherService.GetItemByID(userId);

            var newData = new MappingEntity<ClassEntity, ClassEntity>().Clone(oldData, new ClassEntity());
            newData.ID = null;
            newData.OriginID = oldData.ID;
            newData.Created = DateTime.Now;
            newData.Center = center.ID;
            if (!Name.Equals(oldData.Name))
                newData.Name = Name;
            newData.Skills = new List<string>();
            newData.Subjects = new List<string>();
            newData.TeacherID = teacher.ID;

            _service.CreateQuery().InsertOne(newData);

            var classSubjects = _classSubjectService.GetByClassID(oldData.ID);
            if (classSubjects != null && classSubjects.Count > 0)
            {
                foreach (var csubject in classSubjects)
                {
                    long lessoncount = 0;
                    long examcount = 0;
                    long practicecount = 0;

                    var ncbj = new ClassSubjectEntity();
                    var newMember = new ClassMemberEntity();
                    if (updateCourse)//Create new subject from course
                    {
                        ncbj.CourseID = csubject.CourseID;
                        ncbj.GradeID = csubject.GradeID;
                        ncbj.SubjectID = csubject.SubjectID;
                        ncbj.TeacherID = teacher.ID;
                        //Counter coud be changed in case base course change
                        var nID = CreateNewClassSubject(ncbj, newData, out newMember, out lessoncount, out examcount, out practicecount, false);
                    }
                    else //Clone subject from root class's subjects
                    {
                        ncbj = new MappingEntity<ClassSubjectEntity, ClassSubjectEntity>().Clone(csubject, ncbj);
                        ncbj.CourseID = csubject.CourseID;
                        ncbj.GradeID = csubject.GradeID;
                        ncbj.SubjectID = csubject.SubjectID;
                        ncbj.TeacherID = teacher.ID;
                        ncbj.ClassID = newData.ID;
                        lessoncount = csubject.TotalLessons;
                        examcount = csubject.TotalExams;
                        practicecount = csubject.TotalPractices;
                        _ = _classHelper.CloneClassSubject(ncbj, teacher.ID, csubject.ID);
                    }

                    if (!newData.Skills.Contains(csubject.SkillID))
                        newData.Skills.Add(csubject.SkillID);
                    if (!newData.Subjects.Contains(csubject.SubjectID))
                        newData.Subjects.Add(csubject.SubjectID);
                    if (!newData.Members.Any(t => t.TeacherID == newMember.TeacherID && t.Type == ClassMemberType.TEACHER))
                        newData.Members.Add(newMember);

                    newData.TotalLessons += lessoncount;
                    newData.TotalExams += examcount;
                    newData.TotalPractices += practicecount;
                }
                _service.Save(newData);
            }

            Dictionary<string, object> response = new Dictionary<string, object>()
                {
                    {"Data",newData },
                    {"Error",null },
                    {"Msg","Success" }
                };
            return new JsonResult(response);
        }

        private async Task RemoveClassSubject(ClassSubjectEntity cs)
        {
            ////remove old schedule
            var CsTask = _lessonScheduleService.RemoveClassSubject(cs.ID);
            //remove chapter
            var CtTask = _chapterService.RemoveClassSubjectChapter(cs.ID);
            //remove clone lesson
            var LsTask = _lessonHelper.RemoveClassSubjectLesson(cs.ID);
            //remove progress: learning history => class progress, chapter progress, lesson progress
            var LhTask = _progressHelper.RemoveClassSubjectHistory(cs.ID);
            //remove exam
            var ExTask = _examService.RemoveClassSubjectExam(cs.ID);
            //remove classSubject
            //await Task.WhenAll(CsTask, CtTask, LsTask, LhTask, ExTask, ExDetailTask);
            await _classSubjectService.RemoveAsync(cs.ID);
        }

        private string CreateNewClassSubject(ClassSubjectEntity nSbj, ClassEntity @class, out ClassMemberEntity member, out long lessoncount, out long examcount, out long practicecount, bool notify = true)
        {
            member = new ClassMemberEntity();
            lessoncount = 0;
            examcount = 0;
            practicecount = 0;
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
                examcount = course.TotalExams;
                practicecount = course.TotalPractices;

                var teacher = _teacherService.GetItemByID(nSbj.TeacherID);
                if (teacher == null || !teacher.IsActive || !teacher.Subjects.Contains(nSbj.SubjectID))
                {
                    throw new Exception("Teacher " + nSbj.TeacherID + " is not avaiable");
                }

                nSbj.CourseName = course.Name;
                nSbj.ClassID = @class.ID;
                nSbj.StartDate = @class.StartDate;
                nSbj.EndDate = @class.EndDate;
                nSbj.SkillID = course.SkillID;
                nSbj.Description = course.Description;
                nSbj.LearningOutcomes = course.LearningOutcomes;
                nSbj.TotalLessons = course.TotalLessons;
                nSbj.TotalPractices = course.TotalPractices;
                nSbj.TotalExams = course.TotalExams;

                var skill = _skillService.GetItemByID(nSbj.SkillID);

                var center = _centerService.GetItemByID(@class.Center);

                _classSubjectService.Save(nSbj);

                member = new ClassMemberEntity
                {
                    Name = teacher.FullName,
                    TeacherID = teacher.ID,
                    Type = ClassMemberType.TEACHER
                };
                //Clone Course
                _courseHelper.CloneForClassSubject(nSbj);

                if (notify)
                    _ = _mailHelper.SendTeacherJoinClassNotify(teacher.FullName, teacher.Email, @class.Name, skill?.Name, @class.StartDate, @class.EndDate, center.Name);
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
                    _ = _classSubjectService.RemoveManyClass(ids);
                    //remove Lesson, Part, Question, Answer
                    _ = _lessonHelper.RemoveManyClassLessons(ids);
                    //remove Schedule
                    _ = _lessonScheduleService.RemoveManyClass(ids);
                    //remove History
                    _ = _progressHelper.RemoveClassHistory(ids);
                    //remove Exam
                    _ = _examService.RemoveManyClassExam(ids);
                    //.Collection.DeleteMany(o => ids.Contains(o.ClassID));
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

        #region Add To My Course
        public async Task<JsonResult> AddToMyCourse(string CourseID, string CenterCode, string CourseName = "", string ClassID = null, ClassEntity item = null, Boolean isCreateNewClass = false)
        {
            var userId = User.Claims.GetClaimByType("UserID").Value;

            if (string.IsNullOrEmpty(userId))
            {
                return new JsonResult(new Dictionary<string, object>()
                        {
                            {"Error", "Vui lòng đăng nhập lại" }
                        });
            }
            var teacher = _teacherService.GetItemByID(userId);
            if (teacher == null)
            {
                return new JsonResult(new Dictionary<string, object>()
                        {
                            {"Error", "Vui lòng đăng nhập lại" }
                        });
            }
            var center = _centerService.GetItemByCode(CenterCode);
            if (center == null || teacher.Centers.Count(t => t.Code == CenterCode) == 0)
            {
                return new JsonResult(new Dictionary<string, object>()
                        {
                            {"Error", "Cơ sở không đúng" }
                        });
            }

            if (ClassID != null)
            {
                var Class = _service.GetItemByID(ClassID);
                if (Class == null)
                {
                    return new JsonResult(new Dictionary<string, object>()
                        {
                            {"Error", "Lớp không tồn tại" }
                        });
                }

                var Course = _courseService.GetItemByID(CourseID);//Bài giảng

                Class.Updated = DateTime.Now;
                var oldSubjects = _classSubjectService.GetByClassID(Class.ID);
                var classSubject = new ClassSubjectEntity();
                classSubject.CourseID = Course.ID;
                classSubject.CourseName = CourseName;
                classSubject.SkillID = Course.SkillID;
                classSubject.GradeID = Course.GradeID;
                classSubject.SubjectID = Course.SubjectID;
                classSubject.TeacherID = teacher.ID;
                classSubject.TypeClass = CLASS_TYPE.EXTEND;

                oldSubjects.Add(classSubject);

                Create(Class, center.Code, oldSubjects, null);
            }
            else
            {
                var Course = _courseService.GetItemByID(CourseID);//Bài giảng
                Course.OriginID = Course.ID;
                Course.Center = center.ID;
                Course.Created = DateTime.Now;
                Course.CreateUser = teacher.ID;
                Course.IsAdmin = true;
                Course.IsPublic = false;
                Course.IsActive = true;
                Course.Updated = DateTime.Now;
                Course.TeacherID = teacher.ID;
                Course.TotalPractices = 0;
                Course.TotalLessons = 0;
                Course.TotalExams = 0;
                Course.TargetCenters = new List<string>();
                Course.Name = CourseName == "" ? Course.Name : CourseName;

                Course.ID = null;

                var newID = await CopyCourse(_courseService.GetItemByID(CourseID), Course, userId);

                if (isCreateNewClass)
                {
                    var listclassSubject = new List<ClassSubjectEntity>();
                    var classSubject = new ClassSubjectEntity();
                    classSubject.CourseID = newID;
                    classSubject.SkillID = Course.SkillID;
                    classSubject.GradeID = Course.GradeID;
                    classSubject.SubjectID = Course.SubjectID;
                    classSubject.TeacherID = teacher.ID;
                    classSubject.TypeClass = CLASS_TYPE.EXTEND;

                    listclassSubject.Add(classSubject);

                    Create(item, center.Code, listclassSubject, null);
                }
            }
            return new JsonResult("Thêm thành công");
        }

        public async Task<string> CopyCourse(CourseEntity org_course, CourseEntity target_course, string _userCreate = "")
        {
            try
            {
                var new_course = _cloneCourseMapping.Clone(org_course, new CourseEntity());

                new_course.OriginID = org_course.ID;
                new_course.Name = target_course.Name;
                new_course.Code = target_course.Code;
                new_course.Description = target_course.Description;
                new_course.GradeID = target_course.GradeID;
                new_course.SubjectID = target_course.SubjectID;
                new_course.TeacherID = _userCreate;
                new_course.CreateUser = _userCreate;
                new_course.Center = target_course.Center ?? org_course.Center;
                new_course.SkillID = target_course.SkillID;
                new_course.Created = DateTime.Now;
                new_course.Updated = DateTime.Now;
                new_course.IsActive = true;
                new_course.IsUsed = false;
                new_course.IsPublic = false;
                _courseService.Collection.InsertOne(new_course);

                //var a = _courseChapterService.CreateQuery().Find(o => o.CourseID == org_course.ID).SortBy(o => o.ParentID).ThenBy(o => o.Order).ThenBy(o => o.ID).ToList();
                //foreach (var item in a)
                //{
                await CloneCourseChapter(new CourseChapterEntity
                {
                    OriginID = "0",
                    CourseID = new_course.ID,
                }, _userCreate, org_course.ID);
                //}

                return new_course.ID;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<CourseChapterEntity> CloneCourseChapter(CourseChapterEntity item, string _userCreate, string orgCourseID)
        {
            try
            {
                if (item.OriginID != "0")
                    _courseChapterService.Collection.InsertOne(item);
                else
                {
                    item.ID = "0";
                }

                var lessons = _courseLessonService.GetChapterLesson(orgCourseID, item.OriginID);

                if (lessons != null && lessons.Count() > 0)
                {
                    foreach (var o in lessons)
                    {
                        if (o.ChapterID == item.OriginID)
                        {
                            await _lessonHelper.CopyCourseLessonFromCourseLesson(o, new CourseLessonEntity
                            {
                                CourseID = item.CourseID,
                                ChapterID = item.ID,
                                CreateUser = _userCreate
                            });
                        }
                    }
                }

                var subChapters = _courseChapterService.GetSubChapters(orgCourseID, item.OriginID);
                foreach (var o in subChapters)
                {
                    var new_chapter = _cloneCourseChapterMapping.Clone(o, new CourseChapterEntity());
                    new_chapter.CourseID = item.CourseID;
                    new_chapter.ParentID = item.ID;
                    new_chapter.CreateUser = _userCreate;
                    new_chapter.Created = DateTime.Now;
                    new_chapter.OriginID = o.ID;
                    await CloneCourseChapter(new_chapter, _userCreate, orgCourseID);
                }
                return item;
            }
            catch (Exception ex)
            {
                return null;
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
            var results = _progressHelper.GetClassResults(ClassID).OrderByDescending(t => t.RankPoint).ToList();
            var avgpoint = 0.0;
            var studentresult = _classProgressService.GetStudentResult(ClassID, StudentID);

            if (studentresult != null)
            {
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
            var MyClass = _classService.GetClassByMechanism(CLASS_MECHANISM.PERSONAL, StudentID);
            if (student == null)
                return null;

            var data = new List<StudentSummaryViewModel>();
            if (student.JoinedClasses != null && student.JoinedClasses.Count > 0)
            {
                if (MyClass != null) { student.JoinedClasses.RemoveAt(student.JoinedClasses.IndexOf(MyClass.ID)); }

                foreach (var ClassID in student.JoinedClasses)
                {
                    var @class = _service.GetItemByID(ClassID);
                    if (@class == null) continue;
                    var total_students = _studentService.CountByClass(@class.ID);
                    var summary = new MappingEntity<ClassProgressEntity, StudentSummaryViewModel>()
                        .AutoOrtherType(_classProgressService.GetStudentResult(ClassID, StudentID) ?? new ClassProgressEntity
                        {
                            ClassID = ClassID,
                            StudentID = StudentID,
                        }, new StudentSummaryViewModel()
                        {
                            ClassName = @class.Name,
                            Rank = -1,
                            TotalStudents = (int)total_students,
                            TotalLessons = @class.TotalLessons,
                            TotalExams = @class.TotalExams,
                            TotalPractices = @class.TotalPractices
                        });

                    var results = _progressHelper.GetClassResults(ClassID).OrderByDescending(t => t.RankPoint).ToList();

                    summary.RankPoint = _progressHelper.CalculateRankPoint(summary.TotalPoint, summary.PracticePoint, summary.Completed);
                    summary.AvgPoint = @class.TotalExams > 0 ? summary.TotalPoint / @class.TotalExams : 0;

                    if (results != null && (results.FindIndex(t => t.StudentID == summary.StudentID) >= 0))
                        summary.Rank = results.FindIndex(t => t.RankPoint == summary.RankPoint) + 1;

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
                var summary = new MappingEntity<ClassSubjectProgressEntity, StudentSummaryViewModel>()
                    .AutoOrtherType(_classSubjectProgressService.GetItemByClassSubjectID(sbj.ID, StudentID) ?? new ClassSubjectProgressEntity
                    {
                        ClassID = @class.ID,
                        StudentID = StudentID,
                        ClassSubjectID = sbj.ID
                    }, new StudentSummaryViewModel()
                    {

                        CourseName = sbj.CourseName,
                        Rank = -1,
                        TotalStudents = (int)total_students,
                        TotalLessons = sbj.TotalLessons,
                        TotalExams = sbj.TotalExams,
                        TotalPractices = sbj.TotalPractices
                    });

                if (string.IsNullOrEmpty(summary.CourseName))
                {
                    var course = _courseService.GetItemByID(sbj.CourseID);
                    if (course == null)
                    {
                        summary.SkillName = _skillService.GetItemByID(sbj.SkillID).Name;
                    }
                    else
                    {
                        summary.CourseName = course.Name;
                    }
                }

                summary.AvgPoint = summary.TotalExams > 0 ? summary.TotalPoint / summary.TotalExams : 0;
                summary.RankPoint = _progressHelper.CalculateRankPoint(summary.TotalPoint, summary.PracticePoint, summary.Completed);

                var results = _progressHelper.GetClassSubjectResults(sbj.ID).OrderByDescending(t => t.RankPoint).ToList();
                if (results != null && (results.FindIndex(t => t.StudentID == StudentID) >= 0))
                    summary.Rank = results.FindIndex(t => t.RankPoint == summary.RankPoint) + 1;

                data.Add(summary);
            }
            return data;
        }

        #endregion

        #region Fix Data

        #endregion

        #region Edit
        public IActionResult Editor(string basis, string ID)
        {
            if (string.IsNullOrEmpty("ID"))
                return Redirect($"/{basis}{Url.Action("Index")}");

            var currentClassSbj = _classSubjectService.GetItemByID(ID);
            if (currentClassSbj == null)
                return Redirect($"/{basis}{Url.Action("Index")}");

            var currentClass = _classService.GetItemByID(currentClassSbj.ClassID);

            //var isUsed = isCourseUsed(data.ID);
            //Cap nhat IsUsed
            //if (data.IsUsed != isUsed)
            //{
            //    data.IsUsed = isUsed;
            //    _service.Save(data);
            //}

            ViewBag.ClassSbj = currentClassSbj;
            ViewBag.Class = currentClass;

            var UserID = User.Claims.GetClaimByType("UserID").Value;

            var chapters = _chapterService.GetByClassSubject(ID).ToList();

            ViewBag.Chapter = chapters;
            ViewBag.User = UserID;

            return View("Editor");
        }

        public async Task<JsonResult> CreateOrUpdateLesson(LessonEntity item)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var data = _lessonService.GetItemByID(item.ID);
                if (data == null)
                {
                    item.Created = DateTime.Now;
                    item.CreateUser = UserID;
                    item.IsAdmin = true;
                    item.IsActive = false;
                    item.IsParentCourse = item.ChapterID.Equals("0");
                    item.Updated = DateTime.Now;
                    item.Order = 0;

                    _lessonHelper.InitLesson(item);//insert + create schedule

                    ChangeLessonPosition(item, Int32.MaxValue);//move lesson to bottom of parent

                    //update total lesson to parent chapter
                    await _classHelper.IncreaseLessonCounter(item, 1, item.TemplateType == LESSON_TEMPLATE.EXAM ? 1 : 0, 0);
                }
                else
                {
                    item.Updated = DateTime.Now;
                    var newOrder = item.Order - 1;
                    item.Order = data.Order;
                    item.ClassID = data.ClassID;
                    item.ClassSubjectID = data.ClassSubjectID;

                    //update counter if type change
                    if (item.TemplateType != data.TemplateType)
                    {
                        var examInc = 0;
                        var pracInc = 0;
                        if (_lessonHelper.IsQuizLesson(item.ID)) pracInc = 1;
                        if (item.TemplateType == LESSON_TEMPLATE.LECTURE) // EXAM => LECTURE
                        {
                            examInc = -1;
                            item.IsPractice = pracInc == 1;
                        }
                        else
                        {
                            examInc = 1;
                            item.IsPractice = false;
                            pracInc = pracInc == 1 ? -1 : 0;
                        }
                        await _classHelper.IncreaseLessonCounter(item, 0, examInc, pracInc);
                        //if (!string.IsNullOrEmpty(item.ChapterID) && item.ChapterID != "0")
                        //    _ = _classHelper.IncreaseChapterCounter(item.ChapterID, 0, examInc, pracInc);
                        //else
                        //    _ = _classHelper.IncreaseClassSubjectCounter(item.ClassSubjectID, 0, examInc, pracInc);
                    }

                    _lessonService.CreateQuery().ReplaceOne(o => o.ID == item.ID, item);

                    if (item.Order != newOrder)//change Position
                    {
                        ChangeLessonPosition(item, newOrder);
                    }
                }

                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", item },
                    {"Error",null }
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", null },
                    {"Error",ex.Message }
                });
            }
        }

        [HttpPost]
        public async Task<JsonResult> CopyLesson(string ArrID, string Title, string ChapterID)
        {
            var orgLesson = _lessonService.GetItemByID(ArrID);
            var UserID = User.Claims.GetClaimByType("UserID").Value;

            var new_lesson = new LessonEntity
            {
                ChapterID = ChapterID,
                CreateUser = UserID,
                Title = string.IsNullOrEmpty(Title) ? (orgLesson.Title + (orgLesson.ChapterID == ChapterID ? " (copy)" : "")) : Title,
                Order = (int)_lessonService.CountChapterLesson(ChapterID) + 1
            };
            await _lessonHelper.CopyLessonFromLesson(orgLesson, new_lesson);
            await _classHelper.IncreaseLessonCounter(new_lesson, 1, new_lesson.TemplateType == LESSON_TEMPLATE.EXAM ? 1 : 0, new_lesson.IsPractice ? 1 : 0);

            return new JsonResult("OK");
        }

        [HttpPost]
        public async Task<JsonResult> RemoveLesson(DefaultModel model, string ID)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.ArrID))
                    ID = model.ArrID;
                var lesson = _lessonService.GetItemByID(ID);//TODO: check permission
                if (lesson != null)
                {
                    await _classHelper.IncreaseLessonCounter(lesson, -1, lesson.TemplateType == LESSON_TEMPLATE.EXAM ? -1 : 0, lesson.IsPractice ? -1 : 0);

                    //if (lesson.ChapterID == "0")
                    //    await _classHelper.IncreaseClassSubjectCounter(lesson.ClassSubjectID, -1, lesson.TemplateType == LESSON_TEMPLATE.EXAM ? -1 : 0, lesson.IsPractice ? -1 : 0);
                    //else
                    //    await _classHelper.IncreaseChapterCounter(lesson.ChapterID, -1, lesson.TemplateType == LESSON_TEMPLATE.EXAM ? -1 : 0, lesson.IsPractice ? -1 : 0);

                    ChangeLessonPosition(lesson, int.MaxValue);//chuyển lesson xuống cuối của đối tượng chứa
                    await _lessonHelper.RemoveSingleLesson(lesson.ID);

                    return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", ID },
                                { "Error", null }
                            });
                }
                else
                {
                    return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", null },
                                { "Error", "Item Not Found" }
                            });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", null },
                    {"Error", ex.Message}
                });
            }
        }

        private int ChangeLessonPosition(LessonEntity item, int pos)
        {
            var parts = _lessonService.GetChapterLesson(item.ClassSubjectID, item.ChapterID);
            var ids = parts.Select(o => o.ID).ToList();

            var oldPos = ids.IndexOf(item.ID);
            if (oldPos == pos && oldPos == item.Order)
            {
                return oldPos;
            }
            if (pos > parts.Count())
                pos = parts.Count() - 1;
            item.Order = pos;

            _lessonService.CreateQuery().ReplaceOne(o => o.ID == item.ID, item);
            int entry = -1;
            foreach (var part in parts)
            {
                if (part.ID == item.ID) continue;
                if (entry == pos - 1)
                    entry++;
                entry++;
                part.Order = entry;
                _lessonService.CreateQuery().ReplaceOne(o => o.ID == part.ID, part);
            }
            return pos;
        }

        [HttpPost]
        public async Task<JsonResult> JoinLesson(string ID, string JoinLesson)
        {
            try
            {
                var rootItem = _lessonService.GetItemByID(ID);
                var joinItem = _lessonService.GetItemByID(JoinLesson);
                if (rootItem == null || joinItem == null)
                {
                    return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", null },
                        { "Error", "Dữ liệu không đúng" }
                    });
                }
                var currentIndex = _cloneLessonPartService.CreateQuery().CountDocuments(o => o.ParentID == rootItem.ID);
                var joinParts = _cloneLessonPartService.CreateQuery().Find(o => o.ParentID == joinItem.ID).SortBy(o => o.Order).ToList();

                var hasPractice = false;
                if (joinParts != null && joinParts.Count > 0)
                {
                    foreach (var part in joinParts)
                    {
                        if (quizType.Contains(part.Type))
                            hasPractice = true;
                        part.ParentID = rootItem.ID;
                        part.Order = (int)currentIndex++;
                        _cloneLessonPartService.CreateQuery().ReplaceOne(o => o.ID == part.ID, part);
                    }
                }

                ChangeLessonPosition(joinItem, int.MaxValue);//chuyển lesson xuống cuối của đối tượng chứa

                //change counter
                if (rootItem.TemplateType == LESSON_TEMPLATE.LECTURE)
                    if (!rootItem.IsPractice && hasPractice)
                    {
                        rootItem.IsPractice = true;
                        _lessonService.Save(rootItem);
                        _ = _classHelper.IncreaseLessonCounter(rootItem, 0, 0, 1);

                        //if (rootItem.ChapterID == "0")
                        //    _classHelper.IncreaseClassSubjectCounter(rootItem.ClassSubjectID, 0, 0, 1);
                        //else
                        //    _classHelper.IncreaseChapterCounter(rootItem.ChapterID, 0, 0, 1);
                    }

                //decrease counter
                _ = _classHelper.IncreaseLessonCounter(joinItem, -1, joinItem.TemplateType == LESSON_TEMPLATE.EXAM ? -1 : 0, joinItem.IsPractice ? -1 : 0);

                //if (joinItem.ChapterID == "0")
                //    _classHelper.IncreaseClassSubjectCounter(joinItem.ClassSubjectID, -1, 0 - joinItem.TemplateType == LESSON_TEMPLATE.EXAM ? 1 : 0, 0 - (joinItem.IsPractice ? 1 : 0));
                //else
                //    _classHelper.IncreaseChapterCounter(joinItem.ChapterID, -1, 0 - joinItem.TemplateType == LESSON_TEMPLATE.EXAM ? 1 : 0, 0 - (joinItem.IsPractice ? 1 : 0));

                //remove all lesson content from db
                _ = _lessonHelper.RemoveSingleLesson(joinItem.ID);

                return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", joinItem },
                        { "Del", JoinLesson },
                        { "Error", null }
                    });
            }
            catch (Exception ex)
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", null },
                    { "Error", ex.Message }
                });
            }
        }

        public JsonResult CreateOrUpdateChapter(ChapterEntity item)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;

                if (item.ClassSubjectID == null || _classSubjectService.GetItemByID(item.ClassSubjectID) == null)
                {
                    return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", null },
                        {"Error", "Không tìm thấy học liệu" }
                    });
                }

                var data = _chapterService.GetItemByID(item.ID);
                if (data == null)
                {
                    item.Created = DateTime.Now;
                    item.IsAdmin = true;
                    item.IsActive = false;
                    item.Updated = DateTime.Now;
                    item.Order = int.MaxValue - 1;
                    _chapterService.Save(item);
                    ChangeChapterPosition(item, int.MaxValue);//move chapter to bottom of new parent chap
                }
                else
                {
                    item.Updated = DateTime.Now;
                    var newOrder = item.Order - 1;
                    var oldParent = data.ParentID;

                    data.Name = item.Name;
                    data.ParentID = item.ParentID;
                    data.Description = item.Description;

                    _chapterService.Save(data);
                    if (oldParent != item.ParentID)//Change Root chapter
                    {
                        if (item.TotalLessons > 0)
                        {
                            //decrease old parent counter
                            _ = _classHelper.IncreaseChapterCounter(oldParent, 0 - data.TotalLessons, 0 - data.TotalExams, 0 - data.TotalPractices);
                            _ = _classHelper.IncreaseChapterCounter(item.ParentID, data.TotalLessons, data.TotalExams, data.TotalPractices);
                        }
                        //move chapter to bottom of new parent chap
                        ChangeChapterPosition(data, int.MaxValue);
                    }
                    else
                        ChangeChapterPosition(data, newOrder);
                }

                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", item },
                    { "Error", null }
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", null },
                    {"Error",ex.Message }
                });
            }
        }

        private int ChangeChapterPosition(ChapterEntity item, int pos)
        {
            var parts = new List<ChapterEntity>();
            parts = _chapterService.CreateQuery().Find(o => o.ClassSubjectID == item.ClassSubjectID && o.ParentID == item.ParentID)
                .SortBy(o => o.Order).ThenBy(o => o.ID).ToList();

            var ids = parts.Select(o => o.ID).ToList();

            var oldPos = ids.IndexOf(item.ID);
            if (oldPos == pos && (item.Order == pos))
                return oldPos;

            if (pos > parts.Count())
                pos = parts.Count() - 1;
            item.Order = pos;

            _chapterService.CreateQuery().ReplaceOne(o => o.ID == item.ID, item);
            int entry = -1;
            foreach (var part in parts)
            {
                if (part.ID == item.ID) continue;
                if (entry == pos - 1)
                    entry++;
                entry++;
                part.Order = entry;
                _chapterService.CreateQuery().ReplaceOne(o => o.ID == part.ID, part);
            }
            return pos;
        }

        [HttpPost]
        public async Task<JsonResult> RemoveChapter(DefaultModel model)
        {
            try
            {
                var ID = model.ArrID;
                var chapter = _chapterService.GetItemByID(ID);
                if (chapter == null)
                {
                    return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", null },
                                {"Error", null }
                            });
                }
                //Decrease counter
                if (chapter.TotalLessons > 0)
                    if (chapter.ParentID == "0")
                        await _classHelper.IncreaseClassSubjectCounter(chapter.CourseID, 0 - chapter.TotalLessons, 0 - chapter.TotalExams, 0 - chapter.TotalPractices);
                    else
                        await _classHelper.IncreaseChapterCounter(chapter.ParentID, 0 - chapter.TotalLessons, 0 - chapter.TotalExams, 0 - chapter.TotalPractices);

                //Remove chapter
                await RemoveChapter(chapter);
                return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", ID },
                                {"Error", null }
                            });
            }
            catch (Exception ex)
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", null },
                    {"Error", ex.Message}
                });
            }
        }

        private async Task RemoveChapter(ChapterEntity chap)
        {
            //_lessonService.CreateQuery().DeleteMany(o => o.ChapterID == chap.ID);
            var lessons = _lessonService.GetChapterLesson(chap.ClassSubjectID, chap.ID);
            if (lessons != null && lessons.Count() > 0)
                foreach (var lesson in lessons)
                    _ = _lessonHelper.RemoveSingleLesson(lesson.ID);

            var subchapters = _chapterService.CreateQuery().Find(o => o.ParentID == chap.ID).ToList();
            if (subchapters != null && subchapters.Count > 0)
                foreach (var chapter in subchapters)
                    await RemoveChapter(chapter);
            //move chapter to bottom then delele
            ChangeChapterPosition(chap, int.MaxValue);
            await _chapterService.RemoveAsync(chap.ID);
        }

        [HttpPost]
        public async Task<JsonResult> CopyChapter(string ChapID)
        {
            var orgChapter = _chapterService.GetItemByID(ChapID);
            if (orgChapter == null)
            {
                return new JsonResult(new Dictionary<string, object>
                    {
                        {"Error", "Không tìm thấy thông tin" }
                    });
            }
            else
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var clone_chap = new MappingEntity<ChapterEntity, ChapterEntity>().Clone(orgChapter, new ChapterEntity());
                clone_chap.OriginID = orgChapter.ID;
                clone_chap.Order = (int)_chapterService.GetSubChapters(orgChapter.ClassSubjectID, orgChapter.ParentID).Count();
                clone_chap.CreateUser = UserID;
                clone_chap.Name += " (copy)";
                var chapter = await _classHelper.CloneChapter(clone_chap, orgChapter.CreateUser, orgChapter.ClassSubjectID);
                if (chapter.TotalLessons > 0)
                {
                    if (chapter.ParentID == "0")
                        await _classHelper.IncreaseClassSubjectCounter(chapter.ClassSubjectID, chapter.TotalLessons, chapter.TotalExams, chapter.TotalPractices);
                    else
                        await _classHelper.IncreaseChapterCounter(chapter.ParentID, chapter.TotalLessons, chapter.TotalExams, chapter.TotalPractices);
                }

                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", chapter },
                    { "Error", null }
                });
            }
        }

        [HttpPost]
        public async Task<JsonResult> JoinChapter(string ID, string JoinChapter, string newName, string CreateNewChapter = "off")
        {
            try
            {
                var _userCreate = User.Claims.GetClaimByType("UserID").Value;
                var rootChap = _chapterService.GetItemByID(ID);
                var joinChap = _chapterService.GetItemByID(JoinChapter);
                if (rootChap == null || joinChap == null)
                {
                    return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", null },
                        { "Error", "Dữ liệu không đúng" }
                    });
                }
                var currentChapIndex = (int)_chapterService.GetSubChapters(rootChap.CourseID, rootChap.ParentID).Count();
                var currentLessonIndex = (int)_lessonService.CountChapterLesson(rootChap.ID);

                var joinLessons = _lessonService.GetChapterLesson(joinChap.ClassSubjectID, joinChap.ID).OrderBy(o => o.Order);
                var joinSubChaps = _chapterService.GetSubChapters(joinChap.CourseID, joinChap.ID);

                if (CreateNewChapter.Equals("on"))
                {
                    if (newName != null || newName != "")
                        rootChap.Name = newName;
                    var chapMap = new MappingEntity<ChapterEntity, ChapterEntity>();
                    var clonechap = chapMap.Clone(rootChap, new ChapterEntity());
                    clonechap.Order = currentChapIndex;
                    var newChapter = await _classHelper.CloneChapter(clonechap, _userCreate, rootChap.ClassSubjectID); ;

                    var lessonMapping = new MappingEntity<CourseLessonEntity, CourseLessonEntity>();
                    //var new_lesson = new CourseLessonEntity();
                    if (joinLessons != null && joinLessons.Count() > 0)
                        foreach (var o in joinLessons)
                        {
                            await _lessonHelper.CopyLessonFromLesson(o, new LessonEntity
                            {
                                CreateUser = _userCreate,
                                ChapterID = newChapter.ID,
                                Order = currentLessonIndex++
                            });
                            //await CloneLesson(new_lesson, _userCreate);
                        }
                    if (joinSubChaps != null && joinSubChaps.Count() > 0)
                        foreach (var o in joinSubChaps)
                        {
                            var clone_chap = chapMap.Clone(o, new ChapterEntity());
                            clone_chap.OriginID = o.ID;
                            clone_chap.ParentID = newChapter.ID;
                            clone_chap.Created = DateTime.Now;
                            clone_chap.CreateUser = _userCreate;
                            clone_chap.Order = currentChapIndex++;
                            await _classHelper.CloneChapter(clone_chap, _userCreate, rootChap.CourseID);
                        }

                    //update new chapter counter
                    newChapter.TotalExams += joinChap.TotalExams;
                    newChapter.TotalLessons += joinChap.TotalLessons;
                    newChapter.TotalPractices += joinChap.TotalPractices;
                    _chapterService.Save(newChapter);

                    //add join chapter counter to parent holder
                    if (newChapter.ParentID == "0")
                        await _classHelper.IncreaseClassSubjectCounter(newChapter.ClassSubjectID, newChapter.TotalLessons, newChapter.TotalExams, newChapter.TotalPractices);
                    else
                        await _classHelper.IncreaseChapterCounter(newChapter.ParentID, newChapter.TotalLessons, newChapter.TotalExams, newChapter.TotalPractices);

                    return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", _chapterService.GetItemByID(newChapter.ID)},
                        { "Error", null }
                    });
                }
                else
                {
                    //Append joinChapter's lessons to bottom of rootchap's lessons list
                    if (joinLessons != null && joinLessons.Count() > 0)
                        foreach (var lesson in joinLessons.ToList())
                        {
                            lesson.ChapterID = rootChap.ID;
                            lesson.Order = (int)currentLessonIndex++;
                            _lessonService.Save(lesson);
                        }
                    //Append joinChapter's subchapter to bottom of rootchap's subchapter list
                    if (joinSubChaps != null && joinSubChaps.Count() > 0)
                        foreach (var subchap in joinSubChaps)
                        {
                            subchap.ParentID = rootChap.ParentID;
                            subchap.Order = (int)currentChapIndex++;
                            _chapterService.Save(subchap);
                        }

                    //add joinchap counter to root counter
                    rootChap.TotalExams += joinChap.TotalExams;
                    rootChap.TotalLessons += joinChap.TotalLessons;
                    rootChap.TotalPractices += joinChap.TotalPractices;
                    _chapterService.Save(rootChap);

                    //Move joinChapter to bottom of parent chapter list to correct order
                    ChangeChapterPosition(joinChap, int.MaxValue);
                    //Then remove join chapter
                    _chapterService.Remove(joinChap.ID);

                    return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", _chapterService.GetItemByID(rootChap.ID) },
                        { "Error", null }
                    });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", null },
                    { "Error", ex.Message }
                });
            }
        }


        #endregion

        public IActionResult CheckPoint(DefaultModel model, string basis)
        {
            if (!string.IsNullOrEmpty(model.ID))
            {
                ExamEntity data = _examService.GetItemByID(model.ID);
                if (data != null)
                {
                    var ExamTypes = quizType;

                    var lesson = _lessonService.GetItemByID(data.LessonID);

                    var listParts = _cloneLessonPartService.GetByLessonID(data.LessonID).Where(o => ExamTypes.Contains(o.Type));

                    var mapping = new MappingEntity<LessonEntity, StudentLessonViewModel>();
                    var mapPart = new MappingEntity<CloneLessonPartEntity, PartViewModel>();
                    var mapQuestion = new MappingEntity<CloneLessonPartQuestionEntity, QuestionViewModel>();
                    var mapExam = new MappingEntity<ExamEntity, ExamReviewViewModel>();

                    var examview = mapExam.AutoOrtherType(data, new ExamReviewViewModel()
                    {
                        Details = _examDetailService.Collection.Find(t => t.ExamID == data.ID).ToList()
                    });

                    var lessonview = mapping.AutoOrtherType(lesson, new StudentLessonViewModel()
                    {
                        Part = listParts.Select(o => mapPart.AutoOrtherType(o, new PartViewModel()
                        {
                            Questions = _cloneLessonPartQuestionService.CreateQuery().Find(x => x.ParentID == o.ID).ToList()
                                .Select(z => mapQuestion.AutoOrtherType(z, new QuestionViewModel()
                                {
                                    CloneAnswers = _cloneLessonPartAnswerService.CreateQuery().Find(x => x.ParentID == z.ID).ToList(),
                                    AnswerEssay = o.Type == "ESSAY" ? _examDetailService.CreateQuery().Find(e => e.QuestionID == z.ID && e.ExamID == data.ID)?.FirstOrDefault()?.AnswerValue : string.Empty,
                                    Medias = examview.Details.FirstOrDefault(e => e.QuestionID == z.ID)?.Medias,
                                    TypeAnswer = o.Type,
                                    RealAnswerEssay = o.Type == "ESSAY" ? examview.Details.FirstOrDefault(e => e.QuestionID == z.ID)?.RealAnswerValue : string.Empty,
                                    PointEssay = examview.Details.FirstOrDefault(e => e.QuestionID == z.ID)?.Point ?? 0,
                                    ExamDetailID = examview.Details.FirstOrDefault(e => e.QuestionID == z.ID)?.ID ?? "",
                                    MediasAnswer = examview.Details.FirstOrDefault(e => e.QuestionID == z.ID)?.MediasAnswers,
                                    MaxPoint = z.Point
                                }))?.ToList()
                        })).ToList()
                    });


                    ViewBag.Lesson = lessonview;
                    //ViewBag.Class = currentClass;
                    //ViewBag.Subject = currentCs;
                    //ViewBag.NextLesson = nextLesson;
                    //ViewBag.Chapter = chapter;
                    ViewBag.Type = lesson.TemplateType;
                    ViewBag.Exam = examview;
                }
            }
            ViewBag.Model = model;
            return View();
        }
    }
}
