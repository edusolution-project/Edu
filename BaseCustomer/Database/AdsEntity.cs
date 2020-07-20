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
    public class QCEntity : EntityBase
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

    public class QCService : ServiceBase<QCEntity>
    {
        private readonly IndexService _indexService;

        public QCService(IConfiguration config, IndexService indexService) : base(config)
        {
            _indexService = indexService;
            var indexs = new List<CreateIndexModel<QCEntity>> { };
            Collection.Indexes.CreateManyAsync(indexs);
        }

        public void ChangeStatus(List<string> IDs, bool status)
        {
            CreateQuery().UpdateMany(t => IDs.Contains(t.ID), Builders<QCEntity>.Update.Set(t => t.IsActive, status));
        }
    }

}
