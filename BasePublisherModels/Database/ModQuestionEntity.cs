using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;

namespace BasePublisherModels.Database
{
    public class ModQuestionEntity : EntityBase
    {
        public string LessonPartID { get; set; }
        public string Title { get; set; }
        public int TemplateType { get; set; }
        public int Point { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public int Order { get; set; }
        
    }
    public class ModQuestionService : ServiceBase<ModQuestionEntity>
    {
        public ModQuestionService(IConfiguration config) : base(config, "ModQuestions")
        {

        }
        public ModQuestionService(IConfiguration config, string tableName) : base(config, tableName)
        {

        }
    }
}
