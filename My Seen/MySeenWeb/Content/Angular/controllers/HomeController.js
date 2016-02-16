App.config(function ($stateProvider) {

    $stateProvider
        .state('/', {
            url: '/',
            templateUrl: "Content/Angular/templates/main_pages/home.html",
            controller: 'HomeController',
            reloadOnSearch: false
        });
});

App.controller('HomeController', ['$scope', '$rootScope', '$state', '$stateParams', '$http', '$location', 'Constants',
  function ($scope, $rootScope, $state, $stateParams, $http, $location, constants) {

      //�� ������ ������ ������, ����� ������� �� ��������, ��� ������ ������� ���������
      $rootScope.clearControllers();

  }]);