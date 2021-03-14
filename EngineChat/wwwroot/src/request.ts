import { IRequestParams,IResponse } from "./eduso-api";

export class Requester {
    private _headers: Headers | undefined;
    public constructor(headers?: Headers) {
        if (headers) {
            this._headers = new Headers(headers);
        }
    }
    public sendRequest<T extends IResponse>(url: string, urlPath: string, params?: IRequestParams): Promise<T | any>;
    public sendRequest<T>(url: string, urlPath: string, params?: IRequestParams): Promise<T>;
    public sendRequest<T>(url: string, urlPath: string, params?: IRequestParams): Promise<T> {
        if (params !== undefined) {
            const paramKeys = Object.keys(params);
            if (paramKeys.length !== 0) {
                urlPath += '?';
            }
            urlPath += paramKeys.map((key: string) => {
                var value : string | undefined = params[key];
                if(value == undefined){
                    value = "";
                }
                return encodeURIComponent(key) + "="+encodeURIComponent(value.toString());
            }).join('&');
        }
        //logMessage('New request: ' + urlPath);
        // Send user cookies if the URL is on the same origin as the calling script.
        const options: RequestInit = { credentials: 'same-origin' };
        if (this._headers !== undefined) {
            options.headers = this._headers;
        }
        return fetch(url+"/" +urlPath, options)
            .then((response: Response) => response.text())
            .then((responseTest: string) => JSON.parse(responseTest));
    }
    public callRequest<T extends IResponse>(url: string, urlPath: string,method:string,body:BodyInit,useXHR:boolean,headers:Headers,params?: IRequestParams): Promise<T | any>;
    public callRequest<T>(url: string, urlPath: string,method:string,body:BodyInit,useXHR:boolean,headers:Headers,params?: IRequestParams): Promise<T>;
    public callRequest<T>(url: string, urlPath: string,method:string,body:BodyInit,useXHR:boolean,headers:Headers,params?: IRequestParams): Promise<T> {
        if (params !== undefined) {
            const paramKeys = Object.keys(params);
            if (paramKeys.length !== 0) {
                urlPath += '?';
            }
            urlPath += paramKeys.map((key: string) => {
                var value : string | undefined = params[key];
                if(value == undefined){
                    value = "";
                }
                return encodeURIComponent(key) + "="+encodeURIComponent(value.toString());
            }).join('&');
        }
        //logMessage('New request: ' + urlPath);
        // Send user cookies if the URL is on the same origin as the calling script.
        const options: RequestInit = { credentials: 'same-origin', method:method,body:body };
        if (this._headers !== undefined) {
            var oldHeaders : Headers = new Headers(this._headers);
            headers.forEach((v,k)=>{
                oldHeaders.append(k,v);
            })
            options.headers = oldHeaders;
        }
        return useXHR ? 
        this.xhrRequest(url,urlPath,new Headers(options.headers),options,params)
            .then((response: Response) => response)
            :
        fetch(url+"/" +urlPath, options)
            .then((response: Response) => response.text())
            .then((responseTest: string) => JSON.parse(responseTest));
    }

    public requestForm<T extends IResponse>(url: string, urlPath: string,method:string,body:BodyInit,useXHR:boolean, params?: IRequestParams): Promise<T | any>;
    public requestForm<T>(url: string, urlPath: string,method:string,body:BodyInit,useXHR:boolean, params?: IRequestParams): Promise<T>;
    public requestForm<T>(url: string, urlPath: string,method:string,body:BodyInit,useXHR:boolean, params?: IRequestParams): Promise<T> {
        if (params !== undefined) {
            const paramKeys = Object.keys(params);
            if (paramKeys.length !== 0) {
                urlPath += '?';
            }
            urlPath += paramKeys.map((key: string) => {
                var value : string | undefined = params[key];
                if(value == undefined){
                    value = "";
                }
                return encodeURIComponent(key) + "="+encodeURIComponent(value.toString());
            }).join('&');
        }
        //logMessage('New request: ' + urlPath);
        // Send user cookies if the URL is on the same origin as the calling script.
        const options: RequestInit = { credentials: 'same-origin', method:method ||'POST',body:body };
        if (this._headers !== undefined) {
            options.headers = new Headers(this._headers);//application/x-www-form-urlencoded
            options.headers.set('Content-Type', 'application/x-www-form-urlencoded; charset=utf-8');
        }
        return useXHR ? 
        this.xhrRequest(url,urlPath,new Headers(options.headers),options,params)
            .then((response: Response) => response)
            :
        fetch(url+"/" +urlPath, options)
            .then((response: Response) => response.text())
            .then((responseTest: string) => JSON.parse(responseTest));
    }
    public requestJson<T extends IResponse>(url: string, urlPath: string,method:string,body:BodyInit,UseXHR:boolean, params?: IRequestParams): Promise<T | any>;
    public requestJson<T>(url: string, urlPath: string,method:string,body:BodyInit,UseXHR:boolean, params?: IRequestParams): Promise<T>;
    public requestJson<T>(url: string, urlPath: string,method:string,body:BodyInit,useXHR:boolean, params?: IRequestParams): Promise<T> {
        if (params !== undefined) {
            const paramKeys = Object.keys(params);
            if (paramKeys.length !== 0) {
                urlPath += '?';
            }
            urlPath += paramKeys.map((key: string) => {
                var value : string | undefined = params[key];
                if(value == undefined){
                    value = "";
                }
                return encodeURIComponent(key) + "="+encodeURIComponent(value.toString());
            }).join('&');
        }
        //logMessage('New request: ' + urlPath);
        // Send user cookies if the URL is on the same origin as the calling script.
        const options: RequestInit = { credentials: 'same-origin', method:method ||'POST',body:body};
        if (this._headers !== undefined) {
            options.headers = new Headers(this._headers);
            options.headers.set('Content-Type', 'application/json; charset=utf-8');
        }
        return useXHR ? 
        this.xhrRequest(url,urlPath,new Headers(options.headers),options,params)
            .then((response: Response) => response)
        : fetch(url+"/" +urlPath, options)
            .then((response: Response) => response.text())
            .then((responseTest: string) => JSON.parse(responseTest));
    }

    // open(method: string, url: string): void;
    // open(method: string, url: string, async: boolean, username?: string | null, password?: string | null): void;
    private xhrRequest(url:string,urlPath:string,headers:Headers,init?: RequestInit, params?: IRequestParams): Promise<Response>{
        var xhr = new XMLHttpRequest();
        
       return new Promise((resolve,reject)=>{
            if (params !== undefined) {
                const paramKeys = Object.keys(params);
                if (paramKeys.length !== 0) {
                    urlPath += '?';
                }
                urlPath += paramKeys.map((key: string) => {
                    var value : string | undefined = params[key];
                    if(value == undefined){
                        value = "";
                    }
                    return encodeURIComponent(key) + "="+encodeURIComponent(value.toString());
                }).join('&');
            }
            var method : string = init != undefined && init.method != undefined ? init.method : "POST";
            xhr.open(method,url+"/"+urlPath);

            headers.forEach(function(value,key){
                xhr.setRequestHeader(key, value);
            });

            xhr.send(init?.body)
            xhr.onload = () => {resolve(typeof(xhr.response) == "string" ? JSON.parse(xhr.response) : xhr.response);};
            xhr.onerror = () => {reject(xhr.response);};
            xhr.ontimeout = () => {reject(xhr.response);};
       })
    }
}