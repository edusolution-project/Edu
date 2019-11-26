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
    [BaseAccess.Attribule.AccessCtrl("Lesson schedule")]
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
        private readonly SubjectService _subjectService;
        private readonly ExamService _examService;
        private readonly MappingEntity<LessonEntity, LessonScheduleViewModel> _mapping;
        private readonly CalendarHelper _calendarHelper;

        public LessonScheduleController(
            // GradeService gradeservice
            //, SubjectService subjectService, 
            TeacherService teacherService,
            ClassService classService,
            CourseService courseService,
            ChapterService chapterService,
            LessonService lessonService,
            LessonScheduleService service,
            SubjectService subjectService,
            ExamService examService,
            CalendarHelper calendarHelper
            )
        {
            //_gradeService = gradeservice;
            //_subjectService = subjectService;
            _teacherService = teacherService;
            _courseService = courseService;
            _classService = classService;
            _chapterService = chapterService;
            _lessonService = lessonService;
            _subjectService = subjectService;
            _service = service;
            _examService = examService;
            _calendarHelper = calendarHelper;
            _mapping = new MappingEntity<LessonEntity, LessonScheduleViewModel>();
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetList(DefaultModel model, string ClassID = "", string UserID = "")
        {
            TeacherEntity teacher = null;
            if (string.IsNullOrEmpty(UserID))
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
        public JsonResult GetAssignments(DefaultModel model, string ClassID = "", string UserID = "")
        {
            TeacherEntity teacher = null;
            if (string.IsNullOrEmpty(UserID))
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
                Lessons = (from r in _lessonService.CreateQuery().Find(o => o.CourseID == course.ID && o.Etype > 0).SortBy(o => o.ChapterID).ThenBy(o => o.Order).ThenBy(o => o.ID).ToList()
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
        public JsonResult GetActiveList(DefaultModel model, string ClassID = "", string UserID = "", string SubjectID = "")
        {
            TeacherEntity teacher = null;
            if (string.IsNullOrEmpty(UserID))
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

            var subjects = _subjectService.GetAll().ToList();

            var classFilter = new List<FilterDefinition<ClassEntity>>();
            classFilter.Add(Builders<ClassEntity>.Filter.Where(o => o.TeacherID == UserID));

            if (!string.IsNullOrEmpty(SubjectID))
            {
                classFilter.Add(Builders<ClassEntity>.Filter.Where(o => o.SubjectID == SubjectID));
            }

            var activeClass = _classService.CreateQuery().Find(Builders<ClassEntity>.Filter.And(classFilter)).ToList();
            var activeClassIDs = activeClass.Select(t => t.ID).ToList();

            var data = (from r in _service.CreateQuery().Find(o => activeClassIDs.Contains(o.ClassID) && o.IsActive && o.EndDate >= model.StartDate && o.StartDate <= model.EndDate).ToList()
                        let currentClass = activeClass.SingleOrDefault(o => o.ID == r.ClassID)
                        let subject = subjects.SingleOrDefault(s => s.ID == currentClass.SubjectID)
                        select _mapping.AutoOrtherType(_lessonService.GetItemByID(r.LessonID), new LessonScheduleViewModel()
                        {
                            ScheduleID = r.ID,
                            StartDate = r.StartDate,
                            EndDate = r.EndDate,
                            IsActive = r.IsActive,
                            ClassID = currentClass.ID,
                            SubjectName = subject.Name,
                            ClassName = currentClass.Name
                        }));
            model.TotalRecord = data.Count();
            var returnData = data == null || data.Count() <= 0 || data.Count() < model.PageSize || model.PageSize <= 0
                ? data.ToList()
                : data.Skip((model.PageIndex - 1) * model.PageSize).Take(model.PageSize).ToList();
            var response = new Dictionary<string, object>
            {
                { "Data", returnData },
                { "Model", model }
            };
            return new JsonResult(response);
        }


        [Obsolete]
        [HttpPost]
        public JsonResult GetTodayLessons(DefaultModel model, DateTime date, string UserID = "")
        {
            TeacherEntity teacher = null;
            if (string.IsNullOrEmpty(UserID))
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

            var subjects = _subjectService.GetAll().ToList();

            var classFilter = new List<FilterDefinition<ClassEntity>>();
            classFilter.Add(Builders<ClassEntity>.Filter.Where(o => o.TeacherID == UserID));

            var activeClass = _classService.CreateQuery().Find(Builders<ClassEntity>.Filter.And(classFilter)).SortBy(o => o.SubjectID).ToList();
            var activeClassIDs = activeClass.Select(t => t.ID).ToList();

            var data = new List<LessonScheduleViewModel>();

            var startdate = date.ToLocalTime().Date;
            var enddate = date.AddDays(1);
            foreach (var _class in activeClass)
            {
                var schedule = _service.CreateQuery().Find(o => o.ClassID == _class.ID && o.IsActive && o.EndDate >= startdate && o.StartDate <= enddate).FirstOrDefault();
                if (schedule != null)
                {
                    var lesson = _lessonService.GetItemByID(schedule.LessonID);
                    if (lesson != null)
                    {
                        var subject = subjects.SingleOrDefault(o => o.ID == _class.SubjectID);
                        data.Add(_mapping.AutoOrtherType(lesson, new LessonScheduleViewModel()
                        {
                            ScheduleID = schedule.ID,
                            StartDate = schedule.StartDate,
                            EndDate = schedule.EndDate,
                            IsActive = schedule.IsActive,
                            ClassID = _class.ID,
                            SubjectName = subject.Name,
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
                           let students = //_examService.Collection.Aggregate().Match(o => o.LessonScheduleID == schedule.ID).Group(o => o.StudentID, g => new { Result = 1 }).ToList()
                           _examService.CreateQuery().Distinct(t => t.StudentID, s => s.LessonScheduleID == schedule.ID).ToList()
                           select _mapping.AutoOrtherType(r, new LessonScheduleViewModel()
                           {
                               ScheduleID = schedule.ID,
                               StartDate = schedule.StartDate,
                               EndDate = schedule.EndDate,
                               StudentJoins = students.Count()
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
        public JsonResult Update(LessonScheduleEntity entity)
        {
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            if (entity == null || string.IsNullOrEmpty(entity.ID))
            {
                return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error", "Không tìm thấy lịch học" }
                    });
            }
            else
            {
                var oldItem = _service.GetItemByID(entity.ID);
                if (oldItem == null)
                    return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error", "Không tìm thấy lịch học" }
                    });

                oldItem.StartDate = entity.StartDate;
                oldItem.EndDate = entity.EndDate;
                UpdateCalendar(oldItem, UserID);
                _service.CreateOrUpdate(oldItem);

                return new JsonResult(new Dictionary<string, object> {
                        {"Data",oldItem },
                        {"Error", null }
                    });
            }
        }

        private void UpdateCalendar(LessonScheduleEntity entity, string userid)
        {
            var oldcalendar = _calendarHelper.GetByScheduleId(entity.ID);
            if (oldcalendar != null)
                _calendarHelper.Remove(oldcalendar.ID);
            _calendarHelper.ConvertCalendarFromSchedule(entity, userid);
        }

        [Obsolete]
        public JsonResult CreateCalendar()
        {
            _calendarHelper.ScheduleAutoConvertEvent();
            return new JsonResult("OK");
        }
    }
}
