using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Core_v2.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseCustomerMVC.Controllers.Student
{
    public class MyCourseController : StudentController
    {
        private readonly ClassService _service;
        private readonly CourseService _courseService;
        private readonly TeacherService _teacherService;
        private readonly SubjectService _subjectService;
        private readonly GradeService _gradeService;

        public MyCourseController(ClassService service
            , CourseService courseService
            , TeacherService teacherService
            , SubjectService subjectService
            , GradeService gradeService
            )
        {
            _service = service;
            _courseService = courseService;
            _teacherService = teacherService;
            _subjectService = subjectService;
            _gradeService = gradeService;
        }
        [Obsolete]
        [HttpPost]
        public JsonResult GetList(DefaultModel model,string TeacherID)
        {
            var filter = new List<FilterDefinition<ClassEntity>>();

            if (!string.IsNullOrEmpty(model.SearchText))
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Name.ToLower().Contains(model.SearchText.ToLower())));
            }
            if (model.StartDate > DateTime.MinValue)
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.StartDate >= new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0)));
            }
            if (model.EndDate > DateTime.MinValue)
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.EndDate <= new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59)));
            }
            var data = filter.Count > 0 ? _service.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _service.GetAll();
            model.TotalRecord = data.Count();
            var DataResponse = data == null || data.Count() <= 0 || data.Count() < model.PageSize
                ? data.ToList()
                : data.Skip((model.PageIndex - 1) * model.PageSize).Limit(model.PageSize).ToList();
            var respone = new Dictionary<string, object>
            {
                { "Data", DataResponse.Select(o=> new MyClassViewModel(){
                        ID = o.ID,
                        CourseID = o.CourseID,
                        TeacherID = o.TeacherID,
                        Status = o.IsActive,
                        EndDate = o.EndDate,
                        StartDate = o.StartDate,
                        Name = o.Name,
                        CourseName = _courseService.GetItemByID(o.CourseID) == null ? "" :  _courseService.GetItemByID(o.CourseID).Name,
                        StudentNumber = o.Students.Count,
                        SubjectName = _subjectService.GetItemByID(o.SubjectID) == null ? "":_subjectService.GetItemByID(o.SubjectID).Name,
                        GradeName = _gradeService.GetItemByID(o.GradeID) == null ? "":_gradeService.GetItemByID(o.GradeID).Name,
                        TeacherName = _teacherService.GetItemByID(o.TeacherID) == null ? "" :_teacherService.GetItemByID(o.TeacherID).FullName
                    }) 
                },
                { "Model", model }
            };
            return new JsonResult(respone);
        }


        [System.Obsolete]
        [HttpPost]
        public JsonResult GetDetails(string CourseID)
        {
            var filter = Builders<CourseEntity>.Filter.Where(o => o.ID == CourseID);
            var data = _courseService.Collection.Find(filter);
            var DataResponse = data == null || data.Count() <= 0 ? null : data.First();
            var respone = new Dictionary<string, object>
            {
                { "Data", DataResponse }
            };
            return new JsonResult(respone);

        }

        [System.Obsolete]
        [HttpPost]
        public JsonResult GetLesson(string CourseID)
        {
            var filter = Builders<CourseEntity>.Filter.Where(o => o.ID == CourseID);
            var data = _courseService.Collection.Find(filter);
            var DataResponse = data == null || data.Count() <= 0 ? null : data.First();
            var respone = new Dictionary<string, object>
            {
                { "Data", DataResponse }
            };
            return new JsonResult(respone);

        }
        [System.Obsolete]
        [HttpPost]
        public JsonResult GetPart(string CourseID)
        {
            var filter = Builders<CourseEntity>.Filter.Where(o => o.ID == CourseID);
            var data = _courseService.Collection.Find(filter);
            var DataResponse = data == null || data.Count() <= 0 ? null : data.First();
            var respone = new Dictionary<string, object>
            {
                { "Data", DataResponse }
            };
            return new JsonResult(respone);

        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult StudentCalendar()
        {
            return View();
        }

    }
}
