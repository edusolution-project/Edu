"use strict";
// url
var urlBase = "/cpapi/";
// info user for form
var urlUser = "GetCurrentUser";
//lesson
var urlLesson = {
    "List": "GetListLesson",
    "Details": "GetDetailsLesson",
    "CreateOrUpdate": "CreateOrUpdateLesson",
    "Remve":"RemoveLesson"
};
//lessonpart
var urlLessonPart = {
    "List": "GetListLessonPart",
    "Details": "GetDetailsLessonPart",
    "CreateOrUpdate": "CreateOrUpdateLessonPart",
    "Remve":"RemoveLessonPart"
};
//lessonAnswer
var urlLessonAnswer = {
    "List": "GetListAnswer",
    "Details": "GetDetailsAnswer",
    "CreateOrUpdate": "CreateOrUpdateLessonAnswer",
    "Remve": "RemoveLessonAnswer"
};
//Media
var urlMedia = {
    "Details":"GetDetailsLessonExtends",
    "Update": "UpdateLessonExtends"
}
var modal = $("#exampleModal");
var modalTitle = $("#exampleModalLabel");

//function
//modal
var showModal = function () {
    modal.modal('show');
};
var hideModal = function () {
    modal.modal('hide');
};
//end modal
// lesson
var CreateLesson = function () {
    var modalForm = window.frmCourse;
    showModal();
    modalTitle.html("Tạo Lesson");
    modalForm.innerHTML = '<input type="hidden" id="action" value="' + urlLesson.CreateOrUpdate + '" /><div class="form-group"><label>Title :</label><input type="text" class="form-control" name="Title" aria-describedby="emailHelp" placeholder="Tiêu đề"></div>';
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
};