using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class ManageExamEntity : EntityBase //quan ly ki thi
    {
        [JsonProperty("Name")]
        public String Name { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("Updated")]
        public DateTime Updated { get; set; }
        [JsonProperty("CreaterUser")]
        public String CreateUser { get; set; }
        [JsonProperty("Center")]
        public String Center { get; set; }
    }

    public class ManageExamService : ServiceBase<ManageExamEntity>
    {
        public ManageExamService(IConfiguration configuration) : base(configuration)
        {
            var indexs = new List<CreateIndexModel<ManageExamEntity>>
            {
                //Code_1
                new CreateIndexModel<ManageExamEntity>(
                    new IndexKeysDefinitionBuilder<ManageExamEntity>()
                    .Ascending(t => t.ID))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public IEnumerable<ManageExamEntity> GetItemsByTeacherAndCenter(string userID, string centerID )
        {
            if (String.IsNullOrEmpty(userID))
            {
                return CreateQuery().Find(x => x.Center.Equals(centerID)).SortByDescending(x=>x.CreateUser).ToEnumerable();
            }
            else
            {
                return CreateQuery().Find(x => x.Center.Equals(centerID) && x.CreateUser.Equals(userID)).SortByDescending(x => x.CreateUser).ToEnumerable();
            }
        }
    }
}
