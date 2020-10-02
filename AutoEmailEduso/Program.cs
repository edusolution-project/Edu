using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BaseCoreEmail;
using BaseCustomerEntity.Database;
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
        private static bool isTest = true;
        private static int count = 0;

        private static LessonScheduleService _lessonScheduleService;

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

            isTest = configuration["Test"] == "1";

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
            await SendWeeklyReport();
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
            //quet co so dang active
            //lay mail gv quan ly co so (tru mail huongho@utc.edu.vn dua vao bcc)
            //quet danh sach lop dang hoat dong trong co so, trong moi lop quet tung hoc lieu
            //thong ke tinh trang hoat dong cua tung lop: sl hoc sinh, so bai hoc trong tuan, so hoc sinh tham gia, diem hoc sinh....
            //test: eduso
            var currentTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddHours(1);
            var startWeek = currentTime.AddDays(DayOfWeek.Sunday - currentTime.DayOfWeek);
            var endWeek = startWeek.AddDays(7);

            string subject = $"Báo cáo tiến độ học tập từ ngày {startWeek} đến ngày {endWeek}";
            string body = $"Chào bạn," +
                $"<p>Báo cáo tiến độ học tập từ ngày {startWeek} đến ngày {endWeek}</p>" +
                @"<table style='margin-top:20px; width: 100%; border: solid 1px #333; border-collapse: collapse'>
                    <thead>
                        <tr style='font-weight:bold'>
                            <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse'>STT</td>
                            <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Lớp</td>
                            <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Sĩ số lớp</td>
                            <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Số học sinh chưa đăng nhập hệ thống</td>
                            <td colspan='3' style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Điểm học tập</td>
                            <td colspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Tiến độ học tập</td>
                            <td rowspan='2' style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Ghi chú</td>
                        </tr>
                        <tr>
                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Số học sinh đạt kết quả > 50%</td>
                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Số HS điểm 0%</td>
                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Số HS đạt điểm <= 50%</td>
                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Tài liệu chính quy</td>
                            <td style='text-align:center; border: solid 1px #333; border-collapse: collapse'>Tài liệu chuyên đề</td>
                        </tr>
                    </thead>
                    <tbody>";

            var tbody = "";
            List<TeacherEntity> teacherList = null;
            //var activeCenters = _centerService.CreateQuery().Find(x => x.ExpireDate <= currentTime);//xem lai luc luu la gio GTM hay gi
            var activeCenters = _centerService.GetActiveCenter(currentTime);//xem lai luc luu la gio GTM hay gi
            if (activeCenters != null && activeCenters.Count() > 0)
            {
                foreach (var center in activeCenters.ToList())
                {
                    var teacherHeader = _teacherService.CreateQuery().Find(x => x.Centers.Find(o => o.CenterID == center.ID).CenterID.Contains(center.ID) && x.IsActive == true);

                    var activeClasses = _classService.GetActiveClass(time: currentTime, Center: center.ID).ToList();
                    var index = 0;
                    var totalStudent = 0;
                    if (activeClasses != null && activeClasses.Count() > 0)
                    {
                        foreach (var _class in activeClasses)
                        {
                            var countStudent = _studentService.GetStudentsByClassId(_class.ID).Count();//si so lop
                            totalStudent += countStudent;
                            var classSubjects = _classSubjectService.GetByClassID(_class.ID); //so mon hoc trong lop
                            tbody += "<tr>" +
                                $"<td>{index}</td>" +
                                $"<td>{_class.Name}</td>" +
                                $"<td>{countStudent}</td>" +
                                $"<td>4</td>" +
                                $"<td>20</td>" +
                                $"<td>16</td>" +
                                $"<td><table>";
                            if (classSubjects != null && classSubjects.Count() > 0)

                                foreach (var classsb in classSubjects)
                                {
                                    if (classsb.TypeClass == CLASS_TYPE.STANDARD)
                                    {
                                        var lessonschedules = _lessonScheduleService.CreateQuery().Find(x => x.ClassSubjectID == classsb.ID && x.StartDate >= startWeek && x.EndDate <= endWeek);
                                        tbody +=
                                        $"<tr>" +
                                        $"<td>Môn {classsb.CourseName} có {lessonschedules.CountDocuments()} trong tuần</td>" +
                                        $"</tr>";
                                    }
                                    else
                                    {
                                        var lessonschedules = _lessonScheduleService.CreateQuery().Find(x => x.ClassSubjectID == classsb.ID && x.StartDate >= startWeek && x.EndDate <= endWeek);
                                        tbody +=
                                        $"<tr>" +
                                        $"<td>Môn {classsb.CourseName} có {lessonschedules.CountDocuments()} trong tuần</td>" +
                                        $"</tr>";
                                    }
                                }
                            tbody += $"</table>" +
                                    $"</td>" +
                                    $"<td></td>";
                            index++;
                        }

                    }
                    tbody += $"<tr>" +
                        $"<td></td>" +
                        $"<td>Cộng tổng</td>" +
                        $"<td>{totalStudent}</td>" +
                        $"<td></td>" +
                        $"<td></td>" +
                        $"<td></td>" +
                        $"<td></td>" +
                        $"<td></td>" +
                        $"<td></td>" +
                        $"<td></td>" +
                        $"</tr>" +
                        $"<tr>" +
                        $"<td></td>" +
                        $"<td>Tỉ lệ %</td>" +
                        $"<td></td>" +
                        $"<td></td>" +
                        $"<td></td>" +
                        $"<td></td>" +
                        $"<td></td>" +
                        $"<td></td>" +
                        $"<td></td>" +
                        $"<td></td>" +
                        $"</tr>" +
                        $"<tbody></table>"
                        ;
                }
                //isTest = true;
                var toAddress = isTest ? new List<string> { "nguyenvanhoa2017602593@gmail.com" } : teacherList.Select(t => t.Email).ToList();
                _ = await _mailHelper.SendBaseEmail(new List<string>(), subject, body, MailPhase.WEEKLY_SCHEDULE, null, toAddress);
                count++;
            }
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
