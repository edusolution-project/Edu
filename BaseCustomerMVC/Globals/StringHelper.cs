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
        public static string FixSpecialCharCKE(string orgStr)
        {
            if (string.IsNullOrEmpty(orgStr)) return "";
            var specialCKE = new List<string> { "&amp;", "&nbsp;" };
            return orgStr;
        }
        
        public static string ReplaceSpecialCharacters(string org)
        {

            //dau ‘’
            int[] beginning = { 24, 25, 96 };
            //dau “”
            int[] quotation = { 29, 28 };
            for (int i = 0; i < org.Length; i++)
            {
                if (beginning.Contains((byte)org[i]))
                {
                    org = org.Replace(org[i], '\'');
                }
                if (quotation.Contains((byte)org[i]))
                {
                    org = org.Replace(org[i], '\"');
                }
                if ((byte)org[i] == 125 || (byte)org[i] == 141)
                {
                    org = org.Replace(org[i], '(');
                }
                if ((byte)org[i] == 126 || (byte)org[i] == 142)
                {
                    org = org.Replace(org[i], ')');
                }
            }

            for (int i = 0; i < KyTuDacBiet.Length; i++)
            {
                if (org.Contains(KyTuDacBiet[i]))
                {
                    org = org.Replace(KyTuDacBiet[i], KyTuThuong[i]);
                }
            }
            return org;
        }

        private static readonly String[] KyTuDacBiet = { "&amp;quot;","&amp;","&quot;", "&lt;", "&gt;", "&nbsp;", "&ensp;", "&emsp;", "&thinsp;", "&zwnj;", "&zwj;","&lrm;", "&rlm;",
                                                            "&lsquo;","&rsquo;","&sbquo;","&ldquo;","&rdquo;"};
        private static readonly String[] KyTuThuong = { "\"", "&", "\"", "<", ">", " ", " ", " ", " ", " ", " ", " ", " ", "\'", "\'", ",", "\"", "\"" };
    }
}
