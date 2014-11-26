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
		private Func<double, double> neuronActivateFunctionDerivative;
		private NetLayout netLayout;
		private Matrix<double>[] unpackedCoefs;
		private double regularizationLambda;


		public int DimensionsCount { get; private set; }


		public NetCostFunction(Net network, Matrix<double> inputs, int[] outputIndices, double regularizationLambda) {
			Contract.Requires(Contract.ForAll(outputIndices, i => i >= 0 && i < network.Layout.OutputSize));

			neuronActivateFunction = network.NeuronActivationFunc.Evaluate;
			neuronActivateFunctionDerivative = network.NeuronActivationFunc.Derivative;
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

			// Forward pass.
			for (int i = 0; i < netLayout.CoefsCount; ++i) {
				result = result.InsertColumn(0, DenseVector.Create(result.RowCount, 1));
				result *= unpackedCoefs[i];
				result.Map(neuronActivateFunction, result);
			}

			// Cost function.
			double value = 0;
			int samplesCount = inputs.RowCount;
			Contract.Assert(result.ColumnCount == netLayout.OutputSize);
			var rowNegCostSums = result.FoldByRow((acc, val) => acc + Math.Log(1 - val), 0.0);

			for (int i = 0; i < samplesCount; ++i) {
				double response = result[i, outputIndices[i]];
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

		public void Derivate(Vector<double> resultDerivative, Vector<double> point) {
			point.Unpack(unpackedCoefs);

			Matrix<double> result = inputs;
			var activations = new Matrix<double>[netLayout.CoefsCount];
			var deltas = new Matrix<double>[netLayout.CoefsCount];

			// Forward pass - computation of activations.
			for (int i = 0; i < netLayout.CoefsCount; ++i) {
				// Save activations with extra column full of ones - this will be handy afterwards.
				activations[i] = result.InsertColumn(0, DenseVector.Create(result.RowCount, 1));
				result = activations[i] * unpackedCoefs[i];
				// Save deltas that will be used later later: δ_i = f'(z_i).
				deltas[i] = result.Map(neuronActivateFunctionDerivative);
				result.Map(neuronActivateFunction, result);
			}

			// Backward pass - computation of derivatives.
			
			Contract.Assert(result.ColumnCount == netLayout.OutputSize);
			int samplesCount = inputs.RowCount;

			//
			// Derivative of i-th neuron in the output layer is:
			// δ_i = ∂E/∂y_i * ∂y_i/∂x_i = -(t_i - y_i) * f'(z_i)
			//
			// ∂E/∂y_i =  is derivative of error computed as E := 1/2 * Σ_i(t_i - y_i)^2
			//		The 1/2 won't affect optimization process (and can be even ommited in computation of error function)
			//		but simplifies the derivative.
			//
			// ∂y_i/∂x_i = f'(z_i) is derivative of neuron transfer function and z_i is input to.
			//
						
			// Compute -(t_i - y_i) by subtrancting the 1 where needed.
			// The rest are zeros thus does not need to be subtracted.
			for (int i = 0; i < samplesCount; ++i) {
				result[i, outputIndices[i]] -= 1;  // -(t_i - y_i) = y_i - t_i
			}

			// δ_i = f'(z_i) * -(t_i - y_i).
			deltas[netLayout.CoefsCount - 1].PointwiseMultiply(result, deltas[netLayout.CoefsCount - 1]);

			//
			// Derivative of i-th neuron in non-output layer is little more complicated:
			// ∂E/∂y_i = (chain rule) = Σ_{j∈l+1} ∂E/∂y_j * ∂y_j/∂x_j * ∂x_j/∂y_i, where
			//		i is current layer,
			//		j is next layer downstream,
			//		l is current layer, l + 1 is next one down stream,
			//		∂x_j/∂y_i = w_ij
			// ∂E/∂y_i = Σ_{j∈l+1}(w_ji^(l) * δ_j^(l+1)) * f'(z_i^(l)) 	
			//
			
			for (int i = netLayout.CoefsCount - 2; i >= 0; --i) {
				var coefsNoBias = unpackedCoefs[i + 1].RemoveColumn(0);  // Remove bias coeficients.
				var newDelta = deltas[i + 1].TransposeAndMultiply(coefsNoBias);
				deltas[i].PointwiseMultiply(newDelta);
			}

			
			var gradients = new Matrix<double>[netLayout.CoefsCount];
			for (int i = 0; i < netLayout.CoefsCount; ++i) {
				gradients[i] = activations[i].TransposeThisAndMultiply(deltas[i]);
			}

			gradients.PackTo(resultDerivative);
		}


	}
}
