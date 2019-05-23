using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SME.Utils.ExcelUtils
{
    public interface IXVTRange
    {
        ClosedXML.Excel.IXLRange Range { get; set; }
        /// <summary>
        /// Set value of range
        /// WriteOnly
        /// </summary>
        /// <author>hungnd8</author>
        object Value { set; }

        /// <summary>
        /// Set formula
        /// WriteOnly
        /// </summary>
        /// <author>hungnd8</author>
        string Formula { set; }

        /// <summary>
        /// Get Count of rows in range
        /// ReadOnly
        /// </summary>
        /// <author>hungnd8</author>
        int RowsCount { get; }

        /// <summary>
        /// Get Count of columns in range
        /// ReadOnly
        /// </summary>
        /// <author>hungnd8</author>
        int ColumnsCount { get; }

        /// <summary>
        /// Set boder for rande
        /// </summary>
        /// <param name="borderStyle"></param>
        /// <param name="borderIndex"></param>
        /// <author>hungnd8</author>
        void SetBorder(XLVTBorderStyleValues borderStyle, XVTBorderIndex borderIndex = XVTBorderIndex.OutsideBorder);
    }
}
