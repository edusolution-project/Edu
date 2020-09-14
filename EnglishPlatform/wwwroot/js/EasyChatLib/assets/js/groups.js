(function(text){
    "use strict";
    var __GROUPS = [];
    var __TEXT = text;
    function Group(){

    }
    window.Group = Group;
    var isExist = function(group){
        if(!__GROUPS) __group = [];
        if(__GROUPS.length == 0) return false;
        return __GROUPS.filter(function(v){if(v.id == group.id) return v;}).length > 0;
    }
    var search = function(textSearch){
        return __GROUPS.filter(function(v){
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
    var createAjax = function(){
        return new Ajax();
    }
    Group.prototype.Create = function(url){
        var ajax = createAjax();
        return ajax.proccess("GET", url, null, false).then(function(data){
            //console.log(data);
            __GROUPS = typeof(data) == "string" ? JSON.parse(data) : data;
        });
        //console.log("abc");
    }
    Group.prototype.GetAll = function(){
        return __GROUPS;
    }
    Group.prototype.Search = function(textSearch){
        return search(textSearch);
    }
    Group.prototype.Update = function(groups){
        if(!groups.length) groups = [groups];
        for(var i = 0 ; i < groups.length; i++){
            var item = groups[i];
            if(!isExist(item)){
                __GROUPS.push(item);
            }
        }
    }
    Group.prototype.Remove = function(id){
        var group = search(id);
        if(group.length > 0){
            var index = __GROUPS.indexOf(group[0]);
            __GROUPS.splice(index,1);
        }
        
    }
    return Group;
}(new Text()))