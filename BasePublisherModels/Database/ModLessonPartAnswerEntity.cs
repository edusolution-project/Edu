using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;

namespace BasePublisherModels.Database
{
    public class ModLessonPartAnswerEntity : EntityBase
    {
        public string ParentID { get; set; } // chính là lessonPartID
        public string Content { get; set; }
        public bool IsAnswer { get; set; }
        public string CreateUser { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public int Order { get; set; }
        
    }
    public class ModLessonPartAnswerService : ServiceBase<ModLessonPartAnswerEntity>
    {
        public ModLessonPartAnswerService(IConfiguration config) : base(config, "ModLessonPartAnswers")
        {

        }
        public ModLessonPartAnswerService(IConfiguration config, string tableName) : base(config, tableName)
        {

        }
    }
}
