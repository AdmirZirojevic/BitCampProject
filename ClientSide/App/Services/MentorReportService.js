(function () {

    var mod = angular.module("MentorReportsMod");

    mod.service("mentorReportsService2", ["$http", "$stateParams", "$state", function ($http, $stateParams, $state) {
        var service = this;
        service.surveyTitle = "";
        service.surveys = [];
        service.getActiveSurveys = getActiveSurveys;
        service.surveyQuestions = [];
        service.getSurveyQuestions = getSurveyQuestions;
        service.postQuestionResponse = postQuestionResponse;
        service.createMentorReport = createMentorReport;
        
        service.StudentSurveyComb = StudentSurveyComb;
        service.StudentSurveyCombResponse = "";

        service.registerButton = false;

        service.questionResponses = [];

        getActiveSurveys();

        //function that gets all currently active surveys (or in other words, surveys that are currenty used)
        function getActiveSurveys() {
            $http.get('Helper/GetActiveSurveys/')
            .then(function (response) {
                var surveyList = response.data;
                service.surveys = surveyList;
            }, function (error) {
                console.log(error);
            });
        }

        //function that gets question which are on specific survey
        function getSurveyQuestions(id) {
            $http.get('/Helper/ForSurveyResponses/?surveyId=' + id)
            .then(function (response) {
                console.log(id);
                var questionList = response.data.Result;
                angular.forEach(questionList, function (value, key) {
                    service.surveyQuestions=questionList;
                })
                service.questionResponseQuestionId = response.data.Result.Id;
                
            }, function (error) {
                console.log(error);
            })

        }

         //function that creates the mentor report
        function createMentorReport(mentorReport)
        {
          
            $http.post('/api/MentorReports/', mentorReport)
            .then(function (response) {
                console.log(response);
                $state.go("mentorReport", { name: $stateParams.name, id: $stateParams.id });

                service.surveyQuestions = [];

            }, function (error) {
                console.log(error);
            });
        }

        //function that creates question response (which contains a question and answer to that question)
        function postQuestionResponse($index) {
        
            $http.post('/api/QuestionResponses/', service.surveyQuestions[$index].questionResponse)
            .then(function (response) {
                service.questionResponses.push(response.data);
            }, function (error) {
                console.log(error);
            });
        }



        //function that gets the mentor report based on survey Id and student Id parameters
        function StudentSurveyComb(surveyId,studentId) {
         
            $http.get("/Helper/StudentSurveyCombo?studentId="+studentId+"&surveyId=" + surveyId).then(function (response) {
                service.StudentSurveyCombResponse = response.data;
            });
        }

        return service;

    }]);

})();
