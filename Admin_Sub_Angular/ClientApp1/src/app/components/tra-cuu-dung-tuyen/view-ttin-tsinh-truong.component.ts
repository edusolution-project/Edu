import { Component, Input, OnInit, AfterViewInit, ChangeDetectorRef } from "@angular/core";
import { Restangular } from "ngx-restangular";
@Component({
  selector: 'view-ttin-tsinh-truong',
  templateUrl: './view-ttin-tsinh-truong.component.html'
})
export class ViewTTinTsinhTruongComponent implements OnInit, AfterViewInit {

  @Input() item;
  @Input() capHoc;
  @Input() namHoc;

  //message = "12121";

  isLoading = true;
  ttinTSinhTheoTruong: any = new TtinTSinhTheoTruong();

  ngOnInit(): void {

    this.isLoading = false;
    var ttinTsinhService = this.restangular.all('ttinTsinh');
    ttinTsinhService.all('GetTtinTsinhTheoTruong').post({
      'MaTruongHoc': this.item.MaTruongHoc,
      'CapHoc': this.capHoc,
      'NamHoc': this.namHoc
    }).subscribe(data=> {
      this.ttinTSinhTheoTruong = data;
   //   item.isLoading = false;
    }, function () {
    //  item.isLoading = false;
    });
  }
  ngAfterViewInit() {

    if (this.item != undefined) {

    }

    //console.log(this.vtOpen);
  }
  constructor(private restangular: Restangular) {
  }

}
export class TtinTSinhTheoTruong {
  HuongDanTsinh:any
}

