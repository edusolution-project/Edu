using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using MongoDB.Driver;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using HtmlAgilityPack;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Microsoft.CodeAnalysis.CSharp;
using MongoDB.Bson.Serialization.Serializers;
using System.Net;
using System.IO;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class LessonPartController : TeacherController
    {
        //private readonly GradeService _gradeService;
        //private readonly SubjectService _subjectService;
        //private readonly TeacherService _teacherService;
        //private readonly ClassService _classService;
        //private readonly CourseService _courseService;
        //private readonly ChapterService _chapterService;
        //private readonly LessonScheduleService _lessonScheduleService;

        private readonly CourseLessonService _lessonService;
        private readonly LessonPartService _lessonPartService;
        private readonly LessonPartQuestionService _questionService;
        private readonly LessonPartAnswerService _answerService;
        private readonly FileProcess _fileProcess;
        private readonly VocabularyService _vocabularyService;

        public LessonPartController(
            //GradeService gradeservice,
            //SubjectService subjectService,
            //TeacherService teacherService,
            //ClassService classService,
            //CourseService courseService,
            //ChapterService chapterService,
            //LessonScheduleService lessonScheduleService,

            CourseLessonService lessonService,
            LessonPartService lessonPartService,
            LessonPartQuestionService questionService,
            LessonPartAnswerService answerService,
            FileProcess fileProcess,
            VocabularyService vocabularyService
            )
        {
            //_gradeService = gradeservice;
            //_subjectService = subjectService;
            //_teacherService = teacherService;
            //_courseService = courseService;
            //_classService = classService;
            //_chapterService = chapterService;
            //_lessonScheduleService = lessonScheduleService;

            _lessonService = lessonService;
            _lessonPartService = lessonPartService;
            _questionService = questionService;
            _answerService = answerService;
            _fileProcess = fileProcess;
            _vocabularyService = vocabularyService;
        }

        [HttpPost]
        public JsonResult ChangePosition(string ID, int pos)
        {
            var item = _lessonPartService.CreateQuery().Find(o => o.ID == ID).SingleOrDefault();
            if (item == null) return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", null },
                        {"Error", "Data not found" }
                    });

            var parentLesson = _lessonService.CreateQuery().Find(o => o.ID == item.ParentID
            ).SingleOrDefault();

            if (parentLesson != null)
            {
                var parts = _lessonPartService.CreateQuery().Find(o => o.ParentID == parentLesson.ID).SortBy(o => o.Order).ThenBy(o => o.ID).ToList();
                var ids = parts.Select(o => o.ID).ToList();

                var oldPos = ids.IndexOf(ID);
                if (oldPos == pos)
                    return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", null },
                        {"Error", "Nothing change" }
                    });

                item.Order = pos;

                //var startIdx = oldPos;
                //var endIdx = oldPos;
                //if (oldPos > pos)
                //    startIdx = pos;
                //else
                //    endIdx = pos;


                var filter = Builders<LessonPartEntity>.Filter.Where(o => o.ID == item.ID);
                var update = Builders<LessonPartEntity>.Update.Set("Order", pos);
                var publish = _lessonPartService.Collection.UpdateMany(filter, update);
                int entry = -1;
                foreach (var part in parts)
                {
                    if (part.ID == item.ID) continue;
                    if (entry == pos - 1)
                        entry++;
                    entry++;
                    part.Order = entry;
                    var filterX = Builders<LessonPartEntity>.Filter.Where(o => o.ID == part.ID);
                    var updateX = Builders<LessonPartEntity>.Update.Set("Order", part.Order);
                    var publishX = _lessonPartService.Collection.UpdateMany(filterX, updateX);
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

        [HttpPost]
        public JsonResult GetDetail(string ID)
        {
            var part = _lessonPartService.CreateQuery().Find(o => o.ID == ID).SingleOrDefault();
            if (part == null) return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", null },
                        {"Error", "Data not found" }
                    });
            var full_item = new LessonPartViewModel(part)
            {
                Questions = _questionService.CreateQuery().Find(o => o.ParentID == part.ID).SortBy(o => o.Order).ThenBy(o => o.ID).ToList().Select(t =>
                      new QuestionViewModel(t)
                      {
                          Answers = _answerService.CreateQuery().Find(a => a.ParentID == t.ID).SortBy(o => o.Order).ThenBy(o => o.ID).ToList()
                      }).ToList()
            };
            return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", full_item },
                        {"Error", null }
                    });

        }

        [HttpPost]
        public async Task<JsonResult> CreateOrUpdate(string basis, LessonPartViewModel item, List<string> RemovedQuestions = null, List<string> RemovedAnswers = null)
        {
            //try
            //{
            var parentLesson = _lessonService.CreateQuery().Find(o => o.ID == item.ParentID).SingleOrDefault();
            var createduser = User.Claims.GetClaimByType("UserID").Value;
            if (parentLesson != null)
            {
                if (item.Media != null && item.Media.Name == null) item.Media = null;//valid Media
                var files = HttpContext.Request.Form != null && HttpContext.Request.Form.Files.Count > 0 ? HttpContext.Request.Form.Files : null;
                if (item.ID == "0" || item.ID == null) //create
                {
                    item.CourseID = parentLesson.CourseID;
                    item.Created = DateTime.Now;
                    var maxItem = _lessonPartService.CreateQuery()
                        .Find(o => o.ParentID == item.ParentID)
                        .SortByDescending(o => o.Order).FirstOrDefault();
                    item.Order = maxItem != null ? maxItem.Order + 1 : 0;
                    item.Updated = DateTime.Now;

                    if (item.Media == null || string.IsNullOrEmpty(item.Media.Name) || (!item.Media.Name.ToLower().StartsWith("http") && (files == null || !files.Any(f => f.Name == item.Media.Name))))
                    {
                        item.Media = null;
                    }
                    else
                    {
                        if (item.Media.Name.ToLower().StartsWith("http")) //file url (import)
                        {
                            item.Media.Created = DateTime.Now;
                            item.Media.Size = 0;
                            item.Media.Path = item.Media.Name.Trim();
                        }
                        else
                        {
                            var file = files.Where(f => f.Name == item.Media.Name).SingleOrDefault();
                            if (file != null)
                            {
                                item.Media.Created = DateTime.Now;
                                item.Media.Size = file.Length;
                                item.Media.Path = await _fileProcess.SaveMediaAsync(file, item.Media.OriginalName, "", basis);
                            }
                        }
                    }
                }
                else // Update
                {
                    var olditem = _lessonPartService.GetItemByID(item.ID);
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
                        item.Media.Path = await _fileProcess.SaveMediaAsync(file, item.Media.OriginalName, "", basis);
                    }

                    item.Updated = DateTime.Now;
                    item.Created = olditem.Created;
                    item.Order = olditem.Order;
                    item.CourseID = parentLesson.CourseID;
                }

                var lessonpart = item.ToEntity();
                if (lessonpart.ID != null)
                {
                    _lessonPartService.CreateQuery().ReplaceOne(t => t.ID == lessonpart.ID, lessonpart);
                }
                else
                {
                    _lessonPartService.CreateQuery().InsertOne(lessonpart);
                }
                item.ID = lessonpart.ID;

                switch (lessonpart.Type)
                {
                    case "ESSAY":

                        _questionService.CreateQuery().DeleteMany(t => t.ParentID == lessonpart.ID);
                        var question = new LessonPartQuestionEntity
                        {
                            CourseID = lessonpart.CourseID,
                            Content = "",
                            Description = "",
                            ParentID = lessonpart.ID,
                            CreateUser = createduser,
                            Point = lessonpart.Point,
                            Created = lessonpart.Created,
                        };
                        _questionService.Save(question);
                        break;
                    case "VOCAB":
                        if (lessonpart.Description != null && lessonpart.Description.Length > 0)
                        {
                            var vocabArr = lessonpart.Description.Split('|');
                            if (vocabArr != null && vocabArr.Length > 0)
                            {
                                foreach (var vocab in vocabArr)
                                {
                                    var vocabulary = vocab.Trim().ToLower();
                                    _ = GetVocabByCambridge(vocabulary);
                                    _ = GetVocabByTraTu(vocabulary);
                                }
                            }
                        }
                        break;
                    case "QUIZ2": //remove all previous question
                        var oldQuizIds = _questionService.CreateQuery().Find(q => q.ParentID == lessonpart.ID).Project(i => i.ID).ToEnumerable();
                        foreach (var quizid in oldQuizIds)
                            _answerService.CreateQuery().DeleteMany(a => a.ParentID == quizid);
                        _questionService.CreateQuery().DeleteMany(q => q.ParentID == lessonpart.ID);

                        if (!String.IsNullOrEmpty(item.Description) && item.Description.ToLower().IndexOf("<fillquiz ") >= 0)
                        {
                            var newdescription = "";
                            if (item.Questions == null || item.Questions.Count == 0)
                                item.Questions = ExtractFillQuestionList(item, createduser, out newdescription);
                            lessonpart.Description = newdescription;
                            _lessonPartService.CreateQuery().ReplaceOne(t => t.ID == lessonpart.ID, lessonpart);
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

                        break;
                    default:
                        if (RemovedQuestions != null & RemovedQuestions.Count > 0)
                        {
                            _questionService.CreateQuery().DeleteMany(o => RemovedQuestions.Contains(o.ID));

                            foreach (var quizID in RemovedQuestions)
                            {
                                _answerService.CreateQuery().DeleteMany(o => o.ParentID == quizID);
                            }
                        }

                        if (RemovedAnswers != null & RemovedAnswers.Count > 0)
                            _answerService.CreateQuery().DeleteMany(o => RemovedAnswers.Contains(o.ID));
                        item.CourseID = parentLesson.CourseID;

                        if (item.Questions != null && item.Questions.Count > 0)
                        {
                            await SaveQuestionFromView(item, createduser, files);
                        }
                        break;
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
            else
            {
                return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", null },
                                {"Error", "Parent Item Not found" }
                            });
            }
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetList(string LessonID)
        {
            var root = _lessonService.CreateQuery().Find(o => o.ID == LessonID).SingleOrDefault();
            var data = new Dictionary<string, object> { };

            if (root != null)
            {
                var listLessonPart = _lessonPartService.CreateQuery().Find(o => o.ParentID == LessonID).SortBy(q => q.Order).ThenBy(q => q.ID).ToList();
                if (listLessonPart != null && listLessonPart.Count > 0)
                {
                    var result = new List<LessonPartViewModel>();
                    foreach (var part in listLessonPart)
                    {
                        switch (part.Type)
                        {
                            case "QUIZ1":
                            case "QUIZ2":
                            case "QUIZ3":
                            case "ESSAY":
                                result.Add(new LessonPartViewModel(part)
                                {
                                    Questions = _questionService.CreateQuery().Find(q => q.ParentID == part.ID).SortBy(q => q.Order).ThenBy(q => q.ID).ToList().Select(q => new QuestionViewModel(q)
                                    {
                                        Answers = _answerService.CreateQuery().Find(a => a.ParentID == q.ID).SortBy(a => a.Order).ThenBy(a => a.ID).ToList()
                                    }).ToList()
                                });
                                break;
                            case "VOCAB":
                                result.Add(new LessonPartViewModel(part)
                                {
                                    Description = RenderVocab(part.Description)
                                });
                                break;
                            default:
                                result.Add(new LessonPartViewModel(part));
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

        private string RenderVocab(string description)
        {
            string result = "";
            var vocabs = description.Split('|');
            if (vocabs == null || vocabs.Count() == 0)
                return description;
            foreach (var vocab in vocabs)
            {
                var vocabularies = _vocabularyService.GetItemByCode(vocab.Trim().Replace(" ", "-"));
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

        [HttpPost]
        public async Task<JsonResult> Remove(string ID)
        {
            try
            {

                var item = _lessonPartService.CreateQuery().Find(o => o.ID == ID).SingleOrDefault();
                if (item == null) return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", null },
                                {"Error", "Item Not Found" }
                            });

                var parentLesson = _lessonService.CreateQuery().Find(o => o.ID == item.ParentID
                //&& o.CreateUser == UserID TODO:remove later
                ).SingleOrDefault(); //Chỉ remove được các part thuộc lesson do mình up

                if (parentLesson != null)
                {
                    //var media = _LessonExtendService.CreateQuery().Find(o => o.LessonPartID == item.ID).ToList();
                    //if (media != null && media.Count > 0)
                    //{
                    //    _fileProcess.DeleteFiles(media.Select(o => o.OriginalFile).ToList());
                    //    await _LessonExtendService.RemoveRangeAsync(media.Select(o => o.ID));
                    //}
                    await RemoveLessonPart(ID);

                    return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", "Remove OK" },
                                {"Error", null }
                            });
                }
                else
                    return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", null },
                                {"Error", "Orphan item" }
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
                var item = _lessonPartService.CreateQuery().Find(o => o.ID == ID).SingleOrDefault();
                if (item == null) return;

                var questions = _questionService.CreateQuery().Find(o => o.ParentID == ID).ToList();
                for (int i = 0; questions != null && i < questions.Count; i++)
                    await RemoveQuestion(questions[i].ID);
                await _lessonPartService.RemoveAsync(ID);
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
                //var item = _questionService.CreateQuery().Find(o => o.ID == ID).SingleOrDefault();
                //if (item == null) return;
                await _answerService.CreateQuery().DeleteManyAsync(o => o.ParentID == ID);
                await _questionService.RemoveAsync(ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region GetVocabBy https://dictionary.cambridge.org/
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
                                Created = DateTime.Now,
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

        #region GetVocabBy http://tratu.coviet.vn/
        private async Task GetVocabByTraTu(string vocab)
        {
            //check if vocab is exist
            var code = vocab.ToLower().Replace(" ", "-");
            var olditems = _vocabularyService.GetItemByCode(code);
            if (olditems != null && olditems.Count > 0)
                return;

            var listVocab = new List<VocabularyEntity>();
            var dictUrl = "http://tratu.coviet.vn/hoc-tieng-anh/tu-dien/lac-viet/A-V/" + code + ".html";
            var listExp = new List<PronunExplain>();

            using (var expclient = new WebClient())
            {
                var expDoc = new HtmlDocument();
                string expHtml = expclient.DownloadString(dictUrl);
                expDoc.LoadHtml(expHtml);
                var expContents = expDoc.DocumentNode.SelectNodes("//div[@class=\"p10\"][1]");
                if (expContents != null && expContents.Count() > 0)
                {
                    foreach (var expContent in expContents)
                    {
                        int index = 0;
                        var typeNodes = expContent.SelectNodes(".//div[contains(@id,\"partofspeech\")]/div[@class=\"ub\"]");
                        if (typeNodes == null) continue;
                        foreach (var typeNode in typeNodes)
                        {
                            string[] type = typeNode.InnerText.Split(new char[] { ',' });
                            if (listExp.Any(t => t.WordType == type[0]))
                                continue;
                            var expNodes = expContent.SelectNodes("//div[contains(@id,\"partofspeech_" + index + "\")]/div[@class=\"m\"]");
                            if (expNodes == null)
                                continue;
                            if (expNodes != null && expNodes.Count() > 0)
                            {
                                foreach (var node in expNodes)
                                {
                                    listExp.Add(new PronunExplain
                                    {
                                        WordType = type[0],
                                        Meaning = node.InnerText
                                    });
                                }
                            }
                            index++;
                        }
                    }
                }
            }
            if (listExp == null || listExp.Count == 0)
                return;

            using (var client = new WebClient())
            {
                HtmlDocument doc = new HtmlDocument();
                string html = client.DownloadString(dictUrl);
                doc.LoadHtml(html);

                var contentHolder = doc.DocumentNode.SelectNodes("//div[contains(@id,\"mtd_0\")]");
                if (contentHolder != null && contentHolder.Count() > 0)
                {
                    foreach (var content in contentHolder)
                    {
                        var typeNodes = content.SelectNodes(".//div[contains(@class,\"ub\")]/span");
                        if (typeNodes == null)
                            continue;
                        foreach (var typeNode in typeNodes)
                        {
                            string[] typeTxt = typeNode.InnerText.Split(new char[] { ',' });

                            if (listVocab.Any(t => t.WordType == typeTxt[0]))
                                continue;
                            try
                            {
                                var pronunText = content.SelectSingleNode(".//div[contains(@class,\"p5l fl cB\")]").InnerText;
                                var pronunPath = "";

                                pronunPath = "http://tratu.coviet.vn/sounds/en/" + code.Substring(0, 1) + "/" + code + ".mp3";
                                var vocabulary = new VocabularyEntity
                                {
                                    Name = vocab,
                                    Code = code,
                                    Language = "en-us",
                                    WordType = typeTxt[0],
                                    Pronunciation = pronunText,
                                    PronunAudioPath = pronunPath,
                                    Created = DateTime.Now,
                                    Description = string.Join(", ", listExp.Where(t => t.WordType == typeTxt[0]).Select(t => t.Meaning))
                                };
                                listVocab.Add(vocabulary);
                            }

                            catch (Exception e)
                            {

                            }
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

        private List<QuestionViewModel> ExtractFillQuestionList(LessonPartEntity item, string creator, out string Description)
        {
            Description = item.Description;
            var questionList = new List<QuestionViewModel>();
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
                var Question = new QuestionViewModel
                {
                    ParentID = item.ID,
                    CourseID = item.CourseID,
                    CreateUser = creator,
                    Order = i,
                    Point = 1,
                    Content = inputNode.GetAttributeValue("dsp", null),//phần hiển thị cho học viên
                    Description = quiz.GetAttributeValue("title", null),//phần giải thích đáp án
                    Answers = new List<LessonPartAnswerEntity>
                    {

                    }

                };

                var ansArr = ans.Split('|');
                foreach (var answer in ansArr)
                {
                    if (!string.IsNullOrEmpty(answer.Trim()))
                    {
                        Question.Answers.Add(new LessonPartAnswerEntity
                        {
                            CourseID = item.CourseID,
                            CreateUser = creator,
                            IsCorrect = true,
                            Content = answer.Trim()
                        });
                    }
                }

                questionList.Add(Question);
                var clearnode = HtmlNode.CreateNode("<input></input>");
                clearnode.AddClass("fillquiz");
                inputNode.Remove();
                quiz.Attributes.Remove("contenteditable");
                quiz.Attributes.Remove("readonly");
                quiz.Attributes.Remove("title");
                quiz.ChildNodes.Add(clearnode);
            }

            var removeNodes = doc.DocumentNode.SelectNodes(".//fillquiz[not(input)]");
            if (removeNodes != null && removeNodes.Count() > 0)
                foreach (var node in removeNodes)
                    node.Remove();

            Description = doc.DocumentNode.OuterHtml.ToString();
            return questionList;
        }

        private async Task SaveQuestionFromView(LessonPartViewModel item, string createuser = "auto", IFormFileCollection files = null, string basis = "")
        {
            foreach (var questionVM in item.Questions)
            {
                questionVM.ParentID = item.ID;
                questionVM.CourseID = item.CourseID;

                var quiz = questionVM.ToEntity();

                //if (questionVM.Media != null && questionVM.Media.Name == null) questionVM.Media = null;

                if (questionVM.ID == "0" || questionVM.ID == null || _questionService.GetItemByID(quiz.ID) == null)
                {
                    var maxItem = _questionService.CreateQuery()
                        .Find(o => o.ParentID == item.ID)
                        .SortByDescending(o => o.Order).FirstOrDefault();
                    quiz.Order = questionVM.Order;
                    quiz.Created = DateTime.Now;
                    quiz.Updated = DateTime.Now;
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
                                quiz.Media.Created = DateTime.Now;
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
                                        quiz.Media.Created = DateTime.Now;
                                        quiz.Media.Size = file.Length;
                                        quiz.Media.Path = await _fileProcess.SaveMediaAsync(file, quiz.Media.OriginalName, "", basis);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    var oldquiz = _questionService.GetItemByID(quiz.ID);
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
                            quiz.Media.Path = await _fileProcess.SaveMediaAsync(file, quiz.Media.OriginalName, "", basis);
                        }
                    }

                    quiz.Order = questionVM.Order;
                    quiz.Created = oldquiz.Created;
                    quiz.Updated = DateTime.Now;
                }

                //quiz:
                if (quiz.ID != null)
                {
                    _questionService.CreateQuery().ReplaceOne(t => t.ID == quiz.ID, quiz);
                }
                else
                {
                    _questionService.CreateQuery().InsertOne(quiz);
                }
                questionVM.ID = quiz.ID;

                if (questionVM.Answers != null && questionVM.Answers.Count > 0)
                {
                    foreach (var answer in questionVM.Answers)
                    {
                        answer.ParentID = questionVM.ID;
                        //if (answer.Media != null && answer.Media.Name == null) answer.Media = null;

                        if (answer.ID == "0" || answer.ID == null || _answerService.GetItemByID(answer.ID) == null)
                        {
                            var maxItem = _answerService.CreateQuery().Find(o => o.ParentID == quiz.ID).SortByDescending(o => o.Order).FirstOrDefault();
                            answer.Order = maxItem != null ? maxItem.Order + 1 : 0;
                            answer.Created = DateTime.Now;
                            answer.Updated = DateTime.Now;
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
                                        answer.Media.Created = DateTime.Now;
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
                                                answer.Media.Created = DateTime.Now;
                                                answer.Media.Size = file.Length;
                                                answer.Media.Path = await _fileProcess.SaveMediaAsync(file, answer.Media.OriginalName, "", basis);
                                            }
                                        }

                                    }
                                }
                            }
                        }
                        else
                        {
                            var oldanswer = _answerService.GetItemByID(answer.ID);
                            if ((oldanswer.Media != null && answer.Media != null && oldanswer.Media.Path != answer.Media.Path)
                        || (oldanswer.Media == null && answer.Media != null))//Media change
                            {
                                //TODO: Fix clone on copy question
                                //if (oldanswer.Media != null && !string.IsNullOrEmpty(oldanswer.Media.Path))//Delete old file
                                //    _fileProcess.DeleteFile(oldanswer.Media.Path);

                                if (files == null || !files.Any(f => f.Name == answer.Media.Name))
                                    answer.Media = null;
                                else
                                {
                                    var file = files.Where(f => f.Name == answer.Media.Name).SingleOrDefault();//update media
                                    answer.Media.Created = DateTime.Now;
                                    answer.Media.Size = file.Length;
                                    answer.Media.Path = await _fileProcess.SaveMediaAsync(file, answer.Media.OriginalName, "", basis);
                                }
                            }
                            //else // No Media
                            //{
                            //    quiz.Media = null;
                            //}
                            answer.Order = oldanswer.Order;
                            answer.Created = oldanswer.Created;
                            answer.Updated = DateTime.Now;
                        }
                        //_answerService.CreateOrUpdate(answer);
                        if (answer.ID != null)
                        {
                            _answerService.CreateQuery().ReplaceOne(t => t.ID == answer.ID, answer);
                        }
                        else
                        {
                            _answerService.CreateQuery().InsertOne(answer);
                        }
                    }
                }
            }
        }
    }

    public class PronunExplain
    {
        public string WordType { get; set; }
        public string Meaning { get; set; }
        //public string EG { get; set; }
    }
}
