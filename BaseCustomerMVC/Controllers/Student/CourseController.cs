using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Core_v2.Globals;
using Core_v2.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseCustomerMVC.Controllers.Student
{
    public class CourseController : StudentController
    {
        private readonly ClassService _service;
        private readonly ClassSubjectService _classSubjectService;
        private readonly SkillService _skillService;
        private readonly CourseService _courseService;
        private readonly TeacherService _teacherService;
        //private readonly ClassStudentService _classStudentService;
        private readonly SubjectService _subjectService;
        private readonly GradeService _gradeService;
        private readonly LessonService _lessonService;
        private readonly LessonPartService _lessonPartService;
        private readonly ClassGroupService _classGroupService;

        ////private readonly LessonScheduleService _lessonScheduleService;
        private readonly LessonPartQuestionService _lessonPartQuestionService;
        private readonly LessonPartAnswerService _lessonPartAnswerService;
        private readonly LessonProgressService _lessonProgressService;

        private readonly CloneLessonPartService _cloneLessonPartService;
        private readonly CloneLessonPartAnswerService _cloneLessonPartAnswerService;
        private readonly CloneLessonPartQuestionService _cloneLessonPartQuestionService;

        private readonly ClassProgressService _progressService;
        private readonly ClassSubjectProgressService _classSubjectProgressService;
        private readonly ExamService _examService;
        private readonly ExamDetailService _examDetailService;

        private readonly StudentService _studentService;
        private readonly ChapterService _chapterService;
        private readonly ChapterProgressService _chapterProgressService;
        private readonly ChapterExtendService _chapterExtendService;
        private readonly CalendarHelper _calendarHelper;
        private readonly CacheHelper _cacheHelper;
        private readonly LearningHistoryService _learningHistoryService;
        private readonly CenterService _centerService;

        private readonly NewsService _newsService;
        private readonly NewsCategoryService _newsCategoryService;

        private readonly MappingEntity<LessonEntity, LessonScheduleViewModel> _mapping;
        private readonly MappingEntity<ClassEntity, StudentClassViewModel> _mappingList;
        //private readonly MappingEntity<ClassSubjectEntity, StudentClassViewModelV2> _mappingList;
        private readonly MappingEntity<StudentEntity, ClassStudentViewModel> _studentMapping;
        private readonly MappingEntity<ClassEntity, ClassActiveViewModel> _activeMapping;

        private readonly MappingEntity<LessonPartEntity, CloneLessonPartEntity> _lessonPartMapping;
        private readonly MappingEntity<LessonPartQuestionEntity, CloneLessonPartQuestionEntity> _lessonPartQuestionMapping;
        private readonly MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity> _lessonPartAnswerMapping;


        public CourseController(ClassService service
            , ClassSubjectService classSubjectService
            , SkillService skillService
            , CourseService courseService
            //, ClassStudentService classStudentService
            , TeacherService teacherService
            , SubjectService subjectService
            , GradeService gradeService
            , LessonService lessonService
            , ChapterService chapterService
            , ChapterProgressService chapterProgressService
            , ChapterExtendService chapterExtendService
            , LessonPartQuestionService lessonPartQuestionService
            , LessonPartAnswerService lessonPartAnswerService
            , ClassGroupService classGroupService
            ////, LessonScheduleService lessonScheduleService
            , CloneLessonPartService cloneLessonPartService
            , CloneLessonPartAnswerService cloneLessonPartAnswerService
            , CloneLessonPartQuestionService cloneLessonPartQuestionService
            , LessonPartService lessonPartService
            , ClassProgressService progressService
            , ClassSubjectProgressService classSubjectProgressService
            , ExamService examService
            , ExamDetailService examDetailService
            , StudentService studentService
            , LessonProgressService lessonProgressService
            , LearningHistoryService learningHistoryService
            , CalendarHelper calendarHelper
            , CacheHelper cacheHelper
            , CenterService centerService
            , NewsService newsService
            , NewsCategoryService newsCategoryService
            )
        {
            _lessonProgressService = lessonProgressService;
            _learningHistoryService = learningHistoryService;
            _chapterService = chapterService;
            _chapterExtendService = chapterExtendService;
            _chapterProgressService = chapterProgressService;
            _studentService = studentService;
            _examService = examService;
            _examDetailService = examDetailService;
            _service = service;
            _skillService = skillService;
            _classSubjectService = classSubjectService;
            _courseService = courseService;
            _classGroupService = classGroupService;
            //_classStudentService = classStudentService;
            _teacherService = teacherService;
            _subjectService = subjectService;
            _gradeService = gradeService;
            _lessonService = lessonService;
            ////_lessonScheduleService = lessonScheduleService;
            _cloneLessonPartAnswerService = cloneLessonPartAnswerService;
            _cloneLessonPartService = cloneLessonPartService;
            _cloneLessonPartQuestionService = cloneLessonPartQuestionService;
            _lessonPartService = lessonPartService;
            _progressService = progressService;
            _classSubjectProgressService = classSubjectProgressService;
            _mapping = new MappingEntity<LessonEntity, LessonScheduleViewModel>();
            _mappingList = new MappingEntity<ClassEntity, StudentClassViewModel>();
            _lessonPartQuestionService = lessonPartQuestionService;
            _lessonPartAnswerService = lessonPartAnswerService;
            _studentMapping = new MappingEntity<StudentEntity, ClassStudentViewModel>();
            _activeMapping = new MappingEntity<ClassEntity, ClassActiveViewModel>();
            _calendarHelper = calendarHelper;
            _cacheHelper = cacheHelper;
            _centerService = centerService;
            _newsCategoryService = newsCategoryService;
            _newsService = newsService;

            _lessonPartMapping = new MappingEntity<LessonPartEntity, CloneLessonPartEntity>();
            _lessonPartQuestionMapping = new MappingEntity<LessonPartQuestionEntity, CloneLessonPartQuestionEntity>();
            _lessonPartAnswerMapping = new MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity>();
        }


        [Obsolete]
        [HttpPost]
        public JsonResult GetBestStudents(string basis, int limit = 0)
        {
            var center = _centerService.GetItemByCode(basis);
            if (center == null)
                return Json(new { Err = "Không có dữ liệu" });

            var userId = User.Claims.GetClaimByType("UserID").Value;
            var currentStudent = _studentService.GetItemByID(userId);
            if (center == null)
                return Json(new { Err = "Không có quyền truy cập" });

            var rtn = new List<StudentRankingViewModel>();

            if (currentStudent.JoinedClasses == null)
                return Json(rtn);

            if (limit == 0) limit = 20;
            var current = DateTime.Now.Date;
            var startWeek = current.AddDays(DayOfWeek.Monday - current.DayOfWeek - 7);
            var endWeek = startWeek.AddDays(7);


            var classIDs = _service.CreateQuery().Find(t => currentStudent.JoinedClasses.Contains(t.ID) && (t.Center == center.ID) && (t.EndDate <= DateTime.Now || t.IsActive) && t.ClassMechanism != CLASS_MECHANISM.PERSONAL).Project(t => t.ID).ToList();

            if (classIDs != null && classIDs.Count() > 0)
            {
                foreach (var classid in classIDs)
                {
                    var cacheKey = "GetBestStudents_" + basis + "_" + classid + "_" + startWeek.ToString("yyyyMMdd") + endWeek.ToString("yyyyMMdd") + "_limit_" + limit;
                    List<StudentRankingViewModel> classresult = _cacheHelper.GetCache(cacheKey) as List<StudentRankingViewModel>;
                    if (classresult == null)
                    {
                        var exLessons = _lessonService.CreateQuery().Find(o => o.ClassID == classid && o.StartDate <= endWeek && o.EndDate >= startWeek && (o.TemplateType == 2 || o.IsPractice)).Project(t => new LessonEntity
                        {
                            ID = t.ID,
                            GroupIDs = t.GroupIDs
                        }).ToList();

                        var examIds = exLessons.Select(t => t.ID).ToList();

                        //var activeLessonIds = activeLessons.Select(t => t.ID).ToList();
                        //var examIds = activeLessons.Find(x =>).Project(x => x.ID).ToList();


                        //get groups in class => calculate group lesson count
                        //map student progress with group

                        var classGroups = new List<ClassGroupEntity>();

                        if (examIds.Count() > 0)
                        {
                            var activeProgress = _lessonProgressService.CreateQuery().Find(x => examIds.Contains(x.LessonID) && x.LastDate <= endWeek && x.LastDate >= startWeek && x.Tried > 0).ToList();

                            if (activeProgress.Count() > 0)
                            {
                                var studentResults = (from r in activeProgress
                                                      group r by r.StudentID into g
                                                      let studentGrps = classGroups.Where(t => t.Members.Any(m => m.MemberID == g.Key)).Select(t => t.ID).ToList()
                                                      let exCount = exLessons.Count(e => e.GroupIDs == null || studentGrps == null || studentGrps.Intersect(e.GroupIDs).Any())
                                                      select new StudentRankingViewModel
                                                      {
                                                          StudentID = g.Key,
                                                          AvgPoint = g.Sum(t => t.LastPoint) / exCount,
                                                      }).OrderByDescending(t => t.AvgPoint).Take(limit).ToList();

                                rtn.AddRange(studentResults);
                                if (rtn.Count() > 0)
                                {
                                    foreach (var st in rtn)
                                    {
                                        st.StudentName = _studentService.GetItemByID(st.StudentID)?.FullName;
                                    }
                                }
                                _cacheHelper.SetCache(cacheKey, rtn, endWeek.AddDays(7) - DateTime.Now);
                            }
                        }
                    }
                    if (classresult != null && classresult.Count() > 0)
                        rtn.AddRange(classresult);
                }
            }

            if (rtn.Count() > 0)
                rtn = rtn.OrderByDescending(t => t.AvgPoint).Take(limit).ToList();

            return Json(rtn);

            //var cacheKey = "GetBestStudents_" + basis;
            //var rtn = _cacheHelper.GetCache(cacheKey) as List<StudentRankingViewModel>;
            //if (rtn == null)
            //{
            //    rtn = new List<StudentRankingViewModel>();
            //    var classIDs = _service.CreateQuery().Find(t => t.Center == center.ID && (t.EndDate <= DateTime.Now || t.IsActive)).Project(t => t.ID).ToEnumerable();
            //    var results = _progressService.CreateQuery().Aggregate().Match(t => classIDs.Contains(t.ClassID)).Group(t => t.StudentID, g => new StudentRankingViewModel
            //    {
            //        StudentID = g.Key,
            //        TotalPoint = g.Sum(t => t.TotalPoint),
            //        PracticePoint = g.Sum(t => t.PracticePoint),
            //    }).SortByDescending(s => s.TotalPoint).ThenByDescending(s => s.PracticePoint).Limit(20).ToEnumerable();
            //    foreach (var result in results)
            //    {
            //        var st = _studentService.GetItemByID(result.StudentID);
            //        if (st != null)
            //        {
            //            var firstClassID = classIDs.FirstOrDefault(t => st.JoinedClasses.Contains(t));
            //            result.ClassName = firstClassID != null ? _service.GetItemByID(firstClassID)?.Name : null;
            //            result.StudentName = st.FullName;
            //            rtn.Add(result);
            //        }
            //    }
            //    _cacheHelper.SetCache(cacheKey, rtn);
            //}
            //var response = new Dictionary<string, object>
            //{
            //    { "Data", rtn }
            //};
            //return new JsonResult(response);
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetList(DefaultModel model, ClassSubjectEntity entity, string basis)
        {
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userId))
                return Json(new { Err = "Không được phép truy cập" });


            var filter = new List<FilterDefinition<ClassEntity>>();
            var center = _centerService.GetItemByCode(basis);
            if (center == null)
                return Json(new { Err = "Không được phép truy cập" });
            filter.Add(Builders<ClassEntity>.Filter.Where(o => (o.IsActive && o.Center == center.ID) || o.ClassMechanism == CLASS_MECHANISM.PERSONAL));

            //class filter
            var currentStudent = _studentService.GetItemByID(userId);
            if (currentStudent == null || currentStudent.JoinedClasses == null || currentStudent.JoinedClasses.Count == 0)
                return Json(new { });

            var classIds = new List<string>();

            classIds = currentStudent.JoinedClasses;
            if (classIds == null || classIds.Count() == 0)
                return Json(new { Data = new List<StudentClassViewModel>() });

            //classSubject filter
            var csFilter = new List<FilterDefinition<ClassSubjectEntity>>();
            if (!string.IsNullOrEmpty(entity.GradeID))
            {
                csFilter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.GradeID == entity.GradeID));
            }
            else
            {
                if (!string.IsNullOrEmpty(entity.SubjectID))
                    csFilter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.SubjectID == entity.SubjectID));
            }
            if (csFilter.Count > 0)
            {
                classIds = _classSubjectService.CreateQuery().Find(Builders<ClassSubjectEntity>.Filter.And(csFilter))
                    .Project(t => t.ClassID).ToList()
                    .Distinct()
                    .Where(t => classIds.Contains(t)).ToList();
            }

            filter.Add(Builders<ClassEntity>.Filter.Where(o => classIds.Contains(o.ID)));

            if (!string.IsNullOrEmpty(model.SearchText))
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Name.ToLower().Contains(model.SearchText.ToLower())));
            }

            if (model.StartDate > new DateTime(2000, 1, 1))
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.EndDate >= new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0)));
            }

            if (model.EndDate > new DateTime(2000, 1, 1))
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.StartDate <= new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59)));
            }

            var data = (filter.Count > 0 ? _service.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _service.GetAll()).SortByDescending(t => t.ID);
            model.TotalRecord = data.Count();
            var DataResponse = data == null || data.Count() <= 0 || data.Count() < model.PageSize
                ? data.ToList()
                : data.Skip((model.PageIndex) * model.PageSize).Limit(model.PageSize).ToList();

            var std =
                (from o in DataResponse
                 let progress = _progressService.GetItemByClassID(o.ID, userId)
                 //let course = _courseService.GetItemByID(o.CourseID)
                 //let subject = _subjectService.GetItemByID(o.SubjectID)
                 //let grade = _gradeService.GetItemByID(o.GradeID)
                 let teacher = _teacherService.GetItemByID(o.TeacherID)
                 let complete = progress == null ? 0 : (o.TotalLessons > 0 ? progress.Completed * 100 / o.TotalLessons : 0)
                 select _mappingList.AutoOrtherType(o, new StudentClassViewModel()
                 {
                     StudentNumber = o.Students.Count,
                     TeacherName = teacher == null ? "" : teacher.FullName,
                     Progress = progress,
                     Thumb = string.IsNullOrEmpty(o.Image) ? "/pictures/english1.png" : o.Image,
                     CompletePercent = complete > 100 ? 100 : complete
                 })).ToList();

            var response = new Dictionary<string, object>
            {
                { "Data", std },
                { "Model", model }
            };
            return new JsonResult(response);
        }

        public JsonResult GetActiveList(DateTime today)
        {
            today = today.ToUniversalTime();
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

            var currentStudent = _studentService.GetItemByID(userId);
            if (currentStudent == null || currentStudent.JoinedClasses == null || currentStudent.JoinedClasses.Count == 0)
                return Json(new { });


            filter.Add(Builders<ClassEntity>.Filter.Where(o => currentStudent.JoinedClasses.Contains(o.ID)));
            filter.Add(Builders<ClassEntity>.Filter.Where(o => (o.StartDate <= today) && (o.EndDate >= today)));

            var data = filter.Count > 0 ? _service.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _service.GetAll();

            var std = (from o in data.ToList()
                       let progress = _progressService.GetItemByClassID(o.ID, userId)
                       //let examCount = _lessonScheduleService.CountClassExam(o.ID, end: DateTime.UtcNow)
                       select new
                       {
                           id = o.ID,
                           //courseID = o.CourseID,
                           //courseName = o.Name,
                           //subjectName = _subjectService.GetItemByID(o.SubjectID) == null ? "" : _subjectService.GetItemByID(o.SubjectID).Name,
                           endDate = o.EndDate,
                           percent = (progress == null || o.TotalLessons == 0) ? 0 : progress.Completed * 100 / o.TotalLessons,
                           max = o.TotalLessons,
                           min = progress != null ? progress.Completed : 0,
                           //score = (progress != null && examCount > 0) ? progress.TotalPoint / examCount : 0,
                           thumb = string.IsNullOrEmpty(o.Image) ? "/pictures/english1.png" : o.Image,
                       }).ToList();
            return Json(new { Data = std });
        }

        public JsonResult GetActiveListV2(DateTime today, string basis)
        {
            var center = _centerService.GetItemByCode(basis);
            if (center == null)
                return Json(new { Err = "Không được phép truy cập" });
            //today = today.ToUniversalTime();
            var filter = new List<FilterDefinition<ClassEntity>>();
            filter.Add(Builders<ClassEntity>.Filter.Where(o => o.IsActive && o.Center == center.ID));
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new ReturnJsonModel
                {
                    StatusCode = ReturnStatus.ERROR,
                    StatusDesc = "Authentication Error"
                });
            }

            var currentStudent = _studentService.GetItemByID(userId);
            if (currentStudent == null || currentStudent.JoinedClasses == null || currentStudent.JoinedClasses.Count == 0)
                return Json(new { Data = 0 });

            filter.Add(Builders<ClassEntity>.Filter.Where(o => currentStudent.JoinedClasses.Contains(o.ID)));
            filter.Add(Builders<ClassEntity>.Filter.Where(o => o.ClassMechanism != CLASS_MECHANISM.PERSONAL));
            filter.Add(Builders<ClassEntity>.Filter.Where(o => (o.StartDate <= today) && (o.EndDate >= today)));

            var clIDs = (filter.Count > 0 ? _service.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _service.GetAll()).Project(t => t.ID);
            if (clIDs.Count() == 0)
            {
                return Json(new ReturnJsonModel
                {
                    StatusCode = ReturnStatus.ERROR,
                    StatusDesc = "No class"
                });
            }

            var lstSbj = new List<ClassSubjectEntity>();
            var lstClass = new List<ClassEntity>();
            foreach (var clID in clIDs.ToList())
            {
                lstSbj.AddRange(_classSubjectService.GetByClassID(clID));
                lstClass.Add(_service.GetItemByID(clID));
            }

            var std = (from o in lstSbj.ToList()
                       let _class = lstClass.SingleOrDefault(t => t.ID == o.ClassID)
                       let progress = _classSubjectProgressService.GetItemByClassSubjectID(o.ID, userId)
                       //let examCount = _lessonScheduleService.CountClassExam(o.ID, end: DateTime.UtcNow)
                       //let skill = _skillService.GetItemByID(o.SkillID)
                       let course = _courseService.GetItemByID(o.CourseID)
                       select new
                       {
                           id = o.ID,
                           //courseID = o.CourseID,
                           courseName = course?.Name,
                           startDate = _class.StartDate,
                           endDate = _class.EndDate,
                           percent = (progress == null || o.TotalLessons == 0) ? 0 : progress.Completed * 100 / o.TotalLessons,
                           max = o.TotalLessons,
                           min = progress != null ? progress.Completed : 0,
                           //score = (progress != null && examCount > 0) ? progress.TotalPoint / examCount : 0,
                           thumb = string.IsNullOrEmpty(o.Image) ? course?.Image : o.Image,
                       }).ToList();
            return Json(new { Data = std });
        }

        public JsonResult GetMyCourse(string basis)
        {
            var center = _centerService.GetItemByCode(basis);
            if (center == null)
                return Json(new { Err = "Không được phép truy cập" });

            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new ReturnJsonModel
                {
                    StatusCode = ReturnStatus.ERROR,
                    StatusDesc = "Authentication Error"
                });
            }

            var currentStudent = _studentService.GetItemByID(userId);
            if (currentStudent == null || currentStudent.JoinedClasses == null || currentStudent.JoinedClasses.Count == 0)
                return Json(new { });
            //var filter = new List<FilterDefinition<ClassEntity>>();
            //filter.Add(Builders<ClassEntity>.Filter.Where(o => o.ClassMechanism==CLASS_MECHANISM.PERSONAL && o.TeacherID== currentStudent.ID));

            //filter.Add(Builders<ClassEntity>.Filter.Where(o => currentStudent.JoinedClasses.Contains(o.ID)));
            //filter.Add(Builders<ClassEntity>.Filter.Where(o => (o.StartDate <= today) && (o.EndDate >= today)));

            var clID = _service.GetClassByMechanism(CLASS_MECHANISM.PERSONAL, currentStudent.ID)?.ID;


            var lstSbj = new List<ClassSubjectEntity>();
            var lstClass = new List<ClassEntity>();

            if (!string.IsNullOrEmpty(clID))
            {
                lstSbj.AddRange(_classSubjectService.GetByClassID(clID));
                lstClass.Add(_service.GetItemByID(clID));
            }
            var std = (from o in lstSbj.ToList()
                       let _class = lstClass.SingleOrDefault(t => t.ID == o.ClassID)
                       let progress = _classSubjectProgressService.GetItemByClassSubjectID(o.ID, userId)
                       //let examCount = _lessonScheduleService.CountClassExam(o.ID, end: DateTime.UtcNow)
                       let course = _courseService.GetItemByID(o.CourseID)
                       //let skill = _skillService.GetItemByID(o.SkillID)
                       select new
                       {
                           id = o.ID,
                           courseName = course?.Name,
                           endDate = _class.EndDate,
                           percent = (progress == null || o.TotalLessons == 0) ? 0 : progress.Completed * 100 / o.TotalLessons,
                           max = o.TotalLessons,
                           min = progress != null ? progress.Completed : 0,
                           //score = (progress != null && examCount > 0) ? progress.TotalPoint / examCount : 0
                       }).ToList();
            return Json(new { Data = std });
        }


        public JsonResult GetFinishList(DefaultModel model, DateTime today, string basis)
        {
            var center = _centerService.GetItemByCode(basis);
            if (center == null)
                return Json(new { Err = "Không được phép truy cập" });
            today = today.ToUniversalTime();
            var filter = new List<FilterDefinition<ClassEntity>>();
            filter.Add(Builders<ClassEntity>.Filter.Where(o => o.IsActive && o.Center == center.ID));
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { });
            }

            var currentStudent = _studentService.GetItemByID(userId);
            if (currentStudent == null || currentStudent.JoinedClasses == null || currentStudent.JoinedClasses.Count == 0)
                return Json(new { });

            filter.Add(Builders<ClassEntity>.Filter.Where(o => currentStudent.JoinedClasses.Contains(o.ID)));

            filter.Add(Builders<ClassEntity>.Filter.Where(o => o.EndDate < today));

            var data = filter.Count > 0 ? _service.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _service.GetAll();
            model.TotalRecord = data.CountDocuments();
            var DataResponse = data == null || model.TotalRecord <= 0 || model.TotalRecord < model.PageSize
                ? data.ToList()
                : data.Skip(model.PageIndex * model.PageSize).Limit(model.PageSize).ToList();

            var std = (from o in DataResponse
                       let progress = _progressService.GetItemByClassID(o.ID, userId)
                       let per = (progress == null || o.TotalLessons == 0) ? 0 : progress.Completed * 100 / o.TotalLessons
                       //let examCount = _lessonScheduleService.CountClassExam(o.ID)
                       select new
                       {
                           id = o.ID,
                           //courseID = o.CourseID,
                           title = o.Name,
                           endDate = o.EndDate,
                           per,
                           max = o.TotalLessons,
                           min = progress != null ? progress.Completed : 0,
                           //score = (progress != null && examCount > 0) ? progress.TotalPoint / examCount : 0,
                       }).ToList();
            return Json(new { Data = std });
        }

        public JsonResult GetThisWeekLesson(DateTime today, string basis, bool seekStart = true)
        {
            //try
            //{
            var center = _centerService.GetItemByCode(basis);
            if (center == null)
                return Json(new { Err = "Không được phép truy cập" });
            if (today < new DateTime(1900, 1, 1))
                return Json(new { });
            today = today.ToUniversalTime();
            var startWeek = today;
            if (seekStart)
                startWeek = today.AddDays(DayOfWeek.Sunday - today.DayOfWeek);
            var endWeek = startWeek.AddDays(7);

            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new ReturnJsonModel
                {
                    StatusCode = ReturnStatus.ERROR,
                    StatusDesc = "Authentication Error"
                });
            }
            var currentStudent = _studentService.GetItemByID(userId);

            if (currentStudent == null || currentStudent.JoinedClasses == null || currentStudent.JoinedClasses.Count == 0)
                return Json(new { });

            var @class = _service.GetItemsByIDs(currentStudent.JoinedClasses, center.ID).Where(x => x.IsActive && x.EndDate >= DateTime.Now).ToList();
            var classids = @class.Select(t => t.ID).ToList();
            //var classids = _service.GetItemsByIDs(currentStudent.JoinedClasses, center.ID).Select(t => t.ID).ToList();

            if (classids == null || classids.Count == 0)
                return Json(new { });

            List<StudentLessonScheduleViewModel> listSchedule = new List<StudentLessonScheduleViewModel>();

            var studentGroups = _classGroupService.GetByClassIDs(classids).Where(t => t.Members.Any(m => m.MemberID == userId)).Select(t => t.ID).ToList();


            var lessons = GetActiveClassLesson(startWeek, endWeek, classids);
            if (studentGroups == null || studentGroups.Count == 0)
                listSchedule = ConvertFromLessonToScheduleView(lessons.Where(t => t.GroupIDs == null));
            else
                listSchedule = ConvertFromLessonToScheduleView(lessons.Where(t => t.GroupIDs == null || studentGroups.Intersect(t.GroupIDs).Any()));

            if (listSchedule != null && listSchedule.Count > 0)
                foreach (var schedule in listSchedule)
                {
                    var progress = _lessonProgressService.GetByStudentID_LessonID(userId, schedule.lessonID);

                    schedule.isLearnt = progress == null ? false : true;
                    schedule.lastPoint = (progress != null && progress.Tried > 0) ? progress.LastPoint : -1;
                    var lesson = _lessonService.GetItemByID(schedule.lessonID);
                    schedule.chapterName = lesson.ChapterID == "0" ? "" : _chapterService.GetItemByID(lesson.ChapterID).Name;
                    //_learningHistoryService.GetLastLearnt(userId, schedule.lessonID, schedule.classSubjectID) != null;
                }

            return Json(new { Data = listSchedule });

        }

        private IEnumerable<LessonEntity> GetActiveClassLesson(DateTime start, DateTime end, List<string> classIDs)
        {
            var filter = new List<FilterDefinition<LessonEntity>>();
            filter.Add(Builders<LessonEntity>.Filter.Where(o => o.StartDate <= end && o.EndDate >= start));
            filter.Add(Builders<LessonEntity>.Filter.Where(o => classIDs.Contains(o.ClassID)));

            return _lessonService.Collection.Find(Builders<LessonEntity>.Filter.And(filter)).ToEnumerable();
        }

        private List<StudentLessonScheduleViewModel> ConvertFromLessonToScheduleView(IEnumerable<LessonEntity> data)
        {
            return (from o in data
                    let _class = _service.Collection.Find(t => t.ID == o.ClassID).SingleOrDefault()
                    where _class != null
                    let _cs = _classSubjectService.Collection.Find(t => t.ID == o.ClassSubjectID).SingleOrDefault()
                    where _cs != null
                    let skill = _skillService.GetItemByID(_cs.SkillID)
                    let _subject = _subjectService.Collection.Find(t => t.ID == _cs.SubjectID).SingleOrDefault()
                    //where _subject != null
                    let lessonCalendar = _calendarHelper.GetByScheduleId(o.ID)
                    let onlineUrl = (o.IsOnline && lessonCalendar != null) ? lessonCalendar.UrlRoom : ""
                    select new StudentLessonScheduleViewModel
                    {
                        id = o.ID,
                        classID = _class.ID,
                        className = _class.Name,
                        classSubjectID = _cs.ID,
                        subjectName = _subject == null ? "" : _subject.Name,
                        title = o.Title,
                        lessonID = o.ID,
                        startDate = o.StartDate,
                        endDate = o.EndDate,
                        skill = skill,
                        type = o.TemplateType,
                        onlineUrl = o.IsOnline ? onlineUrl : "",
                        bookName = _cs.CourseName
                    }).ToList();
        }

        //temp not use
        private List<StudentLessonScheduleViewModel> GetThisWeekClassLesson(DateTime startWeek, DateTime endWeek, string classid)
        {
            var cacheKey = "classschedule_" + classid;
            var cacheClass = _cacheHelper.GetCache(cacheKey) as List<StudentLessonScheduleViewModel>;
            if (cacheClass == null)
            {
                var data = GetActiveClassLesson(startWeek, endWeek, new List<string> { classid }).ToList();

                if (data.Count > 0)
                    cacheClass = ConvertFromLessonToScheduleView(data).OrderBy(t => t.startDate).ToList();
                else
                    cacheClass = new List<StudentLessonScheduleViewModel>();

                var expireDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local);
                _cacheHelper.SetCache(cacheKey, cacheClass, new DistributedCacheEntryOptions { AbsoluteExpiration = expireDate });
            }
            return cacheClass;
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetListCompact(DefaultModel model, string TeacherID)
        {
            var filter = new List<FilterDefinition<ClassEntity>>();
            filter.Add(Builders<ClassEntity>.Filter.Where(o => o.IsActive));
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { });
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
            var data = filter.Count > 0 ? _service.Collection.Find(Builders<ClassEntity>.Filter.And(filter)).Project(t => t.ID).ToList() : _service.GetAll().Project(t => t.ID).ToList();
            model.TotalRecord = data.Count();
            var DataResponse = data == null || data.Count() <= 0 || data.Count() < model.PageSize
                ? data.ToList()
                : data.Skip((model.PageIndex - 1) * model.PageSize).Take(model.PageSize).ToList();

            var response = new Dictionary<string, object>
            {
                { "Data", data },
                { "Model", model }
            };
            return new JsonResult(response);
        }

        //[System.Obsolete]
        //[HttpPost]
        //public JsonResult GetDetail(string CourseID, string ClassID)
        //{
        //    try
        //    {
        //        var userId = User.Claims.GetClaimByType("UserID").Value;
        //        if (string.IsNullOrEmpty(userId))
        //        {
        //            return Json(new { });
        //        }

        //        var filterSchedule = Builders<LessonEntity>.Filter.Where(o => o.ClassID == ClassID);
        //        var dataSchedule = _lessonService.Collection.Find(filterSchedule);
        //        if (dataSchedule == null || dataSchedule.Count() <= 0) return Json(new { });
        //        var schedules = dataSchedule.ToEnumerable();
        //        var filter = new List<FilterDefinition<LessonEntity>>();
        //        filter.Add(Builders<LessonEntity>.Filter.Where(o => o.CourseID == CourseID));
        //        //filter.Add(Builders<LessonEntity>.Filter.Where(o=> schedules.Select(x => x.LessonID).Contains(o.ID)));
        //        var data = _lessonService.Collection.Find(Builders<LessonEntity>.Filter.And(filter));
        //        var DataResponse = data == null || data.Count() <= 0 ? null : data.ToList();

        //        var response = new Dictionary<string, object>
        //        {
        //            { "Data",
        //                DataResponse.Select(
        //                o=> _mapping.AutoOrtherType(o,new LessonScheduleViewModel(){
        //                        IsActive = _lessonScheduleService.GetItemByID(ClassID) == null ? false: _lessonScheduleService.GetItemByID(ClassID).IsActive,
        //                        StartDate = _lessonScheduleService.GetItemByID(ClassID) == null ?DateTime.MinValue :  _lessonScheduleService.GetItemByID(ClassID).StartDate,
        //                        EndDate = _lessonScheduleService.GetItemByID(ClassID) == null ? DateTime.MinValue : _lessonScheduleService.GetItemByID(ClassID).EndDate,
        //                        IsView = _learningHistoryService.CreateQuery().Count(x=>x.StudentID == userId && x.LessonID == o.ID && x.ClassID == ClassID) > 0
        //                    })
        //                )
        //            }
        //        };
        //        return new JsonResult(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        var response = new Dictionary<string, object>
        //        {
        //            { "Data", null },
        //            {"Error",ex }
        //        };
        //        return new JsonResult(response);
        //    }
        //}

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
            //var totalLessons = _lessonScheduleService.CreateQuery().CountDocuments(o => o.ClassID == currentClass.ID);
            var studentsView = (from r in students.ToList()
                                    //let learned = _learningHistoryService.CreateQuery().AsQueryable()
                                    //.Where(t => t.StudentID == r.ID && t.ClassID == currentClass.ID)
                                    //.GroupBy(t => new { t.StudentID, t.ClassID, t.LessonID }).Count()
                                select _studentMapping.AutoOrtherType(r, new ClassStudentViewModel()
                                {
                                    ClassName = currentClass.Name,
                                    ClassStatus = "Đang học",
                                    Progress = _progressService.GetItemByClassID(currentClass.ID, r.ID),
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

        //Chỉ dùng cho sidebar cũ
        //public JsonResult GetActiveList()
        //{
        //    var filter = new List<FilterDefinition<ClassEntity>>();
        //    var UserID = User.Claims.GetClaimByType("UserID").Value;

        //    filter.Add(Builders<ClassEntity>.Filter.Where(o => o.IsActive && o.Students.Contains(UserID) && o.EndDate >= DateTime.UtcNow.ToLocalTime().Date));

        //    var activeClasses = _service.Collection.Find(Builders<ClassEntity>.Filter.And(filter)).ToList();

        //    var responseData = new List<ClassActiveViewModel>();

        //    var subjects = _subjectService.GetAll().ToList();

        //    var _activeMapping = new MappingEntity<ClassEntity, ClassActiveViewModel>();

        //    responseData =
        //        (from r in activeClasses.ToList()
        //         let progress = _progressService.GetItemByClassID(r.ID, UserID)
        //         select _activeMapping.AutoOrtherType(r, new ClassActiveViewModel()
        //         {
        //             Progress = progress == null ? 0 : (progress.TotalLessons != 0 ? progress.CompletedLessons.Count * 100 / progress.TotalLessons : 0),
        //             SubjectName = subjects.SingleOrDefault(s => s.ID == r.SubjectID).Name
        //         })).ToList();


        //    //foreach (var _class in activeClasses)
        //    //{
        //    //    var totalLessons = _lessonScheduleService.CreateQuery().CountDocuments(o => o.ClassID == _class.ID);
        //    //    var learned = _learningHistoryService.CreateQuery().Aggregate().Match(o => o.ClassID == _class.ID && o.StudentID ).Group(o => o.LessonID,
        //    //        o => new { x = o.Select(x => 1).First() }).ToList().Count();
        //    //    responseData.Add(_activeMapping.AutoOrtherType(_class, new ClassActiveViewModel()
        //    //    {
        //    //        Progress = (int)(totalLessons != 0 ? learned * 100 / totalLessons : 0),
        //    //        SubjectName = subjects.SingleOrDefault(s => s.ID == _class.SubjectID).Name
        //    //    }));
        //    //}

        //    var response = new Dictionary<string, object>
        //    {
        //        { "Data", responseData }
        //    };
        //    return new JsonResult(response);
        //}

        public IActionResult Index(DefaultModel model, string basis)
        {
            ViewBag.Subjects = _subjectService.GetAll().ToList();
            ViewBag.Grades = _gradeService.GetAll().ToList();
            ViewBag.Model = model;
            if (!string.IsNullOrEmpty(basis))
            {
                var center = _centerService.GetItemByCode(basis);
                if (center != null)
                {
                    ViewBag.Center = center;
                }
            }
            return View();
        }

        /*
         * Xem các khóa học khác
         */
        public IActionResult OtherCourse(string basis)
        {
            var centerID = "";
            if (!string.IsNullOrEmpty(basis))
            {
                var center = _centerService.GetItemByCode(basis);
                if (center != null)
                {
                    ViewBag.Center = center;
                    centerID = center.ID;
                }
            }

            var data = _newsService.CreateQuery().Find(o => o.CenterID == centerID && o.IsActive == true && o.Type == "san-pham").ToList();
            ViewBag.List_Courses = data;
            ViewBag.Subjects = _subjectService.GetAll().ToList();
            ViewBag.Grades = _gradeService.GetAll().ToList();
            return View();
        }

        //public JsonResult GetListProduct()
        //{

        //}

        //public IActionResult Calendar(DefaultModel model, string id, string ClassID)
        //{
        //    if (string.IsNullOrEmpty(id))
        //    {
        //        TempData["Error"] = "Bạn chưa chọn khóa học";
        //        return Redirect($"/{basis}{Url.Action("Index");
        //    }
        //    ViewBag.CourseID = id;
        //    ViewBag.ClassID = ClassID;
        //    ViewBag.Model = model;
        //    return View();
        //}

        public IActionResult Detail(string id, string basis)
        {
            return Redirect(Url.Action("Index", "Course"));
            //var currentClass = _service.GetItemByID(id);
            //var userId = User.Claims.GetClaimByType("UserID").Value;
            //if (currentClass == null)
            //    return Redirect($"/{basis}{Url.Action("Index", "Course")}");
            ////var classStudent = _classStudentService.GetClassStudent(currentClass.ID, userId);
            //if (!_studentService.IsStudentInClass(currentClass.ID, userId))
            //    return Redirect($"/{basis}{Url.Action("Index")}");
            //var vm = new ClassViewModel(currentClass);
            //var subjects = _classSubjectService.GetByClassID(currentClass.ID);
            //var skillIDs = subjects.Select(t => t.SkillID).Distinct();
            //var subjectIDs = subjects.Select(t => t.SubjectID).Distinct();
            //vm.SkillName = string.Join(", ", _skillService.GetList().Where(t => skillIDs.Contains(t.ID)).Select(t => t.Name).ToList());
            //vm.SubjectName = string.Join(", ", _subjectService.Collection.Find(t => subjectIDs.Contains(t.ID)).Project(t => t.Name).ToList());
            //ViewBag.Class = vm;
            //ViewBag.UserID = userId;
            //return View();
        }

        //public IActionResult Syllabus(DefaultModel model, string id)
        //{
        //    if (model == null) return null;
        //    var currentClass = _service.GetItemByID(id);
        //    var userId = User.Claims.GetClaimByType("UserID").Value;
        //    if (currentClass == null)
        //        return Redirect($"/{basis}{Url.Action("Index");
        //    if (currentClass.Students.IndexOf(userId) < 0)
        //        return Redirect($"/{basis}{Url.Action("Index");
        //    ViewBag.Class = currentClass;
        //    return View();
        //}

        public IActionResult Modules(string basis, string id, int old = 0)
        {
            //if (model == null) return null;
            var currentCs = _classSubjectService.GetItemByID(id);
            if (currentCs == null)
                return Redirect($"/{basis}{Url.Action("Index", "Course")}");
            var userId = User.Claims.GetClaimByType("UserID").Value;
            var currentClass = _service.GetItemByID(currentCs.ClassID);
            if (currentClass == null)
                return Redirect($"/{basis}{Url.Action("Index", "Course")}");
            //var classStudent = _classStudentService.GetClassStudent(currentClass.ID, userId);
            //if (classStudent == null)
            if (!_studentService.IsStudentInClass(currentClass.ID, userId))
                return Redirect($"/{basis}{Url.Action("Index", "Course")}");
            var progress = _classSubjectProgressService.GetItemByClassSubjectID(id, userId);
            //long completed = 0;
            //if (progress != null && progress.TotalLessons > 0)
            //    completed = progress.Completed;
            //var subject = _subjectService.GetItemByID(currentCs.SubjectID);
            //if (subject == null)
            //    return Redirect($"/{basis}{Url.Action("Index", "Course")}");
            //ViewBag.Completed = completed;
            ViewBag.ClassSubject = new ClassSubjectViewModel()
            {
                ID = currentCs.ID,
                //Name = subject.Name,
                CourseID = currentCs.CourseID,
                ClassID = currentClass.ID,
                ClassName = currentClass.Name,
                CourseName = string.IsNullOrEmpty(currentCs.CourseName) ? _courseService.GetItemByID(currentCs.CourseID)?.Name : currentCs.CourseName,
                //SkillName = _skillService.GetItemByID(currentCs.SkillID).Name,
                CompletedLesssons = progress == null ? 0 : progress.Completed,
                TotalLessons = currentCs.TotalLessons,
            };
            if (old == 1) return View("Modules_o");
            return View();
        }

        //public IActionResult Assignment(DefaultModel model, string id)
        //{
        //    if (model == null) return null;
        //    var currentClass = _service.GetItemByID(id);
        //    var userId = User.Claims.GetClaimByType("UserID").Value;
        //    if (currentClass == null)
        //        return Redirect($"/{basis}{Url.Action("Index");
        //    //var classStudent = _classStudentService.GetClassStudent(currentClass.ID, userId);
        //    //if (classStudent == null)
        //    //    return Redirect($"/{basis}{Url.Action("Index");
        //    if (!_studentService.IsStudentInClass(currentClass.ID, userId))
        //        return Redirect($"/{basis}{Url.Action("Index");
        //    ViewBag.Class = currentClass;
        //    return View();
        //}

        //public IActionResult References(DefaultModel model, string id)
        //{
        //    if (model == null) return null;
        //    var currentClass = _service.GetItemByID(id);
        //    var userId = User.Claims.GetClaimByType("UserID").Value;
        //    if (currentClass == null)
        //        return Redirect($"/{basis}{Url.Action("Index");
        //    if (!_studentService.IsStudentInClass(currentClass.ID, userId))
        //        return Redirect($"/{basis}{Url.Action("Index");
        //    ViewBag.Class = currentClass;
        //    return View();
        //}

        //public IActionResult Discussions(DefaultModel model)
        //{
        //    if (model == null) return null;
        //    var currentClass = _service.GetItemByID(model.ID);
        //    if (currentClass == null)
        //        return Redirect($"/{basis}{Url.Action("Index");
        //    ViewBag.Class = currentClass;
        //    return View();
        //}

        [HttpPost]
        public JsonResult GetMainChapters(string ID)
        {
            try
            {
                var userId = User.Claims.GetClaimByType("UserID").Value;

                var currentCs = _classSubjectService.GetItemByID(ID);
                if (currentCs == null)
                    return new JsonResult(new Dictionary<string, object>
                    {
                        {"Error", "Không tìm thấy môn học" }
                    });

                var chapters = _chapterService.GetSubChapters(currentCs.ID, "0");
                //var chapterExtends = _chapterExtendService.Search(currentCs.ID);

                var listProgress = new ClassScheduleViewModel(new CourseEntity())
                {

                    Chapters = new List<ChapterEntity>(),
                    Lessons = new List<LessonScheduleViewModel>()
                };

                foreach (var chapter in chapters)
                {
                    //var extend = chapterExtends.SingleOrDefault(t => t.ChapterID == chapter.ID);
                    var progress = _chapterProgressService.GetItemByChapterID(chapter.ID, userId);
                    //if (extend != null) chapter.Description = extend.Description;
                    var viewModel = new MappingEntity<ChapterEntity, ChapterProgressViewModel>().AutoOrtherType(chapter, new ChapterProgressViewModel());
                    viewModel.TotalLessons = chapter.TotalLessons;
                    if (progress != null)
                    {
                        viewModel.TotalLessons = chapter.TotalLessons;
                        viewModel.CompletedLessons = progress.Completed;
                        viewModel.LastDate = progress.LastDate;
                        viewModel.LastLessonID = progress.LastLessonID;
                        viewModel.PracticeAvgPoint = chapter.PracticeCount > 0 ? progress.PracticePoint / chapter.PracticeCount : 0;
                        //progress.PracticeAvgPoint;
                    }
                    listProgress.Chapters.Add(viewModel);
                }
                var _schedulemapping = new MappingEntity<LessonEntity, LessonScheduleViewModel>();

                listProgress.Lessons = (from r in _lessonService.CreateQuery().Find(o => o.ClassSubjectID == currentCs.ID && o.ChapterID == "0").SortBy(o => o.ChapterID).ThenBy(o => o.Order).ThenBy(o => o.ID).ToList()
                                            //let schedule = _lessonScheduleService.CreateQuery().Find(o => o.LessonID == r.ID && o.ClassSubjectID == ID).FirstOrDefault()
                                        let lastjoin = _learningHistoryService.CreateQuery().Find(x => x.StudentID == userId && x.LessonID == r.ID && x.ClassSubjectID == ID).SortByDescending(o => o.ID).FirstOrDefault()
                                        let lastexam = r.TemplateType == LESSON_TEMPLATE.EXAM ? _examService.CreateQuery().Find(x => x.StudentID == userId && x.LessonID == r.ID && x.ClassSubjectID == ID
                                        //&& x.Status
                                        ).SortByDescending(o => o.ID).FirstOrDefault() : null //get lastest exam
                                        select _schedulemapping.AutoOrtherType(r, new LessonScheduleViewModel()
                                        {
                                            ScheduleID = r.ID,
                                            StartDate = r.StartDate,
                                            EndDate = r.EndDate,
                                            IsActive = r.IsActive,
                                            IsView = r.TemplateType == LESSON_TEMPLATE.EXAM ? lastexam != null : lastjoin != null,
                                            LastJoin = r.TemplateType == LESSON_TEMPLATE.EXAM ? (lastexam != null ? lastexam.Updated : DateTime.MinValue) :
                                                 lastjoin != null ? lastjoin.Time : DateTime.MinValue,
                                            DoPoint = (lastexam != null && lastexam.Status) ? (lastexam.MaxPoint > 0 ? lastexam.Point * 100 / lastexam.MaxPoint : 0) : 0,//completed exam only
                                            Tried = lastexam != null ? lastexam.Number : 0,
                                            LastExam = (lastexam != null && lastexam.Status
                                            ) ? lastexam.ID : null
                                        })).ToList();

                var response = new Dictionary<string, object>
                {
                    { "Data", listProgress }
                };

                return new JsonResult(response);
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

        //TODO: FIX DATA ONLY
        //public JsonResult FixProgress()
        //{
        //    var lhs = _learningHistoryService.Collection.Find(t => true).ToList();
        //    foreach (var lh in lhs)
        //    {
        //        _ = _learningHistoryService.CreateHist(lh);
        //    }
        //    var exams = _examService.GetAll().ToList();
        //    foreach (var exam in exams)
        //    {
        //        var lesson = _lessonService.GetItemByID(exam.LessonID);
        //        if (lesson == null)
        //            _ = _examService.RemoveAsync(exam.ID);
        //        else
        //            _ = _examService.Complete(exam, lesson, out _);
        //    }
        //    return Json("Fixed");
        //}

        #region Learningoutcomes
        public IActionResult Learningoutcomes(String basis)
        {
            var userId = User.Claims.GetClaimByType("UserID").Value;
            var student = _studentService.GetItemByID(userId);
            var center = _centerService.GetItemByCode(basis);
            //var lclass = _service.GetItemsByIDs(student.JoinedClasses).Where(x => x.ClassMechanism != CLASS_MECHANISM.PERSONAL).ToList();
            var lclass = _service.GetItemsByIDs(student.JoinedClasses).Where(t => (t.Center == center.ID && t.EndDate >= DateTime.UtcNow) || (t.ClassMechanism == CLASS_MECHANISM.PERSONAL)).OrderBy(t => t.ClassMechanism).ThenByDescending(t => t.StartDate).AsEnumerable();
            if (lclass.Count() == 0)
            {
                return Json("lclass = 0");
            }

            lclass = lclass.ToList().Where(x => x.ClassMechanism != CLASS_MECHANISM.PERSONAL).ToList();
            ViewBag.ListClass = lclass;
            ViewBag.CurrentAccount = student;
            return View("Learningoutcomes");
        }
        #endregion
    }
}
