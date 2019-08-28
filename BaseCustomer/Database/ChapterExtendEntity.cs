using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BaseCustomerEntity.Database
{
    public class ChapterExtendEntity : EntityBase
    {
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
        [JsonProperty("ChapterID")]
        public string ChapterID { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }
    }
    public class ChapterExtendService : ServiceBase<ChapterExtendEntity>
    {
        public ChapterExtendService(IConfiguration config) : base(config)
        {

        }

        public List<ChapterExtendEntity> Search(string ClassID = null, string ChapterID = null)
        {
            var builder = Builders<ChapterExtendEntity>.Filter;
            var filter = FilterDefinition<ChapterExtendEntity>.Empty;
            if (!string.IsNullOrEmpty(ClassID))
                filter &= builder.Eq(o => o.ClassID, ClassID);
            if (!string.IsNullOrEmpty(ChapterID))
                filter &= builder.Eq(o => o.ChapterID, ChapterID);
            return CreateQuery().Find(filter).ToList();
        }
    }
}
