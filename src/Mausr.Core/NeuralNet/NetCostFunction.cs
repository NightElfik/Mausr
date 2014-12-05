using System;
using System.Diagnostics.Contracts;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Mausr.Core.NeuralNet {
	[Serializable]
	public class NetCostFunction : INetCostFunction {
		
		private Net network;

		[NonSerialized]
		private Matrix<double> m_inputs;
		[NonSerialized]
		private int[] m_outputIndices;
		
		[NonSerialized]
		private Matrix<double>[] m_unpackedCoefs;
		[NonSerialized]
		private double m_regularizationLambda;
		
		public int DimensionsCount { get; private set; }


		public void Initialize(Net network) {
			this.network = network;
		}

		public void SetInputsOutputs(Matrix<double> inputs, int[] outputIndices, double regularizationLambda) {			
			m_regularizationLambda = regularizationLambda;
			m_inputs = inputs;
			m_outputIndices = outputIndices;

			m_unpackedCoefs = network.Layout.AllocateCoefMatrices();
			DimensionsCount = m_unpackedCoefs.Sum(x => x.RowCount * x.ColumnCount);
		}


		public double Evaluate(Vector<double> point) {	
			point.UnpackTo(m_unpackedCoefs);
			return Evaluate(m_unpackedCoefs, m_inputs, m_outputIndices, m_regularizationLambda);		
		}

		public void Derivate(Vector<double> resultDerivative, Vector<double> point) {
			point.UnpackTo(m_unpackedCoefs);
			Derivate(resultDerivative, m_unpackedCoefs, m_inputs, m_outputIndices, m_regularizationLambda);
		}


		public double Evaluate(Matrix<double>[] coefs, Matrix<double> inputs, int[] outputIndices, double regularizationLambda) {
			Matrix<double> result = inputs;

			// Forward pass.
			for (int i = 0; i < network.Layout.CoefsCount; ++i) {
				result = result.InsertColumn(0, DenseVector.Create(result.RowCount, 1));
				result = result * coefs[i];
				result.MapInplace(network.NeuronActivationFunc.Evaluate);
			}

			// Cost function.
			double value = 0;
			int samplesCount = inputs.RowCount;
			Contract.Assert(result.ColumnCount == network.Layout.OutputSize);
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
			for (int i = 0; i < network.Layout.CoefsCount; ++i) {
				// Sum all, then subtract the first row.
				var powM = coefs[i].PointwisePower(2);
				regSum += powM.ColumnSums().Sum() - powM.Row(0).Sum();
			}

			value += regSum * regularizationLambda / (2 * samplesCount);
			return value;
		}

		public void Derivate(Vector<double> resultDerivative, Matrix<double>[] coefs,
				Matrix<double> inputs, int[] outputIndices, double regularizationLambda) { 

			Matrix<double> result = inputs;

			// Activation values with extra column of 1s. Matrix on index i is activation of layer i + 1.
			var activations = new Matrix<double>[network.Layout.CoefsCount];

			// Derivations of aftivations. Matrix on index i corresponds to layer i + 1.
			var deltas = new Matrix<double>[network.Layout.CoefsCount];

			// Forward pass - computation of activations.
			for (int i = 0; i < network.Layout.CoefsCount; ++i) {
				// Save activations with extra column full of ones - this will be handy afterwards.
				activations[i] = result.InsertColumn(0, DenseVector.Create(result.RowCount, 1));
				result = activations[i] * coefs[i];
				// Save deltas that will be used later later: δ_i = f'(z_i).
				deltas[i] = result.Map(network.NeuronActivationFunc.Derivate);
				result.MapInplace(network.NeuronActivationFunc.Evaluate);
			}

			// Backward pass - computation of derivatives.

			Contract.Assert(result.ColumnCount == network.Layout.OutputSize);
			int samplesCount = inputs.RowCount;

			//
			// Derivative of i-th neuron in the output layer n_l is:
			// δ_i = ∂E/∂y_i * ∂y_i/∂z_i = -(t_i - y_i) * f'(z_i), where:
			//		z is input value, y is output, and t is target value.
			//
			// ∂E/∂y_i = is derivative of error computed as E := 1/2 * Σ_i(t_i - y_i)^2
			//		The 1/2 won't affect optimization process (and can be even ommited in computation of error function)
			//		but simplifies the derivative.
			//
			// ∂y_i/∂x_i = f'(z_i) is derivative of neuron transfer function and z_i is its input.
			//

			// In our case all targets are vectors with one non-zero: [0, 0, ..., 0, 1, 0, ..., 0].
			// Compute -(t_i - y_i) by subtrancting the 1 where needed.
			// The rest are zeros thus does not need to be subtracted.
			for (int i = 0; i < samplesCount; ++i) {
				result[i, outputIndices[i]] -= 1;  // -(t_i - y_i) = y_i - t_i
			}

			int deltaIndex = network.Layout.CoefsCount - 1;

			// δ_i = f'(z_i) * -(t_i - y_i).
			//deltas[deltaIndex].PointwiseMultiply(result, deltas[deltaIndex]);
			// WTF, I really do not understand why I do not have to include derivative of neuron function here.
			// Is it because derivative of -log(sgm(x)) is -sgm(x) and it cancels out?
			deltas[deltaIndex] = result;

			//
			// Derivative of i-th neuron in non-output layer l is:
			// δ_i^(l) = Σ_{j=1}^{s_{l+1}}(W_ij^(l) * δ_j^(l+1)) * f'(z_i^(l))
			// This can be vectorized as: δ_i^(l) = δ_j^(l+1) * (W^(l))^T .* f'(z_i^(l)).
			//			
			for (deltaIndex -= 1; deltaIndex >= 0; --deltaIndex) {
				var coefsNoBias = coefs[deltaIndex + 1].RemoveRow(0);  // Remove bias coeficients.
				var newDelta = deltas[deltaIndex + 1].TransposeAndMultiply(coefsNoBias);  // δ_j^(l+1) * (W^(l))^T.
				deltas[deltaIndex].PointwiseMultiply(newDelta, deltas[deltaIndex]);  // δ_i^(l) .* f'(z_i^(l).
			}

			//
			// Finally we can compute derivatives from δs.
			// For weights: ∂/∂W_ij^(l) J(W,b,x,y) = a_j^(l) * δ_i^(l + 1).
			// For bias: ∂/∂b_i^(l) J(W,b,x,y) = δ_i^(l + 1).
			// Derivatives has to be averaged for all training samles.
			// The sum is achieved by matrix multiplication and since we have a extra column of 1s
			// in the activation matrix A the bias terms will just work without extra care.
			// D = A^T * δ
			//
			double regPram = regularizationLambda / samplesCount;
			var derivatives = new Matrix<double>[network.Layout.CoefsCount];
			for (int i = 0; i < network.Layout.CoefsCount; ++i) {
				var derivs = activations[i].TransposeThisAndMultiply(deltas[i]);
				derivs.Divide(samplesCount, derivs);
				derivatives[i] = derivs;

				// Regularization - regularize all but bias coeficients.
				var coef = coefs[i];
				Contract.Assert(derivs.RowCount == coef.RowCount);
				Contract.Assert(derivs.ColumnCount == coef.ColumnCount);

				int rows = derivs.RowCount, cols = derivs.ColumnCount;
				for (int r = 1; r < rows; ++r) {
					for (int c = 0; c < cols; ++c) {
						derivs[r, c] += regPram * coef[r, c];
					}
				}
			}

			derivatives.PackTo(resultDerivative);
		}
	}
}
