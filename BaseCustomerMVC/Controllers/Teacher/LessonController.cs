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

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class LessonController : TeacherController
    {
        private readonly GradeService _gradeService;
        private readonly SubjectService _subjectService;
        private readonly TeacherService _teacherService;
        private readonly ClassService _classService;
        private readonly CourseService _courseService;
        private readonly ChapterService _chapterService;
        private readonly LessonService _lessonService;
        private readonly LessonPartService _lessonPartService;
        private readonly LessonPartQuestionService _questionService;
        private readonly LessonPartAnswerService _answerService;
        private readonly LessonScheduleService _lessonScheduleService;

        public LessonController(
            GradeService gradeservice,
            SubjectService subjectService,
            TeacherService teacherService,
            ClassService classService,
            CourseService courseService,
            ChapterService chapterService,
            LessonService lessonService,
            LessonPartService lessonPartService,
            LessonPartQuestionService questionService,
            LessonPartAnswerService answerService,
            LessonScheduleService lessonScheduleService)
        {
            _gradeService = gradeservice;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _courseService = courseService;
            _classService = classService;
            _chapterService = chapterService;
            _lessonService = lessonService;
            _lessonPartService = lessonPartService;
            _questionService = questionService;
            _answerService = answerService;
            _lessonScheduleService = lessonScheduleService;
        }

        public IActionResult Detail(DefaultModel model, string ClassID)
        {
            if (model == null) return null;

            if (ClassID == null)
                return RedirectToAction("Index", "Class");
            var currentClass = _classService.GetItemByID(ClassID);
            if (currentClass == null)
                return RedirectToAction("Index", "Class");
            var Data = _lessonService.GetItemByID(model.ID);
            if (Data == null)
                return RedirectToAction("Index", "Class");
            ViewBag.Class = currentClass;
            ViewBag.Data = Data;
            if (Data.TemplateType == LESSON_TEMPLATE.LECTURE)
                return View();
            else
                return View("Exam");
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

        [HttpPost]
        public JsonResult CreateOrUpdate(LessonEntity item)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var data = _lessonService.CreateQuery().Find(o => o.ID == item.ID).SingleOrDefault();
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
                }
                else
                {
                    item.Updated = DateTime.Now;
                    item.Order = data.Order;
                    _lessonService.CreateQuery().ReplaceOne(o => o.ID == item.ID, item);
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
                ? _lessonService.CreateQuery().Find(o => o.CourseID == item.ID && o.IsParentCourse == true)
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

    }
}
