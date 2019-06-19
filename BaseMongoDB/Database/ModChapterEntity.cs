using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace BaseMongoDB.Database
{
    public class ModChapterEntity : EntityBase
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string CourseID { get; set; }
        public string ParentID { get; set; }
        public int ParentType { get; set; }
        public string CreateUser { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
    }
    public class ModChapterService : ServiceBase<ModChapterEntity>
    {
        public ModChapterService(IConfiguration config) : base(config, "ModChapters")
        {

        }
        public ModChapterService(IConfiguration config, string tableName) : base(config, tableName)
        {

        }
        public object GetByCode(string code)
        {
            return CreateQuery().Find(o => o.Code == code)?.SingleOrDefault();
        }

        public List<ModChapterEntity> getListByCourseId(string courseId)
        {
            var result = new List<ModChapterEntity>();
            var query = CreateQuery().Find(o => o.CourseID == courseId);
            result = query.ToList();
            return result;
        }
    }

    public static class PARENT_TYPE_CODE
    {
        public const int COURSE = 1, CHAPTER = 2;
    }
}
