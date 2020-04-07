using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyReport.Repositories
{
    public class ExcelService
    {
        public ExcelService()
        {

        }
        private void export(string sheetName)
        {
            // tạo mới 1 workbook
            XSSFWorkbook workbook = new XSSFWorkbook();
            // tạo sheet
            ISheet sheet = workbook.CreateSheet(sheetName);

        }
    }
}
