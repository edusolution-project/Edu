using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BaseCoreEmail;
using BaseCustomerEntity.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

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

            isTest = configuration["Test"] == "1";

            Console.WriteLine("Processing Schedule...");
            await SendIncomingLesson();
            Console.WriteLine(count + " mail Sent!");

            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";
                eventLog.WriteEntry(count + " mail Sent!", EventLogEntryType.Information, 101, 1);
            }
            //Console.ReadKey();
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
            string subject = "Thông báo lịch dạy lớp " + currentClass.Name + " - Môn: " + currentSkill +
                " (" + startTime.ToLocalTime().ToString("HH:mm") + "-" + endTime.ToLocalTime().ToString("HH:mm") + " ngày " + startTime.ToLocalTime().ToString("dd/MM/yyyy") + ")";
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

            string subject = "Thông báo lịch học lớp " + currentClass.Name + " - Môn: " + currentSkill + " - Giáo viên: " + teacher.FullName +
                " (" + startTime.ToLocalTime().ToString("HH:mm") + "-" + endTime.ToLocalTime().ToString("HH:mm") + " ngày " + startTime.ToLocalTime().ToString("dd/MM/yyyy") + ")";
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
        }
    }

}
