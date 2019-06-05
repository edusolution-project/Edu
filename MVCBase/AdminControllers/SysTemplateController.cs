using BaseModels;
using CoreLogs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MVCBase.Globals;
using MVCBase.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MVCBase.AdminControllers
{
    [MenuControl(
        CModule = "SysTemplate",
        Name = "Sys : Quản lý Giao diện",
        Order = 40,
        IShow = true,
        Type = MenuType.Sys
    )]
    public class SysTemplateController : AdminController
    {
        protected const string regexElement = @"<\s*static-layout[^>]*>(.*?)\<\s*\/\s*static-layout>|<\s*dynamic-layout[^>]*>(.*?)\<\s*\/\s*dynamic-layout>";
        protected const string regexID = @"id\s*=\s*\""(.*?)""";
        protected const string regexCode = @"code\s*=\""(.*?)""";
        private readonly SysTemplateService _sysTemplate;
        private readonly SysTemplateDetailsService _sysTemplateDetails;
        private readonly ILogs _logs;
        public readonly IHostingEnvironment _environment;
        public SysTemplateController(ILogs logs, IHostingEnvironment environment)
        {
            _sysTemplate = new SysTemplateService();
            _sysTemplateDetails = new SysTemplateDetailsService();
            _logs = logs;
            _environment = environment;
        }
        public ActionResult Index(DefaultModel model)
        {
            System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(_environment.ContentRootPath + "\\Views\\Shared");
            System.IO.FileInfo[] filse = directoryInfo.GetFiles();
            ViewBag.File = filse;
            ViewBag.Data = null;
            ViewBag.Model = model;
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult Edit(int ID)
        {
            var item = _sysTemplate.GetItemByID(ID);
            if(item != null)
            {
                var listData = _sysTemplateDetails.CreateQuery().Find(o => o.TemplateID == item.ID);
                if(listData == null)
                {
                    MatchCollection match = Regex.Matches(item.Html, regexElement, RegexOptions.IgnorePatternWhitespace);
                    int count = match.Count;
                    for(int i = 0; i< count; i++)
                    {
                        var str = match[i].ToString().Trim();
                        var id = Regex.Match(str, regexID).Value.ToString().Replace(@"id=", "").Replace(@"""", "");
                        var code = Regex.Match(str, regexCode).Value.ToString().Replace(@"code=", "").Replace(@"""", "");
                        if (str.IndexOf("static-layout") > -1)
                        {

                        }
                        if (str.IndexOf("dynamic-layout") > -1)
                        {

                        }
                    }
                }
            }
            else
            {
                SetMessageError("Data not found");
            }
            
            return View();
        }
        [HttpPost]
        public ActionResult Delete()
        {
            try
            {
                SetMessageSuccess("Đã xóa thành công");
            }
            catch (Exception ex)
            {
                SetMessageError(ex.ToString());
            }
            return RedirectToAction("Index");
        }
    }
}
