﻿using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using MongoDB.Driver;
using System.Text;
using System.Linq;
using Core_v2.Globals;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using MongoDB.Bson.Serialization.Serializers;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class CloneLessonPartController : TeacherController
    {
        private readonly GradeService _gradeService;
        private readonly SubjectService _subjectService;
        private readonly TeacherService _teacherService;
        private readonly ClassService _classService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly CourseService _courseService;
        private readonly ChapterService _chapterService;
        private readonly LessonService _lessonService;
        private readonly LessonScheduleService _lessonScheduleService;

        private readonly LessonPartService _lessonPartService;
        private readonly LessonPartAnswerService _lessonPartAnswerService;
        private readonly LessonPartQuestionService _lessonPartQuestionService;

        private readonly CloneLessonPartService _service;
        private readonly CloneLessonPartAnswerService _cloneAnswerService;
        private readonly CloneLessonPartQuestionService _cloneQuestionService;

        private readonly MappingEntity<LessonPartEntity, CloneLessonPartEntity> _lessonpartMapping;
        private readonly MappingEntity<LessonPartQuestionEntity, CloneLessonPartQuestionEntity> _lessonpartQuestionMapping;
        private readonly MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity> _lessonpartAnswerMapping;

        private readonly FileProcess _fileProcess;

        public CloneLessonPartController(
            GradeService gradeservice,
            SubjectService subjectService,
            TeacherService teacherService,
            ClassService classService,
            ClassSubjectService classSubjectService,
            CourseService courseService,
            ChapterService chapterService,
            LessonService lessonService,
            LessonScheduleService lessonScheduleService,
            LessonPartService lessonPartService,
            LessonPartQuestionService lessonPartQuestionService,
            LessonPartAnswerService lessonPartAnswerService,
            CloneLessonPartService service,
            CloneLessonPartAnswerService cloneLessonPartAnswerService,
            CloneLessonPartQuestionService cloneLessonPartQuestionService,
            FileProcess fileProcess
            )
        {
            _gradeService = gradeservice;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _courseService = courseService;
            _classSubjectService = classSubjectService;
            _classService = classService;
            _chapterService = chapterService;
            _lessonService = lessonService;
            _lessonScheduleService = lessonScheduleService;
            _lessonPartService = lessonPartService;
            _lessonPartQuestionService = lessonPartQuestionService;
            _lessonPartAnswerService = lessonPartAnswerService;

            _service = service;
            _cloneAnswerService = cloneLessonPartAnswerService;
            _cloneQuestionService = cloneLessonPartQuestionService;

            _lessonpartMapping = new MappingEntity<LessonPartEntity, CloneLessonPartEntity>();
            _lessonpartQuestionMapping = new MappingEntity<LessonPartQuestionEntity, CloneLessonPartQuestionEntity>();
            _lessonpartAnswerMapping = new MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity>();
            _fileProcess = fileProcess;
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetList(string LessonID, string ClassID, string ClassSubjectID)
        {
            var root = _lessonService.CreateQuery().Find(o => o.ID == LessonID).SingleOrDefault();
            var data = new List<CloneLessonPartViewModel>();

            //var currentClass = _classService.CreateQuery().Find(o => o.ID == ClassID).SingleOrDefault();



            var currentCs = _classSubjectService.GetItemByID(ClassSubjectID);
            if (currentCs == null)
                return new JsonResult(
                new Dictionary<string, object> { { "Error", "Subject not found" } });

            var currentClass = _classService.GetItemByID(ClassID);
            if (currentClass == null)
                return new JsonResult(
                new Dictionary<string, object> { { "Error", "Class not found" } });

            if (root != null && currentClass != null)
            {
                var listCloneLessonPart = _service.CreateQuery().Find(o => o.ParentID == LessonID && o.ClassSubjectID == currentCs.ID).SortBy(q => q.Order).ThenBy(q => q.ID).ToList();
                if (listCloneLessonPart != null && listCloneLessonPart.Count > 0)
                {
                    data.AddRange(listCloneLessonPart.Select(o => new CloneLessonPartViewModel(o)
                    {
                        Questions = _cloneQuestionService.CreateQuery().Find(q => q.ParentID == o.ID).SortBy(q => q.Order).ThenBy(q => q.ID).ToList().Select(q => new CloneQuestionViewModel(q)
                        {
                            Answers = _cloneAnswerService.CreateQuery().Find(a => a.ParentID == q.ID).SortBy(a => a.Order).ThenBy(a => a.ID).ToList()
                        }).ToList()
                    }));

                }
                //else //TODO: TEMPORARY USE - REMOVE LATER
                //{
                //    //Clone from lesson part - Temporary
                //    var listLessonPart = _lessonPartService.CreateQuery().Find(o => o.ParentID == LessonID).SortBy(q => q.Order).ThenBy(q => q.ID).ToList();
                //    if (listLessonPart != null && listLessonPart.Count > 0)
                //    {
                //        foreach (var lessonpart in listLessonPart)
                //        {
                //            var clonepart = _lessonpartMapping.AutoOrtherType(lessonpart, new CloneLessonPartEntity());
                //            clonepart.ID = null;
                //            clonepart.OriginID = lessonpart.ID;
                //            clonepart.TeacherID = currentClass.TeacherID;
                //            clonepart.ClassID = currentClass.ID;
                //            CloneLessonPart(clonepart);
                //        }

                //        listCloneLessonPart = _service.CreateQuery().Find(o => o.ParentID == LessonID && o.ClassID == currentClass.ID).SortBy(q => q.Order).ThenBy(q => q.ID).ToList();

                //        data.AddRange(listCloneLessonPart.Select(o => new CloneLessonPartViewModel(o)
                //        {
                //            Questions = _cloneQuestionService.CreateQuery().Find(q => q.ParentID == o.ID).SortBy(q => q.Order).ThenBy(q => q.ID).ToList().Select(q => new CloneQuestionViewModel(q)
                //            {
                //                Answers = _cloneAnswerService.CreateQuery().Find(a => a.ParentID == q.ID).SortBy(a => a.Order).ThenBy(a => a.ID).ToList()
                //            }).ToList()
                //        }));
                //    }
                //}
            }

            var response = new Dictionary<string, object>
            {
                { "Data", data }
            };
            return new JsonResult(response);
        }

        [HttpPost]
        public JsonResult GetDetail(string ID)
        {
            var part = _service.CreateQuery().Find(o => o.ID == ID).SingleOrDefault();
            if (part == null) return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", null },
                        {"Error", "Data not found" }
                    });
            var full_item = new CloneLessonPartViewModel(part)
            {
                Questions = _cloneQuestionService.CreateQuery().Find(o => o.ParentID == part.ID).SortBy(o => o.Order).ThenBy(o => o.ID).ToList().Select(t =>
                      new CloneQuestionViewModel(t)
                      {
                          Answers = _cloneAnswerService.CreateQuery().Find(a => a.ParentID == t.ID).SortBy(o => o.Order).ThenBy(o => o.ID).ToList()
                      }).ToList()
            };

            //if(full_item.Questions != null && full_item.Questions.Count > 0)
            //{
            //    foreach(var quiz in full_item.Questions)
            //    {
            //        var ans = _cloneAnswerService.CreateQuery().Find(a => a.ParentID == quiz.ID).SortBy(o => o.Order).ThenBy(o => o.ID).ToList();

            //        quiz.Answers = ans;
            //    }    
            //}    

            return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", full_item },
                        {"Error", null }
                    });

        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<JsonResult> CreateOrUpdate(CloneLessonPartViewModel item, string ClassSubjectID, List<string> RemovedQuestions = null, List<string> RemovedAnswers = null)
        {
            var _userCreate = User.Claims.GetClaimByType("UserID").Value;
            var root = _lessonService.GetItemByID(item.ParentID);
            var currentCs = _classSubjectService.GetItemByID(ClassSubjectID);

            if (root == null || currentCs == null)
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", null },
                    {"Error", "Parent Item Not found" }
                });
            }

            if (item.Media != null && item.Media.Name == null) item.Media = null;//valid Media
            var files = HttpContext.Request.Form != null && HttpContext.Request.Form.Files.Count > 0 ? HttpContext.Request.Form.Files : null;
            if (item.ID == "0" || item.ID == null) //create
            {
                item.Created = DateTime.Now;
                item.TeacherID = currentCs.TeacherID;
                var maxItem = _service.CreateQuery()
                    .Find(o => o.ParentID == item.ParentID)
                    .SortByDescending(o => o.Order).FirstOrDefault();
                item.Order = maxItem != null ? maxItem.Order + 1 : 0;
                if (item.Media == null || string.IsNullOrEmpty(item.Media.Name) || files == null || !files.Any(f => f.Name == item.Media.Name))
                {
                    item.Media = null;
                }
                else
                {
                    var file = files.Where(f => f.Name == item.Media.Name).SingleOrDefault();
                    if (file != null)
                    {
                        item.Media.Created = DateTime.Now;
                        item.Media.Size = file.Length;
                        item.Media.Path = await _fileProcess.SaveMediaAsync(file, item.Media.OriginalName);
                    }
                }
            }
            else // Update
            {
                var olditem = _service.GetItemByID(item.ID);
                if (olditem == null)
                    return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", null },
                                {"Error", "Item Not Found" }
                            });

                if ((olditem.Media != null && item.Media != null && olditem.Media.Path != item.Media.Path)
                    || (olditem.Media == null && item.Media != null))//Media change
                {
                    if (files == null || !files.Any(f => f.Name == item.Media.Name))
                        return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", null },
                                {"Error", "Upload Fails" }
                            });

                    var file = files.Where(f => f.Name == item.Media.Name).SingleOrDefault();//update media
                    item.Media.Created = DateTime.Now;
                    item.Media.Size = file.Length;
                    item.Media.Path = await _fileProcess.SaveMediaAsync(file, item.Media.OriginalName);
                }
                item.OriginID = olditem.OriginID;
                item.Created = olditem.Created;
                item.Order = olditem.Order;
            }

            item.Updated = DateTime.Now;
            item.ClassID = currentCs.ClassID;
            item.ClassSubjectID = currentCs.ID;
            item.CourseID = currentCs.CourseID;

            var lessonpart = item.ToEntity();
            _service.Save(lessonpart);

            item.ID = lessonpart.ID;

            switch (item.Type)
            {
                case "QUIZ2": //remove all previous question
                    var oldQuizIds = _cloneQuestionService.CreateQuery().Find(q => q.ParentID == item.ID).Project(i => i.ID).ToEnumerable();
                    foreach (var quizid in oldQuizIds)
                        _cloneAnswerService.CreateQuery().DeleteMany(a => a.ParentID == quizid);
                    _cloneQuestionService.CreateQuery().DeleteMany(q => q.ParentID == item.ID);
                    break;
                default:
                    if (RemovedQuestions != null & RemovedQuestions.Count > 0)
                    {
                        _ = _cloneQuestionService.RemoveManyAsync(RemovedQuestions);

                        foreach (var quizID in RemovedQuestions)
                        {
                            _ = _cloneAnswerService.RemoveByParentAsync(quizID);
                        }
                    }

                    if (RemovedAnswers != null & RemovedAnswers.Count > 0)
                        _ = _cloneAnswerService.RemoveManyAsync(RemovedAnswers);
                    break;
            }

            if (item.Questions != null && item.Questions.Count > 0)
            {
                foreach (var questionVM in item.Questions)
                {
                    questionVM.ParentID = item.ID;
                    questionVM.CreateUser = _userCreate;
                    var quiz = questionVM.ToEntity();

                    if (questionVM.Media != null && questionVM.Media.Name == null) questionVM.Media = null;

                    if (questionVM.ID == "0" || questionVM.ID == null || _cloneQuestionService.GetItemByID(quiz.ID) == null)
                    {
                        var maxItem = _cloneQuestionService.CreateQuery()
                            .Find(o => o.ParentID == lessonpart.ID)
                            .SortByDescending(o => o.Order).FirstOrDefault();
                        quiz.Order = maxItem != null ? maxItem.Order + 1 : 0;
                        quiz.Created = DateTime.Now;

                        if (quiz.Media == null || string.IsNullOrEmpty(quiz.Media.Name) || !files.Any(f => f.Name == quiz.Media.Name))
                            quiz.Media = null;
                        else
                        {
                            var file = files.Where(f => f.Name == quiz.Media.Name).SingleOrDefault();
                            if (file != null)
                            {
                                quiz.Media.Created = DateTime.Now;
                                quiz.Media.Size = file.Length;
                                quiz.Media.Path = await _fileProcess.SaveMediaAsync(file, quiz.Media.OriginalName);
                            }
                        }
                    }
                    else
                    {
                        var oldquiz = _cloneQuestionService.GetItemByID(quiz.ID);
                        if ((oldquiz.Media != null && quiz.Media != null && oldquiz.Media.Path != quiz.Media.Path)
                            || (oldquiz.Media == null && quiz.Media != null))//Media change
                        {

                            if (oldquiz.Media != null && !string.IsNullOrEmpty(oldquiz.Media.Path))//Delete old file
                                _fileProcess.DeleteFile(oldquiz.Media.Path);

                            if (files == null || !files.Any(f => f.Name == quiz.Media.Name))
                                quiz.Media = null;
                            else
                            {
                                var file = files.Where(f => f.Name == quiz.Media.Name).SingleOrDefault();//update media
                                quiz.Media.Created = DateTime.Now;
                                quiz.Media.Size = file.Length;
                                quiz.Media.Path = await _fileProcess.SaveMediaAsync(file, quiz.Media.OriginalName);
                            }
                        }

                        quiz.Order = oldquiz.Order;
                        quiz.Created = oldquiz.Created;

                    }

                    quiz.Updated = DateTime.Now;
                    quiz.ClassID = item.ClassID;
                    quiz.ClassSubjectID = item.ClassSubjectID;
                    quiz.LessonID = item.ParentID;
                    quiz.CourseID = item.CourseID;

                    _cloneQuestionService.Save(quiz);

                    questionVM.ID = quiz.ID;

                    if (questionVM.Answers != null && questionVM.Answers.Count > 0)
                    {
                        foreach (var answer in questionVM.Answers)
                        {
                            answer.CreateUser = _userCreate;
                            answer.ParentID = questionVM.ID;
                            if (answer.Media != null && answer.Media.Name == null) answer.Media = null;

                            if (answer.ID == "0" || answer.ID == null || _cloneAnswerService.GetItemByID(answer.ID) == null)
                            {
                                var maxItem = _cloneAnswerService.CreateQuery().Find(o => o.ParentID == quiz.ID).SortByDescending(o => o.Order).FirstOrDefault();
                                answer.Order = maxItem != null ? maxItem.Order + 1 : 0;
                                answer.Created = DateTime.Now;

                                if (answer.Media == null || string.IsNullOrEmpty(answer.Media.Name) || !files.Any(f => f.Name == answer.Media.Name))
                                    answer.Media = null;
                                else
                                {
                                    var file = files.Where(f => f.Name == answer.Media.Name).SingleOrDefault();
                                    if (file != null)
                                    {
                                        answer.Media.Created = DateTime.Now;
                                        answer.Media.Size = file.Length;
                                        answer.Media.Path = await _fileProcess.SaveMediaAsync(file, answer.Media.OriginalName);
                                    }
                                }
                            }
                            else
                            {
                                var oldanswer = _cloneAnswerService.GetItemByID(answer.ID);
                                if ((oldanswer.Media != null && answer.Media != null && oldanswer.Media.Path != answer.Media.Path)
                            || (oldanswer.Media == null && answer.Media != null))//Media change
                                {
                                    if (oldanswer.Media != null && !string.IsNullOrEmpty(oldanswer.Media.Path))//Delete old file
                                        _fileProcess.DeleteFile(oldanswer.Media.Path);

                                    if (files == null || !files.Any(f => f.Name == answer.Media.Name))
                                        answer.Media = null;
                                    else
                                    {

                                        var file = files.Where(f => f.Name == answer.Media.Name).SingleOrDefault();//update media
                                        answer.Media.Created = DateTime.Now;
                                        answer.Media.Size = file.Length;
                                        answer.Media.Path = await _fileProcess.SaveMediaAsync(file, answer.Media.OriginalName);
                                    }
                                }
                                //else // No Media
                                //{
                                //    quiz.Media = null;
                                //}
                                answer.Order = oldanswer.Order;
                                answer.Created = oldanswer.Created;
                                answer.OriginID = oldanswer.OriginID;
                            }

                            answer.Updated = DateTime.Now;
                            answer.ClassID = item.ClassID;
                            answer.ClassSubjectID = item.ClassSubjectID;
                            answer.CourseID = item.CourseID;

                            _cloneAnswerService.Save(answer);
                        }
                    }
                }
            }

            IDictionary<string, object> valuePairs = new Dictionary<string, object>
                        {
                            { "Data", item },
                            //{ "LessonPartExtends", files }
                        };

            return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", item },
                                {"Error", null }
                            });
        }

        [HttpPost]
        public async Task<JsonResult> Remove(string ID)
        {
            try
            {
                //var parentLesson = _lessonService.CreateQuery().Find(o => o.ID == item.ParentID
                //&& o.CreateUser == UserID TODO:remove later
                //).FirstOrDefault(); //Chỉ remove được các part thuộc lesson do mình up
                //TODO: Check for safety later

                await RemoveLessonPart(ID);

                return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", "Remove OK" },
                                {"Error", null }
                            });

            }
            catch (Exception ex)
            {
                return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", null },
                                {"Error", ex.Message}
                            });
            }
        }

        private async Task RemoveLessonPart(string ID)
        {
            try
            {
                var item = _service.CreateQuery().Find(o => o.ID == ID).SingleOrDefault();
                if (item == null) return;

                var questionIds = _cloneQuestionService.CreateQuery().Find(o => o.ParentID == ID).Project(t => t.ID).ToList();
                for (int i = 0; questionIds != null && i < questionIds.Count; i++)
                    await RemoveQuestion(questionIds[i]);
                await _service.RemoveAsync(ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task RemoveQuestion(string ID)
        {
            try
            {
                var item = _cloneQuestionService.CreateQuery().Find(o => o.ID == ID).Project(t => t.ID).FirstOrDefault();
                if (item == null) return;
                await _cloneAnswerService.CreateQuery().DeleteManyAsync(o => o.ParentID == ID);
                await _cloneQuestionService.RemoveAsync(ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult ChangePosition(string ID, int pos, string ClassID)
        {
            var item = _service.CreateQuery().Find(o => o.ID == ID).SingleOrDefault();
            if (item == null) return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", null },
                        {"Error", "Data not found" }
                    });

            var parentLesson = _lessonService.CreateQuery().Find(o => o.ID == item.ParentID
            ).SingleOrDefault();

            if (parentLesson != null)
            {
                var parts = _service.CreateQuery().Find(o => o.ParentID == parentLesson.ID && o.ClassID == ClassID).SortBy(o => o.Order).ThenBy(o => o.ID).ToList();
                var ids = parts.Select(o => o.ID).ToList();

                var oldPos = ids.IndexOf(ID);
                if (oldPos == pos)
                    return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", null },
                        {"Error", "Nothing change" }
                    });

                item.Order = pos;
                var filter = Builders<CloneLessonPartEntity>.Filter.Where(o => o.ID == item.ID);
                var update = Builders<CloneLessonPartEntity>.Update.Set("Order", pos);
                var publish = _service.Collection.UpdateMany(filter, update);
                int entry = -1;
                foreach (var part in parts)
                {
                    if (part.ID == item.ID) continue;
                    if (entry == pos - 1)
                        entry++;
                    entry++;
                    part.Order = entry;
                    var filterX = Builders<CloneLessonPartEntity>.Filter.Where(o => o.ID == part.ID);
                    var updateX = Builders<CloneLessonPartEntity>.Update.Set("Order", part.Order);
                    var publishX = _service.Collection.UpdateMany(filterX, updateX);
                }
                return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", publish },
                        {"Error", null }
                    });
            }
            else
            {
                return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", null },
                        {"Error", "Not found" }
                    });
            }
        }

        private void CloneLessonPart(CloneLessonPartEntity item)
        {
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
                    cloneQuestion.ClassID = item.ClassID;
                    cloneQuestion.LessonID = item.ParentID;
                    CloneLessonQuestion(cloneQuestion);
                }
            }
        }

        private void CloneLessonQuestion(CloneLessonPartQuestionEntity item)
        {
            var _userCreate = User.Claims.GetClaimByType("UserID").Value;
            _cloneQuestionService.Collection.InsertOne(item);
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
            _cloneAnswerService.Collection.InsertOne(item);
        }

        public JsonResult ConvertFillQuestion()
        {
            var fillparts = _lessonPartService.CreateQuery().Find(p => p.Type == "QUIZ2").ToList();
            if (fillparts != null && fillparts.Count > 0)
            {
                foreach (var part in fillparts)
                {
                    if (part.ParentID == "5db92529ab30e73154d7ed56")
                    {
                        var a = 1;
                    }
                    //normalize Description
                    part.Description = (part.Description ?? "").Trim();
                    if (part.Description.IndexOf("fillquiz") >= 0)
                        continue;
                    var desc = part.Description;
                    if (part.Description.Length > 0)
                    {
                        desc = NormalizeDescription(desc);
                        desc = desc.Replace("#", "[FILLREPLACMENT]");
                        desc = desc.Replace("----", "#");
                    }
                    var descArr = desc.Split('#');
                    var descLen = descArr.Length;

                    var questions = _lessonPartQuestionService.CreateQuery().Find(q => q.ParentID == part.ID).ToList();
                    if (questions != null && questions.Count > 0)
                    {
                        if (descLen > 1 && descLen >= (questions.Count + 1)) // has place holder
                        {
                            var pos = descLen - 1;
                            desc = descArr[pos];

                            for (int j = 0; j < questions.Count; j++)
                            {
                                var quiz = questions[j];
                                desc = descArr[--pos] + "<fillquiz><input class='fillquiz'></input></fillquiz>" + desc;
                                quiz.Content = "";
                                _lessonPartQuestionService.Save(quiz);
                            }
                            for (int k = pos; k > 0; k--)
                                desc = descArr[k] + "-----" + desc;
                            part.Description = desc.Replace("[FILLREPLACMENT]", "#");
                        }
                        else
                            for (int i = 0; i < questions.Count; i++)
                            {
                                var quiz = questions[i];
                                var qdesc = (quiz.Content ?? "").Trim();
                                if (qdesc.Length > 0)
                                {
                                    qdesc = NormalizeDescription(qdesc);
                                    qdesc = qdesc.Replace("#", "[FILLREPLACMENT]");
                                    qdesc = qdesc.Replace("----", "#");
                                }
                                var qdescArr = qdesc.Split('#');
                                var qdescLen = qdescArr.Length;
                                if (qdescLen == 2)
                                {
                                    part.Description += ("<p>" + qdescArr[0] + " <fillquiz><input class='fillquiz'></input></fillquiz>" + qdescArr[1] + "</p>").Replace("[FILLREPLACMENT]", "#");
                                }
                                else
                                    part.Description += "<p>" + (quiz.Content ?? "").Trim() + " <fillquiz><input class='fillquiz'></input></fillquiz></p>";
                                quiz.Content = "";
                                _lessonPartQuestionService.Save(quiz);
                            }
                        _lessonPartService.Save(part);
                    }


                }
            }
            var clonefillparts = _service.CreateQuery().Find(p => p.Type == "QUIZ2").ToList();
            if (clonefillparts != null && clonefillparts.Count > 0)
            {
                foreach (var part in clonefillparts)
                {
                    part.Description = (part.Description ?? "").Trim();
                    if (part.Description.IndexOf("fillquiz") >= 0)
                        continue;
                    var desc = part.Description;
                    if (part.Description.Length > 0)
                    {
                        desc = NormalizeDescription(desc);
                        desc = desc.Replace("#", "[FILLREPLACMENT]");
                        desc = desc.Replace("----", "#");
                    }
                    var descArr = desc.Split('#');
                    var descLen = descArr.Length;

                    var questions = _cloneQuestionService.CreateQuery().Find(q => q.ParentID == part.ID).ToList();
                    if (questions != null && questions.Count > 0)
                        if (descLen > 1 && descLen >= questions.Count) // has place holder
                        {
                            var pos = descLen - 1;
                            desc = descArr[pos];

                            for (int j = 0; j < questions.Count; j++)
                            {
                                var quiz = questions[j];
                                desc = descArr[pos--] + "<fillquiz><input class='fillquiz'></input></fillquiz>" + desc;
                                quiz.Content = "";
                                _cloneQuestionService.Save(quiz);
                            }
                            for (int k = pos; k >= 0; k--)
                                desc = descArr[k] + "-----" + desc;
                            part.Description = desc.Replace("[FILLREPLACMENT]", "#");
                        }
                        else
                            for (int i = 0; i < questions.Count; i++)
                            {
                                var quiz = questions[i];
                                var qdesc = (quiz.Content ?? "").Trim();
                                if (qdesc.Length > 0)
                                {
                                    qdesc = NormalizeDescription(qdesc);
                                    qdesc = qdesc.Replace("#", "[FILLREPLACMENT]");
                                    qdesc = qdesc.Replace("----", "#");
                                }
                                var qdescArr = qdesc.Split('#');
                                var qdescLen = qdescArr.Length;
                                if (qdescLen == 2)
                                {
                                    part.Description += ("<p>" + qdescArr[0] + " <fillquiz><input class='fillquiz'></input></fillquiz>" + qdescArr[1] + "</p>").Replace("[FILLREPLACMENT]", "#");
                                }
                                else
                                    part.Description += "<p>" + (quiz.Content ?? "").Trim() + " <fillquiz><input class='fillquiz'></input></fillquiz></p>";
                                quiz.Content = "";
                                _cloneQuestionService.Save(quiz);
                            }

                    _service.Save(part);
                }
            }
            return new JsonResult("Convert Done");
        }

        private string NormalizeDescription(string desc)
        {
            var _result = desc ?? "";
            //var listRegex = ["\.\.\.\."]

            var repArr = new List<string> { ".....", "_____", "-----" };

            _result = _result.Replace("&hellip;", ".").Replace("…","...");

            while (_result.IndexOf("__ ") >= 0)
                _result = _result.Replace("__ ", "__");


            foreach (var repStr in repArr)
            {
                while (_result.IndexOf(repStr) >= 0)
                    _result = _result.Replace(repStr, repStr.Substring(0, 4));
                _result = _result.Replace(repStr.Substring(0, 4), "----");
            }

            _result = Regex.Replace(_result, "([\\.-]){2,4}\\(.*\\)([\\.-]){2,4}", "----");
            return _result;
        }
    }
}
