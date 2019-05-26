using BasePublisherModels.Database;

namespace BasePublisherMVC.AdminControllers
{
    internal class ModCourseViewModel
    {
        private ModCourseEntity item;

        public ModCourseViewModel(ModCourseEntity item)
        {
            this.item = item;
        }

        public ModProgramEntity Program { get; set; }
        public ModGradeEntity Grade { get; set; }
        public ModSubjectEntity Subject { get; set; }
        public int ChildNodeCount { get; internal set; }
    }
}