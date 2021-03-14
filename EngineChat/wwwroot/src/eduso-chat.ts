import { IConfiguration, IUserInfo } from "./eduso-api";
import { EdusoChatBase } from "./eduso-chat-base";

export class ChatEngine extends EdusoChatBase{
    public constructor(master:IUserInfo,url:string, root:HTMLElement){
        var configuration : IConfiguration = {
            current :master,
            debugger:false,
            url :url,
            element:root
        };
        super(configuration);
    }
}