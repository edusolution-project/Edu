﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Newtonsoft.Json;

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
        private readonly ReportService _reportService;
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
            IRoxyFilemanHandler roxyFilemanHandler,
            ReportService reportService
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
            _reportService = reportService;
        }

        #region mail
        public IActionResult Index(DateTime currentTime)
        {
            if (currentTime == null || currentTime <= DateTime.MinValue)
            {
                currentTime = DateTime.Now;
                //currentTime = new DateTime(2020, 11, 1, 8, 00, 00);
            }
            IEnumerable<CenterEntity> centersActive = _centerService.GetActiveCenter(currentTime);//lay co so dang hoat dong

            Dictionary<string, int[]> BlockCenter = new Dictionary<string, int[]>();
            Dictionary<string, int> BlockClass = new Dictionary<string, int>();
            Dictionary<string, double[]> DataClass = new Dictionary<string, double[]>();
            Dictionary<String, String> ClassName = new Dictionary<string, string>();
            Dictionary<string, string> classCenter = new Dictionary<string, string>();
            int totalBlock = 0;

            for (int i = 0; centersActive != null && i < centersActive.Count(); i++)
            {
                List<int> Block = new List<int>();
                var center = centersActive.ElementAt(i);
                if (center.Abbr == "c3vyvp")
                //if (center.Abbr != "eduso")
                //if(center.ID== "5f17bf6569926b0f6481b742")
                {
                    var data = GetDataForReprot(center, currentTime);
                    //DataClass = data;
                    foreach(var item in data)
                    {
                        DataClass.Add(item.Key, item.Value);
                    }
                    var ClassIDs = data?.Keys;
                    foreach(var ClassID in ClassIDs)
                    {
                        //classCenter.Add(ClassID, center.ID);
                        var @class = _classService.GetItemByID(ClassID);
                        if (@class.Name.Contains("10"))
                        {
                            BlockClass.Add(@class.ID, 10);
                            ClassName.Add(@class.ID, @class.Name);
                            Block.Add(10);
                            classCenter.Add(@class.ID, center.ID);
                        }
                        else if (@class.Name.Contains("11"))
                        {
                            BlockClass.Add(@class.ID,11);
                            ClassName.Add(@class.ID, @class.Name);
                            Block.Add(11);
                            classCenter.Add(@class.ID, center.ID);
                        }
                        else if (@class.Name.Contains("12"))
                        {
                            BlockClass.Add(@class.ID, 12);
                            ClassName.Add(@class.ID, @class.Name);
                            Block.Add(12);
                            classCenter.Add(@class.ID, center.ID);
                        }
                        else
                        {
                            BlockClass.Add(@class.ID, 99);
                            ClassName.Add(@class.ID, @class.Name);
                            Block.Add(99);
                            classCenter.Add(@class.ID, center.ID);
                        }
                    }
                }
                BlockCenter.Add(center.ID, Block.Distinct().ToArray<int>());
                totalBlock = Block.Distinct().Count();
            }

            ViewBag.BlockCenter = BlockCenter;
            ViewBag.BlockClass = BlockClass;
            ViewBag.DataClass = DataClass;
            ViewBag.ClassName = ClassName;
            ViewBag.CountBlock = BlockCenter.Values.Sum(x=>x.Length);
            ViewBag.ClassCenters = classCenter;
            ViewBag.CurrentTime = currentTime;

            return View();
        }

        private string GetBase64FromJavaScriptImage(string javaScriptBase64String)
        {
            return Regex.Match(javaScriptBase64String, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
        }

        [HttpPost]
        public async Task<JsonResult> SendMonthlyReport(List<DataToSendMail> Data, Boolean isTest = true)
        {
            var Msg = "";
            try
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                var dataToSend = Data.ToList().GroupBy(x => x.CenterID, (k, g) => new { CenterID = k, Images = g.Select(x => x.Image).ToList()});
                foreach (var d in dataToSend)
                {
                    if(d.CenterID== "5eb982be07ed0c1894762c40")//Co so Eduso
                    {
                        continue;
                    }
                    var center = _centerService.GetItemByID(d.CenterID);
                    var hello = "<div>Kính gửi Thầy/Cô: ";
                    var listTeacherHeader = _teacherService.CreateQuery().Find(x => x.IsActive == true && x.Centers.Any(y => y.CenterID == center.ID)).ToList().FindAll(y => HasRole(y.ID, center.ID, "head-teacher")).ToList();
                    List<string> listEmail = new List<string>();
                    for(int i=0;i<listTeacherHeader.Count();i++)
                    {
                        var t = listTeacherHeader.ElementAt(i);
                        if (t.Email != "huonghl@utc.edu.vn")
                        {
                            listEmail.Add(t.Email);
                            hello += $"<span style='font-weight:600'>{t.FullName}</span>, ";

                        }
                    }

                    var body = await GetContent(d.Images,center.ID);
                    var time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 23, 59, 00);
                    var subject = $"Báo cáo tháng {time.Month-1} - {center.Name}";
                    string note = $"<div>Eduso kính gửi thầy/cô kết quả học tập trong tháng {time.AddMonths(-1).Month} của các lớp.</div>{Note}<div style='font-style:italic;font-size:12px'>Số liệu được cập nhật lần cuối lúc {time.AddDays(-1).ToString("HH:mm - dd/MM/yyyy")}.</div>";
                    var content = $"{hello}<p></p>{note}{body}";

                    List<string> toAddress = isTest == true ? new List<string> { "shin.l0v3.ly@gmail.com", "vietphung.it@gmail.com" } : listEmail;
                    List<string> bccAddress = isTest == true ? null : new List<string> { "nguyenhoa.dev@gmail.com", "vietphung.it@gmail.com", "huonghl@utc.edu.vn", "manhdv@utc.edu.vn" };
                    //_ = await _mailHelper.SendBaseEmail(toAddress, subject, content, MailPhase.WEEKLY_SCHEDULE, null, bccAddress);

                    //List<string> toAddress = new List<string> { "shin.l0v3.ly@gmail.com", "vietphung.it@gmail.com", "huonghl@utc.edu.vn", "buihong9885@gmail.com", "manhdv@utc.edu.vn" };
                    //List<string> toAddress = new List<string> { "shin.l0v3.ly@gmail.com" };
                    //_ = await _mailHelper.SendBaseEmail(toAddress, subject, content, MailPhase.WEEKLY_SCHEDULE, null);
                    Msg += $"Send To {center.Name} is done, ";
                }

                stopWatch.Stop();
                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = stopWatch.Elapsed;

                // Format and display the TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                return Json(Msg+elapsedTime);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        private async Task<String> GetContent(List<String> Images, string CenterID)
        {
            Double totalstChuaVaoLop = 0, totalStudent = 0, totalMinPoint8 = 0, totalMinPoint5 = 0, totalMinPoint2 = 0, totalMinPoint0 = 0, totalChuaHoc = 0;
            var body = "<div>";
            var thead = @"<table style='margin-top:20px; width: 100%; /*border: solid 1px #333*/; border-collapse: collapse'>
                            <thead>
                                        <tr style='font-weight:bold;background-color: bisque'>
                                            <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:10px'>STT</td>
                                            <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:100px'>Lớp</td>
                                            <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'>Sĩ số lớp</td>
                                            <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'>Chưa đăng nhập</td>
                                            <td colspan='5' style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Kết quả luyện tập & kiểm tra</td>
                                        </tr>
                                        <tr style='font-weight:bold;background-color: bisque'>
                                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'>8.0 -> 10</td>
                                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'>5.0 -> 7.9</td>
                                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'> 2.0 -> 4.9</td>
                                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'>0.0 -> 2.0</td>
                                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'>Chưa làm</td>
                                        </tr>
                                    </thead>
                                    <tbody>"; ;
            var tbody = "";

            var dataTable = _reportService.GetReport(new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0), CenterID).Distinct();
            if (dataTable != null)
            {
                Int32 index = 0;
                foreach (var item in dataTable)
                {
                    index++;
                    tbody += "<tr>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:10px'>{index}</td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:100px'>{item.ClassName}</td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:100px'>{item.Students}</td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:100px'>{item.InactiveStudents}</td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:100px'>{item.MinPoint8}</td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:100px'>{item.MinPoint5}</td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:100px'>{item.MinPoint2}</td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:100px'>{item.MinPoint0}</td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:100px'>{item.Students - item.MinPoint0 - item.MinPoint2 - item.MinPoint5 - item.MinPoint8}</td>" +
                    $"</tr>";
                    totalStudent += item.Students;
                    totalstChuaVaoLop += item.InactiveStudents;
                    totalMinPoint8 += item.MinPoint8;
                    totalMinPoint5 += item.MinPoint5;
                    totalMinPoint2 += item.MinPoint2;
                    totalMinPoint0 += item.MinPoint0;
                    totalChuaHoc += (item.Students - item.MinPoint0 - item.MinPoint2 - item.MinPoint5 - item.MinPoint8);
                }
                
                double persentChuaDangNhap = Math.Round((totalstChuaVaoLop / totalStudent) * 100,2);
                double persentMinPoint8 = Math.Round((totalMinPoint8 / totalStudent) * 100,2);
                double persentMinPoint5 = Math.Round((totalMinPoint5 / totalStudent) * 100, 2);
                double persentMinPoint2 = Math.Round((totalMinPoint2 / totalStudent) * 100, 2);
                double persentMinPoint0 = Math.Round((totalMinPoint0 / totalStudent) * 100, 2);
                double persentChuaHoc = 100 - persentMinPoint8 - persentMinPoint5 - persentMinPoint2 - persentMinPoint0;

                if (totalStudent == 0)
                {
                    persentMinPoint8 = 0;
                    persentMinPoint5 = 0;
                    persentMinPoint2 = 0;
                    persentMinPoint0 = 0;
                    persentChuaHoc = 0;
                }

                tbody += $"<tr style='font-weight: 600'>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:10px'></td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:100px'>Tổng</td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:100px'>{totalStudent} <span style='color:red'>(100%)</span></td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:100px'>{totalstChuaVaoLop} <span style='color:red'>({persentChuaDangNhap.ToString("#0.00")}%)</span></td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:100px'>{totalMinPoint8} <span style='color:red'>({persentMinPoint8.ToString("#0.00")}%)</span></td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:100px'>{totalMinPoint5} <span style='color:red'>({persentMinPoint5.ToString("#0.00")}%)</span></td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:100px'>{totalMinPoint2} <span style='color:red'>({persentMinPoint2.ToString("#0.00")}%)</span></td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:100px'>{totalMinPoint0} <span style='color:red'>({persentMinPoint0.ToString("#0.00")}%)</span></td>" +
                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:100px'>{totalChuaHoc} <span style='color:red'>({persentChuaHoc.ToString("#0.00")}%)</span></td>" +
                    $"</tr>";
                tbody += $"</tbody>" +
                        $"<table>";
                body = $"{thead}{tbody}" +
                    $"</div>";

                var chart = "<div>";
                foreach (var image in Images)
                {
                    string base64 = GetBase64FromJavaScriptImage(image);
                    var bytes = Convert.FromBase64String(base64);
                    string link = "";
                    using (var memory = new MemoryStream(bytes))
                    {
                        link = Program.GoogleDriveApiService.CreateLinkViewFile(_roxyFilemanHandler.UploadFileWithGoogleDrive("eduso", "admin", memory));
                    }
                    chart += $"<div>" +
                                $"<img src='{link}' style='width: 100%;' />" +
                            $"</div>";
                }
                chart += "</div>";
                return $"{body}</tbody></table></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr></tbody></table><br><table><tr><td colspan='5'>{chart}</td></tr></table><br><table><tr><td colspan='5'>{extendTeacher}</td></tr></table>";
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Lấy data cho báo cáo tuần
        /// </summary>
        /// <param name="class"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private Dictionary<String, Double[]> GetDataForReprot(CenterEntity center, DateTime dateTime)
        {
            var currentMonth = new DateTime(dateTime.Year, dateTime.Month, 1, 0, 00, 00);
            //var currentMonth = new DateTime(2020, 11, 1, 0, 0, 0);
            var firstDay = currentMonth.AddMonths(-1); //ngay dau tien cua thang 0h 0p 0s
            var lastDay = currentMonth.AddDays(-1).AddHours(23).AddMinutes(59);//ngay cuoi cung cua thang 23h59p00s

            var data = GetDataInMonth(firstDay, lastDay, center);
            return data;
        }

        /// <summary>
        /// //classStudent.ToString(),stChuaVaoLop.ToString(),min8.ToString(),min5.ToString(),min2.ToString(),min0.ToString(),chualam.ToString()
        /// </summary>
        /// <param name="startWeek"></param>Ngày đầu tháng
        /// <param name="endWeek"></param>//ngày cuối tháng
        /// <param name="center"></param>//cơ sở
        /// <returns></returns>
        private Dictionary<String, Double[]> GetDataInMonth(DateTime startWeek, DateTime endWeek, CenterEntity center)
        {
            Dictionary<String, Double[]> dataResponse = new Dictionary<string, double[]>();
            //var classesActive = _classService.CreateQuery().Find(x => x.StartDate < endWeek && x.EndDate >= startWeek && x.Center == center.ID).ToEnumerable().OrderBy(x => x.Name);
            var classesActive = _classService.GetActiveClass4Report(startWeek,endWeek,center.ID).OrderBy(x => x.Name);
            if (classesActive != null)
            {
                double totalStudents = 0, totalStChuaHoc = 0, totalMin8 = 0, totalMin5 = 0, totalMin2 = 0, totalMin0 = 0, totalChuaLam = 0;
                for (int i = 0; i < classesActive.Count(); i++)
                {
                    var @class = classesActive.ElementAt(i);
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

                    //var activeStudents = activeProgress.Select(t => t.StudentID).Distinct();
                    var activeStudents = _lessonProgressService.CreateQuery().Distinct(t => t.StudentID,
                        x => studentIds.Contains(x.StudentID) && activeLessonIds.Contains(x.LessonID)
                        && x.LastDate <= endWeek && x.LastDate >= startWeek).ToEnumerable();

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

                        //luu vao database
                        var report = new ReportEntity()
                        {
                            CenterID = center.ID,
                            ClassID = @class.ID,
                            Students = classStudent,
                            InactiveStudents = stChuaHoc,
                            MinPoint8 = min8,
                            MinPoint5 = min5,
                            MinPoint2 = min2,
                            MinPoint0 = min0,
                            TimeExport = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0),
                            StartDate = startWeek,
                            EndDate = endWeek,
                            ClassName = @class.Name
                        };

                        var oldReport = _reportService.GetReport(new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0), center.ID, @class.ID).ToList();

                        if (oldReport.Count == 0)
                        {
                            _reportService.Save(report);
                        }

                        totalMin8 += min8;
                        totalMin5 += min5;
                        totalMin2 += min2;
                        totalMin0 += min0;
                        totalChuaLam += chualam;
                    }
                    else
                    {
                        var report = new ReportEntity()
                        {
                            CenterID = center.ID,
                            ClassID = @class.ID,
                            Students = classStudent,
                            InactiveStudents = stChuaHoc,
                            MinPoint8 = min8,
                            MinPoint5 = min5,
                            MinPoint2 = min2,
                            MinPoint0 = min0,
                            TimeExport = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0),
                            StartDate = startWeek,
                            EndDate = endWeek,
                            ClassName = @class.Name
                        };
                        var oldReport = _reportService.GetReport(new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0), center.ID, @class.ID).ToList();

                        if (oldReport.Count == 0)
                        {
                            _reportService.Save(report);
                        }
                    }

                    dataResponse.Add(@class.ID, new Double[] { classStudent, stChuaHoc, min8, min5, min2, min0, (classStudent - min8 - min5 - min2 - min0) });
                }
                return dataResponse;
            }
            else
            {
                return dataResponse;
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

        private readonly static String Note = "<div style='font-style:italic;font-size:12px'>Kết quả luyện tập & kiểm tra là điểm trung bình các bài luyện tập và kiểm tra được các thầy/cô lên lịch giao cho Học sinh làm trong tháng.</div>";

        private static readonly string extendTeacher =
        @"<br>
        <div style='color:#333;font-size: 90%;'>
            <div>
                <i style='text-decoration: underline;'>Bạn có thể:</i>
            </div>
            <div style='padding-left: 30px;'>
                <p style='padding-top: 5px;margin: 0;'>
                    <div style='line-height: 30px;'>- Vào <img src='https://static.eduso.vn//images/book-pen.png?w=20&h=20&mode=crop&format=jpg' style='max-width:20px;max-height:20px;vertical-align: middle;'><b style='color: red;'> ""Quản lý lớp học""</b> để tạo lớp, đặt lịch dạy, theo dõi điểm và tiến độ của học sinh</div>
                    <div style='line-height: 30px;'>- Vào <img src='https://static.eduso.vn//images/book-pen.png?w=20&h=20&mode=crop&format=jpg' style='max-width:20px;max-height:20px;vertical-align: middle;'><b style='color: red;'> ""Quản lý lớp học""</b> chọn <img src='https://static.eduso.vn//images/EditBaiGiang.png?w=20&h=20&mode=crop&format=jpg' style='max-width:20px;max-height:20px;vertical-align: middle;'> trong mục <span style='font-weight:600;color:black'>Tác vụ</span> để thêm bài giảng, xoá bài giảng</div>
                    <div style='line-height: 30px;'>- Vào <img src='https://static.eduso.vn//images/book.png?w=20&h=20&mode=crop&format=jpg' style='max-width:20px;max-height:20px;vertical-align: middle'><b style='color: red;'> ""Tạo bài giảng""</b> để soạn bài giảng mới</div>
                    <div style='line-height: 30px;'>- Vào <img src='https://static.eduso.vn//images/celendar.png?w=20&h=20&mode=crop&format=jpg' style='max-width:20px;max-height:20px;vertical-align: middle'><b style='color: red;'> ""Quản lý lịch dạy""</b> để xem thời khóa biểu và vào lớp học trực tuyến</div>
                    <div style='line-height: 30px;'>- Vào <img src='https://static.eduso.vn//images/file.png?w=20&h=20&mode=crop&format=jpg' style='max-width:20px;max-height:20px;vertical-align: middle'><b style='color: red;'> ""Học liệu""</b>, chọn <span style='font-weight:600;color:black'>Học liệu tương tác</span> để tải thêm xuống lớp học</div>
                </p>
            </div>
        </div>";

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

        //public string DrawChart()
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message, ex);
        //    }
        //    return "";
        //}

        //private async Task SendWeeklyReport(CenterEntity center, DateTime currentTime, bool isTest)
        //{
        //    var startWeek = currentTime.AddDays(-7);
        //    var endWeek = startWeek.AddDays(6).AddHours(23).AddMinutes(59).AddMilliseconds(59);
        //    var headRole = _roleService.GetItemByCode("head-teacher");

        //    var headTeacher = _teacherService.CreateQuery().Find(x => x.IsActive == true && x.Centers.Any(y => y.CenterID == center.ID && y.RoleID == headRole.ID) && x.Email != "huonghl@utc.edu.vn");
        //    var subject = "";
        //    subject += $"Báo cáo kết quả học tập của {center.Name} từ ngày {startWeek.ToString("dd-MM-yyyy")} đến ngày {endWeek.ToString("dd-MM-yyyy")}";
        //    var body = "";
        //    var tbody = "";
        //    tbody = @"<table style='margin-top:20px; width: 100%; border: solid 1px #333; border-collapse: collapse'>
        //                <thead>
        //                    <tr style='font-weight:bold;background-color: bisque'>
        //                        <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:10px'>STT</td>
        //                        <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:150px'>Lớp</td>
        //                        <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'>Sĩ số lớp</td>
        //                        <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'>Chưa đăng nhập</td>
        //                        <td colspan='4' style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Kết quả luyện tập & kiểm tra</td>
        //                        <td colspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Tiến độ</td>
        //                    </tr>
        //                    <tr style='font-weight:bold;background-color: bisque'>
        //                        <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'>8.0 -> 10</td>
        //                        <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'>5.0 -> 7.9</td>
        //                        <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'>1.0 -> 4.9</td>
        //                        <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'>0.0</td>
        //                        <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:150px'>Học liệu chính quy</td>
        //                        <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:150px'>Học liệu chuyên đề</td>
        //                    </tr>
        //                </thead>
        //                <tbody>";
        //    var classesActive = _classService.GetActiveClass4Report(currentTime, center.ID);//lay danh sach lop dang hoat dong
        //    var index = 1;
        //    long totalStudent = 0, totalstChuaVaoLop = 0; ;
        //    long tren8 = 0;
        //    long tren5 = 0;
        //    long tren0 = 0;
        //    long bang0 = 0;
        //    string[] style = { "background-color: aliceblueT", "background-color: whitesmoke" };

        //    foreach (var _class in classesActive.OrderBy(x => x.Name))
        //    {
        //        //Lay danh sach ID hoc sinh trong lop
        //        var studentIds = _studentService.GetStudentsByClassId(_class.ID).Select(t => t.ID).ToList();
        //        var classStudent = studentIds.Count();
        //        totalStudent += classStudent;
        //        //Lay danh sach ID bai hoc duoc mo trong tuan
        //        var activeSchedules = _lessonScheduleService.CreateQuery().Find(o => o.ClassID == _class.ID && o.StartDate <= endWeek && o.EndDate >= startWeek)?.Project(t => new LessonScheduleEntity
        //        {
        //            LessonID = t.LessonID,
        //            ClassSubjectID = t.ClassSubjectID,
        //            StartDate = t.StartDate,
        //            EndDate = t.EndDate
        //        })?.ToList();
        //        var activeLessonIds = activeSchedules?.Select(t => t.LessonID)?.ToList();
        //        //Lay danh sach hoc sinh da hoc cac bai tren trong tuan
        //        var activeProgress = _lessonProgressService.CreateQuery().Find(
        //            x => studentIds.Contains(x.StudentID) && activeLessonIds.Contains(x.LessonID)
        //            && x.LastDate <= endWeek && x.LastDate >= startWeek).ToEnumerable();
        //        var activeStudents = activeProgress.Select(t => t.StudentID).Distinct();
        //        //danh sach bai kiem tra
        //        var examIds = _lessonService.CreateQuery().Find(x => (x.TemplateType == 2 || x.IsPractice == true) && activeLessonIds.Contains(x.ID)).Project(x => x.ID).ToList();
        //        var stChuaVaoLop = classStudent - activeStudents.Count();
        //        var dtren8 = "---";
        //        var dtren5 = "---";
        //        var dtren0 = "---";
        //        var dbang0 = "---";
        //        if (examIds.Count > 0) //co bai kiem tra
        //        {
        //            //ket qua lam bai cua hoc sinh trong lop
        //            var classResult = (from r in activeProgress.Where(t => examIds.Contains(t.LessonID) && t.Tried > 0)
        //                               group r by r.StudentID
        //                               into g
        //                               select new StudentResult
        //                               {
        //                                   StudentID = g.Key,
        //                                   ExamCount = g.Count(),
        //                                   AvgPoint = g.Sum(t => t.LastPoint) / examIds.Count
        //                               }).ToList();
        //            //render ket qua hoc tap
        //            var o8 = classResult.Count(t => t.AvgPoint >= 80);
        //            var o5 = classResult.Count(t => t.AvgPoint >= 50 && t.AvgPoint < 80);
        //            var o0 = classResult.Count(t => t.AvgPoint > 0 && t.AvgPoint < 50);
        //            var e0 = classResult.Count(t => t.AvgPoint == 0);
        //            tren8 += o8;
        //            tren5 += o5;
        //            tren0 += o0;
        //            bang0 += e0;
        //            dtren8 = o8.ToString();
        //            dtren5 = o5.ToString();
        //            dtren0 = o0.ToString();
        //            dbang0 = e0.ToString();
        //        }
        //        totalstChuaVaoLop += stChuaVaoLop;
        //        var groupedSchedule = (from r in activeSchedules
        //                               group r by r.ClassSubjectID into g
        //                               select g.OrderByDescending(t => t.StartDate).FirstOrDefault());

        //        var subjectsInfo = (from r in groupedSchedule
        //                            let lesson = _lessonService.GetItemByID(r.LessonID)
        //                            let classSbj = _classSubjectService.GetItemByID(r.ID)
        //                            select new ClassSubjectInfo
        //                            {
        //                                CourseName = classSbj.CourseName,
        //                                LastLessonTitle = lesson.Title,
        //                                Type = classSbj.TypeClass,
        //                                LastTime = r.StartDate
        //                            })?.ToList();
        //        var standardSbj = "";
        //        var extendSbj = "";
        //        foreach (var sbj in subjectsInfo)
        //        {
        //            var content = "<table>" +
        //                    "<tr>" +
        //                        $"<td style='text-align: left'>{sbj.CourseName} - Bài học cuối: {sbj.LastLessonTitle} ({sbj.LastTime.ToLocalTime().ToString("dd-MM-yyyy")})</td>" +
        //                    "</tr>" +
        //                    "</table>";
        //            if (sbj.Type == CLASSSUBJECT_TYPE.STANDARD)
        //                standardSbj += content;
        //            else
        //                extendSbj += content;
        //        }

        //        if (index % 2 == 0)
        //        {
        //            tbody += $"<tr style='{style[1]}'>";
        //        }
        //        else
        //        {
        //            tbody += $"<tr style='{style[0]}'>";
        //        }

        //        tbody +=
        //            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{index}</td>" +
        //            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{_class.Name}</td>" +
        //            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{classStudent}</td>" +
        //            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{(stChuaVaoLop > 0 ? stChuaVaoLop.ToString() : "---")}</td>" +
        //            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{dtren8}</td>" +
        //            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{dtren5}</td>" +
        //            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{dtren0}</td>" +
        //            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{dbang0}</td>" +
        //            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{standardSbj}</td>" +
        //            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{extendSbj}</td></tr>";
        //    }
        //    tbody += @"<tr style='font-weight: 600'>
        //                    <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>
        //                    <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;text-align: left;font-weight: 600'>Tổng</td>" +
        //            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;font-weight: 600'>{totalStudent}</td>" +
        //            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;font-weight: 600'>{totalstChuaVaoLop}</td>" +
        //            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;font-weight: 600'>{tren8}</td>" +
        //            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;font-weight: 600'>{tren5}</td>" +
        //            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;font-weight: 600'>{tren0}</td>" +
        //            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;font-weight: 600'>{bang0}</td>" +
        //            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;font-weight: 600'></td>" +
        //            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;font-weight: 600'></td>" +
        //        @"</tr>
        //            <tbody>
        //            </table>";
        //    body += tbody;

        //    var toAddress = headTeacher.Project(t => t.Email).ToList();
        //    var bccAddress = new List<string> { "nguyenvanhoa2017602593@gmail.com", "vietphung.it@gmail.com", "huonghl@utc.edu.vn" };
        //    if (isTest)
        //    {
        //        toAddress = new List<string> { "nguyenvanhoa2017602593@gmail.com", "vietphung.it@gmail.com" };
        //        bccAddress = null;
        //    }
        //    await _mailHelper.SendBaseEmail(toAddress, subject, body, MailPhase.WEEKLY_SCHEDULE, null, bccAddress);
        //}

        //public async Task SendWeeklyReport(bool isTest)
        //{
        //    var currentTime = new DateTime(2020, 10, 5);
        //    var centersActive = _centerService.GetActiveCenter(currentTime);//lay co so dang hoat dong
        //    foreach (var center in centersActive)
        //    {
        //        await SendWeeklyReport(center, currentTime, isTest);
        //    }
        //}
        #endregion

        #region Report To Hien
        public IActionResult Report()
        {
            try
            {
                var center = _centerService.CreateQuery().Find(x => x.Abbr.Equals("c3vyvp")).FirstOrDefault();
                if (center == null) return Content("center null");
                var @classes = _classService.CreateQuery().Find(x => x.Center.Equals(center.ID));
                if (@classes.CountDocuments() == 0) return Content("class null");
                var listData = new List<Dictionary<String, Object>>();
                foreach (var @class in classes.ToList().Distinct())
                {
                    if (!@class.ID.Equals("5f5f322def17391d0c623b9c")) continue;
                    List<dateTime> listTime = new List<dateTime>
                    {
                        new dateTime(new DateTime(2020,09,01,0,0,0),new DateTime(2020,09,30,23,59,59)),
                        new dateTime(new DateTime(2020,10,01,0,0,0),new DateTime(2020,10,31,23,59,59)),
                        new dateTime(new DateTime(2020,11,01,0,0,0),new DateTime(2020,11,30,23,59,59)),
                        new dateTime(new DateTime(2020,12,01,0,0,0),new DateTime(2020,12,31,23,59,59)),
                    };
                    
                    foreach(var time in listTime)
                    {
                        var data = GetData4Report(time.StartWeek, time.EndWeek, @class);
                        listData.Add(data);
                    }
                }
                ViewBag.ListData = listData;
                return View();
            }
            catch(Exception ex)
            {
                return Content(ex.Message);
            }
        }

        private Dictionary<String,Object> GetData4Report(DateTime startTime, DateTime endTime, ClassEntity @class)
        {
            var studentIds = _studentService.GetStudentIdsByClassId(@class.ID);
            var activeLessons = _lessonScheduleService.CreateQuery().Find(o => o.ClassID == @class.ID && o.StartDate <= endTime && o.EndDate >= startTime).ToList();
            var activeLessonIds = activeLessons.Select(t => t.LessonID).ToList();

            var lessonProgess = _lessonProgressService.CreateQuery().Find(x => x.ClassID.Equals(@class.ID) && activeLessonIds.Contains(x.LessonID));
            if (lessonProgess.CountDocuments() == 0) { }
            var result = lessonProgess.ToList().Where(x => x.Tried > 0).GroupBy(x => x.StudentID);
            Int32 minpoint8 = 0, minpoint5 = 0, minpoint2 = 0, minpoint0 = 0, chualam = 0;
            foreach (var st in studentIds)
            {
                var item = result.Where(x => x.Key == st).FirstOrDefault();
                if (item == null) chualam++;
                else
                {
                    var point = item.Average(x => x.LastPoint);
                    if (point >= 80) minpoint8++;
                    else if (point >= 50 && point < 80) minpoint5++;
                    else if (point >= 20 && point < 50) minpoint2++;
                    else if (point >= 0 && point < 20) minpoint0++;
                }
            }

            var classResult = new ClassResult 
            {
                ChuaLam = chualam,
                MinPoint0 = minpoint0,
                MinPoint2 = minpoint2,
                MinPoint5 = minpoint5,
                MinPoint8 = minpoint8,
                NameClass = @class.Name,
                TotalStudent = studentIds.Count()
            };

            return new Dictionary<string, object>
            {
                {startTime.Month.ToString(),classResult }
            };
        }
        #endregion
    }

    public class ClassResult
    {
        [JsonProperty("MinPoint8")]
        public int MinPoint8 { get; set; }
        [JsonProperty("MinPoint5")]
        public int MinPoint5 { get; set; }
        [JsonProperty("MinPoint2")]
        public int MinPoint2 { get; set; }
        [JsonProperty("MinPoint0")]
        public int MinPoint0 { get; set; }
        [JsonProperty("ChuaLam")]
        public int ChuaLam { get; set; }
        [JsonProperty("TotalStudent")]
        public int TotalStudent { get; set; }
        [JsonProperty("NameClass")]
        public String NameClass { get; set; }
    }
}