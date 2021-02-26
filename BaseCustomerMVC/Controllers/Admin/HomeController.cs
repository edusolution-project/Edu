using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using Core_v2.Interfaces;
using Google.Apis.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Core.Misc;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using RestSharp;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
        private readonly LessonHelper _lessonHelper;
        private readonly ProgressHelper _progressHelper;
        private readonly CalendarHelper _calendarHelper;

        private readonly CourseLessonService _courseLessonService;
        private readonly LessonPartService _lessonPartService;
        private readonly LessonPartQuestionService _questionService;
        private readonly LessonPartAnswerService _answerService;

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

        private readonly CenterService _centerService;
        private readonly StudentService _studentService;
        private readonly TeacherService _teacherService;

        private readonly ReferenceService _referenceService;
        private readonly CalendarService _calendarService;
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly LearningHistoryService _learningHistoryService;

        private readonly AccountService _accountService;
        private readonly LessonService _lessonService;
        private readonly MailHelper _mailHelper;

        private string host;
        private string staticPath;
        private string RootPath { get; }
        private readonly IHostingEnvironment _env;

        public HomeController(
                CourseLessonService courseLessonService,
                LessonPartService lessonPartService,
                LessonPartQuestionService questionService,
                LessonPartAnswerService answerService,

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
                CourseHelper courseHelper,
                LessonHelper lessonHelper,
                ProgressHelper progressHelper,
                CalendarHelper calendarHelper,

                CenterService centerService,
                StudentService studentService,
                TeacherService teacherService,
                IConfiguration iConfig,
                IHostingEnvironment env,
                ReferenceService referenceService,
                CalendarService calendarService,
                LessonScheduleService lessonScheduleService,
                LearningHistoryService learningHistoryService,
                AccountService accountService,
                LessonService lessonService,
                MailHelper mailHelper
            )
        {
            _courseLessonService = courseLessonService;
            _lessonPartService = lessonPartService;
            _questionService = questionService;
            _answerService = answerService;

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
            _lessonHelper = lessonHelper;
            _progressHelper = progressHelper;
            _calendarHelper = calendarHelper;

            _centerService = centerService;
            _studentService = studentService;
            _teacherService = teacherService;
            _referenceService = referenceService;
            _calendarService = calendarService;
            _lessonScheduleService = lessonScheduleService;
            _learningHistoryService = learningHistoryService;

            _accountService = accountService;
            _lessonService = lessonService;
            _mailHelper = mailHelper;

            _env = env;

            host = iConfig.GetValue<string>("SysConfig:Domain");
            staticPath = iConfig.GetValue<string>("SysConfig:StaticPath");
            RootPath = staticPath ?? _env.WebRootPath;
        }

        // GET: Home
        public ActionResult Index()
        {
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            var account = _accountService.GetItemByID(UserID);
            if (account != null)
            {
                ViewBag.Account = account;
            }
            return View();
        }



        #region Fix Region      
        //public async Task<JsonResult> FixScoreData()//Big fix
        //{
        //    var start = DateTime.UtcNow;
        //    var str = "";
        //    str += "Phase 0: ";
        //    //Clear orphan part: Run Once

        //    //var ids = _courseLessonService.GetAll().Project(t => t.ID).ToList();


        //    //long delpart = 0;
        //    //var delIds = _lessonPartService.CreateQuery().Find(t => !ids.Contains(t.ParentID)).Project(t => t.ID).ToList();

        //    //foreach (var partid in delIds)
        //    //{
        //    //    var part = _lessonPartService.GetItemByID(partid);
        //    //    if (_courseLessonService.GetItemByID(part.ParentID) == null)
        //    //    {
        //    //        _lessonPartService.Remove(partid);
        //    //        var quizs = _questionService.GetByPartID(partid);
        //    //        if (quizs != null && quizs.Count() > 0)
        //    //        {
        //    //            foreach (var quiz in quizs)
        //    //            {
        //    //                _questionService.Remove(quiz.ID);
        //    //                await _answerService.CreateQuery().DeleteManyAsync<LessonPartAnswerEntity>(t => t.ParentID == quiz.ID);
        //    //            }
        //    //        }
        //    //        delpart++;
        //    //    }
        //    //}

        //    //var cdelpart = 0;

        //    //var cids = _clonelessonService.GetAll().Project(t => t.ID).ToList();
        //    //var delcIds = _clonelessonPartService.CreateQuery().Find(t => !cids.Contains(t.ParentID)).Project(t => t.ID).ToList();

        //    //foreach (var partid in delcIds)
        //    //{
        //    //    var part = _clonelessonPartService.GetItemByID(partid);
        //    //    if (_clonelessonService.GetItemByID(part.ParentID) == null)
        //    //    {
        //    //        _clonelessonPartService.Remove(partid);
        //    //        var quizs = _clonequestionService.GetByPartID(partid);
        //    //        if (quizs != null && quizs.Count() > 0)
        //    //        {
        //    //            foreach (var quiz in quizs)
        //    //            {
        //    //                _clonequestionService.Remove(quiz.ID);
        //    //                await _cloneanswerService.RemoveByParentAsync(quiz.ID);
        //    //            }
        //    //        }
        //    //        cdelpart++;
        //    //    }
        //    //}
        //    //str += (DateTime.UtcNow - start).TotalSeconds;
        //    //start = DateTime.UtcNow;
        //    //str += (" _ DEL " + delpart + "_" + cdelpart + "<DELDONE>");
        //    var change = 0;
        //    //str += "Phase 1: ";
        //    //_chapterService.CreateQuery().UpdateMany(t => true, Builders<ChapterEntity>.Update.Set(t => t.TotalExams, 0).Set(t => t.TotalLessons, 0).Set(t => t.TotalPractices, 0));
        //    //_classSubjectService.CreateQuery().UpdateMany(t => true, Builders<ClassSubjectEntity>.Update.Set(t => t.TotalExams, 0).Set(t => t.TotalLessons, 0).Set(t => t.TotalPractices, 0));
        //    //_classService.CreateQuery().UpdateMany(t => true, Builders<ClassEntity>.Update.Set(t => t.TotalExams, 0).Set(t => t.TotalLessons, 0).Set(t => t.TotalPractices, 0));
        //    //_courseChapterService.CreateQuery().UpdateMany(t => true, Builders<CourseChapterEntity>.Update.Set(t => t.TotalExams, 0).Set(t => t.TotalLessons, 0).Set(t => t.TotalPractices, 0));
        //    //_courseService.CreateQuery().UpdateMany(t => true, Builders<CourseEntity>.Update.Set(t => t.TotalExams, 0).Set(t => t.TotalLessons, 0).Set(t => t.TotalPractices, 0));

        //    //str += (DateTime.UtcNow - start).TotalSeconds;
        //    //start = DateTime.UtcNow;

        //    //calculate lesson maxpoint
        //    var clids = _courseLessonService.GetAll().Project(t => t.ID).ToList();
        //    var listclass = new List<string>();
        //    foreach (var clid in clids)
        //    {
        //        var cl = _courseLessonService.GetItemByID(clid);
        //        var oldpoint = cl.Point;
        //        //if (oldpoint == 0)
        //        //{
        //        var point = calculateLessonPoint(cl);
        //        if (oldpoint != point)
        //        {
        //            if (!listclass.Contains(cl.CourseID))
        //                listclass.Add(cl.CourseID);
        //            change++;
        //        }
        //        //}
        //        //IncreaseCourseCounter(cl);
        //    }
        //    listclass.ForEach(t => str += (t + " __"));
        //    str += (change + " _" + (DateTime.UtcNow - start).TotalSeconds);
        //    //start = DateTime.UtcNow;
        //    //str += " Phase 2: ";
        //    //calculate clone lesson maxpoint
        //    var lids = _clonelessonService.GetAll().Project(t => t.ID).ToList();
        //    //.Project(t => new LessonEntity
        //    //{
        //    //    ID = t.ID,
        //    //    ChapterID = t.ChapterID,
        //    //    ClassSubjectID = t.ClassSubjectID,
        //    //    TemplateType = t.TemplateType,
        //    //    IsPractice = t.IsPractice,
        //    //    Multiple = t.Multiple
        //    //}).ToList();
        //    //change = 0;

        //    foreach (var lid in lids)
        //    {

        //        var l = _clonelessonService.GetItemByID(lid);
        //        var oldpiont = l.Point;
        //        var point = calculateCloneLessonPoint(l);
        //        if (l.Point != point)
        //            change++;
        //        //await _classHelper.IncreaseLessonCounter(l, 1, l.TemplateType == LESSON_TEMPLATE.EXAM ? 1 : 0, l.IsPractice ? 1 : 0);
        //    }
        //    //str += (change + " _" + (DateTime.UtcNow - start).TotalSeconds);
        //    //start = DateTime.UtcNow;
        //    //str += " Phase 3: ";
        //    ////return Json(str);
        //    ////reapply exam maxpoint
        //    //var exams = _examService.GetAll().ToEnumerable();

        //    //foreach (var e in exams)
        //    //{
        //    //    var lesson = _clonelessonService.GetItemByID(e.LessonID);
        //    //    if (lesson != null)
        //    //    {
        //    //        if (e.MaxPoint == 0) e.MaxPoint = lesson.Point;
        //    //        _examService.Save(e);
        //    //    }
        //    //    else
        //    //    {
        //    //        var parts = _clonelessonPartService.GetByLessonID(e.LessonID);//remove orphan parts
        //    //        foreach (var part in parts)
        //    //        {
        //    //            await _examDetailService.Collection.DeleteManyAsync(t => t.LessonPartID == part.ID);
        //    //            var quizs = _clonequestionService.GetByPartID(part.ID);
        //    //            foreach (var quiz in quizs)
        //    //            {
        //    //                await _cloneanswerService.Collection.DeleteManyAsync(t => t.ParentID == quiz.ID);
        //    //                await _clonequestionService.RemoveAsync(quiz.ID);
        //    //            }
        //    //            await _clonelessonPartService.RemoveAsync(part.ID);
        //    //        }
        //    //        await _examService.RemoveAsync(e.ID);
        //    //    }
        //    //}
        //    //str += (DateTime.UtcNow - start).TotalSeconds;
        //    //start = DateTime.UtcNow;
        //    //str += " Phase 4: ";
        //    //reset progress

        //    //_chapterProgressService.CreateQuery().UpdateMany(t => true, Builders<ChapterProgressEntity>.Update
        //    //    .Set(t => t.TotalPoint, 0)
        //    //    .Set(t => t.PracticePoint, 0)
        //    //    .Set(t => t.PracticeAvgPoint, 0)
        //    //    .Set(t => t.AvgPoint, 0)
        //    //    .Set(t => t.ExamDone, 0)
        //    //    .Set(t => t.PracticeDone, 0)
        //    //    );

        //    //_classSubjectProgressService.CreateQuery().UpdateMany(t => true, Builders<ClassSubjectProgressEntity>.Update
        //    //    .Set(t => t.TotalPoint, 0)
        //    //    .Set(t => t.PracticePoint, 0)
        //    //    .Set(t => t.PracticeAvgPoint, 0)
        //    //    .Set(t => t.AvgPoint, 0)
        //    //    .Set(t => t.ExamDone, 0)
        //    //    .Set(t => t.PracticeDone, 0)
        //    //    );

        //    //_classProgressService.CreateQuery().UpdateMany(t => true, Builders<ClassProgressEntity>.Update
        //    //    .Set(t => t.TotalPoint, 0)
        //    //    .Set(t => t.PracticePoint, 0)
        //    //    .Set(t => t.PracticeAvgPoint, 0)
        //    //    .Set(t => t.AvgPoint, 0)
        //    //    .Set(t => t.ExamDone, 0)
        //    //    .Set(t => t.PracticeDone, 0)
        //    //    );

        //    //_lessonProgressService.CreateQuery().UpdateMany(t => true, Builders<LessonProgressEntity>.Update
        //    //    .Set(t => t.AvgPoint, 0)
        //    //    .Set(t => t.LastPoint, 0)
        //    //    .Set(t => t.LastTry, DateTime.MinValue)
        //    //    .Set(t => t.MaxPoint, 0)
        //    //    .Set(t => t.MinPoint, 0)
        //    //    .Set(t => t.Tried, 0)
        //    //    );

        //    //str += (DateTime.UtcNow - start).TotalSeconds;
        //    //start = DateTime.UtcNow;
        //    //str += " Phase 5: ";

        //    //var lessonProgresses = _lessonProgressService
        //    //    //.CreateQuery().Find(t => t.ClassID == "5f60dd6b0dd2b41448907f26" && t.StudentID == "5f60e2e90dd2b41448909d05")
        //    //    .GetAll()
        //    //    .Project(t => new LessonProgressEntity { ID = t.ID, LessonID = t.LessonID, StudentID = t.StudentID }).ToList();
        //    ////recalculate point
        //    //foreach (var lp in lessonProgresses)
        //    //{
        //    //    var ls = _clonelessonService.GetItemByID(lp.LessonID);
        //    //    if (ls == null)
        //    //    {
        //    //        _lessonProgressService.Remove(lp.ID);
        //    //    }
        //    //    else
        //    //    {
        //    //        var progress = _lessonProgressService.GetItemByID(lp.ID);
        //    //        progress.Multiple = ls.Multiple;
        //    //        _lessonProgressService.Save(progress);

        //    //        var exs = _examService.CreateQuery().Find(t => t.LessonID == lp.LessonID && t.StudentID == lp.StudentID).Project(t => t.ID).SortBy(t => t.Number);
        //    //        foreach (var exid in exs.ToEnumerable())
        //    //        {
        //    //            var ex = _examService.GetItemByID(exid);
        //    //            if (ex != null)
        //    //            {
        //    //                if (ex.Marked) _lessonHelper.CompleteFull(ex, ls, out _, false);
        //    //                else
        //    //                    if (_lessonHelper.IsOvertime(ex))
        //    //                    _lessonHelper.CompleteNoEssay(ex, ls, out _, false);
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //    str += (DateTime.UtcNow - start).TotalSeconds;
        //    start = DateTime.UtcNow;
        //    str += " End. ";
        //    return Json(str);
        //}


        public async Task<JsonResult> FixLessonPoint0()
        {
            var start = DateTime.UtcNow;
            var str = "";
            str += "Phase 0: ";
            var change = 0;

            var clids = _courseLessonService.CreateQuery().Find(t => t.IsPractice && t.Point == 0).Project(t => t.ID).ToList();
            foreach (var clid in clids)
            {
                var cl = _courseLessonService.GetItemByID(clid);
                var oldpoint = cl.Point;
                var isPrac = cl.IsPractice;
                var newPrac = isPrac;
                var point = calculateLessonPoint(cl, newPrac);
                if ((isPrac != newPrac) || (point != oldpoint))
                {
                    _courseLessonService.Save(cl);
                    change++;
                }
            }
            str += " Course Lesson Point change " + change;

            str += " - Phase 2: ";
            var lids = _lessonService.CreateQuery().Find(t => (t.IsPractice || t.TemplateType == LESSON_TEMPLATE.EXAM) && t.Point == 0).Project(t => t.ID).ToList();

            var lchg = 0;
            foreach (var clid in lids)
            {
                var cl = _lessonService.GetItemByID(clid);
                var oldpoint = cl.Point;
                var isPrac = cl.IsPractice;
                var newPrac = isPrac;
                var point = calculateCloneLessonPoint(cl, newPrac);
                if ((isPrac != newPrac) || (point != oldpoint))
                {
                    _lessonService.Save(cl);
                    lchg++;
                    var exams = _examService.CreateQuery().Find(t => t.LessonID == clid).ToList();
                    if (exams != null && exams.Count() > 0)
                    {
                        foreach (var ex in exams)
                        {
                            var lesson = _lessonService.GetItemByID(ex.LessonID);
                            _lessonHelper.CompleteNoEssay(ex, lesson, out point, false);

                            //ex.MaxPoint = point;

                            //_examService.Save(ex);
                        }
                    }
                }
            }

            str += " Lesson Point change " + lchg;

            str += " - Phase 3: ";
            var eids = _lessonProgressService.CreateQuery().Find(t => t.LastPoint > 100).Project(t => t.LessonID).ToList().Distinct();
            var echange = 0;
            foreach (var eid in eids)
            {
                var exs = _examService.CreateQuery().Find(t => eids.Contains(t.LessonID)).ToList();
                foreach (var ex in exs)
                {

                    if (ex.QuestionsTotal < ex.QuestionsPass || ex.MaxPoint < ex.QuestionsTotal)
                    {
                        var lesson = _lessonService.GetItemByID(ex.LessonID);
                        double point = 0;
                        _lessonHelper.CompleteNoEssay(ex, lesson, out point, false);
                        echange++;
                    }
                }
            }

            str += " Exam point change " + echange;

            //foreach (var clid in eids)
            //{
            //    var cl = _lessonService.GetItemByID(clid);
            //    var oldpoint = cl.Point;
            //    var isPrac = cl.IsPractice;
            //    var newPrac = isPrac;
            //    var point = calculateCloneLessonPoint(cl, newPrac);
            //    if ((isPrac != newPrac) || (point != oldpoint))
            //    {
            //        _lessonService.Save(cl);
            //        lchg++;
            //        var exams = _examService.CreateQuery().Find(t => t.LessonID == clid).ToList();
            //        if (exams != null && exams.Count() > 0)
            //        {
            //            foreach (var ex in exams)
            //            {
            //                ex.MaxPoint = point;

            //                _examService.Save(ex);
            //            }
            //        }
            //    }
            //}

            return Json(str);
        }


        public async Task<JsonResult> FixScoreDataV2()
        {
            var start = DateTime.UtcNow;
            var str = "";
            str += "Phase 0: ";
            var change = 0;


            var clids = _courseLessonService.GetAll().Project(t => t.ID).ToList();
            foreach (var clid in clids)
            {
                var cl = _courseLessonService.GetItemByID(clid);
                var oldpoint = cl.Point;
                var isPrac = cl.IsPractice;
                var newPrac = isPrac;
                var point = calculateLessonPoint(cl, newPrac);
                if ((isPrac != newPrac) || (point != oldpoint))
                {
                    _courseLessonService.Save(cl);
                    change++;
                }
            }
            str += " Course Lesson Point change " + change;

            var lids = _clonelessonService.GetAll().Project(t => t.ID).ToList();

            change = 0;
            foreach (var lid in lids)
            {

                var l = _clonelessonService.GetItemByID(lid);
                var oldpoint = l.Point;
                var isPrac = l.IsPractice;
                var newPrac = isPrac;
                var point = calculateCloneLessonPoint(l, newPrac);

                if ((isPrac != newPrac) || (point != oldpoint))
                {
                    _clonelessonService.Save(l);
                    change++;
                }
            }
            str += " - Lesson Point change " + change + " - ";

            //reset progress

            _chapterProgressService.CreateQuery().UpdateMany(t => true, Builders<ChapterProgressEntity>.Update
                .Set(t => t.Completed, 0)
                .Set(t => t.TotalPoint, 0)
                .Set(t => t.PracticePoint, 0)
                .Set(t => t.PracticeAvgPoint, 0)
                .Set(t => t.AvgPoint, 0)
                .Set(t => t.ExamDone, 0)
                .Set(t => t.PracticeDone, 0)
                .Set(t => t.LastDate, new DateTime(1900, 1, 1))
                );

            _classSubjectProgressService.CreateQuery().UpdateMany(t => true, Builders<ClassSubjectProgressEntity>.Update
                .Set(t => t.Completed, 0)
                .Set(t => t.TotalPoint, 0)
                .Set(t => t.PracticePoint, 0)
                .Set(t => t.PracticeAvgPoint, 0)
                .Set(t => t.AvgPoint, 0)
                .Set(t => t.ExamDone, 0)
                .Set(t => t.PracticeDone, 0)
                .Set(t => t.LastDate, new DateTime(1900, 1, 1))
                );

            _classProgressService.CreateQuery().UpdateMany(t => true, Builders<ClassProgressEntity>.Update
                .Set(t => t.Completed, 0)
                .Set(t => t.TotalPoint, 0)
                .Set(t => t.PracticePoint, 0)
                .Set(t => t.PracticeAvgPoint, 0)
                .Set(t => t.AvgPoint, 0)
                .Set(t => t.ExamDone, 0)
                .Set(t => t.PracticeDone, 0)
                .Set(t => t.LastDate, new DateTime(1900, 1, 1))
                );

            _lessonProgressService.CreateQuery().UpdateMany(t => true, Builders<LessonProgressEntity>.Update
                .Set(t => t.AvgPoint, 0)
                .Set(t => t.LastPoint, 0)
                .Set(t => t.LastTry, DateTime.MinValue)
                .Set(t => t.MaxPoint, 0)
                .Set(t => t.MinPoint, 0)
                .Set(t => t.Tried, 0)
                );

            str += (DateTime.UtcNow - start).TotalSeconds;
            start = DateTime.UtcNow;
            str += " Phase 5: ";

            //reapply exam maxpoint
            var exams = _examService.GetAll().ToEnumerable();

            var examPointChange = 0;
            foreach (var e in exams)
            {
                var lesson = _clonelessonService.GetItemByID(e.LessonID);
                if (lesson != null)
                {
                    if (e.MaxPoint == 0 || e.MaxPoint != lesson.Point)
                    {
                        e.MaxPoint = lesson.Point;
                        _examService.Save(e);
                        examPointChange++;
                    }
                }
                else
                {
                    var parts = _clonelessonPartService.GetByLessonID(e.LessonID);//remove orphan parts
                    foreach (var part in parts)
                    {
                        await _examDetailService.Collection.DeleteManyAsync(t => t.LessonPartID == part.ID);
                        var quizs = _clonequestionService.GetByPartID(part.ID);
                        foreach (var quiz in quizs)
                        {
                            await _cloneanswerService.Collection.DeleteManyAsync(t => t.ParentID == quiz.ID);
                            await _clonequestionService.RemoveAsync(quiz.ID);
                        }
                        await _clonelessonPartService.RemoveAsync(part.ID);
                    }
                    await _examService.RemoveAsync(e.ID);
                }
            }

            str += " - ExamPoint Change :" + examPointChange + " - ";

            var lessonProgresses = _lessonProgressService
                //.CreateQuery().Find(t => t.ClassID == "5f649e8ed533d51c9013e9c1" 
                //&& t.StudentID == "5f60e2e90dd2b41448909d05"
                //)
                .GetAll()
                //.Project(t => new LessonProgressEntity { ID = t.ID, LessonID = t.LessonID, StudentID = t.StudentID })
                .ToList();
            lessonProgresses = lessonProgresses//OrderBy(t => t.LessonID).ThenBy(t => t.Tried)
                .ToList();

            var lessonPointChange = 0;

            foreach (var lprg in lessonProgresses)
            {
                if (lprg.ChapterID != "0")
                    await _progressHelper.UpdateChapterLastLearn(lprg, true);
                else
                    await _progressHelper.UpdateClassSubjectLastLearn(new ClassSubjectProgressEntity { LastLessonID = lprg.LessonID, ClassSubjectID = lprg.ClassSubjectID, ClassID = lprg.ClassID, LastDate = lprg.LastDate }, true);

                var lesson = _clonelessonService.GetItemByID(lprg.LessonID);
                if (lesson.IsPractice)
                {
                    var lastestEx = _examService.GetLastestByLessonAndStudent(lprg.LessonID, lprg.StudentID);
                    if (lastestEx != null && lastestEx.MaxPoint > 0)
                    {
                        var lpoint = lastestEx.Point * 100 / lastestEx.MaxPoint;
                        if (lpoint != lprg.LastPoint)
                        {
                            lprg.LastPoint = lpoint;
                            lessonPointChange++;
                        }
                        lprg.Tried = lastestEx.Number;
                        lprg.PointChange = lprg.LastPoint;
                        lprg.LastTry = lastestEx.Created;
                        _lessonProgressService.Save(lprg);
                    }

                    if (lesson.ChapterID != "0")
                        //Task.Run(() =>
                        //{
                        await _progressHelper.UpdateParentChapPoint(lesson.ChapterID, lprg.StudentID, 0, 0, lprg.LastPoint * lesson.Multiple, (long)lesson.Multiple);
                    //});
                    else
                        //Task.Run(() =>
                        //{

                        await _progressHelper.UpdateClassSubjectPoint(lesson.ClassSubjectID, lprg.StudentID, 0, 0, lprg.LastPoint * lesson.Multiple, (long)lesson.Multiple);
                    //});
                }
                else if (lesson.TemplateType == LESSON_TEMPLATE.EXAM)
                {
                    if (lesson.ChapterID == "0")
                        //Task.Run(() =>
                        //{
                        await _progressHelper.UpdateClassSubjectPoint(lesson.ClassSubjectID, lprg.StudentID, lprg.LastPoint * lesson.Multiple, (long)lesson.Multiple, 0, 0);
                    //});
                    else
                        //Task.Run(() =>
                        //{
                        await _progressHelper.UpdateParentChapPoint(lesson.ChapterID, lprg.StudentID, lprg.LastPoint * lesson.Multiple, (long)lesson.Multiple, 0, 0);
                    //});
                }
            }

            str += " - LessonPoint Change :" + lessonPointChange + " - ";

            str += (DateTime.UtcNow - start).TotalSeconds;
            start = DateTime.UtcNow;
            str += " End. ";
            return Json(str);
        }

        public async Task<JsonResult> FixExam()
        {
            _courseLessonService.CreateQuery().UpdateMany(t => t.TemplateType == LESSON_TEMPLATE.EXAM,
                Builders<CourseLessonEntity>.Update.Set(t => t.IsPractice, true).Set(t => t.TemplateType, LESSON_TEMPLATE.LECTURE)
                );

            _clonelessonService.CreateQuery().UpdateMany(t => t.TemplateType == LESSON_TEMPLATE.EXAM,
                Builders<LessonEntity>.Update.Set(t => t.IsPractice, true).Set(t => t.TemplateType, LESSON_TEMPLATE.LECTURE)
                );

            var examSubj = _classSubjectService.CreateQuery().Find(t => t.TypeClass == CLASSSUBJECT_TYPE.EXAM).ToList();
            if (examSubj != null)
            {
                foreach (var sbj in examSubj)
                {
                    _clonelessonService.CreateQuery().UpdateMany(t => t.ClassSubjectID == sbj.ID,
                    Builders<LessonEntity>.Update.Set(t => t.IsPractice, false).Set(t => t.TemplateType, LESSON_TEMPLATE.EXAM)
                    );
                }
            }

            //_courseChapterService.CreateQuery().UpdateMany(t => t.ClassSubjectID == sbj.ID,
            //        Builders<LessonEntity>.Update.Set(t => t.IsPractice, false).Set(t => t.TemplateType, LESSON_TEMPLATE.EXAM)
            //        );




            //var courseChapters = _courseChapterService.CreateQuery()
            //    .Find(t => t.TotalExams > 0).ToList();
            //foreach (var chapter in courseChapters)
            //{
            //    if (chapter.TotalExams > 0)
            //    {
            //        var exCount = chapter.TotalExams;
            //        //await _courseHelper.IncreaseCourseChapterCounter(chapter.ID, 0, 0 - chapter.TotalExams, chapter.TotalExams);
            //        chapter.TotalPractices += exCount;
            //        chapter.TotalExams = 0;
            //        _courseChapterService.Save(chapter);
            //        if (chapter.ParentID == "0")
            //            await _courseHelper.IncreaseCourseCounter(chapter.CourseID, 0, 0 - exCount, exCount);
            //    }
            //}

            //var cloneChapters = _chapterService.CreateQuery()
            //    .Find(t => t.TotalExams > 0).ToEnumerable();
            //foreach (var chapter in cloneChapters)
            //{
            //    if (chapter.TotalExams > 0)
            //    {
            //        var exCount = chapter.TotalExams;
            //        //await _classHelper.IncreaseChapterCounter(chapter.ID, 0, 0 - chapter.TotalExams, chapter.TotalExams);
            //        chapter.TotalPractices += exCount;
            //        chapter.TotalExams = 0;
            //        _courseChapterService.Save(chapter);
            //        if (chapter.ParentID == "0")
            //            await _classHelper.IncreaseClassSubjectCounter(chapter.ClassSubjectID, 0, 0 - exCount, exCount);
            //    }
            //}

            //var chapPrgs = _chapterProgressService.CreateQuery().Find(t => t.ExamDone > 0).ToEnumerable();

            //foreach (var prg in chapPrgs)
            //{
            //    if (prg.ExamDone > 0)
            //    {
            //        var point = prg.TotalPoint;
            //        var count = prg.ExamDone;
            //        prg.PracticeDone += count;
            //        prg.PracticePoint += point;
            //        prg.PracticeAvgPoint = prg.PracticeDone > 0 ? (prg.PracticePoint * 100 / prg.PracticeDone) : 0;

            //        _chapterProgressService.Save(prg);

            //        var chapter = _chapterService.GetItemByID(prg.ChapterID);
            //        if (chapter.ParentID == "0")
            //        {
            //            await _progressHelper.UpdateClassSubjectPoint(chapter.ClassSubjectID, prg.StudentID, 0 - point, 0 - count, point, count);
            //        }
            //    }
            //}
            return Json("OK");
        }

        public async Task<JsonResult> CreateExamSubject()
        {
            var classes = _classService.CreateQuery().Find(t => t.ClassMechanism != CLASS_MECHANISM.PERSONAL).ToList();
            foreach (var @currentClass in classes)
            {
                var subjects = _classSubjectService.CreateQuery().Find(t => t.ClassID == currentClass.ID && t.TypeClass == CLASSSUBJECT_TYPE.EXAM).ToList();
                if (subjects == null || subjects.Count == 0) //create exam Subject
                {
                    var newSbj = new ClassSubjectEntity
                    {
                        ClassID = currentClass.ID,
                        CourseName = "Bài kiểm tra",
                        Description = "Bài kiểm tra",
                        StartDate = currentClass.StartDate,
                        EndDate = currentClass.EndDate,
                        TypeClass = CLASSSUBJECT_TYPE.EXAM,
                        TeacherID = currentClass.TeacherID
                    };
                    _classSubjectService.Save(newSbj);
                }
            }
            return Json("OK");

        }

        //public async Task<JsonResult> FixUTCSchedule(string classList)
        //{
        //    var arr = classList.Split(',');
        //    var classsubjs = new List<ClassSubjectEntity>();
        //    if (arr.Length > 0)
        //        foreach (var clid in arr)
        //        {
        //            classsubjs.AddRange(_classSubjectService.GetByClassID(clid));
        //        }

        //    foreach (var sbj in classsubjs)
        //    {
        //        var rootchaps = _chapterService.GetSubChapters(sbj.ID, "0");
        //        foreach (var chap in rootchaps)
        //        {
        //            if (chap.StartDate > new DateTime(1900, 1, 1))
        //                UpdateChapterCalendar(chap, "5e43fedd4a77b123fc297e90");//Mr Lam
        //        }
        //    }

        //    return Json("OK");

        //}

        //private void UpdateChapterCalendar(ChapterEntity entity, string UserID)
        //{
        //    var lessonids = _clonelessonService.CreateQuery().Find(t => t.ChapterID == entity.ID && t.ClassSubjectID == entity.ClassSubjectID).Project(t => t.ID).ToList();
        //    foreach (var id in lessonids)
        //    {
        //        var schedule = _lessonScheduleService.GetItemByLessonID(id);
        //        schedule.StartDate = entity.StartDate;
        //        schedule.EndDate = entity.EndDate;
        //        UpdateCalendar(schedule, UserID);
        //        _lessonScheduleService.CreateOrUpdate(schedule);
        //    }
        //    var subchaps = _chapterService.GetSubChapters(entity.ClassSubjectID, entity.ID);
        //    foreach (var subchap in subchaps)
        //    {
        //        subchap.StartDate = entity.StartDate;
        //        subchap.EndDate = entity.EndDate;
        //        _chapterService.Save(subchap);
        //        UpdateChapterCalendar(subchap, UserID);
        //    }
        //}

        //private void UpdateCalendar(LessonScheduleEntity entity, string userid)
        //{
        //    //var oldcalendar = _calendarHelper.GetByScheduleId(entity.ID);
        //    //if (oldcalendar != null)
        //    _calendarHelper.RemoveSchedule(entity.ID);
        //    if (entity.IsActive)
        //        _calendarHelper.ConvertCalendarFromSchedule(entity, userid);
        //}

        //public async Task<JsonResult> Remark(string ExamID)//Big fix
        //{
        //    var exam = _examService.GetItemByID(ExamID);
        //    var lesson = _clonelessonService.GetItemByID(exam.LessonID);
        //    _lessonHelper.CompleteNoEssay(exam, lesson, out _, false);
        //    return Json("OK");
        //}

        private double calculateLessonPoint(CourseLessonEntity lesson, bool isPrac)
        {
            isPrac = false;
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
                        _questionService.Collection.UpdateMany(t => t.ParentID == part.ID, Builders<LessonPartQuestionEntity>.Update.Set(t => t.Point, part.Point));
                    }
                    else
                    {
                        point += _questionService.GetByPartID(part.ID).Count();//trắc nghiệm => điểm = số câu hỏi (mỗi câu 1đ)
                        _questionService.Collection.UpdateMany(t => t.ParentID == part.ID, Builders<LessonPartQuestionEntity>.Update.Set(t => t.Point, 1));
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
            _courseLessonService.Save(lesson);
            isPrac = lesson.IsPractice;
            return point;
        }

        private double calculateCloneLessonPoint(LessonEntity lesson, bool isPrac)
        {
            isPrac = false;
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
                        _clonequestionService.Collection.UpdateMany(t => t.ParentID == part.ID, Builders<CloneLessonPartQuestionEntity>.Update.Set(t => t.Point, part.Point));
                    }
                    else
                    {
                        point += _clonequestionService.GetByPartID(part.ID).Count();//trắc nghiệm => điểm = số câu hỏi (mỗi câu 1đ)
                        _clonequestionService.Collection.UpdateMany(t => t.ParentID == part.ID, Builders<CloneLessonPartQuestionEntity>.Update.Set(t => t.Point, 1));
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
            isPrac = lesson.IsPractice;
            return point;
        }

        public JsonResult FixFillquiz()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var partIDs = _lessonPartService.CreateQuery().Find(t => t.Type == "QUIZ2").Project(t => t.ID).ToList();
            foreach (var pid in partIDs)
            {
                var qids = _questionService.CreateQuery().Find(t => t.ParentID == pid).Project(t => t.ID).ToList();
                if (qids != null && qids.Count() > 0)
                {
                    foreach (var qid in qids)
                    {
                        var ans = _answerService.CreateQuery().Find(t => t.ParentID == qid).ToList();
                        if (ans != null && ans.Count() > 0)
                        {
                            foreach (var answer in ans)
                            {
                                answer.Content = validateFill(answer.Content);
                                _answerService.Save(answer);
                            }
                        }
                    }
                }
            }

            var partIDs2 = _clonelessonPartService.CreateQuery().Find(t => t.Type == "QUIZ2").Project(t => t.ID).ToList();
            foreach (var pid in partIDs2)
            {
                var qids = _clonequestionService.CreateQuery().Find(t => t.ParentID == pid).Project(t => t.ID).ToList();
                if (qids != null && qids.Count() > 0)
                {
                    foreach (var qid in qids)
                    {
                        var ans = _cloneanswerService.CreateQuery().Find(t => t.ParentID == qid).ToList();
                        if (ans != null && ans.Count() > 0)
                        {
                            foreach (var answer in ans)
                            {
                                answer.Content = validateFill(answer.Content);
                                _cloneanswerService.Save(answer);
                            }
                        }
                    }
                }
            }
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            return Json($"OK - {elapsedTime}");
        }

        private string validateFill(string org)
        {
            if (string.IsNullOrEmpty(org)) return org;
            org = org.Trim();
            while (org.IndexOf("  ") >= 0)
                org = org.Replace("  ", "");

            return StringHelper.ReplaceSpecialCharacters(org);
        }

        public JsonResult ChangeCenter(string _ClassID, string oldCenter, string newCenter)
        {
            try
            {
                var classIds = _classService.CreateQuery()
                    .Find(t =>
                    t.Subjects.Contains("5e4de8168a6e7b13bca5251c") ||
                    t.Subjects.Contains("5e4df00e8a6e7b13bca52576") ||
                    t.Subjects.Contains("5e4df0388a6e7b13bca52578") ||
                    t.Subjects.Contains("5e4df0268a6e7b13bca52577")
                    ).Project(t => t.ID).ToList();

                var _oldCenter = _centerService.GetItemByCode("eduso");
                var _newCenter = _centerService.GetItemByCode("benh-vien-viet-duc");

                foreach (var ClassID in classIds)
                {


                    if (_oldCenter == null)
                    {
                        return Json("Không tìm thấy cơ sở");
                    }
                    if (_newCenter == null)
                    {
                        return Json("Không tìm thấy cơ sở mới");
                    }

                    var _class = _classService.GetItemByID(ClassID);
                    if (_class == null)
                    {
                        return Json("Không tìm thấy lớp");
                    }

                    else
                    {
                        var _mappingClass = new Core_v2.Globals.MappingEntity<ClassEntity, ClassEntity>();
                        var _mappingClassSub = new Core_v2.Globals.MappingEntity<ClassSubjectEntity, ClassSubjectEntity>();
                        var listTearch = _class.Members;
                        var listNewClsbjID = new List<string>();


                        _class.Center = _newCenter.ID;

                        _classService.Save(_class);//copy lớp

                        //var a = _classProgressService.GetByClassID(_class.ID);
                        //a.ClassID = newClass.ID;
                        //_classProgressService.Save(a);

                        var lstStudent = _studentService.GetStudentsByClassId(_class.ID);
                        foreach (var item in lstStudent)  //copy hoc vien
                        {
                            //item.JoinedClasses.Remove(_class.ID);
                            //item.JoinedClasses.Add(newClass.ID);

                            var has_oldcenter_class = _classService.GetItemsByIDs(item.JoinedClasses).Any(t => t.Center == oldCenter);

                            if (!has_oldcenter_class)
                            {
                                item.Centers.Remove(_oldCenter.ID);
                            }
                            if (!item.Centers.Contains(_newCenter.ID))
                                item.Centers.Add(_newCenter.ID);
                            _studentService.Save(item);



                            //if (item.Centers.Count == 1 && item.Centers.Contains(_oldCenter.ID))
                            //{
                            //    item.Centers.Remove(_oldCenter.ID);
                            //    item.Centers.Add(_newCenter.ID);
                            //}
                            //else if (item.Centers.Count > 1 && item.Centers.Contains(_oldCenter.ID))
                            //{
                            //    item.Centers.Remove(_oldCenter.ID);
                            //    item.Centers.Add(_newCenter.ID);
                            //}
                            //else continue;
                        }

                        foreach (var t in listTearch)
                        {
                            var teacher = _teacherService.GetItemByID(t.TeacherID);
                            var has_oldcenter_class = _classService.CreateQuery().Count(c => c.Center == _oldCenter.ID && c.Members.Any(m => m.TeacherID == teacher.ID)) > 0;
                            var oldcenter_role = teacher.Centers.SingleOrDefault(c => c.CenterID == _oldCenter.ID);

                            if (!has_oldcenter_class)
                            {
                                teacher.Centers.RemoveAll(c => c.CenterID == _oldCenter.ID);
                            }

                            if (!teacher.Centers.Any(c => c.CenterID == _newCenter.ID))
                                teacher.Centers.Add(new CenterMemberEntity
                                {
                                    CenterID = _newCenter.ID,
                                    RoleID = oldcenter_role.RoleID,
                                    Code = _newCenter.Code,
                                    Name = _newCenter.Name
                                });


                            _teacherService.Save(teacher);
                        }

                    }
                }

                var courses = _courseService.CreateQuery().Find(t =>
                    t.SubjectID == "5e4de8168a6e7b13bca5251c" ||
                    t.SubjectID == "5e4df00e8a6e7b13bca52576" ||
                    t.SubjectID == "5e4df0388a6e7b13bca52578" ||
                    t.SubjectID == "5e4df0268a6e7b13bca52577"
                    ).ToEnumerable();
                foreach (var course in courses)
                {
                    course.Center = _newCenter.ID;
                    _courseService.Save(course);
                }

                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public JsonResult DeleteCenter(string centerID = "5fbcb0bcb7df2d22889f2c77")
        {
            //foreach (var teacher in _teacherService.CreateQuery().Find(t => t.Centers.Any(ct => ct.CenterID == centerID)).ToList())
            //{
            //    teacher.Centers.RemoveAll(t => t.CenterID == centerID);
            //    _teacherService.Save(teacher);
            //}

            _teacherService.CreateQuery().UpdateMany(t => t.Centers.Any(ct => ct.CenterID == centerID)
            , Builders<TeacherEntity>.Update.PullFilter<CenterMemberEntity>(cm => cm.Centers, Builders<CenterMemberEntity>.Filter.Where(c => c.CenterID == centerID)));

            _studentService.CreateQuery().UpdateMany(t => t.Centers.Contains(centerID), Builders<StudentEntity>.Update.Pull<string>(ct => ct.Centers, centerID));

            var classes = _classService.CreateQuery().Find(t => t.Center == centerID).ToList();
            if (classes != null && classes.Count > 0)
            {
                var ids = classes.Select(t => t.ID).ToArray();
                if (ids.Length > 0)
                {
                    //Remove Class Student
                    _ = _studentService.LeaveClassAll(ids.ToList());
                    //remove ClassSubject
                    _ = _classSubjectService.RemoveManyClass(ids);
                    //remove Lesson, Part, Question, Answer
                    _ = _lessonHelper.RemoveManyClassLessons(ids);
                    //remove Schedule
                    //_ = _lessonScheduleService.RemoveManyClass(ids);
                    //remove History
                    _ = _progressHelper.RemoveClassHistory(ids);
                    //remove Exam
                    _ = _examService.RemoveManyClassExam(ids);
                    //.Collection.DeleteMany(o => ids.Contains(o.ClassID));
                    //remove Exam Detail
                    _examDetailService.Collection.DeleteMany(o => ids.Contains(o.ClassID));
                    var delete = _classService.Collection.DeleteMany(o => ids.Contains(o.ID));
                }
            }

            var courses = _courseService.CreateQuery().Find(t => t.Center == centerID).ToList();
            if (classes != null && classes.Count > 0)
            {
                var ids = classes.Select(t => t.ID).ToArray();
                if (ids.Length > 0)
                {
                    _chapterService.CreateQuery().DeleteMany(o => ids.Contains(o.CourseID));
                    _courseLessonService.CreateQuery().DeleteMany(o => ids.Contains(o.CourseID));
                    _lessonPartService.CreateQuery().DeleteMany(o => ids.Contains(o.CourseID));
                    _questionService.CreateQuery().DeleteMany(o => ids.Contains(o.CourseID));
                    _answerService.CreateQuery().DeleteMany(o => ids.Contains(o.CourseID));
                }
            }

            return Json("OK");
        }

        //public JsonResult ShareStudent()
        //{
        //    var courses = _courseService.CreateQuery().Find(t => t.IsActive && t.PublicWStudent).ToList();
        //    var count = 0;
        //    foreach (var course in courses)
        //    {
        //        if (course.StudentTargetCenters == null)
        //        {
        //            course.StudentTargetCenters = course.TargetCenters;
        //            course.PublicWStudent = false;
        //            _courseService.Save(course);
        //            count++;
        //        }
        //    }
        //    return Json("OK:" + count);
        //}

        public JsonResult ChangeLinkImage()
        {
            try
            {
                var folder = "eduso/IMG";
                folder += ("/" + DateTime.UtcNow.ToString("yyyyMMdd"));
                string uploads = Path.Combine(RootPath + "/Files", folder);
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                //var i = 1;
                var listImgDriver = _referenceService.CreateQuery().Find(x => x.Image != null && x.Image.Contains("drive.google.com")).ToList();
                foreach (var item in listImgDriver)
                {
                    var path = item.Image.Replace("view", "download");
                    var fileName = path.Substring(path.IndexOf("id=") + 3);
                    using (WebClient myWebClient = new WebClient())
                    {
                        myWebClient.DownloadFile(path, $"{uploads}/{fileName}.jpg");
                        //i++;
                    }

                    item.Image = $"/Files//{folder}/{fileName}.jpg";
                    _referenceService.Save(item);
                }
                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        //public JsonResult FixCalendar()
        //{
        //    var calendars = _calendarService.GetAll().ToEnumerable();
        //    var count = 0;
        //    foreach (var calendar in calendars)
        //    {
        //        var sch = _lessonScheduleService.GetItemByID(calendar.ScheduleID);
        //        if (sch == null)
        //        {
        //            _calendarService.Remove(calendar.ID);
        //            count++;
        //        }
        //    }
        //    return Json("DEL " + count);
        //}

        public JsonResult FixLastUpdate()
        {
            try
            {
                var _learnHistories = _learningHistoryService.CreateQuery().Find(x => x.Time >= new DateTime(2020, 10, 19).ToUniversalTime());
                var learnHistories = (from lh in _learnHistories.ToList()
                                      group lh by new
                                      {
                                          lh.StudentID,
                                          lh.ClassID,
                                          lh.ClassSubjectID
                                      }
                                   into g
                                      select new
                                      {
                                          StudentID = g.Key.StudentID,
                                          ClassID = g.Key.ClassID,
                                          ClassSubjectID = g.Key.ClassSubjectID,
                                          LastTime = g.ToList().OrderByDescending(x => x.Time).FirstOrDefault().Time,
                                          LastLessonID = g.ToList().OrderByDescending(x => x.Time).FirstOrDefault().LessonID
                                      }).ToList();

                string a = "";

                foreach (var lh in learnHistories)
                {
                    //if(lh.StudentID== "5f7e8382f197721750deb12c")
                    {
                        UpdateClassSubjectLastLearn(new ClassSubjectProgressEntity { LastLessonID = lh.LastLessonID, ClassSubjectID = lh.ClassSubjectID, ClassID = lh.ClassID, StudentID = lh.StudentID, LastDate = lh.LastTime });
                        a += _studentService.GetItemByID(lh.StudentID).FullName.ToUpper() + " lớp " + _classService.GetItemByID(lh.ClassID).Name + "; ";
                    }
                }

                return Json($"OK - {a}");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        private async Task UpdateClassSubjectLastLearn(ClassSubjectProgressEntity item)
        {
            var updated = await _classSubjectProgressService.Collection.UpdateManyAsync(t => t.StudentID == item.StudentID && t.ClassSubjectID == item.ClassSubjectID,
                     new UpdateDefinitionBuilder<ClassSubjectProgressEntity>()
                     .Set(t => t.LastDate, item.LastDate)
                     .Set(t => t.LastLessonID, item.LastLessonID)
                     );

            if (updated.ModifiedCount == 0) // no match found => check & create progress
            {
                var currentProgress = _classSubjectProgressService.GetItemByClassSubjectID(item.ClassSubjectID, item.StudentID);
                if (currentProgress == null)
                {
                    var currentSbj = _classSubjectService.GetItemByID(item.ClassSubjectID);
                    if (currentSbj == null)
                        return;
                    currentProgress = new ClassSubjectProgressEntity
                    {
                        ClassID = item.ClassID,
                        ClassSubjectID = item.ClassSubjectID,
                        StudentID = item.StudentID,
                        LastLessonID = item.LastLessonID,
                        LastDate = item.LastDate,
                    };
                    await _classSubjectProgressService.Collection.InsertOneAsync(currentProgress);
                }
            }

            await UpdateClassLastLearn(new ClassProgressEntity { ClassID = item.ClassID, StudentID = item.StudentID, LastDate = item.LastDate });
        }

        private async Task UpdateClassLastLearn(ClassProgressEntity item)
        {

            var updated = await _classProgressService.Collection.UpdateManyAsync(t => t.StudentID == item.StudentID && t.ClassID == item.ClassID,
                    new UpdateDefinitionBuilder<ClassProgressEntity>()
                    .Set(t => t.LastDate, item.LastDate)
                    .Set(t => t.LastLessonID, item.LastLessonID)
                    );

            if (updated.ModifiedCount == 0) // no match found => check & create progress
            {
                var currentProgress = _classProgressService.GetItemByClassID(item.ClassID, item.StudentID);
                if (currentProgress == null)
                {
                    var currentClass = _classService.GetItemByID(item.ClassID);
                    if (currentClass == null)
                        return;
                    currentProgress = new ClassProgressEntity
                    {
                        ClassID = item.ClassID,
                        StudentID = item.StudentID,
                        LastLessonID = item.LastLessonID,
                        LastDate = item.LastDate,
                    };
                    await _classProgressService.Collection.InsertOneAsync(currentProgress);
                }
            }
        }

        public JsonResult UpdateCourseInfo()
        {
            //var listClassSub = from lcs in _classSubjectService.GetAll().ToEnumerable();
            //let CourseName=_courseService.GetItemByID(lcs.CourseID)?.Name
            //select new
            //{
            //    ID = lcs.ID,
            //    CourseID
            //}
            try
            {
                var listClassSub = _classSubjectService.GetAll().ToEnumerable();

                foreach (var item in listClassSub)
                {
                    var course = _courseService.GetItemByID(item.CourseID);
                    item.CourseName = course == null ? "" : course.Name;
                    item.Image = course == null ? "" : course.Image;
                    _classSubjectService.Save(item);
                }
                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public JsonResult MergeCategory()
        {
            var removeSubject = new List<string>
            {
                "5d8c86ada2dba730482f585b",//KET
                "5d8c86b4a2dba730482f585c",//PET
                "5d8c8722a2dba730482f585d",//FCE
                "5db91a62ab30e73154d7ed4a",//Breakthroug
                "5e0179aeea907613dcf2d09d",//A2 KEY
                "5e0179c3ea907613dcf2d09e",//B1 Plim...
                "5e0179dbea907613dcf2d09f",//B2 First
                "5e0179f4ea907613dcf2d0a0",//C1 Advance
                "5e017a14ea907613dcf2d0a1",//C2 Pro
                "5f92f7de86bd390e5493913b",//Tiếng Anh UTC
            };
            var keepSubject = new List<string>
            {
                "5fc0abdd107ea728b4823937",//Cambridge
                "5fc0abd1107ea728b4823935",//Giao tiếp
                "5d8186c9a8e01e0f3c96d9b3",//IELTS
                "5e68587c53f85c27582d1829",//Tiểu học
                "5ec881d3a7cf090694a65517",//TOEIC
                "5edeed982bbb7e0da4568165",//THCS
                "5ef2cb54202f7b26544032e4",//THPT
                "5f8412be72946a2684e3d87a",//Chương trình chung
                "5fc0abfd107ea728b4823945",//Khác
                "5fc0c7a8107ea728b48244a8",//TOELF
                "5fc0c993107ea728b4824505",//Ngữ pháp
                "5fc0c99d107ea728b4824508",//Từ vựng
            };
            var originSubject = new List<string>
            {
                "5d8c86ada2dba730482f585b",//KET
                "5d8c86b4a2dba730482f585c",//PET
                "5d8c8722a2dba730482f585d",//FCE
                "5db91a62ab30e73154d7ed4a",//Breakthroug
                "5e0179aeea907613dcf2d09d",//A2 KEY
                "5e0179c3ea907613dcf2d09e",//B1 Plim...
                "5e0179dbea907613dcf2d09f",//B2 First
                "5e0179f4ea907613dcf2d0a0",//C1 Advance
                //"5e017a14ea907613dcf2d0a1",//C2 Pro
                "5f92f7de86bd390e5493913b",//Tiếng Anh UTC
            };
            var targetSubject = new List<string>
            {
                "5fc0abdd107ea728b4823937",//Cambridge
                "5fc0abdd107ea728b4823937",//Cambridge
                "5fc0abdd107ea728b4823937",//Cambridge
                "5fc0abd1107ea728b4823935",//Giao tiếp
                "5fc0abdd107ea728b4823937",//Cambridge
                "5fc0abdd107ea728b4823937",//Cambridge
                "5fc0abdd107ea728b4823937",//Cambridge
                "5fc0abdd107ea728b4823937",//Cambridge
                //"5fc0abdd107ea728b4823937",//Cambridge
                "5fc0abdd107ea728b4823937",//Cambridge
            };
            var originGrade = new List<string>
            {
                "5d838ad4d5d1bf27e4410c0d",//KET A2
                "5d838ad9d5d1bf27e4410c0e",//PET B1
                "5d8c873ca2dba730482f585e",//FCE B2

                "5db91a79ab30e73154d7ed4b",//Breakthroug A1
                "5db91a80ab30e73154d7ed4c",//Breakthroug A2
                "5db91a85ab30e73154d7ed4d",//Breakthroug B1
                "5db91a8aab30e73154d7ed4e",//Breakthroug B2
                "5db91a90ab30e73154d7ed4f",//Breakthroug C1

                "5e017a63ea907613dcf2d0a2",//A2 KEY A2
                "5e017a9eea907613dcf2d0a3",//B1 Pli... B1
                "5e017bf9ea907613dcf2d0a4",//B2 First B2
                "5e017c01ea907613dcf2d0a5",//C1 Advance C1
                //"5e017c01ea907613dcf2d0a5",//C2 Pro C2

                "5fad07a90dfb8f18d0e50f54",//Tiếng Anh UTC A1
                "5fad07b20dfb8f18d0e50f65",//Tiếng Anh UTC A2
                "5fad07bb0dfb8f18d0e50f66",//Tiếng Anh UTC B1
            };
            var targetGrade = new List<string>
            {
                "5fc0ad20107ea728b4823990",//Cambridge A2
                "5fc0ad2d107ea728b482399b",//Cambridge B1
                "5fc0ad38107ea728b48239a3",//Cambridge B2
                
                "5fc0c602107ea728b48243e9",//Giao tiếp A1
                "5fc0c609107ea728b48243ed",//Giao tiếp A2
                "5fc0c60f107ea728b48243f6",//Giao tiếp B1
                "5fc0c615107ea728b48243f9",//Giao tiếp B2
                "5fc0c61b107ea728b48243ff",//Giao tiếp C1

                "5fc0ad20107ea728b4823990",//Cambridge A2
                "5fc0ad2d107ea728b482399b",//Cambridge B1
                "5fc0ad38107ea728b48239a3",//Cambridge B2
                "5fc0abdd107ea728b4823937",//Cambridge C1

                "5fc0ad0f107ea728b482398a",//Cambridge A1
                "5fc0ad20107ea728b4823990",//Cambridge A2
                "5fc0ad2d107ea728b482399b",//Cambridge B1

                //Cambridge
                "5fc0ad0f107ea728b482398a",//A1
                "5fc0ad20107ea728b4823990",//A2
                "5fc0ad2d107ea728b482399b",//B1
                "5fc0ad38107ea728b48239a3",//B2
                "5fc0abdd107ea728b4823937",//C1
                //Giao tiếp
                "5fc0c602107ea728b48243e9",//A1
                "5fc0c609107ea728b48243ed",//A2
                "5fc0c60f107ea728b48243f6",//B1
                "5fc0c615107ea728b48243f9",//B2
                "5fc0c61b107ea728b48243ff",//C1
            };
            var msg = "";
            //Merge curriculum
            var centers = _centerService.GetAll().ToList();
            foreach (var center in centers)
            {
                msg += "Update Center " + center.Name + ": ";
                //s update curriculum
                var courses = _courseService.CreateQuery()
                    .Find(t => t.Center == center.ID)
                    .ToList();

                int courseUpdated = 0;
                foreach (var course in courses)
                {
                    //switch (course.ID)
                    //{
                    //    //case "5f56e9857a24eb2270ae27c3"://PreIELTS
                    //    //    course.SubjectID = "5fc0abdd107ea728b4823937";//Cambridge
                    //    //    course.GradeID = "5fc0ad2d107ea728b482399b";//B1
                    //    //    break;
                    //    //case "5f2cdbd77f6b89011ce1d98c"://30 chủ đề từ vựng hay - Trang Anh
                    //    //    course.SubjectID = "5fc0c99d107ea728b4824508";//Từ vựng
                    //    //    course.GradeID = "5fc0c9b5107ea728b482450c";//Chung
                    //    //    break;
                    //    default:
                    if (course.Name.ToLower().Contains("preielts"))
                    {
                        if (course.SubjectID != "5fc0abdd107ea728b4823937")
                        {
                            course.SubjectID = "5fc0abdd107ea728b4823937";//Cambridge
                            course.GradeID = "5fc0ad2d107ea728b482399b";//B1
                            _courseService.Save(course);
                            msg += ((course.Name) + ";");
                            courseUpdated++;
                        }
                    }
                    else if (course.Name.ToLower().Contains("30 chủ đề từ vựng hay"))
                    {
                        if (course.SubjectID != "5fc0c99d107ea728b4824508")
                        {
                            course.SubjectID = "5fc0c99d107ea728b4824508";//Từ vựng
                            course.GradeID = "5fc0c9b5107ea728b482450c";//Chung
                            _courseService.Save(course);
                            msg += ((course.Name) + ";");
                            courseUpdated++;
                        }
                    }
                    else if (course.Name.ToLower().Contains("toán") ||
                        course.Name.ToLower().Contains("địa lý") ||
                        course.Name.ToLower().Contains("lịch sử") ||
                        course.Name.ToLower().Contains("tiếng việt") ||
                        course.Name.ToLower().Contains("ngữ văn") ||
                        course.Name.ToLower().Contains("sinh học") ||
                        course.Name.ToLower().Contains("vật lý") ||
                        course.Name.ToLower().Contains("hóa học") ||
                        course.Name.ToLower().Contains("hsg lý") ||
                        course.Name.ToLower().Contains("gdcd")
                        )
                    {
                        if (course.SubjectID != "5fc0abfd107ea728b4823945")
                        {
                            course.SubjectID = "5fc0abfd107ea728b4823945";//Khác
                            course.GradeID = "5fc0ae47107ea728b48239e6";//Chung
                            _courseService.Save(course);
                            msg += ((course.Name) + ";");
                            courseUpdated++;
                        }
                    }
                    else
                    {
                        var idx = originSubject.IndexOf(course.SubjectID);
                        if (idx >= 0)
                        {
                            //update course
                            course.SubjectID = targetSubject[idx];
                            var idxGrd = originGrade.IndexOf(course.GradeID);
                            if (idxGrd >= 0)
                            {
                                //update course
                                course.GradeID = targetGrade[idxGrd];
                                _courseService.Save(course);
                                msg += ((course.Name) + ";");
                                courseUpdated++;
                            }
                        }
                    }
                    //        break;
                    //}
                }
                //update class
                var classes = _classService.CreateQuery().Find(t => t.Center == center.ID).ToList();
                var csbjUpdated = 0;
                var clsUpdated = 0;
                foreach (var @class in classes)
                {
                    var change = false;
                    var sbjArr = new List<string>();
                    var classsbjs = _classSubjectService.GetByClassID(@class.ID);
                    if (classsbjs != null)
                        foreach (var sbj in classsbjs)
                        {
                            var crs = _courseService.GetItemByID(sbj.CourseID);
                            if (crs != null && crs.SubjectID != sbj.SubjectID)
                            {
                                sbj.SubjectID = crs.SubjectID;
                                sbj.GradeID = crs.GradeID;
                                _classSubjectService.Save(sbj);
                                msg += ((sbj.CourseName) + ";");
                                csbjUpdated++;
                                change = true;

                            }
                            else
                            {
                                switch (sbj.SubjectID)
                                {
                                    case "":
                                        break;
                                    default:
                                        var idx = originSubject.IndexOf(sbj.SubjectID);
                                        if (idx >= 0)
                                        {
                                            //update course
                                            sbj.SubjectID = targetSubject[idx];
                                            var idxGrd = originGrade.IndexOf(sbj.GradeID);
                                            if (idxGrd >= 0)
                                            {
                                                //update course
                                                sbj.GradeID = targetGrade[idxGrd];
                                                _classSubjectService.Save(sbj);
                                                msg += ((sbj.ID) + ";");
                                                csbjUpdated++;
                                                change = true;
                                            }
                                        }
                                        else
                                        {
                                            if (sbj.CourseName != null)
                                            {
                                                if (sbj.CourseName.ToLower().Contains("preielts"))
                                                {
                                                    if (sbj.SubjectID != "5fc0abdd107ea728b4823937")
                                                    {
                                                        sbj.SubjectID = "5fc0abdd107ea728b4823937";//Cambridge
                                                        sbj.GradeID = "5fc0ad2d107ea728b482399b";//B1
                                                        _classSubjectService.Save(sbj);
                                                        csbjUpdated++;
                                                        msg += ((sbj.CourseName) + ";");
                                                        change = true;
                                                    }
                                                }
                                                else if (sbj.CourseName.ToLower().Contains("30 chủ đề từ vựng hay"))
                                                {
                                                    if (sbj.SubjectID != "5fc0c99d107ea728b4824508")
                                                    {
                                                        sbj.SubjectID = "5fc0c99d107ea728b4824508";//Từ vựng
                                                        sbj.GradeID = "5fc0c9b5107ea728b482450c";//Chung
                                                        _classSubjectService.Save(sbj);
                                                        csbjUpdated++;
                                                        msg += ((sbj.CourseName) + ";");
                                                        change = true;
                                                    }
                                                }
                                                else if (sbj.CourseName.ToLower().Contains("toán") ||
                                                    sbj.CourseName.ToLower().Contains("địa lý") ||
                                                    sbj.CourseName.ToLower().Contains("lịch sử") ||
                                                    sbj.CourseName.ToLower().Contains("tiếng việt") ||
                                                    sbj.CourseName.ToLower().Contains("ngữ văn") ||
                                                    sbj.CourseName.ToLower().Contains("sinh học") ||
                                                    sbj.CourseName.ToLower().Contains("vật lý") ||
                                                    sbj.CourseName.ToLower().Contains("hóa học") ||
                                                    sbj.CourseName.ToLower().Contains("hsg lý") ||
                                                    sbj.CourseName.ToLower().Contains("gdcd")
                                                    )
                                                {
                                                    if (sbj.SubjectID != "5fc0abfd107ea728b4823945")
                                                    {
                                                        sbj.SubjectID = "5fc0abfd107ea728b4823945";//Khác
                                                        sbj.GradeID = "5fc0ae47107ea728b48239e6";//Chung
                                                        _classSubjectService.Save(sbj);
                                                        msg += ((sbj.CourseName) + ";");
                                                        csbjUpdated++;
                                                        change = true;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                }
                            }
                            if (!sbjArr.Contains(sbj.SubjectID))
                                sbjArr.Add(sbj.SubjectID);
                        }
                    @class.Subjects = sbjArr;
                    if (change)
                    {
                        _classService.Save(@class);
                        clsUpdated++;
                    }
                }



                //update teacher

                msg += ("====> " + courseUpdated + " courses - " + csbjUpdated + " class subject - " + clsUpdated + " class <===== "); // + tcherUpdated + " teacher => ");
            }

            //if (center.Code == "benh-vien-viet-duc")
            //       continue;
            var tcherUpdated = 0;
            var teachers = _teacherService.GetAll().ToList();
            //GetByCenterID(center.ID);
            foreach (var teacher in teachers)
            {
                if (teacher.Email.ToLower() == "huonghl@utc.edu.vn") continue;
                if (teacher.Subjects == null) teacher.Subjects = new List<string>();
                teacher.Subjects.RemoveAll(t => removeSubject.Contains(t));
                //if (center.Code == "benh-vien-viet-duc")
                //       continue;
                foreach (var target in keepSubject)
                    if (!teacher.Subjects.Contains(target))
                        teacher.Subjects.Add(target);
                _teacherService.Save(teacher);
                tcherUpdated++;
            }
            msg += (tcherUpdated + " teacher =< ");


            return Json(msg);
        }

        public JsonResult FixSchedule()
        {
            var schedules = _lessonScheduleService.GetAll().Limit(20000).ToList();
            var start = DateTime.Now;
            var begin = DateTime.Now;
            var counter = 0;
            var str = "Start: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
            while (schedules.Count > 0)
            {
                start = DateTime.Now;
                counter += schedules.Count;
                var bulkLessonOps = new List<WriteModel<LessonEntity>>();
                var bulkCalendarOps = new List<WriteModel<CalendarEntity>>();
                var listid = new List<string>();

                foreach (var schedule in schedules)
                {
                    listid.Add(schedule.ID);
                    //add info to lesson
                    var lesson = _lessonService.GetItemByID(schedule.LessonID);
                    //lesson.IsActive = schedule.IsActive;
                    lesson.StartDate = schedule.StartDate;
                    lesson.EndDate = schedule.EndDate;
                    lesson.IsHideAnswer = schedule.IsHideAnswer;
                    lesson.IsOnline = schedule.IsOnline;
                    var updateLesson = new UpdateOneModel<LessonEntity>(
                         Builders<LessonEntity>.Filter.Where(t => t.ID == schedule.LessonID),
                         Builders<LessonEntity>.Update
                            .Set(t => t.StartDate, schedule.StartDate)
                            .Set(t => t.EndDate, schedule.EndDate)
                            .Set(t => t.IsHideAnswer, schedule.IsHideAnswer)
                            .Set(t => t.IsOnline, schedule.IsOnline));
                    bulkLessonOps.Add(updateLesson);

                    //change calendar schedule => lesson id

                    var update = new UpdateOneModel<CalendarEntity>(
                        Builders<CalendarEntity>.Filter.Where(t => t.ScheduleID == schedule.ID),
                        Builders<CalendarEntity>.Update.Set(t => t.ScheduleID, schedule.LessonID)
                            .Set(t => t.EndDate, schedule.EndDate)
                        )
                    { IsUpsert = false };
                    //_calendarService.CreateQuery().UpdateOne(t => t.ScheduleID == schedule.ID, Builders<CalendarEntity>.Update.Set(t => t.ScheduleID, lesson.ID));
                    bulkCalendarOps.Add(update);
                }
                if (bulkCalendarOps.Count > 0)
                    _calendarService.Collection.BulkWrite(bulkCalendarOps);
                if (bulkLessonOps.Count > 0)
                    _lessonService.Collection.BulkWrite(bulkLessonOps);

                _lessonScheduleService.Collection.DeleteMany<LessonScheduleEntity>(t => listid.Contains(t.ID));

                schedules = _lessonScheduleService.GetAll().Limit(20000).ToList();
                str += " - Round " + counter + ": " + (DateTime.Now - start).TotalMilliseconds + "; ";
                Debug.WriteLine(counter + " - " + (DateTime.Now - begin).TotalMilliseconds);
                start = DateTime.Now;
            }
            str += ("Đã fix " + counter + "; Total: " + (DateTime.Now - begin).TotalMilliseconds + " - End: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
            return Json(str);
        }

        #endregion

        #region Mở rộng

        public IActionResult FixDataPractice()
        {
            var lesson = _courseLessonService.CreateQuery().Find(x => x.IsPractice == false && x.TemplateType == 1);
            var lessonIDs = lesson.ToList().Select(x => x.ID).ToList();
            var lessonparts = _lessonPartService.CreateQuery().Find(x => lessonIDs.Contains(x.ParentID)).ToList();
            var glessonparts = lessonparts.GroupBy(x => x.Type).ToList();
            var clonelessonparts = _clonelessonPartService.CreateQuery().Find(x => lessonIDs.Contains(x.ParentID)).ToList();
            var gclonelessonparts = clonelessonparts.GroupBy(x => x.Type).ToList();
            return Content("");
        }

        public IActionResult FixPoint()
        {
            return Content("");
            //try
            //{
            //    var listLessonID = new List<String> {
            //    "600553e8712e02232c6d4983",
            //    "6005580c712e02232c6ddf31",
            //    "6005544e712e02232c6d4a99",
            //    "6005582c712e02232c6de042",
            //    "60055867712e02232c6de25e",
            //    "6005584b712e02232c6de153",
            //    "600554e6712e02232c6d4bbc",
            //    "60055730712e02232c6d6a1c"
            //    };

            //    foreach (var id in listLessonID)
            //    {
            //        var old = _lessonService.GetItemByID(id);
            //        old.Point = 50;
            //        _lessonService.Save(old);

            //        var es = _examService.CreateQuery().Find(x => x.LessonID == id).ToList();
            //        foreach (var e in es)
            //        {
            //            if (e.MaxPoint == 0)
            //            {
            //                e.MaxPoint = old.Point;
            //                _examService.Save(e);
            //            }
            //        }
            //    }

            //    //var center = _centerService.GetItemByID("5f531183e4f8492394f600b9");//vinh yen
            //    //var @class = _classService.CreateQuery().Find(x => x.Center.Equals(center.ID));
            //    //var listClassID = @class.Project(x => x.ID).ToList();

            //    //var listStudent = _studentService.CreateQuery().Find(x => x.Centers.Contains(center.ID) && x.JoinedClasses.Any(y => listClassID.Contains(y)));
            //    //var listStudentID = listStudent.Project(x => x.ID).ToList();

            //    foreach (var lessonid in listLessonID)
            //    {
            //        var lesson = _lessonService.GetItemByID(lessonid);
            //        var progess = _lessonProgressService.CreateQuery().Find(x => x.LessonID == lessonid);
            //        var exams = _examService.CreateQuery().Find(x => x.LessonID == lessonid).ToList().GroupBy(x => x.StudentID);
            //        foreach (var exam in exams)
            //        {
            //            var stid = exam.Key;
            //            var lexam = exam.ToList().OrderByDescending(x => x.Number);

            //            var item = progess.ToList().Where(x => x.StudentID == stid).ToList();
            //            //if (item.Count() > 1)
            //            //{
            //            //    var a = "";
            //            //}
            //            if (lexam.Count() == 1)
            //            {
            //                var a = item.FirstOrDefault();
            //                //if(a.StudentID == "5f5f2fbeef17391d0c61f1e9")
            //                //{
            //                //    var a1 = "";
            //                //}
            //                a.LastPoint = lesson.Point == 0 ? 0 : (lexam.FirstOrDefault().Point / lesson.Point) * 100;
            //                a.AvgPoint = a.LastPoint;
            //                a.LastTry = lexam.FirstOrDefault().Updated;
            //                _lessonProgressService.Save(a);
            //            }
            //            else
            //            {
            //                var a = item.FirstOrDefault();
            //                a.LastPoint = lesson.Point == 0 ? 0 : (lexam.FirstOrDefault().Point / lesson.Point) * 100;
            //                a.AvgPoint = lexam.Average(x => x.Point / x.MaxPoint);
            //                a.LastTry = lexam.FirstOrDefault().Updated;
            //                _lessonProgressService.Save(a);
            //            }
            //        }
            //        //    //var progess = _lessonProgressService.CreateQuery().Find(x => lessonid.Equals(x.LessonID) && listClassID.Contains(x.ClassID) && listStudentID.Contains(x.StudentID)).ToList();
            //        //    ////var progess = _lessonProgressService.CreateQuery().Find(x => lessonid.Contains(x.LessonID) && listClassID.Contains(x.ClassID) && listStudentID.Contains(x.StudentID)).ToList();
            //        //    ////var exams = (from e in _examService.CreateQuery().Find(x => listLessonID.Contains(x.LessonID) && listStudentID.Contains(x.StudentID)).ToList()
            //        //    ////            group e by e.StudentID
            //        //    ////            into g
            //        //    ////            let maxExam = g.ToList().OrderByDescending(x => x.Number).FirstOrDefault()
            //        //    ////            where maxExam != null
            //        //    ////            select maxExam).ToList();

            //        //    ////var exams = _examService.CreateQuery().Find(x => lessonid.Contains(x.LessonID) && listStudentID.Contains(x.StudentID)).ToList();
            //        //    //var exams = _examService.CreateQuery().Find(x => lessonid.Equals(x.LessonID) && listStudentID.Contains(x.StudentID)).ToList();

            //        //    //foreach (var item in progess)
            //        //    //{
            //        //    //    if (item.LessonID == "60015fc3782f4335041235ac")
            //        //    //    {
            //        //    //        var student123 = exams.Where(x => x.StudentID == "5f5f2fbeef17391d0c61f1e9" && x.LessonID == "60015fc3782f4335041235ac").ToList();
            //        //    //    }
            //        //    //    var exam = exams.Where(x => x.LessonID == item.LessonID && x.StudentID == item.StudentID).OrderByDescending(x => x.Number).ToList();
            //        //    //    var max = exam.FirstOrDefault().Point / lesson.Point;
            //        //    //    var min = exam.Count > 0 ? exam.LastOrDefault().Point / lesson.Point : 0;
            //        //    //    item.MaxPoint = max;
            //        //    //    item.MinPoint = min;
            //        //    //    item.AvgPoint = exam.Average(x => x.LastPoint) / lesson.Point;
            //        //    //    item.PointChange = max - item.LastPoint;
            //        //    //    item.LastTry = exam.FirstOrDefault().Updated;
            //        //    //    item.LastPoint = max / lesson.Point;
            //        //    //    _ = _lessonProgressService.Save(item);
            //        //    //}

            //        }
            //        return Content("OK");
            //}
            //catch(Exception ex)
            //{
            //    return Content(ex.Message);
            //}
        }

        public IActionResult SendMail(String email)
        {
            try
            {
                var acc = _accountService.GetAccountByEmail(email);
                var student = _studentService.GetStudentByEmail(acc.UserName);
                var center = _centerService.CreateQuery().Find(x => student.Centers.Contains(x.ID)).FirstOrDefault();
                var pass = "edu123456";
                if (acc != null)
                {
                    _mailHelper.SendStudentJoinCenterNotify(student.FullName, acc.UserName, pass, center.Name);
                    return Content($"Send mail to {acc.UserName} is OK");
                }
                else
                {
                    return Content("acc null");
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        //public IActionResult FixData()
        //{
        //    try
        //    {
        //        var lessons = _lessonService.CreateQuery().Find(x => x.TemplateType == 1 && x.IsPractice == false).ToList();
        //        foreach(var item in lessons)
        //        {
        //            item.IsPractice = true;
        //            _lessonService.Save(item);
        //        }
        //        return Content($"OK - {lessons.Count()}");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(ex.Message);
        //    }
        //}

        //public IActionResult Center()
        //{
        //    try
        //    {
        //        var centers = _centerService.GetAll();
        //        foreach (var item in centers.ToList())
        //        {
        //            if (item.ExpireDate <= DateTime.Now)
        //            {
        //                item.ExpireDate = new DateTime(2021,1,31);
        //                _centerService.Save(item);
        //            }
        //        }
        //        return Content("done");
        //    }
        //    catch(Exception ex)
        //    {
        //        return Content(ex.Message);
        //    }
        //}
        #endregion
    }
}
