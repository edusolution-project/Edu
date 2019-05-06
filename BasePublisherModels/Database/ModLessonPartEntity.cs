﻿using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using System;

namespace BasePublisherModels.Database
{
    public class ModLessonPartEntity : EntityBase
    {
        public string LessionID { get; set; }
        public string Name { get; set; }
        public bool IsAnswer { get; set; }
        public string Media { get; set; }
        public string CreateUser { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public int Order { get; set; }
    }
    public class ModLessonPartService : ServiceBase<ModLessonPartEntity>
    {
        public ModLessonPartService(IConfiguration config) : base(config, "ModLessonParts")
        {

        }
        public ModLessonPartService(IConfiguration config, string tableName) : base(config, tableName)
        {

        }
    }
}
