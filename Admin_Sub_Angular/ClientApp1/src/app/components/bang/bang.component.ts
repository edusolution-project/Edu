import { Component, OnInit, AfterViewInit, ViewChild } from "@angular/core";
import * as $ from 'jquery';
import { BangData } from "src/app/services/bang-data.service";
import { retry } from "rxjs/operators";
import { CdkVirtualScrollViewport } from "@angular/cdk/scrolling";

@Component({
  selector: 'bang',
  templateUrl: './bang.component.html'
})
export class BangComponent implements AfterViewInit {

  objBangDuLieu_Slide; objBangDuLieu_Fixed; // Đối tượng bảng Fixed, Slide
  BangDuLieuID_Slide = "";
  BangDuLieuID_Fixed = "";
  Bang_ID_TB10 = "";
  Bang_ID_TB11 = "";
  Bang_ID = "BangDuLieu";
  Bang_arrRow_Fixed;
  Bang_arrRow_Slide;
  Bang_arrCell_Fixed;
  Bang_arrCell_Slide;
  Bang_Viewport_NMax;
  Bang_Viewport_N = 50;
  Bang_Viewport_hMin = 0;
  Bang_DoRongHang = 25;
  Bang_nH = 0;
  Bang_nC = 0;
  Bang_nC_Fixed = 5;
  Bang_nC_Slide = 4;
  Bang_arrDoRongCot;
 // listTest=[][];
  Bang_arrMaCot;
  arrTest;
  constructor(private bangData: BangData) {
    this.arrViewport_N = this.arrayOne(this.Viewport_N);
    this.arrTest = this.arrayOne(500);
  }
  ngAfterViewInit() {
    this.Bang_GanGiaTriCacBienTongThe();
    this.Bang_KhoiTao();
    this.Bang_HienThiDuLieu();

  }
  @ViewChild('viewSlied') viewport: CdkVirtualScrollViewport;



  Bang_Height = 470;

  Bang_FixedRow_Height = 50;
  Bang_ChiTiet_Height = this.Bang_Height - this.Bang_FixedRow_Height;
  Viewport_N = 50;
  arrViewport_N;
  Bang_Width_Fixed = 510;
  Bang_Width_Slied = 460;

  bangFixed = [
    {
      Width: '80',
      TieuDe: 'sLNS',
    }
    ,
    {
      Width: '50',
      TieuDe: 'sL'
    }
    ,
    {
      Width: '50',
      TieuDe: 'sK'
    }
    ,
    {
      Width: '50',
      TieuDe: 'sM'
    }
    ,
    {
      Width: '310',
      TieuDe: 'Mô tả'
    }
  ];
  bangSlied = [
    {
      Width: '100',
      TieuDe: 'Đơn vị'
    }
    ,
    {
      Width: '80',
      TieuDe: 'Tự chi'
    }
    ,
    {
      Width: '80',
      TieuDe: 'Hiện vật'
    }
    ,
    {
      Width: '100',
      TieuDe: 'Chi tập trung'
    }
  ];
  Bang_arrHienThi;
  arrayOne(n: number): any[] {
    return Array(n);
  }

  styleNone = "";
  listNganSach = [
    ['1010000', '', '', '', 'KP lương, phụ cấp, tiền ăn', '10', '1000000', '2000000', '3000000'],
    ['10100001', '460', '468', '6000', 'Tiền lương', '0', '0', '0', '0'],
    ['10100002', '460', '468', '6000', 'Tiền lương', '0', '0', '0', '0'],
    ['10100003', '460', '468', '6000', 'Tiền lương', '0', '0', '0', '0'],
    ['10100004', '460', '468', '6000', 'Tiền lương', '0', '0', '0', '0'],
    ['10100006', '460', '468', '6000', 'Tiền lương', '0', '0', '0', '0'],
    ['10100007', '460', '468', '6000', 'Tiền lương', '0', '0', '0', '0'],
    ['10100008', '460', '468', '6000', 'Tiền lương', '0', '0', '0', '0'],
    ['10100009', '460', '468', '6000', 'Tiền lương', '0', '0', '0', '0'],
    ['101000010', '460', '468', '6000', 'Tiền lương', '0', '0', '0', '0'],
    ['101000011', '460', '468', '6000', 'Tiền lương', '0', '0', '0', '0'],
    ['101000012', '460', '468', '6000', 'Tiền lương', '0', '0', '0', '0'],
    ['101000013', '460', '468', '6000', 'Tiền lương', '0', '0', '0', '0'],
    ['101000014', '460', '468', '6000', 'Tiền lương', '0', '0', '0', '0'],
    ['101000015', '460', '468', '6000', 'Tiền lương', '0', '0', '0', '0'],
    ['101000016', '460', '468', '6000', 'Tiền lương', '0', '0', '0', '0'],
    ['101000017', '460', '468', '6000', 'Tiền lương', '0', '0', '0', '0'],
    ['101000018', '460', '468', '6000', 'Tiền lương', '0', '0', '0', '0'],
    ['101000019', '460', '468', '6000', 'Tiền lương', '0', '0', '0', '0'],
  ]
  strDuLieu() {
    let strReturn = "";
    for (var i = 0; i < this.listNganSach.length; i++) {

      if (i > 0) {
        strReturn = strReturn + this.bangData.Bang_DauCachHang;
      }
      for (var j = 0; j < this.listNganSach[i].length; j++) {
        if (j > 0) {
          strReturn += this.bangData.Bang_DauCachO;
        }
        strReturn += this.listNganSach[i][j];
      }
    }
    return strReturn;

  }


  Bang_KhoiTao() {

    //add data listngansach
    this.listNganSach = [];
    for (var c = 0; c < 500; c++) {

      this.listNganSach.push([c + '', '460', '468', '6000', 'Tiền lương', '0', '0', '0', '0']);
    }
    var arr = this.arrayOne(this.Viewport_N);
    for (var z = 0; z < arr.length; z++) {
      if (z > this.listNganSach.length)
        arr[z] = "none";
    }
    this.arrViewport_N = arr;

    var i, j;
    this.objBangDuLieu_Slide = $('#' + this.BangDuLieuID_Slide)[0];
    this.objBangDuLieu_Fixed = $('#' + this.BangDuLieuID_Fixed)[0];
    this.Bang_Viewport_NMax = this.objBangDuLieu_Fixed.rows.length;

    this.Bang_arrRow_Fixed = new Array();
    this.Bang_arrRow_Slide = new Array();
    this.Bang_arrCell_Fixed = new Array();
    this.Bang_arrCell_Slide = new Array();
    for (i = 0; i < this.Bang_Viewport_NMax; i++) {
      this.Bang_arrRow_Fixed.push(this.objBangDuLieu_Fixed.rows[i]);
      this.Bang_arrRow_Slide.push(this.objBangDuLieu_Slide.rows[i]);
      this.Bang_arrCell_Fixed.push(new Array());
      for (j = 0; j < this.Bang_nC_Fixed; j++) {
        this.Bang_arrCell_Fixed[i].push(this.objBangDuLieu_Fixed.rows[i].cells[j]);
      }
      this.Bang_arrCell_Slide.push(new Array());
      for (j = 0; j < this.Bang_nC_Slide; j++) {
        //  this.Bang_arrCell_Slide[i].push(this.objBangDuLieu_Slide.rows[i].cells[j]);
      }
    }
    this.Bang_arrHienThi = this.bangData.Bang_LayMang2ChieuGiaTri(this.strDuLieu());
    this.Bang_nH = this.Bang_arrHienThi.length;
    this.Bang_arrMaCot = this.bangData.Bang_LayMang1ChieuGiaTri("sLNS,sL,sK,sM,sMoTa,iID_MaDonVi,rTuChi,rHienVat,rChiTapTrung");
    this.Bang_nC = this.Bang_arrMaCot.length;
    this.Bang_arrDoRongCot = this.bangData.Bang_LayMang1ChieuGiaTri("80,50,50,50,310,80,80,100");

    let bang = new Bang();
    
    for (i = 0; i < this.Bang_arrMaCot.length; i++) {
        bang = new Bang();
       bang.MaCot = this.Bang_arrMaCot[i];
     // this.listTest.push(bang);
    }
   
  }
  Bang_GanGiaTriCacBienTongThe() {
    //Bang_ID_Div = this.Bang_ID + "_div";
    //Bang_ID_TB00 = this.Bang_ID + "_TB00";
    //Bang_ID_TB00_Div = Bang_ID_TB00 + "_div";
    //Bang_ID_TB01 = this.Bang_ID + "_TB01";
    //Bang_ID_TB01_Div = Bang_ID_TB01 + "_div";
    this.Bang_ID_TB10 = this.Bang_ID + "_TB10";
    //Bang_ID_TB10_Div = this.Bang_ID_TB10 + "_div";
    this.Bang_ID_TB11 = this.Bang_ID + "_TB11";
    //Bang_ID_TB11_Div = Bang_ID_TB11 + "_div";
    //Bang_ID_TR_BangDuLieu = this.Bang_ID + "_TR_DuLieu";
    this.BangDuLieuID_Slide = this.Bang_ID_TB11;
    this.BangDuLieuID_Fixed = this.Bang_ID_TB10;
    //BangDuLieuID_Slide_Div = Bang_ID_TB11_Div;
  }
  Bang_HienThiDuLieu() {
    if (this.Bang_Viewport_N == 0) return;
    var n = 0;
    var hMin = this.Bang_Viewport_hMin;
    var hMax = this.Bang_Viewport_hMin + this.Bang_Viewport_N;
    var tgHeight;
    n = 0;
    tgHeight = (hMin + 1) * this.Bang_DoRongHang;
    $(this.Bang_arrRow_Slide[n]).css('height', tgHeight);
    $(this.Bang_arrRow_Fixed[n]).css('height', tgHeight);

    n = this.Bang_Viewport_N - 1;
    tgHeight = (this.Bang_nH - hMax + 1) * this.Bang_DoRongHang;
    $(this.Bang_arrRow_Slide[n]).css('height', tgHeight);
    $(this.Bang_arrRow_Fixed[n]).css('height', tgHeight);
    // console.log("hMin=", hMin)
    for (var h = hMin; h < hMax; h++) {
      for (var c = 0; c < this.Bang_nC; c++) {
        this.Bang_HienThiDuLieuO(h, c);
      }
      // n++;
    }

  }
  Bang_HienThiDuLieuO(h, c) {
    var GTHienThi = this.Bang_LayDuLieuHienThiCuaO(h, c);
    //Kiểm tra hàng có thuộc Viewport hay không
    if (0 <= h - this.Bang_Viewport_hMin && h - this.Bang_Viewport_hMin < this.Bang_Viewport_N) {
      var i = h - this.Bang_Viewport_hMin;
      var j = this.AnhXaCot_DuLieu_Fixed(c);
      if (j >= 0) {
        // this.Bang_arrCell_Fixed[i][j].innerHTML = GTHienThi;
      }
      j = this.AnhXaCot_DuLieu_Slide(c);
      if (c == 8) {
      }
      if (j >= 0) {
        // this.Bang_arrCell_Slide[i][j].innerHTML = GTHienThi;
      }
    }
  }
  Bang_LayDuLieuHienThiCuaO(h, c) {
    var GTHienThi = this.Bang_arrHienThi[h][c];
    GTHienThi = '<span title="' + this.Bang_arrHienThi[h][c] + '" style="width:' + this.Bang_arrDoRongCot[c] + 'px;">' + this.Bang_arrHienThi[h][c] + '</span>';
    return GTHienThi;
  }

  //Ánh xạ cột c trong Dữ liệu sang cột trong bảng Slide
  AnhXaCot_DuLieu_Slide(c) {
    var vR = c - this.Bang_nC_Fixed;
    if (0 <= vR && vR < this.Bang_nC_Slide)
      return vR;
    return -1;
  }
  //Ánh xạ cột c trong Dữ liệu sang cột trong bảng Fixed
  AnhXaCot_DuLieu_Fixed(c) {
    if (c < this.Bang_nC_Fixed)
      return c;
    return -1;
  }


  Bang_fnScroll(ev) {

    var yTop = $('#' + this.Bang_ID + '_TB21_div').scrollTop();
    if (yTop < 30) yTop = 0;
    $('#' + this.Bang_ID + '_TB20_div').scrollTop(yTop);
    var tem = $('#BangDuLieu_TB21_div .cdk-virtual-scroll-content-wrapper').css('transform');
    $('#BangDuLieu_TB20_div .cdk-virtual-scroll-content-wrapper').css({ 'transform': tem })

    // console.log("y1Top+"+y1Top);


  }

  /*
* Function: Bang_SetPosition
* Purpose:  Set table'position by scrolltop
* Returns:  nothing
* Inputs:   int:y - scrolltop
* Notes:    
*/
  Bang_SetPosition(y) {
    if (this.Bang_Viewport_N == 0) return;
    var yMin = this.Bang_Viewport_hMin * this.Bang_DoRongHang;
    var yMax = yMin + 30 * this.Bang_DoRongHang;
    if (yMin <= y && y <= yMax) return;
    var cs = parseInt((y / this.Bang_DoRongHang) + "");
    if (yMax < y) {
      cs = cs + 20;
    }
    this.fnSetViewportPosition(cs);

  }
  fnSetViewportPosition(newH) {
    if (this.Bang_Viewport_N == 0) return false;
    if (this.Bang_Viewport_hMin + 5 < newH && newH < this.Bang_Viewport_hMin + this.Bang_Viewport_N - 5) {
      return false;
    }

    var hMoi = newH;
    if (hMoi <= this.Bang_Viewport_hMin + 5) {
      hMoi = hMoi - 15;
    }
    else if (hMoi >= this.Bang_Viewport_hMin + this.Bang_Viewport_N - 5) {
      hMoi = hMoi - this.Bang_Viewport_N + 15;
    }

    var hMax = hMoi + this.Bang_Viewport_N;
    if (hMax > this.Bang_nH) {
      hMoi = this.Bang_nH - this.Bang_Viewport_N;
    }
    if (hMoi < 0) hMoi = 0;
    if (hMoi != this.Bang_Viewport_hMin) {
      this.Bang_Viewport_hMin = hMoi;
      this.Bang_HienThiDuLieu();
      //_fnRefressFocusPosition();
      return true;
    }
    return false;
  }
}
class Bang
{
  MaCot: any;
}
