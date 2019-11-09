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
        const _type = item.FeedType;
        const _content = item.Content;
        const _time = new Date(item.TimePost);
        const _sender = item.PosterName == null || void 0 || "" ? "Không xác định" : item.PosterName;

        var itemRender = document.createElement("div");
        itemRender.classList = "item-new-feed item-new-feed__topic";
        itemRender.style.cursor = 'pointer';
        itemRender.id = "compact" + item.ID;
        itemRender.setAttribute('fid', item.ID);
        itemRender.setAttribute('ft', item.FeedType);
        container.prepend(itemRender);

        var el = $("#compact" + item.ID);
        var _shortcontent = $('<div>', { "html": _content }).text();
        if (_shortcontent.length >= 80) {
            _shortcontent = _shortcontent.substr(0, 80) + "..."
        }
        //el.innerHTML += "<div>";
        //el.innerHTML += "<span class='item-new-feed__sender'>" + _sender + "</span> ";
        //el.innerHTML += "<span class='item-new-feed__time'>" + formatDate(_time) + "</span>";
        //el.innerHTML += "</div>";

        //el.innerHTML += "<div class='item-new-feed__title'>" + _title + "</div>";
        //el.innerHTML += "<div class='item-new-feed__content'>" + _shortcontent + "</div>";
        //el.innerHTML += "<div class='item-new-feed__fullcontent d-none'>" + _content + "</input>";
        var buttonAnnouce = $('<button>', { "data-id": item.ID, "class": "btn feed_annouce" + (_type == 1 ? " active" : ""), "data-original-title": "Đánh dấu thông báo", "title": "Đánh dấu thông báo", "onclick": "toggleAnnouncement(this, '" + item.ID + "')" })
            .append($("<i>", { "class": "fas fa-bullhorn" }));

        el.append($("<div>", { "class": "row" })
            .append($("<div>", { "class": "col-md-11", "style": "line-height:2em" })
                .append("<span class='item-new-feed__sender'>" + _sender + "</span> ")
                .append("<span class='item-new-feed__time'>" + formatDate(_time) + "</span>"))
            .append($("<div>", { "class": "col-md-1 text-right" })
                .append(buttonAnnouce.tooltip({ trigger: "hover" }))))
            .append($("<div>", { class: "item-new-feed__title", text: _title }))
            .append($("<div>", { class: "item-new-feed__fullcontent d-none", html: _content }))
            .append($("<div>", { class: "item-new-feed__content", text: _shortcontent }));

        //itemRender.innerHTML += "<div class='fn'>" +
        //"<button class='item-new-feed__like'>Like</button> +
        //"<button data-id='" + item.ID + "' class='item-new-feed__reply' data-toggle='modal' data-target='#new-feed__reply--popup'>Reply</button></div>";
        //el.innerHTML += "<div class='list-comment'></div>";
        el.addEventListener('click', function (event) {
            var _id = item.ID;
            var newFeed = new Newfeed({});
            newFeed._render.Orginal(_id);
            var comment = new Comment({ parent_id: _id });
            comment.load(_id);
        });
    }

    var renderNewFeed = function (item, container, comment, init) {
        if (item == null) {
            return;
        };
        const _title = item.Title;
        const _like = item.Like;
        const _unlike = item.UnLike;
        const _content = item.Content;
        const _type = item.FeedType;
        const _time = new Date(item.TimePost);
        const _sender = item.PosterName == null || void 0 || "" ? "Không xác định" : item.PosterName;

        container.innerHTML += "<div class='item-new-feed item-new-feed__topic' style='cursor: pointer;' id='compact" + item.ID + "' fid='" + item.ID + "' ft='" + item.FeedType + "'></div >";

        //var el = document.getElementById("compact" + item.ID);
        var el = $('#compact' + item.ID);

        var _shortcontent = $('<div>', { "html": _content }).text();
        if (_shortcontent.length >= 80) {
            _shortcontent = _shortcontent.substr(0, 80) + "..."
        }
        var buttonAnnouce = $('<button>', { "data-id": item.ID, "class": "btn feed_annouce" + (_type == 1 ? " active" : ""), "data-original-title": "Đánh dấu thông báo", "title": "Đánh dấu thông báo", "onclick": "toggleAnnouncement(this, '" + item.ID + "')" })
            .append($("<i>", { "class": "fas fa-bullhorn" }));

        el.append($("<div>", { "class": "row" })
            .append($("<div>", { "class": "col-md-11", "style": "line-height:2em" })
                .append("<span class='item-new-feed__sender'>" + _sender + "</span> ")
                .append("<span class='item-new-feed__time'>" + formatDate(_time) + "</span>"))
            .append($("<div>", { "class": "col-md-1 text-right" })
                .append(buttonAnnouce.tooltip({ trigger: "hover" }))))
            .append($("<div>", { class: "item-new-feed__title", text: _title }))
            .append($("<div>", { class: "item-new-feed__fullcontent d-none", html: _content }))
            .append($("<div>", { class: "item-new-feed__content", text: _shortcontent }));
        if (init) {
            $('#new-feed__reply--popup input[name=ParentID]').val(item.ID);
            renderFeedFull(item.ID);
        }
    }

    var renderFeedFull = function (id) {
        var _orgFeed = $('#compact' + id);
        var orgContent = $('#item-feed-full');
        orgContent.empty();
        if (_orgFeed.length == 0) return;
        var _title = _orgFeed.find(".item-new-feed__title").text();
        const _id = _orgFeed.attr('fid');
        const _content = _orgFeed.find(".item-new-feed__fullcontent").html();
        const _time = _orgFeed.find(".item-new-feed__time").text();
        const _sender = _orgFeed.find(".item-new-feed__sender").text();
        const _type = _orgFeed.attr('ft')

        orgContent.append($("<div>", { "class": "title_row row" })
            .append($("<div>", { "class": "item-feed-full__title col-md-10", "text": _title }))
            .append($("<div>", { "class": "col-md-2 text-right" })
                .append($('<button>', { "data-id": _id, "class": "item-new-feed__reply btn-primary btn", "data-toggle": "modal", "data-target": "#new-feed__reply--popup", "data-original-title": "Reply", "title": "Reply", "text": "Reply", "onclick": "initReply()" })
                    //.tooltip().append($("<i>", {"class":"fas fa-reply"}))
                )
            ));
        var buttonAnnouce = $('<button>', { "data-id": _id, "class": "btn feed_annouce" + (_type == 1 ? " active" : ""), "data-original-title": "Annouce", "title": "Annouce", "onclick": "toggleAnnouncement(this, '" + _id + "')" })
            .append($("<i>", { "class": "fas fa-bullhorn" }));
        orgContent.append($("<div>", { "class": "pb-2 pt-2 item-new-feed item-new-feed__reply", "style": "cursor: pointer", "id": _id })
            .append($("<div>", { "class": "row" })
                .append($("<div>", { "class": "col-md-11", "style": "line-height:2em" })
                    .append("<span class='item-new-feed__sender'>" + _sender + "</span> ")
                    .append("<span class='item-new-feed__time'>" + _time + "</span>"))
                .append($("<div>", { "class": "col-md-1 text-right" })
                    .append(buttonAnnouce.tooltip({ trigger: "hover" })))
            )
            .append("<div class='item-new-feed__content'>" + _content + "</div>"));

        //orgContent.append("<div class='fn'><button data-id='" + _id + "' class='item-new-feed__reply' data-toggle='modal' data-target='#new-feed__reply--popup'>Reply</button></div>");
    }

    var renderComment = function (item, container) {
        if (item == null) return;
        const _title = item.Title;
        const _like = item.Like;
        const _unlike = item.UnLike;
        const _content = item.Content;
        const _time = new Date(item.TimePost);
        const _sender = item.PosterName == null || void 0 || "" ? "Undefined" : item.PosterName;
        //$(container).append()
        //container.innerHTML += "<div class='pb-2 pt-2 item-new-feed item-new-feed__reply' style='cursor: pointer;' id='" + item.ID + "'></div >";
        //var el = $('#' + item.ID);

        var el = $('<div>', { "class": "pb-2 pt-2 item-new-feed item-new-feed__reply", "style": "cursor:pointer", "id": item.ID });
        $(container).append(el);
        //document.getElementById(item.ID);
        var buttonAnnouce = $('<button>', {
            "data-id": item.ID, "class": "btn feed_annouce d-none " + (item.FeedType == 1 ? " active" : ""), "data-original-title": "Đánh dấu thông báo", "title": "Đánh dấu thông báo"
            //, "onclick": "toggleAnnouncement(this, '" + item.ID + "')"
        })
            .append($("<i>", { "class": "fas fa-bullhorn" }));

        el.append($("<div>", { "class": "row" })
            .append($("<div>", { "class": "col-md-11", "style": "line-height:2em" })
                .append("<span class='item-new-feed__sender'>" + _sender + "</span> ")
                .append("<span class='item-new-feed__time'>" + formatDate(_time) + "</span>"))
            .append($("<div>", { "class": "col-md-1 text-right" })
                .append(buttonAnnouce.tooltip({ trigger: "hover" })))
        )
            .append("<div class='item-new-feed__content'>" + _content + "</div>");

        //el.innerHTML += "<div>";
        //el.innerHTML += "<div class='col-md-11'>";
        //el.innerHTML += "<span class='item-new-feed__sender'>" + _sender + "</span> ";
        //el.innerHTML += "<span class='item-new-feed__time'>" + formatDate(_time) + "</span>";
        //el.innerHTML += "</div>";
        //el.innerHTML += ""


        ////el.innerHTML += "<div class='item-new-feed__title'>" + _title + "</div>";
        //el.innerHTML += "<div class='item-new-feed__content'>" + _content + "</div>";

        //el.innerHTML += "<div class='fn'>" +
        //"<button class='item-new-feed__like'>Like</button>" +
        //"<button data-id='" + item.ID + "' class='item-new-feed__reply' data-toggle='modal' data-target='#new-feed__reply--popup'>Reply</button></div>";
        //el.innerHTML += "<div class='list-comment'></div>";
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
        itemRender.classList = "pb-2 pt-2 item-new-feed item-new-feed__reply";
        itemRender.style.cursor = 'pointer';
        itemRender.id = item.ID;
        container
            //.querySelector(".list-comment")
            .prepend(itemRender);
        itemRender.innerHTML += "<div>"
        itemRender.innerHTML += "<span class='item-new-feed__sender'>" + _sender + "</span> ";
        itemRender.innerHTML += "<span class='item-new-feed__time'>" + formatDate(_time) + "</span>";
        itemRender.innerHTML += "</div>";

        //itemRender.innerHTML += "<div class='item-new-feed__title'>" + _title + "</div>";
        itemRender.innerHTML += "<div class='item-new-feed__content'>" + _content + "</div>";

        //itemRender.innerHTML += "<div class='fn'>" +
        //"<button class='item-new-feed__like'>Like</button>"
        //"<button data-id='" + item.ID + "' class='item-new-feed__reply' data-toggle='modal' data-target='#new-feed__reply--popup'>Reply</button></div>";
        //itemRender.innerHTML += "<div class='list-comment'></div>";
    }

    Render.prototype.NewFeedNew = renderNewFeedNew;
    Render.prototype.Topic = renderTopic;
    Render.prototype.Orginal = renderFeedFull;
    Render.prototype.NewFeed = renderNewFeed;
    Render.prototype.Comment = renderComment;
    Render.prototype.CommentNew = renderCommentNew;
    return Render;
}()) 