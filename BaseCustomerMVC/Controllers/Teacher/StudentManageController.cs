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
        private readonly ClassService _classService;
        private readonly StudentService _studentService;
        private readonly StudentHelper _studentHelper;
        private readonly IHostingEnvironment _env;


        public StudentManageController(
            AccountService accountService,
            GradeService gradeservice,
            SubjectService subjectService,
            TeacherService teacherService,
            ClassService classService,
            StudentService studentService,
            IHostingEnvironment evn
            )
        {
            _accountService = accountService;
            _gradeService = gradeservice;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _classService = classService;
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

            ViewBag.User = UserID;
            ViewBag.Model = model;
            ViewBag.Managable = CheckPermission(PERMISSION.MEMBER_COURSE_EDIT);
            if (old == 1)
                return View("Index - o");
            return View();
        }

        public JsonResult RemoveStudent(string ClassID, string StudentID)
        {
            if (string.IsNullOrEmpty(ClassID))
                return Json(new { Error = "Class not found" });
            var currentClass = _classService.GetItemByID(ClassID);
            if (currentClass == null || !currentClass.IsActive)
                return Json(new { error = "Class not found" });
            _ = _classService.RemoveStudent(ClassID, StudentID);
            return Json(new { message = "Student Remove OK" });
        }

        public JsonResult AddStudent(string ClassID, string StudentID)
        {
            if (string.IsNullOrEmpty(ClassID))
                return Json(new { Error = "Class not found" });
            var student = _studentService.GetItemByID(StudentID);
            if (student == null)
                return Json(new { Error = "Student not avaiable" });
            if (_classService.AddStudentToClass(ClassID, student.ID) > 0)
            {
                if (!student.IsActive)
                {
                    _studentHelper.ChangeStatus(student.ID, true);
                }
                return Json(new { data = student });
            }
            else
                return Json(new { error = "Can't add student now" });
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
        public async Task<JsonResult> ImportStudent(string ID)
        {
            var form = HttpContext.Request.Form;
            if (string.IsNullOrEmpty(ID)) return new JsonResult("Fail");
            var itemCourse = _classService.GetItemByID(ID);
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
                        _classService.CreateQuery().ReplaceOne(o => o.ID == itemCourse.ID, itemCourse);
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
