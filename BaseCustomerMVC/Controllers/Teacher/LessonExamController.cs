using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Core_v2.Globals;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class LessonExamController : TeacherController
    {
        private readonly LessonExamService _lessonExamService;
        private readonly CloneLessonPartExtensionService _lessonPartExtensionService;
        private readonly CloneLessonPartQuestionExtensionService _lessonPartQuestionExtensionService;
        private readonly CloneLessonPartAnswerExtensionService _lessonPartAnswerExtensionService;
        public LessonExamController(
            LessonExamService lessonExamService
            , CloneLessonPartExtensionService lessonPartExtensionService
            , CloneLessonPartQuestionExtensionService lessonPartQuestionExtensionService
            , CloneLessonPartAnswerExtensionService lessonPartAnswerExtensionService
        )
        {
            _lessonExamService = lessonExamService;
            _lessonPartExtensionService = lessonPartExtensionService;
            _lessonPartQuestionExtensionService = lessonPartQuestionExtensionService;
            _lessonPartAnswerExtensionService = lessonPartAnswerExtensionService;
        }

        public JsonResult GetList(String LessonExamID,String basis)
        {
            try
            {
                var root = _lessonExamService.CreateQuery().Find(o => o.ID == LessonExamID).SingleOrDefault();
                var result = new List<CloneLessonPartViewModel>();
                if (root != null)
                {
                    var listPart = _lessonPartExtensionService.CreateQuery().Find(x=>x.LessonExamID == LessonExamID).SortBy(q => q.Order).ThenBy(q => q.ID).ToList();
                    
                    if ( listPart != null && listPart.Count > 0)
                    {
                        foreach (var part in listPart)
                        {
                            switch (part.Type)
                            {
                                case "QUIZ1":
                                case "QUIZ2":
                                case "QUIZ3":
                                case "QUIZ4":
                                //case "ESSAY":
                                    result.Add(new CloneLessonPartViewModel(part)
                                    {
                                        Questions = _lessonPartQuestionExtensionService.CreateQuery().Find(q => q.ParentID == part.ID).SortBy(q => q.Order).ThenBy(q => q.ID).ToList().Select(q => new CloneQuestionViewModel(q)
                                        {
                                            Answers = _lessonPartAnswerExtensionService.ConvertToCloneLessonPartAns(_lessonPartAnswerExtensionService.CreateQuery().Find(a => a.ParentID == q.ID).SortBy(a => a.Order).ThenBy(a => a.ID).ToList())
                                        }).ToList()
                                    });
                                    break;
                                //case "VOCAB":
                                //    result.Add(new CloneLessonPartViewModel(part)
                                //    {
                                //        Description = RenderVocab(part.Description)
                                //    });
                                //    break;
                                default:
                                    result.Add(new CloneLessonPartViewModel(part));
                                    break;
                            }
                        }
                        
                    }
                }
                return Json(new Dictionary<String, Object>
                {
                    {"Status",true },
                    {"Data",result },
                    {"Msg","" }
                });
            }
            catch(Exception ex)
            {
                return Json(new Dictionary<String, Object> {
                    {"Status",false },
                    {"Msg",ex.Message }
                });
            }
        }
        public JsonResult GetDetailsLesson(string ID)
        {
            try
            {
                var lesson = _lessonExamService.CreateQuery().Find(o => o.ID == ID).FirstOrDefault();

                var response = new Dictionary<string, object>
                {
                    { "Data", lesson }
                };
                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", null },
                    {"Error", ex.Message }
                });
            }
        }

    }
}
