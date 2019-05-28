

import { Component, OnInit, ViewChild, Input } from '@angular/core';

import { AlertService, MessageSeverity } from '../../services/alert.service';
import { AccountService } from '../../services/account.service';
import { Utilities } from '../../services/utilities';
import { User } from '../../models/user.model';
import { UserEdit } from '../../models/user-edit.model';
import { Role } from '../../models/role.model';
import { Permission } from '../../models/permission.model';
import { GlobalService } from 'src/app/services/global.service';
import { Teacher } from 'src/app/models/teacher.model';
import { TeacherService } from 'src/app/services/teacher.service';
import { IMyDpOptions } from 'mydatepicker';


@Component({
  selector: 'teacher-import',
  templateUrl: './teacher-import.component.html'

})
export class TeacherImportComponent implements OnInit {

  private isEditMode = false;
  private isNewUser = false;
  private isSaving = false;
  private isChangePassword = false;
  private isEditingSelf = false;
  private showValidationErrors = false;
  private editingUserName: string;
  private uniqueId: string = Utilities.uniqueId();
  private entity: Teacher = new Teacher();
  private userCreate: User = new User();
  private userEdit: Teacher;
  private allRoles: Role[] = [];
  private disabledTeacherID = false;
  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;
  lstRole: any;
  @Input()
  isViewOnly: boolean;







  @ViewChild('f')
  private form;


  constructor(private alertService: AlertService,
    private teacherService: TeacherService,
    private accoutService: AccountService,
    private globalService: GlobalService,
  ) {
  }
  public model: any = { date: { year: 2018, month: 10, day: 9 } };
  ngOnInit() {
    this.entity.activity = true;


  }
  public myDatePickerOptions: IMyDpOptions = {
    inline: false,
    editableDateField: false,
    openSelectorOnInputClick: true,
    dateFormat: 'dd/mm/yyyy'
  };



  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }
  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage('Saving changes...');
    if (this.entity.dateBornUTC != undefined)
      this.entity.dateBorn = this.entity.dateBornUTC.formatted;
    this.entity.userCreate = this.accoutService.currentUser.userName;
    this.teacherService.newUser(this.entity).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
  }


  private saveSuccessHelper() {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.isChangePassword = false;
    this.showValidationErrors = false;
    if (this.entity.id == null)
      this.alertService.showMessage('Success', `Tài khoản \"${this.entity.teacherId}\" được tạo thành công`, MessageSeverity.success);
    else
      this.alertService.showMessage('Success', `Tài khoản \"${this.entity.teacherId}\" được cập nhật thành công`, MessageSeverity.success);
    this.entity = new Teacher();
    this.entity.activity = true;
    if (this.changesSavedCallback) {
      this.changesSavedCallback();
    }
  }


  private saveFailedHelper(error: any) {
    console.log(error);
   // this.isSaving = false;
    //this.alertService.stopLoadingMessage();
    this.alertService.showMessage("Error", error.data, MessageSeverity.error);

    if (this.changesFailedCallback) {
      this.changesFailedCallback();
    }
  }

  lstFile = [];
  addFile(item) {
    this.lstFile.push(item[0]);

    let formData = new FormData();
    formData.append('file', this.lstFile[0], this.lstFile[0].name);
    console.log(this.lstFile);
    console.log(formData);
    this.teacherService.importExcel(formData)
      .subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    

  }



  private cancel() {

    this.showValidationErrors = false;
    this.resetForm();
    this.alertService.resetStickyMessage();
    if (this.changesCancelledCallback) {
      this.changesCancelledCallback();
    }
  }

  resetForm(replace = false) {

    if (!replace) {
      this.form.reset();
    } else {
      this.formResetToggle = false;

      setTimeout(() => {
        this.formResetToggle = true;
      });
    }
  }


  editUser(user: Teacher, allRoles: Role[]) {
    if (user) {
      this.isNewUser = false;
      this.userEdit = new Teacher();

      Object.assign(this.entity, user);
      this.disabledTeacherID = true;
      if (this.entity.dateBorn) {
        let item = this.entity.dateBorn.split('/');
        let itemtemp = this.entity.dateBorn.split('/');

        if (+item[1] < 10) {
          item[1] = item[1].replace('0', '');
        }
        if (+item[0] < 10) {
          item[0] = item[0].replace('0', '');
        }
        let item1 = {
          'date': {
            'year': item[2],
            'month': item[1],
            'day': item[0]
          },
          'formatted': itemtemp[0] + '/' + itemtemp[1] + '/' + itemtemp[2]
        };
        this.entity.dateBornUTC = item1;
      }
    }
    return new Teacher();
  }
}
