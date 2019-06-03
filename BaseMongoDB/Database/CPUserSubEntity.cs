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
    public class CPUserSubEntity : EntityBase
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string RoleID { get; set; }
        public string UserNameManager { get; set; }
        public string Email { get; set; }
        public string Pass { get; set; }
       
       
        public string Phone { get; set; }
        public bool Activity { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
    }
    public class CPUserSubService : ServiceBase<CPUserSubEntity>
    {
        public CPUserSubService(IConfiguration config) : base(config, "CPUsersSub")
        {

        }

        public CPUserSubService(IConfiguration config, string tableName) : base(config, tableName)
        {
        }

        public CPUserSubEntity GetItemByEmail(string email)
        {
            return CreateQuery().Find(o => o.Email == email)?.SingleOrDefault();
        }

        public CPUserSubEntity GetItemByUserName(string UserName)
        {
            return CreateQuery().Find(o => o.UserName == UserName)?.FirstOrDefault();
        }

        [Obsolete]
        public async Task<BaseResponse<CPUserSubEntity>> getListUserSub(SeachForm model)
        {
           

            BaseResponse<CPUserSubEntity> result = new BaseResponse<CPUserSubEntity>();
            var query = CreateQuery().Find(o=>o.UserNameManager==model.UserName && o.RoleID !="GIAOVIEN" && o.RoleID !="SINHVIEN");
           result.TotalPage = query.Count();
            query = query.Skip(model.pageSize * (model.currentPage - 1)).Limit(model.pageSize);
            await query.ToListAsync();
            result.Data = query.ToList() ;
            
            return result;

        }
    }
}
