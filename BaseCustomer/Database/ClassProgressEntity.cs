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
        //[JsonProperty("ClassSubjectID")]
        //public string ClassSubjectID { get; set; }
        //[JsonProperty("CompletedLessons")]
        //public List<string> CompletedLessons { get; set; }
        [JsonProperty("Completed")]
        public int Completed { get; set; }
        [JsonProperty("TotalLessons")]
        public long TotalLessons { get; set; }
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
    }

    public class ClassProgressService : ServiceBase<ClassProgressEntity>
    {
        private ClassService _classService;
        private ClassSubjectService _classSubjectService;
        private LessonService _lessonService;

        public ClassProgressService(IConfiguration config
            //, ClassService classService, LessonService lessonService, ClassSubjectService classSubjectService
            ) : base(config)
        {
            //_classService = classService;
            //_lessonService = lessonService;
            //_classSubjectService = classSubjectService;
            _classService = new ClassService(config);
            _lessonService = new LessonService(config);
            _classSubjectService = new ClassSubjectService(config);

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

        public async Task UpdateLastLearn(LessonProgressEntity item)
        {
            var currentObj = _classService.GetItemByID(item.ClassID);
            if (currentObj == null) return;
            var progress = GetStudentResult(item.ClassID, item.StudentID);
            if (progress == null)
            {
                var totalLessons = _lessonService.CountClassLesson(item.ClassID);

                //create new progress
                await Collection.InsertOneAsync(new ClassProgressEntity
                {
                    StudentID = item.StudentID,
                    Completed = 1,
                    //CompletedLessons = new List<string>() { item.ClassSubjectID }, //TODO: cập nhật khi số lượng môn học thay đổi
                    TotalLessons = totalLessons,
                    ClassID = currentObj.ID,
                    LastDate = DateTime.Now,
                    LastLessonID = item.LessonID
                });
            }
            else
            {
                var update = new UpdateDefinitionBuilder<ClassProgressEntity>()
                     //.AddToSet(t => t.CompletedLessons, item.ClassSubjectID)
                     .Set(t => t.LastDate, DateTime.Now)
                     .Set(t => t.LastLessonID, item.LessonID);
                if (item.TotalLearnt == 1) //new
                    update = update.Inc(t => t.Completed, 1);

                await Collection.UpdateManyAsync(t => t.ClassID == currentObj.ID && t.StudentID == item.StudentID, update);
            }
        }

        public async Task UpdatePoint(LessonProgressEntity item)
        {
            var progress = GetStudentResult(item.ClassID, item.StudentID);
            if (progress == null)
                return;
            else
            {
                if (item.Tried == 1 || progress.ExamDone == 0)//new
                    progress.ExamDone++;

                progress.TotalPoint += item.PointChange;
                if(progress.TotalPoint > 100)
                    progress.AvgPoint = progress.TotalPoint / progress.ExamDone;

                await Collection.ReplaceOneAsync(t => t.ID == progress.ID, progress);
            }
        }

        public async Task<long> RefreshTotalLessonForClass(string ClassID)
        {
            var totalLessons = _lessonService.CountClassLesson(ClassID);

            var update = new UpdateDefinitionBuilder<ClassProgressEntity>()
                     //.AddToSet(t => t.CompletedLessons, item.ClassSubjectID)
                     .Set(t => t.TotalLessons, totalLessons);

            await Collection.UpdateManyAsync(t => t.ClassID == ClassID, update);
            return totalLessons;
        }

        //public async Task DecreaseCompleted(string ClassID, long decrease)
        //{
        //    var update = new UpdateDefinitionBuilder<ClassProgressEntity>()
        //             //.AddToSet(t => t.CompletedLessons, item.ClassSubjectID)
        //             .Inc(t => t.Completed, 0 - decrease);
        //    await Collection.UpdateManyAsync(t => t.ClassID == ClassID, update);
        //}


        public ClassProgressEntity GetStudentResult(string ClassID, string StudentID)
        {
            try
            {
                return CreateQuery().Find(t => t.ClassID == ClassID && t.StudentID == StudentID)?.FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public async Task DecreaseClassSubject(ClassSubjectProgressEntity clssbj)
        {
            var update = new UpdateDefinitionBuilder<ClassProgressEntity>()
                     //.AddToSet(t => t.CompletedLessons, item.ClassSubjectID)
                     .Inc(t => t.Completed, 0 - clssbj.Completed)
                     .Inc(t => t.ExamDone, 0 - clssbj.ExamDone)
                     .Inc(t => t.TotalPoint, 0 - clssbj.TotalPoint)
                     .Inc(t => t.TotalLessons, 0 - clssbj.TotalLessons);
            await Collection.UpdateManyAsync(t => t.ClassID == clssbj.ClassID && t.StudentID == clssbj.StudentID, update);
        }

        public IEnumerable<StudentRanking> GetClassResults(string ClassID)
        {
            return CreateQuery().Find(t => t.ClassID == ClassID).Project(t => new StudentRanking
            {
                StudentID = t.StudentID,
                AvgPoint = t.AvgPoint,
                ExamDone = t.ExamDone,
                TotalPoint = t.TotalPoint
            }).ToEnumerable();
        }

        public long DecreasePoint(LessonProgressEntity item)
        {
            var filter = Builders<ClassProgressEntity>.Filter.Where(t => t.ClassID == item.ClassID && t.StudentID == item.StudentID);
            var update = Builders<ClassProgressEntity>.Update.Inc(t => t.TotalPoint, 0 - item.LastPoint);
            return Collection.UpdateMany(Builders<ClassProgressEntity>.Filter.And(filter),
                update
                ).ModifiedCount;
        }
    }
}
