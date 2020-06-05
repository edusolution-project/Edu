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
        //[JsonProperty("UID")]
        //public string UID { get; set; }
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

    public class NewsService : ServiceBase<NewsEntity>
    {
        private readonly IndexService _indexService;

        public NewsService(IConfiguration config, IndexService indexService) : base(config)
        {
            _indexService = indexService;
            var indexs = new List<CreateIndexModel<NewsEntity>> { };
            Collection.Indexes.CreateManyAsync(indexs);
        }

        //public new NewsEntity Save(NewsEntity entity)
        //{
        //    if (entity.ID == null || entity.ID == "")
        //    {
        //        entity.UID = _indexService.GetNewIndex("News").ToString();
        //        Collection.InsertOne(entity);
        //    }
        //    else
        //    {
        //        Collection.ReplaceOne(t => t.ID == entity.ID, entity);
        //    }
        //    return entity;
        //}

        public void ChangeStatus(List<string> IDs, bool status, string check)
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

        public NewsEntity GetItemByCode(string Code) => Collection.Find<NewsEntity>(x => x.Code.Equals(Code)).FirstOrDefault();
    }
}
