using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Dto.Util
{
  public  class GlobalUtil
    {
        public static string GenerateAccessToken()
        {
            string source = Guid.NewGuid().ToString();
            string hash;
            hash = EncryptUtils.SHA256Encrypt(source, "");
            return hash;
        }
    }
}
