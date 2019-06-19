using Business.Dto.Form;
using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseMongoDB.Database
{
    public class CourseTeacherConfigEntity : EntityBase
    {
        public string CourseID { get; set; }
        public string ChapterID { get; set; }
        public string LessonID { get; set; }
        public bool IsParentCourse { get; set; } // có phải là course hay không ?
        public int TemplateType { get; set; }
        public int Point { get; set; }
        public int Timer { get; set; }
        public string CreateUser { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public int Order { get; set; }
        public Media Media { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public string TeacherID { get; set; }
        public string createdDate { get; set; }
        public string endedDate { get; set; }

        public bool IsOpen { get; set; }


    }
    public class CourseTeacherConfigService : ServiceBase<CourseTeacherConfigEntity>
    {
        public CourseTeacherConfigService(IConfiguration config) : base(config, "CourseTeacherConfig")
        {

        }

        public CourseTeacherConfigService(IConfiguration config, string tableName) : base(config, tableName)
        {
        }

        public List<CourseTeacherConfigEntity> getListByCourseID(string courseID)
        {
            var result = new List<CourseTeacherConfigEntity>();
            var query = CreateQuery().Find(o => o.CourseID == courseID);
            result = query.ToList();
            return result;

        }


    }
}
