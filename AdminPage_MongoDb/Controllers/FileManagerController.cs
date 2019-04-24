using Microsoft.AspNetCore.Mvc;
using MVCBase.Globals;
using Controller = MVCBase.AdminController;

namespace AdminPage.Controllers
{
    [MenuControl(CModule = "FileManager", Icon = "files",Name = "Sys : Quản lý file")]
    public class FileManagerController : Controller
    {
        public FileManagerController()
        {
            
        }
        // GET: User
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