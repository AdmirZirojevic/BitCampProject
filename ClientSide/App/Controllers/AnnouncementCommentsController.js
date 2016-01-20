(function () {

    var mod = angular.module("AnnouncementCommentMod", []);

    mod.controller("announcementCommentController", ["announcementCommentsService", "$stateParams", function (announcementCommentsService, $stateParams) {
        var ctrl = this;
        ctrl.id = $stateParams.id;
        ctrl.commentId = $stateParams.commentId;
        ctrl.applicationUser = $stateParams.applicationUser;
        ctrl.content = $stateParams.content;
        ctrl.title = $stateParams.title;
        ctrl.type = $stateParams.type;
        ctrl.getPost2 = getPost2;
        ctrl.postComment = postComment;
        ctrl.service = announcementCommentsService;

        var loggedUser = sessionStorage.getItem("userName");
        ctrl.loggedUser = loggedUser;

        getPost2();

        ctrl.editComment = editComment;
        ctrl.deleteComment = deleteComment;

        ctrl.comment = {
            announceId: $stateParams.id
        }

        //function call from injected service
        function getPost2() {
            ctrl.service.getPost2(ctrl.id);
        };

        //function call from injected service
        function postComment() {
            ctrl.service.postComment(ctrl.comment);
            


        };

        //function call from injected service
        function editComment(id, userId, userName,userEmail) {

            ctrl.comment.id = id;
            ctrl.comment.userid = userId;
            ctrl.comment.username = userName;
            ctrl.comment.useremail = userEmail;

            ctrl.service.editComment(ctrl.comment);

            getPost2();

        };

        //function call from injected service
        function deleteComment(id) {

            ctrl.comment.id = id;
            ctrl.service.deleteComment(ctrl.comment);

        };

    }]);

})();