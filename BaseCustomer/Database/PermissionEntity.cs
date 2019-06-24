using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Collections.Generic;

namespace BaseCustomerEntity.Database
{
    public class PermissionEntity:EntityBase
    {
        public string CtrlName { get; set; }
        public string ActName { get; set; }
        public string RoleID { get; set; }
        public bool IsActive { get; set; }
    }
    public class PermissionService : ServiceBase<PermissionEntity>
    {
        public PermissionService(IConfiguration configuration) : base(configuration)
        {

        }
        public List<PermissionEntity> GetIsActive()
        {
            return CreateQuery().Find(o=>o.IsActive == true).ToList();
        }
        public List<PermissionEntity> GetNonActive()
        {
            return CreateQuery().Find(o => o.IsActive == false).ToList();
        }
        public PermissionEntity GetPermission(string roleID,string ctrl,string act)
        {
            return CreateQuery().Find(o => o.CtrlName == ctrl && o.ActName == act && o.RoleID == roleID).SingleOrDefault();
        }
        public List<PermissionEntity> GetPermissions(string ctrl)
        {
            return CreateQuery().Find(o => o.CtrlName == ctrl).ToList();
        }
    }
}
