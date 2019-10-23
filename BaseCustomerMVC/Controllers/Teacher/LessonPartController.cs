using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using MongoDB.Driver;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class LessonPartController : TeacherController
    {
        private readonly GradeService _gradeService;
        private readonly SubjectService _subjectService;
        private readonly TeacherService _teacherService;
        private readonly ClassService _classService;
        private readonly CourseService _courseService;
        private readonly ChapterService _chapterService;
        private readonly LessonService _lessonService;
        private readonly LessonPartService _lessonPartService;
        private readonly LessonPartQuestionService _questionService;
        private readonly LessonPartAnswerService _answerService;
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly FileProcess _fileProcess;

        public LessonPartController(
            GradeService gradeservice,
            SubjectService subjectService,
            TeacherService teacherService,
            ClassService classService,
            CourseService courseService,
            ChapterService chapterService,
            LessonService lessonService,
            LessonPartService lessonPartService,
            LessonScheduleService lessonScheduleService,
            LessonPartQuestionService questionService,
            LessonPartAnswerService answerService,
            FileProcess fileProcess
            )
        {
            _gradeService = gradeservice;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _courseService = courseService;
            _classService = classService;
            _chapterService = chapterService;
            _lessonService = lessonService;
            _lessonPartService = lessonPartService;
            _lessonScheduleService = lessonScheduleService;
            _questionService = questionService;
            _answerService = answerService;
            _fileProcess = fileProcess;
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
        public async Task<JsonResult> CreateOrUpdate(LessonPartViewModel item, List<string> RemovedQuestions = null, List<string> RemovedAnswers = null)
        {
            //try
            //{
                var parentLesson = _lessonService.CreateQuery().Find(o => o.ID == item.ParentID).SingleOrDefault();
                if (parentLesson != null)//Chỉ add/edit được part trong lesson do mình tạo
                {
                    if (item.Media != null && item.Media.Name == null) item.Media = null;//valid Media
                    var files = HttpContext.Request.Form != null && HttpContext.Request.Form.Files.Count > 0 ? HttpContext.Request.Form.Files : null;
                    if (item.ID == "0" || item.ID == null) //create
                    {
                        item.Created = DateTime.Now;
                        var maxItem = _lessonPartService.CreateQuery()
                            .Find(o => o.ParentID == item.ParentID)
                            .SortByDescending(o => o.Order).FirstOrDefault();
                        item.Order = maxItem != null ? maxItem.Order + 1 : 0;
                        item.Updated = DateTime.Now;

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
                            item.Media.Path = await _fileProcess.SaveMediaAsync(file, item.Media.OriginalName);
                        }

                        item.Updated = DateTime.Now;
                        item.Created = olditem.Created;
                        item.Order = olditem.Order;
                    }

                    var lessonpart = item.ToEntity();
                    _lessonPartService.CreateOrUpdate(lessonpart);
                    item.ID = lessonpart.ID;

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

                    if (item.Questions != null && item.Questions.Count > 0)
                    {
                        foreach (var questionVM in item.Questions)
                        {
                            questionVM.ParentID = item.ID;
                            var quiz = questionVM.ToEntity();

                            if (questionVM.Media != null && questionVM.Media.Name == null) questionVM.Media = null;

                            if (questionVM.ID == "0" || questionVM.ID == null || _questionService.GetItemByID(quiz.ID) == null)
                            {
                                var maxItem = _questionService.CreateQuery()
                                    .Find(o => o.ParentID == lessonpart.ID)
                                    .SortByDescending(o => o.Order).FirstOrDefault();
                                quiz.Order = maxItem != null ? maxItem.Order + 1 : 0;
                                quiz.Created = DateTime.Now;
                                quiz.Updated = DateTime.Now;

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
                                        quiz.Media.Path = await _fileProcess.SaveMediaAsync(file, quiz.Media.OriginalName);
                                    }
                                }

                                quiz.Order = oldquiz.Order;
                                quiz.Created = oldquiz.Created;
                                quiz.Updated = DateTime.Now;
                            }

                            _questionService.CreateOrUpdate(quiz);
                            questionVM.ID = quiz.ID;

                            if (questionVM.Answers != null && questionVM.Answers.Count > 0)
                            {
                                foreach (var answer in questionVM.Answers)
                                {

                                    answer.ParentID = questionVM.ID;
                                    if (answer.Media != null && answer.Media.Name == null) answer.Media = null;

                                    if (answer.ID == "0" || answer.ID == null || _answerService.GetItemByID(answer.ID) == null)
                                    {
                                        var maxItem = _answerService.CreateQuery().Find(o => o.ParentID == quiz.ID).SortByDescending(o => o.Order).FirstOrDefault();
                                        answer.Order = maxItem != null ? maxItem.Order + 1 : 0;
                                        answer.Created = DateTime.Now;
                                        answer.Updated = DateTime.Now;

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
                                        var oldanswer = _answerService.GetItemByID(answer.ID);
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
                                        answer.Updated = DateTime.Now;
                                    }
                                    _answerService.CreateOrUpdate(answer);
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
                else
                {
                    return new JsonResult(new Dictionary<string, object>
                            {
                                { "Data", null },
                                {"Error", "Parent Item Not found" }
                            });
                }


            //}
            //catch (Exception ex)
            //{
            //    return new JsonResult(new Dictionary<string, object>
            //                {
            //                    { "Data", null },
            //                    {"Error", ex.Message }
            //                });
            //}
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
                    result.AddRange(listLessonPart.Select(o => new LessonPartViewModel(o)
                    {
                        Questions = _questionService.CreateQuery().Find(q => q.ParentID == o.ID).SortBy(q => q.Order).ThenBy(q => q.ID).ToList().Select(q => new QuestionViewModel(q)
                        {
                            Answers = _answerService.CreateQuery().Find(a => a.ParentID == q.ID).SortBy(a => a.Order).ThenBy(a => a.ID).ToList()
                        }).ToList()
                    }));
                    data = new Dictionary<string, object>
                    {
                        { "Data", result }
                    };
                };
            }

            return new JsonResult(data);
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
    }
}
