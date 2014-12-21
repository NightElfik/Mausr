using System;
using Mausr.Core.Optimization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mausr.Tests.Optimization {
	[TestClass]
	public class SteepestDescentAdvancedOptmizerTests {
		[TestMethod]
		public void OptimizeNoMomentumTest() {
			var instance = new SteepestDescentAdvancedOptmizer(0.05, 0, 0, 256);
			OptimizerTestsHelper.PerformOptimizerTestOnSinCosCrazyFunction(instance);
		}

		[TestMethod]
		public void OptimizeMomentumTest() {
			var instance = new SteepestDescentAdvancedOptmizer(0.05, 0.6, 0.9, 256);
			OptimizerTestsHelper.PerformOptimizerTestOnSinCosCrazyFunction(instance);
		}
	}
}
