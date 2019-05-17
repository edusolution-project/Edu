using CoreLogs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using BasePublisherMVC.Globals;
using BasePublisherMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BasePublisherModels.Database;
using CoreMongoDB.Repositories;
using MongoDB.Driver;

namespace BasePublisherMVC.AdminControllers
{
    public class CPApiController : ControllerBase
    {
        protected const string regexElement = @"<\s*static-layout[^>]*>(.*?)\<\s*\/\s*static-layout>|<\s*dynamic-layout[^>]*>(.*?)\<\s*\/\s*dynamic-layout>";
        protected const string regexID = @"id\s*=\s*\""(.*?)""";
        protected const string regexName = @"name\s*=\s*\""(.*?)""";
        protected const string regexCode = @"code\s*=\""(.*?)""";
        private readonly ILogs _logs;
        private readonly WebMenu _menu;
        private readonly CPMenuService _menuService;
        private readonly CPUserService _userService;
        private readonly CPRoleService _roleService;
        private readonly CPLangEntity _currentLang;
        private readonly CPLangService _langService;
        private readonly Security _security;
        private readonly IConfiguration _configuration;
        public CPApiController(ILogs logs
            , IConfiguration configuration
            , CPMenuService menuService
            , CPUserService userService
            , CPRoleService roleService
            , CPLangService langService
            , Security security)
        {
            _configuration = configuration;
            _logs = logs;
            _menu = new WebMenu();
            _security = security;
            _menuService = menuService;
            _userService = userService;
            _roleService = roleService;
            _langService = langService;

            _currentLang = StartUp.CurrentLang;
        }
        public Response GetLayoutList()
        {
            var data = _configuration.GetSection("Layout").Value;
            if (string.IsNullOrEmpty(data))
            {
                return new Response(404, "data not Found", null);
            }
            else
            {
                return new Response(200, "success", data.Split(',').ToList());
            }
        }
        public Response GetPartialViewList(string name)
        {
            var data = _configuration.GetSection("PartialView")[name];
            if (string.IsNullOrEmpty(data))
            {
                return new Response(404, "data not Found", null);
            }
            else
            {
                return new Response(200, "success", data.Split(',').ToList());
            }
        }
        public Response GetMenuForWeb(string type)
        {
            try
            {
                var data = _menuService.GetItemByType(type, _currentLang.ID);
                if (data == null) return new Response()
                {
                    Code = 404,
                    Data = null,
                    Message = "No Data Found"
                };
                return new Response()
                {
                    Code = 200,
                    Data = data,
                    Message = ""
                };
            }
            catch (Exception ex)
            {
                _logs.WriteLogsError("GETMenuForWeb", ex);
                return new Response()
                {
                    Code = 500,
                    Data = null,
                    Message = ex.Message
                };
            }
        }
        public Response GetCModuleAll()
        {
            try
            {
                var data = _menu.GetControl.Select(o=>new { o.Name,o.Code,o.FullName });
                if (data == null) return new Response() { Code = 404, Data = null, Message = "Data Not Found" };
                return new Response() { Code = 200, Data = data, Message="Success" };
            }
            catch (Exception ex)
            {
                _logs.WriteLogsError("GetTemplateInfo", ex);
                return new Response()
                {
                    Code = 500,
                    Data = null,
                    Message = ex.Message
                };
            }

        }
        public CModule GetCModule(string name)
        {
           return  _menu.GetControl.SingleOrDefault(o => o.FullName == name);
        }
        public ActionResult<object> GetString()
        {
            List<string> listStr = new List<string>()
            {
                "GÀ","CHÓ","LỢN","TRÂU"
            };
            Response response = new Response
            {
                Code = HttpContext.Response.StatusCode,
                Message = "Success",
                Data = listStr
            };
            return response;
        }
        #region Create Begin
        public Response StartPage()
        {
            var listRole = new List<CPRoleEntity>()
            {
                new CPRoleEntity()
                {
                    Name = "Administrator",
                    Code = "administrator",
                    Lock = true
                },
                new CPRoleEntity()
                {
                    Name = "Member",
                    Code = "member",
                    Lock = true
                }
            };
            _roleService.AddRange(listRole);
            List<CPUserEntity> listUser = new List<CPUserEntity>();
            for(int i = 0; i < listRole.Count; i++)
            {
                var user = new CPUserEntity()
                {
                    IsActive = true,
                    BirthDay = DateTime.Now,
                    Created = DateTime.Now,
                    Email = listRole[i].Code+"@gmail.com",
                    Name = listRole[i].Name,
                    RoleID = listRole[i].ID,
                    Phone = "0976497928",
                    Skype = "breakingdawn1235",
                    Pass = Security.Encrypt("123")
                };
                _userService.Add(user);
                listUser.Add(user);
            }
            List<CPLangEntity> listLang = new List<CPLangEntity>()
            {
                new CPLangEntity()
                {
                    IsActive =true,
                    Code = "VN",
                    Name = "Việt Nam"
                },
                new CPLangEntity()
                {
                    IsActive =true,
                    Code = "EN",
                    Name = "English"
                }
            };
            _langService.AddRange(listLang);
            IDictionary<string, dynamic> data = new Dictionary<string, dynamic>();
            data.Add("Role", listRole);
            data.Add("User", listUser);
            data.Add("Lang", listLang);
            return new Response(201, "Create Success", data);
        }
        #endregion
    }
    public class Response
    {
        public Response()
        {
        }

        public Response(long code, string message, object data)
        {
            Code = code;
            Message = message;
            Data = data;
        }

        public long Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
    public class TemplateApiModel
    {
        public string ID { get; set; }
        public string ParrentLayout { get; set; }
        public string PartialView { get; set; }
        public string LayoutName { get; set; }
        public string CModule { get; set; }
        public string Record { get; set; }
        public bool IsDynamic { get; set; }
        public int Order { get; set; }
        public bool IsBody { get; set; }
        public string PartialID { get; set; }
        public string Name { get; set; }
        public string Command { get; set; }
        public string TypeView { get; set; }
    }
  
}
