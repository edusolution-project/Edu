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

namespace BaseCustomerMVC.Controllers.Student
{
    public class CloneLessonPartController : StudentController
    {
        private readonly GradeService _gradeService;
        private readonly SubjectService _subjectService;
        private readonly TeacherService _teacherService;
        private readonly ClassService _classService;
        private readonly CourseService _courseService;
        private readonly ChapterService _chapterService;
        private readonly LessonService _lessonService;
        //private readonly LessonScheduleService _lessonScheduleService;

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
            //LessonScheduleService lessonScheduleService,
            LessonPartService lessonPartService,
            LessonPartQuestionService lessonPartQuestionService,
            LessonPartAnswerService lessonPartAnswerService,
            CloneLessonPartService service,
            CloneLessonPartAnswerService cloneLessonPartAnswerService,
            CloneLessonPartQuestionService cloneLessonPartQuestionService

            )
        {
            _gradeService = gradeservice;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _courseService = courseService;
            _classService = classService;
            _chapterService = chapterService;
            _lessonService = lessonService;
            //_lessonScheduleService = lessonScheduleService;
            _lessonPartService = lessonPartService;
            _lessonPartQuestionService = lessonPartQuestionService;
            _lessonPartAnswerService = lessonPartAnswerService;

            _service = service;
            _cloneLessonPartAnswerService = cloneLessonPartAnswerService;
            _cloneLessonPartQuestionService = cloneLessonPartQuestionService;

            _lessonpartMapping = new MappingEntity<LessonPartEntity, CloneLessonPartEntity>();
            _lessonpartQuestionMapping = new MappingEntity<LessonPartQuestionEntity, CloneLessonPartQuestionEntity>();
            _lessonpartAnswerMapping = new MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity>();
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetList(string LessonID, string ClassID)
        {
            var root = _lessonService.CreateQuery().Find(o => o.ID == LessonID).SingleOrDefault();
            var data = new List<CloneLessonPartViewModel>();

            var currentClass = _classService.CreateQuery().Find(o => o.ID == ClassID).SingleOrDefault();

            if (root != null && currentClass != null)
            {
                var listCloneLessonPart = _service.CreateQuery().Find(o => o.ParentID == LessonID && o.TeacherID == currentClass.TeacherID).SortBy(q => q.Order).ThenBy(q => q.ID).ToList();
                if (listCloneLessonPart != null && listCloneLessonPart.Count > 0)
                {
                    data.AddRange(listCloneLessonPart.Select(o => new CloneLessonPartViewModel(o)
                    {
                        Questions = _cloneLessonPartQuestionService.CreateQuery().Find(q => q.ParentID == o.ID).SortBy(q => q.Order).ThenBy(q => q.ID).ToList().Select(q => new CloneQuestionViewModel(q)
                        {
                            Answers = _cloneLessonPartAnswerService.CreateQuery().Find(a => a.ParentID == q.ID).SortBy(a => a.Order).ThenBy(a => a.ID).ToList()
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
                        {
                            var clonepart = _lessonpartMapping.AutoOrtherType(lessonpart, new CloneLessonPartEntity());
                            clonepart.ID = null;
                            clonepart.OriginID = lessonpart.ID;
                            clonepart.TeacherID = currentClass.TeacherID;
                            CloneLessonPart(clonepart);
                        }

                        listCloneLessonPart = _service.CreateQuery().Find(o => o.ParentID == LessonID && o.TeacherID == currentClass.TeacherID).SortBy(q => q.Order).ThenBy(q => q.ID).ToList();

                        data.AddRange(listCloneLessonPart.Select(o => new CloneLessonPartViewModel(o)
                        {
                            Questions = _cloneLessonPartQuestionService.CreateQuery().Find(q => q.ParentID == o.ID).SortBy(q => q.Order).ThenBy(q => q.ID).ToList().Select(q => new CloneQuestionViewModel(q)
                            {
                                Answers = _cloneLessonPartAnswerService.CreateQuery().Find(a => a.ParentID == q.ID).SortBy(a => a.Order).ThenBy(a => a.ID).ToList()
                            }).ToList()
                        }));
                    }
                }
            }

            var response = new Dictionary<string, object>
            {
                { "Data", data }
            };
            return new JsonResult(response);
        }

        private void CloneLessonPart(CloneLessonPartEntity item)
        {
            var _userCreate = User.Claims.GetClaimByType("UserID").Value;
            _service.Collection.InsertOne(item);
            var list = _lessonPartQuestionService.CreateQuery().Find(o => o.ParentID == item.OriginID).ToList();
            if (list != null)
            {
                foreach (var question in list)
                {
                    var cloneQuestion = _lessonpartQuestionMapping.AutoOrtherType(question, new CloneLessonPartQuestionEntity());
                    cloneQuestion.OriginID = question.ID;
                    cloneQuestion.ParentID = item.ID;
                    cloneQuestion.ID = null;
                    CloneLessonQuestion(cloneQuestion);
                }
            }
        }

        private void CloneLessonQuestion(CloneLessonPartQuestionEntity item)
        {
            var _userCreate = User.Claims.GetClaimByType("UserID").Value;
            _cloneLessonPartQuestionService.Collection.InsertOne(item);
            var list = _lessonPartAnswerService.CreateQuery().Find(o => o.ParentID == item.OriginID).ToList();
            if (list != null)
            {
                foreach (var answer in list)
                {
                    var cloneAnswer = _lessonpartAnswerMapping.AutoOrtherType(answer, new CloneLessonPartAnswerEntity());
                    cloneAnswer.OriginID = answer.ID;
                    cloneAnswer.ParentID = item.ID;
                    cloneAnswer.ID = null;
                    CloneLessonAnswer(cloneAnswer);
                }
            }
        }

        private void CloneLessonAnswer(CloneLessonPartAnswerEntity item)
        {
            _cloneLessonPartAnswerService.Collection.InsertOne(item);
        }
    }
}
