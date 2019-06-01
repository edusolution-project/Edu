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
    public class TeacherEntity : EntityBase
    {
        public string TeacherId { get; set; }
        public string FullName { get; set; }
        public string UserNameManager { get; set; }
        public string DateBorn { get; set; }
        public string Technique { get; set; }
        
        public bool Activity { get; set; }
        public string UserCreate { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
    }
    public class TeacherService : ServiceBase<TeacherEntity>
    {
        public TeacherService(IConfiguration config) : base(config, "Teacher")
        {

        }

        public TeacherService(IConfiguration config, string tableName) : base(config, tableName)
        {
        }

        

        public TeacherEntity GetItemByUserName(string UserName)
        {
            return CreateQuery().Find(o => o.TeacherId == UserName)?.FirstOrDefault();
        }

        public async Task<BaseResponse<TeacherEntity>> getList(SeachForm model)
        {
            BaseResponse<TeacherEntity> result = new BaseResponse<TeacherEntity>();
            var query = CreateQuery().Find(o=>o.UserNameManager==model.UserName);
            result.TotalPage = query.Count();
            await query.Skip(model.pageSize * (model.currentPage-1)).Limit(model.pageSize).ToListAsync();
            result.Data = query.ToList() ;
            return result;

        }

        public List<TeacherEntity> getListByUserNameManager(string userNameManager)
        {
            var result = new List<TeacherEntity>();
            var query = CreateQuery().Find(o => o.UserNameManager == userNameManager);
            result = query.ToList();
            return result;
        }

        public List<TeacherEntity> getListAll()
        {
            var result = new List<TeacherEntity>();
            var query = CreateQuery().Find(o => o.Activity == true);
            result = query.ToList();
            return result;
        }
    }
}
