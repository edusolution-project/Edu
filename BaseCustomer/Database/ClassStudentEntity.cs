﻿using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseCustomerEntity.Database
{
    public class ClassStudentEntity : EntityBase
    {
        [JsonProperty("StudentID")]
        public string StudentID { get; set; }
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
    }

    public class ClassStudentService : ServiceBase<ClassStudentEntity>
    {
        public ClassStudentService(IConfiguration configuration) : base(configuration)
        {

            var indexs = new List<CreateIndexModel<ClassStudentEntity>>
            {
                //ClassID_1
                new CreateIndexModel<ClassStudentEntity>(
                    new IndexKeysDefinitionBuilder<ClassStudentEntity>()
                    .Ascending(t => t.ClassID)),
                //StudentID_1
                new CreateIndexModel<ClassStudentEntity>(
                    new IndexKeysDefinitionBuilder<ClassStudentEntity>()
                    .Ascending(t => t.StudentID)),
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public List<ClassStudentEntity> GetClassStudents(string ClassID)
        {
            return Collection.Find(t => t.ClassID == ClassID).ToList();
        }

        public ClassStudentEntity GetClassStudent(string ClassID, string StudentID)
        {
            return Collection.Find(t => t.ClassID == ClassID && t.StudentID == StudentID).SingleOrDefault();
        }

        public List<string> GetStudentClasses(string StudentID)
        {
            return Collection.Distinct(t => t.ClassID, t => t.StudentID == StudentID).ToList();
        }

        public long RemoveClass(string ClassID)
        {
            return Collection.DeleteMany(t => t.ClassID == ClassID).DeletedCount;
        }
    }
}
