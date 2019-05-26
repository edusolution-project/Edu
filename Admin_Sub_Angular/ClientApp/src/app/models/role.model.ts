// =============================
// Email: info@ebenmonney.com
// www.ebenmonney.com/templates
// =============================

import { Permission } from './permission.model';


export class Role {

    constructor(roleID?:string,name?: string, description?: string, permissions?: Permission[], ) {

        this.name = name;
        this.description = description;
        this.permissions = permissions;
        this.roleID=roleID;
    }
   

    public id: string;
    public roleID: string;
    public name: string;
    public description: string;
    public usersCount: number;
    public permissions: Permission[];
}
