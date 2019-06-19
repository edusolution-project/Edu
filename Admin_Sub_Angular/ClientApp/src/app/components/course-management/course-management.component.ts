
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
import { TeacherService } from 'src/app/services/teacher.service';
import { Teacher } from 'src/app/models/teacher.model';
import { CourseInfoComponent } from './course-info.component';
import { Course } from 'src/app/models/course.model';
import { CourseService } from 'src/app/services/course.service';


@Component({
    selector: 'course-management',
  templateUrl: './course-management.component.html',
  styleUrls: ['./course-management.component.scss']
})
export class CourseManagementComponent implements OnInit, AfterViewInit {
  columns: any[] = [];
  rows: Course[] = [];
  rowsCache: Course[] = [];
  editedCourse: Course;
    
    loadingIndicator: boolean;

  
  

    @ViewChild('editorModal')
    editorModal: ModalDirective;

 

  @ViewChild('courseEditor')
  courseEditor: CourseInfoComponent;



    constructor( private pagerService: PagerService,
        private alertService: AlertService, 
      private courseService: CourseService,
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

      this.courseEditor.changesSavedCallback = () => {
            this.loadData(1);
            this.editorModal.hide();
        };

      this.courseEditor.changesCancelledCallback = () => {
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

      let course = new Course();
      course.currentPage=this.pager.currentPage;
      course.pageSize=this.pager.pageSize;
      course.userName = this.accountService.currentUser.userName;
      this.courseService.getList(course)
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


  


    onDataLoadFailed(error: any) {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.alertService.showStickyMessage('Load Error', `Unable to retrieve users from the server.\r\nErrors: "${Utilities.getHttpResponseMessages(error)}"`,
            MessageSeverity.error, error);
    }


  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.courseID));
    }

    onEditorModalHidden() {
      //  this.userEditor.resetForm(true);
    }


    newUser() {
        this.editorModal.show();
  }



  edit(row: Course) {
      
    this.editedCourse = this.courseEditor.editUser(row);
        this.editorModal.show();
    }


  delete(row: Course) {
        this.alertService.showDialog('Bạn có muốn xóa khóa học\"' + row.description + '\"?', DialogType.confirm, () => this.deleteUserHelper(row));
    }


  deleteUserHelper(row: Course) {
       

        this.alertService.startLoadingMessage('Deleting...');
        this.loadingIndicator = true;

      this.courseService.delete(row)
            .subscribe(results => {
                this.alertService.stopLoadingMessage();
                this.loadingIndicator = false;

                this.loadData(1);
                this.alertService.showMessage('Success', 'Khóa học được xóa thành công', MessageSeverity.success);
            },
            error => {
                this.alertService.stopLoadingMessage();
                this.loadingIndicator = false;

                this.alertService.showStickyMessage('Delete Error', `An error occured whilst deleting the user.\r\nError: "${Utilities.getHttpResponseMessages(error)}"`,
                    MessageSeverity.error, error);
            });
    }



   
}
