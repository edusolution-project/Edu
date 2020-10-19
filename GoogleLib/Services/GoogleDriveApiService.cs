using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleLib.Services
{
    public class GoogleDriveApiService : GoogleLib.Interfaces.IGoogleDriveApiService
    {
        /// <summary>
        /// xem file 
        /// </summary>
        public string URL_VIEW_FILE { get => "https://drive.google.com/uc?export=view&id={id}"; }
        /// <summary>
        /// link dung cho iframe google
        /// </summary>
        public string URL_PREVIEW_FILE { get => "https://drive.google.com/file/d/{id}/preview"; }
        /// <summary>
        /// xem anh nho 
        /// </summary>
        public string URL_THUMBNAIL { get => "https://drive.google.com/thumbnail?id={id}"; }
        private readonly DriveService _driveService;
        private readonly IConfiguration _configuration;
        public GoogleDriveApiService(IConfiguration configuration)
        {
            _configuration = configuration;
            _driveService = GetDriveService() == null ? null : GetDriveService().Result;
        }

        public async Task<string> Delete(string fileId)
        {
            var result = await _driveService.Files.Delete(fileId).ExecuteAsync();
            return result;
        }

        public async Task<byte[]> ViewFile(string fileId)
        {
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                var resquest = await _driveService.Files.Get(fileId).DownloadAsync(stream);
                var result = stream.GetBuffer();
                stream.Close();
                return result;
            }
        }
        public byte[] DownloadFile(string fileID)
        {
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                if (_driveService.Files.Get(fileID).DownloadWithStatus(stream).Status == Google.Apis.Download.DownloadStatus.Completed)
                {
                    var result = stream.GetBuffer();
                    stream.Close();
                    return result;
                }
                else
                {
                    return null;
                }
            }
        }
        public string GetMimeType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out string contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
        /// <summary>
        /// dung nhieu nhat
        /// </summary>
        /// <param name="fileUpload"></param>
        /// <param name="parents"></param>
        /// <returns></returns>
        public string UploadFileStatic(string fileUpload, string parents = "")
        {
            if (!string.IsNullOrEmpty(fileUpload))
            {
                Google.Apis.Drive.v3.Data.File file = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = System.IO.Path.GetFileName(fileUpload),
                    Description = "File uploaded by GoogleDriveApiService",
                    MimeType = GetMimeType(fileUpload),
                };
                if (!string.IsNullOrEmpty(parents))
                {
                    file.Parents = new List<string>() { parents };
                }
                byte[] byteArray = System.IO.File.ReadAllBytes(fileUpload);
                System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);

                var fileRequest = _driveService.Files.Create(file, stream, GetMimeType(fileUpload));
                fileRequest.Fields = "id";
                IUploadProgress result = fileRequest.Upload();
                if (result.Status == UploadStatus.Completed)
                {
                    var fileID = fileRequest.ResponseBody.Id;
                    _ = ShareFile(fileID);
                    return fileID;
                }
                else
                {
                    return result.Exception.Message;
                }
            }

            return null;
        }


        public string UploadFileStatic(string fileName, string mimeType, Stream stream, string parents = "")
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                Google.Apis.Drive.v3.Data.File file = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = fileName,
                    Description = "File uploaded by GoogleDriveApiService",
                    MimeType = mimeType,
                };
                if (!string.IsNullOrEmpty(parents))
                {
                    file.Parents = new List<string>() { parents };
                }

                var fileRequest = _driveService.Files.Create(file, stream, mimeType);
                fileRequest.Fields = "id";
                IUploadProgress result = fileRequest.Upload();
                if (result.Status == UploadStatus.Completed)
                {
                    var fileID = fileRequest.ResponseBody.Id;
                    _ = ShareFile(fileID);
                    return fileID;
                }
                else
                {
                    return result.Exception.Message;
                }
            }

            return null;
        }

        /// <summary>
        /// tao shared file 
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        private async Task ShareFile(string fileId)
        {
            await _driveService.Permissions.Create(new Permission() { Role = "reader", Type = "anyone" }, fileId).ExecuteAsync();
        }

        public string UploadFile(string fileUpload, string parents = "")
        {
            if (!string.IsNullOrEmpty(fileUpload))
            {
                Google.Apis.Drive.v3.Data.File file = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = System.IO.Path.GetFileName(fileUpload),
                    Description = "File uploaded by GoogleDriveApiService",
                    MimeType = GetMimeType(fileUpload),
                };
                if (!string.IsNullOrEmpty(parents))
                {
                    file.Parents = new List<string>() { parents };
                }
                byte[] byteArray = System.IO.File.ReadAllBytes(fileUpload);
                System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);

                var fileRequest = _driveService.Files.Create(file, stream, GetMimeType(fileUpload));
                fileRequest.Fields = "id";
                IUploadProgress result = fileRequest.Upload();
                if (result.Status == UploadStatus.Completed)
                {
                    return fileRequest.ResponseBody.Id;
                }
                else
                {
                    return result.Exception.Message;
                }
            }

            return null;
        }
        public object UploadFile(string fileName, MemoryStream memoryStream, string mimeType, string parents = "")
        {
            Google.Apis.Drive.v3.Data.File file = new Google.Apis.Drive.v3.Data.File()
            {
                Name = fileName,
                Description = "File uploaded by GoogleDriveApiService",
                MimeType = mimeType,
            };
            if (!string.IsNullOrEmpty(parents))
            {
                file.Parents = new List<string>() { parents };
            }

            var fileRequest = _driveService.Files.Create(file, memoryStream, mimeType);
            fileRequest.Fields = "id";
            return fileRequest.Upload();
        }
        /// <summary>
        /// tao folder 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="des"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public Google.Apis.Drive.v3.Data.File CreateDirectory(string title, string des, string parent = "")
        {
            try
            {
                Google.Apis.Drive.v3.Data.File folder = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = title,
                    Description = des,
                    MimeType = "application/vnd.google-apps.folder",
                };
                if (!string.IsNullOrEmpty(parent))
                {
                    folder.Parents = new List<string> { parent };
                }
                FilesResource.CreateRequest request = _driveService.Files.Create(folder);
                request.Fields = "id";
                return request.Execute();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// dung nhieu nhat
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public string CreateFolder(string folderName, string parentID = "")
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = folderName,
                MimeType = "application/vnd.google-apps.folder",
            };

            if (!string.IsNullOrEmpty(parentID))
            {
                IList<string> parents = new List<string>();
                var parent = _driveService.Files.Get(parentID).Execute();
                if (parent != null && parent.Parents != null && parent.Parents.Count > 0)
                {
                    parents = parent.Parents;
                }
                parents.Add(parentID);
                fileMetadata.Parents = parents;
            }
            FilesResource.CreateRequest request = _driveService.Files.Create(fileMetadata);
            request.Fields = "id";
            var file = request.Execute();
            return file.Id;
        }

        public object GetFolderInfo(string id)
        {
            var file = _driveService.Files.Get(id).Execute();
            return file;
        }
        private async Task<DriveService> GetDriveService(string file, string user, string appName, string fileStorge)
        {
            UserCredential credential;
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] {
                            DriveService.Scope.Drive,
                            DriveService.Scope.DriveAppdata,
                            DriveService.Scope.DriveScripts,
                            DriveService.Scope.DriveReadonly,
                            DriveService.Scope.DrivePhotosReadonly,
                            DriveService.Scope.DriveMetadataReadonly,
                            DriveService.Scope.DriveMetadata,
                            DriveService.Scope.DriveFile
                    },
                    user, CancellationToken.None, new FileDataStore(fileStorge));
            }
            // Create the service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = appName,
            });
            return service;
        }


        public string CreateLinkViewFile(string fileId)
        {
            return URL_VIEW_FILE.Replace("{id}", fileId);
        }
        public string CreateLinkPreViewFile(string fileId)
        {
            return URL_PREVIEW_FILE.Replace("{id}", fileId);
        }
        public string CreateLinkThumbnail(string fileId)
        {
            return URL_THUMBNAIL.Replace("{id}", fileId);
        }

        private async Task<DriveService> GetDriveService(string clientId, string clientSecret, string user, string appName, string fileStorge)
        {
            try
            {
                UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets() { ClientId = clientId, ClientSecret = clientSecret },
                    new[] {
                            DriveService.Scope.Drive,
                            DriveService.Scope.DriveAppdata,
                            DriveService.Scope.DriveScripts,
                            DriveService.Scope.DriveReadonly,
                            DriveService.Scope.DrivePhotosReadonly,
                            DriveService.Scope.DriveMetadataReadonly,
                            DriveService.Scope.DriveMetadata,
                            DriveService.Scope.DriveFile
                    },
                    user, CancellationToken.None, new FileDataStore(fileStorge));
                // Create the service.
                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = appName,
                });
                return service;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<DriveService> GetDriveService()
        {
            try
            {
                string file = _configuration.GetSection("GOOGLE:DRIVE:FILE")?.Value,
                clientId = _configuration.GetSection("GOOGLE:DRIVE:CLIENT_ID")?.Value,
                clientSecret = _configuration.GetSection("GOOGLE:DRIVE:CLIENT_SECRET")?.Value,
                user = _configuration.GetSection("GOOGLE:DRIVE:USER")?.Value,
                appName = _configuration.GetSection("GOOGLE:DRIVE:APP")?.Value,
                fileStorge = _configuration.GetSection("GOOGLE:DRIVE:STORGE")?.Value;

                if (string.IsNullOrEmpty(file))
                {
                    return await GetDriveService(clientId, clientSecret, user, appName, fileStorge);
                }
                else
                {
                    return await GetDriveService(file, user, appName, fileStorge);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


    }
}
