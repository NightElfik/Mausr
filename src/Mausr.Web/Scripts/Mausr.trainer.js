"use strict";

function MausrTrainer(options) {
	var self = this;

	this.netId = options.netId;
	this.$msgsContainer = $('#' + options.msgsContainerId);

	this.startUrl = options.startUrl;
	this.$startBtn = $('#' + options.startBtnId);
	this.$startLabel = $('#' + options.startLabelId);

	this.stopUrl = options.stopUrl;
	this.$stopBtn = $('#' + options.stopBtnId);
	this.$stopLabel = $('#' + options.stopLabelId);


	this.hubProxy = $.connection.progressHub;
	this.hubProxy.logging = true;

	this.$startBtn.click(function (e) {
		self.$startLabel.text("Sending start request ...");
		$.ajax({
			url: self.startUrl,
			method: 'POST',
			data: { id: self.netId },
			success: function (data) {
				self.$startLabel.text(data.message);
			}
		});
	});

	this.$stopBtn.click(function (e) {
		self.$stopLabel.text("Sending stop request ...");
		$.ajax({
			url: self.stopUrl,
			method: 'POST',
			data: { id: self.netId },
			success: function (data) {
				self.$stopLabel.text(data.message);
			}
		});
	});

	this.hubProxy.client.progressChanged = function (progress) {
		self.message('Progress: ' + (progress * 100) + ' %');
	};

	this.hubProxy.client.jobCompleted = function () {
		self.message('Completed!', 'success');
		//$.connection.hub.stop();
	};

	this.hubProxy.client.iteration = function (iterationId, trainCost, testCost, trainPredict, testPredict) {
		self.message('i = ' + iterationId
			+ ' trainCost = ' + trainCost
			+ ' testCost = ' + testCost
			+ ' trainPredict = ' + trainPredict
			+ ' testPredict = ' + testPredict);
	};

	self.message('[Client] Connecting to the server.');
	$.connection.hub.start().done(function () {
		self.message('[Client] Connected.');
		self.hubProxy.server.trackJob(self.netId).done(function (running) {
			if (running) {
				self.message('[Client] Server job is currently running.');
			}
			else {
				self.message('[Client] Server job is currently not running.');
			}
		});
	});

	this.hubProxy.client.message = function (message) {
		self.message(message);
	};

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
	this.$msgsContainer.prepend(msg);
};

MausrTrainer.prototype.stop = function () {
	this.message('Client stopped.', 'info');
};