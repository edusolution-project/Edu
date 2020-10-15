using Core_v2.Globals;
using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BaseCustomerEntity.Database
{
    public class ExamEntity : EntityBase
    {
        [JsonProperty("Timer")]
        public int Timer { get; set; }
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
        [JsonProperty("ClassSubjectID")]
        public string ClassSubjectID { get; set; }
        [JsonProperty("TeacherID")]
        public string TeacherID { get; set; }
        [JsonProperty("LessonID")]
        public string LessonID { get; set; }
        [JsonProperty("LessonScheduleID")]
        public string LessonScheduleID { get; set; }
        [JsonProperty("StudentID")]
        public string StudentID { get; set; } // admin/student/teacher
        [JsonProperty("Status")]
        public bool Status { get; set; }
        [JsonProperty("Number")]
        public int Number { get; set; }
        [JsonProperty("CurrentDoTime")]
        public DateTime CurrentDoTime { get; set; }
        [JsonProperty("Point")]
        public double Point { get; set; }
        [JsonProperty("MaxPoint")]
        public double MaxPoint { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("Updated")]
        public DateTime Updated { get; set; }
        [JsonProperty("Marked")]
        public bool Marked { get; set; }
        [JsonProperty("QuestionsTotal")]
        public long QuestionsTotal { get; set; }
        [JsonProperty("QuestionsDone")]
        public long QuestionsDone { get; set; }
        [JsonProperty("QuestionsPass")]
        public long QuestionsPass { get; set; }
        [JsonProperty("LastPoint")]
        public double LastPoint { get; set; }
        //[JsonProperty("OldExamID")]
        //public string OldExamID { get; set; }
    }

    public class ExamService : ServiceBase<ExamEntity>
    {
        private ExamDetailService _examDetailService;
        private CloneLessonPartService _cloneLessonPartService;
        private CloneLessonPartQuestionService _cloneLessonPartQuestionService;
        private CloneLessonPartAnswerService _cloneLessonPartAnswerService;

        private LessonProgressService _lessonProgressService;
        private ChapterProgressService _chapterProgressService;
        private ClassSubjectProgressService _classSubjectProgressService;
        private ClassProgressService _classProgressService;
        //private LessonService _lessonService { get; set; }

        public ExamService(IConfiguration configuration) : base(configuration)
        {
            var indexs = new List<CreateIndexModel<ExamEntity>>
            {
                //ClassSubjectID_1_LessonID_1_StudentID_1_ID_-1
                new CreateIndexModel<ExamEntity>(
                    new IndexKeysDefinitionBuilder<ExamEntity>()
                    .Ascending(t=> t.ClassSubjectID)
                    .Ascending(t=> t.LessonID)
                    .Ascending(t=> t.StudentID)
                    .Descending(t=> t.ID)),
                //LessonID_1_StudentID_1_Status_-1
                 new CreateIndexModel<ExamEntity>(
                    new IndexKeysDefinitionBuilder<ExamEntity>()
                    .Ascending(t=> t.LessonID)
                    .Ascending(t=> t.StudentID)
                    .Descending(t=> t.Status)),
                //LessonScheduleID_1_StudentID_1
                new CreateIndexModel<ExamEntity>(
                    new IndexKeysDefinitionBuilder<ExamEntity>()
                    .Ascending(t=> t.LessonScheduleID)
                    .Ascending(t=> t.StudentID)
                    )
            };

            Collection.Indexes.CreateManyAsync(indexs);

            _examDetailService = new ExamDetailService(configuration);
            _cloneLessonPartService = new CloneLessonPartService(configuration);
            _cloneLessonPartQuestionService = new CloneLessonPartQuestionService(configuration);
            _cloneLessonPartAnswerService = new CloneLessonPartAnswerService(configuration);
            _lessonProgressService = new LessonProgressService(configuration);

            _chapterProgressService = new ChapterProgressService(configuration);
            _classSubjectProgressService = new ClassSubjectProgressService(configuration);
            _classProgressService = new ClassProgressService(configuration);

            //_lessonService = new LessonService(configuration);
        }
        /// <summary>
        /// ID Exam
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool IsOver(string ID)
        {
            var item = GetItemByID(ID);
            if (item == null || item.Status) return true;//break if exam not found or completed
            if (item.Timer == 0) return false;
            double count = (item.Created.AddMinutes(item.Timer) - DateTime.UtcNow).TotalMilliseconds;
            if (count <= 0)
            {
                UpdateStatus(item);
            }
            return count <= 0;
        }

        public Task UpdateStatus(ExamEntity exam)
        {
            exam.Status = true;
            exam.Updated = DateTime.Now;
            CreateOrUpdate(exam);
            return Task.CompletedTask;
        }

        //public ExamEntity CompleteNoEssay(ExamEntity exam, LessonEntity lesson, out double point, bool updateTime = true)
        //{
        //    exam.Status = true;
        //    point = 0;
        //    var pass = 0;
        //    var listDetails = _examDetailService.Collection.Find(o => o.ExamID == exam.ID).ToList();
        //    //Fix duplicate
        //    if (exam.Number == 1)
        //    {
        //        CreateQuery().UpdateMany(t => t.StudentID == exam.StudentID && t.LessonID == exam.StudentID && t.ID != exam.ID, Builders<ExamEntity>.Update.Set(t => t.Number, 0));
        //    }

        //    var regex = new System.Text.RegularExpressions.Regex(@"[^0-9a-zA-Z:,]+");
        //    for (int i = 0; listDetails != null && i < listDetails.Count; i++)
        //    {
        //        // check câu trả lời đúng
        //        bool isTrue = false;
        //        var examDetail = listDetails[i];

        //        // giá trị câu trả lời 
        //        //var answerValue = string.IsNullOrEmpty(examDetail.AnswerValue) ? string.Empty : regex.Replace(examDetail.AnswerValue, "")?.ToLower()?.Trim();

        //        //bài tự luận
        //        if (string.IsNullOrEmpty(examDetail.QuestionID) || examDetail.QuestionID == "0") continue;

        //        var part = _cloneLessonPartService.GetItemByID(examDetail.LessonPartID);
        //        if (part == null) continue; //Lưu lỗi => bỏ qua ko tính điểm

        //        var question = _cloneLessonPartQuestionService.GetItemByID(examDetail.QuestionID);
        //        if (question == null) continue; //Lưu lỗi => bỏ qua ko tính điểm

        //        var realAnswers = _cloneLessonPartAnswerService.CreateQuery().Find(o => o.IsCorrect && o.ParentID == examDetail.QuestionID).ToList();

        //        CloneLessonPartAnswerEntity _correctanswer = null;

        //        //bài chọn hoặc nối đáp án
        //        if (!string.IsNullOrEmpty(examDetail.AnswerID) && realAnswers.Count > 0)
        //        {
        //            switch (part.Type)
        //            {
        //                case "QUIZ1":
        //                    if (_cloneLessonPartAnswerService.GetItemByID(examDetail.AnswerID) == null) continue;
        //                    _correctanswer = realAnswers.FirstOrDefault(t => t.ID == examDetail.AnswerID);
        //                    if (_correctanswer == null) continue;
        //                    examDetail.RealAnswerID = _correctanswer.ID;
        //                    examDetail.RealAnswerValue = _correctanswer.Content;
        //                    break;
        //                case "QUIZ3":
        //                    if (_cloneLessonPartAnswerService.GetItemByID(examDetail.AnswerID) == null) continue;
        //                    _correctanswer = realAnswers.FirstOrDefault(t => t.ID == examDetail.AnswerID);
        //                    //ID not match => check value
        //                    if (_correctanswer == null && !string.IsNullOrEmpty(examDetail.AnswerValue))
        //                        _correctanswer = realAnswers.FirstOrDefault(t => t.Content == examDetail.AnswerValue);
        //                    if (_correctanswer == null) continue;
        //                    examDetail.RealAnswerID = _correctanswer.ID;
        //                    examDetail.RealAnswerValue = _correctanswer.Content;
        //                    break;
        //                case "QUIZ4":
        //                    var realIds = examDetail.AnswerID.Split(',');
        //                    examDetail.RealAnswerID = string.Join(",", realAnswers.Select(t => t.ID));
        //                    examDetail.RealAnswerValue = string.Join(",", realAnswers.Select(t => t.Content));
        //                    if (realIds.Length != realAnswers.Count()) continue;//incorrect
        //                    var isCorrect = true;
        //                    foreach (var id in realIds)
        //                    {
        //                        if (realAnswers.FirstOrDefault(t => t.ID == id) == null)//incorrect
        //                        {
        //                            isCorrect = false;
        //                            break;
        //                        }
        //                    }
        //                    if (isCorrect)
        //                    {
        //                        _correctanswer = realAnswers.FirstOrDefault();
        //                        if (_correctanswer == null) continue;
        //                        _correctanswer.ID = examDetail.AnswerID;
        //                        _correctanswer.Content = examDetail.AnswerValue;
        //                    }
        //                    break;
        //            }
        //        }
        //        else //bài điền từ
        //        {
        //            if (examDetail.AnswerValue != null && part.Type == "QUIZ2")
        //            {
        //                var _realAnwserQuiz2 = realAnswers?.ToList();

        //                if (_realAnwserQuiz2 == null) continue;
        //                List<string> quiz2answer = new List<string>();
        //                foreach (var answer in _realAnwserQuiz2)
        //                {
        //                    if (!string.IsNullOrEmpty(answer.Content))
        //                        foreach (var ans in answer.Content.Split('|'))
        //                        {
        //                            if (!string.IsNullOrEmpty(ans.Trim()))
        //                                quiz2answer.Add(NormalizeSpecialApostrophe(ans.Trim()));
        //                        }
        //                }
        //                var normalizeAns = NormalizeSpecialApostrophe(examDetail.AnswerValue.Trim());

        //                if (quiz2answer.Contains(normalizeAns))
        //                    _correctanswer = _realAnwserQuiz2.FirstOrDefault(); //điền từ đúng, chấp nhận viết hoa viết thường
        //            }

        //        }

        //        if (_correctanswer != null)
        //        {
        //            point += question.Point;
        //            pass++;
        //            examDetail.Point = question.Point;
        //            examDetail.RealAnswerID = _correctanswer.ID;
        //            examDetail.RealAnswerValue = _correctanswer.Content;
        //        }
        //        if (updateTime)
        //            examDetail.Updated = DateTime.Now;
        //        _examDetailService.CreateOrUpdate(examDetail);
        //    }

        //    exam.QuestionsPass = pass;
        //    exam.Point = point;
        //    if (updateTime)
        //        exam.Updated = DateTime.Now;
        //    exam.MaxPoint = lesson.Point;
        //    exam.QuestionsDone = listDetails.Count();
        //    //Tổng số câu hỏi = tổng số câu hỏi + số phần tự luận
        //    exam.QuestionsTotal =
        //        _cloneLessonPartQuestionService.Collection.CountDocuments(t => t.LessonID == exam.LessonID);
        //    //_cloneLessonPartService.Collection.CountDocuments(t => t.ParentID == lesson.ID && t.Type == "essay");

        //    var lessonProgress = _lessonProgressService.UpdateLastPoint(exam).Result;
        //    //_lessonProgressService.GetByClassSubjectID_StudentID_LessonID(exam.ClassSubjectID, exam.StudentID, exam.LessonID);
        //    Save(exam);

        //    if (lesson.TemplateType == LESSON_TEMPLATE.EXAM
        //    //&& lesson.Etype != LESSON_ETYPE.PRACTICE
        //    )
        //    {
        //        if (lessonProgress.ChapterID != "0")
        //            _ = _lessonHelp
        //                _chapterProgressService.UpdatePoint(lessonProgress);
        //        else
        //            _ = _classSubjectProgressService.UpdatePoint(lessonProgress);
        //    }
        //    else
        //    {
        //        if (lessonProgress.ChapterID != "0")
        //            _ = _chapterProgressService.UpdatePracticePoint(lessonProgress);
        //        else
        //            _ = _classSubjectProgressService.UpdatePracticePoint(lessonProgress);
        //    }

        //    return exam;
        //}

        ////Hoàn thành bài tự luận
        //public ExamEntity CompleteFull(ExamEntity exam, LessonEntity lesson, out double point, bool updateTime = true)
        //{
        //    var oldEx = GetItemByID(exam.ID);
        //    exam.Status = true;
        //    point = 0;
        //    var pass = 0;
        //    var listDetails = _examDetailService.Collection.Find(o => o.ExamID == exam.ID).ToList();
        //    foreach (var detail in listDetails)
        //        point += detail.Point;

        //    exam.Point = point;
        //    exam.LastPoint = oldEx != null ? oldEx.Point : 0;
        //    if (updateTime)
        //        exam.Updated = DateTime.Now;
        //    exam.MaxPoint = lesson.Point;
        //    exam.QuestionsDone = listDetails.Count();

        //    var pointchange = exam.MaxPoint > 0 ? (exam.Point - exam.LastPoint) * 100 / exam.MaxPoint : 0;

        //    var lessonProgress = _lessonProgressService.UpdateLastPoint(exam).Result;
        //    Save(exam);
        //    if (lesson.TemplateType == LESSON_TEMPLATE.EXAM)
        //    {
        //        if (lessonProgress.ChapterID != "0")
        //            _ = _chapterProgressService.UpdatePoint(lessonProgress, pointchange);
        //        else
        //            _ = _classSubjectProgressService.UpdatePoint(lessonProgress, pointchange);
        //    }
        //    else
        //    {
        //        if (lessonProgress.ChapterID != "0")
        //            _ = _chapterProgressService.UpdatePracticePoint(lessonProgress, pointchange);
        //        else
        //            _ = _classSubjectProgressService.UpdatePracticePoint(lessonProgress, pointchange);
        //    }

        //    return exam;
        //}

        //public bool ResetLesssonPoint(LessonEntity lesson, string studentID)
        //{
        //    var result = false;
        //    var lessonProgress = _lessonProgressService.GetByStudentID_LessonID(studentID, lesson.ID);
        //    if (lessonProgress != null)
        //    {
        //        if (lesson.TemplateType == LESSON_TEMPLATE.EXAM)
        //        {
        //            _chapterProgressService.DecreasePoint(lessonProgress);
        //            _classSubjectProgressService.DecreasePoint(lessonProgress);
        //            _classProgressService.DecreasePoint(lessonProgress);
        //        }
        //        else
        //        {
        //            _chapterProgressService.DecreasePracticePoint(lessonProgress);
        //            _classSubjectProgressService.DecreasePracticePoint(lessonProgress);
        //            _classProgressService.DecreasePracticePoint(lessonProgress);
        //        }
        //        _lessonProgressService.ResetPoint(lessonProgress);
        //    }
        //    return result;
        //}

        public ExamEntity GetLastestExam(string LessonID)
        {
            return Collection.Find(o => o.LessonID == LessonID).SortByDescending(o => o.Number).FirstOrDefault();
        }

        public async Task RemoveClassExam(string ClassID)
        {
            var extask = Collection.DeleteManyAsync(t => t.ClassID == ClassID);
            var edtask = _examDetailService.Collection.DeleteManyAsync(t => t.ClassID == ClassID);
            await Task.WhenAll(extask, edtask);
        }

        public async Task RemoveManyClassExam(string[] ids)
        {
            var extask = Collection.DeleteManyAsync(t => ids.Contains(t.ClassID));
            var edtask = _examDetailService.Collection.DeleteManyAsync(t => ids.Contains(t.ClassID));
            await Task.WhenAll(extask, edtask);
        }

        public async Task RemoveClassStudentExam(string ClassID, string StudentID)
        {
            var extask = Collection.DeleteManyAsync(t => t.ClassID == ClassID && t.StudentID == StudentID);
            var edtask = _examDetailService.Collection.DeleteManyAsync(t => t.ClassID == ClassID && t.StudentID == StudentID);
            await Task.WhenAll(extask, edtask);
        }

        public async Task RemoveClassSubjectExam(string ClassSubjectID)
        {
            var extask = Collection.DeleteManyAsync(t => t.ClassSubjectID == ClassSubjectID);
            var edtask = _examDetailService.Collection.DeleteManyAsync(t => t.ClassSubjectID == ClassSubjectID);
            await Task.WhenAll(extask, edtask);
        }

        public async Task ConvertClassSubject(ClassSubjectEntity classSubject)
        {
            await Collection.UpdateManyAsync(t => t.ClassID == classSubject.ClassID, Builders<ExamEntity>.Update.Set("ClassSubjectID", classSubject.ID));
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
    }
}
