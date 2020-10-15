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

        private readonly IndexService _indexService;
        private readonly ExamService _examService;
        private readonly ExamDetailService _examDetailService;

        private readonly LessonScheduleService _lessonScheduleService;
        private readonly LessonService _lessonService;
        private readonly LessonHelper _lessonHelper;

        private readonly CloneLessonPartService _cloneLessonPartService;
        private readonly CloneLessonPartAnswerService _cloneLessonPartAnswerService;
        private readonly CloneLessonPartQuestionService _cloneLessonPartQuestionService;

        private readonly TeacherService _teacherService;
        private readonly StudentService _studentService;
        private readonly ClassService _classService;
        //private readonly LearningHistoryService _learningHistoryService;

        private readonly LessonPartAnswerService _lessonPartAnswerService;
        private readonly LessonPartQuestionService _lessonPartQuestionService;

        private readonly ProgressHelper _progressHelper;

        private readonly IRoxyFilemanHandler _roxyFilemanHandler;

        public ExamController(
            IndexService indexService,
            ExamService examService,
            ExamDetailService examDetailService,

            LessonService lessonService,
            LessonScheduleService lessonScheduleService,
            LessonHelper lessonHelper,

            CloneLessonPartService cloneLessonPartService,
            CloneLessonPartAnswerService cloneLessonPartAnswerService,
            CloneLessonPartQuestionService cloneLessonPartQuestionService,

            StudentService studentService,
            ClassService classService,
            TeacherService teacherService,

            LessonPartAnswerService lessonPartAnswerService,
            LessonPartQuestionService lessonPartQuestionService,

            ProgressHelper progressHelper,

            IRoxyFilemanHandler roxyFilemanHandler
            )
        {
            _lessonPartQuestionService = lessonPartQuestionService;
            _lessonPartAnswerService = lessonPartAnswerService;

            //_learningHistoryService = learningHistoryService;

            _examService = examService;
            _indexService = indexService;
            _classService = classService;
            _lessonService = lessonService;
            _lessonHelper = lessonHelper;

            _lessonScheduleService = lessonScheduleService;
            _cloneLessonPartAnswerService = cloneLessonPartAnswerService;
            _cloneLessonPartService = cloneLessonPartService;
            _cloneLessonPartQuestionService = cloneLessonPartQuestionService;
            _examDetailService = examDetailService;
            _studentService = studentService;
            _teacherService = teacherService;

            _progressHelper = progressHelper;

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

        public JsonResult GetDetailExam(string LessonID, string ClassSubjectID, string ClassID)
        {
            try
            {
                var userid = User.Claims.GetClaimByType("UserID").Value;
                var student = _studentService.GetItemByID(userid);
                if (student == null)
                {
                    var response = new Dictionary<string, object>
                    {
                        { "Data", null },
                        {"Error","Học sinh không tồn tại." }
                    };
                    return new JsonResult(response);
                }

                if (string.IsNullOrEmpty(LessonID))
                {
                    var response = new Dictionary<string, object>
                    {
                        { "Data", null },
                        {"Error","Bài học không tồn tại" }
                    };
                    return new JsonResult(response);
                }

                if (string.IsNullOrEmpty(ClassSubjectID))
                {
                    var response = new Dictionary<string, object>
                    {
                        { "Data", null },
                        {"Error","Bài học không tồn tại" }
                    };
                    return new JsonResult(response);
                }

                if (string.IsNullOrEmpty(ClassID))
                {
                    var response = new Dictionary<string, object>
                    {
                        { "Data", null },
                        {"Error","Bài học không tồn tại" }
                    };
                    return new JsonResult(response);
                }

                var exam = _examService.CreateQuery().Find(x => x.LessonID == LessonID && x.ClassSubjectID == ClassSubjectID && x.ClassID == ClassID && x.StudentID == student.ID).SortByDescending(x => x.Number);
                if (exam.CountDocuments() < 2)
                {
                    var response = new Dictionary<string, object>
                    {
                        { "Data", null },
                        {"Msg","Chưa làm lần nào"}
                    };
                    return new JsonResult(response);
                }

                var _exam = exam.ToList()[1];
                var detailExam = _examDetailService.GetByExamID(_exam.ID);
                var dataResponse = new List<ExamDetailViewModel>();
                foreach (var detail in detailExam.ToList())
                {
                    var data = new ExamDetailViewModel()
                    {
                        ID = detail.ID,
                        ExamID = detail.ExamID,
                        QuestionID = detail.QuestionID,
                        AnswerValue = detail.AnswerValue,
                        Point = detail.Point,
                        LessonPartID = detail.LessonPartID,
                        AnswerID = detail.AnswerID
                    };
                    dataResponse.Add(data);
                }

                return new JsonResult(new Dictionary<string, object>
                {
                    {"Data",dataResponse },
                    {"Error",null }
                });
            }
            catch (Exception ex)
            {
                var response = new Dictionary<string, object>
                    {
                        { "Data", null },
                        {"Error",ex.Message }
                    };
                return new JsonResult(response);
            }

        }

        [HttpPost]
        [Obsolete]
        public async Task<JsonResult> Create(ExamEntity item)
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

                var _schedule = _lessonScheduleService.GetItemByLessonID(_lesson.ID);
                if (_schedule == null)
                {
                    return new JsonResult(new Dictionary<string, object>
                    {
                       { "Error", "Thông tin không đúng" }
                    });
                }

                //COMPLETE ALL INCOMPLETE EXAMS
                var incompleted_exs = _examService.GetPreviousIncompletedExams(_lesson.ID, userid).ToList();
                if (incompleted_exs != null && incompleted_exs.Count() > 0)
                {
                    foreach (var ex in incompleted_exs)
                    {
                        _lessonHelper.CompleteNoEssay(ex, _lesson, out _, false);
                    }
                }

                var isMarked = _examService.IsLessonMarked(_lesson.ID, userid);
                if (isMarked)
                {
                    return new JsonResult(new Dictionary<string, object>
                    {
                       { "Error", "Bài kiểm tra đã chấm, không thực hiện lại được!" }
                    });
                }

                item.StudentID = userid;
                item.Number = //(int)_indexService.GetNewIndex(_schedule.ID + "_" + item.StudentID);

                (int)_examService.CountByStudentAndLesson(_lesson.ID, item.StudentID) + 1;
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

                item.QuestionsTotal = _cloneLessonPartQuestionService.CountByLessonID(item.LessonID);
                //TODO: Save Question Total in Lesson info
                item.QuestionsDone = 0;
                item.Marked = false;

                await _progressHelper.ResetLesssonPoint(_lesson, item.StudentID);
            }

            item.Updated = DateTime.Now;

            _examService.Save(item);

            return new JsonResult(new Dictionary<string, object>
            {
                { "Data", item }
            });
        }

        [HttpPost]
        public JsonResult GetCurrentExam(string LessonID)
        {
            var userID = User.Claims.GetClaimByType("UserID").Value;
            var lesson = _lessonService.GetItemByID(LessonID);
            if (lesson == null)
                return new JsonResult(new { Error = "Bài học không đúng" });
            var exam = _examService.GetLastestByLessonAndStudent(LessonID, userID);

            //hết hạn => đóng luôn
            var schedule = _lessonScheduleService.GetItemByLessonID(LessonID);
            if (exam != null && !exam.Status)
            {
                //if (lesson.TemplateType == LESSON_TEMPLATE.EXAM)
                if (schedule != null && schedule.EndDate > new DateTime(1900, 1, 1) && schedule.EndDate <= DateTime.Now)
                {
                    _lessonHelper.CompleteNoEssay(exam, lesson, out _, false);
                }
            }
            return new JsonResult(new { exam, schedule, limit = lesson.Limit });
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

            var listParts = _cloneLessonPartService.GetByLessonID(lesson.ID).ToList();
            var lessonview = mapping.AutoOrtherType(lesson, new StudentLessonViewModel()
            {
                Part = listParts.Select(o => mapPart.AutoOrtherType(o, new PartViewModel()
                {
                    Questions = _cloneLessonPartQuestionService.GetByPartID(o.ID)
                        .Select(z => mapQuestion.AutoOrtherType(z, new QuestionViewModel()
                        {
                            CloneAnswers = _cloneLessonPartAnswerService.GetByQuestionID(z.ID).ToList()
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
            var exam = _examService.GetItemByID(item.ExamID);
            if (!_lessonHelper.IsOvertime(exam))
            {
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
                        return Json(item);
                    }

                    var dataFiles = _roxyFilemanHandler.UploadAnswerBasis($"{basis}", HttpContext);

                    var map = new MappingEntity<ExamDetailEntity, ExamDetailEntity>();


                    var oldItem = _examDetailService.GetByExamAndQuestion(item.ExamID, item.QuestionID);

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
                        item.ID = null;
                        item.Created = DateTime.Now;
                        item.Updated = DateTime.Now;
                        if (!String.IsNullOrEmpty(item.QuestionID))
                        {
                            var question = _cloneLessonPartQuestionService.GetItemByID(item.QuestionID);
                            if (question == null)
                                return new JsonResult("Không tìm thấy câu hỏi");
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
                        _examService.IncreaseQuestionDone(item.ExamID, 1);
                        _examDetailService.Save(item);
                        return Json(item);
                    }
                    else
                    {
                        item.AnswerID = item.AnswerID;
                        if (item.AnswerID == null && item.AnswerValue == null)
                        {
                            var deleted = _examDetailService.RemoveAnswer(item.ExamID, item.QuestionID);
                            if (deleted > 0)
                            {
                                _examService.IncreaseQuestionDone(item.ExamID, -1);
                                return Json(item);
                            }
                        }
                        else
                        {
                            item.Updated = DateTime.Now;
                            var xitem = map.Auto(oldItem, item);
                            _examDetailService.Save(xitem);
                            return Json(xitem);
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
                return Json(item);
            }
            else
            {
                return Json("Access Denied");
            }
        }

        [HttpPost]
        public JsonResult RemoveDetail(ExamDetailEntity item)
        {
            var exam = _examService.GetItemByID(item.ExamID);
            if (exam == null)
            {
                return new JsonResult("Không tìm thấy bài kiểm tra");
            }
            if (!_lessonHelper.IsOvertime(exam))
            {
                //TODO: recheck history for doing exam
                var deleted = _examDetailService.RemoveAnswer(item.ExamID, item.QuestionID);
                if (deleted > 0 && exam.QuestionsDone > 0)
                    _examService.IncreaseQuestionDone(exam.ID, -1);
                return new JsonResult(item);
            }
            else
            {
                return new JsonResult("Bài đã kết thúc");
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
                return new JsonResult("Không tìm thấy bài kiểm tra");
            }
            if (exam.Status)
            {
                //return new JsonResult(new { Point = exam.Point, MaxPoint = exam.MaxPoint, ID = exam.ID, Number = exam.Number, Limit = 0 });
                return new JsonResult("Bài đã kết thúc");
            }
            double point = 0;
            var lesson = _lessonService.GetItemByID(exam.LessonID);

            //COMPLETE ALL PREV INCOMPLETE EXAMS
            var incompleted_exs = _examService.GetPreviousIncompletedExams(exam.LessonID, exam.StudentID, exam.Number);

            if (incompleted_exs != null && incompleted_exs.Count() > 0)
            {
                foreach (var ex in incompleted_exs)
                {
                    _lessonHelper.CompleteNoEssay(ex, lesson, out _, false);
                }
            }

            exam = _lessonHelper.CompleteNoEssay(exam, lesson, out point);
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
