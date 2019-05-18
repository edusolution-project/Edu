using BasePublisherModels.Database;

namespace BasePublisherMVC.Models
{
    public class ModCourseViewModel: ModCourseEntity
    {
        public ModProgramEntity Program { get; set; }
        public ModGradeEntity Grade { get; set; }
        public ModSubjectEntity Subject { get; set; }
        public int ChildNodeCount { get; set; }

        public ModCourseViewModel(ModCourseEntity entity)
        {
            ID = entity.ID;
            IsActive = entity.IsActive;
            Code = entity.Code;
            CreateUser = entity.CreateUser;
            Name = entity.Name;
            ChildNodeCount = 0;
            ProgramID = entity.ProgramID;
            SubjectID = entity.SubjectID;
            GradeID = entity.GradeID;
            Created = entity.Created;
            Updated = entity.Updated;
            Description = entity.Description;
        }
    }
}