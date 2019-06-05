using CoreLogs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using BaseMVC.Globals;
using BaseMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BaseMongoDB.Database;
using CoreMongoDB.Repositories;
using MongoDB.Driver;

namespace BaseMVC.AdminControllers
{
    public class CPApiController : ControllerBase
    {
        protected const string regexElement = @"<\s*static-layout[^>]*>(.*?)\<\s*\/\s*static-layout>|<\s*dynamic-layout[^>]*>(.*?)\<\s*\/\s*dynamic-layout>";
        protected const string regexID = @"id\s*=\s*\""(.*?)""";
        protected const string regexName = @"name\s*=\s*\""(.*?)""";
        protected const string regexCode = @"code\s*=\""(.*?)""";
        private readonly ILogs _logs;
        private readonly WebMenu _menu;
        private readonly SysTemplateDetailService _templateDetailsService;
        private readonly SysTemplatePropertyService _propertyService;
        private readonly SysTemplateService _templateService;
        private readonly CPMenuService _menuService;
        private readonly CPUserService _userService;
        private readonly CPRoleService _roleService;
        private readonly CPLangEntity _currentLang;
        private readonly CPLangService _langService;
        private readonly Security _security;
        private readonly IConfiguration _configuration;
        public CPApiController(ILogs logs
            , IConfiguration configuration
            , SysTemplateDetailService templateDetailService
            , SysTemplateService templateService
            , SysTemplatePropertyService templatePropertyService
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
            _templateDetailsService = templateDetailService;
            _templateService = templateService;
            _menuService = menuService;
            _propertyService = templatePropertyService;
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
        [HttpGet]
        public Response GetTemplateInfo(string ID)
        {
            if (string.IsNullOrEmpty(ID)) return new Response() { Code = 404, Data = null, Message = "Big Zero" };
            try
            {
                Dictionary<string, object> response = new Dictionary<string, object>(); 
                List<LayoutModel> StaticLayout = new List<LayoutModel>();
                List<LayoutModel> DynamicLayout = new List<LayoutModel>();
                List<LayoutModel> DynamicItems = new List<LayoutModel>();
                var data = _templateService.GetByID(ID);
                if (data == null) return new Response() { Code=404,Data=null,Message="Data Not Found" };
                // group Layout Static và Layout Dynamic
                MatchCollection match = Regex.Matches(data.Html, regexElement, RegexOptions.IgnorePatternWhitespace);
                int count = match.Count;
                for (int i = 0; i < count; i++)
                {
                    LayoutModel layout = new LayoutModel();
                    var str = match[i].ToString().Trim();
                    // id vswLogo
                    var id = Regex.Match(str, regexID).Value.ToString().Replace(@"id=", "").Replace(@"""", "");
                    // code = CAdv
                    var code = Regex.Match(str, regexCode).Value.ToString().Replace(@"code=", "").Replace(@"""", "");
                    // Logo
                    var name = Regex.Match(str, regexName).Value.ToString().Replace(@"name=", "").Replace(@"""", "");
                    layout.PartialID = id;
                    layout.LayoutName = name;
                    layout.TypeView = code;
                    if (str.IndexOf("static-layout") > -1)
                    {
                        var staticLayout = ProcessStaticLayout(ID, layout);
                        StaticLayout.Add(staticLayout);
                    }
                    if (str.IndexOf("dynamic-layout") > -1)
                    {
                        layout.IsDynamic = true;
                        var detals = _templateDetailsService.GetListItemDynamicByID(ID, id);
                        int countDetals = detals != null ? detals.Count : 0;
                        if(countDetals > 0)
                        {
                            for(int x = 0; x < countDetals; x++)
                            {
                                var dItem = detals[x];
                                var dLI = ProccessDynamicLayout(dItem);
                                DynamicItems.Add(dLI);
                            }
                        }
                        DynamicLayout.Add(layout);
                    }
                }

                response.Add("dynamic", DynamicLayout);
                response.Add("static", StaticLayout);
                response.Add("dynamicitem", DynamicItems.OrderBy(o=>o.Order));
                return new Response()
                {
                    Code = 200,
                    Data = response,
                    Message = "Success"
                };
            }
            catch (Exception ex)
            {
                _logs.WriteLogsError("GetTemplateInfo","{ID:"+ID+"}", ex);
                return new Response()
                {
                    Code = 500,
                    Data = null,
                    Message = ex.Message
                };
            }
            
        }
        #region Proccess Layout
        private LayoutModel ProccessDynamicLayout(SysTemplateDetailEntity dItem)
        {
            LayoutModel dLI = new LayoutModel
            {
                LayoutName = dItem.LayoutName,
                CModule = dItem.CModule,
                PartialID = dItem.PartialID,
                ParrentLayout = dItem.ParrentID,
                IsDynamic = true,
                IsBody = dItem.IsBody,
                PartialView = dItem.PartialView,
                TypeView = dItem.TypeView,
                Order = dItem.Order
            };
            if (!dItem.IsBody)
            {
                var itemCtrl = _menu.GetControl.SingleOrDefault(o => o.FullName.ToLower() == dItem.CModule.ToLower());
                if (itemCtrl == null)
                {
                    itemCtrl = new CModule()
                    {
                        Code = "Static",
                        FullName = "",
                        Name = "No Exist",
                        Properties = null
                    };
                }
                var properties = itemCtrl.Properties;
                var property = _propertyService.Find(true, o => o.TemplateDetailID == dItem.ID)?.ToList();
                if (property != null && properties != null)
                {
                    List<ProperyCModule> properyCs = new List<ProperyCModule>();
                    string _continue = "";
                    for (int z = 0; z < properties.Count; z++)
                    {
                        var itemPro = properties[z];
                        ProperyCModule properyC = new ProperyCModule()
                        {
                            Key = itemPro.Key,
                            Name = itemPro.Name,
                            Type = itemPro.Type,
                            Value = itemPro.Value
                        };
                        foreach (var itemzPRo in property)
                        {
                            if (_continue != "" && _continue == itemzPRo.Name) continue;
                            if (itemPro.Key == itemzPRo.Name)
                            {
                                properyC.Value = itemzPRo.Value;
                                _continue = itemzPRo.Name;
                                break;
                            }
                        }
                        properyCs.Add(properyC);
                    }
                    dLI.Properties = properyCs;
                }
            }
            return dLI;
        }
        private LayoutModel ProcessStaticLayout(string ID,LayoutModel layout)
        {
            var itemCtrl = _menu.GetControl.SingleOrDefault(o => o.Code.ToLower() == layout.TypeView.ToLower());
            if (itemCtrl == null)
            {
                itemCtrl = new CModule()
                {
                    Code = "Static",
                    FullName = "",
                    Name = "No Exist",
                    Properties = null
                };
            }
            var properties = itemCtrl.Properties;
            layout.LayoutName += "(" + itemCtrl.Name + ")";
            layout.CModule = itemCtrl.FullName;
            var detailsTemplate = _templateDetailsService.GetItemStaticByID(ID, layout.PartialID);
            List<ProperyCModule> properyCs = new List<ProperyCModule>();
            if (detailsTemplate != null)
            {
                layout.TypeView = detailsTemplate.TypeView;
                layout.PartialView = detailsTemplate.PartialView;
                var property = _propertyService.Find(true, o => o.TemplateDetailID == detailsTemplate.ID)?.ToList();

                if (property != null && properties != null)
                {
                    string _continue = "";
                    for (int x = 0; x < properties.Count; x++)
                    {
                        var itemPro = properties[x];
                        ProperyCModule properyC = new ProperyCModule()
                        {
                            Key = itemPro.Key,
                            Name = itemPro.Name,
                            Type = itemPro.Type,
                            Value = itemPro.Value
                        };
                        foreach (var itemzPRo in property)
                        {
                            if (_continue != "" && _continue == itemzPRo.Name) continue;
                            if (itemPro.Key == itemzPRo.Name)
                            {
                                properyC.Value = itemzPRo.Value;
                                _continue = itemzPRo.Name;
                                break;
                            }
                        }
                        properyCs.Add(properyC);
                    }
                    layout.Properties = properyCs;
                }
                else
                {
                    layout.Properties = properties;
                }
            }
            else
            {
                layout.Properties = properties;
            }
            return layout;
        }
        #endregion
        [HttpPost]
        public Response UpdateTemplate(TemplateApiModel model)
        {
            try
            {
                // kiểm tra thông tin truyền vào.
                var form = HttpContext.Request.Form;
                switch (model.Command)
                {
                    case "UpdateInfoItemDynamic":
                        UpdateInfoItemDynamic(model, form);
                        break;
                    case "UpdateInfoStatic":
                        #region UpdateInfoStatic
                        UpdateInfoStatic(model, form);
                        #endregion
                        break;
                    case "AddNewItemDynamic":
                        AddNewItemDynamic(model);
                        break;
                    default: break;
                }
                _logs.WriteLogsInfo(Newtonsoft.Json.JsonConvert.SerializeObject(model)+ "\r\n" +
                    Newtonsoft.Json.JsonConvert.SerializeObject(form)+"\r\n");
                return new Response(200, "Success", model);
            }
            catch (Exception ex)
            {
                _logs.WriteLogsError("UpdateTemplate", Newtonsoft.Json.JsonConvert.SerializeObject(model), ex);
                return new Response(500, ex.Message, null);
            }
        }
        #region Template PRivate
        private void AddNewItemDynamic(TemplateApiModel model)
        {
            if(model.CModule.ToLower() == "renderbody")
            {
               model.IsBody = true;
            }
            var item = new SysTemplateDetailEntity()
            {
                CModule = model.CModule,
                IsBody = model.IsBody,
                LayoutName = model.LayoutName +"("+model.Name+")",
                TypeView = model.TypeView,
                TemplateID = model.Record,
                IsDynamic = true,
                ParrentID = model.ParrentLayout,
                PartialID = Guid.NewGuid().ToString()
            };
            _templateDetailsService.Add(item);
        }
        private void UpdateInfoStatic(TemplateApiModel model, IFormCollection form)
        {
            var item = _templateDetailsService.CreateQuery().Find(o => o.IsDynamic == false 
                        && o.IsBody == false
                        && o.PartialID == model.PartialID
                        && o.TemplateID == model.Record)?.SingleOrDefault();
            if(item == null)
            {
                item = new SysTemplateDetailEntity()
                {
                    IsBody = false,
                    IsDynamic = false,
                    CModule = model.CModule,
                    PartialID = model.PartialID,
                    LayoutName = model.LayoutName,
                    TemplateID = model.Record,
                    TypeView = model.TypeView,
                    ParrentID = string.Empty,
                    PartialView = model.PartialView
                };
                _templateDetailsService.Add(item);
            }
            string detailsTemplateID = item.ID;
            foreach (var key in form.Keys)
            {
                if (model.GetType().GetMember(key).Count() > 0)
                {
                    continue;
                }
                else
                {
                    var itemProperties = _propertyService.CreateQuery().Find(o => o.TemplateDetailID == detailsTemplateID && o.Name == key && o.PartialID == model.PartialID)?.SingleOrDefault();
                    if(itemProperties == null)
                    {
                        itemProperties = new SysTemplatePropertyEntity()
                        {
                            PartialID = item.PartialID,
                            Name = key,
                            TemplateDetailID = detailsTemplateID
                        };
                    }
                    var data = form[key];
                    if (!string.IsNullOrEmpty(data))
                    {
                        itemProperties.Value = data;
                        _propertyService.Add(itemProperties);
                    }
                }
            }
        }
        private void UpdateInfoItemDynamic(TemplateApiModel model, IFormCollection form)
        {
            var item = _templateDetailsService.CreateQuery().Find(o => o.IsDynamic == true
                        && o.IsBody == false
                        && o.ParrentID == model.ParrentLayout
                        && o.PartialID == model.PartialID
                        && o.TemplateID == model.Record)?.SingleOrDefault();
            if (item == null)
            {
                return;
            }
            else
            {
                if (!string.IsNullOrEmpty(model.PartialView))
                {
                    var newITem = item;
                    newITem.PartialView = model.PartialView;
                    _templateDetailsService.Add(newITem);
                }
            }
            string detailsTemplateID = item.ID;
            foreach (var key in form.Keys)
            {
                if (model.GetType().GetMember(key).Count() > 0)
                {
                    continue;
                }
                else
                {
                    var itemProperties = _propertyService.CreateQuery().Find(o => o.TemplateDetailID == detailsTemplateID && 
                    o.Name == key && o.PartialID == model.PartialID)?.SingleOrDefault();
                    if (itemProperties == null)
                    {
                        itemProperties = new SysTemplatePropertyEntity()
                        {
                            PartialID = item.PartialID,
                            Name = key,
                            TemplateDetailID = detailsTemplateID
                        };
                    }
                    var data = form[key];
                    if (!string.IsNullOrEmpty(data))
                    {
                        itemProperties.Value = data;
                        _propertyService.Add(itemProperties);
                    }
                }
            }
        }
        #endregion
        /// <summary>
        /// Update order của thằng layout 
        /// </summary>
        /// <param name="data"> list data  </param>
        /// <returns></returns>
        [HttpPost]
        public Response UpdateOrderDynamicView([FromBody] TemplateApiModel[] data)
        {
            try
            {
                int count = data != null ? data.Count() : 0;
                if (count == 0) return new Response(200, "Data Not Found", null);
                for (int i = 0; i < count; i++)
                {
                    var item = data[i];
                    var itemDetails = _templateDetailsService.GetItemDynamicByID(item.Record, item.PartialID, item.ParrentLayout);
                    itemDetails.Order = item.Order;
                    _templateDetailsService.Add(itemDetails);
                }
                return new Response(200, "Success", count);
            }
            catch(Exception ex)
            {
                _logs.WriteLogsError("UpdateOrderDynamicView", ex);
                return new Response(500, ex.Message, null);
            }
        }
        [HttpPost]
        public Response DeleteDynamicView([FromBody] TemplateApiModel model)
        {
            try
            {
                var item = _templateDetailsService.CreateQuery().Find(o => o.ParrentID == model.ParrentLayout 
                && o.PartialID == model.PartialID && o.TemplateID == model.Record)?.SingleOrDefault();
                if(item != null)
                {
                    var listPro = _propertyService.GetItemByParentID(item.ID);
                    if(listPro != null)
                    {
                        _propertyService.RemoveRange(listPro.Select(o=>o.ID).ToList());
                    }
                    _templateDetailsService.Remove(item.ID);
                }
                _logs.WriteLogsInfo(Newtonsoft.Json.JsonConvert.SerializeObject(model));
                return new Response(200, "Delete Success", null);
            }
            catch (Exception ex)
            {
                _logs.WriteLogsError("DeleteDynamicView", ex);
                return new Response(500, ex.Message, null);
            }
        }
        [HttpGet]
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
                    Activity = true,
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
                    Activity =true,
                    Code = "VN",
                    Name = "Việt Nam"
                },
                new CPLangEntity()
                {
                    Activity =true,
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
