"use strict";

var Render = (function () {
    function Render() {

    }
    var renderTopic = function (item, container) {

    }
    var renderNewFeedNew = function (item, container) {
        const _title = item.Title;
        const _like = item.Like;
        const _unlike = item.UnLike;
        const _content = item.Content;
        const _time = new Date(item.TimePost);
        const _sender = item.PosterName == null || void 0 || "" ? "Không xác định" : item.PosterName;

        var itemRender = document.createElement("div");
        itemRender.classList = "item-new-feed item-new-feed__topic";
        itemRender.style.cursor = 'pointer';
        itemRender.id = item.ID;
        container.prepend(itemRender);
        itemRender.innerHTML += "<div class='item-new-feed__title'>" + _title + "</div>";
        itemRender.innerHTML += "<div class='item-new-feed__content'>" + _content + "</div>";
        itemRender.innerHTML += "<div class='item-new-feed__time'>" + formatDate(_time) + "</div>";
        itemRender.innerHTML += "<div class='item-new-feed__sender'>" + _sender + "</div>";
        itemRender.innerHTML += "<div class='fn'><button class='item-new-feed__like'>Like</button><button data-id='" + item.ID + "' class='item-new-feed__reply' data-toggle='modal' data-target='#new-feed__reply--popup'>Reply</button></div>";
        
    }
    var renderNewFeed = function (item, container, comment) {
        if (item == null) {
            return;
        };
        const _title = item.Title;
        const _like = item.Like;
        const _unlike = item.UnLike;
        const _content = item.Content;
        const _time = new Date(item.TimePost);
        const _sender = item.PosterName == null || void 0 || "" ? "Không xác định" : item.PosterName;
        container.innerHTML += "<div class='item-new-feed item-new-feed__topic' style='cursor: pointer;' id='" + item.ID + "'></div >";
        var el = document.getElementById(item.ID);
        el.innerHTML += "<div class='item-new-feed__title'>" + _title + "</div>";
        el.innerHTML += "<div class='item-new-feed__content'>" + _content + "</div>";
        el.innerHTML += "<div class='item-new-feed__time'>" + formatDate(_time) + "</div>";
        el.innerHTML += "<div class='item-new-feed__sender'>" + _sender + "</div>";
        el.innerHTML += "<div class='fn'><button class='item-new-feed__like'>Like</button><button data-id='" + item.ID + "' class='item-new-feed__reply' data-toggle='modal' data-target='#new-feed__reply--popup'>Reply</button></div>";
        el.innerHTML += "<div class='list-comment'></div>";
    }
    var renderComment = function (item, container) {
        if (item == null) return;
        const _title = item.Title;
        const _like = item.Like;
        const _unlike = item.UnLike;
        const _content = item.Content;
        const _time = new Date(item.TimePost);
        const _sender = item.PosterName == null || void 0 || "" ? "Không xác định" : item.PosterName;
        container.innerHTML += "<div class='p-5 item-new-feed item-new-feed__reply' style='cursor: pointer;' id='" + item.ID + "'></div >";
        var el = document.getElementById(item.ID);
        el.innerHTML += "<div class='item-new-feed__title'>" + _title + "</div>";
        el.innerHTML += "<div class='item-new-feed__content'>" + _content + "</div>";
        el.innerHTML += "<div class='item-new-feed__time'>" + formatDate(_time) + "</div>";
        el.innerHTML += "<div class='item-new-feed__sender'>" + _sender + "</div>";
        el.innerHTML += "<div class='fn'><button class='item-new-feed__like'>Like</button><button data-id='" + item.ID + "' class='item-new-feed__reply' data-toggle='modal' data-target='#new-feed__reply--popup'>Reply</button></div>";
        el.innerHTML += "<div class='list-comment'></div>";
    }
    var renderCommentNew = function (item, container) {
        if (item == null) return;
       
        const _title = item.Title;
        const _like = item.Like;
        const _unlike = item.UnLike;
        const _content = item.Content;
        const _time = new Date(item.TimePost);
        const _sender = item.PosterName == null || void 0 || "" ? "Không xác định" : item.PosterName;
        var itemRender = document.createElement("div");
        itemRender.classList = "p-4 item-new-feed item-new-feed__reply";
        itemRender.style.cursor = 'pointer';
        itemRender.id = item.ID;
        container.querySelector(".list-comment").prepend(itemRender);
        itemRender.innerHTML += "<div class='item-new-feed__title'>" + _title + "</div>";
        itemRender.innerHTML += "<div class='item-new-feed__content'>" + _content + "</div>";
        itemRender.innerHTML += "<div class='item-new-feed__time'>" + formatDate(_time) + "</div>";
        itemRender.innerHTML += "<div class='item-new-feed__sender'>" + _sender + "</div>";
        itemRender.innerHTML += "<div class='fn'><button class='item-new-feed__like'>Like</button><button data-id='" + item.ID + "' class='item-new-feed__reply' data-toggle='modal' data-target='#new-feed__reply--popup'>Reply</button></div>";
        itemRender.innerHTML += "<div class='list-comment'></div>";
    }
    Render.prototype.NewFeedNew = renderNewFeedNew;
    Render.prototype.Topic = renderTopic;
    Render.prototype.NewFeed = renderNewFeed;
    Render.prototype.Comment = renderComment;
    Render.prototype.CommentNew = renderCommentNew;
    return Render;
}()) 