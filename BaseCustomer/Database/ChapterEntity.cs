using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseCustomerEntity.Database
{
    public class ChapterEntity : CourseChapterEntity
    {
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
        [JsonProperty("ClassSubjectID")]
        public string ClassSubjectID { get; set; }
    }
    public class ChapterService : ServiceBase<ChapterEntity>
    {

        private CourseService _courseService;

        public ChapterService(IConfiguration config) : base(config)
        {
            _courseService = new CourseService(config);

            var indexs = new List<CreateIndexModel<ChapterEntity>>
            {
                //SubjectID_1_ParentID_1
                new CreateIndexModel<ChapterEntity>(
                    new IndexKeysDefinitionBuilder<ChapterEntity>()
                    .Ascending(t => t.CourseID)
                    .Ascending(t=> t.ParentID)),
                //ParentID_1
                new CreateIndexModel<ChapterEntity>(
                    new IndexKeysDefinitionBuilder<ChapterEntity>()
                    .Ascending(t=> t.ParentID))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public async Task IncreaseLessonCount(string ID, long increment, List<string> listid = null)//prevent circular ref
        {
            var r = await CreateQuery().UpdateOneAsync(t => t.ID == ID, new UpdateDefinitionBuilder<ChapterEntity>().Inc(t => t.TotalLessons, increment));
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
                        else
                        {
                            var e = 1;
                        }
                    }
                    else
                        _ = _courseService.IncreaseLessonCount(current.CourseID, increment);
                }

            }
        }

        public List<ChapterEntity> GetSubChapters(string CourseID, string ParentID)
        {
            return CreateQuery().Find(c => c.CourseID == CourseID && c.ParentID == ParentID).SortBy(t => t.Order).ToList();
        }

        public async Task RemoveClassSubjectChapter(string ClassSubjectID)
        {
            await Collection.DeleteManyAsync(t => t.ClassSubjectID == ClassSubjectID);
        }
    }
}
