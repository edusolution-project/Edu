using BaseModels;
using CoreLogs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVCBase.Globals;
using MVCBase.Models;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MVCBase.AdminControllers
{
    [MenuControl(
        CModule = "CPTemplate",
        Name ="Sys : Quản lý Layout",
        Order = 40,
        IShow = true
    )]
    public class CPTemplateController : AdminController
    {
        private readonly SysTemplateService _service;
        private readonly ILogs _logs;
        public readonly IHostingEnvironment _environment;
        public CPTemplateController(ILogs logs,IHostingEnvironment environment)
        {
            _service = new SysTemplateService();
            _logs = logs;
            _environment = environment;
        }
        public ActionResult Index(DefaultModel model)
        {
            System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(_environment.ContentRootPath + "\\Views\\Shared");
            ViewBag.Data = null;
            ViewBag.Model = model;
            return View();
        }
        public IActionResult Create(DefaultModel model)
        {
           
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DefaultModel model, CPResourceEntity item)
        {
            
            return View();
        }
        public IActionResult Edit(int ID)
        {
            
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(DefaultModel model, CPResourceEntity item)
        {
            return RedirectToAction("index");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(DefaultModel model)
        {
            int delete = 0;
            if (string.IsNullOrEmpty(model.ArrID))
            {
                SetMessageError("Dữ liệu trống");
                return RedirectToAction("index");

            }
            else
            {
                var arr = model.ArrID.Split(',').Select(int.Parse).ToList();
                int arrCount = arr != null ? arr.Count : 0;
                for (int i = 0; i < arrCount; i++)
                {
                    int id = arr[i];
                    var item = _service.GetItemByID(id);
                    if(item != null)
                    {
                        
                        _service.CreateQuery().Remove(item);
                        await _service.CreateQuery().CompleteAsync();
                        delete++;
                    }
                   
                }
                if(delete > 0)
                {
                    SetMessageSuccess("Đã xóa "+delete+" đối tượng");
                    return RedirectToAction("Index");
                }
                else
                {
                    SetMessageWarning("Không có đổi tượng nào bị xóa");
                    return RedirectToAction("Index");
                }
            }
            
        }
    }
}
