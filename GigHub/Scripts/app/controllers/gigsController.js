var GigsController = function (attendanceService) {
    //then in done method we can access to it 
    var button;

    var init = function (container) {

        //if we have 10 objects with class, we'll have 10 toggleAttendance methods in memory,
        // and if we load more later, to them the method won't apply
        //$(".js-toggle-attendance").click(toggleAttendance);

        //this solves all above problems, and we have only one toggleAttendance in memory
        $(container).on("click", ".js-toggle-attendance", toggleAttendance);
    };

    var toggleAttendance = function (e) {
        button = $(e.target);

        var gigId = button.attr("data-gig-id");

        if (button.hasClass("btn-default"))
            attendanceService.createAttendance(gigId, done, fail);
        else
            attendanceService.deleteAttendance(gigId, done, fail);
    }

    var fail = function () {
        alert("Something failed");
    }

    var done = function () {
        var text = (button.text() == "Going") ? "Going?" : "Going";

        button.toggleClass("btn-info").toggleClass("btn-default").text(text);
    }
    return {
        init: init
    }
}(AttendanceService);
