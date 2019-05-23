import { Component, OnInit } from '@angular/core';
import { fadeInOut } from '../../services/animations';

@Component({
  selector: 'huong-dan',
  templateUrl: './huong-dan.component.html',
  animations: [fadeInOut]
})
export class HuongDanComponent implements OnInit {
    ngOnInit(): void {
      this.Type = "1";
      this.Header = "HƯỚNG DẪN TRA CỨU TRƯỜNG ĐÚNG TUYẾN";
    }

  Type: string;
  Header: string;
  changeLink(type) {
    this.Type = type;
    switch (type) {
      case 1:
        this.Header = "HƯỚNG DẪN TRA CỨU TRƯỜNG ĐÚNG TUYẾN";
        break;
      case 2:
        this.Header = "HƯỚNG DẪN TRA CỨU THÔNG TIN TUYỂN SINH";
        break;
      case 3:
        this.Header = "HƯỚNG DẪN ĐĂNG KÝ TUYỂN SINH";
        break;
      case 4:
        this.Header = "HƯỚNG DẪN TRA CỨU KẾT QUẢ";
        break;
      default:
        this.Header = "HƯỚNG DẪN TRA CỨU TRƯỜNG ĐÚNG TUYẾN";
        break;
    };
  }

}


@Component({
  selector: 'hd-tra-cuu-dung-tuyen',
  templateUrl: './hdTraCuuDungTuyen.html'
})
export class hdTraCuuDungTuyenComponent {
  home = require("../../assets/images/hdsd/1_tra_cuu_truong_dung_tuyen/home.png");
  tracuuhs = require("../../assets/images/hdsd/1_tra_cuu_truong_dung_tuyen/tracuuhs.png");
  popup = require("../../assets/images/hdsd/1_tra_cuu_truong_dung_tuyen/popup.png");
  kqhs = require("../../assets/images/hdsd/1_tra_cuu_truong_dung_tuyen/kqhs.png");
  tracuudiachi = require("../../assets/images/hdsd/1_tra_cuu_truong_dung_tuyen/tracuudiachi.png");
  kqdiachi = require("../../assets/images/hdsd/1_tra_cuu_truong_dung_tuyen/kqdiachi.png");

}
@Component({
  selector: 'hd-tra-cuu-tt-tuyen-sinh',
  templateUrl: './hdTraCuuTTTuyenSinh.html'
})
export class hdTraCuuTTTuyenSinhComponent {
  home = require("../../assets/images/hdsd/2_tra_cuu_thong_tin_tuyen_sinh/home.png");
  tracuu = require("../../assets/images/hdsd/2_tra_cuu_thong_tin_tuyen_sinh/tracuu.png");
  danhsach = require("../../assets/images/hdsd/2_tra_cuu_thong_tin_tuyen_sinh/danhsach.png");
  chitiet = require("../../assets/images/hdsd/2_tra_cuu_thong_tin_tuyen_sinh/chitiet.png");
}
@Component({
  selector: 'hd-dang-ky-tuyen-sinh',
  templateUrl: './hdDKTuyenSinh.html'
})
export class hdDangKyTuyenSinhComponent {
  home = require("../../assets/images/hdsd/3_dang_ky_tuyen_sinh/home.jpg");
  danhsach = require("../../assets/images/hdsd/3_dang_ky_tuyen_sinh/danhsach.png");
  dangky = require("../../assets/images/hdsd/3_dang_ky_tuyen_sinh/dangky.png");
  xacnhan = require("../../assets/images/hdsd/3_dang_ky_tuyen_sinh/xacnhan.png");
}
@Component({
  selector: 'hd-tra-cuu-ket-qua',
  templateUrl: './hdTraCuuKetQua.html'
})
export class hdTraCuuKetQuaComponent {
  home = require("../../assets/images/hdsd/4_tra_cuu_ket_qua/home.png");
  tracuu = require("../../assets/images/hdsd/4_tra_cuu_ket_qua/tracuu.png");
  kq = require("../../assets/images/hdsd/4_tra_cuu_ket_qua/kq.png");
}

