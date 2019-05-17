using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BasePublisherMVC.Globals
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
        public async Task<string> SaveMediaAsync(IFormFile formFile)
        {
            string folder = "";
            string extension = Path.GetExtension(formFile.FileName);
            if (_imageType.Contains(extension.Replace(".",string.Empty).ToUpper())) folder = "IMG";
            if (_videoType.Contains(extension.Replace(".", string.Empty).ToUpper())) folder = "VIDEO";
            if (_audioType.Contains(extension.Replace(".", string.Empty).ToUpper())) folder = "AUDIO";
            string uploads = Path.Combine(RootPath,folder);
            if (!Directory.Exists(uploads))
            {
                Directory.CreateDirectory(uploads);
            }
            var fileName = Guid.NewGuid().ToString() + "_" + Path.GetExtension(formFile.FileName);
            using (var fileStream = new FileStream(Path.Combine(uploads,fileName), FileMode.Create))
            {
                await formFile.CopyToAsync(fileStream);
                return "/Files/"+ folder +"/"+ fileName;
            }
        }
        public void DeleteFiles(List<string> linkFiles)
        {
            for(int i = 0; linkFiles != null && i < linkFiles.Count; i++)
            {
                var item = linkFiles[i];
                if (File.Exists(item))
                {
                    File.Delete(item);
                }
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
