using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using Core_v2.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseCustomerMVC.Controllers.Admin
{
    [BaseAccess.Attribule.AccessCtrl("Trang chủ", "admin", false)]
    public class HomeController : AdminController
    {
        private readonly CourseService _courseService;
        private readonly CourseChapterService _courseChapterService;
        private readonly ClassService _classService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly ChapterService _chapterService;

        private readonly ClassHelper _classHelper;
        private readonly CourseHelper _courseHelper;

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
                ClassProgressService classProgressService,

                CourseService courseService,
                CourseChapterService courseChapterService,
                ClassService classService,
                ClassSubjectService classSubjectService,
                ChapterService chapterService,

                ClassHelper classHelper,
                CourseHelper courseHelper
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

            _courseService = courseService;
            _courseChapterService = courseChapterService;
            _classService = classService;
            _classSubjectService = classSubjectService;
            _chapterService = chapterService;

            _classHelper = classHelper;
            _courseHelper = courseHelper;
        }

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult FixScoreData()//Big fix
        {
            _chapterService.CreateQuery().UpdateMany(t => true, Builders<ChapterEntity>.Update.Set(t => t.TotalExams, 0).Set(t => t.TotalLessons, 0).Set(t => t.TotalPractices, 0));
            _classSubjectService.CreateQuery().UpdateMany(t => true, Builders<ClassSubjectEntity>.Update.Set(t => t.TotalExams, 0).Set(t => t.TotalLessons, 0).Set(t => t.TotalPractices, 0));
            _classService.CreateQuery().UpdateMany(t => true, Builders<ClassEntity>.Update.Set(t => t.TotalExams, 0).Set(t => t.TotalLessons, 0).Set(t => t.TotalPractices, 0));
            _courseChapterService.CreateQuery().UpdateMany(t => true, Builders<CourseChapterEntity>.Update.Set(t => t.TotalExams, 0).Set(t => t.TotalLessons, 0).Set(t => t.TotalPractices, 0));
            _courseService.CreateQuery().UpdateMany(t => true, Builders<CourseEntity>.Update.Set(t => t.TotalExams, 0).Set(t => t.TotalLessons, 0).Set(t => t.TotalPractices, 0));

            //calculate lesson maxpoint
            var courselessons = _lessonService.GetAll().ToEnumerable();
            foreach (var cl in courselessons)
                calculateLessonPoint(cl);
            //calculate clone lesson maxpoint
            var lessons = _clonelessonService.GetAll().ToEnumerable();
            foreach (var l in lessons)
                calculateCloneLessonPoint(l);
            //reapply exam maxpoint
            var exams = _examService.GetAll().ToEnumerable();
            foreach (var e in exams)
            {
                var lesson = _clonelessonService.GetItemByID(e.LessonID);
                if (lesson != null)
                {
                    if (e.MaxPoint == 0) e.MaxPoint = lesson.Point;
                    _examService.Save(e);
                }
                else
                {
                    var parts = _clonelessonPartService.GetByLessonID(e.LessonID);//remove orphan parts
                    foreach (var part in parts)
                    {
                        _examDetailService.Collection.DeleteManyAsync(t => t.LessonPartID == part.ID);
                        var quizs = _clonequestionService.GetByPartID(part.ID);
                        foreach (var quiz in quizs)
                        {
                            _cloneanswerService.Collection.DeleteManyAsync(t => t.ParentID == quiz.ID);
                            _ = _clonequestionService.RemoveAsync(quiz.ID);
                        }
                        _ = _clonelessonPartService.RemoveAsync(part.ID);
                    }
                    _ = _examService.RemoveAsync(e.ID);
                }
            }

            //reset progress

            _chapterProgressService.CreateQuery().UpdateMany(t => true, Builders<ChapterProgressEntity>.Update
                .Set(t => t.TotalPoint, 0)
                .Set(t => t.PracticePoint, 0)
                .Set(t => t.PracticeAvgPoint, 0)
                .Set(t => t.AvgPoint, 0)
                .Set(t => t.ExamDone, 0)
                .Set(t => t.PracticeCount, 0)
                );


            _classSubjectProgressService.CreateQuery().UpdateMany(t => true, Builders<ClassSubjectProgressEntity>.Update
                .Set(t => t.AvgPoint, 0)
                .Set(t => t.TotalPoint, 0)
                .Set(t => t.ExamDone, 0)
                );

            _classProgressService.CreateQuery().UpdateMany(t => true, Builders<ClassProgressEntity>.Update
                .Set(t => t.AvgPoint, 0)
                .Set(t => t.TotalPoint, 0)
                .Set(t => t.ExamDone, 0)
                );

            _lessonProgressService.CreateQuery().UpdateMany(t => true, Builders<LessonProgressEntity>.Update
                .Set(t => t.AvgPoint, 0)
                .Set(t => t.LastPoint, 0)
                .Set(t => t.LastTry, DateTime.MinValue)
                .Set(t => t.MaxPoint, 0)
                .Set(t => t.MinPoint, 0)
                .Set(t => t.Tried, 0)
                );


            var lessonProgresses = _lessonProgressService.GetAll().ToList();

            //recalculate point
            foreach (var lp in lessonProgresses)
            {
                var ls = _clonelessonService.GetItemByID(lp.LessonID);
                if (ls == null)
                {
                    _lessonProgressService.Remove(lp.ID);
                }
                else
                {
                    var exs = _examService.CreateQuery().Find(t => t.LessonID == lp.LessonID && t.StudentID == lp.StudentID).SortBy(t => t.Number);
                    foreach (var ex in exs.ToEnumerable())
                    {
                        if (ex.Marked) _examService.CompleteFull(ex, ls, out _, false);
                        else
                            if (_examService.IsOver(ex.ID))
                            _examService.CompleteNoEssay(ex, ls, out _, false);
                    }
                }
            }

            return Json("OK");
        }

        private double calculateLessonPoint(CourseLessonEntity lesson)
        {
            var point = 0.0;
            var parts = _lessonPartService.GetByLessonID(lesson.ID).Where(t => quizType.Contains(t.Type));

            var hasPractice = false;
            if (parts != null && parts.Count() > 0)
            {
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
                hasPractice = true;
            }

            lesson.Point = point;
            if (lesson.TemplateType == LESSON_TEMPLATE.EXAM)
            {
                lesson.IsPractice = false;
            }
            else
            {
                if (hasPractice)
                    lesson.IsPractice = true;
                else
                    lesson.IsPractice = false;
            }
            _lessonService.Save(lesson);

            if (lesson.ChapterID != "0")
                _ = _courseHelper.IncreaseCourseChapterCounter(lesson.ChapterID, 1, lesson.TemplateType == LESSON_TEMPLATE.EXAM ? 1 : 0, lesson.IsPractice ? 1 : 0);
            else
                _ = _courseHelper.IncreaseCourseCounter(lesson.CourseID, 1, lesson.TemplateType == LESSON_TEMPLATE.EXAM ? 1 : 0, lesson.IsPractice ? 1 : 0);

            return point;
        }

        private double calculateCloneLessonPoint(LessonEntity lesson)
        {
            var point = 0.0;
            var parts = _clonelessonPartService.GetByLessonID(lesson.ID).Where(t => quizType.Contains(t.Type));

            var hasPractice = false;
            if (parts != null && parts.Count() > 0)
            {
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
                hasPractice = true;
            }


            lesson.Point = point;
            if (lesson.TemplateType == LESSON_TEMPLATE.EXAM)
            {
                lesson.IsPractice = false;
            }
            else
            {
                if (hasPractice)
                    lesson.IsPractice = true;
                else
                    lesson.IsPractice = false;
            }
            _clonelessonService.Save(lesson);

            if (lesson.ChapterID != "0")
                _ = _classHelper.IncreaseChapterCounter(lesson.ChapterID, 1, lesson.TemplateType == LESSON_TEMPLATE.EXAM ? 1 : 0, lesson.IsPractice ? 1 : 0);
            else
                _ = _classHelper.IncreaseClassSubjectCounter(lesson.ClassSubjectID, 1, lesson.TemplateType == LESSON_TEMPLATE.EXAM ? 1 : 0, lesson.IsPractice ? 1 : 0);

            return point;
        }
    }
}
