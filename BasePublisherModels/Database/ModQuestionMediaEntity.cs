using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;

namespace BasePublisherModels.Database
{
    public class ModQuestionMediaEntity : EntityBase
    {
        public string QuestionID { get; set; }
        public string LinkHost { get; set; }
        public string LinkFile { get; set; }
        public string OriginalName { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public int Order { get; set; }
        
    }
    public class ModQuestionMediaService : ServiceBase<ModQuestionMediaEntity>
    {
        public ModQuestionMediaService(IConfiguration config) : base(config, "ModQuestionMedias")
        {

        }
        public ModQuestionMediaService(IConfiguration config, string tableName) : base(config, tableName)
        {

        }
    }
}
