using SME.Utils.ExcelUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.Utils.Common.SMEException
{
    public class ImportErrorException : BusinessException
    {
        public FileErrorForm FileError { get; set; }

        public ImportErrorException()
        {

        }

        public ImportErrorException(List<CellImportError> lstError, IXVTWorkbook workbook)
        {
            lstError.ForEach(x =>
            {
                workbook.GetSheet(x.Sheet).AddCellComment(x.Row, x.Column, x.Error);
            });
            FileError = new FileErrorForm()
            {
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                FileName = "Error.xlsx",
                EncodedData = Convert.ToBase64String(GlobalUtil.ReadStreamFully(workbook.ToStream()))
            };
        }
    }
}
