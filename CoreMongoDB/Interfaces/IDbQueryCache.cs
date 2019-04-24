namespace CoreMongoDB.Interfaces
{
    public interface IDbQueryCache
    {
        T GetDataFromCache<T>(string nameCache);
        void SetObjectFromCache<T>(string nameCache, int expire, T value);
        void SetObjectFromCache<T>(string nameCache, T value);
        void ClearCache(string keyName);
        void ClearCacheAll();
    }
}
