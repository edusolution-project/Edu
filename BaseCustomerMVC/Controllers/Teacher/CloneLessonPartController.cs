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
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using MongoDB.Bson.Serialization.Serializers;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml.ConditionalFormatting;
using System.Net;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class CloneLessonPartController : TeacherController
    {
        //private readonly GradeService _gradeService;
        //private readonly SubjectService _subjectService;
        //private readonly TeacherService _teacherService;
        private readonly ClassService _classService;
        private readonly ClassHelper _classHelper;
        private readonly ClassSubjectService _classSubjectService;
        private readonly LessonService _lessonService;
        private readonly LessonHelper _lessonHelper;


        private readonly CloneLessonPartService _cloneLessonPartService;
        private readonly CloneLessonPartAnswerService _cloneAnswerService;
        private readonly CloneLessonPartQuestionService _cloneQuestionService;

        private readonly MappingEntity<LessonPartEntity, CloneLessonPartEntity> _lessonpartMapping;
        private readonly MappingEntity<LessonPartQuestionEntity, CloneLessonPartQuestionEntity> _lessonpartQuestionMapping;
        private readonly MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity> _lessonpartAnswerMapping;
        private readonly MappingEntity<CloneLessonPartEntity, CloneLessonPartViewModel> _cloneLessonPartViewModelMapping;
        private readonly MappingEntity<LessonPartQuestionEntity, CloneQuestionViewModel> _cloneQuestionViewModelMapping;
        //private readonly MappingEntity<LessonPartAnswerEntity> _cloneQuestionViewModelMapping;

        private readonly FileProcess _fileProcess;
        private readonly VocabularyService _vocabularyService;

        private readonly List<string> quizType = new List<string> { "QUIZ1", "QUIZ2", "QUIZ3", "QUIZ4", "ESSAY" };

        public CloneLessonPartController(
            //GradeService gradeservice,
            //SubjectService subjectService,
            //TeacherService teacherService,
            ClassService classService,
            ClassHelper classHelper,
            ClassSubjectService classSubjectService,
            //CourseService courseService,
            //ChapterService chapterService,
            LessonService lessonService,
            LessonHelper lessonHelper,
            //LessonScheduleService lessonScheduleService,
            //LessonPartService lessonPartService,
            //LessonPartQuestionService lessonPartQuestionService,
            //LessonPartAnswerService lessonPartAnswerService,
            CloneLessonPartService service,
            CloneLessonPartAnswerService cloneLessonPartAnswerService,
            CloneLessonPartQuestionService cloneLessonPartQuestionService,
            FileProcess fileProcess,
            VocabularyService vocabularyService
            )
        {
            //_gradeService = gradeservice;
            //_subjectService = subjectService;
            //_teacherService = teacherService;
            //_courseService = courseService;
            _classSubjectService = classSubjectService;
            _classService = classService;
            _classHelper = classHelper;
            //_chapterService = chapterService;
            _lessonService = lessonService;
            //_lessonPartService = lessonPartService;
            //_lessonPartQuestionService = lessonPartQuestionService;
            //_lessonPartAnswerService = lessonPartAnswerService;
            _lessonHelper = lessonHelper;

            _cloneLessonPartService = service;
            _cloneAnswerService = cloneLessonPartAnswerService;
            _cloneQuestionService = cloneLessonPartQuestionService;

            _lessonpartMapping = new MappingEntity<LessonPartEntity, CloneLessonPartEntity>();
            _lessonpartQuestionMapping = new MappingEntity<LessonPartQuestionEntity, CloneLessonPartQuestionEntity>();
            _lessonpartAnswerMapping = new MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity>();
            _cloneLessonPartViewModelMapping = new MappingEntity<CloneLessonPartEntity, CloneLessonPartViewModel>();
            _cloneQuestionViewModelMapping = new MappingEntity<LessonPartQuestionEntity, CloneQuestionViewModel>();
            _fileProcess = fileProcess;
            _vocabularyService = vocabularyService;
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetList(string LessonID, string ClassID, string ClassSubjectID)
        {
            var root = _lessonService.CreateQuery().Find(o => o.ID == LessonID).SingleOrDefault();

            var currentCs = _classSubjectService.GetItemByID(ClassSubjectID);
            if (currentCs == null)
                return new JsonResult(
                new Dictionary<string, object> { { "Error", "Subject not found" } });

            var currentClass = _classService.GetItemByID(ClassID);
            if (currentClass == null)
                return new JsonResult(
                new Dictionary<string, object> { { "Error", "Class not found" } });

            var data = new Dictionary<string, object> { };

            if (root != null && currentClass != null)
            {
                var listCloneLessonPart = _cloneLessonPartService.CreateQuery().Find(o => o.ParentID == LessonID && o.ClassSubjectID == currentCs.ID).SortBy(q => q.Order).ThenBy(q => q.ID).ToList();
                if (listCloneLessonPart != null && listCloneLessonPart.Count > 0)
                {
                    var result = new List<CloneLessonPartViewModel>();
                    foreach (var part in listCloneLessonPart)
                    {
                        switch (part.Type)
                        {
                            case "QUIZ1":
                            case "QUIZ2":
                            case "QUIZ3":
                            case "QUIZ4":
                            case "ESSAY":
                                result.Add(new CloneLessonPartViewModel(part)
                                {
                                    Questions = _cloneQuestionService.CreateQuery().Find(q => q.ParentID == part.ID).SortBy(q => q.Order).ThenBy(q => q.ID).ToList().Select(q => new CloneQuestionViewModel(q)
                                    {
                                        Answers = _cloneAnswerService.CreateQuery().Find(a => a.ParentID == q.ID).SortBy(a => a.Order).ThenBy(a => a.ID).ToList()
                                    }).ToList()
                                });
                                break;
                            case "VOCAB":
                                result.Add(new CloneLessonPartViewModel(part)
                                {
                                    Description = RenderVocab(part.Description)
                                });
                                break;
                            default:
                                result.Add(new CloneLessonPartViewModel(part));
                                break;
                        }
                    }
                    data = new Dictionary<string, object>
                    {
                        { "Data", result }
                    };
                };
            }


            return new JsonResult(data);
        }

        [HttpPost]
        public JsonResult GetDetail(string ID)
        {
            var part = _cloneLessonPartService.CreateQuery().Find(o => o.ID == ID).SingleOrDefault();
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
        public async Task<JsonResult> CreateOrUpdate(string basis, CloneLessonPartViewModel item, List<string> RemovedQuestions = null, List<string> RemovedAnswers = null)
        {
            var createduser = User.Claims.GetClaimByType("UserID").Value;
            var parentLesson = _lessonService.GetItemByID(item.ParentID);
            var currentCs = _classSubjectService.GetItemByID(parentLesson.ClassSubjectID);

            var isPractice = parentLesson.IsPractice;
            if (parentLesson == null || currentCs == null)
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
                item.Created = DateTime.UtcNow;
                item.TeacherID = currentCs.TeacherID;
                var maxItem = _cloneLessonPartService.CreateQuery()
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
                        item.Media.Created = DateTime.UtcNow;
                        item.Media.Size = file.Length;
                        item.Media.Path = await _fileProcess.SaveMediaAsync(file, item.Media.OriginalName, "", basis);
                    }
                }

            }
            else // Update
            {
                var olditem = _cloneLessonPartService.GetItemByID(item.ID);
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
                    item.Media.Created = DateTime.UtcNow;
                    item.Media.Size = file.Length;
                    item.Media.Path = await _fileProcess.SaveMediaAsync(file, item.Media.OriginalName, "", basis);
                }
                item.OriginID = olditem.OriginID;
                item.Created = olditem.Created;
                item.Order = olditem.Order;
            }

            item.Updated = DateTime.UtcNow;
            item.ClassID = currentCs.ClassID;
            item.ClassSubjectID = currentCs.ID;
            item.CourseID = currentCs.CourseID;

            var lessonpart = item.ToEntity();
            _cloneLessonPartService.Save(lessonpart);

            item.ID = lessonpart.ID;

            switch (lessonpart.Type)
            {
                case "ESSAY":
                    _cloneQuestionService.CreateQuery().DeleteMany(t => t.ParentID == lessonpart.ID);
                    var question = new CloneLessonPartQuestionEntity
                    {
                        CourseID = lessonpart.CourseID,
                        Content = "",
                        Description = item.Questions == null ? "" : item.Questions[0].Description,
                        ParentID = lessonpart.ID,
                        CreateUser = createduser,
                        Point = lessonpart.Point,
                        Created = lessonpart.Created,
                    };
                    _cloneQuestionService.Save(question);
                    isPractice = true;
                    break;
                case "VOCAB":
                    if (lessonpart.Description != null && lessonpart.Description.Length > 0)
                    {
                        var vocabArr = lessonpart.Description.Split('|');
                        if (vocabArr != null && vocabArr.Length > 0)
                        {
                            foreach (var vocab in vocabArr)
                            {
                                var vocabulary = vocab.Trim();
                                _ = GetVocab(vocabulary);
                            }
                        }
                    }
                    break;
                case "QUIZ2": //remove all previous question
                    var oldQuizIds = _cloneQuestionService.CreateQuery().Find(q => q.ParentID == lessonpart.ID).Project(i => i.ID).ToEnumerable();
                    foreach (var quizid in oldQuizIds)
                        _cloneAnswerService.CreateQuery().DeleteMany(a => a.ParentID == quizid);
                    _cloneQuestionService.CreateQuery().DeleteMany(q => q.ParentID == lessonpart.ID);

                    if (!String.IsNullOrEmpty(item.Description) && item.Description.ToLower().IndexOf("<fillquiz ") >= 0)
                    {
                        var newdescription = "";
                        if (item.Questions == null || item.Questions.Count == 0)
                            item.Questions = ExtractFillQuestionList(item, createduser, out newdescription);
                        lessonpart.Description = newdescription;
                        _cloneLessonPartService.CreateQuery().ReplaceOne(t => t.ID == lessonpart.ID, lessonpart);
                    }
                    else
                    {
                        //No Question
                    }

                    item.CourseID = parentLesson.CourseID;

                    if (item.Questions != null && item.Questions.Count > 0)
                    {
                        await SaveQuestionFromView(item, createduser, files, basis);
                    }
                    isPractice = true;
                    break;
                case "QUIZ1":
                case "QUIZ3":
                case "QUIZ4":
                    if (RemovedQuestions != null && RemovedQuestions.Count > 0)
                    {
                        _cloneQuestionService.CreateQuery().DeleteMany(o => RemovedQuestions.Contains(o.ID));

                        foreach (var quizID in RemovedQuestions)
                        {
                            _cloneAnswerService.CreateQuery().DeleteMany(o => o.ParentID == quizID);
                        }
                    }

                    if (RemovedAnswers != null && RemovedAnswers.Count > 0)
                        _cloneAnswerService.CreateQuery().DeleteMany(o => RemovedAnswers.Contains(o.ID));
                    item.CourseID = parentLesson.CourseID;

                    if (item.Questions != null && item.Questions.Count > 0)
                    {
                        await SaveQuestionFromView(item, createduser, files);
                    }
                    isPractice = true;
                    break;
                default:
                    return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", item },
                                {"Error", null }
                            });
                    break;
            }

            if (parentLesson.TemplateType == LESSON_TEMPLATE.LECTURE && parentLesson.IsPractice != isPractice)
            {
                parentLesson.IsPractice = isPractice;
                _lessonService.Save(parentLesson);

                //updateLessonPractice 
                _ = _classHelper.ChangeLessonPracticeState(parentLesson);
            }


            _lessonHelper.calculateCloneLessonPoint(item.ParentID);

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
                var item = _cloneLessonPartService.CreateQuery().Find(o => o.ID == ID).SingleOrDefault();
                if (item == null) return;

                var questionIds = _cloneQuestionService.CreateQuery().Find(o => o.ParentID == ID).Project(t => t.ID).ToList();
                for (int i = 0; questionIds != null && i < questionIds.Count; i++)
                    await RemoveQuestion(questionIds[i]);
                await _cloneLessonPartService.RemoveAsync(ID);

                var parentLesson = _lessonService.GetItemByID(item.ParentID);
                if (parentLesson != null)
                {
                    var isQuiz = quizType.Contains(item.Type);
                    if (isQuiz)
                    {
                        _lessonHelper.calculateCloneLessonPoint(item.ParentID);
                        if (parentLesson.TemplateType == LESSON_TEMPLATE.LECTURE)
                        {
                            var quizPartCount = _cloneLessonPartService.GetByLessonID(item.ParentID).Count(t => quizType.Contains(t.Type));
                            if (quizPartCount == 0)//no quiz part
                            {
                                parentLesson.IsPractice = false;
                                _lessonService.Save(parentLesson);
                                //decrease Practice
                                _ = _classHelper.ChangeLessonPracticeState(parentLesson);
                            }
                        }
                    }
                }
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
            var item = _cloneLessonPartService.CreateQuery().Find(o => o.ID == ID).SingleOrDefault();
            if (item == null) return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", null },
                        {"Error", "Data not found" }
                    });

            var parentLesson = _lessonService.CreateQuery().Find(o => o.ID == item.ParentID
            ).SingleOrDefault();

            if (parentLesson != null)
            {
                var parts = _cloneLessonPartService.CreateQuery().Find(o => o.ParentID == parentLesson.ID && o.ClassID == ClassID).SortBy(o => o.Order).ThenBy(o => o.ID).ToList();
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
                var publish = _cloneLessonPartService.Collection.UpdateMany(filter, update);
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
                    var publishX = _cloneLessonPartService.Collection.UpdateMany(filterX, updateX);
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

        //private void CloneLessonPart(CloneLessonPartEntity item)
        //{
        //    _cloneLessonPartService.Collection.InsertOne(item);
        //    var list = _lessonPartQuestionService.CreateQuery().Find(o => o.ParentID == item.OriginID).ToList();
        //    if (list != null)
        //    {
        //        foreach (var question in list)
        //        {
        //            var cloneQuestion = _lessonpartQuestionMapping.AutoOrtherType(question, new CloneLessonPartQuestionEntity());
        //            cloneQuestion.OriginID = question.ID;
        //            cloneQuestion.ParentID = item.ID;
        //            cloneQuestion.ID = null;
        //            cloneQuestion.ClassID = item.ClassID;
        //            cloneQuestion.LessonID = item.ParentID;
        //            CloneLessonQuestion(cloneQuestion);
        //        }
        //    }
        //}

        //private void CloneLessonQuestion(CloneLessonPartQuestionEntity item)
        //{
        //    var _userCreate = User.Claims.GetClaimByType("UserID").Value;
        //    _cloneQuestionService.Collection.InsertOne(item);
        //    var list = _lessonPartAnswerService.CreateQuery().Find(o => o.ParentID == item.OriginID).ToList();
        //    if (list != null)
        //    {
        //        foreach (var answer in list)
        //        {
        //            var cloneAnswer = _lessonpartAnswerMapping.AutoOrtherType(answer, new CloneLessonPartAnswerEntity());
        //            cloneAnswer.OriginID = answer.ID;
        //            cloneAnswer.ParentID = item.ID;
        //            cloneAnswer.ID = null;
        //            CloneLessonAnswer(cloneAnswer);
        //        }
        //    }
        //}

        //private void CloneLessonAnswer(CloneLessonPartAnswerEntity item)
        //{
        //    _cloneAnswerService.Collection.InsertOne(item);
        //}

        private string RenderVocab(string description)
        {
            string result = "";
            var vocabs = description.Split('|');
            if (vocabs == null || vocabs.Count() == 0)
                return description;
            foreach (var vocab in vocabs)
            {
                var vocabularies = _vocabularyService.GetItemByCode(vocab.Trim().Replace("-", ""));
                if (vocabularies != null && vocabularies.Count > 0)
                {
                    result +=
                        $"<div class='vocab-box'>" +
                            $"<b class='word-title'>{vocab.Trim()}</b><span class='word-pron'>{vocabularies[0].Pronunciation}</span>" +
                            $"<div class='vocab-audio'>" +
                                $"<button onclick='PlayPronun(this)'><i class='ic fas fa-volume-up'></i></button>" +
                                $"<audio class='d-none' id='audio' controls><source src='{vocabularies[0].PronunAudioPath}' type='audio/mpeg' />Your browser does not support the audio tag</audio>" +
                            $"</div>" +
                            //$"<div class='vocab-type'>{string.Join(",", vocabularies.Select(t => t.WordType).ToList())}<div/>" +
                            $"<div class='vocab-meaning'>{string.Join("<br/>", vocabularies.Where(t => !string.IsNullOrEmpty(t.Description)).Select(t => "<b>" + WordType.GetShort(t.WordType) + "</b>: " + t.Description).ToList())}</div>" +
                        $"</div>";
                }
            }
            return result;
        }

        private async Task GetVocab(string vocab)
        {
            //check if vocab is exist
            var code = vocab.ToLower().Replace(" ", "-");
            var olditems = _vocabularyService.GetItemByCode(code);
            if (olditems != null && olditems.Count > 0)
                return;

            var listVocab = new List<VocabularyEntity>();
            var pronUrl = "https://dictionary.cambridge.org/vi/dictionary/english/" + code;
            var dictUrl = "https://dictionary.cambridge.org/vi/dictionary/english-vietnamese/" + code;
            var listExp = new List<PronunExplain>();

            using (var expclient = new WebClient())
            {
                var expDoc = new HtmlDocument();
                string expHtml = expclient.DownloadString(dictUrl);
                expDoc.LoadHtml(expHtml);
                var expContents = expDoc.DocumentNode.SelectNodes("//div[contains(@class,\"english-vietnamese kdic\")]");
                if (expContents != null && expContents.Count() > 0)
                {
                    foreach (var expContent in expContents)
                    {
                        var typeNode = expContent.SelectSingleNode(".//span[contains(@class,\"pos dpos\")]");
                        if (typeNode == null) continue;
                        var type = typeNode.InnerText;
                        if (listExp.Any(t => t.WordType == type))
                            continue;
                        var expNodes = expContent.SelectNodes(".//span[contains(@class,\"trans dtrans\")]");
                        if (expNodes != null && expNodes.Count() > 0)
                        {
                            foreach (var node in expNodes)
                            {
                                listExp.Add(new PronunExplain
                                {
                                    WordType = type,
                                    Meaning = node.InnerText
                                });
                            }
                        }

                    }
                }
            }
            if (listExp == null || listExp.Count == 0)
                return;

            using (var client = new WebClient())
            {
                HtmlDocument doc = new HtmlDocument();
                string html = client.DownloadString(pronUrl);
                doc.LoadHtml(html);

                var contentHolder = doc.DocumentNode.SelectNodes("//div[contains(@class, \"pos-header\")]");
                if (contentHolder != null && contentHolder.Count() > 0)
                {
                    foreach (var content in contentHolder)
                    {
                        var type = content.SelectSingleNode(".//span[contains(@class,\"pos dpos\")]").InnerText;

                        if (listVocab.Any(t => t.WordType == type))
                            continue;
                        try
                        {

                            var pronun = content.SelectSingleNode(".//span[contains(@class,\"us dpron-i\")]");
                            var pronunText = pronun.SelectSingleNode(".//span[contains(@class,\"pron dpron\")]").InnerText;
                            var pronunPath = pronun.SelectSingleNode(".//source[1]").GetAttributeValue("src", null);
                            pronunPath = "https://dictionary.cambridge.org" + pronunPath;
                            var vocabulary = new VocabularyEntity
                            {
                                Name = vocab,
                                Code = code,
                                Language = "en-us",
                                WordType = type,
                                Pronunciation = pronunText,
                                PronunAudioPath = pronunPath,
                                Created = DateTime.UtcNow,
                                Description = string.Join(", ", listExp.Where(t => t.WordType == type).Select(t => t.Meaning))
                            };
                            listVocab.Add(vocabulary);

                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
            }

            if (listVocab.Count() == 0) return;
            foreach (var vocal in listVocab)
                _vocabularyService.Save(vocal);
            return;
        }

        private List<CloneQuestionViewModel> ExtractFillQuestionList(CloneLessonPartEntity item, string creator, out string Description)
        {
            Description = item.Description;
            var questionList = new List<CloneQuestionViewModel>();
            //extract Question from Description
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(item.Description);
            var fillquizs = doc.DocumentNode.SelectNodes(".//fillquiz[*[contains(@class,\"fillquiz\")]]");
            if (fillquizs == null || fillquizs.Count() == 0)
                return questionList;

            for (int i = 0; i < fillquizs.Count(); i++)
            {
                var quiz = fillquizs[i];

                var inputNode = quiz.SelectSingleNode(".//*[contains(@class,\"fillquiz\")]");
                if (inputNode == null)
                {
                    continue;
                }

                var ans = inputNode.GetAttributeValue("ans", null);
                if (ans == null)
                    ans = inputNode.GetAttributeValue("placeholder", null);
                if (string.IsNullOrEmpty(ans))
                {
                    inputNode.Remove();
                    continue;
                }
                var Question = new CloneQuestionViewModel
                {
                    ParentID = item.ID,
                    CourseID = item.CourseID,
                    CreateUser = creator,
                    Order = i,
                    Point = 1,
                    Content = inputNode.GetAttributeValue("dsp", null),//phần hiển thị cho học viên
                    Description = quiz.GetAttributeValue("title", null),//phần giải thích đáp án
                    Answers = new List<CloneLessonPartAnswerEntity>()
                };

                var ansArr = ans.Split('|');
                foreach (var answer in ansArr)
                {
                    var validAns = validateFill(answer);
                    if (!string.IsNullOrEmpty(validAns))
                    {
                        Question.Answers.Add(new CloneLessonPartAnswerEntity
                        {
                            CourseID = item.CourseID,
                            CreateUser = creator,
                            IsCorrect = true,
                            Content = validAns
                        });
                    }
                }

                questionList.Add(Question);

                inputNode.Attributes.Remove("contenteditable");
                inputNode.Attributes.Remove("readonly");
                inputNode.Attributes.Remove("title");
                inputNode.Attributes.Remove("value");
                inputNode.Attributes.Remove("dsp");
                inputNode.Attributes.Remove("ans");
                inputNode.Attributes.Remove("placeholder");
                inputNode.Attributes.Remove("size");

                quiz.Attributes.Remove("contenteditable");
                quiz.Attributes.Remove("readonly");
                quiz.Attributes.Remove("title");
                //quiz.ChildNodes.Add(clearnode);
            }

            var removeNodes = doc.DocumentNode.SelectNodes(".//fillquiz[not(input)]");
            if (removeNodes != null && removeNodes.Count() > 0)
                foreach (var node in removeNodes)
                    node.Remove();

            Description = doc.DocumentNode.OuterHtml.ToString();
            return questionList;
        }

        private async Task SaveQuestionFromView(CloneLessonPartViewModel item, string createuser = "auto", IFormFileCollection files = null, string centerCode = "")
        {
            foreach (var questionVM in item.Questions)
            {
                questionVM.ParentID = item.ID;
                questionVM.CreateUser = createuser;
                var quiz = questionVM.ToEntity();

                if (questionVM.ID == "0" || questionVM.ID == null || _cloneQuestionService.GetItemByID(quiz.ID) == null)
                {
                    var maxItem = _cloneQuestionService.CreateQuery()
                        .Find(o => o.ParentID == item.ID)
                        .SortByDescending(o => o.Order).FirstOrDefault();
                    quiz.Order = maxItem != null ? maxItem.Order + 1 : 0;
                    quiz.Created = DateTime.UtcNow;
                    quiz.Updated = DateTime.UtcNow;
                    quiz.CreateUser = createuser;

                    if (quiz.Media != null)
                    {
                        if (string.IsNullOrEmpty(quiz.Media.Name))
                        {
                            if (string.IsNullOrEmpty(quiz.Media.Path))
                            {
                                quiz.Media = null;
                            }
                        }
                        else
                        {
                            if (quiz.Media.Name.ToLower().StartsWith("http"))
                            {
                                quiz.Media.Created = DateTime.UtcNow;
                                quiz.Media.Size = 0;
                                quiz.Media.Path = quiz.Media.Name.Trim();
                            }
                            else
                            {
                                if (files == null || !files.Any(f => f.Name == quiz.Media.Name))
                                {
                                    if (string.IsNullOrEmpty(quiz.Media.Path))
                                        quiz.Media = null;
                                }
                                else
                                {
                                    var file = files.Where(f => f.Name == quiz.Media.Name).SingleOrDefault();
                                    if (file != null)
                                    {
                                        quiz.Media.Created = DateTime.UtcNow;
                                        quiz.Media.Size = file.Length;
                                        quiz.Media.Path = await _fileProcess.SaveMediaAsync(file, quiz.Media.OriginalName, "", centerCode);
                                    }
                                }
                            }
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
                        {
                            //_fileProcess.DeleteFile(oldquiz.Media.Path);
                        }


                        if (files == null || !files.Any(f => f.Name == quiz.Media.Name))
                            quiz.Media = null;
                        else
                        {
                            var file = files.Where(f => f.Name == quiz.Media.Name).SingleOrDefault();//update media
                            quiz.Media.Created = DateTime.UtcNow;
                            quiz.Media.Size = file.Length;
                            quiz.Media.Path = await _fileProcess.SaveMediaAsync(file, quiz.Media.OriginalName, "", centerCode);
                        }
                    }

                    quiz.Order = oldquiz.Order;
                    quiz.Created = oldquiz.Created;

                }

                quiz.Updated = DateTime.UtcNow;
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
                        answer.CreateUser = createuser;
                        answer.ParentID = questionVM.ID;
                        if (answer.Media != null && answer.Media.Name == null) answer.Media = null;

                        if (answer.ID == "0" || answer.ID == null || _cloneAnswerService.GetItemByID(answer.ID) == null)
                        {
                            //var maxItem = _cloneAnswerService.CreateQuery().Find(o => o.ParentID == quiz.ID).SortByDescending(o => o.Order).FirstOrDefault();
                            //answer.Order = maxItem != null ? maxItem.Order + 1 : 0;
                            //answer.Created = DateTime.UtcNow;

                            //if (answer.Media == null || string.IsNullOrEmpty(answer.Media.Name) || !files.Any(f => f.Name == answer.Media.Name))
                            //    answer.Media = null;
                            //else
                            //{
                            //    var file = files.Where(f => f.Name == answer.Media.Name).SingleOrDefault();
                            //    if (file != null)
                            //    {
                            //        answer.Media.Created = DateTime.UtcNow;
                            //        answer.Media.Size = file.Length;
                            //        answer.Media.Path = await _fileProcess.SaveMediaAsync(file, answer.Media.OriginalName, "", centerCode);
                            //    }
                            //}
                            var maxItem = _cloneAnswerService.CreateQuery().Find(o => o.ParentID == quiz.ID).SortByDescending(o => o.Order).FirstOrDefault();
                            answer.Order = maxItem != null ? maxItem.Order + 1 : 0;
                            answer.Created = DateTime.UtcNow;
                            answer.Updated = DateTime.UtcNow;
                            answer.CreateUser = quiz.CreateUser;
                            answer.CourseID = quiz.CourseID;

                            if (answer.Media != null)
                            {
                                if (string.IsNullOrEmpty(answer.Media.Name))
                                {
                                    if (string.IsNullOrEmpty(answer.Media.Path))
                                        answer.Media = null;
                                }
                                else
                                {
                                    if (answer.Media.Name.ToLower().StartsWith("http"))
                                    {
                                        answer.Media.Created = DateTime.UtcNow;
                                        answer.Media.Size = 0;
                                        answer.Media.Path = answer.Media.Name.Trim();
                                    }
                                    else
                                    {
                                        if (files == null || !files.Any(f => f.Name == answer.Media.Name))
                                        {
                                            if (string.IsNullOrEmpty(answer.Media.Path))
                                                answer.Media = null;
                                        }
                                        else
                                        {
                                            var file = files.Where(f => f.Name == answer.Media.Name).SingleOrDefault();
                                            if (file != null)
                                            {
                                                answer.Media.Created = DateTime.UtcNow;
                                                answer.Media.Size = file.Length;
                                                answer.Media.Path = await _fileProcess.SaveMediaAsync(file, answer.Media.OriginalName, "", centerCode);
                                            }
                                        }

                                    }
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
                                {
                                    //_fileProcess.DeleteFile(oldanswer.Media.Path);
                                }


                                if (files == null || !files.Any(f => f.Name == answer.Media.Name))
                                    answer.Media = null;
                                else
                                {

                                    var file = files.Where(f => f.Name == answer.Media.Name).SingleOrDefault();//update media
                                    answer.Media.Created = DateTime.UtcNow;
                                    answer.Media.Size = file.Length;
                                    answer.Media.Path = await _fileProcess.SaveMediaAsync(file, answer.Media.OriginalName, "", centerCode);
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

                        answer.Updated = DateTime.UtcNow;
                        answer.ClassID = item.ClassID;
                        answer.ClassSubjectID = item.ClassSubjectID;
                        answer.CourseID = item.CourseID;

                        _cloneAnswerService.Save(answer);
                    }
                }
            }
        }


        //TODO: Need update later
        private string validateFill(string org)
        {
            if (string.IsNullOrEmpty(org)) return org;
            org = org.Trim();
            while (org.IndexOf("  ") >= 0)
                org = org.Replace("  ", "");
            return StringHelper.ReplaceSpecialCharacters(org);
        }

        //private double calculateLessonPoint(string lessonId)
        //{
        //    var point = 0.0;
        //    var parts = _cloneLessonPartService.GetByLessonID(lessonId).Where(t => quizType.Contains(t.Type));
        //    foreach (var part in parts)
        //    {
        //        if (part.Type == "ESSAY")
        //        {
        //            point += part.Point;
        //            _cloneQuestionService.Collection.UpdateMany(t => t.ParentID == part.ID, Builders<CloneLessonPartQuestionEntity>.Update.Set(t => t.Point, part.Point));
        //        }
        //        else
        //        {
        //            point += _cloneQuestionService.GetByPartID(part.ID).Count();//trắc nghiệm => điểm = số câu hỏi (mỗi câu 1đ)
        //            _cloneQuestionService.Collection.UpdateMany(t => t.ParentID == part.ID, Builders<CloneLessonPartQuestionEntity>.Update.Set(t => t.Point, 1));
        //        }
        //    }
        //    _lessonService.UpdateLessonPoint(lessonId, point);
        //    return point;
        //}

        #region
        public JsonResult CreateExamPart(string basis, List<string> ListLessonPartID, string LessonID)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;

                var lesson = _lessonService.GetItemByID(LessonID);
                if (lesson == null)
                    return Json(new Dictionary<string, Object>
                    {
                        {"Data",null },
                        {"Stt",false },
                        {"Msg","Thông tin không đúng" }
                    });
                if (ListLessonPartID.Count == 0)
                {
                    return Json(new Dictionary<string, Object>
                    {
                        {"Data",null },
                        {"Stt",false },
                        {"Msg","Chưa chọn bài" }
                    });
                }

                var listLessonPart = _cloneLessonPartService.GetByIDs(ListLessonPartID);
                if (listLessonPart.Count() == 0)
                {
                    return Json(new Dictionary<String, Object>
                        {
                            {"Data",null },
                            {"Stt",false },
                            {"Msg","Chưa chọn bài" }
                        });
                }
                foreach (var lessonPart in listLessonPart)
                {
                    _ = CopyPartToLesson(basis, lessonPart, lesson, UserID);
                }

                _lessonHelper.calculateCloneLessonPoint(LessonID);

                return Json(new Dictionary<String, Object>
                            {
                                {"Data",null },
                                {"Stt",true },
                                {"Msg","Thành công" }
                            });
            }
            catch (Exception ex)
            {
                return Json(new Dictionary<String, Object>
                    {
                        {"Data",null },
                        {"Stt",false },
                        {"Msg",ex.Message }
                    });
            }
        }
        #endregion

        private async Task CopyPartToLesson(string basis, CloneLessonPartEntity originPart, LessonEntity targetLesson, string createdUser)
        {
            var newPart = new CloneLessonPartViewModel { };
            _cloneLessonPartViewModelMapping.AutoOrtherTypeWithoutID(originPart, newPart);
            newPart.Questions = _cloneQuestionService.GetByPartID(originPart.ID).Select(question => _cloneQuestionViewModelMapping.AutoOrtherTypeWithoutID(question, new CloneQuestionViewModel
            {
                Created = DateTime.Now,
                CreateUser = createdUser,
                Answers = _cloneAnswerService.GetByQuestionID(question.ID).Select(answer => _lessonpartAnswerMapping.AutoOrtherTypeWithoutID(answer, new CloneLessonPartAnswerEntity
                {
                    Created = DateTime.Now,
                    CreateUser = createdUser,
                    ClassSubjectID = targetLesson.ClassSubjectID,
                    IsCorrect = answer.IsCorrect, //BAD MAPPING
                    ParentID = null
                })).ToList(),
                ClassSubjectID = targetLesson.ClassSubjectID,
                LessonID = targetLesson.ID
            })).ToList();
            newPart.ClassSubjectID = targetLesson.ClassSubjectID;
            newPart.ParentID = targetLesson.ID;
            newPart.ClassID = targetLesson.ID;

            await ClonePart(basis, newPart);
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<JsonResult> ClonePart(string basis, CloneLessonPartViewModel item)
        {
            var createduser = User.Claims.GetClaimByType("UserID").Value;
            var parentLesson = _lessonService.GetItemByID(item.ParentID);
            var currentCs = _classSubjectService.GetItemByID(parentLesson.ClassSubjectID);

            var isPractice = parentLesson.IsPractice;
            if (parentLesson == null || currentCs == null)
            {
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", null },
                    {"Error", "Parent Item Not found" }
                });
            }

            item.Created = DateTime.UtcNow;
            item.TeacherID = currentCs.TeacherID;
            var maxItem = _cloneLessonPartService.CreateQuery()
                .Find(o => o.ParentID == item.ParentID)
                .SortByDescending(o => o.Order).FirstOrDefault();
            item.Order = maxItem != null ? maxItem.Order + 1 : 0;

            item.Updated = DateTime.UtcNow;
            item.ClassID = currentCs.ClassID;
            item.ClassSubjectID = currentCs.ID;
            item.CourseID = currentCs.CourseID;

            var lessonpart = item.ToEntity();
            _cloneLessonPartService.Save(lessonpart);

            item.ID = lessonpart.ID;

            switch (lessonpart.Type)
            {
                case "ESSAY":
                    _cloneQuestionService.CreateQuery().DeleteMany(t => t.ParentID == lessonpart.ID);
                    var question = new CloneLessonPartQuestionEntity
                    {
                        CourseID = lessonpart.CourseID,
                        Content = "",
                        Description = item.Questions == null ? "" : item.Questions[0].Description,
                        ParentID = lessonpart.ID,
                        CreateUser = createduser,
                        Point = lessonpart.Point,
                        Created = lessonpart.Created,
                    };
                    _cloneQuestionService.Save(question);
                    isPractice = true;
                    break;
                //case "VOCAB":
                //    if (lessonpart.Description != null && lessonpart.Description.Length > 0)
                //    {
                //        var vocabArr = lessonpart.Description.Split('|');
                //        if (vocabArr != null && vocabArr.Length > 0)
                //        {
                //            foreach (var vocab in vocabArr)
                //            {
                //                var vocabulary = vocab.Trim();
                //                _ = GetVocab(vocabulary);
                //            }
                //        }
                //    }
                //    break;
                case "QUIZ2": //remove all previous question
                    var oldQuizIds = _cloneQuestionService.CreateQuery().Find(q => q.ParentID == lessonpart.ID).Project(i => i.ID).ToEnumerable();
                    foreach (var quizid in oldQuizIds)
                        _cloneAnswerService.CreateQuery().DeleteMany(a => a.ParentID == quizid);
                    _cloneQuestionService.CreateQuery().DeleteMany(q => q.ParentID == lessonpart.ID);

                    if (!String.IsNullOrEmpty(item.Description) && item.Description.ToLower().IndexOf("<fillquiz ") >= 0)
                    {
                        var newdescription = "";
                        if (item.Questions == null || item.Questions.Count == 0)
                            item.Questions = ExtractFillQuestionList(item, createduser, out newdescription);
                        lessonpart.Description = newdescription;
                        _cloneLessonPartService.CreateQuery().ReplaceOne(t => t.ID == lessonpart.ID, lessonpart);
                    }
                    else
                    {
                        //No Question
                    }

                    item.CourseID = parentLesson.CourseID;

                    if (item.Questions != null && item.Questions.Count > 0)
                    {
                        await SaveQuestionFromView(item, createduser, null, basis);
                    }
                    isPractice = true;
                    break;
                case "QUIZ1":
                case "QUIZ3":
                case "QUIZ4":
                    item.CourseID = parentLesson.CourseID;

                    if (item.Questions != null && item.Questions.Count > 0)
                    {
                        await SaveQuestionFromView(item, createduser, null);
                    }
                    isPractice = true;
                    break;
                default:
                    return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", item },
                                {"Error", null }
                            });
            }

            _lessonHelper.calculateCloneLessonPoint(item.ParentID);

            return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", item },
                                {"Error", null }
                            });
        }


    }
}
