using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Core_v2.Globals;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseCustomerMVC.Controllers.Student
{
    public class LessonController : StudentController
    {
        private readonly SubjectService _subjectService;
        private readonly CourseService _courseService;
        private readonly ClassService _classService;
        //private readonly ClassStudentService _classStudentService;
        private readonly StudentService _studentService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly ChapterService _chapterService;
        private readonly ChapterProgressService _chapterProgressService;
        private readonly LessonScheduleService _lessonScheduleService;

        private readonly LessonService _lessonService;
        private readonly LessonPartService _lessonPartService;
        private readonly LessonHelper _lessonHelper;
        private readonly CenterService _centerService;

        //private readonly LessonPartQuestionService _lessonPartQuestionService;
        //private readonly LessonPartAnswerService _lessonPartAnswerService;

        private readonly ExamService _examService;
        private readonly ExamDetailService _examDetailService;

        private readonly LearningHistoryService _learningHistoryService;
        private readonly ProgressHelper _progressHelper;

        private readonly CloneLessonPartService _cloneLessonPartService;
        private readonly CloneLessonPartAnswerService _cloneLessonPartAnswerService;
        private readonly CloneLessonPartQuestionService _cloneLessonPartQuestionService;

        private readonly MappingEntity<LessonEntity, LessonScheduleViewModel> _schedulemapping;
        private readonly VocabularyService _vocabularyService;
        private readonly List<string> quizType = new List<string> { "QUIZ1", "QUIZ2", "QUIZ3", "QUIZ4", "ESSAY" };

        //private readonly MappingEntity<LessonPartEntity, CloneLessonPartEntity> _lessonPartMapping;
        //private readonly MappingEntity<LessonPartQuestionEntity, CloneLessonPartQuestionEntity> _lessonPartQuestionMapping;
        //private readonly MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity> _lessonPartAnswerMapping;

        public LessonController(
            SubjectService subjectService,
            CourseService courseService,
            ClassService classService,
            StudentService studentService,
            ClassSubjectService classSubjectService,
            ChapterService chapterService,
            ProgressHelper progressHelper,
            ChapterProgressService chapterProgressService,
            LessonScheduleService lessonScheduleService,
            LearningHistoryService learningHistoryService,
            LessonService lessonService,
            LessonHelper lessonHelper,
            ExamService examService,
            ExamDetailService examDetailService,
            LessonPartService lessonPartService,
            //, LessonPartQuestionService lessonPartQuestionService
            //, LessonPartAnswerService lessonPartAnswerService

            CloneLessonPartService cloneLessonPartService,
            CloneLessonPartAnswerService cloneLessonPartAnswerService,
            CloneLessonPartQuestionService cloneLessonPartQuestionService,
            VocabularyService vocabularyService,
            CenterService centerService
            )
        {
            _subjectService = subjectService;
            _courseService = courseService;
            _classService = classService;
            //_classStudentService = classStudentService;
            _studentService = studentService;
            _classSubjectService = classSubjectService;
            _chapterService = chapterService;
            _chapterProgressService = chapterProgressService;
            _lessonScheduleService = lessonScheduleService;
            _learningHistoryService = learningHistoryService;

            _lessonService = lessonService;
            _lessonHelper = lessonHelper;
            _lessonPartService = lessonPartService;
            _examService = examService;
            _examDetailService = examDetailService;

            _cloneLessonPartService = cloneLessonPartService;
            _cloneLessonPartQuestionService = cloneLessonPartQuestionService;
            _cloneLessonPartAnswerService = cloneLessonPartAnswerService;

            _schedulemapping = new MappingEntity<LessonEntity, LessonScheduleViewModel>();
            _vocabularyService = vocabularyService;
            _progressHelper = progressHelper;
            _centerService = centerService;
        }

        //public IActionResult Index()
        //{
        //    var userid = User.Claims.GetClaimByType("UserID").Value;
        //    ViewBag.User = userid;

        //    var subjectids = _classService.CreateQuery().Find(o => o.Students.Contains(userid)).ToList().Select(x => x.SubjectID).ToList();
        //    var subject = _subjectService.CreateQuery().Find(t => subjectids.Contains(t.ID)).ToList();

        //    ViewBag.Subject = subject;

        //    return View();
        //}

        public JsonResult GetList(DefaultModel model)
        {
            //student id
            var userid = User.Claims.GetClaimByType("UserID").Value;
            // lấy class theo student id
            var data = _classService.Collection.Find(o => o.IsActive == true && o.Students.Contains(userid)).ToList();

            if (data != null && data.Count > 0)
            {
                var mapping = new MappingEntity<ClassEntity, TodayClassViewModel>();
                var map2 = new MappingEntity<LessonEntity, LessonScheduleTodayViewModel>() { };
                //id class
                var listID = data.Select(o => o.ID).ToList();
                // lịch học hôm nay
                var schedule = _lessonScheduleService.Collection.Find(o => listID.Contains(o.ClassID)).ToList();
                // có list lessonid
                var listIDSchedule = schedule.Select(x => x.LessonID).ToList();

                var resData = data.Select(o => mapping.AutoOrtherType(o, new TodayClassViewModel()
                {
                    Lessons = schedule != null ? _lessonService.Collection.Find(y => listIDSchedule.Contains(y.ID)).ToList()
                        .Select(y => map2.AutoOrtherType(y, new LessonScheduleTodayViewModel()
                        {
                            ClassID = schedule.SingleOrDefault(x => x.LessonID == y.ID)?.ClassID
                        })).ToList() : null
                }));

                var response = new Dictionary<string, object>()
                    {
                        {"Data", resData }
                    };
                return new JsonResult(response);
            }
            var response2 = new Dictionary<string, object>()
            {
                {"Data", data }
            };
            return new JsonResult(response2);
        }

        //[Obsolete]
        //[HttpPost]
        //public JsonResult GetActiveList(DefaultModel model, string ClassID = "", string UserID = "", string SubjectID = "")
        //{
        //    if (string.IsNullOrEmpty(UserID))
        //        UserID = User.Claims.GetClaimByType("UserID").Value;

        //    var subjects = _subjectService.GetAll().ToList();

        //    var classFilter = new List<FilterDefinition<ClassEntity>>();
        //    classFilter.Add(Builders<ClassEntity>.Filter.Where(o => o.Students.Contains(UserID)));

        //    if (!string.IsNullOrEmpty(SubjectID))
        //    {
        //        classFilter.Add(Builders<ClassEntity>.Filter.Where(o => o.SubjectID == SubjectID));
        //    }

        //    var activeClass = _classService.CreateQuery().Find(Builders<ClassEntity>.Filter.And(classFilter)).ToList();
        //    var activeClassIDs = activeClass.Select(t => t.ID).ToList();

        //    var data = (from r in _lessonScheduleService.CreateQuery().Find(o => activeClassIDs.Contains(o.ClassID) && o.IsActive && o.EndDate >= model.StartDate && o.StartDate <= model.EndDate).ToList()
        //                let currentClass = activeClass.SingleOrDefault(o => o.ID == r.ClassID)
        //                let subject = subjects.SingleOrDefault(s => s.ID == currentClass.SubjectID)
        //                select _schedulemapping.AutoOrtherType(_lessonService.GetItemByID(r.LessonID), new LessonScheduleViewModel()
        //                {
        //                    ScheduleID = r.ID,
        //                    StartDate = r.StartDate,
        //                    EndDate = r.EndDate,
        //                    IsActive = r.IsActive,
        //                    ClassID = currentClass.ID,
        //                    SubjectName = subject.Name,
        //                    ClassName = currentClass.Name
        //                }));

        //    model.TotalRecord = data.Count();
        //    var returnData = data == null || data.Count() <= 0 || data.Count() < model.PageSize || model.PageSize <= 0
        //        ? data.ToList()
        //        : data.Skip((model.PageIndex - 1) * model.PageSize).Take(model.PageSize).ToList();
        //    var response = new Dictionary<string, object>
        //    {
        //        { "Data", returnData },
        //        { "Model", model }
        //    };
        //    return new JsonResult(response);
        //}

        [Obsolete]
        [HttpPost]
        public JsonResult GetTodayLessons(DefaultModel model, DateTime date, string UserID = "")
        {
            if (string.IsNullOrEmpty(UserID))
                UserID = User.Claims.GetClaimByType("UserID").Value;

            var subjects = _subjectService.GetAll().ToList();

            var classFilter = new List<FilterDefinition<ClassEntity>>();
            classFilter.Add(Builders<ClassEntity>.Filter.Where(o => o.Students.Contains(UserID)));

            var activeClass = _classService.CreateQuery().Find(Builders<ClassEntity>.Filter.And(classFilter)).ToEnumerable();
            var activeClassIDs = activeClass.Select(t => t.ID).ToList();

            var data = new List<LessonScheduleViewModel>();

            var startdate = date.ToLocalTime().Date;
            var enddate = date.AddDays(1);
            foreach (var _class in activeClass)
            {
                var schedule = _lessonScheduleService.CreateQuery().Find(o => o.ClassID == _class.ID
                //&& o.IsActive 
                && o.EndDate >= startdate && o.StartDate <= enddate).FirstOrDefault();
                if (schedule != null)
                {
                    var lesson = _lessonService.GetItemByID(schedule.LessonID);
                    if (lesson != null)
                    {
                        //var subject = subjects.SingleOrDefault(o => o.ID == _class.SubjectID);
                        data.Add(_schedulemapping.AutoOrtherType(lesson, new LessonScheduleViewModel()
                        {
                            ScheduleID = schedule.ID,
                            StartDate = schedule.StartDate,
                            EndDate = schedule.EndDate,
                            IsActive = schedule.IsActive,
                            ClassID = _class.ID,
                            //SubjectName = subject.Name,
                            ClassName = _class.Name
                        }));
                    }
                }
            }
            model.TotalRecord = data.Count();
            var returnData = data.ToList();
            var response = new Dictionary<string, object>
            {
                { "Data", returnData },
                { "Model", model }
            };
            return new JsonResult(response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id">LessonID</param>
        /// <param name="ClassID">ClassID</param>
        /// <returns></returns>
        public IActionResult Detail(DefaultModel model, string basis, string ClassID)
        {
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            var student = _studentService.GetItemByID(UserID);
            if (ClassID == null)
                return Redirect($"/{basis}{Url.Action("Index", "Course")}");
            var currentCs = _classSubjectService.GetItemByID(ClassID);
            if (currentCs == null)
                return Redirect($"/{basis}{Url.Action("Index", "Course")}");
            var currentClass = _classService.GetItemByID(currentCs.ClassID);
            if (currentClass == null)
                return Redirect($"/{basis}{Url.Action("Index", "Course")}");
            var lesson = _lessonService.GetItemByID(model.ID);
            if (lesson == null)
                return Redirect($"/{basis}{Url.Action("Index", "Course")}");

            var chapter = _chapterService.GetItemByID(lesson.ChapterID);
            var pass = true;
            ViewBag.Lesson = lesson;
            ViewBag.Type = lesson.TemplateType;
            string condChap = "";
            if (chapter != null && !String.IsNullOrEmpty(chapter.ConditionChapter))//has condition
            {
                var conditionchap = _chapterService.GetItemByID(chapter.ConditionChapter);
                if (conditionchap != null)
                {
                    condChap = conditionchap.Name;
                    if (conditionchap.BasePoint > 0 && chapter.PracticeCount > 0)
                    {
                        //check condition
                        var progress = _chapterProgressService.GetItemByChapterID(conditionchap.ID, UserID);
                        if (progress == null)
                        {
                            pass = false;
                        }
                        else
                        {
                            pass = progress.PracticePoint / chapter.PracticeCount >= conditionchap.BasePoint;
                        }
                    }
                }
                else
                {
                    //????
                }
            }
            if (pass == false)
            {
                ViewBag.FailPass = true;
                ViewBag.CondChap = condChap;
            }
            else
            {
                var nextLesson = _lessonService.CreateQuery().Find(t => t.ChapterID == lesson.ChapterID && t.Order > lesson.Order).SortBy(t => t.Order).FirstOrDefault();
                ViewBag.Class = currentClass;
                if (string.IsNullOrEmpty(currentCs.CourseName))
                    currentCs.CourseName = _courseService.GetItemByID(currentCs.CourseID)?.Name;
                ViewBag.Subject = currentCs;
                ViewBag.NextLesson = nextLesson;
                ViewBag.Chapter = chapter;
            }
            ViewBag.Center = _centerService.GetItemByCode(basis);
            ViewBag.CurrentUser = student;
            //if (newui == 1)
            return View("Detail_new");
            //return View();
        }

        public IActionResult Review(DefaultModel model, string basis)
        {
            var UserID = User.Claims.GetClaimByType("UserID").Value;

            if (string.IsNullOrEmpty(model.ID))
                return Redirect($"/{basis}{Url.Action("Index", "Course")}");

            var exam = _examService.GetItemByID(model.ID);
            if (exam == null)
                return Redirect($"/{basis}{Url.Action("Index", "Course")}");

            var lesson = _lessonService.GetItemByID(exam.LessonID);
            if (lesson == null)
                return Redirect($"/{basis}{Url.Action("Index", "Course")}");

            if (exam.StudentID != UserID && exam.TeacherID != UserID)
                return Redirect($"/{basis}{Url.Action("Index", "Course")}");

            if (!exam.Status)//Check review khi chưa kết thúc bài kiểm tra => hoàn thành bài
            {
                _lessonHelper.CompleteNoEssay(exam, _lessonService.GetItemByID(exam.LessonID), out _);
                //return Redirect($"/{basis}{Url.Action("Index", "Course")}");
                exam = _examService.GetItemByID(exam.ID);
            }

            var nextLesson = _lessonService.CreateQuery().Find(t => t.ChapterID == lesson.ChapterID && t.Order > lesson.Order).SortBy(t => t.Order).FirstOrDefault();

            var currentCs = _classSubjectService.GetItemByID(exam.ClassSubjectID);
            if (currentCs == null)
                return Redirect($"/{basis}{Url.Action("Index", "Course")}");

            var currentClass = _classService.GetItemByID(currentCs.ClassID);
            if (currentClass == null)
                return Redirect($"/{basis}{Url.Action("Index", "Course")}");

            var chapter = _chapterService.GetItemByID(lesson.ChapterID);

            var ExamTypes = quizType;


            var schedule = _lessonScheduleService.GetItemByLessonID(lesson.ID);
            var isHideAnswer = schedule.IsHideAnswer;
            if (schedule.EndDate < DateTime.UtcNow)
                isHideAnswer = false;
            ViewBag.IsHideAnswer = isHideAnswer;

            if (!isHideAnswer)
            {
                var listParts = _cloneLessonPartService.CreateQuery().Find(o => o.ParentID == lesson.ID && o.ClassID == exam.ClassID && ExamTypes.Contains(o.Type)).ToList();

                var mapping = new MappingEntity<LessonEntity, StudentLessonViewModel>();
                var mapPart = new MappingEntity<CloneLessonPartEntity, PartViewModel>();
                var mapQuestion = new MappingEntity<CloneLessonPartQuestionEntity, QuestionViewModel>();
                var mapExam = new MappingEntity<ExamEntity, ExamReviewViewModel>();

                var examview = mapExam.AutoOrtherType(exam, new ExamReviewViewModel()
                {
                    Details = _examDetailService.Collection.Find(t => t.ExamID == exam.ID).ToList()
                });

                var lessonview = mapping.AutoOrtherType(lesson, new StudentLessonViewModel()
                {
                    Part = listParts.Select(o => mapPart.AutoOrtherType(o, new PartViewModel()
                    {
                        Questions = _cloneLessonPartQuestionService.CreateQuery().Find(x => x.ParentID == o.ID).ToList()
                            .Select(z => mapQuestion.AutoOrtherType(z, new QuestionViewModel()
                            {
                                CloneAnswers = _cloneLessonPartAnswerService.CreateQuery().Find(x => x.ParentID == z.ID).ToList(),
                                AnswerEssay = o.Type == "ESSAY" ? _examDetailService.CreateQuery().Find(e => e.QuestionID == z.ID && e.ExamID == exam.ID)?.FirstOrDefault()?.AnswerValue : string.Empty,
                                Medias = examview.Details.FirstOrDefault(e => e.QuestionID == z.ID)?.Medias,
                                TypeAnswer = o.Type,
                                RealAnswerEssay = o.Type == "ESSAY" ? examview.Details.FirstOrDefault(e => e.QuestionID == z.ID)?.RealAnswerValue : string.Empty,
                                PointEssay = examview.Details.FirstOrDefault(e => e.QuestionID == z.ID)?.Point ?? 0,
                                ExamDetailID = examview.Details.FirstOrDefault(e => e.QuestionID == z.ID)?.ID ?? "",
                                MediasAnswer = examview.Details.FirstOrDefault(e => e.QuestionID == z.ID)?.MediasAnswers,
                                MaxPoint = z.Point
                            }))?.ToList()
                    })).ToList()
                });
                ViewBag.Lesson = lessonview;
                ViewBag.Exam = examview;
            }
            else
            {
                ViewBag.Lesson = new StudentLessonViewModel { Title = lesson.Title, ID = lesson.ID };
            }


            ViewBag.Class = currentClass;
            if (string.IsNullOrEmpty(currentCs.CourseName))
                currentCs.CourseName = _courseService.GetItemByID(currentCs.CourseID)?.Name;
            ViewBag.Subject = currentCs;
            ViewBag.NextLesson = nextLesson;
            ViewBag.Chapter = chapter;
            ViewBag.Type = lesson.TemplateType;


            return View();
        }

        [HttpPost]
        public JsonResult GetDetailsLesson(string ID)
        {
            try
            {
                var lesson = _lessonService.CreateQuery().Find(o => o.ID == ID).FirstOrDefault();

                var response = new Dictionary<string, object>
                {
                    { "Data", lesson }
                };
                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", null },
                    {"Error", ex.Message }
                });
            }
        }

        [System.Obsolete]
        [HttpPost]
        public JsonResult GetLesson(string LessonID, string ClassID, string ClassSubjectID)
        {
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userId))
                return new JsonResult(
                new Dictionary<string, object> { { "Error", "Student not found" } });

            var lesson = _lessonService.GetItemByID(LessonID);
            if (lesson == null)
                return new JsonResult(
                new Dictionary<string, object> { { "Error", "Lesson not found" } });

            if (string.IsNullOrEmpty(ClassSubjectID))
                ClassSubjectID = ClassID;

            var currentcs = _classSubjectService.GetItemByID(ClassSubjectID);
            if (currentcs == null)
                return new JsonResult(
                new Dictionary<string, object> { { "Error", "Subject not found" } });

            //if (string.IsNullOrEmpty(ClassID))
            //    ClassID = currentcs.ClassID;

            var currentClass = _classService.GetItemByID(currentcs.ClassID);
            if (currentClass == null)
                return new JsonResult(
                new Dictionary<string, object> { { "Error", "Class not found" } });


            //Create learning history
            _ = _progressHelper.CreateHist(new LearningHistoryEntity()
            {
                ClassID = ClassID,
                ClassSubjectID = ClassSubjectID,
                LessonID = LessonID,
                ChapterID = lesson.ChapterID,
                Time = DateTime.UtcNow,
                StudentID = userId
            });

            var listParts = _cloneLessonPartService.CreateQuery().Find(o => o.ParentID == lesson.ID && o.ClassSubjectID == currentcs.ID).ToList();

            var mapping = new MappingEntity<LessonEntity, StudentLessonViewModel>();
            var mapPart = new MappingEntity<CloneLessonPartEntity, PartViewModel>();
            var mapQuestion = new MappingEntity<CloneLessonPartQuestionEntity, QuestionViewModel>();



            var result = new List<PartViewModel>();
            foreach (var part in listParts)
            {
                var convertedPart = mapPart.AutoOrtherType(part, new PartViewModel());
                switch (part.Type)
                {
                    case "QUIZ1":
                    case "QUIZ3":
                    case "QUIZ4":
                    case "ESSAY":
                        convertedPart.Questions = _cloneLessonPartQuestionService.CreateQuery()
                            .Find(q => q.ParentID == part.ID).SortBy(q => q.Order).ThenBy(q => q.ID).ToList()
                            .Select(q => new QuestionViewModel(q)
                            {
                                CloneAnswers = _cloneLessonPartAnswerService.CreateQuery().Find(x => x.ParentID == q.ID).ToList(),
                                Description = q.Description
                            }).ToList();
                        break;
                    case "QUIZ2":
                        convertedPart.Questions = _cloneLessonPartQuestionService.CreateQuery().Find(q => q.ParentID == part.ID)
                            //.SortBy(q => q.Order).ThenBy(q => q.ID)
                            .ToList()
                            .Select(q => new QuestionViewModel(q)
                            {
                                CloneAnswers = null,
                                Description = null
                            }).ToList();
                        break;
                    case "VOCAB":
                        convertedPart.Description = RenderVocab(part.Description);
                        break;
                    default:
                        break;
                }
                result.Add(convertedPart);
            }

            var dataResponse = mapping.AutoOrtherType(lesson, new StudentLessonViewModel()
            {
                Part = result
            });

            var lastexam = _examService.CreateQuery().Find(o => o.LessonID == LessonID && o.ClassSubjectID == ClassSubjectID
                //&& o.ClassID == ClassID 
                && o.StudentID == userId).SortByDescending(o => o.Created).FirstOrDefault();

            if (lastexam == null)
            {
                var response = new Dictionary<string, object> { { "Data", dataResponse } };
                return new JsonResult(response);
            }
            else //TODO: Double check here
            {
                var currentTimespan = new TimeSpan(0, 0, lesson.Timer, 0);

                if (!lastexam.Status && lesson.Timer > 0) //bài kt cũ chưa xong => check thời gian làm bài
                {
                    var endtime = (lastexam.Created.AddMinutes(lastexam.Timer));
                    if (endtime < DateTime.UtcNow) // hết thời gian 
                    {
                        // => kết thúc bài kt
                        lastexam = _lessonHelper.CompleteNoEssay(lastexam, lesson, out _, false);
                        //throw new NotImplementedException();
                        //lastexam.Status = true;
                        ////TODO: Chấm điểm last exam
                        //_examService.CreateOrUpdate(lastexam);
                    }
                }

                var timeSpan = lastexam.Status ? new TimeSpan(0, 0, lesson.Timer, 0) : (lastexam.Created.AddMinutes(lastexam.Timer) - DateTime.UtcNow);

                //Client check lastexam status để render bài kiểm tra
                return new JsonResult(
                    new Dictionary<string, object> {
                        { "Data", dataResponse },
                        { "Exam", lastexam },
                        { "Timer", (timeSpan.Minutes < 10 ? "0":"") +  timeSpan.Minutes + ":" + (timeSpan.Seconds < 10 ? "0":"") + timeSpan.Seconds }
                    });
            }
        }

        private string RenderVocab(string description)
        {
            string result = "";
            if (string.IsNullOrEmpty(description))
                return "";
            var vocabs = description.Split('|');
            if (vocabs == null || vocabs.Count() == 0)
                return description;
            foreach (var vocab in vocabs)
            {
                var code = vocab.ToLower().Replace(" ", "-");
                var vocabularies = _vocabularyService.GetItemByCode(code);
                if (vocabularies != null && vocabularies.Count > 0)
                {
                    result +=
                        $"<div class='vocab-box'>" +
                            $"<b class='word-title'>{vocab.Trim()}</b><span class='word-pron'>{vocabularies[0].Pronunciation}</span>" +
                            $"<div class='vocab-audio'>" +
                                $"<button onclick='PlayPronun(this)'><i class='ic fas fa-volume-up'></i></button>" +
                                $"<audio class='d-none' id='audio' controls><source src='{vocabularies[0].PronunAudioPath}' type='audio/mpeg' />Your browser does not support the audio tag</audio>" +
                            $"</div>" +
                            //$"<div class='vocab-type'>{string.Join(",", vocabularies.Select(t => t.WordType).ToList())}<div/>" +
                            $"<div class='vocab-meaning'>{string.Join("<br/>", vocabularies.Where(t => !string.IsNullOrEmpty(t.Description)).Select(t => "<b>" + WordType.GetShort(t.WordType) + "</b>: " + t.Description).ToList())}</div>" +
                        $"</div>";
                }
            }
            return result;
        }


        [Obsolete]
        [HttpPost]
        public JsonResult GetSchedules(DefaultModel model)
        {
            var UserID = User.Claims.GetClaimByType("UserID").Value;

            if (string.IsNullOrEmpty(model.ID))
            {
                return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Không có thông tin lớp học" }
                    });
            }

            var currentCs = _classSubjectService.GetItemByID(model.ID);
            if (currentCs == null)
            {
                return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Không có thông tin lớp học" }
                    });
            }

            var currentClass = _classService.GetItemByID(currentCs.ClassID);
            if (currentClass == null)
                return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Không có thông tin lớp học" }
                    });
            //var classStudent = _classStudentService.GetClassStudent(currentClass.ID, UserID);
            //if (classStudent == null)
            if (!_studentService.IsStudentInClass(currentClass.ID, UserID))
                return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Học viên không có trong danh sách lớp" }
                    });

            var course = _courseService.GetItemByID(currentCs.CourseID);

            if (course == null)
            {
                return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Không có thông tin giáo trình" }
                    });
            }

            var classSchedule = new ClassScheduleViewModel(course)
            {
                Chapters = _chapterService.CreateQuery().Find(o => o.CourseID == course.ID).SortBy(o => o.ParentID).ThenBy(o => o.Order).ThenBy(o => o.ID).ToList(),
                //Lessons = (from r in _lessonService.CreateQuery().Find(o => o.CourseID == course.ID).SortBy(o => o.ChapterID).ThenBy(o => o.Order).ThenBy(o => o.ID).ToList()
                //           let schedule = _lessonScheduleService.CreateQuery().Find(o => o.LessonID == r.ID && o.ClassID == model.ID).FirstOrDefault()
                //           let lastjoin = _learningHistoryService.CreateQuery().Find(x => x.StudentID == UserID && x.LessonID == r.ID && x.ClassID == model.ID).SortByDescending(o => o.ID).FirstOrDefault()
                //           select _schedulemapping.AutoOrtherType(r, new LessonScheduleViewModel()
                //           {
                //               ScheduleID = schedule.ID,
                //               StartDate = schedule.StartDate,
                //               EndDate = schedule.EndDate,
                //               IsActive = schedule.IsActive,
                //               IsView = lastjoin != null,
                //               LastJoin = lastjoin != null ? lastjoin.Time : DateTime.MinValue
                //           })).ToList()
            };

            var response = new Dictionary<string, object>
            {
                { "Data", classSchedule },
                { "Model", model }
            };
            return new JsonResult(response);
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetChapterContent(DefaultModel model, string ChapterID)
        {
            var UserID = User.Claims.GetClaimByType("UserID").Value;

            if (string.IsNullOrEmpty(model.ID))
            {
                return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Không có thông tin lớp học" }
                    });
            }

            var currentCs = _classSubjectService.GetItemByID(model.ID);
            if (currentCs == null)
            {
                return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Không có thông tin lớp học" }
                    });
            }

            var currentClass = _classService.GetItemByID(currentCs.ClassID);
            if (currentClass == null)
                return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Không có thông tin lớp học" }
                    });
            //var classStudent = _classStudentService.GetClassStudent(currentClass.ID, UserID);
            //if (classStudent == null)
            if (!_studentService.IsStudentInClass(currentClass.ID, UserID))
                return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Học viên không có trong danh sách lớp" }
                    });

            //var course = _courseService.GetItemByID(currentCs.CourseID);

            //if (course == null)
            //{
            //    return new JsonResult(new Dictionary<string, object> {
            //            {"Data",null },
            //            {"Error",model },
            //            {"Msg","Không có thông tin giáo trình" }
            //        });
            //}

            var classSchedule = new ClassScheduleViewModel()
            {
                Chapters = _chapterService.GetSubChapters(currentCs.ID, ChapterID).ToList(),
                Lessons = (from r in _lessonService.CreateQuery().Find(o => o.ClassSubjectID == currentCs.ID && o.ChapterID == ChapterID).SortBy(o => o.ChapterID).ThenBy(o => o.Order).ThenBy(o => o.ID).ToList()
                           let schedule = _lessonScheduleService.CreateQuery().Find(o => o.LessonID == r.ID && o.ClassSubjectID == model.ID).FirstOrDefault()
                           let lastjoin = _learningHistoryService.CreateQuery().Find(x => x.StudentID == UserID && x.LessonID == r.ID && x.ClassSubjectID == model.ID).SortByDescending(o => o.ID).FirstOrDefault()
                           let lastexam =
                           //r.TemplateType == LESSON_TEMPLATE.EXAM ? 
                           _examService.CreateQuery().Find(x => x.StudentID == UserID && x.LessonID == r.ID && x.ClassSubjectID == model.ID
                           //&& x.Status
                           ).SortByDescending(o => o.ID).FirstOrDefault()
                           //: null //get lastest exam
                           select _schedulemapping.AutoOrtherType(r, new LessonScheduleViewModel()
                           {
                               ScheduleID = schedule.ID,
                               StartDate = schedule.StartDate,
                               EndDate = schedule.EndDate,
                               IsActive = schedule.IsActive,
                               IsView = r.TemplateType == LESSON_TEMPLATE.EXAM ? lastexam != null : lastjoin != null,
                               LastJoin = r.TemplateType == LESSON_TEMPLATE.EXAM ? (lastexam != null ? lastexam.Updated : DateTime.MinValue) :
                                    lastjoin != null ? lastjoin.Time : DateTime.MinValue,
                               DoPoint =
                               (lastexam != null && lastexam.Status) ?
                                    (lastexam.MaxPoint > 0 ? lastexam.Point * 100.00 / lastexam.MaxPoint : 0) : 0,//completed exam only
                               Tried = lastexam != null ? lastexam.Number : 0,
                               LastExam = (lastexam != null && lastexam.Status
                               ) ? lastexam.ID : null
                           })).ToList()
            };

            var response = new Dictionary<string, object>
            {
                { "Data", classSchedule },
                { "Model", model }
            };
            return new JsonResult(response);
        }

        //[Obsolete]
        //[HttpPost]
        //public JsonResult GetAssignments(string ID)
        //{
        //    var UserID = User.Claims.GetClaimByType("UserID").Value;

        //    if (string.IsNullOrEmpty(ID))
        //    {
        //        return new JsonResult(new Dictionary<string, object> {
        //                {"Data",null },
        //                {"Error", ID },
        //                {"Msg","Không có thông tin lớp học" }
        //            });
        //    }

        //    var currentClass = _classService.GetItemByID(ID);
        //    if (currentClass == null || currentClass.Students.IndexOf(UserID) < 0)
        //    {
        //        return new JsonResult(new Dictionary<string, object> {
        //                {"Data",null },
        //                {"Error", ID },
        //                {"Msg","Không có thông tin lớp học" }
        //            });
        //    }

        //    var course = _courseService.GetItemByID(currentClass.CourseID);

        //    if (course == null)
        //    {
        //        return new JsonResult(new Dictionary<string, object> {
        //                {"Data",null },
        //                {"Error", ID },
        //                {"Msg","Không có thông tin giáo trình" }
        //            });
        //    }

        //    var classSchedule = new ClassScheduleViewModel(course)
        //    {

        //        Lessons = (from r in _lessonService.CreateQuery().Find(o => o.Class == course.ID
        //                   //&& o.Etype > 0
        //                   ).SortBy(o => o.ChapterID).ThenBy(o => o.Order).ThenBy(o => o.ID).ToList()
        //                   let schedule = _lessonScheduleService.CreateQuery().Find(o => o.LessonID == r.ID && o.ClassID == ID).FirstOrDefault()
        //                   let lastjoin = _learningHistoryService.CreateQuery().Find(x => x.StudentID == UserID && x.LessonID == r.ID && x.ClassID == ID).SortByDescending(o => o.ID).FirstOrDefault()
        //                   select _schedulemapping.AutoOrtherType(r, new LessonScheduleViewModel()
        //                   {
        //                       ScheduleID = schedule.ID,
        //                       StartDate = schedule.StartDate,
        //                       EndDate = schedule.EndDate,
        //                       IsActive = schedule.IsActive,
        //                       IsView = lastjoin != null,
        //                       LastJoin = lastjoin != null ? lastjoin.Time : DateTime.MinValue
        //                   })).ToList()
        //    };

        //    var response = new Dictionary<string, object>
        //    {
        //        { "Data", classSchedule },
        //        { "Model", ID }
        //    };
        //    return new JsonResult(response);
        //}
    }
}
