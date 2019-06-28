using Core_v2.Repositories;
using MongoDB.Driver;

namespace Core_v2.Interfaces
{
    public interface IServiceBase<T>
    {
        IMongoCollection<T> CreateQuery();
        IMongoCollection<T> Collection { get; }
    }
}
