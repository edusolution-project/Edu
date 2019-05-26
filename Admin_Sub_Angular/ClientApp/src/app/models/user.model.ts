// =============================
// Email: info@ebenmonney.com
// www.ebenmonney.com/templates
// =============================

export class User {
  // Note: Using only optional constructor properties without backing store disables typescript's type checking for the type
  constructor(id?: string, userName?: string, fullName?: string, email?: string, roleID?: string, phone?: string, userNameManager?:string) {

    this.id = id;
    this.userName = userName;
    this.fullName = fullName;
    this.email = email;
    this.roleID=roleID;
    this.userNameManager=userNameManager
      this.phone = phone;
    
  }


  get friendlyName(): string {
    let name = this.fullName || this.userName;

    if (this.jobTitle) {
      name = this.jobTitle + ' ' + name;
    }

    return name;
  }


  public id: string;
  public userName: string;
  public fullName: string;
  public email: string;
  public jobTitle: string;
  public phone: string;
  public isEnabled: boolean;
  public isLockedOut: boolean;
  public roles: string[];
  public roleID: string;
  public activity: boolean;
  public userNameManager: string;
}
