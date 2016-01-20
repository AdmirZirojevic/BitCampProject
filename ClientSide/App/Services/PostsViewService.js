(function () {

    var mod = angular.module("PostsViewMod");

    mod.service("postsViewService", ["$http", function ($http) {
        var service = this;
        service.id = "";
        service.course = "";
        service.courseName = "";
        service.title = "";
        service.content = "";     
        service.applicationUser = "";
        service.type = "";
        service.recDate = "";
        service.dueDate = "";
        service.getPost = getPost;
        service.postComment = postComment;
        service.comment = [];
        service.username = "";

        service.editComment = editComment;
        service.deleteComment = deleteComment;
    

        
       

        //function that gets the specific assignment and information on it
        function getPost(id) {
            $http.get("/Home/FindPost/" + id).then(function (response) {
               
                service.id = response.data.Id;
                service.course = response.data.Course;
                service.courseName = response.data.Course.Name;
                service.title = response.data.Title;
                service.content = response.data.Content 
                service.applicationUser = response.data.ApplicationUser
                service.type = response.data.Type;
               
                var timeDue = new Date(parseInt(response.data.Due.substring(6)));
                service.dueDate = timeDue;

                var timeRec = new Date(parseInt(response.data.RecDate.substring(6)));
                service.recDate = timeRec;

                var listOfComments = response.data.Comments;
                angular.forEach(listOfComments, function (value, key) {
                    listOfComments[key].DateCreated = new Date(parseInt(value.DateCreated.substring(6)));                 
                });
                service.comment = listOfComments;
                
            }, function (error) {
                console.log(error);
            });
        }

        //function that creates the comment
        function postComment(comment) {
            $http.post("/api/Comments/", comment).then(function (response) {
                service.comment.push(response.data);
                comment.content = "";
            });
        }
     
        //function that edits the comment
        function editComment(comment) {
            $http.put("/api/Comments/" + comment.id, comment).then(function (response) {
                service.comment.push(response.data);
                comment.content = "";
            });
        }

        //function that deletes the comment
        function deleteComment(comment) {
            $http.delete("/api/Comments/" + comment.id).then(function (response) {
                service.comment.pop(response.data);
              
            });
        }

        return service;

    }]);

})();