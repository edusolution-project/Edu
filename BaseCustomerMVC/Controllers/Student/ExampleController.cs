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
using System.Threading.Tasks;

namespace BaseCustomerMVC.Controllers.Student
{
    public class ExampleController : StudentController
    {

        private readonly ExamService _service;
        private readonly ExamDetailService _examDetailService;
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly LessonService _lessonService;
        private readonly CloneLessonPartService _cloneLessonPartService;
        private readonly CloneLessonPartAnswerService _cloneLessonPartAnswerService;
        private readonly CloneLessonPartQuestionService _cloneLessonPartQuestionService;
        private readonly TeacherService _teacherService;
        private readonly StudentService _studentService;
        private readonly ClassService _classService;
        private readonly LearningHistoryService _learningHistoryService;

        public ExampleController(ExamService service,
            ExamDetailService examDetailService
        , StudentService studentService
            , ClassService classService
            , LessonService lessonService
            , LessonScheduleService lessonScheduleService
            , CloneLessonPartService cloneLessonPartService
            , CloneLessonPartAnswerService cloneLessonPartAnswerService
            , CloneLessonPartQuestionService cloneLessonPartQuestionService
            , TeacherService teacherService
            , LearningHistoryService learningHistoryService
            )
        {
            _learningHistoryService = learningHistoryService;
            _service = service;
            _classService = classService;
            _lessonService = lessonService;
            _lessonScheduleService = lessonScheduleService;
            _cloneLessonPartAnswerService = cloneLessonPartAnswerService;
            _cloneLessonPartService = cloneLessonPartService;
            _cloneLessonPartQuestionService = cloneLessonPartQuestionService;
            _examDetailService = examDetailService;
            _studentService = studentService;
            _teacherService = teacherService;
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
            var mapping = new MappingEntity<ExamEntity, ExamViewModel>();
            var std = DataResponse.Select(o => mapping.AutoOrtherType(o, new ExamViewModel()
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
                var mapping = new MappingEntity<ExamDetailEntity, ExamDetailViewModel>();

                var respone = new Dictionary<string, object>
                {
                    { "Data", DataResponse.Select(
                        o=> mapping.AutoOrtherType(o,new ExamDetailViewModel(){
                                Answer = _cloneLessonPartAnswerService.GetItemByID(o.AnswerID).Content,
                                Question = _cloneLessonPartQuestionService.GetItemByID(o.QuestionID).Content,
                                RealAnswer = string.IsNullOrEmpty(o.RealAnswerID) || o.RealAnswerID == "0" ? "" : _cloneLessonPartQuestionService.GetItemByID(o.RealAnswerID).Content,
                                StudentName = _studentService.GetItemByID(_service.GetItemByID(o.ExamID).StudentID).FullName
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
        [HttpPost]
        [Obsolete]
        public JsonResult Create(ExamEntity item)
        {
            var userid = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(item.ID) || item.ID == "0")
            {
                var _lesson = _lessonService.GetItemByID(item.LessonID);
                var _class = _classService.GetItemByID(item.ClassID);
                if (_class == null)
                {
                    return new JsonResult("No Class for Student");
                }
                var _currentSchedule = _lessonScheduleService.CreateQuery().Find(o => o.LessonID == _lesson.ID && o.ClassID == _class.ID);
                var _schedule = _currentSchedule != null && _currentSchedule.Count() > 0 ? _currentSchedule.FirstOrDefault() : null;
                if (_schedule != null)
                {
                    item.LessonScheduleID = _schedule.ID;
                    item.Number = (int)_service.CreateQuery().Find(o => o.Timer == _lesson.Timer
                    && o.StudentID == userid
                    && o.Status == true
                    && o.TeacherID == _class.TeacherID
                    && o.LessonScheduleID == _schedule.ID).Count();
                }
                else
                {
                    item.Number = (int)_service.CreateQuery().Find(o => o.Timer == _lesson.Timer
                    && o.StudentID == userid
                    && o.Status == true
                    && o.TeacherID == _class.TeacherID).Count();
                }
                item.Timer = _lesson.Timer;
                item.Point = 0;

                item.StudentID = userid;
                item.TeacherID = _class.TeacherID;
                item.ID = null;
                item.Created = DateTime.Now;
                item.CurrentDoTime = DateTime.Now;
                item.Status = false;
                item.Number = (int)_service.CreateQuery().Find(o => o.LessonScheduleID == item.LessonScheduleID && o.StudentID == item.StudentID).Count() + 1;
            }
            else
            {
                item.Updated = DateTime.Now;
            }
            _service.CreateOrUpdate(item);
            return new JsonResult(item);
        }
        [HttpPost]
        public JsonResult GetCurrentExam(string ClassID, string LessonID)
        {
            var userID = User.Claims.GetClaimByType("UserID").Value;
            var x = _service.CreateQuery().Find(o => o.ClassID == ClassID && o.LessonID == LessonID && o.Status == false && o.StudentID == userID).FirstOrDefault();
            return new JsonResult(x);
        }
        [HttpPost]
        public JsonResult CreateDetails(ExamDetailEntity item)
        {
            if (!_service.IsOverTime(item.ExamID))
            {
                var exam = _service.GetItemByID(item.ExamID);
                _learningHistoryService.CreateHist(new LearningHistoryEntity()
                {
                    ClassID = exam.ClassID,
                    LessonID = exam.LessonID,
                    LessonPartID = item.LessonPartID,
                    QuestionID = item.QuestionID,
                    Time = DateTime.Now,
                    StudentID = User.Claims.GetClaimByType("UserID").Value
                });

                if (string.IsNullOrEmpty(item.ID) || item.ID == "0" || item.ID == "null")
                {
                    var map = new MappingEntity<ExamDetailEntity, ExamDetailEntity>();
                    var oldItem = _examDetailService.CreateQuery().Find(o => o.ExamID == item.ExamID && o.QuestionID == item.QuestionID).FirstOrDefault();
                    if (oldItem == null)
                    {
                        item.Created = DateTime.Now;
                        var question = _cloneLessonPartQuestionService.GetItemByID(item.QuestionID);
                        if (question == null)
                            return new JsonResult("Data Error");
                        item.LessonPartID = question.ParentID;
                        item.QuestionValue = question.Content;
                        item.MaxPoint = question.Point;
                        var xitem = map.AutoWithoutID(item, new ExamDetailEntity() { });
                        _examDetailService.CreateOrUpdate(xitem);
                        return new JsonResult(xitem);
                    }
                    else
                    {
                        var xitem = map.Auto(oldItem, item);
                        _examDetailService.CreateOrUpdate(xitem);
                        return new JsonResult(xitem);
                    }
                }
                else
                {
                    item.Updated = DateTime.Now;
                }
                _examDetailService.CreateOrUpdate(item);
                return new JsonResult(item);
            }
            else
            {
                return new JsonResult("Access Deny");
            }
        }

        //submit form
        [HttpPost]
        public JsonResult CompleteExam(string ExamID, string StudentID)
        {
            var example = _service.GetItemByID(ExamID);
            if (example == null)
            {
                return new JsonResult("Data not found");
            }
            if (example.Status)
            {
                return new JsonResult("Accept deny");
            }
            example.Status = true;
            double point = 0;
            var userID = User.Claims.GetClaimByType("UserID").Value;
            //phair kiem tra lai
            //var listDetails = _examDetailService.Collection.Find(o => o.ExamID == example.ID).ToList();
            //for(int i = 0; listDetails != null&& i < listDetails.Count; i++)
            //{
            //    var examDetail = listDetails[i];
            //    if (string.IsNullOrEmpty(examDetail.QuestionID) || examDetail.QuestionID == "0") continue;
            //    var answer = _cloneLessonPartAnswerService.CreateQuery().Find(o => o.IsCorrect && o.ParentID == examDetail.QuestionID && o.TeacherID == example.TeacherID)?.First();
            //    examDetail.RealAnswerID = answer == null ? "0" : answer.ID;
            //    if (answer != null)
            //    {
            //        if (examDetail.AnswerID == examDetail.RealAnswerID)
            //        {
            //            point += _cloneLessonPartQuestionService.GetItemByID(examDetail.QuestionID).Point;
            //            examDetail.Point = _cloneLessonPartQuestionService.GetItemByID(examDetail.QuestionID).Point;
            //        }
            //    }
            //    _examDetailService.CreateOrUpdate(examDetail);
            //}
            example.Point = point;
            _service.CreateOrUpdate(example);
            return new JsonResult(example);
        }


        public IActionResult Index(DefaultModel model)
        {
            ViewBag.Model = model;
            return View();
        }

        public IActionResult Details(DefaultModel model, string id, string ClassID)
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
