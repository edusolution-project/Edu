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
using System.Threading.Tasks;

namespace BaseCustomerMVC.Controllers.Admin
{
    [IndefindCtrlAttribulte("Quản lý khóa học", "ClassManagement", "admin")]
    public class CourseController : AdminController
    {
        private readonly CourseService _courseService;
        private readonly SubjectService _subjectService;
        private readonly GradeService _gradeService;
        private readonly ClassService _service;
        private readonly TeacherService _teacherService;
        private readonly StudentService _studentService;
        private readonly LessonService _lessonService;
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly ClassProgressService _classProgressService;
        private readonly IHostingEnvironment _env;



        public CourseController(ClassService service
            , SubjectService subjectService
            , GradeService gradeService
            , CourseService courseService
            , TeacherService teacherService
            , StudentService studentService
            , LessonService lessonService
            , LessonScheduleService lessonScheduleService
            , ClassProgressService classProgressService
            , IHostingEnvironment evn)
        {
            _service = service;
            _courseService = courseService;
            _gradeService = gradeService;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _studentService = studentService;
            _lessonService = lessonService;
            _lessonScheduleService = lessonScheduleService;
            _classProgressService = classProgressService;
            _env = evn;
        }

        public ActionResult Index(DefaultModel model)
        {
            ViewBag.Course = _courseService.GetAll()?.ToList();
            ViewBag.Subject = _subjectService.GetAll()?.ToList();
            ViewBag.Grade = _gradeService.GetAll()?.ToList();
            ViewBag.Teacher = _teacherService.GetAll()?.ToList();
            ViewBag.Model = model;
            return View();
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetList(DefaultModel model, string SubjectID = "", string GradeID = "")
        {
            var filter = new List<FilterDefinition<ClassEntity>>();

            if (!string.IsNullOrEmpty(model.SearchText))
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Name.ToLower().Contains(model.SearchText.ToLower())));
            }
            if (model.StartDate > DateTime.MinValue)
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Created >= new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0)));
            }
            if (model.EndDate > DateTime.MinValue)
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Created <= new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59)));
            }
            if (!string.IsNullOrEmpty(SubjectID))
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.SubjectID == SubjectID));
            }
            if (!string.IsNullOrEmpty(GradeID))
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.GradeID == GradeID));
            }
            var data = filter.Count > 0 ? _service.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _service.GetAll();
            model.TotalRecord = data.Count();
            var DataResponse = data == null || data.Count() <= 0 || data.Count() < model.PageSize
                ? data.ToList()
                : data.Skip((model.PageIndex - 1) * model.PageSize).Limit(model.PageSize).ToList();
            var response = new Dictionary<string, object>
            {
                { "Data", DataResponse.Select(o=> new ClassViewModel(o){
                        CourseName = _courseService.GetItemByID(o.CourseID)?.Name,
                        GradeName = _gradeService.GetItemByID(o.GradeID)?.Name,
                        SubjectName = _subjectService.GetItemByID(o.SubjectID)?.Name,
                        TeacherName = _teacherService.GetItemByID(o.TeacherID)?.FullName
                    }).ToList() },
                { "Model", model }
            };
            return new JsonResult(response);

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
        public JsonResult Create(ClassEntity item)
        {
            if (string.IsNullOrEmpty(item.ID) || item.ID == "0")
            {
                item.ID = null;
                item.Created = DateTime.Now;
                _service.CreateQuery().InsertOne(item);


                //Create Class => Create Lesson Schedule & Class Progress
                var lessons = _lessonService.CreateQuery().Find(o => o.CourseID == item.CourseID).ToList();

                //Create Lesson Schedule
                var schedules = new List<LessonScheduleEntity>();
                if (lessons != null)
                    foreach (LessonEntity lesson in lessons)
                    {
                        _lessonScheduleService.CreateQuery().InsertOne(new LessonScheduleEntity
                        {
                            ClassID = item.ID,
                            LessonID = lesson.ID,
                            StartDate = item.StartDate,
                            EndDate = item.EndDate
                        });
                    }
                //Create Class Progress
                var classProgress = new ClassProgressEntity()
                {
                    ClassID = item.ID,
                    TotalLessons = lessons.Count,
                    CompletedLessons = 0,
                    LastDate = DateTime.Now
                };
                _classProgressService.CreateQuery().InsertOne(classProgress);

                Dictionary<string, object> response = new Dictionary<string, object>()
                {
                    {"Data",item },
                    {"Error",null },
                    {"Msg","Thêm thành công" }
                };
                return new JsonResult(response);
            }
            else
            {
                var oldData = _service.GetItemByID(item.ID);
                if (oldData == null) return new JsonResult(null);
                item.Updated = DateTime.Now;
                _service.CreateQuery().ReplaceOne(o => o.ID == item.ID, item);

                if (item.CourseID != oldData.CourseID)
                {
                    //remove old schedule & progress

                    _lessonScheduleService.CreateQuery().DeleteMany(o => o.ClassID == item.ID);
                    _classProgressService.CreateQuery().DeleteMany(o => o.ClassID == item.ID);

                    var lessons = _lessonService.CreateQuery().Find(o => o.CourseID == item.CourseID).ToList();

                    //Create Lesson Schedule
                    var schedules = new List<LessonScheduleEntity>();
                    if (lessons != null)
                        foreach (LessonEntity lesson in lessons)
                        {
                            _lessonScheduleService.CreateQuery().InsertOne(new LessonScheduleEntity
                            {
                                ClassID = item.ID,
                                LessonID = lesson.ID,
                                StartDate = item.StartDate,
                                EndDate = item.EndDate
                            });
                        }
                    //Create Class Progress
                    var classProgress = new ClassProgressEntity()
                    {
                        ClassID = item.ID,
                        TotalLessons = lessons.Count,
                        CompletedLessons = 0,
                        LastDate = DateTime.Now
                    };
                    _classProgressService.CreateQuery().InsertOne(classProgress);
                }

                Dictionary<string, object> response = new Dictionary<string, object>()
                {
                    {"Data",item },
                    {"Error",null },
                    {"Msg","Cập nhập thành công" }
                };
                return new JsonResult(response);
            }
        }

        [HttpPost]
        [Obsolete]
        public JsonResult Delete(DefaultModel model)
        {
            if (model.ArrID.Length <= 0)
            {
                return new JsonResult(null);
            }
            else
            {
                //TODO: Clear chapter, Lesson & Lesson part on delete Course
                if (model.ArrID.Contains(","))
                {

                    var delete = _service.Collection.DeleteMany(o => model.ArrID.Split(',').Contains(o.ID));
                    return new JsonResult(delete);
                }
                else
                {
                    var delete = _service.Collection.DeleteMany(o => model.ArrID == o.ID);
                    return new JsonResult(delete);
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
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
                stream.Close();
                try
                {
                    var readStream = new FileStream(filePath, FileMode.Open);
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
                        _service.CreateQuery().ReplaceOne(o => o.ID == itemCourse.ID, itemCourse);
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

    }
}
