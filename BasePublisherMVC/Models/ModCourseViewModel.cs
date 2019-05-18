using BasePublisherModels.Database;

namespace BasePublisherMVC.AdminControllers
{
    internal class ModCourseViewModel
    {
        private ModCourseEntity t;

        public ModCourseViewModel(ModCourseEntity t)
        {
            this.t = t;
        }

        public ModProgramEntity Program { get; set; }
        public ModGradeEntity Grade { get; set; }
        public ModSubjectEntity Subject { get; set; }
        public int ChildNodeCount { get; set; }
    }
}