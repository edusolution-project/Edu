using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;

namespace BasePublisherModels.Database
{
    public class ModLessonEntity : EntityBase
    {
        public string CourseID { get; set; }
        public string ChapterID { get; set; }
        public bool IsParentCourse { get; set; } // có phải là course hay không ?
        public string CreateUser { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

    }
    public class ModLessonService : ServiceBase<ModLessonEntity>
    {
        public ModLessonService(IConfiguration config) : base(config, "ModLessons")
        {

        }
        public ModLessonService(IConfiguration config, string tableName) : base(config, tableName)
        {

        }
    }
}
