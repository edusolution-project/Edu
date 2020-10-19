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
using RestSharp;
using System.Data;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
//Word
using System.Drawing;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using Spire.Doc.Fields.OMath;

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
                //filter.Add(Builders<CourseEntity>.Filter.Text("\"" + model.SearchText + "\""));
                filter.Add(Builders<CourseEntity>.Filter.Text(model.SearchText));


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
                            SkillName = _skillService.GetItemByID(o.SkillID)?.Name,
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
                    item.Created = DateTime.Now;
                    item.CreateUser = UserID;
                    item.IsAdmin = true;
                    item.IsActive = false;
                    item.Updated = DateTime.Now;
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

                        var filename = DateTime.Now.ToUniversalTime().ToString("yyyyMMddhhmmss") + Path.GetExtension(file.FileName);
                        item.Image = await _fileProcess.SaveMediaAsync(file, filename, "BOOKCOVER");
                    }


                    _service.Save(item);
                }
                else
                {
                    olditem.Updated = DateTime.Now;
                    olditem.Description = item.Description;
                    olditem.SubjectID = item.SubjectID;
                    olditem.GradeID = item.GradeID;
                    olditem.SkillID = item.SkillID;
                    olditem.Name = item.Name;
                    olditem.TeacherID = item.TeacherID;
                    olditem.IsPublic = item.IsPublic;
                    olditem.PublicWStudent = item.PublicWStudent;
                    //if (item.TargetCenters != null && item.TargetCenters[0] != null)
                    //{
                    //    var listCenters = item.TargetCenters[0].Split(',');
                    //    item.TargetCenters = listCenters.ToList();
                    //}
                    olditem.TargetCenters = item.TargetCenters;
                    var files = HttpContext.Request.Form != null && HttpContext.Request.Form.Files.Count > 0 ? HttpContext.Request.Form.Files : null;
                    if (files != null && files.Count > 0)
                    {
                        var file = files[0];

                        var filename = DateTime.Now.ToUniversalTime().ToString("yyyyMMddhhmmss") + Path.GetExtension(file.FileName);
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
                    new_lesson.Created = DateTime.Now;
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

                    var filename = currentCourse.ID + "_" + DateTime.Now.ToUniversalTime().ToString("yyyyMMddhhmmss") + Path.GetExtension(file.FileName);
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
            new_course.Created = DateTime.Now;
            new_course.Updated = DateTime.Now;
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
                    item.Created = DateTime.Now;
                    item.IsAdmin = true;
                    item.IsActive = false;
                    item.Updated = DateTime.Now;
                    item.Order = int.MaxValue - 1;
                    _chapterService.Save(item);
                    ChangeChapterPosition(item, int.MaxValue);//move chapter to bottom of new parent chap
                }
                else
                {
                    item.Updated = DateTime.Now;
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
                    else
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
                            new_lesson.Created = DateTime.Now;
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
                            clone_chap.Created = DateTime.Now;
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
                            //clone_chap.Created = DateTime.Now;
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
                    new_lesson.Created = DateTime.Now;
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
                new_chapter.Created = DateTime.Now;
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
                    item.Created = DateTime.Now;
                    item.CreateUser = UserID;
                    item.IsAdmin = true;
                    item.IsActive = false;
                    item.IsParentCourse = item.ChapterID.Equals("0");
                    item.Updated = DateTime.Now;
                    item.Order = 0;
                    _lessonService.CreateQuery().InsertOne(item);

                    ChangeLessonPosition(item, Int32.MaxValue);//move lesson to bottom of parent

                    //update total lesson to parent chapter
                    if (!string.IsNullOrEmpty(item.ChapterID) && item.ChapterID != "0")
                        _ = _courseHelper.IncreaseCourseChapterCounter(item.ChapterID, 1, item.TemplateType == LESSON_TEMPLATE.EXAM ? 1 : 0, 0);
                    else
                        _ = _courseHelper.IncreaseCourseCounter(item.CourseID, 1, item.TemplateType == LESSON_TEMPLATE.EXAM ? 1 : 0, 0);
                }
                else
                {
                    item.Updated = DateTime.Now;
                    var newOrder = item.Order - 1;
                    item.Order = data.Order;

                    //update counter if type change
                    if (item.TemplateType != data.TemplateType)
                    {
                        var examInc = 0;
                        var pracInc = 0;
                        if (IsLessonHasQuiz(item.ID)) pracInc = 1;
                        if (item.TemplateType == LESSON_TEMPLATE.LECTURE) // EXAM => LECTURE
                        {
                            examInc = -1;
                            item.IsPractice = pracInc == 1;
                        }
                        else
                        {
                            examInc = 1;
                            item.IsPractice = false;
                            pracInc = pracInc == 1 ? -1 : 0;
                        }
                        if (!string.IsNullOrEmpty(item.ChapterID) && item.ChapterID != "0")
                            _ = _courseHelper.IncreaseCourseChapterCounter(item.ChapterID, 0, examInc, pracInc);
                        else
                            _ = _courseHelper.IncreaseCourseCounter(item.CourseID, 0, examInc, pracInc);
                    }

                    _lessonService.CreateQuery().ReplaceOne(o => o.ID == item.ID, item);

                    if (item.Order != newOrder)//change Position
                    {
                        ChangeLessonPosition(item, newOrder);
                    }
                }

                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", item },
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
            new_lesson.Created = DateTime.Now;
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
                    Updated = DateTime.Now,
                    Created = DateTime.Now,
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
                    Updated = DateTime.Now,
                    Created = DateTime.Now,
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
                    Updated = DateTime.Now,
                    Created = DateTime.Now,
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
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
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
                        Created = DateTime.Now,
                        Updated = DateTime.Now,
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
                        Created = DateTime.Now,
                        Updated = DateTime.Now
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
                    Created = DateTime.Now,
                    Updated = DateTime.Now
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
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
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
                    Updated = DateTime.Now,
                    Created = DateTime.Now,
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
                    Updated = DateTime.Now,
                    Created = DateTime.Now,
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
                    Updated = DateTime.Now,
                    Created = DateTime.Now,
                    CourseID = item.CourseID
                };
                if (_item.Media != null && _item.Media.Path != null)
                    if (!_item.Media.Path.StartsWith("http://"))
                        _item.Media.Path = "http://" + _publisherHost + _item.Media.Path;
                await CloneLessonAnswer(_item);
            }
        }
        #endregion

        #region Excel
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
            var filePath = Path.Combine(_env.WebRootPath, dirPath + "\\" + DateTime.Now.ToString("ddMMyyyyhhmmss") + file.FileName);
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

        /// <summary>
        /// Using Spire.Doc v8.9.6
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        #region Import with Word
        public IActionResult ExportQuestionTemplateWithWord(DefaultModel model)
        {
            try
            {
                byte[] toArray = null;
                //Create Table
                using (var stream = new MemoryStream())
                {
                    Document doc = new Document();
                    Section s = doc.AddSection();
                    Table table = s.AddTable(true);

                    //Create Header and Data
                    String[] Header = { "STT", "Nội dung", "Hình ảnh", "Đúng/Sai", "Giải thích" };
                    String[][] data = {
                                  new String[]{"1"  , "Câu hỏi 1"                           , "Hình ảnh hoặc liên kết ảnh" , ""        ,"Giải thích đáp án"},
                                  new String[]{""   , "Nội dung câu trả lời câu hỏi 1 (1)"  , "Hình ảnh hoặc liên kết ảnh" , "True"    ,""},
                                  new String[]{""   , "Nội dung câu trả lời câu hỏi 1 (2)"  , "Hình ảnh hoặc liên kết ảnh" , "False"   ,""},
                                  new String[]{""   , "Nội dung câu trả lời câu hỏi 1 (3)"  , "Hình ảnh hoặc liên kết ảnh" , "False"   ,""},
                                  new String[]{"2"  , "Câu hỏi 2"                           , "Hình ảnh hoặc liên kết ảnh" , ""        ,"Giải thích đáp án"},
                                  new String[]{""   , "Nội dung câu trả lời câu hỏi 2 (1)"  , "Hình ảnh hoặc liên kết ảnh" , "True"    ,""},
                                  new String[]{""   , "Nội dung câu trả lời câu hỏi 2 (2)"  , "Hình ảnh hoặc liên kết ảnh" , "False"   ,""},
                                  new String[]{""   , "Nội dung câu trả lời câu hỏi 2 (3)"  , "Hình ảnh hoặc liên kết ảnh" , "False"   ,""},
                              };
                    //Add Cells
                    table.ResetCells(data.Length + 1, Header.Length);

                    //Header Row
                    TableRow FRow = table.Rows[0];
                    FRow.IsHeader = true;
                    //Row Height
                    FRow.Height = 23;
                    //Header Format
                    FRow.RowFormat.BackColor = Color.AliceBlue;
                    for (int i = 0; i < Header.Length; i++)
                    {
                        //Cell Alignment
                        Paragraph p = FRow.Cells[i].AddParagraph();
                        FRow.Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        p.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        //Data Format
                        TextRange TR = p.AppendText(Header[i]);
                        TR.CharacterFormat.FontName = "Calibri";
                        TR.CharacterFormat.FontSize = 14;
                        TR.CharacterFormat.TextColor = Color.Black;
                        TR.CharacterFormat.Bold = true;
                    }

                    //Data Row
                    for (int r = 0; r < data.Length; r++)
                    {
                        TableRow DataRow = table.Rows[r + 1];

                        //Row Height
                        DataRow.Height = 20;

                        //C Represents Column.
                        for (int c = 0; c < data[r].Length; c++)
                        {
                            //Cell Alignment
                            DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            //Fill Data in Rows
                            Paragraph p2 = DataRow.Cells[c].AddParagraph();
                            TextRange TR2 = p2.AppendText(data[r][c]);
                            //Format Cells
                            p2.Format.HorizontalAlignment = HorizontalAlignment.Left;
                            TR2.CharacterFormat.FontName = "Calibri";
                            TR2.CharacterFormat.FontSize = 12;
                            TR2.CharacterFormat.TextColor = Color.Black;
                        }
                    }

                    //Create a new paragraph
                    //Lưu ý
                    Paragraph paragraph = doc.AddSection().AddParagraph();
                    TextRange TR3=paragraph.AppendText("Lưu ý: Câu hỏi sẽ có số thứ tự; các dòng ngay sau câu hỏi là câu trả lời của câu hỏi \nLiên kết hình ảnh/media có dạng http://... hoặc https://...");
                    TR3.CharacterFormat.FontSize = 12;
                    TR3.CharacterFormat.TextColor = Color.Red;

                    //Save and Launch
                    //doc.SaveToFile("QuizTemplate.docx", FileFormat.Docx2013);
                    //System.Diagnostics.Process.Start("WordTable.docx");
                    doc.SaveToStream(stream, FileFormat.Docx);
                    toArray = stream.ToArray();
                };
                string wordName = $"QuizTemplateWithWord.docx";
                return File(toArray, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", wordName);
            }
            catch (Exception ex)
            {
                return new JsonResult(new Dictionary<string, object> { { "Error", ex.Message } });
            }
        }
        public async Task<JsonResult> ImportQuestionWithWord()
        {
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

                var full_item = new LessonPartViewModel()
                {
                    Questions = new List<QuestionViewModel>()
                };

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                    stream.Close();
                    using (var readStream = new FileStream(filePath, FileMode.Open))
                    {
                        //Create a Document instance and load the word document
                        Document document = new Document(readStream);

                        //Get the first session
                        var sw = document.Sections[0];

                        //Get the first table in the textbox
                        Table table = sw.Body.Tables[0] as Table;

                        //Loop through the paragraphs of the table and extract text to a .txt file
                        //foreach (TableRow row in table.Rows)

                        var pos = -1;

                        for (var i = 1; i < table.Rows.Count; i++)
                        {
                            var row = table.Rows[i];
                            var cells = row.Cells;
                            if (cells[0].Paragraphs[0].Text.ToString()!="") { //question
                                pos++;
                                var question = new QuestionViewModel
                                {
                                    Content = cells[1].Paragraphs[0].Text,//cau hoi
                                    Answers = new List<LessonPartAnswerEntity>() { },//danh sach cau tra loi
                                    Description = cells[4].Paragraphs[0].Text.ToString() == "" ? "" : cells[4].Paragraphs[0].Text.ToString()//giai thich dap an
                                };
                                full_item.Questions.Add(question);
                            }
                            else //answer
                            {
                                var answer = new LessonPartAnswerEntity
                                {
                                    Content = cells[1].Paragraphs[0].Text.ToString().Trim(),
                                    IsCorrect = cells[3].Paragraphs[0].Text.ToString().Trim().ToLower() == "true",
                                };
                                if (cells[2].Paragraphs[0].Text.ToString().StartsWith("http") || cells[2].Paragraphs[0].Text.ToString().StartsWith("https"))
                                {
                                    var media = cells[2].Paragraphs[0].Text.ToString();
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
                            //foreach (TableCell cell in row.Cells)
                            //{
                            //    var question = new QuestionViewModel
                            //    {
                            //        Content = workSheet.Cells[i, contentCol].Value.ToString(),//cau hoi
                            //        Answers = new List<LessonPartAnswerEntity>() { },//danh sach cau tra loi
                            //        Description = workSheet.Cells[i, 5].Value == null ? "" : workSheet.Cells[i, 5].Value.ToString()//giai thich dap an
                            //    };
                            //    //var index = 0;
                            //    foreach (Paragraph paragraph in cell.Paragraphs)
                            //    {
                            //       var str = paragraph.Text;
                            //       var test = GetText(paragraph);
                            //        //sb.AppendLine(paragraph.Text);

                            //        //Get Each Document Object of Paragraph Items
                            //        //foreach (DocumentObject docObject in paragraph.ChildObjects)
                            //        //{
                            //        //    //If Type of Document Object is Picture, Extract.
                            //        //    if (docObject.DocumentObjectType == DocumentObjectType.Picture)
                            //        //    {
                            //        //        DocPicture picture = docObject as DocPicture;
                            //        //        //Name Image
                            //        //        String imageName = String.Format(@"images\Image-{0}.png", index);
                            //        //        //picture
                            //        //        //Save Image
                            //        //        //picture.Image.Save(imageName, System.Drawing.Imaging.ImageFormat.Png);
                            //        //        index++;
                            //        //    }
                            //        //}
                            //    }
                            //}
                        }
                    }
                    //File.WriteAllText("text.txt", sb.ToString());
                    System.IO.File.Delete(filePath);
                    return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", full_item },
                        {"Error", null }
                    });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new Dictionary<string, object> { { "Error", ex.Message } });
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
        //    var start = DateTime.Now;
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
        //    Console.WriteLine("Complete All subject : " + (start - DateTime.Now).TotalSeconds);

        //    var classes = _classService.GetAll().ToList();
        //    foreach (var @class in classes)
        //    {
        //        @class.TotalLessons = await _classProgressService.RefreshTotalLessonForClass(@class.ID);
        //        _classService.Save(@class);
        //    }
        //    Console.WriteLine("Complete All : " + (start - DateTime.Now).TotalSeconds);
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
    }

    public class Counter
    {
        public int Exam { get; set; }
        public int Lesson { get; set; }
    }
}
