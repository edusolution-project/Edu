﻿using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseCustomerEntity.Database
{
    public class ClassSubjectEntity : EntityBase
    {
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
        [JsonProperty("GradeID")]
        public string GradeID { get; set; }
        [JsonProperty("SubjectID")]
        public string SubjectID { get; set; }
        [JsonProperty("CourseID")]
        public string CourseID { get; set; }
        [JsonProperty("TeacherID")]
        public string TeacherID { get; set; }
        [JsonProperty("Syllabus")]
        public string Syllabus { get; set; }
        [JsonProperty("Modules")]
        public string Modules { get; set; }
        [JsonProperty("References")]
        public string References { get; set; }
        [JsonProperty("LearningOutcomes")]
        public string LearningOutcomes { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }
        [JsonProperty("Image")]
        public string Image { get; set; }
    }

    public class ClassSubjectService : ServiceBase<ClassSubjectEntity>
    {
        public ClassSubjectService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<ClassSubjectEntity>>
            {
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public List<ClassSubjectEntity> GetByClassID(string ClassID)
        {
            return Collection.Find(t => t.ClassID == ClassID).ToList();
        }

        public Task RemoveClassSubjects(string ClassID)
        {
            _ = Collection.DeleteManyAsync(t => t.ClassID == ClassID);
            return Task.CompletedTask;
        }
    }

}
