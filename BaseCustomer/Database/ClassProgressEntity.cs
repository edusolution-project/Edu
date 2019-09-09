using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BaseCustomerEntity.Database
{
    public class ClassProgressEntity : EntityBase
    {
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
        [JsonProperty("CompletedLessons")]
        public List<string> CompletedLessons { get; set; }
        [JsonProperty("TotalLessons")]
        public int TotalLessons { get; set; }
        [JsonProperty("LastLesson")]
        public string LastLessonID { get; set; }
        [JsonProperty("LastDate")]
        public DateTime LastDate { get; set; }

    }
    public class ClassProgressService : ServiceBase<ClassProgressEntity>
    {
        private ClassService _classService;
        private LessonService _lessonService;

        public ClassProgressService(IConfiguration config, ClassService classService, LessonService lessonService) : base(config)
        {
            _classService = classService;
            _lessonService = lessonService;
        }

        public void UpdateLastLearn(LearningHistoryEntity item)
        {
            var currentClass = _classService.GetItemByID(item.ClassID);
            var progress = GetItemByClassID(item.ClassID);
            if (progress == null)
            {
                //create new progress
                progress = new ClassProgressEntity()
                {
                    CompletedLessons = new List<string>() { },
                    TotalLessons = (int)_lessonService.CreateQuery().CountDocuments(t => t.CourseID == currentClass.CourseID),
                    ClassID = currentClass.ID,

                };
            }
            progress.LastDate = DateTime.Now;
            progress.LastLessonID = item.LessonID;
            if (progress.CompletedLessons.IndexOf(item.LessonID) <= 0)
                progress.CompletedLessons.Add(item.LessonID);
            CreateOrUpdate(progress);
        }

        public ClassProgressEntity GetItemByClassID(string ClassID)
        {
            return CreateQuery().Find(t => t.ClassID == ClassID).FirstOrDefault();
        }
    }
}
