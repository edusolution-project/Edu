"use strict";
function checkall(t, cmd) {
    if (cmd == void 0) {
        var check = t.checked;
        var listCheckbox = document.querySelectorAll("input[name='cid']");
        var count = listCheckbox.length;
        for (var i = 0; i < count; i++) {
            var item = listCheckbox[i];
            item.checked = check;
        }
    } else {
        var _name = cmd;
        var check = t.checked;
        var listCheckbox = document.querySelectorAll("input[name='cid-" + _name + "']");
        var count = listCheckbox.length;
        for (var i = 0; i < count; i++) {
            var item = listCheckbox[i];
            item.checked = check;
        }
    }
}

function checkAllThat(t, cmd) {
    var check = t.checked;
    var listCheckbox = document.querySelectorAll("input[tel=" + cmd + "]");
    var count = listCheckbox.length;
    for (var i = 0; i < count; i++) {
        var item = listCheckbox[i];
        item.checked = check;
    }
}

function AddPermisstion() {
    var frm = window.myform;
    const arrID = document.getElementById("ArrID");
    const checkbox = document.querySelectorAll("input[type='checkbox']");
    console.log(checkbox);
    const count = checkbox.length;

    for (var i = 0; i < count; i++) {
        if (checkbox[i].name.indexOf('cid-') > -1) {
            arrID.value += (arrID.value == '' ? '' : ',') + checkbox[i].value + "|" + checkbox[i].checked;
        }
    }
    frm.submit();
}

function executeOnly(id, cmd) {
    var frm = window.myform;
    var ctrl = frm.querySelector("#ctrl");
    var arrID = document.getElementById("ArrID");
    arrID.value = id;
    frm.action = "/" + ctrl.value + "/" + cmd
    frm.method = "POST";
    frm.submit();
}

function execute(action, param) {
    var arraction = ["active", "nonactive", "delete", "export"];
    var frm = window.myform;
    var ctrl = frm.querySelector("#ctrl");
    if (arraction.indexOf(action) > -1) {
        if (param) {
            var arrID = document.getElementById("ArrID");
            arrID.value = param;
        } else {
            var listcheck = frm.querySelectorAll("input[name='cid']");
            var arrID = document.getElementById("ArrID");
            var count = listcheck.length;
            var cmdParam = "";
            for (var i = 0; i < count; i++) {
                var item = listcheck[i];
                if (item.checked) {
                    cmdParam += (cmdParam === '' ? '' : ',') + item.value;
                }
            }
            arrID.value = cmdParam;
        }
    }
    frm.action = "/" + ctrl.value + "/" + action;
    frm.method = "post";
    frm.submit();
}

function redirect(action, name, value) {
    var href = window.location.pathname.split("/");
    var control = document.getElementById("ctrl").value;

    var search = "";
    if (action != "create" && action != "edit" && action != "import") {
        var source = window.location;
        search = groupSearch(source.search, name, value);
        if (typeof (window.Search) != 'undefined') {
            console.log(window.Search);
            for (var i = 0; i < window.Search.length; i++) {
                if (i === window.Search.length - 1) break;
                var obj = document.getElementById(window.Search[i]);
                if (obj != null) {
                    console.log(obj.value);
                    var objValue = obj.value;
                    if (!CheckSearchDefault(objValue, window.Search[i + 1])) {
                        search = groupSearch(search, window.Search[i + 1], objValue);
                    }
                }
            }
        }
    } else {
        search = groupSearch("", name, value);
    }
    //debugger;
    var url = "/" + control + "/" + action + search;
    window.location.href = url;
}

function redirectsub(action, name, value) {

    var control = document.getElementById("ctrl").value;

    var search = "";
    if (action != "createsub") {
        var source = window.location;
        search = groupSearch(source.search, name, value);
        if (typeof (window.Search) != 'undefined') {
            console.log(window.Search);
            for (var i = 0; i < window.Search.length; i++) {
                if (i === window.Search.length - 1) break;
                var obj = document.getElementById(window.Search[i]);
                if (obj != null) {
                    console.log(obj.value);
                    var objValue = obj.value;
                    if (!CheckSearchDefault(objValue, window.Search[i + 1])) {
                        search = groupSearch(search, window.Search[i + 1], objValue);
                    }
                }
            }
        }
    } else {
        search = groupSearch("", name, value);
    }
    //debugger;
    var url = "/" + control + "/" + action + search;
    debugger;
    window.location.href = url;
}

function editchap() {
    var frm = window.myform;
    var ctrl = frm.querySelector("#chapctrl");

    var search = "";
    if (typeof (window.Search) != 'undefined') {
        console.log(window.Search);
        for (var i = 0; i < window.Search.length; i++) {
            if (i === window.Search.length - 1) break;
            var obj = document.getElementById(window.Search[i]);
            if (obj != null) {
                console.log(obj.value);
                var objValue = obj.value;
                if (!CheckSearchDefault(objValue, window.Search[i + 1])) {
                    search = groupSearch(search, window.Search[i + 1], objValue);
                }
            }
        }
    }

    //debugger;
    frm.action = "/" + ctrl.value + "/edit" + search;
    frm.method = "post";
    frm.submit();
}

function createchap(courseid) {
    var frm = window.myform;
    var ctrl = frm.querySelector("#ctrl");
    frm.action = "/" + ctrl.value + "/create?CourseID=" + courseid;
    frm.method = "post";
    frm.submit();
}
/***********filter**********/
function groupSearch(search, name, value) {
    var regex = /(\?\w+\=\d\d\d\d-\d\d\-\d\d|\&\w+\=\d\d\d\d-\d\d\-\d\d|\?\w+\=\w+|\&\w+\=\w+|\&\w+\=\d+)/g;
    if (search == null || search == "") {
        return (value != null && value != '' && value !== void 0) ? "?" + name + "=" + value : "";
    }
    else {
        var arr = search.match(regex);
        var count = arr == null ? 0 : arr.length;
        var str = '';
        var iscontains = false;
        for (var i = 0; i < count; i++) {
            var item = arr[i];
            var sitem = item.split('=');
            if (item.indexOf(name) > -1) {
                if (value != null && value != '' && value !== void 0) { str += sitem[0] + "=" + value; }
                iscontains = true;
            } else {
                if (sitem[1] != null && sitem[1] != '' && sitem[1] !== void 0) {
                    //console.log(item);
                    str += item;
                }
            }
        }
        var url = (value != null && value != '' && value !== void 0) ? "&" + name + "=" + value : "";
        return iscontains ? str : str + url;
    }

}

function CheckSearchDefault(value, name) {
    if (typeof (window.SearchDefault) != 'undefined') {
        for (var i = 0; i < window.SearchDefault.length; i++) {
            if (i === window.SearchDefault.length - 1) break;
            if (window.SearchDefault[i] == value && window.SearchDefault[i + 1] === name)
                return true;
            i++;
        }
    }
    return false;
}
/************ end *****************/

var ControlPage = {
    createCommand() {
        const arrAction = ["create", "delete", "active", "nonactive", "export", "import", "clear"]
        const arrFn = ["redirect", "execute", "execute", "execute", "execute", "redirect", "execute"];
        const arrTitle = ["Thêm mới", "Xóa", "Hoạt động", "Dừng hoạt động", "Xuất excel", "Excel to DB", "Xóa cache"]
        const arrClass = [
            "btn btn-sm btn-success",
            "btn btn-sm btn-fab btn-danger",
            "btn btn-sm btn-fab btn-success",
            "btn btn-sm btn-fab btn-dark",
            "btn btn-sm btn-primary",
            "btn btn-sm btn-info",
            "btn btn-sm btn-link"
        ]
        const arrIcon = [
            "add_circle",
            "delete_sweep",
            "lock_open",
            "lock",
            "airplay",
            "add_to_queue",
            "block"
        ]
        const content = document.getElementById("command")
        var count = arrAction.length;
        for (var i = 0; i < count; i++) {
            var eli = document.createElement("i");
            eli.classList = "material-icons";
            eli.append(arrIcon[i]);
            var el = document.createElement("a");
            el.href = "javascript:void(0)";
            el.title = arrTitle[i];
            el.classList = arrClass[i];
            el.setAttribute("onclick", arrFn[i] + "('" + arrAction[i] + "')");
            el.append(eli);
            if (arrAction[i] == "create" || arrAction[i] == "export" || arrAction[i] == "import" || arrAction[i] == "clear") {
                el.append(" " + arrAction[i]);
            }
            content.append(el);
        }
    },
    CreatePageView(action, TotalRecord, PageIndex, PageSize) {

    }
}