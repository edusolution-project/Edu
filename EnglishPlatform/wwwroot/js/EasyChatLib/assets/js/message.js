(function(){
    "use strict";
    function Message(){
        this.Create = function(){

        }
        this.Drop = function(){

        }
        this.Get = function(){
            console.log("x");
        }
    }
    window.Message = Message;

    return Message;
}());