using BaseCustomerEntity.Database;
using Core_v2.Globals;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseCustomerMVC.Globals
{
    public class LessonHelper
    {
        private readonly LessonService _lessonService;

        private readonly LessonPartService _lessonPartService;
        private readonly LessonPartAnswerService _lessonPartAnswerService;
        private readonly LessonPartQuestionService _lessonPartQuestionService;

        private readonly CloneLessonPartService _cloneLessonPartService;
        private readonly CloneLessonPartAnswerService _cloneAnswerService;
        private readonly CloneLessonPartQuestionService _cloneQuestionService;

        private readonly MappingEntity<LessonPartEntity, CloneLessonPartEntity> _lessonPartMapping;
        private readonly MappingEntity<LessonPartQuestionEntity, CloneLessonPartQuestionEntity> _lessonPartQuestionMapping;
        private readonly MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity> _lessonPartAnswerMapping;

        public LessonHelper(
            LessonService lessonService,

            LessonPartService lessonPartService,
            LessonPartQuestionService lessonPartQuestionService,
            LessonPartAnswerService lessonPartAnswerService,

            CloneLessonPartService cloneLessonPartService,
            CloneLessonPartAnswerService cloneLessonPartAnswerService,
            CloneLessonPartQuestionService cloneLessonPartQuestionService
        )
        {
            _lessonService = lessonService;

            _lessonPartService = lessonPartService;
            _lessonPartQuestionService = lessonPartQuestionService;
            _lessonPartAnswerService = lessonPartAnswerService;

            _cloneLessonPartService = cloneLessonPartService;
            _cloneAnswerService = cloneLessonPartAnswerService;
            _cloneQuestionService = cloneLessonPartQuestionService;

            _lessonPartMapping = new MappingEntity<LessonPartEntity, CloneLessonPartEntity>();
            _lessonPartQuestionMapping = new MappingEntity<LessonPartQuestionEntity, CloneLessonPartQuestionEntity>();
            _lessonPartAnswerMapping = new MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity>();
        }

        public void RemoveClone(string id)
        {
            _cloneLessonPartService.Collection.DeleteMany(o => o.ClassID == id);
            _cloneQuestionService.Collection.DeleteMany(o => o.ClassID == id);
            _cloneAnswerService.Collection.DeleteMany(o => o.ClassID == id);
        }

        public void RemoveClone(string[] ids)
        {
            _cloneLessonPartService.Collection.DeleteMany(o => ids.Contains(o.ClassID));
            _cloneQuestionService.Collection.DeleteMany(o => ids.Contains(o.ClassID));
            _cloneAnswerService.Collection.DeleteMany(o => ids.Contains(o.ClassID));
        }

        //Clone Lesson
        //Clone Lesson
        public void CloneLessonForClass(LessonEntity lesson, ClassSubjectEntity @class)
        {
            var listLessonPart = _lessonPartService.CreateQuery().Find(o => o.ParentID == lesson.ID).SortBy(q => q.Order).ThenBy(q => q.ID).ToList();
            if (listLessonPart != null && listLessonPart.Count > 0)
            {
                if (_cloneLessonPartService.CreateQuery().CountDocuments(
                    o => o.ParentID == lesson.ID &&
                    o.TeacherID == @class.TeacherID && o.ClassID == @class.ID) == 0)
                {
                    foreach (var lessonpart in listLessonPart)
                    {
                        var clonepart = _lessonPartMapping.AutoOrtherType(lessonpart, new CloneLessonPartEntity());
                        clonepart.ID = null;
                        clonepart.OriginID = lessonpart.ID;
                        clonepart.TeacherID = @class.TeacherID;
                        clonepart.ClassID = @class.ID;
                        CloneLessonPart(clonepart);
                    }
                }
            }
        }

        private void CloneLessonPart(CloneLessonPartEntity item)
        {
            _cloneLessonPartService.Collection.InsertOne(item);
            var list = _lessonPartQuestionService.CreateQuery().Find(o => o.ParentID == item.OriginID).ToList();
            if (list != null)
            {
                foreach (var question in list)
                {
                    var cloneQuestion = _lessonPartQuestionMapping.AutoOrtherType(question, new CloneLessonPartQuestionEntity());
                    cloneQuestion.OriginID = question.ID;
                    cloneQuestion.ParentID = item.ID;
                    cloneQuestion.ID = null;
                    cloneQuestion.ClassID = item.ClassID;
                    cloneQuestion.LessonID = item.ParentID;
                    CloneLessonQuestion(cloneQuestion);
                }
            }
        }

        private void CloneLessonQuestion(CloneLessonPartQuestionEntity item)
        {
            _cloneQuestionService.Collection.InsertOne(item);
            var list = _lessonPartAnswerService.CreateQuery().Find(o => o.ParentID == item.OriginID).ToList();
            if (list != null)
            {
                foreach (var answer in list)
                {
                    var cloneAnswer = _lessonPartAnswerMapping.AutoOrtherType(answer, new CloneLessonPartAnswerEntity());
                    cloneAnswer.OriginID = answer.ID;
                    cloneAnswer.ParentID = item.ID;
                    cloneAnswer.ID = null;
                    cloneAnswer.ClassID = item.ClassID;
                    CloneLessonAnswer(cloneAnswer);
                }
            }
        }

        private void CloneLessonAnswer(CloneLessonPartAnswerEntity item)
        {
            _cloneAnswerService.Collection.InsertOne(item);
        }

    }
}
