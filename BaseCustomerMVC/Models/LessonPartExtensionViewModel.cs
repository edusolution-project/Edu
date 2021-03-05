using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class LessonPartExtensionViewModel : LessonPartExtensionEntity
    {
        [JsonProperty("TagsName")]
        public String TagsName { get; set; }
        public LessonPartExtensionViewModel()
        {

        }

        public LessonPartExtensionViewModel(LessonPartExtensionEntity o)
        {
            this.Created = o.Created;
            this.Description = o.Description;
            this.ID = o.ID;
            this.Order = o.Order;
            this.ParentID = o.ParentID;
            this.Point = o.Point;
            this.Timer = o.Timer;
            this.Title = o.Title;
            this.Type = o.Type;
            this.Updated = o.Updated;
            this.Media = o.Media;
            this.Questions = new List<QuestionViewModel>();
            this.Tags = o.Tags;
            this.LevelPart = o.LevelPart;
            this.TypePart = o.TypePart;
        }

        [JsonProperty("Questions")]
        public List<QuestionViewModel> Questions { get; set; }

        public LessonPartExtensionEntity ToEntity()
        {
            return new LessonPartExtensionEntity
            {
                OriginID = this.OriginID,
                Created = this.Created,
                Description = this.Description,
                ID = this.ID,
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

    public class QuestionExtensionViewModel : CloneLessonPartQuestionExtensionEntity
    {


        [JsonProperty("Answers")]
        public List<CloneLessonPartAnswerExtensionEntity> Answers { get; set; }

        [JsonProperty("CloneAnswers")]
        public List<CloneLessonPartAnswerExtensionEntity> CloneAnswers { get; set; }

        [JsonProperty("Medias")]
        public List<Media> Medias { get; set; }

        [JsonProperty("MediasAnswer")]
        public List<Media> MediasAnswer { get; set; }
        [JsonProperty("AnswerEssay")]
        public string AnswerEssay { get; set; }

        [JsonProperty("RealAnswerEssay")]
        public string RealAnswerEssay { get; set; }

        [JsonProperty("TypeAnswer")]
        public string TypeAnswer { get; set; }

        [JsonProperty("ExamDetailID")]
        public string ExamDetailID { get; set; }

        [JsonProperty("PointEssay")]
        public double PointEssay { get; set; }

        [JsonProperty("MaxPoint")]
        public double MaxPoint { get; set; }


        public QuestionExtensionViewModel()
        {

        }

        public QuestionExtensionViewModel(CloneLessonPartQuestionExtensionEntity o)
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
            Answers = new List<CloneLessonPartAnswerExtensionEntity>();
        }

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
