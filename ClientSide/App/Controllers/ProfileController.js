(function () {

    var mod = angular.module("ProfileMod", []);

    mod.controller("profileController", ["profileService", function (profileService) {
        var ctrl = this;
        ctrl.service = profileService;
        ctrl.getProfile = getProfile;

        //Function call getProfile() function in injected service.
        function getProfile(){
            ctrl.service.getProfile();
        }

    }]);


    mod.controller('UploadController', ['$scope', 'Upload', '$timeout', function ($scope, Upload, $timeout) {
        $scope.uploadPic = function (file) {
            file.upload = Upload.upload({               
                url: '/Helper/CloudUpload',
                data: { file: file},
            });

            file.upload.then(function (response) {
              
                $timeout(function () {
                    file.result = response.data;
                    location.reload();
                });
            }, function (error) {

                if (error.status > 0)
                    $scope.errorMsg = error.status + ': ' + error.data;
            }, function (prog) {
               
                file.progress = Math.min(100, parseInt(100.0 * prog.loaded / prog.total));
            });
        }
    }]);



    mod.directive("profileDir", function () {
        return {
            templateUrl: "/App/Templates/Profile.html",
            controller: "profileController",
            controllerAs:"ctrl"
        }
    });

})();