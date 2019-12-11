
var Debug = true;

var mod = {
    PREVIEW: "preview",
    TEACHERVIEW: "teacherview",
    TEACHEREDIT: "teacheredit",
    EDIT: "edit",
    EXAM: "exam",
    REVIEW: "review"
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

var writeLog = function (name, msg) {
    if (Debug) {
        console.log(name, msg);
    }
}

var Lesson = (function () {

    var _totalPart = 0;

    var _data = null;

    var _UImode = null;

    var exam_timeout = null;

    var config = {
        container: "",
        mod: "",//preview, edit, exam, review
        url: {
            load: "", //all: load lesson data
            save: "", //edit: save lesson data
            list_part: "",//all: get all part
            load_part: "",//all: load lesson part detail
            save_part: "",//edit: save part,
            move_part: "",//change: change part position,
            del_part: "",
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
            case mod.TEACHEREDIT:
                renderPreview();
                break;
            case mod.EXAM:
                renderExam();
                break;
            case mod.REVIEW:
                renderReview();
                break;
            default:
                renderPreview();
                break;
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
                        loadLessonParts({ LessonID: config.lesson_id, ClassID: config.class_id }, callback);
                        break;
                    default:
                        callback;
                        break;
                }
            }
        });
    }

    var loadLessonParts = function (param, callback) {
        var formData = new FormData();
        if (param != null)
            Object.keys(param).forEach(e => formData.append(e, param[e]));
        Ajax(config.url.list_part, formData, "POST", true).then(function (res) {
            _data.Part = JSON.parse(res).Data;
            callback();
        });
    }

    //render content

    var renderStandardLayout = function () {
        var container = $('#' + config.container);
        container.empty();

        var lessonBox = $("<div>", { "class": "lesson lesson-box h-100 mod_" + config.mod });
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
                var sort = $("<button>", { "class": "btn btn-primary btn-sort", "title": "Sort", "onclick": "SortPart()" });
                var edit = $("<button>", { "class": "btn btn-primary btn-edit", "title": "Edit", "data-toggle": "modal", "data-target": "#lessonModal", "onclick": "EditLesson('" + data.ID + "')" });
                var create = $("<button>", { "class": "btn btn-primary btn-add", "title": "Add", "data-toggle": "modal", "data-target": "#partModal", "onclick": "AddPart('" + data.ID + "')" });
                //var close = $("<button>", { "class": "btn btn-primary btn-close", "text": "X", "onclick": "render.resetLesson()" });
                var remove = $("<button>", { "class": "btn btn-danger btn-remove", "title": "Remove" });

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
            case mod.TEACHERVIEW:
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
                var toggleMode = $("<button>", { "class": "btn btn-primary btn-add", "title": "Switch mode", "onclick": "SwitchMode('" + mod.TEACHEREDIT + "')", "text": "Switch to edit mode" });
                var iconToggle = $("<i>", { "class": "fas fa-edit ml-2" });

                lessonButton.append(toggleMode);
                toggleMode.append(iconToggle);
                headerRow.append(lessonButton);
                break;
            case mod.TEACHEREDIT:
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
                var sort = $("<button>", { "class": "btn btn-primary btn-sort", "title": "Sort", "onclick": "SortPart()" });
                var create = $("<button>", { "class": "btn btn-primary btn-add", "title": "Add", "data-toggle": "modal", "data-target": "#partModal", "onclick": "AddPart('" + data.ID + "')" });
                var toggleMode = $("<button>", { "class": "btn btn-primary btn-add", "title": "Switch mode", "onclick": "SwitchMode('" + mod.TEACHERVIEW + "')", "text": "Switch to view mode" });

                var iconSort = $("<i>", { "class": "fas fa-sort" });
                var iconCreate = $("<i>", { "class": "fas fa-plus-square" });
                var iconToggle = $("<i>", { "class": "fas fa-eye ml-2" });

                lessonButton.append(toggleMode);
                toggleMode.append(iconToggle);
                lessonButton.append(sort);
                sort.append(iconSort);
                lessonButton.append(create);
                create.append(iconCreate);
                headerRow.append(lessonButton);
                break;
            case mod.EXAM:
                //no header
                break;
            case mod.REVIEW:
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
            case mod.EXAM:
                break;
            case mod.REVIEW:
                break;
        }

        //footer: navigation
        switch (config.mod) {
            case mod.PREVIEW:
            case mod.TEACHERVIEW:
            case mod.TEACHEREDIT:
                if (_UImode == UIMode.EXAM_ONLY) {
                    var nav_bottom = $('<div>', { "class": "row" });

                    var prevtab = $("<button>", { "class": "prevtab btn btn-success mr-2", "title": "Prev", "onclick": "PrevPart()" });
                    var iconprev = $("<i>", { "class": "fas fa-arrow-left" });
                    var nexttab = $("<button>", { "class": "nexttab btn btn-success", "title": "Next", "onclick": "NextPart()" });
                    var iconnext = $("<i>", { "class": "fas fa-arrow-right" });
                    prevtab.append(iconprev);
                    nexttab.append(iconnext);

                    _totalPart = data.Part.length;
                    prevtab.prop("disabled", true);

                    if (_totalPart <= 1)
                        nexttab.prop("disabled", true);


                    nav_bottom.append($('<div>', { "class": "col-md-2 text-left" }).append(prevtab));
                    nav_bottom.append($('<div>', { "class": "col-md-8 text-center" }));
                    nav_bottom.append($('<div>', { "class": "col-md-2 text-right" }).append(nexttab));

                    lessonFooter.show().append(nav_bottom);
                }
                else {
                    lessonFooter.hide();
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
            case UIMode.LECTURE_ONLY:
                leftCol.parent().removeClass("col-md-6").removeClass("col-md-12").addClass("col-md-12").show();//scrollwrapper
                rightCol.parent().hide();
                break;
            case UIMode.EXAM_ONLY:
                leftCol.parent().removeClass("col-md-6").removeClass("col-md-12").addClass("col-md-4").show();
                rightCol.parent().removeClass("col-md-6").removeClass("col-md-12").addClass("col-md-8").show();
                break;
            case UIMode.BOTH:
                leftCol.parent().removeClass("col-md-4").removeClass("col-md-6").removeClass("col-md-12").addClass("col-md-6").show();
                rightCol.parent().removeClass("col-md-6").removeClass("col-md-8").removeClass("col-md-12").addClass("col-md-6").show();
                break;
        }
    }

    var switchMode = function (mode) {
        config.mod = mode;
        reloadData();
    }

    //Preview: view + edit
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
                boxHeader.append($("<h5>", { "class": "title col-md-10", "text": (data.Title == null ? "" : data.Title) + time + point }));

                var iconEdit = $("<i>", { "class": "fas fa-edit" });
                var iconTrash = $("<i>", { "class": "fas fa-trash" });

                var boxButton = $("<div>", { "class": "text-right col-md-2" });
                boxButton.append($("<button>", { "class": "btn btn-primary btn-sm mr-1 ml-1", "title": "Edit", "data-toggle": "modal", "data-target": "#partModal", "onclick": "EditPart('" + data.ID + "')" }).append(iconEdit))
                boxButton.append($("<button>", { "class": "btn btn-danger btn-sm mr-1 ml-1", "title": "Remove", "onclick": "RemovePart('" + data.ID + "')" }).append(iconTrash));
                boxHeader.append(boxButton);
                break;
            default:
                boxHeader.append($("<h5>", { "class": "title col-md-12", "text": (data.Title == null ? "" : data.Title) + time + point }));
                break;
        }

        itembox.append(boxHeader);

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
                if (data.Description != null) {
                    itemBody.append($("<div>", { "class": "part-description" }).html(data.Description));
                }
                renderMediaContent(data, itemBody, "IMG");
                itemtitle.prepend($("<i>", { "class": "fas fa-file-image" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                break;
            case "AUDIO":
                var itemBody = $("<div>", { "class": "media-wrapper" });
                if (data.Description != null)
                    itemBody.append($("<div>", { "class": "part-description" }).html(data.Description));
                renderMediaContent(data, itemBody, "AUDIO");
                itemtitle.prepend($("<i>", { "class": "fas fa-music" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                break;
            case "VIDEO":
                var itemBody = $("<div>", { "class": "media-wrapper" });
                if (data.Description != null)
                    itemBody.append($("<div>", { "class": "part-description" }).html(data.Description));
                renderMediaContent(data, itemBody, "VIDEO");
                itemtitle.prepend($("<i>", { "class": "far fa-play-circle" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                break;
            case "DOC":
                var itemBody = $("<div>", { "class": "media-wrapper" });
                if (data.Description != null)
                    wrapper.append($("<div>", { "class": "part-description" }).html(data.Description));
                renderMediaContent(data, itemBody, "DOC");
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
            case "QUIZ3":
                itembox.append(ItemRow);
                var itemBody = $("<div>", { "class": "quiz-wrapper col-8" });
                itemtitle.prepend($("<i>", { "class": "fab fa-leanpub" }));
                renderMediaContent(data, ItemRow, "");
                console.log(ItemRow);
                ItemRow.append(itemBody);
                if (data.Description != null) {
                    ItemRow.append($("<div>", { "class": "part-description" }).html(data.Description));
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
                console.log(itemBody);
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
                    itemBody.append($("<div>", { "class": "part-description" }).html(data.Description));
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

    var renderPreviewQuestion = function (data, template) {
        //render question
        var point = "";

        if (data.Point > 0) {
            point = " (" + data.Point + "p)";
        }
        switch (template) {
            case "QUIZ2":
                var container = $("#" + data.ParentID + " .quiz-wrapper");
                var quizitem = $("<div>", { "class": "quiz-item", "id": data.ID });
                var boxHeader = $("<div>", { "class": "quiz-box-header" });
                boxHeader.append($("<div>", { "class": "quiz-text", "html": breakLine(data.Content) + point }));
                renderMediaContent(data, boxHeader);
                quizitem.append(boxHeader);

                container.append(quizitem);
                var answer_wrapper = $("<div>", { "class": "answer-wrapper" });

                quizitem.append(answer_wrapper);

                if (data.Description !== "") {
                    var extend = $("<div>", { "class": "quiz-extend d-block", "html": breakLine(data.Description) });
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
                if (data.Description !== "") {
                    var extend = $("<div>", { "class": "quiz-extend show", "html": breakLine(data.Description) });
                    quizitem.append(extend);
                }

                var pane_item = $("<div>", { "class": "pane-item" });
                if (data.Media == null) {
                    pane_item.append($("<div>", { "class": "quiz-text", "html": breakLine(data.Content) + point }));
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

                var itembox = $("<div>", { "class": "quiz-item", "id": data.ID });
                var boxHeader = $("<div>", { "class": "quiz-box-header" });
                if (data.Content != null)
                    boxHeader.append($("<h5>", { "class": "title", "html": breakLine(data.Content) + point }));
                else
                    boxHeader.append($("<h5>", { "class": "title", "text": point }));

                renderMediaContent(data, boxHeader);

                itembox.append(boxHeader);
                var answer_wrapper = $("<div>", { "class": "answer-wrapper" });

                itembox.append(answer_wrapper);

                if (data.Description !== "") {
                    var extend = $("<div>", { "class": "quiz-extend show", "html": breakLine(data.Description) });
                    itembox.append(extend);
                }

                container.append(itembox);

                //Render Answer
                for (var i = 0; data.Answers != null && i < data.Answers.length; i++) {
                    var item = data.Answers[i];
                    renderPreviewAnswer(item, template);
                }
                break;
        }
    }

    var renderPreviewAnswer = function (data, template) {
        var container = $("#" + data.ParentID + " .answer-wrapper");
        var answer = $("<fieldset>", { "class": "answer-item", id: data.ID });
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
                    answer.append($("<label>", { "class": "answer-text", "html": breakLine(data.Content) }));

                //answer.draggable({
                //    cursor: "move",
                //    helper: 'clone',
                //    revert: "true",
                //    scroll: true,
                //    start: function (event, ui) {
                //        ui.helper.data('parent', $(this).parent());
                //        alert(1);        
                //    },
                //    stop: function (event, ui) {
                //        //var prevParent = ui.helper.data('parent');
                //        //$(prevParent).find(".placeholder").show();
                //    }
                //});

                container.append(answer);
                break;
            default:
                var form = $("<div>", { "class": "form-check" });
                answer.append(form);
                form.append($("<input>", { "type": "hidden" }));
                form.append($("<input>", { "id": data.ID, "type": "radio", "class": "input-checkbox answer-checkbox form-check-input", "onclick": "answerQuestion(this,'" + data.ParentID + "')", "name": "rd_" + data.ParentID }));
                if (data.Content != null)
                    form.append($("<label>", { "class": "answer-text form-check-label", "for": data.ID, "html": breakLine(data.Content) }));
                renderMediaContent(data, answer);
                container.append(answer);
                break;
        }

    }

    var renderMediaContent = function (data, wrapper, type = "") {

        if (data.Media != null) {
            var mediaHolder = $("<div>", { "class": "media-holder mt-2 mb-2 " + type });
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
                console.log(quiz);
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
                        prevHolder.append($("<div>", { "class": "pane-item placeholder", "text": "Drop your answer here" }));
                }


                if (config.mod == mod.EXAM) {
                    var quiz = prevHolder.data("questionId");
                    if (quiz != null) { delAnswerForStudentNoRender(quiz); }
                    AnswerQuestion(this);
                }

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

                if (config.mod == mod.EXAM) {
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

    var modalAddPart = function (lessonID) {
        var modalForm = window.partForm;
        $(modalForm).empty();
        $('#partModal #modalTitle').html("Add Content");
        $(modalForm).append($("<input>", { "type": "hidden", "name": "ParentID", "value": lessonID }));
        $(modalForm).append($("<input>", { "type": "hidden", "name": "ClassID", "value": config.class_id }));
        $('#action').val(config.url.save_part);
        var selectTemplate = $("<select>", { "class": "templatetype form-control", "name": "Type", "required": "required" }).bind("change", chooseTemplate);
        $(modalForm).append(selectTemplate);
        $(selectTemplate).append("<option value=''>--- Choose content type ---</option>")
            .append("<option value='TEXT'>Text</option>")
            .append("<option value='VIDEO'>Video</option>")
            .append("<option value='AUDIO'>Audio</option>")
            .append("<option value='IMG'>Image</option>")
            .append("<option value='DOC'>Document (PDF, DOC)</option>")
            .append("<option value='VOCAB'>Vocabulary</option>")
            .append("<option value='QUIZ1'>QUIZ: Choose correct answer</option>")
            .append("<option value='QUIZ2'>QUIZ: Fill the word</option>")
            .append("<option value='QUIZ3'>QUIZ: Match answer</option>")
            .append("<option value='ESSAY'>QUIZ: Essay</option>");
        $(modalForm).append($("<div>", { "class": "lesson_parts" }));
        $(modalForm).append($("<div>", { "class": "question_template hide" }));
        $(modalForm).append($("<div>", { "class": "answer_template hide" }));
    }

    var renderEditPart = function (data) {
        var modalForm = window.partForm;
        $('#partModal #modalTitle').html("Update");
        $(modalForm).append($("<input>", { "type": "hidden", "name": "ParentID", "value": data.ParentID }));
        $(modalForm).append($("<input>", { "type": "hidden", "name": "ClassID", "value": config.class_id }));
        $(modalForm).append($("<input>", { "type": "hidden", "name": "ID", "value": data.ID }));
        $(modalForm).append($("<input>", { "type": "hidden", "name": "Type", "value": data.Type }));
        $('#action').val(config.url.save_part);

        $(modalForm).append($("<div>", { "class": "lesson_parts" }));
        $(modalForm).append($("<div>", { "class": "question_template hide" }));
        $(modalForm).append($("<div>", { "class": "answer_template hide" }));
        renderPartTemplate(data.Type, data);
    }

    var removePart = function (id) {
        var check = confirm("Remove this content?");
        if (check) {

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

        contentholder.append($("<label>", { "class": "title", "text": "Title:" }));
        contentholder.append($("<input>", { "type": "text", "name": "Title", "class": "input-text form-control", "placeholder": "Title", "required": "required" }));
        if (data != null && data.Title != null)
            contentholder.find("[name=Title]").val(data.Title);

        contentholder.append(
            $("<div>", { class: "mb-2" }).append(
                $("<textarea>", { "id": "editor", "rows": "5", "name": "Description", "class": "input-text form-control", "placeholder": "Enter content" }))
        );
        if (data != null && data.Description != null)
            contentholder.find("[name=Description]").val(data.Description);
        switch (type) {
            case "TEXT"://Text
                contentholder.append($("<label>", { "class": "title", "text": "Enter your content" }));
                contentholder.append($('#editor'));
                break;
            case "VIDEO":
                contentholder.append($("<label>", { "class": "title", "text": "Choose video" }));
                contentholder.append($("<div>", { "class": "media_holder" }));
                renderAddMedia(contentholder.find(".media_holder"), "", "VIDEO", data != null ? data.Media : null);
                contentholder.append($("<div>", { "class": "media_preview" }));
                break;
            case "AUDIO":
                contentholder.append($("<label>", { "class": "title", "text": "Choose audio" }));
                contentholder.append($("<div>", { "class": "media_holder" }));
                renderAddMedia(contentholder.find(".media_holder"), "", "AUDIO", data != null ? data.Media : null);
                contentholder.append($("<div>", { "class": "media_preview" }));
                break;
            case "IMG":
                contentholder.append($("<label>", { "class": "title", "text": "Choose image" }));
                contentholder.append($("<div>", { "class": "media_holder" }));
                renderAddMedia(contentholder.find(".media_holder"), "", "IMG", data != null ? data.Media : null);
                contentholder.append($("<div>", { "class": "media_preview" }));
                break;
            case "DOC":
                contentholder.append($("<label>", { "class": "title", "text": "Choose document (pdf, doc)" }));
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
                    $("<div>", { class: "mt-1" }).append($("<label>", { "class": "input_label mr-1", "text": "Point" }))
                        .append($("<input>", { "type": "text", "name": "Questions.Point", "class": "input-text part_point form-control", "placeholder": "Point", "value": "1" })));
                questionTemplate.append($("<div>", { class: "mt-1" }).append($("<label>", { "class": "part_label", "text": "Answer (tick if correct)" })));

                var answer_wrapper = $("<div>", { "class": "answer-wrapper" });
                answer_wrapper.append($("<input>", { "type": "button", "class": "btn btn-primary btnAddAnswer ml-3", "value": "+", "onclick": "AddNewAnswer(this)" }));
                questionTemplate.append(answer_wrapper);
                questionTemplate.append($("<textarea>", { "rows": "2", "name": "Questions.Description", "class": "input-text part_description form-control", "placeholder": "Explaination" }));
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
                renderAddMedia(contentholder.find(".media_holder"), "", "", data != null ? data.Media : null);
                contentholder.append($("<div>", { "class": "media_preview" }));
                contentholder.append($("<div>", { "class": "part_content " + type }));
                contentholder.append($("<input>", { "type": "button", "class": "btn btnAddQuestion btn-primary", "value": "Add question", "onclick": "AddNewQuestion(this)" }));
                //Add First Question
                if (data != null && data.Questions != null) {
                    for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
                        var quiz = data.Questions[i];
                        addNewQuestion(quiz);
                    }
                }
                else
                    addNewQuestion();
                break;
            case "QUIZ2"://Trắc nghiệm dạng điền từ
                var questionTemplate = $("<fieldset>", { "class": "fieldQuestion", "Order": 0 });
                var quizWrapper = $("<div>", { "class": "quiz-wrapper" });
                quizWrapper.append($("<input>", { "type": "hidden", "name": "Questions.ID" }));
                quizWrapper.append($("<input>", { "type": "hidden", "name": "Questions.Order", "value": 0 }));
                quizWrapper.append($("<label>", { "class": "fieldset_title", "text": "" }));
                quizWrapper.append($("<input>", { "type": "button", "class": "quiz-remove", "value": "X", "tabindex": -1, "onclick": "RemoveQuestion(this)" }));
                quizWrapper.append($("<input>", { "name": "Questions.Content", "class": "input-text quiz-text form-control", "placeholder": "Position (skip for media content)" }));
                quizWrapper.append($("<label>", { "class": "input_label mr-1", "text": "Point" }));
                quizWrapper.append($("<input>", { "type": "text", "name": "Questions.Point", "class": "input-text part_point form-control", "placeholder": "Point", "value": "1" }));
                quizWrapper.append($("<div>", { "class": "media_holder" }));
                renderAddMedia(quizWrapper.find(".media_holder"), "Questions.");
                quizWrapper.append($("<div>", { "class": "media_preview" }));
                questionTemplate.append(quizWrapper);

                var answer_wrapper = $("<div>", { "class": "answer-wrapper" });
                answer_wrapper.append($("<input>", { "type": "button", "class": "btn btn-primary btnAddAnswer ml-3", "value": "+", "onclick": "AddNewAnswer(this)" }));
                questionTemplate.append(answer_wrapper);
                questionTemplate.append($("<textarea>", { "rows": "2", "name": "Questions.Description", "class": "input-text part_description form-control", "placeholder": "Explanation" }));
                question_template_holder.append(questionTemplate);

                var answerTemplate = $("<fieldset>", { "class": "answer-box m-1" });
                answerTemplate.append($("<input>", { "type": "button", "class": "answer-remove", "value": "X", "tabindex": -1, "onclick": "RemoveAnswer(this)" }));
                answerTemplate.append($("<input>", { "type": "hidden", "name": "Questions.Answers.ID" }));
                answerTemplate.append($("<input>", { "type": "hidden", "name": "Questions.Answers.ParentID", "value": 0 }));
                answerTemplate.append($("<input>", { "type": "hidden", "name": "Questions.Answers.IsCorrect", "value": true }));
                answerTemplate.append($("<input>", { "type": "text", "name": "Questions.Answers.Content", "class": "input-text answer-text form-control", "placeholder": "Answer" }));
                answer_template_holder.append(answerTemplate);

                contentholder.append($("<div>", { "class": "media_holder" }));
                renderAddMedia(contentholder.find(".media_holder"), "", "", data != null ? data.Media : null);
                contentholder.append($("<div>", { "class": "media_preview" }));
                contentholder.append($("<div>", { "class": "part_content " + type }));
                contentholder.append($("<input>", { "type": "button", "class": "btn btnAddQuestion bnt-primary", "value": "Add question", "tabindex": -1, "onclick": "AddNewQuestion(this)" }));

                //Add First Question
                if (data != null && data.Questions != null) {
                    for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
                        var quiz = data.Questions[i];
                        addNewQuestion(quiz);
                    }
                }
                else
                    addNewQuestion();
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
                quizWrapper.append($("<label>", { "class": "input_label", "text": "Point" }));
                quizWrapper.append($("<input>", { "type": "text", "name": "Questions.Point", "class": "input-text part_point form-control", "placeholder": "Point", "value": "1", "tabindex": 0 }));
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
                answer_template_holder.append(answerTemplate);

                contentholder.append($("<div>", { "class": "media_holder" }));
                renderAddMedia(contentholder.find(".media_holder"), "", "", data != null ? data.Media : null);
                contentholder.append($("<div>", { "class": "media_preview" }));
                contentholder.append($("<div>", { "class": "part_content " + type }));
                contentholder.append($("<input>", { "type": "button", "class": "btn btnAddQuestion btn-primary", "value": "Add question", "onclick": "AddNewQuestion(this)", "tabindex": -1 }));

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
                contentholder.append($("<label>", { "class": "title", "text": "Enter title" }));
                contentholder.append($("<textarea>", { "id": "editor", "rows": "15", "name": "Description", "class": "input-text form-control", "placeholder": "Description" }));
                if (data != null && data.description != null)
                    contentholder.find("[name=Description]").val(data.description);
                //CKEDITOR.replace("editor");

                //ClassicEditor
                //    .create(document.querySelector('#editor'))
                //    .then(newEditor => {
                //        myEditor = newEditor;
                //    })
                //    .catch(error => {
                //        console.error(error);
                //    });

                var questionTemplate = $("<fieldset>", { "class": "fieldQuestion", "Order": 0 });
                questionTemplate.append($("<input>", { "type": "hidden", "name": "Questions.ID" }));
                questionTemplate.append($("<input>", { "type": "hidden", "name": "Questions.Order", "value": 0 }));
                questionTemplate.append($("<label>", { "class": "fieldset_title", "text": "" }));

                var quizWrapper = $("<div>", { "class": "quiz-wrapper" });
                quizWrapper.append($("<input>", { "type": "text", "name": "Questions.Content", "class": "input-text quiz-text form-control", "placeholder": "Question", "tabindex": 0 }));
                quizWrapper.append($("<div>", { "class": "media_holder" }));
                renderAddMedia(quizWrapper.find(".media_holder"), "Questions.");
                quizWrapper.append($("<div>", { "class": "media_preview" }));
                quizWrapper.append($("<label>", { "class": "input_label", "text": "Point" }));
                quizWrapper.append($("<input>", { "type": "text", "name": "Questions.Point", "class": "input-text part_point form-control", "placeholder": "Point", "value": "1", "tabindex": 0 }));
                questionTemplate.append(quizWrapper);

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
                else
                    addNewQuestion();
                break;
            default:
                alert("Not implement");
                break;
        }
        CKEDITOR.replace("editor");
        if (data != null && data.Media != null)
            renderMediaContent(data, contentholder.find(".media_preview:first"), type);
        //debugger;
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
                $(clone).find("[name='Questions.Description']").val(data.Description);
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

    var removeQuestion = function (obj) {
        if (confirm("Remove question?")) {
            var id = $(obj).siblings("[name$='.ID']").val();
            var quizHolder = $(obj).parent();
            if (!$(obj).parent().hasClass("fieldQuestion"))
                quizHolder = $(obj).parent().parent();
            quizHolder.parent().append($("<input>", { "type": "hidden", "name": "RemovedQuestions", "value": id }));
            quizHolder.remove();
        }
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
                wrapper.append($("<input>", { "type": "file", "name": "file", "onchange": "changeMedia(this)", "class": "hide", "accept": ".pdf" }));
                break;
            default:
                wrapper.append($("<input>", { "type": "file", "name": "file", "onchange": "changeMedia(this)", "class": "hide" }));
                break;
        }
        wrapper.append($("<input>", { "type": "button", "class": "btn btn-primary btnAddFile m-1 btn-sm", "style": "max-width:80%", "onclick": "chooseFile(this)", "value": "Choose media", "tabindex": -1 }));
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
        var quizpanes = $('.tab-pane-quiz');
        $('.tab-pane.show.active').removeClass('show active');
        $('.tab-pane-quiz.show.active').removeClass('show active');
        $(panes[idx]).addClass('show active');
        $(quizpanes[idx]).addClass('show active');
        $('.prevtab').prop('disabled', idx == 0);
        $('.nexttab').prop('disabled', idx == _totalPart - 1);
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
        nav += '<div class="col-md-1 text-left d-inline-block"><button class="prevtab btn btn-success" data-toggle="tooltip" title="Phần trước" onclick="PrevPart()"><i class="fas fa-arrow-left"></i></button></div>'; //left button
        nav_bottom += '<div class="col-md-4 text-left d-inline-block"><button class="prevtab btn btn-success" onclick="PrevPart()"><i class="fas fa-arrow-left"></i></button></div>';
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
        nav += '<div class="col-md-1 text-right d-inline-block"><button class="nexttab btn btn-success" data-toggle="tooltip" title="Phần sau" onclick="NextPart()"><i class="fas fa-arrow-right"></i></button></div>'; //right button

        nav_bottom += '<div class="col-md-4 text-center d-inline-block"><button class="btn btn-success pl-5 pr-5" onclick="CompleteExam()">Nộp bài</button></div>';
        nav_bottom += '<div class="col-md-4 text-right d-inline-block"><button class="nexttab btn btn-success" onclick="NextPart()"><i class="fas fa-arrow-right"></i></button></div>';
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

    window.LessonInstance = {} || Lesson;

    LessonInstance.onReady = onReady;
    LessonInstance.Version = version;
    LessonInstance.Notification = notification;
    window.BeginExam = beginExam;
    window.AnswerQuestion = AnswerQuestion;
    window.CompleteExam = CompleteExam;
    window.Redo = resetLesson;
    window.GoBack = goBack;

    window.EditLesson = modalEditLesson;
    window.EditPart = modalEditPart;
    window.AddPart = modalAddPart;
    window.SelectTemplate = modalSelectTemplate;

    window.RemovePart = removePart;
    window.SortPart = sortPart;

    window.RemoveQuestion = removeQuestion;
    window.AddNewQuestion = addNewQuestion;

    window.RemoveAnswer = removeAnswer;
    window.AddNewAnswer = addNewAnswer;
    window.ToggleCorrectAnswer = toggleCorrectAnswer;

    window.ReloadData = reloadData;

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

function answerQuestion(obj, quizid) {
    $('.quiz-item#' + quizid + " .quiz-extend").show();
    markQuestion(quizid);
}

function markQuestion(quizid) {
    if ($("#quizNavigator .quiz-wrapper [name=quizNav" + quizid + "].completed").length === 1) {
    } else {
        $("#quizNavigator .quiz-wrapper [name=quizNav" + quizid + "]").addClass("completed");
        var completed = parseInt($(".quizNumber .completed").text()) + 1;
        $(".quizNumber .completed").text(completed);
        if (completed == totalQuiz)
            $(".quizNumber .completed").addClass("finish");
    }
}

function toggleCompact(obj) {
    var parent = $(obj).parent().parent();
    $(parent).toggleClass("compactView");
    if ($(parent).hasClass("compactView"))
        $(obj).text("Expand");
    else
        $(obj).text("Collapse");
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
    requires.each(function () {
        if ($(this).val() == "" || $(this).val() == null) {
            alert("Please fill your content");
            $(this).focus();
            $('.btnSaveForm').show();
            err = true;
            return false;
        }
    });
    if (err) return false;

    var xhr = new XMLHttpRequest();

    var actionUrl = $("#action").val()
    if (form.length > 0)
        actionUrl = form.attr('action');

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
        $('.btnSaveForm').show();
    }
}

