using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.Bussiness.Lib
{
    public class SMEConstant
    {
        public const string CAP_HOC = "CAP_HOC";
        public const string CHUC_VU = "CHUC_VU";
        public const string GIOI_TINH = "GIOI_TINH";
        public const string SERVICE_URL = "SERVICE_URL";
        public const string RESPONSE_STATUS = "RESPONSE_STATUS";
        public const string TRANG_THAI_HOC_SINH = "TRANG_THAI_HOC_SINH";
        public const string TRINH_DO_DAO_TAO = "TRINH_DO_DAO_TAO";
        public const string LOAI_DAY_DU_LIEU = "LOAI_DAY_DU_LIEU";

        /// <summary>
        /// Get sorted value by GET_SORT_VALUE function from Database
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        [DbFunction("Model.Store", "GET_SORT_VALUE")]
        public static string GetSortValue(String title)
        {
            //Direct calls are supported, return itseft

            return title;
        }
    }
}
