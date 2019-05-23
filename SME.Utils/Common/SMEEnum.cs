using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.Utils.Common
{
    public class SMEEnum
    {
        public enum ImportModules
        {
            TINH_THANH,
            QUAN_HUYEN,
            PHUONG_XA,
            TO_THON,
            TON_GIAO,
            TRUONG_HOC,
            DAN_TOC,
            DON_VI,
            CNGANH_DTAO,
            DTUONG_CSACH,
            MON_HOC,
            LY_DO_BO_HOC,
            TIEU_CHI_DL,
            LOAI_DOI_TUONG,
            LINH_VUC_DANH_GIA_SU_PHAT_TRIEN,
            CHI_SO_DANH_GIA_SU_PHAT_TRIEN,
            CHUAN_CHIEU_CAO_CAN_NANG,
            DU_LIEU_DAC_THU,
            NGUOI_DUNG
        }
        public enum VaiTro
        {
            NONE,
            SO_PHONG_BAN,
            NHA_TRUONG,
            QUAN_TRI_HE_THONG,
            QUAN,
            ADMIN_SO
        }

        public enum CapHocEn
        {
            ZERO,
            CAP1,
            CAP2,
            CAP3,
            NHATRE,
            MAUGIAO
        }
        public enum HocKy
        {
            ZERO,
            KY1,
            KY2,
            KY3
        }
        public enum PhanCapBieuMau
        {
            ZERO,
            CAP1,
            CAP2,
            CAP3,
            NHATRE,
            MAUGIAO,
            PHONG,
            SO
        }
        public enum Lop
        {
            ZERO,
            LOP1,
            LOP2,
            LOP3,
            LOP4,
            LOP5,
            LOP6,
            LOP7,
            LOP8,
            LOP9,
            LOP10,
            LOP11,
            LOP12
        }

        public enum ApplicationTypes
        {
            JavaScript = 0,
            NativeConfidential = 1
        };

        public enum ProcessImportStatus
        {
            SUCCESS,
            VALIDATED_FAIL,
            FILE_EMPTY,
            DATA_EMPTY,
            CANNOT_READ_FILE,
            FILE_SAMPLE_FAIL,
            DATA_NOT_EXIT,
            CANNOT_IMPORT
        }


        public enum LoaiDuLieu
        {
            NONE,
            DU_LIEU_SO,
            DU_LIEU_DUNG_SAI,
            DU_LIEU_CHUOI,
            DU_LIEU_LIST
        }

        public enum DuLieuLogic
        {
            SAI,
            DUNG
        }

    }
}
