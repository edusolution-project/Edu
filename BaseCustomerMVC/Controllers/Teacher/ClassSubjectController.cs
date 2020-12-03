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
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace BaseCustomerMVC.Controllers.Teacher
{
    [BaseAccess.Attribule.AccessCtrl("ClassSubject", isShow: false)]
    public class ClassSubjectController : TeacherController
    {
        private readonly GradeService _gradeService;
        private readonly SubjectService _subjectService;
        private readonly TeacherService _teacherService;
        private readonly SkillService _skillService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly CourseService _courseService;

        private readonly ChapterService _chapterService;

        private readonly LessonService _lessonService;
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly LessonProgressService _lessonProgressService;

        private readonly MappingEntity<StudentEntity, ClassStudentViewModel> _mapping;
        private readonly MappingEntity<ClassEntity, ClassActiveViewModel> _activeMapping;
        private readonly MappingEntity<LessonEntity, LessonScheduleViewModel> _lessonMapping;
        private readonly MappingEntity<LessonEntity, LessonResultViewModel> _resultMapping;

        private readonly ExamService _examService;

        public ClassSubjectController(

            GradeService gradeservice,
            SubjectService subjectService,
            ClassSubjectService classSubjectService,
            TeacherService teacherService,

            SkillService skillService,
            CourseService courseService,
            LessonService lessonService,
            LessonScheduleService lessonScheduleService,
            ExamService examService,
            ChapterService chapterService,
            LessonProgressService lessonProgressService
            )
        {
            _gradeService = gradeservice;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _courseService = courseService;
            _skillService = skillService;
            _classSubjectService = classSubjectService;
            _chapterService = chapterService;
            _lessonService = lessonService;
            _lessonScheduleService = lessonScheduleService;
            _lessonProgressService = lessonProgressService;
            _examService = examService;

            _mapping = new MappingEntity<StudentEntity, ClassStudentViewModel>();
            _activeMapping = new MappingEntity<ClassEntity, ClassActiveViewModel>();
            _lessonMapping = new MappingEntity<LessonEntity, LessonScheduleViewModel>();
            _resultMapping = new MappingEntity<LessonEntity, LessonResultViewModel>();
        }

        public JsonResult GetClassSubjects(string ClassID)
        {
            //if (string.IsNullOrEmpty(ClassID))
            //    return null;
            var teacherID = "";
            if (User.IsInRole("teacher"))
                teacherID = User.Claims.GetClaimByType("UserID").Value;
            var response = new Dictionary<string, object>
            {
                { "Data", (from r in _classSubjectService.GetByClassID(ClassID)
                          where string.IsNullOrEmpty(teacherID) || r.TeacherID == teacherID
                          let subject = r.SubjectID != null?  _subjectService.GetItemByID(r.SubjectID): new SubjectEntity()
                          let grade = r.GradeID != null? _gradeService.GetItemByID(r.GradeID): new GradeEntity()
                          let course = _courseService.GetItemByID(r.CourseID) ?? new CourseEntity{ID = r.CourseID}
                          let teacher = _teacherService.GetItemByID(r.TeacherID)
                          let skill = r.SkillID == null? null: _skillService.GetItemByID(r.SkillID)
                          select new ClassSubjectViewModel
                          {
                              ID = r.ID,
                              SubjectID = r.SubjectID,
                              SkillID = r.SkillID,
                              SkillName = skill != null ? skill.Name : "",
                              SkillImage = !string.IsNullOrEmpty(course.Image) ? course.Image : (skill != null ? skill.Image : ""),
                              Color = skill != null? skill.Color : "",
                              SubjectName = subject.Name,
                              GradeID = r.GradeID,
                              GradeName = grade.Name,
                              CourseID = r.CourseID,
                              CourseName = string.IsNullOrEmpty(course.Name) ? skill?.Name : course.Name,
                              TeacherID = r.TeacherID,
                              TeacherName = teacher.FullName,
                              TypeClass = r.TypeClass
                          }).ToList()
                },
            };
            return new JsonResult(response);
        }

        [HttpPost]
        public JsonResult GetContents(string ID, string Parent)
        {
            //try
            //{
            var currentCs = _classSubjectService.GetItemByID(ID);
            if (currentCs == null)
                return new JsonResult(new Dictionary<string, object>
                    {
                        {"Error", "Không tìm thấy lớp học" }
                    });

            if (string.IsNullOrEmpty(Parent))
                Parent = "0";

            var TopID = "";
            if (Parent != "0")
            {
                var top = _chapterService.GetItemByID(Parent);
                if (top == null)
                    return new JsonResult(new Dictionary<string, object>
                    {
                        {"Error", "Không tìm thấy chương" }
                    });
                TopID = top.ParentID;
            }

            var chapters = _chapterService.GetSubChapters(currentCs.ID, Parent);

            var lessons = (from r in _lessonService.CreateQuery().Find(o => o.ClassSubjectID == currentCs.ID && o.ChapterID == Parent).SortBy(o => o.Order).ThenBy(o => o.ID).ToList()
                           let schedule = _lessonScheduleService.CreateQuery().Find(o => o.LessonID == r.ID && o.ClassSubjectID == ID).FirstOrDefault()
                           where schedule != null
                           select _lessonMapping.AutoOrtherType(r, new LessonScheduleViewModel()
                           {
                               ScheduleID = schedule.ID,
                               StartDate = schedule.StartDate,
                               EndDate = schedule.EndDate,
                               IsHideAnswer = schedule.IsHideAnswer,
                               IsActive = schedule.IsActive,
                               IsOnline = schedule.IsOnline
                           })).ToList();

            var response = new Dictionary<string, object>
                {
                    { "RootID", TopID },
                    { "Data", chapters },
                    { "Lesson", lessons }
                };

            return new JsonResult(response);
            //}
            //catch (Exception ex)
            //{
            //    return new JsonResult(new Dictionary<string, object>
            //    {
            //        { "Data", null },
            //        {"Error", ex.Message }
            //    });
            //}
        }


        [HttpPost]
        public JsonResult GetFullStructure(string ID)
        {
            var currentCs = _classSubjectService.GetItemByID(ID);
            if (currentCs == null)
                return new JsonResult(new Dictionary<string, object>
                    {
                        {"Error", "Không tìm thấy học liệu" }
                    });

            var courseDetail = new Dictionary<string, object>
            {
                { "Chapters", _chapterService.CreateQuery().Find(o => o.ClassSubjectID == ID).SortBy(o => o.ParentID).ThenBy(o => o.Order).ThenBy(o => o.ID).ToList() } ,
                { "Lessons", _lessonService.CreateQuery().Find(o => o.ClassSubjectID == ID).SortBy(o => o.ChapterID).ThenBy(o => o.Order).ThenBy(o => o.ID).ToList() }
            };

            var response = new Dictionary<string, object>
                {
                    { "Data", courseDetail }
                };

            return new JsonResult(response);
        }

        [HttpPost]
        public JsonResult GetResults(string ID, string Parent)
        {
            //try
            //{
            var currentCs = _classSubjectService.GetItemByID(ID);
            if (currentCs == null)
                return new JsonResult(new Dictionary<string, object>
                    {
                        {"Error", "Không tìm thấy lớp học" }
                    });

            if (string.IsNullOrEmpty(Parent))
                Parent = "0";

            var TopID = "";
            if (Parent != "0")
            {
                var top = _chapterService.GetItemByID(Parent);
                if (top == null)
                    return new JsonResult(new Dictionary<string, object>
                    {
                        {"Error", "Không tìm thấy chương" }
                    });
                TopID = top.ParentID;
            }

            var chapters = _chapterService.GetSubChapters(currentCs.ID, Parent);

            var lessons = (from r in _lessonService.CreateQuery().Find(o => o.ClassSubjectID == currentCs.ID && o.ChapterID == Parent).SortBy(o => o.Order).ThenBy(o => o.ID).ToList()
                           let schedule = _lessonScheduleService.CreateQuery().Find(o => o.LessonID == r.ID && o.ClassSubjectID == ID).FirstOrDefault()
                           where schedule != null
                           let progress = _lessonProgressService.CreateQuery().Find(o => o.LessonID == r.ID && o.ClassSubjectID == ID)
                           let progressCount = progress.CountDocuments()
                           let examCount = _examService.CreateQuery().Find(o => o.LessonID == r.ID && o.ClassSubjectID == ID).Project(t => t.StudentID).ToList().Distinct().Count()
                           select _resultMapping.AutoOrtherType(r, new LessonResultViewModel()
                           {
                               ScheduleID = schedule.ID,
                               StartDate = schedule.StartDate,
                               EndDate = schedule.EndDate,
                               LearntCount = progressCount,
                               ExamCount = examCount,
                               AvgPoint = progressCount > 0 ? (r.TemplateType == LESSON_TEMPLATE.EXAM ? progress.ToList().Average(t => t.AvgPoint) : 0) : 0,
                               AvgPracticePoint = progressCount > 0 ? (r.TemplateType == LESSON_TEMPLATE.LECTURE ? progress.ToList().Average(t => t.AvgPoint) : 0) : 0
                           })).ToList();

            var response = new Dictionary<string, object>
                {
                    { "RootID", TopID },
                    { "Data", chapters },
                    { "Lesson", lessons }
                };

            return new JsonResult(response);
            //}
            //catch (Exception ex)
            //{
            //    return new JsonResult(new Dictionary<string, object>
            //    {
            //        { "Data", null },
            //        {"Error", ex.Message }
            //    });
            //}
        }
    }
}
