using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Mausr.Core.Optimization {
	/// <summary>
	/// The Rosenbrock function is a non-convex function used as a performance test problem for optimization algorithms.
	/// It is defined as f(x, y) = (a - x)^2 + b * (y - x^2)^2.
	/// http://en.wikipedia.org/wiki/Rosenbrock_function
	/// </summary>
	public class RosenbrockFunction : IFunctionWithDerivative {

		public int DimensionsCount { get { return 2; } }

		private double a, b;

		/// <summary>
		/// The Rosenbrock function is defined as f(x, y) = (a - x)^2 + b * (y - x^2)^2.
		/// </summary>
		public RosenbrockFunction(double a = 1, double b = 100) {
			this.a = a;
			this.b = b;
		}

		public double Evaluate(DenseVector point) {
			Contract.Requires(point.Count == DimensionsCount);
			double dx = a - point[0];
			double dxy = point[1] - point[0] * point[0];
			return dx * dx + b * dxy * dxy;
		}

		public void Derivate(DenseVector resultDerivativeResult, DenseVector point) {
			Contract.Requires(resultDerivativeResult.Count == DimensionsCount);
			Contract.Requires(point.Count == DimensionsCount);
			double x = point[0];
			double y = point[1];
			resultDerivativeResult[0] = (2 * x) * ((2 * b * x * x) - (2 * b * y) + 1) - (2 * a);
			resultDerivativeResult[1] = 2 * b * (y - x * x);
		}

	}
}
