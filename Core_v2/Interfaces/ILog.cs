using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core_v2.Interfaces
{
    public interface ILog
    {
        Task Info(string function,string content);
        Task Error(string function, string content, Exception ex);
        Task Error(string function, Exception ex);
        Task Debug(string function, object content);
    }
}
