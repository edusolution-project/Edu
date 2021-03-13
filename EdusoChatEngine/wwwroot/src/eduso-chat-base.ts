import { IConfiguration } from './eduso-api';
export class EdusoChatBase {
    private readonly _config : IConfiguration;
    protected constructor(config : IConfiguration){
        this._config = config;
    }
    public getConfig():IConfiguration{
        return this._config;
    }
}