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
using Spire.Doc;
using Spire.Doc.Fields;
using Spire.Doc.Documents;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using FileManagerCore.Interfaces;
using System.Drawing;

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
        private readonly LessonScheduleService _lessonScheduleService;

        private readonly MappingEntity<LessonPartEntity, CloneLessonPartEntity> _lessonpartMapping;
        private readonly MappingEntity<LessonPartQuestionEntity, CloneLessonPartQuestionEntity> _lessonpartQuestionMapping;
        private readonly MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity> _lessonpartAnswerMapping;
        private readonly MappingEntity<CloneLessonPartEntity, CloneLessonPartViewModel> _cloneLessonPartViewModelMapping;
        private readonly MappingEntity<LessonPartQuestionEntity, CloneQuestionViewModel> _cloneQuestionViewModelMapping;
        //private readonly MappingEntity<LessonPartAnswerEntity> _cloneQuestionViewModelMapping;

        private readonly FileProcess _fileProcess;
        private readonly VocabularyService _vocabularyService;

        private readonly string[] typeVideo = { ".ogm", ".wmv", ".mpg", ".webm", ".ogv", ".mov", ".asx", ".mpge", ".mp4", ".m4v", ".avi" };
        private readonly string[] typeAudio = { ".opus", ".flac", ".weba", ".webm", ".wav", ".ogg", ".m4a", ".oga", ".mid", ".mp3", ".aiff", ".wma", ".au" };
        private readonly string[] typeImage = { ".jfif", ".pjpeg", ".jpeg", ".pjp", ".jpg", ".png", ".gif", ".bmp", ".dip" };


        private readonly List<string> quizType = new List<string> { "QUIZ1", "QUIZ2", "QUIZ3", "QUIZ4", "ESSAY" };
        private readonly List<string> partTypes = new List<string> { "TEXT", "DOC", "AUDIO", "VIDEO", "IMG", "VOCAB", "QUIZ1", "QUIZ2", "QUIZ3", "QUIZ4", "ESSAY" };
        private readonly List<string> partDSP = new List<string> { "Văn bản", "File văn bản", "Audio", "Video", "Ảnh", "Từ vựng tiếng anh", "Chọn 1 đáp án đúng", "Điền từ", "Nối đáp án", "Chọn 1/nhiều đáp án đúng", "Essay" };

        private readonly IHostingEnvironment _env;
        private readonly IRoxyFilemanHandler _roxyFilemanHandler;

        private string RootPath { get; }
        private string StaticPath { get; }
        private string currentHost { get; }


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
            LessonScheduleService lessonScheduleService,
            //LessonPartService lessonPartService,
            //LessonPartQuestionService lessonPartQuestionService,
            //LessonPartAnswerService lessonPartAnswerService,
            CloneLessonPartService service,
            CloneLessonPartAnswerService cloneLessonPartAnswerService,
            CloneLessonPartQuestionService cloneLessonPartQuestionService,
            FileProcess fileProcess,
            VocabularyService vocabularyService,

            IConfiguration config,
            IHostingEnvironment env,
            IRoxyFilemanHandler roxyFilemanHandler,
            IHttpContextAccessor httpContextAccessor
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
            _lessonScheduleService = lessonScheduleService;

            _lessonpartMapping = new MappingEntity<LessonPartEntity, CloneLessonPartEntity>();
            _lessonpartQuestionMapping = new MappingEntity<LessonPartQuestionEntity, CloneLessonPartQuestionEntity>();
            _lessonpartAnswerMapping = new MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity>();
            _cloneLessonPartViewModelMapping = new MappingEntity<CloneLessonPartEntity, CloneLessonPartViewModel>();
            _cloneQuestionViewModelMapping = new MappingEntity<LessonPartQuestionEntity, CloneQuestionViewModel>();
            _fileProcess = fileProcess;
            _vocabularyService = vocabularyService;

            _env = env;
            RootPath = (config.GetValue<string>("SysConfig:StaticPath") ?? env.WebRootPath) + "/Files";
            StaticPath = (config.GetValue<string>("SysConfig:StaticPath") ?? env.WebRootPath);
            _roxyFilemanHandler = roxyFilemanHandler;
            currentHost = httpContextAccessor.HttpContext.Request.Host.Value;
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
                    //var file = files.Where(f => f.Name == item.Media.Name).SingleOrDefault();
                    //if (file != null)
                    //{
                    //    item.Media.Created = DateTime.UtcNow;
                    //    item.Media.Size = file.Length;
                    //    item.Media.Path = await _fileProcess.SaveMediaAsync(file, item.Media.OriginalName, "", basis);
                    //}
                    if (item.Type == "IMG")
                    {
                        if (item.Media.Name.ToLower().StartsWith("http")) //file url (import)
                        {
                            item.Media.Created = DateTime.UtcNow;
                            item.Media.Size = 0;
                            item.Media.Path = item.Media.Name.Trim();
                            item.Media.Extension = "image/png";
                        }
                        else
                        {
                            var file = files.Where(f => f.Name == item.Media.Name).FirstOrDefault();
                            if (file != null)
                            {
                                item.Media.Created = DateTime.UtcNow;
                                item.Media.Size = file.Length;
                                item.Media.Path = await _fileProcess.SaveMediaAsync(file, item.Media.OriginalName, "", basis);
                                item.Media.Extension = "image/png";
                            }
                        }
                    }
                    else
                    {
                        //foreach (var file in files)
                        //{
                        var file = files.Where(f => f.Name == item.Media.Name).FirstOrDefault();
                        string extension = Path.GetExtension(file.FileName);

                        item.Media = new Media();
                        item.Media.Name = item.Media.OriginalName = file.FileName;
                        item.Media.Created = DateTime.UtcNow;
                        item.Media.Size = file.Length;

                        if (file.FileName.ToLower().EndsWith(".ppt") || file.FileName.ToLower().EndsWith(".pptx"))
                        {
                            item.Media.Path = await _fileProcess.SaveMediaAsync(file, item.Media.OriginalName, "", basis);
                            item.Media.Extension = extension;
                        }
                        else
                        {
                            if (!typeImage.Contains(extension))
                            {
                                var mediarsp = _roxyFilemanHandler.UploadSingleFileWithGoogleDrive(basis, createduser, file);
                                item.Media.Path = mediarsp.Path;
                                if (typeVideo.Contains(extension))
                                {
                                    item.Media.Extension = "video/mp4";
                                }
                                else if (typeAudio.Contains(extension))
                                {
                                    item.Media.Extension = "audio/mp3";
                                }
                                else
                                {
                                    item.Media.Extension = extension;
                                }
                            }
                            else
                            {
                                item.Media.Path = await _fileProcess.SaveMediaAsync(file, item.Media.OriginalName, "", basis);
                                item.Media.Extension = "image/png";
                            }
                        }
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

        //ai laij cmt ham nay vao the @@
        private double calculateLessonPoint(string lessonId)
        {
            var point = 0.0;
            var parts = _cloneLessonPartService.GetByLessonID(lessonId).Where(t => quizType.Contains(t.Type));
            foreach (var part in parts)
            {
                if (part.Type == "ESSAY")
                {
                    point += part.Point;
                    _cloneQuestionService.Collection.UpdateMany(t => t.ParentID == part.ID, Builders<CloneLessonPartQuestionEntity>.Update.Set(t => t.Point, part.Point));
                }
                else
                {
                    point += _cloneQuestionService.GetByPartID(part.ID).Count();//trắc nghiệm => điểm = số câu hỏi (mỗi câu 1đ)
                    _cloneQuestionService.Collection.UpdateMany(t => t.ParentID == part.ID, Builders<CloneLessonPartQuestionEntity>.Update.Set(t => t.Point, 1));
                }
            }
            _lessonService.UpdateLessonPoint(lessonId, point);
            return point;
        }

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
                        if (item.Questions == null || item.Questions.Count == 0)
                        {
                            var newdescription = "";
                            item.Questions = ExtractFillQuestionList(item, createduser, out newdescription);
                            lessonpart.Description = newdescription;
                            _cloneLessonPartService.CreateQuery().ReplaceOne(t => t.ID == lessonpart.ID, lessonpart);
                        }
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

        #region Auto Create Exam
        public JsonResult AutoCreateExam(String basis, LessonEntity item, DateTime startTime, DateTime endTime, List<String> classSubjectIDs, Int32 TotalPart)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                if (!classSubjectIDs.Any())
                {
                    return Json(new Dictionary<String, Object>
                    {
                        {"Status",false },
                        {"Message","Chưa chọn môn nào" },
                        {"Data",null }
                    });
                }

                var lessonIDs = _lessonScheduleService.CreateQuery().Find(x => classSubjectIDs.Contains(x.ClassSubjectID) && x.StartDate <= endTime && x.EndDate >= startTime).Project(x => x.LessonID);
                if (lessonIDs.CountDocuments() == 0)
                {
                    return Json(new Dictionary<String, Object>
                    {
                        {"Status",false },
                        {"Message","Chưa chọn môn nào" },
                        {"Data",null }
                    });
                }

                var cloneLessonParts = _cloneLessonPartService.GetItemsByLessonIDs(lessonIDs.ToList<String>());
                if(cloneLessonParts.Count() < TotalPart)
                {
                    return Json(new Dictionary<String, Object>
                    {
                        {"Status",false },
                        {"Message","Số lượng câu hỏi không đủ" },
                        {"Data",null }
                    });
                }

                var index = 0;
                var indexs = RandomIndex(TotalPart, cloneLessonParts.Count());
                List<CloneLessonPartEntity> newCloneLessonParts = new List<CloneLessonPartEntity>();
                newCloneLessonParts.AddRange(GetLessonParts(indexs, cloneLessonParts.ToList()));
                while(newCloneLessonParts.Count() < indexs.Count()) 
                {
                    var newIndexs = RandomIndex(indexs.Count() - newCloneLessonParts.Count(), cloneLessonParts.Count());
                    while(indexs.Any(x => newIndexs.Contains(x)))
                    {
                        newIndexs = RandomIndex(indexs.Count() - newCloneLessonParts.Count(), cloneLessonParts.Count());
                    }
                    newCloneLessonParts.AddRange(GetLessonParts(newIndexs, cloneLessonParts.ToList()));
                    index++;
                    if (index == cloneLessonParts.Count())
                    {
                        return Json(new Dictionary<String, Object>
                        {
                            {"Status",false },
                            {"Message","Số lượng câu hỏi không đủ" },
                            {"Data",null }
                        });
                    }
                }

                item.ChapterID = "0";
                item.TemplateType = LESSON_TEMPLATE.EXAM;
                item.IsPractice = false;
                var classsbjExam = _classSubjectService.GetClassSubjectExamByClassID(item.ClassID);
                item.ClassSubjectID = classsbjExam == null ? "0" : classsbjExam.ID;

                var lesson = CreateLesson(item, UserID);
                if (lesson == null)
                {
                    return Json(new Dictionary<String, Object>
                    {
                        {"Status",true },
                        {"Message","Lỗi không tạo bài được" },
                        {"Data",null }
                    });
                }

                foreach (var lessonPart in newCloneLessonParts)
                {
                    _ = CopyPartToLesson(basis, lessonPart, lesson, UserID);
                }

                return Json(new Dictionary<String, Object>
                {
                    {"Status",true },
                    {"Message","Tạo thành công" },
                    {"Data",lesson }
                });
            }
            catch (Exception ex)
            {
                return Json(new Dictionary<String, Object>
                {
                    {"Status",false },
                    {"Message",ex.Message },
                    {"Data",null }
                });
            }
        }

        private List<CloneLessonPartEntity> GetLessonParts(List<int> indexs, List<CloneLessonPartEntity> cloneLessonParts)
        {
            List<CloneLessonPartEntity> newCloneLessonParts = new List<CloneLessonPartEntity>();
            foreach (var index in indexs)
            {
                var cloneLessonPart = cloneLessonParts.ElementAtOrDefault(index);
                if (!quizType.Contains(cloneLessonPart.Type))
                {
                    continue;
                }
                if (cloneLessonPart != null)
                {
                    newCloneLessonParts.Add(cloneLessonPart);
                }
            }
            return newCloneLessonParts;
        }

        private LessonEntity CreateLesson(LessonEntity item, String UserID)
        {
            LessonEntity data = null;
            if (!string.IsNullOrEmpty(item.ID))
                data = _lessonService.GetItemByID(item.ID);
            if (data == null)
            {
                item.Created = DateTime.UtcNow;
                item.CreateUser = UserID;
                item.IsAdmin = true;
                item.IsActive = false;
                item.IsParentCourse = item.ChapterID.Equals("0");
                item.Updated = DateTime.UtcNow;
                item.Order = 0;

                _lessonHelper.InitLesson(item);//insert + create schedule (no route)

                ChangeLessonPosition(item, Int32.MaxValue);//move lesson to bottom of parent

                //update total lesson to parent chapter
                _classHelper.IncreaseLessonCounter(item, 1, item.TemplateType == LESSON_TEMPLATE.EXAM ? 1 : 0, 0).Wait();

                return item;
            }
            else
            {
                if (!_lessonHelper.isExamined(data))
                {
                    return data;
                }

                data.Updated = DateTime.UtcNow;
                var newOrder = item.Order - 1;
                data.Order = item.Order;
                data.ClassID = item.ClassID;
                data.ClassSubjectID = item.ClassSubjectID;
                data.Timer = item.Timer;
                data.Limit = item.Limit;
                data.Title = item.Title;
                data.Multiple = item.Multiple;
                data.Etype = item.Etype;
                //item.Point = data.Point;

                //update counter if type change
                if (item.TemplateType != data.TemplateType)
                {
                    data.TemplateType = item.TemplateType;
                    var examInc = 0;
                    var pracInc = 0;
                    if (_lessonHelper.IsQuizLesson(data.ID)) pracInc = 1;
                    if (data.TemplateType == LESSON_TEMPLATE.LECTURE) // EXAM => LECTURE
                    {
                        examInc = -1;
                        data.IsPractice = pracInc == 1;
                    }
                    else
                    {
                        examInc = 1;
                        data.IsPractice = false;
                        pracInc = pracInc == 1 ? -1 : 0;
                    }
                    _classHelper.IncreaseLessonCounter(data, 0, examInc, pracInc).Wait();
                }

                _lessonService.CreateQuery().ReplaceOne(o => o.ID == data.ID, data);

                if (data.Order != newOrder)//change Position
                {
                    ChangeLessonPosition(data, newOrder);
                }
                return data;
            }
        }

        public List<Int32> RandomIndex(Int32 TotalIndex,Int32 max)
        {
            var rd = new Random();
            List<Int32> listIndex = new List<int>();
            do
            {
                Int32 index = rd.Next(0, max);
                if (!listIndex.Contains(index))
                {
                    listIndex.Add(index);
                }
            }
            while (listIndex.Count() != TotalIndex);
            return listIndex;
        }

        private int ChangeLessonPosition(LessonEntity item, int pos)
        {
            var parts = _lessonService.GetChapterLesson(item.ClassSubjectID, item.ChapterID);
            var ids = parts.Select(o => o.ID).ToList();

            var oldPos = ids.IndexOf(item.ID);
            if (oldPos == pos && oldPos == item.Order)
            {
                return oldPos;
            }
            if (pos > parts.Count())
                pos = parts.Count() - 1;
            item.Order = pos;

            _lessonService.CreateQuery().ReplaceOne(o => o.ID == item.ID, item);
            int entry = -1;
            foreach (var part in parts)
            {
                if (part.ID == item.ID) continue;
                if (entry == pos - 1)
                    entry++;
                entry++;
                part.Order = entry;
                _lessonService.CreateQuery().ReplaceOne(o => o.ID == part.ID, part);
            }
            return pos;
        }
        #endregion


        ////////////////////////////////////////////////////////////////////
        #region Import By Word
        public async Task<JsonResult> ImportFromWord(string basis = "", string ParentID = "")
        {
            Boolean Status = false;
            try
            {
                var form = HttpContext.Request.Form;
                if (form == null || form.Files == null || form.Files.Count <= 0)
                    return new JsonResult(new Dictionary<string, object> { { "Error", "Chưa chọn file" } });


                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var file = form.Files[0];
                var dirPath = "Upload\\Quiz";

                if (!Directory.Exists(Path.Combine(_env.WebRootPath, dirPath)))
                    Directory.CreateDirectory(Path.Combine(_env.WebRootPath, dirPath));
                var filePath = Path.Combine(_env.WebRootPath, dirPath + "\\" + DateTime.Now.ToString("ddMMyyyyhhmmss") + file.FileName);


                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    String Msg = "";
                    await file.CopyToAsync(stream);
                    stream.Close();
                    using (var readStream = new FileStream(filePath, FileMode.Open))
                    {
                        //Create a Document instance and load the word document
                        Document document = new Document(readStream);

                        //Get the first session
                        var sw = document.Sections[0];
                        Int32 countTable = sw.Body.Tables.Count;

                        for (int indexTable = 0; indexTable < sw.Body.Tables.Count; indexTable++)
                        {
                            //Get the first table in the textbox
                            Table table = sw.Body.Tables[indexTable] as Table;

                            //var parentLesson = _lessonService.GetItemByID(ParentID);
                            //var isPractice = parentLesson.IsPractice;

                            var item = new CloneLessonPartViewModel();

                            String title = table.Rows[0].Cells[1].Paragraphs[0].Text?.ToString().Trim();

                            item.Title = title;
                            item.Created = DateTime.UtcNow;
                            item.Updated = DateTime.UtcNow;
                            //item.Description = description;
                            item.Point = 0;//đếm câu hỏi
                            item.ParentID = ParentID;

                            var maxItem = _cloneLessonPartService.CreateQuery()
                            .Find(o => o.ParentID == item.ParentID)
                            .SortByDescending(o => o.Order).FirstOrDefault();
                            item.Order = maxItem != null ? maxItem.Order + 1 : 0;

                            //check type
                            var typeRow = table.Rows[2];

                            for (int indexCell = 1; indexCell < typeRow.Cells.Count; indexCell++)
                            {
                                var txtCell = typeRow.Cells[indexCell].Paragraphs[0].Text.ToString().ToUpper();
                                if (txtCell.Contains("X"))
                                {
                                    //type = table.Rows[1].Cells[indexCell].Paragraphs[0].Text.ToString().ToUpper();
                                    item.Type = partTypes[indexCell - 1];
                                    break;
                                }
                            }

                            switch (item.Type)
                            {
                                case "TEXT":
                                case "AUDIO":
                                case "VIDEO":
                                case "DOC":
                                    Msg += (await GetContentOther(table, item.Type, basis, item, UserID));
                                    Status = true;
                                    break;
                                case "IMAGE":
                                    Msg += (await GetContentIMG(table, item.Type, basis, item, UserID));
                                    Status = true;
                                    break;
                                case "VOCAB":
                                    Msg += (await GetContentVocab(table, item.Type, basis, item, UserID));
                                    Status = true;
                                    break;
                                case "QUIZ1":
                                case "QUIZ3":
                                case "QUIZ4":
                                    Msg += (await GetContentQUIZ(table, item.Type, basis, item, UserID));
                                    Status = true;
                                    break;
                                case "QUIZ2":
                                    item.Description = "";
                                    Msg += (await GetContentQuiz2(table, item.Type, basis, item, UserID));
                                    Status = true;
                                    break;
                                case "ESSAY":
                                    item.Type = "ESSAY";
                                    Msg += (await GetContentEssay(table, item.Type, basis, item, UserID));
                                    Status = true;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    _lessonHelper.calculateLessonPoint(ParentID);

                    System.IO.File.Delete(filePath);
                    return new JsonResult(new Dictionary<string, object>
                    {
                        //{ "Data", full_item },
                        {"Msg", Msg },
                        {"Stt",Status == string.IsNullOrEmpty(Msg) }
                    });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new Dictionary<string, object>
                    {
                        //{ "Data", full_item },
                        {"Msg", ex.Message },
                        {"Stt",false }
                    });
            }
        }

        private Dictionary<String, String> GetContentFile(Spire.Doc.Collections.ParagraphCollection documentObject, String basis, String createUser)
        {
            Dictionary<String, String> dataResponse = new Dictionary<String, String>();
            foreach (DocumentObject docObject in documentObject[0].ChildObjects)
            {
                if (docObject.DocumentObjectType == DocumentObjectType.Picture)
                {
                    DocPicture picture = docObject as DocPicture;
                    string fileName = string.Format($"Image{DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")}.png");
                    var pathImage = SaveImageByByteArray(picture.ImageBytes, fileName, createUser, basis);
                    dataResponse.Add("FileName", fileName);
                    dataResponse.Add("FilePath", pathImage);
                }
            }
            return dataResponse;
        }

        private async Task<string> GetContentFile(string basis, CloneLessonPartViewModel item, string createUser, TableCell linkcell, string linkfile)
        {
            foreach (Paragraph prg in linkcell.Paragraphs)
            {

                foreach (DocumentObject obj in prg.ChildObjects)
                {

                    if (obj is Field)
                    {
                        var link = obj as Field;
                        if (link.Type == FieldType.FieldHyperlink)
                        {
                            item.Media = new Media
                            {
                                Created = DateTime.UtcNow,
                                Path = link.Value.Replace("\"", ""),
                                OriginalName = link.FieldText,
                                Name = link.FieldText,
                                Extension = FileProcess.GetContentType(link.FieldText)
                            };
                            break;
                        }
                    }
                    else if (obj is DocPicture)
                    {
                        var pic = obj as DocPicture;
                        var filename = DateTime.Now.ToString("yyyyMMddhhmmssfff") + ".jpg";
                        var path = SaveImageByByteArray(pic.ImageBytes, filename, createUser, basis);

                        item.Media = new Media()
                        {
                            Created = DateTime.UtcNow,
                            Path = path,
                            OriginalName = filename,
                            Name = filename,
                            Extension = "image/jpg"
                        };
                        break;
                    }
                    else if (obj is DocOleObject)
                    {
                        Dictionary<String, String> pathFileOLE = await GetPathFileOLE(obj as DocOleObject, basis, createUser);
                        item.Media = new Media()
                        {
                            Created = DateTime.UtcNow,
                            Path = pathFileOLE["pathFileOLE"],
                            OriginalName = pathFileOLE["packageFileName"],
                            Name = pathFileOLE["packageFileName"],
                            Extension = FileProcess.GetContentType(pathFileOLE["extension"])
                        };
                        break;
                    }
                    else if (obj is TextRange)
                    {
                        var str = (obj as TextRange).Text;
                        if (str.ToLower().StartsWith("http"))
                        {
                            linkfile = str.Trim();
                            var contentType = FileProcess.GetContentType(linkfile);
                            if (contentType == "application/octet-stream")//unknown type
                            {
                                switch (item.Type)
                                {
                                    case "AUDIO":
                                        contentType = "audio/mp3";
                                        break;
                                    case "VIDEO":
                                        contentType = "video/mpeg";
                                        break;
                                    case "DOC":
                                        contentType = "application/pdf";
                                        break;
                                    case "IMAGE":
                                        contentType = "image/jpeg";
                                        break;
                                }
                            }
                            item.Media = new Media()
                            {
                                Created = DateTime.UtcNow,
                                Path = linkfile,
                                OriginalName = linkfile,
                                Name = linkfile,
                                Extension = contentType
                            };
                            break;
                        }
                    }
                }
            }

            return linkfile;
        }

        private async Task<String> GetContentOther(Table table, String type, String basis, CloneLessonPartViewModel item, String createUser = null)
        {
            try
            {
                //description
                String html = await StringHelper.ConvertDocToHtml(table, basis, createUser, StaticPath);
                item.Description = html;

                //file 
                var linkcell = table.Rows[4].Cells[1];

                var linkfile = "";
                linkfile = await GetContentFile(basis, item, createUser, linkcell, linkfile);

                await CreateOrUpdate(basis, item);
                return "";
            }
            catch (Exception ex)
            {
                return $"{type} is error {ex.Message}";
            }
        }

        private async Task CreateOrUpdate(string basis, CloneLessonPartViewModel item)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var parentLesson = _lessonService.GetItemByID(item.ParentID);
                var createduser = User.Claims.GetClaimByType("UserID").Value;
                if (parentLesson != null)
                {
                    var isPractice = parentLesson.IsPractice;
                    var maxItem = _cloneLessonPartService.CreateQuery()
                            .Find(o => o.ParentID == item.ParentID)
                            .SortByDescending(o => o.Order).FirstOrDefault();

                    item.CourseID = parentLesson.CourseID;
                    item.ClassID = parentLesson.ClassID;
                    item.ClassSubjectID = parentLesson.ClassSubjectID;

                    item.Created = DateTime.UtcNow;
                    item.Order = maxItem != null ? maxItem.Order + 1 : 0;
                    item.Updated = DateTime.UtcNow;
                    item.TeacherID = createduser;

                    var lessonpart = item.ToEntity();
                    _cloneLessonPartService.CreateQuery().InsertOne(lessonpart);
                    item.ID = lessonpart.ID;

                    switch (lessonpart.Type)
                    {
                        case "QUIZ2": //remove all previous question

                            if (item.Questions != null && item.Questions.Count > 0)
                            {
                                await SaveQA(item, UserID);
                            }
                            isPractice = true;
                            break;
                        default://QUIZ1,3,4

                            if (item.Questions != null && item.Questions.Count > 0)
                            {
                                await SaveQA(item, UserID);
                            }
                            isPractice = true;
                            break;
                    }

                    if (parentLesson.TemplateType == LESSON_TEMPLATE.LECTURE && parentLesson.IsPractice != isPractice)//non-practice => practice
                    {
                        parentLesson.IsPractice = isPractice;
                        _lessonService.Save(parentLesson);
                        //increase practice counter
                        await _classHelper.ChangeLessonPracticeState(parentLesson);
                    }
                    parentLesson.Point = calculateLessonPoint(item.ParentID);
                    _lessonService.Save(parentLesson);
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                String msg = ex.Message;
            }
        }


        private async Task<String> GetContentQUIZ(Table table, String type, String basis, CloneLessonPartViewModel item, String createUser = null)
        {
            try
            {
                //List<QuestionViewModel> Quiz = new List<QuestionViewModel>();
                Dictionary<String, CloneQuestionViewModel> Quiz = new Dictionary<string, CloneQuestionViewModel>();
                Dictionary<String, List<AnswerViewModel>> listAns = new Dictionary<string, List<AnswerViewModel>>();
                Int32 pos = -1;
                Int32 indexQuiz = 1;
                Int32 indexAns = 1;
                Int32 indexEx = 1;

                var paragraphs = table.Rows[3].Cells[1].Paragraphs;
                var objs = table.Rows[3].Cells[1].ChildObjects;
                foreach (Paragraph para in paragraphs)
                {
                    var content = para.Text.Trim();
                    if (content.Contains($"[Q{indexQuiz}]"))//cau hoi
                    {
                        var question = new CloneQuestionViewModel
                        {
                            ParentID = item.ID,
                            ClassID = item.ClassID,
                            ClassSubjectID = item.ClassSubjectID,
                            CourseID = item.CourseID,
                            Order = item.Order,
                            Created = DateTime.UtcNow,
                            Updated = DateTime.UtcNow,
                            CreateUser = createUser,
                            Point = 1,
                            Answers = new List<CloneLessonPartAnswerEntity>() { },//danh sach cau tra loi,
                        };
                        foreach (DocumentObject obj in para.ChildObjects)
                        {
                            if (obj is TextRange)
                            {
                                question.Content += (obj as TextRange).Text.Replace($"[Q{indexQuiz}]", "");
                            }
                            else if (obj is DocPicture)
                            {
                                var pic = obj as DocPicture;
                                var fileName = $"{DateTime.Now.ToString("yyyyMMddhhmmssfff")}_{indexQuiz}.jpg";
                                var path = SaveImageByByteArray(pic.ImageBytes, fileName, createUser, basis);

                                question.Media = new Media()
                                {
                                    Created = DateTime.UtcNow,
                                    Path = path,
                                    OriginalName = fileName,
                                    Name = fileName,
                                    Extension = "image/jpg"
                                };
                            }
                        }
                        Quiz.Add($"[Q{indexQuiz}]", question);
                        pos++;
                        indexQuiz++;
                    }
                    else if (content.Contains($"[A{indexAns}]"))
                    {
                        var lstAns = para.Text.Replace($"[Đáp án]\v[A{indexAns}]", "").Replace($"[A{indexAns}]", "").Trim().Split('|');
                        foreach (var _ans in lstAns)
                        {
                            var ans = new CloneLessonPartAnswerEntity
                            {
                                ClassID = item.ClassID,
                                ClassSubjectID = item.ClassSubjectID,
                                CourseID = item.CourseID,
                                CreateUser = createUser,
                                Created = DateTime.UtcNow,
                                Updated = DateTime.UtcNow,
                                //Media = null,
                                Content = _ans.Replace("(x)", "").Replace("(X)", "").Trim(),
                                IsCorrect = _ans.Trim().ToUpper().Contains("X") ? true : false,
                            };
                            Quiz[$"[Q{indexAns}]"].Answers.Add(ans);
                        }
                        foreach (DocumentObject obj in para.ChildObjects)
                        {
                            if (obj is DocPicture)
                            {
                                var pic = obj as DocPicture;
                                var previousSibling = (obj.PreviousSibling as TextRange).Text.Replace($"[A{indexAns}]", "").Replace($"[E{indexAns}]", "").Replace("|", "").Replace("(X)", "").Replace("(x)", "").Trim();
                                var fileName = $"{DateTime.Now.ToString("yyyyMMddhhmmssfff")}_ans{previousSibling}_{indexQuiz}_{indexAns}.jpg";
                                var path = SaveImageByByteArray(pic.ImageBytes, fileName, createUser, basis);
                                var _test = lstAns.Contains(previousSibling);
                                var ansWmedia = Quiz[$"[Q{indexAns}]"].Answers.Find(x => x.Content.Contains(previousSibling));
                                ansWmedia.Media = new Media
                                {
                                    Name = fileName,
                                    OriginalName = fileName,
                                    Path = path,
                                    Extension = "image/jpg",
                                    //Extension = System.IO.Path.GetExtension(path),
                                    Created = DateTime.Now,
                                    Size = pic.Size.Width * pic.Size.Height
                                };
                            }
                        }
                        indexAns++;
                    }
                    else if (content.Contains($"[E{indexEx}]"))
                    {
                        var str = content.Replace($"[E{indexEx}]", "").Trim();
                        if (Quiz.ContainsKey($"[Q{indexEx}]"))
                        {
                            if (!String.IsNullOrEmpty(str))
                            {
                                Quiz[$"[Q{indexEx}]"].Description += $"<p>{str}</p>";
                            }
                            while ((para.NextSibling as Paragraph) != null && !(para.NextSibling as Paragraph).Text.Contains($"[A{indexEx + 1}]"))
                            {
                                Quiz[$"[Q{indexEx}]"].Description += $"<p>{(para.NextSibling as Paragraph)?.Text}</p>";
                                paragraphs.Remove(para.NextSibling as Paragraph);
                            }
                            indexEx++;
                        }
                    }
                }

                //desription
                //foreach (Paragraph _para in paragraphs)
                //{
                //    //var _para = paragraphs[0];
                //    while (_para != null && (_para.Text.Trim().ToUpper().Contains("[CÂU HỎI]") || _para.Text.Trim().ToUpper().Contains("[ĐÁP ÁN]")))
                //    {
                //        if (_para.NextSibling != null)
                //        {
                //            paragraphs.Remove((_para.NextSibling as Paragraph));
                //        }
                //        else
                //        {
                //            paragraphs.Remove(_para);
                //        }
                //    }
                //}

                Paragraph nextPrg = paragraphs[0];
                //foreach (Paragraph para in paragraphs)
                while (nextPrg != null)
                {
                    Paragraph para = nextPrg;
                    var content = para.Text.Trim();
                    if (content.ToUpper().Contains("[CÂU HỎI]") || content.ToUpper().Contains("[ĐÁP ÁN]"))
                    {
                        var nextObj = para.NextSibling;
                        while (nextObj != null)
                        {
                            var next = nextObj.NextSibling;
                            objs.Remove(nextObj);
                            nextObj = next;
                        }
                        paragraphs.Remove(para);
                        break;
                    }
                    else
                    {
                        nextPrg = nextPrg.NextSibling as Paragraph;
                    }
                }

                item.Description = await StringHelper.ConvertDocToHtml(table, basis, createUser, StaticPath);

                //file 
                var linkcell = table.Rows[4].Cells[1];

                var linkfile = "";
                linkfile = await GetContentFile(basis, item, createUser, linkcell, linkfile);

                var lpq = new List<CloneQuestionViewModel>();
                for (int i = 0; i < Quiz.Count; i++)
                {
                    var quiz = Quiz.ContainsKey($"[Q{i + 1}]") ? Quiz[$"[Q{i + 1}]"] : null;
                    if (quiz != null)
                    {
                        quiz.Content = quiz.Content.Contains($"[Q{i + 1}]") ? quiz.Content.Replace($"[Q{i + 1}]", "") : quiz.Content;
                        lpq.Add(quiz);
                    }
                }

                item.Questions = lpq;
                await CreateOrUpdate(basis, item);

                return "";
            }
            catch (Exception ex)
            {
                return "Type QUIZ 134 has error: " + ex.Message;
            }
        }

        private async Task<String> GetContentIMG(Table table, String type, String basis, CloneLessonPartViewModel item, String createUser = null)
        {
            try
            {
                var totalRows = table.Rows.Count;
                for (int indexRow = 0; indexRow < totalRows; indexRow++)
                {
                    var contentRow = table.Rows[indexRow];
                    String contentCell0 = contentRow.Cells[0].Paragraphs[0].Text?.ToString().Trim().ToLower();

                    if (contentCell0.Equals("file"))
                    {
                        item.Media = new Media();
                        var contentFileImage = GetContentFile(contentRow.Cells[1].Paragraphs, basis, createUser);
                        if (contentFileImage.Count > 0)
                        {
                            item.Media.Created = DateTime.Now;
                            item.Media.Name = contentFileImage["FileName"];
                            item.Media.Path = contentFileImage["FilePath"];
                            item.Media.Extension = "image/png";
                        }
                        else
                        {
                            item.Media = new Media();
                        }
                        break;
                    }
                    else continue;
                }
                _cloneLessonPartService.CreateOrUpdate(item);
                return "";
            }
            catch (Exception ex)
            {
                return "Type IMG has error: " + ex.Message;
            }
        }

        private async Task<String> GetContentVocab(Table table, String type, String basis, CloneLessonPartViewModel item, String createUser = null)
        {
            try
            {
                var descriptionCell = table.Rows[3].Cells[1];
                var desc = descriptionCell.FirstParagraph.Text;

                var vocabArr = desc.Split('|');
                if (vocabArr != null && vocabArr.Length > 0)
                {
                    foreach (var vocab in vocabArr)
                    {
                        var vocabulary = vocab.Trim().ToLower();
                        _ = GetVocabByCambridge(vocabulary);
                    }
                }
                item.Description = desc;

                await CreateOrUpdate(basis, item);
                return $"";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private async Task<String> GetContentQuiz2(Table table, String type, String basis, CloneLessonPartViewModel item, String createUser = null)
        {
            try
            {
                Dictionary<String, String> _listAns = new Dictionary<String, String>();
                Dictionary<String, String> _listH = new Dictionary<String, String>();
                Dictionary<String, String> _listEx = new Dictionary<String, String>();
                var descCell = table.Rows[3].Cells[1];
                var paragraphs = descCell.Paragraphs;
                Int32 indexquiz = 1, indexans = 1, indexH = 1, indexEx = 1;
                Paragraph nextPrg = paragraphs[0];
                //foreach (Paragraph para in paragraphs)
                while (nextPrg != null)
                {
                    Paragraph para = nextPrg;
                    nextPrg = para.NextSibling as Paragraph;
                    var content = para.Text.Trim();
                    String ex = "";
                    String h = "";
                    if (para.Text.ToUpper().Contains("[CÂU HỎI]")) descCell.Paragraphs.Remove(para);
                    else if (para.Text.Trim().Contains($"[A{indexans}]"))
                    {
                        indexH = indexans;
                        indexEx = indexans;
                        String str = content.Replace($"[A{indexans}]", "").Trim();
                        _listAns.Add($"[A{indexans}]", str);
                        indexans++;
                        descCell.Paragraphs.Remove(para);
                    }
                    else if (para.Text.Trim().Contains($"[H{indexH}]"))
                    {
                        indexEx = indexH;
                        var str = content.Replace($"[H{indexH}]", "").Trim();
                        h += str;
                        while (para.NextSibling != null && (
                            !(para.NextSibling as Paragraph).Text.Contains($"[A{indexH + 1}]")
                            && !(para.NextSibling as Paragraph).Text.Contains($"[E{indexH + 1}]")
                            && !(para.NextSibling as Paragraph).Text.Contains($"[E{indexH}]")))
                        {
                            h += (para.NextSibling as Paragraph)?.Text + " ";
                            descCell.Paragraphs.Remove(para.NextSibling as Paragraph);
                        }
                        _listH.Add($"H{indexH}", h);
                        indexH++;
                        nextPrg = para.NextSibling as Paragraph;
                        descCell.Paragraphs.Remove(para);
                    }
                    else if (para.Text.Trim().Contains($"[E{indexEx}]"))
                    {
                        var str = content.Replace($"[E{indexEx}]", "").Trim();
                        ex += str;
                        while (para.NextSibling != null && (!(para.NextSibling as Paragraph).Text.Contains($"[A{indexEx + 1}]") && !(para.NextSibling as Paragraph).Text.Contains($"[H{indexEx + 1}]")))
                        {
                            ex += (para.NextSibling as Paragraph)?.Text + " ";
                            descCell.Paragraphs.Remove(para.NextSibling as Paragraph);
                        }
                        _listEx.Add($"E{indexEx}", ex);
                        indexEx++;
                        nextPrg = para.NextSibling as Paragraph;
                        descCell.Paragraphs.Remove(para);
                    }
                    else if (para.Text.ToUpper().Contains("[ĐÁP ÁN]")) descCell.Paragraphs.Remove(para);

                }

                var indexQ = 1;
                for (Int32 i = 0; i < paragraphs.Count; i++)
                {
                    var paraText = descCell.Paragraphs[i].Text.Trim();
                    while (paraText.Contains($"_Q{indexQ}_"))
                    {
                        descCell.Paragraphs[i].Replace($"_Q{indexQ}_", $"EDUSOQUIZ2_Q{indexQ}_", true, false);
                        indexQ++;
                    }
                }

                String description = await StringHelper.ConvertDocToHtml(table, basis, createUser, StaticPath);
                foreach (var a in _listAns)
                {
                    var ex = _listEx.ContainsKey($"E{indexquiz}") ? _listEx[$"E{indexquiz}"] : "";
                    var h = _listH.ContainsKey($"H{indexquiz}") ? _listH[$"H{indexquiz}"] : "";
                    var str = a.Value.Replace($"[A{indexquiz}]", "").Trim();
                    String replace3 = $"EDUSOQUIZ2_Q{indexquiz}_";
                    String replace2 = $"<fillquiz contenteditable=\"false\" readonly=\"readonly\" title=\"{ex}\"><input ans=\"{str}\" class=\"fillquiz\" contenteditable=\"false\" dsp=\"{h}\" placeholder=\"{ex}\" readonly=\"readonly\" type=\"text\" value=\"{str}\"/></fillquiz>";
                    description = description.ToString().Replace(replace3, replace2);
                    indexquiz++;
                }
                item.Description = description;

                var newdescription = "";
                item.Questions = ExtractFillQuestionList(item, createUser, out newdescription);
                item.Description = newdescription.ToString();

                //file 
                var linkcell = table.Rows[4].Cells[1];

                var linkfile = "";
                linkfile = await GetContentFile(basis, item, createUser, linkcell, linkfile);

                await CreateOrUpdate(basis, item);

                return $"";
            }
            catch (Exception ex)
            {
                return $"{type} has error {ex.Message}";
            }
        }

        private async Task<String> GetContentEssay(Table table, String type, String basis, CloneLessonPartViewModel item, String createUser = null)
        {
            try
            {
                String descriptionLessonPart = "";
                String descriptionQUIZ = "";
                Double point = 0;

                var descCell = table.Rows[3].Cells[1];
                var paragraphs = descCell.Paragraphs;
                DocumentObject nextPrg = paragraphs[0];

                //lay mo ta
                while (nextPrg != null)
                {
                    if (!(nextPrg is Paragraph))
                    {
                        nextPrg = nextPrg.NextSibling as DocumentObject;
                        continue;
                    }
                    Paragraph para = nextPrg as Paragraph;
                    nextPrg = para.NextSibling as DocumentObject;

                    var content = para.Text.Trim();
                    //if (para.Text.ToUpper().Contains("[DES]"))
                    //    descCell.Paragraphs.Remove(para);
                    //else if (para.Text.Trim().ToUpper().Contains($"[EX]"))
                    if (para.Text.Trim().ToUpper().Contains($"[E]"))
                    {
                        var str = content.Replace($"[E]", "").Trim();
                        descriptionQUIZ += str;
                        while (para.NextSibling != null && (
                            !(para.NextSibling as Paragraph).Text.Contains($"[E]")
                            && !(para.NextSibling as Paragraph).Text.Contains($"[P]")))
                        {
                            descriptionQUIZ += (para.NextSibling as Paragraph)?.Text + " ";
                            if (!(para.NextSibling as Paragraph).Text.Trim().Contains("[P]"))
                            {
                                descCell.Paragraphs.Remove(para.NextSibling as Paragraph);
                            }
                        }
                        nextPrg = para.NextSibling as DocumentObject;
                        descCell.Paragraphs.Remove(para);
                    }
                    else if (para.Text.Trim().ToUpper().Contains($"[P]"))
                    {
                        var str = content.Replace($"[P]", "").Trim();
                        if (!String.IsNullOrEmpty(str))
                        {
                            Double.TryParse(str, out point);
                            while (para.NextSibling != null)
                                descCell.Paragraphs.Remove(para.NextSibling as Paragraph);
                            descCell.Paragraphs.Remove(para);
                        }
                        else
                        {
                            Double.TryParse((para.NextSibling as Paragraph)?.Text.Trim(), out point);
                            while (para.NextSibling != null)
                                descCell.Paragraphs.Remove(para.NextSibling as Paragraph);
                            descCell.Paragraphs.Remove(para);
                        }
                    }
                }


                descriptionLessonPart = await StringHelper.ConvertDocToHtml(table, basis, createUser, StaticPath);
                descriptionQUIZ = $"<p>{descriptionQUIZ}</p>";

                item.Description = descriptionLessonPart;
                item.Point = point;
                var question = new CloneQuestionViewModel
                {
                    Created = DateTime.UtcNow,
                    CreateUser = createUser,
                    Description = descriptionQUIZ,
                    Point = point,
                    MaxPoint = point
                };
                item.Questions = new List<CloneQuestionViewModel>();
                item.Questions.Add(question);

                //file 
                var linkcell = table.Rows[4].Cells[1];

                var linkfile = "";
                linkfile = await GetContentFile(basis, item, createUser, linkcell, linkfile);

                await CreateOrUpdate(basis, item);
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private async Task SaveQA(CloneLessonPartViewModel item, string UserID)
        {
            foreach (var questionVM in item.Questions)
            {
                questionVM.ParentID = item.ID;
                questionVM.CourseID = item.CourseID;

                var quiz = questionVM.ToEntity();
                if (questionVM.ID == "0" || questionVM.ID == null || _cloneQuestionService.GetItemByID(quiz.ID) == null)
                {
                    var _maxItem = _cloneQuestionService.CreateQuery()
                        .Find(o => o.ParentID == item.ID)
                        .SortByDescending(o => o.Order).FirstOrDefault();
                    quiz.Order = questionVM.Order;
                    quiz.Created = DateTime.UtcNow;
                    quiz.Updated = DateTime.UtcNow;
                    quiz.CreateUser = UserID;
                }
                _cloneQuestionService.CreateQuery().InsertOne(quiz);

                questionVM.ID = quiz.ID;

                if (questionVM.Answers != null && questionVM.Answers.Count > 0)
                {
                    foreach (var answer in questionVM.Answers)
                    {
                        answer.ParentID = questionVM.ID;
                        var _maxItem1 = _cloneAnswerService.CreateQuery().Find(o => o.ParentID == quiz.ID).SortByDescending(o => o.Order).FirstOrDefault();
                        answer.Order = _maxItem1 != null ? _maxItem1.Order + 1 : 0;
                        answer.Created = DateTime.UtcNow;
                        answer.Updated = DateTime.UtcNow;
                        answer.CreateUser = quiz.CreateUser;
                        answer.CourseID = quiz.CourseID;
                        _cloneAnswerService.CreateQuery().InsertOne(answer);
                    }
                }
            }
        }

        private async Task<Dictionary<String, String>> GetPathFileOLE(DocOleObject Ole, String basis, String createUser)
        {
            Dictionary<String, String> dataResponse = new Dictionary<String, String>();
            String[] typeVideo = { ".ogm", ".wmv", ".mpg", ".webm", ".ogv", ".mov", ".asx", ".mpge", ".mp4", ".m4v", ".avi" };
            String[] typeAudio = { ".opus", ".flac", ".weba", ".webm", ".wav", ".ogg", ".m4a", ".oga", ".mid", ".mp3", ".aiff", ".wma", ".au" };
            String path = "";
            String user = String.IsNullOrEmpty(createUser) ? "admin" : createUser;
            //foreach (DocumentObject obj in para.ChildObjects)
            //{
            //    if (obj.DocumentObjectType == DocumentObjectType.OleObject)
            //    {
            String typeFile = Ole.ObjectType.ToUpper();
            String packageFileName = Ole.PackageFileName.ToString().Trim();


            var filename = packageFileName.Replace("\\", "#").Split('#').Last();
            String extension = Path.GetExtension(filename);
            dataResponse.Add("packageFileName", filename);
            dataResponse.Add("extension", extension);
            if (typeFile.Contains("AcroExch.Document.11".ToUpper()))//pdf
            {
                path = await SaveFileToDrive(Ole, user, extension, basis);
                dataResponse.Add("pathFileOLE", path);
            }
            else if (typeFile.Contains("Excel.Sheet.12".ToUpper()) || typeFile.Contains("Excel.Sheet.8".ToUpper()))//excel - check
            {
                path = await SaveFileToDrive(Ole, user, extension, basis);
                dataResponse.Add("pathFileOLE", path);
            }
            //else if(typeFile.Contains("Excel.Sheet.12".ToUpper()) || typeFile.Contains("Excel.Sheet.8".ToUpper()))//doc
            //{

            //}
            else if (typeFile.Contains("PowerPoint.Slide.8".ToUpper()) || typeFile.Contains("PowerPoint.Slide.12".ToUpper()))//power point
            {
                path = await SaveFileToDrive(Ole, user, extension, basis);
                dataResponse.Add("pathFileOLE", path);
            }
            else
            {
                if (typeVideo.Contains(extension))//video
                {
                    path = await SaveFileToDrive(Ole, user, extension, basis);
                    dataResponse.Add("pathFileOLE", path);
                }
                else if (typeAudio.Contains(extension))//audio
                {
                    path = await SaveFileToDrive(Ole, user, extension, basis);
                    dataResponse.Add("pathFileOLE", path);
                }
                else
                {
                    path = await SaveFileToDrive(Ole, user, extension, basis);
                    dataResponse.Add("pathFileOLE", path);
                }
                //    }
                //}
            }
            return dataResponse;
        }

        private async Task<String> SaveFileToDrive(DocOleObject Ole, String user, String extension, String basis)
        {
            Byte[] bytes = Ole.NativeData;
            String path = "";
            using (MemoryStream memory = new MemoryStream(bytes))
            {
                path = _roxyFilemanHandler.GoogleDriveApiService.CreateLinkViewFile(_roxyFilemanHandler.UploadFileWithGoogleDrive(basis, user, memory, extension));
                memory.Close();
            }
            return path;
        }

        private string SaveImageByByteArray(byte[] byteArrayIn, string fileName, String user, string center = "")
        {
            try
            {
                return FileProcess.ConvertImageByByteArray(byteArrayIn, fileName, $"{center}/{user}", RootPath);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private async Task GetVocabByCambridge(string vocab)
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
                        if (expNodes == null)
                            return;
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
            //if (listExp == null || listExp.Count == 0)
            //    return;

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
                        var typeNode = content.SelectSingleNode(".//span[contains(@class,\"pos dpos\")]");
                        var typeText = typeNode.InnerText;

                        if (listVocab.Any(t => t.WordType == typeText))
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
                                WordType = typeText,
                                Pronunciation = pronunText,
                                PronunAudioPath = pronunPath,
                                Created = DateTime.UtcNow,
                                Description = string.Join(", ", listExp.Where(t => t.WordType == typeText).Select(t => t.Meaning))
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
        #endregion

        #region Export to Word
        public IActionResult ExportToWord(String basis, String LessonID)
        {
            var lesson = _lessonService.GetItemByID(LessonID);
            if (lesson == null)
                return Json(new Dictionary<String, object> {
                    {"Stt",false },
                    {"Msg","Không tìm thấy bài học." }
                });
            var lessonPart = _cloneLessonPartService.GetByLessonID(LessonID);
            if (lessonPart.Count() == 0)
                return Json(new Dictionary<String, object> {
                    {"Stt",false },
                    {"Msg","Nội dung bài học chưa có." }
                });
            var lessonPartIDs = lessonPart.Select(x => x.ID);
            var lessonPartQuestion = _cloneQuestionService.CreateQuery().Find(x => lessonPartIDs.Contains(x.ParentID)).ToEnumerable();
            var lessonPartQuestionIDs = lessonPartQuestion.Select(x => x.ID);
            var lessonPartAnswer = _cloneAnswerService.CreateQuery().Find(x => lessonPartQuestionIDs.Contains(x.ParentID)).ToEnumerable();
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            try
            {
                byte[] toArray = null;
                using (var stream = new MemoryStream())
                {
                    Document doc = new Document();
                    Section s = doc.AddSection();
                    s.PageSetup.Orientation = PageOrientation.Landscape;

                    var defStyle = doc.AddParagraphStyle("DefStyle");
                    defStyle.CharacterFormat.FontName = "Calibri";
                    defStyle.CharacterFormat.FontSize = 13;

                    var defStyleCenter = doc.AddParagraphStyle("DefStyleCenter");
                    defStyleCenter.ApplyBaseStyle(defStyle.Name);
                    defStyleCenter.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;

                    for (int x = 0; x < lessonPart.Count(); x++)
                    {

                        var _lessonPart = lessonPart.ElementAtOrDefault(x);

                        RenderWordLessonPart(ref s, _lessonPart);
                    }

                    Paragraph paragraph = s.AddParagraph();
                    TextRange TR4 = paragraph.AppendText(Note);
                    TR4.CharacterFormat.Italic = true;
                    TR4.CharacterFormat.FontSize = 12;
                    TR4.CharacterFormat.TextColor = Color.Red;

                    //Save
                    doc.SaveToStream(stream, FileFormat.Docx);
                    toArray = stream.ToArray();
                };
                string wordName = $"{lesson.Title}.docx";
                return File(toArray, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", wordName);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        private void RenderWordLessonPart(ref Section s, LessonPartEntity _lessonPart)
        {

            Table table = s.AddTable(true);

            table.PreferredWidth = new PreferredWidth(WidthType.Percentage, 100);
            table.ResetCells(1, partTypes.Count + 1);

            var titleRow = table.Rows[0];
            var typeRow = table.AddRow();
            var typeRow2 = table.AddRow();
            var descriptionRow = table.AddRow();
            var attachmentRow = table.AddRow();


            #region Title
            var titleRow_Cel1_Content = titleRow.Cells[0].AddParagraph();
            titleRow_Cel1_Content.ApplyStyle("DefStyleCenter");
            titleRow_Cel1_Content.AppendText("Tiêu đề");

            table.ApplyHorizontalMerge(titleRow.GetRowIndex(), 1, partTypes.Count);
            var titleRow_Cel2_Content = titleRow.Cells[1].AddParagraph();
            titleRow_Cel2_Content.ApplyStyle("DefStyle");
            titleRow_Cel2_Content.AppendText(_lessonPart.Title).CharacterFormat.Bold = true;
            #endregion

            #region Type

            var typeRow_cell1 = typeRow.Cells[0];
            typeRow_cell1.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

            var p1 = typeRow_cell1.AddParagraph();
            p1.ApplyStyle("DefStyleCenter");
            p1.AppendText("Kiểu ND");
            var p2 = typeRow_cell1.AddParagraph();
            p2.ApplyStyle("DefStyleCenter");
            p2.AppendText("(x)");

            for (int i = 0; i < partTypes.Count; i++)
            {
                var cell1 = typeRow.Cells[i + 1];
                cell1.CellFormat.HorizontalMerge = CellMerge.None;//prevent previous merge
                cell1.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                var cell_content = cell1.AddParagraph();
                cell_content.ApplyStyle("DefStyleCenter");
                cell_content.AppendText(partDSP[i]).CharacterFormat.FontSize = 12;

                var cell2 = typeRow2.Cells[i + 1];
                cell2.CellFormat.VerticalMerge = CellMerge.None;
                cell2.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                var prg = cell2.AddParagraph();
                prg.ApplyStyle("DefStyleCenter");
                if (_lessonPart.Type == partTypes[i])
                    prg.AppendText("x").CharacterFormat.Bold = true;
                //else
                //    prg.AppendText("").CharacterFormat.Bold = true;
            }
            table.ApplyVerticalMerge(0, typeRow.GetRowIndex(), typeRow2.GetRowIndex());

            #endregion

            #region Description
            var descriptionRow_Cel1 = descriptionRow.Cells[0];
            descriptionRow_Cel1.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
            var descriptionRow_Cel1_Content = descriptionRow_Cel1.AddParagraph();
            descriptionRow_Cel1_Content.ApplyStyle("DefStyleCenter");
            switch (_lessonPart.Type)
            {
                //case "QUIZ1":
                //case "QUIZ2":
                //case "QUIZ3":
                //case "QUIZ4":
                //case "ESSAY":
                //    descriptionRow_Cel1_Content.AppendText("Nội dung");
                //    break;
                //case "VOCAB":
                //    descriptionRow_Cel1_Content.AppendText("Danh sách từ");
                //    break;
                default:
                    descriptionRow_Cel1_Content.AppendText("Nội dung");
                    break;
            }

            table.ApplyHorizontalMerge(descriptionRow.GetRowIndex(), 1, partTypes.Count);
            var descriptionRow_Cel2 = descriptionRow.Cells[1];
            var descriptionRow_Cel2_Content = descriptionRow_Cel2.AddParagraph();
            descriptionRow_Cel2_Content.ApplyStyle("DefStyle");


            //var newdoc = ConvertHtmlToWordDoc(_lessonPart.Description, basis, userid, _lessonPart.Type);
            var html = _lessonPart.Description;
            if (html != null)
                html = html.Replace("src=\"/", "src=\"https://" + currentHost + "/");
            descriptionRow_Cel2_Content.ApplyStyle("DefStyle");
            var answer = "";
            switch (_lessonPart.Type)
            {
                case "QUIZ2":
                    if (!string.IsNullOrEmpty(_lessonPart.Description))
                    {
                        html = "<p style='margin:0pt'><b style='color:red'>[Câu hỏi]</b></p><p style='margin:0pt'>" + RenderQuiz2ForWord(_lessonPart, out answer) + "</p>";
                        answer = "<p style='margin:0pt;margin-top:2pt'><b style='color:red'>[Đáp án]</b></p>" + answer;
                        descriptionRow_Cel2_Content.AppendHTML(html + answer);
                        //var aGrp2 = descriptionRow_Cel2.AddParagraph();
                        //aGrp2.ApplyStyle("DefStyle");
                        //descriptionRow_Cel2_Content.AppendHTML(answer);
                        //aGrp2.ApplyStyle("DefStyle");
                        //foreach (var childprg in aGrp2.ChildObjects)
                        //{
                        //    if (childprg is Paragraph)
                        //    {
                        //        (childprg as Paragraph).ApplyStyle("DefStyle");
                        //    }
                        //}
                    }
                    break;
                case "QUIZ1":
                case "QUIZ3":
                case "QUIZ4":
                    if (!string.IsNullOrEmpty(_lessonPart.Description))
                        descriptionRow_Cel2_Content.AppendHTML("<p style='margin:0pt'>" + html + "</p>");
                    var question = "<p style='margin:0pt'><b style='color:red'>[Câu hỏi]</b></p><p style='margin:0pt'>" + RenderTQuizForWord(_lessonPart, out answer) + "</p>";
                    answer = "<p style='margin:0pt;margin-top:2pt'><b style='color:red'>[Đáp án]</b></p>" + answer;
                    var qGrp = descriptionRow_Cel2.AddParagraph();
                    qGrp.ApplyStyle("DefStyle");
                    qGrp.AppendHTML(question);
                    var aGrp = descriptionRow_Cel2.AddParagraph();
                    aGrp.ApplyStyle("DefStyle");
                    aGrp.AppendHTML(answer);
                    break;
                case "ESSAY":
                    //html = $"<p>[Des]</p>{html}";
                    var lessonPartQuestion = _cloneQuestionService.GetByPartID(_lessonPart.ID).FirstOrDefault();
                    if (lessonPartQuestion != null)
                    {
                        html += $"<p style='margin:0pt'>[E]</p>{lessonPartQuestion.Description}";
                        html += $"<p style='margin:0pt;margin-top:2pt'>[P] {lessonPartQuestion.Point}</p>";
                    }
                    descriptionRow_Cel2_Content.AppendHTML(html);
                    break;
                default:
                    if (!string.IsNullOrEmpty(_lessonPart.Description))
                        descriptionRow_Cel2_Content.AppendHTML(html);
                    break;
            }
            #endregion

            #region Attachment
            var attachmentRow_Cel1 = attachmentRow.Cells[0];
            attachmentRow_Cel1.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
            var attachmentRow_Cel1_Content = attachmentRow_Cel1.AddParagraph();
            attachmentRow_Cel1_Content.ApplyStyle("DefStyleCenter");
            attachmentRow_Cel1_Content.AppendText("File");

            table.ApplyHorizontalMerge(attachmentRow.GetRowIndex(), 1, partTypes.Count);
            var attachmentRow_Cel2_Content = attachmentRow.Cells[1].AddParagraph();
            attachmentRow_Cel2_Content.ApplyStyle("DefStyle");

            if (_lessonPart.Media != null && !string.IsNullOrEmpty(_lessonPart.Media.Path))
            {

                var ext = _lessonPart.Media.Extension;

                if (_lessonPart.Media.Path.ToLower().StartsWith("http"))//external
                {
                    attachmentRow_Cel2_Content.AppendHyperlink(_lessonPart.Media.Path, _lessonPart.Media.OriginalName, HyperlinkType.WebLink);
                }
                else
                {
                    var path = _lessonPart.Media.Path.Replace("/Files/", "Files\\").Replace("/", "\\");
                    var objPath = Path.Combine(StaticPath, path);

                    var maxWidth = 360; var maxHeight = 240;


                    if (ext.ToLower().Contains("image"))
                    {
                        var img = attachmentRow_Cel2_Content.AppendPicture(FileProcess.ImageToByteArray(Image.FromFile(objPath)));
                        var scale = 100;
                        if (img.Width > 0 && img.Height > 0)
                        {
                            if (img.Width / img.Height > maxWidth / maxHeight)
                            {
                                if (img.Width > maxWidth)
                                    scale = (int)(maxWidth * 100.0 / img.Width);
                            }
                            else
                                if (img.Height > maxHeight)
                                scale = (int)(maxHeight * 100.0 / img.Height);
                        }

                        img.WidthScale = scale;
                        img.HeightScale = scale;
                    }
                    else
                    {
                        Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(objPath);
                        var byteIcon = IconToBytes(icon);
                        Document document = new Document();
                        Section section = document.AddSection();
                        Paragraph p = section.AddParagraph();
                        //DocPicture Pic = p.AppendPicture(byteIcon);
                        DocPicture Pic = p.AppendPicture(FileProcess.ImageToByteArray(Image.FromFile($"{StaticPath}\\images\\default-Icon-File.png.jpg")));
                        Pic.Width = 100;
                        Pic.Height = 50;
                        if (ext.Contains("audio"))
                        {
                            var ole = attachmentRow_Cel2_Content.AppendOleObject(objPath, Pic);
                            ole.Width = Pic.Width;
                            ole.Height = Pic.Height;
                        }
                        else if (ext.Contains("video"))
                        {
                            var ole = attachmentRow_Cel2_Content.AppendOleObject(objPath, Pic, OleObjectType.VideoClip);
                            ole.Width = maxWidth;
                            ole.Height = maxHeight;
                        }
                        else
                        {
                            var ole = attachmentRow_Cel2_Content.AppendOleObject(objPath, Pic);
                            ole.Width = maxWidth;
                            ole.Height = maxHeight;
                        }
                    }
                }
            }

            #endregion

            //break part
            s.AddParagraph();
        }

        private string RenderQuiz2ForWord(LessonPartEntity lessonPart, out string answer)
        {
            answer = "";
            var description = lessonPart.Description.Replace("src=\"/", "src=\"https://" + currentHost + "/");
            var returnHtml = "";
            var quizOpen = "<fillquiz>";
            var quizClose = "</fillquiz>";

            var questions = _cloneQuestionService.GetByPartID(lessonPart.ID).ToList();
            if (questions == null || questions.Count() == 0)
                return description;

            var firstOccurIndex = description.IndexOf(quizOpen);
            var i = 0;
            while (firstOccurIndex >= 0 && questions.Count() > 0)
            {
                i++;
                var quiz = questions.First();
                returnHtml += description.Substring(0, firstOccurIndex) + "_Q" + i + "";
                //if (!string.IsNullOrEmpty(quiz.Content))
                //    returnHtml += (":" + quiz.Content);
                returnHtml += "_";
                var ans = _cloneAnswerService.GetByQuestionID(quiz.ID);

                if (ans != null && ans.Count() > 0)
                {
                    answer += "<p style='margin:0pt'>[A" + i + "] ";
                    answer += string.Join(" | ", ans.Select(t => t.Content));
                    answer += "</p>";
                }
                if (!string.IsNullOrEmpty(quiz.Content))
                {
                    answer += ($"<p style='margin:0pt'>[H{i}]" + quiz.Content + "</p>");
                }
                if (!string.IsNullOrEmpty(quiz.Description))
                {
                    answer += ("<p style='margin:0pt'>[E" + i + "]" + quiz.Description + "</p>");
                }

                description = description.Substring(description.IndexOf(quizClose) + quizClose.Length);
                firstOccurIndex = description.IndexOf(quizOpen);
                questions.RemoveAt(0);
            }

            return returnHtml;
        }

        private string RenderTQuizForWord(LessonPartEntity lessonPart, out string answer)
        {
            answer = "";
            var returnHtml = "";

            var questions = _cloneQuestionService.GetByPartID(lessonPart.ID).ToList();
            if (questions == null || questions.Count() == 0)
                return returnHtml;
            var i = 0;
            foreach (var quiz in questions)
            {
                i++;
                returnHtml += "<p style='margin:0pt'>[Q" + i + "] " + RenderHtmlContent(quiz.Content, quiz.Media) + "</p>";
                var ans = _cloneAnswerService.GetByQuestionID(quiz.ID);

                answer += "<p style='margin:0pt'>";

                if (ans != null && ans.Count() > 0)
                {
                    answer += "[A" + i + "] ";
                    answer += string.Join(" | ", ans.Select(t => RenderHtmlContent(t.Content, t.Media) + (t.IsCorrect ? " (x)" : "")));
                }
                answer += "</p>";
                if (!string.IsNullOrEmpty(quiz.Description))
                    answer += ("<p style='margin:0pt'>[E" + i + "] " + quiz.Description + "</p>");

            }

            returnHtml = returnHtml.Replace("src=\"/", "src=\"https://" + currentHost + "/");
            return returnHtml;
        }

        private static byte[] IconToBytes(Icon icon)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                icon.Save(ms);
                return ms.ToArray();
            }
        }

        private string RenderHtmlContent(string text, Media img)
        {
            string returnHTML = "";
            if (!string.IsNullOrEmpty(text))
                returnHTML += text;
            if (img != null)
            {
                if (img.Path.ToLower().StartsWith("http"))
                    returnHTML += "<img src='" + img.Path + "'/>";
                else
                    returnHTML += "<img src='https://static.eduso.vn/" + img.Path + "'/>";
            }
            return returnHTML;
        }

        private static String Note = $"\nGiải thích kí hiệu" +
            $"\n1. [Câu hỏi] : Bắt đầu câu hỏi" +
            $"\n2. [Đáp án] : Bắt đầu đáp án" +
            $"\n3. [Qxxx] : Nội dung câu hỏi (Đối với các dạng câu hỏi trắc nghiệm)." +
            $"\n4. _Qxxx_ : Nội dung câu hỏi (Đối với dạng điền từ - đánh dấu vị trí điền từ)." +
            $"\n5. [Axxx] : Nội dung câu trả lời; các câu hỏi có nhiều đáp án được viết trên cùng một dòng, cách nhau bởi dấu |" +
            $"\n6. (x) : Đánh dấu câu trả lời đúng (Đối với các dạng trắc nghiệm)." +
            $"\n7. [Exxx] : Giải thích cho đáp án đúng" +
            $"\n8. [Hxxx] : Phần hiển thị với học viên tại vị trí điền từ" +
            $"\n9. [P] : Điểm (Đối với dạng essay)" +
            $"\nLưu ý: xxx là số thứ tự câu hỏi/câu trả lời;" +
            $"\nLiên kết hình ảnh/media có dạng http://... hoặc https://...";
        #endregion
    }
}
