using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
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

            isTest = configuration["Test"] == "1";
            isTest = true;

            Console.WriteLine("Processing Schedule...");

            if (!args.Any())
            {
                //default
            }
            else
            {
                switch (args[0])
                {
                    case "SendWeeklyReport":
                        await SendWeeklyReport();
                        break;
                    default:
                        break;
                }
            }

            //await SendIncomingLesson();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            await SendWeeklyReport();
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
            Console.ReadKey();
        }

        public static async Task SendWeeklyReport()
        {
            var currentTime = DateTime.Now;
            //var currentTime = new DateTime(2020,03,30);
            var startWeek = currentTime.AddDays(DayOfWeek.Sunday - currentTime.DayOfWeek);
            var endWeek = startWeek.AddDays(7);
            var centersActive = _centerService.GetActiveCenter(currentTime);//lay co so dang hoat dong
            foreach (var center in centersActive)
            {
                if (center.Abbr == "c3vyvp")//test truong Vinh Yen
                {
                    var teacherHeader = _teacherService.CreateQuery().Find(x => x.IsActive == true && x.Centers.Any(y => y.CenterID == center.ID)).ToList().FindAll(y => HasRole(y.ID, center.ID, "head-teacher"));
                    if (center.Code == "eduso")
                    {
                        teacherHeader.Remove(teacherHeader.Find(x => x.Email == "huonghl@utc.edu.vn"));
                    }

                    var subject = "";
                    subject += $"Báo cáo kết quả học tập của {center.Name} từ ngày {startWeek.ToString("dd-MM-yyyy")} đến ngày {endWeek.ToString("dd-MM-yyyy")}";
                    var body = "";
                    body = @"<table style='margin-top:20px; width: 100%; border: solid 1px #333; border-collapse: collapse'>
                            <thead>
                                <tr style='background-color: bisque'>" +
                                                    $"<th colspan='9' style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Báo cáo kết quả học tập từ ngày {startWeek.ToString("dd-MM-yyyy")} đến ngày {endWeek.ToString("dd-MM-yyyy")}</th>" +
                                                @"</tr>
                                        <tr style='font-weight:bold;background-color: bisque'>
                                            <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse'>STT</td>
                                            <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Lớp</td>
                                            <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:70px'>Sĩ số lớp</td>
                                            <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:70px'>Số học sinh chưa đăng nhập hệ thống</td>
                                            <td colspan='3' style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Số học sinh tham gia làm bài kiểm tra</td>
                                            <td colspan='2' rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Tiến độ học tập</td>
                                        </tr>
                                        <tr style='font-weight:bold;background-color: bisque'>
                                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:70px'>Số học sinh đạt kết quả > 5</td>
                                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:70px'>Số HS điểm 0</td>
                                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:70px'>Số HS đạt điểm <= 5</td>
                                        </tr>
                                    </thead>
                                    <tbody>";
                    //< td style = 'text-align:center; border: solid 1px #333; border-collapse: collapse' > Tài liệu chính quy </ td >
     
                    //                             < td style = 'text-align:center; border: solid 1px #333; border-collapse: collapse' > Tài liệu chuyên đề </ td >
                              var tbody = "";
                    tbody += "<tbody>";
                    var classesActive = _classService.GetActiveClass(currentTime, center.ID);//lay danh sach lop dang hoat dong
                    var index = 1;
                    long totalStudent = 0, totalstChuaVaoLop = 0; ;
                    long lonhon5 = 0;
                    long nhohon5 = 0;
                    long diem0 = 0;
                    var lasttime = new Dictionary<DateTime, string>();
                    string[] style = { "background-color: aliceblueT", "background-color: whitesmoke" };

                    foreach (var _class in classesActive)
                    {
                        var students = _studentService.GetStudentsByClassId(_class.ID);
                        totalStudent += students.Count();
                        var stChuaVaoLop = 0;
                        foreach (var st in students.ToList())
                        {
                            var time = _learningHistory.CreateQuery().Find(x => x.StudentID == st.ID).ToList();
                            if (time.Count == 0)
                            {
                                stChuaVaoLop++;
                            }
                            else
                            {
                                var time1 = time.OrderByDescending(x => x.Time).FirstOrDefault().Time;
                                if (time1 <= startWeek)
                                {
                                    stChuaVaoLop++;
                                }
                            }
                        }

                        totalstChuaVaoLop += stChuaVaoLop;

                        if (index % 1 == 0)
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
                            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{students.Count()}</td>" +
                            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{stChuaVaoLop}</td>";

                        List<double> points = new List<double>();
                        var classSbjes_active = _classSubjectService.CreateQuery().Find(o => o.StartDate <= endWeek && o.EndDate >= startWeek && o.TotalExams > 0 && o.ClassID == _class.ID).ToEnumerable();//danh sach mon hoc trong lop dang hoat dong
                        if (classSbjes_active.Count() != 0)
                        {
                            foreach (var classSbj_active in classSbjes_active)
                            {
                                var lessonshedules = _lessonScheduleService.CreateQuery().Find(o => o.StartDate <= endWeek && o.EndDate >= startWeek && o.ClassSubjectID == classSbj_active.ID)?.ToEnumerable();//danh sach bai hoc dang hoat dong
                                if (lessonshedules.Count() != 0)
                                {
                                    foreach (var lessonshedule in lessonshedules)
                                    {
                                        var lessones = _lessonService.CreateQuery().Find(x => x.TemplateType == 2 && x.ID == lessonshedule.LessonID).Project(x => x.ID).ToList();//danh sach bai kiem tra
                                        foreach (var student in students)
                                        {
                                            if (lessones.Count() == 0)
                                            {
                                                continue;
                                            }
                                            else
                                            {
                                                var lessonsprogess = _lessonProgressService.CreateQuery().Find(x => x.StudentID == student.ID && lessones.Contains(x.LessonID)).ToEnumerable();//danh sach lessonsprogess
                                                if (lessonsprogess.Count() == 0)
                                                {
                                                    //double a = 0;
                                                    points.Add(0);//khong co thi mac dinh la diem 0
                                                }
                                                else
                                                {
                                                    points.Add(lessonsprogess.Select(x => x.LastPoint).Sum() / lessonsprogess.Count());
                                                }
                                            }
                                        }
                                    }
                                }

                                var a = _learningHistory.CreateQuery().Find(x => x.ClassSubjectID == classSbj_active.ID).ToList();
                                var lastTimes = from t in a
                                                group t by t.LessonID
                                                into g
                                                select new
                                                {
                                                    LessonID = g.Key,
                                                    LastTime = (from t2 in g select t2.Time).Max()
                                                };
                                foreach (var lastTime in lastTimes)
                                {
                                    var course = _courseService.GetItemByID(_lessonService.GetItemByID(lastTime.LessonID).CourseID).Name;
                                    if (lasttime.Values.Contains(course))
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        lasttime.Add(lastTime.LastTime, course);
                                    }
                                }
                            }
                        }
                        tbody += $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{points.Where(x => x > 5).Count()}</td>" +
                            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{points.Where(x => x == 0).Count()}</td>" +
                            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{points.Where(x => x <= 5).Count()}</td>" +
                            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>";

                        foreach (var i in lasttime.Distinct())
                        {
                            tbody += "<table>" +
                                "<tr>" +
                                $"<td>Môn {i.Value}</td>" +
                                $"<td>Lần học cuối {i.Key}</td>" +
                                "</tr>" +
                                "</table>";
                        }
                        lonhon5 += points.Where(x => x > 5).Count();
                        nhohon5 += points.Where(x => x <= 5).Count();
                        diem0 += points.Where(x => x == 0).Count();
                        index++;
                    }

                    tbody += @"</td><tr style='font-weight: 600;background-color: yellow'>
                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>
                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;text-align: left'>Tổng</td>" +
                               $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{totalStudent}</td>" +
                               $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{totalstChuaVaoLop}</td>" +
                               $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{lonhon5}</td>" +
                               $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{diem0}</td>" +
                               $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{nhohon5}</td>" +
                               $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>" +
                               //$"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>" +
                               @"</tr>
                                <tbody>
                                </table>";
                    //<tr style='font-weight: 600;background-color: yellow'>
                    //<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>
                    //<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;text-align: left'>Tỉ lệ %</td>
                    //<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>
                    //<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>
                    //<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>
                    //<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>
                    //<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>
                    //<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>
                    //<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>
                    //</tr>

                    body += tbody;

                    var toAddress = new List<string> { "nguyenvanhoa2017602593@gmail.com" };
                    _ = await _mailHelper.SendBaseEmail(new List<string>(), subject, body, MailPhase.WEEKLY_SCHEDULE, null, toAddress);
                }
            }
        }

        //public static async Task SendWeeklyReport()
        //{
        //    //quet co so dang active
        //    //lay mail gv quan ly co so (tru mail huongho@utc.edu.vn dua vao bcc)
        //    //quet danh sach lop dang hoat dong trong co so, trong moi lop quet tung hoc lieu
        //    //thong ke tinh trang hoat dong cua tung lop: sl hoc sinh, so bai hoc trong tuan, so hoc sinh tham gia, diem hoc sinh....
        //    //test: eduso
        //    //var currentTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddHours(1);
        //    var currentTime = new DateTime(2020, 9, 20, 0, 0, 0).AddHours(1);
        //    var startWeek = currentTime.AddDays(DayOfWeek.Sunday - currentTime.DayOfWeek);
        //    var endWeek = startWeek.AddDays(7);

        //    var activeCenters = _centerService.GetActiveCenter(currentTime);//xem lai luc luu la gio GTM hay gi

        //    if (activeCenters != null && activeCenters.Count() > 0)
        //    {
        //        foreach (var center in activeCenters.ToList())
        //        {
        //            if (center.Abbr == "c3vyvp")//test truong Vinh Yen
        //            {
        //                string body = $"Xin chào Thầy/Cô," +
        //                @"<table style='margin-top:20px; width: 100%; border: solid 1px #333; border-collapse: collapse'>
        //                    <thead>
        //                        <tr style='background-color: bisque'>" +
        //                                $"<th colspan='9' style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Báo cáo kết quả học tập từ ngày {startWeek.ToString("dd-MM-yyyy")} đến ngày {endWeek.ToString("dd-MM-yyyy")}</th>" +
        //                            @"</tr>
        //                        <tr style='font-weight:bold;background-color: bisque'>
        //                            <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse'>STT</td>
        //                            <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Lớp</td>
        //                            <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:70px'>Sĩ số lớp</td>
        //                            <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:70px'>Số học sinh chưa đăng nhập hệ thống</td>
        //                            <td colspan='3' style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Số học sinh tham gia làm bài kiểm tra</td>
        //                            <td colspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Tiến độ học tập</td>
        //                        </tr>
        //                        <tr style='font-weight:bold;background-color: bisque'>
        //                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:70px'>Số học sinh đạt kết quả > 5</td>
        //                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:70px'>Số HS điểm 0</td>
        //                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;width:70px'>Số HS đạt điểm <= 5</td>
        //                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Tài liệu chính quy</td>
        //                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Tài liệu chuyên đề</td>
        //                        </tr>
        //                    </thead>
        //                    <tbody>";
        //                var tbody = "";
        //                string subject = $"Báo cáo kết quả học tập của {center.Name.ToUpper()} từ ngày {startWeek.ToString("dd-MM-yyyy")} đến ngày {endWeek.ToString("dd-MM-yyyy")}";
        //                var teacherHeader = _teacherService.CreateQuery().Find(x => x.IsActive == true && x.Centers.Any(y => y.CenterID == center.ID)).ToList().FindAll(y => HasRole(y.ID, center.ID, "head-teacher"));
        //                if (center.Code == "eduso")
        //                {
        //                    teacherHeader.Remove(teacherHeader.Find(x => x.Email == "huonghl@utc.edu.vn"));
        //                }

        //                var activeClasses = _classService.GetActiveClass(time: currentTime, Center: center.ID).ToList();
        //                var index = 1;
        //                string[] style = { "background-color: aliceblueT", "background-color: whitesmoke" };
        //                long totalStudent = 0;
        //                if (activeClasses != null && activeClasses.Count() > 0)
        //                {
        //                    foreach (var _class in activeClasses)
        //                    {
        //                        var students = _studentService.CreateQuery().Find(x => x.JoinedClasses.Contains(_class.ID));//lay danh sach sinh vien trong lop
        //                        var stChuaVaoLop = 0;
        //                        //List<LessonEntity> lessons = null;
        //                        //foreach (var st in students.ToList())
        //                        //{
        //                        //    var time = _learningHistory.CreateQuery().Find(x => x.StudentID == st.ID).ToList();
        //                        //    if (time.Count == 0)
        //                        //    {
        //                        //        stChuaVaoLop++;
        //                        //    }
        //                        //    else
        //                        //    {
        //                        //        var time1 = time.OrderByDescending(x => x.Time).FirstOrDefault().Time;
        //                        //        if (time1 <= startWeek)
        //                        //        {
        //                        //            stChuaVaoLop++;
        //                        //        }
        //                        //    }
        //                        //}

        //                        //var countStudent = _studentService.GetStudentsByClassId(_class.ID).Count();//si so lop
        //                        var countStudent = students.Count();//si so lop
        //                        totalStudent += countStudent;
        //                        var classSubjects = _classSubjectService.CreateQuery().Find(x => (x.EndDate > startWeek && x.StartDate < endWeek) && x.ClassID == _class.ID); //so mon hoc trong tuan dang active
        //                        if (index % 2 == 0)
        //                        {
        //                            tbody += $"<tr style='{style[1]}'>";
        //                        }
        //                        else
        //                        {
        //                            tbody += $"<tr style='{style[0]}'>";
        //                        }
        //                        if (stChuaVaoLop == countStudent)
        //                        {
        //                            tbody +=
        //                                $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{index}</td>" +
        //                                $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;text-align: left'>{_class.Name}</td>" +
        //                                $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{countStudent}</td>" +
        //                                $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{stChuaVaoLop}</td>" +
        //                                $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>0</td>" +
        //                                $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>0</td>" +
        //                                $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>0</td>" +
        //                                $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>0</td>" +
        //                            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>0</td></tr>";
        //                        }
        //                        else
        //                        {

        //                            var hoclieuchinhquy = "";
        //                            var hoclieuchuyende = "";
        //                            List<double> points = null;
        //                            if (classSubjects != null && classSubjects.CountDocuments() > 0)
        //                            {
        //                                foreach (var classsb in classSubjects.ToList())
        //                                {
        //                                    var lessonschedules = _lessonScheduleService.CreateQuery().Find(x => x.ClassSubjectID == classsb.ID && x.StartDate <= endWeek && x.EndDate >= startWeek).Project(x => x.LessonID)?.ToList();
        //                                    foreach (var student in students.ToList())
        //                                    {
        //                                        var a = _lessonProgressService.CreateQuery().Find(x => x.StudentID == student.ID && lessonschedules.Contains(x.LessonID));
        //                                        if (a.CountDocuments() != 0)
        //                                        {
        //                                            points.Add(a.ToList().Average(x => x.LastPoint));
        //                                        }
        //                                        //points.Add(?.ToList().Sum(x=>x.LastPoint));
        //                                    }

        //                                    tbody +=
        //                                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{index}</td>" +
        //                                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse;text-align: left'>{_class.Name}</td>" +
        //                                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{countStudent}</td>" +
        //                                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{stChuaVaoLop}</td>" +
        //                                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{points?.Where(x => x > 5).Count()}</td>" +
        //                                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{points?.Where(x => x == 0).Count()}</td>" +
        //                                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{points?.Where(x => x == 0).Count()}</td>";
        //                                    if (lessonschedules.Count() > 0)
        //                                    {
        //                                        if (classsb.TypeClass == CLASS_TYPE.STANDARD)
        //                                        {
        //                                            var courses = _courseService.CreateQuery().Find(x => x.ID == classsb.CourseID);
        //                                            foreach (var course in courses.ToList())
        //                                            {
        //                                                hoclieuchinhquy += $"<table style='width:100%'>" +
        //                                                    $"<tr>" +
        //                                                    $"<td style='width:50%;text-align:left'>{course.Name} :</td>" +
        //                                                    $"<td style='width:50%;text-align:left'>{lessonschedules.Count()} bài học đang diễn ra</td>" +
        //                                                    $"</tr>" +
        //                                                    $"</table>";
        //                                            }
        //                                        }
        //                                        else
        //                                        {
        //                                            var courses = _courseService.CreateQuery().Find(x => x.ID == classsb.CourseID);
        //                                            foreach (var course in courses.ToList())
        //                                            {
        //                                                hoclieuchuyende += $"<table style='width:100%'>" +
        //                                                   $"<tr>" +
        //                                                   $"<td style='width:50%;text-align:left'>{course.Name}  </td>" +
        //                                                   $"<td style='width:50%;text-align:left'>{lessonschedules.Count()} bài học đang diễn ra</td>" +
        //                                                   $"</tr>" +
        //                                                   $"</table>";
        //                                            }
        //                                        }
        //                                    }
        //                                }
        //                                index++;
        //                            }
        //                            tbody +=
        //                            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{hoclieuchinhquy}</td>" +
        //                            $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{hoclieuchuyende}</td></tr>";
        //                        }
        //                    }

        //                }
        //                tbody += @"<tr style='font-weight: 600;background-color: yellow'>
        //                    <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>
        //                    <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;text-align: left'>Tổng</td>" +
        //                    $"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>{totalStudent}</td>" +
        //                    @"<td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>
        //                    <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>
        //                    <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>
        //                    <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>
        //                    <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>
        //                    <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>
        //                    </tr>
        //                    <tr style='font-weight: 600;background-color: yellow'>
        //                    <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>
        //                    <td style='text-align:center; border: solid 1px #333; border-collapse: collapse;text-align: left'>Tỉ lệ %</td>
        //                    <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>
        //                    <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>
        //                    <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>
        //                    <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>
        //                    <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>
        //                    <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>
        //                    <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'></td>
        //                    </tr>
        //                    <tbody>
        //                    </table>";
        //                body += tbody;
        //                //var toAddress = isTest ? new List<string> { "nguyenvanhoa2017602593@gmail.com" } : teacherHeader.Select(t => t.Email).ToList();
        //                var toAddress = new List<string> { "nguyenvanhoa2017602593@gmail.com" };
        //                _ = await _mailHelper.SendBaseEmail(new List<string>(), subject, body, MailPhase.WEEKLY_SCHEDULE, null, toAddress);
        //            }
        //        }
        //        count++;
        //    }
        //}

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

}
