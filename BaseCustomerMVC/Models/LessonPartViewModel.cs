using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class LessonPartViewModel : LessonPartEntity
    {
        public LessonPartViewModel()
        { }

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

        public LessonPartEntity ToEntity()
        {
            return new LessonPartEntity
            {
                OriginID = this.OriginID,
                Created = this.Created,
                Description = this.Description,
                ID = this.ID,
                IsExam = this.IsExam,
                Order = this.Order,
                ParentID = this.ParentID,
                Point = this.Point,
                Timer = this.Timer,
                Title = this.Title,
                Type = this.Type,
                Updated = this.Updated,
                Media = this.Media
            };
        }

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

        public LessonPartQuestionEntity ToEntity()
        {
            return new LessonPartQuestionEntity
            {
                Created = this.Created,
                Description = this.Description,
                ID = this.ID,
                Updated = this.Updated,
                Content = this.Content,
                CreateUser = this.CreateUser,
                ParentID = this.ParentID,
                Order = this.Order,
                Point = this.Point,
                Media = this.Media,
                OriginID = this.OriginID,
                CourseID = this.CourseID
            };
        }

    }
}
