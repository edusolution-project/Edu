import { IRequestParams,IResponse } from "./eduso-api";

export class Requester {
    private _headers: HeadersInit | undefined;
    public constructor(headers?: HeadersInit) {
        if (headers) {
            this._headers = headers;
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
                return encodeURIComponent(key) + "="+encodeURIComponent(params[key].toString());
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
    public callRequest<T extends IResponse>(url: string, urlPath: string,method:string,body:BodyInit, params?: IRequestParams): Promise<T | any>;
    public callRequest<T>(url: string, urlPath: string,method:string,body:BodyInit, params?: IRequestParams): Promise<T>;
    public callRequest<T>(url: string, urlPath: string,method:string,body:BodyInit, params?: IRequestParams): Promise<T> {
        if (params !== undefined) {
            const paramKeys = Object.keys(params);
            if (paramKeys.length !== 0) {
                urlPath += '?';
            }
            urlPath += paramKeys.map((key: string) => {
                return encodeURIComponent(key) + "="+encodeURIComponent(params[key].toString());
            }).join('&');
        }
        //logMessage('New request: ' + urlPath);
        // Send user cookies if the URL is on the same origin as the calling script.
        const options: RequestInit = { credentials: 'same-origin', method:method ||'POST',body:body };
        if (this._headers !== undefined) {
            options.headers = this._headers;
        }
        return fetch(url+"/" +urlPath, options)
            .then((response: Response) => response.text())
            .then((responseTest: string) => JSON.parse(responseTest));
    }
        
}