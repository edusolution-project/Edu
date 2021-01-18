using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BaseCustomerEntity.Database
{
    public class CourseChapterEntity : EntityBase
    {

        //public CourseChapterEntity() { }

        //public CourseChapterEntity(CourseChapterEntity o)
        //{
        //    Type oldType = this.GetType();
        //    IList<PropertyInfo> props = new List<PropertyInfo>(oldType.GetProperties());

        //    for (int i = 0; props != null && i < props.Count - 1; i++)
        //    {
        //        var item = props[i];
        //        if (item.Name == "ID" || item.Name == "id" || item.Name == "_id") continue;
        //        var value = item.GetValue(o);
        //        this[item.Name] = value;
        //    }
        //    this.Created = DateTime.Now;
        //    this.Updated = DateTime.Now;
        //}

        [JsonProperty("OriginID")]
        public string OriginID { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Code")]
        public string Code { get; set; }
        [JsonProperty("CourseID")]
        public string CourseID { get; set; }
        [JsonProperty("ParentID")]
        public string ParentID { get; set; }
        [JsonProperty("ParentType")]
        public int ParentType { get; set; }
        [JsonProperty("CreateUser")]
        public string CreateUser { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("Updated")]
        public DateTime Updated { get; set; }
        [JsonProperty("IsAdmin")]
        public bool IsAdmin { get; set; }
        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }
        [JsonProperty("Order")]
        public int Order { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("TotalLessons")]
        public long TotalLessons { get; set; }
        [JsonProperty("TotalExams")]
        public long TotalExams { get; set; }
        [JsonProperty("TotalPractices")]
        public long TotalPractices { get; set; }

        [JsonProperty("ConnectID")] //liên kết lộ trình
        public string ConnectID { get; set; }
        [JsonProperty("ConnectType")] //kiểu đối tượng liên kết (chapter/lesson)
        public int ConnectType { get; set; }
        [JsonProperty("Start")] //bắt đầu
        public double Start { get; set; }
        [JsonProperty("Period")] //thời lượng
        public double Period { get; set; }
    }
    public class CourseChapterService : ServiceBase<CourseChapterEntity>
    {

        private CourseService _courseService;

        public CourseChapterService(IConfiguration config) : base(config)
        {
            _courseService = new CourseService(config);

            var indexs = new List<CreateIndexModel<CourseChapterEntity>>
            {
                //SubjectID_1_ParentID_1
                new CreateIndexModel<CourseChapterEntity>(
                    new IndexKeysDefinitionBuilder<CourseChapterEntity>()
                    .Ascending(t => t.CourseID)
                    .Ascending(t=> t.ParentID)),
                //ParentID_1
                new CreateIndexModel<CourseChapterEntity>(
                    new IndexKeysDefinitionBuilder<CourseChapterEntity>()
                    .Ascending(t=> t.ParentID))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }


        public IEnumerable<CourseChapterEntity> GetSubChapters(string CourseID, string ParentID)
        {
            if (string.IsNullOrEmpty(ParentID) || ParentID == "0")
                return CreateQuery().Find(c => c.CourseID == CourseID && c.ParentID == "0").SortBy(t => t.Order).ToEnumerable();
            return CreateQuery().Find(c => c.ParentID == ParentID).SortBy(t => t.Order).ToEnumerable();
        }

        public List<CourseChapterEntity> GetCourseChapters(string CourseID)
        {
            return Collection.Find(x => x.CourseID == CourseID).SortBy(o => o.ParentID).ThenBy(o => o.Order).ThenBy(o => o.ID).ToList();
        }

        public IEnumerable<CourseChapterEntity> GetItemByConnectID(string courseID, string parentID, string connectID)
        {
            if (!string.IsNullOrEmpty(parentID))
                return Collection.Find(x => x.ParentID == parentID && x.ConnectID == connectID).ToEnumerable();
            else
                return Collection.Find(x => (x.CourseID == courseID && x.ParentID == "0") && x.ConnectID == connectID).ToEnumerable();

        }
    }
}
