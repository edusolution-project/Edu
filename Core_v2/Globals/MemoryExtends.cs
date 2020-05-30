using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Text;

namespace Core_v2.Globals
{
    public class MemoryExtends
    {
    }
    public class Security
    {
        private const string Key = "123@Abc#$%*";
        public static string Encrypt(string toEncrypt)
        {
            bool useHashing = true;
            byte[] keyArray;
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(Key));
            }
            else
                keyArray = Encoding.UTF8.GetBytes(Key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        public static string Decrypt(string toDecrypt)
        {
            bool useHashing = true;
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(Key));
            }
            else
                keyArray = Encoding.UTF8.GetBytes(Key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }
    }
    public static class CacheExtends
    {
        public const string DefaultIsControl = "CPControl";
        public const string DefaultPermission = "Permission";
        public const string DefaultLang = "Language";
        public const string DefaultResource = "Resource";
        public const string DefaultAdminController = "CPMenu";
        public const string DefaultClientController = "ClientMenu";

        #region cache
        /// <summary>
        /// get cache
        /// </summary>
        /// <typeparam name="T">kieu du lieu</typeparam>
        /// <param name="nameCache">key</param>
        /// <returns></returns>
        public static T GetDataFromCache<T>(string nameCache)
        {
            ObjectCache cache = MemoryCache.Default;
            var cachedObject = (T)cache[nameCache];
            return cachedObject;
        }
        /// <summary>
        /// set cache
        /// </summary>
        /// <typeparam name="T"> kieu du lieu </typeparam>
        /// <param name="nameCache"> key </param>
        /// <param name="expire"> thoi gia luu phut </param>
        /// <param name="value"> du lieu </param>
        public static void SetObjectFromCache<T>(string nameCache, int expire, T value)
        {
            ObjectCache cache = MemoryCache.Default;
            var cachedObject = value;
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(expire)
            };
            cache.Set(nameCache, cachedObject, policy);

        }
        /// <summary>
        /// clear cache chi dinh 
        /// </summary>
        /// <param name="keyName">key</param>
        public static void ClearCache(string keyName)
        {
            ObjectCache cache = MemoryCache.Default;
            var obj = cache.Get(keyName);
            if (obj != null)
            {
                cache.Remove(keyName);
            }
        }
        /// <summary>
        /// xoas tat ca cache
        /// </summary>
        public static void ClearCacheAll()
        {
            var cache = MemoryCache.Default;
            if (cache.Count() == 0) return;
            foreach (var item in cache)
            {
                cache.Remove(item.Key);
            }
        }
        public static List<string> Keys()
        {
            var cache = MemoryCache.Default;
            return cache.Count() == 0 ? null : cache.Select(o => o.Key)?.ToList();
        }
        #endregion
    }
    public static class Cookies
    {
        public const int ExpiresLogin = 30 * 24 * 60;
        public const string DefaultLogin = "_token";
        public const string DefaultLang = "_language";
        public static void Remove(this HttpContext context, string key)
        {
            if (context.IsExist(key)) context.Response.Cookies.Delete(key);
        }
        public static bool IsExist(this HttpContext context, string key)
        {
            if (context == null) return false;
            if (string.IsNullOrEmpty(key)) return false;
            return context.Request.Cookies[key] != null;
        }
        public static string GetValue(this HttpContext context, string key, bool secure)
        {
            if (context.IsExist(key))
            {
                string value = context.Request.Cookies[key];
                if (string.IsNullOrEmpty(value)) return value;
                return secure ? Security.Decrypt(value) : value;
            }
            else
            {
                return string.Empty;
            }
        }
        public static void SetValue(this HttpContext context, string key, string value)
        {
            Set(context, key, value, null, 0, false);
        }
        public static void SetValue(this HttpContext context, string key, string value, bool secure)
        {
            Set(context, key, value, null, 0, secure);
        }
        public static void SetValue(this HttpContext context, string key, string value, int? expire, bool secure)
        {
            Set(context, key, value, null, expire, secure);
        }
        public static void SetValue(this HttpContext context, string key, string value, CookieOptions option, int? expire, bool secure)
        {
            Set(context, key, value, option, expire, secure);
        }
        private static void Set(HttpContext context, string key, string value, CookieOptions option, int? expireTime, bool secure)
        {
            try
            {
                if (option == null)
                {
                    option = new CookieOptions
                    {
                        Secure = secure,
                        HttpOnly = true,
                        Domain = context.Request.Host.Host,
                        Path = "/"
                    };

                    if (expireTime.HasValue)
                        option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
                    else
                        option.Expires = DateTime.Now.AddDays(0);
                }

                //check for encryption 

                string encodedValue = secure ? Security.Encrypt(value) : value;
                var template = new SetCookieHeaderValue(key)
                {
                    Domain = option.Domain,
                    Expires = option.Expires,
                    HttpOnly = option.HttpOnly,
                    Path = option.Path,
                    Secure = option.Secure,
                };
                var responseCookies = context.Response.Cookies;
                responseCookies.Append(key, encodedValue, option);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetCurrentLang(this HttpContext context, string value)
        {
            if (context.IsExist(DefaultLang))
            {
                return context.GetValue(DefaultLang, false);
            }
            else
            {
                context.SetValue(DefaultLang, value, 360 * 24 * 60, false);
                return value;
            }
        }
        public static void SetCurrentLang(this HttpContext context, string value)
        {
            context.SetValue(DefaultLang, value, 360 * 24 * 60, false);
        }
    }
}
