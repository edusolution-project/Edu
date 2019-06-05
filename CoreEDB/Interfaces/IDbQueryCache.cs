namespace CoreEDB.Interfaces
{
    public interface IDbQueryCache
    {
        T GetDataFromCache<T>(string nameCache);
        void SetObjectFromCache<T>(string nameCache, int expire, T value);
        void ClearCache(string keyName);
        void ClearCacheAll();
    }
}
