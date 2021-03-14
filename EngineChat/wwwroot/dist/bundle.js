(function (global, factory) {
	typeof exports === 'object' && typeof module !== 'undefined' ? factory(exports) :
	typeof define === 'function' && define.amd ? define(['exports'], factory) :
	(factory((global.EdusoChat = {})));
}(this, (function (exports) { 'use strict';

/*! *****************************************************************************
Copyright (c) Microsoft Corporation. All rights reserved.
Licensed under the Apache License, Version 2.0 (the "License"); you may not use
this file except in compliance with the License. You may obtain a copy of the
License at http://www.apache.org/licenses/LICENSE-2.0

THIS CODE IS PROVIDED ON AN *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED
WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
MERCHANTABLITY OR NON-INFRINGEMENT.

See the Apache Version 2.0 License for specific language governing permissions
and limitations under the License.
***************************************************************************** */
/* global Reflect, Promise */

var extendStatics = function(d, b) {
    extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) { if (b.hasOwnProperty(p)) { d[p] = b[p]; } } };
    return extendStatics(d, b);
};

function __extends(d, b) {
    extendStatics(d, b);
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
}

var ChatProvider = /** @class */ (function () {
    function ChatProvider(url, master, request) {
        this._requester = request;
        this._url = url;
        this._master = master;
    }
    ChatProvider.prototype.getList = function (listClass) {
        var _this = this;
        //const requestParams : IRequestParams = {"group" : listClass};
        //var req : BodyInit = JSON.stringify(requestParams);
        var frm = document.createElement("form");
        var req = new FormData(frm); //"group="+listClass;
        req.append("group", listClass);
        var header = new Headers();
        //header.append("group",listClass);
        //"Content-Type", "multipart/form-data"
        //header.set('Content-Type', 'application/x-www-form-urlencoded; charset=utf-8')
        //header.set('Content-Type', 'application/json; charset=utf-8');
        return new Promise(function (resolve, reject) {
            _this._requester.callRequest(_this._url, 'chat/GetContact', 'POST', req, true, header)
                .then(function (res) {
                if (res.code == 200) {
                    resolve(res);
                }
                else {
                    reject(res.message || 'đã có lỗi xảy ra', res.code);
                }
            });
        });
    };
    ChatProvider.prototype.getDetail = function (idChat) {
        var _this = this;
        if (this._master.id == undefined)
            { return null; }
        var requestParams = { master: this._master.id, id: idChat };
        return new Promise(function (resolve, reject) {
            _this._requester.sendRequest(_this._url, '/chat/getdetail', requestParams)
                .then(function (res) {
                if (res.code == 200) {
                    resolve(res);
                }
                else {
                    reject(res);
                }
            });
        });
    };
    ChatProvider.prototype.remove = function (idChat) {
        return true;
    };
    // admin is master , create = datetime.now.stick
    ChatProvider.prototype.create = function (title, members) {
        return true;
    };
    ChatProvider.prototype.updateTitle = function (title) {
        return true;
    };
    ChatProvider.prototype.addMember = function () {
        return true;
    };
    ChatProvider.prototype.kickMember = function () {
        return true;
    };
    return ChatProvider;
}());

var Requester = /** @class */ (function () {
    function Requester(headers) {
        if (headers) {
            this._headers = new Headers(headers);
        }
    }
    Requester.prototype.sendRequest = function (url, urlPath, params) {
        if (params !== undefined) {
            var paramKeys = Object.keys(params);
            if (paramKeys.length !== 0) {
                urlPath += '?';
            }
            urlPath += paramKeys.map(function (key) {
                var value = params[key];
                if (value == undefined) {
                    value = "";
                }
                return encodeURIComponent(key) + "=" + encodeURIComponent(value.toString());
            }).join('&');
        }
        //logMessage('New request: ' + urlPath);
        // Send user cookies if the URL is on the same origin as the calling script.
        var options = { credentials: 'same-origin' };
        if (this._headers !== undefined) {
            options.headers = this._headers;
        }
        return fetch(url + "/" + urlPath, options)
            .then(function (response) { return response.text(); })
            .then(function (responseTest) { return JSON.parse(responseTest); });
    };
    Requester.prototype.callRequest = function (url, urlPath, method, body, useXHR, headers, params) {
        if (params !== undefined) {
            var paramKeys = Object.keys(params);
            if (paramKeys.length !== 0) {
                urlPath += '?';
            }
            urlPath += paramKeys.map(function (key) {
                var value = params[key];
                if (value == undefined) {
                    value = "";
                }
                return encodeURIComponent(key) + "=" + encodeURIComponent(value.toString());
            }).join('&');
        }
        //logMessage('New request: ' + urlPath);
        // Send user cookies if the URL is on the same origin as the calling script.
        var options = { credentials: 'same-origin', method: method, body: body };
        if (this._headers !== undefined) {
            var oldHeaders = new Headers(this._headers);
            headers.forEach(function (v, k) {
                oldHeaders.append(k, v);
            });
            options.headers = oldHeaders;
        }
        return useXHR ?
            this.xhrRequest(url, urlPath, new Headers(options.headers), options, params)
                .then(function (response) { return response; })
            :
                fetch(url + "/" + urlPath, options)
                    .then(function (response) { return response.text(); })
                    .then(function (responseTest) { return JSON.parse(responseTest); });
    };
    Requester.prototype.requestForm = function (url, urlPath, method, body, useXHR, params) {
        if (params !== undefined) {
            var paramKeys = Object.keys(params);
            if (paramKeys.length !== 0) {
                urlPath += '?';
            }
            urlPath += paramKeys.map(function (key) {
                var value = params[key];
                if (value == undefined) {
                    value = "";
                }
                return encodeURIComponent(key) + "=" + encodeURIComponent(value.toString());
            }).join('&');
        }
        //logMessage('New request: ' + urlPath);
        // Send user cookies if the URL is on the same origin as the calling script.
        var options = { credentials: 'same-origin', method: method || 'POST', body: body };
        if (this._headers !== undefined) {
            options.headers = new Headers(this._headers); //application/x-www-form-urlencoded
            options.headers.set('Content-Type', 'application/x-www-form-urlencoded; charset=utf-8');
        }
        return useXHR ?
            this.xhrRequest(url, urlPath, new Headers(options.headers), options, params)
                .then(function (response) { return response; })
            :
                fetch(url + "/" + urlPath, options)
                    .then(function (response) { return response.text(); })
                    .then(function (responseTest) { return JSON.parse(responseTest); });
    };
    Requester.prototype.requestJson = function (url, urlPath, method, body, useXHR, params) {
        if (params !== undefined) {
            var paramKeys = Object.keys(params);
            if (paramKeys.length !== 0) {
                urlPath += '?';
            }
            urlPath += paramKeys.map(function (key) {
                var value = params[key];
                if (value == undefined) {
                    value = "";
                }
                return encodeURIComponent(key) + "=" + encodeURIComponent(value.toString());
            }).join('&');
        }
        //logMessage('New request: ' + urlPath);
        // Send user cookies if the URL is on the same origin as the calling script.
        var options = { credentials: 'same-origin', method: method || 'POST', body: body };
        if (this._headers !== undefined) {
            options.headers = new Headers(this._headers);
            options.headers.set('Content-Type', 'application/json; charset=utf-8');
        }
        return useXHR ?
            this.xhrRequest(url, urlPath, new Headers(options.headers), options, params)
                .then(function (response) { return response; })
            : fetch(url + "/" + urlPath, options)
                .then(function (response) { return response.text(); })
                .then(function (responseTest) { return JSON.parse(responseTest); });
    };
    // open(method: string, url: string): void;
    // open(method: string, url: string, async: boolean, username?: string | null, password?: string | null): void;
    Requester.prototype.xhrRequest = function (url, urlPath, headers, init, params) {
        var xhr = new XMLHttpRequest();
        return new Promise(function (resolve, reject) {
            if (params !== undefined) {
                var paramKeys = Object.keys(params);
                if (paramKeys.length !== 0) {
                    urlPath += '?';
                }
                urlPath += paramKeys.map(function (key) {
                    var value = params[key];
                    if (value == undefined) {
                        value = "";
                    }
                    return encodeURIComponent(key) + "=" + encodeURIComponent(value.toString());
                }).join('&');
            }
            var method = init != undefined && init.method != undefined ? init.method : "POST";
            xhr.open(method, url + "/" + urlPath);
            headers.forEach(function (value, key) {
                xhr.setRequestHeader(key, value);
            });
            xhr.send(init === null || init === void 0 ? void 0 : init.body);
            xhr.onload = function () { resolve(typeof (xhr.response) == "string" ? JSON.parse(xhr.response) : xhr.response); };
            xhr.onerror = function () { reject(xhr.response); };
            xhr.ontimeout = function () { reject(xhr.response); };
        });
    };
    return Requester;
}());

var EdusoChatBase = /** @class */ (function () {
    function EdusoChatBase(config) {
        this._config = config;
        var header = new Headers();
        header.append("security", this._config.current.id || "");
        this._requester = new Requester(header);
        this._chatProvider = new ChatProvider(this._config.url, this._config.current, this._requester);
    }
    EdusoChatBase.prototype.getConfig = function () {
        return this._config;
    };
    EdusoChatBase.prototype.getRequest = function () {
        return this._requester;
    };
    EdusoChatBase.prototype.getChatProvider = function () {
        return this._chatProvider;
    };
    return EdusoChatBase;
}());

var ChatEngine = /** @class */ (function (_super) {
    __extends(ChatEngine, _super);
    function ChatEngine(master, url, root) {
        var _this = this;
        var configuration = {
            current: master,
            debugger: false,
            url: url,
            element: root
        };
        _this = _super.call(this, configuration) || this;
        return _this;
    }
    return ChatEngine;
}(EdusoChatBase));

exports.ChatEngine = ChatEngine;

Object.defineProperty(exports, '__esModule', { value: true });

})));
