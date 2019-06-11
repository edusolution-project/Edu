using Business.Dto.Form;
using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseMongoDB.Database
{
    public class ModLessonPartEntity : EntityBase
    {
        public string ParentID { get; set; } // chính là lessonID
        public string Title { get; set; }
        public int Timer { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public bool IsExam { get; set; }
        public double Point { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public int Order { get; set; }
        public Media Media { get; set; }
    }

    public class ModLessonPartService : ServiceBase<ModLessonPartEntity>
    {
        public ModLessonPartService(IConfiguration config) : base(config, "ModLessonParts")
        {

        }
        public ModLessonPartService(IConfiguration config, string tableName) : base(config, tableName)
        {

        }
        [Obsolete]
        public async Task<BaseResponse<ModLessonPartEntity>> getList(SeachForm model)
        {
            BaseResponse<ModLessonPartEntity> result = new BaseResponse<ModLessonPartEntity>();
            var query = CreateQuery().Find(_ => true);
            result.TotalPage = query.Count();
            result.Data = await query.Skip(model.pageSize * (model.currentPage - 1)).Limit(model.pageSize).ToListAsync();
            return result;

        }
    }
}
