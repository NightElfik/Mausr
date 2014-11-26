using System;
using System.Linq;
using Mausr.Core.NeuralNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mausr.Tests {
	[TestClass]
	public class SigomidActivationFuncTests {
		[TestMethod]
		public void SigomidDerivativeTest() {
			FunctionDerivativeTester.PerformDerivativeTest(new SigomidActivationFunc(),
				Enumerable.Range(-100, 200).Select(x => new double[] { x / 10.0 }).ToArray());
		}
	}
}
