using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class StudentRankingViewModel
    {
        public int Rank { get; set; }
        public string StudentID { get; set; }
        public double AvgPoint { get; set; }
        public double TotalPoint { get; set; }
        public long ExamDone { get; set; }
        public int Count { get; set; }
        public double PracticePoint { get; set; }
        public double RankPoint { get; set; }
    }
}
