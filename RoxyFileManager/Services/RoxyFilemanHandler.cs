using BaseCustomerEntity.Database;
using FileManagerCore.Globals;
using GoogleLib.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;

namespace FileManagerCore.Services
{
    public class RoxyFilemanHandler : Interfaces.IRoxyFilemanHandler
    {
        const string EDUSO_MANAGERFILE = "EDUSO_MANAGERFILE";
        private readonly FolderManagerService _folderManagerService;

        private readonly FileManagerService _fileManagerService;

        private readonly FolderCenterService _folderCenterService;
        private readonly IGoogleDriveApiService _googleDriveService;
        private readonly GConfig _gConfig;
        private readonly IHostingEnvironment _environment;
        private Dictionary<string, string> _lang { get; set; }
        public RoxyFilemanHandler(IHostingEnvironment environment, GConfig gConfig, 
            FolderManagerService folderManagerService, FileManagerService fileManagerService, FolderCenterService folderCenterService)
        {
            _gConfig = gConfig;
            _environment = environment;
            _folderManagerService = folderManagerService;
            _fileManagerService = fileManagerService;
            _folderCenterService = folderCenterService;
            _googleDriveService = Startup.GoogleDrive;
        }
        public List<Dictionary<string, string>> UploadDynamic(string nameFolder, HttpContext httpContext)
        {
            var listUrl = new List<Dictionary<string, string>>();
            string path = Path.Combine(GetFilesRoot(), $"{nameFolder}");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            try
            {
                for (int i = 0; i < httpContext.Request.Form.Files.Count; i++)
                {
                    Dictionary<string, string> item = new Dictionary<string, string>();
                    IFormFile file = httpContext.Request.Form.Files[i];
                    if (CanHandleFile(file.FileName))
                    {
                        FileInfo f = new FileInfo(file.FileName);
                        string filename = MakeUniqueFilename(path, f.Name);
                        string dest = Path.Combine(path, filename);
                        using (var stream = new FileStream(dest, FileMode.Create))
                        {
                            file.CopyTo(stream);
                            stream.Close();
                        }
                        if (GetFileType(new FileInfo(filename).Extension) == "image")
                        {
                            int w = 0;
                            int h = 0;
                            int.TryParse(_gConfig.MAX_IMAGE_WIDTH, out w);
                            int.TryParse(_gConfig.MAX_IMAGE_HEIGHT, out h);
                            ImageResize(dest, dest, w, h);
                        }
                        var imgUrl = dest.Replace(_environment.WebRootPath, "").Replace("\\", "/").ToLower();
                        item.Add(i.ToString(), imgUrl);

                    }
                    else
                    {
                        item.Add("error_" + i.ToString(), file.FileName);
                    }
                    listUrl.Add(item);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listUrl;
        }
        public Dictionary<string, List<MediaResponseModel>> UploadAnswerBasis(string nameFolder, HttpContext httpContext)
        {
            var listUrl = new Dictionary<string, List<MediaResponseModel>>();
            string path = Path.Combine(GetFilesRoot(), $"{nameFolder}");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            try
            {
                var success = new List<MediaResponseModel>();
                var error = new List<MediaResponseModel>();
                for (int i = 0; i < httpContext.Request.Form.Files.Count; i++)
                {
                    MediaResponseModel item = new MediaResponseModel();
                    IFormFile file = httpContext.Request.Form.Files[i];
                    if (CanHandleFile(file.FileName))
                    {
                        FileInfo f = new FileInfo(file.FileName);
                        string filename = MakeUniqueFilename(path, DateTime.Now.Ticks.ToString()+f.Extension);
                        string dest = Path.Combine(path, filename);
                        item.Extends = GetFileType(f.Extension);
                        using (var stream = new FileStream(dest, FileMode.Create))
                        {
                            file.CopyTo(stream);
                            stream.Close();
                        }
                        item.Type = Path.GetExtension(dest);
                        if (item.Extends == "image")
                        {
                            int.TryParse(_gConfig.MAX_IMAGE_WIDTH, out int w);
                            int.TryParse(_gConfig.MAX_IMAGE_HEIGHT, out int h);
                            ImageResize(dest, dest, w, h);
                        }
                        var imgUrl = dest.Replace(_environment.WebRootPath, "").Replace("\\", "/").ToLower();
                        item.Path = imgUrl;
                        success.Add(item);
                    }
                    else
                    {
                        item.Path = file.Name;
                        error.Add(item);
                    }
                }
                listUrl.Add("success", success);
                listUrl.Add("error", error);
            }
            catch (Exception ex)
            {
                return listUrl;
            }

            return listUrl;
        }
        public Dictionary<string, List<MediaResponseModel>> UploadNewFeed(string nameFolder, HttpContext httpContext)
        {
            var listUrl = new Dictionary<string, List<MediaResponseModel>>();
            string path = Path.Combine(GetFilesRoot(), $"NewFeed/{nameFolder}");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            try
            {
                var success = new List<MediaResponseModel>();
                var error = new List<MediaResponseModel>();
                for (int i = 0; i < httpContext.Request.Form.Files.Count; i++)
                {
                    MediaResponseModel item = new MediaResponseModel();
                    IFormFile file = httpContext.Request.Form.Files[i];
                    if (CanHandleFile(file.FileName))
                    {
                        FileInfo f = new FileInfo(file.FileName);
                        string filename = MakeUniqueFilename(path, f.Name);
                        string dest = Path.Combine(path, filename);
                        item.Extends = GetFileType(f.Extension);
                        using (var stream = new FileStream(dest, FileMode.Create))
                        {
                            file.CopyTo(stream);
                            stream.Close();
                        }
                        item.Type = Path.GetExtension(dest);
                        if (item.Extends == "image")
                        {
                            int.TryParse(_gConfig.MAX_IMAGE_WIDTH, out int w);
                            int.TryParse(_gConfig.MAX_IMAGE_HEIGHT, out int h);
                            ImageResize(dest, dest, w, h);
                        }
                        var imgUrl = dest.Replace(_environment.WebRootPath, "").Replace("\\", "/").ToLower();
                        item.Path = imgUrl;
                        success.Add(item);
                    }
                    else
                    {
                        item.Path = file.Name;
                        error.Add(item);
                    }
                }
                listUrl.Add("success", success);
                listUrl.Add("error", error);
            }
            catch (Exception ex)
            {
                return listUrl;
            }

            return listUrl;
        }
        public object UploadEasyImage(string nameFolder,HttpContext httpContext)
        {
            string path = Path.Combine(GetFilesRoot(),$"EasyImage/{nameFolder}");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            IDictionary<string, string> Res = new Dictionary<string, string>(); 
            try
            {
                for (int i = 0; i < httpContext.Request.Form.Files.Count; i++)
                {
                    IFormFile file = httpContext.Request.Form.Files[i];
                    if (CanHandleFile(file.FileName))
                    {
                        FileInfo f = new FileInfo(file.FileName);
                        string filename = MakeUniqueFilename(path, f.Name);
                        string dest = Path.Combine(path, filename);
                        using (var stream = new FileStream(dest, FileMode.Create))
                        {
                            file.CopyTo(stream);
                            stream.Close();
                        }
                        if (GetFileType(new FileInfo(filename).Extension) == "image")
                        {
                            int w = 0;
                            int h = 0;
                            int.TryParse(_gConfig.MAX_IMAGE_WIDTH, out w);
                            int.TryParse(_gConfig.MAX_IMAGE_HEIGHT, out h);
                            ImageResize(dest, dest, w, h);
                        }
                        var imgUrl = dest.Replace(_environment.WebRootPath, "").Replace("\\", "/").ToLower();
                        if (!Res.TryGetValue("default", out string str))
                        {
                            Res.Add("default", imgUrl);
                        }
                        else
                        {
                            Res.Add(i.ToString(), imgUrl);
                        }
                    }
                    else
                    {
                        if (!Res.TryGetValue("error", out string str))
                        {
                            Res.Add("error", LangRes("E_UploadNotAll"));
                        }
                        else
                        {
                            Res.Add("error_" + i.ToString(), LangRes("E_UploadNotAll"));
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                Res.Add("error", ex.Message);
            }
            return Res;
        }
        public string DownloadDir(string path)
        {
            path = FixPath(path);
            if (!Directory.Exists(path))
                throw new Exception(LangRes("E_CreateArchive"));
            string dirName = new FileInfo(path).Name;
            string pathTemp = Path.Combine(_environment.WebRootPath, "tmp");
            if (!Directory.Exists(pathTemp))
            {
                Directory.CreateDirectory(pathTemp);
            }
            string tmpZip = Path.Combine(pathTemp, $"{dirName}.zip");

            if (File.Exists(tmpZip)) File.Delete(tmpZip);
            ZipFile.CreateFromDirectory(path, tmpZip, CompressionLevel.Fastest, true);
            return tmpZip.Replace(_environment.WebRootPath,"");
        }
        public byte[] DownloadFile(string path)
        {
            CheckPath(path);
            path = FixPath(path);
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            Bitmap img = new Bitmap(Bitmap.FromStream(fs));
            fs.Close();
            fs.Dispose();
            byte[] imageBytes;

            using (MemoryStream ms2 = new MemoryStream())
            {
                img.Save(ms2, ImageFormat.Png);
                imageBytes = ms2.ToArray();
                img.Clone();
                img.Dispose();
            }
            return imageBytes;
        }
        public object MoveDir(string path, string newPath)
        {
            CheckPath(path);
            CheckPath(newPath);
            DirectoryInfo source = new DirectoryInfo(FixPath(path));
            DirectoryInfo dest = new DirectoryInfo(FixPath(Path.Combine(newPath, source.Name)));
            if (dest.FullName.IndexOf(source.FullName) == 0)
                throw new Exception(LangRes("E_CannotMoveDirToChild"));
            else if (!source.Exists)
                throw new Exception(LangRes("E_MoveDirInvalisPath"));
            else if (dest.Exists)
                throw new Exception(LangRes("E_DirAlreadyExists"));
            else
            {
                try
                {
                    source.MoveTo(dest.FullName);
                    return new { res = "ok", msg = "" };
                }
                catch (Exception)
                {
                    throw new Exception(LangRes("E_MoveDir") + " \"" + path + "\"");
                }
            }

        }
        protected bool CanHandleFile(string filename)
        {
            bool ret = false;
            FileInfo file = new FileInfo(filename);
            string ext = file.Extension.Replace(".", "").ToLower();
            string setting = _gConfig.FORBIDDEN_UPLOADS.Trim().ToLower();
            if (setting != "")
            {
                List<string> tmp = new List<string>();
                tmp.AddRange(Regex.Split(setting, "\\s+"));
                if (!tmp.Contains(ext))
                    ret = true;
            }
            setting = _gConfig.ALLOWED_UPLOADS.Trim().ToLower();
            if (setting != "")
            {
                List<string> tmp = new List<string>();
                tmp.AddRange(Regex.Split(setting, "\\s+"));
                if (!tmp.Contains(ext))
                    ret = false;
            }

            return ret;
        }
        public object MoveFile(string path, string newPath)
        {
            CheckPath(path);
            CheckPath(newPath);
            FileInfo source = new FileInfo(FixPath(path));
            FileInfo dest = new FileInfo(FixPath(newPath));

            if (!source.Exists)
                throw new Exception(LangRes("E_MoveFileInvalisPath"));
            else if (dest.Exists)
                throw new Exception(LangRes("E_MoveFileAlreadyExists"));
            else if (!CanHandleFile(dest.Name))
                throw new Exception(LangRes("E_FileExtensionForbidden"));
            else
            {
                try
                {
                    source.MoveTo(dest.FullName);
                    return new { res = "ok", msg = "" };
                }
                catch (Exception)
                {
                    throw new Exception(LangRes("E_MoveFile") + " \"" + path + "\"");
                }
            }
        }
        public object RenameDir(string path, string name)
        {
            CheckPath(path);
            DirectoryInfo source = new DirectoryInfo(FixPath(path));
            DirectoryInfo dest = new DirectoryInfo(Path.Combine(source.Parent.FullName, name));
            if (source.FullName == GetFilesRoot())
                throw new Exception(LangRes("E_CannotRenameRoot"));
            else if (!source.Exists)
                throw new Exception(LangRes("E_RenameDirInvalidPath"));
            else if (dest.Exists)
                throw new Exception(LangRes("E_DirAlreadyExists"));
            else
            {
                try
                {
                    source.MoveTo(dest.FullName);
                    return new { res = "ok", msg = "" };
                }
                catch (Exception)
                {
                    throw new Exception(LangRes("E_RenameDir") + " \"" + path + "\"");
                }
            }
        }
        public object RenameFile(string path, string name)
        {
            CheckPath(path);
            FileInfo source = new FileInfo(FixPath(path));
            FileInfo dest = new FileInfo(Path.Combine(source.Directory.FullName, name));
            if (!source.Exists)
                throw new Exception(LangRes("E_RenameFileInvalidPath"));
            else if (!CanHandleFile(name))
                throw new Exception(LangRes("E_FileExtensionForbidden"));
            else
            {
                try
                {
                    source.MoveTo(dest.FullName);
                    return new { res = "ok", msg = "" };
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + "; " + LangRes("E_RenameFile") + " \"" + path + "\"");
                }
            }
        }
        public bool ThumbnailCallback()
        {
            return false;
        }

        public byte[] ShowThumbnail(string path, int width, int height)
        {
            CheckPath(path);
            FileStream fs = new FileStream(FixPath(path), FileMode.Open, FileAccess.Read);
            Bitmap img = new Bitmap(Bitmap.FromStream(fs),new Size(width,height));
            fs.Close();
            fs.Dispose();
            byte[] imageBytes;

            using (MemoryStream ms2 = new MemoryStream())
            {
                img.Save(ms2,ImageFormat.Png);
                imageBytes = ms2.ToArray();
                img.Clone();
                img.Dispose();
            }
            return imageBytes;
        }
        private ImageFormat GetImageFormat(string filename)
        {
            ImageFormat ret = ImageFormat.Jpeg;
            switch (new FileInfo(filename).Extension.ToLower())
            {
                case ".png": ret = ImageFormat.Png; break;
                case ".gif": ret = ImageFormat.Gif; break;
            }
            return ret;
        }
        protected void ImageResize(string path, string dest, int width, int height)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            Image img = Image.FromStream(fs);
            fs.Close();
            fs.Dispose();
            float ratio = (float)img.Width / (float)img.Height;
            if ((img.Width <= width && img.Height <= height) || (width == 0 && height == 0))
                return;

            int newWidth = width;
            int newHeight = Convert.ToInt16(Math.Floor((float)newWidth / ratio));
            if ((height > 0 && newHeight > height) || (width == 0))
            {
                newHeight = height;
                newWidth = Convert.ToInt16(Math.Floor((float)newHeight * ratio));
            }
            Bitmap newImg = new Bitmap(newWidth, newHeight);
            Graphics g = Graphics.FromImage((Image)newImg);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(img, 0, 0, newWidth, newHeight);
            img.Dispose();
            g.Dispose();
            if (dest != "")
            {
                newImg.Save(dest, GetImageFormat(dest));
            }
            newImg.Dispose();
        }
        private bool IsAjaxUpload(string ajax)
        {
            return ajax == "ajax";

        }
        public object Upload(string path,string method,string action , HttpContext httpContext)
        {
            CheckPath(path);
            path = FixPath(path);
            object res = new { res = "ok", msg = "" };
            bool hasErrors = false;
            try
            {
                for (int i = 0; i < httpContext.Request.Form.Files.Count; i++)
                {
                    IFormFile file = httpContext.Request.Form.Files[i];
                    if (CanHandleFile(file.FileName))
                    {
                        FileInfo f = new FileInfo(file.FileName);
                        string filename = MakeUniqueFilename(path, f.Name);
                        string dest = Path.Combine(path, filename);
                        using(var stream = new FileStream(dest, FileMode.Create))
                        {
                            file.CopyTo(stream);
                            stream.Close();
                        }
                        if (GetFileType(new FileInfo(filename).Extension) == "image")
                        {
                            int w = 0;
                            int h = 0;
                            int.TryParse(_gConfig.MAX_IMAGE_WIDTH, out w);
                            int.TryParse(_gConfig.MAX_IMAGE_HEIGHT, out h);
                            ImageResize(dest, dest, w, h);
                        }
                    }
                    else
                    {
                        hasErrors = true;
                        res = new { res = "ok", msg = LangRes("E_UploadNotAll") };
                    }
                }
            }
            catch (Exception ex)
            {
                res = new { res = "error", msg = ex.Message };
            }
            if (IsAjaxUpload(method))
            {
                if (hasErrors) res = new { res = "error", msg = LangRes("E_UploadNotAll") };
            }
            else
            {
                httpContext.Response.WriteAsync("<script>");
                httpContext.Response.WriteAsync("parent.fileUploaded(" + res + ");");
                httpContext.Response.WriteAsync("</script>");
            }

            return res;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }



        public object CreateDir(string path, string name)
        {
            CheckPath(path);
            path = FixPath(path);
            if (!Directory.Exists(path))
                throw new Exception(LangRes("E_CreateDirInvalidPath"));
            else
            {
                try
                {
                    path = Path.Combine(path, name);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    return new { res = "ok", msg = "" };
                }
                catch (Exception)
                {
                    throw new Exception(LangRes("E_CreateDirFailed"));
                }
            }
        }
        public object DeleteDir(string path)
        {
            CheckPath(path);
            path = FixPath(path);
            if (!Directory.Exists(path))
                throw new Exception(LangRes("E_DeleteDirInvalidPath"));
            else if (path == GetFilesRoot())
                throw new Exception(LangRes("E_CannotDeleteRoot"));
            else if (Directory.GetDirectories(path).Length > 0 || Directory.GetFiles(path).Length > 0)
                throw new Exception(LangRes("E_DeleteNonEmpty"));
            else
            {
                try
                {
                    Directory.Delete(path);
                    return new { res = "ok", msg = "" };
                }
                catch (Exception)
                {
                    throw new Exception(LangRes("E_CannotDeleteDir"));
                }
            }
        }
        public object DeleteFile(string path)
        {
            CheckPath(path);
            path = FixPath(path);
            if (!File.Exists(path))
                throw new Exception(LangRes("E_DeleteFileInvalidPath"));
            else
            {
                try
                {
                    File.Delete(path);
                    return new { res = "ok", msg = "" };
                }
                catch (Exception)
                {
                    throw new Exception(LangRes("E_DeletеFile"));
                }
            }
        }

        protected string MakeUniqueFilename(string dir, string filename)
        {
            string ret = filename;
            int i = 0;
            while (File.Exists(Path.Combine(dir, ret)))
            {
                i++;
                ret = Path.GetFileNameWithoutExtension(filename) + "- Copy " + i.ToString() + Path.GetExtension(filename);
            }
            return ret;
        }
        public object CopyFile(string path, string newPath)
        {
            CheckPath(path);
            FileInfo file = new FileInfo(FixPath(path));
            newPath = FixPath(newPath);
            if (!file.Exists) throw new Exception(LangRes("E_CopyFileInvalisPath"));
            else
            {
                string newName = MakeUniqueFilename(newPath, file.Name);
                try
                {
                    File.Copy(file.FullName, Path.Combine(newPath, newName));
                    return new { res = "ok", msg = "" };
                }
                catch (Exception)
                {
                    throw new Exception(LangRes("E_CopyFile"));
                }
            }
        }
        
        public object CopyDir(string path, string newPath)
        {
            CheckPath(path);
            CheckPath(newPath);
            DirectoryInfo dir = new DirectoryInfo(FixPath(path));
            DirectoryInfo newDir = new DirectoryInfo(FixPath(newPath + "/" + dir.Name));

            if (!dir.Exists)
            {
                throw new Exception(LangRes("E_CopyDirInvalidPath"));
            }
            else if (newDir.Exists)
            {
                throw new Exception(LangRes("E_DirAlreadyExists"));
            }
            else
            {
                _copyDir(dir.FullName, newDir.FullName);
            }
            return new { res = "ok", msg = "" };
        }
        
        public List<FILESLIST> ListFiles(string path, string type)
        {
            List<FILESLIST> response = new List<FILESLIST>();
            try
            {
                string fullPath = FixPath(path);
                List<string> files = GetFiles(fullPath, type);
                for (int i = 0; i < files.Count; i++)
                {
                    FileInfo f = new FileInfo(files[i]);
                    int w = 0, h = 0;
                    if (GetFileType(f.Extension) == "image")
                    {
                        try
                        {
                            FileStream fs = new FileStream(f.FullName, FileMode.Open, FileAccess.Read);
                            Image img = Image.FromStream(fs);
                            w = img.Width;
                            h = img.Height;
                            fs.Close();
                            fs.Dispose();
                            img.Dispose();
                        }
                        catch (Exception ex) { throw ex; }
                    }
                    var item = new FILESLIST()
                    {
                        p = $"{path}/{f.Name}",
                        t = Math.Ceiling(LinuxTimestamp(f.LastWriteTime)).ToString(),
                        s = f.Length.ToString(),
                        w = w.ToString(),
                        h = h.ToString()
                    };
                    response.Add(item);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return response;
        }
        protected double LinuxTimestamp(DateTime d)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime();
            TimeSpan timeSpan = (d.ToLocalTime() - epoch);
            return timeSpan.TotalSeconds;

        }
        public List<DIRLIST> ListDirTree(string type)
        {
            List<DIRLIST> response = new List<DIRLIST>();
            try
            {
                DirectoryInfo d = new DirectoryInfo(GetFilesRoot());
                if (!d.Exists)
                {
                    Directory.CreateDirectory(GetFilesRoot());
                }
                var dirs = ListDirs(d.FullName);
                dirs.Insert(0, d.FullName);
                for (int i = 0; i < dirs.Count; i++)
                {
                    string dir = (string)dirs[i];
                    var item = new DIRLIST()
                    {
                        p= dir.Replace(_environment.WebRootPath, "").Replace("\\", "/"),
                        f = GetFiles(dir, type).Count.ToString(),
                        d = Directory.GetDirectories(dir).Length.ToString()
                    };
                    response.Add(item);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return response;
        }
        private void _copyDir(string path, string dest)
        {
            if (!Directory.Exists(dest))
                Directory.CreateDirectory(dest);
            foreach (string f in Directory.GetFiles(path))
            {
                FileInfo file = new FileInfo(f);
                if (!File.Exists(Path.Combine(dest, file.Name)))
                {
                    File.Copy(f, Path.Combine(dest, file.Name));
                }
            }
            foreach (string d in Directory.GetDirectories(path))
            {
                DirectoryInfo dir = new DirectoryInfo(d);
                _copyDir(d, Path.Combine(dest, dir.Name));
            }
        }
        private string FixPath(string path)
        {
            path = path.ToLower();
            var strCheck = GetFilesRoot().Replace(_environment.WebRootPath, "").Replace("\\", "/").ToLower();
            if (path.StartsWith($"{strCheck}/"))
            {
                path = path.Replace($"{strCheck}/", "");
            }
            else
            {
                if (path.StartsWith($"{strCheck}"))
                {
                    path = path.Replace($"{strCheck}", "");
                }
            }
            return Path.Combine(GetFilesRoot(),path);
        }
        protected bool CheckPath(string path)
        {
            return FixPath(path).IndexOf(GetFilesRoot()) != 0;
        }

        protected string GetFileType(string ext)
        {
            string ret = "file";
            ext = ext.ToLower();
            if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif" || ext== ".ico")
                ret = "image";
            else if (ext == ".swf" || ext == ".flv")
                ret = "flash";
            else if (ext == ".mp4" || ext == ".avi" || ext == ".wmv" || ext == ".mov")
                ret = "video";
            else if (ext == ".mp3" || ext == ".mpeg" || ext == ".wma")
                ret = "audio";
            else if (ext == ".docx" || ext == ".doc" || ext == ".txt" || ext == ".ppt" || ext == ".pptx" || ext == ".pdf")
                ret = "doc";
            return ret;
        }
        private List<string> GetFiles(string path, string type)
        {
            List<string> ret = new List<string>();
            if (type == "#")
                type = "";
            string[] files = Directory.GetFiles(path);
            foreach (string f in files)
            {
                if ((GetFileType(new FileInfo(f).Extension) == type) || (type == "") || type == null)
                    ret.Add(f);
            }
            return ret;
        }
        private List<string> ListDirs(string path)
        {
            string[] dirs = Directory.GetDirectories(path);
            List<string> ret = new List<string>();
            foreach (string dir in dirs)
            {
                ret.Add(dir);
                ret.AddRange(ListDirs(dir));
            }
            return ret;
        }
        private string GetFilesRoot()
        {
            if (string.IsNullOrEmpty(_gConfig.FILES_ROOT))
            {
                return Path.Combine(_environment.WebRootPath, GKeyConfiguration._strUpLoad);
            }
            else
            {
                return Path.Combine(_environment.WebRootPath, _gConfig.FILES_ROOT);
            }
        }
        private string GetLangFile()
        {
            string filename = $"{_environment.WebRootPath}\\lang\\{_gConfig.LANG}.json";
            if (!File.Exists(filename)) filename = $"{_environment.WebRootPath}\\lang\\en.json";
            return filename;
        }
        protected string LangRes(string name)
        {
            string ret = name;
            if (_lang == null)
                _lang = ParseJSON(GetLangFile());
            if (_lang.ContainsKey(name))
                ret = _lang[name];

            return ret;
        }
        protected Dictionary<string, string> ParseJSON(string file)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            string json = "";
            try
            {
                json = File.ReadAllText(file, Encoding.UTF8);
            }
            catch (Exception) { }

            json = json.Trim();
            if (json != "")
            {
                if (json.StartsWith("{"))
                    json = json.Substring(1, json.Length - 2);
                json = json.Trim();
                json = json.Substring(1, json.Length - 2);
                string[] lines = Regex.Split(json, "\"\\s*,\\s*\"");
                foreach (string line in lines)
                {
                    string[] tmp = Regex.Split(line, "\"\\s*:\\s*\"");
                    try
                    {
                        if (tmp[0] != "" && !ret.ContainsKey(tmp[0]))
                        {
                            ret.Add(tmp[0], tmp[1]);
                        }
                    }
                    catch (Exception) { }
                }
            }
            return ret;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="user"></param>
        /// <param name="context"></param>
        /// <returns>List fileId</returns>
        public List<MediaResponseModel> UploadFileWithGoogleDrive(string center,string user,HttpContext context)
        {
            string folderId = GetFolder(center, user);
            var listFile = context.Request.Form.Files;
            var count = listFile == null ? 0 : listFile.Count;
            string path = Path.Combine(GetFilesRoot(), $"{center}/{user}");

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            List<MediaResponseModel> response = new List<MediaResponseModel>();
            for (int i = 0; i < count; i++)
            {
                IFormFile file = listFile[i];
                FileInfo f = new FileInfo(file.FileName);
                string filename = MakeUniqueFilename(path, f.Name);
                string dest = Path.Combine(path, filename);
                string fileId = "";
                using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                {
                    file.CopyTo(stream);
                    fileId = _googleDriveService.UploadFileStatic(filename, _googleDriveService.GetMimeType(dest), stream, folderId);
                    stream.Close();
                }
                response.Add(new MediaResponseModel() { Path = fileId,Extends = f.Extension });

                _fileManagerService.Collection.InsertOne(new FileManagerEntity()
                {
                    Extends = f.Extension,
                    FileID = fileId,
                    FolderID = folderId,
                    Name = file.Name,
                    Center = center,
                    UserID = user
                });
            }
            return response;
        }
        public bool DeleteFileWithGoogleDrive(string fileId, string center, string user)
        {
            try
            {
                if (_fileManagerService.RemoveFile(center, user, fileId))
                {
                    _googleDriveService.Delete(fileId);
                    return true;
                }
                return false;
            }
            catch(Exception)
            {
                return false;
            }
        }

        private string GetFolder(string center, string user)
        {
            string centerFolder = _folderCenterService.GetFolderID(center);
            if (string.IsNullOrEmpty(centerFolder))
            {
                centerFolder = CreateFolderCenter(center);
            }
            string folderId = _folderManagerService.GetFolderID(centerFolder, user);
            if (string.IsNullOrEmpty(folderId))
            {
                folderId = CreateFolderUser(centerFolder, user);
            }

            return folderId;
        }
        private string GetRoot()
        {
            string root = _folderCenterService.GetRoot();
            if (string.IsNullOrEmpty(root))
            {
                root = _googleDriveService.CreateDirectory(EDUSO_MANAGERFILE, "Quản lý file của hệ thống").Id;
                _folderCenterService.CreateRoot(root);
            }
            return root;
        }

        private string CreateFolderUser(string centerFolder,string user)
        {
            string folderId = _googleDriveService.CreateDirectory(user, "User Folder create" + DateTime.Now.ToString("yyyy-mm-dd"),centerFolder).Id;
            _folderManagerService.CreateQuery().InsertOne(new FolderManagerEntity()
            {
                Center = centerFolder,
                FolderID = folderId,
                UserID = user
            });

            return folderId;
        }

        private string CreateFolderCenter(string center)
        {
            string root = GetRoot();
            string folderId = _googleDriveService.CreateDirectory(center,"CenterFolder " + DateTime.Now.ToString("yyyy-mm-dd"), root).Id;
            _folderCenterService.CreateQuery().InsertOne(new FolderCenterEntity()
            {
                Center = center,
                FolderID = folderId
            });
            return folderId;
        }

        
    }
}
