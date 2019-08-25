﻿using BaseCustomerEntity.Database;
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
    public class ExamController : StudentController
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
        private readonly LessonPartAnswerService _lessonPartAnswerService;
        private readonly LessonPartQuestionService _lessonPartQuestionService;

        public ExamController(ExamService service,
            LessonPartAnswerService lessonPartAnswerService,
            LessonPartQuestionService lessonPartQuestionService,
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
            _lessonPartQuestionService = lessonPartQuestionService;
            _lessonPartAnswerService = lessonPartAnswerService;
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
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            else
            {
                filter.Add(Builders<ExamEntity>.Filter.Where(o => o.StudentID == userId));
            }
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
                    return new JsonResult(new Dictionary<string, object>
                    {
                       { "Error", "No Class for Student" }
                    });
                }

                var _schedule = _lessonScheduleService.CreateQuery().Find(o => o.LessonID == _lesson.ID && o.ClassID == _class.ID).FirstOrDefault();
                if (_schedule == null)
                {
                    return new JsonResult(new Dictionary<string, object>
                    {
                       { "Error", "No Schedule for Class" }
                    });
                }

                var marked = _service.CreateQuery().Find(o => o.LessonScheduleID == _schedule.ID && o.StudentID == userid && o.Marked).FirstOrDefault();
                if (marked != null)
                {
                    return new JsonResult(new Dictionary<string, object>
                    {
                       { "Error", "This exam has been marked!" }
                    });
                }

                item.LessonScheduleID = _schedule.ID;
                item.Timer = _lesson.Timer;
                item.Point = 0;
                item.MaxPoint = _lesson.Point;
                item.StudentID = userid;
                item.TeacherID = _class.TeacherID;
                item.ID = null;
                item.Created = DateTime.Now;
                item.CurrentDoTime = DateTime.Now;
                item.Status = false;
                item.Number = (int)_service.CreateQuery().CountDocuments(o => o.LessonScheduleID == item.LessonScheduleID && o.StudentID == item.StudentID) + 1;
                item.Marked = false;
            }
            else
            {
                item.Updated = DateTime.Now;
            }
            _service.CreateOrUpdate(item);
            return new JsonResult(new Dictionary<string, object>
                    {
                       { "Data", item }
                    });
        }

        [HttpPost]
        public JsonResult GetCurrentExam(string ClassID, string LessonID)
        {
            var userID = User.Claims.GetClaimByType("UserID").Value;
            var x = _service.CreateQuery().Find(o => o.ClassID == ClassID && o.LessonID == LessonID && o.Status == false && o.StudentID == userID).FirstOrDefault();
            if (x != null)
                x.CurrentDoTime = DateTime.Now;
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
                        if (!String.IsNullOrEmpty(item.QuestionID))
                        {
                            var question = _cloneLessonPartQuestionService.GetItemByID(item.QuestionID);
                            if (question == null)
                                return new JsonResult("Data Error");
                            item.LessonPartID = question.ParentID;
                            item.QuestionValue = question.Content;
                            item.MaxPoint = question.Point;
                        }
                        else // Essay
                        {
                            item.MaxPoint = exam.MaxPoint;
                        }
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
        [Obsolete]
        public JsonResult CompleteExam(string ExamID)
        {
            var exam = _service.GetItemByID(ExamID);

            if (exam == null)
            {
                return new JsonResult("Data not found");
            }
            if (exam.Status)
            {
                return new JsonResult("Access deny");
            }
            exam.Status = true;

            var lesson = _lessonService.GetItemByID(exam.LessonID);

            double point = 0;

            var userID = User.Claims.GetClaimByType("UserID").Value;
            var listDetails = _examDetailService.Collection.Find(o => o.ExamID == exam.ID).ToList();

            for (int i = 0; listDetails != null && i < listDetails.Count; i++)
            {
                var examDetail = listDetails[i];

                //bài tự luận
                if (string.IsNullOrEmpty(examDetail.QuestionID) || examDetail.QuestionID == "0") continue;

                var part = _cloneLessonPartService.GetItemByID(examDetail.LessonPartID);
                if (part == null) continue; //Lưu lỗi => bỏ qua ko tính điểm

                var question = _cloneLessonPartQuestionService.GetItemByID(examDetail.QuestionID);
                if (question == null) continue; //Lưu lỗi => bỏ qua ko tính điểm

                var _realAnswers = _cloneLessonPartAnswerService.CreateQuery().Find(o => o.IsCorrect && o.ParentID == examDetail.QuestionID).ToList();

                CloneLessonPartAnswerEntity _correctanswer = null;

                var realanswer = _realAnswers.FirstOrDefault();
                if (realanswer != null)
                {
                    examDetail.RealAnswerID = realanswer.ID;
                    examDetail.RealAnswerValue = realanswer.Content;
                }

                //bài chọn hoặc nối đáp án
                if (!string.IsNullOrEmpty(examDetail.AnswerID))
                {
                    var answer = _cloneLessonPartAnswerService.GetItemByID(examDetail.AnswerID);
                    if (answer == null) continue;//Lưu lỗi => bỏ qua ko tính điểm


                    switch (part.Type)
                    {
                        case "QUIZ1": //chọn đáp án
                            _correctanswer = _realAnswers.FirstOrDefault(t => t.ID == answer.ID);//chọn đúng đáp án
                            break;
                        case "QUIZ3": //nối đáp án
                            _correctanswer = _realAnswers.FirstOrDefault(t => t.ID == answer.ID || (!string.IsNullOrEmpty(t.Content) && t.Content == answer.Content)); //chọn đúng đáp án (check trường hợp sai ID nhưng cùng content (2 đáp án có hình ảnh, ID khác nhau nhưng cùng content (nội dung như nhau)))
                            break;
                    }
                }
                else //bài điền từ
                {
                    _correctanswer = _realAnswers.FirstOrDefault(t => t.Content.ToLower() == examDetail.AnswerValue.ToLower()); //điền từ đúng, chấp nhận viết hoa viết thường
                }

                if (_correctanswer != null)
                {
                    point += question.Point;
                    examDetail.Point = question.Point;
                    examDetail.RealAnswerID = _correctanswer.ID;
                    examDetail.RealAnswerValue = _correctanswer.Content;
                }

                examDetail.Updated = DateTime.Now;
                _examDetailService.CreateOrUpdate(examDetail);
            }
            exam.Point = point;
            exam.Updated = DateTime.Now;
            exam.MaxPoint = lesson.Point;
            exam.QuestionsDone = listDetails.Count();
            //Tổng số câu hỏi = tổng số câu hỏi + số phần tự luận
            exam.QuestionsTotal =
                _cloneLessonPartQuestionService.Collection.Count(t => t.LessonID == lesson.ID) +
                _cloneLessonPartService.Collection.Count(t => t.ParentID == lesson.ID && t.Type == "essay");

            _service.CreateOrUpdate(exam);
            return new JsonResult(new { Point = point, MaxPoint = lesson.Point });
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