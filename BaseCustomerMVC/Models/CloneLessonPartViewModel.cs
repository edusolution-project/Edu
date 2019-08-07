using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class CloneLessonPartViewModel : CloneLessonPartEntity
    {
        public CloneLessonPartViewModel() { }

        public CloneLessonPartViewModel(CloneLessonPartEntity o)
        {
            this.TeacherID = o.TeacherID;
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
            this.Questions = new List<CloneQuestionViewModel>();
        }
        [JsonProperty("Questions")]
        public List<CloneQuestionViewModel> Questions { get; set; }
        public CloneLessonPartEntity ToEntity()
        {
            return new CloneLessonPartEntity
            {
                ClassID = this.ClassID,
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
                Media = this.Media,
                TeacherID = this.TeacherID
                
            };
        }
    }

    public class CloneQuestionViewModel : CloneLessonPartQuestionEntity
    {

        public CloneQuestionViewModel()
        {
        }

        public CloneQuestionViewModel(LessonPartQuestionEntity o)
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
            Answers = new List<CloneLessonPartAnswerEntity>();
        }

        [JsonProperty("Answers")]
        public List<CloneLessonPartAnswerEntity> Answers { get; set; }

        public CloneLessonPartQuestionEntity ToEntity()
        {
            return new CloneLessonPartQuestionEntity
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
                TeacherID = this.TeacherID,
                ClassID = this.ClassID,
            };
        }
    }
}
