using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;

namespace MVCBase.Globals
{
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
