using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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
                //ClassID_1_LessonID_1_StudentID_1_ID_-1
                new CreateIndexModel<ExamEntity>(
                    new IndexKeysDefinitionBuilder<ExamEntity>()
                    .Ascending(t=> t.ClassID)
                    .Ascending(t=> t.LessonID)
                    .Ascending(t=> t.StudentID)
                    .Descending(t=> t.ID))
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
        public bool IsOverTime(string ID)
        {
            var item = GetItemByID(ID);
            if (item == null) return false;
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

        public ExamEntity Complete(ExamEntity exam, LessonEntity lesson, out double point)
        {
            exam.Status = true;
            point = 0;
            var listDetails = _examDetailService.Collection.Find(o => o.ExamID == exam.ID).ToList();

            for (int i = 0; listDetails != null && i < listDetails.Count; i++)
            {
                var examDetail = listDetails[i];

                //bài tự luận
                if (string.IsNullOrEmpty(examDetail.QuestionID) || examDetail.QuestionID == "0") continue;

                var part = _cloneLessonPartService.GetItemByID(examDetail.LessonPartID);
                if (part == null) continue; //Lưu lỗi => bỏ qua ko tính điểm

                var question = _cloneLessonPartQuestionService.GetItemByID(examDetail.QuestionID);
                if (question == null) continue; //Lưu lỗi => bỏ qua ko tính điểm

                var _realAnswers = _cloneLessonPartAnswerService.CreateQuery().Find(o => o.IsCorrect && o.ParentID == examDetail.QuestionID).ToList();

                CloneLessonPartAnswerEntity _correctanswer = null;

                var realanswer = _realAnswers.FirstOrDefault();
                if (realanswer != null)
                {
                    examDetail.RealAnswerID = realanswer.ID;
                    examDetail.RealAnswerValue = realanswer.Content;
                }

                //bài chọn hoặc nối đáp án
                if (!string.IsNullOrEmpty(examDetail.AnswerID))
                {
                    var answer = _cloneLessonPartAnswerService.GetItemByID(examDetail.AnswerID);
                    if (answer == null) continue;//Lưu lỗi => bỏ qua ko tính điểm


                    switch (part.Type)
                    {
                        case "QUIZ1": //chọn đáp án
                            _correctanswer = _realAnswers.FirstOrDefault(t => t.ID == answer.ID);//chọn đúng đáp án
                            break;
                        case "QUIZ3": //nối đáp án
                            _correctanswer = _realAnswers.FirstOrDefault(t => t.ID == answer.ID || (!string.IsNullOrEmpty(t.Content) && t.Content == answer.Content)); //chọn đúng đáp án (check trường hợp sai ID nhưng cùng content (2 đáp án có hình ảnh, ID khác nhau nhưng cùng content (nội dung như nhau)))
                            break;
                    }
                }
                else //bài điền từ
                {
                    if (examDetail.AnswerValue != null)
                    {
                        List<string> quiz2answer = new List<string>();
                        foreach (var answer in _realAnswers)
                        {
                            if (!string.IsNullOrEmpty(answer.Content))
                                foreach (var ans in answer.Content.Split('/'))
                                {
                                    if (!string.IsNullOrEmpty(ans.Trim()))
                                        quiz2answer.Add(ans.Trim().ToLower());
                                }
                        }

                        if (quiz2answer.Contains(examDetail.AnswerValue.ToLower().Trim()))
                            _correctanswer = _realAnswers.FirstOrDefault(); //điền từ đúng, chấp nhận viết hoa viết thường
                    }

                }

                if (_correctanswer != null)
                {
                    point += question.Point;
                    examDetail.Point = question.Point;
                    examDetail.RealAnswerID = _correctanswer.ID;
                    examDetail.RealAnswerValue = _correctanswer.Content;
                }

                examDetail.Updated = DateTime.Now;
                _examDetailService.CreateOrUpdate(examDetail);
            }
            exam.Point = point;
            exam.Updated = DateTime.Now;
            exam.MaxPoint = lesson.Point;
            exam.QuestionsDone = listDetails.Count();
            //Tổng số câu hỏi = tổng số câu hỏi + số phần tự luận
            exam.QuestionsTotal =
                _cloneLessonPartQuestionService.Collection.CountDocuments(t => t.LessonID == lesson.ID);
            //_cloneLessonPartService.Collection.CountDocuments(t => t.ParentID == lesson.ID && t.Type == "essay");
            _ = _lessonProgressService.UpdateLastPoint(exam);
            var lessonProgress = _lessonProgressService.GetByClassSubjectID_StudentID_LessonID(exam.ClassSubjectID, exam.StudentID, exam.LessonID);

            _ = _chapterProgressService.UpdatePoint(lessonProgress);
            _ = _classSubjectProgressService.UpdatePoint(lessonProgress);
            _ = _classProgressService.UpdatePoint(lessonProgress);

            CreateOrUpdate(exam);

            return exam;
        }

        public Task RemoveClassExam(string ClassID)
        {
            Collection.DeleteManyAsync(t => t.ClassID == ClassID);
            _examDetailService.Collection.DeleteManyAsync(t => t.ClassID == ClassID);
            return Task.CompletedTask;
        }

        public Task RemoveClassStudentExam(string ClassID, string StudentID)
        {
            Collection.DeleteManyAsync(t => t.ClassID == ClassID && t.StudentID == StudentID);
            _examDetailService.Collection.DeleteManyAsync(t => t.ClassID == ClassID && t.StudentID == StudentID);
            return Task.CompletedTask;
        }

        public Task RemoveClassSubjectExam(string ClassSubjectID)
        {
            Collection.DeleteManyAsync(t => t.ClassSubjectID == ClassSubjectID);
            _examDetailService.Collection.DeleteManyAsync(t => t.ClassSubjectID == ClassSubjectID);
            return Task.CompletedTask;
        }

        public async Task ConvertClassSubject(ClassSubjectEntity classSubject)
        {
            await Collection.UpdateManyAsync(t => t.ClassID == classSubject.ClassID, Builders<ExamEntity>.Update.Set("ClassSubjectID", classSubject.ID));
        }
    }
}
