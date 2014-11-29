using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Mausr.Core.Optimization {
	public class SteepestDescentBasicOptmizer : IGradientBasedOptimizer {

		private double step;
		private double minDerivativeMagnitude;
		private double momentumStart;
		private double momentumEnd;
		private int maxIters;

		public int LastIterationsCount { get; private set; }

		public SteepestDescentBasicOptmizer(double step, double minDerivativeMagnitude, int maxIters) {
			this.step = step;
			this.minDerivativeMagnitude = minDerivativeMagnitude;
			this.maxIters = maxIters;
		}

		public SteepestDescentBasicOptmizer(double step, double momentumStart, double momentumEnd, double minDerivativeMagnitude, int maxIters) {
			this.step = step;
			this.momentumStart = momentumStart;
			this.momentumEnd = momentumEnd;
			this.minDerivativeMagnitude = minDerivativeMagnitude;
			this.maxIters = maxIters;
		}


		public bool Optimize(Vector<double> result, IFunctionWithDerivative function,
				Vector<double> initialPosition, Action<Vector<double>> iterationCallback) {
			Contract.Requires(result.Count == function.DimensionsCount);
			Contract.Requires(initialPosition.Count == function.DimensionsCount);

			var derivative = new DenseVector(function.DimensionsCount);
			var prevStep = new DenseVector(function.DimensionsCount);

			initialPosition.CopyTo(result);

			for (int i = 0; i < maxIters; ++i) {
				bool converged = performStep(result, i, function, result, derivative, prevStep);

				if (iterationCallback != null) {
					iterationCallback(result.Clone());
				}

				if (converged) {
					return true;
				}
			}

			LastIterationsCount = maxIters;
			return false;
		}
		

		private bool performStep(Vector<double> result, int iteration, IFunctionWithDerivative function, Vector<double> point,
				Vector<double> derivative, Vector<double> prevStep) {

			bool converged = false;
			function.Derivate(derivative, point);
			if (derivative.SumMagnitudes() < minDerivativeMagnitude) {
				converged = true;
			}

			derivative.Multiply(step, derivative);

			if (momentumEnd == 0) {
				point.Subtract(derivative, result);
			}
			else {
				double momentum = momentumStart + (momentumEnd - momentumStart) * ((double)iteration / maxIters);
				prevStep.Multiply(momentum, prevStep);
				prevStep.Add(derivative, prevStep);
				point.Subtract(prevStep, result);
			}
			
			LastIterationsCount = iteration;
			return converged;
		}

	}
}
