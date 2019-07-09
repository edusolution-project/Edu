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
    public class ExampleController : StudentController
    {
        private readonly StudentService _studentService;
        private readonly ExamService _service;
        private readonly ClassService _classService;
        private readonly ExamDetailService _examDetailService;
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly LessonService _lessonService;
        private readonly CloneLessonPartService _cloneLessonPartService;
        private readonly CloneLessonPartAnswerService _cloneLessonPartAnswerService;
        private readonly CloneLessonPartQuestionService _cloneLessonPartQuestionService;
        public ExampleController(ExamService service,
            ExamDetailService examDetailService
        ,StudentService studentService
            , ClassService classService
            , LessonService lessonService
            , LessonScheduleService lessonScheduleService
            , CloneLessonPartService cloneLessonPartService
            , CloneLessonPartAnswerService cloneLessonPartAnswerService
            , CloneLessonPartQuestionService cloneLessonPartQuestionService
            )
        {
            _service = service;
            _classService = classService;
            _lessonService = lessonService;
            _lessonScheduleService = lessonScheduleService;
            _cloneLessonPartAnswerService = cloneLessonPartAnswerService;
            _cloneLessonPartService = cloneLessonPartService;
            _cloneLessonPartQuestionService = cloneLessonPartQuestionService;
            _examDetailService = examDetailService;
            _studentService = studentService;
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetList(DefaultModel model)
        {
            var filter = new List<FilterDefinition<ExamEntity>>();
            //var userId = User.Claims.GetClaimByType("UserID").Value;
            //if (string.IsNullOrEmpty(userId))
            //{
            //    return null;
            //}
            //else
            //{
            //    filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Students.Contains(userId)));
            //}
            //if (!string.IsNullOrEmpty(model.SearchText))
            //{
            //    filter.Add(Builders<ExamEntity>.Filter.Where(o => o.Name.ToLower().Contains(model.SearchText.ToLower())));
            //}
            if (model.StartDate > DateTime.MinValue)
            {
                filter.Add(Builders<ExamEntity>.Filter.Where(o => o.Created >= new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0)));
            }
            if (model.EndDate > DateTime.MinValue)
            {
                filter.Add(Builders<ExamEntity>.Filter.Where(o => o.Updated <= new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59)));
            }
            var data = filter.Count > 0 ? _service.Collection.Find(Builders<ExamEntity>.Filter.And(filter)) : _service.GetAll();
            model.TotalRecord = data.Count();
            var DataResponse = data == null || data.Count() <= 0 || data.Count() < model.PageSize
                ? data.ToList()
                : data.Skip((model.PageIndex - 1) * model.PageSize).Limit(model.PageSize).ToList();
            var mapping = new MappingEntity<ExamEntity,ExamViewModel>(); 
            var std = DataResponse.Select(o => mapping.AutoOrtherType(o,new ExamViewModel()
            {
                LessonScheduleName = _lessonService.GetItemByID(_lessonScheduleService.GetItemByID(o.LessonScheduleID).LessonID).Title,
                StudentName = _studentService.GetItemByID(o.StudentID).FullName
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
        public JsonResult GetDetails(string examID)
        {
            try
            {
                var userId = User.Claims.GetClaimByType("UserID").Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return null;
                }
                var filter = Builders<ExamDetailEntity>.Filter.Where(o => o.ExamID == examID);
                var data = _examDetailService.Collection.Find(filter);
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
