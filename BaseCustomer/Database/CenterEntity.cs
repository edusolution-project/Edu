using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseCustomerEntity.Database
{
    public class CenterEntity : EntityBase
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Code")]
        public string Code { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }
        [JsonProperty("Status")]
        public bool Status { get; set; }
        [JsonProperty("Limit")]
        public long Limit { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("StartDate")]
        public DateTime StartDate { get; set; }
        [JsonProperty("ExpireDate")]
        public DateTime ExpireDate { get; set; }
        [JsonProperty("IsDefault")]
        public bool IsDefault { get; set; } = false;
    }

    public class CenterService : ServiceBase<CenterEntity>
    {
        public CenterService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<CenterEntity>>
            {

            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public CenterEntity GetItemByCode(string code)
        {
            var listData = CreateQuery().Find(o => o.Code == code)?.ToList();
            if (listData == null || listData.Count == 0) return null;
            return listData?.FirstOrDefault();
        }
        public void ChangeStatus(List<string> IDs, bool status)
        {
            CreateQuery().UpdateMany(t => IDs.Contains(t.ID), Builders<CenterEntity>.Update.Set(t => t.Status, status));
        }

        public CenterEntity GetDefault()
        {
            var listItem = CreateQuery().Find(o => o.IsDefault == true)?.ToList();
            if (listItem != null && listItem.Count > 0) return listItem?.FirstOrDefault();
            return CreateDefault();
        }

        private CenterEntity CreateDefault()
        {
            var item = new CenterEntity()
            {
                Name = "Eduso",
                Code = "eduso",
                Created = DateTime.Now,
                Description = "Default",
                ExpireDate = DateTime.Now.AddDays(365),
                StartDate = DateTime.Now,
                IsDefault = true,
                Limit = 0,
                Status = true
            };
            Save(item);

            return item;
        }
    }
}
