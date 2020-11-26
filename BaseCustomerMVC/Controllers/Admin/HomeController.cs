using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using Core_v2.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
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

        private readonly CourseLessonService _lessonService;
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

        private string host;
        private string staticPath;
        private string RootPath { get; }
        private readonly IHostingEnvironment _env;

        public HomeController(
                CourseLessonService lessonService,
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

                CenterService centerService,
                StudentService studentService,
                TeacherService teacherService,
                IConfiguration iConfig,
                IHostingEnvironment env,
                ReferenceService referenceService,
                CalendarService calendarService,
                LessonScheduleService lessonScheduleService,
                LearningHistoryService learningHistoryService
            )
        {
            _lessonService = lessonService;
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

            _centerService = centerService;
            _studentService = studentService;
            _teacherService = teacherService;
            _referenceService = referenceService;
            _calendarService = calendarService;
            _lessonScheduleService = lessonScheduleService;
            _learningHistoryService = learningHistoryService;

            _env = env;

            host = iConfig.GetValue<string>("SysConfig:Domain");
            staticPath = iConfig.GetValue<string>("SysConfig:StaticPath");
            RootPath = staticPath ?? _env.WebRootPath;
        }

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        //public async Task<JsonResult> FixScoreData()//Big fix
        //{
        //    var start = DateTime.UtcNow;
        //    var str = "";
        //    str += "Phase 0: ";
        //    //Clear orphan part: Run Once

        //    //var ids = _lessonService.GetAll().Project(t => t.ID).ToList();


        //    //long delpart = 0;
        //    //var delIds = _lessonPartService.CreateQuery().Find(t => !ids.Contains(t.ParentID)).Project(t => t.ID).ToList();

        //    //foreach (var partid in delIds)
        //    //{
        //    //    var part = _lessonPartService.GetItemByID(partid);
        //    //    if (_lessonService.GetItemByID(part.ParentID) == null)
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
        //    var clids = _lessonService.GetAll().Project(t => t.ID).ToList();
        //    var listclass = new List<string>();
        //    foreach (var clid in clids)
        //    {
        //        var cl = _lessonService.GetItemByID(clid);
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

        public async Task<JsonResult> FixScoreDataV2()
        {
            var start = DateTime.UtcNow;
            var str = "";
            str += "Phase 0: ";
            var change = 0;


            //var clids = _lessonService.GetAll().Project(t => t.ID).ToList();
            //foreach (var clid in clids)
            //{
            //    var cl = _lessonService.GetItemByID(clid);
            //    var oldpoint = cl.Point;
            //    var isPrac = cl.IsPractice;
            //    var newPrac = isPrac;
            //    var point = calculateLessonPoint(cl, newPrac);
            //    if (isPrac != newPrac)
            //        change++;
            //}
            //str += " Course Lesson Point change " + change;

            //var lids = _clonelessonService.GetAll().Project(t => t.ID).ToList();

            //change = 0;
            //foreach (var lid in lids)
            //{

            //    var l = _clonelessonService.GetItemByID(lid);
            //    var oldpiont = l.Point;
            //    var isPrac = l.IsPractice;
            //    var newPrac = isPrac;
            //    var point = calculateCloneLessonPoint(l, newPrac);


            //    if (isPrac != newPrac)
            //        change++;
            //}
            //str += " - Lesson Point change " + change;

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

            //_lessonProgressService.CreateQuery().UpdateMany(t => true, Builders<LessonProgressEntity>.Update
            //    .Set(t => t.AvgPoint, 0)
            //    .Set(t => t.LastPoint, 0)
            //    .Set(t => t.LastTry, DateTime.MinValue)
            //    .Set(t => t.MaxPoint, 0)
            //    .Set(t => t.MinPoint, 0)
            //    .Set(t => t.Tried, 0)
            //    );

            //str += (DateTime.UtcNow - start).TotalSeconds;
            //start = DateTime.UtcNow;
            //str += " Phase 5: ";

            var lessonProgresses = _lessonProgressService
                //.CreateQuery().Find(t => t.ClassID == "5f60dd6b0dd2b41448907f26" && t.StudentID == "5f60e2e90dd2b41448909d05")
                .GetAll()
                //.Project(t => new LessonProgressEntity { ID = t.ID, LessonID = t.LessonID, StudentID = t.StudentID })
                .ToList();
            lessonProgresses = lessonProgresses.OrderBy(t => t.LessonID).ThenBy(t => t.Tried).ToList();

            foreach (var lprg in lessonProgresses)
            {
                if (lprg.ChapterID != "0")
                    await _progressHelper.UpdateChapterLastLearn(lprg, true);
                else
                    await _progressHelper.UpdateClassSubjectLastLearn(new ClassSubjectProgressEntity { LastLessonID = lprg.LessonID, ClassSubjectID = lprg.ClassSubjectID, ClassID = lprg.ClassID, LastDate = lprg.LastDate }, true);

                var lesson = _clonelessonService.GetItemByID(lprg.LessonID);
                if (lesson.IsPractice)
                {
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

            str += (DateTime.UtcNow - start).TotalSeconds;
            start = DateTime.UtcNow;
            str += " End. ";
            return Json(str);
        }

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
            _lessonService.Save(lesson);
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

        #region Fix Region
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
                    _ = _lessonScheduleService.RemoveManyClass(ids);
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
                    _lessonService.CreateQuery().DeleteMany(o => ids.Contains(o.CourseID));
                    _lessonPartService.CreateQuery().DeleteMany(o => ids.Contains(o.CourseID));
                    _questionService.CreateQuery().DeleteMany(o => ids.Contains(o.CourseID));
                    _answerService.CreateQuery().DeleteMany(o => ids.Contains(o.CourseID));
                }
            }

            return Json("OK");
        }


        public JsonResult ShareStudent()
        {
            var courses = _courseService.CreateQuery().Find(t => t.IsActive && t.PublicWStudent).ToList();
            var count = 0;
            foreach (var course in courses)
            {
                if (course.StudentTargetCenters == null)
                {
                    course.StudentTargetCenters = course.TargetCenters;
                    course.PublicWStudent = false;
                    _courseService.Save(course);
                    count++;
                }
            }
            return Json("OK:" + count);
        }


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

        public JsonResult FixCalendar()
        {
            var calendars = _calendarService.GetAll().ToEnumerable();
            var count = 0;
            foreach (var calendar in calendars)
            {
                var sch = _lessonScheduleService.GetItemByID(calendar.ScheduleID);
                if (sch == null)
                {
                    _calendarService.Remove(calendar.ID);
                    count++;
                }
            }
            return Json("DEL " + count);
        }

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
        #endregion
    }
}
