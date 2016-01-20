(function () {

    var mod = angular.module("commentsCtrl", ["commentsService"]);

    mod.controller("CommentsController", ["CommentsService", function (CommentsService) {

        var self = this;
        self.commentsService = CommentsService;

    }]);

})();