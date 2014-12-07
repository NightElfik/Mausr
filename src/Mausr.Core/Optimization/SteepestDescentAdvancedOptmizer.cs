using System;
using System.Diagnostics.Contracts;
using System.Threading;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Mausr.Core.Optimization {
	/// <summary>
	/// Gradient descent with momentum using Nesterov & Sutskever method.
	/// </summary>
	public class SteepestDescentAdvancedOptmizer : IGradientBasedOptimizer {

		private double step;
		private double minDerivativeMagnitude;
		private double momentumStart;
		private double momentumEnd;
		private int maxIters;

		public int LastIterationsCount { get; private set; }
		
		public SteepestDescentAdvancedOptmizer(double step, double momentumStart, double momentumEnd,
				double minDerivativeMagnitude, int maxIters) {
			this.step = step;
			this.momentumStart = momentumStart;
			this.momentumEnd = momentumEnd;
			this.minDerivativeMagnitude = minDerivativeMagnitude;
			this.maxIters = maxIters;
		}


		public bool Optimize(Vector<double> resultAndInitPosition, IFunctionWithDerivative function,
				Action<Vector<double>> iterationCallback, CancellationToken ct) {
			Contract.Requires(resultAndInitPosition.Count == function.DimensionsCount);

			var derivative = new DenseVector(function.DimensionsCount);
			var prevStep = new DenseVector(function.DimensionsCount);
			var position = new DenseVector(function.DimensionsCount);

			bool converged = false;
			int i = 0;
			for (; i < maxIters; ++i) {
				if (ct.IsCancellationRequested) {
					break;
				}

				resultAndInitPosition.CopyTo(position);

				converged = performStep(resultAndInitPosition, i, function, position, derivative, prevStep);				
				if (iterationCallback != null) {
					iterationCallback(resultAndInitPosition.Clone());
				}

				if (converged) {
					break;
				}
			}

			LastIterationsCount = i;
			return converged;
		}


		private bool performStep(Vector<double> result, int iteration, IFunctionWithDerivative function, Vector<double> point,
				Vector<double> derivative, Vector<double> prevStep) {

			bool converged = false;

			double momentum = momentumStart + (momentumEnd - momentumStart) * ((double)iteration / maxIters);
			// Compute derivative take step to point + momentum * prevStep and compute derivative there.
			prevStep.Multiply(momentum, prevStep);
			prevStep.Add(point, result);
			function.Derivate(derivative, result);

			if (derivative.SumMagnitudes() < minDerivativeMagnitude) {
				converged = true;
			}

			// Scale derivative and subtract from result to take "correction" step.
			derivative.Multiply(step, derivative);
			result.Subtract(derivative, result);	
			
			// Conpute prevStep as (result - point).
			result.Subtract(point, prevStep);
			return converged;
		}

	}
}
