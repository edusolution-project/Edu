using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.Model;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace EasyReport
{
    public class Test
    {
        public void Excel()
        {
            List<SinhVien> list = new List<SinhVien>()
            {
                new SinhVien{ MSSV = "15211TT00xx", Name = "Trần Minh Phát", Phone = "090999xxxx" },
                new SinhVien{ MSSV = "15211TT00xx", Name = "Võ Phương Quân", Phone = "090999xxxx" },
                new SinhVien{ MSSV = "15211TT00xx", Name = "Lê Bảo Long", Phone = "090999xxxx" },
                new SinhVien{ MSSV = "15211TT00xx", Name = "Nguyễn Trung Hiếu", Phone = "090999xxxx" },
            };

            // khởi tạo wb rỗng
            XSSFWorkbook wb = new XSSFWorkbook();

            // Tạo ra 1 sheet
            ISheet sheet = wb.CreateSheet();

            // Bắt đầu ghi lên sheet

            // Tạo row
            var row0 = sheet.CreateRow(0);
            // Merge lại row đầu 3 cột
            row0.CreateCell(0); // tạo ra cell trc khi merge
            CellRangeAddress cellMerge = new CellRangeAddress(0, 0, 0, 3);
            sheet.AddMergedRegion(cellMerge);
            row0.GetCell(0).SetCellValue("Thông tin sinh viên");

            // Ghi tên cột ở row 1
            var row1 = sheet.CreateRow(1);
            row1.CreateCell(0).SetCellValue("MSSV");
            //public XSSFCellStyle(StylesTable stylesSource);
            //public XSSFCellStyle(int cellXfId, int cellStyleXfId, StylesTable stylesSource, ThemesTable theme);
            //row1.Cells[0].CellStyle = new XSSFCellStyle();
            var style = new StylesTable();
            var font = new XSSFFont();

            //public short Color { get; set; }
            //public double FontHeight { get; set; }
            //public short FontHeightInPoints { get; set; }
            //public string FontName { get; set; }
            //public FontSuperScript TypeOffset { get; set; }
            //public bool IsStrikeout { get; set; }
            //public short Charset { get; set; }
            //public FontUnderlineType Underline { get; set; }
            //public short Boldweight { get; set; }
            //public bool IsItalic { get; set; }
            //public bool IsBold { get; set; }
            //public short Index { get; }
            //public int Family { get; set; }

            row1.CreateCell(1).SetCellValue("Tên");
            row1.CreateCell(2).SetCellValue("Phone");

            // bắt đầu duyệt mảng và ghi tiếp tục
            int rowIndex = 2;
            foreach (var item in list)
            {
                // tao row mới
                var newRow = sheet.CreateRow(rowIndex);

                // set giá trị
                newRow.CreateCell(0).SetCellValue(item.MSSV);
                newRow.CreateCell(1).SetCellValue(item.Name);
                newRow.CreateCell(2).SetCellValue(item.Phone);

                // tăng index
                rowIndex++;
            }
            // xong hết thì save file lại
            FileStream fs = new FileStream(@"E:\"+string.Format("{0:yyyyMddhhmmss}",DateTime.Now)+".xlsx", FileMode.CreateNew);
            wb.Write(fs);
        }
    }

    internal class SinhVien
    {
        public string MSSV { get; internal set; }
        public string Name { get; internal set; }
        public string Phone { get; internal set; }
    }
}
