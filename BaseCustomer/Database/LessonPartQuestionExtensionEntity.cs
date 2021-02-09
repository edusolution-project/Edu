using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class LessonPartQuestionExtensionEntity : LessonPartQuestionEntity
    {
        public LessonPartQuestionExtensionEntity()
        {

        }
        public LessonPartQuestionExtensionEntity(LessonPartQuestionEntity o)
        {
            OriginID = o.OriginID;
            Content = o.Content;
            CreateUser = o.CreateUser;
            Created = DateTime.Now;
            Updated = DateTime.Now;
            Point = o.Point;
            Order = o.Order;
            Description = o.Description;
            Media = new Media();
            ParentID = o.ParentID;
            CourseID = o.CourseID;
        }
        //[JsonProperty("TypeQuestion")]
        //public Int32 TypeQuestion { get; set; }
        //[JsonProperty("ExamQuestionArchiveID")]
        //public String ExamQuestionArchiveID { get; set; }
    }

    public class TYPEQUESTION
    {
        public const int EASAY = 1, //DỄ
            NORMAL = 2, //TRUNG BÌNH
            HARD = 3; //KHÓ
    }

    public class LessonPartQuestionExtensionServie : ServiceBase<LessonPartQuestionExtensionEntity>
    {
        public LessonPartQuestionExtensionServie(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<LessonPartQuestionExtensionEntity>>
            {
                //ClassID_1
                new CreateIndexModel<LessonPartQuestionExtensionEntity>(
                    new IndexKeysDefinitionBuilder<LessonPartQuestionExtensionEntity>()
                    .Ascending(t => t.ID)
                    .Ascending(t=>t.ParentID)
                    )
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public IEnumerable<LessonPartQuestionExtensionEntity> GetItemByType(int typeQuestion)
        {
            //return CreateQuery().Find(x => x.TypeQuestion == typeQuestion).ToEnumerable();
            return null;
        }

        public IEnumerable<LessonPartQuestionExtensionEntity> GetItemByPartID(string LessonPartID)
        {
            return CreateQuery().Find(x => x.ParentID == LessonPartID).ToEnumerable();
        }

        public IEnumerable<String> GetIDsByPartID(string LessonPartID)
        {
            return CreateQuery().Find(x => x.ParentID == LessonPartID).Project(x=>x.ID).ToEnumerable();
        }
    }
}
