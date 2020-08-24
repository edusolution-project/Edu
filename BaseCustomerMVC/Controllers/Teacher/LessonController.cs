using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using MongoDB.Driver;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Core_v2.Globals;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class LessonController : TeacherController
    {
        private readonly GradeService _gradeService;
        private readonly SubjectService _subjectService;
        private readonly TeacherService _teacherService;
        private readonly ClassService _classService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly CourseService _courseService;
        private readonly ChapterService _chapterService;
        private readonly LessonService _lessonService;
        private readonly LessonPartService _lessonPartService;
        private readonly LessonPartQuestionService _questionService;
        private readonly LessonPartAnswerService _answerService;
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly CenterService _centerService;

        private readonly CloneLessonPartService _clonepartService;
        private readonly CloneLessonPartQuestionService _clonequestionService;
        private readonly CloneLessonPartAnswerService _cloneanswerService;
        private readonly VocabularyService _vocabularyService;


        public LessonController(
            GradeService gradeservice,
            SubjectService subjectService,
            TeacherService teacherService,
            ClassService classService,
            ClassSubjectService classSubjectService,
            CourseService courseService,
            ChapterService chapterService,
            LessonService lessonService,
            LessonPartService lessonPartService,
            LessonPartQuestionService questionService,
            LessonPartAnswerService answerService,
            LessonScheduleService lessonScheduleService,
            CenterService centerService,
            CloneLessonPartService cloneLessonPartService,
            CloneLessonPartQuestionService cloneLessonPartQuestionService,
            CloneLessonPartAnswerService cloneLessonPartAnswerService,
            VocabularyService vocabularyService
            )
        {
            _gradeService = gradeservice;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _courseService = courseService;
            _classService = classService;
            _classSubjectService = classSubjectService;
            _chapterService = chapterService;
            _lessonService = lessonService;
            _lessonPartService = lessonPartService;
            _questionService = questionService;
            _answerService = answerService;
            _lessonScheduleService = lessonScheduleService;
            _centerService = centerService;

            _clonepartService = cloneLessonPartService;
            _clonequestionService = cloneLessonPartQuestionService;
            _cloneanswerService = cloneLessonPartAnswerService;
            _vocabularyService = vocabularyService;
        }

        public IActionResult Detail(DefaultModel model, string basis, string ClassID, int frameview = 0)
        {
            if (!string.IsNullOrEmpty(basis))
            {
                var center = _centerService.GetItemByCode(basis);
                if (center != null)
                    ViewBag.Center = center;
            }

            ViewBag.RoleCode = User.Claims.GetClaimByType(ClaimTypes.Role).Value;
            if (model == null) return null;
            if (ClassID == null)
                return Redirect($"/{basis}{Url.Action("Index", "Class")}");
            var currentClassSubject = _classSubjectService.GetItemByID(ClassID);
            if (currentClassSubject == null)
                return Redirect($"/{basis}{Url.Action("Index", "Class")}");
            var currentClass = _classService.GetItemByID(currentClassSubject.ClassID);
            if (currentClass == null)
                return Redirect($"/{basis}{Url.Action("Index", "Class")}");
            var Data = _lessonService.GetItemByID(model.ID);
            if (Data == null)
                return Redirect($"/{basis}{Url.Action("Index", "Class")}");
            ViewBag.Class = currentClass;
            ViewBag.Subject = currentClassSubject;
            ViewBag.Lesson = Data;
            ViewBag.Title = Data.Title;

            if (frameview == 1)
                return View("FrameDetails");
            return View();
        }

        public IActionResult Preview(DefaultModel model, string basis, string ClassID)
        {
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            if (ClassID == null)
                return Redirect($"/{basis}{Url.Action("Index", "Class")}");
            var currentClassSubject = _classSubjectService.GetItemByID(ClassID);
            if (currentClassSubject == null)
                return Redirect($"/{basis}{Url.Action("Index", "Class")}");
            var currentClass = _classService.GetItemByID(currentClassSubject.ClassID);
            if (currentClass == null)
                return Redirect($"/{basis}{Url.Action("Index", "Class")}");
            var lesson = _lessonService.GetItemByID(model.ID);
            if (lesson == null)
                return Redirect($"/{basis}{Url.Action("Index", "Class")}");

            var chapter = _chapterService.GetItemByID(lesson.ChapterID);
            var pass = true;
            ViewBag.Lesson = lesson;
            ViewBag.Type = lesson.TemplateType;
            string condChap = "";

            var nextLesson = _lessonService.CreateQuery().Find(t => t.ChapterID == lesson.ChapterID && t.Order > lesson.Order).SortBy(t => t.Order).FirstOrDefault();
            ViewBag.Class = currentClass;
            ViewBag.Subject = currentClassSubject;
            ViewBag.NextLesson = nextLesson;
            ViewBag.Chapter = chapter;
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

        [HttpPost]
        public JsonResult Join(string ID, string JoinLesson)
        {
            try
            {
                var rootItem = _lessonService.GetItemByID(ID);
                var joinItem = _lessonService.GetItemByID(JoinLesson);
                if (rootItem == null || joinItem == null)
                {
                    return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", null },
                        { "Error", "Dữ liệu không đúng" }
                    });
                }
                var currentIndex = _lessonPartService.CreateQuery().CountDocuments(o => o.ParentID == rootItem.ID);
                var joinParts = _lessonPartService.CreateQuery().Find(o => o.ParentID == joinItem.ID).SortBy(o => o.Order).ToList();

                if (joinParts != null && joinParts.Count > 0)
                {
                    foreach (var part in joinParts)
                    {
                        part.ParentID = rootItem.ID;
                        part.Order = (int)currentIndex++;
                        _lessonPartService.CreateQuery().ReplaceOne(o => o.ID == part.ID, part);
                    }
                }

                ChangeLessonPosition(joinItem, int.MaxValue);//chuyển lesson xuống cuối của đối tượng chứa
                _lessonService.Remove(joinItem.ID);

                return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", joinItem },
                        { "Error", null }
                    });
            }
            catch (Exception ex)
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", null },
                    { "Error", ex.Message }
                });
            }
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetSchedules(DefaultModel model)
        {
            TeacherEntity teacher = null;
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            if (!string.IsNullOrEmpty(UserID) && UserID != "0")
            {
                teacher = UserID == "0" ? null : _teacherService.GetItemByID(UserID);
                if (teacher == null)
                {
                    return new JsonResult(new Dictionary<string, object> {
                        {"Error", "Không có thông tin giảng viên" }
                    });
                }
            }
            if (string.IsNullOrEmpty(model.ID))
            {
                return new JsonResult(new Dictionary<string, object> {
                        {"Error","Không có thông tin lớp học" },
                });
            }

            var currentClass = _classService.GetItemByID(model.ID);

            if (currentClass == null)
            {
                return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Không có thông tin lớp học" }
                    });
            }

            var course = _courseService.GetItemByID(currentClass.CourseID);

            if (course == null)
            {
                return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Không có thông tin giáo trình" }
                    });
            }
            var _scheduleMapping = new MappingEntity<LessonEntity, LessonScheduleViewModel>();
            var classSchedule = new ClassScheduleViewModel(course)
            {
                Chapters = _chapterService.CreateQuery().Find(o => o.CourseID == course.ID).SortBy(o => o.ParentID).ThenBy(o => o.Order).ThenBy(o => o.ID).ToList(),
                Lessons = (from r in _lessonService.CreateQuery().Find(o => o.CourseID == course.ID).SortBy(o => o.ChapterID).ThenBy(o => o.Order).ThenBy(o => o.ID).ToList()
                           let schedule = _lessonScheduleService.CreateQuery().Find(o => o.LessonID == r.ID && o.ClassID == model.ID).FirstOrDefault()
                           where schedule != null
                           select _scheduleMapping.AutoOrtherType(r, new LessonScheduleViewModel()
                           {
                               ScheduleID = schedule.ID,
                               StartDate = schedule.StartDate,
                               EndDate = schedule.EndDate,
                               IsActive = schedule.IsActive
                           })).ToList()
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

            var currentClass = _classService.GetItemByID(model.ID);
            if (currentClass == null)
            {
                return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Không có thông tin lớp học" }
                    });
            }

            var course = _courseService.GetItemByID(currentClass.CourseID);

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
                Lessons = (from r in _lessonService.CreateQuery().Find(o => o.CourseID == course.ID && o.ChapterID == ChapterID).SortBy(o => o.ChapterID).ThenBy(o => o.Order).ThenBy(o => o.ID).ToList()
                           let schedule = _lessonScheduleService.CreateQuery().Find(o => o.LessonID == r.ID && o.ClassID == model.ID).FirstOrDefault()
                           select new MappingEntity<LessonEntity, LessonScheduleViewModel>().AutoOrtherType(r, new LessonScheduleViewModel()
                           {
                               ScheduleID = schedule.ID,
                               StartDate = schedule.StartDate,
                               EndDate = schedule.EndDate,
                               IsActive = schedule.IsActive,
                           })).ToList()
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
        public JsonResult UpdateChapterPoint(DefaultModel model, double BasePoint)
        {
            var UserID = User.Claims.GetClaimByType("UserID").Value;

            if (!(BasePoint >= 0 && BasePoint <= 100))
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Error", "Điểm tối thiểu không đúng (0 - 100)" },
                    { "Model", model }
                });
            }

            if (string.IsNullOrEmpty(model.ID))
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Error", "Thông tin không đúng" },
                    { "Model", model }
                });
            }

            var chapter = _chapterService.GetItemByID(model.ID);
            if (chapter == null)
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Error", "Thông tin không đúng" },
                    { "Model", model }
                });
            }
            chapter.BasePoint = BasePoint;
            _chapterService.Save(chapter);
            return new JsonResult(new Dictionary<string, object> {
                        {"Data", chapter },
                        {"Msg","Cập nhật thành công" }
                    });

        }

        [HttpPost]
        public JsonResult UpdateConditionChapter(DefaultModel model, string ConditionChapter)
        {
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            if (!string.IsNullOrEmpty(ConditionChapter))
            {
                var targetChap = _chapterService.GetItemByID(ConditionChapter);
                if (targetChap == null)
                {
                    return new JsonResult(new Dictionary<string, object>
                {
                    { "Error", "Thông tin không đúng" },
                    { "Model", model }
                });
                }
            }
            if (string.IsNullOrEmpty(model.ID))
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Error", "Thông tin không đúng" },
                    { "Model", model }
                });
            }

            var chapter = _chapterService.GetItemByID(model.ID);
            if (chapter == null)
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Error", "Thông tin không đúng" },
                    { "Model", model }
                });
            }
            UpdateConditionChapter(chapter, ConditionChapter);


            return new JsonResult(new Dictionary<string, object> {
                        {"Data", chapter },
                        {"Msg","Cập nhật thành công" }
                    });

        }

        //public JsonResult FixChapterPracticeCount()
        //{
        //    var chapters = _chapterService.CreateQuery().Find(t=> t.ParentID == "0").ToList();
        //    foreach(var chapter in chapters)
        //    {
        //        chapter.PracticeCount = _chapterService.CountChapterPractice(chapter.ID, chapter.ClassSubjectID);
        //    }
        //    return new JsonResult("OK");
        //}

        [HttpPost]
        public JsonResult CreateOrUpdate(LessonEntity item)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var data = _lessonService.GetItemByID(item.ID);
                if (data == null)
                {
                    item.Created = DateTime.Now;
                    item.CreateUser = UserID;
                    item.IsAdmin = true;
                    item.IsActive = false;
                    item.IsParentCourse = item.ChapterID.Equals("0");
                    item.Updated = DateTime.Now;
                    item.Order = 0;
                    var maxItem = new LessonEntity();
                    if (item.IsParentCourse)
                        maxItem = _lessonService.CreateQuery().Find(o => o.CourseID == item.CourseID && o.IsParentCourse).SortByDescending(o => o.Order).FirstOrDefault();
                    else
                        maxItem = _lessonService.CreateQuery().Find(o => o.ChapterID == item.ChapterID).SortByDescending(o => o.Order).FirstOrDefault();
                    if (maxItem != null)
                    {
                        item.Order = maxItem.Order + 1;
                    }
                    _lessonService.CreateQuery().InsertOne(item);
                    //update total lesson to parent chapter
                    if (!string.IsNullOrEmpty(item.ChapterID) && item.ChapterID != "0")
                        _ = _chapterService.IncreaseLessonCounter(item.ChapterID, 1, item.TemplateType == LESSON_TEMPLATE.EXAM ? 1 : 0, 0);
                    else
                        _ = _courseService.IncreaseLessonCounter(item.CourseID, 1, item.TemplateType == LESSON_TEMPLATE.EXAM ? 1 : 0, 0);
                }
                else
                {
                    item.Updated = DateTime.Now;
                    var newOrder = item.Order - 1;
                    item.Order = data.Order;
                    _lessonService.CreateQuery().ReplaceOne(o => o.ID == item.ID, item);

                    if (item.Order != newOrder)//change Position
                    {
                        ChangeLessonPosition(item, newOrder);
                    }
                }

                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", item },
                    {"Error",null }
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", null },
                    {"Error",ex.Message }
                });
            }
        }

        public IActionResult Exam()
        {
            ViewBag.RoleCode = User.Claims.GetClaimByType(ClaimTypes.Role).Value;
            return View();
        }

        [HttpPost]
        public JsonResult ChangePosition(string ID, int pos)
        {
            try
            {

                var item = _lessonService.GetItemByID(ID);
                if (item == null)
                    return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", null },
                                {"Error", "Item Not Found" }
                            });

                var newPos = ChangeLessonPosition(item, pos);
                return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", null },
                                {"Error", "Item Not Found" }
                            });
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

        [HttpPost]
        public JsonResult Remove(DefaultModel model, string ID)
        {
            try
            {
                if (!String.IsNullOrEmpty(model.ArrID))
                    ID = model.ArrID;
                var lesson = _lessonService.GetItemByID(ID);//TODO: check permission
                if (lesson != null)
                {
                    var lessonparts = _lessonPartService.CreateQuery().Find(o => o.ParentID == ID).ToList();
                    if (lessonparts != null && lessonparts.Count > 0)
                        for (int i = 0; lessonparts != null && i < lessonparts.Count; i++)
                            RemoveLessonPart(lessonparts[i].ID);

                    ChangeLessonPosition(lesson, int.MaxValue);//chuyển lesson xuống cuối của đối tượng chứa

                    _ = _chapterService.IncreaseLessonCounter(lesson.ChapterID, -1, lesson.TemplateType == LESSON_TEMPLATE.EXAM ? -1 : 0, lesson.IsPractice ? -1 : 0);
                    _lessonService.Remove(ID);
                    return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", "Remove OK" },
                                {"Error", null }
                            });
                }
                else
                {
                    return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", null },
                                {"Error", "Item Not Found" }
                            });
                }


            }
            catch (Exception ex)
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", null },
                    {"Error", ex.Message}
                });
            }
        }

        private void RemoveLessonPart(string ID)
        {
            try
            {
                var item = _lessonPartService.CreateQuery().Find(o => o.ID == ID).SingleOrDefault();
                if (item == null) return;

                var questions = _questionService.CreateQuery().Find(o => o.ParentID == ID).ToList();
                for (int i = 0; questions != null && i < questions.Count; i++)
                    RemoveQuestion(questions[i].ID);
                _lessonPartService.Remove(ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void RemoveQuestion(string ID)
        {
            try
            {
                var item = _questionService.CreateQuery().Find(o => o.ID == ID).SingleOrDefault();
                if (item == null) return;
                _answerService.CreateQuery().DeleteMany(o => o.ParentID == ID);
                _questionService.Remove(ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void RemoveAnswer(string ID)
        {
            try
            {
                var item = _answerService.CreateQuery().Find(o => o.ID == ID).SingleOrDefault();
                if (item == null) return;
                _answerService.Remove(ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int ChangeLessonPosition(LessonEntity item, int pos)
        {
            var parts = new List<LessonEntity>();
            parts = item.IsParentCourse
                ? _lessonService.CreateQuery().Find(o => o.CourseID == item.CourseID && o.IsParentCourse == true)
                .SortBy(o => o.Order).ThenBy(o => o.ID).ToList()
                : _lessonService.CreateQuery().Find(o => o.ChapterID == item.ChapterID)
                .SortBy(o => o.Order).ThenBy(o => o.ID).ToList();

            var ids = parts.Select(o => o.ID).ToList();

            var oldPos = ids.IndexOf(item.ID);
            if (oldPos == pos)
                return oldPos;

            if (pos > parts.Count())
                pos = parts.Count() - 1;
            item.Order = pos;

            _lessonService.CreateQuery().ReplaceOne(o => o.ID == item.ID, item);
            int entry = -1;
            foreach (var part in parts)
            {
                if (part.ID == item.ID) continue;
                if (entry == pos - 1)
                    entry++;
                entry++;
                part.Order = entry;
                _lessonService.CreateQuery().ReplaceOne(o => o.ID == part.ID, part);
            }
            return pos;
        }

        private void UpdateConditionChapter(ChapterEntity chapter, string ConditionChapter)
        {
            chapter.ConditionChapter = ConditionChapter;
            _chapterService.Save(chapter);
            var subchaps = _chapterService.GetSubChapters(chapter.ClassSubjectID, chapter.ID);
            if (subchaps != null && subchaps.Count > 0)
                subchaps.ForEach((ChapterEntity item) => UpdateConditionChapter(item, ConditionChapter));
        }


        #region Preview

        [HttpPost]
        public JsonResult GetTemplateExam(string ClassSubjectID, string LessonID)
        {
            var userID = User.Claims.GetClaimByType("UserID").Value;
            var lesson = _lessonService.GetItemByID(LessonID);
            if (lesson == null)
                return new JsonResult(new { Error = "Bài học không đúng" });
            var x = new ExamEntity
            {
                ClassID = lesson.ClassID,
                ClassSubjectID = lesson.ClassSubjectID,
                LessonID = lesson.ID,
                ID = "",
                Created = DateTime.Now,
                CurrentDoTime = DateTime.Now,
                LessonScheduleID = "",
                Point = lesson.Point,
                Timer = lesson.Timer
            };
            //hết hạn => đóng luôn
            var schedule = new LessonScheduleEntity();
            return new JsonResult(new { exam = x, schedule, limit = lesson.Limit });
        }

        [HttpPost]
        public JsonResult CreateTemplateExam(ExamEntity item)
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
                       { "Error", "Thông tin không đúng" }
                    });
                }

                var _schedule = _lessonScheduleService.CreateQuery().Find(o => o.LessonID == _lesson.ID && o.ClassID == _class.ID).FirstOrDefault();
                if (_schedule == null)
                {
                    return new JsonResult(new Dictionary<string, object>
                    {
                       { "Error", "Thông tin không đúng" }
                    });
                }

                item.StudentID = userid;
                item.Number = 1;

                item.LessonScheduleID = _schedule.ID;
                item.Timer = _lesson.Timer;
                item.Point = 0;
                item.MaxPoint = _lesson.Point;

                item.TeacherID = _class.TeacherID;
                item.ID = "TEMPLATE";
                item.Created = DateTime.Now;
                item.CurrentDoTime = DateTime.Now;
                item.Status = false;
                item.QuestionsTotal = _clonequestionService.CreateQuery().CountDocuments(o => o.LessonID == item.LessonID);
                item.QuestionsDone = 0;
                item.Marked = false;
            }
            item.Updated = DateTime.Now;
            return new JsonResult(new Dictionary<string, object>
                    {
                       { "Data", item }
                    });
        }

        public JsonResult GetTemplateLesson(string LessonID, string ClassID, string ClassSubjectID)
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

            var listParts = _clonepartService.CreateQuery().Find(o => o.ParentID == lesson.ID && o.ClassSubjectID == currentcs.ID).ToList();

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
                    case "QUIZ4":
                    case "QUIZ3":
                    case "ESSAY":
                        convertedPart.Questions = _clonequestionService.CreateQuery()
                            .Find(q => q.ParentID == part.ID).SortBy(q => q.Order).ThenBy(q => q.ID).ToList()
                            .Select(q => new QuestionViewModel(q)
                            {
                                CloneAnswers = _cloneanswerService.CreateQuery().Find(x => x.ParentID == q.ID).ToList(),
                                Description = q.Description
                            }).ToList();
                        break;
                    case "QUIZ2":
                        convertedPart.Questions = _clonequestionService.CreateQuery().Find(q => q.ParentID == part.ID)
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
                //listParts.Select(o => mapPart.AutoOrtherType(o, new PartViewModel()
                //{
                //    Questions = _cloneLessonPartQuestionService.CreateQuery().Find(x => x.ParentID == o.ID).ToList()
                //        .Select(z => mapQuestion.AutoOrtherType(z, new QuestionViewModel()
                //        {
                //            CloneAnswers = o.Type == "QUIZ2" ? null : _cloneLessonPartAnswerService.CreateQuery().Find(x => x.ParentID == z.ID).ToList(),
                //            Description = o.Type == "QUIZ2" ? null : z.Description
                //        }))?.ToList()
                //})).ToList()
            });

            return new JsonResult(new Dictionary<string, object> { { "Data", dataResponse } });

        }

        private string RenderVocab(string description)
        {
            string result = "";
            var vocabs = description.Split('|');
            if (vocabs == null || vocabs.Count() == 0)
                return description;
            foreach (var vocab in vocabs)
            {
                var vocabularies = _vocabularyService.GetItemByCode(vocab.Trim().Replace("-", ""));
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
        #endregion
    }
}
