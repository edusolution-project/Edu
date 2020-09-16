using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Drawing;

namespace BaseCustomerMVC.Controllers.Student
{
    public class ReferenceController : StudentController
    {
        private readonly TeacherService _teacherService;
        private readonly StudentService _studentService;
        private readonly ClassService _classService;
        private readonly FileProcess _fileProcess;
        private readonly SubjectService _subjectService;
        private readonly GradeService _gradeService;
        private readonly ReferenceService _referenceService;
        private readonly CenterService _centerService;
        private readonly CourseService _courseService;
        private readonly IHostingEnvironment _env;
        private string host;
        private string staticPath;

        public ReferenceController(
            StudentService studentService,
            TeacherService teacherService,
            ClassService classService,
            FileProcess fileProcess,
            SubjectService subjectService,
            GradeService gradeService,
            IHostingEnvironment env,
            IConfiguration iConfig,
            ReferenceService referenceService,
            CenterService centerService,
            CourseService courseService
            )
        {
            _studentService = studentService;
            _teacherService = teacherService;
            _classService = classService;
            _referenceService = referenceService;
            _fileProcess = fileProcess;
            _subjectService = subjectService;
            _gradeService = gradeService;
            _env = env;
            host = iConfig.GetValue<string>("SysConfig:Domain");
            staticPath = iConfig.GetValue<string>("SysConfig:StaticPath");
            _centerService = centerService;
            _courseService = courseService;
        }

        public IActionResult Index(DefaultModel model, int old = 0)
        {
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            var classIds = _studentService.GetItemByID(UserID).JoinedClasses;
            var myClasses = new List<ClassEntity>();
            if (classIds != null && classIds.Count > 0)
            {
                myClasses = _classService.CreateQuery()
                .Find(t => classIds.Contains(t.ID))
                .SortByDescending(t => t.IsActive).ThenByDescending(t => t.StartDate)
                .ToList();
            }
            var subjects = _subjectService.GetAll().ToList();
            var grades = _gradeService.GetAll().ToList();
            ViewBag.Grades = grades;
            ViewBag.Subjects = subjects;
            ViewBag.AllClass = myClasses;
            ViewBag.User = UserID;
            if (old == 1)
                return View("index_o");
            return View();
        }

        public JsonResult GetList(ReferenceEntity entity, DefaultModel defaultModel, string TeacherID, string SubjectID, string GradeID, string basis, Boolean isInteractive = false)
        {
            if (string.IsNullOrEmpty(basis))
            {
                return Json("Không tìm thấy cơ sở");
            }

            if (entity != null)
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var center = _centerService.GetItemByCode(basis);
                var filter = new List<FilterDefinition<ReferenceEntity>>();
                if (isInteractive)
                {
                    var _filter = new List<FilterDefinition<CourseEntity>> { Builders<CourseEntity>.Filter.Where(o => o.IsActive && o.IsPublic && o.TargetCenters.Contains(center.ID)) };
                    if (!string.IsNullOrEmpty(defaultModel.SearchText))
                    {
                        _filter.Add(Builders<CourseEntity>.Filter.Text("\"" + defaultModel.SearchText + "\""));
                    }

                    var result = _courseService.CreateQuery().Find(Builders<CourseEntity>.Filter.And(_filter));
                    defaultModel.TotalRecord = result.CountDocuments();

                    var returnData = result.Skip(defaultModel.PageSize * defaultModel.PageIndex).Limit(defaultModel.PageSize).ToList();
                    return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", returnData },
                        { "Model", defaultModel }
                    });
                }
                else
                {
                    switch (entity.Range)
                    {
                        //case REF_RANGE.TEACHER:
                        //    filter.Add(filterTeacher);
                        //    break;
                        case REF_RANGE.CLASS:
                            if (string.IsNullOrEmpty(entity.Target))
                                return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", new List<ReferenceEntity>() },
                                { "Model", defaultModel }
                            });
                            var tIDs = _classService.GetItemByID(entity.Target).Members.Select(t => t.TeacherID);
                            filter.Add(Builders<ReferenceEntity>.Filter.Where(o =>
                                (o.Range == REF_RANGE.CLASS && o.Target == entity.Target) ||
                                (o.Range == REF_RANGE.TEACHER && tIDs.Contains(o.Target))
                            ));
                            break;
                        case REF_RANGE.ALL:
                            filter.Add(Builders<ReferenceEntity>.Filter.Where(o => (o.Range == REF_RANGE.ALL)));
                            break;
                        default:
                            var classIds = _studentService.GetItemByID(UserID).JoinedClasses;
                            var teacherIDs = new List<string>();
                            if (classIds != null && classIds.Count > 0)
                            {
                                foreach (var classid in classIds)
                                    teacherIDs.AddRange(_classService.GetItemByID(classid).Members.Select(t => t.TeacherID));
                                teacherIDs = teacherIDs.Distinct().ToList();
                            }
                            filter.Add(
                                Builders<ReferenceEntity>.Filter.Where(o =>
                                (o.Range == REF_RANGE.CLASS && classIds.Contains(o.Target)) ||
                                (o.Range == REF_RANGE.TEACHER && teacherIDs.Contains(o.Target)) ||
                                (o.Range == REF_RANGE.ALL)
                            ));
                            break;
                    }
                    if (!string.IsNullOrEmpty(SubjectID))
                    {
                        filter.Add(Builders<ReferenceEntity>.Filter.Eq(t => t.SubjectID, SubjectID));
                    }
                    if (!string.IsNullOrEmpty(GradeID))
                    {
                        filter.Add(Builders<ReferenceEntity>.Filter.Eq(t => t.GradeID, GradeID));
                    }
                    if (!string.IsNullOrEmpty(defaultModel.SearchText))
                    {
                        filter.Add(Builders<ReferenceEntity>.Filter.Text("\"" + defaultModel.SearchText + "\""));
                    }
                    var result = _referenceService.CreateQuery().Find(Builders<ReferenceEntity>.Filter.And(filter));
                    defaultModel.TotalRecord = result.CountDocuments();
                    var returnData = result.Skip(defaultModel.PageSize * defaultModel.PageIndex).Limit(defaultModel.PageSize).ToList();
                    return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", returnData },
                        { "Model", defaultModel }
                    });

                }
            }
            return null;
        }

        public JsonResult GetClassList(ReferenceEntity entity, DefaultModel defaultModel)
        {
            if (entity != null)
            {
                var currentClass = _classService.GetItemByID(entity.Target);
                if (currentClass == null) return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", null},
                    { "Error", "Không có thông tin lớp" }
                });
                var teacherIDs = currentClass.Members.Select(t => t.TeacherID);
                var filter = new List<FilterDefinition<ReferenceEntity>>();
                filter.Add(Builders<ReferenceEntity>.Filter.Where(o =>
                    (o.Range == REF_RANGE.ALL) ||
                    (o.Range == REF_RANGE.TEACHER && teacherIDs.Contains(o.Target)) ||
                    (o.Range == REF_RANGE.CLASS && o.Target == entity.Target))
                );
                if (!string.IsNullOrEmpty(defaultModel.SearchText))
                {
                    filter.Add(Builders<ReferenceEntity>.Filter.Text("\"" + defaultModel.SearchText + "\""));
                }
                var result = _referenceService.CreateQuery().Find(Builders<ReferenceEntity>.Filter.And(filter));
                defaultModel.TotalRecord = result.CountDocuments();
                var returnData = result.Skip(defaultModel.PageSize * defaultModel.PageIndex).Limit(defaultModel.PageSize).ToList();
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", returnData },
                    { "Model", defaultModel }
                });
            }
            return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", null},
                });
        }

        public IActionResult Download(string ID)
        {
            if (!string.IsNullOrEmpty(ID))
            {
                var item = _referenceService.GetItemByID(ID);
                if (item == null)
                    return BadRequest();
                _ = _referenceService.IncDownload(ID, 1);

                //TODO: create download log
                if (item.Media != null)
                {
                    try
                    {
                        var filePath = Path.Combine(string.IsNullOrEmpty(staticPath) ? _env.WebRootPath : staticPath, item.Media.Path.TrimStart('/').Replace("/", "\\"));
                        var stream = System.IO.File.OpenRead(filePath);
                        return File(stream, "application/octet-stream", item.Media.Name);
                    }
                    catch
                    {
                        return NotFound();
                    }
                }
            }
            return BadRequest();
        }

        public IActionResult OpenLink(string ID)
        {
            if (!string.IsNullOrEmpty(ID))
            {
                var item = _referenceService.GetItemByID(ID);
                if (item == null)
                    return BadRequest();
                _ = _referenceService.IncLink(ID, 1);
                //TODO: create view log
                var url = item.Link;
                if (!string.IsNullOrEmpty(url))
                {
                    if (!url.ToLower().StartsWith("http://") && !url.ToLower().StartsWith("https://"))
                        url = "http://" + host + url;
                    return Redirect(url);
                }
            }
            return BadRequest();
        }

        public JsonResult View(string ID)
        {
            if (!string.IsNullOrEmpty(ID))
                _ = _referenceService.IncView(ID, 1);
            return Json("OK");
        }
    }
}
