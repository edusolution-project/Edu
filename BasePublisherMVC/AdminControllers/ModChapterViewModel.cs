using BasePublisherModels.Database;

namespace BasePublisherMVC.AdminControllers
{
    internal class ModChapterViewModel
    {
        private ModChapterEntity o;

        public ModChapterViewModel(ModChapterEntity o)
        {
            this.o = o;
        }

        public ModChapterEntity Parent { get; set; }
        public object Order { get; internal set; }
    }
}