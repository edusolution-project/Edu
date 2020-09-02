﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BaseCustomerEntity.Database;
using BaseCustomerMVC.Controllers.Student;
using BaseCustomerMVC.Globals;
using BaseEasyRealTime.Entities;
using Core_v2.Interfaces;
using Core_v2.Repositories;
using FileManagerCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

namespace EnglishPlatform.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EasyChatController : ControllerBase
    {
        protected readonly Dictionary<string, List<string>> _mapUserOffline = new Dictionary<string, List<string>>();
        protected readonly Dictionary<string, string> _mapConnectId = new Dictionary<string,string>();
        protected readonly Dictionary<string, List<string>> _mapUsersConnectionId = new Dictionary<string, List<string>>();
        private readonly IRoxyFilemanHandler _roxyFilemanHandler;
        private readonly IHubContext<ChatHub> _hubContent;
        private readonly StudentService _studentService;
        private readonly ClassService _classService;
        private readonly ILog _log;
        public EasyChatController(IHubContext<ChatHub> hubContent,IRoxyFilemanHandler roxyFilemanHandler, StudentService studentService, ClassService classService, ILog log)
        {
            
            _hubContent = hubContent;
            _roxyFilemanHandler = roxyFilemanHandler;
            _studentService = studentService;
            _classService = classService;
            _log = log;
        }

        // danh sách group 
        [HttpGet]
        public List<MemberInfo> GetClassList()
        {
            try
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    List<string> classIdList = _studentService.GetItemByID(User.FindFirst("UserID").Value)?.JoinedClasses;
                    if(classIdList != null && classIdList.Count >0)
                    {
                        var listClass = _classService.GetItemsByIDs(classIdList)?.Select(o => new MemberInfo()
                        {
                            ID = o.ID,
                            Name = o.Name,
                            Center = o.Center
                        })?.ToList();

                        return listClass;
                    }
                }

            }
            catch(Exception ex)
            {
                _log.Error(MethodBase.GetCurrentMethod().Name, ex);
            }
            return null;
        }
        [HttpGet]
        public List<MemberInfo> GetMembersInClass (string className)
        {
            try
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    if (!string.IsNullOrEmpty(className))
                    {
                        List<StudentEntity> listStudentOnClass = _studentService.CreateQuery().Find(o => o.JoinedClasses.Contains(className))?.ToList();
                        if (listStudentOnClass != null && listStudentOnClass.Count > 0)
                        {
                            var listClass = listStudentOnClass?.Select(o => new MemberInfo()
                            {
                                ID = o.ID,
                                Name = o.FullName,
                                Center = className
                            })?.ToList();

                            return listClass;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _log.Error(MethodBase.GetCurrentMethod().Name, ex);
            }
            return null;
        }

        [HttpPost]
        public List<MemberInfo> GetMembers(List<string> classNames)
        {
            try
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    List<StudentEntity> listStudentOnClass = new List<StudentEntity>();
                    if (classNames != null && classNames.Count > 0)
                    {
                        for(int  i = 0; i < classNames.Count; i++)
                        {
                            string className = classNames[i];
                            var listStudent = _studentService.CreateQuery().Find(o => o.JoinedClasses.Contains(className))?.ToList();
                            if (listStudent != null && listStudent.Count > 0)
                            {
                                listStudentOnClass.AddRange(listStudent);
                            }
                        }
                        
                        if (listStudentOnClass != null && listStudentOnClass.Count > 0)
                        {
                            HashSet<StudentEntity> students = new HashSet<StudentEntity>(listStudentOnClass);
                            var listClass = students?.Select(o => new MemberInfo()
                            {
                                ID = o.ID,
                                Name = o.FullName
                            })?.ToList();

                            return listClass;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _log.Error(MethodBase.GetCurrentMethod().Name, ex);
            }
            return null;
        }

        [HttpPost]
        public bool EditMessage(string id,string message)
        {
            try
            {
                var files = HttpContext.Request.Form.Files;
                if (!string.IsNullOrEmpty(message))
                {

                }
                if (files != null && files.Count > 0)
                {

                }
            }
            catch (Exception ex)
            {
                _log.Error(MethodBase.GetCurrentMethod().Name, ex);
            }
            return false;
        }

        public bool GetMessages(string groupName, string user)
        {
            try
            {
                if (!string.IsNullOrEmpty(groupName))
                {

                }
                if (!string.IsNullOrEmpty(user))
                {

                }
            }
            catch (Exception ex)
            {
                _log.Error(MethodBase.GetCurrentMethod().Name, ex);
            }
            return false;
        }

        [HttpPost]
        public bool RemoveMessage(string id)
        {
            try
            {
                var files = HttpContext.Request.Form.Files;
                if (!string.IsNullOrEmpty(id))
                {

                }
                if (files != null && files.Count > 0)
                {

                }
            }
            catch (Exception ex)
            {
                _log.Error(MethodBase.GetCurrentMethod().Name, ex);
            }
            return false;
        }

        [HttpPost]
        public bool CreateMessage(string message,string groupName, string userName)
        {
            try
            {
                var files = HttpContext.Request.Form.Files;
                if (!string.IsNullOrEmpty(message))
                {

                }
                if (files != null && files.Count > 0)
                {

                }
            }
            catch(Exception ex)
            {
                _log.Error(MethodBase.GetCurrentMethod().Name, ex);
            }
            return false;
        }

        // add key connectid và key userId
        private bool AddMapUserWithConnectId(string userId,string connectId)
        {
            lock(_mapUsersConnectionId)
            {
                if (!_mapUsersConnectionId.TryGetValue(userId, out List<string> connectionIds))
                {
                    connectionIds = new List<string>() { };
                    _mapUsersConnectionId.Add(userId, connectionIds);
                }
                lock(connectionIds)
                {
                    connectionIds.Add(connectId);
                }
            }
            return false;
        }
        private bool AddMapConnectIdWithUser(string userId,string connectId)
        {
            lock (_mapConnectId)
            {
                if (!_mapConnectId.TryGetValue(connectId, out string connection))
                {
                    return _mapConnectId.TryAdd(connectId, userId);
                }
            }
            return false;
        }

        private void RemoveMapConnectionId(string connectionId)
        {
            lock(_mapConnectId)
            {
                if(_mapConnectId.TryGetValue(connectionId,out string userId))
                {
                    _mapConnectId.Remove(connectionId);
                }
            }
        }
        private void RemoveMapConnectionIdWithUser(string userId, string connectionId)
        {
            lock (_mapUsersConnectionId)
            {
                if (_mapUsersConnectionId.TryGetValue(userId, out List<string> connections))
                {
                    if(connections == null)
                    {
                        _mapUsersConnectionId.Remove(userId);
                    }
                    else
                    {
                        if (connections.Count > 0)
                        {
                            lock(connections)
                            {
                                connections.Remove(connectionId);
                            }
                            if (connections == null)
                            {
                                _mapUsersConnectionId.Remove(userId);
                            }
                        }
                    }
                }
            }
        }

        private void FillUserToGroup(List<string> groupsName, string connectId)
        {

        }
    }

    public class MemberInfo
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Center { get; set; }
    }
}