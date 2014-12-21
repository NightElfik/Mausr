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
		private double momentumStart;
		private double momentumEnd;
		private int maxIters;
		

		public SteepestDescentAdvancedOptmizer(double step, double momentumStart, double momentumEnd, int maxIters) {
			Contract.Requires(step > 0);
			Contract.Requires(maxIters > 0);

			this.step = step;
			this.momentumStart = momentumStart;
			this.momentumEnd = momentumEnd;
			this.maxIters = maxIters;
		}


		public bool Optimize(Vector<double> resultAndInitPos, IFunctionWithDerivative function, double minDerivCompMaxMagn,
				Action<int, Func<Vector<double>>> iterationCallback, CancellationToken ct) {
			Contract.Requires(resultAndInitPos.Count == function.DimensionsCount);
			Contract.Requires(minDerivCompMaxMagn > 0);

			var derivative = new DenseVector(function.DimensionsCount);
			var prevStep = new DenseVector(function.DimensionsCount);
			var position = new DenseVector(function.DimensionsCount);
			
			for (int iter = 0; iter < maxIters; ++iter) {
				if (ct.IsCancellationRequested) {
					break;
				}

				resultAndInitPos.CopyTo(position);

				double momentum = momentumStart + (momentumEnd - momentumStart) * ((double)iter / maxIters);
				// Compute derivative take step to point + momentum * prevStep and compute derivative there.
				prevStep.Multiply(momentum, prevStep);
				prevStep.Add(position, resultAndInitPos);
				function.Derivate(derivative, resultAndInitPos);
				if (derivative.AbsoluteMaximum() < minDerivCompMaxMagn) {
					return true;
				}
				
				// Scale derivative and subtract from result to take "correction" step.
				derivative.Multiply(step, derivative);
				resultAndInitPos.Subtract(derivative, resultAndInitPos);

				// Conpute prevStep as (result - point).
				resultAndInitPos.Subtract(position, prevStep);

				if (iterationCallback != null) {
					iterationCallback(iter, resultAndInitPos.Clone);
				}
			}

			return false;
		}

		
	}
}
