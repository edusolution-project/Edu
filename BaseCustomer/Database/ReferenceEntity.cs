using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseCustomerEntity.Database
{
    public class ReferenceEntity : EntityBase
    {
        [JsonProperty("OwnerID")]
        public string OwnerID { get; set; }
        [JsonProperty("OwnerName")]
        public string OwnerName { get; set; }
        [JsonProperty("Title")]
        public string Title { get; set; }
        [JsonProperty("Media")]
        public Media Media { get; set; }
        [JsonProperty("Link")]
        public string Link { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }
        [JsonProperty("CreateTime")]
        public DateTime CreateTime { get; set; }
        [JsonProperty("UpdateTime")]
        public DateTime UpdateTime { get; set; }
        [JsonProperty("Downloaded")]
        public int Downloaded { get; set; }
        [JsonProperty("LastDownload")]
        public DateTime LastDownload { get; set; }
        [JsonProperty("Viewed")]
        public int Viewed { get; set; }
        [JsonProperty("LastView")]
        public DateTime LastView { get; set; }
        [JsonProperty("Range")]
        public string Range { get; set; }
        [JsonProperty("Target")]
        public string Target { get; set; }
        [JsonProperty("Image")]
        public string Image { get; set; }
    }

    public class REF_RANGE
    {
        public const string
            ALL = "all",
            TEACHER = "teacher",
            CLASS = "class",
            CLASSSUBJECT = "classsubject",
            COURSE = "course",
            STUDENT = "student", PRIVATE = "private", UNSET = "";
    }

    //public class Tag
    //{
    //    [JsonProperty("Name")]
    //    public string Name { get; set; }
    //    [JsonProperty("Value")]
    //    public string Value { get; set; }
    //}

    public class ReferenceService : ServiceBase<ReferenceEntity>
    {
        public ReferenceService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<ReferenceEntity>>
            {
                new CreateIndexModel<ReferenceEntity>(
                    new IndexKeysDefinitionBuilder<ReferenceEntity>()
                    .Ascending(t => t.OwnerID)
                    .Descending(t=> t.CreateTime)),
                new CreateIndexModel<ReferenceEntity>(
                    new IndexKeysDefinitionBuilder<ReferenceEntity>()
                    .Ascending(t => t.Range)
                    .Ascending(t=> t.Target)
                    .Descending(t=> t.CreateTime)),
                new CreateIndexModel<ReferenceEntity>(
                    new IndexKeysDefinitionBuilder<ReferenceEntity>().Text(t=> t.Title)
                )
            };
            Collection.Indexes.CreateManyAsync(indexs);
        }

        public async Task IncDownload(string ID, int increment)
        {
            await Collection.UpdateOneAsync(t => t.ID == ID, new UpdateDefinitionBuilder<ReferenceEntity>()
                .Inc(t => t.Downloaded, 1)
                .Set(t => t.LastDownload, DateTime.Now));
        }

        public async Task IncView(string ID, int increment)
        {
            await Collection.UpdateOneAsync(t => t.ID == ID, new UpdateDefinitionBuilder<ReferenceEntity>()
                .Inc(t => t.Viewed, 1)
                .Set(t => t.LastView, DateTime.Now));
        }
    }
}
