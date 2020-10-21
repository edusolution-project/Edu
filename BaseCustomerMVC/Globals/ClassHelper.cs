using BaseCustomerEntity.Database;
using Core_v2.Globals;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseCustomerMVC.Globals
{
    public class ClassHelper
    {
        private readonly ClassService _classService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly ChapterService _chapterService;
        private readonly LessonService _lessonService;
        private readonly LessonHelper _lessonHelper;

        private readonly MappingEntity<ChapterEntity, ChapterEntity> _chapterMapping = new MappingEntity<ChapterEntity, ChapterEntity>();

        public ClassHelper(
            ClassService classService,
            ClassSubjectService classSubjectService,
            ChapterService chapterService,
            LessonService lessonService,
            LessonHelper lessonHelper
        )
        {
            _chapterService = chapterService;
            _classSubjectService = classSubjectService;
            _classService = classService;
            _lessonService = lessonService;

            _lessonHelper = lessonHelper;
        }

        public async Task<ClassSubjectEntity> CloneClassSubject(ClassSubjectEntity item, string _userCreate, string orgClassSubjectID)
        {
            _classSubjectService.Collection.InsertOne(item);
            await CloneChapter(new ChapterEntity() { ClassID = item.ClassID, ClassSubjectID = item.ID, OriginID = "0" }, _userCreate, orgClassSubjectID);
            return item;
        }

        public async Task<ChapterEntity> CloneChapter(ChapterEntity item, string _userCreate, string orgClassSubjectID)
        {
            if (item.OriginID != "0")
                _chapterService.Collection.InsertOne(item);
            else
            {
                item.ID = "0";
            }

            var lessons = _lessonService.GetChapterLesson(orgClassSubjectID, item.OriginID);

            if (lessons != null && lessons.Count() > 0)
            {
                foreach (var o in lessons)
                {
                    await _lessonHelper.CopyLessonFromLesson(o, new LessonEntity
                    {
                        ChapterID = item.ID,
                        ClassID = item.ClassID,
                        ClassSubjectID = item.ClassSubjectID,
                        CreateUser = _userCreate,
                    });
                }
            }

            var subChapters = _chapterService.GetSubChapters(orgClassSubjectID, item.OriginID);
            foreach (var o in subChapters)
            {
                var new_chapter = _chapterMapping.Clone(o, new ChapterEntity());
                new_chapter.ClassID = item.ClassID;
                new_chapter.ClassSubjectID = item.ClassSubjectID;
                new_chapter.ParentID = item.ID;
                new_chapter.CreateUser = _userCreate;
                new_chapter.Created = DateTime.UtcNow;
                new_chapter.OriginID = o.ID;
                await CloneChapter(new_chapter, _userCreate, orgClassSubjectID);
            }
            return item;
        }

        internal async Task ChangeLessonPracticeState(LessonEntity lesson)
        {
            if (lesson.ChapterID != "0")
                await IncreaseChapterCounter(lesson.ChapterID, 0, 0, (lesson.IsPractice ? 1 : -1) * (long)lesson.Multiple);
            else
                await IncreaseClassSubjectCounter(lesson.ClassSubjectID, 0, 0, (lesson.IsPractice ? 1 : -1) * (long)lesson.Multiple);
        }

        public async Task IncreaseLessonCounter(LessonEntity lesson, long lessonInc, long examInc, long pracInc)
        {
            if (lesson.ChapterID != "0")
                await IncreaseChapterCounter(lesson.ChapterID, lessonInc, examInc * (long)lesson.Multiple, pracInc * (long)lesson.Multiple);
            else
                await IncreaseClassSubjectCounter(lesson.ClassSubjectID, lessonInc, examInc * (long)lesson.Multiple, pracInc * (long)lesson.Multiple);
        }

        public async Task IncreaseChapterCounter(string ID, long lesInc, long examInc, long pracInc, List<string> listid = null)//prevent circular ref
        {
            var r = await _chapterService.CreateQuery().UpdateOneAsync(t => t.ID == ID, new UpdateDefinitionBuilder<ChapterEntity>()
                .Inc(t => t.TotalLessons, lesInc)
                .Inc(t => t.TotalExams, examInc)
                .Inc(t => t.TotalPractices, pracInc));
            if (r.ModifiedCount > 0)
            {
                if (listid == null)
                    listid = new List<string> { ID };
                else
                    listid.Add(ID);
                var current = _chapterService.GetItemByID(ID);
                if (current != null)
                {
                    if (!string.IsNullOrEmpty(current.ParentID) && (current.ParentID != "0"))
                    {
                        if (listid.IndexOf(current.ParentID) < 0)
                            await IncreaseChapterCounter(current.ParentID, lesInc, examInc, pracInc, listid);
                    }
                    else
                        await IncreaseClassSubjectCounter(current.ClassSubjectID, lesInc, examInc, pracInc);
                }
            }
        }

        public async Task IncreaseClassSubjectCounter(string ID, long lesInc, long examInc, long pracInc)
        {
            var r = await _classSubjectService.CreateQuery().UpdateOneAsync(t => t.ID == ID, new UpdateDefinitionBuilder<ClassSubjectEntity>()
                .Inc(t => t.TotalLessons, lesInc)
                .Inc(t => t.TotalExams, examInc)
                .Inc(t => t.TotalPractices, pracInc));
            if (r.ModifiedCount > 0)
            {
                var current = _classSubjectService.GetItemByID(ID);
                if (current != null)
                {
                    await IncreaseClassCounter(current.ClassID, lesInc, examInc, pracInc);
                }
            }
        }

        public async Task IncreaseClassCounter(string ID, long lesInc, long examInc, long pracInc)
        {
            var r = await _classService.CreateQuery().UpdateOneAsync(t => t.ID == ID, new UpdateDefinitionBuilder<ClassEntity>()
                .Inc(t => t.TotalLessons, lesInc)
                .Inc(t => t.TotalExams, examInc)
                .Inc(t => t.TotalPractices, pracInc));
        }

    }

}
