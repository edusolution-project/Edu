// ====================================================
// More Templates: https://www.ebenmonney.com/templates
// Email: support@ebenmonney.com
// ====================================================

import { NgModule, ErrorHandler, APP_INITIALIZER  } from "@angular/core";
import { RouterModule } from "@angular/router";
import { FormsModule } from "@angular/forms";
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { TranslateModule, TranslateLoader } from "@ngx-translate/core";
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { ToastaModule } from 'ngx-toasta';
import { ModalModule } from 'ngx-bootstrap/modal';
import { TooltipModule } from "ngx-bootstrap/tooltip";
import { PopoverModule } from "ngx-bootstrap/popover";
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { CarouselModule } from 'ngx-bootstrap/carousel';
import { ChartsModule } from 'ng2-charts';
import { RestangularModule, Restangular } from 'ngx-restangular';
import { MyDatePickerModule } from 'mydatepicker';

import { AppRoutingModule } from './app-routing.module';
import { AppErrorHandler } from './app-error.handler';
import { AppTitleService } from './services/app-title.service';
import { AppTranslationService, TranslateLanguageLoader } from './services/app-translation.service';
import { ConfigurationService } from './services/configuration.service';
import { AlertService } from './services/alert.service';
import { LocalStoreManager } from './services/local-store-manager.service';
import { EndpointFactory } from './services/endpoint-factory.service';
import { NotificationService } from './services/notification.service';
import { NotificationEndpoint } from './services/notification-endpoint.service';
import { AccountService } from './services/account.service';
import { AccountEndpoint } from './services/account-endpoint.service';

import { EqualValidator } from './directives/equal-validator.directive';
import { LastElementDirective } from './directives/last-element.directive';
import { AutofocusDirective } from './directives/autofocus.directive';
import { BootstrapTabDirective } from './directives/bootstrap-tab.directive';
import { BootstrapToggleDirective } from './directives/bootstrap-toggle.directive';
import { BootstrapSelectDirective } from './directives/bootstrap-select.directive';
import { BootstrapDatepickerDirective } from './directives/bootstrap-datepicker.directive';
import { GroupByPipe } from './pipes/group-by.pipe';

import { AppComponent } from "./components/app.component";
import { LoginComponent } from "./components/login/login.component";
import { HomeComponent } from "./components/home/home.component";
import { CustomersComponent } from "./components/customers/customers.component";
import { ProductsComponent } from "./components/products/products.component";
import { OrdersComponent } from "./components/orders/orders.component";
import { SettingsComponent } from "./components/settings/settings.component";
import { AboutComponent } from "./components/about/about.component";
import { NotFoundComponent } from "./components/not-found/not-found.component";

import { BannerDemoComponent } from "./components/controls/banner-demo.component";
import { TodoDemoComponent } from "./components/controls/todo-demo.component";
import { StatisticsDemoComponent } from "./components/controls/statistics-demo.component";
import { NotificationsViewerComponent } from "./components/controls/notifications-viewer.component";
import { SearchBoxComponent } from "./components/controls/search-box.component";
import { UserInfoComponent } from "./components/controls/user-info.component";
import { UserPreferencesComponent } from "./components/controls/user-preferences.component";
import { UsersManagementComponent } from "./components/controls/users-management.component";
import { RolesManagementComponent } from "./components/controls/roles-management.component";
import { RoleEditorComponent } from "./components/controls/role-editor.component";
import { HuongDanComponent, hdTraCuuDungTuyenComponent, hdTraCuuTTTuyenSinhComponent, hdDangKyTuyenSinhComponent, hdTraCuuKetQuaComponent } from "./components/huong-dan/huongdan.component";
import { QuyDinhTuyenSinhComponent } from "./components/quy-dinh-tuyen-sinh/quy-dinh-tuyen-sinh.component";
import { TraCuuHoSoDuTuyenComponent } from "./components/tra-cuu-ho-so-du-tuyen/tra-cuu-ho-so-du-tuyen.component";
import { GlobalService } from "./services/global.service";
import { TraCuuDungTuyenComponent } from "./components/tra-cuu-dung-tuyen/tra-cuu-dung-tuyen.component";
import { NgxSelectModule } from 'ngx-select-ex';
import { DatePipe } from "@angular/common";
import { DangKyTuyenSinhComponent } from "./components/dang-ky-tuyen-sinh/dang-ky-tuyen-sinh.component";
import { DangKyTuyenSinhCap1Component } from "./components/dang-ky-tuyen-sinh/dang-ky-tuyen-sinh-cap1.component";
import { DKThanhCongComponent } from "./components/dang-ky-tuyen-sinh/dk-thanh-cong.component";
import { ViewTTinTsinhTruongComponent } from "./components/tra-cuu-dung-tuyen/view-ttin-tsinh-truong.component";
import { TraCuuTheoTruongComponent } from "./components/tra-cuu-theo-truong/tra-cuu-theo-truong.component";
import { DangKyTuyenSinhCap2Component } from "./components/dang-ky-tuyen-sinh/dang-ky-tuyen-sinh-cap2.component";
import {  BangComponent } from "./components/bang/bang.component";
import { BangData } from "./services/bang-data.service";
import { ScrollingModule } from '@angular/cdk/scrolling';
import { GraphQLModule } from "./graphql.module";
import { DangKyTuyenSinhCapMamNonComponent } from "./components/dang-ky-tuyen-sinh/dang-ky-tuyen-sinh-cap-mam-non.component";
@NgModule({
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    AppRoutingModule,
    ScrollingModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useClass: TranslateLanguageLoader
      }
    }),
    NgxDatatableModule,
    ToastaModule.forRoot(),
    TooltipModule.forRoot(),
    PopoverModule.forRoot(),
    BsDropdownModule.forRoot(),
    CarouselModule.forRoot(),
    ModalModule.forRoot(),
    ChartsModule,
    RestangularModule.forRoot(RestangularConfigFactory),
    NgxSelectModule,
    MyDatePickerModule,
    GraphQLModule
     
  ],
  declarations: [
    AppComponent,
    LoginComponent,
    HomeComponent,
    CustomersComponent,
    ProductsComponent,
    OrdersComponent,
    SettingsComponent,
    UsersManagementComponent, UserInfoComponent, UserPreferencesComponent,
    RolesManagementComponent, RoleEditorComponent,
    AboutComponent,
    NotFoundComponent,
    NotificationsViewerComponent,
    SearchBoxComponent,
    StatisticsDemoComponent, TodoDemoComponent, BannerDemoComponent,
    EqualValidator,
    LastElementDirective,
    AutofocusDirective,
    BootstrapTabDirective,
    BootstrapToggleDirective,
    BootstrapSelectDirective,
    BootstrapDatepickerDirective,
    GroupByPipe,
    HuongDanComponent,
    hdTraCuuDungTuyenComponent,
    hdTraCuuTTTuyenSinhComponent,
    hdDangKyTuyenSinhComponent,
    hdTraCuuKetQuaComponent,
    QuyDinhTuyenSinhComponent,
    TraCuuHoSoDuTuyenComponent,
    TraCuuDungTuyenComponent,
    DangKyTuyenSinhComponent,
    DangKyTuyenSinhCap1Component,
    DangKyTuyenSinhCap2Component,
    DangKyTuyenSinhCapMamNonComponent,
    DKThanhCongComponent,
    ViewTTinTsinhTruongComponent,
    TraCuuTheoTruongComponent,
    BangComponent
  ],
  providers: [
    { provide: 'BASE_URL', useFactory: GlobalService.getBaseUrl },
    { provide: ErrorHandler, useClass: AppErrorHandler },
    { provide: APP_INITIALIZER, useFactory: loadCurrentYearFactory, deps: [GlobalService], multi: true },
    { provide: APP_INITIALIZER, useFactory: loadSoGiaoDucDataFactory, deps: [GlobalService], multi: true },
    [DatePipe],
    AlertService,
    ConfigurationService,
    AppTitleService,
    AppTranslationService,
    NotificationService,
    NotificationEndpoint,
    AccountService,
    AccountEndpoint,
    LocalStoreManager,
    EndpointFactory,
    GlobalService,
    BangData
  ],
  bootstrap: [AppComponent]
})
export class AppModule {

}

export function RestangularConfigFactory(RestangularProvider) {
  RestangularProvider.setBaseUrl(GlobalService.getBaseUrl + "api");
  //RestangularProvider.setDefaultHeaders({ 'Authorization': 'Bearer UDXPx-Xko0w4BRKajozCVy20X11MRZs1' });
}
export function loadCurrentYearFactory(startupService: GlobalService): Function {
  return () => startupService.loadCurrentYear();
}


export function loadSoGiaoDucDataFactory(startupService: GlobalService): Function {
  return () => startupService.loadSoGiaoDucData();
}


