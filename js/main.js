var app = angular.module('jolt-net-app', ['ngRoute', 'hljs']);

app.config(function($routeProvider) {
	$routeProvider
		.when('/', {
			templateUrl: 'partials/home.html',
			controller: 'PageCtrl'
		})
		.when('/doc/gameclient', {
			templateUrl: 'partials/doc/gameclient.html',
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