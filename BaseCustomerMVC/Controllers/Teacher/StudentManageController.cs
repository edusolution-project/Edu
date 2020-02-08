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

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class StudentManageController : TeacherController
    {
        private readonly GradeService _gradeService;
        private readonly AccountService _accountService;
        private readonly SubjectService _subjectService;
        private readonly TeacherService _teacherService;
        private readonly SkillService _skillService;
        private readonly ClassService _classService;
        private readonly ClassStudentService _classStudentService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly StudentService _studentService;
        private readonly StudentHelper _studentHelper;
        private readonly ClassProgressService _progressService;
        private readonly ScoreStudentService _scoreStudentService;
        private readonly LearningHistoryService _learningHistoryService;
        private readonly ExamService _examService;
        private readonly IHostingEnvironment _env;


        public StudentManageController(
            AccountService accountService,
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
            ScoreStudentService scoreStudentService,
            StudentService studentService,
            IHostingEnvironment evn
            )
        {
            _accountService = accountService;
            _gradeService = gradeservice;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _examService = examService;
            _learningHistoryService = learningHistoryService;
            _classService = classService;
            _skillService = skillService;
            _progressService = classProgressService;
            _scoreStudentService = scoreStudentService;
            _classStudentService = classStudentService;
            _classSubjectService = classSubjectService;
            _studentService = studentService;
            _env = evn;

            _studentHelper = new StudentHelper(studentService, accountService);
        }

        public IActionResult Index(DefaultModel model, int old = 0)
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
            if (old == 1)
                return View("Index - o");
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
            var deleted = _classStudentService.RemoveClassStudent(ClassID, StudentID);
            if (deleted > 0)
            {
                //remove history, exam, exam detail, progress...
                _learningHistoryService.RemoveClassStudentHistory(ClassID, StudentID);
                _examService.RemoveClassStudentExam(ClassID, StudentID);
            }
            return Json(new { msg = "đã xóa " + deleted + " học viên" });
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
            var classstudents = _classStudentService.GetClassStudents(ClassID);
            if (classstudents.Any(t => t.StudentID == StudentID))
            {
                //already in class
                return Json(new { data = @class, msg = "Học viên đã có trong lớp" });
            }
            var newstudent = new ClassStudentEntity { ClassID = ClassID, StudentID = StudentID };
            _classStudentService.Save(newstudent);
            if (newstudent.ID != null)
                return Json(new { data = @class, msg = "Học viên đã được thêm vào lớp" });
            return Json(new { error = "Có lỗi, vui lòng thực hiện lại" });
        }

        public JsonResult GetList(DefaultModel model, string SubjectID, string ClassID, string TeacherID, string SkillID, string GradeID)
        {
            var filterCs = new List<FilterDefinition<ClassSubjectEntity>>();
            if (!string.IsNullOrEmpty(ClassID))
                filterCs.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.ClassID == ClassID));
            if (!string.IsNullOrEmpty(SubjectID))
                filterCs.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.SubjectID == SubjectID));
            else
            {
                //filter trong những môn được phân công
            }
            if (!string.IsNullOrEmpty(TeacherID))
                filterCs.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.TeacherID == TeacherID));
            if (!string.IsNullOrEmpty(SkillID))
                filterCs.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.SkillID == SkillID));
            if (!string.IsNullOrEmpty(GradeID))
                filterCs.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.GradeID == GradeID));


            var classids = (filterCs.Count > 0 ? _classSubjectService.Collection
                .Distinct(t => t.ClassID, Builders<ClassSubjectEntity>.Filter.And(filterCs))
                //.Find(Builders<ClassSubjectEntity>.Filter.And(filterCs))
                : _classSubjectService.Collection.Distinct(t => t.ClassID, Builders<ClassSubjectEntity>.Filter.Empty)).ToList();

            if (classids == null || classids.Count() == 0)
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Model", model }
                });

            var studentids = _classStudentService.Collection.Find(t => classids.Contains(t.ClassID)).SortByDescending(t => t.ID);
            model.TotalRecord = studentids.CountDocuments();
            var filter = new List<FilterDefinition<StudentEntity>>();

            var retStudents = studentids.Skip(model.PageIndex * model.PageSize).Limit(model.PageSize).ToList();

            //filter.Add(Builders<StudentEntity>.Filter.Where(o => retStudents.Contains(o.ID)));
            //var students = filter.Count > 0 ? _studentService.Collection.Find(Builders<StudentEntity>.Filter.And(filter)) : _studentService.GetAll();

            var _mapping = new MappingEntity<StudentEntity, ClassStudentViewModel>();

            var studentsView =
                (from r in retStudents
                 let @class = _classService.GetItemByID(r.ClassID)
                 //where @class != null
                 let @student = _studentService.GetItemByID(r.StudentID)
                 //where @student != null
                 let progress = _progressService.GetItemByClassID(@class.ID, @student.ID)
                 let percent = (progress == null || progress.TotalLessons == 0) ? 0 : progress.CompletedLessons.Count * 100 / progress.TotalLessons
                 select _mapping.AutoOrtherType(@student, new ClassStudentViewModel()
                 {
                     ClassID = @class.ID,
                     ClassName = @class.Name,
                     ClassStatus = "Đang học",
                     LastJoinDate = DateTime.Now,
                     Progress = progress,
                     Percent = percent > 100 ? 100 : percent,
                     Score = _scoreStudentService.GetScoreStudentByStudentIdAndClassId(@class.ID, @student.ID)
                 })).ToList();

            var response = new Dictionary<string, object>
            {
                { "Data", studentsView },
                { "Model", model }
            };
            return new JsonResult(response);
        }

        public JsonResult Search(string term)
        {
            return Json(_studentService.Search(term, 100));
        }

        //[HttpPost]
        //[DisableRequestSizeLimit]
        //public async Task<JsonResult> SaveInfo(ClassEntity entity)
        //{

        //    if (String.IsNullOrEmpty(entity.ID))
        //    {
        //        new JsonResult(
        //            new Dictionary<string, object>
        //            {
        //                { "Error", "Không có thông tin lớp"}
        //            });
        //    }

        //    var currentClass = _service.GetItemByID(entity.ID);
        //    if (currentClass == null)
        //        if (String.IsNullOrEmpty(entity.ID))
        //        {
        //            new JsonResult(
        //                new Dictionary<string, object>
        //                {
        //                { "Error", "Không có thông tin lớp"}
        //                });
        //        }

        //    try
        //    {
        //        var files = HttpContext.Request.Form != null && HttpContext.Request.Form.Files.Count > 0 ? HttpContext.Request.Form.Files : null;
        //        if (files != null && files.Count > 0)
        //        {
        //            var file = files[0];

        //            var filename = currentClass.ID + "_" + DateTime.Now.ToUniversalTime().ToString("yyyyMMddhhmmss") + Path.GetExtension(file.FileName);
        //            currentClass.Image = await _fileProcess.SaveMediaAsync(file, filename);
        //        }
        //        currentClass.Description = entity.Description ?? "";
        //        currentClass.Syllabus = entity.Syllabus ?? "";
        //        currentClass.Modules = entity.Modules ?? "";
        //        currentClass.LearningOutcomes = entity.LearningOutcomes ?? "";
        //        currentClass.References = entity.References ?? "";
        //        _service.CreateOrUpdate(currentClass);

        //        return new JsonResult(
        //            new Dictionary<string, object>
        //            {
        //                { "Data", currentClass }
        //            }
        //        );
        //    }
        //    catch (Exception e)
        //    {
        //        return new JsonResult(
        //                new Dictionary<string, object>
        //                {
        //                    { "Error", e.Message}
        //                });
        //    }
        //}

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
                            var classStudents = _classStudentService.GetClassStudents(ClassID);
                            var keyCol = 2;
                            for (int i = 1; i <= totalRows; i++)
                            {
                                if (workSheet.Cells[i, 1].Value == null || workSheet.Cells[i, 1].Value.ToString() == "STT") continue;
                                var studentEmail = workSheet.Cells[i, keyCol].Value == null ? "" : workSheet.Cells[i, keyCol].Value.ToString();
                                if (string.IsNullOrEmpty(studentEmail)) continue;
                                var student = _studentService.CreateQuery().Find(o => o.Email == studentEmail).SingleOrDefault();

                                if (student == null) continue;
                                if (classStudents.Any(t => t.StudentID == student.ID)) continue;

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

        public IActionResult ConvertStudent()
        {
            var classes = _classService.GetAll().ToList();
            foreach (var @class in classes)
            {
                _classStudentService.RemoveClass(@class.ID);
                var students = @class.Students;
                foreach (var student in students)
                {
                    _classStudentService.Save(new ClassStudentEntity
                    {
                        ClassID = @class.ID,
                        StudentID = student
                    });
                }
            }
            return null;
        }

        public async Task<IActionResult> StudentTemplate(DefaultModel model)
        {

            var list = new List<StudentEntity>() { new StudentEntity() {
                ID = "undefined"
                } };
            var data = list.Select(o => new
            {
                STT = 1,
                Email = "email@gmail.com",
                Ten = "Nguyễn Văn A",
                Ngay_sinh = "01/30/1999",
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
    }
}
