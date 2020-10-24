using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using FileManagerCore.Interfaces;
using FileManagerCore.Services;
using GoogleLib.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace EmailTemplate.Controllers
{
    public class EmailController : Controller
    {
        private readonly ILogger<EmailController> _logger;
        private readonly MailHelper _mailHelper;
        private readonly ClassService _classService;
        private readonly CenterService _centerService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly LessonService _lessonService;
        private readonly LessonScheduleService _scheduleService;
        private readonly AccountService _accountService;
        private readonly StudentService _studentService;
        private readonly TeacherService _teacherService;
        private readonly SkillService _skillService;
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly RoleService _roleService;
        private readonly CourseService _courseService;
        private readonly LearningHistoryService _learningHistory;
        private readonly LessonProgressService _lessonProgressService;
        public EmailController(ILogger<EmailController> logger, MailHelper mailHelper,
            ClassService classService,
            CenterService centerService,
            ClassSubjectService classSubjectService,
            LessonService lessonService,
            LessonScheduleService scheduleService,
            AccountService accountService,
            StudentService studentService,
            TeacherService teacherService,
            SkillService skillService,
            LessonScheduleService lessonScheduleService,
            RoleService roleService,
            CourseService courseService,
            LearningHistoryService learningHistory,
            LessonProgressService lessonProgressService
        )
        {
            _logger = logger;
            _mailHelper = mailHelper;
            _centerService = centerService;
            _classService = classService;
            _classSubjectService = classSubjectService;
            _lessonService = lessonService;
            _scheduleService = scheduleService;
            _accountService = accountService;
            _studentService = studentService;
            _teacherService = teacherService;
            _skillService = skillService;
            _lessonScheduleService = lessonScheduleService;
            _roleService = roleService;
            _courseService = courseService;
            _learningHistory = learningHistory;
            _lessonProgressService = lessonProgressService;
        }
        public IActionResult Index(DateTime currentTime)
        {
            if (currentTime == null || currentTime <= DateTime.MinValue)
            {
                currentTime = DateTime.Now;
            }
            IEnumerable<CenterEntity> centersActive = _centerService.GetActiveCenter(currentTime);//lay co so dang hoat dong
            Dictionary<string, Dictionary<int, string[]>> dataClass = new Dictionary<string, Dictionary<int, string[]>>();
            Dictionary<string, string> classCenter = new Dictionary<string, string>();
            for(int i = 0; centersActive != null && i < centersActive.Count(); i++)
            {
                var center = centersActive.ElementAt(i);
                var classesActive = _classService.GetActiveClass(currentTime, center.ID);
                if(classesActive != null)
                {
                    foreach(var _class in classesActive)
                    {
                        classCenter.Add(_class.ID, center.ID);
                        var data = GetDataForReprot(_class, currentTime);
                        dataClass.Add(_class.ID, data);
                    }
                }
            }

            ViewBag.Centers = classCenter;
            ViewBag.Data = dataClass;
            // danh sach lop
            //var classesActive = _classService.GetActiveClass(currentTime, center.ID);

            return View();
        }

        public string DrawChart()
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return "";
        }

        private async Task SendWeeklyReport(CenterEntity center, DateTime currentTime, bool isTest)
        {
            var startWeek = currentTime.AddDays(-7);
            var endWeek = startWeek.AddDays(6).AddHours(23).AddMinutes(59).AddMilliseconds(59);
            var headRole = _roleService.GetItemByCode("head-teacher");

            var headTeacher = _teacherService.CreateQuery().Find(x => x.IsActive == true && x.Centers.Any(y => y.CenterID == center.ID && y.RoleID == headRole.ID) && x.Email != "huonghl@utc.edu.vn");
            var subject = "";
            subject += $"Báo cáo kết quả học tập của {center.Name} từ ngày {startWeek.ToString("dd-MM-yyyy")} đến ngày {endWeek.ToString("dd-MM-yyyy")}";
            var body = "";
            var tbody = "";
            tbody = @"<table style='margin-top:20px; width: 100%; border: solid 1px #333; border-collapse: collapse'>
                        <thead>
                            <tr style='font-weight:bold;background-color: bisque'>
                                <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:10px'>STT</td>
                                <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:150px'>Lớp</td>
                                <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'>Sĩ số lớp</td>
                                <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'>Học sinh chưa học</td>
                                <td colspan='4' style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Kết quả luyện tập & kiểm tra</td>
                                <td colspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Tiến độ</td>
                            </tr>
                            <tr style='font-weight:bold;background-color: bisque'>
                                <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'>8.0 -> 10</td>
                                <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'>5.0 -> 7.9</td>
                                <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'>1.0 -> 4.9</td>
                                <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'>0.0</td>
                                <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:150px'>Học liệu chính quy</td>
                                <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:150px'>Học liệu chuyên đề</td>
                            </tr>
                        </thead>
                        <tbody>";
            var classesActive = _classService.GetActiveClass(currentTime, center.ID);//lay danh sach lop dang hoat dong
            var index = 1;
            long totalStudent = 0, totalstChuaVaoLop = 0; ;
            long tren8 = 0;
            long tren5 = 0;
            long tren0 = 0;
            long bang0 = 0;
            string[] style = { "background-color: aliceblueT", "background-color: whitesmoke" };

            foreach (var _class in classesActive.OrderBy(x => x.Name))
            {
                //Lay danh sach ID hoc sinh trong lop
                var studentIds = _studentService.GetStudentsByClassId(_class.ID).Select(t => t.ID).ToList();
                var classStudent = studentIds.Count();
                totalStudent += classStudent;
                //Lay danh sach ID bai hoc duoc mo trong tuan
                var activeSchedules = _lessonScheduleService.CreateQuery().Find(o => o.ClassID == _class.ID && o.StartDate <= endWeek && o.EndDate >= startWeek)?.Project(t => new LessonScheduleEntity
                {
                    LessonID = t.LessonID,
                    ClassSubjectID = t.ClassSubjectID,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate
                })?.ToList();
                var activeLessonIds = activeSchedules?.Select(t => t.LessonID)?.ToList();
                //Lay danh sach hoc sinh da hoc cac bai tren trong tuan
                var activeProgress = _lessonProgressService.CreateQuery().Find(
                    x => studentIds.Contains(x.StudentID) && activeLessonIds.Contains(x.LessonID)
                    && x.LastDate <= endWeek && x.LastDate >= startWeek).ToEnumerable();
                var activeStudents = activeProgress.Select(t => t.StudentID).Distinct();
                //danh sach bai kiem tra
                var examIds = _lessonService.CreateQuery().Find(x => (x.TemplateType == 2 || x.IsPractice == true) && activeLessonIds.Contains(x.ID)).Project(x => x.ID).ToList();
                var stChuaVaoLop = classStudent - activeStudents.Count();
                var dtren8 = "---";
                var dtren5 = "---";
                var dtren0 = "---";
                var dbang0 = "---";
                if (examIds.Count > 0) //co bai kiem tra
                {
                    //ket qua lam bai cua hoc sinh trong lop
                    var classResult = (from r in activeProgress.Where(t => examIds.Contains(t.LessonID) && t.Tried > 0)
                                       group r by r.StudentID
                                       into g
                                       select new StudentResult
                                       {
                                           StudentID = g.Key,
                                           ExamCount = g.Count(),
                                           AvgPoint = g.Sum(t => t.LastPoint) / examIds.Count
                                       }).ToList();
                    //render ket qua hoc tap
                    var o8 = classResult.Count(t => t.AvgPoint >= 80);
                    var o5 = classResult.Count(t => t.AvgPoint >= 50 && t.AvgPoint < 80);
                    var o0 = classResult.Count(t => t.AvgPoint > 0 && t.AvgPoint < 50);
                    var e0 = classResult.Count(t => t.AvgPoint == 0);
                    tren8 += o8;
                    tren5 += o5;
                    tren0 += o0;
                    bang0 += e0;
                    dtren8 = o8.ToString();
                    dtren5 = o5.ToString();
                    dtren0 = o0.ToString();
                    dbang0 = e0.ToString();
                }
                totalstChuaVaoLop += stChuaVaoLop;
                var groupedSchedule = (from r in activeSchedules
                                       group r by r.ClassSubjectID into g
                                       select g.OrderByDescending(t => t.StartDate).FirstOrDefault());

                var subjectsInfo = (from r in groupedSchedule
                                    let lesson = _lessonService.GetItemByID(r.LessonID)
                                    let classSbj = _classSubjectService.GetItemByID(r.ID)
                                    select new ClassSubjectInfo
                                    {
                                        CourseName = classSbj.CourseName,
                                        LastLessonTitle = lesson.Title,
                                        Type = classSbj.TypeClass,
                                        LastTime = r.StartDate
                                    })?.ToList();
                var standardSbj = "";
                var extendSbj = "";
                foreach (var sbj in subjectsInfo)
                {
                    var content = "<table>" +
                            "<tr>" +
                                $"<td style='text-align: left'>{sbj.CourseName} - Bài học cuối: {sbj.LastLessonTitle} ({sbj.LastTime.ToLocalTime().ToString("dd-MM-yyyy")})</td>" +
                            "</tr>" +
                            "</table>";
                    if (sbj.Type == CLASS_TYPE.STANDARD)
                        standardSbj += content;
                    else
                        extendSbj += content;
                }

                if (index % 2 == 0)
                {
                    tbody += $"<tr style='{style[1]}'>";
                }
                else
                {
                    tbody += $"<tr style='{style[0]}'>";
                }

                tbody +=
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{index}</td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{_class.Name}</td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{classStudent}</td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{(stChuaVaoLop > 0 ? stChuaVaoLop.ToString() : "---")}</td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{dtren8}</td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{dtren5}</td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{dtren0}</td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{dbang0}</td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{standardSbj}</td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{extendSbj}</td></tr>";
            }
            tbody += @"<tr style='font-weight: 600'>
                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>
                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;text-align: left;font-weight: 600'>Tổng</td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;font-weight: 600'>{totalStudent}</td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;font-weight: 600'>{totalstChuaVaoLop}</td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;font-weight: 600'>{tren8}</td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;font-weight: 600'>{tren5}</td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;font-weight: 600'>{tren0}</td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;font-weight: 600'>{bang0}</td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;font-weight: 600'></td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;font-weight: 600'></td>" +
                @"</tr>
                    <tbody>
                    </table>";
            body += tbody;

            var toAddress = headTeacher.Project(t => t.Email).ToList();
            var bccAddress = new List<string> { "nguyenvanhoa2017602593@gmail.com", "vietphung.it@gmail.com", "huonghl@utc.edu.vn" };
            if (isTest)
            {
                toAddress = new List<string> { "nguyenvanhoa2017602593@gmail.com", "vietphung.it@gmail.com" };
                bccAddress = null;
            }
            await _mailHelper.SendBaseEmail(toAddress, subject, body, MailPhase.WEEKLY_SCHEDULE, null, bccAddress);
        }

        public async Task SendWeeklyReport(bool isTest)
        {
            var currentTime = new DateTime(2020, 10, 5);
            var centersActive = _centerService.GetActiveCenter(currentTime);//lay co so dang hoat dong
            foreach (var center in centersActive)
            {
                await SendWeeklyReport(center, currentTime, isTest);
            }
        }
        private string GetBase64FromJavaScriptImage(string javaScriptBase64String)
        {
            return Regex.Match(javaScriptBase64String, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
        }
        [HttpPost]
        public async Task<JsonResult> SendMonthlyReport(string image)
        {
            try
            {
                //var dateTime = DateTime.Now;
                var dateTime = new DateTime(2020, 11, 01);
                var day = dateTime.Day;
                var month = dateTime.Month;
                var year = dateTime.Year;

                var currentTime = new DateTime(year, month, day, 0, 0, 0);
                var startMonth = currentTime.AddMonths(-1);
                var endMonth = currentTime.AddDays(-1).AddHours(23).AddMinutes(59);

                //var centersActive = _centerService.GetActiveCenter(currentTime);//lay co so dang hoat dong trong thang
                //foreach(var center in centersActive)
                //{
                //    if (center.Abbr == "c3vyvp")//test truong Vinh Yen
                //    {
                //        var listTeacherHeader = _teacherService.CreateQuery().Find(x => x.IsActive == true && x.Centers.Any(y => y.CenterID == center.ID)).ToList().FindAll(y => HasRole(y.ID, center.ID, "head-teacher")).Select(x => x.Email).ToList();
                //        if (listTeacherHeader.Contains("huonghl@utc.edu.vn"))
                //        {
                //            listTeacherHeader.Remove("huonghl@utc.edu.vn");
                //        }
                string base64 = GetBase64FromJavaScriptImage(image);
                var bytes = Convert.FromBase64String(base64);
                string link = "";
                using(var memory = new MemoryStream(bytes))
                {
                    link = "";//Program.GoogleDriveApiService.CreateLinkPreViewFile(_roxyFilemanHandler.UploadFileWithGoogleDrive("eduso", "admin", memory));
                    //_roxyFilemanHandler.UploadFileWithGoogleDrive("eduso", "admin", memory);
                }

                var body = $"<div><img src='{link}' /></div>";
                StringBuilder test = new StringBuilder();
                test.Append("<div data-image=\""+image+"\"><img src=");
                test.Append(image);
                test.Append("/></div>");
                var subject = "div tesst";
                //var isTest = true;
                //var toAddress = isTest == true ? new List<string> { "nguyenvanhoa2017602593@gmail.com", "vietphung.it@gmail.com" } : listTeacherHeader;
                //var bccAddress = isTest == true ? null : new List<string> { "nguyenhoa.dev@gmail.com", "vietphung.it@gmail.com", "huonghl@utc.edu.vn", "buihong9885@gmail.com" };
                //_ = await _mailHelper.SendBaseEmail(toAddress, subject, body, MailPhase.WEEKLY_SCHEDULE, null, bccAddress);
                var isTest = true;
                var toAddress = isTest == true ? new List<string> { "nguyenvanhoa2017602593@gmail.com", "vietphung.it@gmail.com" } : new List<string> { "shin.l0v3.ly@gmail.com" };
                try
                {
                    _ = await _mailHelper.SendBaseEmail(toAddress, subject, body, MailPhase.WEEKLY_SCHEDULE, null, toAddress);
                    return Json("ok");
                }
                catch (Exception ex)
                {
                    return Json(ex.Message);
                }

                //    }
                //}
                //return Json("OK");
                //var dataResponse = new Dictionary<string, object>();
                //foreach (var center in centersActive)
                //{
                //    var dataInClass = new Dictionary<string, object>();
                //    if (center.Abbr == "c3vyvp")//test truong Vinh Yen
                //    {
                //        var classesActive = _classService.GetActiveClass(currentTime, center.ID);//lay danh sach lop dang hoat dong
                //        var index = 0;
                //        foreach (var @class in classesActive.OrderBy(x => x.Name))
                //        {
                //            var data = GetDataForReprot(@class, dateTime);
                //            if (!dataInClass.Keys.Contains(@class.Name))
                //            {
                //                dataInClass.Add($"{@class.Name}", data);
                //            }
                //            else
                //            {
                //                dataInClass.Add($"{@class.Name} - {index}", data);
                //            }
                //            index++;
                //        }
                //        dataResponse.Add($"{center.Name}", dataInClass);
                //    }
                //}
                //return Json(dataResponse);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
        [HttpGet]
        public Dictionary<int, string[]> GetDataReprot(ClassEntity @class, DateTime dateTime)
        {
            return GetDataForReprot(@class, dateTime);
        }

        private Dictionary<int, string[]> GetDataForReprot(ClassEntity @class, DateTime dateTime)
        {
            //[] => {}
            var dataResponse = new Dictionary<int, string[]>();

            //var dateTime = DateTime.Now;
            var day = dateTime.Day;
            var month = dateTime.Month;
            var year = dateTime.Year;

            var currentTime = new DateTime(year, month, day, 0, 0, 0).AddMonths(-1);//Lùi 1 tháng
            var sw1 = currentTime;//Tuan 1
            var ew1 = currentTime.AddDays(6).AddHours(23).AddMinutes(59);

            var sw2 = ew1.AddMinutes(1);//Tuan 2
            var ew2 = sw2.AddDays(6).AddHours(23).AddMinutes(59);

            var sw3 = ew2.AddMinutes(1);//Tuan 3
            var ew3 = sw3.AddDays(6).AddHours(23).AddMinutes(59);

            var sw4 = ew3.AddMinutes(1);//Tuan 4
            var ew4 = new DateTime(year, month, day, 0, 0, 0).AddDays(-1);//Lui 1 ngay

            var listDateTime = new List<dateTime>
            {
                new dateTime(sw1, ew1),
                new dateTime(sw2, ew2),
                new dateTime(sw3, ew3),
                new dateTime(sw4, ew4)
            };

            var key = 0;
            foreach (var item in listDateTime)
            {
                var data = GetDataInWeek(item.StartWeek, item.EndWeek, @class);
                dataResponse.Add(key, data);
                key++;
            };

            return dataResponse;
        }
        /// <summary>
        /// //classStudent.ToString(),stChuaVaoLop.ToString(),min8.ToString(),min5.ToString(),min2.ToString(),min0.ToString(),chualam.ToString()
        /// </summary>
        /// <param name="startWeek"></param>
        /// <param name="endWeek"></param>
        /// <param name="class"></param>
        /// <returns></returns>
        private string[] GetDataInWeek(DateTime startWeek,DateTime endWeek,ClassEntity @class)
        {
            //data can lay
            var min8 = 9;
            var min5 = 0;
            var min2 = 0;
            var min0 = 0;
            var chualam = 0;

            //Lay danh sach ID hoc sinh trong lop
            var studentIds = _studentService.GetStudentsByClassId(@class.ID).Select(t => t.ID).ToList();
            var classStudent = studentIds.Count();

            var activeSchedules = _lessonScheduleService.CreateQuery().Find(o => o.ClassID == @class.ID && o.StartDate <= endWeek && o.EndDate >= startWeek)?.Project(t => new LessonScheduleEntity
            {
                LessonID = t.LessonID,
                ClassSubjectID = t.ClassSubjectID,
                StartDate = t.StartDate,
                EndDate = t.EndDate
            })?.ToList();
            var activeLessonIds = activeSchedules?.Select(t => t.LessonID)?.ToList();

            //Lay danh sach hoc sinh da hoc cac bai tren trong tuan
            var activeProgress = _lessonProgressService.CreateQuery().Find(
                x => studentIds.Contains(x.StudentID) && activeLessonIds.Contains(x.LessonID)
                && x.LastDate <= endWeek && x.LastDate >= startWeek).ToEnumerable();
            var activeStudents = activeProgress.Select(t => t.StudentID).Distinct();
            var stChuaVaoLop = classStudent - activeStudents.Count();

            var examIds = _lessonService.CreateQuery().Find(x => (x.TemplateType == 2 || x.IsPractice == true) && activeLessonIds.Contains(x.ID)).Project(x => x.ID).ToList();
            if (examIds.Count > 0) //co bai kiem tra
            {
                //ket qua lam bai cua hoc sinh trong lop
                var classResult = (from r in activeProgress.Where(t => examIds.Contains(t.LessonID) && t.Tried > 0)
                                   group r by r.StudentID
                                   into g
                                   select new StudentResult
                                   {
                                       StudentID = g.Key,
                                       ExamCount = g.Count(),
                                       AvgPoint = g.Sum(t => t.LastPoint) / examIds.Count
                                   }).ToList();
                //render ket qua hoc tap
                min8 = classResult.Count(t => t.AvgPoint >= 80);
                min5 = classResult.Count(t => t.AvgPoint >= 50 && t.AvgPoint < 80);
                min2 = classResult.Count(t => t.AvgPoint >= 20 && t.AvgPoint < 50);
                min0 = classResult.Count(t => t.AvgPoint >= 0 && t.AvgPoint < 20);
                chualam = classStudent - (min0 + min2 + min5 + min8);
            }

            string[] data = {classStudent.ToString(),stChuaVaoLop.ToString(),min8.ToString(),min5.ToString(),min2.ToString(),min0.ToString(),chualam.ToString() };
            return data;
        }

        private bool HasRole(string userid, string center, string role)
        {
            var teacher = _teacherService.GetItemByID(userid);
            if (teacher == null) return false;
            var centerMember = teacher.Centers.Find(t => t.CenterID == center);
            if (centerMember == null) return false;
            if (_roleService.GetItemByID(centerMember.RoleID).Code != role) return false;
            return true;
        }

        public class StudentResult
        {
            public string StudentID { get; set; }
            public long ExamCount { get; set; }
            public double AvgPoint { get; set; }
        }

        public class ClassSubjectInfo
        {
            public string CourseName { get; set; }
            public string LastLessonTitle { get; set; }
            public DateTime LastTime { get; set; }
            public int Type { get; set; }
        }

        public class ScheduleView : LessonScheduleEntity
        {
            public string LessonName { get; set; }

            public ScheduleView(LessonScheduleEntity schedule)
            {
                LessonID = schedule.LessonID;
                StartDate = schedule.StartDate;
                EndDate = schedule.EndDate;
            }
        }

        public class dateTime
        {
            public DateTime StartWeek { get; set; }
            public DateTime EndWeek { get; set; }

            public dateTime(DateTime sw, DateTime ew)
            {
                this.StartWeek = sw;
                this.EndWeek = ew;
            }
        }
    }
}