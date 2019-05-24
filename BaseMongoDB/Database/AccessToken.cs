using Business.Dto.Util;
using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Business.Dto.Util;
//using SME.Utils.Common;
namespace BaseMongoDB.Database
{
    public class AccessTokenEntity : EntityBase
    {
        public string ACCESS_TOKEN_KEY { get; set; }
        public string NGUOI_DUNG_ID { get; set; }
        public DateTime EXPIRED_TIME { get; set; }
        public DateTime STARTED_TIME { get; set; }
        public long IS_LOG_OUT { get; set; }
        public string USER_AGENT { get; set; }
        public string USERNAME { get; set; }
    }
    public class AccessTokenService : ServiceBase<AccessTokenEntity>
    {
        public AccessTokenService(IConfiguration config) : base(config, "AccessToken")
        {

        }

        public AccessTokenService(IConfiguration config, string tableName) : base(config, tableName)
        {
        }

        public string GetNewToken(string nguoiDungId, string userAgent, string username)
        {
          // AccessTokenService accessTokenService = GetService<AccessTokenService>();
            string token = GlobalUtil.GenerateAccessToken();
            string encryptedToken = EncryptUtils.SHA256Encrypt(token, username);
            //while (accessTokenService.Find(encryptedToken) != null)
            //{
            //    token = GlobalUtil.GenerateAccessToken();
            //    encryptedToken = EncryptUtils.SHA256Encrypt(token, username);
            //}
            //ACCESS_TOKEN at = new ACCESS_TOKEN();
            //at.ACCESS_TOKEN_KEY = encryptedToken;
            //at.EXPIRED_TIME = DateTime.Now.AddMinutes(GlobalConstants.SESSION_TIMEOUT);
            //at.IS_LOG_OUT = GlobalConstants.FALSE;
            //at.NGUOI_DUNG_ID = nguoiDungId;
            //at.STARTED_TIME = DateTime.Now;
            //at.USER_AGENT = userAgent;
            //at.USERNAME = username;
            //Insert(at);
            //Save();
            //string token = GlobalUtil.GenerateAccessToken();
            //string encryptedToken = EncryptUtils.SHA256Encrypt(token, username);
            //var data = CreateQuery().Find(o=>o.ACCESS_TOKEN_KEY== encryptedToken)
            return token;
        }
    }
}
