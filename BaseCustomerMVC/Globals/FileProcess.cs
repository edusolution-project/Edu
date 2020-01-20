using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BaseCustomerMVC.Globals
{
    public class FileProcess
    {
        private readonly IHostingEnvironment _evn;
        private string RootPath { get; }
        private readonly HashSet<string> _imageType = new HashSet<string>() { "JPG", "JPEG", "GIF", "PNG", "ICO", "SVG" };
        private readonly HashSet<string> _videoType = new HashSet<string>() { "MP4", "AVI", "WMV", "MOV", "FLV" };
        private readonly HashSet<string> _audioType = new HashSet<string>() { "MP3", "WAV", "WMA", "OGG", "AU", "EA" };
        public FileProcess(IHostingEnvironment evn)
        {
            _evn = evn;
            RootPath = _evn.WebRootPath + "/Files";
        }
        public async Task<string> SaveMediaAsync(IFormFile formFile, string filename = "", string folder = "")
        {
            string extension = Path.GetExtension(formFile.FileName);
            string type = extension.Replace(".", string.Empty).ToUpper();

            if (string.IsNullOrEmpty(folder))
                if (_imageType.Contains(type)) folder = "IMG";
                else if (_videoType.Contains(type)) folder = "VIDEO";
                else if (_audioType.Contains(type)) folder = "AUDIO";
                else folder = "OTHERS";

            folder += ("/" + DateTime.Now.ToString("yyyyMMdd"));
            string uploads = Path.Combine(RootPath, folder);
            if (!Directory.Exists(uploads))
            {
                Directory.CreateDirectory(uploads);
            }
            var fileName = Guid.NewGuid().ToString() + "_" + Path.GetExtension(string.IsNullOrEmpty(filename) ? formFile.FileName : filename);
            using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
            {
                await formFile.CopyToAsync(fileStream);
                return "/Files/" + folder + "/" + fileName;
            }
        }

        public void DeleteFiles(List<string> linkFiles)
        {
            for (int i = 0; linkFiles != null && i < linkFiles.Count; i++)
            {
                var item = linkFiles[i];
                if (File.Exists(item))
                {
                    File.Delete(item);
                }
            }
        }
        public async Task<bool> UpdateAsync(string link, IFormFile file)
        {
            string path = Path.Combine(RootPath, link);
            if (!File.Exists(path))
            {
                return false;
            }
            using (var fileStream = new FileStream(Path.Combine(RootPath, link), FileMode.Open))
            {
                await file.CopyToAsync(fileStream);
                return true;
            }
        }
        public void DeleteFile(string linkFile)
        {
            var item = linkFile;
            if (File.Exists(item))
            {
                File.Delete(item);
            }
        }
    }
}
