using System;

namespace BasePublisherMVC.Models
{
    public class DefaultModel
    {
        public string Command { get; set; }
        public string SearchText { get; set; }

        private int _pageIndex;

        public int PageIndex
        {
            get { return _pageIndex; }
            set { _pageIndex = value - 1; }
        }

        public int PageSize { get; set; } = 50;

        public int TotalRecord { get; set; }

        public string LangID { get; set; }
        public string LangCode { get; set; }

        public string Sort { get; set; }

        public string Record { get; set; }
        public string ID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string ArrID { get; set; }
    }

    public class ModProgramModel : DefaultModel
    {
        public string Subject { get; set; }
        public string Grade { get; set; }
    }

    public class ModCourseModel : DefaultModel
    {
        public string Program { get; set; }
        public string Subject { get; set; }
        public string Grade { get; set; }
    }

    public class ModChapterModel : DefaultModel
    {
        public string CourseID { get; set; }
        public string ParentID { get; set; }
    }
}
