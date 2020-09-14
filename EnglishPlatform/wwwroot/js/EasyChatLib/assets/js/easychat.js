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
        id:"easy-chat",
        currentUser :{id:"test",name:"Hoàng Thái Long",email:"longthaihoang94@gmail.com",center:""},
        members : [{id:"test2",name:"member"}],
        groups:[{id:"test4",name:"group"}]
    };
    var __MESSAGE = message,__MEMBER = member, __GROUP = group,__CURRENTUSER={},__SIGNALR = signalR;
    
    function EasyChat(){
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
        var parent = self.parentElement;
        var active = parent.querySelector('.active');
        if(active){
            active.classList.remove("active");
        }
        self.classList.add("active");
        var root = getRoot();
        var boxMessage = root.querySelector('.easy-chat__content--right');
        if(boxMessage){
            boxMessage.querySelector('.contact-info .avatar>img').src = self.querySelector(".contact-info .avatar>img").src;
            boxMessage.querySelector('.contact-info .user-info .name').innerHTML = self.querySelector(".contact-info .user-info .name").innerHTML;

            //console.log(self.dataset);
            if(self.dataset.group == "true"){
                loadMessage(__defaulConfig.currentUser.center,__defaulConfig.currentUser.id,self.dataset.id,"");
            }
            else{
                loadMessage(__defaulConfig.currentUser.center,__defaulConfig.currentUser.id,"",self.dataset.id);
            }

            boxMessage.classList.add('open');
        }
    }
    var time = 0;
    var loadMessage = function(center,user,groupId,receiver){
        console.log(center,user,groupId,receiver);
    }
    EasyChat.CloseMessageBox = function(){
        var root = getRoot();
        var parent = root.querySelector('.easy-chat__content .list-contact');
        var active = parent.querySelector('.active');
        if(active){
            active.classList.remove("active");
        }
        var boxMessage = root.querySelector('.easy-chat__content--right');
        if(boxMessage){
            boxMessage.classList.remove('open');
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
                    if(top == 0){
                        //add element load
                        if(isDone){
                            //ko load quá nhiều lần
                            isDone = false;
                            var root = getRoot();
                            var parent = root.querySelector('.easy-chat__content .list-contact');
                            //set time out // remove element load
                            var active = parent.querySelector('.active');
                            if(active){
                                __MESSAGE.Get(active.dataset.id);
                                //then isDone = true;
                                isDone = true;
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
                        if(keyHove == e.key){
                            keyHove = "";
                        }
                        if(keyHove == "" && e.key == "Enter"){
                            textArea.value = valueMessage;
                            //console.log('send');
                            SendMessage();
                        }
                    });
                }
                var inputFiles = formChat.querySelector('input[type="file"]');
                var btnImage = formChat.querySelector('[id="ec-add-image"]');
                var btnVideo = formChat.querySelector('[id="ec-add-video"]');
                var btnFile = formChat.querySelector('[id="ec-add-file"]');
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
            }
        }
    }
    var SendMessage = function(){
        var messageText = getRoot().querySelector('.form-chat textarea').value;
        var files = getRoot().querySelector('.form-chat input[type="file"]').files;
        //var 
        console.log(messageText);
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
            //setTimeout(function(){
            //    ConnectHub();
            //},5000);
        }
    }
    //hứng thông tin từ client
    __SIGNALR.on("UserOnline",function(user,connectionId){
        console.log(user,connectionId);
        //update connection for user.
    })
    __SIGNALR.onclose(function(){
        //ConnectHub();
    });

    return EasyChat;
}(new Message(), new Member(), new Group(), connectionHubChat,ui))