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
        public string StudentId { get; set; }
        
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string UserNameManager { get; set; }
        public string DateBorn { get; set; }
        public string Technique { get; set; }
        public string Classes { get; set; }

        public bool Activity { get; set; }
        public string UserCreate { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
    }
    public class StudentService : ServiceBase<StudentEntity>
    {
        public StudentService(IConfiguration config) : base(config, "Student")
        {

        }

        public StudentService(IConfiguration config, string tableName) : base(config, tableName)
        {
        }

        public async Task<BaseResponse<StudentEntity>> getList(SeachForm model)
        {
            try
            {
                BaseResponse<StudentEntity> result = new BaseResponse<StudentEntity>();
                var query = CreateQuery().Find(o => o.UserNameManager == model.UserName);
                result.TotalPage = query.Count();
                await query.Skip(model.pageSize * (model.currentPage - 1)).Limit(model.pageSize).ToListAsync();
                result.Data = query.ToList();
                return result;


            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            return null;
        }
        public List<StudentEntity> getListALL()
        {
            var result = new List<StudentEntity>();
            var query = CreateQuery().Find(o => o.Activity == true);
            result = query.ToList();
            return result;
        }


    }
}
