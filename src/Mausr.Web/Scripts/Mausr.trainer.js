"use strict";

function MausrTrainer(options) {
	var self = this;

	this.netId = options.netId;
	this.$msgsContainer = $('#' + options.msgsContainerId);

	this.costChartId = options.costChartId;
	this.predictChartId = options.predictChartId;

	this.startUrl = options.startUrl;
	this.$startBtn = $('#' + options.startBtnId);
	this.$startLabel = $('#' + options.startLabelId);

	this.stopUrl = options.stopUrl;
	this.$stopBtn = $('#' + options.stopBtnId);
	this.$stopLabel = $('#' + options.stopLabelId);


	this.hubProxy = $.connection.progressHub;
	this.hubProxy.logging = true;

	this.costChart;
	this.costChartData;
	this.costChartOptions = {
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

	this.predictChart;
	this.predictChartData;
	this.predictChartOptions = {
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
		return false;
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
		return false;
	});

	this.hubProxy.client.progressChanged = function (progress) {
		self.message('Progress: ' + (progress * 100) + ' %');
	};

	this.hubProxy.client.jobCompleted = function () {
		self.message('Completed!', 'success');
		//$.connection.hub.stop();
	};

	this.hubProxy.client.iteration = function (iterationId, trainCost, testCost, trainPredict, testPredict) {
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