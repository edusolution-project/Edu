using BaseCustomerEntity.Database;
using Core_v2.Globals;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseCustomerMVC.Globals
{
    public class LessonHelper
    {
        private readonly LessonService _lessonService;
        private readonly CourseLessonService _courseLessonService;

        private readonly ExamService _examService;
        private readonly ExamDetailService _examDetailService;

        private readonly CalendarHelper _calendarHelper;

        private readonly LessonPartService _lessonPartService;
        private readonly LessonPartAnswerService _lessonPartAnswerService;
        private readonly LessonPartQuestionService _lessonPartQuestionService;

        private readonly CloneLessonPartService _cloneLessonPartService;
        private readonly CloneLessonPartAnswerService _cloneAnswerService;
        private readonly CloneLessonPartQuestionService _cloneQuestionService;

        private readonly ProgressHelper _progressHelper;

        private readonly MappingEntity<LessonPartEntity, CloneLessonPartEntity> _lessonPartMapping = new MappingEntity<LessonPartEntity, CloneLessonPartEntity>();
        private readonly MappingEntity<LessonPartQuestionEntity, CloneLessonPartQuestionEntity> _lessonPartQuestionMapping = new MappingEntity<LessonPartQuestionEntity, CloneLessonPartQuestionEntity>();
        private readonly MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity> _lessonPartAnswerMapping = new MappingEntity<LessonPartAnswerEntity, CloneLessonPartAnswerEntity>();

        private readonly MappingEntity<CloneLessonPartEntity, CloneLessonPartEntity> _cloneLessonPartMapping = new MappingEntity<CloneLessonPartEntity, CloneLessonPartEntity>();
        private readonly MappingEntity<CloneLessonPartQuestionEntity, CloneLessonPartQuestionEntity> _cloneLessonPartQuestionMapping = new MappingEntity<CloneLessonPartQuestionEntity, CloneLessonPartQuestionEntity>();
        private readonly MappingEntity<CloneLessonPartAnswerEntity, CloneLessonPartAnswerEntity> _cloneLessonPartAnswerMapping = new MappingEntity<CloneLessonPartAnswerEntity, CloneLessonPartAnswerEntity>();

        private readonly MappingEntity<CourseLessonEntity, LessonEntity> _courseLessonMapping = new MappingEntity<CourseLessonEntity, LessonEntity>();
        private readonly MappingEntity<LessonEntity, LessonEntity> _lessonMapping = new MappingEntity<LessonEntity, LessonEntity>();
        private readonly List<string> quizType = new List<string> { "QUIZ1", "QUIZ2", "QUIZ3", "QUIZ4", "ESSAY" };
        private readonly DateTime invalidTime = new DateTime(1900, 1, 1);

        //extension
        private readonly CloneLessonPartExtensionService _cloneLessonPartExtensionService;
        private readonly CloneLessonPartQuestionExtensionService _cloneLessonPartQuestionExtensionService;
        private readonly CloneLessonPartAnswerExtensionService _cloneLessonPartAnswerExtensionService;

        public LessonHelper(
            LessonService lessonService,
            CourseLessonService courseLessonService,

            ExamService examService,
            ExamDetailService examDetailService,

            CalendarHelper calendarHelper,

            LessonPartService lessonPartService,
            LessonPartQuestionService lessonPartQuestionService,
            LessonPartAnswerService lessonPartAnswerService,

            CloneLessonPartService cloneLessonPartService,
            CloneLessonPartAnswerService cloneLessonPartAnswerService,
            CloneLessonPartQuestionService cloneLessonPartQuestionService,

            ProgressHelper progressHelper,
            CloneLessonPartExtensionService cloneLessonPartExtensionService,
            CloneLessonPartQuestionExtensionService cloneLessonPartQuestionExtensionService,
            CloneLessonPartAnswerExtensionService cloneLessonPartAnswerExtensionService

        )
        {
            _lessonService = lessonService;
            _courseLessonService = courseLessonService;

            _examService = examService;
            _examDetailService = examDetailService;

            _calendarHelper = calendarHelper;

            _lessonPartService = lessonPartService;
            _lessonPartQuestionService = lessonPartQuestionService;
            _lessonPartAnswerService = lessonPartAnswerService;

            _cloneLessonPartService = cloneLessonPartService;
            _cloneAnswerService = cloneLessonPartAnswerService;
            _cloneQuestionService = cloneLessonPartQuestionService;

            _progressHelper = progressHelper;
            _cloneLessonPartExtensionService = cloneLessonPartExtensionService;
            _cloneLessonPartQuestionExtensionService = cloneLessonPartQuestionExtensionService;
            _cloneLessonPartAnswerExtensionService = cloneLessonPartAnswerExtensionService;
        }

        public async Task RemoveClassSubjectLesson(string ClassSubjectID)
        {
            var lids = _lessonService.GetClassSubjectLesson(ClassSubjectID).Select(o => o.ID).ToList();
            if (lids != null && lids.Count() > 0)
                _calendarHelper.RemoveManySchedules(lids);

            var lstask = _lessonService.Collection.DeleteManyAsync(o => o.ClassSubjectID == ClassSubjectID);

            var cltask = _cloneLessonPartService.Collection.DeleteManyAsync(o => o.ClassSubjectID == ClassSubjectID);
            var cqtask = _cloneQuestionService.Collection.DeleteManyAsync(o => o.ClassSubjectID == ClassSubjectID);
            var catask = _cloneAnswerService.Collection.DeleteManyAsync(o => o.ClassSubjectID == ClassSubjectID);
            await Task.WhenAll(lstask, cltask, cqtask, catask);
        }

        public async Task RemoveManyClassLessons(string[] ids)
        {
            var lids = _lessonService.Collection.Find(o => ids.Contains(o.ClassID)).Project(o => o.ID).ToList();
            if (lids != null && lids.Count() > 0)
                _calendarHelper.RemoveManySchedules(lids);

            var lstask = _lessonService.Collection.DeleteManyAsync(o => ids.Contains(o.ClassID));

            var cltask = _cloneLessonPartService.Collection.DeleteManyAsync(o => ids.Contains(o.ClassID));
            var cqtask = _cloneQuestionService.Collection.DeleteManyAsync(o => ids.Contains(o.ClassID));
            var catask = _cloneAnswerService.Collection.DeleteManyAsync(o => ids.Contains(o.ClassID));
            await Task.WhenAll(lstask, cltask, cqtask, catask);
        }

        public async Task RemoveSingleLesson(string id)
        {
            var lstask = _lessonService.Collection.DeleteManyAsync(o => o.ID == id);
            _calendarHelper.RemoveLessonSchedule(id);
            //var sctask = _lessonScheduleService.Collection.DeleteManyAsync(o => o.LessonID == id);
            var cltask = _cloneLessonPartService.Collection.DeleteManyAsync(o => o.ParentID == id);
            var quizs = _cloneQuestionService.Collection.Find(o => o.LessonID == id).Project(t => t.ID).ToList();
            var cqtask = _cloneQuestionService.Collection.DeleteManyAsync(o => o.LessonID == id);
            Task<DeleteResult> catask = null;
            if (quizs != null && quizs.Count > 0)
            {
                catask = _cloneAnswerService.Collection.DeleteManyAsync(o => quizs.Contains(o.ParentID));
                await Task.WhenAll(lstask, cltask, cqtask, catask);
            }
            else
                await Task.WhenAll(lstask, cltask, cqtask);
        }

        public async Task RemoveSingleLessonContent(string id)
        {
            var cltask = _cloneLessonPartService.Collection.DeleteManyAsync(o => o.ParentID == id);
            var quizs = _cloneQuestionService.Collection.Find(o => o.LessonID == id).Project(t => t.ID).ToList();
            var cqtask = _cloneQuestionService.Collection.DeleteManyAsync(o => o.LessonID == id);
            Task<DeleteResult> catask = null;
            if (quizs != null && quizs.Count > 0)
            {
                catask = _cloneAnswerService.Collection.DeleteManyAsync(o => quizs.Contains(o.ParentID));
                await Task.WhenAll(cltask, cqtask, catask);
            }
            else
                await Task.WhenAll(cltask, cqtask);
        }



        public double calculateLessonPoint(string lessonId)
        {
            var point = 0.0;
            var parts = _lessonPartService.GetByLessonID(lessonId).Where(t => quizType.Contains(t.Type));
            if (parts != null && parts.Count() > 0)
                foreach (var part in parts)
                {
                    if (part.Type == "ESSAY")
                    {
                        point += part.Point;
                        _lessonPartQuestionService.Collection.UpdateMany(t => t.ParentID == part.ID, Builders<LessonPartQuestionEntity>.Update.Set(t => t.Point, part.Point));
                    }
                    else
                    {
                        point += _lessonPartQuestionService.GetByPartID(part.ID).Count();//trắc nghiệm => điểm = số câu hỏi (mỗi câu 1đ)
                        _lessonPartQuestionService.Collection.UpdateMany(t => t.ParentID == part.ID, Builders<LessonPartQuestionEntity>.Update.Set(t => t.Point, 1));
                    }
                }
            _courseLessonService.UpdateLessonPoint(lessonId, point);
            return point;
        }

        public double calculateCloneLessonPoint(string lessonId)
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

        #region Copy CourseLesson From CourseLesson
        public async Task CopyCourseLessonFromCourseLesson(CourseLessonEntity orgItem, CourseLessonEntity cloneItem, bool sync = false)
        {
            var lesson = cloneItem;
            if (!sync)
            {
                lesson = _courseLessonMapping.Clone(orgItem, new CourseLessonEntity());
                lesson.CreateUser = cloneItem.CreateUser;
                lesson.OriginID = orgItem.ID;
                lesson.Created = DateTime.UtcNow;
                lesson.Updated = DateTime.UtcNow;
                lesson.CourseID = cloneItem.CourseID;
                lesson.ChapterID = cloneItem.ChapterID;
                lesson.Order = cloneItem.Order;
                _courseLessonService.CreateQuery().InsertOne(lesson);
            }
            else
            {
                lesson.Point = orgItem.Point;
                lesson.IsPractice = orgItem.IsPractice;
                lesson.Updated = DateTime.UtcNow;
                _courseLessonService.Save(lesson);
            }

            var parts = _lessonPartService.CreateQuery().Find(o => o.ParentID == lesson.OriginID);

            foreach (var part in parts.ToEnumerable())
            {
                CloneCourseLessonPart(part, new LessonPartEntity
                {
                    ParentID = lesson.ID,
                    CourseID = lesson.CourseID
                });
            }
            if (sync)
            {
                calculateLessonPoint(lesson.ID);
            }
        }

        private void CloneCourseLessonPart(LessonPartEntity orgItem, LessonPartEntity cloneItem)
        {
            var item = _lessonPartMapping.Clone(orgItem, new CloneLessonPartEntity());
            item.ParentID = cloneItem.ParentID;
            item.CourseID = cloneItem.CourseID;
            item.OriginID = orgItem.ID;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            _lessonPartService.Collection.InsertOne(item);

            var questions = _lessonPartQuestionService.GetByPartID(item.OriginID);
            if (questions != null && questions.Count() > 0)
            {
                foreach (var question in questions)
                {
                    CloneCourseQuestion(question, new LessonPartQuestionEntity
                    {
                        ParentID = item.ID,
                        CourseID = item.CourseID
                    });
                }
            }
        }

        private void CloneCourseQuestion(LessonPartQuestionEntity orgItem, LessonPartQuestionEntity cloneItem)
        {
            var item = _lessonPartQuestionMapping.Clone(orgItem, new LessonPartQuestionEntity());

            item.OriginID = orgItem.ID;
            item.ParentID = cloneItem.ParentID;
            item.CourseID = cloneItem.CourseID;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            _lessonPartQuestionService.Collection.InsertOne(item);

            var list = _lessonPartAnswerService.GetByQuestionID(item.OriginID);
            if (list != null && list.Count() > 0)
            {
                foreach (var answer in list)
                {
                    CloneCourseAnswer(answer, new LessonPartAnswerEntity
                    {
                        ParentID = item.ID,
                        CourseID = item.CourseID
                    });

                }
            }
        }

        private void CloneCourseAnswer(LessonPartAnswerEntity orgItem, LessonPartAnswerEntity cloneItem)
        {
            var item = _lessonPartAnswerMapping.Clone(orgItem, new CloneLessonPartAnswerEntity());
            item.OriginID = orgItem.ID;
            item.ParentID = cloneItem.ParentID;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;
            item.CourseID = cloneItem.CourseID;

            _lessonPartAnswerService.Collection.InsertOne(item);
        }
        #endregion

        #region Copy Lesson From CourseLesson
        public async Task CopyLessonFromCourseLesson(CourseLessonEntity orgItem, LessonEntity cloneItem, DateTime begin, bool keepData = false)
        {
            var lesson = _courseLessonMapping.AutoOrtherType(orgItem, new LessonEntity());

            if (keepData)
            {
                lesson = cloneItem;
                lesson.IsPractice = orgItem.IsPractice;
                lesson.Point = orgItem.Point;
                _lessonService.Save(lesson);
            }
            else
            {
                lesson.ChapterID = cloneItem.ChapterID;
                lesson.OriginID = orgItem.ID;
                lesson.ClassID = cloneItem.ClassID;
                lesson.ClassSubjectID = cloneItem.ClassSubjectID;
                lesson.Created = DateTime.UtcNow;
                lesson.Updated = DateTime.UtcNow;
                lesson.ID = null;
                if (orgItem.Start == 0 && orgItem.Period == 0)//route not set
                {
                    lesson = InitLesson(lesson);
                }
                else
                {
                    if (begin > new DateTime(1900, 1, 1))
                    {
                        lesson.StartDate = begin.AddDays(orgItem.Start);
                        lesson.EndDate = begin.AddDays(orgItem.Start + orgItem.Period);
                    }
                    lesson = InitLesson(lesson);
                }
            }

            var lessonParts = _lessonPartService.GetByLessonID(lesson.OriginID).OrderBy(q => q.Order).ThenBy(q => q.ID).ToList();
            if (lessonParts != null && lessonParts.Count() > 0)
            {
                foreach (var lessonpart in lessonParts)
                {
                    CopyPart(lessonpart, new CloneLessonPartEntity
                    {
                        ParentID = lesson.ID,
                        TeacherID = lesson.CreateUser,
                        ClassID = lesson.ClassID,
                        ClassSubjectID = lesson.ClassSubjectID,
                    });
                }
            }
        }

        private void CopyPart(LessonPartEntity orgItem, CloneLessonPartEntity cloneItem)
        {

            var item = _lessonPartMapping.AutoOrtherType(orgItem, new CloneLessonPartEntity());
            item.ParentID = cloneItem.ParentID;
            item.ID = null;
            item.OriginID = orgItem.ID;
            item.TeacherID = cloneItem.TeacherID;
            item.ClassID = cloneItem.ClassID;
            item.ClassSubjectID = cloneItem.ClassSubjectID;

            _cloneLessonPartService.Collection.InsertOne(item);

            var questions = _lessonPartQuestionService.GetByPartID(item.OriginID);
            if (questions != null && questions.Count() > 0)
            {
                foreach (var question in questions)
                {
                    CopyQuestion(question, new CloneLessonPartQuestionEntity
                    {
                        ParentID = item.ID,
                        ClassID = item.ClassID,
                        ClassSubjectID = item.ClassSubjectID,
                        LessonID = item.ParentID
                    });
                }
            }
        }

        private void CopyQuestion(LessonPartQuestionEntity orgItem, CloneLessonPartQuestionEntity cloneItem)
        {
            var item = _lessonPartQuestionMapping.AutoOrtherType(orgItem, new CloneLessonPartQuestionEntity());

            item.OriginID = orgItem.ID;
            item.ParentID = cloneItem.ParentID;
            item.ID = null;
            item.ClassID = cloneItem.ClassID;
            item.ClassSubjectID = cloneItem.ClassSubjectID;
            item.LessonID = cloneItem.LessonID;

            _cloneQuestionService.Collection.InsertOne(item);

            var list = _lessonPartAnswerService.GetByQuestionID(item.OriginID);
            if (list != null && list.Count() > 0)
            {
                foreach (var answer in list)
                {
                    CopyAnswer(answer, new CloneLessonPartAnswerEntity
                    {
                        ParentID = item.ID,
                        ClassID = item.ClassID,
                        ClassSubjectID = item.ClassSubjectID,
                    });

                }
            }
        }

        private void CopyAnswer(LessonPartAnswerEntity orgItem, CloneLessonPartAnswerEntity cloneItem)
        {
            var item = _lessonPartAnswerMapping.AutoOrtherType(orgItem, new CloneLessonPartAnswerEntity());
            item.OriginID = orgItem.ID;
            item.ParentID = cloneItem.ParentID;
            item.ClassID = cloneItem.ClassID;
            item.ClassSubjectID = cloneItem.ClassSubjectID;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;
            item.ID = null;

            _cloneAnswerService.Collection.InsertOne(item);
        }

        #endregion

        #region Copy Lesson From Other Lesson
        public async Task CopyLessonFromLesson(LessonEntity orgLesson, LessonEntity cloneItem)
        {
            var lesson = _lessonMapping.Clone(orgLesson, new LessonEntity());
            lesson.ChapterID = cloneItem.ChapterID;
            lesson.OriginID = orgLesson.OriginID; //switch origin to root course - if existed
            lesson.ClassID = string.IsNullOrEmpty(cloneItem.ClassID) ? orgLesson.ClassID : cloneItem.ClassID;
            lesson.ClassSubjectID = string.IsNullOrEmpty(cloneItem.ClassSubjectID) ? orgLesson.ClassSubjectID : cloneItem.ClassSubjectID;
            lesson.Title = string.IsNullOrEmpty(cloneItem.Title) ? orgLesson.Title : cloneItem.Title;
            lesson.Created = DateTime.UtcNow;
            lesson.Updated = DateTime.UtcNow;
            lesson.Order = cloneItem.Order;
            //copy all schedule except online class
            //lesson.StartDate = DateTime.MinValue;
            //lesson.EndDate = DateTime.MinValue;
            //lesson.IsHideAnswer = false;
            lesson.IsOnline = false;
            lesson.ID = null;

            lesson = InitLesson(lesson);

            var lessonParts = _cloneLessonPartService.GetByLessonID(orgLesson.ID).OrderBy(q => q.Order).ThenBy(q => q.ID).ToList();
            if (lessonParts != null && lessonParts.Count() > 0)
            {
                foreach (var lessonpart in lessonParts)
                {
                    CopyClonePart(lessonpart, new CloneLessonPartEntity
                    {
                        ParentID = lesson.ID,
                        TeacherID = lesson.CreateUser,
                        ClassID = lesson.ClassID,
                        ClassSubjectID = lesson.ClassSubjectID,
                    });
                }
            }
        }

        private void CopyClonePart(CloneLessonPartEntity orgItem, CloneLessonPartEntity cloneItem)
        {

            var item = _cloneLessonPartMapping.Clone(orgItem, new CloneLessonPartEntity());
            item.ParentID = cloneItem.ParentID;
            item.ID = null;
            item.OriginID = orgItem.ID;
            item.TeacherID = cloneItem.TeacherID;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;
            item.ClassID = cloneItem.ClassID;
            item.ClassSubjectID = cloneItem.ClassSubjectID;

            _cloneLessonPartService.Collection.InsertOne(item);

            var questions = _cloneQuestionService.GetByPartID(item.OriginID);
            if (questions != null && questions.Count() > 0)
            {
                foreach (var question in questions)
                {
                    CopyCloneQuestion(question, new CloneLessonPartQuestionEntity
                    {
                        ParentID = item.ID,
                        ClassID = item.ClassID,
                        ClassSubjectID = item.ClassSubjectID,
                        LessonID = item.ParentID
                    });
                }
            }
        }

        private void CopyCloneQuestion(CloneLessonPartQuestionEntity orgItem, CloneLessonPartQuestionEntity cloneItem)
        {
            var item = _cloneLessonPartQuestionMapping.Clone(orgItem, new CloneLessonPartQuestionEntity());

            item.OriginID = orgItem.ID;
            item.ParentID = cloneItem.ParentID;
            item.ID = null;
            item.ClassID = cloneItem.ClassID;
            item.ClassSubjectID = cloneItem.ClassSubjectID;
            item.LessonID = cloneItem.LessonID;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            _cloneQuestionService.Collection.InsertOne(item);

            var list = _cloneAnswerService.GetByQuestionID(item.OriginID);
            if (list != null && list.Count() > 0)
            {
                foreach (var answer in list)
                {
                    CopyCloneAnswer(answer, new CloneLessonPartAnswerEntity
                    {
                        ParentID = item.ID,
                        ClassID = item.ClassID,
                        ClassSubjectID = item.ClassSubjectID,
                    });
                }
            }
        }

        private void CopyCloneAnswer(CloneLessonPartAnswerEntity orgItem, CloneLessonPartAnswerEntity cloneItem)
        {
            var item = _cloneLessonPartAnswerMapping.Clone(orgItem, new CloneLessonPartAnswerEntity());
            item.OriginID = orgItem.ID;
            item.ParentID = cloneItem.ParentID;
            item.ClassID = cloneItem.ClassID;
            item.ClassSubjectID = cloneItem.ClassSubjectID;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            _cloneAnswerService.Collection.InsertOne(item);
        }
        #endregion

        #region Score
        public ExamEntity CompleteNoEssay(ExamEntity exam, LessonEntity lesson, out double point, bool updateTime = true)
        {
            exam.Status = true;
            point = 0;
            var pass = 0;
            var listDetails = _examDetailService.Collection.Find(o => o.ExamID == exam.ID).ToList();

            //Fix duplicate
            if (exam.Number == 1)
            {
                _examService.CreateQuery().UpdateMany(t => t.StudentID == exam.StudentID && t.LessonID == exam.StudentID && t.ID != exam.ID, Builders<ExamEntity>.Update.Set(t => t.Number, 0));
            }

            var completedQ = new List<string>();

            for (int i = 0; listDetails != null && i < listDetails.Count; i++)
            {
                //var regex = new System.Text.RegularExpressions.Regex(@"[^0-9a-zA-Z:,]+");
                // check câu trả lời đúng
                bool isTrue = false;
                var examDetail = listDetails[i];

                // giá trị câu trả lời 
                //var answerValue = string.IsNullOrEmpty(examDetail.AnswerValue) ? string.Empty : regex.Replace(examDetail.AnswerValue, "")?.ToLower()?.Trim();

                //bài tự luận
                if (string.IsNullOrEmpty(examDetail.QuestionID) || examDetail.QuestionID == "0") continue;

                if (completedQ.Contains(examDetail.QuestionID)) continue;
                completedQ.Add(examDetail.QuestionID);


                var part = _cloneLessonPartService.GetItemByID(examDetail.LessonPartID);
                var part = new CloneLessonPartEntity();
                if(String.IsNullOrEmpty(exam.LessonExtensionID))
                {
                    part = _cloneLessonPartService.GetItemByID(examDetail.LessonPartID);
                }
                else if(!String.IsNullOrEmpty(exam.LessonExtensionID)){
                    part = _cloneLessonPartExtensionService.GetItemByID(examDetail.LessonPartID);
                }
                if (part == null) continue; //Lưu lỗi => bỏ qua ko tính điểm

                var question =new CloneLessonPartQuestionEntity();
                if (String.IsNullOrEmpty(exam.LessonExtensionID))
                {
                    question = _cloneQuestionService.GetItemByID(examDetail.QuestionID);
                }
                else if (!String.IsNullOrEmpty(exam.LessonExtensionID))
                {
                    question = _cloneLessonPartQuestionExtensionService.GetItemByID(examDetail.QuestionID);
                }
                if (question == null) continue; //Lưu lỗi => bỏ qua ko tính điểm

                var realAnswers = new List<CloneLessonPartAnswerExtensionEntity>();
                if (String.IsNullOrEmpty(exam.LessonExtensionID))
                {
                    //realAnswers = _cloneAnswerService.GetByQuestionID(examDetail.QuestionID).Where(o => o.IsCorrect).ToList();
                    var test = _cloneAnswerService.CreateQuery().Find(o => o.ParentID == examDetail.QuestionID).ToList(); //need check
                    foreach(var a in test)
                    {
                        var b = new CloneLessonPartAnswerExtensionEntity();
                        b = a as CloneLessonPartAnswerExtensionEntity;
                        realAnswers.Add(b);
                    }
                }
                else if (!String.IsNullOrEmpty(exam.LessonExtensionID))
                {
                    realAnswers = _cloneLessonPartAnswerExtensionService.GetByQuestionID(examDetail.QuestionID).Where(o => o.IsCorrect).ToList();
                }

                CloneLessonPartAnswerEntity _correctanswer = null;

                //bài chọn hoặc nối đáp án
                if (!string.IsNullOrEmpty(examDetail.AnswerID) && realAnswers.Count > 0)
                {
                    var ans = new CloneLessonPartAnswerExtensionEntity();
                    switch (part.Type)
                    {
                        case "QUIZ1":
                            if(String.IsNullOrEmpty(exam.LessonExtensionID))
                            {
                                ans = _cloneAnswerService.GetItemByID(examDetail.AnswerID) as CloneLessonPartAnswerExtensionEntity;
                            }
                            else if (!String.IsNullOrEmpty(exam.LessonExtensionID))
                            {
                                ans = _cloneLessonPartAnswerExtensionService.GetItemByID(examDetail.AnswerID);
                            }
                            if (ans == null) continue;
                            _correctanswer = realAnswers.FirstOrDefault(t => t.ID == examDetail.AnswerID);
                            if (_correctanswer == null) continue;
                            examDetail.RealAnswerID = _correctanswer.ID;
                            examDetail.RealAnswerValue = _correctanswer.Content;
                            break;
                        case "QUIZ3":
                            //var ans = new CloneLessonPartAnswerExtensionEntity();
                            if (String.IsNullOrEmpty(exam.LessonExtensionID))
                            {
                                ans = _cloneAnswerService.GetItemByID(examDetail.AnswerID) as CloneLessonPartAnswerExtensionEntity;
                            }
                            else if (!String.IsNullOrEmpty(exam.LessonExtensionID))
                            {
                                ans = _cloneLessonPartAnswerExtensionService.GetItemByID(examDetail.AnswerID);
                            }
                            if (ans == null) continue;
                            _correctanswer = realAnswers.FirstOrDefault(t => t.ID == examDetail.AnswerID);
                            //ID not match => check value
                            if (_correctanswer == null && !string.IsNullOrEmpty(examDetail.AnswerValue))
                                _correctanswer = realAnswers.FirstOrDefault(t => t.Content == examDetail.AnswerValue);
                            if (_correctanswer == null) continue;
                            examDetail.RealAnswerID = _correctanswer.ID;
                            examDetail.RealAnswerValue = _correctanswer.Content;
                            break;
                        case "QUIZ4":
                            var realIds = examDetail.AnswerID.Split(',');
                            examDetail.RealAnswerID = string.Join(",", realAnswers.Select(t => t.ID));
                            examDetail.RealAnswerValue = string.Join(",", realAnswers.Select(t => t.Content));
                            if (realIds.Length != realAnswers.Count()) continue;//incorrect
                            var isCorrect = true;
                            foreach (var id in realIds)
                            {
                                if (realAnswers.FirstOrDefault(t => t.ID == id) == null)//incorrect
                                {
                                    isCorrect = false;
                                    break;
                                }
                            }
                            if (isCorrect)
                            {
                                _correctanswer = realAnswers.FirstOrDefault();
                                if (_correctanswer == null) continue;
                                _correctanswer.ID = examDetail.AnswerID;
                                _correctanswer.Content = examDetail.AnswerValue;
                            }
                            break;
                    }
                }
                else //bài điền từ
                {
                    if (examDetail.AnswerValue != null && part.Type == "QUIZ2")
                    {
                        var _realAnwserQuiz2 = realAnswers?.ToList();

                        if (_realAnwserQuiz2 == null) continue;
                        List<CorrectAns> quiz2answer = new List<CorrectAns>();
                        foreach (var answer in _realAnwserQuiz2)
                        {
                            if (!string.IsNullOrEmpty(answer.Content))
                                foreach (var ans in answer.Content.Split('|'))
                                {
                                    if (!string.IsNullOrEmpty(ans.Trim()))
                                        quiz2answer.Add(new CorrectAns
                                        {
                                            ID = answer.ID,
                                            Value = NormalizeSpecialApostrophe(ans.Trim())
                                        }); ;
                                }
                        }
                        var normalizeAns = NormalizeSpecialApostrophe(examDetail.AnswerValue.Trim());

                        var cr = quiz2answer.FirstOrDefault(t => t.Value == normalizeAns);
                        if (cr != null)
                            _correctanswer = _realAnwserQuiz2.FirstOrDefault(t => t.ID == cr.ID); //điền từ đúng, chấp nhận viết hoa viết thường
                    }
                }

                if (_correctanswer != null)
                {
                    point += question.Point;
                    pass++;
                    examDetail.Point = question.Point;
                    examDetail.RealAnswerID = _correctanswer.ID;
                    examDetail.RealAnswerValue = _correctanswer.Content;
                }
                if (updateTime)
                    examDetail.Updated = DateTime.UtcNow;
                _examDetailService.Save(examDetail);
            }
            exam.QuestionsPass = pass;
            exam.Point = point;
            if (updateTime)
                exam.Updated = DateTime.UtcNow;
            exam.MaxPoint = lesson.Point;
            exam.QuestionsDone = listDetails.Count();
            //Tổng số câu hỏi = tổng số câu hỏi + số phần tự luận
            exam.QuestionsTotal = _cloneQuestionService.CountByLessonID(exam.LessonID); //chạy data fix điểm ktra 4/3/2021 xong thì phải bỏ cmt ra

            //temp fix
            if (exam.QuestionsTotal < exam.QuestionsDone || exam.MaxPoint < exam.QuestionsDone)
            {
                exam.QuestionsTotal = exam.QuestionsDone;
                exam.MaxPoint = exam.QuestionsDone;
            }

            var lessonProgress = _progressHelper.UpdateLessonPoint(exam).Result;

            _examService.Save(exam);

            //if (lessonProgress.ChapterID != "0")
            //    _ = _progressHelper.UpdateChapterPoint(lessonProgress);
            //else
            //    _ = _progressHelper.UpdateClassSubjectPoint(lessonProgress);

            return exam;
        }

        //Hoàn thành bài tự luận
        public ExamEntity CompleteFull(ExamEntity exam, LessonEntity lesson, out double point, bool updateTime = true)
        {
            var oldEx = _examService.GetItemByID(exam.ID);
            exam.Status = true;
            point = 0;
            var pass = 0;

            var listDetails = _examDetailService.GetByExamID(exam.ID);// Collection.Find(o => o.ExamID == exam.ID).ToList();
            foreach (var detail in listDetails)
                point += detail.Point;

            exam.Point = point;
            exam.LastPoint = oldEx != null ? oldEx.Point : 0;
            if (updateTime)
                exam.Updated = DateTime.UtcNow;
            exam.MaxPoint = lesson.Point;
            exam.QuestionsDone = listDetails.Count();
            _ = _progressHelper.UpdateLessonPoint(exam);
            _examService.Save(exam);
            return exam;
        }
        #endregion

        public LessonEntity InitLesson(LessonEntity lesson)
        {
            _lessonService.Save(lesson);
            _calendarHelper.ConvertCalendarFromLesson(lesson, "");
            return lesson;
        }

        public async Task ConvertClassSubject(ClassSubjectEntity classSubject)
        {
            var cltask = _cloneLessonPartService.Collection.UpdateManyAsync(t => t.ClassID == classSubject.ClassID, Builders<CloneLessonPartEntity>.Update.Set("ClassSubjectID", classSubject.ID));
            var cqtask = _cloneQuestionService.Collection.UpdateManyAsync(t => t.ClassID == classSubject.ClassID, Builders<CloneLessonPartQuestionEntity>.Update.Set("ClassSubjectID", classSubject.ID));
            var catask = _cloneAnswerService.Collection.UpdateManyAsync(t => t.ClassID == classSubject.ClassID, Builders<CloneLessonPartAnswerEntity>.Update.Set("ClassSubjectID", classSubject.ID));
            await Task.WhenAll(cltask, cqtask, catask);
        }

        public bool IsQuizLesson(string ID)
        {
            return _cloneLessonPartService.GetByLessonID(ID).Any(t => quizType.Contains(t.Type));
        }

        private string NormalizeSpecialApostrophe(string originStr)
        {
            return originStr
                .Replace("‘", "'")
                .Replace("’", "'")
                .Replace("“", "\"")
                .Replace("”", "\"")
                .Replace(" ", " ");
        }

        public bool IsOvertime(ExamEntity item, LessonEntity lesson, out int endType)
        {
            endType = EXAM_END_TYPE.OVERDATE;
            if (item == null || item.Status) return true;//break if exam not found or completed
            //var schedule = _lessonScheduleService.GetItemByLessonID(item.LessonID);
            //if (schedule == null) return true;//break if no schedule
            if (lesson.EndDate > new DateTime(1900, 1, 1) && lesson.EndDate < DateTime.UtcNow) //hết hạn
                return true;
            endType = EXAM_END_TYPE.TIMEOUT;
            if (item.Timer == 0) return false;
            double count = (item.Created.ToUniversalTime().AddMinutes(item.Timer) - DateTime.UtcNow).TotalMilliseconds;//hết thời gian làm bài
            return count <= 0;
        }

        internal bool isExamined(LessonEntity lesson)
        {
            if (lesson.TemplateType == LESSON_TEMPLATE.EXAM && lesson.IsPractice)
            {
                var isExamined = _examService.CreateQuery().Find(t => t.LessonID == lesson.ID).Any();
                //check
                return isExamined;
            }
            else//LECTURE only
                return true;
        }

    }

    public class CorrectAns
    {
        public string ID { get; set; }
        public string Value { get; set; }
    }

    public class EXAM_END_TYPE
    {
        public static int OVERDATE = 1,//het han theo lich bai
            TIMEOUT = 2;//het thoi gian lam bai
    }
}
