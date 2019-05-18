using BasePublisherModels.Database;

namespace BasePublisherMVC.Models
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