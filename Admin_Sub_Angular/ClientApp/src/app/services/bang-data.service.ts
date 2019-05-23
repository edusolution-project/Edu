import { Injectable } from "@angular/core";

@Injectable()
export class BangData {
  Bang_DauCachHang = '#|';
  Bang_DauCachO = "##";
  Bang_Viewport_N = 50;
  Bang_Viewport_hMin = 0;
  Bang_DoRongHang = 25;
  Bang_LayMang2ChieuGiaTri(strTG:string) {
    var arr = new Array();
    //var strTG = document.getElementById(id).value;
    if (strTG != "") {
      strTG = strTG + "";
      var arrTG = strTG.split(this.Bang_DauCachHang);
      for (var i = 0; i < arrTG.length; i++) {
        arr[i] = arrTG[i].split(this.Bang_DauCachO);
      }
    }
    return arr;
  }
  Bang_LayMang1ChieuGiaTri(strTG: string) {
  var arr = new Array();
    if (strTG != "") {
      arr = strTG.split(",");
    }
  return arr;
}

  Bang_KhoiTao() {

  }

  Bang_HienThiDuLieu() {
    if (this.Bang_Viewport_N == 0) return;
    var n = 0;
    var hMin = this.Bang_Viewport_hMin;
    var hMax = this.Bang_Viewport_hMin + this.Bang_Viewport_N;
    var tgHeight;
    n = 0;
    tgHeight = (hMin + 1) * this.Bang_DoRongHang;
   // $(Bang_arrRow_Slide[n]).css('height', tgHeight);
  //  $(Bang_arrRow_Fixed[n]).css('height', tgHeight);
  }
}
