var urlBase = "/js/EasyChatLib/";
var ui = new UI({power : urlBase+"assets/Icon/Outline/power.svg",
    avatar: urlBase +"assets/Icon/Outline/person.svg",
    message: urlBase +"assets/Icon/Fill/message-circle.svg",
    image: urlBase +"assets/Icon/Outline/image.svg",
    video: urlBase +"assets/Icon/Outline/film.svg",
    file: urlBase +"assets/Icon/Outline/file-2.svg",
    navigation: urlBase +"assets/Icon/Fill/navigation-2.svg"});
var connectionHubChat = new signalR.HubConnectionBuilder()
    .withUrl("https://easychat.eduso.vn/chathub")
    .build();
(function(message,member,group,signalR,UI){
    "use strict";
    var __defaulConfig = {
        center:"eduso",
        id:"easy-chat",
        currentUser :{id:"test",name:"Hoàng Thái Long",email:"longthaihoang94@gmail.com",center:""},
        members : [{id:"test2",name:"member"}],
        groups:[{id:"test4",name:"group"}]
    };
    var __MESSAGE = message,__MEMBER = member, __GROUP = group,__CURRENTUSER={},__SIGNALR = signalR;
    var time = 0,pageIndex=0,pageSize=10,isDataNull=false;
    var _notification;
    function EasyChat(){
        _notification = new notification(); 
    }
    window.EasyChat = EasyChat;
    var _mergeConfig = function(config){
        if(config){
            var keys = Object.keys(config);
            for(var i = 0 ; i < keys.length;i++){
                var key = keys[i];
                if(config[key]){
                    __defaulConfig[key] = config[key]
                }
            }
        }
    }
    EasyChat.OpenMessageBox = function(self){
        //debugger;
        var parent = self.parentElement;
        var root = getRoot();
        if(!self.classList.contains('active'))
        {
            var listmessage = root.querySelector('.list-messages');
            if(listmessage){
                listmessage.innerHTML ="";
            }
            isDataNull= false;
            pageIndex= 0;
            time = 0;
            var active = parent.querySelector('.active');
            if(active){
                active.classList.remove("active");
            }
            self.classList.add("active");
            var boxMessage = root.querySelector('.easy-chat__content--right');
            if(boxMessage){
                boxMessage.querySelector('.contact-info .avatar>img').src = self.querySelector(".contact-info .avatar>img").src;
                boxMessage.querySelector('.contact-info .user-info .name').innerHTML = self.querySelector(".contact-info .user-info .name").innerHTML;

                //console.log(self.dataset);
                if(self.dataset.group == "true"){
                    loadMessage(__defaulConfig.center,__defaulConfig.currentUser.id,self.dataset.id,"",0,0);
                }
                else{
                    loadMessage(__defaulConfig.center,__defaulConfig.currentUser.id,"",self.dataset.id,0,0);
                }

                boxMessage.classList.add('open');
            }
            var contentBox = root.querySelector('.easy-chat__content');
            if (contentBox) {
                if (window.outerWidth < 992) {
                    contentBox.setAttribute('style', 'z-index:999999999999');
                }
                else {
                    contentBox.setAttribute('style', 'width:0px;border:0;padding:0');
                }
               
            }
            var left = root.querySelector('.easy-chat__content--left');
            if(left){
                left.style.display='none';
            }
        }
        resetItemNoti(self.dataset.id);
    }
    var isLoad = false;
    var createURLloadMessage = function(user,groupId,receiver,start,page){
        if(start && page){
            return __defaulConfig.extendsUrl.GetMessage
            .replace("{user}",user)
            .replace("{receiver}",receiver)
            .replace("{groupId}",groupId)
            .replace("{messageId}","")
            .replace("{startDate}",start)
            .replace("{pageIndex}",page)
            .replace("{pageSize}",pageSize)
        }
        else{
            return __defaulConfig.extendsUrl.GetMessage
            .replace("{user}",user)
            .replace("{receiver}",receiver)
            .replace("{groupId}",groupId)
            .replace("{messageId}","")
            .replace("{startDate}",time)
            .replace("{pageIndex}",pageIndex)
            .replace("{pageSize}",pageSize)
        }
    }
    var loadMessage = function(center,user,groupId,receiver,start,page){
        //{pageIndex}&pageSize={pageSize}
        var ajax = new Ajax();
        //var listMessage = getRoot().querySelector(".list-messages");
        // var startDate = new Date().getTime();
        // if(listMessage.children.length > 0){
        //     startDate = parseFloat(listMessage.children[0].querySelector("time").dataset.time);
        // }
        var url = createURLloadMessage(user,groupId,receiver,start,page);
        if(start && page){
            isLoad = false;
        }
        if(!isLoad && !isDataNull){
            isLoad = true;
            ajax.proccessData("POST",url,null).then(function(res){
                var dataJson = typeof(res) == "string" ? JSON.parse(res) : res;
                if(dataJson != null && dataJson.code == 200){
                    var data = dataJson.data;
                    if(data != null){
                        //console.log(data);
                        var objData = data.data;
                        isDataNull = objData == null || objData.length <= 0;
                        renderMessage(objData,false);
                        pageIndex = objData ==null || objData.length == 0 ? pageIndex : data.pageIndex;
                        isLoad = false;
                    }
                    else{
                        isDataNull = true;
                    }
                }
            }).finally(function(){
                isLoad = false;
            });  
        }
    }
    var renderMessage = function(data,isUpdate){
        var html = "";
        for(var i = 0; data != null && i < data.length ; i++){
            var message = data[i];
            var sender = message.sender;
            //var groupId = message.groupId;
            var isMaster = sender == __defaulConfig.currentUser.id;
            //var isPrivate = __GROUP.GetItemByID(groupId).length <= 0;
            var senderInfo = isMaster ? [__defaulConfig.currentUser] : __MEMBER.GetItemByID(sender);
            html += UI.renderGroupMessage(isMaster,senderInfo[0].name,null,[message]);
        }
        
        var listmessage = getRoot().querySelector('.list-messages');
        if(pageIndex == 0 || isUpdate){
            listmessage.innerHTML += html;
            listmessage.scrollTo(0,listmessage.scrollHeight);
        }
        else{
            listmessage.innerHTML = html + listmessage.innerHTML;
        }
        //console.log(listmessage.height,listmessage.outerHeight);
        
        //addEventOnloadImage();
    }

    var addEventOnloadImage = function () {
        var listImage = getRoot().querySelectorAll("[data-src]");
        if (listImage) {
            setTimeout(function () {
                for (var i = 0; i < listImage.length; i++) {
                    var item = listImage[i];
                    if (item) {
                        if (item.dataset.src != item.src) {
                            item.src = item.dataset.src;
                        }
                    }
                }
            }, 5000);
        }
    }


    EasyChat.prototype.AdminSendMessage = function(user,message){
        var obj = {
            center : __defaulConfig.center,
            user: user,
            groupId: "",
            receiver : user,
            message: message,
        }
        var frm = document.createElement('form');
        frm.setAttribute("enctype","multipart/form-data");
        var formData = new FormData(frm);
        var keys = Object.keys(obj);
        var url = __defaulConfig.extendsUrl.SendMessage,query="";
        //var object = {};
        for(var i = 0; i < keys.length ; i++){
            var key = keys[i];
            if(obj[key]){ 
                query+= query == ""? "?"+key+"="+obj[key] : "&"+key+"="+obj[key];
            }
        }
        new Ajax().proccessData("POST", url+query, formData).then(function(res){
            //{"code":200,"message":"SUCCESS","data":{"content":"xin chao","data":null,"sender":"5d8389c2d5d1bf27e4410c04","groupId":"5e7206342ab6d6169c02b1f8","time":1600450346555.0994,"isDel":false,"ID":"5f64ef2ab7a41d3308cb5e52"}}
            var jsonData = typeof(res) == "string" ? JSON.parse(res) : res;
            if(jsonData.code == 200){
                var listmessage = getRoot().querySelector('.list-messages');
                var messageText = getRoot().querySelector('.form-chat textarea');
                var files = getRoot().querySelector('.form-chat input[type="file"]');
                messageText.value = "";
                files.value = "";
                var isMaster = __defaulConfig.currentUser.id == jsonData.data.sender;
                var senderInfo = isMaster ? [__defaulConfig.currentUser] : __MEMBER.GetItemByID(jsonData.data.sender);
                //var isGroup = getRoot().querySelector('.item-contact.active').dataset.group == "true";
                //var lastItem = listmessage.querySelectorAll(".message");
                listmessage.innerHTML += UI.renderGroupMessage(isMaster,senderInfo[0].name,null,[jsonData.data]);
                listmessage.scrollTo(0, listmessage.scrollHeight);
                //addEventOnloadImage();
            }
            //console.log(res);
        });
    }

    EasyChat.CloseMessageBox = function(){
        var root = getRoot();
        var left = root.querySelector('.easy-chat__content--left');
        if(left){
            left.style.display='block';
        }
        var parent = root.querySelector('.easy-chat__content .list-contact');
        var active = parent.querySelector('.active');
        if(active){
            active.classList.remove("active");
        }
        var boxMessage = root.querySelector('.easy-chat__content--right');
        if(boxMessage){
            boxMessage.classList.remove('open');
        }
        var contentBox = root.querySelector('.easy-chat__content');
        if(contentBox){
            contentBox.removeAttribute('style');
        }
    }
    EasyChat.prototype.UpdateMember = function(member){
        __MEMBER.Update(member);
    }
    EasyChat.prototype.RemoveMember = function(id){
        __MEMBER.Remove(id);
    }
    EasyChat.prototype.UpdateGroup = function(group){
        __GROUP.Update(group);
    }
    EasyChat.prototype.RemoveGroup = function(id){
        __GROUP.Remove(id);
    }
    EasyChat.prototype.GetMembers = function(){
        return __MEMBER.GetAll();
    }
    EasyChat.prototype.GetGroups = function(){
        return __GROUP.GetAll();
    }
    EasyChat.prototype.GetConfig = function(){
        return __defaulConfig;
    }
    EasyChat.prototype.Create = function(obj){
        _mergeConfig(obj);
        __CURRENTUSER = __defaulConfig.currentUser;
        __GROUP.Create(__defaulConfig.url.group.getlist).then(function(){

            var array = __GROUP.GetAll();
            var listString = [];
            for(var i = 0; i < array.length ; i++){
                var item = array[i];
                if(item){
                    listString.push(item.id);
                }
            }
            __MEMBER.Create(__defaulConfig.url.member.getlist,listString).then(function(){
                renderHTML();
                ConnectHub();
                getNoti();
            });
        });
    }
    EasyChat.prototype.Destroy = function(){

    }
    var getRoot = function(){
        return document.getElementById(__defaulConfig.id);
    }
    var renderHTML = function(){
       var root = getRoot();
       if(root){
           root.innerHTML = UI.render();
           addEventBasic();
           renderListData();
       }
    }
    var renderListData = function(){
        var root = getRoot();
        var content = root.querySelector('.easy-chat__content .list-contact');
        content.innerHTML = UI.renderListItemContact(__GROUP.GetAll(),true,"EasyChat.OpenMessageBox(this)");
        content.innerHTML += UI.renderListItemContact(__MEMBER.GetAll(),false,"EasyChat.OpenMessageBox(this)");
        getNotication();
    }
    var getNotication = function(){
        //some things
        //sort
        sortWithNoti();
    }
    var sortWithNoti = function(){

    }
    var addEventBasic = function(){
        var root = getRoot();
        if(root){
            //ball click
            var ball = root.querySelector(".easy-chat__ball");
            var content = root.querySelector('.easy-chat__content');
            if(ball){
                ball.addEventListener('click',function(){
                    if(content){
                        content.classList.toggle('open');
                        resetBoxNoti();
                        EasyChat.CloseMessageBox();
                    }
                });
            }
            //end ball click
            var listmessage = root.querySelector('.content--right__body .list-messages');
            if(listmessage){
                var isDone = true;
                listmessage.addEventListener('scroll',function(e){
                    //console.log(e.target.scrollTop);
                    var top = e.target.scrollTop;
                    if(top <= 5){
                        //add element load
                        var root = getRoot();
                            var parent = root.querySelector('.easy-chat__content .list-contact');
                            //set time out // remove element load
                            var active = parent.querySelector('.active');
                            if(active){
                                if(active.dataset.group == "true"){
                                    loadMessage(__defaulConfig.center,__defaulConfig.currentUser.id,active.dataset.id,"");
                                }
                                else{
                                    loadMessage(__defaulConfig.center,__defaulConfig.currentUser.id,"",active.dataset.id);
                                }
                            }
                        //e.target.removeEventListener('scroll')
                    }
                });
            }
            //search
            var SearchInput = root.querySelector('.input-text-search');
            if(SearchInput){
                SearchInput.addEventListener('keyup',function(){
                    var listContact = root.querySelector('.list-contact');
                    if(listContact){
                        var listItemContact = listContact.querySelectorAll('.item-contact');
                        if(listItemContact){
                            var textExtends = new Text();
                            var value = SearchInput.value;
                            for(var i = 0; i < listItemContact.length ; i++){
                                var contact = listItemContact[i];
                                var name = contact.querySelector('.name');
                                if(textExtends.ClearUnicode(name.innerHTML,true).indexOf(textExtends.ClearUnicode(value,true))<=-1 && value != ""){
                                    contact.style.display='none';
                                }
                                else{
                                    contact.style.display='block';
                                }
                            }

                        }
                    }
                });
            }
            //send message 
            var formChat= root.querySelector('.form-chat');
            if(formChat){
                var textArea = formChat.querySelector('textarea');
                if(textArea){
                    var keyHove = "",valueMessage = "";
                    textArea.addEventListener('keydown',function(e){
                        valueMessage = textArea.value;
                        if(keyHove == "") keyHove = e.key;
                        //console.log("keydow",e.key);
                    });
                    textArea.addEventListener('keyup',function(e){
                        if(keyHove == 'Shift' && e.key == "Enter"){
                            // xong dong
                            console.log('xuong dong');
                        }
                        else{
                            if(keyHove == e.key){
                                keyHove = "";
                            }
                            if(e.key == "Enter"){
                                textArea.value = valueMessage;
                                //console.log('send');
                                SendMessage();
                            }
                        }
                    });
                    textArea.onfocus = function(){
                        var active =getRoot().querySelector('.item-contact.active');
                        if(active){
                            resetItemNoti(active.dataset.id);
                        }
                    }
                }
                var inputFiles = formChat.querySelector('input[type="file"]');
                var btnImage = formChat.querySelector('[id="ec-add-image"]');
                var btnVideo = formChat.querySelector('[id="ec-add-video"]');
                var btnFile = formChat.querySelector('[id="ec-add-file"]');
                var btnSend = formChat.querySelector('.btn-send');
                if(btnSend){
                    btnSend.addEventListener('click',function(){
                        SendMessage();
                    });
                }
                btnImage.addEventListener('click',function(){
                    inputFiles.setAttribute('accept',"image/*");
                    inputFiles.click();
                });
                btnVideo.addEventListener('click',function(){
                    inputFiles.setAttribute('accept',"video/*");
                    inputFiles.click();
                });
                btnFile.addEventListener('click',function(){
                    inputFiles.setAttribute('accept',"application/pdf,application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document*");
                    inputFiles.click();
                });

                inputFiles.addEventListener('change',function(){
                    var files = inputFiles.files;
                    if(files.length > 0){SendMessage();}
                });
            }
        }
    }
    var SendMessage = function(){
        var messageText = getRoot().querySelector('.form-chat textarea').value;
        var files = getRoot().querySelector('.form-chat input[type="file"]').files;
        var msg = "";
        if(messageText.replace(/\s/g, '') != ""){
            msg = messageText;
        }
        if(msg.length == 0 && files.length == 0) return;
        PostMessage(msg,files);
        messageText = "";
    }
    var PostMessage = function(message,files){
        var ajax = new Ajax();
        //(string messageId ,string center, string user, string groupId, string receiver, string message, string connectionId)
        var root = getRoot();
        var active = root.querySelector('.item-contact.active');
        if(active){
            var obj = {
                center : __defaulConfig.center,
                user : __defaulConfig.currentUser.id,
                groupId : active.dataset.group =="true" ? active.dataset.id : null,
                receiver : active.dataset.group =="false" ? active.dataset.id : null,
                message : typeof(message) == "string" ? message : null,
            }
            var frm = document.createElement('form');
            frm.setAttribute("enctype","multipart/form-data");
            var formData = new FormData(frm);
            var keys = Object.keys(obj);
            var url = __defaulConfig.extendsUrl.SendMessage,query="";
            //var object = {};
            for(var i = 0; i < keys.length ; i++){
                var key = keys[i];
                if(obj[key]){ 
                    //object[key] = obj[key];
                    //formData.append(key,obj[key]);
                    query+= query == ""? "?"+key+"="+obj[key] : "&"+key+"="+obj[key];
                }
            }
            if(files){
                if(files.length > 0){
                    for(var i = 0; i < files.length; i++){
                        var file = files[i];
                        formData.append("Files",file);
                    }
                }
            }
            ajax.proccessData("POST", url+query, formData).then(function(res){
                //{"code":200,"message":"SUCCESS","data":{"content":"xin chao","data":null,"sender":"5d8389c2d5d1bf27e4410c04","groupId":"5e7206342ab6d6169c02b1f8","time":1600450346555.0994,"isDel":false,"ID":"5f64ef2ab7a41d3308cb5e52"}}
                var jsonData = typeof(res) == "string" ? JSON.parse(res) : res;
                if(jsonData.code == 200){
                    var listmessage = getRoot().querySelector('.list-messages');
                    var messageText = getRoot().querySelector('.form-chat textarea');
                    var files = getRoot().querySelector('.form-chat input[type="file"]');
                    messageText.value = "";
                    files.value = "";
                    var isMaster = __defaulConfig.currentUser.id == jsonData.data.sender;
                    var senderInfo = isMaster ? [__defaulConfig.currentUser] : __MEMBER.GetItemByID(jsonData.data.sender);
                    //var isGroup = getRoot().querySelector('.item-contact.active').dataset.group == "true";
                    //var lastItem = listmessage.querySelectorAll(".message");
                    listmessage.innerHTML += UI.renderGroupMessage(isMaster,senderInfo[0].name,null,[jsonData.data]);
                    listmessage.scrollTo(0, listmessage.scrollHeight);
                    //addEventOnloadImage();
                }
                //console.log(res);
            });
        }
        else{
            console.log("Không tìm thấy địa chỉ cần gửi");
        }
    }
    var ConnectHub = function(){
        try{
            __SIGNALR.start().then(function () {
                var listGroup = __GROUP.GetAll();
                var strGroupNames = "";
                for(var i = 0; i< listGroup.length;i++){
                    var group = listGroup[i];
                    strGroupNames+= strGroupNames == ""? group.id : ","+group.id;
                }
                //add vao group
                __SIGNALR.invoke("Online", __CURRENTUSER.id, strGroupNames);
            });
        }
        catch (ex) {
            console.log(ex);
            setTimeout(function(){
               ConnectHub();
            },5000);
        }
    }
    //hứng thông tin từ client
    __SIGNALR.on("UserOnline",function(user,connectionId){
        console.log(user,connectionId);
        //update connection for user.
    })
    __SIGNALR.onclose(function(){
        ConnectHub();
    });
    __SIGNALR.on("Notication",function(userId){
        showNoti([userId]);
    });
    __SIGNALR.on("Test",function(userId){
        //showNoti([userId]);
        console.log(userId);
    });
    __SIGNALR.on("RemoveMessage", function (message, groupId) {
        removeMessageHTML(message.ID);
    });

    var removeMessageHTML = function (messageId) {
        var message = getRoot().querySelector("[data-id='" + messageId + "']");
        if (message) {
            message.innerHTML = '<div class="content" style="padding-left:25px; color :#ccc"> tin nhắn đã bị xóa </div>';
            var parent = message.parentElement;
            if (parent) {
                var vm = parent.querySelector(".view-more-meta-data");
                if (vm) {
                    vm.remove();
                }
                var btn = parent.querySelector('.button-extends');
            }
        }
    }

    __SIGNALR.on("ReceiverMessage",function(data,receiver,groupId){

        var id = receiver??groupId;
        var active = getRoot().querySelector("[data-id='"+id+"']");
        if(active){
            if(active.classList.contains("active")){
                if(data.sender != __defaulConfig.currentUser.id){
                    renderMessage([data],true);
                }
            }
        }
        if(data.sender != __defaulConfig.currentUser.id){
            showNoti([id]);
        }
    });
    var getNoti = function(){
        var ajax = new Ajax();
        //"https://localhost:44374/Chat/GetNotifications?user={user}&groupNames={groupNames}"
        var array = __GROUP.GetAll().map(function(value){ return value.id;});
        var url = __defaulConfig.extendsUrl.GetNoti.replace("{user}",__defaulConfig.currentUser.id).replace("{groupNames}",array.toString());
        //proccess = function (method, url, data, async) 
        ajax.proccess("GET", url, null, false).then(function(data){
            var objData = typeof(data) == "string" ? JSON.parse(data) : data;
            if(objData != null && objData.length > 0){
                showNoti(objData);
            }
        });

    }
    var showNoti = function(data){
        setNotiCount(data);
        try{
            if(_notification){
                var el = _notification.show({
                    type: "success",
                    msg: "bạn có "+data.length+" tin nhắn",
                    timeOut: 5000
                });
                el.addEventListener("click",function(){
                    var content = getRoot().querySelector('.easy-chat__content');
                    if(!content.classList.contains("open")){
                        content.classList.add('open');
                    }
                });
            }
        }catch(ex){
            console.log(ex);
        }
    }
    var setNotiCount = function(data){
        var boxNoti = getRoot().querySelector('.easy-chat__ball .box-noti');
        if(boxNoti){
            var noti = boxNoti.querySelector('.noti');
            if(noti){
            noti.innerHTML = data.length > 100 ? "99+" : data.length+"";
            boxNoti.classList.add('open');}
        }
        var contact = getRoot().querySelector('.list-contact');
        if(contact){
            for(var i = 0; i < data.length;i++){
                var id = data[0];
                var item = contact.querySelector('[data-id="'+id+'"]');
                if(item){
                    var noti = item.querySelector('.noti');
                    if(noti){
                        noti.innerHTML = "!";
                        noti.classList.add('open');
                        contact.insertBefore(item,contact.children[0]);
                    }
                }
            }
        }
    }
    var resetItemNoti = function(id){
        var item = getRoot().querySelector('[data-id="'+id+'"]');
        if(item){
            var noti = item.querySelector('.noti');
            if(noti){
                noti.innerHTML = "";
                noti.classList.remove('open');
            }
        }
        checkNotiShowHide();
    }
    var resetBoxNoti = function(){
        var boxNoti = getRoot().querySelector('.easy-chat__ball .box-noti');
        if(boxNoti){
            var noti = boxNoti.querySelector('.noti');
            if(noti){
            noti.innerHTML = "";
            boxNoti.classList.remove('open');}
        }
    }
    var checkNotiShowHide = function(){
        var contact = getRoot().querySelector('.list-contact');
        if(contact){
            var list = contact.querySelectorAll('.noti.open');
            if(list == null || list.length <= 0){
                resetBoxNoti();
            }
        }
    }
    EasyChat.RemoveMessage = function (self) {
        var id = self.dataset.message;
        var user = __defaulConfig.currentUser.id;
        if (id && user) {
            UI.CreateAnswerBox("Bạn muốn xóa tin nhắn này !", function () {
                var ajax = new Ajax();
                //(method, url, data, async)
                ajax.proccess("POST", __defaulConfig.extendsUrl.RemoveMessage.replace("{user}", user).replace("{messageId}", id).replace("&connectionId={connectionId}", ""), JSON.stringify({ user: user, messageId: id }), true)
                    .then(function (res) {
                        var data = typeof (res) == "string" && res != "" ? JSON.parse(res) : res;
                        if (data.code == 200) {
                            removeMessageHTML(id);
                        }

                    });
            });
        }
    }
    EasyChat.RemoveMessageAdmin = function (id,userAdmin) {
        var id = id;
        var user = userAdmin;
        if (id && user) {
            UI.CreateAnswerBox("Bạn muốn xóa tin nhắn này !", function () {
                var ajax = new Ajax();
                //(method, url, data, async)
                ajax.proccess("POST", __defaulConfig.extendsUrl.RemoveMessage.replace("{user}", user).replace("{messageId}", id).replace("&connectionId={connectionId}", ""), JSON.stringify({ user: user, messageId: id }), true)
                    .then(function (res) {
                        var data = typeof (res) == "string" && res != "" ? JSON.parse(res) : res;
                        if (data.code == 200) {
                            removeMessageHTML(id);
                        }

                    });
            });
        }
    }
    EasyChat.ViewMore = function (self) {
        var parent = self.parentElement;
        if (parent) {
            var p = parent.parentElement.querySelector(".meta-data");
            if (p) {
                p.classList.toggle("open");
                if (self.classList.contains("show")) {
                    self.classList.remove("show");
                    self.innerHTML = "xem thêm";
                }
                else {
                    self.classList.add("show");
                    self.innerHTML = "thu gọn";
                }
            }

        }
    }
    return EasyChat;
}(new Message(), new Member(), new Group(), connectionHubChat,ui))