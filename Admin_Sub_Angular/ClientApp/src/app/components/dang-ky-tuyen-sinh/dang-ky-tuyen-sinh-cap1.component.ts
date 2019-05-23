import { Component, OnInit, ViewChild, TemplateRef, OnChanges } from '@angular/core';
import { Restangular } from "ngx-restangular";
import { DomSanitizer } from '@angular/platform-browser';
import { GlobalService } from '../../services/global.service';
import { ActivatedRoute, Router } from '@angular/router';
import { INgxSelectOption } from 'ngx-select-ex';
import * as $ from 'jquery';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { AppComponent } from '../app.component';
import { ModalDirective, BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
@Component({
  selector: 'dang-ky-tuyen-sinh-cap1',
  templateUrl: './dang-ky-tuyen-sinh-cap1.component.html'
})
export class DangKyTuyenSinhCap1Component implements OnInit {
  listDotTuyenSinh1: dotTuyenSinhdto = new dotTuyenSinhdto();
  captcha: Captcha = new Captcha('', '');
  entity: dotTuyenSinhdto = new dotTuyenSinhdto(this.captcha);
  dotTuyenSinhId;
  show1 = true;
  show2 = true;
  url = '../assets/chuacothongtin.pdf';
  camTraiTuyen;
  dungTuyen;
  image;

  public formResetToggle = true;
  regexName = /^[a-zA-Z\d\ÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀẾỂ ưăạảấầẩẫậắằẳẵặẹẻẽềếềểỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵỷỹ\-!(),.:']+$/;
  regexPhone = /^(?:0|\\+)[0-9\\s.\\/-]{9,20}$/;
  regexEmail = /^(([^<>()\[\]\.,;:\s@\"]+(\.[^<>()\[\]\.,;:\s@\"]+)*)|(\".+\"))@(([^<>()[\]\.,;:\s@\"]+\.)+[^<>()[\]\.,;:\s@\"]{2,})$/i;
  currentYear: any = "";
  lstFile = [];
  soGiaoDucData = [];
  config = [];
  lstNamSinh = [];
  public lstNamHoc;
  public lstGioiTinh;
  public lstTinhThanh;
  public lstQuanHuyen;
  public lstPhuongXa;
  public lstToThon;
  public lstTinhThanhCuTru;
  public lstQuanHuyenCuTru;
  public lstPhuongXaCuTru;
  public lstToThonCuTru;
  public lstDanToc;
  public lstTruongHoc;
  public lstTruongHocSelect2;
  public lstHocSinh;
  public ThoiGianDky: dotTuyenSinhdto = new dotTuyenSinhdto();;
  showdetail = true;
  chon = "--Chọn--";
  tinhThanhService = this.restangular.all('TinhThanh');
  quanHuyenService = this.restangular.all('QuanHuyen');
  phuongXaService = this.restangular.all('PhuongXa');
  toThonService = this.restangular.all('ToThon');
  danTocService = this.restangular.all('DanToc');
  truongHocService = this.restangular.all('TruongHoc');
  dotTuyenSinhService = this.restangular.all('dotTuyenSinh');

  flagMaTthanhThtru = false;
  flagMaQhuyenThtru = false;
  flagMaPxaThtru = false;
  flagMaTthanhCuTru = false;
  flagMaQhuyenCuTru = false;
  flagMaPxaCuTru = false;
  constructor(private route: ActivatedRoute, private router: Router,
    private restangular: Restangular,
    private saniti: DomSanitizer,
    private globalServices: GlobalService,
    private alertService: AlertService,
    private modalService: BsModalService,
    private appComponent: AppComponent) {

  }

  @ViewChild('tempHoTen') hoTen: TemplateRef<any>;
  @ViewChild('tempNoiSinh') noiSinh: TemplateRef<any>;
  @ViewChild('tempHoKhauThuongTru') hoKhauThuongTru: TemplateRef<any>;
  @ViewChild('tempHoKhauTamTru') hoKhauTamTru: TemplateRef<any>;
  @ViewChild('tempTenChuHo') tenChuHo: TemplateRef<any>;
  @ViewChild('tempSongCung') songCung: TemplateRef<any>;
  @ViewChild('tempSoDienThoaiNhaRieng') soDienThoaiNhaRieng: TemplateRef<any>;
  @ViewChild('tempKhiCanBaoTin') khiCanBaoTin: TemplateRef<any>;
  @ViewChild('tempDaQuaMauGiao') daQuaMauGiao: TemplateRef<any>;
  @ViewChild('tempThongTinBo') thongTinBo: TemplateRef<any>;
  @ViewChild('tempThongTinMe') thongTinMe: TemplateRef<any>;
  @ViewChild('tempThongTinNguoiNuoiDuong') thongTinNguoiNuoiDuong: TemplateRef<any>;
  @ViewChild('tempChonTruong') chonTruong: TemplateRef<any>;
  @ViewChild('tempDangKyHocBanTru') dangKyHocBanTru: TemplateRef<any>;
  @ViewChild('tempTepDinhKem') tepDinhKem: TemplateRef<any>;
  @ViewChild('tempEmail') email: TemplateRef<any>;
  @ViewChild('tempCamKet') camKet: TemplateRef<any>;
  @ViewChild('tempGhiChu') ghiChu: TemplateRef<any>;
  ngOnInit() {
    let arr = ['hoTen', 'noiSinh', 'hoKhauThuongTru', 'hoKhauTamTru', 'tenChuHo', 'songCung', 'soDienThoaiNhaRieng', 'khiCanBaoTin'
      , 'daQuaMauGiao', 'thongTinBo', 'thongTinMe', 'thongTinNguoiNuoiDuong', 'chonTruong', 'dangKyHocBanTru', 'tepDinhKem', 'email', 'camKet', 'ghiChu'];
    for (var i = 0; i < arr.length; i++) {
      this.config.push({ name: arr[i] });
    }
    this.dotTuyenSinhId = this.route.snapshot.paramMap.get("id")
    this.lstNamHoc = this.globalServices.getlistNamHoc();
    this.lstGioiTinh = this.globalServices.lstGioiTinh;
    this.currentYear = this.globalServices.currentYear;
    this.listDotTuyenSinh1.NgaySinh = "";
    this.entity.GioiTinh = "";
    this.entity.MaTthanhThtru = this.globalServices.soGiaoDucData.MaTinh;
    this.entity.MaQhuyenThtru = ""
    this.entity.MaPxaThtru = "";
    this.entity.TenHocSinh = "";
    this.entity.NgaySinhSearchUTC = "";
    //this.entity.MaTthonThtru = "N351001006001";
    this.entity.MaTthanhCuTru = this.globalServices.soGiaoDucData.MaTinh;
    this.entity.MaQhuyenCuTru = ""
    this.entity.MaPxaCuTru = "";
    this.entity.MaTthonCuTru = "";
    this.entity.CoHocMgiao = "1";
    this.entity.DkyHocBanTru = "2";
    this.lstNamSinh = this.globalServices.bindAgeYear();
    this.dotTuyenSinhService.all('GetById')
      .post({ 'DotTuyenSinhId': this.dotTuyenSinhId })
      .subscribe(data => {
        this.ThoiGianDky = data;
      }, error => {
        console.log(error);
      });

    this.tinhThanhService.all("GetAll").post().subscribe(
      response => {
        this.lstTinhThanh = this.globalServices.convertObjectToSelect2(response, 'MaTinhThanh', 'TenTinhThanh', "--Chọn--");
        this.lstTinhThanhCuTru = this.globalServices.convertObjectToSelect2(response, 'MaTinhThanh', 'TenTinhThanh', "--Chọn--");
        this.entity.NoiSinh = this.globalServices.soGiaoDucData.MaTinh;
        this.entity.MaTthanhThtru = this.globalServices.soGiaoDucData.MaTinh;
        this.entity.MaTthanhCuTru = this.globalServices.soGiaoDucData.MaTinh;
      }, error => {
        console.log(error);
      }
    );
    this.quanHuyenService.all("GetByMaTinhThanh").post({ 'MaTinhThanh': this.entity.MaTthanhThtru }).subscribe(
      response => {
        this.lstQuanHuyen = this.globalServices.convertObjectToSelect2(response, 'MaQuanHuyen', 'TenQuanHuyen', '--Chọn--');
        this.lstQuanHuyenCuTru = this.globalServices.convertObjectToSelect2(response, 'MaQuanHuyen', 'TenQuanHuyen', '--Chọn--');
      }, error => {
        console.log(error);
      }
    );


    this.phuongXaService.all("GetByMaQuanHuyen").post({ 'MaQuanHuyen': this.entity.MaQhuyenThtru }).subscribe(
      response => {
        this.lstPhuongXa = this.globalServices.convertObjectToSelect2(response, 'MaPhuongXa', 'TenPhuongXa', "--Chọn--");
        this.lstPhuongXaCuTru = this.globalServices.convertObjectToSelect2(response, 'MaPhuongXa', 'TenPhuongXa', "--Chọn--");
      }, error => {
        console.log(error);
      }
    );
    this.danTocService.all("GetAll").post().subscribe(
      response => {
        this.lstDanToc = this.globalServices.convertObjectToSelect2(response, 'MaDanToc', 'TenDanToc', '--Chọn--');
        this.entity.MaDanToc = "01";
      }, error => {
        console.log(error);
      }
    );

    this.truongHocService.all("SearchDungTuyen").post({ 'DotTuyenSinh': this.dotTuyenSinhId }).subscribe(
      response => {
        this.lstTruongHoc = response;
        this.lstTruongHocSelect2 = this.globalServices.convertObjectToSelect2(response, 'MaTruongHoc', 'TenTruongHoc', '--Chọn--');
        this.entity.MaTruong = "";
      }, error => {
        console.log(error);
      }
    );
    this.refreshCapcha();
  }


  doMaTthanhThtru(options: INgxSelectOption[]) {
    let MaTthanhThtru: string | number = '-1';
    if (options != undefined && options.length > 0) {
      MaTthanhThtru = options[0].value;
    }
    else {
      MaTthanhThtru = this.entity.MaTthanhThtru
    }

    if (this.flagMaTthanhThtru === true && this.bSearchHocsinh === false) {
      this.quanHuyenService.all("GetByMaTinhThanh").post({ 'MaTinhThanh': MaTthanhThtru }).subscribe(
        response => {
          this.lstQuanHuyen = this.globalServices.convertObjectToSelect2(response, 'MaQuanHuyen', 'TenQuanHuyen', '--Chọn--');
          if (response.length > 0 && !this.bSearchHocsinh) {
            $('#dsQuanHuyen .ngx-select').trigger('click');
            setTimeout(() => {
              $('#dsQuanHuyen .ngx-select__choices').addClass('show');
            }, 100);
          }
          else {
            this.entity.MaQhuyenThtru = "";
          }
        }, error => {
          console.log(error);
        }
      );
    }
    else {
      this.flagMaTthanhThtru = true;
    }

  }
  doMaQhuyenThtru(options: INgxSelectOption[]) {
    $('#dsQuanHuyen .ngx-select__choices').removeClass('show');
    let MaQhuyenThtru: string | number = '-1';
    if (options != undefined && options.length > 0) {
      MaQhuyenThtru = options[0].value;
    }
    else {
      MaQhuyenThtru = this.entity.MaQhuyenThtru;
    }
    if (this.flagMaQhuyenThtru == true && this.bSearchHocsinh === false) {
      this.phuongXaService.all("GetByMaQuanHuyen").post({ 'MaQuanHuyen': MaQhuyenThtru }).subscribe(
        response => {
          this.lstPhuongXa = this.globalServices.convertObjectToSelect2(response, 'MaPhuongXa', 'TenPhuongXa', "--Chọn--");
          if (response.length > 0 && !this.bSearchHocsinh) {
            $('#dsPhuongXa .ngx-select').trigger('click');
            setTimeout(() => {
              $('#dsPhuongXa .ngx-select__choices').addClass('show');
            }, 100);
          }
          else {
            this.entity.MaPxaThtru = "";
          }
        }, error => {
          console.log(error);
        }
      );
    }
    else {
      this.flagMaQhuyenThtru = true;
    }
  }
  doMaPxaThtru(options: INgxSelectOption[]) {
    $('#dsPhuongXa .ngx-select__choices').removeClass('show');

    let MaPxaThtru: string | number = '-1';
    if (options != undefined && options.length > 0) {
      MaPxaThtru = options[0].value;
    }

    if (this.flagMaPxaThtru == true && this.bSearchHocsinh === false) {
      this.toThonService.all("GetByMaPhuongXa").post({ 'MaPhuongXa': MaPxaThtru }).subscribe(
        response => {
          this.lstToThon = this.globalServices.convertObjectToSelect2(response, 'MaToThon', 'TenToThon', '--Chọn--');
          if (response.length > 0 && !this.bSearchHocsinh) {
            $('#dsToThon .ngx-select').trigger('click');
            setTimeout(() => {
              $('#dsToThon .ngx-select__choices').addClass('show');
            }, 100);
          }
          else {
            this.entity.MaTthonThtru = "";
          }
        }, error => {
          console.log(error);
        }
      );
    }
    else {
      this.flagMaPxaThtru = true;
    }
  }
  doMaTthonThtru(options: INgxSelectOption[]) {
    $('#dsToThon .ngx-select__choices').removeClass('show');
    if (options != undefined && options.length > 0) {
      this.entity.MaTthonThtru = options[0].value+"";
      this.loadDungTuyen(options[0].value);
      this.checkCamTraiTuyen(this.entity.MaTruong);
      this.checkDungTuyen(this.entity.MaTruong);
    }
  }

  doMaTthanhCuTru(options: INgxSelectOption[]) {
    let MaTthanhCuTru: string | number = '-1';
    if (options != undefined && options.length > 0) {
      MaTthanhCuTru = options[0].value;
    }
    else {
      MaTthanhCuTru = this.entity.MaTthanhCuTru
    }

    if (this.flagMaTthanhCuTru == true && this.bSearchHocsinh === false) {
      this.quanHuyenService.all("GetByMaTinhThanh").post({ 'MaTinhThanh': MaTthanhCuTru }).subscribe(
        response => {
          this.lstQuanHuyenCuTru = this.globalServices.convertObjectToSelect2(response, 'MaQuanHuyen', 'TenQuanHuyen', '--Chọn--');
          if (response.length > 0 && !this.bSearchHocsinh) {
            $('#dsQuanHuyenCuTru .ngx-select').trigger('click');
            setTimeout(() => {
              $('#dsQuanHuyenCuTru .ngx-select__choices').addClass('show');
            }, 100);
          }
          else {
            this.entity.MaQhuyenCuTru = "";
          }
        }, error => {
          console.log(error);
        }
      );
    }
    else {
      this.flagMaTthanhCuTru = true;
    }

  }
  doMaQhuyenCuTru(options: INgxSelectOption[]) {
    $('#dsQuanHuyenCuTru .ngx-select__choices').removeClass('show');
    let MaQhuyenCuTru: string | number = '-1';
    if (options != undefined && options.length > 0) {
      MaQhuyenCuTru = options[0].value;
    }
    if (this.flagMaQhuyenCuTru == true && this.bSearchHocsinh === false) {
      this.phuongXaService.all("GetByMaQuanHuyen").post({ 'MaQuanHuyen': MaQhuyenCuTru }).subscribe(
        response => {
          this.lstPhuongXaCuTru = this.globalServices.convertObjectToSelect2(response, 'MaPhuongXa', 'TenPhuongXa', "--Chọn--");
          if (response.length > 0 && !this.bSearchHocsinh) {
            $('#dsPhuongXaCuTru .ngx-select').trigger('click');
            setTimeout(() => {
              $('#dsPhuongXaCuTru .ngx-select__choices').addClass('show');
            }, 100);
          }
          else {
            this.entity.MaPxaCuTru = "";
          }
        }, error => {
          console.log(error);
        }
      );
    }
    else {
      this.flagMaQhuyenCuTru = true;
    }
  }
  doMaPxaCuTru(options: INgxSelectOption[]) {
    $('#dsPhuongXaCuTru .ngx-select__choices').removeClass('show');

    let MaPxaCuTru: string | number = '-1';
    if (options != undefined && options.length > 0) {
      MaPxaCuTru = options[0].value;
    }
    if (this.flagMaPxaCuTru == true && this.bSearchHocsinh === false) {
      this.toThonService.all("GetByMaPhuongXa").post({ 'MaPhuongXa': MaPxaCuTru }).subscribe(
        response => {
          this.lstToThonCuTru = this.globalServices.convertObjectToSelect2(response, 'MaToThon', 'TenToThon', '--Chọn--');
          if (response.length > 0 && !this.bSearchHocsinh) {
            $('#dsToThonCuTru .ngx-select').trigger('click');
            setTimeout(() => {
              $('#dsToThonCuTru .ngx-select__choices').addClass('show');
            }, 100);
          }
          else {
            this.entity.MaTthonCuTru = "";
          }
        }, error => {
          console.log(error);
        }
      );
    }
    else {
      this.flagMaPxaCuTru = true;
    }
  }
  doMaTthonCuTru(options: INgxSelectOption[]) {
    $('#dsToThonCuTru .ngx-select__choices').removeClass('show');
    if (options != undefined && options.length > 0) {
      this.entity.MaTthonCuTru = options[0].value + "";
      this.loadDungTuyen(options[0].value);
      this.checkCamTraiTuyen(this.entity.MaTruong);
      this.checkDungTuyen(this.entity.MaTruong);
    }
  }
  doChonTruong(options: INgxSelectOption[]) {
    if (options != undefined && options.length > 0) {
      this.checkCamTraiTuyen(options[0].value);
      this.checkDungTuyen(options[0].value);
    }
  }
  showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  DchiThtruChange(oldVal, newVal) {
    if (newVal && oldVal != "") {
      this.lstQuanHuyenCuTru = this.lstQuanHuyen;
      this.lstPhuongXaCuTru = this.lstPhuongXa;
      this.lstToThonCuTru = this.lstToThon;
      this.entity.DchiCuTru = this.entity.DchiThtru;
      this.entity.MaTthanhCuTru = this.entity.MaTthanhThtru;
      this.flagMaTthanhCuTru = false;
      this.entity.MaQhuyenCuTru = this.entity.MaQhuyenThtru;
      this.flagMaQhuyenCuTru = false;
      this.entity.MaPxaCuTru = this.entity.MaPxaThtru;
      this.flagMaPxaCuTru = false;
      this.entity.MaTthonCuTru = this.entity.MaTthonThtru;
    }
  }
  bSearchHocsinh = false;
  SearchHocsinh(item) {

    this.appComponent.isViewContentLoading = true;
    this.bSearchHocsinh = true;
    if (this.entity.NgaySinhSearchUTC != undefined) {
      this.entity.NgaySinh = this.globalServices.convertLocalDate(this.entity.NgaySinhSearchUTC);
    }
    this.restangular.all("hocSinh").all("GetByCondition").post({
      'TenHocSinh': this.entity.TenHocSinh,
      'NgaySinh': this.entity.NgaySinh,
      'CapHoc': 1,
      'NamHoc': this.globalServices.currentYear,
      'MaTinhThanh': this.globalServices.soGiaoDucData.MaTinh
    }).subscribe(
      data => {
        if (data.length == undefined || data.length === 0) {
          // this.chooseHocSinh(data[0]);
          this.showErrorAlert("", "không tìm thấy học sinh")
        }
        else if (data.length === 1) {
          this.chooseHocSinh(data[0]);
          this.alertService.showMessage("", "Tìm thấy một học sinh", MessageSeverity.success);
        }
        else {
          //  this.chooseHocSinh(data[0]);
          this.lstHocSinh = data;
          this.openModal(this.chonHocSinh);
        }
        this.appComponent.isViewContentLoading = false;
        //this.bSearchHocsinh = false;

      },
      error => {
        this.showErrorAlert("", error.data);
        this.appComponent.isViewContentLoading = false;
        // this.bSearchHocsinh = false;
      }
    );
  }

  chooseHocSinh(item: dotTuyenSinhdto) {
    if (this.modalRef) {
      this.modalRef.hide();
    }
    this.bSearchHocsinh = true;
    let ngaySinh = this.entity.NgaySinhSearchUTC;

    
    this.entity.NgaySinhUTC = this.entity.NgaySinhSearchUTC = ngaySinh;
    this.entity.NoiSinh = item.NoiSinh;
    this.entity.MaDanToc = item.MaDanToc;
    this.entity.DchiThtru = item.DchiThtru;
    this.entity.GioiTinh = item.GioiTinh + "";
    this.entity.DchiCuTru = item.DchiTmtru;
    this.entity.SoDthoaiCdinh = item.SoDthoaiCdinh;
    this.entity.BaoTin = item.BaoTin;
    this.entity.TenTruongCu = item.TenTruongMN;
    if (item.TenTruongMN == "" || item.TenTruongMN == null || item.TenTruongMN) {
      this.entity.CoHocMgiao = "2";
    }
    this.entity.TenCha = item.HoTenBo;
    this.entity.TenMe = item.HoTenMe;
    this.entity.NnghiepCuaCha = item.NgheNghiepBo;
    this.entity.NnghiepCuaMe = item.NgheNghiepMe;
    this.entity.NoiCtacCuaCha = item.NoiLamViecBo;
    this.entity.NoiCtacCuaMe = item.NoiLamViecMe;
    this.entity.DthoaiCuaCha = item.SoDienThoaiBo;
    this.entity.DthoaiCuaMe = item.SoDienThoaiMe;
    this.entity.NamSinhCuaMe = item.NgaySinhMe;
    this.entity.NamSinhCuaCha = item.NgaySinhBo;
    
    this.toThonService.all("GetByMaPhuongXa").post({ 'MaPhuongXa': item.MaPxaThtru }).subscribe(
      response => {
        this.lstToThon = this.globalServices.convertObjectToSelect2(response, 'MaToThon', 'TenToThon', '--Chọn--');
        this.entity.MaTthonThtru = item.MaTthonThtru;
      }
    );
    this.phuongXaService.all("GetByMaQuanHuyen").post({ 'MaQuanHuyen': item.MaQhuyenThtru }).subscribe(
      response => {
        this.lstPhuongXa = this.globalServices.convertObjectToSelect2(response, 'MaPhuongXa', 'TenPhuongXa', "--Chọn--");
        setTimeout(() => {
          this.entity.MaPxaThtru = item.MaPxaThtru;
        },100);
       
      }
    );
    this.quanHuyenService.all("GetByMaTinhThanh").post({ 'MaTinhThanh': item.MaTthanhThtru }).subscribe(
      response => {
        this.lstQuanHuyen = this.globalServices.convertObjectToSelect2(response, 'MaQuanHuyen', 'TenQuanHuyen', '--Chọn--');
        setTimeout(() => {
          this.entity.MaQhuyenThtru = item.MaQhuyenThtru
        },100);
      }
    );

    this.toThonService.all("GetByMaPhuongXa").post({ 'MaPhuongXa': item.MaPxaTmtru }).subscribe(
      response => {
        this.lstToThonCuTru = this.globalServices.convertObjectToSelect2(response, 'MaToThon', 'TenToThon', '--Chọn--');

        setTimeout(() => {
          this.entity.MaTthonCuTru = item.MaTthonTmtru;
        }, 100);
       
      }
    );
    this.phuongXaService.all("GetByMaQuanHuyen").post({ 'MaQuanHuyen': item.MaQhuyenTmtru }).subscribe(
      response => {
        this.lstPhuongXaCuTru = this.globalServices.convertObjectToSelect2(response, 'MaPhuongXa', 'TenPhuongXa', "--Chọn--");
        setTimeout(() => {
          this.entity.MaPxaCuTru = item.MaPxaTmtru;
        }, 100);
      });
    this.quanHuyenService.all("GetByMaTinhThanh").post({ 'MaTinhThanh': item.MaTthanhTmtru }).subscribe(
      response => {
        this.lstQuanHuyenCuTru = this.globalServices.convertObjectToSelect2(response, 'MaQuanHuyen', 'TenQuanHuyen', '--Chọn--');
        setTimeout(() => {
          this.entity.MaQhuyenCuTru = item.MaQhuyenTmtru
        },100);
      }
    );

    this.entity.MaTthanhThtru = item.MaTthanhThtru;
    this.entity.MaTthanhCuTru = item.MaTthanhTmtru;
   


    setTimeout(() => {
      this.bSearchHocsinh = false;
    }, 5000);
  }

  checkCamTraiTuyen(MaTruong: string | number) {
    if (MaTruong != "") {
      for (var i = 0; i < this.lstTruongHoc.length; i++) {
        if (this.lstTruongHoc[i].MaTruongHoc == MaTruong) {
          this.camTraiTuyen = this.lstTruongHoc[i].CamTraiTuyen;
          return;
        }
      }
    }
    else {
      this.camTraiTuyen = "";
    }
  }
  checkDungTuyen(MaTruong: string | number) {
    
    let lstMaToThonThuongTru, lstMaToThonTamTru;
    let bThuongTru, bTamTru;
    if (MaTruong) {
      var tthonThtru = this.entity.MaTthonThtru;
      var tthonCuTru = this.entity.MaTthonCuTru;
      for (var i = 0; i < this.lstTruongHoc.length; i++) {
        if (this.lstTruongHoc[i].MaTruongHoc == MaTruong) {
          lstMaToThonThuongTru = this.lstTruongHoc[i].LstMaToThonThuongTru;
          lstMaToThonTamTru = this.lstTruongHoc[i].LstMaToThonTamTru;
          break;
        }
      }
      if (tthonThtru && lstMaToThonThuongTru) {
        bThuongTru = lstMaToThonThuongTru.includes(tthonThtru);
      }
      if (tthonCuTru && lstMaToThonTamTru) {
        bTamTru = lstMaToThonTamTru.includes(tthonCuTru);
      }
     
      if (bThuongTru || bTamTru) {
        this.dungTuyen = "";
        this.entity.DungTuyen = "1";
      }

      else {
        this.dungTuyen = "1";
        this.entity.DungTuyen = "0";
      }


    }
    else {
      this.dungTuyen = "";
    }
  }

  loadDungTuyen(maToThon: string | number) {
    if (maToThon) {
      if (!this.entity.MaTruong) {
        this.truongHocService.all('SearchOneDungTuyen')
          .post({
            'MaToThon': maToThon,
            'MaTinhThanh': this.globalServices.soGiaoDucData.MaTinh,
            'DotTuyenSinh': this.dotTuyenSinhId
          })
          .subscribe(data => {
            if (data.MaTruongHoc) {
              this.entity.MaTruong = data.MaTruongHoc;
            }
          }, error => {
            console.log(error);
          });
      }
    }
  };
  addFile(item) {
    let maxMb = 10;
    let maxLength = maxMb * 1024 * 1024;
    let totalLength = 0;
    for (var i = 0; i < item.length; i++) {
      if (item[i].type == "application/pdf" || item[i].type == "image/jpeg") {
        this.lstFile.push(item[i]);
      }
      else
        return;
    }
    for (var i = 0; i < this.lstFile.length; i++) {
      totalLength += this.lstFile[i].size;
      if (totalLength > maxLength)
        this.lstFile.pop();
    }

  }
  deleteFile(idx) {
    if (idx >= 0) {
      this.lstFile.splice(idx, 1);
    }
  }
  quaylai() {
    $('#divDangKyHoSo').css("display", "");
    this.showdetail = !this.showdetail;
  }
  chuyenDangKy(form) {


    //if ($scope.formDky.$invalid || !$scope.accept) {
    //  $scope.invalidate = true;
    //  $scope.refreshCapcha();
    //} else {
    //  $scope.invalidate = false;
    //  $scope.showdetail = true;
    //  $scope.mappingNameForView();
    //}

    if (form.valid) {
      $('#divDangKyHoSo').css("display", "none");
      this.showdetail = !this.showdetail;
    }
    else {
      this.showErrorAlert("", "Chưa nhập đầy đủ thông tin");
      this.refreshCapcha();
    }

  }
  xacNhanDangKy() {
    this.appComponent.isViewContentLoading = true;
    let formData = new FormData();
    for (let i = 0; i < this.lstFile.length; i++) {
      formData.append('file' + i, this.lstFile[i], this.lstFile[i].name);
    }
    this.entity.NgaySinh = this.globalServices.convertLocalDate(this.entity.NgaySinhUTC);
    Object.keys(this.entity).forEach(item => {
      formData.append('data[' + item + ']', this.entity[item]);
    });
    formData.append('data[Captcha][Captcha]', this.entity.Captcha.Captcha);
    formData.append('data[Captcha][Key]', this.entity.Captcha.Key);
    formData.append('data[DotTuyenSinhId]', this.dotTuyenSinhId);
    this.restangular.all('HoSoDuTuyen').all("DangKy").customPOST(formData, undefined, undefined, { 'Content-Type': undefined }).subscribe(
      response => {
        this.globalServices.hoSoThanhCong = response;
        console.log(response);
        this.router.navigate(['/dk-thanh-cong']);
        this.appComponent.isViewContentLoading = false;
      }, error => {
        console.log(error);
        this.showErrorAlert("", error.data);
        this.appComponent.isViewContentLoading = false;

      }
    );
  }
  refreshCapcha() {
    this.restangular.all('Util').all("GetCaptcha").post().subscribe(
      response => {
        this.image = this.saniti.bypassSecurityTrustUrl('data:image/JPEG;base64,' + response.EncodedData);
        this.entity.Captcha.Key = response.FileName;
      }
      , error => {
        console.log(error);
      }
    );
  }
  @ViewChild('ChonHocSinh') chonHocSinh: TemplateRef<any>;
  modalRef: BsModalRef;
  openModal(template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template, { class: 'modal-lg' });
    $('.modal-content').css('display', 'block')
  }

}
export class dotTuyenSinhdto {
  DotTuyenSinhId: string;
  active: boolean;
  NgaySinh: any;
  GioiTinh: string;
  NoiSinh: string;
  MaDanToc: string;
  MaTthanhThtru: any;
  TenHocSinh: string;
  MaQhuyenThtru: any;
  MaPxaThtru: string;
  MaTthonThtru: string;
  MaTthanhCuTru: any;
  MaQhuyenCuTru: string;
  MaPxaCuTru: string;
  MaTthonCuTru: string;
  NgaySinhUTC: any;
  NgaySinhSearchUTC: any;
  MaTinhThanh: string;
  CoHocMgiao: string;
  DkyHocBanTru: string;
  MaTruong: string;
  NgayBatDau: any;
  NgayKetThuc: any;
  DchiThtru: string;
  DungTuyen: string;
  DchiCuTru: string;
  Captcha: Captcha;
  DchiTmtru: string;
  TenTruongCu: any;
  TenTruongMN: any;
  NnghiepCuaCha: any;
  TenCha: any;
  HoTenBo: any;
  TenMe: any;
  HoTenMe: any;
  NgheNghiepBo: any;
  NgheNghiepMe: any;
  NnghiepCuaMe: any;
  NoiCtacCuaCha: any;
  NoiCtacCuaMe: any;
  DthoaiCuaCha: any;
  DthoaiCuaMe: any;
  NamSinhCuaMe: any;
  NamSinhCuaCha: any;
  NoiLamViecBo: any;
  NoiLamViecMe: any;
  SoDienThoaiBo: any;
  SoDienThoaiMe: any;
  NgaySinhMe: any;
  NgaySinhBo: any;
    SoDthoaiCdinh: any;
    BaoTin: any;
    MaPxaTmtru: any;
    MaQhuyenTmtru: any;
    MaTthanhTmtru: any;
    MaTthonTmtru: string;
  constructor(Captcha?: Captcha) {
    this.Captcha = Captcha;
  }
}
export class Captcha {
  Captcha: any;
  Key: any;

  constructor(Captcha?: string, Key?: string) {

    this.Captcha = Captcha;
    this.Key = Key;
  }
}
