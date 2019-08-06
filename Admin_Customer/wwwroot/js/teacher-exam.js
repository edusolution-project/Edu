var urlBase = "/teacher/";

var myEditor;
var totalQuiz = 0;

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

var urlExam = {
    "Detail": "GetDetail",
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

        var lessonHeader = $("<div>", { "class": "card-header" });
        lessonContent.append(lessonHeader);

        //row
        var row = $("<div>", { "class": "row" });
        lessonHeader.append(row);

        //header

        var cardBody = $("<div>", { "id": "check-student", "class": "card-body" });
        lessonContent.append(cardBody);
        //row
        var lessonRow = $("<div>", { "class": "row" });
        cardBody.append(lessonRow);


        var tabsleft = $("<div>", { "id": "menu-left", "class": "col-md-2" });
        var lessontabs = $("<div>", { "class": "lesson-tabs" });
        var tabs = $("<ul>", { "id": "pills-tab", "class": "nav flex-column nav-pills", "role": "tablist", "aria-orientation": "vertical" });


        var title = $("<div>", { "class": "lesson-header-title col-lg-4", "text": data.Title });
        row.append(title);

        if (data.TemplateType == 2) {
            if (data.Timer > 0) {
                title.text(title.text() + " - thời gian: " + data.Timer + "p");

                var counter = $("<div>", { "class": "text-center col-lg-4", "text": "Thời gian làm bài " });
                var counterdate = $("<span>", { "id": "counter", "class": "time-counter", "text": (data.Timer < 10 ? ("0" + data.Timer) : data.Timer) + ":00" });
                counter.append(counterdate);
                row.append(counter);
            }
            if (data.Point > 0) {
                title.text(title.text() + " (" + data.Point + "đ)");
            }
        }
        var sort = $("<a>", { "class": "btn btn-sm btn-sort", "text": "Sắp xếp", "onclick": "lessonService.renderSort()" });
        var edit = $("<a>", { "class": "btn btn-sm btn-edit", "text": "Sửa", "onclick": "lessonService.renderEdit('" + data.ID + "')" });
        var close = $("<a>", { "class": "btn btn-sm btn-close", "text": "X", "onclick": "render.resetLesson()" });
        var remove = $("<a>", { "class": "btn btn-sm btn-remove", "text": "Xóa", "onclick": "lessonService.remove('" + data.ID + "')" });
        //lessonHeader.append(sort);
        //lessonHeader.append(edit);
        //lessonHeader.append(close);
        //lessonHeader.append(remove); //removeLesson


        //lessonRow.append(tabsleft);
        tabsleft.append(lessontabs);
        lessontabs.append(tabs);

        var lessonBody = $("<div>", { "class": "lesson-body", "id": data.ID });

        var bodyright = $("<div>", { "class": "col-md-12" });
        var tabscontent = $("<div>", { "id": "pills-tabContent", "class": "tab-content" });
        lessonBody.append(tabscontent);

        lessonRow.append(bodyright);
        bodyright.append(lessonBody);
        lessonBox.append(lessonContainer);

        containerLesson.append(lessonBox);
        var pointBox = $("<div>", { "class": "point-box", "style": " margin: 10px; padding: 10px; border: dashed 1px #CCC" });
        var totalPoint = $("<div>");
        totalPoint.html("Tổng điểm: 0");
        var confirmPoint = $("<button>", { "class": "btn btn-sm btn-edit", "text": "Lưu điểm", "onclick": "", "style": "background: #CCC", "onclick":"alert(\"Đã lưu bảng điểm\")" });
        pointBox.append(totalPoint);
        pointBox.append(confirmPoint);
        lessonBody.append(pointBox);
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
                    template.lesson(data.data.TemplateType, data.data);
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
            var lessonpart = data[i];
            render.part(lessonpart);
        }

    },
    editPart: function (data) {
        var modalForm = window.modalForm;
        modalTitle.html("Cập nhật nội dung");
        $(modalForm).append($("<input>", { "type": "hidden", "name": "ParentID", "value": data.ParentID }));
        $(modalForm).append($("<input>", { "type": "hidden", "name": "ID", "value": data.ID }));
        $(modalForm).append($("<input>", { "type": "hidden", "name": "Type", "value": data.Type }));

        $('#action').val(urlLessonPart.CreateOrUpdate);



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
        var container = $(".tab-content");
        var tabsitem = $("<div>", { "id": "pills-part-" + data.QuestionID, "class": "tab-pane fade", "role": "tabpanel", "aria-labelledby": "pills-" + data.ID });
        var itembody = $("<div>", { "class": "card-body" });
        tabsitem.append(itembody);

        var itembox = $("<div>", { "class": "part-box", "id": data.ID });
        tabsitem.append(itembox);

        var itemBody = $("<div>", { "class": "content-wrapper" });
        if (data.Question != null) {
            itemBody.append($("<div>", { "class": "doc-content" }).html(data.Question));
        }
        var resultHolder = $("<div>", { "class": "result-Holder" });
        var chooseAnswer = $("<div>");
        chooseAnswer.html("Trả lời: " + data.AnswerValue);
        var correctAnswer = $("<div>");
        correctAnswer.html("Đáp án: " + data.RealAnswerValue);
        var point = $("<div>");
        point.html("Điểm: " + data.Point);

        resultHolder.append(chooseAnswer).append(correctAnswer).append(point);
        itemBody.append(resultHolder);
        itembox.append(itemBody);
        container.append(tabsitem);
        tabsitem.addClass("show active");
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

                if (data.Description !== "") {
                    var extend = $("<div>", { "class": "quiz-extend", "text": data.Description, "style": "display:none" });
                    quizitem.append(extend);
                }

                for (var i = 0; data.Answers != null && i < data.Answers.length; i++) {
                    var answer = data.Answers[i];
                    render.answers(answer, template);
                }
                break;
            case "QUIZ3":
                var container = $("#" + data.ParentID + " .quiz-wrapper");

                var quizitem = $("<div>", { "class": "quiz-item", "id": data.ID });

                var quiz_part = $("<div>", { "class": "col-lg-6 quiz-pane" });
                var answer_part = $("<div>", { "class": "col-lg-6 answer-pane no-child" });
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
                        var quizId = $(this).parent().attr('id');
                        answerQuestion($(this), quizId);
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
                    var answer = data.Answers[i];
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

                if (data.Description !== "") {
                    var extend = $("<div>", { "class": "quiz-extend", "text": data.Description, "style": "display:none" });
                    itembox.append(extend);
                }

                container.append(itembox);

                //Render Answer
                for (var i = 0; data.Answers != null && i < data.Answers.length; i++) {
                    var answer = data.Answers[i];
                    render.answers(answer, template);
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
                    answer.append($("<input>", { "type": "text", "class": "input-text answer-text form-control", "placeholder": data.Content, "onchange": "answerQuestion(this,'" + data.ParentID + "')" }));
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
        wrapper.append($("<input>", { "type": "button", "class": "btn btn-primary btnAddFile", "onclick": "chooseFile(this)", "value": "Chọn file", "tabindex": -1 }));
        wrapper.append($("<input>", { "type": "button", "class": "btn btn-primary btnResetFile hide", "onclick": "resetMedia(this)", "value": "x", "tabindex": -1 }));
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
                    mediaHolder.append($("<img>", { "class": "img-fluid lazy", "src": data.Media.Path }));
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
                            mediaHolder.append($("<embed>", { "src": data.Media.Path }));
                    break;
            }
            wrapper.append(mediaHolder);
        }
    }
};

var load = {
    exam: function (id) {
        var url = urlBase + "Exam/";
        $.ajax({
            type: "POST",
            url: url + urlExam.Detail,
            data: { ID: id },
            dataType: "json",
            success: function (data) {
                if (data.Error == null) {
                    render.lesson(data.Lesson);
                    render.lessonPart(data.Data);
                } else {
                    alert("Có lỗi kết nối, vui lòng thử lại");
                    console.log(data.Error);
                }
                //load.listPart(data.Data.ID, classid);
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