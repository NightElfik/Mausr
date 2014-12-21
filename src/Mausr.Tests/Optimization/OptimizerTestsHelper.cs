using System;
using System.Threading;
using MathNet.Numerics.LinearAlgebra.Double;
using Mausr.Core.Optimization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mausr.Tests.Optimization {
	public static class OptimizerTestsHelper {

		public static void PerformOptimizerTestOnSinCosCrazyFunction(IGradientBasedOptimizer optimizer) {

			const double initialX = -0.35;
			const double initialY = -2;
			const double expectedX = 1.0996537;
			const double expectedY = -2.85224318;
			const double epsilon = 1e-5;

			var f = new SinCosCrazyFunction();
			
			var actual = new DenseVector(2);
			actual[0] = initialX;
			actual[1] = initialY;

			bool converged = optimizer.Optimize(actual, f, epsilon, null, CancellationToken.None);

			Assert.IsTrue(Math.Abs(actual[0] - expectedX) < epsilon);
			Assert.IsTrue(Math.Abs(actual[1] - expectedY) < epsilon);

			Assert.IsTrue(converged);
		}

	}
}
