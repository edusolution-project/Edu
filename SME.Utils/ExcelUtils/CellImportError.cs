using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.Utils.ExcelUtils
{
    public class CellImportError
    {
        public int Column { get; set; }
        public int Row { get; set; }
        public int Sheet { get; set; }
        public string Error { get; set; }
    }
}
