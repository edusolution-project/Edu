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
                console.log('DONE -	The operation is complete')
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
    Ajax(_url, _method, data, true)
        .then(function (res) {
            if (fn != void 0) fn();
        }).catch(function (res) {
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
    Ajax(url, "POST", data, true).then(function () {
        if (fn != void 0) fn();
    })
}
function hideModal() {
    $('.modal').hide();
    $('.modal-backdrop').hide();
    $('body').removeClass("modal-open");
}
//var urlStart = '@Url.Action("Create","Example")';
function StartDoExam(url, Timer, TeacherID, ClassID, LessonID, LessonScheduleID, StudentID) {
    var dataform = new FormData();
        dataform.append("Timer", Timer);
        dataform.append("TeacherID", TeacherID);
        dataform.append("ClassID", ClassID);
        dataform.append("LessonID", LessonID);
        dataform.append("LessonScheduleID", LessonScheduleID);
    dataform.append("StudentID", StudentID);
    Ajax(url, "POST", dataform, false)
        .then(function (res) {
            console.log(res);
        })
        .catch(function (err) {
            console.log(err);
        })

}
//var urlComplete = '@Url.Action("CompleteExam","Example")';
function CompelteExample(url,ExamID,StudentID) {
    var dataform = new FormData();
        dataform.append("ExamID", ExamID);
        dataform.append("StudentID", StudentID);
    Ajax(url, "POST", dataform, false)
        .then(function (res) {
            console.log(res);
        })
        .catch(function (err) {
            console.log(err);
        })
}
//var urlChose = '@Url.Action("CreateDetails","Example")';
function ChosenAnswer(url,ID, ExamID, QuestionID, AnswerID, StudentID,AnswerValue) {
    var dataform = new FormData();
        dataform.append("ExamID", ExamID);
        dataform.append("ID", ID);
        dataform.append("AnswerID", AnswerID);
        dataform.append("QuestionID", QuestionID);
        dataform.append("StudentID", StudentID);
        dataform.append("AnswerValue", AnswerValue);
    Ajax(url, "POST", dataform, false)
        .then(function (res) {
            console.log(res);
        })
        .catch(function (err) {
            console.log(err);
        })
}
function OverTime(ExamID, StudentID) {
    var dataform = new FormData();
    dataform.append("ExamID", ExamID);
    dataform.append("StudentID", StudentID);
    Ajax(url, "POST", dataform, false)
        .then(function (res) {
            console.log(res);
        })
        .catch(function (err) {
            console.log(err);
        })
}
function LocalStore(ExamID, value) {
    localStorage.setItem(ExamID, value);
}
function LoadLocalStore(ExamID) {
    return localStorage.getItem(ExamID);
}