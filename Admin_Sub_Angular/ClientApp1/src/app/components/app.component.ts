// ====================================================
// More Templates: https://www.ebenmonney.com/templates
// Email: support@ebenmonney.com
// ====================================================

import { Component, ViewEncapsulation, OnInit, OnDestroy, ViewChildren, AfterViewInit, QueryList, ElementRef } from "@angular/core";
import { Router, NavigationStart } from '@angular/router';
import { ToastaService, ToastaConfig, ToastOptions, ToastData } from 'ngx-toasta';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, AlertDialog, DialogType, AlertMessage, MessageSeverity } from '../services/alert.service';
import { NotificationService } from "../services/notification.service";
import { AppTranslationService } from "../services/app-translation.service";
import { AccountService } from '../services/account.service';
import { LocalStoreManager } from '../services/local-store-manager.service';
import { AppTitleService } from '../services/app-title.service';
import { AuthService } from '../services/auth.service';
import { ConfigurationService } from '../services/configuration.service';
import { Permission } from '../models/permission.model';
import { LoginComponent } from "../components/login/login.component";
import { Restangular } from "ngx-restangular";
import { forEach } from "@angular/router/src/utils/collection";
import { GlobalService } from "../services/global.service";

var alertify: any = require('../assets/scripts/alertify.js');


@Component({
  selector: "app-root",
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class AppComponent implements OnInit, AfterViewInit {

  isAppLoaded: boolean;
  isUserLoggedIn: boolean;
  shouldShowLoginModal: boolean;
  removePrebootScreen: boolean;
  newNotificationCount = 0;
  appTitle = "Hệ thống học trực tuyến";
  appLogo = "";
  soGiaoDucData: SoGiaoDucData;
  parentMessage = "message from parent"
  lstMenu = [
    {
      'name': 'Quy định tuyển sinh',
      'class': 'fa fa-file-text',
      'link': 'quy-Dinh-Tuyen-Sinh',
      'active': false
    },
    {
      'name': 'Tra cứu đúng tuyến',
      'class': 'fa fa-search',
      'link': 'tra-cuu-dung-tuyen',
      'active': false
    },
    {
      'name': 'Thông tin tuyển sinh',
      'class': 'fa fa-graduation-cap',
      'link': 'tra-cuu-theo-truong',
      'active': false
    },
    {
      'name': 'Đăng ký tuyển sinh',
      'class': 'fa fa-pencil',
      'link': 'dang-ky-tuyen-sinh',
      'active': false
    },
    {
      'name': 'Hướng dẫn',
      'class': 'fa fa-file-text',
      'link': 'huong-dan',
      'active': false
    },
    {

      'name': 'Tra cứu kết quả',
      'class': 'fa fa-search',
      'link': 'tra-cuu-ho-so-du-tuyen',
      'active': false
    }
  ];
  test: SoGiaoDucData = new SoGiaoDucData();
  isViewContentLoading = false ;
  
  stickyToasties: number[] = [];

  dataLoadingConsecutiveFailurs = 0;
  notificationsLoadingSubscription: any;
  currentYear: number =0;


  @ViewChildren('loginModal,loginControl')
  modalLoginControls: QueryList<any>;

  loginModal: ModalDirective;
  loginControl: LoginComponent;


  get notificationsTitle() {

    let gT = (key: string) => this.translationService.getTranslation(key);

    if (this.newNotificationCount)
      return `${gT("app.Notifications")} (${this.newNotificationCount} ${gT("app.New")})`;
    else
      return gT("app.Notifications");
  }


  constructor(storageManager: LocalStoreManager, private toastaService: ToastaService, private toastaConfig: ToastaConfig,
    private accountService: AccountService, private alertService: AlertService, private notificationService: NotificationService, private appTitleService: AppTitleService,
    private authService: AuthService, private translationService: AppTranslationService, public configurations: ConfigurationService, public router: Router, private restangular: Restangular,
    private globalService: GlobalService) {
    storageManager.initialiseStorageSyncListener();
    translationService.addLanguages(["en", "fr", "de", "pt", "ar", "ko"]);
    translationService.setDefaultLanguage('en');
    this.toastaConfig.theme = 'bootstrap';
    this.toastaConfig.position = 'top-right';
    this.toastaConfig.limit = 100;
    this.toastaConfig.showClose = true;
    this.appTitleService.appName = this.appTitle;
    
  }


  ngAfterViewInit() {
    this.modalLoginControls.changes.subscribe((controls: QueryList<any>) => {
      controls.forEach(control => {
        if (control) {
          if (control instanceof LoginComponent) {
            this.loginControl = control;
            this.loginControl.modalClosedCallback = () => this.loginModal.hide();
          }
          else {
            this.loginModal = control;
            this.loginModal.show();
          }
        }
      });
    });
  }


  onLoginModalShown() {
    this.alertService.showStickyMessage("Session Expired", "Your Session has expired. Please log in again", MessageSeverity.info);
  }


  onLoginModalHidden() {
    this.alertService.resetStickyMessage();
    this.loginControl.reset();
    this.shouldShowLoginModal = false;

    if (this.authService.isSessionExpired)
      this.alertService.showStickyMessage("Session Expired", "Your Session has expired. Please log in again to renew your session", MessageSeverity.warn);
  }


  onLoginModalHide() {
    this.alertService.resetStickyMessage();
  }

  
   ngOnInit() {
     this.isUserLoggedIn = this.authService.isLoggedIn;
     //this.soGiaoDucData = this.globalService.soGiaoDucData;
     this.soGiaoDucData = JSON.parse(localStorage.getItem("SoGiaoDuc"));
     //this.appLogo = this.soGiaoDucData.LogoUrl;
    this.alertService.getDialogEvent().subscribe(alert => this.showDialog(alert));
    this.alertService.getMessageEvent().subscribe(message => this.showToast(message, false));
    this.alertService.getStickyMessageEvent().subscribe(message => this.showToast(message, true));

    this.authService.reLoginDelegate = () => this.shouldShowLoginModal = true;

    this.authService.getLoginStatusEvent().subscribe(isLoggedIn => {
      this.isUserLoggedIn = isLoggedIn;


      if (this.isUserLoggedIn) {
        this.initNotificationsLoading();
      }
      else {
        this.unsubscribeNotifications();
      }

      setTimeout(() => {
        if (!this.isUserLoggedIn) {
          this.alertService.showMessage("Session Ended!", "", MessageSeverity.default);
        }
      }, 500);
    });

    this.router.events.subscribe(event => {
      if (event instanceof NavigationStart) {
        let url = (<NavigationStart>event).url;

        if (url !== url.toLowerCase()) {
          this.router.navigateByUrl((<NavigationStart>event).url.toLowerCase());
        }
      }
    });
  }


  ngOnDestroy() {
    this.unsubscribeNotifications();
  }


  private unsubscribeNotifications() {
    if (this.notificationsLoadingSubscription)
      this.notificationsLoadingSubscription.unsubscribe();
  }

  initNotificationsLoading() {

    this.notificationsLoadingSubscription = this.notificationService.getNewNotificationsPeriodically()
      .subscribe(notifications => {
        this.dataLoadingConsecutiveFailurs = 0;
        this.newNotificationCount = notifications.filter(n => !n.isRead).length;
      },
        error => {
          this.alertService.logError(error);

          if (this.dataLoadingConsecutiveFailurs++ < 20)
            setTimeout(() => this.initNotificationsLoading(), 5000000);
          else
            this.alertService.showStickyMessage("Load Error", "Loading new notifications from the server failed!", MessageSeverity.error);
        });
  }


  markNotificationsAsRead() {

    let recentNotifications = this.notificationService.recentNotifications;

    if (recentNotifications.length) {
      this.notificationService.readUnreadNotification(recentNotifications.map(n => n.id), true)
        .subscribe(response => {
          for (let n of recentNotifications) {
            n.isRead = true;
          }

          this.newNotificationCount = recentNotifications.filter(n => !n.isRead).length;
        },
          error => {
            this.alertService.logError(error);
            this.alertService.showMessage("Notification Error", "Marking read notifications failed", MessageSeverity.error);

          });
    }
  }



  showDialog(dialog: AlertDialog) {

    alertify.set({
      labels: {
        ok: dialog.okLabel || "OK",
        cancel: dialog.cancelLabel || "Cancel"
      }
    });

    switch (dialog.type) {
      case DialogType.alert:
        alertify.alert(dialog.message);

        break
      case DialogType.confirm:
        alertify
          .confirm(dialog.message, (e) => {
            if (e) {
              dialog.okCallback();
            }
            else {
              if (dialog.cancelCallback)
                dialog.cancelCallback();
            }
          });

        break;
      case DialogType.prompt:
        alertify
          .prompt(dialog.message, (e, val) => {
            if (e) {
              dialog.okCallback(val);
            }
            else {
              if (dialog.cancelCallback)
                dialog.cancelCallback();
            }
          }, dialog.defaultValue);

        break;
    }
  }





  showToast(message: AlertMessage, isSticky: boolean) {

    if (message == null) {
      for (let id of this.stickyToasties.slice(0)) {
        this.toastaService.clear(id);
      }

      return;
    }

    let toastOptions: ToastOptions = {
      title: message.summary,
      msg: message.detail,
      timeout: isSticky ? 0 : 4000
    };


    if (isSticky) {
      toastOptions.onAdd = (toast: ToastData) => this.stickyToasties.push(toast.id);

      toastOptions.onRemove = (toast: ToastData) => {
        let index = this.stickyToasties.indexOf(toast.id, 0);

        if (index > -1) {
          this.stickyToasties.splice(index, 1);
        }

        toast.onAdd = null;
        toast.onRemove = null;
      };
    }


    switch (message.severity) {
      case MessageSeverity.default: this.toastaService.default(toastOptions); break;
      case MessageSeverity.info: this.toastaService.info(toastOptions); break;
      case MessageSeverity.success: this.toastaService.success(toastOptions); break;
      case MessageSeverity.error: this.toastaService.error(toastOptions); break;
      case MessageSeverity.warn: this.toastaService.warning(toastOptions); break;
      case MessageSeverity.wait: this.toastaService.wait(toastOptions); break;
    }
  }





  logout() {
    this.authService.logout();
    this.authService.redirectLogoutUser();
  }


  getYear() {
    return new Date().getUTCFullYear();
  }

  test1() {
    alert("1");
  }

  get userName(): string {
    return "Nguyễn Chí Nghiệp";
    //return this.authService.currentUser ? this.authService.currentUser.userName : "";
  }


  get fullName(): string {
    return this.authService.currentUser ? this.authService.currentUser.fullName : "";
  }



  get canViewCustomers() {
    return this.accountService.userHasPermission(Permission.viewUsersPermission); //eg. viewCustomersPermission
  }

  get canViewProducts() {
    return this.accountService.userHasPermission(Permission.viewUsersPermission); //eg. viewProductsPermission
  }

  get canViewOrders() {
    return true; //eg. viewOrdersPermission
  }


  selectMenu(item) {
    this.router.navigate([item.link]);
    this.lstMenu.forEach((item1) => {
      item1.active = false;
    });
    item.active = true;
  }
  get isShow() {

    if (this.router.url == "/") {
      return true;
    }
    else
      return false;
  }

  set(key: string, data: any): void {
    try {
      localStorage.setItem(key, JSON.stringify(data));
    } catch (e) {
      console.error('Error saving to localStorage', e);
    }
  }
  get(key: string) {
    try {
      return JSON.parse(localStorage.getItem(key));
    } catch (e) {
      console.error('Error getting data from localStorage', e);
      return null;
    }
  }
 


}
export  class SoGiaoDucData {
    
  public MaSo: string;
  public TieuDe2: string;
  public TieuDe1: string;
  public LogoUrl: string;
  public DiaChiSoGd: string;

}
