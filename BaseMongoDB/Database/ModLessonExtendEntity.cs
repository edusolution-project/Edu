using Business.Dto.Form;
using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace BaseMongoDB.Database
{
    public class ModLessonExtendEntity : EntityBase
    {
        public string LessonPartID { get; set; }

        public string NameOriginal { get; set; }
        public string File { get; set; }
        public string OriginalFile { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
    }
    public class ModLessonExtendService : ServiceBase<ModLessonExtendEntity>
    {
        public ModLessonExtendService(IConfiguration config) : base(config, "ModLessonExtends")
        {

        }
        public ModLessonExtendService(IConfiguration config, string tableName) : base(config, tableName)
        {

        }
        [Obsolete]
        public async Task<BaseResponse<ModLessonExtendEntity>> getList(SeachForm model)
        {
            BaseResponse<ModLessonExtendEntity> result = new BaseResponse<ModLessonExtendEntity>();
            var query = CreateQuery().Find(_ => true);
            result.TotalPage = query.Count();
            result.Data = await query.Skip(model.pageSize * (model.currentPage - 1)).Limit(model.pageSize).ToListAsync();
            return result;

        }
    }
}
