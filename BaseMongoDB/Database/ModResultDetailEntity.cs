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
    public class ModResultDetailEntity : EntityBase
    {
        public string ResultID { get; set; }
        public string QuestionID { get; set; }
        public string AnswerID { get; set; }
        public string ValueAnswer { get; set; }
        public bool IsTrue { get; set; }
        public string Note { get; set; }
        public DateTime Created { get; set; }
        public DateTime Update { get; set; }
    }
    public class ModResultDetailService : ServiceBase<ModResultDetailEntity>
    {
        public ModResultDetailService(IConfiguration config) : base(config, "ModResultDetails")
        {

        }
        [Obsolete]
        public async Task<BaseResponse<ModResultDetailEntity>> getList(SeachForm model)
        {
            BaseResponse<ModResultDetailEntity> result = new BaseResponse<ModResultDetailEntity>();
            var query = CreateQuery().Find(_ => true);
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
        [Obsolete]
        public async Task<BaseResponse<ModResultDetailEntity>> getListByResult(SeachForm model,string resultID)
        {
            BaseResponse<ModResultDetailEntity> result = new BaseResponse<ModResultDetailEntity>();
            var query = CreateQuery().Find(o=>o.ResultID == resultID);
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
