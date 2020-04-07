using System;
using System.Collections.Generic;
using System.Text;

namespace EasyReport.Models
{
    public class ExcelHeaderModel
    {
        public MergeModel Merge { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
    }
}
