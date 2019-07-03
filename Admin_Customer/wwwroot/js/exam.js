
let myEditor;

//lesson
var urlLesson = {
    "List": "GetListLesson",
    "Details": "GetDetailsLesson",
    "CreateOrUpdate": "CreateOrUpdateLesson",
    "Remove": "RemoveLesson",
    "Location": "/ModLessons/Detail/",
    "ChangeParent": "ChangeLessonParent",
    "ChangePos": "ChangeLessonPosition"
};

//lessonpart
var urlLessonPart = {
    "List": "GetListLessonPart",
    "Details": "GetDetailsLessonPart",
    "CreateOrUpdate": "CreateOrUpdateLessonPart",
    "Remove": "RemoveLessonPart",
    "ChangePos": "ChangeLessonPartPosition"
};

//lessonAnswer
var urlLessonAnswer = {
    "List": "GetListAnswer",
    "Details": "GetDetailsAnswer",
    "CreateOrUpdate": "CreateOrUpdateLessonAnswer",
    "Remove": "RemoveLessonAnswer"
};

//Media
var urlMedia = {
    "List": "GetListLessonExtends",
    "Details": "GetDetailsLessonExtends",
    "Update": "UpdateLessonExtends"
}

var urlCourse = {
    "Location": "/ModCourses/Detail/",
    "HomeLocation": "/ModCourses/Index"
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
        var lessonHeader = $("<div>", { "class": "card-header py-3" });
        lessonContent.append(lessonHeader);

        //header
        var ButtonStart = $("<div>", { "class": "d-flex justify-content-center pt-5 pb-5" });
        var btnButton = $("<div>", { "class": "btn btn-primary", "onclick": "start(this)", "text": " Bắt đầu làm bài thi" });
        lessonContent.append(ButtonStart);
        ButtonStart.append(btnButton);
        //Body
        var cardBody = $("<div>", { "id": "check-student" , "class": "card-body d-none" });
        lessonContent.append(cardBody);
        //row
        var lessonRow = $("<div>", { "class": "row" });
        cardBody.append(lessonRow);


        var tabsleft = $("<div>", { "id": "menu-left", "class": "col-md-3" });
        var lessontabs = $("<div>", { "class": "lesson-tabs" });
        var tabs = $("<ul>", { "id": "pills-tab", "class": "nav flex-column nav-pills", "role": "tablist", "aria-orientation": "vertical" });


        var title = $("<div>", { "class": "lesson-header-title", "text": data.title });
        lessonHeader.append(title);

        if (data.templateType == 2) {
            if (data.timer > 0) {
                title.text(title.text() + " - thời gian: " + data.timer + "p");
            }
            else {
                title.text(title.text() + " (" + data.point + "đ)");
            }
        }
        var sort = $("<a>", { "class": "btn btn-sm btn-sort", "text": "Sắp xếp", "onclick": "lessonService.renderSort()" });
        var edit = $("<a>", { "class": "btn btn-sm btn-edit", "text": "Sửa", "onclick": "lessonService.renderEdit('" + data.id + "')" });
        var close = $("<a>", { "class": "btn btn-sm btn-close", "text": "X", "onclick": "render.resetLesson()" });
        var remove = $("<a>", { "class": "btn btn-sm btn-remove", "text": "Xóa", "onclick": "lessonService.remove('" + data.id + "')" });
        //lessonHeader.append(sort);
        //lessonHeader.append(edit);
        //lessonHeader.append(close);
        //lessonHeader.append(remove); //removeLesson


        lessonRow.append(tabsleft);
        tabsleft.append(lessontabs);
        lessontabs.append(tabs);

        var lessonBody = $("<div>", { "class": "lesson-body", "id": data.id });

        var bodyright = $("<div>", { "class": "col-md-9" });
        var BtnFinish = $("<div>", { "class": "d-flex justify-content-center" });
        var Finish = $("<button>", { "class": "btn btn-primary", "onclick": "goBack()", "text": " Hoàn thành" });       
        var button = $("<div>", { "class": "text-right" });
        lessonHeader.append(button);
        var counter = $("<div>", { "class": "text-center", "text": "Thời gian làm bài:" });
        var counterdate = $("<span>", { "id": "counter", "text": " 20:00" });
        counter.append(counterdate);
        lessonHeader.append(counter);
        var prevtab = $("<button>", { "class": "prevtab btn btn-success mr-2", "title": "Quay lại", "onclick": "tab_goback()" });
        var iconprev = $("<i>", { "class": "fas fa-arrow-left" });
        var nexttab = $("<button>", { "class": "nexttab btn btn-success", "title": "Tiếp tục", "onclick": "tab_gonext()" });
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
        var btn = $("<a>", { "class": "btn btn-sm btn-success", "text": "Thêm nội dung", "href": "javascript:void(0)", "onclick": "Create.lessonPart('" + data.id + "')" });
        //createLessonPart.append(btn);

        var newLessonPart = $("<div>", { "class": "new-lesson-part", "id": data.id + new Date().getDate() });

        bodyright.append(lessonBody);

        bodyright.append(BtnFinish);
        BtnFinish.append(Finish);

        //lessonContainer.append(createLessonPart);
        //lessonContainer.append(newLessonPart);
        lessonBox.append(lessonContainer);

        //var lessonContainerBackground = $("<div>", { "class": "lesson-container-bg" });
        //containerLesson.append(lessonContainerBackground);
        containerLesson.append(lessonBox);
    },
    renderSort: function () {
        $(".lesson-body").toggleClass("sorting");
        $(".lesson-body .part-box").toggleClass("compactView");
        if ($(".lesson-body").hasClass("sorting")) {
            $(".lesson-header .btn-sort").text("Bỏ sắp xếp");
            $(".lesson-body").sortable({
                revert: "invalid",
                update: function (event, ui) {
                    var id = $(ui.item).attr("id");
                    var ar = $(this).parent().find(".part-box");
                    var index = $(ar).index(ui.item);
                    lessonPartService.changePos(id, index);
                }
            });
            $(".lesson-body").sortable("enable");
            $(".part-box").disableSelection();
        }
        else {
            $(".lesson-header .btn-sort").text("Sắp xếp");
            $(".lesson-body").sortable("disable");
        }
    },
    renderEdit: function (id) {
        var modalForm = $(window.modalForm);
        showModal();
        modalTitle.html("Chọn Template");
        modalForm.html(template.Type('lesson'));
        var xhr = new XMLHttpRequest();
        var url = urlBase;
        xhr.open('GET', url + urlLesson.Details + "?ID=" + id + "&UserID=" + userID + "&ClientID=" + clientID);
        xhr.send({});
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var data = JSON.parse(xhr.responseText);
                if (data.code == 200) {
                    console.log(data.data);
                    template.lesson(data.data.templateType, data.data);
                }
            }
        }
    },
    remove: function (id) {
        var ChapterID = $("#ChapterID").val();
        var CourseID = $("#CourseID").val();
        var check = confirm("bạn muốn xóa nội dung này ?");
        if (check) {
            var xhr = new XMLHttpRequest();
            var url = urlBase;
            xhr.open('POST', url + urlLesson.Remove + "?ID=" + id + "&UserID=" + userID + "&ClientID=" + clientID);
            xhr.send({});
            xhr.onreadystatechange = function () {
                if (xhr.readyState == 4 && xhr.status == 200) {
                    var data = JSON.parse(xhr.responseText);
                    if (data.code == 200) {
                        if (ChapterID != "" && ChapterID != "0")
                            document.location = urlChapter.Location + ChapterID;
                        else
                            if (CourseID != "" && CourseID != "0")
                                document.location = urlCourse.Location + ChapterID;
                            else
                                document.location = urlCourse.HomeLocation;
                    }
                }
            }
        }
    },
    changePos: function (id, pos) {
        var url = urlBase;
        $.ajax({
            type: "POST",
            url: url + urlLesson.ChangePos,
            data: {
                "ID": id,
                "UserID": userID,
                "ClientID": clientID,
                "pos": pos
            },
            success: function (data) {
                console.log(data.message);
                //document.location = document.location;
            },
            error: function (data) {
                console.log(data);
            }
        });
    },
    changeParent: function (id, chapterid, courseid) {
        var url = urlBase;
        $.ajax({
            type: "POST",
            url: url + urlLesson.ChangeParent,
            data: {
                "ID": id,
                "UserID": userID,
                "ClientID": clientID,
                "ChapterID": chapterid,
                "CourseID": courseid
            },
            success: function (data) {
                console.log(data.message);
                //document.location = document.location;
            },
            error: function (data) {
                console.log(data);
            }
        });
    },
    create: function () {
        var modalForm = $(window.modalForm);
        showModal();
        modalTitle.html("Chọn Template");
        modalForm.html(template.Type('lesson'));
    }

}

var render = {
    resetLesson: function () {
        $(containerLesson).html("");
    },
    sortLesson: function () {

    },
    lesson: function (data) {
        lessonService.renderData(data);
    },
    lessonPart: function (data) {
        for (var i = 0; data != null && i < data.length; i++) {
            var item = data[i];
            render.part(item);
        }

    },
    editPart: function (data) {
        var modalForm = window.modalForm;
        modalTitle.html("Cập nhật nội dung");
        $(modalForm).append($("<input>", { "type": "hidden", "name": "ParentID", "value": data.parentID }));
        $(modalForm).append($("<input>", { "type": "hidden", "name": "ID", "value": data.id }));
        $(modalForm).append($("<input>", { "type": "hidden", "name": "Type", "value": data.type }));

        $('#action').val(urlLessonPart.CreateOrUpdate);



        $(modalForm).append($("<div>", { "class": "lesson_parts" }));
        $(modalForm).append($("<div>", { "class": "question_template hide" }));
        $(modalForm).append($("<div>", { "class": "answer_template hide" }));
        template.lessonPart(data.type, data);

    },
    part: function (data) {
        var time = "", point = "";

        if (data.timer > 0) {
            time = " (" + data.timer + "p)";
        }
        if (data.point > 0) {
            point = " (" + data.point + "đ)";
        }

        var container = $("#" + data.parentID).find(".tab-content");
        var tabContainer = $("#menu-left #pills-tab");


        //tabs         
        var lessonitem = $("<li>", { "class": "nav-item" });
        var item = $("<a>", { "id": "pills-" + data.id, "class": "nav-link", "data-toggle": "pill", "href": "#pills-part-" + data.id, "role": "tab", "aria-controls": "pills-" + data.id, "aria-selected": "false", "text": data.title });
        lessonitem.append(item);
        tabContainer.append(lessonitem);
        
        // tabs content
        var tabsitem = $("<div>", { "id": "pills-part-" + data.id, "class": "tab-pane fade", "role": "tabpanel", "aria-labelledby": "pills-" + data.id });
        var itembody = $("<div>", { "class": "card-body" });
        tabsitem.append(itembody);

        var itembox = $("<div>", { "class": "part-box" + data.type, "id": data.id });
        itembody.append(itembox);


        var boxHeader = $("<div>", { "class": "part-box-header" });
        if (data.title != null) {
            boxHeader.append($("<h4>", { "class": "title", "text": data.title + time + point }));
        }
        //boxHeader.append($("<a>", { "class": "btn btn-sm btn-view", "text": "Thu gọn", "onclick": "toggleCompact(this)" }));
        //boxHeader.append($("<a>", { "class": "btn btn-sm btn-edit", "text": "Sửa", "onclick": "edit.lessonPart('" + data.id + "')" }))
        //boxHeader.append($("<a>", { "class": "btn btn-sm btn-close", "text": "Xóa", "onclick": "Create.removePart('" + data.id + "')" }));
        itembox.append(boxHeader);
        switch (data.type) {
            case "TEXT":
                var itemBody = $("<div>", { "class": "content-wrapper" });
                if (data.description != null) {
                    itemBody.append($("<div>", { "class": "doc-content" }).html(data.description));
                }
                item.prepend($("<i>", { "class": "far fa-file-word" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                break;
            case "IMG":
                var itemBody = $("<div>", { "class": "media-wrapper" });
                render.mediaContent(data, itemBody, "IMG");
                if (data.description != null)
                    wrapper.append($("<div>", { "class": "description", "text": data.description }));
                item.prepend($("<i>", { "class": "fas fa-file-image" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                break;
            case "AUDIO":
                var itemBody = $("<div>", { "class": "media-wrapper" });
                render.mediaContent(data, itemBody, "AUDIO");
                if (data.description != null)
                    wrapper.append($("<div>", { "class": "description", "text": data.description }));
                item.prepend($("<i>", { "class": "fas fa-music" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                break;
            case "VIDEO":
                var itemBody = $("<div>", { "class": "media-wrapper" });
                render.mediaContent(data, itemBody, "VIDEO");
                if (data.description != null)
                    wrapper.append($("<div>", { "class": "description", "text": data.description }));
                item.prepend($("<i>", { "class": "far fa-play-circle" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                break;
            case "DOC":
                var itemBody = $("<div>", { "class": "media-wrapper" });
                render.mediaContent(data, itemBody, "DOC");
                if (data.description != null)
                    wrapper.append($("<div>", { "class": "description", "text": data.description }));
                item.prepend($("<i>", { "class": "fas fa-file-word" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                break;
            case "QUIZ1":
            case "QUIZ2":
                var itemBody = $("<div>", { "class": "quiz-wrapper" });
                item.prepend($("<i>", { "class": "fab fa-leanpub" }));
                itembox.append(itemBody);
                render.mediaContent(data, itemBody, "");
                container.append(tabsitem);
                //Render Content

                //Render Question
                for (var i = 0; data.questions != null && i < data.questions.length; i++) {
                    var item = data.questions[i];
                    render.questions(item, data.type);
                }
                break;
            case "QUIZ3":
                var itemBody = $("<div>", { "class": "quiz-wrapper" });
                item.prepend($("<i>", { "class": "fab fa-leanpub" }));
                itembox.append(itemBody);
                render.mediaContent(data, itemBody, "");
                var answers_box = $("<div>", { "class": "answer-wrapper no-child" });
                itembox.append(answers_box);
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
                for (var i = 0; data.questions != null && i < data.questions.length; i++) {
                    var item = data.questions[i];
                    render.questions(item, data.type);
                }
                break;
            case "ESSAY":
                var itemBody = $("<div>", { "class": "content-wrapper" });
                if (data.description != null) {
                    itemBody.append($("<div>", { "class": "doc-content" }).html(data.description));
                }
                item.prepend($("<i>", { "class": "fab fa-leanpub" }));
                itembox.append(itemBody);
                container.append(tabsitem);
                var itemBody = $("<div>", { "class": "quiz-wrapper" });
                itembox.append(itemBody);
                render.mediaContent(data, itemBody, "");
                for (var i = 0; data.questions != null && i < data.questions.length; i++) {
                    var item = data.questions[i];
                    render.questions(item, data.type);
                }
                break;
        }
        if (tabContainer.find(".nav-item").length == 1) {
            item.addClass("active");
            tabsitem.addClass("show active");
        }
    },
    questions: function (data, template) {
        switch (template) {
            case "QUIZ2":
                var container = $("#" + data.parentID + " .quiz-wrapper");

                var quizitem = $("<div>", { "class": "quiz-item", "id": data.id });
                var boxHeader = $("<div>", { "class": "quiz-box-header" });
                boxHeader.append($("<div>", { "class": "quiz-text", "text": data.content }));
                render.mediaContent(data, boxHeader);
                quizitem.append(boxHeader);

                container.append(quizitem);
                var answer_wrapper = $("<div>", { "class": "answer-wrapper" });

                quizitem.append(answer_wrapper);

                if (data.description !== "") {
                    var extend = $("<div>", { "class": "quiz-extend", "text": data.description });
                    quizitem.append(extend);
                }

                for (var i = 0; data.answers != null && i < data.answers.length; i++) {
                    var item = data.answers[i];
                    render.answers(item, template);
                }
                break;
            case "QUIZ3":
                var container = $("#" + data.parentID + " .quiz-wrapper");

                var quizitem = $("<div>", { "class": "quiz-item", "id": data.id });

                var quiz_part = $("<div>", { "class": "quiz-pane" });
                var answer_part = $("<div>", { "class": "answer-pane no-child" });
                quizitem.append(quiz_part);
                quizitem.append(answer_part);

                var pane_item = $("<div>", { "class": "pane-item" });
                if (data.media == null) {
                    pane_item.append($("<div>", { "class": "quiz-text", "text": data.content }));
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

                for (var i = 0; data.answers != null && i < data.answers.length; i++) {
                    var item = data.answers[i];
                    render.answers(item, template);
                }
                break;
            default:
                var point = "";

                if (data.point > 0) {
                    point = " (" + data.point + "đ)";
                }
                var container = $("#" + data.parentID + " .quiz-wrapper");

                var itembox = $("<div>", { "class": "quiz-item", "id": data.id });
                var boxHeader = $("<div>", { "class": "quiz-box-header" });
                if (data.content != null)
                    boxHeader.append($("<h4>", { "class": "title", "text": data.content + point }));
                else
                    boxHeader.append($("<h4>", { "class": "title", "text": point }));

                render.mediaContent(data, boxHeader);

                itembox.append(boxHeader);
                var answer_wrapper = $("<div>", { "class": "answer-wrapper" });

                itembox.append(answer_wrapper);

                if (data.description !== "") {
                    var extend = $("<div>", { "class": "quiz-extend", "text": data.description });
                    itembox.append(extend);
                }

                container.append(itembox);

                //Render Answer
                for (var i = 0; data.answers != null && i < data.answers.length; i++) {
                    var item = data.answers[i];
                    render.answers(item, template);
                }
                break;
        }
    },
    answers: function (data, template) {
        var container = $("#" + data.parentID + " .answer-wrapper");
        var answer = $("<fieldset>", { "class": "answer-item" });
        switch (template) {
            case "QUIZ2":

                if ($(container).find(".answer-item").length == 0) {
                    answer.append($("<input>", { "type": "text", "class": "input-text answer-text", "placeholder": data.content }));
                    container.append(answer);
                }
                else {
                    var oldval = $(container).find(".answer-text").attr("placeholder");
                    $(container).find(".answer-text").attr("placeholder", oldval + " | " + data.content);
                }
                break;
            case "QUIZ3":
                var placeholder = $("#" + data.parentID).find(".answer-pane");
                $(placeholder).removeClass("no-child");
                placeholder.empty().append($("<div>", { "class": "pane-item placeholder", "text": "Thả câu trả lời tại đây" }));
                container = $("#" + data.parentID).parent().siblings(".answer-wrapper");

                if (data.content != null)
                    answer.append($("<input>", { "type": "hidden", "value": data.content }));

                if (data.media != null) {
                    render.mediaContent(data, answer);
                }
                else
                    answer.append($("<label>", { "class": "answer-text", "text": data.content }));

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
                answer.append($("<input>", { "type": "hidden" }));
                answer.append($("<input>", { "type": "checkbox", "class": "input-checkbox answer-checkbox", "onclick": "toggleCorrectAnswer(this)" }));
                if (data.content != null)
                    answer.append($("<label>", { "class": "answer-text", "text": data.content }));
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
        if (data.media != null) {
            var mediaHolder = $("<div>", { "class": "media-holder " + type });
            switch (type) {
                case "IMG":
                    mediaHolder.append($("<img>", { "src": data.media.path }));
                    break;
                case "VIDEO":
                    mediaHolder.append("<video controls><source src='" + data.media.path + "' type='" + data.media.extension + "' />Your browser does not support the video tag</video>");
                    break;
                case "AUDIO":
                    mediaHolder.append("<audio controls><source src='" + data.media.path + "' type='" + data.media.extension + "' />Your browser does not support the audio tag</audio>");
                    break;
                case "DOC":
                    mediaHolder.append($("<embed>", { "src": data.media.path, "width": "100%", "height": "800px" }));
                    break;
                default:
                    if (data.media.extension != null)
                        if (data.media.extension.indexOf("image") >= 0)
                            mediaHolder.append($("<img>", { "src": data.media.path }));
                        else if (data.media.extension.indexOf("video") >= 0)
                            mediaHolder.append("<video controls><source src='" + data.media.path + "' type='" + data.media.extension + "' />Your browser does not support the video tag</video>");
                        else if (data.media.extension.indexOf("audio") >= 0)
                            mediaHolder.append("<audio controls><source src='" + data.media.path + "' type='" + data.media.extension + "' />Your browser does not support the audio tag</audio>");
                        else
                            mediaHolder.append($("<embed>", { "src": data.media.path }));
                    break;
            }
            wrapper.append(mediaHolder);
        }

    }
};

var load = {
    lesson: function (id) {
        if (id != $("#LessonID").val())
            document.location = urlLesson.Location + id;
        else {
            var xhr = new XMLHttpRequest();
            //var url = urlBase;
            //xhr.open('GET', url + urlLesson.Details + "?ID=" + id + "&UserID=" + userID + "&ClientID=" + clientID);
            //xhr.send({});
            //xhr.onreadystatechange = function () {
            //    if (xhr.readyState == 4 && xhr.status == 200) {
            var responseText = '{"code":200,"message":"Success get all","data":{"courseID":"5cf1f2f2f7bae231dc6af312","chapterID":"5cf616cf2fdd75139cf633ee","isParentCourse":false,"templateType":1,"point":0,"timer":0,"createUser":"5ce207bc27e2fc0b4cf765e8","title":"fasdfasf","code":"5ce207bc27e2fc0b4cf765e8_5cf1f2f2f7bae231dc6af312_5cf616cf2fdd75139cf633ee_fasdfasf","isActive":false,"isAdmin":true,"order":0,"media":null,"created":"2019-06-05T06:46:07.241Z","updated":"2019-06-05T06:46:07.241Z","id":"5cf7652f09b1b420e8fb68fb"}}';
            var data = JSON.parse(responseText);
            if (data.code == 200) {

                render.lesson(data.data);
                load.listPart(data.data.id);
            }
            //    }
            //}
        }
    },
    listPart: function (lessonID) {

        var xhr = new XMLHttpRequest();
        //var url = urlBase;
        //xhr.open('GET', url + urlLessonPart.List + "?LessonID=" + lessonID + "&UserID=" + userID + "&ClientID=" + clientID);
        //xhr.send({});
        //xhr.onreadystatechange = function () {
        //    if (xhr.readyState == 4 && xhr.status == 200) {
        var responseText = '{"code":200,"message":"Success","data":[{"questions":[],"parentID":"5cf7652f09b1b420e8fb68fb","title":"Test Dạng văn bản","timer":0,"description":"<p>Không phản ứng mức đề xuất nới trần số giờ làm thêm tối đa từ 300 giờ lên mức 400 giờ/năm nhưng Phó Trưởng ban Quan hệ lao động của Tổng liên đoàn đề xuất phải duy trì quy định về giới hạn số giờ làm thêm trong tháng. Theo ông Lê Đình Quảng, có thể nới hơn nữa giới hạn làm thêm tối đa 30 giờ/tháng nhưng không nên bỏ trần quy định này để tránh tình trạng sử dụng “vắt sức” người lao động để làm thêm giờ trong một khoảng thời gian dài liên tục.</p><p>Ông Quảng nhấn mạnh: “Quy định phải chống được sự lạm dụng chính sách làm thêm, để việc làm thêm đúng với bản chất là nhằm bù đắp sự thiếu hụt lao động đột xuất, nhằm tạo sự linh hoạt, tăng tính cạnh tranh của thị trường lao động Việt Nam so với các nước… chứ không phải ngày nào cũng tăng ca, tháng nào cũng làm thêm, doanh nghiệp lợi dụng việc tăng giờ làm thêm để trả lương thấp, buộc người lao động không có lựa chọn nào ngoài việc phải “tự nguyện” làm thêm giờ mới có thu nhập để đủ trang trải cuộc sống. Phải nói rõ nhà nước không khuyến khích việc làm thêm”.</p>","type":"TEXT","isExam":false,"point":0.0,"created":"2019-07-02T01:02:23.446Z","updated":"2019-07-02T02:18:14.186Z","order":0,"media":null,"id":"5d1aad1f8850bf445c825bcf"},{"questions":[],"parentID":"5cf7652f09b1b420e8fb68fb","title":"Test Video","timer":0,"description":null,"type":"VIDEO","isExam":false,"point":0.0,"created":"2019-07-02T01:02:42.078Z","updated":"2019-07-02T01:02:42.079Z","order":1,"media":{"name":"ui-id-1","originalName":"test.mp4","path":"/Files/VIDEO/20190702/760f209e-9d35-4e64-a318-0f07def10803_.mp4","extension":"video/mp4","created":"2019-07-02T01:02:42.079Z","size":4900365.0},"id":"5d1aad328850bf445c825bd0"},{"questions":[],"parentID":"5cf7652f09b1b420e8fb68fb","title":"test audio","timer":0,"description":null,"type":"AUDIO","isExam":false,"point":0.0,"created":"2019-07-02T02:20:20.516Z","updated":"2019-07-02T02:20:20.518Z","order":2,"media":{"name":"ui-id-1","originalName":"test.mp3","path":"/Files/AUDIO/20190702/ce921a63-0e44-4792-8c17-a6ccd0609083_.mp3","extension":"audio/mp3","created":"2019-07-02T02:20:20.518Z","size":2947074.0},"id":"5d1abf64859e6606b4f12a1f"},{"questions":[],"parentID":"5cf7652f09b1b420e8fb68fb","title":"tes img","timer":0,"description":null,"type":"IMG","isExam":false,"point":0.0,"created":"2019-07-02T02:20:47.466Z","updated":"2019-07-02T02:20:47.466Z","order":3,"media":{"name":"ui-id-2","originalName":"_102853861_kingscollege-geo.jpg","path":"/Files/IMG/20190702/d0a0836e-b2ac-4797-a4ad-71527664f87f_.jpg","extension":"image/jpeg","created":"2019-07-02T02:20:47.466Z","size":57957.0},"id":"5d1abf7f859e6606b4f12a20"},{"questions":[{"answers":[{"parentID":"5d1ac075859e6606b4f12a22","content":"A To make an appointment.","isCorrect":true,"createUser":null,"created":"2019-07-02T02:24:53.139Z","updated":"2019-07-02T02:24:53.139Z","media":null,"order":0,"id":"5d1ac075859e6606b4f12a23"},{"parentID":"5d1ac075859e6606b4f12a22","content":"B To rent a car.","isCorrect":false,"createUser":null,"created":"2019-07-02T02:24:53.148Z","updated":"2019-07-02T02:24:53.148Z","media":null,"order":1,"id":"5d1ac075859e6606b4f12a24"},{"parentID":"5d1ac075859e6606b4f12a22","content":"C To ask about a fee.","isCorrect":false,"createUser":null,"created":"2019-07-02T02:24:53.149Z","updated":"2019-07-02T02:24:53.149Z","media":null,"order":2,"id":"5d1ac075859e6606b4f12a25"},{"parentID":"5d1ac075859e6606b4f12a22","content":"D To apply for a position.","isCorrect":false,"createUser":null,"created":"2019-07-02T02:24:53.15Z","updated":"2019-07-02T02:24:53.15Z","media":null,"order":3,"id":"5d1ac075859e6606b4f12a26"}],"content":"Why is the woman calling?","createUser":null,"created":"2019-07-02T02:24:53.13Z","updated":"2019-07-02T02:24:53.13Z","point":1.0,"order":0,"description":null,"media":null,"parentID":"5d1ac075859e6606b4f12a21","id":"5d1ac075859e6606b4f12a22"}],"parentID":"5cf7652f09b1b420e8fb68fb","title":"Part 3 Questions 11-20","timer":0,"description":null,"type":"QUIZ1","isExam":false,"point":0.0,"created":"2019-07-02T02:24:53.128Z","updated":"2019-07-02T02:24:53.128Z","order":4,"media":null,"id":"5d1ac075859e6606b4f12a21"},{"questions":[{"answers":[{"parentID":"5d1ac382859e6606b4f12a38","content":"A","isCorrect":true,"createUser":null,"created":"2019-07-02T02:37:54.537Z","updated":"2019-07-02T02:38:32.45Z","media":null,"order":0,"id":"5d1ac35a859e6606b4f12a34"},{"parentID":"5d1ac382859e6606b4f12a38","content":"B","isCorrect":true,"createUser":null,"created":"2019-07-02T02:37:54.538Z","updated":"2019-07-02T02:38:32.451Z","media":null,"order":1,"id":"5d1ac35a859e6606b4f12a35"},{"parentID":"5d1ac382859e6606b4f12a38","content":"C","isCorrect":true,"createUser":null,"created":"2019-07-02T02:37:54.54Z","updated":"2019-07-02T02:38:32.452Z","media":null,"order":2,"id":"5d1ac35a859e6606b4f12a36"},{"parentID":"5d1ac382859e6606b4f12a38","content":"D","isCorrect":true,"createUser":null,"created":"2019-07-02T02:37:54.541Z","updated":"2019-07-02T02:38:32.453Z","media":null,"order":3,"id":"5d1ac35a859e6606b4f12a37"}],"content":"What event is being described?","createUser":null,"created":"2019-07-02T02:37:54.535Z","updated":"2019-07-02T02:38:32.449Z","point":0.0,"order":0,"description":null,"media":null,"parentID":"5d1ac12b859e6606b4f12a27","id":"5d1ac382859e6606b4f12a38"}],"parentID":"5cf7652f09b1b420e8fb68fb","title":"PART 4","timer":0,"description":null,"type":"QUIZ2","isExam":false,"point":0.0,"created":"2019-07-02T02:27:55.019Z","updated":"2019-07-02T02:38:32.448Z","order":5,"media":{"name":"ui-id-3","originalName":"the_best_court_in_the_world.jpg","path":"/Files/IMG/20190702/f5617d5d-33dc-4fb5-b2d1-3d7d466ee3f5_.jpg","extension":"image/jpeg","created":"0001-01-01T00:00:00Z","size":0.0},"id":"5d1ac12b859e6606b4f12a27"},{"questions":[{"answers":[{"parentID":"5d1ac23d859e6606b4f12a2e","content":"A","isCorrect":true,"createUser":null,"created":"2019-07-02T02:32:29.997Z","updated":"2019-07-02T02:34:05.583Z","media":null,"order":0,"id":"5d1ac23d859e6606b4f12a2f"},{"parentID":"5d1ac23d859e6606b4f12a2e","content":"B","isCorrect":true,"createUser":null,"created":"2019-07-02T02:33:53.253Z","updated":"2019-07-02T02:34:05.584Z","media":null,"order":1,"id":"5d1ac291859e6606b4f12a31"},{"parentID":"5d1ac23d859e6606b4f12a2e","content":"C","isCorrect":true,"createUser":null,"created":"2019-07-02T02:33:53.254Z","updated":"2019-07-02T02:34:05.585Z","media":null,"order":2,"id":"5d1ac291859e6606b4f12a32"}],"content":"Voyages Jules Verne arrange your travel insurance","createUser":null,"created":"2019-07-02T02:32:29.996Z","updated":"2019-07-02T02:34:05.582Z","point":1.0,"order":0,"description":null,"media":null,"parentID":"5d1ac23d859e6606b4f12a2d","id":"5d1ac23d859e6606b4f12a2e"}],"parentID":"5cf7652f09b1b420e8fb68fb","title":"Questions 31-34 refer to the following advertisement","timer":0,"description":null,"type":"QUIZ3","isExam":false,"point":0.0,"created":"2019-07-02T02:32:29.994Z","updated":"2019-07-02T02:34:05.466Z","order":6,"media":null,"id":"5d1ac23d859e6606b4f12a2d"},{"questions":[],"parentID":"5cf7652f09b1b420e8fb68fb","title":null,"timer":0,"description":"<p>12:00 noon-1:00 P.M., Salon A&nbsp;</p><p>Touring to promote his latest book, Outthinking Public Opinion , author Damian Schnauz makes a stop at the Uppsala International Book Fair to discuss his latest subject, take questions, and sign his books.</p><p>Introductory Course in Graphic Design 1:30-2:30 P.M., Visual Media Centre Professional digital designers Allen Doubek and Ivanette Lacasse will present useful techniques and provide attendees with hands-on practice opportunities.</p>","type":"ESSAY","isExam":false,"point":0.0,"created":"2019-07-02T02:33:23.305Z","updated":"2019-07-02T02:42:46.843Z","order":7,"media":null,"id":"5d1ac273859e6606b4f12a30"}]}';
        var data = JSON.parse(responseText);
        if (data.code == 200) {
            render.lessonPart(data.data);

        }
        //    }
        //}
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