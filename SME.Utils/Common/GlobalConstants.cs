using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.Utils.Common
{
    public static class GlobalConstants
    {
        public const string MODULE = "MODULE";

        /**
         * Define Modules
         * */
        public const string GLOBAL = "GLOBAL";
        public const string FUNCTION_PROVINCE_LIST = "PROVINCE_LIST";
        public const string FUNCTION_PROVINCE_CREATE = "PROVINCE_CREATE";
        public const string FUNCTION_PROVINCE_SEARCH = "PROVINCE_SEARCH";
        public const string FUNCTION_PROVINCE_DETAIL = "PROVINCE_DETAIL";
        public const string FUNCTION_PROVINCE_ADD = "PROVINCE_ADD";
        public const string BO_HOC_CREATE = "BO_HOC_CREATE";

        // Ton Giao 
        public const string FUNCTION_RELIGION_LIST = "RELIGION_LIST";
        public const string FUNCTION_RELIGION_CREATE = "RELIGION_CREATE";
        public const string FUNCTION_RELIGION_SEARCH = "RELIGION_SEARCH";
        public const string FUNCTION_RELIGION_DETAIL = "RELIGION_DETAIL";
        public const string FUNCTION_RELIGION_ADD = "RELIGION_ADD";
        public const string FUNCTION_RELIGION_LOCK = "RELIGION_LOCK";
        public const string FUNCTION_RELIGION_UNLOCK = "RELIGION_UNLOCK";
        public const string FUNCTION_RELIGION_UPDATE = "RELIGION_UPDATE";
        public const string FUNCTION_RELIGION_UPLOAD = "RELIGION_UPLOAD";

        // Truong hoc 
        public const string FUNCTION_SCHOOL_LIST = "SCHOOL_LIST";
        public const string FUNCTION_SCHOOL_CREATE = "SCHOOL_CREATE";
        public const string FUNCTION_SCHOOL_SEARCH = "SCHOOL_SEARCH";
        public const string FUNCTION_SCHOOL_DETAIL = "SCHOOL_DETAIL";
        public const string FUNCTION_SCHOOL_ADD = "SCHOOL_ADD";
        public const string FUNCTION_SCHOOL_LOCK = "SCHOOL_LOCK";
        public const string FUNCTION_SCHOOL_UNLOCK = "SCHOOL_UNLOCK";
        public const string FUNCTION_SCHOOL_UPDATE = "SCHOOL_UPDATE";
        public const string FUNCTION_SCHOOL_UPLOAD = "SCHOOL_UPLOAD";

        //Organization
        public const string FUNCTION_ORGANIZATION_LIST = "ORGANIZATION_LIST";
        public const string FUNCTION_ORGANIZATION_CREATE = "ORGANIZATION_CREATE";
        public const string FUNCTION_ORGANIZATION_SEARCH = "ORGANIZATION_SEARCH";
        public const string FUNCTION_ORGANIZATION_DETAIL = "ORGANIZATION_DETAIL";
        public const string FUNCTION_ORGANIZATION_ADD = "ORGANIZATION_ADD";
        public const string FUNCTION_ORGANIZATION_UPDATE = "ORGANIZATION_UPDATE";
        public const string FUNCTION_ORGANIZATION_LOCK = "ORGANIZATION_LOCK";
        public const string FUNCTION_ORGANIZATION_UNLOCK = "ORGANIZATION_UNLOCK";
        public const string FUNCTION_ORGANIZATION_UPLOAD = "ORGANIZATION_UPLOAD";


        /**Districts**/
        public const string FUNCTION_DISTRICT_LIST = "DISTRICT_LIST";
        public const string FUNCTION_DISTRICT_SEARCH = "DISTRICT_SEARCH";
        public const string FUNCTION_DISTRICT_DETAIL = "DISTRICT_DETAIL";
        public const string FUNCTION_DISTRICT_ADD = "DISTRICT_ADD";
        public const string FUNCTION_DISTRICT_UPDATE = "DISTRICT_UPDATE";
        public const string FUNCTION_DISTRICT_LOCK = "DISTRICT_LOCK";
        public const string FUNCTION_DISTRICT_UNLOCK = "DISTRICT_UNLOCK";
        public const string FUNCTION_DISTRICT_UPLOAD = "DISTRICT_UPLOAD";

        /**CH_DAY_DU_LIEU**/
        public const string FUNCTION_CH_DAY_DU_LIEU_LIST = "CH_DAY_DU_LIEU_LIST";
        public const string FUNCTION_CH_DAY_DU_LIEU_SEARCH = "CH_DAY_DU_LIEU_SEARCH";
        public const string FUNCTION_CH_DAY_DU_LIEU_DETAIL = "CH_DAY_DU_LIEU_DETAIL";
        public const string FUNCTION_CH_DAY_DU_LIEU_ADD = "CH_DAY_DU_LIEU_ADD";
        public const string FUNCTION_CH_DAY_DU_LIEU_UPDATE = "CH_DAY_DU_LIEU_UPDATE";
        public const string FUNCTION_CH_DAY_DU_LIEU_LOCK = "CH_DAY_DU_LIEU_LOCK";
        public const string FUNCTION_CH_DAY_DU_LIEU_UNLOCK = "CH_DAY_DU_LIEU_UNLOCK";
        public const string FUNCTION_CH_DAY_DU_LIEU_CHECK_EXIST = "CH_DAY_DU_LIEU_CHECK_EXIST";
        public const string FUNCTION_CH_DAY_DU_LIEU_CHECK_DONG_BO = "CH_DAY_DU_LIEU_CHECK_DONG_BO";
        /**CH_DDL_DOT**/
        public const string FUNCTION_CH_DDL_DOT_LIST = "CH_DDL_DOT_LIST";
        public const string FUNCTION_CH_DDL_DOT_SEARCH = "CH_DDL_DOT_SEARCH";
        public const string FUNCTION_CH_DDL_DOT_DETAIL = "CH_DDL_DOT_DETAIL";
        public const string FUNCTION_CH_DDL_DOT_ADD = "CH_DDL_DOT_ADD";
        public const string FUNCTION_CH_DDL_DOT_UPDATE = "CH_DDL_DOT_UPDATE";
        public const string FUNCTION_CH_DDL_DOT_LOCK = "CH_DDL_DOT_LOCK";
        public const string FUNCTION_CH_DDL_DOT_UNLOCK = "CH_DDL_DOT_UNLOCK";
        public const string FUNCTION_CH_DDL_DOT_UPLOAD = "CH_DDL_DOT_UPLOAD";
        public const string FUNCTION_CH_DDL_DOT_CHECK_DATE = "CH_DDL_DOT_CHECK_DATE";
        public const string FUNC_CH_DDL_DOT_CHECK_DATE_DAU_NAM = "CH_DDL_DOT_DAU_NAM";
        public const string FUNC_CH_DDL_DOT_CHECK_DATE_PHAT_SINH = "CH_DDL_DOT_PHAT_SINH";
        public const string FUNC_CH_DDL_DOT_CHECK_DATE_CUOI_NAM = "CH_DDL_DOT_CUOI_NAM";

        /**YC_DDL_TRUONG_HOC**/
        public const string FUNCTION_YC_DDL_TRUONG_HOC_LIST = "YC_DDL_TRUONG_HOC_LIST";
        public const string FUNCTION_YC_DDL_TRUONG_HOC_SEARCH = "YC_DDL_TRUONG_HOC_SEARCH";
        public const string FUNCTION_YC_DDL_TRUONG_HOC_DETAIL = "YC_DDL_TRUONG_HOC_DETAIL";
        public const string FUNCTION_YC_DDL_TRUONG_HOC_ADD = "YC_DDL_TRUONG_HOC_ADD";
        public const string FUNCTION_YC_DDL_TRUONG_HOC_UPDATE = "YC_DDL_TRUONG_HOC_UPDATE";
        public const string FUNCTION_YC_DDL_TRUONG_HOC_LOCK = "YC_DDL_TRUONG_HOC_LOCK";
        public const string FUNCTION_YC_DDL_TRUONG_HOC_UNLOCK = "YC_DDL_TRUONG_HOC_UNLOCK";
        public const string FUNCTION_YC_DDL_TRUONG_HOC_CHECK_EXIST = "YC_DDL_TRUONG_HOC_CHECK_EXIST";
        public const string FUNCTION_YC_DDL_TRUONG_HOC_NOT_EXIST = "YC_DDL_TRUONG_HOC_NOT_EXIST";
        public const string FUNCTION_YC_DDL_TRUONG_HOC_NO_VALID = "YC_DDL_TRUONG_HOC_NO_VALID";
        public const string FUNCTION_YC_DDL_TRUONG_HOC_NO_FOUND = "YC_DDL_TRUONG_HOC_NO_FOUND";
        public const string FUNCTION_YC_DDL_TRUONG_HOC_SUM = "YC_DDL_TRUONG_HOC_SUM";
        /**CH_DDL_TRUONG_HOC**/
        public const string FUNCTION_CH_DDL_TRUONG_HOC_LIST = "CH_DDL_TRUONG_HOC_LIST";
        public const string FUNCTION_CH_DDL_TRUONG_HOC_SEARCH = "CH_DDL_TRUONG_HOC_SEARCH";
        public const string FUNCTION_CH_DDL_TRUONG_HOC_DETAIL = "CH_DDL_TRUONG_HOC_DETAIL";
        public const string FUNCTION_CH_DDL_TRUONG_HOC_ADD = "CH_DDL_TRUONG_HOC_ADD";
        public const string FUNCTION_CH_DDL_TRUONG_HOC_UPDATE = "CH_DDL_TRUONG_HOC_UPDATE";
        public const string FUNCTION_CH_DDL_TRUONG_HOC_LOCK = "CH_DDL_TRUONG_HOC_LOCK";
        public const string FUNCTION_CH_DDL_TRUONG_HOC_UNLOCK = "CH_DDL_TRUONG_HOC_UNLOCK";
        public const string FUNCTION_CH_DDL_TRUONG_HOC_CHECK_EXIST = "CH_DDL_TRUONG_HOC_CHECK_EXIST";
        public const string FUNCTION_CH_DDL_TRUONG_HOC_DATE_VALID = "CH_DDL_TRUONG_HOC_DATE_VALID";
        /****/
        /**LICH_SU_DAY_DL**/
        public const string FUNCTION_LICH_SU_DAY_DL_LIST = "LICH_SU_DAY_DL_LIST";
        public const string FUNCTION_LICH_SU_DAY_DL_SEARCH = "LICH_SU_DAY_DL_SEARCH";
        public const string FUNCTION_LICH_SU_DAY_DL_DETAIL = "LICH_SU_DAY_DL_DETAIL";
        public const string FUNCTION_LICH_SU_DAY_DL_ADD = "LICH_SU_DAY_DL_ADD";
        public const string FUNCTION_LICH_SU_DAY_DL_UPDATE = "LICH_SU_DAY_DL_UPDATE";
        public const string FUNCTION_LICH_SU_DAY_DL_LOCK = "LICH_SU_DAY_DL_LOCK";
        public const string FUNCTION_LICH_SU_DAY_DL_UNLOCK = "LICH_SU_DAY_DL_UNLOCK";
        public const string FUNCTION_LICH_SU_DAY_DL_UPLOAD = "LICH_SU_DAY_DL_UPLOAD";
        public const string FUNCTION_LICH_SU_DAY_DL_NOT_FOUND = "LICH_SU_DAY_DL_NOT_FOUND";

        public const string FUNCTION_PROVINCE_UPDATE = "PROVINCE_UPDATE";
        public const string FUNCTION_PROVINCE_LOCK = "PROVINCE_LOCK";
        public const string FUNCTION_PROVINCE_UNLOCK = "PROVINCE_UNLOCK";
        public const string FUNCTION_ETHNIC_LIST = "ETHNIC_LIST";
        public const string FUNCTION_ETHNIC_CREATE = "ETHNIC_CREATE";
        public const string FUNCTION_ETHNIC_DETAIL = "ETHNIC_DETAIL";
        public const string FUNCTION_ETHNIC_ADD = "ETHNIC_ADD";
        public const string FUNCTION_ETHNIC_SEARCH = "ETHNIC_SEARCH";
        public const string FUNCTION_ETHNIC_UPDATE = "ETHNIC_UPDATE";
        public const string FUNCTION_ETHNIC_LOCK = "PROVINCE_LOCK";
        public const string FUNCTION_ETHNIC_UNLOCK = "PROVINCE_UNLOCK";
        // Doi tac
        public const string FUNCTION_PARTNER_LIST = "PARTNER_LIST";
        public const string FUNCTION_PARTNER_UPDATE = "PARTNER_UPDATE";
        public const string FUNCTION_PARTNER_ADD = "PARTNER_ADD";

        public const string FUNCTION_USER_LIST = "USER_LIST";
        public const string FUNCTION_USER_CREATE = "USER_CREATE";
        public const string FUNCTION_USER_DETAIL = "USER_DETAIL";
        public const string FUNCTION_USER_ADD = "USER_ADD";
        public const string FUNCTION_USER_SEARCH = "USER_SEARCH";
        public const string FUNCTION_USER_UPDATE = "USER_UPDATE";
        public const string FUNCTION_PROVINCE_UPLOAD = "PROVINCE_UPLOAD";

        //Mon Hoc
        public const string FUNCTION_SUBJECT_LIST = "SUBJECT_LIST";
        public const string FUNCTION_SUBJECT_DETAIL = "SUBJECT_DETAIL";
        public const string FUNCTION_SUBJECT_ADD = "SUBJECT_ADD";
        public const string FUNCTION_SUBJECT_SEARCH = "SUBJECT_SEARCH";
        public const string FUNCTION_SUBJECT_UPDATE = "SUBJECT_UPDATE";
        public const string FUNCTION_SUBJECT_LOCK = "SUBJECT_LOCK";
        public const string FUNCTION_SUBJECT_UNLOCK = "SUBJECT_UNLOCK";
        public const string FUNCTION_SUBJECT_UPLOAD = "SUBJECT_UPLOAD";

        // khai báo Hamlet
        public const string FUNCTION_HAMLET_LIST = "HAMLET_LIST";
        public const string FUNCTION_HAMLET_SEARCH = "HAMLET_SEARCH";
        public const string FUNCTION_HAMLET_UPLOAD = "HAMLET_UPLOAD";
        public const string FUNCTION_HAMLET_CREATE = "HAMLET_CREATE";
        public const string FUNCTION_HAMLET_DETAIL = "HAMLET_DETAIL";
        public const string FUNCTION_HAMLET_ADD = "HAMLET_ADD";
        public const string FUNCTION_HAMLET_UPDATE = "HAMLET_UPDATE";
        public const string FUNCTION_HAMLET_GETALL = "HAMLET_GETALL";
        public const string FUNCTION_HAMLET_LOCK = "HAMLET_LOCK";
        //Chuyen Nganh Dao Tao

        public const string FUNCTION_CNGANHDTAO_LIST = "CNGANHDTAO_LIST";
        public const string FUNCTION_CNGANHDTAO_CREATE = "CNGANHDTAO_CREATE";
        public const string FUNCTION_CNGANHDTAO_SEARCH = "CNGANHDTAO_SEARCH";
        public const string FUNCTION_CNGANHDTAO_DETAIL = "CNGANHDTAO_DETAIL";
        public const string FUNCTION_CNGANHDTAO_ADD = "CNGANHDTAO_ADD";
        public const string FUNCTION_CNGANHDTAO_UPLOAD = "CNGANHDTAO_UPLOAD";
        public const string FUNCTION_CNGANHDTAO_UPDATE = "CNGANHDTAO_UPDATE";
        public const string FUNCTION_CNGANHDTAO_LOCK = "CNGANHDTAO_LOCK";

        //Loai Doi Tuong
        public const string FUNCTION_LOAIDOITUONG_LIST = "LOAI_DOI_TUONG_LIST";
        public const string FUNCTION_LOAIDOITUONG_CREATE = "LOAI_DOI_TUONG_CREATE";
        public const string FUNCTION_LOAIDOITUONG_SEARCH = "LOAI_DOI_TUONG_SEARCH";
        public const string FUNCTION_LOAIDOITUONG_DETAIL = "LOAI_DOI_TUONG_DETAIL";
        public const string FUNCTION_LOAIDOITUONG_ADD = "LOAI_DOI_TUONG_ADD";
        public const string FUNCTION_LOAIDOITUONG_UPLOAD = "LOAI_DOI_TUONG_UPLOAD";
        public const string FUNCTION_LOAIDOITUONG_UPDATE = "LOAI_DOI_TUONG_UPDATE";
        public const string FUNCTION_LOAIDOITUONG_LOCK = "LOAI_DOI_TUONG_LOCK";

        // DOWNLOAD TEMPLATE
        public const string TEMPLATE_PREFIX = "TEMPLATE_";        //Phuong Xa
        public const string FUNCTION_VILLAGE_LIST = "VILLAGE_LIST";
        public const string FUNCTION_VILLAGE_SEARCH = "VILLAGE_SEARCH";
        public const string FUNCTION_VILLAGE_DETAIL = "VILLAGE_DETAIL";
        public const string FUNCTION_VILLAGE_ADD = "VILLAGE_ADD";
        public const string FUNCTION_VILLAGE_UPDATE = "VILLAGE_UPDATE";
        public const string FUNCTION_VILLAGE_LOCK = "VILLAGE_LOCK";
        public const string FUNCTION_VILLAGE_UNLOCK = "VILLAGE_UNLOCK";
        public const string FUNCTION_VILLAGE_UPLOAD = "VILLAGE_UPLOAD";

        /**POLICY**/
        public const string FUNCTION_POLICY_LIST = "POLICY_LIST";
        public const string FUNCTION_POLICY_SEARCH = "POLICY_SEARCH";
        public const string FUNCTION_POLICY_DETAIL = "POLICY_DETAIL";
        public const string FUNCTION_POLICY_ADD = "POLICY_ADD";
        public const string FUNCTION_POLICY_UPDATE = "POLICY_UPDATE";
        public const string FUNCTION_POLICY_LOCK = "POLICY_LOCK";
        public const string FUNCTION_POLICY_UNLOCK = "POLICY_UNLOCK";
        public const string FUNCTION_POLICY_UPLOAD = "POLICY_UPLOAD";

        /**CHANGE PASSWORD**/
        public const string FUNCTION_CHANGE_PASS = "CHANGE_PASS";

        //reset pass
        public const string FUNCTION_RESET_PASS = "RESET_PASS";

        /** IMPORT MODULE **/
        public const string IMPORT_MODULE = "IMPORT";

        /** Export Template **/
        public const string EXPORT_FONTNAME = "Times New Roman";
        public const string EXPORT_TEMPLATE_PREFIX = "EXPORT_TEMPLATE";
        public const string TK_TINHHINH_HOCLUC_PREFIX = "EXPORT_TEMPLATE_HL_HS_";
        public const string TK_TINHHINH_HANHKIEM_PREFIX = "EXPORT_TEMPLATE_HK_HS_";
        public const string TK_HANHKIEM_HOCLUC_PREFIX = "EXPORT_TEMPLATE_HK_HL_HS_";

        public const string TK_SOLUONG_BOHOC_PREFIX = "EXPORT_TEMPLATE_BH_HS_";
        public const string TK_SOLUONG_HOCSINH_PREFIX = "EXPORT_TEMPLATE_SL_HS_";
        public const string TK_SOLUONG_HOCSINH_THEO_PHAN_BAN_PREFIX = "EXPORT_TEMPLATE_SL_HS_THEO_PHAN_BAN_";
        public const string TK_TINHHINH_TOTNGHIEP_PREFIX = "EXPORT_TEMPLATE_HS_TN_";
        public const string SOLUONG_TRINHDO_GV_PREFIX = "EXPORT_TEMPLATE_TD_GV_";
        public const string TUYEN_SINH_LOP_10_PREFIX = "EXPORT_TEMPLATE_TUYEN_SINH_LOP_10_";
        public const string EXCEL_EXTENSION = ".xlsx";


        /**GET_ALL_STUDENTS**/
        public const string FUNCTION_GET_ALL_STUDENTS = "GET_ALL_STUDENTS";

        public const string TK_PHAN_LOAI_DIEM_PREFIX = "EXPORT_TEMPLATE_PHAN_LOAI_DIEM";

        /**REPORT_SL_HS**/
        public const string FUNCTION_SOLUONG_HS_LIST = "SOLUONG_HS_LIST";
        public const string FUNCTION_SOLUONG_HS_EXPORT = "SOLUONG_HS_EXPORT";

        /**BH_HS**/
        public const string FUNCTION_BOHOC_HS_LIST = "BOHOC_HS_LIST";
        public const string FUNCTION_BOHOC_HS_EXPORT = "BOHOC_HS_EXPORT";

        /**Thong ke Hanh Kiem HS**/
        public const string FUNCTION_HANHKIEM_HS_LIST = "HANHKIEM_HS_LIST";
        public const string FUNCTION_HANHKIEM_HS_EXPORT = "HANHKIEM_HS_EXPORT";
        /**Thong ke Tot Nghiep**/
        public const string FUNCTION_TOTNGHIEP_HS_LIST = "TOTNGHIEP_HS_LIST";
        public const string FUNCTION_TOTNGHIEP_HS_EXPORT = "TOTNGHIEP_HS_EXPORT";

        //TK học lực học sinh
        public const string FUNCTION_HOCLUC_HS_LIST = "HOCLUC_HS_LIST";
        public const string FUNCTION_HOCLUC_HS_EXPORT = "HOCLUC_HS_EXPORT";
        // tk hạn kiểm học lực học sinh dân tộc thiểu số
        public const string FUNCTION_HANHKIEM_HOCLUC_HS_LIST = "HANHKIEM_HOCLUC_HS_LIST";
        public const string FUNCTION_HANHKIEM_HOCLUC_HS_EXPORT = "HANHKIEM_HOCLUC_HS_LIST";

        public const string FUNCTION_LY_DO_BO_HOC_LIST = "LY_DO_BO_HOC_LIST";

        public const string BC_LOP_MON_DAC_BIET_C1 = "EXPORT_TEMPLATE_BC_LOP_MON_DAC_BIET_C1";
        public const string BC_LOP_MON_DAC_BIET_C2 = "EXPORT_TEMPLATE_BC_LOP_MON_DAC_BIET_C2";
        public const string BC_LOP_MON_DAC_BIET_C3 = "EXPORT_TEMPLATE_BC_LOP_MON_DAC_BIET_C3";

        public const string BC_DOI_NGU_GIAO_VIEN = "EXPORT_TEMPLATE_BC_DOI_NGU_GIAO_VIEN";

        public const string DS_TRUONG_CHUA_DAY_DL = "EXPORT_TEMPLATE_DS_TRUONG_CHUA_DAY_DL";
        public const string DS_LS_DONG_BO_TIEU_CHI_EMIS = "EXPORT_TEMPLATE_LS_DONG_BO_TIEU_CHI_EMIS";

        public const string TK_PHAN_LOAI_DIEM_TB_PREFIX = "EXPORT_TEMPLATE_PHAN_LOAI_DIEM_TB";
        public const long GIOI_TINH_NU = 0;
        public const long GIOI_TINH_NAM = 1;
        public const long DAN_TOC_THIEU_SO = 1;
        public const long HS_KHUYET_TAT = 1; // Khuyet tat van dong
        public const long HS_KHUYET_TAT_1 = 2; // Khuyet tat nghe noi
        public const long HS_KHUYET_TAT_2 = 3; // Khuyet tat tam nhin
        public const long HS_KHUYET_TAT_3 = 4; // Khuyet tat than kinh tam than
        public const long HS_KHUYET_TAT_4 = 5; // Khuyet tat tri tuye
        public const long HS_KHUYET_TAT_5 = 6; // Khac
        public const long KY_HOC_I = 1;
        public const long KY_HOC_II = 2;
        public const long KY_HOC_CN = 3;
        public const string CAP1 = "1";
        public const string CAP2 = "2";
        public const string CAP3 = "3";

        public const string TK_DANH_HIEU_HS_C1 = "EXPORT_TEMPLATE_DANH_HIEU_HS_C1";
        public const string TK_DANH_HIEU_HS_C2 = "EXPORT_TEMPLATE_DANH_HIEU_HS_C2";
        public const string TK_DANH_HIEU_HS_C3 = "EXPORT_TEMPLATE_DANH_HIEU_HS_C3";

        public const string TK_KET_QUA_HS_C1 = "EXPORT_TEMPLATE_KET_QUA_HS_C1";
        public const string TK_KET_QUA_HS_C2 = "EXPORT_TEMPLATE_KET_QUA_HS_C2";
        public const string TK_KET_QUA_HS_C3 = "EXPORT_TEMPLATE_KET_QUA_HS_C3";


        //Thuonglv3 add constant of TSDC
        public const string ENCRYPTED_PASSWORD = "dfhot3478hsdfid^43irgSy%$#gbdortw496&^T$^";
        public const int STATUS_ACTIVE = 1;
        public const int STATUS_DEACTIVE = 0;
        public const string MESSAGE_EXCEPTION = "Hệ thống đang bảo trì. Vui lòng quay lại sau";
        public const string MESSAGE_EXCEPTION_INVALID_USER = "Bạn không có quyền thực hiện chức năng này!";
        public const string MESSAGE_EXCEPTION_INVALID_SSO_USER = "Người dùng không tồn tại trong hệ thống!";
        public const string MESSAGE_EXCEPTION_BAD_REQUEST = "Dữ liệu có chứa ký tự nguy hiểm";
        public const string MA_DA_NANG = "48";

        public const string HEADER_AUTHORIZATION = "Authorization";
        public const string HEADER_USERNAME = "Username";
        public const string HEADER_PASSWORD = "Password";
        public const string HEADER_USER_AGENT = "User-Agent";
        public const string HEADER_REQ_MESSAGE_ID = "ReqMessageId";
        public const int SESSION_TIMEOUT = 30;
        // lịch sử đồng bộ emis
        public const long EMIS_TIEU_CHI_HOAT_DONG = 1;
        public const long EMIS_TIEU_CHI_NGUNG_HOAT_DONG = 0;

        public const long TIEU_CHI_BCD_HOAT_DONG = 1;
        public const long TIEU_CHI_BCD_NGUNG_HOAT_DONG = 0;

        public const string LICH_SU_DONG_BO_EMIS_SUCCESS = "00";
        public const string LICH_SU_DONG_BO_EMIS_ERROR = "01";
        public const string LICH_SU_DONG_BO_EMIS_WARNING = "03";
        public const string LICH_SU_DONG_BO_EMIS_NOT_IN_DB = "04";
        public const int LICH_SU_DONG_BO_EMIS_TRANG_THAI_NGUNG_HOAT_DONG = 0;
        public const int LICH_SU_DONG_BO_EMIS_TRANG_THAI_HOAT_DONG = 1;

        public const int CAPTCHA_LENGTH = 5;

        public const long TIME_CACHE = 5 * 60 * 1000;

        public const long LOAI_DON_VI_SO = 1;
        public const long LOAI_DON_VI_PHONG = 2;

        public const long LOAI_TAI_LIEU_CAU_HINH_TRUONG_HOC = 2;
        public const long LOAI_TAI_LIEU_HO_SO_DU_TUYEN = 3;
        public const long TRUE = 1;
        public const long FALSE = 0;
        public const string PHHS = "Phụ huynh học sinh";
        #region Báo cáo
        public const int BAO_CAO_CAP_PHONG = 1;
        public const int BAO_CAO_CAP_SO = 2;
        public const int BAO_CAO_CAP_QUAN = 3;
        #endregion
        #region Tinh trang cua dot tuyen sinh dua vao ngay bat dau, ket thuc
        public const long DTS_DANG_HOAT_DONG = 0;
        public const long DTS_CHUA_BAT_DAU = 1;
        public const long DTS_DA_KET_THUC = 2;
        #endregion
        #region Vai tro nguoi dung
        public const int VAITRO_CAPSO_PB = 1;
        public const int VAITRO_CAP_TRUONG = 2;
        public const int VAITRO_ADMIN = 3;
        public const int VAITRO_CAP_QUAN = 4;
        public const int VAITRO_ADMIN_SO = 5;
        #endregion
        #region Dan toc
        public const string DAN_TOC_KINH = "01";

        #endregion
        #region Cap hoc
        public const long CAP_1 = 1;
        public const long CAP_2 = 2;
        public const long CAP_3 = 3;
        public const long NHA_TRE = 4;
        public const long MAU_GIAO = 5;

        public const string TRUONG_TIEU_HOC = "1";
        public const string TRUONG_NHA_TRE = "4";
        public const string TRUONG_MAU_GIAO = "5";
        public const string TRUONG_MAM_NON = "45";
        #endregion

        #region Trang Thai Ho so
        // Trang thai ho so chi de hien thi trong lich su
        public const long HO_SO_CAP_NHAT_NGAY_HEN = 9;
        public const long HO_SO_XU_LY_LAI = 8;
        public const long HO_SO_BI_SUA = 7;
        public const long HO_SO_SO_THAO = 1;
        public const long HO_SO_DA_NHAN = 2;
        public const long HO_SO_DA_DUYET = 3;
        public const long HO_SO_CHUYEN_TIEP = 4;
        public const long HO_SO_TU_CHOI = 5;
        public const long HO_SO_TRAI_TUYEN_MOI_TAO = 6;
        public const long DUNG_TUYEN = 1;
        public const int TRAI_TUYEN = 0;
        #endregion

        #region Loai cu tru
        public const long LOAI_THUONG_TRU_KT1 = 1;
        public const long LOAI_TAM_TRU_KT2 = 2;
        public const long LOAI_TAM_TRU_KT3 = 3;
        public const long LOAI_TAM_TRU_KT4 = 4;
        #endregion

        #region Gioi tinh
        public const long NAM = 1;
        public const long NU = 0;
        #endregion

        #region Cau hinh thong tin diem mon van, toan cho hoc sinh tieu hoc
        public const long HK2 = 1;
        public const string MA_MON_VAN = "Van";
        public const string MA_MON_TOAN = "Toan";
        public const long LOAI_DIEM_CUOI_KY = 6;
        public const long LOAI_DANH_GIA_MON_HOC = 1;
        public const long LOAI_DANH_GIA_NANG_LUC = 2;
        public const long LOAI_DANH_GIA_PHAM_CHAT = 3;
        public const string HOAN_THANH_TO = "T";
        public const string HOAN_THANH = "H";
        public const string CHUA_HOAN_THANH = "C";
        #endregion

        #region Hoc Tieng Anh
        public const long KHONG_HOC_TA = 1;
        public const long HOC_TA_TAI_TRUONG = 2;
        public const long HOC_TA_TAI_TRUNGTAM = 3;
        #endregion

        #region Cap chuyen ho so len cap tren
        public const long CAP_PHHS = 0;
        public const long CAP_TRUONG = 1;
        public const long CAP_PHONG = 2;
        public const long CAP_QUAN = 3;
        #endregion

        #region Thao tac voi ho so
        public const long TU_CHOI = 1;
        public const long PHE_DUYET = 2;
        public const long CHUYEN_CAP_TREN = 3;

        public const long RECORD_PROCESSED_PER_DAY_DEFAULT = 40;
        #endregion
        #region Tiêu chí emis
        public const long LOAI_TIEU_CHI_SCRIPT = 1;
        public const long LOAI_TIEU_CHI_PROC = 0;
        #endregion
        #region emis tiêu chí biểu mẫu
        public const long TRANG_THAI_TON_TAI = 1;
        public const long TRANG_THAI_KHONG_TON_TAI = 0;
        #endregion
        #region Cau hinh truong
        public class CauHinhTruongHoc
        {
            public const string DaCauHinh = "Đã cấu hình";
            public const string ChuaCauHinh = "Chưa cấu hình";
        }
        #endregion

        #region Loai ngoai ngu dang ky ho so
        public const long NGOAI_NGU_TIENG_ANH = 1;
        public const long NGOAI_NGU_TIENG_PHAP = 2;
        public const long NGOAI_NGU_TIENG_NHAT = 3;
        public const long NGOAI_NGU_TIENG_DUC = 4;
        public const long NGOAI_NGU_TIENG_HAN = 5;
        #endregion
        #region Phan cap bieu mau
        public const long THAO_TAC_TONG_HOP = 1;
        public const long THAO_TAC_NHAP_LIEU = 2;
        public const long TONG_HOP_NHAP_LIEU = 3;
        public const long TRANG_THAI_HIEU_LUC = 1;
        public const long TRANG_THAI_HET_HIEU_LUC = 0;
        #endregion
        #region Nhom bieu mau
        public const long LOAI_BIEU_MAU_PHONG = 1;
        public const long LOAI_BIEU_MAU_SO = 0;
        public const long LOAI_BIEU_MAU_TRUONG = 2;
        #endregion
        public const string COMMON_INVALID_USER = "Tên đăng nhập hoặc mật khẩu không đúng";
        public const string COMMON_SSO_INVALID_USER = "Tài khoản chưa được đăng ký trên hệ thống";
        public const string COMMON_SSO_INVALID_AUTHORIZE = "Xác thực không hợp lệ";
        public const string COMMON_INVALID_CAPTCHA = "Mã xác nhận không hợp lệ";

        #region trang thai yeu cau dong bo du lieu
        public const long YCDB_TRANG_THAI_NEW = 1;
        public const long YCDB_TRANG_THAI_DELETED = 0;
        public const long YCDB_TRANG_THAI_RESOLVED = 2;
        #endregion

        #region loai yeu cau dong bo du lieu
        public const long YCDB_LOAI_YC_DAU_NAM = 1;
        public const long YCDB_LOAI_YC_CUOI_NAM = 3;
        public const long YCDB_LOAI_YC_PHAT_SINH = 2;
        public const long YCDB_LOAI_YC_ALL = 4;
        #endregion

        #region loai can bo
        public const long LOAI_CAN_BO_CBQL = 1;
        public const long LOAI_CAN_BO_GIAO_VIEN = 2;
        public const long LOAI_CAN_BO_NHAN_VIEN = 3;
        #endregion
        #region học sinh theo năm
        public const long TRANG_THAI_DANG_HOC_HOAC_DA_HOAN_THANH = 1;
        public const long TRANG_THAI_CHUYEN_TRUONG = 3;
        public const long TRANG_THAI_BO_HOC = 4;
        #endregion

        public const long LOAI_TAI_KHOAN_ORIGIN = 1;
        public const long LOAI_TAI_KHOAN_VIETTEL_STUDY = 2;

        #region loại báo cáo
        public const long LOAI_BAO_CAO_SO = 1;
        public const long LOAI_BAO_CAO_PHONG = 2;
        public const long LOAI_BAO_CAO_TRUONG = 3;
        public const long LOAI_BAO_CAO_ADMIN = 4;
        #endregion

        public const string SUCCESS = "success";
        public const string DU_LIEU_DAC_THU = "EXPORT_TEMPLATE_DU_LIEU_DAC_THU";

        #region list dữ liệu
        public static List<long> LstTrueFalse = new List<long>() { 0, 1 };
        public static List<long> LstGioiTinh = new List<long>() { 0, 1 };
        public static List<long> LstNhomMau = new List<long>() { 1, 2, 3, 4, 5 };
        public static List<long> LstTphanGdinh = new List<long>() { 1, 2, 3 };
        public static List<long> LstDtuongUuTien = new List<long>() { 15, 16, 17 };
        public static List<long> LstLoaiTamTru = new List<long>() { 2, 3, 4 };
        #endregion

        #region loại service
        public const long WS_SuaHocSinhTheoLo = 9;
        public const long WS_SuaGiaoVienTheoLo = 10;
        public const long WS_DayTtinDauNam = 11;
        public const long WS_DayTtinHocSinh = 12;
        public const long WS_DayTtinDiem = 13;
        public const long WS_DayTtinCuoiNam = 14;
        public const long WS_DayTtinTreMamNon = 14;
        #endregion
        public const long DANG_HOAT_DONG = 1;
        public const long NGUNG_HOAT_DONG = 0;
        public enum StatusCode
        {
            Succeed = 200,
            ErrorException = 500,
            ErrorBusiness = 201,
            ErrorDuplicateUpdateRequest = 202,
            ErrorDuplicateDeleteRequest = 203,
            DuplicateSuccessRequest = 452,
            DuplicateErrorRequest = 453
        };

        public static Dictionary<long, long> DicKhoiHocCapHoc = new Dictionary<long, long>()
        {
            {1, 1}, {2, 1},{3, 1}, {4, 1}, {5, 1},
            {6, 2}, {7, 2},{8, 2}, {9, 2},
            {10, 3}, {11, 3},{12, 3},
            {13, 4}, {14, 4},{15, 4},
            {16, 5}, {17, 5},{18, 5}
        };
        public static Dictionary<long, string> DicViTriChinh = new Dictionary<long, string>()
        {
            {1, "Hiệu trưởng" }, {2, "Phó hiệu trưởng"}
        };
        public static Dictionary<long, string> DicViTri = new Dictionary<long, string>()
        {
            {1, "Nhân viên thư viện" }, {2, "Nhân viên thiết bị"}, {3, "Nhân viên y tế"},
            {4, "Nhân viên thí nghiệm" }, {5, "Nhân viên kỹ thuật nghiệp vụ"},
            { 6, "Nhân viên khác (Thủ quỹ, kế toán, phục vụ, văn phòng....)"}
        };
        public const long MA_KET_QUA_LAY_DL_EMIS_NOT_MANAGE = 2;
        public const long MA_KET_QUA_LAY_DL_EMIS_SUCESS = 1;
        public const long MA_KET_QUA_LAY_DL_EMIS_ERROR = 0;

        public const string CAP_AP_DUNG_PHONG = "999";
        public const string CAP_AP_DUNG_SO = "9999";
        public static string PROVINCE_CODE_SYNC = ConfigurationManager.AppSettings["PROVINCE_CODE_SYNC"];
        public static long SMAS_ID = Int64.Parse(ConfigurationManager.AppSettings["SMAS_ID"]);
        public const string MA_DAN_TOC_KHAC = "99";
    }
}

