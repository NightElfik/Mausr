"use strict";

$('#refreshBtn').val("Refresh (v2)")

function MausrPainter() {
	var self = this;

	this.$mainCanvas = $('#mainCanvas');
	this.$clearBtn = $('#clearCanvasBtn');
	this.$replayCanvas = $('#replayCanvas');
	this.$replayBtn = $('#replayBtn');
	this.$saveBtn = $('#saveBtn');

	this.context = this.$mainCanvas[0].getContext('2d');
	this.initContext(this.context);

	this.drawing = false;

	this.currentRefTime;
	this.currentLine = [];
	this.allLines = [];

	this.$mainCanvas.mousedown(function (e) {
		var coords = self.getMousePos(this, e);
		self.paintStart(coords.x, coords.y);
	});

	this.$mainCanvas.mousemove(function (e) {
		var coords = self.getMousePos(this, e);
		self.paintContinue(coords.x, coords.y);
	});

	this.$mainCanvas.mouseup(function (e) {
		var coords = self.getMousePos(this, e);
		self.paintEnd(coords.x, coords.y);
	});

	this.$mainCanvas.mouseleave(function (e) {
		var coords = self.getMousePos(this, e);
		self.paintEnd(coords.x, coords.y);
	});

	this.$mainCanvas.bind('touchstart', function (e) {
		e.preventDefault();
		console.log(e);
		var coords = self.getTouchPos(this, e.originalEvent);
		self.paintStart(coords.x, coords.y);
	});

	this.$mainCanvas.bind('touchmove', function (e) {
		e.preventDefault();
		var coords = self.getTouchPos(this, e.originalEvent);
		self.paintContinue(coords.x, coords.y);
	});

	this.$mainCanvas.bind('touchend', function (e) {
		e.preventDefault();
		self.paintEnd();
	});

	this.$clearBtn.click(function () {
		self.drawing = false;
		self.currentLine = [];
		self.allLines = [];
		self.redraw();
	});

	this.$replayBtn.click(function () {
		self.replay(self.$replayCanvas[0], self.allLines);
	});

	this.$saveBtn.click(function () {
		var symbolId = $('#symbolSelect').val();
		self.save(symbolId, self.allLines, function (imgIdStr) {
			$('#imageResponse').attr('src', '/SymbolDrawings/256/8/' + imgIdStr + '.png');
		});
	});


};

MausrPainter.prototype.getMousePos = function (canvas, e) {
	var rect = canvas.getBoundingClientRect();
	return {
		x: e.clientX - rect.left,
		y: e.clientY - rect.top
	};
};

MausrPainter.prototype.getTouchPos = function (canvas, e) {
	var rect = canvas.getBoundingClientRect();
	return {
		x: e.touches[0].clientX - rect.left,
		y: e.touches[0].clientY - rect.top
	};
};

MausrPainter.MIN_SAMPLE_DIST_SQ = 4 * 4;

MausrPainter.prototype.initContext = function (context) {
	context.strokeStyle = '#000000';
	context.lineJoin = 'round';
	context.lineCap = 'round';
	context.lineWidth = 8;
};

MausrPainter.prototype.now = function (x) {
	return Math.round(performance.now());
};

MausrPainter.prototype.startLine = function (x, y) {
	if (this.allLines.length == 0) {
		this.currentRefTime = this.now();
	}
	this.currentLine = [];
	this.allLines.push(this.currentLine);
	this.addPtToCurrLine(x, y);
};

MausrPainter.prototype.addPtToCurrLine = function (x, y) {
	this.currentLine.push({
		x: x / this.context.canvas.width,
		y: y / this.context.canvas.height,
		t: this.now() - this.currentRefTime
	});
};

MausrPainter.prototype.distSqToLastPoint = function (x, y) {
	assert(this.currentLine.length > 0);

	var lastPt = this.currentLine[this.currentLine.length - 1];
	var dx = x - lastPt.x;
	var dy = y - lastPt.y;

	return dx * dx + dy * dy;
}

MausrPainter.prototype.paintStart = function (x, y) {
	if (this.drawing) {
		return;
	}

	this.drawing = true;

	this.startLine(x, y);
	this.redraw();
};

MausrPainter.prototype.paintContinue = function (x, y) {
	if (!this.drawing) {
		return;
	}

	var distSq = this.distSqToLastPoint(x, y);
	if (distSq < MausrPainter.MIN_SAMPLE_DIST_SQ) {
		return;
	}

	this.addPtToCurrLine(x, y);
	this.redraw();
};

MausrPainter.prototype.paintEnd = function (x, y) {
	if (!this.drawing) {
		return;
	}

	this.drawing = false;

	if (x && y) {
		this.addPtToCurrLine(x, y);
	}
	this.redraw();
};

MausrPainter.prototype.redraw = function () {
	// Clears the canvas
	var width = this.context.canvas.width;
	var height = this.context.canvas.height;
	this.context.clearRect(0, 0, width, height);

	if (this.allLines.length > 0) {
		this.context.beginPath();

		for (var l = 0; l < this.allLines.length; l++) {
			var line = this.allLines[l];
			assert(line.length > 0);

			this.context.moveTo(line[0].x * width, line[0].y * height);

			for (var i = 1; i < line.length; i += 1) {
				var pt = line[i];
				this.context.lineTo(pt.x * width, pt.y * height);
			}
		}

		this.context.stroke();
	}
};

MausrPainter.prototype.replay = function (canvas, lines) {
	var context = canvas.getContext("2d");
	this.initContext(context);

	var width = context.canvas.width;
	var height = context.canvas.height;
	context.clearRect(0, 0, width, height);

	var lineIndex = 0;
	var coordIndex = 0;
	var time = lines[0][0].t;

	var moveIndex = function () {
		coordIndex += 1;
		if (coordIndex >= lines[lineIndex].length) {
			coordIndex = 0;
			lineIndex += 1;
		}
	};

	var drawCurrState = function () {
		context.clearRect(0, 0, width, height);
		context.beginPath();

		for (var l = 0; l <= lineIndex; l++) {
			var line = lines[l];
			assert(line.length > 0);

			context.moveTo(line[0].x * width, line[0].y * height);

			for (var i = 1; i < line.length; i += 1) {
				if (l == lineIndex && i > coordIndex) {
					break;
				}

				var pt = line[i];
				context.lineTo(pt.x * width, pt.y * height);
			}
		}

		context.stroke();
	};

	var step = function () {
		moveIndex();
		if (lineIndex >= lines.length) {
			return;
		}

		drawCurrState();
		var newT = lines[lineIndex][coordIndex].t;
		var dt = newT - time;
		time = newT;

		setTimeout(step, dt);
	};

	step();
};

MausrPainter.prototype.save = function (symbolId, lines, callback) {
	var json = JSON.stringify(lines);

	$.ajax({
		type: "POST",
		url: "/SymbolDrawings/Save",
		data: { symbolId: symbolId, jsonData: JSON.stringify(lines) }
	})
	.done(callback);
};
