var urlBase = "/ModLessons/";
var modal = $("#exampleModal");
var modalTitle = $("#exampleModalLabel");
var Actions = ["CreateLesson", "CreateChapter", "CreateNewLesson", "CreateNewLessonPart"];
var showModal = function () {
    modal.modal('show');
};
var hideModal = function () {
    modal.modal('hide');
};
var CreateLesson = function (courseid, chapterid) {
    var modalForm = window.frmLesson;
    /* body... */
    // <div class="course"><div id="content"></div><div class="fn"><a href="javascript:void(0);" onclick="CreateChapter()">Create Chapter</a><a href="javascript:void(0);" onclick="CreateLession()">Create Lession</a></div></div>
    showModal();
    modalTitle.html("Tạo bài giảng");
    modalForm.innerHTML = '<input type="hidden" id="action" value="' + Actions[0] + '" /><input type="hidden" Name="CourseID" value="' + courseid + '" /><input type="hidden" Name="ChapterID" value="' + chapterid + '" /><div class="form-group"><label>Tiêu đề :</label><input type="text" class="form-control" name="Title" placeholder="Tiêu đề"></div>';
};
var CreateChapter = function (ID, ParentID) {
    if (ParentID == void 0) ParentID = "0";
    var modalForm = window.frmLesson;
    modalTitle.html("Tạo Chapter");
    var content = document.getElementById(ID);
    if (content == null) alert("lỗi không tìm thấy container_id");
    modalForm.innerHTML = '<input type="hidden" id="action" value="' + Actions[1] + '" /><input type="hidden" name="ParentID" value="' + ParentID + '" /><input type="hidden" name="CourseID" value="' + ID + '" />';
    modalForm.innerHTML += '<div class="form-group"><label>Title :</label><input type="text" class="form-control" name="Name" aria-describedby="emailHelp" placeholder="Tiêu đề"></div>';
    showModal();
};
var CreateUnit = function (CourseID, ChapterID) {
    var modalForm = window.frmLesson;
    modalTitle.html("Tạo Lesson");
    modalForm.innerHTML = '<input type="hidden" id="action" value="' + Actions[2] + '" /><input type="hidden" name="ChapterID" value="' + ChapterID + '" /><input type="hidden" name="CourseID" value="' + CourseID + '" />';

    modalForm.innerHTML += '<div class="form-group"><label>Title :</label><input type="text" class="form-control" name="Title" aria-describedby="emailHelp" placeholder="Tiêu đề"></div>';
    modalForm.innerHTML += '<div class="form-group"><label>Nội dung :</label><textarea class="form-control" name="Content" placeholder="Nội dung"></textarea></div>';
    modalForm.innerHTML += '<div class="form-group"><div class="btn btn-primary btn-sm float-left" id="choose-file"><span>Choose file</span><input name="Files" type="file" multiple></div></div><br/>';
    modalForm.innerHTML += '<div class="form-group"><label>Select Type :</label><select name="TemplateType" class="form-control selectpicker" data-style="btn btn-primary btn-round" title="Single Select"><option disabled selected> Single Option</option><option value="0">Tự luận</option><option value="1">Trắc nghiệm</option></select></div>';
    modalForm.innerHTML += '<div class="row"><div class="col-6"><div class="form-group"><label>Point :</label><input type="number" value="1" class="form-control" name="Point" aria-describedby="emailHelp" placeholder="Điểm ex(10)"></div></div><div class="col-6"><div class="form-group"><label>Timer :</label><input type="number" value="0" class="form-control" name="Timer" aria-describedby="emailHelp" placeholder="(0=nolimit )"></div></div></div>';
    showModal();
    document.getElementById("choose-file").addEventListener("click", function () {
        this.querySelector('input[type="file"]').click();
    });
};
var CreateLessonPart = function (ID) {
    var modalForm = window.frmLesson;
    modalTitle.html("Tạo LessonPart");
    modalForm.innerHTML = '<input type="hidden" id="action" value="' + Actions[3] + '" /><input type="hidden" name="LessonID" value="' + ID + '" />';
    modalForm.innerHTML += '<div class="form-group"><label>Title :</label><input type="text" class="form-control" name="Title" aria-describedby="emailHelp" placeholder="Tiêu đề"></div>';
    modalForm.innerHTML += '<div class="form-check"><label class="form-check-label"><input class="form-check-input" type="checkbox" name="IsAnswer" value="">Đây là đáp án đúng<span class="form-check-sign"><span class="check"></span></span>';
    showModal();
};
var submitForm = function (event) {
    event.preventDefault();
    var Form = window.frmLesson;
    //var isAnwer = Form.querySelector("input[name='IsAnswer']");
    //if (isAnwer != null) isAnwer.value = isAnwer.checked;
    var formdata = new FormData(Form);
    var xhr = new XMLHttpRequest();
    var url = urlBase;
    xhr.open('POST', url + $("#action").val());
    xhr.send(formdata);
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            var data = JSON.parse(xhr.responseText);
            if (data.code == 200) {
                if ($("#action").val() == Actions[0]) {
                    //RenderList after create lesson
                    renderCourse(data.data);
                }
                if ($("#action").val() == Actions[1]) {
                    renderChapter(data.data);
                }
                if ($("#action").val() == Actions[2]) {
                    renderLesson(data.data.Lesson);
                }
                if ($("#action").val() == Actions[3]) {
                    console.log(data.data);
                    renderLessonDetails(data.data);
                }
            }
            hideModal();
        }
    }
};
var renderLessonDetails = function (data) {
    //Lesson: { content: "fghfgh", title: "fgfghf", code: "fgfghf", chapterID: "5cdb6e219ff4312918f9c970", courseID: "5cdb6de59ff4312918f9c96c", … }
    //LessonExtend: Array(2)
    //0: { lessonID: "5cdb93e738033234bc2ba696", nameOriginal: "download.jpg", file: "/Files/IMG/9c4bd7bf-916c-4a3b-99a4-870808ceafcc_.jpg", createUser: "5cdb6b95439ced2f78596b6d", created: "2019-05-15T04:22:16.209Z", … }
    //1: { lessonID: "5cdb93e738033234bc2ba696", nameOriginal: "illustration-geiranger.jpg", file: "/Files/IMG/8c189a77-4025-473b-a634-0cbac3c7ec5e_.jpg", createUser: "5cdb6b95439ced2f78596b6d", created: "2019-05-15T04:22:18.143Z", … }
    //length: 2
    //__proto__: Array(0)
    //LessonPart: []
    var Lesson = data.Lesson;
    var Media = data.LessonExtend;
    var Answer = data.LessonPart;
    if (Media != void 0 && Media != [] && Media != null) {
        var containerMedia = document.getElementById("media" + Lesson.id);
        for (var i = 0; Media != [] && Media != null && i < Media.length; i++) {
            var item = Media[i];
            if (item.file.indexOf("IMG") > -1) {
                var img = document.createElement("img");
                img.src = item.file;
                img.alt = item.nameOriginal;
                img.style.maxWidth = "100%";
                img.style.float = "left";
                img.style.marginRight = "5px";
                img.style.height = "150px";
                img.style.maxHeight = "100%";
                img.classList = "img-responsive";
                containerMedia.appendChild(img);
            } else {
                if (item.file.indexOf("VIDEO") > -1) {
                    var iframe = document.createElement("iframe");
                    iframe.src = item.file;
                    containerMedia.appendChild(iframe);
                } else {
                    if (item.file.indexOf("AUDIO") > -1) {
                        var iframe = document.createElement("audio");
                        iframe.setAttribute("controls", "");
                        var source = document.createElement("source");
                        source.src = item.file;
                        iframe.appendChild(source);
                        containerMedia.appendChild(iframe);
                    }
                }
            }
        }
    }
    if (Answer != void 0 && Answer != [] && Answer != null){
        var arr = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "K", "H", "L", "N", "M", "O", "P", "Q", "W", "X", "Y", "Z"];
        var container = document.getElementById(Lesson.id);
        var box = document.getElementById("part-item" + Lesson.id);
        if (box == null) {
            box = document.createElement("div");
            box.style.width = "100%";
            box.style.padding = "20px 0";
            box.classList = "row";
            box.innerHTML = "";
            box.id = "part-item" + Lesson.id;
        } else {
            box.innerHTML = "";
            container.removeChild(box);
        }
        for (var i = 0; Answer != [] && Answer != null && i < Answer.length; i++) {
            var item = Answer[i];
            var part = document.createElement("div");
            part.classList = "col-12 col-sm-12 col-md-6 col-lg-6 col-xl-3";
            console.log(item);
            if (item.isAnswer) {
                part.classList.add("text-danger");
            }
            part.style.position = "relative";

            part.innerHTML = arr[i] + " - " + item.title;
            part.setAttribute("onclick", "ToggleTarget('" + item.lessonID + "','" + item.id + "')");
            box.appendChild(part);
            var deletebtn = document.createElement("button");
            deletebtn.innerHTML = "x";
            deletebtn.classList = "btn btn-sm btn-danger";
            deletebtn.style.position = "absolute";
            deletebtn.style.right = "5px";
            deletebtn.style.top = "0px";
            deletebtn.style.padding = "2px";
            deletebtn.setAttribute("onclick", "DeleteLessonPart('" + item.id + "')");
            part.id = "part" + item.id;
            part.appendChild(deletebtn);
        }
        container.prepend(box);
    }
    if ($("#action").val() != Actions[3]) {
        renderFnLessonPart(Lesson.id, Lesson.templateType);
    }
};
var renderCourse = function (data) {
    var cousre = document.querySelector("#Course");
    if (cousre == null) alert("lỗi ko có container_id");
    cousre.innerHTML = "";
    var divCourse = document.createElement("div");
    divCourse.classList = "course";
    // chưa thông tin cần appendchild
    var divContent = document.createElement("div");
    divContent.id = "content";
    divCourse.appendChild(divContent);
    //thông course .
    var courseItem = document.createElement("div");
    courseItem.id = "course-item-" + data.id;
    courseItem.classList = "course-item";
    courseItem.innerHTML = '<div class="course-item__header"><h4 class="title">' + data.name + '</h><div>';
    courseItem.innerHTML += '<div class="course-item__body" id="' + data.id + '"><div>'
    cousre.appendChild(divCourse);
    divContent.appendChild(courseItem);
    divContent.appendChild(renderFn(data.id));
    var deletebtn = document.createElement("button");
    deletebtn.innerHTML = "x";
    deletebtn.classList = "btn btn-sm btn-danger";
    deletebtn.style.position = "absolute";
    deletebtn.style.right = "10px";
    deletebtn.style.top = "10px";
    deletebtn.setAttribute("onclick", "DeleteCourse('" + data.id + "')");
    courseItem.style.position = "relative";
    
    courseItem.appendChild(deletebtn);
};
var renderChapter = function (data) {
    var chapter = data.Chapter == void 0 ? data : data.Chapter;
    var idContainer = chapter.parentID == "0" ? chapter.courseID : chapter.parentID;
    var container = document.getElementById(idContainer);
    var chapterItem = document.createElement("div");
    chapterItem.classList = "chapter-item";
    chapterItem.id = "chapter-item" + chapter.id;
    var chapterHeaderItem = document.createElement("div");
    chapterHeaderItem.classList = "chapter-item__header";
    chapterHeaderItem.innerHTML = '<h4 class="title">' + chapter.name + '</h4>';
    var chapterBodyItem = document.createElement("div");
    chapterBodyItem.classList = "chapter-item__body";
    chapterBodyItem.id = chapter.id;
    chapterItem.appendChild(chapterHeaderItem);
    chapterItem.appendChild(chapterBodyItem);
    chapterItem.appendChild(renderFn(chapter.courseID, chapter.id));
    container.appendChild(chapterItem);
    var deletebtn = document.createElement("button");
    deletebtn.innerHTML = "x";
    deletebtn.classList = "btn btn-sm btn-danger";
    deletebtn.style.position = "absolute";
    deletebtn.style.right = "10px";
    deletebtn.style.top = "10px";
    deletebtn.setAttribute("onclick", "DeleteChapter('" + data.id + "')");
    chapterItem.style.position = "relative";
    chapterItem.appendChild(deletebtn);
};
var renderLesson = function (data) {
    var container_id = data.chapterID == "0" ? data.courseID : data.chapterID;
    var container = document.getElementById(container_id);
    var lesson = document.createElement("div");
    lesson.style.position = "relative";
    lesson.classList = "lesson-item";
    lesson.id = "lesson-item" + data.id;
    container.appendChild(lesson);

    var deletebtn = document.createElement("button");
    deletebtn.innerHTML = "x";
    deletebtn.classList = "btn btn-sm btn-danger";
    deletebtn.style.position = "absolute";
    deletebtn.style.right = "10px";
    deletebtn.style.top = "10px";
    deletebtn.setAttribute("onclick", "DeleteLesson('" + data.id + "')");
    lesson.innerHTML = '<div class="lession-item__header"><h4 class="title">' + data.title + '</h4></div>';
    lesson.innerHTML += '<div class="lession-item__body"><div id="media' + data.id + '"></div><div class="content">' + data.content + '</div><div id="' + data.id + '"></div></div>';
    lesson.appendChild(deletebtn);
    var url = urlBase + "GetLessionDetails?LessonID=" + data.id;
    var xhr = new XMLHttpRequest();
    xhr.open('GET', url);
    xhr.send({});
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            var data = JSON.parse(xhr.responseText);
            if (data.code == 200) {
                renderLessonDetails(data.data);
            }
        }
    }
};
var renderFnLessonPart = function (lessonID, type) {
    var container = document.getElementById(lessonID);
    if (type > 0) {
        var divfn = document.createElement("div");
        divfn.classList = "fn fn-lesson";
        var createAnswer = document.createElement("button");
        createAnswer.classList = "btn btn-sm btn-success";
        createAnswer.type = "button";
        createAnswer.innerHTML = "add lessonpart";
        createAnswer.setAttribute("onclick", "CreateLessonPart('" + lessonID + "')");
        divfn.appendChild(createAnswer);
        container.appendChild(divfn);
    } else {
        var texteara = document.createElement("textarea");
        texteara.rows = 3;
        texteara.classList = "form-control";
        texteara.placeholder = "Noi dung anwser";
        container.appendChild(texteara);
    }
}
var renderFn = function (courseID, chapterID) {
    if (chapterID == void 0) chapterID = "0";
    var div = document.createElement("div");
    div.classList = "fn";
    var buttonChapter = document.createElement("button");
    buttonChapter.classList = "btn btn-sm btn-private";
    buttonChapter.append("Thêm Chapter");
    buttonChapter.type = "button";
    buttonChapter.setAttribute("onclick", "CreateChapter('" + courseID + "','" + chapterID + "')");

    var buttonLesson = document.createElement("button");
    buttonLesson.classList = "btn btn-sm btn-success";
    buttonLesson.type = "button";
    buttonLesson.append("Thêm Lesson");
    buttonLesson.setAttribute("onclick", "CreateLesson('" + courseID + "','" + chapterID + "')");
    div.appendChild(buttonChapter);
    div.appendChild(buttonLesson);
    return div;
};

var renderCourseNoComplete = function (data) {
    var noComplete = document.getElementById("course-no-complete");
    var ul = document.createElement("ul");
    ul.innerHTML = " List Course Đang biên soạn ";
    noComplete.appendChild(ul);
    ul.classList = "ul_course-no-complete";
    for (var i = 0; data != null && i < data.length; i++) {
        var item = data[i];
        var li = document.createElement("li");
        li.innerHTML = item.name;
        li.setAttribute("onclick", "renderCourseDetails('" + item.id + "')")
        ul.appendChild(li)
    }
};
var renderCourseDetails = function (id) {
    var url = "/ModCourses/GetCourseDetails?CourseID=" + id;
    var xhr = new XMLHttpRequest();
    xhr.open('GET', url);
    xhr.send({});
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            var data = JSON.parse(xhr.responseText);
            if (data.code == 200) {
                var course = data.data.Course;
                renderCourse(course);
                var chapter = data.data.Chapter;
                for (var i = 0; chapter != null && i < chapter.length; i++) {
                    var item = chapter[i];
                    renderChapter(item);
                }
                var lesson = data.data.Lesson;
                for (var i = 0; lesson != null && i < lesson.length; i++) {
                    var item = lesson[i];
                    renderLesson(item);
                }

            }
        }
    }
};
var DeleteLesson = function (ID) {
    var check = confirm("Bạn muốn x lesson này !");
    if (check) {
        var data = { ID: ID };
        var url = "/ModCourses/RemoveLesson?ID="+ID;
        var xhr = new XMLHttpRequest();
        xhr.open('Post', url);
        xhr.send(JSON.stringify(data));
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var data = JSON.parse(xhr.responseText);
                if (data.code == 200) {
                    //lesson-item5cdbce81a5a08a1c485934f4
                    var target = document.getElementById("lesson-item" + ID);
                    target.innerHTML = "";
                    target.style.display = "none";
                    target.style.height = "0px";
                    target.style.width = "0px";
                } else {
                    alert(data.message);
                }
            }
        }
    }
};
var DeleteCourse = function (ID) {
    var check = confirm("Bạn muốn x Course này !");
    if (check) {
        var data = { ID: ID };
        var url = "/ModCourses/RemoveCourse?ID=" + ID;
        var xhr = new XMLHttpRequest();
        xhr.open('Post', url);
        xhr.send(JSON.stringify(data));
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var data = JSON.parse(xhr.responseText);
                if (data.code == 200) {
                    //lesson-item5cdbce81a5a08a1c485934f4
                    var target = document.getElementById("Course");
                    target.innerHTML = "";
                } else {
                    alert(data.message);
                }
            }
        }
    }
};
var DeleteChapter = function (ID, CourseID) {
    var check = confirm("Bạn muốn x chapter này !");
    if (check) {
        var data = { ID: ID };
        var url = "/ModCourses/RemoveChapter?ID=" + ID + "&CourseID=" + CourseID;
        var xhr = new XMLHttpRequest();
        xhr.open('Post', url);
        xhr.send(JSON.stringify(data));
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var data = JSON.parse(xhr.responseText);
                if (data.code == 200) {
                    //lesson-item5cdbce81a5a08a1c485934f4
                    var target = document.getElementById("chapter-item" + ID);
                    target.innerHTML = "";
                    target.style.display = "none";
                    target.style.height = "0px";
                    target.style.width = "0px";
                } else {
                    alert(data.message);
                }
            }
        }
    }
};
var DeleteLessonPart = function (ID) {
    var check = confirm("Bạn muốn x đáp án này !");
    if (check) {
        var data = { LessonPartID: ID };
        var url = "/ModCourses/RemoveLessonPart?LessonPartID=" + ID;
        var xhr = new XMLHttpRequest();
        xhr.open('Post', url);
        xhr.send(JSON.stringify(data));
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var data = JSON.parse(xhr.responseText);
                if (data.code == 200) {
                    //lesson-item5cdbce81a5a08a1c485934f4
                    var target = document.getElementById("part" + ID);
                    target.innerHTML = "";
                    target.style.display = "none";
                    target.style.height = "0px";
                    target.style.width = "0px";
                } else {
                    alert(data.message);
                }
            }
        }
    }
};
