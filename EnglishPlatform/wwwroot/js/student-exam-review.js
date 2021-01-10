
var Debug = true;

var writeLog = function (name, msg) {
    if (Debug) {
        console.log(name, msg);
    }
}

var lessontimeout = null;

var ExamReview = (function () {
    var _idNaviton = 'review_navition';
    var _totalPart = 0;
    var _type = 0;
    var _totalQuiz = 0;
    var _correctQuiz = 0;

    function ExamReview() {
        //window.ExamReview = {} || ExamReview;
        this.onReady = onReady;
        this.Version = version;
        this.Notification = notification;
        this.GetConfig = getConfig;
    }

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
        //$('#lessonSummary').toggleClass("expand");
        //$(obj).toggleClass('btn-warning');
        $(obj).toggleClass('btn-warning');
        if ($('#QuizNav').hasClass("show")) {
            $('#QuizNav').removeClass("show");
            $('.right-content').removeClass("showQuiz");
        }
        else {
            $('#QuizNav').addClass("show");
            $('.right-content').addClass("showQuiz");
        }
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
        //if (!config.isTeacher) {
        //    $('.tab-pane .part-column').addClass('scrollbar-outer').scrollbar();
        //}
        renderNavigation();
    }

    var getNavigationRoot = function () {
        id = config.isTeacher ? _idNaviton : 'QuizNav';
        var doc = document.getElementById(id);
        //if (!config.isTeacher) {
        //    doc.classList.add('show');
        //    $('.quizNumber').focus().click();
        //}
        return doc;
    }

    var daCham = 0;

    var renderNavigation = function () {
        daCham = 0;
        var lesson = config.lesson;
        var parts = lesson.Part;
        var root = getNavigationRoot();
        root.innerHTML = "";
        var number = 1;
        var ul = document.createElement("ul");
        //ul.style.width = "calc(100% - 100px)";
        ul.style.display = "inline-block";
        for (var i = 0; i < parts.length; i++) {
            var part = parts[i];
            number = renderItemNavigation(ul, part, number);
        }
        if (config.isTeacher) {
            root.innerHTML = "<div class='number-reivew-point' style='width:100px;display:inline-block; white-space:nowrap'>(Đã chấm " + daCham + "/" + (number - 1) + ")</div>";
        }
        root.appendChild(ul);
        var data = config.exam;
        var completetime = moment(data.Updated);//.format("DD/MM/YYYY hh:mm:ss A");
        var starttime = moment(data.Created);
        var duration = moment.duration(completetime.diff(starttime));
        _correctQuiz = $('#QuizNav ul li.success').length;
        _totalQuiz = $('#QuizNav ul li').length;

        $(".top-menu[for=lesson-info]").prepend($('<span>',
            {
                class: "m-2 text-muted font-weight-bold",
                style: "font-size: 150%;white-space: nowrap;",

            }).append($("<span>", { class: "no-mobile" }).append("Kết quả: ")).append($('<span>',
                { class: "text-primary", text: config.exam.Point + "/" + config.exam.MaxPoint })).append(" (" + durationFormat(duration) + ")"));

        var totalhead = document.getElementById('total-point-head');
        if (totalhead) {
            totalhead.innerHTML = document.querySelectorAll("#" + _idNaviton + " ul li").length;
        }
    }

    var updateShowPoint = function (id) {
        var point = getNavigationRoot().querySelector('.number-reivew-point');
        var listUnChecked = getNavigationRoot().querySelectorAll('.unchecked');
        var total = getNavigationRoot().querySelector("ul").childElementCount;
        point.innerHTML = '(' + (total - listUnChecked.length) + '/' + total + ')';
        if (listUnChecked.length == 0 && id != void 0) {
            updatePoint(1, 2, id);
        }
        //point.style.background = "red";
        //setTimeout(function () {
        //    point.style.background = "#fff";
        //}, 3000);
    }

    var renderItemNavigation = function (el, data, index) {
        var examDetails = config.exam.Details;
        for (var i = 0; i < data.Questions.length; i++) {
            var item = data.Questions[i];
            //console.log(item);
            var examDetail = examDetails.filter(o => o.QuestionID == item.ID);
            if (examDetail) {
                var detail = examDetail.length > 0 ? examDetail[0] : null;
                var title = "Làm đúng";
                var _class = "success";
              debugger
                if (item.TypeAnswer != "ESSAY") {
                    // câu trắc nghiệm
                    if (detail) {
                        var cautraloidung = detail.RealAnswerValue == null ? '' : detail.RealAnswerValue//.replace(/[^a-z0-9\s]/gi, '').toLowerCase().trim();
                        var cautraloi = detail.AnswerValue;//.replace(/[^a-z0-9\s]/gi, '').toLowerCase().trim();
                        var _check = false;
                        if (cautraloidung == cautraloi) { _check = true; }
                        if (detail.AnswerID != null && detail.AnswerID == detail.RealAnswerID) { _check = true; }
                        if (detail.Point > 0) { _check = true; }
                        if (!_check) {
                            _class = "danger";
                            title = "Làm sai";
                        }
                        else {
                            _class = "success";
                            title = "Làm đúng";
                        }
                    }
                    else {
                        _class = "bg-warning";
                        title = "chưa làm";
                    }
                    daCham++;
                } else {
                    if (detail) {
                        console.log(item);
                        debugger
                        if (item.RealAnswerEssay || item.PointEssay >= 0) {
                            _class = "essay success";
                            if (item.PointEssay <= 0) {
                                _class = "essay danger";
                            }
                            _class += " checked";
                            title = "Đã chấm";
                            daCham++;
                        }
                        else {
                            _class = "essay unchecked";
                            title = "Chưa chấm";
                        }
                    }
                    else {
                        _class = "essay bg-warning";
                        title = "Chưa làm";
                    }
                }
            }
            var li = document.createElement("li");
            li.dataset.id = item.ID;
            li.dataset.part = data.ID;
            //console.log(data);
            li.dataset.maxpoint = data.Point;
            li.classList = "item-review-nav " + _class;
            li.innerHTML = '<a>' + index + '</a>';
            li.querySelector("a").addEventListener("click", _eventGotoSelection);
            li.querySelector("a").setAttribute("title", title);
            el.appendChild(li);
            index++;
        }
        return index;
    }

    var _eventGotoSelection = function (event) {
        var id = this.parentElement.dataset.id;
        var partID = this.parentElement.dataset.part;
        var part = document.getElementById("pills-part-" + partID);



        if (part != null) {
            var el = part.querySelector("[id='" + id + "']");
            if (!part.classList.contains("active")) {
                part.parentElement.querySelector('.tab-pane.active').classList.remove(...["show", "active"]);
                part.classList.add(...["show", "active"]);
            }
            var select = part.querySelector(".selection");
            if (select != null) {
                select.classList.remove("selection");
            }
            var aSelect = getNavigationRoot().querySelector('.selection');
            if (aSelect != null) {
                aSelect.classList.remove("selection");
            }
            //console.log(getNavigationRoot());
            this.classList.add("selection");
            el.classList.add("selection");
            //console.log(el);
            el.scrollIntoView({ behavior: "smooth", block: "end", inline: "nearest" })

            var panes = $('.tab-pane');
            var index = panes.index($('.tab-pane.active'));
            goPartInx(index);
        }
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

        for (var i = 0; i < data.Part.length; i++) {
            var _data = data.Part[i];
            if (_data.Type == "QUIZ1" || _data.Type == "QUIZ4") {
                addCssAnswerQ14(_data);
            }
        }

        if ($('textarea[data-type=ESSAY]').length > 0) {
            $(document).ready(function () {
                $('textarea[data-type=ESSAY]').each(function () {
                    CKEDITOR.replace($(this).attr('id'));
                });
            });
        }
    }

    var addCssAnswerQ14 = function (data) {
        for (var i = 0; i < data.Questions.length; i++) {
            var cloneAnswers = data.Questions[i].CloneAnswers;
            for (var j = 0; j < cloneAnswers.length; j++) {
                var id = cloneAnswers[j].ID;
                var isCorrect = cloneAnswers[j].IsCorrect;
                var answerWmedia = $("#" + id)[0];
                if (isCorrect) {
                    if (answerWmedia) {
                        $(answerWmedia).find("img").css("border", "1px solid #28a745");
                        $(answerWmedia).find("img").css("border-radius", "5px");
                    }
                }
                else {
                    if (answerWmedia) {
                        $(answerWmedia).find("img").css("border", "1px solid #dc3545");
                        $(answerWmedia).find("img").css("border-radius", "5px");
                    }
                }
            }
        }
    }

    var durationFormat = function (_duration) {
        var result = "";
        if (_duration.asHours() > 1)
            result += _duration.asHours().toFixed(0) + ":";
        if (_duration.asMinutes() > 0)
            result += _duration.minutes().toFixed(0) + ":";
        result += _duration.seconds();
        return result;
    }

    var renderResult = function () {
        var data = config.exam;

        var lastpoint = (data.MaxPoint > 0 ? (data.Point * 100 / data.MaxPoint) : 0);
        var completetime = moment(data.Updated);//.format("DD/MM/YYYY hh:mm:ss A");
        var starttime = moment(data.Created);
        var duration = moment.duration(completetime.diff(starttime));

        lastExamResult = $("<div>", { id: "last-result", class: "text-center p-1 text-white" });


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

        $('#lessonSummary #nav-menu .text-center').prepend(lastExamResult);

    }

    var renderLessonPart = function (data, index, type) {
        //debugger
        //writeLog("renderLessonPart", data);
        var active = "";
        if (index != void 0 && index == 0) {
            active = "show active";
        }

        var styleTeacher = config.isTeacher ? "height: 80vh;overflow: auto;" : "";

        var html = '<div id="pills-part-' + data.ID + '" style="' + styleTeacher + '"  class="tab-pane fade ' + active + '" role="tabpanel" aria-labelledby="pills-' + data.ID + '">';
        html += '<div class="part-box ' + data.Type + '" id="' + data.ID + '">';
        //console.log(data);
        switch (data.Type) {
            case "QUIZ1": html += renderQUIZ1(data); //type == 2 ? renderQUIZ1(data) : renderQUIZ1_BG(data); //chon 1 dap an dung
                break;
            case "QUIZ2": html += renderQUIZ2(data);//type == 2 ? renderQUIZ2(data) : renderQUIZ2_BG(data);
                break;
            case "QUIZ3": html += renderQUIZ3(data); //type == 2 ? renderQUIZ3(data) : renderQUIZ3_BG(data);
                break;
            case "QUIZ4": html += renderQUIZ4(data); //type == 2 ? renderQUIZ3(data) : renderQUIZ3_BG(data); //chon 1/nhieu dap an dung
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

    //check ki tu dac biet
    var checkSpecialCharacters = function (chain) {
        //debugger
        chain = chain.trim();
        var space = [160, 173, 8194, 8195, 8201, 8204, 8205, 8206, 8207];//khoang trang
        var beginning = [8217, 8216, 180, 8216, 8242, 8219];//ki tu ‘’ trong word
        var quotation = [8220, 8221, 8243];//check ki tu “” trong word

        for (i = 0; i < chain.length; i++) {//check ki tu khoang trang dac biet
            if (space.includes(chain[i].charCodeAt())) {
                chain = chain.replace(chain[i], "_");
            }
        }

        for (i = 0; i < chain.length; i++) {//check ki tu ‘’ trong word
            if (beginning.includes(chain[i].charCodeAt())) {
                //chain[i] = "'";
                chain = chain.replace(chain[i], "'");
            }
        }

        for (i = 0; i < chain.length; i++) {//check ki tu “” trong word
            if (quotation.includes(chain[i].charCodeAt())) {
                //chain[i] = "\"";
                chain = chain.replace(chain[i], "\"");
            }
        }
        //debugger
        return chain;
    }

    var renderAnswer = function (data, type) {
        //debugger
        var quizId = data.QuestionID;

        var cautraloidung = data.RealAnswerEssay == null ? '' : data.RealAnswerValue//.replace(/[^a-z0-9\s]/gi, '').toLowerCase().trim();
        console.log(cautraloidung);
        var cautraloi = data.AnswerValue//.replace(/[^a-z0-9\s]/gi, '').toLowerCase().trim();
        var _check = false;
        if (cautraloidung == cautraloi) {
            _check = true;
        }
        if (data.AnswerID != null && data.AnswerID == data.RealAnswerID) { _check = true; }
        if (data.Point > 0) { _check = true; }
        //console.log(type);
        if (data.AnswerID != null && data.AnswerID != "") {
            switch (type) {
                case "QUIZ1":
                    var _answer = $('#' + data.AnswerID).clone().removeClass("d-none");
                    $('#' + quizId + ' .student-answer').append(_answer);
                    break;
                case "QUIZ3":
                    var _answer = $('#' + data.AnswerID).clone().removeClass("d-none");
                    //if (!_check)//wrong answer                        
                    //    _answer.addClass("bg-danger");
                    if (_check) {//dap an dung
                        _answer.removeClass("bg-danger");
                        _answer.addClass("bg-success");
                    }
                    else {//dap an sai
                        _answer.removeClass("bg-success");
                        _answer.addClass("bg-danger");
                    }
                    $('#' + quizId + ' .student-answer').append(_answer);
                    break;
                case "QUIZ4":
                    var IDs = data.AnswerID.split(',');
                    IDs.forEach(function (value) {
                        $('#' + quizId + ' .student-answer').append($('#' + value).clone().removeClass("d-none"));
                    });
                    break;
            }

        }
        else { //"QUIZ2"
            if (type == "ESSAY") {
                $('#' + quizId + ' .student-answer').append(" <span class='text-dark'>" + data.AnswerValue + "</span>");
            } else {
                if (_check) {
                    var content_answer = "";
                    var html = "";
                    $('#' + quizId + ' .student-answer').append("<span class='text-success'>" + data.AnswerValue + "</span>");

                    var _answer = $("#quiz2-" + quizId)[0];
                    content_answer = $(_answer).find(".text-success")[0].textContent.trim();

                    var correct_answer = _answer.nextElementSibling;
                    var content_correct_answer = $(correct_answer).find(".text-success")[0].textContent;
                    var listContent = content_correct_answer.split(' | ');
                    for (i = 0; i < listContent.length; i++) {
                        if (listContent[i] == content_answer) {
                        //if (data.Point>0) {
                            html += " | <span style='font-weight:600'>" + listContent[i] + "</span>";
                        }
                        else {
                            html += " | " + listContent[i];
                        }
                    }
                    $($(correct_answer).find(".text-success")[0]).html(html.substring(3));
                }
                else {
                    //$('#' + quizId + ' .student-answer').append(" <span class='text-danger'><del>" + data.AnswerValue + "</del><span>");
                    $('#' + quizId + ' .student-answer').append(" <span class='text-danger'>" + data.AnswerValue + "<span>");

                    var _answer = $("#quiz2-" + quizId)[0];
                    var a = $(_answer).find(".text-danger")[0].textContent;
                    var content_answer = checkSpecialCharacters(a);

                    var correct_answer = _answer.nextElementSibling;
                    var content_correct_answer = $(correct_answer).find(".text-success")[0].textContent;
                    var listContent = content_correct_answer.split(' | ');

                    for (i = 0; i < listContent.length; i++) {
                        listContent[i] = checkSpecialCharacters(listContent[i]);
                    }

                    var html = "";
                    var tile = "";

                    for (var i = 0; i < listContent.length; i++) {
                        var arrayTxt1 = listContent[i].split(" ");//dap an dung
                        var arrayTxt2 = content_answer.split(" ");//dap an hoc sinh dien
                        if (arrayTxt1.length > arrayTxt2.length) {
                            var test1 = 0;
                            for (j = 0; j < arrayTxt2.length; j++) {
                                if (arrayTxt1[j] == arrayTxt2[j]) {
                                    test1++;
                                }
                            }
                            tile += test1 + ",";
                        }
                        else {
                            test2 = 0;
                            for (j = 0; j < arrayTxt1.length; j++) {
                                if (arrayTxt1[j] == arrayTxt2[j]) {
                                    test2++;
                                }
                            }
                            tile += test2 + ",";
                        }
                    }

                    var tile = tile.split(",");
                    tile.pop();
                    for (var i = 0; i < tile.length; i++) {
                        tile[i] = parseInt(tile[i]);
                    }
                    var index = 0;
                    max = tile[0];

                    //debugger
                    for (var i = 0; i < tile.length; i++) {
                        if (max < tile[i]) {
                            max = tile[i];
                            index = i;
                        }
                    }

                    //debugger
                    if (max != 0) {
                        var chodung1 = "";//ben dap an hoc sinh tra loi
                        var chodung2 = "";//ben dap an dung

                        var detail_CorrectAnswer = listContent[index].split(" "); //dap an dung của hệ thống
                        var detail_Answer = content_answer.split(" "); //dap an hoc vien dien
                        //debugger
                        if (detail_CorrectAnswer.length == detail_Answer.length) {//TH dap an dung = dap an hoc sinh dien
                            //debugger
                            for (var i = 0; i < detail_Answer.length; i++) {
                                if (detail_CorrectAnswer[i] == detail_Answer[i]) {
                                    chodung1 += "<span style='color:#28a745'>" + detail_Answer[i] + "</span> ";
                                    chodung2 += "<span style='font-weight:600'>" + detail_CorrectAnswer[i] + "</span> ";
                                }
                                else {
                                    chodung1 += "<span style='font-weight:600;border-bottom: 1px solid'>" + detail_Answer[i] + "</span> ";
                                    chodung2 += "<span style='font-weight:600;border-bottom: 1px solid;'>" + detail_CorrectAnswer[i] + "</span> ";
                                }
                            }
                        }
                        else if (detail_CorrectAnswer.length > detail_Answer.length) {//TH dap an dung dai hon dap an hoc sinh dien
                            //debugger
                            for (var i = 0; i < detail_Answer.length; i++) {
                                if (detail_CorrectAnswer[i] == detail_Answer[i]) {
                                    chodung1 += "<span style='color:#28a745'>" + detail_Answer[i] + "</span> ";
                                    chodung2 += "<span style='font-weight:600'>" + detail_CorrectAnswer[i] + "</span> ";
                                }
                                else {
                                    chodung1 += " <span style='font-weight:600;border-bottom: 1px solid;color:#dc3545'>" + detail_Answer[i] + "</span> ";
                                    chodung2 += " <span style='font-weight:600;border-bottom: 1px solid;'>" + detail_CorrectAnswer[i] + "</span> ";
                                }
                            }
                            for (var i = detail_Answer.length; i < detail_CorrectAnswer.length; i++) {
                                chodung1 += " <span style='font-weight:600;color:#dc3545'>-</span> ";
                                chodung2 += "<span style='border-bottom: 1px solid;color:#28a745'>" + detail_CorrectAnswer[i] + "</span> ";
                            }
                        }
                        else {//TH dap an dung ngan hon dap an hoc sinh dien
                            //debugger
                            for (var i = 0; i < detail_CorrectAnswer.length; i++) {
                                if (detail_CorrectAnswer[i] == detail_Answer[i]) {
                                    chodung1 += "<span style='color:#28a745'>" + detail_Answer[i] + "</span> ";
                                    chodung2 += "<span style='font-weight:600'>" + detail_CorrectAnswer[i] + "</span> ";
                                }
                                else {
                                    chodung1 += "<span style='font-weight:600;border-bottom: 1px solid;color:#dc3545'>" + detail_Answer[i] + "</span> ";
                                    chodung2 += "<span style='font-weight:600;border-bottom: 1px solid'>" + detail_CorrectAnswer[i] + "</span> ";
                                }
                            }
                            for (var i = detail_CorrectAnswer.length; i < detail_Answer.length; i++) {
                                chodung1 += "<span style='border-bottom: 1px solid;color:#dc3545'>" + detail_Answer[i] + "</span> ";
                            }
                        }
                        newlistContent = "";
                        for (var i = 0; i < listContent.length; i++) {
                            if (i == index) {
                                listContent[i] = chodung2;
                            }
                            newlistContent += listContent[i] + " | ";
                        }
                        $($(_answer).find(".text-danger")[0]).html(chodung1);
                        $($(correct_answer).find(".text-success")[0]).html(newlistContent.substring(0, newlistContent.lastIndexOf('|') - 1));
                        //debugger
                    }

                    /*
                    if (max != 0) {
                        for (i = 0; i < tile.length; i++) {
                            if (max < tile[i]) {
                                max = tile[i];
                                index = i;
                            }
                        }

                        var chodung = "";
                        if (listContent[index].length > content_answer.length) {//TH dap an dung dai hon dap an
                            while (content_answer.length < listContent[index].length) {
                                content_answer += "_";
                            }
                            for (i = 0; i < content_answer.length; i++) {
                                if (content_answer[i] == listContent[index][i]) {
                                    chodung += "<span style='color:#28a745'>" + content_answer[i] + "</span>";
                                }
                                else {
                                    chodung += "<span style='font-weight:600'>" + content_answer[i] + "</span>";
                                }
                            }
                        }
                        else {//TH dap an dung ngan hon dap an
                            for (i = 0; i < listContent[index].length; i++) {
                                if (content_answer[i] == listContent[index][i]) {
                                    chodung += "<span style='color:#28a745'>" + content_answer[i] + "</span>";
                                }
                                else {
                                    chodung += "<span style='font-weight:600'>" + content_answer[i] + "</span>";
                                }
                            }
                            for (i = listContent[index].length; i < content_answer.length; i++) {
                                //debugger
                                chodung += "<span style='font-weight:600'>" + content_answer[i] + "</span>";
                            }
                        }
                        html += chodung;
                        $($(_answer).find(".text-danger")[0]).html(html);

                        sosanh = "";
                        newlistContent = "";
                        for (i = 0; i < listContent[index].length; i++) {
                            if (content_answer[i] != listContent[index][i]) {
                                sosanh += "<span style='font-weight:600'>" + listContent[index][i] + "</span>";
                            }
                            else {
                                sosanh += listContent[index][i];
                            }
                        }
                        for (i = 0; i < listContent.length; i++) {
                            if (i == index) {
                                listContent[i] = sosanh;
                            }
                            newlistContent += listContent[i] + " | ";
                        }
                        //debugger
                        $($(correct_answer).find(".text-success")[0]).html(newlistContent.substring(0, newlistContent.lastIndexOf('|') - 1));
                    }
                    */
                    /*
                    if (max != 0) {
                        for (i = 0; i < tile.length; i++) {
                            if (max < tile[i]) {
                                max = tile[i];
                                index = i;
                            }
                        }

                        var chosai = "";
                        //debugger
                        if (listContent[index].length > content_answer.length) {//TH dap an dung dai hon dap an
                            for (i = 0; i < listContent[index].length; i++) {
                                if (listContent[index][i] == content_answer[i]) {
                                    chosai += listContent[index][i];
                                }
                                else {
                                    chosai += "<span style='color:red'>" + listContent[index][i] + "</span>";
                                }
                            }
                            //for (i = (listContent[index].length - content_answer.length); i < listContent[index].length; i++) {
                            //    chosai += listContent[index][i];
                            //}
                        }
                        else {//TH dap an dung ngan hon dap an
                            while (content_answer.length > listContent[index].length) {
                                listContent[index] += "_";
                            }
                            for (i = 0; i < content_answer.length; i++) {
                                if (listContent[index][i] == content_answer[i]) {
                                    //debugger
                                    if (i >= listContent[index][i].length) {
                                        //listContent[index][i] += "";
                                        //break;
                                    }
                                    //else {
                                    chosai += listContent[index][i];
                                    //}
                                }
                                else {
                                    chosai += "<span style='color:red'>" + listContent[index][i] + "</span>";
                                }
                            }
                        }

                        for (i = 0; i < listContent.length; i++) {
                            if (i == index) {
                                listContent[i] = chosai;
                            }
                            html += listContent[i] + " | ";
                        }
                        $($(correct_answer).find(".text-success")[0]).html(html.substring(0, html.lastIndexOf('|') - 1));
                        //debugger
                    }
                    */
                }
            }
        }

    }

    var renderTEXT = function (data) {
        var title = "";
        if (data.Title != null) {
            var title = '<div class="part-box-header pl-3 pr-3"><h5 class="title">' + data.Title + '</h5></div>';
        }
        var html = "<div class='part-column'>" + title + '<div class="content-wrapper p-3">';
        html += '<div class="doc-content">' + data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn") + '</div>';
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
        //longht sửa 
        html += '<div class="media-holder ' + data.Type + '"><audio controls=""><source src="' + data.Media.Path + '" type="audio/mpeg">Your browser does not support the audio tag</audio></div>';
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

        if (data.Media.Path.endsWith("doc") || data.Media.Path.endsWith("docx")) {
            html += "<div class='media-holder " + data.Type + "'><iframe src='https://docs.google.com/gview?url=https://" + window.location.hostname + data.Media.Path + "&embedded=true' style='width:100%, height:800px'></iframe></div>";
        }
        else
            html += '<div class="media-holder ' + data.Type + '"><embed src="' + data.Media.Path + '#view=FitH" style="width: 100%; height: 800px; border:1px solid"></div>';
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
        //debugger
        //writeLog("renderQUIZ1", data);
        var toggleButton = '<button class="btn-toggle-width btn btn-success" onclick="togglePanelWidth(this)"><i class="fas fa-arrows-alt-h"></i></button>';
        var html = '<div class="col-md-6 d-inline-block h-100 overflow-auto" style="border-right: dashed 1px #CCC"><div class="part-box-header part-column">';
        if (data.Title != null)
            html += '<h5 class="title">' + data.Title + '</h5 >';
        if (data.Description != null)
            html += '<div class="description">' + data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn") + '</div>';
        html += renderMedia(data.Media) + toggleButton + '</div></div>';
        html += '<div class="col-md-6 d-inline-block align-top h-100 overflow-auto"><div class="quiz-wrapper part-column">';
        for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
            var item = data.Questions[i];
            html += '<div class="quiz-item" id="' + item.ID + '" data-part-id="' + item.ParentID + '" data-quiz-type="QUIZ1">';
            html += '<div class="quiz-box-header"><h5 class="title">' + item.Content + '</h5>' + renderMedia(item.Media) + '</div>';
            html += '<div class="answer-wrapper">';
            html += '<div class="student-answer">';
            html += '<fieldset class="answer-item d-inline mr-3 align-top">';
            html += '<div style="cursor: pointer; display:inline-block" class="form-check">';
            html += '<label class="answer-text form-check-label"><i>Trả lời: </i></label>';
            html += '</div>';
            html += '</fieldset>';
            html += '</div>';
            html += '<div>';
            html += '<fieldset class="answer-item d-inline mr-3 align-top">';
            html += '<div style="cursor: pointer; display:inline-block" class="form-check">';
            html += '<label class="answer-text form-check-label"><i>Đáp án đúng: </i></label>';
            html += '</div>';
            html += '</fieldset>';
            for (var x = 0; item.CloneAnswers != null && x < item.CloneAnswers.length; x++) {
                var answer = item.CloneAnswers[x];
                if (!answer.Content) {
                    answer.Content = "";
                }
                //if(!answer.IsCorrect) continue;
                html += '<fieldset class="answer-item d-inline mr-3 align-top" id="' + answer.ID + '">';
                html += '<div style="cursor: pointer; display:inline-block" class="form-check" data-part-id="' + data.ID + '" data-lesson-id="' + data.ParentID + '" data-question-id="' + item.ID + '" data-id="' + answer.ID + '" data-type="QUIZ1" data-value="' + answer.Content + '">';
                if (answer.IsCorrect) {
                    html += '<label class="answer-text form-check-label text-success" for="' + answer.ID + '">' + answer.Content + '</label>';
                }
                else {
                    html += '<label class="answer-text form-check-label text-danger" for="' + answer.ID + '"><del>' + answer.Content + '</del></label>';
                }
                html += '</div>';
                html += renderMedia(answer.Media);
                html += '</fieldset>';
            }
            html += '</div>';
            var description = "";
            if (item.Description != null)
                description = item.Description.replace(/\n/g, '<br/>').replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn");
            html += '<div class="explaination d-none text-info p-3"><i>' + description + '</i></div>';
            html += '</div></div>';
        }
        html += '</div></div>';
        return html;
    }

    //dien tu
    var renderQUIZ2 = function (data) {
        //console.log(data);
        //writeLog("renderQUIZ2", data);
        var toggleButton = '<button class="btn-toggle-width btn btn-success" onclick="togglePanelWidth(this)"><i class="fas fa-arrows-alt-h"></i></button>';
        var html = '<div class="col-md-6 d-inline-block h-100 overflow-auto" style="border-right: dashed 1px #CCC"><div class="part-box-header part-column">';
        if (data.Title != null)
            html += '<h5 class="title">' + data.Title + '</h5 >';
        if (data.Description != null)
            html += '<div class="description">' + data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn") + '</div>';
        html += renderMedia(data.Media) + toggleButton + '</div></div>';
        html += '<div class="col-md-6 d-inline-block align-top h-100"><div class="quiz-wrapper part-column">';
        for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
            var item = data.Questions[i];
            var itemContent = item.Content == null ? "Quiz " + (i + 1) + " : " : item.Content;
            html += '<div class="quiz-item" id="' + item.ID + '" data-part-id="' + item.ParentID + '" data-quiz-type="QUIZ2">';
            html += '<div class="quiz-box-header"><h5 class="title">' + itemContent + '</h5>' + renderMedia(item.Media) + '</div>';
            html += '<div class="answer-wrapper row">';
            html += '<fieldset class="answer-item student-answer col-md-6" id="quiz2-' + item.ID + '">';
            html += '<i>Trả lời: </i>';
            html += '</fieldset>';
            var content = "";
            for (var x = 0; item.CloneAnswers != null && x < item.CloneAnswers.length; x++) {
                var answer = item.CloneAnswers[x];
                content += content == "" ? answer.Content : " | " + answer.Content;
            }
            html += '<fieldset class="answer-item col-md-6" id="quiz2-' + item.ID + '">';
            html += '<i>Đáp án đúng :</i> <span class="text-success">' + content + '<span>';
            html += '</fieldset>';
            var description = "";
            if (item.Description != null)
                description = item.Description.replace(/\n/g, '<br/>').replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn");
            html += '<div class="explaination d-none text-info p-3"><i>' + description + '</i></div>';
            html += '</div></div>';
        }
        html += '</div></div>';
        return html;
    }

    //noi dap an
    var renderQUIZ3 = function (data) {
        //writeLog("renderQUIZ3", data);
        var toggleButton = '<button class="btn-toggle-width btn btn-success" onclick="togglePanelWidth(this)"><i class="fas fa-arrows-alt-h"></i></button>';
        var html = '<div class="col-md-6 d-inline-block h-100 overflow-auto" style="border-right: dashed 1px #CCC"><div class="part-box-header part-column">';
        if (data.Title != null)
            html += '<h5 class="title">' + data.Title + '</h5 >';
        if (data.Description != null)
            html += '<div class="description">' + data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn") + '</div>';
        html += renderMedia(data.Media) + toggleButton + '</div></div>';
        html += '<div class="col-md-6 d-inline-block align-top h-100 p-0">';
        html += '<div class="h-100 align-top p-0"><div class="quiz-wrapper align-top part-column">';

        html += '<div class="row m-0">';
        html += '<div class="quiz-pane col-4 align-top"><div class="pane-item"><div class="quiz-text text-center">Quiz</div></div></div>';
        html += '<div class="quiz-pane col-4 align-top"><div class="pane-item"><div class="quiz-text text-center">Trả lời</div></div></div>';
        html += '<div class="quiz-pane col-4 align-top"><div class="pane-item"><div class="quiz-text text-center">Đáp án đúng</div></div></div>';
        html += '</div>';
        for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
            var item = data.Questions[i];
            var content = "";

            if (item.Media != null)
                content = renderMedia(item.Media)
            else
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
                description = item.Description.replace(/\n/g, '<br/>').replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn");
            html += '<div class="explaination d-none text-info p-3"><i>' + description + '</i></div>';
            html += '</div>';

        }
        html += '</div></div></div>';
        return html;
    }

    var renderQUIZ4 = function (data) {
        //writeLog("renderQUIZ4", data);
        var toggleButton = '<button class="btn-toggle-width btn btn-success" onclick="togglePanelWidth(this)"><i class="fas fa-arrows-alt-h"></i></button>';
        var html = '<div class="col-md-6 d-inline-block h-100 overflow-auto" style="border-right: dashed 1px #CCC"><div class="part-box-header part-column">';
        if (data.Title != null)
            html += '<h5 class="title">' + data.Title + '</h5 >';
        if (data.Description != null)
            html += '<div class="description">' + data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn") + '</div>';
        html += renderMedia(data.Media) + toggleButton + '</div></div>';
        html += '<div class="col-md-6 d-inline-block align-top h-100"><div class="quiz-wrapper part-column">';
        for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
            var item = data.Questions[i];
            html += '<div class="quiz-item" id="' + item.ID + '" data-part-id="' + item.ParentID + '" data-quiz-type="QUIZ4">';
            html += '<div class="quiz-box-header"><h5 class="title">' + item.Content + '</h5>' + renderMedia(item.Media) + '</div>';
            html += '<div class="answer-wrapper">';
            html += '<div class="student-answer">';
            html += '<fieldset class="answer-item d-inline mr-3 align-top">';
            html += '<div style="cursor: pointer; display:inline-block" class="form-check">';
            html += '<label class="answer-text form-check-label"><i>Trả lời: </i></label>';
            html += '</div>';
            html += '</fieldset>';
            html += '</div>';
            html += '<div>';
            html += '<fieldset class="answer-item d-inline mr-3 align-top">';
            html += '<div style="cursor: pointer; display:inline-block" class="form-check">';
            html += '<label class="answer-text form-check-label"><i>Đáp án đúng: </i></label>';
            html += '</div>';
            html += '</fieldset>';
            for (var x = 0; item.CloneAnswers != null && x < item.CloneAnswers.length; x++) {
                var answer = item.CloneAnswers[x];
                //if(!answer.IsCorrect) continue;
                html += '<fieldset class="answer-item d-inline mr-3 align-top" id="' + answer.ID + '">';
                html += '<div style="cursor: pointer; display:inline-block" class="form-check" data-part-id="' + data.ID + '" data-lesson-id="' + data.ParentID + '" data-question-id="' + item.ID + '" data-id="' + answer.ID + '" data-type="QUIZ1" data-value="' + answer.Content + '">';
                if (answer.IsCorrect)
                    html += '<label class="answer-text form-check-label text-success" for="' + answer.ID + '">' + (answer.Media != null ? renderMedia(answer.Media) : answer.Content) + '</label>';
                else
                    html += '<label class="answer-text form-check-label text-danger" for="' + answer.ID + '"><del>' + (answer.Media != null ? renderMedia(answer.Media) : answer.Content) + '</del></label>';
                html += '</div>';
                html += renderMedia(answer.Media);
                html += '</fieldset>';
            }
            html += '</div>';
            var description = "";
            if (item.Description != null)
                description = item.Description.replace(/\n/g, '<br/>').replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn");
            html += '<div class="explaination d-none text-info p-3"><i>' + description + '</i></div>';
            html += '</div></div>';
        }
        html += '</div></div>';
        return html;
    }

    var renderESSAY = function (data) {
        var toggleButton = '<button class="btn-toggle-width btn btn-success" onclick="togglePanelWidth(this)"><i class="fas fa-arrows-alt-h"></i></button>';
        var html = '<div class="col-md-6 d-inline-block h-100 overflow-auto" style="border-right: dashed 1px #CCC"><div class="part-box-header part-column">';
        if (data.Title != null)
            html += '<h5 class="title">' + data.Title + '</h5 >';
        if (data.Description != null)
            html += '<div class="description">' + data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn") + '</div>';
        html += renderMedia(data.Media) + toggleButton + '</div></div>';
        html += '<div class="col-md-6 d-inline-block align-top h-100"><div class="quiz-wrapper part-column">';

        html += '<div class="quiz-wrapper p-3">';
        html += '<div class="quiz-item" id="' + data.ID + '" data-part-id="' + data.ID + '"></div>';
        html += '<div class="answer-wrapper">';
        //html += '<div class="answer-content"><textarea data-part-id="' + data.ID + '" data-lesson-id="' + data.ParentID + '" data-type="ESSAY" id="essay-' + data.ID + '" class="form-control" row="3" placeholder="Answer" onfocusout="AnswerQuestion(this)"></textarea></div>';
        //html += '</div></div>';
        for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
            var item = data.Questions[i];
            var itemContent = item.Content == null ? "Quiz " + (i + 1) + " : " : item.Content;
            html += '<div class="quiz-item" id="' + item.ID + '" data-part-id="' + item.ParentID + '" data-quiz-type="ESSAY">';
            html += '<div class="quiz-box-header"><h5 class="title">' + itemContent + '</h5>' + renderMedia(item.Media) + '</div>';
            html += '<div class="answer-wrapper row">';
            html += '<fieldset class="answer-item student-answer col-md-12" id="essay-' + item.ID + '">';
            html += '<i>Trả lời</i>';
            html += '</fieldset>';
            var anwerMedia = item.MediasAnswer;
            var medias = item.Medias;//file học viên upload
            html += '<fieldset class="col-md-12">';
            if (medias != null && medias.length > 0) {
                html += '<div class="attachment mt-3"><strong>File đính kèm:</strong> ';
                html += '<div>'
                for (var z = 0; medias != null && z < medias.length; z++) {
                    var itemMedia = medias[z];
                    html += renderMedia(itemMedia);
                }
                html += '</div></div>';
            }
            html += '</fieldset>';

            var content = item.RealAnswerEssay;// cau tra loi cua giao vien
            var point = item.PointEssay; // điểm giáo viên chấm
            //console.log(data);
            //console.log(item);
            if (config.isTeacher) {
                html += '<fieldset data-last="false" class="answer-item col-md-12" id="essay-teacher-' + item.ID + '" style="padding:10px; border:1px solid #ccc">';
                if (content != "" && content != null) {
                    html += '<div class="alert alert-success"><span class="text-success">Đã chấm</span></div>';
                } else {
                    html += '<div class="alert alert-danger"><span class="text-danger">Chưa chấm</span></div>';
                }
                html += '<div> Điểm :<input onkeyup="validate(this)" max="' + data.Point + '" min="0" type="number" value="' + point + '" style="width:40px;text-align:right;margin-bottom:10px"> /' + data.Point + '</div>';
                html += '<i>Bài chữa :</i>';
                var realContent = content == null ? "" : content;
                html += '<div><textarea style="width:100%; padding:5px" rows="6" name="TEXT_CKEDITOR_' + item.ID + '">' + realContent + '</textarea></div>';

                //upload file
                var type = "type='file'";
                var strFile = "this.parentElement.querySelector('input[" + type + "]')";
                html += '<div data-target="' + item.ID + '"><input type="file" name="files" multiple style="display:none">';
                html += '<button class="btn btn-sm btn-success" onclick="uploadFile(this)">Upload file</button>';
                html += '<div class="preview">';
                for (var x = 0; anwerMedia != null && x < anwerMedia.length; x++) {
                    var mediaFile = anwerMedia[x];
                    html += renderMediaAnswer(mediaFile);
                }
                html += '</div>'
                debugger
                var textBtn = ((content == null || content == '') && point == 0) ? "chấm điểm" : "chấm lại";
                var updatEvent = "updatePoint(this,'" + item.ExamDetailID + "')";

                html += '<div style="margin-top:20px"><button type="button" class="btn btn-sm btn-success" onclick="' + updatEvent + '"> ' + textBtn + ' </button></div>';

                html += '</fieldset>';
                html += "<script>if (CKEDITOR) {CKEDITOR.replace('TEXT_CKEDITOR_" + item.ID + "',{extraPlugins: '',image2_alignClasses: ['image-align-left', 'image-align-center', 'image-align-right'],image2_disableResizer: true});}</script>";
            } else {
                if (point == 0 && content == null) {
                    html += '<fieldset class="answer-item col-md-12" id="essay-teacher-' + data.ID + '" style="padding:10px; border:1px solid #ccc">';
                    html += 'Chưa chấm điểm';
                    html += '</fieldset>';
                } else {
                    html += '<fieldset class="answer-item col-md-12" id="essay-teacher-' + item.ID + '" style="padding:10px; border:1px solid #ccc">';
                    html += '<div class="mb-3"> Điểm : <strong>' + point + '/' + item.MaxPoint + '</strong></div>';
                    html += '<i>Nhận xét của giáo viên :</i>';
                    var realContent = content == null ? "" : content;
                    html += '<div class="text-info">' + realContent + '</div>';
                    html += '</fieldset>';
                    if (anwerMedia) {
                        html += '<fieldset><div> File đính kèm : ';
                        for (var x = 0; anwerMedia != null && x < anwerMedia.length; x++) {
                            var mediaFile = anwerMedia[x];
                            html += renderMedia(mediaFile);
                        }
                        html += '</div></fieldset>';
                    }
                }
            }
            var description = "";
            if (item.Description != null)
                description = item.Description.replace(/\n/g, '<br/>').replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn");
            html += '<div class="explaination d-none text-info p-3"><i>' + description + '</i></div>';
            html += '</div></div>';
        }
        html += '</div></div>';

        //writeLog("renderESSAY", data);
        //var html = '<div class="part-column"><div class="part-box-header p-3">' + (data.Title == null ? '' : ('<h5 class="title">' + data.Title + '</h5>')) + '<div class="description">' + data.Description + '</div>' + renderMedia(data.Media) + '</div>';
        //html += '<div class="quiz-wrapper p-3">';
        //html += '<div class="quiz-item" id="' + data.ID + '" data-part-id="' + data.ID + '"></div>';
        //html += '<div class="answer-wrapper">';
        //html += '<div class="answer-content"><textarea data-part-id="' + data.ID + '" data-lesson-id="' + data.ParentID + '" data-type="ESSAY" id="essay-' + data.ID + '" class="form-control" row="3" placeholder="Answer" onfocusout="AnswerQuestion(this)"></textarea></div>';
        //html += '</div></div>';
        html += '</div></div>';
        return html;
    }

    var renderMedia = function (data) {
        if (data == null || data == void 0 || data == "") return "";
        var arr = data.Extension.split('/');
        if (arr.includes("video")) {
            return '<div class="media-holder "><video controls=""><source src="' + data.Path.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn") + '" type="' + data.Extension + '">Your browser does not support the video tag</video></div>';
        }
        if (arr.includes("audio")) {
            return '<div class="media-holder "><audio controls=""><source src="' + data.Path.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn") + '" type="' + data.Extension + '">Your browser does not support the audio tag</audio></div>'
        }
        if (arr.includes("image")) {
            return '<div class="media-holder "><img src="' + data.Path.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn") + '" class="img-fluid lazy" title="' + data.Name + '"></div>';
        }
        return '<div class="media-holder"><a target="_blank" href="' + data.Path + '" style="display:block">Download</a></div>';
    }

    var renderMediaAnswer = function (data) {
        if (data == null || data == void 0 || data == "") return "";
        var arr = data.Extension.split('/');
        if (arr.includes("video")) {
            return '<div class="media-holder"><video controls=""><source src="' + data.Path.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn") + '" type="' + data.Extension + '">Your browser does not support the video tag</video></div>';
        }
        if (arr.includes("audio")) {
            return '<div class="media-holder "><audio controls=""><source src="' + data.Path.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn") + '" type="' + data.Extension + '">Your browser does not support the audio tag</audio></div>'
        }
        if (arr.includes("image")) {
            return '<div class="media-holder "><img src="' + data.Path.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn") + '" class="img-fluid lazy" title="' + data.Name + '"></div>';
        }
        return '<div class="media-holder"><a target="_blank" href="' + data.Path + '" style="display:block">Download</a></div>';
    }

    var renderContent = function (data) {
        var tabList = '<div id="pills-tabContent" class="tab-content">';
        //var nav = '<div id="nav-menu" class="col-md-12 mb-3 pb-3 bd-bottom">';
        //var nav_bottom_wrapper = $('<div>', { id: "nav-menu", class: 'd-flex justify-content-between align-items-baseline' });
        //'<div id="nav-menu" class="col-md-12 mt-3 pt-3 bd-top">';
        //nav += '<div class="col-md-1 text-left d-inline-block"><button class="prevtab btn btn-success" data-toggle="tooltip" title="Phần trước" onclick="tab_goback()"><i class="fas fa-arrow-circle-left"></i></button></div>'; //left button

        //var quiz_counter = $('<div>', { id: 'quiz-counter-holder', class: "d-inline-block text-white font-weight-bold align-middle pl-2" });

        var lesson_info_holder = $('.top-menu[for=lesson-info]');
        var prev_btn = $('<button>', { class: "prevtab btn btn-primary m-2", disabled: "disabled", onclick: "window.PrevPart()" }).append($('<i>', { class: "fas fa-arrow-circle-left" })).append($("<span>", { class: "no-mobile ml-2" }).append("Câu trước"));
        lesson_info_holder.append(prev_btn);//.append(quiz_counter);
        //var ct_action_holder = $('<div>', { class: "text-center" });

        //if (_type != 1) {
        var explain_btn = $('<button>', { class: "btn btn-primary mt-2 mr-2 mb-2", onclick: "ToggleExplain(this)" }).append('<i class="fas fa-info-circle"></i>').append($("<span>", { class: "no-mobile ml-2" }).append("Giải thích"));
        //var golist_btn = $('<button>', { class: "btn btn-success pl-3 pr-3 ml-1", onclick: "GoList(this)", text: "Về danh sách" });
        var redo_btn = $('<button>', { class: "btn btn-primary mt-2 mb-2", onclick: "Redo(this)" }).append($("<i>", { "class": "fas fa-redo-alt" })).append($("<span>", { class: "no-mobile ml-2" }).append("Thực hiện lại"));
        lesson_info_holder.append(explain_btn).append(redo_btn);//.append(golist_btn);
        //}
        var next_btn_holder = $('<div>', { class: "text-right align-self-end" });
        var next_btn = $('<button>', { class: "nexttab btn btn-primary m-2", onclick: "window.NextPart()" }).append($('<i>', { class: "fas fa-arrow-circle-right" })).append($("<span>", { class: "no-mobile ml-2" }).append("Câu sau"));
        lesson_info_holder.append(next_btn);

        //nav_bottom_wrapper.append(prev_btn_holder).append(ct_action_holder).append(next_btn_holder);



        //nav += '<div class="lesson-tabs col-md-10 d-inline-block"><ul id="pills-tab" class="nav nav-pills compact" onclick="toggle_tab_compact()">';

        _totalPart = data.Part.length;
        if (_totalPart <= 1)
            next_btn.prop("disabled", true);
        //console.log(data.Part);
        for (var i = 0; i < data.Part.length; i++) {
            var item = data.Part[i];
            var icon = arrIcon[item.Type];
            if (icon == void 0) icon = arrIcon["TEXT"];
            var content = renderLessonPart(item, i, data.TemplateType);
            tabList += content;
            var title = item.Title == void 0 || item.Title == null || item.Title == "null" ? "" : item.Title;
        }

        tabList += '</div>';
        html = '<div class="lesson-body" id="' + data.ID + '"><div id="' + _idNaviton + '"></div>' + tabList + '</div>';
        //$('#lessonSummary').append(nav_bottom_wrapper);
        $('<div>', { id: 'quizIdx_holder' }).appendTo($('#lessonSummary'));
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
        $(".top-menu[for=lesson-info] .prevtab")
            .after($('<button>', { class: "quizNumber btn btn-primary mt-2 mr-2 mb-2", onclick: "window.ToggleNav(this)", tooltips: "Ẩn hiện bảng theo dõi" })
                .append($("<i>", { class: "fas fa-question-circle" })).append($("<span>", { class: "no-mobile ml-2" }).append("Danh sách câu trả lời")));
        $(".quizNumber").focus().click();
    }

    var redo = function () {
        document.location = config.url.exam + "/" + config.lesson.ID + "/" + config.exam.ClassSubjectID + "#redo";
    }

    var goList = function () {
        document.location = config.url.list + "/" + config.exam.ClassSubjectID + "#" + config.lesson.ChapterID;
    }

    var updatePoint = function (_this, id, obj) {
        if (obj == void 0) {
            var qID = _this.offsetParent.id.replace("essay-teacher-", "");
            var _url = config.url.point;
            var point = _this.offsetParent.querySelector('input[type="number"]').value;
            var answer = CKEDITOR.instances["TEXT_CKEDITOR_" + qID].getData();
            var isLAst = _this.offsetParent.dataset.last;
            //console.log(point, answer);

            var _form = new FormData();
            _form.append("ID", id);
            _form.append("Point", point);
            _form.append("RealAnswerValue", answer);
            _form.append("isLast", isLAst);

            var files = document.querySelector('[data-target="' + qID + '"]>input[type="file"]');
            if (files == null) {
                files = document.querySelector('[data-target="' + id + '"]>input[type="file"]');
            }
            if (files != null) {
                for (var x = 0; x < files.files.length; x++) {
                    _form.append("files", files.files[x]);
                }
            }

            var _ajax = new MyAjax();
            _ajax.proccess("POST", _url, _form).then(function (res) {
                var dataJson = JSON.parse(res);
                if (dataJson.Data != null && dataJson.Data.ExamID != void 0) {
                    _this.innerHTML = "chấm lại";
                    _this.offsetParent.querySelector('.alert').style.display = 'block';
                    var selection = document.querySelector("[data-id='" + qID + "']");
                    if (selection) {

                        selection.classList.remove(...["unchecked", "success", "danger"]);
                        if (point > 0) {
                            selection.classList.add(...["success", "checked"]);
                        }
                        else {
                            selection.classList.add(...["danger", "checked"]);
                        }
                    }
                }
                updateShowPoint(id);
            });
        } else {
            if (config.isTeacher) {
                var _url = config.url.point;
                var _form = new FormData();
                _form.append("ID", obj);
                _form.append("isLast", true);
                var _ajax = new MyAjax();
                _ajax.proccess("POST", _url, _form).then(function (res) {
                    var dataJson = JSON.parse(res);
                    if (dataJson.Data != null) {
                        var data = dataJson.Data;

                        var pointHTML = document.querySelector(".card-header .title>.point");
                        if (pointHTML != null) {
                            var curentPoint = pointHTML.querySelector('.current-point');
                            if (curentPoint != null) {
                                curentPoint.innerHTML = data.Point;
                            }
                        }

                    }
                });
            }
        }
    }

    var tinhLaiDiem = function (id) {

    }
    var getConfig = function () {
        return config;
    }
    var uploadFile = function (_that) {
        var parent = _that.parentElement;
        var file = parent.querySelector("input[type='file']");
        var preview = parent.querySelector(".preview");
        preview.innerHTML = "";
        if (file) {
            file.click();
            file.onchange = function () {
                var listFiles = file.files;
                for (var i = 0; i < listFiles.length; i++) {
                    var item = listFiles[i];
                    preview.innerHTML += '<div>' + item.name + '</div>';
                }
            }
        }
    }

    var togglePanelWidth = function (obj) {
        var parent = $(obj).offsetParent();
        if (parent.hasClass("col-md-6")) {
            parent.removeClass("col-md-6").addClass("col-md-2");
            parent.siblings().removeClass("col-md-6").addClass("col-md-10");
        }
        else {
            if (parent.hasClass("col-md-4")) {
                parent.removeClass("col-md-4").addClass("col-md-6");
                parent.siblings().removeClass("col-md-8").addClass("col-md-6");
            }
            else {
                parent.removeClass("col-md-2").addClass("col-md-4");
                parent.siblings().removeClass("col-md-10").addClass("col-md-8");
            }
        }
    }
    var validate = function (_this) {
        var value = _this.value;
        var max = _this.getAttribute('max');
        if (parseInt(value) > parseInt(max)) {
            alert("Điểm không thể lớn hơn " + max);
            _this.value = max;
        }
    }
    window.ExamReview = {} || ExamReview;
    ExamReview.onReady = onReady;
    window.PrevPart = prevPart;
    window.NextPart = nextPart;
    window.GoQuiz = goQuiz;
    window.ToggleExplain = toggleExplain;
    window.GoList = goList;
    window.Redo = redo;
    window.ToggleNav = toggleNav;
    window.updatePoint = updatePoint;
    window.tinhLaiDiem = tinhLaiDiem;
    window.uploadFile = uploadFile;
    window.togglePanelWidth = togglePanelWidth;
    window.validate = validate;
    return ExamReview;
}());

var isMobileDevice = function () {
    //return true;
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
        // true for mobile device
        //console.log("Mobile detected");
        return true;
    } else {
        // false for not mobile device
        //console.log("Desktop detected");
        return false;
    }
};
