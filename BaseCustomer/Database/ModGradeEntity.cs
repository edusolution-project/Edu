using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseCustomerEntity.Database
{
    public class ModGradeEntity : EntityBase
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Code")]
        public string Code { get; set; }
        [JsonProperty("SubjectID")]
        public string SubjectID { get; set; }
        [JsonProperty("ParentID")]
        public string ParentID { get; set; }
        [JsonProperty("CreateUser")]
        public string CreateUser { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("Updated")]
        public DateTime Updated { get; set; }
        [JsonProperty("IsAdmin")]
        public bool IsAdmin { get; set; }
        [JsonProperty("NIsActiveame")]
        public bool IsActive { get; set; }
        [JsonProperty("Order")]
        public int Order { get; set; }
    }
    public class ModGradeService : ServiceBase<ModGradeEntity>
    {
        public ModGradeService(IConfiguration config) : base(config, "ModGrades", "VES")
        {

        }
        public ModGradeService(IConfiguration config, string tableName) : base(config, tableName)
        {

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

        public List<ModGradeEntity> GetRootItems(string SubjectID = "0")
        {
            return CreateQuery().Find(o => o.IsActive && (string.IsNullOrEmpty(o.ParentID) || o.ParentID.Equals("0")) && (SubjectID == "0" || o.SubjectID == SubjectID)).ToList();
        }


        public long CountSubGradeByID(string id)
        {
            return CreateQuery().CountDocuments(o => o.ParentID == id);
        }

    }
}
