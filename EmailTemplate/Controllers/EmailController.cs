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
        private readonly IRoxyFilemanHandler _roxyFilemanHandler;
        //private static bool isTest = false;
        public EmailController(ILogger<EmailController> logger, 
            MailHelper mailHelper,
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
            LessonProgressService lessonProgressService,
            IRoxyFilemanHandler roxyFilemanHandler
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
            _roxyFilemanHandler = roxyFilemanHandler;
        }
        public IActionResult Index(DateTime currentTime)
        {
            if (currentTime == null || currentTime <= DateTime.MinValue)
            {
                currentTime = DateTime.Now;
            }
            IEnumerable<CenterEntity> centersActive = _centerService.GetActiveCenter(currentTime);//lay co so dang hoat dong
            //Dictionary<string, Dictionary<int, string[]>> dataClass = new Dictionary<string, Dictionary<int, string[]>>();
            //Dictionary<string, string> classCenter = new Dictionary<string, string>();
            Dictionary<string, Dictionary<int, double[]>> dataCenter = new Dictionary<string, Dictionary<int, double[]>>();
            Dictionary<string, string> centerName = new Dictionary<string, string>();
            for (int i = 0; centersActive != null && i < centersActive.Count(); i++)
            {
                //long totalStudents = 0, totalStChuaHoc = 0, totalMin8 = 0, totalMin5 = 0, totalMin2 = 0, totalMin0 = 0, totalChuaLam = 0;
                var center = centersActive.ElementAt(i);
                if (center.Abbr == "c3vyvp")
                {
                    var data = GetDataForReprot(center, currentTime);
                    dataCenter.Add(center.ID, data);
                    centerName.Add(center.ID, center.Name);
                    //var classesActive = _classService.GetActiveClass(currentTime, center.ID);
                    //if (classesActive != null)
                    //{
                    //    foreach (var _class in classesActive)
                    //    {
                    //classCenter.Add(_class.ID, center.ID);
                    //dataClass.Add(_class.ID, data);
                    //dataClassName.Add(_class.ID, _class.Name);
                    //    }
                    //}
                }
            }

            //ViewBag.Centers = classCenter;
            //ViewBag.Data = dataClass;
            //ViewBag.ClassName = dataClassName;
            ViewBag.DataCenter = dataCenter;
            ViewBag.CenterName = centerName;

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
            var classesActive = _classService.GetActiveClass4Report(currentTime, center.ID);//lay danh sach lop dang hoat dong
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
        public async Task<JsonResult> SendMonthlyReport(List<DataToSendMail> Data)
        {
            var Msg = "";
            try
            {
                var dataToSend = Data.ToList().GroupBy(x => x.CenterID, (k, g) => new { CenterID = k, Images = g.Select(x => x.Image).ToList() });
                foreach (var d in dataToSend)
                {
                    var center = _centerService.GetItemByID(d.CenterID);
                    var hello = "";
                    var listTeacherHeader = _teacherService.CreateQuery().Find(x => x.IsActive == true && x.Centers.Any(y => y.CenterID == center.ID)).ToList().FindAll(y => HasRole(y.ID, center.ID, "head-teacher")).ToList();
                    List<string> listEmail = new List<string>();
                    foreach (var t in listTeacherHeader)
                    {
                        if (t.Email != "huonghl@utc.edu.vn")
                        {
                            listEmail.Add(t.Email);
                            hello += $"<p>Kính gửi Thầy/Cô {t.FullName}</p>";
                        }
                    }

                    var body = await ContentToSendEmail(d.Images, center.Name);
                    var subject = $"Báo cáo kết quả học tập {center.Name.ToUpper()} THÁNG {DateTime.Now.Month - 1}";
                    var content = $"{hello}{body}";
                    
                    //List<string> toAddress = isTest == true ? new List<string> { "shin.l0v3.ly@gmail.com", "vietphung.it@gmail.com" } : listEmail;
                    //List<string> bccAddress = isTest == true ? null : new List<string> { "nguyenhoa.dev@gmail.com", "vietphung.it@gmail.com", "huonghl@utc.edu.vn", "buihong9885@gmail.com" };
                    //_ = await _mailHelper.SendBaseEmail(toAddress, subject, content, MailPhase.WEEKLY_SCHEDULE, null, bccAddress);

                    //List<string> toAddress = new List<string> { "shin.l0v3.ly@gmail.com", "vietphung.it@gmail.com","huonghl@utc.edu.vn", "buihong9885@gmail.com" };
                    List<string> toAddress = new List<string> { "shin.l0v3.ly@gmail.com" , "vietphung.it@gmail.com" };
                    _ = await _mailHelper.SendBaseEmail(toAddress, subject, content, MailPhase.WEEKLY_SCHEDULE, null);
                    Msg += $"Send To {center.Name} is done, ";
                }
                return Json(Msg);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        private async Task<string> ContentToSendEmail(List<string> Images, string CenterName)
        {
            var body = "";
            var index = 0;
            foreach (var image in Images)
            {
                string base64 = GetBase64FromJavaScriptImage(image);
                var bytes = Convert.FromBase64String(base64);
                string link = "";
                using (var memory = new MemoryStream(bytes))
                {
                    link = Program.GoogleDriveApiService.CreateLinkViewFile(_roxyFilemanHandler.UploadFileWithGoogleDrive("eduso", "admin", memory));
                }
                body += $"<div>" +
                    $"<h3 style='text-align: center;;width: 90%;'>Kết quả học tập {CenterName.ToUpper()} tháng {DateTime.Now.Month - 1}<h3>".ToUpper() +
                    $"<img src='{link}' style='width: 90%;height: 90%;' />" +
                    $"</div>";
                index++;
            }
            return body;
        }

        /// <summary>
        /// Lấy data cho báo cáo tuần
        /// </summary>
        /// <param name="class"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private Dictionary<int, double[]> GetDataForReprot(CenterEntity center, DateTime dateTime)
        {
            //[] => {}
            var dataResponse = new Dictionary<int, double[]>();

            var currentMonth = new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0);
            var firstDay = currentMonth.AddMonths(-1); //ngay dau tien cua thang 0h 0p 0s
            var lastDay = currentMonth.AddDays(-1);//ngay cuoi cung cua thang 0h0p00s

            var sw1 = firstDay.AddDays(DayOfWeek.Sunday - firstDay.DayOfWeek + 1);//tuần chứa ngày đầu tiên trong tháng
            var ew1 = firstDay.AddDays(6).AddHours(23).AddMinutes(59);//tuần chứa ngày đầu tiên trong tháng

            var sw2 = ew1.AddMinutes(1);
            var ew2 = sw2.AddDays(6).AddHours(23).AddMinutes(59);

            var sw3 = ew2.AddMinutes(1);
            var ew3 = sw3.AddDays(6).AddHours(23).AddMinutes(59);

            var sw4 = ew3.AddMinutes(1);
            var ew4 = sw4.AddDays(6).AddHours(23).AddMinutes(59);

            var sw = lastDay.AddDays(DayOfWeek.Saturday - lastDay.DayOfWeek + 1);//tuần chứa ngày cuối cùng trong tháng
            var ew = sw.AddDays(6).AddHours(23).AddMinutes(59);//tuần chứa ngày cuối cùng trong tháng

            DateTime sw5 = new DateTime(1900, 1, 1);
            DateTime ew5 = new DateTime(1900, 1, 7);

            var listDateTime = new List<dateTime>();
            if (ew4 == ew)
            {
                listDateTime = new List<dateTime> {
                new dateTime(sw1, ew1),
                new dateTime(sw2, ew2),
                new dateTime(sw3, ew3),
                new dateTime(sw4, ew4)
                };
            }
            else
            {
                sw5 = ew4.AddMinutes(1);
                ew5 = sw5.AddDays(6).AddHours(23).AddMinutes(59);
                listDateTime = new List<dateTime> {
                new dateTime(sw1, ew1),
                new dateTime(sw2, ew2),
                new dateTime(sw3, ew3),
                new dateTime(sw4, ew4),
                new dateTime(sw5, ew5)
                };
            }

            //var dateTime = DateTime.Now;
            //var day = dateTime.Day;
            //var month = dateTime.Month;
            //var year = dateTime.Year;
            //var currentTime = new DateTime(year, month, day, 0, 0, 0).AddMonths(-1);//Lùi 1 tháng
            //var sw1 = currentTime;//Tuan 1
            //var ew1 = currentTime.AddDays(6).AddHours(23).AddMinutes(59);

            //var sw2 = ew1.AddMinutes(1);//Tuan 2
            //var ew2 = sw2.AddDays(6).AddHours(23).AddMinutes(59);

            //var sw3 = ew2.AddMinutes(1);//Tuan 3
            //var ew3 = sw3.AddDays(6).AddHours(23).AddMinutes(59);

            //var sw4 = ew3.AddMinutes(1);//Tuan 4
            //var ew4 = new DateTime(year, month, day, 0, 0, 0).AddDays(-1);//Lui 1 ngay
            //var listDateTime = new List<dateTime>
            //{
            //    new dateTime(sw1, ew1),
            //    new dateTime(sw2, ew2),
            //    new dateTime(sw3, ew3),
            //    new dateTime(sw4, ew4)
            //};

            var key = 0;
            foreach (var item in listDateTime)
            {
                var data = GetDataInWeek(item.StartWeek, item.EndWeek, center);
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
        private double[] GetDataInWeek(DateTime startWeek, DateTime endWeek, CenterEntity center)
        {
            var classesActive = _classService.GetActiveClass4Report(startWeek.AddDays(1), center.ID);
            if (classesActive != null)
            {
                double totalStudents = 0, totalStChuaHoc = 0, totalMin8 = 0, totalMin5 = 0, totalMin2 = 0, totalMin0 = 0, totalChuaLam = 0;
                foreach (var @class in classesActive)
                {
                    //data can lay
                    var min8 = 0;
                    var min5 = 0;
                    var min2 = 0;
                    var min0 = 0;
                    var chualam = 0;

                    //Lay danh sach ID hoc sinh trong lop
                    var studentIds = _studentService.GetStudentsByClassId(@class.ID).Select(t => t.ID).ToList();
                    var classStudent = studentIds.Count();
                    totalStudents += classStudent;

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
                    var stChuaHoc = classStudent - activeStudents.Count();
                    totalStChuaHoc += stChuaHoc;

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

                        totalMin8 += min8;
                        totalMin5 += min5;
                        totalMin2 += min2;
                        totalMin0 += min0;
                        totalChuaLam += chualam;
                    }
                }
                double[] data = { totalStudents, totalStChuaHoc, totalMin8, totalMin5, totalMin2, totalMin0, totalChuaLam };
                return data;
            }
            else
            {
                return new double[] { };
            }
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

        public class DataToSendMail
        {
            public string CenterID { get; set; }
            public string Image { get; set; }
            //public string ClassID { get; set; }
        }
    }
}