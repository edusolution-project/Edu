﻿using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseCustomerEntity.Database
{
    public class CourseEntity : EntityBase
    {
        [JsonProperty("OriginID")]
        public string OriginID { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }
        [JsonProperty("Code")]
        public string Code { get; set; }
        [JsonProperty("CreateUser")]
        public string CreateUser { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("Updated")]
        public DateTime Updated { get; set; }
        [JsonProperty("GradeID")]
        public string GradeID { get; set; }
        [JsonProperty("SubjectID")]
        public string SubjectID { get; set; }
        [JsonProperty("TeacherID")]
        public string TeacherID { get; set; }
        [JsonProperty("IsAdmin")]
        public bool IsAdmin { get; set; }
        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }
        [JsonProperty("Order")]
        public int Order { get; set; }
    }
    public class CourseService : ServiceBase<CourseEntity>
    {
        public CourseService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<CourseEntity>>
            {
                //TeacherID_1_SubjectID_1_GradeID_1_IsActive_1
                new CreateIndexModel<CourseEntity>(
                    new IndexKeysDefinitionBuilder<CourseEntity>()
                    .Ascending(t=> t.TeacherID)
                    .Ascending(t => t.SubjectID)
                    .Ascending(t=> t.GradeID)
                    .Ascending(t=>t.IsActive)
                    ),
                //SubjectID_1_GradeID_1_IsActive_1
                new CreateIndexModel<CourseEntity>(
                    new IndexKeysDefinitionBuilder<CourseEntity>()
                    .Ascending(t => t.SubjectID)
                    .Ascending(t=> t.GradeID)
                    )
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }
    }
}
