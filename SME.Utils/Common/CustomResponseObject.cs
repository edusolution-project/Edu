using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;

namespace SME.Utils.Common
{
    public class CustomResponseObject
    {
        public CustomResponseObject()
        {
            this.ResponseCode = 0;
            this.ResponseMessage = "SUCCESS";
        }
        /**
         * Contructor by resource key
         * 
         * */
        public CustomResponseObject(int ResponseCode, string ResouceKey, object data = null)
        {
            ResourceManager rm = new ResourceManager("ApplicationResource", Assembly.GetExecutingAssembly());
            String message = rm.GetString(ResouceKey, CultureInfo.CurrentCulture);
            this.ResponseCode = ResponseCode;
            this.ResponseMessage = message;
            this.data = data;
        }
        public CustomResponseObject(int ResponseCode, string FunctionKey, string ResouceKey, object data)
        {
            ResourceManager rm = new ResourceManager("SME.Utils.ApplicationResource",
                Assembly.GetExecutingAssembly());
            string resouceKey = FunctionKey + "_" + ResponseCode.ToString("D3");
            String message = rm.GetString(resouceKey);
            this.ResponseCode = ResponseCode;
            this.ResponseMessage = message;
            this.data = data;
        }

        public CustomResponseObject(int ResponseCode, string FunctionKey, string ResouceKey, object data, string MessageResource)
        {
            ResourceManager rm = new ResourceManager("SME.Utils." + MessageResource.Trim(),
                Assembly.GetExecutingAssembly());
            string resouceKey = FunctionKey + "_" + ResponseCode.ToString("D3");
            String message = rm.GetString(resouceKey);
            this.ResponseCode = ResponseCode;
            this.ResponseMessage = message;
            this.data = data;
        }


        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public object data { get; set; }
    }
}