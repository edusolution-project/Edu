using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SME.API.Models
{
    public class ErrorModel
    {
        public string error { get; set; }
        public string error_description { get; set; }

        public static ErrorModel FromJson(string jsonStr)
        {
            return JsonConvert.DeserializeObject(jsonStr) as ErrorModel;
        }
    }
}