using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Core_v2.Globals;
using Core_v2.Interfaces;
using FileManagerCore.Globals;
using FileManagerCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class ExamController : TeacherController
    {

        private readonly ExamService _service;
        private readonly ExamDetailService _examDetailService;
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly LessonService _lessonService;

        private readonly LessonPartQuestionService _lessonPartQuestionService;
        private readonly LessonPartAnswerService _lessonPartAnswerService;

        private readonly CloneLessonPartService _cloneLessonPartService;
        private readonly CloneLessonPartAnswerService _cloneLessonPartAnswerService;
        private readonly CloneLessonPartQuestionService _cloneLessonPartQuestionService;

        private readonly TeacherService _teacherService;
        private readonly StudentService _studentService;
        private readonly ClassService _classService;
        private readonly LearningHistoryService _learningHistoryService;
        private readonly LessonProgressService _lessonProgressService;
        //private readonly ScoreService _scoreService;
        private readonly IRoxyFilemanHandler _roxyFilemanHandler;

        public ExamController(ExamService service,
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
            , LessonPartQuestionService lessonPartQuestionService
            , LessonPartAnswerService lessonPartAnswerService
            , LessonProgressService lessonProgressService
            //, ScoreService scoreService
            , IRoxyFilemanHandler roxyFilemanHandler
            )
        {
            _learningHistoryService = learningHistoryService;
            _service = service;
            _classService = classService;
            _lessonService = lessonService;
            _lessonScheduleService = lessonScheduleService;
            _cloneLessonPartAnswerService = cloneLessonPartAnswerService;
            _cloneLessonPartService = cloneLessonPartService;
            _cloneLessonPartQuestionService = cloneLessonPartQuestionService;
            _examDetailService = examDetailService;
            _studentService = studentService;
            _teacherService = teacherService;
            _lessonPartQuestionService = lessonPartQuestionService;
            _lessonPartAnswerService = lessonPartAnswerService;
            _lessonProgressService = lessonProgressService;
            //_scoreService = scoreService;
            _roxyFilemanHandler = roxyFilemanHandler;
        }

        [Obsolete]
        [HttpGet]
        [HttpPost]
        public JsonResult GetListStudents(DefaultModel model, string ClassID)
        {
            if (!string.IsNullOrEmpty(model.ID))
            {
                var lessonid = model.ID;
                var lesson = _lessonService.GetItemByID(lessonid);
                if (lesson == null)
                {
                    return new JsonResult(new Dictionary<string, object>{
                    {"Error", "Bài kiểm tra không tồn tại" }
                    });
                }

                var list = _service.CreateQuery().AsQueryable()
                .Where(o => o.ClassID == ClassID && o.LessonID == lessonid)
                .GroupBy(o => new { o.StudentID, o.LessonID }).Select(r => new ExamEntity { ID = r.Max(t => t.ID), StudentID = r.Key.StudentID, LessonID = r.Key.LessonID }).ToList();
                var returnData = (from r in list
                                  let student = _studentService.GetItemByID(r.StudentID)
                                  let exam = _service.GetItemByID(r.ID)
                                  select new ExamViewModel
                                  {
                                      ID = r.ID,
                                      StudentID = student.ID,
                                      StudentName = student.FullName,
                                      Created = exam.Created,
                                      Status = exam.Status,
                                      Marked = exam.Marked,
                                      Point = exam.Point,
                                      MaxPoint = exam.MaxPoint,
                                      Number = exam.Number
                                  }).ToList();
                var response = new Dictionary<string, object>
                {
                    { "Data", returnData },
                    { "Model", model },
                    { "Error", null }
                };
                return new JsonResult(response);
            }
            else
            {
                // 1 - lay danh sach lesson 
                // 2 - lay danh sach student theo lesson
                // 3 - lay chi tiet bai 
                var list = _service.CreateQuery().Find(o => o.ClassID == ClassID)?.ToList()?
                    .GroupBy(o => new { o.LessonID }).Select(r => new ExamEntity { ID = r.Max(t => t.ID), LessonID = r.Key.LessonID })?.ToList();
                var returnData = (from r in list
                                  let exam = _service.GetItemByID(r.ID)
                                  let student = _studentService.GetItemByID(exam.StudentID)
                                  let lesson = _lessonService.GetItemByID(r.LessonID)
                                  select new ExamViewModel
                                  {
                                      LessonID = r.LessonID,
                                      LessonScheduleName = lesson.Title,
                                      ID = exam.ID,
                                      StudentID = student.ID,
                                      StudentName = student.FullName,
                                      Created = exam.Created,
                                      Status = exam.Status,
                                      Marked = exam.Marked,
                                      Point = exam.Point,
                                      MaxPoint = exam.MaxPoint,
                                      Number = exam.Number
                                  }).ToList()?.GroupBy(z => z.LessonID);
                model.TotalRecord = returnData.Count();



                var response = new Dictionary<string, object>
                {
                    { "Data", returnData },
                    { "Model", model },
                    { "Error", null }
                };
                return new JsonResult(response);
            }
        }

        [System.Obsolete]
        [HttpPost]
        [HttpGet]
        public JsonResult GetDetail(string ID, bool CheckPoint)
        {
            try
            {
                var exam = _service.GetItemByID(ID);
                var lesson = _lessonService.GetItemByID(exam.LessonID);
                var student = _studentService.GetItemByID(exam.StudentID);
                var currentClass = _classService.GetItemByID(exam.ClassID);
                var parts = _cloneLessonPartService.CreateQuery().Find(t => t.ParentID == lesson.ID).ToList();

                if (exam == null || lesson == null || student == null || currentClass == null)
                    return new JsonResult(new Dictionary<string, object>
                    {

                        { "Data", null },
                        {"Error", "Data Error" }
                    });
                var examdetails = _examDetailService.Collection.Find(o => o.ExamID == exam.ID)?.ToList();

                if (examdetails == null || examdetails.Count() == 0)
                {
                    return new JsonResult(new Dictionary<string, object>
                    {

                        { "Data", null },
                        {"Error", "No data" }
                    });
                }

                var _examdetails = examdetails.ToList();

                var mapping = new MappingEntity<ExamDetailEntity, TeacherExamDetailViewModel>();
                var partmapping = new MappingEntity<CloneLessonPartEntity, ExamPartViewModel>();


                var result = new MappingEntity<ExamEntity, TeacherExamViewModel>().AutoOrtherType(exam, new TeacherExamViewModel()
                {
                    StudentName = student.FullName,
                    ClassName = currentClass.Name,
                    LessonName = lesson.Title,
                    Multiple = lesson.Multiple,
                    TeacherName = _teacherService.GetItemByID(exam.TeacherID).FullName,
                    MarkDate = exam.Marked ? exam.Updated : DateTime.MinValue
                });

                result.Parts = (from p in parts
                                select partmapping.AutoOrtherType(p, new ExamPartViewModel()
                                {
                                    ExamDetails = (from r in _examdetails
                                                   where r.LessonPartID == p.ID
                                                   let answer = r.AnswerID != null ? _cloneLessonPartAnswerService.GetItemByID(r.AnswerID)
                                                       ?? _lessonPartAnswerService.GetItemByID(r.AnswerID) //TEMP
                                                       : null
                                                   let question = _cloneLessonPartQuestionService.GetItemByID(r.QuestionID)
                                                   ?? _lessonPartQuestionService.GetItemByID(r.QuestionID) //TEMP
                                                   where question != null
                                                   select
                                                   new TeacherExamDetailViewModel()
                                                   {
                                                       ID = r.ID,
                                                       LessonPartID = question.ParentID,
                                                       QuestionID = question.ID,
                                                       AnswerID = answer != null ? answer.ID : null,
                                                       AnswerValue = answer != null ? answer.Content : r.AnswerValue,
                                                       QuestionValue = question != null ? question.Content : r.QuestionValue,
                                                       RealAnswerValue = r.RealAnswerValue,
                                                       Point = r.Point,
                                                       MaxPoint = question.Point,
                                                   }
                                    ).ToList()
                                })).ToList();


                var response = new Dictionary<string, object>
                {
                    { "Data", result }
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

        public JsonResult GetLessonProgressList(string ID)
        {
            var result = new List<StudentLessonResultViewModel>();
            var lesson = _lessonService.GetItemByID(ID);
            if (lesson == null)
                return Json("No data");
            var listStudent = _studentService.GetStudentsByClassId(lesson.ClassID);
            if (listStudent != null && listStudent.Count() > 0)
            {
                foreach (var student in listStudent)
                {
                    var examresult = _service.CreateQuery().Find(t => t.StudentID == student.ID && t.LessonID == lesson.ID).SortByDescending(t => t.ID).ToList();
                    var progress = _lessonProgressService.GetByClassSubjectID_StudentID_LessonID(lesson.ClassSubjectID, student.ID, lesson.ID);
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

        public IActionResult Detail(DefaultModel model, string basis)
        {
            if (model == null) return null;
            var currentExam = _service.GetItemByID(model.ID);
            if (currentExam == null)
                return Redirect($"/{basis}{Url.Action("Index", "Class")}");
            var lesson = _lessonService.GetItemByID(currentExam.LessonID);
            ViewBag.Lesson = lesson;
            ViewBag.Class = _classService.GetItemByID(currentExam.ClassID);
            ViewBag.Exam = currentExam;
            return View();
        }
        [HttpPost]
        public JsonResult UpdatePoint([FromForm]string ID, [FromForm]string RealAnswerValue, [FromForm] double Point, string basis, [FromForm] bool isLast = false)
        {
            try
            {
                var oldItem = _examDetailService.GetItemByID(ID);
                if (isLast)
                {
                    var currentExam = _service.GetItemByID(oldItem.ExamID);
                    var point = 0.0;
                    var lesson = _lessonService.GetItemByID(currentExam.LessonID);
                    currentExam.Point = point;
                    currentExam.Marked = true;
                    currentExam = _service.CompleteFull(currentExam, lesson, out point);
                    return new JsonResult(new Dictionary<string, object>
                        {
                            { "Data", _service.GetItemByID(oldItem.ExamID) }
                        });
                }
                else
                {
                    //Dictionary<string, List<MediaResponseModel>> listFilesUpload = _roxyFilemanHandler.UploadAnswerBasis(basis, HttpContext);
                    List<MediaResponseModel> listFileUpload = _roxyFilemanHandler.UploadFileWithGoogleDrive(basis, User.FindFirst("UserID").Value, HttpContext);
                    if (listFileUpload.Count > 0)
                    {
                        var listMedia = new List<Media>();
                        for (int i = 0; i < listFileUpload.Count; i++)
                        {
                            var media = new Media()
                            {
                                Created = DateTime.Now,
                                Extension = listFileUpload[i].Extends,
                                Name = listFileUpload[i].Path,
                                OriginalName = listFileUpload[i].Path,
                                Path = FileManagerCore.Globals.Startup.GoogleDrive.CreateLinkViewFile(listFileUpload[i].Path)
                            };
                            listMedia.Add(media);
                        }
                        oldItem.MediasAnswers = listMedia;
                    }

                    oldItem.RealAnswerValue = RealAnswerValue;
                    oldItem.Point = Point;
                    _examDetailService.CreateOrUpdate(oldItem);
                }
                var response = new Dictionary<string, object>
                {
                    { "Data", oldItem }
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
    }
}
