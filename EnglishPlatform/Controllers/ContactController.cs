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
                if(User != null && User.Identity.IsAuthenticated)
                {
                    var type = User.FindFirst("Type")?.Value;
                    var user = User.FindFirst(ClaimTypes.Email)?.Value;
                    var curentID = "";
                    var currentUser = _studentService.Collection.Find(o => o.Email == user)?.SingleOrDefault();
                    if(currentUser ==null)
                    {
                        var currentTeacher = _teacherService.Collection.Find(o => o.Email == user)?.SingleOrDefault();
                        if(currentTeacher == null)
                        {

                        }
                        else
                        {
                            curentID = currentTeacher.ID;
                        }
                    }
                    else
                    {
                        curentID = currentUser.ID;
                    }
                    var listItem = _classService.Collection.Find(o => o.IsActive == true && (o.Students.Contains(curentID) || o.TeacherID == curentID) && o.EndDate >= DateTime.Now && o.StartDate <= DateTime.Now).ToList();
                    HashSet<object> listTeacher = new HashSet<object>();
                    for (var i = 0; listItem != null && i < listItem.Count; i++)
                    {
                        var item = listItem[i];
                        var teacher = _teacherService.GetItemByID(item.TeacherID);
                        if (teacher == null) teacher = new TeacherEntity() { Email = "longthaihoang94@gmail.com", FullName = "Hoàng Thái Long" };
                        listTeacher.Add(new { name=teacher.FullName, email = teacher.Email });
                        var Name = item.Name;
                        var members = new HashSet<string>() { teacher.Email };
                        var students = _studentService.Collection.Find(o => item.Students.Contains(o.ID))?.ToList();
                        if(students != null)
                        {
                            members = students.Select(o => o.Email).ToHashSet();
                            members.Add(teacher.Email);
                        }
                        var isGroup = CreateGroup(Name, item.ID, members, teacher.Email);
                        if (isGroup)
                        {
                            item.IsGroup = true;
                            _classService.CreateOrUpdate(item);
                        }
                    }
                    var listFriend = _groupService.Collection.Find(o => o.Members.Contains(user) && o.IsPrivateChat == true).ToList();
                    var listGroup = _groupService.Collection.Find(o => o.Members.Contains(user) && o.IsPrivateChat == false).ToList();
                    if(listGroup == null) return new JsonResult(new { code = 404, msg = "không tìm thấy !!" });
                    var emails = listGroup.Select(o => o.MasterGroup?.First());
                    return new JsonResult(new { code = 200, msg = "success !!", data = new {
                        group = listGroup,
                        teacher = _teacherService.Collection.Find(_=> emails.Contains(_.Email))?.ToList(),
                        friend = listFriend
                    } });
                }
                else
                {
                    return new JsonResult(new { code = 405, msg = "bạn không đủ quyền !!" });
                }
            }
            catch(Exception ex)
            {
                return new JsonResult(new { code = 500, msg = ex.Message });
            }
        }

        private bool CreateGroup(string Name,string classID, HashSet<string> members, string master)
        {
            var item = _groupService.Collection.Find(o => o.Name == classID && o.Members.Count == members.Count && o.IsPrivateChat == false && o.MasterGroup.Contains(master))?.ToList();
            if(item == null || item.Count == 0)
            {
                _groupService.Create(Name, classID, master,members,new HashSet<string> { master });
                return true;
            }
            return false;
        }
    }
}