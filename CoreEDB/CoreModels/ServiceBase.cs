using CoreEDB.Database;
using CoreEDB.Interfaces;
using CoreEDB.Repositories;

namespace CoreEDB.CoreModels
{
    public abstract class ServiceBase<T> where T : EntityBase
    {
        protected readonly string _tableName;
        protected readonly ExtendsContext<T> _contenxt;
        public ServiceBase(string tableName)
        {
            _tableName = tableName;
            _contenxt = new ExtendsContext<T>(_tableName);
        }
        public IRepository<T> CreateQuery()
        {
            return new Repository<T>(_contenxt,_tableName);
        }
    }
}
