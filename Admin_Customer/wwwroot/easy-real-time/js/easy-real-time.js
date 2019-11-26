"use strict";

var easyRealTime = (function () {
    var listGroup = [], // list Class
        listFriend = [], // theo danh sách kết bạn , 
        teacherList = []; // theo danh sách class
    var request = function (method, url, dataForm, async) {
        return new Promise(function (resolve, reject) {
            var req = new XMLHttpRequest() || window.XMLHttpRequest || ActiveXObject("Microsoft.XMLHTTP");
            req.open(method, url, async);
            req.addEventListener('load', function () {
                if (req.status == 200) {
                    resolve(JSON.parse(req.response));
                } else {
                    if (req.status == 404) {
                        throw new Error(xmlPath, "404");
                    }
                }
            });
            req.addEventListener('error', function (event) {
                reject(event);
            });
            req.send(dataForm);
        });
    }
    var defaultOptions = {
        //list group 
        id: {
            group: "group_id",
            friend: "friend_id",
            content:"content_id"
        },
        url: {
            newfeed: {
                get: "/newfeed/get?",
                post: "/newfeed/create",
                remove: "/newfeed/remove",
                view:"/newfeed/updateview"
            },
            chat: {

                get: "/comment/get?",
                post: "/comment/create",
                remove:"/comment/remove"
            },
            comment: {
                get: "/chat/get?",
                post: "/chat/create",
                remove:"/chat/remove"
            }
        }
    }

    function easyRealTime(options) {
        this.config = defaultOptions;
        extendOptions(options);
        this.createChatBox = createChatBox;
        this.createNewFeedBox = createNewFeedBox;
        loadDataInfo().then(function (data) {
            console.log(data);
            if (data.code != 404 && data.code != 405 && data.code != 500) {
                listGroup = data.data.group;
                teacherList = data.data.teacher;
                listFriend = data.data.friend;
            } else {
                //thông báo
            }
        });
    }
    var extendOptions = function (options) {
        if (options == null || typeof (options) == "undefined") return defaultOptions;
        for (var key in options)
            if (options.hasOwnProperty(key))
                defaultOptions[key] = options[key];
        return defaultOptions;
    }

    

    var loadDataInfo = function () {
       return request("GET", "/Contact/get", {}, true)
    }

    var createUrlGet = function (pageIndex,pageSize,id,state,receivers) {
        //newfeed id state receivers pageIndex pageSize
        //comment string parentID,bool IsReply, long pageIndex, long pageSize
        //chat string receiver,long pageIndex, long pageSize
    }
    var createUrlPost = function () {
        //newfeed string title,string content,int state, HashSet<string> receivers
        //comment string parentID, string content
        //chat string content, int state, string receiver
    }
    var createUrlRemove = function (id) {
        //newfeed id
        //comment id
        //chat id
    }
    var createNewFeedBox = function () {

    }
    var createCommentBox = function () {

    }
    var createChatBox = function () {
        
        request("GET",)
    }
    return easyRealTime;
}());