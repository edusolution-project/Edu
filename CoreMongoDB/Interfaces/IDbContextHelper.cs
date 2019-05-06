using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreMongoDB.Interfaces
{
    public interface IDbContextHelper<T>
    {
        IMongoCollection<T> CreateQuery();
        IMongoCollection<T> Collection { get;}
    }
}
