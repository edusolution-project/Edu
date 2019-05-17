using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BasePublisherModels.Database
{
    public class CPLoginLogEntity : EntityBase
    {
        public string IP { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public bool IsActive { get; set; }
        public DateTime Created { get; set; }
    }
    public class CPLoginLogService : ServiceBase<CPLoginLogEntity>
    {
        public CPLoginLogService(IConfiguration config) : base(config, "CPLoginLogs")
        {

        }

        public CPLoginLogService(IConfiguration config, string tableName) : base(config, tableName)
        {
        }

        public CPLoginLogEntity GetItemByCode(string token)
        {
            return CreateQuery().Find(o => o.Token == token)?.SingleOrDefault();
        }
        public CPLoginLogEntity GetItemByEmail(string Email)
        {
            return CreateQuery().Find(o => o.Email == Email)?.SingleOrDefault();
        }
        public Task RemoveLog(string token)
        {
            var item = GetItemByCode(token);
            if (item == null) return Task.CompletedTask;
            Remove(item.ID);
            return Task.CompletedTask;
        }
        public Task SetLogin(CPLoginLogEntity item)
        {
            if (item == null) return Task.CompletedTask;
            var current = GetItemByEmail(item.Email);
            if (current == null)
            {
                Add(item);
                return Task.CompletedTask;
            }
            else
            {
                item.ID = current.ID;
                Add(item);
                return Task.CompletedTask;
            }
        }
        public string GetEmailFromDb(string token)
        {
            var data = CreateQuery().Find(o => o.IsActive == true && o.Token == token)?.SingleOrDefault();
            return data == null ? string.Empty : data.Email;
        }
    }
}
