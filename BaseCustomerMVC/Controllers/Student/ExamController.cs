﻿using BaseCustomerEntity.Database;
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

        ////private readonly LessonScheduleService _lessonScheduleService;
        private readonly LessonService _lessonService;
        private readonly LessonHelper _lessonHelper;

        private readonly CloneLessonPartService _cloneLessonPartService;
        private readonly CloneLessonPartAnswerService _cloneLessonPartAnswerService;
        private readonly CloneLessonPartQuestionService _cloneLessonPartQuestionService;

        private readonly TeacherService _teacherService;
        private readonly StudentService _studentService;
        private readonly ClassService _classService;
        //private readonly LearningHistoryService _learningHistoryService;
        private readonly LessonProgressService _lessonProgressService;

        private readonly LessonPartAnswerService _lessonPartAnswerService;
        private readonly LessonPartQuestionService _lessonPartQuestionService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly ClassSubjectProgressService _classSubjectProgressService;

        //
        private readonly ChapterService _chapterService;
        private readonly CenterService _centerService;
        private readonly LessonExamService _lessonExamService;
        private readonly CloneLessonPartExtensionService _cloneLessonPartExtensionService;

        private readonly ProgressHelper _progressHelper;

        private readonly IRoxyFilemanHandler _roxyFilemanHandler;

        public ExamController(
            IndexService indexService,
            ExamService examService,
            ExamDetailService examDetailService,

            LessonService lessonService,
            ////LessonScheduleService lessonScheduleService,
            LessonHelper lessonHelper,

            CloneLessonPartService cloneLessonPartService,
            CloneLessonPartAnswerService cloneLessonPartAnswerService,
            CloneLessonPartQuestionService cloneLessonPartQuestionService,

            StudentService studentService,
            ClassService classService,
            TeacherService teacherService,

            LessonPartAnswerService lessonPartAnswerService,
            LessonPartQuestionService lessonPartQuestionService,
            LessonProgressService lessonProgressService,
            ClassSubjectService classSubjectService,
            ClassSubjectProgressService classSubjectProgressService,

            ProgressHelper progressHelper,

            IRoxyFilemanHandler roxyFilemanHandler,
            /////////////
            ChapterService chapterService,
            CenterService centerService,
            LessonExamService lessonExamService,
            CloneLessonPartExtensionService cloneLessonPartExtensionService
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

            ////_lessonScheduleService = lessonScheduleService;
            _cloneLessonPartAnswerService = cloneLessonPartAnswerService;
            _cloneLessonPartService = cloneLessonPartService;
            _cloneLessonPartQuestionService = cloneLessonPartQuestionService;
            _examDetailService = examDetailService;
            _studentService = studentService;
            _teacherService = teacherService;
            _lessonProgressService = lessonProgressService;
            _classSubjectService = classSubjectService;
            _classSubjectProgressService = classSubjectProgressService;

            _progressHelper = progressHelper;

            _roxyFilemanHandler = roxyFilemanHandler;
            //////////
            _chapterService = chapterService;
            _centerService = centerService;
            _lessonExamService = lessonExamService;
            _cloneLessonPartExtensionService = cloneLessonPartExtensionService;
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
                LessonScheduleName = _lessonService.GetItemByID(o.LessonID).Title,
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

                var exam = _examService.CreateQuery().Find(x => x.LessonID == LessonID
                //&& x.ClassSubjectID == ClassSubjectID && x.ClassID == ClassID 
                && x.StudentID == student.ID).SortByDescending(x => x.Number);
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

                //var _schedule = _lessonScheduleService.GetItemByLessonID(_lesson.ID);
                //if (_schedule == null)
                //{
                //    return new JsonResult(new Dictionary<string, object>
                //    {
                //       { "Error", "Thông tin không đúng" }
                //    });
                //}

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

                    if (_lesson.StartDate > DateTime.UtcNow)
                    {
                        return new JsonResult(new Dictionary<string, object>
                        {
                           { "Error", "Bài kiểm tra chưa được mở!" }
                        });
                    }
                }

                if (_lesson.EndDate > new DateTime(1900, 1, 1) && _lesson.EndDate <= DateTime.UtcNow)
                {
                    return new JsonResult(new Dictionary<string, object>
                    {
                       { "Error", "Bài đã quá hạn!" }
                    });
                }

                //item.LessonScheduleID = _lesson.ID;
                item.Timer = _lesson.Timer;
                item.Point = 0;
                item.MaxPoint = _lesson.Point;
                item.TeacherID = _class.TeacherID;
                item.ID = null;
                item.Created = DateTime.UtcNow;
                item.CurrentDoTime = DateTime.UtcNow;
                item.Status = false;

                item.QuestionsTotal = _cloneLessonPartQuestionService.CountByLessonID(item.LessonID);
                //TODO: Save Question Total in Lesson info
                item.QuestionsDone = 0;
                item.Marked = false;

                await _progressHelper.UpdateLessonPoint(item, item.Number == 1);//increase counter for first exam only
            }

            item.Updated = DateTime.UtcNow;

            _examService.Save(item);

            return new JsonResult(new Dictionary<string, object>
            {
                { "Data", item }
            });
        }

        [HttpPost]
        public JsonResult GetCurrentExam(string LessonID, string ID)
        {
            var userID = User.Claims.GetClaimByType("UserID").Value;
            var lesson = _lessonService.GetItemByID(LessonID);
            if (lesson == null)
                return new JsonResult(new { Error = "Bài học không đúng" });
            var exam = _examService.GetLastestByLessonAndStudent(LessonID, userID);

            //hết hạn => đóng luôn
            //var schedule = _lessonScheduleService.GetItemByLessonID(LessonID);
            //schedule.StartDate = schedule.StartDate.ToUniversalTime();
            //schedule.EndDate = schedule.EndDate.ToUniversalTime();
            if (exam != null && !exam.Status)
            {
                //if (lesson.TemplateType == LESSON_TEMPLATE.EXAM)
                if (lesson.EndDate > new DateTime(1900, 1, 1) && lesson.EndDate <= DateTime.UtcNow)
                {
                    _lessonHelper.CompleteNoEssay(exam, lesson, out _, false);
                }
                else
                {
                    if (_lessonHelper.IsOvertime(exam, lesson, out _))//Overtime
                        _lessonHelper.CompleteNoEssay(exam, lesson, out _, false);
                    else if (exam.ID != ID)//change Exam
                        _lessonHelper.CompleteNoEssay(exam, lesson, out _, false);
                    else
                        exam.CurrentDoTime = DateTime.UtcNow;
                }
            }
            return new JsonResult(new { exam, schedule = new LessonEntity { StartDate = lesson.StartDate, EndDate = lesson.EndDate }, limit = lesson.Limit });
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
            if (item.ExamID == null || item.ExamID == "undefined")
            {
                return new JsonResult(new { error = "Không tìm thấy bài kiểm tra", reset = true });
            }
            var exam = _examService.GetItemByID(item.ExamID);

            var lesson = _lessonService.GetItemByID(exam.LessonID);
            if (lesson == null)
            {
                return new JsonResult(new { error = "Không tìm thấy bài học", reset = true });
            }
            var endtype = EXAM_END_TYPE.OVERDATE;
            if (!_lessonHelper.IsOvertime(exam, lesson, out endtype))
            {
                //TODO: recheck history for doing exam
                //await _learningHistoryService.CreateHist(new LearningHistoryEntity()
                //{
                //    ClassID = exam.ClassID,
                //    ClassSubjectID = exam.ClassSubjectID,
                //    LessonID = exam.LessonID,
                //    LessonPartID = item.LessonPartID,
                //    QuestionID = item.QuestionID,
                //    Time = DateTime.UtcNow,
                //    StudentID = User.Claims.GetClaimByType("UserID").Value
                //});
                var files = HttpContext.Request.Form?.Files;
                if (string.IsNullOrEmpty(item.ID) || item.ID == "0" || item.ID == "null")
                {
                    if (item.AnswerID == null && item.AnswerValue == null && files.Count == 0)
                    {
                        return Json(item);
                    }

                    var dataFiles = _roxyFilemanHandler.UploadAnswerBasis($"{basis}/Student", HttpContext);

                    var map = new MappingEntity<ExamDetailEntity, ExamDetailEntity>();


                    var oldItem = _examDetailService.GetByExamAndQuestion(item.ExamID, item.QuestionID);

                    if (dataFiles.TryGetValue("success", out List<MediaResponseModel> listFiles) && listFiles.Count > 0)
                    {
                        var listMedia = new List<Media>();
                        for (int i = 0; i < listFiles.Count; i++)
                        {
                            var media = new Media()
                            {
                                Created = DateTime.UtcNow,
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
                        item.Created = DateTime.UtcNow;
                        item.Updated = DateTime.UtcNow;
                        if (!String.IsNullOrEmpty(item.QuestionID))
                        {
                            var question = _cloneLessonPartQuestionService.GetItemByID(item.QuestionID);
                            if (question == null)
                                return new JsonResult(new { error = "Không tìm thấy câu hỏi" });
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
                            item.Updated = DateTime.UtcNow;
                            var xitem = map.Auto(oldItem, item);
                            _examDetailService.Save(xitem);
                            return Json(xitem);
                        }

                    }
                }
                else
                {
                    item.Updated = DateTime.UtcNow;
                }
                _examDetailService.CreateOrUpdate(item);
                exam.Updated = DateTime.UtcNow;
                _examService.CreateOrUpdate(exam);
                return Json(item);
            }
            else
            {
                var lastestEx = _examService.GetLastestByLessonAndStudent(exam.LessonID, exam.StudentID);
                if (lastestEx.ID != exam.ID)
                    return new JsonResult(new { error = "Bạn hoặc ai đó đang làm lại bài kiểm tra này. Vui lòng không thực hiện bài trên phiên làm việc đồng thời!", reset = true });

                if (endtype == EXAM_END_TYPE.OVERDATE)
                    return new JsonResult(new { error = $"Bài đã quá hạn", date = lesson.EndDate, reset = true });
                else
                    return new JsonResult(new { error = $"Thời gian làm bài đã hết", reset = true });
            }
        }

        [HttpPost]
        public JsonResult RemoveDetail(ExamDetailEntity item)
        {
            var exam = _examService.GetItemByID(item.ExamID);
            if (exam == null)
            {
                return new JsonResult(new { error = "Không tìm thấy bài kiểm tra", reset = true });
            }
            var lesson = _lessonService.GetItemByID(exam.LessonID);
            if (lesson == null)
            {
                return new JsonResult(new { error = "Không tìm thấy bài học", reset = true });
            }
            var endtype = EXAM_END_TYPE.OVERDATE;

            if (!_lessonHelper.IsOvertime(exam, lesson, out endtype))
            {
                //TODO: recheck history for doing exam
                var deleted = _examDetailService.RemoveAnswer(item.ExamID, item.QuestionID);
                if (deleted > 0 && exam.QuestionsDone > 0)
                    _examService.IncreaseQuestionDone(exam.ID, -1);
                return new JsonResult(item);
            }
            else
            {
                if (endtype == EXAM_END_TYPE.OVERDATE)
                    return new JsonResult(new { error = $"Bài đã quá hạn", date = lesson.EndDate, reset = true });
                else
                    return new JsonResult(new { error = $"Thời gian làm bài đã hết", reset = true });
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
                return new JsonResult($"Bài đã kết thúc lúc {TimeZone.CurrentTimeZone.ToLocalTime(exam.Updated).ToString("HH:mm:ss dd-MM-yyyy")} - Trạng thái bài kiểm tra đã hoàn thành");
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

            //----TH loi 0s -> reset lượt làm
            //var created = exam.Created;
            //var end = exam.Updated;
            //var subtract = end.Subtract(created).TotalSeconds;
            //var UserID = User.Claims.GetClaimByType("UserID").Value;
            //if (subtract < 1)//Th bai ktra 0s
            //{
            //    var detailExams = _examDetailService.GetByExamID(ExamID);
            //    if (detailExams.Any())
            //    {
            //        _examDetailService.CreateQuery().DeleteMany(x => x.ExamID.Equals(ExamID) && x.StudentID.Equals(UserID));
            //    }

            //    var lessonprogress = _lessonProgressService.GetByStudentID_LessonID(UserID, exam.LessonID);
            //    if (lessonprogress == null) return Json(new Dictionary<String, Object>
            //            {
            //                {"Msg","Chưa có thông tin bài làm" }
            //            });
            //    _examService.CreateQuery().DeleteOne(x => x.ID == exam.ID);
            //    if (lessonprogress.Tried == 1)
            //        _lessonProgressService.CreateQuery().DeleteOne(x => x.ID == lessonprogress.ID);
            //    else
            //    {
            //        lessonprogress.Tried = lessonprogress.Tried - 1;
            //        var exams = _examService.CreateQuery().Find(x => x.LessonID == exam.LessonID && UserID == x.StudentID).SortByDescending(x => x.Created);
            //        lessonprogress.LastDate = exams.FirstOrDefault().Created;
            //        lessonprogress.LastPoint = exams.FirstOrDefault().LastPoint;
            //        lessonprogress.AvgPoint = exams.ToList().Average(x => x.LastPoint);
            //        lessonprogress.MinPoint = exams.ToList().Min(x => x.LastPoint);
            //        lessonprogress.MaxPoint = exams.ToList().Max(x => x.LastPoint);
            //        Int32 index = (Int32)exams.CountDocuments() - 1;
            //        lessonprogress.PointChange = exams.FirstOrDefault().LastPoint - exams.ToList().ElementAtOrDefault(index).LastPoint;
            //        _lessonProgressService.CreateQuery().ReplaceOne(x => x.ID == lessonprogress.ID, lessonprogress, new UpdateOptions() { IsUpsert = false });
            //        _examService.CreateQuery().DeleteOne(x => x.ID == exam.ID);
            //    }

            //    return new JsonResult("Có lỗi trong quá trình mở bài (0s). Vui lòng thực hiện lại thao tác");
            //}

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

        public JsonResult GetLessonProgressList(string LessonID)
        {
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            var StudentID = _studentService.GetItemByID(userId).ID;

            var result = new List<StudentLessonResultViewModel>();
            var lesson = _lessonService.GetItemByID(LessonID);
            if (lesson == null)
                return Json("No data");
            var listStudent = new List<StudentEntity>();
            if (!string.IsNullOrEmpty(StudentID))
                listStudent.Add(_studentService.GetItemByID(StudentID));
            else
                listStudent = _studentService.GetStudentsByClassId(lesson.ClassID).ToList();

            if (listStudent != null && listStudent.Count() > 0)
            {
                foreach (var student in listStudent)
                {
                    var examresult = _examService.CreateQuery().Find(t => t.StudentID == student.ID && t.LessonID == lesson.ID).SortByDescending(t => t.ID).ToList();
                    var progress = _lessonProgressService.GetByStudentID_LessonID(student.ID, lesson.ID);
                    var tried = examresult.Count();
                    var maxpoint = tried == 0 ? 0 : examresult.Max(t => t.MaxPoint > 0 ? t.Point * 100 / t.MaxPoint : 0);
                    var minpoint = tried == 0 ? 0 : examresult.Min(t => t.MaxPoint > 0 ? t.Point * 100 / t.MaxPoint : 0);
                    var avgpoint = tried == 0 ? 0 : examresult.Average(t => t.MaxPoint > 0 ? t.Point * 100 / t.MaxPoint : 0);

                    var lastEx = examresult.FirstOrDefault();
                    result.Add(new StudentLessonResultViewModel(student)
                    {
                        LastTried = lastEx?.Created ?? new DateTime(1900, 1, 1),
                        MaxPoint = maxpoint,
                        MinPoint = minpoint,
                        AvgPoint = avgpoint,
                        TriedCount = tried,
                        LastOpen = progress?.LastDate ?? new DateTime(1900, 1, 1),
                        OpenCount = progress?.TotalLearnt ?? 0,
                        LastPoint = lastEx != null ? (lastEx.MaxPoint > 0 ? lastEx.Point * 100 / lastEx.MaxPoint : 0) : 0,
                        IsCompleted = lastEx != null && lastEx.Status,
                        ListExam = examresult.Select(t => new ExamDetailCompactView(t)).ToList()
                    });
                }
            }

            var response = new Dictionary<string, object>
                {
                    { "Data", result }
                };
            return new JsonResult(response);
        }

        public async Task<JsonResult> GetLessonProgressListInWeek(String basis, String ClassSubjectID, DateTime StartWeek, DateTime EndWeek)
        {
            var userId = User.Claims.GetClaimByType("UserID").Value;
            var student = _studentService.GetItemByID(userId);
            if (student == null)
            {
                return Json("Học sinh không tồn tại");
            }

            var classSbj = _classSubjectService.GetItemByID(ClassSubjectID);
            if (classSbj == null)
            {
                return Json("Môn học không có");
            }

            var result = new List<StudentLessonResultViewModel>();
            if (classSbj.TypeClass == CLASSSUBJECT_TYPE.EXAM)
            {
                result = await _progressHelper.GetLessonProgressList(StartWeek, EndWeek, student, classSbj, true);
            }
            else
            {
                var data = await _progressHelper.GetLessonProgressList(StartWeek, EndWeek, student, classSbj);
                foreach (var d in data.ToList())
                {
                    var target = _classSubjectProgressService.CreateQuery().Find(x => x.ClassID == classSbj.ClassID && x.ClassSubjectID == classSbj.ID && x.StudentID == userId).FirstOrDefault();
                    d.Target = target == null ? 0 : target.Target;
                    result.Add(d);
                }
            }

            return Json(result);
        }


        #region Liên quan đến kì thi +  kho đề
        public JsonResult GetLessonExams(DefaultModel model,String basis)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var st = _studentService.GetItemByID(UserID);
                var listClassIDs = st.JoinedClasses;
                var csbjExam = _classSubjectService.GetClassSubjectExamByClassIDs(listClassIDs);
                var csbjExamIds = csbjExam.Select(x => x.ID).ToList();

                var filter = new List<FilterDefinition<LessonEntity>>();
                switch (model.Sort){
                    case "latest":
                        break;
                    case "oldest":
                        break;
                    case "byname":
                        break;
                    case "created":
                        break;
                    default:
                        break;
                }

                var lessons = _lessonService.CreateQuery().Find(x =>
                            csbjExamIds.Contains(x.ClassSubjectID) &&
                            x.TemplateType == LESSON_TEMPLATE.EXAM &&
                            (x.LessonExamID != null || x.LessonExamID.Count() > 0)
                            ).ToList().OrderByDescending(x=>x.Created);

                model.TotalRecord = lessons.Count();
                return Json(new Dictionary<String, Object> {
                    {"Status",true },
                    {"Msg","" },
                    {"Data",lessons }
                });
            }
            catch(Exception ex)
            {
                return Json(new Dictionary<String, Object> {
                    {"Status",false },
                    {"Msg",ex.Message },
                    {"Data",null }
                });
            }
        }

        public IActionResult Detail(String basis, DefaultModel model, String ClassID)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var student = _studentService.GetItemByID(UserID);
                if (ClassID == null)
                    return Redirect($"/{basis}{Url.Action("Index", "Course")}");
                var currentCs = _classSubjectService.GetItemByID(ClassID);
                if (currentCs == null)
                    return Redirect($"/{basis}{Url.Action("Index", "Course")}");
                var currentClass = _classService.GetItemByID(currentCs.ClassID);
                if (currentClass == null)
                    return Redirect($"/{basis}{Url.Action("Index", "Course")}");
                var lesson = _lessonService.GetItemByID(model.ID);
                var index = new Random().Next(0, lesson.LessonExamID.Count());
                var lessonExam = _lessonExamService.GetItemByID(lesson.LessonExamID[index]);
                if (lesson == null)
                    return Redirect($"/{basis}{Url.Action("Index", "Course")}");

                var chapter = _chapterService.GetItemByID(lesson.ChapterID);
                //var pass = true;
                ViewBag.LessonExam = lessonExam;
                ViewBag.Lesson = lesson;
                ViewBag.Type = lesson.TemplateType;
                //string condChap = "";
                //if (chapter != null && !String.IsNullOrEmpty(chapter.ConditionChapter))//has condition
                //{
                //    var conditionchap = _chapterService.GetItemByID(chapter.ConditionChapter);
                //    if (conditionchap != null)
                //    {
                //        condChap = conditionchap.Name;
                //        if (conditionchap.BasePoint > 0 && chapter.PracticeCount > 0)
                //        {
                //            //check condition
                //            var progress = _chapterProgressService.GetItemByChapterID(conditionchap.ID, UserID);
                //            if (progress == null)
                //            {
                //                pass = false;
                //            }
                //            else
                //            {
                //                pass = progress.PracticePoint / chapter.PracticeCount >= conditionchap.BasePoint;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        //????
                //    }
                //}
                //if (pass == false)
                //{
                //    ViewBag.FailPass = true;
                //    ViewBag.CondChap = condChap;
                //}
                //else
                //{
                //    var nextLesson = _lessonService.CreateQuery().Find(t => t.ChapterID == lesson.ChapterID && t.Order > lesson.Order).SortBy(t => t.Order).FirstOrDefault();
                ViewBag.Class = currentClass;
                //    if (string.IsNullOrEmpty(currentCs.CourseName))
                //        currentCs.CourseName = _courseService.GetItemByID(currentCs.CourseID)?.Name;
                ViewBag.Subject = currentCs;
                //    ViewBag.NextLesson = nextLesson;
                //    ViewBag.Chapter = chapter == null ? new ChapterEntity() : chapter;
                //}
                ViewBag.Center = _centerService.GetItemByCode(basis);
                ViewBag.CurrentUser = student;

                ViewBag.Target = _classSubjectProgressService.GetItemByClassSubjectID(currentCs.ID, UserID)?.Target;
                //if (newui == 1)
                return View("Detail");
                //return View();
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult GetCurrentLessonExam(string LessonExamID, string ID,String ClassSubjectID)
        {
            var userID = User.Claims.GetClaimByType("UserID").Value;
            var lessonExam = _lessonExamService.GetItemByID(LessonExamID);
            var lesson = _lessonService.CreateQuery().Find(x => x.ClassSubjectID.Equals(ClassSubjectID) && x.LessonExamID.Contains(lessonExam.ID)).FirstOrDefault();
            if (lesson == null)
                return new JsonResult(new { Error = "Bài học không đúng" });
            var exam = _examService.GetLastestByLessonAndStudent(lesson.ID, userID);

            //hết hạn => đóng luôn
            //var schedule = _lessonScheduleService.GetItemByLessonID(LessonID);
            //schedule.StartDate = schedule.StartDate.ToUniversalTime();
            //schedule.EndDate = schedule.EndDate.ToUniversalTime();
            if (exam != null && !exam.Status)
            {
                //if (lesson.TemplateType == LESSON_TEMPLATE.EXAM)
                if (lesson.EndDate > new DateTime(1900, 1, 1) && lesson.EndDate <= DateTime.UtcNow)
                {
                    _lessonHelper.CompleteNoEssay(exam, lesson, out _, false);
                }
                else
                {
                    if (_lessonHelper.IsOvertime(exam, lesson, out _))//Overtime
                        _lessonHelper.CompleteNoEssay(exam, lesson, out _, false);
                    else if (exam.ID != ID)//change Exam
                        _lessonHelper.CompleteNoEssay(exam, lesson, out _, false);
                    else
                        exam.CurrentDoTime = DateTime.UtcNow;
                }
            }
            return new JsonResult(new { exam, schedule = new LessonEntity { StartDate = lesson.StartDate, EndDate = lesson.EndDate }, limit = lesson.Limit });
        }

        [HttpPost]
        [Obsolete]
        public async Task<JsonResult> CreateExam(ExamEntity item)
        {
            var userid = User.Claims.GetClaimByType("UserID").Value;

            if (string.IsNullOrEmpty(item.ID) || item.ID == "0")
            {
                var _lesson = _lessonExamService.GetItemByID(item.LessonExamID);
                var _class = _classService.GetItemByID(item.ClassID);

                if (_class == null)
                {
                    return new JsonResult(new Dictionary<string, object>
                    {
                       { "Error", "Thông tin không đúng" }
                    });
                }

                //var _schedule = _lessonScheduleService.GetItemByLessonID(_lesson.ID);
                //if (_schedule == null)
                //{
                //    return new JsonResult(new Dictionary<string, object>
                //    {
                //       { "Error", "Thông tin không đúng" }
                //    });
                //}

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

                    if (_lesson.StartDate > DateTime.UtcNow)
                    {
                        return new JsonResult(new Dictionary<string, object>
                        {
                           { "Error", "Bài kiểm tra chưa được mở!" }
                        });
                    }
                }

                if (_lesson.EndDate > new DateTime(1900, 1, 1) && _lesson.EndDate <= DateTime.UtcNow)
                {
                    return new JsonResult(new Dictionary<string, object>
                    {
                       { "Error", "Bài đã quá hạn!" }
                    });
                }
                var lesson = _lessonService.CreateQuery().Find(x => x.ClassSubjectID.Equals(item.ClassSubjectID) && x.LessonExamID.Contains(item.LessonExamID)).FirstOrDefault();
                //item.LessonScheduleID = _lesson.ID;
                item.Timer = _lesson.Timer;
                item.Point = 0;
                item.MaxPoint = _lesson.Point;
                item.TeacherID = _class.TeacherID;
                item.ID = null;
                item.Created = DateTime.UtcNow;
                item.CurrentDoTime = DateTime.UtcNow;
                item.Status = false;
                item.LessonID = lesson.ID;
                item.CodeExam = _lesson.CodeExam;

                item.QuestionsTotal = _cloneLessonPartExtensionService.CountByLessonID(item.LessonExamID);
                //TODO: Save Question Total in Lesson info
                item.QuestionsDone = 0;
                item.Marked = false;

                await _progressHelper.UpdateLessonPoint(item, item.Number == 1);//increase counter for first exam only
            }

            item.Updated = DateTime.UtcNow;

            _examService.Save(item);

            return new JsonResult(new Dictionary<string, object>
            {
                { "Data", item }
            });
        }
        #endregion
    }
}
