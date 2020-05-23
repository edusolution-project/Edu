﻿var Debug = true;
var mod = {
    PREVIEW: "preview",
    TEACHERVIEW: "teacherview",
    TEACHEREDIT: "teacheredit",
    EDIT: "edit",
    STUDENT_EXAM: "studentexam",
    STUDENT_LECTURE: "studentlecture",
    STUDENT_REVIEW: "studentreview"
}

var TEMPLATE_TYPE = {
    EXAM: 2,
    LESSON: 1
}

var UIMode = {
    LECTURE_ONLY: "lecture",
    EXAM_ONLY: "exam",
    BOTH: "both"
}

var Lesson = (function () {

    var _totalPart = 0;

    var _data = null;

    var _UImode = null;

    var exam_timeout = null;

    var config = {
        container: "",
        mod: "",
        url: {
            //Teacher
            load: "", //all: load lesson data
            save: "", //edit: save lesson data
            list_part: "",//all: get all part
            load_part: "",//all: load lesson part detail
            save_part: "",//edit: save part,
            move_part: "",//change: change part position,
            del_part: "",
            import_quiz: "",
            export_quiztemp: "",
            //Student
            current: "", //exam: load current exam state
            start: "", //exam: start exam
            answer: "", //exam: set answer for question
            removeans: "", //exam: unset answer for question
            end: "", //exam: complete lesson
            review: "" //review: review result
        },
        overdue: false, //check if lesson is over
        lesson_id: "",
        class_id: "",
        exam_id: ""
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
        console.log(config.mod);
        switch (config.mod) {
            case mod.PREVIEW:
            case mod.TEACHEREDIT:
            case mod.TEACHERVIEW:
                renderStandardLayout();
                renderPreview();
                break;
            case mod.STUDENT_EXAM:
                renderExam();
                break;
            case mod.STUDENT_LECTURE:
                renderStandardLayout();
                renderLecture();
                break;
            case mod.STUDENT_REVIEW:
                renderStandardLayout();
                renderStudentReview();
                break;
        }
        window.getLocalData = getLocalData
        window.ShowFullScreen = showFullScreen;
        var hash = window.location.hash;
        if (hash.startsWith('#')) {
            hash = hash.split('#')[1]
            //console.log(hash)
            switch (hash) {
                case 'redo':
                    redoExam();
                    window.history.pushState({ "html": document.html, "pageTitle": document.title }, "", window.location.href.substr(0, window.location.href.indexOf('#')));
                    break;
            }
        }
    }

    var reloadData = function () {
        _data = null;
        _UImode = null;//reset UIMode
        onReady(config);
    }

    //local storage function
    var setLocalData = function (key, data) {
        var enData = b64EncodeUnicode(JSON.stringify(data));
        localStorage.setItem(config.lesson_id + "_" + config.class_subject_id + "_" + key, enData);
    }

    var getLocalData = function (key) {
        var data = localStorage.getItem(config.lesson_id + "_" + config.class_subject_id + "_" + key)
        if (data == null) return null;
        return JSON.parse(b64DecodeUnicode(data));
    }

    var removeLocalData = function (key) {
        return localStorage.removeItem(config.lesson_id + "_" + config.class_subject_id + "_" + key);
    }

    //function

    var isNull = function (data) {
        return data == null || data == void 0 || data == "" || data == typeof (void 0);
    }

    var countdown = function () {
        clearTimeout(exam_timeout);
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
                CompleteExam(true);
                return;
            }
        }
        var text = (minutes < 10 ? ("0" + minutes) : minutes) + ":" + (second < 10 ? ("0" + second) : second);
        $(".time-counter").text(text);
        exam_timeout = setTimeout(function () {
            countdown();
        }, 1000);
    }

    var stopCountdown = function () {
        clearTimeout(exam_timeout);
    }

    var resetLesson = function () {
        document.location = document.location.href.replace('#', '');
    }

    //Load Data
    var loadLesssonData = function (param, callback) {
        var formData = new FormData();
        if (param != null)
            Object.keys(param).forEach(e => formData.append(e, param[e]));
        Ajax(config.url.load, formData, "POST", true).then(function (res) {
            if (!isNull(res)) {
                _data = JSON.parse(res).Data;
                switch (config.mod) {
                    case mod.PREVIEW: //curriculum view
                    case mod.TEACHERVIEW:
                    case mod.TEACHEREDIT:
                        loadLessonParts({ LessonID: config.lesson_id, ClassID: config.class_id, ClassSubjectID: config.class_subject_id }, callback);
                        break;
                    case mod.STUDENT_REVIEW:
                        loadLessonParts({ LessonID: config.lesson_id, ClassID: config.class_id, ClassSubjectID: config.class_subject_id, ExamID: config.exam_id }, callback);
                        break;
                    default:
                        callback();
                        break;
                }
            }
        })
    }

    var loadLessonParts = function (param, callback) {
        //console.log(param);
        var formData = new FormData();
        if (param != null)
            Object.keys(param).forEach(e => formData.append(e, param[e]));
        Ajax(config.url.list_part, formData, "POST", true).then(function (res) {
            _data.Part = JSON.parse(res).Data;
            callback();
        });
    }

    //render content

    var renderStandardLayout = function (hideHeader) {
        var container = $('#' + config.container);
        container.empty();

        var lessonBox = $("<div>", { "class": "lesson lesson-box h-100 mod_" + config.mod });
        container.append(lessonBox);

        var lessonContainer = $("<div>", { "class": "lesson-container h-100" });
        lessonBox.append(lessonContainer);
        var lessonContent = $("<div>", { "class": "card h-100 border-0" });
        lessonContainer.append(lessonContent);

        //header row (contain title; lesson action - default hide)
        var lessonHeader = $("<div>", { "class": "card-header d-none", style: "display:none" });
        lessonContent.append(lessonHeader);

        //footer row (part navigation, question navigation... - default: hide)
        var lessonFooter = $('<div>', { "class": "card-footer", "style": "display:none" });
        lessonContent.append(lessonFooter);

        //main content (lesson content; part list...., 2 cols interface)
        var cardBody = $("<div>", { "class": "card-body position-absolute pb-0", "style": "top: 0; bottom: 0; left: 0; right: 0" });
        lessonContent.append(cardBody);

        var lessonBody = $("<div>", { "class": "lesson-body h-100", "id": config.lesson_id });
        cardBody.append(lessonBody);

        var partsHolder = $("<div>", { "id": "pills-tabContent", "class": "tab-content h-100 row" });
        //render 2 columns layout base on UIMode
        switch (_UImode) {
            case UIMode.EXAM_ONLY:
                partsHolder.append($("<div>", { "class": "col-md-4 h-100 main-column pr-2 pl-1", "id": "leftCol" }))
                    .append($("<div>", { "class": "col-md-8 h-100 main-column pr-1 pl-2", "id": "rightCol" }));
                break;
            default:
                partsHolder.append($("<div>", { "class": "col-md-6 h-100 main-column pr-2 pl-1", "id": "leftCol" }))
                    .append($("<div>", { "class": "col-md-6 h-100 main-column pr-1 pl-2", "id": "rightCol" }));
                break;
        }



        lessonBody.append(partsHolder);
        $('.main-column').addClass('scrollbar-outer').scrollbar();
    }

    var renderLessonData = function () {
        var lesson_action_holder = $('.top-menu[for=lesson-info]');
        if (isNull(_data)) {
            throw "No data";
        }
        var data = _data;

        var mainContainer = $('#' + config.container);

        var lessonHeader = mainContainer.find('.card-header');
        var lessonBody = mainContainer.find('.card-body');
        var lessonFooter = mainContainer.find('.card-footer');

        _totalPart = data.Part != null ? data.Part.length : 0;
        //header
        console.log(config.mod);
        switch (config.mod) {
            case mod.PREVIEW:
                var headerRow = $("<div>", { "class": "justify-content-between d-none" }).empty();
                //lessonHeader.show().append(headerRow);
                var title_wrapper = $("<div>", { "class": "lesson-header-title" });
                var title = $("<h5>");
                var titleText = $("<span>", { "class": "title-text", "text": data.Title })
                title.append(titleText);
                title_wrapper.append(title);
                headerRow.append(title_wrapper);

                if (data.TemplateType == TEMPLATE_TYPE.EXAM) {
                    if (data.Timer > 0) {
                        var titleTimer = $("<span>", { "class": "title-timer", "text": " - duration: " + data.Timer + "m" });
                        title.append(titleTimer);
                    }
                    if (data.Point > 0) {
                        var titlePoint = $("<span>", { "class": "title-point", "text": " (" + data.Point + "p)" });
                        title.append(titlePoint);
                    }
                }

                var lessonButton = $("<div>", { "class": "lesson-button" });
                var sort = $("<button>", { "class": "btn btn-primary btn-sort", "title": "Sắp xếp", "onclick": "SortPart()" });
                //var edit = $("<button>", { "class": "btn btn-primary btn-edit", "title": "Sửa", "data-toggle": "modal", "data-target": "#lessonModal", "onclick": "EditLesson('" + data.ID + "')" });
                var create = $("<button>", { "class": "btn btn-primary btn-add", "title": "Thêm", "data-toggle": "modal", "data-target": "#partModal", "onclick": "AddPart('" + data.ID + "','" + data.TemplateType + "')" });
                //var close = $("<button>", { "class": "btn btn-primary btn-close", "text": "X", "onclick": "render.resetLesson()" });
                //var remove = $("<button>", { "class": "btn btn-danger btn-remove", "title": "Remove" });

                var iconSort = $("<i>", { "class": "fas fa-sort mt-2 mb-2 mr-2" });
                //var iconEdit = $("<i>", { "class": "fas fa-edit mt-2 mb-2 mr-2" });
                var iconCreate = $("<i>", { "class": "fas fa-plus-square mt-2 mb-2 mr-2" });
                var iconTrash = $("<i>", { "class": "fas fa-trash mr-2" });
                lessonButton.append(iconSort);


                lessonButton.append(sort);
                sort.prepend(iconSort).append("Sắp xếp");


                if (!(_totalPart > 0)) {
                    $(sort).prop("disabled", true);
                }

                //lessonButton.append(edit);
                //edit.prepend(iconEdit).append("Sửa");
                lessonButton.append(create);
                create.prepend(iconCreate).append("Thêm nội dung");
                //lessonButton.append(remove); //removeLesson
                //remove.append(iconTrash);
                //headerRow.append(lessonButton);

                lesson_action_holder.empty().prepend(lessonButton);

                break;
            case mod.TEACHERVIEW:
                var headerRow = $("<div>", { "class": "d-flex justify-content-between" });
                //lessonHeader.show().append(headerRow);

                var title_wrapper = $("<div>", { "class": "lesson-header-title" });
                var title = $("<h5>");
                var titleText = $("<span>", { "class": "title-text", "text": data.Title })
                title.append(titleText);
                title_wrapper.append(title);
                headerRow.append(title_wrapper);

                if (data.TemplateType == TEMPLATE_TYPE.EXAM) {
                    if (data.Timer > 0) {
                        var titleTimer = $("<span>", { "class": "title-timer", "text": " - thời lượng: " + data.Timer + "m" });
                        title.append(titleTimer);
                    }
                    if (data.Point > 1) {
                        var titlePoint = $("<span>", { "class": "title-point", "text": " (" + data.Point + "p)" });
                        title.append(titlePoint);
                    }
                }

                var lessonButton = $("<div>", { "class": "lesson-button" });
                var toggleMode = $("<button>", { "class": "btn btn-primary btn-add mt-2 mb-2 mr-2", "title": "Bật chế đô sửa", "onclick": "SwitchMode('" + mod.TEACHEREDIT + "')" });
                var iconToggle = $("<i>", { "class": "fas fa-edit mr-2" });

                lessonButton.append(toggleMode);
                toggleMode.append(iconToggle).append("Sửa");
                //headerRow.append(lessonButton);
                lesson_action_holder.prepend(lessonButton);
                break;
            case mod.TEACHEREDIT:
                var headerRow = $("<div>", { "class": "d-flex justify-content-between" }).empty();
                lessonHeader.show().append(headerRow);

                var title_wrapper = $("<div>", { "class": "lesson-header-title" });
                var title = $("<h5>");
                var titleText = $("<span>", { "class": "title-text", "text": data.Title })
                title.append(titleText);
                title_wrapper.append(title);
                headerRow.append(title_wrapper);

                if (data.TemplateType == TEMPLATE_TYPE.EXAM) {
                    if (data.Timer > 0) {
                        var titleTimer = $("<span>", { "class": "title-timer", "text": " - duration: " + data.Timer + "m" });
                        title.append(titleTimer);
                    }
                    if (data.Point > 0) {
                        var titlePoint = $("<span>", { "class": "title-point", "text": " (" + data.Point + "đ)" });
                        title.append(titlePoint);
                    }
                }

                var lessonButton = $("<div>", { "class": "lesson-button" });
                var sort = $("<button>", { "class": "btn btn-primary btn-sort mt-2 mb-2 mr-2", "title": "Sort", "onclick": "SortPart()" });
                var create = $("<button>", { "class": "btn btn-primary btn-add mt-2 mb-2 mr-2", "title": "Add", "data-toggle": "modal", "data-target": "#partModal", "onclick": "AddPart('" + data.ID + "','" + data.TemplateType + "')" });
                var toggleMode = $("<button>", { "class": "btn btn-primary btn-add mt-2 mb-2 mr-2", "title": "Về chế độ xem", "onclick": "SwitchMode('" + mod.TEACHERVIEW + "')" });

                var iconSort = $("<i>", { "class": "fas fa-sort mr-2" });
                var iconCreate = $("<i>", { "class": "fas fa-plus-square mr-2" });
                var iconToggle = $("<i>", { "class": "fas fa-eye mr-2" });

                if (!(_totalPart > 1)) {
                    $(sort).prop("disabled", true);
                }

                lessonButton.append(toggleMode);
                toggleMode.append(iconToggle).append("Xem");
                lessonButton.append(sort);
                sort.append(iconSort).append("Sắp xếp");
                lessonButton.append(create);
                create.append(iconCreate).append("Thêm nội dung");
                //headerRow.append(lessonButton);
                lesson_action_holder.find(".lesson-button").remove();
                lesson_action_holder.append(lessonButton);
                break;
            case mod.STUDENT_EXAM:
            case mod.STUDENT_REVIEW:
                lessonBody.css('top', 0);
                //no header
                break;
            case mod.REVIEW:
                lessonBody.css('top', 0);
                //no header
                break;
        }

        for (var i = 0; data.Part != null && i < data.Part.length; i++) {

            var item = data.Part[i];
            switch (item.Type) {
                case "QUIZ1":
                case "QUIZ2":
                case "QUIZ3":
                case "ESSAY":
                    switchUIMode(UIMode.EXAM_ONLY);
                    break;
                default:
                    switchUIMode(UIMode.LECTURE_ONLY);
            }
        }
        console.log(_UImode);
        $('.mod_' + config.mod).addClass("uimod_" + _UImode);
        //body
        switch (config.mod) {
            case mod.PREVIEW:
            case mod.TEACHERVIEW:
            case mod.TEACHEREDIT:
                var partMenu = $("<div>", { "id": "part-menu", "class": "w-100", "style": "display:none;" });
                lessonBody.append(partMenu);
                var lessontabs = $("<div>", { "class": "lesson-tabs" });
                partMenu.append(lessontabs);
                var tabs = $("<ul>", { "id": "pills-tab", "class": "nav flex-column nav-pills", "role": "tablist", "aria-orientation": "vertical" });
                lessontabs.append(tabs);
                for (var i = 0; data.Part != null && i < data.Part.length; i++) {
                    var item = data.Part[i];
                    renderPreviewPart(item);
                }
                if (data.Part != null && data.Part.length == 1) {
                    $('.fas.fa-caret-down:first').click();
                }
                if (_UImode == UIMode.EXAM_ONLY) {
                    $('#rightCol .tab-pane').each(function () {
                        var media = null;
                        var html = null;
                        if ($(this).find(".QUIZ3").length > 0) {
                            $(this).addClass("h-100");
                            media = $(this).find(".Q3_absrow > .media-holder:first");
                        }
                        else {
                            media = $(this).find(".quiz-wrapper > .media-holder");
                        }
                        html = $(this).find(".part-description");
                        $(this).addClass("m-0");
                        $(this).clone().removeClass('tab-pane').addClass('tab-pane-quiz')
                            .empty().append($(this).find('.part-box-header')).append(media).append(html)
                            .appendTo('#leftCol')
                    })
                }
                break;
            case mod.STUDENT_EXAM:
                console.log("student exam");
                var partMenu = $("<div>", { "id": "part-menu", "class": "w-100", "style": "display:none;" });
                lessonBody.append(partMenu);
                var lessontabs = $("<div>", { "class": "lesson-tabs" });
                partMenu.append(lessontabs);
                var tabs = $("<ul>", { "id": "pills-tab", "class": "nav flex-column nav-pills", "role": "tablist", "aria-orientation": "vertical" });
                lessontabs.append(tabs);
                for (var i = 0; data.Part != null && i < data.Part.length; i++) {
                    var item = data.Part[i];
                    renderStudentPart(item);
                }
                if (_UImode == UIMode.EXAM_ONLY) {
                    $('#rightCol .tab-pane').each(function () {
                        var media = null;
                        var html = null;
                        if ($(this).find(".QUIZ3").length > 0) {
                            $(this).addClass("h-100");
                            media = $(this).find(".Q3_absrow > .media-holder:first");
                        }
                        else {
                            media = $(this).find(".quiz-wrapper > .media-holder");
                        }
                        html = $(this).find(".part-description");
                        $(this).addClass("m-0");
                        $(this).clone().removeClass('tab-pane').addClass('tab-pane-quiz')
                            .empty().append($(this).find('.part-box-header')).append(media).append(html)
                            .appendTo('#leftCol')
                    })
                }
                break;
            case mod.STUDENT_LECTURE:

                var partMenu = $("<div>", { "id": "part-menu", "class": "w-100", "style": "display:none;" });
                lessonBody.append(partMenu);
                var lessontabs = $("<div>", { "class": "lesson-tabs" });
                partMenu.append(lessontabs);
                var tabs = $("<ul>", { "id": "pills-tab", "class": "nav flex-column nav-pills", "role": "tablist", "aria-orientation": "vertical" });
                lessontabs.append(tabs);
                for (var i = 0; data.Part != null && i < data.Part.length; i++) {
                    var item = data.Part[i];
                    renderStudentPart(item);
                }
                if (data.Part != null && data.Part.length == 1) {
                    $('.fas.fa-caret-down:first').click();
                }
                switch (_UImode) {
                    case UIMode.EXAM_ONLY:
                    case UIMode.BOTH:
                        var dataform = new FormData();
                        dataform.append("ClassID", config.class_id);
                        dataform.append("ClassSubjectID", config.class_subject_id);
                        dataform.append("LessonID", config.lesson_id);
                        if ($('#' + config.container).find("#ExamID").length == 0) {
                            $('#' + config.container).prepend($("<input>", { type: "hidden", name: "ExamID", id: "ExamID" }));
                        }
                        if (!checkExam()) {
                            localStorage.clear();
                        }

                        var continue_exam = false;
                        //get lastest exam data from server
                        Ajax(config.url.current, dataform, "POST", true).then(function (res) {
                            var exam;
                            try {
                                var data = JSON.parse(res);
                                exam = data.exam
                            } catch (e) {
                                console.log(e)
                            }
                            //console.log(exam);
                            //console.log(getLocalData("CurrentExam"));
                            //if no data is found => new attempt => clear all local storage
                            if (exam == null) {
                                localStorage.clear();
                                console.log("New Fresh Exam");
                                renderLectureExam(exam, false);
                            }
                            else {
                                //
                                if (isNull(getLocalData("CurrentExam")) || (getLocalData("CurrentExam") != exam.ID)) //display last result & render new exam
                                {
                                    //console.log(data);
                                    localStorage.clear();
                                    console.log("New Exam");
                                    //console.log(getLocalData("CurrentExam"))
                                    renderLectureExam(exam, false);
                                }
                                else {
                                    console.log("Exam Continue")
                                    setLocalData("CurrentExam", exam.ID);
                                    $('#ExamID').val(exam.ID);
                                    renderLectureExam(exam, true);
                                }
                            }
                        });
                        break;
                    //case UIMode.EXAM_ONLY:
                    //$('#rightCol .tab-pane').each(function () {
                    //    var media = null;
                    //    var html = null;
                    //    if ($(this).find(".QUIZ3").length > 0) {
                    //        $(this).addClass("h-100");
                    //        media = $(this).find(".Q3_absrow > .media-holder:first");
                    //    }
                    //    else {
                    //        media = $(this).find(".quiz-wrapper > .media-holder");
                    //    }
                    //    html = $(this).find(".part-description");
                    //    $(this).addClass("m-0");
                    //    $(this).clone().removeClass('tab-pane').addClass('tab-pane-quiz')
                    //        .empty().append($(this).find('.part-box-header')).append(media).append(html)
                    //        .appendTo('#leftCol')
                    //})

                    //break;
                }

                break;
            default:
                break;
        }

        //footer: navigation
        switch (config.mod) {
            case mod.PREVIEW:
            case mod.TEACHERVIEW:
            case mod.TEACHEREDIT:
                if (_UImode == UIMode.EXAM_ONLY) {
                    var lesson_action_holder = $('.top-menu[for=lesson-info]');
                    var nav_bottom = $('<div>', { "class": "row" });

                    var prevtab = $("<button>", { "class": "prevtab btn btn-primary mt-2 mb-2 mr-2", "title": "Câu trước", "onclick": "PrevPart()" });
                    var iconprev = $("<i>", { "class": "fas fa-arrow-circle-left mr-2" });
                    var nexttab = $("<button>", { "class": "nexttab btn btn-primary mt-2 mb-2 ml-2", "title": "Câu tiếp", "onclick": "NextPart()" });
                    var iconnext = $("<i>", { "class": "fas fa-arrow-circle-right ml-2" });
                    prevtab.append(iconprev).append("Câu trước");
                    nexttab.append(iconnext).prepend("Câu sau");


                    prevtab.prop("disabled", true);

                    if (_totalPart <= 1)
                        nexttab.prop("disabled", true);


                    //var _footerLeft = $('<div>', { "class": "col-md-2 text-left" });
                    //_footerLeft.append(prevtab);
                    //var _footerRight = $('<div>', { "class": "col-md-2 text-right" });
                    //var _footerCenter = $('<div>', { "class": "col-md-8 text-center" });
                    var btnExplain = $("<button>", { "class": "btn btn-primary mt-2 mb-2", "title": "Bật/tắt giải thích", "onclick": "ToggleExplanation(this)" }).append('<i class="fas fa-info-circle mr-2"></i>').append("Giải thích");
                    //_footerCenter.append(btnExplain);

                    lesson_action_holder.find("> button").remove();
                    lesson_action_holder.append(prevtab).append(btnExplain).append(nexttab);

                    //lessonFooter.show().append(nav_bottom);
                }
                else {
                    lessonFooter.hide();
                }
                break;
            case mod.STUDENT_EXAM:

                //if (_UImode == UIMode.EXAM_ONLY) {
                //var nav_bottom = $('<div>', { "class": "d-flex justify-content-between" });
                var nav_bottom = lesson_action_holder
                nav_bottom.empty();

                var prevtab = $("<button>", { "class": "prevtab btn btn-primary m-2", "title": "Câu trước", "onclick": "PrevPart()" });
                var iconprev = $("<i>", { "class": "fas fa-arrow-circle-left mr-2" });
                var nexttab = $("<button>", { "class": "nexttab btn btn-primary m-2", "title": "Câu tiếp", "onclick": "NextPart()" });
                var iconnext = $("<i>", { "class": "fas fa-arrow-circle-right mr-2" });
                prevtab.append(iconprev).append("Câu trước");;
                nexttab.append(iconnext).append("Câu sau");;


                prevtab.prop("disabled", true);

                if (_totalPart <= 1)
                    nexttab.prop("disabled", true);

                //var _footerLeft = $('<div>', { "class": "text-left" });
                //_footerLeft.append(prevtab);
                //var _footerRight = $('<div>', { "class": "text-right" });
                //_footerRight.append(nexttab);
                //var _footerCenter = $('<div>', { "class": "text-center" });

                nav_bottom.append(prevtab);

                //lessonFooter.show().append(nav_bottom);
                lessonFooter.append($('<div>', { id: 'quizIdx_holder' }));
                lessonFooter.addClass('expand');

                var quiz_counter = $('<div>', { id: 'quiz-counter-holder', class: "text-white font-weight-bold align-middle" });
                nav_bottom.append(quiz_counter);

                var complete_btn = $('<button>', { class: "btn btn-primary mt-2 mb-2", onclick: "CompleteExam()" }).append('<i class="fas fa-save mr-2"></i>').append("Nộp bài");;
                var timer = $('<div>', { id: 'bottom-counter', class: "font-weight-bold m-2 text-danger", style: "font-size:200%" })
                    .append('<i class="far fa-clock mr-2" style="font-size:85%"></i>')
                    .append($("<span>", { class: "time-counter " }));


                nav_bottom.prepend(timer).append(complete_btn);

                renderQuizCounter();
                $(".time-counter").html(getLocalData("Timer"));
                countdown();
                nav_bottom.append(nexttab);
                //}
                break;
            case mod.REVIEW:
                break;
        }
    }

    var switchUIMode = function (mode) {
        var mainContainer = $('#' + config.container);
        var leftCol = mainContainer.find('#leftCol');
        var rightCol = mainContainer.find('#rightCol');

        if (_UImode == UIMode.BOTH) return;
        if (mode == _UImode) return;
        if (isNull(_UImode))
            _UImode = mode;
        else
            _UImode = UIMode.BOTH;
        switch (_UImode) {
            case UIMode.LECTURE_ONLY:
                leftCol.parent().removeClass("col-md-6").removeClass("col-md-12").addClass("col-md-12").show();//scrollwrapper
                rightCol.parent().hide();
                break;
            case UIMode.EXAM_ONLY:
                leftCol.parent().removeClass("col-md-6").removeClass("col-md-12").addClass("col-md-4").show();
                rightCol.parent().removeClass("col-md-6").removeClass("col-md-12").addClass("col-md-8").show();
                break;
            case UIMode.BOTH:
                leftCol.parent().removeClass("col-md-6").removeClass("col-md-4").removeClass("col-md-12").addClass("col-md-6").show();
                rightCol.parent().removeClass("col-md-6").removeClass("col-md-8").removeClass("col-md-12").addClass("col-md-6").show();
                break;
        }
    }

    var switchMode = function (mode) {
        config.mod = mode;
        $('.top-menu[for=lesson-info]').empty();
        reloadData();
    }

    //Preview: view + edit
    var renderPreview = function () {
        //Prepare data
        if (isNull(_data)) {
            loadLesssonData({ ID: config.lesson_id }, renderLessonData);
        }
        else {
            //Render Content
            renderLessonData();
        }
    }

    var renderPreviewPart = function (data) {
        var mainContainer = $('#' + config.container);
        var leftCol = mainContainer.find('#leftCol');
        var rightCol = mainContainer.find('#rightCol');

        var time = "", point = "";

        if (data.Timer > 0) {
            time = " (" + data.Timer + "m)";
        }
        if (data.Point > 0) {
            point = " (" + data.Point + "p)";
        }

        var container = $(leftCol);
        if (data.Type.toString().startsWith("QUIZ") || data.Type == "ESSAY")
            container = $(rightCol);
        var listPartContainer = $("#part-menu #pills-tab");

        //tabs
        var lessonitem = null;
        if ($('li #pills-' + data.ID).length > 0) {
            lessonitem = $('li #pills-' + data.ID).parent().empty()
        }
        else {
            lessonitem = $("<li>", { "class": "nav-item" });
            listPartContainer.append(lessonitem);
        }

        var itemtitle = $("<a>", { "id": "pills-" + data.ID, "class": "nav-link", "data-toggle": "pill", "href": "#pills-part-" + data.ID, "role": "tab", "aria-controls": "pills-" + data.ID, "aria-selected": "false", "text": data.Title });
        lessonitem.append(itemtitle);

        var tabsitem = null;

        if ($('#pills-part-' + data.ID).length > 0) {
            //clear old-tab
            tabsitem = $('#pills-part-' + data.ID).empty();
        }
        else
            tabsitem = $("<div>", { "id": "pills-part-" + data.ID, "class": "tab-pane" + (_UImode == UIMode.EXAM_ONLY ? " hide" : "") + " w-100", "role": "tabpanel", "aria-labelledby": "pills-" + data.ID });

        var itembox = $("<div>", { "class": "part-box " + data.Type, "id": data.ID });
        tabsitem.append(itembox);

        var ItemRow = $("<div>", { "class": "row " + (_UImode == UIMode.EXAM_ONLY ? " Q3_absrow" : "") });

        var boxHeader = $("<div>", { "class": "part-box-header row" });

        switch (config.mod) {
            case mod.PREVIEW:
            case mod.TEACHEREDIT:
                boxHeader.append($("<h5>", {
                    "class": "title col-md-10 font-weight-bold", "text": (data.Title == null ? "" : data.Title) + time
                    //+ point
                }));

                var iconEdit = $("<i>", { "class": "fas fa-edit" });
                var iconTrash = $("<i>", { "class": "fas fa-trash" });

                var boxButton = $("<div>", { "class": "text-right col-md-2" });
                boxButton.append($("<button>", { "class": "btn btn-primary btn-sm mr-1 ml-1", "style": "width: 40px", "title": "Sửa", "data-toggle": "modal", "data-target": "#partModal", "onclick": "EditPart('" + data.ID + "')" }).append(iconEdit))
                boxButton.append($("<button>", { "class": "btn btn-danger btn-sm mr-1 ml-1", "style": "width: 40px", "title": "Xóa", "onclick": "RemovePart('" + data.ID + "')" }).append(iconTrash));
                boxHeader.append(boxButton);
                break;
            default:
                boxHeader.append($("<h5>", {
                    "class": "title col-md-12", "text": (data.Title == null ? "" : data.Title) + time
                    //+ point
                }));
                break;
        }

        itembox.append(boxHeader);


        var collapseSwitch = $("<i>", { class: "fas fa-caret-down pl-2 pr-2 pt-1 pb-1", style: "cursor:pointer", onclick: "toggleExpand(this)" });
        //$(collapseSwitch).click(function () {
        //    toggleExpand(this);
        //});

        //itembox.append(ItemRow);
        switch (data.Type) {
            case "TEXT":
                //boxHeader.find(".title").append(collapseSwitch);
                var itemBody = $("<div>", { "class": "content-wrapper" });
                if (data.Description != null) {
                    itemBody.append($("<div>", { "class": "doc-content" }).html(data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn")));
                }
                itemtitle.prepend($("<i>", { "class": "far fa-file-word" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                break;
            case "IMG":
                boxHeader.find(".title").append(collapseSwitch);
                var itemBody = $("<div>", { "class": "media-wrapper collapsable collapse" });
                if (data.Description != null) itemBody.append($("<div>", { "class": "d-flex justify-content-center" }).append($("<div>", { "class": "part-description" }).html(data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn"))));
                renderMediaContent(data, itemBody, "IMG");
                itemtitle.prepend($("<i>", { "class": "fas fa-file-image" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                break;
            case "AUDIO":
                boxHeader.find(".title").append(collapseSwitch);
                var itemBody = $("<div>", { "class": "media-wrapper collapsable collapse" });
                if (data.Description != null)
                    itemBody.append($("<div>", { "class": "d-flex justify-content-center" }).append($("<div>", { "class": "part-description" }).html(data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn"))));
                renderMediaContent(data, itemBody, "AUDIO");
                itemtitle.prepend($("<i>", { "class": "fas fa-music" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                break;
            case "VIDEO":
                boxHeader.find(".title").append(collapseSwitch);
                var itemBody = $("<div>", { "class": "media-wrapper collapsable collapse" });
                if (data.Description != null)
                    itemBody.append($("<div>", { "class": "d-flex justify-content-center" }).append($("<div>", { "class": "part-description" }).html(data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn"))));
                renderMediaContent(data, itemBody, "VIDEO");
                itemtitle.prepend($("<i>", { "class": "far fa-play-circle" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                break;
            case "DOC":
                boxHeader.find(".title").append(collapseSwitch);
                var itemBody = $("<div>", { "class": "media-wrapper collapsable collapse" });
                if (data.Description != null)
                    itemBody.append($("<div>", { "class": "d-flex justify-content-center" }).append($("<div>", { "class": "part-description" }).html(data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn"))));
                renderMediaContent(data, itemBody, "DOC");
                itemtitle.prepend($("<i>", { "class": "fas fa-file-word" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                break;
            case "QUIZ1":
                var itemBody = $("<div>", { "class": "quiz-wrapper" });
                itemtitle.prepend($("<i>", { "class": "fab fa-leanpub" }));
                itembox.append(itemBody);
                renderMediaContent(data, itemBody, "");
                container.append(tabsitem);
                //Render Description
                if (data.Description != null) {
                    itemBody.append($("<div>", { "class": "part-description" }).html(data.Description));
                }
                //Render Question
                totalQuiz = data.Questions.length;
                for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
                    var item = data.Questions[i];
                    renderPreviewQuestion(item, data.Type);
                }
                break;
            case "QUIZ2":
                var itemBody = $("<div>", { "class": "quiz-wrapper" });
                itemtitle.prepend($("<i>", { "class": "fab fa-leanpub" }));
                itembox.append(itemBody);
                renderMediaContent(data, itemBody, "");
                container.append(tabsitem);
                //Render Description
                if (data.Description != null) {
                    itemBody.append($("<div>", { "class": "part-description" }).html(data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn")));
                }
                //Render Question
                totalQuiz = data.Questions.length;
                var fillquizs = itembox.find("fillquiz");
                for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {

                    var item = data.Questions[i];
                    //console.log(item);
                    try {
                        var input = $(fillquizs[i]).find("input");
                        $(input).attr("dsp", item.Content);
                        var answers = [];
                        if (item.Answers.length > 0) {
                            for (var j = 0; j < item.Answers.length; j++)
                                answers.push(item.Answers[j].Content);
                            var content = answers.join(" | ");
                            var size = 5;
                            if (content.length > size) size = content.length;
                            $(input).attr("placeholder", content).attr("size", size);
                        }
                    }
                    catch (e) {
                        console.log("No placeholder found");
                    }
                }
                break;
            case "QUIZ3":
                if (data.Description != null) {
                    itembox.append($("<div>", { "class": "part-description" }).html(data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn")));
                }
                itembox.append(ItemRow);
                var itemBody = $("<div>", { "class": "quiz-wrapper col-8" });
                itemtitle.prepend($("<i>", { "class": "fab fa-leanpub" }));
                renderMediaContent(data, ItemRow, "");
                //console.log(ItemRow);
                ItemRow.append(itemBody);

                ItemRow.find(".media-holder").addClass("col-12");
                var answers_box = $("<div>", { "class": "answer-wrapper no-child col-4", "data-part-id": data.ID });
                ItemRow.append(answers_box);
                $(answers_box).droppable({
                    tolerance: "intersect",
                    accept: ".answer-item",
                    activeClass: "hasAnswer",
                    hoverClass: "answerHover",
                    drop: function (event, ui) {
                        var prevHolder = ui.helper.data('parent');
                        $(prevHolder).find(".placeholder").show();
                        $(this).append($(ui.draggable));
                    }
                });
                //console.log(itemBody);
                //console.log(tabsitem);
                //console.log(container);
                container.append(tabsitem);
                //Render Question
                totalQuiz = data.Questions.length;
                for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
                    var item = data.Questions[i];
                    renderPreviewQuestion(item, data.Type);
                }
                break;
            case "ESSAY":
                var itemBody = $("<div>", { "class": "content-wrapper" });
                if (data.Description != null) {
                    itemBody.append($("<div>", { "class": "part-description" }).html(data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn")));
                }
                itemtitle.prepend($("<i>", { "class": "fab fa-leanpub" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                var itemBody = $("<div>", { "class": "quiz-wrapper" });
                itembox.append(itemBody);

                renderMediaContent(data, itemBody, "");
                totalQuiz = data.Questions.length;
                for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
                    var item = data.Questions[i];
                    renderPreviewQuestion(item, data.Type);
                }
                break;
        }

        itembox.find('iframe,embed,video').each(function (i, item) {
            //if (!$(item)[0].hasAttribute('allow')) {
            //    $(item).attr('allow', 'fullscreen');
            //    $(item).attr('allowfullscreen', 'true');
            //}
            $(item).wrap($('<div>', {
                class: 'fullscreenSupport d-inline-block'
            }));
            $(item).parent().append($('<div>').append($('<button>', {
                class: "btn btn-primary mt-1", click: function () {
                    ShowFullScreen(this)
                }
            }).append('<i class="ic fa fa-expand mr-2"></i>').append('Xem toàn màn hình')))
        });

        if (typeof (WirisPlugin) != 'undefined')
            itembox.html(WirisPlugin.Parser.initParse(itembox.html()));

        if (listPartContainer.find(".nav-item").length == 1) {
            itemtitle.addClass("active");
            tabsitem.addClass("show active");
        }
        //$('.btn[title]').tooltip({
        //    trigger: 'hover'
        //});

        $('.Q3_absrow .quiz-wrapper').addClass('h-100').addClass('scrollbar-outer').scrollbar();
        $('.Q3_absrow .answer-wrapper').addClass('h-100').addClass('scrollbar-outer').scrollbar();

        //renderMath

        startDragDrop();
    }

    var showFullScreen = function (obj) {
        $(obj).parent().siblings('iframe,embed,video')[0].webkitRequestFullscreen();
    }

    var renderPreviewQuestion = function (data, template) {
        //render question
        var point = "";

        if (data.Point > 0) {
            point = " (" + data.Point + "p)";
        }
        switch (template) {
            case "QUIZ2":
                //change here
                var container = $("#" + data.ParentID + " .quiz-wrapper");
                var quizitem = $("<div>", { "class": "quiz-item", "id": data.ID, "data-part-id": data.ParentID });
                var boxHeader = $("<div>", { "class": "quiz-box-header" });
                boxHeader.append($("<div>", {
                    "class": "quiz-text", "html": breakLine(data.Content)
                    //+ point
                }));
                renderMediaContent(data, boxHeader);
                quizitem.append(boxHeader);

                container.append(quizitem);
                var answer_wrapper = $("<div>", { "class": "answer-wrapper" });

                quizitem.append(answer_wrapper);

                if (data.Description !== null) {
                    var extend = $("<div>", { "class": "quiz-extend", "html": breakLine(data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn")) });
                    quizitem.append(extend);
                }

                for (var i = 0; data.Answers != null && i < data.Answers.length; i++) {
                    var item = data.Answers[i];
                    renderPreviewAnswer(item, template);
                }
                break;
            case "QUIZ3":
                var container = $("#" + data.ParentID + " .quiz-wrapper");
                //console.log(container);
                var quizitem = $("<div>", { "class": "quiz-item", "id": data.ID, "data-part-id": data.ParentID });

                var quiz_part = $("<div>", { "class": "quiz-pane col-6" });
                var answer_part = $("<div>", {
                    "class": "answer-pane no-child col-6",
                    "data-question-id": data.ID,
                    "data-part-id": data.ParentID,
                    "data-lesson-id": config.lesson_id,
                    "data-type": template
                });
                quizitem.append(quiz_part);
                quizitem.append(answer_part);
                if (data.Description !== null) {
                    var extend = $("<div>", { "class": "quiz-extend", "html": breakLine(data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn")) });
                    quizitem.append(extend);
                }

                var pane_item = $("<div>", { "class": "pane-item" });
                if (data.Media == null) {
                    pane_item.append($("<div>", {
                        "class": "quiz-text", "html": breakLine(data.Content)
                        //+ point
                    }));
                } else {
                    renderMediaContent(data, pane_item);
                }

                quiz_part.append(pane_item);
                container.append(quizitem);

                $(answer_part).droppable({
                    tolerance: "intersect",
                    accept: ".answer-item",
                    activeClass: "hasAnswer",
                    hoverClass: "answerHover",
                    drop: function (event, ui) {
                        $(this).find(".placeholder").hide();
                        var prevHolder = ui.helper.data('parent');

                        if ($(this).find(".answer-item").length > 0) {//remove all answer to box
                            //$(container).siblings(".answer-wrapper").append($(this).find(".answer-item"));
                            $(prevHolder).append($(this).find(".answer-item"));
                        }
                        else {
                            $(prevHolder).find(".placeholder").show();
                        };
                        $(this).append($(ui.draggable));
                    }
                });

                for (var i = 0; data.Answers != null && i < data.Answers.length; i++) {
                    var item = data.Answers[i];
                    renderPreviewAnswer(item, template);
                }
                break;
            default:
                var container = $("#" + data.ParentID + " .quiz-wrapper");

                var itembox = $("<div>", { "class": "quiz-item", "id": data.ID, "data-part-id": data.ParentID });
                var boxHeader = $("<div>", { "class": "quiz-box-header" });
                if (data.Content != null)
                    boxHeader.append($("<h5>", {
                        "class": "title", "html": breakLine(data.Content)
                        //+ point
                    }));
                else
                    boxHeader.append($("<h5>", { "class": "title", "text": point }));

                renderMediaContent(data, boxHeader);

                itembox.append(boxHeader);
                var answer_wrapper = $("<div>", { "class": "answer-wrapper" });

                itembox.append(answer_wrapper);

                if (data.Description !== null) {
                    var extend = $("<div>", { "class": "quiz-extend", "html": breakLine(data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn")) });
                    itembox.append(extend);
                }

                container.append(itembox);

                if (template == "ESSAY") {
                    itembox.append(
                        $("<div>", { class: "mb-2" }).append(
                            $("<textarea>", { "id": "editor", "rows": "5", "name": "Description", "class": "input-text form-control", "placeholder": "Enter content" }))
                    );
                    if (data != null && data.Description != null)
                        itembox.find("[name=Description]").val(data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn"));
                    CKEDITOR.replace("editor");
                    itembox.append($("<div>", { class: "mb-2 text-right" })
                        .append($('<button>', { 'class': 'btn btn-primary mr-2', text: 'File đính kèm', disabled: true }))
                        .append($('<button>', { 'class': 'btn btn-primary', text: 'Lưu', disabled: true }))
                    );
                }
                else {
                    //Render Answer
                    for (var i = 0; data.Answers != null && i < data.Answers.length; i++) {
                        var item = data.Answers[i];
                        renderPreviewAnswer(item, template);
                    }
                }

                break;
        }
    }

    var renderPreviewAnswer = function (data, template) {
        var container = $("#" + data.ParentID + " .answer-wrapper");
        var answer = $("<fieldset>", { "class": "answer-item", id: data.ID });
        switch (template) {
            case "QUIZ2":
                //no more use
                if ($(container).find(".answer-item").length == 0) {
                    answer.append($("<input>", { "type": "text", "class": "input-text answer-text form-control", "placeholder": data.Content }));
                    container.append(answer);
                }
                else {
                    var oldval = $(container).find(".answer-text").attr("placeholder");
                    $(container).find(".answer-text").attr("placeholder", oldval + " | " + data.Content);
                }
                break;
            case "QUIZ3":
                var placeholder = $("#" + data.ParentID).find(".answer-pane");
                $(placeholder).removeClass("no-child");
                placeholder.empty().append($("<div>", { "class": "pane-item placeholder", "text": "Drop your answer here" }));
                container = $("#" + data.ParentID).parent().siblings(".answer-wrapper");

                if (data.Content != null)
                    answer.append($("<input>", { "type": "hidden", "value": data.Content }));

                if (data.Media != null) {
                    renderMediaContent(data, answer);
                }
                else
                    answer.append($("<label>", { "class": "answer-text", "html": breakLine(data.Content) }));
                container.append(answer);
                break;
            default:
                var form = $("<div>", { "class": "form-check" });
                answer.append(form);
                form.append($("<input>", { "type": "hidden" }));
                form.append($("<input>", { "id": data.ID, "type": "radio", "class": "input-checkbox answer-checkbox form-check-input", "name": "rd_" + data.ParentID }));
                if (data.Content != null)
                    form.append($("<label>", { "class": "answer-text form-check-label", "for": data.ID, "html": breakLine(data.Content) }));
                renderMediaContent(data, answer);
                container.append(answer);
                break;
        }

    }

    var renderMediaContent = function (data, wrapper, type = "") {
        //console.log('renderMedia');
        //console.log(data);
        if (data.Media != null) {
            var mediaHolder = $("<div>", { "class": "media-holder mt-2 mb-2 " + type });
            //var contentWrapper = $("<div>", { class: "m-content" });
            switch (type) {
                case "IMG":
                    mediaHolder.append(
                        $("<img>", { "class": "img-fluid lazy", "src": data.Media.Path.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn") }));
                    break;
                case "VIDEO":
                    mediaHolder.append("<video controls><source src='" + data.Media.Path.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn") + "' type='" + data.Media.Extension + "' />Your browser does not support the video tag</video>");
                    break;
                case "AUDIO":
                    mediaHolder.append("<audio id='audio' controls><source src='" + data.Media.Path.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn") + "' type='" + data.Media.Extension + "' />Your browser does not support the audio tag</audio>");
                    break;
                case "DOC":
                    if (data.Media.Path.endsWith("doc") || data.Media.Path.endsWith("docx") ||
                        data.Media.Path.endsWith("ppt") || data.Media.Path.endsWith("pptx") ||
                        data.Media.Path.endsWith("xls") || data.Media.Path.endsWith("xlsx")
                    ) {
                        mediaHolder.append($("<iframe>", { "src": "https://view.officeapps.live.com/op/embed.aspx?src=https://" + window.location.hostname + data.Media.Path + "", "class": "embed-frame", "frameborder": "0" }));
                    }
                    else {
                        if (data.Media != null)
                            mediaHolder.append($("<embed>", { "src": data.Media.Path.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn") + "#view=FitH", "class": "embed-frame" }));
                    }
                    break;
                default:
                    if (data.Media.Extension != null)
                        if (data.Media.Extension.indexOf("image") >= 0)
                            mediaHolder.append(
                                $("<img>", { "class": "img-fluid lazy", "src": data.Media.Path.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn") }));
                        else if (data.Media.Extension.indexOf("video") >= 0)
                            mediaHolder.append("<video controls><source src='" + data.Media.Path.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn") + "' type='" + data.Media.Extension + "' />Your browser does not support the video tag</video>");
                        else if (data.Media.Extension.indexOf("audio") >= 0)
                            mediaHolder.append("<audio id='audio' controls><source src='" + data.Media.Path.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn") + "' type='" + data.Media.Extension + "' />Your browser does not support the audio tag</audio>");
                        else {
                            if (data.Media.Path.endsWith("doc") || data.Media.Path.endsWith("docx") ||
                                data.Media.Path.endsWith("ppt") || data.Media.Path.endsWith("pptx") ||
                                data.Media.Path.endsWith("xls") || data.Media.Path.endsWith("xlsx")
                            ) {
                                mediaHolder.append($("<iframe>", { "src": "https://view.officeapps.live.com/op/embed.aspx?src=https://" + window.location.hostname + data.Media.Path + "", "class": "embed-frame", "frameborder": "0" }));
                            }
                            else
                                mediaHolder.append($("<embed>", { "src": data.Media.Path.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn") + "#view=FitH" }));
                        }

                    break;
            }
            wrapper.append(mediaHolder);
        }
    }

    var startDragDrop = function () {
        $(".answer-item").draggable({
            cursor: "move",
            helper: 'clone',
            revert: "true",
            scroll: true,
            start: function (event, ui) {
                var _parent = $(this).parent();
                if (_parent.hasClass('answer-wrapper')) {
                    _parent.addClass('dragging');
                    $('.quiz-wrapper').addClass('dragtarget');
                }
                else {
                    $('.answer-wrapper').addClass('droptarget');
                    $('.quiz-wrapper').addClass('dragging');
                }
            },
            stop: function (event, ui) {
                $('.quiz-wrapper').removeClass('dragging').removeClass('droptarget');
                $('.answer-wrapper').removeClass('dragging').removeClass('droptarget');
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
                console.log(part);
                console.log($("#" + part + " .answer-wrapper"));
                if ($(this).find(".answer-item").length > 0) {//remove all answer to box
                    $("#" + part + " .answer-wrapper").append($(this).find(".answer-item"));
                }

                $(this).append(item);

                if ($(prevHolder).find(".answer-item").length == 1) {

                    if ($(prevHolder).find(".placeholder").length > 0) {
                        $(prevHolder).find(".placeholder").show();
                    }
                    else {
                        $(prevHolder).append($("<div>", { "class": "pane-item placeholder", "text": "Drop your answer here" }));
                    }
                }

                //if (config.mod == mod.STUDENT_EXAM) {
                var quiz = prevHolder.data("questionId");
                if (quiz != null) {
                    delAnswerForStudentNoRender(quiz);
                }

                AnswerQuestion(this);
                //}

                $(this).find(".placeholder").hide();
            }
        });
        $(".answer-wrapper.no-child[data-part-id]").droppable({
            tolerance: "intersect",
            accept: ".answer-item",
            activeClass: "hasAnswer",
            hoverClass: "answerHover",
            drop: function (event, ui) {
                $(this).find(".placeholder").hide();
                var prevHolder = $(ui.helper).parent();

                if (config.mod == mod.STUDENT_EXAM) {
                    var quiz = prevHolder.data("questionId");
                    if (quiz != null) { delAnswerForStudent(quiz); }
                }
                prevHolder.remove($(ui.helper));

                $(this).append($(ui.draggable));

                if ($(prevHolder).find(".answer-item").length == 1) {
                    if ($(prevHolder).find(".placeholder").length > 0)
                        $(prevHolder).find(".placeholder").show();
                    else
                        prevHolder.append($("<div>", { "class": "pane-item placeholder", "text": "Drop your answer here" }));
                }

                //AnswerQuestion(this);
            }
        });
    }

    var breakLine = function (data) {
        if (data == null)
            return "";
        return data.replace(/\n/g, "<br/>");
    }

    var toggleExplanation = function (obj) {
        $(obj).toggleClass("btn-warning");
        if ($(obj).hasClass("btn-warning")) {
            $(".quiz-extend:not(.show)").addClass("show");
        } else {
            $(".quiz-extend").removeClass("show");
        }
    }

    //Edit

    function modalSelectTemplate(type) {
        var modal = $("#lessonModal");
        if (type != null && type != "")
            modal.find("[name=TemplateType]").val(type);
        var template = modal.find("[name=TemplateType]").val();
        switch (template) {
            case "1": //bài giảng
                modal.find("[for=examOnly]").hide();
                modal.find("[for=examOnly] input").prop("required", false);
                modal.find("[for=examOnly] select").prop("required", false);
                break;
            default:
                modal.find("[for=examOnly]").show();
                modal.find("[for=examOnly] input").prop("required", true);
                modal.find("[for=examOnly] select").prop("required", true);
                break;
        }
    }

    var modalEditLesson = function (ID) {
        var modal = $('#lessonModal');
        $.ajax({
            type: "POST",
            url: config.url.load,
            data: { ID: ID },
            dataType: "json",
            success: function (data) {
                if (data.Data != null) {
                    var item = data.Data;
                    SelectTemplate(item.TemplateType);
                    //template.lesson(data.Data.TemplateType, data.Data);
                    modal.find("[name=Title]").val(item.Title);
                    modal.find("[name=ChapterID]").val(item.ChapterID);
                    modal.find("[name=ID]").val(item.ID);
                    modal.find("[name=Point]").val(item.Point);
                    modal.find("[name=Timer]").val(item.Timer);
                    modal.find("[name=Limit]").val(item.Limit);
                    modal.find("[name=Multiple]").val(item.Multiple);
                    modal.find("[name=Etype]").val(item.Etype);
                }
                else {
                    alert("Error")
                }
            }
        });

        return false;
    }

    var modalEditPart = function (id) {
        stopAllMedia();
        var modalForm = window.partForm;
        $('#action').val(config.url.save_part);
        //showModal();
        $(modalForm).empty();
        $.ajax({
            type: "POST",
            url: config.url.load_part,
            data: { ID: id },
            dataType: "json",
            success: function (data) {
                renderEditPart(data.Data);
            }
        });
    }

    var modalAddPart = function (lessonID, type) {
        var modalForm = window.partForm;
        $(modalForm).empty();
        $('#partModal #modalTitle').html("Thêm nội dung");
        $(modalForm).append($("<input>", { "type": "hidden", "name": "ParentID", "value": lessonID }));
        $(modalForm).append($("<input>", { "type": "hidden", "name": "ClassID", "value": config.class_id }));
        $(modalForm).append($("<input>", { "type": "hidden", "name": "ClassSubjectID", "value": config.class_subject_id }));
        $('#action').val(config.url.save_part);
        var selectTemplate = $("<select>", { "class": "templatetype form-control", "name": "Type", "required": "required" }).bind("change", chooseTemplate);
        $(modalForm).append(selectTemplate);
        $(selectTemplate).append("<option value=''>--- Chọn kiểu nội dung ---</option>");
        //if (type == TEMPLATE_TYPE.LESSON) {
        $(selectTemplate).append("<option value='TEXT'>Văn bản</option>")
            .append("<option value='VIDEO'>Video</option>")
            .append("<option value='AUDIO'>Audio</option>")
            .append("<option value='IMG'>Hình ảnh</option>")
            .append("<option value='DOC'>File văn bản (PDF, DOC, PPT, XLS)</option>")
        //.append("<option value='VOCAB'>Vocabulary</option>")
        //}
        //else {
        $(selectTemplate).append("<option value='QUIZ1'>QUIZ: Chọn đáp án đúng</option>")
            .append("<option value='QUIZ2'>QUIZ: Điền từ</option>")
            .append("<option value='QUIZ3'>QUIZ: Nối đáp án</option>")
        //.append("<option value='ESSAY'>QUIZ: Essay</option>");
        //}
        $(modalForm).append($("<div>", { "class": "lesson_parts" }));
        $(modalForm).append($("<div>", { "class": "question_template hide" }));
        $(modalForm).append($("<div>", { "class": "answer_template hide" }));
    }

    var renderEditPart = function (data) {
        var modalForm = window.partForm;
        $('#partModal #modalTitle').html("Update");
        $(modalForm).append($("<input>", { "type": "hidden", "name": "ParentID", "value": data.ParentID }));
        $(modalForm).append($("<input>", { "type": "hidden", "name": "ClassID", "value": config.class_id }));
        $(modalForm).append($("<input>", { "type": "hidden", "name": "ClassSubjectID", "value": config.class_subject_id }));
        $(modalForm).append($("<input>", { "type": "hidden", "name": "ID", "value": data.ID }));
        $(modalForm).append($("<input>", { "type": "hidden", "name": "Type", "value": data.Type }));
        $('#action').val(config.url.save_part);
        switch (data.Type) {
            case "QUIZ2":
                $("#partModal").find('.btnSaveForm').attr("onclick", "submitQuizFill(event)");
                break;
            default:
                $("#partModal").find('.btnSaveForm').attr("onclick", "submitForm(event)");
                //$(modalForm).find('.btnSaveForm').unbind().click(function () { return submitForm(event) });
                break;
        }

        $(modalForm).append($("<div>", { "class": "lesson_parts" }));
        $(modalForm).append($("<div>", { "class": "question_template hide" }));
        $(modalForm).append($("<div>", { "class": "answer_template hide" }));
        renderPartTemplate(data.Type, data);
    }

    var removePart = function (id) {

        Swal.fire({
            title: 'Xóa nội dung này?',
            text: "Bạn sẽ không thể khôi phục lại sau khi xóa!",
            icon: 'warning',
            showCancelButton: true,
            focusConfirm: false,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#CCC',
            cancelButtonText: 'Hủy',
            confirmButtonText: 'Đồng ý xóa!'
        }).then((result) => {
            if (result.value) {
                //ExcuteOnlyItem(ID, '@Url.Action("RemoveChapter", "Curriculum")', DeleteChapterCallback);
                $.ajax({
                    type: "POST",
                    url: config.url.del_part,
                    data: {
                        "ID": id
                    },
                    success: function (data) {
                        if (data.Error == null) {
                            document.location = document.location;
                        }
                        else {
                            alert(data.Error);
                        }
                    }
                });
            }
        })
    }

    var changePartPos = function (id, pos) {
        $.ajax({
            type: "POST",
            url: config.url.move_part,
            data: {
                "ID": id,
                "pos": pos,
                "ClassID": config.class_id
            },
            success: function (data) {
                console.log(data.message);
            }
        });
    }

    var sortPart = function () {
        $("#pills-tab").toggleClass("sorting");
        if ($("#pills-tab").hasClass("sorting")) {
            $("#part-menu").show();
            $(".card-header .btn-sort").addClass("btn-warning");
            $("#pills-tab").sortable({
                revert: "invalid",
                update: function (event, ui) {
                    var id = $(ui.item).find('a').attr("id").replace("pills-", "");
                    var ar = $(this).find("li");
                    var index = $(ar).index(ui.item);
                    changePartPos(id, index);
                }
            });
            $("#pills-tab").sortable("enable");
            $("#pills-tab").disableSelection();
        }
        else {
            $("#part-menu").hide();
            $(".card-header .btn-sort").removeClass("btn-warning");
            $("#pills-tab").sortable("disable");
            window.ReloadData();
        }
    }

    var chooseTemplate = function () {
        var type = $('.templatetype').val();
        renderPartTemplate(type);
    }

    var renderPartTemplate = function (type, data = null) {
        var contentholder = $('.lesson_parts');
        contentholder.empty();
        var question_template_holder = $('.question_template');
        question_template_holder.empty();
        var answer_template_holder = $('.answer_template');
        answer_template_holder.empty();

        contentholder.append($("<label>", { "class": "title", "text": "Tiêu đề:" }));
        contentholder.append($("<input>", { "type": "text", "name": "Title", "class": "input-text form-control", "placeholder": "Tiêu đề" }));
        if (data != null && data.Title != null)
            contentholder.find("[name=Title]").val(data.Title);

        var desc = $("<textarea>", { "id": "editor", "rows": "5", "name": "Description", "class": "input-text form-control", "placeholder": "Nội dung" });
        contentholder.append($("<div>", { class: "mb-2" }).append(desc));

        var description = "";

        if (data != null && data.Description != null)
            description = data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn");

        desc.val(description);
        switch (type) {
            case "TEXT"://Text
                contentholder.append($("<label>", { "class": "title", "text": "Nhập nội dung" }));
                contentholder.append($('#editor'));
                break;
            case "VIDEO":
                contentholder.append($("<label>", { "class": "title", "text": "Chọn file video" }));
                contentholder.append($("<div>", { "class": "media_holder" }));
                renderAddMedia(contentholder.find(".media_holder"), "", "VIDEO", data != null ? data.Media : null);
                contentholder.append($("<div>", { "class": "media_preview" }));
                break;
            case "AUDIO":
                contentholder.append($("<label>", { "class": "title", "text": "Chọn file audio" }));
                contentholder.append($("<div>", { "class": "media_holder" }));
                renderAddMedia(contentholder.find(".media_holder"), "", "AUDIO", data != null ? data.Media : null);
                contentholder.append($("<div>", { "class": "media_preview" }));
                break;
            case "IMG":
                contentholder.append($("<label>", { "class": "title", "text": "Chọn hình ảnh" }));
                contentholder.append($("<div>", { "class": "media_holder" }));
                renderAddMedia(contentholder.find(".media_holder"), "", "IMG", data != null ? data.Media : null);
                contentholder.append($("<div>", { "class": "media_preview" }));
                break;
            case "DOC":
                contentholder.append($("<label>", { "class": "title", "text": "Chọn văn bản (pdf, doc, ppt, xls)" }));
                contentholder.append($("<div>", { "class": "media_holder" }));
                renderAddMedia(contentholder.find(".media_holder"), "", "DOC", data != null ? data.Media : null);
                contentholder.append($("<div>", { "class": "media_preview" }));
                break;
            case "QUIZ1"://Trắc nghiệm chuẩn
                var questionTemplate = $("<fieldset>", { "class": "fieldQuestion", "Order": 0 });
                questionTemplate.append($("<input>", { "type": "hidden", "name": "Questions.ID" }));
                questionTemplate.append($("<input>", { "type": "hidden", "name": "Questions.Order", "value": 0 }));
                questionTemplate.append($("<label>", { "class": "fieldset_title", "text": "" }));
                questionTemplate.append($("<input>", { "type": "button", "class": "quiz-remove", "value": "X", "tabindex": -1, "onclick": "RemoveQuestion(this)" }));
                questionTemplate.append($("<textarea>", { "rows": "3", "name": "Questions.Content", "class": "input-text quiz-text form-control", "placeholder": "Question" }));

                questionTemplate.append($("<div>", { "class": "media_holder" }));
                renderAddMedia(questionTemplate.find(".media_holder"), "Questions.");
                questionTemplate.append($("<div>", { "class": "media_preview" }));
                questionTemplate.append(
                    $("<div>", { class: "mt-1 d-none" }).append($("<label>", { "class": "input_label mr-1", "text": "Điểm" }))
                        .append($("<input>", { "type": "text", "name": "Questions.Point", "class": "input-text part_point form-control", "placeholder": "Point", "value": "1" })));
                questionTemplate.append($("<div>", { class: "mt-1" }).append($("<label>", { "class": "part_label", "text": "Answer (tick if correct)" })));

                var answer_wrapper = $("<div>", { "class": "answer-wrapper" });
                answer_wrapper.append($("<input>", { "type": "button", "class": "btn btn-primary btnAddAnswer ml-3", "value": "+", "onclick": "AddNewAnswer(this)" }));
                questionTemplate.append(answer_wrapper);
                questionTemplate.append($("<textarea>", { "rows": "2", "name": "Questions.Description", "class": "input-text part_description form-control", "placeholder": "Explanation" }));
                question_template_holder.append(questionTemplate);

                var answerTemplate = $("<fieldset>", { "class": "answer-box m-1" });
                answerTemplate.append($("<input>", { "type": "button", "class": "answer-remove", "value": "X", "tabindex": -1, "onclick": "RemoveAnswer(this)" }));
                answerTemplate.append($("<input>", { "type": "hidden", "name": "Questions.Answers.ID" }));
                answerTemplate.append($("<input>", { "type": "hidden", "name": "Questions.Answers.ParentID", "value": 0 }));
                answerTemplate.append($("<input>", { "type": "hidden", "name": "Questions.Answers.IsCorrect" }));
                answerTemplate.append($("<input>", { "type": "checkbox", "class": "input-checkbox answer-checkbox", "onclick": "ToggleCorrectAnswer(this)" }));
                answerTemplate.append($("<input>", { "type": "text", "name": "Questions.Answers.Content", "class": "input-text answer-text form-control", "placeholder": "Answer" }));
                answerTemplate.append($("<div>", { "class": "media_holder mt-1" }));
                renderAddMedia(answerTemplate.find(".media_holder"), "Questions.Answers.");
                answerTemplate.append($("<div>", { "class": "media_preview" }));
                answer_template_holder.append(answerTemplate);

                contentholder.append($("<div>", { "class": "media_holder" }));
                contentholder.append($("<div>", { "class": "media_preview" }));
                renderAddMedia(contentholder.find(".media_holder"), "", "", data != null ? data.Media : null);
                contentholder.append($("<div>", { "class": "part_content " + type }));
                contentholder.append($("<button>", { "type": "button", "class": "btn btnAddQuestion btn-primary", "onclick": "AddNewQuestion(this)" }).append('<i class="fas fa-plus"></i>').append(' Thêm câu hỏi'));
                contentholder.append($("<button>", { "type": "button", "class": "btn btnCloneQuestion btn-primary ml-2", "onclick": "ShowCloneQuestion(this)" }).append('<i class="fas fa-plus"></i>').append(' Thêm từ file'));

                //Add First Question
                if (data != null && data.Questions != null) {
                    for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
                        var quiz = data.Questions[i];
                        addNewQuestion(quiz);
                    }
                }
                //else
                //    addNewQuestion();
                break;
            case "QUIZ2"://Trắc nghiệm dạng điền từ

                contentholder.append($("<div>", { "class": "media_holder" }));
                renderAddMedia(contentholder.find(".media_holder"), "", "", data != null ? data.Media : null);
                contentholder.append($("<div>", { "class": "media_preview" }));
                contentholder.append($("<div>", { "class": "part_content " + type }));

                //Add First Question

                var quizContent = $.parseHTML("<div>" + description + "</div>");
                var fillquizs = $(quizContent).find("fillquiz");
                //console.log(data);
                if (data != null && data.Questions != null) {
                    for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {

                        var quiz = data.Questions[i];
                        //console.log(quiz);
                        var input = $(fillquizs[i]).find("input");
                        var answers = [];
                        if (quiz.Answers.length > 0) {
                            for (var j = 0; j < quiz.Answers.length; j++)
                                answers.push(quiz.Answers[j].Content);
                            var content = answers.join(" | ");
                            var size = 5;

                            if (content.length > size) size = content.length;
                            $(input).attr("ans", content);
                            $(input).attr("size", size);
                            $(input).attr("dsp", quiz.Content);
                            $(input).attr("placeholder", content);
                            $(input).attr("value", "");
                        }
                        $(fillquizs[i]).attr("title", quiz.Description == null ? "" : quiz.Description);
                    }
                    //console.log($(quizContent).prop("innerHTML"));
                    desc.val($(quizContent).prop("innerHTML"));
                }

                //else
                //    addNewQuestion();
                break;
            case "QUIZ3"://Trắc nghiệm match
                var questionTemplate = $("<fieldset>", { "class": "fieldQuestion", "Order": 0 });
                questionTemplate.append($("<input>", { "type": "hidden", "name": "Questions.ID" }));
                questionTemplate.append($("<input>", { "type": "hidden", "name": "Questions.Order", "value": 0 }));
                questionTemplate.append($("<label>", { "class": "fieldset_title", "text": "" }));
                questionTemplate.append($("<input>", { "type": "button", "class": "quiz-remove", "value": "X", "tabindex": -1, "onclick": "RemoveQuestion(this)" }));

                var quizWrapper = $("<div>", { "class": "quiz-wrapper" });
                quizWrapper.append($("<input>", { "type": "text", "name": "Questions.Content", "class": "input-text quiz-text form-control input-sm", "placeholder": "Question", "tabindex": 0 }));
                quizWrapper.append($("<div>", { "class": "media_holder" }));
                renderAddMedia(quizWrapper.find(".media_holder"), "Questions.");
                quizWrapper.append($("<div>", { "class": "media_preview" }));
                quizWrapper.append($("<label>", { "class": "input_label d-none", "text": "Điểm " }));
                quizWrapper.append($("<input>", { "type": "text", "name": "Questions.Point", "class": "d-none input-text part_point form-control", "placeholder": "Point", "value": "1", "tabindex": 0 }));
                questionTemplate.append(quizWrapper);

                var answer_wrapper = $("<div>", { "class": "answer-wrapper" });
                answer_wrapper.append($("<input>", { "type": "button", "class": "btn btn-primary btnAddAnswer ml-3", "value": "+", "tabindex": -1, "onclick": "AddNewAnswer(this)" }));
                questionTemplate.append(answer_wrapper);
                question_template_holder.append(questionTemplate);

                var answerTemplate = $("<fieldset>", { "class": "answer-box m-1 selected" });
                answerTemplate.append($("<input>", { "type": "button", "class": "answer-remove", "value": "X", "tabindex": -1, "onclick": "RemoveAnswer(this)" }));
                answerTemplate.append($("<input>", { "type": "hidden", "name": "Questions.Answers.ID" }));
                answerTemplate.append($("<input>", { "type": "hidden", "name": "Questions.Answers.ParentID", "value": 0 }));
                answerTemplate.append($("<input>", { "type": "hidden", "name": "Questions.Answers.IsCorrect", "value": true }));
                answerTemplate.append($("<input>", { "type": "checkbox", "class": "input-checkbox answer-checkbox", "checked": "checked", "tabindex": 0, "onclick": "ToggleCorrectAnswer(this)" }));
                answerTemplate.append($("<input>", { "type": "text", "name": "Questions.Answers.Content", "class": "input-text answer-text form-control", "placeholder": "Answer (tick if correct)", "tabindex": 0 }));
                answerTemplate.append($("<div>", { "class": "media_holder" }));
                renderAddMedia(answerTemplate.find(".media_holder"), "Questions.Answers.");
                answerTemplate.append($("<div>", { "class": "media_preview" }));
                var x = Math.floor((Math.random() * 2));

                if (x > 0)
                    answer_template_holder.append(answerTemplate);
                else
                    answer_template_holder.prepend(answerTemplate);

                contentholder.append($("<div>", { "class": "media_holder" }));
                renderAddMedia(contentholder.find(".media_holder"), "", "", data != null ? data.Media : null);
                contentholder.append($("<div>", { "class": "media_preview" }));
                contentholder.append($("<div>", { "class": "part_content " + type }));
                contentholder.append($("<input>", { "type": "button", "class": "btn btnAddQuestion btn-primary", "value": "Add question", "onclick": "AddNewQuestion(this)", "tabindex": -1 }));
                contentholder.append($("<button>", { "type": "button", "class": "btn btnCloneQuestion btn-primary ml-2", "onclick": "ShowCloneQuestion(this)" }).append('<i class="fas fa-plus"></i>').append(' Thêm từ file'));

                //Add question
                if (data != null && data.Questions != null) {
                    for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
                        var quiz = data.Questions[i];
                        addNewQuestion(quiz);
                    }
                }
                else
                    addNewQuestion();
                break;
            case "ESSAY"://Tự luận
                contentholder.append($("<label>", { "class": "title", "text": "Điểm" }));
                contentholder.append($("<input>", { "type": "text", "name": "Point", "class": "input-text form-control", "placeholder": "Điểm", "required": "required", value: data.Point }));
                ////contentholder.append($("<textarea>", { "id": "editor", "rows": "15", "name": "Description", "class": "input-text form-control", "placeholder": "Description" }));
                //if (data != null && data.description != null)
                //    contentholder.find("[name=Description]").val(data.description);
                //console.log(data);
                //CKEDITOR.replace("editor");

                //ClassicEditor
                //    .create(document.querySelector('#editor'))
                //    .then(newEditor => {
                //        myEditor = newEditor;
                //    })
                //    .catch(error => {
                //        console.error(error);
                //    });

                //var questionTemplate = $("<fieldset>", { "class": "fieldQuestion", "Order": 0 });
                //questionTemplate.append($("<input>", { "type": "hidden", "name": "Questions.ID" }));
                //questionTemplate.append($("<input>", { "type": "hidden", "name": "Questions.Order", "value": 0 }));
                //questionTemplate.append($("<label>", { "class": "fieldset_title", "text": "" }));

                //var quizWrapper = $("<div>", { "class": "quiz-wrapper" });
                //quizWrapper.append($("<input>", { "type": "text", "name": "Questions.Content", "class": "input-text quiz-text form-control", "placeholder": "Question", "tabindex": 0 }));
                //quizWrapper.append($("<div>", { "class": "media_holder" }));
                //renderAddMedia(quizWrapper.find(".media_holder"), "Questions.");
                //quizWrapper.append($("<div>", { "class": "media_preview" }));
                //quizWrapper.append($("<label>", { "class": "input_label", "text": "Point" }));
                //quizWrapper.append($("<input>", { "type": "text", "name": "Questions.Point", "class": "input-text part_point form-control", "placeholder": "Point", "value": "1", "tabindex": 0 }));
                //questionTemplate.append(quizWrapper);


                contentholder.append($("<div>", { "class": "media_holder" }));
                renderAddMedia(contentholder.find(".media_holder"), "", "", data != null ? data.Media : null);
                contentholder.append($("<div>", { "class": "media_preview" }));
                contentholder.append($("<div>", { "class": "part_content " + type }));

                if (data != null && data.Questions != null) {
                    for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
                        var quiz = data.Questions[i];
                        addNewQuestion(quiz);
                    }
                }
                //else
                //    addNewQuestion();
                break;
            default:
                alert("Not implement");
                break;
        }
        //CKEDITOR.replace("editor");        
        CKEDITOR.plugins.addExternal('ckeditor_wiris', 'https://www.wiris.net/demo/plugins/ckeditor/', 'plugin.js');

        switch (type) {
            case "QUIZ2":
                CKEDITOR.replace('editor', {
                    allowedContent: true,
                    extraPlugins: 'uploadimage,youtube,ckeditor_wiris,fillquiz',
                    removeDialogTabs: 'textfield',
                    removePlugins: 'forms'
                });
                CKEDITOR.on('dialogDefinition', function (ev) {
                    var dialogName = ev.data.name,
                        dialogDefinition = ev.data.definition;
                    console.log(ev.data);
                    if (dialogName === 'textfield') {
                        console.log(ev.data);
                        dialogDefinition.removeContents('info');
                    }
                });

                break;
            default:
                CKEDITOR.replace('editor', {
                    allowedContent: true,
                    extraPlugins: 'uploadimage,youtube,ckeditor_wiris'
                });
                break;
        }

        if (data != null && data.Media != null)
            renderMediaContent(data, contentholder.find(".media_preview:first"), type);
    }

    var addNewQuestion = function (data = null) {
        var container = $('.lesson_parts > .part_content');
        var template = $('.question_template > fieldset');
        var currentpos = $(container).find(".fieldQuestion").length;
        var clone = template.clone();
        $(clone).find('.fieldset_title').text("Quiz " + (currentpos + 1));
        $(clone).attr("Order", currentpos);
        $(clone).find("[name='Questions.Order']").val(currentpos);
        if (data != null) {
            if (data.ID != null)
                $(clone).find("[name='Questions.ID']").val(data.ID);
            if (data.Point != null)
                $(clone).find("[name='Questions.Point']").val(data.Point);
            if (data.Content != null)
                $(clone).find("[name='Questions.Content']").val(data.Content);
            if (data.Description != null)
                $(clone).find("[name='Questions.Description']").val(data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn"));
            if (data.Media != null) {
                renderAddMedia(clone.find(".media_holder"), "Questions.", "", data.Media);
                renderMediaContent(data, clone.find(".media_preview:first"), "");
            }
        }

        $(clone).find("[name^='Questions.']").each(function () {
            $(this).attr("name", $(this).attr("name").replace("Questions.", "Questions[" + (currentpos) + "]."));
        });
        $(container).append(clone);
        if (data != null && data.Answers != null) {
            for (var i = 0; data.Answers != null && i < data.Answers.length; i++) {
                var answer = data.Answers[i];
                addNewAnswer(null, clone, answer);
            }
        }
        else
            clone.find(".btnAddAnswer").click();//add new answer
    }

    var addNewAnswer = function (obj, wrapper = null, data = null) {
        var question;
        if (obj != null) {
            question = $(obj).parent().parent();
        } else
            question = wrapper;
        var question_pos = $(question).attr("Order");
        var container = $(question).find('.answer-wrapper');
        var template = $('.answer_template > .answer-box');
        var clone = template.clone();
        var currentpos = $(container).find(".answer-box").length;

        if (data != null) {
            if (data.ID != null)
                $(clone).find("[name='Questions.Answers.ID']").val(data.ID);
            if (data.ParentID != null)
                $(clone).find("[name='Questions.Answers.ParentID']").val(data.ParentID);
            if (data.Content != null)
                $(clone).find("[name='Questions.Answers.Content']").val(data.Content);
            if (data.IsCorrect != null) {
                $(clone).find("[name='Questions.Answers.IsCorrect']").val(data.IsCorrect);
                $(clone).find(".answer-checkbox").prop("checked", data.IsCorrect);
                if (data.IsCorrect)
                    $(clone).find(".answer-checkbox").parent().addClass("selected");
            }
            if (data.Media != null) {
                renderAddMedia(clone.find(".media_holder"), "Questions.Answers.", "", data.Media);
                renderMediaContent(data, clone.find(".media_preview:first"), "");
            }
        }

        $(clone).find("[name^='Questions.Answers.']").each(function () {
            $(this).attr("name", $(this).attr("name").replace("Questions.Answers.", "Questions[" + (question_pos) + "].Answers[" + currentpos + "]."));
        });
        if (obj != null)
            $(obj).before(clone);
        else
            $(wrapper).find(".btnAddAnswer").before(clone);
    }

    var showCloneQuestion = function () {
        Swal.fire({
            title: '<strong>Chọn thao tác</strong>',
            icon: 'question',
            html:
                '<button type="button" class="btn btn-primary mr-2" onclick="ChooseQuestionFile(this)"><i class="fas fa-upload"></i> Chọn file câu hỏi </button>' +
                '<input type="file" class="d-none" accept=".xlsx, .xls"/>' +
                '<button type="button" class="btn btn-secondary mr-2" onclick="DownloadQuestionTemplate(this)"><i class="fas fa-file-download"></i> Tải file mẫu </button>',
            //'<button type="button" class="btn btn-info" onclick="ExportQuestion(this)"><i class="fas fa-download"></i> Xuất câu hỏi</button>',
            confirmButtonText: 'Đóng',
        })
    }

    var downloadQuestionTemplate = function () {
        window.open(config.url.export_quiztemp);
    }

    var chooseQuestionFile = function (obj) {
        $(obj).siblings('input').unbind().change(function (e) {
            uploadQuestionFile(e);
        });
        $(obj).siblings('input').focus().click();
    }

    var uploadQuestionFile = function (e) {
        Swal.showLoading();
        var xhr = new XMLHttpRequest();
        var formData = new FormData();
        formData.append('file', e.target.files[0]);
        xhr.open('POST', config.url.import_quiz);
        xhr.send(formData);
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var data = JSON.parse(xhr.responseText);
                if (data.Error != null && data.Error != "") {
                    Swal.hideLoading();
                    Swal.clickConfirm();
                    Swal.fire({
                        title: 'Thông báo',
                        icon: 'error',
                        text: data.Error
                    }).then(() => {
                        showCloneQuestion();
                    })
                }
                else {
                    var dt = data.Data;
                    if (dt != null && dt.Questions != null) {
                        for (var i = 0; dt.Questions != null && i < dt.Questions.length; i++) {
                            var quiz = dt.Questions[i];
                            addNewQuestion(quiz);
                        }
                    }
                    Swal.hideLoading();
                    Swal.clickConfirm()
                }
            }
        }
    }

    var removeQuestion = function (obj) {

        Swal.fire({
            title: 'Xác nhận xóa câu hỏi?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Xóa',
            cancelButtonText: 'Bỏ qua'
        }).then((result) => {
            if (result.value) {
                var id = $(obj).siblings("[name$='.ID']").val();
                var quizHolder = $(obj).parent();
                if (!$(obj).parent().hasClass("fieldQuestion"))
                    quizHolder = $(obj).parent().parent();
                quizHolder.parent().append($("<input>", { "type": "hidden", "name": "RemovedQuestions", "value": id }));
                quizHolder.remove();
            }
        })
    }

    var removeAnswer = function (obj) {
        if (confirm("Remove answer?")) {
            var id = $(obj).siblings("[name$='.ID']").val();
            if (id !== "")
                $(obj).parent().parent().append($("<input>", { "type": "hidden", "name": "RemovedAnswers", "value": id }));
            $(obj).parent().remove();
        }
    }

    var renderAddMedia = function (wrapper, prefix, type = "", data = null) {
        console.log(data);
        wrapper.empty();
        wrapper.append($("<input>", { "type": "hidden", "name": prefix + "Media.Name", "for": "medianame" }));
        wrapper.append($("<input>", { "type": "hidden", "name": prefix + "Media.OriginalName", "for": "mediaorgname" }));
        wrapper.append($("<input>", { "type": "hidden", "name": prefix + "Media.Extension", "for": "mediaext" }));
        wrapper.append($("<input>", { "type": "hidden", "name": prefix + "Media.Path", "for": "mediapath" }));
        switch (type) {
            case "IMG":
                wrapper.append($("<input>", { "type": "file", "name": "file", "onchange": "changeMedia(this)", "class": "hide", "accept": "image/*" }));
                break;
            case "VIDEO":
                wrapper.append($("<input>", { "type": "file", "name": "file", "onchange": "changeMedia(this)", "class": "hide", "accept": "video/*" }));
                break;
            case "AUDIO":
                wrapper.append($("<input>", { "type": "file", "name": "file", "onchange": "changeMedia(this)", "class": "hide", "accept": "audio/*" }));
                break;
            case "DOC":
                wrapper.append($("<input>", { "type": "file", "name": "file", "onchange": "changeMedia(this)", "class": "hide", "accept": ".pdf, .doc, .docx, .ppt, .pptx, .xls, .xlsx" }));
                break;
            default:
                wrapper.append($("<input>", { "type": "file", "name": "file", "onchange": "changeMedia(this)", "class": "hide" }));
                break;
        }
        wrapper.append($("<input>", { "type": "button", "class": "btn btn-primary btnAddFile " + type + " m-1 btn-sm", "style": "max-width:80%", "onclick": "chooseFile(this)", "value": "Chọn file", "tabindex": -1 }));
        wrapper.append($("<input>", { "type": "button", "class": "btn btn-danger btnResetFile btn-sm hide m-1", "onclick": "resetMedia(this)", "value": "x", "tabindex": -1 }));
        if (data != null) {
            if (data.Name != null) $(wrapper).find("[name='" + prefix + "Media.Name']").val(data.Name);
            if (data.OriginalName != null) {
                $(wrapper).find("[name='" + prefix + "Media.OriginalName']").val(data.OriginalName);
                $(wrapper).find(".btnAddFile").val(data.OriginalName);
                $(wrapper).find(".btnResetFile").removeClass("hide");
            }
            if (data.Extension != null) $(wrapper).find("[name='" + prefix + "Media.Extension']").val(data.Extension);
            if (data.Path != null) {
                $(wrapper).find("[name='" + prefix + "Media.Path']").val(data.Path);
            }
        }
    }

    var toggleCorrectAnswer = function (obj) {
        var isChecked = $(obj).prop('checked');
        $(obj).prev().val(isChecked);
        if ($(obj).parent().hasClass("selected") != isChecked)
            $(obj).parent().toggleClass("selected");
    }

    //Navigation
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
        console.log(quizid);
        var _partid = _quiz.attr('data-part-id');
        var _part = $('.tab-pane#pills-part-' + _partid);
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
        var quizpanes = $('.tab-pane-quiz');
        $('.tab-pane.show.active').removeClass('show active');
        $('.tab-pane-quiz.show.active').removeClass('show active');
        $(panes[idx]).addClass('show active');
        $(quizpanes[idx]).addClass('show active');
        $('.prevtab').prop('disabled', idx == 0);
        $('.nexttab').prop('disabled', idx == _totalPart - 1);
    }

    var toggleNav = function (obj) {
        $(obj).toggleClass('btn-warning');
        if ($('#QuizNav').hasClass("show")) {
            $('#QuizNav').removeClass("show");
            $('.right-content').removeClass("showQuiz");
        }
        else {
            $('#QuizNav').addClass("show");
            $('.right-content').addClass("showQuiz");
        }


        //$('#quizIdx_holder').toggle();
        //if ($('#quizIdx_holder:visible').length > 0)
        //    $('.card-footer').addClass("expand");
        //else
        //    $('.card-footer').removeClass("expand");
        //$(obj).toggleClass('btn-warning');
    }

    //Exam
    var renderExam = function () {
        //load lastest exam state from server
        var dataform = new FormData();
        dataform.append("ClassID", config.class_id);
        dataform.append("ClassSubjectID", config.class_subject_id);
        dataform.append("LessonID", config.lesson_id);
        if ($('#' + config.container).find("#ExamID").length == 0)
            $('#' + config.container).prepend($("<input>", { type: "hidden", name: "ExamID", id: "ExamID" }));
        if (!checkExam()) {
            localStorage.clear();
        }
        //get lastest exam data from server
        Ajax(config.url.current, dataform, "POST", true).then(function (res) {

            var exam, schedule, limit = 0;
            try {
                var data = JSON.parse(res);
                exam = data.exam
                schedule = data.schedule
                limit = data.limit
            } catch (e) {
                console.log(e)
            }
            //console.log(exam.ID);
            //console.log(getLocalData("CurrentExam"));
            //if no data is found => new attempt => clear all local storage
            if (exam == null) {
                localStorage.clear();
                console.log("New Fresh Exam");
                renderNewExam(null, schedule);
            }
            else {
                //
                if (isNull(getLocalData("CurrentExam"))) //display last result & render new exam
                {
                    //console.log(data);
                    localStorage.clear();
                    console.log("New Exam");
                    //console.log(getLocalData("CurrentExam"))
                    renderNewExam(exam, schedule, limit);
                }
                else {
                    if (getLocalData("CurrentExam") != exam.ID) //unmatched id => complete exam
                    {
                        console.log(getLocalData("CurrentExam"))
                        console.log(exam.ID)
                        console.log("Unmatched Exam");
                        localStorage.clear();
                        setLocalData("Timer", "00:00");
                        $('#ExamID').val(exam.ID);
                        CompleteExam(true);
                        //console.log("Complete Exam")
                    }
                    else {
                        if (exam.Status) {
                            localStorage.clear();
                            console.log("New Exam");
                            //console.log(getLocalData("CurrentExam"))
                            renderNewExam(exam, schedule, limit);
                        }
                        else {
                            $('#ExamID').val(exam.ID);
                            var current = exam.CurrentDoTime;
                            var start = exam.Created;
                            var timer = moment(current) - moment(start);
                            if (moment(timer).minutes() >= exam.Timer)//Timeout
                            {
                                console.log("Exam Timeout")
                                setLocalData("Timer", "00:00");
                                CompleteExam(true);
                            }
                            else {
                                console.log("Exam Continue")
                                setLocalData("CurrentExam", exam.ID);
                                $('#ExamID').val(exam.ID);
                                //render Exam
                                var _sec = 59 - moment(timer).second();
                                var _minutes = exam.Timer - moment(timer).minutes() - (_sec > 0 ? 1 : 0);
                                var timer = (_minutes >= 10 ? _minutes : "0" + _minutes) + ":" + (_sec >= 10 ? _sec : "0" + _sec)
                                setLocalData("Timer", timer);
                                renderExamDetail();
                                countdown();
                            }
                        }
                    }
                }
            }
        });
    }

    var renderNewExam = function (data, schedule, limit) {
        var lesson_action_holder = $('.top-menu[for=lesson-info]');
        var wrapper = $("<div>", { "class": "p-3 col-md-12 text-center" });
        if (lesson_action_holder.length == 0)
            lesson_action_holder = wrapper;
        $('#' + config.container).append($(wrapper));

        var doButton;
        var endDate = moment(schedule.EndDate);
        var startDate = moment(schedule.StartDate);
        var now = moment();
        var isOverdue = false;
        if (endDate > moment(new Date(1900, 1, 1)) && endDate <= now) {
            console.log("Over due")
            doButton = $('<div>', {
                "class": "btn btn-danger m-2",
                "style": "cursor: pointer",
                "disabled": "disabled"
            }).append('<i class="fas fa-ban mr-2"></i>').append("Quá hạn (" + endDate.format("DD/MM/YYYY hh:mm A") + ")");
            isOverdue = true;
        }
        else {
            if (startDate >= now) {
                console.log("Early")
                doButton = $('<div>', {
                    "class": "btn btn-danger m-2 lesson-action",
                    "style": "cursor: pointer",
                    "disabled": "disabled"
                }).append('<i class="fas fa-ban mr-2"></i>').append("Bài chưa được mở (" + startDate.format("DD/MM/YYYY hh:mm A") + ")");
                isOverdue = true
            }
            else {
                console.log("In due")
                doButton = $('<div>', {
                    "class": "btn btn-primary m-2 lesson-action",
                    "onclick": "BeginExam(this)",
                    "style": "cursor: pointer",
                }).append('<i class="fas fa-play mr-2"></i>').append('Làm bài');
            }
        }

        if (isNull(data)) {
            //var backButton = $('<div>', {
            //    "class": "btn btn-primary m-3",
            //    "onclick": "GoBack()",
            //    "style": "cursor: pointer",
            //    "text": "Về danh sách"
            //});
            lesson_action_holder.append(doButton);
            //wrapper.append(doButton);
            //wrapper.append(backButton);
            lastExamResult =
                $("<div>", { id: "last-result", class: "text-center" })
                    //.append($('<div>', { class: "col-md-12 text-center p-3 h5 text-info", text: "Lượt làm cuối (lần 0) chưa bắt đầu" }))
                    .append($('<div>', { class: "text-center h4 btn-primary mt-5 btn", text: "Làm bài ngay", style: "cursor:pointer" }).click(function () {
                        BeginExam(this)
                    }).prepend($('<i>', { class: "fas fa-play mr-2" })));
            wrapper.append(lastExamResult);
        }
        else {
            var lastExam = data;
            this.exam_id = lastExam.ID;
            var tried = lastExam.Number;
            var doable = true;
            var lastpoint = (lastExam.MaxPoint > 0 ? (lastExam.Point * 100 / lastExam.MaxPoint) : 0);

            var lastdate = moment(lastExam.Updated).format("DD/MM/YYYY hh:mm A");
            lastExamResult =
                $("<div>", { id: "last-result", class: "text-center" })
                    .append($('<div>', { class: "col-md-12 text-center p-3 h5 text-info", text: "Lượt làm cuối (lần " + tried + ") đã kết thúc lúc " + lastdate }))
                    .append($('<div>', { class: "col-md-12 text-center h4 text-success", text: "Kết quả : " + (lastExam.QuestionsPass == null ? 0 : lastExam.QuestionsPass) + "/" + lastExam.QuestionsTotal })).html();
            wrapper.append(lastExamResult);

            tryleft = limit - tried;
            //var doButton = null;
            //var backButton = $('<div>', {
            //    "class": "btn btn-primary m-3",
            //    "onclick": "GoBack()",
            //    "style": "cursor: pointer",
            //    "text": "Về danh sách"
            //});
            var reviewButton = $('<div>', {
                "class": "btn btn-primary m-2 lesson-action",
                "onclick": 'Review(\'' + lastExam.ID + '\')',
                "style": "cursor: pointer"
            }).append('<i class="fas fa-poll mr-2"></i>').append("Xem đáp án");
            if (!isOverdue)
                if (limit > 0) {
                    tryleft = limit - tried;
                    if (tryleft > 0) {
                        doButton = $('<div>', {
                            "class": "btn btn-primary m-2 lesson-action",
                            "onclick": "BeginExam(this)",
                            "style": "cursor: pointer"
                        }).append('<i class="fas fa-play mr-2"></i>').append('Bạn còn <b>' + tryleft + '</b> lượt làm lại bài. Thực hiện lại?');
                    }
                    else {
                        doButton = $('<div>', { class: "btn btn-danger m-2" }).append('<i class="fas fa-ban mr-2"></i>').append('Hết lượt làm bài</div>');
                        doable = false;
                    }
                }
                else {
                    var doButton = $('<div>', {
                        "class": "btn btn-primary m-2 lesson-action",
                        "onclick": "$(this).prop('disabled',true); Redo(this); ",
                        "style": "cursor: pointer"
                    }).append('<i class="fas fa-play mr-2"></i>').append('Làm lại bài');
                }

            lesson_action_holder.append(doButton)
                .append(reviewButton);
        }
    }

    var renderLectureExam = function (data, isContinue) {

        var wrapper = $("<div>", { "class": "w-100 text-center partWrapper" });
        $('#rightCol').find(".partWrapper").remove();
        if (data != null) {
            var lastExam = data;
            this.exam_id = lastExam.ID;
            var lastpoint = (lastExam.MaxPoint > 0 ? (lastExam.Point * 100 / lastExam.MaxPoint) : 0);
            if (isContinue) {
                //alert('here');
                $('#rightCol').append($(wrapper));
                var completeButton = $('<div>', {
                    "class": "btn btn-primary w-50 mt-3 btnCompleteExam",
                    "onclick": 'CompleteLectureExam(\'' + lastExam.ID + '\')',
                    "style": "cursor: pointer"
                }).append('<i class="fas fa-save mr-2"></i>').append("Nộp bài");
                wrapper.append(completeButton);
                $('#rightCol').find('.tab-pane').show();
                renderQuizCounter();
            }
            else {
                $('#rightCol').prepend($(wrapper));
                var lastdate = moment(lastExam.Updated).format("DD/MM/YYYY hh:mm A");
                lastExamResult =
                    $("<div>", { id: "last-result", class: "text-center" })
                        .append($('<div>', { class: "col-md-12 text-center p-3 h5 text-info", text: "Lượt làm bài đã kết thúc lúc " + lastdate }))
                        .append($('<div>', { class: "col-md-12 text-center h4 text-success", text: "Kết quả: " + (lastExam.QuestionsPass == null ? 0 : lastExam.QuestionsPass) + "/" + lastExam.QuestionsTotal })).html();
                wrapper.append(lastExamResult);

                var reviewButton = $('<div>', {
                    "class": "btn btn-primary m-2 lesson-action",
                    "onclick": 'Review(\'' + lastExam.ID + '\')',
                    "style": "cursor: pointer"
                }).append('<i class="fas fa-poll mr-2"></i>').append("Xem đáp án");

                var doButton = $('<div>', {
                    "class": "btn btn-primary m-2 lesson-action",
                    "onclick": "$(this).prop('disabled',true); DoLectureExam(this); ",
                    "style": "cursor: pointer"
                }).append('<i class="fas fa-play mr-2"></i>').append("Làm lại bài");
                wrapper.append(doButton)
                    .append(reviewButton);
                $('#rightCol').find('.tab-pane').hide().removeClass("show");
            }
        }
        else {
            $('#rightCol').prepend($(wrapper));
            var doButton = $('<div>', {
                "class": "btn btn-primary m-3",
                "onclick": "$(this).prop('disabled',true); DoLectureExam(this); ",
                "style": "cursor: pointer",
                "text": 'Luyện tập'
            });
            wrapper.append(doButton);
            $('#rightCol').find('.tab-pane').hide();
        }
    }

    var doLectureExam = function (obj) {
        $(obj).parent().remove();
        redoExam(obj);
        $('#rightCol').find('.tab-pane').hide();
    }

    var startExam = function (obj) {
        if (obj != null)
            $(obj).prop("disabled", true);
        console.log("Create Exam");
        var dataform = new FormData();
        dataform.append("LessonID", config.lesson_id);
        dataform.append("ClassSubjectID", config.class_subject_id);
        dataform.append("ClassID", config.class_id);
        Ajax(config.url.start, dataform, "POST", false)
            .then(function (res) {
                var data = JSON.parse(res);
                if (data.Error == null) {
                    //notification("success", "Bắt đầu làm bài", 1500);
                    //console.log("NewID: " + data.Data.ID);
                    $("#ExamID").val(data.Data.ID);
                    setLocalData("CurrentExam", data.Data.ID);

                    renderExamDetail();

                    //console.log(data);
                    if (data.Data.Timer > 0) {
                        var _minutes = data.Data.Timer;
                        //console.log(_minutes);
                        var timer = (_minutes >= 10 ? _minutes : "0" + _minutes) + ":00";
                        setLocalData("Timer", timer);
                        //console.log($(".time-counter"));
                    }
                } else {
                    notification("error", data.Error, 3000);
                    if (obj != null)
                        $(obj).prop("disabled", false);
                }
            })
            .catch(function (err) {
                notification("error", err, 3000);
            });
    }

    var renderExamDetail = function () {

        renderStandardLayout(true);
        $('#' + config.container).prepend($("<input>", { type: "hidden", name: "ExamID", value: getLocalData("CurrentExam"), id: "ExamID" }));
        loadLesssonData({
            "LessonID": config.lesson_id,
            "ClassSubjectID": config.class_subject_id,
            "ClassID": config.class_id
        }, renderLessonData);
    }

    var renderStudentPart = function (data) {
        //console.log(data);
        var mainContainer = $('#' + config.container);
        var leftCol = mainContainer.find('#leftCol');
        var rightCol = mainContainer.find('#rightCol');

        var time = "", point = "";

        if (data.Timer > 0) {
            time = " (" + data.Timer + "m)";
        }
        if (data.Point > 0) {
            point = " (" + data.Point + "p)";
        }

        var container = $(leftCol);
        if (data.Type.toString().startsWith("QUIZ") || data.Type == "ESSAY")
            container = $(rightCol);
        var listPartContainer = $("#part-menu #pills-tab");

        //tabs
        var lessonitem = null;
        if ($('li #pills-' + data.ID).length > 0) {
            lessonitem = $('li #pills-' + data.ID).parent().empty()
        }
        else {
            lessonitem = $("<li>", { "class": "nav-item" });
            listPartContainer.append(lessonitem);
        }

        var itemtitle = $("<a>", { "id": "pills-" + data.ID, "class": "nav-link", "data-toggle": "pill", "href": "#pills-part-" + data.ID, "role": "tab", "aria-controls": "pills-" + data.ID, "aria-selected": "false", "text": data.Title });
        lessonitem.append(itemtitle);

        var tabsitem = null;

        if ($('#pills-part-' + data.ID).length > 0) {
            //clear old-tab
            tabsitem = $('#pills-part-' + data.ID).empty();
        }
        else
            tabsitem = $("<div>", { "id": "pills-part-" + data.ID, "class": "tab-pane" + (_UImode == UIMode.EXAM_ONLY ? " hide" : "") + " w-100", "role": "tabpanel", "aria-labelledby": "pills-" + data.ID });

        var itembox = $("<div>", { "class": "part-box " + data.Type, "id": data.ID });
        tabsitem.append(itembox);

        var ItemRow = $("<div>", { "class": "row " + (_UImode == UIMode.EXAM_ONLY ? " Q3_absrow" : "") });

        var boxHeader = $("<div>", { "class": "part-box-header row" });

        boxHeader.append($("<h5>", {
            "class": "title col-md-12", "text": (data.Title == null ? "" : data.Title) + time
            //+ point
        }));

        itembox.append(boxHeader);

        var collapseSwitch = $("<i>", { class: "fas fa-caret-down pl-2 pr-2 pt-1 pb-1", style: "cursor:pointer" });
        $(collapseSwitch).click(function () {
            toggleExpand(this);
        });

        //itembox.append(ItemRow);
        switch (data.Type) {
            case "TEXT":
                //boxHeader.append(collapseSwitch);
                var itemBody = $("<div>", { "class": "content-wrapper" });
                if (data.Description != null) {
                    itemBody.append($("<div>", { "class": "doc-content" }).html(data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn")));
                }
                itemtitle.prepend($("<i>", { "class": "far fa-file-word" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                break;
            case "IMG":
                boxHeader.find(".title").append(collapseSwitch);
                var itemBody = $("<div>", { "class": "media-wrapper collapsable collapse" });
                if (data.Description != null) {
                    itemBody.append($("<div>", { "class": "d-flex justify-content-center" }).append($("<div>", { "class": "part-description" }).html(data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn"))));
                }
                renderMediaContent(data, itemBody, "IMG");
                itemtitle.prepend($("<i>", { "class": "fas fa-file-image" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                break;
            case "AUDIO":
                boxHeader.find(".title").append(collapseSwitch);
                var itemBody = $("<div>", { "class": "media-wrapper collapsable collapse" });
                if (data.Description != null)
                    itemBody.append($("<div>", { "class": "d-flex justify-content-center" }).append($("<div>", { "class": "part-description" }).html(data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn"))));
                renderMediaContent(data, itemBody, "AUDIO");
                itemtitle.prepend($("<i>", { "class": "fas fa-music" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                break;
            case "VIDEO":
                boxHeader.find(".title").append(collapseSwitch);
                var itemBody = $("<div>", { "class": "media-wrapper collapsable collapse" });
                if (data.Description != null)
                    itemBody.append($("<div>", { "class": "d-flex justify-content-center" }).append($("<div>", { "class": "part-description" }).html(data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn"))));
                renderMediaContent(data, itemBody, "VIDEO");
                itemtitle.prepend($("<i>", { "class": "far fa-play-circle" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                break;
            case "DOC":
                boxHeader.find(".title").append(collapseSwitch);
                var itemBody = $("<div>", { "class": "media-wrapper collapsable collapse" });
                if (data.Description != null)
                    itemBody.append($("<div>", { "class": "d-flex justify-content-center" }).append($("<div>", { "class": "part-description" }).html(data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn"))));
                renderMediaContent(data, itemBody, "DOC");
                itemtitle.prepend($("<i>", { "class": "fas fa-file-word" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                break;
            case "QUIZ1":
                var itemBody = $("<div>", { "class": "quiz-wrapper" });
                itemtitle.prepend($("<i>", { "class": "fab fa-leanpub" }));
                itembox.append(itemBody);
                renderMediaContent(data, itemBody, "");
                container.append(tabsitem);
                //Render Description
                if (data.Description != null) {
                    itemBody.append($("<div>", { "class": "part-description" }).html(data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn")));
                }
                //Render Question
                totalQuiz = data.Questions.length;
                for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
                    var item = data.Questions[i];
                    renderExamQuestion(item, data.Type);
                }
                break;
            case "QUIZ2":
                var itemBody = $("<div>", { "class": "quiz-wrapper" });
                itemtitle.prepend($("<i>", { "class": "fab fa-leanpub" }));
                itembox.append(itemBody);
                renderMediaContent(data, itemBody, "");
                container.append(tabsitem);
                //Render Description
                if (data.Description != null) {
                    itemBody.append($("<div>", { "class": "part-description" }).html(data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn")));
                }
                
                //Render Question
                console.log(data.Questions.length);
                for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
                    var item = data.Questions[i];
                    renderFillQuestion(item, i);
                }
                break;
            case "QUIZ3":
                itembox.append(ItemRow);
                var itemBody = $("<div>", { "class": "quiz-wrapper col-8" });
                itemtitle.prepend($("<i>", { "class": "fab fa-leanpub" }));
                renderMediaContent(data, ItemRow, "");
                ItemRow.append(itemBody);
                if (data.Description != null) {
                    ItemRow.append($("<div>", { "class": "part-description" }).html(data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn")));
                }
                ItemRow.find(".media-holder").addClass("col-12");
                var answers_box = $("<div>", { "class": "answer-wrapper no-child col-4", "data-part-id": data.ID });
                ItemRow.append(answers_box);
                $(answers_box).droppable({
                    tolerance: "intersect",
                    accept: ".answer-item",
                    activeClass: "hasAnswer",
                    hoverClass: "answerHover",
                    drop: function (event, ui) {
                        var prevHolder = ui.helper.data('parent');
                        $(prevHolder).find(".placeholder").show();
                        $(this).append($(ui.draggable));
                    }
                });
                container.append(tabsitem);
                //Render Question
                totalQuiz = data.Questions.length;
                for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
                    var item = data.Questions[i];
                    renderExamQuestion(item, data.Type);
                }
                break;
            case "ESSAY":
                var itemBody = $("<div>", { "class": "content-wrapper" });
                if (data.Description != null) {
                    itemBody.append($("<div>", { "class": "part-description" }).html(data.Description.replace("http://publisher.edusolution.vn", "https://publisher.eduso.vn")));
                }
                itemtitle.prepend($("<i>", { "class": "fab fa-leanpub" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                var itemBody = $("<div>", { "class": "quiz-wrapper" });
                itembox.append(itemBody);

                renderMediaContent(data, itemBody, "");
                totalQuiz = data.Questions.length;
                for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
                    var item = data.Questions[i];
                    renderExamQuestion(item, data.Type);
                }
                break;
        }

        itembox.find('iframe,embed,video').each(function (i, item) {
            //if (!$(item)[0].hasAttribute('allow')) {
            //    $(item).attr('allow', 'fullscreen');
            //    $(item).attr('allowfullscreen', 'true');
            //}
            $(item).wrap($('<div>', {
                class: 'fullscreenSupport'
            }));
            $(item).parent().append($('<div>').append($('<button>', {
                class: "btn btn-primary mt-1", click: function () {
                    ShowFullScreen(this)
                }
            }).append('<i class="ic fa fa-expand mr-2"></i>').append('Xem toàn màn hình')))
        });

        if (listPartContainer.find(".nav-item").length == 1) {
            itemtitle.addClass("active");
            tabsitem.addClass("show active");
        }
        //$('.btn[title]').tooltip({
        //    trigger: 'hover'
        //});

        $('.Q3_absrow .quiz-wrapper').addClass('h-100').addClass('scrollbar-outer').scrollbar();
        $('.Q3_absrow .answer-wrapper').addClass('h-100').addClass('scrollbar-outer').scrollbar();

        startDragDrop();
    }

    var renderExamQuestion = function (data, template) {
        //render question
        var point = "";
        if (data.Point > 0) {
            point = " (" + data.Point + "p)";
        }
        switch (template) {
            case "QUIZ3":
                var container = $("#" + data.ParentID + " .quiz-wrapper");
                var quizitem = $("<div>", { "class": "quiz-item", "id": data.ID, "data-part-id": data.ParentID });

                var quiz_part = $("<div>", { "class": "quiz-pane col-6" });
                var answer_part = $("<div>", {
                    "class": "answer-pane no-child col-6",
                    "data-question-id": data.ID,
                    "data-part-id": data.ParentID,
                    "data-lesson-id": config.lesson_id,
                    "data-type": template
                });
                quizitem.append(quiz_part);
                quizitem.append(answer_part);
                var pane_item = $("<div>", { "class": "pane-item" });
                if (data.Media == null) {
                    pane_item.append($("<div>", {
                        "class": "quiz-text", "html": breakLine(data.Content)
                        //+ point
                    }));
                } else {
                    renderMediaContent(data, pane_item);
                }
                quiz_part.append(pane_item);
                container.append(quizitem);

                $(answer_part).droppable({
                    tolerance: "intersect",
                    accept: ".answer-item",
                    activeClass: "hasAnswer",
                    hoverClass: "answerHover",
                    drop: function (event, ui) {
                        $(this).find(".placeholder").hide();
                        var prevHolder = ui.helper.data('parent');

                        if ($(this).find(".answer-item").length > 0) {//remove all answer to box
                            //$(container).siblings(".answer-wrapper").append($(this).find(".answer-item"));
                            $(prevHolder).append($(this).find(".answer-item"));
                        }
                        else {
                            $(prevHolder).find(".placeholder").show();
                        };
                        $(this).append($(ui.draggable));
                    }
                });

                for (var i = 0; data.CloneAnswers != null && i < data.CloneAnswers.length; i++) {
                    var item = data.CloneAnswers[i];
                    renderExamAnswer(item, data.ParentID, template);
                }
                break;
            default:
                var container = $("#" + data.ParentID + " .quiz-wrapper");

                var itembox = $("<div>", { "class": "quiz-item", "id": data.ID, "data-part-id": data.ParentID });
                var boxHeader = $("<div>", { "class": "quiz-box-header" });
                if (data.Content != null)
                    boxHeader.append($("<h5>", {
                        "class": "title", "html": breakLine(data.Content)
                        //+ point
                    }));
                else
                    boxHeader.append($("<h5>", { "class": "title", "text": point }));

                renderMediaContent(data, boxHeader);

                itembox.append(boxHeader);
                var answer_wrapper = $("<div>", { "class": "answer-wrapper" });

                itembox.append(answer_wrapper);

                container.append(itembox);

                //Render Answer
                for (var i = 0; data.CloneAnswers != null && i < data.CloneAnswers.length; i++) {
                    var item = data.CloneAnswers[i];
                    renderExamAnswer(item, data.ParentID, template);
                }
                break;
        }
    }

    var renderFillQuestion = function (data, pos) {

        var container = $("#" + data.ParentID + " .quiz-wrapper .part-description");

        var holder = $(container).find("fillquiz")[pos];
        if (holder == null) return;
        //console.log(holder);
        var input = $(holder).find(".fillquiz");
        $(holder).addClass("quiz-item").attr("id", data.ID);
        //console.log(data);
        $(input)
            .attr("id", "inputQZ2-" + data.ID)
            .attr("data-part-id", data.ParentID)
            .attr("data-lesson-id", config.lesson_id)
            .attr("data-question-id", data.ID)
            .attr("data-type", "QUIZ2")
            .attr("autocomplte", "off")
            .attr("value", "")
            .removeAttr("ans")
            .removeAttr("readonly")
            .removeAttr("contenteditable")
            .attr("placeholder", data.Content)
            .blur(function () {
                AnswerQuestion(this);
            });
    }

    var renderExamAnswer = function (data, partid, template) {
        var container = $("#" + data.ParentID + " .answer-wrapper");
        var answer = $("<fieldset>", { "class": "answer-item", id: data.ID });
        switch (template) {
            case "QUIZ2":
                if ($(container).find(".answer-item").length == 0) {
                    answer.append($("<input>", {
                        "type": "text",
                        "class": "input-text answer-text form-control",
                        "onfocusout": "AnswerQuestion(this)",
                        "id": "inputQZ2-" + data.ParentID,
                        //"data-id": data.ID,
                        "data-part-id": partid,
                        "data-lesson-id": config.lesson_id,
                        "data-question-id": data.ParentID,
                        "data-type": template,
                    }).attr("autocomplte", "off"));
                    container.append(answer);
                }
                break;
            case "QUIZ3":
                var placeholder = $("#" + data.ParentID).find(".answer-pane");
                $(placeholder).removeClass("no-child");
                placeholder.empty().append($("<div>", {
                    "class": "pane-item placeholder",
                    "text": "Drop your answer here"
                }));

                container = $("#" + data.ParentID).parent().siblings(".answer-wrapper");

                if (data.Content != null)
                    answer.append($("<input>", { "type": "hidden", "value": data.Content }));

                if (data.Media != null) {
                    renderMediaContent(data, answer);
                }
                else
                    answer.append($("<label>", { "class": "answer-text", "html": breakLine(data.Content) }));

                var x = Math.floor((Math.random() * 2));

                if (x > 0)
                    container.append(answer);
                else
                    container.prepend(answer);

                //container.append(answer);
                break;
            default:
                var form = $("<div>", { "class": "form-check" });
                answer.append(form);
                form.append($("<input>", { "type": "hidden" }));
                form.append($("<input>", {
                    "id": data.ID, "type": "radio",
                    "class": "input-checkbox answer-checkbox form-check-input",
                    "onclick": "AnswerQuestion(this)",
                    "data-part-id": partid,
                    "data-lesson-id": config.lesson_id,
                    "data-question-id": data.ParentID,
                    "data-id": data.ID,
                    "data-type": template,
                    "data-value": data.Content,
                    "name": "rd_" + data.ParentID
                }));
                if (data.Content != null)
                    form.append($("<label>", { "class": "answer-text form-check-label", "for": data.ID, "html": breakLine(data.Content) }));
                renderMediaContent(data, answer);
                container.append(answer);
                break;
        }

    }

    var completeExam = function (isOvertime) {
        var lesson_action_holder = $('.top-menu[for=lesson-info]');
        lesson_action_holder.empty();
        if ($('#QuizNav').hasClass("show"))
            toggleNav();
        if (isOvertime || true) {
            //var exam = document.querySelector("input[name='ExamID']");
            //var exam = getLocalData("CurrentExam");
            var dataform = new FormData();
            //console.log("Complete :" + $('#ExamID').val());
            dataform.append("ExamID", $('#ExamID').val());
            Ajax(config.url.end, dataform, "POST", true)
                .then(function (res) {
                    stopCountdown();
                    var data = JSON.parse(res);
                    //if (isOvertime)
                    //    notification("success", "Thời gian làm bài đã hết", 1500);
                    //else
                    //    notification("success", "Đã nộp bài", 1500);
                    localStorage.clear();
                    renderCompleteExam(data);
                })
                .catch(function (err) {
                    console.log(err);
                });
        }
    }

    var completeLectureExam = function () {
        var dataform = new FormData();
        console.log("Complete :" + $('#ExamID').val());
        dataform.append("ExamID", $('#ExamID').val());
        Ajax(config.url.end, dataform, "POST", true)
            .then(function (res) {
                stopCountdown();
                var data = JSON.parse(res);
                //notification("success", "Đã nộp bài", 3000);
                localStorage.clear();
                document.location.href = window.location.href.substr(0, window.location.href.indexOf('#'));
            })
            .catch(function (err) {
                console.log(err);
            });
    }

    var renderCompleteExam = function (data) {
        var lesson_action_holder = $('.top-menu[for=lesson-info]');

        var wrapper = $("<div>", { "class": "p-3 col-md-12 text-center" });
        $('#' + config.container).empty().append($(wrapper));
        if (isNull(data)) {
            var doButton = $('<div>', {
                "class": "btn btn-primary m-2",
                "onclick": "$(this).prop('disabled',true); BeginExam(this);",
                "style": "cursor: pointer",
                "text": "Làm bài"
            });
            //var backButton = $('<div>', {
            //    "class": "btn btn-primary m-3",
            //    "onclick": "$(this).prop('disabled',true); GoBack();",
            //    "style": "cursor: pointer",
            //    "text": "Về danh sách"
            //});
            lesson_action_holder.append(doButton);
            //wrapper.append(backButton);
        }
        else {
            var lastExam = data;

            var lastpoint = (lastExam.maxPoint > 0 ? (lastExam.point * 100 / lastExam.maxPoint) : 0);

            var limit = lastExam.limit;
            var tried = lastExam.number;
            console.log(lastExam);
            lastExamResult =
                $("<div>", { id: "last-result", class: "text-center" })
                    .append($('<div>', { class: "col-md-12 text-center p-3 h5 text-info", text: "Chúc mừng! Bạn đã hoàn thành bài kiểm tra (lần " + tried + ")" }))
                    .append($('<div>', { class: "col-md-12 text-center h4 text-success", text: "Kết quả: " + (lastExam.questionsPass == null ? 0 : lastExam.questionsPass) + "/" + lastExam.questionsTotal }));

            wrapper.append(lastExamResult);
            //console.log(data);

            tryleft = limit - tried;

            //var backButton = $('<div>', {
            //    "class": "btn btn-primary m-3",
            //    "onclick": "GoBack()",
            //    "style": "cursor: pointer",
            //    "text": "Về danh sách"
            //});
            var reviewButton = $('<div>', {
                "class": "btn btn-primary m-2",
                "onclick": 'Review(\'' + lastExam.id + '\'); $(this).prop(\'disabled\',true)',
                "style": "cursor: pointer"
            }).append('<i class="fas fa-poll mr-2"></i>').append("Xem đáp án");
            if (limit > 0) {
                tryleft = limit - tried;
                if (tryleft > 0) {
                    var doButton = $('<div>', {
                        "class": "btn btn-primary m-2",
                        "onclick": "$(this).prop('disabled',true); BeginExam(this);",
                        "style": "cursor: pointer"
                    }).append('<i class="fas fa-play mr-2"></i>').append('Bạn còn <b>' + tryleft + '</b> lượt làm lại bài. Thực hiện lại?');
                }
                else {
                    doButton = $('<div>', { class: "btn btn-danger m-2" }).append('<i class="fas fa-ban mr-2"></i>').append('Hết lượt làm bài');
                    doable = false;
                }
            }
            else {
                var doButton = $('<div>', {
                    "class": "btn btn-primary m-2",
                    "onclick": "$(this).prop('disabled',true); Redo(this);",
                    "style": "cursor: pointer"
                }).append('<i class="fas fa-play mr-2"></i>').append('Làm lại bài');
            }
            console.log(doButton);
            $(lesson_action_holder).append(doButton)
                .append(reviewButton);
            console.log(lesson_action_holder);
            //.append(backButton);
        }
    }

    //student review
    var renderStudentReview = function (data) {
        loadLesssonData({
            "LessonID": config.lesson_id,
            "ClassID": config.class_id,
            "ExamID": config.exam_id
        }, renderLessonData);
    }

    var renderLecture = function () {
        loadLesssonData({
            "LessonID": config.lesson_id,
            "ClassSubjectID": config.class_subject_id,
            "ClassID": config.class_id
        }, renderLessonData);
    }

    var goBack = function () {
        document.location = "/student/Course/Modules/" + config.class_id;
    }

    var redoExam = function (obj) {
        var lesson_action_holder = $('.top-menu[for=lesson-info]')
        lesson_action_holder.empty()
        console.log("Redo Exam");
        localStorage.clear();
        startExam(obj);
    }

    var review = function (examid) {
        document.location = config.url.review + '/' + examid;
    }

    /// tồn tại exam cũ thì return true;
    var checkExam = function () {
        return !isNull(getLocalData("CurrentExam"));
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
        //writeLog(url, data);
        var request = new XMLHttpRequest();
        return new Promise(function (resolve, reject) {
            // Setup our listener to process completed requests
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
        //if (config.mod != mod.STUDENT_EXAM)
        //    return;
        // dataset trên item
        var dataset = _this.dataset;
        //console.log(dataset);
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
        console.log(dataset);
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
                //answerID = dataset.id;
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
        dataform.append("ExamID", $("input[name=ExamID]").val());
        //console.log($("input[name=ExamID]"));
        if (type != "ESSAY") {

            dataform.append("LessonPartID", partID);
            dataform.append("AnswerID", answerID);
            dataform.append("QuestionID", questionId);
            dataform.append("AnswerValue", value);
        } else {
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

    var delAnswerForStudent = function (quizID) {
        removeLocalData(quizID);

        var dataform = new FormData();
        dataform.append("ExamID", $('#ExamID').val());
        dataform.append("QuestionID", quizID);
        Ajax(config.url.removeans, dataform, "POST", false)
            .then(function (res) {

                renderQuizCounter();
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
        removeLocalData(quizID);
        var dataform = new FormData();
        dataform.append("ExamID", $('#ExamID').val());
        dataform.append("QuestionID", quizID);
        Ajax(config.url.removeans, dataform, "POST", false)
            .then(function (res) {
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
        setLocalData(quizID, value);
        renderQuizCounter();
        //startDragDrop();
        var xxx = document.getElementById("quizNav" + quizID);
        if (xxx != null) {
            xxx.classList.add("completed");
        }
    }

    var renderQuizCounter = function () {
        //console.log("render");
        var listQuiz = document.querySelectorAll(".quiz-item");
        var count = 0;
        var answerList = '';
        //writeLog("renderQuizCounter", listQuiz);
        for (var i = 0; listQuiz != null && i < listQuiz.length; i++) {
            var item = listQuiz[i];
            var answer = getLocalData(item.id);
            //console.log(item.id);
            var completed = "";
            if (answer != null && answer != void 0 && answer != "") {
                //console.log(answer);
                count++;
                completed = "completed";
                rendAgain(answer);
            }
            answerList += '<button class="btn bg-secondary text-white rounded-quiz ' + completed + '" type="button" id="quizNav' + item.id + '" name="quizNav' + item.id + '" onclick="window.GoQuiz(\'' + item.id + '\')">' + (i + 1) + '</button>';
        }

        var quiz_number_counter = $('#quiz_number_counter');
        if (quiz_number_counter.length > 0) {
            quiz_number_counter.find(".completed").text(count);
            quiz_number_counter.find(".total").text(listQuiz.length);
        } else {
            $(".top-menu[for=lesson-info] .prevtab").after($('<button>', {
                id: "quiz_number_counter",
                class: "quizNumber btn btn-primary mt-2 mr-2 mb-2",
                onclick: "ToggleNav(this)",
                tooltips: "Toggle Scoreboard"
            })
                .append($("<i>", { class: "fas fa-question-circle mr-2" }))
                .append("Câu đã trả lời ").append($("<span>", {
                    class: "completed ml-1",
                    text: count
                })).append(" / ")
                .append($("<span>", {
                    class: "total",
                    text: listQuiz.length
                }))
            );
        }
        //var html = '<div id="quizNavigator" class="overlay">';
        //html += '<a href="javascript:void(0)" class="closebtn" onclick="closeNav()">×</a>';
        //html += '<div class="overlay-content card-body">';
        //html += '<div class="input-group mb-3 quiz-wrapper">';
        //html += answerList;
        //html += '<div style="display:none" id="btn-completed" class="d-flex justify-content-center pt-5 pb-5"><button class="btn btn-primary" onclick="ExamComplete()" data-original-title="" title=""> Nộp bài </button></div>';
        //html += '</div>'
        //html += '</div>';
        //html += '</div>';

        var quizNavigator = $('#quizNavigator');
        if (quizNavigator.length == 0) {
            quizNavigator = $('<div>', { id: "quizNavigator", class: "p-1" });
            quizNavigator.append($('<div>', { class: "input-group quiz-wrapper" }).append(answerList));
            $('#quizIdx_holder').append(quizNavigator);
        }
        $("#QuizNav").append(quizNavigator);
        if (listQuiz != null && count >= listQuiz.length) {
            //console.log(count, listQuiz.length);
            var btn = document.getElementById("btn-completed");
            if (btn != null) btn.style.display = "block";
        } else {
            var btn = document.getElementById("btn-completed");
            if (btn != null) btn.style.display = "none!important";
        }

        startDragDrop();
    }

    var rendAgain = function (value) {
        //console.log(value);
        var arr = value.split('~~');
        var quizID = arr[0];
        var answerID = arr[1];
        var answerValue = arr[2];
        var type = arr[3];
        switch (type) {
            case "QUIZ1":
                var answer = $('#' + answerID);
                $(answer).find("input[type='radio']").attr("checked", "");
                break;
            case "QUIZ2":
                var quiz = $('#inputQZ2-' + quizID);
                //console.log(answerValue);
                //console.log(quizID);
                $(quiz).val(answerValue);
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

    window.LessonInstance = {} || Lesson;

    LessonInstance.onReady = onReady;
    LessonInstance.Version = version;
    LessonInstance.Notification = notification;
    window.BeginExam = startExam;
    window.AnswerQuestion = AnswerQuestion;
    window.CompleteExam = completeExam;
    window.CompleteLectureExam = completeLectureExam;
    window.Redo = redoExam;
    window.DoLectureExam = doLectureExam;
    window.GoBack = goBack;
    window.GoQuiz = goQuiz;

    window.EditLesson = modalEditLesson;
    window.EditPart = modalEditPart;
    window.AddPart = modalAddPart;
    window.SelectTemplate = modalSelectTemplate;

    window.RemovePart = removePart;
    window.SortPart = sortPart;

    window.RemoveQuestion = removeQuestion;
    window.AddNewQuestion = addNewQuestion;
    window.ShowCloneQuestion = showCloneQuestion;
    window.ChooseQuestionFile = chooseQuestionFile;
    window.UploadQuestionFile = uploadQuestionFile;
    window.DownloadQuestionTemplate = downloadQuestionTemplate;

    window.RemoveAnswer = removeAnswer;
    window.AddNewAnswer = addNewAnswer;
    window.ToggleCorrectAnswer = toggleCorrectAnswer;
    window.ToggleExplanation = toggleExplanation;
    window.ToggleNav = toggleNav;
    window.ReloadData = reloadData;
    window.Review = review;

    window.NextPart = nextPart;
    window.PrevPart = prevPart;

    window.SwitchMode = switchMode;
    return LessonInstance;
}());

function chooseFile(obj) {
    $(obj).siblings("input[type=file]").focus().click();
}

function changeMedia(obj) {
    $(obj).uniqueId();
    $(obj).attr("name", $(obj).attr("id"));
    var file = $(obj)[0].files[0];
    $(obj).siblings("[for=medianame]").val($(obj).attr("id"));
    $(obj).siblings(".btnAddFile").val(file.name);
    $(obj).siblings("[for=mediaorgname]").val(file.name);
    $(obj).siblings("[for=mediaext]").val(file.type);
    $(obj).siblings("[for=mediapath]").val("");//clear path
    $(obj).siblings(".media_preview").remove();
    $(obj).siblings(".btnResetFile").show();
}

function resetMedia(obj) {
    $(obj).siblings(".btnAddFile").val("Choose media");
    $(obj).siblings("input[type=file]").val('').attr("name", "");
    $(obj).siblings("[for=medianame]").val('');
    $(obj).siblings("[for=mediaorgname]").val('');
    $(obj).siblings("[for=mediaext]").val('');
    $(obj).siblings("[for=mediapath]").val('');
    $(obj).parent().siblings(".media_preview").remove();
    $(obj).hide();
}

var hideModal = function (modalId) {
    if (modalId != null)
        $(modalId).modal('hide');
    else
        $(".modal").modal('hide');
};

var submitForm = function (event, modalId, callback) {
    event.preventDefault();
    $('.btnSaveForm').hide();

    var form = $(modalId).find('form');
    var Form = form.length > 0 ? form[0] : window.partForm;
    var formdata = new FormData(Form);

    if ($('textarea[name="Description"]').length > 0) {
        formdata.delete("Description");

        //formdata.append("Description", myEditor.getData())
        formdata.append("Description", CKEDITOR.instances.editor.getData())
    }
    var err = false;
    var requires = $(Form).find(':required');

    document.activeElement.blur();

    requires.each(function () {
        var obj = $(this);
        if ($(this).val() == "" || $(this).val() == null) {
            Swal.fire({
                title: 'Lưu ý',
                text: "Vui lòng điền đầy đủ nội dung",
                icon: 'warning',
                confirmButtonText: "Đóng"
            }).then(() => {
                $('.btnSaveForm').show();
                $(obj).focus();
            });
            err = true;
            return false;
        }
    });
    if (err) return false;

    var xhr = new XMLHttpRequest();

    var actionUrl = $("#action").val()
    if (form.length > 0)
        actionUrl = form.attr('action');

    $('.btnSaveForm').after($("<div>", { class: "pending", text: "Đang gửi dữ liệu, vui lòng đợi..." }));
    xhr.open('POST', actionUrl);
    xhr.send(formdata);
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            var data = JSON.parse(xhr.responseText);
            if (data.Error == null || data.Error == "") {
                //switch (actionUrl) {
                //case "Lesson/" + urlLesson.CreateOrUpdate:
                //render.lesson(data.data);
                //document.location = urlLesson.Location + data.Data.ID;
                //document.location = document.location;
                //    break;
                //case "LessonPart/" + urlLessonPart.CreateOrUpdate:
                //    var part = data.Data;
                //   //render.part(part);
                if (callback == "addPart") {
                    var part = data.Data;
                    window.AddPart(part);
                }
                else {
                    if (callback == null)
                        window.ReloadData();
                    else
                        callback;
                }
                hideModal(modalId);
            }
            else {
                alert(data.Error);
            }
        }
        $('.btnSaveForm').siblings('.pending').remove();
        $('.btnSaveForm').show();
    }
}

var submitQuizFill = function (event, modalId, callback) {
    event.preventDefault();
    $('.btnSaveForm').hide();

    var form = $(modalId).find('form');
    var Form = form.length > 0 ? form[0] : window.partForm;
    var formdata = new FormData(Form);

    if ($('textarea[name="Description"]').length > 0) {
        formdata.delete("Description");

        var CKEContent = CKEDITOR.instances.editor.getData();
        var description = $.parseHTML("<div>" + CKEContent + "</div>");
        var quizs = $(description).find("fillquiz");
        if (quizs.length > 0) {
            var pos = -1;
            for (var i = 0; i < quizs.length; i++) {
                var question = quizs[i];
                var inp = $(question).find("input");
                if (inp.length == 0) {
                    $(question).attr('bad', 1);
                    continue; //skip empty fillquiz
                }
                pos++;
                //console.log(inp);
                var answers = $(inp).attr("ans") == null ? $(inp).attr("placeholder") : $(inp).attr("ans");
                var display = $(inp).attr("dsp");
                if (display == null) display = "";
                var explanation = $(question).attr("title");
                if (explanation == null) explain = "";

                if (answers != null) {
                    //console.log(answers);    
                    formdata.append("Questions[" + pos + "].Order", pos);
                    formdata.append("Questions[" + pos + "].Point", 1);
                    formdata.append("Questions[" + pos + "].Content", display);
                    formdata.append("Questions[" + pos + "].Description", explanation);

                    var ans = answers.split("|");
                    if (ans.length > 0) {
                        for (var j = 0; j < ans.length; j++) {
                            var an = ans[j].trim();
                            formdata.append("Questions[" + pos + "].Answers[" + j + "].Content", an);
                            formdata.append("Questions[" + pos + "].Answers[" + j + "].IsCorrect", true);
                        }
                        //console.log(ans);
                    }

                    $(question).removeAttr("title");
                    $(question).find("input").removeAttr("ans");
                    $(question).find("input").removeAttr("dsp");
                    $(question).find("input").attr("placeholder", "");
                    $(question).find("input").attr("size", "10");
                    $(question).find("input").attr("value", "");
                }
            }
        }
        //console.log(CKEContent);
        //console.log($(description).prop("innerHTML"));
        $(description).find("[bad=1]").remove();//remove all bad fillquiz;

        //console.log("Description", $(description).prop("innerHTML"));
        formdata.append("Description", $(description).prop("innerHTML"));
    }
    var err = false;
    var requires = $(Form).find(':required');

    document.activeElement.blur();

    requires.each(function () {
        var obj = $(this);
        if ($(this).val() == "" || $(this).val() == null) {
            Swal.fire({
                title: 'Lưu ý',
                text: "Vui lòng điền đầy đủ nội dung",
                icon: 'warning',
                confirmButtonText: "Đóng"
            }).then(() => {
                $('.btnSaveForm').show();
                $(obj).focus();
            });
            err = true;
            return false;
        }
    });
    if (err) return false;

    var xhr = new XMLHttpRequest();

    var actionUrl = $("#action").val()
    if (form.length > 0)
        actionUrl = form.attr('action');

    $('.btnSaveForm').after($("<div>", { class: "pending", text: "Đang gửi dữ liệu, vui lòng đợi..." }));
    xhr.open('POST', actionUrl);
    xhr.send(formdata);
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            var data = JSON.parse(xhr.responseText);
            if (data.Error == null || data.Error == "") {
                //switch (actionUrl) {
                //case "Lesson/" + urlLesson.CreateOrUpdate:
                //render.lesson(data.data);
                //document.location = urlLesson.Location + data.Data.ID;
                //document.location = document.location;
                //    break;
                //case "LessonPart/" + urlLessonPart.CreateOrUpdate:
                //    var part = data.Data;
                //   //render.part(part);
                if (callback == "addPart") {
                    var part = data.Data;
                    window.AddPart(part);
                }
                else {
                    if (callback == null)
                        window.ReloadData();
                    else
                        callback;
                }
                hideModal(modalId);
            }
            else {
                alert(data.Error);
            }
        }
        $('.btnSaveForm').siblings('.pending').remove();
        $('.btnSaveForm').show();
    }
}

var toggleExpand = function (obj) {
    $('.collapsable').addClass('collapse');
    $('.media-holder video').trigger('pause');

    if (!$(obj).hasClass("fa-caret-up"))//expand
    {
        $('.title > .fa-caret-up').removeClass("fa-caret-up");
        $(obj).addClass("fa-caret-up");
        $(obj).parent().parent().siblings('.collapse').removeClass("collapse");

        $('#leftCol').animate({
            scrollTop: $(obj).parent().parent().parent().parent().offset().top - 60
        }, 500);
    }
    else {
        $(obj).removeClass("fa-caret-up");
    }

}