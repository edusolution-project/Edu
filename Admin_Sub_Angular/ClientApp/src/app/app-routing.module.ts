

import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { LoginComponent } from "./components/login/login.component";
import { HomeComponent } from "./components/home/home.component";
import { CustomersComponent } from "./components/customers/customers.component";
import { ProductsComponent } from "./components/products/products.component";
import { OrdersComponent } from "./components/orders/orders.component";
import { SettingsComponent } from "./components/settings/settings.component";
import { AboutComponent } from "./components/about/about.component";
import { NotFoundComponent } from "./components/not-found/not-found.component";
import { AuthService } from './services/auth.service';
import { AuthGuard } from './services/auth-guard.service';
import { HuongDanComponent } from './components/huong-dan/huongdan.component';
import { QuyDinhTuyenSinhComponent } from './components/quy-dinh-tuyen-sinh/quy-dinh-tuyen-sinh.component';
import { TraCuuHoSoDuTuyenComponent } from './components/tra-cuu-ho-so-du-tuyen/tra-cuu-ho-so-du-tuyen.component';
import { TraCuuDungTuyenComponent } from './components/tra-cuu-dung-tuyen/tra-cuu-dung-tuyen.component';
import { DangKyTuyenSinhComponent } from './components/dang-ky-tuyen-sinh/dang-ky-tuyen-sinh.component';
import { DangKyTuyenSinhCap1Component } from './components/dang-ky-tuyen-sinh/dang-ky-tuyen-sinh-cap1.component';
import { DangKyTuyenSinhCap2Component } from './components/dang-ky-tuyen-sinh/dang-ky-tuyen-sinh-cap2.component';
import { DKThanhCongComponent } from './components/dang-ky-tuyen-sinh/dk-thanh-cong.component';
import { TraCuuTheoTruongComponent } from './components/tra-cuu-theo-truong/tra-cuu-theo-truong.component';
import { BangComponent } from './components/bang/bang.component';
import { DangKyTuyenSinhCapMamNonComponent } from './components/dang-ky-tuyen-sinh/dang-ky-tuyen-sinh-cap-mam-non.component';



const routes: Routes = [
  { path: "", component: HomeComponent, canActivate: [AuthGuard], data: { title: "Home" } },
  { path: "login", component: LoginComponent, data: { title: "Login" } },
  { path: "customers", component: CustomersComponent, canActivate: [AuthGuard], data: { title: "Customers" } },
  { path: "products", component: ProductsComponent, canActivate: [AuthGuard], data: { title: "Products" } },
  { path: "orders", component: OrdersComponent, canActivate: [AuthGuard], data: { title: "Orders" } },
  { path: "settings", component: SettingsComponent, canActivate: [AuthGuard], data: { title: "Settings" } },
  { path: "about", component: AboutComponent, data: { title: "About Us" } },
  { path: "home", redirectTo: "/", pathMatch: "full" },
  { path: "huong-dan", component: HuongDanComponent, data: {title:"Hướng dẫn"} },
  { path: "quy-dinh-tuyen-sinh", component: QuyDinhTuyenSinhComponent, data: { title: "Quy định tuyển sinh" } },
  { path: "tra-cuu-ho-so-du-tuyen", component: TraCuuHoSoDuTuyenComponent, data: { title: "Tra cứu kết quả" } },
  { path: "tra-cuu-dung-tuyen", component: TraCuuDungTuyenComponent, data: { title: "Tra cứu đúng tuyến" } },
  { path: "dang-ky-tuyen-sinh", component: DangKyTuyenSinhComponent, data: { title: "Đăng ký tuyển sinh" } },
  { path: "dang-ky-tuyen-sinh-cap1/:id", component: DangKyTuyenSinhCap1Component, data: { title: "Đăng ký tuyển sinh" } },
  { path: "dang-ky-tuyen-sinh-cap2/:id", component: DangKyTuyenSinhCap2Component, data: { title: "Đăng ký tuyển sinh" } },
  { path: "dang-ky-tuyen-sinh-cap-mam-non/:id", component: DangKyTuyenSinhCapMamNonComponent, data: { title: "Đăng ký tuyển sinh" } },
  { path: "dk-thanh-cong", component: DKThanhCongComponent, data: { title: "Đăng ký thành công" } },
  { path: "tra-cuu-theo-truong", component: TraCuuTheoTruongComponent, data: { title: "Đăng ký thành công" } },
  { path: "bang", component: BangComponent, data: { title: "Nhập bảng" } },
  { path: "**", component: NotFoundComponent, data: { title: "Page Not Found" } }
 
];


@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
  providers: [AuthService, AuthGuard]
})
export class AppRoutingModule { }
