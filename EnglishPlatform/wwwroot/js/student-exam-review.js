
var Debug = true;

var writeLog = function (name, msg) {
    if (Debug) {
        console.log(name, msg);
    }
}

var lessontimeout = null;

var ExamReview = (function () {

    var _totalPart = 0;
    var _type = 0;

    var prevPart = function () {
        var panes = $('.tab-pane');
        var index = panes.index($('.tab-pane.active'));
        if (index > 0) {
            goPartInx(index - 1);
        }
    }

    var nextPart = function () {
        var panes = $('.tab-pane');
        var index = panes.index($('.tab-pane.active'));
        if (index < panes.length - 1) {
            goPartInx(index + 1);
        }
    }

    var goQuiz = function (quizid) {
        var _quiz = $('#' + quizid);
        var _partid = _quiz.attr('data-part-id');
        var _part = $('#pills-part-' + _partid);
        if (_part.hasClass('active')) {
            return false;
        }
        var panes = $('.tab-pane');
        var index = panes.index(_part);
        goPartInx(index);
    }

    var goPartInx = function (idx) {
        stopAllMedia();
        var panes = $('.tab-pane');
        $('.tab-pane.show.active').removeClass('show active');
        $(panes[idx]).addClass('show active');
        $('.prevtab').prop('disabled', idx == 0);
        $('.nexttab').prop('disabled', idx == _totalPart - 1);
    }

    var toggleNav = function (obj) {
        $('#lessonSummary').toggleClass("expand");
        $(obj).toggleClass('btn-warning');
    }

    var toggleExplain = function (obj) {
        $(obj).toggleClass('btn-warning');
        $('.explaination').toggleClass("d-none");
    }

    var config = {
        id: "exam-review",
        url: {
            exam: "",
            list: ""
        },
        lesson: "",
        exam: "",
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

    var onReady = function (yourConfig) {
        config = groupConfig(yourConfig);
        //writeLog(this.__proto__.constructor.name, config);
        renderExam();
        renderQuizCounter();
        renderResult();

        $('.tab-pane .part-column').addClass('scrollbar-outer').scrollbar();
    }

    var renderExam = function () {
        //writeLog("renderLesson", data);
        var data = config.lesson;
        //console.log(data);
        var container = document.getElementById(config.id);
        if (container == null) throw "Element id " + config.id + " not exist - create now";

        var lessonContainer = $('<div>', { id: "lessonContainer", class: "h-100" });
        $(container).empty().append(lessonContainer);
        var bodyExam = $('<div>', { id: "body-exam", class: "card-body p-0" })
        lessonContainer.append(bodyExam);
        _type = data.TemplateType;

        var content = renderContent(data);

        bodyExam.append(content);

        if ($('textarea[data-type=ESSAY]').length > 0) {
            $(document).ready(function () {
                $('textarea[data-type=ESSAY]').each(function () {
                    CKEDITOR.replace($(this).attr('id'));
                });
            });
        }
    }

    var durationFormat = function (_duration) {
        var result = "";
        if (_duration.asHours() > 1)
            result += _duration.asHours().toFixed(0) + "h ";
        if (_duration.asMinutes() > 0)
            result += _duration.minutes().toFixed(0) + "m ";
        result += _duration.seconds() + "s";
        return result;
    }

    var renderResult = function () {
        var data = config.exam;

        var lastpoint = (data.MaxPoint > 0 ? (data.Point * 100 / data.MaxPoint) : 0);
        var completetime = moment(data.Updated);//.format("DD/MM/YYYY hh:mm:ss A");
        var starttime = moment(data.Created);
        var duration = moment.duration(completetime.diff(starttime));

        lastExamResult =
            $("<div>", { id: "last-result", class: "text-center pt-1 text-white" })
                .append("Work time: " + durationFormat(duration) + " - Score: " + lastpoint.toFixed(0) + "%");

        $('#lessonSummary').prepend(lastExamResult);
        $('#quiz_number_counter .completed').text(data.QuestionsDone);

        var details = data.Details;
        if (details != null && details.length > 0) {
            for (var i = 0; i < details.length; i++) {
                var detail = details[i];
                //console.log(detail);
                var quizType = $('#' + detail.QuestionID).attr('data-quiz-type');
                $('#quizNav' + detail.QuestionID).addClass("bg-" + (detail.Point > 0 ? "success" : "danger"));
                renderAnswer(detail, quizType);
            }
        }
    }

    var renderLessonPart = function (data, index, type) {
        //writeLog("renderLessonPart", data);
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

    var renderAnswer = function (data, type) {
        var quizId = data.QuestionID;
        if (data.AnswerID != null && data.AnswerID != "") {
            var _answer = $('#' + data.AnswerID).clone().removeClass("d-none");
            if (data.AnswerID != data.RealAnswerID)//wrong answer
                switch (type) {
                    case "QUIZ3":
                        _answer.addClass("bg-danger");
                        break;
                }
            $('#' + quizId + ' .student-answer').append(_answer);
        }
        else { //"QUIZ2"
            if (data.Point > 0)
                $('#' + quizId + ' .student-answer').append(" <span class='text-success'>" + data.AnswerValue + "</span>");
            else
                $('#' + quizId + ' .student-answer').append(" <span class='text-danger'><del>" + data.AnswerValue + "</del><span>");
        }

    }

    var renderTEXT = function (data) {
        var title = "";
        if (data.Title != null) {
            var title = '<div class="part-box-header pl-3 pr-3"><h5 class="title">' + data.Title + '</h5></div>';
        }
        var html = "<div class='part-column'>" + title + '<div class="content-wrapper p-3">';
        html += '<div class="doc-content">' + data.Description + '</div>';
        html += '</div></div>';
        return html;
    }

    var renderVIDEO = function (data) {
        var title = '<div class="part-box-header col-md-2 d-inline-block part-column"><h5 class="title">' + data.Title + '</h5></div>';
        var html = title + '<div class="media-wrapper col-md-10 d-inline-block align-top">';
        html += '<div class="media-holder VIDEO"><video controls=""><source src="' + data.Media.Path + '" type="' + data.Media.Extension + '">Your browser does not support the video tag</video></div>';
        html += '</div>';
        return html;
    }

    var renderAUDIO = function (data) {
        var title = '<div class="part-box-header col-md-2 d-inline-block part-column"><h5 class="title">' + data.Title + '</h5></div>';
        var html = title + '<div class="media-wrapper col-md-10 d-inline-block align-top">';
        html += '<div class="media-holder ' + data.Type + '"><audio controls=""><source src="' + data.Media.Path + '" type="' + data.Media.Extension + '">Your browser does not support the audio tag</audio></div>';
        html += '</div>';
        return html;
    }

    var renderIMG = function (data) {
        var title = '<div class="part-box-header col-md-2 d-inline-block part-column"><h5 class="title">' + data.Title + '</h5></div>';
        var html = title + '<div class="media-wrapper col-md-10 d-inline-block align-top">';
        html += '<div class="media-holder ' + data.Type + '"><img src="' + data.Media.Path + '" title="' + data.Title + '" class="img-fluid lazy"></div>';
        html += '</div>';
        return html;
    }

    var renderDOC = function (data) {
        var title = '<div class="part-box-header col-md-2 d-inline-block part-column"><h5 class="title">' + data.Title + '</h5></div>';
        var html = title + '<div class="media-wrapper col-md-10 d-inline-block align-top">';
        html += '<div class="media-holder ' + data.Type + '"><embed src="' + data.Media.Path + '" style="width: 100%; height: 800px; border:1px solid"></div>';
        html += '</div>';
        return html;
    }

    var renderVOCAB = function (data) {
        var title = '<div class="part-box-header col-md-2 d-inline-block part-column"><h5 class="title">' + data.Title + '</h5></div>';
        var html = title + '<div class="media-wrapper col-md-10 d-inline-block align-top">';
        html += '<div class="media-holder ' + data.Type + '">Not Support</div>';
        html += '</div>';
        return html;
    }

    var renderQUIZ1 = function (data) {
        //writeLog("renderQUIZ1", data);
        console.log(data);
        var toggleButton = '<button class="btn-toggle-width btn btn-success" onclick="togglePanelWidth(this)"><i class="fas fa-arrows-alt-h"></i></button>';
        var html = '<div class="col-md-6 d-inline-block h-100" style="border-right: dashed 1px #CCC"><div class="part-box-header part-column">';
        if (data.Title != null)
            html += '<h5 class="title">' + data.Title + '</h5 >';
        if (data.Description != null)
            html += '<div class="description">' + data.Description + '</div>';
        html += renderMedia(data.Media) + toggleButton + '</div></div>';
        html += '<div class="col-md-6 d-inline-block align-top h-100"><div class="quiz-wrapper part-column">';
        for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
            var item = data.Questions[i];
            html += '<div class="quiz-item" id="' + item.ID + '" data-part-id="' + item.ParentID + '" data-quiz-type="QUIZ1">';
            html += '<div class="quiz-box-header"><h5 class="title">' + item.Content + '</h5>' + renderMedia(item.Media) + '</div>';
            html += '<div class="answer-wrapper">';
            html += '<div class="student-answer">';
            html += '<fieldset class="answer-item d-inline mr-3">';
            html += '<div style="cursor: pointer; display:inline-block" class="form-check">';
            html += '<label class="answer-text form-check-label"><i>Your Answer: </i></label>';
            html += '</div>';
            html += '</fieldset>';
            html += '</div>';
            html += '<div>';
            html += '<fieldset class="answer-item d-inline mr-3">';
            html += '<div style="cursor: pointer; display:inline-block" class="form-check">';
            html += '<label class="answer-text form-check-label"><i>Correct Answer: </i></label>';
            html += '</div>';
            html += '</fieldset>';
            for (var x = 0; item.CloneAnswers != null && x < item.CloneAnswers.length; x++) {
                var answer = item.CloneAnswers[x];
                //if(!answer.IsCorrect) continue;
                html += '<fieldset class="answer-item d-inline mr-3" id="' + answer.ID + '">';
                html += '<div style="cursor: pointer; display:inline-block" class="form-check" data-part-id="' + data.ID + '" data-lesson-id="' + data.ParentID + '" data-question-id="' + item.ID + '" data-id="' + answer.ID + '" data-type="QUIZ1" data-value="' + answer.Content + '" onclick="AnswerQuestion(this)">';
                if (answer.IsCorrect)
                    html += '<label class="answer-text form-check-label text-success" for="' + answer.ID + '">' + answer.Content + '</label>';
                else
                    html += '<label class="answer-text form-check-label text-danger" for="' + answer.ID + '"><del>' + answer.Content + '</del></label>';
                html += '</div>';
                html += renderMedia(answer.Media);
                html += '</fieldset>';
            }
            html += '</div>';
            var description = "";
            if (item.Description != null)
                description = item.Description.replace(/\n/g, '<br/>');
            html += '<div class="explaination d-none text-info p-3"><i>' + description + '</i></div>';
            html += '</div></div>';
        }
        html += '</div></div>';
        return html;
    }

    var renderQUIZ2 = function (data) {
        //writeLog("renderQUIZ2", data);
        var toggleButton = '<button class="btn-toggle-width btn btn-success" onclick="togglePanelWidth(this)"><i class="fas fa-arrows-alt-h"></i></button>';
        var html = '<div class="col-md-6 d-inline-block h-100" style="border-right: dashed 1px #CCC"><div class="part-box-header part-column">';
        if (data.Title != null)
            html += '<h5 class="title">' + data.Title + '</h5 >';
        if (data.Description != null)
            html += '<div class="description">' + data.Description + '</div>';
        html += renderMedia(data.Media) + toggleButton + '</div></div>';
        html += '<div class="col-md-6 d-inline-block align-top h-100"><div class="quiz-wrapper part-column">';
        for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
            var item = data.Questions[i];
            var itemContent = item.Content == null ? "Quiz " + (i + 1) + " : " : item.Content;
            html += '<div class="quiz-item" id="' + item.ID + '" data-part-id="' + item.ParentID + '" data-quiz-type="QUIZ2">';
            html += '<div class="quiz-box-header"><h5 class="title">' + itemContent + '</h5>' + renderMedia(item.Media) + '</div>';
            html += '<div class="answer-wrapper row">';
            html += '<fieldset class="answer-item student-answer col-md-6" id="quiz2-' + item.ID + '">';
            html += '<i>Your answer</i>';
            html += '</fieldset>';
            var content = "";
            for (var x = 0; item.CloneAnswers != null && x < item.CloneAnswers.length; x++) {
                var answer = item.CloneAnswers[x];
                content += content == "" ? answer.Content : " | " + answer.Content;
            }
            html += '<fieldset class="answer-item col-md-6" id="quiz2-' + item.ID + '">';
            html += '<i>Correct Answer :</i> <span class="text-success">' + content + '<span>';
            html += '</fieldset>';
            var description = "";
            if (item.Description != null)
                description = item.Description.replace(/\n/g, '<br/>');
            html += '<div class="explaination d-none text-info p-3"><i>' + description + '</i></div>';
            html += '</div></div>';
        }
        html += '</div></div>';
        return html;
    }

    var renderQUIZ3 = function (data) {
        //writeLog("renderQUIZ3", data);
        var toggleButton = '<button class="btn-toggle-width btn btn-success" onclick="togglePanelWidth(this)"><i class="fas fa-arrows-alt-h"></i></button>';
        var html = '<div class="col-md-6 d-inline-block h-100" style="border-right: dashed 1px #CCC"><div class="part-box-header part-column">';
        if (data.Title != null)
            html += '<h5 class="title">' + data.Title + '</h5 >';
        if (data.Description != null)
            html += '<div class="description">' + data.Description + '</div>';
        html += renderMedia(data.Media) + toggleButton + '</div></div>';
        html += '<div class="col-md-6 d-inline-block align-top h-100 p-0">';
        html += '<div class="h-100 align-top p-0"><div class="quiz-wrapper align-top part-column">';

        html += '<div class="row m-0">';
        html += '<div class="quiz-pane col-4 align-top"><div class="pane-item"><div class="quiz-text text-center">Quiz</div></div></div>';
        html += '<div class="quiz-pane col-4 align-top"><div class="pane-item"><div class="quiz-text text-center">Your answer</div></div></div>';
        html += '<div class="quiz-pane col-4 align-top"><div class="pane-item"><div class="quiz-text text-center">Correct answer</div></div></div>';
        html += '</div>';
        for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
            var item = data.Questions[i];
            var content = "";
            if (item.Content != null)
                content = item.Content.replace(/\n/g, '<br/>');
            var answers = "";
            html += '<div class="quiz-item row m-0" id="' + item.ID + '" data-part-id="' + item.ParentID + '" data-quiz-type="QUIZ3">';
            html += '<div class="quiz-pane col-4 align-top"><div class="pane-item"><div class="quiz-text">' + content + '</div></div></div>';
            html += '<div class="answer-pane col-4 ui-droppable align-top student-answer"></div>';
            html += '<div class="answer-pane col-4 ui-droppable align-top">';
            for (var x = 0; item.CloneAnswers != null && x < item.CloneAnswers.length; x++) {
                var answer = item.CloneAnswers[x];
                //if (!answer.IsCorrect) continue;
                var media = renderMedia(answer.Media);
                if (media == "") {
                    answers += '<fieldset data-type="QUIZ3" class="answer-item ui-draggable ui-draggable-handle' + (answer.IsCorrect ? " bg-success" : " d-none") + '" id="' + answer.ID + '"><label class="answer-text">' + answer.Content + '</label></fieldset>';
                }
                else {
                    answers += '<fieldset data-type="QUIZ3" class="answer-item ui-draggable ui-draggable-handle' + (answer.IsCorrect ? " bg-success" : " d-none") + '" id="' + answer.ID + '">' + media + '</fieldset>';
                }
            }
            html += answers + '</div>';
            var description = "";
            if (item.Description != null)
                description = item.Description.replace(/\n/g, '<br/>');
            html += '<div class="explaination d-none text-info p-3"><i>' + description + '</i></div>';
            html += '</div>';

        }
        html += '</div></div></div>';
        return html;
    }

    var renderESSAY = function (data) {
        //writeLog("renderESSAY", data);
        var html = '<div class="part-column"><div class="part-box-header p-3">' + (data.Title == null ? '' : ('<h5 class="title">' + data.Title + '</h5>')) + '<div class="description">' + data.Description + '</div>' + renderMedia(data.Media) + '</div>';
        html += '<div class="quiz-wrapper p-3">';
        html += '<div class="quiz-item" id="' + data.ID + '" data-part-id="' + data.ID + '"></div>';
        html += '<div class="answer-wrapper">';
        html += '<div class="answer-content"><textarea data-part-id="' + data.ID + '" data-lesson-id="' + data.ParentID + '" data-type="ESSAY" id="essay-' + data.ID + '" class="form-control" row="3" placeholder="Answer" onfocusout="AnswerQuestion(this)"></textarea></div>';
        html += '</div></div>';
        html += '</div>';
        return html;
    }

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
        //var nav = '<div id="nav-menu" class="col-md-12 mb-3 pb-3 bd-bottom">';
        var nav_bottom_wrapper = $('<div>', { id: "nav-menu", class: 'col-md-12 pl-0 pr-0' });
        //'<div id="nav-menu" class="col-md-12 mt-3 pt-3 bd-top">';
        //nav += '<div class="col-md-1 text-left d-inline-block"><button class="prevtab btn btn-success" data-toggle="tooltip" title="Phần trước" onclick="tab_goback()"><i class="fas fa-arrow-left"></i></button></div>'; //left button

        var quiz_counter = $('<div>', { id: 'quiz-counter-holder', class: "d-inline-block text-white font-weight-bold align-middle pl-3" });

        var prev_btn_holder = $('<div>', { class: "col-md-3 col-sm-2 text-left d-inline-block" });
        var prev_btn = $('<button>', { class: "prevtab btn btn-success", disabled: "disabled", onclick: "window.PrevPart()" }).append($('<i>', { class: "fas fa-arrow-left" }));
        prev_btn_holder.append(prev_btn).append(quiz_counter);
        var ct_action_holder = $('<div>', { class: "col-md-6 col-sm-8 text-center d-inline-block" });

        if (_type != 1) {
            var explain_btn = $('<button>', { class: "btn btn-success pl-3 pr-3", onclick: "ToggleExplain(this)", text: "Toggle explaination" });
            var golist_btn = $('<button>', { class: "btn btn-success pl-3 pr-3 ml-1", onclick: "GoList(this)", text: "Back to list" });
            var redo_btn = $('<button>', { class: "btn btn-success pl-3 pr-3 mr-1", onclick: "Redo(this)", text: "Do it again" });
            ct_action_holder.append(redo_btn).append(explain_btn).append(golist_btn);
        }
        var next_btn_holder = $('<div>', { class: "col-md-3 col-sm-2 text-right d-inline-block" });
        var next_btn = $('<button>', { class: "nexttab btn btn-success", onclick: "window.NextPart()" }).append($('<i>', { class: "fas fa-arrow-right" }));
        next_btn_holder.append(next_btn);

        nav_bottom_wrapper.append(prev_btn_holder).append(ct_action_holder).append(next_btn_holder);

        $('<div>', { id: 'quizIdx_holder' }).appendTo(nav_bottom_wrapper);

        //nav += '<div class="lesson-tabs col-md-10 d-inline-block"><ul id="pills-tab" class="nav nav-pills compact" onclick="toggle_tab_compact()">';

        _totalPart = data.Parts.length;
        if (_totalPart <= 1)
            next_btn.prop("disabled", true);

        for (var i = 0; i < data.Parts.length; i++) {
            var item = data.Parts[i];
            var icon = arrIcon[item.Type];
            if (icon == void 0) icon = arrIcon["TEXT"];
            var content = renderLessonPart(item, i, data.TemplateType);
            tabList += content;
            var title = item.Title == void 0 || item.Title == null || item.Title == "null" ? "" : item.Title;
        }
        tabList += '</div>';
        html = '<div class="lesson-body" id="' + data.ID + '">' + tabList + '</div>';
        $('#lessonSummary').append(nav_bottom_wrapper);
        return html;
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

    var renderQuizCounter = function () {
        var listQuiz = document.querySelectorAll(".quiz-item");
        var count = 0;
        var answerList = '';
        //writeLog("renderQuizCounter", listQuiz);
        for (var i = 0; listQuiz != null && i < listQuiz.length; i++) {
            var item = listQuiz[i];
            var answer = localStorage.getItem(config.lesson_id + config.class_id + item.id);
            var compeleted = "";
            if (answer != null && answer != void 0 && answer != "") {
                count++;
                compeleted = "completed";
                rendAgain(answer);
            }
            answerList += '<button class="btn bg-secondary text-white rounded-quiz ' + compeleted + '" type="button" id="quizNav' + item.id + '" name="quizNav' + item.id + '" onclick="window.GoQuiz(\'' + item.id + '\')">' + (i + 1) + '</button>';
        }

        var quiz_number_holder = $("#quiz-counter-holder");
        var quiz_number_counter = $('#quiz_number_counter');
        if (quiz_number_counter.length > 0) {
            quiz_number_counter.find(".completed").text(count);
            quiz_number_counter.find(".total").text(listQuiz.length);
        } else {
            quiz_number_holder.append($("<button>", {
                id: "quiz_number_counter",
                class: "quizNumber btn btn-success font-weight-bold"
            })
                .append($("<span>", {
                    class: "completed",
                    text: count
                })).append(" / ")
                .append($("<span>", {
                    class: "total",
                    text: listQuiz.length
                }))).append($('<button>', { class: "quizNumber btn btn-success ml-2", onclick: "window.ToggleNav(this)", tooltips: "Ẩn hiện bảng theo dõi" })
                    .append($("<i>", { class: "fa fa-bars" })));
        }

        var quizNavigator = $('#quizNavigator');
        if (quizNavigator.length == 0) {
            quizNavigator = $('<div>', { id: "quizNavigator", class: "p-2" });
            quizNavigator.append($('<div>', { class: "input-group quiz-wrapper" }).append(answerList));
            $('#quizIdx_holder').append(quizNavigator);
        }
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
    }

    var redo = function () {
        document.location = config.url.exam + "/" + config.lesson.ID + "/" + config.exam.ClassID + "#redo";
    }

    var goList = function () {
        document.location = config.url.list + "/" + config.exam.ClassID + "#" + config.lesson.ChapterID;
    }

    window.ExamReview = {} || ExamReview;
    ExamReview.onReady = onReady;
    ExamReview.Version = version;
    ExamReview.Notification = notification;
    window.PrevPart = prevPart;
    window.NextPart = nextPart;
    window.GoQuiz = goQuiz;
    window.ToggleExplain = toggleExplain;
    window.GoList = goList;
    window.Redo = redo;
    window.ToggleNav = toggleNav;
    return ExamReview;
}());

