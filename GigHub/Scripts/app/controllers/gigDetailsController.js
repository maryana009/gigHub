var GigsDetailsController = function (followingService) {
    //then in done method we can access to it 
    var followingButton;

    var init = function (container) {

        //if we have 10 objects with class, we'll have 10 toggleAttendance methods in memory,
        // and if we load more later, to them the method won't apply
        $(".js-toggle-follow").click(toggleFollowing);

        //this solves all above problems, and we have only one toggleAttendance in memory
        //$(container).on("click", ".js-toggle-follow", toggleAttendance);
    };

    var toggleFollowing = function (e) {
        followingButton = $(e.target);

        var userId = followingButton.attr("data-user-id");

        if (followingButton.hasClass("btn-default"))
            followingService.createFollowing(userId, done, fail);
        else 
            followingService.deleteFollowing(userId, done, fail);

    }

    var fail = function () {
        alert("Something failed");
    }

    var done = function () {
        var text = (followingButton.text() === "Following") ? "Follow?" : "Following";

        followingButton.toggleClass("btn-info").toggleClass("btn-default").text(text);
    }
    return {
        init: init
    }
}(FollowingService);