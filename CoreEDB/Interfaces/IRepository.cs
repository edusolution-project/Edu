using CoreEDB.CoreModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreEDB.Interfaces
{
    public interface IRepository<TEntity> where TEntity : EntityBase
    {
        int TotalRecord { get; }
        TEntity Get(int id);
        int Complete();
        Task<int> CompleteAsync();
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        IEnumerable<TEntity> GetValueByPage(Expression<Func<TEntity, bool>> predicate, int pageIndex, int pageSize);
        // This method was not in the videos, but I thought it would be useful to add.
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);
        TEntity SelectFirst(Expression<Func<TEntity, bool>> predicate);
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        IEnumerable<TEntity> Find(bool isReady, Expression<Func<TEntity, bool>> predicate);
        void Update(TEntity oldItem, TEntity newItem);
        IEnumerable<TEntity> ToListCache(Expression<Func<TEntity, bool>> predicate);
        IEnumerable<TEntity> ToListCache(Expression<Func<TEntity, bool>> predicate, int expires);
        void ClearCache();
    }
}
