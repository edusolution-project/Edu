"use strict";
var notification = (function () {
    function notification() {
    }
    var _iconError = '<div class="icon icon-error"><?xml version="1.0" encoding="iso-8859-1"?><svg version="1.1" id="Capa_1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px" viewBox="0 0 50 50" style="enable-background:new 0 0 50 50;" xml:space="preserve"><circle style="fill:#D75A4A;" cx="25" cy="25" r="25"/><polyline style="fill:none;stroke:#FFFFFF;stroke-width:2;stroke-linecap:round;stroke-miterlimit:10;" points="16,34 25,25 34,16"/><polyline style="fill:none;stroke:#FFFFFF;stroke-width:2;stroke-linecap:round;stroke-miterlimit:10;" points="16,16 25,25 34,34"/><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g></svg></div>';
    var _iconSuccess = '<div class="icon icon-success"><?xml version="1.0" encoding="iso-8859-1"?><svg version="1.1" id="Capa_1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px" viewBox="0 0 50 50" style="enable-background:new 0 0 50 50;" xml:space="preserve"><circle style="fill:#25AE88;" cx="25" cy="25" r="25"/><polyline style="fill:none;stroke:#FFFFFF;stroke-width:2;stroke-linecap:round;stroke-linejoin:round;stroke-miterlimit:10;" points="38,15 22,33 12,25 "/><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g></svg></div>';
    var config = {
        type: "success",
        msg: "",
        timeOut: 2000
    }
    notification.prototype.show = function (options) {
        groupConfig(options);
        var all = document.querySelectorAll(".notification");
        var div = document.createElement("div");
        var icon = config.type == "success" ? _iconSuccess : _iconError;
        if (all == null || all.length == 0 || all == void 0) {
            div.classList = 'notification ' + config.type;
            div.id = 'notification_0';
            div.innerHTML = icon + " " + config.msg;
            div.setAttribute('style', 'z-index:999999');
            document.body.appendChild(div);
        } else {
            div.classList = 'notification ' + config.type;
            div.id = 'notification_' + all.length;
            div.innerHTML = icon + " " + config.msg;
            div.setAttribute('style', 'top:' + (10 + 6 * all.length) + '%;z-index:999999');
            document.body.appendChild(div);
        }
        setTimeout(function () {
            var item = document.getElementById("notification_" + all.length);
            if (item != null) {
                item.remove();
                setShowNotification();
            }
        }, config.timeOut + (all.length * 1000));

        return div;
    }
    var groupConfig = function (options) {
        if (options == null || typeof (options) == "undefined") return config;
        var keys = Object.keys(options);
        for (var i = 0; i < keys.length; i++) {
            var key = keys[i];
            if (options[key]) config[key] = options[key];
        }
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