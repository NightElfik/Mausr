using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using MathNet.Numerics.LinearAlgebra;
using Mausr.Core.Optimization;

namespace Mausr.Core.NeuralNet {
	/// <summary>
	/// Neural network trainer using back-propagation algorithm.
	/// </summary>
	public class NetTrainer : NetEvaluator {

		public IGradientBasedOptimizer Optimizer { get; private set; }
		public double RegularizationLambda { get; private set; }


		public NetTrainer(Net network, IGradientBasedOptimizer optimizer, double regularizationLambda)
			: base(network) {

			Optimizer = optimizer;
			RegularizationLambda = regularizationLambda;
		}


		/// <summary>
		/// Trains the network from scratch by initializing coefficients and optimizing const function.
		/// Returns true if optimization converged.
		/// </summary>
		/// <param name="inputs">Matrix with data in rows.</param>
		/// <param name="outputIds">Array of output IDs should be activated for each input.
		///		Net transfer function is used to transform given IDs to neuron indices</param>
		public bool Train(Matrix<double> inputs, int[] outputIds, double minDerivCompMaxMagn, int initSeeed,
				Action<int, Func<Vector<double>>> iterationCallback, CancellationToken ct) {
			InitializeCoefs(Net, initSeeed);
			return TrainMore(inputs, outputIds, minDerivCompMaxMagn, iterationCallback, ct);
		}

		/// <summary>
		/// Trains the network more by taking current coefficients as starting point.
		/// </summary>
		public bool TrainMore(Matrix<double> inputs, int[] outputIds, double minDerivCompMaxMagn,
				Action<int, Func<Vector<double>>> iterationCallback, CancellationToken ct) {
			Contract.Requires(inputs.ColumnCount == Net.Layout.InputSize);

			int[] outIndices = outputIds.Select(x => Net.MapOutputToOutNeuron(x)).ToArray();
			Net.CostFunction.SetInputsOutputs(inputs, outIndices, RegularizationLambda);
			var result = Net.Coefficients.Pack();

			bool status = Optimizer.Optimize(result, Net.CostFunction, minDerivCompMaxMagn, iterationCallback, ct);

			result.UnpackTo(Net.Coefficients);

			return status;
		}


		public bool TrainBatch(Matrix<double> inputs, int batchSize, int learnRounds, int[] outputIds, int initSeed,
				double minDerivCompMaxMagn, Action<int, Func<Vector<double>>> iterationCallback, CancellationToken ct) {

			bool status = false;
			InitializeCoefs(Net, initSeed);

			if (batchSize == 0) {
				for (int lr = 0; lr < learnRounds; ++lr) {
					status = TrainMore(inputs, outputIds, minDerivCompMaxMagn, iterationCallback, ct);
				}
				return status;
			}


			int[] outIndices = outputIds.Select(x => Net.MapOutputToOutNeuron(x)).ToArray();
			int batchesConut = (inputs.RowCount + batchSize - 1) / batchSize;
			var inputBatches = new Matrix<double>[batchSize];
			var outIndicesBatches = new int[batchSize][];

			var result = Net.Coefficients.Pack();

			// Cut inputs to batches.
			for (int i = 0; i < batchesConut; ++i) {
				int srcIndex = i * batchSize;
				int samples = i == batchesConut - 1 ? inputs.RowCount - srcIndex : batchSize;
				inputBatches[i] = inputs.SubMatrix(i, samples, 0, inputs.ColumnCount);
				outIndicesBatches[i] = new int[samples];
				Array.Copy(outIndices, srcIndex, outIndicesBatches[i], 0, samples);
			}


			for (int lr = 0; lr < learnRounds; ++lr) {
				for (int i = 0; i < batchesConut; ++i) {
					Net.CostFunction.SetInputsOutputs(inputBatches[i], outIndicesBatches[i], RegularizationLambda);
					status = Optimizer.Optimize(result, Net.CostFunction, minDerivCompMaxMagn, iterationCallback, ct);
				}
			}

			result.UnpackTo(Net.Coefficients);

			return status;
		}

		public void InitializeCoefs(Net network, int seed) {
			Contract.Requires(network.Coefficients != null);
			Contract.Requires(Contract.ForAll(network.Coefficients, c => c != null));

			var rand = new Random(seed);
			foreach (var coef in Net.Coefficients) {
				double epsilon = Math.Sqrt(6) / Math.Sqrt(coef.ColumnCount + coef.RowCount);
				double twoEps = 2 * epsilon;

				// Do not use MapInplace since it is parallel and Random class is not thread safe.

				for (int r = 0; r < coef.RowCount; ++r) {
					for (int c = 0; c < coef.ColumnCount; ++c) {
						coef[r, c] = rand.NextDouble() * twoEps - epsilon;
					}
				}
			}
		}

	}
}
