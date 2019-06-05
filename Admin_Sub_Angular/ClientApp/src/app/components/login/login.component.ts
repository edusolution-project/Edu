// =============================
// Email: info@ebenmonney.com
// www.ebenmonney.com/templates
// =============================

import { Component, OnInit, OnDestroy, Input } from '@angular/core';

import { AlertService, MessageSeverity, DialogType } from '../../services/alert.service';
import { AuthService } from '../../services/auth.service';
import { ConfigurationService } from '../../services/configuration.service';
import { Utilities } from '../../services/utilities';
import { UserLogin } from '../../models/user-login.model';
import { Restangular } from 'ngx-restangular';
import { User } from 'src/app/models/user.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})

export class LoginComponent implements OnInit, OnDestroy {

  userLogin = new UserLogin();
  isLoading = false;
  formResetToggle = true;
  modalClosedCallback: () => void;
  loginStatusSubscription: any;
  services: any;
  @Input()
  isModal = false;


  constructor(private restangular: Restangular,
    private alertService: AlertService,
     private authService: AuthService, private configurations: ConfigurationService,
     private router: Router) {

  }


  ngOnInit() {
    this.services = this.restangular.all('Authentication');
    this.userLogin.rememberMe = this.authService.rememberMe;

    if (this.getShouldRedirect()) {
      this.authService.redirectLoginUser();
    } else {
      this.loginStatusSubscription = this.authService.getLoginStatusEvent().subscribe(isLoggedIn => {
        if (this.getShouldRedirect()) {
          this.authService.redirectLoginUser();
        }
      });
    }
  }


  ngOnDestroy() {
    if (this.loginStatusSubscription) {
      this.loginStatusSubscription.unsubscribe();
    }
  }


  getShouldRedirect() {
    return !this.isModal && this.authService.isLoggedIn && !this.authService.isSessionExpired;
  }


  showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }

  closeModal() {
    if (this.modalClosedCallback) {
      this.modalClosedCallback();
    }
  }


  login() {
    this.isLoading = true;

    this.alertService.startLoadingMessage("", "Attempting login...");
    this.services.all("LogIn").post(this.userLogin).subscribe(
      response => {
       
        this.isLoading = false;
        let user = new User(
          "name"
        );
        if(response.token!=null)
        {
        this.authService.savetest(response);
        }
        else
        {
          this.alertService.showMessage("Không đúng thông tin tên đăng nhập và mật khẩu","", MessageSeverity.error);
        }
        setTimeout(() => {
    this.isLoading = false;
    this.router.navigate(['/']);
        }, 500);
      }, error => {
            
        console.log(error);
        this.isLoading = false;
        this.alertService.showMessage("Không đúng thông tin tên đăng nhập và mật khẩu","", MessageSeverity.error);
      }
    );

    setTimeout(() => {
      this.alertService.stopLoadingMessage();
    }, 500);

    

    // this.authService.login(this.userLogin.userName, this.userLogin.password, this.userLogin.rememberMe)
    //   .subscribe(
    //     user => {
    //       setTimeout(() => {
    //         this.alertService.stopLoadingMessage();
    //         this.isLoading = false;
    //         this.reset();

    //         if (!this.isModal) {
    //           this.alertService.showMessage('Login', `Welcome ${user.userName}!`, MessageSeverity.success);
    //         } else {
    //           this.alertService.showMessage('Login', `Session for ${user.userName} restored!`, MessageSeverity.success);
    //           setTimeout(() => {
    //             this.alertService.showStickyMessage('Session Restored', 'Please try your last operation again', MessageSeverity.default);
    //           }, 500);

    //           this.closeModal();
    //         }
    //       }, 500);
    //     },
    //     error => {

    //       this.alertService.stopLoadingMessage();

    //       if (Utilities.checkNoNetwork(error)) {
    //         this.alertService.showStickyMessage(Utilities.noNetworkMessageCaption, Utilities.noNetworkMessageDetail, MessageSeverity.error, error);
    //       //  this.offerAlternateHost();
    //       } else {
    //         const errorMessage = Utilities.getHttpResponseMessage(error);

    //         if (errorMessage) {
    //           this.alertService.showStickyMessage('Unable to login', this.mapLoginErrorMessage(errorMessage), MessageSeverity.error, error);
    //         } else {
    //           this.alertService.showStickyMessage('Unable to login', 'An error occured whilst logging in, please try again later.\nError: ' + Utilities.getResponseBody(error), MessageSeverity.error, error);
    //         }
    //       }

    //       setTimeout(() => {
    //         this.isLoading = false;
    //       }, 500);
    //     });
  }


  offerAlternateHost() {

    if (Utilities.checkIsLocalHost(location.origin) && Utilities.checkIsLocalHost(this.configurations.baseUrl)) {
      this.alertService.showDialog('Dear Developer!\nIt appears your backend Web API service is not running...\n' +
        'Would you want to temporarily switch to the online Demo API below?(Or specify another)',
        DialogType.prompt,
        (value: string) => {
          this.configurations.baseUrl = value;
          this.configurations.tokenUrl = value;
          this.alertService.showStickyMessage('API Changed!', 'The target Web API has been changed to: ' + value, MessageSeverity.warn);
        },
        null,
        null,
        null,
        this.configurations.fallbackBaseUrl);
    }
  }


  mapLoginErrorMessage(error: string) {

    if (error == 'invalid_username_or_password') {
      return 'Invalid username or password';
    }

    if (error == 'invalid_grant') {
      return 'This account has been disabled';
    }

    return error;
  }


  reset() {
    this.formResetToggle = false;

    setTimeout(() => {
      this.formResetToggle = true;
    });
  }
}