using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SME.API.Models
{
    public class AuthenFailModel
    {
        public string ErrorDescription { get; set; }
        public int FailCount { get; set; }
        public string FieldName { get; set; }

        public string ToJSon()
        {
            return JsonConvert.SerializeObject(this);
        }
        public static AuthenFailModel FromJson(string jsonStr)
        {
            return JsonConvert.DeserializeObject(jsonStr) as AuthenFailModel;
        }
    }
}