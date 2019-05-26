// =============================
// Email: info@ebenmonney.com
// www.ebenmonney.com/templates
// =============================

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
import { UserInfoComponent } from './user-info.component';
import { Restangular } from 'ngx-restangular';
import { PagerService } from 'src/app/services/pager.service';


@Component({
    selector: 'users-management',
    templateUrl: './users-management.component.html',
    styleUrls: ['./users-management.component.scss']
})
export class UsersManagementComponent implements OnInit, AfterViewInit {
    columns: any[] = [];
    rows: User[] = [];
    rowsCache: User[] = [];
    editedUser: UserEdit;
    sourceUser: UserEdit;
    editingUserName: { name: string };
    loadingIndicator: boolean;

    allRoles: Role[] = [];
    services:any;

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

    @ViewChild('userEditor')
    userEditor: UserInfoComponent;

    constructor( private pagerService: PagerService,private restangular: Restangular,private alertService: AlertService, private translationService: AppTranslationService, private accountService: AccountService) {
    }
    private allItems: any[];

    // pager object
    pager: any = {};

    // paged items
    pagedItems: any[];
    totalPageTemp:number=100;

    ngOnInit() {
        this.services = this.restangular.all('Account');


        if (this.canManageUsers) {
            this.columns.push({ name: '', width: 160, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false });
        }
      

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


    addNewUserToList() {
        if (this.sourceUser) {
            Object.assign(this.sourceUser, this.editedUser);

            let sourceIndex = this.rowsCache.indexOf(this.sourceUser, 0);
            if (sourceIndex > -1) {
                Utilities.moveArrayItem(this.rowsCache, sourceIndex, 0);
            }

            sourceIndex = this.rows.indexOf(this.sourceUser, 0);
            if (sourceIndex > -1) {
                Utilities.moveArrayItem(this.rows, sourceIndex, 0);
            }

            this.editedUser = null;
            this.sourceUser = null;
        } else {
            const user = new User();
            Object.assign(user, this.editedUser);
            this.editedUser = null;

            let maxIndex = 0;
            for (const u of this.rowsCache) {
                if ((<any>u).index > maxIndex) {
                    maxIndex = (<any>u).index;
                }
            }

            (<any>user).index = maxIndex + 1;

            this.rowsCache.splice(0, 0, user);
            this.rows.splice(0, 0, user);
            this.rows = [...this.rows];
        }
    }


    loadData(page: number) {
        this.alertService.startLoadingMessage();
        this.loadingIndicator = true;

        
        this.pager = this.pagerService.getPager(this.rows.length, page);
       
        if (page < 1 || page > this.totalPageTemp) {
            this.loadingIndicator = false;
          this.alertService.stopLoadingMessage();
          return;
      }
        this.services.all("getSubUser").post(
            {'currentPage':page,
        'pageSize':this.pager.pageSize,
        'userName':this.accountService.currentUser.userName}
        ).subscribe(
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
            }, error => {
                this.alertService.stopLoadingMessage();
              this.loadingIndicator = false;
            }
          );
          
        // if (this.canViewRoles) {
        //     this.accountService.getUsersAndRoles().subscribe(results => this.onDataLoadSuccessful(results[0], results[1]), error => this.onDataLoadFailed(error));
        // } else {
        //     this.accountService.getUsers().subscribe(users => this.onDataLoadSuccessful(users, this.accountService.currentUser.roles.map(x => new Role(x))), error => this.onDataLoadFailed(error));
        // }
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
        this.editedUser = this.userEditor.newUser(this.allRoles);
        this.editorModal.show();
    }


    editUser(row: UserEdit) {
     
        if(row.roleID=="SUPERADMIN")
        {
        this.alertService.showMessage("","Không thể sửa tài khoản quản tri", MessageSeverity.warn);
        return;
        }
        this.editingUserName = { name: row.userName };
        this.sourceUser = row;
        this.editedUser = this.userEditor.editUser(row, this.allRoles);
        this.editorModal.show();
    }


    deleteUser(row: UserEdit) {
        this.alertService.showDialog('Bạn có muốn dừng hoạt động tài khoản \"' + row.userName + '\"?', DialogType.confirm, () => this.deleteUserHelper(row));
    }


    deleteUserHelper(row: UserEdit) {
      
        if(row.roleID=="SUPERADMIN")
        {
            this.alertService.showStickyMessage('Delete Error', 'Không thể dừng hoạt động tài khoản quản trị',
                    MessageSeverity.error, "");
                    return;
        }

        this.alertService.startLoadingMessage('Deleting...');
        this.loadingIndicator = true;

        this.accountService.deleteUser(row)
            .subscribe(results => {
                this.alertService.stopLoadingMessage();
                this.loadingIndicator = false;

                this.loadData(1);
                this.alertService.showMessage('Success', `Tài khoản \"${row.userName}\" được cập nhật thành công`, MessageSeverity.success);
            },
            error => {
                this.alertService.stopLoadingMessage();
                this.loadingIndicator = false;

                this.alertService.showStickyMessage('Delete Error', `An error occured whilst deleting the user.\r\nError: "${Utilities.getHttpResponseMessages(error)}"`,
                    MessageSeverity.error, error);
            });
    }



    get canAssignRoles() {
        return this.accountService.userHasPermission(Permission.assignRolesPermission);
    }

    get canViewRoles() {
        return this.accountService.userHasPermission(Permission.viewRolesPermission);
    }

    get canManageUsers() {
        return this.accountService.userHasPermission(Permission.manageUsersPermission);
    }
}
