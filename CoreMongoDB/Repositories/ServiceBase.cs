using Microsoft.Extensions.Configuration;

namespace CoreMongoDB.Repositories
{
    public abstract class ServiceBase<T> : DbContextHelper<T> where T : EntityBase
    {
        private const string defaultConn = "DefaultConn";
        public ServiceBase(IConfiguration config) : base(config,defaultConn)
        {

        }
        public ServiceBase(IConfiguration config, string tableName) : base(config, tableName, defaultConn)
        {

        }
        public ServiceBase(IConfiguration config, string tableName,string connStr = defaultConn) : base(config, tableName, connStr)
        {

        }
    }
}
