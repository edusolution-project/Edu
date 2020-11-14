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

        private readonly List<string> quizType = new List<string> { "QUIZ1", "QUIZ2", "QUIZ3", "QUIZ4", "ESSAY" };
        private string RootPath { get; }

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
                 ModCourseService modservice

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

            RootPath = (config.GetValue<string>("SysConfig:StaticPath") ?? evn.WebRootPath) + "/Files";
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
                var subject = _subjectService.CreateQuery().Find(t => teacher.Subjects.Contains(t.ID)).ToList();
                var grade = _gradeService.CreateQuery().Find(t => teacher.Subjects.Contains(t.SubjectID)).ToList();
                var skills = _skillService.GetList();
                var courses = _service.CreateQuery().Find(t => t.Center.Equals(center.ID)).SortByDescending(o => o.ID).ToList();
                ViewBag.Grades = grade;
                ViewBag.Subjects = subject;
                ViewBag.Skills = skills;
                ViewBag.Courses = courses;
            }

            ViewBag.AllCenters = teacher.Centers.Select(t => new CenterEntity { Code = t.Code, Name = t.Name, ID = t.CenterID }).ToList();

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

                        var filename = DateTime.UtcNow.ToString("yyyyMMddhhmmss") + Path.GetExtension(file.FileName);
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
                    olditem.SkillID = item.SkillID;
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

                        var filename = DateTime.UtcNow.ToString("yyyyMMddhhmmss") + Path.GetExtension(file.FileName);
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

                    var filename = currentCourse.ID + "_" + DateTime.UtcNow.ToString("yyyyMMddhhmmss") + Path.GetExtension(file.FileName);
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
            new_course.IsActive = true;
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
                if (data == null)
                {
                    item.Created = DateTime.UtcNow;
                    item.IsAdmin = true;
                    item.IsActive = false;
                    item.Updated = DateTime.UtcNow;
                    item.Order = int.MaxValue - 1;
                    _chapterService.Save(item);
                    ChangeChapterPosition(item, int.MaxValue);//move chapter to bottom of new parent chap
                }
                else
                {
                    item.Updated = DateTime.UtcNow;
                    var newOrder = item.Order - 1;
                    var oldParent = data.ParentID;

                    data.Name = item.Name;
                    data.ParentID = item.ParentID;
                    data.Description = item.Description;

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
                    {"Error",ex.Message }
                });
            }
        }

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

            if (pos > parts.Count())
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
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var data = _lessonService.GetItemByID(item.ID);
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

                    if (data.TemplateType == LESSON_TEMPLATE.LECTURE)
                        data.Limit = 0;

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

                    _lessonService.CreateQuery().ReplaceOne(o => o.ID == data.ID, data);

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
                    {"Error",ex.Message }
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
                        if (quizType.Contains(part.Type))
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
            return _lessonPartService.GetByLessonID(ID).Any(t => quizType.Contains(t.Type));
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

        #region Import with Word

        public IActionResult ExportQuestionTemplateWithWord()
        {

            var lessonPart = _lessonPartService.GetByLessonID("5fa574b67b16a810dcc35680");
            var lessonPartIDs = lessonPart.Select(x => x.ID);
            var lessonPartQuestion = _lessonPartQuestionService.CreateQuery().Find(x => lessonPartIDs.Contains(x.ParentID)).ToEnumerable();
            var lessonPartQuestionIDs = lessonPartQuestion.Select(x => x.ID);
            var lessonPartAnswer = _lessonPartAnswerService.CreateQuery().Find(x => lessonPartQuestionIDs.Contains(x.ParentID)).ToEnumerable();

            //LessonPartEntity lessonPart = new LessonPartEntity
            //{
            //    Title = "",
            //    Description = "",
            //    Point = 0,
            //    //new Media().Path = @"H://Template/quiz_example.png"
            //};

            //LessonPartQuestionEntity lessonPartQuestion = new LessonPartQuestionEntity
            //{
            //    Content = "What is the day today?",
            //    Description = "Nội dung mô tả",
            //    Media = new Media(),
            //};

            //LessonPartAnswerEntity lessonPartAnswer = new LessonPartAnswerEntity
            //{
            //    Content = "",
            //    Media = new Media(),
            //    IsCorrect = true
            //};

            //try
            //{
            byte[] toArray = null;
            using (var stream = new MemoryStream())
            {
                Document doc = new Document();
                Section s = doc.AddSection();
                s.PageSetup.Orientation = PageOrientation.Landscape;

                //for (int x = 0; x < 10; x++)
                for (int x = 0; x < lessonPart.Count(); x++)
                {
                    var _lessonPart = lessonPart.ElementAtOrDefault(x);
                    if (_lessonPart.Type.Equals("DOC") || _lessonPart.Type.Equals("0")) continue;
                    Table table = s.AddTable(true);

                    

                    //Create Title, Header and Data
                    String[] Title = { "Tiêu đề", _lessonPart.Title };
                    String[] Type = { "Kiểu (Đánh X)", "Văn bản", "Audio", "Video", "Hình ảnh", "Từ vựng", "Quiz1", "Quiz2", "Quiz3", "Quiz4", "Essay" };
                    String[] Content = { "Nội dung", _lessonPart.Description };
                    String[] File = { "File", "" };
                    String[] Point = { "Điểm", "(0 - 100: chỉ áp dụng cho bài tự luận - các nội dung khác có trắc nghiệm, điền từ tự động tính mỗi câu hỏi | vị trí điền từ 1 điểm)" };
                    String[] Header = { "STT", "Thông tin câu hỏi" };
                    String[] type = { "QUIZ1", "QUIZ3", "QUIZ4" };

                    //Add Cells
                    //table.ResetCells(16, Type.Length);
                    var lessonQinPart = lessonPartQuestion.ToList().FindAll(o => o.ParentID == _lessonPart.ID);
                    var lessonAinPart = lessonPartAnswer.ToList().FindAll(o => lessonQinPart.Select(a => a.ID).Contains(o.ParentID));

                    Int32 lengthData = //lessonAinPart.Count() + 
                        5 * lessonQinPart.Count();
                    //???????????
                    //table.ResetCells(type.Contains(_lessonPart.Type) ? lengthData : 6, Type.Length);
                    //hardfix for test
                    table.ResetCells(50, Type.Length);



                    #region Title Row
                    TableRow TitleRow = table.Rows[0];

                    

                    //TitleRow.IsHeader = true;
                    TitleRow.Height = 23;//row height
                    TitleRow.RowFormat.BackColor = Color.AliceBlue;
                    for (int i = 0; i < 2; i++)
                    {
                        //Cell Alignment
                        Paragraph p = TitleRow.Cells[i].AddParagraph();
                        TitleRow.Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Middle;

                        p.Format.HorizontalAlignment = HorizontalAlignment.Left;
                        TextRange TR = p.AppendText(Title[i]);
                        TR.CharacterFormat.Bold = false;
                    }
                    //Merge Cell
                    table.ApplyHorizontalMerge(0, 1, Type.Length - 1);//ghep cac o canh nhau
                    #endregion

                    #region Type Row
                    TableRow TypeRow = table.Rows[1];
                    //TitleRow.IsHeader = true;
                    TitleRow.Height = 23;//row height
                    TitleRow.RowFormat.BackColor = Color.AliceBlue;
                    for (int i = 0; i < Type.Length; i++)
                    {
                        //Cell Alignment
                        Paragraph p = TypeRow.Cells[i].AddParagraph();
                        TypeRow.Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        p.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        //Data Format
                        TextRange TR = p.AppendText(Type[i]);
                    }

                    TableRow CheckRow = table.Rows[2];
                    CheckRow.Height = 23;//row height
                    CheckRow.RowFormat.BackColor = Color.AliceBlue;
                    String[] type1 = { "TEXT", "AUDIO", "VIDEO", "IMG", "VOCAB", "QUIZ1", "QUIZ2", "QUIZ3", "QUIZ4", "ESSAY" };
                    for (Int32 indexType = 0; indexType < type1.Length; indexType++)
                    {
                        if (type1[indexType] == _lessonPart.Type)
                        {
                            CheckRow.Cells[indexType + 1].AddParagraph().AppendText("x");
                            CheckRow.Cells[indexType + 1].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            CheckRow.Cells[indexType + 1].AddParagraph().Format.HorizontalAlignment = HorizontalAlignment.Center;
                        }
                    }
                    //CheckRow.Cells[x + 1].AddParagraph().AppendText("x");
                    //CheckRow.Cells[x + 1].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                    //CheckRow.Cells[x + 1].AddParagraph().Format.HorizontalAlignment = HorizontalAlignment.Center;

                    table.ApplyVerticalMerge(0, 1, 2);//ghep cac o tren duoi lien nhau
                    #endregion

                    #region Content + File +Point Row
                    TableRow ContentRow = table.Rows[3];
                    //TitleRow.IsHeader = true;
                    ContentRow.Height = 23;//row height
                    ContentRow.RowFormat.BackColor = Color.AliceBlue;
                    for (int i = 0; i < 2; i++)
                    {
                        //Cell Alignment
                        Paragraph p = ContentRow.Cells[i].AddParagraph();
                        ContentRow.Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        p.Format.HorizontalAlignment = HorizontalAlignment.Left;
                        //Data Format
                        TextRange TR = p.AppendText(Content[i]);
                        TR.CharacterFormat.FontName = "Calibri";
                        TR.CharacterFormat.FontSize = 13;
                        TR.CharacterFormat.TextColor = Color.Black;
                        TR.CharacterFormat.Bold = true;
                    }
                    //Merge Cell
                    table.ApplyHorizontalMerge(3, 1, Type.Length - 1);//ghep cac o canh nhau

                    TableRow FileRow = table.Rows[4];
                    //TitleRow.IsHeader = true;
                    FileRow.Height = 23;//row height
                    FileRow.RowFormat.BackColor = Color.AliceBlue;
                    for (int i = 0; i < 2; i++)
                    {
                        if (i == 0)
                        {
                            Paragraph p = FileRow.Cells[i].AddParagraph();
                            FileRow.Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            p.Format.HorizontalAlignment = HorizontalAlignment.Left;

                            //Data Format
                            TextRange TR = p.AppendText(File[i]);
                            TR.CharacterFormat.FontName = "Calibri";
                            TR.CharacterFormat.FontSize = 13;
                            TR.CharacterFormat.TextColor = Color.Black;
                            TR.CharacterFormat.Bold = true;
                        }
                        else
                        {
                            //Cell Alignment
                            Paragraph p = FileRow.Cells[i].AddParagraph();
                            FileRow.Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            p.Format.HorizontalAlignment = HorizontalAlignment.Left;
                            DocPicture Pic = p.AppendPicture(ImageToByteArray(Image.FromFile(Path.Combine(RootPath, "quiz_example.png"))));
                            var scale = 100;
                            if (Pic.Width > 160)
                            {
                                scale = (int)(160.0 * 100 / Pic.Width);
                            }
                            Pic.WidthScale = scale;
                            Pic.HeightScale = scale;
                        }
                    }
                    //Merge Cell
                    table.ApplyHorizontalMerge(4, 1, Type.Length - 1);//ghep cac o canh nhau

                    TableRow PointRow = table.Rows[5];
                    //TitleRow.IsHeader = true;
                    PointRow.Height = 23;//row height
                    PointRow.RowFormat.BackColor = Color.AliceBlue;
                    for (int i = 0; i < 2; i++)
                    {
                        //Cell Alignment
                        Paragraph p = PointRow.Cells[i].AddParagraph();
                        PointRow.Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        p.Format.HorizontalAlignment = HorizontalAlignment.Left;
                        //Data Format
                        TextRange TR = p.AppendText(Point[i]);
                    }
                    //Merge Cell
                    table.ApplyHorizontalMerge(5, 1, Type.Length - 1);//ghep cac o canh nhau
                    #endregion

                    #region Quiz1,3,4
                    //if (x + 1 == 6 || x + 1 == 8 || x + 1 == 9)
                    if (type.Contains(_lessonPart.Type))
                    {
                        #region Câu hỏi trắc nghiệm (áp dụng với Quiz1, Quiz 3, Quiz 4)
                        TableRow NoteQuiz = table.Rows[6];
                        //TitleRow.IsHeader = true;
                        NoteQuiz.Height = 23;//row height
                        NoteQuiz.RowFormat.BackColor = Color.AliceBlue;
                        for (int i = 0; i < 1; i++)
                        {
                            //Cell Alignment
                            Paragraph p = NoteQuiz.Cells[i].AddParagraph();
                            NoteQuiz.Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            p.Format.HorizontalAlignment = HorizontalAlignment.Center;
                            //Data Format
                            TextRange TR = p.AppendText("Câu hỏi trắc nghiệm (áp dụng với Quiz1, Quiz 3, Quiz 4)");
                            TR.CharacterFormat.FontName = "Calibri";
                            TR.CharacterFormat.FontSize = 13;
                            TR.CharacterFormat.TextColor = Color.Black;
                            TR.CharacterFormat.Bold = true;
                        }
                        //Merge Cell
                        table.ApplyHorizontalMerge(6, 0, Type.Length - 1);//ghep cac o canh nhau
                        #endregion

                        #region Header Quiz Row
                        TableRow HeaderQuiz = table.Rows[7];
                        HeaderQuiz.IsHeader = true;
                        //Row Height
                        HeaderQuiz.Height = 23;
                        //Header Format
                        HeaderQuiz.RowFormat.BackColor = Color.AliceBlue;
                        for (int i = 0; i < 2; i++)
                        {
                            //Cell Alignment
                            Paragraph p = HeaderQuiz.Cells[i].AddParagraph();
                            HeaderQuiz.Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            p.Format.HorizontalAlignment = HorizontalAlignment.Center;
                            //Data Format
                            TextRange TR = p.AppendText(Header[i]);
                            TR.CharacterFormat.FontName = "Calibri";
                            TR.CharacterFormat.FontSize = 13;
                            TR.CharacterFormat.TextColor = Color.Black;
                            TR.CharacterFormat.Bold = true;
                        }
                        table.ApplyHorizontalMerge(7, 1, Type.Length - 1);//ghep cac o canh nhau
                        #endregion

                        #region ContentQuiz
                        var index = 8;
                        Int32 indexQuestion = 1;
                        foreach (var question in lessonQinPart.ToList())
                        {
                            //continue;
                            //Boolean isQuestion = true;
                            //var listAns = lessonPartAnswer.ToList().FindAll(o => o.ParentID == question.ID);
                            //if (isQuestion)
                            //{
                            //    indexQuestion++;
                            //    isQuestion = false;
                            TableRow row8 = table.Rows[index];
                            Paragraph pr81 = row8.Cells[0].AddParagraph();
                            TextRange trr81 = pr81.AppendText(indexQuestion.ToString());
                            Paragraph pr82 = row8.Cells[1].AddParagraph();
                            TextRange trr82 = pr82.AppendText("Tiêu đề");
                            Paragraph pr83 = row8.Cells[3].AddParagraph();
                            TextRange trr83 = pr83.AppendText(question.Content);
                            table.ApplyHorizontalMerge(index, 1, 2);
                            table.ApplyHorizontalMerge(index, 3, 10);
                            indexQuestion++;

                            TableRow row9 = table.Rows[index + 1];
                            Paragraph pr92 = row9.Cells[1].AddParagraph();
                            TextRange trr92 = pr92.AppendText("Mô tả");
                            Paragraph pr93 = row9.Cells[3].AddParagraph();
                            TextRange trr93 = pr93.AppendText(question.Description);
                            table.ApplyHorizontalMerge(index + 1, 1, 2);
                            table.ApplyHorizontalMerge(index + 1, 3, 10);

                            TableRow row10 = table.Rows[index + 2];
                            Paragraph pr102 = row10.Cells[1].AddParagraph();
                            TextRange trr102 = pr102.AppendText("File đính kèm");
                            Paragraph pr103 = row10.Cells[3].AddParagraph();
                            TextRange trr103 = pr103.AppendText(question.Media?.Path);
                            //DocPicture PicQ = pr103.AppendPicture(ImageToByteArray(Image.FromFile(@"H:/Template/example.png")));
                            //PicQ.Width = 50;
                            //PicQ.Height = 20;
                            table.ApplyHorizontalMerge(index + 2, 1, 2);
                            table.ApplyHorizontalMerge(index + 2, 3, 10);

                            TableRow row11 = table.Rows[index + 3];
                            Paragraph pr112 = row11.Cells[1].AddParagraph();
                            TextRange trr112 = pr112.AppendText("Đáp án");
                            Paragraph pr113 = row11.Cells[3].AddParagraph();
                            TextRange trr113 = pr113.AppendText("STT");
                            Paragraph pr114 = row11.Cells[4].AddParagraph();
                            TextRange trr114 = pr114.AppendText("Nội dung");
                            Paragraph pr117 = row11.Cells[7].AddParagraph();
                            TextRange trr117 = pr117.AppendText("Hình ảnh");
                            Paragraph pr1110 = row11.Cells[10].AddParagraph();
                            TextRange trr1110 = pr1110.AppendText("Đúng/Sai");
                            table.ApplyHorizontalMerge(index + 3, 1, 2);
                            table.ApplyHorizontalMerge(index + 3, 4, 6);
                            table.ApplyHorizontalMerge(index + 3, 7, 9);

                            var listAns = lessonPartAnswer.ToList().FindAll(o => o.ParentID == question.ID);
                            Int32 indexAns = 0;
                            foreach (var ans in listAns)
                            {
                                // continue;
                                var IndexRowAns = index + 4 + indexAns;
                                TableRow rowAns = table.Rows[IndexRowAns];
                                table.ApplyHorizontalMerge(IndexRowAns, 1, 2);
                                table.ApplyHorizontalMerge(IndexRowAns, 4, 6);
                                table.ApplyHorizontalMerge(IndexRowAns, 7, 9);

                                //    //stt
                                Paragraph p0 = rowAns.Cells[3].AddParagraph();
                                TextRange txt0 = p0.AppendText($"{indexAns + 1}");
                                txt0.CharacterFormat.FontSize = 12;
                                txt0.CharacterFormat.TextColor = Color.Black;

                                //    //noi dung
                                Paragraph p1 = rowAns.Cells[4].AddParagraph();
                                TextRange txt1 = p1.AppendText(ans.Content);
                                txt0.CharacterFormat.FontSize = 12;
                                txt0.CharacterFormat.TextColor = Color.Black;

                                //    //hinh anh
                                Paragraph p2 = rowAns.Cells[7].AddParagraph();
                                TextRange txt2 = p2.AppendText(ans.Media?.Path);
                                txt2.CharacterFormat.FontSize = 12;
                                txt2.CharacterFormat.TextColor = Color.Black;
                                //DocPicture PicAns1 = p2.AppendPicture(ImageToByteArray(Image.FromFile(@"H:/Template/example.png")));
                                //PicAns1.Width = 50;
                                //PicAns1.Height = 20;

                                Paragraph _p3 = rowAns.Cells[10].AddParagraph();
                                TextRange txt3 = _p3.AppendText(ans.IsCorrect ? "x" : "");
                                txt3.CharacterFormat.FontSize = 12;
                                txt3.CharacterFormat.TextColor = Color.Black;
                                rowAns.Cells[10].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                                _p3.Format.HorizontalAlignment = HorizontalAlignment.Center;
                                //IndexRowAns++;
                                indexAns++;
                            }
                            if (_lessonPart.Type.Equals("QUIZ3"))
                                index += listAns.Count() < 4 ? 4 + listAns.Count() : listAns.Count();
                            else
                                index += listAns.Count();
                        }
                        #endregion
                    }
                    #endregion
                    Paragraph p3 = s.AddParagraph();
                    TextRange TR3 = p3.AppendText("\n");
                }
                //Create a new paragraph
                //Lưu ý
                Paragraph paragraph = s.AddParagraph();
                TextRange TR4 = paragraph.AppendText("\nLưu ý: Câu hỏi sẽ có số thứ tự; các dòng ngay sau câu hỏi là câu trả lời của câu hỏi \nLiên kết hình ảnh/media có dạng http://... hoặc https://...");
                TR4.CharacterFormat.FontSize = 12;
                TR4.CharacterFormat.TextColor = Color.Red;

                //Save
                doc.SaveToStream(stream, FileFormat.Docx);
                toArray = stream.ToArray();
            };
            string wordName = $"QuizTemplateWithWord.docx";
            return File(toArray, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", wordName);
            //}
            //catch (Exception ex)
            //{
            //    return Content(ex.Message);
            //}
        }

        public async Task<JsonResult> ImportQuestionWithWord(string basis = "", string ParentID = "")
        {
            Boolean Status = false;
            try
            {
                var form = HttpContext.Request.Form;
                if (form == null || form.Files == null || form.Files.Count <= 0)
                    return new JsonResult(new Dictionary<string, object> { { "Error", "Chưa chọn file" } });

                var file = form.Files[0];
                var dirPath = "Upload\\Quiz";

                if (!Directory.Exists(Path.Combine(_env.WebRootPath, dirPath)))
                    Directory.CreateDirectory(Path.Combine(_env.WebRootPath, dirPath));
                var filePath = Path.Combine(_env.WebRootPath, dirPath + "\\" + DateTime.Now.ToString("ddMMyyyyhhmmss") + file.FileName);

                //var full_item = new LessonPartViewModel()
                //{
                //    Questions = new List<QuestionViewModel>(),
                //    ParentID = ParentID
                //};

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
                            String description = table.Rows[3].Cells[1].Paragraphs[0].Text?.ToString().Trim();
                            String point = table.Rows[5].Cells[1].Paragraphs[0].Text?.ToString().Trim();
                            item.Title = title;
                            item.Created = DateTime.UtcNow;
                            item.Updated = DateTime.UtcNow;
                            item.Description = description;
                            Int32.TryParse(point, out Int32 _point);
                            item.Point = _point;//mac dinh la 0 diem
                            item.ParentID = ParentID;

                            var maxItem = _lessonPartService.CreateQuery()
                            .Find(o => o.ParentID == item.ParentID)
                            .SortByDescending(o => o.Order).FirstOrDefault();
                            item.Order = maxItem != null ? maxItem.Order + 1 : 0;

                            //check type
                            var type = "";
                            var typeRow = table.Rows[2];
                            for (int indexCell = 1; indexCell < typeRow.Cells.Count; indexCell++)
                            {
                                var txtCell = typeRow.Cells[indexCell].Paragraphs[0].Text.ToString().ToUpper();
                                if (txtCell.Contains("x") || txtCell.Contains("X"))
                                {
                                    type = table.Rows[1].Cells[indexCell].Paragraphs[0].Text.ToString().ToUpper();
                                    item.Type = type;
                                }
                            }

                            switch (type)
                            {
                                case "VĂN BẢN":
                                    item.Type = "TEXT";
                                    return new JsonResult(new Dictionary<string, object>
                                        {
                                            //{ "Data", full_item },
                                            {"Msg", "Dạng văn bản đang trong quá trình hoàn thiện, vui lòng quay lại sau." },
                                            {"Stt",false }
                                        });
                                //break;
                                case "AUDIO":
                                    item.Type = "AUDIO";
                                    return new JsonResult(new Dictionary<string, object>
                                        {
                                            //{ "Data", full_item },
                                            {"Msg", "Dạng Audio đang trong quá trình hoàn thiện, vui lòng quay lại sau." },
                                            {"Stt",false }
                                        });
                                //break;
                                case "VIDEO":
                                    item.Type = "VIDEO";
                                    return new JsonResult(new Dictionary<string, object>
                                        {
                                            //{ "Data", full_item },
                                            {"Msg", "Dạng Video đang trong quá trình hoàn thiện, vui lòng quay lại sau." },
                                            {"Stt",false }
                                        });
                                //break;
                                case "HÌNH ẢNH":
                                    item.Type = "IMG";
                                    Msg += await GetContentIMG(table, type, basis, item);
                                    Status = true;
                                    break;
                                case "TỪ VỰNG":
                                    item.Type = "VOCAB";
                                    return new JsonResult(new Dictionary<string, object>
                                        {
                                            //{ "Data", full_item },
                                            {"Msg", "Dạng Từ vựng đang trong quá trình hoàn thiện, vui lòng quay lại sau." },
                                            {"Stt",false }
                                        });
                                //break;
                                case "QUIZ1":
                                    Msg += await GetContentQUIZ(table, type, basis, item);
                                    Status = true;
                                    break;
                                case "QUIZ2":
                                    item.Description = "";
                                    Msg += await GetContentQuiz2andVocab(table, type, basis, item);
                                    Status = true;
                                    break;
                                case "QUIZ3":
                                    Msg += await GetContentQUIZ(table, type, basis, item);
                                    Status = true;
                                    break;
                                case "QUIZ4":
                                    Msg += await GetContentQUIZ(table, type, basis, item);
                                    Status = true;
                                    break;
                                case "ESSAY":
                                    item.Type = "ESSAY";
                                    return new JsonResult(new Dictionary<string, object>
                                        {
                                            //{ "Data", full_item },
                                            {"Msg", "Dạng Essay đang trong quá trình hoàn thiện, vui lòng quay lại sau." },
                                            {"Stt",false }
                                        });
                                //break;
                                default:
                                    break;
                            }
                        }
                    }
                    System.IO.File.Delete(filePath);
                    return new JsonResult(new Dictionary<string, object>
                    {
                        //{ "Data", full_item },
                        {"Msg", Msg },
                        {"Stt",Status }
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
                List<QuestionViewModel> Quiz = new List<QuestionViewModel>();
                Int32 pos = -1;

                var totalRows = table.Rows.Count;
                for (int indexRow = 8; indexRow < totalRows; indexRow++)
                {
                    var contentRow = table.Rows[indexRow];
                    String contentCell0 = contentRow.Cells[0].Paragraphs[0].Text?.ToString().Trim().ToLower();
                    String contentCell1 = contentRow.Cells[1].Paragraphs[0].Text?.ToString().Trim().ToLower();
                    String contentCell2 = contentRow.Cells[2].Paragraphs[0].Text?.ToString().Trim().ToLower().ToLower();
                    if (contentCell0 != "" || !String.IsNullOrEmpty(contentCell0))
                    {
                        pos++;
                        //var question = new QuestionViewModel();
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
                            Media = new Media(),
                            Content = contentRow.Cells[2].Paragraphs[0].Text?.ToString().Trim()
                        };
                        Quiz.Add(question);
                    }

                    else if ((contentCell0 == "" && contentCell1 == "" || String.IsNullOrEmpty(contentCell0) && String.IsNullOrEmpty(contentCell1)) && (contentCell2 != "" || !String.IsNullOrEmpty(contentCell2))) //answer
                    {
                        if (contentCell2.Equals("stt")) continue;
                        else
                        {
                            var answer = new LessonPartAnswerEntity
                            {
                                CourseID = item.CourseID,
                                CreateUser = createUser,
                                Created = DateTime.UtcNow,
                                Updated = DateTime.UtcNow,
                                Media = new Media()
                            };
                            String contentCell3 = contentRow.Cells[3].Paragraphs[0].Text?.ToString().Trim();
                            String contentCell4 = contentRow.Cells[4].Paragraphs[0].Text?.ToString().Trim().ToLower();
                            String contentCell5 = contentRow.Cells[5].Paragraphs[0].Text?.ToString().Trim().ToLower();

                            answer.Content = contentCell3;
                            answer.IsCorrect = contentCell5.Equals("x");

                            if (String.IsNullOrEmpty(contentCell4))
                            {
                                var contentFile = GetContentFile(contentRow.Cells[4].Paragraphs, basis);
                                if (contentFile.Count > 0)
                                {
                                    answer.Media.Created = DateTime.Now;
                                    answer.Media.Name = contentFile["FileName"];
                                    answer.Media.Path = contentFile["FilePath"];
                                    answer.Media.Extension = "image/png";
                                }
                            }
                            else if (contentCell4.StartsWith("https") || contentCell4.StartsWith("http"))
                            {
                                answer.Media.Created = DateTime.Now;
                                answer.Media.Name = contentCell4;
                                answer.Media.Path = contentCell4;
                                answer.Media.Extension = "image/png";
                            }

                            //_lessonPartAnswerService.CreateOrUpdate(answer);
                            Quiz[pos].Answers.Add(answer);
                        }
                    }
                    else if ((contentCell0 != null || contentCell0 != "") && (contentCell1 != null || contentCell1 != ""))//Question
                    {
                        if (contentCell1.Contains("tiêu đề"))
                        {
                            Quiz[pos].Content = contentRow.Cells[2].Paragraphs[0].Text?.ToString().Trim();
                        }
                        else if (contentCell1.Contains("mô tả"))
                        {
                            Quiz[pos].Description = contentRow.Cells[2].Paragraphs[0].Text?.ToString().Trim();
                        }
                        else if (contentCell1.Contains("file đính kèm"))
                        {
                            if (String.IsNullOrEmpty(contentCell2))
                            {
                                var contentFileQuiz = GetContentFile(contentRow.Cells[2].Paragraphs, basis);
                                if (contentFileQuiz.Count > 0)
                                {
                                    Quiz[pos].Media.Created = DateTime.Now;
                                    Quiz[pos].Media.Name = contentFileQuiz["FileName"];
                                    Quiz[pos].Media.Path = contentFileQuiz["FilePath"];
                                    Quiz[pos].Media.Extension = "image/png";
                                }
                            }
                            else if (contentCell2.StartsWith("http") || contentCell2.StartsWith("https"))
                            {
                                Quiz[pos].Media.Created = DateTime.Now;
                                Quiz[pos].Media.Name = contentCell2;
                                Quiz[pos].Media.Path = contentCell2;
                                Quiz[pos].Media.Extension = "image/png";
                            }
                        }
                        else if (contentCell1.Contains("giải thích đáp án"))
                        {

                        }

                    }
                    else
                    {

                    }
                }

                item.Questions = Quiz;
                await CreateOrUpdateLessonPart(basis, item);

                return "Type QUIZ 134 is OK";
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
                        var contentFileImage = GetContentFile(contentRow.Cells[1].Paragraphs, basis);
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
                return "Type IMG is OK";
            }
            catch (Exception ex)
            {
                return "Type IMG has error: " + ex.Message;
            }
        }

        private async Task<String> GetContentQuiz2andVocab(Table table, String type, String basis, LessonPartViewModel item, String createUser = null)
        {
            try
            {
                if (type.Equals("VOCAB"))
                {
                    return $"{type} is OK";
                }
                else
                {
                    var totalRows = table.Rows.Count;
                    for (int indexRow = 0; indexRow < totalRows; indexRow++)
                    {
                        var contentRow = table.Rows[indexRow];
                        String contentCell0 = contentRow.Cells[0].Paragraphs[0].Text?.ToString().Trim().ToLower();

                        if (contentCell0.Equals("nội dung"))
                        {
                            Int32 countPara = contentRow.Cells[1].Paragraphs.Count;
                            var txt = contentRow.Cells[1].Paragraphs[0].Text?.ToString().Trim().ToLower();
                            for (int indexPara = 0; indexPara < countPara; indexPara++)
                            {
                                List<LessonPartAnswerEntity> listAns = new List<LessonPartAnswerEntity>();
                                String contentQUIZ2 = contentRow.Cells[1].Paragraphs[indexPara].Text?.ToString().Trim();
                                while (contentQUIZ2.Contains("{{") && contentQUIZ2.Contains("}}"))
                                {
                                    Int32 startIndex = contentQUIZ2.IndexOf("{{") + 2;
                                    Int32 lenghtStr = contentQUIZ2.IndexOf("}}") - startIndex;
                                    String str = contentQUIZ2.Substring(startIndex, lenghtStr);
                                    var answer = new LessonPartAnswerEntity
                                    {
                                        CourseID = item.CourseID,
                                        CreateUser = createUser,
                                        Created = DateTime.UtcNow,
                                        Updated = DateTime.UtcNow,
                                        Media = new Media(),
                                        Content = str,
                                        IsCorrect = true,
                                    };
                                    listAns.Add(answer);
                                    if (contentQUIZ2.Contains("{{") && contentQUIZ2.Contains("}}"))
                                    {
                                        contentQUIZ2 = contentQUIZ2.Replace($"{{{{{str}}}}}", str);
                                    }
                                }

                                //String formatDes = "";

                                foreach (var ans in listAns)
                                {
                                    String str = ans.Content;
                                    String formatDes = $"<fillquiz contenteditable='false' readonly='readonly' title=''><input ans='{str}' class='fillquiz' contenteditable='false' dsp='{str}' placeholder='{str}' readonly='readonly' type='text' value='{str}' /></fillquiz>";
                                    contentQUIZ2 = contentQUIZ2.Replace(str, formatDes);
                                }
                                item.Description += $"<p>{contentQUIZ2}</p>\n\n";
                            }
                        }
                        else continue;
                    }

                    var newdescription = "";
                    item.Questions = ExtractFillQuestionList(item, createUser, out newdescription);
                    item.Description = newdescription;
                    await CreateOrUpdateLessonPart(basis, item);

                    return $"{type} is OK";
                }
            }
            catch (Exception ex)
            {
                return $"{type} has error {ex.Message}";
            }
        }

        //private async Task<String> GetContentDescription(String html)
        //{
        //    String _html = html.Replace("<p>", "").Replace("</p>", "").Replace("<fillquiz><input class='fillquiz' type='text'></fillquiz>", "");
        //    if(html.Contains("<img height"))
        //    {
        //        String srcImg=
        //    }

        //    return "";
        //}

        private Dictionary<String, String> GetContentFile(Spire.Doc.Collections.ParagraphCollection documentObject, String basis)
        {
            Dictionary<String, String> dataResponse = new Dictionary<String, String>();
            foreach (DocumentObject docObject in documentObject[0].ChildObjects)
            {
                if (docObject.DocumentObjectType == DocumentObjectType.Picture)
                {
                    DocPicture picture = docObject as DocPicture;
                    string fileName = string.Format($"Image{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}.png");
                    var pathImage = SaveImageByByteArray(picture.ImageBytes, fileName, basis);
                    dataResponse.Add("FileName", fileName);
                    dataResponse.Add("FilePath", pathImage);
                }
            }
            return dataResponse;
        }

        private String SaveImageByByteArray(byte[] byteArrayIn, string fileName, string center = "")
        {
            try
            {
                MemoryStream ms = new MemoryStream(byteArrayIn);
                Image returnImage = Image.FromStream(ms);
                //return returnImage;

                var size = returnImage.Size;//get size image
                var IMG = (Image)(new Bitmap(returnImage, (size.Width <= 800 && size.Height <= 800 ? size : new Size(800, 800)))); //resize image
                var folder = center == "" ? "eduso" : center + $"/IMG/{DateTime.UtcNow.ToString("yyyyMMdd")}";
                string uploads = Path.Combine(RootPath, folder);
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
                using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                {
                    IMG.Save(fileStream, ImageFormat.Jpeg);
                }
                return $"{"/Files"}/{folder}/{fileName}";
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //private async Task CreateOrUpdateLessonPart(string basis, string parentID, string type, string title, List<LessonPartViewModel> lessonPartViewModels)
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
                        //case "ESSAY":
                        //    _questionService.CreateQuery().DeleteMany(t => t.ParentID == lessonpart.ID);
                        //    var question = new LessonPartQuestionEntity
                        //    {
                        //        CourseID = lessonpart.CourseID,
                        //        Content = "",
                        //        Description = item.Questions == null ? "" : item.Questions[0].Description,
                        //        ParentID = lessonpart.ID,
                        //        CreateUser = createduser,
                        //        Point = lessonpart.Point,
                        //        Created = lessonpart.Created,
                        //    };
                        //    _questionService.Save(question);
                        //    isPractice = true;
                        //    break;
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

        private string GetText(Paragraph para)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (DocumentObject obj in para.ChildObjects)
            {
                if (obj is OfficeMath)
                {
                    stringBuilder.Append((obj as OfficeMath).ToMathMLCode());
                }
                else if (obj is TextRange)
                {
                    stringBuilder.Append((obj as TextRange).Text);
                }
            }
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
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
                            Content = validAns
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

        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }
        #endregion

        #region FIX RESOURCES

        [HttpGet]
        public JsonResult FixResourcesV2()
        {
            var courses = _service.GetAll().ToList();
            foreach (var course in courses)
            {
                if (course.TeacherID == null)
                {
                    course.TeacherID = course.CreateUser;
                    _service.Save(course);
                }
            }

            //var chapters = _chapterService.GetAll().ToList();
            //foreach (var chapter in chapters)
            //{
            //    chapter.TotalLessons = 0;
            //    _chapterService.Save(chapter);
            //}
            //var courses = _service.GetAll().ToList();
            //foreach (var course in courses)
            //{
            //    course.TotalLessons = 0;
            //    _service.Save(course);
            //}
            //var subjects = _classSubjectService.GetAll().ToList();
            //foreach (var sbj in subjects)
            //{
            //    sbj.TotalLessons = 0;
            //    _classSubjectService.Save(sbj);
            //}
            //var allclass = _classService.GetAll().ToList();
            //foreach (var @class in allclass)
            //{
            //    @class.TotalLessons = 0;
            //    _classService.Save(@class);
            //}
            //var alllessons = _lessonService.GetAll().ToList();
            //foreach (var lesson in alllessons)
            //{
            //    var course = _service.GetItemByID(lesson.CourseID);
            //    if (course == null)
            //    {
            //        _lessonService.Remove(lesson.ID);
            //    }
            //}
            //int count = 0;
            //foreach (var chapter in chapters)
            //{
            //    count++;
            //    var course = _service.GetItemByID(chapter.CourseID);
            //    if (course == null)//not valid
            //        _chapterService.Remove(chapter.ID);
            //    chapter.TotalLessons = _lessonService.CountChapterLesson(chapter.ID);
            //    if (chapter.TotalLessons > 0)
            //    {
            //        _chapterService.Save(chapter);
            //        if (string.IsNullOrEmpty(chapter.ParentID) || chapter.ParentID == "0")
            //        {
            //            _ = _service.IncreaseLessonCount(chapter.CourseID, chapter.TotalLessons);
            //        }
            //        else
            //        {
            //            _ = _chapterService.IncreaseLessonCount(chapter.ParentID, chapter.TotalLessons);
            //        }
            //    }
            //}
            ////update classsubject && class
            //var classes = _classService.GetAll().ToList();
            //foreach (var @class in classes)
            //{
            //    @class.TotalLessons = 0;
            //    var sbjs = _classSubjectService.GetByClassID(@class.ID);
            //    foreach (var sbj in sbjs)
            //    {
            //        var course = _service.GetItemByID(sbj.CourseID);
            //        if (course != null)
            //        {
            //            sbj.TotalLessons = course.TotalLessons;
            //            @class.TotalLessons += course.TotalLessons;
            //            _classSubjectService.Save(sbj);
            //        }
            //    }
            //    _classService.Save(@class);
            //}

            //var _newchapters = _newchapterService.GetAll().ToList();
            //foreach (var chapter in _newchapters)
            //{
            //    var orgChap = _chapterService.GetItemByID(chapter.OriginID);
            //    if (orgChap != null)
            //    {
            //        chapter.TotalLessons = orgChap.TotalLessons;
            //        _newchapterService.CreateQuery().ReplaceOne(t => t.ID == chapter.ID, chapter);
            //    }
            //}
            return new JsonResult("Update OK");
        }

        //public async Task<JsonResult> FixResourcesV3()
        //{
        //    Console.WriteLine("start");
        //    var start = DateTime.UtcNow;
        //    //Run once
        //    //_chapterService.Collection.DeleteMany(t => true);
        //    //_lessonService.Collection.DeleteMany(t => true);

        //    //copy to coursechapter collection
        //    //Fixed
        //    var unfixchapters = _newchapterService.GetAll().ToEnumerable();
        //    foreach (var unfix in unfixchapters)
        //    {
        //        var sourcechapter = _chapterMappingRev.AutoOrtherType(unfix, new CourseChapterEntity());
        //        _chapterService.Collection.InsertOne(sourcechapter);
        //    }

        //    //clear old chapter
        //    _ = _newchapterService.RemoveAllAsync();


        //    //copy to courselesson collection
        //    //Fixed
        //    var unfixlessons = _newlessonService.GetAll().ToEnumerable();
        //    foreach (var unfix in unfixlessons)
        //    {
        //        var sourcelesson = _lessonMappingRev.AutoOrtherType(unfix, new CourseLessonEntity());
        //        _lessonService.Collection.InsertOne(sourcelesson);
        //    }
        //    //clear old lesson
        //    _ = _newlessonService.RemoveAllAsync();

        //    //clone chapter
        //    //var courses = _service.Collection.Find(t=> t.ID == "5e6524b7fd6d8e01304cd66e").ToList();
        //    var courses = _service.GetAll().ToList();
        //    foreach (var course in courses)
        //    {
        //        var classsubjects = _classSubjectService.GetByCourseID(course.ID);

        //        if (classsubjects != null && classsubjects.Count > 0)
        //        {
        //            foreach (var subject in classsubjects)
        //            {
        //                _ = FixClassSubject(subject, course.ID);
        //            }
        //        }
        //    }
        //    Console.WriteLine("Complete All subject : " + (start - DateTime.UtcNow).TotalSeconds);

        //    var classes = _classService.GetAll().ToList();
        //    foreach (var @class in classes)
        //    {
        //        @class.TotalLessons = await _classProgressService.RefreshTotalLessonForClass(@class.ID);
        //        _classService.Save(@class);
        //    }
        //    Console.WriteLine("Complete All : " + (start - DateTime.UtcNow).TotalSeconds);
        //    return new JsonResult("Update done");
        //}

        //private async Task FixClassSubject(ClassSubjectEntity subject, string CourseID)
        //{
        //    var counter = await FixChapter(subject, null, CourseID);
        //    subject.TotalExams = counter.Exam;
        //    subject.TotalLessons = counter.Lesson;
        //    _classSubjectService.Save(subject);
        //    await _classSubjectProgressService.CreateQuery().UpdateManyAsync(t => t.ClassSubjectID == subject.ID,
        //            Builders<ClassSubjectProgressEntity>.Update.Set(t => t.TotalLessons, subject.TotalLessons)
        //        );
        //}

        //private async Task<Counter> FixChapter(ClassSubjectEntity subject, CourseChapterEntity rootchapter, string courseID)
        //{
        //    var rootid = "0";
        //    var newid = "0";
        //    var counter = new Counter { Exam = 0, Lesson = 0 };
        //    if (rootchapter != null)
        //    {
        //        //chapter
        //        ChapterEntity newchapter = _chapterMapping.AutoOrtherType(rootchapter, new ChapterEntity());

        //        newchapter.OriginID = rootchapter.ID;
        //        newchapter.ClassID = subject.ClassID;
        //        newchapter.ClassSubjectID = subject.ID;
        //        newchapter.TotalLessons = 0;
        //        newchapter.TotalExams = 0;
        //        newchapter.ID = "";
        //        await _newchapterService.CreateQuery().InsertOneAsync(newchapter);

        //        //chapter progress
        //        await _chapterProgressService.CreateQuery()
        //             .UpdateManyAsync(t => t.ClassSubjectID == subject.ID && t.ChapterID == newchapter.OriginID,
        //             Builders<ChapterProgressEntity>.Update.Set(t => t.ChapterID, newchapter.ID).Set(t => t.ParentID, rootchapter.ParentID));
        //        rootid = rootchapter.ID;
        //        newid = newchapter.ID;
        //    }

        //    var lessons = _lessonService.Collection.Find(t => t.ChapterID == rootid && t.CourseID == courseID).ToList();
        //    if (lessons != null && lessons.Count() > 0)
        //    {
        //        foreach (var rootlesson in lessons)
        //        {
        //            //lesson
        //            LessonEntity newlesson = _lessonMapping.AutoOrtherType(rootlesson, new LessonEntity());
        //            newlesson.OriginID = rootlesson.ID;
        //            newlesson.ChapterID = newid;
        //            newlesson.ClassID = subject.ClassID;
        //            newlesson.ClassSubjectID = subject.ID;
        //            if (newlesson.TemplateType == LESSON_TEMPLATE.EXAM)
        //                counter.Exam++;
        //            else
        //                counter.Lesson++;
        //            newlesson.ID = "";

        //            await _newlessonService.CreateQuery().InsertOneAsync(newlesson);
        //            //lesson part
        //            await _cloneLessonPartService.CreateQuery()
        //                .UpdateManyAsync(t => t.ClassSubjectID == subject.ID && t.ParentID == newlesson.OriginID,
        //                Builders<CloneLessonPartEntity>.Update.Set(t => t.ParentID, newlesson.ID));

        //            //lesson progress
        //            await _lessonProgressService.CreateQuery()
        //                .UpdateManyAsync(t => t.ClassSubjectID == subject.ID && t.LessonID == newlesson.OriginID,
        //                Builders<LessonProgressEntity>.Update.Set(t => t.LessonID, newlesson.ID).Set(t => t.ChapterID, newid));
        //            //exam
        //            await _examService.CreateQuery()
        //                .UpdateManyAsync(t => t.ClassSubjectID == subject.ID && t.LessonID == newlesson.OriginID,
        //                Builders<ExamEntity>.Update.Set(t => t.LessonID, newlesson.ID));
        //            //schedule
        //            await _lessonScheduleService.CreateQuery()
        //                .UpdateManyAsync(t => t.LessonID == newlesson.OriginID && t.ClassSubjectID == subject.ID,
        //                Builders<LessonScheduleEntity>.Update.Set(t => t.LessonID, newlesson.ID));
        //        }
        //    }

        //    var subchaps = _chapterService.GetSubChapters(courseID, rootid);
        //    if (subchaps != null && subchaps.Count > 0)
        //    {
        //        foreach (var subchap in subchaps)
        //        {
        //            subchap.ParentID = newid;
        //            var subCounter = await FixChapter(subject, subchap, courseID);
        //            counter.Exam += subCounter.Exam;
        //            counter.Lesson += subCounter.Lesson;
        //        }
        //    }

        //    if (newid != "0")
        //    {
        //        await _newchapterService.Collection.UpdateManyAsync(t => t.ID == newid,
        //            Builders<ChapterEntity>.Update.Set(t => t.TotalLessons, counter.Lesson).Set(t => t.TotalExams, counter.Exam));
        //    }
        //    return counter;
        //}

        //public async Task<JsonResult> FixResourcesV4()
        //{
        //    Console.WriteLine("start");
        //    var parts = _cloneLessonPartService.GetAll().ToList();
        //    foreach (var part in parts)
        //    {
        //        await _cloneLessonPartQuestionService.Collection.UpdateManyAsync(t => t.ParentID == part.ID, Builders<CloneLessonPartQuestionEntity>.Update.Set(t => t.LessonID, part.ParentID));
        //    }
        //    return new JsonResult("Update done");
        //}

        //public async Task<JsonResult> FixLessonCounter()
        //{
        //    var classes = _classService.GetAll().ToList();
        //    foreach (var cclass in classes)
        //    {
        //        cclass.TotalLessons = _newlessonService.CountClassLesson(cclass.ID);
        //        _classService.Save(cclass);
        //    }
        //    var subjects = _classSubjectService.GetAll().ToList();
        //    foreach (var subject in subjects)
        //    {
        //        subject.TotalLessons = _newlessonService.CountClassSubjectLesson(subject.ID);
        //        _classSubjectService.Save(subject);


        //        await FixChapterCounter(subject.ID, "0");

        //    }
        //    return new JsonResult("Update done");
        //}

        //public async Task<JsonResult> FixProgress()
        //{
        //    Console.WriteLine("start");
        //    var classes = _classService.GetAll().ToList();
        //    //_ = _examService.RemoveAllAsync();
        //    //_ = _examDetailService.RemoveAllAsync();



        //    foreach (var @class in classes)
        //    {
        //        var sbjs = _classSubjectService.GetByClassID(@class.ID);
        //        var classLessons = _newlessonService.CountClassLesson(@class.ID);

        //        foreach (var sbj in sbjs)
        //        {
        //            var sbjprgs = _classSubjectProgressService.CreateQuery().Find(t => t.ClassSubjectID == sbj.ID).ToList();
        //            var totalLessons = _newlessonService.CountClassSubjectLesson(sbj.ID);

        //            foreach (var sbjprg in sbjprgs)
        //            {
        //                var student = _studentService.GetItemByID(sbjprg.StudentID);
        //                if (student.JoinedClasses == null || !student.JoinedClasses.Contains(sbjprg.ClassID))
        //                {
        //                    _classProgressService.Remove(sbjprg.ID);
        //                    continue;
        //                }

        //                sbjprg.TotalLessons = totalLessons;
        //                sbjprg.Completed = (int)await _lessonProgressService.CreateQuery().CountDocumentsAsync(t => t.ClassSubjectID == sbj.ID && t.StudentID == sbjprg.StudentID);
        //                _classSubjectProgressService.Save(sbjprg);
        //            }
        //        }


        //        var progresses = _classProgressService.CreateQuery().Find(t => t.ClassID == @class.ID).ToList();
        //        foreach (var progress in progresses)
        //        {

        //            var student = _studentService.GetItemByID(progress.StudentID);
        //            if (student.JoinedClasses == null || !student.JoinedClasses.Contains(progress.ClassID))
        //            {
        //                _classProgressService.Remove(progress.ID);
        //                continue;
        //            }
        //            //var exams = _examService.CreateQuery().Find(t => t.ClassID == progress.ID && t.StudentID == progress.StudentID && t.Status).SortByDescending(t=> t.Updated);

        //            //var count = 0;
        //            //var ids = new List<string>();
        //            //foreach(var exam in exams)
        //            //progress.ExamDone = sbjqry.ToList().GroupBy(t=> t.LessonID, )
        //            //progress.TotalPoint = sbjqry.ToList().Sum(t => t.Point);
        //            //if (progress.ExamDone > 0)
        //            //{
        //            //    progress.AvgPoint = progress.TotalPoint / progress.ExamDone;
        //            //}
        //            //else
        //            //    progress.AvgPoint = 0;
        //            progress.TotalLessons = classLessons;
        //            progress.Completed = (int)await _lessonProgressService.CreateQuery().CountDocumentsAsync(t => t.ClassID == @class.ID && t.StudentID == progress.StudentID);
        //            _classProgressService.Save(progress);
        //        }
        //    }

        //    _ = _lessonProgressService.CreateQuery().UpdateManyAsync(t => true, Builders<LessonProgressEntity>.Update
        //        .Set(t => t.AvgPoint, 0).Set(t => t.PointChange, 0).Set(t => t.LastPoint, 0).Set(t => t.MaxPoint, 0).Set(t => t.MinPoint, 0)
        //        .Set(t => t.Tried, 0));
        //    _ = _chapterProgressService.CreateQuery().UpdateManyAsync(t => true, Builders<ChapterProgressEntity>.Update
        //        .Set(t => t.AvgPoint, 0).Set(t => t.ExamDone, 0).Set(t => t.TotalPoint, 0));
        //    _ = _classSubjectProgressService.CreateQuery().UpdateManyAsync(t => true, Builders<ClassSubjectProgressEntity>.Update
        //        .Set(t => t.AvgPoint, 0).Set(t => t.ExamDone, 0).Set(t => t.TotalPoint, 0));
        //    _ = _classProgressService.CreateQuery().UpdateManyAsync(t => true, Builders<ClassProgressEntity>.Update
        //        .Set(t => t.AvgPoint, 0).Set(t => t.ExamDone, 0).Set(t => t.TotalPoint, 0));

        //    var exams = _examService.GetAll().SortBy(t => t.StudentID).ThenBy(t => t.LessonID).ThenBy(t => t.Number).ToList();
        //    foreach (var exam in exams)
        //    {
        //        double point = 0;
        //        var lesson = _newlessonService.GetItemByID(exam.LessonID);
        //        var student = _studentService.GetItemByID(exam.StudentID);
        //        if (lesson == null || student == null || !student.JoinedClasses.Contains(exam.ClassID))
        //            _examService.Remove(exam.ID);
        //        else
        //        {
        //            //if(exam.ClassID == "5e4165a2fce9522790ccf65d" && exam.StudentID == "5d838a00d5d1bf27e4410c06")
        //            _ = _examService.Complete(exam, lesson, out point);
        //        }
        //    }

        //    return new JsonResult("Update done");
        //}

        //private async Task<long> FixChapterCounter(string ClassSubjectID, string ParentID)
        //{
        //    var subchaps = _newchapterService.GetSubChapters(ClassSubjectID, ParentID);
        //    ChapterEntity parentChap = null;
        //    if (ParentID != "0")
        //    {
        //        parentChap = _newchapterService.GetItemByID(ParentID);
        //        parentChap.TotalLessons = _newlessonService.CountChapterLesson(ParentID);
        //    }
        //    if (subchaps != null && subchaps.Count > 0)
        //        foreach (var chap in subchaps)
        //        {
        //            var counter = await FixChapterCounter(ClassSubjectID, chap.ID);
        //            if (parentChap != null)
        //                parentChap.TotalLessons += counter;
        //        }
        //    if (ParentID != "0")
        //    {
        //        _newchapterService.Save(parentChap);
        //        return parentChap.TotalLessons;
        //    }
        //    else
        //        return 0;

        //}

        //#endregion
        #endregion

        #region Filter Special Characters
        private string FilterSpecialCharacters(string str)
        {
            return str.Replace("‘", "'")
                .Replace("’", "'")
                .Replace("“", "\"")
                .Replace("”", "\"")
                .Replace("&quot;", "\"");
        }
        #endregion
    }

    public class Counter
    {
        public int Exam { get; set; }
        public int Lesson { get; set; }
    }
}
