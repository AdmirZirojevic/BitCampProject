(function () {

    var app = angular.module("bitClassroomApp", [
        'ui.router',
        'jcs-autoValidate',
        'mgcrea.ngStrap',
        'angularSpinner',
        'smart-table',
        'ngFileUpload',
        'RegisterLoginMod',
        'CourseFeedMod',
        'IndividualAssignmentMod',
        'StudentTableMod',
        'ProfileMod',
        'NotificationMod',
        'SearchMod',
        'CreateAssignmentMod',
        'StartDoneAssignmentMod',
        'AnnouncementCommentMod',
        'PostsViewMod',
        'ExploreCoursesMod',
        'MentorInfoMod',
        'PaypalMod',
        'MentorReportsMod',
        'CalendarMod']);
        


    app.run([
    'defaultErrorMessageResolver',
    function (defaultErrorMessageResolver) {
        // passing a culture into getErrorMessages('fr-fr') will get the culture specific messages
        // otherwise the current default culture is returned.
        defaultErrorMessageResolver.getErrorMessages().then(function (errorMessages) {
            errorMessages['namePattern'] = "Can only contain letters, minimum one capitalized";
        });
    }]);

    //////////////////////////////////////////////////////////////////////

    app.factory('httpRequestInterceptor', [function () {
        return {
            request: function (config) {
                var token = sessionStorage.getItem('tokenKey');
                if (token) {
                    config.headers['Authorization'] = 'Bearer ' + token;
                }
                return config;
            }
        };
    }]);

    app.config(function ($httpProvider, $sceProvider) {
        $httpProvider.interceptors.push('httpRequestInterceptor');
        $sceProvider.enabled(false);
    });

    /////////////////////////////////////////////////////////////

    app.config(function ($stateProvider, $urlRouterProvider) {

        $urlRouterProvider.otherwise("/");

        $stateProvider.state("calendar", {
            url: "/Calendar/:id",
            views: {
                "viewCalendar": {
                    templateUrl: "/App/Templates/Calendar.html",
                    controller: "calendarController",
                    controllerAs: "ctrl"
                }
            }
        }).state("paymentCanceled", {
            url: "/paymentCanceled",
            views: {
                "paymentCancel": {
                    templateUrl: "/App/Templates/PaymentCanceled.html",
                    controller: "paymentCanceledController",
                    controllerAs: "ctrl"
                }
            }
        }).state("paymentConfirmation", {
            url: "/paymentConfirmation",
            views: {
                "paymentConfirmed": {
                    templateUrl: "/App/Templates/PaymentConfirmed.html",
                    controller: "paymentConfirmedController",
                    controllerAs: "ctrl"
                }
            }
        }).state("mentorInfo", {
            url: "/MentorInfo",
            views: {
                "mentorInfoProfile": {
                    templateUrl: "/App/Templates/MentorInfo.html",
                    controller: "mentorInfoController",
                    controllerAs: "ctrl"
                }
            }
        }).state("exploreCourses", {
            url: "/ExploreCourses",
            views: {
                "courseList": {
                    templateUrl: "/App/Templates/ExploreCourses.html",
                    controller: "exploreCoursesController",
                    controllerAs: "ctrl"
                }
            }
        }).state("assignments", {
            url: "/Assignments",
            views: {
                "startDone": {
                    templateUrl: "/App/Templates/StartDoneAssignment.html",
                    controller: "startDoneAssignmentController",
                    controllerAs: "ctrl"
                }
            }
        }).state("createAssignment", {
            url: "/CreateAssignment/:name/:id",
            views: {
                "createForm":{
                templateUrl: "/App/Templates/CreateAssignment.html",
                controller: "createAssignmentController",
                controllerAs: "ctrl"
                }, "studentInfo": {
                    templateUrl: "/App/Templates/StudentInfo.html",
                    controller: "studentTableController",
                    controllerAs: "ctrl"
                }
            }            
        }).state("search", {
            url: "/search/",
            views: {
                "searchTable": {
                templateUrl: "/App/Templates/SearchResults.html",
                controller: "searchController",
                controllerAs: "ctrl"
                }
            }
        }).state("courseFeed", {
            url: "/courseFeed/:id/:name",
            views: {
                "feed":{
            templateUrl: "/App/Templates/CourseFeed.html",
            controller: "courseFeedController",
                controllerAs:"ctrl"
                }
            }
        }).state("individualAssignments", {
            url: "/IndividualAssignments",
            views: {
                "individual":{
            templateUrl: "/App/Templates/IndividualAssignments.html",
            controller: "individualAssignmentsController",
            controllerAs: "ctrl"
                }
            }
        }).state("individualAssignmentsMent", {
            url: "/IndividualAssignmentsMentor",
            views: {
                "individual":{
            templateUrl: "/App/Templates/IndividualAssignmentsMentor.html",
            controller: "individualAssignmentsController",
            controllerAs: "ctrl"
                }
            }
        }).state("studentTable", {
            url: "/StudentTable/:name/:id",
            views: {
                "studentInfo": {
                    templateUrl: "/App/Templates/StudentInfo.html",
                    controller: "studentTableController",
                    controllerAs: "ctrl"
                },
                "studentTable":{
                    templateUrl: "/App/Templates/StudentTable.html",
                    controller: "studentTableController",
                    controllerAs: "ctrl"
                }
            }
        }).state("comment", {
            url: "/Assignment/:id/",
            views:{
                "commentAssignment":{
            templateUrl: "/App/Templates/PostsComments.html",
            controller: "postsViewController",
                    controllerAs: "ctrl"
                }
            }
        }).state("comment2", {
            url: "/Announcement/:id/",
            views:{
                "commentAnnouncement":{
            templateUrl: "/App/Templates/AnnouncementComments.html",
            controller: "announcementCommentController",
            controllerAs: "ctrl"
                }
            }            
        }).state("mentorReport", {
            url: "/MentorReports/:name/:id",
            views: {
                "createMentorReport": {
                    templateUrl: "/App/Templates/MentorReports.html",
                    controller: "mentorReportController",
                    controllerAs: "ctrl"
                }, "studentInfo": {
                    templateUrl: "/App/Templates/StudentInfo.html",
                    controller: "studentTableController",
                    controllerAs: "ctrl"
                }
            }

        });
    });


})();

