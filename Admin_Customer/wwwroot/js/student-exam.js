var urlBase = "/student/";
var myEditor;
var totalQuiz = 0;
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
                //console.log('DONE -	The operation is complete')
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
var urlChapter = {
    "Location": "/ModChapters/Detail/"
}

var modal = $("#lessonModal");
var modalTitle = $("#modalTitle");
var containerLesson = $("#lessonContainer");
var userID = "";
var clientID = "";

var lessonService = {
    renderData: function (data) {
        $(containerLesson).html("");
        //var lessonBox = $("<div>", { "class": "lesson lesson-box p-fixed" });
        var lessonBox = $("<div>", { "class": "lesson lesson-box" });
        var lessonContainer = $("<div>", { "class": "lesson-container" });
        lessonBox.append(lessonContainer);
        var lessonContent = $("<div>", { "class": "card shadow mb-4" });
        lessonContainer.append(lessonContent);
        //header
        var lessonHeader = $("<div>", { "class": "card-header" });
        lessonContent.append(lessonHeader);

        //header row
        var HeaderRow = $("<div>", { "class": "row" });
        lessonHeader.append(HeaderRow);

        //header
        var ButtonStart = $("<div>", { "class": "d-flex justify-content-center pt-5 pb-5" });
        var btnButton = $("<div>", { "class": "btn btn-primary", "onclick": "BeginExam(this,'" + data.ID + "','" + C_classID + "')", "text": " Bắt đầu làm bài thi" });
        lessonContent.append(ButtonStart);
        ButtonStart.append(btnButton);
        //Body
        var cardBody = $("<div>", { "id": "check-student", "class": "card-body d-none" });
        lessonContent.append(cardBody);
        //row
        var lessonRow = $("<div>", { "class": "row" });
        cardBody.append(lessonRow);


        var tabsleft = $("<div>", { "id": "menu-left", "class": "col-md-2" });
        var lessontabs = $("<div>", { "class": "lesson-tabs" });
        var tabs = $("<ul>", { "id": "pills-tab", "class": "nav flex-column nav-pills", "role": "tablist", "aria-orientation": "vertical" });


        var title = $("<div>", { "class": "lesson-header-title col-lg-4", "text": data.Title });
        HeaderRow.append(title);

        if (data.TemplateType == 2) {
            if (data.Timer > 0) {
                title.text(title.text() + " - thời gian: " + data.Timer + "p");

                var counter = $("<div>", { "class": "text-center col-lg-4", "text": "Thời gian làm bài " });
                var counterdate = $("<span>", { "id": "counter", "class": "time-counter", "text": (data.Timer < 10 ? ("0" + data.Timer) : data.Timer) + ":00" });
                counter.append(counterdate);
                HeaderRow.append(counter);
            }
            if (data.Point > 0) {
                title.text(title.text() + " (" + data.Point + "đ)");
            }
        }
        var sort = $("<a>", { "class": "btn btn-sm btn-sort", "text": "Sắp xếp", "onclick": "lessonService.renderSort()" });
        var edit = $("<a>", { "class": "btn btn-sm btn-edit", "text": "Sửa", "onclick": "lessonService.renderEdit('" + data.ID + "')" });
        var close = $("<a>", { "class": "btn btn-sm btn-close", "text": "X", "onclick": "render.resetLesson()" });
        var remove = $("<a>", { "class": "btn btn-sm btn-remove", "text": "Xóa", "onclick": "lessonService.remove('" + data.ID + "')" });



        lessonRow.append(tabsleft);
        tabsleft.append(lessontabs);
        lessontabs.append(tabs);

        var lessonBody = $("<div>", { "class": "lesson-body", "id": data.ID });

        var bodyright = $("<div>", { "class": "col-md-10" });
        var button = $("<div>", { "class": "text-right col-lg-4" });
        HeaderRow.append(button);

        var prevtab = $("<button>", { "class": "prevtab btn btn-success mr-2", "data-toggle": "tooltip", "title": "Quay lại", "onclick": "tab_goback()" });
        var iconprev = $("<i>", { "class": "fas fa-arrow-left" });
        var nexttab = $("<button>", { "class": "nexttab btn btn-success", "data-toggle": "tooltip", "title": "Tiếp tục", "onclick": "tab_gonext()" });
        var iconnext = $("<i>", { "class": "fas fa-arrow-right" });
        button.append(prevtab);
        prevtab.append(iconprev);
        button.append(nexttab);
        nexttab.append(iconnext);

        var tabscontent = $("<div>", { "id": "pills-tabContent", "class": "tab-content" });
        lessonBody.append(tabscontent);

        lessonRow.append(bodyright);


        // add lesson part
        var createLessonPart = $("<div>", { "class": "add-lesson-part" });
        var btn = $("<a>", { "class": "btn btn-sm btn-success", "text": "Thêm nội dung", "href": "javascript:void(0)", "onclick": "Create.lessonPart('" + data.ID + "')" });
        //createLessonPart.append(btn);

        var newLessonPart = $("<div>", { "class": "new-lesson-part", "id": data.ID + new Date().getDate() });

        bodyright.append(lessonBody);

        //lessonContainer.append(createLessonPart);
        //lessonContainer.append(newLessonPart);
        lessonBox.append(lessonContainer);

        //var lessonContainerBackground = $("<div>", { "class": "lesson-container-bg" });
        //containerLesson.append(lessonContainerBackground);
        containerLesson.append(lessonBox);
    }
}

var render = {
    resetLesson: function () {
        $(containerLesson).html("");
    },
    lesson: function (data) {
        lessonService.renderData(data);
    },
    lessonPart: function (data, lsid, clsid) {
        for (var i = 0; data != null && i < data.length; i++) {
            var lessonpart = data[i];
            if (lessonpart.Title == null) continue;
            render.part(lessonpart, lsid, clsid);
        }

    },
    part: function (data) {

        var time = "", point = "";

        if (data.Timer > 0) {
            time = " (" + data.Timer + "p)";
        }
        if (data.Point > 0) {
            point = " (" + data.Point + "đ)";
        }

        var container = $("#" + data.ParentID).find(".tab-content");
        var tabContainer = $("#menu-left #pills-tab");


        //tabs         
        var lessonitem = $("<li>", { "class": "nav-item" });
        var itemtitle = $("<a>", { "id": "pills-" + data.ID, "class": "nav-link", "data-toggle": "pill", "href": "#pills-part-" + data.ID, "role": "tab", "aria-controls": "pills-" + data.ID, "aria-selected": "false", "text": data.Title });
        lessonitem.append(itemtitle);
        tabContainer.append(lessonitem);

        // tabs content
        var tabsitem = $("<div>", { "id": "pills-part-" + data.ID, "class": "tab-pane fade", "role": "tabpanel", "aria-labelledby": "pills-" + data.ID });
        //var itembody = $("<div>", { "class": "card-body" });
        //tabsitem.append(itembody);

        var itembox = $("<div>", { "class": "part-box " + data.Type, "id": data.ID });
        tabsitem.append(itembox);

        //item row
        //var ItemRow = $("<div>", { "class": "row"});

        var boxHeader = $("<div>", { "class": "part-box-header" });
        if (data.Title != null) {
            boxHeader.append($("<h5>", { "class": "title", "text": data.Title + time + point }));
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
                render.mediaContent(data, itemBody, "IMG");
                if (data.Description != null)
                    wrapper.append($("<div>", { "class": "description", "text": data.Description }));
                itemtitle.prepend($("<i>", { "class": "fas fa-file-image" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                break;
            case "AUDIO":
                var itemBody = $("<div>", { "class": "media-wrapper" });
                render.mediaContent(data, itemBody, "AUDIO");
                if (data.Description != null)
                    wrapper.append($("<div>", { "class": "description", "text": data.Description }));
                itemtitle.prepend($("<i>", { "class": "fas fa-music" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                break;
            case "VIDEO":
                var itemBody = $("<div>", { "class": "media-wrapper" });
                render.mediaContent(data, itemBody, "VIDEO");
                if (data.Description != null)
                    wrapper.append($("<div>", { "class": "description", "text": data.Description }));
                itemtitle.prepend($("<i>", { "class": "far fa-play-circle" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                break;
            case "DOC":
                var itemBody = $("<div>", { "class": "media-wrapper" });
                render.mediaContent(data, itemBody, "DOC");
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
                render.mediaContent(data, itemBody, "");
                container.append(tabsitem);
                //Render Content

                //Render Question
                for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
                    var quizitem = data.Questions[i];
                    render.questions(quizitem, data.Type);
                }
                break;
            case "QUIZ3":
                var itemBody = $("<div>", { "class": "quiz-wrapper col-lg-9" });
                itemtitle.prepend($("<i>", { "class": "fab fa-leanpub" }));
                ItemRow.append(itemBody);
                render.mediaContent(data, itemBody, "");
                var answers_box = $("<div>", { "class": "answer-wrapper no-child col-lg-3" });
                ItemRow.append(answers_box);
                $(answers_box).droppable({
                    tolerance: "intersect",
                    accept: ".answer-item",
                    activeClass: "hasAnswer",
                    hoverClass: "answerHover",
                    drop: function (event, ui) {
                        var prevHolder = $(ui.helper).parent();
                        var quiz = prevHolder.data("id");
                        if (quiz == void 0 || quiz == null || quiz == "") return;
                        prevHolder.remove($(ui.helper));
                        prevHolder.append($("<div>", { "class": "pane-item placeholder", "text": "Thả câu trả lời tại đây" }));
                        //$(prevHolder).find(".placeholder").show();
                        $(this).append($(ui.draggable));
                        answerQuestion(this, quiz, typeof (void 0), "");
                    }
                });
                container.append(tabsitem);
                //Render Question
                for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
                    var quizitem = data.Questions[i];
                    render.questions(quizitem, data.Type);
                }
                break;
            case "ESSAY":
                var itemBody = $("<div>", { "class": "content-wrapper" });
                if (data.Description != null) {
                    itemBody.append($("<div>", { "class": "doc-content" }).html(data.Description));
                }
                itemBody.append($("<textarea>", { "class": "content-answer", "onfocusout": "alert(this.value)" }))
                itemtitle.prepend($("<i>", { "class": "fab fa-leanpub" }));
                ItemRow.append(itemBody);
                container.append(tabsitem);
                var itemBody = $("<div>", { "class": "quiz-wrapper" });
                itembox.append(itemBody);
                render.mediaContent(data, itemBody, "");
                for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
                    var quizitem = data.Questions[i];
                    render.questions(quizitem, data.Type);
                }
                break;
        }

        if (tabContainer.find(".nav-item").length == 1) {
            itemtitle.addClass("active");
            tabsitem.addClass("show active");
        }
    },
    questions: function (data, template) {
        //add quiz indicator to question panel
        $("#quizNavigator .quiz-wrapper").append($("<button>", { "class": "btn btn-outline-secondary rounded-quiz", "type": "button", "text": ++totalQuiz, "name": "quizNav" + data.ID }));
        $(".quizNumber .total").text(totalQuiz);

        //render question
        switch (template) {
            case "QUIZ2":
                var container = $("#" + data.ParentID + " .quiz-wrapper");

                var quizitem = $("<div>", { "class": "quiz-item", "id": data.ID });
                var boxHeader = $("<div>", { "class": "quiz-box-header" });
                boxHeader.append($("<div>", { "class": "quiz-text", "text": data.Content }));
                render.mediaContent(data, boxHeader);
                quizitem.append(boxHeader);

                container.append(quizitem);
                var answer_wrapper = $("<div>", { "class": "answer-wrapper" });

                quizitem.append(answer_wrapper);

                //if (data.Description !== "") {
                //    var extend = $("<div>", { "class": "quiz-extend", "text": data.Description, "style": "display:none" });
                //    quizitem.append(extend);
                //}

                for (var i = 0; data.CloneAnswers != null && i < data.CloneAnswers.length; i++) {
                    var answer = data.CloneAnswers[i];
                    render.answers(answer, template);
                }
                break;
            case "QUIZ3":
                var container = $("#" + data.ParentID + " .quiz-wrapper");

                var quizitem = $("<div>", { "class": "quiz-item", "id": data.ID });

                var quiz_part = $("<div>", { "class": "quiz-pane col-8" });
                var answer_part = $("<div>", { "class": "answer-pane no-child col-4" });
                quizitem.append(quiz_part);
                quizitem.append(answer_part);

                var pane_item = $("<div>", { "class": "pane-item" });
                if (data.Media == null) {
                    pane_item.append($("<div>", { "class": "quiz-text", "text": data.Content }));
                } else {
                    render.mediaContent(data, pane_item);
                }

                quiz_part.append(pane_item);
                container.append(quizitem);

                $(answer_part).droppable({
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
                        var prevHolder = ui.helper.data('parent');
                        $(prevHolder).find(".placeholder").show();
                        $(this).html("");
                        $(this).append(item);
                        answerQuestion(this, questionID, anserID, answerValue);
                    }
                });

                for (var i = 0; data.CloneAnswers != null && i < data.CloneAnswers.length; i++) {
                    var answer = data.CloneAnswers[i];
                    render.answers(answer, template);
                }
                break;
            default:
                var point = "";

                if (data.Point > 0) {
                    point = " (" + data.Point + "đ)";
                }
                var container = $("#" + data.ParentID + " .quiz-wrapper");

                var itembox = $("<div>", { "class": "quiz-item", "id": data.ID });
                var boxHeader = $("<div>", { "class": "quiz-box-header" });
                if (data.Content != null)
                    boxHeader.append($("<h5>", { "class": "title", "text": data.Content + point }));
                else
                    boxHeader.append($("<h5>", { "class": "title", "text": point }));

                render.mediaContent(data, boxHeader);

                itembox.append(boxHeader);
                var answer_wrapper = $("<div>", { "class": "answer-wrapper" });

                itembox.append(answer_wrapper);

                //if (data.Description !== "") {
                //    var extend = $("<div>", { "class": "quiz-extend", "text": data.Description, "style": "display:none" });
                //    itembox.append(extend);
                //}

                container.append(itembox);

                //Render Answer
                for (var i = 0; data.CloneAnswers != null && i < data.CloneAnswers.length; i++) {
                    var answer = data.CloneAnswers[i];
                    render.answers(answer, template);
                }
                break;
        }
    },
    answers: function (data, template) {
        var container = $("#" + data.ParentID + " .answer-wrapper");
        var answer = $("<fieldset>", { "class": "answer-item", "id": data.ID });
        switch (template) {
            case "QUIZ2":

                if ($(container).find(".answer-item").length == 0) {
                    answer.append($("<input>", { "type": "text", "class": "input-text answer-text", "placeholder": data.Content, "onfocusout": "answerQuestion(this,'" + data.ParentID + "','" + data.ID + "',$(this).val())" }));
                    container.append(answer);
                }
                else {
                    var oldval = $(container).find(".answer-text").attr("placeholder");
                    $(container).find(".answer-text").attr("placeholder", oldval + " | " + data.Content);
                }
                break;
            case "QUIZ3":
                var placeholder = $("#" + data.ParentID).find(".answer-pane");
                $(placeholder).attr("data-id", data.ParentID)
                $(placeholder).removeClass("no-child");
                placeholder.empty().append($("<div>", { "class": "pane-item placeholder", "text": "Thả câu trả lời tại đây" }));
                container = $("#" + data.ParentID).parent().siblings(".answer-wrapper");

                if (data.Content != null)
                    answer.append($("<input>", { "type": "hidden", "value": data.Content }));

                if (data.Media != null) {
                    render.mediaContent(data, answer);
                }
                else
                    answer.append($("<label>", { "class": "answer-text", "text": data.Content }));

                answer.draggable({
                    cursor: "move",
                    helper: 'clone',
                    revert: "true",
                    scroll: true,
                    start: function (event, ui) {

                    },
                    stop: function (event, ui) {

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
                render.mediaContent(data, answer);
                container.append(answer);
                break;
        }

    },
    mediaAdd: function (wrapper, prefix, type = "", data = null) {
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
        wrapper.append($("<input>", { "type": "button", "class": "btn btnAddFile", "onclick": "chooseFile(this)", "value": "Chọn file", "tabindex": -1 }));
        wrapper.append($("<input>", { "type": "button", "class": "btn btnResetFile hide", "onclick": "resetMedia(this)", "value": "x", "tabindex": -1 }));
        if (data != null) {
            if (data.name != null) $(wrapper).find("[name='" + prefix + "Media.Name']").val(data.name);
            if (data.originalName != null) {
                $(wrapper).find("[name='" + prefix + "Media.OriginalName']").val(data.originalName);
                $(wrapper).find(".btnAddFile").val(data.originalName);
                $(wrapper).find(".btnResetFile").removeClass("hide");
            }
            if (data.extension != null) $(wrapper).find("[name='" + prefix + "Media.Extension']").val(data.extension);
            if (data.path != null) $(wrapper).find("[name='" + prefix + "Media.Path']").val(data.path);
        }
    },
    mediaContent: function (data, wrapper, type = "") {
        if (data.Media != null) {
            var mediaHolder = $("<div>", { "class": "media-holder " + type });
            switch (type) {
                case "IMG":
                    mediaHolder.append($("<img>", { "src": data.Media.Path, "class": "img-fluid lazy" }));
                    break;
                case "VIDEO":
                    mediaHolder.append("<video controls><source src='" + data.Media.Path + "' type='" + data.Media.Extension + "' />Your browser does not support the video tag</video>");
                    break;
                case "AUDIO":
                    mediaHolder.append("<audio controls><source src='" + data.Media.Path + "' type='" + data.Media.Extension + "' />Your browser does not support the audio tag</audio>");
                    break;
                case "DOC":
                    mediaHolder.append($("<embed>", { "src": data.Media.Path, "width": "100%", "height": "800px" }));
                    break;
                default:
                    if (data.Media.Extension != null)
                        if (data.Media.Extension.indexOf("image") >= 0)
                            mediaHolder.append($("<img>", { "src": data.Media.Path, "class": "img-fluid lazy" }));
                        else if (data.Media.Extension.indexOf("video") >= 0)
                            mediaHolder.append("<video controls><source src='" + data.Media.Path + "' type='" + data.Media.Extension + "' />Your browser does not support the video tag</video>");
                        else if (data.Media.Extension.indexOf("audio") >= 0)
                            mediaHolder.append("<audio controls><source src='" + data.Media.Path + "' type='" + data.Media.Extension + "' />Your browser does not support the audio tag</audio>");
                        else
                            mediaHolder.append($("<embed>", { "src": data.Answers.path }));
                    break;
            }
            wrapper.append(mediaHolder);
        }
    }
};

var load = {
    lesson: function (id, classid, url) {
        var checkSupport = false;
        if (typeof (Storage) !== "undefined") {
            // Code for localStorage/sessionStorage.
            checkSupport = true;
        } else {
            // Sorry! No Web Storage support..
            checkSupport = false;
        }
        $.ajax({
            type: "POST",
            url: url,
            data: { LessonID: id, ClassID: classid },
            dataType: "json",
            success: function (data) {
                if (data.Exam != null && data.Exam != void 0) {
                    document.querySelector("input[name='ExamID']").value = data.Exam.ID;
                    if (checkSupport) {
                        LoadCurrentExam();
                        $("#counter").html(data.Timer);
                    } else {
                        render.lesson(data.Data);
                        render.lessonPart(data.Data.Parts, data.Data.ID, classid);
                        console.log("render Part");
                        start();
                        $("#counter").html(data.Timer);
                    }
                    countdown();
                } else {
                    render.lesson(data.Data);
                    render.lessonPart(data.Data.Parts, data.Data.ID, classid)
                }
            }
        });
    },
    listPart: function (lessonID, classID) {

        var url = urlBase + "CloneLessonPart/";
        $.ajax({
            type: "POST",
            url: url + "GetList",
            data: { LessonID: lessonID, ClassID: classID },
            dataType: "json",
            success: function (data) {
                render.lessonPart(data.Data);
            }
        });
    },
    listExtends: function (partID) {
        var xhr = new XMLHttpRequest();
        //var url = urlBase;
        //xhr.open('GET', url + urlMedia.List + "?ID=" + partID + "&UserID=" + userID + "&ClientID=" + clientID);
        //xhr.send({});
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var data = JSON.parse(xhr.responseText);
                if (data.code == 200) {
                    render.listExtends(data.data);
                }
            }
        }
    },
    listAnswer: function (partID) {
        var xhr = new XMLHttpRequest();
        //var url = urlBase;
        //xhr.open('GET', url + urlLessonAnswers.List + "?LessonPartID=" + partID + "&UserID=" + userID + "&ClientID=" + clientID);
        //xhr.send({});
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var data = JSON.parse(xhr.responseText);
                if (data.code == 200) {
                    render.listAnswer(data.data);
                }
            }
        }
    },
    MediaForm: function (type) {
        var file = $('#file');
        if (type == 'video') {
            file.attr("accept", "video/*");
        }
        if (type == 'audio') {
            file.attr("accept", "audio/*");
        }
        if (type == 'img') {
            file.attr("accept", "image/*");
        }
        file.click();
    }
};

function answerQuestion(obj, quizid, answerID, answerValue) {
    //console.log(obj, quizid, answerID, answerValue);
    //$('.quiz-item#' + quizid + " .quiz-extend").show();
    if (quizid == void 0 || quizid == null || quizid == "") return;
    if (obj.type != void 0) {
        if (obj.type == "checkbox" || obj.type == "radio") {
            $(obj).attr("checked", "");
        }
        if (obj.type == "text") {
            obj.value = answerValue;
        }
    }
    markQuestion(quizid, answerID);
    var dataform = new FormData();
    dataform.append("ID", obj.parentElement.parentElement.parentElement.getAttribute("data-id"));
    dataform.append("ExamID", document.querySelector("input[name='ExamID']").value);
    dataform.append("AnswerID", answerID);
    dataform.append("QuestionID", quizid);
    dataform.append("AnswerValue", answerValue);
    Ajax(urlChose, "POST", dataform, false)
        .then(function (res) {
            if (res != "Accept Deny") {
                var data = JSON.parse(res);
                obj.parentElement.parentElement.parentElement.setAttribute("data-id", data.ID);
                SetCurrentExam()
            }
        })
        .catch(function (err) {
            console.log(err);
        })

}

function markQuestion(quizid, answerid) {
    if (answerid == typeof (void 0) || answerid == void 0 || answerid == "") {
        $("#quizNavigator .quiz-wrapper [name=quizNav" + quizid + "]").removeClass("completed");
        var completed = parseInt($(".quizNumber .completed").text()) - 1;
        $(".quizNumber .completed").text(completed);
        if (completed == totalQuiz) $(".quizNumber .completed").removeClass("finish");
    } else {
        if ($("#quizNavigator .quiz-wrapper [name=quizNav" + quizid + "].completed").length === 1) {
        } else {
            $("#quizNavigator .quiz-wrapper [name=quizNav" + quizid + "]").addClass("completed");
            var completed = parseInt($(".quizNumber .completed").text()) + 1;
            $(".quizNumber .completed").text(completed);
            if (completed == totalQuiz)
                $(".quizNumber .completed").addClass("finish");
        }
    }
}

function BeginExam(_this, LessonID, ClassID) {
    var dataform = new FormData();
    dataform.append("ClassID", ClassID);
    dataform.append("LessonID", LessonID);
    Ajax(urlStart, "POST", dataform, false)
        .then(function (res) {
            var data = JSON.parse(res);
            document.querySelector("input[name='ExamID']").value = data.ID;
            start(_this);
            SetCurrentExam();
        })
        .catch(function (err) {
            console.log(err);
        })
}

function GetCurrentExam() {
    var dataform = new FormData();
    dataform.append("ClassID", ClassID);
    dataform.append("LessonID", LessonID);
    return Ajax(urlCurrentExam, "POST", dataform, true)
        .then(function (res) {
            if (res == null) return;
            var data = JSON.parse(res);
            document.querySelector("input[name='ExamID']").value = data.ID;

            start();
        })
        .catch(function (err) {
            console.log(err);
        })
}
//exampleid
function LoadCurrentExam() {
    //localstorge
    var html = localStorage.getItem($("input[name='ExamID']").val());
    var html2 = localStorage.getItem($("input[name='ExamID']").val() + "_Quiz");
    var html3 = localStorage.getItem($("input[name='ExamID']").val() + "_QuizNav");
    $("#lessonContainer").html(html);
    $("#quiz-number_123").html(html2);
    $("#quizNavigator").html(html3);

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
            answerQuestion(this, questionID, anserID, answerValue);
        }
    });
    $(".answer-wrapper.no-child").droppable({
        tolerance: "intersect",
        accept: ".answer-item",
        activeClass: "hasAnswer",
        hoverClass: "answerHover",
        drop: function (event, ui) {
            var prevHolder = $(ui.helper).parent();
            var quiz = prevHolder.data("id");
            if (quiz == void 0 || quiz == null || quiz == "") return;
            prevHolder.remove($(ui.helper));
            prevHolder.append($("<div>", { "class": "pane-item placeholder", "text": "Thả câu trả lời tại đây" }));
            //$(prevHolder).find(".placeholder").show();
            $(this).append($(ui.draggable));
            answerQuestion(this, quiz, typeof (void 0), "");
        }
    });
}

function SetCurrentExam() {
    var html = $("#lessonContainer").html();
    var html2 = $("#quiz-number_123").html();
    var html3 = $("#quizNavigator").html();
    localStorage.setItem($("input[name='ExamID']").val() + "_QuizNav", html3);
    localStorage.setItem($("input[name='ExamID']").val() + "_Quiz", html2);
    localStorage.setItem($("input[name='ExamID']").val(), html);
}

function RemoveCurrentExam() {
    localStorage.removeItem($("input[name='ExamID']").val() + "_QuizNav");
    localStorage.removeItem($("input[name='ExamID']").val() + "_Quiz");
    localStorage.removeItem($("input[name='ExamID']").val());
}
//hoàn thành
function ExamComplete(url, force) {

    if (!force) { // chưa hết giờ
        if (confirm("Có phải bạn muốn nộp bài")) {
            var exam = document.querySelector("input[name='ExamID']");
            var dataform = new FormData();
            dataform.append("ExamID", exam.value);
            return Ajax(url, "POST", dataform, true)
                .then(function (res) {
                    if (res == null) return;
                    var data = JSON.parse(res);
                    RemoveCurrentExam();
                    $(".lesson-container").empty();
                    $("#quizNavigator").addClass('d-none');
                    $("#finish").addClass('d-none');
                    $('#lessonContainer').removeClass('col-md-10');
                    $(".lesson-container").append('<div class="card show mb-4"></div>');
                    $(".card").append('<div class="card-body d-flex justify-content-center"><h3>Bạn đã hoàn thành bài thi</h3></div>');
                    $(".card").append('<div class="content card-body d-flex justify-content-center"></div>');
                    $(".content").append('<a href="#" onclick="goBack()"> Quay về trang bài học </a>');
                })
                .catch(function (err) {
                    console.log(err);
                })
        }
    }
    else //hết giờ => bắt buộc nộp bài 
    {
        var exam = document.querySelector("input[name='ExamID']");
            var dataform = new FormData();
            dataform.append("ExamID", exam.value);
            return Ajax(url, "POST", dataform, true)
                .then(function (res) {
                    if (res == null) return;
                    var data = JSON.parse(res);
                    RemoveCurrentExam();
                    $(".lesson-container").empty();
                    $("#quizNavigator").addClass('d-none');
                    $("#finish").addClass('d-none');
                    $('#lessonContainer').removeClass('col-md-10');
                    $(".lesson-container").append('<div class="card show mb-4"></div>');
                    $(".card").append('<div class="card-body d-flex justify-content-center"><h3>Bạn đã hoàn thành bài thi</h3></div>');
                    $(".card").append('<div class="content card-body d-flex justify-content-center"></div>');
                    $(".content").append('<a href="#" onclick="goBack()"> Quay về trang bài học </a>');
                })
                .catch(function (err) {
                    console.log(err);
                })
    }
}