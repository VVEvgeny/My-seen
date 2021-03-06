App.config(function($stateProvider) {

    $stateProvider
        .state("webgl",
        {
            url: "/tests/webgl/",
            templateUrl: "Content/Angular/templates/tests/webgl.html",
            controller: "WebGLController",
            reloadOnSearch: false
        });
});

var TestWebGLRun = false;

App.controller("WebGLController",
[
    "$scope", "$rootScope", "$state", "$stateParams", "$http", "$location", "Constants", "$anchorScroll",
    function($scope, $rootScope, $state, $stateParams, $http, $location, constants, $anchorScroll) {

        $anchorScroll();
        $rootScope.pageId = constants.PageIds.TestWebGL;

        TestWebGLRun = true;
        requestAnimationFrame(render);

        $scope.$on("$destroy",
            function() {

                TestWebGLRun = false;

            });

    }
]);