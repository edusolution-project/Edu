

import { Component } from '@angular/core';
import { fadeInOut } from '../../services/animations';
import { ConfigurationService } from '../../services/configuration.service';
import { AccountService } from 'src/app/services/account.service';


@Component({
    selector: 'home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss'],
    animations: [fadeInOut]
})
export class HomeComponent {


  test = "";
  condition = false;
  constructor(public configurations: ConfigurationService, private accoutService: AccountService) {
      
    let user = accoutService.currentUser;
    if (user.roleID == "GIAOVIEN") {
      this.test = "212121212121";
      this.condition = true;

    }
    }
}
