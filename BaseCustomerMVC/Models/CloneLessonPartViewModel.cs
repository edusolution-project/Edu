using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class CloneLessonPartViewModel : LessonPartEntity
    {

        public CloneLessonPartViewModel(LessonPartEntity o)
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
            this.Questions = new List<CloneQuestionViewModel>();
        }
        [JsonProperty("Questions")]
        public List<CloneQuestionViewModel> Questions { get; set; }
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
              
    }
}
