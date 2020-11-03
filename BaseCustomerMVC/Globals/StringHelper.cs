﻿using BaseCustomerEntity.Database;
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
        public static string FixSpecialCharCKE(string orgStr)
        {
            if (string.IsNullOrEmpty(orgStr)) return "";
            var specialCKE = new List<string> { "&amp;", "&nbsp;" };
            return orgStr;
        }
        
        public static string ReplaceSpecialCharacters(string str)
        {
            //dau ‘’
            int[] beginning = { 24, 25, 96 };
            //dau “”
            int[] quotation = { 29, 28 };
            for (int i = 0; i < str.Length; i++)
            {
                if (beginning.Contains((byte)str[i]))
                {
                    str = str.Replace(str[i], '\'');
                }
                if (quotation.Contains((byte)str[i]))
                {
                    str = str.Replace(str[i], '\"');
                }
                if ((byte)str[i]==125 || (byte)str[i] == 141)
                {
                    str = str.Replace(str[i], '(');
                }
                if ((byte)str[i] == 126 || (byte)str[i] == 142)
                {
                    str = str.Replace(str[i], ')');
                }
            }
            return str;
        }
    }
}
