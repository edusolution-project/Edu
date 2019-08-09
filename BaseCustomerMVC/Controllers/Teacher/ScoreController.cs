using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using MongoDB.Driver;
using System.Linq;
using Core_v2.Globals;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class ScoreController : TeacherController
    {
        private readonly ScoreService _scoreService;

        private readonly ExamService _examService;
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

        public ScoreController(ExamService examService,
            ExamDetailService examDetailService
        , StudentService studentService
            , ScoreService scoreService
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
            _scoreService = scoreService;
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
            _lessonPartQuestionService = lessonPartQuestionService;
            _lessonPartAnswerService = lessonPartAnswerService;
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
        public JsonResult Save(string ExamID, List<ExamDetailEntity> ExamDetails)
        {
            var exam = _examService.GetItemByID(ExamID);
            if (exam == null)
            {
                return new JsonResult(new Dictionary<string, object>{
                    {"Error", "Bài kiểm tra không tồn tại" }
                });
            }

            var lesson = _lessonService.GetItemByID(exam.LessonID);
            if (lesson == null)
            {
                return new JsonResult(new Dictionary<string, object>{
                    {"Error", "Có lỗi dữ liệu" }
                });
            }

            exam.Marked = true;
            exam.Updated = DateTime.Now;

            var score = new ScoreEntity()
            {
                ClassID = exam.ClassID,
                LessonID = exam.LessonID,
                Multiple = lesson.Multiple,
                ScoreType = lesson.Etype,
                StudentID = exam.StudentID,
                TeacherID = exam.TeacherID,
                Updated = DateTime.Now,
                Point = 0,
                MaxPoint = lesson.Point
            };
            var point = 0.0;
            if (ExamDetails != null && ExamDetails.Count > 0)
            {
                foreach (var detail in ExamDetails)
                {
                    var _detail = _examDetailService.GetItemByID(detail.ID);
                    if (_detail != null)
                    {
                        _detail.Point = detail.Point;
                        point += detail.Point;
                        _detail.Updated = DateTime.Now;
                        _examDetailService.CreateQuery().ReplaceOne(e => e.ID == _detail.ID, _detail);
                    }

                }
            }

            exam.Point = point;
            score.Point = point;

            _examService.CreateQuery().ReplaceOne(e => e.ID == exam.ID, exam);
            _scoreService.CreateQuery().InsertOne(score);
            return new JsonResult(new Dictionary<string, object>{
                    {"Data", "Đã lưu bảng điểm" }
                });
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetClassScores(string ClassID)
        {
            if (string.IsNullOrEmpty(ClassID))
            {
                return new JsonResult(new Dictionary<string, object>{
                    {"Error", "Lớp không tồn tại" }
                });
            }

            var currentClass = _classService.GetItemByID(ClassID);
            if (currentClass == null)
            {
                return new JsonResult(new Dictionary<string, object>{
                    {"Error", "Lớp không tồn tại" }
                });
            }

            var scores = _scoreService.CreateQuery().Find(t => t.ClassID == ClassID).ToList();
            var scorelist = scores.Where(o => o.ClassID == ClassID)
               .GroupBy(o => o.StudentID).Select(r => new ScoreSummaryViewModel
               {
                   StudentID = r.Key,
                   PartialScore = r.Sum(t => t.ScoreType == LESSON_ETYPE.PARTIAL ? t.Point * t.Multiple : 0),
                   PartialSum = r.Sum(t => t.ScoreType == LESSON_ETYPE.PARTIAL ? t.Multiple : 0),
                   EndingScore = r.Sum(t => t.ScoreType == LESSON_ETYPE.END ? t.Point * t.Multiple : 0),
                   EndingSum = r.Sum(t => t.ScoreType == LESSON_ETYPE.END ? t.Multiple : 0),
               }).ToList();

            if (scorelist.Count() > 0)
            {
                foreach (var score in scorelist)
                {
                    score.StudentName = _studentService.GetItemByID(score.StudentID).FullName;
                    var multipleSum = score.EndingSum + score.PartialSum;
                    score.AvgScore = (multipleSum > 0) ? (score.PartialScore + score.EndingScore) / multipleSum : 0;
                    score.Scores = scores.FindAll(t => t.StudentID == score.StudentID);
                    score.AvgPartial = score.PartialSum > 0 ? score.PartialScore / score.PartialSum : 0;
                    score.AvgEnd = score.EndingSum > 0 ? score.EndingScore / score.EndingSum : 0;
                }
            }

            return new JsonResult(new Dictionary<string, object>{
                    {"Data", scorelist }
                });
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetStudentScores(string ClassID, string StudentID)
        {
            if (string.IsNullOrEmpty(ClassID))
            {
                return new JsonResult(new Dictionary<string, object>{
                    {"Error", "Lớp không tồn tại" }
                });
            }

            var currentClass = _classService.GetItemByID(ClassID);
            if (currentClass == null)
            {
                return new JsonResult(new Dictionary<string, object>{
                    {"Error", "Lớp không tồn tại" }
                });
            }

            if (string.IsNullOrEmpty(StudentID))
            {
                return new JsonResult(new Dictionary<string, object>{
                    {"Error", "Học viên không tồn tại" }
                });
            }

            var student = _studentService.GetItemByID(StudentID);
            if (student == null)
            {
                return new JsonResult(new Dictionary<string, object>{
                    {"Error", "Học viên không tồn tại" }
                });
            }

            var scores = _scoreService.CreateQuery().Find(t => t.ClassID == ClassID && t.StudentID == StudentID).ToList();

            var mapping = new MappingEntity<ScoreEntity, ScoreDetailViewModel>();
            var returnData = scores.Select(t => mapping.AutoOrtherType(t, new ScoreDetailViewModel()
            {
                StudentName = student.FullName,
                LessonName = _lessonService.GetItemByID(t.LessonID).Title,
                ExamID = _examService.CreateQuery().Find(e => e.Marked && t.ClassID == ClassID && e.StudentID == StudentID).FirstOrDefault().ID
            })).ToList();

            return new JsonResult(new Dictionary<string, object>{
                    {"Data", returnData }
                });
        }
    }
}
