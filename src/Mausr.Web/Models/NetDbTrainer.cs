using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Mausr.Core;
using Mausr.Core.NeuralNet;
using Mausr.Core.Optimization;
using Mausr.Web.DataContexts;
using Mausr.Web.Infrastructure;

namespace Mausr.Web.Models {
	public class NetDbTrainer {

		private readonly string netId;
		private readonly TrainStorageManager storageManager;
		private readonly Job job;

		private Stopwatch stopwatch = new Stopwatch();


		private TrainSettings trainSettings;
		private Net network;
		private NetEvaluator netEvaluator;

		private Matrix<double>[] unpackedCoefs;

		private int iterationId;
		private int iterationSendInterval = 5;

		private List<float> trainCosts = new List<float>();
		private List<float> testCosts = new List<float>();

		private List<float> trainPredicts = new List<float>();
		private List<float> testPredicts = new List<float>();

		private Matrix<double> trainInputs;
		private int[] trainOutNeuronIndices;
		private int[] trainOutIds;

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
				//fail("Grain settings file was not found.");
				return;
			}

			using (var db = MausrDb.Create()) {
				sendMessage("Initializing learning environment.");
				iterationId = 0;
				int inputSize = trainSettings.InputImgSizePx * trainSettings.InputImgSizePx;
				int outputSize = db.Symbols.Count();

				var layout = new NetLayout(inputSize, trainSettings.HiddenLayersSizes, outputSize);
				network = new Net(layout, new SigomidActivationFunc(), new NetCostFunction());
				network.SetOutputMap(db.Symbols.Select(s => s.SymbolId).ToArray());
				netEvaluator = new NetEvaluator(network);

				var optimizer = new SteepestDescentAdvancedOptmizer(trainSettings.LearningRate,
					trainSettings.MomentumStartPerc / 100.0, trainSettings.MomentumEndPerc / 100.0,
					trainSettings.MinDerivativeMagnitude, trainSettings.MaxIteratinosPerBatch);
				var trainer = new NetTrainer(network, optimizer, trainSettings.RegularizationLambda);

				unpackedCoefs = network.Layout.AllocateCoefMatrices();

				sendMessage("Preparing inputs and outputs.");
				prepareInOut(db);

				sendMessage(string.Format("Learning of {0} samples started.", trainInputs.RowCount));
				trainer.TrainBatch(trainInputs, trainSettings.BatchSize, trainSettings.LearnRounds,
					trainOutIds, trainIterationCallback, job.CancellationToken);

				stopwatch.Stop();
			}
		}

		private void prepareInOut(MausrDb db) {
			var ic = new NetInputConvertor();
			var dbInputs = db.SymbolDrawings.ToList();

			var baseInputsAndDrawings = dbInputs
				.Select(i => new { SymbolId = i.Symbol.SymbolId, RawDrawing = i.RawDrawing })
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
					dataProc.Normalize(iad.RawDrawing);
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

			testInputs = ic.CreateInputsMatrix(inputsAndDrawings.Take(testSamplesCount).Select(x => x.RawDrawing), rasterizer);
			testOutIds = inputsAndDrawings.Take(testSamplesCount).Select(x => x.SymbolId).ToArray();
			testOutNeuronIndices = ic.CreateOutIndicesFromIds(testOutIds, network);

			trainInputs = ic.CreateInputsMatrix(inputsAndDrawings.Skip(testSamplesCount).Select(x => x.RawDrawing), rasterizer);
			trainOutIds = inputsAndDrawings.Skip(testSamplesCount).Select(x => x.SymbolId).ToArray();
			trainOutNeuronIndices = ic.CreateOutIndicesFromIds(trainOutIds, network);
		}

		private void trainIterationCallback(Vector<double> point) {
			iterationId += 1;
			if (iterationId % iterationSendInterval != 0) {
				return;
			}

			point.UnpackTo(unpackedCoefs);

			double trainCost = network.CostFunction.Evaluate(unpackedCoefs,
				trainInputs, trainOutNeuronIndices, trainSettings.RegularizationLambda);
			trainCosts.Add((float)trainCost);

			double testCost = network.CostFunction.Evaluate(unpackedCoefs,
				testInputs, testOutNeuronIndices, trainSettings.RegularizationLambda);
			testCosts.Add((float)testCost);

			var trainPredictions = netEvaluator.Predict(trainInputs, unpackedCoefs);
			int correctTrainPredicts = trainPredictions.Zip(trainOutIds, (actual, expected) => actual == expected ? 1 : 0).Sum();
			float trainPredict = (float)correctTrainPredicts / trainOutIds.Length;
			trainPredicts.Add(trainPredict);

			var testPredictions = netEvaluator.Predict(testInputs, unpackedCoefs);
			int correctTestPredicts = testPredictions.Zip(testOutIds, (actual, expected) => actual == expected ? 1 : 0).Sum();
			float testPredict = (float)correctTestPredicts / testOutIds.Length;
			testPredicts.Add(testPredict);

			// Notify client.
			job.Clients.iteration(iterationId, trainCost, testCost, trainPredict, testPredict);
		}


		private void sendMessage(string msg, params object[] args) {
			string timestamp = string.Format("{0:0000.0} ", stopwatch.Elapsed.TotalSeconds);
			job.Clients.message(timestamp + string.Format(msg, args));
		}




	}
}