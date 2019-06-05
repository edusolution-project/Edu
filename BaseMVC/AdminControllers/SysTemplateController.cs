
using CoreLogs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using BaseMVC.Globals;
using BaseMVC.Models;
using System;
using System.Text.RegularExpressions;
using BaseMongoDB.Database;
using MongoDB.Driver;

namespace BaseMVC.AdminControllers
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
        private readonly SysTemplateDetailService _sysTemplateDetails;
        private readonly ILogs _logs;
        public readonly IHostingEnvironment _environment;
        public SysTemplateController(ILogs logs, IHostingEnvironment environment, SysTemplateService templateService
            , SysTemplateDetailService templateDetailService)
        {
            _sysTemplate = templateService;
            _sysTemplateDetails = templateDetailService;
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
        public ActionResult Edit(string ID)
        {
            var item = _sysTemplate.GetByID(ID);
            if(item != null)
            {
                var listData = _sysTemplateDetails.CreateQuery().Find(o => o.TemplateID == item.ID)?.SingleOrDefault();
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
