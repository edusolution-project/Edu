import { Component, Query } from '@angular/core';
import { Restangular } from "ngx-restangular";
import { DomSanitizer } from '@angular/platform-browser';
import { MessageSeverity, AlertService } from '../../services/alert.service';
import { forEach } from '@angular/router/src/utils/collection';
import { AppComponent } from '../app.component';
import { Apollo } from 'apollo-angular';
import gql from 'graphql-tag';
import { Observable, Subscription } from 'rxjs';
import { map } from 'rxjs/operators';
import { error } from 'protractor';
@Component({
  selector: 'tra-cuu-ho-so-du-tuyen',
  templateUrl: './tra-cuu-ho-so-du-tuyen.component.html'
})
export class TraCuuHoSoDuTuyenComponent {
  show1 = true;
  url = '../assets/chuacothongtin.pdf';
  capChaService;
  image: any = "";
  key;
  capchart1: Captcha = new Captcha('', '');
  dataSearch: HoSoDuTuyenSearchForm = new HoSoDuTuyenSearchForm(this.capchart1, '');
  HoSo: HoSoDuTuyenSearchForm = new HoSoDuTuyenSearchForm();
  isShow = false;
  tenTrangThai: string = "";

  captCha: any;

  constructor(private restangular: Restangular,
    private saniti: DomSanitizer,
    private alertService: AlertService,
    private appComponent: AppComponent,
    private apollo: Apollo) {
    this.refreshCapcha();

  }

  search() {
    this.isShow = true;
    this.appComponent.isViewContentLoading = true;
    //this.restangular.all('HoSoDuTuyen').all("TraCuuHsDuTuyenPHHS").post(this.dataSearch).subscribe(
    //  response => {
    //    if (response.HoSoDuTuyenId == undefined || response == null) {
    //      this.showErrorAlert("", "Mã hồ sơ không tồn tại");
    //      this.appComponent.isViewContentLoading = false;
    //      return;
    //    }

    //    this.HoSo = response;
    //    this.tenTrangThai = this.convertTrangThaiHoSo(this.HoSo.TrangThai);
    //    this.refreshCapcha();
    //    this.alertService.showMessage("", "Tra cứu thành công", MessageSeverity.success);
    //    this.appComponent.isViewContentLoading = false;
    //  }
    //  , error => {
    //    this.showErrorAlert("", error.data);
    //    this.appComponent.isViewContentLoading = false;
    //  }
    //);

    let query = gql`query QueryPosts($seachForm:HoSoDuTuyenSearchForm)
{
  traCuuHsDuTuyenPHHS(hoSoDuTuyenSeachForm:$seachForm) {
    tenHocSinh
    capHoc
    dotTuyenSinhId
    ngayCapNhat
    ngaySinh
    trangThai
    dungTuyen
    truongDangKy
    lyDoTuChoi
    hoSoDuTuyenId
  }
}
`;
    this.apollo.watchQuery<any>({
      query: query,
      variables: {
        seachForm: this.dataSearch
      },
      fetchPolicy: 'no-cache'
    })
      .valueChanges
      .subscribe(({ data, loading }) => {
        if (loading == false) {
          this.refreshCapcha();
          if (data.traCuuHsDuTuyenPHHS == null || data.traCuuHsDuTuyenPHHS.hoSoDuTuyenId == undefined ) {
            this.showErrorAlert("", "Mã hồ sơ không tồn tại");
            this.appComponent.isViewContentLoading = false;
            return;
          }

          this.HoSo = data.traCuuHsDuTuyenPHHS;
          this.tenTrangThai = this.convertTrangThaiHoSo(this.HoSo.TrangThai);
          
          this.alertService.showMessage("", "Tra cứu thành công", MessageSeverity.success);
          this.appComponent.isViewContentLoading = false;
        }
      },
      error => {
        this.appComponent.isViewContentLoading = false;
        this.showErrorAlert("", error.message.replace("GraphQL error: ", ""))
      });

  }
  refreshCapcha() {
    let query = gql`{
  getCaptcha{
    fileName
    encodedData
  }
}`;
    this.apollo.watchQuery<any>({
      query: query,
      fetchPolicy: 'no-cache'
    })
      .valueChanges
      .subscribe(({ data, loading }) => {
        if (loading == false) {
          this.captCha = data;
          this.image = this.saniti.bypassSecurityTrustUrl('data:image/JPEG;base64,' + this.captCha.getCaptcha.encodedData);
          this.dataSearch.captcha.key = this.captCha.getCaptcha.fileName;
        }
      });

    //this.restangular.all('Util').all("GetCaptcha").post().subscribe(
    //  response => {
    //    this.image = this.saniti.bypassSecurityTrustUrl('data:image/JPEG;base64,' + response.EncodedData);
    //    this.dataSearch.Captcha.Key = response.FileName;
    //  }
    //  , error => {
    //    console.log(error);
    //  }
    //
    //);

  };


  showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }

  convertTrangThaiHoSo(item) {
    let lstTrangThaiHoSo = [
      {
        'value': '1',
        'label': 'Chờ duyệt'
      },
      {
        'value': '2',
        'label': 'Đã tiếp nhận'
      },
      {
        'value': '3',
        'label': 'Đã duyệt'
      },
      {
        'value': '4',
        'label': 'Đang xử lý'
      }, {
        'value': '5',
        'label': 'Từ chối duyệt'
      }
    ];

    for (var i = 0; i < lstTrangThaiHoSo.length; i++) {
      if (item == lstTrangThaiHoSo[i]["value"]) {
        return lstTrangThaiHoSo[i]["label"];
      }
    }
    return "";
  }
}
export class HoSoDuTuyenSearchForm {
  captcha: Captcha;
  maHoSo;
  TrangThai;
  LyDoTuChoi;
  constructor(captcha?: Captcha, maHoSo?: string) {
    this.captcha = captcha;
    this.maHoSo = maHoSo;
  }

}
export class Captcha {
  captcha: any;
  key: any;

  constructor(captcha?: string, key?: string) {

    this.captcha = captcha;
    this.key = key;
  }
}
export class FileDownLoadForm {
  encodedData: string;
  contentType: string;
  fileName: string;
}

