using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SME.Utils.ExcelUtils
{
    public interface IXVTWorksheet
    {
        ClosedXML.Excel.IXLWorksheet Worksheet { get; set; }
        /// <summary>
        /// Get Range by first address and last Address
        /// </summary>
        /// <param name="firstCellAddress"></param>
        /// <param name="lastCellAddress"></param>
        /// <returns></returns>
        /// <author>hungnd8</author>
        IXVTRange GetRange(string firstCellAddress, string lastCellAddress);

        /// <summary>
        /// Get range by ColNum and RowNum
        /// </summary>
        /// <param name="firstRow"></param>
        /// <param name="firstCoumn"></param>
        /// <param name="lastRow"></param>
        /// <param name="lastColumn"></param>
        /// <returns></returns>
        /// <author>hungnd8</author>
        IXVTRange GetRange(int firstRow, int firstCoumn, int lastRow, int lastColumn);

        /// <summary>
        /// Get Range by Address
        /// </summary>
        /// <param name="CellAddress"></param>
        /// <returns></returns>
        /// <author>hungnd8</author>
        IXVTRange GetRange(string CellAddress);

        /// <summary>
        /// Set Cell Value by Address
        /// </summary>
        /// <param name="CellAddress"></param>
        /// <param name="value"></param>
        /// <author>hungnd8</author>
        void SetCellValue(string CellAddress, string value);

        /// <summary>
        /// set cell value
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <author>hungnd8</author>
        void SetCellValue(int row, int column, string value);

        /// <summary>
        /// Delete defined Sheet
        /// </summary>
        /// <author>hungnd8</author>
        void Delete();

        /// <summary>
        /// Get or Set Sheet Name
        /// </summary>
        /// <author>hungnd8</author>
        string SheetName { get; set; }

        /// <summary>
        /// copy Sheet
        /// </summary>
        /// <param name="SheetName"></param>
        /// <author>hungnd8</author>
        void CopyTo(string SheetName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SheetName"></param>
        /// <param name="Position"></param>
        /// <author>hungnd8</author>
        void CopyTo(string SheetName, int Position);
        /// <summary>
        /// Copy Anything of row to target row
        /// </summary>
        /// <param name="PoisionTempRow"></param>
        /// <param name="firstRow"></param>
        /// <param name="lastRow"></param>
        /// <author>hungnd8</author>
        void CopyRow(int PoisionTempRow, int firstRow, int lastRow);

        /// <summary>
        /// An dong
        /// </summary>
        /// <param name="Row"></param>
        /// <author>hungnd8</author>
        void HideRow(int Row);

        /// <summary>
        /// Xoa Dong
        /// </summary>
        /// <param name="Row"></param>
        /// <author>hungnd8</author>
        void DeleteRow(int Row);

        /// <summary>
        /// lay len gia tri cua cell thong qua dia chi cua cell
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        /// <author>hungnd8</author>
        object GetValueOfCell(string Address);

        /// <summary>
        /// lay len gia tri cua cell khong convert dinh dang theo cot va dong
        /// </summary>
        /// <param name="Row"></param>
        /// <param name="Col"></param>
        /// <returns></returns>
        object GetNonFormatValueOfCell(int Row, int Col);
        /// <summary>
        /// lay len gia tri cua cell thong qua thu tu cua dong va cot
        /// </summary>
        /// <param name="Row"></param>
        /// <param name="Col"></param>
        /// <returns></returns>
        /// <author>hungnd8</author>
        object GetValueOfCell(int Row, int Col);

        /// <summary>
        /// gan gia tri cua cell thong qua dia chi cua cell
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="value"></param>
        /// <author>hungnd8</author>
        void SetValueOfCell(string Address, object value);

        /// <summary>
        /// gan gia tri cua cell thong qua thu tu cua dong va cot
        /// </summary>
        /// <param name="Row"></param>
        /// <param name="Col"></param>
        /// <param name="value"></param>
        /// <author>hungnd8</author>
        void SetValueOfCell(int Row, int Col, object value);

        void AddCellComment(int Row, int Col, string value);

        void SetCellBorderThin(int Row, int Col);
        void SetRangeBorderThin(int FirstRow, int FirstCol, int LastRow, int LastCol);
        void SetCellFont(int Row, int Col, string fontName, int fontSize, bool bold, bool italic);
        void SetRangeFont(int FirstRow, int FirstCol, int LastRow, int LastCol, string fontName, int fontSize, bool bold, bool italic);

        void SetCellHorizontal(int Row, int Col, XLAlignmentHorizontalValues Align);
        void SetCellVertical(int Row, int Col, XLAlignmentVerticalValues Valign);
        void AddColumns(string cellAddress, int numberColumn);
    }
}
