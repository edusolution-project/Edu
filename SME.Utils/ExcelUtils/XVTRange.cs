//VIETTEL-IT
//<author>hungnd8</author>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SME.Utils.ExcelUtils
{
    public class XVTRange : IXVTRange
    {
        public ClosedXML.Excel.IXLRange Range { get; set; }

        /// <summary>
        /// Declare Range
        /// </summary>
        /// <param name="range"></param>
        public XVTRange(ClosedXML.Excel.IXLRange range)
        {
            Range = range;
        }

        #region Method of Object
        /// <summary>
        /// Set boder for rande
        /// </summary>
        /// <param name="borderStyle"></param>
        /// <param name="borderIndex"></param>
        public void SetBorder(XLVTBorderStyleValues borderStyle, XVTBorderIndex borderIndex = XVTBorderIndex.OutsideBorder)
        {
            switch (borderIndex)
            {
                case XVTBorderIndex.DiagonalBorder: Range.Style.Border.DiagonalBorder = GetBorderStyle(borderStyle);
                    break;
                case XVTBorderIndex.BottomBorder: Range.Style.Border.BottomBorder = GetBorderStyle(borderStyle);
                    break;
                case XVTBorderIndex.TopBorder: Range.Style.Border.TopBorder = GetBorderStyle(borderStyle);
                    break;
                case XVTBorderIndex.RightBorder: Range.Style.Border.RightBorder = GetBorderStyle(borderStyle);
                    break;
                case XVTBorderIndex.LeftBorder: Range.Style.Border.LeftBorder = GetBorderStyle(borderStyle);
                    break;
                default: Range.Style.Border.OutsideBorder = GetBorderStyle(borderStyle);
                    break;
            }
        }
        
        #endregion

        #region Properties Of Object

        /// <summary>
        /// Set value of range
        /// WriteOnly
        /// </summary>
        public object Value
        {
            set
            {
                Range.Value = value;
            }
        }

        /// <summary>
        /// Set formula
        /// WriteOnly
        /// </summary>
        public string Formula
        {
            set
            {
                Range.FormulaA1 = value;
            }
        }

        /// <summary>
        /// Get Count of rows in range
        /// ReadOnly
        /// </summary>
        public int RowsCount
        {
            get
            {
                return Range.RowCount();
            }
        }

        /// <summary>
        /// Get Count of columns in range
        /// ReadOnly
        /// </summary>
        public int ColumnsCount
        {
            get
            {
                return Range.ColumnCount();
            }
        }

        #endregion

        #region Private Function
        private ClosedXML.Excel.XLBorderStyleValues GetBorderStyle(XLVTBorderStyleValues borderStyle)
        {
            ClosedXML.Excel.XLBorderStyleValues lineStyle;
            switch (borderStyle)
            {
                case XLVTBorderStyleValues.Dashed: lineStyle = ClosedXML.Excel.XLBorderStyleValues.Dashed;
                    break;
                case XLVTBorderStyleValues.Dotted: lineStyle = ClosedXML.Excel.XLBorderStyleValues.Dotted;
                    break;
                case XLVTBorderStyleValues.Double: lineStyle = ClosedXML.Excel.XLBorderStyleValues.Double;
                    break;
                case XLVTBorderStyleValues.Medium: lineStyle = ClosedXML.Excel.XLBorderStyleValues.Medium;
                    break;
                case XLVTBorderStyleValues.Thick: lineStyle = ClosedXML.Excel.XLBorderStyleValues.Thick;
                    break;
                default: lineStyle = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    break;
            }
            return lineStyle;
        }
        #endregion
    }
}
