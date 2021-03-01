﻿using BaseCustomerEntity.Database;
using FileManagerCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmailTemplate.Services
{
    public class ReportService
    {
        private static ClassService _classService;
        private static CenterService _centerService;
        private static ClassSubjectService _classSubjectService;
        private static LessonService _lessonService;
        //private static LessonScheduleService _scheduleService;
        private static AccountService _accountService;
        private static StudentService _studentService;
        private static TeacherService _teacherService;
        private static SkillService _skillService;
        private static IRoxyFilemanHandler _roxyFilemanHandler;
        public ReportService(
            ClassService classService, 
            CenterService centerService,
            ClassSubjectService classSubjectService,
            LessonService lessonService,
            //LessonScheduleService lessonScheduleService,
            AccountService accountService,
            StudentService studentService,
            SkillService skillService,
            IRoxyFilemanHandler roxyFilemanHandler
            )
        {
            _classService = classService;
            _centerService = centerService;
            _classSubjectService = classSubjectService;
            _lessonService = lessonService;
            //_scheduleService = lessonScheduleService;
            _accountService = accountService;
            _studentService = studentService;
            _skillService = skillService;
            _roxyFilemanHandler = roxyFilemanHandler;
        }

        public async Task<string> GetWeek()
        {
            return await
                Task.FromResult<string>("");
        }
    }
}
