(function () {

    var mod = angular.module("PostsViewMod", []);

    mod.controller("postsViewController", ["postsViewService", "$stateParams", function (postsViewService, $stateParams) {
        var ctrl = this;
        ctrl.id = $stateParams.id;
        ctrl.commentId = $stateParams.commentId;
        ctrl.applicationUser = $stateParams.applicationUser;
        ctrl.content = $stateParams.content;
        ctrl.title = $stateParams.title;
        ctrl.type = $stateParams.type;
        ctrl.getPost = getPost;
        ctrl.postComment = postComment;
        ctrl.service = postsViewService;

        var loggedUser = sessionStorage.getItem("userName");
        ctrl.loggedUser = loggedUser;


        getPost();

        ctrl.editComment = editComment;
        ctrl.deleteComment = deleteComment;

        ctrl.comment = {
            assignId: $stateParams.id,
 
        }

        //function call from injected service
        function getPost() {
            ctrl.service.getPost(ctrl.id); 
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

            getPost();

        };

        //function call from injected service
        function deleteComment(id) {

            ctrl.comment.id = id;
            ctrl.service.deleteComment(ctrl.comment);

            

        };

    }]);

})();