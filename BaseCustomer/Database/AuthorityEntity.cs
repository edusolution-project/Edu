using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace BaseCustomerEntity.Database
{
    public class AuthorityEntity : EntityBase
    {
        [JsonProperty("Description")]
        public string Description { get; set; }
        [JsonProperty("CtrlName")]
        public string CtrlName { get; set; }
        [JsonProperty("ActName")]
        public string ActName { get; set; }
        [JsonProperty("Type")]
        public AuthorityType Type { get; set; }
    }

    public class AuthorityService : ServiceBase<AuthorityEntity>
    {
        public AuthorityService(IConfiguration config) : base(config)
        {

        }
        private bool IsExist(string ctrlName,string actName,AuthorityType type, out AuthorityEntity item)
        {
            item = CreateQuery().Find(o => o.CtrlName == ctrlName && o.ActName == actName && o.Type == type)?.FirstOrDefault();
            return  item != null;
        }
        public AuthorityEntity CreateView(string des,string ctrlName, string actName)
        {
            AuthorityType type = AuthorityType.VIEW;
            return Create(des,ctrlName,actName,type);
        }
        public AuthorityEntity CreateAdd(string des, string ctrlName, string actName)
        {
            AuthorityType type = AuthorityType.ADD;
            return Create(des, ctrlName, actName, type);
        }
        public AuthorityEntity CreateUpdate(string des, string ctrlName, string actName)
        {
            AuthorityType type = AuthorityType.UPDATE;
            return Create(des, ctrlName, actName, type);
        }
        public AuthorityEntity CreateDelete(string des, string ctrlName, string actName)
        {
            AuthorityType type = AuthorityType.DELETE;
            return Create(des, ctrlName, actName, type);
        }
        public AuthorityEntity CreateImport(string des, string ctrlName, string actName)
        {
            AuthorityType type = AuthorityType.IMPORT;
            return Create(des, ctrlName, actName, type);
        }
        public AuthorityEntity CreateExport(string des, string ctrlName, string actName)
        {
            AuthorityType type = AuthorityType.EXPORT;
            return Create(des, ctrlName, actName, type);
        }
        public AuthorityEntity CreateApproved(string des, string ctrlName, string actName)
        {
            AuthorityType type = AuthorityType.APPROVED;
            return Create(des, ctrlName, actName, type);
        }
        private AuthorityEntity Create(string des, string ctrlName, string actName, AuthorityType type)
        {
            if (IsExist(ctrlName, actName, type, out AuthorityEntity item))
            {
                item.Description = des;
            }
            else
            {
                item = new AuthorityEntity
                {
                    ActName = actName,
                    CtrlName = ctrlName,
                    Description = des,
                    Type = type
                };
            }
            CreateOrUpdate(item);
            return item;
        }

    }
    public enum AuthorityType
    {
        VIEW,
        UPDATE,
        ADD,
        DELETE,
        IMPORT,
        EXPORT,
        APPROVED
    }
}
