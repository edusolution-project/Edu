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
        [JsonProperty("LastLessonID")]
        public string LastLessonID { get; set; }
        [JsonProperty("LastDate")]
        public DateTime LastDate { get; set; }
        [JsonProperty("ExamDone")]
        public long ExamDone { get; set; }
        [JsonProperty("AvgPoint")]
        public double AvgPoint { get; set; }
        [JsonProperty("ExamResult")]
        public double ExamResult { get; set; }
        [JsonProperty("TotalPoint")]
        public double TotalPoint { get; set; }
        [JsonProperty("PracticePoint")]
        public double PracticePoint { get; set; }
        [JsonProperty("PracticeDone")]
        public long PracticeDone { get; set; }
        [JsonProperty("PracticeAvgPoint")]
        public double PracticeAvgPoint { get; set; }
        [JsonProperty("PracticeResult")]
        public double PracticeResult { get; set; }
    }

    public class ClassSubjectProgressService : ServiceBase<ClassSubjectProgressEntity>
    {
        private ClassSubjectService _classSubjectService;
        private IConfiguration config;
        private string dbName;

        public ClassSubjectProgressService(IConfiguration config
            ) : base(config)
        {
            _classSubjectService = new ClassSubjectService(config);

            var indexs = new List<CreateIndexModel<ClassSubjectProgressEntity>>
            {
                //ClassID_1
                new CreateIndexModel<ClassSubjectProgressEntity>(
                    new IndexKeysDefinitionBuilder<ClassSubjectProgressEntity>()
                    .Ascending(t => t.StudentID)
                    .Ascending(t => t.ClassID)
                    .Ascending(t => t.ClassSubjectID)),
                new CreateIndexModel<ClassSubjectProgressEntity>(
                    new IndexKeysDefinitionBuilder<ClassSubjectProgressEntity>()
                    .Ascending(t => t.ClassID)
                    .Ascending(t => t.StudentID)
                    .Ascending(t => t.ClassSubjectID)),
                new CreateIndexModel<ClassSubjectProgressEntity>(
                    new IndexKeysDefinitionBuilder<ClassSubjectProgressEntity>()
                    .Ascending(t => t.ClassSubjectID))
            };
            Collection.Indexes.CreateManyAsync(indexs);
        }

        public ClassSubjectProgressService(IConfiguration config, string dbName) : base(config, dbName)
        {
        }

        //public async Task UpdateLastLearn(LessonProgressEntity item)
        //{
        //    var currentObj = _classSubjectService.GetItemByID(item.ClassSubjectID);
        //    if (currentObj == null) return;
        //    var progress = GetItemByClassSubjectID(item.ClassSubjectID, item.StudentID);
        //    if (progress == null)
        //    {

        //        //var totalLessons = _lessonService.CountClassSubjectLesson(item.ClassSubjectID);

        //        //create new progress
        //        await Collection.InsertOneAsync(new ClassSubjectProgressEntity
        //        {
        //            StudentID = item.StudentID,
        //            Completed = 1,
        //            //TotalLessons = totalLessons,
        //            ClassID = currentObj.ClassID,
        //            ClassSubjectID = currentObj.ID,
        //            LastDate = DateTime.Now,
        //            LastLessonID = item.LessonID
        //        });
        //    }
        //    else
        //    {
        //        var update = new UpdateDefinitionBuilder<ClassSubjectProgressEntity>()
        //             .Set(t => t.LastDate, DateTime.Now)
        //             .Set(t => t.LastLessonID, item.LessonID);
        //        if (item.TotalLearnt == 1) //new
        //            update = update.Inc(t => t.Completed, 1);

        //        await Collection.UpdateManyAsync(t => t.ClassSubjectID == currentObj.ID && t.StudentID == item.StudentID, update);
        //    }
        //}

        //public async Task UpdatePoint(LessonProgressEntity item, double pointchange = 0)
        //{
        //    var progress = GetItemByClassSubjectID(item.ClassSubjectID, item.StudentID);
        //    if (progress == null)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        if (item.Tried == 1 || progress.ExamDone == 0)//new
        //            progress.ExamDone += (long)item.Multiple;
        //        progress.TotalPoint += (pointchange > 0 ? pointchange : item.PointChange) * item.Multiple;//%
        //        progress.AvgPoint = progress.TotalPoint / progress.ExamDone;
        //        await Collection.ReplaceOneAsync(t => t.ID == progress.ID, progress);
        //    }
        //}

        //public async Task UpdatePracticePoint(LessonProgressEntity item, double pointchange = 0)
        //{
        //    var progress = GetItemByClassSubjectID(item.ClassSubjectID, item.StudentID);
        //    var change = (pointchange > 0 ? pointchange : item.PointChange);
        //    if (progress == null)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        if (item.Tried == 1 || progress.PracticeDone == 0)//new
        //            progress.PracticeDone += (long)item.Multiple;
        //        progress.LastLessonID = item.ID;
        //        progress.PracticePoint += change;
        //        progress.PracticeAvgPoint = progress.PracticePoint / progress.PracticeDone;
        //        await Collection.ReplaceOneAsync(t => t.ID == progress.ID, progress);
        //    }
        //}

        public ClassSubjectProgressEntity GetItemByClassSubjectID(string ClassSubjectID, string StudentID)
        {
            return CreateQuery().Find(t => t.ClassSubjectID == ClassSubjectID && t.StudentID == StudentID).FirstOrDefault();
        }

        public IEnumerable<ClassSubjectProgressEntity> GetListOfCurrentSubject(string ClassSubjectID)
        {
            var currentObj = _classSubjectService.GetItemByID(ClassSubjectID);
            if (currentObj == null) return null;
            return CreateQuery().Find(t => t.ClassSubjectID == currentObj.ID).ToEnumerable();
        }

        //public long DecreasePoint(LessonProgressEntity item)
        //{
        //    var filter = Builders<ClassSubjectProgressEntity>.Filter.Where(t => t.ClassSubjectID == item.ClassSubjectID && t.StudentID == item.StudentID);
        //    var update = Builders<ClassSubjectProgressEntity>.Update.Inc(t => t.TotalPoint, 0 - item.LastPoint);
        //    return Collection.UpdateMany(Builders<ClassSubjectProgressEntity>.Filter.And(filter),
        //        update
        //        ).ModifiedCount;
        //}

        //public long DecreasePracticePoint(LessonProgressEntity item)
        //{
        //    var filter = Builders<ClassSubjectProgressEntity>.Filter.Where(t => t.ClassSubjectID == item.ClassSubjectID && t.StudentID == item.StudentID);
        //    var update = Builders<ClassSubjectProgressEntity>.Update.Inc(t => t.PracticePoint, 0 - item.LastPoint);
        //    return Collection.UpdateMany(Builders<ClassSubjectProgressEntity>.Filter.And(filter),
        //        update
        //        ).ModifiedCount;
        //}
    }


}
