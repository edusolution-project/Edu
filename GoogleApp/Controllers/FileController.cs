using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BaseCustomerEntity.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace GoogleApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly string EDUSO_MANAGERFILE = "EDUSO_MANAGERFILE";
        private readonly IConfiguration _configuration;
        private readonly FolderManagerService _folderManagerService;
        private readonly FileManagerService _fileManagerService;
        private readonly FolderCenterService _folderCenterService;

        public FileController(IConfiguration configuration, FolderManagerService folderManagerService, FileManagerService fileManagerService, FolderCenterService folderCenterService)
        {
            _configuration = configuration;
            _folderManagerService = folderManagerService;
            _fileManagerService = fileManagerService;
            _folderCenterService = folderCenterService;
            EDUSO_MANAGERFILE = _configuration.GetSection("GOOGLE:DRIVE:ROOT").Value;
        }
        [HttpPost]
        public List<MediaResponseModel> Upload(string center, string user, IFormFileCollection formFiles) 
        {
            try
            {
                string folderId = GetFolder(center, user);
                var listFile = formFiles;
                var count = listFile == null ? 0 : listFile.Count;
                string path = Path.Combine(GetRoot(), $"{center}/{user}");
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
                        fileId = Program.GoogleDriveApiService.UploadFileStatic(filename, Program.GoogleDriveApiService.GetMimeType(dest), stream, folderId);
                        stream.Close();
                    }
                    response.Add(new MediaResponseModel() { Path = fileId, Extends = f.Extension });

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
            catch (Exception)
            {
                return null;
            }
        }
        [HttpPost]
        public bool Delete(string fileId, string center, string user)
        {
            try
            {
                if (_fileManagerService.RemoveFile(center, user, fileId))
                {
                    Program.GoogleDriveApiService.Delete(fileId);
                    return true;
                }
                return false;
            }
            catch (Exception)
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
                root = Program.GoogleDriveApiService.CreateDirectory(EDUSO_MANAGERFILE, "Quản lý file của hệ thống").Id;
                _folderCenterService.CreateRoot(root);
            }
            return root;
        }

        private string CreateFolderUser(string centerFolder, string user)
        {
            string folderId = Program.GoogleDriveApiService.CreateDirectory(user, "User Folder create" + DateTime.Now.ToString("yyyy-mm-dd"), centerFolder).Id;
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
            string folderId = Program.GoogleDriveApiService.CreateDirectory(center, "CenterFolder " + DateTime.Now.ToString("yyyy-mm-dd"), root).Id;
            _folderCenterService.CreateQuery().InsertOne(new FolderCenterEntity()
            {
                Center = center,
                FolderID = folderId
            });
            return folderId;
        }
        protected string MakeUniqueFilename(string dir, string filename)
        {
            string ret = filename;
            int i = 0;
            while (System.IO.File.Exists(Path.Combine(dir, ret)))
            {
                i++;
                ret = Path.GetFileNameWithoutExtension(filename) + "- Copy " + i.ToString() + Path.GetExtension(filename);
            }
            return ret;
        }
    }
    public class MediaResponseModel
    {
        public string Path { get; set; }
        public string Extends { get; set; }
        public string Type { get; set; }
    }
}
