"use strict";
var ChuHoa = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "K", "L", "N", "M", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"];
var ChuThuong = ["a", "b", "c", "d", "e", "f", "g", "h", "i", "k", "L", "N", "M", "O", "P", "q", "s", "t", "u", "v", "w", "x", "y", "z"];
var So = [0, 1, 2, 3, 4, 6, 7, 8, 9];

// url
var urlBase = "/cpapi/";
// info user for form
var urlUser = "GetCurrentUser";
//chapter
var urlChapter = {
    "List": "",
    "Details": "",
    "CreateOrUpdate": "",
    "Remove":""
}
//lesson
var urlLesson = {
    "List": "GetListLesson",
    "Details": "GetDetailsLesson",
    "CreateOrUpdate": "CreateOrUpdateLesson",
    "Remove":"RemoveLesson"
};
//lessonpart
var urlLessonPart = {
    "List": "GetListLessonPart",
    "Details": "GetDetailsLessonPart",
    "CreateOrUpdate": "CreateOrUpdateLessonPart",
    "Remove":"RemoveLessonPart"
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
    "List":"GetListLessonExtends",
    "Details":"GetDetailsLessonExtends",
    "Update": "UpdateLessonExtends"
}
var modal = $("#LessonExample");
var modalTitle = $("#exampleModalLabel");
var containerLesson = document.getElementById("template-lesson");
var userID = "";
var clientID = "";
//function
var GetCurrentUser = function () {
    var modalForm = window.modalForm;
    var xhr = new XMLHttpRequest();
    var url = urlBase;
    xhr.open('GET', url + urlUser);
    xhr.send({});
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            var data = JSON.parse(xhr.responseText);
            if (data.code == 200) {
                userID = data.data.UserID;
                clientID = data.data.ClientID;
                if (modalForm != null) {
                    modalForm.innerHTML += '<input type="hidden" name="UserID" value="' + data.data.UserID + '" />';
                    modalForm.innerHTML += '<input type="hidden" name="ClientID" value="' + data.data.ClientID + '" />';
                }
            }
        }
    }
    return xhr;
}
GetCurrentUser();
//modal
var showModal = function () {
    modal.modal('show');
};
var hideModal = function () {
    modal.modal('hide');
};
//end modal
var CreateChapter = function (CourseID) {
    var modalForm = window.frmCourse;
    showModal();
    modalTitle.html("Tạo Chapter");
    modalForm.innerHTML = '<input type="hidden" id="action" value="' + urlChapter.CreateOrUpdate + '" /><div class="form-group"><label>Title :</label><input type="text" class="form-control" name="Name" aria-describedby="emailHelp" placeholder="Tiêu đề"></div>';
    modalForm.innerHTML += '<input type="hidden" name="CourseID" value="' + CourseID + '" />';
    GetCurrentUser();
}
var CreateLesson = function (CourseID,ChapterID) {
    var modalForm = window.frmCourse;
    showModal();
    modalTitle.html("Tạo Lesson");
    modalForm.innerHTML = '<input type="hidden" id="action" value="' + urlLesson.CreateOrUpdate + '" /><div class="form-group"><label>Title :</label><input type="text" class="form-control" name="Title" aria-describedby="emailHelp" placeholder="Tiêu đề"></div>';
    modalForm.innerHTML += '<input type="hidden" name="ChapterID" value="' + ChapterID + '" /><input type="hidden" name="CourseID" value="' + CourseID + '" />';
    GetCurrentUser();
};
var CreateLessonPart = function (LessonID) {
    var modalForm = window.frmCourse;
    modalTitle.html("Tạo Lesson Part");
    modalForm.innerHTML = '<input type="hidden" id="action" value="' + urlLessonPart.CreateOrUpdate + '" /><input type="hidden" name="ParentID" value="' + LessonID + '" />';
    modalForm.innerHTML += '<div class="form-group"><label>Title :</label><input type="text" class="form-control" name="Title" aria-describedby="emailHelp" placeholder="Tiêu đề"></div>';
    modalForm.innerHTML += '<div class="form-group"><label>Nội dung :</label><textarea class="form-control" name="Content" placeholder="Nội dung"></textarea></div>';
    modalForm.innerHTML += '<div class="form-group"><div class="btn btn-primary btn-sm float-left" id="choose-file"><span>Choose file</span><input name="Files" type="file" multiple></div></div><br/>';
    modalForm.innerHTML += '<div class="form-group"><label>Select Type :</label><select name="TemplateType" class="form-control selectpicker" data-style="btn btn-primary btn-round" title="Single Select"><option disabled selected> Single Option</option><option value="0">Tự luận</option><option value="1">Trắc nghiệm</option></select></div>';
    modalForm.innerHTML += '<div class="row"><div class="col-6"><div class="form-group"><label>Point :</label><input type="number" value="1" class="form-control" name="Point" aria-describedby="emailHelp" placeholder="Điểm ex(10)"></div></div><div class="col-6"><div class="form-group"><label>Timer :</label><input type="number" value="0" class="form-control" name="Timer" aria-describedby="emailHelp" placeholder="(0=nolimit )"></div></div></div>';
    showModal();
    document.getElementById("choose-file").addEventListener("click", function () {
        this.querySelector('input[type="file"]').click();
    });
    GetCurrentUser();
};
var CreateLessonAnswer = function (LessonPartID) {
    var modalForm = window.frmCourse;
    modalTitle.html("Tạo Lesson Answer");
    modalForm.innerHTML = '<input type="hidden" id="action" value="' + urlLessonAnswer.CreateOrUpdate + '" /><input type="hidden" name="ParentID" value="' + LessonPartID + '" />';
    modalForm.innerHTML += '<div class="form-group"><label>Title :</label><input type="text" class="form-control" name="Title" aria-describedby="emailHelp" placeholder="Tiêu đề"></div>';
    modalForm.innerHTML += '<div class="form-check"><label class="form-check-label"><input class="form-check-input" type="checkbox" name="IsAnswer" value="">Đây là đáp án đúng<span class="form-check-sign"><span class="check"></span></span>';
    showModal();
    GetCurrentUser();
};
//GetDetails
var getDetailsChapter = function () {
    var modalForm = window.frmCourse;
    var xhr = new XMLHttpRequest();
    var url = urlBase;
    xhr.open('GET', url + urlUser);
    xhr.send(formdata);
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            var data = JSON.parse(xhr.responseText);
            if (data.code == 200) {
                //modalForm.innerHTML += '<input type="hidden" name="UserID" value="' + data.data.UserID + '" />';
                //modalForm.innerHTML += '<input type="hidden" name="ClientID" value="' + data.data.ClientID + '" />';
            }
        }
    }
}
var getDetailsLesson = function () {
    var modalForm = window.frmCourse;
    var xhr = new XMLHttpRequest();
    var url = urlBase;
    xhr.open('GET', url + urlUser);
    xhr.send(formdata);
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            var data = JSON.parse(xhr.responseText);
            if (data.code == 200) {
                //modalForm.innerHTML += '<input type="hidden" name="UserID" value="' + data.data.UserID + '" />';
                //modalForm.innerHTML += '<input type="hidden" name="ClientID" value="' + data.data.ClientID + '" />';
            }
        }
    }
}
var getDetailsLessonPart = function () {
    var modalForm = window.frmCourse;
    var xhr = new XMLHttpRequest();
    var url = urlBase;
    xhr.open('GET', url + urlUser);
    xhr.send(formdata);
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            var data = JSON.parse(xhr.responseText);
            if (data.code == 200) {
                //modalForm.innerHTML += '<input type="hidden" name="UserID" value="' + data.data.UserID + '" />';
                //modalForm.innerHTML += '<input type="hidden" name="ClientID" value="' + data.data.ClientID + '" />';
            }
        }
    }
}
var getDetailsLessonAnswer = function () {
    var modalForm = window.frmCourse;
    var xhr = new XMLHttpRequest();
    var url = urlBase;
    xhr.open('GET', url + urlUser);
    xhr.send(formdata);
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            var data = JSON.parse(xhr.responseText);
            if (data.code == 200) {
                //modalForm.innerHTML += '<input type="hidden" name="UserID" value="' + data.data.UserID + '" />';
                //modalForm.innerHTML += '<input type="hidden" name="ClientID" value="' + data.data.ClientID + '" />';
            }
        }
    }
}
var getDetailsMedia = function () {
    var modalForm = window.modalForm;
    var xhr = new XMLHttpRequest();
    var url = urlBase;
    xhr.open('GET', url + urlUser);
    xhr.send(formdata);
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            var data = JSON.parse(xhr.responseText);
            if (data.code == 200) {
                //modalForm.innerHTML += '<input type="hidden" name="UserID" value="' + data.data.UserID + '" />';
                //modalForm.innerHTML += '<input type="hidden" name="ClientID" value="' + data.data.ClientID + '" />';
            }
        }
    }
}
//update data
var updateChapter = function () {

}
var updateLesson = function () {

}
var updateLessonPart = function () {

}
var updateLessonAnswer = function () {

}
var updateMedia = function () {

}
//render
var renderChapter = function (data) {
    console.log(data);
}
var renderLesson = function (data) {
    console.log(data);
}
var renderLessonPart = function (data) {
    console.log(data);
}
var renderLessonAnswer = function (data) {
    console.log(data);
}
// template
var deleteButton = function (id,fnName) {
    var deletebtn = document.createElement("button");
    deletebtn.innerHTML = '<i class="fas fa-times-circle"></i>';
    deletebtn.classList = "btn btn-sm btn-danger";
    deletebtn.style.position = "absolute";
    deletebtn.style.right = "5px";
    deletebtn.style.top = "0px";
    deletebtn.style.padding = "2px";
    deletebtn.setAttribute("onclick", fnName + "('" + id + "')");
    return deletebtn;
}
var editButton = function (id, fnName) {
    var editbtn = document.createElement("button");
    editbtn.innerHTML = '<i class="fas fa-pencil-alt"></i>';
    editbtn.classList = "btn btn-sm btn-private";
    editbtn.style.position = "absolute";
    editbtn.style.right = "12px";
    editbtn.style.top = "0px";
    editbtn.style.padding = "2px";
    editbtn.setAttribute("onclick", fnName + "('" + id + "')");
    return editbtn;
}
//submit form
var submitForm = function (event) {
    event.preventDefault();
    var Form = window.modalForm;
    var isAnwer = Form.querySelector("input[name='IsAnswer']");
    if (isAnwer != null) isAnwer.value = isAnwer.checked;
    var formdata = new FormData(Form);
    var xhr = new XMLHttpRequest();
    var url = urlBase;
    xhr.open('POST', url + $("#action").val());
    xhr.send(formdata);
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            var data = JSON.parse(xhr.responseText);
            if (data.code == 200) {
                if ($("#action").val() == urlLesson.CreateOrUpdate) {
                    render.lesson(data.data);
                }
                if ($("#action").val() == urlLessonPart.CreateOrUpdate) {
                    render.part(data.data.LessonPart);
                }
                if ($("#action").val() == urlLessonAnswer.CreateOrUpdate) {
                    render.answer(data.data);
                }
            }
            hideModal();
        }
    }
};

//
var Create = {
    removePart: function (id) {
        var check = confirm("bạn muốn xóa nội dung này ?");
        if (check) {
            var xhr = new XMLHttpRequest();
            var url = urlBase;
            xhr.open('POST', url + urlLessonPart.Remove + "?ID=" + id + "&UserID=" + userID + "&ClientID=" + clientID);
            xhr.send({});
            xhr.onreadystatechange = function () {
                if (xhr.readyState == 4 && xhr.status == 200) {
                    var data = JSON.parse(xhr.responseText);
                    if (data.code == 200) {
                        var container = $("#" + data.data);
                        container.css("display", "none");
                    }
                }
            }
        }
    },
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
        GetCurrentUser();
    },
    lessonPart: function (lessonID) {
        var modalForm = window.modalForm;
        showModal();
        modalTitle.html("Chọn Template cho lesson part");
        modalForm.innerHTML = template.Type('lessonPart');
        modalForm.innerHTML +='<input type="hidden" name="ParentID" value="' + lessonID + '">';
        GetCurrentUser();
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
        $("#action").val(urlLessonAnswer.CreateOrUpdate);
        GetCurrentUser();
    }
};
var render = {
    resetLesson: function () {
        containerLesson.innerHTML = "";
    },
    lesson: function (data) {
        render.resetLesson();
        var lessonBox = document.createElement("div");
        lessonBox.classList = "lesson lesson-box p-fixed";
        var lessonContainer = document.createElement("div");
        lessonContainer.classList = "lesson-container";
        lessonBox.appendChild(lessonContainer);
        //header
        var lessonHeader = document.createElement("div");
        lessonHeader.classList = "lesson-header";
        var lessonHeaderTitle = document.createElement("div");
        lessonHeaderTitle.classList = "lesson-header-title";
        lessonHeaderTitle.innerHTML = data.title;
        lessonHeader.appendChild(lessonHeaderTitle);
        if (data.templateType == 2) {
            if (data.timer > 0) {
                var timer = document.createElement("div");
                timer.classList = "timer";
                timer.innerHTML ="Thời gian : "+ data.timer;
                lessonHeader.appendChild(timer);
            }
            if (data.point > 0) {
                var point = document.createElement("div");
                point.classList = "point";
                point.innerHTML = "Điểm : "+ data.point;
                lessonHeader.appendChild(point);
            }
        }
        var close = document.createElement("a");
        close.classList = "btn btn-sm btn-danger btn-close";
        close.innerHTML = "x";
        close.setAttribute("onclick", "render.resetLesson()");
        lessonHeader.appendChild(close)
        var remove = document.createElement("a");
        remove.classList = "btn btn-sm btn-remove";
        remove.innerHTML = "remove";
        remove.setAttribute("onclick", "Create.removeLesson('" + data.id + "')");
        lessonHeader.appendChild(remove); //removeLesson

        lessonContainer.appendChild(lessonHeader);
        var lessonBody = document.createElement("div");
        lessonBody.classList = "lesson-body";
        lessonBody.id = data.id;

        // lesson part
        var createLessonPart = document.createElement("div");
        createLessonPart.classList = "lesson-part";
        var btn = document.createElement("a");
        btn.classList = "btn btn-sm btn-success";
        btn.href = "javascript:void(0)";
        btn.setAttribute("onclick", "Create.lessonPart('" + data.id + "')");
        btn.innerHTML = "Tạo nội dung";
        createLessonPart.appendChild(btn);
        lessonBody.appendChild(createLessonPart);
        lessonContainer.appendChild(lessonBody);
        lessonBox.appendChild(lessonContainer);
        containerLesson.appendChild(lessonBox);
    },
    lessonPart: function (data) {
        for (var i = 0; data != null && i < data.length; i++) {
            var item = data[i];
            render.part(item);
        }
    },
    part: function (data) {
        var timer = "", pointer = "";
        if (data.templateType == 2) {
            if (data.timer > 0) {
                timer = " (" + data.timer + " phút)";
            }
            if (data.point > 0) {
                pointer = " (" + data.point + " điểm)";
            }
        }

        var container = $("#" + data.parentID);
        var partBox = document.createElement("div");
        partBox.classList = "part-box";
        partBox.id = data.id;
        var partItem = document.createElement("div");
        partItem.classList = "part-item";
        partBox.appendChild(partItem);
        var partItemHeader = document.createElement("div");
        partItemHeader.classList = "part-item-header";
        var partTitle = document.createElement("h4");
        partTitle.classList = "title";
        partTitle.innerHTML = data.title + timer + pointer;
        partItemHeader.appendChild(partTitle);
        
        var close = document.createElement("a");
        close.classList = "btn btn-sm btn-danger btn-close";
        close.innerHTML = "x";
        close.style.padding = "3px";
        close.setAttribute("onclick", "Create.removePart('" + data.id + "')");
        partItemHeader.appendChild(close);
        partItem.appendChild(partItemHeader);
        var body = document.createElement("div");
        body.classList = "part-item-body row";
        var media = document.createElement("div");
        media.id = "media" + data.id;
        media.classList = "media-box";
        var bodyContent = document.createElement("div");
        bodyContent.id = "body-content" + data.id;
        bodyContent.classList = "body-content";
        bodyContent.innerHTML = data.content;
        body.appendChild(media);
        body.appendChild(bodyContent);

        partBox.append(body);
        if (data.isExample) {
            var answer = document.createElement("div");
            answer.id = "anwser" + data.id;
            answer.classList = "col-12";
            var createAnwser = document.createElement("button");
            createAnwser.classList = "btn btn-sm";
            createAnwser.setAttribute("onclick", "Create.answer('" + data.id + "')");
            createAnwser.innerHTML = "+";
            answer.appendChild(createAnwser);
            body.appendChild(answer);
            load.listAnswer(data.id);
        }
        container.append(partBox);
        load.listExtends(data.id);
    },
    listExtends: function (data) {
        for (var i = 0; data != null && i < data.length; i++) {
            var item = data[i];
            render.extends(item);
        }
    },
    extends: function (data) {
        var media = $("#media" + data.lessonPartID);
        var body = $("#body" + data.lessonPartID);
        if (data.originalFile.split('/')[2] == "IMG") {
            var imgBox = document.createElement("div");
            imgBox.classList = "img-box";
            var img = document.createElement("img");
            img.src = data.file;
            img.alt = data.nameOriginal;
            img.classList = "img-responsive";
            imgBox.appendChild(img);
            media.append(imgBox);
            media.addClass("col-12 col-md-4");
            body.addClass("col-12 col-md-8");
            
        }
        if (data.originalFile.split('/')[2] == "VIDEO") {
            var videoBox = document.createElement("div");
            videoBox.classList = "video-box";
            var video = document.createElement("iframe");
            video.src = data.file;
            videoBox.append(video);
            media.append(videoBox);
            media.addClass("col-12 col-md-12");
            body.addClass("col-12 col-md-12");
        }
        if (data.originalFile.split('/')[2] == "AUDIO") {
            var videoBox = document.createElement("div");
            videoBox.classList = "audio-box";
            var video = document.createElement("audio");
            video.setAttribute("controls", "");
            var source = document.createElement("source");
            source.src = data.file;
            video.append(source);
            videoBox.append(video);
            media.append(videoBox);
            media.addClass("col-12 col-md-12");
            body.addClass("col-12 col-md-12");
        }
    },
    listAnswer: function (data) {
        for (var i = 0; data != null && i < data.length; i++) {
            var item = data[i];
            render.answer(item);
        }
    },
    answer: function (data) {
        var container = $("#anwser" + data.parentID); //anwser5ce175ac9fd9252268a3e341
        var listAndSer = container.find(".answer-box");
        if (listAndSer == null || listAndSer.length == 0) {
            var anwserBox = document.createElement("div");
            anwserBox.classList = "answer-box row";
            container.append(anwserBox);
            var answerItem = document.createElement("div");
            if (data.isAnswer) {
                answerItem.classList = "answer-item text-danger col-3";
            } else {
                answerItem.classList = "answer-item col-3";
            }
            answerItem.innerHTML = data.content;
            anwserBox.appendChild(answerItem);
        } else {
            var answerItem = document.createElement("div");
            if (data.isAnswer) {
                answerItem.classList = "answer-item text-danger col-3";
            } else {
                answerItem.classList = "answer-item col-3";
            }
            answerItem.innerHTML = data.content;
            listAndSer.append(answerItem);
        }
    }
};
var load = {
    lesson: function (id) {
        var xhr = new XMLHttpRequest();
        var url = urlBase;
        xhr.open('GET', url + urlLesson.Details + "?ID=" + id + "&UserID=" + userID + "&ClientID=" + clientID);
        xhr.send({});
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var data = JSON.parse(xhr.responseText);
                if (data.code == 200) {
                    render.lesson(data.data);
                    load.listPart(data.data.id);
                }
            }
        }
    },
    listPart: function (lessonID) {
        var xhr = new XMLHttpRequest();
        var url = urlBase;
        xhr.open('GET', url + urlLessonPart.List + "?LessonID=" + lessonID + "&UserID=" + userID + "&ClientID=" + clientID);
        xhr.send({});
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var data = JSON.parse(xhr.responseText);
                if (data.code == 200) {
                    render.lessonPart(data.data);
                }
            }
        }
    },
    listExtends: function (partID) {
        var xhr = new XMLHttpRequest();
        var url = urlBase;
        xhr.open('GET', url + urlMedia.List + "?ID=" + partID + "&UserID=" + userID + "&ClientID=" + clientID);
        xhr.send({});
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
        var url = urlBase;
        xhr.open('GET', url + urlLessonAnswer.List + "?LessonPartID=" + partID + "&UserID=" + userID + "&ClientID=" + clientID);
        xhr.send({});
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
    defaults: function (type) {
        var chapterid = $("[sid='ChapterID']").val();
        var courseid = $("[sid='CourseID']").val();
        if (type == "lesson") {
            return [
                {
                    "Name": "Bài giảng",
                    "Function": "template.lesson(1)",
                    "Icon": "/Contents/img/Application-Engine-icon-150x150.png"
                },
                {
                    "Name": "Bài tập",
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
                            "Value": "",
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
                    "Name": "Tạo trang bài tập",
                    "Hidden": [
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
                            "Value": "",
                            "Type": "text",
                            "Length": "col-12"
                        }, {
                            "DisplayName": "Thời gian làm bài (phút)",
                            "Name": "Timer",
                            "Value": 0,
                            "Type": "number",
                            "Length": "col-6"
                        }, {
                            "DisplayName": "Điểm",
                            "Name": "Point",
                            "Value": 0,
                            "Type": "number",
                            "Length": "col-6"
                        }
                    ]
                }
            ];
        }
        if (type == "lessonPart") {
            return [
                {
                    "Name": "Nội dung bài giảng",
                    "Function": "template.lessonPart(1)",
                    "Icon": "/Contents/img/Application-Engine-icon-150x150.png"
                },
                {
                    "Name": "Nội dung bài tập",
                    "Function": "template.lessonPart(2)",
                    "Icon": "/Contents/img/Application-Engine-icon-150x150.png"
                }
            ];
        }
        if (type == "lessonPart1") {
            return [
                {
                    "Name": "Tạo nội dung bài giảng",
                    "Hidden": [
                        {
                            "Name": "TemplateType",
                            "Value": 1,
                        }
                    ],
                    "Show": [
                        {
                            "DisplayName": "Tiêu đề",
                            "Name": "Title",
                            "Value": "",
                            "Type": "text",
                            "Length": "col-12"
                        },
                        {
                            "DisplayName": "Nội dung",
                            "Name": "Content",
                            "Value": "",
                            "Type": "text",
                            "Length": "col-12"
                        }
                    ],
                    "FileUpload": [
                        {
                            "Name": "Video",
                            "Icon": "<i class='fa fa-video-camera'></i>",
                            "Function": "load.MediaForm('video')",
                            "Length": "col-4"
                        },
                        {
                            "Name": "Image",
                            "Icon": "<i class='fa fa-picture-o'></i>",
                            "Function": "load.MediaForm('img')",
                            "Length": "col-4"
                        },
                        {
                            "Name": "Audio",
                            "Icon": "<i class='fa fa-file-audio-o'></i>",
                            "Function": "load.MediaForm('audio')",
                            "Length": "col-4"
                        }
                    ]

                }
            ];
        }
        if (type == "lessonPart2") {
            return [
                {
                    "Name": "Tạo nội dung bài tập",
                    "Hidden": [
                        {
                            "Name": "TemplateType",
                            "Value": 2,
                        }
                    ],
                    "Show": [
                        {
                            "DisplayName": "Tiêu đề",
                            "Name": "Title",
                            "Value": "",
                            "Type": "text",
                            "Length": "col-12"
                        },
                        {
                            "DisplayName": "Nội dung",
                            "Name": "Content",
                            "Value": "",
                            "Type": "text",
                            "Length": "col-12"
                        },
                        {
                            "DisplayName": "Thời gian làm bài (phút)",
                            "Name": "Timer",
                            "Value": 0,
                            "Type": "text",
                            "Length": "col-6"
                        }, {
                            "DisplayName": "Điểm",
                            "Name": "Point",
                            "Value": 0,
                            "Type": "number",
                            "Length": "col-6"
                        }
                    ],
                    "FileUpload": [
                        {
                            "Name": "Video",
                            "Icon": "<i class='fa fa-video-camera'></i>",
                            "Function": "load.MediaForm('video')",
                            "Length": "col-4"
                        },
                        {
                            "Name": "Image",
                            "Icon": "<i class='fa fa-picture-o'></i>",
                            "Function": "load.MediaForm('img')",
                            "Length": "col-4"
                        },
                        {
                            "Name": "Audio",
                            "Icon": "<i class='fa fa-file-audio-o'></i>",
                            "Function": "load.MediaForm('audio')",
                            "Length": "col-4"
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
            html += '<img src="' + data[i].Icon + '" alt="' + data[i].Name + '"></li>';
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
        modalForm.innerHTML +=str;
    },
    lesson: function (type) {
        var data = template.defaults("lesson" + type);
        template.loadFormHTML(data);
        GetCurrentUser();
        var modalForm = window.modalForm;
        modalForm += '<input type="hidden" name="TemplateType" value="' + type + '"/>';
        $("#action").val(urlLesson.CreateOrUpdate)
    },
    lessonPart: function (type) {
        var data = template.defaults("lessonPart" + type);
        template.loadFormHTML(data);
        var modalForm = window.modalForm;
        modalForm += '<input type="hidden" name="TemplateType" value="' + type + '"/>';
        $("#action").val(urlLessonPart.CreateOrUpdate);
    }
};

document.addEventListener("DOMContentLoaded", function () {

})