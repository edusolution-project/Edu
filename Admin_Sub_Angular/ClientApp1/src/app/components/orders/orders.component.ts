// ====================================================
// More Templates: https://www.ebenmonney.com/templates
// Email: support@ebenmonney.com
// ====================================================

import { Component } from '@angular/core';
import { fadeInOut } from '../../services/animations';

@Component({
    selector: 'orders',
    templateUrl: './orders.component.html',
    styleUrls: ['./orders.component.css'],
    animations: [fadeInOut]
})
export class OrdersComponent {
  isProfileActivated = true;
  isPreferencesActivated = false;
  isUsersActivated = false;
  isRolesActivated = false;
  readonly profileTab = "profile";
  readonly preferencesTab = "preferences";
  readonly usersTab = "users";
  readonly rolesTab = "roles";
  onShowTab(event) {
    let activeTab = event.target.hash.split("#", 2).pop();
    this.isProfileActivated = activeTab == this.profileTab;
    this.isPreferencesActivated = activeTab == this.preferencesTab;
    this.isUsersActivated = activeTab == this.usersTab;
    this.isRolesActivated = activeTab == this.rolesTab;
    console.log(this.isPreferencesActivated);
  }
}
