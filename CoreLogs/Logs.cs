using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace CoreLogs
{
    public class Logs : ILogs
    {
        protected string _path;
        protected string _fileName = string.Format("{0:yyyyMMdd}", DateTime.Now) + ".txt";
        private readonly IHostingEnvironment _environment;
        public Logs(IHostingEnvironment environment)
        {
            _environment = environment;
            _path = _environment.WebRootPath + "/Logs";
        }
        public Logs(string path)
        {
            _path = path.IndexOf("/Logs")>-1 ? path : path + "/Logs";
        }
        public Task WriteLogs(string msg)
        {
            string path = _path;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return WriteLogsAsync(path, msg);
        }
        ~Logs()
        {
            
        }
        public Task WriteLogsError(string Name, string param ,Exception ex)
        {
            string msg = "Function : " + Name + "\r\n"
                        + "Param : " + param + "\r\n"
                        + "Message : " + ex.Message + "\r\n"
                        + "Source : " + ex.Source + "\r\n"
                        + "TargetSite : " + ex.TargetSite + "\r\n"
                        + "InnerException : " + ex.InnerException + "\r\n"
                        + "StackTrace : " + ex.StackTrace + "\r\n"
                        + "HResult : " + ex.HResult + "\r\n"
                        + "HelpLink : " + ex.HelpLink + "\r\n"
                        ;
            string path = _path + "/Error/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return WriteLogsAsync(path, msg);
        }
        public Task WriteLogsError(string Name,Exception ex)
        {
            string msg = "Function : " + Name + "\r\n"
                        + "Message : " + ex.Message + "\r\n"
                        + "Source : " + ex.Source + "\r\n"
                        + "TargetSite : " + ex.TargetSite + "\r\n"
                        + "InnerException : " + ex.InnerException + "\r\n"
                        + "StackTrace : " + ex.StackTrace + "\r\n"
                        + "HResult : " + ex.HResult + "\r\n"
                        + "HelpLink : " + ex.HelpLink + "\r\n"
                        ;
            string path = _path + "/Error/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return WriteLogsAsync(path, msg);
        }
        public Task WriteLogsError(Exception ex)
        {
            string msg = "Message : " + ex.Message + "\r\n"
                        + "Source : " + ex.Source + "\r\n"
                        + "TargetSite : " + ex.TargetSite + "\r\n"
                        + "InnerException : " + ex.InnerException + "\r\n"
                        + "StackTrace : " + ex.StackTrace + "\r\n"
                        + "HResult : " + ex.HResult + "\r\n"
                        + "HelpLink : " + ex.HelpLink + "\r\n"
                        ;
            string path = _path + "/Error/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return WriteLogsAsync(path, msg);
        }
        public Task WriteLogsError(string msg)
        {
            string path = _path + "/Error/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return WriteLogsAsync(path, msg);
        }

        public Task WriteLogsInfo(string msg)
        {
            string path = _path + "/Info/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return WriteLogsAsync(path, msg);
        }

        protected Task WriteLogsAsync(string path,string msg)
        {
            string logFile = path + _fileName;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Time : " + string.Format("{0: yyyy-MM-dd HH:mm:ss}", DateTime.Now) + "======================");
            sb.AppendLine(msg);
            sb.AppendLine("End ======================================================================================== ");

            if (!File.Exists(logFile))
            {
                var file = File.Create(logFile);
                using (var stream = new StreamWriter(file))
                {
                    var streamwrite = stream.WriteLineAsync(sb.ToString());
                    
                    stream.Dispose();
                    file.Dispose();
                }
            }
            else
            {

                using (var logWriter = new StreamWriter(logFile,true))
                {
                    logWriter.NewLine = sb.ToString();
                    //string content = sb.ToString() + fileText;
                    logWriter.WriteLineAsync();
                    logWriter.Dispose();
                };
            }
            
            return Task.CompletedTask;
        }
    }
}
