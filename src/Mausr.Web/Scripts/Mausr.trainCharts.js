"use strict";

function MausrTrainCharts(options) {
	var self = this;
	
	self.costChartId = options.costChartId;
	self.predictChartId = options.predictChartId;

	self.trainData = options.trainData;

	self.costChart = undefined;
	self.costChartData = undefined;
	self.costChartOptions = {
		width: '100%',
		height: 450,
		hAxis: {
			title: 'Iterations'
		},
		vAxis: {
			title: 'Cost (log scale)',
			logScale: true
		},
		chartArea: { left: 78, top: 10, width: '84%', height: '80%' },
		legend: { position: 'bottom' }
	};

	self.predictChart = undefined;
	self.predictChartData = undefined;
	self.predictChartOptions = {
		width: '100%',
		height: 450,
		hAxis: {
			title: 'Iterations'
		},
		vAxis: {
			title: 'Predict success'
		},
		chartArea: { left: 78, top: 10, width: '84%', height: '80%' },
		legend: { position: 'bottom' }
	};

	google.load('visualization', '1', { packages: ['corechart'] });
	google.setOnLoadCallback(function () { self.initChart(); });
};


MausrTrainCharts.prototype.initChart = function () {
	var self = this;

	self.costChart = new google.visualization.LineChart(document.getElementById(self.costChartId));
	self.predictChart = new google.visualization.LineChart(document.getElementById(self.predictChartId));

	self.costChartData = new google.visualization.DataTable();
	self.costChartData.addColumn('number', 'X');
	self.costChartData.addColumn('number', 'Train set');
	self.costChartData.addColumn('number', 'Test set');

	self.predictChartData = new google.visualization.DataTable();
	self.predictChartData.addColumn('number', 'X');
	self.predictChartData.addColumn('number', 'Train set');
	self.predictChartData.addColumn('number', 'Test set');

	if (self.trainData) {
		var ticks = self.trainData.IteraionNumbers;
		var trainCosts = self.trainData.TrainCosts;
		var testCosts = self.trainData.TestCosts;
		var trainPredicts = self.trainData.TrainPredicts;
		var testPredicts = self.trainData.TestPredicts;
		for (var i = 0; i < ticks.length; ++i) {
			self.addChartsPoints(ticks[i], trainCosts[i], testCosts[i], trainPredicts[i], testPredicts[i]);
		}
	}

	self.redrawCharts();
};

MausrTrainCharts.prototype.addChartsPoints = function (i, trainCost, testCost, trainPredict, testPredict) {
	var self = this;

	self.costChartData.addRows([[i, trainCost, testCost]]);
	self.predictChartData.addRows([[i, trainPredict, testPredict]]);
};

MausrTrainCharts.prototype.redrawCharts = function () {
	var self = this;

	self.costChart.draw(self.costChartData, self.costChartOptions);
	self.predictChart.draw(self.predictChartData, self.predictChartOptions);
};

MausrTrainCharts.prototype.clear = function () {
	var self = this;

	self.trainData = undefined;
	self.initChart();
};