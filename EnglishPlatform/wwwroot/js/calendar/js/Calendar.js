"use strict";
var EduCalendar = (function(){
    var config = {
        container_id:'schedule'
    }
    function EduCalendar(){
        this.schedule = new CalendarLib.Schedule();
        this.onReady = DocReady;
        this.loadConfig = loadConfig;
        this.config = config;
        this.renderCalendar = RenderCalendar;
        this.useEduTemplate = UseEduTemplate;
    }
    var groupConfig = function(options){
        if (options == null || typeof (options) == "undefined") return config;
        for (var key in options)
            if (options.hasOwnProperty(key)) config[key] = options[key];
        return config;
    }
    var loadConfig = function(cusOptions){
        this.config = groupConfig(cusOptions);
        return this;
    }
    var RenderCalendar = function(){
        this.schedule.onLoad(this.config);
        if(this.config.useTemplate == true){
           renderHeader(this.schedule);
        }
    }
    var renderHeader = function(calendar){
        var root = document.getElementById(config.container_id);
        var timeOut;
        if(root != null){
            if(timeOut != void 0) clearTimeout(timeOut);
            var selectMonth = root.querySelector(".fc-selectMonth-button");
            var selectYear = root.querySelector(".fc-selectYear-button");
            if(selectMonth != null && selectYear != null){

                root.querySelector(".fc-now-button").setAttribute('disabled',true);

                selectMonth.innerHTML = renderMonth(calendar);
                selectYear.innerHTML = renderYear(calendar);
                selectMonth.querySelector("select").onchange = function(){
                    gotoDate(calendar);
                }
                selectYear.querySelector("select").onchange = function(){
                    gotoDate(calendar);
                }
            }else{
                timeOut = setTimeout(function(){
                    renderHeader(calendar);
                },100);
            }
        }
    }
    var renderMonth = function(calendar){
        var html = "<select>";
        if(calendar != null){
            for(var i = 0; i < 12;i++){
                var selected = "";
                var currentMonth = "";
                if(i == calendar.EduCalendar.getDate().getMonth()){
                    selected = "selected";
                }
                if(i == calendar.EduCalendar.getNow().getMonth()){
                    currentMonth = " *";
                }
                html += `<option ${selected} value="${i}">tháng ${i+1}${currentMonth}</option>`;
            }
        }
        html += "</select>";
        return html;
    }
    var renderYear = function(calendar){
        var html = "<select>";
        if(calendar != null){
            var now = new Date().getFullYear();
            for(var i = -10; i < 10;i++){
                var selected = "";
                var currentYear = "";
                if((i+now) == calendar.EduCalendar.getDate().getFullYear()){
                    selected = "selected";
                }
                if((i+now) == calendar.EduCalendar.getNow().getFullYear()){
                    currentYear = " *";
                }
                html += `<option ${selected} value="${i+now}">năm ${i+now}${currentYear}</option>`;
            }
        }
        html += "</select>";
        return html;
    }
    var UseEduTemplate = function(){
        var _self = this;
        var optionsTemplate = {
            useTemplate:true,
            customButtons:{
                btnAddEvent:{
                    text: "Thêm sự kiện",
                    click:function(){
                        addEvent(_self);
                    }
                },
                selectMonth:{
                    text:(new Date().getMonth()+1),
                    icon: "icon",
                    themeIcon: "themeIcon",
                    bootstrapFontAwesome: "bootstrapFontAwesome",
                    click:function(){}
                },
                selectYear:{
                    text:(new Date().getFullYear()),
                    icon: "",
                    themeIcon: "",
                    bootstrapFontAwesome: "",
                    click:function(){}
                },
                prevTime:{
                    text:"<",
                    //icon: "right-single-arrow",
                    click:function(){
                        prev(_self)
                    }
                },
                nextTime:{
                    text:">",
                    //icon: "right-single-arrow",
                    click:function(){
                        next(_self);
                    }
                },
                now:{
                    text:'Hôm nay',
                    icon: "",
                    themeIcon: "",
                    bootstrapFontAwesome: "",
                    click:function(){
                        today(_self);
                    }
                }
            },
            header:{
                left:"prevTime,selectMonth,selectYear,nextTime,now",
                right:"btnAddEvent"
            }
        }
        this.config = groupConfig(optionsTemplate);
        return this;
    }
    var today = function(self){
        self.schedule.EduCalendar.today();
        var root = document.getElementById(self.config.container_id);
        var selectMonth = root.querySelector(".fc-selectMonth-button>select");
        var selectYear = root.querySelector(".fc-selectYear-button>select");
        root.querySelector(".fc-now-button").setAttribute("disabled",true);
        if(selectMonth != null && selectYear != null){
            var currentTime = self.schedule.EduCalendar.getDate();
            selectMonth.value = currentTime.getMonth();
            selectYear.value = currentTime.getFullYear();
        }
    }
    var gotoDate = function(calendar){
        var root = calendar.EduCalendar.el;
        var selectMonth = root.querySelector(".fc-selectMonth-button>select");
        var selectYear = root.querySelector(".fc-selectYear-button>select");
        var date = new Date(selectYear.value,selectMonth.value,1);
        calendar.EduCalendar.gotoDate(date);

        var currentDate = calendar.EduCalendar.getDate();
        var now = calendar.EduCalendar.getNow();
        if(currentDate.getMonth() == now.getMonth() && currentDate.getFullYear() == now.getFullYear()){
            root.querySelector(".fc-now-button").setAttribute("disabled",true);
        }
        else{
            root.querySelector(".fc-now-button").removeAttribute("disabled");
        }
    }
    var prev = function(self){
        var calendar = self.schedule.EduCalendar;
        calendar.prev();
        var currentDate = calendar.getDate();
        var now = calendar.getNow();
        var root = calendar.el;
        var selectMonth = root.querySelector(".fc-selectMonth-button>select");
        var selectYear = root.querySelector(".fc-selectYear-button>select");
        if(selectMonth != null && selectYear != null){
            selectMonth.value = currentDate.getMonth();
            selectYear.value = currentDate.getFullYear();
        }
        if(currentDate.getMonth() == now.getMonth() && currentDate.getFullYear() == now.getFullYear()){
            root.querySelector(".fc-now-button").setAttribute("disabled",true);
        }
        else{
            root.querySelector(".fc-now-button").removeAttribute("disabled");
        }
    }
    var next = function(self){
        var calendar = self.schedule.EduCalendar;
        calendar.next();
        var currentDate = calendar.getDate();
        var now = calendar.getNow();
        var root = calendar.el;
        var selectMonth = root.querySelector(".fc-selectMonth-button>select");
        var selectYear = root.querySelector(".fc-selectYear-button>select");
        if(selectMonth != null && selectYear != null){
            selectMonth.value = currentDate.getMonth();
            selectYear.value = currentDate.getFullYear();
        }
        if(currentDate.getMonth() == now.getMonth() && currentDate.getFullYear() == now.getFullYear()){
            root.querySelector(".fc-now-button").setAttribute("disabled",true);
        }
        else{
            root.querySelector(".fc-now-button").removeAttribute("disabled");
        }
    }
    var addEvent = function(self,info){
        var darkbox = document.getElementById("dark-smooke");
        if(darkbox == null){
            darkbox = document.createElement("div");
            darkbox.id = "dark-smooke";
            darkbox.classList = "dark-smooke";
            document.body.appendChild(darkbox);
        }
        darkbox.onclick = function(){
            document.body.classList.remove("open-add-event");
        }
        document.body.classList.add("open-add-event");
        if(info != void 0 && info != null){
            console.log(info);
        }
    }
    var createFormAddEvent = function(){
        
    }
    var viewEvent = function(){

    }
    var editEvent = function(){

    }
    var removeEvent = function(){

    }

    var DocReady = function(t){
        document.addEventListener("DOMContentLoaded",t,!0);
    }
    return EduCalendar;
}())