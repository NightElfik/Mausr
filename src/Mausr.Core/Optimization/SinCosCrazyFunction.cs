using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Mausr.Core.Optimization {
	/// <summary>
	/// Taken from http://en.wikipedia.org/wiki/Gradient_descent.
	/// It is defined as f(x, y) = sin(1/2 * x^2 - 1/4 * y^2 + 3) * cos(2 * x + 1 - e^y).
	/// http://en.wikipedia.org/wiki/Rosenbrock_function
	/// </summary>
	public class SinCosCrazyFunction : IFunctionWithDerivative {

		public int DimensionsCount { get { return 2; } }

		public double Evaluate(DenseVector point) {
			Contract.Requires(point.Count == DimensionsCount);
			double x = point[0];
			double y = point[1];
			return Math.Sin(0.5 * x * x - 0.25 * y * y + 3) * Math.Cos(2 * x + 1 - Math.Exp(y));
		}

		public void Derivate(DenseVector resultDerivativeResult, DenseVector point) {
			Contract.Requires(resultDerivativeResult.Count == DimensionsCount);
			Contract.Requires(point.Count == DimensionsCount);
			double x = point[0];
			double y = point[1];
			double expy = Math.Exp(y);
			double a = 2 * x + 1 - expy;
			double b = 0.5 * x * x - 0.25 * y * y + 3;
			resultDerivativeResult[0] = x * Math.Cos(a) * Math.Cos(b)
				- 2 * Math.Sin(a) * Math.Sin(b);
			resultDerivativeResult[1] = expy * Math.Sin(a) * Math.Sin(b) 
				- 0.5 * y * Math.Cos(a) * Math.Cos(b);
		}

	}
}
