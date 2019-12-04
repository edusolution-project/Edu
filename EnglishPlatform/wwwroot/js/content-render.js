
var Debug = true;
const mod = {
    PREVIEW = "preview",
    EDIT = "edit",
    EXAM = "exam",
    REVIEW = "review"
}

var writeLog = function (name, msg) {
    if (Debug) {
        console.log(name, msg);
    }
}

var Lesson = (function () {

    var data = null;

    var mixtype = false;

    var exam_timeout = null;

    var config = {
        id: "",
        mod: "",//preview, edit, exam, review
        url: {
            load: "", //all: load lesson data
            save: "", //edit: save lesson data
            save_part: "",//edit: save part,
            move_part: "",//edit: change part position,
            current: "", //exam: load current exam state
            start: "", //exam: start exam
            answer: "", //exam: set answer for question
            removeans: "", //exam: unset answer for question
            end: "" //exam: complete lesson
        },
        lesson_id: "",
        class_id: ""
    }

    // icon show on lesson parts list
    const arrIcon = {
        "TEXT": "<i class='fas fa-file-word'></i> ",
        "VIDEO": "<i class='fas fa-play-circle'></i> ",
        "AUDIO": "<i class='fas fa-music'></i> ",
        "IMG": "<i class='fas fa-file-image'></i> ",
        "DOC": "<i class='fas fa-file-word'></i> ",
        "VOCAB": "<i class='fas fa-file-word'></i> "
    };

    var onReady = function (yourConfig) {
        config = groupConfig(yourConfig);
        writeLog(this.__proto__.constructor.name, config);
        switch (config.TemplateType) {
            case mod.PREVIEW:
                break;
            case mod.EDIT:
                break;
            case mod.EXAM:
                renderExam();
                break;
            case mod.REVIEW:
                break;
        }
    }


    //function
    function countdown() {
        clearTimeout(lessontimeout);
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
                ExamComplete(true);
                return;
            }
        }
        var text = (minutes < 10 ? ("0" + minutes) : minutes) + ":" + (second < 10 ? ("0" + second) : second);
        $(".time-counter").text(text);
        lessontimeout = setTimeout(function () {
            countdown();
        }, 1000);
    }

    function stopcountdown() {
        clearTimeout(exam_timeout);
    }

    var resetLesson = function () {
        document.location = document.location.href.replace('#', '');
    }


    var renderExam = function () {
        var data = getCurrentExamState(); // get current exam state
        if (data == null || data == {} || data == "" || data == "{}" || data == void 0) {
            var formData = new FormData();
            formData.append("LessonID", config.lesson_id);
            formData.append("ClassID", config.class_id);
            Ajax(config.url.load, formData, "POST", true).then(function (res) {
                if (res != "Access Deny" && res != "null" && res != null && res.message != "res is not defined" && res != void 0) {
                    var resData = JSON.parse(res);
                    if (resData.Data.TemplateType != 1) {
                        var exam = resData.Exam;
                        if (exam == null || exam.Status == true) {
                            localStorage.removeItem("CurrentExam");
                            if (exam != null) resData.Exam.ID = null;
                            renderLesson(resData);
                        }
                        else {
                            localStorage.setItem("CurrentExam", resData.Exam.ID);
                            renderLesson(resData);
                            renderBoDem();
                            $("#counter").html(resData.Timer);
                            countdown();
                            $('.quizNumber').removeClass("d-none");
                        }
                    }

                    startDragDrop();
                }
            });
        } else {
            renderLesson(data);
            if (data.Data.TemplateType != 1) {
                renderBoDem();
                $("#counter").html(localStorage.getItem("Timer"));
                var currentExam = localStorage.getItem("CurrentExam");
                if (currentExam != null) {
                    countdown();
                    $('.quizNumber').removeClass("d-none");
                }
            }
            startDragDrop();
        }
    }



    var setCurrentData = function (data) {
        var enData = b64EncodeUnicode(JSON.stringify(data))
        localStorage.setItem(config.lesson_id + "_" + config.class_id, enData);
    }

    var setLocalData = function (key, data) {
        var enData = b64EncodeUnicode(JSON.stringify(data));
        localStorage.setItem(config.lesson_id + "_" + key, enData);
    }



    var getCurrentExamState = function () {

        //load lastest exam state from server
        var dataform = new FormData();
        dataform.append("ClassID", config.class_id);
        dataform.append("LessonID", config.lesson_id);
        Ajax(config.url.current, dataform, "POST", true).then(function (res) {
            //if no data is found => new attempt => clear all local storage
            if (res == null || res == "null") { localStorage.clear(); return null; }
            else { //
                var data = JSON.parse(res);
                document.querySelector("input[name='ExamID']").value = data.ID;
                //TODO: currentTimer here;
                var current = data.CurrentDoTime;
                var start = data.Created;
                var timer = moment(current) - moment(start);
                if (moment(timer).minutes() >= data.Timer)//Timeout
                {
                    localStorage.setItem("Timer", "00:00");
                    ExamComplete(true);
                }
                else {
                    var _sec = 59 - moment(timer).second();
                    var _minutes = data.Timer - moment(timer).minutes() - (_sec > 0 ? 1 : 0);
                    var timer = (_minutes >= 10 ? _minutes : "0" + _minutes) + ":" + (_sec >= 10 ? _sec : "0" + _sec)
                    setLocalData("Timer", timer);
                    setLocalData("CurrentExam", data.ID);
                    $("#counter").html(localStorage.getItem("Timer"));
                    countdown();
                }

            }
        });
        var data = localStorage.getItem(config.lesson_id + "_" + config.class_id);
        var enData = b64DecodeUnicode(data);
        if (enData == null || enData == {} || enData == "" || enData == "{}" || enData == void 0) {
            return null;
        } else {
            return JSON.parse(enData);
        }
    }

    var renderLesson = function (data) {
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
            lessonContainer.innerHTML = '<div class="lesson lesson-box"><div class="lesson-container"><div class="card shadow mb-4"><div class="card-header"><div class="row"><div class="lesson-header-title col-lg-4">' + data.Data.Title + '</div><div class="text-center col-lg-4">Thời gian làm bài <span id="counter" class="time-counter">' + counter + '</span></div></div></div><div id="body-exam" class="card-body"></div></div></div></div>';

            var bodyExam = lessonContainer.querySelector("#body-exam");
            var lastExam = data.Exam;
            var lastExamResult = "";
            var doButton = '<div class="btn btn-primary" onclick="BeginExam(this)" style="cursor: pointer;">Bắt đầu làm bài</div>';
            var tryleft = 0;

            var limit = data.Data.Limit;

            if (lastExam != null && lastExam.Status == true) {
                var lastdate = moment(lastExam.Updated).format("DD/MM/YYYY hh:mm:ss A");
                lastExamResult = "<div id='last-result' class='col-md-12 text-center'>Bài kiểm tra đã hoàn thành lúc " + lastdate + "</div>";

                var tried = lastExam.Number;
                tryleft = limit - tried;

                if (limit > 0) {
                    tryleft = limit - tried;
                    if (tryleft > 0) {
                        doButton = '<div class="btn btn-primary" onclick="BeginExam(this)" style="cursor: pointer;">Bạn còn <b>' + tryleft + '</b> lần làm lại. Thực hiện lại bài?</div>';
                    }
                    else
                        doButton = '<div class="btn btn-danger">Bạn đã hết lượt làm bài</div>';
                }
                else {
                    doButton = '<div class="btn btn-primary" onclick="BeginExam(this)" style="cursor: pointer;">Thực hiện lại bài</div>';
                }
            }
            var content = checkExam()
                ? renderContent(data.Data)
                : (lastExamResult + '<div class="text-center pt-5 pb-5 col-md-12">' + doButton + '</div>');
            bodyExam.innerHTML = '<div class="row">' + content + '</div>';
            if (checkExam) {
                document.querySelector("input[name='ExamID']").value = localStorage.getItem("CurrentExam");
            }
        }
        if ($('textarea[data-type=ESSAY]').length > 0) {
            $(document).ready(function () {
                CKEDITOR.replace($('textarea[data-type=ESSAY]').attr('id'));
            });
        }

    }
    /// tồn tại exam cũ thì return true;
    var checkExam = function () {
        //var exam = document.getElementById("ExamID");
        //if (exam == null || exam.value == void 0 || exam.value == null || exam.value == "") return false;
        return localStorage.getItem("CurrentExam") != null && localStorage.getItem("CurrentExam") != void 0 && localStorage.getItem("CurrentExam") != "" && localStorage.getItem("CurrentExam") != typeof (void 0);
    }
    //render bài tập
    var rednerLessonPart = function (data, index, type) {
        writeLog("rednerLessonPart", data);
        var active = "";
        if (index != void 0 && index == 0) {
            active = "show active";
        }

        var html = '<div id="pills-part-' + data.ID + '" class="tab-pane fade ' + active + '" role="tabpanel" aria-labelledby="pills-' + data.ID + '">';
        html += '<div class="part-box ' + data.Type + '" id="' + data.ID + '">';

        switch (data.Type) {
            case "QUIZ1": html += renderQUIZ1(data); //type == 2 ? renderQUIZ1(data) : renderQUIZ1_BG(data);
                break;
            case "QUIZ2": html += renderQUIZ2(data);//type == 2 ? renderQUIZ2(data) : renderQUIZ2_BG(data);
                break;
            case "QUIZ3": html += renderQUIZ3(data); //type == 2 ? renderQUIZ3(data) : renderQUIZ3_BG(data);
                break;
            case "ESSAY": html += renderESSAY(data); //type == 2 ? renderESSAY(data) : renderESSAY_BG(data);
                break;
            case "VOCAB": html += renderVOCAB(data); //type == 2 ? renderVOCAB(data) : renderVOCAB_BG(data);
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
        var title = '<div class="part-box-header col-md-2 d-inline-block sticky-top"><h5 class="title">' + data.Title + '</h5></div>';
        var html = title + '<div class="content-wrapper col-md-10 d-inline-block align-top">';
        html += '<div class="doc-content">' + data.Description + '</div>';
        html += '</div>';
        return html;
    }

    var renderVIDEO = function (data) {
        var title = '<div class="part-box-header col-md-2 d-inline-block sticky-top"><h5 class="title">' + data.Title + '</h5></div>';
        var html = title + '<div class="media-wrapper col-md-10 d-inline-block align-top">';
        html += '<div class="media-holder VIDEO"><video controls=""><source src="' + data.Media.Path + '" type="' + data.Media.Extension + '">Your browser does not support the video tag</video></div>';
        html += '</div>';
        return html;
    }

    var renderAUDIO = function (data) {
        var title = '<div class="part-box-header col-md-2 d-inline-block sticky-top"><h5 class="title">' + data.Title + '</h5></div>';
        var html = title + '<div class="media-wrapper col-md-10 d-inline-block align-top">';
        html += '<div class="media-holder ' + data.Type + '"><audio controls=""><source src="' + data.Media.Path + '" type="' + data.Media.Extension + '">Your browser does not support the audio tag</audio></div>';
        html += '</div>';
        return html;
    }

    var renderIMG = function (data) {
        var title = '<div class="part-box-header col-md-2 d-inline-block sticky-top"><h5 class="title">' + data.Title + '</h5></div>';
        var html = title + '<div class="media-wrapper col-md-10 d-inline-block align-top">';
        html += '<div class="media-holder ' + data.Type + '"><img src="' + data.Media.Path + '" title="' + data.Title + '" class="img-fluid lazy"></div>';
        html += '</div>';
        return html;
    }

    var renderDOC = function (data) {
        var title = '<div class="part-box-header col-md-2 d-inline-block sticky-top"><h5 class="title">' + data.Title + '</h5></div>';
        var html = title + '<div class="media-wrapper col-md-10 d-inline-block align-top">';
        html += '<div class="media-holder ' + data.Type + '"><embed src="' + data.Media.Path + '" style="width: 100%; height: 800px; border:1px solid"></div>';
        html += '</div>';
        return html;
    }

    var renderVOCAB = function (data) {
        var title = '<div class="part-box-header col-md-2 d-inline-block sticky-top"><h5 class="title">' + data.Title + '</h5></div>';
        var html = title + '<div class="media-wrapper col-md-10 d-inline-block align-top">';
        html += '<div class="media-holder ' + data.Type + '">Not Support</div>';
        html += '</div>';
        return html;
    }

    var renderQUIZ1 = function (data) {
        writeLog("renderQUIZ1", data);
        var toggleButton = '<button class="btn-toggle-width btn btn-info" onclick="togglePanelWidth(this)"><i class="fas fa-arrows-alt-h"></i></button>';
        var html = '<div class="part-box-header col-md-6 d-inline-block sticky-top"> <h5 class="title">' + data.Title + '</h5>' + renderMedia(data.Media) + toggleButton + '</div>';
        html += '<div class="quiz-wrapper col-md-6 d-inline-block align-top">';
        for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
            var item = data.Questions[i];
            html += '<div class="quiz-item" id="' + item.ID + '" data-part-id="' + item.ParentID + '">';
            html += '<div class="quiz-box-header"><h5 class="title">(' + item.Point + 'đ) ' + item.Content + '</h5>' + renderMedia(item.Media) + '</div>';
            html += '<div class="answer-wrapper">';
            for (var x = 0; item.CloneAnswers != null && x < item.CloneAnswers.length; x++) {
                var answer = item.CloneAnswers[x];
                html += '<fieldset class="answer-item d-inline mr-4" id="' + answer.ID + '">';
                html += '<div style="cursor: pointer; display:inline-block" class="form-check" data-part-id="' + data.ID + '" data-lesson-id="' + data.ParentID + '" data-question-id="' + item.ID + '" data-id="' + answer.ID + '" data-type="QUIZ1" data-value="' + answer.Content + '" onclick="AnswerQuestion(this)">';
                html += '<input id="' + answer.ID + '" type="radio" data-type="QUIZ1" value="' + answer.Content + '" class="input-checkbox answer-checkbox form-check-input" name="rd_' + item.ID + '">';
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
        writeLog("renderQUIZ2", data);
        var toggleButton = '<button class="btn-toggle-width btn btn-info" onclick="togglePanelWidth(this)"><i class="fas fa-arrows-alt-h"></i></button>';
        var html = '<div class="part-box-header col-md-6 d-inline-block sticky-top"> <h5 class="title">' + data.Title + '</h5>' + renderMedia(data.Media) + toggleButton + '</div>';
        html += '<div class="quiz-wrapper col-md-6 d-inline-block align-top">';
        for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
            var item = data.Questions[i];
            var itemContent = item.Content == null ? "Câu hỏi số " + (i + 1) + " : " : item.Content;
            html += '<div class="quiz-item" id="' + item.ID + '" data-part-id="' + item.ParentID + '">';
            html += '<div class="quiz-box-header"><h5 class="title">(' + item.Point + 'đ) ' + itemContent + '</h5>' + renderMedia(item.Media) + '</div>';
            html += '<div class="answer-wrapper">';
            html += '<fieldset class="answer-item" id="quiz2-' + item.ID + '">';
            var content = "";
            //for (var x = 0; item.CloneAnswers != null && x < item.CloneAnswers.length; x++) {
            //    var answer = item.CloneAnswers[x];
            //    content += content == "" ? answer.Content : " | " + answer.Content;
            //}
            html += '<input id="inputQZ2-' + item.ID + '" data-part-id="' + data.ID + '" data-lesson-id="' + data.ParentID + '" data-question-id="' + item.ID + '" data-id="" data-type="QUIZ2" type="text" class="input-text answer-text form-control" placeholder="' + content + '" onfocusout="AnswerQuestion(this)" autocomplete="off">';
            html += '</fieldset>';
            html += '</div></div>';
        }
        html += '</div>';
        return html;
    }

    var renderQUIZ3 = function (data) {
        writeLog("renderQUIZ3", data);
        var toggleButton = '<button class="btn-toggle-width btn btn-info" onclick="togglePanelWidth(this)"><i class="fas fa-arrows-alt-h"></i></button>';
        var html = '<div class="part-box-header col-md-6 d-inline-block sticky-top"> <h5 class="title">' + data.Title + '</h5>' + renderMedia(data.Media) + toggleButton + '</div>';
        html += '<div class="col-md-6 d-inline-block align-top">';
        html += '<div class="quiz-wrapper col-lg-8 d-inline-block align-top">';
        var answers = "";
        for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
            var item = data.Questions[i];
            var content = item.Content == null || item.Content == "null" || item.Content == void 0 ? "___" : item.Content;
            html += '<div class="quiz-item row" id="' + item.ID + '" data-part-id="' + item.ParentID + '">';
            html += '<div class="quiz-pane col-6 align-top"><div class="pane-item"><div class="quiz-text">' + content + '</div></div></div>';
            html += '<div class="answer-pane col-6 ui-droppable align-top" data-part-id="' + data.ID + '" data-lesson-id="' + data.ParentID + '" data-question-id="' + item.ID + '" data-type="' + data.Type + '"><div class="pane-item placeholder">Thả câu trả lời tại đây</div></div>';
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
        html += '<div class="answer-wrapper no-child col-lg-4 ui-droppable d-inline-block sticky-top" data-part-id="' + data.ID + '">' + answers + '</div>';
        html += '</div>';
        return html;
    }

    var renderESSAY = function (data) {
        writeLog("renderESSAY", data);
        var html = '<div class="part-box-header">' + (data.Title == null ? '' : ('<h5 class="title">' + data.Title + '</h5>')) + '<div class="description">' + data.Description + '</div>' + renderMedia(data.Media) + '</div>';
        html += '<div class="quiz-wrapper">';
        html += '<div class="quiz-item" id="' + data.ID + '" data-part-id="' + data.ID + '"></div>';
        html += '<div class="answer-wrapper">';
        html += '<div class="answer-content"><textarea data-part-id="' + data.ID + '" data-lesson-id="' + data.ParentID + '" data-type="ESSAY" id="essay-' + data.ID + '" class="form-control" row="3" placeholder="Câu trả lời" onfocusout="AnswerQuestion(this)"></textarea></div>';
        html += '</div>';
        html += '</div>';
        return html;
    }

    /// bg
    var renderVOCAB_BG = function (data) {
        //if (!checkExam()) return '<div class="justify-content-center pt-5 pb-5"><div class="btn btn-primary" onclick="BeginExam(this)" style="cursor: pointer;"> Bắt đầu làm bài</div></div>';
        var title = '<div class="part-box-header"><h5 class="title">' + data.Title + '</h5></div>';
        var html = title + '<div class="media-wrapper">';
        html += '<div class="media-holder ' + data.Type + '">Not Support</div>';
        html += '</div>';
        return html;
    }

    var renderQUIZ1_BG = function (data) {
        //if (!checkExam()) return '<div class="justify-content-center pt-5 pb-5"><div class="btn btn-primary" onclick="BeginExam(this)" style="cursor: pointer;"> Bắt đầu làm bài</div></div>';
        var html = '<div class="part-box-header"> <h5 class="title">' + data.Title + '</h5>' + renderMedia(data.Media) + '</div>';
        html += '<div class="quiz-wrapper">';
        for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
            var item = data.Questions[i];
            html += '<div class="quiz-item" id="' + item.ID + '" data-part-id="' + item.ParentID + '">';
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
        //if (!checkExam()) return '<div class="justify-content-center pt-5 pb-5"><div class="btn btn-primary" onclick="BeginExam(this)" style="cursor: pointer;"> Bắt đầu làm bài</div></div>';
        var html = '<div class="part-box-header"> <h5 class="title">' + data.Title + '</h5>' + renderMedia(data.Media) + '</div>';
        html += '<div class="quiz-wrapper">';
        for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
            var item = data.Questions[i];
            var itemContent = item.Content == null ? "Câu hỏi số " + (i + 1) + " : " : item.Content;
            html += '<div class="quiz-item" id="' + item.ID + '" data-part-id="' + item.ParentID + '">';
            html += '<div class="quiz-box-header"><h5 class="title"> - ' + itemContent + ' (' + item.Point + 'đ)</h5>' + renderMedia(item.Media) + '</div>';
            html += '<div class="answer-wrapper">';
            html += '<fieldset class="answer-item" id="quiz2-' + item.ID + '">';
            var content = "";
            for (var x = 0; item.CloneAnswers != null && x < item.CloneAnswers.length; x++) {
                var answer = item.CloneAnswers[x];
                content += content == "" ? answer.Content : " | " + answer.Content;
            }
            html += '<input id="inputQZ2-' + item.ID + '" data-type="QUIZ2" type="text" class="input-text answer-text form-control" placeholder="' + content + '" onfocusout="AnswerQuestion(this)" autocomplete="off">';
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
                html += '<div class="quiz-item row" id="' + item.ID + '" data-part-id="' + item.ParentID + '">';
                html += '<div class="quiz-pane col-6"><div class="pane-item"><div class="quiz-text">' + content + '</div></div></div>';
                html += '<div class="answer-pane col-6 ui-droppable" data-id="' + item.ID + '"><div class="pane-item placeholder">Thả câu trả lời tại đây</div></div>';
                html += '</div>';
                for (var x = 0; item.CloneAnswers != null && x < item.CloneAnswers.length; x++) {
                    var answer = item.CloneAnswers[x];
                    var media = renderMedia(answer.Media);
                    if (media == "") {
                        answers += '<fieldset data-type="QUIZ3" class="answer-item" id="' + answer.ID + '"><label class="answer-text">' + answer.Content + '</label></fieldset>';
                    }
                    else {
                        answers += '<fieldset data-type="QUIZ3" class="answer-item" id="' + answer.ID + '">' + media + '</fieldset>';
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
        html += '<div class="quiz-item" id="' + data.ID + '" data-part-id="' + data.ID + '"></div>';
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
        var nav = '<div id="nav-menu" class="col-md-12 mb-3 pb-3 bd-bottom">';
        var nav_bottom = '<div id="nav-menu" class="col-md-12 mt-3 pt-3 bd-top">';
        nav += '<div class="col-md-1 text-left d-inline-block"><button class="prevtab btn btn-success" data-toggle="tooltip" title="Phần trước" onclick="tab_goback()"><i class="fas fa-arrow-left"></i></button></div>'; //left button
        nav_bottom += '<div class="col-md-4 text-left d-inline-block"><button class="prevtab btn btn-success" onclick="tab_goback()"><i class="fas fa-arrow-left"></i></button></div>';
        nav += '<div class="lesson-tabs col-md-10 d-inline-block"><ul id="pills-tab" class="nav nav-pills compact" onclick="toggle_tab_compact()">';
        for (var i = 0; i < data.Parts.length; i++) {
            var item = data.Parts[i];
            var icon = arrIcon[item.Type];
            if (icon == void 0) icon = arrIcon["TEXT"];
            tabList += rednerLessonPart(item, i, data.TemplateType);
            var title = item.Title == void 0 || item.Title == null || item.Title == "null" ? "" : item.Title;
            if (i == 0) {
                nav += '<li class="nav-item"><a id="pills-' + item.ID + '" class="nav-link active" data-toggle="pill" data-target="#pills-part-' + item.ID + '" onclick="tab_set_active(this)">' + (i + 1).toString() + " - " + icon + '' + title + '</a></li>';
            }
            else {
                nav += '<li class="nav-item"><a id="pills-' + item.ID + '" class="nav-link" data-toggle="pill" data-target="#pills-part-' + item.ID + '" onclick="tab_set_active(this)">' + (i + 1).toString() + " - " + icon + '' + title + '</a></li>';
            }
        }
        nav += '</ul></div>';
        nav += '<div class="col-md-1 text-right d-inline-block"><button class="nexttab btn btn-success" data-toggle="tooltip" title="Phần sau" onclick="tab_gonext()"><i class="fas fa-arrow-right"></i></button></div>'; //right button

        nav_bottom += '<div class="col-md-4 text-center d-inline-block"><button class="btn btn-success pl-5 pr-5" onclick="ExamComplete()">Nộp bài</button></div>';
        nav_bottom += '<div class="col-md-4 text-right d-inline-block"><button class="nexttab btn btn-success" onclick="tab_gonext()"><i class="fas fa-arrow-right"></i></button></div>';
        nav_bottom += '</div>';
        nav += '</div>';
        tabList += '</div>';
        html = //nav + 
            '<div class="col-md-12"><div class="lesson-body" id="' + data.ID + '">' + tabList + '</div></div>' + nav_bottom;
        return html;
    }

    var beginExam = function (_this) {
        var dataform = new FormData();
        dataform.append("LessonID", config.lesson_id);
        dataform.append("ClassID", config.class_id);
        Ajax(config.url.start, dataform, "POST", false)
            .then(function (res) {
                var data = JSON.parse(res);
                if (data.Error == null) {
                    notification("success", "Bắt đầu làm bài", 3000);
                    document.querySelector("input[name='ExamID']").value = data.Data.ID;
                    // chưa viết hàm khác kịp (chỉ load những phần chưa load);????
                    localStorage.setItem("CurrentExam", data.Data.ID);
                    renderExam();
                    renderBoDem();
                    countdown();
                    $('.quizNumber').removeClass("d-none");
                } else {
                    notification("error", data.Error, 3000);
                }
            })
            .catch(function (err) {
                notification("error", err, 3000);
            })
    }

    var goBack = function () {
        document.location = "/student/Course/Modules/" + config.class_id;
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






    var AnswerQuestion = function (_this) {
        // dataset trên item
        var dataset = _this.dataset;
        //loại câu hỏi
        var type = dataset.type;

        //lessonPartID
        var partID = "";
        //questionID
        var questionId = "";
        // câu trả lời
        var answerID = "";
        //nội dung câu trả lời
        var value = "";

        switch (type) {
            case "QUIZ1":
                partID = dataset.partId;
                questionId = dataset.questionId;
                answerID = dataset.id;
                value = dataset.value;
                break;
            case "QUIZ2":
                partID = dataset.partId;
                questionId = dataset.questionId;
                answerID = dataset.id;
                // value là data động tự điền
                value = _this.value;
                break;
            case "QUIZ3":
                partID = dataset.partId;
                questionId = dataset.questionId;
                var item = _this.querySelector('fieldset');
                var label = item.querySelector('label');
                if (label == null) {
                    label = item.querySelector("[src]").src;
                }
                else {
                    label = item.querySelector('label').innerHTML;
                }
                value = item == void 0 ? "" : label;
                answerID = item.id;
                break;
            case "ESSAY":
                partID = dataset.partId;
                value = _this.value;
                answerID = _this.id;
                break;
            default:
                break;
        }
        var dataform = new FormData();
        if (type != "ESSAY") {
            dataform.append("ExamID", document.querySelector("input[name='ExamID']").value);
            dataform.append("LessonPartID", partID);
            dataform.append("AnswerID", answerID);
            dataform.append("QuestionID", questionId);
            dataform.append("AnswerValue", value);
        } else {
            dataform.append("ExamID", document.querySelector("input[name='ExamID']").value);
            dataform.append("LessonPartID", partID);
            dataform.append("AnswerValue", value);
        }
        Ajax(config.url.answer, dataform, "POST", false).then(function (res) {
        })
            .catch(function (err) {
                notification("error", err, 3000);
            });
        if (value == "") {
            delAnswerForStudent(questionId);
        } else {
            saveAnswerForStudent(questionId, answerID, value, type);
        }
    }

    var ExamComplete = function (isOvertime) {
        if (isOvertime || confirm("Có phải bạn muốn nộp bài")) {
            var exam = document.querySelector("input[name='ExamID']");
            var dataform = new FormData();
            dataform.append("ExamID", exam.value);
            Ajax(config.url.end, dataform, "POST", true)
                .then(function (res) {
                    stopcountdown();
                    var data = JSON.parse(res);
                    notification("success", "Nộp bài thành công", 3000);
                    //$(".lesson-container").empty();
                    $('#body-exam').hide();
                    $("#quizNavigator").addClass('d-none');
                    $("#finish").remove();
                    $(".quizNumber").remove();
                    $('#lessonContainer').removeClass('col-md-10');
                    $(".lesson-container").append('<div class="card show mb-4"></div>');
                    $(".card:first").append('<div class="card-body d-flex justify-content-center"><h3>Bạn đã hoàn thành bài thi</h3></div>');
                    $(".card:first").append('<div class="card-body d-flex justify-content-center"><h3>Điểm trắc nghiệm: ' + (data.maxPoint == 0 ? 0 : (data.point * 100 / data.maxPoint)).toFixed(0) + '%</h3></div>');
                    $(".card:first").append('<div class="content card-body d-flex justify-content-center"></div>');

                    $(".content").append('<a href="#" onclick="window.Redo()" class="mr-2"> Làm lại bài </a>');
                    $(".content").append('<a href="#" onclick="" class="mr-2"> Kiểm tra đáp án </a>');
                    $(".content").append('<a href="#" onclick="window.GoBack()" class="mr-2"> Quay về trang bài học </a>');

                })
                .catch(function (err) {
                    console.log(err);
                });
        }
    }

    var delAnswerForStudent = function (quizID) {
        localStorage.removeItem(config.lesson_id + config.class_id + quizID);
        var dataform = new FormData();
        dataform.append("ExamID", document.querySelector("input[name='ExamID']").value);
        dataform.append("QuestionID", quizID);
        Ajax(config.url.removeans, dataform, "POST", false)
            .then(function (res) {
                renderBoDem();
                var xxx = document.getElementById("quizNav" + quizID);
                if (xxx != null) {
                    xxx.classList.remove("completed");
                }
            })
            .catch(function (err) {
                notification("error", err, 3000);
            });
    }

    var delAnswerForStudentNoRender = function (quizID) {
        localStorage.removeItem(config.lesson_id + config.class_id + quizID);
        var dataform = new FormData();
        dataform.append("ExamID", document.querySelector("input[name='ExamID']").value);
        dataform.append("QuestionID", quizID);
        Ajax(config.url.removeans, dataform, "POST", false)
            .then(function (res) {
                renderBoDem();
                var xxx = document.getElementById("quizNav" + quizID);
                if (xxx != null) {
                    xxx.classList.remove("completed");
                }
            })
            .catch(function (err) {
                notification("error", err, 3000);
            });
    }

    var saveAnswerForStudent = function (quizID, answerID, answerValue, type) {
        if (quizID == void 0) return;
        var value = quizID + "~~" + answerID + "~~" + answerValue + "~~" + type;
        localStorage.setItem(config.lesson_id + config.class_id + quizID, value);
        renderBoDem();
        //startDragDrop();
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
                var content = quiz.querySelector('[data-question-id="' + quizID + '"]');
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
                var prevHolder = $(ui.helper).parent();
                prevHolder.remove($(ui.helper));
                var quiz = prevHolder.data("questionId");

                if ($(prevHolder).attr("data-question-id") == $(this).attr("data-question-id"))
                    return false;

                var part = $(prevHolder).attr("data-part-id");

                if ($(this).find(".answer-item").length > 0) {//remove all answer to box
                    $("#" + part + " .answer-wrapper").append($(this).find(".answer-item"));
                }

                $(this).append(item);

                if ($(prevHolder).find(".answer-item").length == 1) {
                    if ($(prevHolder).find(".placeholder").length > 0)
                        $(prevHolder).find(".placeholder").show();
                    else
                        prevHolder.append($("<div>", { "class": "pane-item placeholder", "text": "Thả câu trả lời tại đây" }));
                }

                var quiz = prevHolder.data("questionId");
                if (quiz != null) { delAnswerForStudentNoRender(quiz); }
                $(this).find(".placeholder").hide();
                //item.data("parent", questionID);
                AnswerQuestion(this);
            }
        });
        $(".answer-wrapper.no-child").droppable({
            tolerance: "intersect",
            accept: ".answer-item",
            activeClass: "hasAnswer",
            hoverClass: "answerHover",
            drop: function (event, ui) {
                $(this).find(".placeholder").hide();
                var prevHolder = $(ui.helper).parent();
                var quiz = prevHolder.data("questionId");
                if (quiz != null) { delAnswerForStudent(quiz); }
                prevHolder.remove($(ui.helper));

                $(this).append($(ui.draggable));

                if ($(prevHolder).find(".answer-item").length == 1) {
                    if ($(prevHolder).find(".placeholder").length > 0)
                        $(prevHolder).find(".placeholder").show();
                    else
                        prevHolder.append($("<div>", { "class": "pane-item placeholder", "text": "Thả câu trả lời tại đây" }));
                }

                //AnswerQuestion(this);
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
            answerList += '<button class="btn btn-outline-secondary rounded-quiz ' + compeleted + '" type="button" id="quizNav' + item.id + '" name="quizNav' + item.id + '" onclick="goNav(\'' + item.id + '\')">' + (i + 1) + '</button>';
        }
        var quiz_number_counter = $("#quiz_number_counter");
        if (quiz_number_counter.length > 0) {
            quiz_number_counter.find(".completed").text(count);
            quiz_number_counter.find(".total").text(listQuiz.length);
        } else {
            $("#lessonContainer").append($("<span>", {
                id: "quiz_number_counter",
                style: "top: 30%",
                class: "number quizNumber d-none",
                onclick: "openNav()"
            })
                .append($("<span>", {
                    class: "completed",
                    text: count
                })).append(" / ")
                .append($("<span>", {
                    class: "total",
                    text: listQuiz.length
                })));
        }
        var html = '<div id="quizNavigator" class="overlay">';
        html += '<a href="javascript:void(0)" class="closebtn" onclick="closeNav()">×</a>';
        html += '<div class="overlay-content card-body">';
        html += '<div class="input-group mb-3 quiz-wrapper">';
        html += answerList;
        html += '<div style="display:none" id="btn-completed" class="d-flex justify-content-center pt-5 pb-5"><button class="btn btn-primary" onclick="ExamComplete()" data-original-title="" title=""> Nộp bài </button></div>';
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
    window.Redo = resetLesson;
    window.GoBack = goBack;

    return ExamStudent;
}());

