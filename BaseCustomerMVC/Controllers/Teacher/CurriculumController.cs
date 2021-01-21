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
using System.Security.Claims;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
//Word
using System.Drawing;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using Spire.Doc.Fields.OMath;
using System.Drawing.Imaging;
using HtmlAgilityPack;
using Spire.Doc.Formatting;
using System.Net;
using FileManagerCore.Interfaces;
using System.Net.NetworkInformation;
using Google.Apis.Util;
using System.Data.OleDb;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using RestSharp.Extensions;
using MongoDB.Bson.Serialization.Serializers;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
//using Spire.Pdf.Exporting.XPS.Schema;
using GoogleLib.Services;
using MongoDB.Driver.Linq;
using Spire.Doc.Collections;
using System.Runtime.CompilerServices;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.Style;

namespace BaseCustomerMVC.Controllers.Teacher
{
    [BaseAccess.Attribule.AccessCtrl("Bài giảng chung", "teacher")]
    public class CurriculumController : TeacherController
    {
        private readonly CourseService _service;
        private readonly CourseHelper _courseHelper;
        private readonly SubjectService _subjectService;
        private readonly CourseChapterService _chapterService;
        private readonly GradeService _gradeService;
        private readonly CourseLessonService _lessonService;
        private readonly SkillService _skillService;
        private readonly LessonHelper _lessonHelper;

        private readonly LessonPartService _lessonPartService;
        private readonly LessonPartAnswerService _lessonPartAnswerService;
        private readonly LessonPartQuestionService _lessonPartQuestionService;

        private readonly TeacherService _teacherService;
        private readonly TeacherHelper _teacherHelper;
        private readonly ModCourseService _modservice;
        private readonly ModSubjectService _modsubjectService;
        private readonly ModChapterService _modchapterService;
        private readonly ModGradeService _modgradeService;
        private readonly ModLessonService _modlessonService;
        private readonly ModLessonPartService _modlessonPartService;
        private readonly ModLessonPartAnswerService _modlessonPartAnswerService;
        private readonly ModLessonPartQuestionService _modlessonPartQuestionService;
        private readonly ModLessonExtendService _modlessonExtendService;

        private readonly FileProcess _fileProcess;
        private readonly IHostingEnvironment _env;
        private string _publisherHost;

        private readonly CenterService _centerService;
        private readonly RoleService _roleService;

        private readonly MappingEntity<CourseEntity, CourseViewModel> _courseViewMapping;

        //fixing data
        private readonly ChapterService _newchapterService;
        private readonly LessonService _newlessonService;
        private readonly ClassService _classService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly ClassProgressService _classProgressService;
        private readonly ClassSubjectProgressService _classSubjectProgressService;
        private readonly ChapterProgressService _chapterProgressService;
        private readonly LessonProgressService _lessonProgressService;
        private readonly ExamService _examService;
        private readonly ExamDetailService _examDetailService;
        private readonly CloneLessonPartService _cloneLessonPartService;
        private readonly CloneLessonPartQuestionService _cloneLessonPartQuestionService;
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly StudentService _studentService;
        private readonly CourseLessonService _courseLessonService;

        private readonly MappingEntity<ChapterEntity, CourseChapterEntity> _chapterMappingRev = new MappingEntity<ChapterEntity, CourseChapterEntity>();
        private readonly MappingEntity<CourseChapterEntity, ChapterEntity> _chapterMapping = new MappingEntity<CourseChapterEntity, ChapterEntity>();
        private readonly MappingEntity<LessonEntity, CourseLessonEntity> _lessonMappingRev = new MappingEntity<LessonEntity, CourseLessonEntity>();
        private readonly MappingEntity<CourseLessonEntity, LessonEntity> _lessonMapping = new MappingEntity<CourseLessonEntity, LessonEntity>();

        private readonly MappingEntity<CourseLessonEntity, CourseLessonEntity> _cloneCourseLessonMapping = new MappingEntity<CourseLessonEntity, CourseLessonEntity>();
        private readonly MappingEntity<CourseChapterEntity, CourseChapterEntity> _cloneCourseChapterMapping = new MappingEntity<CourseChapterEntity, CourseChapterEntity>();
        private readonly MappingEntity<CourseEntity, CourseEntity> _cloneCourseMapping = new MappingEntity<CourseEntity, CourseEntity>();
        private readonly VocabularyService _vocabularyService;
        private readonly IRoxyFilemanHandler _roxyFilemanHandler;

        private readonly List<string> quizTypes = new List<string> { "QUIZ1", "QUIZ2", "QUIZ3", "QUIZ4", "ESSAY" };
        private readonly List<string> partTypes = new List<string> { "TEXT", "DOC", "AUDIO", "VIDEO", "IMG", "VOCAB", "QUIZ1", "QUIZ2", "QUIZ3", "QUIZ4", "ESSAY" };
        private readonly List<string> partDSP = new List<string> { "Văn bản", "File văn bản", "Audio", "Video", "Ảnh", "Từ vựng tiếng anh", "Chọn 1 đáp án đúng", "Điền từ", "Nối đáp án", "Chọn 1/nhiều đáp án đúng", "Essay" };



        private string RootPath { get; }
        private string StaticPath { get; }

        private string currentHost { get; }


        public CurriculumController(CourseService service,
                 CourseHelper courseHelper,
                 SubjectService subjectService,
                 CourseChapterService chapterService,
                 GradeService gradeService,
                 CourseLessonService lessonService,
                 SkillService skillService,
                 LessonPartService lessonPartService,
                 LessonPartAnswerService lessonPartAnswerService,
                 LessonPartQuestionService lessonPartQuestionService,
                 TeacherService teacherService,
                 TeacherHelper teacherHelper,
                 ModCourseService modservice,

                 LessonHelper lessonHelper

                , RoleService roleService
                , ModSubjectService modsubjectService
                , ModChapterService modchapterService
                , ModGradeService modgradeService
                , ModLessonService modlessonService
                , ModLessonPartService modlessonPartService
                , ModLessonPartAnswerService modlessonPartAnswerService
                , ModLessonPartQuestionService modlessonPartQuestionService
                , ModLessonExtendService modlessonExtendService
                , IHostingEnvironment evn
                , IConfiguration config
                , FileProcess fileProcess
                , CenterService centerService

                 //use for fixing data
                 , ClassSubjectService classSubjectService
                 , CloneLessonPartService cloneLessonPartService
                 , CloneLessonPartQuestionService cloneLessonPartQuestionService
                 , ChapterService newchapterService
                 , LessonService newlessonService
                 , ChapterProgressService chapterProgressService
                 , ClassSubjectProgressService classSubjectProgressService
                 , ClassProgressService classProgressService
                 , LessonProgressService lessonProgressService
                 , ExamService examService
                 , ClassService classService
                 , LessonScheduleService lessonScheduleService
                 , ExamDetailService examDetailService
                 , StudentService studentService
                 , CourseLessonService courseLessonService
                 , VocabularyService vocabularyService
                 , IRoxyFilemanHandler roxyFilemanHandler
                 , IHttpContextAccessor httpContextAccessor
                 )
        {
            _service = service;
            _courseHelper = courseHelper;
            //_programService = programService;
            _subjectService = subjectService;
            _chapterService = chapterService;
            _gradeService = gradeService;
            _skillService = skillService;
            _lessonService = lessonService;
            _centerService = centerService;
            _roleService = roleService;
            _lessonHelper = lessonHelper;

            _lessonPartService = lessonPartService;
            _lessonPartAnswerService = lessonPartAnswerService;
            _lessonPartQuestionService = lessonPartQuestionService;

            //_lessonExtendService = lessonExtendService;
            _teacherService = teacherService;
            _teacherHelper = teacherHelper;
            _modservice = modservice;

            //_modprogramService = modprogramService;
            _modsubjectService = modsubjectService;
            _modchapterService = modchapterService;
            _modgradeService = modgradeService;
            _modlessonService = modlessonService;
            _modlessonPartService = modlessonPartService;
            _modlessonPartAnswerService = modlessonPartAnswerService;
            _modlessonPartQuestionService = modlessonPartQuestionService;
            _modlessonExtendService = modlessonExtendService;

            _courseViewMapping = new MappingEntity<CourseEntity, CourseViewModel>();
            _env = evn;
            _fileProcess = new FileProcess(evn, config);
            _publisherHost = config.GetValue<string>("SysConfig:PublisherDomain");

            //fix
            _classService = classService;
            _cloneLessonPartService = cloneLessonPartService;
            _cloneLessonPartQuestionService = cloneLessonPartQuestionService;
            _classSubjectService = classSubjectService;
            _newchapterService = newchapterService;
            _newlessonService = newlessonService;
            _chapterProgressService = chapterProgressService;
            _classProgressService = classProgressService;
            _classSubjectProgressService = classSubjectProgressService;
            _lessonProgressService = lessonProgressService;
            _examService = examService;
            _examDetailService = examDetailService;
            _lessonScheduleService = lessonScheduleService;
            _studentService = studentService;
            _courseLessonService = courseLessonService;
            _vocabularyService = vocabularyService;

            _roxyFilemanHandler = roxyFilemanHandler;

            RootPath = (config.GetValue<string>("SysConfig:StaticPath") ?? evn.WebRootPath) + "/Files";
            StaticPath = (config.GetValue<string>("SysConfig:StaticPath") ?? evn.WebRootPath);
            currentHost = httpContextAccessor.HttpContext.Request.Host.Value;
        }

        #region PAGE
        public IActionResult Index(DefaultModel model, string basis, int old = 0)
        {
            CenterEntity center = null;
            if (!string.IsNullOrEmpty(basis))
            {
                center = _centerService.GetItemByCode(basis);
                if (center != null)
                    ViewBag.Center = center;
            }
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            var teacher = _teacherService.CreateQuery().Find(t => t.ID == UserID).SingleOrDefault();//: new TeacherEntity();

            if (teacher == null)
                return Redirect("/login");

            if (teacher != null && teacher.Subjects != null)
            {
                var subject = _subjectService.CreateQuery().Find(t => teacher.Subjects.Contains(t.ID) && t.IsActive).SortBy(t => t.Name).ToList();
                var grade = _gradeService.CreateQuery().Find(t => teacher.Subjects.Contains(t.SubjectID)).ToList();
                var skills = _skillService.GetList();
                var courses = _service.CreateQuery().Find(t => t.Center.Equals(center.ID)).SortByDescending(o => o.ID).ToList();
                ViewBag.Grades = grade;
                ViewBag.Subjects = subject;
                ViewBag.Skills = skills;
                ViewBag.Courses = courses;
            }
            var centersIDs = teacher.Centers.Select(t => t.CenterID).ToList();
            ViewBag.AllCenters = _centerService.CreateQuery().Find(t => centersIDs.Contains(t.ID) && t.ExpireDate >= DateTime.UtcNow && t.Status == true).ToList();

            if (!_teacherHelper.HasRole(UserID, center.ID, "head-teacher"))
                ViewBag.Teachers = new List<UserViewModel> { new UserViewModel { ID = teacher.ID, Name = teacher.FullName } };
            else
                ViewBag.Teachers = _teacherService.Collection.Find(t => t.Centers.Any(o => o.Code == basis)).ToEnumerable().Select(t => new UserViewModel { ID = t.ID, Name = t.FullName }).ToList();

            var modsubject = _modsubjectService.GetAll().ToList();
            var modgrade = _modgradeService.GetAll().ToList();

            ViewBag.ModGrade = modgrade;
            ViewBag.ModSubject = modsubject;
            ViewBag.User = UserID;

            ViewBag.RoleCode = User.Claims.GetClaimByType(ClaimTypes.Role).Value;
            ViewBag.Model = model;
            //ViewBag.ListCenters = _centerService.GetAll().ToList();
            if (old == 1)
                return View("Index_o");
            return View();

        }

        public IActionResult Detail(string basis, string ID)
        {
            return Redirect($"{basis}/{Url.Action("Modules", "Curriculum")}/{ID}");
        }

        public IActionResult Modules(string basis, string ID)
        {
            if (string.IsNullOrEmpty("ID"))
                return Redirect($"/{basis}{Url.Action("Index")}");

            var data = _service.GetItemByID(ID);
            if (data == null)
                return Redirect($"/{basis}{Url.Action("Index")}");

            //var isUsed = isCourseUsed(data.ID);
            //Cap nhat IsUsed
            //if (data.IsUsed != isUsed)
            //{
            //    data.IsUsed = isUsed;
            //    _service.Save(data);
            //}

            ViewBag.Data = data;
            ViewBag.Title = data.Name;

            var UserID = User.Claims.GetClaimByType("UserID").Value;

            var chapters = _chapterService.CreateQuery().Find(t => t.CourseID == ID).ToList();

            ViewBag.Chapter = chapters;
            ViewBag.User = UserID;
            ViewBag.Course = data;

            return View();
        }

        public IActionResult Route(string basis, string ID)
        {
            if (string.IsNullOrEmpty("ID"))
                return Redirect($"/{basis}{Url.Action("Index")}");

            var data = _service.GetItemByID(ID);
            if (data == null)
                return Redirect($"/{basis}{Url.Action("Index")}");

            //var isUsed = isCourseUsed(data.ID);
            //Cap nhat IsUsed
            //if (data.IsUsed != isUsed)
            //{
            //    data.IsUsed = isUsed;
            //    _service.Save(data);
            //}

            ViewBag.Data = data;
            ViewBag.Title = data.Name;

            var UserID = User.Claims.GetClaimByType("UserID").Value;

            var chapters = _chapterService.CreateQuery().Find(t => t.CourseID == ID).ToList();

            ViewBag.Chapter = chapters;
            ViewBag.User = UserID;
            ViewBag.Course = data;

            return View();
        }

        //[BaseAccess.Attribule.AccessCtrl("Bài giảng chung", "teacher")]
        public IActionResult Lesson(DefaultModel model, string basis, string CourseID, string ClassID, int frameview = 0)
        {
            //if (!User.IsInRole("head-teacher"))
            //    return Redirect("/");
            //if (!string.IsNullOrEmpty(basis))
            //{
            //    var center = _centerService.GetItemByCode(basis);
            //    if (center != null)
            //        ViewBag.Center = center;
            //}

            if (CourseID == null)
            {
                if (ClassID == null)
                    return Redirect($"/{basis}{Url.Action("Index")}");
                else
                    CourseID = ClassID;
            }
            var currentCourse = _service.GetItemByID(CourseID);
            if (currentCourse == null)
                return Redirect($"/{basis}{Url.Action("Index")}");
            var Data = _lessonService.GetItemByID(model.ID);
            if (Data == null)
                return Redirect($"/{basis}{Url.Action("Index")}");

            ViewBag.Course = currentCourse;
            ViewBag.Data = Data;
            if (frameview == 1)
                return View("LessonFrame");
            //ViewBag.RoleCode = "head-teacher";
            return View();
        }
        #endregion

        #region Course

        [HttpPost]
        public JsonResult GetList(DefaultModel model, string Center, string SubjectID = "", string GradeID = "")
        {
            var filter = new List<FilterDefinition<CourseEntity>>();

            var UserID = User.Claims.GetClaimByType("UserID").Value;
            if (!string.IsNullOrEmpty(Center))
            {
                var @center = _centerService.GetItemByCode(Center);
                if (@center == null) return new JsonResult(new Dictionary<string, object>
                    {
                        { "Error", "Cơ sở không đúng"}
                    });
                filter.Add(Builders<CourseEntity>.Filter.Where(o => o.Center == @center.ID));
            }

            var teacher = _teacherService.GetItemByID(UserID);
            CenterMemberEntity memberEntity = null;

            if (teacher == null)
            {
                return new JsonResult(new Dictionary<string, object>
                    {
                        { "Error", "Bạn không được quyền thực hiện thao tác này"}
                    });
            }
            memberEntity = teacher.Centers.FirstOrDefault(t => t.Code == Center);
            if (memberEntity == null)
            {
                return new JsonResult(new Dictionary<string, object>
                    {
                        { "Error", "Bạn không được quyền thực hiện thao tác này"}
                    });
            }

            if (!string.IsNullOrEmpty(SubjectID))
            {
                filter.Add(Builders<CourseEntity>.Filter.Where(o => o.SubjectID == SubjectID));
            }
            else
            {
                //lọc các môn được phân công
                filter.Add(Builders<CourseEntity>.Filter.Where(o => teacher.Subjects.Contains(o.SubjectID)));
            }
            if (!string.IsNullOrEmpty(GradeID))
            {
                filter.Add(Builders<CourseEntity>.Filter.Where(o => o.GradeID == GradeID));
            }

            if (_roleService.GetItemByID(memberEntity.RoleID).Code != "head-teacher")
                //if (User.Claims.GetClaimByType(ClaimTypes.Role).Value == "teacher")
                filter.Add(Builders<CourseEntity>.Filter.Where(o => o.TeacherID == UserID));

            if (!string.IsNullOrEmpty(model.SearchText))
                filter.Add(Builders<CourseEntity>.Filter.Text("\"" + model.SearchText + "\""));
            //filter.Add(Builders<CourseEntity>.Filter.Text(model.SearchText));


            var data = (filter.Count > 0 ? _service.Collection.Find(Builders<CourseEntity>.Filter.And(filter)) : _service.GetAll()).SortByDescending(t => t.ID);
            model.TotalRecord = data.CountDocuments();

            var response = new Dictionary<string, object>
            {
            };

            if (model.PageIndex < 0 || model.PageIndex * model.PageSize > model.TotalRecord)
            {
                response = new Dictionary<string, object>
                {
                    { "Model", model }
                };
            }
            else
            {
                var DataResponse = data == null || model.TotalRecord <= 0 //|| model.TotalRecord < model.PageSize
                    ? data
                    : data.Skip((model.PageIndex) * model.PageSize).Limit(model.PageSize);
                //var DataResponse = data;

                var rsp = DataResponse.ToEnumerable().Select(o =>
                    _courseViewMapping.AutoOrtherType(o, new CourseViewModel()
                    {
                        //SkillName = _skillService.GetItemByID(o.SkillID)?.Name,
                        GradeName = _gradeService.GetItemByID(o.GradeID)?.Name,
                        SubjectName = _subjectService.GetItemByID(o.SubjectID)?.Name,
                        TeacherName = _teacherService.GetItemByID(o.TeacherID)?.FullName
                    })).ToList();


                response = new Dictionary<string, object>
                {
                    { "Data", rsp},
                    { "Model", model }
                };
            }
            return new JsonResult(response);
        }

        [HttpPost]
        public JsonResult GetActiveList(DefaultModel model, string Center = "", string SubjectID = "", string GradeID = "", bool cp = false)
        {
            var filter = new List<FilterDefinition<CourseEntity>>();

            var UserID = User.Claims.GetClaimByType("UserID").Value;

            if (!string.IsNullOrEmpty(Center))
            {
                var @center = _centerService.GetItemByCode(Center);
                if (@center == null) return new JsonResult(new Dictionary<string, object>
                    {
                        { "Error", "Cơ sở không đúng"}
                    });
                filter.Add(Builders<CourseEntity>.Filter.Where(o => o.Center == @center.ID));
            }
            if (!string.IsNullOrEmpty(SubjectID))
            {
                filter.Add(Builders<CourseEntity>.Filter.Where(o => o.SubjectID == SubjectID));
            }
            if (!string.IsNullOrEmpty(GradeID))
            {
                filter.Add(Builders<CourseEntity>.Filter.Where(o => o.GradeID == GradeID));
            }

            if (User.Claims.GetClaimByType(ClaimTypes.Role).Value == "teacher")
                filter.Add(Builders<CourseEntity>.Filter.Where(o => o.TeacherID == UserID));

            filter.Add(Builders<CourseEntity>.Filter.Where(o => o.IsActive));

            var data = filter.Count > 0 ? _service.Collection.Find(Builders<CourseEntity>.Filter.And(filter)) : _service.GetAll();

            //model.TotalRecord = data.CountDocuments();

            //var DataResponse = data == null || model.TotalRecord <= 0 || model.TotalRecord < model.PageSize
            //    ? data
            //    : data.Skip((model.PageIndex - 1) * model.PageSize).Limit(model.PageSize);
            var DataResponse = data;

            var response = new Dictionary<string, object>
            {
                { "Data", DataResponse.ToList().Select(o =>

                    _courseViewMapping.AutoOrtherType(o, new CourseViewModel(){
                        GradeName = _gradeService.GetItemByID(o.GradeID)?.Name,
                        SubjectName = _subjectService.GetItemByID(o.SubjectID)?.Name,
                        TeacherName = _teacherService.GetItemByID(o.TeacherID)?.FullName
                    })).ToList()
                },
                { "Model", model }
            };
            return new JsonResult(response);
        }

        [HttpPost]
        public JsonResult GetCourseDetail(DefaultModel model)
        {
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            //var teacher = _teacherService.GetItemByID(UserID);

            var filter = new List<FilterDefinition<ClassEntity>>();

            var course = _service.GetItemByID(model.ID);

            if (course == null)
            {
                return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Không có thông tin giáo trình" }
                    });
            }
            var courseDetail = new Dictionary<string, object>
            {
                { "Chapters", _chapterService.CreateQuery().Find(o => o.CourseID == course.ID).SortBy(o => o.ParentID).ThenBy(o => o.Order).ThenBy(o => o.ID).ToList() } ,
                { "Lessons", _lessonService.CreateQuery().Find(o => o.CourseID == course.ID).SortBy(o => o.ChapterID).ThenBy(o => o.Order).ThenBy(o => o.ID).ToList() }
            };

            var response = new Dictionary<string, object>
            {
                { "Data", courseDetail },
                { "Model", model }
            };
            return new JsonResult(response);
        }

        public JsonResult GetContents(string ID, string Parent)
        {
            var currentClass = _service.GetItemByID(ID);
            if (currentClass == null)
                return new JsonResult(new Dictionary<string, object>
                    {
                        {"Error", "Không tìm thấy lớp học" }
                    });

            if (string.IsNullOrEmpty(Parent))
                Parent = "0";

            var TopID = "";
            if (Parent != "0")
            {
                var top = _chapterService.GetItemByID(Parent);
                if (top == null)
                    return new JsonResult(new Dictionary<string, object>
                    {
                        {"Error", "Không tìm thấy chương" }
                    });
                TopID = top.ParentID;
            }

            var chapters = _chapterService.CreateQuery().Find(c => c.CourseID == currentClass.ID && c.ParentID == Parent).ToList();

            var lessons = _lessonService.CreateQuery().Find(o => o.CourseID == currentClass.ID && o.ChapterID == Parent).SortBy(o => o.Order).ThenBy(o => o.ID).ToList();

            var response = new Dictionary<string, object>
                {
                    { "RootID", TopID },
                    { "Data", chapters },
                    { "Lesson", lessons }
                };

            return new JsonResult(response);
        }

        [HttpPost]
        public async Task<JsonResult> CreateOrUpdate(CourseEntity item, string CenterCode)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var olditem = _service.CreateQuery().Find(o => o.ID == item.ID).SingleOrDefault();
                if (olditem == null)
                {
                    var center = _centerService.GetItemByCode(CenterCode);
                    if (string.IsNullOrEmpty(CenterCode) || center == null)
                        return new JsonResult(new Dictionary<string, object>
                    {
                        {"Error", "Cơ sở không đúng" }
                    });
                    item.Center = center.ID;
                    item.Created = DateTime.UtcNow;
                    item.CreateUser = UserID;
                    item.IsAdmin = true;
                    item.IsActive = false;
                    item.Updated = DateTime.UtcNow;
                    item.TeacherID = UserID;
                    item.TotalPractices = 0;
                    item.TotalLessons = 0;
                    item.TotalExams = 0;
                    item.SkillID = "7";//HARD CODE TO PREVENT ERRROR, REMOVE LATER
                    //item.IsPublic = false;
                    //if (item.TargetCenters != null && item.TargetCenters[0] != null)
                    //{
                    //    var listCenters = item.TargetCenters[0].Split(',');
                    //    item.TargetCenters = listCenters.ToList();
                    //}

                    var files = HttpContext.Request.Form != null && HttpContext.Request.Form.Files.Count > 0 ? HttpContext.Request.Form.Files : null;
                    if (files != null && files.Count > 0)
                    {
                        var file = files[0];

                        var filename = DateTime.UtcNow.ToString("yyyyMMddhhmmss") + System.IO.Path.GetExtension(file.FileName);
                        item.Image = await _fileProcess.SaveMediaAsync(file, filename, "BOOKCOVER");
                    }


                    _service.Save(item);
                }
                else
                {
                    olditem.Updated = DateTime.UtcNow;
                    olditem.Description = item.Description;
                    olditem.SubjectID = item.SubjectID;
                    olditem.GradeID = item.GradeID;
                    //olditem.SkillID = item.SkillID;
                    olditem.Name = item.Name;
                    olditem.TeacherID = item.TeacherID;
                    //olditem.IsPublic = item.IsPublic;
                    //olditem.PublicWStudent = item.PublicWStudent;
                    //if (item.TargetCenters != null && item.TargetCenters[0] != null)
                    //{
                    //    var listCenters = item.TargetCenters[0].Split(',');
                    //    item.TargetCenters = listCenters.ToList();
                    //}
                    olditem.TargetCenters = item.TargetCenters;
                    olditem.StudentTargetCenters = item.StudentTargetCenters;
                    var files = HttpContext.Request.Form != null && HttpContext.Request.Form.Files.Count > 0 ? HttpContext.Request.Form.Files : null;
                    if (files != null && files.Count > 0)
                    {
                        var file = files[0];

                        var filename = DateTime.UtcNow.ToString("yyyyMMddhhmmss") + System.IO.Path.GetExtension(file.FileName);
                        olditem.Image = await _fileProcess.SaveMediaAsync(file, filename, "BOOKCOVER");
                    }
                    _service.Save(olditem);
                    //update class subject using this course, temporary use
                    //_classSubjectService.UpdateCourseSkill(olditem.ID, olditem.SkillID);
                }

                return new JsonResult(new Dictionary<string, object>
                {
                    {"Data", item },
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

        [HttpPost]
        public async Task<JsonResult> CloneCourse(string CourseID, CourseEntity newcourse)
        {
            var _userCreate = User.Claims.GetClaimByType("UserID").Value;
            var course = _service.GetItemByID(CourseID);//Clone
            if (course == null)
            {
                return Json(new { error = "Dữ liệu không đúng, vui lòng kiểm tra lại" });
            }

            await CopyCourse(course, newcourse, _userCreate);//?????

            return Json("OK");
        }

        [HttpPost]
        public async Task<JsonResult> MergeCourse(string CourseID, CourseEntity newcourse, string joinCourseID)
        {
            try
            {
                var _userCreate = User.Claims.GetClaimByType("UserID").Value;
                var org_course = _service.GetItemByID(CourseID);
                var join_course = _service.GetItemByID(joinCourseID);
                if (org_course == null || join_course == null)
                {
                    return Json(new { error = "Dữ liệu không đúng, vui lòng kiểm tra lại" });
                }

                //var lessonOfJoinCourse = _lessonService.CreateQuery().Find(o => o.CourseID == joinCourseID && o.ChapterID == "0");
                var rootchapOrder = (int)_chapterService.GetSubChapters(org_course.ID, "0").Count();
                var rootlessonOrder = (int)_lessonService.GetChapterLesson(org_course.ID, "0").Count();
                var newCourseID = await CopyCourse(org_course, newcourse, _userCreate);

                var rootchapterOfJoinCourse = _chapterService.GetSubChapters(joinCourseID, "0");
                //*** clone JoinCourse's chapters ***
                foreach (var chapter in rootchapterOfJoinCourse)
                {
                    var clone_chap = _cloneCourseChapterMapping.Clone(chapter, new CourseChapterEntity());
                    clone_chap.CourseID = newCourseID;
                    clone_chap.CreateUser = _userCreate;
                    clone_chap.Order = rootchapOrder++;
                    clone_chap.OriginID = chapter.ID;
                    var item = await CloneChapter(clone_chap, _userCreate, joinCourseID);
                }
                //** clone joinCourse's root lesson
                var rootlessonOfJoinCourse = _lessonService.CreateQuery().Find(o => o.CourseID == joinCourseID && o.ChapterID == "0").SortBy(o => o.Order).ToEnumerable();
                foreach (var o in rootlessonOfJoinCourse)
                {
                    var new_lesson = _cloneCourseLessonMapping.Clone(o, new CourseLessonEntity());
                    new_lesson.CreateUser = _userCreate;
                    new_lesson.Created = DateTime.UtcNow;
                    new_lesson.CourseID = newCourseID;
                    new_lesson.OriginID = o.ID;
                    new_lesson.ChapterID = "0";
                    new_lesson.Order = rootlessonOrder++;
                    await CloneLesson(o, _userCreate);
                }
                //increase target course counter
                await _courseHelper.IncreaseCourseCounter(newCourseID, join_course.TotalLessons, join_course.TotalExams, join_course.TotalPractices);
                return Json("OK");
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Remove(DefaultModel model)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                if (model.ArrID == null)
                {
                    return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", "Bài giảng đã xóa" },
                                {"Error", null }
                            });
                }
                var ID = model.ArrID;
                var course = _service.CreateQuery().Find(o => o.ID == ID).SingleOrDefault();
                if (course == null)
                {
                    return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", "Bài giảng đã bị xóa" },
                                {"Error", null }
                            });
                }


                if (course.TeacherID != UserID)
                {
                    var center = course.Center;
                    if (!_teacherHelper.HasRole(UserID, center, "head-teacher"))
                        return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", null },
                                {"Error", "Bạn không có quyền thực hiện thao tác này" }
                            });
                }


                _chapterService.CreateQuery().DeleteMany(o => o.CourseID == course.ID);
                _lessonService.CreateQuery().DeleteMany(o => o.CourseID == course.ID);
                _lessonPartAnswerService.CreateQuery().DeleteMany(o => o.CourseID == course.ID);
                _lessonPartQuestionService.CreateQuery().DeleteMany(o => o.CourseID == course.ID);
                _lessonPartQuestionService.CreateQuery().DeleteMany(o => o.CourseID == course.ID);

                var lessons = _lessonService.CreateQuery().Find(o => o.CourseID == course.ID).ToList();

                if (lessons != null)
                {
                    foreach (var lesson in lessons)
                    {
                        var parts = _lessonPartService.CreateQuery().Find(o => o.ParentID == lesson.ID).ToList();
                        foreach (var part in parts)
                        {
                            var quizs = _lessonPartQuestionService.CreateQuery().Find(o => o.ParentID == part.ID).ToList();
                            foreach (var quiz in quizs)
                            {
                                _lessonPartAnswerService.CreateQuery().DeleteMany(o => o.ParentID == quiz.ID);
                            }
                            _lessonPartQuestionService.CreateQuery().DeleteMany(o => o.ParentID == lesson.ID);
                        }
                        _lessonPartQuestionService.CreateQuery().DeleteMany(o => o.ParentID == lesson.ID);
                    }
                    _lessonService.CreateQuery().DeleteMany(o => o.CourseID == course.ID);
                }
                await _service.RemoveAsync(ID);
                return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", "Đã xóa bài giảng" },
                                {"Error", null }
                            });
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

        [HttpPost]
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
                    var filter = Builders<CourseEntity>.Filter.Where(o => model.ArrID.Split(',').Contains(o.ID) && o.IsActive != true);
                    var update = Builders<CourseEntity>.Update.Set("IsActive", true);
                    var publish = _service.Collection.UpdateMany(filter, update);
                    return new JsonResult(publish);
                }
                else
                {
                    var filter = Builders<CourseEntity>.Filter.Where(o => model.ArrID == o.ID && o.IsActive != true);
                    var update = Builders<CourseEntity>.Update.Set("IsActive", true);
                    var publish = _service.Collection.UpdateMany(filter, update);
                    return new JsonResult(publish);
                }

            }
        }

        [HttpPost]
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
                    var filter = Builders<CourseEntity>.Filter.Where(o => model.ArrID.Split(',').Contains(o.ID) && o.IsActive == true);
                    var update = Builders<CourseEntity>.Update.Set("IsActive", false);
                    var publish = _service.Collection.UpdateMany(filter, update);
                    return new JsonResult(publish);
                }
                else
                {
                    var filter = Builders<CourseEntity>.Filter.Where(o => model.ArrID == o.ID && o.IsActive == true);
                    var update = Builders<CourseEntity>.Update.Set("IsActive", false);
                    var publish = _service.Collection.UpdateMany(filter, update);
                    return new JsonResult(publish);
                }


            }
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<JsonResult> SaveInfo(CourseEntity entity, string basis)
        {
            var currentCourse = _service.GetItemByID(entity.ID);
            if (currentCourse == null)
            {
                new JsonResult(
                    new Dictionary<string, object>
                    {
                        { "Error", "Thông tin không chính xác"}
                    });
            }

            currentCourse.Outline = entity.Outline ?? "";
            currentCourse.Description = entity.Description ?? "";
            currentCourse.LearningOutcomes = entity.LearningOutcomes ?? "";

            try
            {
                var files = HttpContext.Request.Form != null && HttpContext.Request.Form.Files.Count > 0 ? HttpContext.Request.Form.Files : null;
                if (files != null && files.Count > 0)
                {
                    var file = files[0];

                    var filename = currentCourse.ID + "_" + DateTime.UtcNow.ToString("yyyyMMddhhmmss") + System.IO.Path.GetExtension(file.FileName);
                    currentCourse.Image = await _fileProcess.SaveMediaAsync(file, filename, "BOOKCOVER", basis);
                }
                _service.CreateOrUpdate(currentCourse);

                return new JsonResult(
                    new Dictionary<string, object>
                    {
                        { "Data", currentCourse }
                    }
                );
            }
            catch (Exception e)
            {
                return new JsonResult(
                        new Dictionary<string, object>
                        {
                            { "Error", e.Message}
                        });
            }
        }

        public async Task<string> CopyCourse(CourseEntity org_course, CourseEntity target_course, string _userCreate = "")
        {
            var new_course = _cloneCourseMapping.Clone(org_course, new CourseEntity());

            new_course.OriginID = org_course.ID;
            new_course.Name = target_course.Name;
            new_course.Code = target_course.Code;
            new_course.Description = target_course.Description;
            new_course.GradeID = target_course.GradeID;
            new_course.SubjectID = target_course.SubjectID;
            new_course.TeacherID = _userCreate;
            new_course.CreateUser = _userCreate;
            new_course.Center = target_course.Center ?? org_course.Center;
            new_course.SkillID = target_course.SkillID;
            new_course.Created = DateTime.UtcNow;
            new_course.Updated = DateTime.UtcNow;
            new_course.IsActive = org_course.IsActive;
            new_course.IsUsed = false;
            _service.Collection.InsertOne(new_course);

            await CloneChapter(new CourseChapterEntity
            {
                OriginID = "0",
                CourseID = new_course.ID
            }, _userCreate, org_course.ID);

            return new_course.ID;
        }

        #endregion Course

        #region Chapter
        /*--- API ---*/
        [HttpPost]
        public JsonResult CreateOrUpdateChapter(CourseChapterEntity item)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;

                if (item.CourseID == null || _service.GetItemByID(item.CourseID) == null)
                {
                    return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", null },
                        {"Error", "No Course Found" }
                    });
                }

                var data = _chapterService.GetItemByID(item.ID);

                CourseChapterEntity parent = null;
                if (data.ParentID != "0")
                {
                    parent = _chapterService.GetItemByID(data.ParentID);
                }

                var needUpdate = true;
                if (data == null)
                {
                    item.Created = DateTime.UtcNow;
                    item.IsAdmin = true;
                    item.IsActive = false;
                    item.Updated = DateTime.UtcNow;
                    item.Order = int.MaxValue - 1;
                    item.Period = 7;

                    if (item.Start <= 0) //unset
                    {
                        item.Start = 0;
                        item.Period = 0;
                    }

                    _chapterService.Save(item);

                    ChangeChapterPosition(item, int.MaxValue);//move chapter to bottom of new parent chap

                    //needUpdate = true;
                }
                else
                {
                    item.Updated = DateTime.UtcNow;
                    var newOrder = item.Order - 1;
                    var oldParent = data.ParentID;

                    data.Name = item.Name;
                    data.ParentID = item.ParentID;
                    data.Description = item.Description;

                    data.ConnectType = item.ConnectType;
                    data.ConnectID = "";
                    data.Period = 7;

                    if (item.Start <= 0)
                    {
                        item.Start = 0;
                        data.Period = 0;
                    }

                    data.Start = item.Start;

                    //if (data.Period != item.Period)
                    //{
                    //    needUpdate = true;
                    //}

                    data.ConnectID = "";
                    //data.Period = item.Period;

                    _chapterService.Save(data);

                    if (oldParent != item.ParentID)//Change Root chapter
                    {
                        if (item.TotalLessons > 0)
                        {
                            //decrease old parent counter
                            _ = _courseHelper.IncreaseCourseChapterCounter(oldParent, 0 - data.TotalLessons, 0 - data.TotalExams, 0 - data.TotalPractices);

                            //increase old parent counter
                            _ = _courseHelper.IncreaseCourseChapterCounter(item.ParentID, data.TotalLessons, data.TotalExams, data.TotalPractices);
                        }
                        //move chapter to bottom of new parent chap
                        ChangeChapterPosition(data, int.MaxValue);
                    }
                    else if (data.Order != newOrder)
                        ChangeChapterPosition(data, newOrder);
                }
                //update route
                if (needUpdate)
                {
                    UpdateRoute(data, parent);
                }

                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", item },
                    { "Error", null }
                });
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

        private void UpdateRoute(CourseChapterEntity obj, CourseChapterEntity parent)
        {
            //if (string.IsNullOrEmpty(obj.ConnectID))
            //{
            //    obj.Start = 0;
            //    if (parent != null)
            //        obj.Start = parent.Start;
            //}
            //else
            //{
            //    if (obj.ConnectType == CONNECT_TYPE.CHAPTER)
            //    {
            //        var prev = _chapterService.GetItemByID(obj.ConnectID);
            //        if (prev == null) // move to root
            //        {
            //            if (parent != null)
            //                obj.Start = parent.Start;
            //        }
            //        else
            //            obj.Start = prev.Start + prev.Period;
            //    }
            //    else
            //    {
            //        var prev = _lessonService.GetItemByID(obj.ConnectID);
            //        if (prev == null) // move to root
            //        {
            //            if (parent != null)
            //                obj.Start = parent.Start;
            //        }
            //        else
            //            obj.Start = prev.Start + prev.Period;
            //    }
            //}

            _chapterService.Save(obj);

            //var linked_period = obj.Period;
            //linked_period = Math.Max((double)obj.Period + UpdateConnectedRoute(obj), linked_period);

            UpdateParentRoute(parent, obj.Start + obj.Period);
            UpdateSubRoute(obj);
        }

        private void UpdateRoute(CourseLessonEntity obj, CourseChapterEntity parent)
        {
            //if (string.IsNullOrEmpty(obj.ConnectID))
            //{
            //    obj.Start = 0;
            //    if (parent != null)
            //        obj.Start = parent.Start;
            //}
            //else
            //{
            //    if (obj.ConnectType == CONNECT_TYPE.CHAPTER)
            //    {
            //        var prev = _chapterService.GetItemByID(obj.ConnectID);
            //        if (prev == null) // move to root
            //        {
            //            if (parent != null)
            //                obj.Start = parent.Start;
            //        }
            //        else
            //            obj.Start = prev.Start + prev.Period;
            //    }
            //    else
            //    {
            //        var prev = _lessonService.GetItemByID(obj.ConnectID);
            //        if (prev == null) // move to root
            //        {
            //            if (parent != null)
            //                obj.Start = parent.Start;
            //        }
            //        else
            //            obj.Start = prev.Start + prev.Period;
            //    }
            //}

            _lessonService.Save(obj);

            //var linked_period = obj.Period;
            //linked_period = Math.Max((double)obj.Period + UpdateConnectedRoute(new CourseChapterEntity { ID = obj.ID, ParentID = obj.ChapterID, CourseID = obj.CourseID, Start = obj.Start, Period = obj.Period }), linked_period);

            UpdateParentRoute(parent, obj.Start + obj.Period);
        }


        private double UpdateConnectedRoute(CourseChapterEntity data)
        {
            double linked_period = data.Period;
            var connectedChaps = _chapterService.GetItemByConnectID(data.CourseID, data.ParentID, data.ID);
            if (connectedChaps != null && connectedChaps.Count() > 0)
            {
                foreach (var chap in connectedChaps)
                {
                    chap.Start = data.Start + data.Period;
                    linked_period = Math.Max((double)chap.Period + UpdateConnectedRoute(chap), linked_period);
                    _chapterService.Save(chap);
                    UpdateSubRoute(chap);
                }
            }

            var connectedLessons = _lessonService.GetItemByConnectID(data.CourseID, data.ParentID, data.ID);
            if (connectedLessons != null && connectedLessons.Count() > 0)
            {
                foreach (var lesson in connectedLessons)
                {
                    lesson.Start = data.Start + data.Period;
                    linked_period = Math.Max((double)lesson.Period + UpdateConnectedRoute(new CourseChapterEntity { ID = lesson.ID, ParentID = lesson.ChapterID, CourseID = lesson.CourseID, Start = lesson.Start, Period = lesson.Period }), linked_period);
                    _lessonService.Save(lesson);
                }
            }
            return linked_period;
        }


        private void UpdateParentRoute(CourseChapterEntity data, double expected_size)
        {
            if (data == null) return;
            if (data.Start <= 0)//unset
                return;
            if (data.Start + data.Period < expected_size)
            {
                data.Period = expected_size - data.Start;
                _chapterService.Save(data);
                var linked_period = UpdateConnectedRoute(data);
                if (data.ParentID != "0")
                {
                    var parent = _chapterService.GetItemByID(data.ParentID);
                    UpdateParentRoute(parent, data.Start + linked_period);
                }
            }
        }

        private void UpdateSubRoute(CourseChapterEntity data)
        {
            var subChaps = _chapterService.GetSubChapters(data.CourseID, data.ID);
            if (subChaps != null && subChaps.Count() > 0)
            {
                foreach (var chap in subChaps)
                {
                    chap.Start = data.Start;
                    chap.Period = data.Period;
                    //if (string.IsNullOrEmpty(chap.ConnectID))//direct child
                    //{
                    //    chap.Start = data.Start;
                    //    if (chap.Period == 0 || chap.Period + chap.Start > data.Start + data.Period)
                    //        chap.Period = data.Period;
                    //}
                    //else
                    //{
                    //    if (chap.Start > data.Start + data.Period)
                    //    {
                    //        chap.Start = data.Start + data.Period;
                    //        chap.Period = 0;
                    //    }
                    //    if (chap.Start + chap.Period > data.Start + data.Period)
                    //        chap.Period = data.Start + data.Period - chap.Start;//resize route to fit parent
                    //}
                    _chapterService.Save(chap);
                    UpdateSubRoute(chap);
                }
            }

            var subLessons = _lessonService.GetChapterLesson(data.ID);
            if (subLessons != null && subLessons.Count() > 0)
            {
                foreach (var lesson in subLessons)
                {
                    //if (string.IsNullOrEmpty(lesson.ConnectID))//direct child
                    //{
                    //    lesson.Start = data.Start;
                    //    if (lesson.Period == 0 || lesson.Period + lesson.Start > data.Start + data.Period)
                    //        lesson.Period = data.Period;
                    //}
                    //else
                    //{
                    //    if (lesson.Start > data.Start + data.Period)
                    //    {
                    //        lesson.Start = data.Start + data.Period;
                    //        lesson.Period = 0;
                    //    }
                    //    if (lesson.Start + lesson.Period > data.Start + data.Period)
                    //        lesson.Period = data.Start + data.Period - lesson.Start;//resize route to fit parent
                    //}
                    //if (string.IsNullOrEmpty(lesson.ConnectID))//direct child
                    //{
                    //    lesson.Start = data.Start;
                    //}
                    //if (lesson.Period > 0)//route set
                    //    if (lesson.Start + lesson.Period > data.Start + data.Period)
                    //        lesson.Period = data.Start + data.Period - lesson.Start;//resize route to fit parent
                    //lesson.Period = data.Period;
                    lesson.Start = data.Start;
                    lesson.Period = data.Period;
                    _lessonService.Save(lesson);
                }
            }
        }

        //private void UpdateRoute(CourseLessonEntity data)
        //{
        //    var connectedChaps = _chapterService.GetItemByConnectID(data.CourseID, data.ChapterID, data.ID);
        //    if (connectedChaps != null && connectedChaps.Count() > 0)
        //    {
        //        foreach (var chap in connectedChaps)
        //        {
        //            chap.Start = data.Start + data.Period;
        //            _chapterService.Save(chap);
        //            UpdateRoute(chap);
        //        }
        //    }

        //    var connectedLessons = _lessonService.GetItemByConnectID(data.CourseID, data.ChapterID, data.ID);
        //    if (connectedLessons != null && connectedLessons.Count() > 0)
        //    {
        //        foreach (var lesson in connectedLessons)
        //        {
        //            lesson.Start = data.Start + data.Period;
        //            UpdateRoute(lesson);
        //        }
        //    }
        //}

        [HttpPost]
        public async Task<JsonResult> JoinChapter(string ID, string JoinChapter, string newName, string CreateNewChapter = "off")
        {
            try
            {
                var _userCreate = User.Claims.GetClaimByType("UserID").Value;
                var rootChap = _chapterService.GetItemByID(ID);
                var joinChap = _chapterService.GetItemByID(JoinChapter);
                if (rootChap == null || joinChap == null)
                {
                    return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", null },
                        { "Error", "Dữ liệu không đúng" }
                    });
                }
                var currentChapIndex = (int)_chapterService.GetSubChapters(rootChap.CourseID, rootChap.ParentID).Count();
                var currentLessonIndex = (int)_lessonService.CountChapterLesson(rootChap.ID);

                var joinLessons = _lessonService.GetChapterLesson(joinChap.ID).OrderBy(o => o.Order);
                var joinSubChaps = _chapterService.GetSubChapters(joinChap.CourseID, joinChap.ID);

                if (CreateNewChapter.Equals("on"))
                {
                    var clonechap = _cloneCourseChapterMapping.Clone(rootChap, new CourseChapterEntity());
                    if (newName != null || newName != "")
                        clonechap.Name = newName;
                    clonechap.Order = currentChapIndex;
                    clonechap.OriginID = rootChap.ID;

                    var newChapter = await CloneChapter(clonechap, _userCreate, rootChap.CourseID); ;

                    var lessonMapping = new MappingEntity<CourseLessonEntity, CourseLessonEntity>();
                    //var new_lesson = new CourseLessonEntity();
                    if (joinLessons != null && joinLessons.Count() > 0)
                        foreach (var o in joinLessons)
                        {
                            var new_lesson = lessonMapping.Clone(o, new CourseLessonEntity());
                            new_lesson.CreateUser = _userCreate;
                            new_lesson.Created = DateTime.UtcNow;
                            new_lesson.ChapterID = newChapter.ID;
                            new_lesson.OriginID = o.ID;
                            new_lesson.Order = currentLessonIndex++;
                            await CloneLesson(new_lesson, _userCreate);
                        }
                    if (joinSubChaps != null && joinSubChaps.Count() > 0)
                        foreach (var o in joinSubChaps)
                        {
                            var clone_chap = _cloneCourseChapterMapping.Clone(o, new CourseChapterEntity());
                            clone_chap.OriginID = o.ID;
                            clone_chap.ParentID = newChapter.ID;
                            clone_chap.Created = DateTime.UtcNow;
                            clone_chap.CreateUser = _userCreate;
                            clone_chap.Order = currentChapIndex++;
                            await CloneChapter(clone_chap, _userCreate, rootChap.CourseID);
                        }

                    //update new chapter counter
                    newChapter.TotalExams += joinChap.TotalExams;
                    newChapter.TotalLessons += joinChap.TotalLessons;
                    newChapter.TotalPractices += joinChap.TotalPractices;
                    _chapterService.Save(newChapter);

                    //add join chapter counter to parent holder
                    if (newChapter.ParentID == "0")
                        await _courseHelper.IncreaseCourseCounter(newChapter.CourseID, newChapter.TotalLessons, newChapter.TotalExams, newChapter.TotalPractices);
                    else
                        await _courseHelper.IncreaseCourseChapterCounter(newChapter.ParentID, newChapter.TotalLessons, newChapter.TotalExams, newChapter.TotalPractices);

                    return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", _chapterService.GetItemByID(newChapter.ID)},
                        { "Error", null }
                    });
                }
                else
                {
                    //Append joinChapter's lessons to bottom of rootchap's lessons list
                    if (joinLessons != null && joinLessons.Count() > 0)
                        foreach (var lesson in joinLessons.ToList())
                        {
                            lesson.ChapterID = rootChap.ID;
                            lesson.Order = (int)currentLessonIndex++;
                            _lessonService.Save(lesson);
                        }
                    //Append joinChapter's subchapter to bottom of rootchap's subchapter list
                    if (joinSubChaps != null && joinSubChaps.Count() > 0)
                        foreach (var subchap in joinSubChaps)
                        {
                            subchap.ParentID = rootChap.ParentID;
                            subchap.Order = (int)currentChapIndex++;
                            _chapterService.Save(subchap);
                            //var clone_chap = _cloneCourseChapterMapping.Clone(o, new CourseChapterEntity());
                            //clone_chap.OriginID = o.ID;
                            //clone_chap.ParentID = rootChap.ID;
                            //clone_chap.Created = DateTime.UtcNow;
                            //clone_chap.CreateUser = _userCreate;
                            //clone_chap.Order = currentChapIndex++;
                            //await CloneChapter(clone_chap, _userCreate, rootChap.CourseID);
                        }

                    //add joinchap counter to root counter
                    rootChap.TotalExams += joinChap.TotalExams;
                    rootChap.TotalLessons += joinChap.TotalLessons;
                    rootChap.TotalPractices += joinChap.TotalPractices;
                    _chapterService.Save(rootChap);
                    //await _courseHelper.IncreaseCourseChapterCounter(rootChap.ID, joinChap.TotalLessons, joinChap.TotalExams, joinChap.TotalPractices);

                    //Move joinChapter to bottom of parent chapter list to correct order
                    ChangeChapterPosition(joinChap, int.MaxValue);
                    //Then remove join chapter
                    _chapterService.Remove(joinChap.ID);

                    return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", _chapterService.GetItemByID(rootChap.ID) },
                        { "Error", null }
                    });
                }
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
        public async Task<JsonResult> CopyChapter(string ChapID, string CourseID)
        {
            var orgChapter = _chapterService.GetItemByID(ChapID);
            if (orgChapter == null)
            {
                return new JsonResult(new Dictionary<string, object>
                    {
                        {"Error", "Không tìm thấy thông tin" }
                    });
            }
            else
            {
                var clone_chap = _cloneCourseChapterMapping.Clone(orgChapter, new CourseChapterEntity());
                clone_chap.OriginID = orgChapter.ID;
                clone_chap.Order = (int)_chapterService.GetSubChapters(orgChapter.CourseID, orgChapter.ParentID).Count();
                var chapter = await CloneChapter(clone_chap, orgChapter.CreateUser, orgChapter.CourseID);
                if (chapter.TotalLessons > 0)
                {
                    if (chapter.ParentID == "0")
                        await _courseHelper.IncreaseCourseCounter(chapter.CourseID, chapter.TotalLessons, chapter.TotalExams, chapter.TotalPractices);
                    else
                        await _courseHelper.IncreaseCourseChapterCounter(chapter.ParentID, chapter.TotalLessons, chapter.TotalExams, chapter.TotalPractices);
                }


                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", chapter },
                    { "Error", null }
                });
            }
        }

        [HttpPost]
        public async Task<JsonResult> RemoveChapter(DefaultModel model)
        {
            try
            {
                var ID = model.ArrID;
                var chapter = _chapterService.GetItemByID(ID);
                if (chapter == null)
                {
                    return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", null },
                                {"Error", null }
                            });
                }
                //Decrease counter
                if (chapter.TotalLessons > 0)
                    if (chapter.ParentID == "0")
                        await _courseHelper.IncreaseCourseCounter(chapter.CourseID, 0 - chapter.TotalLessons, 0 - chapter.TotalExams, 0 - chapter.TotalPractices);
                    else
                        await _courseHelper.IncreaseCourseChapterCounter(chapter.ParentID, 0 - chapter.TotalLessons, 0 - chapter.TotalExams, 0 - chapter.TotalPractices);

                //Remove chapter
                await RemoveCourseChapter(chapter);
                return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", ID },
                                {"Error", null }
                            });
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

        /*--- FUNC ---*/
        private async Task<CourseChapterEntity> CloneChapter(CourseChapterEntity item, string _userCreate, string orgCourseID)
        {
            if (item.OriginID != "0")
                _chapterService.Collection.InsertOne(item);
            else
            {
                item.ID = "0";
            }

            var lessons = _lessonService.GetChapterLesson(orgCourseID, item.OriginID);

            if (lessons != null && lessons.Count() > 0)
            {
                foreach (var o in lessons)
                {
                    var new_lesson = _cloneCourseLessonMapping.Clone(o, new CourseLessonEntity());
                    new_lesson.CourseID = item.CourseID;
                    new_lesson.ChapterID = item.ID;
                    new_lesson.CreateUser = _userCreate;
                    new_lesson.Created = DateTime.UtcNow;
                    new_lesson.OriginID = o.ID;
                    await CloneLesson(new_lesson, _userCreate);
                }
            }

            var subChapters = _chapterService.GetSubChapters(orgCourseID, item.OriginID);
            foreach (var o in subChapters)
            {
                var new_chapter = _cloneCourseChapterMapping.Clone(o, new CourseChapterEntity());
                new_chapter.CourseID = item.CourseID;
                new_chapter.ParentID = item.ID;
                new_chapter.CreateUser = _userCreate;
                new_chapter.Created = DateTime.UtcNow;
                new_chapter.OriginID = o.ID;
                await CloneChapter(new_chapter, _userCreate, orgCourseID);
            }
            return item;
        }

        private async Task RemoveCourseChapter(CourseChapterEntity chap)
        {
            //_lessonService.CreateQuery().DeleteMany(o => o.ChapterID == chap.ID);
            var lessons = _lessonService.GetChapterLesson(chap.ID);
            if (lessons != null && lessons.Count() > 0)
                foreach (var lesson in lessons)
                    _ = RemoveSingleLesson(lesson);

            var subchapters = _chapterService.CreateQuery().Find(o => o.ParentID == chap.ID).ToList();
            if (subchapters != null && subchapters.Count > 0)
                foreach (var chapter in subchapters)
                    await RemoveCourseChapter(chapter);
            ChangeChapterPosition(chap, int.MaxValue);
            await _chapterService.RemoveAsync(chap.ID);
        }

        private int ChangeChapterPosition(CourseChapterEntity item, int pos)
        {
            var parts = new List<CourseChapterEntity>();
            parts = _chapterService.CreateQuery().Find(o => o.CourseID == item.CourseID && o.ParentID == item.ParentID)
                .SortBy(o => o.Order).ThenBy(o => o.ID).ToList();

            var ids = parts.Select(o => o.ID).ToList();

            var oldPos = ids.IndexOf(item.ID);
            if (oldPos == pos && (item.Order == pos))
                return oldPos;

            if (pos > parts.Count() + 1)
                pos = parts.Count() - 1;
            item.Order = pos;

            _chapterService.CreateQuery().ReplaceOne(o => o.ID == item.ID, item);
            int entry = -1;
            foreach (var part in parts)
            {
                if (part.ID == item.ID) continue;
                if (entry == pos - 1)
                    entry++;
                entry++;
                part.Order = entry;
                _chapterService.CreateQuery().ReplaceOne(o => o.ID == part.ID, part);
            }
            return pos;
        }

        #endregion Chapter

        #region Lesson
        /*--- API ---*/
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
        public JsonResult CreateOrUpdateLesson(CourseLessonEntity item)
        {
            try
            {
                var needUpdate = true;

                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var data = _lessonService.GetItemByID(item.ID);

                CourseChapterEntity parent = null;
                if (data.ChapterID != "0")
                {
                    parent = _chapterService.GetItemByID(data.ChapterID);
                }

                if (data == null)
                {
                    item.Created = DateTime.UtcNow;
                    item.CreateUser = UserID;
                    item.IsAdmin = true;
                    item.IsActive = false;
                    item.IsParentCourse = item.ChapterID.Equals("0");
                    item.Updated = DateTime.UtcNow;
                    item.Order = 0;
                    _lessonService.CreateQuery().InsertOne(item);
                    item.Period = 7;
                    if (item.Start <= 0)
                    {
                        item.Start = 0;
                        item.Period = 0;
                    }
                    

                    //if (item.Start > 0)
                    //    needUpdate = true;

                    ChangeLessonPosition(item, Int32.MaxValue);//move lesson to bottom of parent

                    //update total lesson to parent chapter
                    if (!string.IsNullOrEmpty(item.ChapterID) && item.ChapterID != "0")
                        _ = _courseHelper.IncreaseCourseChapterCounter(item.ChapterID, 1, item.TemplateType == LESSON_TEMPLATE.EXAM ? 1 : 0, 0);
                    else
                        _ = _courseHelper.IncreaseCourseCounter(item.CourseID, 1, item.TemplateType == LESSON_TEMPLATE.EXAM ? 1 : 0, 0);
                    data = item;
                }
                else
                {
                    var oldTemplate = data.TemplateType;
                    data.TemplateType = item.TemplateType;
                    data.Title = item.Title;
                    data.Timer = item.Timer;
                    data.Multiple = item.Multiple;
                    data.Etype = item.Etype;
                    data.Limit = item.Limit;
                    data.ConnectType = item.ConnectType;

                    data.Period = 7;

                    //if (data.Start != data.Start)
                    //{
                    //    needUpdate = true;
                    //}
                    if (item.Start < 0)//unset
                    {
                        item.Start = 0;
                        data.Period = 0;
                    }

                    data.Start = item.Start;
                    data.ConnectID = "";


                    //if (data.TemplateType == LESSON_TEMPLATE.LECTURE)
                    //    data.Limit = 0;

                    data.Updated = DateTime.UtcNow;

                    var newOrder = item.Order - 1;

                    //update counter if type change
                    if (item.TemplateType != oldTemplate)
                    {
                        var examInc = 0;
                        var pracInc = 0;
                        if (IsLessonHasQuiz(item.ID)) pracInc = 1;
                        if (item.TemplateType == LESSON_TEMPLATE.LECTURE) // EXAM => LECTURE
                        {
                            examInc = -1;
                            data.IsPractice = pracInc == 1;
                        }
                        else
                        {
                            examInc = 1;
                            data.IsPractice = false;
                            pracInc = pracInc == 1 ? -1 : 0;
                        }
                        if (!string.IsNullOrEmpty(data.ChapterID) && data.ChapterID != "0")
                            _ = _courseHelper.IncreaseCourseChapterCounter(data.ChapterID, 0, examInc, pracInc);
                        else
                            _ = _courseHelper.IncreaseCourseCounter(data.CourseID, 0, examInc, pracInc);
                    }

                    _lessonService.Save(data);

                    if (needUpdate)
                    {
                        UpdateRoute(data, parent);
                    }

                    //_lessonService.CreateQuery().ReplaceOne(o => o.ID == data.ID, data);

                    if (data.Order != newOrder)//change Position
                    {
                        ChangeLessonPosition(data, newOrder);
                    }
                }

                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", data },
                    {"Error",null }
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
        public async Task<JsonResult> JoinLesson(string ID, string JoinLesson)
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

                var hasPractice = false;
                if (joinParts != null && joinParts.Count > 0)
                {
                    foreach (var part in joinParts)
                    {
                        if (quizTypes.Contains(part.Type))
                            hasPractice = true;
                        part.ParentID = rootItem.ID;
                        part.Order = (int)currentIndex++;
                        _lessonPartService.CreateQuery().ReplaceOne(o => o.ID == part.ID, part);
                    }
                }

                ChangeLessonPosition(joinItem, int.MaxValue);//chuyển lesson xuống cuối của đối tượng chứa

                //change counter
                if (rootItem.TemplateType == LESSON_TEMPLATE.LECTURE)
                    if (!rootItem.IsPractice && hasPractice)
                    {
                        rootItem.IsPractice = true;
                        _lessonService.Save(rootItem);
                        if (rootItem.ChapterID == "0")
                            await _courseHelper.IncreaseCourseCounter(rootItem.CourseID, 0, 0, 1);
                        else
                            await _courseHelper.IncreaseCourseChapterCounter(rootItem.ChapterID, 0, 0, 1);
                    }

                //decrease counter
                if (joinItem.ChapterID == "0")
                    await _courseHelper.IncreaseCourseCounter(joinItem.CourseID, -1, 0 - joinItem.TemplateType == LESSON_TEMPLATE.EXAM ? 1 : 0, 0 - (joinItem.IsPractice ? 1 : 0));
                else
                    await _courseHelper.IncreaseCourseChapterCounter(joinItem.ChapterID, -1, 0 - joinItem.TemplateType == LESSON_TEMPLATE.EXAM ? 1 : 0, 0 - (joinItem.IsPractice ? 1 : 0));

                //remove all lesson content from db
                await RemoveSingleLesson(joinItem);

                return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", joinItem },
                        { "Del", JoinLesson },
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
        public async Task<JsonResult> CopyLesson(string ArrID, string Title, string ChapterID, string CourseID)
        {
            var orgLesson = _lessonService.GetItemByID(ArrID);
            var new_lesson = _cloneCourseLessonMapping.Clone(orgLesson, new CourseLessonEntity());
            new_lesson.OriginID = orgLesson.ID;
            new_lesson.ChapterID = ChapterID;
            new_lesson.Created = DateTime.UtcNow;
            new_lesson.Order = (int)_lessonService.CountChapterLesson(ChapterID);
            new_lesson.Title = string.IsNullOrEmpty(Title) ? (orgLesson.Title + (orgLesson.ChapterID == ChapterID ? " (copy)" : "")) : Title;
            await CloneLesson(new_lesson, orgLesson.CreateUser);
            if (!string.IsNullOrEmpty(new_lesson.ChapterID) && new_lesson.ChapterID != "0")
                _ = _courseHelper.IncreaseCourseChapterCounter(new_lesson.ChapterID, 1, new_lesson.TemplateType == LESSON_TEMPLATE.EXAM ? 1 : 0, new_lesson.IsPractice ? 1 : 0);
            else
                _ = _courseHelper.IncreaseCourseCounter(new_lesson.CourseID, 1, new_lesson.TemplateType == LESSON_TEMPLATE.EXAM ? 1 : 0, new_lesson.IsPractice ? 1 : 0);
            return new JsonResult("OK");
        }

        [HttpPost]
        public async Task<JsonResult> RemoveLesson(DefaultModel model, string ID)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.ArrID))
                    ID = model.ArrID;
                var lesson = _lessonService.GetItemByID(ID);//TODO: check permission
                if (lesson != null)
                {
                    if (lesson.ChapterID == "0")
                        await _courseHelper.IncreaseCourseCounter(lesson.CourseID, -1, lesson.TemplateType == LESSON_TEMPLATE.EXAM ? -1 : 0, lesson.IsPractice ? -1 : 0);
                    else
                        await _courseHelper.IncreaseCourseChapterCounter(lesson.ChapterID, -1, lesson.TemplateType == LESSON_TEMPLATE.EXAM ? -1 : 0, lesson.IsPractice ? -1 : 0);
                    await RemoveSingleLesson(lesson);

                    return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", ID },
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

        /*--- FUNC ---*/
        private int ChangeLessonPosition(CourseLessonEntity item, int pos)
        {
            var parts = _lessonService.GetChapterLesson(item.CourseID, item.ChapterID);
            var ids = parts.Select(o => o.ID).ToList();

            var oldPos = ids.IndexOf(item.ID);
            if (oldPos == pos && oldPos == item.Order)
                return oldPos;

            if (pos > parts.Count() + 1)
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

        private async Task CloneLesson(CourseLessonEntity item, string _userCreate)
        {
            _lessonService.CreateQuery().InsertOne(item);

            var parts = _lessonPartService.CreateQuery().Find(o => o.ParentID == item.OriginID);
            foreach (var _child in parts.ToEnumerable())
            {
                var _item = new LessonPartEntity()
                {
                    OriginID = _child.ID,
                    Title = _child.Title,
                    Description = _child.Description,
                    Media = _child.Media,
                    Point = _child.Point,
                    Order = _child.Order,
                    ParentID = item.ID,
                    Timer = _child.Timer,
                    Type = _child.Type,
                    Updated = DateTime.UtcNow,
                    Created = DateTime.UtcNow,
                    CourseID = item.CourseID,
                };

                await CloneLessonPart(_item, _userCreate);
            }
        }

        private async Task CloneLessonPart(LessonPartEntity item, string _userCreate)
        {
            _lessonPartService.Collection.InsertOne(item);
            var questions = _lessonPartQuestionService.CreateQuery().Find(o => o.ParentID == item.OriginID);
            foreach (var _child in questions.ToEnumerable())
            {
                var _item = new LessonPartQuestionEntity()
                {
                    OriginID = _child.ID,
                    Content = _child.Content,
                    CreateUser = _userCreate,
                    Description = _child.Description,
                    Media = _child.Media,
                    Point = _child.Point,
                    Order = _child.Order,
                    ParentID = item.ID,
                    Updated = DateTime.UtcNow,
                    Created = DateTime.UtcNow,
                    CourseID = item.CourseID,
                };
                ////change Media path
                //if (_item.Media != null && _item.Media.Path != null)
                //    if (!_item.Media.Path.StartsWith("http://"))
                //        _item.Media.Path = "http://" + _publisherHost + _item.Media.Path;
                await CloneLessonQuestion(_item, _userCreate);
            }
        }

        private async Task CloneLessonQuestion(LessonPartQuestionEntity item, string _userCreate)
        {
            _lessonPartQuestionService.Collection.InsertOne(item);
            var answers = _lessonPartAnswerService.CreateQuery().Find(o => o.ParentID == item.OriginID);
            foreach (var _child in answers.ToEnumerable())
            {
                var _item = new LessonPartAnswerEntity()
                {
                    OriginID = _child.ID,
                    Content = _child.Content,
                    CreateUser = _userCreate,
                    IsCorrect = _child.IsCorrect,
                    Media = _child.Media,
                    Order = _child.Order,
                    ParentID = item.ID,
                    Updated = DateTime.UtcNow,
                    Created = DateTime.UtcNow,
                    CourseID = item.CourseID
                };
                //if (_item.Media != null && _item.Media.Path != null)
                //    //if (!_item.Media.Path.StartsWith("http://"))
                //    //    _item.Media.Path = "http://" + _publisherHost + _item.Media.Path;
                //    if (_item.Media.Path.StartsWith("http://"))
                //        _item.Media.Path = "http://" + _publisherHost + _item.Media.Path;
                await CloneLessonAnswer(_item);
            }
        }

        private async Task CloneLessonAnswer(LessonPartAnswerEntity item)
        {
            await _lessonPartAnswerService.Collection.InsertOneAsync(item);
        }

        private async Task RemoveSingleLesson(CourseLessonEntity lesson)
        {
            var lessonparts = _lessonPartService.CreateQuery().Find(o => o.ParentID == lesson.ID).ToList();
            if (lessonparts != null && lessonparts.Count > 0)
                for (int i = 0; lessonparts != null && i < lessonparts.Count; i++)
                    RemoveLessonPart(lessonparts[i].ID);
            ChangeLessonPosition(lesson, int.MaxValue);//chuyển lesson xuống cuối của đối tượng chứa

            await _lessonService.RemoveAsync(lesson.ID);
        }

        private void RemoveLessonPart(string ID)
        {
            try
            {
                var item = _lessonPartService.GetItemByID(ID);
                if (item == null) return;

                var questions = _lessonPartQuestionService.GetByPartID(item.ID);

                foreach (var question in questions)
                    RemoveQuestion(question.ID);
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
                var item = _lessonPartQuestionService.CreateQuery().Find(o => o.ID == ID).SingleOrDefault();
                if (item == null) return;
                _lessonPartAnswerService.CreateQuery().DeleteMany(o => o.ParentID == ID);
                _lessonPartQuestionService.Remove(ID);
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
                var item = _lessonPartAnswerService.CreateQuery().Find(o => o.ID == ID).SingleOrDefault();
                if (item == null) return;
                _lessonPartAnswerService.Remove(ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsLessonHasQuiz(string ID)
        {
            return _lessonPartService.GetByLessonID(ID).Any(t => quizTypes.Contains(t.Type));
        }
        #endregion

        #region PUBLISHER

        [HttpPost]
        //CLONE MOD COURSE
        public async Task<JsonResult> Clone(string CourseID, CourseEntity newcourse)
        {
            var _userCreate = User.Claims.GetClaimByType("UserID").Value;
            var course = _modservice.GetItemByID(CourseID); //publisher
            if (course != null)
            {
                //var grade = _modgradeService.GetItemByID(item.GradeID);
                //var subject = _modsubjectService.GetItemByID(item.SubjectID);
                //var programe = _modprogramService.GetItemByID(item.ProgramID);
                var chapter_root = _modchapterService.CreateQuery().Find(o => o.CourseID == CourseID && o.ParentID == "0");
                var lesson_root = _modlessonService.CreateQuery().Find(o => o.CourseID == CourseID && o.ChapterID == "0");

                var clone_course = new CourseEntity()
                {
                    OriginID = course.ID,
                    Name = newcourse.Name,
                    Code = course.Code,
                    Description = newcourse.Description,
                    GradeID = newcourse.GradeID,
                    TeacherID = _userCreate,
                    SubjectID = newcourse.SubjectID,
                    CreateUser = _userCreate,
                    SkillID = newcourse.SkillID,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow,
                    IsActive = true,
                    IsAdmin = false,
                    Order = course.Order,
                };
                _service.Collection.InsertOne(clone_course);

                foreach (var chapter in chapter_root.ToEnumerable())
                {
                    await CloneModChapter(new ChapterEntity()
                    {
                        OriginID = chapter.ID,
                        Name = chapter.Name,
                        Code = chapter.Code,
                        CourseID = clone_course.ID,
                        ParentID = chapter.ParentID,
                        ParentType = chapter.ParentType,
                        CreateUser = _userCreate,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow,
                        IsActive = true,
                        IsAdmin = false,
                        Order = chapter.Order
                    }, _userCreate);
                }

                foreach (var o in lesson_root.ToEnumerable())
                {
                    await CloneModLesson(new LessonEntity()
                    {
                        Media = o.Media,
                        ChapterID = "0",
                        CreateUser = _userCreate,
                        Code = o.Code,
                        OriginID = o.ID,
                        CourseID = CourseID,
                        IsParentCourse = o.IsParentCourse,
                        IsAdmin = false,
                        Timer = o.Timer,
                        Point = o.Point,
                        IsActive = o.IsActive,
                        Title = o.Title,
                        TemplateType = o.TemplateType,
                        Order = o.Order,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    }, _userCreate);
                }
            }
            return new JsonResult("OK");
        }


        [HttpPost]
        public JsonResult GetModList(DefaultModel model, string SubjectID = "", string GradeID = "")
        {
            var filter = new List<FilterDefinition<ModCourseEntity>>();

            var UserID = User.Claims.GetClaimByType("UserID").Value;

            if (!string.IsNullOrEmpty(SubjectID))
            {
                filter.Add(Builders<ModCourseEntity>.Filter.Where(o => o.SubjectID == SubjectID));
            }
            if (!string.IsNullOrEmpty(GradeID))
            {
                filter.Add(Builders<ModCourseEntity>.Filter.Where(o => o.GradeID == GradeID));
            }

            var data = filter.Count > 0 ? _modservice.Collection.Find(Builders<ModCourseEntity>.Filter.And(filter)) : _modservice.GetAll();
            model.TotalRecord = data.CountDocuments();
            var DataResponse = data == null || model.TotalRecord <= 0 || model.TotalRecord < model.PageSize
                ? data
                : data.Skip((model.PageIndex - 1) * model.PageSize).Limit(model.PageSize);

            var response = new Dictionary<string, object>
            {
                { "Data", DataResponse.ToList() },
                { "Model", model }
            };
            return new JsonResult(response);
        }

        private async Task CloneModChapter(ChapterEntity item, string _userCreate)
        {
            _chapterService.Collection.InsertOne(item);

            var lessons = _modlessonService.CreateQuery().Find(o => o.ChapterID == item.OriginID);

            foreach (var o in lessons.ToEnumerable())
            {
                await CloneModLesson(new LessonEntity()
                {
                    Media = o.Media,
                    ChapterID = item.ID,
                    CreateUser = _userCreate,
                    Code = o.Code,
                    OriginID = o.ID,
                    CourseID = item.CourseID,
                    IsParentCourse = o.IsParentCourse,
                    IsAdmin = false,
                    Timer = o.Timer,
                    Point = o.Point,
                    IsActive = o.IsActive,
                    Title = o.Title,
                    TemplateType = o.TemplateType,
                    Order = o.Order,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                }, _userCreate);
            }

            var subChapters = _modchapterService.Collection.Find(o => o.ParentID == item.OriginID);
            foreach (var o in subChapters.ToEnumerable())
            {
                await CloneModChapter(new ChapterEntity()
                {
                    OriginID = o.ID,
                    Name = o.Name,
                    Code = o.Code,
                    CourseID = item.CourseID,
                    ParentID = item.ID, //Edit by VietPhung 20190701
                    ParentType = o.ParentType,
                    CreateUser = _userCreate,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow,
                    IsActive = true,
                    IsAdmin = false,
                    Order = o.Order
                }, _userCreate);
            }
        }

        private async Task CloneModLesson(LessonEntity item, string _userCreate)
        {
            throw new Exception("TEMPORARY DISABLED");

            if (item.Media != null && item.Media.Path != null)
                if (!item.Media.Path.StartsWith("http://"))
                    item.Media.Path = "http://" + _publisherHost + item.Media.Path;
            //TODO: Double check
            _lessonService.CreateQuery().InsertOne(item);

            //if (!string.IsNullOrEmpty(item.ChapterID) && item.ChapterID != "0")
            //    _ = _chapterService.IncreaseLessonCounter(item.ChapterID, 1, 0, 0);
            //else
            //    _ = _service.IncreaseLessonCounter(item.CourseID, 1, 0, 0);


            var parts = _modlessonPartService.CreateQuery().Find(o => o.ParentID == item.OriginID);
            foreach (var _child in parts.ToEnumerable())
            {
                var _item = new LessonPartEntity()
                {
                    OriginID = _child.ID,
                    Title = _child.Title,
                    Description = _child.Description != null ? _child.Description.Replace("src=\"/", "src=\"http://" + _publisherHost + "/") : null,
                    Media = _child.Media,
                    Point = _child.Point,
                    Order = _child.Order,
                    ParentID = item.ID,
                    Timer = _child.Timer,
                    Type = _child.Type,
                    Updated = DateTime.UtcNow,
                    Created = DateTime.UtcNow,
                    CourseID = item.CourseID
                };
                if (_item.Media != null && _item.Media.Path != null)
                    if (!_item.Media.Path.StartsWith("http://"))
                        _item.Media.Path = "http://" + _publisherHost + _item.Media.Path;
                await CloneModLessonPart(_item, _userCreate);
            }
        }

        private async Task CloneModLessonPart(LessonPartEntity item, string _userCreate)
        {
            _lessonPartService.Collection.InsertOne(item);
            var questions = _modlessonPartQuestionService.CreateQuery().Find(o => o.ParentID == item.OriginID);
            foreach (var _child in questions.ToEnumerable())
            {
                var _item = new LessonPartQuestionEntity()
                {
                    OriginID = _child.ID,
                    Content = _child.Content,
                    CreateUser = _userCreate,
                    Description = _child.Description != null ? _child.Description.Replace("src=\"/", "src=\"http://" + _publisherHost + "/") : null,
                    Media = _child.Media,
                    Point = _child.Point,
                    Order = _child.Order,
                    ParentID = item.ID,
                    Updated = DateTime.UtcNow,
                    Created = DateTime.UtcNow,
                    CourseID = item.CourseID,
                };
                //change Media path
                if (_item.Media != null && _item.Media.Path != null)
                    if (!_item.Media.Path.StartsWith("http://"))
                        _item.Media.Path = "http://" + _publisherHost + _item.Media.Path;
                await CloneModLessonQuestion(_item, _userCreate);
            }
        }

        private async Task CloneModLessonQuestion(LessonPartQuestionEntity item, string _userCreate)
        {
            _lessonPartQuestionService.Collection.InsertOne(item);
            var answers = _modlessonPartAnswerService.CreateQuery().Find(o => o.ParentID == item.OriginID);
            foreach (var _child in answers.ToEnumerable())
            {
                var _item = new LessonPartAnswerEntity()
                {
                    OriginID = _child.ID,
                    Content = _child.Content,
                    CreateUser = _userCreate,
                    IsCorrect = _child.IsCorrect,
                    Media = _child.Media,
                    Order = _child.Order,
                    ParentID = item.ID,
                    Updated = DateTime.UtcNow,
                    Created = DateTime.UtcNow,
                    CourseID = item.CourseID
                };
                if (_item.Media != null && _item.Media.Path != null)
                    if (!_item.Media.Path.StartsWith("http://"))
                        _item.Media.Path = "http://" + _publisherHost + _item.Media.Path;
                await CloneLessonAnswer(_item);
            }
        }
        #endregion

        #region Import with Excel
        public IActionResult ExportQuestionTemplate(DefaultModel model)
        {
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("Mau cau hoi");
                //workSheet.InsertRow(1, 3);
                //header

                workSheet.Cells[1, 1].Value = "STT";
                workSheet.Cells[1, 2].Value = "Nội dung";
                workSheet.Cells[1, 3].Value = "Liên kết";
                workSheet.Cells[1, 4].Value = "Đúng/sai";
                workSheet.Cells[1, 5].Value = "Giải thích";

                var headerCells = workSheet.Cells[1, 1, 1, workSheet.Dimension.Columns];
                headerCells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                headerCells.Style.Font.Bold = true;

                var col1 = workSheet.Column(1);
                col1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                col1.Width = 10;

                var col2 = workSheet.Column(2);
                col2.Width = 40;

                var col3 = workSheet.Column(3);
                col3.Width = 40;

                var col4 = workSheet.Column(4);
                col4.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                col4.Width = 10;

                var col5 = workSheet.Column(5);
                col5.Width = 60;

                //question template
                workSheet.Cells[2, 1].Value = "1";
                workSheet.Cells[2, 2].Value = "Câu hỏi 1";
                workSheet.Cells[2, 3].Value = "https://eduso.vn/images/quiz_example.png";
                workSheet.Cells[2, 4].Value = "";
                workSheet.Cells[2, 5].Value = "Giải thích đáp án câu 1";
                //answer template
                workSheet.Cells[3, 1].Value = "";
                workSheet.Cells[3, 2].Value = "Nội dung trả lời 1";
                workSheet.Cells[3, 3].Value = "https://eduso.vn/images/example.png";
                workSheet.Cells[3, 4].Value = "TRUE";


                workSheet.Cells[4, 1].Value = "";
                workSheet.Cells[4, 2].Value = "Nội dung trả lời 2";
                workSheet.Cells[4, 3].Value = "https://eduso.vn/images/example.png";
                workSheet.Cells[4, 4].Value = "FALSE";

                workSheet.Cells[5, 1].Value = "";
                workSheet.Cells[5, 2].Value = "Nội dung trả lời 3";
                workSheet.Cells[5, 3].Value = "https://eduso.vn/images/example.png";
                workSheet.Cells[5, 4].Value = "FALSE";

                //question template
                workSheet.Cells[6, 1].Value = "2";
                workSheet.Cells[6, 2].Value = "Câu hỏi 2";
                workSheet.Cells[6, 3].Value = "https://eduso.vn/images/quiz_example.png";
                workSheet.Cells[6, 4].Value = "";
                workSheet.Cells[6, 5].Value = "Giải thích đáp án câu 2";

                //answer template
                workSheet.Cells[7, 1].Value = "";
                workSheet.Cells[7, 2].Value = "Nội dung trả lời 1 - Câu 2";
                workSheet.Cells[7, 3].Value = "https://eduso.vn/images/example.png";
                workSheet.Cells[7, 4].Value = "FALSE";

                workSheet.Cells[8, 1].Value = "";
                workSheet.Cells[8, 2].Value = "Nội dung trả lời 2 - Câu 2";
                workSheet.Cells[8, 3].Value = "https://eduso.vn/images/example.png";
                workSheet.Cells[8, 4].Value = "FALSE";

                workSheet.Cells[9, 1].Value = "";
                workSheet.Cells[9, 2].Value = "Nội dung trả lời 3 - Câu 2";
                workSheet.Cells[9, 3].Value = "https://eduso.vn/images/example.png";
                workSheet.Cells[9, 4].Value = "FALSE";


                workSheet.Cells[11, 1].Value = "Lưu ý: Câu hỏi sẽ có số thứ tự; các dòng ngay sau câu hỏi là câu trả lời của câu hỏi";
                workSheet.Cells[12, 1].Value = "Liên kết hình ảnh/media có dạng http://... hoặc https://...";
                var note = workSheet.Cells[11, 1, 12, workSheet.Dimension.Columns];
                note.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                note.Style.Font.Italic = true;


                package.Save();
            }
            stream.Position = 0;
            string excelName = $"QuizTemplate.xlsx";

            //return File(stream, "application/octet-stream", excelName);  
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        public async Task<JsonResult> ImportQuestion()
        {
            var form = HttpContext.Request.Form;

            if (form == null || form.Files == null || form.Files.Count <= 0)
                return new JsonResult(new Dictionary<string, object> { { "Error", "Chưa chọn file" } });
            var file = form.Files[0];
            var dirPath = "Upload\\Quiz";
            if (!Directory.Exists(Path.Combine(_env.WebRootPath, dirPath)))
                Directory.CreateDirectory(Path.Combine(_env.WebRootPath, dirPath));
            var filePath = Path.Combine(_env.WebRootPath, dirPath + "\\" + DateTime.UtcNow.ToString("ddMMyyyyhhmmss") + file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
                stream.Close();
                try
                {
                    var pos = -1;
                    var full_item = new LessonPartViewModel()
                    {
                        Questions = new List<QuestionViewModel>()
                    };
                    using (var readStream = new FileStream(filePath, FileMode.Open))
                    {
                        using (ExcelPackage package = new ExcelPackage(readStream))
                        {
                            ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                            int totalRows = workSheet.Dimension.Rows;
                            var contentCol = 2;
                            if (totalRows < 2)
                                return new JsonResult(new Dictionary<string, object> { { "Error", "Không có dữ liệu" } });

                            for (int i = 2; i <= totalRows; i++)
                            {
                                if (workSheet.Cells[i, contentCol].Value == null || workSheet.Cells[i, contentCol].Value.ToString() == "") continue;

                                if (workSheet.Cells[i, 1].Value != null && workSheet.Cells[i, 1].Value.ToString() != "")//Question
                                {
                                    pos++;
                                    var question = new QuestionViewModel
                                    {
                                        Content = workSheet.Cells[i, contentCol].Value.ToString(),//cau hoi
                                        Answers = new List<LessonPartAnswerEntity>() { },//danh sach cau tra loi
                                        Description = workSheet.Cells[i, 5].Value == null ? "" : workSheet.Cells[i, 5].Value.ToString()//giai thich dap an
                                    };
                                    if (workSheet.Cells[i, 3].Value != null)
                                    {
                                        var media = workSheet.Cells[i, 3].Value.ToString().Trim();
                                        if (media != "")
                                            question.Media = new Media
                                            {
                                                OriginalName = media,
                                                Name = media,
                                                Path = media,
                                                Extension = GetContentType(media)
                                            };
                                    }
                                    full_item.Questions.Add(question);
                                }
                                else //answer
                                {
                                    var answer = new LessonPartAnswerEntity
                                    {
                                        Content = workSheet.Cells[i, contentCol].Value.ToString().Trim(),
                                        IsCorrect = workSheet.Cells[i, 4].Value.ToString().ToLower() == "true",
                                    };
                                    if (workSheet.Cells[i, 3].Value != null)
                                    {
                                        var media = workSheet.Cells[i, 3].Value.ToString();
                                        if (media != "")
                                            answer.Media = new Media
                                            {
                                                OriginalName = media,
                                                Name = media,
                                                Path = media,
                                                Extension = GetContentType(media)
                                            };
                                    }
                                    full_item.Questions[pos].Answers.Add(answer);
                                }
                            }
                        }
                    }
                    System.IO.File.Delete(filePath);
                    return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", full_item },
                        {"Error", null }
                    });
                }
                catch (Exception ex)
                {
                    return new JsonResult(new Dictionary<string, object> { { "Error", ex.Message } });
                }
            }
        }
        #endregion


        #region Import with Word
        /// <summary>
        /// Using Spire.Doc v8.9.6
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IActionResult ExportToWord(String basis, String LessonID)
        {
            var lesson = _lessonService.GetItemByID(LessonID);
            if (lesson == null)
                return Json(new Dictionary<String, object> {
                    {"Stt",false },
                    {"Msg","Không tìm thấy bài học." }
                });
            var lessonPart = _lessonPartService.GetByLessonID(LessonID);
            if (lessonPart.Count() == 0)
                return Json(new Dictionary<String, object> {
                    {"Stt",false },
                    {"Msg","Nội dung bài học chưa có." }
                });
            var lessonPartIDs = lessonPart.Select(x => x.ID);
            var lessonPartQuestion = _lessonPartQuestionService.CreateQuery().Find(x => lessonPartIDs.Contains(x.ParentID)).ToEnumerable();
            var lessonPartQuestionIDs = lessonPartQuestion.Select(x => x.ID);
            var lessonPartAnswer = _lessonPartAnswerService.CreateQuery().Find(x => lessonPartQuestionIDs.Contains(x.ParentID)).ToEnumerable();
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            try
            {
                byte[] toArray = null;
                using (var stream = new MemoryStream())
                {
                    Document doc = new Document();
                    Section s = doc.AddSection();
                    s.PageSetup.Orientation = PageOrientation.Landscape;

                    var defStyle = doc.AddParagraphStyle("DefStyle");
                    defStyle.CharacterFormat.FontName = "Calibri";
                    defStyle.CharacterFormat.FontSize = 13;

                    var defStyleCenter = doc.AddParagraphStyle("DefStyleCenter");
                    defStyleCenter.ApplyBaseStyle(defStyle.Name);
                    defStyleCenter.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;

                    for (int x = 0; x < lessonPart.Count(); x++)
                    {

                        var _lessonPart = lessonPart.ElementAtOrDefault(x);

                        RenderWordLessonPart(ref s, _lessonPart);
                    }

                    Paragraph paragraph = s.AddParagraph();
                    TextRange TR4 = paragraph.AppendText(Note);
                    TR4.CharacterFormat.Italic = true;
                    TR4.CharacterFormat.FontSize = 12;
                    TR4.CharacterFormat.TextColor = Color.Red;

                    //Save
                    doc.SaveToStream(stream, FileFormat.Docx);
                    toArray = stream.ToArray();
                };
                string wordName = $"{lesson.Title}.docx";
                return File(toArray, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", wordName);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        private void RenderWordLessonPart(ref Spire.Doc.Section s, LessonPartEntity _lessonPart)
        {

            Table table = s.AddTable(true);

            table.PreferredWidth = new PreferredWidth(WidthType.Percentage, 100);
            table.ResetCells(1, partTypes.Count + 1);

            var titleRow = table.Rows[0];
            var typeRow = table.AddRow();
            var typeRow2 = table.AddRow();
            var descriptionRow = table.AddRow();
            var attachmentRow = table.AddRow();


            #region Title
            var titleRow_Cel1_Content = titleRow.Cells[0].AddParagraph();
            titleRow_Cel1_Content.ApplyStyle("DefStyleCenter");
            titleRow_Cel1_Content.AppendText("Tiêu đề");

            table.ApplyHorizontalMerge(titleRow.GetRowIndex(), 1, partTypes.Count);
            var titleRow_Cel2_Content = titleRow.Cells[1].AddParagraph();
            titleRow_Cel2_Content.ApplyStyle("DefStyle");
            titleRow_Cel2_Content.AppendText(_lessonPart.Title).CharacterFormat.Bold = true;
            #endregion

            #region Type

            var typeRow_cell1 = typeRow.Cells[0];
            typeRow_cell1.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

            var p1 = typeRow_cell1.AddParagraph();
            p1.ApplyStyle("DefStyleCenter");
            p1.AppendText("Kiểu ND");
            var p2 = typeRow_cell1.AddParagraph();
            p2.ApplyStyle("DefStyleCenter");
            p2.AppendText("(x)");

            for (int i = 0; i < partTypes.Count; i++)
            {
                var cell1 = typeRow.Cells[i + 1];
                cell1.CellFormat.HorizontalMerge = CellMerge.None;//prevent previous merge
                cell1.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                var cell_content = cell1.AddParagraph();
                cell_content.ApplyStyle("DefStyleCenter");
                cell_content.AppendText(partDSP[i]).CharacterFormat.FontSize = 12;

                var cell2 = typeRow2.Cells[i + 1];
                cell2.CellFormat.VerticalMerge = CellMerge.None;
                cell2.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                var prg = cell2.AddParagraph();
                prg.ApplyStyle("DefStyleCenter");
                if (_lessonPart.Type == partTypes[i])
                    prg.AppendText("x").CharacterFormat.Bold = true;
                //else
                //    prg.AppendText("").CharacterFormat.Bold = true;
            }
            table.ApplyVerticalMerge(0, typeRow.GetRowIndex(), typeRow2.GetRowIndex());

            #endregion

            #region Description
            var descriptionRow_Cel1 = descriptionRow.Cells[0];
            descriptionRow_Cel1.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
            var descriptionRow_Cel1_Content = descriptionRow_Cel1.AddParagraph();
            descriptionRow_Cel1_Content.ApplyStyle("DefStyleCenter");
            switch (_lessonPart.Type)
            {
                //case "QUIZ1":
                //case "QUIZ2":
                //case "QUIZ3":
                //case "QUIZ4":
                //case "ESSAY":
                //    descriptionRow_Cel1_Content.AppendText("Nội dung");
                //    break;
                //case "VOCAB":
                //    descriptionRow_Cel1_Content.AppendText("Danh sách từ");
                //    break;
                default:
                    descriptionRow_Cel1_Content.AppendText("Nội dung");
                    break;
            }

            table.ApplyHorizontalMerge(descriptionRow.GetRowIndex(), 1, partTypes.Count);
            var descriptionRow_Cel2 = descriptionRow.Cells[1];
            var descriptionRow_Cel2_Content = descriptionRow_Cel2.AddParagraph();
            descriptionRow_Cel2_Content.ApplyStyle("DefStyle");


            //var newdoc = ConvertHtmlToWordDoc(_lessonPart.Description, basis, userid, _lessonPart.Type);
            var html = _lessonPart.Description;
            if (html != null)
                html = html.Replace("src=\"/", "src=\"https://" + currentHost + "/");
            descriptionRow_Cel2_Content.ApplyStyle("DefStyle");
            var answer = "";
            switch (_lessonPart.Type)
            {
                case "QUIZ2":
                    if (!string.IsNullOrEmpty(_lessonPart.Description))
                    {
                        html = "<p style='margin:0pt'><b style='color:red'>[Câu hỏi]</b></p><p style='margin:0pt'>" + RenderQuiz2ForWord(_lessonPart, out answer) + "</p>";
                        answer = "<p style='margin:0pt;margin-top:2pt'><b style='color:red'>[Đáp án]</b></p>" + answer;
                        descriptionRow_Cel2_Content.AppendHTML(html + answer);
                        //var aGrp2 = descriptionRow_Cel2.AddParagraph();
                        //aGrp2.ApplyStyle("DefStyle");
                        //descriptionRow_Cel2_Content.AppendHTML(answer);
                        //aGrp2.ApplyStyle("DefStyle");
                        //foreach (var childprg in aGrp2.ChildObjects)
                        //{
                        //    if (childprg is Paragraph)
                        //    {
                        //        (childprg as Paragraph).ApplyStyle("DefStyle");
                        //    }
                        //}
                    }
                    break;
                case "QUIZ1":
                case "QUIZ3":
                case "QUIZ4":
                    if (!string.IsNullOrEmpty(_lessonPart.Description))
                        descriptionRow_Cel2_Content.AppendHTML("<p style='margin:0pt'>" + html + "</p>");
                    var question = "<p style='margin:0pt'><b style='color:red'>[Câu hỏi]</b></p><p style='margin:0pt'>" + RenderTQuizForWord(_lessonPart, out answer) + "</p>";
                    answer = "<p style='margin:0pt;margin-top:2pt'><b style='color:red'>[Đáp án]</b></p>" + answer;
                    var qGrp = descriptionRow_Cel2.AddParagraph();
                    qGrp.ApplyStyle("DefStyle");
                    qGrp.AppendHTML(question);
                    var aGrp = descriptionRow_Cel2.AddParagraph();
                    aGrp.ApplyStyle("DefStyle");
                    aGrp.AppendHTML(answer);
                    break;
                case "ESSAY":
                    //html = $"<p>[Des]</p>{html}";
                    var lessonPartQuestion = _lessonPartQuestionService.GetByPartID(_lessonPart.ID).FirstOrDefault();
                    if (lessonPartQuestion != null)
                    {
                        html += $"<p style='margin:0pt'>[E]</p>{lessonPartQuestion.Description}";
                        html += $"<p style='margin:0pt;margin-top:2pt'>[P] {lessonPartQuestion.Point}</p>";
                    }
                    descriptionRow_Cel2_Content.AppendHTML(html);
                    break;
                default:
                    if (!string.IsNullOrEmpty(_lessonPart.Description))
                        descriptionRow_Cel2_Content.AppendHTML(html);
                    break;
            }
            #endregion

            #region Attachment
            var attachmentRow_Cel1 = attachmentRow.Cells[0];
            attachmentRow_Cel1.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
            var attachmentRow_Cel1_Content = attachmentRow_Cel1.AddParagraph();
            attachmentRow_Cel1_Content.ApplyStyle("DefStyleCenter");
            attachmentRow_Cel1_Content.AppendText("File");

            table.ApplyHorizontalMerge(attachmentRow.GetRowIndex(), 1, partTypes.Count);
            var attachmentRow_Cel2_Content = attachmentRow.Cells[1].AddParagraph();
            attachmentRow_Cel2_Content.ApplyStyle("DefStyle");

            if (_lessonPart.Media != null && !string.IsNullOrEmpty(_lessonPart.Media.Path))
            {

                var ext = _lessonPart.Media.Extension;

                if (_lessonPart.Media.Path.ToLower().StartsWith("http"))//external
                {
                    attachmentRow_Cel2_Content.AppendHyperlink(_lessonPart.Media.Path, _lessonPart.Media.OriginalName, HyperlinkType.WebLink);
                }
                else
                {
                    var path = _lessonPart.Media.Path.Replace("/Files/", "Files\\").Replace("/", "\\");
                    var objPath = Path.Combine(StaticPath, path);

                    var maxWidth = 360; var maxHeight = 240;


                    if (ext.ToLower().Contains("image"))
                    {
                        var img = attachmentRow_Cel2_Content.AppendPicture(FileProcess.ImageToByteArray(Image.FromFile(objPath)));
                        var scale = 100;
                        if (img.Width > 0 && img.Height > 0)
                        {
                            if (img.Width / img.Height > maxWidth / maxHeight)
                            {
                                if (img.Width > maxWidth)
                                    scale = (int)(maxWidth * 100.0 / img.Width);
                            }
                            else
                                if (img.Height > maxHeight)
                                scale = (int)(maxHeight * 100.0 / img.Height);
                        }

                        img.WidthScale = scale;
                        img.HeightScale = scale;
                    }
                    else
                    {
                        Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(objPath);
                        var byteIcon = IconToBytes(icon);
                        Document document = new Document();
                        Section section = document.AddSection();
                        Paragraph p = section.AddParagraph();
                        //DocPicture Pic = p.AppendPicture(byteIcon);
                        DocPicture Pic = p.AppendPicture(FileProcess.ImageToByteArray(Image.FromFile($"{StaticPath}\\images\\default-Icon-File.png.jpg")));
                        Pic.Width = 100;
                        Pic.Height = 50;
                        if (ext.Contains("audio"))
                        {
                            var ole = attachmentRow_Cel2_Content.AppendOleObject(objPath, Pic);
                            ole.Width = Pic.Width;
                            ole.Height = Pic.Height;
                        }
                        else if (ext.Contains("video"))
                        {
                            var ole = attachmentRow_Cel2_Content.AppendOleObject(objPath, Pic, OleObjectType.VideoClip);
                            ole.Width = maxWidth;
                            ole.Height = maxHeight;
                        }
                        else
                        {
                            var ole = attachmentRow_Cel2_Content.AppendOleObject(objPath, Pic);
                            ole.Width = maxWidth;
                            ole.Height = maxHeight;
                        }
                    }
                }
            }

            #endregion

            //break part
            s.AddParagraph();
        }

        private static byte[] IconToBytes(Icon icon)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                icon.Save(ms);
                return ms.ToArray();
            }
        }

        private string RenderTQuizForWord(LessonPartEntity lessonPart, out string answer)
        {
            answer = "";
            var returnHtml = "";

            var questions = _lessonPartQuestionService.GetByPartID(lessonPart.ID).ToList();
            if (questions == null || questions.Count() == 0)
                return returnHtml;
            var i = 0;
            foreach (var quiz in questions)
            {
                i++;
                returnHtml += "<p style='margin:0pt'>[Q" + i + "] " + RenderHtmlContent(quiz.Content, quiz.Media) + "</p>";
                var ans = _lessonPartAnswerService.GetByQuestionID(quiz.ID);

                answer += "<p style='margin:0pt'>";

                if (ans != null && ans.Count() > 0)
                {
                    answer += "[A" + i + "] ";
                    answer += string.Join(" | ", ans.Select(t => RenderHtmlContent(t.Content, t.Media) + (t.IsCorrect ? " (x)" : "")));
                }
                answer += "</p>";
                if (!string.IsNullOrEmpty(quiz.Description))
                    answer += ("<p style='margin:0pt'>[E" + i + "] " + quiz.Description + "</p>");

            }

            returnHtml = returnHtml.Replace("src=\"/", "src=\"https://" + currentHost + "/");
            return returnHtml;
        }

        private string RenderQuiz2ForWord(LessonPartEntity lessonPart, out string answer)
        {
            answer = "";
            var description = lessonPart.Description.Replace("src=\"/", "src=\"https://" + currentHost + "/");
            var returnHtml = "";
            var quizOpen = "<fillquiz>";
            var quizClose = "</fillquiz>";

            var questions = _lessonPartQuestionService.GetByPartID(lessonPart.ID).ToList();
            if (questions == null || questions.Count() == 0)
                return description;

            var firstOccurIndex = description.IndexOf(quizOpen);
            var i = 0;
            while (firstOccurIndex >= 0 && questions.Count() > 0)
            {
                i++;
                var quiz = questions.First();
                returnHtml += description.Substring(0, firstOccurIndex) + "_Q" + i + "";
                //if (!string.IsNullOrEmpty(quiz.Content))
                //    returnHtml += (":" + quiz.Content);
                returnHtml += "_";
                var ans = _lessonPartAnswerService.GetByQuestionID(quiz.ID);

                if (ans != null && ans.Count() > 0)
                {
                    answer += "<p style='margin:0pt'>[A" + i + "] ";
                    answer += string.Join(" | ", ans.Select(t => t.Content));
                    answer += "</p>";
                }
                if (!string.IsNullOrEmpty(quiz.Content))
                {
                    answer += ($"<p style='margin:0pt'>[H{i}]" + quiz.Content + "</p>");
                }
                if (!string.IsNullOrEmpty(quiz.Description))
                {
                    answer += ("<p style='margin:0pt'>[E" + i + "]" + quiz.Description + "</p>");
                }

                description = description.Substring(description.IndexOf(quizClose) + quizClose.Length);
                firstOccurIndex = description.IndexOf(quizOpen);
                questions.RemoveAt(0);
            }

            return returnHtml;
        }

        private string RenderHtmlContent(string text, Media img)
        {
            string returnHTML = "";
            if (!string.IsNullOrEmpty(text))
                returnHTML += text;
            if (img != null)
            {
                if (img.Path.ToLower().StartsWith("http"))
                    returnHTML += "<img src='" + img.Path + "'/>";
                else
                    returnHTML += "<img src='https://static.eduso.vn/" + img.Path + "'/>";
            }
            return returnHTML;
        }

        public IActionResult ExportWordTemplate()
        {
            try
            {
                byte[] toArray = null;
                using (var stream = new MemoryStream())
                {
                    Document doc = new Document();
                    Section s = doc.AddSection();
                    s.PageSetup.Orientation = PageOrientation.Landscape;

                    var defStyle = doc.AddParagraphStyle("DefStyle");
                    defStyle.CharacterFormat.FontName = "Calibri";
                    defStyle.CharacterFormat.FontSize = 13;

                    var defStyleCenter = doc.AddParagraphStyle("DefStyleCenter");
                    defStyleCenter.ApplyBaseStyle(defStyle.Name);
                    defStyleCenter.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;

                    //Add Cells
                    for (Int32 indexTable = 0; indexTable < partTypes.Count; indexTable++)
                    {
                        Table table = s.AddTable(true);
                        table.ResetCells(5, 12);

                        //Title Row
                        TableRow titleRow = table.Rows[0];
                        Paragraph p_titleRow_cell0 = titleRow.Cells[0].AddParagraph();
                        titleRow.Cells[0].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        p_titleRow_cell0.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        p_titleRow_cell0.AppendText("Tiêu đề");

                        Paragraph p_titleRow_cell1 = titleRow.Cells[1].AddParagraph();
                        titleRow.Cells[1].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        p_titleRow_cell1.Format.HorizontalAlignment = HorizontalAlignment.Left;
                        p_titleRow_cell1.AppendText("Nhập tiêu đề tại đây.");
                        table.ApplyHorizontalMerge(0, 1, partDSP.Count);

                        //Type Row
                        TableRow typeRow = table.Rows[1];
                        Paragraph p_typeRow_cell0 = typeRow.Cells[0].AddParagraph();
                        typeRow.Cells[0].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        p_typeRow_cell0.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        p_typeRow_cell0.AppendText("Kiểu ND \n(x)");
                        table.ApplyVerticalMerge(0, 1, 2);
                        for (int i = 1; i <= partDSP.Count; i++)
                        {
                            var item = partDSP[i - 1];
                            Paragraph p_typeRow_cell = typeRow.Cells[i].AddParagraph();
                            typeRow.Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            p_typeRow_cell.Format.HorizontalAlignment = HorizontalAlignment.Center;
                            p_typeRow_cell.AppendText(item);
                        }

                        TableRow checkType = table.Rows[2];
                        Paragraph p_typeRow_cell1 = checkType.Cells[indexTable + 1].AddParagraph();
                        checkType.Cells[indexTable + 1].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        p_typeRow_cell1.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        p_typeRow_cell1.AppendText("x").CharacterFormat.Bold = true;


                        //Content Row
                        TableRow contentRow = table.Rows[3];
                        Paragraph p_contentRow_cell0 = contentRow.Cells[0].AddParagraph();
                        contentRow.Cells[0].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        p_contentRow_cell0.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        //if (indexTable >= 6 && indexTable <= partTypes.Count)
                        //    p_contentRow_cell0.AppendText("Nội dung");
                        //else if(indexTable==5)
                        //    p_contentRow_cell0.AppendText("Danh sách từ");
                        //else
                        //    p_contentRow_cell0.AppendText("Mô tả");

                        Paragraph p_contentRow_cell1 = contentRow.Cells[1].AddParagraph();
                        contentRow.Cells[0].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        p_contentRow_cell1.Format.HorizontalAlignment = HorizontalAlignment.Left;

                        if (indexTable >= 6 && indexTable <= partTypes.Count)
                        {
                            p_contentRow_cell0.AppendText("Nội dung");
                            p_contentRow_cell1.AppendText("Soạn thảo nội dung tại đây");
                            p_contentRow_cell1.AppendBreak(BreakType.LineBreak);
                            if (indexTable == 6 || indexTable == 8 || indexTable == 9)
                            {
                                p_contentRow_cell1.AppendText("[Câu hỏi]").CharacterFormat.Bold = true;
                                p_contentRow_cell1.AppendText("[Q1] Nội dung câu hỏi");
                                p_contentRow_cell1.AppendBreak(BreakType.LineBreak);
                                p_contentRow_cell1.AppendText("[Đáp án]").CharacterFormat.Bold = true;
                                p_contentRow_cell1.AppendBreak(BreakType.LineBreak);
                                p_contentRow_cell1.AppendText("[A1] A. Đáp án 1 | B. Đáp án 2(x) | C. Đáp án 3");
                                p_contentRow_cell1.AppendBreak(BreakType.LineBreak);
                                p_contentRow_cell1.AppendText("[E1] Giải thích đáp án");
                            }
                            else if (indexTable == 7)
                            {
                                p_contentRow_cell1.AppendText("[Câu hỏi]").CharacterFormat.Bold = true;
                                p_contentRow_cell1.AppendBreak(BreakType.LineBreak);
                                p_contentRow_cell1.AppendText("Nội dung phần điền từ _Q1_");
                                p_contentRow_cell1.AppendBreak(BreakType.LineBreak);
                                p_contentRow_cell1.AppendText("[H1] Hiển thị phần học viên");
                                p_contentRow_cell1.AppendBreak(BreakType.LineBreak);
                                p_contentRow_cell1.AppendText("[E1] Giải thích đáp án");
                            }
                            else
                            {
                                p_contentRow_cell1.RemoveFrame();
                                p_contentRow_cell1.AppendText("Soạn thảo nội dung tại đây");
                                p_contentRow_cell1.AppendBreak(BreakType.LineBreak);
                                p_contentRow_cell1.AppendText("[E] Giải thích đáp án");
                                p_contentRow_cell1.AppendBreak(BreakType.LineBreak);
                                p_contentRow_cell1.AppendText("[P] Điểm");
                            }
                        }
                        else if (indexTable == 5)
                        {
                            p_contentRow_cell0.AppendText("Danh sách từ");
                            p_contentRow_cell1.AppendText("Các từ cách nhau bởi dấu \"|\" .");
                        }
                        else
                        {
                            p_contentRow_cell0.AppendText("Nội dung");
                            p_contentRow_cell1.AppendText("Soạn thảo nội dung tại đây");
                        }

                        table.ApplyHorizontalMerge(3, 1, partDSP.Count);

                        //File Row
                        TableRow fileRow = table.Rows[4];
                        Paragraph p_fileRow_cell0 = fileRow.Cells[0].AddParagraph();
                        fileRow.Cells[0].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        p_fileRow_cell0.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        p_fileRow_cell0.AppendText("File");

                        Paragraph p_fileRow_cell1 = fileRow.Cells[1].AddParagraph();
                        fileRow.Cells[0].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        p_fileRow_cell1.Format.HorizontalAlignment = HorizontalAlignment.Left;
                        p_fileRow_cell1.AppendText("Thêm file hoặc liên kết tại đây");
                        table.ApplyHorizontalMerge(4, 1, partDSP.Count);

                        s.AddParagraph().AppendBreak(BreakType.LineBreak);
                    }

                    //Create a new paragraph
                    //Lưu ý
                    Paragraph paragraph = s.AddParagraph();
                    TextRange TR4 = paragraph.AppendText(Note);
                    TR4.CharacterFormat.Italic = true;
                    TR4.CharacterFormat.FontSize = 12;
                    TR4.CharacterFormat.TextColor = Color.Red;

                    //Save
                    doc.SaveToStream(stream, FileFormat.Docx);
                    toArray = stream.ToArray();
                };
                string wordName = $"QuizTemplateWithWord.docx";
                return File(toArray, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", wordName);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public async Task<JsonResult> ImportFromWord(string basis = "", string ParentID = "")
        {
            Boolean Status = false;
            try
            {
                var form = HttpContext.Request.Form;
                if (form == null || form.Files == null || form.Files.Count <= 0)
                    return new JsonResult(new Dictionary<string, object> { { "Error", "Chưa chọn file" } });


                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var file = form.Files[0];
                var dirPath = "Upload\\Quiz";

                if (!Directory.Exists(Path.Combine(_env.WebRootPath, dirPath)))
                    Directory.CreateDirectory(Path.Combine(_env.WebRootPath, dirPath));
                var filePath = Path.Combine(_env.WebRootPath, dirPath + "\\" + DateTime.Now.ToString("ddMMyyyyhhmmss") + file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    String Msg = "";
                    await file.CopyToAsync(stream);
                    stream.Close();
                    using (var readStream = new FileStream(filePath, FileMode.Open))
                    {
                        //Create a Document instance and load the word document
                        Document document = new Document(readStream);

                        //Get the first session
                        var sw = document.Sections[0];
                        Int32 countTable = sw.Body.Tables.Count;

                        for (int indexTable = 0; indexTable < sw.Body.Tables.Count; indexTable++)
                        {
                            //Get the first table in the textbox
                            Table table = sw.Body.Tables[indexTable] as Table;

                            //var parentLesson = _lessonService.GetItemByID(ParentID);
                            //var isPractice = parentLesson.IsPractice;

                            LessonPartViewModel item = new LessonPartViewModel();

                            String title = table.Rows[0].Cells[1].Paragraphs[0].Text?.ToString().Trim();

                            item.Title = title;
                            item.Created = DateTime.UtcNow;
                            item.Updated = DateTime.UtcNow;
                            //item.Description = description;
                            item.Point = 0;//đếm câu hỏi
                            item.ParentID = ParentID;

                            var maxItem = _lessonPartService.CreateQuery()
                            .Find(o => o.ParentID == item.ParentID)
                            .SortByDescending(o => o.Order).FirstOrDefault();
                            item.Order = maxItem != null ? maxItem.Order + 1 : 0;

                            //check type
                            var typeRow = table.Rows[2];

                            for (int indexCell = 1; indexCell < typeRow.Cells.Count; indexCell++)
                            {
                                var txtCell = typeRow.Cells[indexCell].Paragraphs[0].Text.ToString().ToUpper();
                                if (txtCell.Contains("X"))
                                {
                                    //type = table.Rows[1].Cells[indexCell].Paragraphs[0].Text.ToString().ToUpper();
                                    item.Type = partTypes[indexCell - 1];
                                    break;
                                }
                            }

                            switch (item.Type)
                            {
                                case "TEXT":
                                case "AUDIO":
                                case "VIDEO":
                                case "DOC":
                                    Msg += (await GetContentOther(table, item.Type, basis, item, UserID));
                                    Status = true;
                                    break;
                                case "IMAGE":
                                    Msg += (await GetContentIMG(table, item.Type, basis, item, UserID));
                                    Status = true;
                                    break;
                                case "VOCAB":
                                    Msg += (await GetContentVocab(table, item.Type, basis, item, UserID));
                                    Status = true;
                                    break;
                                case "QUIZ1":
                                case "QUIZ3":
                                case "QUIZ4":
                                    Msg += (await GetContentQUIZ(table, item.Type, basis, item, UserID));
                                    Status = true;
                                    break;
                                case "QUIZ2":
                                    item.Description = "";
                                    Msg += (await GetContentQuiz2(table, item.Type, basis, item, UserID));
                                    Status = true;
                                    break;
                                case "ESSAY":
                                    item.Type = "ESSAY";
                                    Msg += (await GetContentEssay(table, item.Type, basis, item, UserID));
                                    Status = true;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    _lessonHelper.calculateLessonPoint(ParentID);

                    System.IO.File.Delete(filePath);
                    return new JsonResult(new Dictionary<string, object>
                    {
                        //{ "Data", full_item },
                        {"Msg", Msg },
                        {"Stt",Status == string.IsNullOrEmpty(Msg) }
                    });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new Dictionary<string, object>
                    {
                        //{ "Data", full_item },
                        {"Msg", ex.Message },
                        {"Stt",false }
                    });
            }
        }

        private async Task<String> GetContentQUIZ(Table table, String type, String basis, LessonPartViewModel item, String createUser = null)
        {
            try
            {
                //List<QuestionViewModel> Quiz = new List<QuestionViewModel>();
                Dictionary<String, QuestionViewModel> Quiz = new Dictionary<string, QuestionViewModel>();
                Dictionary<String, List<AnswerViewModel>> listAns = new Dictionary<string, List<AnswerViewModel>>();
                Int32 pos = -1;
                Int32 indexQuiz = 1;
                Int32 indexAns = 1;
                Int32 indexEx = 1;

                var paragraphs = table.Rows[3].Cells[1].Paragraphs;
                var objs = table.Rows[3].Cells[1].ChildObjects;
                foreach (Paragraph para in paragraphs)
                {
                    var content = para.Text.Trim();
                    if (content.Contains($"[Q{indexQuiz}]"))//cau hoi
                    {
                        var question = new QuestionViewModel
                        {
                            ParentID = item.ID,
                            CourseID = item.CourseID,
                            Order = item.Order,
                            Created = DateTime.UtcNow,
                            Updated = DateTime.UtcNow,
                            CreateUser = createUser,
                            Point = 1,
                            Answers = new List<LessonPartAnswerEntity>() { },//danh sach cau tra loi,
                        };
                        foreach (DocumentObject obj in para.ChildObjects)
                        {
                            if (obj is TextRange)
                            {
                                question.Content += (obj as TextRange).Text.Replace($"[Q{indexQuiz}]", "");
                            }
                            else if (obj is DocPicture)
                            {
                                var pic = obj as DocPicture;
                                var fileName = $"{DateTime.Now.ToString("yyyyMMddhhmmssfff")}_{indexQuiz}.jpg";
                                var path = SaveImageByByteArray(pic.ImageBytes, fileName, createUser, basis);

                                question.Media = new Media()
                                {
                                    Created = DateTime.UtcNow,
                                    Path = path,
                                    OriginalName = fileName,
                                    Name = fileName,
                                    Extension = "image/jpg"
                                };
                            }
                        }
                        Quiz.Add($"[Q{indexQuiz}]", question);
                        pos++;
                        indexQuiz++;
                    }
                    else if (content.Contains($"[A{indexAns}]"))
                    {
                        var lstAns = para.Text.Replace($"[Đáp án]\v[A{indexAns}]", "").Replace($"[A{indexAns}]", "").Trim().Split('|');
                        foreach (var _ans in lstAns)
                        {
                            var ans = new LessonPartAnswerEntity
                            {
                                CourseID = item.CourseID,
                                CreateUser = createUser,
                                Created = DateTime.UtcNow,
                                Updated = DateTime.UtcNow,
                                //Media = null,
                                Content = _ans.Replace("(x)", "").Replace("(X)", "").Trim(),
                                IsCorrect = _ans.Trim().ToUpper().Contains("X") ? true : false,
                            };
                            Quiz[$"[Q{indexAns}]"].Answers.Add(ans);
                        }
                        foreach (DocumentObject obj in para.ChildObjects)
                        {
                            if (obj is DocPicture)
                            {
                                var pic = obj as DocPicture;
                                var previousSibling = (obj.PreviousSibling as TextRange).Text.Replace($"[A{indexAns}]", "").Replace($"[E{indexAns}]", "").Replace("|", "").Replace("(X)", "").Replace("(x)", "").Trim();
                                var fileName = $"{DateTime.Now.ToString("yyyyMMddhhmmssfff")}_ans{previousSibling}_{indexQuiz}_{indexAns}.jpg";
                                var path = SaveImageByByteArray(pic.ImageBytes, fileName, createUser, basis);
                                var _test = lstAns.Contains(previousSibling);
                                var ansWmedia = Quiz[$"[Q{indexAns}]"].Answers.Find(x => x.Content.Contains(previousSibling));
                                ansWmedia.Media = new Media
                                {
                                    Name = fileName,
                                    OriginalName = fileName,
                                    Path = path,
                                    Extension = "image/jpg",
                                    //Extension = System.IO.Path.GetExtension(path),
                                    Created = DateTime.Now,
                                    Size = pic.Size.Width * pic.Size.Height
                                };
                            }
                        }
                        indexAns++;
                    }
                    else if (content.Contains($"[E{indexEx}]"))
                    {
                        var str = content.Replace($"[E{indexEx}]", "").Trim();
                        if (Quiz.ContainsKey($"[Q{indexEx}]"))
                        {
                            if (!String.IsNullOrEmpty(str))
                            {
                                Quiz[$"[Q{indexEx}]"].Description += $"<p>{str}</p>";
                            }
                            while ((para.NextSibling as Paragraph) != null && !(para.NextSibling as Paragraph).Text.Contains($"[A{indexEx + 1}]"))
                            {
                                Quiz[$"[Q{indexEx}]"].Description += $"<p>{(para.NextSibling as Paragraph)?.Text}</p>";
                                paragraphs.Remove(para.NextSibling as Paragraph);
                            }
                            indexEx++;
                        }
                    }
                }

                //desription
                //foreach (Paragraph _para in paragraphs)
                //{
                //    //var _para = paragraphs[0];
                //    while (_para != null && (_para.Text.Trim().ToUpper().Contains("[CÂU HỎI]") || _para.Text.Trim().ToUpper().Contains("[ĐÁP ÁN]")))
                //    {
                //        if (_para.NextSibling != null)
                //        {
                //            paragraphs.Remove((_para.NextSibling as Paragraph));
                //        }
                //        else
                //        {
                //            paragraphs.Remove(_para);
                //        }
                //    }
                //}

                Paragraph nextPrg = paragraphs[0];
                //foreach (Paragraph para in paragraphs)
                while (nextPrg != null)
                {
                    Paragraph para = nextPrg;
                    var content = para.Text.Trim();
                    if (content.ToUpper().Contains("[CÂU HỎI]") || content.ToUpper().Contains("[ĐÁP ÁN]"))
                    {
                        var nextObj = para.NextSibling;
                        while (nextObj != null)
                        {
                            var next = nextObj.NextSibling;
                            objs.Remove(nextObj);
                            nextObj = next;
                        }
                        paragraphs.Remove(para);
                        break;
                    }
                    else
                    {
                        nextPrg = nextPrg.NextSibling as Paragraph;
                    }
                }

                item.Description = await ConvertDocToHtml(table, basis, createUser);

                //file 
                var linkcell = table.Rows[4].Cells[1];

                var linkfile = "";
                linkfile = await GetContentFile(basis, item, createUser, linkcell, linkfile);

                List<QuestionViewModel> lpq = new List<QuestionViewModel>();
                for (int i = 0; i < Quiz.Count; i++)
                {
                    var quiz = Quiz.ContainsKey($"[Q{i + 1}]") ? Quiz[$"[Q{i + 1}]"] : null;
                    if (quiz != null)
                    {
                        quiz.Content = quiz.Content.Contains($"[Q{i + 1}]") ? quiz.Content.Replace($"[Q{i + 1}]", "") : quiz.Content;
                        lpq.Add(quiz);
                    }
                }

                item.Questions = lpq;
                await CreateOrUpdateLessonPart(basis, item);

                return "";
            }
            catch (Exception ex)
            {
                return "Type QUIZ 134 has error: " + ex.Message;
            }
        }

        private async Task<String> GetContentIMG(Table table, String type, String basis, LessonPartViewModel item, String createUser = null)
        {
            try
            {
                var totalRows = table.Rows.Count;
                for (int indexRow = 0; indexRow < totalRows; indexRow++)
                {
                    var contentRow = table.Rows[indexRow];
                    String contentCell0 = contentRow.Cells[0].Paragraphs[0].Text?.ToString().Trim().ToLower();

                    if (contentCell0.Equals("file"))
                    {
                        item.Media = new Media();
                        var contentFileImage = GetContentFile(contentRow.Cells[1].Paragraphs, basis, createUser);
                        if (contentFileImage.Count > 0)
                        {
                            item.Media.Created = DateTime.Now;
                            item.Media.Name = contentFileImage["FileName"];
                            item.Media.Path = contentFileImage["FilePath"];
                            item.Media.Extension = "image/png";
                        }
                        else
                        {
                            item.Media = new Media();
                        }
                        break;
                    }
                    else continue;
                }
                _lessonPartService.CreateOrUpdate(item);
                return "";
            }
            catch (Exception ex)
            {
                return "Type IMG has error: " + ex.Message;
            }
        }

        private async Task<String> GetContentVocab(Table table, String type, String basis, LessonPartViewModel item, String createUser = null)
        {
            try
            {
                var descriptionCell = table.Rows[3].Cells[1];
                var desc = descriptionCell.FirstParagraph.Text;

                var vocabArr = desc.Split('|');
                if (vocabArr != null && vocabArr.Length > 0)
                {
                    foreach (var vocab in vocabArr)
                    {
                        var vocabulary = vocab.Trim().ToLower();
                        _ = GetVocabByCambridge(vocabulary);
                    }
                }
                item.Description = desc;

                await CreateOrUpdateLessonPart(basis, item);
                return $"";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private async Task<String> GetContentQuiz2(Table table, String type, String basis, LessonPartViewModel item, String createUser = null)
        {
            try
            {
                Dictionary<String, String> _listAns = new Dictionary<String, String>();
                Dictionary<String, String> _listH = new Dictionary<String, String>();
                Dictionary<String, String> _listEx = new Dictionary<String, String>();
                var descCell = table.Rows[3].Cells[1];
                var paragraphs = descCell.Paragraphs;
                Int32 indexquiz = 1, indexans = 1, indexH = 1, indexEx = 1;
                Paragraph nextPrg = paragraphs[0];
                //foreach (Paragraph para in paragraphs)
                while (nextPrg != null)
                {
                    Paragraph para = nextPrg;
                    nextPrg = para.NextSibling as Paragraph;
                    var content = para.Text.Trim();
                    String ex = "";
                    String h = "";
                    if (para.Text.ToUpper().Contains("[CÂU HỎI]")) descCell.Paragraphs.Remove(para);
                    else if (para.Text.Trim().Contains($"[A{indexans}]"))
                    {
                        indexH = indexans;
                        indexEx = indexans;
                        String str = content.Replace($"[A{indexans}]", "").Trim();
                        _listAns.Add($"[A{indexans}]", str);
                        indexans++;
                        descCell.Paragraphs.Remove(para);
                    }
                    else if (para.Text.Trim().Contains($"[H{indexH}]"))
                    {
                        indexEx = indexH;
                        var str = content.Replace($"[H{indexH}]", "").Trim();
                        h += str;
                        while (para.NextSibling != null && (
                            !(para.NextSibling as Paragraph).Text.Contains($"[A{indexH + 1}]")
                            && !(para.NextSibling as Paragraph).Text.Contains($"[E{indexH + 1}]")
                            && !(para.NextSibling as Paragraph).Text.Contains($"[E{indexH}]")))
                        {
                            h += (para.NextSibling as Paragraph)?.Text + " ";
                            descCell.Paragraphs.Remove(para.NextSibling as Paragraph);
                        }
                        _listH.Add($"H{indexH}", h);
                        indexH++;
                        nextPrg = para.NextSibling as Paragraph;
                        descCell.Paragraphs.Remove(para);
                    }
                    else if (para.Text.Trim().Contains($"[E{indexEx}]"))
                    {
                        var str = content.Replace($"[E{indexEx}]", "").Trim();
                        ex += str;
                        while (para.NextSibling != null && (!(para.NextSibling as Paragraph).Text.Contains($"[A{indexEx + 1}]") && !(para.NextSibling as Paragraph).Text.Contains($"[H{indexEx + 1}]")))
                        {
                            ex += (para.NextSibling as Paragraph)?.Text + " ";
                            descCell.Paragraphs.Remove(para.NextSibling as Paragraph);
                        }
                        _listEx.Add($"E{indexEx}", ex);
                        indexEx++;
                        nextPrg = para.NextSibling as Paragraph;
                        descCell.Paragraphs.Remove(para);
                    }
                    else if (para.Text.ToUpper().Contains("[ĐÁP ÁN]")) descCell.Paragraphs.Remove(para);

                }

                var indexQ = 1;
                for (Int32 i = 0; i < paragraphs.Count; i++)
                {
                    var paraText = descCell.Paragraphs[i].Text.Trim();
                    while (paraText.Contains($"_Q{indexQ}_"))
                    {
                        descCell.Paragraphs[i].Replace($"_Q{indexQ}_", $"EDUSOQUIZ2_Q{indexQ}_", true, false);
                        indexQ++;
                    }
                }

                String description = await ConvertDocToHtml(table, basis, createUser);
                foreach (var a in _listAns)
                {
                    var ex = _listEx.ContainsKey($"E{indexquiz}") ? _listEx[$"E{indexquiz}"] : "";
                    var h = _listH.ContainsKey($"H{indexquiz}") ? _listH[$"H{indexquiz}"] : "";
                    var str = a.Value.Replace($"[A{indexquiz}]", "").Trim();
                    String replace3 = $"EDUSOQUIZ2_Q{indexquiz}_";
                    String replace2 = $"<fillquiz contenteditable=\"false\" readonly=\"readonly\" title=\"{ex}\"><input ans=\"{str}\" class=\"fillquiz\" contenteditable=\"false\" dsp=\"{h}\" placeholder=\"{ex}\" readonly=\"readonly\" type=\"text\" value=\"{str}\"/></fillquiz>";
                    description = description.ToString().Replace(replace3, replace2);
                    indexquiz++;
                }
                item.Description = description;

                var newdescription = "";
                item.Questions = ExtractFillQuestionList(item, createUser, out newdescription);
                item.Description = newdescription.ToString();

                //file 
                var linkcell = table.Rows[4].Cells[1];

                var linkfile = "";
                linkfile = await GetContentFile(basis, item, createUser, linkcell, linkfile);

                await CreateOrUpdateLessonPart(basis, item);

                return $"";
            }
            catch (Exception ex)
            {
                return $"{type} has error {ex.Message}";
            }
        }

        private async Task GetVocabByCambridge(string vocab)
        {
            //check if vocab is exist
            var code = vocab.ToLower().Replace(" ", "-");
            var olditems = _vocabularyService.GetItemByCode(code);
            if (olditems != null && olditems.Count > 0)
                return;

            var listVocab = new List<VocabularyEntity>();
            var pronUrl = "https://dictionary.cambridge.org/vi/dictionary/english/" + code;
            var dictUrl = "https://dictionary.cambridge.org/vi/dictionary/english-vietnamese/" + code;
            var listExp = new List<PronunExplain>();

            using (var expclient = new WebClient())
            {
                var expDoc = new HtmlDocument();
                string expHtml = expclient.DownloadString(dictUrl);
                expDoc.LoadHtml(expHtml);
                var expContents = expDoc.DocumentNode.SelectNodes("//div[contains(@class,\"english-vietnamese kdic\")]");
                if (expContents != null && expContents.Count() > 0)
                {
                    foreach (var expContent in expContents)
                    {
                        var typeNode = expContent.SelectSingleNode(".//span[contains(@class,\"pos dpos\")]");
                        if (typeNode == null) continue;
                        var type = typeNode.InnerText;
                        if (listExp.Any(t => t.WordType == type))
                            continue;
                        var expNodes = expContent.SelectNodes(".//span[contains(@class,\"trans dtrans\")]");
                        if (expNodes == null)
                            return;
                        if (expNodes != null && expNodes.Count() > 0)
                        {
                            foreach (var node in expNodes)
                            {
                                listExp.Add(new PronunExplain
                                {
                                    WordType = type,
                                    Meaning = node.InnerText
                                });
                            }
                        }

                    }
                }
            }
            //if (listExp == null || listExp.Count == 0)
            //    return;

            using (var client = new WebClient())
            {
                HtmlDocument doc = new HtmlDocument();
                string html = client.DownloadString(pronUrl);
                doc.LoadHtml(html);

                var contentHolder = doc.DocumentNode.SelectNodes("//div[contains(@class, \"pos-header\")]");
                if (contentHolder != null && contentHolder.Count() > 0)
                {
                    foreach (var content in contentHolder)
                    {
                        var typeNode = content.SelectSingleNode(".//span[contains(@class,\"pos dpos\")]");
                        var typeText = typeNode.InnerText;

                        if (listVocab.Any(t => t.WordType == typeText))
                            continue;
                        try
                        {

                            var pronun = content.SelectSingleNode(".//span[contains(@class,\"us dpron-i\")]");
                            var pronunText = pronun.SelectSingleNode(".//span[contains(@class,\"pron dpron\")]").InnerText;
                            var pronunPath = pronun.SelectSingleNode(".//source[1]").GetAttributeValue("src", null);
                            pronunPath = "https://dictionary.cambridge.org" + pronunPath;
                            var vocabulary = new VocabularyEntity
                            {
                                Name = vocab,
                                Code = code,
                                Language = "en-us",
                                WordType = typeText,
                                Pronunciation = pronunText,
                                PronunAudioPath = pronunPath,
                                Created = DateTime.UtcNow,
                                Description = string.Join(", ", listExp.Where(t => t.WordType == typeText).Select(t => t.Meaning))
                            };
                            listVocab.Add(vocabulary);

                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
            }

            if (listVocab.Count() == 0) return;
            foreach (var vocal in listVocab)
                _vocabularyService.Save(vocal);
            return;
        }

        private async Task<String> GetContentOther(Table table, String type, String basis, LessonPartViewModel item, String createUser = null)
        {
            try
            {
                //description
                String html = await ConvertDocToHtml(table, basis, createUser);
                item.Description = html;

                //file 
                var linkcell = table.Rows[4].Cells[1];

                var linkfile = "";
                linkfile = await GetContentFile(basis, item, createUser, linkcell, linkfile);

                //foreach (Paragraph prg in linkcell.Paragraphs)
                //{

                //    foreach (DocumentObject obj in prg.ChildObjects)
                //    {

                //        if (obj is Field)
                //        {
                //            var link = obj as Field;
                //            if (link.Type == FieldType.FieldHyperlink)
                //            {
                //                item.Media = new Media
                //                {
                //                    Created = DateTime.UtcNow,
                //                    Path = link.Value.Replace("\"", ""),
                //                    OriginalName = link.FieldText,
                //                    Name = link.FieldText,
                //                    Extension = GetContentType(link.FieldText)
                //                };
                //                break;
                //            }
                //        }
                //        else if (obj is DocPicture)
                //        {
                //            var pic = obj as DocPicture;
                //            var filename = DateTime.Now.ToString("yyyyMMddhhmmss") + ".jpg";
                //            var path = SaveImageByByteArray(pic.ImageBytes, filename, basis);

                //            item.Media = new Media()
                //            {
                //                Created = DateTime.UtcNow,
                //                Path = path,
                //                OriginalName = filename,
                //                Name = filename,
                //                Extension = "image/jpg"
                //            };
                //            break;
                //        }
                //        else if (obj is DocOleObject)
                //        {
                //            Dictionary<String, String> pathFileOLE = await GetPathFileOLE(obj as DocOleObject, basis, createUser);
                //            item.Media = new Media()
                //            {
                //                Created = DateTime.UtcNow,
                //                Path = pathFileOLE["pathFileOLE"],
                //                OriginalName = pathFileOLE["packageFileName"],
                //                Name = pathFileOLE["packageFileName"],
                //                Extension = GetContentType(pathFileOLE["extension"])
                //            };
                //            break;
                //        }
                //        else if (obj is TextRange)
                //        {
                //            var str = (obj as TextRange).Text;
                //            if (str.ToLower().StartsWith("http"))
                //            {
                //                linkfile = str.Trim();
                //                var contentType = GetContentType(linkfile);
                //                if (contentType == "application/octet-stream")//unknown type
                //                {
                //                    switch (item.Type)
                //                    {
                //                        case "AUDIO":
                //                            contentType = "audio/mp3";
                //                            break;
                //                        case "VIDEO":
                //                            contentType = "video/mpeg";
                //                            break;
                //                        case "DOC":
                //                            contentType = "application/pdf";
                //                            break;
                //                        case "IMAGE":
                //                            contentType = "image/jpeg";
                //                            break;
                //                    }
                //                }
                //                item.Media = new Media()
                //                {
                //                    Created = DateTime.UtcNow,
                //                    Path = linkfile,
                //                    OriginalName = linkfile,
                //                    Name = linkfile,
                //                    Extension = contentType
                //                };
                //                break;
                //            }
                //        }
                //    }
                //}

                await CreateOrUpdateLessonPart(basis, item);
                return "";
            }
            catch (Exception ex)
            {
                return $"{type} is error {ex.Message}";
            }
        }

        private async Task<String> GetContentEssay(Table table, String type, String basis, LessonPartViewModel item, String createUser = null)
        {
            try
            {
                String descriptionLessonPart = "";
                String descriptionQUIZ = "";
                Double point = 0;

                var descCell = table.Rows[3].Cells[1];
                var paragraphs = descCell.Paragraphs;
                DocumentObject nextPrg = paragraphs[0];

                //lay mo ta
                while (nextPrg != null)
                {
                    if (!(nextPrg is Paragraph))
                    {
                        nextPrg = nextPrg.NextSibling as DocumentObject;
                        continue;
                    }
                    Paragraph para = nextPrg as Paragraph;
                    nextPrg = para.NextSibling as DocumentObject;

                    var content = para.Text.Trim();
                    //if (para.Text.ToUpper().Contains("[DES]"))
                    //    descCell.Paragraphs.Remove(para);
                    //else if (para.Text.Trim().ToUpper().Contains($"[EX]"))
                    if (para.Text.Trim().ToUpper().Contains($"[E]"))
                    {
                        var str = content.Replace($"[E]", "").Trim();
                        descriptionQUIZ += str;
                        while (para.NextSibling != null && (
                            !(para.NextSibling as Paragraph).Text.Contains($"[E]")
                            && !(para.NextSibling as Paragraph).Text.Contains($"[P]")))
                        {
                            descriptionQUIZ += (para.NextSibling as Paragraph)?.Text + " ";
                            if (!(para.NextSibling as Paragraph).Text.Trim().Contains("[P]"))
                            {
                                descCell.Paragraphs.Remove(para.NextSibling as Paragraph);
                            }
                        }
                        nextPrg = para.NextSibling as DocumentObject;
                        descCell.Paragraphs.Remove(para);
                    }
                    else if (para.Text.Trim().ToUpper().Contains($"[P]"))
                    {
                        var str = content.Replace($"[P]", "").Trim();
                        if (!String.IsNullOrEmpty(str))
                        {
                            Double.TryParse(str, out point);
                            while (para.NextSibling != null)
                                descCell.Paragraphs.Remove(para.NextSibling as Paragraph);
                            descCell.Paragraphs.Remove(para);
                        }
                        else
                        {
                            Double.TryParse((para.NextSibling as Paragraph)?.Text.Trim(), out point);
                            while (para.NextSibling != null)
                                descCell.Paragraphs.Remove(para.NextSibling as Paragraph);
                            descCell.Paragraphs.Remove(para);
                        }
                    }
                }


                descriptionLessonPart = await ConvertDocToHtml(table, basis, createUser);
                descriptionQUIZ = $"<p>{descriptionQUIZ}</p>";

                item.Description = descriptionLessonPart;
                item.Point = point;
                var question = new QuestionViewModel
                {
                    Created = DateTime.UtcNow,
                    CreateUser = createUser,
                    Description = descriptionQUIZ,
                    Point = point,
                    MaxPoint = point
                };
                item.Questions = new List<QuestionViewModel>();
                item.Questions.Add(question);

                //file 
                var linkcell = table.Rows[4].Cells[1];

                var linkfile = "";
                linkfile = await GetContentFile(basis, item, createUser, linkcell, linkfile);

                await CreateOrUpdateLessonPart(basis, item);
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private async Task<String> ConvertDocToHtml(Table table, string basis, string user)
        {
            var cell = table.Rows[3].Cells[1];

            //Create New doc2
            Document doc2 = new Document();
            Section s2 = doc2.AddSection();

            foreach (DocumentObject item in cell.ChildObjects)
            {
                if (item is Table)
                {
                    Table newtb = (Table)item.Clone();
                    s2.Tables.Add(newtb);
                }
                else
                if (item is Paragraph)
                {
                    Paragraph newPara = (Paragraph)item.Clone();
                    s2.Paragraphs.Add(newPara);
                }
            }
            var path = Path.Combine(StaticPath, "Files/" + basis + "/" + user);
            var temp = DateTime.Now.ToString("yyyyMMhhmmss");

            String content = "";
            using (MemoryStream memory = new MemoryStream())
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                doc2.SaveToFile(path + "/" + temp + ".html", FileFormat.Html);
                using (var reader = new StreamReader(path + "/" + temp + ".html"))
                {
                    content = reader.ReadToEnd();
                }
                try
                {
                    System.IO.File.Delete(path + "/" + temp + ".html");
                    System.IO.File.Delete(path + "/" + temp + ".css");
                    System.IO.File.Delete(path + "/" + temp + "_styles.css");
                }
                catch
                {
                }
            }

            for (int i = 0; i < 1; i++)
            {
                Int32 lastIndex = content.IndexOf("</p>");
                content = content.ToString().Remove(0, lastIndex + 4);
            }
            String strToReplace4 = "<br></div></body></html>";
            String strToReplace2 = @"</div></body></html>";
            String strToreplace3 = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\"><html xmlns=\"http://www.w3.org/1999/xhtml\"><head><meta http-equiv=\"Content-Type\" content=\"application/xhtml+xml; charset=utf-8\" /><title></title></head><body style=\"pagewidth:595.35pt;pageheight:841.95pt;\"><div class=\"Section0\"><div style=\"min-height:20pt\" /><p class=\"Normal\"><span style=\"color:#FF0000;font-size:12pt;\"></span></p>";
            string test = content
                .ToString()
                .Replace("Evaluation Warning: The document was created with Spire.Doc for .NET.", "")
                .Replace(strToReplace4, "")
                .Replace(strToReplace2, "")
                .Replace(temp + "_images/", "/Files/" + basis + "/" + user + "/" + temp + "_images/")
                .Replace("<link href=\"" + temp + "_styles.css\" type=\"text/css\" rel=\"stylesheet\"/>", "")
                .Replace(strToreplace3, "");

            return test.Trim();
        }

        //private String ConvertHtmlToDoc(String html, String basis, String user, String type, List<String> lessonQuestionIDs = null, List<LessonPartAnswerEntity> lessonPartAnswers = null)
        //{
        //    try
        //    {
        //        if (String.IsNullOrEmpty(html)) return "";
        //        String _html = "";
        //        if (type.Equals("QUIZ2"))
        //        {
        //            if (lessonQuestionIDs == null || lessonPartAnswers == null) return "";

        //            var str = html.Replace("\"", "'").Replace("<fillquiz><input class='fillquiz' type='text'></fillquiz>", "|").Split('|');
        //            Int32 indexStr = 0;
        //            foreach (var questionID in lessonQuestionIDs)
        //            {
        //                var _listAns = lessonPartAnswers.FindAll(x => questionID.Contains(x.ParentID));
        //                if (_listAns.Count() > 1)
        //                {
        //                    String contentAns = "";
        //                    foreach (var item in _listAns)
        //                    {
        //                        contentAns += $"{item.Content}|";
        //                    }
        //                    contentAns = contentAns.Remove(contentAns.LastIndexOf('|'));
        //                    if (indexStr + 1 != str.Length)
        //                        _html += $"{str[indexStr]}{{{{{contentAns}}}}}";
        //                    if (indexStr + 1 == str.Length)
        //                        _html += str[indexStr + 1];
        //                }
        //                else
        //                {
        //                    if (indexStr + 1 != str.Length)
        //                        _html += $"{str[indexStr]}{{{{{_listAns[0].Content}}}}}";
        //                    if (indexStr + 1 == str.Length)
        //                        _html += str[indexStr + 1];
        //                }
        //                indexStr++;
        //            }
        //        }

        //        html = String.IsNullOrEmpty(_html) ? html : _html;
        //        String fileName = $"temp{DateTime.Now.ToString("yyyyMMddHHmmss")}.html";
        //        String folder = $"/Temp/{basis}/{user}/";
        //        string uploads = $"{RootPath}{folder}";
        //        if (!Directory.Exists(uploads))
        //        {
        //            Directory.CreateDirectory(uploads);
        //        }
        //        String strDoc = "";
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            StreamWriter writer = new StreamWriter(ms);

        //            writer.WriteLine(html);
        //            writer.Flush();

        //            //You have to rewind the MemoryStream before copying
        //            ms.Seek(0, SeekOrigin.Begin);

        //            using (FileStream fs = new FileStream(uploads + fileName, FileMode.OpenOrCreate))
        //            {
        //                ms.CopyTo(fs);
        //                fs.Flush();
        //            }

        //            using (MemoryStream memory = new MemoryStream())
        //            {
        //                Document doc = new Document();
        //                doc.LoadFromFile(uploads + fileName, FileFormat.Html, XHTMLValidationType.None);

        //                strDoc = doc.GetText();
        //            }

        //            System.IO.File.Delete(uploads + fileName);
        //        }
        //        return strDoc.Replace("Evaluation Warning: The document was created with Spire.Doc for .NET.", "");
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}

        //private Document ConvertHtmlToWordDoc(String html, String basis, String user, String type, List<String> lessonQuestionIDs = null, List<LessonPartAnswerEntity> lessonPartAnswers = null)
        //{
        //    var retDoc = new Document();
        //    try
        //    {
        //        if (String.IsNullOrEmpty(html)) return retDoc;
        //        String _html = "";
        //        if (type.Equals("QUIZ2"))
        //        {
        //            if (lessonQuestionIDs == null || lessonPartAnswers == null) return retDoc;

        //            var str = html.Replace("\"", "'").Replace("<fillquiz><input class='fillquiz' type='text'></fillquiz>", "|").Split('|');
        //            Int32 indexStr = 0;
        //            foreach (var questionID in lessonQuestionIDs)
        //            {
        //                var _listAns = lessonPartAnswers.FindAll(x => questionID.Contains(x.ParentID));
        //                if (_listAns.Count() > 1)
        //                {
        //                    String contentAns = "";
        //                    foreach (var item in _listAns)
        //                    {
        //                        contentAns += $"{item.Content}|";
        //                    }
        //                    contentAns = contentAns.Remove(contentAns.LastIndexOf('|'));
        //                    if (indexStr + 1 != str.Length)
        //                        _html += $"{str[indexStr]}{{{{{contentAns}}}}}";
        //                    if (indexStr + 1 == str.Length)
        //                        _html += str[indexStr + 1];
        //                }
        //                else
        //                {
        //                    if (indexStr + 1 != str.Length)
        //                        _html += $"{str[indexStr]}{{{{{_listAns[0].Content}}}}}";
        //                    if (indexStr + 1 == str.Length)
        //                        _html += str[indexStr + 1];
        //                }
        //                indexStr++;
        //            }
        //        }

        //        html = String.IsNullOrEmpty(_html) ? html : _html;
        //        String fileName = $"temp{DateTime.Now.ToString("yyyyMMddHHmmss")}.html";
        //        String folder = $"/Temp/{basis}/{user}/";
        //        string uploads = $"{RootPath}{folder}";
        //        if (!Directory.Exists(uploads))
        //        {
        //            Directory.CreateDirectory(uploads);
        //        }
        //        String strDoc = "";
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            StreamWriter writer = new StreamWriter(ms);

        //            writer.WriteLine(html);
        //            writer.Flush();

        //            //You have to rewind the MemoryStream before copying
        //            ms.Seek(0, SeekOrigin.Begin);

        //            using (FileStream fs = new FileStream(uploads + fileName, FileMode.OpenOrCreate))
        //            {
        //                ms.CopyTo(fs);
        //                fs.Flush();
        //            }

        //            using (MemoryStream memory = new MemoryStream())
        //            {
        //                Document doc = new Document();
        //                doc.LoadFromFile(uploads + fileName, FileFormat.Html, XHTMLValidationType.None);

        //                retDoc = doc;
        //            }

        //            System.IO.File.Delete(uploads + fileName);
        //        }
        //        return retDoc;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw (ex);
        //    }
        //}

        private async Task<Dictionary<String, String>> GetPathFileOLE(Paragraph para, String basis, String createUser)
        {
            Dictionary<String, String> dataResponse = new Dictionary<String, String>();
            String[] typeVideo = { ".ogm", ".wmv", ".mpg", ".webm", ".ogv", ".mov", ".asx", ".mpge", ".mp4", ".m4v", ".avi" };
            String[] typeAudio = { ".opus", ".flac", ".weba", ".webm", ".wav", ".ogg", ".m4a", ".oga", ".mid", ".mp3", ".aiff", ".wma", ".au" };
            String path = "";
            String user = String.IsNullOrEmpty(createUser) ? "admin" : createUser;
            foreach (DocumentObject obj in para.ChildObjects)
            {
                if (obj.DocumentObjectType == DocumentObjectType.OleObject)
                {
                    DocOleObject Ole = obj as DocOleObject;
                    String typeFile = Ole.ObjectType.ToUpper();
                    String packageFileName = Ole.PackageFileName.ToString().Trim();
                    String extension = Path.GetExtension(packageFileName);
                    dataResponse.Add("packageFileName", packageFileName);
                    dataResponse.Add("extension", extension);
                    if (typeFile.Contains("AcroExch.Document.11".ToUpper()))//pdf
                    {
                        path = await SaveFileToDrive(Ole, user, extension, basis);
                        dataResponse.Add("pathFileOLE", path);
                    }
                    else if (typeFile.Contains("Excel.Sheet.12".ToUpper()) || typeFile.Contains("Excel.Sheet.8".ToUpper()))//excel - check
                    {
                        path = await SaveFileToDrive(Ole, user, extension, basis);
                        dataResponse.Add("pathFileOLE", path);
                    }
                    //else if(typeFile.Contains("Excel.Sheet.12".ToUpper()) || typeFile.Contains("Excel.Sheet.8".ToUpper()))//doc
                    //{

                    //}
                    else if (typeFile.Contains("PowerPoint.Slide.8".ToUpper()) || typeFile.Contains("PowerPoint.Slide.12".ToUpper()))//power point
                    {
                        path = await SaveFileToDrive(Ole, user, extension, basis);
                        dataResponse.Add("pathFileOLE", path);
                    }
                    else
                    {
                        if (typeVideo.Contains(extension))//video
                        {
                            path = await SaveFileToDrive(Ole, user, extension, basis);
                            dataResponse.Add("pathFileOLE", path);
                        }
                        else if (typeAudio.Contains(extension))//audio
                        {
                            path = await SaveFileToDrive(Ole, user, extension, basis);
                            dataResponse.Add("pathFileOLE", path);
                        }
                        else
                        {
                            path = await SaveFileToDrive(Ole, user, extension, basis);
                            dataResponse.Add("pathFileOLE", path);
                        }
                    }
                }
            }
            return dataResponse;
        }

        private async Task<Dictionary<String, String>> GetPathFileOLE(DocOleObject Ole, String basis, String createUser)
        {
            Dictionary<String, String> dataResponse = new Dictionary<String, String>();
            String[] typeVideo = { ".ogm", ".wmv", ".mpg", ".webm", ".ogv", ".mov", ".asx", ".mpge", ".mp4", ".m4v", ".avi" };
            String[] typeAudio = { ".opus", ".flac", ".weba", ".webm", ".wav", ".ogg", ".m4a", ".oga", ".mid", ".mp3", ".aiff", ".wma", ".au" };
            String path = "";
            String user = String.IsNullOrEmpty(createUser) ? "admin" : createUser;
            //foreach (DocumentObject obj in para.ChildObjects)
            //{
            //    if (obj.DocumentObjectType == DocumentObjectType.OleObject)
            //    {
            String typeFile = Ole.ObjectType.ToUpper();
            String packageFileName = Ole.PackageFileName.ToString().Trim();


            var filename = packageFileName.Replace("\\", "#").Split('#').Last();
            String extension = Path.GetExtension(filename);
            dataResponse.Add("packageFileName", filename);
            dataResponse.Add("extension", extension);
            if (typeFile.Contains("AcroExch.Document.11".ToUpper()))//pdf
            {
                path = await SaveFileToDrive(Ole, user, extension, basis);
                dataResponse.Add("pathFileOLE", path);
            }
            else if (typeFile.Contains("Excel.Sheet.12".ToUpper()) || typeFile.Contains("Excel.Sheet.8".ToUpper()))//excel - check
            {
                path = await SaveFileToDrive(Ole, user, extension, basis);
                dataResponse.Add("pathFileOLE", path);
            }
            //else if(typeFile.Contains("Excel.Sheet.12".ToUpper()) || typeFile.Contains("Excel.Sheet.8".ToUpper()))//doc
            //{

            //}
            else if (typeFile.Contains("PowerPoint.Slide.8".ToUpper()) || typeFile.Contains("PowerPoint.Slide.12".ToUpper()))//power point
            {
                path = await SaveFileToDrive(Ole, user, extension, basis);
                dataResponse.Add("pathFileOLE", path);
            }
            else
            {
                if (typeVideo.Contains(extension))//video
                {
                    path = await SaveFileToDrive(Ole, user, extension, basis);
                    dataResponse.Add("pathFileOLE", path);
                }
                else if (typeAudio.Contains(extension))//audio
                {
                    path = await SaveFileToDrive(Ole, user, extension, basis);
                    dataResponse.Add("pathFileOLE", path);
                }
                else
                {
                    path = await SaveFileToDrive(Ole, user, extension, basis);
                    dataResponse.Add("pathFileOLE", path);
                }
                //    }
                //}
            }
            return dataResponse;
        }

        private async Task<String> SaveFileToDrive(DocOleObject Ole, String user, String extension, String basis)
        {
            Byte[] bytes = Ole.NativeData;
            String path = "";
            using (MemoryStream memory = new MemoryStream(bytes))
            {
                path = _roxyFilemanHandler.GoogleDriveApiService.CreateLinkViewFile(_roxyFilemanHandler.UploadFileWithGoogleDrive(basis, user, memory, extension));
                memory.Close();
            }
            return path;
        }

        private Dictionary<String, String> GetContentFile(Spire.Doc.Collections.ParagraphCollection documentObject, String basis, String createUser)
        {
            Dictionary<String, String> dataResponse = new Dictionary<String, String>();
            foreach (DocumentObject docObject in documentObject[0].ChildObjects)
            {
                if (docObject.DocumentObjectType == DocumentObjectType.Picture)
                {
                    DocPicture picture = docObject as DocPicture;
                    string fileName = string.Format($"Image{DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")}.png");
                    var pathImage = SaveImageByByteArray(picture.ImageBytes, fileName, createUser, basis);
                    dataResponse.Add("FileName", fileName);
                    dataResponse.Add("FilePath", pathImage);
                }
            }
            return dataResponse;
        }

        private async Task<string> GetContentFile(string basis, LessonPartViewModel item, string createUser, TableCell linkcell, string linkfile)
        {
            foreach (Paragraph prg in linkcell.Paragraphs)
            {

                foreach (DocumentObject obj in prg.ChildObjects)
                {

                    if (obj is Field)
                    {
                        var link = obj as Field;
                        if (link.Type == FieldType.FieldHyperlink)
                        {
                            item.Media = new Media
                            {
                                Created = DateTime.UtcNow,
                                Path = link.Value.Replace("\"", ""),
                                OriginalName = link.FieldText,
                                Name = link.FieldText,
                                Extension = GetContentType(link.FieldText)
                            };
                            break;
                        }
                    }
                    else if (obj is DocPicture)
                    {
                        var pic = obj as DocPicture;
                        var filename = DateTime.Now.ToString("yyyyMMddhhmmssfff") + ".jpg";
                        var path = SaveImageByByteArray(pic.ImageBytes, filename, createUser, basis);

                        item.Media = new Media()
                        {
                            Created = DateTime.UtcNow,
                            Path = path,
                            OriginalName = filename,
                            Name = filename,
                            Extension = "image/jpg"
                        };
                        break;
                    }
                    else if (obj is DocOleObject)
                    {
                        Dictionary<String, String> pathFileOLE = await GetPathFileOLE(obj as DocOleObject, basis, createUser);
                        item.Media = new Media()
                        {
                            Created = DateTime.UtcNow,
                            Path = pathFileOLE["pathFileOLE"],
                            OriginalName = pathFileOLE["packageFileName"],
                            Name = pathFileOLE["packageFileName"],
                            Extension = GetContentType(pathFileOLE["extension"])
                        };
                        break;
                    }
                    else if (obj is TextRange)
                    {
                        var str = (obj as TextRange).Text;
                        if (str.ToLower().StartsWith("http"))
                        {
                            linkfile = str.Trim();
                            var contentType = GetContentType(linkfile);
                            if (contentType == "application/octet-stream")//unknown type
                            {
                                switch (item.Type)
                                {
                                    case "AUDIO":
                                        contentType = "audio/mp3";
                                        break;
                                    case "VIDEO":
                                        contentType = "video/mpeg";
                                        break;
                                    case "DOC":
                                        contentType = "application/pdf";
                                        break;
                                    case "IMAGE":
                                        contentType = "image/jpeg";
                                        break;
                                }
                            }
                            item.Media = new Media()
                            {
                                Created = DateTime.UtcNow,
                                Path = linkfile,
                                OriginalName = linkfile,
                                Name = linkfile,
                                Extension = contentType
                            };
                            break;
                        }
                    }
                }
            }

            return linkfile;
        }

        private string SaveImageByByteArray(byte[] byteArrayIn, string fileName, String user, string center = "")
        {
            try
            {
                return FileProcess.ConvertImageByByteArray(byteArrayIn, fileName, $"{center}/{user}", RootPath);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private async Task CreateOrUpdateLessonPart(string basis, LessonPartViewModel item)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var parentLesson = _lessonService.GetItemByID(item.ParentID);
                var createduser = User.Claims.GetClaimByType("UserID").Value;
                if (parentLesson != null)
                {
                    var isPractice = parentLesson.IsPractice;
                    var maxItem = _lessonPartService.CreateQuery()
                            .Find(o => o.ParentID == item.ParentID)
                            .SortByDescending(o => o.Order).FirstOrDefault();

                    item.CourseID = parentLesson.CourseID;
                    item.Created = DateTime.UtcNow;
                    item.Order = maxItem != null ? maxItem.Order + 1 : 0;
                    item.Updated = DateTime.UtcNow;

                    var lessonpart = item.ToEntity();
                    _lessonPartService.CreateQuery().InsertOne(lessonpart);
                    item.ID = lessonpart.ID;

                    switch (lessonpart.Type)
                    {
                        case "QUIZ2": //remove all previous question
                            item.CourseID = parentLesson.CourseID;

                            if (item.Questions != null && item.Questions.Count > 0)
                            {
                                await SaveQA(item, UserID);
                            }
                            isPractice = true;
                            break;
                        default://QUIZ1,3,4
                            item.CourseID = parentLesson.CourseID;

                            if (item.Questions != null && item.Questions.Count > 0)
                            {
                                await SaveQA(item, UserID);
                            }
                            isPractice = true;
                            break;
                    }

                    if (parentLesson.TemplateType == LESSON_TEMPLATE.LECTURE && parentLesson.IsPractice != isPractice)//non-practice => practice
                    {
                        parentLesson.IsPractice = isPractice;
                        _lessonService.Save(parentLesson);
                        //increase practice counter
                        await _courseHelper.ChangeLessonPracticeState(parentLesson);
                    }
                    //calculateLessonPoint(item.ParentID);

                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                String msg = ex.Message;
            }
        }

        private async Task SaveQA(LessonPartViewModel item, string UserID)
        {
            foreach (var questionVM in item.Questions)
            {
                questionVM.ParentID = item.ID;
                questionVM.CourseID = item.CourseID;

                var quiz = questionVM.ToEntity();
                if (questionVM.ID == "0" || questionVM.ID == null || _lessonPartQuestionService.GetItemByID(quiz.ID) == null)
                {
                    var _maxItem = _lessonPartQuestionService.CreateQuery()
                        .Find(o => o.ParentID == item.ID)
                        .SortByDescending(o => o.Order).FirstOrDefault();
                    quiz.Order = questionVM.Order;
                    quiz.Created = DateTime.UtcNow;
                    quiz.Updated = DateTime.UtcNow;
                    quiz.CreateUser = UserID;
                }
                _lessonPartQuestionService.CreateQuery().InsertOne(quiz);

                questionVM.ID = quiz.ID;

                if (questionVM.Answers != null && questionVM.Answers.Count > 0)
                {
                    foreach (var answer in questionVM.Answers)
                    {
                        answer.ParentID = questionVM.ID;
                        var _maxItem1 = _lessonPartAnswerService.CreateQuery().Find(o => o.ParentID == quiz.ID).SortByDescending(o => o.Order).FirstOrDefault();
                        answer.Order = _maxItem1 != null ? _maxItem1.Order + 1 : 0;
                        answer.Created = DateTime.UtcNow;
                        answer.Updated = DateTime.UtcNow;
                        answer.CreateUser = quiz.CreateUser;
                        answer.CourseID = quiz.CourseID;
                        _lessonPartAnswerService.CreateQuery().InsertOne(answer);
                    }
                }
            }
        }

        private List<QuestionViewModel> ExtractFillQuestionList(LessonPartEntity item, string creator, out string Description)
        {
            Description = item.Description;
            var questionList = new List<QuestionViewModel>();
            //extract Question from Description
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(item.Description);
            var fillquizs = doc.DocumentNode.SelectNodes(".//fillquiz[*[contains(@class,\"fillquiz\")]]");
            if (fillquizs == null || fillquizs.Count() == 0)
                return questionList;

            for (int i = 0; i < fillquizs.Count(); i++)
            {
                var quiz = fillquizs[i];

                var inputNode = quiz.SelectSingleNode(".//*[contains(@class,\"fillquiz\")]");
                if (inputNode == null)
                {
                    continue;
                }

                //move text behind fillQuiz
                var textNode = quiz.SelectSingleNode(".//text()");
                if (textNode != null)
                {
                    var cloneNode = textNode.Clone();
                    textNode.Remove();
                    quiz.ParentNode.InsertAfter(cloneNode, quiz);
                }

                var ans = inputNode.GetAttributeValue("ans", null);
                if (ans == null)
                    ans = inputNode.GetAttributeValue("placeholder", null);
                if (string.IsNullOrEmpty(ans))
                {
                    inputNode.Remove();
                    continue;
                }
                var Question = new QuestionViewModel
                {
                    ParentID = item.ID,
                    CourseID = item.CourseID,
                    CreateUser = creator,
                    Order = i,
                    Point = 1,
                    Content = inputNode.GetAttributeValue("dsp", null),//phần hiển thị cho học viên
                    Description = quiz.GetAttributeValue("title", null),//phần giải thích đáp án
                    Answers = new List<LessonPartAnswerEntity>
                    {

                    }

                };

                var ansArr = ans.Split('|');
                foreach (var answer in ansArr)
                {
                    //var validAns = validateFill(answer);
                    var validAns = answer;
                    if (!string.IsNullOrEmpty(validAns))
                    {
                        Question.Answers.Add(new LessonPartAnswerEntity
                        {
                            CourseID = item.CourseID,
                            CreateUser = creator,
                            IsCorrect = true,
                            Content = StringHelper.ReplaceSpecialCharacters(validAns)
                        });
                    }
                }

                questionList.Add(Question);
                //var clearnode = HtmlNode.CreateNode("<input></input>");
                //clearnode.AddClass("fillquiz");
                inputNode.Attributes.Remove("contenteditable");
                inputNode.Attributes.Remove("readonly");
                inputNode.Attributes.Remove("title");
                inputNode.Attributes.Remove("value");
                inputNode.Attributes.Remove("dsp");
                inputNode.Attributes.Remove("ans");
                inputNode.Attributes.Remove("placeholder");
                inputNode.Attributes.Remove("size");

                quiz.Attributes.Remove("contenteditable");
                quiz.Attributes.Remove("readonly");
                quiz.Attributes.Remove("title");
                //quiz.ChildNodes.Add(clearnode);
            }

            var removeNodes = doc.DocumentNode.SelectNodes(".//fillquiz[not(input)]");
            if (removeNodes != null && removeNodes.Count() > 0)
                foreach (var node in removeNodes)
                    node.Remove();

            Description = doc.DocumentNode.OuterHtml.ToString();
            return questionList;
        }



        private string validateFill(string org)
        {
            if (string.IsNullOrEmpty(org)) return org;
            org = org.Trim();
            while (org.IndexOf("  ") >= 0)
                org = org.Replace("  ", " ");


            org = StringHelper.ReplaceSpecialCharacters(org.Trim());
            return org;
        }

        private static String Note = $"\nGiải thích kí hiệu" +
            $"\n1. [Câu hỏi] : Bắt đầu câu hỏi" +
            $"\n2. [Đáp án] : Bắt đầu đáp án" +
            $"\n3. [Qxxx] : Nội dung câu hỏi (Đối với các dạng câu hỏi trắc nghiệm)." +
            $"\n4. _Qxxx_ : Nội dung câu hỏi (Đối với dạng điền từ - đánh dấu vị trí điền từ)." +
            $"\n5. [Axxx] : Nội dung câu trả lời; các câu hỏi có nhiều đáp án được viết trên cùng một dòng, cách nhau bởi dấu |" +
            $"\n6. (x) : Đánh dấu câu trả lời đúng (Đối với các dạng trắc nghiệm)." +
            $"\n7. [Exxx] : Giải thích cho đáp án đúng" +
            $"\n8. [Hxxx] : Phần hiển thị với học viên tại vị trí điền từ" +
            $"\n9. [P] : Điểm (Đối với dạng essay)" +
            $"\nLưu ý: xxx là số thứ tự câu hỏi/câu trả lời;" +
            $"\nLiên kết hình ảnh/media có dạng http://... hoặc https://...";

        #endregion
        private string GetContentType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings.Add(".dnct", "application/dotnetcoretutorials");
            string contentType;
            if (!provider.TryGetContentType(fileName, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }

    }

    public class Counter
    {
        public int Exam { get; set; }
        public int Lesson { get; set; }
    }
}
