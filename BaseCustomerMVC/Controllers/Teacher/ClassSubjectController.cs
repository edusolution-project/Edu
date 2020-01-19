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
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class ClassSubjectController : TeacherController
    {
        private readonly GradeService _gradeService;
        private readonly AccountService _accountService;
        private readonly SubjectService _subjectService;
        private readonly TeacherService _teacherService;
        private readonly ClassService _classService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly CourseService _courseService;
        private readonly ClassProgressService _progressService;

        private readonly ChapterService _chapterService;
        private readonly ChapterExtendService _chapterExtendService;
        private readonly LessonService _lessonService;
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly StudentService _studentService;
        private readonly ScoreStudentService _scoreStudentService;
        private readonly LessonProgressService _lessonProgressService;
        private readonly LearningHistoryService _learningHistoryService;

        private readonly MappingEntity<StudentEntity, ClassMemberViewModel> _mapping;
        private readonly MappingEntity<ClassEntity, ClassActiveViewModel> _activeMapping;
        private readonly IHostingEnvironment _env;


        //private readonly LessonPartService _lessonPartService;
        //private readonly LessonPartAnswerService _lessonPartAnswerService;
        //private readonly LessonPartQuestionService _lessonPartQuestionService;
        private readonly ExamService _examService;
        private readonly ExamDetailService _examDetailService;

        //private readonly CloneLessonPartService _cloneLessonPartService;
        //private readonly CloneLessonPartAnswerService _cloneAnswerService;
        //private readonly CloneLessonPartQuestionService _cloneQuestionService;

        //private readonly MappingEntity<LessonPartEntity, CloneLessonPartEntity> _lessonPartMapping;
        //private readonly MappingEntity<LessonPartQuestionEntity, CloneLessonPartQuestionEntity> _lessonPartQuestionMapping;
        //private readonly MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity> _lessonPartAnswerMapping;

        private readonly FileProcess _fileProcess;
        private readonly StudentHelper _studentHelper;
        private readonly LessonHelper _lessonHelper;
        private readonly MappingEntity<LessonEntity, StudentModuleViewModel> _moduleViewMapping;
        private readonly MappingEntity<LessonEntity, StudentAssignmentViewModel> _assignmentViewMapping;


        public ClassSubjectController(
            AccountService accountService,
            GradeService gradeservice,
            SubjectService subjectService,
            ClassSubjectService classSubjectService,
            TeacherService teacherService,
            ClassService service,
            CourseService courseService,
            ClassProgressService progressService,

            ChapterService chapterService,
            ChapterExtendService chapterExtendService,
            LessonService lessonService,
            LessonScheduleService lessonScheduleService,
            ExamService examService,
            ExamDetailService examDetailService,
            LearningHistoryService learningHistoryService,

            ScoreStudentService scoreStudentService,
            LessonPartService lessonPartService,
            LessonPartQuestionService lessonPartQuestionService,
            LessonPartAnswerService lessonPartAnswerService,
            LessonProgressService lessonProgressService,

            CloneLessonPartService cloneLessonPartService,
            CloneLessonPartAnswerService cloneLessonPartAnswerService,
            CloneLessonPartQuestionService cloneLessonPartQuestionService,

            StudentService studentService, IHostingEnvironment evn,

            FileProcess fileProcess)
        {
            _accountService = accountService;
            _gradeService = gradeservice;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _courseService = courseService;
            _classService = service;
            _classSubjectService = classSubjectService;
            _progressService = progressService;

            _chapterService = chapterService;
            _chapterExtendService = chapterExtendService;
            _lessonService = lessonService;
            _lessonScheduleService = lessonScheduleService;
            _lessonProgressService = lessonProgressService;

            _examService = examService;
            _examDetailService = examDetailService;
            _learningHistoryService = learningHistoryService;
            _scoreStudentService = scoreStudentService;

            _studentService = studentService;
            _mapping = new MappingEntity<StudentEntity, ClassMemberViewModel>();
            _activeMapping = new MappingEntity<ClassEntity, ClassActiveViewModel>();
            _env = evn;
            _fileProcess = fileProcess;

            _studentHelper = new StudentHelper(studentService, accountService);
            _lessonHelper = new LessonHelper(
                lessonService,
                lessonPartService,
                lessonPartQuestionService,
                lessonPartAnswerService,
                cloneLessonPartService,
                cloneLessonPartAnswerService,
                cloneLessonPartQuestionService
                );
            _moduleViewMapping = new MappingEntity<LessonEntity, StudentModuleViewModel>();
            _assignmentViewMapping = new MappingEntity<LessonEntity, StudentAssignmentViewModel>();
        }

        [HttpPost]
        public JsonResult GetList(DefaultModel model, string SubjectID = "", string GradeID = "", string UserID = "")
        {
            var filter = new List<FilterDefinition<ClassEntity>>();
            filter.Add(Builders<ClassEntity>.Filter.Where(o => o.IsActive));
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
                        {"Msg","Teacher not found" }
                    });
                }
            }
            if (teacher != null)
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.TeacherID == UserID));

            if (!string.IsNullOrEmpty(model.SearchText))
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Name.ToLower().Contains(model.SearchText.ToLower())));
            }
            if (!string.IsNullOrEmpty(SubjectID))
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.SubjectID == SubjectID));
            }
            //if (!string.IsNullOrEmpty(GradeID))
            //{
            //    filter.Add(Builders<ClassEntity>.Filter.Where(o => o.GradeID == GradeID));
            //}

            var data = filter.Count > 0 ? _classService.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _classService.GetAll();
            model.TotalRecord = data.CountDocuments();
            var DataResponse = data == null || model.TotalRecord <= 0 // || model.TotalRecord < model.PageSize
                ? data
                : data.Skip((model.PageIndex) * model.PageSize).Limit(model.PageSize);

            var response = new Dictionary<string, object>
            {
                { "Data", DataResponse.ToList().Select(o=> new ClassViewModel(o){
                        CourseName = _courseService.GetItemByID(o.CourseID)?.Name,
                        GradeName = _gradeService.GetItemByID(o.GradeID)?.Name,
                        SubjectName = _subjectService.GetItemByID(o.SubjectID).Name,
                        TeacherName = _teacherService.GetItemByID(o.TeacherID).FullName
                    })
                },
                { "Model", model }
            };
            return new JsonResult(response);
        }

        public JsonResult GetActiveList(DateTime today)
        {
            var filter = new List<FilterDefinition<ClassEntity>>();
            filter.Add(Builders<ClassEntity>.Filter.Where(o => o.IsActive));
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new ReturnJsonModel
                {
                    StatusCode = ReturnStatus.ERROR,
                    StatusDesc = "Authentication Error"
                });
            }
            filter.Add(Builders<ClassEntity>.Filter.Where(o => o.TeacherID == userId));
            filter.Add(Builders<ClassEntity>.Filter.Where(o => (o.StartDate <= today) && (o.EndDate >= today)));

            var data = filter.Count > 0 ? _classService.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _classService.GetAll();

            var std = (from o in data.ToList()
                       let progress = _progressService.GetItemByClassID(o.ID, userId)
                       let percent = progress == null || progress.TotalLessons == 0 ? 0 : progress.CompletedLessons.Count * 100 / progress.TotalLessons
                       let totalweek = (o.EndDate.Date - o.StartDate.Date).TotalDays / 7
                       select new
                       {
                           id = o.ID,
                           courseID = o.CourseID,
                           courseName = o.Name,
                           subjectName = _subjectService.GetItemByID(o.SubjectID) == null ? "" : _subjectService.GetItemByID(o.SubjectID).Name,
                           thumb = o.Image ?? "",
                           endDate = o.EndDate,
                           //week = totalweek > 0 ? (DateTime.Now.Date - o.StartDate.Date).TotalDays / 7 / totalweek : 0,
                           students = o.Students.Count
                       }).ToList();
            return Json(new { Data = std });
        }

        public JsonResult GetFinishList(DefaultModel model, DateTime today)
        {
            var filter = new List<FilterDefinition<ClassEntity>>();
            filter.Add(Builders<ClassEntity>.Filter.Where(o => o.IsActive));
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            filter.Add(Builders<ClassEntity>.Filter.Where(o => o.TeacherID == userId));
            filter.Add(Builders<ClassEntity>.Filter.Where(o => o.EndDate < today));

            var data = filter.Count > 0 ? _classService.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _classService.GetAll();
            model.TotalRecord = data.CountDocuments();
            var DataResponse = data == null || model.TotalRecord <= 0 || model.TotalRecord < model.PageSize
                ? data.ToList()
                : data.Skip(model.PageIndex * model.PageSize).Limit(model.PageSize).ToList();

            var std = (from o in DataResponse
                       let progress = _progressService.GetItemByClassID(o.ID, userId)
                       let per = progress == null || progress.TotalLessons == 0 ? 0 : progress.CompletedLessons.Count * 100 / progress.TotalLessons
                       select new
                       {
                           id = o.ID,
                           courseID = o.CourseID,
                           title = o.Name,
                           endDate = o.EndDate,
                           per,
                           //score = progress != null ? progress.AvgPoint : 0
                       }).ToList();
            return Json(new { Data = std });
        }

        public JsonResult GetManageList(DefaultModel model, string SubjectID = "", string GradeID = "", string TeacherID = "", string UserID = "")
        {
            var filter = new List<FilterDefinition<ClassEntity>>();
            if (!string.IsNullOrEmpty(SubjectID))
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.SubjectID == SubjectID));
            }
            if (!string.IsNullOrEmpty(GradeID))
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.GradeID == GradeID));
            }
            if (!string.IsNullOrEmpty(TeacherID))
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.TeacherID == TeacherID));
            }

            var data = filter.Count > 0 ? _classService.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _classService.GetAll();

            var response = new Dictionary<string, object>
            {
                { "Data", data.SortByDescending(t=> t.IsActive).ThenByDescending(t=> t.ID).ToList().Select(o=> new ClassViewModel(o){
                        CourseName = _courseService.GetItemByID(o.CourseID)?.Name,
                        GradeName = _gradeService.GetItemByID(o.GradeID)?.Name,
                        SubjectName = _subjectService.GetItemByID(o.SubjectID).Name,
                        TeacherName = _teacherService.GetItemByID(o.TeacherID).FullName
                    })
                },
                { "Model", model }
            };
            return new JsonResult(response);
        }

        public JsonResult GetThisWeekLesson(DateTime today)
        {
            var startWeek = today.AddDays(DayOfWeek.Sunday - today.DayOfWeek);
            var endWeek = startWeek.AddDays(7);

            var filter = new List<FilterDefinition<LessonScheduleEntity>>();
            filter.Add(Builders<LessonScheduleEntity>.Filter.Where(o => o.IsActive));
            filter.Add(Builders<LessonScheduleEntity>.Filter.Where(o => o.StartDate <= endWeek && o.EndDate >= startWeek));
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new ReturnJsonModel
                {
                    StatusCode = ReturnStatus.ERROR,
                    StatusDesc = "Authentication Error"
                });
            }

            var classFilter = new List<FilterDefinition<ClassEntity>>();
            classFilter.Add(Builders<ClassEntity>.Filter.Where(o => o.TeacherID == userId));
            var classIds = _classService.Collection.Find(Builders<ClassEntity>.Filter.And(classFilter)).Project(t => t.ID).ToList();

            filter.Add(Builders<LessonScheduleEntity>.Filter.Where(t => classIds.Contains(t.ClassID)));

            var data = _lessonScheduleService.Collection.Find(Builders<LessonScheduleEntity>.Filter.And(filter));

            var std = (from o in data.ToList()
                       let _lesson = _lessonService.Collection.Find(t => t.ID == o.LessonID).SingleOrDefault()
                       where _lesson != null
                       let _class = _classService.Collection.Find(t => t.ID == o.ClassID).SingleOrDefault()
                       where _class != null
                       //let isLearnt = _learningHistoryService.GetLastLearnt(userId, o.LessonID) != null
                       select new
                       {
                           id = o.ID,
                           classID = _class.ID,
                           className = _class.Name,
                           title = _lesson.Title,
                           lessonID = _lesson.ID,
                           startDate = o.StartDate,
                           endDate = o.EndDate,
                           students = _class.Students.Count
                           //isLearnt = isLearnt
                       }).ToList();
            return Json(new { Data = std });
        }


        //public JsonResult GetActiveList()
        //{
        //    var filter = new List<FilterDefinition<ClassEntity>>();
        //    TeacherEntity teacher = null;
        //    var UserID = User.Claims.GetClaimByType("UserID").Value;
        //    if (!string.IsNullOrEmpty(UserID) && UserID != "0")
        //    {
        //        teacher = UserID == "0" ? null : _teacherService.GetItemByID(UserID);
        //        if (teacher == null)
        //        {
        //            return new JsonResult(new Dictionary<string, object> {
        //                {"Data",null },
        //                {"Error",UserID },
        //                {"Msg","Không có thông tin giảng viên" }
        //            });
        //        }
        //    }
        //    if (teacher != null)
        //        filter.Add(Builders<ClassEntity>.Filter.Where(o => o.TeacherID == UserID && o.EndDate >= DateTime.Now.ToLocalTime().Date));

        //    var data = filter.Count > 0 ? _classService.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _classService.GetAll();
        //    var DataResponse = data;
        //    if (data != null)
        //        DataResponse = data.Limit(3);

        //    var response = new Dictionary<string, object>
        //    {
        //        { "Data", DataResponse.ToList().Select(t=> _activeMapping.AutoOrtherType(t, new ClassActiveViewModel(){
        //            Progress = (int)((DateTime.Now.ToLocalTime().Date - t.StartDate.ToLocalTime().Date).TotalDays  * 100 / (t.EndDate.ToLocalTime().Date - t.StartDate.ToLocalTime().Date).TotalDays),
        //            SubjectName = _subjectService.GetItemByID(t.SubjectID).Name
        //            }))
        //            }
        //    };
        //    return new JsonResult(response);
        //}

        [HttpPost]
        [Obsolete]
        public JsonResult Create(ClassEntity item, List<ClassSubjectEntity> ClassSubjects)
        {
            if (string.IsNullOrEmpty(item.ID) || item.ID == "0")
            {
                item.ID = null;
                item.Created = DateTime.Now;
                var userId = User.Claims.GetClaimByType("UserID").Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return new JsonResult(new Dictionary<string, object>()
                        {
                            {"Error", "Permission Error" }
                        });
                }
                item.TeacherID = userId; // creator
                _classService.CreateOrUpdate(item);

                //Create class subjects
                if (ClassSubjects != null && ClassSubjects.Count > 0)
                {
                    foreach (var csubject in ClassSubjects)
                    {
                        var subject = _subjectService.GetItemByID(csubject.SubjectID);
                        if (subject == null) continue;
                        var course = _courseService.GetItemByID(csubject.CourseID);
                        if (course == null || !course.IsActive)
                            return new JsonResult(new Dictionary<string, object>()
                        {
                            {"Error", "Curriculum for " + subject.Name + " is not available" }
                        });
                        csubject.ClassID = item.ID;
                        csubject.Description = course.Description;
                        csubject.LearningOutcomes = course.LearningOutcomes;
                        //subject.Image = course.Image;
                        _classSubjectService.CreateOrUpdate(csubject);

                        //Create Class => Create Lesson Schedule & Clone all lesson
                        var lessons = _lessonService.CreateQuery().Find(o => o.CourseID == csubject.CourseID).ToList();

                        var schedules = new List<LessonScheduleEntity>();
                        if (lessons != null)
                            foreach (LessonEntity lesson in lessons)
                            {
                                _lessonScheduleService.CreateQuery().InsertOne(new LessonScheduleEntity
                                {
                                    ClassID = item.ID,
                                    ClassSubjectID = csubject.ID,
                                    LessonID = lesson.ID,
                                    IsActive = true
                                });
                                _lessonHelper.CloneLessonForClass(lesson, csubject);
                            }
                        _courseService.Collection.UpdateOneAsync(t => t.ID == csubject.CourseID, new UpdateDefinitionBuilder<CourseEntity>().Set(t => t.IsUsed, true));
                    }
                }

                //var course = _courseService.GetItemByID(item.CourseID);
                //if (course == null || !course.IsActive)
                //    return new JsonResult(new Dictionary<string, object>()
                //        {
                //            {"Error", "Curriculum is not available" }
                //        });

                Dictionary<string, object> response = new Dictionary<string, object>()
                {
                    {"Data",item },
                    {"Error",null },
                    {"Msg","Success" }
                };
                return new JsonResult(response);
            }
            else
            {
                //Edit common data only, class subject must be edited in class detail page
                var oldData = _classService.GetItemByID(item.ID);
                if (oldData == null) return new JsonResult(new Dictionary<string, object>()
                {
                    {"Error", "Class not found" }
                });

                //if (item.Code != oldData.Code)
                //{
                //    if (_classService.CreateQuery().Find(t => t.Code == item.Code).FirstOrDefault() != null)
                //        return new JsonResult(new Dictionary<string, object>()
                //        {
                //            {"Error", "Class code used" }
                //        });
                //}

                oldData.Updated = DateTime.Now;


                //if (item.CourseID != oldData.CourseID)
                //{
                //    //remove old schedule
                //    _lessonScheduleService.CreateQuery().DeleteMany(o => o.ClassID == item.ID);
                //    //remove clone lesson part
                //    _lessonHelper.RemoveClone(item.ID);
                //    //remove progress: learning history => class progress, chapter progress, lesson progress
                //    _learningHistoryService.RemoveClassHistory(item.ID);
                //    //resest exam
                //    _examService.RemoveClassExam(item.ID);

                //    //Create Class => Create Lesson Schedule & Clone all lesson
                //    var lessons = _lessonService.CreateQuery().Find(o => o.CourseID == item.CourseID).ToList();

                //    if (lessons != null)
                //        foreach (LessonEntity lesson in lessons)
                //        {
                //            var schedule = new LessonScheduleEntity
                //            {
                //                ClassID = item.ID,
                //                LessonID = lesson.ID,
                //                IsActive = true
                //            };
                //            _lessonScheduleService.CreateOrUpdate(schedule);
                //            //_calendarHelper.ConvertCalendarFromSchedule(schedule, "");

                //            _lessonHelper.CloneLessonForClass(lesson, item);
                //        }
                //}

                oldData.Name = item.Name;
                oldData.Code = item.Code;
                oldData.StartDate = item.StartDate;
                oldData.EndDate = item.EndDate;
                oldData.Description = item.Description;
                //oldData.CourseID = item.CourseID;
                //oldData.GradeID = item.GradeID;
                //oldData.TeacherID = item.TeacherID;

                _classService.CreateOrUpdate(oldData);

                _courseService.Collection.UpdateOneAsync(t => t.ID == item.CourseID, new UpdateDefinitionBuilder<CourseEntity>().Set(t => t.IsUsed, true));

                Dictionary<string, object> response = new Dictionary<string, object>()
                {
                    {"Data",item },
                    {"Error",null },
                    {"Msg","Success" }
                };
                return new JsonResult(response);
            }
        }

        [HttpPost]
        [Obsolete]
        public JsonResult Remove(DefaultModel model)
        {
            if (model.ArrID.Length <= 0)
            {
                return new JsonResult(
                    new Dictionary<string, object>
                    {
                        { "Error", "Nothing to Delete" }
                    }
                );
            }
            else
            {

                var ids = model.ArrID.Split(',');
                if (ids.Length > 0)
                {
                    //remove Schedule, Part, Question, Answer
                    _lessonScheduleService.CreateQuery().DeleteMany(o => ids.Contains(o.ClassID));
                    _lessonHelper.RemoveClone(ids);
                    _examService.Collection.DeleteMany(o => ids.Contains(o.ClassID));
                    _examDetailService.Collection.DeleteMany(o => ids.Contains(o.ClassID));
                    var delete = _classService.Collection.DeleteMany(o => ids.Contains(o.ID));
                    return new JsonResult(delete);
                }
                else
                {
                    return new JsonResult(
                       new Dictionary<string, object>
                       {
                            { "Error", "Nothing to Delete" }
                       }
                    );
                }
            }
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
                    var filter = Builders<ClassEntity>.Filter.Where(o => model.ArrID.Split(',').Contains(o.ID) && o.IsActive == false);
                    var update = Builders<ClassEntity>.Update.Set("IsActive", true);
                    var publish = _classService.Collection.UpdateMany(filter, update);
                    return new JsonResult(publish);
                }
                else
                {
                    var filter = Builders<ClassEntity>.Filter.Where(o => model.ArrID == o.ID && o.IsActive == false);
                    var update = Builders<ClassEntity>.Update.Set("IsActive", true);
                    var publish = _classService.Collection.UpdateMany(filter, update);
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
                    var filter = Builders<ClassEntity>.Filter.Where(o => model.ArrID.Split(',').Contains(o.ID) && o.IsActive == true);
                    var update = Builders<ClassEntity>.Update.Set("IsActive", false);
                    var publish = _classService.Collection.UpdateMany(filter, update);
                    return new JsonResult(publish);
                }
                else
                {
                    var filter = Builders<ClassEntity>.Filter.Where(o => model.ArrID == o.ID && o.IsActive == true);
                    var update = Builders<ClassEntity>.Update.Set("IsActive", false);
                    var publish = _classService.Collection.UpdateMany(filter, update);
                    return new JsonResult(publish);
                }


            }
        }
    }
}
