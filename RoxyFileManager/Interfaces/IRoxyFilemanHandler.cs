using FileManagerCore.Globals;
using GoogleLib.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace FileManagerCore.Interfaces
{
    public interface IRoxyFilemanHandler
    {
        IGoogleDriveApiService GoogleDriveApiService { get; }
        List<MediaResponseModel> UploadFileWithGoogleDrive(string center, string user, HttpContext context);
        MediaResponseModel UploadSingleFileWithGoogleDrive(string center, string user, IFormFile file);

        bool DeleteFileWithGoogleDrive(string fileId, string center, string user);

        List<DIRLIST> ListDirTree(string type);
        List<FILESLIST> ListFiles(string path, string type);
        object CopyDir(string path, string newPath);
        object CopyFile(string path, string newPath);
        object CreateDir(string path, string name);
        object DeleteDir(string path);
        object DeleteFile(string path);
        object Upload(string path, string method, string action, HttpContext httpContext);
        object UploadEasyImage(string folderName, HttpContext httpContext);
        object MoveDir(string path, string newPath);
        object MoveFile(string path, string newPath);
        object RenameDir(string path, string name);
        object RenameFile(string path, string name);
        bool ThumbnailCallback();
        byte[] ShowThumbnail(string path, int width, int height);
        byte[] DownloadFile(string path);
        string DownloadDir(string path);
        List<Dictionary<string, string>> UploadDynamic(string nameFolder, HttpContext httpContext);
        Dictionary<string, List<MediaResponseModel>> UploadNewFeed(string nameFolder, HttpContext httpContext);
        Dictionary<string, List<MediaResponseModel>> UploadAnswerBasis(string nameFolder, HttpContext httpContext);
    }
}
