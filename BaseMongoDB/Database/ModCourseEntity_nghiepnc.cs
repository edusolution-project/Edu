using Business.Dto.Form;
using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseMongoDB.Database
{
    public class ModCourseEntity_nghiepnc : EntityBase
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
    public class ModCourseService_nghiepnc : ServiceBase<ModCourseEntity_nghiepnc>
    {
        public ModCourseService_nghiepnc(IConfiguration config) : base(config, "ModCourses")
        {

        }
        public ModCourseService_nghiepnc(IConfiguration config, string tableName) : base(config, tableName)
        {

        }
        public List<ModCourseEntity_nghiepnc> getList(SeachForm seachForm)
        {
            var result = new List<ModCourseEntity_nghiepnc>();
            var query = CreateQuery().Find(o => o.GradeID == seachForm.gradleID && o.SubjectID == seachForm.subjectID &&
                                           o.ProgramID==seachForm.programID && o.IsActive==true);
              result = query.ToList();
              return result;
        }
    }
}
