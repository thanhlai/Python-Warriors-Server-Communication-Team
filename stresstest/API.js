app.factory('API', function(Restangular, SERVICES) {
	return {
		// username, password, email
		register: function(userObj) {
			return Restangular.one(SERVICES.register + 'player').post(userObj);
		},

		// username, password
		login: function(loginObj) {
			return Restangular.one(SERVICES.login + 'login').post(loginObj);
		}
	}
});