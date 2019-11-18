using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Claims;
using FileManagerCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FileManagerCore.Controllers
{
    public class RoxyFileController : Controller
    {
        private readonly IRoxyFilemanHandler _roxy;
        public RoxyFileController(IRoxyFilemanHandler roxy)
        {
            _roxy = roxy;
        }
        public string Token()
        {
            return "token";
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ListDirTree(string type)
        {
            return new JsonResult(_roxy.ListDirTree(type));
        }
        [HttpPost]
        public JsonResult ListFiles(string d, string type)
        {
            return new JsonResult(_roxy.ListFiles(d,type));
        }
        [HttpPost]
        public JsonResult CopyDir(string d, string n)
        {
            return new JsonResult(_roxy.CopyDir(d, n));
        }
        [HttpPost]
        public JsonResult CopyFile(string f, string n)
        {
            return new JsonResult(_roxy.CopyFile(f, n));
        }
        [HttpPost]
        public JsonResult CreateDir(string d, string n)
        {
            return new JsonResult(_roxy.CreateDir(d, n));
        }
        [HttpPost]
        public JsonResult DeleteDir(string d)
        {
            return new JsonResult(_roxy.DeleteDir(d));
        }
        [HttpPost]
        public JsonResult DeleteFile(string f)
        {
            return new JsonResult(_roxy.DeleteFile(f));
        }
        [HttpPost]
        public JsonResult MoveDir(string d, string n)
        {
            return new JsonResult(_roxy.MoveDir(d, n));
        }
        [HttpPost]
        public JsonResult MoveFile(string f, string n)
        {
            return new JsonResult(_roxy.MoveFile(f, n));
        }

        [HttpPost]
        public JsonResult RenameDir(string d, string n)
        {
            return new JsonResult(_roxy.RenameDir(d, n));
        }
        [HttpPost]
        public JsonResult RenameFile(string f, string n)
        {
            return new JsonResult(_roxy.RenameFile(f, n));
        }
        [HttpPost]
        public JsonResult UploadEasyImage()
        {
            string folderName = "anonymous";
            if (User.Identity.IsAuthenticated)
            {
                if(User.HasClaim(o=>o.Type == "Name"))
                {
                    folderName = User.Claims.FirstOrDefault(o => o.Type == "Name")?.Value;
                }
                if (string.IsNullOrEmpty(folderName))
                {
                    folderName = "anonymous";
                }
            }
            return new JsonResult(_roxy.UploadEasyImage(folderName,HttpContext));
        }
        [HttpPost]
        public JsonResult Upload(string d,string method,string action)
        {
            return new JsonResult(_roxy.Upload(d,method,action,HttpContext));
        }
        [HttpGet]
        public IActionResult ShowThumbnail(string f,int width, int height)
        {        
            return File(_roxy.ShowThumbnail(f,width,height), "image/png");

        }
        [HttpGet]
        public ActionResult DownloadFile(string f)
        {
            string nameFile = "";
            if (f.Contains("/"))
            {
                var arr = f.Split('/');
                nameFile = arr[arr.Length - 1];
            }
            else
            {
                if (f.Contains("\\"))
                {
                    var arr = f.Split('\\');
                    nameFile = arr[arr.Length - 1];
                }
            }
            var imageBytes = _roxy.DownloadFile(f);
            Response.Headers.Add("Content-Disposition", "attachment; filename=\"" + nameFile + "\"");
            return File(imageBytes, "application/octet-stream");
        }

        public ActionResult DownloadDir(string d)
        {
            var path = _roxy.DownloadDir(d);
            string nameFile = "";
            if (path.Contains("/"))
            {
                var arr = path.Split('/');
                nameFile = arr[arr.Length - 1];
            }
            else
            {
                if (path.Contains("\\"))
                {
                    var arr = path.Split('\\');
                    nameFile = arr[arr.Length - 1];
                }
            }
            Response.Headers.Add("Content-Disposition", "attachment; filename=\"" + nameFile + "\"");
            return File(path, "application/octet-stream");
        }

    }

}
