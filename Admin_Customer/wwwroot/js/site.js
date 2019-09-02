"use strict";
var g_actions = ["getlist", "getdetails", "create", "delete", "import", "export"];
// request đến server
var Ajax = function (url, method, data, async) {
    var request = new XMLHttpRequest();
    // Return it as a Promise
    return new Promise(function (resolve, reject) {
        // Setup our listener to process compeleted requests
        request.onreadystatechange = function () {
            //0	UNSENT	Client has been created.open() not called yet.
            //1	OPENED	open() has been called.
            //2	HEADERS_RECEIVED	send() has been called, and headers and status are available.
            //3	LOADING	Downloading; responseText holds partial data.
            //4	DONE	The operation is complete.

            // Only run if the request is complete
            //if (request.readyState == 0) {
            //    console.log('UNSENT	Client has been created.open() not called yet')
            //}
            //if (request.readyState == 1) {
            //    console.log('OPENED	open() has been called')
            //}
            //if (request.readyState == 2) {
            //    console.log('HEADERS_RECEIVED	send() has been called, and headers and status are available')
            //}
            //if (request.readyState == 3) {
            //    console.log('LOADING	Downloading; responseText holds partial data')
            //}
            if (request.readyState == 4) {
                //console.log('DONE -	The operation is complete')
                // Process the response
                if (request.status >= 200 && request.status < 300) {
                    // If successful
                    resolve(request.response);
                } else {
                    // If failed
                    reject({
                        status: request.status,
                        statusText: request.statusText
                    });
                }
            }
        };
        request.open(method || 'GET', url, async || true);
        // Send the request
        request.send(data);
    });
}

function Submit(formName, url, actionName, fn) {

    var form = document.querySelector('form[name="' + formName + '"]');
    var _url = url == "" || url == void 0 || url == null ? form.action : url;
    var _method = form.method;
    var requires = $(form).find(':required');
    var err = false;

    requires.each(function () {
        if ($(this).val() == "" || $(this).val() == null) {
            alert("Vui lòng nhập đủ thông tin");
            $(this).focus();
            err = true;
            return false;
        }
    });

    if (err) return false;

    if (actionName.toLowerCase() == "delete" || actionName.toLowerCase() == "publish" || actionName.toLowerCase == "unpublish") {
        var arr_input = form.querySelector('input[name="ArrID"]');
        var listCheckBox = form.querySelectorAll('input[name="cid"]');
        if (arr_input != null) {
            for (var i = 0; listCheckBox != null && listCheckBox.length > 0 && i < listCheckBox.length; i++) {
                if (listCheckBox[i].checked) {
                    arr_input.value += arr_input.value == ""
                        ? listCheckBox[i].value
                        : "," + listCheckBox[i].value;
                }
            }
        }
    }
    $(form).find("input:disabled").removeAttr("disabled");

    var data = new FormData(form);
    showLoading("Đang cập nhật ...");
    Ajax(_url, _method, data, true)
        .then(function (res) {
            hideLoading();
            if (fn != void 0) fn(res);
        }).catch(function (res) {
            hideLoading();
            //notification("err", "Có lỗi, vui lòng thực hiện lại", 2000);
            alert('Có lỗi, vui lòng thực hiện lại');
            console.log(actionName, res);
        });
}

function Export(formName, url) {
    var form = document.querySelector('form[name="' + formName + '"]');
    var arr_input = form.querySelector('input[name="ArrID"]');
    var listCheckBox = form.querySelectorAll('input[name="cid"]');
    if (arr_input != null) {
        for (var i = 0; listCheckBox != null && listCheckBox.length > 0 && i < listCheckBox.length; i++) {
            if (listCheckBox[i].checked) {
                arr_input.value += arr_input.value == ""
                    ? listCheckBox[i].value
                    : "," + listCheckBox[i].value;
            }
        }
    }
    var listParams = form.querySelectorAll('input');
    var query = "";
    for (var i = 0; i < listParams.length; i++) {
        if (listParams[i].value != null && listParams[i].type != "checkbox") {
            query += query == ""
                ? "?" + listParams[i].name + "=" + listParams[i].value
                : "&" + listParams[i].name + "=" + listParams[i].value;
        }
    }
    var _url = url + query;
    window.open(_url, "_blank");
}

function Add(_this) {
    var modal = document.querySelector(_this.getAttribute("data-target"));
    var form = modal.querySelector('form');
    var inputID = form.querySelector('input[name="ID"]');
    if (inputID != null) {
        inputID.value = "0";
    }
    var listinput = form.querySelectorAll('input');
    for (var i = 0; i < listinput.length; i++) {
        listinput[i].value = "";
    }
    var listselect = form.querySelectorAll('select');
    for (var i = 0; i < listselect.length; i++) {
        listselect[i].value = $(listselect[i]).find('option:first').attr("value");
    }
    $(form).find("input:disabled").removeAttr("disabled");
}

function Edit(id, urlGetData, urlPostData, _this) {
    var modal = document.querySelector(_this.getAttribute("data-target"));
    var form = modal.querySelector('form');
    var inputID = form.querySelector('input[name="ID"]');
    if (inputID == null) {
        inputID = "<input name='ID' type='hidden' value='" + id + "' />";
        form.innerHTML += inputID;
    }
    else {
        inputID.value = id;
    }
    $(form).find("input[locked]").attr("disabled", "disabled");
    var data = new FormData(form);
    Ajax(urlGetData, "POST", data, true).then(function (res) {
        var item = JSON.parse(res);
        var listinput = $(form).find('input');
        for (var i = 0; i < listinput.length; i++) {
            listinput[i].value = item.Data[listinput[i].name];
            if ($(listinput[i]).hasClass("hiddenDate")) {
                var fieldId = $(listinput[i]).attr("id");
                $(listinput[i]).prev().removeClass("hasDatepicker").val($.datepicker.formatDate('dd/mm/yy', new Date(item.Data[listinput[i].name]))).datepicker({
                    changeMonth: true,
                    changeYear: true,
                    dateFormat: 'dd/mm/yy',
                    altField: '#' + fieldId,
                    altFormat: 'yy-mm-dd'
                });
            }
        }
        var listselect = form.querySelectorAll('select');
        for (var i = 0; i < listselect.length; i++) {
            listselect[i].value = item.Data[listselect[i].name];
        }
    });
}

function ExcuteOnlyItem(id, url, fn) {
    var data = new FormData();
    data.append("ArrID", id);
    Ajax(url, "POST", data, true).then(function (res) {
        if (fn != void 0) fn(res);
        console.log(res)
    })
}

function hideModal() {
    $('.modal').hide();
    $('.modal-backdrop').hide();
    $('body').removeClass("modal-open");
}

function ToggleStatus(obj) {
    var action = $(obj).attr("onclick");
    if (action.indexOf("UnPublish") > 0)
        $(obj).attr("onclick", action.replace("UnPublish", "Publish"));
    else
        $(obj).attr("onclick", action.replace("Publish", "UnPublish"));
    $(obj).toggleClass("btn-success").toggleClass("btn-danger");
}

function ToggleSwitch(obj) {
    var action = $(obj).attr("onclick");
    if (action.indexOf("UnPublish") > 0)
        $(obj).attr("onclick", action.replace("UnPublish", "Publish"));
    else
        $(obj).attr("onclick", action.replace("Publish", "UnPublish"));
}

function toggleCollapse(obj) {
    var target = $(obj).attr('data-target');
    $(target).collapse('toggle');
    $(obj).find("i").toggleClass("expand");
}

//icon thông báo , thành công và thất bại
var iconSuccess = '<div class="notify-icon icon-success"><?xml version="1.0" encoding="iso-8859-1"?><svg version="1.1" id="Capa_1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px" viewBox="0 0 50 50" style="enable-background:new 0 0 50 50;" xml:space="preserve"><circle style="fill:#25AE88;" cx="25" cy="25" r="25"/><polyline style="fill:none;stroke:#FFFFFF;stroke-width:2;stroke-linecap:round;stroke-linejoin:round;stroke-miterlimit:10;" points="38,15 22,33 12,25 "/></svg></div>';
var iconError = '<div class="notify-icon icon-error"><?xml version="1.0" encoding="iso-8859-1"?><svg version="1.1" id="Capa_1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px" viewBox="0 0 50 50" style="enable-background:new 0 0 50 50;" xml:space="preserve"><circle style="fill:#D75A4A;" cx="25" cy="25" r="25"/><polyline style="fill:none;stroke:#FFFFFF;stroke-width:2;stroke-linecap:round;stroke-miterlimit:10;" points="16,34 25,25 34,16"/><polyline style="fill:none;stroke:#FFFFFF;stroke-width:2;stroke-linecap:round;stroke-miterlimit:10;" points="16,16 25,25 34,34"/></svg></div>';
//thông báo

// mã hóa
function b64EncodeUnicode(str) {
    if (str == null || str == void 0 || str == "") return "";
    // first we use encodeURIComponent to get percent-encoded UTF-8,
    // then we convert the percent encodings into raw bytes which
    // can be fed into btoa.
    return btoa(encodeURIComponent(str).replace(/%([0-9A-F]{2})/g,
        function toSolidBytes(match, p1) {
            return String.fromCharCode('0x' + p1);
        }));
}

// giải mã
function b64DecodeUnicode(str) {
    if (str == null || str == void 0 || str == "") return "";
    // Going backwards: from bytestream, to percent-encoding, to original string.
    return decodeURIComponent(atob(str).split('').map(function (c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));
}

var notification = function (type, msg, timeOut) {
    if (type == void 0 || type == null) type = "success";
    var all = document.querySelectorAll(".notification");
    if (type == "success") {
        if (all == null || all.length == 0 || all == void 0) {
            document.body.innerHTML += "<div class='notification success' id='notification_0'>" + iconSuccess + " " + msg + "</div>";
        } else {
            document.body.innerHTML += "<div id='notification_" + all.length + "' class='notification success' style='top:" + (10 + 6 * all.length) + "%'>" + iconSuccess + " " + msg + "</div>";
        }
    } else {
        if (all == null || all.length == 0 || all == void 0) {
            document.body.innerHTML += "<div class='notification error' id='notification_0'>" + iconError + " " + msg + "</div>";
        } else {
            document.body.innerHTML += "<div id='notification_" + all.length + "' class='notification error' style='top:" + (10 + 6 * all.length) + "%'>" + iconError + " " + msg + "</div>";
        }
    }
    setTimeout(function () {
        var item = document.getElementById("notification_" + all.length);
        if (item != null) {
            item.remove();
            setShowNotification();
        }
    }, timeOut + (all.length * 1000));
}

//hiện thị thông báo
var setShowNotification = function () {
    var all = document.querySelectorAll(".notification");
    for (var i = 0; i < all.length; i++) {
        all[i].style.top = (10 + 6 * i) + "%";
    }
}


function showLoading(message) {
    if ($("body > .loadingState").length > 0)
        return;
    else {
        $("body").append($("<div>", { "class": "loadingState", "style": "background: black;position: fixed;top: 0;left: 0;right: 0;bottom: 0;opacity: 0.9;z-index: 9999;" }));
        if (message == null || message == "")
            message = "Đang xử lý...";
        $("body > .loadingState").append($("<div>", { "text": message, "style": "padding:10px 50px; border-radius: 10px; background: #CCC; font-size:26px; position:absolute; left: calc(50% - 120px); top: 45%;" }));
    }
}

function hideLoading() {
    $("body > .loadingState").remove();
}

function stopAllMedia() {
    $("video").each(function () {
        $(this)[0].pause();
    });
    $("audio").each(function () {
        $(this)[0].pause();
    });
}