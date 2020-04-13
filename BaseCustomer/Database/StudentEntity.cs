using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BaseCustomerEntity.Database
{
    public class StudentEntity : EntityBase
    {
        [JsonProperty("StudentId")]
        public string StudentId { get; set; } // mã sinh viên
        [JsonProperty("Avatar")]
        public string Avatar { get; set; }
        [JsonProperty("FullName")]
        public string FullName { get; set; } //họ và tên
        [JsonProperty("Email")]
        public string Email { get; set; }
        [JsonProperty("Phone")]
        public string Phone { get; set; }
        [JsonProperty("Address")]
        public string Address { get; set; }
        [JsonProperty("Class")]
        public List<string> Class { get; set; } //danh sách lớp tham gia
        [JsonProperty("DateBorn")]
        public DateTime DateBorn { get; set; }// ngày sinh
        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }
        [JsonProperty("UserCreate")]
        public string UserCreate { get; set; }
        [JsonProperty("CreateDate")]
        public DateTime CreateDate { get; set; }
        [JsonProperty("Skype")]
        public string Skype { get; set; }
        [JsonProperty("JoinedClasses")]
        public List<string> JoinedClasses { get; set; }
    }
    public class StudentService : ServiceBase<StudentEntity>
    {
        public StudentService(IConfiguration configuration) : base(configuration)
        {
            var indexs = new List<CreateIndexModel<StudentEntity>>
            {
                new CreateIndexModel<StudentEntity>(
                    new IndexKeysDefinitionBuilder<StudentEntity>()
                    .Text(t => t.FullName).Text(t=> t.Email))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public List<StudentEntity> Search(string name, int limit = 0)
        {
            return Collection.Find(Builders<StudentEntity>.Filter.Text("\"" + name + "\"")).Limit(limit).ToList();
        }


        //joined class
        public long CountByClass(string ClassID)
        {
            return Collection.CountDocuments(t => t.JoinedClasses.Contains(ClassID));
        }

        public List<StudentEntity> GetClassStudents(string ClassID)
        {
            return Collection.Find(t => t.JoinedClasses.Contains(ClassID)).ToList();
        }

        public List<string> GetClassStudentIDs(string ClassID)
        {
            return Collection.Find(t => t.JoinedClasses.Contains(ClassID)).Project(t => t.ID).ToList();
        }

        public bool IsStudentInClass(string ClassID, string ID)
        {
            return Collection.CountDocuments(t => t.ID == ID && t.JoinedClasses.Contains(ClassID)) > 0;
        }

        public async Task LeaveClassAll(string ClassID)
        {
            await Collection.UpdateManyAsync(t => t.JoinedClasses.Contains(ClassID), Builders<StudentEntity>.Update.Pull(t => t.JoinedClasses, ClassID));
        }

        public async Task LeaveClassAll(List<string> ClassIDs)
        {
            await Collection.UpdateManyAsync(t => t.JoinedClasses.Exists(o => ClassIDs.Contains(o)), Builders<StudentEntity>.Update.PullAll(t => t.JoinedClasses, ClassIDs));
        }

        public long LeaveClass(string ClassID, string StudentID)
        {
            return Collection.UpdateMany(t => t.ID == StudentID, Builders<StudentEntity>.Update.Pull(t => t.JoinedClasses, ClassID)).ModifiedCount;
        }

        public long JoinClass(string ClassID, string StudentID)
        {
            return Collection.UpdateMany(t => t.ID == StudentID, Builders<StudentEntity>.Update.AddToSet(t => t.JoinedClasses, ClassID)).ModifiedCount;
        }
    }

    public class StudentRanking
    {
        public int Rank { get; set; }
        public string StudentID { get; set; }
        public double AvgPoint { get; set; }
        public double TotalPoint { get; set; }
        public long ExamDone { get; set; }
        public int Count { get; set; }
    }
}
