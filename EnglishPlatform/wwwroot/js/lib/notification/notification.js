"use strict";
var notification = (function () {
    function notification() {
        //this._iconSuccess = '<div class="icon icon-success"><?xml version="1.0" encoding="iso-8859-1"?><svg version="1.1" id="Capa_1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px" viewBox="0 0 50 50" style="enable-background:new 0 0 50 50;" xml:space="preserve"><circle style="fill:#25AE88;" cx="25" cy="25" r="25"/><polyline style="fill:none;stroke:#FFFFFF;stroke-width:2;stroke-linecap:round;stroke-linejoin:round;stroke-miterlimit:10;" points="38,15 22,33 12,25 "/><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g></svg></div>';
        //this._iconError = '<div class="icon icon-error"><?xml version="1.0" encoding="iso-8859-1"?><svg version="1.1" id="Capa_1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px" viewBox="0 0 50 50" style="enable-background:new 0 0 50 50;" xml:space="preserve"><circle style="fill:#D75A4A;" cx="25" cy="25" r="25"/><polyline style="fill:none;stroke:#FFFFFF;stroke-width:2;stroke-linecap:round;stroke-miterlimit:10;" points="16,34 25,25 34,16"/><polyline style="fill:none;stroke:#FFFFFF;stroke-width:2;stroke-linecap:round;stroke-miterlimit:10;" points="16,16 25,25 34,34"/><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g></svg></div>';
    }
    var config = {
        type: "success",
        msg: "",
        timeOut:2000
    }
    notification.prototype.show = function (options) {
        this._config = groupConfig(options);
        var all = document.querySelectorAll(".notification");
        if (this._config.type == "success") {
            if (all == null || all.length == 0 || all == void 0) {
                document.body.innerHTML += "<div class='notification success' id='notification_0'>" + this._iconSuccess + " " + this.msg + "</div>";
            } else {
                document.body.innerHTML += "<div id='notification_" + all.length + "' class='notification success' style='top:" + (10 + 6 * all.length) + "%'>" + this._iconSuccess + " " + this.msg + "</div>";
            }
        } else {
            if (all == null || all.length == 0 || all == void 0) {
                document.body.innerHTML += "<div class='notification error' id='notification_0'>" + this._iconError + " " + this.msg + "</div>";
            } else {
                document.body.innerHTML += "<div id='notification_" + all.length + "' class='notification error' style='top:" + (10 + 6 * all.length) + "%'>" + this._iconError + " " + this.msg + "</div>";
            }
        }
        setTimeout(function () {
            var item = document.getElementById("notification_" + all.length);
            if (item != null) {
                item.remove();
                setShowNotification();
            }
        }, this._config.timeOut + (all.length * 1000));

    }
    var groupConfig = function (options) {
        if (options == null || typeof (options) == "undefined") return config;
        for (var key in options)
            if (options.hasOwnProperty(key)) config[key] = options[key];
        return config;
    }
    var setShowNotification = function () {
        var all = document.querySelectorAll(".notification");
        for (var i = 0; i < all.length; i++) {
            all[i].style.top = (10 + 6 * i) + "%";
        }
    }
    return notification;
}())