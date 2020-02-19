﻿using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BaseCustomerEntity.Database
{
    public class ChapterEntity : EntityBase
    {
        [JsonProperty("OriginID")]
        public string OriginID { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Code")]
        public string Code { get; set; }
        [JsonProperty("CourseID")]
        public string CourseID { get; set; }
        [JsonProperty("ParentID")]
        public string ParentID { get; set; }
        [JsonProperty("ParentType")]
        public int ParentType { get; set; }
        [JsonProperty("CreateUser")]
        public string CreateUser { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("Updated")]
        public DateTime Updated { get; set; }
        [JsonProperty("IsAdmin")]
        public bool IsAdmin { get; set; }
        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }
        [JsonProperty("Order")]
        public int Order { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("TotalLesssons")]
        public long TotalLesssons { get; set; }
    }
    public class ChapterService : ServiceBase<ChapterEntity>
    {
        public ChapterService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<ChapterEntity>>
            {
                //SubjectID_1_ParentID_1
                new CreateIndexModel<ChapterEntity>(
                    new IndexKeysDefinitionBuilder<ChapterEntity>()
                    .Ascending(t => t.CourseID)
                    .Ascending(t=> t.ParentID)),
                //ParentID_1
                new CreateIndexModel<ChapterEntity>(
                    new IndexKeysDefinitionBuilder<ChapterEntity>()
                    .Ascending(t=> t.ParentID))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }
        public object GetByCode(string code)
        {
            return CreateQuery().Find(o => o.Code == code)?.SingleOrDefault();
        }
    }
}
