﻿using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BaseCustomerEntity.Database
{
    public class CloneLessonPartEntity : LessonPartEntity
    {
        [JsonProperty("TeacherID")]
        public string TeacherID { get; set; }

        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
        [JsonProperty("ClassSubjectID")]
        public string ClassSubjectID { get; set; }

    }

    public class CloneLessonPartService : ServiceBase<CloneLessonPartEntity>
    {
        private IConfiguration config;
        private string dbName;

        public CloneLessonPartService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<CloneLessonPartEntity>>
            {
                //ClassID_1_ParentID_1
                new CreateIndexModel<CloneLessonPartEntity>(
                    new IndexKeysDefinitionBuilder<CloneLessonPartEntity>()
                    .Ascending(t => t.ClassID).Ascending(t=> t.ParentID)),
                //ParentID_1
                new CreateIndexModel<CloneLessonPartEntity>(
                    new IndexKeysDefinitionBuilder<CloneLessonPartEntity>()
                    .Ascending(t=> t.ParentID))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public CloneLessonPartService(IConfiguration config, string dbName) : base(config, dbName)
        {
        }

        public IEnumerable<CloneLessonPartEntity> GetByLessonID(string LessonID)
        {
            return CreateQuery().Find(o => o.ParentID == LessonID)
                                .SortBy(q => q.Order)
                                .ThenBy(q => q.ID).ToEnumerable();
        }

        public IEnumerable<CloneLessonPartEntity> GetByIDs(List<String> IDs)
        {
            return Collection.Find(x => IDs.Contains(x.ID)).ToEnumerable();
        }

        public List<String> GetLessonIDByTypeEssay(String ClassID, List<string> ClassSbjIDs)
        {
            return CreateQuery().Find(x=>x.ClassID == ClassID && ClassSbjIDs.Contains(x.ClassSubjectID) && x.Type == "ESSAY").Project(x=>x.ParentID).ToList();
        }

        public IEnumerable<CloneLessonPartEntity> GetItemsByLessonIDs(List<String> lessonIDs)
        {
            return CreateQuery().Find(o => lessonIDs.Contains(o.ParentID))
                                .SortBy(q => q.Order)
                                .ThenBy(q => q.ID).ToEnumerable();
        }

        public IEnumerable<CloneLessonPartEntity> GetItemsByLessonIDs_TypeQuiz(List<string> lessonIDs, List<string> quizType)
        {
            return CreateQuery().Find(o => lessonIDs.Contains(o.ParentID) && quizType.Contains(o.Type))
                                .SortBy(q => q.Order)
                                .ThenBy(q => q.ID).ToEnumerable();
        }
    }
}
