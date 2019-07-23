using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using MongoDB.Driver;
using System.Text;
using System.Linq;
using Core_v2.Globals;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class LessonScheduleController : TeacherController
    {
        //private readonly GradeService _gradeService;
        //private readonly SubjectService _subjectService;
        private readonly TeacherService _teacherService;
        private readonly ClassService _classService;
        private readonly CourseService _courseService;
        private readonly ChapterService _chapterService;
        private readonly LessonService _lessonService;
        private readonly LessonScheduleService _service;
        private readonly ExamService _examService;
        private readonly MappingEntity<LessonEntity, LessonScheduleViewModel> _mapping;

        public LessonScheduleController(
            // GradeService gradeservice
            //, SubjectService subjectService, 
            TeacherService teacherService,
            ClassService classService,
            CourseService courseService,
            ChapterService chapterService,
            LessonService lessonService,
            LessonScheduleService service,
            ExamService examService)
        {
            //_gradeService = gradeservice;
            //_subjectService = subjectService;
            _teacherService = teacherService;
            _courseService = courseService;
            _classService = classService;
            _chapterService = chapterService;
            _lessonService = lessonService;
            _service = service;
            _examService = examService;
            _mapping = new MappingEntity<LessonEntity, LessonScheduleViewModel>();
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetList(DefaultModel model, string ClassID = "", string UserID = "")
        {
            TeacherEntity teacher = null;
            if (string.IsNullOrEmpty("UserID"))
                UserID = User.Claims.GetClaimByType("UserID").Value;
            if (!string.IsNullOrEmpty(UserID) && UserID != "0")
            {
                teacher = UserID == "0" ? null : _teacherService.GetItemByID(UserID);
                if (teacher == null)
                {
                    return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Không có thông tin giảng viên" }
                    });
                }
            }
            if (string.IsNullOrEmpty(ClassID))
            {
                return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Không có thông tin lớp học" }
                    });
            }

            var currentClass = _classService.GetItemByID(ClassID);
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
                Chapters = _chapterService.CreateQuery().Find(o => o.CourseID == course.ID).SortBy(o => o.ParentID).ThenBy(o => o.Order).ThenBy(o => o.ID).ToList(),
                Lessons = (from r in _lessonService.CreateQuery().Find(o => o.CourseID == course.ID).SortBy(o => o.ChapterID).ThenBy(o => o.Order).ThenBy(o => o.ID).ToList()
                           let schedule = _service.CreateQuery().Find(o => o.LessonID == r.ID && o.ClassID == ClassID).FirstOrDefault()
                           where schedule != null
                           select _mapping.AutoOrtherType(r, new LessonScheduleViewModel()
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
        public JsonResult GetExamList(DefaultModel model, string ClassID = "")
        {
            TeacherEntity teacher = null;
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            if (!string.IsNullOrEmpty(UserID) && UserID != "0")
            {
                teacher = UserID == "0" ? null : _teacherService.GetItemByID(UserID);
                if (teacher == null)
                {
                    return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Không có thông tin giảng viên" }
                    });
                }
            }
            if (string.IsNullOrEmpty(ClassID))
            {
                return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Không có thông tin lớp học" }
                    });
            }

            var currentClass = _classService.GetItemByID(ClassID);
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

            var examlist = new ClassScheduleViewModel(course)
            {
                Lessons = (from r in _lessonService.CreateQuery().Find(o => o.CourseID == course.ID && o.TemplateType == LESSON_TEMPLATE.EXAM).SortBy(o => o.ChapterID).ThenBy(o => o.Order).ThenBy(o => o.ID).ToList()
                           let schedule = _service.CreateQuery().Find(o => o.LessonID == r.ID && o.ClassID == ClassID).FirstOrDefault()
                           where schedule != null
                           select _mapping.AutoOrtherType(r, new LessonScheduleViewModel()
                           {
                               ScheduleID = schedule.ID,
                               StartDate = schedule.StartDate,
                               EndDate = schedule.EndDate,
                               StudentJoins = _examService.CreateQuery().Count(o => o.LessonScheduleID == schedule.ID)
                           })).ToList()
            };

            var response = new Dictionary<string, object>
            {
                { "Data", examlist },
                { "Model", model }
            };
            return new JsonResult(response);
        }


        [HttpPost]
        [Obsolete]
        public JsonResult Publish(DefaultModel model)
        {

            if (model.ArrID.Length <= 0)
            {
                return new JsonResult(null);
            }
            else
            {
                if (model.ArrID.Contains(","))
                {
                    var filter = Builders<LessonScheduleEntity>.Filter.Where(o => model.ArrID.Split(',').Contains(o.ID) && o.IsActive != true);
                    var update = Builders<LessonScheduleEntity>.Update.SetOnInsert("IsActive", true);
                    var publish = _service.Collection.UpdateMany(filter, update, new UpdateOptions() { IsUpsert = true });
                    return new JsonResult(publish);
                }
                else
                {
                    var filter = Builders<LessonScheduleEntity>.Filter.Where(o => model.ArrID == o.ID && o.IsActive != true);
                    var update = Builders<LessonScheduleEntity>.Update.Set("IsActive", true);
                    var publish = _service.Collection.UpdateMany(filter, update, new UpdateOptions() { IsUpsert = true });
                    return new JsonResult(publish);
                }

            }
        }

        [HttpPost]
        [Obsolete]
        public JsonResult UnPublish(DefaultModel model)
        {
            if (model.ArrID.Length <= 0)
            {
                return new JsonResult(null);
            }
            else
            {
                if (model.ArrID.Contains(","))
                {
                    var filter = Builders<LessonScheduleEntity>.Filter.Where(o => model.ArrID.Split(',').Contains(o.ID) && o.IsActive == true);
                    var update = Builders<LessonScheduleEntity>.Update.Set("IsActive", false);
                    var publish = _service.Collection.UpdateMany(filter, update);
                    return new JsonResult(publish);
                }
                else
                {
                    var filter = Builders<LessonScheduleEntity>.Filter.Where(o => model.ArrID == o.ID && o.IsActive == true);
                    var update = Builders<LessonScheduleEntity>.Update.Set("IsActive", false);
                    var publish = _service.Collection.UpdateMany(filter, update);
                    return new JsonResult(publish);
                }


            }
        }

        [HttpPost]
        [Obsolete]
        public JsonResult UpdateSchedule(LessonScheduleEntity entity)
        {
            if (entity == null || string.IsNullOrEmpty(entity.ID))
            {
                return new JsonResult(null);
            }
            else
            {
                var oldItem = _service.GetItemByID(entity.ID);
                if (oldItem == null)
                    return new JsonResult(null);

                oldItem.StartDate = entity.StartDate;
                oldItem.EndDate = entity.EndDate;

                _service.CreateQuery().ReplaceOne(o => o.ID == oldItem.ID, oldItem);
                return new JsonResult(oldItem);
            }
        }


    }
}
