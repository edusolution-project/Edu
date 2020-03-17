"use trict";
// dec2hex :: Integer -> String
function dec2hex(dec) {
    return ('0' + dec.toString(16)).substr(-2)
}

// generateId :: Integer -> String
function generateId(len) {
    var arr = new Uint8Array((len || 40) / 2)
    window.crypto.getRandomValues(arr)
    return Array.from(arr, dec2hex).join('')
}
var createBlodUrl = function (file) {
    if (file == null || file == void 0 || file == "") return "";
    return URL.createObjectURL(file);
}
var boxRemove = function () {
    var btn = document.createElement('button');
    btn.classList = 'btn btn-remove';
    btn.setAttribute('onclick', 'removeMedia(this)');
    btn.innerText = 'x';
    return btn;
}
var removeMedia = function (self) {
    var parent = self.parentElement;
    var state = parent.dataset.state;
    delete filesList[state];
    if (parent.parentElement != null && parent.parentElement != void 0) {
        var root = parent.parentElement.parentElement;
        if (root != void 0 && root != null) {
            root.querySelector("input[type='file']").value="";
        }
    }
    parent.remove();
}

var boxMedia = function (id) {
    var div = document.createElement("div");
    div.dataset.state = id;
    div.classList = 'box-view-media';
    var x = boxRemove();
    div.appendChild(x);
    return div;
}
var createBoxMedia = function (id, url, type, isHidden) {
    var subStr = type.split('/')[0];
    switch (subStr) {
        case 'video':
            return videoBox(id, url);
        case 'audio':
            return audioBox(id, url);
        case 'image':
            return imgBox(id, url);
        default:
            return defaultBox(id, url);
    }
}
var boxContent = function () {
    var div = document.createElement("div");
    div.classList = 'box-content-view-media';
    return div;
}
var videoBox = function (id, url) {
    var root = boxMedia(id);
    var parent = boxContent();
    var video = document.createElement('video');
    video.src = url;
    video.classList = 'media';
    video.controls = "true";
    parent.appendChild(video);
    root.appendChild(parent);
    return root;
}
var audioBox = function (id, url) {
    var root = boxMedia(id);
    var parent = boxContent();
    var audio = document.createElement('audio');
    audio.src = url;
    audio.classList = 'media';
    audio.controls = "true";
    parent.appendChild(audio);
    root.appendChild(parent);
    return root;
}
var imgBox = function (id, url) {
    var root = boxMedia(id);
    var parent = boxContent();
    var img = document.createElement('img');
    img.src = url;
    img.classList = 'media';
    parent.appendChild(img);
    root.appendChild(parent);
    return root;
}
var defaultBox = function (id, url) {
    var root = boxMedia(id);
    var parent = boxContent();
    var img = document.createElement('iframe');
    img.src = url;
    img.classList = 'media';
    parent.appendChild(img);
    root.appendChild(parent);
    return root;
}
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

var _ajax = new MyAjax();