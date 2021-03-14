using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Core_v2.Globals;
using FileManagerCore.Interfaces;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class CloneLessonPartExamController : TeacherController
    {

        private readonly LessonExamService _lessonExamService;
        private readonly LessonPartExtensionService _lessonPartExtensionService;
        private readonly LessonPartQuestionExtensionServie _lessonPartQuestionExtensionServie;
        private readonly LessonPartAnswerExtensionService _lessonPartAnswerExtensionService;
        private readonly FileProcess _fileProcess;
        private readonly ExamQuestionArchiveService _examQuestionArchive;

        private readonly IHostingEnvironment _env;
        private readonly IRoxyFilemanHandler _roxyFilemanHandler;

        private readonly string[] typeVideo = { ".ogm", ".wmv", ".mpg", ".webm", ".ogv", ".mov", ".asx", ".mpge", ".mp4", ".m4v", ".avi" };
        private readonly string[] typeAudio = { ".opus", ".flac", ".weba", ".webm", ".wav", ".ogg", ".m4a", ".oga", ".mid", ".mp3", ".aiff", ".wma", ".au" };
        private readonly string[] typeImage = { ".jfif", ".pjpeg", ".jpeg", ".pjp", ".jpg", ".png", ".gif", ".bmp", ".dip" };
        private string RootPath { get; }
        private string StaticPath { get; }
        private string currentHost { get; }
        public CloneLessonPartExamController(
            LessonExamService lessonExamService
            , LessonPartExtensionService lessonPartExtensionService
            , LessonPartQuestionExtensionServie lessonPartQuestionExtensionServie
            , LessonPartAnswerExtensionService lessonPartAnswerExtensionService
            , FileProcess fileProcess
            , IConfiguration config
            , IHostingEnvironment env
            , IRoxyFilemanHandler roxyFilemanHandler
            , ExamQuestionArchiveService examQuestionArchive
            )
        {
            _lessonExamService = lessonExamService;
            _lessonPartExtensionService = lessonPartExtensionService;
            _lessonPartQuestionExtensionServie = lessonPartQuestionExtensionServie;
            _lessonPartAnswerExtensionService = lessonPartAnswerExtensionService;
            _fileProcess = fileProcess;
            _roxyFilemanHandler = roxyFilemanHandler;
            _env = env;
            _examQuestionArchive = examQuestionArchive;
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<JsonResult> CreateOrUpdate(
            string basis
            , LessonPartViewModel item
            , String BankQuizID
            , List<String> TagsIDs
            , Int32 TypePart
            , Int32 LevelPart
            , List<string> RemovedQuestions = null
            , List<string> RemovedAnswers = null)
        {
            try
            {
                var createduser = User.Claims.GetClaimByType("UserID").Value;
                var storageQuiz = _examQuestionArchive.GetItemByID(BankQuizID);

                if (item.Media != null && item.Media.Name == null) item.Media = null;//valid Media
                var files = HttpContext.Request.Form != null && HttpContext.Request.Form.Files.Count > 0 ? HttpContext.Request.Form.Files : null;
                if (item.ID == "0" || item.ID == null) //create
                {
                    item.Created = DateTime.UtcNow;
                    //item.TeacherID = createduser;
                    var maxItem = _lessonPartExtensionService.CreateQuery()
                        .Find(o => o.ParentID == item.ParentID)
                        .SortByDescending(o => o.Order).FirstOrDefault();
                    item.Order = maxItem != null ? maxItem.Order + 1 : 0;
                    if (item.Media == null || string.IsNullOrEmpty(item.Media.Name) || files == null || !files.Any(f => f.Name == item.Media.Name))
                    {
                        item.Media = null;
                    }
                    else
                    {
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
                    var olditem = _lessonPartExtensionService.GetItemByID(item.ID);
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
                //item.ClassID = currentCs.ClassID;
                //item.ClassSubjectID = currentCs.ID;
                //item.CourseID = currentCs.CourseID;

                var test = item.ToEntity();
                LessonPartExtensionEntity lessonpart = new LessonPartExtensionEntity(test)
                {
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow,
                    ExamQuestionArchiveID = BankQuizID,
                    Tags = TagsIDs.Count() == 0 ? new List<string>() : TagsIDs,
                    TypePart = TypePart,
                    LevelPart = LevelPart
                };
                _lessonPartExtensionService.Save(lessonpart);

                item.ID = lessonpart.ID;

                switch (lessonpart.Type)
                {
                    case "ESSAY":
                        _lessonPartQuestionExtensionServie.CreateQuery().DeleteMany(t => t.ParentID == lessonpart.ID);
                        var question = new LessonPartQuestionExtensionEntity
                        {
                            CourseID = lessonpart.CourseID,
                            Content = "",
                            Description = item.Questions == null ? "" : item.Questions[0].Description,
                            ParentID = lessonpart.ID,
                            CreateUser = createduser,
                            Point = lessonpart.Point,
                            Created = lessonpart.Created,
                        };
                        _lessonPartQuestionExtensionServie.Save(question);
                        //isPractice = true;
                        break;
                    case "QUIZ2": //remove all previous question
                        var oldQuizIds = _lessonPartQuestionExtensionServie.CreateQuery().Find(q => q.ParentID == lessonpart.ID).Project(i => i.ID).ToEnumerable();
                        foreach (var quizid in oldQuizIds)
                            _lessonPartAnswerExtensionService.CreateQuery().DeleteMany(a => a.ParentID == quizid);
                        _lessonPartQuestionExtensionServie.CreateQuery().DeleteMany(q => q.ParentID == lessonpart.ID);

                        if (!String.IsNullOrEmpty(item.Description) && item.Description.ToLower().IndexOf("<fillquiz ") >= 0)
                        {
                            var newdescription = "";
                            if (item.Questions == null || item.Questions.Count == 0)
                                item.Questions = ExtractFillQuestionList(item, createduser, out newdescription);
                            lessonpart.Description = newdescription;
                            _lessonPartExtensionService.CreateQuery().ReplaceOne(t => t.ID == lessonpart.ID, lessonpart);
                        }
                        else
                        {
                            //No Question
                        }

                        //item.CourseID = parentLesson.CourseID;

                        if (item.Questions != null && item.Questions.Count > 0)
                        {
                            await SaveQuestionFromView(item, createduser, files, basis);
                        }
                        //isPractice = true;
                        break;
                    case "QUIZ1":
                    case "QUIZ3":
                    case "QUIZ4":
                        if (RemovedQuestions != null && RemovedQuestions.Count > 0)
                        {
                            _lessonPartQuestionExtensionServie.CreateQuery().DeleteMany(o => RemovedQuestions.Contains(o.ID));

                            foreach (var quizID in RemovedQuestions)
                            {
                                _lessonPartAnswerExtensionService.CreateQuery().DeleteMany(o => o.ParentID == quizID);
                            }
                        }

                        if (RemovedAnswers != null && RemovedAnswers.Count > 0)
                            _lessonPartAnswerExtensionService.CreateQuery().DeleteMany(o => RemovedAnswers.Contains(o.ID));
                        //item.CourseID = parentLesson.CourseID;

                        if (item.Questions != null && item.Questions.Count > 0)
                        {
                            await SaveQuestionFromView(item, createduser, files);
                        }
                        //isPractice = true;
                        break;
                    default:
                        return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", item },
                                {"Error", null }
                            });
                }

                //if (parentLesson.TemplateType == LESSON_TEMPLATE.LECTURE && parentLesson.IsPractice != isPractice)
                //{
                //    parentLesson.IsPractice = isPractice;
                //    _lessonService.Save(parentLesson);

                //    //updateLessonPractice 
                //    _ = _classHelper.ChangeLessonPracticeState(parentLesson);
                //}


                //_lessonHelper.calculateCloneLessonPoint(item.ParentID);

                return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", item },
                                {"Error", null }
                            });
            }
            catch (Exception e)
            {
                return Json(e.Message);
            }
        }
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

                //move text behind fillQuiz
                var textNode = quiz.SelectSingleNode(".//text()");
                if (textNode != null)
                {
                    var cloneNode = textNode.Clone();
                    textNode.Remove();
                    quiz.ParentNode.InsertAfter(cloneNode, quiz);
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
                    var validAns = validateFill(answer);
                    if (!string.IsNullOrEmpty(validAns))
                    {
                        Question.Answers.Add(new LessonPartAnswerEntity
                        {
                            CourseID = item.CourseID,
                            CreateUser = creator,
                            IsCorrect = true,
                            Content = validAns
                        });
                    }
                }

                questionList.Add(Question);
                //var clearnode = HtmlNode.CreateNode("<input></input>");
                //clearnode.AddClass("fillquiz");
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
        private string validateFill(string org)
        {
            if (string.IsNullOrEmpty(org)) return org;
            org = org.Trim();
            while (org.IndexOf("  ") >= 0)
                org = org.Replace("  ", "");
            return StringHelper.ReplaceSpecialCharacters(org);
        }

        private async Task SaveQuestionFromView(LessonPartViewModel item, string createuser = "auto", IFormFileCollection files = null, string basis = "", string UserID = "")
        {
            foreach (var questionVM in item.Questions)
            {
                questionVM.ParentID = item.ID;
                questionVM.CourseID = item.CourseID;

                var quiz = questionVM.ToEntity();

                //if (questionVM.Media != null && questionVM.Media.Name == null) questionVM.Media = null;

                if (questionVM.ID == "0" || questionVM.ID == null || _lessonPartQuestionExtensionServie.GetItemByID(quiz.ID) == null)
                {
                    var maxItem = _lessonPartQuestionExtensionServie.CreateQuery()
                        .Find(o => o.ParentID == item.ID)
                        .SortByDescending(o => o.Order).FirstOrDefault();
                    quiz.Order = questionVM.Order;
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
                                quiz.Media.Extension = "image/png";
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
                                    var file = files.Where(f => f.Name == quiz.Media.Name).FirstOrDefault();
                                    string extension = Path.GetExtension(file.FileName);
                                    if (file != null)
                                    {
                                        if (typeImage.Contains(extension))
                                        {

                                            quiz.Media.Created = DateTime.UtcNow;
                                            quiz.Media.Size = file.Length;
                                            quiz.Media.Path = await _fileProcess.SaveMediaAsync(file, quiz.Media.OriginalName, "", basis);
                                            quiz.Media.Extension = "image/png";
                                        }
                                        else
                                        {
                                            var mediarsp = _roxyFilemanHandler.UploadSingleFileWithGoogleDrive(basis, UserID, file);
                                            quiz.Media = new Media();
                                            quiz.Media.Name = item.Media.OriginalName = file.FileName;
                                            quiz.Media.Created = DateTime.UtcNow;
                                            quiz.Media.Size = file.Length;
                                            if (typeVideo.Contains(extension))
                                            {
                                                quiz.Media.Extension = "video/mp4";
                                            }
                                            else if (typeAudio.Contains(extension))
                                            {
                                                quiz.Media.Extension = "audio/mp3";
                                            }
                                            else
                                            {
                                                quiz.Media.Extension = extension;
                                            }
                                            //item.Media.Extension = extension.Equals(".mp4")?"video/mp4":extension;
                                            quiz.Media.Path = mediarsp.Path;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    var oldquiz = _lessonPartQuestionExtensionServie.GetItemByID(quiz.ID);
                    if ((oldquiz.Media != null && quiz.Media != null && oldquiz.Media.Path != quiz.Media.Path)
                        || (oldquiz.Media == null && quiz.Media != null))//Media change
                    {

                        //if (oldquiz.Media != null && !string.IsNullOrEmpty(oldquiz.Media.Path))//Delete old file
                        //    _fileProcess.DeleteFile(oldquiz.Media.Path);

                        if (files == null || !files.Any(f => f.Name == quiz.Media.Name))
                            quiz.Media = null;
                        else
                        {
                            var file = files.Where(f => f.Name == quiz.Media.Name).FirstOrDefault();//update media
                            string extension = Path.GetExtension(file.FileName);
                            if (file != null)
                            {
                                if (typeImage.Contains(extension))
                                {
                                    quiz.Media.Created = DateTime.UtcNow;
                                    quiz.Media.Size = file.Length;
                                    quiz.Media.Path = await _fileProcess.SaveMediaAsync(file, quiz.Media.OriginalName, "", basis);
                                    quiz.Media.Extension = "image/png";
                                }
                                else//foreach (var file in files)
                                {

                                    var mediarsp = _roxyFilemanHandler.UploadSingleFileWithGoogleDrive(basis, UserID, file);
                                    quiz.Media = new Media();
                                    quiz.Media.Name = item.Media.OriginalName = file.FileName;
                                    quiz.Media.Created = DateTime.UtcNow;
                                    quiz.Media.Size = file.Length;
                                    if (typeVideo.Contains(extension))
                                    {
                                        quiz.Media.Extension = "video/mp4";
                                    }
                                    else if (typeAudio.Contains(extension))
                                    {
                                        quiz.Media.Extension = "audio/mp3";
                                    }
                                    else
                                    {
                                        quiz.Media.Extension = extension;
                                    }
                                    //item.Media.Extension = extension.Equals(".mp4")?"video/mp4":extension;
                                    quiz.Media.Path = mediarsp.Path;
                                }
                            }
                        }
                    }

                    quiz.Order = questionVM.Order;
                    quiz.Created = oldquiz.Created;
                    quiz.Updated = DateTime.UtcNow;
                }

                //quiz:
                if (quiz.ID != null)
                {
                    _lessonPartQuestionExtensionServie.CreateQuery().ReplaceOne(t => t.ID == quiz.ID, quiz as LessonPartQuestionExtensionEntity);
                }
                else
                {
                    _lessonPartQuestionExtensionServie.CreateQuery().InsertOne(quiz as LessonPartQuestionExtensionEntity);
                }
                questionVM.ID = quiz.ID;

                if (questionVM.Answers != null && questionVM.Answers.Count > 0)
                {
                    foreach (var answer in questionVM.Answers)
                    {
                        answer.ParentID = questionVM.ID;
                        //if (answer.Media != null && answer.Media.Name == null) answer.Media = null;

                        if (answer.ID == "0" || answer.ID == null || _lessonPartAnswerExtensionService.GetItemByID(answer.ID) == null)
                        {
                            var maxItem = _lessonPartAnswerExtensionService.CreateQuery().Find(o => o.ParentID == quiz.ID).SortByDescending(o => o.Order).FirstOrDefault();
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
                                            var file = files.Where(f => f.Name == answer.Media.Name).FirstOrDefault();
                                            string extension = Path.GetExtension(file.FileName);
                                            if (file != null)
                                            {
                                                if (typeImage.Contains(extension))
                                                {
                                                    answer.Media.Created = DateTime.UtcNow;
                                                    answer.Media.Size = file.Length;
                                                    answer.Media.Path = await _fileProcess.SaveMediaAsync(file, answer.Media.OriginalName, "", basis);
                                                    answer.Media.Extension = "image/png";
                                                }
                                                else
                                                {
                                                    var mediarsp = _roxyFilemanHandler.UploadSingleFileWithGoogleDrive(basis, UserID, file);
                                                    answer.Media = new Media();
                                                    answer.Media.Name = item.Media.OriginalName = file.FileName;
                                                    answer.Media.Created = DateTime.UtcNow;
                                                    answer.Media.Size = file.Length;
                                                    if (typeVideo.Contains(extension))
                                                    {
                                                        answer.Media.Extension = "video/mp4";
                                                    }
                                                    else if (typeAudio.Contains(extension))
                                                    {
                                                        answer.Media.Extension = "audio/mp3";
                                                    }
                                                    else
                                                    {
                                                        answer.Media.Extension = extension;
                                                    }
                                                    //item.Media.Extension = extension.Equals(".mp4")?"video/mp4":extension;
                                                    answer.Media.Path = mediarsp.Path;
                                                }
                                            }

                                            //if (answer.Media != null)

                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            var oldanswer = _lessonPartAnswerExtensionService.GetItemByID(answer.ID);
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
                                    var file = files.Where(f => f.Name == answer.Media.Name).FirstOrDefault();//update media
                                    string extension = Path.GetExtension(file.FileName);
                                    if (file != null)
                                    {
                                        if (typeImage.Contains(extension))
                                        {
                                            answer.Media.Created = DateTime.UtcNow;
                                            answer.Media.Size = file.Length;
                                            answer.Media.Path = await _fileProcess.SaveMediaAsync(file, answer.Media.OriginalName, "", basis);
                                            answer.Media.Extension = "image/png";
                                        }
                                        else
                                        {
                                            var mediarsp = _roxyFilemanHandler.UploadSingleFileWithGoogleDrive(basis, UserID, file);
                                            answer.Media = new Media();
                                            answer.Media.Name = item.Media.OriginalName = file.FileName;
                                            answer.Media.Created = DateTime.UtcNow;
                                            answer.Media.Size = file.Length;
                                            if (typeVideo.Contains(extension))
                                            {
                                                answer.Media.Extension = "video/mp4";
                                            }
                                            else if (typeAudio.Contains(extension))
                                            {
                                                answer.Media.Extension = "audio/mp3";
                                            }
                                            else
                                            {
                                                answer.Media.Extension = extension;
                                            }
                                            //item.Media.Extension = extension.Equals(".mp4")?"video/mp4":extension;
                                            answer.Media.Path = mediarsp.Path;
                                        }
                                    }
                                }
                            }
                            //else // No Media
                            //{
                            //    quiz.Media = null;
                            //}
                            answer.Order = oldanswer.Order;
                            answer.Created = oldanswer.Created;
                            answer.Updated = DateTime.UtcNow;
                        }
                        //_answerService.CreateOrUpdate(answer);
                        if (answer.ID != null)
                        {
                            _lessonPartAnswerExtensionService.CreateQuery().ReplaceOne(t => t.ID == answer.ID, answer as LessonPartAnswerExtensionEntity);
                        }
                        else
                        {
                            _lessonPartAnswerExtensionService.CreateQuery().InsertOne(answer as LessonPartAnswerExtensionEntity);
                        }
                    }
                }
            }
        }

        public JsonResult GetDetail(string ID)
        {
            var part = _lessonPartExtensionService.CreateQuery().Find(o => o.ID == ID).SingleOrDefault();
            if (part == null) return new JsonResult(new Dictionary<string, object>
                    {
                        { "Data", null },
                        {"Error", "Data not found" }
                    });
            var full_item = new LessonPartViewModel(part)
            {
                Questions = _lessonPartQuestionExtensionServie.CreateQuery().Find(o => o.ParentID == part.ID).SortBy(o => o.Order).ThenBy(o => o.ID).ToList().Select(t =>
                      new QuestionViewModel(t)
                      {
                          Answers = _lessonPartAnswerExtensionService.ConvertToCloneLessonPartAns(_lessonPartAnswerExtensionService.CreateQuery().Find(a => a.ParentID == t.ID).SortBy(o => o.Order).ThenBy(o => o.ID).ToList())
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
    }
}
