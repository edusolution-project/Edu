using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class LessonPartViewModel : LessonPartEntity
    {

        public LessonPartViewModel(LessonPartEntity o)
        {
            this.Created = o.Created;
            this.Description = o.Description;
            this.ID = o.ID;
            this.IsExam = o.IsExam;
            this.Order = o.Order;
            this.ParentID = o.ParentID;
            this.Point = o.Point;
            this.Timer = o.Timer;
            this.Title = o.Title;
            this.Type = o.Type;
            this.Updated = o.Updated;
            this.Media = o.Media;
            this.Questions = new List<QuestionViewModel>();
        }
        [JsonProperty("Questions")]
        public List<QuestionViewModel> Questions { get; set; }

    }

    public class QuestionViewModel : LessonPartQuestionEntity
    {

        public QuestionViewModel()
        {
        }

        public QuestionViewModel(LessonPartQuestionEntity o)
        {
            this.Created = o.Created;
            this.Description = o.Description;
            this.ID = o.ID;
            this.Updated = o.Updated;
            this.Content = o.Content;
            this.CreateUser = o.CreateUser;
            this.ParentID = o.ParentID;
            this.Order = o.Order;
            this.Point = o.Point;
            this.Media = o.Media;
            Answers = new List<LessonPartAnswerEntity>();
        }

        [JsonProperty("Answers")]
        public List<LessonPartAnswerEntity> Answers { get; set; }

        [JsonProperty("CloneAnswers")]
        public List<CloneLessonPartAnswerEntity> CloneAnswers { get; set; }

    }
}
