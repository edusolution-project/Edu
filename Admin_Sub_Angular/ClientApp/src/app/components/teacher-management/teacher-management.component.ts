
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
import { TeacherInfoComponent } from './teacher-info.component';
import { TeacherService } from 'src/app/services/teacher.service';
import { Teacher } from 'src/app/models/teacher.model';
import { TeacherImportComponent } from './teacher-import.component';


@Component({
    selector: 'teacher-management',
    templateUrl: './teacher-management.component.html',
    styleUrls: ['./teacher-management.component.scss']
})
export class TeachersManagementComponent implements OnInit, AfterViewInit {
    columns: any[] = [];
    rows: User[] = [];
    rowsCache: User[] = [];
    editedUser: Teacher;
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
  userEditor: TeacherInfoComponent;

  @ViewChild('teacherImport')
  teacherImport: TeacherImportComponent;

    constructor( private pagerService: PagerService,
        private alertService: AlertService, 
        private teacherService: TeacherService,
        private accountService: AccountService) {
    }
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

      this.teacherImport.changesSavedCallback = () => {
        this.loadData(1);
        this.editorModalExcel.hide();
      };
      this.teacherImport.changesCancelledCallback = () => {
        this.editorModalExcel.hide();
      }

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
 
    let teacher= new Teacher();
teacher.currentPage=this.pager.currentPage;
teacher.pageSize=this.pager.pageSize;
teacher.userName=this.accountService.currentUser.userName;

        this.teacherService.getList (teacher)
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


    editUser(row: Teacher) {
       
        this.editingUserName = { name: row.userName };
      
        this.editedUser = this.userEditor.editUser(row, this.allRoles);
        this.editorModal.show();
    }


    deleteUser(row: Teacher) {
        this.alertService.showDialog('Bạn có muốn xóa tài khoản \"' + row.teacherId + '\"?', DialogType.confirm, () => this.deleteUserHelper(row));
    }


    deleteUserHelper(row: Teacher) {
       

        this.alertService.startLoadingMessage('Deleting...');
        this.loadingIndicator = true;

        this.teacherService.deleteUser(row)
            .subscribe(results => {
                this.alertService.stopLoadingMessage();
                this.loadingIndicator = false;

                this.loadData(1);
                this.alertService.showMessage('Success', `Tài khoản \"${row.teacherId}\" được xóa thành công`, MessageSeverity.success);
            },
            error => {
                this.alertService.stopLoadingMessage();
                this.loadingIndicator = false;

                this.alertService.showStickyMessage('Delete Error', `An error occured whilst deleting the user.\r\nError: "${Utilities.getHttpResponseMessages(error)}"`,
                    MessageSeverity.error, error);
            });
    }



   
}
