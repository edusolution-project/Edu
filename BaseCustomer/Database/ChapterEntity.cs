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
    public class ChapterEntity : CourseChapterEntity
    {
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
        [JsonProperty("ClassSubjectID")]
        public string ClassSubjectID { get; set; }
        [JsonProperty("BasePoint")] //Lowest Point
        public double BasePoint { get; set; }
        [JsonProperty("ConditionChapter")] //Allowing open lesson based on condition-chapter's result
        public string ConditionChapter { get; set; }
        [JsonProperty("StartDate")]
        public DateTime StartDate { get; set; }
        [JsonProperty("EndDate")]
        public DateTime EndDate { get; set; }
        [JsonProperty("PracticeCount")]//Count of Completed Non-exam lesson 
        public double PracticeCount { get; set; }
    }
    public class ChapterService : ServiceBase<ChapterEntity>
    {

        private CourseService _courseService;
        private LessonService _lessonService;
        private CloneLessonPartService _lessonPartService;
        //private ChapterProgressService _chapterProgressService;

        public ChapterService(IConfiguration config) : base(config)
        {
            _courseService = new CourseService(config);
            _lessonService = new LessonService(config);
            _lessonPartService = new CloneLessonPartService(config);
            //_chapterProgressService = new ChapterProgressService(config);

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
                        if (listid.IndexOf(current.ParentID) < 0) _ = IncreaseLessonCount(current.ParentID, increment, listid);
                    }
                    else
                        _ = _courseService.IncreaseLessonCount(current.CourseID, increment);
                }
            }
        }

        public List<ChapterEntity> GetSubChapters(string ClassSubjectID, string ParentID)
        {
            return CreateQuery().Find(c => c.ClassSubjectID == ClassSubjectID && c.ParentID == ParentID).SortBy(t => t.Order).ToList();
        }

        public async Task RemoveClassSubjectChapter(string ClassSubjectID)
        {
            await Collection.DeleteManyAsync(t => t.ClassSubjectID == ClassSubjectID);
        }

        public async Task IncreasePracticeCount(string ID, long increment, List<string> listid = null)//prevent circular ref
        {
            var r = CreateQuery().FindOneAndUpdate(t => t.ID == ID, new UpdateDefinitionBuilder<ChapterEntity>().Inc(t => t.PracticeCount, increment));
            if (r != null)
            {
                //_ = _chapterProgressService.CreateQuery().UpdateManyAsync(t => t.ChapterID == ID, new UpdateDefinitionBuilder<ChapterProgressEntity>().Set(t => t.PracticeCount, r.PracticeCount));

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
                            await IncreasePracticeCount(current.ParentID, increment, listid);
                    }
                }
            }
        }

        public int CountChapterPractice(string chapterID, string classSubjectID)
        {
            var result = 0;
            var lessonids = _lessonService.CreateQuery().Find(t => t.TemplateType == LESSON_TEMPLATE.LECTURE && t.ChapterID == chapterID).Project(t => t.ID).ToEnumerable();
            if (lessonids != null && lessonids.Count() > 0)
            {
                var quizList = new List<string> { "QUIZ1", "QUIZ2", "QUZI3", "ESSAY" };
                foreach (var id in lessonids)
                {
                    if (_lessonPartService.CreateQuery().Find(t => t.ParentID == id && quizList.Contains(t.Type)).CountDocuments() > 0)
                        result++;
                }
            }
            var subchaps = GetSubChapters(classSubjectID, chapterID);
            if (subchaps != null && subchaps.Count > 0)
            {
                foreach (var chap in subchaps)
                    result += CountChapterPractice(chap.ID, chap.ClassSubjectID);
            }
            return result;

        }
    }
}
