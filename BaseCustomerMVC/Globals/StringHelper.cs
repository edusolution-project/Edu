using BaseCustomerEntity.Database;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Spire.Doc;
using Spire.Doc.Documents;
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

        public static async Task<string> ConvertDocToHtml(Table table, string basis, string user, string StaticPath = "")
        {
            var cell = table.Rows[3].Cells[1];

            //Create New doc2
            Document doc2 = new Document();
            Section s2 = doc2.AddSection();

            foreach (DocumentObject item in cell.ChildObjects)
            {
                if (item is Table)
                {
                    Table newtb = (Table)item.Clone();
                    s2.Tables.Add(newtb);
                }
                else
                if (item is Paragraph)
                {
                    Paragraph newPara = (Paragraph)item.Clone();
                    s2.Paragraphs.Add(newPara);
                }
            }
            var path = Path.Combine(StaticPath, "Files/" + basis + "/" + user);
            var temp = DateTime.Now.ToString("yyyyMMhhmmss");

            String content = "";
            using (MemoryStream memory = new MemoryStream())
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                doc2.SaveToFile(path + "/" + temp + ".html", FileFormat.Html);
                using (var reader = new StreamReader(path + "/" + temp + ".html"))
                {
                    content = reader.ReadToEnd();
                }
                try
                {
                    File.Delete(path + "/" + temp + ".html");
                    File.Delete(path + "/" + temp + ".css");
                    File.Delete(path + "/" + temp + "_styles.css");
                }
                catch
                {
                }
            }

            for (int i = 0; i < 1; i++)
            {
                Int32 lastIndex = content.IndexOf("</p>");
                content = content.ToString().Remove(0, lastIndex + 4);
            }
            String strToReplace4 = "<br></div></body></html>";
            String strToReplace2 = @"</div></body></html>";
            String strToreplace3 = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\"><html xmlns=\"http://www.w3.org/1999/xhtml\"><head><meta http-equiv=\"Content-Type\" content=\"application/xhtml+xml; charset=utf-8\" /><title></title></head><body style=\"pagewidth:595.35pt;pageheight:841.95pt;\"><div class=\"Section0\"><div style=\"min-height:20pt\" /><p class=\"Normal\"><span style=\"color:#FF0000;font-size:12pt;\"></span></p>";
            string test = content
                .ToString()
                .Replace("Evaluation Warning: The document was created with Spire.Doc for .NET.", "")
                .Replace(strToReplace4, "")
                .Replace(strToReplace2, "")
                .Replace(temp + "_images/", "/Files/" + basis + "/" + user + "/" + temp + "_images/")
                .Replace("<link href=\"" + temp + "_styles.css\" type=\"text/css\" rel=\"stylesheet\"/>", "")
                .Replace(strToreplace3, "");

            return test.Trim();
        }

    }
}
