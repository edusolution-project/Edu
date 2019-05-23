using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SME.Utils.ExcelUtils
{
    public class XVTWorksheet : IXVTWorksheet
    {
        #region Declare Variable
        public ClosedXML.Excel.IXLWorksheet Worksheet { get; set; }
        /// <summary>
        /// Declate Worksheet
        /// </summary>
        /// <param name="worksheet"></param>
        public XVTWorksheet(ClosedXML.Excel.IXLWorksheet worksheet)
        {
            Worksheet = worksheet;
        }

        /// <summary>
        /// Declate Worksheet
        /// </summary>
        public XVTWorksheet()
        {
        }
        #endregion

        #region Method of Object
        /// <summary>
        /// Get Range by first address and last Address
        /// </summary>
        /// <param name="firstCellAddress"></param>
        /// <param name="lastCellAddress"></param>
        /// <returns></returns>
        public IXVTRange GetRange(string firstCellAddress, string lastCellAddress)
        {
            return new XVTRange(Worksheet.Range(firstCellAddress, lastCellAddress));
        }

        /// <summary>
        /// Get range by ColNum and RowNum
        /// </summary>
        /// <param name="firstRow"></param>
        /// <param name="firstCoumn"></param>
        /// <param name="lastRow"></param>
        /// <param name="lastColumn"></param>
        /// <returns></returns>
        public IXVTRange GetRange(int firstRow, int firstCoumn, int lastRow, int lastColumn)
        {
            return new XVTRange(Worksheet.Range(firstRow, firstCoumn, lastRow, lastColumn));
        }

        /// <summary>
        /// Get Range by Address
        /// </summary>
        /// <param name="CellAddress"></param>
        /// <returns></returns>
        public IXVTRange GetRange(string CellAddress)
        {
            return new XVTRange(Worksheet.Range(CellAddress));
        }

        /// <summary>
        /// Set Cell Value by Address
        /// </summary>
        /// <param name="CellAddress"></param>
        /// <param name="value"></param>
        public void SetCellValue(string CellAddress, string value)
        {
            IXVTRange XCell = new XVTRange(Worksheet.Range(CellAddress));
            XCell.Value = value;
        }

        /// <summary>
        /// Set Cell Value
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public void SetCellValue(int row, int column, string value)
        {
            IXVTRange XCell = new XVTRange(Worksheet.Range(row, column, row, column));
            XCell.Value = value;
        }

        /// <summary>
        /// Delete defined Sheet
        /// </summary>
        public void Delete()
        {
            Worksheet.Delete();
        }

        /// <summary>
        /// Copy một khoảng dữ liệu (tập các Cell) sang một khoảng khác trong sheet (Độ cao của các Row sẽ không được Copy)
        /// </summary>
        /// <param name="range">Khoảng dữ liệu (tập các Cell)</param>
        /// <param name="firstRow">Vị trí hàng đầu tiên của khoảng sẽ paste dữ liệu</param>
        /// <param name="firstColumn">Vị trí cột đầu tiên của khoảng sẽ paste dữ liệu</param>
        public void CopyPaste(IXVTRange xRange, int firstRow, int firstColumn, bool copyStyleOnly = false)
        {
            ClosedXML.Excel.IXLRange TempRange = Worksheet.Range(firstRow, firstColumn, firstRow + xRange.RowsCount - 1,
                firstColumn + xRange.ColumnsCount - 1);
        }

        /// <summary>
        /// Copy Anything of row to target row
        /// </summary>
        /// <param name="PoisionTempRow"></param>
        /// <param name="firstRow"></param>
        /// <param name="lastRow"></param>
        public void CopyRow(int PoisionTempRow, int firstRow, int lastRow)
        {
            ClosedXML.Excel.IXLRow TmpRow = Worksheet.Row(PoisionTempRow);
            ClosedXML.Excel.IXLRow TargetRow;
            for (int i = firstRow; i <= lastRow; i++)
            {
                TargetRow = Worksheet.Row(i);
                TmpRow.CopyTo(TargetRow);
            }
        }

        /// <summary>
        /// Copy Sheet
        /// </summary>
        /// <param name="SheetName"></param>
        public void CopyTo(string SheetName)
        {
                Worksheet.CopyTo(SheetName);
        }
        /// <summary>
        /// Copy Sheet
        /// </summary>
        /// <param name="SheetName"></param>
        public void CopyTo(string SheetName, int Position)
        {

            Worksheet.CopyTo(SheetName, Position);
        }

        /// <summary>
        /// Hide Row
        /// </summary>
        /// <param name="Row"></param>
        public void HideRow(int Row)
        {
            Worksheet.Row(Row).Hide();
        }

        /// <summary>
        /// Delete Row
        /// </summary>
        /// <param name="Row"></param>
        public void DeleteRow(int Row)
        {
            Worksheet.Row(Row).Delete();
        }

        public object GetValueOfCell(string Address)
        {
            return Worksheet.Cell(Address).Value;
        }
        public object GetNonFormatValueOfCell(int Row, int Col)
        {
            return Worksheet.Cell(Row, Col).GetFormattedString();
        }
        public object GetValueOfCell(int Row, int Col)
        {
            return Worksheet.Cell(Row, Col).Value;
        }
        public void SetValueOfCell(string Address, object value)
        {
            Worksheet.Cell(Address).Value = value;
        }
        public void SetValueOfCell(int Row, int Col, object value)
        {
            Worksheet.Cell(Row, Col).Value = value;
        }

        public void AddCellComment(int Row, int Col, string value)
        {
            Worksheet.Cell(Row, Col).Style.Font.FontColor = XLColor.Red;
            if(Worksheet.Cell(Row, Col).Comment.Count > 0) { 
                Worksheet.Cell(Row, Col).Comment.AddNewLine();
            }
            Worksheet.Cell(Row, Col).Comment.AddText(value);
        }

        public void AddCellBackgroundColor(int Row, int Col, string value)
        {
            Worksheet.Cell(Row, Col).Style.Font.FontColor = XLColor.Red;
            if (Worksheet.Cell(Row, Col).Comment.Count > 0)
            {
                Worksheet.Cell(Row, Col).Comment.AddNewLine();
            }
            Worksheet.Cell(Row, Col).Comment.AddText(value);
        }

        public void SetCellFont(int Row, int Col, string fontName, int fontSize, bool bold, bool italic)
        {
            Worksheet.Cell(Row, Col).Style.Font.FontName = fontName;
            Worksheet.Cell(Row, Col).Style.Font.FontSize = fontSize;
            Worksheet.Cell(Row, Col).Style.Font.SetBold(bold);
            Worksheet.Cell(Row, Col).Style.Font.SetItalic(italic);
        }

        public void SetRangeFont(int FirstRow, int FirstCol, int LastRow, int LastCol, string fontName, int fontSize, bool bold, bool italic)
        {
            GetRange(FirstRow, FirstCol, LastRow, LastCol).Range.Style.Font.FontName = fontName;
            GetRange(FirstRow, FirstCol, LastRow, LastCol).Range.Style.Font.FontSize = fontSize;
            GetRange(FirstRow, FirstCol, LastRow, LastCol).Range.Style.Font.SetBold(bold);
            GetRange(FirstRow, FirstCol, LastRow, LastCol).Range.Style.Font.SetItalic(italic);
        }

        public void SetCellBorderThin(int Row, int Col)
        {
            Worksheet.Cell(Row, Col).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            Worksheet.Cell(Row, Col).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            Worksheet.Cell(Row, Col).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            Worksheet.Cell(Row, Col).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        }

        public void SetRangeBorderThin(int FirstRow, int FirstCol, int LastRow, int LastCol)
        {
            GetRange(FirstRow, FirstCol, LastRow, LastCol).Range.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            GetRange(FirstRow, FirstCol, LastRow, LastCol).Range.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            GetRange(FirstRow, FirstCol, LastRow, LastCol).Range.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            GetRange(FirstRow, FirstCol, LastRow, LastCol).Range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        }

        public void SetCellHorizontal(int Row, int Col, XLAlignmentHorizontalValues Align)
        {
            Worksheet.Cell(Row, Col).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        }
        public void SetCellVertical(int Row, int Col, XLAlignmentVerticalValues Valign)
        {
            Worksheet.Cell(Row, Col).Style.Alignment.SetVertical(Valign);
        }

        public void AddColumns(string cellAddress, int numberColumn)
        {
            GetRange(cellAddress).Range.InsertColumnsAfter(numberColumn);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get or Set Sheet Name
        /// </summary>
        public string SheetName
        {
            get
            {
                return Worksheet.Name;
            }
            set
            {
                Worksheet.Name = value;
            }
        }

        #endregion
    }
}
