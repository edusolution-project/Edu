using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace BasePublisherModels.Database
{
    public class CPAccessEntity : EntityBase
    {
        public string CModule { get; set; }
        public string CMethod { get; set; }
        public string RoleID { get; set; }
        public bool IsActive { get; set; }
    }
    public class CPAccessService : ServiceBase<CPAccessEntity>
    {
        public CPAccessService(IConfiguration config) : base(config, "CPAccess")
        {

        }
        public CPAccessService(IConfiguration config,string tableName) : base(config, tableName)
        {

        }
        public bool GetPermission(string roleID, string ctrlName, string actName)
        {
            var data = GetItem(roleID, ctrlName, actName);
            if (data == null) return false;
            else return data.IsActive;
        }
        public CPAccessEntity GetItem(string roleID, string ctrlName, string actName)
        {
            var data = CreateQuery().Find(o => o.RoleID == roleID && o.CModule == ctrlName && o.CMethod == actName).SingleOrDefault();
            return data ?? null;
        }
        public Dictionary<string, bool> GetPermissions(string roleID)
        {
            var data = CreateQuery().Find(o => o.RoleID == roleID).ToList();
            if (data == null) return null;
            else
            {
                var res = new Dictionary<string, bool>();
                int count = data.Count();
                for (int i = 0; i < count; i++)
                {
                    var item = data[i];
                    res.Add(item.CModule + "-" + item.CMethod, item.IsActive);
                }
                return res;
            }
        }
        public List<CPAccessEntity> GetItemByRoleID(string roleID)
        {
            return CreateQuery().Find(o => o.RoleID == roleID)?.ToList();
        }
    }
}
