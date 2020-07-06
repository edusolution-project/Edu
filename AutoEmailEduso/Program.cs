using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
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
        private static LessonScheduleService _scheduleService;
        private static AccountService _accountService;
        private static StudentService _studentService;

        static void Main(string[] args)
        {


            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            _centerService = new CenterService(configuration);
            _classService = new ClassService(configuration);
            _classSubjectService = new ClassSubjectService(configuration);
            _scheduleService = new LessonScheduleService(configuration);
            _accountService = new AccountService(configuration);

            var centerCount = _centerService.GetAll().CountDocuments();
            Console.WriteLine(centerCount);

            Console.ReadKey();
        }

        protected void SendIncomingLesson()
        {
            var currentTime = DateTime.Now;
            var activeClasses = _classService.GetActiveClass(time: currentTime, Center: null);
            if (activeClasses != null && activeClasses.Count() > 0)
            {
                foreach (var @class in activeClasses)
                {
                    var activeSchedules = _scheduleService.GetIncomingSchedules(time: currentTime, period: 60, ClassID: @class.ID).OrderByDescending(t => t.ClassSubjectID);
                    if (activeSchedules != null && activeSchedules.Count() > 0)
                    {
                        string subjectID = "";
                        ClassSubjectEntity currentSubject = null;
                        TeacherEntity currentTeacher = null;
                        var studentList = new List<string>();
                        var schedules = new List<ScheduleView>();
                        foreach (var schedule in activeSchedules)
                        {
                            if (subjectID != schedule.ClassSubjectID)//change subject
                            {
                                if (!string.IsNullOrEmpty(subjectID))
                                {
                                    //Send Mail for current Pending
                                }
                                subjectID = schedule.ClassSubjectID;
                                var newsubject = _classSubjectService.GetItemByID(schedule.ClassSubjectID);
                                schedules = new List<ScheduleView>();
                                studentList = new List<string>();
                            }
                            else
                            {

                            }
                        }
                    }
                }
            }
        }
    }

    public class ScheduleView : LessonScheduleEntity
    {
        public string LessonName { get; set; }
    }

}
