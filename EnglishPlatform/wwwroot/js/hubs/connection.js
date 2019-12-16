"use strict";
var changeTimeToString = function (time) {
    var date = new Date(`${time}`);
    console.log(date, time);
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
const connection = new signalR.HubConnectionBuilder().withUrl("/hub")
    .configureLogging(signalR.LogLevel.Information)
    .build();
const _ajax = new MyAjax();
const _render = new Render();
var _easyRealTime = new easyRealTime();
var onstart = function () {
    connection.start().then(function () {
        connection.invoke("GoToClass", "Online");
        _easyRealTime.loadGoToClass();
    }).catch(function (err) {
        console.log(err)
    })
}
onstart();

connection.onclose(function () {
    setTimeout(function () { onstart() }, 2000);
})



connection.on("JoinGroup", function (data) {
    console.log(data);
})
connection.on("LeaveGroup", function (data) {
    console.log(data);
})
connection.on("CommentNewFeed", function (data) {
   
})
connection.on("Offline", function (data) {
    
})

connection.on("ReceiveGroup", function (data) {
    if (data.message.type != void 0 && data.message.type != "" && data.message.type != null) {
        var realData = data.message.obj.data;
        var item = realData;
        if (data.message.type == "newfeed") {
            var strId = item.receivers[0];
            if (item.receivers[0] == null || item.receivers[0] == void 0 || item.receivers[0] == "") {
                strId = "";
            }
            var news = document.getElementById("list-news-old-" + strId);
            var old = document.getElementById(realData.ID);
            if (old == null) {
                var html = `<div class="item-feed" id="${item.ID}">
                                    <div class="user-info-post">
                                        <div class="row">
                                            <div class="col-lg-7">
                                                <div class="info-user">
                                                    <div class="avatar-user">
                                                        <svg width='24' height='24' viewBox='0 0 24 24' fill='none' xmlns='http://www.w3.org/2000/svg'>
                                                            <path d='M24 12C24 18.6275 18.6275 24 12 24C5.3725 24 0 18.6275 0 12C0 5.3725 5.3725 0 12 0C18.6275 0 24 5.3725 24 12Z' fill='#7CC6F1' />
                                                            <path d='M14.7283 11.6221C15.8306 10.7919 16.5447 9.47278 16.5447 7.98981C16.5447 5.48401 14.506 3.44531 12 3.44531C9.4942 3.44531 7.45551 5.48401 7.45551 7.98981C7.45551 9.47278 8.16943 10.7919 9.27173 11.6221C6.32043 12.7299 4.21417 15.5804 4.21417 18.9141H5.62042C5.62042 15.3962 8.48236 12.5345 12.0002 12.5345C15.5178 12.5345 18.3798 15.3962 18.3798 18.9141H19.786C19.7858 15.5804 17.6797 12.7299 14.7283 11.6221ZM8.86176 7.98981C8.86176 6.25928 10.2695 4.85156 12 4.85156C13.7305 4.85156 15.1382 6.25928 15.1382 7.98981C15.1382 9.72034 13.7305 11.1282 12 11.1282C10.2695 11.1282 8.86176 9.72034 8.86176 7.98981Z' fill='white' />
                                                        </svg>
                                                    </div>
                                                    <div class="name">${item.name}</div>
                                                </div>
                                            </div>
                                            <div class="col-lg-5">
                                                <span class="float-right text-time">${formatDate(item.created)}</span>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="content-post">
                                        ${item.content}
                                    </div>
                                    <div class="extends-post">
                                    <a href="javascript:void(0)" onclick="openChat('${item.ID}')">
                                        <span id="number-comment-${item.ID}">0</span> trả lời <svg width="8" height="8" viewBox="0 0 8 8" fill="none" xmlns="http://www.w3.org/2000/svg">
                                            <path fill-rule="evenodd" clip-rule="evenodd" d="M3.98419 0.46476L7.16617 3.64674C7.36144 3.842 7.36144 4.15859 7.16617 4.35385L3.98419 7.53583C3.78893 7.73109 3.47235 7.73109 3.27709 7.53583C3.08183 7.34057 3.08183 7.02398 3.27709 6.82872L5.60551 4.50029L1.18762 4.50029C0.91148 4.50029 0.687622 4.27644 0.687622 4.00029C0.687622 3.72415 0.91148 3.50029 1.18762 3.50029L5.60551 3.50029L3.27709 1.17187C3.08183 0.976605 3.08183 0.660022 3.27709 0.46476C3.47235 0.269498 3.78893 0.269498 3.98419 0.46476Z" fill="#D03239" />
                                        </svg>
                                    </a>
                                    </div>
                                </div>`;
                news.innerHTML += html;
                news.scrollBy(0, news.offsetHeight);
            }
        }
        if (data.message.type == "comment") {
            var numberComment = document.getElementById("number-comment-" + item.parentID);
            if (numberComment != null) {
                numberComment.innerHTML = parseInt(numberComment.innerHTML) + 1;
            }
            var html = `<div class="item-feed item-comment">
                                    <div class="user-info-post">
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <div class="info-user">
                                                    <div class="avatar-user">
                                                        <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                                            <path d="M24 12C24 18.6275 18.6275 24 12 24C5.3725 24 0 18.6275 0 12C0 5.3725 5.3725 0 12 0C18.6275 0 24 5.3725 24 12Z" fill="#7CC6F1"></path>
                                                            <path d="M14.7283 11.6221C15.8306 10.7919 16.5447 9.47278 16.5447 7.98981C16.5447 5.48401 14.506 3.44531 12 3.44531C9.4942 3.44531 7.45551 5.48401 7.45551 7.98981C7.45551 9.47278 8.16943 10.7919 9.27173 11.6221C6.32043 12.7299 4.21417 15.5804 4.21417 18.9141H5.62042C5.62042 15.3962 8.48236 12.5345 12.0002 12.5345C15.5178 12.5345 18.3798 15.3962 18.3798 18.9141H19.786C19.7858 15.5804 17.6797 12.7299 14.7283 11.6221ZM8.86176 7.98981C8.86176 6.25928 10.2695 4.85156 12 4.85156C13.7305 4.85156 15.1382 6.25928 15.1382 7.98981C15.1382 9.72034 13.7305 11.1282 12 11.1282C10.2695 11.1282 8.86176 9.72034 8.86176 7.98981Z" fill="white"></path>
                                                        </svg>
                                                    </div>
                                                    <div class="name">${item.name}</div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="content-post">
                                        ${item.content}
                                    </div>
                                </div>`;
            var ojChat = document.getElementById("comment-box-new-feed");
            var commentBody = ojChat.querySelector(".body-comment");
            commentBody.innerHTML += html;
            commentBody.scrollBy(0, commentBody.offsetHeight);
        }
    } else {
        var ulcontentView = document.getElementById("chat-box").querySelector("#ul-contact-views");
        var item = data.message;
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
            nameUser = data.userSend;
        }
        li.innerHTML = `<div class="${pullClass} avatar-group">${nameUser.trim().substring(0, 1).toUpperCase()}</div>
                            <div class="chat-item">
                                <div class="chat-item-header">
                                    <h5>${nameUser}</h5>
                                    <small class="text-muted">${changeTimeToString(data.time)}</small>
                                </div>
                            <p>${item.content}</p>
                        </div>`;
        ulcontentView.appendChild(li);
        var contactViews = document.getElementById("contact-views");
        contactViews.scrollIntoView({ block: "end" });
    }
    
})
