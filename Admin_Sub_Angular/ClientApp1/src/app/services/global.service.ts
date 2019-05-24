// ====================================================
// More Templates: https://www.ebenmonney.com/templates
// Email: support@ebenmonney.com
// ====================================================

import { Injectable, Injector } from '@angular/core';
import { Restangular } from 'ngx-restangular';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { EndpointFactory } from './endpoint-factory.service';
import { ConfigurationService } from './configuration.service';
import { map } from 'rxjs/operators';

@Injectable()
export class GlobalService {
  private readonly _url: string;
  private readonly _urlSoGiaoDucData;
  http: any;
  //static getBaseUrl: any = 'http://localhost:55264/';
  //static getManUrl: any = 'http://localhost:8889/';

//static getBaseUrl: any = 'http://10.60.157.111:8013/';
  //static getManUrl: any = 'http://10.60.157.111:8014/';

  static getBaseUrl: any = 'http://localhost:9999/';
  static getManUrl: any = 'http://apiquanly.tuyensinhdaucap.edu.vn/';

  constructor(http: HttpClient) {
    this.http = http;
    this.getlistNamHoc();
    this._url = GlobalService.getBaseUrl + 'api/util/GetCurrentYear1';
    this._urlSoGiaoDucData = GlobalService.getBaseUrl + 'api/SoGiaoDucC12/RetrieveByDomain';
    //super(http, configurations, injector);
    // this.test();
  }
  hoSoThanhCong;
  currentYear='2019';
  soGiaoDucData;
  public lstNamHoc;
  // This is the method you want to call at bootstrap
  // Important: It should return a Promise
  loadCurrentYear(): Promise<any> {
    //this.currentYear = '2019';
    //return;
    //return this.http
    //  .get(this._url)
    //  .toPromise().then(
    //    res => {
    //      const data = res as any;
    //      this.currentYear = data;
    //    })
    //  .catch(error => console.log(error));
    return null;
  }

  public getlistNamHoc() {
    this.lstNamHoc = [];
    for (let i = +this.currentYear - 2; i <= +this.currentYear+2; i++) {
      this.lstNamHoc.push({
        'id': '' + i,
        'text': '' + i + ' - ' + (i + 1)
      });
    }
    return this.lstNamHoc;
   
  }
  
  public lstCapHoc = [
    {
      'value': '45',
      'label': 'Cấp Mầm non'
    },
    {
      'value': '1',
      'label': 'Cấp 1'
    },
    {
      'value': '2',
      'label': 'Cấp 2'
    }
  ];
  public lstGioiTinh = [
    {
      'id': '',
      'text': 'Chọn'
    },
    {
      'id': '1',
      'text': 'Nam'
    },
    {
      'id': '0',
      'text': 'Nữ'
    }
  ];
  public bindAgeYear = function () {
    var year = new Date().getFullYear() - 18;
    var range = [];
    for (var i = 1; i < 82; i++) {
      range.push(year - i);
    }
    return this.convertArrayToSelect2(range);
  };
  public convertArrayToSelect2(arr: any) {
    let resultData: SelectOptionData[] = [];
    for (let i = 0; i < arr.length; i++) {
      let item: SelectOptionData = new SelectOptionData();
      item.id = arr[i];
      item.text = arr[i];
      resultData.push(item);
    }
    return resultData;
  }
  //select 2 need id and text
  public convertObjectToSelect2(arr: any, id: string, text: string, isChon="") {
    let resultData: SelectOptionData[] = [];
    for (let i = 0; i < arr.length; i++) {
      let item: SelectOptionData = new SelectOptionData();
      item.id = arr[i][id];
      item.text = arr[i][text];
      resultData.push(item);
    }
    if (isChon != undefined && isChon != "") {
      resultData.unshift({ 'id': '', 'text': isChon });
    }
    
    return resultData;
  }
  convertLocalDate(date: string) {
    var dateArray = date.split('/');
    var convertedDate = dateArray[2] + '-' + dateArray[1] + '-' + dateArray[0];
    return convertedDate;
  }

  loadSoGiaoDucData(): Promise<any> {
    return null;
    //let domain = window.location.host;
    //this.soGiaoDucData = JSON.parse(localStorage.getItem("SoGiaoDuc"));
    //if (this.soGiaoDucData == undefined || this.soGiaoDucData.Domain != domain) {
    //  return this.http
    //    .post(this._urlSoGiaoDucData, { domain: domain })
    //    .toPromise().then(
    //      res => {
    //        const data = res as any;
    //        localStorage.setItem("SoGiaoDuc", JSON.stringify(data));
    //        this.soGiaoDucData = JSON.parse(localStorage.getItem("SoGiaoDuc"));
    //      })
    //    .catch(
    //    error => {
    //      console.log(error);
    //    }
    //    );
    //}

  }
}
export class SelectOptionData
{
  id: string;
  text:string
}


