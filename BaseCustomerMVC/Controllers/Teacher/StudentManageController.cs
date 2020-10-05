﻿using BaseCustomerEntity.Database;
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
using Microsoft.Extensions.Configuration;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Microsoft.AspNetCore.Razor.Language;
using OfficeOpenXml.ConditionalFormatting;
using OfficeOpenXml.Style;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class StudentManageController : TeacherController
    {
        private readonly GradeService _gradeService;
        private readonly AccountService _accountService;
        private readonly RoleService _roleService;
        private readonly SubjectService _subjectService;
        private readonly TeacherService _teacherService;
        private readonly TeacherHelper _teacherHelper;
        private readonly SkillService _skillService;
        private readonly ClassService _classService;
        //private readonly ClassStudentService _classStudentService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly StudentService _studentService;
        private readonly StudentHelper _studentHelper;
        private readonly ProgressHelper _progressHelper;
        private readonly ClassProgressService _classProgressService;
        private readonly ClassSubjectProgressService _classSubjectProgressService;
        private readonly ScoreStudentService _scoreStudentService;
        private readonly LearningHistoryService _learningHistoryService;
        private readonly ExamService _examService;
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly CenterService _centerService;
        private readonly IndexService _indexService;

        private readonly MailHelper _mailHelper;
        private readonly IHostingEnvironment _env;
        private IConfiguration _configuration;
        private readonly string _defaultPass;

        public StudentManageController(
            AccountService accountService,
            RoleService roleService,
            GradeService gradeservice,
            SubjectService subjectService,
            TeacherService teacherService,
            TeacherHelper teacherHelper,
            ClassService classService,
            SkillService skillService,
            ExamService examService,
            //ClassStudentService classStudentService,
            ClassSubjectService classSubjectService,
            LearningHistoryService learningHistoryService,
            ClassProgressService classProgressService,
            ClassSubjectProgressService classSubjectProgressService,
            ScoreStudentService scoreStudentService,
            LessonScheduleService lessonScheduleService,
            StudentService studentService,
            CenterService centerService,
            ProgressHelper progressHelper,
            StudentHelper studentHelper,
            IndexService indexService,
            MailHelper mailHelper,
            IHostingEnvironment evn,
            IConfiguration iConfig
            )
        {
            _accountService = accountService;
            _roleService = roleService;
            _gradeService = gradeservice;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _teacherHelper = teacherHelper;
            _examService = examService;
            _learningHistoryService = learningHistoryService;
            _classService = classService;
            _skillService = skillService;
            _classProgressService = classProgressService;
            _classSubjectProgressService = classSubjectProgressService;
            _scoreStudentService = scoreStudentService;
            //_classStudentService = classStudentService;
            _classSubjectService = classSubjectService;
            _lessonScheduleService = lessonScheduleService;
            _studentService = studentService;
            _centerService = centerService;
            _env = evn;
            _mailHelper = mailHelper;
            _configuration = iConfig;
            _defaultPass = _configuration.GetValue<string>("SysConfig:DP");
            _indexService = indexService;
            _studentHelper = studentHelper;
            _progressHelper = progressHelper;
        }

        public IActionResult Index(DefaultModel model, string basis)
        {
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            var teacher = _teacherService.CreateQuery().Find(t => t.ID == UserID).SingleOrDefault();
            if (teacher == null)
                return Redirect("/login");
            if (!string.IsNullOrEmpty(basis))
            {
                var center = _centerService.GetItemByCode(basis);
                if (center != null)
                    ViewBag.Center = center;
                ViewBag.IsHeadTeacher = _teacherHelper.HasRole(UserID, center.ID, "head-teacher");
            }

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

        public IActionResult Detail(DefaultModel model, string basis)
        {
            if (!string.IsNullOrEmpty(basis))
            {
                var center = _centerService.GetItemByCode(basis);
                if (center != null)
                    ViewBag.Center = center;
            }
            var student = _studentService.GetItemByID(model.ID);
            //var MyClassID = _classService.GetClassByMechanism(CLASS_MECHANISM.PERSONAL, student.ID).ID;
            //student.JoinedClasses.RemoveAt(student.JoinedClasses.IndexOf(MyClassID));
            if (student == null)
                return Redirect($"/{basis}{Url.Action("Index")}");
            ViewBag.Student = student;
            return View();
        }

        public async Task<JsonResult> CreateNewStudent(StudentEntity student, string basis)
        {
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            var teacher = _teacherService.GetItemByID(UserID);
            var center = _centerService.GetItemByCode(basis);
            var Status = false;

            long left = 0;
            if (!string.IsNullOrEmpty(basis))
            {
                center = _centerService.GetItemByCode(basis);
                if (center == null)
                    return null;
                var totalStudent = _studentService.CountByCenter(center.ID);
                if (center.Limit > 0)
                    left = center.Limit - totalStudent;
                else
                    left = long.MaxValue;
            }

            if (center == null || left <= 0)
            {
                Dictionary<string, object> response = new Dictionary<string, object>()
                    {
                        {"Data",null },
                        {"Error",null },
                        {"Msg",$"Cơ sở {center.Name} đã đạt hạt mức.Vui lòng liên hệ với quản trị viên để thêm hạn mức!" },
                        {"Status",Status }
                    };
                return Json(response);
                //return Json(new { error = "Cơ sở " + center.Name + " đã hết hạn mức." });
            }

            if (string.IsNullOrEmpty(student.ID) || student.ID == "0")
            {
                if (student.FullName == "" || student.Email == "") return null;

                if (!ExistEmail(student.Email))
                {
                    student.CreateDate = DateTime.Now;
                    student.IsActive = true;
                    student.UserCreate = teacher.ID;
                    student.Centers = new List<string>() { center.ID };
                    if (student.JoinedClasses != null && student.JoinedClasses[0] != null)
                    {
                        var listClass = student.JoinedClasses[0].Split(',');
                        student.JoinedClasses = listClass.ToList();
                    }
                    _studentService.CreateQuery().InsertOne(student);
                    Status = true;
                    Dictionary<string, object> response = new Dictionary<string, object>()
                    {
                        {"Data",student },
                        {"Error",null },
                        {"Msg","Thêm thành công" },
                        {"Status",Status }
                    };
                    var account = new AccountEntity()
                    {
                        CreateDate = DateTime.Now,
                        IsActive = true,
                        PassTemp = Core_v2.Globals.Security.Encrypt(_defaultPass),
                        PassWord = Core_v2.Globals.Security.Encrypt(_defaultPass),
                        UserCreate = student.UserCreate,
                        Type = ACCOUNT_TYPE.STUDENT,
                        UserID = student.ID,
                        UserName = student.Email.ToLower().Trim(),
                        RoleID = _roleService.GetItemByCode("student").ID
                    };
                    _accountService.CreateQuery().InsertOne(account);

                    if (student.JoinedClasses != null && student.JoinedClasses.Count > 0)
                    {
                        var pass = _defaultPass;
                        foreach (var clid in student.JoinedClasses)
                        {
                            var @class = _classService.GetItemByID(clid);
                            if (@class != null && !string.IsNullOrEmpty(@class.ID))
                                _ = _mailHelper.SendStudentJoinClassNotify(student.FullName, student.Email, pass, @class.Name, @class.StartDate, @class.EndDate, center.Name);
                            pass = "";//prevent send multiple mail with password
                        }
                    }
                    else
                        _ = _mailHelper.SendStudentJoinCenterNotify(student.FullName, student.Email, _defaultPass, center.Name);
                    return new JsonResult(response);
                }
                else
                {
                    Dictionary<string, object> response = new Dictionary<string, object>()
                    {
                        {"Data",null },
                        {"Error",student },
                        {"Msg","Email đã được sử dụng" },
                        {"Status",Status }
                    };
                    return new JsonResult(response);
                }
            }
            else
            {
                var oldStudent = _studentService.GetItemByID(student.ID);
                oldStudent.FullName = student.FullName;
                if (oldStudent.JoinedClasses == null) oldStudent.JoinedClasses = new List<string>();
                var listClass = new List<string>();
                if (!String.IsNullOrEmpty(student.JoinedClasses[0]))
                    listClass = student.JoinedClasses[0].Split(',').ToList();
                var newClasses = new List<string>();

                if (listClass != null && listClass.Count > 0)
                {
                    foreach (var newClass in listClass)
                    {
                        if (!string.IsNullOrEmpty(newClass))
                            if (oldStudent.JoinedClasses.IndexOf(newClass) < 0)
                            {
                                oldStudent.JoinedClasses.Add(newClass);
                                newClasses.Add(newClass);
                            }
                    }
                }


                var infochange = false;

                if (oldStudent.DateBorn != student.DateBorn)
                    oldStudent.DateBorn = student.DateBorn;

                if (oldStudent.Phone != student.Phone)
                {
                    oldStudent.Phone = student.Phone;
                    infochange = true;
                }

                if (oldStudent.FullName != student.FullName)
                {
                    oldStudent.FullName = student.FullName;
                    infochange = true;
                }

                if (_studentService.Save(oldStudent) != null)
                {
                    if (infochange)
                    {
                        var acc = _accountService.GetAccountByEmail(oldStudent.Email);
                        if (acc != null)
                        {
                            acc.Name = oldStudent.FullName;
                            acc.Phone = oldStudent.Phone;
                            _accountService.Save(acc);
                        }
                        if (newClasses.Count > 0)
                        {
                            foreach (var clid in newClasses)
                            {
                                var @class = _classService.GetItemByID(clid);
                                if (@class != null && !string.IsNullOrEmpty(@class.ID))
                                    _ = _mailHelper.SendStudentJoinClassNotify(student.FullName, student.Email, "", @class.Name, @class.StartDate, @class.EndDate, center.Name);
                            }

                        }

                        //check teacher account
                        //var tc = _teacherService.GetItemByEmail(oldStudent.Email);
                        //if (tc != null)
                        //{
                        //    tc.FullName = oldStudent.FullName;
                        //    tc.Phone = oldStudent.Phone;
                        //    tc.DateBorn = oldStudent.DateBorn;
                        //    _teacherService.Save(tc);
                        //}
                    }
                    Status = true;

                }
                Dictionary<string, object> response = new Dictionary<string, object>()
                    {
                        {"Data",student },
                        {"Error",null },
                        {"Msg","Cập nhật thành công" },
                        {"Status",Status }
                    };
                return new JsonResult(response);
            }
        }

        private bool ExistEmail(string email)
        {
            return _studentService.CreateQuery().CountDocuments(o => o.Email == email) > 0;
        }

        public async Task<JsonResult> RemoveStudent(string StudentID, string basis, string JoinedClasses = null, string ClassID = null)
        {
            var Error = "";
            var Status = false;
            try
            {
                //if (string.IsNullOrEmpty(ClassID) || string.IsNullOrEmpty(StudentID))
                if (string.IsNullOrEmpty(StudentID))
                {
                    return Json(new
                    {
                        Status = false,
                        error = "Thông tin không chính xác"
                    });
                }
                //var deleted = _classStudentService.RemoveClassStudent(ClassID, StudentID);
                var student = _studentService.GetItemByID(StudentID);
                var center = _centerService.GetItemByCode(basis);
                var classes = new List<ClassEntity>();
                if (student.JoinedClasses != null)
                {
                    classes = _classService.GetItemsByIDs(student.JoinedClasses).Where(t => t.Center == center.ID).ToList();//SELECT CENTER's CLASSES ONLY
                    if (classes != null)
                        foreach (var @class in classes)
                        {
                            if (_studentService.LeaveClass(@class.ID, StudentID) > 0)
                            {
                                //remove history, exam, exam detail, progress...
                                _ = _progressHelper.RemoveClassStudentHistory(@class.ID, StudentID);
                                _ = _examService.RemoveClassStudentExam(@class.ID, StudentID);
                            }
                        }
                    student = _studentService.GetItemByID(StudentID);
                }
                else
                    student.JoinedClasses = new List<string>();
                if (student.Centers != null)
                {
                    student.Centers.Remove(center.ID);
                    if (student.Centers.Count == 0)
                        student.IsActive = false;
                    _studentService.Save(student);
                    Status = true;
                }
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }
            var DataResponse = new Dictionary<string, object>
            {
                { "msg", "Đã xóa học viên" },
                { "error", Error},
                { "Status", Status }
            };
            return Json(DataResponse);
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
            if (student.JoinedClasses == null)
            {
                student.JoinedClasses = new List<string> { };
                _studentService.Save(student);//init JoinedClass;
            }
            if (student.Centers == null)
            {
                student.Centers = new List<string> { };
                _studentService.Save(student);//init Center;
            }
            if (_studentService.IsStudentInClass(ClassID, student.ID))
            {
                return Json(new { data = @class, msg = "Học viên đã có trong lớp" });
            }
            if (_studentService.JoinClass(ClassID, StudentID, @class.Center) > 0)
                return Json(new { data = @class, msg = "Học viên đã được thêm vào lớp" });
            return Json(new { error = "Có lỗi, vui lòng thực hiện lại" });
        }

        public JsonResult GetList(DefaultModel model, string Center, string SubjectID, string ClassID, string TeacherID, string SkillID, string GradeID, string Sort = "ASC")
        {
            var filterCs = new List<FilterDefinition<ClassSubjectEntity>>();
            if (!HasRole(User.Claims.GetClaimByType("UserID").Value, Center, "head-teacher"))
            {
                TeacherID = User.Claims.GetClaimByType("UserID").Value;
            }
            var classids = new List<string>();
            var checkClass = false;
            if (!string.IsNullOrEmpty(ClassID))
            {
                checkClass = true;
                classids.Add(ClassID);
            }
            else
            {
                if (!string.IsNullOrEmpty(SubjectID))
                { filterCs.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.SubjectID == SubjectID)); checkClass = true; }

                if (!string.IsNullOrEmpty(TeacherID))
                {
                    filterCs.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.TeacherID == TeacherID));
                    checkClass = true;
                }
                if (!string.IsNullOrEmpty(SkillID))
                {
                    filterCs.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.SkillID == SkillID));
                    checkClass = true;
                }
                if (!string.IsNullOrEmpty(GradeID))
                {

                    filterCs.Add(Builders<ClassSubjectEntity>.Filter.Where(o => o.GradeID == GradeID));
                    checkClass = true;
                }
                classids =
                    filterCs.Count > 0 ? _classSubjectService.Collection.Distinct(t => t.ClassID, Builders<ClassSubjectEntity>.Filter.And(filterCs)).ToList()
                : _classService.Collection.Find(t => true).Project(t => t.ID).ToList();
            }

            if (checkClass && classids == null || classids.Count() == 0)
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Model", model }
                });

            var stfilter = new List<FilterDefinition<StudentEntity>>();

            if (!string.IsNullOrEmpty(Center))
            {
                var @center = _centerService.GetItemByCode(Center);
                if (@center == null) return new JsonResult(new Dictionary<string, object>
                    {
                        { "Error", "Cơ sở không đúng"}
                    });
                stfilter.Add(Builders<StudentEntity>.Filter.Where(o => o.Centers.Contains(@center.ID)));
            }

            if (checkClass)
                stfilter.Add(Builders<StudentEntity>.Filter.AnyIn(t => t.JoinedClasses, classids));
            if (!string.IsNullOrEmpty(model.SearchText))

                stfilter.Add(Builders<StudentEntity>.Filter.And(
                        Builders<StudentEntity>.Filter.Text("\"" + model.SearchText + "\"")));
            var list = _studentService.Collection.Find(Builders<StudentEntity>.Filter.And(stfilter));

            if (Sort == "ASC")
                list = list.SortBy(t => t.ID);
            else
                list = list.SortByDescending(t => t.ID);
            //var list = _studentService.GetAll().SortByDescending(t => t.ID);

            model.TotalRecord = list.CountDocuments();

            var _mapping = new MappingEntity<StudentEntity, ClassStudentViewModel>();

            var retStudents = list.Skip(model.PageIndex * model.PageSize).Limit(model.PageSize).ToList()
                .Select(t => _mapping.AutoOrtherType(t, new ClassStudentViewModel()
                {
                    ClassID = ClassID,
                    ClassName = string.IsNullOrEmpty(ClassID) ?
                       (t.JoinedClasses == null ? "" : string.Join("; ", _classService.GetMultipleClassName(t.JoinedClasses, t.ID))) :
                        _classService.GetItemByID(ClassID).Name,
                }));

            var response = new Dictionary<string, object>
            {
                { "Data", retStudents },
                { "Model", model },
            };
            return new JsonResult(response);
        }

        public JsonResult Search(string term, string Center)
        {
            var stfilter = new List<FilterDefinition<StudentEntity>>();
            if (!string.IsNullOrEmpty(Center))
            {
                var @center = _centerService.GetItemByCode(Center);
                if (@center == null) return new JsonResult(new Dictionary<string, object>
                    {
                        { "Error", "Cơ sở không đúng"}
                    });
                stfilter.Add(Builders<StudentEntity>.Filter.Where(o => o.Centers.Contains(@center.ID)));
            }
            if (!string.IsNullOrEmpty(term))
                stfilter.Add(Builders<StudentEntity>.Filter.Text("\"" + term + "\""));
            return Json(_studentService.CreateQuery().Find(Builders<StudentEntity>.Filter.And(stfilter)).Limit(100).ToEnumerable());
            //return Json(_studentService.Search(term, 100));
        }

        public async Task<JsonResult> ChangePass(string basis, string ID, string Password)
        {

            if (string.IsNullOrEmpty(basis))
                return Json(new { error = "Bạn không được quyền thực hiện chức năng này" });

            var center = _centerService.GetItemByCode(basis);
            if (center == null)
                return Json(new { error = "Bạn không được quyền thực hiện chức năng này" });

            var currentUser = User.Claims.GetClaimByType("UserID").Value;
            var currentTeacher = _teacherService.GetItemByID(currentUser);

            //if (!(currentTeacher.Centers.Any(t => t.CenterID == center.ID && _teacherHelper.HasRole(currentUser, center.ID, "head-teacher"))))
            //{
            //    return Json(new { error = "Bạn không được quyền thực hiện chức năng này" });
            //}

            if (string.IsNullOrEmpty(ID))
                return Json(new { error = "Thông tin không chính xác" });

            var updateStudent = _studentService.GetItemByID(ID);
            if (updateStudent == null)
                return Json(new { error = "Thông tin không chính xác" });

            if (String.IsNullOrEmpty(Password) || Password.Length < 6)
                return Json(new { error = "Độ dài mật khẩu tối thiểu 6 ký tự" });

            var acc = _accountService.GetAccountByEmail(updateStudent.Email);
            if (acc == null)
                return Json(new { error = "Thông tin không chính xác" });
            acc.PassWord = Core_v2.Globals.Security.Encrypt(Password);
            acc.PassTemp = acc.PassWord;
            _accountService.Save(acc);
            _ = _mailHelper.SendPasswordChangeNotify(acc, Password);
            return Json(new { msg = "Đã đổi mật khẩu" });
        }

        #region Batch Import
        [HttpPost]
        [Obsolete]
        public async Task<JsonResult> ImportStudent(string basis, string ClassID)
        {
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(UserID))
                return null;
            var form = HttpContext.Request.Form;
            //if (string.IsNullOrEmpty(ClassID))
            //    return Json(new { error = "Không có thông tin lớp" });
            var @class = new ClassEntity();
            if (!string.IsNullOrEmpty(ClassID))
            {
                @class = _classService.GetItemByID(ClassID);
                if (@class == null)
                    return Json(new { error = "Thông tin lớp không đúng" });
            }
            var center = new CenterEntity();
            long left = 0;
            if (!string.IsNullOrEmpty(basis))
            {
                center = _centerService.GetItemByCode(basis);
                if (center == null)
                    return null;
                var totalStudent = _studentService.CountByCenter(center.ID);
                if (center.Limit > 0)
                    left = center.Limit - totalStudent;
                else
                    left = long.MaxValue;
            }

            if (center == null || left <= 0)
            {
                return Json(new { error = "Cơ sở " + center.Name + " đã hết hạn mức." });
            }

            var abbr = "st." + (string.IsNullOrEmpty(center.Abbr) ? "eduso" : center.Abbr);

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
                            var classStudents = string.IsNullOrEmpty(ClassID) ? new List<string>() : _studentService.GetStudentIdsByClassId(ClassID).ToList();
                            var keyCol = 4;
                            for (int i = 1; i <= totalRows; i++)
                            {
                                var isValidMail = true;
                                if (left <= 0) continue;
                                if (workSheet.Cells[i, 1].Value == null || workSheet.Cells[i, 1].Value.ToString() == "STT") continue;
                                var email = (workSheet.Cells[i, keyCol].Value == null ? "" : workSheet.Cells[i, keyCol].Value.ToString()).ToLower().Trim();
                                if (string.IsNullOrEmpty(email))
                                //    continue; skip => create auto mail
                                {
                                    isValidMail = false;
                                    email = abbr + "." + _indexService.GetNewIndex(abbr) + "@eduso.vn";
                                }

                                string name = workSheet.Cells[i, 2].Value == null ? "" : workSheet.Cells[i, 2].Value.ToString();
                                string dateStr = workSheet.Cells[i, 3].Value == null ? "" : workSheet.Cells[i, 3].Value.ToString();
                                var birthdate = new DateTime();
                                DateTime.TryParseExact(dateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                                                   DateTimeStyles.None,
                                                   out birthdate);
                                var phone = workSheet.Cells[i, 5].Value == null ? "" : workSheet.Cells[i, 5].Value.ToString();

                                var student = _studentService.GetStudentByEmail(email);
                                var visiblePass = "";
                                if (student == null)
                                {
                                    //create student
                                    var acc = _accountService.GetAccountByEmail(email);
                                    student = new StudentEntity
                                    {
                                        //StudentId = code,
                                        FullName = name,
                                        DateBorn = birthdate,
                                        Email = email,
                                        Phone = phone,
                                        //Skype = skype,
                                        CreateDate = DateTime.Now,
                                        UserCreate = UserID,
                                        IsActive = true,
                                        Centers = new List<string> { center.ID },
                                        JoinedClasses = new List<string> { }
                                    };
                                    if (@class != null && !string.IsNullOrEmpty(@class.ID))
                                        student.JoinedClasses.Add(@class.ID);
                                    if (acc == null)
                                    {
                                        await _studentService.CreateQuery().InsertOneAsync(student);
                                        left--;
                                        //counter++;
                                        visiblePass = _defaultPass;
                                        var account = new AccountEntity()
                                        {
                                            CreateDate = DateTime.Now,
                                            IsActive = true,
                                            PassTemp = Core_v2.Globals.Security.Encrypt(_defaultPass),
                                            PassWord = Core_v2.Globals.Security.Encrypt(_defaultPass),
                                            UserCreate = student.UserCreate,
                                            Type = ACCOUNT_TYPE.STUDENT,
                                            UserID = student.ID,
                                            UserName = student.Email.ToLower().Trim(),
                                            RoleID = _roleService.GetItemByCode("student").ID
                                        };
                                        _accountService.CreateQuery().InsertOne(account);
                                        if (isValidMail)
                                            if (@class != null && !string.IsNullOrEmpty(@class.ID))
                                                _ = _mailHelper.SendStudentJoinClassNotify(student.FullName, student.Email, visiblePass, @class.Name, @class.StartDate, @class.EndDate, center.Name);
                                            else
                                                _ = _mailHelper.SendStudentJoinCenterNotify(student.FullName, student.Email, visiblePass, center.Name);
                                    }
                                    else
                                    {
                                        if (acc.Type != ACCOUNT_TYPE.STUDENT)
                                        {
                                            await _studentService.CreateQuery().InsertOneAsync(student);
                                            if (@class != null && !string.IsNullOrEmpty(@class.ID))
                                                _ = _mailHelper.SendStudentJoinClassNotify(student.FullName, student.Email, visiblePass, @class.Name, @class.StartDate, @class.EndDate, center.Name);
                                            else
                                                _ = _mailHelper.SendStudentJoinCenterNotify(student.FullName, student.Email, visiblePass, center.Name);
                                            //counter++;
                                        }
                                        else //acc = student & student not found ????
                                        {
                                            continue;
                                            //unknown Err;                                            
                                        }
                                    }
                                }
                                else
                                {
                                    if (student.Centers == null) student.Centers = new List<string> { };
                                    if (student.JoinedClasses == null) student.JoinedClasses = new List<string> { };

                                    if (student.Centers.Contains(center.ID) && (string.IsNullOrEmpty(ClassID) || classStudents.Any(t => t == student.ID))) continue; //student in class

                                    if (!student.Centers.Contains(center.ID))
                                        student.Centers.Add(center.ID);

                                    _studentService.Save(student);
                                    if (!string.IsNullOrEmpty(ClassID))
                                    {
                                        if (classStudents.Any(t => t == student.ID)) continue;
                                        _studentService.JoinClass(ClassID, student.ID, @class.Center);
                                        _ = _mailHelper.SendStudentJoinClassNotify(student.FullName, student.Email, visiblePass, @class.Name, @class.StartDate, @class.EndDate, center.Name);
                                    }
                                    else
                                    {
                                        _ = _mailHelper.SendStudentJoinCenterNotify(student.FullName, student.Email, "", center.Name);
                                    }
                                }
                                counter++;
                            }
                        }
                    }
                    System.IO.File.Delete(filePath);
                    var message = "Đã thêm mới " + counter + " học viên!";
                    if (center.Limit > 0)
                        message += " Hạn mức hiện tại còn " + left + " tài khoản trống!";
                    return Json(new { msg = message });
                }
                catch (Exception ex)
                {
                    return Json(new { error = ex.Message });
                }
            }
        }

        public IActionResult ExportTemplate(DefaultModel model)
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
                SDT = "0123456789"
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

        public IActionResult ExportStudent(string basic, string ClassID)
        {
            var center = _centerService.GetItemByCode(basic);
            var Data4Export = ClassID == null ? _studentService.CreateQuery().Find(x => x.Centers.Contains(center.ID)) : _studentService.CreateQuery().Find(x => x.Centers.Contains(center.ID) && x.JoinedClasses.Contains(ClassID));
            var @class = ClassID == null ? "" : _classService.GetItemByID(ClassID).Name;
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            var teacher = _teacherService.CreateQuery().Find(t => t.ID == UserID).SingleOrDefault();
            var stream = new MemoryStream();

            //xuat file excel
            try
            {

                using (ExcelPackage p = new ExcelPackage(stream))
                {
                    // đặt tên người tạo file
                    p.Workbook.Properties.Author = teacher.FullName;

                    // đặt tiêu đề cho file
                    p.Workbook.Properties.Title = "Danh sách học viên";

                    //Tạo một sheet để làm việc trên đó
                    p.Workbook.Worksheets.Add($"Lớp {@class}");

                    // lấy sheet vừa add ra để thao tác
                    ExcelWorksheet ws = p.Workbook.Worksheets[1];

                    // đặt tên cho sheet
                    ws.Name = $"DSHV {@class}";
                    // fontsize mặc định cho cả sheet
                    ws.Cells.Style.Font.Size = 11;
                    // font family mặc định cho cả sheet
                    ws.Cells.Style.Font.Name = "Calibri";

                    // Tạo danh sách các column header
                    string[] arrColumnHeader = new string[] { };
                    //if (ClassID != null)
                    //{
                    arrColumnHeader = new string[]{
                        "STT",
                        "Họ tên",
                        "Ngày sinh",
                        "Email",
                        "Số điện thoại"
                    };
                    //}
                    //else
                    //{
                    //    arrColumnHeader = new string[]{
                    //        "STT",
                    //    "Họ tên",
                    //    "Lớp",
                    //    "Email",
                    //    "Số điện thoại",
                    //    "Năm sinh"
                    //};
                    //}

                    // lấy ra số lượng cột cần dùng dựa vào số lượng header
                    var countColHeader = arrColumnHeader.Count();

                    // merge các column lại từ column 1 đến số column header
                    // gán giá trị cho cell vừa merge là Thống kê thông tni User Kteam
                    ws.Cells[1, 1].Value = ClassID == null ? $"Thống kê danh sách học viên {center.Name}" : $"Thống kê danh sách học viên lớp {@class}";
                    ws.Cells[1, 1, 1, countColHeader].Merge = true;
                    // in đậm
                    ws.Cells[1, 1, 1, countColHeader].Style.Font.Bold = true;
                    // căn giữa
                    ws.Cells[1, 1, 1, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    int colIndex = 1;
                    int rowIndex = 2;

                    //tạo các header từ column header đã tạo từ bên trên
                    foreach (var item in arrColumnHeader)
                    {
                        var cell = ws.Cells[rowIndex, colIndex];

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

                        colIndex++;
                    }

                    // lấy ra danh sách UserInfo từ ItemSource của DataGrid
                    //List<UserInfo> userList = dtgExcel.ItemsSource.Cast<UserInfo>().ToList();

                    // với mỗi item trong danh sách sẽ ghi trên 1 dòng
                    int index = 1;
                    foreach (var item in Data4Export.ToList())
                    {
                        // bắt đầu ghi từ cột 1. Excel bắt đầu từ 1 không phải từ 0
                        colIndex = 1;

                        // rowIndex tương ứng từng dòng dữ liệu
                        rowIndex++;

                        //gán giá trị cho từng cell                      
                        ws.Cells[rowIndex, colIndex++].Value = index;
                        ws.Cells[rowIndex, colIndex++].Value = item.FullName;

                        // lưu ý phải .ToShortDateString để dữ liệu khi in ra Excel là ngày như ta vẫn thấy.Nếu không sẽ ra tổng số :v
                        ws.Cells[rowIndex, colIndex++].Value = (item.DateBorn > new DateTime(1900, 1, 1)) ? item.DateBorn.ToLocalTime().ToString("dd/MM/yyyy") : "";

                        //if (ClassID != null)
                        //{
                        //    ws.Cells[rowIndex, colIndex++].Value = _classService.CreateQuery().Find(x=>item.JoinedClasses.Contains(x.ID)).FirstOrDefault().Name;
                        //}
                        ws.Cells[rowIndex, colIndex++].Value = item.Email;
                        ws.Cells[rowIndex, colIndex++].Value = item.Phone;

                        index++;
                    }
                    var cells = ws.Cells[1, countColHeader, (int)Data4Export.CountDocuments() + 2, countColHeader];
                    cells.AutoFitColumns();
                    cells.Style.Border.Top.Style =
                    cells.Style.Border.Left.Style =
                    cells.Style.Border.Right.Style =
                    cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    //Lưu file lại
                    //Byte[] bin = p.GetAsByteArray();
                    p.Save();
                }
                stream.Position = 0;
            }
            catch (Exception EE)
            {
            }

            //using (var package = new ExcelPackage(stream))
            //{
            //    var workSheet = package.Workbook.Worksheets.Add("DS_HV");
            //    workSheet.Cells.LoadFromCollection(Data4Export, true);
            //    package.Save();
            //}
            string excelName = ClassID == null ? $"Danh sách học viên {center.Name}.xlsx" : $"Danh sách học viên lớp {@class}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
        #endregion

        private bool HasRole(string userid, string center, string role)
        {
            var teacher = _teacherService.GetItemByID(userid);
            if (teacher == null) return false;
            var centerMember = teacher.Centers.Find(t => t.Code == center);
            if (centerMember == null) return false;
            if (_roleService.GetItemByID(centerMember.RoleID).Code != role) return false;
            return true;
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetBestStudents(string basis)
        {
            var center = _centerService.GetItemByCode(basis);
            if (center == null)
                return Json(new { Err = "Không có dữ liệu" });

            var list = new List<StudentRankingViewModel>();

            var classIDs = _classService.CreateQuery().Find(t => t.Center == center.ID).Project(t => t.ID).ToEnumerable();
            var results = _classProgressService.CreateQuery().Aggregate().Match(t => classIDs.Contains(t.ClassID)).Group(t => t.StudentID, g => new StudentRankingViewModel
            {
                StudentID = g.Key,
                TotalPoint = g.Sum(t => t.TotalPoint),
                PracticePoint = g.Sum(t => t.PracticePoint),
            }).SortByDescending(s => s.TotalPoint).ThenByDescending(s => s.PracticePoint).Limit(20).ToEnumerable();

            var rtn = new List<StudentRankingViewModel>();
            foreach (var result in results)
            {
                var st = _studentService.GetItemByID(result.StudentID);
                if (st != null)
                {
                    result.StudentName = st.FullName;
                    rtn.Add(result);
                }
            }

            var response = new Dictionary<string, object>
            {
                { "Data", rtn }
            };
            return new JsonResult(response);
        }


        //public async Task<JsonResult> ConvertStudent()
        //{
        //    var classes = _classService.GetAll().ToList();
        //    var count = 0;
        //    var str = "";
        //    _studentService.Collection.UpdateMany(t => true, Builders<StudentEntity>.Update.Unset(t => t.JoinedClasses));
        //    foreach (var @class in classes)
        //    {
        //        await _studentService.LeaveClassAll(@class.ID);
        //        //if (@class.ID != "5e652e05fd6d8e01304cd67c") continue;
        //        //_classStudentService.RemoveClass(@class.ID);
        //        var students = _classStudentService.GetClassStudents(@class.ID);

        //        foreach (var student in students)
        //        {
        //            if (_studentService.JoinClass(@class.ID, student.StudentID) > 0)
        //                count++;
        //            else
        //            {
        //                str += student.ID + "<br/>";
        //            }
        //        }
        //    }
        //    return new JsonResult("OK " + str);
        //}
    }
}
