
var Debug = true;
var writeLog = function (name, msg) {
    if (Debug) {
        console.log(name, msg);
    }
}

var ExamStudent = (function () {
    function countdown() {
        clearTimeout(r);
        var time = $(".time-counter").text().trim();
        if (time == "" || time == null) return;
        var minutes = parseInt(time.split(":")[0]);
        var second = parseInt(time.split(":")[1]);
        if (second > 0) {
            second--;
        }
        else {
            if (minutes > 0) {
                minutes--;
                second = 59;
            }
            else {
                ExamComplete();
                return;
            }
        }
        var text = (minutes < 10 ? ("0" + minutes) : minutes) + ":" + (second < 10 ? ("0" + second) : second);
        $(".time-counter").text(text);
        localStorage.setItem("Timer", text);
        var r = setTimeout(function () {
            countdown();
        }, 1000);
    }
    var config = {
        id: "exam-student",
        url: {
            load: "",
            current: "",
            start: "",
            chosen: "",
            end: ""
        },
        lesson_id: "",
        class_id: ""
    }
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
    // icon show menu left
    const arrIcon = {
        "TEXT": "<i class='fas fa-file-word'></i> ",
        "VIDEO": "<i class='fas fa-play-circle'></i> ",
        "AUDIO": "<i class='fas fa-music'></i> ",
        "IMG": "<i class='fas fa-file-image'></i> ",
        "DOC": "<i class='fas fa-file-word'></i> ",
        "VOCAB": "<i class='fas fa-file-word'></i> "
    };
    //icon thông báo , thành công và thất bại
    const iconSuccess = '<div class="icon icon-success"><?xml version="1.0" encoding="iso-8859-1"?><svg version="1.1" id="Capa_1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px" viewBox="0 0 50 50" style="enable-background:new 0 0 50 50;" xml:space="preserve"><circle style="fill:#25AE88;" cx="25" cy="25" r="25"/><polyline style="fill:none;stroke:#FFFFFF;stroke-width:2;stroke-linecap:round;stroke-linejoin:round;stroke-miterlimit:10;" points="38,15 22,33 12,25 "/><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g></svg></div>';
    const iconError = '<div class="icon icon-error"><?xml version="1.0" encoding="iso-8859-1"?><svg version="1.1" id="Capa_1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px" viewBox="0 0 50 50" style="enable-background:new 0 0 50 50;" xml:space="preserve"><circle style="fill:#D75A4A;" cx="25" cy="25" r="25"/><polyline style="fill:none;stroke:#FFFFFF;stroke-width:2;stroke-linecap:round;stroke-miterlimit:10;" points="16,34 25,25 34,16"/><polyline style="fill:none;stroke:#FFFFFF;stroke-width:2;stroke-linecap:round;stroke-miterlimit:10;" points="16,16 25,25 34,34"/><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g><g></g></svg></div>';
    //thông báo
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

    
    var onReady = function (yourConfig) {
        config = groupConfig(yourConfig);
        writeLog(this.__proto__.constructor.name, config);
        getCurrentData();
    }
    var getCurrentData = function () {
        var data = currentData();
        if (data == null || data == {} || data == "" || data == "{}" || data == void 0) {
            var formData = new FormData();
            formData.append("LessonID", config.lesson_id);
            formData.append("ClassID", config.class_id);
            Ajax(config.url.load, formData, "POST", true).then(function (res) {
                if (res != "Accept Deny" && res != "null" && res != null && res.message != "res is not defined" && res != void 0) {
                    var resData = JSON.parse(res);
                    renderLesson(resData);
                    setCurrentData(resData);
                    if (resData.Data.TemplateType != 1) {
                        renderBoDem();
                        $("#counter").html(resData.Timer);
                    }
                    startDragDrop();
                }
            });
        } else {
            renderLesson(data);
            if (data.Data.TemplateType != 1) {
                renderBoDem();
                $("#counter").html(localStorage.getItem("Timer"));
                countdown();
            }
            startDragDrop();
        }
    }
    var setCurrentData = function (data) {
        var enData = b64EncodeUnicode(JSON.stringify(data))
        localStorage.setItem(config.lesson_id + "_" + config.class_id, enData);
    }
    var currentData = function () {
        var dataform = new FormData();
        dataform.append("ClassID", config.class_id);
        dataform.append("LessonID", config.lesson_id);
        Ajax(config.url.current, dataform, "POST", true).then(function (res) {
            if (res == null || res == "null") { localStorage.clear(); return null; }
            else {
                var data = JSON.parse(res);
                document.querySelector("input[name='ExamID']").value = data.ID;
            }
        });
        var data = localStorage.getItem(config.lesson_id + "_" + config.class_id);
        var enData = b64DecodeUnicode(data)
        if (enData == null || enData == {} || enData == "" || enData == "{}" || enData == void 0) {
            return null;
        } else {
            return JSON.parse(enData);
        }
    }
    var renderLesson = function (data) {
        var tab = '<div class="text-right col-lg-4"><button class="prevtab btn btn-success mr-2" data-toggle="tooltip" title="Quay lại" onclick="tab_goback()"><i class="fas fa-arrow-left"></i></button><button class="nexttab btn btn-success" data-toggle="tooltip" title="Tiếp tục" onclick="tab_gonext()"><i class="fas fa-arrow-right"></i></button></div>';
        writeLog("renderLesson", data);
        var container = document.getElementById(config.id);
        if (container == null) throw "Element id " + config.id + " not exist - create now";
        if (data.Data.TemplateType == 1) {
            // bài giảng
            writeLog("bài giảng", data);
            container.innerHTML = '<div class="" id="lessonContainer"></div>';
            var lessonContainer = container.querySelector("#lessonContainer");
            lessonContainer.innerHTML = '<div class="lesson lesson-box"><div class="lesson-container"><div class="card shadow mb-4"><div class="card-header"><div class="row"><div class="lesson-header-title col-lg-8">' + data.Data.Title + '</div>' + tab + '</div></div><div id="body-exam" class="card-body"></div></div></div></div>';
            var bodyExam = lessonContainer.querySelector("#body-exam");
            var content = renderContent(data.Data);
            bodyExam.innerHTML = '<div class="row">' + content + '</div>';
        }
        else {
            // bài thi
            writeLog("bài tập", data);
            var counter = data.Data.Timer >= 10 ? data.Data.Timer + ":00" : "0" + data.Data.Timer + ":00";
            if (localStorage.getItem("Timer") == null) localStorage.setItem("Timer", counter);
            container.innerHTML = '<div class="" id="lessonContainer"></div>';
            var lessonContainer = container.querySelector("#lessonContainer");
            lessonContainer.innerHTML = '<div class="lesson lesson-box"><div class="lesson-container"><div class="card shadow mb-4"><div class="card-header"><div class="row"><div class="lesson-header-title col-lg-4">' + data.Data.Title + '</div><div class="text-center col-lg-4">Thời gian làm bài <span id="counter" class="time-counter">' + counter + '</span></div>' + tab + '</div></div><div id="body-exam" class="card-body"></div></div></div></div>';
            var bodyExam = lessonContainer.querySelector("#body-exam");
            var content = checkExam()
                ? renderContent(data.Data)
                : '<div class="justify-content-center pt-5 pb-5"><div class="btn btn-primary" onclick="BeginExam(this)" style="cursor: pointer;"> Bắt đầu làm bài thi</div></div>';
            bodyExam.innerHTML = '<div class="row">' + content + '</div>';
            if (checkExam) {
                document.querySelector("input[name='ExamID']").value = localStorage.getItem("CurrentExam");
            }
        }
    }
    /// tồn tại exam cũ thì return true;
    var checkExam = function () {
        var exam = document.getElementById("ExamID");
        if (exam == null || exam.value == void 0 || exam.value == null || exam.value == "") return false;
        return localStorage.getItem("CurrentExam") != null && localStorage.getItem("CurrentExam") != void 0 && localStorage.getItem("CurrentExam") != "" && localStorage.getItem("CurrentExam") != typeof(void 0);
    }
    //render bài tập
    var rednerLessonPart = function (data, index,type) {
        writeLog("rednerLessonPart", data);
        var active = "";
        if (index != void 0 && index == 0) {
            active = "show active";
        }
        var html = '<div id="pills-part-' + data.ID + '" class="tab-pane fade ' + active + '" role="tabpanel" aria-labelledby="pills-' + data.ID + '">';
        html += '<div class="part-box ' + data.Type + '" id="' + data.ID + '">';
        switch (data.Type) {
            case "QUIZ1": html += type == 0 ? renderQUIZ1(data) : renderQUIZ1_BG(data);
                break;
            case "QUIZ2": html += type == 0 ? renderQUIZ2(data) : renderQUIZ2_BG(data);
                break;
            case "QUIZ3": html += type == 0 ? renderQUIZ3(data) : renderQUIZ3_BG(data);
                break;
            case "ESSAY": html += type == 0 ? renderESSAY(data) : renderESSAY_BG(data);
                break;
            case "VOCAB": html += type == 0 ? renderVOCAB(data) : renderVOCAB_BG(data);
                break;
            case "DOC": html += renderDOC(data);
                break;
            case "IMG": html += renderIMG(data);
                break;
            case "AUDIO": html += renderAUDIO(data);
                break;
            case "VIDEO": html += renderVIDEO(data);
                break;
            default: html += renderTEXT(data); //text
                break;
        }
        html += '</div></div>';
        return html;
    }
    var renderTEXT = function (data) {
        var title = '<div class="part-box-header"><h5 class="title">' + data.Title + '</h5></div>';
        var html = title + '<div class="content-wrapper">';
        html += '<div class="row"></div>';
        html += '<div class="doc-content">' + data.Description + '</div>';
        html += '</div>';
        return html;
    }
    var renderVIDEO = function (data) {
        var title = '<div class="part-box-header"><h5 class="title">' + data.Title + '</h5></div>';
        var html = title + '<div class="media-wrapper">';
        html += '<div class="media-holder VIDEO"><video controls=""><source src="' + data.Media.Path + '" type="' + data.Media.Extension + '">Your browser does not support the video tag</video></div>';
        html += '</div>';
        return html;
    }
    var renderAUDIO = function (data) {
        var title = '<div class="part-box-header"><h5 class="title">' + data.Title + '</h5></div>';
        var html = title + '<div class="media-wrapper">';
        html += '<div class="media-holder ' + data.Type + '"><audio controls=""><source src="' + data.Media.Path + '" type="' + data.Media.Extension + '">Your browser does not support the audio tag</audio></div>';
        html += '</div>';
        return html;
    }
    var renderIMG = function (data) {
        var title = '<div class="part-box-header"><h5 class="title">' + data.Title + '</h5></div>';
        var html = title + '<div class="media-wrapper">';
        html += '<div class="media-holder ' + data.Type + '"><img src="' + data.Media.Path + '" title="' + data.Title + '" class="img-fluid lazy"></div>';
        html += '</div>';
        return html;
    }
    var renderDOC = function (data) {
        var title = '<div class="part-box-header"><h5 class="title">' + data.Title + '</h5></div>';
        var html = title + '<div class="media-wrapper">';
        html += '<div class="media-holder ' + data.Type + '"><embed src="' + data.Media.Path + '" style="width: 100%; height: 800px; border:1px solid"></div>';
        html += '</div>';
        return html;
    }
    var renderVOCAB = function (data) {
        var title = '<div class="part-box-header"><h5 class="title">' + data.Title + '</h5></div>';
        var html = title + '<div class="media-wrapper">';
        html += '<div class="media-holder ' + data.Type + '">Not Support</div>';
        html += '</div>';
        return html;
    }
    var renderQUIZ1 = function (data) {
        var html = '<div class="part-box-header"> <h5 class="title">' + data.Title + '</h5>' + renderMedia(data.Media) + '</div>';
        html += '<div class="quiz-wrapper">';
        for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
            var item = data.Questions[i];
            html += '<div class="quiz-item" id="' + item.ID + '">';
            html += '<div class="quiz-box-header"><h5 class="title"> - ' + item.Content + ' (' + item.Point + 'đ)</h5>' + renderMedia(item.Media) + '</div>';
            html += '<div class="answer-wrapper">';
            for (var x = 0; item.CloneAnswers != null && x < item.CloneAnswers.length; x++) {
                var answer = item.CloneAnswers[x];
                html += '<fieldset class="answer-item" id="' + answer.ID + '">';
                html += '<div class="form-check">';
                html += '<input id="' + answer.ID + '" type="radio" data-type="QUIZ1" value="' + answer.Content+'" class="input-checkbox answer-checkbox form-check-input" onclick="AnswerQuestion(this)" name="rd_' + item.ID + '">';
                html += '<label class="answer-text form-check-label" for="' + answer.ID + '">' + answer.Content + '</label>';
                html += '</div>';
                html += renderMedia(answer.Media);
                html += '</fieldset>';
            }
            html += '</div></div>';
        }
        html += '</div>';
        return html;
    }
    var renderQUIZ2 = function (data) {
        var html = '<div class="part-box-header"> <h5 class="title">' + data.Title + '</h5>' + renderMedia(data.Media) + '</div>';
        html += '<div class="quiz-wrapper">';
        for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
            var item = data.Questions[i];
            var itemContent = item.Content == null ? "Câu hỏi số " + (i + 1) + " : " : item.Content;
            html += '<div class="quiz-item" id="' + item.ID + '">';
            html += '<div class="quiz-box-header"><h5 class="title"> - ' + itemContent + ' (' + item.Point + 'đ)</h5>' + renderMedia(item.Media) + '</div>';
            html += '<div class="answer-wrapper">';
            html += '<fieldset class="answer-item" id="quiz2-'+item.ID+'">';
            var content = "";
            for (var x = 0; item.CloneAnswers != null && x < item.CloneAnswers.length; x++) {
                var answer = item.CloneAnswers[x];
                content += content == "" ? answer.Content : " | " + answer.Content;
            }
            html += '<input id="inputQZ2-' + item.ID +'" data-type="QUIZ2" type="text" class="input-text answer-text" placeholder="' + content + '" onfocusout="AnswerQuestion(this)">';
            html += '</fieldset>';
            html += '</div></div>';
        }
        html += '</div>';
        return html;
    }
    var renderQUIZ3 = function (data) {
        var html = '<div class="part-box-header"> <h5 class="title">' + data.Title + '</h5></div>';
        html += '<div class="row">';
        html += '<div class="quiz-wrapper col-lg-8">' + renderMedia(data.Media);
        var answers = "";
        for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
            var item = data.Questions[i];
            var content = item.Content == null || item.Content == "null" || item.Content == void 0 ? "___" :item.Content;
            html += '<div class="quiz-item row" id="' + item.ID + '">';
            html += '<div class="quiz-pane col-6"><div class="pane-item"><div class="quiz-text">' + content + '</div></div></div>';
            html += '<div class="answer-pane col-6 ui-droppable" data-id="' + item.ID + '"><div class="pane-item placeholder">Thả câu trả lời tại đây</div></div>';
            html += '</div>';
            for (var x = 0; item.CloneAnswers != null && x < item.CloneAnswers.length; x++) {
                var answer = item.CloneAnswers[x];
                var media = renderMedia(answer.Media);
                if (media == "") {
                    answers += '<fieldset data-type="QUIZ3" class="answer-item ui-draggable ui-draggable-handle" id="' + answer.ID + '"><label class="answer-text">' + answer.Content + '</label></fieldset>';
                }
                else {
                    answers += '<fieldset data-type="QUIZ3" class="answer-item ui-draggable ui-draggable-handle" id="' + answer.ID + '">' + media + '</fieldset>';
                }
                
            }
        }
        html += '</div>';
        html += '<div class="answer-wrapper no-child col-lg-4 ui-droppable">' + answers + '</div>';
        html += '</div>';
        return html;
    }
    var renderESSAY = function (data) {
        var html = '<div class="part-box-header"> <h5 class="title">' + data.Title + '</h5>' + renderMedia(data.Media) + '</div>';
        html += '<div class="quiz-wrapper">';
        html += '<div class="quiz-item" id="' + data.ID + '"></div>';
        html += '<div class="answer-wrapper">';
        html += '<div class="answer-content"><textarea data-type="ESSAY" id="essay-' + data.ID +'" class="form-control" row="3" placeholder="Câu trả lời tự luận" onfocusout="AnswerQuestion(this)"></textarea></div>';
        html += '</div>';
        html += '</div>';
        return html;
    }

    /// bg
    var renderVOCAB_BG = function (data) {
        if (!checkExam()) return '<div class="justify-content-center pt-5 pb-5"><div class="btn btn-primary" onclick="BeginExam(this)" style="cursor: pointer;"> Bắt đầu làm bài</div></div>';
        var title = '<div class="part-box-header"><h5 class="title">' + data.Title + '</h5></div>';
        var html = title + '<div class="media-wrapper">';
        html += '<div class="media-holder ' + data.Type + '">Not Support</div>';
        html += '</div>';
        return html;
    }
    var renderQUIZ1_BG = function (data) {
        if (!checkExam()) return '<div class="justify-content-center pt-5 pb-5"><div class="btn btn-primary" onclick="BeginExam(this)" style="cursor: pointer;"> Bắt đầu làm bài</div></div>';
        var html = '<div class="part-box-header"> <h5 class="title">' + data.Title + '</h5>' + renderMedia(data.Media) + '</div>';
        html += '<div class="quiz-wrapper">';
        for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
            var item = data.Questions[i];
            html += '<div class="quiz-item" id="' + item.ID + '">';
            html += '<div class="quiz-box-header"><h5 class="title"> - ' + item.Content + ' (' + item.Point + 'đ)</h5>' + renderMedia(item.Media) + '</div>';
            html += '<div class="answer-wrapper">';
            for (var x = 0; item.CloneAnswers != null && x < item.CloneAnswers.length; x++) {
                var answer = item.CloneAnswers[x];
                html += '<fieldset class="answer-item" id="' + answer.ID + '">';
                html += '<div class="form-check">';
                html += '<input id="' + answer.ID + '" type="radio" data-type="QUIZ1" value="' + answer.Content + '" class="input-checkbox answer-checkbox form-check-input" onclick="AnswerQuestion(this)" name="rd_' + item.ID + '">';
                html += '<label class="answer-text form-check-label" for="' + answer.ID + '">' + answer.Content + '</label>';
                html += '</div>';
                html += renderMedia(answer.Media);
                html += '</fieldset>';
            }
            html += '</div></div>';
        }
        html += '</div>';
        return html;
    }
    var renderQUIZ2_BG = function (data) {
        if (!checkExam()) return '<div class="justify-content-center pt-5 pb-5"><div class="btn btn-primary" onclick="BeginExam(this)" style="cursor: pointer;"> Bắt đầu làm bài</div></div>';
        var html = '<div class="part-box-header"> <h5 class="title">' + data.Title + '</h5>' + renderMedia(data.Media) + '</div>';
        html += '<div class="quiz-wrapper">';
        for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
            var item = data.Questions[i];
            var itemContent = item.Content == null ? "Câu hỏi số " + (i + 1) + " : " : item.Content;
            html += '<div class="quiz-item" id="' + item.ID + '">';
            html += '<div class="quiz-box-header"><h5 class="title"> - ' + itemContent + ' (' + item.Point + 'đ)</h5>' + renderMedia(item.Media) + '</div>';
            html += '<div class="answer-wrapper">';
            html += '<fieldset class="answer-item" id="quiz2-' + item.ID + '">';
            var content = "";
            for (var x = 0; item.CloneAnswers != null && x < item.CloneAnswers.length; x++) {
                var answer = item.CloneAnswers[x];
                content += content == "" ? answer.Content : " | " + answer.Content;
            }
            html += '<input id="inputQZ2-' + item.ID + '" data-type="QUIZ2" type="text" class="input-text answer-text" placeholder="' + content + '" onfocusout="AnswerQuestion(this)">';
            html += '</fieldset>';
            html += '</div></div>';
        }
        html += '</div>';
        return html;
    }
    var renderQUIZ3_BG = function (data) {
        if (checkExam()) {
            var html = '<div class="part-box-header"> <h5 class="title">' + data.Title + '</h5></div>';
            html += '<div class="row">';
            html += '<div class="quiz-wrapper col-lg-8">' + renderMedia(data.Media);
            var answers = "";
            for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
                var item = data.Questions[i];
                var content = item.Content == null || item.Content == "null" || item.Content == void 0 ? "___" : item.Content;
                html += '<div class="quiz-item row" id="' + item.ID + '">';
                html += '<div class="quiz-pane col-6"><div class="pane-item"><div class="quiz-text">' + content + '</div></div></div>';
                html += '<div class="answer-pane col-6 ui-droppable" data-id="' + item.ID + '"><div class="pane-item placeholder">Thả câu trả lời tại đây</div></div>';
                html += '</div>';
                for (var x = 0; item.CloneAnswers != null && x < item.CloneAnswers.length; x++) {
                    var answer = item.CloneAnswers[x];
                    var media = renderMedia(answer.Media);
                    if (media == "") {
                        answers += '<fieldset data-type="QUIZ3" class="answer-item ui-draggable ui-draggable-handle" id="' + answer.ID + '"><label class="answer-text">' + answer.Content + '</label></fieldset>';
                    }
                    else {
                        answers += '<fieldset data-type="QUIZ3" class="answer-item ui-draggable ui-draggable-handle" id="' + answer.ID + '">' + media + '</fieldset>';
                    }

                }
            }
            html += '</div>';
            html += '<div class="answer-wrapper no-child col-lg-4 ui-droppable">' + answers + '</div>';
            html += '</div>';
            return html;
        }
        else {
            return '<div class="justify-content-center pt-5 pb-5"><div class="btn btn-primary" onclick="BeginExam(this)" style="cursor: pointer;"> Bắt đầu làm bài</div></div>';
        }
        
    }
    var renderESSAY_BG = function (data) {
        if (!checkExam()) return '<div class="justify-content-center pt-5 pb-5"><div class="btn btn-primary" onclick="BeginExam(this)" style="cursor: pointer;"> Bắt đầu làm bài</div></div>';
        var html = '<div class="part-box-header"> <h5 class="title">' + data.Title + '</h5>' + renderMedia(data.Media) + '</div>';
        html += '<div class="quiz-wrapper">';
        html += '<div class="quiz-item" id="' + data.ID + '"></div>';
        html += '<div class="answer-wrapper">';
        html += '<div class="answer-content"><textarea data-type="ESSAY" id="essay-' + data.ID + '" class="form-control" row="3" placeholder="Câu trả lời tự luận" onfocusout="AnswerQuestion(this)"></textarea></div>';
        html += '</div>';
        html += '</div>';
        return html;
    }
    ///end bg

    var renderMedia = function (data) {
        if (data == null || data == void 0 || data == "") return "";
        var arr = data.Extension.split('/');
        if (arr.includes("video")) {
            return '<div class="media-holder "><video controls=""><source src="' + data.Path + '" type="' + data.Extension + '">Your browser does not support the video tag</video></div>';
        }
        if (arr.includes("audio")) {
            return '<div class="media-holder "><audio controls=""><source src="' + data.Path + '" type="' + data.Extension + '">Your browser does not support the audio tag</audio></div>'
        }
        if (arr.includes("image")) {
            return '<div class="media-holder "><img src="' + data.Path + '" class="img-fluid lazy" title="' + data.Name + '"></div>';
        }
        return "";
    }
    var renderContent = function (data) {
        var tabList = '<div id="pills-tabContent" class="tab-content">';
        var html = '<div id="menu-left" class="col-md-2"><div class="lesson-tabs"><ul id="pills-tab" class="nav flex-column nav-pills" role="tablist" aria-orientation="vertical">';
        for (var i = 0; i < data.Parts.length; i++) {
            var item = data.Parts[i];
            var icon = arrIcon[item.Type];
            if (icon == void 0) icon = arrIcon["TEXT"];
            tabList += rednerLessonPart(item, i, data.TemplateType);
            var title = item.Title == void 0 || item.Title == null || item.Title == "null" ? "" : item.Title;
            if (i == 0) {
                html += '<li class="nav-item"><a id="pills-' + item.ID + '" class="nav-link active" data-toggle="pill" href="#pills-part-' + item.ID + '" role="tab" aria-controls="pills-' + item.ID + '" aria-selected="false">' + (i + 1).toString() + " - " + icon + '' + title + '</a></li>';
            }
            else {
                html += '<li class="nav-item"><a id="pills-' + item.ID + '" class="nav-link" data-toggle="pill" href="#pills-part-' + item.ID + '" role="tab" aria-controls="pills-' + item.ID + '" aria-selected="false">' + (i + 1).toString() + " - " + icon + '' + title + '</a></li>';
            }
        }
        html += '</ul></div></div>';
        tabList += '</div>';
        html += '<div class="col-md-10"><div class="lesson-body" id="' + data.ID + '">' + tabList + '</div></div>';
        return html;
    }
    var beginExam = function (_this) {
        var dataform = new FormData();
            dataform.append("LessonID", config.lesson_id);
            dataform.append("ClassID", config.class_id);
        Ajax(config.url.start, dataform, "POST", false)
            .then(function (res) {
                if (res != "Accept Deny") {
                    notification("success", "Bắt đầu làm bài", 3000);
                    var data = JSON.parse(res);
                    document.querySelector("input[name='ExamID']").value = data.ID;
                    // chưa viết hàm khác kịp (chỉ load những phần chưa load);
                    localStorage.setItem("CurrentExam", data.ID);
                    getCurrentData();
                    countdown();

                } else {
                    notification("error", res, 3000);
                }
            })
            .catch(function (err) {
                notification("error", err, 3000);
            })
    }
    var version = function () {
        return "1.0.0";
    }
    // ghép config default với config customer
    var groupConfig = function (options) {
        if (options == null || typeof (options) == "undefined") return config;
        for (var key in options)
            if (options.hasOwnProperty(key)) config[key] = options[key];
        return config;
    }
    /// Ajax.then var data = JSON.parse(res);
    var Ajax = function (url, data, method, async) {
        writeLog(url, data);
        var request = new XMLHttpRequest();
        return new Promise(function (resolve, reject) {
            // Setup our listener to process compeleted requests
            request.onreadystatechange = function () {
                if (request.readyState == 4) {
                    // Process the response
                    if (request.status >= 200 && request.status < 300) {
                        // If successful
                        resolve(request.response);
                    } else {
                        //0	UNSENT	Client has been created.open() not called yet.
                        //1	OPENED	open() has been called.
                        //2	HEADERS_RECEIVED	send() has been called, and headers and status are available.
                        //3	LOADING	Downloading; responseText holds partial data.
                        //4	DONE	The operation is complete.
                        var arrStatus = [
                            "UNSENT	Client has been created.open() not called yet.",
                            "OPENED	open() has been called.",
                            "HEADERS_RECEIVED	send() has been called, and headers and status are available.",
                            "LOADING	Downloading; responseText holds partial data.",
                            "DONE	The operation is complete."
                        ];
                        var msg = request.statusText == "" ? "Có lỗi xảy ra (" + arrStatus[request.status] + ")" : request.statusText;
                        notification("error", msg, 5000);
                        // If failed
                        reject({
                            status: request.status,
                            statusText: request.statusText
                        });
                    }
                } else {
                    return null;
                }
            };
            request.open(method || 'POST', url, async || true);
            // Send the request
            request.send(data);
        });
    }
    var AnswerQuestion = function (_this, _quizid, _answerID, _answerValue) {
        var type = _this.dataset.type;
        if (type == null || type == void 0 || type == "") {
            type = _this.querySelector("fieldset").dataset.type;
        }
        var quizID = "";
        var answerValue = "";
        var anwswerID = "";
        switch (type) {
            case "QUIZ1":
                quizID = _this.name.replace("rd_", "");
                answerValue = _this.value;
                anwswerID = _this.parentElement.parentElement.id;
                break;
            case "QUIZ2":
                quizID = _this.parentElement.id.replace("quiz2-","");
                answerValue = _this.value;
                anwswerID = _this.parentElement.id;
                break;
            case "QUIZ3":
                quizID = _this.dataset.id == void 0 ? _quizid : _this.dataset.id;
                var item = _this.querySelector('fieldset');
                var label = item.querySelector('label');
                if (label == null) {
                    label = item.querySelector("[src]").src;
                }
                else {
                    label = item.querySelector('label').innerHTML;
                }
                answerValue = _this.dataset.id == void 0 ? "" : label;
                anwswerID = item.id;
                break;
            case "ESSAY":
                quizID = _this.id.replace("essay-", "");
                answerValue = _this.value;
                anwswerID = _this.id;
                break;
            default:
                quizID = _quizid;
                answerValue = _answerID;
                anwswerID = _answerValue;
                break;
        }
        var dataform = new FormData();
            dataform.append("ExamID", document.querySelector("input[name='ExamID']").value);
            dataform.append("AnswerID", anwswerID);
            dataform.append("QuestionID", quizID);
            dataform.append("AnswerValue", answerValue);
        Ajax(config.url.chosen, dataform, "POST", false).then(function (res) {
        })
        .catch(function (err) {
            notification("error", err, 3000);
        });
        if (answerValue == "") {
            delAnswerForStudent(quizID);
        } else {
            saveAnswerForStudent(quizID, anwswerID, answerValue, type);
        }
    }
    var ExamComplete = function () {
        if (confirm("Có phải bạn muốn nộp bài")) {
            var exam = document.querySelector("input[name='ExamID']");
            var dataform = new FormData();
            dataform.append("ExamID", exam.value);
            Ajax(config.url.end, dataform, "POST", true)
                .then(function (res) {
                    notification("success", "Nộp bài thành công", 3000);
                    $(".lesson-container").empty();
                    $("#quizNavigator").addClass('d-none');
                    $("#finish").addClass('d-none');
                    $('#lessonContainer').removeClass('col-md-10');
                    $(".lesson-container").append('<div class="card show mb-4"></div>');
                    $(".card").append('<div class="card-body d-flex justify-content-center"><h3>Bạn đã hoàn thành bài thi</h3></div>');
                    $(".card").append('<div class="content card-body d-flex justify-content-center"></div>');
                    $(".content").append('<a href="#" onclick="goBack()"> Quay về trang bài học </a>');
                    localStorage.clear();
            })
            .catch(function (err) {
                console.log(err);
            });
           
        }
        
    }
    var delAnswerForStudent = function (quizID) {
        localStorage.removeItem(config.lesson_id + config.class_id + quizID);
        renderBoDem();
        document.getElementById("")
        startDragDrop();
        var xxx = document.getElementById("quizNav" + quizID);
        if (xxx != null) {
            xxx.classList.remove("completed");
        } 
    }
    var saveAnswerForStudent = function (quizID, answerID, answerValue, type) {
        if (quizID == void 0) return;
        var value = quizID + "~~" + answerID + "~~" + answerValue + "~~" + type;
        localStorage.setItem(config.lesson_id + config.class_id + quizID, value);
        renderBoDem();
        startDragDrop();
        var xxx = document.getElementById("quizNav" + quizID);
        if (xxx != null) {
            xxx.classList.add("completed");
        } 
    }
    var rendAgain = function (value) {
        var arr = value.split('~~');
        var quizID = arr[0];
        var answerID = arr[1];
        var answerValue = arr[2];
        var type = arr[3];
        switch (type) {
            case "QUIZ1":
                var answer = document.getElementById(answerID);
                answer.querySelector("input[type='radio']").setAttribute("checked", "");
                break;
            case "QUIZ2":
                var quiz = document.getElementById("inputQZ2-" + quizID);
                quiz.setAttribute("value", answerValue);
                break;
            case "QUIZ3":
                var quiz = document.getElementById(quizID);
                var content = quiz.querySelector('[data-id="' + quizID + '"]');
                var answer = document.getElementById(answerID);
                var abc = answer.outerHTML;
                answer.remove();
                content.innerHTML = abc;
                break;
            case "ESSAY":
                var quiz = document.getElementById("essay-" + quizID);
                quiz.innerHTML = answerValue;
                break;
            default: return;
        }
    }
    var startDragDrop = function () {
        $(".answer-item").draggable({
            cursor: "move",
            helper: 'clone',
            revert: "true",
            scroll: true,
            start: function (event, ui) {

            },
            stop: function (event, ui) {

            }
        });
        $('.answer-pane.ui-droppable').droppable({
            tolerance: "intersect",
            accept: ".answer-item",
            activeClass: "hasAnswer",
            hoverClass: "answerHover",
            drop: function (event, ui) {
                var item = $(ui.draggable);
                var anserID = item.attr("id");
                var questionID = $(this).attr("data-id");
                if (questionID == void 0) return;
                var answerValue = item.find(".media-holder  > img").attr("src");
                var prevHolder = $(ui.helper).parent();
                prevHolder.remove($(ui.helper));
                //$(prevHolder).find(".placeholder").show();
                $(this).html("");
                $(this).append(item);
                item.data("parent", questionID);
                AnswerQuestion(this, questionID, anserID, answerValue);
            }
        });
        $(".answer-wrapper.no-child").droppable({
            tolerance: "intersect",
            accept: ".answer-item",
            activeClass: "hasAnswer",
            hoverClass: "answerHover",
            drop: function (event, ui) {
                debugger;
                var prevHolder = $(ui.helper).parent();
                var quiz = prevHolder.data("id");
                if (quiz == void 0 || quiz == null || quiz == "") return;
                prevHolder.remove($(ui.helper));
                prevHolder.append($("<div>", { "class": "pane-item placeholder", "text": "Thả câu trả lời tại đây" }));
                //$(prevHolder).find(".placeholder").show();
                $(this).append($(ui.draggable));
                AnswerQuestion(this, quiz, typeof (void 0), "");
            }
        });
    }
    var renderBoDem = function () {
        var listQuiz = document.querySelectorAll(".quiz-item");
        var count = 0;
        var answerList = '';
        writeLog("renderBoDem", listQuiz);
        for (var i = 0; listQuiz != null && i < listQuiz.length; i++) {
            var item = listQuiz[i];
            var answer = localStorage.getItem(config.lesson_id + config.class_id + item.id);
            var compeleted = "";
            if (answer != null && answer != void 0 && answer != "") {
                count++;
                compeleted = "completed";
                rendAgain(answer);
            }
            answerList += '<button class="btn btn-outline-secondary rounded-quiz ' + compeleted + '" type="button" id="quizNav' + item.id + '" name="quizNav' + item.id + '">' + (i + 1) + '</button>';
        }
        var quiz_number_123 = document.getElementById("quiz-number_123");
        if (quiz_number_123 != null) {
            quiz_number_123.querySelector(".completed").innerHTML = count;
            quiz_number_123.querySelector(".total").innerHTML = listQuiz.length;
        } else {
            document.body.innerHTML += '<span id="quiz-number_123" style="top:30%" class="number quizNumber" onclick="openNav()"><span class="completed">' + count + '</span> / <span class="total">' + listQuiz.length + '</span></span>';
        }
        var html = '<div id="quizNavigator" class="overlay">';
        html += '<a href="javascript:void(0)" class="closebtn" onclick="closeNav()">×</a>';
        html += '<div class="overlay-content card-body">';
        html += '<div class="input-group mb-3 quiz-wrapper">';
        html += answerList;
        html += '<div style="display:none" id="btn-completed" class="d-flex justify-content-center pt-5 pb-5"><button class="btn btn-primary" onclick="ExamComplete()" data-original-title="" title=""> Hoàn thành</button></div>';
        html += '</div>'
        html += '</div>';
        html += '</div>';
        var quizNavigator = document.getElementById("quizNavigator");
        if (quizNavigator != null) {
            if (listQuiz != null && count >= listQuiz.length) {
                console.log(count, listQuiz.length);
                var btn = document.getElementById("btn-completed");
                if (btn != null) btn.style.display = "block";
            } else {
                var btn = document.getElementById("btn-completed");
                if (btn != null) btn.style.display = "none!important";
            }
        } else {
            document.body.innerHTML += html;
        }
        startDragDrop();
    }
    window.ExamStudent = {} || ExamStudent;
    ExamStudent.onReady = onReady;
    ExamStudent.Version = version;
    ExamStudent.Notification = notification;
    window.BeginExam = beginExam;
    window.AnswerQuestion = AnswerQuestion;
    window.ExamComplete = ExamComplete;
    return ExamStudent;
}());