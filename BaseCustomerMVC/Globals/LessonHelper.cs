using BaseCustomerEntity.Database;
using Core_v2.Globals;
using MongoDB.Driver;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseCustomerMVC.Globals
{
    public class LessonHelper
    {
        private readonly LessonService _lessonService;
        private readonly CourseLessonService _courseLessonService;

        private readonly LessonScheduleService _lessonScheduleService;
        private readonly CalendarHelper _calendarHelper;

        private readonly LessonPartService _lessonPartService;
        private readonly LessonPartAnswerService _lessonPartAnswerService;
        private readonly LessonPartQuestionService _lessonPartQuestionService;

        private readonly CloneLessonPartService _cloneLessonPartService;
        private readonly CloneLessonPartAnswerService _cloneAnswerService;
        private readonly CloneLessonPartQuestionService _cloneQuestionService;

        private readonly MappingEntity<LessonPartEntity, CloneLessonPartEntity> _lessonPartMapping = new MappingEntity<LessonPartEntity, CloneLessonPartEntity>();
        private readonly MappingEntity<LessonPartQuestionEntity, CloneLessonPartQuestionEntity> _lessonPartQuestionMapping = new MappingEntity<LessonPartQuestionEntity, CloneLessonPartQuestionEntity>();
        private readonly MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity> _lessonPartAnswerMapping = new MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity>();

        private readonly MappingEntity<CloneLessonPartEntity, CloneLessonPartEntity> _cloneLessonPartMapping = new MappingEntity<CloneLessonPartEntity, CloneLessonPartEntity>();
        private readonly MappingEntity<CloneLessonPartQuestionEntity, CloneLessonPartQuestionEntity> _cloneLessonPartQuestionMapping = new MappingEntity<CloneLessonPartQuestionEntity, CloneLessonPartQuestionEntity>();
        private readonly MappingEntity<CloneLessonPartAnswerEntity, CloneLessonPartAnswerEntity> _cloneLessonPartAnswerMapping = new MappingEntity<CloneLessonPartAnswerEntity, CloneLessonPartAnswerEntity>();

        private readonly MappingEntity<CourseLessonEntity, LessonEntity> _courseLessonMapping = new MappingEntity<CourseLessonEntity, LessonEntity>();
        private readonly MappingEntity<LessonEntity, LessonEntity> _lessonMapping = new MappingEntity<LessonEntity, LessonEntity>();

        public LessonHelper(
            LessonService lessonService,
            CourseLessonService courseLessonService,

            LessonScheduleService lessonScheduleService,
            CalendarHelper calendarHelper,

            LessonPartService lessonPartService,
            LessonPartQuestionService lessonPartQuestionService,
            LessonPartAnswerService lessonPartAnswerService,

            CloneLessonPartService cloneLessonPartService,
            CloneLessonPartAnswerService cloneLessonPartAnswerService,
            CloneLessonPartQuestionService cloneLessonPartQuestionService
        )
        {
            _lessonService = lessonService;
            _courseLessonService = courseLessonService;

            _lessonScheduleService = lessonScheduleService;
            _calendarHelper = calendarHelper;

            _lessonPartService = lessonPartService;
            _lessonPartQuestionService = lessonPartQuestionService;
            _lessonPartAnswerService = lessonPartAnswerService;

            _cloneLessonPartService = cloneLessonPartService;
            _cloneAnswerService = cloneLessonPartAnswerService;
            _cloneQuestionService = cloneLessonPartQuestionService;
        }

        public async Task RemoveClassSubjectLesson(string ClassSubjectID)
        {
            var lstask = _lessonService.Collection.DeleteManyAsync(o => o.ClassSubjectID == ClassSubjectID);

            var scids = _lessonScheduleService.Collection.Find(o => o.ClassSubjectID == ClassSubjectID).Project(o => o.ID).ToList();
            if(scids != null && scids.Count() > 0)
                _calendarHelper.RemoveManySchedules(scids);

            var sctask = _lessonScheduleService.Collection.DeleteManyAsync(o => o.ClassSubjectID == ClassSubjectID);
            var cltask = _cloneLessonPartService.Collection.DeleteManyAsync(o => o.ClassSubjectID == ClassSubjectID);
            var cqtask = _cloneQuestionService.Collection.DeleteManyAsync(o => o.ClassSubjectID == ClassSubjectID);
            var catask = _cloneAnswerService.Collection.DeleteManyAsync(o => o.ClassSubjectID == ClassSubjectID);
            await Task.WhenAll(lstask, sctask, cltask, cqtask, catask);
        }

        public async Task RemoveManyClassLessons(string[] ids)
        {
            var lstask = _lessonService.Collection.DeleteManyAsync(o => ids.Contains(o.ClassID));
            
            var scids = _lessonScheduleService.Collection.Find(o => ids.Contains(o.ClassID)).Project(o => o.ID).ToList();
            if (scids != null && scids.Count() > 0)
                _calendarHelper.RemoveManySchedules(scids);

            var sctask = _lessonScheduleService.Collection.DeleteManyAsync(o => ids.Contains(o.ClassID));
            var cltask = _cloneLessonPartService.Collection.DeleteManyAsync(o => ids.Contains(o.ClassID));
            var cqtask = _cloneQuestionService.Collection.DeleteManyAsync(o => ids.Contains(o.ClassID));
            var catask = _cloneAnswerService.Collection.DeleteManyAsync(o => ids.Contains(o.ClassID));
            await Task.WhenAll(lstask, sctask, cltask, cqtask, catask);
        }

        public async Task RemoveSingleLesson(string id)
        {
            var lstask = _lessonService.Collection.DeleteManyAsync(o => o.ID == id);
            var sctask = _lessonScheduleService.Collection.DeleteManyAsync(o => o.LessonID == id);
            var cltask = _cloneLessonPartService.Collection.DeleteManyAsync(o => o.ParentID == id);
            var quizs = _cloneQuestionService.Collection.Find(o => o.LessonID == id).Project(t => t.ID).ToList();
            var cqtask = _cloneQuestionService.Collection.DeleteManyAsync(o => o.LessonID == id);
            Task<DeleteResult> catask = null;
            if (quizs != null && quizs.Count > 0)
            {
                catask = _cloneAnswerService.Collection.DeleteManyAsync(o => quizs.Contains(o.ParentID));
                await Task.WhenAll(lstask, sctask, cltask, cqtask, catask);
            }
            else
                await Task.WhenAll(lstask, sctask, cltask, cqtask);
        }

        #region Copy CourseLesson From CourseLesson
        public async Task CopyCourseLessonFromCourseLesson(CourseLessonEntity orgItem, CourseLessonEntity cloneItem)
        {
            var lesson = _courseLessonMapping.Clone(orgItem, new CourseLessonEntity());
            lesson.CreateUser = cloneItem.CreateUser;
            lesson.OriginID = orgItem.ID;
            lesson.Created = DateTime.Now;
            lesson.Updated = DateTime.Now;
            lesson.CourseID = cloneItem.CourseID;
            lesson.ChapterID = cloneItem.ChapterID;
            lesson.Order = cloneItem.Order;

            _courseLessonService.CreateQuery().InsertOne(lesson);

            var parts = _lessonPartService.CreateQuery().Find(o => o.ParentID == lesson.OriginID);

            foreach (var part in parts.ToEnumerable())
            {
                CloneCourseLessonPart(part, new LessonPartEntity
                {
                    ParentID = lesson.ID,
                    CourseID = lesson.CourseID
                });
            }
        }

        private void CloneCourseLessonPart(LessonPartEntity orgItem, LessonPartEntity cloneItem)
        {
            var item = _lessonPartMapping.Clone(orgItem, new CloneLessonPartEntity());
            item.ParentID = cloneItem.ParentID;
            item.CourseID = cloneItem.CourseID;
            item.OriginID = orgItem.ID;
            item.Created = DateTime.Now;
            item.Updated = DateTime.Now;

            _lessonPartService.Collection.InsertOne(item);

            var questions = _lessonPartQuestionService.GetByPartID(item.OriginID);
            if (questions != null && questions.Count() > 0)
            {
                foreach (var question in questions)
                {
                    CloneCourseQuestion(question, new LessonPartQuestionEntity
                    {
                        ParentID = item.ID,
                        CourseID = item.CourseID
                    });
                }
            }
        }

        private void CloneCourseQuestion(LessonPartQuestionEntity orgItem, LessonPartQuestionEntity cloneItem)
        {
            var item = _lessonPartQuestionMapping.Clone(orgItem, new LessonPartQuestionEntity());

            item.OriginID = orgItem.ID;
            item.ParentID = cloneItem.ParentID;
            item.CourseID = cloneItem.CourseID;
            item.Created = DateTime.Now;
            item.Updated = DateTime.Now;

            _lessonPartQuestionService.Collection.InsertOne(item);

            var list = _lessonPartAnswerService.GetByQuestionID(item.OriginID);
            if (list != null && list.Count() > 0)
            {
                foreach (var answer in list)
                {
                    CloneCourseAnswer(answer, new LessonPartAnswerEntity
                    {
                        ParentID = item.ID,
                        CourseID = item.CourseID
                    });

                }
            }
        }

        private void CloneCourseAnswer(LessonPartAnswerEntity orgItem, LessonPartAnswerEntity cloneItem)
        {
            var item = _lessonPartAnswerMapping.Clone(orgItem, new CloneLessonPartAnswerEntity());
            item.OriginID = orgItem.ID;
            item.ParentID = cloneItem.ParentID;
            item.Created = DateTime.Now;
            item.Updated = DateTime.Now;
            item.CourseID = cloneItem.CourseID;

            _lessonPartAnswerService.Collection.InsertOne(item);
        }
        #endregion

        #region Copy Lesson From CourseLesson
        public async Task CopyLessonFromCourseLesson(CourseLessonEntity orgItem, LessonEntity cloneItem)
        {
            var lesson = _courseLessonMapping.AutoOrtherType(orgItem, new LessonEntity());
            lesson.ChapterID = cloneItem.ChapterID;
            lesson.OriginID = orgItem.ID;
            lesson.ClassID = cloneItem.ClassID;
            lesson.ClassSubjectID = cloneItem.ClassSubjectID;
            lesson.Created = DateTime.Now;
            lesson.Updated = DateTime.Now;
            lesson.ID = null;

            lesson = InitLesson(lesson);

            var lessonParts = _lessonPartService.GetByLessonID(lesson.OriginID).OrderBy(q => q.Order).ThenBy(q => q.ID).ToList();
            if (lessonParts != null && lessonParts.Count() > 0)
            {
                foreach (var lessonpart in lessonParts)
                {
                    CopyPart(lessonpart, new CloneLessonPartEntity
                    {
                        ParentID = lesson.ID,
                        TeacherID = lesson.CreateUser,
                        ClassID = lesson.ClassID,
                        ClassSubjectID = lesson.ClassSubjectID,
                    });
                }
            }
        }

        private void CopyPart(LessonPartEntity orgItem, CloneLessonPartEntity cloneItem)
        {

            var item = _lessonPartMapping.AutoOrtherType(orgItem, new CloneLessonPartEntity());
            item.ParentID = cloneItem.ParentID;
            item.ID = null;
            item.OriginID = orgItem.ID;
            item.TeacherID = cloneItem.TeacherID;
            item.ClassID = cloneItem.ClassID;
            item.ClassSubjectID = cloneItem.ClassSubjectID;

            _cloneLessonPartService.Collection.InsertOne(item);

            var questions = _lessonPartQuestionService.GetByPartID(item.OriginID);
            if (questions != null && questions.Count() > 0)
            {
                foreach (var question in questions)
                {
                    CopyQuestion(question, new CloneLessonPartQuestionEntity
                    {
                        ParentID = item.ID,
                        ClassID = item.ClassID,
                        ClassSubjectID = item.ClassSubjectID,
                        LessonID = item.ParentID
                    });
                }
            }
        }

        private void CopyQuestion(LessonPartQuestionEntity orgItem, CloneLessonPartQuestionEntity cloneItem)
        {
            var item = _lessonPartQuestionMapping.AutoOrtherType(orgItem, new CloneLessonPartQuestionEntity());

            item.OriginID = orgItem.ID;
            item.ParentID = cloneItem.ParentID;
            item.ID = null;
            item.ClassID = cloneItem.ClassID;
            item.ClassSubjectID = cloneItem.ClassSubjectID;
            item.LessonID = cloneItem.LessonID;

            _cloneQuestionService.Collection.InsertOne(item);

            var list = _lessonPartAnswerService.GetByQuestionID(item.OriginID);
            if (list != null && list.Count() > 0)
            {
                foreach (var answer in list)
                {
                    CopyAnswer(answer, new CloneLessonPartAnswerEntity
                    {
                        ParentID = item.ID,
                        ClassID = item.ClassID,
                        ClassSubjectID = item.ClassSubjectID,
                    });

                }
            }
        }

        private void CopyAnswer(LessonPartAnswerEntity orgItem, CloneLessonPartAnswerEntity cloneItem)
        {
            var item = _lessonPartAnswerMapping.AutoOrtherType(orgItem, new CloneLessonPartAnswerEntity());
            item.OriginID = orgItem.ID;
            item.ParentID = cloneItem.ParentID;
            item.ClassID = cloneItem.ClassID;
            item.ClassSubjectID = cloneItem.ClassSubjectID;
            item.Created = DateTime.Now;
            item.Updated = DateTime.Now;
            item.ID = null;

            _cloneAnswerService.Collection.InsertOne(item);
        }
        #endregion

        #region Copy Lesson From Other Lesson
        public async Task CopyLessonFromLesson(LessonEntity orgLesson, LessonEntity cloneItem)
        {
            var lesson = _lessonMapping.Clone(orgLesson, new LessonEntity());
            lesson.ChapterID = cloneItem.ChapterID;
            lesson.OriginID = orgLesson.ID;
            lesson.ClassID = string.IsNullOrEmpty(cloneItem.ClassID) ? orgLesson.ClassID : cloneItem.ClassID;
            lesson.ClassSubjectID = string.IsNullOrEmpty(cloneItem.ClassSubjectID) ? orgLesson.ClassSubjectID : cloneItem.ClassSubjectID;
            lesson.Title = string.IsNullOrEmpty(cloneItem.Title) ? orgLesson.Title : cloneItem.Title;
            lesson.Created = DateTime.Now;
            lesson.Updated = DateTime.Now;
            lesson.Order = cloneItem.Order;
            lesson.ID = null;

            lesson = InitLesson(lesson);

            var lessonParts = _cloneLessonPartService.GetByLessonID(lesson.OriginID).OrderBy(q => q.Order).ThenBy(q => q.ID).ToList();
            if (lessonParts != null && lessonParts.Count() > 0)
            {
                foreach (var lessonpart in lessonParts)
                {
                    CopyClonePart(lessonpart, new CloneLessonPartEntity
                    {
                        ParentID = lesson.ID,
                        TeacherID = lesson.CreateUser,
                        ClassID = lesson.ClassID,
                        ClassSubjectID = lesson.ClassSubjectID,
                    });
                }
            }
        }

        private void CopyClonePart(CloneLessonPartEntity orgItem, CloneLessonPartEntity cloneItem)
        {

            var item = _cloneLessonPartMapping.Clone(orgItem, new CloneLessonPartEntity());
            item.ParentID = cloneItem.ParentID;
            item.ID = null;
            item.OriginID = orgItem.ID;
            item.TeacherID = cloneItem.TeacherID;
            item.Created = DateTime.Now;
            item.Updated = DateTime.Now;
            item.ClassID = cloneItem.ClassID;
            item.ClassSubjectID = cloneItem.ClassSubjectID;

            _cloneLessonPartService.Collection.InsertOne(item);

            var questions = _cloneQuestionService.GetByPartID(item.OriginID);
            if (questions != null && questions.Count() > 0)
            {
                foreach (var question in questions)
                {
                    CopyCloneQuestion(question, new CloneLessonPartQuestionEntity
                    {
                        ParentID = item.ID,
                        ClassID = item.ClassID,
                        ClassSubjectID = item.ClassSubjectID,
                        LessonID = item.ParentID
                    });
                }
            }
        }

        private void CopyCloneQuestion(CloneLessonPartQuestionEntity orgItem, CloneLessonPartQuestionEntity cloneItem)
        {
            var item = _cloneLessonPartQuestionMapping.Clone(orgItem, new CloneLessonPartQuestionEntity());

            item.OriginID = orgItem.ID;
            item.ParentID = cloneItem.ParentID;
            item.ID = null;
            item.ClassID = cloneItem.ClassID;
            item.ClassSubjectID = cloneItem.ClassSubjectID;
            item.LessonID = cloneItem.LessonID;
            item.Created = DateTime.Now;
            item.Updated = DateTime.Now;

            _cloneQuestionService.Collection.InsertOne(item);

            var list = _cloneAnswerService.GetByQuestionID(item.OriginID);
            if (list != null && list.Count() > 0)
            {
                foreach (var answer in list)
                {
                    CopyCloneAnswer(answer, new CloneLessonPartAnswerEntity
                    {
                        ParentID = item.ID,
                        ClassID = item.ClassID,
                        ClassSubjectID = item.ClassSubjectID,
                    });
                }
            }
        }

        private void CopyCloneAnswer(CloneLessonPartAnswerEntity orgItem, CloneLessonPartAnswerEntity cloneItem)
        {
            var item = _cloneLessonPartAnswerMapping.Clone(orgItem, new CloneLessonPartAnswerEntity());
            item.OriginID = orgItem.ID;
            item.ParentID = cloneItem.ParentID;
            item.ClassID = cloneItem.ClassID;
            item.ClassSubjectID = cloneItem.ClassSubjectID;
            item.Created = DateTime.Now;
            item.Updated = DateTime.Now;

            _cloneAnswerService.Collection.InsertOne(item);
        }
        #endregion

        public LessonEntity InitLesson(LessonEntity lesson)
        {
            _lessonService.Save(lesson);
            var schedule = new LessonScheduleEntity
            {
                ClassID = lesson.ClassID,
                ClassSubjectID = lesson.ClassSubjectID,
                LessonID = lesson.ID,
                Type = lesson.TemplateType,
                IsActive = true
            };
            _lessonScheduleService.Save(schedule);
            _calendarHelper.ConvertCalendarFromSchedule(schedule, "");
            return lesson;
        }


        public async Task ConvertClassSubject(ClassSubjectEntity classSubject)
        {
            var cltask = _cloneLessonPartService.Collection.UpdateManyAsync(t => t.ClassID == classSubject.ClassID, Builders<CloneLessonPartEntity>.Update.Set("ClassSubjectID", classSubject.ID));
            var cqtask = _cloneQuestionService.Collection.UpdateManyAsync(t => t.ClassID == classSubject.ClassID, Builders<CloneLessonPartQuestionEntity>.Update.Set("ClassSubjectID", classSubject.ID));
            var catask = _cloneAnswerService.Collection.UpdateManyAsync(t => t.ClassID == classSubject.ClassID, Builders<CloneLessonPartAnswerEntity>.Update.Set("ClassSubjectID", classSubject.ID));
            await Task.WhenAll(cltask, cqtask, catask);
        }
    }
}
