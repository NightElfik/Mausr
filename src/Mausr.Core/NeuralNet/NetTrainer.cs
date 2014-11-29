using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
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


		/// <param name="inputs">Matrix with data in rows.</param>
		/// <param name="outputIndices">Array of indices of output neurons that should be activated for each input.</param>
		public bool Train(Matrix<double> inputs, int[] outputIndices) {
			Contract.Requires(inputs.ColumnCount == Net.Layout.InputSize);

			var costFunction = new NetCostFunction(Net, inputs, outputIndices, RegularizationLambda);

			InitializeCoefs(Net);
			var point = Net.Coefficients.Pack();
			var result = new DenseVector(point.Count);

			bool status = Optimizer.Optimize(result, costFunction, point);

			result.UnpackTo(Net.Coefficients);

			return status;
		}
	
		public bool Train(List<Vector<double>> optimizationSteps, Matrix<double> inputs, int[] outputIndices) {
			Contract.Requires(inputs.ColumnCount == Net.Layout.InputSize);

			var costFunction = new NetCostFunction(Net, inputs, outputIndices, RegularizationLambda);

			InitializeCoefs(Net);
			var point = Net.Coefficients.Pack();

			bool status = Optimizer.Optimize(optimizationSteps, costFunction, point);

			optimizationSteps[optimizationSteps.Count - 1].UnpackTo(Net.Coefficients);

			return status;
		}

		public void InitializeCoefs(Net network) {
			Contract.Requires(network.Coefficients != null);
			Contract.Requires(Contract.ForAll(network.Coefficients, c => c != null));

			var rand = new Random();
			foreach (var coef in Net.Coefficients) {
				double epsilon = Math.Sqrt(6) / Math.Sqrt(coef.ColumnCount + coef.RowCount);
				double twoEps = 2 * epsilon;
				coef.MapInplace(x => rand.NextDouble() * twoEps - epsilon);
			}
		}

	}
}
