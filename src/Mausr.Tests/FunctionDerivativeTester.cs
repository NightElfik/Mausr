using System;
using MathNet.Numerics.LinearAlgebra.Double;
using Mausr.Core.Optimization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mausr.Tests {
	public static class FunctionDerivativeTester {

		const double COMPUTE_EPSILON = 1e-5;
		const double CHECK_EPSILON = 1e-5;


		public static void PerformDerivativeTest(IFunctionWithDerivative func, params double[][] points) {

			var nd = new NumericDerivator(func.Evaluate, COMPUTE_EPSILON);
			var actual = new DenseVector(points[0].Length);
			var expected = new DenseVector(points[0].Length);

			foreach (var point in points) {
				var pt = new DenseVector(point);
				nd.ComputeDerivative(expected, pt);
				func.Derivate(actual, pt);

				for (int i = 0; i < expected.Count; ++i) {
					Assert.IsTrue(Math.Abs(expected[i] - actual[i]) < CHECK_EPSILON);
				}
			}
		}

	}
}
