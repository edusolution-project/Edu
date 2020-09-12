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
        navigation: urlBase +"assets/Icon/Fill/navigation-2.svg"
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
                '<div class="extends">'+
                '<button class="btn btn-sm btn-img">'+
                '<img src="'+_config.image+'" alt="hình ảnh">'+
                '</button>'+
                '<button class="btn btn-sm btn-video">'+
                '<img src="'+_config.video+'" alt="video">'+
                '</button>'+
                '<button class="btn btn-sm btn-file">'+
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
            data.avatar = _config.avatar;
        }
        return '<div class="item-contact" data-id="'+data.id+'" data-group="'+isGroup+'" onclick="'+callBack+'"><div class="contact-info"><div class="avatar" data-status=""><img src="'+data.avatar+'" alt=""></div><div class="user-info"><div class="name">'+data.name+'</div><div class="status-text">...</div></div></div><div class="time-online">...</div></div>';
    }
    var renderListItemContact = function(data,isGroup,callBack){
        var html = '';
        for(var i = 0 ; i < data.length; i++){
            var item = data[i];
            html += createItemContact(item,isGroup,callBack);
        }
        return html;
    }
    return UI;
}());