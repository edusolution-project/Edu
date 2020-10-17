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
    public class GroupAndUserEntity : EntityBase
    {
        [JsonProperty("GroupID")]
        public string GroupID { get; set; }
        [JsonProperty("UserID")]
        public string UserID { get; set; }
        [JsonProperty("TimeJoin")]
        public double TimeJoin { get; set; }
        [JsonProperty("TimeLife")]
        public double TimeLife { get; set; }
    }
    public class GroupAndUserService : ServiceBase<GroupAndUserEntity>
    {
        public GroupAndUserService(IConfiguration configuration) : base(configuration)
        {
            var indexs = new List<CreateIndexModel<ExamEntity>>
            {
                //ClassSubjectID_1_LessonID_1_StudentID_1_ID_-1
                new CreateIndexModel<ExamEntity>(
                    new IndexKeysDefinitionBuilder<ExamEntity>()
                    .Ascending(t=> t.ClassSubjectID)
                    .Ascending(t=> t.LessonID)
                    .Ascending(t=> t.StudentID)
                    .Descending(t=> t.ID)),
                //LessonID_1_StudentID_1_Status_-1
                 new CreateIndexModel<ExamEntity>(
                    new IndexKeysDefinitionBuilder<ExamEntity>()
                    .Ascending(t=> t.LessonID)
                    .Ascending(t=> t.StudentID)
                    .Descending(t=> t.Status)),
                //LessonScheduleID_1_StudentID_1
                new CreateIndexModel<ExamEntity>(
                    new IndexKeysDefinitionBuilder<ExamEntity>()
                    .Ascending(t=> t.LessonScheduleID)
                    .Ascending(t=> t.StudentID)
                    )
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }


        [Obsolete]
        public async Task CreateTimeJoin(string groupName, string userid)
        {
            var oldItem = CreateQuery().Find(o => o.GroupID == groupName && o.UserID == userid)?.FirstOrDefault();
            if(oldItem != null)
            {
                return;
            }
            else
            {
                double unixTimestamp = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
                oldItem = new GroupAndUserEntity()
                {
                    GroupID = groupName,
                    UserID = userid,
                    TimeJoin = unixTimestamp,
                    TimeLife = unixTimestamp
                };
                await CreateQuery().InsertOneAsync(oldItem, CancellationToken.None);
            }
        }

        public async Task UpdateTimeLife(string user)
        {
            var listData = CreateQuery().Find(o => o.UserID == user)?.ToList();
            double unixTimestamp = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            for (int i = 0; listData != null && i < listData.Count; i++)
            {
                var item = listData[i];
                item.TimeLife = unixTimestamp;
                await CreateQuery().ReplaceOneAsync(Builders<GroupAndUserEntity>.Filter.Eq(s => s.ID, item.ID), item);
            }
        }
    }
}
