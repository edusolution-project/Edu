using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Core_v2.Interfaces;
using Core_v2.Globals;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseCustomerMVC.Controllers.Student
{
    public class LessonController : StudentController
    {
        private readonly SubjectService _subjectService;
        private readonly CourseService _courseService;
        private readonly ClassService _classService;
        private readonly ChapterService _chapterService;
        private readonly LessonScheduleService _lessonScheduleService;

        private readonly LessonService _lessonService;
        private readonly LessonPartService _lessonPartService;
        private readonly LessonPartQuestionService _lessonPartQuestionService;
        private readonly LessonPartAnswerService _lessonPartAnswerService;

        private readonly ExamService _examService;
        private readonly ExamDetailService _examDetailService;

        private readonly LearningHistoryService _learningHistoryService;

        private readonly CloneLessonPartService _cloneLessonPartService;
        private readonly CloneLessonPartAnswerService _cloneLessonPartAnswerService;
        private readonly CloneLessonPartQuestionService _cloneLessonPartQuestionService;

        private readonly MappingEntity<LessonEntity, LessonScheduleViewModel> _schedulemapping;
        private readonly MappingEntity<LessonPartEntity, CloneLessonPartEntity> _lessonPartMapping;
        private readonly MappingEntity<LessonPartQuestionEntity, CloneLessonPartQuestionEntity> _lessonPartQuestionMapping;
        private readonly MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity> _lessonPartAnswerMapping;

        public LessonController(
            SubjectService subjectService
            , CourseService courseService
            , ClassService classService
            , ChapterService chapterService
            , LessonScheduleService lessonScheduleService
            , LearningHistoryService learningHistoryService

            , LessonService lessonService
            , LessonPartService lessonPartService
            , LessonPartQuestionService lessonPartQuestionService
            , LessonPartAnswerService lessonPartAnswerService

            , CloneLessonPartService cloneLessonPartService
            , CloneLessonPartAnswerService cloneLessonPartAnswerService
            , CloneLessonPartQuestionService cloneLessonPartQuestionService

            )
        {
            _subjectService = subjectService;
            _courseService = courseService;
            _classService = classService;
            _chapterService = chapterService;
            _lessonScheduleService = lessonScheduleService;
            _learningHistoryService = learningHistoryService;

            _lessonService = lessonService;
            _lessonPartService = lessonPartService;

            _schedulemapping = new MappingEntity<LessonEntity, LessonScheduleViewModel>();
        }

        public IActionResult Index()
        {
            var userid = User.Claims.GetClaimByType("UserID").Value;
            ViewBag.User = userid;

            var subjectids = _classService.CreateQuery().Find(o => o.Students.Contains(userid)).ToList().Select(x => x.SubjectID).ToList();
            var subject = _subjectService.CreateQuery().Find(t => subjectids.Contains(t.ID)).ToList();

            ViewBag.Subject = subject;

            return View();
        }

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

        [Obsolete]
        [HttpPost]
        public JsonResult GetActiveList(DefaultModel model, string ClassID = "", string UserID = "", string SubjectID = "")
        {
            if (string.IsNullOrEmpty(UserID))
                UserID = User.Claims.GetClaimByType("UserID").Value;

            var subjects = _subjectService.GetAll().ToList();

            var classFilter = new List<FilterDefinition<ClassEntity>>();
            classFilter.Add(Builders<ClassEntity>.Filter.Where(o => o.Students.Contains(UserID)));

            if (!string.IsNullOrEmpty(SubjectID))
            {
                classFilter.Add(Builders<ClassEntity>.Filter.Where(o => o.SubjectID == SubjectID));
            }

            var activeClass = _classService.CreateQuery().Find(Builders<ClassEntity>.Filter.And(classFilter)).ToList();
            var activeClassIDs = activeClass.Select(t => t.ID).ToList();

            var data = (from r in _lessonScheduleService.CreateQuery().Find(o => activeClassIDs.Contains(o.ClassID) && o.IsActive && o.EndDate >= model.StartDate && o.StartDate <= model.EndDate).ToList()
                        let currentClass = activeClass.SingleOrDefault(o => o.ID == r.ClassID)
                        let subject = subjects.SingleOrDefault(s => s.ID == currentClass.SubjectID)
                        select _schedulemapping.AutoOrtherType(_lessonService.GetItemByID(r.LessonID), new LessonScheduleViewModel()
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

            var subjects = _subjectService.GetAll().ToList();

            var classFilter = new List<FilterDefinition<ClassEntity>>();
            classFilter.Add(Builders<ClassEntity>.Filter.Where(o => o.Students.Contains(UserID)));

            var activeClass = _classService.CreateQuery().Find(Builders<ClassEntity>.Filter.And(classFilter)).SortBy(o => o.SubjectID).ToList();
            var activeClassIDs = activeClass.Select(t => t.ID).ToList();

            var data = new List<LessonScheduleViewModel>();

            var startdate = date.ToLocalTime().Date;
            var enddate = date.AddDays(1);
            foreach (var _class in activeClass)
            {
                var schedule = _lessonScheduleService.CreateQuery().Find(o => o.ClassID == _class.ID && o.IsActive && o.EndDate >= startdate && o.StartDate <= enddate).FirstOrDefault();
                if (schedule != null)
                {
                    var lesson = _lessonService.GetItemByID(schedule.LessonID);
                    if (lesson != null)
                    {
                        var subject = subjects.SingleOrDefault(o => o.ID == _class.SubjectID);
                        data.Add(_schedulemapping.AutoOrtherType(lesson, new LessonScheduleViewModel()
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id">LessonID</param>
        /// <param name="ClassID">ClassID</param>
        /// <returns></returns>
        public IActionResult Detail(DefaultModel model, string id, string ClassID)
        {
            if (ClassID == null)
                return RedirectToAction("Index", "Class");
            var currentClass = _classService.GetItemByID(ClassID);
            if (currentClass == null)
                return RedirectToAction("Index", "Class");
            var lesson = _lessonService.GetItemByID(model.ID);
            if (lesson == null)
                return RedirectToAction("Index", "Class");
            ViewBag.Class = currentClass;
            ViewBag.Lesson = lesson;
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
        public JsonResult GetLesson(string LessonID, string ClassID)
        {
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userId))
                return new JsonResult(
                new Dictionary<string, object> { { "Error", "Student not found" } });

            var lesson = _lessonService.GetItemByID(LessonID);
            if (lesson == null)
                return new JsonResult(
                new Dictionary<string, object> { { "Error", "Lesson not found" } });

            var currentClass = _classService.GetItemByID(ClassID);
            if (currentClass == null)
                return new JsonResult(
                new Dictionary<string, object> { { "Error", "Class not found" } });

            var listParts = _cloneLessonPartService.CreateQuery().Find(o => o.ParentID == lesson.ID && o.ClassID == ClassID).ToList();

            //Create learning history
            _learningHistoryService.CreateHist(new LearningHistoryEntity()
            {
                ClassID = ClassID,
                LessonID = LessonID,
                Time = DateTime.Now,
                StudentID = userId
            });

            if (listParts == null || listParts.Count <= 0)
            {
                var listparts = _lessonPartService.CreateQuery().Find(o => o.ParentID == lesson.ID).ToList();
                if (listparts != null)
                {
                    //Ko map mà clone lesson, bao giờ giáo viên cũng sẽ clone trước
                    //Clone from lesson part - Temporary
                    var listLessonPart = _lessonPartService.CreateQuery().Find(o => o.ParentID == LessonID).SortBy(q => q.Order).ThenBy(q => q.ID).ToList();
                    if (listLessonPart != null && listLessonPart.Count > 0)
                    {
                        var _lessonPartMapping = new MappingEntity<LessonPartEntity, CloneLessonPartEntity>();
                        foreach (var lessonpart in listLessonPart)
                        {
                            var clonepart = _lessonPartMapping.AutoOrtherType(lessonpart, new CloneLessonPartEntity());
                            clonepart.ID = null;
                            clonepart.OriginID = lessonpart.ID;
                            clonepart.TeacherID = currentClass.TeacherID;
                            clonepart.ClassID = currentClass.ID;
                            CloneLessonPart(clonepart);
                        }

                        listParts = _cloneLessonPartService.CreateQuery().Find(o => o.ParentID == LessonID && o.ClassID == currentClass.ID).SortBy(q => q.Order).ThenBy(q => q.ID).ToList();
                    }
                }
            }

            var listData = listParts.ToList();

            var lastexam = _examService.CreateQuery().Find(o => o.LessonID == LessonID && o.ClassID == ClassID && o.StudentID == userId).SortByDescending(o => o.Created).FirstOrDefault();

            if (lastexam == null)
            {
                MappingEntity<LessonEntity, StudentLessonViewModel> mapping = new MappingEntity<LessonEntity, StudentLessonViewModel>();
                MappingEntity<CloneLessonPartEntity, PartViewModel> mapPart = new MappingEntity<CloneLessonPartEntity, PartViewModel>();
                MappingEntity<CloneLessonPartQuestionEntity, QuestionViewModel> mapQuestion = new MappingEntity<CloneLessonPartQuestionEntity, QuestionViewModel>();

                var dataResponse = mapping.AutoOrtherType(lesson, new StudentLessonViewModel()
                {
                    Parts = listData.Select(o => mapPart.AutoOrtherType(o, new PartViewModel()
                    {
                        Questions = _cloneLessonPartQuestionService.CreateQuery().Find(x => x.ParentID == o.ID).ToList()
                            .Select(z => mapQuestion.AutoOrtherType(z, new QuestionViewModel()
                            {
                                CloneAnswers = _cloneLessonPartAnswerService.CreateQuery().Find(x => x.ParentID == z.ID).ToList()
                            }))?.ToList()
                    })).ToList()
                });

                var respone = new Dictionary<string, object> { { "Data", dataResponse } };
                return new JsonResult(respone);
            }
            else //TODO: Double check here
            {
                //if (_examService.IsOverTime(lastexam.ID))
                //{
                MappingEntity<LessonEntity, StudentLessonViewModel> mapping = new MappingEntity<LessonEntity, StudentLessonViewModel>();
                MappingEntity<CloneLessonPartEntity, PartViewModel> mapPart = new MappingEntity<CloneLessonPartEntity, PartViewModel>();
                MappingEntity<CloneLessonPartQuestionEntity, QuestionViewModel> mapQuestion = new MappingEntity<CloneLessonPartQuestionEntity, QuestionViewModel>();

                var dataResponse = mapping.AutoOrtherType(lesson, new StudentLessonViewModel()
                {
                    Parts = listData.Select(o => mapPart.AutoOrtherType(o, new PartViewModel()
                    {
                        Questions = _cloneLessonPartQuestionService.CreateQuery().Find(x => x.ParentID == o.ID).ToList()
                            .Select(z => mapQuestion.AutoOrtherType(z, new QuestionViewModel()
                            {
                                CloneAnswers = _cloneLessonPartAnswerService.CreateQuery().Find(x => x.ParentID == z.ID).ToList()
                            }))?.ToList()
                    })).ToList()
                });


                var currentTimespan = new TimeSpan(0, 0, lesson.Timer, 0);

                if (!lastexam.Status) //bài kt cũ chưa xong => check thời gian làm bài
                {
                    var endtime = (lastexam.Created.AddMinutes(lastexam.Timer));
                    if (endtime < DateTime.UtcNow) // hết thời gian => kết thúc bài kt
                    {

                    }
                }




                var timeSpan = lastexam.Status ? new TimeSpan(0, 0, lesson.Timer, 0) : (lastexam.Created.AddMinutes(lastexam.Timer) - DateTime.UtcNow);



                return new JsonResult(
                    new Dictionary<string, object> {
                        { "Data", dataResponse },
                        { "Exam", lastexam },
                        { "Timer", (timeSpan.Minutes < 10 ? "0":"") +  timeSpan.Minutes + ":" + (timeSpan.Seconds < 10 ? "0":"") + timeSpan.Seconds }
                    });
                //}
                //else
                //{

                //    //var listED = _examDetailService.CreateQuery().Find(o => o.ExamID == exam.ID).ToList(); ?? WHAT IS THIS

                //    //Khong su dung currentDotime de check thoi gian, chi so sanh voi thoi gian bat dau lam bai: Created
                //    //var timere = exam.CurrentDoTime.AddMinutes(exam.Timer) - DateTime.UtcNow;
                //    var timere = lastexam.Created.AddMinutes(lastexam.Timer) - DateTime.UtcNow;
                //    // so sanh va dua ra dap an cua sinh vien ????

                //    MappingEntity<LessonEntity, StudentLessonViewModel> mapping = new MappingEntity<LessonEntity, StudentLessonViewModel>();
                //    MappingEntity<CloneLessonPartEntity, PartViewModel> mapPart = new MappingEntity<CloneLessonPartEntity, PartViewModel>();
                //    MappingEntity<CloneLessonPartQuestionEntity, QuestionViewModel> mapQuestion = new MappingEntity<CloneLessonPartQuestionEntity, QuestionViewModel>();
                //    var dataResponse = mapping.AutoOrtherType(lesson, new StudentLessonViewModel()
                //    {
                //        Parts = listData.Select(o => mapPart.AutoOrtherType(o, new PartViewModel()
                //        {
                //            Questions = _cloneLessonPartQuestionService.CreateQuery().Find(x => x.ParentID == o.ID).ToList()
                //                .Select(z => mapQuestion.AutoOrtherType(z, new QuestionViewModel()
                //                {
                //                    CloneAnswers = _cloneLessonPartAnswerService.CreateQuery().Find(x => x.ParentID == z.ID).ToList()
                //                }))?.ToList()
                //        })).ToList()
                //    });

                //    var respone = new Dictionary<string, object>
                //    {
                //        {
                //            "Data", dataResponse
                //        },
                //        {
                //             "Exam", lastexam
                //        },
                //        {
                //            "Timer", timere.Minutes < 10 ? "0"+  timere.Minutes + ":" + timere.Seconds : timere.Minutes + ":" + timere.Seconds
                //        }
                //    };
                //    return new JsonResult(respone);
                //}
            }
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

            var currentClass = _classService.GetItemByID(model.ID);
            if (currentClass == null || currentClass.Students.IndexOf(UserID) < 0)
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
                           let schedule = _lessonScheduleService.CreateQuery().Find(o => o.LessonID == r.ID && o.ClassID == model.ID).FirstOrDefault()
                           let lastjoin = _learningHistoryService.CreateQuery().Find(x => x.StudentID == UserID && x.LessonID == r.ID && x.ClassID == model.ID).SortByDescending(o => o.ID).FirstOrDefault()
                           select _schedulemapping.AutoOrtherType(r, new LessonScheduleViewModel()
                           {
                               ScheduleID = schedule.ID,
                               StartDate = schedule.StartDate,
                               EndDate = schedule.EndDate,
                               IsActive = schedule.IsActive,
                               IsView = lastjoin != null,
                               LastJoin = lastjoin != null ? lastjoin.Time : DateTime.MinValue
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
        public JsonResult GetAssignments(string ID)
        {
            var UserID = User.Claims.GetClaimByType("UserID").Value;

            if (string.IsNullOrEmpty(ID))
            {
                return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error", ID },
                        {"Msg","Không có thông tin lớp học" }
                    });
            }

            var currentClass = _classService.GetItemByID(ID);
            if (currentClass == null || currentClass.Students.IndexOf(UserID) < 0)
            {
                return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error", ID },
                        {"Msg","Không có thông tin lớp học" }
                    });
            }

            var course = _courseService.GetItemByID(currentClass.CourseID);

            if (course == null)
            {
                return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error", ID },
                        {"Msg","Không có thông tin giáo trình" }
                    });
            }

            var classSchedule = new ClassScheduleViewModel(course)
            {

                Lessons = (from r in _lessonService.CreateQuery().Find(o => o.CourseID == course.ID && o.Etype > 0).SortBy(o => o.ChapterID).ThenBy(o => o.Order).ThenBy(o => o.ID).ToList()
                           let schedule = _lessonScheduleService.CreateQuery().Find(o => o.LessonID == r.ID && o.ClassID == ID).FirstOrDefault()
                           let lastjoin = _learningHistoryService.CreateQuery().Find(x => x.StudentID == UserID && x.LessonID == r.ID && x.ClassID == ID).SortByDescending(o => o.ID).FirstOrDefault()
                           select _schedulemapping.AutoOrtherType(r, new LessonScheduleViewModel()
                           {
                               ScheduleID = schedule.ID,
                               StartDate = schedule.StartDate,
                               EndDate = schedule.EndDate,
                               IsActive = schedule.IsActive,
                               IsView = lastjoin != null,
                               LastJoin = lastjoin != null ? lastjoin.Time : DateTime.MinValue
                           })).ToList()
            };

            var response = new Dictionary<string, object>
            {
                { "Data", classSchedule },
                { "Model", ID }
            };
            return new JsonResult(response);
        }

        //TEMPORARY CLONE
        private void CloneLessonPart(CloneLessonPartEntity item)
        {
            _cloneLessonPartService.Collection.InsertOne(item);
            var list = _lessonPartQuestionService.CreateQuery().Find(o => o.ParentID == item.OriginID).ToList();
            if (list != null)
            {
                foreach (var question in list)
                {
                    var cloneQuestion = _lessonPartQuestionMapping.AutoOrtherType(question, new CloneLessonPartQuestionEntity());
                    cloneQuestion.OriginID = question.ID;
                    cloneQuestion.ParentID = item.ID;
                    cloneQuestion.ID = null;
                    cloneQuestion.ClassID = item.ClassID;
                    cloneQuestion.LessonID = item.ParentID;
                    CloneLessonQuestion(cloneQuestion);
                }
            }
        }

        private void CloneLessonQuestion(CloneLessonPartQuestionEntity item)
        {
            var _userCreate = User.Claims.GetClaimByType("UserID").Value;
            _cloneLessonPartQuestionService.Collection.InsertOne(item);
            var list = _lessonPartAnswerService.CreateQuery().Find(o => o.ParentID == item.OriginID).ToList();
            if (list != null)
            {
                foreach (var answer in list)
                {
                    var cloneAnswer = _lessonPartAnswerMapping.AutoOrtherType(answer, new CloneLessonPartAnswerEntity());
                    cloneAnswer.OriginID = answer.ID;
                    cloneAnswer.ParentID = item.ID;
                    cloneAnswer.ID = null;
                    cloneAnswer.ClassID = item.ClassID;
                    CloneLessonAnswer(cloneAnswer);
                }
            }
        }

        private void CloneLessonAnswer(CloneLessonPartAnswerEntity item)
        {
            _cloneLessonPartAnswerService.Collection.InsertOne(item);
        }


    }
}
