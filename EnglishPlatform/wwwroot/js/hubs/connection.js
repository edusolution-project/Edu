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
var onstart = function () {
    connection.start().then(function () {

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
    var item = JSON.parse(data);
    if (item.ParentID == null)
        _render.NewFeedNew(item, document.getElementById("list-new-feed"));
    else
        //_render.CommentNew(item, document.getElementById(item.ParentID),1);
        _render.CommentNew(item, document.getElementById("list-comment"));
})
connection.on("Offline", function (data) {
    console.log(data);
})

connection.on("ReceiveGroup", function (data) {
    console.log(data);
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
})
