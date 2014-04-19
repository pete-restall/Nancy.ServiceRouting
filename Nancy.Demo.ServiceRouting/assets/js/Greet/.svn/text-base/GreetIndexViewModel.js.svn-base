var Nancy = Nancy || {};
Nancy.Demo = Nancy.Demo || {};
Nancy.Demo.ServiceRouting = Nancy.Demo.ServiceRouting || {};

(function (exports, $, ko, undefined) {
	var greetingContentClass = 'greeting';
	var greetingContentErrorClass = 'error';

	exports.GreetingIndexViewModel = function() {
		this.name = ko.observable('');
		this.requestType = ko.observable('json');
		this.greetingContent = ko.observable('');
		this.greetingContentClass = ko.observable(greetingContentClass);
	};

	exports.GreetingIndexViewModel.prototype.greet = function() {
		var vm = this;
		$.ajax({
			method: 'PUT',
			url: '/greet',
			dataType: vm.requestType(),
			data: {
				Name: vm.name()
			},
			error: function(xhr, status, message) {
				vm.greetingContentClass(greetingContentErrorClass);
				vm.greetingContent(message);
			},
			success: function(data) {
				vm.greetingContentClass(greetingContentClass);
				vm.greetingContent(vm.requestType() == 'json'? data.text: data);
			}
		});
	};
})(Nancy.Demo.ServiceRouting, jQuery, ko, undefined);
