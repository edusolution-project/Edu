using BaseAccess.Attribule;
using BaseAccess.Interfaces;
using BaseAccess.Services;
using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Core_v2.Globals;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BaseCustomerMVC.Controllers.Admin
{
    [BaseAccess.Attribule.AccessCtrl("Phân quyền (root)", "admin", 1)]
    public class AuthorityController : AdminController
    {
        private readonly AuthorityService _authorityService;
        private readonly IAccess _access;
        public AuthorityController(AuthorityService authorityService)
        {
            _authorityService = authorityService;
            _access = new AccessService();
        }

        public ActionResult Index(DefaultModel model)
        {
            var assembly = GetAssembly();
            ViewBag.AdminCtrl = _access.GetAccessByAttribue<Globals.AdminController>(assembly, "admin");
            ViewBag.TeacherCtrl = _access.GetAccessByAttribue<Globals.TeacherController>(assembly, "teacher");
            ViewBag.StudentCtrl = _access.GetAccessByAttribue<Globals.StudentController>(assembly, "student");
            List<AuthorityEntity> data = _authorityService.GetAll()?.ToList();
            ViewBag.Data = data;
            return View();
        }
        [HttpPost]
        public JsonResult Save([FromBody] List<AuthorityEntity> authorities)
        {
            if(authorities != null)
            {
                int count = authorities.Count;
                for (int i = 0; i < count; i++)
                {
                    AuthorityEntity item = authorities[i];
                    switch (item.Type)
                    {
                        case AuthorityType.VIEW: _authorityService.CreateView(item.Area, item.Description, item.CtrlName, item.ActName); break;
                        case AuthorityType.UPDATE: _authorityService.CreateUpdate(item.Area, item.Description, item.CtrlName, item.ActName); break;
                        case AuthorityType.ADD: _authorityService.CreateAdd(item.Area, item.Description, item.CtrlName, item.ActName); break;
                        case AuthorityType.DELETE: _authorityService.CreateDelete(item.Area, item.Description, item.CtrlName, item.ActName); break;
                        case AuthorityType.IMPORT: _authorityService.CreateImport(item.Area, item.Description, item.CtrlName, item.ActName); break;
                        case AuthorityType.EXPORT: _authorityService.CreateExport(item.Area, item.Description, item.CtrlName, item.ActName); break;
                        case AuthorityType.OTHER: _authorityService.CreateOther(item.Area, item.Description, item.CtrlName, item.ActName); break;
                        case AuthorityType.APPROVED: _authorityService.CreateApproved(item.Area, item.Description, item.CtrlName, item.ActName); break;
                        default:break;
                    }
                }
            }
            return new JsonResult(authorities);
        }
        private Assembly GetAssembly()
        {
            return Assembly.GetExecutingAssembly();
        }
    }
}
