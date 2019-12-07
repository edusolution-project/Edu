using Core_v2.Globals;

namespace Core_v2.Interfaces
{
    public interface IKeyServices
    {
        string EncryptKey(KeyModel item);
        KeyModel Decrypt(string key);
    }
}
