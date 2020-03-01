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

namespace BaseCustomerMVC.Controllers.Teacher
{
    [BaseAccess.Attribule.AccessCtrl("Bài giảng chung", "teacher")]
    public class CurriculumController : TeacherController
    {
        private readonly CourseService _service;
        //private readonly ProgramService _programService;
        private readonly SubjectService _subjectService;
        private readonly ChapterService _chapterService;
        private readonly GradeService _gradeService;
        private readonly LessonService _lessonService;
        private readonly SkillService _skillService;
        private readonly LessonPartService _lessonPartService;
        private readonly CloneLessonPartService _cloneLessonPartService;
        private readonly LessonPartAnswerService _lessonPartAnswerService;
        private readonly LessonPartQuestionService _lessonPartQuestionService;
        private readonly LessonExtendService _lessonExtendService;
        private readonly TeacherService _teacherService;
        private readonly ClassService _classService;
        private readonly ClassSubjectService _classSubjectService;

        private readonly ModCourseService _modservice;
        private readonly ModProgramService _modprogramService;
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

        private readonly MappingEntity<CourseEntity, CourseViewModel> _courseViewMapping;

        public CurriculumController(CourseService service,
                 ProgramService programService,
                 SubjectService subjectService,
                 ChapterService chapterService,
                 GradeService gradeService,
                 LessonService lessonService,
                 SkillService skillService,
                 LessonPartService lessonPartService,
                 CloneLessonPartService cloneLessonPartService,
                 LessonPartAnswerService lessonPartAnswerService,
                 LessonPartQuestionService lessonPartQuestionService,
                 LessonExtendService lessonExtendService,
                 TeacherService teacherService,
                 ModCourseService modservice,
                 ClassService classService,
                 ClassSubjectService classSubjectService

                , ModProgramService modprogramService
                , ModSubjectService modsubjectService
                , ModChapterService modchapterService
                , ModGradeService modgradeService
                , ModLessonService modlessonService
                , ModLessonPartService modlessonPartService
                , ModLessonPartAnswerService modlessonPartAnswerService
                , ModLessonPartQuestionService modlessonPartQuestionService
                , ModLessonExtendService modlessonExtendService
                , IHostingEnvironment evn
                , FileProcess fileProcess
                 )
        {
            _service = service;
            //_programService = programService;
            _subjectService = subjectService;
            _chapterService = chapterService;
            _gradeService = gradeService;
            _skillService = skillService;
            _lessonService = lessonService;
            _lessonPartService = lessonPartService;
            _cloneLessonPartService = cloneLessonPartService;
            _lessonPartAnswerService = lessonPartAnswerService;
            _lessonPartQuestionService = lessonPartQuestionService;
            _lessonExtendService = lessonExtendService;
            _teacherService = teacherService;
            _modservice = modservice;
            _classService = classService;
            _classSubjectService = classSubjectService;
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
            _fileProcess = new FileProcess(evn);
        }

        public IActionResult Index(DefaultModel model, int old = 0)
        {
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            var teacher = _teacherService.CreateQuery().Find(t => t.ID == UserID).SingleOrDefault();//: new TeacherEntity();

            //if (!User.IsInRole("head-teacher"))
            //    return Redirect("/");

            if (teacher != null && teacher.Subjects != null)
            {
                var subject = _subjectService.CreateQuery().Find(t => teacher.Subjects.Contains(t.ID)).ToList();
                var grade = _gradeService.CreateQuery().Find(t => teacher.Subjects.Contains(t.SubjectID)).ToList();
                var skills = _skillService.GetList();
                ViewBag.Grades = grade;
                ViewBag.Subjects = subject;
                ViewBag.Skills = skills;
            }


            var modsubject = _modsubjectService.GetAll().ToList();
            var modgrade = _modgradeService.GetAll().ToList();

            ViewBag.ModGrade = modgrade;
            ViewBag.ModSubject = modsubject;
            ViewBag.User = UserID;

            ViewBag.RoleCode = User.Claims.GetClaimByType(ClaimTypes.Role).Value;
            ViewBag.Model = model;
            if (old == 1)
                return View("Index_o");
            return View();

        }

        public IActionResult Detail(string ID)
        {
            if (string.IsNullOrEmpty("ID"))
                return RedirectToAction("Index");

            //if (!User.IsInRole("head-teacher"))
            //    return Redirect("/");

            var data = _service.GetItemByID(ID);
            if (data == null)
                return RedirectToAction("Index");

            ViewBag.Data = data;
            ViewBag.Title = data.Name;
            var UserID = User.Claims.GetClaimByType("UserID").Value;

            var chapters = _chapterService.CreateQuery().Find(t => t.CourseID == ID).ToList();

            ViewBag.Chapter = chapters;
            ViewBag.User = UserID;
            ViewBag.Course = data;
            ViewBag.Subject = _subjectService.GetItemByID(data.SubjectID);
            ViewBag.Grade = _gradeService.GetItemByID(data.GradeID);

            //ViewBag.RoleCode = "head-teacher";

            return View();
        }

        public IActionResult Modules(string ID)
        {
            if (string.IsNullOrEmpty("ID"))
                return RedirectToAction("Index");

            var data = _service.GetItemByID(ID);
            if (data == null)
                return RedirectToAction("Index");

            ViewBag.Data = data;
            ViewBag.Title = data.Name;
            var UserID = User.Claims.GetClaimByType("UserID").Value;

            var chapters = _chapterService.CreateQuery().Find(t => t.CourseID == ID).ToList();

            ViewBag.Chapter = chapters;
            ViewBag.User = UserID;
            ViewBag.Course = data;

            return View();
        }

        public IActionResult Assignments(string ID)
        {
            if (string.IsNullOrEmpty("ID"))
                return RedirectToAction("Index");

            var data = _service.GetItemByID(ID);
            if (data == null)
                return RedirectToAction("Index");

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
        public IActionResult Lesson(DefaultModel model, string CourseID, int frameview = 0)
        {
            //if (!User.IsInRole("head-teacher"))
            //    return Redirect("/");

            if (CourseID == null)
                return RedirectToAction("Index");
            var currentCourse = _service.GetItemByID(CourseID);
            if (currentCourse == null)
                return RedirectToAction("Index");
            var Data = _lessonService.GetItemByID(model.ID);
            if (Data == null)
                return RedirectToAction("Index");

            ViewBag.Course = currentCourse;
            ViewBag.Data = Data;
            if (frameview == 1)
                return View("LessonFrame");
            //ViewBag.RoleCode = "head-teacher";
            return View();
        }

        #region Course

        [HttpPost]
        public JsonResult GetList(DefaultModel model, string SubjectID = "", string GradeID = "")
        {
            var filter = new List<FilterDefinition<CourseEntity>>();

            var UserID = User.Claims.GetClaimByType("UserID").Value;
            var teacher = _teacherService.GetItemByID(UserID);
            if (teacher == null)
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
            if (User.Claims.GetClaimByType(ClaimTypes.Role).Value == "teacher")
                filter.Add(Builders<CourseEntity>.Filter.Where(o => o.CreateUser == UserID));

            if (!string.IsNullOrEmpty(model.SearchText))
                filter.Add(Builders<CourseEntity>.Filter.Text("\"" + model.SearchText + "\""));


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

                response = new Dictionary<string, object>
                {
                    { "Data", DataResponse.ToList().Select(o =>

                        _courseViewMapping.AutoOrtherType(o, new CourseViewModel(){
                            SkillName = _skillService.GetItemByID(o.SkillID)?.Name,
                            GradeName = _gradeService.GetItemByID(o.GradeID)?.Name,
                            SubjectName = _subjectService.GetItemByID(o.SubjectID)?.Name,
                            TeacherName = _teacherService.GetItemByID(o.CreateUser)?.FullName
                        })).ToList()
                    },
                    { "Model", model }
                };
            }
            return new JsonResult(response);
        }


        [HttpPost]
        public JsonResult GetActiveList(DefaultModel model, string SubjectID = "", string GradeID = "", bool cp = false)
        {
            var filter = new List<FilterDefinition<CourseEntity>>();

            var UserID = User.Claims.GetClaimByType("UserID").Value;

            if (!string.IsNullOrEmpty(SubjectID))
            {
                filter.Add(Builders<CourseEntity>.Filter.Where(o => o.SubjectID == SubjectID));
            }
            if (!string.IsNullOrEmpty(GradeID))
            {
                filter.Add(Builders<CourseEntity>.Filter.Where(o => o.GradeID == GradeID));
            }

            if (User.Claims.GetClaimByType(ClaimTypes.Role).Value == "teacher")
                filter.Add(Builders<CourseEntity>.Filter.Where(o => o.CreateUser == UserID));

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
                        TeacherName = _teacherService.GetItemByID(o.CreateUser)?.FullName
                    })).ToList()
                },
                { "Model", model }
            };
            return new JsonResult(response);
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

        [HttpPost]
        public JsonResult CreateOrUpdate(CourseEntity item)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var olditem = _service.CreateQuery().Find(o => o.ID == item.ID).SingleOrDefault();
                if (olditem == null)
                {
                    item.Created = DateTime.Now;
                    item.CreateUser = UserID;
                    item.IsAdmin = true;
                    item.IsActive = false;
                    item.Updated = DateTime.Now;
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
                    _service.Save(olditem);
                    //update class subject using this course, temporary use
                    _classSubjectService.UpdateCourseSkill(olditem.ID, olditem.SkillID);
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
        public async Task<JsonResult> Remove(DefaultModel model)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                if (model.ArrID == null)
                {
                    return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", "Nothing to remove" },
                                {"Error", null }
                            });
                }
                var ID = model.ArrID;
                var course = _service.CreateQuery().Find(o => o.ID == ID).SingleOrDefault();
                if (course == null)
                {
                    return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", "Already removed" },
                                {"Error", null }
                            });
                }
                if (course.CreateUser != UserID)
                    return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", null },
                                {"Error", "Permisson Error" }
                            });
                var learningClass = _classService.CreateQuery().CountDocuments(o => o.CourseID == course.ID);
                if (learningClass > 0)
                    return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", null },
                                {"Error", "Course in use" }
                            });

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
                                { "Data", "Remove OK" },
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
        public async Task<JsonResult> SaveInfo(CourseEntity entity)
        //public async Task<JsonResult> SaveInfo(CourseEntity entity)
        {

            //if (string.IsNullOrEmpty(entity.ID))
            //{
            //    new JsonResult(
            //        new Dictionary<string, object>
            //        {
            //            { "Error", "Không có thông tin lớp"}
            //        });
            //}

            var currentCourse = _service.GetItemByID(entity.ID);
            if (currentCourse == null)
            {
                new JsonResult(
                    new Dictionary<string, object>
                    {
                        { "Error", "Curriculum not found"}
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
                    currentCourse.Image = await _fileProcess.SaveMediaAsync(file, filename);
                }
                //currentClass.Description = entity.Description ?? "";
                //currentClass.Syllabus = entity.Syllabus ?? "";
                //currentClass.Modules = entity.Modules ?? "";
                //currentClass.LearningOutcomes = entity.LearningOutcomes ?? "";
                //currentClass.References = entity.References ?? "";
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

        #endregion Course

        #region Chapter
        [HttpPost]
        public JsonResult CreateOrUpdateChapter(ChapterEntity item)
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
                    item.CreateUser = UserID;
                    item.IsAdmin = true;
                    item.IsActive = false;
                    item.Updated = DateTime.Now;
                    item.Order = int.MaxValue - 1;
                    _chapterService.CreateQuery().InsertOne(item);
                    ChangeChapterPosition(item, int.MaxValue);//reorder
                }
                else
                {
                    item.Updated = DateTime.Now;
                    var newOrder = item.Order - 1;
                    item.Order = data.Order;
                    item.Created = data.Created;
                    item.CreateUser = data.CreateUser;
                    _chapterService.CreateQuery().ReplaceOne(o => o.ID == item.ID, item);
                    if (item.ParentID != data.ParentID)
                    {
                        if (item.TotalLessons > 0)
                        {
                            //decrease old parent chapter total lesson
                            if (string.IsNullOrEmpty(data.ParentID) || data.ParentID == "0")
                            {
                                _ = _service.IncreaseLessonCount(item.CourseID, 0 - data.TotalLessons);
                            }
                            //increase new parent chapter total lesson
                            if (string.IsNullOrEmpty(item.ParentID) || item.ParentID == "0")
                            {
                                _ = _chapterService.IncreaseLessonCount(item.ParentID, data.TotalLessons);
                            }
                        }
                        ChangeChapterPosition(item, int.MaxValue);
                    }
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
        public async Task<JsonResult> RemoveChapter(DefaultModel model)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var ID = model.ArrID;
                var chapter = _chapterService.CreateQuery().Find(o => o.ID == ID).SingleOrDefault();
                if (chapter == null)
                {
                    return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", "Already removed" },
                                {"Error", null }
                            });
                }
                if (chapter.CreateUser != UserID)
                    return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", null },
                                {"Error", "Permisson Error" }
                            });

                await RemoveChapter(chapter);
                return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", "Remove OK" },
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

        private async Task RemoveChapter(ChapterEntity chap)
        {
            _lessonService.CreateQuery().DeleteMany(o => o.ChapterID == chap.ID);
            var subchapters = _chapterService.CreateQuery().Find(o => o.ParentID == chap.ID).ToList();
            if (subchapters != null && subchapters.Count > 0)
                foreach (var chapter in subchapters)
                    await RemoveChapter(chapter);
            ChangeChapterPosition(chap, int.MaxValue);
            await _chapterService.RemoveAsync(chap.ID);
        }

        private int ChangeChapterPosition(ChapterEntity item, int pos)
        {
            var parts = new List<ChapterEntity>();
            parts = _chapterService.CreateQuery().Find(o => o.CourseID == item.CourseID && o.ParentID == item.ParentID)
                .SortBy(o => o.Order).ThenBy(o => o.ID).ToList();

            var ids = parts.Select(o => o.ID).ToList();

            var oldPos = ids.IndexOf(item.ID);
            if (oldPos == pos)
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

        [Obsolete]
        [HttpPost]
        public JsonResult GetCourseDetail(DefaultModel model)
        {
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
                { "Chapters",_chapterService.CreateQuery().Find(o => o.CourseID == course.ID).SortBy(o => o.ParentID).ThenBy(o => o.Order).ThenBy(o => o.ID).ToList() } ,
                { "Lessons", _lessonService.CreateQuery().Find(o => o.CourseID == course.ID).SortBy(o => o.ChapterID).ThenBy(o => o.Order).ThenBy(o => o.ID).ToList() }
            };

            var response = new Dictionary<string, object>
            {
                { "Data", courseDetail },
                { "Model", model }
            };
            return new JsonResult(response);
        }

        [HttpPost]
        public async Task<JsonResult> Clone(string CourseID, CourseEntity newcourse)
        {
            var _userCreate = User.Claims.GetClaimByType("UserID").Value;
            var item = _modservice.GetItemByID(CourseID); //publisher
            if (item != null)
            {
                //var grade = _modgradeService.GetItemByID(item.GradeID);
                //var subject = _modsubjectService.GetItemByID(item.SubjectID);
                //var programe = _modprogramService.GetItemByID(item.ProgramID);
                var course = _modservice.GetItemByID(CourseID);
                if (course != null)
                {
                    var chapter_root = _modchapterService.CreateQuery().Find(o => o.CourseID == CourseID && o.ParentID == "0");
                    var lesson_root = _modlessonService.CreateQuery().Find(o => o.CourseID == CourseID && o.ChapterID == "0");

                    var clone_course = new CourseEntity()
                    {
                        OriginID = course.ID,
                        Name = newcourse.Name,
                        Code = course.Code,
                        Description = newcourse.Description,
                        GradeID = newcourse.GradeID,
                        SubjectID = newcourse.SubjectID,
                        CreateUser = _userCreate,
                        SkillID = newcourse.SkillID,
                        Created = DateTime.Now,
                        Updated = DateTime.Now,
                        IsActive = true,
                        IsAdmin = false,
                        Order = course.Order
                    };
                    _service.Collection.InsertOne(clone_course);

                    //if (chapter_root != null && chapter_root.CountDocuments() > 0)
                    //{
                    foreach (var chapter in chapter_root.ToEnumerable())
                    {
                        await CloneChapter(new ChapterEntity()
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
                    //}

                    foreach (var o in lesson_root.ToEnumerable())
                    {
                        await CloneLesson(new LessonEntity()
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
            }
            return new JsonResult("OK");
        }

        private async Task CloneChapter(ChapterEntity item, string _userCreate)
        {
            _chapterService.Collection.InsertOne(item);

            var lessons = _modlessonService.CreateQuery().Find(o => o.ChapterID == item.OriginID);

            foreach (var o in lessons.ToEnumerable())
            {
                await CloneLesson(new LessonEntity()
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
                await CloneChapter(new ChapterEntity()
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

        private async Task CloneLesson(LessonEntity item, string _userCreate)
        {
            if (item.Media != null && item.Media.Path != null)
                if (!item.Media.Path.StartsWith("http://"))
                    item.Media.Path = "http://publisher.edusolution.vn" + item.Media.Path;

            _lessonService.CreateQuery().InsertOne(item);

            var parts = _modlessonPartService.CreateQuery().Find(o => o.ParentID == item.OriginID);
            foreach (var _child in parts.ToEnumerable())
            {
                var _item = new LessonPartEntity()
                {
                    OriginID = _child.ID,
                    Title = _child.Title,
                    Description = _child.Description != null ? _child.Description.Replace("src=\"/", "src=\"http://publisher.edusolution.vn/") : null,
                    IsExam = _child.IsExam,
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
                if (_item.Media != null && _item.Media.Path != null)
                    if (!_item.Media.Path.StartsWith("http://"))
                        _item.Media.Path = "http://publisher.edusolution.vn" + _item.Media.Path;
                await CloneLessonPart(_item, _userCreate);
            }
        }

        private async Task CloneLessonPart(LessonPartEntity item, string _userCreate)
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
                    Description = _child.Description != null ? _child.Description.Replace("src=\"/", "src=\"http://publisher.edusolution.vn/") : null,
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
                        _item.Media.Path = "http://publisher.edusolution.vn" + _item.Media.Path;
                await CloneLessonQuestion(_item, _userCreate);
            }
        }

        private async Task CloneLessonQuestion(LessonPartQuestionEntity item, string _userCreate)
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
                        _item.Media.Path = "http://publisher.edusolution.vn" + _item.Media.Path;
                await CloneLessonAnswer(_item);
            }
        }

        private async Task CloneLessonAnswer(LessonPartAnswerEntity item)
        {
            await _lessonPartAnswerService.Collection.InsertOneAsync(item);
        }

        [HttpGet]
        public JsonResult FixResources()
        {
            var parts = _lessonPartService.GetAll().ToList();
            foreach (var item in parts)
            {
                if (item.Description != null)
                {
                    if (item.Description.IndexOf("src=") > 0)
                        if (item.Description.IndexOf("src=\"/") > 0)
                        {
                            item.Description = item.Description.Replace("src=\"/", "src=\"http://publisher.edusolution.vn/");
                            _lessonPartService.CreateOrUpdate(item);
                        }
                }
            }
            var cloneparts = _cloneLessonPartService.GetAll().ToList();
            foreach (var item in cloneparts)
            {
                if (item.Description != null)
                {
                    if (item.Description.IndexOf("src=") > 0)
                        if (item.Description.IndexOf("src=\"/") > 0)
                        {
                            item.Description = item.Description.Replace("src=\"/", "src=\"http://publisher.edusolution.vn/");
                            _cloneLessonPartService.CreateOrUpdate(item);
                        }
                }
            }
            return new JsonResult("OK");
        }

        [HttpGet]
        public JsonResult FixResourcesV2()
        {
            var chapters = _chapterService.GetAll().ToList();
            foreach (var chapter in chapters)
            {
                chapter.TotalLessons = 0;
                _chapterService.Save(chapter);
            }
            var courses = _service.GetAll().ToList();
            foreach (var course in courses)
            {
                course.TotalLessons = 0;
                _service.Save(course);
            }
            var subjects = _classSubjectService.GetAll().ToList();
            foreach (var sbj in subjects)
            {
                sbj.TotalLessons = 0;
                _classSubjectService.Save(sbj);
            }
            var allclass = _classService.GetAll().ToList();
            foreach (var @class in allclass)
            {
                @class.TotalLessons = 0;
                _classService.Save(@class);
            }
            var alllessons = _lessonService.GetAll().ToList();
            foreach (var lesson in alllessons)
            {
                var course = _service.GetItemByID(lesson.CourseID);
                if (course == null)
                {
                    _lessonService.Remove(lesson.ID);
                }
            }
            int count = 0;
            foreach (var chapter in chapters)
            {
                count++;
                var course = _service.GetItemByID(chapter.CourseID);
                if (course == null)//not valid
                    _chapterService.Remove(chapter.ID);
                chapter.TotalLessons = _lessonService.CountChapterLesson(chapter.ID);
                if (chapter.TotalLessons > 0)
                {
                    _chapterService.Save(chapter);
                    if (string.IsNullOrEmpty(chapter.ParentID) || chapter.ParentID == "0")
                    {
                        _ = _service.IncreaseLessonCount(chapter.CourseID, chapter.TotalLessons);
                    }
                    else
                    {
                        _ = _chapterService.IncreaseLessonCount(chapter.ParentID, chapter.TotalLessons);
                    }
                }
            }
            //update classsubject && class
            var classes = _classService.GetAll().ToList();
            foreach (var @class in classes)
            {
                @class.TotalLessons = 0;
                var sbjs = _classSubjectService.GetByClassID(@class.ID);
                foreach (var sbj in sbjs)
                {
                    var course = _service.GetItemByID(sbj.CourseID);
                    if (course != null)
                    {
                        sbj.TotalLessons = course.TotalLessons;
                        @class.TotalLessons += course.TotalLessons;
                        _classSubjectService.Save(sbj);
                    }
                }
                _classService.Save(@class);
            }
            return new JsonResult("Update " + count + " chapter");
        }
    }
}
