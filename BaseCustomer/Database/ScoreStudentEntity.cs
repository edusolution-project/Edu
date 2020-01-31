using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseCustomerEntity.Database
{
    public class ScoreStudentEntity : EntityBase
    {
        [JsonProperty("StudentID")]
        public string StudentID { get; set; }
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
        [JsonProperty("ClassSubjectID")]
        public string ClassSubjectID { get; set; }
        [JsonProperty("TeacherID")]
        public string TeacherID { get; set; }
        [JsonProperty("AutoAvgPoint")]
        public double AutoAvgPoint { get; set; }
        [JsonProperty("ManualAvgPoint")]
        public double ManualAvgPoint { get; set; }
        [JsonProperty("UserUpdated")]
        public string UserUpdated { get; set; }
        [JsonProperty("Updated")]
        public DateTime Updated { get; set; }
        [JsonProperty("MarkElements")]
        public List<MarkElement> MarkElements { get; set; } = new List<MarkElement>();
    }

    public class MarkElement : EntityBase
    {
        [JsonProperty("Type")]
        public int? Type { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }
        [JsonProperty("Percent")]
        public double Percent { get; set; }

        [JsonProperty("Point")]
        public double Point { get; set; }

        [JsonProperty("IsMapStructure")]
        public bool IsMapStructure { get; set; }
    }

    public class ScoreStudentService : ServiceBase<ScoreStudentEntity>
    {
        public ScoreStudentService(IConfiguration config) : base(config)
        {

        }

        public ScoreStudentEntity GetScoreStudentByStudentIdAndClassId(string studentid, string classid)
        {
            return Collection.Find(o => o.StudentID == studentid && o.ClassID == classid)?.SingleOrDefault();
        }

        public async Task UpdateClassSubject(ClassSubjectEntity classSubject)
        {
            await Collection.UpdateManyAsync(t => t.ClassID == classSubject.ClassID, Builders<ScoreStudentEntity>.Update.Set("ClassSubjectID", classSubject.ID));
        }
    }
}
