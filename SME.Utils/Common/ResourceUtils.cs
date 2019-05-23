using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace SME.Utils.Common
{
    public static class ResourceUtils
    {
        public static string GetResourceByKey(string resouceKey)
        {
            ResourceManager rm = new ResourceManager("SME.Utils.ApplicationResource",
                Assembly.GetExecutingAssembly());
            return rm.GetString(resouceKey);
        }

        public static string GetResourceByKey(string resource, string name)
        {
            ResourceManager rm = new ResourceManager("SME.Utils." + resource, Assembly.GetExecutingAssembly());
            return rm.GetString(name);
        }
        public static string Message { get; set; }

        public static string GetMessage(string message)
        {
            if (message.Length > 0)
                return Message = message;
            else
                return string.Empty;
        }
    }
}
