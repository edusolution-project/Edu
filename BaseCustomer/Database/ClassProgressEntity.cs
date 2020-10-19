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
    public class ClassProgressEntity : EntityBase
    {
        [JsonProperty("StudentID")]
        public string StudentID { get; set; }
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
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
        [JsonProperty("TotalPoint")]
        public double TotalPoint { get; set; }
        [JsonProperty("PracticePoint")]
        public double PracticePoint { get; set; }
        [JsonProperty("PracticeDone")]
        public long PracticeDone { get; set; }
        [JsonProperty("PracticeAvgPoint")]
        public double PracticeAvgPoint { get; set; }
    }

    public class ClassProgressService : ServiceBase<ClassProgressEntity>
    {
        public ClassProgressService(IConfiguration config
            ) : base(config)
        {
            var indexs = new List<CreateIndexModel<ClassProgressEntity>>
            {
                //ClassID_1
                new CreateIndexModel<ClassProgressEntity>(
                    new IndexKeysDefinitionBuilder<ClassProgressEntity>()
                    .Ascending(t => t.StudentID)
                    .Ascending(t => t.ClassID))
            };
            Collection.Indexes.CreateManyAsync(indexs);
        }

        //public async Task UpdateLastLearn(LessonProgressEntity item)
        //{
        //    var progress = GetStudentResult(item.ClassID, item.StudentID);
        //    if (progress == null)
        //    {
        //        progress = new ClassProgressEntity
        //        {
        //            StudentID = item.StudentID,
        //            Completed = 1,
        //            ClassID = item.ClassID,
        //            LastDate = DateTime.Now,
        //            LastLessonID = item.LessonID
        //        };
        //        //create new progress
        //        await Collection.InsertOneAsync(progress);
        //    }
        //    else
        //    {
        //        var update = new UpdateDefinitionBuilder<ClassProgressEntity>()
        //             .Set(t => t.LastDate, DateTime.Now)
        //             .Set(t => t.LastLessonID, item.LessonID);
        //        if (item.TotalLearnt == 1) //new
        //            update = update.Inc(t => t.Completed, 1);

        //        await Collection.UpdateManyAsync(t => t.ClassID == item.ClassID && t.StudentID == item.StudentID, update);
        //    }
        //}

        //public async Task UpdatePoint(LessonProgressEntity item, double pointchange = 0)
        //{
        //    var progress = GetStudentResult(item.ClassID, item.StudentID);
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
        //        if (progress.AvgPoint > 100)
        //            throw (new Exception(item.LessonID + " - " + item.ID));
        //        await Collection.ReplaceOneAsync(t => t.ID == progress.ID, progress);
        //    }
        //}

        //public async Task UpdatePracticePoint(LessonProgressEntity item, double pointchange = 0)
        //{
        //    var progress = GetStudentResult(item.ClassID, item.StudentID);
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

        //public async Task DecreaseCompleted(string ClassID, long decrease)
        //{
        //    var update = new UpdateDefinitionBuilder<ClassProgressEntity>()
        //             //.AddToSet(t => t.CompletedLessons, item.ClassSubjectID)
        //             .Inc(t => t.Completed, 0 - decrease);
        //    await Collection.UpdateManyAsync(t => t.ClassID == ClassID, update);
        //}


        //public ClassProgressEntity GetStudentResult(string ClassID, string StudentID)
        //{
        //    try
        //    {
        //        return CreateQuery().Find(t => t.ClassID == ClassID && t.StudentID == StudentID)?.FirstOrDefault();
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        //public async Task DecreaseClassSubject(ClassSubjectProgressEntity clssbj)
        //{
        //    var update = new UpdateDefinitionBuilder<ClassProgressEntity>()
        //             //.AddToSet(t => t.CompletedLessons, item.ClassSubjectID)
        //             .Inc(t => t.Completed, 0 - clssbj.Completed)
        //             .Inc(t => t.ExamDone, 0 - clssbj.ExamDone)
        //             .Inc(t => t.TotalPoint, 0 - clssbj.TotalPoint)
        //             .Inc(t => t.PracticePoint, 0 - clssbj.PracticePoint)
        //             .Inc(t => t.PracticeDone, 0 - clssbj.PracticeDone);
        //    await Collection.UpdateManyAsync(t => t.ClassID == clssbj.ClassID && t.StudentID == clssbj.StudentID, update);
        //}

        //public long DecreasePoint(LessonProgressEntity item)
        //{
        //    var filter = Builders<ClassProgressEntity>.Filter.Where(t => t.ClassID == item.ClassID && t.StudentID == item.StudentID);
        //    var update = Builders<ClassProgressEntity>.Update.Inc(t => t.TotalPoint, 0 - item.LastPoint);
        //    return Collection.UpdateMany(Builders<ClassProgressEntity>.Filter.And(filter),
        //        update
        //        ).ModifiedCount;
        //}

        //public long DecreasePracticePoint(LessonProgressEntity item)
        //{
        //    var filter = Builders<ClassProgressEntity>.Filter.Where(t => t.ClassID == item.ClassID && t.StudentID == item.StudentID);
        //    var update = Builders<ClassProgressEntity>.Update.Inc(t => t.PracticePoint, 0 - item.LastPoint);
        //    return Collection.UpdateMany(Builders<ClassProgressEntity>.Filter.And(filter),
        //        update
        //        ).ModifiedCount;
        //}

        public ClassProgressEntity GetByClassID(string ClassID)
        {
            return Collection.Find(x => x.ClassID == ClassID).FirstOrDefault();
        }

        public ClassProgressEntity GetItemByClassID(string ClassID, string StudentID)
        {
            return CreateQuery().Find(t => t.ClassID == ClassID && t.StudentID == StudentID).FirstOrDefault();
        }
    }
}
