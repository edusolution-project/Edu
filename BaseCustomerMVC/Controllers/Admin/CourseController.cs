using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Core_v2.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Core_v2.Globals;
using System.Threading.Tasks;
using BaseEasyRealTime.Entities;

namespace BaseCustomerMVC.Controllers.Admin
{
    [BaseAccess.Attribule.AccessCtrl("Quản lý lớp", "admin", 5)]
    public class CourseController : AdminController
    {
        private readonly CourseService _courseService;
        private readonly SubjectService _subjectService;
        private readonly GradeService _gradeService;
        private readonly ClassService _service;
        private readonly SkillService _skillService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly TeacherService _teacherService;
        private readonly StudentService _studentService;
        private readonly LessonService _lessonService;
        private readonly LearningHistoryService _learningHistoryService;
        private readonly ProgressHelper _progressHelper;
        private readonly CalendarService _calendarService;

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

        private readonly LessonScheduleService _lessonScheduleService;
        private readonly CalendarHelper _calendarHelper;
        //private readonly ClassStudentService _classStudentService;
        private readonly IHostingEnvironment _env;

        private readonly LessonHelper _lessonHelper;
        private readonly CenterService _centerService;
        private readonly AccountService _accountService;
        private readonly MailHelper _mailHelper;
        private readonly ChapterService _chapterService;
        private readonly CourseHelper _courseHelper;
        private readonly ClassProgressService _classProgressService;
        private readonly GroupService _groupService;
        private readonly TeacherHelper _teacherHelper;

        private readonly RoleService _roleService;

        public CourseController(ClassService service,
            ClassSubjectService classSubjectService,
            SubjectService subjectService,
            GradeService gradeService,
            CourseService courseService,
            TeacherService teacherService,
            StudentService studentService,
            SkillService skillService,
            //ClassStudentService classStudentService,
            LessonService lessonService,
            LessonScheduleService lessonScheduleService,
            ExamService examService,
            ExamDetailService examDetailService,

            LessonPartService lessonPartService,
            LessonPartQuestionService lessonPartQuestionService,
            LessonPartAnswerService lessonPartAnswerService,
            LearningHistoryService learningHistoryService,
            ProgressHelper progressHelper,

            CloneLessonPartService cloneLessonPartService,
            CloneLessonPartAnswerService cloneLessonPartAnswerService,
            CloneLessonPartQuestionService cloneLessonPartQuestionService,
            FileProcess fileProcess,
            CalendarHelper calendarHelper,
            CenterService centerService,
            AccountService accountService,
            MailHelper mailHelper,
            ChapterService chapterService,
            CourseHelper courseHelper,
            LessonHelper lessonHelper,
            ClassProgressService classProgressService,
            GroupService groupService,
            RoleService roleService,
            TeacherHelper teacherHelper,
            IHostingEnvironment evn)
        {
            _service = service;
            _classSubjectService = classSubjectService;
            _courseService = courseService;
            _gradeService = gradeService;
            _skillService = skillService;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _studentService = studentService;
            _lessonService = lessonService;
            _lessonScheduleService = lessonScheduleService;
            _examService = examService;
            _examDetailService = examDetailService;
            _learningHistoryService = learningHistoryService;

            _calendarHelper = calendarHelper;

            _fileProcess = fileProcess;

            _lessonHelper = lessonHelper;
            _roleService = roleService;

            _progressHelper = progressHelper;

            _centerService = centerService;
            _accountService = accountService;
            _mailHelper = mailHelper;
            _chapterService = chapterService;
            _courseHelper = courseHelper;
            _classProgressService = classProgressService;
            _groupService = groupService;
            _teacherHelper = teacherHelper;
            _env = evn;
        }

        public ActionResult Index(DefaultModel model)
        {
            ViewBag.Course = _courseService.GetAll()?.ToList();
            ViewBag.Subject = _subjectService.GetAll()?.ToList();
            ViewBag.Grade = _gradeService.GetAll()?.ToList();
            ViewBag.Teacher = _teacherService.GetAll()?.ToList();
            ViewBag.Centers = _centerService.GetAll().ToList();
            ViewBag.Model = model;
            return View();
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetList(DefaultModel model, string CenterID = "", string SubjectID = "", string GradeID = "")
        {
            var returndata = FilterClass(model, CenterID, SubjectID, GradeID);

            var response = new Dictionary<string, object>
                {
                    { "Data", returndata},
                    { "Model", model }
                };
            return new JsonResult(response);
        }

        private List<Dictionary<string, object>> FilterClass(DefaultModel model, string Center = "", string SubjectID = "", string GradeID = "")
        {
            model.TotalRecord = 0;
            var filter = new List<FilterDefinition<ClassSubjectEntity>>();
            var classfilter = new List<FilterDefinition<ClassEntity>>();

            if (!string.IsNullOrEmpty(SubjectID))
            {
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.SubjectID == SubjectID));
            }

            if (!string.IsNullOrEmpty(GradeID))
            {
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.GradeID == GradeID));
            }

            var data = new List<string>();
            if (filter.Count > 0)
            {
                var dCursor = _classSubjectService.Collection
                .Distinct(t => t.ClassID, Builders<ClassSubjectEntity>.Filter.And(filter));
                data = dCursor.ToList();
            }

            if (!string.IsNullOrEmpty(Center))
            {
                classfilter.Add(Builders<ClassEntity>.Filter.Where(o => o.Center == Center));
            }

            if (data.Count > 0)
                classfilter.Add(Builders<ClassEntity>.Filter.Where(t => data.Contains(t.ID)));

            if (!string.IsNullOrEmpty(model.SearchText))
                classfilter.Add(Builders<ClassEntity>.Filter.Text(
                    //"\"" + 
                    model.SearchText
                    //+ "\""
                    ));

            if (classfilter.Count == 0)
                return null;

            var classResult = _service.Collection.Find(
                Builders<ClassEntity>.Filter.And(
                    Builders<ClassEntity>.Filter.Where(o => o.ClassMechanism != CLASS_MECHANISM.PERSONAL),
                    Builders<ClassEntity>.Filter.And(classfilter)
                ));

            model.TotalRecord = classResult.CountDocuments();

            var classData = classResult.SortByDescending(t => t.IsActive).ThenByDescending(t => t.StartDate).Skip(model.PageIndex * model.PageSize).Limit(model.PageSize).ToList();
            var returndata = from o in classData
                             let creator = _teacherService.GetItemByID(o.TeacherID) //Todo: Fix
                             select new Dictionary<string, object>
                                {
                                 { "ID", o.ID },
                                 { "Name", o.Name },
                                 { "Students",
                                     _studentService.CountByClass(o.ID)
                                 },
                                 { "Created", o.Created },
                                 { "IsActive", o.IsActive },
                                 { "Image", o.Image },
                                 { "StartDate", o.StartDate },
                                 { "EndDate", o.EndDate },
                                 { "Order", o.Order },
                                 { "Skills", o.Skills },
                                 { "Members", o.Members },
                                 { "Description", o.Description },
                                 { "CreatorName", creator?.FullName },
                                 { "ClassMechanism", o.ClassMechanism }
                             };
            return returndata.ToList();
        }



        //private List<Dictionary<string, object>> FilterClass(DefaultModel model, string SubjectID = "", string GradeID = "", string TeacherID = "", bool skipActive = true)
        //{
        //    var filter = new List<FilterDefinition<ClassSubjectEntity>>();
        //    var classfilter = new List<FilterDefinition<ClassEntity>>();
        //    FilterDefinition<ClassEntity> ownerfilter = null;

        //    if (!string.IsNullOrEmpty(SubjectID))
        //    {
        //        filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.SubjectID == SubjectID));
        //    }
        //    else
        //    {
        //        var UserID = User.Claims.GetClaimByType("UserID").Value;
        //        var teacher = _teacherService.GetItemByID(UserID);
        //        if (teacher == null)
        //            return null;
        //        filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => teacher.Subjects.Contains(o.SubjectID)));
        //    }
        //    if (!string.IsNullOrEmpty(GradeID))
        //    {
        //        filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.GradeID == GradeID));
        //    }
        //    if (!string.IsNullOrEmpty(TeacherID))
        //    {
        //        filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.TeacherID == TeacherID));
        //        if (string.IsNullOrEmpty(SubjectID) && string.IsNullOrEmpty(GradeID))
        //            ownerfilter = new FilterDefinitionBuilder<ClassEntity>().Where(o => o.TeacherID == TeacherID);
        //    }
        //    if (model.StartDate > new DateTime(1900, 1, 1))
        //        filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.EndDate >= model.StartDate));
        //    if (model.StartDate > new DateTime(1900, 1, 1))
        //        filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.StartDate <= model.EndDate));


        //    var data = _classSubjectService.Collection
        //        .Distinct(t => t.ClassID, filter.Count > 0 ? Builders<ClassSubjectEntity>.Filter.And(filter) : Builders<ClassSubjectEntity>.Filter.Empty).ToList();
        //    //filter by classsubject
        //    if (data.Count > 0)
        //    {
        //        if (ownerfilter != null)
        //            classfilter.Add(
        //                Builders<ClassEntity>.Filter.Or(ownerfilter,
        //                Builders<ClassEntity>.Filter.Where(t => data.Contains(t.ID) && (t.IsActive || skipActive))));
        //        else
        //            classfilter.Add(Builders<ClassEntity>.Filter.Where(t => data.Contains(t.ID) && (t.IsActive || skipActive)));
        //    }

        //    if (!string.IsNullOrEmpty(model.SearchText))
        //        classfilter.Add(Builders<ClassEntity>.Filter.Text("\"" + model.SearchText + "\""));


        //    var classResult = _service.Collection.Find(Builders<ClassEntity>.Filter.And(classfilter));
        //    model.TotalRecord = classResult.CountDocuments();
        //    var classData = classResult.SortByDescending(t => t.IsActive).ThenByDescending(t => t.StartDate).Skip(model.PageIndex * model.PageSize).Limit(model.PageSize).ToList();
        //    var returndata = from o in classData
        //                     let skillIDs = _classSubjectService.GetByClassID(o.ID).Select(t => t.SkillID).Distinct()
        //                     let creator = _teacherService.GetItemByID(o.TeacherID) //Todo: Fix
        //                     let sname = skillIDs == null ? "" : string.Join(", ", _skillService.GetList().Where(t => skillIDs.Contains(t.ID)).Select(t => t.Name).ToList())
        //                     select new Dictionary<string, object>
        //                        {
        //                         { "ID", o.ID },
        //                         { "Name", o.Name },
        //                         { "Students",
        //                             _studentService.CountByClass(o.ID)
        //                             /*_classStudentService.GetClassStudents(o.ID).Count*/ },
        //                         { "Created", o.Created },
        //                         { "IsActive", o.IsActive },
        //                         { "Image", o.Image },
        //                         { "StartDate", o.StartDate },
        //                         { "EndDate", o.EndDate },
        //                         { "Order", o.Order },
        //                         { "Skills", o.Skills },
        //                         { "Members", o.Members },
        //                         { "Description", o.Description },
        //                         { "SkillName", sname },
        //                         { "Creator", o.TeacherID },
        //                         { "CreatorName", creator.FullName }
        //                     };
        //    return returndata.ToList();
        //}


        [Obsolete]
        [HttpPost]
        public JsonResult GetCourse(string GradeID, string SubjectID)
        {
            var filter = new List<FilterDefinition<CourseEntity>>();
            if (!string.IsNullOrEmpty(GradeID))
            {
                filter.Add(Builders<CourseEntity>.Filter.Where(o => o.GradeID == GradeID));
            }
            if (!string.IsNullOrEmpty(SubjectID))
            {
                filter.Add(Builders<CourseEntity>.Filter.Where(o => o.SubjectID == SubjectID));
            }
            filter.Add(Builders<CourseEntity>.Filter.Where(o => o.IsActive == true));
            var data = filter.Count > 0 ? _courseService.Collection.Find(Builders<CourseEntity>.Filter.And(filter)) : _courseService.GetAll();

            var DataResponse = data.ToList();
            return new JsonResult(DataResponse);
        }

        [System.Obsolete]
        [HttpPost]
        public JsonResult GetDetails(string id)
        {
            var filter = Builders<ClassEntity>.Filter.Where(o => o.ID == id);
            var data = _service.Collection.Find(filter);
            var DataResponse = data == null || data.Count() <= 0 ? null : data.First();
            var response = new Dictionary<string, object>
            {
                { "Data", DataResponse }
            };
            return new JsonResult(response);

        }

        [HttpPost]
        [Obsolete]
        public async Task<JsonResult> Clone(string ID, string CenterCode, string Name)
        {

            var orgClass = _service.GetItemByID(ID);
            if (orgClass == null)
                return new JsonResult(new Dictionary<string, object>()
                {
                    {"Error","Lớp không tồn tại"}
                });

            var center = _centerService.GetItemByCode(CenterCode);
            if (center == null)
                return new JsonResult(new Dictionary<string, object>()
                {
                    {"Error","Cơ sở không tồn tại"}
                });

            var headTeacherRole = _roleService.GetItemByCode("head-teacher");

            //get head-teacher of new center
            var hc = _teacherService.CreateQuery().Find(t => t.Centers.Any(c => c.CenterID == center.ID && c.RoleID == headTeacherRole.ID) && t.Email != "huonghl@utc.edu.vn").FirstOrDefault();

            if (hc == null)
                return new JsonResult(new Dictionary<string, object>()
                {
                    {"Error","Cơ sở không có giáo viên quản lý"}
                });

            //Copy Class
            var newClass = new MappingEntity<ClassEntity, ClassEntity>().Clone(orgClass, new ClassEntity());
            if (!string.IsNullOrEmpty(Name))
                newClass.Name = Name;
            newClass.Center = center.ID;
            newClass.Created = DateTime.UtcNow;
            newClass.TeacherID = hc.ID;
            newClass.IsActive = false;
            newClass.Members = new List<ClassMemberEntity> {
                new ClassMemberEntity { Name = hc.FullName, TeacherID = hc.ID, Type = ClassMemberType.OWNER},
                new ClassMemberEntity { Name = hc.FullName, TeacherID = hc.ID, Type = ClassMemberType.TEACHER}
            };

            _service.Save(newClass);

            //Copy Course & ClassSubject

            var classSbjs = _classSubjectService.GetByClassID(ID);

            foreach (var classSbj in classSbjs)
            {
                //if (classSbj.TypeClass == CLASSSUBJECT_TYPE.EXAM)
                //    continue;

                //copy course
                //if (classSbj.TypeClass == CLASSSUBJECT_TYPE.EXAM) continue;
                var orgCourse = _courseService.GetItemByID(classSbj.CourseID);

                var newClassSbj = new MappingEntity<ClassSubjectEntity, ClassSubjectEntity>().Clone(classSbj, new ClassSubjectEntity());
                newClassSbj.ClassID = newClass.ID;
                newClassSbj.TeacherID = hc.ID;
                newClassSbj.CourseID = orgCourse?.ID;
                newClassSbj.CourseName = string.IsNullOrEmpty(classSbj.CourseName) ? orgCourse.Name : classSbj.CourseName;
                newClassSbj.Image = string.IsNullOrEmpty(classSbj.Image) ? orgCourse?.Image : classSbj.Image;

                if (orgCourse != null)//origin course is exist => copy course to new center & share to ref
                {
                    if (orgCourse.TargetCenters != null)
                        if (!orgCourse.TargetCenters.Contains(center.ID))
                            _courseService.ShareToCenter(orgCourse.ID, center.ID);

                    var copied = _courseService.GetCopiedItemInCenter(orgCourse.ID, center.ID);
                    if (copied != null)
                    {
                        newClassSbj.CourseID = copied.ID;
                    }
                    else
                    {
                        var newCourse = new CourseEntity { Center = center.ID, CreateUser = hc.ID };
                        newCourse = await _courseHelper.CopyCourse(orgCourse, newCourse);
                        newClassSbj.CourseID = newCourse.ID;
                    }

                }
                //copy classsubject
                _classSubjectService.Save(newClassSbj);

                _courseHelper.CloneForClassSubject(newClassSbj);
            }


            var subjects = _classSubjectService.CreateQuery().Find(t => t.ClassID == newClass.ID && t.TypeClass == CLASSSUBJECT_TYPE.EXAM).ToList();
            if (subjects.Count() == 0)
            {
                var newSbj = new ClassSubjectEntity
                {
                    ClassID = newClass.ID,
                    CourseName = "Bài kiểm tra",
                    Description = "Bài kiểm tra",
                    StartDate = newClass.StartDate,
                    EndDate = newClass.EndDate,
                    TypeClass = CLASSSUBJECT_TYPE.EXAM,
                    TeacherID = newClass.TeacherID
                };
                _classSubjectService.Save(newSbj);
            }

            Dictionary<string, object> DataResponse = new Dictionary<string, object>()
            {
                {"Data",newClass}
            };
            return new JsonResult(DataResponse);
        }
    }
}