(function(text){
    "use strict";
    var __MEMBERS = [];
    var __TEXT = text;
    function Member(){

    }
    window.Member = Member;
    var isExist = function(member){
        if(!__MEMBERS) __MEMBER = [];
        if(__MEMBERS.length == 0) return false;
        return __MEMBERS.filter(function(v){if(v.id == member.id) return v;}).length > 0;
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
    Member.prototype.GetAll = function(){
        return __MEMBERS;
    }
    Member.prototype.Search = function(textSearch){
        return search(textSearch);
    }
    Member.prototype.Update = function(members){
        if(!members.length) members = [members];
        for(var i = 0 ; i < members.length; i++){
            var item = members[i];
            if(!isExist(item)){
                __MEMBERS.push(item);
            }
        }
    }
    Member.prototype.Remove = function(id){
        var member = search(id);
        if(member.length > 0){
            var index = __MEMBERS.indexOf(member[0]);
            __MEMBERS.splice(index,1);
        }
        
    }
    return Member;
}(new Text()))