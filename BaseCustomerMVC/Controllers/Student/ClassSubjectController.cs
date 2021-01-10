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

                var lclass = _classService.GetItemsByIDs(student.JoinedClasses).Where(t => (t.Center == center.ID && t.EndDate.AddDays(1) >= DateTime.UtcNow) || (t.ClassMechanism == CLASS_MECHANISM.PERSONAL)).OrderBy(t => t.ClassMechanism).ThenByDescending(t => t.StartDate).AsEnumerable();
                var lclasses = lclass.Where(x => x.IsActive && x.EndDate >= DateTime.UtcNow).ToList();
                foreach (var _class in lclasses.ToList())
                //foreach (var _class in lclass.ToList())
                {
                    var csbjs = _classSubjectService.GetByClassID(_class.ID).AsEnumerable();
                    if (!string.IsNullOrEmpty(SubjectID))
                        csbjs = csbjs.Where(t => t.SubjectID == SubjectID).AsEnumerable();
                    if (!string.IsNullOrEmpty(GradeID))
                        csbjs = csbjs.Where(t => t.GradeID == GradeID).AsEnumerable();

                    if (csbjs.Count() > 0)
                    {
                        var data = (from r in csbjs
                                    let subject = r.SubjectID != null ? _subjectService.GetItemByID(r.SubjectID) : new SubjectEntity()
                                    let grade = r.GradeID != null ? _gradeService.GetItemByID(r.GradeID) : new GradeEntity()
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
                                        SubjectName = subject == null && r.TypeClass == CLASSSUBJECT_TYPE.EXAM ? "Kiểm tra & đánh giá" : subject?.Name,
                                        GradeID = r.GradeID,
                                        GradeName = grade?.Name,
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

        public JsonResult GetLearningSummary(String basis, Boolean isPractice, Int32 TypeFilter)
        {
            //try
            //{
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
                return Json("lclass = 0");
            }

            lclass = lclass.ToList().Where(x => x.ClassMechanism != CLASS_MECHANISM.PERSONAL);
            var listClassIDs = lclass.Select(x => x.ID).ToList();
            var listClassSbj = _classSubjectService.CreateQuery().Find(x => listClassIDs.Contains(x.ClassID));
            var listClassSbjIds = listClassSbj.ToList().Select(x => x.ID);
            var classSbjExam = listClassSbj.ToList().Where(x => x.TypeClass == CLASSSUBJECT_TYPE.EXAM);
            var classSbjPractices = listClassSbj.ToList().Where(x => x.TypeClass != CLASSSUBJECT_TYPE.EXAM);
            var activelessonTemp = _lessonScheduleService.CreateQuery().Find(x => listClassSbjIds.Contains(x.ClassSubjectID) && x.StartDate <= DateTime.Now).ToList();
            var progess = _lessonProgressService.CreateQuery().Find(x => listClassSbjIds.Contains(x.ClassSubjectID) && x.StudentID == student.ID).ToList();
            List<LessonScheduleEntity> activelesson = new List<LessonScheduleEntity>();
            foreach (var item in lclass)
            {
                var _activelesson = activelessonTemp.Where(x => x.EndDate >= item.StartDate && x.ClassID == item.ID).ToList();
                activelesson.AddRange(_activelesson);
            }
            //data Practice
            List<StudentSummaryViewModel> practiceResult = new List<StudentSummaryViewModel>();
            {
                var classSbjPracticeIDs = classSbjPractices.Select(x => x.ID).ToList();
                var activelessonIDs = activelesson.Select(x => x.LessonID).ToList();
                var lessonPractice = _lessonService.CreateQuery().Find(x => activelessonIDs.Contains(x.ID) && x.IsPractice).ToList();
                var lessonIDsPractice = lessonPractice.Select(x => x.ID);
                //var a = progess.Where(x => x.StudentID == student.ID && lessonIDsPractice.Contains(x.LessonID)).GroupBy(x => x.ClassID).ToList();
                var _practiceResult = progess.Where(x => x.StudentID == student.ID && lessonIDsPractice.Contains(x.LessonID)).Select(x =>
                      new StudentSummaryViewModel
                      {
                          StudentID = student.ID,
                          ClassSubjectID = x.ClassSubjectID,
                          CourseName = listClassSbj.ToList().Where(y => y.ID == x.ClassSubjectID).FirstOrDefault().CourseName,
                          ClassID = x.ClassID,
                          TypeClassSbj = listClassSbj.ToList().Where(y => y.ID == x.ClassSubjectID).FirstOrDefault().TypeClass,
                          TotalLessons = lessonPractice.Where(y => y.ClassSubjectID == x.ClassSubjectID).Count(),
                          PracticePoint = x.LastPoint
                          //PracticeAvgPoint = lessonPractice.Where(y => y.ClassSubjectID == x.ClassSubjectID).Count() > 0 ? x.ToList().Select(y => y.LastPoint).Sum() / lessonPractice.Where(y => y.ClassSubjectID == x.Key.ClassSubjectID).Count() : 0,
                      }).GroupBy(x => new { x.ClassID, x.ClassSubjectID }).Select(x =>
                           new StudentSummaryViewModel
                           {
                               ClassID = x.Key.ClassID,
                               StudentID = x.ToList().FirstOrDefault().StudentID,
                               ClassSubjectID = x.Key.ClassSubjectID,
                               CourseName = x.ToList().FirstOrDefault().CourseName,
                               TypeClassSbj = x.ToList().FirstOrDefault().TypeClassSbj,
                               TotalLessons = x.ToList().FirstOrDefault().TotalLessons,
                               PracticeAvgPoint = x.ToList().FirstOrDefault().TotalLessons > 0 ? x.ToList().Where(y => y.ClassSubjectID == x.Key.ClassSubjectID).Sum(y => y.PracticePoint) / x.ToList().FirstOrDefault().TotalLessons : 0,
                           }
                      );
                //var test = practiceResult.GroupBy(x => x.ClassID).ToList();
                practiceResult.AddRange(_practiceResult);
            }

            //data Exam
            var dataExam = new List<StudentExamVM>();
            {
                var classSbjExamIDs = classSbjExam.Select(x => x.ID).ToList();
                var lessonIDsExam = activelesson.Where(x => classSbjExamIDs.Contains(x.ClassSubjectID)).Select(x => x.LessonID).ToList();
                var examResult = progess.Where(x => x.StudentID == student.ID && lessonIDsExam.Contains(x.LessonID) && x.Tried > 0);
                foreach (var item in examResult)
                {
                    var _item = new StudentExamVM()
                    {
                        StudentID = student.ID,
                        ClassSubjectID = item.ClassSubjectID,
                        ClassID = item.ClassID,
                        TypeClassSbj = classSbjExam.Where(y => y.ID == item.ClassSubjectID).FirstOrDefault().TypeClass,
                        LessonID = item.LessonID,
                        Point = item.LastPoint
                    };
                    dataExam.Add(_item);
                }
            }

            var data = new List<StudentSummaryViewModel>();
            var a = practiceResult.GroupBy(x => x.ClassID);
            foreach(var item in a)
            {
                for (int i = 0; i < item.Count(); i++)
                {
                    var _item = item.ElementAtOrDefault(i);
                    _item.Order = i + 1;
                    data.Add(_item);
                }
            }
            Dictionary<String, Object> data_response = new Dictionary<String, Object>();
            data_response.Add("Practice", data);
            data_response.Add("Exam", dataExam);
            return Json(data_response);

            //var data = new Dictionary<String, Object>();
            //for (Int32 i = 0; i < lclass.Count(); i++)
            //{
            //    var @class = lclass.ElementAtOrDefault(i);
            //    if (@class == null) continue;
            //    var total_students = _studentService.CountByClass(@class.ID);
            //    var a = GetClassSubjectSummary(@class, student, total_students);
            //    data.Add(@class.ID, a);
            //}
            //return new JsonResult(data);
            //}
            //catch (Exception ex)
            //{
            //    return new JsonResult(ex.Message);
            //}
        }

        private Dictionary<String, Object> GetClassSubjectSummary(ClassEntity @class, StudentEntity Student, long total_students)
        {
            Dictionary<String, Object> data_response = new Dictionary<String, Object>();
            var data = new List<StudentSummaryViewModel>();
            var dataExam = new List<StudentSummaryExamViewModel>();

            var subjects = _classSubjectService.GetByClassID(@class.ID);

            var index = 0;
            var result = new List<StudentLessonResultViewModel>();
            foreach (var sbj in subjects.OrderBy(x => x.TypeClass))
            {
                //var activeLessons = _lessonScheduleService.CreateQuery().Find(o => o.ClassID == @class.ID && o.ClassSubjectID == sbj.ID && (o.StartDate != new DateTime() || o.EndDate != new DateTime()) && o.StartDate <= DateTime.Now).ToList();
                //var activeLessonIds = activeLessons.Select(t => t.LessonID).ToList();
                //var activeProgress = _lessonProgressService.CreateQuery().Find(x => x.StudentID == Student.ID && activeLessonIds.Contains(x.LessonID) && x.LastDate <= DateTime.Now).ToList();
                index = index + 1;
                if (sbj.TypeClass != CLASSSUBJECT_TYPE.EXAM)
                {
                    result = _progressHelper.GetLessonProgressList(sbj.StartDate, DateTime.Now, Student, sbj).Result;

                    //if (activeLessonIds.Count == 0) continue;
                    //var practiceIds = _lessonService.CreateQuery().Find(x => x.IsPractice == true && activeLessonIds.Contains(x.ID)).Project(x => x.ID).ToList();
                    if (result.Count == 0)
                    {
                        index = index - 1;
                        continue;
                    }
                    //var studentResult = activeProgress.Where(x => practiceIds.Contains(x.LessonID) && x.Tried > 0).ToList();

                    var summary = new MappingEntity<ClassSubjectProgressEntity, StudentSummaryViewModel>()
                        .AutoOrtherType(_classSubjectProgressService.GetItemByClassSubjectID(sbj.ID, Student.ID) ?? new ClassSubjectProgressEntity
                        {
                            ClassID = @class.ID,
                            StudentID = Student.ID,
                            ClassSubjectID = sbj.ID
                        }, new StudentSummaryViewModel()
                        {
                            CourseName = sbj.CourseName,
                            TotalPractices = sbj.TotalPractices,
                            TypeClassSbj = sbj.TypeClass,
                            Order = index
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
                    //if (TypeFilter.Equals(Type_Filter.LESSONSCHEDULE))
                    //{
                    summary.PracticeAvgPoint = result.Count() > 0 ? result.Average(x => x.LastPoint) : 0;
                    //}
                    data.Add(summary);
                }
                else
                {
                    var activeLessons = _lessonScheduleService.CreateQuery().Find(o => o.ClassID == @class.ID && o.ClassSubjectID == sbj.ID && (o.StartDate != new DateTime() || o.EndDate != new DateTime()) && o.StartDate <= DateTime.Now).ToList();
                    var activeLessonIds = activeLessons.Select(t => t.LessonID).ToList();
                    //if (activeLessonIds.Count > 0)
                    //{
                    //    var a = _lessonProgressService.CreateQuery().Find(x => activeLessonIds.Contains(x.LessonID)).ToList();
                    //}
                    var exam = _lessonProgressService.CreateQuery().Find(x => activeLessonIds.Contains(x.LessonID)).ToList().GroupBy(x => x.Multiple).Select(x => new { Multiple = x.Key, ListPoint = x.ToList().Where(y => y.Tried > 0).Select(y => y.LastPoint).ToList() });
                    var summary = new MappingEntity<ClassSubjectProgressEntity, StudentSummaryExamViewModel>()
                        .AutoOrtherType(_classSubjectProgressService.GetItemByClassSubjectID(sbj.ID, Student.ID) ?? new ClassSubjectProgressEntity
                        {
                            ClassID = @class.ID,
                            StudentID = Student.ID,
                            ClassSubjectID = sbj.ID
                        }, new StudentSummaryExamViewModel()
                        {
                            CourseName = sbj.CourseName,
                            TotalExams = sbj.TotalExams,
                            TypeClassSbj = sbj.TypeClass,
                            Order = index
                        });

                    var Multiple1 = exam.Where(x => x.Multiple == 1).FirstOrDefault();
                    var Multiple2 = exam.Where(x => x.Multiple == 2).FirstOrDefault();
                    var Multiple3 = exam.Where(x => x.Multiple == 3).FirstOrDefault();
                    summary.Multiple1 = Multiple1 == null || Multiple1.ListPoint != null ? 0 : Multiple1.ListPoint.Average();
                    summary.Multiple2 = Multiple2 == null || Multiple2.ListPoint != null ? 0 : Multiple2.ListPoint.Average();
                    summary.Multiple3 = Multiple3 == null || Multiple3.ListPoint != null ? 0 : Multiple3.ListPoint.Average();

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
                    dataExam.Add(summary);
                }
            }

            data_response.Add("Practice", data);
            data_response.Add("Exam", dataExam);
            return data_response;
        }

        public JsonResult GetLearningResult(DefaultModel model, string ClassSubjectID, String basis, bool isPractice = false)
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
                return Json("Không tìm thấy môn học");

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

            var a = CLASSSUBJECT_TYPE.EXAM;

            var response = new Dictionary<string, object>
            {
                { "Data", lessons},
                { "Model", model }
            };
            return new JsonResult(response);
        }

        public JsonResult GetLearningProgress(String ClassSubjectID, String basis)
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
                return Json("Không tìm thấy môn học");

            List<LessonProgressEntity> data;
            data = _lessonProgressService.GetByClassSubjectID_StudentID(ClassSubjectID, StudentID);

            var subjects = _classSubjectService.GetByCourseID(currentCs.CourseID);

            var lessons = (from progress in data
                           let schedule = _lessonScheduleService.CreateQuery().Find(o => o.LessonID == progress.LessonID && o.ClassSubjectID == ClassSubjectID).FirstOrDefault()
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

        public JsonResult GetDetailCourse(String basis, String ClassSubjectID, Boolean HasExam = false)
        {
            try
            {
                string _studentid = User.Claims.GetClaimByType("UserID").Value;
                var student = _studentService.GetItemByID(_studentid);
                var lesson = _lessonService.CreateQuery().Find(x => x.ClassSubjectID == ClassSubjectID).FirstOrDefault();
                var @class = _classService.GetItemByID(lesson.ClassID);
                var sbj = _classSubjectService.GetItemByID(lesson.ClassSubjectID);
                var startDate = sbj.StartDate;
                List<LessonScheduleEntity> activeLessons = new List<LessonScheduleEntity>();
                //if(TypeFilter == Type_Filter.LESSONSCHEDULE)
                //{
                //    activeLessons = _lessonScheduleService.CreateQuery().Find(o => o.ClassID == @class.ID && o.ClassSubjectID == sbj.ID && (o.StartDate != new DateTime() || o.EndDate != new DateTime()) && o.StartDate <= DateTime.Now).ToList();
                //}
                //if(TypeFilter == Type_Filter.SHOWALL)
                //{
                //    activeLessons = _lessonScheduleService.CreateQuery().Find(o => o.ClassID == @class.ID && o.ClassSubjectID == sbj.ID ).ToList();
                //}
                var listTime = GetListWeek(startDate);
                List<StudentDetailVM> dataresponse = new List<StudentDetailVM>();
                List<StudentLessonResultViewModel> result = new List<StudentLessonResultViewModel>();
                foreach (var item in listTime)
                {

                    result = _progressHelper.GetLessonProgressList(item.Value.StartTime, item.Value.EndTime, student, sbj).Result;
                    if (result.Count == 0)
                    {
                        continue;
                    }
                    var data = new StudentDetailVM();
                    var index = item.Key;
                    var startTime = item.Value.StartTime;
                    var endTime = item.Value.EndTime;
                    //activeLessons = _lessonScheduleService.CreateQuery().Find(o => o.ClassID == @class.ID && o.StartDate <= endTime && o.EndDate >= startTime && o.ClassSubjectID == ClassSubjectID).ToList();

                    //var activeLessonIds = activeLessons.Select(t => t.LessonID).ToList();

                    //var activeProgress = _lessonProgressService.CreateQuery().Find(x => x.StudentID == student.ID && activeLessonIds.Contains(x.LessonID) && x.LastDate <= endTime).ToList();
                    //// danh sach bai
                    //if (activeProgress.Count == 0)
                    //{
                    //    continue;
                    //}
                    //else
                    //{
                    //    var practiceIds = _lessonService.CreateQuery().Find(x => x.IsPractice == true && activeLessonIds.Contains(x.ID)).Project(x => x.ID).ToList();
                    data.Week = index;
                    //var exam = _examService.CreateQuery().Find(x => x.StudentID == student.ID && practiceIds.Contains(x.LessonID)).ToList().GroupBy(x=>x.LessonID);
                    //var point = exam.Count == 0 ? 0 : exam.Average(x => x.LastPoint);
                    //var _data = activeProgress.Where(x => x.Tried > 0 && practiceIds.Contains(x.LessonID)).ToList();
                    //var lessonResult = activeProgress.Where(x => practiceIds.Contains(x.LessonID) && x.Tried > 0).ToList();
                    var point = result.Count() > 0 ? result.Average(x => x.LastPoint).ToString() : "";
                    data.Point = point.ToString();

                    data.StartTime = startTime;
                    data.EndTime = endTime;
                    //data.TotalPractice = activeLessonIds.Count();
                    //data.PracticeDone = (Int32)_examService.CreateQuery().Find(x => x.StudentID == student.ID && practiceIds.Contains(x.LessonID)).ToList().Select(x => x.LessonID).Distinct().Count();
                    //}
                    dataresponse.Add(data);
                }

                if (HasExam)
                {
                    var sbjExam = _classSubjectService.GetClassSubjectExamByClassID(@class.ID);
                    activeLessons = _lessonScheduleService.CreateQuery().Find(o => o.ClassSubjectID == sbjExam.ID).ToList();
                    var activeLessonIds = activeLessons.Select(t => t.LessonID).ToList();
                    var exams = _lessonService.CreateQuery().Find(x => x.TemplateType == 2 && activeLessonIds.Contains(x.ID)).ToList();

                    foreach (var exam in exams)
                    {
                        var examresult = _examService.CreateQuery().Find(t => t.StudentID == student.ID && t.LessonID == exam.ID).SortByDescending(t => t.ID).ToList();
                        var progress = _lessonProgressService.GetByStudentID_LessonID(student.ID, exam.ID);
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
                            ListExam = examresult.Select(t => new ExamDetailCompactView(t)).ToList(),
                            LessonName = exam.Title,
                            LessonID = exam.ID,
                            ClassSubjectID = exam.ClassSubjectID
                        });
                    }
                }
                return Json(new Dictionary<String, Object>
                {
                    {"Practice",dataresponse },
                    {"Exam",result }
                });
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        private Dictionary<Int32, DateTimeVM> GetListWeek(DateTime StartDate)
        {
            var currentTime = DateTime.Now;
            Dictionary<Int32, DateTimeVM> listDateTime = new Dictionary<int, DateTimeVM>();

            var dt = StartDate;
            var day = dt.Day;
            var month = dt.Month;
            var year = dt.Year;
            var today = new DateTime(year, month, day, 0, 0, 0);
            var startWeek = today.AddDays(DayOfWeek.Sunday - today.DayOfWeek + 1);
            var endWeek = startWeek.AddDays(7).AddMinutes(-1);
            var Week1 = new DateTimeVM { StartTime = startWeek, EndTime = endWeek };
            listDateTime.Add(1, Week1);

            var index = 2;
            while (endWeek < currentTime)
            {
                startWeek = endWeek.AddMinutes(1);
                endWeek = startWeek.AddDays(6).AddHours(23).AddMinutes(59);
                var weekn = new DateTimeVM { StartTime = startWeek, EndTime = endWeek };
                listDateTime.Add(index, weekn);
                index++;
            }
            listDateTime[index - 1].EndTime = DateTime.Now;
            return listDateTime;
        }

        public class StudentSummaryExamViewModel : StudentSummaryViewModel
        {
            public Double Multiple1 { get; set; }
            public Double Multiple2 { get; set; }
            public Double Multiple3 { get; set; }
        }

        public class DateTimeVM
        {
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
        }

        protected class StudentDetailVM
        {
            public int Week { get; set; }
            public String Point { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public Int32 PracticeDone { get; set; }
            public Int32 TotalPractice { get; set; }
        }

        public class StudentExamVM
        {
            public Double Point { get; set; }
            public String LessonID { get; set; }
            public String ClassSubjectID { get; set; }
            public String StudentID { get; set; }
            public String ClassID { get; set; }
            public Double TypeClassSbj { get; set; }
        }

        //public class Type_Filter
        //{
        //    public const Int32 LESSONSCHEDULE = 1, //Theo lịch giáo viên phân công
        //   SHOWALL = 2, //Tất cả
        //   TIME = 3; //Theo thời gian
        //}
    }
}
