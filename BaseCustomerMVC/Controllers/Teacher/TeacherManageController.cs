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
using Microsoft.Extensions.Configuration;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class TeacherManageController : TeacherController
    {
        private readonly GradeService _gradeService;
        private readonly AccountService _accountService;
        private readonly RoleService _roleService;
        private readonly SubjectService _subjectService;
        private readonly TeacherService _teacherService;
        private readonly SkillService _skillService;
        private readonly ClassService _classService;
        //private readonly ClassStudentService _classStudentService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly StudentService _studentService;
        private readonly StudentHelper _studentHelper;
        private readonly ClassProgressService _classProgressService;
        private readonly ClassSubjectProgressService _classSubjectProgressService;
        private readonly ScoreStudentService _scoreStudentService;
        private readonly LearningHistoryService _learningHistoryService;
        private readonly ExamService _examService;
        private readonly CenterService _centerService;
        private readonly TeacherHelper _teacherHelper;
        private readonly string _defaultPass;
        private readonly MailHelper _mailHelper;
        private readonly IHostingEnvironment _env;
        private IConfiguration _configuration;


        public TeacherManageController(
            AccountService accountService,
            RoleService roleService,
            GradeService gradeservice,
            SubjectService subjectService,
            TeacherService teacherService,
            ClassService classService,
            SkillService skillService,
            ExamService examService,
            //ClassStudentService classStudentService,
            ClassSubjectService classSubjectService,
            LearningHistoryService learningHistoryService,
            ClassProgressService classProgressService,
            ClassSubjectProgressService classSubjectProgressService,
            ScoreStudentService scoreStudentService,
            StudentService studentService,
            CenterService centerService,
            TeacherHelper teacherHelper,
            MailHelper mailHelper,
            IHostingEnvironment evn
            , IConfiguration iConfig
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
            //_classStudentService = classStudentService;
            _classSubjectService = classSubjectService;
            _studentService = studentService;
            _teacherHelper = teacherHelper;
            _centerService = centerService;
            _mailHelper = mailHelper;
            _env = evn;
            _configuration = iConfig;
            _defaultPass = _configuration.GetValue<string>("SysConfig:DP");
            _studentHelper = new StudentHelper(studentService, accountService);
        }

        public IActionResult Index(DefaultModel model, string basis)
        {
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            if (!string.IsNullOrEmpty(basis))
            {
                var center = _centerService.GetItemByCode(basis);
                if (center != null)
                    ViewBag.Center = center;
                ViewBag.IsHeadTeacher = _teacherHelper.HasRole(UserID, center.ID, "head-teacher");
            }

            var teacher = _teacherService.CreateQuery().Find(t => t.ID == UserID).SingleOrDefault();

            var subject = new List<SubjectEntity>();
            var grade = new List<GradeEntity>();
            //if (teacher != null && teacher.Subjects != null)
            //{
            //    subject = _subjectService.CreateQuery().Find(t => teacher.Subjects.Contains(t.ID)).ToList();
            //    grade = _gradeService.CreateQuery().Find(t => teacher.Subjects.Contains(t.SubjectID)).ToList();
            //}

            ViewBag.Roles = _roleService.CreateQuery().Find(r => r.Type == "teacher").ToList();
            //ViewBag.Grade = grade;
            ViewBag.Subject = subject;
            //ViewBag.Skills = _skillService.GetList();

            ViewBag.User = UserID;
            ViewBag.Model = model;
            //ViewBag.Managable = CheckPermission(PERMISSION.MEMBER_COURSE_EDIT);
            return View();
        }


        public JsonResult GetList(DefaultModel model, string basis)//, string ClassID, string SubjectID
        {
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            var filterTC = new List<FilterDefinition<TeacherEntity>>();
            if (!string.IsNullOrEmpty(basis))
            {
                var center = _centerService.GetItemByCode(basis);
                if (center != null)
                    ViewBag.Center = center;
                ViewBag.IsHeadTeacher = _teacherHelper.HasRole(UserID, center.ID, "head-teacher");
                filterTC.Add(Builders<TeacherEntity>.Filter.Where(o => o.Centers.Any(t => t.CenterID == center.ID)));
            }

            //if (!string.IsNullOrEmpty(SubjectID))
            //    filterTC.Add(Builders<TeacherEntity>.Filter.Where(o => o.Subjects.Contains(SubjectID)));

            //if (!string.IsNullOrEmpty(ClassID))
            //{
            //    var _class = _classService.GetItemByID(ClassID);
            //    if (_class == null) return new JsonResult(new Dictionary<string, object>
            //    {
            //        { "Model", model }
            //    });
            //    var listid = _class.Members.Select(t => t.TeacherID).Distinct().ToList();

            //    filterTC.Add(Builders<TeacherEntity>.Filter.Where(o => listid.Contains(o.ID)));
            //}

            if (!string.IsNullOrEmpty(model.SearchText))
                filterTC.Add(Builders<TeacherEntity>.Filter.Text("\"" + model.SearchText + "\""));
            var list = _teacherService.Collection.Find(Builders<TeacherEntity>.Filter.And(filterTC)).SortByDescending(t => t.ID);

            model.TotalRecord = list.CountDocuments();

            var roleList = new List<string> { "teacher", "head-teacher" };
            var roles = _roleService.CreateQuery().Find(o => roleList.Contains(o.Code)).ToList();
            var _mapping = new MappingEntity<TeacherEntity, TeacherViewModel>();

            var data =

                from t in list.Skip(model.PageIndex * model.PageSize).Limit(model.PageSize).ToList()
                    //let account = _accountService.CreateQuery().Find(o => o.UserID == t.ID && o.Type == ACCOUNT_TYPE.TEACHER).FirstOrDefault()
                    //where account != null
                let role = roles.Find(r => r.ID == t.Centers.Single(c => c.Code == basis).RoleID)
                where role != null
                select _mapping.AutoOrtherType(t, new TeacherViewModel()
                {
                    SubjectList = t.Subjects == null ? null : _subjectService.CreateQuery().Find(o => t.Subjects.Contains(o.ID)).ToList(),
                    RoleID = role.ID,
                    RoleName = role.Name,
                    //AccountID = account.ID
                });

            //var _mapping = new MappingEntity<StudentEntity, ClassStudentViewModel>();

            //var retStudents = list.Skip(model.PageIndex * model.PageSize).Limit(model.PageSize).ToList()
            //    .Select(t => _mapping.AutoOrtherType(t, new ClassStudentViewModel()
            //    {
            //        ClassID = ClassID,
            //        ClassName = string.IsNullOrEmpty(ClassID) ?
            //            string.Join("; ", _classService.GetMultipleClassName(t.JoinedClasses)) :
            //            _classService.GetItemByID(ClassID).Name
            //    }));

            var response = new Dictionary<string, object>
            {
                { "Data", data },
                { "Model", model }
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

        public JsonResult AddMember(string basis, TeacherEntity tc, string RoleID)
        {
            if (string.IsNullOrEmpty(basis))
                return Json(new { error = "Cơ sở không đúng" });

            var center = _centerService.GetItemByCode(basis);
            if (center == null)
                return Json(new { error = "Cơ sở không đúng" });

            var currentUser = User.Claims.GetClaimByType("UserID").Value;
            var currentTeacher = _teacherService.GetItemByID(currentUser);

            if (!(currentTeacher.Centers.Any(t => t.CenterID == center.ID && _teacherHelper.HasRole(currentUser, center.ID, "head-teacher"))))
            {
                return Json(new { error = "Bạn không được quyền thực hiện chức năng này" });
            }
            tc.Email = tc.Email.ToLower().Trim();
            if (string.IsNullOrEmpty(tc.Email))
                return Json(new { error = "Thông tin giáo viên không đúng" });
            if (tc.ID != null)
            {
                var oldobj = _teacherService.GetItemByID(tc.ID);
                if (oldobj == null)
                    return Json(new { error = "Thông tin giáo viên không đúng" });
                oldobj.FullName = tc.FullName;
                oldobj.Phone = tc.Phone;
                var exist = oldobj.Centers.RemoveAll(t => t.CenterID == center.ID) > 0;
                oldobj.Centers.Add(new CenterMemberEntity
                {
                    CenterID = center.ID,
                    Code = center.Code,
                    Name = center.Name,
                    RoleID = RoleID
                });
                _teacherService.Save(oldobj);
                if (!exist)
                    _ = _mailHelper.SendTeacherJoinCenterNotify(tc.FullName, tc.Email, "", center.Name);
            }
            else
            {
                if (ExistEmail(tc.Email))
                    return Json(new { error = "Email đã được sử dụng" });

                var teacher = new TeacherEntity
                {
                    CreateDate = DateTime.Now,
                    Email = tc.Email,
                    FullName = tc.FullName,
                    Centers = new List<CenterMemberEntity>
                    {
                        new CenterMemberEntity
                        {
                            CenterID = center.ID,
                            Code = center.Code,
                            Name = center.Name,
                            RoleID = RoleID
                        }
                    },
                    Phone = tc.Phone,
                    IsActive = true,
                    Subjects = currentTeacher.Subjects,//copy creator subjects
                    UserCreate = currentUser
                };
                _teacherService.CreateQuery().InsertOne(teacher);
                var account = new AccountEntity()
                {
                    CreateDate = DateTime.Now,
                    IsActive = true,
                    PassTemp = Core_v2.Globals.Security.Encrypt(_defaultPass),
                    PassWord = Core_v2.Globals.Security.Encrypt(_defaultPass),
                    UserCreate = teacher.UserCreate,
                    Type = ACCOUNT_TYPE.TEACHER,
                    UserID = teacher.ID,
                    UserName = teacher.Email,
                    Name = teacher.FullName,
                    RoleID = teacher.ID
                };
                _accountService.CreateQuery().InsertOne(account);
                _ = _mailHelper.SendTeacherJoinCenterNotify(teacher.FullName, teacher.Email, _defaultPass, center.Name);
            }
            return Json(new { msg = "Giáo viên đã được cập nhật" });
        }

        public async Task<JsonResult> RemoveMember(string basis, string ID)
        {

            if (string.IsNullOrEmpty(basis))
                return Json(new { error = "Cơ sở không đúng" });

            var center = _centerService.GetItemByCode(basis);
            if (center == null)
                return Json(new { error = "Cơ sở không đúng" });

            var currentUser = User.Claims.GetClaimByType("UserID").Value;
            var currentTeacher = _teacherService.GetItemByID(currentUser);

            if (!(currentTeacher.Centers.Any(t => t.CenterID == center.ID && _teacherHelper.HasRole(currentUser, center.ID, "head-teacher"))))
            {
                return Json(new { error = "Bạn không được quyền thực hiện chức năng này" });
            }

            if (string.IsNullOrEmpty(ID))
                return Json(new { error = "Thông tin không chính xác" });

            var removeTeacher = _teacherService.GetItemByID(ID);
            if (removeTeacher == null)
                return Json(new { error = "Thông tin không chính xác" });


            if (removeTeacher.Centers.Any(t => t.CenterID == center.ID))
            {
                removeTeacher.Centers.RemoveAll(t => t.CenterID == center.ID);
                if (removeTeacher.Centers.Count == 0)//no center left
                    removeTeacher.IsActive = false; //deactive teacher
                _teacherService.Save(removeTeacher);
                return Json(new { msg = "Đã xóa giáo viên" });
            }
            return Json(new { msg = "Giáo viên không tồn tại hoặc đã bị xóa" });
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

            if (!(currentTeacher.Centers.Any(t => t.CenterID == center.ID && _teacherHelper.HasRole(currentUser, center.ID, "head-teacher"))))
            {
                return Json(new { error = "Bạn không được quyền thực hiện chức năng này" });
            }

            if (string.IsNullOrEmpty(ID))
                return Json(new { error = "Thông tin không chính xác" });

            var updateTeacher = _teacherService.GetItemByID(ID);
            if (updateTeacher == null)
                return Json(new { error = "Thông tin không chính xác" });

            if (String.IsNullOrEmpty(Password) || Password.Length < 6)
                return Json(new { error = "Độ dài mật khẩu tối thiểu 6 ký tự" });

            var acc = _accountService.GetAccountByEmail(updateTeacher.Email);
            if (acc == null)
                return Json(new { error = "Thông tin không chính xác" });
            acc.PassWord = Core_v2.Globals.Security.Encrypt(Password);
            _accountService.Save(acc);
            return Json(new { msg = "Đã đổi mật khẩu" });
        }

        #region Batch Import
        [HttpPost]
        public async Task<JsonResult> ImportMember(string basis)
        {
            var center = _centerService.GetItemByCode(basis);
            if (center == null)
                return Json(new { error = "Bạn không được quyền thực hiện chức năng này" });

            var currentUser = User.Claims.GetClaimByType("UserID").Value;
            var currentTeacher = _teacherService.GetItemByID(currentUser);

            if (!(currentTeacher.Centers.Any(t => t.CenterID == center.ID && _teacherHelper.HasRole(currentUser, center.ID, "head-teacher"))))
            {
                return Json(new { error = "Bạn không được quyền thực hiện chức năng này" });
            }

            var roleList = new List<string> { "teacher", "head-teacher" };
            var roles = _roleService.CreateQuery().Find(o => roleList.Contains(o.Code)).ToList();

            var form = HttpContext.Request.Form;

            if (form == null) return new JsonResult(null);
            if (form.Files == null || form.Files.Count <= 0) return new JsonResult(null);
            var file = form.Files[0];
            var dirPath = Path.Combine(_env.WebRootPath, "Temp");
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            var filePath = Path.Combine(dirPath, basis + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss"));
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
                            var keyCol = 3;
                            var headTeacherRole = roles.Single(t => t.Code == "head-teacher");
                            var teacherRole = roles.Single(t => t.Code == "teacher");
                            for (int i = 1; i <= totalRows; i++)
                            {
                                if (workSheet.Cells[i, 1].Value == null || workSheet.Cells[i, 1].Value.ToString() == "STT") continue;
                                var email = workSheet.Cells[i, keyCol].Value == null ? "" : workSheet.Cells[i, keyCol].Value.ToString().Trim().ToLower();
                                if (string.IsNullOrEmpty(email)) continue;


                                var name = workSheet.Cells[i, 2].Value.ToString();
                                var phone = workSheet.Cells[i, 4].Value != null ? workSheet.Cells[i, 4].Value.ToString().Trim() : "";
                                var role = (workSheet.Cells[i, 5].Value != null && workSheet.Cells[i, 5].Value.ToString() == "x") ? headTeacherRole.ID : teacherRole.ID;

                                var teacher = _teacherService.GetItemByEmail(email);

                                AccountEntity acc = null;
                                if (teacher != null)//add teacher to center
                                {
                                    teacher.FullName = name;
                                    teacher.Phone = phone;


                                    var exist = teacher.Centers.RemoveAll(t => t.CenterID == center.ID) > 0;
                                    teacher.Centers.Add(new CenterMemberEntity
                                    {
                                        CenterID = center.ID,
                                        Code = center.Code,
                                        Name = center.Name,
                                        RoleID = role
                                    });
                                    _teacherService.Save(teacher);
                                    acc = _accountService.GetAccountByEmail(teacher.Email);
                                    acc.Name = teacher.FullName;
                                    acc.Phone = teacher.Phone;
                                    _accountService.Save(acc);
                                    if (!exist)
                                        _ = _mailHelper.SendTeacherJoinCenterNotify(teacher.FullName, teacher.Email, "", center.Name);
                                }
                                else
                                {
                                    teacher = new TeacherEntity
                                    {
                                        CreateDate = DateTime.Now,
                                        Email = email,
                                        FullName = name,
                                        Centers = new List<CenterMemberEntity>
                                        {
                                            new CenterMemberEntity
                                            {
                                                CenterID = center.ID,
                                                Code = center.Code,
                                                Name = center.Name,
                                                RoleID = role
                                            }
                                        },
                                        Phone = phone,
                                        IsActive = true,
                                        Subjects = currentTeacher.Subjects,//copy creator subjects
                                        UserCreate = currentUser
                                    };
                                    _teacherService.CreateQuery().InsertOne(teacher);
                                    var account = new AccountEntity()
                                    {
                                        CreateDate = DateTime.Now,
                                        IsActive = true,
                                        PassTemp = Core_v2.Globals.Security.Encrypt(_defaultPass),
                                        PassWord = Core_v2.Globals.Security.Encrypt(_defaultPass),
                                        UserCreate = teacher.UserCreate,
                                        Type = ACCOUNT_TYPE.TEACHER,
                                        UserID = teacher.ID,
                                        UserName = teacher.Email,
                                        Name = teacher.FullName,
                                        RoleID = teacher.ID
                                    };
                                    _accountService.CreateQuery().InsertOne(account);
                                    _ = _mailHelper.SendTeacherJoinCenterNotify(teacher.FullName, teacher.Email, _defaultPass, center.Name);
                                }
                                counter++;
                            }
                        }
                    }
                    System.IO.File.Delete(filePath);
                    return Json(new { msg = "Đã thêm mới " + counter + " giáo viên" });
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
                Email = "email@gmail.com",
                SDT = "0123456789",
                Truong_bo_mon = "x",
            });
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("DS_GV");
                workSheet.Cells.LoadFromCollection(data, true);
                package.Save();
            }
            stream.Position = 0;
            string excelName = $"TeacherTemplate.xlsx";

            //return File(stream, "application/octet-stream", excelName);  
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
        #endregion

        private bool ExistEmail(string email)
        {
            var _currentData = _teacherService.CreateQuery().Find(o => o.Email == email);
            return _currentData.CountDocuments() > 0;
        }

        public async Task<JsonResult> FixAccName()
        {
            var accs = _accountService.GetAll().ToList();
            foreach (var acc in accs)
            {
                if (acc.Name == null)
                {
                    if (acc.Type == "teacher")
                    {
                        var tc = _teacherService.GetItemByID(acc.UserID);
                        if (tc != null)
                        {
                            acc.Name = tc.FullName;
                            _accountService.Save(acc);
                        }
                    }
                }
            }
            return Json("OK");
        }
    }
}
