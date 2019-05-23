import { Component, OnInit } from '@angular/core';
import { Restangular } from "ngx-restangular";
import { DomSanitizer } from '@angular/platform-browser';
import { GlobalService } from '../../services/global.service';
@Component({
  selector: 'quy-dinh-tuyen-sinh',
  templateUrl: './quy-dinh-tuyen-sinh.component.html'
})
export class QuyDinhTuyenSinhComponent implements OnInit {
  listDotTuyenSinh1: dotTuyenSinhdto[];
  show1 = true;
  url = '../assets/chuacothongtin.pdf';
  constructor(private restangular: Restangular, private saniti: DomSanitizer, private globalServices: GlobalService) {
    
  }
  Header: any;
  ngOnInit() {
    var services = this.restangular.all('dotTuyenSinh');
    let currentYear = this.globalServices.currentYear;
    let soGiaoDucData = JSON.parse(localStorage.getItem("SoGiaoDuc"));
    this.Header = "";
    services.all("GetByNamHocCapHoc").post({ namhoc: currentYear, captuyensinh: '1', MaDonVi: soGiaoDucData.MaTinh }).subscribe(
      response => {
        this.listDotTuyenSinh1 = response;
        for (var i = 0; i < response.length; i++) {
          if (response[i].TepCongVan != null && response[i].TepCongVan!="") {
            this.url = GlobalService.getManUrl + response[i].TepCongVan;
            this.Header = response[i]["TenQuanHuyen"];
            //$scope.url = $sce.trustAsResourceUrl(url);
            break;
          }
          else {
            this.url = "../assets/chuacothongtin.pdf";
          }
        }
      }, error => {
        console.log(error);

      }
    );
  }
 

  viewUrlPDF() {
    return this.saniti.bypassSecurityTrustResourceUrl(this.url);
  }

 
  chonDotTS(item) {
    this.listDotTuyenSinh1.forEach((item1) => {
      item1.active = false;
    });
    item.active = true;
    this.Header = item.TenQuanHuyen;
    if (item.DotTuyenSinhId != null && item.TepCongVan!=null) {
      this.url = GlobalService.getManUrl + item.TepCongVan;
    }
    else {
      this.url = "../assets/chuacothongtin.pdf";
    }
    this.viewUrlPDF();
  }
}
export class dotTuyenSinhdto {
  DotTuyenSinhId: string;
  active: boolean;
}
