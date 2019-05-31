import { Injectable } from '@angular/core';
import { Restangular } from 'ngx-restangular';
import { Student } from '../models/student.model';

@Injectable({
  providedIn: 'root'
})
export class StudentService {
  constructor(
    private restangular: Restangular) {

  }


  newUser(teacher: Student) {
    return this.restangular.all('Student').all('Create').post(teacher);
  }

  getList(teacher: Student) {
    return this.restangular.all('Student').all('getList').post(teacher);
  }


  deleteUser(teacher: Student) {
    return this.restangular.all('Student').all('Delete').post(teacher);
  }

  importExcel(formData: FormData) {

    return this.restangular.all('Student').all('ImportExcel').customPOST(formData, undefined, undefined, { 'Content-Type': undefined });
  }
}
