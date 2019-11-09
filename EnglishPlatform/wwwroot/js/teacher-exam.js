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
    "SaveScore": "Score/Save"
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
        var lessonForm = $("<form>", { "id": data.ID });

        lessonForm.append($("<input>", { "type": "hidden", "name": "ExamID", "value": data.ID }));

        lessonBox.append(lessonContainer);
        var lessonContent = $("<div>", { "class": "card shadow mb-4" });

        lessonContainer.append(lessonContent);


        var lessonHeader = $("<div>", { "class": "card-header" });
        lessonContent.append(lessonHeader);
        lessonContent.append(lessonForm);
        //row
        var row = $("<div>", { "class": "row" });
        lessonHeader.append(row);

        //header
        var cardBody = $("<div>", { "id": "check-student", "class": "card-body" });
        lessonForm.append(cardBody);

        //row
        var lessonRow = $("<div>", { "class": "row" });
        cardBody.append(lessonRow);


        var lessontabs = $("<div>", { "class": "lesson-tabs" });
        var tabs = $("<ul>", { "id": "pills-tab", "class": "nav flex-column nav-pills", "role": "tablist", "aria-orientation": "vertical" });


        var titleRow = $("<div>", { "class": "lesson-header-title" })
        var title = $("<div>");
        var examInfo = $("<div>")
        titleRow.append(title);
        titleRow.append(examInfo);
        row.append(titleRow);

        title.text(data.ClassName + " / " + data.LessonName + " - Điểm chuẩn: " + data.MaxPoint + "đ - Hệ số: " + data.Multiple);

        var completeTime = $.datepicker.formatDate('dd/mm/yy', new Date(data.Updated));

        examInfo.text("Học viên: " + data.StudentName + " - Ngày nộp bài: " + completeTime)

        lessontabs.append(tabs);

        var lessonBody = $("<div>", { "class": "lesson-body", "id": data.ID });

        var bodyright = $("<div>", { "class": "col-md-12" });
        var tabscontent = $("<div>", { "id": "pills-tabContent", "class": "tab-content" });
        lessonBody.append(tabscontent);

        lessonRow.append(bodyright);

        bodyright.append($("<h4>", { "text": "Nội dung học viên đã làm:" }));

        bodyright.append(lessonBody);

        bodyright.append($("<h5>", { "text": "Số lượng câu hoàn thành:" + data.QuestionsDone + " / " + data.QuestionsTotal }));

        //lessonBox.append(lessonContainer);

        containerLesson.append(lessonBox);
        var pointBox = $("<div>", { "class": "point-box", "class": "mt-2 mb-2 p-2", "style": "border: dashed 1px #CCC" });
        var totalPoint = $("<div>");
        totalPoint.html("<h4>Tổng điểm: " + data.Point + "/" + data.MaxPoint + "</h4>");
        pointBox.append(totalPoint);

        if (data.Marked) {
            var markedBox = $("<div>", { "text": "Bài thi đã được chấm lúc " + new Date(data.MarkDate).toLocaleDateString() + " bởi " + data.TeacherName });
            pointBox.append(markedBox);
        }
        else {
            var confirmPoint = $("<button>", { "class": "btn btn-sm btn-primary", "text": "Save điểm", "onclick": "SaveScore('" + data.ID + "')" });
            pointBox.append(confirmPoint);
        }
        cardBody.append(pointBox);
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
        //if (data.Timer > 0) {
        //    time = " (" + data.Timer + "p)";
        //}
        //if (data.Point > 0) {
        //    point = " (" + data.Point + "đ)";
        //}
        var container = $(".tab-content");
        var tabsitem = $("<div>", { "id": "pills-part-" + data.QuestionID, "class": "tab-pane fade", "role": "tabpanel", "aria-labelledby": "pills-" + data.ID });
        var itembody = $("<div>", { "class": "card-body m-0 p-0 pt-2" });
        tabsitem.append(itembody);


        var partTitle = $("<h6>", { "text": data.Title, "class": "font-weight-bold" });
        itembody.append(partTitle);

        var itembox = $("<div>", { "class": "question-part-box", "id": data.ID });
        tabsitem.append(itembox);
        var questions = data.ExamDetails;
        var itemBody = $("<div>", { "class": "content-wrapper" });
        if (questions != null && questions.length > 0) {
            for (var i = 0; i < questions.length; i++) {
                var question = questions[i];
                if (question.QuestionValue != null) {
                    var resultHolder = $("<div>", { "class": "result-Holder" });

                    resultHolder.append($("<input>", { "type": "hidden", "name": "ExamDetails[" + i + "].ID", "value": question.ID }));

                    resultHolder.append($("<div>", { "class": "question-content" }).html("Q: " + question.QuestionValue));
                    var chooseAnswer = $("<div>");
                    chooseAnswer.html("A: " + question.AnswerValue);
                    var correctAnswer = $("<div>");
                    correctAnswer.html("C: " + question.RealAnswerValue);
                    var point = $("<div>");
                    point.append($("<span>", { "text": "Điểm: " }));
                    point.append($("<input>", { "type": "text", "readonly": "readonly", "name": "ExamDetails[" + i + "].Point", "value": question.Point, "style": "width:50px", "class": "text-right d-inline-block form-control" }));
                    point.append($("<span>", { "text": " / " + question.MaxPoint }));
                }
                resultHolder.append(chooseAnswer).append(correctAnswer).append(point);
                itemBody.append(resultHolder);
            }
        }



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
                point = " (" + data.Point + "đ)";
                var container = $("#" + data.ParentID + " .quiz-wrapper");

                var quizitem = $("<div>", { "class": "quiz-item", "id": data.ID });
                var boxHeader = $("<div>", { "class": "quiz-box-header" });
                boxHeader.append($("<div>", { "class": "quiz-text", "text": data.Content + point }));
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
                point = " (" + data.Point + "đ)";
                var container = $("#" + data.ParentID + " .quiz-wrapper");

                var quizitem = $("<div>", { "class": "quiz-item", "id": data.ID });

                var quiz_part = $("<div>", { "class": "col-lg-6 quiz-pane" });
                var answer_part = $("<div>", { "class": "col-lg-6 answer-pane no-child" });
                quizitem.append(quiz_part);
                quizitem.append(answer_part);

                var pane_item = $("<div>", { "class": "pane-item" });
                if (data.Media == null) {
                    pane_item.append($("<div>", { "class": "quiz-text", "text": data.Content + point }));
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
                    render.lesson(data.Data);
                    render.lessonPart(data.Data.Parts);
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

function SaveScore(examid) {

    var myform = document.getElementById(examid);
    var fd = new FormData(myform);
    $.ajax({
        url: urlBase + urlExam.SaveScore,
        data: fd,
        cache: false,
        processData: false,
        contentType: false,
        type: 'POST',
        success: function (data) {
            if (data.Err != null) {
                alert(data.Err);
            }
            else {
                alert("Đã Save bảng điểm");
            }
            // do something with the result
        }
    });
    return false;
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