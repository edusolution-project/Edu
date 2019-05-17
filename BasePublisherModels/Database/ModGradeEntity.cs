using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BasePublisherModels.Database
{
    public class ModGradeEntity : EntityBase
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string ParentID { get; set; }
        public string CreateUser { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
    }
    public class ModGradeService : ServiceBase<ModGradeEntity>
    {
        public ModGradeService(IConfiguration config) : base(config, "ModGrades")
        {

        }
        public ModGradeService(IConfiguration config, string tableName) : base(config, tableName)
        {

        }


        public new ModGradeEntity GetByID(string id)
        {
            return base.Find(true, o => o.ID == id).SingleOrDefault();
        }

        public ModGradeEntity GetItemByCode(string code)
        {
            return CreateQuery().Find(o => o.Code == code)?.SingleOrDefault();
        }

        public List<ModGradeEntity> GetItemsByParentID(string parentid)
        {
            return string.IsNullOrEmpty(parentid)
                ? base.GetAll().ToList()
                : CreateQuery().Find(o => o.ParentID == parentid).ToList();
        }

        public List<ModGradeEntity> GetRootItems()
        {
            return CreateQuery().Find(o => o.IsActive && (string.IsNullOrEmpty(o.ParentID) || o.ParentID.Equals("0"))).ToList();
        }


        public long CountSubGradeByID(string id)
        {
            return CreateQuery().CountDocuments(o => o.ParentID == id);
        }

    }
}
