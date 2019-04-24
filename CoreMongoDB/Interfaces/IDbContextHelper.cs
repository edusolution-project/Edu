using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreMongoDB.Interfaces
{
    public interface IDbContextHelper<T>
    {
        T GetByID(string ID);
        Task<T> GetByIDAsync(string ID);
        List<T> GetAll();
        Task<List<T>> GetAllAsync();
        void Add(T item);
        Task AddAsync(T item);
        void AddRange(List<T> listItem);
        Task AddRangeAsync(List<T> listItem);
        void Remove(string ID);
        Task RemoveAsync(string ID);
        void RemoveRange(List<string> listItem);
        Task RemveRangeAsync(List<string> listItem);
        IMongoCollection<T> CreateQuery();
    }
}
