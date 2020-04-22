var CalendarSideBar = (function () {
    var _url;
    function CalendarSideBar(url) {
        _url = url;
    }
    var loadData = function (startDate, endDate) {

        if (startDate == void 0) {
            startDate = new Date();
            var currentDay = startDate.getDay();
            var numberDay = 0;
            switch (currentDay) {
                case 1:
                    numberDay = 0;
                    break;
                case 2:
                    numberDay = 1;
                    break;
                case 3:
                    numberDay = 2;
                    break;
                case 4:
                    numberDay = 3;
                    break;
                case 5:
                    numberDay = 4;
                    break;
                case 6:
                    numberDay = 5
                    break;
                default:
                    numberDay = -1;
                    break;
            }
        }

        _ajax.proccess("GET", `$url?start${startDate}&end=${endDate}`, {}).then(function (data) {
            console.log(data);
        })
    }
    return CalendarSideBar;
}())