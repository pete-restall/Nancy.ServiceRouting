var Nancy = Nancy || {};
Nancy.Demo = Nancy.Demo || {};
Nancy.Demo.ServiceRouting = Nancy.Demo.ServiceRouting || {};

(function (exports, $, ko, undefined) {
	exports.AsyncIndexViewModel = function() {
		this.results = ko.observableArray([]);
		this.timerIds = ko.observableArray([]);
		this.selectedTimerId = ko.observable();
	};

	var poorMansGuid = function() {
		return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
			var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
			return v.toString(16);
		});
	};

	var twoDecimals = function(value) {
		return ((value < 10) ? '0' : '') + value;
	};

	var threeDecimals = function(value) {
		return ((value < 10) ? '00' : (value < 100) ? '0' : '') + value;
	};

	exports.AsyncIndexViewModel.prototype.startTimer = function() {
		var vm = this;
		var timerId = poorMansGuid();
		vm.timerIds.push(timerId);

		$.ajax({
			method: 'GET',
			url: '/async/timer/' + timerId + '/start',
			timeout: 2 * 60 * 1000,
			dataType: 'json',
			error: function (xhr, status, message) {
				vm.results.push('[START ERROR] ' + message);
				vm.timerIds.remove(timerId);
			},
			success: function (data) {
				vm.results.push(
					'--> Timer ' + data.id +
					' ran for ' + twoDecimals(data.minutes) + ':' + twoDecimals(data.seconds) + '.' + threeDecimals(data.milliseconds) +
					', using threads ' + data.startedOnThread + ' and ' + data.endedOnThread);

				vm.timerIds.remove(timerId);
			}
		});
	};

	exports.AsyncIndexViewModel.prototype.stopTimer = function() {
		var vm = this;
		var timerId = vm.selectedTimerId();
		if (!timerId)
			return;

		vm.timerIds.remove(timerId);

		$.ajax({
			method: 'GET',
			url: '/async/timer/' + timerId + '/stop',
			dataType: 'json',
			error: function(xhr, status, message) {
				vm.results.push('[STOP ERROR] ' + message);
			},
			success: function() {
				vm.results.push('<-- Stopped timer ' + timerId);
			}
		});
	};
})(Nancy.Demo.ServiceRouting, jQuery, ko, undefined);
