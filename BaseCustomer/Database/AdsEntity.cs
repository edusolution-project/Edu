using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Driver;
using Microsoft.AspNetCore.Http;

namespace BaseCustomerEntity.Database
{
    public class AdsEntity : EntityBase
    {
        [JsonProperty("Banner")]
        public string Banner { get; set; }
        [JsonProperty("CreateDate")]
        public DateTime CreateDate { get; set; }
        [JsonProperty("PublishDate")]
        public DateTime PublishDate { get; set; }
        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }
        [JsonProperty("Code")]
        public string Code { get; set; }
        [JsonProperty("NameCampaign")]
        public string NameCampaign { get; set; }
        [JsonProperty("EndDate")]
        public DateTime EndDate { get; set; }
    }

    public class AdsService : ServiceBase<AdsEntity>
    {
        private readonly IndexService _indexService;

        public AdsService(IConfiguration config, IndexService indexService) : base(config)
        {
            _indexService = indexService;
            var indexs = new List<CreateIndexModel<AdsEntity>> { };
            Collection.Indexes.CreateManyAsync(indexs);
        }

        public void ChangeStatus(List<string> IDs, bool status)
        {
            CreateQuery().UpdateMany(t => IDs.Contains(t.ID), Builders<AdsEntity>.Update.Set(t => t.IsActive, status));
        }

        public AdsEntity GetItemByCode(string Code) => Collection.Find<AdsEntity>(x => x.Code.Equals(Code) && x.IsActive == true).FirstOrDefault();
    }

}
