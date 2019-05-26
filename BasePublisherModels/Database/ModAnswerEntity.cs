using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using System;

namespace BasePublisherModels.Database
{
    public class ModAnswerEntity : EntityBase
    {
        public string LessonPartID { get; set; } // chính là lessonPartID
        public string Content { get; set; }
        public string QuestionID { get; set; }
        public int TemplateType { get; set; }
        public bool IsAnswer { get; set; }
        public string CreateUser { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public int Order { get; set; }
        
    }
    public class ModAnswerService : ServiceBase<ModAnswerEntity>
    {
        public ModAnswerService(IConfiguration config) : base(config, "ModAnswers")
        {

        }
        public ModAnswerService(IConfiguration config, string tableName) : base(config, tableName)
        {

        }
    }
}
