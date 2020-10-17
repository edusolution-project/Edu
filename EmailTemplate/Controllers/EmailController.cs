﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
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
            if(currentTime == null || currentTime <= DateTime.MinValue)
            {
                currentTime = DateTime.Now;
            }
            var startWeek = currentTime.AddDays(-7);
            var endWeek = startWeek.AddDays(6).AddHours(23).AddMinutes(59).AddMilliseconds(59);
            var headRole = _roleService.GetItemByCode("head-teacher");
            var centersActive = _centerService.GetActiveCenter(currentTime);//lay co so dang hoat dong

            ViewBag.Centers = centersActive;

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

        public async Task SendWeeklyReport(CenterEntity center)
        {

        }

        public async Task SendWeeklyReport(bool isTest)
        {
            //var currentTime = DateTime.Now;
            var currentTime = new DateTime(2020, 10, 5);
            //var startWeek = currentTime.AddDays(DayOfWeek.Sunday - currentTime.DayOfWeek).AddDays(1);
            //var endWeek = startWeek.AddDays(7);
            var startWeek = currentTime.AddDays(-7);
            var endWeek = startWeek.AddDays(6).AddHours(23).AddMinutes(59).AddMilliseconds(59);

            var headRole = _roleService.GetItemByCode("head-teacher");

            var centersActive = _centerService.GetActiveCenter(currentTime);//lay co so dang hoat dong
            foreach (var center in centersActive)
            {
                //if (center.Abbr == "c3vyvp")//test truong Vinh Yen
                //{
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
                    var activeSchedules = _lessonScheduleService.CreateQuery().Find(o => o.ClassID == _class.ID && o.StartDate <= endWeek && o.EndDate >= startWeek).Project(t => new LessonScheduleEntity
                    {
                        LessonID = t.LessonID,
                        ClassSubjectID = t.ClassSubjectID,
                        StartDate = t.StartDate,
                        EndDate = t.EndDate
                    }).ToList();


                    var activeLessonIds = activeSchedules.Select(t => t.LessonID).ToList();

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
                                        }).ToList();


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
        }

        public async Task SendIncomingLesson(bool isTest)
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
                                    //Send Mail for lastest class subject
                                    await SendStudentSchedule(schedules, currentTeacher, studentList, currentClass, skill.Name, subjectID, currentTime, currentTime.AddMinutes(period), center, isTest);
                                    await SendTeacherSchedule(schedules, currentTeacher, currentClass, skill.Name, subjectID, currentTime, currentTime.AddMinutes(period), center, isTest);
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
                            //Send Mail for lastest class subject
                            await SendStudentSchedule(schedules, currentTeacher, studentList, currentClass, skill.Name, subjectID, currentTime, currentTime.AddMinutes(period), center, isTest);
                            await SendTeacherSchedule(schedules, currentTeacher, currentClass, skill.Name, subjectID, currentTime, currentTime.AddMinutes(period), center, isTest);
                        }
                    }
                }
            }
        }

        private async Task SendTeacherSchedule(List<ScheduleView> schedules, TeacherEntity currentTeacher, ClassEntity currentClass, string currentSkill, string subjectID, DateTime startTime, DateTime endTime, CenterEntity center, bool isTest)
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

        private async Task SendStudentSchedule(List<ScheduleView> schedules, TeacherEntity teacher, List<StudentEntity> studentList, ClassEntity currentClass, string currentSkill, string subjectID, DateTime startTime, DateTime endTime, CenterEntity center,bool isTest)
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
            await _mailHelper.SendBaseEmail(new List<string>(), subject, body, MailPhase.WEEKLY_SCHEDULE, null, toAddress);
        }

        //function
        private bool HasRole(string userid, string center, string role)
        {
            var teacher = _teacherService.GetItemByID(userid);
            if (teacher == null) return false;
            var centerMember = teacher.Centers.Find(t => t.CenterID == center);
            if (centerMember == null) return false;
            if (_roleService.GetItemByID(centerMember.RoleID).Code != role) return false;
            return true;
        }
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
}