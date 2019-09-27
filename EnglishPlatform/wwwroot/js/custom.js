/* ----------  bootstrapTabControl  --------------- */

function tab_gonext() {
    //var i, 
    //    //items = $('#pills-tab .nav-item .nav-link'), 
    //    panes = $('.tab-pane');
    //for (i = 0; i < panes.length; i++) {
    //    if ($(panes[i]).hasClass('active') == true) {
    //        break;
    //    }
    //}
    stopAllMedia();

    var panes = $('.tab-pane');
    var index = panes.index($('.tab-pane.active'));

    if (index < panes.length - 1) {
        // for tab
        //$(items[i]).removeClass('active');
        //$(items[i + 1]).addClass('active');
        // for pane
        $(panes[index]).removeClass('show active');
        $(panes[index + 1]).addClass('show active');
    }
    //tab_scroll_active();
};

function tab_goback() {
    //var i, items = $('#pills-tab .nav-item .nav-link'), pane = $('.tab-pane');
    //for (i = 0; i < items.length; i++) {
    //    if ($(items[i]).hasClass('active') == true) {
    //        break;
    //    }
    //}
    stopAllMedia();

    var panes = $('.tab-pane');
    var index = panes.index($('.tab-pane.active'));

    if (index > 0) {
        // for tab
        //$(items[i]).removeClass('active');
        //$(items[i + 1]).addClass('active');
        // for pane
        $(panes[index]).removeClass('show active');
        $(panes[index - 1]).addClass('show active');
    }


    //if (i != 0) {
    //    // for tab
    //    $(items[i]).removeClass('active');
    //    $(items[i - 1]).addClass('active');
    //    // for pane
    //    $(pane[i]).removeClass('show active');
    //    $(pane[i - 1]).addClass('show active');
    //}
    //tab_scroll_active();
};

function tab_go(part) {
    if ($('#pills-' + part).hasClass('active')) {
        return false;
    }
    $('.tab-pane.active').removeClass('active show');
    $('.nav-link.active').removeClass('active');
    $('#pills-' + part).removeClass('active');
    $('#pills-' + part).addClass('active');
    $('#pills-part-' + part).addClass('active show');
}

function tab_scroll_active() {
    var firsttab = $('#pills-tab .nav-item:eq(0)');
    var activetab = $('#pills-tab .nav-item .active').parent();
    $('#pills-tab').scrollTop(activetab.offset().top - firsttab.offset().top)
}

function tab_set_active(obj) {
    var r = setTimeout(function () {
        var firsttab = $('#pills-tab .nav-item:eq(0)');
        var activetab = $(obj).parent();
        $('#pills-tab').scrollTop(activetab.offset().top - firsttab.offset().top);
    }, 50);
}

function goNav(obj) {
    var question = $('#' + obj);
    var part = $(question).attr('data-part-id');
    //var firsttab = $('#pills-tab .nav-item:eq(0)');
    //var activetab = $('#pills-' + part);
    //$('#pills-tab').scrollTop(activetab.offset().top - firsttab.offset().top);
    tab_go(part);
    $(question).find('input:eq(0)').focus();
}

function toggle_tab_compact() {
    $('#pills-tab').toggleClass("compact");
}

/* check all input */
function toggleAll(obj, wrapperid) {
    var wrapper = $('#' + wrapperid);
    var ischecked = $(obj).prop("checked");
    $(wrapper).find('[name=cid]').prop("checked", ischecked);
}

function setToggle(wrapperid) {
    var wrapper = $('#' + wrapperid);
    $(wrapper).find('input[type=checkbox]:first').prop("checked", $(wrapper).find("[name=cid]:not(:checked)").length === 0);
}

function start(obj) {
    $(obj).parent().removeClass("d-flex").hide();
    $("#check-student").removeClass("d-none");
    $("#finish").removeClass("d-none");
    countdown();
}

function countdown() {
    clearTimeout(r);
    var time = $(".time-counter").text().trim();
    var minutes = parseInt(time.split(":")[0]);
    var second = parseInt(time.split(":")[1]);
    if (second > 0) {
        second--;
    }
    else {
        if (minutes > 0) {
            minutes--;
            second = 59;
        }
        else {
            endtime();
            return;
        }
    }
    var text = (minutes < 10 ? ("0" + minutes) : minutes) + ":" + (second < 10 ? ("0" + second) : second);
    $(".time-counter").text(text);
    localStorage.setItem("Timer", text);
    var r = setTimeout(function () {
        countdown();
    }, 1000);
}

function endtime() {
    //document.location = 'Lesson';
    //alert("Thời gian làm bài đã kết thúc! Cảm ơn bạn");
}

function togglePanelWidth(obj) {
    var parent = $(obj).parent();
    if (parent.hasClass("col-md-6")) {
        parent.removeClass("col-md-6").addClass("col-md-2");
        parent.siblings().removeClass("col-md-6").addClass("col-md-10");
    }
    else {
        if (parent.hasClass("col-md-4")) {
            parent.removeClass("col-md-4").addClass("col-md-6");
            parent.siblings().removeClass("col-md-8").addClass("col-md-6");
        }
        else {
            parent.removeClass("col-md-2").addClass("col-md-4");
            parent.siblings().removeClass("col-md-10").addClass("col-md-8");
        }
    }
}

/* tooltip */
$(document).ready(function () {
    $('.btn').tooltip({ trigger: 'hover' });
    //$('.lazy').Lazy();
});

/* Search */
$(document).ready(function () {

    $("#Search-form").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $(".card.shadow *").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });

    $("#searchText").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $(".card.shadow *").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });

});
