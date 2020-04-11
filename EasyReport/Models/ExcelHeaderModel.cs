using System;
using System.Collections.Generic;
using System.Text;

namespace EasyReport.Models
{
    public class ExcelHeaderModel
    {
        public MergeModel Merge { get; set; }
        public string Field { get; set; } // tên trường trong table
        public string Value { get; set; } // text , 1 2 3 hoặc "xin chào"
    }
}
