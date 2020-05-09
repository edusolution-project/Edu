using System;

namespace BaseCustomerMVC.Models
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

        public int PageSize { get; set; } = 20;

        public long TotalRecord { get; set; }

        public string LangID { get; set; }
        public string LangCode { get; set; }

        public string Sort { get; set; }

        public string Record { get; set; }
        public string ID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string ArrID { get; set; }

        public bool Status { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
