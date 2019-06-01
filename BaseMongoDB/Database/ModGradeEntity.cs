﻿using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;

namespace BaseMongoDB.Database
{
    public class ModGradeEntity : EntityBase
    {

        public string Name { get; set; }
        public string Code { get; set; }
        public string SubjectID { get; set; }
        public string ParentID { get; set; }
        public string CreateUser { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
    }
    public class ModGradeService : ServiceBase<ModGradeEntity>
    {
        public ModGradeService(IConfiguration config) : base(config, "ModGrades")
        {

        }
        public ModGradeService(IConfiguration config, string tableName) : base(config, tableName)
        {

        }

        public object GetItemByCode(string code)
        {
            return CreateQuery().Find(o => o.Code == code)?.SingleOrDefault();
        }
    }
}
