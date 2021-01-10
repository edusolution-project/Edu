using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BaseCoreEmail;
using BaseCustomerEntity.Database;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using OfficeOpenXml;
using OfficeOpenXml.Style;

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
                    case "ReportToExcel":
                        Console.WriteLine("Dang xuat bao cao");
                        await ReportToExcel();
                        break;
                    case "ServiceRenewalNotice":
                        Console.WriteLine(ServiceRenewalNotice().Result);
                        break;
                    case "BenTre":
                        await BenTre();
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
                                //var toAddress = isTest == true ? new List<string> { "shin.l0v3.ly@gmail.com" } : new List<string> { "shin.l0v3.ly@gmail.com" };
                                var bccAddress = isTest == true ? null : new List<string> { "nguyenhoa.dev@gmail.com", "vietphung.it@gmail.com", "huonghl@utc.edu.vn" };
                                _ = await _mailHelper.SendBaseEmail(toAddress, subject, $"Kính gửi Thầy/Cô: <span style='font-weight:600'>{item.FullName}</span>," + content, MailPhase.WEEKLY_SCHEDULE, null, bccAddress);
                            }
                        }
                        else if (listTeacherHeader.Count() == 1)
                        {
                            var toAddress = isTest == true ? new List<string> { "nguyenvanhoa2017602593@gmail.com", "vietphung.it@gmail.com", "k.chee.dinh@gmail.com" } : new List<string> { listTeacherHeader[0].Email };
                            //var toAddress = isTest == true ? new List<string> { "shin.l0v3.ly@gmail.com" } : new List<string> { "shin.l0v3.ly@gmail.com" };
                            var bccAddress = isTest == true ? null : new List<string> { "nguyenhoa.dev@gmail.com", "vietphung.it@gmail.com", "huonghl@utc.edu.vn" };
                            _ = await _mailHelper.SendBaseEmail(toAddress, subject, $"Kính gửi Thầy/Cô: <span style='font-weight:600'>{listTeacherHeader[0].FullName}</span>," + content, MailPhase.WEEKLY_SCHEDULE, null, bccAddress);
                        }
                        else
                        {
                            Console.WriteLine($"{listTeacherHeader.Count()}");
                        }
                    }
                    Console.WriteLine($"Send Weekly Report To {center.Name} Is Done!");
                }
            }
            catch (Exception ex)
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
                        //var toAddress = isTest == true ? new List<string> { "shin.l0v3.ly@gmail.com" } : new List<string> { "shin.l0v3.ly@gmail.com" };
                        var bccAddress = isTest == true ? null : new List<string> { "nguyenhoa.dev@gmail.com", "vietphung.it@gmail.com" };
                        _ = await _mailHelper.SendBaseEmail(toAddress, Subject(item.FullName, @class.Name, centerName, startweek, endWeek), $"<div>Kính gửi Thầy/Cô: <span style='font-weight:600'>{item.FullName}</span>,</div>" + content, MailPhase.WEEKLY_SCHEDULE, null, bccAddress);
                    }
                }
                else if (listTeacher.Count == 1)
                {
                    var toAddress = isTest == true ? new List<string> { "nguyenvanhoa2017602593@gmail.com", "vietphung.it@gmail.com", "k.chee.dinh@gmail.com" } : new List<string> { listTeacher[0].Email };
                    //var toAddress = isTest == true ? new List<string> { "shin.l0v3.ly@gmail.com" } : new List<string> { "shin.l0v3.ly@gmail.com" };
                    var bccAddress = isTest == true ? null : new List<string> { "nguyenhoa.dev@gmail.com", "vietphung.it@gmail.com" };
                    _ = await _mailHelper.SendBaseEmail(toAddress, Subject(listTeacher[0].FullName, @class.Name, centerName, startweek, endWeek), $"<div>Kính gửi Thầy/Cô: <span style='font-weight:600'>{listTeacher[0].FullName}</span>,</div>" + content, MailPhase.WEEKLY_SCHEDULE, null, bccAddress);
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
                DateTime currentTime = new DateTime(2020, 12, 13, 23, 59, 00);
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

                        var test = new Dictionary<String, String>();

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

        #region ReportToExcel
        public static async Task ReportToExcel()
        {
            try
            {
                //var activeCenters = _centerService.GetActiveCenter(DateTime.Now);
                var activeCenters = _centerService.GetActiveCenter(new DateTime(2020,12,31));
                if (activeCenters.Count() == 0)
                {
                    Console.WriteLine($"active Centers = 0");
                }

                foreach (var center in activeCenters)
                {
                    //if (center.Abbr.Contains("utc"))
                    //if (center.Abbr.Contains("c3vyvp"))
                    if (center.Abbr.Contains("c3cvpvp"))
                    {
                        var Students = _studentService.CreateQuery().Find(x => x.Centers.Contains(center.ID)).ToList();
                        var TotalStudentsinCenter = Students.Count(); //Tong so hoc sinh
                        var Teachers = _teacherService.CreateQuery().Find(x => x.Centers.Any(y => y.CenterID == center.ID) && x.Email != "huonghl@utc.edu.vn").ToList();
                        var TotalTeachersinCenter = Teachers.Count(); //Tong so giao vien
                        var ExpireDate = center.ExpireDate; //Han muc
                        var ClassesinCenter = _classService.CreateQuery().Find(x => x.Center == center.ID).ToList();
                        var TotalClassesinCenter = ClassesinCenter.Count(); //Tong so lop hoc co trong co so
                        var ListStudentIDs = Students.Select(x => x.ID).ToList();
                        var ActiveStudents = _lessonProgressService.CreateQuery().Find(x => x.TotalLearnt > 0 && ListStudentIDs.Contains(x.StudentID)).ToList().GroupBy(x => x.StudentID);
                        var InactiveStudentsnoGroup = _lessonProgressService.CreateQuery().Find(x => x.TotalLearnt == 0 && ListStudentIDs.Contains(x.StudentID)).ToList();
                        var TotalActiveStudents = ActiveStudents.Count();

                        Center4Report2Excel dataCenter = new Center4Report2Excel()
                        {
                            CenterID = center.ID,
                            CenterName = center.Name,
                            StartDate = center.StartDate,
                            //EndDate = center.ExpireDate,
                            EndDate = new DateTime(2020, 12, 31),
                            Limit = (Int32)center.Limit,
                            TotalTeachersinCenter = TotalTeachersinCenter,
                            TotalStudentsinCenter = TotalStudentsinCenter,
                            TotalClass = TotalClassesinCenter,
                            TotalClassActive = ClassesinCenter.Where(x => x.IsActive == true).Count(),
                            DaHoc = TotalActiveStudents
                        };

                        List<Class4Report2Excel> dataResponse = new List<Class4Report2Excel>();

                        foreach (var @class in ClassesinCenter.OrderBy(x => x.Name))
                        {
                            //lay danh sach giao vien trong lop
                            var listNameTeachers = "";
                            if (@class.Members != null)
                            {
                                var members = @class.Members.Where(x => x.Type == ClassMemberType.TEACHER);
                                if (members.Count() > 0)
                                {
                                    foreach (var mem in members)
                                    {
                                        var teacherFName = _teacherService.GetItemByID(mem.TeacherID).FullName.Trim();
                                        //var teacherName = teacherFName.Substring(teacherFName.LastIndexOf(" "));
                                        var str = teacherFName.Split(" ");
                                        //var teacherName = str[str.Length - 1];
                                        var teacherName = teacherFName;
                                        listNameTeachers += $"{teacherName}, ";
                                    }
                                    listNameTeachers = listNameTeachers.Remove(listNameTeachers.LastIndexOf(",")).Trim();
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

                            var studentsinClass = Students.Where(x => x.JoinedClasses.Contains(@class.ID));
                            var studentIDsinClass = studentsinClass.Select(x => x.ID).ToList();

                            //var listTime = GetListMonth(center.StartDate,center.ExpireDate);
                            var listTime = new Dictionary<Int32, DataTime>(){
                                //{8,new DataTime{ StartTime = new DateTime(2020,8,1,0,0,0),EndTime = new DateTime(2020,8,31,23,59,0)} },
                                //{9,new DataTime{ StartTime = new DateTime(2020,9,1,0,0,0),EndTime = new DateTime(2020,9,30,23,59,0)} },
                                //{10,new DataTime{ StartTime = new DateTime(2020,10,1,0,0,0),EndTime = new DateTime(2020,10,31,23,59,0)} },
                                //{11,new DataTime{ StartTime = new DateTime(2020,11,1,0,0,0),EndTime = new DateTime(2020,11,30,23,59,0)} },
                                //{12,new DataTime{ StartTime = new DateTime(2020,12,1,0,0,0),EndTime = new DateTime(2020,12,31,23,59,0)} },
                            };
                            foreach (var time in listTime)
                            {
                                var dataClass = new Class4Report2Excel();
                                dataClass = NewMethod(center, @class, studentIDsinClass, time.Value.StartTime, time.Value.EndTime, listNameTeachers, studentsinClass.ToList(), InactiveStudentsnoGroup.ToList());
                                dataResponse.Add(dataClass);
                            }
                        }
                        //var error = Export2Excelv2(dataResponse, dataCenter,"3thang").Result;
                        var test1 = Export2Excel(dataResponse, dataCenter,"8").Result;
                        //Console.WriteLine(error);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static Class4Report2Excel NewMethod(CenterEntity center, ClassEntity @class, List<string> studentIDsinClass, DateTime StartTime, DateTime EndTime, String listNameTeachers, List<StudentEntity> studentsinClass, List<LessonProgressEntity> InactiveStudentsnoGroup)
        {
            Class4Report2Excel dataClass = new Class4Report2Excel();

            dataClass.CenterID = center.ID;
            dataClass.ClassID = @class.ID;
            dataClass.ClassName = @class.Name;
            dataClass.TeacherName = listNameTeachers;
            dataClass.StartDate = @class.StartDate;
            dataClass.EndDate = @class.EndDate;
            dataClass.StudentinClass = studentsinClass.Count();
            dataClass.DontActiveStudent = InactiveStudentsnoGroup.Where(x => x.ClassID == @class.ID).Count();
            dataClass.Status = @class.IsActive ? "Hoạt động" : "Không hoạt động";

            //danh sach mon hoc trong lop
            var classSbjs = _classSubjectService.CreateQuery().Find(x => x.ClassID == @class.ID).ToList();
            //danh sach bai hoc trong lop
            var activeLessons = _lessonScheduleService.CreateQuery().Find(o => o.ClassID == @class.ID && o.StartDate <= EndTime && o.EndDate >= StartTime).ToList();
            var activeLessonIds = activeLessons.Select(x => x.LessonID).ToList();
            //danh sach bai luyen tap + kiem tra
            var examIds = _lessonService.CreateQuery().Find(x => (x.TemplateType == 2 || x.IsPractice == true) && activeLessonIds.Contains(x.ID)).Project(x => x.ID).ToList();
            //Lay danh sach hoc sinh da hoc cac bai tren trong tuan
            var activeProgress = _lessonProgressService.CreateQuery().Find(x => studentIDsinClass.Contains(x.StudentID) && activeLessonIds.Contains(x.LessonID)
                && x.LastDate <= new DateTime(2020, 12, 31, 23, 59, 00) && x.LastDate >= center.StartDate).ToList();
            //ket qua lam bai cua hoc sinh trong lop
            var classResult = (from r in activeProgress.Where(t => examIds.Contains(t.LessonID) && t.Tried > 0)
                               group r by r.StudentID
                               into g
                               select new StudentResult
                               {
                                   StudentID = g.Key,
                                   AvgPoint = g.Average(t => t.LastPoint),
                               }).ToList();

            //render ket qua hoc tap
            var minPoint8 = classResult.Count(t => t.AvgPoint >= 80);
            var minPoint5 = classResult.Count(t => t.AvgPoint >= 50 && t.AvgPoint < 80);
            var minPoint2 = classResult.Count(t => t.AvgPoint >= 20 && t.AvgPoint < 50);
            var minPoint0 = classResult.Count(t => t.AvgPoint >= 0 && t.AvgPoint < 20);

            dataClass.MinPoint0 = minPoint0;
            dataClass.MinPoint2 = minPoint2;
            dataClass.MinPoint5 = minPoint5;
            dataClass.MinPoint8 = minPoint8;
            return dataClass;
        }

        private static async Task<String> Export2Excel(List<Class4Report2Excel> data, Center4Report2Excel dataCenter, String month = "")
        {
            try
            {
                using (ExcelPackage p = new ExcelPackage())
                {
                    // đặt tên người tạo file
                    p.Workbook.Properties.Author = "Admin";

                    // đặt tiêu đề cho file
                    p.Workbook.Properties.Title = $"Báo cáo thống kê {dataCenter.CenterName} ({dataCenter.StartDate.ToString("dd/MM/yyyy")} - {dataCenter.EndDate.ToString("dd/MM/yyyy")})";

                    //Tạo một sheet để làm việc trên đó
                    p.Workbook.Worksheets.Add($"{dataCenter.CenterName}");

                    // lấy sheet vừa add ra để thao tác
                    ExcelWorksheet ws = p.Workbook.Worksheets[1];

                    // đặt tên cho sheet
                    ws.Name = $"{dataCenter.CenterName}";
                    // fontsize mặc định cho cả sheet
                    ws.Cells.Style.Font.Size = 11;
                    // font family mặc định cho cả sheet
                    ws.Cells.Style.Font.Name = "Calibri";

                    // Tạo danh sách các column header
                    //string[] arrColumnHeader = {"#","Lớp","Ngày bắt đầu","Ngày kết thúc","Sĩ số","Chưa sử dụng hệ thống","10.0 - 8.0","7.9 - 5.0","4.9 - 2.0","1.9 - 0.0"};
                    string[] arrColumnHeader = { "#", "Lớp", "Ngày bắt đầu", "Ngày kết thúc", "Sĩ số", "", "10.0 - 8.0", "7.9 - 5.0", "4.9 - 2.0", "1.9 - 0.0", "Chưa làm" };

                    // lấy ra số lượng cột cần dùng dựa vào số lượng header
                    var countColHeader = arrColumnHeader.Count();

                    // merge các column lại từ column 1 đến số column header
                    // gán giá trị cho cell vừa merge là Thống kê thông tni User Kteam
                    #region row 1
                    ws.Cells[1, 1].Value = $"Báo cáo thống kê {dataCenter.CenterName} ({dataCenter.StartDate.ToString("dd/MM/yyyy")} - {dataCenter.EndDate.ToString("dd/MM/yyyy")})";
                    ws.Cells[1, 1, 1, countColHeader].Merge = true;
                    // in đậm
                    ws.Cells[1, 1, 1, countColHeader].Style.Font.Bold = true;
                    // căn giữa
                    ws.Cells[1, 1, 1, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    #endregion

                    #region row 2
                    ws.Cells[2, countColHeader - 1].Value = "Hạn mức";
                    // in đậm
                    ws.Cells[2, countColHeader - 1].Style.Font.Bold = true;
                    // căn giữa
                    ws.Cells[2, countColHeader - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    ws.Cells[2, countColHeader].Value = dataCenter.Limit;
                    // in đậm
                    ws.Cells[2, countColHeader].Style.Font.Bold = true;
                    // căn giữa
                    ws.Cells[2, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    #endregion

                    #region row 3
                    ws.Cells[3, countColHeader - 1].Value = "Tổng số giáo viên";
                    // in đậm
                    ws.Cells[3, countColHeader - 1].Style.Font.Bold = true;
                    // căn giữa
                    ws.Cells[3, countColHeader - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    ws.Cells[3, countColHeader].Value = dataCenter.TotalTeachersinCenter;
                    // in đậm
                    ws.Cells[3, countColHeader].Style.Font.Bold = true;
                    // căn giữa
                    ws.Cells[3, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    #endregion

                    #region row 4
                    ws.Cells[4, countColHeader - 1].Value = "Tổng số học viên";
                    // in đậm
                    ws.Cells[4, countColHeader - 1].Style.Font.Bold = true;
                    // căn giữa
                    ws.Cells[4, countColHeader - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    ws.Cells[4, countColHeader].Value = dataCenter.TotalStudentsinCenter;
                    // in đậm
                    ws.Cells[4, countColHeader].Style.Font.Bold = true;
                    // căn giữa
                    ws.Cells[4, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    #endregion

                    #region row 5
                    ws.Cells[5, countColHeader - 1].Value = "Lớp đang hoạt động";
                    // in đậm
                    ws.Cells[5, countColHeader - 1].Style.Font.Bold = true;
                    // căn giữa
                    ws.Cells[5, countColHeader - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    ws.Cells[5, countColHeader].Value = $"{dataCenter.TotalClassActive}/{dataCenter.TotalClass}";
                    // in đậm
                    ws.Cells[5, countColHeader].Style.Font.Bold = true;
                    // căn giữa
                    ws.Cells[5, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    #endregion

                    #region row 6
                    ws.Cells[6, countColHeader - 1].Value = "Đã học";
                    // in đậm
                    ws.Cells[6, countColHeader - 1].Style.Font.Bold = true;
                    // căn giữa
                    ws.Cells[6, countColHeader - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    ws.Cells[6, countColHeader].Value = $"{dataCenter.DaHoc}/{dataCenter.TotalStudentsinCenter}";
                    // in đậm
                    ws.Cells[6, countColHeader].Style.Font.Bold = true;
                    // căn giữa
                    ws.Cells[6, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    #endregion

                    int colIndex = 1;
                    int rowIndex = 7;

                    //tạo các header từ column header đã tạo từ bên trên
                    foreach (var item in arrColumnHeader)
                    {
                        var cell = ws.Cells[rowIndex, colIndex];

                        //set màu thành gray
                        var fill = cell.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(157, 195, 230));

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
                    for (Int32 index = 0; index < data.Count(); index++)
                    {
                        var item = data.ElementAtOrDefault(index);
                        // bắt đầu ghi từ cột 1. Excel bắt đầu từ 1 không phải từ 0
                        colIndex = 1;

                        // rowIndex tương ứng từng dòng dữ liệu
                        rowIndex++;

                        //gán giá trị cho từng cell                      
                        ws.Cells[rowIndex, colIndex++].Value = index + 1;
                        ws.Cells[rowIndex, colIndex++].Value = item.ClassName;
                        ws.Cells[rowIndex, colIndex++].Value = item.StartDate.ToShortDateString();
                        ws.Cells[rowIndex, colIndex++].Value = item.EndDate.ToShortDateString();
                        ws.Cells[rowIndex, colIndex++].Value = item.StudentinClass;
                        ws.Cells[rowIndex, colIndex++].Value = item.DontActiveStudent;
                        ws.Cells[rowIndex, colIndex++].Value = item.MinPoint8;
                        ws.Cells[rowIndex, colIndex++].Value = item.MinPoint5;
                        ws.Cells[rowIndex, colIndex++].Value = item.MinPoint2;
                        ws.Cells[rowIndex, colIndex++].Value = item.MinPoint0;
                        ws.Cells[rowIndex, colIndex++].Value = item.StudentinClass - item.MinPoint0 - item.MinPoint2 - item.MinPoint5 - item.MinPoint8;

                        // lưu ý phải .ToShortDateString để dữ liệu khi in ra Excel là ngày như ta vẫn thấy.Nếu không sẽ ra tổng số :v
                        //ws.Cells[rowIndex, colIndex++].Value = item.Birthday.ToShortDateString();

                    }

                    //Lưu file lại
                    Byte[] bin = p.GetAsByteArray();
                    File.WriteAllBytes($"H:\\Hoa\\ChuyenVP\\Month{month}{dataCenter.CenterName}{DateTime.Now.ToString("HHmmssddMMyyyy")}.xlsx", bin);
                }
                return "";
            }
            catch (Exception EE)
            {
                return EE.Message;
            }
        }

        private static async Task<String> Export2Excelv2(List<Class4Report2Excel> data, Center4Report2Excel dataCenter, String Month)
        {
            try
            {
                using (ExcelPackage p = new ExcelPackage())
                {
                    // đặt tên người tạo file
                    p.Workbook.Properties.Author = "Admin";

                    // đặt tiêu đề cho file
                    p.Workbook.Properties.Title = $"Báo cáo kết quả {dataCenter.CenterName} ({dataCenter.StartDate.ToString("dd/MM/yyyy")} - {dataCenter.EndDate.ToString("dd/MM/yyyy")})";

                    //Tạo một sheet để làm việc trên đó
                    p.Workbook.Worksheets.Add($"{dataCenter.CenterName}");

                    // lấy sheet vừa add ra để thao tác
                    ExcelWorksheet ws = p.Workbook.Worksheets[1];

                    // đặt tên cho sheet
                    ws.Name = $"{dataCenter.CenterName}";
                    // fontsize mặc định cho cả sheet
                    ws.Cells.Style.Font.Size = 11;
                    // font family mặc định cho cả sheet
                    ws.Cells.Style.Font.Name = "Calibri";

                    // Tạo danh sách các column header
                    //string[] arrColumnHeader = { "#", "Lớp", "Ngày bắt đầu", "Ngày kết thúc", "Sĩ số", "Chưa sử dụng hệ thống", "10.0 - 8.0", "7.9 - 5.0", "4.9 - 2.0", "1.9 - 0.0","Chưa làm" };
                    string[] arrColumnHeader = { "#", "Lớp", "Giáo viên", "Ngày bắt đầu", "Ngày kết thúc", "Sĩ số", "Tháng", "Kết quả", "", "", "", "" };

                    // lấy ra số lượng cột cần dùng dựa vào số lượng header
                    var countColHeader = arrColumnHeader.Count();

                    // merge các column lại từ column 1 đến số column header
                    // gán giá trị cho cell vừa merge là Thống kê thông tni User Kteam
                    #region row 1
                    {
                        ws.Cells[1, 1].Value = $"Báo cáo kết quả {dataCenter.CenterName} ({dataCenter.StartDate.ToString("dd/MM/yyyy")} - {dataCenter.EndDate.ToString("dd/MM/yyyy")})";
                        ws.Cells[1, 1, 1, 108].Merge = true;
                        // in đậm
                        ws.Cells[1, 1, 1, 108].Style.Font.Bold = true;
                        ws.Cells[1, 1, 1, 108].Style.Font.Size = 18;
                        ws.Cells[1, 1, 1, 108].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[1, 1, 1, 108].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        // căn giữa
                        ws.Row(1).Height = 50;
                        var border = ws.Cells[1, 1, 1, 108].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[1, 1, 1, 108].Style.WrapText = true;
                    }
                    #endregion

                    #region row 2
                    ws.Cells[2, 1, 2, 2].Value = $"Hạn mức: {dataCenter.Limit}";
                    ws.Cells[2, 1, 2, 2].Merge = true;
                    // in đậm
                    ws.Cells[2, 1, 2, 2].Style.Font.Bold = true;
                    // căn giữa
                    ws.Cells[2, 1, 2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    ws.Cells[2, 80, 2, 97].Value = "Điểm 8.0 - 10";
                    ws.Cells[2, 80, 2, 97].Merge = true;
                    ws.Cells[2, 98, 2, 108].Style.Fill.PatternType = ExcelFillStyle.DarkDown; // cell co \\
                    ws.Cells[2, 98, 2, 108].Merge = true;
                    ws.Cells[2, 98, 2, 108].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(134, 208, 141));
                    //ws.Cells[2, 98, 2, 108].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Blue);
                    #endregion

                    #region row 3
                    ws.Cells[3, 1, 3, 2].Value = $"Tổng số giáo viên: {dataCenter.TotalTeachersinCenter}";
                    ws.Cells[3, 1, 3, 2].Merge = true;
                    // in đậm
                    ws.Cells[3, 1, 3, 2].Style.Font.Bold = true;
                    // căn giữa
                    ws.Cells[3, 1, 3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    ws.Cells[3, 80, 3, 97].Value = "Điểm 5.0 - 7.9";
                    ws.Cells[3, 80, 3, 97].Merge = true;
                    ws.Cells[3, 98, 3, 108].Style.Fill.PatternType = ExcelFillStyle.Gray0625;
                    ws.Cells[3, 98, 3, 108].Merge = true;
                    ws.Cells[3, 98, 3, 108].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(157, 195, 230));
                    //ws.Cells[3, 98, 3, 108].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Blue);
                    #endregion

                    #region row 4
                    ws.Cells[4, 1, 4, 2].Value = $"Tổng số học viên: {dataCenter.TotalStudentsinCenter}";
                    ws.Cells[4, 1, 4, 2].Merge = true;
                    // in đậm
                    ws.Cells[4, 1, 4, 2].Style.Font.Bold = true;
                    // căn giữa
                    ws.Cells[4, 1, 4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    ws.Cells[4, 4].Value = $"Hoạt động: {dataCenter.DaHoc}";
                    // in đậm
                    ws.Cells[4, 4].Style.Font.Bold = true;
                    // căn giữa
                    ws.Cells[4, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    ws.Cells[4, 80, 4, 97].Value = "Điểm 2.0 - 4.9";
                    ws.Cells[4, 80, 4, 97].Merge = true;
                    ws.Cells[4, 98, 4, 108].Style.Fill.PatternType = ExcelFillStyle.DarkUp;
                    ws.Cells[4, 98, 4, 108].Merge = true;
                    ws.Cells[4, 98, 4, 108].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 255, 102));
                    //ws.Cells[4, 98, 4, 108].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Blue);
                    #endregion

                    #region row 5
                    ws.Cells[5, 1, 5, 2].Value = $"Tổng số lớp: {dataCenter.TotalClass}";
                    ws.Cells[5, 1, 5, 2].Merge = true;
                    // in đậm
                    ws.Cells[5, 1, 5, 2].Style.Font.Bold = true;
                    // căn giữa
                    ws.Cells[5, 1, 5, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    ws.Cells[5, 4, 5, 4].Value = $"Lớp đang hoạt động: {dataCenter.TotalClassActive}";
                    // in đậm
                    ws.Cells[5, 4, 5, 4].Style.Font.Bold = true;
                    // căn giữa
                    ws.Cells[5, 4, 5, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    ws.Cells[5, 80, 5, 97].Value = "Điểm 0.0 - 1.9";
                    ws.Cells[5, 80, 5, 97].Merge = true;
                    ws.Cells[5, 98, 5, 108].Style.Fill.PatternType = ExcelFillStyle.LightVertical;
                    ws.Cells[5, 98, 5, 108].Merge = true;
                    ws.Cells[5, 98, 5, 108].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(237, 67, 67));
                    //ws.Cells[5, 98, 5, 108].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Blue);
                    #endregion

                    #region row 6
                    ws.Cells[6, 80, 6, 97].Value = "Chưa học";
                    ws.Cells[6, 80, 6, 97].Merge = true;
                    ws.Cells[6, 98, 6, 108].Style.Fill.PatternType = ExcelFillStyle.DarkTrellis;
                    ws.Cells[6, 98, 6, 108].Merge = true;
                    //ws.Cells[6, 98, 6, 108].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(174,157,245));
                    ws.Cells[6, 98, 6, 108].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Blue);
                    #endregion

                    int colIndex = 1;
                    int rowIndex = 8;

                    //tạo các header từ column header đã tạo từ bên trên
                    //foreach (var item in arrColumnHeader)
                    for (var i = 0; i < 108; i++)
                    {
                        var item = arrColumnHeader.ElementAtOrDefault(i);
                        var cell = ws.Cells[rowIndex, colIndex];
                        //căn chỉnh các border
                        var border = cell.Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        cell.Style.WrapText = true;

                        //gán giá trị
                        cell.Value = item;
                        colIndex++;
                        if (colIndex == 8)
                        {
                            ws.Cells[rowIndex, 8, rowIndex, 108].Merge = true;
                        }
                        if (colIndex == 1)
                        {
                            ws.Column(colIndex).Width = 2.5;
                        }
                        if (colIndex == 2 || colIndex == 4 || colIndex == 5)
                        {
                            ws.Column(colIndex).Width = 15;
                        }
                        if (colIndex == 3)
                        {
                            ws.Column(colIndex).Width = 22;
                        }
                        if (colIndex == 6)
                        {
                            ws.Column(colIndex).Width = 7;
                        }
                        if (colIndex == 7)
                        {
                            ws.Column(colIndex).Width = 9;
                        }
                    }

                    // lấy ra danh sách UserInfo từ ItemSource của DataGrid
                    //List<UserInfo> userList = dtgExcel.ItemsSource.Cast<UserInfo>().ToList();
                    var datatest = data.GroupBy(x => x.ClassID);

                    for (int i = 8; i <= 8 + 100; i++)
                    {
                        ws.Column(i).Width = 1;
                    }

                    // với mỗi item trong danh sách sẽ ghi trên 1 dòng
                    for (Int32 index = 0; index < datatest.Count(); index++)
                    {
                        var item = datatest.ElementAtOrDefault(index);
                        var inforClass = item.FirstOrDefault();
                        // bắt đầu ghi từ cột 1. Excel bắt đầu từ 1 không phải từ 0
                        colIndex = 1;

                        // rowIndex tương ứng từng dòng dữ liệu
                        rowIndex++;

                        //gán giá trị cho từng cell        
                        {
                            var currentrow = rowIndex;
                            var currentcol = colIndex++;
                            var cell = ws.Cells[currentrow, currentcol, currentrow + item.Count() - 1, currentcol];
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            cell.Merge = true;
                            cell.Value = index + 1;
                            var border = cell.Style.Border;
                            border.Bottom.Style =
                                border.Top.Style =
                                border.Left.Style =
                                border.Right.Style = ExcelBorderStyle.Thin;
                        }
                        {
                            var currentrow = rowIndex;
                            var currentcol = colIndex++;
                            var cell = ws.Cells[currentrow, currentcol, currentrow + item.Count() - 1, currentcol];
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            cell.Merge = true;
                            cell.Value = $"{inforClass.ClassName}";
                            cell.Style.WrapText = true;
                            var border = cell.Style.Border;
                            border.Bottom.Style =
                                border.Top.Style =
                                border.Left.Style =
                                border.Right.Style = ExcelBorderStyle.Thin;
                        }
                        {
                            var currentrow = rowIndex;
                            var currentcol = colIndex++;
                            var cell = ws.Cells[currentrow, currentcol, currentrow + item.Count() - 1, currentcol];
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            cell.Merge = true;
                            cell.Value = $"{inforClass.TeacherName}";
                            cell.Style.WrapText = true;
                            var border = cell.Style.Border;
                            border.Bottom.Style =
                                border.Top.Style =
                                border.Left.Style =
                                border.Right.Style = ExcelBorderStyle.Thin;
                        }
                        {
                            var currentrow = rowIndex;
                            var currentcol = colIndex++;
                            var cell = ws.Cells[currentrow, currentcol, currentrow + item.Count() - 1, currentcol];
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            cell.Merge = true;
                            cell.Value = inforClass.StartDate.ToString("dd/MM/yyyy");
                            cell.Style.WrapText = true;
                            var border = cell.Style.Border;
                            border.Bottom.Style =
                                border.Top.Style =
                                border.Left.Style =
                                border.Right.Style = ExcelBorderStyle.Thin;
                        }
                        {
                            var currentrow = rowIndex;
                            var currentcol = colIndex++;
                            var cell = ws.Cells[currentrow, currentcol, currentrow + item.Count() - 1, currentcol];
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            cell.Merge = true;
                            cell.Value = inforClass.EndDate.ToString("dd/MM/yyyy");
                            cell.Style.WrapText = true;
                            var border = cell.Style.Border;
                            border.Bottom.Style =
                                border.Top.Style =
                                border.Left.Style =
                                border.Right.Style = ExcelBorderStyle.Thin;
                        }
                        {
                            var currentrow = rowIndex;
                            var currentcol = colIndex++;
                            var cell = ws.Cells[currentrow, currentcol, currentrow + item.Count() - 1, currentcol];
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            cell.Merge = true;
                            cell.Value = inforClass.StudentinClass;
                            var border = cell.Style.Border;
                            border.Bottom.Style =
                                border.Top.Style =
                                border.Left.Style =
                                border.Right.Style = ExcelBorderStyle.Thin;
                        }

                        for (Int32 j = 0; j < item.Count(); j++)
                        {
                            var _colIndex = colIndex;
                            var _item = item.ElementAtOrDefault(j);

                            if (item.Key == "5f5af539171ba81edc6ec410")
                            {
                                var a = "";
                            }

                            var persentMinPoint8 = Math.Round((((double)_item.MinPoint8 / _item.StudentinClass) * 100), 0, MidpointRounding.ToEven);
                            var persentMinPoint5 = Math.Round((((double)_item.MinPoint5 / _item.StudentinClass) * 100), 0, MidpointRounding.ToEven);
                            var persentMinPoint2 = Math.Round((((double)_item.MinPoint2 / _item.StudentinClass) * 100), 0, MidpointRounding.ToEven);
                            var persentMinPoint0 = Math.Round((((double)_item.MinPoint0 / _item.StudentinClass) * 100), 0, MidpointRounding.ToEven);
                            if (persentMinPoint0 + persentMinPoint2 + persentMinPoint5 + persentMinPoint8 > 100)
                            {
                                if (persentMinPoint0 != 0) persentMinPoint0 = 100 - persentMinPoint2 - persentMinPoint5 - persentMinPoint8;
                                else if (persentMinPoint0 == 0 && persentMinPoint2 != 0) persentMinPoint2 = 100 - persentMinPoint5 - persentMinPoint8;
                            }

                            var persentChuaLam = 100 - persentMinPoint0 - persentMinPoint2 - persentMinPoint5 - persentMinPoint8;

                            {
                                var cell = ws.Cells[rowIndex + j, _colIndex++];
                                cell.Value = $"Tháng { 9 + j}";
                                var border = cell.Style.Border;
                                border.Bottom.Style =
                                    border.Top.Style =
                                    border.Left.Style =
                                    border.Right.Style = ExcelBorderStyle.Thin;
                            }

                            Int32 colPerSent8 = _colIndex + (Int32)persentMinPoint8;
                            if (persentMinPoint8 > 0)
                            {
                                var cellColPersent8 = ws.Cells[rowIndex + j, colPerSent8];
                                ws.Cells[rowIndex + j, _colIndex, rowIndex + j, colPerSent8].Value = $"{persentMinPoint8}%";
                                //set màu thành lightgreen
                                var fill8 = ws.Cells[rowIndex + j, _colIndex, rowIndex + j, colPerSent8].Style.Fill;
                                fill8.PatternType = ExcelFillStyle.DarkDown;
                                fill8.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(134, 208, 141));
                                //fill8.BackgroundColor.SetColor(System.Drawing.Color.Blue);
                                ws.Cells[rowIndex + j, _colIndex, rowIndex + j, colPerSent8].Merge = true;
                                // in đậm
                                ws.Cells[rowIndex + j, _colIndex, rowIndex + j, colPerSent8].Style.Font.Bold = true;
                                // căn giữa
                                ws.Cells[rowIndex + j, _colIndex, rowIndex + j, colPerSent8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                //căn chỉnh các border
                                var border = ws.Cells[rowIndex + j, _colIndex, rowIndex + j, colPerSent8].Style.Border;
                                border.Bottom.Style =
                                    border.Top.Style =
                                    border.Left.Style =
                                    border.Right.Style = ExcelBorderStyle.Thin;
                            }
                            else
                            {
                                colPerSent8 -= 1;
                            }

                            Int32 colPerSent5 = colPerSent8 + (Int32)persentMinPoint5;
                            if (persentMinPoint5 > 0)
                            {
                                var cellColPersent5 = ws.Cells[rowIndex + j, colPerSent5];
                                //cellColPersent5.Value = $"{persentMinPoint5}%";
                                ws.Cells[rowIndex + j, colPerSent8 + 1, rowIndex + j, colPerSent5].Value = $"{persentMinPoint5}%";
                                //set màu thành lightgreen
                                var fill5 = ws.Cells[rowIndex + j, colPerSent8 + 1, rowIndex + j, colPerSent5].Style.Fill;
                                fill5.PatternType = ExcelFillStyle.Gray0625;
                                fill5.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(157, 195, 230));
                                //fill5.BackgroundColor.SetColor(System.Drawing.Color.Blue);
                                ws.Cells[rowIndex + j, colPerSent8 + 1, rowIndex + j, colPerSent5].Merge = true;
                                // in đậm
                                ws.Cells[rowIndex + j, colPerSent8 + 1, rowIndex + j, colPerSent5].Style.Font.Bold = true;
                                // căn giữa
                                ws.Cells[rowIndex + j, colPerSent8 + 1, rowIndex + j, colPerSent5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                var border = ws.Cells[rowIndex + j, colPerSent8 + 1, rowIndex + j, colPerSent5].Style.Border;
                                border.Bottom.Style =
                                    border.Top.Style =
                                    border.Left.Style =
                                    border.Right.Style = ExcelBorderStyle.Thin;
                            }

                            Int32 colPerSent2 = colPerSent5 + (Int32)persentMinPoint2;
                            if (persentMinPoint2 > 0)
                            {
                                var cellColPersent2 = ws.Cells[rowIndex + j, colPerSent2];
                                ws.Cells[rowIndex + j, colPerSent5 + 1, rowIndex + j, colPerSent2].Value = $"{persentMinPoint2}%";
                                var fill2 = ws.Cells[rowIndex + j, colPerSent5 + 1, rowIndex + j, colPerSent2].Style.Fill;
                                fill2.PatternType = ExcelFillStyle.DarkUp;
                                fill2.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 255, 102));
                                //fill2.BackgroundColor.SetColor(System.Drawing.Color.Blue);
                                ws.Cells[rowIndex + j, colPerSent5 + 1, rowIndex + j, colPerSent2].Merge = true;
                                // in đậm
                                ws.Cells[rowIndex + j, colPerSent5 + 1, rowIndex + j, colPerSent2].Style.Font.Bold = true;
                                // căn giữa
                                ws.Cells[rowIndex + j, colPerSent5 + 1, rowIndex + j, colPerSent2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                var border = ws.Cells[rowIndex + j, colPerSent5 + 1, rowIndex + j, colPerSent2].Style.Border;
                                border.Bottom.Style =
                                    border.Top.Style =
                                    border.Left.Style =
                                    border.Right.Style = ExcelBorderStyle.Thin;
                            }

                            Int32 colPerSent0 = colPerSent2 + (Int32)persentMinPoint0;
                            if (persentMinPoint0 > 0)
                            {
                                var cellColPersent0 = ws.Cells[rowIndex + j, colPerSent0];
                                ws.Cells[rowIndex + j, colPerSent2 + 1, rowIndex + j, colPerSent0].Value = $"{persentMinPoint0}%";
                                var fill0 = ws.Cells[rowIndex + j, colPerSent2 + 1, rowIndex + j, colPerSent0].Style.Fill;
                                fill0.PatternType = ExcelFillStyle.LightVertical;
                                fill0.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(237, 67, 67));
                                //fill0.BackgroundColor.SetColor(System.Drawing.Color.Blue);
                                ws.Cells[rowIndex + j, colPerSent2 + 1, rowIndex + j, colPerSent0].Merge = true;
                                // in đậm
                                ws.Cells[rowIndex + j, colPerSent2 + 1, rowIndex + j, colPerSent0].Style.Font.Bold = true;
                                // căn giữa
                                ws.Cells[rowIndex + j, colPerSent2 + 1, rowIndex + j, colPerSent0].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                var border = ws.Cells[rowIndex + j, colPerSent5 + 1, rowIndex + j, colPerSent0].Style.Border;
                                border.Bottom.Style =
                                    border.Top.Style =
                                    border.Left.Style =
                                    border.Right.Style = ExcelBorderStyle.Thin;
                            }

                            Int32 colPerSent = colPerSent0 + (Int32)persentChuaLam;
                            if (persentChuaLam > 0)
                            {
                                if (persentMinPoint8 == 0)
                                {
                                    var cellCollPersent = ws.Cells[rowIndex + j, colPerSent + 1];
                                    ws.Cells[rowIndex + j, colPerSent0 + 1, rowIndex + j, colPerSent + 1].Value = $"{persentChuaLam}%";
                                    var fill = ws.Cells[rowIndex + j, colPerSent0 + 1, rowIndex + j, colPerSent + 1].Style.Fill;
                                    fill.PatternType = ExcelFillStyle.DarkTrellis;
                                    fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(174, 157, 245));
                                    //fill.BackgroundColor.SetColor(System.Drawing.Color.Blue);
                                    ws.Cells[rowIndex + j, colPerSent0 + 1, rowIndex + j, colPerSent + 1].Merge = true;
                                    // in đậm
                                    ws.Cells[rowIndex + j, colPerSent0 + 1, rowIndex + j, colPerSent + 1].Style.Font.Bold = true;
                                    // căn giữa
                                    ws.Cells[rowIndex + j, colPerSent0 + 1, rowIndex + j, colPerSent + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    var border = ws.Cells[rowIndex + j, colPerSent5 + 1, rowIndex + j, colPerSent + 1].Style.Border;
                                    border.Bottom.Style =
                                        border.Top.Style =
                                        border.Left.Style =
                                        border.Right.Style = ExcelBorderStyle.Thin;
                                }
                                else
                                {
                                    var cellCollPersent = ws.Cells[rowIndex + j, colPerSent];
                                    ws.Cells[rowIndex + j, colPerSent0 + 1, rowIndex + j, colPerSent].Value = $"{persentChuaLam}%";
                                    var fill = ws.Cells[rowIndex + j, colPerSent0 + 1, rowIndex + j, colPerSent].Style.Fill;
                                    fill.PatternType = ExcelFillStyle.DarkTrellis;
                                    fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(174, 157, 245));
                                    //fill.BackgroundColor.SetColor(System.Drawing.Color.Blue);
                                    ws.Cells[rowIndex + j, colPerSent0 + 1, rowIndex + j, colPerSent].Merge = true;
                                    // in đậm
                                    ws.Cells[rowIndex + j, colPerSent0 + 1, rowIndex + j, colPerSent].Style.Font.Bold = true;
                                    // căn giữa
                                    ws.Cells[rowIndex + j, colPerSent0 + 1, rowIndex + j, colPerSent].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    var border = ws.Cells[rowIndex + j, colPerSent5 + 1, rowIndex + j, colPerSent].Style.Border;
                                    border.Bottom.Style =
                                        border.Top.Style =
                                        border.Left.Style =
                                        border.Right.Style = ExcelBorderStyle.Thin;
                                }
                            }
                        }
                        rowIndex += item.Count();
                        ws.Cells[rowIndex, 1, rowIndex, 108].Merge = true;
                        ws.Row(rowIndex).Height = 15;

                    }

                    //Lưu file lại
                    Byte[] bin = p.GetAsByteArray();
                    File.WriteAllBytes($"H:\\Hoa\\BenTre\\{Month}{dataCenter.CenterName}{DateTime.Now.ToString("HHmmssddMMyyyy")}v2.xlsx", bin);
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        private static Dictionary<Int32, DataTime> GetListTime(DateTime currentTime)
        {
            var currentMonth = currentTime.Month;
            var currentYear = currentTime.Year;
            var firstDayofMonth = new DateTime(currentYear, currentMonth, 1, 00, 00, 00);
            var lastDayofMonth = firstDayofMonth.AddMonths(1).AddMilliseconds(-1);
            Dictionary<Int32, DataTime> data = new Dictionary<int, DataTime>();
            data.Add(currentMonth, new DataTime { StartTime = firstDayofMonth, EndTime = lastDayofMonth });
            while (lastDayofMonth.Month <= 12)
            {
                firstDayofMonth = firstDayofMonth.AddMonths(1);
                lastDayofMonth = firstDayofMonth.AddMonths(1).AddMilliseconds(-1);
                var key = firstDayofMonth.Month;
                data.Add(key, new DataTime { StartTime = firstDayofMonth, EndTime = lastDayofMonth });
            }
            return data;
        }
        #endregion

        #region Service renewal notice
        private static async Task<String> ServiceRenewalNotice()
        {
            try
            {
                var currentTime = DateTime.UtcNow;
                //lay danh sach co so dang hoat dong
                var centers = _centerService.CreateQuery().Find(x=>x.Status == true).ToList();
                //var centers = _centerService.CreateQuery().Find(t => t.ExpireDate >= currentTime && t.Status == true).ToList();
                foreach (var item in centers)
                {
                    //lay danh sach lop dang hoat dong
                    var classesActive = _classService.GetActiveClass4Report(item.StartDate, item.ExpireDate, item.ID);
                    if (classesActive.Count() == 0) continue;

                    var endTime = item.ExpireDate;
                    var totalTotalMilliseconds = currentTime.Subtract(endTime).TotalMilliseconds;
                    var totalTotalDays = currentTime.Subtract(endTime).TotalDays;
                    if (totalTotalDays >= 0 && totalTotalDays <= 7 && item.ExpireDate.Month == currentTime.Month)
                    {
                        //gui mail thong bao het han
                        var listTeacherHeader = _teacherService.CreateQuery().Find(x => x.IsActive == true && x.Centers.Any(y => y.CenterID == item.ID)).ToList().FindAll(y => HasRole(y.ID, item.ID, "head-teacher")).ToList();
                        if (listTeacherHeader.Any(x => x.Email == "huonghl@utc.edu.vn"))
                        {
                            listTeacherHeader.RemoveAt(listTeacherHeader.FindIndex(x => x.Email == "huonghl@utc.edu.vn"));
                        }

                        if (listTeacherHeader.Count == 0) continue;

                        String listNameTeachers = "";
                        List<String> listEmailTeachers = new List<string>();
                        foreach(var teacher in listTeacherHeader)
                        {
                            listNameTeachers += $"{teacher.FullName}, ";
                            listEmailTeachers.Add(teacher.Email);
                        }
                        listNameTeachers = listNameTeachers.Remove(listNameTeachers.LastIndexOf(",")).Trim() + ".";
                        String subject = "THÔNG BÁO GIA HẠN DỊCH VỤ";

                        var thead = "<thead>" +
                            "<tr>" +
                            "<td style='padding: 0.75pt;font-size:18pt;font-family:Arial,sans-serif;font-weight:bold;text-align: center'>THƯ THÔNG BÁO GIA HẠN DỊCH VỤ</td>" +
                            "</tr>" +
                            "<tr>" +
                            "<td style='padding: 0.75pt;font-size:8pt;font-family:Arial,sans-serif;text-align: center;font-style: italic'>(Thành thật xin lỗi, nếu trong thời gian thư đang chuyển đi mà quý thầy cô đã thực hiện gia hạn)</td>" +
                            "</tr>" +
                            "<tr>" +
                            $"<td style='padding: 0.75pt;font-size:8pt;font-family:Arial,sans-serif;text-align: center'>Ngày gửi: {DateTime.Now.ToString("dd-MM-yyyy")}</td>" +
                            "</tr>" +
                            "</thead>";
                        var tbody = "<tbody>" +
                            "<tr>" +
                            $"<td style='font-size:10pt;font-family:Helvetica,sans-serif'>Kính gửi: thầy/cô <span style='font-weight: bold;font-family:\"Segoe UI\",sans-serif; color: black; background-image:initial; background-position:initial; background-size:initial; background-repeat:initial; background-origin:initial; background-clip:initial'>{listNameTeachers}<span></td>" +
                            "</tr>" +
                            "<tr>" +
                            $"<td style='font-size:10pt;font-family:Helvetica,sans-serif'>Lời đầu tiên, <span style='font-weight:bold'>EDUSO</span> xin trân trọng cảm ơn sự hợp tác giữa <span style='font-weight:bold'>{item.Name}</span> với <span style='font-weight:bold'>EDUSO</span> về việc sử dụng Eduso Platform trong giảng dạy và học tập.</td>" +
                            "</tr>" +
                            "<tr>" +
                            $"<td style='font-size:10pt;font-family:Helvetica,sans-serif'>Theo như thời gian sử dụng dịch vụ của <span style='font-weight:bold'>{item.Name}</span> với <span style='font-weight:bold'>EDUSO</span> từ <span style='font-weight:bold'>{item.StartDate.ToUniversalTime().ToString("HH:mm dd/MM/yyyy")}</span> đến <span style='font-weight:bold'>{item.ExpireDate.ToLocalTime().ToString("HH:mm dd/MM/yyyy")}.</span></td>" +
                            "</tr>" +
                            "<tr>" +
                            $"<td style='font-size:10pt;font-family:Helvetica,sans-serif'><span style='font-weight:bold'>EDUSO</span> nhận thấy <span style='font-weight:bold'>{item.Name}</span> sắp đến ngày hết hạn sử dụng, sau ngày hết hạn thì các dịch vụ trên hệ thống Eduso sẽ tạm dừng hoạt động.</td>" +
                            "</tr>" +
                            "<tr>" +
                            $"<td style='font-size:10pt;font-family:Helvetica,sans-serif'>Để gia hạn dịch vụ, vui lòng liên hệ bộ phận chăm sóc khách hàng theo số điện thoại 0989085398 - 02432444439</td>" +
                            "</tr>" +
                            "<tr>" +
                            $"<td style='font-size:10pt;font-family:Helvetica,sans-serif'>Chân thành cảm ơn sự hợp tác của <span style='font-weight:bold'>{item.Name}</span> với <span style='font-weight:bold'>EDUSO</span>.</td>" +
                            "</tr>" +
                            "<tr>" +
                            $"<td style='font-size:10pt;font-family:Helvetica,sans-serif;color:#0000ff;text-align: center'>Trân trọng!</td>" +
                            "</tr>" +
                            "<tr>" +
                            $"<td style='font-size:10pt;font-family:Helvetica,sans-serif;color:#0000ff;text-align: center'>----------------o0o-----------</td>" +
                            "</tr>" +
                            "</tbody>";

                        var table2 = @"<table cellpadding='0' cellspacing='0' style='margin:auto;border-spacing:0px; border-collapse:collapse; color: rgb(68, 68, 68); width: 480px; font-size:9pt; font-family:Arial,sans-serif; line-height:14px'><tbody><tr><td colspan='2' style='padding: 0px 0px 5px; width: 480px'><font color='#134f5c'><font face='Arial, sans-serif'><span style='font-size:14.6667px'><b>EDU</b></span><span style='font-weight:bold;font-size:14.6667px'>SO</span></font><b style='font-size:9pt'>.,JSC</b></font><font color='#ff9900'><font face='Arial, sans-serif'><b><span style='font-size:14.6667px'><br></span></b></font></font></td></tr><tr><td colspan='2' style='padding:0px 0px 5px;width:480px;color:rgb(23,147,210)'></td></tr><tr><td colspan='2' style='padding:0px 0px 2px;width:480px;border-top:1px dotted rgb(19,19,19)'>&nbsp;</td></tr><tr><td valign='middle' style='padding:0px;width:120px;vertical-align:middle'><img src='https://ci3.googleusercontent.com/proxy/QAgPRZBFrmIBlGmpla2ITc2bGA9bDK-jAclDqfTcfevFuuKJSYrVRL3tBSDbre6GDcdcRw7fRTECfG5ANvDxMvkQiXZbAd56B4pNx3a8cSg2THUFbi0kScAaG8p1jrykO5s4=s0-d-e1-ft#https://drive.google.com/uc?id=1P2PWW0Do8B1PfWW-r1Kjy_tDK2mp92ma&amp;export=download' width='96' height='95' class='CToWUd'></td><td valign='middle' style='font-family:Arial,sans-serif;padding:0px;width:360px;color:rgb(19,19,19);vertical-align:middle'><table cellpadding='0' cellspacing='0' style='border-spacing:0px;border-collapse:collapse;background-color:transparent'><tbody><tr><td style='padding:1px'><br></td></tr><tr><td style='padding:1px'><span style='font-family:Arial,sans-serif;font-size:9pt'>Hotline: (+84)989 085 398</span></td></tr><tr><td style='font-family:Arial,sans-serif;padding:1px;font-size:9pt'><span style='font-size:9pt'>Mail:&nbsp;<a href='http://buithihong98@gmail.com/' target='_blank' data-saferedirecturl='https://www.google.com/url?q=http://buithihong98@gmail.com/&amp;source=gmail&amp;ust=1609476250700000&amp;usg=AFQjCNETDHvUCqOIe01CJEJwgt6gTrPCkw'>eduso.vn@gmail.com</a></span>&nbsp;</td></tr><tr><td style='padding:1px'><span style='background-color:transparent'><font face='Arial, sans-serif'><span style='font-size:12px'>Trường&nbsp;</span></font><span style='font-size:9pt'>Đại học Giao thông vận tải -&nbsp;</span></span><span style='background-color:transparent'><font face='Arial, sans-serif'><span style='font-size:12px'>Số 3 Phố Cầu Giâ</span></font></span><font style='background-color:transparent;font-size:12px' face='Arial, sans-serif'>y <br>-&nbsp;</font><font face='Arial, sans-serif' style='background-color:transparent'><span style='font-size:12px'>Láng Thượng - Đống Đa -&nbsp;</span></font><span style='background-color:transparent;font-size:9pt'>Hà Nội -&nbsp; Việt Nam</span><font face='Arial, sans-serif' style='background-color:transparent;font-size:9pt'><br></font></td></tr><tr><td style='padding:2px 0px 0px 1px'><span style='display:inline-block;height:23px'><a href='https://www.facebook.com/engcooacademy/' style='background-color:transparent;color:rgb(51,122,183)' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://www.facebook.com/engcooacademy/&amp;source=gmail&amp;ust=1609476250700000&amp;usg=AFQjCNGKOaTRls1lZt_lcFHOqghSGBtnLQ'><img alt='Facebook icon' border='0' width='23' height='23' src='https://ci3.googleusercontent.com/proxy/mjiWTZdOYIWVF9PvTDENxZ6wQOuPAlFcn8ifTjfAuWRilo8dSpvX4l41IZgRY291qHCNVuF42W5YnOsux7SW-_NZ_nzBFoyx9RXY096p16IIPundLZxXuJgdlBEEyn-qrPROAmxOkxQ=s0-d-e1-ft#https://codetwocdn.azureedge.net/images/mail-signatures/generator/compact-logo/fb.png' style='border:0px;vertical-align:middle;height:23px;width:23px' class='CToWUd'></a>&nbsp;&nbsp;<a href='https://www.youtube.com/channel/UCC4hOZxED2bHeZJSZT8jydw?view_as=subscriber' style='background-color:transparent;color:rgb(51,122,183)' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://www.youtube.com/channel/UCC4hOZxED2bHeZJSZT8jydw?view_as%3Dsubscriber&amp;source=gmail&amp;ust=1609476250700000&amp;usg=AFQjCNGfTeFFzglJMtBFYrt1A1mPlgwklw'><img alt='Youtbue icon' border='0' width='23' height='23' src='https://ci4.googleusercontent.com/proxy/3yJ04s_GTj8Eck5ACth9ub3_mXYz9hUld0f6NMft-hC_i9M91dE06tpBbNOnc--l8okk-gS8GKbpichkfW88R2CG3u_8n0H_bU6tVdcUcTjLX8JkjbKpSJuY2xg2PWulNFSbyAuW60M=s0-d-e1-ft#https://codetwocdn.azureedge.net/images/mail-signatures/generator/compact-logo/yt.png' style='border:0px;vertical-align:middle;height:23px;width:23px' class='CToWUd'></a>&nbsp;&nbsp;</span></td></tr></tbody></table></td></tr><tr><td colspan='2' style='padding:2px 0px 0px;width:480px;border-bottom:1px dotted rgb(19,19,19)'>&nbsp;</td></tr></tbody></table>";

                        var body = $"<table style='margin: auto'>{thead}{tbody}</table>{table2}";

                        var toAddress = isTest == true ? new List<string> { "nguyenvanhoa2017602593@gmail.com","k.chee.dinh@gmail.com", "vietphung.it@gmail.com" } : listEmailTeachers;
                        //var toAddress = isTest == true ? new List<string> { "nguyenvanhoa2017602593@gmail.com", "k.chee.dinh@gmail.com" } : new List<string> { "nguyenvanhoa2017602593@gmail.com" };
                        var bccAddress = isTest == true ? null : new List<string> { "nguyenhoa.dev@gmail.com", "vietphung.it@gmail.com", "huonghl@utc.edu.vn" };
                        _ = await _mailHelper.SendBaseEmail(toAddress, subject, body, MailPhase.WEEKLY_SCHEDULE, null, bccAddress);
                        //_ = await _mailHelper.SendBaseEmail(toAddress, subject, body, MailPhase.WEEKLY_SCHEDULE);
                    }
                }
                return "Thong bao thanh cong";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

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

        private static Dictionary<Int32, DataTime> GetListMonth(DateTime startTime,DateTime endTime)
        {
            var month = startTime.Month;
            var year = startTime.Year;
            var firstTime = new DateTime(year, month, 1, 0, 0, 0);
            //List<DataTime> data = new List<DataTime>();
            Dictionary<Int32, DataTime> data = new Dictionary<Int32, DataTime>();
            var timeMonth = new DataTime()
            {
                StartTime = firstTime,
                EndTime = firstTime.AddMonths(1).AddMinutes(-1)
            };
            data.Add(month,timeMonth);

            while(timeMonth.EndTime.Month <= 12 && year == 2020)
            {
                firstTime = timeMonth.StartTime.AddMonths(1);
                var lastTime = firstTime.AddMonths(1).AddMinutes(-1);
                month = firstTime.Month;
                timeMonth.StartTime = firstTime;
                timeMonth.EndTime = lastTime;
                data.Add(month,timeMonth);
            }
            return data;
        }

        #region ds hoc sinh chua lam cs ben tre
        public static async Task BenTre()
        {
            var center = _centerService.CreateQuery().Find(x=>x.Abbr == "c3btvp").FirstOrDefault();
            var listClass = _classService.CreateQuery().Find(x => x.Center == center.ID).ToList();
            foreach(var item in listClass)
            {
                var students = _studentService.GetStudentsByClassId(item.ID);
                var lessonProgess = _lessonProgressService.CreateQuery().Find(x => x.ClassID == item.ID && x.TotalLearnt == 0).ToList().GroupBy(x => x.StudentID)
                    .Select(x => new 
                    {
                        StudentName = _studentService.GetItemByID(x.Key).FullName,
                        ClassName = item.Name
                    });
                var a = "";
            }
        }
        #endregion
    }

    #region class
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

    public class Class4Report2Excel
    {
        public String ClassID { get; set; }
        public String ClassName { get; set; }
        public String TeacherName { get; set; }
        public String CenterID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Int32 StudentinClass { get; set; }
        public String Status { get; set; }

        // phan nay se tach rieng ra class
        public Int32 DontActiveStudent { get; set; }
        public Int32 MinPoint8 { get; set; }
        public Int32 MinPoint5 { get; set; }
        public Int32 MinPoint2 { get; set; }
        public Int32 MinPoint0 { get; set; }
        public Int32 DontLeant { get; set; }

    }

    public class Center4Report2Excel
    {
        public String CenterID { get; set; }
        public String CenterName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Int32 Limit { get; set; }
        public Int32 TotalTeachersinCenter { get; set; }
        public Int32 TotalStudentsinCenter { get; set; }
        public Int32 TotalClassActive { get; set; }
        public Int32 TotalClass { get; set; }
        public Int32 DaHoc { get; set; }
    }

    public class DataTime
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
    #endregion
}
