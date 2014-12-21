"use strict";

function MausrPainter(options) {
	var self = this;

	self.predictUrl = options.predictUrl;
	self.autoPredictDelay = options.autoPredictDelay;
	self.$predictResults = $('#' + options.predictResultsId);
	self.$spinner = options.spinnerId ? $('#' + options.spinnerId) : undefined;
	self.$duration = options.durationId ? $('#' + options.durationId) : undefined;
	self.predictTimeout = undefined;

	self.$mainCanvas = $('#' + options.canvasId);
	self.$jsonText = options.jsonTextId ? $('#' + options.jsonTextId) : undefined;
	self.$clearBtn = $('#' + options.clearBtnId);
	self.$replayCanvas = options.replayCanvasId ? $('#' + options.replayCanvasId) : undefined;
	self.$replayBtn = options.replayBtnId ? $('#' + options.replayBtnId) : undefined;
	self.$drawnUsingTouchInput = options.drawnUsingTouchId ? $('#' + options.drawnUsingTouchId) : undefined;
	if (self.$drawnUsingTouchInput) {
		self.$drawnUsingTouchInput.val('False');
	}

	self.context = self.$mainCanvas[0].getContext('2d');
	self.initContext(self.context);

	self.drawnUsingTouch = false;
	self.guid = self.generateGuid();
	self.drawing = false;

	self.replayTimeout = undefined;

	self.currentRefTime;
	self.currentLine = [];
	self.allLines = [];


	self.$mainCanvas.mousedown(function (e) {
		var coords = self.getMousePos(this, e);
		self.paintStart(coords.x, coords.y);
		return false;
	});

	self.$mainCanvas.mousemove(function (e) {
		var coords = self.getMousePos(this, e);
		self.paintContinue(coords.x, coords.y);
		return false;
	});

	self.$mainCanvas.mouseup(function (e) {
		var coords = self.getMousePos(this, e);
		self.paintEnd(coords.x, coords.y);
		return false;
	});

	self.$mainCanvas.mouseleave(function (e) {
		var coords = self.getMousePos(this, e);
		self.paintEnd(coords.x, coords.y);
		return false;
	});

	self.$mainCanvas.bind('touchstart', function (e) {
		e.preventDefault();
		self.drawnUsingTouch = true;
		if (self.$drawnUsingTouchInput) {
			self.$drawnUsingTouchInput.val('True');
		}
		var coords = self.getTouchPos(this, e.originalEvent);
		self.paintStart(coords.x, coords.y);
		return false;
	});

	self.$mainCanvas.bind('touchmove', function (e) {
		e.preventDefault();
		var coords = self.getTouchPos(this, e.originalEvent);
		self.paintContinue(coords.x, coords.y);
		return false;
	});

	self.$mainCanvas.bind('touchend', function (e) {
		e.preventDefault();
		self.paintEnd();
		return false;
	});

	self.$clearBtn.click(function () {
		self.stopReplay();
		self.drawing = false;
		self.drawnUsingTouch = false;
		self.guid = self.generateGuid();
		if (self.$drawnUsingTouchInput) {
			self.$drawnUsingTouchInput.val('False');
		}
		if (self.$duration) {
			self.$duration.hide();
		}
		self.currentLine = [];
		self.allLines = [];
		self.redraw();
		if (self.$jsonText) {
			self.$jsonText.text("");
		}
		self.$predictResults.empty();
		return false;
	});

	if (self.$replayBtn) {
		this.$replayBtn.click(function () {
			self.$replayCanvas.show();
			self.replay(self.$replayCanvas[0], self.allLines);
			return false;
		});
	}
};

MausrPainter.MIN_SAMPLE_DIST_SQ = 4 * 4;



MausrPainter.prototype.getMousePos = function (canvas, e) {
	//var rect = canvas.getBoundingClientRect();
	var pos = $(canvas).offset();
	return {
		x: e.clientX - pos.left,
		y: e.clientY - pos.top
	};
};

MausrPainter.prototype.getTouchPos = function (canvas, e) {
	var rect = canvas.getBoundingClientRect();
	return {
		x: e.touches[0].clientX - rect.left,
		y: e.touches[0].clientY - rect.top
	};
};

MausrPainter.prototype.initContext = function (context) {
	context.strokeStyle = '#000000';
	context.lineJoin = 'round';
	context.lineCap = 'round';
	context.lineWidth = 8;
};

MausrPainter.prototype.now = performance.now
	? function () { return Math.round(performance.now()); }
	: function () { return Math.round(new Date().getTime()); }

MausrPainter.prototype.startLine = function (x, y) {
	var self = this;

	if (self.allLines.length == 0) {
		self.currentRefTime = self.now();
	}
	self.currentLine = [];
	self.allLines.push(self.currentLine);
	self.addPtToCurrLine(x, y);
};

MausrPainter.prototype.addPtToCurrLine = function (x, y) {
	var self = this;
	var offset = 0;
	var wid = self.context.canvas.width;
	var hei = self.context.canvas.height;

	// Check if this point is the same as the last one and if it is then add offset
	// to make it a line.
	if (self.currentLine.length > 0) {
		var lastPoint = self.currentLine[self.currentLine.length - 1];
		var lastX = lastPoint.x * wid;
		var lastY = lastPoint.y * hei;
		if (Math.abs(x - lastX) < 1 && Math.abs(y - lastY) < 1) {
			offset = 0.5;
		}
	}

	this.currentLine.push({
		x: (x + offset) / wid,
		y: y / hei,
		t: self.now() - self.currentRefTime
	});
};

MausrPainter.prototype.distSqToLastPoint = function (x, y) {
	var self = this;
	assert(self.currentLine.length > 0);

	var lastPt = self.currentLine[self.currentLine.length - 1];
	var dx = x - lastPt.x * self.context.canvas.width;
	var dy = y - lastPt.y * self.context.canvas.height;

	return dx * dx + dy * dy;
}

MausrPainter.prototype.paintStart = function (x, y) {
	var self = this;
	if (self.drawing) {
		return;
	}

	self.drawing = true;

	self.startLine(x, y);
	self.redraw();
};

MausrPainter.prototype.paintContinue = function (x, y) {
	var self = this;
	if (!self.drawing) {
		return;
	}

	var distSq = self.distSqToLastPoint(x, y);
	if (distSq < MausrPainter.MIN_SAMPLE_DIST_SQ) {
		return;
	}

	self.addPtToCurrLine(x, y);
	self.redraw();
};

MausrPainter.prototype.paintEnd = function (x, y) {
	var self = this;
	if (!self.drawing) {
		return;
	}

	self.drawing = false;

	if (x && y) {
		self.addPtToCurrLine(x, y);
	}
	self.redraw();
	self.exportText();
	if (self.predictUrl) {
		self.startPredictTimeout();
	}
};

MausrPainter.prototype.redraw = function () {
	var self = this;
	// Clears the canvas
	var width = self.context.canvas.width;
	var height = self.context.canvas.height;
	self.context.clearRect(0, 0, width, height);

	if (self.allLines.length > 0) {
		self.context.beginPath();

		for (var l = 0; l < self.allLines.length; l++) {
			var line = self.allLines[l];
			assert(line.length > 0);

			self.context.moveTo(line[0].x * width, line[0].y * height);
			if (line.length == 1) {
				// Line just started.
				self.context.lineTo(line[0].x * width + 0.5, line[0].y * height);
			}
			else {
				for (var i = 1; i < line.length; i += 1) {
					var pt = line[i];
					self.context.lineTo(pt.x * width, pt.y * height);
				}
			}
		}

		self.context.stroke();
	}
};

MausrPainter.prototype.exportText = function () {
	var self = this;
	if (self.$jsonText) {
		self.$jsonText.text(JSON.stringify(self.allLines));
	}
};


MausrPainter.prototype.replay = function (canvas, lines) {
	var self = this;

	self.stopReplay();
	var context = canvas.getContext("2d");
	self.initContext(context);
	self.currentRefTime = self.now();

	var width = context.canvas.width;
	var height = context.canvas.height;
	context.clearRect(0, 0, width, height);

	if (lines.length == 0) {
		return;
	}

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

		self.replayTimeout = setTimeout(step, dt);
	};

	step();
};

MausrPainter.prototype.stopReplay = function () {
	var self = this;
	if (self.replayTimeout) {
		clearTimeout(self.replayTimeout);
		self.replayTimeout = undefined;
	}
};

MausrPainter.prototype.startPredictTimeout = function () {
	var self = this;
	if (self.predictTimeout) {
		clearTimeout(self.predictTimeout);
	}
	self.predictTimeout = setTimeout(function () { self.predict(); }, self.autoPredictDelay);
};

MausrPainter.prototype.predict = function () {
	var self = this;
	var linesData = JSON.stringify(self.allLines);
	self.$spinner.show();
	$.ajax({
		url: self.predictUrl,
		method: 'POST',
		data: {
			JsonData: linesData,
			DrawnUsingTouch: self.drawnUsingTouch ? 'True' : 'False',
			Guid: self.guid
		},
		success: function (data) {
			self.$spinner.hide();
			self.$predictResults.empty();
			var results = data.Results;
			if (results.length == 0) {
				self.$predictResults.append($('<p>No results.</p>'));
			}
			else {
				for (var i = 0; i < results.length; ++i) {
					self.showResult(results[i]);
				}
			}
			self.$duration.text("Query took " + Math.round(data.Duration) + " ms.");
			self.$duration.show();
		},
		error: function () {
			self.$spinner.hide();
		}
	});
};

MausrPainter.prototype.showResult = function (result) {
	var self = this;

	var entity = result.HtmlEntity.length == 0 ? '' : '<code>&amp;' + result.HtmlEntity + ';</code> or ';

	self.$predictResults.append($('<li />')
		.append($('<div class="cont" />')
			.append($('<h2>' + result.Symbol + '</h3>'))
			.append($('<p>' + result.SymbolName + '</p>'))
			.append($('<p>HTML: ' + entity + '<code>&amp;#' + result.UtfCode + ';</code></p>'))
			.append($('<p>UTF: <code>U+' + result.UtfCode.toString(16) + '</code></p>'))
			.append($('<p class="conf">Confidence: ' + (Math.round(result.Rating * 1000) / 10) + '%</p>')))
	);
};

MausrPainter.prototype.generateGuid = function () {
	var d = Date.now();
	return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
		var r = (d + Math.random() * 16) % 16 | 0;
		d = Math.floor(d / 16);
		return (c == 'x' ? r : (r & 0x3 | 0x8)).toString(16);
	});
};