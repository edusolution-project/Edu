using CoreMongoDB.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Business.Dto.Form;
using MongoDB.Driver;

namespace BaseMongoDB.Database
{
    public class ModResultEntity : EntityBase
    {
        public string UserDoID { get; set; } // người làm bài
        public string UserCheckID { get; set; } // người chấm bài +  nếu toàn bộ là trắc nghiệm trường này  = "0"
        public string LessonID { get; set; }
        public DateTime TimeStartDoing { get; set; }
        public DateTime TimeFinished { get; set; }
        public int Status { get; set; } // 0 đang làm bài , 1 đã hoàn thành chờ chấm , 2 đã chấm bài
        public string Note { get; set; }
        public long NumberDo { get; set; }
        public DateTime Created { get; set; }
        public DateTime Update { get; set; }
    }
    public class ModResultService : ServiceBase<ModResultEntity>
    {
        public ModResultService(IConfiguration config) : base(config, "ModResults")
        {

        }
        [Obsolete]
        public async Task<BaseResponse<ModResultEntity>> getListByStudentID(SeachForm model)
        {
            BaseResponse<ModResultEntity> result = new BaseResponse<ModResultEntity>();
            var query = CreateQuery().Find(o => o.UserDoID == model.UserName);
            result.TotalPage = query.Count();
            try
            {
                result.Data = await query.Skip(model.pageSize * (model.currentPage - 1)).Limit(model.pageSize).ToListAsync();
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
            }
            return result;

        }
        [Obsolete]
        public async Task<BaseResponse<ModResultEntity>> getList(SeachForm model)
        {
            BaseResponse<ModResultEntity> result = new BaseResponse<ModResultEntity>();
            var query = CreateQuery().Find(_=>true);
            result.TotalPage = query.Count();
            try
            {
                await query.Skip(model.pageSize * (model.currentPage - 1)).Limit(model.pageSize).ToListAsync();
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
            }
            result.Data = query.ToList();
            return result;

        }
    }
}
