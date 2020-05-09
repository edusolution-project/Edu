using BaseCustomerEntity.Database;
using BaseCustomerEntity.Globals;
using BaseEasyRealTime.Entities;
using Core_v2.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Admin_Customer.Controllers
{
    public class ContactController : Controller
    {
        private readonly GroupService _groupService;
        private readonly ClassService _classService;
        private readonly TeacherService _teacherService;
        private readonly StudentService _studentService;
        private readonly ILog _log;
        public ContactController(GroupService groupService,
            TeacherService teacherService
            , StudentService studentService
            , ClassService classService
            , ILog log)
        {
            _groupService = groupService;
            _classService = classService;
            _teacherService = teacherService;
            _studentService = studentService;
            _log = log;
        }
        [HttpGet]
        public JsonResult Get()
        {
            try
            {
                return new JsonResult(new { code = 405, msg = "bạn không đủ quyền !!" });
            }
            catch(Exception ex)
            {
                return new JsonResult(new { code = 500, msg = ex.Message });
            }
        }

        private bool CreateGroup(string Name,string classID, HashSet<string> members, string master)
        {
            return false;
        }
    }
}