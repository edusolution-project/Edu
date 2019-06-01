
import { Injectable } from '@angular/core';
import { Restangular } from 'ngx-restangular';
import { Course } from '../models/course.model';



@Injectable()
export class CourseService {



  constructor(
    private restangular: Restangular) {

  }


  newUser(course: Course) {
    return this.restangular.all('Course').all('Create').post(course);
  }

  getList(course: Course) {
    return this.restangular.all('Course').all('getList').post(course);
  }


  delete(course: Course) {
    return this.restangular.all('Course').all('Delete').post(course);
  }


  getListModSubject() {
    return this.restangular.all('Course').all('getListModSubject').post();
  }

  getListModGradle() {
    return this.restangular.all('Course').all('getListModGradle').post();
  }

  getListModProgram() {
    return this.restangular.all('Course').all('getListModProgram').post();
  }

  getListCourse(subjectID:string,gradleID:string,programID:string) {
    return this.restangular.all('Course').all('getListModCourse').post({
      'subjectID': subjectID,
      'gradleID': gradleID,
      'programID': programID
    });
  }

  getListTeacher(userName: string) {
    return this.restangular.all('Teacher').all('getList').post({ 'userName': userName });
  }

  getListStudent(userName: string) {
    return this.restangular.all('Student').all('getList').post({ 'userName': userName });
  }
}
  











 
