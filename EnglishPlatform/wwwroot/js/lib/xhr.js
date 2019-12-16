"use trict";
var MyAjax = (function () {
    function MyAjax() {
        this._request = new XMLHttpRequest();
        this._noti = new notification();
    }
    MyAjax.prototype.proccess = function (method, url, data, async) {
        var request = this._request;
        var notification = this._noti;

        return new Promise(function (resolve, reject) {
            request.onreadystatechange = function () {
                if (request.readyState == 4) {
                    // Process the response
                    if (request.status >= 200 && request.status < 300) {
                        // If successful
                        resolve(request.response);
                    } else {
                        //0	UNSENT	Client has been created.open() not called yet.
                        //1	OPENED	open() has been called.
                        //2	HEADERS_RECEIVED	send() has been called, and headers and status are available.
                        //3	LOADING	Downloading; responseText holds partial data.
                        //4	DONE	The operation is complete.
                        var arrStatus = [
                            "UNSENT	Client has been created.open() not called yet.",
                            "OPENED	open() has been called.",
                            "HEADERS_RECEIVED	send() has been called, and headers and status are available.",
                            "LOADING	Downloading; responseText holds partial data.",
                            "DONE	The operation is complete."
                        ];
                        var _msg = request.statusText == "" ? "Có lỗi xảy ra (" + arrStatus[request.status] + ")" : request.statusText;
                        notification.show({
                            type: "error",
                            msg: _msg,
                            timeout: 5000
                        });
                        // If failed
                        reject({
                            status: request.status,
                            statusText: request.statusText
                        });
                    }
                }
            }
            request.open(method || 'POST', url, async || true);
            //request.setRequestHeader('Content-type', 'application/x-www-form-urlencoded;multipart/form-data;application/json');
            // Send the request
            try {
                request.send(data);
            } catch (err) {
                request.setRequestHeader('Content-type', 'application/json; charset=utf-8');
                request.send(data);
            }
        });
    }
    MyAjax.prototype.creatFormData = function (obj, frm) {
        var data = frm == void 0 || frm == null || typeof (frm) != "object"
            ? new FormData()
            : new FormData(frm);
        for (var key in obj) {
            if (data.hasOwnProperty(key)) {
                data[key] = obj[key];
            } else {
                data.append(key, obj[key]);
            }
        }
        return data;
    }
    return MyAjax;
}());