using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SME.Utils.Common
{
    /// <summary>
    /// Created By: HaNN
    /// Created Date: 17/04/2017
    /// Các hằng số của hệ thống
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Hằng số cho HOSE
        /// </summary>
 
        public static class EXCEL
        {
            public const int SO = 1;
            public const int PO = 2;
            public const string EXCEL_SO = "SO";
            public const string EXCEL_PO = "PO";
        }


        public const string ORACLE_EXCEPTION_PARAMETER_NAME = "PROC_RETURN";
        public const string SQL_EXCEPTION_PARAMETER_NAME = "PROC_RETURN";
        public const int ERR_SQL_BASE = 200;
        public const int ERR_SQL_OPEN_CONNECTION_FAIL = ERR_SQL_BASE + 1;
        public const int ERR_SQL_EXECUTE_COMMAND_FAIL = ERR_SQL_BASE + 2;
        public const int ERR_SQL_DISCOVERY_PARAMS_FAIL = ERR_SQL_BASE + 3;
        public const int ERR_SQL_ASSIGN_PARAMS_FAIL = ERR_SQL_BASE + 4;

        public const int ERR_STOCK_BASE = 300;
        public const int ERR_STOCK_INFO_NOT_RECEIVED = ERR_STOCK_BASE + 1;

        public static readonly List<string> AbnormalIntValue = new List<string> { string.Empty, "KHÔNG CÓ", "-", "KHONG CO" };
    }
}
