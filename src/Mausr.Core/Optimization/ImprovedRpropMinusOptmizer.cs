using System;
using System.Diagnostics.Contracts;
using System.Threading;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Mausr.Core.Optimization {
	public class ImprovedRpropMinusOptmizer : IGradientBasedOptimizer {

		private double initialStep;
		private double maxStep;
		private double stepUpMult;
		private double stepDownMult;
		private int maxIters;


		public ImprovedRpropMinusOptmizer(double initialStep, double maxStep, double stepUpMult, double stepDownMult, int maxIters) {
			Contract.Requires(initialStep > 0);
			Contract.Requires(stepUpMult > 1);
			Contract.Requires(stepDownMult > 0 && stepDownMult < 1);
			Contract.Requires(maxIters > 0);

			this.initialStep = initialStep;
			this.maxStep = maxStep;
			this.stepUpMult = stepUpMult;
			this.stepDownMult = stepDownMult;
			this.maxIters = maxIters;
		}


		public bool Optimize(Vector<double> resultAndInitPos, IFunctionWithDerivative function, double minDerivCompMaxMagn,
				Action<int, Func<Vector<double>>> iterationCallback, CancellationToken ct) {
			Contract.Requires(resultAndInitPos.Count == function.DimensionsCount);
			Contract.Requires(minDerivCompMaxMagn > 0);

			double minStep = minDerivCompMaxMagn / 100;

			int[] prevStepSigns = new int[function.DimensionsCount];
			var stepSize = new DenseVector(function.DimensionsCount);
			var derivative = new DenseVector(function.DimensionsCount);

			for (int i = 0; i < stepSize.Count; ++i) {
				stepSize[i] = initialStep;
			}
						
			for (int iter = 0; iter < maxIters; ++iter) {
				if (ct.IsCancellationRequested) {
					break;
				}

				// Compute current derivative.
				function.Derivate(derivative, resultAndInitPos);
				if (derivative.AbsoluteMaximum() < minDerivCompMaxMagn) {
					return true;
				}


				// Update step sizes and step weights.
				for (int i = 0; i < prevStepSigns.Length; ++i) {
					int currSign = Math.Sign(derivative[i]);
					int sign = currSign * prevStepSigns[i];

					if (sign > 0) {
						stepSize[i] = Math.Min(maxStep, stepSize[i] * stepUpMult);
					}
					else if (sign < 0) {
						stepSize[i] = Math.Max(minStep, stepSize[i] * stepDownMult);
						currSign = 0;
					}
					
					resultAndInitPos[i] -= currSign * stepSize[i];
					prevStepSigns[i] = currSign;
				}

				if (iterationCallback != null) {
					iterationCallback(iter, resultAndInitPos.Clone);
				}
			}

			return false;
		}


	}
}
