using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Mausr.Core.Optimization {
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


		public bool Optimize(DenseVector result, IFunctionWithDerivative function, DenseVector initialPosition) {
			Contract.Requires(result.Count == function.DimensionsCount);
			Contract.Requires(initialPosition.Count == function.DimensionsCount);

			var derivative = new DenseVector(function.DimensionsCount);
			var prevStep = new DenseVector(function.DimensionsCount);

			initialPosition.CopyTo(result);

			for (int i = 0; i < maxIters; ++i) {
				if (performStep(result, i, function, result, derivative, prevStep)) {
					return true;
				}
			}

			LastIterationsCount = maxIters;
			return false;
		}

		public bool Optimize(List<DenseVector> steps, IFunctionWithDerivative function, DenseVector initialPosition) {
			Contract.Requires<ArgumentNullException>(steps != null);
			Contract.Requires(initialPosition.Count == function.DimensionsCount);

			var derivative = new DenseVector(function.DimensionsCount);
			var prevStep = new DenseVector(function.DimensionsCount);
			var lastPoint = new DenseVector(function.DimensionsCount);
			initialPosition.CopyTo(lastPoint);
			steps.Add(lastPoint);

			for (int i = 0; i < maxIters; ++i) {
				var newPoint = new DenseVector(function.DimensionsCount);
				steps.Add(newPoint);
				if (performStep(newPoint, i, function, lastPoint, derivative, prevStep)) {
					return true;
				}
				lastPoint = newPoint;
			}

			LastIterationsCount = maxIters;
			return false;
		}


		private bool performStep(DenseVector result, int iteration, IFunctionWithDerivative function, DenseVector point,
				DenseVector derivative, DenseVector prevStep) {

			bool converged = false;

			double momentum = momentumStart + (momentumEnd - momentumStart) * ((double)iteration / maxIters);
			prevStep.Multiply(momentum, prevStep);
			prevStep.Add(point, result);
			function.Derivate(derivative, result);

			if (derivative.SumMagnitudes() < minDerivativeMagnitude) {
				converged = true;
			}

			derivative.Multiply(step, derivative);
			result.Subtract(derivative, result);	
		
			result.Subtract(point, prevStep);

			LastIterationsCount = iteration;
			return converged;
		}

	}
}
