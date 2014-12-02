var app = angular.module('StressTester', [
	// 'angular-lodash',
	'restangular'
]);

app.constant('SERVICES', {
	server: 'http://techproprojects.bcit.ca:6363/',
	register: 'RegisterService.svc/',
	login: 'LoginService.svc/',
	game: 'GameAPI.svc/gameapi/'
});

app.config(function(RestangularProvider, SERVICES) {
	var baseUrl = SERVICES;
	RestangularProvider.setBaseUrl(baseUrl);
});

app.controller('MainCtrl', function($scope, $http, API) {
	$scope.logins = $scope.logins || [];
	$scope.user = {
		username: "thanh1993",
		password: "thanh1993",
		email: "thanh@thanh.com"
	};

	// API.register($scope.user).then(function(success) {
	// 	console.log(success);
	// }, function(fail) {

	// });
});