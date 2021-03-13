export enum USER_TYPE{
    ADMINISTRATOR,SUPPORT,SUPPORT_CENTER,TEACHER,STUDENT,ORTHER
} 
export interface IMessage{
    id:string,
    sender:IUserInfo,
    receiver?:IUserInfo, // 1-1
    id_chat?:string, // group
    text?:string,
    attachmements?:string[],
    created:number,
}
export interface IUserInfo {
    id?:string,
    username:string,
    avatar?: string,
    full_name?:string,
    type:USER_TYPE,
    status:boolean,
}
export interface IMember extends IUserInfo{
    last_read?:string // message_id trong chat
}
export interface IChat{
    id:string,
    title:string,
    members:IMember[],
    admin:IUserInfo[],
    attachmements?:string[],
    last_message:IMessage,
    created:number
}
export interface IResponse{
    code:number,
    message?:string,
    data?:any
}
export interface IErrorResponse extends IResponse{
    code:500,
    data:null
}
export interface IRequestParams{
    [param:string]:any
}
export interface IConfiguration{
    debugger:boolean,
    element?:HTMLElement,
    [key:string]:any
}