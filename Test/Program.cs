using NPOI.HSSF.UserModel;
using NPOI.SS.Converter;
using System;
using System.IO;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //var a = new EasyReport.Test();
            //FileStream fs = new FileStream("C:\\Users\\longht\\source\\repos\\edusolution-project\\Edu\\Test\\EventExpenseReport.xls", FileMode.Open, FileAccess.Read);
            //HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs,true);
            //var sheet = templateWorkbook.GetSheet("Event Budget");
            //var header = sheet.Header;
            //var footer = sheet.Footer;

            string fileName = "EventExpenseReport.xls";
            fileName = Path.Combine(Environment.CurrentDirectory, fileName);
            HSSFWorkbook workbook = ExcelToHtmlUtils.LoadXls(fileName);


            ExcelToHtmlConverter excelToHtmlConverter = new ExcelToHtmlConverter();

            // Process the Excel file
            excelToHtmlConverter.ProcessWorkbook(workbook);

            var doc = excelToHtmlConverter.Document;

            var path = Path.ChangeExtension(fileName, "html");
            // Output the HTML file
            excelToHtmlConverter.Document.Save(path);
        }
    }
}
