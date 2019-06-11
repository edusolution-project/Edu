using Business.Dto.Form;
using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace BaseMongoDB.Database
{
    public class ModLessonEntity : EntityBase
    {
        public string CourseID { get; set; }
        public string ChapterID { get; set; }
        public bool IsParentCourse { get; set; } // có phải là course hay không ?
        public int TemplateType { get; set; }
        public int Point { get; set; }
        public int Timer { get; set; }
        public string CreateUser { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public int Order { get; set; }
        public Media Media { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

    }
    public class ModLessonService : ServiceBase<ModLessonEntity>
    {
        public ModLessonService(IConfiguration config) : base(config, "ModLessons")
        {

        }
        public ModLessonService(IConfiguration config, string tableName) : base(config, tableName)
        {

        }
        [Obsolete]
        public async Task<BaseResponse<ModLessonEntity>> getList(SeachForm model)
        {
            BaseResponse<ModLessonEntity> result = new BaseResponse<ModLessonEntity>();
            var query = CreateQuery().Find(_=>true);
            result.TotalPage = query.Count();
            result.Data = await query.Skip(model.pageSize * (model.currentPage - 1)).Limit(model.pageSize).ToListAsync();
            return result;

        }
    }
}
