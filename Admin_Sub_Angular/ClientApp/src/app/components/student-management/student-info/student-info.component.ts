import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from '../../../services/account.service';
import { Utilities } from '../../../services/utilities';
import { User } from '../../../models/user.model';
import { UserEdit } from '../../../models/user-edit.model';
import { Role } from '../../../models/role.model';
import { Permission } from '../../../models/permission.model';
import { GlobalService } from 'src/app/services/global.service';
import { Student } from 'src/app/models/student.model';
import { StudentService } from 'src/app/services/student.service';
import { IMyDpOptions } from 'mydatepicker';

@Component({
  selector: 'student-info',
  templateUrl: './student-info.component.html',
  styleUrls: ['./student-info.component.scss']
})
export class StudentInfoComponent implements OnInit {
  private isEditMode = false;
  private isNewUser = false;
  private isSaving = false;
  private isChangePassword = false;
  private isEditingSelf = false;
  private showValidationErrors = false;
  private editingUserName: string;
  private uniqueId: string = Utilities.uniqueId();
  private entity: Student = new Student();
  private userCreate: User = new User();
  private userEdit: Student;
  private allRoles: Role[] = [];
  private disabledStudentID=false;
  public formResetToggle = true;
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  lstRole:any;

  @Input()
  isViewOnly: boolean;

  @ViewChild('f')
  private form;


  constructor(private alertService: AlertService, 
    private studentService: StudentService,
    private accoutService: AccountService,
    private globalService:GlobalService,
    ) {}
  public model: any = { date: { year: 2018, month: 10, day: 9 } };
  ngOnInit() {
    this.entity.activity=true;
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
    if(this.entity.dateBornUTC!=undefined)
    this.entity.dateBorn=this.entity.dateBornUTC.formatted;
    this.entity.userCreate=this.accoutService.currentUser.userName;
	this.studentService.newUser(this.entity).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
  }


  private saveSuccessHelper() {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.isChangePassword = false;
    this.showValidationErrors = false;
    if(this.entity.id==null)
    this.alertService.showMessage('Success', `Tài khoản \"${this.entity.studentId}\" được tạo thành công`, MessageSeverity.success);
    else
    this.alertService.showMessage('Success', `Tài khoản \"${this.entity.studentId}\" được cập nhật thành công`, MessageSeverity.success);
    this.entity= new Student();
    this.entity.activity=true;
    if (this.changesSavedCallback) {
      this.changesSavedCallback();
    }
  }


  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
   this.alertService.showMessage("",error.data,MessageSeverity.error);

    if (this.changesFailedCallback) {
      this.changesFailedCallback();
    }
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

  editUser(user: Student, allRoles: Role[]) {
    if (user) {
      this.isNewUser = false;
      this.userEdit = new Student();
      
      Object.assign(this.entity, user);
     this.disabledStudentID=true;
	if(this.entity.dateBorn){
      let item=this.entity.dateBorn.split('/');
      let itemtemp=this.entity.dateBorn.split('/');
    
     if(+item[1]<10)
     {
      item[1]=item[1].replace('0','');
     }
     if(+item[0]<10)
     {
      item[0]=item[0].replace('0','');
     }
     let item1= {
      'date':{
      'year': item[2],
      'month': item[1],
      'day': item[0]},
    'formatted':itemtemp[0]+'/'+itemtemp[1]+'/'+itemtemp[2]};
    this.entity.dateBornUTC=item1;
    }
  }
  return new Student();
}
}
