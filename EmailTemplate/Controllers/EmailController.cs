using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmailTemplate.Controllers
{
    public class EmailController : Controller
    {
        private readonly ILogger<EmailController> _logger;
        //private static ClassService _classService;
        //private static CenterService _centerService;
        //private static ClassSubjectService _classSubjectService;
        //private static LessonService _lessonService;
        //private static LessonScheduleService _scheduleService;
        //private static AccountService _accountService;
        //private static StudentService _studentService;
        //private static TeacherService _teacherService;
        //private static SkillService _skillService;
        private static MailHelper _mailHelper;
        public EmailController(ILogger<EmailController> logger, MailHelper mailHelper)
        {
            _logger = logger;
            _mailHelper = mailHelper;
        }
        public IActionResult Index()
        {
            return View();
        }

        public string DrawChart()
        {
            try
            {

            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return "";
        }
    }
}