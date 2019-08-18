﻿/* ----------  bootstrapTabControl  --------------- */

function tab_gonext() {
    var i, items = $('#pills-tab .nav-item .nav-link'), pane = $('.tab-pane');
    for (i = 0; i < items.length; i++) {
        if ($(items[i]).hasClass('active') == true) {
            break;
        }
    }
    if (i < items.length - 1) {
        // for tab
        $(items[i]).removeClass('active');
        $(items[i + 1]).addClass('active');
        // for pane
        $(pane[i]).removeClass('show active');
        $(pane[i + 1]).addClass('show active');
    }
    tab_scroll_active();
};

function tab_goback() {
    var i, items = $('#pills-tab .nav-item .nav-link'), pane = $('.tab-pane');
    for (i = 0; i < items.length; i++) {
        if ($(items[i]).hasClass('active') == true) {
            break;
        }
    }
    if (i != 0) {
        // for tab
        $(items[i]).removeClass('active');
        $(items[i - 1]).addClass('active');
        // for pane
        $(pane[i]).removeClass('show active');
        $(pane[i - 1]).addClass('show active');
    }
    tab_scroll_active();
};

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
