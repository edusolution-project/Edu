using Core_v2.Interfaces;
using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class RoleEntity: EntityBase
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Code")]
        public string Code { get; set; }
        [JsonProperty("Type")]
        public string Type { get; set; }
        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }
        [JsonProperty("UserCreate")]
        public string UserCreate { get; set; }
        [JsonProperty("CreateDate")]
        public DateTime CreateDate { get; set; }
        [JsonProperty("ParentID")]
        public string ParentID { get; set; }
        [JsonProperty("Level")]
        public int Level { get; set; }
        [JsonProperty("Group")]
        public int Group { get; set; }
    }
    public class RoleService : ServiceBase<RoleEntity>
    {
        private readonly ILog _log;
        public RoleService(IConfiguration configuration) : base(configuration)
        {
            _log = new Log(configuration);
        }
        public RoleEntity GetItemByCode(string code)
        {
            var listItem = Collection.Find(o => !string.IsNullOrEmpty(o.Code) && o.Code == code)?.ToList();
            return listItem != null && listItem.Count > 0 ? listItem.First() : null;
        }
        public bool RemoveRole(string id)
        {
            var oldItem = GetItemByID(id);
            if(oldItem != null)
            {
                oldItem.IsActive = false;
                var filter = Builders<RoleEntity>.Filter.Eq(o => o.ID, id);
                var update = Builders<RoleEntity>.Update.Set(o => o.IsActive, false);
                CreateQuery().UpdateOne(filter, update);
                return true;
            }
            return false;

        }
        public bool CreateNewRole(RoleEntity item)
        {
            try
            {
                if (string.IsNullOrEmpty(item.ID))
                {
                    if (!string.IsNullOrEmpty(item.ParentID))
                    {
                        var parentItem = GetItemByCode(item.ParentID);
                        if (parentItem != null)
                        {
                            item.Level = parentItem.Level + 1;
                            CreateOrUpdate(item);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    CreateOrUpdate(item);
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                _log.Error("CreateNewRole", ex);
                return false;
            }
        }
        public bool UpdateRole(RoleEntity item)
        {
            try
            {
                if (!string.IsNullOrEmpty(item.ID))
                {
                    var oldItem = GetItemByID(item.ID);
                    if(oldItem != null)
                    {
                        oldItem.Name = item.Name;
                        CreateOrUpdate(oldItem);
                        return true;
                    }
                }
                return false;
            }
            catch(Exception ex)
            {
                _log.Error("UpdateRole", ex);
                return false;
            }
        }
    }
}
