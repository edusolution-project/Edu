using Business.Dto.Form;
using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace BaseMongoDB.Database
{
    public class ModLessonPartQuestionEntity : EntityBase
    {
        public string Content { get; set; }
        public string CreateUser { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public double Point { get; set; }
        public int Order { get; set; }
        public string Description { get; set; }
        public Media Media { get; set; }
        public string ParentID { get; set; }

    }
    public class ModLessonPartQuestionService : ServiceBase<ModLessonPartQuestionEntity>
    {
        public ModLessonPartQuestionService(IConfiguration config) : base(config, "ModLessonPartQuestions")
        {

        }
        public ModLessonPartQuestionService(IConfiguration config, string tableName) : base(config, tableName)
        {

        }
        [Obsolete]
        public async Task<BaseResponse<ModLessonPartQuestionEntity>> getList(SeachForm model)
        {
            BaseResponse<ModLessonPartQuestionEntity> result = new BaseResponse<ModLessonPartQuestionEntity>();
            var query = CreateQuery().Find(_ => true);
            result.TotalPage = query.Count();
            result.Data = await query.Skip(model.pageSize * (model.currentPage - 1)).Limit(model.pageSize).ToListAsync();
            return result;

        }
    }
}
