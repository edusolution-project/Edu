using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClosedXML.Excel;
using System.IO;
using SME.Utils.ExcelUtils;
using SME.Utils.Common.SMEException;

namespace SME.Utils.ExcelUtils
{
    public class XVTWorkbook : IXVTWorkbook
    {
        #region Declare Variable
        public ClosedXML.Excel.XLWorkbook Workbook { get; set; }

        /// <summary>
        /// Declare Workbook
        /// </summary>
        /// <param name="workbook"></param>
        public XVTWorkbook(ClosedXML.Excel.XLWorkbook workbook)
        {
            Workbook = workbook;
        }
        #endregion

        #region Method of Object

        /// <summary>
        /// Get Worksheet by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IXVTWorksheet GetSheet(int index)
        {
            return new XVTWorksheet(Workbook.Worksheet(index));
        }

        /// <summary>
        /// Copy Sheet to Next Position
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="SheetName"></param>
        /// <returns></returns>
        public IXVTWorksheet CopySheet(IXVTWorksheet worksheet, string SheetName = "New Scheet")
        {
            // Check neu sheet name da ton tai them so vao phia sau
            int i = 0;
            while (Workbook.Worksheets.Any(a => a.Name == SheetName))
            {
                SheetName = SheetName + i;
                i++;
            }
            worksheet.CopyTo(SheetName);
            return new XVTWorksheet(Workbook.Worksheets.Last());
        }

        public IXVTWorksheet CreateSheet(string SheetName = "New Scheet")
        {

            int i = 0;
            string newName = SheetName;
            while (Workbook.Worksheets.Any(a => a.Name == newName))
            {
                newName = SheetName + i;
                i++;
            }
            return new XVTWorksheet(Workbook.Worksheets.Add(newName));
        }

        /// <summary>
        /// Convert Workbook to stream
        /// </summary>
        /// <returns>Stream</returns>
        public Stream ToStream()
        {
            MemoryStream OutPut = new MemoryStream();
            Workbook.SaveAs(OutPut);
            // return the filestream
            // Rewind the memory stream to the beginning
            OutPut.Seek(0, SeekOrigin.Begin);
            return OutPut;
        }

        /// <summary>
        /// Save Workbook to file
        /// </summary>
        /// <returns>Stream</returns>
        public void SaveToFile(string path)
        {
            Workbook.SaveAs(path);
        }

        public decimal? GetDecimal(int sheetIdx, string cellAddress)
        {
            if (this.GetWorksheets().Count < sheetIdx)
            {
                throw new BusinessException(string.Format("Sheet {0} Cell {1} - không được để trống", sheetIdx, cellAddress));
            }
            IXLWorksheet wsheet = this.Workbook.Worksheet(sheetIdx);
            IXLCell cell = wsheet.Cell(cellAddress);
            if (cell == null)
            {
                throw new BusinessException(string.Format("Sheet {0} Cell {1} - không được để trống", sheetIdx, cellAddress));
            }
            object value = cell.Value;
            decimal result;
            if (Decimal.TryParse(value.ToString(), out result))
            {
                return result;
            }
            else
            {
                throw new BusinessException(string.Format("Sheet {0} Cell {1} - giá trị không phải là số hoặc số quá nhỏ hoặc lớn (chỉ được trong khoảng {2} : {3})",
                    sheetIdx, cellAddress, Decimal.MinValue, Decimal.MaxValue));
            }
        }

        public void SetCellValue(int sheetIdx, string cellAddress, object value)
        {
            if (this.GetWorksheets().Count < sheetIdx)
            {
                throw new BusinessException(string.Format("Sheet {0} không tồn tại", sheetIdx));
            }
            IXLWorksheet wsheet = this.Workbook.Worksheet(sheetIdx);
            IXLCell cell = wsheet.Cell(cellAddress);
            cell.Value = value;
        }
        #endregion

        #region Properties Of Object

        public List<XVTWorksheet> GetWorksheets() { 
                return Workbook.Worksheets.Select(x => new XVTWorksheet(x)).ToList();
        }
        #endregion
    }
}
