var Nancy = Nancy || {};
Nancy.Demo = Nancy.Demo || {};
Nancy.Demo.ServiceRouting = Nancy.Demo.ServiceRouting || {};

(function (exports, $, ko, undefined) {
	var demoContentClass = 'demoContent';
	var demoContentErrorClass = 'error';

	exports.IndexResponseViewModel = function() {
		this.selectedDemoClass = ko.observable(demoContentClass);
		this.selectedDemoContent = ko.observable('<p>Select a demo from the list above...</p>');
	};

	exports.IndexResponseViewModel.prototype.selectDemoContent = function(href) {
		var vm = this;
		$.ajax({
			url: href,
			dataType: 'html',
			error: function(xhr, status, message) {
				vm.selectedDemoClass(demoContentErrorClass);
				vm.selectedDemoContent(message);
			},
			success: function(data) {
				vm.selectedDemoClass(demoContentClass);
				vm.selectedDemoContent(data);
			}
		});
	};
})(Nancy.Demo.ServiceRouting, jQuery, ko, undefined);
