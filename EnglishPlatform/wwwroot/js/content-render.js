﻿
var Debug = true;

var mod = {
    PREVIEW: "preview",
    EDIT: "edit",
    EXAM: "exam",
    REVIEW: "review"
}

var TEMPLATE_TYPE = {
    EXAM: 2,
    LESSON: 1
}

var UIMode = {
    LEFT: 1,
    RIGHT: 2,
    BOTH: 3
}

var writeLog = function (name, msg) {
    if (Debug) {
        console.log(name, msg);
    }
}

var Lesson = (function () {

    var _data = null;

    var _UImode = null;

    var exam_timeout = null;

    var config = {
        container: "",
        mod: "",//preview, edit, exam, review
        url: {
            load: "", //all: load lesson data
            save: "", //edit: save lesson data
            load_part: "",//all: load lesson part data
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
        switch (config.mod) {
            case mod.PREVIEW:
                renderPreview();
                break;
            case mod.EDIT:
                renderEdit();
                break;
            case mod.EXAM:
                renderExam();
                break;
            case mod.REVIEW:
                renderReview();
                break;
        }
    }

    //local storage function
    var setLocalData = function (key, data) {
        var enData = b64EncodeUnicode(JSON.stringify(data));
        localStorage.setItem(config.lesson_id + "_" + key, enData);
    }

    var getLocalData = function (key) {
        return localStorage.getItem(config.lesson_id + "_" + key);
    }

    var removeLocalData = function (key) {
        return localStorage.removeItem(config.lesson_id + "_" + key);
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

    //Preview
    var renderPreview = function () {
        renderStandardLayout();
        //Prepare data
        if (isNull(_data)) {
            loadLesssonData({ ID: config.lesson_id }, renderLessonData);
        }
        else {
            //Render Content
            renderLessonData();
        }
    }

    //Lesson
    var loadLesssonData = function (param, callback) {
        var formData = new FormData();
        if (param != null)
            Object.keys(param).forEach(e => formData.append(e, param[e]));
        Ajax(config.url.load, formData, "POST", true).then(function (res) {
            if (!isNull(res)) {
                _data = JSON.parse(res).Data;
                loadLessonParts({ LessonID: config.lesson_id }, callback);
            }
        });
    }

    //LessonPart
    var loadLessonParts = function (param, callback) {
        var formData = new FormData();
        if (param != null)
            Object.keys(param).forEach(e => formData.append(e, param[e]));
        Ajax(config.url.load_part, formData, "POST", true).then(function (res) {
            _data.Part = JSON.parse(res).Data;
            callback();
        });
    }


    //render content

    var renderStandardLayout = function () {
        var container = $('#' + config.container);

        var lessonBox = $("<div>", { "class": "lesson lesson-box h-100" });
        container.append(lessonBox);

        var lessonContainer = $("<div>", { "class": "lesson-container h-100" });
        lessonBox.append(lessonContainer);
        var lessonContent = $("<div>", { "class": "card h-100" });
        lessonContainer.append(lessonContent);

        //header row (contain title; lesson action - default hide)
        var lessonHeader = $("<div>", { "class": "card-header", style: "display:none" });
        lessonContent.append(lessonHeader);

        //main content (lesson content; part list...., 2 cols interface)
        var cardBody = $("<div>", { "class": "card-body h-100 position-relative" });
        lessonContent.append(cardBody);

        var lessonBody = $("<div>", { "class": "lesson-body h-100", "id": config.lesson_id });
        cardBody.append(lessonBody);

        //footer row (part navigation, question navigation... - default: hide)
        var lessonFooter = $('<div>', { "class": "card-footer", "style": "display:none" });
        lessonContent.append(lessonFooter);


        var partsHolder = $("<div>", { "id": "pills-tabContent", "class": "tab-content h-100 row" });
        //render 2 columns layout
        partsHolder.append($("<div>", { "class": "col-md-6 h-100 main-column pr-1 pl-1", "id": "leftCol" }))
            .append($("<div>", { "class": "col-md-6 h-100 main-column pr-1 pl-1", "id": "rightCol" }));

        lessonBody.append(partsHolder);
        $('.main-column').addClass('scrollbar-outer').scrollbar();
    }

    var renderLessonData = function () {

        if (isNull(_data)) {
            throw "No data";
        }

        var data = _data;

        var mainContainer = $('#' + config.container);

        var lessonHeader = mainContainer.find('.card-header');
        var lessonBody = mainContainer.find('.card-body');
        var lessonFooter = mainContainer.find('.card-footer');

        //header
        switch (config.mod) {
            case mod.PREVIEW:
                var headerRow = $("<div>", { "class": "row" });
                lessonHeader.show().append(headerRow);

                var title_wrapper = $("<div>", { "class": "lesson-header-title col-md-9 col-sm-12" });
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

                var lessonButton = $("<div>", { "class": "lesson-button col-md-3 col-sm-12 text-right" });
                var sort = $("<button>", { "class": "btn btn-primary btn-sort", "title": "Sort", "onclick": "lessonService.renderSort()" });
                var edit = $("<button>", { "class": "btn btn-primary btn-edit", "title": "Edit", "data-toggle": "modal", "data-target": "#lessonModal", "onclick": "EditLesson('" + data.ID + "')" });
                var create = $("<button>", { "class": "btn btn-primary btn-add", "title": "Add", "data-toggle": "modal", "data-target": "#partModal", "onclick": "Create.lessonPart('" + data.ID + "')" });
                //var close = $("<button>", { "class": "btn btn-primary btn-close", "text": "X", "onclick": "render.resetLesson()" });
                var remove = $("<button>", { "class": "btn btn-danger btn-remove", "title": "Remove", "onclick": "lessonService.remove('" + data.ID + "')" });

                var iconSort = $("<i>", { "class": "fas fa-sort" });
                var iconEdit = $("<i>", { "class": "fas fa-edit" });
                var iconCreate = $("<i>", { "class": "fas fa-plus-square" });
                var iconTrash = $("<i>", { "class": "fas fa-trash" });
                lessonButton.append(iconSort);

                lessonButton.append(sort);
                sort.append(iconSort);
                lessonButton.append(edit);
                edit.append(iconEdit);
                lessonButton.append(create);
                create.append(iconCreate);
                lessonButton.append(remove); //removeLesson
                remove.append(iconTrash);

                headerRow.append(lessonButton);

                break;
            case mod.EXAM:
                break;
            case mod.REVIEW:
                break;
        }

        //body
        switch (config.mod) {
            case mod.PREVIEW:
                var partMenu = $("<div>", { "id": "part-menu", "class": "w-100", "style": "display:none;" });
                lessonBody.append(partMenu);
                var lessontabs = $("<div>", { "class": "lesson-tabs" });
                partMenu.append(lessontabs);
                var tabs = $("<ul>", { "id": "pills-tab", "class": "nav flex-column nav-pills", "role": "tablist", "aria-orientation": "vertical" });
                lessontabs.append(tabs);
                for (var i = 0; data.Part != null && i < data.Part.length; i++) {
                    var item = data.Part[i];
                    renderDataPart(item);
                    switch (item.Type) {
                        case "QUIZ1":
                        case "QUIZ2":
                        case "QUIZ3":
                        case "ESSAY":
                            switchUIMode(UIMode.RIGHT);
                            break;
                        default:
                            switchUIMode(UIMode.LEFT);
                    }
                }
                break;
            case mod.EXAM:
                break;
            case mod.REVIEW:
                break;
        }

        //footer: navigation
        switch (config.mod) {
            case mod.PREVIEW:

                if (_UImode != UIMode.BOTH) {
                    var nav_bottom = $('<div>', { "class": "row" });

                    var prevtab = $("<button>", { "class": "prevtab btn btn-success mr-2", "title": "Prev", "onclick": "tab_goback()" });
                    var iconprev = $("<i>", { "class": "fas fa-arrow-left" });
                    var nexttab = $("<button>", { "class": "nexttab btn btn-success", "title": "Next", "onclick": "tab_gonext()" });
                    var iconnext = $("<i>", { "class": "fas fa-arrow-right" });
                    prevtab.append(iconprev);
                    nexttab.append(iconnext);

                    nav_bottom.append($('<div>', { "class": "col-md-2 text-left" }).append(prevtab));
                    nav_bottom.append($('<div>', { "class": "col-md-8 text-center" }));
                    nav_bottom.append($('<div>', { "class": "col-md-2 text-right" }).append(nexttab));

                    lessonFooter.show().append(nav_bottom);
                }
                break;
            case mod.EXAM:
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
            case UIMode.LEFT:
                leftCol.removeClass("col-md-6").removeClass("col-md-12").addClass("col-md-12").show();
                rightCol.hide();
                break;
            case UIMode.RIGHT:
                leftCol.hide();
                rightCol.removeClass("col-md-6").removeClass("col-md-12").addClass("col-md-12").show();
                break;
            case UIMode.BOTH:
                leftCol.removeClass("col-md-6").removeClass("col-md-12").addClass("col-md-6").show();
                rightCol.removeClass("col-md-6").removeClass("col-md-12").addClass("col-md-6").show();
                break;
        }
    }

    var renderDataPart = function (data) {
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
            tabsitem = $("<div>", { "id": "pills-part-" + data.ID, "class": "tab-pane w-100", "role": "tabpanel", "aria-labelledby": "pills-" + data.ID });

        var itembox = $("<div>", { "class": "part-box " + data.Type, "id": data.ID });
        tabsitem.append(itembox);

        var ItemRow = $("<div>", { "class": "row" });

        var boxHeader = $("<div>", { "class": "part-box-header row" });
        if (data.Title != null) {
            boxHeader.append($("<h5>", { "class": "title col-md-10", "text": data.Title + time + point }));
        }
        //boxHeader.append($("<a>", { "class": "btn btn-sm btn-view", "text": "Collapse", "onclick": "toggleCompact(this)" }));

        var iconEdit = $("<i>", { "class": "fas fa-edit" });
        var iconCreate = $("<i>", { "class": "fas fa-plus-square" });
        var iconTrash = $("<i>", { "class": "fas fa-trash" });

        var boxButton = $("<div>", { "class": "text-right col-md-2" });
        boxButton.append($("<button>", { "class": "btn btn-primary btn-sm mr-1", "title": "Edit", "data-toggle": "modal", "data-target": "#partModal", "onclick": "lessonPartService.edit('" + data.ID + "')" }).append(iconEdit))
        boxButton.append($("<button>", { "class": "btn btn-danger btn-sm", "title": "Remove", "onclick": "lessonPartService.remove('" + data.ID + "')" }).append(iconTrash));

        itembox.append(boxHeader);
        boxHeader.append(boxButton);
        //itembox.append(ItemRow);
        switch (data.Type) {
            case "TEXT":
                var itemBody = $("<div>", { "class": "content-wrapper" });
                if (data.Description != null) {
                    itemBody.append($("<div>", { "class": "doc-content" }).html(data.Description));
                }
                itemtitle.prepend($("<i>", { "class": "far fa-file-word" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                break;
            case "IMG":
                var itemBody = $("<div>", { "class": "media-wrapper" });
                renderMediaContent(data, itemBody, "IMG");
                if (data.Description != null)
                    wrapper.append($("<div>", { "class": "description", "text": data.Description }));
                itemtitle.prepend($("<i>", { "class": "fas fa-file-image" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                break;
            case "AUDIO":
                var itemBody = $("<div>", { "class": "media-wrapper" });
                renderMediaContent(data, itemBody, "AUDIO");
                if (data.Description != null)
                    wrapper.append($("<div>", { "class": "description", "text": data.Description }));
                itemtitle.prepend($("<i>", { "class": "fas fa-music" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                break;
            case "VIDEO":
                var itemBody = $("<div>", { "class": "media-wrapper" });
                renderMediaContent(data, itemBody, "VIDEO");
                if (data.Description != null)
                    wrapper.append($("<div>", { "class": "description", "text": data.Description }));
                itemtitle.prepend($("<i>", { "class": "far fa-play-circle" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                break;
            case "DOC":
                var itemBody = $("<div>", { "class": "media-wrapper" });
                renderMediaContent(data, itemBody, "DOC");
                if (data.Description != null)
                    wrapper.append($("<div>", { "class": "description", "text": data.Description }));
                itemtitle.prepend($("<i>", { "class": "fas fa-file-word" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                break;
            case "QUIZ1":
            case "QUIZ2":
                var itemBody = $("<div>", { "class": "quiz-wrapper" });
                itemtitle.prepend($("<i>", { "class": "fab fa-leanpub" }));
                itembox.append(itemBody);
                renderMediaContent(data, itemBody, "");
                container.append(tabsitem);
                //Render Content

                //Render Question
                totalQuiz = data.Questions.length;
                for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
                    var item = data.Questions[i];
                    renderQuestionView(item, data.Type);
                }
                break;
            case "QUIZ3":
                var itemBody = $("<div>", { "class": "quiz-wrapper col-8" });
                itemtitle.prepend($("<i>", { "class": "fab fa-leanpub" }));
                renderMediaContent(data, ItemRow, "");
                ItemRow.append(itemBody);
                ItemRow.find(".media-holder").addClass("col-12");
                var answers_box = $("<div>", { "class": "answer-wrapper no-child col-4" });
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
                    renderQuestionView(item, data.Type);
                }
                break;
            case "ESSAY":
                var itemBody = $("<div>", { "class": "content-wrapper" });
                if (data.Description != null) {
                    itemBody.append($("<div>", { "class": "doc-content" }).html(data.Description));
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
                    renderQuestionView(item, data.Type);
                }
                break;
        }

        if (listPartContainer.find(".nav-item").length == 1) {
            itemtitle.addClass("active");
            tabsitem.addClass("show active");
        }
        $('.btn').tooltip({
            trigger: "hover"
        });

    }

    var renderMediaContent = function (data, wrapper, type = "") {

        if (data.Media != null) {
            var mediaHolder = $("<div>", { "class": "media-holder " + type });
            switch (type) {
                case "IMG":
                    mediaHolder.append($("<img>", { "class": "img-fluid lazy", "src": data.Media.Path }));
                    break;
                case "VIDEO":
                    mediaHolder.append("<video controls><source src='" + data.Media.Path + "' type='" + data.Media.Extension + "' />Your browser does not support the video tag</video>");
                    break;
                case "AUDIO":
                    mediaHolder.append("<audio id='audio' controls><source src='" + data.Media.Path + "' type='" + data.Media.Extension + "' />Your browser does not support the audio tag</audio>");
                    break;
                case "DOC":
                    mediaHolder.append($("<embed>", { "src": data.Media.Path + "#view=FitH", "width": "100%", "height": "800px" }));
                    break;
                default:
                    if (data.Media.Extension != null)
                        if (data.Media.Extension.indexOf("image") >= 0)
                            mediaHolder.append($("<img>", { "class": "img-fluid lazy", "src": data.Media.Path }));
                        else if (data.Media.Extension.indexOf("video") >= 0)
                            mediaHolder.append("<video controls><source src='" + data.Media.Path + "' type='" + data.Media.Extension + "' />Your browser does not support the video tag</video>");
                        else if (data.Media.Extension.indexOf("audio") >= 0)
                            mediaHolder.append("<audio id='audio' controls><source src='" + data.Media.Path + "' type='" + data.Media.Extension + "' />Your browser does not support the audio tag</audio>");
                        else
                            mediaHolder.append($("<embed>", { "src": data.Media.Path + "#view=FitH" }));
                    break;
            }
            wrapper.append(mediaHolder);
        }
    }

    var renderQuestionView = function (data, template) {
        //render question
        switch (template) {
            case "QUIZ2":
                var container = $("#" + data.ParentID + " .quiz-wrapper");
                var quizitem = $("<div>", { "class": "quiz-item", "id": data.ID });
                var boxHeader = $("<div>", { "class": "quiz-box-header" });
                boxHeader.append($("<div>", { "class": "quiz-text", "text": data.Content }));
                renderMediaContent(data, boxHeader);
                quizitem.append(boxHeader);

                container.append(quizitem);
                var answer_wrapper = $("<div>", { "class": "answer-wrapper" });

                quizitem.append(answer_wrapper);

                if (data.Description !== "") {
                    var extend = $("<div>", { "class": "quiz-extend", "text": data.Description });
                    quizitem.append(extend);
                }

                for (var i = 0; data.Answers != null && i < data.Answers.length; i++) {
                    var item = data.Answers[i];
                    renderAnswerView(item, template);
                }
                break;
            case "QUIZ3":
                var container = $("#" + data.ParentID + " .quiz-wrapper");

                var quizitem = $("<div>", { "class": "quiz-item", "id": data.ID });

                var quiz_part = $("<div>", { "class": "quiz-pane col-6" });
                var answer_part = $("<div>", { "class": "answer-pane no-child col-6" });
                quizitem.append(quiz_part);
                quizitem.append(answer_part);

                var pane_item = $("<div>", { "class": "pane-item" });
                if (data.Media == null) {
                    pane_item.append($("<div>", { "class": "quiz-text", "text": data.Content }));
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
                    renderAnswerView(item, template);
                }
                break;
            default:
                var point = "";

                if (data.Point > 0) {
                    point = " (" + data.Point + "p)";
                }
                var container = $("#" + data.ParentID + " .quiz-wrapper");

                var itembox = $("<div>", { "class": "quiz-item", "id": data.ID });
                var boxHeader = $("<div>", { "class": "quiz-box-header" });
                if (data.Content != null)
                    boxHeader.append($("<h5>", { "class": "title", "text": data.Content + point }));
                else
                    boxHeader.append($("<h5>", { "class": "title", "text": point }));

                renderMediaContent(data, boxHeader);

                itembox.append(boxHeader);
                var answer_wrapper = $("<div>", { "class": "answer-wrapper" });

                itembox.append(answer_wrapper);

                if (data.Description !== "") {
                    var extend = $("<div>", { "class": "quiz-extend", "text": data.Description });
                    itembox.append(extend);
                }

                container.append(itembox);

                //Render Answer
                for (var i = 0; data.Answers != null && i < data.Answers.length; i++) {
                    var item = data.Answers[i];
                    renderAnswerView(item, template);
                }
                break;
        }
    }

    var renderAnswerView = function (data, template) {
        var container = $("#" + data.ParentID + " .answer-wrapper");
        var answer = $("<fieldset>", { "class": "answer-item" });
        switch (template) {
            case "QUIZ2":

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
                    answer.append($("<label>", { "class": "answer-text", "text": data.Content }));

                answer.draggable({
                    cursor: "move",
                    helper: 'clone',
                    revert: "true",
                    scroll: true,
                    start: function (event, ui) {
                        ui.helper.data('parent', $(this).parent());
                    },
                    stop: function (event, ui) {
                        //var prevParent = ui.helper.data('parent');
                        //$(prevParent).find(".placeholder").show();
                    }
                });

                container.append(answer);
                break;
            default:
                var form = $("<div>", { "class": "form-check" });
                answer.append(form);
                form.append($("<input>", { "type": "hidden" }));
                form.append($("<input>", { "id": data.ID, "type": "radio", "class": "input-checkbox answer-checkbox form-check-input", "onclick": "answerQuestion(this,'" + data.ParentID + "')", "name": "rd_" + data.ParentID }));
                if (data.Content != null)
                    form.append($("<label>", { "class": "answer-text form-check-label", "for": data.ID, "text": data.Content }));
                renderMediaContent(data, answer);
                container.append(answer);
                break;
        }

    }

    //Exam
    var renderExam = function () {
        var data = _data;
        if (isNull(data)) data = getCurrentExamState(); // get current exam state
        if (isNull(data)) {
            res = loadLesssonData({
                "LessonID": config.lesson_id,
                "ClassID": config.class_id
            });

            if (isNull(res)) {
                var resData = JSON.parse(res);
                if (resData.Data.TemplateType != TEMPLATE_TYPE.LESSON) {
                    var exam = resData.Exam;
                    if (exam == null || exam.Status == true) {
                        removeLocalData("CurrentExam");
                        if (exam != null) resData.Exam.ID = null;
                        renderLesson(resData);
                    }
                    else {
                        setLocalData("CurrentExam", resData.Exam.ID);
                        renderLesson(resData);
                        renderBoDem();
                        $("#counter").html(resData.Timer);
                        countdown();
                        $('.quizNumber').removeClass("d-none");
                    }
                }
                startDragDrop();
            }
        } else {
            renderLesson(data);
            if (data.Data.TemplateType != TEMPLATE_TYPE.LESSON) {
                renderBoDem();
                $("#counter").html(getLocalData("Timer"));
                var currentExam = getLocalData("CurrentExam");
                if (currentExam != null) {
                    countdown();
                    $('.quizNumber').removeClass("d-none");
                }
            }
            startDragDrop();
        }
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
                    setLocalData("Timer", "00:00");
                    CompleteExam(true);
                }
                else {
                    var _sec = 59 - moment(timer).second();
                    var _minutes = data.Timer - moment(timer).minutes() - (_sec > 0 ? 1 : 0);
                    var timer = (_minutes >= 10 ? _minutes : "0" + _minutes) + ":" + (_sec >= 10 ? _sec : "0" + _sec)
                    setLocalData("Timer", timer);
                    setLocalData("CurrentExam", data.ID);
                    $("#counter").html(getLocalData("Timer"));
                    countdown();
                }

            }
        });
        var data = localStorage.getItem(config.lesson_id + "_" + config.class_id);
        var enData = b64DecodeUnicode(data);
        if (enData == null || enData == {} || enData == "" || enData == "{}" || enData == void 0) {
            return null;
        } else {
            _data = JSON.parse(enData);
            return _data;
        }
    }

    var renderLesson = function (data) {
        writeLog("renderLesson", data);
        var container = document.getElementById(config.container);
        if (container == null) throw "Element id " + config.container + " not exist - create now";
        if (data.Data.TemplateType == TEMPLATE_TYPE.LESSON) {
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
            if (getLocalData("Timer") == null) setLocalData("Timer", counter);
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
                document.querySelector("input[name='ExamID']").value = getLocalData("CurrentExam");
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
        return !isNull(getLocalData("CurrentExam"));
    }
    //render bài tập
    var renderSinglePart = function (data, index, type) {
        writeLog("renderSinglePart", data);
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
            tabList += renderSinglePart(item, i, data.TemplateType);
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

        nav_bottom += '<div class="col-md-4 text-center d-inline-block"><button class="btn btn-success pl-5 pr-5" onclick="CompleteExam()">Nộp bài</button></div>';
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

    var CompleteExam = function (isOvertime) {
        if (isOvertime || confirm("Có phải bạn muốn nộp bài")) {
            var exam = document.querySelector("input[name='ExamID']");
            var dataform = new FormData();
            dataform.append("ExamID", exam.value);
            Ajax(config.url.end, dataform, "POST", true)
                .then(function (res) {
                    stopCountdown();
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
        html += '<div style="display:none" id="btn-completed" class="d-flex justify-content-center pt-5 pb-5"><button class="btn btn-primary" onclick="CompleteExam()" data-original-title="" title=""> Nộp bài </button></div>';
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
    window.CompleteExam = CompleteExam;
    window.Redo = resetLesson;
    window.GoBack = goBack;

    return ExamStudent;
}());

