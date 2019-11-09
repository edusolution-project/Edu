"use strict";
const connection = new signalR.HubConnectionBuilder().withUrl("/hub")
    .configureLogging(signalR.LogLevel.Information)
    .build();
const _ajax = new MyAjax();
const _render = new Render();
var onstart = function () {
    connection.start().then(function () {
        _ajax.proccess("POST", _urlListCourse, {}).then(function (data) {            
            var listGroup = JSON.parse(data).Data;
            for (var i = 0; listGroup != null && i < listGroup.length; i++) {
                const item = listGroup[i];
                connection.invoke("GoToClass", item.ID);
            }
        });
    }).catch(function (err) {
        console.log(err)
    })
}
onstart();

connection.onclose(function () {
    setTimeout(function () { onstart() }, 2000);
})



connection.on("JoinGroup", function (data) {
    //console.log(data);
})
connection.on("LeaveGroup", function (data) {
    //console.log(data);
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
    //console.log(data);
})
