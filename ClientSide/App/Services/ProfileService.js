(function () {

    var mod = angular.module("ProfileMod");

    mod.service("profileService", ["$http", "regLogService", function ($http, regLogService) {

        var service = this;
        service.name = "";
        service.profileImg = "";
        service.mentor = "";
        service.courses = [];
        service.students = [];
        service.studentsCourses = [];
        service.hasMentor = false;
        service.hasCourses = false;
        service.hasStudents = false;
        service.hasStudentsCourses = false;
        service.isMentor = false;
        service.isStudent = false;
        service.getProfile = getProfile;
        service.regLogService = regLogService;

        ////Function invoked from controller, create request on server, to get basic profile information(Profile picture, course collection, mentor...)
        function getProfile() {
            $http.get("/Home/UserProfile/").then(function (response) {
                service.name = response.data.Name + " " + response.data.Lastname;
                service.profileImg = response.data.ProfileUrl;                
                

                if (response.data.Mentor!=null) {
                    service.mentor = response.data.Mentor.Name + " " + response.data.Mentor.Lastname;
                    service.hasMentor = true;
                    service.isStudent = true;
                    service.regLogService.isStudent = true;
                }
                if (response.data.Courses!=null) {
                    service.courses = response.data.Courses;
                    service.hasCourses = true;
                    service.regLogService.isStudent = true;
                }
                if (response.data.Students != null) {
                    service.students = response.data.Students;
                    service.isMentor = true;
                    service.hasStudents = true;
                    
                }
                if (response.data.StudentsCourses!=null) {
                    service.studentsCourses = response.data.StudentsCourses;
                    service.hasStudentsCourses = true;
                }


            }, function (error) {
                console.log(error);
            });
        }

    }]);

})();