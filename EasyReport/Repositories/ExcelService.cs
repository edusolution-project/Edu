using Core_v2.Interfaces;
using Core_v2.Repositories;
using EasyReport.Models;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyReport.Repositories
{
    public class ExcelService
    {
        private readonly ILog _log;
        public ExcelService()
        {
            _log = new Log();
        }
        private void export(string sheetName)
        {
            // tạo mới 1 workbook
            XSSFWorkbook workbook = new XSSFWorkbook();
            // tạo sheet
            ISheet sheet = workbook.CreateSheet(sheetName);

        }

        public string Export<T>(string sheetName, string fileName, List<ExcelHeaderModel> header, List<T> data, int limit)
        {
            try
            {
                XSSFWorkbook workbook = new XSSFWorkbook();
                // tạo sheet
                ISheet sheet = workbook.CreateSheet(sheetName);
                int start = CreateHeader(sheet, header);
                int count = data.Count;
                if (count > 0)
                {
                    if (count > limit)
                    {
                        List<Task> tasks = new List<Task>();
                        int x = count % limit;
                        int over = x == 0 ? count / limit : (count / limit) + 1;
                        for (int i = 0; i < over; i++)
                        {
                            int overStart = i == 0 ? start : start + (i + limit);
                            Task.Run(()=>ProccessBody<T>(sheet, data, overStart, i == 0)).Wait();
                        }
                        
                    }
                    else
                    {
                        ProccessBody<T>(sheet, data, start,true);
                    }
                }
                if (string.IsNullOrEmpty(fileName)) fileName = string.Format("{0:yyyyMddhhmmss}", DateTime.Now);
                string path = @"E:\" + fileName + ".xlsx";
                FileStream fs = new FileStream(path, FileMode.CreateNew);
                workbook.Write(fs);
                return path;
            }
            catch (Exception ex)
            {
                StackTrace stackTrace = new StackTrace();
                _log.Error(stackTrace.GetFrame(1).GetMethod().Name, ex);
                return string.Empty;
            }
        }

        private int CreateHeader(ISheet sheet, List<ExcelHeaderModel> header)
        {
            IRow row = sheet.CreateRow(0);
            int index = 0;
            int lastRow = 1;
            for(int i = 0; i < header.Count; i++)
            {
                ICell cell = row.CreateCell(index);
                var item = header[i];
                if(item.Merge != null)
                {
                    //int firstRow, int lastRow, int firstCol, int lastCol
                    CellRangeAddress cellMerge = new CellRangeAddress(item.Merge.FirstRow, item.Merge.LastRow, item.Merge.FirstCol, item.Merge.LastCol);
                    sheet.AddMergedRegion(cellMerge);
                    index = item.Merge.LastCol;
                    lastRow = item.Merge.LastRow > lastRow ? item.Merge.LastRow : lastRow ;
                }
                else
                {
                    index++;
                }
                cell.SetCellValue(item.Value);
            }

            return lastRow;
        }

        private Task ProccessBody<T>(ISheet sheet, List<T> data, int start, bool isFirst)
        {
            try
            {
                IList<PropertyInfo> propertyInfos = data[0].GetType().GetProperties().ToList();
                for (int i = isFirst ? 0 : start; i < data.Count; i++)
                {
                    IRow row = sheet.CreateRow(i+start);
                    var item = data[i];
                    ICell STT = row.CreateCell(0);
                    STT.SetCellValue(string.Format("{0:yyyyMddhhmmssfff}",DateTime.Now));
                    for(int ii=0;ii < propertyInfos.Count; ii++)
                    {
                        ICell cell = row.CreateCell(ii + 1);
                        PropertyInfo prc = propertyInfos[ii];
                        object value = prc.GetValue(item, null);
                        if(value != null)
                        {
                            Convert.ChangeType(value, prc.PropertyType);
                            switch (prc.PropertyType.Name)
                            {
                                case "Boolean":
                                    cell.SetCellValue((bool)value);
                                    break;
                                case "Double":
                                    cell.SetCellValue((double)value);
                                    break;
                                case "DateTime":
                                    cell.SetCellValue((DateTime)value);
                                    break;
                                default:
                                    cell.SetCellValue((string)value);
                                    break;
                            }
                        }
                        else
                        {
                            cell.SetCellType(CellType.Blank);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                _log.Error("ProccessBody"+ start, ex).Wait();
            }
            return Task.CompletedTask;
        }

        private IList<PropertyInfo> GetProperties<T>(T item) {
            Type oldType = item.GetType();
            IList<PropertyInfo> oldProps = new List<PropertyInfo>(oldType.GetProperties());
            return oldProps;
        } 
    }
}
