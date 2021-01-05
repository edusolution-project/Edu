(function(){
    "use strict";
    var urlBase = "/js/EasyChatLib/";
    var  _config = {
        power: urlBase +"assets/Icon/Outline/power.svg",
        avatar: urlBase +"assets/Icon/Outline/person.svg",
        message: urlBase +"assets/Icon/Fill/message-circle.svg",
        image: urlBase +"assets/Icon/Outline/image.svg",
        video: urlBase +"assets/Icon/Outline/film.svg",
        file: urlBase +"assets/Icon/Outline/file-2.svg",
        navigation: urlBase +"assets/Icon/Fill/navigation-2.svg",
        edit : urlBase +"assets/Icon/Outline/image-1.svg",
        extends : urlBase +"assets/Icon/Outline/more-vertical.svg",
        trash : urlBase +"assets/Icon/Outline/icon_trash_24.png",
        loading : urlBase +"assets/Icon/loading.svg",
    };
    function UI(config){
        _mergeConfig(config);
        this.render = createRoot.bind(this);
        this.renderListItemContact = renderListItemContact.bind(this);
        this.CreateItemContact = updateItemContact;
    }

    var _mergeConfig = function(config){
        if(config){
            var keys = Object.keys(config);
            for(var i = 0; i < keys.length; i++){
                var key = keys[i];
                if(config[key]){
                    _config[key] = config[key];
                }
            }
        }
    }

    window.UI = UI;
    var createRoot = function(){
        return '<div class="easy-chat" id="easy-chat">'+createBall()+createBody()+'</div>';
    }
    var createBall = function(){
        return '<div class="easy-chat__ball"><div class="icon-ball"><img src="'+_config.message+'" alt="easy chat"></div><div class="box-noti"><div class="noti"></div></div></div>';
    }
    var createBody = function(){
        return '<div class="easy-chat__content">'+createBodyLeft()+createBodyRight()+'</div>';
    }
    var createBodyLeft = function(){
        return '<div class="easy-chat__content--left"><div class="content--left__header"><div class="form-search"><div class="box-text"><span class="icon icon-search"></span><input type="text" class="input-text-search" placeholder="search"></div></div></div><div class="content--left__body"><div class="list-contact small-scroll"></div></div></div>';
    }
    var createBodyRight = function(){
        return '<div class="easy-chat__content--right">'+createBodyRightHeader()+createBodyRightBody()+createBodyRightFooter()+'</div>';
    }
    var createBodyRightHeader = function(){
        return '<div class="content--right__header">'+createContactInfo()+'</div>';
    }
    var createBodyRightBody = function(){
        return '<div class="content--right__body"><div class="list-messages small-scroll"></div></div>';
    }
    var  createBodyRightFooter = function(){
        return  '<div class="content--right__footer">'+
                '<div class="form-chat">'+
                '<input type="file" style="display:none" multiple/>'+
                '<div class="extends">'+
                '<button id="ec-add-image" class="btn btn-sm btn-img">'+
                '<img src="'+_config.image+'" alt="hình ảnh">'+
                '</button>'+
                '<button id="ec-add-video" class="btn btn-sm btn-video">'+
                '<img src="'+_config.video+'" alt="video">'+
                '</button>'+
                '<button id="ec-add-file" class="btn btn-sm btn-file">'+
                '<img src="'+_config.file+'" alt="tệp tin">'+
                '</button>'+
                '</div>'+
                '<textarea resize="none" class="box-text small-scroll" contenteditable="true" placeholder="text ..."></textarea>'+
                '<div class="send">'+
                '<button class="btn btn-send">'+
                '<img src="'+_config.navigation+'" alt="">'+
                '</button>'+
                '</div>'+
                '</div>'+
                '</div>';
    }
    var createContactInfo = function(){
        return '<div class="contact-info">'+
            '<div class="avatar">'+
            '    <img src="'+_config.avatar+'" alt="ảnh đại diện">'+
            '</div>'+
            '<div class="user-info">'+
            '    <div class="name">...</div>'+
            '    <div class="time-online">...</div>'+
            '</div>'+
        '</div>'+
        '<div class="extends">'+
            '<button class="btn btn-close" onclick="EasyChat.CloseMessageBox()">'+
                '<i class="fa fa-arrow-left" aria-hidden="true"></i>'+
            '</button>'+
        '</div>';
    }
    var createItemContact = function (data, isGroup, callBack) {
        var _strLop = isGroup && !data.isSystem ? "Lớp " : "";
        var _bgColor = data.isSystem ? "#45b3f3" : "#fff";
        if (!data.avatar) {
            data.avatar = isGroup ? _config.image : _config.avatar;
        }
        return '<div class="item-contact" style="background:' + _bgColor + '" title="' + data.name + '" data-id="' + data.id + '" data-group="' + isGroup + '" onclick="' + callBack + '"><div class="contact-info"><div class="avatar" data-status=""><img src="' + data.avatar + '" alt=""></div><div class="user-info"><div class="name">' + _strLop + data.name + '</div><div class="status-text">...</div></div></div><div class="time-online">...</div><div class="noti"></div></div>';
    }
    var updateItemContact = function (data, isGroup, callBack) {
        var _strLop = isGroup && !data.isSystem ? "Lớp " : "";
        var _bgColor = data.isSystem ? "#45b3f3" : "#fff";
        if (!data.avatar) {
            data.avatar = isGroup ? _config.image : _config.avatar;
        }
        var div = document.createElement("div");
        div.classList = "item-contact";
        div.setAttribute("style","background:" + _bgColor);
        div.setAttribute("title",data.name);
        div.setAttribute("data-id",data.id);
        div.setAttribute("data-group",isGroup);
        div.setAttribute("onclick",callBack);
        div.innerHTML = '<div class="contact-info"><div class="avatar" data-status=""><img src="' + data.avatar + '" alt=""></div><div class="user-info"><div class="name">' + _strLop + data.name + '</div><div class="status-text">...</div></div></div><div class="time-online">...</div><div class="noti open">1</div>';
        return div;
    }
    var renderListItemContact = function(data,isGroup,callBack){
        var html = '';
        for(var i = 0 ; i < data.length; i++){
            var item = data[i];
            html += createItemContact(item,isGroup,callBack);
        }
        return html;
    }
    UI.prototype.renderGroupMessage = function(isSender,name,avatar,messages,isAdmin,sender){
        var classSender= !isSender && !(isAdmin == true && sender == g_EasyChatURL.SYSTEM_EDUSO) ? "message-sender":"message-receiver";
        var _avatar = !avatar ? _config.avatar : avatar;
        var msgs = renderMessages(messages,isSender,isAdmin);
        var html =
            '<div class="message '+classSender+'" data-user-send="'+sender+'">'+
                '<div class="user-info">'+
                    '<div class="avatar"><img src="'+_avatar+'" alt="'+name+'"></div>'+
                    '<div class="name">'+name+'</div>'+
                '</div>'+
                '<div class="group-message">'+msgs[0]+'</div>'+
                '<div data-time="'+msgs[1]+'" class="time-send">'+ConveterTime(msgs[1])+'</div>'+
            '</div>';
        return html;
    }
    var renderMessages = function(messages,isSender,isAdmin){
        var html = "";
        var time = 0;
        if(messages){
            for(var i =0; i < messages.length; i++){
                var msg = renderMesssage(messages[i],isSender,isAdmin);
                html += msg[0];
                time = time < msg[1] ? msg[1] : time;
            }
        }
        return [html,time];
    }
    var Extensions = {
        IMAGE :["jpg","jpeg","png","gif","tiff"],
        VIDEO :["mp4","3gp","flv","wmv","avi","mpeg","wav","ogg"],
        AUDIO :["mp3","audio"],
        DOC:["pdf","doc","docx","ppt","pptx","txt"]
    }
    var Type ={
        IMAGE:0,AUDIO:1,VIDEO:2,DOC:3,ORTHER:4
    }
    var getExtensionType = function(extension){
        if(Extensions.IMAGE.indexOf(extension)>-1) return Type.IMAGE;
        if(Extensions.AUDIO.indexOf(extension)>-1) return Type.AUDIO;
        if(Extensions.VIDEO.indexOf(extension)>-1) return Type.VIDEO;
        if(Extensions.DOC.indexOf(extension)>-1) return Type.DOC;
        return Type.ORTHER;
    }
    var renderMesssage = function (message, isSender,isAdmin) {
        var html = "";
        var time = typeof (message.time) == "string" ? parseFloat(message.time) : message.time;
        if (message.isDel) {
            html += createDataDel();
        }
        else {
            var medias = message.data;
            var text = message.content;
            
            if (text) {
                html += createDataText(message.ID, text, isSender, message.sender,isAdmin);
            }
            if (medias) {
                var exts = medias.length > 0 && (isSender || (message.sender == g_EasyChatURL.SYSTEM_EDUSO && isAdmin == true)) ? createExtendsSettings(message.ID) : "";
                if (medias.length > 0) {
                    var viewMore = medias.length <= 1 ? "" : "<div class='view-more-meta-data'><a onclick='EasyChat.ViewMore(this)' style='display:block;width:100%;text-align: center;padding-top: 10px;'>xem thêm</a></div>"
                    html += '<div data-id="' + message.ID + '" class="data data-meta"><div class="content"><div class="meta-data">';

                    for (var i = 0; i < medias.length; i++) {
                        if (medias[i]) {
                            html += createMetaData(message.ID, medias[i], isSender);
                        }
                    }
                    html += '</div>' + viewMore+'</div>' + exts + "</div>";
                }
            }
        }
        return [html, time];
    }
    var ConveterTime = function(time){
        var min = 60*24*60;
        var now = new Date().getTime();
        var sub = (now - time)/min;
        if(sub <= 0) return "vừa xong";
        if(sub <= 59) return sub.toFixed(0)+" phút trước";
        if(sub/60 <= 23) return (sub/60).toFixed(0)+ "giờ trước";
        //getHours(), getMinutes(), getSeconds(), getMilliseconds()
        var time = new Date(time);
        var date = time.getDate() >= 10 ? time.getDate().toString() : "0"+time.getDate().toString();
        var year = time.getFullYear();
        var month = time.getMonth()+1>=10 ? (time.getMonth()+1).toString() : "0"+(time.getMonth()+1).toString();
        var hour = time.getHours() >= 10 ? time.getHours().toString() : "0"+time.getHours().toString();
        var minute = time.getMinutes()>=10 ? time.getMinutes().toString() : "0"+time.getMinutes().toString();
        
        return date+"-"+month+"-"+year+" "+hour+":"+minute;
        //var date = new Date(time);
    }
    var createExtendsSettings = function(id){
        var eventOpen = "";//"javascript:this.parentElement.childNodes[1].classList.toggle('open')";
        return '<div class="button-extends"><button class="btn btn-delete" data-message="' + id + '" onclick="EasyChat.RemoveMessage(this)"><img src="' + _config.trash + '" alt="Xóa"></button></div>';
    }
    var createDataDel = function () {
        var exts = "";
        return '<div class="data data-text"><div class="content" style="padding-left:25px; color :#ccc"> tin nhắn đã bị xóa </div>' + exts + '</div>';
    }
    var createDataText = function(id,message,isSender,sender, isAdmin){
        var exts = isSender  || (isAdmin == true && sender == g_EasyChatURL.SYSTEM_EDUSO)? createExtendsSettings(id) : "";
         return '<div data-id="'+id+'" class="data data-text"><div class="content" style="padding-left:25px">'+message+'</div>'+exts+'</div>';
    }
    var createMetaData = function(id,data){
        var type = getExtensionType(data.type.toLowerCase().replace(".", ""));
        var html = '<div class="item-meta-data">';
        switch(type){
            case Type.IMAGE:
                html += '<div class="data-image"><div class="view-full" onclick="UI.OpenImage(this)">xem bản đầy đủ</div><img class="lazy-loaded" data-src="' + data.url + '" src="https://drive.google.com/thumbnail?id=' + data.id + '" alt="' + data.id + data.type + '"></div>';
                break;
            case Type.AUDIO:
                html += '<audio controls><source src="'+data.url+'" type="audio/ogg"><source src="'+data.url+'" type="audio/mpeg">Your browser does not support the audio tag.</audio>';
                break;
            case Type.VIDEO:
                html += '<video style="max-width:300px" src="'+data.url+'" controls>Your browser does not support the video tag.</video>';
                break;
            case Type.DOC:
                html+='<div class="data-'+data.type.replace(".","")+'"><a href="'+data.url+'">'+data.id+data.type+'</a></div>';
                break;
            default:
                html+='<div class="data-file"><a href="'+data.url+'">'+data.id+data.type+'</a></div>';
                break;
        }
        html += '</div>';
        return html;
    }
    UI.OpenImage = function (self) {
        var parent = self.parentElement;
        var data = parent.querySelector("[data-src]");
        if (data) {
            window.open(data.dataset.src, "_blank");
        }
    }
    UI.prototype.CreateAnswerBox = function (message,callBack) {
        if (!message) message = " Bạn muốn xóa tin nhắn này !";
        var div = document.createElement("div");
        div.setAttribute("style", "position:fixed;top:0;left:0;right:0;z-index:999999999999;background:#000;width:100%;height:100%;opacity:0.5");
        document.body.appendChild(div);
        var box = document.createElement("div");
        box.setAttribute("style", "position:fixed;top:0;left:0;right:0;z-index:9999999999999;bottom: 0;background:#fff;opacity:1;width:300px;max-width:90%;margin:auto;height:150px;padding:20px");
        var titleMessage = document.createElement("div");
        titleMessage.setAttribute("style", "padding: 30px 0;text-align: center;font-weight: bold;");
        titleMessage.classList = "title-comfirm-box-chat";
        titleMessage.innerHTML = message;
        var bodyMessage = document.createElement("div");
        bodyMessage.style.textAlign = "center";
        bodyMessage.classList = "body-comfirm-box-chat";
        var buttonYes = document.createElement("span");
        buttonYes.classList = "btn btn-sm btn-danger";
        buttonYes.setAttribute("style", "width:40%;margin-right:5px");
        buttonYes.innerHTML = "yes";
        var buttonNo = document.createElement("span");
        buttonNo.setAttribute("style", "width:40%;margin-left:5px")
        buttonNo.classList = "btn btn-sm btn-primary";
        buttonNo.innerHTML = "no";
       
        buttonYes.onclick = function () {
            callBack.call();
            destroy(div);
            destroy(box);
        };
        buttonNo.onclick = function () {
            destroy(div);
            destroy(box);
        };
        bodyMessage.appendChild(buttonYes);
        bodyMessage.appendChild(buttonNo);
        box.appendChild(titleMessage);
        box.appendChild(bodyMessage);
        document.body.appendChild(box);

        
    }
    var destroy = function (el) {
        if (el) {
            el.remove();
        }
    }
    var setStyleShadow = function (el) {
        el.setAttr
    }
    return UI;
}());