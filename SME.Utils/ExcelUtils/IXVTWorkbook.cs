using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SME.Utils.ExcelUtils
{
    public interface IXVTWorkbook
    {
        /// <summary>
        /// Get Worksheet by index
        /// Sheet đầu tiên có index = 1
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <author>hungnd8</author>
        IXVTWorksheet GetSheet(int index);

        /// <summary>
        /// Copy Sheet to Next Position
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="SheetName"></param>
        /// <returns></returns>
        /// <author>hungnd8</author>
        IXVTWorksheet CopySheet(IXVTWorksheet worksheet, string SheetName = "New Scheet");
        IXVTWorksheet CreateSheet(string SheetName = "New Scheet");
        /// <summary>
        /// Convert Workbook to stream
        /// </summary>
        /// <returns></returns>
        /// <author>hungnd8</author>
        Stream ToStream();

        /// <summary>
        /// Save Workbook to file
        /// </summary>
        /// <returns>Stream</returns>
        /// <author>phuongh1</author>
        void SaveToFile(string path);

        List<XVTWorksheet> GetWorksheets();
        decimal? GetDecimal(int sheetIdx, string cellAddress);
        void SetCellValue(int sheetIdx, string cellAddress, object value);
    }
}
