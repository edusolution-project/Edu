using System;
using System.Threading.Tasks;

namespace CoreLogs
{
    public interface ILogs
    {
        Task WriteLogs(string msg);
        Task WriteLogsError(string msg);
        Task WriteLogsError(Exception ex);
        Task WriteLogsError(string Name, Exception ex);
        Task WriteLogsError(string Name, string param, Exception ex);
        Task WriteLogsInfo(string msg);
    }
}
