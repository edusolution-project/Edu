using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using MongoDB.Driver;
using System.Linq;
using Core_v2.Globals;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using BaseEasyRealTime.Entities;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Google.Apis.Drive.v3.Data;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Diagnostics;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class ClassController : TeacherController
    {
        private readonly GradeService _gradeService;
        private readonly AccountService _accountService;
        private readonly SubjectService _subjectService;
        private readonly TeacherService _teacherService;
        private readonly ClassService _service;
        //private readonly ClassStudentService _classStudentService;
        private readonly SkillService _skillService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly CourseService _courseService;
        private readonly CourseHelper _courseHelper;
        private readonly ClassHelper _classHelper;
        private readonly LessonHelper _lessonHelper;

        private readonly ClassProgressService _classProgressService;
        private readonly ClassSubjectProgressService _classSubjectProgressService;
        private readonly ProgressHelper _progressHelper;

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
        private readonly IHostingEnvironment _env;

        private readonly ChapterProgressService _chapterProgressService;

        private readonly ExamService _examService;
        private readonly ExamDetailService _examDetailService;

        private readonly FileProcess _fileProcess;
        private readonly StudentHelper _studentHelper;
        private readonly TeacherHelper _teacherHelper;
        private readonly MailHelper _mailHelper;

        private readonly MappingEntity<LessonEntity, StudentModuleViewModel> _moduleViewMapping;
        private readonly MappingEntity<LessonEntity, StudentAssignmentViewModel> _assignmentViewMapping;

        private readonly CenterService _centerService;
        private readonly RoleService _roleService;
        private readonly CloneLessonPartService _cloneLessonPartService;
        private readonly CloneLessonPartAnswerService _cloneLessonPartAnswerService;
        private readonly CloneLessonPartQuestionService _cloneLessonPartQuestionService;
        private readonly LessonPartService _lessonPartService;
        private readonly LessonPartQuestionService _lessonPartQuestionService;

        private readonly GroupService _groupService;

        private readonly List<string> quizType = new List<string> { "QUIZ1", "QUIZ2", "QUIZ3", "QUIZ4", "ESSAY" };
        private readonly CourseChapterService _courseChapterService;
        private readonly CourseLessonService _courseLessonService;
        private readonly MappingEntity<CourseChapterEntity, CourseChapterEntity> _cloneCourseChapterMapping = new MappingEntity<CourseChapterEntity, CourseChapterEntity>();
        private readonly MappingEntity<CourseLessonEntity, CourseLessonEntity> _cloneCourseLessonMapping = new MappingEntity<CourseLessonEntity, CourseLessonEntity>();
        private readonly LessonPartAnswerService _lessonPartAnswerService;
        private readonly MappingEntity<CourseEntity, CourseEntity> _cloneCourseMapping = new MappingEntity<CourseEntity, CourseEntity>();
        private readonly ClassService _classService;

        public ClassController(
            AccountService accountService,
            GradeService gradeservice,
            SubjectService subjectService,
            ClassSubjectService classSubjectService,
            //ClassStudentService classStudentService,
            TeacherService teacherService,
            ClassService service,
            SkillService skillService,
            CourseService courseService,
            CourseHelper courseHelper,
            ClassHelper classHelper,

            ClassProgressService classProgressService,
            ClassSubjectProgressService classSubjectProgressService,
            ProgressHelper progressHelper,

            ChapterService chapterService,
            ChapterExtendService chapterExtendService,
            LessonService lessonService,
            LessonScheduleService lessonScheduleService,
            ExamService examService,
            ExamDetailService examDetailService,
            LearningHistoryService learningHistoryService,

            ScoreStudentService scoreStudentService,
            LessonProgressService lessonProgressService,

            StudentService studentService, IHostingEnvironment evn,

            FileProcess fileProcess,
            LessonHelper lessonHelper,
            StudentHelper studentHelper,
            TeacherHelper teacherHelper,
            MailHelper mailHelper,

            ChapterProgressService chapterProgressService,
            CenterService centerService
            , CloneLessonPartService cloneLessonPartService
            , CloneLessonPartAnswerService cloneLessonPartAnswerService
            , CloneLessonPartQuestionService cloneLessonPartQuestionService
            , LessonPartService lessonPartService
            , LessonPartQuestionService lessonPartQuestionService,
            RoleService roleService,
            GroupService groupService,
            CourseChapterService courseChapterService,
            CourseLessonService courseLessonService,
            LessonPartAnswerService lessonPartAnswerService,
            ClassService classService
            )
        {
            _accountService = accountService;
            _gradeService = gradeservice;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _courseService = courseService;
            _courseHelper = courseHelper;
            _classHelper = classHelper;
            _service = service;
            _skillService = skillService;
            _classSubjectService = classSubjectService;
            //_classStudentService = classStudentService;
            _classProgressService = classProgressService;
            _classSubjectProgressService = classSubjectProgressService;
            _progressHelper = progressHelper;

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

            _studentHelper = studentHelper;
            _teacherHelper = teacherHelper;
            _lessonHelper = lessonHelper;
            _mailHelper = mailHelper;

            _moduleViewMapping = new MappingEntity<LessonEntity, StudentModuleViewModel>();
            _assignmentViewMapping = new MappingEntity<LessonEntity, StudentAssignmentViewModel>();

            _chapterProgressService = chapterProgressService;
            _centerService = centerService;
            _roleService = roleService;
            _cloneLessonPartService = cloneLessonPartService;
            _cloneLessonPartQuestionService = cloneLessonPartQuestionService;
            _lessonPartService = lessonPartService;
            _lessonPartQuestionService = lessonPartQuestionService;
            _cloneLessonPartAnswerService = cloneLessonPartAnswerService;
            _groupService = groupService;
            _courseChapterService = courseChapterService;
            _courseLessonService = courseLessonService;
            _lessonPartAnswerService = lessonPartAnswerService;
            _classService = classService;
        }

        public IActionResult Index(DefaultModel model, string basis, int old = 0)
        {

            var UserID = User.Claims.GetClaimByType("UserID").Value;
            var teacher = _teacherService.CreateQuery().Find(t => t.ID == UserID).SingleOrDefault();

            if (teacher != null && teacher.Subjects != null)
            {
                var subject = _subjectService.CreateQuery().Find(t => teacher.Subjects.Contains(t.ID)).ToList();
                var grade = _gradeService.CreateQuery().Find(t => teacher.Subjects.Contains(t.SubjectID)).ToList();
                ViewBag.Grades = grade;
                ViewBag.Subjects = subject;
                ViewBag.Skills = _skillService.GetList();
            }
            var center = new CenterEntity();
            if (!string.IsNullOrEmpty(basis))
            {
                center = _centerService.GetItemByCode(basis);
                if (center != null)
                    ViewBag.Center = center;
            }
            ViewBag.IsHeadTeacher = _teacherHelper.HasRole(UserID, center.ID, "head-teacher");

            ViewBag.User = UserID;
            ViewBag.Model = model;
            ViewBag.Managable = CheckPermission(PERMISSION.COURSE_EDIT);
            if (old == 1)
                return View("Index_o");
            return View();
        }

        public IActionResult Detail(DefaultModel model, string basis)
        {
            //if (!string.IsNullOrEmpty(basis))
            //{
            //    var center = _centerService.GetItemByCode(basis);
            //    if (center != null)
            //        ViewBag.Center = center;
            //    var UserID = User.Claims.GetClaimByType("UserID").Value;
            //    ViewBag.IsHeadTeacher = _teacherHelper.HasRole(UserID, center.ID, "head-teacher");
            //}

            if (model == null) return null;
            var currentClass = _service.GetItemByID(model.ID);
            if (currentClass == null)
                return Redirect($"/{basis}{Url.Action("Index")}");
            var vm = new ClassViewModel(currentClass);
            var subjects = _classSubjectService.GetByClassID(currentClass.ID);
            var skillIDs = subjects.Select(t => t.SkillID).Distinct();
            var subjectIDs = subjects.Select(t => t.SubjectID).Distinct().Where(s => s != "").ToList();
            vm.SkillName = string.Join(", ", _skillService.GetList().Where(t => skillIDs.Contains(t.ID)).Select(t => t.Name).ToList());
            vm.SubjectName = string.Join(", ", _subjectService.Collection.Find(t => subjectIDs.Contains(t.ID)).Project(t => t.Name).ToList());
            vm.TotalStudents = _studentService.CountByClass(currentClass.ID);
            ViewBag.Class = vm;
            //ViewBag.Subject = _subjectService.GetItemByID(currentClass.SubjectID);
            //ViewBag.Grade = _gradeService.GetItemByID(currentClass.GradeID);

            var UserID = User.Claims.GetClaimByType("UserID").Value;
            var teacher = _teacherService.CreateQuery().Find(t => t.ID == UserID).SingleOrDefault();
            if (teacher != null && teacher.Subjects != null)
            {
                var listsubjects = _subjectService.CreateQuery().Find(t => teacher.Subjects.Contains(t.ID)).ToList();
                var listgrades = _gradeService.CreateQuery().Find(t => teacher.Subjects.Contains(t.SubjectID)).ToList();
                ViewBag.Grades = listgrades;
                ViewBag.Subjects = listsubjects;
            }
            return View();
        }

        //public IActionResult References(DefaultModel model)
        //{
        //    if (model == null) return null;
        //    var currentClass = _service.GetItemByID(model.ID);
        //    if (currentClass == null)
        //        return Redirect($"/{basis}{Url.Action("Index");
        //    ViewBag.Class = currentClass;
        //    var UserID = User.Claims.GetClaimByType("UserID").Value;
        //    var myClasses = _service.CreateQuery()
        //        .Find(t => t.TeacherID == UserID)
        //        //.Find(t=> true)
        //        //.Project(Builders<ClassEntity>.Projection.Include(t => t.ID).Include(t => t.Name))
        //        .ToList();
        //    ViewBag.AllClass = myClasses;
        //    ViewBag.User = UserID;
        //    return View();
        //}

        public IActionResult Modules(DefaultModel model, string basis)
        {
            if (!string.IsNullOrEmpty(basis))
            {
                var center = _centerService.GetItemByCode(basis);
                if (center != null)
                    ViewBag.Center = center;
            }
            if (model == null) return null;
            var currentClass = _service.GetItemByID(model.ID);
            if (currentClass == null)
                return Redirect($"/{basis}{Url.Action("Index")}");
            ViewBag.Class = currentClass;
            return View();
        }

        public IActionResult StudentDetail(string basis, string ID, string ClassID)
        {
            if (!string.IsNullOrEmpty(basis))
            {
                var center = _centerService.GetItemByCode(basis);
                if (center != null)
                    ViewBag.Center = center;
            }
            if (string.IsNullOrEmpty(ClassID))
                return Redirect($"/{basis}{Url.Action("Index")}");
            var currentClass = _service.GetItemByID(ClassID);
            if (currentClass == null)
                return Redirect($"/{basis}{Url.Action("Index")}");
            ViewBag.Class = currentClass;

            if (string.IsNullOrEmpty(ID))
                return Redirect($"/{basis}{Url.Action("Member", "Class", new { ID = ClassID })}");

            var student = _studentService.GetItemByID(ID);
            if (student == null)
                return Redirect($"/{basis}{Url.Action("Member", "Class", new { ID = ClassID })}");

            ViewBag.Student = student;

            return View();
        }

        #region ClassDetail

        public JsonResult GetListTeacher(string basis, string SubjectID = "")
        {
            var filter = new List<FilterDefinition<TeacherEntity>>();

            var UserID = User.Claims.GetClaimByType("UserID").Value;

            if (string.IsNullOrEmpty(SubjectID))
                return new JsonResult(new Dictionary<string, object> { });
            filter.Add(Builders<TeacherEntity>.Filter.Where(o => o.Centers.Any(c => c.Code == basis)));
            filter.Add(Builders<TeacherEntity>.Filter.Where(o => o.Subjects.Contains(SubjectID)));
            //hide huonghl@utc.edu.vn
            if (basis != "eduso")
                filter.Add(Builders<TeacherEntity>.Filter.Where(t => t.Email != "huonghl@utc.edu.vn"));
            var teachers = _teacherService.Collection.Find(Builders<TeacherEntity>.Filter.And(filter));

            var response = new Dictionary<string, object>
            {
                { "Data", teachers.ToList().Select(o => new
                    {
                       id = o.ID,
                       fullname = o.FullName
                    }
                    ).ToList()
                }
            };
            return new JsonResult(response);
        }

        public JsonResult GetMarks(DefaultModel model)
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
            filter.Add(Builders<StudentEntity>.Filter.Where(o => currentClass.Students.Contains(o.ID)));
            var students = filter.Count > 0 ? _studentService.Collection.Find(Builders<StudentEntity>.Filter.And(filter)) : _studentService.GetAll();
            var studentsView = students.ToList().Select(t => _mapping.AutoOrtherType(t, new ClassStudentViewModel()
            {
                ClassName = currentClass.Name,
                ClassStatus = "Đang học",
                LastJoinDate = DateTime.UtcNow
            })).ToList();

            var response = new Dictionary<string, object>
            {
                { "Data",new Dictionary<string, object> {
                        {"Teacher",teacher },
                        {"Students",studentsView }
                    }
                },
                { "Model", model }
            };
            return new JsonResult(response);
        }

        public JsonResult GetClassResult(String basis, String ClassID, DefaultModel model)
        {
            var @class = _classService.GetItemByID(ClassID);
            if (@class == null)
            {
                return Json("Lớp không tồn tại.");
            }

            //Lay danh sach ID hoc sinh trong lop
            var students = _studentService.GetStudentsByClassId(@class.ID).ToList();
            if (students.Count == 0)
            {
                return Json("Lớp chưa có học viên.");
            }
            var studentIds = students.Select(t => t.ID).ToList();

            //lay danh sach classsubject trong lop
            var classsubjects = _classSubjectService.GetByClassID(@class.ID);
            if (classsubjects.Count == 0)
            {
                return Json("Lớp chưa có môn học");
            }
            var classsubjectsIDs = classsubjects.Select(x => x.ID).ToList();

            var progess = _lessonProgressService.CreateQuery().Find(x => classsubjectsIDs.Contains(x.ClassSubjectID) && x.Tried > 0);
            var activelesson = _lessonScheduleService.CreateQuery().Find(o => classsubjectsIDs.Contains(o.ClassSubjectID) && (o.StartDate <= DateTime.Now || o.StartDate <= @class.EndDate) && o.EndDate >= @class.StartDate).ToList();
            var litsIds = activelesson.Select(x => x.LessonID);
            var listIdsPractice = _lessonService.CreateQuery().Find(x => litsIds.Contains(x.ID) && x.IsPractice == true).ToList().Select(x=>x.ID);
            var listIdsExam = _lessonService.CreateQuery().Find(x => litsIds.Contains(x.ID) && x.TemplateType == 2).ToList().Select(x=>x.ID);
            var progessPractice = progess.ToList().Where(x => listIdsPractice.Contains(x.LessonID) && x.Tried > 0).GroupBy(x => x.StudentID).Select(x => new { StudentID = x.Key, Point = x.ToList().Sum(y => y.LastPoint) / listIdsPractice.Count() });
            var progessExam = progess.ToList().Where(x => listIdsExam.Contains(x.LessonID) && x.Tried > 0).GroupBy(x => x.StudentID).Select(x => new { StudentID = x.Key, Point = x.ToList().Sum(y => y.LastPoint) / listIdsExam.Count() });

            var data = new List<StudentSummaryViewModel>();
            var dataResult = progess.ToList().GroupBy(x => x.StudentID).Select(x=>
            new StudentSummaryViewModel
            {
                StudentID = x.Key,
                FullName = students.Where(y=>y.ID == x.Key).FirstOrDefault().FullName,
                PracticeAvgPoint = listIdsPractice.Count() > 0 ? x.ToList().Where(y=> listIdsPractice.Contains(y.LessonID)).Sum(y=>y.LastPoint) / listIdsPractice.Count() : 0,
                AvgPoint = listIdsExam.Count() > 0 ? x.ToList().Where(y => listIdsExam.Contains(y.LessonID)).Sum(y => y.LastPoint) / listIdsExam.Count() : 0
            });

            if(dataResult.Count() != students.Count())
            {
                foreach(var st in students)
                {
                    var item = dataResult.Where(x => x.StudentID == st.ID).FirstOrDefault();
                    if (item == null)
                    {
                        item.StudentID = st.ID;
                        item.FullName = st.FullName;
                        item.PracticeAvgPoint = 0;
                        item.AvgPoint = 0;
                    }
                    data.Add(item);
                }
            }
            else
            {
                data.AddRange(dataResult);
            }
            //data.AddRange(dataResult);

            Dictionary<String, Object> dataresponse = new Dictionary<string, object>
            {
                {"Data",data },
                {"Model",model }
            };
            return Json(dataresponse);
        }

        //public JsonResult GetClassResult(DefaultModel model, string ClassID, DateTime startTime, DateTime endTime, Boolean isFillter = false)
        //{
        //    var @class = _service.GetItemByID(ClassID);
        //    if (@class == null)
        //        return null;
        //    var data = new List<StudentSummaryViewModel>();
        //    if (!isFillter)
        //    {
        //        List<StudentSummaryViewModel> listSummary = ClassResultSummary(@class);
        //        var response = new Dictionary<string, object>
        //            {
        //                { "Data", listSummary},
        //                { "Model", model }
        //            };
        //        return new JsonResult(response);
        //    }
        //    else
        //    {
        //        startTime = startTime.AddMilliseconds(1);
        //        endTime = endTime.AddHours(23).AddMinutes(59).AddMilliseconds(59);
        //        //Lay danh sach ID hoc sinh trong lop
        //        var students = _studentService.GetStudentsByClassId(@class.ID).ToList();
        //        var studentIds = students.Select(t => t.ID).ToList();
        //        //totalStudent += studentIds.Count();

        //        var classStudent = studentIds.Count();

        //        //Lay danh sach ID bai hoc duoc mo trong tuan
        //        var activeLessons = _lessonScheduleService.CreateQuery().Find(o => o.ClassID == @class.ID && o.StartDate <= endTime && o.EndDate >= startTime).ToList();
        //        var activeLessonIds = activeLessons.Select(t => t.LessonID).ToList();

        //        var totalPractice = 0;
        //        var totalExam = 0;
        //        foreach (var item in activeLessonIds)
        //        {
        //            var lesson = _lessonService.GetItemByID(item);
        //            if (lesson.IsPractice)
        //            {
        //                totalPractice++;
        //            }
        //            else if (lesson.TemplateType == 2)
        //            {
        //                totalExam++;
        //            }
        //        }

        //        //Lay danh sach hoc sinh da hoc cac bai tren trong tuan
        //        var activeProgress = _lessonProgressService.CreateQuery().Find(
        //            x => studentIds.Contains(x.StudentID) && activeLessonIds.Contains(x.LessonID)
        //            && x.LastDate <= endTime && x.LastDate >= startTime).ToEnumerable();

        //        // danh sach bai kiem tra
        //        var listExam = _lessonService.CreateQuery().Find(x => (x.TemplateType == 2) && activeLessonIds.Contains(x.ID)).ToList();
        //        var examIds = listExam.Select(x => x.ID).ToList();

        //        var exams = (from e in _examService.CreateQuery().Find(x => examIds.Contains(x.LessonID)).ToList()
        //                     group e by e.StudentID
        //                     into ge
        //                     select new
        //                     {
        //                         StudentID = ge.Key,
        //                         CompletedExam = ge.ToList().Select(x => x.LessonID).Distinct().Count(),
        //                         test = ge
        //                     }).ToList();

        //        //danh sach bai luyen tap
        //        var listPractice = _lessonService.CreateQuery().Find(x => (x.IsPractice == true) && activeLessonIds.Contains(x.ID)).ToList();
        //        var practiceIds = listPractice.Select(x => x.ID).ToList();

        //        var practices = (from e in _examService.CreateQuery().Find(x => practiceIds.Contains(x.LessonID)).ToList()
        //                         group e by e.StudentID
        //                     into ge
        //                         select new
        //                         {
        //                             StudentID = ge.Key,
        //                             CompletedPractice = ge.ToList().Select(x => x.LessonID).Distinct().Count(),
        //                             test = ge
        //                         }).ToList();

        //        //ket qua lam bai kiem tra cua hoc sinh trong lop
        //        var classResult1 = (from r in activeProgress.Where(t => examIds.Contains(t.LessonID) && t.Tried > 0)
        //                            group r by r.StudentID
        //                           into g
        //                            let _CompletedExam = exams.Where(x => x.StudentID == g.Key).FirstOrDefault().CompletedExam
        //                            select new StudentResult
        //                            {
        //                                StudentID = g.Key,
        //                                AvgPoint = g.Average(t => t.LastPoint),
        //                                StudentName = _studentService.GetItemByID(g.Key)?.FullName.Trim(),
        //                                CompletedExam = _CompletedExam,
        //                                TotalLesson = activeLessonIds.Count,
        //                                isExam = true
        //                            }).ToList();

        //        //ket qua lam bai luyen tap cua hoc sinh trong lop
        //        var classResult2 = (from r in activeProgress.Where(t => practiceIds.Contains(t.LessonID) && t.Tried > 0)
        //                            group r by r.StudentID
        //                           into g
        //                            let _CompletedPractice = practices.Where(x => x.StudentID == g.Key).FirstOrDefault().CompletedPractice
        //                            select new StudentResult
        //                            {
        //                                StudentID = g.Key,
        //                                AvgPoint = g.Average(t => t.LastPoint),
        //                                StudentName = _studentService.GetItemByID(g.Key)?.FullName.Trim(),
        //                                CompletedPractice = _CompletedPractice,
        //                                TotalPractice = activeLessonIds.Count,
        //                                isExam = false
        //                            }).ToList();

        //        var results = _progressHelper.GetClassResults(@class.ID)
        //        .OrderByDescending(t => t.RankPoint).ToList();

        //        List<StudentSummaryViewModel> studentSummaryViewModels = new List<StudentSummaryViewModel>();
        //        foreach (var item in students)
        //        {
        //            var totalStudent = students.Count();
        //            var result = results.FirstOrDefault(t => t.StudentID == item.ID) ?? new StudentRankingViewModel();
        //            var rankPoint = _progressHelper.CalculateRankPoint(result.TotalPoint, result.PracticePoint, result.Count);
        //            var exam = classResult1.Find(x => x.StudentID == item.ID);
        //            var practice = classResult2.Find(x => x.StudentID == item.ID);

        //            var studentSummary = new StudentSummaryViewModel
        //            {
        //                ExamDone = exam != null ? exam.CompletedExam : 0,
        //                Completed = result.Count,
        //                PracticeDone = practice != null ? practice.CompletedPractice : 0,
        //                StudentID = item.ID,
        //                FullName = item.FullName?.ToString(),
        //                RankPoint = rankPoint,
        //                Rank = results.FindIndex(t => t.RankPoint == rankPoint) + 1,
        //                ExamResult = @class.TotalExams > 0 ? result.TotalPoint / @class.TotalExams : 0,
        //                PracticeResult = @class.TotalPractices > 0 ? result.PracticePoint / @class.TotalPractices : 0,
        //                TotalPoint = result.TotalPoint,
        //                PracticePoint = result.PracticePoint,
        //                PracticeAvgPoint = practice != null && practice.CompletedPractice > 0 ? practice.AvgPoint : 0,
        //                AvgPoint = exam != null && exam.CompletedExam > 0 ? exam.AvgPoint : 0,
        //                TotalExams = totalExam,
        //                TotalLessons = @class.TotalLessons,
        //                TotalPractices = totalPractice,
        //                TotalStudents = totalStudent,
        //                LastDate = result.LastDate
        //            };
        //            studentSummaryViewModels.Add(studentSummary);
        //        }
        //        var response = new Dictionary<string, object>
        //            {
        //                { "Data", studentSummaryViewModels},
        //                { "Model", model }
        //            };
        //        return new JsonResult(response);
        //    }
        //}

        private List<StudentSummaryViewModel> ClassResultSummary(ClassEntity @class)
        {
            var students = _studentService.GetStudentsByClassId(@class.ID).Select(t => new StudentEntity { ID = t.ID, FullName = t.FullName });
            var total_students = students.Count();

            var results = _progressHelper.GetClassResults(@class.ID)
                .OrderByDescending(t => t.RankPoint).ToList();

            var listSummary = (from student in students
                               let result = results.FirstOrDefault(t => t.StudentID == student.ID) ?? new StudentRankingViewModel()
                               let rankPoint = _progressHelper.CalculateRankPoint(result.TotalPoint, result.PracticePoint, result.Count)
                               select new StudentSummaryViewModel
                               {
                                   ExamDone = result.ExamDone,
                                   Completed = result.Count,
                                   PracticeDone = result.PracticeDone,
                                   StudentID = student.ID,
                                   FullName = student.FullName,
                                   RankPoint = rankPoint,
                                   Rank = results.FindIndex(t => t.RankPoint == rankPoint) + 1,
                                   ExamResult = @class.TotalExams > 0 ? result.TotalPoint / @class.TotalExams : 0,
                                   PracticeResult = @class.TotalPractices > 0 ? result.PracticePoint / @class.TotalPractices : 0,
                                   TotalPoint = result.TotalPoint,
                                   PracticePoint = result.PracticePoint,
                                   PracticeAvgPoint = result.PracticeDone > 0 ? result.PracticePoint / result.PracticeDone : 0,
                                   AvgPoint = result.ExamDone > 0 ? result.TotalPoint / result.ExamDone : 0,
                                   TotalExams = @class.TotalExams,
                                   TotalLessons = @class.TotalLessons,
                                   TotalPractices = @class.TotalPractices,
                                   TotalStudents = total_students,
                                   LastDate = result.LastDate
                               }).ToList();
            return listSummary;
        }

        public JsonResult GetClass_Result(DefaultModel model, string ClassID, DateTime startTime, DateTime endTime)
        {
            var @class = _classService.GetItemByID(ClassID);
            if (@class == null)
            {
                var dataresponse = new Dictionary<String, Object>
                {
                    {"Stt",false },
                    { "Msg","Không tìm thấy lớp"}
                };
                return new JsonResult(dataresponse);
            }
            else
            {
                startTime = startTime.AddMilliseconds(1);
                endTime = endTime.AddHours(23).AddMilliseconds(59);
                //Lay danh sach ID hoc sinh trong lop
                var students = _studentService.GetStudentsByClassId(@class.ID).ToList();
                var studentIds = students.Select(t => t.ID).ToList();
                //totalStudent += studentIds.Count();

                var classStudent = studentIds.Count();

                //Lay danh sach ID bai hoc duoc mo trong tuan
                var activeLessons = _lessonScheduleService.CreateQuery().Find(o => o.ClassID == @class.ID && o.StartDate <= endTime && o.EndDate >= startTime).ToList();
                var activeLessonIds = activeLessons.Select(t => t.LessonID).ToList();

                //Lay danh sach hoc sinh da hoc cac bai tren trong tuan
                var activeProgress = _lessonProgressService.CreateQuery().Find(
                    x => studentIds.Contains(x.StudentID) && activeLessonIds.Contains(x.LessonID)
                    && x.LastDate <= endTime && x.LastDate >= startTime).ToEnumerable();

                // danh sach bai kiem tra + luyen tap
                var examIds = _lessonService.CreateQuery().Find(x => (x.TemplateType == 2 || x.IsPractice == true) && activeLessonIds.Contains(x.ID)).ToList();

                var exams = (from e in _examService.CreateQuery().Find(x => examIds.Select(y => y.ID).Contains(x.LessonID)).ToList()
                             group e by e.StudentID
                             into ge
                             select new
                             {
                                 StudentID = ge.Key,
                                 CompletedLesson = ge.ToList().Select(x => x.LessonID).Distinct().Count()
                             }).ToList();

                //ket qua lam bai cua hoc sinh trong lop
                var classResult = (from r in activeProgress.Where(t => examIds.Select(y => y.ID).Contains(t.LessonID) && t.Tried > 0)
                                   group r by r.StudentID
                                   into g
                                   let _CompletedLesson = exams.Where(x => x.StudentID == g.Key).FirstOrDefault().CompletedLesson
                                   select new StudentResult
                                   {
                                       StudentID = g.Key,
                                       ExamCount = g.Count() == 0 ? 0 : g.Count(),
                                       AvgExamPoint = g.Average(t => t.LastPoint),
                                       StudentName = _studentService.GetItemByID(g.Key)?.FullName.Trim(),
                                       CompletedLesson = _CompletedLesson,
                                       TotalLesson = activeLessonIds.Count,

                                   }).ToList();

                foreach (var item in students)
                {
                    var studentResult = classResult.Where(x => x.StudentID == item.ID).FirstOrDefault();
                    if (studentResult == null)
                    {
                        studentResult = new StudentResult { StudentID = item.ID, ExamCount = 0, AvgExamPoint = 0, StudentName = item.FullName.Trim(), AvgTimeDoExam = "--", CompletedLesson = 0, TotalLesson = activeLessonIds.Count() };
                        classResult.Add(studentResult);
                    }
                }

                List<StudentResult> _classResult = new List<StudentResult>();
                foreach (var item in students)
                {
                    var studentResult = classResult.Where(x => x.StudentID == item.ID).FirstOrDefault();
                    _classResult.Add(studentResult);
                }

                var results = _progressHelper.GetClassResults(@class.ID)
                .OrderByDescending(t => t.RankPoint).ToList();

                List<StudentSummaryViewModel> studentSummaryViewModels = new List<StudentSummaryViewModel>();
                foreach (var item in _classResult)
                {
                    var totalStudent = _classResult.Count();
                    var result = results.FirstOrDefault(t => t.StudentID == item.StudentID) ?? new StudentRankingViewModel();
                    var rankPoint = _progressHelper.CalculateRankPoint(result.TotalPoint, result.PracticePoint, result.Count);
                    var studentSummary = new StudentSummaryViewModel
                    {
                        ExamDone = item.CompletedLesson,
                        Completed = result.Count,
                        PracticeDone = result.PracticeDone,
                        StudentID = item.StudentID,
                        FullName = item.StudentName,
                        RankPoint = rankPoint,
                        Rank = results.FindIndex(t => t.RankPoint == rankPoint) + 1,
                        ExamResult = @class.TotalExams > 0 ? result.TotalPoint / @class.TotalExams : 0,
                        PracticeResult = @class.TotalPractices > 0 ? result.PracticePoint / @class.TotalPractices : 0,
                        TotalPoint = result.TotalPoint,
                        PracticePoint = result.PracticePoint,
                        PracticeAvgPoint = result.PracticeDone > 0 ? result.PracticePoint / result.PracticeDone : 0,
                        AvgPoint = result.ExamDone > 0 ? result.TotalPoint / result.ExamDone : 0,
                        TotalExams = @class.TotalExams,
                        TotalLessons = @class.TotalLessons,
                        TotalPractices = @class.TotalPractices,
                        TotalStudents = totalStudent,
                        LastDate = result.LastDate
                    };
                    studentSummaryViewModels.Add(studentSummary);
                }
                var response = new Dictionary<string, object>
                    {
                        { "Data", studentSummaryViewModels},
                        { "Model", model }
                    };
                return new JsonResult(response);
            }
        }

        public async Task<JsonResult> GetClassSubjectResult(DefaultModel model, string ClassSubjectID)
        {
            try
            {
                var lesson = _lessonService.CreateQuery().Find(x => x.ClassSubjectID == ClassSubjectID).FirstOrDefault();
                if (lesson == null)
                {
                    return new JsonResult(new Dictionary<String, Object> {
                                        {"Error","Chưa có bài học" },
                                        {"DataStudent",""},
                                        {"DataTime","" }
                                    });
                }
                var @class = _classService.GetItemByID(lesson.ClassID);
                var sbj = _classSubjectService.GetItemByID(lesson.ClassSubjectID);
                var startDate = sbj.StartDate;
                var listStudent = _studentService.GetStudentsByClassId(@class.ID);
                var listTime = GetListWeek(startDate);
                Dictionary<String, Object> dataResponse = new Dictionary<string, object>();
                Dictionary<String, Object> dataTime = new Dictionary<string, object>();
                
                var start = listTime.FirstOrDefault().Value.StartTime;
                var end = listTime.LastOrDefault().Value.EndTime;

                var progress = _lessonProgressService.CreateQuery().Find(t => t.ClassSubjectID == sbj.ID).ToList();
                var activeLessons = _lessonScheduleService.GetActiveLesson(start, end, sbj.ID).ToList();
                var a = activeLessons.Select(y => y.LessonID).ToList();
                var activelessonPractice = _lessonService.CreateQuery().Find(x => a.Contains(x.ID) && x.IsPractice).ToList();

                var index = 1;

                var activeLessonDic = new Dictionary<int, List<string>>();

                foreach (var student in listStudent)
                {
                    List<StudentLessonResultViewModel> result = new List<StudentLessonResultViewModel>();
                    List<StudentDetailVM> dataresponse = new List<StudentDetailVM>();

                    var countLessonids = 0;
                    foreach (var item in listTime)
                    {
                        if (!activeLessonDic.ContainsKey(item.Key))
                        {
                            var activeIds = activeLessons.Where(t => t.EndDate > item.Value.StartTime && t.StartDate <= item.Value.EndTime).Select(t => t.LessonID).ToList();
                            var activePractice = _lessonService.CreateQuery().Find(t => activeIds.Contains(t.ID) && (t.IsPractice || t.TemplateType == LESSON_TEMPLATE.EXAM)).Project(t => t.ID).ToList();
                            activeLessonDic[item.Key] = activePractice;
                        }

                        var lessonids = activeLessonDic[item.Key];
                        countLessonids += lessonids.Count();

                        var data = new StudentDetailVM();
                        data.StudentName = student.FullName;
                        data.StudentID = student.ID;
                        data.Week = item.Key;

                        var presult = progress.Where(t => t.StudentID == student.ID && lessonids.Contains(t.LessonID));
                        if (lessonids.Count() == 0) continue;
                        var point = presult.Count() > 0 ? (presult.Sum(x => x.LastPoint)/lessonids.Count()).ToString() : "---";
                        data.Point = point.ToString();

                        data.StartTime = item.Value.StartTime;
                        data.EndTime = item.Value.EndTime;
                        data.TotalPractice = lessonids.Count();
                        data.TotalLessons = activelessonPractice.ToList().Count();

                        dataresponse.Add(data);

                        if (!dataTime.ContainsKey((item.Key - 1).ToString()))
                        {
                            dataTime.Add((item.Key-1).ToString(), new { item.Value.StartTime, item.Value.EndTime });
                        }
                    }

                    dataResponse.Add(index.ToString(), dataresponse);
                    index++;
                }
                
                return new JsonResult(new Dictionary<String, Object> {
                                        {"DataStudent",dataResponse },
                                        {"DataTime",dataTime },
                                        {"ListStudents",null }
                                    });
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        //public JsonResult GetClassSubjectResult(DefaultModel model, string ClassSubjectID)
        //{
        //    var csbj = _classSubjectService.GetItemByID(ClassSubjectID);
        //    if (csbj == null)
        //        return null;
        //    var data = new List<StudentSummaryViewModel>();

        //    var students = _studentService.GetStudentsByClassId(csbj.ClassID).Select(t => new StudentEntity { ID = t.ID, FullName = t.FullName });

        //    var total_students = students.Count();

        //    var csbjResults = _progressHelper.GetClassSubjectResults(csbj.ID)
        //        .OrderByDescending(t => t.RankPoint).ToList();

        //    var listSummary = (from student in students
        //                       let result = csbjResults.FirstOrDefault(t => t.StudentID == student.ID) ?? new StudentRankingViewModel()
        //                       let rankPoint = _progressHelper.CalculateRankPoint(result.TotalPoint, result.PracticePoint, result.Count)
        //                       select new StudentSummaryViewModel
        //                       {
        //                           ExamDone = result.ExamDone,
        //                           PracticeDone = result.PracticeDone,
        //                           Completed = result.Count,
        //                           StudentID = student.ID,
        //                           FullName = student.FullName,
        //                           RankPoint = rankPoint,
        //                           Rank = csbjResults.FindIndex(t => t.RankPoint == rankPoint) + 1,
        //                           ExamResult = csbj.TotalExams > 0 ? result.TotalPoint / csbj.TotalExams : 0,
        //                           PracticeResult = csbj.TotalPractices > 0 ? result.PracticePoint / csbj.TotalPractices : 0,
        //                           TotalPoint = result.TotalPoint,
        //                           PracticePoint = result.PracticePoint,
        //                           PracticeAvgPoint = result.PracticeDone > 0 ? result.PracticePoint / result.PracticeDone : 0,
        //                           AvgPoint = result.ExamDone > 0 ? result.TotalPoint / result.ExamDone : 0,
        //                           TotalExams = csbj.TotalExams,
        //                           TotalLessons = csbj.TotalLessons,
        //                           TotalPractices = csbj.TotalPractices,
        //                           TotalStudents = total_students,
        //                           LastDate = result.LastDate
        //                       }).ToList();
        //    var rootChapters = _chapterService.GetSubChapters(ClassSubjectID, "0").ToList();
        //    var chapterSummary = from r in rootChapters
        //                         select _progressHelper.GetChapterResults(r.ID).ToList();

        //    var response = new Dictionary<string, object>
        //    {
        //        { "Data", listSummary},
        //        { "Chapter", rootChapters },
        //        { "Student", students },
        //        { "Result", chapterSummary }
        //    };
        //    return new JsonResult(response);
        //}

        public JsonResult GetStudentSummary(String basis, String StudentID, String ClassID)
        {
            try
            {
                var student = _studentService.GetItemByID(StudentID);
                if (student == null)
                {
                    return Json("Không tìm thấy học viên.");
                }

                var center = _centerService.GetItemByCode(basis);
                if (center == null)
                {
                    return Json("Cơ sở không tồn tại.");
                }

                var @class = _classService.GetItemByID(ClassID);
                if (@class == null)
                {
                    return Json("Lớp không tồn tại");
                }

                Dictionary<String, Object> data_response = new Dictionary<String, Object>();
                var data = new List<StudentSummaryViewModel>();
                //var dataExam = new List<StudentSummaryExamViewModel>();
                var dataExam = new List<StudentExamVM>();

                var subjects = _classSubjectService.GetByClassID(@class.ID);
                var ClassSbjIDs = subjects.Select(x => x.ID).ToList();

                var progess = _lessonProgressService.CreateQuery().Find(x => ClassSbjIDs.Contains(x.ClassSubjectID) && x.StudentID == StudentID).ToList();
                var activelesson = _lessonScheduleService.CreateQuery().Find(x=>ClassSbjIDs.Contains(x.ClassSubjectID) && x.StartDate <= DateTime.Now && x.EndDate >= @class.StartDate).ToList();
                var activelessonIDs = activelesson.Select(x => x.LessonID).ToList();
                var lessonExam = _lessonService.CreateQuery().Find(x => activelessonIDs.Contains(x.ID) && x.TemplateType == 2).ToList();
                var lessonIDsExam = lessonExam.Select(x=>x.ID);
                var lessonPractice= _lessonService.CreateQuery().Find(x => activelessonIDs.Contains(x.ID) && x.IsPractice).ToList();
                var lessonIDsPractice= lessonPractice.Select(x=>x.ID);
                var practiceResult = progess.Where(x=>x.StudentID == StudentID && lessonIDsPractice.Contains(x.LessonID)).GroupBy(x => x.ClassSubjectID).Select(x =>
                    new StudentSummaryViewModel
                    {
                        StudentID = StudentID,
                        ClassSubjectID = x.Key,
                        CourseName = subjects.Where(y => y.ID == x.Key).FirstOrDefault().CourseName,
                        ClassID = @class.ID,
                        TypeClassSbj = subjects.Where(y => y.ID == x.Key).FirstOrDefault().TypeClass,
                        TotalLessons = lessonPractice.Where(y => y.ClassSubjectID == x.Key).Count(),
                        PracticeAvgPoint = lessonPractice.Where(y => y.ClassSubjectID == x.Key).Count() > 0 ? x.ToList().Select(y => y.LastPoint).Sum()/ lessonPractice.Where(y => y.ClassSubjectID == x.Key).Count() : 0,
                    });

                var examResult = progess.Where(x => x.StudentID == StudentID && lessonIDsExam.Contains(x.LessonID) && x.Tried > 0);
                foreach(var item in examResult)
                {
                    var _item = new StudentExamVM()
                    {
                        StudentID = StudentID,
                        ClassSubjectID = item.ClassSubjectID,
                        ClassID = @class.ID,
                        TypeClassSbj = subjects.Where(y => y.ID == item.ClassSubjectID).FirstOrDefault().TypeClass,
                        LessonID = item.LessonID,
                        Point = item.LastPoint
                    };
                    dataExam.Add(_item);
                }

                for(int i=0;i<practiceResult.Count();i++)
                {
                    var item = practiceResult.ElementAtOrDefault(i);
                    item.Order = i + 1;
                    data.Add(item);
                }

                data_response.Add("Practice", data);
                data_response.Add("Exam", dataExam);
                return Json(data_response);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message);
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
            public String StudentName { get; set; }
            public String StudentID { get; set; }
            public int TotalLessons { get; set; }
            public Boolean inWeek { get; set; }
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

        public class Type_Filter
        {
            public const Int32 LESSONSCHEDULE = 1, //Theo lịch giáo viên phân công
           SHOWALL = 2, //Tất cả
           TIME = 3; //Theo thời gian
        }
        #endregion

        #region Homepage
        public JsonResult GetActiveList(DateTime today, string Center)
        {
            today = today.ToUniversalTime();
            var filter = new List<FilterDefinition<ClassEntity>>();
            filter.Add(Builders<ClassEntity>.Filter.Where(o => o.IsActive));
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new ReturnJsonModel
                {
                    StatusCode = ReturnStatus.ERROR,
                    StatusDesc = "Phiên làm việc đã kết thúc, yêu cầu đăng nhập lại"
                });
            }
            if (!string.IsNullOrEmpty(Center))
            {
                var @center = _centerService.GetItemByCode(Center);
                if (@center == null) return new JsonResult(new Dictionary<string, object>
                {
                    { "Error", "Cơ sở không đúng"}
                });
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Center == @center.ID));
            }
            filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Members.Any(t => t.TeacherID == userId && t.Type == ClassMemberType.TEACHER)));
            filter.Add(Builders<ClassEntity>.Filter.Where(o => (o.StartDate <= today) && (o.EndDate >= today)));

            var data = (filter.Count > 0 ? _service.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _service.GetAll()).SortByDescending(t => t.ID);

            var std = (from o in data.ToList()
                       let totalweek = (o.EndDate.Date - o.StartDate.Date).TotalDays / 7
                       //let subject = _subjectService.GetItemByID(o.SubjectID)
                       let studentCount = //_classStudentService.GetClassStudents(o.ID).Count
                       _studentService.CountByClass(o.ID)
                       select new
                       {
                           id = o.ID,
                           //courseID = o.CourseID,
                           courseName = o.Name,
                           //subjectName = subject == null ? "" : subject.Name,
                           thumb = o.Image ?? "",
                           endDate = o.EndDate,
                           //week = totalweek > 0 ? (DateTime.UtcNow.Date - o.StartDate.Date).TotalDays / 7 / totalweek : 0,
                           students = studentCount
                       }).ToList();
            return Json(new { Data = std });
        }

        public JsonResult GetFinishList(DefaultModel model, DateTime today, string Center)
        {
            today = today.ToUniversalTime();
            var filter = new List<FilterDefinition<ClassEntity>>();
            filter.Add(Builders<ClassEntity>.Filter.Where(o => o.IsActive));
            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            if (!string.IsNullOrEmpty(Center))
            {
                var @center = _centerService.GetItemByCode(Center);
                if (@center == null) return new JsonResult(new Dictionary<string, object>
                {
                    { "Error", "Cơ sở không đúng"}
                });
                filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Center == @center.ID));
            }
            filter.Add(Builders<ClassEntity>.Filter.Where(o => o.Members.Any(t => t.TeacherID == userId && t.Type == ClassMemberType.TEACHER)));
            filter.Add(Builders<ClassEntity>.Filter.Where(o => o.EndDate < today));

            var data = filter.Count > 0 ? _service.Collection.Find(Builders<ClassEntity>.Filter.And(filter)) : _service.GetAll();
            model.TotalRecord = data.CountDocuments();
            var DataResponse = data == null || model.TotalRecord <= 0 || model.TotalRecord < model.PageSize
                ? data.SortByDescending(t => t.ID).ToList()
                : data.SortByDescending(t => t.ID).Skip(model.PageIndex * model.PageSize).Limit(model.PageSize).ToList();

            var std = (from o in DataResponse
                       select new
                       {
                           id = o.ID,
                           //courseID = o.CourseID,
                           title = o.Name,
                           endDate = o.EndDate,
                       }).ToList();
            return Json(new { Data = std });
        }

        public JsonResult GetThisWeekLesson(DateTime today, string Center)
        {
            today = today.ToUniversalTime();
            if (today < new DateTime(1900, 1, 1))
                return Json(new { });
            var startWeek = today.AddDays(DayOfWeek.Sunday - today.DayOfWeek);
            var endWeek = startWeek.AddDays(7);

            var userId = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new ReturnJsonModel
                {
                    StatusCode = ReturnStatus.ERROR,
                    StatusDesc = "Authentication Error"
                });
            }

            var classFilter = new List<FilterDefinition<ClassEntity>>();
            if (!string.IsNullOrEmpty(Center))
            {
                var @center = _centerService.GetItemByCode(Center);
                if (@center == null) return new JsonResult(new Dictionary<string, object>
                {
                    { "Error", "Cơ sở không đúng"}
                });
                classFilter.Add(Builders<ClassEntity>.Filter.Where(o => o.Members.Any(t => t.TeacherID == userId) && o.Center == @center.ID));
            }

            var classIds = _service.Collection.Find(Builders<ClassEntity>.Filter.And(classFilter)).Project(t => t.ID).ToList();

            var filter = new List<FilterDefinition<LessonScheduleEntity>>();
            filter.Add(Builders<LessonScheduleEntity>.Filter.Where(o => o.StartDate <= endWeek && o.EndDate >= startWeek));
            filter.Add(Builders<LessonScheduleEntity>.Filter.Where(t => classIds.Contains(t.ClassID)));

            //var csIds = _lessonScheduleService.Collection.Distinct(t => t.ClassSubjectID, Builders<LessonScheduleEntity>.Filter.And(filter)).ToList();

            var data = _lessonScheduleService.Collection.Find(Builders<LessonScheduleEntity>.Filter.And(filter)).ToList();

            //var data = _classSubjectService.Collection.Find(t => csIds.Contains(t.ID));
            var std = (from o in data.ToList()
                       let _lesson = _lessonService.Collection.Find(t => t.ID == o.LessonID).SingleOrDefault()
                       where _lesson != null
                       let _class = _service.Collection.Find(t => t.ID == o.ClassID).SingleOrDefault()
                       where _class != null
                       let _sbj = _classSubjectService.GetItemByID(o.ClassSubjectID)
                       where _sbj != null
                       let skill = _skillService.GetItemByID(_sbj.SkillID)
                       let studentCount = _studentService.CountByClass(_class.ID)
                       select new
                       {
                           id = o.ID,
                           classID = _class.ID,
                           classsubjectID = _sbj.ID,
                           className = _class.Name,
                           title = _lesson.Title,
                           lessonID = _lesson.ID,
                           startDate = o.StartDate,
                           endDate = o.EndDate,
                           students = studentCount,
                           skill = skill,
                           bookName = _sbj.CourseName
                           //isLearnt = isLearnt
                       }).OrderBy(t => t.startDate).ToList();

            return Json(new { Data = std });
        }
        #endregion

        #region Manage
        public JsonResult GetManageList(DefaultModel model, string Center, string SubjectID = "", string GradeID = "", string TeacherID = "", bool skipActive = true)
        {
            var center = new CenterEntity();
            if (!string.IsNullOrEmpty(Center))
            {
                center = _centerService.GetItemByCode(Center);
                if (@center == null) return new JsonResult(new Dictionary<string, object>
                {
                    { "Error", "Cơ sở không đúng"}
                });
            }

            var returndata = FilterClass(model, center.ID, SubjectID, GradeID, "", skipActive, true);
            //model.TotalRecord = totalrec;

            var response = new Dictionary<string, object>
                {
                    { "Data", returndata},
                    { "Model", model }
                };
            return new JsonResult(response);
        }

        public JsonResult GetClassList(DefaultModel model, string Center, string SubjectID = "", string GradeID = "")
        {
            var center = new CenterEntity();


            if (!string.IsNullOrEmpty(Center))
            {
                center = _centerService.GetItemByCode(Center);
                if (@center == null) return new JsonResult(new Dictionary<string, object>
                {
                    { "Error", "Cơ sở không đúng"}
                });

            }

            var returndata = FilterClass(model, center.ID, SubjectID, GradeID, User.Claims.GetClaimByType("UserID").Value, true);
            //model.TotalRecord = totalrec;

            var response = new Dictionary<string, object>
                {
                    { "Data", returndata},
                    { "Model", model }
                };
            return new JsonResult(response);
        }

        private List<Dictionary<string, object>> FilterClass(DefaultModel model, string Center, string SubjectID = "", string GradeID = "", string TeacherID = "", bool skipActive = true, bool isManager = false)
        {
            model.TotalRecord = 0;
            var filter = new List<FilterDefinition<ClassSubjectEntity>>();
            var classfilter = new List<FilterDefinition<ClassEntity>>();

            var skip_owned = false;
            FilterDefinition<ClassEntity> ownerfilter = null;
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            var teacher = _teacherService.GetItemByID(UserID);

            if (!string.IsNullOrEmpty(SubjectID))
            {
                skip_owned = true;
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.SubjectID == SubjectID));
            }
            else
            {
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => teacher.Subjects.Contains(o.SubjectID)));
            }
            if (!string.IsNullOrEmpty(GradeID))
            {
                skip_owned = true;
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.GradeID == GradeID));
            }

            if (model.StartDate > new DateTime(1900, 1, 1))
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.EndDate >= model.StartDate));

            if (model.StartDate > new DateTime(1900, 1, 1))
                filter.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.StartDate <= model.EndDate));


            var data = new List<string>();
            if (filter.Count > 0)
            {
                var dCursor = _classSubjectService.Collection
                .Distinct(t => t.ClassID, Builders<ClassSubjectEntity>.Filter.And(filter));
                data = dCursor.ToList();
            }

            if (!string.IsNullOrEmpty(TeacherID))
            {
                if (!skip_owned)
                    classfilter.Add(Builders<ClassEntity>.Filter.Where(o => o.Members.Any(t => t.TeacherID == TeacherID && t.Type == ClassMemberType.TEACHER)));
                else
                    classfilter.Add(Builders<ClassEntity>.Filter.Where(o => o.Members.Any(t => t.TeacherID == TeacherID)));
            }

            if (!skipActive)
                classfilter.Add(Builders<ClassEntity>.Filter.Where(o => o.IsActive));

            if (data.Count > 0)
                classfilter.Add(Builders<ClassEntity>.Filter.Where(t => data.Contains(t.ID)));

            if (!string.IsNullOrEmpty(model.SearchText))
                classfilter.Add(Builders<ClassEntity>.Filter.Text(
                    //"\"" + 
                    model.SearchText
                    //+ "\""
                    ));

            if (classfilter.Count == 0)
                return null;

            var classResult = _service.Collection.Find(
                Builders<ClassEntity>.Filter.And(
                    Builders<ClassEntity>.Filter.Where(o => o.Center == Center && o.ClassMechanism != CLASS_MECHANISM.PERSONAL),
                    Builders<ClassEntity>.Filter.And(classfilter)
                ));

            model.TotalRecord = classResult.CountDocuments();

            var classData = classResult.SortByDescending(t => t.IsActive).ThenByDescending(t => t.StartDate).Skip(model.PageIndex * model.PageSize).Limit(model.PageSize).ToList().OrderBy(x => x.Name);
            var returndata = from o in classData
                             let skillIDs = _classSubjectService.GetByClassID(o.ID).Select(t => t.SkillID).Distinct()
                             let creator = _teacherService.GetItemByID(o.TeacherID) //Todo: Fix
                             let sname = skillIDs == null ? "" : string.Join(", ", _skillService.GetList().Where(t => skillIDs.Contains(t.ID)).Select(t => t.Name).ToList())
                             let teachers = (o.Members == null || o.Members.Count == 0) ? "" : string.Join(", ", o.Members.Where(t => t.Type != ClassMemberType.OWNER).Select(t => t.TeacherID).Distinct().Select(m => _teacherService.GetItemByID(m)?.FullName))
                             select new Dictionary<string, object>
                                {
                                 { "ID", o.ID },
                                 { "Name", o.Name },
                                 { "Students", 
                                     //_classStudentService.GetClassStudents(o.ID).Count 
                                     _studentService.CountByClass(o.ID)
                                 },
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
                                 { "Teachers", teachers },
                                 { "Creator", o.TeacherID },
                                 { "CreatorName", creator==null?"":creator.FullName },
                                 { "ClassMechanism", o.ClassMechanism }
                             };
            return returndata.ToList();
        }

        [HttpPost]
        [Obsolete]
        public JsonResult Create(ClassEntity item, string CenterCode, List<ClassSubjectEntity> classSubjects, IFormFile fileUpload)
        {
            var userId = User.Claims.GetClaimByType("UserID").Value;

            if (string.IsNullOrEmpty(userId))
            {
                return new JsonResult(new Dictionary<string, object>()
                        {
                            {"Error", "Vui lòng đăng nhập lại" }
                        });
            }



            var cm = _teacherService.GetItemByID(userId);
            if (cm == null)
            {
                return new JsonResult(new Dictionary<string, object>()
                        {
                            {"Error", "Vui lòng đăng nhập lại" }
                        });
            }
            var center = _centerService.GetItemByCode(CenterCode);
            if (center == null || cm.Centers.Count(t => t.Code == CenterCode) == 0)
            {
                return new JsonResult(new Dictionary<string, object>()
                        {
                            {"Error", "Cơ sở không đúng" }
                        });
            }

            var isHeadTeacher = _teacherHelper.HasRole(userId, center.ID, "head-teacher");

            var tc_sj = new List<TeacherSubjectsViewModel>();

            if (string.IsNullOrEmpty(item.ID) || item.ID == "0")
            {
                item.ID = null;
                item.Created = DateTime.UtcNow;


                if (classSubjects == null || classSubjects.Count == 0)
                {
                    return new JsonResult(new Dictionary<string, object>()
                        {
                            {"Error", "Cần chọn ít nhất một môn học" }
                        });
                }

                item.TeacherID = userId; // creator
                item.Skills = new List<string>();
                item.Subjects = new List<string>();
                item.Members = new List<ClassMemberEntity> { new ClassMemberEntity { TeacherID = userId, Type = ClassMemberType.OWNER, Name = cm.FullName } };
                item.TotalLessons = 0;
                item.TotalPractices = 0;
                item.TotalExams = 0;
                item.IsActive = true;
                item.StartDate = item.StartDate.ToUniversalTime();
                item.EndDate = item.EndDate.ToUniversalTime();
                item.Center = center.ID;

                if (fileUpload != null)
                {
                    var pathImage = _fileProcess.SaveMediaAsync(fileUpload, "", "CLASSIMG", center.Code).Result;
                    item.Image = pathImage;
                }

                _service.Save(item);

                var subjects = _classSubjectService.CreateQuery().Find(t => t.ClassID == item.ID && t.TypeClass == CLASSSUBJECT_TYPE.EXAM).ToList();
                if (subjects.Count() == 0)
                {
                    var newSbj = new ClassSubjectEntity
                    {
                        ClassID = item.ID,
                        CourseName = "Bài kiểm tra",
                        Description = "Bài kiểm tra",
                        StartDate = item.StartDate,
                        EndDate = item.EndDate,
                        TypeClass = CLASSSUBJECT_TYPE.EXAM,
                        TeacherID = item.TeacherID
                    };
                    _classSubjectService.Save(newSbj);
                    classSubjects.Add(newSbj);
                }


                //Create class subjects
                if (classSubjects != null && classSubjects.Count > 0)
                {
                    foreach (var csubject in classSubjects)
                    {
                        var teacher = _teacherService.GetItemByID(csubject.TeacherID);
                        var newMember = new ClassMemberEntity();
                        long lessoncount = 0;
                        long examcount = 0;
                        long practicecount = 0;
                        var nID = CreateNewClassSubject(csubject, item, out newMember, out lessoncount, out examcount, out practicecount);
                        if (!item.Skills.Contains(csubject.SkillID))
                            item.Skills.Add(csubject.SkillID);
                        if (!item.Subjects.Contains(csubject.SubjectID))
                            item.Subjects.Add(csubject.SubjectID);
                        if (!item.Members.Any(t => t.TeacherID == newMember.TeacherID && t.Type == ClassMemberType.TEACHER))
                            item.Members.Add(newMember);
                        item.TotalLessons += lessoncount;
                        item.TotalExams += examcount;
                        item.TotalPractices += practicecount;
                        var skill = _skillService.GetItemByID(csubject.SkillID);
                        if (skill == null) continue;
                        var course = _courseService.GetItemByID(csubject.CourseID);
                        var tc = tc_sj.SingleOrDefault(t => t.TeacherId == teacher.ID);
                        if (tc == null)
                            tc_sj.Add(new TeacherSubjectsViewModel
                            {
                                TeacherId = teacher.ID,
                                FullName = teacher.FullName,
                                Email = teacher.Email,
                                SubjectList = new List<SubjectModel> { new SubjectModel { SkillName = skill.Name, BookName = course != null ? course.Name : "" } }
                            });
                        else
                            tc.SubjectList.Add(new SubjectModel { SkillName = skill.Name, BookName = course != null ? course.Name : "" });
                    }

                    //Send email for each teacher
                    if (tc_sj.Count > 0)
                        foreach (var tc in tc_sj)
                            _ = Task.Run(() =>
                            {
                                _ = _mailHelper.SendTeacherJoinClassNotify(tc, item, center.Name);
                            });

                    _service.Save(item);
                }
                Dictionary<string, object> response = new Dictionary<string, object>()
                {
                    {"Data",item },
                    {"Error",null },
                    {"Msg","Success" }
                };
                return new JsonResult(response);
            }
            else
            {
                var processCS = new List<string>();
                var oldData = _service.GetItemByID(item.ID);
                if (oldData == null) return new JsonResult(new Dictionary<string, object>()
                {
                    {"Error", "Không tìm thấy lớp" }
                });

                oldData.Updated = DateTime.UtcNow;
                var mustUpdateName = false;
                if (oldData.Name != item.Name)
                {
                    oldData.Name = item.Name;
                    mustUpdateName = true;
                }
                oldData.Code = item.Code;
                oldData.StartDate = item.StartDate.ToUniversalTime();
                oldData.EndDate = item.EndDate.ToUniversalTime();
                oldData.Description = item.Description;
                //_service.Save(oldData);

                oldData.Skills = new List<string>();
                oldData.Subjects = new List<string>();
                var creator = _teacherService.GetItemByID(oldData.TeacherID);
                oldData.Members = new List<ClassMemberEntity> { };
                if (creator != null)
                    oldData.Members.Add(new ClassMemberEntity { TeacherID = creator.ID, Type = ClassMemberType.OWNER, Name = creator.FullName });
                oldData.TotalLessons = 0;
                oldData.TotalExams = 0;
                oldData.TotalPractices = 0;
                oldData.ClassMechanism = item.ClassMechanism;

                var oldSubjects = _classSubjectService.GetByClassID(item.ID);

                if (oldSubjects != null)
                {
                    foreach (var oSbj in oldSubjects)
                    {
                        if (oSbj.TypeClass != CLASSSUBJECT_TYPE.EXAM)
                        {
                            var nSbj = classSubjects.Find(t => t.ID == oSbj.ID);
                            if (!isHeadTeacher && oSbj.TeacherID != userId) //other teacher's class => skip
                                nSbj = oSbj;

                            var update = false;

                            if (nSbj == null || (nSbj.CourseID != oSbj.CourseID))
                            //delete oldSubject
                            {
                                _ = RemoveClassSubject(oSbj);
                                if (nSbj != null)
                                    nSbj.ID = null;//remove ID to create new
                            }

                            if (nSbj != null)
                            {
                                var newMember = new ClassMemberEntity();
                                long lessoncount = 0;
                                long examcount = 0;
                                long practicecount = 0;

                                if (nSbj.CourseID != oSbj.CourseID)//SkillID ~ CourseID
                                {
                                    update = true;
                                    nSbj.ID = CreateNewClassSubject(nSbj, oldData, out newMember, out lessoncount, out examcount, out practicecount);
                                    if (string.IsNullOrEmpty(nSbj.ID))//Error
                                        continue;
                                }
                                else //Not change
                                {
                                    //update period
                                    oSbj.StartDate = item.StartDate.ToUniversalTime();
                                    oSbj.EndDate = item.EndDate.ToUniversalTime();
                                    oSbj.TypeClass = nSbj.TypeClass;
                                    var teacher = _teacherService.GetItemByID(nSbj.TeacherID);
                                    if (teacher == null) continue;

                                    if (oSbj.TeacherID != nSbj.TeacherID) //change teacher
                                    {
                                        update = true;
                                        oSbj.TeacherID = nSbj.TeacherID;
                                        var skill = _skillService.GetItemByID(oSbj.SkillID);
                                        if (skill == null) continue;
                                        var course = _courseService.GetItemByID(nSbj.CourseID);
                                        var tc = tc_sj.SingleOrDefault(t => t.TeacherId == teacher.ID);
                                        if (tc == null)
                                            tc_sj.Add(new TeacherSubjectsViewModel
                                            {
                                                TeacherId = teacher.ID,
                                                FullName = teacher.FullName,
                                                Email = teacher.Email,
                                                SubjectList = new List<SubjectModel> { new SubjectModel { SkillName = skill.Name, BookName = course != null ? course.Name : "" } }
                                            });
                                        else
                                            tc.SubjectList.Add(new SubjectModel { SkillName = skill.Name, BookName = course != null ? course.Name : "" });
                                        _ = Task.Run(() =>
                                        {
                                            _ = _mailHelper.SendTeacherJoinClassNotify(teacher.FullName, teacher.Email, item.Name, skill.Name, item.StartDate, item.EndDate, center.Name);
                                        });
                                    }

                                    if (isHeadTeacher || update) // head-teacher | owned classsubject => update
                                        _classSubjectService.Save(oSbj);

                                    examcount = oSbj.TotalExams;
                                    lessoncount = oSbj.TotalLessons;
                                    practicecount = oSbj.TotalPractices;
                                    newMember = new ClassMemberEntity
                                    {
                                        TeacherID = teacher.ID,
                                        Name = teacher.FullName,
                                        Type = ClassMemberType.TEACHER
                                    };
                                }

                                processCS.Add(nSbj.ID);
                                if (!oldData.Skills.Contains(nSbj.SkillID))
                                    oldData.Skills.Add(nSbj.SkillID);
                                if (!oldData.Subjects.Contains(nSbj.SubjectID))
                                    oldData.Subjects.Add(nSbj.SubjectID);
                                if (!oldData.Members.Any(t => t.TeacherID == newMember.TeacherID && t.Type == ClassMemberType.TEACHER))
                                    oldData.Members.Add(newMember);
                                //add counter

                                oldData.TotalLessons += lessoncount;
                                oldData.TotalExams += examcount;
                                oldData.TotalPractices += practicecount;
                            }
                        }
                    }
                }

                if (classSubjects != null && classSubjects.Count > 0)
                {
                    foreach (var nSbj in classSubjects)
                    {
                        if (processCS.IndexOf(nSbj.ID) >= 0)
                            continue;
                        //create new subject
                        var newMember = new ClassMemberEntity();
                        long lessoncount = 0;
                        long examcount = 0;
                        long practicecount = 0;
                        var nID = CreateNewClassSubject(nSbj, oldData, out newMember, out lessoncount, out examcount, out practicecount);
                        if (string.IsNullOrEmpty(nSbj.ID))//Error
                            continue;

                        if (!oldData.Skills.Contains(nSbj.SkillID))
                            oldData.Skills.Add(nSbj.SkillID);
                        if (!oldData.Subjects.Contains(nSbj.SubjectID))
                            oldData.Subjects.Add(nSbj.SubjectID);
                        if (!oldData.Members.Any(t => t.TeacherID == newMember.TeacherID && t.Type == ClassMemberType.TEACHER))
                            oldData.Members.Add(newMember);
                        oldData.TotalLessons += lessoncount;
                        oldData.TotalExams += examcount;
                        oldData.TotalPractices += practicecount;
                    }
                }

                if (fileUpload != null)
                {
                    var pathImage = _fileProcess.SaveMediaAsync(fileUpload, "", "CLASSIMG", center.Code).Result;
                    oldData.Image = pathImage;
                }

                //update data
                _service.Save(oldData);

                if (tc_sj.Count > 0)
                    foreach (var tc in tc_sj)
                        _ = Task.Run(() =>
                        {
                            _ = _mailHelper.SendTeacherJoinClassNotify(tc, item, center.Name);
                        });

                if (mustUpdateName)
                {
                    var change = _groupService.UpdateGroupDisplayName(oldData.ID, oldData.Name);
                }

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
        }

        [HttpPost]
        [Obsolete]
        public JsonResult CloneClass(string ID, string CenterCode, string Name, bool updateCourse = false)
        {
            var oldData = _service.GetItemByID(ID);
            var center = _centerService.GetItemByCode(CenterCode);
            var userId = User.Claims.GetClaimByType("UserID").Value;
            var teacher = _teacherService.GetItemByID(userId);

            var newData = new MappingEntity<ClassEntity, ClassEntity>().Clone(oldData, new ClassEntity());
            newData.ID = null;
            newData.OriginID = oldData.ID;
            newData.Created = DateTime.UtcNow;
            newData.Center = center.ID;
            if (!Name.Equals(oldData.Name))
                newData.Name = Name;
            newData.Skills = new List<string>();
            newData.Subjects = new List<string>();
            newData.TeacherID = teacher.ID;

            _service.CreateQuery().InsertOne(newData);

            var classSubjects = _classSubjectService.GetByClassID(oldData.ID);
            if (classSubjects != null && classSubjects.Count > 0)
            {
                foreach (var csubject in classSubjects)
                {
                    long lessoncount = 0;
                    long examcount = 0;
                    long practicecount = 0;

                    var ncbj = new ClassSubjectEntity();
                    var newMember = new ClassMemberEntity();
                    if (updateCourse)//Create new subject from course
                    {
                        ncbj.CourseID = csubject.CourseID;
                        ncbj.GradeID = csubject.GradeID;
                        ncbj.SubjectID = csubject.SubjectID;
                        ncbj.TeacherID = teacher.ID;
                        //Counter coud be changed in case base course change
                        var nID = CreateNewClassSubject(ncbj, newData, out newMember, out lessoncount, out examcount, out practicecount, false);
                    }
                    else //Clone subject from root class's subjects
                    {
                        ncbj = new MappingEntity<ClassSubjectEntity, ClassSubjectEntity>().Clone(csubject, ncbj);
                        ncbj.CourseID = csubject.CourseID;
                        ncbj.GradeID = csubject.GradeID;
                        ncbj.SubjectID = csubject.SubjectID;
                        ncbj.TeacherID = teacher.ID;
                        ncbj.ClassID = newData.ID;
                        lessoncount = csubject.TotalLessons;
                        examcount = csubject.TotalExams;
                        practicecount = csubject.TotalPractices;
                        _ = _classHelper.CloneClassSubject(ncbj, teacher.ID, csubject.ID);
                    }

                    if (!newData.Skills.Contains(csubject.SkillID))
                        newData.Skills.Add(csubject.SkillID);
                    if (!newData.Subjects.Contains(csubject.SubjectID))
                        newData.Subjects.Add(csubject.SubjectID);
                    if (!newData.Members.Any(t => t.TeacherID == newMember.TeacherID && t.Type == ClassMemberType.TEACHER))
                        newData.Members.Add(newMember);

                    newData.TotalLessons += lessoncount;
                    newData.TotalExams += examcount;
                    newData.TotalPractices += practicecount;
                }
                _service.Save(newData);
            }

            Dictionary<string, object> response = new Dictionary<string, object>()
                {
                    {"Data",newData },
                    {"Error",null },
                    {"Msg","Success" }
                };
            return new JsonResult(response);
        }

        private async Task RemoveClassSubject(ClassSubjectEntity cs)
        {
            ////remove old schedule
            var CsTask = _lessonScheduleService.RemoveClassSubject(cs.ID);
            //remove chapter
            var CtTask = _chapterService.RemoveClassSubjectChapter(cs.ID);
            //remove clone lesson
            var LsTask = _lessonHelper.RemoveClassSubjectLesson(cs.ID);
            //remove progress: learning history => class progress, chapter progress, lesson progress
            var LhTask = _progressHelper.RemoveClassSubjectHistory(cs.ID);
            //remove exam
            var ExTask = _examService.RemoveClassSubjectExam(cs.ID);
            //remove classSubject
            //await Task.WhenAll(CsTask, CtTask, LsTask, LhTask, ExTask, ExDetailTask);
            await _classSubjectService.RemoveAsync(cs.ID);
        }

        private string CreateNewClassSubject(ClassSubjectEntity nSbj, ClassEntity @class, out ClassMemberEntity member, out long lessoncount, out long examcount, out long practicecount, bool notify = true)
        {
            member = new ClassMemberEntity();
            lessoncount = 0;
            examcount = 0;
            practicecount = 0;
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
                examcount = course.TotalExams;
                practicecount = course.TotalPractices;

                var teacher = _teacherService.GetItemByID(nSbj.TeacherID);
                if (teacher == null || !teacher.IsActive || !teacher.Subjects.Contains(nSbj.SubjectID))
                {
                    throw new Exception("Teacher " + nSbj.TeacherID + " is not avaiable");
                }

                nSbj.CourseName = course.Name;
                nSbj.ClassID = @class.ID;
                nSbj.StartDate = @class.StartDate;
                nSbj.EndDate = @class.EndDate;
                nSbj.SkillID = course.SkillID;
                nSbj.Description = course.Description;
                nSbj.LearningOutcomes = course.LearningOutcomes;
                nSbj.TotalLessons = course.TotalLessons;
                nSbj.TotalPractices = course.TotalPractices;
                nSbj.TotalExams = course.TotalExams;
                nSbj.Image = course.Image;

                var skill = _skillService.GetItemByID(nSbj.SkillID);

                var center = _centerService.GetItemByID(@class.Center);

                _classSubjectService.Save(nSbj);

                member = new ClassMemberEntity
                {
                    Name = teacher.FullName,
                    TeacherID = teacher.ID,
                    Type = ClassMemberType.TEACHER
                };
                //Clone Course
                _courseHelper.CloneForClassSubject(nSbj);

                if (notify)
                    _ = Task.Run(() =>
                    {
                        _ = _mailHelper.SendTeacherJoinClassNotify(teacher.FullName, teacher.Email, @class.Name, nSbj.CourseName, @class.StartDate, @class.EndDate, center.Name);
                    });
                return nSbj.ID;
            }
            catch (Exception e)
            {
                return "";
            }
        }

        [HttpPost]
        [Obsolete]
        public JsonResult Remove(DefaultModel model)
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
                    //Remove Class Student
                    _ = _studentService.LeaveClassAll(ids.ToList());
                    //remove ClassSubject
                    _ = _classSubjectService.RemoveManyClass(ids);
                    //remove Lesson, Part, Question, Answer
                    _ = _lessonHelper.RemoveManyClassLessons(ids);
                    //remove Schedule
                    _ = _lessonScheduleService.RemoveManyClass(ids);
                    //remove History
                    _ = _progressHelper.RemoveClassHistory(ids);
                    //remove Exam
                    _ = _examService.RemoveManyClassExam(ids);
                    //.Collection.DeleteMany(o => ids.Contains(o.ClassID));
                    //remove Exam Detail
                    _examDetailService.Collection.DeleteMany(o => ids.Contains(o.ClassID));
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
        #endregion

        #region Add To My Course
        public async Task<JsonResult> AddToMyCourse(string CourseID, string CenterCode, string CourseName = "", string ClassID = null, ClassEntity item = null, Boolean isCreateNewClass = false)
        {
            var userId = User.Claims.GetClaimByType("UserID").Value;

            if (string.IsNullOrEmpty(userId))
            {
                return new JsonResult(new Dictionary<string, object>()
                        {
                            {"Error", "Vui lòng đăng nhập lại" }
                        });
            }
            var teacher = _teacherService.GetItemByID(userId);
            if (teacher == null)
            {
                return new JsonResult(new Dictionary<string, object>()
                        {
                            {"Error", "Vui lòng đăng nhập lại" }
                        });
            }
            var center = _centerService.GetItemByCode(CenterCode);
            if (center == null || teacher.Centers.Count(t => t.Code == CenterCode) == 0)
            {
                return new JsonResult(new Dictionary<string, object>()
                        {
                            {"Error", "Cơ sở không đúng" }
                        });
            }

            if (ClassID != null)
            {
                var Class = _service.GetItemByID(ClassID);
                if (Class == null)
                {
                    return new JsonResult(new Dictionary<string, object>()
                        {
                            {"Error", "Lớp không tồn tại" }
                        });
                }

                var Course = _courseService.GetItemByID(CourseID);//Bài giảng

                Class.Updated = DateTime.UtcNow;
                var oldSubjects = _classSubjectService.GetByClassID(Class.ID);
                var classSubject = new ClassSubjectEntity();
                classSubject.CourseID = Course.ID;
                classSubject.CourseName = CourseName;
                classSubject.SkillID = Course.SkillID;
                classSubject.GradeID = Course.GradeID;
                classSubject.SubjectID = Course.SubjectID;
                classSubject.TeacherID = teacher.ID;
                classSubject.TypeClass = CLASSSUBJECT_TYPE.EXTEND;

                oldSubjects.Add(classSubject);

                Create(Class, center.Code, oldSubjects, null);
            }
            else
            {
                var Course = _courseService.GetItemByID(CourseID);//Bài giảng
                //Course.OriginID = Course.ID;
                Course.Center = center.ID;
                //Course.Created = DateTime.UtcNow;
                //Course.CreateUser = teacher.ID;
                //Course.IsAdmin = true;
                //Course.IsPublic = false;
                //Course.IsActive = true;
                //Course.Updated = DateTime.UtcNow;
                //Course.TeacherID = teacher.ID;
                //Course.TotalPractices = 0;
                //Course.TotalLessons = 0;
                //Course.TotalExams = 0;
                Course.Name = CourseName == "" ? Course.Name : CourseName;

                Course.ID = null;

                var newID = await CopyCourse(_courseService.GetItemByID(CourseID), Course, userId);

                if (isCreateNewClass)
                {
                    var listclassSubject = new List<ClassSubjectEntity>();
                    var classSubject = new ClassSubjectEntity();
                    classSubject.CourseID = newID;
                    classSubject.SkillID = Course.SkillID;
                    classSubject.GradeID = Course.GradeID;
                    classSubject.SubjectID = Course.SubjectID;
                    classSubject.TeacherID = teacher.ID;
                    classSubject.TypeClass = CLASSSUBJECT_TYPE.EXTEND;

                    listclassSubject.Add(classSubject);

                    Create(item, center.Code, listclassSubject, null);
                }
            }
            return new JsonResult("Thêm thành công");
        }

        public async Task<string> CopyCourse(CourseEntity org_course, CourseEntity target_course, string _userCreate = "")
        {
            try
            {
                var new_course = _cloneCourseMapping.Clone(org_course, new CourseEntity());

                new_course.OriginID = org_course.ID;
                new_course.Name = target_course.Name;
                //new_course.Code = target_course.Code;
                //new_course.Description = target_course.Description;
                //new_course.GradeID = target_course.GradeID;
                //new_course.SubjectID = target_course.SubjectID;
                new_course.TeacherID = _userCreate;
                new_course.CreateUser = _userCreate;
                new_course.Center = target_course.Center ?? org_course.Center;
                //new_course.SkillID = target_course.SkillID;
                new_course.Created = DateTime.UtcNow;
                new_course.Updated = DateTime.UtcNow;
                new_course.IsActive = true;
                new_course.IsUsed = false;
                new_course.IsPublic = false;
                new_course.TargetCenters = new List<string>();

                _courseService.Collection.InsertOne(new_course);

                //var a = _courseChapterService.CreateQuery().Find(o => o.CourseID == org_course.ID).SortBy(o => o.ParentID).ThenBy(o => o.Order).ThenBy(o => o.ID).ToList();
                //foreach (var item in a)
                //{
                await CloneCourseChapter(new CourseChapterEntity
                {
                    OriginID = "0",
                    CourseID = new_course.ID,
                }, _userCreate, org_course.ID);
                //}

                return new_course.ID;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<CourseChapterEntity> CloneCourseChapter(CourseChapterEntity item, string _userCreate, string orgCourseID)
        {
            try
            {
                if (item.OriginID != "0")
                    _courseChapterService.Collection.InsertOne(item);
                else
                {
                    item.ID = "0";
                }

                var lessons = _courseLessonService.GetChapterLesson(orgCourseID, item.OriginID);

                if (lessons != null && lessons.Count() > 0)
                {
                    foreach (var o in lessons)
                    {
                        if (o.ChapterID == item.OriginID)
                        {
                            await _lessonHelper.CopyCourseLessonFromCourseLesson(o, new CourseLessonEntity
                            {
                                CourseID = item.CourseID,
                                ChapterID = item.ID,
                                CreateUser = _userCreate
                            });
                        }
                    }
                }

                var subChapters = _courseChapterService.GetSubChapters(orgCourseID, item.OriginID);
                foreach (var o in subChapters)
                {
                    var new_chapter = _cloneCourseChapterMapping.Clone(o, new CourseChapterEntity());
                    new_chapter.CourseID = item.CourseID;
                    new_chapter.ParentID = item.ID;
                    new_chapter.CreateUser = _userCreate;
                    new_chapter.Created = DateTime.UtcNow;
                    new_chapter.OriginID = o.ID;
                    await CloneCourseChapter(new_chapter, _userCreate, orgCourseID);
                }
                return item;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region Student Detail
        public JsonResult GetLearningProgress(DefaultModel model, string ClassID, string ClassSubjectID)
        {
            var StudentID = model.ID;
            if (!_studentService.IsStudentInClass(ClassID, StudentID))
                return null;

            var currentClass = _service.GetItemByID(ClassID);
            if (currentClass == null)
                return null;

            List<LessonProgressEntity> data;
            if (string.IsNullOrEmpty(ClassSubjectID))
                data = _lessonProgressService.GetByClassID_StudentID(ClassID, StudentID);
            else
                data = _lessonProgressService.GetByClassSubjectID_StudentID(ClassSubjectID, StudentID);

            var subjects = _classSubjectService.GetByClassID(ClassID);

            var lessons = (from progress in data
                           let schedule = _lessonScheduleService.CreateQuery().Find(o => o.LessonID == progress.LessonID && o.ClassID == currentClass.ID).FirstOrDefault()
                           where schedule != null
                           let classsubject = subjects.Single(t => t.ID == schedule.ClassSubjectID)
                           where classsubject != null
                           let lesson = _lessonService.GetItemByID(progress.LessonID)
                           select _moduleViewMapping.AutoOrtherType(lesson, new StudentModuleViewModel()
                           {
                               ScheduleID = schedule.ID,
                               ScheduleStart = schedule.StartDate,
                               //Skill = String.IsNullOrEmpty(classsubject.SkillID) ?"":_skillService.GetItemByID(classsubject.SkillID).Name,
                               ScheduleEnd = schedule.EndDate,
                               IsActive = schedule.IsActive,
                               LearnCount = progress.TotalLearnt,
                               TemplateType = lesson.TemplateType,
                               LearnStart = progress.FirstDate,
                               LearnLast = progress.LastDate
                           })).OrderBy(r => r.LearnStart).ThenBy(r => r.ChapterID).ThenBy(r => r.ID).ToList();

            var response = new Dictionary<string, object>
            {
                { "Data", lessons},
                { "Model", model }
            };
            return new JsonResult(response);
        }

        public JsonResult GetLearningResult(DefaultModel model, string ClassID, string ClassSubjectID, bool isPractice = false)
        {
            var StudentID = model.ID;
            if (!_studentService.IsStudentInClass(ClassID, StudentID))
                return null;

            var currentClass = _service.GetItemByID(ClassID);
            if (currentClass == null)
                return null;


            var currentCs = _classSubjectService.GetItemByID(ClassSubjectID);
            if (currentCs == null)
                return null;

            var data = new List<LessonProgressEntity>();
            var passExams = new List<LessonEntity>();
            if (!string.IsNullOrEmpty(ClassSubjectID))
            //{
            //passExams = isPractice ?  _lessonService.GetClassSubjectExams(ClassID) : _lessonService.GetClassSubjectPractices(ClassSubjectID);
            //passExams = _lessonScheduleService.GetClassExam(ClassID, model.StartDate, model.EndDate);
            //data = _lessonProgressService.GetByClassID_StudentID(ClassID, StudentID);
            //}
            //else
            {
                passExams = (!isPractice ? _lessonService.GetClassSubjectExams(ClassSubjectID) : _lessonService.GetClassSubjectPractices(ClassSubjectID)).ToList();
                //passExams = _lessonScheduleService.GetClassSubjectExam(ClassSubjectID, model.StartDate, model.EndDate);
                data = _lessonProgressService.GetByClassSubjectID_StudentID(ClassSubjectID, StudentID);
            }

            var subjects = _classSubjectService.GetByClassID(ClassID);

            var lessons = (
                            from lesson in passExams
                            let progress = data.FirstOrDefault(t => t.StudentID == model.ID && t.LessonID == lesson.ID) ?? new LessonProgressEntity()
                            //from progress in data
                            //where progress.Tried > 0
                            //let schedule = _lessonScheduleService.CreateQuery().Find(o => o.LessonID == progress.LessonID && o.ClassID == currentClass.ID).FirstOrDefault()
                            //where schedule != null
                            //let classsubject = subjects.Single(t => t.ID == schedule.ClassSubjectID)
                            //where classsubject != null
                            let schedule = _lessonScheduleService.GetItemByLessonID(lesson.ID)
                            //let lesson = _lessonService.GetItemByID(schedule.LessonID)
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

        public JsonResult GetLearningSummary(DefaultModel model, string ClassID)
        {
            var StudentID = model.ID;
            if (!_studentService.IsStudentInClass(ClassID, StudentID))
                return null;

            var currentClass = _service.GetItemByID(ClassID);
            if (currentClass == null)
                return null;

            var total_students = _studentService.CountByClass(currentClass.ID);
            var rank = -1;
            var results = _progressHelper.GetClassResults(ClassID).OrderByDescending(t => t.RankPoint).ToList();
            var avgpoint = 0.0;
            var studentresult = _classProgressService.GetItemByClassID(ClassID, StudentID);

            if (studentresult != null)
            {
                var examCount = _lessonScheduleService.CountClassExam(ClassID, null);
                if (examCount > 0)
                    avgpoint = studentresult.TotalPoint / examCount;
            }
            if (studentresult != null && results != null && (results.FindIndex(t => t.StudentID == StudentID) >= 0))
                rank = results.FindIndex(t => t.TotalPoint == studentresult.TotalPoint) + 1;
            var response = new Dictionary<string, object>
            {
                { "Result", new { pos = rank, total = total_students, avg = avgpoint } },
                { "Data", GetClassSubjectSummary(currentClass, StudentID, total_students)},
                { "Model", model }
            };

            return new JsonResult(response);
        }

        //public JsonResult GetStudentSummary(DefaultModel model)
        //{
        //    var StudentID = model.ID;
        //    var student = _studentService.GetItemByID(StudentID);
        //    var MyClass = _classService.GetClassByMechanism(CLASS_MECHANISM.PERSONAL, StudentID);
        //    if (student == null)
        //        return null;

        //    var data = new List<StudentSummaryViewModel>();
        //    if (student.JoinedClasses != null && student.JoinedClasses.Count > 0)
        //    {
        //        if (MyClass != null) { student.JoinedClasses.RemoveAt(student.JoinedClasses.IndexOf(MyClass.ID)); }

        //        foreach (var ClassID in student.JoinedClasses)
        //        {
        //            var @class = _service.GetItemByID(ClassID);
        //            if (@class == null) continue;
        //            var total_students = _studentService.CountByClass(@class.ID);
        //            var summary = new MappingEntity<ClassProgressEntity, StudentSummaryViewModel>()
        //                .AutoOrtherType(_classProgressService.GetItemByClassID(ClassID, StudentID) ?? new ClassProgressEntity
        //                {
        //                    ClassID = ClassID,
        //                    StudentID = StudentID,
        //                }, new StudentSummaryViewModel()
        //                {
        //                    ClassName = @class.Name,
        //                    Rank = -1,
        //                    TotalStudents = (int)total_students,
        //                    TotalLessons = @class.TotalLessons,
        //                    TotalExams = @class.TotalExams,
        //                    TotalPractices = @class.TotalPractices
        //                });

        //            var results = _progressHelper.GetClassResults(ClassID).OrderByDescending(t => t.RankPoint).ToList();

        //            summary.RankPoint = _progressHelper.CalculateRankPoint(summary.TotalPoint, summary.PracticePoint, summary.Completed);
        //            summary.AvgPoint = @class.TotalExams > 0 ? summary.TotalPoint / @class.TotalExams : 0;

        //            if (results != null && (results.FindIndex(t => t.StudentID == summary.StudentID) >= 0))
        //                summary.Rank = results.FindIndex(t => t.RankPoint == summary.RankPoint) + 1;

        //            data.Add(summary);
        //            data.AddRange(GetClassSubjectSummary(@class, StudentID, total_students));
        //        }
        //    }
        //    var response = new Dictionary<string, object>
        //    {
        //        { "Data", data},
        //        { "Model", model }
        //    };
        //    return new JsonResult(response);
        //}

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

        #endregion

        #region Fix Data

        #endregion

        #region Edit
        public IActionResult Editor(string basis, string ID)
        {
            if (string.IsNullOrEmpty("ID"))
                return Redirect($"/{basis}{Url.Action("Index")}");

            var currentClassSbj = _classSubjectService.GetItemByID(ID);
            if (currentClassSbj == null)
                return Redirect($"/{basis}{Url.Action("Index")}");

            var currentClass = _classService.GetItemByID(currentClassSbj.ClassID);

            //var isUsed = isCourseUsed(data.ID);
            //Cap nhat IsUsed
            //if (data.IsUsed != isUsed)
            //{
            //    data.IsUsed = isUsed;
            //    _service.Save(data);
            //}

            ViewBag.ClassSbj = currentClassSbj;
            ViewBag.Class = currentClass;

            var UserID = User.Claims.GetClaimByType("UserID").Value;

            var chapters = _chapterService.GetByClassSubject(ID).ToList();
            ViewBag.Chapter = chapters;
            ViewBag.User = UserID;

            return View("Editor");
        }

        public async Task<JsonResult> CreateOrUpdateLesson(LessonEntity item)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                LessonEntity data = null;
                if (!string.IsNullOrEmpty(item.ID))
                    data = _lessonService.GetItemByID(item.ID);
                if (data == null)
                {
                    item.Created = DateTime.UtcNow;
                    item.CreateUser = UserID;
                    item.IsAdmin = true;
                    item.IsActive = false;
                    item.IsParentCourse = item.ChapterID.Equals("0");
                    item.Updated = DateTime.UtcNow;
                    item.Order = 0;

                    _lessonHelper.InitLesson(item);//insert + create schedule

                    ChangeLessonPosition(item, Int32.MaxValue);//move lesson to bottom of parent

                    //update total lesson to parent chapter
                    await _classHelper.IncreaseLessonCounter(item, 1, item.TemplateType == LESSON_TEMPLATE.EXAM ? 1 : 0, 0);
                }
                else
                {
                    if (!_lessonHelper.isExamined(data))
                    {
                        return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", null },
                                { "Error", "Bài học đã bị khóa, không điều chỉnh được" }
                            });
                    }

                    item.Updated = DateTime.UtcNow;
                    var newOrder = item.Order - 1;
                    item.Order = data.Order;
                    item.ClassID = data.ClassID;
                    item.ClassSubjectID = data.ClassSubjectID;

                    //update counter if type change
                    if (item.TemplateType != data.TemplateType)
                    {
                        var examInc = 0;
                        var pracInc = 0;
                        if (_lessonHelper.IsQuizLesson(item.ID)) pracInc = 1;
                        if (item.TemplateType == LESSON_TEMPLATE.LECTURE) // EXAM => LECTURE
                        {
                            examInc = -1;
                            item.IsPractice = pracInc == 1;
                        }
                        else
                        {
                            examInc = 1;
                            item.IsPractice = false;
                            pracInc = pracInc == 1 ? -1 : 0;
                        }
                        await _classHelper.IncreaseLessonCounter(item, 0, examInc, pracInc);
                        //if (!string.IsNullOrEmpty(item.ChapterID) && item.ChapterID != "0")
                        //    _ = _classHelper.IncreaseChapterCounter(item.ChapterID, 0, examInc, pracInc);
                        //else
                        //    _ = _classHelper.IncreaseClassSubjectCounter(item.ClassSubjectID, 0, examInc, pracInc);
                    }

                    _lessonService.CreateQuery().ReplaceOne(o => o.ID == item.ID, item);

                    if (item.Order != newOrder)//change Position
                    {
                        ChangeLessonPosition(item, newOrder);
                    }
                }

                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", item },
                    {"Error",null }
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", null },
                    {"Error",ex.Message }
                });
            }
        }

        [HttpPost]
        public async Task<JsonResult> CopyLesson(string ArrID, string Title, string ChapterID)
        {
            var orgLesson = _lessonService.GetItemByID(ArrID);
            var UserID = User.Claims.GetClaimByType("UserID").Value;

            var new_lesson = new LessonEntity
            {
                ChapterID = ChapterID,
                CreateUser = UserID,
                Title = string.IsNullOrEmpty(Title) ? (orgLesson.Title + (orgLesson.ChapterID == ChapterID ? " (copy)" : "")) : Title,
                Order = (int)_lessonService.CountChapterLesson(ChapterID) + 1
            };
            await _lessonHelper.CopyLessonFromLesson(orgLesson, new_lesson);
            await _classHelper.IncreaseLessonCounter(new_lesson, 1, new_lesson.TemplateType == LESSON_TEMPLATE.EXAM ? 1 : 0, new_lesson.IsPractice ? 1 : 0);

            return new JsonResult("OK");
        }

        [HttpPost]
        public async Task<JsonResult> RemoveLesson(DefaultModel model, string ID)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.ArrID))
                    ID = model.ArrID;
                //check if lesson is lock: taken exam || practiced
                var lesson = _lessonService.GetItemByID(ID);//TODO: check permission
                if (lesson != null)
                {
                    if (!_lessonHelper.isExamined(lesson))
                    {
                        return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", null },
                                { "Error", "Bài học đã bị khóa, không điều chỉnh được" }
                            });
                    }

                    await _classHelper.IncreaseLessonCounter(lesson, -1, lesson.TemplateType == LESSON_TEMPLATE.EXAM ? -1 : 0, lesson.IsPractice ? -1 : 0);
                    ChangeLessonPosition(lesson, int.MaxValue);//chuyển lesson xuống cuối của đối tượng chứa
                    await _lessonHelper.RemoveSingleLesson(lesson.ID);

                    return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", ID },
                                { "Error", null }
                            });
                }
                else
                {
                    return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", null },
                                { "Error", "Bài học không tồn tại" }
                            });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", null },
                    {"Error", ex.Message}
                });
            }
        }

        private int ChangeLessonPosition(LessonEntity item, int pos)
        {
            var parts = _lessonService.GetChapterLesson(item.ClassSubjectID, item.ChapterID);
            var ids = parts.Select(o => o.ID).ToList();

            var oldPos = ids.IndexOf(item.ID);
            if (oldPos == pos && oldPos == item.Order)
            {
                return oldPos;
            }
            if (pos > parts.Count())
                pos = parts.Count() - 1;
            item.Order = pos;

            _lessonService.CreateQuery().ReplaceOne(o => o.ID == item.ID, item);
            int entry = -1;
            foreach (var part in parts)
            {
                if (part.ID == item.ID) continue;
                if (entry == pos - 1)
                    entry++;
                entry++;
                part.Order = entry;
                _lessonService.CreateQuery().ReplaceOne(o => o.ID == part.ID, part);
            }
            return pos;
        }

        [HttpPost]
        public async Task<JsonResult> JoinLesson(string ID, string JoinLesson)
        {
            try
            {
                var rootItem = _lessonService.GetItemByID(ID);
                var joinItem = _lessonService.GetItemByID(JoinLesson);


                if (rootItem == null || joinItem == null)
                {
                    return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", null },
                        { "Error", "Dữ liệu không đúng" }
                    });
                }

                if (!_lessonHelper.isExamined(rootItem))
                {
                    return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", null },
                                { "Error", "Bài học " + rootItem.Title + " đã bị khóa, không điều chỉnh được" }
                            });
                }

                if (!_lessonHelper.isExamined(joinItem))
                {
                    return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", null },
                                { "Error", "Bài học " + joinItem.Title + " đã bị khóa, không điều chỉnh được" }
                            });
                }

                var currentIndex = _cloneLessonPartService.CreateQuery().CountDocuments(o => o.ParentID == rootItem.ID);
                var joinParts = _cloneLessonPartService.CreateQuery().Find(o => o.ParentID == joinItem.ID).SortBy(o => o.Order).ToList();

                var hasPractice = false;
                if (joinParts != null && joinParts.Count > 0)
                {
                    foreach (var part in joinParts)
                    {
                        if (quizType.Contains(part.Type))
                            hasPractice = true;
                        part.ParentID = rootItem.ID;
                        part.Order = (int)currentIndex++;
                        _cloneLessonPartService.CreateQuery().ReplaceOne(o => o.ID == part.ID, part);
                    }
                }

                ChangeLessonPosition(joinItem, int.MaxValue);//chuyển lesson xuống cuối của đối tượng chứa

                //change counter
                if (rootItem.TemplateType == LESSON_TEMPLATE.LECTURE)
                    if (!rootItem.IsPractice && hasPractice)
                    {
                        rootItem.IsPractice = true;
                        _lessonService.Save(rootItem);
                        _ = _classHelper.IncreaseLessonCounter(rootItem, 0, 0, 1);

                        //if (rootItem.ChapterID == "0")
                        //    _classHelper.IncreaseClassSubjectCounter(rootItem.ClassSubjectID, 0, 0, 1);
                        //else
                        //    _classHelper.IncreaseChapterCounter(rootItem.ChapterID, 0, 0, 1);
                    }

                //decrease counter
                _ = _classHelper.IncreaseLessonCounter(joinItem, -1, joinItem.TemplateType == LESSON_TEMPLATE.EXAM ? -1 : 0, joinItem.IsPractice ? -1 : 0);

                //if (joinItem.ChapterID == "0")
                //    _classHelper.IncreaseClassSubjectCounter(joinItem.ClassSubjectID, -1, 0 - joinItem.TemplateType == LESSON_TEMPLATE.EXAM ? 1 : 0, 0 - (joinItem.IsPractice ? 1 : 0));
                //else
                //    _classHelper.IncreaseChapterCounter(joinItem.ChapterID, -1, 0 - joinItem.TemplateType == LESSON_TEMPLATE.EXAM ? 1 : 0, 0 - (joinItem.IsPractice ? 1 : 0));

                //remove all lesson content from db
                _ = _lessonHelper.RemoveSingleLesson(joinItem.ID);

                return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", joinItem },
                        { "Del", JoinLesson },
                        { "Error", null }
                    });
            }
            catch (Exception ex)
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", null },
                    { "Error", ex.Message }
                });
            }
        }

        public JsonResult CreateOrUpdateChapter(ChapterEntity item)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;

                if (item.ClassSubjectID == null || _classSubjectService.GetItemByID(item.ClassSubjectID) == null)
                {
                    return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", null },
                        {"Error", "Không tìm thấy học liệu" }
                    });
                }

                var data = _chapterService.GetItemByID(item.ID);
                if (data == null)
                {
                    item.Created = DateTime.UtcNow;
                    item.IsAdmin = true;
                    item.IsActive = false;
                    item.Updated = DateTime.UtcNow;
                    item.Order = int.MaxValue - 1;
                    _chapterService.Save(item);
                    ChangeChapterPosition(item, int.MaxValue);//move chapter to bottom of new parent chap
                }
                else
                {
                    item.Updated = DateTime.UtcNow;
                    var newOrder = item.Order - 1;
                    var oldParent = data.ParentID;

                    data.Name = item.Name;
                    data.ParentID = item.ParentID;
                    data.Description = item.Description;

                    _chapterService.Save(data);
                    if (oldParent != item.ParentID)//Change Root chapter
                    {
                        if (item.TotalLessons > 0)
                        {
                            //decrease old parent counter
                            _ = _classHelper.IncreaseChapterCounter(oldParent, 0 - data.TotalLessons, 0 - data.TotalExams, 0 - data.TotalPractices);
                            _ = _classHelper.IncreaseChapterCounter(item.ParentID, data.TotalLessons, data.TotalExams, data.TotalPractices);
                        }
                        //move chapter to bottom of new parent chap
                        ChangeChapterPosition(data, int.MaxValue);
                    }
                    else
                        ChangeChapterPosition(data, newOrder);
                }

                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", item },
                    { "Error", null }
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", null },
                    {"Error",ex.Message }
                });
            }
        }

        private int ChangeChapterPosition(ChapterEntity item, int pos)
        {
            var parts = new List<ChapterEntity>();
            parts = _chapterService.CreateQuery().Find(o => o.ClassSubjectID == item.ClassSubjectID && o.ParentID == item.ParentID)
                .SortBy(o => o.Order).ThenBy(o => o.ID).ToList();

            var ids = parts.Select(o => o.ID).ToList();

            var oldPos = ids.IndexOf(item.ID);
            if (oldPos == pos && (item.Order == pos))
                return oldPos;

            if (pos > parts.Count())
                pos = parts.Count() - 1;
            item.Order = pos;

            _chapterService.CreateQuery().ReplaceOne(o => o.ID == item.ID, item);
            int entry = -1;
            foreach (var part in parts)
            {
                if (part.ID == item.ID) continue;
                if (entry == pos - 1)
                    entry++;
                entry++;
                part.Order = entry;
                _chapterService.CreateQuery().ReplaceOne(o => o.ID == part.ID, part);
            }
            return pos;
        }

        [HttpPost]
        public async Task<JsonResult> RemoveChapter(DefaultModel model)
        {
            try
            {
                var ID = model.ArrID;
                var chapter = _chapterService.GetItemByID(ID);
                if (chapter == null)
                {
                    return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", null },
                                {"Error", null }
                            });
                }
                //Decrease counter
                if (chapter.TotalLessons > 0)
                    if (chapter.ParentID == "0")
                        await _classHelper.IncreaseClassSubjectCounter(chapter.CourseID, 0 - chapter.TotalLessons, 0 - chapter.TotalExams, 0 - chapter.TotalPractices);
                    else
                        await _classHelper.IncreaseChapterCounter(chapter.ParentID, 0 - chapter.TotalLessons, 0 - chapter.TotalExams, 0 - chapter.TotalPractices);

                //Remove chapter
                await RemoveChapter(chapter);
                return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", ID },
                                {"Error", null }
                            });
            }
            catch (Exception ex)
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", null },
                    {"Error", ex.Message}
                });
            }
        }

        private async Task RemoveChapter(ChapterEntity chap)
        {
            //_lessonService.CreateQuery().DeleteMany(o => o.ChapterID == chap.ID);
            var lessons = _lessonService.GetChapterLesson(chap.ClassSubjectID, chap.ID);
            if (lessons != null && lessons.Count() > 0)
                foreach (var lesson in lessons)
                    _ = _lessonHelper.RemoveSingleLesson(lesson.ID);

            var subchapters = _chapterService.CreateQuery().Find(o => o.ParentID == chap.ID).ToList();
            if (subchapters != null && subchapters.Count > 0)
                foreach (var chapter in subchapters)
                    await RemoveChapter(chapter);
            //move chapter to bottom then delele
            ChangeChapterPosition(chap, int.MaxValue);
            await _chapterService.RemoveAsync(chap.ID);
        }

        [HttpPost]
        public async Task<JsonResult> CopyChapter(string ChapID)
        {
            var orgChapter = _chapterService.GetItemByID(ChapID);
            if (orgChapter == null)
            {
                return new JsonResult(new Dictionary<string, object>
                    {
                        {"Error", "Không tìm thấy thông tin" }
                    });
            }
            else
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var clone_chap = new MappingEntity<ChapterEntity, ChapterEntity>().Clone(orgChapter, new ChapterEntity());
                clone_chap.OriginID = orgChapter.ID;
                clone_chap.Order = (int)_chapterService.GetSubChapters(orgChapter.ClassSubjectID, orgChapter.ParentID).Count();
                clone_chap.CreateUser = UserID;
                clone_chap.Name += " (copy)";
                var chapter = await _classHelper.CloneChapter(clone_chap, orgChapter.CreateUser, orgChapter.ClassSubjectID);
                if (chapter.TotalLessons > 0)
                {
                    if (chapter.ParentID == "0")
                        await _classHelper.IncreaseClassSubjectCounter(chapter.ClassSubjectID, chapter.TotalLessons, chapter.TotalExams, chapter.TotalPractices);
                    else
                        await _classHelper.IncreaseChapterCounter(chapter.ParentID, chapter.TotalLessons, chapter.TotalExams, chapter.TotalPractices);
                }

                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", chapter },
                    { "Error", null }
                });
            }
        }

        [HttpPost]
        public async Task<JsonResult> JoinChapter(string ID, string JoinChapter, string newName, string CreateNewChapter = "off")
        {
            try
            {
                var _userCreate = User.Claims.GetClaimByType("UserID").Value;
                var rootChap = _chapterService.GetItemByID(ID);
                var joinChap = _chapterService.GetItemByID(JoinChapter);
                if (rootChap == null || joinChap == null)
                {
                    return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", null },
                        { "Error", "Dữ liệu không đúng" }
                    });
                }
                var currentChapIndex = (int)_chapterService.GetSubChapters(rootChap.ClassSubjectID, rootChap.ParentID).Count();
                var currentLessonIndex = (int)_lessonService.CountChapterLesson(rootChap.ID);

                var joinLessons = _lessonService.GetChapterLesson(joinChap.ClassSubjectID, joinChap.ID).OrderBy(o => o.Order);
                var joinSubChaps = _chapterService.GetSubChapters(joinChap.CourseID, joinChap.ID);

                if (CreateNewChapter.Equals("on"))
                {

                    var chapMap = new MappingEntity<ChapterEntity, ChapterEntity>();
                    var clonechap = chapMap.Clone(rootChap, new ChapterEntity());
                    clonechap.Order = currentChapIndex;
                    if (newName != null || newName != "")
                        clonechap.Name = newName;
                    clonechap.OriginID = rootChap.ID;
                    var newChapter = await _classHelper.CloneChapter(clonechap, _userCreate, rootChap.ClassSubjectID); ;

                    var lessonMapping = new MappingEntity<CourseLessonEntity, CourseLessonEntity>();
                    //var new_lesson = new CourseLessonEntity();
                    if (joinLessons != null && joinLessons.Count() > 0)
                        foreach (var o in joinLessons)
                        {
                            await _lessonHelper.CopyLessonFromLesson(o, new LessonEntity
                            {
                                CreateUser = _userCreate,
                                ChapterID = newChapter.ID,
                                Order = currentLessonIndex++
                            });
                            //await CloneLesson(new_lesson, _userCreate);
                        }
                    if (joinSubChaps != null && joinSubChaps.Count() > 0)
                        foreach (var o in joinSubChaps)
                        {
                            var clone_chap = chapMap.Clone(o, new ChapterEntity());
                            clone_chap.OriginID = o.ID;
                            clone_chap.ParentID = newChapter.ID;
                            clone_chap.Created = DateTime.UtcNow;
                            clone_chap.CreateUser = _userCreate;
                            clone_chap.Order = currentChapIndex++;
                            await _classHelper.CloneChapter(clone_chap, _userCreate, rootChap.CourseID);
                        }

                    //update new chapter counter
                    newChapter.TotalExams += joinChap.TotalExams;
                    newChapter.TotalLessons += joinChap.TotalLessons;
                    newChapter.TotalPractices += joinChap.TotalPractices;
                    _chapterService.Save(newChapter);

                    //add join chapter counter to parent holder
                    if (newChapter.ParentID == "0")
                        await _classHelper.IncreaseClassSubjectCounter(newChapter.ClassSubjectID, newChapter.TotalLessons, newChapter.TotalExams, newChapter.TotalPractices);
                    else
                        await _classHelper.IncreaseChapterCounter(newChapter.ParentID, newChapter.TotalLessons, newChapter.TotalExams, newChapter.TotalPractices);

                    return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", _chapterService.GetItemByID(newChapter.ID)},
                        { "Error", null }
                    });
                }
                else
                {
                    //Append joinChapter's lessons to bottom of rootchap's lessons list
                    if (joinLessons != null && joinLessons.Count() > 0)
                        foreach (var lesson in joinLessons.ToList())
                        {
                            lesson.ChapterID = rootChap.ID;
                            lesson.Order = (int)currentLessonIndex++;
                            _lessonService.Save(lesson);
                        }
                    //Append joinChapter's subchapter to bottom of rootchap's subchapter list
                    if (joinSubChaps != null && joinSubChaps.Count() > 0)
                        foreach (var subchap in joinSubChaps)
                        {
                            subchap.ParentID = rootChap.ParentID;
                            subchap.Order = (int)currentChapIndex++;
                            _chapterService.Save(subchap);
                        }

                    //add joinchap counter to root counter
                    rootChap.TotalExams += joinChap.TotalExams;
                    rootChap.TotalLessons += joinChap.TotalLessons;
                    rootChap.TotalPractices += joinChap.TotalPractices;
                    _chapterService.Save(rootChap);

                    //Move joinChapter to bottom of parent chapter list to correct order
                    ChangeChapterPosition(joinChap, int.MaxValue);
                    //Then remove join chapter
                    _chapterService.Remove(joinChap.ID);

                    return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", _chapterService.GetItemByID(rootChap.ID) },
                        { "Error", null }
                    });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", null },
                    { "Error", ex.Message }
                });
            }
        }
        #endregion

        public IActionResult CheckPoint(DefaultModel model, string basis)
        {
            if (!string.IsNullOrEmpty(model.ID))
            {
                ExamEntity data = _examService.GetItemByID(model.ID);
                if (data != null)
                {
                    var ExamTypes = quizType;

                    var lesson = _lessonService.GetItemByID(data.LessonID);

                    var listParts = _cloneLessonPartService.GetByLessonID(data.LessonID).Where(o => ExamTypes.Contains(o.Type));

                    var mapping = new MappingEntity<LessonEntity, StudentLessonViewModel>();
                    var mapPart = new MappingEntity<CloneLessonPartEntity, PartViewModel>();
                    var mapQuestion = new MappingEntity<CloneLessonPartQuestionEntity, QuestionViewModel>();
                    var mapExam = new MappingEntity<ExamEntity, ExamReviewViewModel>();

                    var examview = mapExam.AutoOrtherType(data, new ExamReviewViewModel()
                    {
                        Details = _examDetailService.Collection.Find(t => t.ExamID == data.ID).ToList()
                    });

                    var lessonview = mapping.AutoOrtherType(lesson, new StudentLessonViewModel()
                    {
                        Part = listParts.Select(o => mapPart.AutoOrtherType(o, new PartViewModel()
                        {
                            Questions = _cloneLessonPartQuestionService.CreateQuery().Find(x => x.ParentID == o.ID).ToList()
                                .Select(z => mapQuestion.AutoOrtherType(z, new QuestionViewModel()
                                {
                                    CloneAnswers = _cloneLessonPartAnswerService.CreateQuery().Find(x => x.ParentID == z.ID).ToList(),
                                    AnswerEssay = o.Type == "ESSAY" ? _examDetailService.CreateQuery().Find(e => e.QuestionID == z.ID && e.ExamID == data.ID)?.FirstOrDefault()?.AnswerValue : string.Empty,
                                    Medias = examview.Details.FirstOrDefault(e => e.QuestionID == z.ID)?.Medias,
                                    TypeAnswer = o.Type,
                                    RealAnswerEssay = o.Type == "ESSAY" ? examview.Details.FirstOrDefault(e => e.QuestionID == z.ID)?.RealAnswerValue : string.Empty,
                                    PointEssay = examview.Details.FirstOrDefault(e => e.QuestionID == z.ID)?.Point ?? 0,
                                    ExamDetailID = examview.Details.FirstOrDefault(e => e.QuestionID == z.ID)?.ID ?? "",
                                    MediasAnswer = examview.Details.FirstOrDefault(e => e.QuestionID == z.ID)?.MediasAnswers,
                                    MaxPoint = z.Point
                                }))?.ToList()
                        })).ToList()
                    });

                    ViewBag.Lesson = lessonview;
                    //ViewBag.Class = currentClass;
                    //ViewBag.Subject = currentCs;
                    //ViewBag.NextLesson = nextLesson;
                    //ViewBag.Chapter = chapter;
                    ViewBag.Type = lesson.TemplateType;
                    ViewBag.Exam = examview;
                }
            }
            ViewBag.Model = model;
            return View();
        }

        private class StudentResult
        {
            public string StudentID { get; set; }
            public int ExamCount { get; set; }
            public double AvgPracticePoint { get; set; }
            public double AvgExamPoint { get; set; }
            public string StudentName { get; set; }
            public string AvgTimeDoExam { get; set; }
            public int CompletedExam { get; set; }
            public int CompletedPractice { get; set; }
            public int CompletedLesson { get; set; }
            public int TotalLesson { get; set; }
            public int TotalPractice { get; set; }
            public Boolean isExam { get; set; }
        }

        #region File Excel
        //[HttpPost]
        public IActionResult ExportTablePoint(String basis, String ClassID)
        {
            var center = _centerService.GetItemByCode(basis);
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            var teacher = _teacherService.CreateQuery().Find(t => t.ID == UserID).SingleOrDefault();
            if (String.IsNullOrEmpty(ClassID))
            {
                return Json(new Dictionary<String, Object>
                {
                    {"Data","Không tìm thấy lớp tương ứng" }
                });
            }

            var @class = _classService.GetItemByID(ClassID);
            List<StudentSummaryViewModel> listSummary = ClassResultSummary(@class);

            var stream = new MemoryStream();

            //xuat file excel
            try
            {
                using (ExcelPackage p = new ExcelPackage(stream))
                {
                    // đặt tên người tạo file
                    p.Workbook.Properties.Author = teacher.FullName;

                    // đặt tiêu đề cho file
                    p.Workbook.Properties.Title = $"Bảng điểm {@class.Name}";

                    //Tạo một sheet để làm việc trên đó
                    p.Workbook.Worksheets.Add($"{@class.Name}");

                    // lấy sheet vừa add ra để thao tác
                    ExcelWorksheet ws = p.Workbook.Worksheets[1];

                    // đặt tên cho sheet
                    ws.Name = $"Bảng điểm {@class}";
                    // fontsize mặc định cho cả sheet
                    ws.Cells.Style.Font.Size = 11;
                    // font family mặc định cho cả sheet
                    ws.Cells.Style.Font.Name = "Calibri";

                    // Tạo danh sách các column header
                    string[] arrColumnHeader1 = new string[]{
                        "STT",
                        "Họ tên",
                        "Tiến độ",
                        "Lần học cuối",
                        "Luyện tập",
                        "",
                        "Kiểm tra",
                        "",
                        "Xếp hạng"
                    };

                    string[] arrColumnHeader2 = new string[]{
                        "Tiến độ",
                        "Điểm trung bình",
                        "Tiến độ",
                        "Điểm trung bình",
                    };

                    // lấy ra số lượng cột cần dùng dựa vào số lượng header
                    var countColHeader = arrColumnHeader1.Count();

                    // merge các column lại từ column 1 đến số column header
                    // gán giá trị cho cell vừa merge là Thống kê thông tni User Kteam
                    // ws.Cells[1, 1].Value = ClassID == null ? $"Thống kê danh sách học viên {center.Name}" : $"Thống kê danh sách học viên lớp @class";
                    ws.Cells[1, 1, 1, countColHeader].Merge = true;
                    // in đậm
                    ws.Cells[1, 1, 1, countColHeader].Style.Font.Bold = true;
                    // căn giữa
                    ws.Cells[1, 1, 1, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    int colIndex = 1;
                    int rowIndex = 2;

                    //tạo các header từ column header đã tạo từ bên trên
                    foreach (var item in arrColumnHeader1)
                    {
                        var cell = ws.Cells[rowIndex, colIndex];
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Justify;
                        //set màu thành gray
                        var fill = cell.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                        //căn chỉnh các border
                        var border = cell.Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;
                        cell.AutoFitColumns();
                        //gán giá trị
                        cell.Value = item;
                        if (colIndex <= 4 || colIndex >= 9)
                            ws.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
                        colIndex++;
                    }
                    ws.Cells[2, 5, 2, 6].Merge = true;
                    ws.Cells[2, 7, 2, 8].Merge = true;

                    Int32 rowIndex1 = 3, colIndex1 = 5;
                    foreach (var item in arrColumnHeader2)
                    {
                        var cell = ws.Cells[rowIndex1, colIndex1];
                        cell.AutoFitColumns();
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Justify;
                        //set màu thành gray
                        var fill = cell.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                        //căn chỉnh các border
                        var border = cell.Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        //gán giá trị
                        cell.Value = item;
                        colIndex1++;
                    }

                    Int32 rowIndexData = 4;
                    foreach (var item in listSummary)
                    {
                        Int32 colIndexData = 1;
                        //gan gia tri cho tung cell
                        ws.Cells[rowIndexData, colIndexData++].Value = rowIndexData - 3;
                        ws.Cells[rowIndexData, colIndexData++].Value = item.FullName;
                        ws.Cells[rowIndexData, colIndexData++].Value = $"{Math.Round(((Double)item.Completed / item.TotalLessons) * 100, 2)}% ({item.Completed}/{item.TotalLessons})";
                        ws.Cells[rowIndexData, colIndexData++].Value = $"{item.LastDate.ToString()}";
                        ws.Cells[rowIndexData, colIndexData++].Value = $"{item.PracticeDone}/{item.TotalPractices}";
                        if (item.PracticeAvgPoint > 0)
                            ws.Cells[rowIndexData, colIndexData++].Value = $"{Math.Round(item.PracticeAvgPoint, 2)}%";
                        else
                            ws.Cells[rowIndexData, colIndexData++].Value = $"0.00%";
                        ws.Cells[rowIndexData, colIndexData++].Value = $"{item.ExamDone}/{item.TotalExams}";
                        if (item.AvgPoint > 0)
                            ws.Cells[rowIndexData, colIndexData++].Value = $"{Math.Round(item.AvgPoint, 2)}%";
                        else
                            ws.Cells[rowIndexData, colIndexData++].Value = $"0.00%";
                        if (item.Rank > 0)
                            ws.Cells[rowIndexData, colIndexData++].Value = $"{item.Rank}/{item.TotalStudents}";
                        else
                            ws.Cells[rowIndexData, colIndexData++].Value = $"---";

                        rowIndexData++;
                    }

                    for (Int32 i = 1; i < listSummary.Count + 4; i++)
                    {
                        for (Int32 j = 1; j <= arrColumnHeader1.Count(); j++)
                        {
                            var cell = ws.Cells[i, j];
                            //căn chỉnh các border
                            var border = cell.Style.Border;
                            border.Bottom.Style =
                                border.Top.Style =
                                border.Left.Style =
                                border.Right.Style = ExcelBorderStyle.Thin;
                            cell.AutoFitColumns();

                            if (i == 3 && (j < 5 || j > 8))
                            {
                                ws.Cells[2, j, i, j].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[2, j, i, j].Style.VerticalAlignment = ExcelVerticalAlignment.Justify;
                            }
                            else
                            {
                                if (j == 1 || j >= 5)
                                {
                                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    cell.Style.VerticalAlignment = ExcelVerticalAlignment.Justify;
                                }
                                else if (j == 2)
                                {
                                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    cell.Style.VerticalAlignment = ExcelVerticalAlignment.Justify;
                                }
                                else
                                {
                                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    cell.Style.VerticalAlignment = ExcelVerticalAlignment.Justify;
                                }
                            }

                        }
                    }
                    p.Save();
                }
                stream.Position = 0;
                string excelName = $"Bảng điểm {@class.Name}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
        #endregion

        public class TablePoint
        {
            public String StudentName { get; set; }
            public String Progress { get; set; }
            public String LastLearn { get; set; }
            public String ProgressPractice { get; set; }
            public String AvgPointPractice { get; set; }
            public String ProgressExam { get; set; }
            public String AvgPointExam { get; set; }
            public String Rank { get; set; }
        }

        //test
        public async Task<List<StudentLessonResultViewModel>> GetLessonProgressListTest(DateTime StartWeek, DateTime EndWeek, List<String> studentIDs, ClassSubjectEntity classSbj, Boolean isExam = false)
        {
            List<StudentLessonResultViewModel> result = new List<StudentLessonResultViewModel>();
            if (StartWeek > classSbj.EndDate) return result;
            if (StartWeek == new DateTime(2020, 10, 12))
            {
                var test = "";
            }
            //lay danh sach bai hoc trogn tuan
            var activeLessons = _lessonScheduleService.CreateQuery().Find(o => o.ClassSubjectID == classSbj.ID && o.StartDate <= EndWeek && o.EndDate >= StartWeek).ToList();
            var activeLessonIds = activeLessons.Select(t => t.LessonID).ToList();

            //danh sach bai luyen tap
            List<LessonEntity> practices = new List<LessonEntity>();

            if (isExam)
            {
                practices = _lessonService.CreateQuery().Find(x => x.TemplateType == 2 && activeLessonIds.Contains(x.ID)).ToList();
            }
            else
            {
                practices = _lessonService.CreateQuery().Find(x => x.IsPractice == true && activeLessonIds.Contains(x.ID)).ToList();
            }

            if (practices.Count > 0)
            {
                var a = "";
            }

            foreach (var practice in practices)
            {
                var examresult = _examService.CreateQuery().Find(t => studentIDs.Contains(t.StudentID) && t.LessonID == practice.ID).SortByDescending(t => t.ID).ToList();
                var progress = _lessonProgressService.CreateQuery().Find(t => studentIDs.Contains(t.StudentID) && t.LessonID == practice.ID).FirstOrDefault(); ;
                var tried = examresult.Count();
                var maxpoint = tried == 0 ? 0 : examresult.Max(t => t.MaxPoint > 0 ? t.Point * 100 / t.MaxPoint : 0);
                var minpoint = tried == 0 ? 0 : examresult.Min(t => t.MaxPoint > 0 ? t.Point * 100 / t.MaxPoint : 0);
                var avgpoint = tried == 0 ? 0 : examresult.Average(t => t.MaxPoint > 0 ? t.Point * 100 / t.MaxPoint : 0);

                var lastEx = examresult.FirstOrDefault();
                result.Add(new StudentLessonResultViewModel(new StudentEntity())
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
                    LessonName = practice.Title,
                    LessonID = practice.ID,
                    ClassSubjectID = practice.ClassSubjectID
                });
            }
            return result;
        }
    }
}
