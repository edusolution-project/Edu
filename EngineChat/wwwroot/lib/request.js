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
export { Requester };
