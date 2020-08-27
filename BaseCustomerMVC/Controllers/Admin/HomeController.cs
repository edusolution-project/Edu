using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using Core_v2.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseCustomerMVC.Controllers.Admin
{
    [BaseAccess.Attribule.AccessCtrl("Trang chủ", "admin", false)]
    public class HomeController : AdminController
    {
        private readonly CourseLessonService _lessonService;
        private readonly LessonPartService _lessonPartService;
        private readonly LessonPartQuestionService _questionService;

        private readonly LessonService _clonelessonService;
        private readonly CloneLessonPartService _clonelessonPartService;
        private readonly CloneLessonPartQuestionService _clonequestionService;
        private readonly CloneLessonPartAnswerService _cloneanswerService;

        private readonly LessonProgressService _lessonProgressService;
        private readonly ChapterProgressService _chapterProgressService;
        private readonly ClassProgressService _classProgressService;
        private readonly ClassSubjectProgressService _classSubjectProgressService;

        private readonly ExamService _examService;
        private readonly ExamDetailService _examDetailService;
        private readonly List<string> quizType = new List<string> { "QUIZ1", "QUIZ2", "QUIZ3", "QUIZ4", "ESSAY" };

        public HomeController(
                CourseLessonService lessonService,
                LessonPartService lessonPartService,
                LessonPartQuestionService questionService,

                LessonService clonelessonService,
                CloneLessonPartService clonelessonPartService,
                CloneLessonPartQuestionService clonequestionService,
                CloneLessonPartAnswerService cloneanswerService,
                ExamService examService,
                ExamDetailService examDetailService,
                LessonProgressService lessonProgressService,
                ChapterProgressService chapterProgressService,
                ClassSubjectProgressService classSubjectProgressService,
                ClassProgressService classProgressService
            )
        {
            _lessonService = lessonService;
            _lessonPartService = lessonPartService;
            _questionService = questionService;

            _clonelessonService = clonelessonService;
            _clonelessonPartService = clonelessonPartService;
            _clonequestionService = clonequestionService;
            _cloneanswerService = cloneanswerService;
            _lessonProgressService = lessonProgressService;
            _chapterProgressService = chapterProgressService;
            _classSubjectProgressService = classSubjectProgressService;
            _classProgressService = classProgressService;

            _examService = examService;
            _examDetailService = examDetailService;
        }

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        //public JsonResult FixScoreData()//Big fix
        //{
        //    //calculate lesson maxpoint
        //    var courselessons = _lessonService.GetAll().ToEnumerable();
        //    foreach (var cl in courselessons)
        //        calculateLessonPoint(cl.ID);
        //    //calculate clone lesson maxpoint
        //    var lessons = _clonelessonService.GetAll().ToEnumerable();
        //    foreach (var l in lessons)
        //        calculateCloneLessonPoint(l.ID);
        //    //reapply exam maxpoint
        //    var exams = _examService.GetAll().ToEnumerable();
        //    foreach (var e in exams)
        //    {
        //        var lesson = _clonelessonService.GetItemByID(e.LessonID);
        //        if (lesson != null)
        //        {
        //            if (e.MaxPoint == 0) e.MaxPoint = lesson.Point;
        //            _examService.Save(e);
        //        }
        //        else
        //        {
        //            var parts = _clonelessonPartService.GetByLessonID(e.LessonID);//remove orphan parts
        //            foreach (var part in parts)
        //            {
        //                _examDetailService.Collection.DeleteManyAsync(t => t.LessonPartID == part.ID);
        //                var quizs = _clonequestionService.GetByPartID(part.ID);
        //                foreach (var quiz in quizs)
        //                {
        //                    _cloneanswerService.Collection.DeleteManyAsync(t => t.ParentID == quiz.ID);
        //                    _ = _clonequestionService.RemoveAsync(quiz.ID);
        //                }
        //                _ = _clonelessonPartService.RemoveAsync(part.ID);
        //            }
        //            _ = _examService.RemoveAsync(e.ID);
        //        }
        //    }

        //    //reset progress

        //    _chapterProgressService.CreateQuery().UpdateMany(t => true, Builders<ChapterProgressEntity>.Update
        //        .Set(t => t.TotalPoint, 0)
        //        .Set(t => t.PracticePoint, 0)
        //        .Set(t => t.PracticeAvgPoint, 0)
        //        .Set(t => t.AvgPoint, 0)
        //        .Set(t => t.ExamDone, 0)
        //        .Set(t => t.PracticeCount, 0)
        //        );


        //    _classSubjectProgressService.CreateQuery().UpdateMany(t => true, Builders<ClassSubjectProgressEntity>.Update
        //        .Set(t => t.AvgPoint, 0)
        //        .Set(t => t.TotalPoint, 0)
        //        .Set(t => t.ExamDone, 0)
        //        );

        //    _classProgressService.CreateQuery().UpdateMany(t => true, Builders<ClassProgressEntity>.Update
        //        .Set(t => t.AvgPoint, 0)
        //        .Set(t => t.TotalPoint, 0)
        //        .Set(t => t.ExamDone, 0)
        //        );

        //    _lessonProgressService.CreateQuery().UpdateMany(t => true, Builders<LessonProgressEntity>.Update
        //        .Set(t => t.AvgPoint, 0)
        //        .Set(t => t.LastPoint, 0)
        //        .Set(t => t.LastTry, DateTime.MinValue)
        //        .Set(t => t.MaxPoint, 0)
        //        .Set(t => t.MinPoint, 0)
        //        .Set(t => t.Tried, 0)
        //        );


        // POST: Home/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }



        public JsonResult FixData()
        {
            var courselessons = _lessonService.GetAll().ToEnumerable();
            foreach (var cl in courselessons)
                calculateCloneLessonPoint(cl.ID);
            var lessons = _clonelessonService.GetAll().ToEnumerable();
            foreach (var l in lessons)
                calculateCloneLessonPoint(l.ID);
            return Json("OK");
        }

        private double calculateLessonPoint(string lessonId)
        {
            var point = 0.0;
            var parts = _lessonPartService.GetByLessonID(lessonId).Where(t => quizType.Contains(t.Type));
            foreach (var part in parts)
            {
                if (part.Type == "ESSAY")
                {
                    point += part.Point;
                }
                else
                {
                    point += _questionService.GetByPartID(part.ID).Count();//trắc nghiệm => điểm = số câu hỏi (mỗi câu 1đ)
                }
            }
            _lessonService.UpdateLessonPoint(lessonId, point);
            return point;
        }

        private double calculateCloneLessonPoint(string lessonId)
        {
            var point = 0.0;
            var parts = _clonelessonPartService.GetByLessonID(lessonId).Where(t => quizType.Contains(t.Type));
            foreach (var part in parts)
            {
                if (part.Type == "ESSAY")
                {
                    point += part.Point;
                }
                else
                {
                    point += _clonequestionService.GetByPartID(part.ID).Count();//trắc nghiệm => điểm = số câu hỏi (mỗi câu 1đ)
                }
            }
            _clonelessonService.UpdateLessonPoint(lessonId, point);
            return point;
        }
    }
}
