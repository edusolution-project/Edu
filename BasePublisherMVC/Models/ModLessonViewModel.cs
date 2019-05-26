using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using BasePublisherModels.Database;

namespace BasePublisherMVC.Models
{
    public class ModLessonViewModel : ModLessonEntity
    {
        public ModChapterEntity Parent { get; set; }

        public ModLessonViewModel(ModLessonEntity entity)
        {
            ID = entity.ID;
            IsActive = entity.IsActive;
            Code = entity.Code;
            CreateUser = entity.CreateUser;
            Parent = null;
            Created = entity.Created;
            Updated = entity.Updated;
            CourseID = entity.CourseID;
        }

    }
}
