"use strict";
var easyRealTime = (function () {
    var listGroup = [], // list Class
        listFriend = [], // theo danh sách kết bạn , 
        teacherList = []; // theo danh sách class
    var request = new MyAjax();
    var defaultOptions = {
        //list group 
        id: {
            chatBox: "chat-box",
            newFeed: "easy-new-feed-real-time"
        },
        url: {
            newfeed: {
                get: "/newfeed/get?",
                post: "/newfeed/create",
                remove: "/newfeed/remove",
                view: "/newfeed/updateview"
            },
            comment: {
                get: "/comment/get?",
                post: "/comment/create",
                remove: "/comment/remove"
            },
            chat: {
                get: "/chat/get?",
                post: "/chat/create",
                remove: "/chat/remove"
            }
        }
    }
    var chatBox, newFeed;
    function easyRealTime(options) {
        this.config = defaultOptions;
        // đồng bô option
        extendOptions(options);
        chatBox = document.getElementById(defaultOptions.id.chatBox);
        newFeed = document.getElementById(defaultOptions.id.newFeed);
        loadDataInfo().then(function (res) {
            var data = {};
            if (res != null && res != "") {
                data = JSON.parse(res);
            }
            if (data.code != 404 && data.code != 405 && data.code != 500) {

                createChatBox();
                createNewFeedBox();
                listGroup = data.data.group;
                teacherList = data.data.teacher;
                listFriend = data.data.friend;
                if (newFeed != null) {
                    // tạo list ul li cho group
                    renderGroupForNewFeed(listGroup);
                }
                if (chatBox != null) {
                    // tạo list ul li cho chat
                    renderGroupForChat(listGroup);
                    // tạo list ul li cho chat
                    renderTeacherForChat(teacherList);
                    // tạo list ul li cho chat
                    renderFriendForChat(listFriend);
                }

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

    var renderGroupForNewFeed = function (data) {

    }
    var renderGroupForChat = function (data) {
        var abc;
        if (connection.connectionState == 1) {
            if (abc != null && abc != void 0) {
                clearTimeout(abc);
            }
            var _listGroup = document.getElementById('list-group');
            _listGroup.style.display = 'block';
            for (var i = 0; data != null && i < data.length; i++) {
                var item = data[i];
                var li = document.createElement("li");
                li.innerHTML =
                    ` <a href="#contact-${i}" data-state="1" data-group="chat-${item.name}">
                    <div class="contact">
                        <div class="pull-left avatar-group">${item.name.substring(0, 1).toUpperCase()}</div>
                        <div class="msg-preview">
                            <h6>${item.displayName}</h6>
                            <small class="text-muted">${item.members.length} members</small>
                            <div id="chat-alert-${item.name}"></div>
                        </div>
                    </div>
                </a>`;
                _listGroup.appendChild(li);
                li.removeEventListener('click', function () {
                });
                li.addEventListener('click', function () {
                    loadMessage(this);
                });
                connection.invoke("GoToClass", `chat-${item.name}`);
                connection.invoke("GoToClass", `newfeed-${item.name}`);
            }

        } else {
            abc = setTimeout(function () {
                renderGroupForChat(data);
            }, 3000);
        }
    }
    var loadMessage = function (self) {
        if (self.classList == "active") {
            var contactViews = document.getElementById("contact-views");
            contactViews.scrollIntoView({ block: "end" });
            return;
        }
        var parent = self.parentElement;
        var active = parent.querySelector('li.active');

        if (active != null) active.classList.remove('active');

        self.classList.add('active');

        var data = self.querySelector('a');
        if (data != null) {
            var state = data.dataset.state;
            var group = data.dataset.group;

            var ulcontentView = chatBox.querySelector("#ul-contact-views");
            if (ulcontentView != null) {
                ulcontentView.innerHTML = "";
                renderContactView(state, group, 0, 10, ulcontentView);
            }

            var formChat = chatBox.querySelector('#form-chat');
            formChat.innerHTML = `<div class="send-message">
                                    <div class="input-group">
                                        <input type="text" id="content-message" class="form-control" placeholder="Type your message">
                                        <span class="input-group-btn">
                                            <button id="send-chat" class="btn btn-default" type="button">Send</button>
                                        </span>
                                    </div>
                                </div>`;

            formChat.querySelector('button#send-chat').onclick = function () {
                var content = formChat.querySelector('input#content-message').value;
                var formData = new FormData();
                formData.append("receiver", group);
                formData.append("state", state);
                formData.append("content", content);
                sendMessage(formData, group);
                formChat.querySelector('input#content-message').value = "";
            }

        }
    }
    var renderContactView = function (state, group, pageIndex, pageSize, el) {
        var url = `${defaultOptions.url.chat.get}state=${state}&receiver=${group}&pageIndex=${pageIndex}&pageSize=${pageSize}`;
        request.proccess("GET", url, {}).then(function (data) {
            var dataJson = JSON.parse(data);
            if (dataJson.code == 200) {
                for (var i = 0; i < dataJson.data.length; i++) {
                    var item = dataJson.data[i];
                    var li = document.createElement("li");
                    var pullClass = "pull-";
                    var nameUser = "";
                    if (g_CurrentUser != void 0 && item.sender == g_CurrentUser.email) {

                        li.classList = "right";
                        pullClass = "pull-right";
                        nameUser = g_CurrentUser.name;
                    }
                    else {
                        li.classList = "left";
                        pullClass = "pull-left";
                        nameUser = item.sender;
                    }
                    li.innerHTML = `<div class="${pullClass} avatar-group">${nameUser.trim().substring(0, 1).toUpperCase()}</div>
                            <div class="chat-item">
                                <div class="chat-item-header">
                                    <h5>${nameUser}</h5>
                                    <small class="text-muted">${changeTimeToString(item.created)}</small>
                                </div>
                            <p>${item.content}</p>
                        </div>`;
                    el.appendChild(li);

                }
                var contactViews = document.getElementById("contact-views");
                contactViews.scrollIntoView({ block: "end" });
            }
        });
    }
    var changeTimeToString = function (time) {
        var date = new Date(`${time}`);
        var now = new Date().getTime();
        var sub = now - date.getTime();
        // 1s = 1000 => 60 => 60000 => 60phuts = 60*
        var divide = parseInt(sub / 60000);
        if (divide >= 0 && divide < 1) return "vài giây trước";
        if (divide >= 1 && divide < 60) return `${divide} phút trước`;
        if (divide >= 60 && divide < 60 * 24) return `${parseInt(divide / 60)} giờ trước`;
        if (divide >= 60 * 24 && divide < 60 * 24 * 30) return `${parseInt(divide / 60 * 24)} ngày trước`;
        return `${date.getDate()}-${date.getMonth() + 1}-${date.getFullYear()}`;
    }
    var sendMessage = function (data, group) {
        request.proccess("POST", defaultOptions.url.chat.post, data)
            .then(function (data) {
                var dataJson = JSON.parse(data);
                if (dataJson.code == 200 || dataJson.code == 201) {
                    var obj = {};
                    obj["content"] = dataJson.data.content;
                    obj["medias"] = dataJson.data.medias;
                    obj["sender"] = dataJson.data.sender;
                    obj["receiver"] = group;
                    connection.invoke("SendToGroup", obj, group);



                } else {
                    console.log("lỗi")
                }
            })
            .catch(function (err) { console.log(err); });
    }
    var renderTeacherForChat = function (data) {

    }
    var renderFriendForChat = function (data) {

    }



    var loadDataInfo = function () {
        return request.proccess("GET", "/Contact/get", {}, true)
    }

    var createUrlGet = function (pageIndex, pageSize, id, state, receivers) {
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
        if (chatBox != null) {
            chatBox.innerHTML = `
                <div class="header-chat-box" id="header-chat-box">Chat</div>
                <div class="chat-room">
                    <div class="row">
                        <div class="col-md-5">
                            <ul class="nav nav-tabs contact-list scrollbar-wrapper scrollbar-outer" id="list-group"></ul>
                        </div>
                        <div class="col-md-7">
                            <div class="tab-content scrollbar-wrapper wrapper scrollbar-outer">
                                <div class="tab-pane active" id="contact-views">
                                  <div class="chat-body">
                                    <ul class="chat-message" id="ul-contact-views"></ul>
                                  </div>
                                </div>
                            </div>
                            <div id="form-chat">
                                
                            </div>
                        </div>
                    </div>
                </div>
            `;
            var headerChatBox = document.getElementById('header-chat-box');
            headerChatBox.onclick = function () {
                headerChatBox.parentElement.classList.toggle('active');
            }
        }

    }
    return easyRealTime;
}());