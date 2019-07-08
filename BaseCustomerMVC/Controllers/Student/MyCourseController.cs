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
    public class MyCourseController : StudentController
    {
        private readonly ClassService _service;
        private readonly CourseService _courseService;
        private readonly TeacherService _teacherService;
        private readonly SubjectService _subjectService;
        private readonly GradeService _gradeService;
        private readonly LessonService _lessonService;
        private readonly LessonPartService _lessonPartService;
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly CloneLessonPartService _cloneLessonPartService;
        private readonly CloneLessonPartAnswerService _cloneLessonPartAnswerService;
        private readonly CloneLessonPartQuestionService _cloneLessonPartQuestionService;
        private readonly Core_v2.Globals.MappingEntity<LessonEntity, LessonScheduleViewModel> _mapping;
        private readonly Core_v2.Globals.MappingEntity<ClassEntity, MyClassViewModel> _mappingList;
        public MyCourseController(ClassService service
            , CourseService courseService
            , TeacherService teacherService
            , SubjectService subjectService
            , GradeService gradeService
            , LessonService lessonService
            , LessonScheduleService lessonScheduleService
            , CloneLessonPartService cloneLessonPartService
            , CloneLessonPartAnswerService cloneLessonPartAnswerService
            , CloneLessonPartQuestionService cloneLessonPartQuestionService
            , LessonPartService lessonPartService
            )
        {
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
            _mapping = new Core_v2.Globals.MappingEntity<LessonEntity, LessonScheduleViewModel>();
            _mappingList = new Core_v2.Globals.MappingEntity<ClassEntity, MyClassViewModel>();
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetList(DefaultModel model, string TeacherID)
        {
            var filter = new List<FilterDefinition<ClassEntity>>();
            //var userId = User.Claims.GetClaimByType("UserID").Value;
            //if (string.IsNullOrEmpty(userId))
            //{
            //    return null;
            //}
            //else
            //{
            //    filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Students.Contains(userId)));
            //}
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

            var std = DataResponse.Select(o => _mappingList.AutoOrtherType(o,new MyClassViewModel() {
                CourseName = _courseService.GetItemByID(o.CourseID) == null ? "" : _courseService.GetItemByID(o.CourseID).Name,
                StudentNumber = o.Students.Count,
                SubjectName = _subjectService.GetItemByID(o.SubjectID) == null ? "" : _subjectService.GetItemByID(o.SubjectID).Name,
                GradeName = _gradeService.GetItemByID(o.GradeID) == null ? "" : _gradeService.GetItemByID(o.GradeID).Name,
                TeacherName = _teacherService.GetItemByID(o.TeacherID) == null ? "" : _teacherService.GetItemByID(o.TeacherID).FullName
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
        public JsonResult GetDetails(string CourseID,string ClassID)
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
                                //IsActive = _lessonScheduleService.GetItemByID(ClassID).IsActive,
                                //StartDate = _lessonScheduleService.GetItemByID(ClassID).StartDate,
                                //EndDate = _lessonScheduleService.GetItemByID(ClassID).EndDate,
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

        [System.Obsolete]
        [HttpPost]
        public JsonResult GetLesson(string LessonID,string ClassID)
        {
            //var userId = User.Claims.GetClaimByType("UserID").Value;
            //if (string.IsNullOrEmpty(userId))
            //{
            //    return null;
            //}
            var lesson = _lessonService.GetItemByID(LessonID);
            if (lesson == null) return null;
            var listPart = _cloneLessonPartService.CreateQuery().Find(o => o.ParentID == lesson.ID);
           // var listPartOriginal = _lessonPartService.CreateQuery().Find(o => o.ParentID == lesson.ID);
            if (listPart == null) return null;

            MappingEntity<LessonEntity, LessonViewModel> mapping = new MappingEntity<LessonEntity, LessonViewModel>();
            MappingEntity<CloneLessonPartEntity, PartViewModel> mapPart = new MappingEntity<CloneLessonPartEntity, PartViewModel>();
            MappingEntity<CloneLessonPartQuestionEntity, QuestionViewModel> mapQuestion = new MappingEntity<CloneLessonPartQuestionEntity, QuestionViewModel>();
            var dataResponse = mapping.AutoOrtherType(lesson, new LessonViewModel()
            {
                Parts = listPart.ToList().Select(o => mapPart.AutoOrtherType(o, new PartViewModel()
                {
                    Questions = _cloneLessonPartQuestionService.CreateQuery().Find(x => x.ParentID == o.ID)?.ToList()
                        .Select(z => mapQuestion.AutoOrtherType(z, new QuestionViewModel()
                        {
                            CloneAnswers = _cloneLessonPartAnswerService.CreateQuery().Find(x => x.ParentID == z.ID).ToList()
                        }))?.ToList()
                })).ToList()
            });

            var respone = new Dictionary<string, object>
            {
                { "Data", dataResponse }
            };
            return new JsonResult(respone);

        }

        public IActionResult Index(DefaultModel model)
        {
            ViewBag.Model = model;
            return View();
        }
        public IActionResult Calendar(DefaultModel model, string id,string ClassID)
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
    }
}
