// =============================
// Email: info@ebenmonney.com
// www.ebenmonney.com/templates
// =============================

import { Component, OnInit, ViewChild, Input } from '@angular/core';

import { AlertService, MessageSeverity } from '../../services/alert.service';
import { AccountService } from '../../services/account.service';
import { Utilities } from '../../services/utilities';
import { User } from '../../models/user.model';
import { UserEdit } from '../../models/user-edit.model';
import { Role } from '../../models/role.model';
import { Permission } from '../../models/permission.model';
import { GlobalService } from 'src/app/services/global.service';


@Component({
  selector: 'user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.scss']
})
export class UserInfoComponent implements OnInit {

  private isEditMode = false;
  private isNewUser = false;
  private isSaving = false;
  private isChangePassword = false;
  private isEditingSelf = false;
  private showValidationErrors = false;
  private editingUserName: string;
  private uniqueId: string = Utilities.uniqueId();
  private user: User = new User();
  private userCreate: User = new User();
  private userEdit: UserEdit;
  private allRoles: Role[] = [];

  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;
  lstRole:any;
    @Input()
  isViewOnly: boolean;

  @Input()
  isGeneralEditor = false;

private disabledUserName=false;



  @ViewChild('f')
  private form;

  // ViewChilds Required because ngIf hides template variables from global scope
  @ViewChild('userName')
  private userName;

  @ViewChild('userPassword')
  private userPassword;

  @ViewChild('email')
  private email;

  @ViewChild('currentPassword')
  private currentPassword;

  @ViewChild('newPassword')
  private newPassword;

  @ViewChild('confirmPassword')
  private confirmPassword;

  @ViewChild('roles')
  private roles;

  @ViewChild('rolesSelector')
  private rolesSelector;


  constructor(private alertService: AlertService, private accountService: AccountService,private globalService:GlobalService) {
  }

  ngOnInit() {
    let range:Role[] =[
  ];
  let item = new Role();
  item.roleID="CANBO";
  item.name="Cán bộ";
  range.push(item);

  item = new Role();
  item.roleID="LANHDAO";
  item.name="Lãnh đạo";
  range.push(item);

    this.lstRole=this.globalService.convertObjectToSelect2(range,"roleID","name");
    this.userCreate.roleID="CANBO";
    this.userCreate.activity=true;;
   
  }



  private getRoleByName(name: string) {
    return this.allRoles.find((r) => r.name == name);
  }



  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  public deletePasswordFromUser(user: UserEdit | User) {
    const userEdit = <UserEdit>user;

    delete userEdit.currentPassword;
    delete userEdit.newPassword;
    delete userEdit.confirmPassword;
  }


  private edit() {
  
    if (!this.isGeneralEditor) {
      this.isEditingSelf = true;
      this.userEdit = new UserEdit();
      Object.assign(this.userEdit, this.user);
     
    } else {
      if (!this.userEdit) {
        this.userEdit = new UserEdit();
      }

      this.isEditingSelf = this.accountService.currentUser ? this.userEdit.id == this.accountService.currentUser.id : false;
    }
    
    this.isEditMode = true;
    this.showValidationErrors = true;
    this.isChangePassword = false;
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage('Saving changes...');

this.userCreate.userNameManager=this.accountService.currentUser.userName;
    if (this.isNewUser) {
      this.accountService.newUser(this.userCreate).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    } else {
      this.accountService.newUser(this.userCreate).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
  }


  private saveSuccessHelper(user?: User) {
    if (user) {
      Object.assign(this.userEdit, user);
    }

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.isChangePassword = false;
    this.showValidationErrors = false;
    if(this.userCreate.id==null)
    this.alertService.showMessage('Success', `Tài khoản \"${this.userCreate.userName}\" được tạo thành công`, MessageSeverity.success);
    else
    this.alertService.showMessage('Success', `Tài khoản \"${this.userCreate.userName}\" được cập nhật thành công`, MessageSeverity.success);
    this.userCreate= new User();
    this.userCreate.roleID="CANBO";
    this.userCreate.activity=true;
    if (this.changesSavedCallback) {
      this.changesSavedCallback();
    }
  }


  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
   this.alertService.showMessage("",error.data,MessageSeverity.error);
    //this.alertService.showStickyMessage('Save Error', 'The below errors occured whilst saving your changes:', MessageSeverity.error, error);
    //this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    if (this.changesFailedCallback) {
      this.changesFailedCallback();
    }
  }



  private testIsRoleUserCountChanged(currentUser: User, editedUser: User) {

    const rolesAdded = this.isNewUser ? editedUser.roles : editedUser.roles.filter(role => currentUser.roles.indexOf(role) == -1);
    const rolesRemoved = this.isNewUser ? [] : currentUser.roles.filter(role => editedUser.roles.indexOf(role) == -1);

    const modifiedRoles = rolesAdded.concat(rolesRemoved);

    if (modifiedRoles.length) {
      setTimeout(() => this.accountService.onRolesUserCountChanged(modifiedRoles));
    }
  }



  private cancel() {
    if (this.isGeneralEditor) {
      this.userEdit = this.user = new UserEdit();
    } else {
      this.userEdit = new UserEdit();
    }

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (!this.isGeneralEditor) {
      this.isEditMode = false;
    }

    if (this.changesCancelledCallback) {
      this.changesCancelledCallback();
    }
  }


 





  private changePassword() {
    this.isChangePassword = true;
  }


  private unlockUser() {
    this.isSaving = true;
    this.alertService.startLoadingMessage('Unblocking user...');


    this.accountService.unblockUser(this.userEdit.id)
      .subscribe(() => {
        this.isSaving = false;
        this.userEdit.isLockedOut = false;
        this.alertService.stopLoadingMessage();
        this.alertService.showMessage('Success', 'User has been successfully unblocked', MessageSeverity.success);
      },
        error => {
          this.isSaving = false;
          this.alertService.stopLoadingMessage();
          this.alertService.showStickyMessage('Unblock Error', 'The below errors occured whilst unblocking the user:', MessageSeverity.error, error);
          this.alertService.showStickyMessage(error, null, MessageSeverity.error);
        });
  }


  resetForm(replace = false) {
    this.isChangePassword = false;

    if (!replace) {
      this.form.reset();
    } else {
      this.formResetToggle = false;

      setTimeout(() => {
        this.formResetToggle = true;
      });
    }
  }


  newUser(allRoles: Role[]) {
    this.isGeneralEditor = true;
    this.isNewUser = true;

    this.allRoles = [...allRoles];
    this.editingUserName = null;
    this.user = this.userEdit = new UserEdit();
    this.userEdit.isEnabled = true;
    this.edit();

    return this.userEdit;
  }

  editUser(user: User, allRoles: Role[]) {
    if(user.roleID=="SUPERADMIN")
    {
      return;
    }
    if (user) {
      this.isGeneralEditor = true;
      this.isNewUser = false;
      this.disabledUserName=true;
      this.setRoles(user, allRoles);
      this.editingUserName = user.userName;
      this.user = new User();
      this.userEdit = new UserEdit();
      Object.assign(this.user, user);
      Object.assign(this.userEdit, user);
      Object.assign(this.userCreate, user);
      this.edit();

      return this.userEdit;
    } else {
      return this.newUser(allRoles);
    }
  }


  displayUser(user: User, allRoles?: Role[]) {

    this.user = new User();
    Object.assign(this.user, user);
    this.deletePasswordFromUser(this.user);
    this.setRoles(user, allRoles);

    this.isEditMode = false;
  }



  private setRoles(user: User, allRoles?: Role[]) {

    this.allRoles = allRoles ? [...allRoles] : [];

    if (user.roles) {
      for (const ur of user.roles) {
        if (!this.allRoles.some(r => r.name == ur)) {
          this.allRoles.unshift(new Role(ur));
        }
      }
    }

    if (allRoles == null || this.allRoles.length != allRoles.length) {
      setTimeout(() => {
        if (this.rolesSelector) {
          this.rolesSelector.refresh();
        }
      });
    }
  }



  get canViewAllRoles() {
    return this.accountService.userHasPermission(Permission.viewRolesPermission);
  }

  get canAssignRoles() {
    return this.accountService.userHasPermission(Permission.assignRolesPermission);
  }
}
