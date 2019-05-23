import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { Restangular } from "ngx-restangular";
import { DomSanitizer } from '@angular/platform-browser';
import { GlobalService } from '../../services/global.service';
import { MessageSeverity, AlertService } from '../../services/alert.service';
import { DatePipe } from '@angular/common';
import { log } from 'util';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { IMyDpOptions } from 'mydatepicker';
import { INgxSelectOption } from 'ngx-select-ex';
import { AppComponent } from '../app.component';
@Component({
  selector: 'tra-cuu-dung-tuyen',
  templateUrl: './tra-cuu-dung-tuyen.component.html'
})
export class TraCuuDungTuyenComponent implements OnInit {

  show1 = true;
  url = '../assets/chuacothongtin.pdf';
  modalRef: BsModalRef;
  public lstCapHoc;
  public testData;
  public lstNamHoc;
  public lstTruongHoc;
  public lstTinhThanh;
  public lstQuanHuyen;
  public lstPhuongXa;
  public lstToThon;
  public lstHocSinh = {};
  flagMaTthanhThtru = false;
  flagMaQhuyenThtru = false;
  flagMaPxaThtru = false;
  errorMessage = "";
  firstLoad = true;
  dataSearch: TraCuuDungTuyenSearch = new TraCuuDungTuyenSearch();
  tinhThanhService = this.restangular.all('TinhThanh');
  quanHuyenService = this.restangular.all('QuanHuyen');
  phuongXaService = this.restangular.all('PhuongXa');
  toThonService = this.restangular.all('ToThon');
  public myDatePickerOptions: IMyDpOptions = {
    // inline: false,
    // editableDateField: false,
    openSelectorOnInputClick: true,
    dateFormat: 'dd/mm/yyyy'
  };

  @ViewChild('ChonHocSinh') chonHocSinh: TemplateRef<any>;
  constructor(private datePipe: DatePipe, private restangular: Restangular, private saniti: DomSanitizer,
    private globalServices: GlobalService, private alertService: AlertService,
    private modalService: BsModalService, private appComponent: AppComponent) {

  }
  ngOnInit() {
   // this.dataSearch.tenHocSinh = "Nguyen chi nghiep";
    let soGiaoDucData = this.globalServices.soGiaoDucData;
    this.dataSearch.MaTinhThanh = soGiaoDucData.MaTinh;
    this.dataSearch.MaQuanHuyen = "";
    this.dataSearch.MaPhuongXa = "";
    this.dataSearch.MaToThon = "";
    this.dataSearch.searchType = "HoTen";
    this.dataSearch.capHoc = "1";
    this.dataSearch.namHoc = this.globalServices.currentYear;
console.log(this.dataSearch);
    this.dataSearch.LoaiCuTru = "1";
    this.lstCapHoc = this.globalServices.convertObjectToSelect2(this.globalServices.lstCapHoc, "value", "label");
    this.lstNamHoc = this.globalServices.getlistNamHoc();
    
    this.tinhThanhService.all("GetAll").post().subscribe(
      response => {
        this.lstTinhThanh = this.globalServices.convertObjectToSelect2(response, 'MaTinhThanh', 'TenTinhThanh', "--Chọn--");
      }, error => {
        console.log(error);
      }
    );
    this.quanHuyenService.all("GetByMaTinhThanh").post({ 'MaTinhThanh': this.dataSearch.MaTinhThanh }).subscribe(
      response => {
        this.lstQuanHuyen = this.globalServices.convertObjectToSelect2(response, 'MaQuanHuyen', 'TenQuanHuyen', '--QUẬN HUYỆN--');
      }, error => {
        console.log(error);
      }
    );

  }

  doMaQuanHuyen(options: INgxSelectOption[]) {
    $('#dsQuanHuyen .ngx-select__choices').removeClass('show');
    let MaQhuyenThtru: string | number = '-1';
    if (options != undefined && options.length > 0) {
      MaQhuyenThtru = options[0].value;
    }
    if (this.flagMaQhuyenThtru == true) {
      this.phuongXaService.all("GetByMaQuanHuyen").post({ 'MaQuanHuyen': MaQhuyenThtru }).subscribe(
        response => {
          this.lstPhuongXa = this.globalServices.convertObjectToSelect2(response, 'MaPhuongXa', 'TenPhuongXa', "--PHƯỜNG XÃ--");
          if (response.length > 0) {
            $('#dsPhuongXa .ngx-select').trigger('click');
            setTimeout(() => {
              $('#dsPhuongXa .ngx-select__choices').addClass('show');
            }, 100);
          }
          else {
            this.dataSearch.MaPhuongXa = "";
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
  doMaPhuongXa(options: INgxSelectOption[]) {
    $('#dsPhuongXa .ngx-select__choices').removeClass('show');
    let MaPxaCuTru: string | number = '-1';
    if (options != undefined && options.length > 0) {
      MaPxaCuTru = options[0].value;
    }
    if (this.flagMaPxaThtru == true) {
      this.toThonService.all("GetByMaPhuongXa").post({ 'MaPhuongXa': MaPxaCuTru }).subscribe(
        response => {
          this.lstToThon = this.globalServices.convertObjectToSelect2(response, 'MaToThon', 'TenToThon', '--TỔ THÔN--');
          if (response.length > 0) {
            $('#dsToThon .ngx-select').trigger('click');
            setTimeout(() => {
              $('#dsToThon .ngx-select__choices').addClass('show');
            }, 100);
          }
          else {
            this.dataSearch.MaToThon = "";
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
  doMaToThon(options: INgxSelectOption[]) {
    $('#dsToThon .ngx-select__choices').removeClass('show');
  }

  showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }

  convertLocalDate(date) {
    var convertedDate = date.year + '-' + date.month + '-' + date.day;
    return convertedDate;
  }
  search(f) {
    if (!f.form.valid)
      return;
    this.firstLoad = false;
    this.lstTruongHoc = null;
    this.appComponent.isViewContentLoading = true;
    if (this.dataSearch.searchType == "HoTen") {
      if (this.dataSearch.ngaySinhUTC != undefined) {
        this.dataSearch.ngaySinh = this.convertLocalDate(this.dataSearch.ngaySinhUTC.date);
      }

      this.restangular.all("hocSinh").all("GetByCondition").post(this.dataSearch).subscribe(
        data => {
          console.log(data.length);
          if (data.length === 0 || data.length == undefined) {
            this.errorMessage = 'Không tìm thấy học sinh!';
          } else if (data.length == 1) {
            this.chooseHocSinh(data[0]);
          } else {
            this.lstHocSinh = data;
            this.openModal(this.chonHocSinh);
          }
          this.appComponent.isViewContentLoading = false;
        },
        error => {
          this.showErrorAlert("", error.data);
          this.appComponent.isViewContentLoading = false;
        }
      );
    }
    else {
      this.restangular.all("ttinTsinh").all('SearchTruongHocDungTuyen').post(this.dataSearch).subscribe(data => {

        //$rootScope.isViewLoading = false;
        if (data.length == 0) {
          this.errorMessage = 'Không có trường học đúng tuyến!';


        }
        else {
          this.lstTruongHoc = data;
        }
        this.appComponent.isViewContentLoading = false;
      }, error => {
        this.appComponent.isViewContentLoading = false;
        // $rootScope.isViewLoading = false;
      });
    }




  }
  ChangeHoTen() {
    this.flagMaQhuyenThtru = false;
    this.flagMaPxaThtru = false;
    this.flagMaTthanhThtru = false;
  }
  chon() {
    alert(1);
  }
  openModal(template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template, { class: 'modal-lg' });
    $('.modal-content').css('display', 'block')
  }

  chooseHocSinh(item) {
    this.modalRef.hide();
    if (!item.MaTthonThtru && !item.MaTthonTmtru) {
      this.errorMessage = 'Không có trường học đúng tuyến!';
    }

    let count = 0;
    if (item.MaTthonThtru) {
      this.restangular.all("ttinTsinh").all("SearchTruongHocDungTuyen").post({
        LoaiCuTru: "1",
        MaToThon: item.MaTthonThtru,
        CapHoc: this.dataSearch.capHoc,
        NamHoc: this.dataSearch.namHoc
      }).subscribe(
        data => {
          this.lstTruongHoc = data;
        }
      );
    } else {
      count++;
    }
  }

}
export class TraCuuDungTuyenSearch {
  ngaySinh: any;
  ngaySinhUTC: any;
  searchType: string;
  capHoc: string;
  namHoc: any;
  tenHocSinh: any;
  MaTinhThanh: any;
  MaQuanHuyen: string;
  MaPhuongXa: string;
  MaToThon: string;
  LoaiCuTru: string;

}
