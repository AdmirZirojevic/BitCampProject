(function () {

    var mod = angular.module("commentsService", ["ngResource"]);

    mod.factory("CommentsFactory", ["$resource", function ($resource) {
        return $resource("/Home/FindPost/:id");
    }]);


    mod.service("CommentsService", ["CommentsFactory", function (CommentsFactory) {

        var self = this;
        self.listOfComments = [];
        self.getComment = getComment;

        function getComment(index) {
            var commentId = self.listOfComments[index].Id;
            self.listOfComments[index] = CommentsFactory.get({ Id: commentId });
        }


        return self;
    }]);

})();