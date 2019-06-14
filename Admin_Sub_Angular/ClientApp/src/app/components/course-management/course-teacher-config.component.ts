
import { Component, OnInit, AfterViewInit, TemplateRef, ViewChild, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { AlertService, DialogType, MessageSeverity } from '../../services/alert.service';
import { AccountService } from '../../services/account.service';
import { Utilities } from '../../services/utilities';
import { CourseInfoComponent } from './course-info.component';
import { Course } from 'src/app/models/course.model';
import { CourseService } from 'src/app/services/course.service';
import { ActivatedRoute } from '@angular/router';


@Component({
  selector: 'course-teacher-config',
  templateUrl: './course-teacher-config.component.html',
  styleUrls: ['./course-teacher-config.component.scss']
})
export class CourseTeacherConfigComponent implements OnInit {
  columns: any[] = [];
  rows: Course[] = [];
  rowsCache: Course[] = [];
  editedCourse: Course;

  loadingIndicator: boolean;




  @ViewChild('editorModal')
  editorModal: ModalDirective;



  @ViewChild('courseEditor')
  courseEditor: CourseInfoComponent;



  constructor(
    private alertService: AlertService,
    private courseService: CourseService,
    private accountService: AccountService,
    private route: ActivatedRoute) {
  }

  courseID;
  ngOnInit() {
    this.courseID = this.route.snapshot.paramMap.get("id")
    this.loadData(1);
  }

  loadData(page: number) {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    let course = new Course();
    course.teacherID = this.accountService.currentUser.userName;
    this.courseService.getList(course)
      .subscribe(
        response => {
          this.rows = response.data;
          this.rowsCache = response.data;
          this.loadingIndicator = false;
          this.alertService.stopLoadingMessage();

        }, error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;
        }
      );

  }
  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.courseID));
  }


}
