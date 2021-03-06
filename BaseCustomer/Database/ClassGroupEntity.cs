using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BaseCustomerEntity.Database
{
    public class ClassGroupEntity : EntityBase
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }
        [JsonProperty("Members")]
        public List<GroupMember> Members { get; set; }
    }
    public class ClassGroupService : ServiceBase<ClassGroupEntity>
    {
        public ClassGroupService(IConfiguration configuration) : base(configuration)
        {
            var indexs = new List<CreateIndexModel<ClassGroupEntity>>
            {
                new CreateIndexModel<ClassGroupEntity>(
                    new IndexKeysDefinitionBuilder<ClassGroupEntity>()
                    .Ascending(t => t.ClassID))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public IEnumerable<ClassGroupEntity> GetByClassID(string ClassID)
        {
            return Collection.Find(t => t.ClassID == ClassID).ToEnumerable();
        }
    }

    public class GroupMember
    {
        [JsonProperty("MemberID")]
        public string MemberID { get; set; }
        [JsonProperty("Role")]
        public string MemberRole { get; set; }
        [JsonProperty("JoinDate")]
        public DateTime JoinDate { get; set; }
    }

    public class GroupMemberRole
    {
        public const string TEACHER = "tc", STUDENT = "st";
    }
}
