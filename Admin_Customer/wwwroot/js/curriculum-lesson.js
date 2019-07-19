var urlBase = "/teacher/";

var myEditor;

//lesson
var urlLesson = {
    "List": "GetListLesson",
    "Details": "GetDetailsLesson",
    "CreateOrUpdate": "CreateOrUpdate",
    "Remove": "Remove",
    "Location": "/ModLessons/Detail/",
    "ChangeParent": "ChangeLessonParent",
    "ChangePos": "ChangeLessonPosition"
};

//lessonpart
var urlLessonPart = {
    "List": "GetListLessonPart",
    "Details": "GetDetailsLessonPart",
    "CreateOrUpdate": "CreateOrUpdate",
    "Remove": "Remove",
    "ChangePos": "ChangePosition"
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
    "Location": "Curriculum/Detail/",
    "HomeLocation": "Curriculum/Index"
}

var urlChapter = {
    "Location": "/ModChapters/Detail/"
}

var modal = $("#lessonModal");
var modalTitle = $("#modalTitle");
var containerLesson = $("#lessonContainer");
var userID = "";
var clientID = "";


var submitForm = function (event) {
    event.preventDefault();
    $('.btnSaveForm').hide();
    var Form = window.modalForm;

    var formdata = new FormData(Form);

    if ($('textarea[name="Description"]').length > 0) {
        formdata.delete("Description");
        formdata.append("Description", myEditor.getData())
    }

    var xhr = new XMLHttpRequest();
    var url = urlBase;
    xhr.open('POST', url + $("#action").val());
    xhr.send(formdata);
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            var data = JSON.parse(xhr.responseText);
            if (data.Error == null || data.Error == "") {
                switch ($("#action").val()) {
                    case "Lesson/" + urlLesson.CreateOrUpdate:
                        //render.lesson(data.data);
                        //document.location = urlLesson.Location + data.Data.ID;
                        document.location = document.location;
                        break;
                    case "LessonPart/" + urlLessonPart.CreateOrUpdate:
                        var part = data.Data;
                        //render.part(part);
                        load.lesson(data.Data.ParentID);
                }
                hideModal();
            }
            else {
                alert(data.Error);
            }
        }
        $('.btnSaveForm').show();
    }
}

var Create = {
    removeLesson: function (id) {
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
                        render.resetLesson();
                    }
                }
            }
        }
    },
    lesson: function () {
        var modalForm = $(window.modalForm);
        showModal();
        modalTitle.html("Chọn Template");
        modalForm.html(template.Type('lesson'));
    },
    lessonPart: function (lessonID) {
        var modalForm = window.modalForm;
        showModal();
        modalTitle.html("Tạo nội dung mới ");
        $(modalForm).empty();
        $(modalForm).append($("<input>", { "type": "hidden", "name": "ParentID", "value": lessonID }));
        $('#action').val("LessonPart/" + urlLessonPart.CreateOrUpdate);
        var selectTemplate = $("<select>", { "class": "templatetype", "onchange": "chooseTemplate()", "name": "Type" });
        $(modalForm).append(selectTemplate);
        $(selectTemplate).append("<option value='0'>--- Chọn kiểu nội dung ---</option>")
            .append("<option value='TEXT'>Nội dung văn bản</option>")
            .append("<option value='VIDEO'>Nội dung video</option>")
            .append("<option value='AUDIO'>Nội dung audio</option>")
            .append("<option value='IMG'>Nội dung hình ảnh</option>")
            .append("<option value='DOC'>Nội dung pdf</option>")
            .append("<option value='VOCAB'>Nội dung từ vựng tiếng Anh</option>")
            .append("<option value='QUIZ1'>Bài tập trắc nghiệm dạng chuẩn</option>")
            .append("<option value='QUIZ2'>Bài tập nghiệm dạng điền từ</option>")
            .append("<option value='QUIZ3'>Bài tập trắc nghiệm nối đáp án</option>")
            .append("<option value='ESSAY'>Bài tập tự luận</option>");
        $(modalForm).append($("<div>", { "class": "lesson_parts" }));
        $(modalForm).append($("<div>", { "class": "question_template hide" }));
        $(modalForm).append($("<div>", { "class": "answer_template hide" }));
    },
    answer: function (lessonPartID) {
        var modalForm = window.modalForm;
        showModal();
        modalTitle.html("Thêm câu trả lời :");
        var data = [
            {
                "Name": "Tạo câu trả lời : ",
                "Hidden": null,
                "Show": [
                    {
                        "DisplayName": "Nội dung",
                        "Name": "Content",
                        "Value": "",
                        "Type": "text",
                        "Length": "col-12"
                    },
                    {
                        "DisplayName": "Đây là đáp án đúng",
                        "Name": "IsAnswer",
                        "Value": "",
                        "Type": "checkbox",
                        "Length": "col-12"
                    }
                ]
            }
        ];
        modalForm.innerHTML = "";
        template.loadFormHTML(data);
        modalForm.innerHTML += '<input type="hidden" name="ParentID" value="' + lessonPartID + '">';
        $("#action").val(urlLessonAnswers.CreateOrUpdate);
        GetCurrentUser();
    },
};

var chooseTemplate = function () {
    //render template base on selected type
    var type = $('.templatetype').val();
    template.lessonPart(type);
}


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
        //Body
        var cardBody = $("<div>", { "class": "card-body" });
        lessonContent.append(cardBody);
        //row
        var lessonRow = $("<div>", { "class": "row" });
        cardBody.append(lessonRow);


        var tabsleft = $("<div>", { "id": "menu-left", "class": "col-md-3" });
        var lessontabs = $("<div>", { "class": "lesson-tabs" });
        var tabs = $("<ul>", { "id": "pills-tab", "class": "nav flex-column nav-pills", "role": "tablist", "aria-orientation": "vertical" });

        var title = $("<div>", { "class": "lesson-header-title" });
        var titleText = $("<span>", { "class": "title-text", "text": data.Title })
        title.append(titleText);

        lessonHeader.append(title);

        if (data.TemplateType == 2) {
            if (data.Timer > 0) {
                var titleTimer = $("<span>", { "class": "title-timer", "text": " - thời gian: " + data.Timer + "p" });
                title.append(titleTimer);
            }
            if (data.Point > 0) {
                var titlePoint = $("<span>", { "class": "title-point", "text": " (" + data.Point + "đ)" });
                title.append(titlePoint);
            }
        }
        var sort    = $("<button>", { "class": "btn btn-primary btn-sort", "title": "Sắp xếp", "onclick": "lessonService.renderSort()" });
        var edit    = $("<button>", { "class": "btn btn-primary btn-edit", "title": "Sửa", "onclick": "lessonService.renderEdit('" + data.ID + "')" });
        var create  = $("<button>", { "class": "btn btn-primary btn-add", "title": "Thêm nội dung", "onclick": "Create.lessonPart('" + data.ID + "')" });
        var close   = $("<button>", { "class": "btn btn-primary btn-close", "text": "X", "onclick": "render.resetLesson()" });
        var remove  = $("<button>", { "class": "btn btn-danger btn-remove", "title": "Xóa", "onclick": "lessonService.remove('" + data.ID + "')" });

        var iconSort = $("<i>", { "class": "fas fa-sort" });
        var iconEdit = $("<i>", { "class": "fas fa-edit" });
        var iconCreate = $("<i>", { "class": "fas fa-plus-square" });
        var iconTrash = $("<i>", { "class": "fas fa-trash" });
        lessonHeader.append(sort);
        sort.append(iconSort);
        lessonHeader.append(edit);
        edit.append(iconEdit);
        lessonHeader.append(create);
        create.append(iconCreate);
        //lessonHeader.append(close);
        lessonHeader.append(remove); //removeLesson
        remove.append(iconTrash);


        lessonRow.append(tabsleft);
        tabsleft.append(lessontabs);
        lessontabs.append(tabs);

        var lessonBody = $("<div>", { "class": "lesson-body", "id": data.ID });

        var bodyright = $("<div>", { "class": "col-md-9" });
        var button = $("<div>", { "class": "float-right" });
        lessonHeader.append(button);
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
    },
    renderSort: function () {
        $("#pills-tab").toggleClass("sorting");
        if ($("#pills-tab").hasClass("sorting")) {
            $(".card-header .btn-sort").addClass("btn-warning");
            $("#pills-tab").sortable({
                revert: "invalid",
                update: function (event, ui) {
                    var id = $(ui.item).find('a').attr("id").replace("pills-", "");
                    var ar = $(this).find("li");
                    var index = $(ar).index(ui.item);
                    lessonPartService.changePos(id, index);
                }
            });
            $("#pills-tab").sortable("enable");
            $("#pills-tab").disableSelection();
        }
        else {
            $(".card-header .btn-sort").removeClass("btn-warning");
            $("#pills-tab").sortable("disable");
        }
    },
    renderEdit: function (id) {
        var modalForm = $(window.modalForm);
        showModal();
        modalTitle.html("Chọn Template");
        modalForm.html(template.Type('lesson'));
        var url = urlBase + "Lesson/";
        $.ajax({
            type: "POST",
            url: url + urlLesson.Details,
            data: { ID: id },
            dataType: "json",
            success: function (data) {
                template.lesson(data.Data.TemplateType, data.Data);
            }
        });
    },
    remove: function (id) {
        var CourseID = $("#CourseID").val();
        var check = confirm("bạn muốn xóa nội dung này ?");
        if (check) {
            $.ajax({
                type: "POST",
                url: urlBase + "Lesson/" + urlLesson.Remove,
                data: {
                    "ID": id
                },
                success: function (data) {
                    if (data.Error == null) {
                        document.location = urlBase + urlCourse.Location + CourseID;
                    }
                    else {
                        alert(data.Error);
                    }
                }
            });
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

var lessonPartService = {
    render: function (id) {
        var modalForm = window.modalForm;
        showModal();
        $(modalForm).empty();
        var url = urlBase + "LessonPart/";
        $.ajax({
            type: "POST",
            url: url + "GetDetail",
            data: { ID: id },
            dataType: "json",
            success: function (data) {
                render.editPart(data.Data);
            }
        });
    },
    edit: function (id) {
        var modalForm = window.modalForm;
        showModal();
        $(modalForm).empty();
        var url = urlBase + "LessonPart/";
        $.ajax({
            type: "POST",
            url: url + "GetDetail",
            data: { ID: id },
            dataType: "json",
            success: function (data) {
                render.editPart(data.Data);
            }
        });
    },
    changePos: function (id, pos) {
        var url = urlBase;
        $.ajax({
            type: "POST",
            url: urlBase + "LessonPart/" + urlLessonPart.ChangePos,
            data: {
                "ID": id,
                "pos": pos
            },
            success: function (data) {
                console.log(data.message);
            }
        });
    },
    remove: function (id) {
        var check = confirm("bạn muốn xóa nội dung này ?");
        if (check) {

            $.ajax({
                type: "POST",
                url: urlBase + "LessonPart/" + urlLessonPart.Remove,
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

}

var questionService = {
    remove: function (obj) {
        if (confirm("Xóa câu hỏi?")) {
            var id = $(obj).siblings("[name$='.ID']").val();
            var quizHolder = $(obj).parent();
            if (!$(obj).parent().hasClass("fieldQuestion"))
                quizHolder = $(obj).parent().parent();
            quizHolder.parent().append($("<input>", { "type": "hidden", "name": "RemovedQuestions", "value": id }));
            quizHolder.remove();
        }
    }
}

var answerService = {
    remove: function (obj) {
        if (confirm("Xóa đáp án?")) {
            var id = $(obj).siblings("[name$='.ID']").val();
            if (id !== "")
                $(obj).parent().parent().append($("<input>", { "type": "hidden", "name": "RemovedAnswers", "value": id }));
            $(obj).parent().remove();
        }
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
        $(modalForm).append($("<input>", { "type": "hidden", "name": "ParentID", "value": data.ParentID }));
        $(modalForm).append($("<input>", { "type": "hidden", "name": "ID", "value": data.ID }));
        $(modalForm).append($("<input>", { "type": "hidden", "name": "Type", "value": data.Type }));

        $('#action').val("LessonPart/" + urlLessonPart.CreateOrUpdate);

        $(modalForm).append($("<div>", { "class": "lesson_parts" }));
        $(modalForm).append($("<div>", { "class": "question_template hide" }));
        $(modalForm).append($("<div>", { "class": "answer_template hide" }));
        template.lessonPart(data.Type, data);

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
        var itembody = $("<div>", { "class": "card-body" });
        tabsitem.append(itembody);

        var itembox = $("<div>", { "class": "part-box " + data.Type, "id": data.ID });
        itembody.append(itembox);


        var boxHeader = $("<div>", { "class": "part-box-header" });
        if (data.Title != null) {
            boxHeader.append($("<h5>", { "class": "title", "text": data.Title + time + point }));
        }
        //boxHeader.append($("<a>", { "class": "btn btn-sm btn-view", "text": "Thu gọn", "onclick": "toggleCompact(this)" }));
        var boxButton = $("<div>", { "class": "text-right" });
        boxButton.append($("<button>", { "class": "btn btn-primary btn-sm btn-edit", "text": "Sửa", "onclick": "lessonPartService.edit('" + data.ID + "')" }))
        boxButton.append($("<button>", { "class": "btn btn-danger btn-sm btn-close", "text": "Xóa", "onclick": "lessonPartService.remove('" + data.ID + "')" }));

        itembox.append(boxHeader);
        boxHeader.append(boxButton);

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
                totalQuiz = data.Questions.length;
                for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
                    var item = data.Questions[i];
                    render.questions(item, data.Type);
                }
                break;
            case "QUIZ3":
                var itemBody = $("<div>", { "class": "quiz-wrapper" });
                itemtitle.prepend($("<i>", { "class": "fab fa-leanpub" }));
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
                totalQuiz = data.Questions.length;
                for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
                    var item = data.Questions[i];
                    render.questions(item, data.Type);
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
                render.mediaContent(data, itemBody, "");
                totalQuiz = data.Questions.length;
                for (var i = 0; data.Questions != null && i < data.Questions.length; i++) {
                    var item = data.Questions[i];
                    render.questions(item, data.Type);
                }
                break;
        }

        if (tabContainer.find(".nav-item").length == 1) {
            itemtitle.addClass("active");
            tabsitem.addClass("show active");
        }
    },
    questions: function (data, template) {
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

                if (data.Description !== "") {
                    var extend = $("<div>", { "class": "quiz-extend", "text": data.Description });
                    quizitem.append(extend);
                }

                for (var i = 0; data.Answers != null && i < data.Answers.length; i++) {
                    var item = data.Answers[i];
                    render.answers(item, template);
                }
                break;
            case "QUIZ3":
                var container = $("#" + data.ParentID + " .quiz-wrapper");

                var quizitem = $("<div>", { "class": "quiz-item", "id": data.ID });

                var quiz_part = $("<div>", { "class": "quiz-pane" });
                var answer_part = $("<div>", { "class": "answer-pane no-child" });
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
                    render.answers(item, template);
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
                    boxHeader.append($("<h4>", { "class": "title", "text": data.Content + point }));
                else
                    boxHeader.append($("<h4>", { "class": "title", "text": point }));

                render.mediaContent(data, boxHeader);

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
                    render.answers(item, template);
                }
                break;
        }
    },
    answers: function (data, template) {
        var container = $("#" + data.ParentID + " .answer-wrapper");
        var answer = $("<fieldset>", { "class": "answer-item" });
        switch (template) {
            case "QUIZ2":

                if ($(container).find(".answer-item").length == 0) {
                    answer.append($("<input>", { "type": "text", "class": "input-text answer-text", "placeholder": data.Content }));
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
                answer.append($("<input>", { "type": "radio", "class": "input-checkbox answer-checkbox", "onclick": "answerQuestion(this,'" + data.ParentID + "')", "name": "rd_" + data.ParentID }));
                if (data.Content != null)
                    answer.append($("<label>", { "class": "answer-text", "text": data.Content }));
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
        wrapper.append($("<input>", { "type": "button", "class": "btn btn-primary btnAddFile", "onclick": "chooseFile(this)", "value": "Chọn file", "tabindex": -1 }));
        wrapper.append($("<input>", { "type": "button", "class": "btn btn-danger btnResetFile hide", "onclick": "resetMedia(this)", "value": "x", "tabindex": -1 }));
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
    },
    mediaContent: function (data, wrapper, type = "") {

        if (data.Media != null) {
            var mediaHolder = $("<div>", { "class": "media-holder " + type });
            switch (type) {
                case "IMG":
                    mediaHolder.append($("<img>", { "src": data.Media.Path }));
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
                            mediaHolder.append($("<img>", { "src": data.Media.Path }));
                        else if (data.Media.Extension.indexOf("video") >= 0)
                            mediaHolder.append("<video controls><source src='" + data.Media.Path + "' type='" + data.Media.Extension + "' />Your browser does not support the video tag</video>");
                        else if (data.Media.Extension.indexOf("audio") >= 0)
                            mediaHolder.append("<audio controls><source src='" + data.Media.Path + "' type='" + data.Media.Extension + "' />Your browser does not support the audio tag</audio>");
                        else
                            mediaHolder.append($("<embed>", { "src": data.Media.Path }));
                    break;
            }
            wrapper.append(mediaHolder);
        }

    }
};

var load = {
    lesson: function (id, classid) {
        if (id != $("#LessonID").val())
            document.location = urlLesson.Location + id;
        else {
            var url = urlBase + "Lesson/";
            $.ajax({
                type: "POST",
                url: url + urlLesson.Details,
                data: { ID: id },
                dataType: "json",
                success: function (data) {
                    render.lesson(data.Data);
                    load.listPart(data.Data.ID, classid);
                }
            });
        }
    },
    listPart: function (lessonID, classID) {

        var url = urlBase + "Curriculum/";
        $.ajax({
            type: "POST",
            url: url + "GetListLessonPart",
            data: { LessonID: lessonID },
            dataType: "json",
            success: function (data) {
                for (var i = 0; data.Data != null && i < data.Data.length; i++) {
                    var item = data.Data[i];
                    render.part(item);
                }
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

var template = {
    defaults: function (type, data = null) {
        var chapterid = $("#ChapterID").val();
        var courseid = $("#CourseID").val();
        if (type == "lesson") {
            return [
                {
                    "Name": "Bài giảng",
                    "Function": "template.lesson(1)",
                    "Icon": "/Contents/img/Application-Engine-icon-150x150.png"
                },
                {
                    "Name": "Bài kiểm tra",
                    "Function": "template.lesson(2)",
                    "Icon": "/Contents/img/Application-Engine-icon-150x150.png"
                }
            ];
        }
        if (type == "lesson1") {
            return [
                {
                    "Name": "Tạo trang bài giảng",
                    "Hidden": [
                        {
                            "Name": "ID",
                            "Value": (data != null && data.ID != null) ? data.ID : ""
                        },
                        {
                            "Name": "TemplateType",
                            "Value": "1",
                        }, {
                            "Name": "CourseID",
                            "Value": courseid
                        }, {
                            "Name": "ChapterID",
                            "Value": chapterid
                        }
                    ],
                    "Show": [
                        {
                            "DisplayName": "Tiêu đề",
                            "Name": "Title",
                            "Value": (data != null && data.Title != null) ? data.Title : "",
                            "Type": "text",
                            "Length": "col-12"
                        }
                    ]
                }
            ];
        }
        if (type == "lesson2") {
            return [
                {
                    "Name": "Tạo trang bài kiểm tra",
                    "Hidden": [
                        {
                            "Name": "ID",
                            "Value": (data != null && data.ID != null) ? data.ID : ""
                        },
                        {
                            "Name": "TemplateType",
                            "Value": "2",
                        }, {
                            "Name": "CourseID",
                            "Value": courseid
                        }, {
                            "Name": "ChapterID",
                            "Value": chapterid
                        }
                    ],
                    "Show": [
                        {
                            "DisplayName": "Tiêu đề",
                            "Name": "Title",
                            "Value": (data != null && data.Title != null) ? data.Title : "",
                            "Type": "text",
                            "Length": "col-12"
                        }, {
                            "DisplayName": "Thời gian làm bài (phút)",
                            "Name": "Timer",
                            "Value": (data != null && data.Timer != null) ? data.Timer : 0,
                            "Type": "number",
                            "Length": "col-6"
                        }, {
                            "DisplayName": "Điểm",
                            "Name": "Point",
                            "Value": (data != null && data.Point != null) ? data.Point : 0,
                            "Type": "number",
                            "Length": "col-6"
                        }
                    ]
                }
            ];
        }
    },
    Type: function (type) {
        var html = '<ul class="template_type">';
        var data = template.defaults(type);
        var count = data == null ? 0 : data.length;
        for (var i = 0; i < count; i++) {
            html += '<li class="template_type_item" onclick="' + data[i].Function + '">';
            html += '<span>' + data[i].Name + '</span>';
            //html += '<img src="' + data[i].Icon + '" alt="' + data[i].Name + '"></li>';
            html += '</li>';
        }
        html += "</ul>";
        return html;
    },
    loadFormHTML: function (data) {
        var modalForm = window.modalForm;
        var str = "";
        var item = data[0];
        modalTitle.html(item.Name);
        var count = item.Hidden == null ? 0 : item.Hidden.length;
        for (var i = 0; i < count; i++) {
            var hiddenItem = item.Hidden[i];
            str += '<input type="hidden" name="' + hiddenItem.Name + '" value="' + hiddenItem.Value + '"/>';
        }
        var countShow = item.Show == null ? 0 : item.Show.length;
        str += "<div class='row'>";
        for (var i = 0; i < countShow; i++) {
            var showItem = item.Show[i];
            str += '<div class="' + showItem.Length + '">'
            if (showItem.Type != "checkbox") {
                str += '<div class="form-group">';
                str += '<label class="bmd-label-floating">' + showItem.DisplayName + ' : </label>';
                str += '<input name="' + showItem.Name + '" type="' + showItem.Type + '" value="' + showItem.Value + '" placeholder="' + showItem.DisplayName + '" class="form-control">';
                str += '</div>';
            } else {
                str += '<div class="form-check"><label class="form-check-label"><input class="form-check-input" type="' + showItem.Type + '" name="' + showItem.Name + '" value="">' + showItem.DisplayName + '<span class="form-check-sign"><span class="check"></span></span>';
                str += '</label></div>';
            }
            str += '</div>';
        }
        if (item.FileUpload != null && item.FileUpload != void 0 && typeof (item.FileUpload) == typeof ([])) {
            var countFileShow = item.FileUpload.length;
            str += '<input type="file" name="file" id="file" style="display:none"/>';
            str += '<div class="col-12"> Upload File : </div>';
            for (var i = 0; i < countFileShow; i++) {
                var fileItem = item.FileUpload[i];
                str += '<div class="' + fileItem.Length + '">'
                str += '<div class="form-group">';
                str += '<button type="button" class="btn btn-success form-control" onclick="' + fileItem.Function + '">' + fileItem.Name + "  " + fileItem.Icon + '</button>'
                str += '</div>';
                str += '</div>';
            }
        }
        str += "</div>";
        $(".template_type").css("display", "none");
        modalForm.innerHTML += str;
    },
    lesson: function (type, data = null) {
        var datatemplate = template.defaults("lesson" + type, data);
        template.loadFormHTML(datatemplate);
        //GetCurrentUser();
        var modalForm = window.modalForm;
        //modalForm += '<input type="hidden" name="TemplateType" value="' + type + '"/>';
        $("#action").val("Lesson/" + urlLesson.CreateOrUpdate);
    },
    lessonPart: function (type, data = null) {
        var contentholder = $('.lesson_parts');
        contentholder.empty();
        var question_template_holder = $('.question_template');
        question_template_holder.empty();
        var answer_template_holder = $('.answer_template');
        answer_template_holder.empty();

        contentholder.append($("<label>", { "class": "title", "text": "Tiêu đề:" }));
        contentholder.append($("<input>", { "type": "text", "name": "Title", "class": "input-text form-control", "placeholder": "Nhập tiêu đề" }));
        if (data != null && data.Title != null)
            contentholder.find("[name=Title]").val(data.Title);

        switch (type) {
            case "TEXT"://Text
                contentholder.append($("<label>", { "class": "title", "text": "Nhập nội dung văn bản" }));
                contentholder.append($("<textarea>", { "id": "editor", "rows": "15", "name": "Description", "class": "input-text form-control", "placeholder": "Nội dung văn bản" }));
                if (data != null && data.Description != null)
                    contentholder.find("[name=Description]").val(data.Description);

                ClassicEditor
                    .create(document.querySelector('#editor'))
                    .then(newEditor => {
                        myEditor = newEditor;
                    })
                    .catch(error => {
                        console.error(error);
                    });
                break;
            case "VIDEO":
                contentholder.append($("<label>", { "class": "title", "text": "Chọn file Video" }));
                contentholder.append($("<div>", { "class": "media_holder" }));
                render.mediaAdd(contentholder.find(".media_holder"), "", "VIDEO", data != null ? data.Media : null);
                contentholder.append($("<div>", { "class": "media_preview" }));
                break;
            case "AUDIO":
                contentholder.append($("<label>", { "class": "title", "text": "Chọn file Audio" }));
                contentholder.append($("<div>", { "class": "media_holder" }));
                render.mediaAdd(contentholder.find(".media_holder"), "", "AUDIO", data != null ? data.Media : null);
                contentholder.append($("<div>", { "class": "media_preview" }));
                break;
            case "IMG":
                contentholder.append($("<label>", { "class": "title", "text": "Chọn file ảnh" }));
                contentholder.append($("<div>", { "class": "media_holder" }));
                render.mediaAdd(contentholder.find(".media_holder"), "", "IMG", data != null ? data.Media : null);
                contentholder.append($("<div>", { "class": "media_preview" }));
                break;
            case "DOC":
                contentholder.append($("<label>", { "class": "title", "text": "Chọn file pdf" }));
                contentholder.append($("<div>", { "class": "media_holder" }));
                render.mediaAdd(contentholder.find(".media_holder"), "", "DOC", data != null ? data.Media : null);
                contentholder.append($("<div>", { "class": "media_preview" }));
                break;
            case "QUIZ1"://Trắc nghiệm chuẩn
                var questionTemplate = $("<fieldset>", { "class": "fieldQuestion", "Order": 0 });
                questionTemplate.append($("<input>", { "type": "hidden", "name": "Questions.ID" }));
                questionTemplate.append($("<input>", { "type": "hidden", "name": "Questions.Order", "value": 0 }));
                questionTemplate.append($("<label>", { "class": "fieldset_title", "text": "" }));
                questionTemplate.append($("<input>", { "type": "button", "class": "quiz-remove", "value": "X", "onclick": "questionService.remove(this)", "tabindex": -1 }));
                questionTemplate.append($("<textarea>", { "rows": "3", "name": "Questions.Content", "class": "input-text quiz-text form-control", "placeholder": "Nội dung câu hỏi" }));
                questionTemplate.append($("<div>", { "class": "media_holder" }));
                render.mediaAdd(questionTemplate.find(".media_holder"), "Questions.");
                questionTemplate.append($("<div>", { "class": "media_preview" }));
                questionTemplate.append($("<label>", { "class": "input_label", "text": "Điểm" }));
                questionTemplate.append($("<input>", { "type": "text", "name": "Questions.Point", "class": "input-text part_point form-control", "placeholder": "Điểm", "value": "1" }));
                questionTemplate.append($("<label>", { "class": "part_label", "text": "Đáp án (click chọn nếu là đáp án đúng)" }));

                var answer_wrapper = $("<div>", { "class": "answer-wrapper" });
                answer_wrapper.append($("<input>", { "type": "button", "class": "btn btnAddAnswer", "value": "+", "onclick": "addNewAnswer(this)" }));
                questionTemplate.append(answer_wrapper);
                questionTemplate.append($("<textarea>", { "rows": "2", "name": "Questions.Description", "class": "input-text part_description", "placeholder": "Giải thích đáp án" }));
                question_template_holder.append(questionTemplate);

                var answerTemplate = $("<fieldset>", { "class": "answer-box" });
                answerTemplate.append($("<input>", { "type": "button", "class": "answer-remove", "value": "X", "onclick": "answerService.remove(this)", "tabindex": -1 }));
                answerTemplate.append($("<input>", { "type": "hidden", "name": "Questions.Answers.ID" }));
                answerTemplate.append($("<input>", { "type": "hidden", "name": "Questions.Answers.ParentID", "value": 0 }));
                answerTemplate.append($("<input>", { "type": "hidden", "name": "Questions.Answers.IsCorrect" }));
                answerTemplate.append($("<input>", { "type": "checkbox", "class": "input-checkbox answer-checkbox", "onclick": "toggleCorrectAnswer(this)" }));
                answerTemplate.append($("<input>", { "type": "text", "name": "Questions.Answers.Content", "class": "input-text answer-text form-control", "placeholder": "Đáp án" }));
                answerTemplate.append($("<div>", { "class": "media_holder" }));
                render.mediaAdd(answerTemplate.find(".media_holder"), "Questions.Answers.");
                answerTemplate.append($("<div>", { "class": "media_preview" }));
                answer_template_holder.append(answerTemplate);

                contentholder.append($("<label>", { "class": "title", "text": "Chọn file media" }));
                contentholder.append($("<div>", { "class": "media_holder" }));
                render.mediaAdd(contentholder.find(".media_holder"), "", "", data != null ? data.Media : null);
                contentholder.append($("<div>", { "class": "media_preview" }));
                contentholder.append($("<div>", { "class": "part_content " + type }));
                contentholder.append($("<input>", { "type": "button", "class": "btn btnAddQuestion", "value": "Thêm câu hỏi", "onclick": "addNewQuestion()" }));
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
                quizWrapper.append($("<input>", { "type": "button", "class": "quiz-remove", "value": "X", "onclick": "questionService.remove(this)", "tabindex": -1 }));
                quizWrapper.append($("<input>", { "name": "Questions.Content", "class": "input-text quiz-text", "placeholder": "Vị trí điền/ Bỏ trống nếu chọn dạng media" }));
                quizWrapper.append($("<label>", { "class": "input_label", "text": "Điểm" }));
                quizWrapper.append($("<input>", { "type": "text", "name": "Questions.Point", "class": "input-text part_point", "placeholder": "Điểm", "value": "1" }));
                quizWrapper.append($("<div>", { "class": "media_holder" }));
                render.mediaAdd(quizWrapper.find(".media_holder"), "Questions.");
                quizWrapper.append($("<div>", { "class": "media_preview" }));
                questionTemplate.append(quizWrapper);

                var answer_wrapper = $("<div>", { "class": "answer-wrapper" });
                answer_wrapper.append($("<input>", { "type": "button", "class": "btn btnAddAnswer", "value": "+", "onclick": "addNewAnswer(this)" }));
                questionTemplate.append(answer_wrapper);
                questionTemplate.append($("<textarea>", { "rows": "2", "name": "Questions.Description", "class": "input-text part_description", "placeholder": "Giải thích đáp án" }));
                question_template_holder.append(questionTemplate);

                var answerTemplate = $("<fieldset>", { "class": "answer-box" });
                answerTemplate.append($("<input>", { "type": "button", "class": "answer-remove", "value": "X", "onclick": "answerService.remove(this)", "tabindex": -1 }));
                answerTemplate.append($("<input>", { "type": "hidden", "name": "Questions.Answers.ID" }));
                answerTemplate.append($("<input>", { "type": "hidden", "name": "Questions.Answers.ParentID", "value": 0 }));
                answerTemplate.append($("<input>", { "type": "hidden", "name": "Questions.Answers.IsCorrect", "value": true }));
                answerTemplate.append($("<input>", { "type": "text", "name": "Questions.Answers.Content", "class": "input-text answer-text", "placeholder": "Đáp án" }));
                answer_template_holder.append(answerTemplate);

                contentholder.append($("<label>", { "class": "title", "text": "Chọn file media" }));
                contentholder.append($("<div>", { "class": "media_holder" }));
                render.mediaAdd(contentholder.find(".media_holder"), "", "", data != null ? data.Media : null);
                contentholder.append($("<div>", { "class": "media_preview" }));
                contentholder.append($("<div>", { "class": "part_content " + type }));
                contentholder.append($("<input>", { "type": "button", "class": "btn btnAddQuestion", "value": "Thêm câu hỏi", "onclick": "addNewQuestion()", "tabindex": -1 }));

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
                questionTemplate.append($("<input>", { "type": "button", "class": "quiz-remove", "value": "X", "onclick": "questionService.remove(this)", "tabindex": -1 }));

                var quizWrapper = $("<div>", { "class": "quiz-wrapper" });
                quizWrapper.append($("<input>", { "type": "text", "name": "Questions.Content", "class": "input-text quiz-text form-control", "placeholder": "Nội dung câu hỏi", "tabindex": 0 }));
                quizWrapper.append($("<div>", { "class": "media_holder" }));
                render.mediaAdd(quizWrapper.find(".media_holder"), "Questions.");
                quizWrapper.append($("<div>", { "class": "media_preview" }));
                quizWrapper.append($("<label>", { "class": "input_label", "text": "Điểm" }));
                quizWrapper.append($("<input>", { "type": "text", "name": "Questions.Point", "class": "input-text part_point form-control", "placeholder": "Điểm", "value": "1", "tabindex": 0 }));
                questionTemplate.append(quizWrapper);

                var answer_wrapper = $("<div>", { "class": "answer-wrapper" });
                answer_wrapper.append($("<input>", { "type": "button", "class": "btn btn-primary btnAddAnswer", "value": "Thêm câu trả lời", "onclick": "addNewAnswer(this)", "tabindex": -1 }));
                questionTemplate.append(answer_wrapper);
                question_template_holder.append(questionTemplate);

                var answerTemplate = $("<fieldset>", { "class": "answer-box selected" });
                answerTemplate.append($("<input>", { "type": "button", "class": "answer-remove", "value": "X", "onclick": "answerService.remove(this)", "tabindex": -1 }));
                answerTemplate.append($("<input>", { "type": "hidden", "name": "Questions.Answers.ID" }));
                answerTemplate.append($("<input>", { "type": "hidden", "name": "Questions.Answers.ParentID", "value": 0 }));
                answerTemplate.append($("<input>", { "type": "hidden", "name": "Questions.Answers.IsCorrect", "value": true }));
                answerTemplate.append($("<input>", { "type": "checkbox", "class": "input-checkbox answer-checkbox", "onclick": "toggleCorrectAnswer(this)", "checked": "checked", "tabindex": 0 }));
                answerTemplate.append($("<input>", { "type": "text", "name": "Questions.Answers.Content", "class": "input-text answer-text form-control", "placeholder": "Đáp án (click chọn nếu là đáp án đúng)", "tabindex": 0 }));
                answerTemplate.append($("<div>", { "class": "media_holder" }));
                render.mediaAdd(answerTemplate.find(".media_holder"), "Questions.Answers.");
                answerTemplate.append($("<div>", { "class": "media_preview" }));
                answer_template_holder.append(answerTemplate);

                contentholder.append($("<label>", { "class": "title", "text": "Chọn file media", "tabindex": -1 }));
                contentholder.append($("<div>", { "class": "media_holder" }));
                render.mediaAdd(contentholder.find(".media_holder"), "", "", data != null ? data.Media : null);
                contentholder.append($("<div>", { "class": "media_preview" }));
                contentholder.append($("<div>", { "class": "part_content " + type }));
                contentholder.append($("<input>", { "type": "button", "class": "btn btnAddQuestion", "value": "Thêm câu hỏi", "onclick": "addNewQuestion()", "tabindex": -1 }));

                //Thêm câu hỏi
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
                contentholder.append($("<label>", { "class": "title", "text": "Nhập nội dung văn bản" }));
                contentholder.append($("<textarea>", { "id": "editor", "rows": "15", "name": "Description", "class": "input-text form-control", "placeholder": "Nội dung văn bản" }));
                if (data != null && data.description != null)
                    contentholder.find("[name=Description]").val(data.description);

                ClassicEditor
                    .create(document.querySelector('#editor'))
                    .then(newEditor => {
                        myEditor = newEditor;
                    })
                    .catch(error => {
                        console.error(error);
                    });

                var questionTemplate = $("<fieldset>", { "class": "fieldQuestion", "Order": 0 });
                questionTemplate.append($("<input>", { "type": "hidden", "name": "Questions.ID" }));
                questionTemplate.append($("<input>", { "type": "hidden", "name": "Questions.Order", "value": 0 }));
                questionTemplate.append($("<label>", { "class": "fieldset_title", "text": "" }));

                var quizWrapper = $("<div>", { "class": "quiz-wrapper" });
                quizWrapper.append($("<input>", { "type": "text", "name": "Questions.Content", "class": "input-text quiz-text form-control", "placeholder": "Nội dung câu hỏi", "tabindex": 0 }));
                quizWrapper.append($("<div>", { "class": "media_holder" }));
                render.mediaAdd(quizWrapper.find(".media_holder"), "Questions.");
                quizWrapper.append($("<div>", { "class": "media_preview" }));
                quizWrapper.append($("<label>", { "class": "input_label", "text": "Điểm" }));
                quizWrapper.append($("<input>", { "type": "text", "name": "Questions.Point", "class": "input-text part_point form-control", "placeholder": "Điểm", "value": "1", "tabindex": 0 }));
                questionTemplate.append(quizWrapper);

                contentholder.append($("<label>", { "class": "title", "text": "Chọn file media", "tabindex": -1 }));
                contentholder.append($("<div>", { "class": "media_holder" }));
                render.mediaAdd(contentholder.find(".media_holder"), "", "", data != null ? data.Media : null);
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

        if (data != null && data.Media != null)
            render.mediaContent(data, contentholder.find(".media_preview:first"), type);
        //debugger;
    }
};

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
    $(obj).siblings(".btnAddFile").val("Chọn file");
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
        $(obj).text("Mở rộng");
    else
        $(obj).text("Thu gọn");
}

var addNewQuestion = function (data = null) {
    console.log(data);
    var container = $('.lesson_parts > .part_content');
    var template = $('.question_template > fieldset');
    var currentpos = $(container).find(".fieldQuestion").length;
    var clone = template.clone();
    $(clone).find('.fieldset_title').text("Câu hỏi " + (currentpos + 1));
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
            render.mediaAdd(clone.find(".media_holder"), "Questions.", "", data.Media);
            render.mediaContent(data, clone.find(".media_preview:first"), "");
        }
    }

    $(clone).find("[name^='Questions.']").each(function () {
        $(this).attr("name", $(this).attr("name").replace("Questions.", "Questions[" + (currentpos) + "]."));
    });
    $(container).append(clone);
    console.log(data);
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

    console.log(data);
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
            render.mediaAdd(clone.find(".media_holder"), "Questions.Answers.", "", data.Media);
            render.mediaContent(data, clone.find(".media_preview:first"), "");
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


var showModal = function () {
    modal.modal('show');
};

var hideModal = function () {
    modal.modal('hide');
};