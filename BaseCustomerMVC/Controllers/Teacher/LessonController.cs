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
            LessonScheduleService lessonScheduleService)
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
        }

        public IActionResult Detail(DefaultModel model, string ClassID, int frameview = 0)
        {
            ViewBag.RoleCode = User.Claims.GetClaimByType(ClaimTypes.Role).Value;
            if (model == null) return null;
            if (ClassID == null)
                return RedirectToAction("Index", "Class");
            var currentClassSubject = _classSubjectService.GetItemByID(ClassID);
            if (currentClassSubject == null)
                return RedirectToAction("Index", "Class");
            var currentClass = _classService.GetItemByID(currentClassSubject.ClassID);
            if (currentClass == null)
                return RedirectToAction("Index", "Class");
            var Data = _lessonService.GetItemByID(model.ID);
            if (Data == null)
                return RedirectToAction("Index", "Class");
            ViewBag.Class = currentClass;
            ViewBag.Subject = currentClassSubject;
            ViewBag.Lesson = Data;
            ViewBag.Title = Data.Title;

            if (frameview == 1)
                return View("FrameDetails");
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
                        _ = _chapterService.IncreaseLessonCount(item.ChapterID, 1);
                    else
                        _ = _courseService.IncreaseLessonCount(item.CourseID, 1);
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
                    _lessonService.Remove(ID);
                    _ = _chapterService.IncreaseLessonCount(lesson.ChapterID, -1);
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

    }
}
