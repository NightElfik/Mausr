"use strict";

function MausrTrainer(options) {
	var self = this;

	self.netId = options.netId;
	self.$msgsContainer = $('#' + options.msgsContainerId);

	self.startUrl = options.startUrl;
	self.$startBtn = $('#' + options.startBtnId);

	self.stopUrl = options.stopUrl;
	self.$stopBtn = $('#' + options.stopBtnId);
	self.$cancelBtn = $('#' + options.cancelBtnId);

	self.setAsDefaultUrl = options.setAsDefaultUrl;
	self.$setAsDefaultBtn = $('#' + options.setAsDefaultBtnId);

	self.hubProxy = $.connection.progressHub;
	self.hubProxy.logging = true;

	self.chartManager = new MausrTrainCharts(options);

	self.$startBtn.click(function (e) {
		self.message("[Client] Sending start request.");
		$.ajax({
			url: self.startUrl,
			method: 'POST',
			data: { id: self.netId },
			success: function (data) {
				self.reset();
				self.message("[Client] Started request sent successfully.");
			}
		});
		return false;
	});

	self.$stopBtn.click(function (e) {
		self.message("[Client] Sending stop request.");
		$.ajax({
			url: self.stopUrl,
			method: 'POST',
			data: { id: self.netId, cancel: "False" },
			success: function (data) {
				self.message("[Client] Stop request sent successfully.");
			}
		});
		return false;
	});

	self.$cancelBtn.click(function (e) {
		self.message("[Client] Sending cancel request.");
		$.ajax({
			url: self.stopUrl,
			method: 'POST',
			data: { id: self.netId, cancel: "True" },
			success: function (data) {
				self.message("[Client] Cancel request sent successfully.");
			}
		});
		return false;
	});

	self.$setAsDefaultBtn.click(function (e) {
		self.message("[Client] Sending se default net request.");
		$.ajax({
			url: self.setAsDefaultUrl,
			method: 'POST',
			data: { id: self.netId },
			success: function (data) {
				self.message("[Client] Set as default request sent successfully.");
			}
		});
		return false;
	});

	self.hubProxy.client.progressChanged = function (progress) {
		self.message('Progress: ' + (progress * 100) + ' %');
	};

	self.hubProxy.client.jobCompleted = function () {
		self.message('Completed!', 'success');
		//$.connection.hub.stop();
	};

	self.hubProxy.client.iteration = function (iterationId, trainCost, testCost, trainPredict, testPredict) {
		self.chartManager.addChartsPoints(iterationId, trainCost, testCost, trainPredict, testPredict);
		self.chartManager.redrawCharts();
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

	self.hubProxy.client.message = function (message) {
		self.message(message);
	};
};

MausrTrainer.prototype.reset = function () {
	var self = this;
	self.$msgsContainer.empty();
	self.chartManager.clear();
};

MausrTrainer.prototype.message = function (message, type) {
	var self = this;
	var msg = $('<p />').text(message);
	if (type) {
		msg.addClass('alert').addClass('alert-' + type);
	}
	self.$msgsContainer.prepend(msg);
};

MausrTrainer.prototype.stop = function () {
	var self = this;
	self.message('Client stopped.', 'info');
};