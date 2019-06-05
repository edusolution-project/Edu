using CoreEDB.CoreModels;
using CoreEDB.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace CoreEDB.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : EntityBase
    {
        protected readonly DbContext _context;
        private readonly string _tableName;
        public Repository(DbContext context,string tableName)
        {
            _context = context;
            _tableName = tableName;
        }

        public int TotalRecord
        {
            get
            {
                return _context != null ? _context.Set<TEntity>().Count() : 0;
            }
        }

        public void Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
        }
        public int Complete()
        {
            QueryCacheManager.ExpireTag(_tableName);
            return _context.SaveChanges();
        }
        public async Task<int> CompleteAsync()
        {
            QueryCacheManager.ExpireTag(_tableName);
            return await _context.SaveChangesAsync();
        }
        public void AddRange(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().AddRange(entities);
        }

        public IEnumerable<TEntity> Find(bool isReady,Expression<Func<TEntity, bool>> predicate)
        {
            return isReady 
                ? _context.Set<TEntity>().Where(predicate)
                : _context.Set<TEntity>();
        }

        public void Update(TEntity oldItem,TEntity newItem)
        {
            newItem.ID = oldItem.ID;
            _context.Entry(oldItem).CurrentValues.SetValues(newItem);
        }
        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().Where(predicate);
        }

        public TEntity Get(int id)
        {
            return _context.Set<TEntity>().Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _context.Set<TEntity>().ToList();
        }

        public IEnumerable<TEntity> GetValueByPage(Expression<Func<TEntity, bool>> predicate, int pageIndex, int pageSize)
        {
            return _context.Set<TEntity>()
                .Where(predicate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public void Remove(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().RemoveRange(entities);
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().SingleOrDefault(predicate);
        }
        public TEntity SelectFirst(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().FirstOrDefault(predicate);
        }
        public IEnumerable<TEntity> ToListCache(Expression<Func<TEntity, bool>> predicate)
        {
            var data = (IQueryable<TEntity>)_context.Set<TEntity>().Where(predicate).FromCache(
                new MemoryCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromMinutes(120)
                }, _tableName);
            return data;
        }
        public IEnumerable<TEntity> ToListCache(Expression<Func<TEntity, bool>> predicate,int expires)
        {
            var data = (IQueryable<TEntity>)_context.Set<TEntity>().Where(predicate).FromCache(
                new MemoryCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromMinutes(expires)
                }, _tableName);
            return data;
        }
        public void ClearCache()
        {
            QueryCacheManager.ExpireTag(_tableName);
        }
    }
}
