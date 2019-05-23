using Microsoft.AspNetCore.Http;
using SME.Utils.Common.SMEException;
using SME.Utils.ExcelUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace SME.Utils.Common
{
    public static class GlobalUtil
    {
        public static string GenerateAccessToken()
        {
            string source = Guid.NewGuid().ToString();
            string hash;
            hash = EncryptUtils.SHA256Encrypt(source, "");
            return hash;
        }
        //Create random captcha by anphabet and nummeric with length
        public static string RandomCaptcha(int length)
        {
            Random random = new Random();
            //string combination = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            string combination = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            StringBuilder captcha = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                captcha.Append(combination[random.Next(combination.Length)]);
            }
            return captcha.ToString();
        }

        public static void ValidateFileXlsx(IFormFile postedFile)
        {
            if (postedFile.Length <= 0)
            {
                throw new BusinessException("Sai định dạng file!");
            }
            if (!postedFile.ContentType.ToLower().Equals("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
            {
                throw new BusinessException("Sai content-type!");
            }
            if (!postedFile.FileName.ToLower().EndsWith(".xlsx"))
            {
                throw new BusinessException("Sai đuôi file!");
            }

            IXVTWorkbook wb = XVTExcel.OpenWorkbook(postedFile.OpenReadStream());
            if (wb == null)
            {
                throw new BusinessException("Không phải file xlsx!");
            }
            postedFile.OpenReadStream().Position = 0;

        }


        //check strong pass (With characters, white space, nummerric, special, capital
        public static bool IsStrongPassword(string password, out string ErrorMessage)
        {
            ErrorMessage = string.Empty;
            // Minimum and Maximum Length of field - 6 to 12 Characters
            if (password.Length < 8 || password.Length > 18)
            {
                ErrorMessage = "Mật khẩu không được nhỏ hơn 8 ký tự hoặc không được lớn hơn 18 ký tự";
                return false;
            }
            // Check white space in pass
            //if (password.Any(c => char.IsWhiteSpace(c)))
            //{
            //    ErrorMessage = "Mật khẩu không được chứa ký tự space";
            //    return false;
            //}

            // Numeric Character - At least one character
            if (!password.Any(c => char.IsDigit(c)))
            {
                ErrorMessage = "Mật khẩu phải chứa ít nhất một ký tự chữ thường";
                return false;
            }
            // special Character - At least one special character
            if (!password.Any(c => char.IsPunctuation(c)))
            {
                ErrorMessage = "Mật khẩu phải chứa ít nhất 1 ký tự đặc biệt";
                return false;
            }
            // At least one Capital Letter
            if (!password.Any(c => char.IsUpper(c)))
            {
                ErrorMessage = "Mật khẩu phải chứa ít nhất một ký tự chữ in hoa";
                return false;
            }
            return true;
        }
        //Convert long to datetime
        public static DateTime MilisecondsToTime(long time)
        {
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0);
            return start.AddMilliseconds(time).ToLocalTime();
        }

        public static string ToUpperFirstCharacter(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return "";
            return str[0].ToString().ToUpper() + str.Substring(1);
        }

        public static T ConvertFormToObject<T>(this IFormCollection nvc) where T : new()
        {
            T RetVal = new T();
            var Entity = typeof(T);
            var PropDict = new Dictionary<string, PropertyInfo>();
            try
            {
                if (nvc != null)
                {
                    var dic = nvc.ToDictionary(o => o.Key, o => o.Value);
                    var Props = Entity.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    PropDict = Props.ToDictionary(p => p.Name.ToUpper(), p => p);
                    var dicList = new Dictionary<string, object>();
                    foreach (var item in dic)
                    {
                        var key = item.Key;
                        var lstKey = key.Split('[');
                        key = lstKey[1].Split(']')[0];
                        if (PropDict.ContainsKey(key.ToUpper()))
                        {
                            var Info = PropDict[key.ToUpper()];
                            if ((Info != null) && Info.CanWrite)
                            {
                                object Val = null;
                                if (Info.PropertyType.IsGenericType && Info.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                                {
                                    Type itemType = Info.PropertyType.GetGenericArguments()[0];
                                    Val = item.Value == DBNull.Value ? null : (object)item.Value.ToList();
                                    //var value = ChangeType(Val, Info.PropertyType);
                                    if (!dicList.ContainsKey(key.ToUpper()))
                                    {
                                        dicList.Add(key.ToUpper(), Val);
                                    }
                                    else
                                    {
                                        List<string> tmpVal = (List<string>)dicList[key.ToUpper()];
                                        var Val1 = item.Value == DBNull.Value ? null : (List<string>)item.Value.ToList();
                                        if (Val1 != null && tmpVal != null)
                                        {
                                            tmpVal.AddRange(Val1);
                                        }
                                        Val = tmpVal;
                                        dicList[key.ToUpper()] = Val;
                                    }
                                    Info.SetValue(RetVal, ChangeType(Val, Info.PropertyType));
                                }
                                else
                                {
                                    Val = item.Value == DBNull.Value ? null : (object)item.Value.ToString();
                                    Info.SetValue(RetVal, ChangeType(Val, Info.PropertyType));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return RetVal;
        }

        public static object ChangeType(object value, Type type)
        {
            var t = type;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return Convert.ChangeType(value, t, CultureInfo.InvariantCulture);
        }

        public static string FormatFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return string.Empty;
            }
            List<string> listName = fullName.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.ToLower()).ToList();
            string sFullName = string.Empty;
            listName.ForEach(x =>
            {
                string sName = x.First().ToString().ToUpper() + x.Substring(1);
                if (sName.Contains("'"))
                {
                    int pos = sName.IndexOf('\'');
                    while (pos > -1 && pos < sName.Length - 1)
                    {
                        string nextName = sName.Substring(pos + 1, 1).ToUpper()
                        + (pos + 2 >= sName.Length ? string.Empty : sName.Substring(pos + 2));
                        sName = sName.Substring(0, pos + 1) + nextName;
                        pos = nextName.IndexOf('\'');
                    }

                }
                sFullName += " " + sName;
            });
            return sFullName.Substring(1);
        }
        public static long DecryptId(string id)
        {
            string sid = StringCipher.Decrypt(id, GlobalConstants.ENCRYPTED_PASSWORD);
            return Convert.ToInt64(sid);
        }
        public static string EncryptId(long id)
        {
            return StringCipher.Encrypt(id.ToString(), GlobalConstants.ENCRYPTED_PASSWORD);
        }
        public static byte[] ReadStreamFully(Stream input)
        {
            input.Position = 0;
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        //check ATTT file pdf
        public static bool IsPDFHeader(Stream fileStream)
        {
            byte[] buffer = null;
            BinaryReader binaryReader = new BinaryReader(fileStream);
            buffer = binaryReader.ReadBytes(5);
            var enc = new ASCIIEncoding();
            var header = enc.GetString(buffer);
            //%PDF−1.0
            // If you are loading it into a long, this is (0x04034b50).
            if (buffer[0] == 0x25 && buffer[1] == 0x50
                && buffer[2] == 0x44 && buffer[3] == 0x46)
            {
                return header.StartsWith("%PDF-");
            }
            return false;

        }
        //check ATTT file jpeg
        public static bool IsJPGHeader(Stream fileStream)
        {
            try
            {
                using (Image image = Image.FromStream(fileStream))
                {
                    return image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
            catch (OutOfMemoryException)
            {
                return false;
            }
        }

        public static void ValidateFileType(IFormFile postedFile)
        {
            if (postedFile.Length <= 0)
            {
                throw new BusinessException("Sai định dạng file!");
            }
            if (postedFile.ContentType.ToLower().Equals("image/jpeg"))
            {
                if (!postedFile.FileName.ToLower().EndsWith(".jpg"))
                {
                    throw new BusinessException("Sai đuôi file!");
                }
                if (!GlobalUtil.IsJPGHeader(postedFile.OpenReadStream()))
                {
                    throw new BusinessException("Sai định dạng file!");
                }
            }
            else if (postedFile.ContentType.ToLower().EndsWith("application/pdf"))
            {
                if (!postedFile.FileName.ToLower().EndsWith(".pdf"))
                {
                    throw new BusinessException("Sai đuôi file!");
                }
                if (!GlobalUtil.IsPDFHeader(postedFile.OpenReadStream()))
                {
                    throw new BusinessException("Sai định dạng file!");
                }
            }
            else
            {
                throw new BusinessException("Bạn chỉ được lựa chọn định dạng file .pdf và jpg để tải lên!");
            }
        }
        public static long DecryptIdFromHex(string id)
        {
            string sid = StringCipher.Decrypt(id, GlobalConstants.ENCRYPTED_PASSWORD, false);
            return Convert.ToInt64(sid);
        }
        public static string EncryptIdToHex(long id)
        {
            return StringCipher.Encrypt(id.ToString(), GlobalConstants.ENCRYPTED_PASSWORD, false);
        }

        public static void SaveStreamToFile(string fileFullPath, Stream stream)
        {
            if (stream.Length == 0) return;

            // Create a FileStream object to write a stream to a file
            using (FileStream fileStream = new FileStream(fileFullPath, FileMode.Create))
            {
                // Fill the bytes[] array with the stream data
                byte[] bytesInStream = new byte[stream.Length];
                stream.Read(bytesInStream, 0, (int)bytesInStream.Length);

                // Use FileStream object to write to the specified file
                fileStream.Write(bytesInStream, 0, bytesInStream.Length);
            }
        }
        public static string EncodedFileNameForDownload(string fileName)
        {
            return EncodedFileNameForSafety(fileName) + "_" + DateTime.Now.ToString("yyyy_MM_dd");
        }
        public static string EncodedFileNameForSafety(string name)
        {
            // first trim the raw string
            string safe = name.Trim();
            // replace spaces with hyphens
            safe = safe.Replace(" ", "-").ToLower();
            // replace any 'double spaces' with singles
            if (safe.IndexOf("--") > -1)
                while (safe.IndexOf("--") > -1)
                    safe = safe.Replace("--", "-");
            // trim out illegal characters
            safe = Regex.Replace(safe, "[^a-z0-9\\-]", "");
            // trim the length
            if (safe.Length > 50)
                safe = safe.Substring(0, 49);
            // clean the beginning and end of the filename
            char[] replace = { '-', '.' };
            safe = safe.TrimStart(replace);
            safe = safe.TrimEnd(replace);
            return safe;
        }
        /// <summary>
        /// Hàm gửi Mail - HungND8 - 17/01/2018
        /// </summary>
        /// <param name="MailTo">Địa chỉ mail gửi tới</param>
        /// <param name="MailFrom">Địa chỉ mail gửi đi</param>
        /// <param name="Subject">Tiêu đề mail</param>
        /// <param name="Content">Nội dung Mail</param>
        /// <param name="MailName">Tên loại mail (Gmail, Exchange, Hotmail...)</param>
        /// <param name="MailHost">Địa chỉ mail Server (smtp.Gmail.com, smtp.hotmail.com...)</param>
        /// <param name="MailPort">Cổng SMTP gửi mail (465, 85, ....)</param>
        /// <param name="MailPass">Pass đăng nhập mail đã được giải mã (Pass trunk)</param>
        private static void SendGmailMail(string MailTo,
            string MailFrom,
            string Subject,
            string Content,
            string MailName,
            string MailHost,
            int MailPort,
            string MailPass)
        {
            var fromAddress = new MailAddress(MailFrom, MailName);
            var toAddress = new MailAddress(MailTo);

            var smtp = new SmtpClient
            {
                Host = MailHost,
                Port = MailPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, MailPass)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = Subject,
                Body = Content,
                BodyEncoding = System.Text.Encoding.UTF8,
                SubjectEncoding = System.Text.Encoding.UTF8,
                HeadersEncoding = System.Text.Encoding.UTF8,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }
    }
}
