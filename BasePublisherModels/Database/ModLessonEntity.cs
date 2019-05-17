using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;

namespace BasePublisherModels.Database
{
    public class ModLessonEntity : EntityBase
    {
        public string Content { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public int Timer { get; set; } = 0;
        public double Point { get; set; }
        public string ChapterID { get; set; }
        public string CourseID { get; set; }
        public int TemplateType { get; set; } //0 tự luận . 1// trắc nghiệm , 2 là trắc nghiệm, 3 lầ trắc nghiệm
        public string CreateUser { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }

    }
    public class ModLessonService : ServiceBase<ModLessonEntity>
    {
        public ModLessonService(IConfiguration config) : base(config, "ModLessons")
        {

        }
        public ModLessonService(IConfiguration config, string tableName) : base(config, tableName)
        {

        }
        public object GetItemByCode(string code)
        {
            return CreateQuery().Find(o => o.Code == code)?.SingleOrDefault();
        }
    }
}
