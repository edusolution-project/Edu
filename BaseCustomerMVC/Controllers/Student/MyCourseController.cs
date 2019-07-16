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
    public class MyCourseController : StudentController
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
        private readonly Core_v2.Globals.MappingEntity<LessonEntity, LessonScheduleViewModel> _mapping;
        private readonly Core_v2.Globals.MappingEntity<ClassEntity, MyClassViewModel> _mappingList;
        private readonly ExamService _examService;
        private readonly ExamDetailService _examDetailService;
        private readonly LessonPartQuestionService _lessonPartQuestionService;
        private readonly LessonPartAnswerService _lessonPartAnswerService;
        private readonly StudentService _studentService;
        private readonly MappingEntity<StudentEntity, ClassMemberViewModel> _studentMapping;
        private readonly ChapterService _chapterService;
        public MyCourseController(ClassService service
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
            , ExamService examService
            , ExamDetailService examDetailService
            , StudentService studentService
            )
        {
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
            _mapping = new Core_v2.Globals.MappingEntity<LessonEntity, LessonScheduleViewModel>();
            _mappingList = new Core_v2.Globals.MappingEntity<ClassEntity, MyClassViewModel>();
            _lessonPartQuestionService = lessonPartQuestionService;
            _lessonPartAnswerService = lessonPartAnswerService;
            _studentMapping = new MappingEntity<StudentEntity, ClassMemberViewModel>();
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

            var std = DataResponse.Select(o => _mappingList.AutoOrtherType(o,new MyClassViewModel() {
                CourseName = _courseService.GetItemByID(o.CourseID) == null ? "" : _courseService.GetItemByID(o.CourseID).Name,
                StudentNumber = o.Students.Count,
                SubjectName = _subjectService.GetItemByID(o.SubjectID) == null ? "" : _subjectService.GetItemByID(o.SubjectID).Name,
                GradeName = _gradeService.GetItemByID(o.GradeID) == null ? "" : _gradeService.GetItemByID(o.GradeID).Name,
                TeacherName = _teacherService.GetItemByID(o.TeacherID) == null ? "" : _teacherService.GetItemByID(o.TeacherID).FullName
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
        public JsonResult GetDetails(string CourseID,string ClassID)
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
        public JsonResult GetLesson(string LessonID,string ClassID)
        {
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            var lesson = _lessonService.GetItemByID(LessonID);
            if (lesson == null) return null;
            var listPart = _cloneLessonPartService.CreateQuery().Find(o => o.ParentID == lesson.ID).ToList();
            
            // var listPartOriginal = _lessonPartService.CreateQuery().Find(o => o.ParentID == lesson.ID);
            bool IsNull = false;
            if (listPart == null || listPart.Count <= 0)
            {
                var listparts = _lessonPartService.CreateQuery().Find(o => o.ParentID == lesson.ID).ToList();
                if(listparts != null)
                {
                    var mapp = new MappingEntity<LessonPartEntity, CloneLessonPartEntity>();
                    listPart = listparts.Select(o => mapp.AutoOrtherType(o, new CloneLessonPartEntity() { })).ToList();
                }
                IsNull = true;
            }
            var listData = listPart.ToList();
            var exam = _examService.CreateQuery().Find(o => o.LessonID == LessonID && o.ClassID == ClassID && o.StudentID == userId && o.Status == false).FirstOrDefault();
            if (exam == null || _examService.IsOverTime(exam.ID))
            {
                if (!IsNull)
                {
                    MappingEntity<LessonEntity, LessonViewModel> mapping = new MappingEntity<LessonEntity, LessonViewModel>();
                    MappingEntity<CloneLessonPartEntity, PartViewModel> mapPart = new MappingEntity<CloneLessonPartEntity, PartViewModel>();
                    MappingEntity<CloneLessonPartQuestionEntity, QuestionViewModel> mapQuestion = new MappingEntity<CloneLessonPartQuestionEntity, QuestionViewModel>();
                    var dataResponse = mapping.AutoOrtherType(lesson, new LessonViewModel()
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

                    var respone = new Dictionary<string, object>{{ "Data", dataResponse }};
                    return new JsonResult(respone);
                }
                else
                {
                    MappingEntity<LessonEntity, LessonViewModel> mapping = new MappingEntity<LessonEntity, LessonViewModel>();
                    MappingEntity<CloneLessonPartEntity, PartViewModel> mapPart = new MappingEntity<CloneLessonPartEntity, PartViewModel>();
                    MappingEntity<LessonPartQuestionEntity, QuestionViewModel> mapQuestion = new MappingEntity<LessonPartQuestionEntity, QuestionViewModel>();
                    MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity> mapAnswer = new MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity>();
                    var dataResponse = mapping.AutoOrtherType(lesson, new LessonViewModel()
                    {
                        Parts = listData.Select(o => mapPart.AutoOrtherType(o, new PartViewModel()
                        {
                            Questions = _lessonPartQuestionService.CreateQuery().Find(x => x.ParentID == o.ID).ToList()
                                .Select(z => mapQuestion.AutoOrtherType(z, new QuestionViewModel()
                                {
                                    CloneAnswers = _lessonPartAnswerService.CreateQuery().Find(x => x.ParentID == z.ID).ToList().Select(y => mapAnswer.AutoOrtherType(y, new CloneLessonPartAnswerEntity() {
                                        IsCorrect = false
                                    })).ToList()
                                }))?.ToList()
                        })).ToList()
                    });

                    var respone = new Dictionary<string, object> { { "Data", dataResponse } };
                    return new JsonResult(respone);
                }
            }
            else
            {
                var listED = _examDetailService.CreateQuery().Find(o => o.ExamID == exam.ID).ToList();
                var timere = exam.CurrentDoTime.AddMinutes(exam.Timer) - DateTime.UtcNow;
                // so sanh va dua ra dap an cua sinh vien
                if (!IsNull)
                {
                    MappingEntity<LessonEntity, LessonViewModel> mapping = new MappingEntity<LessonEntity, LessonViewModel>();
                    MappingEntity<CloneLessonPartEntity, PartViewModel> mapPart = new MappingEntity<CloneLessonPartEntity, PartViewModel>();
                    MappingEntity<CloneLessonPartQuestionEntity, QuestionViewModel> mapQuestion = new MappingEntity<CloneLessonPartQuestionEntity, QuestionViewModel>();
                    var dataResponse = mapping.AutoOrtherType(lesson, new LessonViewModel()
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

                    var respone = new Dictionary<string, object>
                    {
                        {
                            "Data", dataResponse
                        },
                        {
                             "Exam", exam
                        },
                        {
                            "Timer", timere.Minutes < 10 ? "0"+  timere.Minutes+":"+timere.Seconds: timere.Minutes+":"+timere.Seconds
                        }
                    };
                    return new JsonResult(respone);
                }
                else
                {
                    MappingEntity<LessonEntity, LessonViewModel> mapping = new MappingEntity<LessonEntity, LessonViewModel>();
                    MappingEntity<CloneLessonPartEntity, PartViewModel> mapPart = new MappingEntity<CloneLessonPartEntity, PartViewModel>();
                    MappingEntity<LessonPartQuestionEntity, QuestionViewModel> mapQuestion = new MappingEntity<LessonPartQuestionEntity, QuestionViewModel>();
                    MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity> mapAnswer = new MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity>();
                    var dataResponse = mapping.AutoOrtherType(lesson, new LessonViewModel()
                    {
                        Parts = listData.Select(o => mapPart.AutoOrtherType(o, new PartViewModel()
                        {
                            Questions = _lessonPartQuestionService.CreateQuery().Find(x => x.ParentID == o.ID).ToList()
                                .Select(z => mapQuestion.AutoOrtherType(z, new QuestionViewModel()
                                {
                                    CloneAnswers = _lessonPartAnswerService.CreateQuery().Find(x => x.ParentID == z.ID).ToList().Select(y => mapAnswer.AutoOrtherType(y, new CloneLessonPartAnswerEntity()
                                    {
                                        IsCorrect = false
                                    })).ToList()
                                }))?.ToList()
                        })).ToList()
                    });

                    var respone = new Dictionary<string, object>
                    {
                        {
                            "Data", dataResponse
                        },
                        {
                             "Exam", exam
                        },
                        {
                            "Timer", timere.Minutes < 10 ? "0"+  timere.Minutes+":"+timere.Seconds: timere.Minutes+":"+timere.Seconds
                        }
                    };
                    return new JsonResult(respone);
                }
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
            var studentsView = students.ToList().Select(t => _studentMapping.AutoOrtherType(t, new ClassMemberViewModel()
            {
                ClassName = currentClass.Name,
                ClassStatus = "Đang học",
                LastJoinDate = DateTime.Now
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
        
        public IActionResult Index(DefaultModel model)
        {
            ViewBag.Model = model;
            return View();
        }
        
        public IActionResult Calendar(DefaultModel model, string id,string ClassID)
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

        public IActionResult Detail(DefaultModel model,string id)
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
    }
}
