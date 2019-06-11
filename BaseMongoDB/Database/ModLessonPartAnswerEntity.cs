using Business.Dto.Form;
using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace BaseMongoDB.Database
{
    public class ModLessonPartAnswerEntity : EntityBase
    {
        public string ParentID { get; set; } // chính là lessonPartID
        public string Content { get; set; }
        public bool IsCorrect { get; set; }
        public string CreateUser { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Media Media { get; set; }
        public int Order { get; set; }

    }
    public class ModLessonPartAnswerService : ServiceBase<ModLessonPartAnswerEntity>
    {
        public ModLessonPartAnswerService(IConfiguration config) : base(config, "ModLessonPartAnswers")
        {

        }
        public ModLessonPartAnswerService(IConfiguration config, string tableName) : base(config, tableName)
        {

        }
        [Obsolete]
        public async Task<BaseResponse<ModLessonPartAnswerEntity>> getList(SeachForm model)
        {
            BaseResponse<ModLessonPartAnswerEntity> result = new BaseResponse<ModLessonPartAnswerEntity>();
            var query = CreateQuery().Find(_ => true);
            result.TotalPage = query.Count();
            result.Data = await query.Skip(model.pageSize * (model.currentPage - 1)).Limit(model.pageSize).ToListAsync();
            return result;

        }
    }
}
