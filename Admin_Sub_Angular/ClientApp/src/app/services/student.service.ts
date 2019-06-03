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


  newUser(student: Student) {
    return this.restangular.all('Student').all('Create').post(student);
  }

  getList(student: Student) {
    return this.restangular.all('Student').all('getList').post(student);
  }


  deleteUser(student: Student) {
    return this.restangular.all('Student').all('Delete').post(student);
  }

  importExcel(formData: FormData) {

    return this.restangular.all('Student').all('ImportExcel').customPOST(formData, undefined, undefined, { 'Content-Type': undefined });
  }
}
