using Google.Apis.Drive.v3.Data;
using System.IO;
using System.Threading.Tasks;

namespace GoogleLib.Interfaces
{
    public interface IGoogleDriveApiService
    {
        string URL_VIEW_FILE { get; }
        string URL_PREVIEW_FILE { get; }
        string URL_THUMBNAIL {get;}
        Google.Apis.Drive.v3.Data.File CreateDirectory(string title, string des, string parent = "");
        string CreateFolder(string folderName, string parentID = "");
        Task<string> Delete(string fileId);
        byte[] DownloadFile(string fileID);
        object GetFolderInfo(string id);
        string UploadFile(string fileUpload, string parents = "");
        object UploadFile(string fileName, MemoryStream memoryStream, string mimeType, string parents = "");
        string UploadFileStatic(string fileUpload, string parents = "");
        Task<byte[]> ViewFile(string fileId);
        string CreateLinkViewFile(string fileId);
        string CreateLinkPreViewFile(string fileId);
        string CreateLinkThumbnail(string fileId);
        string GetMimeType(string fileName);
        string UploadFileStatic(string fileName, string mimeType, MemoryStream stream, string parents = "");
    }
}