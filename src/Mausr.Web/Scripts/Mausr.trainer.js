"use strict";

function MausrTrainer(options) {
	var self = this;

	self.netId = options.netId;
	self.$msgsContainer = $('#' + options.msgsContainerId);

	self.costChartId = options.costChartId;
	self.predictChartId = options.predictChartId;

	self.startUrl = options.startUrl;
	self.$startBtn = $('#' + options.startBtnId);
	self.$startLabel = $('#' + options.startLabelId);

	self.stopUrl = options.stopUrl;
	self.$stopBtn = $('#' + options.stopBtnId);

	self.setAsDefaultUrl = options.setAsDefaultUrl;
	self.$setAsDefaultBtn = $('#' + options.setAsDefaultBtnId);


	self.hubProxy = $.connection.progressHub;
	self.hubProxy.logging = true;

	self.costChart = undefined;
	self.costChartData = undefined;
	self.costChartOptions = {
		width: 500,
		height: 563,
		hAxis: {
			title: 'Iterations'
		},
		vAxis: {
			title: 'Cost (log scale)',
			logScale: true
		},
	};

	self.predictChart = undefined;
	self.predictChartData = undefined;
	self.predictChartOptions = {
		width: 500,
		height: 563,
		hAxis: {
			title: 'Iterations'
		},
		vAxis: {
			title: 'Predict success'
		},
	};

	google.load('visualization', '1', { packages: ['corechart'] });
	google.setOnLoadCallback(function () { self.initChart(); });

	self.$startBtn.click(function (e) {
		self.$startLabel.text("Sending start request ...");
		$.ajax({
			url: self.startUrl,
			method: 'POST',
			data: { id: self.netId },
			success: function (data) {
				self.$startLabel.text(data.message);
			}
		});
		return false;
	});

	self.$stopBtn.click(function (e) {
		self.$startLabel.text("Sending stop request ...");
		$.ajax({
			url: self.stopUrl,
			method: 'POST',
			data: { id: self.netId },
			success: function (data) {
				self.$startLabel.text(data.message);
			}
		});
		return false;
	});

	self.$setAsDefaultBtn.click(function (e) {
		self.$startLabel.text("Sending request ...");
		$.ajax({
			url: self.setAsDefaultUrl,
			method: 'POST',
			data: { id: self.netId },
			success: function (data) {
				self.$startLabel.text(data.message);
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
		self.addChartsPoints(iterationId, trainCost, testCost, trainPredict, testPredict);
		self.redrawCharts();
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

	//self.hub.client.fail = function (message) {
	//	self.message(message, 'danger');
	//	self.stop();
	//};

	//self.hub.client.done = function (message) {
	//	self.message(message, 'success');
	//	self.stop();
	//};

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

MausrTrainer.prototype.initChart = function () {
	var self = this;

	self.costChart = new google.visualization.LineChart(document.getElementById(self.costChartId));
	self.predictChart = new google.visualization.LineChart(document.getElementById(self.predictChartId));

	self.costChartData = new google.visualization.DataTable();
	self.costChartData.addColumn('number', 'X');
	self.costChartData.addColumn('number', 'Train');
	self.costChartData.addColumn('number', 'Test');

	self.predictChartData = new google.visualization.DataTable();
	self.predictChartData.addColumn('number', 'X');
	self.predictChartData.addColumn('number', 'Train');
	self.predictChartData.addColumn('number', 'Test');
};


MausrTrainer.prototype.addChartsPoints = function (i, trainCost, testCost, trainPredict, testPredict) {
	var self = this;

	self.costChartData.addRows([[i, trainCost, testCost]]);
	self.predictChartData.addRows([[i, trainPredict, testPredict]]);
};

MausrTrainer.prototype.redrawCharts = function () {
	var self = this;

	self.costChart.draw(self.costChartData, self.costChartOptions);
	self.predictChart.draw(self.predictChartData, self.predictChartOptions);
};