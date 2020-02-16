using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseCustomerEntity.Database
{
    public class ClassSubjectProgressEntity : EntityBase
    {
        [JsonProperty("StudentID")]
        public string StudentID { get; set; }
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
        [JsonProperty("ClassSubjectID")]
        public string ClassSubjectID { get; set; }
        [JsonProperty("Completed")]
        public int Completed { get; set; }
        [JsonProperty("TotalLessons")]
        public long TotalLessons { get; set; }
        [JsonProperty("LastLessonID")]
        public string LastLessonID { get; set; }
        [JsonProperty("LastDate")]
        public DateTime LastDate { get; set; }
        [JsonProperty("AvgPoint")]
        public double AvgPoint { get; set; }
    }

    public class ClassSubjectProgressService : ServiceBase<ClassSubjectProgressEntity>
    {
        private ClassSubjectService _classSubjectService;
        private LessonService _lessonService;

        public ClassSubjectProgressService(IConfiguration config,
            ClassSubjectService classSubjectService,
            LessonService lessonService) : base(config)
        {
            _lessonService = lessonService;
            _classSubjectService = classSubjectService;

            var indexs = new List<CreateIndexModel<ClassSubjectProgressEntity>>
            {
                //ClassID_1
                new CreateIndexModel<ClassSubjectProgressEntity>(
                    new IndexKeysDefinitionBuilder<ClassSubjectProgressEntity>()
                    .Ascending(t => t.StudentID)
                    .Ascending(t => t.ClassID)
                    .Ascending(t => t.ClassSubjectID))
            };
            Collection.Indexes.CreateManyAsync(indexs);
        }

        public async Task UpdateLastLearn(LessonProgressEntity item)
        {
            var currentObj = _classSubjectService.GetItemByID(item.ClassSubjectID);
            if (currentObj == null) return;
            var progress = GetItemByClassSubjectID(item.ClassSubjectID, item.StudentID);
            if (progress == null)
            {

                var totalLessons = _lessonService.CountCourseLesson(currentObj.CourseID);

                //create new progress
                await Collection.InsertOneAsync(new ClassSubjectProgressEntity
                {
                    StudentID = item.StudentID,
                    Completed = 1,
                    TotalLessons = totalLessons,
                    ClassID = currentObj.ClassID,
                    ClassSubjectID = currentObj.ID,
                    LastDate = DateTime.Now,
                    LastLessonID = item.LessonID
                });
            }
            else
            {
                var update = new UpdateDefinitionBuilder<ClassSubjectProgressEntity>()
                     .Set(t => t.LastDate, DateTime.Now)
                     .Set(t => t.LastLessonID, item.LessonID);
                if (item.TotalLearnt == 1) //new
                    update = update.Inc(t => t.Completed, 1);

                await Collection.UpdateManyAsync(t => t.ClassSubjectID == currentObj.ID && t.StudentID == item.StudentID, update);
            }
        }

        public ClassSubjectProgressEntity GetItemByClassSubjectID(string ClassSubjectID, string StudentID)
        {
            return CreateQuery().Find(t => t.ClassSubjectID == ClassSubjectID && t.StudentID == StudentID).FirstOrDefault();
        }

        public List<ClassSubjectProgressEntity> GetClassListOfCurrentSubject(string ClassSubjectID)
        {
            var currentObj = _classSubjectService.GetItemByID(ClassSubjectID);
            if (currentObj == null) return null;
            return CreateQuery().Find(t => t.ClassID == currentObj.ClassID).ToList();
        }

        //public ClassProgressEntity GetItemByClassSubjectID(string ClassSubjectID, string StudentID)
        //{
        //    return CreateQuery().Find(t => t.ClassSubjectID == ClassSubjectID && t.StudentID == StudentID).FirstOrDefault();
        //}
    }
}
