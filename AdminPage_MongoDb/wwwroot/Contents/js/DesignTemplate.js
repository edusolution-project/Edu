
var notification = (function () {
    "use strict";
    function Noti(msg,type){
        var notiBox = document.getElementById("noti-box");
        if(notiBox == null){
            notiBox = _createBoxNotification();
        }
        notiBox.append(msg);
        notiBox.style.background = "#9f0303f7";
        if(type=="info"){
            notiBox.style.background = "#17a2b8";
        }
        if(type == "warn"){
            notiBox.style.background = "#ffc107";
        }
        if(type == "success"){
            notiBox.style.background = "#28a745";
        }
        _show(notiBox);
    } 
    function _createBoxNotification(){
        const div = document.createElement("div");
              div.classList = "box-notification";
              div.id="noti-box";
              _styleNotification(div);
        document.body.append(div);
        return div;
    }
    function _styleNotification(el){
        el.style.opacity = '0';
        el.style.color='#fff';
        el.fontSize = '1.5em';
        el.fontWeight = '600';
        el.padding = '5px 10px';
        el.position = 'fixed';
        el.top = '90px';
        el.left = '120px';
        el.transitionDuration = '2s';
        el.transitionProperty = 'opacity';
    }
    function _show(el){
        el.style.opacity = '1';
        setTime(_hide(el),5000);
    };
    function _hide(el){
        el.style.opacity = '0';
    }
}());

/**
version : 1.1.0
**/

(function (root, factory) {
    if (typeof define === 'function' && define.amd) {
        // AMD
        define(['Layout'], factory);
    }
    else if (typeof exports === 'object') {
        // CommonJS
        module.exports = factory(require('Layout'));
    }
    else {
        // Browser globals (Note: root is window)
        root.returnExports = factory(root.Layout);
    }
}(this, function () {
    "use strict";
    /** 
        Thông báo notifi
    */
    function _createNotifial(msg) {
        var noti = _setElement("div", "noti", "noti", "");
        noti.classList = "notification";
        noti.append(msg);
        document.body.append(noti);
        return noti;
    }
    function ShowNotifial(msg) {
        var noti = document.getElementById("noti");
        if (noti != null) {
            noti.classList += " show";
        } else {
            _createNotifial(msg).classList += " show";
        }
        setTimeout(() => {
            CloseNotifial();
        }, 10000);
    }
    function CloseNotifial() {
        var noti = document.getElementById("noti");
        if (noti != null) {
            noti.classList = "notification";
        }
    }
    /** 
        úng dụng sẵn sàng chạy
     */
    function onReady(_thisFunc) { window.addEventListener("DOMContentLoaded", _thisFunc, !1); }
    /** 
        phiên bản của plugin
     */
    function version() { return "1.1.0"; };
    /**
        auto submint form
     */
    function deleteDynamicView(idLayout,parrentLayout){
        var url = defaultOptions.url_delete;
        var data = {};
        data.ParrentLayout = parrentLayout;
        data.PartialID = idLayout;
        data.Record = defaultOptions.template_id;
        console.log(data);
        var result = confirm("Bạn muốn xóa view này ?");
        if(result){
            _ajaxPostJsonPromise(url,JSON.stringify(data)).then(function(res){
                var response = JSON.parse(res);
                ShowNotifial(response.message);
                if(response.code == 200){
                    setTimeout(function(){
                        window.location.reload();
                    },3000);
                }
            })
            .catch(function(err){
                ShowNotifial(err.message);
            });
        }else{
            
        }
    }
    function save(id) {
        var frm = _getElement(id);
        var data = new FormData(frm);
        _ajaxPostPromise(frm.action, data).then(function (data) {
            var obj = JSON.parse(data);
            if (obj.code == 200) {
                ShowNotifial(obj.message);
            } else {
                ShowNotifial(obj.message);
            }
        });
    };
    /**
        mở form tùy chọn các thông tin cần thiết
     */
    function openCustomer(id) {
        var form = document.getElementById(id);
        if (form.classList == "") {
            form.classList = "open-customer";
        } else {
            form.classList = "";
        }
    }
    /**
        mở form add thêm view phần dynamic layout
     */
    function openAddItem(idLayout) {
        _createFormPop(idLayout);
    };
    /**
        đưa thông tin sắp xếp layout vào từng form
     */
    var setOrderDynamic = function (id) {
        var element = document.getElementById(id);
        if (element == null) return;
        var items = element.querySelectorAll(".item");
        var arr = [];
        items.forEach(function (item, i) {
           var cmodule = item.querySelector('input[name="CModule"]');
           var partialID = item.querySelector('input[name="PartialID"]');
           var parrentLayout = item.querySelector('input[name="ParrentLayout"]');
           var obj = {};
           obj.CModule= cmodule.value;
           obj.PartialID = partialID.value;
           obj.ParrentLayout = parrentLayout.value;
           obj.Order = i;
           obj.Record = defaultOptions.template_id;
           arr.push(obj);
        });
        // var data = new FormData();
        // data.append("data",);
        _ajaxPostJsonPromise(defaultOptions.url_order,JSON.stringify(arr)).then(function(res){
            console.log(res);
        }).catch(function(err){
            console.error(err);
        });
    }
    /**
        gom tất cả thông tin của 2 thằng array và làm 1
     */
    function exchanges(option, newOption) {
        var i = groupOptions({}, option);
        for (var s in newOption) "object" != typeof option[s] || null === option[s] || Array.isArray(option[s])
            ? void 0 !== newOption[s] && (i[s] = newOption[s])
            : i[s] = groupOptions(option[s], newOption[s]);
        return i
    }
    /**
        phục vụ cho thằng exchanges
     */
    var groupOptions = Object.assign || function (data) {
        for (var e, o = arguments, i = 1, n = arguments.length; i < n; i++) {
            e = o[i];
            for (var s in e)
                Object.prototype.hasOwnProperty.call(e, s) && (data[s] = e[s])
        }
        return data;
    },
        /** tùy chọn element html */
        _getElement = function (_elementID) { return document.getElementById(_elementID); },
        _setElement = function (element, elementID, elementName, elementValue) {
            var el = document.createElement(element);
            el.id = elementID;
            el.name = elementName;
            if (element.toLowerCase() == "input") {
                el.value = elementValue
            } else {
                el.append(elementValue);
            }
            return el;
        },
        _createFormPop = function (idLayout) {
            var blackshadow = document.querySelector("div.shadow-black");
            var modal = document.querySelector("div.modal-template");
            var modalForm = document.getElementById("modal-form");
            if (blackshadow == null) {
                _createPopup(idLayout);
                blackshadow = document.querySelector("div.shadow-black");
                modal = document.querySelector("div.modal-template");
                modalForm = document.getElementById("modal-form");
                var record = _setInputHidden("Record", defaultOptions.template_id);
                modalForm.append(record);
                var input = modalForm.querySelector('input[name="ParrentLayout"]');
                if (input != null) input.value = idLayout;
                var IsDynamic = _setInputHidden("IsDynamic", true);
                var command = _setInputHidden("Command", "AddNewItemDynamic");
                modalForm.append(IsDynamic);
                modalForm.append(command);
                document.addEventListener("click", function (e) {
                    if (e.target == blackshadow) {
                        document.body.removeAttribute("style");
                        modal.style.display = 'none';
                        blackshadow.style.display = 'none';
                        window.location.reload();
                    }
                });
            } else {
                var input = modalForm.querySelector('input[name="ParrentLayout"]');
                input.value = idLayout;
                modal.style.display = 'block';
                blackshadow.style.display = 'block';
            }
        },
        _createGroupBox = function (el, value) {
            var box = document.createElement("div");
            var lable = document.createElement("label");
            box.append(lable);
            lable.append(value);
            box.append(el);
            el.addEventListener("change",function(){
                var selected = el.querySelector('option[value="'+el.value+'"]');
                var input = box.querySelector('input[name="TypeView"]');
                var name = box.querySelector('input[name="Name"]')
                if(selected != null){
                    var typeview = selected.getAttribute("data-typeview");
                    if(input == null){
                        input = _setInputHidden("TypeView",typeview);
                        box.append(input);
                    }else{
                        input.value = typeview;
                    }

                    if(name != null){
                        name.value = selected.innerHTML;
                    }else{
                        name=_setInputHidden("name",selected.innerHTML);
                        box.append(name);
                    }
                }
                else{
                    if(input != null){
                        input.value = "";
                    }
                    if(name != null){
                        name.value = "";
                    }
                }
            });
            return box;
        },
        _createPopup = function (idLayout) {
            const modal = _setElement("div", "modal-add-item", "modal", "");
            modal.classList = "modal-template modal-add-item";
            const modalForm = _setElement("form", "modal-form", "modalForm", "");
            modalForm.action = defaultOptions.url_save;

            var record = _setInputHidden("Record", defaultOptions.template_id);

            modalForm.append(record);

            const modalHeader = _setElement("div", "modal-header", "modal-header", "");
            modalHeader.classList = "modal-header";
            modalHeader.append("Thêm View");
            const modalBody = _setElement("div", "modal-body", "modal-body", "");
            modalBody.classList = "modal-body";
            const template = _setInputHidden("ParrentLayout", idLayout);
            modalBody.append(template);
            _getControl().then(function (data) {
                var obj = JSON.parse(data);
                if (obj != null && obj.code == 200) {
                    var select = _createControlSelect("CModule", "", obj.data);
                    var div = _createGroupBox(select, "Chức năng");
                    var input = _setElement("input", "modalLayoutName", "LayoutName", "");
                    input.setAttribute("required", "");
                    input.classList = "form-control";
                    input.placeholder = "(Ex : Slider)";
                    var div2 = _createGroupBox(input, "Tên chức năng");
                    modalBody.append(div);
                    modalBody.append(div2);
                } else {
                    throw obj.message;
                }
            });


            const modalFooter = _setElement("div", "modal-footer", "modal-footer", "");
            modalFooter.classList = "modal-footer";
            const save = _setButtonSave(modalForm.id);

            modalFooter.append(save);
            modalForm.append(modalHeader);
            modalForm.append(modalBody);
            modalForm.append(modalFooter);
            modal.append(modalForm);

            const shawndowBlack = _setElement("div", "shadow-dark", "box", "");
            shawndowBlack.classList = "shadow shadow-black";

            document.body.append(shawndowBlack);
            document.body.append(modal);
        },
        _setInputHidden = function (name, value) {
            var input = document.createElement("input");
            input.type = "hidden";
            input.name = name,
                input.value = value;
            return input;
        },
        _setButtonSave = function (id) {
            var save = _setElement("button", "", "", "Save");
            save.type = "button";
            save.setAttribute("onclick", "window.Layout.Save('" + id + "')");
            return save;
        }
        //** end tùy chọn html */
        , /** tùy chọn xhr */
        _ajaxGetPromise = function (url, data, async) {
            return new Promise(function (resolve, reject) {
                var xhr = new XMLHttpRequest();
                xhr.open("GET", url, async);
                xhr.onload = function () {
                    if (this.status >= 200 && this.status < 300) {
                        resolve(xhr.response);
                    } else {
                        reject({
                            status: this.status,
                            statusText: xhr.response
                        });
                    }
                };
                xhr.onerror = function () {
                    reject({
                        status: this.status,
                        statusText: xhr.response
                    });
                };
                xhr.send(data);
            });
        },
        _ajaxPostPromise = function (url, data) {
            return new Promise(function (resolve, reject) {
                var xhr = new XMLHttpRequest();
                xhr.open("POST", url);
                xhr.onload = function () {
                    if (this.status >= 200 && this.status < 300) {
                        resolve(xhr.response);
                    } else {
                        reject({
                            status: this.status,
                            statusText: xhr.response
                        });
                    }
                };
                xhr.onerror = function () {
                    reject({
                        status: this.status,
                        statusText: xhr.response
                    });
                };
                xhr.send(data);
            });
        },
        _ajaxPostJsonPromise = function (url, data) {
            return new Promise(function (resolve, reject) {
                var xhr = new XMLHttpRequest();
                xhr.open("POST", url);
                xhr.setRequestHeader("Content-type", "application/json; charset=utf8");
                xhr.onload = function () {
                    if (this.status >= 200 && this.status < 300) {
                        resolve(xhr.response);
                    } else {
                        reject({
                            status: this.status,
                            statusText: xhr.response
                        });
                    }
                };
                xhr.onerror = function () {
                    reject({
                        status: this.status,
                        statusText: xhr.response
                    });
                };
                xhr.send(data);
            });
        },
        //** tùy chọn get api data */
        _getMenuByType = function (type) {
            if (type == "" || type == null || type == void 0) return null;
            const url = defaultOptions.url_menu + "?type=" + type;
            return _ajaxGetPromise(url, { type: type }, true);
        }, 
        _getControl = function () {
            var url = defaultOptions.url_control;
            return _ajaxGetPromise(url, {}, true);
        }, 
        _getView = function (type) {
            var url = defaultOptions.url_view + "?name=" + type;
            return _ajaxGetPromise(url, { name: type }, true);
        }, 
        _createMenuSelect = function (name, value, data) {
            const sl = _setElement("select", name, name, "");
            sl.setAttribute("required", "");
            const opr = _setElement("option", "", "", "---------- chọn chuyên mục ----------");
            sl.append(opr);
            if (data != null) {
                var _listData = data;
                _listData.forEach(function (v, i) {
                    // console.log(v);
                    var option = document.createElement("option");
                    option.value = v.id;
                    // console.log(v, value);
                    v.id == value ? option.setAttribute("selected", "") : option.removeAttribute("selected");
                    v.parentID > 0 ? option.append("---" + v.name) : option.append(v.name);
                    sl.append(option);
                });
            }
            return sl;
        },
        _createControlSelect = function (name, value, data) {
            const sl = _setElement("select", name, name, "");
            sl.classList = "form-control";
            sl.setAttribute("required", "");
            const opr = _setElement("option", "", "", "RenderBody");
            opr.value = "RenderBody";
            sl.append(opr);
            if (data != null) {
                data.forEach(function (v, i) {
                    var newOption = document.createElement("option");
                    newOption.setAttribute("data-typeview",v.code);
                    v.fullName == value ? newOption.setAttribute("selected", "") : newOption.removeAttribute("selected");
                    newOption.value = v.fullName;
                    newOption.innerHTML = v.name;
                    sl.append(newOption);
                });

            }
            return sl;
        }
        //** end tùy chon get data từ api và select option  */
        //**   default tùy chọn */
        ,
        defaultOptions = {
            container_id: "render-layout",
            template_id: 1,
            url_save: "/cpapi/updatetemplate",
            url_template_info: "/cpapi/gettemplateinfo",
            url_menu: "/cpapi/getmenuforweb",
            url_control: "/cpapi/getcmoduleall",
            url_layout: "/cpapi/getlayoutlist",
            url_view: "/cpapi/getpartialviewlist",
            url_order: "/cpapi/UpdateOrderDynamicView",
            url_delete:"/cpapi/DeleteDynamicView"
        },
        //** _self chạy hết ứng dụng allMethod */
        _self = {},
        allMethod = function () {
            function method(options) {
                defaultOptions = exchanges(defaultOptions, options);
                _self = this;
                _self._create().then(function (res) {
                    var obj = JSON.parse(res);
                    if (obj.code == 200) {
                        if (obj.data != null)
                            _self._renderLayout(obj.data);
                        else
                            throw "No Data Found";
                    } else {
                        throw obj.message;
                    }
                });
            }
            // create khởi tạo load data với api gettemplateinfo
            return method.prototype._create = function () {
                const url = defaultOptions.url_template_info + "?ID=" + defaultOptions.template_id;
                return _ajaxGetPromise(url, { ID: defaultOptions.template_id }, true);
            }
                /** render html to document */
                , method.prototype._renderLayout = function (data) {
                    _self._createStaticLayout(data.static);
                    _self._createDynamicLayout(data.dynamic, data.dynamicitem);
                }, method.prototype._createStaticLayout = function (data) {
                    var count = data.length;
                    for (var i = 0; i < count; i++) {
                        var item = data[i];
                        var el = document.getElementById(item.partialID);
                        el.innerHTML = "";
                        var div = document.createElement("div");
                        div.style.display = 'block';
                        div.style.height = "100%";
                        div.style.lineHeight = "40px";
                        div.style.cursor = "pointer";
                        div.setAttribute("onclick", "window.Layout.openCustomer('" + el.id + "Form" + "')");
                        div.append(item.layoutName);
                        el.append(div);
                        var nameEl = _setInputHidden("LayoutName", item.layoutName);
                        var record = _setInputHidden("Record", defaultOptions.template_id);
                        var idView = _setInputHidden("PartialID", item.partialID);
                        var command = _setInputHidden("Command", "UpdateInfoStatic");
                        var form = _self._createFormStatic(el, item.properties, item.cModule);
                        form.append(nameEl);
                        form.append(record);
                        form.append(idView);
                        form.append(command);

                        _self._createListView(form, item.typeView, item.partialView);

                    }
                }, method.prototype._createDynamicLayout = function (data, dataItems) {
                    if (data != null && typeof (data) == typeof ([])) {
                        data.forEach(function (v, i) {
                            var item = v;
                            var el = document.getElementById(item.partialID);
                            el.classList = "sortable o-sortable o-access";
                            var div = document.createElement("div");
                            div.classList = "item-create";
                            _self._createAddItemDynamic(div, item.partialID);
                            el.append(div);
                            _self._createDynamicItems(el, dataItems);
                            sortable('#' + item.partialID)[0].addEventListener('sortupdate', function (e) {
                                setOrderDynamic(item.partialID);
                            });
                        });
                    }
                }, method.prototype._createDynamicItems = function (el, data) {
                    var dataReal = data.filter(function (item) {
                        if (item.parrentLayout == el.id) return item;
                    });
                    dataReal.forEach(function (v, i) {
                        if (v.isBody) {
                            _self._createRenderBody(el,v);
                        } else {
                            _self._createFormDynamic(el, v);
                        }
                    });

                }, method.prototype._createFormStatic = function (el, data, cmodule) {
                    var nameForm = el.id + "Form";
                    var form = _setElement("form", nameForm, nameForm, "");
                    form.action = defaultOptions.url_save;
                    var IsDynamic = _setInputHidden("IsDynamic", false);
                    var CModule = _setInputHidden("CModule", cmodule);
                    var save = _setButtonSave(nameForm);
                    form.append(save);
                    form.append(IsDynamic);
                    form.append(CModule);
                    el.append(form);
                    // append;
                    if (data != null && typeof (data) == typeof ([])) {
                        data.forEach(function (v, i) {
                            const value = v.value;
                            if (v.type != null) {
                                // menu dạng select
                                var lb = _setElement("label", "", "", v.name + " : ");
                                var div = _setElement("div", "", "", lb);
                                var type = v.type.replace("type|", "");
                                _getMenuByType(type).then(function (data) {
                                    var menu = JSON.parse(data)
                                    var _menu = _createMenuSelect("MenuID", value, menu.data);
                                    div.append(_menu);
                                })
                                form.append(div);
                            }
                            else {
                                var div = document.createElement("div");
                                var lb = document.createElement("label");
                                lb.append(v.name + " : ");
                                var input = document.createElement("input");
                                input.setAttribute("required", "");
                                input.name = v.key;
                                input.value = value;
                                input.placeholder = v.name;
                                div.append(lb);
                                div.append(input);
                                form.append(div);
                            }
                        });
                    }
                    return form;
                }, method.prototype._createRenderBody = function (el,data) {
                    var div = _setElement("div", "renderBody", "renderBody", "Render Body");
                    var partialID = _setInputHidden("PartialID", data.partialID);
                    var idView = _setInputHidden("CModule", data.cModule);
                    var parrentLayout = _setInputHidden("ParrentLayout",data.parrentLayout);
                    div.classList="item";
                    div.append(partialID);
                    div.append(idView);
                    div.append(parrentLayout);
                    var delButton = _self._createButtonDelete(data.partialID,data.parrentLayout);
                    div.append(delButton);
                    el.append(div);
                }, method.prototype._createFormDynamic = function (el, data) {
                    // console.log(data);

                    var nameEl = _setInputHidden("LayoutName", data.layoutName);
                    var parrentLayout = _setInputHidden("ParrentLayout", data.parrentLayout);
                    var partialID = _setInputHidden("PartialID", data.partialID);
                    var record = _setInputHidden("Record", defaultOptions.template_id);
                    var idView = _setInputHidden("CModule", data.cModule);
                    var command = _setInputHidden("Command", "UpdateInfoItemDynamic");
                    var group = document.createElement("div");
                    var nameForm = data.partialID + "Form";
                    group.classList = "item";

                    // create delete

                    var delButton = _self._createButtonDelete(data.partialID,data.parrentLayout);
                    group.append(delButton);


                    var div = document.createElement("div");
                    div.style.display = 'block';
                    div.style.height = "100%";
                    div.style.lineHeight = "40px";
                    div.style.cursor = "pointer";
                    div.setAttribute("onclick", "window.Layout.openCustomer('" + nameForm + "')");
                    div.append(data.layoutName);
                    group.append(div);
                    
                    var form = _setElement("form", nameForm, nameForm, "");
                    form.action = defaultOptions.url_save;
                    form.append(nameEl);
                    form.append(record);
                    form.append(parrentLayout);
                    form.append(idView);
                    form.append(command);
                    form.append(partialID);

                    var IsDynamic = _setInputHidden("IsDynamic", true);
                    var CModule = _setInputHidden("CModule", data.cModule);
                    var save = _setButtonSave(nameForm);
                    form.append(save);
                    form.append(IsDynamic);
                    form.append(CModule);
                    group.append(form);
                    el.append(group);
                    _self._createListView(form, data.typeView, data.partialView);
                    // append;
                    if (data != null && data.properties != null && data.properties != []) {
                        data.properties.forEach(function (v, i) {
                            // console.log(v);
                            const value = v.value;
                            if (v.type != null) {
                                // menu dạng select
                                var lb = _setElement("label", "", "", v.name + " : ");
                                var div = _setElement("div", "", "", lb);
                                var type = v.type.replace("type|", "");
                                _getMenuByType(type).then(function (data) {
                                    var menu = JSON.parse(data)
                                    var _menu = _createMenuSelect("MenuID", value, menu.data);
                                    div.append(_menu);
                                })
                                form.append(div);
                            }
                            else {
                                var div = document.createElement("div");
                                var lb = document.createElement("label");
                                lb.append(v.name + " : ");
                                var input = document.createElement("input");
                                input.setAttribute("required", "");
                                input.name = v.key;
                                input.value = value;
                                input.placeholder = v.name;
                                div.append(lb);
                                div.append(input);
                                form.append(div);
                            }
                        });
                    }
                    return form;
                }, method.prototype._createAddItemDynamic = function (el, idLayout) {
                    var button = document.createElement("button");
                    button.setAttribute("onclick", "window.Layout.OpenModal('" + idLayout + "')");
                    button.classList = "btn btn-success";
                    button.append("Create New +");
                    button.type = "button";
                    el.append(button);
                }, method.prototype._createListView = function (el, type, value) {
                    var div = document.createElement("div");

                    var typeView = _setInputHidden("TypeView", type);
                    div.append(typeView);

                    var lb = document.createElement("label");
                    lb.append("Chọn view : ");
                    var select = _setElement("select", "PartialView", "PartialView", "");
                    var opt = _setElement("option", "", "", "---------- Chọn view -------");
                    _getView(type).then(function (data) {
                        select.append(opt);
                        var obj = JSON.parse(data);
                        if (obj.code == 200) {
                            obj.data.forEach(function (v, i) {
                                var options = _setElement("option", "", "", v);
                                v == value ? options.setAttribute("selected", "") : options.removeAttribute("selected");
                                select.append(options);
                            });
                        }

                    });
                    div.append(lb);
                    div.append(select);
                    el.append(div);
                }, method.prototype._createButtonDelete = function(id,parrentID){
                    var button = _setElement("a","","","Xóa");
                    button.href = "javascript:void(0)";
                    button.classList = "btn btn-dynamic btn-delete";
                    button.setAttribute("onclick","window.Layout.delete('"+id+"','"+parrentID+"')");
                    return button;
                }
                , method
        }(),
        d = allMethod;
    window.Layout = window.Layout || {},
        window.Layout.version = version,
        window.Layout.onReady = onReady,
        window.Layout.widget = d,
        window.Layout.Save = save,
        window.Layout.openCustomer = openCustomer;
        window.Layout.delete = deleteDynamicView;
    window.Layout.OpenModal = openAddItem;
}));