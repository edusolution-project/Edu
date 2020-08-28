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
        private readonly CloneLessonPartService _lessonPartService;
        private readonly CloneLessonPartQuestionService _questionService;
        private readonly CloneLessonPartAnswerService _answerService;

        private readonly MappingEntity<ChapterEntity, ChapterEntity> _chapterMapping = new MappingEntity<ChapterEntity, ChapterEntity>();
        private readonly MappingEntity<LessonEntity, LessonEntity> _lessonMapping = new MappingEntity<LessonEntity, LessonEntity>();
        private readonly MappingEntity<CloneLessonPartEntity, CloneLessonPartEntity> _lessonPartMapping = new MappingEntity<CloneLessonPartEntity, CloneLessonPartEntity>();
        private readonly MappingEntity<CloneLessonPartQuestionEntity, CloneLessonPartQuestionEntity> _lessonPartQuestionMapping = new MappingEntity<CloneLessonPartQuestionEntity, CloneLessonPartQuestionEntity>();
        private readonly MappingEntity<CloneLessonPartAnswerEntity, CloneLessonPartAnswerEntity> _lessonPartAnswerMapping = new MappingEntity<CloneLessonPartAnswerEntity, CloneLessonPartAnswerEntity>();

        public ClassHelper(
            ClassService classService,
            ClassSubjectService classSubjectService,
            ChapterService chapterService,
            LessonService lessonService,
            CloneLessonPartService cloneLessonPartService,
            CloneLessonPartQuestionService cloneLessonPartQuestionService,
            CloneLessonPartAnswerService cloneLessonPartAnswerService
        )
        {
            _chapterService = chapterService;
            _classSubjectService = classSubjectService;
            _classService = classService;
            _lessonService = lessonService;
            _lessonPartService = cloneLessonPartService;
            _questionService = cloneLessonPartQuestionService;
            _answerService = cloneLessonPartAnswerService;
        }

        public async Task<ClassSubjectEntity> CloneClassSubject(ClassSubjectEntity item, string _userCreate, string orgClassSubjectID)
        {
            _classSubjectService.Collection.InsertOne(item);
            await CloneChapter(new ChapterEntity() { ClassID = item.ClassID, ClassSubjectID = item.ID, ID = "0" }, _userCreate, orgClassSubjectID);
            return item;
        }

        private async Task<ChapterEntity> CloneChapter(ChapterEntity item, string _userCreate, string orgClassSubjectID)
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
                    var new_lesson = _lessonMapping.Clone(o, new LessonEntity());
                    new_lesson.ChapterID = item.ID;
                    new_lesson.CreateUser = _userCreate;
                    new_lesson.Created = DateTime.Now;
                    new_lesson.OriginID = o.ID;
                    await CloneLesson(new_lesson, _userCreate);
                }
            }

            var subChapters = _chapterService.GetSubChapters(orgClassSubjectID, item.OriginID);
            foreach (var o in subChapters)
            {
                var new_chapter = _chapterMapping.Clone(o, new ChapterEntity());
                new_chapter.CourseID = item.CourseID;
                new_chapter.ParentID = item.ID;
                new_chapter.CreateUser = _userCreate;
                new_chapter.Created = DateTime.Now;
                new_chapter.OriginID = o.ID;
                await CloneChapter(new_chapter, _userCreate, orgClassSubjectID);
            }
            return item;
        }

        private async Task CloneLesson(LessonEntity item, string _userCreate)
        {
            _lessonService.CreateQuery().InsertOne(item);

            var parts = _lessonPartService.GetByLessonID(item.OriginID);
            foreach (var _child in parts)
            {
                var _item = _lessonPartMapping.Clone(_child, new CloneLessonPartEntity());
                _item.OriginID = _child.ID;
                _item.Updated = DateTime.Now;
                _item.Created = DateTime.Now;
                _item.ParentID = item.ID;
                _item.ClassID = item.ClassID;
                _item.ClassSubjectID = item.ClassSubjectID;

                await CloneLessonPart(_item, _userCreate);
            }
        }

        private async Task CloneLessonPart(CloneLessonPartEntity item, string _userCreate)
        {
            _lessonPartService.Collection.InsertOne(item);

            var questions = _questionService.GetByPartID(item.OriginID);
            foreach (var _child in questions)
            {
                var _item = _lessonPartQuestionMapping.Clone(_child, new CloneLessonPartQuestionEntity());
                _item.OriginID = _child.ID;
                _item.CreateUser = _userCreate;
                _item.ParentID = item.ID;
                _item.Updated = DateTime.Now;
                _item.Created = DateTime.Now;
                _item.ClassID = item.ClassID;
                _item.ClassSubjectID = item.ClassSubjectID;

                await CloneLessonQuestion(_item, _userCreate);
            };
        }

        private async Task CloneLessonQuestion(CloneLessonPartQuestionEntity item, string _userCreate)
        {
            _questionService.Collection.InsertOne(item);

            var answers = _answerService.GetByQuestionID(item.OriginID);
            foreach (var _child in answers)
            {
                var _item = _lessonPartAnswerMapping.Clone(_child, new CloneLessonPartAnswerEntity());

                _item.OriginID = _child.ID;
                _item.CreateUser = _userCreate;
                _item.ParentID = item.ID;
                _item.Updated = DateTime.Now;
                _item.Created = DateTime.Now;
                _item.ClassID = item.ClassID;
                _item.ClassSubjectID = item.ClassSubjectID;

                await _answerService.Collection.InsertOneAsync(_item);
            };
        }

        internal async Task ChangeLessonPracticeState(LessonEntity lesson)
        {
            if (lesson.ChapterID != "0")
                await IncreaseChapterCounter(lesson.ChapterID, 0, 0, lesson.IsPractice ? 1 : -1);
            else
                await IncreaseClassSubjectCounter(lesson.ClassSubjectID, 0, 0, lesson.IsPractice ? 1 : -1);
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
