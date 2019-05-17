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
    "Details":"GetDetailsLessonExtends",
    "Update": "UpdateLessonExtends"
}
var modal = $("#exampleModal");
var modalTitle = $("#exampleModalLabel");

//function
var GetCurrentUser = function () {
    var modalForm = window.frmCourse;
    var xhr = new XMLHttpRequest();
    var url = urlBase;
    xhr.open('GET', url + urlUser);
    xhr.send(formdata);
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            var data = JSON.parse(xhr.responseText);
            if (data.code == 200) {
                modalForm.innerHTML += '<input type="hidden" name="UserID" value="' + data.data.UserID + '" />';
                modalForm.innerHTML += '<input type="hidden" name="ClientID" value="' + data.data.ClientID + '" />';
            }
        }
    }
}
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
    var Form = window.frmCourse;
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
                if ($("#action").val() == urlChapter.CreateOrUpdate) {
                    renderChapter(data.data);
                }
                if ($("#action").val() == urlLesson.CreateOrUpdate) {
                    renderLesson(data.data);
                }
                if ($("#action").val() == urlLessonPart.CreateOrUpdate) {
                    renderLessonPart(data.data);
                }
                if ($("#action").val() == urlLessonAnswer.CreateOrUpdate) {
                    renderLessonAnswer(data.data);
                }
            }
            hideModal();
        }
    }
};