﻿using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using Mausr.Core;
using Mausr.Core.NeuralNet;
using Mausr.Web.Infrastructure;
using Mausr.Web.Models;

namespace Mausr.Web.NeuralNet {
	public class CurrentEvaluator {

		private readonly TrainStorageManager trainStorageManager;


		public string NetId { get; private set; }

		public NetEvaluator Evaluator { get; private set; }
		public TrainSettings TrainSettings { get; private set; }

		private RawDataProcessor dataProcessor = new RawDataProcessor();
		private Rasterizer rasterizer = new Rasterizer();
		private NetInputConvertor inputConvertor = new NetInputConvertor();


		public CurrentEvaluator(TrainStorageManager tsm) {
			trainStorageManager = tsm;
			NetId = trainStorageManager.LoadDefaultNetName();
			if (NetId != null) {
				var net = trainStorageManager.LoadNet(NetId);
				var settings = trainStorageManager.LoadTrainSettings(NetId);
				if (net != null && settings != null) {
					Evaluator = new NetEvaluator(net);
					TrainSettings = settings;
				}
			}
		}

		
		public bool SetDefaultNetwork(string netId) {
			var net = trainStorageManager.LoadNet(netId);
			var settings = trainStorageManager.LoadTrainSettings(NetId);
			if (net == null || settings == null) {
				return false;
			}

			NetId = netId;
			Evaluator = new NetEvaluator(net);
			TrainSettings = settings;
			return trainStorageManager.SaveDefaultNetName(netId);
		}

		public IEnumerable<Prediction> PredictTopN(RawDrawing drawing, int predictionsCount, double minActivation) {
			if (Evaluator == null) {
				return null;
			}

			dataProcessor.NormalizeInPlace(drawing);
			var img = rasterizer.Rasterize(drawing, TrainSettings.InputImgSizePx, TrainSettings.PenThicknessPerc / 100f, true, false);
			var input = inputConvertor.ImageToVector(img);
			return Evaluator.PredictTopN(input, predictionsCount, minActivation);
		}

	}
}