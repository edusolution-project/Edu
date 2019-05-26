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
    public class StudentEntity : EntityBase
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string UserNameManager { get; set; }
        public string DateBorn { get; set; }
        public string Technique { get; set; }

        public bool Activity { get; set; }
        public string UserCreate { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
    }
    public class StudentEntityService : ServiceBase<StudentEntity>
    {
        public StudentEntityService(IConfiguration config) : base(config, "Student")
        {

        }

        public StudentEntityService(IConfiguration config, string tableName) : base(config, tableName)
        {
        }



        public StudentEntity GetItemByUserName(string UserName)
        {
            return CreateQuery().Find(o => o.UserName == UserName)?.FirstOrDefault();
        }

        public BaseResponse<StudentEntity> getListUserSub(SeachForm model)
        {
            BaseResponse<StudentEntity> result = new BaseResponse<StudentEntity>();
            var query = CreateQuery().Find(o => o.UserNameManager == model.UserName).ToList();
            result.TotalPage = query.Count();
            query = query.Skip(model.pageSize * (model.currentPage - 1)).Take(model.pageSize).ToList();
            result.Data = query;
            return result;

        }
    }
}
