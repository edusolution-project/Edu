using CoreMongoDB.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Business.Dto.Form;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace BaseMongoDB.Database
{
    public class ModPointBoardEntity : EntityBase
    {
        public string LessonID { get; set; }
        public string TeacherID { get; set; }
        public string StudentID { get; set; }
        public int Coefficient { get; set; } // hệ số  1 , 2 , 3 ...
        public double PointNumber { get; set; } // 9.0 , 7.6 
        public string PointString { get; set; } // A B C D E F
        public string Note { get; set; } // ghi chú nếu cần.
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
    public class ModPointBoardService : ServiceBase<ModPointBoardEntity>
    {
        public ModPointBoardService(IConfiguration config) : base(config, "ModPointBoards")
        {

        }
        [Obsolete]
        public async Task<BaseResponse<ModPointBoardEntity>> getList(SeachForm model)
        {
            BaseResponse<ModPointBoardEntity> result = new BaseResponse<ModPointBoardEntity>();
            var query = CreateQuery().Find(_ => true);
            result.TotalPage = query.Count();
            result.Data = await query.Skip(model.pageSize * (model.currentPage - 1)).Limit(model.pageSize).ToListAsync();
            return result;

        }
    }
}
