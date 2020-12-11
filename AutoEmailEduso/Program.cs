using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using BaseCoreEmail;
using BaseCustomerEntity.Database;
using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace AutoEmailEduso
{
    class Program
    {
        private static ClassService _classService;
        private static CenterService _centerService;
        private static ClassSubjectService _classSubjectService;
        private static LessonService _lessonService;
        private static LessonScheduleService _scheduleService;
        private static AccountService _accountService;
        private static StudentService _studentService;
        private static TeacherService _teacherService;
        private static SkillService _skillService;
        private static MailHelper _mailHelper;
        private static bool isTest = false;
        private static int count = 0;

        private static LessonScheduleService _lessonScheduleService;
        private static RoleService _roleService;
        private static CourseService _courseService;
        private static LearningHistoryService _learningHistory;
        private static LessonProgressService _lessonProgressService;
        private static ClassProgressService _classProgressService;
        private static ExamService _examService;

        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            _centerService = new CenterService(configuration);
            _classService = new ClassService(configuration);
            _classSubjectService = new ClassSubjectService(configuration);
            _lessonService = new LessonService(configuration);
            _scheduleService = new LessonScheduleService(configuration);
            _accountService = new AccountService(configuration);
            _studentService = new StudentService(configuration);
            _teacherService = new TeacherService(configuration);
            _mailHelper = new MailHelper(configuration);
            _skillService = new SkillService(configuration);
            _lessonScheduleService = new LessonScheduleService(configuration);
            _roleService = new RoleService(configuration);
            _courseService = new CourseService(configuration);
            _learningHistory = new LearningHistoryService(configuration);
            _lessonProgressService = new LessonProgressService(configuration);
            _classProgressService = new ClassProgressService(configuration);
            _examService = new ExamService(configuration);

            isTest = configuration["Test"] == "1";

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            if (!args.Any())
            {
                Console.WriteLine("Processing Schedule...");
                //default
                await SendIncomingLesson();
            }
            else
            {
                switch (args[0])
                {
                    case "SendWeeklyReport":
                        Console.WriteLine("Processing Weekly Report ...");
                        await SendWeeklyReport();
                        break;
                    case "SendWeeklyReportToTeacher":
                        Console.WriteLine("Processing Weekly Report To Teacher ...");
                        await SendWeeklyReportToTeacher();
                        break;
                    case "SendMailReportToCustomerCare":
                        Console.WriteLine("Processing Send Mail Report To Customer Care ...");
                        await SendMailReportToCustomerCare();
                        break;
                    case "SendTeacherScheduleToLesson":
                        Console.WriteLine("Processing Send Teacher Schedule To Lesson ...");
                        await SendTeacherScheduleToLesson();
                        break;
                    case "sendmail":
                        Console.WriteLine("Processing Send Teacher Schedule To Lesson ...");
                        await sendmail();
                        break;
                    default:
                        break;
                }
            }
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
            Console.WriteLine(count + " mail Sent!");

            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";
                eventLog.WriteEntry(count + " mail Sent!", EventLogEntryType.Information, 101, 1);
            }
            //Console.ReadKey();
        }

        public static async Task SendWeeklyReport()
        {
            try
            {
                var currentTime = DateTime.Now;
                var day = currentTime.Day;
                var month = currentTime.Month;
                var year = currentTime.Year;
                var startWeek = new DateTime(year, month, day, 0, 0, 0).AddDays(-7).AddMinutes(1);
                var endWeek = startWeek.AddDays(6).AddHours(23).AddMinutes(58).AddMilliseconds(59);
                var centersActive = _centerService.GetActiveCenter(currentTime);//lay co so dang hoat dong

                foreach (var center in centersActive)
                {
                    //var percent = "";
                    //if (center.Abbr == "c3vyvp")//test truong Vinh Yen
                    {
                        var listTeacherHeader = _teacherService.CreateQuery().Find(x => x.IsActive == true && x.Centers.Any(y => y.CenterID == center.ID)).ToList().FindAll(y => HasRole(y.ID, center.ID, "head-teacher")).ToList();
                        if (listTeacherHeader.Any(x => x.Email == "huonghl@utc.edu.vn"))
                        {
                            listTeacherHeader.RemoveAt(listTeacherHeader.FindIndex(x => x.Email == "huonghl@utc.edu.vn"));
                        }

                        var subject = "";
                        subject += $"B/c tuần {center.Name} ({startWeek.ToString("dd/MM/yyyy")} - {endWeek.ToString("dd/MM/yyyy")})";
                        var body = "";
                        body += @"<table style='margin-top:20px; width: 100%; border: solid 1px #333; border-collapse: collapse'>
                            <thead>
                                        <tr style='font-weight:bold;background-color: bisque'>
                                            <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:10px'>STT</td>
                                            <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:100px'>Lớp</td>
                                            <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'>Sĩ số lớp</td>
                                            <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'>Học sinh chưa học</td>
                                            <td colspan='5' style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Kết quả luyện tập & kiểm tra</td>
                                            <!--<td colspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Tiến độ</td>-->
                                        </tr>
                                        <tr style='font-weight:bold;background-color: bisque'>
                                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'>8.0 -> 10</td>
                                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'>5.0 -> 7.9</td>
                                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'> 2.0 -> 4.9</td>
                                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'> 0.0 -> 1.9</td>
                                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'>Chưa làm</td>
                                        </tr>
                                    </thead>
                                    <tbody>";
                        var tbody = "";
                        tbody += "<tbody>";
                        //var classesActive = _classService.GetActiveClass(currentTime, center.ID);//lay danh sach lop dang hoat dong
                        var classesActive = _classService.GetActiveClass4Report(startWeek, endWeek, center.ID);//lay danh sach lop dang hoat dong
                        var index = 1;
                        long totalStudent = 0, totalstChuaVaoLop = 0, totalActiveStudents = 0;
                        long tren8 = 0;
                        long tren5 = 0;
                        long tren2 = 0;
                        long tren0 = 0;
                        var lasttime1 = new Dictionary<string, string>();
                        var lasttime2 = new Dictionary<string, string>();
                        string[] style = { "background-color: aliceblueT", "background-color: whitesmoke" };
                        if (classesActive.Count() == 0)
                        {
                            continue;
                        }

                        foreach (var _class in classesActive.OrderBy(x => x.Name))
                        {
                            //lay danh sach giao vien trong lop
                            var listNameTeachers = "";
                            if (_class.Members != null)
                            {
                                var members = _class.Members.Where(x => x.Type == ClassMemberType.TEACHER);
                                if (members.Count() > 0)
                                {
                                    foreach (var mem in members)
                                    {
                                        var teacherFName = _teacherService.GetItemByID(mem.TeacherID).FullName.Trim();
                                        //var teacherName = teacherFName.Substring(teacherFName.LastIndexOf(" "));
                                        var str = teacherFName.Split(" ");
                                        var teacherName = str[str.Length - 1];
                                        listNameTeachers += $"{teacherName}, ";
                                    }
                                    listNameTeachers = " - thầy/cô: " + listNameTeachers.Remove(listNameTeachers.LastIndexOf(",")).Trim();
                                }
                                else
                                {
                                    listNameTeachers = "";
                                }
                            }
                            else
                            {
                                listNameTeachers = "";
                            }

                            //Lay danh sach ID hoc sinh trong lop
                            var students = _studentService.GetStudentsByClassId(_class.ID).ToList();
                            var studentIds = students.Select(t => t.ID).ToList();
                            //totalStudent += studentIds.Count();

                            var classStudent = studentIds.Count();
                            totalStudent += classStudent;

                            //Lay danh sach ID bai hoc duoc mo trong tuan

                            var activeLessons = _lessonScheduleService.CreateQuery().Find(o => o.ClassID == _class.ID && o.StartDate <= endWeek && o.EndDate >= startWeek).ToList();

                            var activeLessonIds = activeLessons.Select(t => t.LessonID).ToList();

                            //Lay danh sach hoc sinh da hoc cac bai tren trong tuan
                            var activeProgress = _lessonProgressService.CreateQuery().Find(
                                x => studentIds.Contains(x.StudentID) && activeLessonIds.Contains(x.LessonID)
                                && x.LastDate <= endWeek && x.LastDate >= startWeek).ToEnumerable();

                            //var a = activeProgress.ToList();
                            //Lay danh sach hoc sinh da hoc cac bai tren trong tuan
                            var activeStudents = _lessonProgressService.CreateQuery().Distinct(t => t.StudentID,
                                x => studentIds.Contains(x.StudentID) && activeLessonIds.Contains(x.LessonID)
                                && x.LastDate <= endWeek && x.LastDate >= startWeek).ToEnumerable();

                            var _activeStudents = _lessonProgressService.CreateQuery().Distinct(t => t.StudentID,
                                x => studentIds.Contains(x.StudentID)).ToEnumerable();
                            //totalActiveStudents += activeStudents.Count();

                            var stChuaVaoLop = classStudent - activeStudents.Count();
                            totalstChuaVaoLop += stChuaVaoLop;

                            // danh sach bai kiem tra
                            var examIds = _lessonService.CreateQuery().Find(x => (x.TemplateType == 2 || x.IsPractice == true) && activeLessonIds.Contains(x.ID)).Project(x => x.ID).ToList();
                            //ket qua lam bai cua hoc sinh trong lop
                            var classResult = (from r in activeProgress.Where(t => examIds.Contains(t.LessonID) && t.Tried > 0)
                                               group r by r.StudentID
                                               into g
                                               select new StudentResult
                                               {
                                                   StudentID = g.Key,
                                                   ExamCount = g.Count(),
                                                   AvgPoint = g.Average(t => t.LastPoint),
                                                   StudentName = _studentService.GetItemByID(g.Key)?.FullName,
                                               }).ToList();

                            //render ket qua hoc tap
                            var min8 = classResult.Count(t => t.AvgPoint >= 80);
                            var min5 = classResult.Count(t => t.AvgPoint >= 50 && t.AvgPoint < 80);
                            var min2 = classResult.Count(t => t.AvgPoint >= 20 && t.AvgPoint < 50);
                            var min0 = classResult.Count(t => t.AvgPoint >= 0 && t.AvgPoint < 20);

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
                                $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{_class.Name}{listNameTeachers}</td>" +
                                $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{studentIds.Count()}</td>";
                            if (stChuaVaoLop == 0)
                            {
                                tbody += $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>--</td>";
                            }
                            else
                            {
                                tbody += $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{stChuaVaoLop}</td>";
                            }


                            List<double> points = new List<double>();
                            var classSbjes_active = _classSubjectService.CreateQuery().Find(o => o.StartDate <= endWeek && o.EndDate >= startWeek && o.TotalExams > 0 && o.ClassID == _class.ID).ToEnumerable();//danh sach mon hoc trong lop dang hoat dong

                            var diemtren8 = min8 == 0 ? "--" : min8.ToString();
                            var diemtren5 = min5 == 0 ? "--" : min5.ToString();
                            var diemtren2 = min2 == 0 ? "--" : min2.ToString();
                            var diemtren0 = min0 == 0 ? "--" : min0.ToString();
                            //tbody += $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{diemtren8}</td>" +
                            //    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{diemtren5}</td>" +
                            //    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{diemtren2}</td>" +
                            //    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{diemtren0}</td>" +
                            //    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{studentIds.Count() - min8 - min5 - min2 - min0}</td>";
                            tbody += $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse; background-color:lightgreen'>{diemtren8}</td>" +
                                $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse; background-color:lightblue'>{diemtren5}</td>" +
                                $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse; background-color:#ffff33'>{diemtren2}</td>" +
                                $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse; background-color:#ff454d'>{diemtren0}</td>" +
                                $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse; background-color:rgb(194,194,216)'>{studentIds.Count() - min8 - min5 - min2 - min0}</td>";
                            tren8 += min8;
                            tren5 += min5;
                            tren2 += min2;
                            tren0 += min0;

                            classResult.AddRange(from item in students
                                                 where !classResult.Any(x => x.StudentID == item.ID)
                                                 select new StudentResult { StudentID = item.ID, ExamCount = 0, AvgPoint = 0, StudentName = item.FullName, AvgTimeDoExam = "" });
                            index++;
                        }

                        double tilechuavaolop = ((double)totalstChuaVaoLop / totalStudent) * 100;
                        double tiletren8 = ((double)tren8 / totalStudent) * 100;
                        double tiletren5 = ((double)tren5 / totalStudent) * 100;
                        double tiletren2 = ((double)tren2 / totalStudent) * 100;
                        double tiletren0 = ((double)tren0 / totalStudent) * 100;
                        double tilechualam = ((double)(totalStudent - tren8 - tren5 - tren2 - tren0) / totalStudent) * 100;

                        tbody += @"</td><tr style='font-weight: 600'>
                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>
                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;font-weight: 600'>Tổng</td>" +
                                   $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;font-weight: 600'>{totalStudent}</td>" +
                                   $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;font-weight: 600'>{totalstChuaVaoLop} (<span style='color:red'>{tilechuavaolop.ToString("#0.00")}%</span>)</td>" +
                                   $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;font-weight: 600'>{tren8} (<span style='color:red'>{tiletren8.ToString("#0.00")}%</span>)</td>" +
                                   $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;font-weight: 600'>{tren5} (<span style='color:red'>{tiletren5.ToString("#0.00")}%</span>)</td>" +
                                   $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;font-weight: 600'>{tren2} (<span style='color:red'>{tiletren2.ToString("#0.00")}%</span>)</td>" +
                                   $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;font-weight: 600'>{tren0} (<span style='color:red'>{tiletren0.ToString("#0.00")}%</span>)</td>" +
                                   $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;font-weight: 600'>{totalStudent - tren8 - tren5 - tren2 - tren0} (<span style='color:red'>{tilechualam.ToString("#0.00")}%</span>)</td>" +
                                   @"</tr>
                                <tbody>
                                </table>" + extendTeacher;

                        body += tbody;
                        var PercentActiveStudent = ((double)(totalStudent - totalstChuaVaoLop) / totalStudent * 100);
                        var content = $"<p style='display: none'> Học sinh hoạt động: {PercentActiveStudent.ToString("#0.00")}% " +
                            $"- Hoàn thành kiểm tra: {(tiletren8 + tiletren5 + tiletren2 + tiletren0).ToString("#0.00")}% " +
                            $"(điểm trên 8: {tiletren8.ToString("#0.00")}%, điểm trên 5: {tiletren5.ToString("#0.00")}%)</p> <br> \t" +
                            $"<br><div>Eduso gửi thầy/cô báo cáo <span style='font-weight:600'>tuần</span> của <span style='font-weight:600'>{center.Name}</span> từ {startWeek.ToString("dd/MM/yyyy")} đến {endWeek.ToString("dd/MM/yyyy")}</div>" +
                            $"<div style='font-style: italic;font-size: 12px'>Kết quả học tập là điểm trung bình các bài kiểm tra & luyện tập mà thầy/cô lên lịch giao cho Học sinh làm trong tuần.</div>" +
                            $"<div style='font-style: italic;font-size: 12px'>Kết quả được cập nhật lần cuối lúc {endWeek.ToString("HH:mm - dd/MM/yyyy")}</div>" +
                            body;

                        if (listTeacherHeader.Count() > 1)
                        {
                            foreach (var item in listTeacherHeader)
                            {
                                var toAddress = isTest == true ? new List<string> { "nguyenvanhoa2017602593@gmail.com", "vietphung.it@gmail.com", "k.chee.dinh@gmail.com" } : new List<string> { item.Email };
                                var bccAddress = isTest == true ? null : new List<string> { "nguyenhoa.dev@gmail.com", "vietphung.it@gmail.com", "huonghl@utc.edu.vn" };
                                _ = await _mailHelper.SendBaseEmail(toAddress, subject, $"Kính gửi Thầy/Cô: <span style='font-weight:600'>{item.FullName}</span>," + content, MailPhase.WEEKLY_SCHEDULE, null, bccAddress);

                                //isTest = true;
                                //var toAddress = isTest == true ? new List<string> { "shin.l0v3.ly@gmail.com" } : new List<string> { "shin.l0v3.ly@gmail.com" };
                                //_ = await _mailHelper.SendBaseEmail(toAddress, subject, $"Kính gửi Thầy/Cô: <span style='font-weight:600'>{item.FullName}</span>," + content, MailPhase.WEEKLY_SCHEDULE, null, toAddress);
                            }
                        }
                        else if (listTeacherHeader.Count() == 1)
                        {
                            var toAddress = isTest == true ? new List<string> { "nguyenvanhoa2017602593@gmail.com", "vietphung.it@gmail.com", "k.chee.dinh@gmail.com" } : new List<string> { listTeacherHeader[0].Email };
                            var bccAddress = isTest == true ? null : new List<string> { "nguyenhoa.dev@gmail.com", "vietphung.it@gmail.com", "huonghl@utc.edu.vn" };
                            _ = await _mailHelper.SendBaseEmail(toAddress, subject, $"Kính gửi Thầy/Cô: <span style='font-weight:600'>{listTeacherHeader[0].FullName}</span>," + content, MailPhase.WEEKLY_SCHEDULE, null, bccAddress);
                            //isTest = true;
                            //var toAddress = isTest == true ? new List<string> { "shin.l0v3.ly@gmail.com"} : new List<string> { "shin.l0v3.ly@gmail.com" };
                            //_ = await _mailHelper.SendBaseEmail(toAddress, subject, $"Kính gửi Thầy/Cô: <span style='font-weight:600'>{listTeacherHeader[0].FullName}</span>," +content , MailPhase.WEEKLY_SCHEDULE, null, toAddress);
                        }
                        else
                        {
                            Console.WriteLine($"{listTeacherHeader.Count()}");
                        }
                    }
                    Console.WriteLine($"Send Weekly Report To {center.Name} Is Done!");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static async Task SendWeeklyReportToTeacher()
        {
            try
            {
                var currentTime = DateTime.Now;
                var day = currentTime.Day;
                var month = currentTime.Month;
                var year = currentTime.Year;
                var startWeek = new DateTime(year, month, day, 0, 0, 0).AddDays(-7);
                var endWeek = startWeek.AddDays(6).AddHours(23).AddMinutes(59).AddMilliseconds(59);

                var centersActive = _centerService.GetActiveCenter(currentTime);//lay co so dang hoat dong
                //centersActive.ToList().Remove(centersActive.ToList().Find(x => x.Abbr == "eduso"));
                foreach (var center in centersActive)
                {
                    //var percent = "";
                    //if (center.Abbr != "eduso")//test truong Vinh Yen
                    //if (center.Abbr == "c3vyvp")//test truong Vinh Yen
                    {
                        var classesActive = _classService.GetActiveClass4Report(startWeek, endWeek, center.ID);//lay danh sach lop dang hoat dong
                        if (classesActive.Count() == 0)
                        {
                            continue;
                        }

                        foreach (var _class in classesActive.OrderBy(x => x.Name))
                        {
                            if (_class.Members == null)
                            {
                                continue;
                            }
                            //Lay danh sach ID hoc sinh trong lop
                            var students = _studentService.GetStudentsByClassId(_class.ID).ToList();
                            var studentIds = students.Select(t => t.ID).ToList();
                            var classStudent = studentIds.Count();
                            if (classStudent == 0)
                            {
                                continue;
                            }

                            //Lay danh sach ID bai hoc duoc mo trong tuan
                            var activeLessons = _lessonScheduleService.CreateQuery().Find(o => o.ClassID == _class.ID && o.StartDate <= endWeek && o.EndDate >= startWeek).ToList();
                            var activeLessonIds = activeLessons.Select(t => t.LessonID).ToList();

                            //Lay danh sach hoc sinh da hoc cac bai tren trong tuan
                            var activeProgress = _lessonProgressService.CreateQuery().Find(
                                x => studentIds.Contains(x.StudentID) && activeLessonIds.Contains(x.LessonID)
                                && x.LastDate <= endWeek && x.LastDate >= startWeek).ToEnumerable();

                            var activeStudents = _lessonProgressService.CreateQuery().Distinct(t => t.StudentID,
                                x => studentIds.Contains(x.StudentID) && activeLessonIds.Contains(x.LessonID)
                                && x.LastDate <= endWeek && x.LastDate >= startWeek).ToEnumerable();

                            // danh sach bai kiem tra
                            var examIds = _lessonService.CreateQuery().Find(x => (x.TemplateType == 2 || x.IsPractice == true) && activeLessonIds.Contains(x.ID)).Project(x => x.ID).ToList();
                            //var examIds = _lessonService.CreateQuery().Find(x => (x.TemplateType == 1 || x.IsPractice == true) && activeLessonIds.Contains(x.ID)).Project(x => x.ID).ToList();
                            //var exams = _examService.CreateQuery().Find(x => examIds.Contains(x.ID)).ToList();

                            var exams = (from e in _examService.CreateQuery().Find(x => examIds.Contains(x.LessonID)).ToList()
                                         group e by e.StudentID
                                         into ge
                                         let hours = (int)ge.Average(x => x.Updated.Subtract(x.Created).TotalHours)
                                         let minutes = (int)ge.Average(x => x.Updated.Subtract(x.Created).TotalMinutes) - 60 * hours
                                         let seconds = ge.Average(x => x.Updated.Subtract(x.Created).TotalSeconds) - 60 * minutes - 60 * hours * 60
                                         select new
                                         {
                                             StudentID = ge.Key,
                                             //AvgTimeDoExam = ((int)ge.Average(x => x.Updated.Subtract(x.Created).Hours) == 0 ? "" : $"{Math.Abs((int)ge.Average(x => x.Updated.Subtract(x.Created).Hours))} giờ ") + ((int)ge.Average(x => x.Updated.Subtract(x.Created).Minutes) == 0 ? "0 phút" : $"{Math.Abs((int)ge.Average(x => x.Updated.Subtract(x.Created).Minutes))} phút"),
                                             AvgTimeDoExam = (hours == 0 ? "" : $"{hours}h ") + (minutes == 0 ? "" : $"{minutes}p ") + (seconds == 0 ? "0s" : $"{Math.Round(seconds)}s"),
                                             CompletedLesson = ge.ToList().Select(x => x.LessonID).Distinct().Count()
                                         }).ToList();

                            //ket qua lam bai cua hoc sinh trong lop
                            var classResult = (from r in activeProgress.Where(t => examIds.Contains(t.LessonID) && t.Tried > 0)
                                               group r by r.StudentID
                                               into g
                                               let _AvgTimeDoExam = exams.Count == 0 ? "0s" : exams.Where(x => x.StudentID == g.Key)?.FirstOrDefault().AvgTimeDoExam
                                               let _CompletedLesson = exams.Where(x => x.StudentID == g.Key).FirstOrDefault().CompletedLesson
                                               select new StudentResult
                                               {
                                                   StudentID = g.Key,
                                                   ExamCount = g.Count() == 0 ? 0 : g.Count(),
                                                   AvgPoint = g.Average(t => t.LastPoint),
                                                   StudentName = _studentService.GetItemByID(g.Key)?.FullName.Trim(),
                                                   AvgTimeDoExam = _AvgTimeDoExam,
                                                   CompletedLesson = _CompletedLesson,
                                                   TotalLesson = activeLessonIds.Count
                                               }).ToList();

                            foreach (var item in students)
                            {
                                var studentResult = classResult.Where(x => x.StudentID == item.ID).FirstOrDefault();
                                if (studentResult == null)
                                {
                                    studentResult = new StudentResult { StudentID = item.ID, ExamCount = 0, AvgPoint = 0, StudentName = item.FullName.Trim(), AvgTimeDoExam = "--", CompletedLesson = 0, TotalLesson = activeLessonIds.Count() };
                                    classResult.Add(studentResult);
                                }
                            }

                            List<StudentResult> _classResult = new List<StudentResult>();
                            foreach (var item in students)
                            {
                                var studentResult = classResult.Where(x => x.StudentID == item.ID).FirstOrDefault();
                                _classResult.Add(studentResult);
                            }



                            await SendContentToTeacher(_class, _classResult, startWeek, endWeek, center.Name);
                            Console.WriteLine($"Send to class {_class.Name} - { center.Name} is done");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static async Task SendMailReportToCustomerCare()
        {
            DateTime currentTime = DateTime.Now;
            var day = currentTime.Day;
            var month = currentTime.Month;
            var year = currentTime.Year;
            var startWeek = new DateTime(year, month, day, 0, 0, 0).AddDays(-7);
            var endWeek = startWeek.AddDays(6).AddHours(23).AddMinutes(59).AddMilliseconds(59);
            List<ReportViewModal> listReport = new List<ReportViewModal>();

            var centersActive = _centerService.GetActiveCenter(currentTime);
            foreach (var center in centersActive)
            {
                //if (center.Abbr.Equals("utc2"))
                {
                    Int32 TotalStudents = 0, TotalInactiveStudents = 0, TotalMinPoint8 = 0, TotalMinPoint5 = 0, TotalMinPoint2 = 0, TotalMinPoint0 = 0, TotalDontWork = 0;
                    String ClassName4ReportPoint = "";//Ten lop co ti le hoc sinh dat diem 0 -> 4.9 > 30%
                    String ClassName4ReportInactive = "";//ten lop co so hoc sinh chua lam > 10
                    var classesActive = _classService.GetActiveClass4Report(startWeek, endWeek, center.ID);//lay danh sach lop dang hoat dong
                    if (classesActive.Count() == 0)
                    {
                        continue;
                    }

                    //Int32 totalstd = (Int32)_studentService.CreateQuery().Find(x => x.Centers.Contains(center.ID)).CountDocuments();
                    Int32 totalstd = 0;

                    foreach (var @class in classesActive)
                    {
                        //Lay danh sach ID hoc sinh trong lop
                        var students = _studentService.GetStudentsByClassId(@class.ID).ToList();
                        var studentIds = students.Select(t => t.ID).ToList();
                        totalstd += studentIds.Count();
                        //Lay danh sach ID bai hoc duoc mo trong tuan
                        var activeLessons = _lessonScheduleService.CreateQuery().Find(o => o.ClassID == @class.ID && o.StartDate <= endWeek && o.EndDate >= startWeek).ToList();
                        var activeLessonIds = activeLessons.Select(t => t.LessonID).ToList();
                        //Lay danh sach hoc sinh da hoc cac bai tren trong tuan
                        var activeProgress = _lessonProgressService.CreateQuery().Find(
                            x => studentIds.Contains(x.StudentID) && activeLessonIds.Contains(x.LessonID)
                            && x.LastDate <= endWeek && x.LastDate >= startWeek).ToEnumerable();
                        var activeStudents = _lessonProgressService.CreateQuery().Distinct(t => t.StudentID,
                                x => studentIds.Contains(x.StudentID)).ToEnumerable();
                        // danh sach bai kiem tra
                        var examIds = _lessonService.CreateQuery().Find(x => (x.TemplateType == 2 || x.IsPractice == true) && activeLessonIds.Contains(x.ID)).Project(x => x.ID).ToList();
                        //var examIds = _lessonService.CreateQuery().Find(x => (x.TemplateType == 1 || x.IsPractice == true) && activeLessonIds.Contains(x.ID)).Project(x => x.ID).ToList();

                        //ket qua lam bai cua hoc sinh trong lop
                        var classResult = (from r in activeProgress.Where(t => examIds.Contains(t.LessonID) && t.Tried > 0)
                                           group r by r.StudentID
                                           into g
                                           select new StudentResult
                                           {
                                               StudentID = g.Key,
                                               ExamCount = g.Count(),
                                               AvgPoint = g.Average(t => t.LastPoint),
                                               StudentName = _studentService.GetItemByID(g.Key)?.FullName,
                                           }).ToList();

                        //render ket qua hoc tap
                        var min8 = classResult.Count(t => t.AvgPoint >= 80);
                        var min5 = classResult.Count(t => t.AvgPoint >= 50 && t.AvgPoint < 80);
                        var min2 = classResult.Count(t => t.AvgPoint >= 20 && t.AvgPoint < 50);
                        var min0 = classResult.Count(t => t.AvgPoint >= 0 && t.AvgPoint < 20);

                        //render nhung thu can lay
                        Int32 siso = studentIds.Count();
                        Int32 hocsinhchuahoc = siso - activeStudents.Count();
                        Int32 point0to49 = min0 + min2;
                        Int32 chualam = siso - min0 - min2 - min5 - min8;

                        //render ti le %
                        Double percentPoint0To49 = Math.Round((((Double)point0to49 / siso) * 100), 2);
                        Double percentChualam = Math.Round((((Double)chualam / siso) * 100), 2);

                        if (percentChualam > 30)
                        {
                            ClassName4ReportInactive += $"{@class.Name}, ";
                        }
                        if (percentPoint0To49 > 30)
                        {
                            ClassName4ReportPoint += $"{@class.Name}, ";
                        }

                        TotalStudents += siso;
                        TotalInactiveStudents += hocsinhchuahoc;
                        TotalMinPoint8 += min8;
                        TotalMinPoint5 += min5;
                        TotalMinPoint2 += min2;
                        TotalMinPoint0 += min0;
                        TotalDontWork += chualam;

                    }

                    if (!String.IsNullOrEmpty(ClassName4ReportPoint))
                    {
                        ClassName4ReportPoint = ClassName4ReportPoint.Remove(ClassName4ReportPoint.LastIndexOf(",")) + ".";
                    }
                    if (!String.IsNullOrEmpty(ClassName4ReportInactive))
                    {
                        ClassName4ReportInactive = ClassName4ReportInactive.Remove(ClassName4ReportInactive.LastIndexOf(",")) + ".";
                    }

                    ReportViewModal data = new ReportViewModal
                    {
                        CenterName = center.Name,
                        //TotalStudents = TotalStudents,
                        TotalStudents = (int)totalstd,
                        TotalInactiveStudents = TotalInactiveStudents,
                        MinPoint8 = TotalMinPoint8,
                        MinPoint5 = TotalMinPoint5,
                        MinPoint2 = TotalMinPoint2,
                        MinPoint0 = TotalMinPoint0,
                        TotalDontWork = TotalDontWork,
                        ClassName4ReportPoint = ClassName4ReportPoint,
                        ClassName4ReportInactive = ClassName4ReportInactive
                    };

                    listReport.Add(data);
                }
            }

            String body = await SendContentToCustomerCare(listReport);
            String subject = $"B/c Tổng hợp các cơ sở Tuần ({startWeek.ToString("dd/MM/yyyy")} - {endWeek.ToString("dd/MM/yyyy")})";
            String content = $"<div style='font-style: italic;font-size: 12px'>Kết quả học tập là điểm trung bình các bài kiểm tra & luyện tập mà thầy/cô lên lịch giao cho Học sinh làm trong tuần.</div>" +
                        $"<div style='font-style: italic;font-size: 12px'>Kết quả được cập nhật lần cuối lúc {endWeek.ToString("HH:mm - dd/MM/yyyy")}</div>" +
                        $"{body}";

            List<String> toAddress = isTest ? new List<String> { "nguyenvanhoa2017602593@gmail.com", "k.chee.dinh@gmail.com" } : new List<String> { "nguyenhoa.dev@gmail.com", "kchidinh@gmail.com", "buihong9885@gmail.com", "huonghl@utc.edu.vn", "vietphung.it@gmail.com" };
            _ = await _mailHelper.SendBaseEmail(toAddress, subject, content, MailPhase.WEEKLY_SCHEDULE);
            Console.WriteLine("Send To Team Customer Care is Done");
        }

        private static async Task SendContentToTeacher(ClassEntity @class, List<StudentResult> studentResults, DateTime startweek, DateTime endWeek, String centerName)
        {
            if (studentResults.Count > 0 && @class != null)
            {
                var members = @class.Members.Where(x => x.Type == ClassMemberType.TEACHER);
                var listTeacher = new List<TeacherEntity>();
                foreach (var mem in members.ToList())
                {
                    var _teacher = _teacherService.GetItemByID(mem.TeacherID);

                    if (!listTeacher.Any(x => x.Email == _teacher.Email))
                    {
                        listTeacher.Add(_teacher);
                    }
                }

                var teacher = _teacherService.GetItemByID(@class.TeacherID);

                var thead = @"<thead>
                        <tr style='font-weight:bold;background-color:bisque'>
                            <th rowspan='2' style='text-align:center;border:solid 1px #333;border-collapse:collapse;width:10px'>STT</th>
                            <th rowspan='2' style='text-align:center;border:solid 1px #333;border-collapse:collapse;width:100px'>Họ và tên</th>
                            <th colspan='6' style='text-align:center;border:solid 1px #333;border-collapse:collapse'>Kết quả luyện tập và kiểm tra</th>
                        </tr>
                        <tr style='font-weight:bold;background-color:bisque'>
                            <th style='text-align:center;border:solid 1px #333;border-collapse:collapse;width:50px'>8.0 - 10.0</th>
                            <th style='text-align:center;border:solid 1px #333;border-collapse:collapse;width:50px'>5.0 - 7.9</th>
                            <th style='text-align:center;border:solid 1px #333;border-collapse:collapse;width:50px'>2.0 - 4.9</th>
                            <th style='text-align:center;border:solid 1px #333;border-collapse:collapse;width:50px'>0.0 - 1.9</th>
                            <th style='text-align:center;border:solid 1px #333;border-collapse:collapse;width:50px'>Bài đã làm</th>
                            <th style='text-align:center;border:solid 1px #333;border-collapse:collapse;width:50px'>Thời lượng làm bài trung bình</th>
                        </tr>
                    </thead>";
                var tbody = "";

                for (int i = 0; i < studentResults.Count; i++)
                {
                    var student = studentResults[i];
                    var point = student.AvgPoint / 10;
                    var avgTimeDoExam = student.AvgTimeDoExam;
                    var completedLesson = student.CompletedLesson;
                    var totalLesson = student.TotalLesson;
                    tbody += "<tr>" +
                                $"<td style='text-align:center;border:solid 1px #333;border-collapse:collapse'>{i + 1}</td>" +
                                $"<td style='text-align:center;border:solid 1px #333;border-collapse:collapse;vertical-align: middle'>{student.StudentName}</td>";
                    if (point >= 8)
                    {
                        tbody += $"<td style='text-align:center;border:solid 1px #333;border-collapse:collapse;color: black;background-color:lightgreen'>{point.ToString("#0.00")}</td>" +
                                $"<td style='text-align:center;border:solid 1px #333;border-collapse:collapse;color: black'></td>" +
                                $"<td style='text-align:center;border:solid 1px #333;border-collapse:collapse;color: black'></td>" +
                                $"<td style='text-align:center;border:solid 1px #333;border-collapse:collapse;color: black'></td>";
                    }
                    else if (point < 8 && point >= 5)
                    {
                        tbody += $"<td style='text-align:center;border:solid 1px #333;border-collapse:collapse;color: black'></td>" +
                                $"<td style='text-align:center;border:solid 1px #333;border-collapse:collapse;color: black;background-color:lightblue;'>{point.ToString("#0.00")}</td>" +
                                $"<td style='text-align:center;border:solid 1px #333;border-collapse:collapse;color: black'></td>" +
                                $"<td style='text-align:center;border:solid 1px #333;border-collapse:collapse;color: black'></td>";
                    }
                    else if (point < 5 && point >= 2)
                    {
                        tbody += $"<td style='text-align:center;border:solid 1px #333;border-collapse:collapse;color: black'></td>" +
                                    $"<td style='text-align:center;border:solid 1px #333;border-collapse:collapse;color: black'></td>" +
                                    $"<td style='text-align:center;border:solid 1px #333;border-collapse:collapse;color: black;background-color:#FFFF33;'>{point.ToString("#0.00")}</td>" +
                                    $"<td style='text-align:center;border:solid 1px #333;border-collapse:collapse;color: black'></td>";
                    }
                    else if (point < 2 && point >= 0)
                    {
                        if (avgTimeDoExam.Equals("--"))
                        {
                            tbody += $"<td style='text-align:center;border:solid 1px #333;border-collapse:collapse;color: black'></td>" +
                                    $"<td style='text-align:center;border:solid 1px #333;border-collapse:collapse;color: black'></td>" +
                                    $"<td style='text-align:center;border:solid 1px #333;border-collapse:collapse;color: black'></td>" +
                                    $"<td style='text-align:center;border:solid 1px #333;border-collapse:collapse;color: black;'>--</td>";
                        }
                        else
                        {
                            tbody += $"<td style='text-align:center;border:solid 1px #333;border-collapse:collapse;color: black'></td>" +
                                    $"<td style='text-align:center;border:solid 1px #333;border-collapse:collapse;color: black'></td>" +
                                    $"<td style='text-align:center;border:solid 1px #333;border-collapse:collapse;color: black'></td>" +
                                    $"<td style='text-align:center;border:solid 1px #333;border-collapse:collapse;color: black;background-color: #ff454d;'>{point.ToString("#0.00")}</td>";
                        }
                    }
                    else
                    {
                        tbody += $"<td style='text-align:center;border:solid 1px #333;border-collapse:collapse;color: black'></td>" +
                                $"<td style='text-align:center;border:solid 1px #333;border-collapse:collapse;color: black'></td>" +
                                $"<td style='text-align:center;border:solid 1px #333;border-collapse:collapse;color: black'></td>" +
                                $"<td style='text-align:center;border:solid 1px #333;border-collapse:collapse;color: black'></td>";
                    }
                    tbody += $"<td style='text-align:center;border:solid 1px #333;border-collapse:collapse'>{completedLesson}/{totalLesson}</td>" +
                            $"<td style='text-align:center;border:solid 1px #333;border-collapse:collapse'>{avgTimeDoExam}</td>" +
                            $"</tr>";
                }

                var body = @"<body>
                                <table style='margin-top:20px;width:100%;border:solid 1px #333;border-collapse:collapse'>" +
                                thead +
                                $"<tbody>{tbody}</tbody>" +
                                @"</table>
                            </body>";

                var content = $"<br><div>Eduso gửi thầy/cô báo cáo <span style='font-weight:600'>tuần</span> của <span style='font-weight:600'>{@class.Name}</span> từ {startweek.ToString("dd/MM/yyyy")} đến {endWeek.ToString("dd/MM/yyyy")}</div>" +
                                $"<div style='font-style: italic;font-size: 12px'>Kết quả học tập là điểm trung bình các bài kiểm tra & luyện tập mà thầy/cô lên lịch giao cho Học sinh làm trong tuần.</div>" +
                                $"<div style='font-style: italic;font-size: 12px'>Kết quả được cập nhật lần cuối lúc {endWeek.ToString("HH:mm - dd/MM/yyyy")}</div>" +
                                $"{body}" +
                                $"{extendTeacher}";

                if (listTeacher.Count > 1)
                {
                    foreach (var item in listTeacher)
                    {
                        var toAddress = isTest == true ? new List<string> { "nguyenvanhoa2017602593@gmail.com", "vietphung.it@gmail.com", "k.chee.dinh@gmail.com" } : new List<string> { item.Email };
                        var bccAddress = isTest == true ? null : new List<string> { "nguyenhoa.dev@gmail.com", "vietphung.it@gmail.com" };
                        _ = await _mailHelper.SendBaseEmail(toAddress, Subject(item.FullName, @class.Name, centerName, startweek, endWeek), $"<div>Kính gửi Thầy/Cô: <span style='font-weight:600'>{item.FullName}</span>,</div>" + content, MailPhase.WEEKLY_SCHEDULE, null, bccAddress);

                        //isTest = true;
                        //var toAddress = isTest == true ? new List<string> { "shin.l0v3.ly@gmail.com" } : new List<string> { "shin.l0v3.ly@gmail.com" };
                        //_ = await _mailHelper.SendBaseEmail(toAddress, Subject(item.FullName, @class.Name, centerName, startweek, endWeek), $"<div>Kính gửi Thầy/Cô: <span style='font-weight:600'>{item.FullName}</span>,</div>" + content, MailPhase.WEEKLY_SCHEDULE, null, toAddress);
                    }
                }
                else if (listTeacher.Count == 1)
                {
                    var toAddress = isTest == true ? new List<string> { "nguyenvanhoa2017602593@gmail.com", "vietphung.it@gmail.com", "k.chee.dinh@gmail.com" } : new List<string> { listTeacher[0].Email };
                    var bccAddress = isTest == true ? null : new List<string> { "nguyenhoa.dev@gmail.com", "vietphung.it@gmail.com" };
                    _ = await _mailHelper.SendBaseEmail(toAddress, Subject(listTeacher[0].FullName, @class.Name, centerName, startweek, endWeek), $"<div>Kính gửi Thầy/Cô: <span style='font-weight:600'>{listTeacher[0].FullName}</span>,</div>" + content, MailPhase.WEEKLY_SCHEDULE, null, bccAddress);

                    //isTest = true;
                    //var toAddress = isTest == true ? new List<string> { "shin.l0v3.ly@gmail.com" } : new List<string> { "shin.l0v3.ly@gmail.com" };
                    //_ = await _mailHelper.SendBaseEmail(toAddress, Subject(listTeacher[0].FullName, @class.Name, centerName, startweek, endWeek), $"<div>Kính gửi Thầy/Cô: <span style='font-weight:600'>{listTeacher[0].FullName}</span>,</div>" + content, MailPhase.WEEKLY_SCHEDULE, null, toAddress);
                }
                else
                {
                    Console.WriteLine($"Lop {@class.Name} khong co member");
                }
            }
        }

        private static async Task<String> SendContentToCustomerCare(List<ReportViewModal> dataReport)
        {
            String[] style = { "background-color: aliceblueT", "background-color: whitesmoke" };
            String body = "";
            String thead = "";
            String tbody = "<tbody>";

            thead += @"<thead>
                            <tr style='font-weight:bold;background-color: bisque'>
                                <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;'>STT</td>
                                <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;'>Tên cơ sở</td>
                                <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;'>Tổng số học sinh</td>
                                <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;'>Tổng số học sinh chưa đăng nhập</td>
                                <td colspan='5' style='text-align:center; border: solid 1px #333; border-collapse: collapse;'>Kết quả luyện tập & kiểm tra</td>
                                <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;'>Lớp có tỉ lệ điểm dưới 5 lớn hơn 30%</td>
                                <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;'>Lớp có số học sinh chưa làm lớn hơn 30%</td>
                            </tr>
                            <tr style='font-weight:bold;background-color: bisque'>
                                <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;'>8.0 đến 10</td>
                                <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;'>5.0 đến 7.9</td>
                                <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;'>2.0 đến 4.9</td>
                                <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;'>0 đến 1.9</td>
                                <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;'>Chưa làm</td>
                            </tr>
                        </thead>";
            Int32 indexRow = 1;
            foreach (var data in dataReport)
            {
                if (indexRow % 2 == 0)
                {
                    tbody += $"<tr style='{style[1]}'>";
                }
                else
                {
                    tbody += $"<tr style='{style[0]}'>";
                }
                tbody += $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:10px'>{indexRow}</td>" +
                            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:150px'>{data.CenterName}</td> " +
                            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:75px'>{data.TotalStudents}</td> " +
                            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:75px;background-color:rgb(179,179,206)'>{data.TotalInactiveStudents}</td> " +
                            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:75px;background-color:lightgreen'>{data.MinPoint8}</td> " +
                            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:75px;background-color:lightblue'>{data.MinPoint5}</td> " +
                            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:75px;background-color:#ffff33'>{data.MinPoint2}</td> " +
                            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:75px;background-color:#ff454d'>{data.MinPoint0}</td> " +
                            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:75px;background-color:rgb(194,194,216)'>{data.TotalDontWork}</td> " +
                            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:25%'>{data.ClassName4ReportPoint}</td> " +
                            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:25%'>{data.ClassName4ReportInactive}</td> " +
                        $"</tr>";
                indexRow++;
            }
            tbody += "</tbody>";
            body += $"<body><table style='margin-top:20px; width: 100%; border: solid 1px #333; border-collapse: collapse'>{thead}{tbody}</table></body>";

            return body;
        }

        public static async Task SendIncomingLesson()
        {
            var currentTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0).AddHours(1).ToUniversalTime();
            Console.WriteLine(currentTime);
            var activeClasses = _classService.GetActiveClass(time: currentTime, Center: null).ToList();
            var period = 60;
            if (activeClasses != null && activeClasses.Count() > 0)
            {
                foreach (var @class in activeClasses)
                {
                    var activeSchedules = _scheduleService.GetIncomingSchedules(time: currentTime, period: period, ClassID: @class.ID)
                        .OrderBy(t => t.ClassID)
                        .ThenBy(t => t.ClassSubjectID)
                        .ThenBy(t => t.StartDate).ToList();
                    if (activeSchedules != null && activeSchedules.Count() > 0)
                    {
                        string subjectID = "";
                        string classID = "";
                        ClassEntity currentClass = null;
                        ClassSubjectEntity currentSubject = null;
                        TeacherEntity currentTeacher = null;
                        var studentList = new List<StudentEntity>();
                        var schedules = new List<ScheduleView>();
                        var center = new CenterEntity();
                        foreach (var schedule in activeSchedules)
                        {
                            if (classID != schedule.ClassID)
                            {
                                studentList = _studentService.GetStudentsByClassId(schedule.ClassID).ToList();
                                if (studentList == null || studentList.Count == 0) // no student in class
                                    continue;
                                currentClass = _classService.GetItemByID(schedule.ClassID);
                                classID = schedule.ClassID;
                                center = _centerService.GetItemByID(currentClass.Center);
                            }
                            if (subjectID != schedule.ClassSubjectID)//change subject
                            {
                                if (!string.IsNullOrEmpty(subjectID))
                                {
                                    var skill = _skillService.GetItemByID(currentSubject.ID);
                                    count++;
                                    //Send Mail for lastest class subject
                                    _ = SendStudentSchedule(schedules, currentTeacher, studentList, currentClass, skill.Name, subjectID, currentTime, currentTime.AddMinutes(period), center);
                                    _ = SendTeacherSchedule(schedules, currentTeacher, currentClass, skill.Name, subjectID, currentTime, currentTime.AddMinutes(period), center);
                                }
                                subjectID = schedule.ClassSubjectID;
                                currentSubject = _classSubjectService.GetItemByID(subjectID);
                                currentTeacher = _teacherService.GetItemByID(currentSubject.TeacherID);
                                var newsubject = _classSubjectService.GetItemByID(schedule.ClassSubjectID);
                                schedules = new List<ScheduleView>();
                            }
                            schedules.Add(new ScheduleView(schedule)
                            {
                                LessonName = _lessonService.GetItemByID(schedule.LessonID).Title
                            });
                        }
                        //send last Class subject
                        if (!string.IsNullOrEmpty(subjectID))
                        {
                            var skill = _skillService.GetItemByID(currentSubject.SkillID);
                            count++;
                            //Send Mail for lastest class subject
                            _ = SendStudentSchedule(schedules, currentTeacher, studentList, currentClass, skill.Name, subjectID, currentTime, currentTime.AddMinutes(period), center);
                            _ = SendTeacherSchedule(schedules, currentTeacher, currentClass, skill.Name, subjectID, currentTime, currentTime.AddMinutes(period), center);
                        }
                    }
                }
            }
        }

        private static async Task SendTeacherScheduleToLesson()
        {
            try
            {
                //DateTime currentTime = DateTime.Now;
                DateTime currentTime =new DateTime(2020,12,13,23,59,00);
                var day = currentTime.Day;
                var month = currentTime.Month;
                var year = currentTime.Year;
                var startWeek = new DateTime(year, month, day, 0, 0, 0).AddDays(-7).AddMinutes(1);
                var endWeek = startWeek.AddDays(6).AddHours(23).AddMinutes(58).AddMilliseconds(59);

                var centersActive = _centerService.GetActiveCenter(currentTime).OrderBy(x => x.Name);
                if (centersActive.Count() > 0)
                    for (int i = 0; i < centersActive.Count(); i++)
                    {
                        var center = centersActive.ElementAtOrDefault(i);
                        if (!center.Abbr.Equals("c3vyvp"))
                        {
                            continue;
                        }
                        var classesActive = _classService.GetActiveClass4Report(startWeek, endWeek, center.ID);

                        var test =new Dictionary<String, String>();

                        if (classesActive.Count() == 0) continue;
                        for (int j = 0; j < classesActive.Count(); j++)
                        {
                            var @class = classesActive.ElementAtOrDefault(j);
                            var activeLessons = _lessonScheduleService.CreateQuery().Find(o => o.ClassID == @class.ID && o.StartDate <= endWeek && o.EndDate >= startWeek).ToList();
                            String className = @class.Name.ToLower().Contains("lớp") ? @class.Name : $"Lớp {@class.Name}";

                            //if (activeLessons.Count > 0)
                            //{
                            //    var x = "a";
                            //}
                            if (@class.Members == null) continue;
                            var listTeachers = @class.Members.Where(x => x.Type == ClassMemberType.TEACHER);
                            if (activeLessons.Count == 0)
                            {
                                String subject = $"Nhắc đặt lịch học Tuần lớp {className} ({startWeek.ToString("dd/MM/yyyy")} - {endWeek.ToString("dd/MM/yyyy")})";
                                foreach (var teacher in listTeachers)
                                {
                                    //var a = _classSubjectService.CreateQuery().Find(x => x.ClassID == @class.ID && x.TeacherID == teacher.TeacherID).ToList();
                                    var inforTeacher = _teacherService.GetItemByID(teacher.TeacherID);
                                    String body = "Xin chào Thầy/Cô: " + inforTeacher.FullName + ",";
                                    //foreach(var str in a)
                                    //{
                                    //    body += $"<p>Môn học {str.CourseName} chưa có lịch học trong Tuần từ {startWeek.ToString("dd/MM/yyyy")} đến {endWeek.ToString("dd/MM/yyyy")}</p>";
                                    //}
                                    body += $"<br><p>{className} chưa được đặt lịch học trong Tuần từ {startWeek.ToString("dd/MM/yyyy")} đến {endWeek.ToString("dd/MM/yyyy")}</p>";
                                    body += "<p>Thầy/Cô truy cập vào hệ thống" +
                                        " -> Chọn <img src='https://static.eduso.vn//images/book-pen.png?w=20&h=20&mode=crop&format=jpg' style='max-width:20px;max-height:20px;vertical-align: middle;'><b style='color: red;'> \"Quản lý lớp học\"</b>" +
                                        " -> Chọn lớp cần đặt lịch học -> tạo lịch dạy và đặt lịch học tuần cho môn học.</p>";
                                    body += extendTeacher;

                                var toAddress = isTest ? new List<string> { "nguyenvanhoa2017602593@gmail.com" } : new List<string> { inforTeacher.Email };
                                //_ = await _mailHelper.SendBaseEmail(toAddress, subject, body, MailPhase.WEEKLY_SCHEDULE);
                                Console.WriteLine("Send to " + inforTeacher.FullName + " is done");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static async Task SendTeacherSchedule(List<ScheduleView> schedules, TeacherEntity currentTeacher, ClassEntity currentClass, string currentSkill, string subjectID, DateTime startTime, DateTime endTime, CenterEntity center)
        {
            string subject =
                "Nhắc lịch dạy " +
                //"Thông báo lịch dạy lớp " + currentClass.Name + " - Môn: " + currentSkill +
                startTime.ToLocalTime().ToString("HH:mm") + "-" + endTime.ToLocalTime().ToString("HH:mm") + " ngày " + startTime.ToLocalTime().ToString("dd/MM/yyyy");
            string body = "Chào " + currentTeacher.FullName + "," +
                "<p>Lịch dạy của bạn trong lớp " + currentClass.Name + " - Môn: " + currentSkill + "</p>" +
                @"<table style='margin-top:20px; width: 100%; border: solid 1px #333; border-collapse: collapse'>
                    <thead>
                        <tr style='font-weight:bold'>
                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Tên bài</td>
                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Thời gian bắt đầu</td>
                        </tr>
                    </thead>
                    <tbody>";
            foreach (var schedule in schedules)
            {
                body += @"<tr>
                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'><a href='https://eduso.vn/" + center.Code + "/teacher/Lesson/Detail/" + schedule.LessonID + "/" + subjectID + "' target='_blank'>" + schedule.LessonName + @"</td>
                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>" + schedule.StartDate.ToLocalTime().ToString("dd/MM/yyyy HH:mm:tt") + @"</td>
                          </tr>";
            }
            body += @"</tbody>
                </table>";
            var toAddress = isTest ? new List<string> { "vietphung.it@gmail.com" } : new List<string> { currentTeacher.Email };
            _ = await _mailHelper.SendBaseEmail(toAddress, subject, body, MailPhase.WEEKLY_SCHEDULE);
        }

        private static async Task SendStudentSchedule(List<ScheduleView> schedules, TeacherEntity teacher, List<StudentEntity> studentList, ClassEntity currentClass, string currentSkill, string subjectID, DateTime startTime, DateTime endTime, CenterEntity center)
        {

            string subject =
                "Nhắc lịch học " +
            //"Thông báo lịch học lớp " + currentClass.Name + " - Môn: " + currentSkill + " - Giáo viên: " + teacher.FullName +
            startTime.ToLocalTime().ToString("HH:mm") + "-" + endTime.ToLocalTime().ToString("HH:mm") + " ngày " + startTime.ToLocalTime().ToString("dd/MM/yyyy");
            string body = "Chào bạn," +
                "<p>Lịch học của bạn trong lớp " + currentClass.Name + " - Môn: " + currentSkill + "</p>" +
                @"<table style='margin-top:20px; width: 100%; border: solid 1px #333; border-collapse: collapse'>
                    <thead>
                        <tr style='font-weight:bold'>
                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Tên bài</td>
                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Thời gian bắt đầu</td>
                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Thời gian phải hoàn thành</td>
                        </tr>
                    </thead>
                    <tbody>";
            foreach (var schedule in schedules)
            {
                body += @"<tr>
                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'><a href='https://eduso.vn/" + center.Code + "/student/Lesson/Detail/" + schedule.LessonID + "/" + subjectID + "' target='_blank'>" + schedule.LessonName + @"</a></td>
                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>" + schedule.StartDate.ToLocalTime().ToString("dd/MM/yyyy HH:mm:tt") + @"</td>
                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>" + schedule.EndDate.ToLocalTime().ToString("dd/MM/yyyy HH:mm:tt") + @"</td>
                          </tr>";
            }
            body += @"</tbody>
                </table>";
            var toAddress = isTest ? new List<string> { "vietphung.it@gmail.com" } : studentList.Select(t => t.Email).ToList();
            _ = await _mailHelper.SendBaseEmail(new List<string>(), subject, body, MailPhase.WEEKLY_SCHEDULE, null, toAddress);
        }

        #region
        //private static async Task sendmail()
        //{
        //    List<DateTime> dateTimes = new List<DateTime>
        //    {
        //        new DateTime(2020,10,1,8,0,0),
        //        new DateTime(2020,10,8,8,0,0),
        //        new DateTime(2020,10,15,8,0,0),
        //        new DateTime(2020,10,22,8,0,0),
        //        new DateTime(2020,10,29,8,0,0),
        //        new DateTime(2020,11,5,8,0,0),
        //        new DateTime(2020,11,12,8,0,0),
        //        new DateTime(2020,11,19,8,0,0),
        //    };

        //    List<Data> data = new List<Data>();

        //    foreach(DateTime currentTime in dateTimes)
        //    {
        //        var _data = GetDataForExcel(currentTime).Result;
        //        data.Add(_data);
        //    }

        //    String body = GetContent(data);
        //    String subject = "Lấy data làm excel";
        //    var toAddress = new List<String> { "nguyenvanhoa2017602593@gmail.com" };
        //    _ = await _mailHelper.SendBaseEmail(toAddress, subject, body, MailPhase.WEEKLY_SCHEDULE);
        //    Console.WriteLine($"Is Done!");
        //}
        //private static async Task<Data> GetDataForExcel(DateTime currentTime)
        //{
        //    var day = currentTime.Day;
        //    var month = currentTime.Month;
        //    var year = currentTime.Year;
        //    var startWeek = new DateTime(year, month, day, 0, 0, 0).AddDays(-3).AddMinutes(1);
        //    var endWeek = startWeek.AddDays(6).AddHours(23).AddMinutes(58).AddMilliseconds(59);

        //    var center = _centerService.CreateQuery().Find(x => x.ExpireDate >= currentTime && x.Abbr.Equals("c3vyvp") && x.Status == true).FirstOrDefault();//lay co so dang hoat dong

        //    var classesActive = _classService.GetActiveClass4Report(startWeek, endWeek, center.ID);//lay danh sach lop dang hoat dong
        //    Int32 totalStudent = 0, totalActiveStudents = 0; ;
        //    long tren8 = 0, tren5 = 0, tren2 = 0, tren0 = 0;
        //    string[] style = { "background-color: aliceblueT", "background-color: whitesmoke" };
        //    if (classesActive.Count() == 0)
        //    {
        //        //continue;
        //    }

        //    foreach (var _class in classesActive.OrderBy(x => x.Name))
        //    {
        //        //Lay danh sach ID hoc sinh trong lop
        //        var students = _studentService.GetStudentsByClassId(_class.ID).ToList();
        //        var studentIds = students.Select(t => t.ID).ToList();

        //        var classStudent = studentIds.Count();
        //        totalStudent += classStudent;

        //        //Lay danh sach ID bai hoc duoc mo trong tuan
        //        var activeLessons = _lessonScheduleService.CreateQuery().Find(o => o.ClassID == _class.ID && o.StartDate <= endWeek && o.EndDate >= startWeek).ToList();
        //        var activeLessonIds = activeLessons.Select(t => t.LessonID).ToList();

        //        //Lay danh sach hoc sinh da hoc cac bai tren trong tuan
        //        var activeProgress = _lessonProgressService.CreateQuery().Find(
        //            x => studentIds.Contains(x.StudentID) && activeLessonIds.Contains(x.LessonID)
        //            && x.LastDate <= endWeek && x.LastDate >= startWeek).ToEnumerable();


        //        //Lay danh sach hoc sinh da hoc cac bai tren trong tuan
        //        var activeStudents = _lessonProgressService.CreateQuery().Distinct(t => t.StudentID,
        //            x => studentIds.Contains(x.StudentID)).ToEnumerable();
        //        totalActiveStudents += activeStudents.Count();

        //        //var stChuaVaoLop = classStudent - activeStudents.Count();
        //        //totalstChuaVaoLop += stChuaVaoLop;

        //        // danh sach bai kiem tra
        //        var examIds = _lessonService.CreateQuery().Find(x => (x.TemplateType == 2 || x.IsPractice == true) && activeLessonIds.Contains(x.ID)).Project(x => x.ID).ToList();

        //        //ket qua lam bai cua hoc sinh trong lop
        //        var classResult = (from r in activeProgress.Where(t => examIds.Contains(t.LessonID) && t.Tried > 0)
        //                           group r by r.StudentID
        //                           into g
        //                           select new StudentResult
        //                           {
        //                               StudentID = g.Key,
        //                               ExamCount = g.Count(),
        //                               AvgPoint = g.Average(t => t.LastPoint),
        //                               StudentName = _studentService.GetItemByID(g.Key)?.FullName,
        //                           }).ToList();

        //        //render ket qua hoc tap
        //        var min8 = classResult.Count(t => t.AvgPoint >= 80);
        //        var min5 = classResult.Count(t => t.AvgPoint >= 50 && t.AvgPoint < 80);
        //        var min2 = classResult.Count(t => t.AvgPoint >= 20 && t.AvgPoint < 50);
        //        var min0 = classResult.Count(t => t.AvgPoint >= 0 && t.AvgPoint < 20);

        //        tren8 += min8;
        //        tren5 += min5;
        //        tren2 += min2;
        //        tren0 += min0;
        //    }

        //    double tiletren8 = Math.Round(((double)tren8 / totalActiveStudents) * 100,2);
        //    double tiletren5 = Math.Round(((double)tren5 / totalActiveStudents) * 100,2);
        //    double tiletren2 = Math.Round(((double)tren2 / totalActiveStudents) * 100,2);
        //    double tiletren0 = 100 - tiletren2 - tiletren5 - tiletren8;
        //    //double tiletren0 = ((double)tren0 / totalActiveStudents) * 100;
        //    var datarespone = new Data
        //    {
        //        TotalStudent = totalStudent,
        //        TotalStudentActive = totalActiveStudents,
        //        PersentPoint2 = tiletren0,
        //        PersentPoint5 = tiletren2,
        //        PersentPoint8 = tiletren5,
        //        PersentPoint10 = tiletren8,
        //        currentTime = currentTime
        //    };
        //    return datarespone;
        //}

        //private static String GetContent(List<Data> datas)
        //{
        //    String body = "";
        //    body += @"<table style='margin-top:20px; width: 100%; border: solid 1px #333; border-collapse: collapse'>
        //                    <thead>
        //                                <tr style='font-weight:bold;background-color: bisque'>
        //                                    <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:10px'>Tháng</td>
        //                                    <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:100px'>Số lượng tài khoản</td>
        //                                    <td colspan='4' style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Kết quả luyện tập & kiểm tra</td>
        //                                </tr>
        //                                <tr style='font-weight:bold;background-color: bisque'>                                        
        //                                    <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'> 0.0 -> 1.9</td>
        //                                    <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'> 2.0 -> 4.9</td>
        //                                    <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'>5.0 -> 7.9</td>
        //                                    <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'>8.0 -> 10</td>
        //                                </tr>
        //                            </thead>
        //                            <tbody>";
        //    String tbody = "";
        //    foreach(var data in datas)
        //    {
        //        tbody += $"<tr>" +
        //            $"<td>Tháng {data.currentTime.Month}</td>" +
        //            $"<td>{data.TotalStudentActive}/{data.TotalStudent}</td>" +
        //            $"<td>{data.PersentPoint2} %</td>" +
        //            $"<td>{data.PersentPoint5} %</td>" +
        //            $"<td>{data.PersentPoint8} %</td>" +
        //            $"<td>{data.PersentPoint10} %</td>" +
        //            $"</tr>";
        //    }
        //    tbody += "</tbody></table>";
        //    body += tbody;
        //    return body;
        //}

        private static async Task sendmail()
        {
            List<DateTime> dateTimes = new List<DateTime>
            {
                new DateTime(2020,10,1,8,0,0),
                new DateTime(2020,10,8,8,0,0),
                new DateTime(2020,10,15,8,0,0),
                new DateTime(2020,10,22,8,0,0),
                new DateTime(2020,10,29,8,0,0),
                new DateTime(2020,11,5,8,0,0),
                new DateTime(2020,11,12,8,0,0),
                new DateTime(2020,11,19,8,0,0),
                new DateTime(2020,11,26,8,0,0),
                new DateTime(2020,12,3,8,0,0),
                new DateTime(2020,12,10,8,0,0),
            };

            List<Data> data = new List<Data>();

            List<CenterEntity> listCenterActive = new List<CenterEntity>();
            List<TeacherEntity> listTeacher = new List<TeacherEntity>();
            //foreach (DateTime currentTime in dateTimes)
            //{
            //    var centersActive = _centerService.GetActiveCenter(currentTime) as CenterEntity;//lay co so dang hoat dong
            //    listCenterActive.Add(centersActive);
            //}

            var a = dateTimes.LastOrDefault();
            var centersActive = _centerService.GetActiveCenter(dateTimes.LastOrDefault()).ToList();//lay co so dang hoat dong
            var b = _centerService.GetActiveCenter(DateTime.Now).ToList();

            foreach (var center in centersActive)
            {
                if (center.Abbr == "c3vyvp")
                {
                    var indexWeek = 0;
                    var index = 1;
                    data = new List<Data>();
                    foreach (DateTime currentTime in dateTimes)
                    {
                        var _data = await GetDataForExcel(currentTime, center);
                        if (currentTime.Month == dateTimes.ElementAtOrDefault(index).Month)
                        {
                            indexWeek++;
                            _data.Week = $"Tuần {indexWeek} tháng {currentTime.Month}";
                        }
                        else
                        {
                            indexWeek++;
                            _data.Week = $"Tuần {indexWeek} tháng {currentTime.Month}";
                            indexWeek = 0;
                        }
                        data.Add(_data);
                        index++;
                    }

                    var listTeacherHeader = _teacherService.CreateQuery().Find(x => x.IsActive == true && x.Centers.Any(y => y.CenterID == center.ID)).ToList().FindAll(y => HasRole(y.ID, center.ID, "head-teacher")).ToList();
                    if (listTeacherHeader.Any(x => x.Email == "huonghl@utc.edu.vn"))
                    {
                        listTeacherHeader.RemoveAt(listTeacherHeader.FindIndex(x => x.Email == "huonghl@utc.edu.vn"));
                    }

                    String body = GetContent(data);
                    String subject = $"B/c tháng {center.Name}";
                    var toAddress = isTest == true ? new List<String> { "nguyenvanhoa2017602593@gmail.com", "k.chee.dinh@gmail.com" } : listTeacherHeader.Select(x => x.Email).ToList();
                    var toBcc = isTest == true ? null : new List<String> { "nguyenhoa.dev@gmail.com", "vietphung.it@gmail.com", "huonghl@utc.edu.vn", "kchidinh@gmail.com" };
                    _ = await _mailHelper.SendBaseEmail(toAddress, subject, body, MailPhase.WEEKLY_SCHEDULE, null, toBcc);
                    Console.WriteLine($"{center.Name} Is Done!");
                }
            }
        }
        private static async Task<Data> GetDataForExcel(DateTime currentTime, CenterEntity center)
        {
            var day = currentTime.Day;
            var month = currentTime.Month;
            var year = currentTime.Year;
            var startWeek = new DateTime(year, month, day, 0, 0, 0).AddDays(-3).AddMinutes(1);
            var endWeek = startWeek.AddDays(6).AddHours(23).AddMinutes(58).AddMilliseconds(59);

            //var center = _centerService.CreateQuery().Find(x => x.ExpireDate >= currentTime && x.Abbr.Equals("c3vyvp") && x.Status == true).FirstOrDefault();//lay co so dang hoat dong

            var classesActive = _classService.GetActiveClass4Report(startWeek, endWeek, center.ID);//lay danh sach lop dang hoat dong
            Int32 totalStudent = 0, totalActiveStudents = 0; ;
            long tren8 = 0, tren5 = 0, tren0 = 0;
            string[] style = { "background-color: aliceblueT", "background-color: whitesmoke" };
            if (classesActive.Count() == 0)
            {
                //continue;
            }

            foreach (var _class in classesActive.OrderBy(x => x.Name))
            {
                //Lay danh sach ID hoc sinh trong lop
                var students = _studentService.GetStudentsByClassId(_class.ID).ToList();
                var studentIds = students.Select(t => t.ID).ToList();

                var classStudent = studentIds.Count();
                totalStudent += classStudent;

                //Lay danh sach ID bai hoc duoc mo trong tuan
                var activeLessons = _lessonScheduleService.CreateQuery().Find(o => o.ClassID == _class.ID && o.StartDate <= endWeek && o.EndDate >= startWeek).ToList();
                var activeLessonIds = activeLessons.Select(t => t.LessonID).ToList();

                //Lay danh sach hoc sinh da hoc cac bai tren trong tuan
                var activeProgress = _lessonProgressService.CreateQuery().Find(
                    x => studentIds.Contains(x.StudentID) && activeLessonIds.Contains(x.LessonID)
                    && x.LastDate <= endWeek && x.LastDate >= startWeek).ToEnumerable();


                //Lay danh sach hoc sinh da hoc cac bai tren trong tuan
                var activeStudents = _lessonProgressService.CreateQuery().Distinct(t => t.StudentID,
                    x => studentIds.Contains(x.StudentID)).ToEnumerable();
                totalActiveStudents += activeStudents.Count();

                //var stChuaVaoLop = classStudent - activeStudents.Count();
                //totalstChuaVaoLop += stChuaVaoLop;

                // danh sach bai kiem tra
                var examIds = _lessonService.CreateQuery().Find(x => (x.TemplateType == 2 || x.IsPractice == true) && activeLessonIds.Contains(x.ID)).Project(x => x.ID).ToList();
                //var examIds = _lessonService.CreateQuery().Find(x => (x.TemplateType == 1 || x.IsPractice == true) && activeLessonIds.Contains(x.ID)).Project(x => x.ID).ToList();

                //ket qua lam bai cua hoc sinh trong lop
                var classResult = (from r in activeProgress.Where(t => examIds.Contains(t.LessonID) && t.Tried > 0)
                                   group r by r.StudentID
                                   into g
                                   select new StudentResult
                                   {
                                       StudentID = g.Key,
                                       ExamCount = g.Count(),
                                       AvgPoint = g.Average(t => t.LastPoint),
                                       StudentName = _studentService.GetItemByID(g.Key)?.FullName,
                                   }).ToList();

                //render ket qua hoc tap
                var min8 = classResult.Count(t => t.AvgPoint >= 80);
                var min5 = classResult.Count(t => t.AvgPoint >= 50 && t.AvgPoint < 80);
                var min0 = classResult.Count(t => t.AvgPoint >= 0 && t.AvgPoint < 50);

                tren8 += min8;
                tren5 += min5;
                tren0 += min0;
            }

            double tiletren8 = Math.Round(((double)tren8 / totalActiveStudents) * 100, 2);
            double tiletren5 = Math.Round(((double)tren5 / totalActiveStudents) * 100, 2);
            //double tiletren2 = Math.Round(((double)tren2 / totalActiveStudents) * 100,2);
            //double tiletren0 = 100 - tiletren2 - tiletren5 - tiletren8;
            double tiletren0 = Math.Round(((double)tren0 / totalActiveStudents) * 100, 2);
            double tilechualam = 100 - tiletren0 - tiletren5 - tiletren8;
            var datarespone = new Data
            {
                TotalStudent = totalStudent,
                TotalStudentActive = totalActiveStudents,
                PersentMinPoint0 = tiletren0,
                PersentMinPoint5 = tiletren5,
                PersentMinPoint8 = tiletren8,
                currentTime = currentTime,
                DontWork = tilechualam
            };
            return datarespone;
        }

        private static String GetContent(List<Data> datas)
        {
            String body = "";
            body += @"<table style='margin-top:20px; width: 100%; border: solid 1px #333; border-collapse: collapse'>
                            <thead>
                                        <tr style='font-weight:bold;background-color: bisque'>
                                            <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:10px'>STT</td>
                                            <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:100px'>Sĩ số</td>
                                            <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:100px'>Chưa vào hệ thống</td>
                                            <td colspan='4' style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Kết quả luyện tập & kiểm tra</td>
                                        </tr>
                                        <tr style='font-weight:bold;background-color: bisque'>                                        
                                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'> 0.0 -> 4.9</td>
                                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'>5.0 -> 7.9</td>
                                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'>8.0 -> 10</td>
                                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:50px'>Chưa làm</td>
                                        </tr>
                                    </thead>
                                    <tbody>";
            String tbody = "";
            var listData = datas.GroupBy(x => x.Week.Substring(x.Week.Length - 2)).OrderByDescending(x => x.Key);
            foreach (var _data in listData)
            {
                var _datas = _data.ToList().OrderByDescending(x => x.Week);
                foreach (var data in _datas)
                {
                    tbody += $"<tr>" +
                        $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{data.Week}</td>" +
                        $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{data.TotalStudent}</td>" +
                        $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{data.TotalStudent - data.TotalStudentActive}</td>" +
                        $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;background-color:#ffff33'>{data.PersentMinPoint0} %</td>" +
                        $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;background-color: lightblue'>{data.PersentMinPoint5} %</td>" +
                        $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;background-color: lightgreen'>{data.PersentMinPoint8} %</td>" +
                        $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;background-color: rgb(194,194,216)'>{data.DontWork} %</td>" +
                        $"</tr>";
                }
            }
            tbody += "</tbody></table>";
            body += tbody;
            return body;
        }
        #endregion

        //function
        private static bool HasRole(string userid, string center, string role)
        {
            var teacher = _teacherService.GetItemByID(userid);
            if (teacher == null) return false;
            var centerMember = teacher.Centers.Find(t => t.CenterID == center);
            if (centerMember == null) return false;
            if (_roleService.GetItemByID(centerMember.RoleID).Code != role) return false;
            return true;
        }

        private class ClassSubjectInfo
        {
            public string Name { get; set; }
            public string LessonName { get; set; }
            public DateTime Start { get; set; }
            public int Type { get; set; }
        }

        private class StudentResult
        {
            public string StudentID { get; set; }
            public int ExamCount { get; set; }
            public double AvgPoint { get; set; }
            public string StudentName { get; set; }
            public string AvgTimeDoExam { get; set; }
            public int CompletedLesson { get; set; }
            public int TotalLesson { get; set; }
        }

        private static void WriteText()
        {
            string fileLPath = @"G:\New folder\listStudenID.txt";

            List<string> lines = new List<string>();
            var StudentsID = _studentService.GetAll().Project(x => x.ID).ToList();
            lines.AddRange(StudentsID);

            System.IO.File.WriteAllLines(fileLPath, lines);
        }

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

        private static String Subject(String TeacherName, String ClassName, String CenterName, DateTime startWeek, DateTime endWeek)
        {
            if (ClassName.ToLower().Contains("lớp"))
            {
                String subject = $"B/c tuần {ClassName}, thầy/cô {TeacherName}, {CenterName} ({startWeek.ToString("dd/MM/yyyy")} - {endWeek.ToString("dd/MM/yyyy")})";
                return subject;
            }
            else
            {
                String subject = $"B/c tuần lớp {ClassName}, thầy/cô {TeacherName}, {CenterName} ({startWeek.ToString("dd/MM/yyyy")} - {endWeek.ToString("dd/MM/yyyy")})";
                return subject;
            }
        }
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

    public class ReportViewModal : ReportEntity
    {
        public String CenterName { get; set; } //ten co so
        public int TotalStudents { get; set; } //tong so hoc sinh
        public String ClassName4ReportPoint { get; set; } //Ten lop co ti le hoc sinh dat diem 0 -> 4.9 > 30%
        public String ClassName4ReportInactive { get; set; }//ten lop co so hoc sinh chua lam > 10
        public Int32 TotalDontWork { get; set; } //tong so hoc sinh chua lam bai
        public Int32 TotalInactiveStudents { get; set; }//tong so hoc sinh chua hoc
    }

    public class Data
    {
        //public Int32 TotalStudent { get; set; }
        //public Int32 TotalStudentActive { get; set; }
        //public Double PersentPoint2 { get; set; }
        //public Double PersentPoint5 { get; set; }
        //public Double PersentPoint8 { get; set; }
        //public Double PersentPoint10 { get; set; }
        //public DateTime currentTime { get; set; }
        public Int32 TotalStudent { get; set; }
        public Int32 TotalStudentActive { get; set; }
        public Double PersentMinPoint0 { get; set; }
        public Double PersentMinPoint5 { get; set; }
        public Double PersentMinPoint8 { get; set; }
        public DateTime currentTime { get; set; }
        public Double DontWork { get; set; }
        public String Week { get; set; }
    }

}
