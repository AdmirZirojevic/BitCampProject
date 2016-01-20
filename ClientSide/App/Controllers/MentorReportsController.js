(function () {

    var mod = angular.module("MentorReportsMod", []);


    mod.controller("mentorReportController", ["mentorReportsService2", "$stateParams", function (mentorReportsService2, $stateParams) {
        var ctrl = this;
        
        ctrl.service = mentorReportsService2;
        ctrl.getActiveSurveys = getActiveSurveys;
        ctrl.survey = null;
        ctrl.getSurveyQuestions = getSurveyQuestions;
        ctrl.postQuestionResponse = postQuestionResponse;
        ctrl.createMentorReport = createMentorReport;

        ctrl.ifFormValid = false;  //variable which is used in validation (true/false)

        ctrl.StudentSurveyComb = StudentSurveyComb;

        //survey id
        ctrl.data = {
            repeatSelect: null,
            availableOptions: ctrl.service.surveys
        }
       
       
        ctrl.questionResponse = {                   
            Answer: "",
            StudentId: $stateParams.id
        }

        ctrl.mentorReport = {
            StudentId: $stateParams.id
        }

        //function call from the injected service
        function StudentSurveyComb(surveyId,studentId)
        {
            studentId = $stateParams.id;
           
            ctrl.service.StudentSurveyComb(surveyId, studentId)
        }

        //function call from the injected service
        function getActiveSurveys() {
            ctrl.service.getActiveSurveys();
        }

        //function call from the injected service
        function getSurveyQuestions() {
                   
            ctrl.service.getSurveyQuestions();
        }

         //function call from the injected service
        function postQuestionResponse($index, surveyId,questionId)
        {               

            ctrl.service.surveyQuestions[$index].questionResponse.StudentId = $stateParams.id
            ctrl.service.surveyQuestions[$index].questionResponse.SurveyId = surveyId;
            ctrl.service.surveyQuestions[$index].questionResponse.QuestionId = questionId;

            if (ctrl.service.surveyQuestions[$index].questionResponse.Answer != null || ctrl.service.surveyQuestions[$index].questionResponse.Answer != "") {
                ctrl.ifFormValid = true;
                
            }

            if (ctrl.service.StudentSurveyCombResponse.SurveyId == surveyId && ctrl.service.StudentSurveyCombResponse.StudentId == $stateParams.id)               
                return;

            ctrl.service.postQuestionResponse($index);
        }

         //function call from the injected service
        function createMentorReport(surveyId) {
               
            ctrl.mentorReport.SurveyId = surveyId;

            if (ctrl.ifFormValid == false)
            {
                bootbox.alert("Please answer all of the questions");
                return;
            }

            if (ctrl.service.StudentSurveyCombResponse.SurveyId == surveyId && ctrl.service.StudentSurveyCombResponse.StudentId == $stateParams.id) {
                bootbox.alert("You already submitted this report!");              
                return;
            }

            ctrl.service.createMentorReport(ctrl.mentorReport);
            bootbox.alert("Report submitted!");
        }
    }]);


})();
