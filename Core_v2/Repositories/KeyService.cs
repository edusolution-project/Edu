using Core_v2.Globals;
using Core_v2.Interfaces;

namespace Core_v2.Repositories
{
    public class KeyService : IKeyServices
    {
        public KeyModel Decrypt(string key)
        {
            var dataKey = Security.Decrypt(key);
            return string.IsNullOrEmpty(dataKey)? null : Newtonsoft.Json.JsonConvert.DeserializeObject<KeyModel>(dataKey);
        }

        public string EncryptKey(KeyModel dataKey)
        {
            var data = Newtonsoft.Json.JsonConvert.SerializeObject(dataKey);
            return Security.Decrypt(data);
        }
    }
}
