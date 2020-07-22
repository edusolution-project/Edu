using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseCustomerEntity.Database
{
    public class CourseChapterEntity : EntityBase
    {
        public CourseChapterEntity() { }

        public CourseChapterEntity(CourseChapterEntity o)
        {
            this.OriginID = o.ID;
            this.Name = o.Name;
            this.Code = o.Code;
            this.CourseID = o.CourseID;
            this.ParentID = o.ParentID;
            this.ParentType = o.ParentType;
            this.Created = DateTime.Now;
            this.Updated = DateTime.Now;
            this.CreateUser = o.CreateUser;
            this.IsAdmin = o.IsAdmin;
            this.IsActive = o.IsActive;
            this.Order = o.Order;
            this.Description = o.Description;
            this.TotalLessons = o.TotalLessons;
            this.TotalExams = o.TotalExams;
        }

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

        public async Task IncreaseLessonCount(string ID, long increment, List<string> listid = null)//prevent circular ref
        {
            var r = await CreateQuery().UpdateOneAsync(t => t.ID == ID, new UpdateDefinitionBuilder<CourseChapterEntity>().Inc(t => t.TotalLessons, increment));
            if (r.ModifiedCount > 0)
            {
                if (listid == null)
                    listid = new List<string> { ID };
                else
                    listid.Add(ID);
                var current = GetItemByID(ID);
                if (current != null)
                {
                    if (!string.IsNullOrEmpty(current.ParentID) && (current.ParentID != "0"))
                    {
                        if (listid.IndexOf(current.ParentID) < 0)
                            _ = IncreaseLessonCount(current.ParentID, increment, listid);
                    }
                    else
                        _ = _courseService.IncreaseLessonCount(current.CourseID, increment);
                }

            }
        }

        public List<CourseChapterEntity> GetSubChapters(string CourseID, string ParentID)
        {
            return CreateQuery().Find(c => c.CourseID == CourseID && c.ParentID == ParentID).SortBy(t => t.Order).ToList();
        }
    }
}
