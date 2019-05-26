using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;

namespace BasePublisherModels.Database
{
    public class ModLessonMediaEntity : EntityBase
    {
        public string LessonPartID { get; set; }
        public string NameOriginal { get; set; }
        public string File { get; set; }
        public string OriginalFile { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
    }
    public class ModLessonMediaService : ServiceBase<ModLessonMediaEntity>
    {
        public ModLessonMediaService(IConfiguration config) : base(config, "ModLessonMedias")
        {

        }
        public ModLessonMediaService(IConfiguration config, string tableName) : base(config, tableName)
        {

        }
    }
}
