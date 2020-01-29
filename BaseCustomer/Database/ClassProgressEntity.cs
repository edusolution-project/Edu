﻿using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseCustomerEntity.Database
{
    public class ClassProgressEntity : EntityBase
    {
        [JsonProperty("StudentID")]
        public string StudentID { get; set; }
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
        [JsonProperty("ClassSubjectID")]
        public string ClassSubjectID { get; set; }
        [JsonProperty("CompletedLessons")]
        public List<string> CompletedLessons { get; set; }
        [JsonProperty("TotalLessons")]
        public int TotalLessons { get; set; }
        [JsonProperty("LastLessonID")]
        public string LastLessonID { get; set; }
        [JsonProperty("LastDate")]
        public DateTime LastDate { get; set; }
        [JsonProperty("AvgPoint")]
        public double AvgPoint { get; set; }        
    }

    public class ClassProgressService : ServiceBase<ClassProgressEntity>
    {
        private ClassService _classService;
        private LessonService _lessonService;

        public ClassProgressService(IConfiguration config, ClassService classService, LessonService lessonService) : base(config)
        {
            _classService = classService;
            _lessonService = lessonService;

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

        public async Task UpdateLastLearn(LearningHistoryEntity item)
        {
            var currentClass = _classService.GetItemByID(item.ClassID);
            var progress = GetItemByClassID(item.ClassID, item.StudentID);
            if (progress == null)
            {
                //create new progress
                await Collection.InsertOneAsync(new ClassProgressEntity
                {
                    StudentID = item.StudentID,
                    CompletedLessons = new List<string>() { item.LessonID },
                    TotalLessons = (int)_lessonService.CreateQuery().CountDocuments(t => t.CourseID == currentClass.CourseID),
                    ClassID = currentClass.ID,
                    LastDate = DateTime.Now,
                    LastLessonID = item.LessonID
                });
            }
            else
            {
                await Collection.UpdateManyAsync(t => t.ClassID == item.ClassID && t.StudentID == item.StudentID,
                     new UpdateDefinitionBuilder<ClassProgressEntity>()
                     .AddToSet(t => t.CompletedLessons, item.LessonID)
                     .Set(t => t.LastDate, DateTime.Now)
                     .Set(t => t.LastLessonID, item.LessonID)
                     );
            }
        }

        public ClassProgressEntity GetItemByClassID(string ClassID, string StudentID)
        {
            return CreateQuery().Find(t => t.ClassID == ClassID && t.StudentID == StudentID).FirstOrDefault();
        }
    }
}
