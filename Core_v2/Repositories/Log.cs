using Core_v2.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Core_v2.Repositories
{
    public class Log : ILog
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;
        private string _path { get; set; }
        public Log(IConfiguration configuration, IHostingEnvironment env)
        {
            _env = env;
            _configuration = configuration;
            _path = string.IsNullOrEmpty(getPathLog()) ? _env.WebRootPath : getPathLog();
        }

        public Log(IConfiguration configuration)
        {
            _configuration = configuration;
            _path = string.IsNullOrEmpty(getPathLog()) ? "" : getPathLog();
        }
        public Task Debug(string function, object content)
        {
            var path = Path.Combine(_path,"weblog", function + "\\Debug");
            return writeLog(path, JsonConvert.SerializeObject(content));
        }

        public Task Error(string function, string content, Exception ex)
        {
            var path = Path.Combine(_path, "weblog", function + "\\Error");
            return writeLog(path, content +"\n\r" + ex.ToString());
        }

        public Task Error(string function, Exception ex)
        {
            var path = Path.Combine(_path, "weblog", function + "\\Error");
            return writeLog(path, ex.ToString());
        }

        public Task Info(string function, string content)
        {
            var path = Path.Combine(_path, "weblog", function + "\\Info");
            return writeLog(path, content);
        }

        private Task writeLog(string path,string content)
        {
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            string fileName = string.Format("{0:yyyyMddHHmm}", DateTime.Now) + ".json";
            string logFile = Path.Combine(path, fileName);
            using (StreamWriter logWriter = File.AppendText(logFile))
            {
                logWriter.WriteLine(proccessValue(content));
            }
            return Task.CompletedTask;
        }
        private string proccessValue(string value)
        {
            Dictionary<string, object> obj = new Dictionary<string, object>()
            {
                {"Time :", DateTime.Now },
                {"Value :", value}
            };
            return JsonConvert.SerializeObject(obj);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns> /{folder}/{folder}/ </returns>
        private string getPathLog()
        {
            try
            {
               return _configuration.GetSection("Path:Logs").Value;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
