import { Requester } from './request';
import { IResponse, IMessage, IErrorResponse, IUserInfo, IRequestParams, IChat, IMember } from './eduso-api';
interface ChatEntity {
    id:string,
    title:string,
    last_message:IMessage,
    created:number
}
interface ChatResponse extends IResponse{
    code: 200,
    data?: ChatEntity[]
}
interface ChatDetailRespone extends IResponse{
    code: 200,
    data?:IChat
}

export class ChatProvider{
    private readonly _master : IUserInfo;
    private _url: string;
    private readonly _requester: Requester;
    public constructor(url:string,master:IUserInfo,request:Requester){
        this._requester = request;
        this._url = url;
        this._master = master;
    }
    public getList(listClass:string):Promise<ChatResponse>| null{
        //const requestParams : IRequestParams = {"group" : listClass};
        //var req : BodyInit = JSON.stringify(requestParams);
        var frm : HTMLFormElement = document.createElement("form");
        var req : BodyInit =new FormData(frm);  //"group="+listClass;
        req.append("group" , listClass);
        var header = new Headers();
        //header.append("group",listClass);
        //"Content-Type", "multipart/form-data"
        //header.set('Content-Type', 'application/x-www-form-urlencoded; charset=utf-8')
        //header.set('Content-Type', 'application/json; charset=utf-8');
        return new Promise((resolve: (result: ChatResponse) => void, reject: (reason: string, code:number) => void) => {
            this._requester.callRequest(this._url,'chat/GetContact','POST', req,true,header)
            .then((res:ChatResponse | IErrorResponse)=>{
                if(res.code == 200){
                    resolve(res)
                }
                else{
                    reject(res.message || 'đã có lỗi xảy ra', res.code);
                }
            })
        });
    }
    public getDetail(idChat:string):Promise<ChatDetailRespone> | null{
        if(this._master.id == undefined) return null;
        const requestParams : IRequestParams = {master:this._master.id,id:idChat}
        return new Promise((resolve:(result:ChatDetailRespone)=>void,reject:(result:IErrorResponse)=>void)=>{
            this._requester.sendRequest<ChatDetailRespone>(this._url, '/chat/getdetail', requestParams)
            .then((res:ChatDetailRespone | IErrorResponse)=>{
                if(res.code == 200){
                    resolve(res)
                }
                else{
                    reject(res);
                }
            })
        })
    }
    public remove(idChat:string):boolean{
        return true;
    }
    // admin is master , create = datetime.now.stick
    public create(title:string,members:IMember[]):boolean{
        return true;
    }
    public updateTitle(title:string) : boolean{
        return true;
    }
    public addMember():boolean{
        return true;
    }
    public kickMember():boolean{
        return true;
    }
} 