using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using BasePublisherModels.Database;

namespace BasePublisherMVC.ViewModel
{
    public class ModGradeViewModel : ModGradeEntity
    {
        public ModGradeEntity Parent { get; set; }
        public long SubGradeCount { get; set; }

        public ModGradeViewModel(ModGradeEntity entity)
        {
            ID = entity.ID;
            ParentID = entity.ParentID;
            IsActive = entity.IsActive;
            Code = entity.Code;
            CreateUser = entity.CreateUser;
            Name = entity.Name;
            SubGradeCount = 0;
            Parent = null;
            Created = entity.Created;
            Updated = entity.Updated;
        }

    }
}
