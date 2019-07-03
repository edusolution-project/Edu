using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using MongoDB.Driver;
using System.Text;
using System.Linq;
using Core_v2.Globals;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class CloneLessonPartController : TeacherController
    {
        private readonly GradeService _gradeService;
        private readonly SubjectService _subjectService;
        private readonly TeacherService _teacherService;
        private readonly ClassService _classService;
        private readonly CourseService _courseService;
        private readonly ChapterService _chapterService;
        private readonly LessonService _lessonService;
        private readonly LessonScheduleService _lessonScheduleService;

        private readonly LessonPartService _lessonPartService;
        private readonly LessonPartAnswerService _lessonPartAnswerService;
        private readonly LessonPartQuestionService _lessonPartQuestionService;

        private readonly CloneLessonPartService _service;
        private readonly CloneLessonPartAnswerService _cloneLessonPartAnswerService;
        private readonly CloneLessonPartQuestionService _cloneLessonPartQuestionService;
        private readonly MappingEntity<LessonPartEntity, CloneLessonPartEntity> _lessonpartMapping;
        private readonly MappingEntity<LessonPartQuestionEntity, CloneLessonPartQuestionEntity> _lessonpartQuestionMapping;
        private readonly MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity> _lessonpartAnswerMapping;


        public CloneLessonPartController(
            GradeService gradeservice,
            SubjectService subjectService,
            TeacherService teacherService,
            ClassService classService,
            CourseService courseService,
            ChapterService chapterService,
            LessonService lessonService,
            LessonScheduleService lessonScheduleService,
            CloneLessonPartService service,
            CloneLessonPartAnswerService cloneLessonPartAnswerService,
            CloneLessonPartQuestionService cloneLessonPartQuestionService,
            MappingEntity<LessonPartEntity, CloneLessonPartEntity> lessonpartMapping,
            MappingEntity<LessonPartQuestionEntity, CloneLessonPartQuestionEntity> lessonpartQuestionMapping,
            MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity> lessonpartAnswerMapping
            )
        {
            _gradeService = gradeservice;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _courseService = courseService;
            _classService = classService;
            _chapterService = chapterService;
            _lessonService = lessonService;
            _lessonScheduleService = lessonScheduleService;
            _cloneLessonPartAnswerService = cloneLessonPartAnswerService;
            _cloneLessonPartQuestionService = cloneLessonPartQuestionService;
            _service = service;
            _lessonpartMapping = lessonpartMapping;
            _lessonpartQuestionMapping = lessonpartQuestionMapping;
            _lessonpartAnswerMapping = lessonpartAnswerMapping;
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetListPart(string LessonID)
        {
            var root = _lessonService.CreateQuery().Find(o => o.ID == LessonID).SingleOrDefault();
            var data = new Dictionary<string, object> { };

            if (root != null)
            {
                var listCloneLessonPart = _service.CreateQuery().Find(o => o.ParentID == LessonID).SortBy(q => q.Order).ThenBy(q => q.ID).ToList();
                if (listCloneLessonPart != null && listCloneLessonPart.Count > 0)
                {
                    var result = new List<CloneLessonPartViewModel>();
                    result.AddRange(listCloneLessonPart.Select(o => new CloneLessonPartViewModel(o)
                    {
                        Questions = _cloneLessonPartQuestionService.CreateQuery().Find(q => q.ParentID == o.ID).SortBy(q => q.Order).ThenBy(q => q.ID).ToList().Select(q => new CloneQuestionViewModel(q)
                        {
                            CloneAnswers = _cloneLessonPartAnswerService.CreateQuery().Find(a => a.ParentID == q.ID).SortBy(a => a.Order).ThenBy(a => a.ID).ToList()
                        }).ToList()
                    }));
                }
                else
                {
                    //Clone from lesson part
                    var listLessonPart = _lessonPartService.CreateQuery().Find(o => o.ParentID == LessonID).SortBy(q => q.Order).ThenBy(q => q.ID).ToList();
                    if (listLessonPart != null && listLessonPart.Count > 0)
                    {
                        foreach (var lessonpart in listLessonPart)
                            CloneLessonPart(_lessonpartMapping.AutoOrtherType(lessonpart, new CloneLessonPartEntity()
                            {
                                OriginID = lessonpart.ID,
                                ID = null
                            }));
                    }
                }
            }

            var respone = new Dictionary<string, object>
            {
                { "Data", data }
            };
            return new JsonResult(respone);
        }

        private void CloneLessonPart(CloneLessonPartEntity item)
        {
            var _userCreate = User.Claims.GetClaimByType("UserID").Value;
            _lessonPartService.Collection.InsertOne(item);
            var list = _lessonPartQuestionService.CreateQuery().Find(o => o.ParentID == item.OriginID).ToList();
            if (list != null)
            {
                foreach (var question in list)
                {
                    CloneLessonQuestion(_lessonpartQuestionMapping.AutoOrtherType(question, new CloneLessonPartQuestionEntity()
                    {
                        OriginID = question.ID,
                        ID = null
                    }));
                }
            }
        }

        private void CloneLessonQuestion(CloneLessonPartQuestionEntity item)
        {
            var _userCreate = User.Claims.GetClaimByType("UserID").Value;
            _lessonPartQuestionService.Collection.InsertOne(item);
            var list = _lessonPartAnswerService.CreateQuery().Find(o => o.ParentID == item.OriginID).ToList();
            if (list != null)
            {
                foreach (var answer in list)
                {
                    CloneLessonAnswer(_lessonpartAnswerMapping.AutoOrtherType(answer, new CloneLessonPartAnswerEntity()
                    {
                        OriginID = answer.ID,
                        ID = null
                    }));
                }
            }
        }

        private void CloneLessonAnswer(CloneLessonPartAnswerEntity item)
        {
            _lessonPartAnswerService.Collection.InsertOne(item);
        }
    }
}
