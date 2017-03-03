var app = angular.module('jolt-net-app', ['ngRoute', 'hljs']);

app.config(function($routeProvider) {
	$routeProvider
		.when('/', {
			templateUrl: 'partials/home.html',
			controller: 'PageCtrl'
		})
		.when('/test-center', {
			templateUrl: 'partials/testcenter.html',
			controller: 'PageCtrl'
		})
		.when('/doc/gameclient', {
			templateUrl: 'partials/doc/gameclient.html',
			controller: 'PageCtrl'
		})
		.when('/doc/session-manager', {
			templateUrl: 'partials/doc/sessionmanager.html',
			controller: 'PageCtrl'
		})
		.when('/doc/trophy', {
			templateUrl: 'partials/doc/trophy.html',
			controller: 'PageCtrl'
		})
		.when('/doc/data-storage', {
			templateUrl: 'partials/doc/datastorage.html',
			controller: 'PageCtrl'
		})
		.when('/doc/score', {
			templateUrl: 'partials/doc/score.html',
			controller: 'PageCtrl'
		})
		.otherwise('/404', {
			templateUrl: '404.html',
			controller: 'PageCtrl'
		})
});

app.config(function (hljsServiceProvider) {
	hljsServiceProvider.setOptions({
		// replace tab with 2 spaces
		tabReplace: '    '
	});
});

app.controller('PageCtrl', function ($scope, $timeout) {});
app.controller('HeaderController', function ($scope, $location) 
{ 
    $scope.isActive = function (viewLocation) { 
        return viewLocation === $location.path();
    };
})