using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using BasePublisherModels.Database;

namespace BasePublisherMVC.ViewModel
{
    public class ModChapterViewModel : ModChapterEntity
    {
        public ModChapterEntity Parent { get; set; }

        public ModChapterViewModel(ModChapterEntity entity)
        {
            ID = entity.ID;
            ParentID = entity.ParentID;
            IsActive = entity.IsActive;
            Code = entity.Code;
            CreateUser = entity.CreateUser;
            Name = entity.Name;
            Parent = null;
            Created = entity.Created;
            Updated = entity.Updated;
            Order = entity.Order;
            CourseID = entity.CourseID;
        }

    }
}
