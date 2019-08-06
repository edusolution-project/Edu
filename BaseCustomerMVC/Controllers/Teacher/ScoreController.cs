using BaseCustomerMVC.Globals;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using BaseCustomerMVC.Models;
using BaseCustomerEntity.Database;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class ScoreController : TeacherController
    {

        private readonly ExamService _examService;
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

        public ScoreController(ExamService examService,
            ExamDetailService examDetailService, 
            StudentService studentService, 
            ClassService classService, 
            LessonService lessonService,
            LessonScheduleService lessonScheduleService,
            CloneLessonPartService cloneLessonPartService,
            CloneLessonPartAnswerService cloneLessonPartAnswerService,
            CloneLessonPartQuestionService cloneLessonPartQuestionService,
            TeacherService teacherService,
            LearningHistoryService learningHistoryService
            )
        {
            _learningHistoryService = learningHistoryService;
            _examService = examService;
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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(DefaultModel model)
        {
            return View();
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetList(DefaultModel model, string ClassID)
        {

            var lessonid = model.ID;


            var respone = new Dictionary<string, object>
            {
                { "Data", model },
                { "Model", model },
                { "Error", null }
            };
            return new JsonResult(respone);
        }
    }
}
