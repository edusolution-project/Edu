using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Core_v2.Globals;
using Core_v2.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseCustomerMVC.Controllers.Student
{
    public class CourseController : StudentController
    {
        private readonly ClassService _service;
        private readonly CourseService _courseService;
        private readonly TeacherService _teacherService;
        private readonly SubjectService _subjectService;
        private readonly GradeService _gradeService;
        private readonly LessonService _lessonService;
        private readonly LessonPartService _lessonPartService;
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly CloneLessonPartService _cloneLessonPartService;
        private readonly CloneLessonPartAnswerService _cloneLessonPartAnswerService;
        private readonly CloneLessonPartQuestionService _cloneLessonPartQuestionService;

        private readonly ClassProgressService _progressService;
        private readonly ExamService _examService;
        private readonly ExamDetailService _examDetailService;
        private readonly LessonPartQuestionService _lessonPartQuestionService;
        private readonly LessonPartAnswerService _lessonPartAnswerService;
        private readonly StudentService _studentService;
        private readonly ChapterService _chapterService;
        private readonly LearningHistoryService _learningHistoryService;

        private readonly MappingEntity<LessonEntity, LessonScheduleViewModel> _mapping;
        private readonly MappingEntity<ClassEntity, StudentClassViewModel> _mappingList;
        private readonly MappingEntity<StudentEntity, ClassMemberViewModel> _studentMapping;
        private readonly MappingEntity<ClassEntity, ClassActiveViewModel> _activeMapping;

        private readonly MappingEntity<LessonPartEntity, CloneLessonPartEntity> _lessonPartMapping;
        private readonly MappingEntity<LessonPartQuestionEntity, CloneLessonPartQuestionEntity> _lessonPartQuestionMapping;
        private readonly MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity> _lessonPartAnswerMapping;


        public CourseController(ClassService service
            , CourseService courseService
            , TeacherService teacherService
            , SubjectService subjectService
            , GradeService gradeService
            , LessonService lessonService
            , ChapterService chapterService
            , LessonPartQuestionService lessonPartQuestionService
            , LessonPartAnswerService lessonPartAnswerService
            , LessonScheduleService lessonScheduleService
            , CloneLessonPartService cloneLessonPartService
            , CloneLessonPartAnswerService cloneLessonPartAnswerService
            , CloneLessonPartQuestionService cloneLessonPartQuestionService
            , LessonPartService lessonPartService
            , ClassProgressService progressService
            , ExamService examService
            , ExamDetailService examDetailService
            , StudentService studentService
            , LearningHistoryService learningHistoryService
            )
        {
            _learningHistoryService = learningHistoryService;
            _chapterService = chapterService;
            _studentService = studentService;
            _examService = examService;
            _examDetailService = examDetailService;
            _service = service;
            _courseService = courseService;
            _teacherService = teacherService;
            _subjectService = subjectService;
            _gradeService = gradeService;
            _lessonService = lessonService;
            _lessonScheduleService = lessonScheduleService;
            _cloneLessonPartAnswerService = cloneLessonPartAnswerService;
            _cloneLessonPartService = cloneLessonPartService;
            _cloneLessonPartQuestionService = cloneLessonPartQuestionService;
            _lessonPartService = lessonPartService;
            _progressService = progressService;
            _mapping = new MappingEntity<LessonEntity, LessonScheduleViewModel>();
            _mappingList = new MappingEntity<ClassEntity, StudentClassViewModel>();
            _lessonPartQuestionService = lessonPartQuestionService;
            _lessonPartAnswerService = lessonPartAnswerService;
            _studentMapping = new MappingEntity<StudentEntity, ClassMemberViewModel>();
            _activeMapping = new MappingEntity<ClassEntity, ClassActiveViewModel>();

            _lessonPartMapping = new MappingEntity<LessonPartEntity, CloneLessonPartEntity>();
            _lessonPartQuestionMapping = new MappingEntity<LessonPartQuestionEntity, CloneLessonPartQuestionEntity>();
            _lessonPartAnswerMapping = new MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity>();
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetList(DefaultModel model, string TeacherID)
        {
            var filter = new List<FilterDefinition<ClassEntity>>();
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            else
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Students.Contains(userId)));
            }
            if (!string.IsNullOrEmpty(model.SearchText))
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Name.ToLower().Contains(model.SearchText.ToLower())));
            }
            if (model.StartDate > DateTime.MinValue)
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.StartDate >= new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0)));
            }
            if (model.EndDate > DateTime.MinValue)
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.EndDate <= new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59)));
            }
            var data = filter.Count > 0 ? _service.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _service.GetAll();
            model.TotalRecord = data.Count();
            var DataResponse = data == null || data.Count() <= 0 || data.Count() < model.PageSize
                ? data.ToList()
                : data.Skip((model.PageIndex - 1) * model.PageSize).Limit(model.PageSize).ToList();


            var std = DataResponse.Select(o => _mappingList.AutoOrtherType(o, new StudentClassViewModel()
            {
                CourseName = _courseService.GetItemByID(o.CourseID) == null ? "" : _courseService.GetItemByID(o.CourseID).Name,
                StudentNumber = o.Students.Count,
                SubjectName = _subjectService.GetItemByID(o.SubjectID) == null ? "" : _subjectService.GetItemByID(o.SubjectID).Name,
                GradeName = _gradeService.GetItemByID(o.GradeID) == null ? "" : _gradeService.GetItemByID(o.GradeID).Name,
                TeacherName = _teacherService.GetItemByID(o.TeacherID) == null ? "" : _teacherService.GetItemByID(o.TeacherID).FullName,
                Progress = _progressService.GetItemByClassID(o.ID)
            })).ToList();

            var respone = new Dictionary<string, object>
            {
                { "Data", std},
                { "Model", model }
            };
            return new JsonResult(respone);
        }


        [System.Obsolete]
        [HttpPost]
        public JsonResult GetDetails(string CourseID, string ClassID)
        {
            try
            {
                var userId = User.Claims.GetClaimByType("UserID").Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return null;
                }
                var filterSchedule = Builders<LessonScheduleEntity>.Filter.Where(o => o.ClassID == ClassID);
                var dataSchedule = _lessonScheduleService.Collection.Find(filterSchedule);
                if (dataSchedule == null || dataSchedule.Count() <= 0) return null;
                var schedules = dataSchedule.ToEnumerable();
                var filter = new List<FilterDefinition<LessonEntity>>();
                filter.Add(Builders<LessonEntity>.Filter.Where(o => o.CourseID == CourseID));
                //filter.Add(Builders<LessonEntity>.Filter.Where(o=> schedules.Select(x => x.LessonID).Contains(o.ID)));
                var data = _lessonService.Collection.Find(Builders<LessonEntity>.Filter.And(filter));
                var DataResponse = data == null || data.Count() <= 0 ? null : data.ToList();

                var respone = new Dictionary<string, object>
                {
                    { "Data", DataResponse.Select(
                        o=> _mapping.AutoOrtherType(o,new LessonScheduleViewModel(){
                                IsActive = _lessonScheduleService.GetItemByID(ClassID) == null ? false: _lessonScheduleService.GetItemByID(ClassID).IsActive,
                                StartDate = _lessonScheduleService.GetItemByID(ClassID) == null ?DateTime.MinValue :  _lessonScheduleService.GetItemByID(ClassID).StartDate,
                                EndDate = _lessonScheduleService.GetItemByID(ClassID) == null ? DateTime.MinValue : _lessonScheduleService.GetItemByID(ClassID).EndDate,
                                IsView = _learningHistoryService.CreateQuery().Count(x=>x.StudentID == userId && x.LessonID == o.ID && x.ClassID == ClassID) > 0
                            })
                        )
                    }
                };
                return new JsonResult(respone);
            }
            catch (Exception ex)
            {
                var respone = new Dictionary<string, object>
                {
                    { "Data", null },
                    {"Error",ex }
                };
                return new JsonResult(respone);
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

            var currentClass = _service.GetItemByID(ClassID);
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
        public JsonResult GetMembers(DefaultModel model)
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

            var currentClass = _service.GetItemByID(model.ID);
            if (currentClass == null || currentClass.Students.IndexOf(UserID) < 0)
            {
                return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Không có thông tin lớp học" }
                    });
            }

            var teacher = _teacherService.GetItemByID(currentClass.TeacherID);

            var filter = new List<FilterDefinition<StudentEntity>>();
            filter.Add(Builders<StudentEntity>.Filter.Where(o => currentClass.Students.Contains(o.ID)));
            var students = filter.Count > 0 ? _studentService.Collection.Find(Builders<StudentEntity>.Filter.And(filter)) : _studentService.GetAll();
            var totalLessons = _lessonScheduleService.CreateQuery().CountDocuments(o => o.ClassID == currentClass.ID);
            var studentsView = (from r in students.ToList()
                                let learned = _learningHistoryService.CreateQuery().AsQueryable()
                                .Where(t => t.StudentID == r.ID && t.ClassID == currentClass.ID)
                                .GroupBy(t => new { t.StudentID, t.ClassID, t.LessonID }).Count()
                                select _studentMapping.AutoOrtherType(r, new ClassMemberViewModel()
                                {
                                    ClassName = currentClass.Name,
                                    ClassStatus = "Đang học",
                                    Progress = (int)(totalLessons != 0 ? learned * 100 / totalLessons : 0),
                                })).ToList();

            var response = new Dictionary<string, object>
            {
                { "Data",new Dictionary<string, object> {
                        {"Teacher",teacher },
                        {"Students",studentsView }
                    }
                },
                { "Model", model }
            };
            return new JsonResult(response);
        }

        public JsonResult GetActiveList()
        {
            var filter = new List<FilterDefinition<ClassEntity>>();
            var UserID = User.Claims.GetClaimByType("UserID").Value;

            filter.Add(Builders<ClassEntity>.Filter.Where(o => o.IsActive && o.Students.Contains(UserID) && o.EndDate >= DateTime.Now.ToLocalTime().Date));

            var activeClasses = _service.Collection.Find(Builders<ClassEntity>.Filter.And(filter)).ToList();

            var responseData = new List<ClassActiveViewModel>();

            var subjects = _subjectService.GetAll().ToList();

            var _activeMapping = new MappingEntity<ClassEntity, ClassActiveViewModel>();

            foreach (var _class in activeClasses)
            {
                var totalLessons = _lessonScheduleService.CreateQuery().CountDocuments(o => o.ClassID == _class.ID);
                var learned = _learningHistoryService.CreateQuery().Aggregate().Match(o => o.ClassID == _class.ID).Group(o => o.LessonID,
                    o => new { x = o.Select(x => 1).First() }).ToList().Count();
                responseData.Add(_activeMapping.AutoOrtherType(_class, new ClassActiveViewModel()
                {
                    Progress = (int)(totalLessons != 0 ? learned * 100 / totalLessons : 0),
                    SubjectName = subjects.SingleOrDefault(s => s.ID == _class.SubjectID).Name
                }));
            }

            var response = new Dictionary<string, object>
            {
                { "Data", responseData }
            };
            return new JsonResult(response);
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

            var currentClass = _service.GetItemByID(model.ID);
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
                           select _mapping.AutoOrtherType(r, new LessonScheduleViewModel()
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
        public JsonResult GetAssignments(DefaultModel model)
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

            var currentClass = _service.GetItemByID(model.ID);
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
                //Chapters = _chapterService.CreateQuery().Find(o => o.CourseID == course.ID).SortBy(o => o.ParentID).ThenBy(o => o.Order).ThenBy(o => o.ID).ToList(),
                Lessons = (from r in _lessonService.CreateQuery().Find(o => o.CourseID == course.ID && o.Etype > 0).SortBy(o => o.ChapterID).ThenBy(o => o.Order).ThenBy(o => o.ID).ToList()
                           let schedule = _lessonScheduleService.CreateQuery().Find(o => o.LessonID == r.ID && o.ClassID == model.ID).FirstOrDefault()
                           let lastjoin = _learningHistoryService.CreateQuery().Find(x => x.StudentID == UserID && x.LessonID == r.ID && x.ClassID == model.ID).SortByDescending(o => o.ID).FirstOrDefault()
                           select _mapping.AutoOrtherType(r, new LessonScheduleViewModel()
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



        public IActionResult Index(DefaultModel model)
        {
            ViewBag.Model = model;
            return View();
        }

        public IActionResult Calendar(DefaultModel model, string id, string ClassID)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Bạn chưa chọn khóa học";
                return RedirectToAction("Index");
            }
            ViewBag.CourseID = id;
            ViewBag.ClassID = ClassID;
            ViewBag.Model = model;
            return View();
        }

        public IActionResult Detail(DefaultModel model, string id)
        {
            if (model == null) return null;
            var currentClass = _service.GetItemByID(id);
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (currentClass == null)
                return RedirectToAction("Index");
            if (currentClass.Students.IndexOf(userId) < 0)
                return RedirectToAction("Index");
            ViewBag.Class = currentClass;
            return View();
        }

        public IActionResult Syllabus(DefaultModel model, string id)
        {
            if (model == null) return null;
            var currentClass = _service.GetItemByID(id);
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (currentClass == null)
                return RedirectToAction("Index");
            if (currentClass.Students.IndexOf(userId) < 0)
                return RedirectToAction("Index");
            ViewBag.Class = currentClass;
            return View();
        }

        public IActionResult Modules(DefaultModel model, string id)
        {
            if (model == null) return null;
            var currentClass = _service.GetItemByID(id);
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (currentClass == null)
                return RedirectToAction("Index");
            if (currentClass.Students.IndexOf(userId) < 0)
                return RedirectToAction("Index");
            ViewBag.Class = currentClass;
            return View();
        }

        public IActionResult Assignment(DefaultModel model, string id)
        {
            if (model == null) return null;
            var currentClass = _service.GetItemByID(id);
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (currentClass == null)
                return RedirectToAction("Index");
            if (currentClass.Students.IndexOf(userId) < 0)
                return RedirectToAction("Index");
            ViewBag.Class = currentClass;
            return View();
        }

        public IActionResult Discussions(DefaultModel model)
        {
            if (model == null) return null;
            var currentClass = _service.GetItemByID(model.ID);
            if (currentClass == null)
                return RedirectToAction("Index");
            ViewBag.Class = currentClass;
            return View();
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
