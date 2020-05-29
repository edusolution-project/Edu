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
    public class NewsEntity : EntityBase
    {
        [JsonProperty("CategoryID")]
        public string CategoryID { get; set; }
        [JsonProperty("Title")]
        public string Title { get; set; }
        [JsonProperty("Code")]
        public string Code { get; set; }
        [JsonProperty("Thumbnail")]
        public string Thumbnail { get; set; }
        [JsonProperty("Summary")]
        public string Summary { get; set; }
        [JsonProperty("Content")]
        public string Content { get; set; }
        [JsonProperty("IsHot")]
        public bool IsHot { get; set; }
        [JsonProperty("IsTop")]
        public bool IsTop { get; set; }
        [JsonProperty("CreateDate")]
        public DateTime CreateDate { get; set; }
        [JsonProperty("LastEdit")]
        public DateTime LastEdit { get; set; }
        [JsonProperty("PublishDate")]
        public DateTime PublishDate { get; set; }

    }

    public class NewsService:ServiceBase<NewsEntity>
    {
        public NewsService(IConfiguration config) :base(config)
        {
            var indexs = new List<CreateIndexModel<NewsEntity>> { };
            Collection.Indexes.CreateManyAsync(indexs);
        }

        public void ChangeStatus(List<string> IDs, bool status,string check)
        {
            if (check.Equals("IsTop"))
            {
                CreateQuery().UpdateMany(t => IDs.Contains(t.ID), Builders<NewsEntity>.Update.Set(t => t.IsTop, status));
            }
            if (check.Equals("IsHot"))
            {
                CreateQuery().UpdateMany(t => IDs.Contains(t.ID), Builders<NewsEntity>.Update.Set(t => t.IsHot, status));
            }
        }
    }
}
