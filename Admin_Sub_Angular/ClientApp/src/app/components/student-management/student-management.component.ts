import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../services/alert.service';
import { AppTranslationService } from '../../services/app-translation.service';
import { AccountService } from '../../services/account.service';
import { Utilities } from '../../services/utilities';
import { User } from '../../models/user.model';
import { Role } from '../../models/role.model';
import { Permission } from '../../models/permission.model';
import { UserEdit } from '../../models/user-edit.model';
import { Restangular } from 'ngx-restangular';
import { PagerService } from 'src/app/services/pager.service';
import { UserInfoComponent } from '../controls/user-info.component';
import { StudentInfoComponent } from './student-info/student-info.component';
import { StudentService } from 'src/app/services/student.service';
import { Student } from 'src/app/models/student.model';

@Component({
  selector: 'student-management',
  templateUrl: './student-management.component.html',
  styleUrls: ['./student-management.component.scss']
})
export class StudentManagementComponent implements OnInit {
	columns: any[] = [];
    rows: User[] = [];
    rowsCache: User[] = [];
    editedUser: Student;
    sourceUser: UserEdit;
    editingUserName: { name: string };
    loadingIndicator: boolean;

    allRoles: Role[] = [];
  
    @ViewChild('indexTemplate')
    indexTemplate: TemplateRef<any>;

    @ViewChild('userNameTemplate')
    userNameTemplate: TemplateRef<any>;

    @ViewChild('rolesTemplate')
    rolesTemplate: TemplateRef<any>;

    @ViewChild('actionsTemplate')
    actionsTemplate: TemplateRef<any>;

    @ViewChild('editorModal')
    editorModal: ModalDirective;

	@ViewChild('editorModalExcel')
	editorModalExcel: ModalDirective;

    @ViewChild('teacherEditor')
    userEditor: StudentInfoComponent;
  constructor(private pagerService: PagerService,
        private alertService: AlertService, 
        private studentService: StudentService,
        private accountService: AccountService) { }

  private allItems: any[];

    // pager object
    pager: any = {};

    // paged items
    pagedItems: any[];
    totalPageTemp:number=100;
    pagesTemp:any[];

    ngOnInit() {
        this.loadData(1);
      
    }



    ngAfterViewInit() {

        this.userEditor.changesSavedCallback = () => {
            this.loadData(1);
            this.editorModal.hide();
        };

        this.userEditor.changesCancelledCallback = () => {
            this.editedUser = null;
            this.sourceUser = null;
            this.editorModal.hide();
        };
    }



    loadData(page: number) {
        this.alertService.startLoadingMessage();
        this.loadingIndicator = true;
        this.pager = this.pagerService.getPager(this.rows.length, page);
       
        if (page < 1 || page > this.totalPageTemp) {
            if(page < 1)  this.pager.currentPage=this.pager.currentPage+1;
            if(page > this.totalPageTemp)  this.pager.currentPage=this.pager.currentPage-1;
            this.pager.pages=  this.pagesTemp;
            this.loadingIndicator = false;
            this.alertService.stopLoadingMessage();
          return;
      }
 
    let student= new Student();
		student.currentPage=this.pager.currentPage;
		student.pageSize=this.pager.pageSize;
		student.userName=this.accountService.currentUser.userName;

        this.studentService.getList (student)
           .subscribe(
            response => {
              this.rows = response.data;
              this.rowsCache=response.data;
              this.loadingIndicator = false;
              this.alertService.stopLoadingMessage();
              
         
            // get pager object from service
            this.pager = this.pagerService.getPager(response.totalPage, page);
            if (page < 1 || page > this.pager.totalPages) {
               
              return;
          }
           
            // get current page of items
            this.pagedItems = this.rows.slice(this.pager.startIndex, this.pager.endIndex + 1);
            this.totalPageTemp=this.pager.totalPages;
            this.pagesTemp=this.pager.pages;
         
            }, error => {
                this.alertService.stopLoadingMessage();
              this.loadingIndicator = false;
            }
          );
          
     
    }


    onDataLoadSuccessful(users: User[], roles: Role[]) {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        users.forEach((user, index, users) => {
            (<any>user).index = index + 1;
        });

        this.rowsCache = [...users];
        this.rows = users;

        this.allRoles = roles;
    }


    onDataLoadFailed(error: any) {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.alertService.showStickyMessage('Load Error', `Unable to retrieve users from the server.\r\nErrors: "${Utilities.getHttpResponseMessages(error)}"`,
            MessageSeverity.error, error);
    }


    onSearchChanged(value: string) {
        this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.userName, r.fullName, r.email));
    }

    onEditorModalHidden() {
        this.editingUserName = null;
        this.userEditor.resetForm(true);
    }


    newUser() {
        this.editingUserName = null;
        this.sourceUser = null;
        this.editorModal.show();
  }

  importExcel() {
    this.editorModalExcel.show();
  }


    editUser(row: Student) {
       
        this.editingUserName = { name: row.userName };
      
        this.editedUser = this.userEditor.editUser(row, this.allRoles);
        this.editorModal.show();
    }


    deleteUser(row: Student) {
        this.alertService.showDialog('Bạn có muốn xóa tài khoản \"' + row.studentId + '\"?', DialogType.confirm, () => this.deleteUserHelper(row));
    }


    deleteUserHelper(row: Student) {
        this.alertService.startLoadingMessage('Deleting...');
        this.loadingIndicator = true;

        this.studentService.deleteUser(row)
            .subscribe(results => {
                this.alertService.stopLoadingMessage();
                this.loadingIndicator = false;

                this.loadData(1);
                this.alertService.showMessage('Success', `Tài khoản \"${row.studentId}\" được xóa thành công`, MessageSeverity.success);
            },
            error => {
                this.alertService.stopLoadingMessage();
                this.loadingIndicator = false;

                this.alertService.showStickyMessage('Delete Error', `An error occured whilst deleting the user.\r\nError: "${Utilities.getHttpResponseMessages(error)}"`,
                    MessageSeverity.error, error);
            });
    }

}
