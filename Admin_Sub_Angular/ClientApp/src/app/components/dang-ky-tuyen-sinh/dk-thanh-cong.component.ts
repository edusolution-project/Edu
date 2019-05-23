import { Component, OnInit } from "@angular/core";
import { GlobalService } from '../../services/global.service';
import { HoSoDuTuyenSearchForm } from "../tra-cuu-ho-so-du-tuyen/tra-cuu-ho-so-du-tuyen.component";


@Component({
  selector: 'dk-thanh-cong',
  templateUrl:'./dk-thanh-cong.component.html'
})
export class DKThanhCongComponent implements OnInit
{
  item: HoSoThanhCong = new HoSoThanhCong();
  
 
  constructor(private globalService: GlobalService) {

  }
  ngOnInit() {
    this.item = this.globalService.hoSoThanhCong;
    if (this.item == undefined)
      this.item = new HoSoThanhCong();
  }
}
export class HoSoThanhCong
{
  public MaHoSo: any;
  public DiaDiem: any;
  public NgayHen: any;
  
}

