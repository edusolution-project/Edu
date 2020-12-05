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

namespace BaseCustomerMVC.Controllers.Student
{
    [BaseAccess.Attribule.AccessCtrl("ClassSubject", isShow: false)]
    public class ClassSubjectController : StudentController
    {
        private readonly GradeService _gradeService;
        //private readonly AccountService _accountService;
        private readonly SubjectService _subjectService;
        private readonly TeacherService _teacherService;
        private readonly ClassService _classService;
        private readonly SkillService _skillService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly CourseService _courseService;
        //private readonly ClassProgressService _progressService;

        private readonly ChapterService _chapterService;
        //private readonly ChapterExtendService _chapterExtendService;
        private readonly LessonService _lessonService;
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly StudentService _studentService;
        //private readonly ScoreStudentService _scoreStudentService;
        private readonly LessonProgressService _lessonProgressService;
        //private readonly LearningHistoryService _learningHistoryService;

        private readonly MappingEntity<StudentEntity, ClassStudentViewModel> _mapping;
        private readonly MappingEntity<ClassEntity, ClassActiveViewModel> _activeMapping;
        private readonly MappingEntity<LessonEntity, LessonScheduleViewModel> _lessonMapping;
        //private readonly IHostingEnvironment _env;


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

        //private readonly FileProcess _fileProcess;
        //private readonly StudentHelper _studentHelper;
        //private readonly LessonHelper _lessonHelper;
        private readonly MappingEntity<LessonEntity, StudentModuleViewModel> _moduleViewMapping;
        private readonly MappingEntity<LessonEntity, StudentAssignmentViewModel> _assignmentViewMapping;
        private readonly CenterService _centerService;
        //private readonly ClassProgressService _classProgressService;
        private readonly ProgressHelper _progressHelper;
        private readonly ClassSubjectProgressService _classSubjectProgressService;


        public ClassSubjectController(
            //AccountService accountService,
            GradeService gradeservice,
            SubjectService subjectService,
            ClassSubjectService classSubjectService,
            TeacherService teacherService,
            ClassService service,
            SkillService skillService,
            CourseService courseService,
            //ClassProgressService progressService,

            ChapterService chapterService,
            //ChapterExtendService chapterExtendService,
            LessonService lessonService,
            LessonScheduleService lessonScheduleService,
            ExamService examService,
            ExamDetailService examDetailService,
            //LearningHistoryService learningHistoryService,

            //ScoreStudentService scoreStudentService,
            LessonProgressService lessonProgressService,

            StudentService studentService,
            //IHostingEnvironment evn,
            //LessonHelper lessonHelper,
            //StudentHelper studentHelper,
            CenterService centerService,
            //ClassProgressService classProgressService,
            ProgressHelper progressHelper,
            ClassSubjectProgressService classSubjectProgressService

            //FileProcess fileProcess
            )
        {
            //_accountService = accountService;
            _gradeService = gradeservice;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _courseService = courseService;
            _classService = service;
            _skillService = skillService;
            _classSubjectService = classSubjectService;
            //_progressService = progressService;

            _chapterService = chapterService;
            //_chapterExtendService = chapterExtendService;
            _lessonService = lessonService;
            _lessonScheduleService = lessonScheduleService;
            _lessonProgressService = lessonProgressService;

            _examService = examService;
            _examDetailService = examDetailService;
            //_learningHistoryService = learningHistoryService;
            //_scoreStudentService = scoreStudentService;

            _studentService = studentService;
            _mapping = new MappingEntity<StudentEntity, ClassStudentViewModel>();
            _activeMapping = new MappingEntity<ClassEntity, ClassActiveViewModel>();
            //_env = evn;
            //_fileProcess = fileProcess;

            //_studentHelper = studentHelper;
            //_lessonHelper = lessonHelper;
            _moduleViewMapping = new MappingEntity<LessonEntity, StudentModuleViewModel>();
            _assignmentViewMapping = new MappingEntity<LessonEntity, StudentAssignmentViewModel>();
            _lessonMapping = new MappingEntity<LessonEntity, LessonScheduleViewModel>();
            _centerService = centerService;
            //_classProgressService = classProgressService;
            _progressHelper = progressHelper;
            _classSubjectProgressService = classSubjectProgressService;
        }

        public JsonResult GetClassSubjects(string ClassID, string SubjectID, string GradeID, string basis)
        {
            string _studentid = User.Claims.GetClaimByType("UserID").Value;
            var student = _studentService.GetItemByID(_studentid);
            if (student == null)
            {
                return Json("Không tìm thấy học viên");
            }

            var center = _centerService.GetItemByCode(basis);
            if (center == null)
            {
                return Json("Cơ sở không tồn tại");
            }


            if (ClassID == null)
            {
                var retClass = new List<ClassEntity>();
                var retClassSbj = new List<ClassSubjectViewModel>();

                var lclass = _classService.GetItemsByIDs(student.JoinedClasses).Where(t => (t.Center == center.ID && t.EndDate >= DateTime.UtcNow) || (t.ClassMechanism == CLASS_MECHANISM.PERSONAL)).OrderBy(t => t.ClassMechanism).ThenByDescending(t => t.StartDate).AsEnumerable();

                foreach (var _class in lclass.ToList())
                {
                    var csbjs = _classSubjectService.GetByClassID(_class.ID).AsEnumerable();
                    if (!string.IsNullOrEmpty(SubjectID))
                        csbjs = csbjs.Where(t => t.SubjectID == SubjectID).AsEnumerable();
                    if (!string.IsNullOrEmpty(GradeID))
                        csbjs = csbjs.Where(t => t.GradeID == GradeID).AsEnumerable();

                    if (csbjs.Count() > 0)
                    {
                        var data = (from r in csbjs
                                    let subject = _subjectService.GetItemByID(r.SubjectID)
                                    let grade = _gradeService.GetItemByID(r.GradeID)
                                    let course = _courseService.GetItemByID(r.CourseID) ?? new CourseEntity()
                                    let skill = r.SkillID == null ? null : _skillService.GetItemByID(r.SkillID)
                                    let teacher = _teacherService.GetItemByID(r.TeacherID)
                                    select new ClassSubjectViewModel
                                    {
                                        ID = r.ID,
                                        SubjectID = r.SubjectID,
                                        SkillID = r.SkillID,
                                        SkillName = skill != null ? skill.Name : "",
                                        SkillImage = string.IsNullOrEmpty(r.Image) ? (!string.IsNullOrEmpty(course.Image) ? course.Image : (skill != null ? skill.Image : "")) : r.Image,
                                        Color = skill != null ? skill.Color : "",
                                        SubjectName = subject.Name,                                        
                                        GradeID = r.GradeID,
                                        GradeName = grade.Name,
                                        CourseID = r.CourseID,
                                        CourseName = string.IsNullOrEmpty(r.CourseName) ? course.Name : r.CourseName,
                                        TeacherID = r.TeacherID,
                                        TeacherName = teacher == null ? "" : teacher.FullName,
                                        TypeClass = r.TypeClass == null ? CLASSSUBJECT_TYPE.STANDARD : r.TypeClass,
                                        ClassName = _class.Name,
                                        ClassID = r.ClassID,
                                        StartDate = _class.StartDate,
                                        EndDate = _class.EndDate
                                    }).ToList();
                        retClassSbj.AddRange(data);
                        retClass.Add(_class);
                    }
                }
                var response = new Dictionary<string, object>
                {
                    {"Data", retClassSbj },
                    {"Classes", retClass }
                };
                return new JsonResult(response);
            }
            else
            {
                var response = new Dictionary<string, object>
                {
                    { "Data", (from r in _classSubjectService.GetByClassID(ClassID)
                              let subject = _subjectService.GetItemByID(r.SubjectID)
                              let grade = _gradeService.GetItemByID(r.GradeID)
                              let course = _courseService.GetItemByID(r.CourseID) ?? new CourseEntity()
                              let skill = r.SkillID == null? null: _skillService.GetItemByID(r.SkillID)
                              let teacher = _teacherService.GetItemByID(r.TeacherID)
                              select new ClassSubjectViewModel
                              {
                                  ID = r.ID,
                                  SubjectID = r.SubjectID,
                                  SkillID = r.SkillID,
                                  SkillName = skill != null ? skill.Name : "",
                                  SkillImage = !string.IsNullOrEmpty(course.Image) ? course.Image : (skill != null ? skill.Image : ""),
                                  Color = skill != null ? skill.Color : "",
                                  SubjectName = subject.Name,
                                  GradeID = r.GradeID,
                                  GradeName = grade.Name,
                                  CourseID = r.CourseID,
                                  CourseName = course.Name,
                                  TeacherID = r.TeacherID,
                                  TeacherName = teacher == null? "Lớp cá nhân" : teacher.FullName,
                                  TypeClass = r.TypeClass == null ? CLASSSUBJECT_TYPE.STANDARD : r.TypeClass
                              }).ToList()
                    },
                    {"ClassMechanism",_classService.GetItemByID(ClassID).ClassMechanism }
                };
                return new JsonResult(response);
            }
        }

        [HttpPost]
        public JsonResult GetContents(String ID, String Parent)
        {
            //try
            //{
            var currentClass = _classSubjectService.GetItemByID(ID);
            if (currentClass == null)
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

            var chapters = _chapterService.CreateQuery().Find(c => c.ClassSubjectID == currentClass.ID && c.ParentID == Parent).ToList();
            //var chapterExtends = _chapterExtendService.Search(currentClass.ID);

            //foreach (var chapter in chapters)
            //{
            //    var extend = chapterExtends.SingleOrDefault(t => t.ChapterID == chapter.ID);
            //    if (extend != null) chapter.Description = extend.Description;
            //}

            var lessons = (from r in _lessonService.CreateQuery().Find(o => o.ClassSubjectID == currentClass.ID && o.ChapterID == Parent).SortBy(o => o.Order).ThenBy(o => o.ID).ToList()
                           let schedule = _lessonScheduleService.CreateQuery().Find(o => o.LessonID == r.ID && o.ClassSubjectID == ID).FirstOrDefault()
                           where schedule != null
                           select _lessonMapping.AutoOrtherType(r, new LessonScheduleViewModel()
                           {
                               ScheduleID = schedule.ID,
                               StartDate = schedule.StartDate,
                               EndDate = schedule.EndDate,
                               IsActive = schedule.IsActive
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

        public JsonResult GetLearningSummary(String basis)
        {
            try
            {
                string _studentid = User.Claims.GetClaimByType("UserID").Value;
                var student = _studentService.GetItemByID(_studentid);
                if (student == null)
                {
                    return Json("Không tìm thấy học viên.");
                }

                var center = _centerService.GetItemByCode(basis);
                if (center == null)
                {
                    return Json("Cơ sở không tồn tại.");
                }

                if (student.JoinedClasses.Count == 0)
                {
                    return Json("Sinh viên chưa có trong lớp.");
                }

                var lclass = _classService.GetItemsByIDs(student.JoinedClasses).Where(t => (t.Center == center.ID && t.EndDate >= DateTime.UtcNow) || (t.ClassMechanism == CLASS_MECHANISM.PERSONAL)).OrderBy(t => t.ClassMechanism).ThenByDescending(t => t.StartDate).AsEnumerable();
                if (lclass.Count() == 0)
                {
                    return null;
                }

                var data = new List<StudentSummaryViewModel>();
                for (Int32 i = 0; i < lclass.Count(); i++)
                {
                    var @class = lclass.ElementAtOrDefault(i);
                    if (@class == null) continue;
                    var total_students = _studentService.CountByClass(@class.ID);
                    //var summary = new MappingEntity<ClassProgressEntity, StudentSummaryViewModel>()
                    //    .AutoOrtherType(_classProgressService.GetItemByClassID(@class.ID, student.ID) ?? new ClassProgressEntity
                    //    {
                    //        ClassID = @class.ID,
                    //        StudentID = student.ID,
                    //    }, new StudentSummaryViewModel()
                    //    {
                    //        ClassName = @class.Name,
                    //        Rank = -1,
                    //        TotalStudents = (int)total_students,
                    //        TotalLessons = @class.TotalLessons,
                    //        TotalExams = @class.TotalExams,
                    //        TotalPractices = @class.TotalPractices
                    //    });

                    //var results = _progressHelper.GetClassResults(@class.ID).OrderByDescending(t => t.RankPoint).ToList();

                    //summary.RankPoint = _progressHelper.CalculateRankPoint(summary.TotalPoint, summary.PracticePoint, summary.Completed);
                    //summary.AvgPoint = @class.TotalExams > 0 ? summary.TotalPoint / @class.TotalExams : 0;

                    //if (results != null && (results.FindIndex(t => t.StudentID == summary.StudentID) >= 0))
                    //    summary.Rank = results.FindIndex(t => t.RankPoint == summary.RankPoint) + 1;

                    //data.Add(summary);
                    data.AddRange(GetClassSubjectSummary(@class, student.ID, total_students));
                }
                return new JsonResult(data);
            }
            catch(Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        private List<StudentSummaryViewModel> GetClassSubjectSummary(ClassEntity @class, string StudentID, long total_students)
        {
            var data = new List<StudentSummaryViewModel>();

            var subjects = _classSubjectService.GetByClassID(@class.ID);

            foreach (var sbj in subjects)
            {
                var summary = new MappingEntity<ClassSubjectProgressEntity, StudentSummaryViewModel>()
                    .AutoOrtherType(_classSubjectProgressService.GetItemByClassSubjectID(sbj.ID, StudentID) ?? new ClassSubjectProgressEntity
                    {
                        ClassID = @class.ID,
                        StudentID = StudentID,
                        ClassSubjectID = sbj.ID
                    }, new StudentSummaryViewModel()
                    {

                        CourseName = sbj.CourseName,
                        Rank = -1,
                        TotalStudents = (int)total_students,
                        TotalLessons = sbj.TotalLessons,
                        TotalExams = sbj.TotalExams,
                        TotalPractices = sbj.TotalPractices
                    });

                if (string.IsNullOrEmpty(summary.CourseName))
                {
                    var course = _courseService.GetItemByID(sbj.CourseID);
                    if (course == null)
                    {
                        summary.SkillName = _skillService.GetItemByID(sbj.SkillID).Name;
                    }
                    else
                    {
                        summary.CourseName = course.Name;
                    }
                }

                summary.AvgPoint = summary.TotalExams > 0 ? summary.TotalPoint / summary.TotalExams : 0;
                summary.RankPoint = _progressHelper.CalculateRankPoint(summary.TotalPoint, summary.PracticePoint, summary.Completed);

                var results = _progressHelper.GetClassSubjectResults(sbj.ID).OrderByDescending(t => t.RankPoint).ToList();
                if (results != null && (results.FindIndex(t => t.StudentID == StudentID) >= 0))
                    summary.Rank = results.FindIndex(t => t.RankPoint == summary.RankPoint) + 1;

                data.Add(summary);
            }
            return data;
        }

        public JsonResult GetLearningResult(DefaultModel model,String ClassSubjectID,String basis, Boolean isPractice = false)
        {
            string _studentid = User.Claims.GetClaimByType("UserID").Value;
            var student = _studentService.GetItemByID(_studentid);
            if (student == null)
            {
                return Json("Không tìm thấy học viên");
            }

            var center = _centerService.GetItemByCode(basis);
            if (center == null)
            {
                return Json("Cơ sở không tồn tại");
            }
            var StudentID = _studentid;

            var currentCs = _classSubjectService.GetItemByID(ClassSubjectID);
            if (currentCs == null)
                return null;

            var data = new List<LessonProgressEntity>();
            var passExams = new List<LessonEntity>();
            if (!string.IsNullOrEmpty(ClassSubjectID))
            {
                passExams = (!isPractice ? _lessonService.GetClassSubjectExams(ClassSubjectID) : _lessonService.GetClassSubjectPractices(ClassSubjectID)).ToList();
                data = _lessonProgressService.GetByClassSubjectID_StudentID(ClassSubjectID, StudentID);
            }

            var lessons = (
                            from lesson in passExams
                            let progress = data.FirstOrDefault(t => t.StudentID == student.ID && t.LessonID == lesson.ID) ?? new LessonProgressEntity()
                            let schedule = _lessonScheduleService.GetItemByLessonID(lesson.ID)
                            select _assignmentViewMapping.AutoOrtherType(lesson, new StudentAssignmentViewModel()
                            {
                                ScheduleID = schedule.ID,
                                ScheduleStart = schedule.StartDate,
                                ScheduleEnd = schedule.EndDate,
                                IsActive = schedule.IsActive,
                                LearnCount = progress.Tried,
                                LearnLast = progress.LastTry,
                                Result = progress.LastPoint,
                                LessonId = lesson.ID,
                            })).OrderByDescending(r => r.ScheduleStart).ThenBy(r => r.ChapterID).ThenBy(r => r.LessonId).ToList();

            var response = new Dictionary<string, object>
            {
                { "Data", lessons},
                { "Model", model }
            };
            return new JsonResult(response);
        }

        public JsonResult GetLearningProgress(String ClassSubjectID,String basis)
        {
            string _studentid = User.Claims.GetClaimByType("UserID").Value;
            var student = _studentService.GetItemByID(_studentid);
            if (student == null)
            {
                return Json("Không tìm thấy học viên");
            }

            var center = _centerService.GetItemByCode(basis);
            if (center == null)
            {
                return Json("Cơ sở không tồn tại");
            }
            var StudentID = _studentid;

            var currentCs = _classSubjectService.GetItemByID(ClassSubjectID);
            if (currentCs == null)
                return null;

            List<LessonProgressEntity> data;
            data = _lessonProgressService.GetByClassSubjectID_StudentID(ClassSubjectID, StudentID);

            var subjects = _classSubjectService.GetByCourseID(currentCs.CourseID);

            var lessons = (from progress in data
                           let schedule = _lessonScheduleService.CreateQuery().Find(o => o.LessonID == progress.LessonID && o.ClassSubjectID ==ClassSubjectID).FirstOrDefault()
                           where schedule != null
                           let classsubject = subjects.Single(t => t.ID == schedule.ClassSubjectID)
                           where classsubject != null
                           let lesson = _lessonService.GetItemByID(progress.LessonID)
                           select _moduleViewMapping.AutoOrtherType(lesson, new StudentModuleViewModel()
                           {
                               ScheduleID = schedule.ID,
                               ScheduleStart = schedule.StartDate,
                               Skill = _skillService.GetItemByID(classsubject.SkillID).Name,
                               ScheduleEnd = schedule.EndDate,
                               IsActive = schedule.IsActive,
                               LearnCount = progress.TotalLearnt,
                               TemplateType = lesson.TemplateType,
                               LearnStart = progress.FirstDate,
                               LearnLast = progress.LastDate
                           })).OrderBy(r => r.LearnStart).ThenBy(r => r.ChapterID).ThenBy(r => r.ID).ToList();

            var response = new Dictionary<string, object>
            {
                { "Data", lessons}
            };
            return new JsonResult(response);
        }
    }
}
