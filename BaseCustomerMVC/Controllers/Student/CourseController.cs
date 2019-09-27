using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Core_v2.Globals;
using Core_v2.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseCustomerMVC.Controllers.Student
{
    public class CourseController : StudentController
    {
        private readonly ClassService _service;
        private readonly CourseService _courseService;
        private readonly TeacherService _teacherService;
        private readonly SubjectService _subjectService;
        private readonly GradeService _gradeService;
        private readonly LessonService _lessonService;
        private readonly LessonPartService _lessonPartService;
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly LessonPartQuestionService _lessonPartQuestionService;
        private readonly LessonPartAnswerService _lessonPartAnswerService;

        private readonly CloneLessonPartService _cloneLessonPartService;
        private readonly CloneLessonPartAnswerService _cloneLessonPartAnswerService;
        private readonly CloneLessonPartQuestionService _cloneLessonPartQuestionService;

        private readonly ClassProgressService _progressService;
        private readonly ExamService _examService;
        private readonly ExamDetailService _examDetailService;

        private readonly StudentService _studentService;
        private readonly ChapterService _chapterService;
        private readonly LearningHistoryService _learningHistoryService;

        private readonly MappingEntity<LessonEntity, LessonScheduleViewModel> _mapping;
        private readonly MappingEntity<ClassEntity, StudentClassViewModel> _mappingList;
        private readonly MappingEntity<StudentEntity, ClassMemberViewModel> _studentMapping;
        private readonly MappingEntity<ClassEntity, ClassActiveViewModel> _activeMapping;

        private readonly MappingEntity<LessonPartEntity, CloneLessonPartEntity> _lessonPartMapping;
        private readonly MappingEntity<LessonPartQuestionEntity, CloneLessonPartQuestionEntity> _lessonPartQuestionMapping;
        private readonly MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity> _lessonPartAnswerMapping;


        public CourseController(ClassService service
            , CourseService courseService
            , TeacherService teacherService
            , SubjectService subjectService
            , GradeService gradeService
            , LessonService lessonService
            , ChapterService chapterService
            , LessonPartQuestionService lessonPartQuestionService
            , LessonPartAnswerService lessonPartAnswerService
            , LessonScheduleService lessonScheduleService
            , CloneLessonPartService cloneLessonPartService
            , CloneLessonPartAnswerService cloneLessonPartAnswerService
            , CloneLessonPartQuestionService cloneLessonPartQuestionService
            , LessonPartService lessonPartService
            , ClassProgressService progressService
            , ExamService examService
            , ExamDetailService examDetailService
            , StudentService studentService
            , LearningHistoryService learningHistoryService
            )
        {
            _learningHistoryService = learningHistoryService;
            _chapterService = chapterService;
            _studentService = studentService;
            _examService = examService;
            _examDetailService = examDetailService;
            _service = service;
            _courseService = courseService;
            _teacherService = teacherService;
            _subjectService = subjectService;
            _gradeService = gradeService;
            _lessonService = lessonService;
            _lessonScheduleService = lessonScheduleService;
            _cloneLessonPartAnswerService = cloneLessonPartAnswerService;
            _cloneLessonPartService = cloneLessonPartService;
            _cloneLessonPartQuestionService = cloneLessonPartQuestionService;
            _lessonPartService = lessonPartService;
            _progressService = progressService;
            _mapping = new MappingEntity<LessonEntity, LessonScheduleViewModel>();
            _mappingList = new MappingEntity<ClassEntity, StudentClassViewModel>();
            _lessonPartQuestionService = lessonPartQuestionService;
            _lessonPartAnswerService = lessonPartAnswerService;
            _studentMapping = new MappingEntity<StudentEntity, ClassMemberViewModel>();
            _activeMapping = new MappingEntity<ClassEntity, ClassActiveViewModel>();

            _lessonPartMapping = new MappingEntity<LessonPartEntity, CloneLessonPartEntity>();
            _lessonPartQuestionMapping = new MappingEntity<LessonPartQuestionEntity, CloneLessonPartQuestionEntity>();
            _lessonPartAnswerMapping = new MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity>();
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetList(DefaultModel model, string TeacherID)
        {
            var filter = new List<FilterDefinition<ClassEntity>>();
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            else
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Students.Contains(userId)));
            }
            if (!string.IsNullOrEmpty(model.SearchText))
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Name.ToLower().Contains(model.SearchText.ToLower())));
            }
            if (model.StartDate > DateTime.MinValue)
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.StartDate >= new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0)));
            }
            if (model.EndDate > DateTime.MinValue)
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.EndDate <= new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59)));
            }
            var data = filter.Count > 0 ? _service.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _service.GetAll();
            model.TotalRecord = data.Count();
            var DataResponse = data == null || data.Count() <= 0 || data.Count() < model.PageSize
                ? data.ToList()
                : data.Skip((model.PageIndex - 1) * model.PageSize).Limit(model.PageSize).ToList();


            var std = DataResponse.Select(o => _mappingList.AutoOrtherType(o, new StudentClassViewModel()
            {
                CourseName = _courseService.GetItemByID(o.CourseID) == null ? "" : _courseService.GetItemByID(o.CourseID).Name,
                StudentNumber = o.Students.Count,
                SubjectName = _subjectService.GetItemByID(o.SubjectID) == null ? "" : _subjectService.GetItemByID(o.SubjectID).Name,
                GradeName = _gradeService.GetItemByID(o.GradeID) == null ? "" : _gradeService.GetItemByID(o.GradeID).Name,
                TeacherName = _teacherService.GetItemByID(o.TeacherID) == null ? "" : _teacherService.GetItemByID(o.TeacherID).FullName,
                Progress = _progressService.GetItemByClassID(o.ID)
            })).ToList();

            var respone = new Dictionary<string, object>
            {
                { "Data", std},
                { "Model", model }
            };
            return new JsonResult(respone);
        }


        [System.Obsolete]
        [HttpPost]
        public JsonResult GetDetails(string CourseID, string ClassID)
        {
            try
            {
                var userId = User.Claims.GetClaimByType("UserID").Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return null;
                }
                var filterSchedule = Builders<LessonScheduleEntity>.Filter.Where(o => o.ClassID == ClassID);
                var dataSchedule = _lessonScheduleService.Collection.Find(filterSchedule);
                if (dataSchedule == null || dataSchedule.Count() <= 0) return null;
                var schedules = dataSchedule.ToEnumerable();
                var filter = new List<FilterDefinition<LessonEntity>>();
                filter.Add(Builders<LessonEntity>.Filter.Where(o => o.CourseID == CourseID));
                //filter.Add(Builders<LessonEntity>.Filter.Where(o=> schedules.Select(x => x.LessonID).Contains(o.ID)));
                var data = _lessonService.Collection.Find(Builders<LessonEntity>.Filter.And(filter));
                var DataResponse = data == null || data.Count() <= 0 ? null : data.ToList();

                var respone = new Dictionary<string, object>
                {
                    { "Data", DataResponse.Select(
                        o=> _mapping.AutoOrtherType(o,new LessonScheduleViewModel(){
                                IsActive = _lessonScheduleService.GetItemByID(ClassID) == null ? false: _lessonScheduleService.GetItemByID(ClassID).IsActive,
                                StartDate = _lessonScheduleService.GetItemByID(ClassID) == null ?DateTime.MinValue :  _lessonScheduleService.GetItemByID(ClassID).StartDate,
                                EndDate = _lessonScheduleService.GetItemByID(ClassID) == null ? DateTime.MinValue : _lessonScheduleService.GetItemByID(ClassID).EndDate,
                                IsView = _learningHistoryService.CreateQuery().Count(x=>x.StudentID == userId && x.LessonID == o.ID && x.ClassID == ClassID) > 0
                            })
                        )
                    }
                };
                return new JsonResult(respone);
            }
            catch (Exception ex)
            {
                var respone = new Dictionary<string, object>
                {
                    { "Data", null },
                    {"Error",ex }
                };
                return new JsonResult(respone);
            }

        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetMembers(DefaultModel model)
        {
            var UserID = User.Claims.GetClaimByType("UserID").Value;

            if (string.IsNullOrEmpty(model.ID))
            {
                return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Không có thông tin lớp học" }
                    });
            }

            var currentClass = _service.GetItemByID(model.ID);
            if (currentClass == null || currentClass.Students.IndexOf(UserID) < 0)
            {
                return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Không có thông tin lớp học" }
                    });
            }

            var teacher = _teacherService.GetItemByID(currentClass.TeacherID);

            var filter = new List<FilterDefinition<StudentEntity>>();
            filter.Add(Builders<StudentEntity>.Filter.Where(o => currentClass.Students.Contains(o.ID)));
            var students = filter.Count > 0 ? _studentService.Collection.Find(Builders<StudentEntity>.Filter.And(filter)) : _studentService.GetAll();
            var totalLessons = _lessonScheduleService.CreateQuery().CountDocuments(o => o.ClassID == currentClass.ID);
            var studentsView = (from r in students.ToList()
                                let learned = _learningHistoryService.CreateQuery().AsQueryable()
                                .Where(t => t.StudentID == r.ID && t.ClassID == currentClass.ID)
                                .GroupBy(t => new { t.StudentID, t.ClassID, t.LessonID }).Count()
                                select _studentMapping.AutoOrtherType(r, new ClassMemberViewModel()
                                {
                                    ClassName = currentClass.Name,
                                    ClassStatus = "Đang học",
                                    Progress = (int)(totalLessons != 0 ? learned * 100 / totalLessons : 0),
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
            var UserID = User.Claims.GetClaimByType("UserID").Value;

            filter.Add(Builders<ClassEntity>.Filter.Where(o => o.IsActive && o.Students.Contains(UserID) && o.EndDate >= DateTime.Now.ToLocalTime().Date));

            var activeClasses = _service.Collection.Find(Builders<ClassEntity>.Filter.And(filter)).ToList();

            var responseData = new List<ClassActiveViewModel>();

            var subjects = _subjectService.GetAll().ToList();

            var _activeMapping = new MappingEntity<ClassEntity, ClassActiveViewModel>();

            foreach (var _class in activeClasses)
            {
                var totalLessons = _lessonScheduleService.CreateQuery().CountDocuments(o => o.ClassID == _class.ID);
                var learned = _learningHistoryService.CreateQuery().Aggregate().Match(o => o.ClassID == _class.ID).Group(o => o.LessonID,
                    o => new { x = o.Select(x => 1).First() }).ToList().Count();
                responseData.Add(_activeMapping.AutoOrtherType(_class, new ClassActiveViewModel()
                {
                    Progress = (int)(totalLessons != 0 ? learned * 100 / totalLessons : 0),
                    SubjectName = subjects.SingleOrDefault(s => s.ID == _class.SubjectID).Name
                }));
            }

            var response = new Dictionary<string, object>
            {
                { "Data", responseData }
            };
            return new JsonResult(response);
        }

        public IActionResult Index(DefaultModel model)
        {
            ViewBag.Model = model;
            return View();
        }

        public IActionResult Calendar(DefaultModel model, string id, string ClassID)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Bạn chưa chọn khóa học";
                return RedirectToAction("Index");
            }
            ViewBag.CourseID = id;
            ViewBag.ClassID = ClassID;
            ViewBag.Model = model;
            return View();
        }

        public IActionResult Detail(DefaultModel model, string id)
        {
            if (model == null) return null;
            var currentClass = _service.GetItemByID(id);
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (currentClass == null)
                return RedirectToAction("Index");
            if (currentClass.Students.IndexOf(userId) < 0)
                return RedirectToAction("Index");
            ViewBag.Class = currentClass;
            return View();
        }

        public IActionResult Syllabus(DefaultModel model, string id)
        {
            if (model == null) return null;
            var currentClass = _service.GetItemByID(id);
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (currentClass == null)
                return RedirectToAction("Index");
            if (currentClass.Students.IndexOf(userId) < 0)
                return RedirectToAction("Index");
            ViewBag.Class = currentClass;
            return View();
        }

        public IActionResult Modules(DefaultModel model, string id)
        {
            if (model == null) return null;
            var currentClass = _service.GetItemByID(id);
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (currentClass == null)
                return RedirectToAction("Index");
            if (currentClass.Students.IndexOf(userId) < 0)
                return RedirectToAction("Index");
            ViewBag.Class = currentClass;
            return View();
        }

        public IActionResult Assignment(DefaultModel model, string id)
        {
            if (model == null) return null;
            var currentClass = _service.GetItemByID(id);
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (currentClass == null)
                return RedirectToAction("Index");
            if (currentClass.Students.IndexOf(userId) < 0)
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



    }
}
