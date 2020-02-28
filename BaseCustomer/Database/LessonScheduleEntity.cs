using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseCustomerEntity.Database
{
    public class LessonScheduleEntity : EntityBase
    {
        [JsonProperty("LessonID")]
        public string LessonID { get; set; }
        [JsonProperty("Type")]
        public int? Type { get; set; }

        [JsonProperty("StartDate")]
        public DateTime StartDate { get; set; }
        [JsonProperty("EndDate")]
        public DateTime EndDate { get; set; }
        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
        [JsonProperty("ClassSubjectID")]
        public string ClassSubjectID { get; set; }
    }

    public class SCHEDULE_TYPE
    {
        public const int LECTURE = LESSON_TEMPLATE.LECTURE,
            EXAM = LESSON_TEMPLATE.EXAM,
            WEBINAR = 3;
    }

    public class LessonScheduleService : ServiceBase<LessonScheduleEntity>
    {
        public LessonScheduleService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<LessonScheduleEntity>>
            {
                //ClassID_1_StartDate_1
                new CreateIndexModel<LessonScheduleEntity>(
                    new IndexKeysDefinitionBuilder<LessonScheduleEntity>()
                    .Ascending(t => t.ClassID)),
                //ClassID_1_LessonID_1
                new CreateIndexModel<LessonScheduleEntity>(
                    new IndexKeysDefinitionBuilder<LessonScheduleEntity>()
                    .Ascending(t => t.ClassID)
                    .Ascending(t => t.LessonID)),
                //LessonID_1_StartDate_1
                new CreateIndexModel<LessonScheduleEntity>(
                    new IndexKeysDefinitionBuilder<LessonScheduleEntity>()
                    .Ascending(t=> t.LessonID).Ascending(t=> t.StartDate))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public LessonScheduleEntity GetItemByLessonID_ClassSubjectID(string lessonid, string classsubjectid)
        {
            if (string.IsNullOrEmpty(lessonid) || lessonid == "0" || string.IsNullOrEmpty(classsubjectid)) return null;
            return Collection.Find(o => o.LessonID == lessonid && o.ClassSubjectID == classsubjectid)?.SingleOrDefault();
        }

        public async Task UpdateClassSubject(ClassSubjectEntity classSubject)
        {
            await Collection.UpdateManyAsync(t => t.ClassID == classSubject.ClassID, Builders<LessonScheduleEntity>.Update.Set("ClassSubjectID", classSubject.ID));
        }
    }
}
