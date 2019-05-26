using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Linq;

namespace BasePublisherModels.Database
{
    public class ModCourseEntity : EntityBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string CreateUser { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string GradeID { get; set; }
        public string SubjectID { get; set; }
        public string ProgramID { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
    }
    public class ModCourseService : ServiceBase<ModCourseEntity>
    {
        public ModCourseService(IConfiguration config) : base(config, "ModCourses")
        {

        }
        public ModCourseService(IConfiguration config, string tableName) : base(config, tableName)
        {

        }

        public object GetItemByCode(string code)
        {
            return CreateQuery().Find(o => o.Code == code)?.SingleOrDefault();
        }
    }
}
