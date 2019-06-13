

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
import { CourseService } from 'src/app/services/course.service';
import { Course } from 'src/app/models/course.model';


@Component({
  selector: 'course-info',
  templateUrl: './course-info.component.html',
  styleUrls: ['./course-info.component.scss']
})
export class CourseInfoComponent implements OnInit {

  private isEditMode = false;
  private isNewUser = false;
  private isSaving = false;
  private entity: Course = new Course();
  private userCreate: User = new User();
  private couseEdit: Course;
  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;
  lstRole: any;
  @Input()
  isViewOnly: boolean;

  listModProgram = [];
  listModGradle = [];
  listModSubject = [];
  listCourse = [];
  listTeacher = [];
  listStudent = [];




  @ViewChild('f')
  private form;


  constructor(private alertService: AlertService,
    private courseService: CourseService,
    private accoutService: AccountService,
    private globalService: GlobalService,
  ) {
  }
  public model: any = { date: { year: 2018, month: 10, day: 9 } };
  ngOnInit() {
    this.entity.activity = true;
    this.courseService.getListModProgram().subscribe(
      response => {
        this.listModProgram = this.globalService.convertObjectToSelect2(response, "id", "name");
        if (this.listModProgram.length > 0) {
          this.entity.programID = this.listModProgram[0]["id"];
        }
      },
      error => {
        console.log(error);
      }
    );

    this.courseService.getListModGradle().subscribe(
      response => {
        this.listModGradle = this.globalService.convertObjectToSelect2(response, "id", "name");
        if (this.listModGradle.length > 0) {
          this.entity.gradeID = this.listModGradle[0]["id"];
        }
      },
      error => {
        console.log(error);
      }
    );

   
    this.courseService.getListModSubject().subscribe(
      response => {
        this.listModSubject = this.globalService.convertObjectToSelect2(response, "id", "name");
        if (this.listModSubject.length > 0) {
          this.entity.subjectID = this.listModSubject[0]["id"];
        }
      },
      error => {
        console.log(error);
      }
    );
    setTimeout(() => {
      this.courseService.getListCourse(this.entity.subjectID, this.entity.gradeID, this.entity.programID).subscribe(
        response => {
          this.listCourse = this.globalService.convertObjectToSelect2(response, "id", "name");
          if (this.listCourse.length > 0) {
            this.entity.courseID = this.listCourse[0]["id"];
          }
        },
        error => {
          console.log(error);
        }
      );
    }, 1000);

    this.courseService.getListTeacher(this.accoutService.currentUser.userName).subscribe(
      response => {
        this.listTeacher = this.globalService.convertObjectToSelect2(response.data, "teacherId", "fullName");
        if (this.listTeacher.length > 0) {
          this.entity.teacherID = this.listTeacher[0]["id"];
        }
      },
      error => {
        console.log(error);
      }
    );

    this.courseService.getListStudent(this.accoutService.currentUser.userName).subscribe(
      response => {
        this.listStudent = this.globalService.convertObjectToSelect2(response.data, "studentId", "fullName");
      },
      error => {
        console.log(error);
      }
    );



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
    if (this.entity.createdDateUTC != undefined)
      this.entity.createdDate = this.entity.createdDateUTC.formatted;

    if (this.entity.endedDateUTC != undefined)
      this.entity.endedDate = this.entity.endedDateUTC.formatted;
    this.entity.userCreate = this.accoutService.currentUser.userName;
    this.courseService.newUser(this.entity).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
  }


  private saveSuccessHelper() {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    if (this.entity.id == null)
      this.alertService.showMessage('Success', 'Tạo khóa học thành công', MessageSeverity.success);
    else
      this.alertService.showMessage('Success', 'Cập nhật khóa học thành công', MessageSeverity.success);
    this.entity = new Course();
    this.entity.activity = true;
    if (this.changesSavedCallback) {
      this.changesSavedCallback();
    }
  }


  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showMessage("", error.data, MessageSeverity.error);

    if (this.changesFailedCallback) {
      this.changesFailedCallback();
    }
  }






  private cancel() {

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


  editUser(user: Course,) {
    if (user) {
      this.isNewUser = false;
      Object.assign(this.entity, user);
      if (this.entity.createdDate) {
        let item = this.entity.createdDate.split('/');
        let itemtemp = this.entity.createdDate.split('/');

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
        this.entity.createdDateUTC=item1;
      }

      if (this.entity.endedDate) {
        let item = this.entity.endedDate.split('/');
        let itemtemp = this.entity.endedDate.split('/');

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
        this.entity.endedDateUTC = item1;
      }
    }
    return new Course();
  }
}
