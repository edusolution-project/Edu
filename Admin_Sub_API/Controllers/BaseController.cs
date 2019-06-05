using Data.Access.Object.Entities.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using SME.Bussiness.Lib.Dto;
using SME.Bussiness.Lib.Service;
using SME.Utils.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Viettel.Business.BusinessServices;

namespace SME.API.Controllers
{

    public class BaseController : ControllerBase
    {

        protected readonly ISession _session;
        private  SMEEntities _context;
        protected readonly IConfiguration _configuration;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        public string _webRootPath;
        public string _contentRootPath;


        public BaseController(IHttpContextAccessor httpContextAccessor, SMEEntities context, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _context = context;
            _configuration = configuration;
            _webRootPath = hostingEnvironment.WebRootPath;
            _contentRootPath = hostingEnvironment.ContentRootPath;
            _httpContextAccessor = httpContextAccessor;
        }

        
        public SMEEntities DbContext
        {
            get
            {
                if (_context == null)
                {
                    _context = new SMEEntities();
                }
                return _context;
            }
        }

 

        TruongHocService _TruongHocService;
        public TruongHocService TruongHocService
        {
            get
            {
                if (_TruongHocService == null)
                {
                    _TruongHocService = new TruongHocService(DbContext);
                }
                return _TruongHocService;
            }
        }

        ImageDonviService _ImageDonviService;
        public ImageDonviService ImageDonviService
        {
            get
            {
                if (_ImageDonviService == null)
                {
                    _ImageDonviService = new ImageDonviService(DbContext);
                }
                return _ImageDonviService;
            }
        }

        DonViService _DonViService;
        public DonViService DonViService
        {
            get
            {
                if (_DonViService == null)
                {
                    _DonViService = new DonViService(DbContext);
                }
                return _DonViService;
            }
        }

        ChDayDuLieuService daydulieuSevice;
        public ChDayDuLieuService DayDuLieuService
        {
            get
            {
                if (daydulieuSevice == null) daydulieuSevice = new ChDayDuLieuService(DbContext);
                return daydulieuSevice;
            }
        }

        ChDdlDotService _DdlDotService;
        public ChDdlDotService DayDuLieuDotService
        {
            get
            {
                if (_DdlDotService == null) _DdlDotService = new ChDdlDotService(DbContext);
                return _DdlDotService;
            }
        }

        ChDdlTruongHocService ddlTruongHocService;
        public ChDdlTruongHocService DdlTruongHocService
        {
            get
            {
                if (ddlTruongHocService == null)
                {
                    ddlTruongHocService = new ChDdlTruongHocService(DbContext);
                }
                return ddlTruongHocService;
            }
        }
        QuanHuyenService _QuanHuyenService;
        public QuanHuyenService QuanHuyenService
        {
            get
            {
                if (_QuanHuyenService == null)
                {
                    _QuanHuyenService = new QuanHuyenService(DbContext);
                }
                return _QuanHuyenService;
            }
        }

        YcDdlTruongHocService YcDdlTruongHoc;
        public YcDdlTruongHocService YcDdlTruongHocService
        {
            get
            {
                if (YcDdlTruongHoc == null)
                {
                    YcDdlTruongHoc = new YcDdlTruongHocService(DbContext);
                }
                return YcDdlTruongHoc;
            }
        }

        DongBoDuLieuService dongboDuLieuService;
        public DongBoDuLieuService DongBoTruongHocService
        {
            get
            {
                if (dongboDuLieuService == null) dongboDuLieuService = new DongBoDuLieuService(DbContext);
                return dongboDuLieuService;
            }
        }
        public YcDdlTruongHocService DongBoSoService
        {
            get
            {
                if (YcDdlTruongHoc == null) YcDdlTruongHoc = new YcDdlTruongHocService(DbContext);
                return YcDdlTruongHoc;
            }
        }
        ChDdlTruongHocService _ChDdlTruongHoc;
        public ChDdlTruongHocService ChDdlTruongHocService
        {
            get
            {
                if (_ChDdlTruongHoc == null) _ChDdlTruongHoc = new ChDdlTruongHocService(DbContext);
                return _ChDdlTruongHoc;
            }
        }

        LichSuDayDlService lichSuDayDLService;
        public LichSuDayDlService LichSuDayDlService
        {
            get
            {
                if (lichSuDayDLService == null) lichSuDayDLService = new LichSuDayDlService(DbContext);
                return lichSuDayDLService;
            }
        }

        DongBoDuLieuService dongBoDuLieu;
        public DongBoDuLieuService DongBoDLService
        {
            get
            {
                if (dongBoDuLieu != null) dongBoDuLieu = new DongBoDuLieuService(DbContext);
                return dongBoDuLieu;
            }

        }

        PhuongXaService _PhuongXaService;
        public PhuongXaService PhuongXaService
        {
            get
            {
                if (_PhuongXaService == null)
                {
                    _PhuongXaService = new PhuongXaService(DbContext);
                }
                return _PhuongXaService;
            }
        }

        MonHocService _MonHocService;
        public MonHocService MonHocService
        {
            get
            {
                if (_MonHocService == null)
                {
                    _MonHocService = new MonHocService(DbContext);
                }
                return _MonHocService;
            }
        }

        DanTocService _DanTocService;
        public DanTocService DanTocService
        {
            get
            {
                if (_DanTocService == null)
                {
                    _DanTocService = new DanTocService(DbContext);
                }
                return _DanTocService;
            }
        }

        DuLieuTruongService _DuLieuTruongService;
        public DuLieuTruongService DuLieuTruongService
        {
            get
            {
                if (_DuLieuTruongService == null)
                {
                    _DuLieuTruongService = new DuLieuTruongService(DbContext);
                }
                return _DuLieuTruongService;
            }
        }

        CnganhDtaoService _CnganhDtaoService;
        public CnganhDtaoService CnganhDtaoService
        {
            get
            {
                if (_CnganhDtaoService == null)
                {
                    _CnganhDtaoService = new CnganhDtaoService(DbContext);
                }
                return _CnganhDtaoService;
            }
        }
        ToThonService _ToThonService;
        public ToThonService ToThonService
        {
            get
            {
                if (_ToThonService == null)
                {
                    _ToThonService = new ToThonService(DbContext);
                }
                return _ToThonService;
            }
        }

        NguoiDungService _NguoiDungService;
        public NguoiDungService NguoiDungService
        {
            get
            {
                if (_NguoiDungService == null)
                {
                    _NguoiDungService = new NguoiDungService(DbContext);
                }
                return _NguoiDungService;
            }
        }
        DoiTacService _DoiTacService;
        public DoiTacService DoiTacService
        {
            get
            {
                if (_DoiTacService == null)
                {
                    _DoiTacService = new DoiTacService(DbContext);
                }
                return _DoiTacService;
            }
        }

        DsachServiceService _DsachServiceService;
        public DsachServiceService DsachServiceService
        {
            get
            {
                if (_DsachServiceService == null)
                {
                    _DsachServiceService = new DsachServiceService(DbContext);
                }
                return _DsachServiceService;
            }
        }

        PquyenHTDTService _PquyenService;
        public PquyenHTDTService PquyenHTDTService
        {
            get
            {
                if (_PquyenService == null)
                {
                    _PquyenService = new PquyenHTDTService(DbContext);
                }
                return _PquyenService;
            }
        }

        DtuongCsachService _DtuongCsachService;
        public DtuongCsachService DtuongCsachService
        {
            get
            {
                if (_DtuongCsachService == null)
                {
                    _DtuongCsachService = new DtuongCsachService(DbContext);
                }
                return _DtuongCsachService;
            }
        }

        HocSinhService _HocSinhService;
        public HocSinhService HocSinhService
        {
            get
            {
                if (_HocSinhService == null)
                {
                    _HocSinhService = new HocSinhService(DbContext);
                }
                return _HocSinhService;
            }
        }

        LyDoBoHocService _LyDoBoHocService;
        public LyDoBoHocService LyDoBoHocService
        {
            get
            {
                if (_LyDoBoHocService == null)
                {
                    _LyDoBoHocService = new LyDoBoHocService(DbContext);
                }
                return _LyDoBoHocService;
            }
        }

        GiaoVienService _GiaoVienService;
        public GiaoVienService GiaoVienService
        {
            get
            {
                if (_GiaoVienService == null)
                {
                    _GiaoVienService = new GiaoVienService(DbContext);
                }
                return _GiaoVienService;
            }
        }

        LopHocService _LopHocService;
        public LopHocService LopHocService
        {
            get
            {
                if (_LopHocService == null)
                {
                    _LopHocService = new LopHocService(DbContext);
                }
                return _LopHocService;
            }
        }

        HocSinhTheoNamService _HocSinhTheoNamService;
        public HocSinhTheoNamService HocSinhTheoNamService
        {
            get
            {
                if (_HocSinhTheoNamService == null)
                {
                    _HocSinhTheoNamService = new HocSinhTheoNamService(DbContext);
                }
                return _HocSinhTheoNamService;
            }
        }

        CanBoTheoNamService _CanBoTheoNamService;
        public CanBoTheoNamService CanBoTheoNamService
        {
            get
            {
                if (_CanBoTheoNamService == null)
                {
                    _CanBoTheoNamService = new CanBoTheoNamService(DbContext);
                }
                return _CanBoTheoNamService;
            }
        }
        TongKetHsService _TongKetHService;
        public TongKetHsService TongKetHService
        {
            get
            {
                if (_TongKetHService == null)
                {
                    _TongKetHService = new TongKetHsService(DbContext);
                }
                return _TongKetHService;
            }
        }

        LoaiDoiTuongService _LoaiDoiTuongService;
        public LoaiDoiTuongService LoaiDoiTuongService
        {
            get
            {
                if (_LoaiDoiTuongService == null)
                {
                    _LoaiDoiTuongService = new LoaiDoiTuongService(DbContext);
                }
                return _LoaiDoiTuongService;
            }
        }

        TieuChiDLService _TieuChiDLService;
        public TieuChiDLService TieuChiDLService
        {
            get
            {
                if (_TieuChiDLService == null)
                {
                    _TieuChiDLService = new TieuChiDLService(DbContext);
                }
                return _TieuChiDLService;
            }
        }

        ChDuLieuListService _ChDuLieuListService;
        public ChDuLieuListService ChDuLieuListService
        {
            get
            {
                if (_ChDuLieuListService == null)
                {
                    _ChDuLieuListService = new ChDuLieuListService(DbContext);
                }
                return _ChDuLieuListService;
            }
        }

        LichSuDongBoTieuChiEMISService lichSuDongBoTieuChiEMISService;
        public LichSuDongBoTieuChiEMISService LichSuDongBoTieuChiEMISService
        {
            get
            {
                if (lichSuDongBoTieuChiEMISService == null) lichSuDongBoTieuChiEMISService = new LichSuDongBoTieuChiEMISService(DbContext);
                return lichSuDongBoTieuChiEMISService;
            }
        }


        CauHinhBaoCaoDongService cauHinhBaoCaoDongService;
        public CauHinhBaoCaoDongService CauHinhBaoCaoDongService
        {
            get
            {
                if (cauHinhBaoCaoDongService == null) cauHinhBaoCaoDongService = new CauHinhBaoCaoDongService(DbContext);
                return cauHinhBaoCaoDongService;
            }
        }

        //HieuNN
        CauHinhEmailService cauHinhEmailService;
        public CauHinhEmailService CauHinhEmailService
        {
            get
            {
                if (cauHinhEmailService == null) cauHinhEmailService = new CauHinhEmailService(DbContext);
                return cauHinhEmailService;
            }
        }

        CauHinhTempEmailService cauHinhTempEmailService;
        public CauHinhTempEmailService CauHinhTempEmailService
        {
            get
            {
                if (cauHinhTempEmailService == null) cauHinhTempEmailService = new CauHinhTempEmailService(DbContext);
                return cauHinhTempEmailService;
            }
        }
        //

        protected ModelStateDictionary clearException(ModelStateDictionary input)
        {
            ModelStateDictionary resvals = new ModelStateDictionary();
            List<string> keys = input.Keys.ToList();
            foreach (string keyItem in keys)
            {
                
                //ModelMetadata modelStateOut = null;
                ModelStateEntry modelState = null;
                input.TryGetValue(keyItem, out modelState);
                if (modelState != null)
                {
                    ///modelStateOut = new ModelMetadata();
                    foreach (var error in modelState.Errors)
                    {
                        //ModelError modelError = new ModelError(error.ErrorMessage);
                        //modelStateOut. Errors.Add(modelError);
                        resvals.AddModelError(keyItem, error.ErrorMessage);
                    }
                    //resvals.AddModelError(keyItem, modelStateOut);// Add(keyItem, modelStateOut);
                }
            }
            return resvals;
        }


        //Thuonglv3
        public virtual B GetService<B>()
        {
            try
            {
                return (B)typeof(B).GetConstructor(new Type[] { typeof(SMEEntities) }).Invoke(new object[] { this.DbContext });
            }
            catch
            {
                return default(B);
            }
        }

        private NguoiDungDto _UserInfo;
        public NguoiDungDto GetUserInfo()
        {
            if (_UserInfo == null)
            {
                string loginName = GetHeader("Username");
                _UserInfo = GetService<NguoiDungService>().FindUser(loginName);
            }
            return _UserInfo;
        }

        public string GetHeader(string key)
        {
            Microsoft.Extensions.Primitives.StringValues headerValues;
            //IEnumerable<string> headerValues;
            var value = string.Empty;
            if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue(key, out headerValues))//TryGetValues(key, out headerValues))
            {
                value = headerValues.FirstOrDefault();
            }
            return value;
        }

        public string GetPartnerLoginName()
        {
            return GetHeader("Username").ToUpper();
        }

        public bool IsValidTruongHoc(string maTruongHoc)
        {
            string userName = GetPartnerLoginName();
            HE_THONG_DOI_TAC dt = GetService<DoiTacService>().FindPartner(userName);
            if (dt != null)
            {
                if (dt.TAT_CA.GetValueOrDefault() == GlobalConstants.TRUE)
                {
                    return true;
                }
                return GetService<DoiTacTruongHocService>().IsValidTruongHoc(maTruongHoc, dt.HE_THONG_DOI_TAC_ID);
            }
            return false;
        }

        public bool IsValidTruongHoc(long ycDdlTruongHocId)
        {
            string userName = GetPartnerLoginName();
            YC_DDL_TRUONG_HOC yc = GetService<YcDdlTruongHocService>().Find(ycDdlTruongHocId);
            HE_THONG_DOI_TAC dt = GetService<DoiTacService>().FindPartner(userName);
            if (yc != null && dt != null)
            {
                if(dt.TAT_CA.GetValueOrDefault() == GlobalConstants.TRUE)
                {
                    return true;
                }
                return GetService<DoiTacTruongHocService>().IsValidTruongHoc(yc.MA_TRUONG_HOC, dt.HE_THONG_DOI_TAC_ID);
            }
            return false;
        }

        public DateTime BeginRequestTime { get; set; }

    }
}