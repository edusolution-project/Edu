using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Core_v2.Globals;
using Core_v2.Interfaces;
using FileManagerCore.Globals;
using FileManagerCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using MongoDB.Driver;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseCustomerMVC.Controllers.Student
{
    public class ExamController : StudentController
    {

        private readonly ExamService _examService;
        private readonly ExamDetailService _examDetailService;
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly LessonService _lessonService;
        private readonly CloneLessonPartService _cloneLessonPartService;
        private readonly CloneLessonPartAnswerService _cloneLessonPartAnswerService;
        private readonly CloneLessonPartQuestionService _cloneLessonPartQuestionService;
        private readonly TeacherService _teacherService;
        private readonly StudentService _studentService;
        private readonly ClassService _classService;
        private readonly LearningHistoryService _learningHistoryService;
        private readonly LessonPartAnswerService _lessonPartAnswerService;
        private readonly LessonPartQuestionService _lessonPartQuestionService;
        private readonly IRoxyFilemanHandler _roxyFilemanHandler;

        public ExamController(ExamService examService,
            LessonPartAnswerService lessonPartAnswerService,
            LessonPartQuestionService lessonPartQuestionService,
            ExamDetailService examDetailService
        , StudentService studentService
            , ClassService classService
            , LessonService lessonService
            , LessonScheduleService lessonScheduleService
            , CloneLessonPartService cloneLessonPartService
            , CloneLessonPartAnswerService cloneLessonPartAnswerService
            , CloneLessonPartQuestionService cloneLessonPartQuestionService
            , TeacherService teacherService
            , LearningHistoryService learningHistoryService
            , IRoxyFilemanHandler roxyFilemanHandler
            )
        {
            _lessonPartQuestionService = lessonPartQuestionService;
            _lessonPartAnswerService = lessonPartAnswerService;
            _learningHistoryService = learningHistoryService;
            _examService = examService;
            _classService = classService;
            _lessonService = lessonService;
            _lessonScheduleService = lessonScheduleService;
            _cloneLessonPartAnswerService = cloneLessonPartAnswerService;
            _cloneLessonPartService = cloneLessonPartService;
            _cloneLessonPartQuestionService = cloneLessonPartQuestionService;
            _examDetailService = examDetailService;
            _studentService = studentService;
            _teacherService = teacherService;
            _roxyFilemanHandler = roxyFilemanHandler;
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetList(DefaultModel model)
        {
            var filter = new List<FilterDefinition<ExamEntity>>();
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            else
            {
                filter.Add(Builders<ExamEntity>.Filter.Where(o => o.StudentID == userId));
            }
            if (model.StartDate > DateTime.MinValue)
            {
                filter.Add(Builders<ExamEntity>.Filter.Where(o => o.Created >= new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0)));
            }
            if (model.EndDate > DateTime.MinValue)
            {
                filter.Add(Builders<ExamEntity>.Filter.Where(o => o.Updated <= new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59)));
            }
            var data = filter.Count > 0 ? _examService.Collection.Find(Builders<ExamEntity>.Filter.And(filter)) : _examService.GetAll();
            model.TotalRecord = data.Count();
            var DataResponse = data == null || data.Count() <= 0 || data.Count() < model.PageSize
                ? data.ToList()
                : data.Skip((model.PageIndex - 1) * model.PageSize).Limit(model.PageSize).ToList();
            var mapping = new MappingEntity<ExamEntity, ExamViewModel>();
            var std = DataResponse.Select(o => mapping.AutoOrtherType(o, new ExamViewModel()
            {
                LessonScheduleName = _lessonService.GetItemByID(_lessonScheduleService.GetItemByID(o.LessonScheduleID).LessonID).Title,
                StudentName = _studentService.GetItemByID(o.StudentID).FullName
            })).ToList();

            var response = new Dictionary<string, object>
            {
                { "Data", std},
                { "Model", model }
            };
            return new JsonResult(response);
        }


        [System.Obsolete]
        [HttpPost]
        public JsonResult GetDetails(string examID)
        {
            try
            {
                var userId = User.Claims.GetClaimByType("UserID").Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return null;
                }
                var filter = Builders<ExamDetailEntity>.Filter.Where(o => o.ExamID == examID);
                var data = _examDetailService.Collection.Find(filter);
                var DataResponse = data == null || data.Count() <= 0 ? null : data.ToList();
                var mapping = new MappingEntity<ExamDetailEntity, ExamDetailViewModel>();

                var response = new Dictionary<string, object>
                {
                    { "Data", DataResponse.Select(
                        o=> mapping.AutoOrtherType(o,new ExamDetailViewModel(){
                                Answer = _cloneLessonPartAnswerService.GetItemByID(o.AnswerID).Content,
                                Question = _cloneLessonPartQuestionService.GetItemByID(o.QuestionID).Content,
                                RealAnswer = string.IsNullOrEmpty(o.RealAnswerID) || o.RealAnswerID == "0" ? "" : _cloneLessonPartQuestionService.GetItemByID(o.RealAnswerID).Content,
                                StudentName = _studentService.GetItemByID(_examService.GetItemByID(o.ExamID).StudentID).FullName
                            })
                        )
                    }
                };
                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                var response = new Dictionary<string, object>
                {
                    { "Data", null },
                    {"Error",ex }
                };
                return new JsonResult(response);
            }

        }

        [HttpPost]
        [Obsolete]
        public JsonResult Create(ExamEntity item)
        {
            var userid = User.Claims.GetClaimByType("UserID").Value;

            if (string.IsNullOrEmpty(item.ID) || item.ID == "0")
            {
                var _lesson = _lessonService.GetItemByID(item.LessonID);
                var _class = _classService.GetItemByID(item.ClassID);

                if (_class == null)
                {
                    return new JsonResult(new Dictionary<string, object>
                    {
                       { "Error", "Thông tin không đúng" }
                    });
                }

                var _schedule = _lessonScheduleService.CreateQuery().Find(o => o.LessonID == _lesson.ID && o.ClassID == _class.ID).FirstOrDefault();
                if (_schedule == null)
                {
                    return new JsonResult(new Dictionary<string, object>
                    {
                       { "Error", "Thông tin không đúng" }
                    });
                }

                //COMPLETE ALL INCOMPLETE EXAMS
                var incompleted_exs = _examService.CreateQuery().Find(o => o.LessonID == _lesson.ID && o.StudentID == userid && o.Status == false).SortBy(t => t.Number).ToEnumerable();
                if (incompleted_exs != null && incompleted_exs.Count() > 0)
                {
                    foreach (var ex in incompleted_exs)
                    {
                        _examService.CompleteNoEssay(ex, _lesson, out _, false);
                    }
                }

                var marked = _examService.CreateQuery().Find(o => o.LessonScheduleID == _schedule.ID && o.StudentID == userid && o.Marked).FirstOrDefault();
                if (marked != null)
                {
                    return new JsonResult(new Dictionary<string, object>
                    {
                       { "Error", "Bài kiểm tra đã chấm, không thực hiện lại được!" }
                    });
                }

                item.StudentID = userid;
                item.Number = (int)_examService.CreateQuery().CountDocuments(o => o.LessonScheduleID == _schedule.ID && o.StudentID == item.StudentID) + 1;
                if (_lesson.Limit > 0 && item.Number > _lesson.Limit)
                {
                    return new JsonResult(new Dictionary<string, object>
                    {
                       { "Error", "Bạn đã hết lượt làm bài!" }
                    });
                }
                if (_lesson.TemplateType == LESSON_TEMPLATE.EXAM)
                {

                    if (_schedule.StartDate.ToLocalTime() > DateTime.Now)
                    {
                        return new JsonResult(new Dictionary<string, object>
                        {
                           { "Error", "Bài kiểm tra chưa được mở!" }
                        });
                    }
                }

                if (_schedule.EndDate.ToLocalTime() > new DateTime(1900, 1, 1) && _schedule.EndDate.ToLocalTime() <= DateTime.Now)
                {
                    return new JsonResult(new Dictionary<string, object>
                    {
                       { "Error", "Bài đã quá hạn!" }
                    });
                }


                item.LessonScheduleID = _schedule.ID;
                item.Timer = _lesson.Timer;
                item.Point = 0;
                item.MaxPoint = _lesson.Point;

                item.TeacherID = _class.TeacherID;
                item.ID = null;
                item.Created = DateTime.Now;
                item.CurrentDoTime = DateTime.Now;
                item.Status = false;

                item.QuestionsTotal = _cloneLessonPartQuestionService.CreateQuery().CountDocuments(o => o.LessonID == item.LessonID);
                item.QuestionsDone = 0;
                item.Marked = false;
                _examService.ResetLesssonPoint(_lesson, item.StudentID);
            }

            item.Updated = DateTime.Now;
            //_examService.CreateOrUpdate(item);//MAPPING BUG
            _examService.Save(item);
            return new JsonResult(new Dictionary<string, object>
            {
                { "Data", item }
            });
        }

        [HttpPost]
        public JsonResult GetCurrentExam(string ClassSubjectID, string LessonID)
        {
            var userID = User.Claims.GetClaimByType("UserID").Value;
            var lesson = _lessonService.GetItemByID(LessonID);
            if (lesson == null)
                return new JsonResult(new { Error = "Bài học không đúng" });
            var x = _examService.CreateQuery().Find(o => o.ClassSubjectID == ClassSubjectID && o.LessonID == LessonID &&
            //o.Status == false && 
            o.StudentID == userID).SortByDescending(o => o.ID).FirstOrDefault();
            //hết hạn => đóng luôn
            var schedule = _lessonScheduleService.GetItemByLessonID(LessonID);
            if (x != null && !x.Status)
            {
                if (lesson.TemplateType == LESSON_TEMPLATE.EXAM)
                    if (schedule != null && schedule.EndDate > new DateTime(1900, 1, 1) && schedule.EndDate <= DateTime.Now)
                    {
                        x.Status = true;
                        x.Updated = schedule.EndDate;
                        _examService.Save(x);
                    }
                x.CurrentDoTime = DateTime.Now;
            }
            return new JsonResult(new { exam = x, schedule, limit = lesson.Limit });
        }

        [HttpPost]
        public JsonResult GetReview(string ExamID)
        {
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userId))
                return Json(new ReturnJsonModel
                {
                    StatusCode = ReturnStatus.ERROR,
                    StatusDesc = "Student not found",
                });

            var exam = _examService.GetItemByID(ExamID);
            if (exam == null)
                return Json(new ReturnJsonModel
                {
                    StatusCode = ReturnStatus.ERROR,
                    StatusDesc = "Exam not found",
                });

            if (!exam.Status)
                return Json(new ReturnJsonModel
                {
                    StatusCode = ReturnStatus.ERROR,
                    StatusDesc = "Exam not complete",
                });

            var lesson = _lessonService.GetItemByID(exam.LessonID);
            if (lesson == null)
                return Json(new ReturnJsonModel
                {
                    StatusCode = ReturnStatus.ERROR,
                    StatusDesc = "Origin Exam not found",
                });


            var mapping = new MappingEntity<LessonEntity, StudentLessonViewModel>();
            var mapPart = new MappingEntity<CloneLessonPartEntity, PartViewModel>();
            var mapQuestion = new MappingEntity<CloneLessonPartQuestionEntity, QuestionViewModel>();

            var listParts = _cloneLessonPartService.CreateQuery().Find(o => o.ParentID == lesson.ID && o.ClassID == exam.ClassID).ToList();
            var lessonview = mapping.AutoOrtherType(lesson, new StudentLessonViewModel()
            {
                Part = listParts.Select(o => mapPart.AutoOrtherType(o, new PartViewModel()
                {
                    Questions = _cloneLessonPartQuestionService.CreateQuery().Find(x => x.ParentID == o.ID).ToList()
                        .Select(z => mapQuestion.AutoOrtherType(z, new QuestionViewModel()
                        {
                            CloneAnswers = _cloneLessonPartAnswerService.CreateQuery().Find(x => x.ParentID == z.ID).ToList()
                        }))?.ToList()
                })).ToList()
            });


            //Get full result
            return new JsonResult(
                new Dictionary<string, object> {
                        { "Data", lessonview },
                        { "Exam", exam }
                });
        }

        [HttpPost]
        public JsonResult CreateDetail(string basis, ExamDetailEntity item)
        {
            if (item.ExamID == null)
            {
                return new JsonResult("Access Denied");
            }
            if (!_examService.IsOver(item.ExamID))
            {
                var exam = _examService.GetItemByID(item.ExamID);

                //TODO: recheck history for doing exam
                //await _learningHistoryService.CreateHist(new LearningHistoryEntity()
                //{
                //    ClassID = exam.ClassID,
                //    ClassSubjectID = exam.ClassSubjectID,
                //    LessonID = exam.LessonID,
                //    LessonPartID = item.LessonPartID,
                //    QuestionID = item.QuestionID,
                //    Time = DateTime.Now,
                //    StudentID = User.Claims.GetClaimByType("UserID").Value
                //});
                var files = HttpContext.Request.Form?.Files;
                if (string.IsNullOrEmpty(item.ID) || item.ID == "0" || item.ID == "null")
                {
                    if (item.AnswerID == null && item.AnswerValue == null && files.Count == 0)
                    {
                        return new JsonResult(item);
                    }

                    var dataFiles = _roxyFilemanHandler.UploadAnswerBasis($"{basis}", HttpContext);

                    var map = new MappingEntity<ExamDetailEntity, ExamDetailEntity>();
                    var oldItem = _examDetailService.CreateQuery().Find(o => o.ExamID == item.ExamID && o.QuestionID == item.QuestionID).FirstOrDefault();
                    exam.Updated = DateTime.Now;
                    if (dataFiles.TryGetValue("success", out List<MediaResponseModel> listFiles) && listFiles.Count > 0)
                    {
                        var listMedia = new List<Media>();
                        for (int i = 0; i < listFiles.Count; i++)
                        {
                            var media = new Media()
                            {
                                Created = DateTime.Now,
                                Extension = GetContentType(listFiles[i].Path),
                                Name = listFiles[i].Path,
                                OriginalName = listFiles[i].Path,
                                Path = listFiles[i].Path
                            };
                            listMedia.Add(media);
                        }
                        item.Medias = listMedia;
                    }
                    if (oldItem == null)
                    {
                        exam.QuestionsDone += 1;
                        item.Created = DateTime.Now;
                        item.Updated = DateTime.Now;
                        if (!String.IsNullOrEmpty(item.QuestionID))
                        {
                            var question = _cloneLessonPartQuestionService.GetItemByID(item.QuestionID);
                            if (question == null)
                                return new JsonResult("Data Error");
                            item.ClassID = exam.ClassID;
                            item.StudentID = exam.StudentID;
                            item.LessonPartID = question.ParentID;
                            item.QuestionValue = question.Content;
                            item.MaxPoint = question.Point;
                        }
                        else // Essay
                        {
                            item.MaxPoint = exam.MaxPoint;
                        }
                        _examService.CreateOrUpdate(exam);
                        var xitem = map.Clone(item, new ExamDetailEntity() { });
                        _examDetailService.CreateOrUpdate(xitem);
                        return new JsonResult(xitem);
                    }
                    else
                    {
                        item.AnswerID = item.AnswerID;
                        if (item.AnswerID == null && item.AnswerValue == null)
                        {
                            var deleted = _examDetailService.CreateQuery().DeleteMany(o => o.ExamID == item.ExamID && o.QuestionID == item.QuestionID).DeletedCount;

                            if (deleted > 0)
                            {
                                exam.QuestionsDone -= 1;
                                exam.Updated = DateTime.Now;
                                _examService.CreateOrUpdate(exam);
                                return new JsonResult(item);
                            }
                        }
                        else
                        {
                            item.Updated = DateTime.Now;
                            var xitem = map.Auto(oldItem, item);
                            _examDetailService.CreateOrUpdate(xitem);
                            return new JsonResult(xitem);
                        }

                    }
                }
                else
                {
                    item.Updated = DateTime.Now;
                }
                _examDetailService.CreateOrUpdate(item);
                exam.Updated = DateTime.Now;
                _examService.CreateOrUpdate(exam);
                return new JsonResult(item);
            }
            else
            {
                return new JsonResult("Access Denied");
            }
        }

        [HttpPost]
        public async Task<JsonResult> RemoveDetail(ExamDetailEntity item)
        {
            if (!_examService.IsOver(item.ExamID))
            {
                var exam = _examService.GetItemByID(item.ExamID);
                //TODO: recheck history for doing exam
                //await _learningHistoryService.CreateHist(new LearningHistoryEntity()
                //{
                //    ClassID = exam.ClassID,
                //    LessonID = exam.LessonID,
                //    ClassSubjectID = exam.ClassSubjectID,
                //    LessonPartID = item.LessonPartID,
                //    QuestionID = item.QuestionID,
                //    Time = DateTime.Now,
                //    StudentID = User.Claims.GetClaimByType("UserID").Value
                //});
                if (exam != null)
                {
                    var deleted = await _examDetailService.CreateQuery().DeleteManyAsync(o => o.ExamID == item.ExamID && o.QuestionID == item.QuestionID);
                    exam.Updated = DateTime.Now;
                    if (deleted.DeletedCount > 0 && exam.QuestionsDone > 0)
                        exam.QuestionsDone -= 1;

                    _examService.CreateQuery().ReplaceOne(t => t.ID == exam.ID, exam);
                }
                return new JsonResult(item);
            }
            else
            {
                return new JsonResult("Access Denied");
            }
        }

        //submit form
        [HttpPost]
        [Obsolete]
        public JsonResult CompleteExam(string ExamID)
        {
            var exam = _examService.GetItemByID(ExamID);

            if (exam == null)
            {
                return new JsonResult("Data not found");
            }
            if (exam.Status)
            {
                //return new JsonResult(new { Point = exam.Point, MaxPoint = exam.MaxPoint, ID = exam.ID, Number = exam.Number, Limit = 0 });
                return new JsonResult("Access Denied");
            }
            double point = 0;
            var lesson = _lessonService.GetItemByID(exam.LessonID);
            exam = _examService.CompleteNoEssay(exam, lesson, out point);
            return new JsonResult(new
            {
                Point = point,
                MaxPoint = exam.MaxPoint,
                ID = exam.ID,
                Number = exam.Number,
                Limit = lesson.Limit,
                QuestionsTotal = exam.QuestionsTotal,
                QuestionsPass = exam.QuestionsPass
            });
        }

        public IActionResult Index(DefaultModel model)
        {
            ViewBag.Model = model;
            return View();
        }

        public IActionResult Details(DefaultModel model, string id, string ClassID, string basis)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Bài học không đúng";
                return Redirect($"/{basis}{Url.Action("Index")}");
            }
            ViewBag.CourseID = id;
            ViewBag.ClassID = ClassID;
            ViewBag.Model = model;
            return View();
        }

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
}
