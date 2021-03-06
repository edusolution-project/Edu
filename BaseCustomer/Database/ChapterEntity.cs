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
        [JsonProperty("IsHideAnswer")]
        public bool IsHideAnswer { get; set; }


        public ChapterEntity()
        {
        }


    }
    public class ChapterService : ServiceBase<ChapterEntity>
    {
        private CourseService _courseService;

        public ChapterService(IConfiguration config) : base(config)
        {
            _courseService = new CourseService(config);
            //_lessonService = new LessonService(config);
            //_lessonPartService = new CloneLessonPartService(config);
            //_chapterProgressService = new ChapterProgressService(config);

            var indexs = new List<CreateIndexModel<ChapterEntity>>
            {
                //ClassSubjectID_1_ParentID_1
                new CreateIndexModel<ChapterEntity>(
                    new IndexKeysDefinitionBuilder<ChapterEntity>()
                    .Ascending(t => t.ClassSubjectID)
                    .Ascending(t=> t.ParentID)),
                //ParentID_1
                new CreateIndexModel<ChapterEntity>(
                    new IndexKeysDefinitionBuilder<ChapterEntity>()
                    .Ascending(t=> t.ParentID))
            };
            Collection.Indexes.CreateManyAsync(indexs);
        }

        public ChapterService(IConfiguration config, string dbName) : base(config, dbName)
        {

        }

        public async Task IncreaseLessonCounter(string ID, long lesInc, long examInc, long pracInc, List<string> listid = null)//prevent circular ref
        {
            var r = await CreateQuery().UpdateOneAsync(t => t.ID == ID, new UpdateDefinitionBuilder<ChapterEntity>()
                .Inc(t => t.TotalLessons, lesInc)
                .Inc(t => t.TotalExams, examInc)
                .Inc(t => t.TotalPractices, pracInc));
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
                            _ = IncreaseLessonCounter(current.ParentID, lesInc, examInc, pracInc, listid);
                    }
                    else
                        _ = _courseService.IncreaseLessonCounter(current.CourseID, lesInc, examInc, pracInc);
                }
            }
        }

        public IEnumerable<ChapterEntity> GetByClassSubject(string ClassSubjectID)
        {
            return CreateQuery().Find(c => c.ClassSubjectID == ClassSubjectID).SortBy(t => t.Order).ToEnumerable();
        }

        public IEnumerable<ChapterEntity> GetSubChapters(string ClassSubjectID, string ParentID)
        {
            if (!string.IsNullOrEmpty(ClassSubjectID))
                return CreateQuery().Find(c => c.ClassSubjectID == ClassSubjectID && c.ParentID == ParentID).SortBy(t => t.Order).ToEnumerable();
            else
                return CreateQuery().Find(c => c.ParentID == ParentID).SortBy(t => t.Order).ToEnumerable();
        }

        public async Task RemoveClassSubjectChapter(string ClassSubjectID)
        {
            await Collection.DeleteManyAsync(t => t.ClassSubjectID == ClassSubjectID);
        }

        public IEnumerable<ChapterEntity> GetByClassSubjectID(string ClassSubjectID)
        {
            return Collection.Find(x => x.ClassSubjectID == ClassSubjectID).ToEnumerable();
        }

        public List<string> GetChapTreeIDs(string ClassSubjectID, string rootchapID)
        {
            var listIDs = new List<string> { rootchapID };
            var subchapIDs = GetSubChapters(ClassSubjectID, rootchapID).Select(c => c.ID).ToList();
            foreach (var cid in subchapIDs)
            {
                var scids = GetChapTreeIDs(ClassSubjectID, cid);
                if (scids.Contains(rootchapID))//circular ref
                    scids.Remove(rootchapID);
                listIDs.AddRange(GetChapTreeIDs(ClassSubjectID, cid));
            }
            return listIDs;
        }
    }
}
