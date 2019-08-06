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

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class CurriculumController : TeacherController
    {
        private readonly CourseService _service;
        private readonly ProgramService _programService;
        private readonly SubjectService _subjectService;
        private readonly ChapterService _chapterService;
        private readonly GradeService _gradeService;
        private readonly LessonService _lessonService;
        private readonly LessonPartService _lessonPartService;
        private readonly LessonPartAnswerService _lessonPartAnswerService;
        private readonly LessonPartQuestionService _lessonPartQuestionService;
        private readonly LessonExtendService _lessonExtendService;
        private readonly TeacherService _teacherService;
        private readonly ClassService _classService;

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

        private readonly MappingEntity<CourseEntity, CourseViewModel> _courseViewMapping;

        public CurriculumController(CourseService service,
                 ProgramService programService,
                 SubjectService subjectService,
                 ChapterService chapterService,
                 GradeService gradeService,
                 LessonService lessonService,
                 LessonPartService lessonPartService,
                 LessonPartAnswerService lessonPartAnswerService,
                 LessonPartQuestionService lessonPartQuestionService,
                 LessonExtendService lessonExtendService,
                 TeacherService teacherService,
                 ModCourseService modservice,
                 ClassService classService

                , ModProgramService modprogramService
                , ModSubjectService modsubjectService
                , ModChapterService modchapterService
                , ModGradeService modgradeService
                , ModLessonService modlessonService
                , ModLessonPartService modlessonPartService
                , ModLessonPartAnswerService modlessonPartAnswerService
                , ModLessonPartQuestionService modlessonPartQuestionService
                , ModLessonExtendService modlessonExtendService)
        {
            _service = service;
            _programService = programService;
            _subjectService = subjectService;
            _chapterService = chapterService;
            _gradeService = gradeService;
            _lessonService = lessonService;
            _lessonPartService = lessonPartService;
            _lessonPartAnswerService = lessonPartAnswerService;
            _lessonPartQuestionService = lessonPartQuestionService;
            _lessonExtendService = lessonExtendService;
            _teacherService = teacherService;
            _modservice = modservice;
            _classService = classService;
            _modprogramService = modprogramService;
            _modsubjectService = modsubjectService;
            _modchapterService = modchapterService;
            _modgradeService = modgradeService;
            _modlessonService = modlessonService;
            _modlessonPartService = modlessonPartService;
            _modlessonPartAnswerService = modlessonPartAnswerService;
            _modlessonPartQuestionService = modlessonPartQuestionService;
            _modlessonExtendService = modlessonExtendService;

            _courseViewMapping = new MappingEntity<CourseEntity, CourseViewModel>();
        }

        public IActionResult Index(DefaultModel model)
        {
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            var teacher = _teacherService.CreateQuery().Find(t => t.ID == UserID).SingleOrDefault();//: new TeacherEntity();

            //if (!User.IsInRole("head-teacher"))
            //    return Redirect("/");

            if (teacher != null && teacher.Subjects != null)
            {
                var subject = _subjectService.CreateQuery().Find(t => teacher.Subjects.Contains(t.ID)).ToList();
                var grade = _gradeService.CreateQuery().Find(t => teacher.Subjects.Contains(t.SubjectID)).ToList();
                ViewBag.Grade = grade;
                ViewBag.Subject = subject;
            }

            var modsubject = _modsubjectService.GetAll().ToList();
            var modgrade = _modgradeService.GetAll().ToList();

            ViewBag.ModGrade = modgrade;
            ViewBag.ModSubject = modsubject;
            ViewBag.User = UserID;

            ViewBag.RoleCode =  User.Claims.GetClaimByType(ClaimTypes.Role).Value;
            ViewBag.Model = model;
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
            ViewBag.Title = "Chi tiết giáo trình - " + data.Name;
            var subject = new List<SubjectEntity>();
            var grade = new List<GradeEntity>();

            var UserID = User.Claims.GetClaimByType("UserID").Value;
            var teacher = _teacherService.CreateQuery().Find(t => t.ID == UserID).SingleOrDefault();


            if (teacher != null && teacher.Subjects != null)
            {
                subject = _subjectService.CreateQuery().Find(t => teacher.Subjects.Contains(t.ID)).ToList();
                grade = _gradeService.CreateQuery().Find(t => teacher.Subjects.Contains(t.SubjectID)).ToList();
            }

            var chapters = _chapterService.CreateQuery().Find(t => t.CourseID == ID).ToList();

            ViewBag.Chapter = chapters;
            ViewBag.Grade = grade;
            ViewBag.Subject = subject;
            ViewBag.User = UserID;
            //ViewBag.RoleCode = "head-teacher";

            return View();
        }

        public IActionResult Lesson(DefaultModel model, string CourseID)
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
            //ViewBag.RoleCode = "head-teacher";
            return View();
        }

        #region Course

        [Obsolete]
        [HttpPost]
        public JsonResult GetList(DefaultModel model, string SubjectID = "", string GradeID = "")
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
            filter.Add(Builders<CourseEntity>.Filter.Where(o => o.CreateUser == UserID));

            var data = filter.Count > 0 ? _service.Collection.Find(Builders<CourseEntity>.Filter.And(filter)) : _service.GetAll();
            model.TotalRecord = data.Count();
            var DataResponse = data == null || data.Count() <= 0 || data.Count() < model.PageSize
                ? data
                : data.Skip((model.PageIndex - 1) * model.PageSize).Limit(model.PageSize);

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

        [Obsolete]
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
            model.TotalRecord = data.Count();
            var DataResponse = data == null || data.Count() <= 0 || data.Count() < model.PageSize
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
                var data = _service.CreateQuery().Find(o => o.ID == item.ID).SingleOrDefault();
                if (data == null)
                {
                    item.Created = DateTime.Now;
                    item.CreateUser = UserID;
                    item.IsAdmin = true;
                    item.IsActive = false;
                    item.Updated = DateTime.Now;
                    _service.CreateQuery().InsertOne(item);
                }
                else
                {
                    item.Updated = DateTime.Now;
                    item.Order = data.Order;
                    item.Created = data.Created;
                    item.CreateUser = data.CreateUser;
                    _service.CreateQuery().ReplaceOne(o => o.ID == item.ID, item);
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
        public JsonResult Remove(DefaultModel model)
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

                _service.Remove(ID);
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
                    var filter = Builders<CourseEntity>.Filter.Where(o => model.ArrID.Split(',').Contains(o.ID) && o.IsActive != true);
                    var update = Builders<CourseEntity>.Update.SetOnInsert("IsActive", true);
                    var publish = _service.Collection.UpdateMany(filter, update, new UpdateOptions() { IsUpsert = true });
                    return new JsonResult(publish);
                }
                else
                {
                    var filter = Builders<CourseEntity>.Filter.Where(o => model.ArrID == o.ID && o.IsActive != true);
                    var update = Builders<CourseEntity>.Update.Set("IsActive", true);
                    var publish = _service.Collection.UpdateMany(filter, update, new UpdateOptions() { IsUpsert = true });
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

                var data = _chapterService.CreateQuery().Find(o => o.ID == item.ID).SingleOrDefault();
                if (data == null)
                {
                    item.Created = DateTime.Now;
                    item.CreateUser = UserID;
                    item.IsAdmin = true;
                    item.IsActive = false;
                    item.Updated = DateTime.Now;
                    var max = _chapterService.CreateQuery()
                             .CountDocuments(o => o.ParentID == item.ParentID);
                    item.Order = (int)max;

                    _chapterService.CreateQuery().InsertOne(item);
                }
                else
                {
                    item.Updated = DateTime.Now;
                    item.Order = data.Order;
                    item.Created = data.Created;
                    item.CreateUser = data.CreateUser;
                    _chapterService.CreateQuery().ReplaceOne(o => o.ID == item.ID, item);
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
        public JsonResult RemoveChapter(DefaultModel model)
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

                RemoveChapter(chapter.ID);

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

        private void RemoveChapter(string chapid)
        {
            _lessonService.CreateQuery().DeleteMany(o => o.ChapterID == chapid);
            var subchapters = _chapterService.CreateQuery().Find(o => o.ParentID == chapid).ToList();
            if (subchapters != null && subchapters.Count > 0)
                foreach (var chapter in subchapters)
                    RemoveChapter(chapter.ID);
            _chapterService.Remove(chapid);
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
        public JsonResult Clone(string CourseID, CourseEntity newcourse)
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
                    var chapter_root = _modchapterService.CreateQuery().Find(o => o.CourseID == CourseID && o.ParentID == "0").ToList();
                    var lesson = _modlessonService.CreateQuery().Find(o => o.CourseID == CourseID).ToList();

                    var clone_course = new CourseEntity()
                    {
                        OriginID = course.ID,
                        Name = newcourse.Name,
                        Code = course.Code,
                        Description = newcourse.Description,
                        GradeID = newcourse.GradeID,
                        SubjectID = newcourse.SubjectID,
                        CreateUser = _userCreate,
                        Created = DateTime.Now,
                        Updated = DateTime.Now,
                        IsActive = true,
                        IsAdmin = false,
                        Order = course.Order
                    };
                    _service.Collection.InsertOne(clone_course);
                    if (chapter_root != null && chapter_root.Count > 0)
                    {
                        var clone_chapter = chapter_root.Select(o => new ChapterEntity()
                        {
                            OriginID = o.ID,
                            Name = o.Name,
                            Code = o.Code,
                            CourseID = clone_course.ID,
                            ParentID = o.ParentID,
                            ParentType = o.ParentType,
                            CreateUser = _userCreate,
                            Created = DateTime.Now,
                            Updated = DateTime.Now,
                            IsActive = true,
                            IsAdmin = false,
                            Order = o.Order
                        }).ToList();
                        for (int i = 0; i < clone_chapter.Count; i++)
                        {
                            CloneChapter(clone_chapter[i]);
                        }
                    }

                    var clone_lesson = lesson.Select(o => new LessonEntity()
                    {
                        Media = o.Media,
                        ChapterID = _chapterService.Collection.Find(x => x.OriginID == o.ChapterID && x.CourseID == clone_course.ID).First() != null ? _chapterService.Collection.Find(x => x.OriginID == o.ChapterID && x.CourseID == clone_course.ID).First().ID : "0",
                        CreateUser = _userCreate,
                        Code = o.Code,
                        OriginID = o.ID,
                        CourseID = clone_course.ID,
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
                    }).ToList();

                    if (clone_lesson != null)
                    {
                        for (int i = 0; i < clone_lesson.Count; i++)
                        {
                            var lessonItem = clone_lesson[i];
                            CloneLesson(lessonItem);
                        }
                    }
                }
            }
            return new JsonResult("OK");
        }

        private void CloneChapter(ChapterEntity item)
        {
            var _userCreate = User.Claims.GetClaimByType("UserID").Value;
            _chapterService.Collection.InsertOne(item);

            var listChild = _modchapterService.Collection.Find(o => o.ParentID == item.OriginID).ToList();
            if (listChild.Count > 0)
            {
                var listChildClone = listChild.Select(o => new ChapterEntity()
                {
                    OriginID = o.ID,
                    Name = o.Name,
                    Code = o.Code,
                    CourseID = item.CourseID,
                    ParentID = item.ID,//Edit by VietPhung 20190701
                    ParentType = o.ParentType,
                    CreateUser = _userCreate,
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    IsActive = true,
                    IsAdmin = false,
                    Order = o.Order
                }).ToList();
                for (int i = 0; i < listChildClone.Count; i++)
                {
                    var _item = listChildClone[i];
                    CloneChapter(_item);
                }
            }

        }

        private void CloneLesson(LessonEntity item)
        {
            var _userCreate = User.Claims.GetClaimByType("UserID").Value;
            if (item.Media != null && item.Media.Path != null)
                if (!item.Media.Path.StartsWith("http://"))
                    item.Media.Path = "http://publisher.edusolution.vn" + item.Media.Path;

            _lessonService.CreateQuery().InsertOne(item);

            var lessonpart = _modlessonPartService.CreateQuery().Find(o => o.ParentID == item.OriginID).ToList();
            if (lessonpart != null)
            {
                for (int i = 0; i < lessonpart.Count; i++)
                {
                    var _child = lessonpart[i];
                    var _item = new LessonPartEntity()
                    {
                        OriginID = _child.ID,
                        Title = _child.Title,
                        Description = _child.Description,
                        IsExam = _child.IsExam,
                        Media = _child.Media,
                        Point = _child.Point,
                        Order = _child.Order,
                        ParentID = item.ID,
                        Timer = _child.Timer,
                        Type = _child.Type,
                        Updated = DateTime.Now,
                        Created = DateTime.Now
                    };
                    if (_item.Media != null && _item.Media.Path != null)
                        if (!_item.Media.Path.StartsWith("http://"))
                            _item.Media.Path = "http://publisher.edusolution.vn" + _item.Media.Path;
                    CloneLessonPart(_item);
                }
            }
        }

        private void CloneLessonPart(LessonPartEntity item)
        {
            var _userCreate = User.Claims.GetClaimByType("UserID").Value;
            _lessonPartService.Collection.InsertOne(item);
            var list = _modlessonPartQuestionService.CreateQuery().Find(o => o.ParentID == item.OriginID).ToList();
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var _child = list[i];
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
                        Created = DateTime.Now
                    };
                    //change Media path
                    if (_item.Media != null && _item.Media.Path != null)
                        if (!_item.Media.Path.StartsWith("http://"))
                            _item.Media.Path = "http://publisher.edusolution.vn" + _item.Media.Path;
                    CloneLessonQuestion(_item);
                }
            }
        }

        private void CloneLessonQuestion(LessonPartQuestionEntity item)
        {
            var _userCreate = User.Claims.GetClaimByType("UserID").Value;
            _lessonPartQuestionService.Collection.InsertOne(item);
            var list = _modlessonPartAnswerService.CreateQuery().Find(o => o.ParentID == item.OriginID).ToList();
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var _child = list[i];
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
                        Created = DateTime.Now
                    };
                    if (_item.Media != null && _item.Media.Path != null)
                        if (!_item.Media.Path.StartsWith("http://"))
                            _item.Media.Path = "http://publisher.edusolution.vn" + _item.Media.Path;
                    CloneLessonAnswer(_item);
                }
            }
        }

        private void CloneLessonAnswer(LessonPartAnswerEntity item)
        {
            _lessonPartAnswerService.Collection.InsertOne(item);
        }
    }
}
