using BaseCustomerEntity.Database;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace BaseCustomerMVC.Globals
{
    public class StringHelper
    {
        public string FixSpecialCharCKE(string orgStr)
        {
            if (string.IsNullOrEmpty(orgStr)) return "";
            var specialCKE = new List<string> { "&amp;", "&nbsp;" };
            return orgStr;
        }

    }
}
