using System;
using System.Collections.Generic;
using System.Text;

namespace EasyReport.Models
{
    public class MergeModel
    {
        public MergeModel(int firstRow, int lastRow, int firstCol, int lastCol)
        {
            FirstRow = firstRow;
            LastRow = lastRow;
            FirstCol = firstCol;
            LastCol = lastCol;
        }

        public int FirstRow { get; set; }
       public int LastRow { get; set; }
       public int FirstCol { get; set; }
       public int LastCol { get; set; }
    }
}
