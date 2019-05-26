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
//Media
var urlMedia = {
    "List":"GetListLessonExtends",
    "Details":"GetDetailsLessonExtends",
    "Update": "UpdateLessonExtends"
}
//Question
var urlQuestion = {
    "List": "GetListQuestion",
    "Details": "GetDetailQuestion",
    "CreateOrUpdate": "CreateOrUpdateQuestion",
    "Remove": "RemoveQuestion"
}
//QuestionMedia
var urlQuestionMedia = {
    "List": "GetListAnswer",
    "ListByQuestionID": "GetListAnswerByQuestionID",
    "Details": "GetDetailsAnswer",
    "CreateOrUpdate": "CreateOrUpdateLessonAnswer",
    "Remove": "RemoveAnswer"
}

var modal = $("#LessonExample");
var modalTitle = $("#exampleModalLabel");
var containerLesson = document.getElementById("template-lesson");
var userID = "";
var clientID = "";
//function
var GetCurrentUser = function () {
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

var func = {
    Cancel : function(_this){
        var boo = confirm("Có phải bản muốn hủy bản ghi này ?!");
        if(boo){
            var form = _this.parentElement.parentElement;
            var action = form.querySelector('input[name="action"]');
            if(
                urlLesson.List == action.value ||
                urlLesson.Details == action.value ||
                urlLesson.CreateOrUpdate == action.value ||
                urlLesson.Remove == action.value
            )
                render.resetLesson();
            else{
                var parent = form.parentElement;
                var parentRoot = parent.parentNode;
                parentRoot.removeChild(parent);
            }
        }
    },
    Done : function(_this){
        var form = _this.parentElement.parentElement;
        var parentReal = "";
        var action = form.querySelector('input[name="action"]').value;
        if(action == urlLesson.CreateOrUpdate){
            parentReal = form.parentElement.parentElement;
        }else{
            parentReal = form.parentElement;
        }
        var isAnwer = form.querySelector("input[name='IsAnswer']");
        if (isAnwer != null) isAnwer.value = isAnwer.checked;
        var formdata = new FormData(form);
        var xhr = new XMLHttpRequest();
        var url = urlBase;
        xhr.open('POST', url + action);
        xhr.send(formdata);
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var data = JSON.parse(xhr.responseText);
                if (data.code == 200) {
                    // render giá trị ra form
                    parentReal.id = data.data.id;
                    form.querySelector('input[name="ID"]').value=data.data.id;
                    form.classList.add("complete");
                    //end 
                    if (action == urlLesson.CreateOrUpdate) {
                        render.lesson(data.data,form);
                    }
                    else
                        if (action == urlLessonPart.CreateOrUpdate) {
                            render.part(data.data.LessonPart,form);
                        }
                        else
                            if(action == urlQuestion.CreateOrUpdate){
                                render.question(data.data,form);
                            }
                            else
                                if(action == urlQuestionMedia.CreateOrUpdate){
                                    render.questionMedia(data.data,form);
                                }
                }else{
                    if(data.code == 301){
                        alert("bạn vừa mất quyền biên tập vui lòng đăng nhập lại");
                    }else{
                        if(data.code == 404){
                            alert("ko tìm thấy đối tượng");
                        }
                        else{
                            if(data.code == 500){
                                alert("có lỗi xảy ra trong quá trình biên tập");
                            }
                        }
                    }
                    console.log(data.code,data.message);
                }
            }
        }
    },
    Edit:function(_this){
        var form = _this.parentElement.parentElement;
        console.log(form);
    },
    Delete:function(_this){
        var form = _this.parentElement.parentElement;
        console.log(form);
        var boo = confirm("Có phải bản muốn xóa bản ghi này ?!");
    },
    Hidden:function(_this){
        var form = _this.parentElement.parentElement;
        var action = form.querySelector('input[name="action"]');
        if(
            urlLesson.List == action.value ||
            urlLesson.Details == action.value ||
            urlLesson.CreateOrUpdate == action.value ||
            urlLesson.Remove == action.value
        )
            render.resetLesson();
        else{
            var parent = form.parentElement;
            if(parent.classList.indexOf('minimize')>-1){
                parent.classList.remove('minimize');
            }else{
                parent.classList.add('minimize');
            }
        }
    }
}

var formCreate = {
    cancelAndDone : [
        {
            "Name":"Cancel",
            "Icon":"<i class='material-icons'>cancel</i>",
            "Element":"a",
            "Href":"javascript:void(0)",
            "Func":"func.Cancel(this)",
            "Class":"btn btn-sm btn-danger btn-fab text-light",
            "Title":"Hủy"
        },
        {
            "Name":"Cancel",
            "Icon":"<i class='material-icons'>done</i>",
            "Element":"a",
            "Href":"javascript:void(0)",
            "Func":"func.Done(this)",
            "Class":"btn btn-sm btn-success btn-fab text-light",
            "Title":"Lưu hoàn thành"
        }
    ],
    edit_del_hide :[
        {
            "Name":"Edit",
            "Icon":"<i class='material-icons'>border_color</i>",
            "Element":"a",
            "Href":"javascript:void(0)",
            "Func":"func.Edit(this)",
            "Class":"btn btn-sm btn-info btn-fab text-light",
            "Title":"Sửa"
        },
        {
            "Name":"Delete",
            "Icon":"<i class='material-icons'>delete_sweep</i>",
            "Element":"a",
            "Href":"javascript:void(0)",
            "Func":"func.Delete(this)",
            "Class":"btn btn-sm btn-fab btn-muted text-light",
            "Title":"Xóa"
        },
        {
            "Name":"Minime",
            "Icon":"<i class='material-icons'>minimize</i>",
            "Element":"a",
            "Href":"javascript:void(0)",
            "Func":"func.Hidden(this)",
            "Class":"btn btn-sm btn-warning btn-fab text-light",
            "Title":"ẩn"
        }
    ],
    extendsFunc : function(el,type){
        if(el == null) alert("chưa khởi tạo form");
        var div = el.querySelector(".fn-lesson-btn");
        if(div == null){
            div = document.createElement("div");
            div.classList = "fn-lesson-btn";
        }
        div.innerHTML = "";
        var arr = [];
        //create
        if(type == 0){
            arr = formCreate.cancelAndDone;
            
        }else {
            // complete
            arr = formCreate.edit_del_hide;
        }
        for(var i = 0 ; i < arr.length;i++){
            var item = arr[i];
            var elItem = document.createElement(item.Element);
            elItem.classList = item.Class;
            elItem.href = item.Href;
            elItem.setAttribute("onclick",item.Func);
            elItem.innerHTML = item.Icon;
            div.appendChild(elItem);
        }
        el.appendChild(div);
    },
    chooseTemplateLesson : function(){
        var modalForm = $(window.modalForm);
        showModal();
        modalTitle.html("Chọn Template cho Lesson");
        modalForm.html(formTemplate.TemplateLesson());
    },
    lesson:function(type){
        formTemplate.newTemplateLessonForm(type);
    },
    lessonPart:function(lessonID,type){
        var modalForm = $(window.modalForm);
        showModal();
        modalTitle.html("Chọn Template cho Lesson Part");
        modalForm.html(formTemplate.TemplateLessonPart(lessonID,type));
    },
    lessonPartBT:function(type,_this){
        //type là keeir hiển thị nội dung
        var lessonId = _this.getAttribute('data-lesson-id');
        formTemplate.newTemplateLessonPartForm(type,lessonId,1);//1 dạng bài tập
    },
    Question:function(id){
        var modalForm = $(window.modalForm);
        showModal();
        modalTitle.html("Chọn Template cho câu hỏi");
        modalForm.html(formTemplate.TemplateQuestion(id));
    },
    QuestionForm:function(type,id){
        var container = document.getElementById(id);
        var body = container.querySelector(".lb-item-body");
        body.innerHTML += formTemplate.newQuestionAndAnswer(type,id);
        //body.querySelector(".fn-lb-item-body").style.display = 'none';
        hideModal();
    }
};
var formTemplate = {
    lessonpartBT :[
        {
            "DisplayName":"Dạng 1",
            "Name": "Template 1",
            "Func": "formCreate.lessonPartBT(1,this)",
            "Icon": "/Contents/img/1.jpg",
            "ID":"",
            "Value":""
        },
        {
            "DisplayName":"Dạng 2",
            "Name": "Template 2",
            "Func": "formCreate.lessonPartBT(2,this)",
            "Icon": "/Contents/img/2.jpg",
            "ID":"",
            "Value":""
        },
        {
            "DisplayName":"Dạng 3",
            "Name": "Template 3",
            "Func": "formCreate.lessonPartBT(3,this)",
            "Icon": "/Contents/img/3.jpg",
            "ID":"",
            "Value":""
        },
        {
            "DisplayName":"Dạng 4",
            "Name": "Template 4",
            "Func": "formCreate.lessonPartBT(4,this)",
            "Icon": "/Contents/img/4.jpg",
            "ID":"",
            "Value":""
        },
        {
            "DisplayName":"Dạng 5",
            "Name": "Template 5",
            "Func": "formCreate.lessonPartBT(5,this)",
            "Icon": "/Contents/img/5.jpg",
            "ID":"",
            "Value":""
        },
        {
            "DisplayName":"Dạng 6",
            "Name": "Template 6",
            "Func": "formCreate.lessonPartBT(6,this)",
            "Icon": "/Contents/img/6.jpg",
            "ID":"",
            "Value":""
        }
    ],
    lessonForm:{
        "Extends":[
            {
                "DisplayName":"thời gian làm bài",
                "Name":"Timer"
            },
            {
                "DisplayName":"điểm làm bài",
                "Name":"Point"
            }
        ],
        "Items":[
            {
                "DisplayName":"thời gian làm bài",
                "Name":"Title"
            }
        ]
    },
    lesson : [
        {
            "DisplayName":"Bài giảng",
            "Name":"IsExample",
            "ID":"",
            "Value":0,
            "Class":"col-6",
            "Func":"formCreate.lesson(0)",
            "Element":"div",
            "Type":"",
            "Title":"",
            "Icon":""
            
        },
        {
            "DisplayName":"Bài tập",
            "Name":"IsExample",
            "ID":"",
            "Value":1,
            "Class":"col-6",
            "Func":"formCreate.lesson(1)",
            "Element":"div",
            "Type":"",
            "Title":"",
            "Icon":""
        }

    ],
    TemplateQuestion:function(id){
        var arr = [
            {
            "DisplayName":"Dạng 1",
            "Name": "Template 1",
            "Func": "formCreate.QuestionForm(1,'"+id+"')",
            "Icon": "/Contents/img/1.jpg",
            "ID":"",
            "Value":""
        },
        {
            "DisplayName":"Dạng 2",
            "Name": "Template 2",
            "Func": "formCreate.QuestionForm(2,'"+id+"')",
            "Icon": "/Contents/img/2.jpg",
            "ID":"",
            "Value":""
        },
        {
            "DisplayName":"Dạng 3",
            "Name": "Template 3",
            "Func": "formCreate.QuestionForm(3,'"+id+"')",
            "Icon": "/Contents/img/3.jpg",
            "ID":"",
            "Value":""
        }
        ];
        return html.ul_type(arr);
    },
    TemplateLessonPart:function(LessonID,type){
        var arr = [];
        if(type == 1){
            arr = formTemplate.lessonpartBT;
        }else{
            arr = formTemplate.lessonpartBG;
        }
        return html.ul_type(arr,LessonID,type);
    },
    TemplateLesson : function(){
        var arr = formTemplate.lesson;
        return html.ul_type(arr);
    },
    newTemplateLessonForm :function(type){
        var templateLesson = document.getElementById("template-lesson");
        templateLesson.innerHTML ="";
        templateLesson.appendChild(html.newLesson(type));
        hideModal();
    },
    newTemplateLessonPartForm:function(TypeTemplate,lessonID,TypePart){
        var bodyItem = document.getElementById(lessonID).querySelector(".lesson-body > .lesson-body-item");
        bodyItem.appendChild(html.newLessonPart(TypeTemplate,lessonID,TypePart));
        var form = bodyItem.querySelector("form[name='lessonpart_id']");
        formCreate.extendsFunc(form,0);
        hideModal();
    },
    newQuestionAndAnswer:function(type,id){
        if(type == 1){ // 1 câu hỏi nhiều đáp án
            var htmls = '<div class="col-12"><div class="lb-item-questions">'+formTemplate.newQuestionForm(id,type)+'<div class="lb-item-answers"></div>'+
            '</div></div>';
            return htmls;
        }
        if(type == 2){ // nhieu cau hoi , nhieu dap an
            var htmls = '<div class="col-6">'+
                '<div class="lb-item-questions">'+formTemplate.newQuestionForm(id,type)+'</div>'+
            '</div>'+
            '<div class="col-6">'+
                '<div class="lb-item-answers">'+
                    
                '</div>'+
            '</div>';
            return htmls;
        }
    },
    newQuestionForm :function(id,type){
        return '<div id="question_id" class="lb-item-question">'+
                '<form name="question_id" enctype="mutilpart/form-data">'+
                    '<input type="hidden" name="ID" value="">'+
                    '<input type="hidden" name="action" value="'+urlQuestion.CreateOrUpdate+'">'+
                    '<input type="hidden" name="UserID" value="'+userID+'">'+
                    '<input type="hidden" name="ClientID" value="'+clientID+'">'+
                    '<input type="hidden" name="LessonPartID" value="'+id+'">'+
                    '<input type="hidden" name="TemplateType" value="'+type+'">'+
                    '<div class="question-header row">'+
                        '<div class="col-10">'+
                            '<input type="text" class="form-control" name="Title" placeholder=" Ex : Câu hỏi số 1 : ">'+
                        '</div>'+
                        '<div class="col-2">'+
                            '<input type="number" class="form-control" name="Point" placeholder="Điểm">'+
                        '</div>'+
                    '</div>'+formTemplate.newFuncCancelDone()+'</form></div>';
    }
    ,
    newAnswerForm:function(id){
        return '<div id="anwser_id" class="lb-item-answer">'+
                    '<form name="anwser_id" enctype="mutilpart/form-data">'+
                        '<input type="hidden" name="ID" value="">'+
                        '<input type="hidden" name="action" value="'+urlQuestionMedia.CreateOrUpdate+'">'+
                        '<input type="hidden" name="UserID" value="'+userID+'">'+
                        '<input type="hidden" name="ClientID" value="'+clientID+'">'+
                        '<input type="hidden" name="LessonPartID" value="'+id+'">'+
                        '<input type="hidden" name="TemplateType" value="'+type+'">'+
                        '<div class="question-header row">'+
                           ' <div class="col-12">'+
                                '<input type="text" class="form-control" name="Title" placeholder=" Ex : Câu trả lời ">'+
                            '</div>'+
                            
                            '<div class="col-6">'+
                                '<select name="QuestionID" class="form-control">'+
                                    '<option value="">----------- Chọn câu hỏi -----------</option>'+
                                '</select>'+
                            '</div>'+
                            '<div class="col-6">'+
                                '<div class="form-check" style="margin-top: 15px;">'+
                                    '<label class="form-check-label">'+
                                        '<input class="form-check-input" type="checkbox" name="IsAnswer"> Đây là câu trả lời '
                                        '<span class="form-check-sign">'+
                                            '<span class="check"></span>'+
                                        '</span>'+
                                    '</label>'+
                                '</div>'+
                            '</div>'+
                        '</div>'+formTemplate.newFuncCancelDone()+'</form></div>';
    },
    newFuncCancelDone:function(){
        return '<div class="fn-lesson-btn">'+
                    '<a href="#" class="btn btn-sm btn-danger btn-fab text-light" title="Hủy" onclick="func.Cancel(this)"><i class="material-icons">cancel</i></a>'+
                    '<a href="#" class="btn btn-sm btn-success btn-fab text-light" title="Lưu" onclick="func.Done(this)"><i class="material-icons">done</i></a>'+
                '</div>';
    },
    newFuncDeleteEdit:function(){
        return '<div class="fn-lesson-btn">'+
                    '<a href="#" class="btn btn-sm btn-danger btn-fab text-light" title="Hủy" onclick="func.Edit(this)"><i class="material-icons">border_color</i></a>'+
                    '<a href="#" class="btn btn-sm btn-success btn-fab text-light" title="Lưu" onclick="func.Delete(this)"><i class="material-icons">delete_sweep</i></a>'+
                '</div>';
    }
};
var html = {
    showMedia:function(data){
        if(data == []) return;
        var obj = data[0];
        if(obj == null) return;
        var arr = obj.file.split('/');
        var container = document.getElementById(obj.lessonPartID);
        if(container != null){
            if(arr.indexOf("IMG")>-1){
                var img = container.querySelector(".image-preview > img");
                img.src = obj.file;
            }else{
                if(arr.indexOf("VIDEO")>-1){
                    var video = container.querySelector(".video-preview>iframe");
                    video.src=obj.file; // sau chuyeenr sang api stream
                }else {
                    if(arr.indexOf("AUDIO")>-1){
                        var audio = container.querySelector(".audio-preview source");
                        audio.src = obj.file;
                    }
                }
            }
        }
    },
    newItemLesson:function(type){
        if(type == 1){
           var str = '<div class="row"><div class="lesson-header-extends col-2">';
               str += '<div class="timer">'; 
               str += '<input class="form-control" type="number" name="Timer" placeholder="thời gian làm bài"/>'
               str += '</div><div class="pointer">';
               str += '<input class="form-control" type="number" name="Point" placeholder="điểm làm bài"/>';
               str +='</div></div>';
               str+= '<div class="lesson-header-title col-10"><input type="text" class="form-control" name="Title" placeholder="Tiêu đề của lesson biên tập"/></div></div>';
            return str;
        }else{
            var str = '<div class="row">';
                str+= '<div class="lesson-header-title col-12"><input type="text" class="form-control" name="Title" placeholder="Tiêu đề của lesson biên tập"/></div></div>';
            return str;
        }
    },
    newLesson : function(type,ID){
        var box = document.createElement("div");
        box.classList ="lesson lesson-box p-fixed";
        var container = document.createElement("div");//<div class="lesson-container" id="lesson_id">
        container.classList = "lesson-container";
        container.id = ID == void 0 ? "lesson_id" : ID;
        var lessonHeader = document.createElement("div");
        lessonHeader.classList = "lesson-header";
        box.appendChild(container);
        container.appendChild(lessonHeader);

        // tạo form
        var form = html.form(ID,container.id);
        var Course = document.querySelector("input[sid='CourseID']");
        var Chapter = document.querySelector("input[sid='ChapterID']");
        form.innerHTML +='<input type="hidden" name="action" value="'+urlLesson.CreateOrUpdate+'">';
        form.innerHTML += '<input type="hidden" name="ChapterID" value="'+Chapter.value+'">';
        form.innerHTML += '<input type="hidden" name="CourseID" value="'+Course.value+'">';
        form.innerHTML += html.newItemLesson(type);
        formCreate.extendsFunc(form,0);
        lessonHeader.appendChild(form);
        return box;
    },
    form : function(ID,name){
        var _id = ID == void 0 ? "":ID;
        var form = document.createElement("form");
            form.name = name;
            form.setAttribute("enctype","multipart/form-data");//enctype="multipart/form-data"
            form.innerHTML = '<input type="hidden" name="ID" value="'+_id+'">';
            form.innerHTML += '<input type="hidden" name="UserID" value="'+userID+'">';
            form.innerHTML += '<input type="hidden" name="ClientID" value="'+clientID+'">';
        return form;
    },
    ul_type : function(data,LessonID,type){
        var html = '<ul class="template_type">';
        var count = data == null ? 0 : data.length;
        var style =  count > 2 ? 'style="width:calc(100%/3)"' : 'style="width:calc(100%/'+count+')"';
        for (var i = 0; i < count; i++) {
            if(LessonID != void 0 && type != void 0){
                html += '<li data-lesson-id="'+LessonID+'" data-type="'+type+'" class="template_type_item" '+style+' onclick="' + data[i].Func + '">';
            }
            else{
                html += '<li class="template_type_item" '+style+' onclick="' + data[i].Func + '">';
            }
            html += '<span>' + data[i].DisplayName + '</span>';
            html += '<img src="' + data[i].Icon + '" alt="' + data[i].DisplayName + '"></li>';
        }
        html += "</ul>";
        return html;
    },
    checkbox :function(item){
        var str = '<div class="form-check">';
        str += '<label class="form-check-label">';
        if(item.Checked){
            str += '<input class="'+item.Class+'" onclick="'+item.Func+'" type="checkbox" name="'+item.Name+'" id="'+item.ID+'" value="'+item.Value+'" checked>';
        }else{
            str += '<input class="'+item.Class+'" onclick="'+item.Func+'" type="checkbox" name="'+item.Name+'" id="'+item.ID+'" value="'+item.Value+'">';
        }
        str += item.DisplayName;
        str += '<span class="circle"><span class="check"></span></span></label></div>';
        return str;
    },
    radio : function(item){
        var str = '<div class="form-check form-check-radio">';
        str += '<label class="form-check-label">';
        if(item.Checked){
            str += '<input class="'+item.Class+'" onclick="'+item.Func+'" type="radio" name="'+item.Name+'" id="'+item.ID+'" value="'+item.Value+'" checked>';
        }else{
            str += '<input class="'+item.Class+'" onclick="'+item.Func+'" type="radio" name="'+item.Name+'" id="'+item.ID+'" value="'+item.Value+'">';
        }
        str += item.DisplayName;
        str += '<span class="circle"><span class="check"></span></span></label></div>';
        return str;
    },
    inputHidden : function(item){
        return '<input type="hidden" name="'+item.Name+'" id="'+item.ID+'" value="'+item.Value+'">';
    },
    currentLesson : function(data){
        var box = document.createElement("div");
        box.classList ="lesson lesson-box p-fixed";
        var container = document.createElement("div");//<div class="lesson-container" id="lesson_id">
        container.classList = "lesson-container";
        container.id = data.id;
        var lessonHeader = document.createElement("div");
        lessonHeader.classList = "lesson-header";
        box.appendChild(container);
        container.appendChild(lessonHeader);

        // tạo form
        var form = html.form(data.id,data.id);
        form.classList = "complete";
        var Course = document.querySelector("input[sid='CourseID']");
        var Chapter = document.querySelector("input[sid='ChapterID']");
        form.innerHTML +='<input type="hidden" name="action" value="'+urlLesson.CreateOrUpdate+'">';
        form.innerHTML += '<input type="hidden" name="ChapterID" value="'+data.chapterID+'">';
        form.innerHTML += '<input type="hidden" name="CourseID" value="'+data.courseID+'">';
        var col2="",col10="";
        if(data.timer > 0 || data.point > 0){
            var strTimerPoint = '';
            if(data.timer > 0){
                strTimerPoint +='<div class="timer"><input class="form-control" type="number" value="'+data.timer+'" name="Timer" placeholder="thời gian làm bài"/><span>'+data.timer+':00 phút</span></div>';
            }
            if(data.point > 0){
                strTimerPoint +='<div class="pointer"><input class="form-control" value="'+data.point+'" type="number" name="Point" placeholder="điểm làm bài"/><span>'+data.point+' điểm</span></div>';
            }
            col2 ='<div class="lesson-header-extends col-2">'+strTimerPoint+'</div>';
            col10 ='<div class="lesson-header-title col-10"><h4>'+data.title+'</h4><input type="text" class="form-control" value="'+data.title+'" name="Title" placeholder="Tiêu đề của lesson biên tập"/></div>';

        }else{
            col10 = '<div class="lesson-header-title col-12"><h4>'+data.title+'</h4><input type="text" class="form-control" value="'+data.title+'" name="Title" placeholder="Tiêu đề của lesson biên tập"/></div>';
        }
        form.innerHTML +='<div class="row">'+col2+col10+'</div>';
        formCreate.extendsFunc(form,1);
        lessonHeader.appendChild(form);
        var lessonBody = document.createElement("div");
            lessonBody.classList ="lesson-body";
            var lessonItem = document.createElement("div");
            lessonItem.classList ="lesson-body-item";
            lessonBody.appendChild(lessonItem);
            var fnLessonPart = document.createElement("div");
            fnLessonPart.classList = "fn-lesson-add-part";
            fnLessonPart.innerHTML = '<a href="#" class="btn btn-sm btn-success text-light" title="Thêm part mới" onclick="formCreate.lessonPart('+"'"+data.id+"'"+',0)"><i class="material-icons">add</i> Thêm nội dung bài giảng</a><a href="#" class="btn btn-sm btn-success text-light" title="Thêm part mới" onclick="formCreate.lessonPart('+"'"+data.id+"'"+',1)"><i class="material-icons">add</i> Thêm nội dung bài tập</a>';
            lessonBody.appendChild(fnLessonPart);
        container.appendChild(lessonBody);
        return box;
    },
    newLessonPart:function(TypeTemplate,LessonID,TypePart){
        var header = "",content="",isExample = false,media = null;
        if(TypePart == 1){
            isExample = true;
            header = '<div class="lb-item-header-title col-10">'+
                        '<input type="text" class="form-control" name="Title" placeholder=" Ex : Câu 1 : Hãy hoàn thành các câu hỏi sau : ">'+
                    '</div>'+
                    '<div class="lb-item-header-extends col-2">'+
                         '<div class="timer">'+
                            '<input class="form-control" type="number" name="Timer" value="" placeholder="thời gian làm bài"/>'+
                        '</div>'+
                        '<div class="pointer">'+
                            '<input class="form-control" type="number" name="Point" value="" placeholder="điểm làm bài"/>'+
                        '</div>'+
                    '</div>';
        }else{
            isExample = false;
            header = '<div class="lb-item-header-title col-12">'+
                        '<input type="text" class="form-control" name="Title" placeholder=" Ex : Câu 1 : Hãy hoàn thành các câu hỏi sau : ">'+
                    '</div>';
        }
        if(TypeTemplate == 1){ //text
            content = '<div class="row">'+
                        '<div class="col-12">'+
                            '<textarea row=1 class="form-control" name="Content" placeholder=" Ex : Nội dung câu hỏi được mô tả "></textarea>'+
                        '</div>'+
                    '</div>';
        }else {
            if(TypeTemplate == 2){ //img 1/3
   
                var imgSrc = "https://cdn3.iconfinder.com/data/icons/web-document-icons/512/Upload_Document-512.png";
                if(media != null && media.length > 0){
                    imgSrc = media[0].file;
                }             content = '<div class="row">'+
                        '<div class="col-4">'+
                            '<div class="avatar">'+
                              '<div class="image-preview" onclick="loadFiles(this)">'+
                                '<img src="'+imgSrc+'" alt="">'+
                              '</div>'+
                              '<input type="file" id="file" name="files" style="display: none;" accept="image/x-png,image/gif,image/jpeg">'+
                            '</div>'+
                        '</div>'+
                        '<div class="col-8">'+
                            '<textarea row=1 class="form-control" name="Content" placeholder=" Ex : Nội dung câu hỏi được mô tả "></textarea>'+
                        '</div>'+
                    '</div>';
            }else{
                if(TypeTemplate == 3){
   
                var imgSrc = "https://cdn3.iconfinder.com/data/icons/web-document-icons/512/Upload_Document-512.png";
                if(media != null && media.length > 0){
                    imgSrc = media[0].file;
                }                 content = '<div class="row">'+
                        '<div class="col-12">'+
                            '<div class="avatar">'+
                              '<div class="image-preview" onclick="loadFiles(this)">'+
                                '<img src="'+imgSrc+'" alt="">'+
                              '</div>'+
                              '<input type="file" id="file" name="files" style="display: none;" accept="image/x-png,image/gif,image/jpeg">'+
                            '</div>'+
                        '</div>'+
                        '<div class="col-12">'+
                            '<textarea row=1 class="form-control" name="Content" placeholder=" Ex : Nội dung câu hỏi được mô tả "></textarea>'+
                        '</div>'+
                    '</div>';
                }
            }
        }
        var box = document.createElement("div");
        box.classList = "lb-body-item-part";
        var html = '<form name="lessonpart_id" enctype="multipart/form-data">'+
                        '<input type="hidden" name="ID" value="">'+
                        '<input type="hidden" name="action" value="'+urlLessonPart.CreateOrUpdate+'">'+
                        '<input type="hidden" name="UserID" value="'+userID+'">'+
                        '<input type="hidden" name="ClientID" value="'+clientID+'">'+
                        '<input type="hidden" name="ParentID" value="'+LessonID+'">'+
                        '<input type="hidden" name="TemplateType" value="'+TypeTemplate+'">'+
                        '<input type="hidden" name="IsExample" value="'+isExample+'">'+
                        '<div class="lb-item-header row">{header}'+
                            '<div class="lb-item-header-content col-12">{content}'+
                            '</div>'+
                        '</div>'+
                    '</form>';
        box.innerHTML = html.replace("{header}",header).replace("{content}",content);
        return box;
    },
    currentLessonPart : function(obj){
        var data=obj;
        var media = null;
        var header = "",content="";
        if(data.isExample){
            header = '<div class="lb-item-header-title col-10">'+'<h4>'+data.title+'</h4>'+
                        '<input type="text" class="form-control" name="Title" placeholder=" Ex : Câu 1 : Hãy hoàn thành các câu hỏi sau : ">'+
                    '</div>'+
                    '<div class="lb-item-header-extends col-2">'+
                         '<div class="timer">'+'<span>'+data.timer + ":00 phút"+'</span>'+
                            '<input class="form-control" type="number" name="Timer" value="" placeholder="thời gian làm bài"/>'+
                        '</div>'+
                        '<div class="pointer">'+'<span>'+data.point + " điểm"+'</span>'+
                            '<input class="form-control" type="number" name="Point" value="" placeholder="điểm làm bài"/>'+
                        '</div>'+
                    '</div>';
        }else{
            header = '<div class="lb-item-header-title col-12">'+'<h4>'+data.title+'</h4>'+
                        '<input type="text" class="form-control" name="Title" placeholder=" Ex : Câu 1 : Hãy hoàn thành các câu hỏi sau : ">'+
                    '</div>';
        }
        if(data.content != '' || data.content != null){
            if(data.templateType == 1){ //text
                content = '<div class="row">'+
                            '<div class="col-12">'+data.content+
                                '<textarea row=1 class="form-control" name="Content" placeholder=" Ex : Nội dung câu hỏi được mô tả ">'+data.content+'</textarea>'+
                            '</div>'+
                        '</div>';
            }else {
                if(data.templateType == 2){ //img 1/3
                    var imgSrc = "https://cdn3.iconfinder.com/data/icons/web-document-icons/512/Upload_Document-512.png";
                    if(media != null && media.length > 0){
                        imgSrc = media[0].file;
                    }
                    content = '<div class="row">'+
                            '<div class="col-4">'+
                                '<div class="avatar">'+
                                  '<div class="image-preview" onclick="loadFiles(this)">'+
                                    '<img src="'+imgSrc+'" alt="">'+
                                  '</div>'+
                                  '<input type="file" id="file" name="files" style="display: none;" accept="image/x-png,image/gif,image/jpeg">'+
                                '</div>'+
                            '</div>'+
                            '<div class="col-8">'+data.content+
                                '<textarea row=1 class="form-control" name="Content" placeholder=" Ex : Nội dung câu hỏi được mô tả ">'+data.content+'</textarea>'+
                            '</div>'+
                        '</div>';
                }else{
                    if(data.templateType == 3){
                        var imgSrc = "https://cdn3.iconfinder.com/data/icons/web-document-icons/512/Upload_Document-512.png";
                        if(media != null && media.length > 0){
                            imgSrc = media[0].file;
                        }
                        content = '<div class="row">'+
                            '<div class="col-12">'+
                                '<div class="avatar">'+
                                  '<div class="image-preview" onclick="loadFiles(this)">'+
                                    '<img src="'+imgSrc+'" alt="">'+
                                  '</div>'+
                                  '<input type="file" id="file" name="files" style="display: none;" accept="image/x-png,image/gif,image/jpeg">'+
                                '</div>'+
                            '</div>'+
                            '<div class="col-12">'+data.content+
                                '<textarea row=1 class="form-control" name="Content" placeholder=" Ex : Nội dung câu hỏi được mô tả ">'+data.content+'</textarea>'+
                            '</div>'+
                        '</div>';
                    }
                }
            }
        }
        var box = document.createElement("div");
        box.classList = "lb-body-item-part";
        box.id = data.id;
        var html = '<form name="'+data.id+'" enctype="multipart/form-data" class="complete">'+
                        '<input type="hidden" name="ID" value="'+data.id+'">'+
                        '<input type="hidden" name="action" value="'+urlLessonPart.CreateOrUpdate+'">'+
                        '<input type="hidden" name="UserID" value="'+userID+'">'+
                        '<input type="hidden" name="ClientID" value="'+clientID+'">'+
                        '<input type="hidden" name="ParentID" value="'+data.parentID+'">'+
                        '<input type="hidden" name="TemplateType" value="'+data.templateType+'">'+
                        '<input type="hidden" name="IsExample" value="'+data.isExample+'">'+
                        '<div class="lb-item-header row">{header}'+
                            '<div class="lb-item-header-content col-12">{content}'+
                            '</div>'+
                        '</div>'+
                        '<div class="fn-lesson-btn">'+
                            '<a href="#" class="btn btn-sm btn-info btn-fab text-light" title="Sửa" onclick="func.Edit(this)"><i class="material-icons">border_color</i></a>'+
                            '<a href="#" class="btn btn-sm btn-fab btn-muted text-light" title="Xóa" onclick="func.Delete(this)"><i class="material-icons">delete_sweep</i></a>'+
                        '</div>'
                    '</form>';
        box.innerHTML = html.replace("{header}",header).replace("{content}",content);
        if(data.isExample){
            box.innerHTML += '<div class="lb-item-body row"><div class="col-12"><div class="fn-lb-item-body"><a href="#" class="btn btn-sm btn-success text-light" title="Thêm part mới" onclick="formCreate.Question('+"'"+data.id+"'"+')"><i class="material-icons">add</i> Thêm câu hỏi</a></div></div></div>';
        }

        load.loadMedia(data.id);

        return box;
    },
    currentQuestion:function(data){
        return '<div id="'+data.id+'" class="lb-item-question">'+
                '<form name="'+data.id+'" enctype="mutilpart/form-data" class="complete">'+
                    '<input type="hidden" name="ID" value="'+data.id+'">'+
                    '<input type="hidden" name="action" value="'+urlQuestion.CreateOrUpdate+'">'+
                    '<input type="hidden" name="UserID" value="'+userID+'">'+
                    '<input type="hidden" name="ClientID" value="'+clientID+'">'+
                    '<input type="hidden" name="LessonPartID" value="'+data.lessonPartID+'">'+
                    '<input type="hidden" name="TemplateType" value="'+data.templateType+'">'+
                    '<div class="question-header row">'+
                        '<div class="col-10"><h4>'+data.title+'</h4>'+
                            '<input type="text" class="form-control" name="Title" placeholder=" Ex : Câu hỏi số 1 : ">'+
                        '</div>'+
                        '<div class="col-2"><span>'+data.point+'</span>'+
                            '<input type="number" class="form-control" name="Point" placeholder="Điểm">'+
                        '</div>'+
                    '</div>'+formTemplate.newFuncDeleteEdit()+'</form></div>'
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
        modalForm.innerHTML = template.Type('lessonPart',lessonID,"ParentID");
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
        $("#action").val(urlQuestionMedia.CreateOrUpdate);
        GetCurrentUser();
    }
};
var render = {
    resetLesson: function () {
        containerLesson.innerHTML = "";
    },
    lesson: function (data,frm) {

        //console.log(data);
        if(frm == void 0){
            var templateLesson = document.getElementById("template-lesson");
            templateLesson.innerHTML ="";
            templateLesson.appendChild(html.currentLesson(data));
        }else{
            frm.name = data.id;
            var timer = frm.querySelector('input[name="Timer"]');
            if(timer != null){
                timer.classList = "hidden";
                timer.parentElement.innerHTML += '<span>'+data.timer + ":00 phút"+'</span>';
            }
            var pointer = frm.querySelector('input[name="Point"]');
            if(pointer != null){
                pointer.classList = "hidden";
                pointer.parentElement.innerHTML += '<span>'+data.point + " điểm"+'</span>';
            }

            var tile = '<h4>'+data.title+'</h4>';
            var header = frm.querySelector("input[name='Title']");
            if(header != null){
                header.parentElement.innerHTML += tile;
            }

            var fn = frm.querySelector('.fn-lesson-btn');
            var nodeRoot = fn.parentNode;
            nodeRoot.removeChild(fn);
            formCreate.extendsFunc(nodeRoot,1)

            var lessonBody = document.createElement("div");
            lessonBody.classList ="lesson-body";
            var lessonItem = document.createElement("div");
            lessonItem.classList ="lesson-body-item";
            lessonBody.appendChild(lessonItem);
            var fnLessonPart = document.createElement("div");
            fnLessonPart.classList = "fn-lesson-add-part";
            fnLessonPart.innerHTML = '<a href="#" class="btn btn-sm btn-success text-light" title="Thêm part mới" onclick="formCreate.lessonPart('+"'"+data.id+"'"+',0)"><i class="material-icons">add</i> Thêm nội dung bài giảng</a><a href="#" class="btn btn-sm btn-success text-light" title="Thêm part mới" onclick="formCreate.lessonPart('+"'"+data.id+"'"+',1)><i class="material-icons">add</i> Thêm nội dung bài tập</a>';

            lessonBody.appendChild(fnLessonPart);
            frm.parentElement.parentElement.appendChild(lessonBody);

        }
    },
    lessonPart: function (data) {
        for (var i = 0; data != null && i < data.length; i++) {
            var item = data[i];
            render.part(item);
        }
    },
    part: function (obj,frm) {
        var data = obj;
        if(frm == void 0){
            var container = document.getElementById(data.parentID);
            container.querySelector(".lesson-body>.lesson-body-item").prepend(html.currentLessonPart(data));
        }else{
            frm.name = data.id;
            frm.classList.add('complete');
            frm.parentElement.id=data.id;
            var timer =frm.querySelector('.timer');
            var pointer = frm.querySelector('.pointer');
            var title = frm.querySelector('.lb-item-header-title');
            if(timer != null){
                timer.innerHTML += '<span>'+data.timer + ":00 phút"+'</span>';
            }
            if(pointer != null){
                pointer.innerHTML += '<span>'+data.point + " điểm"+'</span>';
            }
            if(title == null){
                title.innerHTML += '<h4>'+data.title+'</h4>';
            }
            var contentxx = frm.querySelector('.lb-item-header-content > textarea');
            if(data.content != '' && data.content != null && contentxx != null ){
              contentxx.innerHTML = data.content;
            }
            var fn = frm.querySelector('.fn-lesson-btn');
            var nodeRoot = fn.parentNode;
            nodeRoot.removeChild(fn);
            formCreate.extendsFunc(nodeRoot,1);

            var lessonpartBody = document.createElement("div");
            lessonpartBody.classList ="lb-item-body row";
            var lessonPartItem = document.createElement("div");
            lessonPartItem.classList ="col-12";
            lessonpartBody.appendChild(lessonPartItem);
            var fnLessonPart = document.createElement("div");
            fnLessonPart.classList = "fn-lb-item-body";
            fnLessonPart.innerHTML = '<a href="#" class="btn btn-sm btn-success text-light" title="Thêm part mới" onclick="formCreate.Question('+"'"+data.id+"'"+')"><i class="material-icons">add</i> Thêm câu hỏi</a>';

            lessonPartItem.appendChild(fnLessonPart);
            frm.parentElement.appendChild(lessonpartBody);
        }
        load.listQuestion(obj.id);
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
    },
    question:function(data,frm){
        console.log(data);
        // created: "2019-05-26T22:06:41.8591133+07:00"
        // id: "5ceaab819e0eb428501c42f0"
        // lessonPartID: "5cea6ca79e0eb428501c42ed"
        // order: 2
        // point: 2
        // title: "cvbbvn"
        // updated: "2019-05-26T22:06:41.8590261+07:00"
        if(frm == void 0){
            var container = document.getElementById(data.lessonPartID);
            var str = "";
            if(data.type == 1){
                str = '<div class="question-item col-12"></div>';
                str += '<div class="answer-item col-12"></div>';
            }
            else{
                str = '<div class="question-item col-6"></div>';
                str += '<div class="answer-item col-6"></div>';
            }
            container.querySelector(".lb-item-body").innerHTML +=str;
            container.querySelector(".lb-item-body .question-item").innerHTML += html.currentQuestion(data);
        }
        else{

        }
    },
    listQuestion:function(data){
        for (var i = 0; data != null && i < data.length; i++) {
            var item = data[i];
            render.question(item);
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
        xhr.open('GET', url + urlQuestionMedia.List + "?LessonPartID=" + partID + "&UserID=" + userID + "&ClientID=" + clientID);
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
    },
    loadMedia:function(id){
        var xhr = new XMLHttpRequest();
        var url = urlBase;
        xhr.open('GET', url + urlMedia.List + "?ID=" + id + "&UserID=" + userID + "&ClientID=" + clientID);
        xhr.send({});
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var data = JSON.parse(xhr.responseText);
                if (data.code == 200) {
                    html.showMedia(data.data);
                }
            }
        }
    },
    listQuestion:function(id){
        var xhr = new XMLHttpRequest();
        var url = urlBase;
        xhr.open('GET', url + urlQuestion.List + "?LessonPartID=" + id + "&UserID=" + userID + "&ClientID=" + clientID);
        xhr.send({});
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var data = JSON.parse(xhr.responseText);
                if (data.code == 200) {
                    render.listQuestion(data.data);
                }
            }
        }
    }
};
var template = {
    defaults: function (type,parentID,parentName) {
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
                    "Function": "template.lessonPart('lessonPartBG',this)",
                    "Icon": "/Contents/img/Application-Engine-icon-150x150.png"
                },
                {
                    "Name": "Nội dung bài tập",
                    "Function": "template.lessonPart('lessonPartBT','"+parentID+"','"+parentName+"')",
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
        if (type == "lessonPartBG") {
            return [
                {
                    "Name": "Template 1",
                    "Function": "template.lessonPart(bg1)",
                    "Icon": "/Contents/img/Application-Engine-icon-150x150.png"
                },
                {
                    "Name": "Template 2",
                    "Function": "template.lessonPart(bg2)",
                    "Icon": "/Contents/img/Application-Engine-icon-150x150.png"
                },
                {
                    "Name": "Template 3",
                    "Function": "template.lessonPart(bg3)",
                    "Icon": "/Contents/img/Application-Engine-icon-150x150.png"
                },
                {
                    "Name": "Template 4",
                    "Function": "template.lessonPart(bg4)",
                    "Icon": "/Contents/img/Application-Engine-icon-150x150.png"
                },
            ];
        }
        if (type == "lessonPartBT") {
            return [
                {
                    "Name": "Template 1",
                    "Function": "template.lessonPartBT(1,'"+parentID+"','"+parentName+"')",
                    "Icon": "/Contents/img/1.jpg"
                },
                {
                    "Name": "Template 2",
                    "Function": "template.lessonPartBT(2,'"+parentID+"','"+parentName+"')",
                    "Icon": "/Contents/img/2.jpg"
                },
                {
                    "Name": "Template 3",
                    "Function": "template.lessonPartBT(3,'"+parentID+"','"+parentName+"')",
                    "Icon": "/Contents/img/3.jpg"
                },
                {
                    "Name": "Template 4",
                    "Function": "template.lessonPartBT(4,'"+parentID+"','"+parentName+"')",
                    "Icon": "/Contents/img/4.jpg"
                },
                {
                    "Name": "Template 5",
                    "Function": "template.lessonPartBT(5,'"+parentID+"','"+parentName+"')",
                    "Icon": "/Contents/img/5.jpg"
                },
                {
                    "Name": "Template 6",
                    "Function": "template.lessonPartBT(6,'"+parentID+"','"+parentName+"')",
                    "Icon": "/Contents/img/6.jpg"
                }
                // ,{
                //     "Name": "Template 7",
                //     "Function": "template.lessonPartBT(7,'"+parentID+"','"+parentName+"')",
                //     "Icon": "/Contents/img/Application-Engine-icon-150x150.png"
                // },
                // {
                //     "Name": "Template 8",
                //     "Function": "template.lessonPartBT(8,'"+parentID+"','"+parentName+"')",
                //     "Icon": "/Contents/img/Application-Engine-icon-150x150.png"
                // },
            ];
        }
        if (type == "bt1") {
            return [
                {
                    "Question": {
                        "Class": "col-12",

                    },
                    "Answer": [{

                    }]
                }
            ]
        }
    },
    Type: function (type,parentID,nameParentID) {
        var html = '<ul class="template_type">';
        var data = template.defaults(type,parentID,nameParentID);
        var count = data == null ? 0 : data.length;
        var style =  count > 2 ? 'style="width:calc(100%/3)"' : 'style="width:calc(100%/'+count+')"';
        for (var i = 0; i < count; i++) {
            html += '<li class="template_type_item" '+style+' onclick="' + data[i].Function + '">';
            html += '<span>' + data[i].Name + '</span>';
            html += '<img src="' + data[i].Icon + '" alt="' + data[i].Name + '"></li>';
        }
        html += "</ul>";
        return html;
    },
    loadTemplateHTML: function (data) {
        var modalForm = window.modalForm;
        var str = "";
        var item = data[0];
        modalTitle.html(item.Name);
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
    lessonPart: function (type,parentID,parentName) {
        var modalForm = window.modalForm;
        modalForm.innerHTML = template.Type(type,parentID,parentName);
        //modalForm += '<input type="hidden" name="TemplateType" value="' + type + '"/>';
        //$("#action").val(urlLessonPart.CreateOrUpdate);
    },
    lessonPartBT:function(template_type,parentID,parentName){
        var modalForm = window.modalForm;
        //modalForm.innerHTML = template.Type(type);
        template.loadTemplateHTML('bt'+template_type);
        modalForm += '<input type="hidden" name="TemplateType" value="' + template_type + '"/>';
        modalForm += '<input type="hidden" name="IsExample" value="true"/>';
        //$("#action").val(urlLessonPart.CreateOrUpdate);
        console.log(parentID,parentName);
    }
};

document.addEventListener("DOMContentLoaded", function () {

})