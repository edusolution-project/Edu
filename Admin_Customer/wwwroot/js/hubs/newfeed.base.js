"use strict";
const arrayNumberSymbol = ["một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín"];
const arrayNumber = [1, 2, 3, 4, 5, 6, 7, 8, 9];
const arrayTime = ["giây trước", "phút trước", "giờ trước", "ngày trước"];
function formatDate(date) {

    var hours = date.getHours();
    var minutes = date.getMinutes();
    var ampm = hours >= 12 ? 'pm' : 'am';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes + ' ' + ampm;
    return date.getMonth() + 1 + "/" + date.getDate() + "/" + date.getFullYear() + "  " + strTime;
}

var Comment = (function (hub) {
    var config = {
        url: {
            post: "",
            load: "/student/discuss/GetListComment",
            feel: ""
        },
        parent_id:""
    }
    function Comment(options) {
        this._ajax = new MyAjax();
        this._config = groupConfig();
        this._render = new Render();
    }
    var groupConfig = function (options) {
        if (options == null || typeof (options) == "undefined") return config;
        for (var key in options)
            if (options.hasOwnProperty(key)) config[key] = options[key];
        return config;
    }
    Comment.prototype.loadConfig = function (options) {
        this._config = groupConfig(options);
    }
    Comment.prototype.load = function (classID,take, skip) {
        const _self = this;
        const _obj = { newFeedID: classID == void 0 || null || "" ? _self._config.parent_id : classID, skip: skip || 0, take: take || 5 };
        var dataFrm = _self._ajax.creatFormData(_obj);
        this._ajax.proccess("POST", _self._config.url.load, dataFrm).then(function (data) {
            var jsonData = JSON.parse(data);
            const container = document.getElementById(classID == void 0 || null || "" ? _self._config.parent_id : classID);
            for (var i = 0; jsonData != null && i < jsonData.length; i++) {
                var item = jsonData[i];
                if (item.Content == "" || item.Content == null || item.Content == void 0) continue;
                _self._render.Comment(item, container);
            }
        })
            .catch(function (err) { console.error(err); });
    }
    return Comment;
}(connection));
var Newfeed = (function (hub) {
    var config = {
        url: {
            post: "",
            load: "",
            feel: ""
        },
        group_id: "",
        id:"list-new-feed"
    }
    var comment;
    function Newfeed(options) {
        this._config = groupConfig(options);
        comment = this._comment = new Comment();
        this._ajax = new MyAjax();
        this._render = new Render();
    }
    var groupConfig = function (options) {
        if (options == null || typeof (options) == "undefined") return config;
        for (var key in options)
            if (options.hasOwnProperty(key)) config[key] = options[key];
        return config;
    }
    //Newfeed.prototype.onReady = function (t) {
    //    document.addEventListener("DOMContentLoaded", t,true);
    //}
    var renderHTML = function (data) {

    };
    var addEvent = function () {
        document.addEventListener('click', function (event) {
            var _self = event.target;
            if (event.target.classList.contains("item-new-feed__title")) {
                comment.load(event.target.parentElement.id);
            }
            if (event.target.classList.contains('item-new-feed__like')) {
                console.log('like');
            }
            if (event.target.classList.contains('item-new-feed__like.active')) {
                console.log('unlike');
            }
            if (event.target.classList.contains('item-new-feed__reply')) {
                var reply_popup = document.getElementById('new-feed__reply--popup');
                if (reply_popup != null) {
                    reply_popup.querySelector('input[name="ParentID"]').value = _self.dataset.id;
                }
            }
            console.log(event.target, event.target.classList);
        });
    }
    Newfeed.prototype.load = function (skip, take) {
        const _self = this;
        var dataFrm = _self._ajax.creatFormData({ classID: _self._config.group_id, skip: skip, take: take });
        this._ajax.proccess("POST", _self._config.url.load, dataFrm).then(function (data) {
            var jsonData = JSON.parse(data);
            const container = document.getElementById(_self._config.id);
            for (var i = -1; jsonData != null &&  i < jsonData.length; i++) {
                if (i >= 0) {
                    var item = jsonData[i];
                    if (item.Content == "" || item.Content == null || item.Content == void 0) continue;
                    _self._render.NewFeed(item, container, _self._comment);
                } else {
                    _self._render.NewFeed(null, container, _self._comment);
                }
            }
            addEvent();
        })
        .catch(function (err) { console.error(err); });
    }
    Newfeed.prototype.post = function (dataForm) {
        this._ajax.proccess("POST", this._config.url.post, dataForm).then(function (data) {
            
        })
        .catch(function (err) { console.error(err); });
    }
    return Newfeed;
}(connection))
