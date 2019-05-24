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
  selector: 'tra-cuu-theo-truong',
  templateUrl: './tra-cuu-theo-truong.component.html'
})
export class TraCuuTheoTruongComponent implements OnInit {

  show1 = true;
  url = '../assets/chuacothongtin.pdf';
  modalRef: BsModalRef;
  public lstCapHoc;
  public testData;
  public lstNamHoc;
  public lstTruongHoc;
  public lstTinhThanh;
  public lstQuanHuyen;
  public lstTruongHocSearch;
  public lstToThon;
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
  truongHocService = this.restangular.all('TruongHoc');
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
    let soGiaoDucData = this.globalServices.soGiaoDucData;
    this.dataSearch.MaTinhThanh = soGiaoDucData.MaTinh;
    this.dataSearch.MaQuanHuyen = "";
    this.dataSearch.MaPhuongXa = "";
    this.dataSearch.MaToThon = "";
    this.dataSearch.searchType = "HoTen";
    this.dataSearch.capHoc = "1";
    this.dataSearch.namHoc = this.globalServices.currentYear;
    this.lstCapHoc = this.globalServices.convertObjectToSelect2(this.globalServices.lstCapHoc, "value", "label", "--Chọn--");
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
    if (options != undefined && options.length > 0) {
      this.dataSearch.MaQuanHuyen = options[0].value;
    }
    this.showDanhSachTruongHoc();
  }
  doCapHoc(options: INgxSelectOption[]) {
    if (options != undefined && options.length > 0) {
      this.dataSearch.capHoc = options[0].value;
    }
    this.showDanhSachTruongHoc();
  }
  doMaTruongHoc(options: INgxSelectOption[]) {
    $('#dsTruongHoc .ngx-select__choices').removeClass('show');
  }

  showDanhSachTruongHoc() {
    this.lstTruongHocSearch = null;
    if (this.flagMaQhuyenThtru == true) {
      this.truongHocService.all("getListTruongHocTheoDonViVaCapHoc").post(this.dataSearch).subscribe(
        response => {

          this.lstTruongHocSearch = this.globalServices.convertObjectToSelect2(response, 'MaTruongHoc', 'TenTruongHoc', '--Chọn trường--');
          if (response.length > 0) {
            $('#dsTruongHoc .ngx-select').trigger('click');
            setTimeout(() => {
              $('#dsTruongHoc .ngx-select__choices').addClass('show');
            }, 100);
          }
          else {
            this.dataSearch.MaTruongHoc = "";
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
    this.appComponent.isViewContentLoading = true;
    this.firstLoad = false;
    this.lstTruongHoc = null;
    this.restangular.all("ttinTsinh").all("SearchTruongHoc").post(this.dataSearch).subscribe(
      data => {
        this.lstTruongHoc = data;
        this.appComponent.isViewContentLoading = false;

      },
      error => {
        this.showErrorAlert("", error.data);
        this.appComponent.isViewContentLoading = false;
      }
    );
  }
  changeCheckbox() {
    this.dataSearch.SoQuanLy = !this.dataSearch.SoQuanLy;
  }

}
export class TraCuuDungTuyenSearch {
  ngaySinh: any;
  ngaySinhUTC: any;
  searchType: string;
  capHoc: any;
  namHoc: any;
  tenHocSinh: any;
  MaTinhThanh: any;
  MaQuanHuyen: any;
  MaPhuongXa: string;
  MaToThon: string;
  SoQuanLy: boolean;
  MaTruongHoc: string;

}
