using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreMongoDB.Interfaces
{
    public interface IServiceBase<T>
    {
        T GetByID(string ID);
        Task<T> GetByIDAsync(string ID);
        IEnumerable<T> GetAll();
        Task<IEnumerable<T>> GetAllAsync();
        void Add(T item);
        Task AddAsync(T item);
        void AddRange(IEnumerable<T> listItem);
        Task AddRangeAsync(IEnumerable<T> listItem);
        Task UpdateAsync(Expression<Func<T, bool>> expression, T item);
        void Update(Expression<Func<T, bool>> expression, T item);
        void Remove(string ID);
        Task RemoveAsync(string ID);
        void RemoveRange(IEnumerable<string> listItem);
        Task RemoveRangeAsync(IEnumerable<string> listItem);
        IMongoCollection<T> CreateQuery();
    }
}
