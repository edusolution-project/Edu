using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;

namespace BasePublisherModels.Database
{
    public class ModLessonExtendEntity : EntityBase
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
    public class ModLessonExtendService : ServiceBase<ModLessonExtendEntity>
    {
        public ModLessonExtendService(IConfiguration config) : base(config, "ModLessonExtends")
        {

        }
        public ModLessonExtendService(IConfiguration config, string tableName) : base(config, tableName)
        {

        }
    }
}
