using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Mausr.Core;
using Mausr.Core.NeuralNet;
using Mausr.Core.Optimization;
using Mausr.Web.Entities;
using Mausr.Web.Models;

namespace Mausr.Web.NeuralNet {
	public class NetDbTrainer {

		private readonly string netId;
		private readonly TrainStorageManager storageManager;
		private readonly Job job;

		private Stopwatch stopwatch = new Stopwatch();


		private TrainSettings trainSettings;
		private Net network;
		private NetEvaluator netEvaluator;

		private Matrix<double>[] unpackedCoefs;

		private TrainData trainData;

		//private List<RawDrawing> trainDrawings;
		private Matrix<double> trainInputs;
		private int[] trainOutNeuronIndices;
		private int[] trainOutIds;

		//private List<RawDrawing> testDrawings;
		private Matrix<double> testInputs;
		private int[] testOutNeuronIndices;
		private int[] testOutIds;

		public NetDbTrainer(string netId, TrainStorageManager tsm, Job job) {
			this.netId = netId;
			this.storageManager = tsm;
			this.job = job;
		}

		public void TrainNetwork() {
			stopwatch.Reset();
			stopwatch.Start();

			sendMessage("Loading training settings.");
			trainSettings = storageManager.LoadTrainSettings(netId);
			if (trainSettings == null) {
				sendMessage("Train settings file was not found.");
				return;
			}

			using (var db = MausrDb.Create()) {
				sendMessage("Initializing learning environment.");
				trainData = new TrainData();

				int inputSize = trainSettings.InputImgSizePx * trainSettings.InputImgSizePx;
				int outputSize = db.Symbols.Count();

				var layout = new NetLayout(inputSize, trainSettings.HiddenLayersSizes, outputSize);
				network = new Net(layout, new SigomidActivationFunc(), new NetCostFunction());
				network.SetOutputMap(db.Symbols.Select(s => s.SymbolId).ToArray());
				netEvaluator = new NetEvaluator(network);


				var optimizer = createOptimizer(trainSettings);
				var trainer = new NetTrainer(network, optimizer, trainSettings.RegularizationLambda);

				unpackedCoefs = network.Layout.AllocateCoefMatrices();

				sendMessage("Preparing inputs and outputs.");
				prepareInOut(db);

				sendMessage(string.Format("Learning of {0} samples started.", trainInputs.RowCount));
				bool converged = trainer.TrainBatch(trainInputs, trainSettings.BatchSize, trainSettings.LearnRounds,
					trainOutIds, trainSettings.InitSeed, trainSettings.MinDerivCompMaxMagn, trainIterationCallback, job.CancellationToken);

				if (job.CancellationToken.IsCancellationRequested) {
					sendMessage("Training {0}.", job.Canceled ? "canceled" : "stopped");
				}
				else {
					sendMessage("Training done ({0}converged).", converged ? "" : "not ");
				}

				if (!job.Canceled) {
					sendMessage("Saving trained data.");
					if (!storageManager.SaveNet(netId, network)) {
						sendMessage("Failed to save the network.");
					}

					if (!storageManager.SaveTrainData(netId, trainData)) {
						sendMessage("Failed to save training data.");
					}

					sendMessage("Saving results visualization.");
					if (!createAndSaveResultsVis(3, 0.01, 40)) {
						sendMessage("Failed visualize results.");
					}
				}

				stopwatch.Stop();
			}

			sendMessage("All done.");
		}

		private IGradientBasedOptimizer createOptimizer(TrainSettings trainSettings) {
			switch (trainSettings.OptimizationAlgorithm) {
				case OptimizationAlgorithm.BasicGradientDescent:
					return new SteepestDescentBasicOptmizer(trainSettings.LearningRate,
						trainSettings.MomentumStartPerc / 100.0, trainSettings.MomentumEndPerc / 100.0,
						trainSettings.MaxIteratinosPerBatch);

				case OptimizationAlgorithm.NesterovSutskeverGradientDescent:
					return new SteepestDescentAdvancedOptmizer(trainSettings.LearningRate,
						trainSettings.MomentumStartPerc / 100.0, trainSettings.MomentumEndPerc / 100.0,
						trainSettings.MaxIteratinosPerBatch);

				case OptimizationAlgorithm.RpropPlus:
					return new RpropPlusOptmizer(trainSettings.RpropInitStep, trainSettings.RpropMaxStep,
						trainSettings.RpropStepUpMult, trainSettings.RpropStepDownMult,
						trainSettings.MaxIteratinosPerBatch);

				case OptimizationAlgorithm.ImprovedRpropMinus:
				default:
					return new ImprovedRpropMinusOptmizer(trainSettings.RpropInitStep, trainSettings.RpropMaxStep,
						trainSettings.RpropStepUpMult, trainSettings.RpropStepDownMult,
						trainSettings.MaxIteratinosPerBatch);
			}
		}

		private void prepareInOut(MausrDb db) {
			var ic = new NetInputConvertor();
			var dbInputs = db.SymbolDrawings.ToList();

			var baseInputsAndDrawings = dbInputs
				.Select(sd => new { SymbolId = sd.Symbol.SymbolId, RawDrawing = sd.GetRawDrawing() })
				.ToList();

			var inputsAndDrawings = baseInputsAndDrawings.ToList();
			var dataProc = new RawDataProcessor();

			if (trainSettings.GenerateExtraInputsByRotation != 0) {
				inputsAndDrawings.AddRange(baseInputsAndDrawings
					.Select(x => new {
						SymbolId = x.SymbolId,
						RawDrawing = dataProc.Rotate(x.RawDrawing, trainSettings.GenerateExtraInputsByRotation)
					}));
				inputsAndDrawings.AddRange(baseInputsAndDrawings
					.Select(x => new {
						SymbolId = x.SymbolId,
						RawDrawing = dataProc.Rotate(x.RawDrawing, -trainSettings.GenerateExtraInputsByRotation)
					}));
			}

			if (trainSettings.NormalizeInput) {
				foreach (var iad in inputsAndDrawings) {
					dataProc.NormalizeInPlace(iad.RawDrawing);
				}
			}

			ic.Shuffle(inputsAndDrawings);

			var rasterizer = new Rasterizer() {
				ImageSize = trainSettings.InputImgSizePx,
				DrawPoints = false,
				ExtraMargin = true,
				PenSizePerc = trainSettings.PenThicknessPerc / 100f
			};

			int testSamplesCount = 1 + (trainSettings.TestDataSetSizePerc * inputsAndDrawings.Count / 100);
			{
				var testSet = inputsAndDrawings.Take(testSamplesCount).ToList();
				var testDrawings = testSet.Select(x => x.RawDrawing).ToList();

				testInputs = ic.CreateInputsMatrix(testDrawings, rasterizer);
				testOutIds = testSet.Select(x => x.SymbolId).ToArray();
				testOutNeuronIndices = ic.CreateOutIndicesFromIds(testOutIds, network);
			}
			{
				var trainSet = inputsAndDrawings.Skip(testSamplesCount).ToList();
				var trainDrawings = trainSet.Select(x => x.RawDrawing).ToList();

				trainInputs = ic.CreateInputsMatrix(trainDrawings, rasterizer);
				trainOutIds = trainSet.Select(x => x.SymbolId).ToArray();
				trainOutNeuronIndices = ic.CreateOutIndicesFromIds(trainOutIds, network);
			}
		}

		private bool createAndSaveResultsVis(int predsCount, double minActivation, int imgCols) {
			bool result = true;
			using (var trainImg = createResultsVis(trainInputs, predsCount, minActivation, trainOutIds, imgCols)) {
				result &= storageManager.SaveTrainResultsImg(netId, trainImg);
			}

			using (var testImg = createResultsVis(testInputs, predsCount, minActivation, testOutIds, imgCols)) {
				result &= storageManager.SaveTestResultsImg(netId, testImg);
			}

			return result;
		}

		private Bitmap createResultsVis(Matrix<double> inputs, int predsCount, double minActivation,
				int[] outIds, int imgCols) {
			var predictions = netEvaluator.PredictTopN(inputs, predsCount, minActivation);

			// Draw images directly from net input to be able to visually debug rasterization.
			var visData = predictions.Zip(outIds, (preds, correctOutId) => {
				int index = 0;
				int outId = -1;
				double outVal = 0;

				foreach (var p in preds) {
					if (p.OutputId == correctOutId) {
						outId = p.OutputId;
						outVal = p.NeuronOutputValue;
						break;
					}
					index += 1;
				}

				Color c;
				if (outId < 0) {
					c = Color.LightPink;
				}
				else if (index == 0) {
					c = Color.White;
				}
				else {
					c = Color.Yellow;
				}

				return new {
					Color = c,
					OutVal = outVal,
				};
			}).Zip(inputs.EnumerateRows(), (data, row) => {
				return new Tuple<Vector<double>, Color, string>(row, data.Color, ((int)Math.Round(data.OutVal * 100)).ToString());
			}).ToList();

			return new Rasterizer().DrawDataToGrid(visData, trainSettings.InputImgSizePx, Color.Gray, imgCols);
		}

		private void trainIterationCallback(int iteration, Func<Vector<double>> pointFunc) {
			if (iteration < trainSettings.SkipFrstIters || iteration % trainSettings.TrainEvaluationIters != 0) {
				return;
			}

			var point = pointFunc();
			trainData.IteraionNumbers.Add(iteration);
			point.UnpackTo(unpackedCoefs);

			double trainCost = network.CostFunction.Evaluate(unpackedCoefs,
				trainInputs, trainOutNeuronIndices, trainSettings.RegularizationLambda);
			trainData.TrainCosts.Add((float)trainCost);

			double testCost = network.CostFunction.Evaluate(unpackedCoefs,
				testInputs, testOutNeuronIndices, trainSettings.RegularizationLambda);
			trainData.TestCosts.Add((float)testCost);

			var trainPredictions = netEvaluator.Predict(trainInputs, unpackedCoefs);
			int correctTrainPredicts = trainPredictions.Zip(trainOutIds, (actual, expected) => actual == expected ? 1 : 0).Sum();
			float trainPredict = (float)correctTrainPredicts / trainOutIds.Length;
			trainData.TrainPredicts.Add(trainPredict);

			var testPredictions = netEvaluator.Predict(testInputs, unpackedCoefs);
			int correctTestPredicts = testPredictions.Zip(testOutIds, (actual, expected) => actual == expected ? 1 : 0).Sum();
			float testPredict = (float)correctTestPredicts / testOutIds.Length;
			trainData.TestPredicts.Add(testPredict);

			// Notify client.
			job.Clients.iteration(iteration, trainCost, testCost, trainPredict, testPredict);
		}


		private void sendMessage(string msg, params object[] args) {
			string timestamp = string.Format(CultureInfo.InvariantCulture, "[{0:0000.0}] ", stopwatch.Elapsed.TotalSeconds);
			job.Clients.message(timestamp + string.Format(msg, args));
		}




	}
}