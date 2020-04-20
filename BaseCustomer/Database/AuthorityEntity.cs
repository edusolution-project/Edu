using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;

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
        [JsonProperty("Area")]
        public string Area { get; set; }
    }

    public class AuthorityService : ServiceBase<AuthorityEntity>
    {
        public AuthorityService(IConfiguration config) : base(config)
        {

        }
        private bool IsExist(string area, string ctrlName,string actName,AuthorityType type, out AuthorityEntity item)
        {
            item = CreateQuery().Find(o => o.Area == area && o.CtrlName == ctrlName && o.ActName == actName && o.Type == type)?.FirstOrDefault();
            return  item != null ;
        }

        public bool IsExist(string area,string ctrlName, string actName, out AuthorityEntity item)
        {
            try
            {
                item = CreateQuery().Find(o => o.Area == area && o.CtrlName == ctrlName && o.ActName == actName)?.FirstOrDefault();
                return item != null;
            }
            catch(Exception ex)
            {
                item = null;
                return false;
            }
        }

        public AuthorityEntity CreateView(string area, string des, string ctrlName, string actName)
        {
            AuthorityType type = AuthorityType.VIEW;
            return Create(area, des, ctrlName, actName, type);
        }
        public AuthorityEntity CreateAdd(string area, string des, string ctrlName, string actName)
        {
            AuthorityType type = AuthorityType.ADD;
            return Create(area, des, ctrlName, actName, type);
        }
        public AuthorityEntity CreateUpdate(string area, string des, string ctrlName, string actName)
        {
            AuthorityType type = AuthorityType.UPDATE;
            return Create(area, des, ctrlName, actName, type);
        }
        public AuthorityEntity CreateDelete(string area, string des, string ctrlName, string actName)
        {
            AuthorityType type = AuthorityType.DELETE;
            return Create(area, des, ctrlName, actName, type);
        }
        public AuthorityEntity CreateImport(string area, string des, string ctrlName, string actName)
        {
            AuthorityType type = AuthorityType.IMPORT;
            return Create(area, des, ctrlName, actName, type);
        }
        public AuthorityEntity CreateExport(string area, string des, string ctrlName, string actName)
        {
            AuthorityType type = AuthorityType.EXPORT;
            return Create(area, des, ctrlName, actName, type);
        }
        public AuthorityEntity CreateApproved(string area, string des, string ctrlName, string actName)
        {
            AuthorityType type = AuthorityType.APPROVED;
            return Create(area,des, ctrlName, actName, type);
        }
        public AuthorityEntity CreateOther(string area, string des, string ctrlName, string actName)
        {
            AuthorityType type = AuthorityType.OTHER;
            return Create(area, des, ctrlName, actName, type);
        }
        private AuthorityEntity Create(string area,string des, string ctrlName, string actName, AuthorityType type)
        {
            if (IsExist(area,ctrlName, actName, type, out AuthorityEntity item))
            {
                item.Description = des;
                item.Area = area;
                item.Type = type;
            }
            else
            {
                item = new AuthorityEntity
                {
                    Area = area,
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
        APPROVED,
        OTHER,
        UNSET
    }
}
