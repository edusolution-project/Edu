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
        edit : urlBase +"assets/Icon/Outline/image-1.svg"
    };
    function UI(config){
        _config = config;
        this.render = createRoot.bind(this);
        this.renderListItemContact = renderListItemContact.bind(this);
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
                '<img src="'+_config.power+'" alt="đóng chat">'+
            '</button>'+
        '</div>';
    }
    var createItemContact = function(data,isGroup,callBack){
        if(!data.avatar){
            data.avatar = isGroup ? _config.image : _config.avatar;
        }
        return '<div class="item-contact" title="'+data.name+'" data-id="'+data.id+'" data-group="'+isGroup+'" onclick="'+callBack+'"><div class="contact-info"><div class="avatar" data-status=""><img src="'+data.avatar+'" alt=""></div><div class="user-info"><div class="name">'+data.name+'</div><div class="status-text">...</div></div></div><div class="time-online">...</div></div>';
    }
    var renderListItemContact = function(data,isGroup,callBack){
        var html = '';
        for(var i = 0 ; i < data.length; i++){
            var item = data[i];
            html += createItemContact(item,isGroup,callBack);
        }
        return html;
    }
    UI.prototype.renderGroupMessage = function(isSender,name,avatar,messages){
        var classSender= isSender ? "message-sender":"message-receiver";
        var _avatar = !avatar ? _config.avatar : avatar;
        var msgs = renderMessages(messages,isSender);
        var html =
            '<div class="message '+classSender+'">'+
                '<div class="user-info">'+
                    '<div class="avatar"><img src="'+_avatar+'" alt="'+name+'"></div>'+
                    '<div class="name">'+name+'</div>'+
                '</div>'+
                '<div class="group-message">'+msgs[0]+'</div>'+
                '<div data-time="'+msgs[1]+'" class="time-send">'+ConveterTime(msgs[1])+'</div>'+
            '</div>';
        return html;
    }
    var renderMessages = function(messages,isSender){
        var html = "";
        var time = 0;
        if(messages){
            for(var i =0; i < messages.length; i++){
                var msg = renderMesssage(messages[i],isSender);
                html += msg[0];
                if(i==0){
                    time = msg[1];
                }
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
    var renderMesssage = function(message,isSender){
        var medias = message.data;
        var text = message.content;
        var html = "";
        if(text){
            html += createDataText(message.id,text,isSender);
        }
        if(medias){
            for(var i = 0; i < medias.length; i++){
                if(medias[i]){
                    html += createMetaData(medias[i],isSender);
                }
            }
        }
        return [html,message.time];
    }
    var ConveterTime = function(time){
        return time;
    }
    var createExtendsSettings = function(){
        var eventOpen = "javascript:this.parentElement.childNodes[1].classList.toggle('open')";
        return '<div class="button-extends">'+
            '<div class="dropdown-list">'+
                '<button onclick="'+eventOpen+'" class="btn btn-extends"> <img src="assets/Icon/Outline/more-vertical.svg" alt=""></button>'+
                '<div class="item-extends">'+
                    '<button class="btn btn-delete"><img src="'+_config.image+'" alt="Xóa"></button>'+
                    '<button class="btn btn-edit"><img src="'+_config.edit+'" alt=""></button>'+
                '</div>'+
            '</div>'+
        '</div>';
    }
    var createDataText = function(id,message,isSender){
        var exts = isSender ? createExtendsSettings() : "";
         return '<div data=id="'+id+'" class="data data-text"><div class="content">'+message+'</div>'+exts+'</div>';
    }
    var createMetaData = function(data,isSender){
        var type = getExtensionType(data.type.toLowerCase().replace(".",""));
        var html = '<div class="content">'+'<div data-id="'+data.id+'" class="meta-data">';
        var exts = isSender ? createExtendsSettings():"";
        switch(type){
            case Type.IMAGE:
                html += '<img src="'+data.url+'" alt="'+data.id+data.type+'">';
                break;
            case Type.AUDIO:
                html += '<audio controls><source src="'+data.url+'" type="audio/ogg"><source src="'+data.url+'" type="audio/mpeg">Your browser does not support the audio tag.</audio>';
                break;
            case Type.VIDEO:
                html += '<video src="'+data.url+'" controls>Your browser does not support the video tag.</video>';
                break;
            case Type.DOC:
                html+='<div class="data-'+data.type.replace(".","")+'"><a href="'+data.url+'">'+data.id+data.type+'</a></div>';
                break;
            default:
                html+='<div class="data-file"><a href="'+data.url+'">'+data.id+data.type+'</a></div>';
                break;
        }
        html += '</div></div>'+exts;
        return '<div class="data">'+html+'</div>'

    }
    return UI;
}());