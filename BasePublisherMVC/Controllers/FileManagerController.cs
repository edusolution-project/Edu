using Microsoft.AspNetCore.Mvc;
using BasePublisherMVC.Globals;

namespace BasePublisherMVC.AdminControllers
{
    [MenuControl(CModule = "FileManager", Icon = "document", Name = "Sys : Quản lý file")]
    public class FileManagerController : AdminController
    {
        public FileManagerController()
        {
            
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
    }
}
