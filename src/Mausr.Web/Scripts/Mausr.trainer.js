"use strict";

function MausrTrainer(options) {
	var self = this;

	this.$msgsContainerId = $('#' + options.msgsContainerId);
	this.netId = options.netId;

	this.hubProxy = $.connection.progressHub;
	this.hubProxy.logging = true;

	this.hubProxy.client.progressChanged = function (jobId, progress) {
		self.message('Progress: ' + progress + ' %');
	};

	this.hubProxy.client.jobCompleted = function (jobId) {
		self.message('Completed!', 'success');
		$.connection.hubProxy.stop();
	};

	self.message('[Client] Connecting to the server.');
	$.connection.hub.start().done(function () {
		self.message('[Client] Connected.');
		self.hubProxy.server.trackJob(self.netId);
	});

	//this.hub.client.message = function (message) {
	//	self.message(message);
	//};

	//this.hub.client.fail = function (message) {
	//	self.message(message, 'danger');
	//	self.stop();
	//};

	//this.hub.client.done = function (message) {
	//	self.message(message, 'success');
	//	self.stop();
	//};

};

MausrTrainer.prototype.message = function (message, type) {
	var msg = $('<p />').text(message);
	if (type) {
		msg.addClass('alert').addClass('alert-' + type);
	}
	this.$msgsContainerId.prepend(msg);
};

MausrTrainer.prototype.stop = function () {
	this.message('Client stopped.', 'info');
};