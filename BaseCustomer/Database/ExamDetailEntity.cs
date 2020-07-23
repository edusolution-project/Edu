﻿using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace BaseCustomerEntity.Database
{
    public class ExamDetailEntity : EntityBase
    {
        [JsonProperty("ExamID")]
        public string ExamID { get; set; }
        [JsonProperty("LessonPartID")]
        public string LessonPartID { get; set; }
        [JsonProperty("QuestionID")]
        public string QuestionID { get; set; }
        [JsonProperty("AnswerID")]
        public string AnswerID { get; set; }
        [JsonProperty("RealAnswerID")]
        public string RealAnswerID { get; set; }
        [JsonProperty("QuestionValue")]
        public string QuestionValue { get; set; }
        [JsonProperty("AnswerValue")]
        public string AnswerValue { get; set; }
        [JsonProperty("RealAnswerValue")]
        public string RealAnswerValue { get; set; }
        [JsonProperty("Point")]
        public double Point { get; set; }
        [JsonProperty("MaxPoint")]
        public double MaxPoint { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("Updated")]
        public DateTime Updated { get; set; }
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
        [JsonProperty("ClassSubjectID")]
        public string ClassSubjectID { get; set; }
        [JsonProperty("StudentID")]
        public string StudentID { get; set; }
        [JsonProperty("Medias")]
        public List<Media> Medias { get; set; } // file học viên
        [JsonProperty("MediasAnswers")]
        public List<Media> MediasAnswers { get; set; } // file trả lời giáo viên nếu cần
    }
    public class ExamDetailService : ServiceBase<ExamDetailEntity>
    {
        public ExamDetailService(IConfiguration configuration) : base(configuration)
        {

        }

        public async Task ConvertClassSubject(ClassSubjectEntity classSubject)
        {
            await Collection.UpdateManyAsync(t => t.ClassID == classSubject.ClassID, Builders<ExamDetailEntity>.Update.Set("ClassSubjectID", classSubject.ID));
        }
    }
}
