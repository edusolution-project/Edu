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
    public class CourseEntity : EntityBase
    {
        public string CourseID { get; set; }
        public string SubjectID { get; set; }
        public string GradeID { get; set; }
        public string ProgramID { get; set; }
        public string Description { get; set; }
        public string TeacherID { get; set; }
        public List<string> StudentID { get; set; }
        
        public bool Activity { get; set; }
        public string UserCreate { get; set; }
        public string UserNameManager { get; set; }
        public string CreatedDate { get; set; }
        public string EndedDate { get; set; }


    }
    public class CourseService : ServiceBase<CourseEntity>
    {
        public CourseService(IConfiguration config) : base(config, "Course")
        {

        }

        public CourseService(IConfiguration config, string tableName) : base(config, tableName)
        {
        }



        //public TeacherEntity GetItemByUserName(string UserName)
        //{
        //    return CreateQuery().Find(o => o.TeacherId == UserName)?.FirstOrDefault();
        //}

        [Obsolete]
        public async Task<BaseResponse<CourseEntity>> getList(SeachForm model)
        {
            BaseResponse<CourseEntity> result = new BaseResponse<CourseEntity>();
            var query = CreateQuery().Find(o => o.UserCreate == model.UserName);
            result.TotalPage = query.Count();
            try
            { 
            await query.Skip(model.pageSize * (model.currentPage - 1)).Limit(model.pageSize).ToListAsync();
        }
            catch(Exception ex)
            {
                string s = ex.ToString();
            }
            result.Data = query.ToList();
            return result;

        }

        //public List<TeacherEntity> getListByUserNameManager(string userNameManager)
        //{
        //    var result = new List<TeacherEntity>();
        //    var query = CreateQuery().Find(o => o.UserNameManager == userNameManager);
        //    result = query.ToList();
        //    return result;

        //}
    }
}
