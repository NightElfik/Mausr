using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Mausr.Core.Optimization;

namespace Mausr.Core.NeuralNet {
	public class NetCostFunction : IFunctionWithDerivative {

		private Matrix<double> inputs;
		private int[] outputIndices;

		private Func<double, double> neuronActivateFunction;
		private NetLayout netLayout;
		private Matrix<double>[] unpackedCoefs;
		private double regularizationLambda;


		public int DimensionsCount { get; private set; }


		public NetCostFunction(Net network, Matrix<double> inputs, int[] outputIndices, double regularizationLambda) {

			neuronActivateFunction = network.NeuronActivationFunc.Get();
			netLayout = network.Layout;
			this.inputs = inputs;
			this.outputIndices = outputIndices;
			this.regularizationLambda = regularizationLambda;

			unpackedCoefs = new Matrix<double>[netLayout.CoefsCount];
			DimensionsCount = 0;
			for (int i = 0; i < netLayout.CoefsCount; ++i) {
				DimensionsCount += netLayout.GetLayerSize(i) * netLayout.GetLayerSize(i + 1);
				unpackedCoefs[i] = new DenseMatrix(netLayout.GetCoefMatrixRows(i), netLayout.GetCoefMatrixCols(i));
			}

		}


		public double Evaluate(Vector<double> point) {
			point.Unpack(unpackedCoefs);

			Matrix<double> result = inputs;
			var activations = new Matrix<double>[netLayout.CoefsCount];

			// Forward pass.
			for (int i = 0; i < netLayout.CoefsCount; ++i) {
				result = result.InsertColumn(0, DenseVector.Create(result.RowCount, 1));
				result *= unpackedCoefs[i];
				result.Map(neuronActivateFunction, result);
				activations[i] = result;
			}

			// Cost function.
			double value = 0;
			int samplesCount = inputs.RowCount;
			var resultValues = activations[netLayout.CoefsCount - 1];
			Contract.Assert(resultValues.ColumnCount == netLayout.OutputSize);
			var rowNegCostSums = resultValues.FoldByRow((acc, val) => acc + Math.Log(1 - val), 0.0);

			for (int i = 0; i < samplesCount; ++i) {
				double response = resultValues[i, outputIndices[i]];
				// Math.Log(x) of the one that supposed to be activated and Math.Log(1 - x) of all others.
				// That's Math.Log(x) of the one and + sum(Math.Log(1 - x)) of all minus Math.Log(1 - x) of
				// the acctvated that should have been ommited in the sum.
				value -= Math.Log(response) + rowNegCostSums[i] - Math.Log(1 - response);
			}

			value /= samplesCount;

			// Regularization - sum of all coefs except those for bias units (the first row).
			double regSum = 0;
			for (int i = 0; i < netLayout.CoefsCount; ++i) {
				// Sum all, then subtract the first row.
				var powM = unpackedCoefs[i].PointwisePower(2);
				regSum += powM.ColumnSums().Sum() - powM.Row(0).Sum();				
			}

			value += regSum * regularizationLambda / (2 * samplesCount);
			return value;
		}

		public void Derivate(Vector<double> resultDerivativeResult, Vector<double> point) {
			throw new NotImplementedException();
		}


	}
}
