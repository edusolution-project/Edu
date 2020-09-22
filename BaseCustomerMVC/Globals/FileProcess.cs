using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
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

        public FileProcess(IHostingEnvironment evn, IConfiguration iConfig)
        {
            _evn = evn;
            RootPath = (iConfig.GetValue<string>("SysConfig:StaticPath") ?? _evn.WebRootPath) + "/Files";
        }
        public async Task<string> SaveMediaAsync(IFormFile formFile, string filename = "", string folder = "", string center = "", bool resize = false, int stat_width = 600, int stat_height = 800)
        {
            string extension = Path.GetExtension(formFile.FileName);
            string type = extension.Replace(".", string.Empty).ToUpper();

            if (string.IsNullOrEmpty(folder))
                if (_imageType.Contains(type)) folder = "IMG";
                else if (_videoType.Contains(type)) folder = "VIDEO";
                else if (_audioType.Contains(type)) folder = "AUDIO";
                else folder = "OTHERS";
            if (!string.IsNullOrEmpty(center))
                folder = center + "/" + folder;
            folder += ("/" + DateTime.Now.ToString("yyyyMMdd"));
            string uploads = Path.Combine(RootPath, folder);
            if (!Directory.Exists(uploads))
            {
                Directory.CreateDirectory(uploads);
            }
            //Resize from stream
            var fileName = Guid.NewGuid().ToString() + "_" + Path.GetExtension(string.IsNullOrEmpty(filename) ? formFile.FileName : filename);
            /***************************/
            //Neu la anh thi resize
            if (_imageType.Contains(type))
            {
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);
                //if (stat_width == 0 || stat_height == 0)
                //{
                //    stat_width = 512;
                //    stat_height = 384;
                //}
                var standardSize = new SixLabors.Primitives.Size(stat_width, stat_height);

                using (Stream inputStream = formFile.OpenReadStream())
                {
                    using (var image = Image.Load<Rgba32>(inputStream))
                    {
                        var imageEncoder = new JpegEncoder()
                        {
                            Quality = 90,
                            Subsample = JpegSubsample.Ratio444
                        };

                        int width = image.Width;
                        int height = image.Height;
                        if ((width > standardSize.Width) || (height > standardSize.Height))
                        {
                            ResizeOptions options = new ResizeOptions
                            {
                                Mode = ResizeMode.Max,
                                Size = standardSize,
                            };
                            image.Mutate(x => x
                             .Resize(options));

                            //.Grayscale());
                        }
                        //image.Save($"/Files/{folder}/{fileName}"); // Automatic encoder selected based on extension.
                        using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                        {
                            image.Save(fileStream, imageEncoder);
                            //await formFile.CopyToAsync(fileStream);
                            //return "/Files/" + folder + "/" + fileName;
                            return $"{"/Files/"}/{folder}/{fileName}";
                        }
                    }
                }
            }
            /*********************************/
            else
            {
                using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                {
                    await formFile.CopyToAsync(fileStream);
                    //return "/files/" + folder + "/" + filename;
                    return $"{"/Files"}/{folder}/{fileName}";
                }
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
