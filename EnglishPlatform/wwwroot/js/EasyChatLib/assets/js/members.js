(function(text){
    "use strict";
    var __MEMBERS           = [];
    var __TEXT              = text;
    var __DEFAULT_MEMBER    = { id: g_EasyChatURL.SYSTEM_EDUSO, name:"Thông tin cập nhật hệ thống",center:"eduso",avatar:"https://eduso.vn/images/Logo.png",isSystem:true};
    var __CSKH_MEMBER       = { id: g_EasyChatURL.CSKH_EDUSO, name:"Chăm sóc khách hàng",center:"eduso",avatar:"https://eduso.vn/images/Logo.png",isSystem:false,isCSKH:true};
    function Member(){

    }
    window.Member   = Member;
    var isExist = function(member){
        if(!__MEMBERS) __MEMBER = [];
        if(__MEMBERS.length == 0) return false;
        return __MEMBERS.filter(function(v){if(v.id == member.id) return v;}).length > 0;
    }
    var createAjax = function(){
        return new Ajax();
    }
    var search = function(textSearch){
        return __MEMBERS.filter(function(v){
            var keys = Object.keys(v);
            for(var i = 0 ; i < keys.length; i++){
                var key = keys[i];
                if(v[key]){
                    if(typeof(v[key]=="string")){
                        var oldValue = __TEXT.ClearUnicode(v[key],true);
                        var realSearch = __TEXT.ClearUnicode(textSearch,true);
                        if(oldValue == realSearch || oldValue.indexOf(realSearch) > -1){
                            return v;
                        }
                    }
                }
            }
        });
    }
    Member.prototype.GetAdmin = function(){
        return __DEFAULT_MEMBER;
    }
    Member.prototype.GetSupportCustomer = function(){
        return __CSKH_MEMBER;
    }
    Member.prototype.Create = function(url,listClass){
        var ajax = createAjax();
        return ajax.proccessWithDataHeader("GET", url, {classNames:listClass} , false).then(function(data){
            //console.log(data);
            __MEMBERS = typeof(data) == "string" ? JSON.parse(data) : data;
        });
    }
    Member.prototype.GetItemByID = function(id){
        if(id == __DEFAULT_MEMBER.id){
            return [__DEFAULT_MEMBER];
        }
        if(id == __CSKH_MEMBER){
            return [__CSKH_MEMBER];
        }
        return __MEMBERS.filter(function(v){if(v.id == id) return v;});
    }
    Member.prototype.GetAll = function(){
        updateItem(__DEFAULT_MEMBER);
        updateItem(__CSKH_MEMBER);
        return __MEMBERS;
    }
    Member.prototype.Search = function(textSearch){
        return search(textSearch);
    }
    Member.prototype.Update = updateItem;
    var updateItem = function(members){
        if(!members.length) members = [members];
        for(var i = 0 ; i < members.length; i++){
            var item = members[i];
            if(!isExist(item)){
                __MEMBERS.push(item);
            }
        }
    };
    Member.prototype.Remove = function(id){
        var member = search(id);
        if(member.length > 0){
            var index = __MEMBERS.indexOf(member[0]);
            __MEMBERS.splice(index,1);
        }
        
    }
    return Member;
}(new Text()))