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
using System.Globalization;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class StudentManageController : TeacherController
    {
        private readonly GradeService _gradeService;
        private readonly AccountService _accountService;
        private readonly RoleService _roleService;
        private readonly SubjectService _subjectService;
        private readonly TeacherService _teacherService;
        private readonly SkillService _skillService;
        private readonly ClassService _classService;
        private readonly ClassStudentService _classStudentService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly StudentService _studentService;
        private readonly StudentHelper _studentHelper;
        private readonly ClassProgressService _classProgressService;
        private readonly ClassSubjectProgressService _classSubjectProgressService;
        private readonly ScoreStudentService _scoreStudentService;
        private readonly LearningHistoryService _learningHistoryService;
        private readonly ExamService _examService;
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly IHostingEnvironment _env;


        public StudentManageController(
            AccountService accountService,
            RoleService roleService,
            GradeService gradeservice,
            SubjectService subjectService,
            TeacherService teacherService,
            ClassService classService,
            SkillService skillService,
            ExamService examService,
            ClassStudentService classStudentService,
            ClassSubjectService classSubjectService,
            LearningHistoryService learningHistoryService,
            ClassProgressService classProgressService,
            ClassSubjectProgressService classSubjectProgressService,
            ScoreStudentService scoreStudentService,
            LessonScheduleService lessonScheduleService,
            StudentService studentService,
            IHostingEnvironment evn
            )
        {
            _accountService = accountService;
            _roleService = roleService;
            _gradeService = gradeservice;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _examService = examService;
            _learningHistoryService = learningHistoryService;
            _classService = classService;
            _skillService = skillService;
            _classProgressService = classProgressService;
            _classSubjectProgressService = classSubjectProgressService;
            _scoreStudentService = scoreStudentService;
            _classStudentService = classStudentService;
            _classSubjectService = classSubjectService;
            _lessonScheduleService = lessonScheduleService;
            _studentService = studentService;
            _env = evn;

            _studentHelper = new StudentHelper(studentService, accountService);
        }

        public IActionResult Index(DefaultModel model)
        {
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            var teacher = _teacherService.CreateQuery().Find(t => t.ID == UserID).SingleOrDefault();

            var subject = new List<SubjectEntity>();
            var grade = new List<GradeEntity>();
            if (teacher != null && teacher.Subjects != null)
            {
                subject = _subjectService.CreateQuery().Find(t => teacher.Subjects.Contains(t.ID)).ToList();
                grade = _gradeService.CreateQuery().Find(t => teacher.Subjects.Contains(t.SubjectID)).ToList();

            }

            ViewBag.Grade = grade;
            ViewBag.Subject = subject;
            ViewBag.Skills = _skillService.GetList();

            ViewBag.User = UserID;
            ViewBag.Model = model;
            ViewBag.Managable = CheckPermission(PERMISSION.MEMBER_COURSE_EDIT);
            return View();
        }

        public IActionResult Detail(DefaultModel model)
        {
            var student = _studentService.GetItemByID(model.ID);
            if (student == null)
                return RedirectToAction("Index");
            ViewBag.Student = student;
            return View();
        }

        public JsonResult RemoveStudent(string ClassID, string StudentID)
        {
            if (string.IsNullOrEmpty(ClassID) || string.IsNullOrEmpty(StudentID))
            {
                return Json(new
                {
                    error = "Thông tin không chính xác"
                });
            }
            //var deleted = _classStudentService.RemoveClassStudent(ClassID, StudentID);
            if (_studentService.LeaveClass(ClassID, StudentID) > 0)
            {
                //remove history, exam, exam detail, progress...
                _learningHistoryService.RemoveClassStudentHistory(ClassID, StudentID);
                _examService.RemoveClassStudentExam(ClassID, StudentID);
            }
            return Json(new { msg = "đã xóa học viên" });
        }

        public JsonResult AddStudent(string ClassID, string StudentID)
        {
            if (string.IsNullOrEmpty(ClassID))
                return Json(new { error = "Lớp không tồn tại" });
            var @class = _classService.GetItemByID(ClassID);
            if (@class == null)
                return Json(new { error = "Lớp không tồn tại" });
            var student = _studentService.GetItemByID(StudentID);
            if (student == null)
                return Json(new { error = "Học viên không tồn tại" });
            if (_studentService.IsStudentInClass(ClassID, student.ID))
            {
                return Json(new { data = @class, msg = "Học viên đã có trong lớp" });
            }
            if (_studentService.JoinClass(ClassID, StudentID) > 0)
                return Json(new { data = @class, msg = "Học viên đã được thêm vào lớp" });
            return Json(new { error = "Có lỗi, vui lòng thực hiện lại" });
        }

        public JsonResult GetList(DefaultModel model, string SubjectID, string ClassID, string TeacherID, string SkillID, string GradeID)
        {
            var filterCs = new List<FilterDefinition<ClassSubjectEntity>>();
            if (User.IsInRole("teacher"))
            {
                TeacherID = User.Claims.GetClaimByType("UserID").Value;
            }
            var classids = new List<string>();
            if (!string.IsNullOrEmpty(ClassID))
            {
                classids.Add(ClassID);
            }
            else
            {
                if (!string.IsNullOrEmpty(SubjectID))
                    filterCs.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.SubjectID == SubjectID));
                if (!string.IsNullOrEmpty(TeacherID))
                    filterCs.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.TeacherID == TeacherID));
                if (!string.IsNullOrEmpty(SkillID))
                    filterCs.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.SkillID == SkillID));
                if (!string.IsNullOrEmpty(GradeID))
                    filterCs.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.GradeID == GradeID));
                classids =
                    filterCs.Count > 0 ? _classSubjectService.Collection.Distinct(t => t.ClassID, Builders<ClassSubjectEntity>.Filter.And(filterCs)).ToList()
                : _classService.Collection.Find(t => true).Project(t => t.ID).ToList();
            }

            if (classids == null || classids.Count() == 0)
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Model", model }
                });
            var list =
                    string.IsNullOrEmpty(model.SearchText) ?
                        _studentService.Collection.Find(Builders<StudentEntity>.Filter.AnyIn(t => t.JoinedClasses, classids))
                        .SortByDescending(t => t.ID)
                        :
                        _studentService.Collection.Find(Builders<StudentEntity>.Filter.And(
                        Builders<StudentEntity>.Filter.AnyIn(t => t.JoinedClasses, classids),
                        Builders<StudentEntity>.Filter.Text("\"" + model.SearchText + "\"")))
                        .SortByDescending(t => t.ID);
            //var list = _classStudentService.Collection.Find(t => classids.Contains(t.ClassID) && studentids.Contains(t.StudentID)).SortByDescending(t => t.ID);
            model.TotalRecord = list.CountDocuments();

            var _mapping = new MappingEntity<StudentEntity, ClassStudentViewModel>();

            var retStudents = list.Skip(model.PageIndex * model.PageSize).Limit(model.PageSize).ToList()
                .Select(t => _mapping.AutoOrtherType(t, new ClassStudentViewModel()
                {
                    ClassID = ClassID,
                    ClassName = string.IsNullOrEmpty(ClassID) ?
                        string.Join("; ", _classService.GetMultipleClassName(t.JoinedClasses)) :
                        _classService.GetItemByID(ClassID).Name
                }));

            var response = new Dictionary<string, object>
            {
                { "Data", retStudents },
                { "Model", model }
            };
            return new JsonResult(response);
        }

        public JsonResult Search(string term)
        {
            return Json(_studentService.Search(term, 100));
        }

        #region Batch Import
        [HttpPost]
        [Obsolete]
        public async Task<JsonResult> ImportStudent(string ClassID)
        {
            var form = HttpContext.Request.Form;
            if (string.IsNullOrEmpty(ClassID))
                return Json(new { error = "Không có thông tin lớp" });
            var @class = _classService.GetItemByID(ClassID);
            if (@class == null)
                return Json(new { error = "Không có thông tin lớp" });

            if (form == null) return new JsonResult(null);
            if (form.Files == null || form.Files.Count <= 0) return new JsonResult(null);
            var file = form.Files[0];
            var filePath = Path.Combine(_env.WebRootPath, @class.ID + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss"));
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
                stream.Close();
                try
                {
                    var counter = 0;

                    using (var readStream = new FileStream(filePath, FileMode.Open))
                    {
                        using (ExcelPackage package = new ExcelPackage(readStream))
                        {
                            ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                            int totalRows = workSheet.Dimension.Rows;
                            var classStudents =
                                _studentService.GetClassStudentIDs(ClassID);
                            var keyCol = 4;
                            var defPass = "Eduso123";
                            for (int i = 1; i <= totalRows; i++)
                            {
                                if (workSheet.Cells[i, 1].Value == null || workSheet.Cells[i, 1].Value.ToString() == "STT") continue;
                                var studentEmail = workSheet.Cells[i, keyCol].Value == null ? "" : workSheet.Cells[i, keyCol].Value.ToString();
                                if (string.IsNullOrEmpty(studentEmail)) continue;
                                var student = _studentService.CreateQuery().Find(o => o.Email == studentEmail).SingleOrDefault();

                                if (student == null) continue;
                                if (classStudents.Any(t => t == student.ID)) continue;

                                _classStudentService.Save(new ClassStudentEntity
                                {
                                    StudentID = student.ID,
                                    ClassID = @class.ID
                                });
                                counter++;
                            }
                        }
                    }
                    System.IO.File.Delete(filePath);
                    return Json(new { msg = "Đã thêm mới " + counter + " học viên" });
                }
                catch (Exception ex)
                {
                    return Json(new { error = ex.Message });
                }
            }
        }

        public async Task<IActionResult> ExportTemplate(DefaultModel model)
        {
            var list = new List<StudentEntity>() { new StudentEntity() {
                ID = "undefined"
                } };
            var data = list.Select(o => new
            {
                STT = 1,
                Ho_ten = "Nguyễn Văn A",
                Ngay_sinh = "dd/mm/yyyy",
                Email = "email@gmail.com",
                SDT = "0123456789",
                SkypeId = "skypeid",
            });
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("DS_HV");
                workSheet.Cells.LoadFromCollection(data, true);
                package.Save();
            }
            stream.Position = 0;
            string excelName = $"StudentTemplate.xlsx";

            //return File(stream, "application/octet-stream", excelName);  
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
        #endregion

        public async Task<JsonResult> ConvertStudent()
        {
            var classes = _classService.GetAll().ToList();
            var count = 0;
            var str = "";
            _studentService.Collection.UpdateMany(t => true, Builders<StudentEntity>.Update.Unset(t => t.JoinedClasses));
            foreach (var @class in classes)
            {
                await _studentService.LeaveClassAll(@class.ID);
                //if (@class.ID != "5e652e05fd6d8e01304cd67c") continue;
                //_classStudentService.RemoveClass(@class.ID);
                var students = _classStudentService.GetClassStudents(@class.ID);

                foreach (var student in students)
                {
                    if (_studentService.JoinClass(@class.ID, student.StudentID) > 0)
                        count++;
                    else
                    {
                        str += student.ID + "<br/>";
                    }
                }
            }
            return new JsonResult("OK " + str);
        }

    }
}
