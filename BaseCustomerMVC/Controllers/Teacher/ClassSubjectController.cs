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
        private readonly AccountService _accountService;
        private readonly SubjectService _subjectService;
        private readonly TeacherService _teacherService;
        private readonly ClassService _classService;
        private readonly SkillService _skillService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly CourseService _courseService;
        private readonly ClassProgressService _progressService;

        private readonly ChapterService _chapterService;
        private readonly ChapterExtendService _chapterExtendService;
        private readonly LessonService _lessonService;
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly StudentService _studentService;
        private readonly ScoreStudentService _scoreStudentService;
        private readonly LessonProgressService _lessonProgressService;
        private readonly LearningHistoryService _learningHistoryService;

        private readonly MappingEntity<StudentEntity, ClassStudentViewModel> _mapping;
        private readonly MappingEntity<ClassEntity, ClassActiveViewModel> _activeMapping;
        private readonly MappingEntity<LessonEntity, LessonScheduleViewModel> _lessonMapping;
        private readonly MappingEntity<LessonEntity, LessonResultViewModel> _resultMapping;
        private readonly IHostingEnvironment _env;


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
        private readonly StudentHelper _studentHelper;
        private readonly LessonHelper _lessonHelper;
        private readonly MappingEntity<LessonEntity, StudentModuleViewModel> _moduleViewMapping;
        private readonly MappingEntity<LessonEntity, StudentAssignmentViewModel> _assignmentViewMapping;


        public ClassSubjectController(
            AccountService accountService,
            GradeService gradeservice,
            SubjectService subjectService,
            ClassSubjectService classSubjectService,
            TeacherService teacherService,
            ClassService service,
            SkillService skillService,
            CourseService courseService,
            ClassProgressService progressService,

            ChapterService chapterService,
            ChapterExtendService chapterExtendService,
            LessonService lessonService,
            LessonScheduleService lessonScheduleService,
            ExamService examService,
            ExamDetailService examDetailService,
            LearningHistoryService learningHistoryService,

            ScoreStudentService scoreStudentService,
            LessonPartService lessonPartService,
            LessonPartQuestionService lessonPartQuestionService,
            LessonPartAnswerService lessonPartAnswerService,
            LessonProgressService lessonProgressService,

            CloneLessonPartService cloneLessonPartService,
            CloneLessonPartAnswerService cloneLessonPartAnswerService,
            CloneLessonPartQuestionService cloneLessonPartQuestionService,

            StudentService studentService, IHostingEnvironment evn,

            FileProcess fileProcess)
        {
            _accountService = accountService;
            _gradeService = gradeservice;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _courseService = courseService;
            _classService = service;
            _skillService = skillService;
            _classSubjectService = classSubjectService;
            _progressService = progressService;

            _chapterService = chapterService;
            _chapterExtendService = chapterExtendService;
            _lessonService = lessonService;
            _lessonScheduleService = lessonScheduleService;
            _lessonProgressService = lessonProgressService;

            _examService = examService;
            _examDetailService = examDetailService;
            _learningHistoryService = learningHistoryService;
            _scoreStudentService = scoreStudentService;

            _studentService = studentService;
            _mapping = new MappingEntity<StudentEntity, ClassStudentViewModel>();
            _activeMapping = new MappingEntity<ClassEntity, ClassActiveViewModel>();
            _env = evn;
            _fileProcess = fileProcess;

            _studentHelper = new StudentHelper(studentService, accountService);
            _lessonHelper = new LessonHelper(
                lessonService,
                lessonPartService,
                lessonPartQuestionService,
                lessonPartAnswerService,
                cloneLessonPartService,
                cloneLessonPartAnswerService,
                cloneLessonPartQuestionService
                );
            _moduleViewMapping = new MappingEntity<LessonEntity, StudentModuleViewModel>();
            _assignmentViewMapping = new MappingEntity<LessonEntity, StudentAssignmentViewModel>();
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
                          let subject = _subjectService.GetItemByID(r.SubjectID)
                          let grade = _gradeService.GetItemByID(r.GradeID)
                          let course = _courseService.GetItemByID(r.CourseID)
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
                              CourseName = course.Name,
                              TeacherID = r.TeacherID,
                              TeacherName = teacher.FullName
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
                           let examCount = _examService.CreateQuery().Find(o => o.LessonID == r.ID && o.ClassSubjectID == ID).Project(t=> t.StudentID).ToList().Distinct().Count()
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
