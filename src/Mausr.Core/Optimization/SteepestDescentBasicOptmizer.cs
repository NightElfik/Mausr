using System;
using System.Diagnostics.Contracts;
using System.Threading;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Mausr.Core.Optimization {
	public class SteepestDescentBasicOptmizer : IGradientBasedOptimizer {

		private double step;
		private double momentumStart;
		private double momentumEnd;
		private int maxIters;


		public SteepestDescentBasicOptmizer(double step, int maxIters) {
			Contract.Requires(step > 0);
			Contract.Requires(maxIters > 0);

			this.step = step;
			this.maxIters = maxIters;
		}

		public SteepestDescentBasicOptmizer(double step, double momentumStart, double momentumEnd, int maxIters) {
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
			
			for (int iter = 0; iter < maxIters; ++iter) {
				if (ct.IsCancellationRequested) {
					break;
				}

				function.Derivate(derivative, resultAndInitPos);
				if (derivative.AbsoluteMaximum() < minDerivCompMaxMagn) {
					return true;
				}

				derivative.Multiply(step, derivative);

				double momentum = momentumStart + (momentumEnd - momentumStart) * ((double)iter / maxIters);
				prevStep.Multiply(momentum, prevStep);
				prevStep.Add(derivative, prevStep);
				resultAndInitPos.Subtract(prevStep, resultAndInitPos);					

				if (iterationCallback != null) {
					iterationCallback(iter, resultAndInitPos.Clone);
				}
			}

			return false;
		}

	}
}
