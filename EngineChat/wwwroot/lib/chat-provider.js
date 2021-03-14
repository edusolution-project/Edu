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
            return null;
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
export { ChatProvider };
