using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Mausr.Core.Optimization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mausr.Tests.Optimization {
	[TestClass]
	public class NumericDerivatorTests {

		const double COMPUTE_EPSILON = 1e-4;
		const double CHECK_EPSILON = 1e-7;

		[TestMethod]
		public void SinComputeDerivativeTest() {
			performComputeDerivativeTest(Math.Sin, Math.Cos,
				0, 1, 2, 3, 4, 5, 9, 7, 8, 9, 10, Math.PI / 2, Math.PI, 2 * Math.PI, Math.E);
		}

		[TestMethod]
		public void Poly1DComputeDerivativeTest() {
			// (2 * x^2 - 7 * x + 1)' = 4x - 7
			performComputeDerivativeTest(x => ((2 * x) - 7) * x + 1, x => 4 * x - 7,
				Enumerable.Range(-100, 200).Select(x => x / 10.0).ToArray());
		}

		[TestMethod]
		public void Poly3DComputeDerivativeTest() {
			// f = z^3 - x^2 * y^2 + 3 * z * x + 10
			// ∂f/∂x = 3 * z - 2 * x * y^2
			// ∂f/∂y = - 2 * x^2 * y
			// ∂f/∂z = 3 * (x + z^2)
			performComputeDerivativeTest(p => p[2] * p[2] * p[2] - p[0] * p[0] * p[1] * p[1] + 3 * p[2] * p[0] + 10,
				(d, p) => {
					double x = p[0], y = p[1], z = p[2];
					d[0] = 3 * z - 2 * x * y * y;
					d[1] = -2 * x * x * y;
					d[2] = 3 * (x + z * z);
				},
				Enumerable.Range(-10, 20).SelectMany(x =>
					Enumerable.Range(-10, 20).SelectMany(y =>
						Enumerable.Range(-10, 20).Select(z =>
							new double[] { x / 5.0, y / 5.0, z / 5.0 }
						).ToArray()
					).ToArray()
				).ToArray());
		}

		private void performComputeDerivativeTest(Func<double, double> func,
				Func<double, double> derivFunc, params double[] points) {
			performComputeDerivativeTest(p => func(p[0]), (d, p) => d[0] = derivFunc(p[0]),
				points.Select(x => new double[] { x }).ToArray());
		}

		private void performComputeDerivativeTest(Func<Vector<double>, double> func,
				Action<Vector<double>, Vector<double>> derivFunc, params double[][] points) {

			var nd = new NumericDerivator(func, COMPUTE_EPSILON);
			var actual = new DenseVector(points[0].Length);
			var expected = new DenseVector(points[0].Length);

			foreach (var point in points) {
				var pt = new DenseVector(point);
				nd.ComputeDerivative(actual, pt);
				derivFunc(expected, pt);

				for (int i = 0; i < expected.Count; ++i) {
					Assert.IsTrue(Math.Abs(expected[i] - actual[i]) < CHECK_EPSILON);
				}
			}
		}

	}
}
