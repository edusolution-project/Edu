import { Component, OnInit, AfterViewInit } from '@angular/core';
import { Restangular } from "ngx-restangular";
import { DomSanitizer } from '@angular/platform-browser';
import { GlobalService } from '../../services/global.service';
import { ActivatedRoute, Router } from '@angular/router';
@Component({
  selector: 'dang-ky-tuyen-sinh',
  templateUrl: './dang-ky-tuyen-sinh.component.html'
})
export class DangKyTuyenSinhComponent implements OnInit {
  listDotTuyenSinhMamNon: dotTuyenSinhdto[];
  listDotTuyenSinh1: dotTuyenSinhdto[];
  listDotTuyenSinh2: dotTuyenSinhdto[];
  show1 = true;
  show2 = true;
  showMN = true;
  currentYear: any = "";
  constructor(private route: ActivatedRoute, private router: Router, private restangular: Restangular, private saniti: DomSanitizer, private globalServices: GlobalService) {

  }
  ngOnInit() {
    var services = this.restangular.all('dotTuyenSinh');
    this.currentYear = this.globalServices.currentYear;
    let soGiaoDucData = JSON.parse(localStorage.getItem("SoGiaoDuc"));

    services.all("GetByNamHocCapHoc").post({ namhoc: this.currentYear, captuyensinh: '45', MaDonVi: soGiaoDucData.MaTinh }).subscribe(
      response => {
        this.listDotTuyenSinhMamNon = response;
      }, error => {
        console.log(error);

      }
    );

    services.all("GetByNamHocCapHoc").post({ namhoc: this.currentYear, captuyensinh: '1', MaDonVi: soGiaoDucData.MaTinh }).subscribe(
      response => {
        this.listDotTuyenSinh1 = response;
      }, error => {
        console.log(error);

      }
    );
    services.all("GetByNamHocCapHoc").post({ namhoc: this.currentYear, captuyensinh: '2', MaDonVi: soGiaoDucData.MaTinh }).subscribe(
      response => {
        this.listDotTuyenSinh2 = response;
      }, error => {
        console.log(error);

      }
    );
  }
  changeView(item) {
    if (item == 1) {
      this.show1 = !this.show1;
    }
    if (item == 2) {
      this.show2 = !this.show2;
    }
    if (item == 3) {
      this.showMN = !this.showMN;
    }}

  chonDotTS(item) {
    if (item.CapTuyenSinh == "2")
      this.router.navigate(['/dang-ky-tuyen-sinh-cap2', item.DotTuyenSinhId]);
    else if (item.CapTuyenSinh == "2")
      this.router.navigate(['/dang-ky-tuyen-sinh-cap1', item.DotTuyenSinhId]);
    else
      this.router.navigate(['/dang-ky-tuyen-sinh-cap-mam-non', item.DotTuyenSinhId]);
  }
}
export class dotTuyenSinhdto {
  DotTuyenSinhId: string;
  active: boolean;
}
