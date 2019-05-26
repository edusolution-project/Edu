
import { Injectable } from '@angular/core';
import { Restangular } from 'ngx-restangular';
import { Teacher } from '../models/teacher.model';



@Injectable()
export class TeacherService {
 
  

  constructor(
    private restangular: Restangular) {

  }
  

newUser(teacher: Teacher) {
   return this.restangular.all('Teacher').all('Create').post(teacher);
  }

getList(teacher: Teacher)
{
  return this.restangular.all('Teacher').all('getList').post(teacher);
}


  deleteUser(teacher: Teacher) {
    return this.restangular.all('Teacher').all('Delete').post(teacher);
   }

  }
  











 
