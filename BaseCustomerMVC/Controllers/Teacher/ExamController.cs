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

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class ExamController : TeacherController
    {

        private readonly ExamService _service;
        private readonly ExamDetailService _examDetailService;
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly LessonService _lessonService;

        private readonly LessonPartQuestionService _lessonPartQuestionService;
        private readonly LessonPartAnswerService _lessonPartAnswerService;

        private readonly CloneLessonPartService _cloneLessonPartService;
        private readonly CloneLessonPartAnswerService _cloneLessonPartAnswerService;
        private readonly CloneLessonPartQuestionService _cloneLessonPartQuestionService;

        private readonly TeacherService _teacherService;
        private readonly StudentService _studentService;
        private readonly ClassService _classService;
        private readonly LearningHistoryService _learningHistoryService;

        public ExamController(ExamService service,
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
            , LessonPartQuestionService lessonPartQuestionService
            , LessonPartAnswerService lessonPartAnswerService
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
            _lessonPartQuestionService = lessonPartQuestionService;
            _lessonPartAnswerService = lessonPartAnswerService;
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetListStudents(DefaultModel model, string ClassID)
        {
            var lessonid = model.ID;
            var lesson = _lessonService.GetItemByID(lessonid);
            if (lesson == null)
            {
                return new JsonResult(new Dictionary<string, object>{
                    {"Error", "Bài kiểm tra không tồn tại" }
                    });
            }

            var list = _service.CreateQuery().AsQueryable()
            .Where(o => o.ClassID == ClassID && o.LessonID == lessonid)
            .GroupBy(o => o.StudentID).Select(r => new ExamEntity { ID = r.Max(t => t.ID), StudentID = r.Key }).ToList();
            var returnData = (from r in list
                              let student = _studentService.GetItemByID(r.StudentID)
                              let exam = _service.GetItemByID(r.ID)
                              select new ExamViewModel
                              {
                                  ID = r.ID,
                                  StudentID = student.ID,
                                  StudentName = student.FullName,
                                  Created = exam.Created,
                                  Status = exam.Status,
                                  Marked = exam.Marked,
                                  Point = exam.Point,
                                  MaxPoint = exam.MaxPoint
                              }).ToList();



            //var filter = new List<FilterDefinition<ExamEntity>>();
            //var data = filter.Count > 0 ? _service.Collection.Find(Builders<ExamEntity>.Filter.And(filter)) : _service.GetAll();
            //model.TotalRecord = data.Count();
            //var DataResponse = data == null || data.Count() <= 0 || data.Count() < model.PageSize
            //    ? data.ToList()
            //    : data.Skip((model.PageIndex - 1) * model.PageSize).Limit(model.PageSize).ToList();
            //var mapping = new MappingEntity<ExamEntity, ExamViewModel>();
            //var std = DataResponse.Select(o => mapping.AutoOrtherType(o, new ExamViewModel()
            //{
            //    LessonScheduleName = _lessonService.GetItemByID(_lessonScheduleService.GetItemByID(o.LessonScheduleID).LessonID).Title,
            //    StudentName = _studentService.GetItemByID(o.StudentID).FullName
            //})).ToList();

            var response = new Dictionary<string, object>
            {
                { "Data", returnData },
                { "Model", model },
                { "Error", null }
            };
            return new JsonResult(response);
        }


        [System.Obsolete]
        [HttpPost]
        public JsonResult GetDetail(string ID)
        {
            try
            {
                var exam = _service.GetItemByID(ID);
                var lesson = _lessonService.GetItemByID(exam.LessonID);
                var student = _studentService.GetItemByID(exam.StudentID);
                var currentClass = _classService.GetItemByID(exam.ClassID);
                var parts = _cloneLessonPartService.CreateQuery().Find(t => t.ParentID == lesson.ID && t.TeacherID == currentClass.TeacherID).ToList();

                if (exam == null || lesson == null || student == null || currentClass == null)
                    return new JsonResult(new Dictionary<string, object>
                    {

                        { "Data", null },
                        {"Error", "Data Error" }
                    });



                var filter = Builders<ExamDetailEntity>.Filter.Where(o => o.ExamID == ID);
                var examdetails = _examDetailService.Collection.Find(filter);

                if (examdetails.Count() == 0)
                {
                    return new JsonResult(new Dictionary<string, object>
                    {

                        { "Data", null },
                        {"Error", "No data" }
                    });
                }

                var _examdetails = examdetails.ToList();

                var mapping = new MappingEntity<ExamDetailEntity, TeacherExamDetailViewModel>();
                var partmapping = new MappingEntity<CloneLessonPartEntity, ExamPartViewModel>();


                var result = new MappingEntity<ExamEntity, TeacherExamViewModel>().AutoOrtherType(exam, new TeacherExamViewModel()
                {
                    StudentName = student.FullName,
                    ClassName = currentClass.Name,
                    LessonName = lesson.Title,
                    Multiple = lesson.Multiple,
                    TeacherName = _teacherService.GetItemByID(exam.TeacherID).FullName,
                    MarkDate = exam.Marked ? exam.Updated : DateTime.MinValue
                });

                result.Parts = (from p in parts
                                select partmapping.AutoOrtherType(p, new ExamPartViewModel()
                                {
                                    ExamDetails = (from r in _examdetails
                                                   where r.LessonPartID == p.ID
                                                   let answer = r.AnswerID != null ? _cloneLessonPartAnswerService.GetItemByID(r.AnswerID)
                                                       ?? _lessonPartAnswerService.GetItemByID(r.AnswerID) //TEMP
                                                       : null
                                                   let question = _cloneLessonPartQuestionService.GetItemByID(r.QuestionID)
                                                   ?? _lessonPartQuestionService.GetItemByID(r.QuestionID) //TEMP
                                                   where question != null
                                                   select
                                                   new TeacherExamDetailViewModel()
                                                   {
                                                       ID = r.ID,
                                                       LessonPartID = question.ParentID,
                                                       QuestionID = question.ID,
                                                       AnswerID = answer != null ? answer.ID : null,
                                                       AnswerValue = answer != null ? answer.Content : r.AnswerValue,
                                                       QuestionValue = question != null ? question.Content : r.QuestionValue,
                                                       RealAnswerValue = r.RealAnswerValue,
                                                       Point = r.Point,
                                                       MaxPoint = question.Point,
                                                   }
                                    ).ToList()
                                })).ToList();


                var response = new Dictionary<string, object>
                {
                    { "Data", result }
                };
                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                var response = new Dictionary<string, object>
                {

                    { "Data", null },
                    {"Error",ex }
                };
                return new JsonResult(response);
            }
        }

        public IActionResult Detail(DefaultModel model)
        {
            if (model == null) return null;
            var currentExam = _service.GetItemByID(model.ID);
            if (currentExam == null)
                return RedirectToAction("Index", "Class");
            var lesson = _lessonService.GetItemByID(currentExam.LessonID);
            ViewBag.Lesson = lesson;
            ViewBag.Class = _classService.GetItemByID(currentExam.ClassID);
            ViewBag.Exam = currentExam;
            return View();
        }
    }
}
