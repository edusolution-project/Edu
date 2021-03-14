import { ChatProvider } from './chat-provider';
import { IConfiguration } from './eduso-api';
import { Requester } from './request';
export class EdusoChatBase {
    private readonly _config : IConfiguration;
    private _requester : Requester | undefined; 
    private _chatProvider: ChatProvider | undefined;
    protected constructor(config : IConfiguration){
        this._config = config;
        var header : HeadersInit = new Headers();
        header.append("security",this._config.current.id || "");
        this._requester = new Requester(header);
        this._chatProvider = new ChatProvider(this._config.url,this._config.current,this._requester);
    }
    public getConfig():IConfiguration{
        return this._config;
    }
    public getRequest(){
        return this._requester;
    }
    public getChatProvider(){
        return this._chatProvider;
    }
}