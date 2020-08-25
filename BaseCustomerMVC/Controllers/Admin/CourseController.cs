using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Core_v2.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Core_v2.Globals;
using System.Threading.Tasks;
using BaseEasyRealTime.Entities;

namespace BaseCustomerMVC.Controllers.Admin
{
    [BaseAccess.Attribule.AccessCtrl("Quản lý lớp", "admin", 5)]
    public class CourseController : AdminController
    {
        private readonly CourseService _courseService;
        private readonly SubjectService _subjectService;
        private readonly GradeService _gradeService;
        private readonly ClassService _service;
        private readonly SkillService _skillService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly TeacherService _teacherService;
        private readonly StudentService _studentService;
        private readonly LessonService _lessonService;
        private readonly LearningHistoryService _learningHistoryService;
        
        //private readonly LessonPartService _lessonPartService;
        //private readonly LessonPartAnswerService _lessonPartAnswerService;
        //private readonly LessonPartQuestionService _lessonPartQuestionService;

        private readonly ExamService _examService;
        private readonly ExamDetailService _examDetailService;


        //private readonly CloneLessonPartService _cloneLessonPartService;
        //private readonly CloneLessonPartAnswerService _cloneAnswerService;
        //private readonly CloneLessonPartQuestionService _cloneQuestionService;

        //private readonly MappingEntity<LessonPartEntity, CloneLessonPartEntity> _lessonPartMapping;
        //private readonly MappingEntity<LessonPartQuestionEntity, CloneLessonPartQuestionEntity> _lessonPartQuestionMapping;
        //private readonly MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity> _lessonPartAnswerMapping;

        private readonly FileProcess _fileProcess;

        private readonly LessonScheduleService _lessonScheduleService;
        private readonly CalendarHelper _calendarHelper;
        //private readonly ClassStudentService _classStudentService;
        private readonly IHostingEnvironment _env;

        private readonly LessonHelper _lessonHelper;
        private readonly CenterService _centerService;
        private readonly AccountService _accountService;
        private readonly MailHelper _mailHelper;
        private readonly ChapterService _chapterService;
        private readonly CourseHelper _courseHelper;
        private readonly ClassProgressService _classProgressService;
        private readonly GroupService _groupService;
        private readonly TeacherHelper _teacherHelper;

        public CourseController(ClassService service,
            ClassSubjectService classSubjectService,
            SubjectService subjectService,
            GradeService gradeService,
            CourseService courseService,
            TeacherService teacherService,
            StudentService studentService,
            SkillService skillService,
            //ClassStudentService classStudentService,
            LessonService lessonService,
            LessonScheduleService lessonScheduleService,
            ExamService examService,
            ExamDetailService examDetailService,

            LessonPartService lessonPartService,
            LessonPartQuestionService lessonPartQuestionService,
            LessonPartAnswerService lessonPartAnswerService,
            LearningHistoryService learningHistoryService,

            CloneLessonPartService cloneLessonPartService,
            CloneLessonPartAnswerService cloneLessonPartAnswerService,
            CloneLessonPartQuestionService cloneLessonPartQuestionService,
            FileProcess fileProcess,
            CalendarHelper calendarHelper,
            CenterService centerService,
            AccountService accountService,
            MailHelper mailHelper,
            ChapterService chapterService,
            CourseHelper courseHelper,
            ClassProgressService classProgressService,
            GroupService groupService,
            TeacherHelper teacherHelper,
            IHostingEnvironment evn)
        {
            _service = service;
            _classSubjectService = classSubjectService;
            _courseService = courseService;
            _gradeService = gradeService;
            _skillService = skillService;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _studentService = studentService;
            _lessonService = lessonService;
            _lessonScheduleService = lessonScheduleService;
            _examService = examService;
            _examDetailService = examDetailService;
            _learningHistoryService = learningHistoryService;

            _calendarHelper = calendarHelper;

            _fileProcess = fileProcess;

            _lessonHelper = new LessonHelper(
               lessonService,
               lessonPartService,
               lessonPartQuestionService,
               lessonPartAnswerService,
               cloneLessonPartService,
               cloneLessonPartAnswerService,
               cloneLessonPartQuestionService
               );

            _centerService = centerService;
            _accountService = accountService;
            _mailHelper = mailHelper;
            _chapterService = chapterService;
            _courseHelper = courseHelper;
            _classProgressService = classProgressService;
            _groupService = groupService;
            _teacherHelper = teacherHelper;
            _env = evn;
        }

        public ActionResult Index(DefaultModel model)
        {
            ViewBag.Course = _courseService.GetAll()?.ToList();
            ViewBag.Subject = _subjectService.GetAll()?.ToList();
            ViewBag.Grade = _gradeService.GetAll()?.ToList();
            ViewBag.Teacher = _teacherService.GetAll()?.ToList();
            ViewBag.Centers = _centerService.GetAll().ToList();
            ViewBag.Model = model;
            return View();
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetList(DefaultModel model, string SubjectID = "", string GradeID = "")
        {
            //var filter = new List<FilterDefinition<ClassEntity>>();

            //if (!string.IsNullOrEmpty(model.SearchText))
            //{
            //    filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Name.ToLower().Contains(model.SearchText.ToLower())));
            //}
            //if (model.StartDate > DateTime.MinValue)
            //{
            //    filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Created >= new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0)));
            //}
            //if (model.EndDate > DateTime.MinValue)
            //{
            //    filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Created <= new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59)));
            //}
            //if (!string.IsNullOrEmpty(SubjectID))
            //{
            //    filter.Add(Builders<ClassEntity>.Filter.Where(o => o.SubjectID == SubjectID));
            //}
            //if (!string.IsNullOrEmpty(GradeID))
            //{
            //    filter.Add(Builders<ClassEntity>.Filter.Where(o => o.GradeID == GradeID));
            //}
            //var data = filter.Count > 0 ? _service.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _service.GetAll();
            //model.TotalRecord = data.Count();
            //var DataResponse = data == null || data.Count() <= 0 || data.Count() < model.PageSize
            //    ? data.ToList()
            //    : data.Skip(model.PageIndex * model.PageSize).Limit(model.PageSize).ToList();
            //var response = new Dictionary<string, object>
            //{
            //    { "Data", DataResponse.Select(o=> new ClassViewModel(o){
            //            CourseName = _courseService.GetItemByID(o.CourseID)?.Name,
            //            GradeName = _gradeService.GetItemByID(o.GradeID)?.Name,
            //            SubjectName = _subjectService.GetItemByID(o.SubjectID)?.Name,
            //            TeacherName = _teacherService.GetItemByID(o.TeacherID)?.FullName
            //        }).ToList() },
            //    { "Model", model }
            //};
            //return new JsonResult(response);
            var returndata = FilterClass(model, SubjectID, GradeID);

            var response = new Dictionary<string, object>
                {
                    { "Data", returndata},
                    { "Model", model }
                };
            return new JsonResult(response);

        }

        private List<Dictionary<string, object>> FilterClass(DefaultModel model, string SubjectID = "", string GradeID = "", string TeacherID = "", bool skipActive = true)
        {
            var filter = new List<FilterDefinition<ClassSubjectEntity>>();
            var classfilter = new List<FilterDefinition<ClassEntity>>();
            FilterDefinition<ClassEntity> ownerfilter = null;

            if (!string.IsNullOrEmpty(SubjectID))
            {
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.SubjectID == SubjectID));
            }
            else
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var teacher = _teacherService.GetItemByID(UserID);
                if (teacher == null)
                    return null;
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => teacher.Subjects.Contains(o.SubjectID)));
            }
            if (!string.IsNullOrEmpty(GradeID))
            {
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.GradeID == GradeID));
            }
            if (!string.IsNullOrEmpty(TeacherID))
            {
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.TeacherID == TeacherID));
                if (string.IsNullOrEmpty(SubjectID) && string.IsNullOrEmpty(GradeID))
                    ownerfilter = new FilterDefinitionBuilder<ClassEntity>().Where(o => o.TeacherID == TeacherID);
            }
            if (model.StartDate > new DateTime(1900, 1, 1))
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.EndDate >= model.StartDate));
            if (model.StartDate > new DateTime(1900, 1, 1))
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.StartDate <= model.EndDate));


            var data = _classSubjectService.Collection
                .Distinct(t => t.ClassID, filter.Count > 0 ? Builders<ClassSubjectEntity>.Filter.And(filter) : Builders<ClassSubjectEntity>.Filter.Empty).ToList();
            //filter by classsubject
            if (data.Count > 0)
            {
                if (ownerfilter != null)
                    classfilter.Add(
                        Builders<ClassEntity>.Filter.Or(ownerfilter,
                        Builders<ClassEntity>.Filter.Where(t => data.Contains(t.ID) && (t.IsActive || skipActive))));
                else
                    classfilter.Add(Builders<ClassEntity>.Filter.Where(t => data.Contains(t.ID) && (t.IsActive || skipActive)));
            }

            if (!string.IsNullOrEmpty(model.SearchText))
                classfilter.Add(Builders<ClassEntity>.Filter.Text("\"" + model.SearchText + "\""));


            var classResult = _service.Collection.Find(Builders<ClassEntity>.Filter.And(classfilter));
            model.TotalRecord = classResult.CountDocuments();
            var classData = classResult.SortByDescending(t => t.IsActive).ThenByDescending(t => t.StartDate).Skip(model.PageIndex * model.PageSize).Limit(model.PageSize).ToList();
            var returndata = from o in classData
                             let skillIDs = _classSubjectService.GetByClassID(o.ID).Select(t => t.SkillID).Distinct()
                             let creator = _teacherService.GetItemByID(o.TeacherID) //Todo: Fix
                             let sname = skillIDs == null ? "" : string.Join(", ", _skillService.GetList().Where(t => skillIDs.Contains(t.ID)).Select(t => t.Name).ToList())
                             select new Dictionary<string, object>
                                {
                                 { "ID", o.ID },
                                 { "Name", o.Name },
                                 { "Students",
                                     _studentService.CountByClass(o.ID)
                                     /*_classStudentService.GetClassStudents(o.ID).Count*/ },
                                 { "Created", o.Created },
                                 { "IsActive", o.IsActive },
                                 { "Image", o.Image },
                                 { "StartDate", o.StartDate },
                                 { "EndDate", o.EndDate },
                                 { "Order", o.Order },
                                 { "Skills", o.Skills },
                                 { "Members", o.Members },
                                 { "Description", o.Description },
                                 { "SkillName", sname },
                                 { "Creator", o.TeacherID },
                                 { "CreatorName", creator.FullName }
                             };
            return returndata.ToList();
        }


        [Obsolete]
        [HttpPost]
        public JsonResult GetCourse(string GradeID, string SubjectID)
        {
            var filter = new List<FilterDefinition<CourseEntity>>();
            if (!string.IsNullOrEmpty(GradeID))
            {
                filter.Add(Builders<CourseEntity>.Filter.Where(o => o.GradeID == GradeID));
            }
            if (!string.IsNullOrEmpty(SubjectID))
            {
                filter.Add(Builders<CourseEntity>.Filter.Where(o => o.SubjectID == SubjectID));
            }
            filter.Add(Builders<CourseEntity>.Filter.Where(o => o.IsActive == true));
            var data = filter.Count > 0 ? _courseService.Collection.Find(Builders<CourseEntity>.Filter.And(filter)) : _courseService.GetAll();

            var DataResponse = data.ToList();
            return new JsonResult(DataResponse);
        }

        [System.Obsolete]
        [HttpPost]
        public JsonResult GetDetails(string id)
        {
            var filter = Builders<ClassEntity>.Filter.Where(o => o.ID == id);
            var data = _service.Collection.Find(filter);
            var DataResponse = data == null || data.Count() <= 0 ? null : data.First();
            var response = new Dictionary<string, object>
            {
                { "Data", DataResponse }
            };
            return new JsonResult(response);

        }

        [HttpPost]
        [Obsolete]
        public JsonResult Create(ClassEntity item, List<ClassSubjectEntity> ClassSubjects)
        {
            throw new Exception("not test");

            //if (string.IsNullOrEmpty(item.ID) || item.ID == "0")
            //{
            //    item.ID = null;
            //    item.Created = DateTime.Now;

            //    if (!String.IsNullOrEmpty(item.Code))
            //    {
            //        if (_service.CreateQuery().Find(t => t.Code == item.Code).FirstOrDefault() != null)
            //            return new JsonResult(new Dictionary<string, object>()
            //            {
            //                {"Error", "Mã môn học đã được sử dụng" }
            //            });
            //    }

            //    _service.CreateOrUpdate(item);

            //    if (ClassSubjects != null && ClassSubjects.Count > 0)
            //    {
            //        foreach (var csubject in ClassSubjects)
            //        {
            //            var subject = _subjectService.GetItemByID(csubject.SubjectID);
            //            if (subject == null) continue;
            //            var course = _courseService.GetItemByID(item.CourseID);
            //            if (course == null || !course.IsActive)
            //                return new JsonResult(new Dictionary<string, object>()
            //            {
            //                {"Error", "Curriculum for " + subject.Name + " is not available" }
            //            });
            //            csubject.ClassID = item.ID;
            //            csubject.Description = course.Description;
            //            csubject.LearningOutcomes = course.LearningOutcomes;
            //            //subject.Image = course.Image;
            //            _classSubjectService.CreateOrUpdate(csubject);

            //            //Create Class => Create Lesson Schedule & Clone all lesson
            //            var lessons = _lessonService.CreateQuery().Find(o => o.CourseID == item.CourseID).ToList();

            //            var schedules = new List<LessonScheduleEntity>();
            //            if (lessons != null)
            //                foreach (LessonEntity lesson in lessons)
            //                {
            //                    _lessonScheduleService.CreateQuery().InsertOne(new LessonScheduleEntity
            //                    {
            //                        ClassID = item.ID,
            //                        ClassSubjectID = csubject.ID,
            //                        LessonID = lesson.ID,
            //                        IsActive = true
            //                    });
            //                    _lessonHelper.CloneLessonForClass(lesson, csubject);
            //                }
            //            _courseService.Collection.UpdateOneAsync(t => t.ID == item.CourseID, new UpdateDefinitionBuilder<CourseEntity>().Set(t => t.IsUsed, true));
            //        }
            //    }

            //    Dictionary<string, object> response = new Dictionary<string, object>()
            //    {
            //        {"Data",item },
            //        {"Error",null },
            //        {"Msg","Thêm thành công" }
            //    };
            //    return new JsonResult(response);
            //}
            //else
            //{
            //    var oldData = _service.GetItemByID(item.ID);
            //    if (oldData == null) return new JsonResult(new Dictionary<string, object>()
            //    {
            //        {"Error", "Class not found" }
            //    });
            //    if (item.Code != oldData.Code)
            //    {
            //        if (_service.CreateQuery().Find(t => t.Code == item.Code).FirstOrDefault() != null)
            //            return new JsonResult(new Dictionary<string, object>()
            //            {
            //                {"Error", "Mã môn học đã được sử dụng" }
            //            });
            //    }

            //    oldData.Updated = DateTime.Now;
            //    _service.CreateOrUpdate(oldData);

            //    if (ClassSubjects != null && ClassSubjects.Count > 0)
            //    {
            //        foreach (var csubject in ClassSubjects)
            //        {
            //            var subject = _subjectService.GetItemByID(csubject.SubjectID);
            //            if (subject == null) continue;
            //            var course = _courseService.GetItemByID(item.CourseID);
            //            if (course == null || !course.IsActive)
            //                return new JsonResult(new Dictionary<string, object>()
            //            {
            //                {"Error", "Curriculum for " + subject.Name + " is not available" }
            //            });
            //            csubject.ClassID = item.ID;
            //            csubject.Description = course.Description;
            //            csubject.LearningOutcomes = course.LearningOutcomes;
            //            //subject.Image = course.Image;
            //            _classSubjectService.CreateOrUpdate(csubject);

            //            //Create Class => Create Lesson Schedule & Clone all lesson
            //            var lessons = _lessonService.CreateQuery().Find(o => o.CourseID == item.CourseID).ToList();

            //            var schedules = new List<LessonScheduleEntity>();
            //            if (lessons != null)
            //                foreach (LessonEntity lesson in lessons)
            //                {
            //                    _lessonScheduleService.CreateQuery().InsertOne(new LessonScheduleEntity
            //                    {
            //                        ClassID = item.ID,
            //                        ClassSubjectID = csubject.ID,
            //                        LessonID = lesson.ID,
            //                        IsActive = true
            //                    });
            //                    _lessonHelper.CloneLessonForClass(lesson, csubject);
            //                }
            //            _courseService.Collection.UpdateOneAsync(t => t.ID == item.CourseID, new UpdateDefinitionBuilder<CourseEntity>().Set(t => t.IsUsed, true));
            //        }
            //    }
            //    else
            //    {
            //        TODO: Replace with classSubject
            //        remove old schedule
            //        _lessonScheduleService.CreateQuery().DeleteMany(o => o.ClassID == item.ID);
            //        remove clone lesson part
            //        _lessonHelper.RemoveClone(item.ID);
            //        remove progress: learning history => class progress, chapter progress, lesson progress
            //        _learningHistoryService.RemoveClassHistory(item.ID);
            //        resest exam
            //        _examService.RemoveClassExam(item.ID);
            //        remove old subject
            //        var oldSubjects = _classSubjectService.RemoveClassSubjects(item.ID);
            //        _lessonHelper.RemoveClone(item.ID);
            //    }



            //    if (item.CourseID != oldData.CourseID)
            //    {
            //        //remove old schedule
            //        _lessonScheduleService.CreateQuery().DeleteMany(o => o.ClassID == item.ID);
            //        //remove clone lesson part
            //        _lessonHelper.RemoveClone(item.ID);
            //        //remove progress: learning history => class progress, chapter progress, lesson progress
            //        _learningHistoryService.RemoveClassHistory(item.ID);
            //        //resest exam
            //        _examService.RemoveClassExam(item.ID);

            //        //Create Class => Create Lesson Schedule & Clone all lesson
            //        var lessons = _lessonService.CreateQuery().Find(o => o.CourseID == item.CourseID).ToList();

            //        if (lessons != null)
            //            foreach (LessonEntity lesson in lessons)
            //            {
            //                var schedule = new LessonScheduleEntity
            //                {
            //                    ClassID = item.ID,
            //                    LessonID = lesson.ID,
            //                    IsActive = true
            //                };
            //                _lessonScheduleService.CreateOrUpdate(schedule);
            //                //_calendarHelper.ConvertCalendarFromSchedule(schedule, "");

            //                _lessonHelper.CloneLessonForClass(lesson, item);
            //            }
            //    }

            //    oldData.Name = item.Name;
            //    oldData.Code = item.Code;
            //    oldData.StartDate = item.StartDate;
            //    oldData.EndDate = item.EndDate;
            //    oldData.CourseID = item.CourseID;
            //    oldData.GradeID = item.GradeID;
            //    oldData.TeacherID = item.TeacherID;



            //    _courseService.Collection.UpdateOneAsync(t => t.ID == item.CourseID, new UpdateDefinitionBuilder<CourseEntity>().Set(t => t.IsUsed, true));

            //    Dictionary<string, object> response = new Dictionary<string, object>()
            //    {
            //        {"Data",item },
            //        {"Error",null },
            //        {"Msg","Cập nhập thành công" }
            //    };
            //    return new JsonResult(response);
            //}
        }

        [HttpPost]
        [Obsolete]
        public JsonResult Delete(DefaultModel model)
        {
            if (model.ArrID.Length <= 0)
            {
                return new JsonResult(
                    new Dictionary<string, object>
                    {
                        { "Error", "Nothing to Delete" }
                    }
                );
            }
            else
            {
                var ids = model.ArrID.Split(',');
                if (ids.Length > 0)
                {
                    //remove Schedule, Part, Question, Answer
                    _ = _lessonScheduleService.RemoveManyClass(ids);
                    //CreateQuery().DeleteMany(o => ids.Contains(o.ClassID));
                    _ = _lessonHelper.RemoveClone(ids);
                    _ = _examService.RemoveManyClassExam(ids);
                    //Collection.DeleteMany(o => ids.Contains(o.ClassID));
                    //_examDetailService.Collection.DeleteMany(o => ids.Contains(o.ClassID));
                    var delete = _service.Collection.DeleteMany(o => ids.Contains(o.ID));
                    return new JsonResult(delete);
                }
                else
                {
                    return new JsonResult(
                       new Dictionary<string, object>
                       {
                            { "Error", "Nothing to Delete" }
                       }
                    );
                }
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
                    var filter = Builders<ClassEntity>.Filter.Where(o => model.ArrID.Split(',').Contains(o.ID) && o.IsActive == false);
                    var update = Builders<ClassEntity>.Update.Set("IsActive", true);
                    var publish = _service.Collection.UpdateMany(filter, update);
                    return new JsonResult(publish);
                }
                else
                {
                    var filter = Builders<ClassEntity>.Filter.Where(o => model.ArrID == o.ID && o.IsActive == false);
                    var update = Builders<ClassEntity>.Update.Set("IsActive", true);
                    var publish = _service.Collection.UpdateMany(filter, update);
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
                    var filter = Builders<ClassEntity>.Filter.Where(o => model.ArrID.Split(',').Contains(o.ID) && o.IsActive == true);
                    var update = Builders<ClassEntity>.Update.Set("IsActive", false);
                    var publish = _service.Collection.UpdateMany(filter, update);
                    return new JsonResult(publish);
                }
                else
                {
                    var filter = Builders<ClassEntity>.Filter.Where(o => model.ArrID == o.ID && o.IsActive == true);
                    var update = Builders<ClassEntity>.Update.Set("IsActive", false);
                    var publish = _service.Collection.UpdateMany(filter, update);
                    return new JsonResult(publish);
                }


            }
        }

        [HttpPost]
        [Obsolete]
        public async Task<JsonResult> Import(string ID)
        {
            var form = HttpContext.Request.Form;
            if (string.IsNullOrEmpty(ID)) return new JsonResult("Fail");
            var itemCourse = _service.GetItemByID(ID);
            if (itemCourse == null) return new JsonResult("Fail");
            if (itemCourse.Students == null) itemCourse.Students = new List<string>();
            if (form == null) return new JsonResult(null);
            if (form.Files == null || form.Files.Count <= 0) return new JsonResult(null);
            var file = form.Files[0];
            var filePath = Path.Combine(_env.WebRootPath, itemCourse.ID + "_" + file.FileName + DateTime.Now.ToString("ddMMyyyyhhmmss"));
            List<StudentEntity> studentList = null;
            var defCenter = _centerService.GetItemByCode("eduso");
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
                stream.Close();
                try
                {
                    using (var readStream = new FileStream(filePath, FileMode.Open))
                    {
                        using (ExcelPackage package = new ExcelPackage(readStream))
                        {
                            ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                            int totalRows = workSheet.Dimension.Rows;
                            studentList = new List<StudentEntity>();
                            for (int i = 1; i <= totalRows; i++)
                            {
                                if (workSheet.Cells[i, 1].Value == null || workSheet.Cells[i, 1].Value.ToString() == "STT") continue;
                                var studentEmail = workSheet.Cells[i, 5].Value == null ? "" : workSheet.Cells[i, 5].Value.ToString();
                                var student = _studentService.CreateQuery().Find(o => o.Email == studentEmail).SingleOrDefault();
                                if (student != null)
                                    studentList.Add(student);
                            }

                            var listID = studentList.Select(o => o.ID);
                            itemCourse.Students.AddRange(listID);
                            itemCourse.Students = itemCourse.Students.Distinct().ToList();
                            foreach (var student in itemCourse.Students)
                            {
                                _studentService.JoinClass(itemCourse.ID, student, defCenter.ID);
                            }
                            _service.CreateQuery().ReplaceOne(o => o.ID == itemCourse.ID, itemCourse);
                        }
                    }
                    System.IO.File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    return new JsonResult(ex);
                }
            }
            Dictionary<string, object> response = new Dictionary<string, object>()
            {
                {"Data",studentList}
            };
            return new JsonResult(response);
        }


        public JsonResult FixDefCenter()
        {
            var defCenter = _centerService.GetItemByCode("eduso");
            var defCenters = new List<string> { defCenter.ID };
            _studentService.CreateQuery().UpdateMany(t => true, Builders<StudentEntity>.Update.Set(t => t.Centers, defCenters));//student
            _service.CreateQuery().UpdateMany(t => true, Builders<ClassEntity>.Update.Set(t => t.Center, defCenter.ID));//class
            _courseService.CreateQuery().UpdateMany(t => true, Builders<CourseEntity>.Update.Set(t => t.Center, defCenter.ID));//course
            var defCenterMember = new List<CenterMemberEntity> { new CenterMemberEntity { CenterID = defCenter.ID, Code = defCenter.Code, Name = defCenter.Name } };
            var teachers = _teacherService.GetAll().ToList();
            var teacherrole = "5d808d08cf9a821bdc9daf7b";
            var headrole = "5d808d08cf9a821bdc9daf79";
            foreach (var teacher in teachers)
            {
                if (teacher.Centers != null && teacher.Centers.Count > 0) continue;
                var acc = _accountService.GetAccountByEmail(teacher.Email);
                if (acc == null) continue;
                teacher.Centers = defCenterMember;
                if (acc.Type == "teacher")
                    teacher.Centers[0].RoleID = teacherrole;
                else
                    teacher.Centers[0].RoleID = headrole;
                _teacherService.Save(teacher);
            }
            return Json("OK");
        }

        #region Hoa them

        private async Task RemoveClassSubject(ClassSubjectEntity cs)
        {
            ////remove old schedule
            var CsTask = _lessonScheduleService.RemoveClassSubject(cs.ID);
            //remove chapter
            var CtTask = _chapterService.RemoveClassSubjectChapter(cs.ID);
            //remove clone lesson
            var LsTask = _lessonHelper.RemoveClassSubjectLesson(cs.ID);
            //remove progress: learning history => class progress, chapter progress, lesson progress
            var LhTask = _learningHistoryService.RemoveClassSubjectHistory(cs.ID);
            //remove exam
            var ExTask = _examService.RemoveClassSubjectExam(cs.ID);
            //remove classSubject
            //await Task.WhenAll(CsTask, CtTask, LsTask, LhTask, ExTask);
            await _classSubjectService.RemoveAsync(cs.ID);
        }

        public JsonResult Clone(ClassEntity item, List<ClassSubjectEntity> classSubjects, IFormFile fileUpload,string CenterCode)
        {
            var center = _centerService.GetItemByCode(CenterCode);
            var teachers = _teacherService.GetAll();
            List<TeacherEntity> tearcherHead = new List<TeacherEntity>();
            foreach(var teacher in teachers.ToList())
            {
                if (teacher.Centers != null && teacher.Centers.Find(x => x.CenterID == center.ID) != null )
                    tearcherHead.Add(teacher);
            }

            //var tearcherHead = _teacherService.GetAll();
            if (center == null)
            {
                return new JsonResult(new Dictionary<string, object>()
                        {
                            {"Error", "Cơ sở không đúng" }
                        });
            }
            var processCS = new List<string>();
            var oldData = _service.GetItemByID(item.ID);
            
            var newData = new MappingEntity<ClassEntity, ClassEntity>().Clone(oldData, new ClassEntity());
            newData.ID = null;
            newData.OriginID = oldData.ID;
            newData.Created = DateTime.Now;
            newData.Center = center.ID;
            newData.Name = item.Name;
            newData.Skills = new List<string>();
            newData.Subjects = new List<string>();
            foreach (var teacher in tearcherHead)
            {
                if (_teacherHelper.HasRole(teacher.ID, center.ID, "head-teacher"))
                {
                    newData.TeacherID = teacher.ID;
                    break;
                }
            }
            var _teacher = _teacherService.GetItemByID(newData.TeacherID);
            newData.Members = new List<ClassMemberEntity> { new ClassMemberEntity { TeacherID = _teacher.ID, Type = ClassMemberType.OWNER, Name = _teacher.FullName } };
            //newData.Members.Add(new ClassMemberEntity { TeacherID = _teacher.ID, Type = ClassMemberType.TEACHER, Name = _teacher.FullName });

            //if (fileUpload != null)
            //{
            //    var pathImage = _fileProcess.SaveMediaAsync(fileUpload, "", "CLASSIMG", center.Code).Result;
            //    oldData.Image = pathImage;
            //}

            //update data
            _service.CreateQuery().InsertOne(newData);

            classSubjects = _classSubjectService.CreateQuery().Find(o => o.ClassID == oldData.ID).ToList();
            if (classSubjects != null && classSubjects.Count > 0)
            {
                foreach (var csubject in classSubjects)
                {
                    var teacher = _teacherService.GetItemByID(newData.TeacherID);
                    var ncbj = new ClassSubjectEntity();
                    ncbj.CourseID = csubject.CourseID;
                    ncbj.GradeID = csubject.GradeID;
                    ncbj.SubjectID = csubject.SubjectID;
                    ncbj.TeacherID = teacher.ID;
                    var newMember = new ClassMemberEntity();
                    long lessoncount = 0;
                    //csubject.TeacherID = teacher.ID;
                    var nID = CreateNewClassSubject(ncbj, newData, out newMember, out lessoncount);
                    if (!newData.Skills.Contains(csubject.SkillID))
                        newData.Skills.Add(csubject.SkillID);
                    if (!newData.Subjects.Contains(csubject.SubjectID))
                        newData.Subjects.Add(csubject.SubjectID);
                    if (!newData.Members.Any(t => t.TeacherID == newMember.TeacherID && t.Type == ClassMemberType.TEACHER))
                        newData.Members.Add(newMember);
                    //var skill = _skillService.GetItemByID(csubject.SkillID);
                    //if (skill == null) continue;
                    //_ = _mailHelper.SendTeacherJoinClassNotify(teacher.FullName, teacher.Email, item.Name, skill.Name, item.StartDate, item.EndDate, center.Name);
                }
                _service.Save(newData);
            }
            //if (mustUpdateName)
            //{
            //    var change = _groupService.UpdateGroupDisplayName(oldData.ID, oldData.Name);
            //}

            //refresh class total lesson => no need
            //_ = _classProgressService.RefreshTotalLessonForClass(oldData.ID);

            Dictionary<string, object> response = new Dictionary<string, object>()
                {
                    {"Data",item },
                    {"Error",null },
                    {"Msg","Success" }
                };
            return new JsonResult(response);
        }

        private string CreateNewClassSubject(ClassSubjectEntity nSbj, ClassEntity @class, out ClassMemberEntity member, out long lessoncount)
        {
            member = new ClassMemberEntity();
            lessoncount = 0;
            try
            {
                var subject = _subjectService.GetItemByID(nSbj.SubjectID);
                if (subject == null)
                {
                    throw new Exception("Subject " + nSbj.SubjectID + " is not avaiable");
                }
                var course = _courseService.GetItemByID(nSbj.CourseID);
                if (course == null || !course.IsActive)
                {
                    throw new Exception("Course " + nSbj.CourseID + " is not avaiable");
                }

                lessoncount = course.TotalLessons;

                var teacher = _teacherService.GetItemByID(nSbj.TeacherID);
                if (teacher == null || !teacher.IsActive || !teacher.Subjects.Contains(nSbj.SubjectID))
                {
                    //throw new Exception("Teacher " + nSbj.TeacherID + " is not avaiable");
                    teacher.Subjects.Add(nSbj.ID);
                }

                nSbj.ClassID = @class.ID;
                nSbj.StartDate = @class.StartDate;
                nSbj.EndDate = @class.EndDate;

                nSbj.SkillID = course.SkillID;
                nSbj.Description = course.Description;
                nSbj.LearningOutcomes = course.LearningOutcomes;
                nSbj.TotalLessons = course.TotalLessons;

                var skill = _skillService.GetItemByID(nSbj.SkillID);

                var center = _centerService.GetItemByID(@class.Center);

                _classSubjectService.Save(nSbj);
                //_ = _mailHelper.SendTeacherJoinClassNotify(teacher.FullName, teacher.Email, @class.Name, skill?.Name, @class.StartDate, @class.EndDate, center.Name);
                
                //Clone Course
                _courseHelper.CloneForClassSubject(nSbj);

                member = new ClassMemberEntity
                {
                    Name = teacher.FullName,
                    TeacherID = teacher.ID,
                    Type = ClassMemberType.TEACHER
                };
                return nSbj.ID;
            }
            catch (Exception e)
            {
                return "";
            }
        }

        #endregion
    }
}