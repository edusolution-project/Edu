﻿using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using MongoDB.Driver;
using System.Text;
using System.Linq;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class ClassController : TeacherController
    {
        private readonly GradeService _gradeService;
        private readonly SubjectService _subjectService;
        private readonly TeacherService _teacherService;
        private readonly ClassService _service;
        private readonly CourseService _courseService;
        private readonly ChapterService _chapterService;
        private readonly LessonService _lessonService;
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly StudentService _studentService;

        public ClassController(GradeService gradeservice
           , SubjectService subjectService, TeacherService teacherService, ClassService service,
            CourseService courseService, ChapterService chapterService, LessonService lessonService, LessonScheduleService lessonScheduleService,
            StudentService studentService)
        {
            _gradeService = gradeservice;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _courseService = courseService;
            _service = service;
            _chapterService = chapterService;
            _lessonService = lessonService;
            _lessonScheduleService = lessonScheduleService;
            _studentService = studentService;
        }

        public IActionResult Index(DefaultModel model)
        {
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            var teacher = User.IsInRole("teacher") ? _teacherService.CreateQuery().Find(t => t.ID == UserID).SingleOrDefault() : new TeacherEntity();

            var subject = new List<SubjectEntity>();
            var grade = new List<GradeEntity>();

            if (teacher != null && teacher.Subjects != null)
            {
                subject = _subjectService.CreateQuery().Find(t => teacher.Subjects.Contains(t.ID)).ToList();
                grade = _gradeService.CreateQuery().Find(t => teacher.Subjects.Contains(t.SubjectID)).ToList();
            }
            ViewBag.Grade = grade;
            ViewBag.Subject = subject;

            ViewBag.Model = model;
            return View();
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetList(DefaultModel model, string SubjectID = "", string GradeID = "", string UserID = "")
        {
            var filter = new List<FilterDefinition<ClassEntity>>();
            TeacherEntity teacher = null;
            if (string.IsNullOrEmpty(UserID))
                UserID = User.Claims.GetClaimByType("UserID").Value;
            if (!string.IsNullOrEmpty(UserID) && UserID != "0")
            {
                teacher = UserID == "0" ? null : _teacherService.GetItemByID(UserID);
                if (teacher == null)
                {
                    return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Không có thông tin giảng viên" }
                    });
                }
            }
            if (teacher != null)
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.TeacherID == UserID));

            if (!string.IsNullOrEmpty(model.SearchText))
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Name.ToLower().Contains(model.SearchText.ToLower())));
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
                ? data
                : data.Skip((model.PageIndex - 1) * model.PageSize).Limit(model.PageSize);

            var response = new Dictionary<string, object>
            {
                { "Data", DataResponse.ToList().Select(o=> new ClassViewModel(o){
                        CourseName = _courseService.GetItemByID(o.CourseID)?.Name,
                        GradeName = _gradeService.GetItemByID(o.GradeID)?.Name,
                        SubjectName = _subjectService.GetItemByID(o.SubjectID).Name,
                        TeacherName = _teacherService.GetItemByID(o.TeacherID).FullName
                    })
                },
                { "Model", model }
            };
            return new JsonResult(response);
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetDetailsLesson(DefaultModel model, string SubjectID = "", string GradeID = "", string UserID = "")
        {
            var filter = new List<FilterDefinition<ClassEntity>>();
            TeacherEntity teacher = null;
            if (string.IsNullOrEmpty("UserID"))
                UserID = User.Claims.GetClaimByType("UserID").Value;
            if (!string.IsNullOrEmpty(UserID) && UserID != "0")
            {
                teacher = UserID == "0" ? null : _teacherService.GetItemByID(UserID);
                if (teacher == null)
                {
                    return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Không có thông tin giảng viên" }
                    });
                }
            }
            if (teacher != null)
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.TeacherID == UserID));

            if (!string.IsNullOrEmpty(model.SearchText))
            {
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Name.ToLower().Contains(model.SearchText.ToLower())));
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
                ? data
                : data.Skip((model.PageIndex - 1) * model.PageSize).Limit(model.PageSize);

            var response = new Dictionary<string, object>
            {
                { "Data", DataResponse.ToList().Select(o=> new ClassViewModel(o){
                        CourseName = _courseService.GetItemByID(o.CourseID)?.Name,
                        GradeName = _gradeService.GetItemByID(o.GradeID)?.Name,
                        SubjectName = _subjectService.GetItemByID(o.SubjectID).Name,
                        TeacherName = _teacherService.GetItemByID(o.TeacherID).FullName
                    })
                },
                { "Model", model }
            };
            return new JsonResult(response);
        }

        public JsonResult GetListMember(DefaultModel model)
        {
            if (string.IsNullOrEmpty(model.ID))
                return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Không tìm thấy thông tin lớp" }
                    });
            var currentClass = _service.GetItemByID(model.ID);
            if (currentClass == null)
                return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",model },
                        {"Msg","Không tìm thấy thông tin lớp" }
                    });
            var teacher = _teacherService.GetItemByID(currentClass.TeacherID);

            var filter = new List<FilterDefinition<StudentEntity>>();
            filter.Add(Builders<StudentEntity>.Filter.Where(o => currentClass.Students.Contains(o.StudentId)));
            var students = filter.Count > 0 ? _studentService.Collection.Find(Builders<StudentEntity>.Filter.And(filter)) : _studentService.GetAll();
            //model.TotalRecord = data.Count();
            //var DataResponse = data == null || data.Count() <= 0 || data.Count() < model.PageSize
            //    ? data
            //    : data.Skip((model.PageIndex - 1) * model.PageSize).Limit(model.PageSize);

            var response = new Dictionary<string, object>
            {
                { "Data",new Dictionary<string, object> {
                        {"Teacher",teacher },
                        {"Students",students }
                    }
                },
                { "Model", model }
            };
            return new JsonResult(response);
        }


        public JsonResult GetActiveList()
        {
            var filter = new List<FilterDefinition<ClassEntity>>();
            TeacherEntity teacher = null;
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            if (!string.IsNullOrEmpty(UserID) && UserID != "0")
            {
                teacher = UserID == "0" ? null : _teacherService.GetItemByID(UserID);
                if (teacher == null)
                {
                    return new JsonResult(new Dictionary<string, object> {
                        {"Data",null },
                        {"Error",UserID },
                        {"Msg","Không có thông tin giảng viên" }
                    });
                }
            }
            if (teacher != null)
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.TeacherID == UserID && o.EndDate < DateTime.Now));

            var data = filter.Count > 0 ? _service.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _service.GetAll();
            var DataResponse = data;
            if (data != null)
                DataResponse = data.Limit(3);

            var response = new Dictionary<string, object>
            {
                { "Data", DataResponse.ToList() }
            };
            return new JsonResult(response);
        }

        public IActionResult Detail(DefaultModel model)
        {
            if (model == null) return null;
            var currentClass = _service.GetItemByID(model.ID);
            if (currentClass == null)
                return RedirectToAction("Index");
            ViewBag.Class = currentClass;
            return View();
        }
    }
}
